using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace XpoHelpers
{
    public class CachedUnitOfWorks : IDisposable
    {
        #region Declarations
        readonly IXpoDocument document;

        readonly Dictionary<Thread, CachedUnitOfWork> cachedUnitOfWorks = new Dictionary<Thread, CachedUnitOfWork>();
        readonly object lockObject = new object();

        readonly int delayClean = 60;

        Timer timerCleanUnitOfWorks;
        #endregion

        #region Events
        public event EventHandler<CleaningUnitOfWorkArgs> CleaningUnitOfWorkEvent;
        void OnCleaningUnitOfWork(CachedUnitOfWork task)
        {
            var temp = CleaningUnitOfWorkEvent;
            if (temp != null)
            {
                var cache = new CleaningUnitOfWorkArgs() { Task = task };
                temp(this, cache);
            }
        }
        #endregion

        #region Constructors
        public CachedUnitOfWorks(IXpoDocument document)
        {
            this.document = document;
        }
        #endregion

        #region Private Methods
        void CleanUnusedUnitOfWorks(bool bForce = false)
        {
            bool bReStartTimer = false;

            List<CachedUnitOfWork> tasks = null;
            lock (lockObject)
            {
                StopCleanUnitOfWorksTimer();

                if (bForce)
                    tasks = cachedUnitOfWorks.Values.ToList();
                else
                {
                    var delay = TimeSpan.FromSeconds(delayClean);
                    tasks = (from c in cachedUnitOfWorks.Values.AsParallel()
                             where !c.InUse && (DateTime.UtcNow - c.LastTimeUsed) >= delay
                             select c).ToList();
                }

                if (tasks != null)
                    tasks.ForEach(task => cachedUnitOfWorks.Remove(task.Owner));
                bReStartTimer = cachedUnitOfWorks.Count > 0;
            }

            if (tasks != null && tasks.Count > 0)
            {
                tasks.ForEach(task =>
                {
                    OnCleaningUnitOfWork(task);
                    task.CleanUnitOfWork();
                });
            }

            if (bReStartTimer)
                StartCleanUnitOfWorksTimer();
        }

        void StartCleanUnitOfWorksTimer()
        {
            var delay = TimeSpan.FromSeconds(delayClean);
            StartCleanUnitOfWorksTimer(delay);
        }

        void StartCleanUnitOfWorksTimer(TimeSpan delay)
        {
            lock (lockObject)
            {
                if (timerCleanUnitOfWorks == null)
                {
                    timerCleanUnitOfWorks = new Timer((o) =>
                    {
                        CleanUnusedUnitOfWorks();
                    }, this, delay, delay);
                }
            }
        }

        void StopCleanUnitOfWorksTimer()
        {
            lock (lockObject)
            {
                if (timerCleanUnitOfWorks != null)
                {
                    timerCleanUnitOfWorks.Dispose();
                    timerCleanUnitOfWorks = null;
                }
            }
        }
        #endregion

        #region Public Methods
        public CachedUnitOfWork BeginUnitOfWork()
        {
            lock (lockObject)
            {
                if (!cachedUnitOfWorks.ContainsKey(Thread.CurrentThread))
                {
                    var idl = document.GetDataLayer();
                    cachedUnitOfWorks[Thread.CurrentThread] = new CachedUnitOfWork(idl);
                }
                else
                    cachedUnitOfWorks[Thread.CurrentThread].RenewUnitOfWork();

                cachedUnitOfWorks[Thread.CurrentThread].NotInUseEvent += (s, e) =>
                {
                    StartCleanUnitOfWorksTimer();
                };

                return cachedUnitOfWorks[Thread.CurrentThread];
            }
        }

        public bool ContainsThread()
        {
            lock (lockObject)
            {
                return cachedUnitOfWorks.ContainsKey(Thread.CurrentThread);
            }
        }

       #endregion

        #region IDisposable
        public void Dispose()
        {
            AutoResetEvent waitHandle = null;
            lock (lockObject)
            {
                if (timerCleanUnitOfWorks != null)
                {
                    waitHandle = new AutoResetEvent(false);
                    timerCleanUnitOfWorks.Change(0, System.Threading.Timeout.Infinite);
                    timerCleanUnitOfWorks.Dispose(waitHandle);
                }
            }

            if (waitHandle != null)
            {
                waitHandle.WaitOne();
                waitHandle.Dispose();
            }

            CleanUnusedUnitOfWorks(true);
        }
        #endregion
    }
}
