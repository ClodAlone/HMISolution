using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MSModel;
using System.Data;
using Opc.Ua;
#if !NET_CORE
using System.Windows.Media;
using System.Windows.Controls;
#endif
using OPCUAViewModel;
using ViewModelLib;
using UFInterfaces;
using log4net;
using System.Globalization;
using DevExpress.Xpo;
using XpoHelpers;
using UFUAServerBase;
using System.ComponentModel;
using Utilities;
using Utilities.Logger;

namespace MSServer
{
    public enum ActivationInterval 
    {
        Invalid,
        On,
        Inside, 
        Outside
    }

    public class ScheduledEvent : IDisposable, IEntityReference
    {
        bool executed = false;
        bool inerror = false;
        DateTime StartConnection;
        bool connected = false;

#region Properties
        private string _Name;
        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
            }
        }

        private NodeId _NodeId;
        public NodeId NodeId
        {
            get
            {
                return _NodeId;
            }
            set
            {
                _NodeId = value;
            }
        }
        private bool _Enable;
        public bool Enable
        {
            get
            {
                return _Enable;
            }
            set
            {
                _Enable = value;
            }
        }
        private bool _RuntimeEnable;
        public bool RuntimeEnable
        {
            get { return _RuntimeEnable; }
            set
            {
                _RuntimeEnable = value;
            }
        }

        private ScheduleType _Type;
        public ScheduleType Type
        {
            get
            {
                return _Type;
            }
            set
            {
                _Type = value;
            }
        }

        private DateTime _Time;
        public DateTime Time
        {
            get
            {
                return _Time;
            }
            set
            {
                _Time = value;
            }
        }
        private DateTime _Date;
        public DateTime Date
        {
            get
            {
                return _Date;
            }
            set
            {
                _Date = value;
            }
        }

        private DateTime _TimeOff;
        public DateTime TimeOff
        {
            get
            {
                return _TimeOff;
            }
            set
            {
                _TimeOff = value;
            }
        }
        private DateTime _DateOff;
        public DateTime DateOff
        {
            get
            {
                return _DateOff;
            }
            set
            {
                _DateOff = value;
            }
        }
        private List<SchedulingDate> _WeeklyCalendar = new List<SchedulingDate>();
        public List<SchedulingDate> WeeklyCalendar
        {
            get
            {
                return _WeeklyCalendar;
            }
        }

        private List<SchedulingDate> _Calendar = new List<SchedulingDate>();
        public List<SchedulingDate> Calendar
        {
            get
            {
                return _Calendar;
            }
        }
        private List<SchedulingDate> _ExceptionsCalendar = new List<SchedulingDate>();
        public List<SchedulingDate> ExceptionsCalendar
        {
            get
            {
                return _ExceptionsCalendar;
            }
        }
        private MSUANodeManager _NodeManager;
        public MSUANodeManager nodeManager
        {
            get { return _NodeManager; }
            set
            {
                _NodeManager = value;
            }
        }

        private string _ValueOn;
        public string ValueOn
        {
            get { return _ValueOn; }
            set
            {
                _ValueOn = value;
            }
        }

        private string _ValueOff;
        public string ValueOff
        {
            get { return _ValueOff; }
            set
            {
                _ValueOff = value;
            }
        }

        private string _ExceptionValueOn;
        public string ExceptionValueOn
        {
            get { return _ExceptionValueOn; }
            set
            {
                _ExceptionValueOn = value;
            }
        }

        private string _ExceptionValueOff;
        public string ExceptionValueOff
        {
            get { return _ExceptionValueOff; }
            set
            {
                _ExceptionValueOff = value;
            }
        }

        private bool _ExecOnAtStartup;
        public bool ExecOnAtStartup
        {
            get
            {
                return _ExecOnAtStartup;
            }
            set
            {
                _ExecOnAtStartup = value;
            }
        }

        private bool _ExecOffAtStartup;
        public bool ExecOffAtStartup
        {
            get
            {
                return _ExecOffAtStartup;
            }
            set
            {
                _ExecOffAtStartup = value;
            }
        }

        private bool _HasCommandOn;
        public bool HasCommandOn
        {
            get { return _HasCommandOn; }
            internal set { _HasCommandOn = value; }
        }
        private bool _HasCommandOff;
        public bool HasCommandOff
        {
            get { return _HasCommandOff; }
            internal set { _HasCommandOff = value; }
        }

        private bool _HasCommandOnEx;
        public bool HasCommandOnEx
        {
            get { return _HasCommandOnEx; }
            internal set { _HasCommandOnEx = value; }
        }
        private bool _HasCommandOffEx;
        public bool HasCommandOffEx
        {
            get { return _HasCommandOffEx; }
            internal set { _HasCommandOffEx = value; }
        }
        #endregion

        #region OpcUaEntityReference
        private string _EnableVarName;
        public string EnableVarName
        {
            get { return _EnableVarName; }
            set
            {
                _EnableVarName = value;
            }
        }
        public OPCUAEntityReference OPCEnableVar;
        private string _ScheduleItemName;
        public string ScheduleItemName
        {
            get
            {
                return _ScheduleItemName;
            }
            set
            {
                ScheduleItemName = _ScheduleItemName;
            }
        }

        public OPCUAEntityReference OPCItem;
        readonly Object lockObject = new Object();

        PropertyObserver<OPCUAEntityReference> observer;
        PropertyObserver<MonitoredItemViewModel> observerMonitoredModel;
        PropertyObserver<OPCUAEntityReference> observerEnable;
        PropertyObserver<MonitoredItemViewModel> observerEnableMonitoredModel;
        public bool PrepareExecution(String sessionname)
        {
            SetUp();
            connected = OPCItem == null;
            if (OPCItem != null
#if !NET_CORE
                && OPCItem.IsValid
#endif
                )
            {
                observer = new PropertyObserver<OPCUAEntityReference>(OPCItem);

                observer.RegisterHandler(n => n.MonitoredItemViewModel, n =>
                {
                    connected = true;
                    observer.UnregisterHandler(p => p.MonitoredItemViewModel);
                    observerMonitoredModel =
                        new PropertyObserver<MonitoredItemViewModel>(n.MonitoredItemViewModel);
                    observerMonitoredModel.RegisterHandler(m => m.DataValue, m =>
                    {

                        if (m.DataValue != null)
                        {
                            //UpdateNotificationStatus(m.DataValue);
                        }
                    });

                });

                OPCItem.Resolve(sessionname);
                OPCItem.SetInUse(this, true);
                StartConnection = DateTime.Now;
            }

            RuntimeEnable = OPCEnableVar == null;
            if (OPCEnableVar != null
#if !NET_CORE
                && OPCEnableVar.IsValid
#endif
                )
            {
                RuntimeEnable = false;
                observerEnable = new PropertyObserver<OPCUAEntityReference>(OPCEnableVar);
                observerEnable.RegisterHandler(n => n.MonitoredItemViewModel, n =>
                {
                    observerEnable.UnregisterHandler(p => p.MonitoredItemViewModel);
                    observerEnableMonitoredModel =
                        new PropertyObserver<MonitoredItemViewModel>(n.MonitoredItemViewModel);
                    observerEnableMonitoredModel.RegisterHandler(m => m.DataValue, m =>
                    {

                        if (m.DataValue != null)
                        {
                            UpdateRuntimeEnable(m.DataValue);
                            nodeManager.UpdateSchedulerEnableState(NodeId, new Opc.Ua.DataValue(Enable && RuntimeEnable));
                        }
                    });

                });
                OPCEnableVar.Resolve(sessionname);
                OPCEnableVar.SetInUse(this, true);
            }

            nodeManager.UpdateSchedulerEnableState(NodeId, new Opc.Ua.DataValue(Enable && RuntimeEnable));
            SetExecuteCommandOff();
            return true;
        }

        public void SetExecuteCommandOff()
        {
            nodeManager.UpdateSchedulerExecuteOffCommandState(NodeId, new Opc.Ua.DataValue((activeon == ActivationInterval.On ? false : true)));
        }
        ActivationInterval activeon = ActivationInterval.Invalid;
        ActivationInterval lastactiveon = ActivationInterval.Invalid;
        public void SetUp()
        {
            switch (Type)
            {
                case ScheduleType.everyMinute:
                    if (Time.Second == TimeOff.Second)
                        activeon = ActivationInterval.On;
                    else if (Time.Second < TimeOff.Second)
                        activeon = ActivationInterval.Inside;
                    else
                        activeon = ActivationInterval.Outside;
                    break;
                case ScheduleType.everyHour:
                    if (Time.Minute == TimeOff.Minute)
                        activeon = ActivationInterval.On;
                    else if (Time.Minute < TimeOff.Minute)
                        activeon = ActivationInterval.Inside;
                    else
                        activeon = ActivationInterval.Outside;
                    break;
                case ScheduleType.everyDay:
                case ScheduleType.everyMonday:
                case ScheduleType.everyTuesday:
                case ScheduleType.everyWednesday:
                case ScheduleType.everyThursday:
                case ScheduleType.everyFriday:
                case ScheduleType.everySaturday:
                case ScheduleType.everySunday:
                    if (Time == TimeOff)
                        activeon = ActivationInterval.On;
                    else if (Time < TimeOff)
                        activeon = ActivationInterval.Inside;
                    else
                        activeon = ActivationInterval.Outside;
                    break;
            }
        }
