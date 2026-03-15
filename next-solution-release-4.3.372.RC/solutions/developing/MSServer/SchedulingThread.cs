using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MSServer
{
    class SchedulingThread : IDisposable  
    {
        private Thread WorkerThread;
        protected ManualResetEvent StopWorkerThread;
        protected readonly object lockThreadObject = new object();
        private object lockListObj = new object();

        #region Properties
        private List<ScheduledEvent> _EventList = new List<ScheduledEvent>();
        public List<ScheduledEvent> EventList
        {
            get { return _EventList; }
        }

        private bool _Updated = false;
        public bool Updated
        {
            get { return _Updated; }
            set
            {
                _Updated = value;
            }
        }
        
        #endregion

        #region Methods
        public virtual bool Startup(MSUAServer server)
        {
            lock (lockThreadObject)
            {
                if (WorkerThread == null)
                    WorkerThread = new Thread(WorkingThread);

                if (StopWorkerThread == null)
                    StopWorkerThread = new ManualResetEvent(false);
                else
                    StopWorkerThread.Reset();

                if (!WorkerThread.IsAlive)
                    WorkerThread.Start(server);

                return WorkerThread != null && (WorkerThread.ThreadState & (ThreadState.Stopped | ThreadState.Unstarted)) == 0;
            }
        }

        public int PostEvent(ScheduledEvent m)
        {
            lock (lockListObj)
            {
                var se = EventList.Find((o) => { return o.NodeId == m.NodeId; });
                if(se != null)
                    EventList.Remove(se);
                EventList.Add(m);
                Updated = true;
            }
            return 0;
        }

        public int PostEvent(List<ScheduledEvent> events)
        {
            if (events.Count == 0)
                return -1;

            lock (lockListObj)
            {
                foreach (var e in events)
                {
                    var se = EventList.Find((o) => { return o.NodeId == e.NodeId; });
                    if (se != null)
                        EventList.Remove(se);
                    EventList.Add(e);
                }
            }
            Updated = true;
            return 0;
        }

        public void StopThread()
        {
            if (StopWorkerThread != null)
                StopWorkerThread.Set();
            if (WorkerThread != null)
                WorkerThread.Join();
        }
        #endregion

        #region Thread
        List<ScheduledEvent> currentEvents = new List<ScheduledEvent>();
        int eventIndex = 0;
        protected virtual void WorkingThread(object data)
        {
            MSUAServer server = data as MSUAServer;
            if (server == null)
                return;

                int sleepCycle = 500;
                while (true)
                {
                    if (StopWorkerThread.WaitOne(sleepCycle, false))
                        break;
                    DateTime presentTime = DateTime.Now;

                    lock (lockListObj)
                    {
                        if (EventList.Count == 0)
                            continue;
                        for (int i = 0; eventIndex < EventList.Count && i < 100; i++)
                        {
                            int idx = eventIndex++;
                            if(EventList[idx].Enable == true || EventList[idx].RuntimeEnable == true)
                                currentEvents.Add(EventList[idx]);
                        }
                        if (eventIndex >= EventList.Count)
                            eventIndex = 0;
                    }

                    // work currentEvents list
                    foreach (var sc in currentEvents)
                    {
                        sc.Scheduling(presentTime);
                    }

                    currentEvents.Clear();

                    
                }
        }
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (StopWorkerThread != null)
                StopWorkerThread.Set();
            if (WorkerThread != null)
                WorkerThread.Join();

            lock (lockListObj)
            {
                EventList.Clear();
            }
            if (StopWorkerThread != null)
                StopWorkerThread.Dispose();

            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
