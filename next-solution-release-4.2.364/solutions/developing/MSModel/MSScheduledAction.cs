using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using System.ComponentModel;
using Opc.Ua;
using UFInterfaces.PropertyControl;
using OPCUAViewModel;
using System.Windows;
using System.Windows.Input;
using Utilities;
using System.Data.SqlTypes;

namespace MSModel
{
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum ScheduleType
    {
        everyMinute,
        everyHour,
        everyDay,
        everySunday,
        everyMonday,
        everyTuesday,
        everyWednesday,
        everyThursday,
        everyFriday,
        everySaturday,
        weeklyPlan,
        calendar
    }

    #region Converters for XPObject

    public class ConvertNodeId : ValueConverter
    {

        public override object ConvertFromStorageType(object value)
        {
            if (!(value is String) || String.IsNullOrEmpty(value.ToString()))
            {
                return null;
            }
            return NodeId.Parse((String)value);
        }

        public override object ConvertToStorageType(object value)
        {
            if ((value == null) || String.IsNullOrEmpty(value.ToString()))
            {
                return null;
            }
            return value.ToString();
        }

        public override Type StorageType
        {
            get
            {
                return typeof(string);
            }
        }
    }

    public class ConvertExpandedNodeId : ValueConverter
    {

        public override object ConvertFromStorageType(object value)
        {
            if (!(value is String) || String.IsNullOrEmpty(value.ToString()))
            {
                return null;
            }
            
            return ExpandedNodeId.Parse((String)value);
        }

        public override object ConvertToStorageType(object value)
        {
            if ((value == null) || String.IsNullOrEmpty(value.ToString()))
            {
                return null;
            }
            return value.ToString();
        }

        public override Type StorageType
        {
            get
            {
                return typeof(string);
            }
        }
    }


    public class ConvertDataType : ValueConverter
    {

        public override object ConvertFromStorageType(object value)
        {



            object n = null;
            try
            {
                if (value.GetType() == typeof(int))
                {
                    n = new NodeId((uint)Convert.ToInt32(value));
                }
                else if (value.GetType() == typeof(Guid))
                {
                    n = new NodeId((Guid)value);
                }
            }
            catch
            {
            }
            return n;
        }

        public override object ConvertToStorageType(object value)
        {
            object n = null;
            try
            {
                NodeId nodo = (NodeId)value;
                if (nodo.IdType == IdType.Numeric)
                {
                    n = TypeInfo.GetBuiltInType(nodo);
                }
                else if (nodo.IdType == IdType.Guid)
                {
                    n = nodo.Identifier;
                }
            }
            catch
            {
            }
            return n;
        }

        public override Type StorageType
        {
            get
            {
                return typeof(BuiltInType);
            }
        }
    }

    #endregion
    public class MSScheduledAction : XPObject,
#if !NET_STANDARD
        ICommandSource, 
#endif
        IDataErrorInfo, INotifyPropertyVisibilityChanged, XpoHelpers.UndoRedoIXPSimpleObjectHelper.IUniqueIdentifier
    {
#region Ctors
        public MSScheduledAction(Session session)
            : base(session)
        {
            
        }
#endregion

#region Properties Default Values
        // List of constant default values for each property where you want handle a default value.
        //const int defaultPropertyName = -1;
        const bool defaultEnable = true;
        public const bool defaultExecOnAtStartup = true;
        public const bool defaultExecOffAtStartup = true;

        /// <summary>
        /// Adds inside this method the nullable property where you want handle a default value.
        /// </summary>
        private void EnsureDefaultValues()
        {
            // Examples of how to handle a default value
            //if (!PropertyName.HasValue)
            //    PropertyName = defaultPropertyName;
            //if (TimeSpanPropertyName == TimeSpan.Zero)
            //    TimeSpanPropertyName = TimeSpan.FromMinutes(1);
            //if (DateTimePropertyName == DateTime.MinValue)
            //    DateTimePropertyName = DateTime.UtcNow;

            if (!Enable.HasValue)
                Enable = defaultEnable;
            if (_Time != DateTime.MinValue)
                _Time = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, _Time.Hour, _Time.Minute, _Time.Second);
            if (_TimeOff != DateTime.MinValue)
                _TimeOff = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, _TimeOff.Hour, _TimeOff.Minute, _TimeOff.Second);