#endregion

#region Methods

        void UpdateRuntimeEnable(DataValue value)
        {
            if (value.Value == null)
                return;
            lock (lockObject)
            {
                if (TypeInfo.IsNumericType(value.WrappedValue.TypeInfo.BuiltInType) || value.WrappedValue.TypeInfo.BuiltInType == BuiltInType.Boolean)
                {
                    var doubleValue = Convert.ToDouble(value.Value);
                    RuntimeEnable = (doubleValue != 0);
                }
            }
        }
        bool WriteOPCItemValue(object value)
        {
            if (OPCItem == null)
                return true;
            
            if (OPCItem.MonitoredItemViewModel != null)
            {
                try
                {

                    OPCItem.MonitoredItemViewModel.WriteValue(value);
                    inerror = false;
                    return true;
                }
                catch (Exception ex)
                { 
                    //log something
                    if (!inerror)
                    {
#if !NET_CORE
                        var syslog = LogManager.GetLogger(Properties.Resources.LoggerSource);
#else
                        var syslog = LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), Properties.Resources.LoggerSource);
#endif
                        syslog.Error(String.Format(Properties.Resources.WriteVariableException, OPCItem.MonitoredItemViewModel.DisplayName), ex);
                        inerror = true;
                    }
                }
            }
            else
            { 
                //log something
                if (!inerror)
                {
                    inerror = true;
#if !NET_CORE
                    var syslog = LogManager.GetLogger(Properties.Resources.LoggerSource);
#else
                    var syslog = LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), Properties.Resources.LoggerSource);
