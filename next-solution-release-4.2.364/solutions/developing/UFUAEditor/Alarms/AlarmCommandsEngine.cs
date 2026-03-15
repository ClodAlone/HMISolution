using CommandManager.Executer;
using DocumentManager.ComponentService;
using Opc.Ua;
using OPCUAViewModel;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using UFInterfaces;
using UFUAAlarm;
using UFUAEditor.ComponentService;
using UFUAModel;
using Utilities;

namespace UFUAEditor.Alarms
{
    public class AlarmCommandsEngine : IDisposable
    {
        #region Declaration
        readonly Dispatcher dispatcher;
        readonly IUFUAEditorManager manager;
        readonly IDocument document;
        readonly IEntityReference entityReference;

        readonly Dictionary<string, AlarmCommandsExecuter> commandsExecuter = new Dictionary<string, AlarmCommandsExecuter>();
        readonly List<NotifyCollectionChangedEventArgs> notifyCollectionChangesList = new List<NotifyCollectionChangedEventArgs>();
        private readonly Dictionary<string, AlarmStatus> _mapAlarmStatusPlaySound = new Dictionary<string, AlarmStatus>();

        OPCUAEntityReference serverEntityReference;
        MonitoredItemViewModel serverMonitoredItem;
        SafeObservableCollection<ConditionStateViewModel> conditionStateList;
        List<ConditionStateViewModel> currentConditionState;
        Dictionary<string, string> conditionStateMap;

        DispatcherOperation dpCollectionChanged;

        bool bSubscribed;
        #endregion

        #region Constructors
        public AlarmCommandsEngine(Dispatcher dispatcher, IUFUAEditorManager manager, IDocument document)
        {
            this.dispatcher = dispatcher;
            this.manager = manager;
            this.document = document;

            entityReference = new EntityWeakReference();
        }
        #endregion

        #region Public Properties
        public bool IsEmpty => !commandsExecuter.Any() && !_mapAlarmStatusPlaySound.Any();

        #endregion

        #region Public Methods
        public void Add(string conditionName, UFUAModel.UFUAAlarmThreshold alarmThreshold)
        {
            if (commandsExecuter.ContainsKey(conditionName))
                return;

            var executer = new AlarmCommandsExecuter(alarmThreshold, document);
            if (!executer.IsEmpty)
                commandsExecuter.Add(conditionName, executer);
        }

        public void Init(string sessionName)
        {
            if (IsEmpty)
                return;

            currentConditionState = new List<ConditionStateViewModel>();
            conditionStateMap = new Dictionary<string, string>();

            SubscribeServerSession(sessionName);
            PrepareCommandsExecution(sessionName);
        }
        #endregion

        #region Private Methods
        void SubscribeServerSession(string sessionName)
        {
            if (bSubscribed)
                return;
            bSubscribed = true;

            var server = manager.GetServerEntityReference(document);
            if (server == null)
                return;
            try
            {
                serverEntityReference = server.FromXml<OPCUAEntityReference>();
            }
            catch
            { }

            if (serverEntityReference == null || !serverEntityReference.IsValid)
                return;

            serverEntityReference.PropertyChanged += serverEntityReference_PropertyChanged;
            if (serverEntityReference.MonitoredItemViewModel != null)
                serverEntityReference_PropertyChanged(serverEntityReference, new PropertyChangedEventArgs("MonitoredItemViewModel"));

            serverEntityReference.SetInUse(entityReference, true);
            serverEntityReference.Resolve(sessionName);
        }