            if (!ExecOnAtStartup.HasValue)
                ExecOnAtStartup = defaultExecOnAtStartup;
            if (!ExecOffAtStartup.HasValue)
                ExecOffAtStartup = defaultExecOffAtStartup;
        }
#endregion

#region Not Persistence Properties
        [Browsable(false)]
        [NonPersistent]
        public bool HasCommands
        {
            get
            {
                return !String.IsNullOrEmpty(CommandsOn) ||
                    !String.IsNullOrEmpty(CommandsOff);
            }
        }
#endregion

#region Properties
        public string FullName
        {
            get
            {
                if (_MSFolderAss != null)
                    return string.Format("{0}\\{1}", fullPath(_MSFolderAss), _Name);

                return _Name;
            }
        }

        private string _Name;
        //[Indexed(Unique = false)]
        [MergablePropertyAttribute(false)]
        [Size(SizeAttribute.Unlimited)]
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                SetPropertyValue("Name", ref _Name, value);
            }
        }

        private Guid _NodeId;
        [Custom("Generate", "Guid")]
        [ReadOnly(true)]
        public Guid NodeId
        {
            get
            {
                return _NodeId;
            }
            set
            {
                SetPropertyValue("NodeId", ref _NodeId, value);
            }
        }

        private bool? _Enable;
        public bool? Enable
        {
            get
            {
                return _Enable;
            }
            set
            {
                SetPropertyValue("Enable", ref _Enable, value);
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
                if (SetPropertyValue("Type", ref _Type, value))
                {
                    this.RaisePropertyChangedEvent("Time");
                    this.RaisePropertyChangedEvent("TimeOff");
                    OnPropertyVisiblityChanged("Type");
                }
            }
        }

        public UFUAModel.DataType DataType
        {
            get
            {
                return UFUAModel.DataType.Boolean;
            }
        }

        public UFUAModel.ModelType ModelType
        {
            get
            {
                return UFUAModel.ModelType.Variable;
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
                if (value != null && value < SqlDateTime.MinValue.Value)
                {
                    var minValue = SqlDateTime.MinValue.Value;
                    value = minValue.Add(value.TimeOfDay);
                }

                SetPropertyValue("Time", ref _Time, value);
                this.RaisePropertyChangedEvent("TimeOff");
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
                SetPropertyValue("Date", ref _Date, value);
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
                if (value != null && value < SqlDateTime.MinValue.Value)
                {
                    var minValue = SqlDateTime.MinValue.Value;
                    value = minValue.Add(value.TimeOfDay);
                }

                SetPropertyValue("TimeOff", ref _TimeOff, value);
                this.RaisePropertyChangedEvent("Time");
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
                SetPropertyValue("DateOff", ref _DateOff, value);
            }
        }

        private string _EnableVariable;
        [Size(SizeAttribute.Unlimited)]
        public string EnableVariable
        {
            get { return _EnableVariable; }
            set
            {
                SetPropertyValue("EnableVariable", ref _EnableVariable, value);
            }
        }

