using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommandManager;
using log4net;
using Opc.Ua;
using OPCUAViewModel;
using UFInterfaces;
#if !NET_STANDARD
using System.Windows.Threading;
using UFInterfaces.Commandable;
using System.Windows.Media;
using System.Windows.Controls;
#endif
using ViewModelLib;
using Utilities;
using DocumentManager.ComponentService;
using System.Threading;
using System.ComponentModel;
using UFEventEditor.ComponentService;
using ExpressionManager;

namespace UFEventModel
{
    public class EventObject :
#if !NET_STANDARD
        ICommandable, 
#endif
        IEntityReference, IDisposable
    {
        #region Data
        bool TagErrorLogDone = false;
        bool EnableErrorLogDone = false;
        bool ValueErrorLogDone = false;
        bool initialTimeoutChecked = false;
        IEventEditorManager eventManager;

        ExpressionEntity expressionEntity;
        ExpressionEntity enableExpressionEntity;
        ExpressionEntity valueExpressionEntity;
        #endregion

        #region Properties
        public string Name { get; set; }
        public Guid NodeId { get; set; }
        public bool Enable { get; set; }
        public EventType Type { get; set; }
        public OPCUAEntityReference Tag { get; set; }
        public OPCUAEntityReference EnableTag { get; set; }
        public OPCUAEntityReference ValueTag { get; set; }
        public ConditionType ConditionType { get; set; }
        public double ActivationValue { get; set; }
        public string ActivationStringValue { get; set; }

        public ScheduleType SchedType { get; set; }
        public DateTime Time { get; set; }
        public DateTime Date { get; set; }

        public EventWaitHandle FireSchedule { get; set; }

        object lockObject = new object();

#if !NET_STANDARD
        static readonly ILog log = LogManager.GetLogger(Properties.Resources.EventLog);
#else
        static readonly ILog log = LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), Properties.Resources.EventLog);