        void serverEntityReference_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OPCUAEntityReference n = (OPCUAEntityReference)sender;
            if (e.PropertyName == "MonitoredItemViewModel")
            {
                dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                {
                    if (bDisposed)
                        return;

                    lock (notifyCollectionChangesList)
                    {
                        notifyCollectionChangesList.Clear();
                        //if (serverMonitoredItem != null)
                        //    serverMonitoredItem.PropertyChanged -= serverMonitoredItem_PropertyChanged;

                        if (conditionStateList != null)
                            conditionStateList.CollectionChanged -= ConditionState_CollectionChanged;

                        CleanCurrentConditionState();
                        if (conditionStateMap != null)
                            conditionStateMap.Clear();

                        serverMonitoredItem = n.MonitoredItemViewModel;
                        if (serverMonitoredItem != null)
                        {
                            currentConditionState.AddRange(serverMonitoredItem.GetCurrentConditionStateList());
                            foreach (var conditionState in currentConditionState)
                            {
                                if (!conditionStateMap.ContainsKey(conditionState.Condition))
                                    conditionStateMap.Add(conditionState.Condition, ConditionStateNames.Inactive);
                                conditionState.PropertyChanged += ConditionState_PropertyChanged;
                                ConditionState_PropertyChanged(conditionState, new PropertyChangedEventArgs("EnabledState"));
                            }

                            conditionStateList = serverMonitoredItem.ConditionStateList;
                            if (conditionStateList != null)
                                conditionStateList.CollectionChanged += ConditionState_CollectionChanged;

                            //serverMonitoredItem.PropertyChanged += serverMonitoredItem_PropertyChanged;
                            //serverMonitoredItem_PropertyChanged(serverMonitoredItem, new PropertyChangedEventArgs("Quality"));
                        }
                    }
                });
            }
        }

        void ConditionState_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            lock (notifyCollectionChangesList)
            {
                var bNewDispatcherOperation = notifyCollectionChangesList.Count == 0;
                notifyCollectionChangesList.Add(e);

                if (bNewDispatcherOperation || dpCollectionChanged == null ||
                    dpCollectionChanged.Status == DispatcherOperationStatus.Completed ||
                    dpCollectionChanged.Status == DispatcherOperationStatus.Aborted)
                {
                    dpCollectionChanged = dispatcher.BeginInvokeAsynchronouslyInBackground(() => {
                        var notifyList = new List<NotifyCollectionChangedEventArgs>();
                        lock (notifyCollectionChangesList)
                        {
                            notifyList.AddRange(notifyCollectionChangesList);
                            notifyCollectionChangesList.Clear();
                        }

                        foreach (var item in notifyList)
                        {
                            if (item.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
                            {
                                CleanCurrentConditionState();
                            } else
                            {
                                if (item.NewItems != null && item.NewItems.Count > 0)
                                {
                                    foreach (ConditionStateViewModel conditionState in item.NewItems)
                                    {
                                        var branchNodeId = new NodeId(conditionState.Branch);
                                        if (!branchNodeId.IsNullNodeId)
                                            continue;

                                        if (!conditionStateMap.ContainsKey(conditionState.Condition))
                                            conditionStateMap.Add(conditionState.Condition, ConditionStateNames.Inactive);
                                        conditionState.PropertyChanged += ConditionState_PropertyChanged;
                                        ConditionState_PropertyChanged(conditionState, new PropertyChangedEventArgs("EnabledState"));
                                        currentConditionState.Add(conditionState);
                                    }
                                }

                                if (item.OldItems != null && item.OldItems.Count > 0)
                                {
                                    foreach (ConditionStateViewModel conditionState in item.OldItems)
                                    {
                                        var branchNodeId = new NodeId(conditionState.Branch);
                                        if (!branchNodeId.IsNullNodeId)
                                            continue;

                                        conditionState.PropertyChanged -= ConditionState_PropertyChanged;
                                        conditionStateMap.Remove(conditionState.Condition);
                                        currentConditionState.Remove(conditionState);
                                    }
                                }
                            }
                        }
                    });
                }
            }
        }

        void ConditionState_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var conditionState = (ConditionStateViewModel)sender;

            var branchNodeId = new NodeId(conditionState.Branch);
            
#if !NET_STANDARD
            if (_mapAlarmStatusPlaySound.ContainsKey(conditionState.Condition))
            {
                var alarm = _mapAlarmStatusPlaySound[conditionState.Condition];
                var bStopPlay = conditionState.EnabledState.Contains(ConditionStateNames.Disabled) ||
                                conditionState.EnabledState.Contains(ConditionStateNames.Acknowledged) ||
                                conditionState.EnabledState.Contains(ConditionStateNames.Suppressed) ||
                                conditionState.EnabledState.Contains(ConditionStateNames.Unconfirmed) ||
                                conditionState.EnabledState.Contains(AlarmState.Shelved.ToString()) ||
                                conditionState.EnabledState.Contains(AlarmState.Deleted.ToString());

                if (bStopPlay)
                    SoundState.PlayAlarmSound(alarm, false);
                else if (conditionState.EnabledState.Contains(ConditionStateNames.Active))
                    SoundState.PlayAlarmSound(alarm, true);
            }