#region OpcUaEntityReference
        private string _ScheduleItem;
        [Size(SizeAttribute.Unlimited)]
        public string ScheduleItem
        {
            get { return _ScheduleItem; }
            set
            {
                SetPropertyValue("ScheduleItem", ref _ScheduleItem, value);
            }
        }

        public bool SchedulerExceptions
        {
            get
            {
                return true;
            }
        }

        private string _ValueOn;
        [Size(SizeAttribute.Unlimited)]
        public string ValueOn
        {
            get
            {
                return _ValueOn;
            }
            set
            {
                SetPropertyValue("ValueOn", ref _ValueOn, value);
            }
        }
        private string _ValueOff;
        [Size(SizeAttribute.Unlimited)]
        public string ValueOff
        {
            get
            {
                return _ValueOff;
            }
            set
            {
                SetPropertyValue("ValueOff", ref _ValueOff, value);
            }
        }

        private string _CommandsOn;
        [Category("Execution")]
        [Size(SizeAttribute.Unlimited)]
        public string CommandsOn
        {
            get
            {
                return _CommandsOn;
            }
            set
            {
                SetPropertyValue("CommandsOn", ref _CommandsOn, value);
            }
        }

        private string _CommandsOff;
        [Category("Execution")]
        [Size(SizeAttribute.Unlimited)]
        public string CommandsOff
        {
            get
            {
                return _CommandsOff;
            }
            set
            {
                SetPropertyValue("CommandsOff", ref _CommandsOff, value);
            }
        }

        private bool _RuntimeSelectable;
        public bool RuntimeSelectable
        {
            get
            {
                return _RuntimeSelectable;
            }
            set
            {
                SetPropertyValue("RuntimeSelectable", ref _RuntimeSelectable, value);
            }
        }

        private String _AccessRole;
        [Size(SizeAttribute.Unlimited)]
        public String AccessRole
        {
            get { return _AccessRole; }
            set
            {
                SetPropertyValue("AccessRole", ref _AccessRole, value);
            }
        }
        private int _AccessLevels;
        public int AccessLevels
        {
            get
            {
                return _AccessLevels;
            }
            set
            {
                SetPropertyValue("AccessLevels", ref _AccessLevels, value);
            }
        }

        private int _AccessMasks;
        public int AccessMasks
        {
            get
            {
                return _AccessMasks;
            }
            set
            {
                SetPropertyValue("AccessMasks", ref _AccessMasks, value);
            }
        }
        
#endregion

        [Association("MSScheduledAction-WeeklyCalendarItems"), Aggregated]
        [Browsable (false)]
        public XPCollection<WeeklyCalendarItem> WeeklyCalendar
        {
            get
            {
                return GetCollection<WeeklyCalendarItem>("WeeklyCalendar");
            }
        }

        [Association("MSScheduledAction-CalendarItems"), Aggregated]
        [Browsable(false)]
        public XPCollection<CalendarItem> Calendar
        {
            get
            {
                return GetCollection<CalendarItem>("Calendar");
            }
        }

        [Association("MSScheduledAction-ExceptionsCalendarItems"), Aggregated]
        [Browsable(false)]
        public XPCollection<ExceptionsCalendarItem> ExceptionsCalendar
        {
            get
            {
                return GetCollection<ExceptionsCalendarItem>("ExceptionsCalendar");
            }
        }

        [Association("MSScheduledAction-MSAction"), Aggregated]
        [Browsable(false)]
        public XPCollection<MSAction> ActionsOn
        {
            get
            {
                return GetCollection<MSAction>("ActionsOn");
            }
        }

        [Association("MSScheduledAction-MSAction"), Aggregated]
        [Browsable(false)]
        public XPCollection<MSAction> ActionsOff
        {
            get
            {
                return GetCollection<MSAction>("ActionsOff");
            }
        }

        private MSFolder _MSFolderAss;
        [Association("MSFolder-MSScheduledActions")]
        [Browsable(false)]
        public MSFolder MSFolderAss
        {
            get
            {
                return _MSFolderAss;
            }
            set
            {
                SetPropertyValue("MSFolderAss", ref _MSFolderAss, value);
            }
        }

        private bool? _ExecOnAtStartup;
        public bool? ExecOnAtStartup
        {
            get
            {
                return _ExecOnAtStartup;
            }
            set
            {
                SetPropertyValue("ExecOnAtStartup", ref _ExecOnAtStartup, value);
            }
        }

        private bool? _ExecOffAtStartup;
        public bool? ExecOffAtStartup
        {
            get
            {
                return _ExecOffAtStartup;
            }
            set
            {
                SetPropertyValue("ExecOffAtStartup", ref _ExecOffAtStartup, value);
            }
        }

        private bool _HideRuntimeExceptionTab;
        public bool HideRuntimeExceptionTab
        {
            get
            {
                return _HideRuntimeExceptionTab;
            }
            set
            {
                SetPropertyValue("ShowRuntimeExceptionTab", ref _HideRuntimeExceptionTab, value);
            }
        }

        private bool _HideRuntimeSettingsTab;
        public bool HideRuntimeSettingsTab
        {
            get
            {
                return _HideRuntimeSettingsTab;
            }
            set
            {
                SetPropertyValue("ShowRuntimeSettingsTab", ref _HideRuntimeSettingsTab, value);
            }
        }