#endif

        public String Expression { get; set; }
        public String EnableExpression { get; set; }
        public String ValueExpression { get; set; }

        public String lastExpressionError { get; set; }
        #endregion

        #region Ctors
        public EventObject()
        { 
        }
        #endregion

        #region IDisposable Members
        public void Dispose()
        {
            if (Tag != null && Type == EventType.Tag)
                Tag.SetInUse(this, false);

            if (EnableTag != null)
                EnableTag.SetInUse(this, false);

            if (ValueTag != null)
                ValueTag.SetInUse(this, false);

            if (observer != null)
            {
                observer.Dispose();
                observer = null;
            }

            if (observerMonitoredModel != null)
            {
                observerMonitoredModel.Dispose();
                observerMonitoredModel = null;
            }

            if (observerEnable != null)
            {
                observerEnable.Dispose();
                observerEnable = null;
            }

            if (observerEnableData != null)
            {
                observerEnableData.Dispose();
                observerEnableData = null;
            }

            if (observerValue != null)
            {
                observerValue.Dispose();
                observerValue = null;
            }

            if (observerValueData != null)
            {
                observerValueData.Dispose();
                observerValueData = null;
            }

            var listCommands = RuntimeCommandList as CommandManagerList;
            if (listCommands != null)
            {
                listCommands.ForEach(command =>
                {
                    command.Terminate();
                });
            }
            FreeRuntimeCommandList();
        }
        #endregion

        #region Methods
        PropertyObserver<OPCUAEntityReference> observer;
        PropertyObserver<MonitoredItemViewModel> observerMonitoredModel;
        PropertyObserver<OPCUAEntityReference> observerEnable;
        PropertyObserver<MonitoredItemViewModel> observerEnableData;
        PropertyObserver<OPCUAEntityReference> observerValue;
        PropertyObserver<MonitoredItemViewModel> observerValueData;

        PropertyObserver<MonitoredItemViewModel> observerExpressionTempVariable;
        bool firstval = true;
        public bool PrepareExecution(String sessionname, IDocument parent)
        {
            if (parent != null)
                eventManager = parent.GetService(typeof(IEventEditorManager)) as IEventEditorManager;

            observer = new PropertyObserver<OPCUAEntityReference>(Tag);

            observer.RegisterHandler(n => n.MonitoredItemViewModel, n =>
            {
                observer.UnregisterHandler(p => p.MonitoredItemViewModel);

                if (expressionEntity != null)
                {
                    expressionEntity.ParserError -= ExpressionEntity_ParserError;
                    expressionEntity.ExecutionError -= ExpressionEntity_ExecutionError;
                    expressionEntity.Dispose();
                    expressionEntity = null;
                }

                //ServerInstance.AddToMapNodeIdToNotifications(this);
                if (n.MonitoredItemViewModel != null)
                {
                    MonitoredItemViewModel monitor = n.MonitoredItemViewModel;
                    if(!string.IsNullOrEmpty(Expression))
                    {
                        expressionEntity = ExpressionBucket.GetInstance(parent).AddExpression(n.MonitoredItemViewModel, Expression, "");
                        expressionEntity.ParserError += ExpressionEntity_ParserError;
                        expressionEntity.ExecutionError += ExpressionEntity_ExecutionError;
                        monitor = expressionEntity.TempVariable;
                    }
                    CheckIfNeedExecuteEventCommands(monitor);

                    observerMonitoredModel =
                        new PropertyObserver<MonitoredItemViewModel>(monitor);
                    observerMonitoredModel.RegisterHandler(m => m.DataValue, m =>
                    {
                        CheckIfNeedExecuteEventCommands(m);
                        if (StatusCode.IsGood(m.DataValue.StatusCode))
                        {
                            if (TagErrorLogDone)
                            {
                                log.Info(string.Format(Properties.Resources.TagRestored, Tag.HumanReadable));
                                TagErrorLogDone = false;
                            }
                        }
                        else if (initialTimeoutChecked && !TagErrorLogDone)
                        {
                            log.Error(string.Format(Properties.Resources.TagInvalid, Tag.HumanReadable));
                            TagErrorLogDone = true;
                        }
                    });
                }
            });

            try
            {
                Tag.Resolve(sessionname, parent);
                Tag.SetInUse(this, true);
            }
            catch (Exception e)
            {
                return false;
            }


            return PrepareExecutionEnable(sessionname, parent);
        }

        void ExpressionEntity_ParserError(object sender, EventArgs e)
        {
            OnParserError((ExpressionEntity)sender);
        }

        public event EventHandler<ExpressionEntity> ParserError;
        protected void OnParserError(ExpressionEntity expressionEntity)
        {
            var t = ParserError;
            if (t != null)
                t(this, expressionEntity);
        }

        void ExpressionEntity_ExecutionError(object sender, EventArgs e)
        {
            OnExecutionErrorError((ExpressionEntity)sender);
        }

        public event EventHandler<ExpressionEntity> ExecutionError;
        protected void OnExecutionErrorError(ExpressionEntity expressionEntity)
        {
            var t = ExecutionError;
            if (t != null)
                t(this, expressionEntity);
        }

        public bool PrepareExecutionEnable(String sessionname, IDocument parent)
        {
            if (parent != null)
                eventManager = parent.GetService(typeof(IEventEditorManager)) as IEventEditorManager;

            if (EnableTag != null && EnableTag.IsValid)
            {
                observerEnable = new PropertyObserver<OPCUAEntityReference>(EnableTag);
                observerEnable.RegisterHandler(n => n.MonitoredItemViewModel, n =>
                {
                    observerEnable.UnregisterHandler(p => p.MonitoredItemViewModel);
                    if (enableExpressionEntity != null)
                    {
                        enableExpressionEntity.ParserError -= ExpressionEntity_ParserError;
                        enableExpressionEntity.ExecutionError -= ExpressionEntity_ExecutionError;
                        enableExpressionEntity.Dispose();
                        enableExpressionEntity = null;
                    }
                    if (n.MonitoredItemViewModel != null)
                    {
                        MonitoredItemViewModel monitor = n.MonitoredItemViewModel;
                        if(!string.IsNullOrEmpty(EnableExpression))
                        {
                            enableExpressionEntity = ExpressionBucket.GetInstance(parent).AddExpression(n.MonitoredItemViewModel, EnableExpression, "");
                            enableExpressionEntity.ParserError += ExpressionEntity_ParserError;
                            enableExpressionEntity.ExecutionError += ExpressionEntity_ExecutionError;
                            monitor = enableExpressionEntity.TempVariable;
                        }
                        
                        observerEnableData =
                        new PropertyObserver<MonitoredItemViewModel>(monitor);
                        observerEnableData.RegisterHandler(m => m.DataValue, m =>
                        {
                            if(StatusCode.IsGood(m.DataValue.StatusCode))
                            {
                                if (EnableErrorLogDone)
                                {
                                    log.Info(string.Format(Properties.Resources.EnableTagRestored, EnableTag.HumanReadable));
                                    EnableErrorLogDone = false;
                                    if (Type == EventType.Tag && Tag.MonitoredItemViewModel != null)
                                        CheckIfNeedExecuteEventCommands((expressionEntity != null ?
                                            expressionEntity.TempVariable : Tag.MonitoredItemViewModel));
                                }
                            }
                            else if (initialTimeoutChecked && !EnableErrorLogDone)
                            {
                                log.Error(string.Format(Properties.Resources.EnableTagInvalid, EnableTag.HumanReadable));
                                EnableErrorLogDone = true;
                            }
                        });
                    }
                });
                
                try
                {
                    EnableTag.Resolve(sessionname, parent);
                    EnableTag.SetInUse(this, true);
                }
                catch (Exception e)
                {
                    return false;
                }
            }
            if (ValueTag != null && ValueTag.IsValid)
            {
                observerValue = new PropertyObserver<OPCUAEntityReference>(ValueTag);
                observerValue.RegisterHandler(n => n.MonitoredItemViewModel, n =>
                {
                    observerValue.UnregisterHandler(p => p.MonitoredItemViewModel);

                    if (valueExpressionEntity != null)
                    {
                        valueExpressionEntity.ParserError -= ExpressionEntity_ParserError;
                        valueExpressionEntity.ExecutionError -= ExpressionEntity_ExecutionError;
                        valueExpressionEntity.Dispose();
                        valueExpressionEntity = null;
                    }

                    if (n.MonitoredItemViewModel != null)
                    {
                        MonitoredItemViewModel monitor = n.MonitoredItemViewModel;
                        if(!string.IsNullOrEmpty(ValueExpression))
                        {
                            valueExpressionEntity = ExpressionBucket.GetInstance(parent).AddExpression(n.MonitoredItemViewModel, ValueExpression, "");
                            valueExpressionEntity.ParserError += ExpressionEntity_ParserError;
                            valueExpressionEntity.ExecutionError += ExpressionEntity_ExecutionError;
                            monitor = valueExpressionEntity.TempVariable;
                        }
                        
                        observerValueData = new PropertyObserver<MonitoredItemViewModel>(monitor);
                        observerValueData.RegisterHandler(m => m.DataValue, m =>
                        {
                            if (StatusCode.IsGood(m.DataValue.StatusCode))
                            {
                                if (ValueErrorLogDone)
                                {
                                    log.Info(string.Format(Properties.Resources.ValueTagRestored, ValueTag.HumanReadable));
                                    ValueErrorLogDone = false;
                                    if (Type == EventType.Tag && Tag.MonitoredItemViewModel != null)
                                        CheckIfNeedExecuteEventCommands((expressionEntity != null ? 
                                            expressionEntity.TempVariable : Tag.MonitoredItemViewModel));
                                }
                            }
                            else if (initialTimeoutChecked && !ValueErrorLogDone)
                            {
                                log.Error(string.Format(Properties.Resources.ValueTagInvalid, ValueTag.HumanReadable));
                                ValueErrorLogDone = true;
                            }
                        });
                    }
                });

                try
                {
                    ValueTag.Resolve(sessionname, parent);
                    ValueTag.SetInUse(this, true);
                }
                catch (Exception e)
                {
                    return false;
                }
            }
            return true;
        }

        void CheckIfNeedExecuteEventCommands(MonitoredItemViewModel m)
        {
            if (m.DataValue != null && DataValue.IsGood(m.DataValue))
            {
                if (!firstval)
                    CheckIfNeedExecuteEventCommands(m.DataValue);
                else
                {
                    firstval = false;
                    oldVal = (m.DataValue.Value);
                }
            }
        }
        
        object oldVal;
        bool go = false;
        bool executed = false;
        void CheckIfNeedExecuteEventCommands(DataValue val)
        {
            go = false;

            if (EnableTag != null)
            {
                DataValue dv = null;
                try
                {
                    if (enableExpressionEntity != null && enableExpressionEntity.TempVariable != null)
                        dv = enableExpressionEntity.TempVariable.DataValue;
                    else if (EnableTag.MonitoredItemViewModel != null)
                        dv = EnableTag.MonitoredItemViewModel.DataValue;

                    if (dv == null ||
                       !StatusCode.IsGood(dv.StatusCode) ||
                        Convert.ToBoolean(dv.Value) != true
                       )
                    {
                        //Event disabled
                        return;
                    }
                }
                catch (Exception enEx)
                {
                    log.Error(string.Format(Properties.Resources.EnableTagException, EnableTag.HumanReadable, enEx.Message));
                    return;
                }
            }


            lock (lockObject)
            {
                if (TypeInfo.IsNumericType(val.WrappedValue.TypeInfo.BuiltInType) || val.WrappedValue.TypeInfo.BuiltInType == BuiltInType.Boolean)
                {
                    double doubleValue;
                    double olddouble;

                    try
                    {
                        doubleValue = Convert.ToDouble(val.Value);
                        olddouble = Convert.ToDouble(oldVal);
                    }
                    catch (Exception ex)
                    {
                        log.Error(string.Format(Properties.Resources.EventTagException, Tag.HumanReadable, ex.Message));
                        return;
                    }
                    

                    double activationValue = ActivationValue;

                    if (ValueTag != null)
                    {
                        DataValue dv = null;
                        if (valueExpressionEntity != null && valueExpressionEntity.TempVariable != null)
                        {
                            dv = valueExpressionEntity.TempVariable.DataValue;
                        }
                        else if(ValueTag.MonitoredItemViewModel != null)
                            dv = ValueTag.MonitoredItemViewModel.DataValue;

                        try
                        {
                            activationValue = Convert.ToDouble(dv.Value);
                        }
                        catch (Exception ex)
                        {
                            if(ConditionType != ConditionType.OnChange)
                            {
                                log.Error(string.Format(Properties.Resources.ValueTagException, ValueTag.HumanReadable, ex.Message));
                                return;
                            }
                        }
                    }
                    switch (ConditionType)
                    {
                        case UFEventModel.ConditionType.OnChange:
                            if (doubleValue != olddouble)
                            {
                                go = true;
                                executed = false;
                            }
                            break;
                        case UFEventModel.ConditionType.Equals:
                            go = doubleValue != olddouble && doubleValue == activationValue;
                            break;
                        case UFEventModel.ConditionType.GreaterThan:
                            go = doubleValue != olddouble && doubleValue > activationValue;
                            break;
                        case UFEventModel.ConditionType.GreaterThanOrEqual:
                            go = doubleValue != olddouble && doubleValue >= activationValue;
                            break;
                        case UFEventModel.ConditionType.LessThan:
                            go = doubleValue != olddouble && doubleValue < activationValue;
                            break;
                        case UFEventModel.ConditionType.LessThanOrEqual:
                            go = doubleValue != olddouble && doubleValue <= activationValue;
                            break;
                        case UFEventModel.ConditionType.NotEqual:
                            go = doubleValue != olddouble && doubleValue != activationValue;
                            break;
                    }
                    if (ConditionType != UFEventModel.ConditionType.OnChange)
                    {
                        if (!go)
                            executed = false;
                    }
                }
                else if (val.WrappedValue.TypeInfo.BuiltInType == BuiltInType.String)// && ConditionType == UFEventModel.ConditionType.OnChange)
                {
                    string activationStringValue = ActivationStringValue;

                    if (ValueTag != null)
                    {
                        try
                        {
                            activationStringValue = Convert.ToString(ValueTag.MonitoredItemViewModel.DataValue.Value);
                        }
                        catch (Exception ex)
                        {
                            if (ConditionType != ConditionType.OnChange)
                            {
                                log.Error(string.Format(Properties.Resources.ValueTagException, ValueTag.HumanReadable, ex.Message));
                                return;
                            }
                        }
                    }
                    switch (ConditionType)
                    {
                        case UFEventModel.ConditionType.OnChange:
                            //System.Diagnostics.Trace.TraceInformation("Event String {0}", val.Value.ToString());
                            if ((oldVal == null && val.Value != null) || oldVal.ToString() != val.Value.ToString())
                            {
                                go = true;
                                executed = false;
                            }
                            break;
                        case UFEventModel.ConditionType.Equals:
                            //System.Diagnostics.Trace.TraceInformation("Event String {0}", val.Value.ToString());
                            if ((oldVal == null && val.Value != null && val.Value.ToString() == ActivationStringValue) || 
                                (oldVal != null && val.Value != null && oldVal.ToString() != val.Value.ToString() && val.Value.ToString() == activationStringValue))
                            {
                                go = true;
                                executed = false;
                            }
                            break;
                        case UFEventModel.ConditionType.NotEqual:
                            //System.Diagnostics.Trace.TraceInformation("Event String {0}", val.Value.ToString());
                            if ((oldVal == null && val.Value != null && val.Value.ToString() == ActivationStringValue) ||
                                (oldVal != null && val.Value != null && oldVal.ToString() != val.Value.ToString() && val.Value.ToString() != activationStringValue))
                            {
                                go = true;
                                executed = false;
                            }
                            break;
                    }
                }
                oldVal = val.Value;

                if (FireSchedule != null)
                    FireSchedule.Set();
            }
        }
        private string _LastMessage = string.Empty;
        public string LastMessage { get { return _LastMessage; } set { _LastMessage = value; } }
        bool launchAllCommands()
        {
            bool bRet = true;
            foreach (CommandManager.CommandManager command in RuntimeCommandList)
            {

#if NET_STANDARD
                if (eventManager != null && command.IsUICommand())
                {
                    command.OpcuaEntityReference = null;
                    var commandList = new CommandManagerList();
                    commandList.Add(command);
                    var json = commandList.ToJSON();
                    json = json.Replace(":http:\\/\\/progea.com", "");
                    json = json.Replace(":NaN", ":0");
                    var eventArgs = new EventEventArgs()
                    {
                        JsonCommand = json
                    };

                    eventManager.OnFireUIEvent(this, eventArgs);
                    if (eventArgs.bExecuted)
                        continue;
                }
#endif              

                if (command.CanExecute())
                {
                    try
                    {
                        command.BlindExecute();
                    }
                    catch (Exception ex)
                    {
                        LastMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                        log.Error(string.Format(Properties.Resources.ErrorExecutingCommand, command.Name, Name, LastMessage));
                        bRet = false;
                    }
                }
            }
            return bRet;
        }
        DateTime lastexecution = DateTime.Now;
        bool bEnableExceptionLogged = false;
        public bool Schedule()
        {
            bool ret = true;

            if (EnableTag != null)
            {
                try
                {
                    DataValue dv = null;
                    if (enableExpressionEntity != null && enableExpressionEntity.TempVariable != null)
                        dv = enableExpressionEntity.TempVariable.DataValue;
                    else if (EnableTag.MonitoredItemViewModel != null)
                        dv = EnableTag.MonitoredItemViewModel.DataValue;
                    if (dv == null ||
                        !StatusCode.IsGood(dv.StatusCode) ||
                        Convert.ToBoolean(dv.Value) != true
                        )
                    {
                        //Event disabled
                        return ret;
                    }
                }
                catch (Exception enEx)
                {
                    if(!bEnableExceptionLogged)
                    {
                        log.Error(string.Format(Properties.Resources.EnableTagException, EnableTag.HumanReadable, enEx.Message));
                        bEnableExceptionLogged = true;
                    }
                    
                    return ret;
                }
            }
            bEnableExceptionLogged = false;

            if (Type == EventType.Tag)
            {
                if (go && !executed)
                {
                    executed = true;
                    launchAllCommands();
                }
            }
            else
            {
                var present = DateTime.Now;
                DateTime on = Time;
                switch (SchedType)
                {
                    case ScheduleType.everyMinute:
                        on = new DateTime(present.Year, present.Month, present.Day, present.Hour, present.Minute, Time.Second);
                        break;
                    case ScheduleType.everyHour:
                        on = new DateTime(present.Year, present.Month, present.Day, present.Hour, Time.Minute, Time.Second);
                        break;
                    case ScheduleType.everyDay:
                        on = new DateTime(present.Year, present.Month, present.Day, Time.Hour, Time.Minute, Time.Second);
                        break;
                    case ScheduleType.everyMonday:
                        on = GetTimeForEveryDay(present, Time, DayOfWeek.Monday);
                        break;
                    case ScheduleType.everyTuesday:
                        on = GetTimeForEveryDay(present, Time, DayOfWeek.Tuesday);
                        break;
                    case ScheduleType.everyWednesday:
                        on = GetTimeForEveryDay(present, Time, DayOfWeek.Wednesday);
                        break;
                    case ScheduleType.everyThursday:
                        on = GetTimeForEveryDay(present, Time, DayOfWeek.Thursday);
                        break;
                    case ScheduleType.everyFriday:
                        on = GetTimeForEveryDay(present, Time, DayOfWeek.Friday);
                        break;
                    case ScheduleType.everySaturday:
                        on = GetTimeForEveryDay(present, Time, DayOfWeek.Saturday);
                        break;
                    case ScheduleType.everySunday:
                        on = GetTimeForEveryDay(present, Time, DayOfWeek.Sunday);
                        break;
                    case ScheduleType.everyMonth:
                        on = new DateTime(present.Year, present.Month, Date.Day, Time.Hour, Time.Minute, Time.Second);
                        break;
                    case ScheduleType.everyYear:
                        on = new DateTime(present.Year, Date.Month, Date.Day, Time.Hour, Time.Minute, Time.Second);
                        break;
                }
                if (present > on && on > lastexecution)
                {
                    lastexecution = on;
                    ret = launchAllCommands();
                }
            }

            return ret;
        }

        DateTime GetTimeForEveryDay(DateTime p, DateTime t, DayOfWeek d)
        {
            DateTime dt = new DateTime(p.Year, p.Month, p.Day, t.Hour, t.Minute, t.Second);
            if (p.DayOfWeek != d)
                dt = dt.AddDays((p.DayOfWeek <= d ? d - p.DayOfWeek : 7 - (p.DayOfWeek - d)));
            return dt;
        }

        public void CheckInitialConnectionTimeout()
        {
            if (EnableTag != null)
            {
                if (EnableTag.MonitoredItemViewModel == null ||
                    EnableTag.MonitoredItemViewModel.DataValue == null || !StatusCode.IsGood(EnableTag.MonitoredItemViewModel.DataValue.StatusCode))
                {
                    log.Error(string.Format(Properties.Resources.EnableTagInvalid, EnableTag.HumanReadable));
                    EnableErrorLogDone = true;
                }
            }

            if (Type == EventType.Tag && ValueTag != null)
            {
                if (ValueTag.MonitoredItemViewModel == null ||
                    ValueTag.MonitoredItemViewModel.DataValue == null || !StatusCode.IsGood(ValueTag.MonitoredItemViewModel.DataValue.StatusCode))
                {
                    log.Error(string.Format(Properties.Resources.ValueTagInvalid, ValueTag.HumanReadable));
                    ValueErrorLogDone = true;
                }
            }

            if (Type == EventType.Tag && Tag != null)
            {
                if (Tag.MonitoredItemViewModel == null ||
                    Tag.MonitoredItemViewModel.DataValue == null || !StatusCode.IsGood(Tag.MonitoredItemViewModel.DataValue.StatusCode))
                {
                    log.Error(string.Format(Properties.Resources.TagInvalid, Tag.HumanReadable));
                    TagErrorLogDone = true;
                }
            }
            initialTimeoutChecked = true;
        }
