using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcurrentLog4Net
{
    public class ConcurrentStream : Stream
    {
        private string path;
        private bool append;
        private FileAccess access;
        private FileShare share;
        private QueueManager queueManager;
        private static ConcurrentStream instance;

        public static ConcurrentStream GetInstance(string path,
         bool append,
         FileAccess access,
         FileShare share)
        {
            if (instance == null)
            {
                instance = new ConcurrentStream(path, append, access, share);
            }
            return instance;
        }
        private ConcurrentStream(
         string path,
         bool append,
         FileAccess access,
         FileShare share
         )
        {
            this.path = path;
            this.append = append;
            this.access = access;
            this.share = share;
            this.queueManager = QueueManager.GetInstance(path, append, access, share);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            CachedEntry entry = new CachedEntry(buffer, offset, count);
            queueManager.Enqueue(entry);
        }

        public override bool CanRead
        {
            get { return false; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override void Flush()
        {
            return;
        }

        public override long Length
        {
            get { return 0L; }
        }

        public override long Position
        {
            get { return 0L; }
            set { ;}
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return 0;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return 0L;
        }

        public override void SetLength(long value)
        { } 
 
    }
}
