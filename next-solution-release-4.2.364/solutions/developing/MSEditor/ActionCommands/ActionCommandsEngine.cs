using CommandManager.Executer;
using DocumentManager.ComponentService;
using log4net;
using MSSchedulerSettings.ComponentService;
using Opc.Ua;
using OPCUAViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using UFInterfaces;
using Utilities;

namespace MSEditor.ActionCommands
{
    public class ActionCommandsEngine : IDisposable
    {
        #region Declaration
        readonly Dispatcher dispatcher;
        readonly ISchedulerEditorManager manager;
        readonly IDocument document;
        readonly IEntityReference entityReference;

        readonly Dictionary<string, ActionCommandsExecuter> commandsExecuter = new Dictionary<string, ActionCommandsExecuter>();
        readonly Dictionary<string, bool> actionPreviousStates = new Dictionary<string, bool>();
        readonly Dictionary<OPCUAEntityReference, string> opcuaEntityReferences = new Dictionary<OPCUAEntityReference, string>();
        readonly Dictionary<OPCUAEntityReference, MonitoredItemViewModel> enabledStates = new Dictionary<OPCUAEntityReference, MonitoredItemViewModel>();
        readonly Dictionary<OPCUAEntityReference, MonitoredItemViewModel> schedulerStates = new Dictionary<OPCUAEntityReference, MonitoredItemViewModel>();

        readonly Dictionary<OPCUAEntityReference, MonitoredItemViewModel> executionOffStates = new Dictionary<OPCUAEntityReference, MonitoredItemViewModel>();

        static readonly String goodQuality = new Opc.Ua.StatusCode(Opc.Ua.StatusCodes.Good).ToString();

        static readonly ILog log = LogManager.GetLogger(Properties.Resources.SchedulerLog);

        readonly Object lockObject = new Object();

        bool bSubscribed;
        #endregion

        #region Constructors
        public ActionCommandsEngine(Dispatcher dispatcher, ISchedulerEditorManager manager, IDocument document)
        {
            this.dispatcher = dispatcher;
            this.manager = manager;
            this.document = document;

            entityReference = new EntityWeakReference();
        }
        #endregion

        #region Public Properties
        public bool IsEmpty
        {
            get
            {
                return commandsExecuter.Count == 0;
            }
        }
        #endregion

        #region Public Methods
        public void Add(string nodeId, MSModel.MSScheduledAction scheduledAction)
        {
            if (commandsExecuter.ContainsKey(nodeId))
                return;

            var executer = new ActionCommandsExecuter(scheduledAction, document);
            if (!executer.IsEmpty)
                commandsExecuter.Add(nodeId, executer);
        }

        public void Init(string sessionName)
        {
            if (IsEmpty)
                return;

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

            commandsExecuter.Keys.ToList().ForEach(nodeId => 
            {
                OPCUAEntityReference schedulerEntityReference = null;
                var serverEntity = manager.GetSchedulerEntityReference(document, nodeId);
                try
                {
                    if (serverEntity != null)
                        schedulerEntityReference = serverEntity.FromXml<OPCUAEntityReference>();
                }
                catch
                { }

                if (schedulerEntityReference != null && schedulerEntityReference.IsValid)
                    opcuaEntityReferences.Add(schedulerEntityReference, nodeId);
            });

            opcuaEntityReferences.Keys.ToList().ForEach(schedulerEntityReference => 
            {
                schedulerEntityReference.PropertyChanged += serverEntityReference_PropertyChanged;
                if (schedulerEntityReference.MonitoredItemViewModel != null)
                    serverEntityReference_PropertyChanged(entityReference, new PropertyChangedEventArgs("MonitoredItemViewModel"));

                schedulerEntityReference.SetInUse(entityReference, true);
                schedulerEntityReference.Resolve(sessionName);
            });
        }