#endregion

#region Override Methods

        public override void AfterConstruction()
        {
            base.AfterConstruction();

            EnsureDefaultValues();
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();

            EnsureDefaultValues();
        }

#endregion

#region IUniqueIdentifier
        [Browsable(false)]
        [NonPersistent]
        public String UniqueIdentifier
        {
            get
            {
                return NodeId.ToString();
            }
        }
#endregion


#region IDataErrorInfo
        [Browsable(false)]
        public string Error
        {
            get
            {
                var context = new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null);
                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

                return !System.ComponentModel.DataAnnotations.Validator.TryValidateObject(this, context, results)
                    ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                    : null;
            }
        }

        public string this[string propertyName]
        {
            get
            {
                String s = PerformValidation(propertyName);
                if (!String.IsNullOrEmpty(s))
                    return s;
                var context = new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null)
                {
                    MemberName = propertyName
                };

                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
                var propertyInfo = GetType().GetProperty(propertyName);
                if (propertyInfo != null)
                {
                    var value = propertyInfo.GetValue(this, null);

                    return !System.ComponentModel.DataAnnotations.Validator.TryValidateProperty(value, context, results)
                        ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                        : null;
                }

                return null;
            }
        }
#endregion

#region ICommandSource Members
#if !NET_STANDARD
        // ICommandSource Interface allow the object to expose commands in Command Explorer Window.

        [Browsable(false)]
        public ICommand Command
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        public object CommandParameter
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        public IInputElement CommandTarget
        {
            get
            {
                return null;
            }
        }
#endif
#endregion

#region INotifyPropertyVisibilityChanged Members

        /// <summary>
        /// Gets the visibility state for the property with the given name.
        /// </summary>
        /// <param name="propertyName">The property name that you want konw the current visibility state.</param>
        /// <returns></returns>
        bool INotifyPropertyVisibilityChanged.this[string propertyName]
        {
            get
            {
                if (propertyName == "Time" || propertyName == "TimeOff")
                {
                    return Type != ScheduleType.weeklyPlan && Type != ScheduleType.calendar;
                }

                if (propertyName == "ExecOnAtStartup" || propertyName == "ExecOffAtStartup")
                {
                    return Type == ScheduleType.weeklyPlan || Type == ScheduleType.calendar;
                }

                return true;
            }
        }

        /// <summary>
        /// Raised when a property visibility state on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyVisiblityChanged;

        /// <summary>
        /// Raises this object's PropertyVisiblityChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has changed his value and has triggered the change of visibility.</param>
        protected void OnPropertyVisiblityChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyVisiblityChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

#endregion