#endregion

#region runtime commands

        CommandManagerList runtimeCommandList;
        public IEnumerable RuntimeCommandList
        {
            get
            {
                if (runtimeCommandList == null)
                {
                    if (listCommands != null)
                    {
                        var sourceString = listCommands.ToXml();
                        runtimeCommandList = sourceString.FromXml<CommandManagerList>();
                    }
                    else
                        runtimeCommandList = new CommandManagerList();
                }

                return runtimeCommandList;
            }
        }

        public void FreeRuntimeCommandList()
        {
            if (runtimeCommandList == null)
                return;
            runtimeCommandList.Clear();
            runtimeCommandList = null;
        }

#endregion
#region ICommandable Members

        CommandManagerList listCommands;
        public IEnumerable CommandList
        {
            get
            {
                if (listCommands == null)
                    listCommands = new CommandManagerList();
                return listCommands;
            }
            set
            {
                if (listCommands == null)
                    listCommands = new CommandManagerList();
                listCommands.Clear();
                foreach (var v in value)
                    listCommands.Add(v as CommandManager.CommandManager);
            }
        }

#if !NET_STANDARD
        [Browsable(false)]
        public bool WebHMISupported
        {
            get
            {
                return true;
            }
        }
#endif
#endregion

#region IEntityReference Members

        public ImageSource CollapsedImageSource
        {
            get { throw new NotImplementedException(); }
        }

        public ImageSource ExpandedImageSource
        {
            get { throw new NotImplementedException(); }
        }

        public ContextMenu contextMenu
        {
            get { throw new NotImplementedException(); }
        }

        public object Tooltip
        {
            get { throw new NotImplementedException(); }
        }

        public object ContainedObject
        {
            get { return null; }
        }

        public object EntityParent
        {
            get { throw new NotImplementedException(); }
        }

        public string TypeDefinitionString
        {
            get { throw new NotImplementedException(); }
        }

#endregion
    }
}
