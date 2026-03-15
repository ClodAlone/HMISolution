using DocumentManager.ComponentService;
using log4net;
using Opc.Ua;
using OPCUAViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UFInterfaces;
using UFUAEditor.ComponentService;
using Utilities;

namespace UFProjectManager
{
    public class EventLogInputParameters
    {
        public string SourceName { get; set; }
        public string EventMessage { get; set; }
        public string EventDetails { get; set; }
        public string EventComment { get; set; }
        public string UserName { get; set; }
        public DateTime RecordingTime { get; set; }
        public EventSeverity Severity { get; set; }
        public DateTime LatencyTime { get; set; }
    }

    public class EventLogCommand : IDisposable
    {
        #region Declarations
        TimeSpan retryDelay = TimeSpan.FromMilliseconds(Properties.Settings.Default.EventLogErrorRetryIntervall);
        object lockEventLogThread = new object();
        Thread EventLogThread;
        ManualResetEvent AddEventLogEvent;
        List<EventLogInputParameters> PendingEventLogRequests;
        bool bInitialized;
        bool ExitMode;
        OPCUAEntityReference addEventLogMethod;
        SessionViewModel sessionViewModel;
#if !NET_STANDARD
        internal static readonly ILog logGeneral = LogManager.GetLogger(Properties.Resources.GeneralLog);
#else
        internal static readonly ILog logGeneral = LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), Properties.Resources.GeneralLog);