        void serverEntityReference_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (bDisposed)
                return;
            OPCUAEntityReference n = (OPCUAEntityReference)sender;
            if (e.PropertyName == "MonitoredItemViewModel")
            {
                Task.Factory.StartNew(() => {
                    lock (lockObject)
                    {
                        if (schedulerStates.ContainsKey(n))
                            schedulerStates[n].PropertyChanged -= schedulerState_PropertyChanged;
                        schedulerStates[n] = n.MonitoredItemViewModel;
                        if (enabledStates.ContainsKey(n))
                            enabledStates[n].PropertyChanged -= enabledState_PropertyChanged;

                        if (executionOffStates.ContainsKey(n))
                            executionOffStates[n].PropertyChanged -= executeoffState_PropertyChanged;
                    }
                    if (n.MonitoredItemViewModel != null)
                    {
                        n.MonitoredItemViewModel.PropertyChanged += schedulerState_PropertyChanged;
                            EvaluateCommandExecution(n.MonitoredItemViewModel, n);
                            //schedulerState_PropertyChanged(schedulerStates[n], new PropertyChangedEventArgs("DataValue"));
                    }
                    
                });
            }
        }

        void EvaluateCommandExecution(MonitoredItemViewModel m, OPCUAEntityReference schedulerEntityReference)
        {
            MonitoredItemViewModel enableItem = null;
            MonitoredItemViewModel offItem = null;
            string nodeId;
            bool bApplyChanges = false;
            lock (lockObject)
            {

                if (schedulerEntityReference == null || !opcuaEntityReferences.ContainsKey(schedulerEntityReference))
                    return;

                nodeId = opcuaEntityReferences[schedulerEntityReference];

                if (!enabledStates.ContainsKey(schedulerEntityReference))
                {
                    var child = m.GetChildItem(MSServerInfo.MSServerInfo.GetEnableStateName());
                    if (child != null)
                    {
                        enabledStates.Add(schedulerEntityReference, child);
                        child.PropertyChanged += enabledState_PropertyChanged;
                        enableItem = child;
                        bApplyChanges = true;
                    }
                }
                else
                    enableItem = enabledStates[schedulerEntityReference];


                if (!executionOffStates.ContainsKey(schedulerEntityReference))
                {
                    var offchild = m.GetChildItem(MSServerInfo.MSServerInfo.GetExecuteOffName());
                    if (offchild != null)
                    {
                        executionOffStates.Add(schedulerEntityReference, offchild);
                        offchild.PropertyChanged += executeoffState_PropertyChanged;
                        offItem = offchild;
                        bApplyChanges = true;
                    }
                }
                else
                    offItem = executionOffStates[schedulerEntityReference];
                if (bApplyChanges)
                    m.GetSubscriptionViewModelParent().ApplyChanges();
            }
            if (enableItem == null || enableItem.DataValue == null ||
                    !StatusCode.IsGood(enableItem.DataValue.StatusCode))
                    return;

            if (offItem == null || offItem.DataValue == null ||
                        !StatusCode.IsGood(offItem.DataValue.StatusCode))
                return;
            bool enabledState;
            if (bool.TryParse(enableItem.Value, out enabledState) && enabledState)
            {
                if (commandsExecuter.ContainsKey(nodeId))
                {
                    bool actionState;
                    if (bool.TryParse(m.Value, out actionState))
                    {
                        if (!actionPreviousStates.ContainsKey(nodeId) || actionPreviousStates[nodeId] != actionState)
                        {
                            bool bExOff;
                            if (actionState && (commandsExecuter[nodeId].ExecOnAtStartup || m.Quality == goodQuality))
                            {
                                actionPreviousStates[nodeId] = actionState;
                                commandsExecuter[nodeId].Execute(ActionCommandsEventType.CommandsOn);
                                log.Info(string.Format(Properties.Resources.CommandsExecuted, commandsExecuter[nodeId].Scheduler, Properties.Resources.CommandON));
                            }
                            else if (!actionState && bool.TryParse(offItem.Value, out bExOff) && bExOff && (commandsExecuter[nodeId].ExecOffAtStartup || m.Quality == goodQuality))
                            {
                                actionPreviousStates[nodeId] = actionState;
                                commandsExecuter[nodeId].Execute(ActionCommandsEventType.CommandsOff);
                                log.Info(string.Format(Properties.Resources.CommandsExecuted, commandsExecuter[nodeId].Scheduler, Properties.Resources.CommandOFF));
                            }
                        }
                    }
                }
            }
            
        }
        void schedulerState_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (bDisposed)
                return;

