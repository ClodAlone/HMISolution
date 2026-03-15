using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;

namespace MSModel
{
    [DeferredDeletion(false)]
    public class MSScheduledActionRuntime : XPObject
    {
        #region Ctors
        public MSScheduledActionRuntime(Session session)
            : base(session)
        {
            
        }
        #endregion

        #region Properties Default Values
        // List of constant default values for each property where you want handle a default value.
        //const int defaultPropertyName = -1;

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
        }
        #endregion

        #region Properties
        //public string FullName
        //{
        //    get
        //    {
        //        if (_MSFolderAss != null)
        //            return string.Format("{0}\\{1}", fullPath(_MSFolderAss), _Name);

        //        return _Name;
        //    }
        //}

        //private string _Name;
        //[Indexed(Unique = false)]
        //public string Name
        //{
        //    get
        //    {
        //        return _Name;
        //    }
        //    set
        //    {
        //        SetPropertyValue("Name", ref _Name, value);
        //    }
        //}

        private Guid _NodeId;
        [Custom("Generate", "Guid")]
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

        //private bool _Enable = true;
        //public bool Enable
        //{
        //    get
        //    {
        //        return _Enable;
        //    }
        //    set
        //    {
        //        SetPropertyValue("Enable", ref _Enable, value);
        //    }
        //}

        //private ScheduleType _Type;
        //public ScheduleType Type
        //{
        //    get
        //    {
        //        return _Type;
        //    }
        //    set
        //    {
        //        SetPropertyValue("Type", ref _Type, value);
        //    }
        //}

        //public UFUAModel.DataType DataType
        //{
        //    get
        //    {
        //        return UFUAModel.DataType.Boolean;
        //    }
        //}

        //public UFUAModel.ModelType ModelType
        //{
        //    get
        //    {
        //        return UFUAModel.ModelType.Variable;
        //    }
        //}

        private DateTime _Time;
        public DateTime Time
        {
            get
            {
                return _Time;
            }
            set
            {
                SetPropertyValue("Time", ref _Time, value);
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
                SetPropertyValue("TimeOff", ref _TimeOff, value);
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
        //private string _ValueOn;
        //public string ValueOn
        //{
        //    get
        //    {
        //        return _ValueOn;
        //    }
        //    set
        //    {
        //        SetPropertyValue("ValueOn", ref _ValueOn, value);
        //    }
        //}
        //private string _ValueOff;
        //public string ValueOff
        //{
        //    get
        //    {
        //        return _ValueOff;
        //    }
        //    set
        //    {
        //        SetPropertyValue("ValueOff", ref _ValueOff, value);
        //    }
        //}

        //#region OpcUaEntityReference
        //private string _ScheduleItem;
        //[Size(SizeAttribute.Unlimited)]
        //public string ScheduleItem
        //{
        //    get { return _ScheduleItem; }
        //    set
        //    {
        //        SetPropertyValue("ScheduleItem", ref _ScheduleItem, value);
        //    }
        //}
        //private string _EnableVariable;
        //[Size(SizeAttribute.Unlimited)]
        //public string EnableVariable
        //{
        //    get { return _EnableVariable; }
        //    set
        //    {
        //        SetPropertyValue("EnableVariable", ref _EnableVariable, value);
        //    }
        //}



        //private string _EnableVarName;
        //public string EnableVarName
        //{
        //    get
        //    {
        //        return _EnableVarName;
        //    }
        //    set
        //    {
        //        SetPropertyValue("EnableVarName", ref _EnableVarName, value);
        //    }
        //}

        //private string _ScheduleItemName;
        //public string ScheduleItemName
        //{
        //    get
        //    {
        //        return _ScheduleItemName;
        //    }
        //    set
        //    {
        //        SetPropertyValue("ScheduleItemName", ref _ScheduleItemName, value);
        //    }
        //}
        //private bool _RuntimeSelectable = false;
        //public bool RuntimeSelectable
        //{
        //    get
        //    {
        //        return _RuntimeSelectable;
        //    }
        //    set
        //    {
        //        SetPropertyValue("RuntimeSelectable", ref _RuntimeSelectable, value);
        //    }
        //}
        //private String _AccessRole;
        //public String AccessRole
        //{
        //    get { return _AccessRole; }
        //    set
        //    {
        //        SetPropertyValue("AccessRole", ref _AccessRole, value);
        //    }
        //}
        //private uint _AccessLevel;
        //public uint AccessLevel
        //{
        //    get
        //    {
        //        return _AccessLevel;
        //    }
        //    set
        //    {
        //        SetPropertyValue("AccessLevel", ref _AccessLevel, value);
        //    }
        //}

        //private uint _AccessMask;
        //public uint AccessMask
        //{
        //    get
        //    {
        //        return _AccessMask;
        //    }
        //    set
        //    {
        //        SetPropertyValue("AccessMask", ref _AccessMask, value);
        //    }
        //}
        //#endregion

        [Association("MSScheduledActionRuntime-WeeklyCalendarItems"), Aggregated]
        public XPCollection<WeeklyCalendarItem> WeeklyCalendar
        {
            get
            {
                return GetCollection<WeeklyCalendarItem>("WeeklyCalendar");
            }
        }

        [Association("MSScheduledActionRuntime-CalendarItems"), Aggregated]
        public XPCollection<CalendarItem> Calendar
        {
            get
            {
                return GetCollection<CalendarItem>("Calendar");
            }
        }

        [Association("MSScheduledActionRuntime-ExceptionsCalendarItems"), Aggregated]
        public XPCollection<ExceptionsCalendarItem> ExceptionsCalendar
        {
            get
            {
                return GetCollection<ExceptionsCalendarItem>("ExceptionsCalendar");
            }
        }

        //[Association("MSScheduledAction-MSAction"), Aggregated]
        //public XPCollection<MSAction> ActionsOn
        //{
        //    get
        //    {
        //        return GetCollection<MSAction>("ActionsOn");
        //    }
        //}

        //[Association("MSScheduledAction-MSAction"), Aggregated]
        //public XPCollection<MSAction> ActionsOff
        //{
        //    get
        //    {
        //        return GetCollection<MSAction>("ActionsOff");
        //    }
        //}

        //private MSFolder _MSFolderAss;
        //[Association("MSFolder-MSScheduledActions")]
        //public MSFolder MSFolderAss
        //{
        //    get
        //    {
        //        return _MSFolderAss;
        //    }
        //    set
        //    {
        //        SetPropertyValue("MSFolderAss", ref _MSFolderAss, value);
        //    }
        //}

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
    }
}
