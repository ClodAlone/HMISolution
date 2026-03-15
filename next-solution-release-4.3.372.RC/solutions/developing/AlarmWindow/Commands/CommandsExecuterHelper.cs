using CommandManager.Executer;
using DocumentManager.ComponentService;
using OPCUAViewModel;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using UFInterfaces;
using UFUAEditor.ComponentService;
using Utilities;

namespace AlarmWindow.Commands
{
    public class CommandsExecuterHelper : IDisposable
    {
        #region Declaration
        readonly Dispatcher dispatcher;
        readonly IUFUAEditorManager ufuaEditorManager;
        readonly IDocument document;
        readonly IEntityReference entityReference;

        readonly Dictionary<String, CommandsExecuter> commandsExecuter = new Dictionary<String, CommandsExecuter>();

        OPCUAEntityReference serverEntityReference;
        MonitoredItemViewModel serverMonitoredItem;
        SafeObservableCollection<ConditionStateViewModel> conditionStateList;
        String sessionName;

        Dictionary<String, String> commandsOnDbClick;

        bool bSubscribed;
        DispatcherOperation dpCollectionChanged;
        List<System.Collections.Specialized.NotifyCollectionChangedEventArgs> notifyCollectionChangesList = new List<System.Collections.Specialized.NotifyCollectionChangedEventArgs>();

        #endregion

        #region Constructors
        public CommandsExecuterHelper(Dispatcher dispatcher, IUFUAEditorManager ufuaEditorManager, IDocument document, IEntityReference entityReference)
        {
            this.dispatcher = dispatcher;
            this.ufuaEditorManager = ufuaEditorManager;
            this.document = document;
            this.entityReference = entityReference;
        }
        #endregion

        #region Public Methods
        public void Resolve(string sessionName)
        {
            this.sessionName = sessionName;
            SubscribeServerSession(sessionName);
        }

        public bool CanExecute(ConditionStateViewModel conditionState)
        {
            if (commandsExecuter.ContainsKey(conditionState.Condition))
                return commandsExecuter[conditionState.Condition].CanExecute;

            return false;
        }

        public void Execute(ConditionStateViewModel conditionState)
        {
            if (commandsExecuter.ContainsKey(conditionState.Condition))
                commandsExecuter[conditionState.Condition].Execute();
        }

        public void RemoteExecute(ConditionStateViewModel conditionState)
        {
            if (commandsExecuter.ContainsKey(conditionState.Condition))
                commandsExecuter[conditionState.Condition].RemoteExecute();
        }
        #endregion

        #region Private Methods
        void SubscribeServerSession(string sessionName)
        {
            if (bSubscribed)
                return;
            bSubscribed = true;

            commandsOnDbClick = ufuaEditorManager.GetAlarmCommandsOnDbClick(document) as Dictionary<string, string>;
            if (commandsOnDbClick == null || commandsOnDbClick.Count == 0)
                return;

            var server = ufuaEditorManager.GetServerEntityReference(document);
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

                    //if (serverMonitoredItem != null)
                    //    serverMonitoredItem.PropertyChanged -= serverMonitoredItem_PropertyChanged;
                    serverMonitoredItem = n.MonitoredItemViewModel;

                    if (serverMonitoredItem != null)
                    {
                        if (conditionStateList != null)
                        {
                            conditionStateList.CollectionChanged -= ConditionState_CollectionChanged;
                            TerminateCommandsExecuters();
                        }

                        conditionStateList = serverMonitoredItem.ConditionStateList;
                        if (conditionStateList != null)
                        {
                            conditionStateList.CollectionChanged += ConditionState_CollectionChanged;
                            PrepareCommandsExecuters();
                        }

                        //serverMonitoredItem.PropertyChanged += serverMonitoredItem_PropertyChanged;
                        //serverMonitoredItem_PropertyChanged(serverMonitoredItem, new PropertyChangedEventArgs("Quality"));
                    }
                });
            }
        }
        void ConditionState_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            bool bNewDispatcherOperation = false;
            lock (notifyCollectionChangesList)
            {
                bNewDispatcherOperation = notifyCollectionChangesList.Count == 0;
                notifyCollectionChangesList.Add(e);
            }
            if (bNewDispatcherOperation || dpCollectionChanged == null ||
                dpCollectionChanged.Status == DispatcherOperationStatus.Completed ||
                dpCollectionChanged.Status == DispatcherOperationStatus.Aborted)
            {
                dpCollectionChanged = dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                {
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
                            CleanCommandsExecuters();
                        }
                        else
                        {

                            if (item.NewItems != null && item.NewItems.Count > 0)
                            {
                                foreach (ConditionStateViewModel conditionState in item.NewItems)
                                    PrepareCommandsExecuter(conditionState);
                            }

                            if (item.OldItems != null && item.OldItems.Count > 0)
                            {
                                foreach (ConditionStateViewModel conditionState in item.OldItems)
                                    TerminateCommandsExecuter(conditionState);
                            }


                        }
                    }
                });
            }
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

        void PrepareCommandsExecuters()
        {
            if (serverMonitoredItem != null && serverMonitoredItem.ConditionStateList != null)
            {
                var conditionStateList = serverMonitoredItem.ConditionStateList.ToList();
                foreach (var conditionState in conditionStateList)
                    PrepareCommandsExecuter(conditionState);
            }
        }

        void TerminateCommandsExecuters()
        {
            if (serverMonitoredItem != null && serverMonitoredItem.ConditionStateList != null)
            {
                var conditionStateList = serverMonitoredItem.ConditionStateList.ToList();
                foreach (var conditionState in conditionStateList)
                    TerminateCommandsExecuter(conditionState);
            }
        }

        void PrepareCommandsExecuter(ConditionStateViewModel conditionState)
        {
            if (!commandsOnDbClick.ContainsKey(conditionState.Condition))
                return;

            var commands = commandsOnDbClick[conditionState.Condition];
            if (!commandsExecuter.ContainsKey(conditionState.Condition))
            {
                var executer = CommandsExecuter.CreateFromXaml(commands, document);
                if (executer != null)
                {
                    executer.PrepareExecution(sessionName);
                    commandsExecuter.Add(conditionState.Condition, executer);
                }
            }
        }

        void TerminateCommandsExecuter(ConditionStateViewModel conditionState)
        {
            if (!commandsOnDbClick.ContainsKey(conditionState.Condition))
                return;

            if (commandsExecuter.ContainsKey(conditionState.Condition))
            {
                commandsExecuter[conditionState.Condition].TerminateExecution();
                commandsExecuter.Remove(conditionState.Condition);
            }
        }

        void CleanCommandsExecuters()
        {
            if (commandsExecuter != null)
            {
                foreach (var executer in commandsExecuter.Values)
                    executer.TerminateExecution();
                commandsExecuter.Clear();
            }
        }

        void AbortDispatcherOperations()
        {

            if (dpCollectionChanged != null &&
                dpCollectionChanged.Status != DispatcherOperationStatus.Aborted &&
                dpCollectionChanged.Status != DispatcherOperationStatus.Completed)
                dpCollectionChanged.Abort();
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
            TerminateCommandsExecuters();

            if (conditionStateList != null)
                conditionStateList.CollectionChanged -= ConditionState_CollectionChanged;

            CleanCommandsExecuters();
            AbortDispatcherOperations();
        }
        #endregion

    }
}