#endif
                    syslog.Error(String.Format(Properties.Resources.UnconnectedVariable, OPCItem.HumanReadable));
                }
            }
            return false;
        }
        DateTime thisdate;

        public void SetThisDate()
        {
            thisdate = new DateTime(Date.Year, Date.Month, Date.Day, Time.Hour, Time.Minute, Time.Second);
        }

        private static int SchedulingDateCompareStart(SchedulingDate x, SchedulingDate y)
        {
            if (x == null)
            {
                if (y == null)
                    return 0; //==
                else
                    return -1;// x < y
            }
            else
            {
                //x!= null
                if (y == null)
                    return 1; //x > y
                else
                {
                    return x.StartDate > y.StartDate ? 1 : -1;
                }
            }
        }

        private bool IsOnHoliday(DateTime present)
        {
            ExceptionsCalendar.Sort(SchedulingDateCompareStart);
            
            bool inside = false;
            foreach (var exc in ExceptionsCalendar)
            {
                if (exc.Type != CalendarItemType.WeekNDate && present >= exc.StartDate && present <= exc.EndDate)
                    return true;
                else if (exc.Type == CalendarItemType.WeekNDate)
                {
                    inside = IsWeekNDateInside(exc, present);
                    if (inside)
                        return true;
                }
            }
            return false;
        }

        private void PerformSchedulerAction(DateTime evalTime, DateTime onTime, DateTime offTime, bool changeperiod)
        {
            if (activeon == ActivationInterval.On)
            {
                if (evalTime >= onTime && changeperiod)
                {
                    if (!executed || dofirstexecution)
                        SetOn(exception: IsOnHoliday(evalTime));
                }
                else
                {
                    if (executed || dofirstexecution)
                        SetOff(false, exception: IsOnHoliday(evalTime));
                }
            }
            else if (activeon == ActivationInterval.Inside)
            {
                if (evalTime >= onTime && evalTime < offTime)
                {
                    if (!executed || dofirstexecution)
                        SetOn(exception: IsOnHoliday(evalTime));
                }
                else
                {
                    if (executed || dofirstexecution)
                        SetOff(exception: IsOnHoliday(evalTime));
                }
            }
            else
            {
                if (evalTime < offTime || evalTime >= onTime)
                {
                    if (!executed || dofirstexecution)
                        SetOn(exception: IsOnHoliday(evalTime));
                }
                else
                {
                    if (executed || dofirstexecution)
                        SetOff(exception: IsOnHoliday(evalTime));
                }
            }
        }

        bool bEnableExceptionLogged = false;
        
        public void Scheduling(DateTime present)
        {
            if (Type != ScheduleType.calendar && Type!= ScheduleType.weeklyPlan && activeon == ActivationInterval.Invalid)
                SetUp();
            bool onHoliday = IsOnHoliday(present);
            bool execSomething = HasCommandOffEx || HasCommandOnEx || 
                !string.IsNullOrEmpty(ExceptionValueOn) || !string.IsNullOrEmpty(ExceptionValueOff);
            if (!Enable || !RuntimeEnable || (onHoliday && !execSomething))
                return;

            if (!connected || (DateTime.Now - StartConnection).TotalSeconds < Properties.Settings.Default.ConnectionDelay)
                return;
            try
            {
                if (OPCEnableVar != null && OPCEnableVar.MonitoredItemViewModel != null && Convert.ToBoolean(OPCEnableVar.MonitoredItemViewModel.DataValue.Value) != true)
                    return;
            }
            catch(Exception ex)
            {
                if(!bEnableExceptionLogged)
                {
#if !NET_CORE
                    var syslog = LogManager.GetLogger(Properties.Resources.LoggerSource);
#else
                    var syslog = LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), Properties.Resources.LoggerSource);
#endif
                    syslog.Error(String.Format(Properties.Resources.EnableVariableException, OPCEnableVar.HumanReadable, ex.Message));
                    bEnableExceptionLogged = true;
                }
                return;
            }

            bEnableExceptionLogged = false;

            DateTime on = Time;
            DateTime off = TimeOff;

            switch (Type)
            { 
                case ScheduleType.everyMinute:
                    on = new DateTime(present.Year, present.Month, present.Day, present.Hour, present.Minute, Time.Second);
                    off = new DateTime(present.Year, present.Month, present.Day, present.Hour, present.Minute, TimeOff.Second);
                    PerformSchedulerAction(present, on, off, present.Minute != LastExecutionTime.Minute);
                    break;
                case ScheduleType.everyHour:
                    on = new DateTime(present.Year, present.Month, present.Day, present.Hour, Time.Minute, Time.Second);
                    off = new DateTime(present.Year, present.Month, present.Day, present.Hour, TimeOff.Minute, TimeOff.Second);
                    PerformSchedulerAction(present, on, off, present.Hour != LastExecutionTime.Hour);
                    break;
                case ScheduleType.everyDay:
                    on = new DateTime(present.Year, present.Month, present.Day, Time.Hour, Time.Minute, Time.Second);
                    off = new DateTime(present.Year, present.Month, present.Day, TimeOff.Hour, TimeOff.Minute, TimeOff.Second);
                    PerformSchedulerAction(present, on, off, present.Day != LastExecutionTime.Day);
                    break;
                case ScheduleType.everyMonday:
                    on = GetTimeForEveryDay(present, Time, DayOfWeek.Monday);
                    off = GetTimeForEveryDay(present, TimeOff, DayOfWeek.Monday);
                    PerformSchedulerAction(present, on, off, present.Day != LastExecutionTime.Day);
                    break;
                case ScheduleType.everyTuesday:
                    on = GetTimeForEveryDay(present, Time, DayOfWeek.Tuesday);
                    off = GetTimeForEveryDay(present, TimeOff, DayOfWeek.Tuesday);
                    PerformSchedulerAction(present, on, off, present.Day != LastExecutionTime.Day);
                    break;
                case ScheduleType.everyWednesday:
                    on = GetTimeForEveryDay(present, Time, DayOfWeek.Wednesday);
                    off = GetTimeForEveryDay(present, TimeOff, DayOfWeek.Wednesday);
                    PerformSchedulerAction(present, on, off, present.Day != LastExecutionTime.Day);
                    break;
                case ScheduleType.everyThursday:
                    on = GetTimeForEveryDay(present, Time, DayOfWeek.Thursday);
                    off = GetTimeForEveryDay(present, TimeOff, DayOfWeek.Thursday);
                    PerformSchedulerAction(present, on, off, present.Day != LastExecutionTime.Day);
                    break;
                case ScheduleType.everyFriday:
                    on = GetTimeForEveryDay(present, Time, DayOfWeek.Friday);
                    off = GetTimeForEveryDay(present, TimeOff, DayOfWeek.Friday);
                    PerformSchedulerAction(present, on, off, present.Day != LastExecutionTime.Day);
                    break;
                case ScheduleType.everySaturday:
                    on = GetTimeForEveryDay(present, Time, DayOfWeek.Saturday);
                    off = GetTimeForEveryDay(present, TimeOff, DayOfWeek.Saturday);
                    PerformSchedulerAction(present, on, off, present.Day != LastExecutionTime.Day);
                    break;
                case ScheduleType.everySunday:
                    on = GetTimeForEveryDay(present, Time, DayOfWeek.Sunday);
                    off = GetTimeForEveryDay(present, TimeOff, DayOfWeek.Sunday);
                    PerformSchedulerAction(present, on, off, present.Day != LastExecutionTime.Day);
                    break;
                case ScheduleType.calendar:
                case ScheduleType.weeklyPlan:
                    {
                        List<SchedulingDate> lista = WeeklyCalendar;
                        
                        if (Type == ScheduleType.calendar)
                            lista = Calendar;
                        
                        lista.Sort(SchedulingDateCompareStart);
                        if (lista != null && lista.Count > 0)
                        {
                            bool inside = false;
                            foreach (var si in lista)
                            { 
                                if(Type == ScheduleType.calendar && si.Type != CalendarItemType.WeekNDate && si.EndDate < present)
                                    continue;

                                activeon = (si.StartDate == si.EndDate ? ActivationInterval.On : ActivationInterval.Invalid);

                                if (si.Type == CalendarItemType.WeekNDate)
                                {
                                    inside = IsWeekNDateInside(si, present);
                                    if (inside)
                                        break;
                                        
                                }
                                else
                                {
                                    if (activeon == ActivationInterval.On)
                                    {
                                        if (CheckStartDate(present, si))
                                        {
                                            inside = true;
                                            break;
                                        }
                                    }
                                    else
                                        if (CheckInsideDates(present, si))
                                    {
                                        inside = true;
                                        break;
                                    }
                                }
                            }
                            if (activeon != lastactiveon)
                                SetExecuteCommandOff();
                            lastactiveon = activeon;
                            

                            if (inside && (!executed || dofirstexecution))
                            { 
                                //setOn
                                SetOn(exception: onHoliday);
#if DEBUG
                                System.Diagnostics.Debug.WriteLine("SetOn holy:{0} time:{1}",
                                    onHoliday, present);
#endif
                            }
                            else if (!inside && (executed || dofirstexecution))
                            { 
                                //setOff
                                SetOff(exception: onHoliday);
#if DEBUG
                                System.Diagnostics.Debug.WriteLine("SetOff holy:{0} time:{1}",
                                    onHoliday, present);
#endif
                            }
                        }
                        else if (dofirstexecution || executed) //no scheduling interval, sure off
                            SetOff(exception: onHoliday);
                        //return;
                    }
                    break;
            }

        }

        bool CheckInsideDates(DateTime present, SchedulingDate si)
        {
            if (si == null)
                return false;
            if (Type == ScheduleType.calendar)
            {
                if (present >= si.StartDate && present <= si.EndDate)
                    return true;
            }
            else if (Type == ScheduleType.weeklyPlan)
            {
                if (
                    (present.DayOfWeek > si.StartDate.DayOfWeek || (present.DayOfWeek == si.StartDate.DayOfWeek && present.TimeOfDay >= si.StartDate.TimeOfDay)) && 
                    (present.DayOfWeek<si.EndDate.DayOfWeek || (present.DayOfWeek == si.EndDate.DayOfWeek && present.TimeOfDay <= si.EndDate.TimeOfDay))
                    )
                    return true;
            }
            return false;
        }
        bool CheckStartDate(DateTime present, SchedulingDate si)
        {
            if (si == null)
                return false;

            if (Type == ScheduleType.calendar)
            {
                if (present >= si.StartDate && LastExecutionTime < si.StartDate)
                    return true;
            }
            else if (Type == ScheduleType.weeklyPlan)
            {
                if ((present.DayOfWeek > si.StartDate.DayOfWeek || (present.DayOfWeek == si.StartDate.DayOfWeek && present.TimeOfDay >= si.StartDate.TimeOfDay)) &&
                    (LastExecutionTime.DayOfWeek < si.StartDate.DayOfWeek || (LastExecutionTime.DayOfWeek == si.StartDate.DayOfWeek && LastExecutionTime.TimeOfDay < si.StartDate.TimeOfDay)))
                        return true;
            }
            return false;
        }
        bool IsWeekNDateInside(SchedulingDate si, DateTime present)
        {
            if (si.Month == CalendarMonthType.Any ||
                                    (si.Month == CalendarMonthType.Even && present.Month % 2 == 0) || //month even
                                    (si.Month == CalendarMonthType.Odd && present.Month % 2 != 0) || //month odd
                                    (si.Month < CalendarMonthType.Odd && ((int)si.Month == present.Month)) //this month
                                    )
            {
                if (si.DayOfMonth != (int)MSModel.MonthDayAny.NoDate)
                {
                    switch (si.DayOfMonth)
                    {
                        case (int)MSModel.MonthDayAny.First:
                            if (present.Day == 1 &&
                                (present.TimeOfDay >= si.StartDate.TimeOfDay && present.TimeOfDay <= si.EndDate.TimeOfDay))
                                return true;
                            break;
                        case (int)MSModel.MonthDayAny.Last:
                            if (present.AddMonths(1).AddDays(-1).Day == present.Day &&
                                (present.TimeOfDay >= si.StartDate.TimeOfDay && present.TimeOfDay <= si.EndDate.TimeOfDay))
                                return true;
                            break;
                        default:
                            {
                                int day = si.DayOfMonth - 2;
                                if (present.Day == day &&
                                    (present.TimeOfDay >= si.StartDate.TimeOfDay && present.TimeOfDay <= si.EndDate.TimeOfDay))
                                    return true;
                            }
                            break;
                    }
                }
                else
                {
                    if (si.WeekOfMonth == CalendarWeekType.Any ||
                    (si.WeekOfMonth == CalendarWeekType.Last && present.GetWeekOfMonth() > DateTime.DaysInMonth(present.Year, (int)si.Month + 1) / 7 + (DateTime.DaysInMonth(present.Year, (int)si.Month + 1) % 7 != 0 ? 1 : 0)) ||
                    (present.GetWeekOfMonth() == (int)si.WeekOfMonth + 1)
                    )
                    {
                        if ((si.DayOfWeek == CalendarDayType.Any || ((int)present.DayOfWeek == (int)si.DayOfWeek))
                            && (present.TimeOfDay >= si.StartDate.TimeOfDay && present.TimeOfDay <= si.EndDate.TimeOfDay))
                            return true;
                    }
                }
            }
            return false;
        }

        DateTime GetTimeForEveryDay(DateTime p, DateTime t, DayOfWeek d)
        {
            DateTime dt = new DateTime(p.Year, p.Month, p.Day, t.Hour, t.Minute, t.Second);
            if (p.DayOfWeek != d)
                dt = dt.AddDays((p.DayOfWeek <= d ? d - p.DayOfWeek : 7 - (p.DayOfWeek - d)));
            return dt;
        }
        void SetOn(bool write = true, bool exception = false)
        {
            string valToWrite = ValueOn;
            if(exception && !string.IsNullOrEmpty(ExceptionValueOn))
                valToWrite = ExceptionValueOn;
            System.Diagnostics.Trace.TraceInformation("{3} - Scheduler SetOn di {0} write:{1} ValueOn:{2}", Name, write, ValueOn, DateTime.Now.ToLongTimeString());
            bool setVar = (write && !string.IsNullOrEmpty(valToWrite));
            dofirstexecution = false;
            Byte bVal = exception ? 
                (Byte)ActionCommandsEventType.CommandsOnEx :
                (Byte)ActionCommandsEventType.CommandsOn;
            nodeManager.UpdateSchedulerVariable(NodeId,
                new Opc.Ua.DataValue() 
                {
                    Value = bVal
                }
                );
            
            executed = (setVar ? WriteOPCItemValue(valToWrite) : true);
            LastExecutionTime = DateTime.Now;
            SaveSchedulerStatus(LastExecutionTime);
            bool execCommands = (!exception && HasCommandOn) || (exception && HasCommandOnEx);
            string evDetails = string.Empty;
            if (!string.IsNullOrEmpty(valToWrite) && execCommands)
            {
                evDetails = string.Format(Properties.Resources.And,
                    string.Format(Properties.Resources.VariableSet, valToWrite),
                    Properties.Resources.CommandListExecuted);
            }
            else if (!string.IsNullOrEmpty(valToWrite))
            {
                evDetails = string.Format(Properties.Resources.VariableSet, valToWrite);
            }
            else if (execCommands)
            {
                evDetails = Properties.Resources.CommandListExecuted;
            }

            nodeManager.MSUANodeManager_SystemEvent(this, new DriverBaseInterfaces.SystemEventArgs()
            {
                sourceNode = ObjectIds.Server,
                sourceName = Properties.Resources.LoggerSource,
                EventName = string.Format(Properties.Resources.SchedulerActivated, Name),
                severity = EventSeverity.Low,
                time = DateTime.UtcNow,
                eventtype = ObjectTypeIds.SystemStatusChangeEventType,
                details = evDetails,
                state = string.Format(Properties.Resources.Active),
                logtype = (int)System.Diagnostics.EventLogEntryType.Information,
                logdestination = (int)LoggerDestination.Scheduler
            });
        }
        void SetOff(bool write = true, bool exception = false)
        {
            System.Diagnostics.Trace.TraceInformation("{3} - Scheduler SetOff di {0} write:{1} ValueOff:{2}", Name, write, ValueOff, DateTime.Now.ToLongTimeString());
            string valToWrite = ValueOff;
            if (exception && !string.IsNullOrEmpty(ExceptionValueOff))
                valToWrite = ExceptionValueOff;
            bool setVar = (write && !string.IsNullOrEmpty(valToWrite));
            dofirstexecution = false;
            Byte bVal = exception ?
                (Byte)ActionCommandsEventType.CommandsOffEx :
                (Byte)ActionCommandsEventType.CommandsOff;
            nodeManager.UpdateSchedulerVariable(NodeId,
                new Opc.Ua.DataValue()
                {
                    Value = bVal
                }
                );
            executed = (setVar ? !WriteOPCItemValue(valToWrite) : false);
            LastExecutionTime = DateTime.Now;
            SaveSchedulerStatus(LastExecutionTime);
            bool execCommands = (!exception && HasCommandOff) || (exception && HasCommandOffEx);
            string evDetails = string.Empty;
            if (!string.IsNullOrEmpty(valToWrite) && execCommands)
            {
                evDetails = string.Format(Properties.Resources.And,
                    string.Format(Properties.Resources.VariableSet, valToWrite),
                    Properties.Resources.CommandListExecuted);
            }
            else if (!string.IsNullOrEmpty(valToWrite))
            {
                evDetails = string.Format(Properties.Resources.VariableSet, valToWrite);
            }
            else if (execCommands)
            {
                evDetails = Properties.Resources.CommandListExecuted;
            }

            nodeManager.MSUANodeManager_SystemEvent(this, new DriverBaseInterfaces.SystemEventArgs()
            {
                sourceNode = ObjectIds.Server,
                sourceName = Properties.Resources.LoggerSource,
                EventName = string.Format(Properties.Resources.SchedulerDeactivated, Name),
                severity = EventSeverity.Low,
                time = DateTime.UtcNow,
                eventtype = ObjectTypeIds.SystemStatusChangeEventType,
                details = evDetails,
                state = string.Format(Properties.Resources.Inactive),
                logtype = (int)System.Diagnostics.EventLogEntryType.Information,
                logdestination = (int)LoggerDestination.Scheduler
            });
        }