            MonitoredItemViewModel m = (MonitoredItemViewModel)sender;
            if (e.PropertyName == "DataValue")
            {
                if (m.DataValue != null && StatusCode.IsGood(m.DataValue.StatusCode))
                {
                    Task.Factory.StartNew(() => {
                        OPCUAEntityReference schedulerEntityReference = null;
                        lock (lockObject)
                        {
                            schedulerEntityReference = (from c in schedulerStates.Keys
                                                            where schedulerStates[c] == m
                                                            select c).FirstOrDefault();
                        }
                        EvaluateCommandExecution(m, schedulerEntityReference);
                    });
                }
            }
        }

        void executeoffState_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (bDisposed)
                return;

            MonitoredItemViewModel m = (MonitoredItemViewModel)sender;
            if (e.PropertyName == "DataValue")
            {
                if (m.DataValue != null && StatusCode.IsGood(m.DataValue.StatusCode))
                {
                    Task.Factory.StartNew(() => {
                        OPCUAEntityReference schedulerEntityReference = null;
                        MonitoredItemViewModel mon = null;
                        lock (lockObject)
                        {
                            schedulerEntityReference = (from c in executionOffStates.Keys
                                                            where executionOffStates[c] == m
                                                            select c).FirstOrDefault();
                            if (schedulerStates.ContainsKey(schedulerEntityReference))
                                mon = schedulerStates[schedulerEntityReference];
                                //schedulerState_PropertyChanged(schedulerStates[schedulerEntityReference], new PropertyChangedEventArgs("DataValue"));
                        }
                        if(mon != null)
                            EvaluateCommandExecution(mon, schedulerEntityReference);
                    });
                }
            }
        }

        void enabledState_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (bDisposed)
                return;

            MonitoredItemViewModel m = (MonitoredItemViewModel)sender;
            if (e.PropertyName == "DataValue")
            {
                if (m.DataValue != null && StatusCode.IsGood(m.DataValue.StatusCode))
                {
                    Task.Factory.StartNew(() => {
                        OPCUAEntityReference schedulerEntityReference = null;
                        MonitoredItemViewModel mon = null;
                        lock (lockObject)
                        {
                            schedulerEntityReference = (from c in enabledStates.Keys
                                                            where enabledStates[c] == m
                                                            select c).FirstOrDefault();

                            if (schedulerStates.ContainsKey(schedulerEntityReference))
                                mon = schedulerStates[schedulerEntityReference];
                                
                                //schedulerState_PropertyChanged(schedulerStates[schedulerEntityReference], new PropertyChangedEventArgs("DataValue"));
                        }
                        EvaluateCommandExecution(mon, schedulerEntityReference);
                    });
                }
            }
        }

        void UnsubscribeServerSession()
        {
            if (!bSubscribed)
                return;
            bSubscribed = false;

            opcuaEntityReferences.Keys.ToList().ForEach(schedulerEntityReference => 
            {
                schedulerEntityReference.PropertyChanged -= serverEntityReference_PropertyChanged;
                schedulerEntityReference.SetInUse(entityReference, false);
            });
            opcuaEntityReferences.Clear();

            schedulerStates.Values.ToList().ForEach(monitoredItem =>
            {
                monitoredItem.PropertyChanged -= schedulerState_PropertyChanged;
            });
            schedulerStates.Clear();

            enabledStates.Values.ToList().ForEach(monitoredItem =>
            {
                monitoredItem.PropertyChanged -= enabledState_PropertyChanged;
            });
            enabledStates.Clear();

            executionOffStates.Values.ToList().ForEach(monitoredItem =>
            {
                monitoredItem.PropertyChanged -= executeoffState_PropertyChanged;
            });
            executionOffStates.Clear();
        }

        void PrepareCommandsExecution(string sessionName)
        {
            commandsExecuter.Values.ToList().ForEach(executer => executer.PrepareExecution(sessionName));
        }

        void TerminateCommandsExecution()
        {
            commandsExecuter.Values.ToList().ForEach(executer => executer.TerminateExecution());
        }
        #endregion

        #region IDisposable
        bool bDisposed;
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;

            lock (lockObject)
            {
                UnsubscribeServerSession();
                TerminateCommandsExecution();

                commandsExecuter.Clear();
            }
        }
        #endregion
    }
}
