using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
#if !NET_STANDARD
using System.Threading.Tasks;
using System.Windows.Threading;
#endif
using CommandManager;
using DocumentManager.ComponentService;
using ExpressionManager;
using OPCUAViewModel;
using UFEventEditor.Document;
using UFEventModel;
using UFInterfaces;

namespace UFEventEditor
{
    public class EventsThread : IDisposable
    {
        #region Declarations
        private Thread WorkerThread;
        protected ManualResetEvent StopWorkerThread;
        AutoResetEvent FireScheduleEvent;
        protected readonly object lockThreadObject = new object();
        private object lockListObj = new object();
        private List<EventObject> EventList;
        private List<EventObject> SchedEventList;
        EventEditorDocument Doc;
        //private const string eventssessionname = "EventsManager";
        string eventssessionname;
        Timer timeoutTimer;
        #endregion

        #region Properties
        #endregion

        #region Public methods
        public virtual bool Startup(EventEditorDocument server)
        {
            //carica eventi
            //quelli semplici vengono avviati con OPCUAEntityReference()
            //se ci sono eventi temporizzati, vengono passati al thread.
            Doc = server;
            if (Doc == null)
                return false;

            eventssessionname = Doc.SessionString;
            Doc.UpdateSessionSettings();

            lock (lockThreadObject)
            {
                if (FireScheduleEvent == null)
                    FireScheduleEvent = new AutoResetEvent(false);
                else
                    FireScheduleEvent.Reset();
            }

            EventList = Doc.GetExecutionEvents().ToList();
            foreach (var ev in EventList)
            {
                var listCommands = ev.RuntimeCommandList as CommandManagerList;
                listCommands.ForEach(command =>
                {
                    command.Init(ev, Doc.Parent ?? Doc, eventssessionname);
                });

                if (!ev.Enable)
                {
                    Doc.LogMessage(string.Format(Properties.Resources.EventDisabledWarning, ev.Name), System.Diagnostics.EventLogEntryType.Warning);
                    continue;
                }
                if(ev.Type == EventType.Tag)
                {
                    if (ev.Tag != null)
                    {
                        if (SchedEventList == null)
                            SchedEventList = new List<EventObject>();
                        ev.PrepareExecution(eventssessionname, Doc);
                        SchedEventList.Add(ev);
                        ev.FireSchedule = FireScheduleEvent;
                    }
                }
                else if(ev.Type == EventType.Schedule)
                {
                    if (SchedEventList == null)
                        SchedEventList = new List<EventObject>();
                    ev.PrepareExecutionEnable(eventssessionname, Doc);
                    SchedEventList.Add(ev);
                }

                ev.ParserError += (s, e) =>
                {
                    ExpressionEntity_ParserError(e, s);
                };

                ev.ExecutionError += (s, e) =>
                {
                    ExpressionEntity_ExecutionError(e, s);
                };
            }

            if (SchedEventList == null || SchedEventList.Count == 0)
                return false;

            //start timeout timer
            int delay = Properties.Settings.Default.ConnectionTimeout * 1000;
            timeoutTimer = new Timer((o) =>
            {
                lock (lockThreadObject)
                {
                    if (timeoutTimer != null)
                    {
                        timeoutTimer.Dispose();
                        timeoutTimer = null;
                    }
                }
                
                foreach (var ev in EventList)
                {
                    if (ev.Enable)
                        ev.CheckInitialConnectionTimeout();
                }
            }, this, delay, Timeout.Infinite);
            lock (lockThreadObject)
            {
                if (WorkerThread == null)
                {
                    WorkerThread = new Thread(WorkingThread);
#if !NET_STANDARD
                    WorkerThread.SetApartmentState(ApartmentState.STA);
#endif
                }

                if (StopWorkerThread == null)
                    StopWorkerThread = new ManualResetEvent(false);
                else
                    StopWorkerThread.Reset();

                if (!WorkerThread.IsAlive)
                    WorkerThread.Start(server);

                return WorkerThread != null && (WorkerThread.ThreadState & (ThreadState.Stopped | ThreadState.Unstarted)) == 0;
            }
        }

        private void ExpressionEntity_ParserError(object sender, object e)
        {
            var expressionEntity = (ExpressionEntity)sender;
            var evObj = (EventObject)e;
            var error = expressionEntity.GetParserError();
            if (evObj.lastExpressionError != error)
            {
                evObj.lastExpressionError = error;
                if (!String.IsNullOrEmpty(error))
                    Doc.LogMessage(string.Format(Properties.Resources.ErrorParseExpression, evObj.Name, 
                        expressionEntity.Formula, error), System.Diagnostics.EventLogEntryType.Error);
            }
        }

        private void ExpressionEntity_ExecutionError(object sender, object e)
        {
            var expressionEntity = (ExpressionEntity)sender;
            var evObj = (EventObject)e;
            var exception = expressionEntity.GetExecutionError();
            if (exception != null)
            {
                Doc.LogMessage(string.Format(Properties.Resources.ErrorParseExpression, evObj.Name,
                    expressionEntity.Formula, exception.Message), System.Diagnostics.EventLogEntryType.Error);
            }
        }

        void StopThread()
        {
            Thread workerthread = WorkerThread;
            lock (lockThreadObject)
            {
                if (timeoutTimer != null)
                {
                    timeoutTimer.Dispose();
                    timeoutTimer = null;
                }

                if (StopWorkerThread != null)
                    StopWorkerThread.Set();

                if (FireScheduleEvent != null)
                    FireScheduleEvent.Set();
            }
            if (workerthread != null)
                workerthread.Join();

            if (EventList != null)
            {
                foreach (var ev in EventList)
                    ev.Dispose();
            }
        }
        #endregion

        #region Thread
        protected virtual void WorkingThread(object data)
        { 
            //caricamento degli eventi
            int sleepCycle = Properties.Settings.Default.ScheduleTimeEvents;

            while (true)
            {
                foreach (var ev in EventList)
                {
                    if (ev.Enable)
                    {
                        var listCommands = ev.RuntimeCommandList as CommandManagerList;
                        listCommands.ForEach(command =>
                        {
                            command.CanExecute();
                        });
                    }
                }

                FireScheduleEvent.WaitOne(sleepCycle);
                if (StopWorkerThread.WaitOne(0))
                    break;

                if (SchedEventList != null && SchedEventList.Count > 0)
                {
                    foreach (var ev in SchedEventList)
                    {
                        if (!ev.Schedule())
                            Doc.LogMessage(string.Format(Properties.Resources.ErrorEvent, ev.Name, ev.LastMessage), System.Diagnostics.EventLogEntryType.Error);
                        else if (ev.LastMessage != string.Empty)
                        {
                            ev.LastMessage = string.Empty;
                            Doc.LogMessage(string.Format(Properties.Resources.ReturnFromErrorEvent, ev.Name), System.Diagnostics.EventLogEntryType.Information);
                        }
                    }
                }
            }
        }
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            StopThread();
            
            if (StopWorkerThread != null)
                StopWorkerThread.Dispose();

            if (FireScheduleEvent != null)
                FireScheduleEvent.Dispose();

            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