#endregion

#region IDisposable Members
        public void CloseOPCEntityEnableConnection()
        {
            if (OPCEnableVar != null)
            {
                OPCEnableVar.SetInUse(this, false);
                OPCEnableVar = null;
            }
            if (observerEnable != null)
            {
                observerEnable.Dispose();
                observerEnable = null;
            }
            if (observerEnableMonitoredModel != null)
            {
                observerEnableMonitoredModel.Dispose();
                observerEnableMonitoredModel = null;
            }
        }
        public void CloseOPCEntityConnection()
        {
            if(OPCItem != null)
            {
                OPCItem.SetInUse(this, false);
                OPCItem = null;
            }
            if(observer != null)
            {
                observer.Dispose();
                observer = null;
            }
            if(observerMonitoredModel != null)
            {
                observerMonitoredModel.Dispose();
                observerMonitoredModel  = null;
            }
        }
        public void Dispose()
        {
            CloseOPCEntityConnection();
            CloseOPCEntityEnableConnection();
            if (dl != null)
            {
                dl.Dispose();
                dl = null;
            }
        }

#endregion

#region IEntityReference Members
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ImageSource CollapsedImageSource
        {
            get { throw new NotImplementedException(); }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ImageSource ExpandedImageSource
        {
            get { throw new NotImplementedException(); }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ContextMenu contextMenu
        {
            get { throw new NotImplementedException(); }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public object Tooltip
        {
            get { throw new NotImplementedException(); }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public object ContainedObject
        {
            get { throw new NotImplementedException(); }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public object EntityParent
        {
            get { throw new NotImplementedException(); }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string TypeDefinitionString
        {
            get { throw new NotImplementedException(); }
        }
#endregion

#region Persistence
        IDataLayer dl;
        DateTime LastExecutionTime;

        public void CreateDataLayer(String settings)
        {
            try
            {
                string conn = XpoHelper.GetConnectionString(settings, Properties.Settings.Default.TypeLabel, Name, Properties.Settings.Default.DefaultPersistenceFileExt);
                var dict = new DevExpress.Xpo.Metadata.ReflectionDictionary();
                var store = XpoDefault.GetConnectionProvider(conn, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema);
                dict.GetDataStoreSchema(typeof(SchedulerPersistence).Assembly);
                dl = new ThreadSafeDataLayer(dict, store);
            }
            catch (Exception e)
            {
                Opc.Ua.Utils.Trace(e, "Unexpected error creating the persistence data layer for Scheduler '{0}'", this.Name);
            }
        }
        bool dofirstexecution = false;
        public void LoadSchedulerStatus()
        {
            if (dl != null)
            {
                using (UnitOfWork ufw = new UnitOfWork(dl))
                { 
                    var status = (from sched in new XPQuery<SchedulerPersistence>(ufw)
                                      select sched).ToList();
                    bool havePersistence = status.Count > 0;
                    //if (status.Count > 0)
                    {
                        LastExecutionTime = (havePersistence ? status[0].LastExecutionTime : DateTime.Now/*????*/);
                        /*CommandExecuted*/executed = (havePersistence ? status[0].CommandExecuted : false);
                        if (Type == ScheduleType.calendar || Type == ScheduleType.weeklyPlan)
                        {
                            List<SchedulingDate> lista = WeeklyCalendar;

                            if (Type == ScheduleType.calendar)
                                lista = Calendar;

                            lista.Sort(SchedulingDateCompareStart);
                            if (lista != null && lista.Count > 0)
                            {
                                foreach (var si in lista)
                                {
                                    //if (si.EndDate < LastExecutionTime)
                                    //    continue;
                                    if (si.Type == CalendarItemType.WeekNDate)
                                    {
                                        if (si.Month == CalendarMonthType.Any ||
                                            (si.Month == CalendarMonthType.Even && LastExecutionTime.Month % 2 == 0) || //month even
                                            (si.Month == CalendarMonthType.Odd && LastExecutionTime.Month % 2 != 0) || //month odd
                                            (si.Month < CalendarMonthType.Odd && ((int)si.Month + 1 == LastExecutionTime.Month)) //this month
                                            )
                                        {
                                            if (si.WeekOfMonth == CalendarWeekType.Any ||
                                                (si.WeekOfMonth == CalendarWeekType.Last && LastExecutionTime.GetWeekOfMonth() > DateTime.DaysInMonth(LastExecutionTime.Year, (int)si.Month + 1) / 7 + (DateTime.DaysInMonth(LastExecutionTime.Year, (int)si.Month + 1) % 7 != 0 ? 1 : 0)) ||
                                                (LastExecutionTime.GetWeekOfMonth() == (int)si.WeekOfMonth + 1)
                                                )
                                            {
                                                if ((si.DayOfWeek == CalendarDayType.Any || ((int)LastExecutionTime.DayOfWeek == (int)si.DayOfWeek))
                                                   && (LastExecutionTime.TimeOfDay >= si.StartDate.TimeOfDay && LastExecutionTime.TimeOfDay <= si.EndDate.TimeOfDay))
                                                {
                                                    dofirstexecution = ExecOnAtStartup/*true*/;
                                                    break;
                                                }
                                                else
                                                {
                                                    dofirstexecution = ExecOffAtStartup;
                                                    break;
                                                }
                                            }

                                        }
                                    }
                                    else
                                    {
                                        if (CheckInsideDates(LastExecutionTime, si))//(LastExecutionTime >= si.StartDate && LastExecutionTime <= si.EndDate)
                                        {
                                            dofirstexecution = ExecOnAtStartup/*true*/;
                                            break;
                                        }
                                        else
                                        {
                                            dofirstexecution = ExecOffAtStartup;
                                            //break;
                                        }
                                    }
                                }
                                
                            }
                            else
                            {
                                //no scheduling interval, sure off
                                dofirstexecution = ExecOffAtStartup;
                            }
                        }
                        else
                        {
                            DateTime on = Time;
                            DateTime off = TimeOff;
                            bool typeOn = (!string.IsNullOrEmpty(ValueOn) || 
                                HasCommandOn || (IsOnHoliday(DateTime.Now) && (HasCommandOnEx || !string.IsNullOrEmpty(ExceptionValueOn))));
                            bool typeOff = (!string.IsNullOrEmpty(ValueOff) || HasCommandOff || 
                                (IsOnHoliday(DateTime.Now) && (HasCommandOffEx || !string.IsNullOrEmpty(ExceptionValueOff))));
                            switch (Type)
                            { 
                                case ScheduleType.everyMinute:
                                    if (!havePersistence)
                                        LastExecutionTime.Subtract(new TimeSpan(0, 1, 0));
                                    on = new DateTime(LastExecutionTime.Year, LastExecutionTime.Month, LastExecutionTime.Day, LastExecutionTime.Hour, LastExecutionTime.Minute, Time.Second);
                                    off = new DateTime(LastExecutionTime.Year, LastExecutionTime.Month, LastExecutionTime.Day, LastExecutionTime.Hour, LastExecutionTime.Minute, TimeOff.Second);
                                    break;
                                case ScheduleType.everyHour:
                                    if (!havePersistence)
                                        LastExecutionTime.Subtract(new TimeSpan(1, 0, 0));
                                    on = new DateTime(LastExecutionTime.Year, LastExecutionTime.Month, LastExecutionTime.Day, LastExecutionTime.Hour, Time.Minute, Time.Second);
                                    off = new DateTime(LastExecutionTime.Year, LastExecutionTime.Month, LastExecutionTime.Day, LastExecutionTime.Hour, TimeOff.Minute, TimeOff.Second);
                                    break;
                                case ScheduleType.everyDay:
                                    if (!havePersistence)
                                        LastExecutionTime.Subtract(new TimeSpan(1, 0, 0, 0));
                                    on = new DateTime(LastExecutionTime.Year, LastExecutionTime.Month, LastExecutionTime.Day, Time.Hour, Time.Minute, Time.Second);
                                    off = new DateTime(LastExecutionTime.Year, LastExecutionTime.Month, LastExecutionTime.Day, TimeOff.Hour, TimeOff.Minute, TimeOff.Second);
                                    break;
                                case ScheduleType.everyMonday:
                                    if (!havePersistence && LastExecutionTime.DayOfWeek == DayOfWeek.Monday)
                                        LastExecutionTime.Subtract(new TimeSpan(1, 0, 0, 0));
                                    on = GetTimeForEveryDay(LastExecutionTime, Time, DayOfWeek.Monday);
                                    off = GetTimeForEveryDay(LastExecutionTime, TimeOff, DayOfWeek.Monday);
                                    break;
                                case ScheduleType.everyTuesday:
                                    if (!havePersistence && LastExecutionTime.DayOfWeek == DayOfWeek.Tuesday)
                                        LastExecutionTime.Subtract(new TimeSpan(1, 0, 0, 0));
                                    on = GetTimeForEveryDay(LastExecutionTime, Time, DayOfWeek.Tuesday);
                                    off = GetTimeForEveryDay(LastExecutionTime, TimeOff, DayOfWeek.Tuesday);
                                    break;
                                case ScheduleType.everyWednesday:
                                    if (!havePersistence && LastExecutionTime.DayOfWeek == DayOfWeek.Wednesday)
                                        LastExecutionTime.Subtract(new TimeSpan(1, 0, 0, 0));
                                    on = GetTimeForEveryDay(LastExecutionTime, Time, DayOfWeek.Wednesday);
                                    off = GetTimeForEveryDay(LastExecutionTime, TimeOff, DayOfWeek.Wednesday);
                                    break;
                                case ScheduleType.everyThursday:
                                    if (!havePersistence && LastExecutionTime.DayOfWeek == DayOfWeek.Thursday)
                                        LastExecutionTime.Subtract(new TimeSpan(1, 0, 0, 0));
                                    on = GetTimeForEveryDay(LastExecutionTime, Time, DayOfWeek.Thursday);
                                    off = GetTimeForEveryDay(LastExecutionTime, TimeOff, DayOfWeek.Thursday);
                                    break;
                                case ScheduleType.everyFriday:
                                    if (!havePersistence && LastExecutionTime.DayOfWeek == DayOfWeek.Friday)
                                        LastExecutionTime.Subtract(new TimeSpan(1, 0, 0, 0));
                                    on = GetTimeForEveryDay(LastExecutionTime, Time, DayOfWeek.Friday);
                                    off = GetTimeForEveryDay(LastExecutionTime, TimeOff, DayOfWeek.Friday);
                                    break;
                                case ScheduleType.everySaturday:
                                    if (!havePersistence && LastExecutionTime.DayOfWeek == DayOfWeek.Saturday)
                                        LastExecutionTime.Subtract(new TimeSpan(1, 0, 0, 0));
                                    on = GetTimeForEveryDay(LastExecutionTime, Time, DayOfWeek.Saturday);
                                    off = GetTimeForEveryDay(LastExecutionTime, TimeOff, DayOfWeek.Saturday);
                                    break;
                                case ScheduleType.everySunday:
                                    if (!havePersistence && LastExecutionTime.DayOfWeek == DayOfWeek.Sunday)
                                        LastExecutionTime.Subtract(new TimeSpan(1, 0, 0, 0));
                                    on = GetTimeForEveryDay(LastExecutionTime, Time, DayOfWeek.Sunday);
                                    off = GetTimeForEveryDay(LastExecutionTime, TimeOff, DayOfWeek.Sunday);
                                    break;
                            }
                            if (executed && DateTime.Now > off && typeOff)//to execute off 
                            {
                                dofirstexecution = true;
                            }
                            else if (!executed && (DateTime.Now > on && DateTime.Now < off) && typeOn)//to execute on
                            {
                                dofirstexecution = true;
                            }
                        }
                    }
                }
            }
        }
        
        private void SaveSchedulerStatus(DateTime execTtime)
        {
            if (dl != null)
            {
                using (UnitOfWork ufw = new UnitOfWork(dl))
                {
                    var status = (from sched in new XPQuery<SchedulerPersistence>(ufw)
                                  select sched).ToList();
                    if (status.Count > 0)
                        status[0].Delete();
                    var newstatus = new SchedulerPersistence(ufw)
                    {
                        LastExecutionTime = execTtime,
                        CommandExecuted = executed
                    };
                    ufw.CommitChangesAndDropIdentityMap();
                }
            }
        }
#endregion
    }
    static class DateTimeExtensions
    {
        static GregorianCalendar _gc = new GregorianCalendar();
        public static int GetWeekOfMonth(this DateTime time)
        {
            DateTime first = new DateTime(time.Year, time.Month, 1);
            return time.GetWeekOfYear() - first.GetWeekOfYear() + 1;
        }

        static int GetWeekOfYear(this DateTime time)
        {
            return _gc.GetWeekOfYear(time, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        }
    }
}