#endif

        IEntityReference entity;
        #endregion

        #region Methods
        public void Init(IEntityReference entity, IDocument parent, String sessionName)
        {
            if (bInitialized)
                return;
            this.entity = entity;

            var uaEditorManager = parent.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
            if (uaEditorManager != null)
            {
                var opcString = uaEditorManager.GetNodeIdEntityReference(parent,
                        UFUAServerInfo.BrowserNames.AddEventLog,
                        UFUAServerInfo.Guids.RootTagsGuid.ToString());
                if (!String.IsNullOrEmpty(opcString))
                {
                    try
                    {
                        addEventLogMethod = opcString.FromXml<OPCUAEntityReference>();
                    }
                    catch
                    { }
                }
            }

            if (addEventLogMethod != null)
            {
                addEventLogMethod.PropertyChanged += AddEventLogMethod_PropertyChanged;
                addEventLogMethod.Resolve(sessionName);
                addEventLogMethod.SetInUse(entity, true);
                bInitialized = true;
            }
            else
                logGeneral.Error(Properties.Resources.ClientEventLogNotInitialized);
        }

        void AddEventLogMethod_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var entityReference = sender as OPCUAEntityReference;
            if (e.PropertyName == "NodeIdViewModel")
            {
                if (sessionViewModel != null)
                {
                    sessionViewModel.PropertyChanged -= SessionViewModel_PropertyChanged;
                    sessionViewModel = null;
                }

                sessionViewModel = entityReference.NodeIdViewModel?.sessionViewModel;
                if (sessionViewModel != null)
                {
                    sessionViewModel.PropertyChanged += SessionViewModel_PropertyChanged;
                    if (sessionViewModel.Connected)
                        SessionViewModel_PropertyChanged(sessionViewModel, new System.ComponentModel.PropertyChangedEventArgs("Connected"));
                }
            }
        }

        void SessionViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var session = sender as SessionViewModel;
            if (e.PropertyName == "Connected")
            {
                if (session.Connected)
                {
                    lock (lockEventLogThread)
                    {
                        if (PendingEventLogRequests?.Count > 0)
                            AddEventLogEvent.Set();
                    }
                }
            }
        }

        public void AddLogEntity(string source, DateTime recordingTime, string message, string details, string comment, string userName,
                               System.Diagnostics.EventLogEntryType eventSeverity)
        {
            if (!bInitialized)
                return;

            Opc.Ua.EventSeverity severity = Opc.Ua.EventSeverity.Low;
            bool enLog = false;
            switch (eventSeverity)
            {
                case EventLogEntryType.Error:
                    severity = Opc.Ua.EventSeverity.High;
                    if (Properties.Settings.Default.IsErrorLogEnabled)
                        enLog = true;
                    break;
                case EventLogEntryType.FailureAudit:
                case EventLogEntryType.Warning:
                    severity = Opc.Ua.EventSeverity.Medium;
                    if (Properties.Settings.Default.IsWarningLogEnabled)
                        enLog = true;
                    break;
                case EventLogEntryType.Information:
                case EventLogEntryType.SuccessAudit:
                    severity = Opc.Ua.EventSeverity.Low;
                    if (Properties.Settings.Default.IsInformationLogEnabled)
                        enLog = true;
                    break;
                default:
                    break;
            }

            if (enLog)
            {
                Execute(new EventLogInputParameters()
                {
                    SourceName = source ?? String.Empty,
                    EventMessage = message ?? String.Empty,
                    EventDetails = details ?? String.Empty,
                    EventComment = comment ?? String.Empty,
                    UserName = userName ?? String.Empty,
                    RecordingTime = recordingTime,
                    Severity = severity
                });
            }
        }

        private bool CallMethod(Variant[] variants)
        {
            if (!CanExecute())
                return false;
            try
            {
                VariantCollection outputs = addEventLogMethod.NodeIdViewModel.CallMethod(variants);
                if (outputs == null || outputs.Count == 0)
                    return true;
                if (outputs.Count > 0)
                {
                    logGeneral.Error(Convert.ToString(outputs[0].Value));
                    return false;
                }
            }
            catch (Exception e)
            {
                logGeneral.Error(e.Message, e);
                return false;
            }
            return true;
        }

        void Terminate()
        {
            if (!bInitialized)
                return;
            bInitialized = false;
            if (sessionViewModel != null)
                sessionViewModel.PropertyChanged -= SessionViewModel_PropertyChanged;
            if (addEventLogMethod != null)
            {
                addEventLogMethod.PropertyChanged -= AddEventLogMethod_PropertyChanged;
                addEventLogMethod.SetInUse(entity, false);
            }
        }

        private bool CanExecute()
        {
            var session = sessionViewModel;
            return session != null && session.Connected;
        }

        private void Execute(EventLogInputParameters parameters)
        {
            if (bDisposed)
                return;

            lock (lockEventLogThread)
            {
                if (EventLogThread == null)
                {
                    PendingEventLogRequests = new List<EventLogInputParameters>();
                    AddEventLogEvent = new ManualResetEvent(false);
                    EventLogThread = new Thread((o) =>
                    {
                        int retryCount = 0;
                        int timeoutOnWait = -1;
                        while (!ExitMode)
                        {
                            if (!ExitMode)
                            {
                                AddEventLogEvent.WaitOne(timeoutOnWait);
                                timeoutOnWait = -1;
                            }

                            List<EventLogInputParameters> list;
                            List<EventLogInputParameters> listToRetry = new List<EventLogInputParameters>();
                            lock (lockEventLogThread)
                            {
                                list = PendingEventLogRequests.FindAll(item =>
                                item.LatencyTime == DateTime.MinValue || DateTime.Compare(item.LatencyTime.Add(retryDelay), DateTime.Now) < 0);
                                list.ForEach(item => PendingEventLogRequests.Remove(item));
                                if (PendingEventLogRequests.Count > 0)
                                    timeoutOnWait = Properties.Settings.Default.MaxEventLogErrorRetryCount;
                                AddEventLogEvent.Reset();
                            }

                            list.ForEach((item) =>
                            {
                                var inputs = new VariantCollection();
                                inputs.Add(new Variant(item.SourceName));
                                inputs.Add(new Variant(item.EventMessage));
                                inputs.Add(new Variant(item.EventDetails));
                                inputs.Add(new Variant(item.EventComment));
                                inputs.Add(new Variant(item.UserName));
                                inputs.Add(new Variant(item.RecordingTime));
                                inputs.Add(new Variant(item.Severity));
                                if (!CallMethod(inputs.ToArray()))
                                    listToRetry.Add(item);
                            });

                            if (listToRetry.Count > 0)
                            {
                                if (!ExitMode)
                                {
                                    if (++retryCount > Properties.Settings.Default.MaxEventLogErrorRetryCount)
                                    {
                                        logGeneral.Error(Properties.Resources.MaxEventLogErrorCountReached);
                                        retryCount = 0;
                                    }
                                    else
                                    {
                                        listToRetry.ForEach(item => item.LatencyTime = DateTime.Now);
                                        timeoutOnWait = Properties.Settings.Default.EventLogErrorRetryIntervall;
                                        lock (lockEventLogThread)
                                        {
                                            PendingEventLogRequests.AddRange(listToRetry);
                                        }
                                    }
                                }
                                else
                                    logGeneral.Error(Properties.Resources.PendingRequestLostOnExit);
                            }
                            else if (list.Count > 0)
                            {
                                retryCount = 0;
                            }
                        }
                    });
                    EventLogThread.Priority = ThreadPriority.BelowNormal;
                    EventLogThread.Name = "EventLogThread";
                    EventLogThread.Start();
                }

                PendingEventLogRequests.Add(parameters);
                if (PendingEventLogRequests.Count > Properties.Settings.Default.MaxPendingMessagesCount)
                {
                    while (PendingEventLogRequests.Count > 0 && PendingEventLogRequests.Count > Properties.Settings.Default.MaxPendingMessagesCount)
                        PendingEventLogRequests.RemoveAt(0);
                    logGeneral.Error(Properties.Resources.MaxPendingMessagesCountReached);
                }

                if (CanExecute())
                    AddEventLogEvent.Set();
            }
        }
        
        bool bDisposed;
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;
            
            Terminate();

            ExitMode = true;
            Thread eventLogThread = null;
            ManualResetEvent stopEvent = null;
            lock (lockEventLogThread)
            {
                stopEvent = AddEventLogEvent;
                eventLogThread = EventLogThread;
            }

            if (stopEvent != null)
                stopEvent.Set();
            if (eventLogThread != null)
                eventLogThread.Join();
            if (stopEvent != null)
                stopEvent.Dispose();
        }
        #endregion
    }
}