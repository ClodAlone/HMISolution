using log4net;
using log4net.Appender;
using log4net.Repository.Hierarchy;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConcurrentLog4Net
{
    internal class QueueManager
    {

        private string path;
        private bool append;
        private FileAccess access;
        private FileShare share;

        private Queue syncQueue = Queue.Synchronized(new Queue());

        private bool running = false;
        private Random rnd = new Random();
        private DateTime retryTime = DateTime.MaxValue;

        private static TimeSpan RETRY_MAX_SPAN = TimeSpan.FromMinutes(5);
        private static QueueManager instance;
        private const int MAX_BATCH_SIZE = 100;

        AutoResetEvent eventNewEntry = new AutoResetEvent(false);

        public static QueueManager GetInstance(string path,
                  bool append,
                  FileAccess access,
                  FileShare share)
        {
            if (instance == null)
            {
                instance = new QueueManager(path, append, access, share);
            }
            return instance;
        }
        private QueueManager(
         string path,
         bool append,
         FileAccess access,
         FileShare share)
        {
            this.path = path;
            this.append = append;
            this.access = access;
            this.share = share;
        }
        internal void Enqueue(CachedEntry entry)
        {
            syncQueue.Enqueue(entry);
            while (syncQueue.Count > Properties.Settings.Default.MaxQueueSize)
                syncQueue.Dequeue();
            eventNewEntry.Set();

            if (!running)
            {
                lock (this)
                {
                    running = true;
                    Thread th = new Thread(new ThreadStart(this.Dequeue));
                    th.IsBackground = true;
                    th.Priority = ThreadPriority.Lowest;
                    th.Start();
                }
            }
        }
        private void Dequeue()
        {
            CachedEntry entry = null;
            while (eventNewEntry.WaitOne())
            {
                try
                {
                    var rootAppender = ((Hierarchy)
#if !NET_STANDARD
                        LogManager.GetRepository()
#else
                        LogManager.GetRepository(System.Reflection.Assembly.GetEntryAssembly())
#endif
                        ).Root.Appenders.OfType<FileAppender>().FirstOrDefault();
                    if (rootAppender != null)
                        path = rootAppender.File;

                    long maxFileSize = 0;
                    var rollingFileAppender = ((Hierarchy)
#if !NET_STANDARD
                        LogManager.GetRepository()
#else
                        LogManager.GetRepository(System.Reflection.Assembly.GetEntryAssembly())
#endif
                        ).Root.Appenders.OfType<RollingFileAppender>().FirstOrDefault();
                    if (rollingFileAppender != null)
                        maxFileSize = rollingFileAppender.MaxFileSize;

                    using (FileStream fs = new FileStream(path, FileMode.Append, access, share))
                    {
                        int counter = 0;
                        while (syncQueue.Count > 0)
                        {
                            entry = (CachedEntry)syncQueue.Dequeue();

                            if (entry != null)
                            {
                                Write(entry, fs);
                            }

                            if (maxFileSize > 0)
                            {
                                if (fs.Length >= maxFileSize)
                                    break;
                            }
                            else if (++counter >= Properties.Settings.Default.MaxQueueSize)
                                break;

                            Thread.Sleep(rnd.Next(100));
                        }
                    }

                    Thread.Sleep(rnd.Next(1000));
                }
                catch (IOException ioe)
                {
                    //if (DateTime.Now - retryTime > RETRY_MAX_SPAN)
                    //{
                    //    lock (this)
                    //    {
                    //        running = false;
                    //    }
                    //    // throw;
                    //}
                    //When can't aquire lock
                    //Wait random time then retry
                    Thread.Sleep(rnd.Next(1000));
                    //Console.WriteLine("Retry:" + DateTime.Now);
                    retryTime = DateTime.Now;
                    eventNewEntry.Set();
                }
                catch
                {
                    syncQueue.Clear();
                    Thread.Sleep(rnd.Next(1000));
                }
            }
        }
        private void Write(CachedEntry entry, FileStream fs)
        {
            fs.Write(entry.Buffer, entry.Offset, entry.Count);
            fs.Flush();
        }
    }
}