#region Methods

        public string GetRelativeName()
        {
            string name = string.Format("{0}", Name);
            if (MSFolderAss != null)
            {
                name = string.Format("{0}/{1}", MSFolderAss.GetRelativeName(), Name);
            }
            return name;
        }

        string fullPath(MSFolder folder)
        {
            if (folder.MSFolderAss != null)
                return string.Format("{0}\\{1}", fullPath(folder.MSFolderAss), folder.Name);
            return folder.Name;
        }

        protected String PerformValidation(String propertyName)
        {
            if (propertyName == "Name")
            {
                if (String.IsNullOrWhiteSpace(Name))
                {
                    return Properties.Resources.ObjectNameMissing;
                }
                else if (MSFolderAss != null && (from c in MSFolderAss.MSScheduledActions.AsParallel()
                                                 where c != this && c.Name == Name
                                                 select c).ToList().Count > 0)
                {
                    return Properties.Resources.EventNameAlreadyExists;
                }
                else if ((from tag in new XPQuery<MSModel.MSScheduledAction>(Session, true)/*.AsParallel()*/
                          where tag.MSFolderAss == null && tag.Name == Name
                          select tag).ToList().Count > 1)
                {
                    return Properties.Resources.EventNameAlreadyExists;
                }
            }
            else if (propertyName == "AccessLevels")
            {
                if (AccessLevels < 0)
                    return Properties.Resources.AccessLevelNonNegative;
            }
            return null;
        }

        public void Fill(SimpleScheduledEvent se)
        {
            Name = se.FullName;
            NodeId = se.Guid;
            Enable = se.Enable;
            Type = se.Type;
            Time = se.Time;
            Date = se.Date;
            TimeOff = se.TimeOff;
            DateOff = se.DateOff;
            EnableVariable = se.EnableVariable;
            ScheduleItem = se.ScheduleVariable;
            ValueOn = se.ValueOn;
            ValueOff = se.ValueOff;
            AccessRole = se.AccessRole;
            AccessLevels = se.AccessLevels;
            AccessMasks = se.AccessMasks;
            ExecOnAtStartup = se.ExecOnAtStartup;
            ExecOffAtStartup = se.ExecOffAtStartup;

            while (WeeklyCalendar.Count > 0)
            {
                WeeklyCalendar[0].Delete();
                if (Calendar.Count > 0)
                    Calendar[0].Delete();
            }
            foreach (var w in se.WeeklyCalendar)
            {
                WeeklyCalendar.Add(new WeeklyCalendarItem(Session)
                {
                    EndDate = w.EndDate,
                    StartDate = w.StartDate
                });
            }

            while (Calendar.Count > 0)
                Calendar[0].Delete();
            if (se.Type != ScheduleType.weeklyPlan)
            {
                foreach (var c in se.Calendar)
                    Calendar.Add(new CalendarItem(Session)
                    {
                        DayOfWeek = c.DayOfWeek,
                        EndDate = c.EndDate,
                        Month = c.Month,
                        StartDate = c.StartDate,
                        Type = c.Type,
                        WeekOfMonth = c.WeekOfMonth,
                        DayOfMonth = c.DayOfMonth
                    });
            }
            else
            {
                foreach (var w in se.WeeklyCalendar)
                {
                    Calendar.Add(new CalendarItem(Session)
                    {
                        DayOfWeek = w.DayOfWeek,
                        EndDate = w.EndDate,
                        Month = CalendarMonthType.Any,
                        StartDate = w.StartDate,
                        Type = CalendarItemType.WeekNDate,
                        WeekOfMonth = CalendarWeekType.Any,
                        DayOfMonth = (int)MonthDayAny.NoDate
                    });
                }
            }

            while (ExceptionsCalendar.Count > 0)
                ExceptionsCalendar[0].Delete();
            foreach (var c in se.ExceptionsCalendar)
                ExceptionsCalendar.Add(new ExceptionsCalendarItem(Session)
                {
                    DayOfWeek = c.DayOfWeek,
                    EndDate = c.EndDate,
                    Month = c.Month,
                    StartDate = c.StartDate,
                    Type = c.Type,
                    WeekOfMonth = c.WeekOfMonth,
                    DayOfMonth = c.DayOfMonth
                });
        }

#endregion
    }
}