#endif

            if (!branchNodeId.IsNullNodeId ||
                !commandsExecuter.ContainsKey(conditionState.Condition) ||
                !conditionStateMap.ContainsKey(conditionState.Condition) || 
                conditionStateMap[conditionState.Condition] == conditionState.EnabledState ||
                conditionState.EnabledState == ConditionStateNames.Disabled)
                return;

            if (conditionStateMap[conditionState.Condition].Contains(ConditionStateNames.Inactive) && 
                !conditionState.EnabledState.Contains(ConditionStateNames.Inactive))
            {
                commandsExecuter[conditionState.Condition].Execute(AlarmCommandsEventType.CommandsOn);
            }
            if (!conditionStateMap[conditionState.Condition].Contains(ConditionStateNames.Inactive) &&
                conditionState.EnabledState.Contains(ConditionStateNames.Inactive))
            {
                commandsExecuter[conditionState.Condition].Execute(AlarmCommandsEventType.CommandsOff);
            }
            if (conditionStateMap[conditionState.Condition].Contains(ConditionStateNames.Unacknowledged) && 
                !conditionState.EnabledState.Contains(ConditionStateNames.Unacknowledged))
            {
                commandsExecuter[conditionState.Condition].Execute(AlarmCommandsEventType.CommandsAck);
            }
            if (conditionStateMap[conditionState.Condition].Contains(ConditionStateNames.Unconfirmed) &&
                !conditionState.EnabledState.Contains(ConditionStateNames.Unconfirmed))
            {
                commandsExecuter[conditionState.Condition].Execute(AlarmCommandsEventType.CommandsReset);
            }

            conditionStateMap[conditionState.Condition] = conditionState.EnabledState;
        }

        //void serverMonitoredItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    MonitoredItemViewModel m = (MonitoredItemViewModel)sender;
        //    if (e.PropertyName == "DataValue" || e.PropertyName == "Quality")
        //    {
        //        dispatcher.BeginInvokeAsynchronouslyInBackground(() => 
        //        {
        //            if (bDisposed)
        //                return;
        //        });
        //    }
        //}

        void UnsubscribeServerSession()
        {
            if (!bSubscribed)
                return;
            bSubscribed = false;

            if (serverEntityReference == null || !serverEntityReference.IsValid)
                return;

            serverEntityReference.PropertyChanged -= serverEntityReference_PropertyChanged;
            //if (serverMonitoredItem != null)
            //    serverMonitoredItem.PropertyChanged -= serverMonitoredItem_PropertyChanged;

            serverEntityReference.SetInUse(entityReference, false);
        }

        void PrepareCommandsExecution(string sessionName)
        {
            commandsExecuter.Values.ToList().ForEach(executer => executer.PrepareExecution(sessionName));
        }

        void TerminateCommandsExecution()
        {
            commandsExecuter.Values.ToList().ForEach(executer => executer.TerminateExecution());
        }

        void CleanCurrentConditionState()
        {
            if (currentConditionState != null && currentConditionState.Count > 0)
            {
                foreach (var conditionState in currentConditionState)
                    conditionState.PropertyChanged -= ConditionState_PropertyChanged;
                currentConditionState.Clear();
            }
        }
        #endregion

        #region IDisposable
        bool bDisposed;
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;

            UnsubscribeServerSession();
            TerminateCommandsExecution();

            lock (notifyCollectionChangesList)
            {
                notifyCollectionChangesList.Clear();

                if (conditionStateList != null)
                    conditionStateList.CollectionChanged -= ConditionState_CollectionChanged;
                CleanCurrentConditionState();
                if (conditionStateMap != null)
                    conditionStateMap.Clear();

                if (dpCollectionChanged != null &&
                    dpCollectionChanged.Status != DispatcherOperationStatus.Aborted &&
                    dpCollectionChanged.Status != DispatcherOperationStatus.Completed)
                    dpCollectionChanged.Abort();
            }

            commandsExecuter.Clear();
            _mapAlarmStatusPlaySound.Clear();
            SoundState.ShutDown();
        }
        #endregion

        /// <summary>
        /// Add the tag sound file to possible managed alarms to be reproduced in client
        /// </summary>
        /// <param name="conditionName">The alarm threshold unique identifier</param>
        /// <param name="alarmStatus">Contains the sound file required properties to be played</param>
        public void AddPlaySoundCommand(string conditionName, AlarmStatus alarmStatus)
        {
            if(!_mapAlarmStatusPlaySound.ContainsKey(conditionName))
                _mapAlarmStatusPlaySound.Add(conditionName, alarmStatus);
        }
    }
}
