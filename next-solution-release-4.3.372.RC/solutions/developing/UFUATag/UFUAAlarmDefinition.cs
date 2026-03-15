using System;
using System.Linq;
using DevExpress.Xpo;
using System.ComponentModel;
using System.Collections.Generic;
using Opc.Ua;
using UFInterfaces.PropertyControl;
using Utilities;
#if !NET_STANDARD
using Utilities.Xpo.UndoRedo;
#endif

namespace UFUAModel
{
    [Exportable(RequiredKeys = new string[] { "FolderPath", "Name" }, ImportFolderInfo = "FolderPath")]
    [DeferredDeletion(false)]
    public class UFUAAlarmDefinition : XPObject, IDataErrorInfo, INotifyPropertyVisibilityChanged, INotifyPropertyReadOnlyChanged
#if !NET_STANDARD
        , IUndoRedoXpo
#endif
    {
        #region Constructors
        public UFUAAlarmDefinition(Session session)
            : base(session)
        { }
        #endregion

        #region Properties Default Values
        // List of constant default values for each property where you want handle a default value.
        const AlarmType defaultAlarmType = UFUAModel.AlarmType.TripAlarm;
        const double defaultHighHighLimit = 100;
        const double defaultHighLimit = 90;
        const double defaultLowLimit = -90;
        const double defaultLowLowLimit = -100;
        const ConditionType defaultConditionType = UFUAModel.ConditionType.Equals;
        const double defaultActivationLowValue = 0;
        const double defaultActivationValue = 1;
        const DeviationType defaultDeviationType = UFUAModel.DeviationType.AbsoluteValue;
        const int defaultSeverity = 100;
        const bool defaultCreateOptionalAlarmMembers = true;

        /// <summary>
        /// Adds inside this method the nullable property where you want handle a default value.
        /// </summary>
        private void EnsureDefaultValues()
        {
            // Examples of how to handle a default value
            //if (!_PropertyName.HasValue)
            //    _PropertyName = defaultPropertyName;
            //if (_TimeSpanPropertyName == TimeSpan.Zero)
            //    _TimeSpanPropertyName = TimeSpan.FromMinutes(1);
            //if (_DateTimePropertyName == DateTime.MinValue)
            //    _DateTimePropertyName = DateTime.UtcNow;

            if (!_AlarmType.HasValue)
                _AlarmType = defaultAlarmType;
            if (!_HighHighLimit.HasValue)
                _HighHighLimit = defaultHighHighLimit;
            if (!_HighLimit.HasValue)
                _HighLimit = defaultHighLimit;
            if (!_LowLimit.HasValue)
                _LowLimit = defaultLowLimit;
            if (!_LowLowLimit.HasValue)
                _LowLowLimit = defaultLowLowLimit;
            if (!_ConditionType.HasValue)
                _ConditionType = defaultConditionType;
            if (!_ActivationLowValue.HasValue)
                _ActivationLowValue = defaultActivationLowValue;
            if (!_ActivationValue.HasValue)
                _ActivationValue = defaultActivationValue;
            if (!_DeviationType.HasValue)
                _DeviationType = defaultDeviationType;
            if (!_Severity.HasValue)
                _Severity = defaultSeverity;
            if (!_EnableQualityGood.HasValue)
                _EnableQualityGood = true;
            if (!_SupportAck.HasValue)
                _SupportAck = true;
            if (!_SupportReset.HasValue)
                _SupportReset = true;
            if (!_Beep.HasValue)
                _Beep = true;
            if (!_SaveEventsLog.HasValue)
                _SaveEventsLog = true;
            if (_TimeUnit == TimeSpan.Zero)
                _TimeUnit = TimeSpan.FromMinutes(1);
            if (!_CreateAlrMemberAckedState.HasValue)
                _CreateAlrMemberAckedState = defaultCreateOptionalAlarmMembers;
            if (!_CreateAlrMemberConfirmedState.HasValue)
                _CreateAlrMemberConfirmedState = defaultCreateOptionalAlarmMembers;
            if (!_CreateAlrMemberConfirm.HasValue)
                _CreateAlrMemberConfirm = defaultCreateOptionalAlarmMembers;
            if (!_CreateAlrMemberSuppressedState.HasValue)
                _CreateAlrMemberSuppressedState = defaultCreateOptionalAlarmMembers;
            if (!_CreateAlrMemberShelvingState.HasValue)
                _CreateAlrMemberShelvingState = defaultCreateOptionalAlarmMembers;
            if (!_CreateAlrMemberEnabledState.HasValue)
                _CreateAlrMemberEnabledState = defaultCreateOptionalAlarmMembers;
            if (!_CreateAlrMemberActiveState.HasValue)
                _CreateAlrMemberActiveState = defaultCreateOptionalAlarmMembers;
            if (!_CreateAlrMemberQuality.HasValue)
                _CreateAlrMemberQuality = defaultCreateOptionalAlarmMembers;

#if NET_STANDARD
            if (!_CreateAlrMemberLocalTime.HasValue)
                _CreateAlrMemberLocalTime = defaultCreateOptionalAlarmMembers;
#endif
        }
#endregion

            #region Not Persistence Properties
        [Browsable(false)]
        [NonPersistent]
        public String SourcePath
        {
            get
            {
                if (UFUAAlarmDefinitions != null)
                    return UFUAAlarmDefinitions.GetRelativeName();

                return String.Empty;
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

            #region Properties

        private string _Name;
        //[Indexed(Unique = false)]
        [MergablePropertyAttribute(false)]
        [Size(SizeAttribute.Unlimited)]
        [Exportable]
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

        private AlarmType? _AlarmType;
        [Exportable]
        public AlarmType? AlarmType
        {
            get
            {
                return _AlarmType;
            }
            set
            {
                if (SetPropertyValue("AlarmType", ref _AlarmType, value))
                {
                    this.RaisePropertyChangedEvent("TimeUnit");
                    this.RaisePropertyChangedEvent("ActivationLowValue");
                    this.RaisePropertyChangedEvent("ActivationValue");
                    OnPropertyVisiblityChanged("AlarmType");
                }
            }
        }

        private bool _EnableHighHighLimit;
        [Exportable]
        public bool EnableHighHighLimit
        {
            get
            {
                return _EnableHighHighLimit;
            }
            set
            {
                SetPropertyValue("EnableHighHighLimit", ref _EnableHighHighLimit, value);
                this.RaisePropertyChangedEvent("HighHighLimit");
                this.RaisePropertyChangedEvent("HighLimit");
                this.RaisePropertyChangedEvent("LowLimit");
                this.RaisePropertyChangedEvent("LowLowLimit");
            }
        }

        private double? _HighHighLimit;
        [Exportable]
        public double? HighHighLimit
        {
            get
            {
                return _HighHighLimit;
            }
            set
            {
                SetPropertyValue("HighHighLimit", ref _HighHighLimit, value);
                //this.RaisePropertyChangedEvent("HighHighLimit");
                this.RaisePropertyChangedEvent("HighLimit");
                this.RaisePropertyChangedEvent("LowLimit");
                this.RaisePropertyChangedEvent("LowLowLimit");
            }
        }

        private bool _EnableHighLimit;
        [Exportable]
        public bool EnableHighLimit
        {
            get
            {
                return _EnableHighLimit;
            }
            set
            {
                SetPropertyValue("EnableHighLimit", ref _EnableHighLimit, value);
                this.RaisePropertyChangedEvent("HighHighLimit");
                this.RaisePropertyChangedEvent("HighLimit");
                this.RaisePropertyChangedEvent("LowLimit");
                this.RaisePropertyChangedEvent("LowLowLimit");
            }
        }

        private double? _HighLimit;
        [Exportable]
        public double? HighLimit
        {
            get
            {
                return _HighLimit;
            }
            set
            {
                SetPropertyValue("HighLimit", ref _HighLimit, value);
                this.RaisePropertyChangedEvent("HighHighLimit");
                //this.RaisePropertyChangedEvent("HighLimit");
                this.RaisePropertyChangedEvent("LowLimit");
                this.RaisePropertyChangedEvent("LowLowLimit");
            }
        }

        private bool _EnableLowLimit;
        [Exportable]
        public bool EnableLowLimit
        {
            get
            {
                return _EnableLowLimit;
            }
            set
            {
                SetPropertyValue("EnableLowLimit", ref _EnableLowLimit, value);
                this.RaisePropertyChangedEvent("HighHighLimit");
                this.RaisePropertyChangedEvent("HighLimit");
                this.RaisePropertyChangedEvent("LowLimit");
                this.RaisePropertyChangedEvent("LowLowLimit");
            }
        }

        private double? _LowLimit;
        [Exportable]
        public double? LowLimit
        {
            get
            {
                return _LowLimit;
            }
            set
            {
                SetPropertyValue("LowLimit", ref _LowLimit, value);
                this.RaisePropertyChangedEvent("HighHighLimit");
                this.RaisePropertyChangedEvent("HighLimit");
                //this.RaisePropertyChangedEvent("LowLimit");
                this.RaisePropertyChangedEvent("LowLowLimit");
            }
        }

        private bool _EnableLowLowLimit;
        [Exportable]
        public bool EnableLowLowLimit
        {
            get
            {
                return _EnableLowLowLimit;
            }
            set
            {
                SetPropertyValue("EnableLowLowLimit", ref _EnableLowLowLimit, value);
                this.RaisePropertyChangedEvent("HighHighLimit");
                this.RaisePropertyChangedEvent("HighLimit");
                this.RaisePropertyChangedEvent("LowLimit");
                this.RaisePropertyChangedEvent("LowLowLimit");
            }
        }

        private double? _LowLowLimit;
        [Exportable]
        public double? LowLowLimit
        {
            get
            {
                return _LowLowLimit;
            }
            set
            {
                SetPropertyValue("LowLowLimit", ref _LowLowLimit, value);
                this.RaisePropertyChangedEvent("HighHighLimit");
                this.RaisePropertyChangedEvent("HighLimit");
                this.RaisePropertyChangedEvent("LowLimit");
                //this.RaisePropertyChangedEvent("LowLowLimit");
            }
        }

        private ConditionType? _ConditionType;
        [Exportable]
        public ConditionType? ConditionType
        {
            get
            {
                if (StatisticData == StatDef.StatProps.TotalTimeOn)
                    return UFUAModel.ConditionType.GreaterThanOrEqual;
                return _ConditionType;
            }
            set
            {
                if (SetPropertyValue("ConditionType", ref _ConditionType, value))
                {
                    this.RaisePropertyChangedEvent("ActivationLowValue");
                    this.RaisePropertyChangedEvent("ActivationValue");
                    OnPropertyVisiblityChanged("ConditionType");
                }
            }
        }

        private StatDef.StatProps _StatisticData;
        [Exportable]
        public StatDef.StatProps StatisticData
        {
            get
            {
                return _StatisticData;
            }
            set
            {
                if (SetPropertyValue("StatisticData", ref _StatisticData, value))
                {
                    OnPropertyReadOnlyChanged("StatisticData");
                    OnPropertyVisiblityChanged("StatisticData");
                }
            }
        }

        [NonPersistent]
        public TimeSpan TotalTimeOn
        {
            get
            {
                if (StatisticData != StatDef.StatProps.TotalTimeOn)
                    return TimeSpan.Zero;
                return TimeSpan.FromSeconds(ActivationValue.Value);
            }
            set
            {
                if (StatisticData == StatDef.StatProps.TotalTimeOn)
                    ActivationValue = value.TotalSeconds;
            }
        }

        private double? _ActivationLowValue;
        [Exportable]
        public double? ActivationLowValue
        {
            get
            {
                return _ActivationLowValue;
            }
            set
            {
                if (SetPropertyValue("ActivationLowValue", ref _ActivationLowValue, value))
                {
                    this.RaisePropertyChangedEvent("ActivationValue");
                }
            }
        }

        private double? _ActivationValue;
        [Exportable]
        public double? ActivationValue
        {
            get
            {
                return _ActivationValue;
            }
            set
            {
                if (SetPropertyValue("ActivationValue", ref _ActivationValue, value))
                {
                    this.RaisePropertyChangedEvent("ActivationLowValue");
                }
            }
        }

        private DeviationType? _DeviationType;
        [Exportable]
        public DeviationType? DeviationType
        {
            get
            {
                return _DeviationType;
            }
            set
            {
                SetPropertyValue("DeviationType", ref _DeviationType, value);
            }
        }

        private int? _Severity;
        [Exportable]
        public int? Severity
        {
            get
            {
                return _Severity;
            }
            set
            {
                if (SetPropertyValue("Severity", ref _Severity, value))
                {
                    OnPropertyVisiblityChanged("Severity");
                }
            }
        }

        private bool? _EnableQualityGood;
        [Exportable]
        public bool? EnableQualityGood
        {
            get
            {
                return _EnableQualityGood;
            }
            set
            {
                SetPropertyValue("EnableQualityGood", ref _EnableQualityGood, value);
            }
        }

        private bool? _SaveEventsLog;
        [Exportable]
        public bool? SaveEventsLog
        {
            get
            {
                return _SaveEventsLog;
            }
            set
            {
                SetPropertyValue("SaveEventsLog", ref _SaveEventsLog, value);
            }
        }

        private bool _SaveBranches;
        [Exportable]
        public bool SaveBranches
        {
            get
            {
                return _SaveBranches;
            }
            set
            {
                SetPropertyValue("SaveBranches", ref _SaveBranches, value);
            }
        }

        private bool? _SupportAck;
        [Exportable]
        public bool? SupportAck
        {
            get
            {
                return _SupportAck;
            }
            set
            {
                SetPropertyValue("SupportAck", ref _SupportAck, value);
            }
        }

        private bool? _SupportReset;
        [Exportable]
        public bool? SupportReset
        {
            get
            {
                return _SupportReset;
            }
            set
            {
                SetPropertyValue("SupportReset", ref _SupportReset, value);
            }
        }

        private bool? _Beep;
        [Exportable]
        public bool? Beep
        {
            get
            {
                return _Beep;
            }
            set
            {
                SetPropertyValue("Beep", ref _Beep, value);
            }
        }

        private string _SoundFile;
        [Size(SizeAttribute.Unlimited)]
        [Exportable]
        public string SoundFile
        {
            get
            {
                return _SoundFile;
            }
            set
            {
                SetPropertyValue("SoundFile", ref _SoundFile, value);
            }
        }

        private bool _RepeatSoundContinuously;
        [Exportable]
        public bool RepeatSoundContinuously
        {
            get
            {
                return _RepeatSoundContinuously;
            }
            set
            {
                SetPropertyValue("RepeatSoundContinuously", ref _RepeatSoundContinuously, value);
            }
        }

        private bool _UseTimeStamp;
        [Exportable]
        public bool UseTimeStamp
        {
            get
            {
                return _UseTimeStamp;
            }
            set
            {
                SetPropertyValue("UseTimeStamp", ref _UseTimeStamp, value);
            }
        }

        /// <summary>
        /// Get or Set the time unit used in the rate of change alarm activation.
        /// </summary>
        private TimeSpan _TimeUnit;
        [Exportable]
        public TimeSpan TimeUnit
        {
            get
            {
                return _TimeUnit;
            }
            set
            {
                SetPropertyValue("TimeUnit", ref _TimeUnit, value);
            }
        }

        /// <summary>
        /// when the alarm is off and the variable changes
        /// own value, then this delay time is wait before
        /// evaluating the alarm
        /// </summary>
        private TimeSpan _DelayTimeOn;
        [Exportable]
        public TimeSpan DelayTimeOn
        {
            get
            {
                return _DelayTimeOn;
            }
            set
            {
                SetPropertyValue("DelayTimeOn", ref _DelayTimeOn, value);
            }
        }

        /// <summary>
        /// when the alarm is on and the variable changes
        /// own value, then this delay time is wait before
        /// evaluating the alarm
        /// </summary>
        private TimeSpan _DelayTimeOff;
        [Exportable]
        public TimeSpan DelayTimeOff
        {
            get
            {
                return _DelayTimeOff;
            }
            set
            {
                SetPropertyValue("DelayTimeOff", ref _DelayTimeOff, value);
            }
        }

        private int _UserReadAccessMask;
        [Exportable]
        public int UserReadAccessMask
        {
            get
            {
                return _UserReadAccessMask;
            }
            set
            {
                SetPropertyValue("UserReadAccessMask", ref _UserReadAccessMask, value);
            }
        }

        private int _UserWriteAccessMask;
        [Exportable]
        public int UserWriteAccessMask
        {
            get
            {
                return _UserWriteAccessMask;
            }
            set
            {
                SetPropertyValue("UserWriteAccessMask", ref _UserWriteAccessMask, value);
            }
        }

        private int _UserAccessLevel;
        [Exportable]
        public int UserAccessLevel
        {
            get
            {
                return _UserAccessLevel;
            }
            set
            {
                SetPropertyValue("UserAccessLevel", ref _UserAccessLevel, value);
            }
        }

        //private AccessLevels? _AccessLevel;
        //[Exportable]
        //public AccessLevels? AccessLevel
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

        private bool? _CreateAlrMemberAckedState;
        public bool? CreateAlrMemberAckedState
        {
            get
            {
                return _CreateAlrMemberAckedState;
            }
            set
            {
                SetPropertyValue("CreateAlrMemberAckedState", ref _CreateAlrMemberAckedState, value);
            }
        }
        private bool? _CreateAlrMemberConfirmedState;
        public bool? CreateAlrMemberConfirmedState
        {
            get
            {
                return _CreateAlrMemberConfirmedState;
            }
            set
            {
                SetPropertyValue("CreateAlrMemberConfirmedState", ref _CreateAlrMemberConfirmedState, value);
            }
        }
        private bool? _CreateAlrMemberConfirm;
        public bool? CreateAlrMemberConfirm
        {
            get
            {
                return _CreateAlrMemberConfirm;
            }
            set
            {
                SetPropertyValue("CreateAlrMemberConfirm", ref _CreateAlrMemberConfirm, value);
            }
        }
        private bool? _CreateAlrMemberSuppressedState;
        public bool? CreateAlrMemberSuppressedState
        {
            get
            {
                return _CreateAlrMemberSuppressedState;
            }
            set
            {
                SetPropertyValue("CreateAlrMemberSuppressedState", ref _CreateAlrMemberSuppressedState, value);
            }
        }
        private bool? _CreateAlrMemberShelvingState;
        public bool? CreateAlrMemberShelvingState
        {
            get
            {
                return _CreateAlrMemberShelvingState;
            }
            set
            {
                SetPropertyValue("CreateAlrMemberShelvingState", ref _CreateAlrMemberShelvingState, value);
            }
        }
        private bool? _CreateAlrMemberEnabledState;
        public bool? CreateAlrMemberEnabledState
        {
            get
            {
                return _CreateAlrMemberEnabledState;
            }
            set
            {
                SetPropertyValue("CreateAlrMemberEnabledState", ref _CreateAlrMemberEnabledState, value);
            }
        }
        private bool? _CreateAlrMemberActiveState;
        public bool? CreateAlrMemberActiveState
        {
            get
            {
                return _CreateAlrMemberActiveState;
            }
            set
            {
                SetPropertyValue("CreateAlrMemberActiveState", ref _CreateAlrMemberActiveState, value);
            }
        }
        private bool? _CreateAlrMemberQuality;
        public bool? CreateAlrMemberQuality
        {
            get
            {
                return _CreateAlrMemberQuality;
            }
            set
            {
                SetPropertyValue("CreateAlrMemberQuality", ref _CreateAlrMemberQuality, value);
            }
        }
#if NET_STANDARD
        private bool? _CreateAlrMemberLocalTime;
        public bool? CreateAlrMemberLocalTime
        {
            get
            {
                return _CreateAlrMemberLocalTime;
            }
            set
            {
                SetPropertyValue("CreateAlrMemberLocalTime", ref _CreateAlrMemberLocalTime, value);
            }
        }
#endif

        private UFUAAlarmSource _UFUAAlarmDefinitions;
        [Association("UFUAAlarmSource-UFUAAlarmDefinitions")]
        public UFUAAlarmSource UFUAAlarmDefinitions
        {
            get
            {
                return _UFUAAlarmDefinitions;
            }
            set
            {
                SetPropertyValue("UFAlarmDefinitions", ref _UFUAAlarmDefinitions, value);
            }
        }

        [Browsable(false)]
        [NonPersistent]
        [Exportable]
        public String FolderPath
        {
            get
            {
                if (UFUAAlarmDefinitions != null)
                    return UFUAAlarmDefinitions.GetRelativeName();
                else return string.Empty;
            }
        }
        
        //[Association("UFUATag-UFUAAlarmDefinition")]
        //public XPCollection<UFUATag> UFUATags
        //{
        //    get
        //    {
        //        return GetCollection<UFUATag>("UFUATags");
        //    }
        //}

        [Association("UFUAAlarmDefinition-UFUAAlarmThresholds"), Aggregated]
        public XPCollection<UFUAAlarmThreshold> UFUAAlarmThresholds
        {
            get
            {
                return GetCollection<UFUAAlarmThreshold>("UFUAAlarmThresholds");
            }
        }
            #endregion

#if !NET_STANDARD
            #region IUndoRedoXpo
        [Browsable(false)]
        [NonPersistent]
        public String PathIdentifier
        {
            get
            {
                if (UFUAAlarmDefinitions != null)
                    return String.Format("{0}\\{1}", UFUAAlarmDefinitions.PathIdentifier, Name);
                else
                    return Name;
            }
        }

        [Browsable(false)]
        [NonPersistent]
        public String UniqueIdentifier
        {
            get
            {
                return NodeId.ToString();
            }
        }

        [Browsable(false)]
        [NonPersistent]
        public String ParentIdentifier
        {
            get
            {
                if (UFUAAlarmDefinitions != null)
                    return UFUAAlarmDefinitions.UniqueIdentifier;
                return String.Empty;
            }
        }

        [Browsable(false)]
        [NonPersistent]
        public String OwnerIdentifier
        {
            get
            {
                return String.Empty;
            }
        }

        [Browsable(false)]
        [NonPersistent]
        public String ParentOwnerIdentifier
        {
            get
            {
                return String.Empty;
            }
        }
            #endregion
#endif

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
                if (propertyName == "EnableHighHighLimit" || propertyName == "HighHighLimit" ||
                    propertyName == "EnableHighLimit" || propertyName == "HighLimit" ||
                    propertyName == "EnableLowLimit" || propertyName == "LowLimit" ||
                    propertyName == "EnableLowLowLimit" || propertyName == "LowLowLimit")

                {
                    return AlarmType != UFUAModel.AlarmType.TripAlarm;
                }
                else if (propertyName == "ConditionType")
                {
                    return AlarmType == UFUAModel.AlarmType.TripAlarm;
                }
                else if (propertyName == "ActivationLowValue")
                {
                    return AlarmType == UFUAModel.AlarmType.TripAlarm && StatisticData != StatDef.StatProps.TotalTimeOn && ConditionType == UFUAModel.ConditionType.Between;
                }
                else if (propertyName == "ActivationValue")
                {
                    return AlarmType == UFUAModel.AlarmType.TripAlarm && StatisticData != StatDef.StatProps.TotalTimeOn;
                }
                else if (propertyName == "TotalTimeOn")
                {
                    return AlarmType == UFUAModel.AlarmType.TripAlarm && StatisticData == StatDef.StatProps.TotalTimeOn;
                }
                else if (propertyName == "DeviationType")
                {
                    return AlarmType == UFUAModel.AlarmType.ExclusiveDeviation ||
                        AlarmType == UFUAModel.AlarmType.ExclusiveRateOfChange ||
                        AlarmType == UFUAModel.AlarmType.NonExclusiveDeviation ||
                        AlarmType == UFUAModel.AlarmType.NonExclusiveRateOfChange;
                }
                else if (propertyName == "TimeUnit")
                {
                    return AlarmType == UFUAModel.AlarmType.ExclusiveRateOfChange ||
                        AlarmType == UFUAModel.AlarmType.NonExclusiveRateOfChange;
                }
                else if (propertyName == "DelayTimeOn" || propertyName == "DelayTimeOff")
                {
                    return AlarmType != UFUAModel.AlarmType.ExclusiveRateOfChange &&
                        AlarmType != UFUAModel.AlarmType.NonExclusiveRateOfChange;
                }
                else if (propertyName == "SupportAck" || propertyName == "SupportReset")
                {
                    return Severity > 0;
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

            #region INotifyPropertyReadOnlyChanged Members

        /// <summary>
        /// Gets the readonly state for the property with the given name.
        /// </summary>
        /// <param name="propertyName">The property name that you want konw the current readonly state.</param>
        /// <returns></returns>
        bool INotifyPropertyReadOnlyChanged.this[string propertyName]
        {
            get
            {
                if (propertyName == "ConditionType")
                    return StatisticData == StatDef.StatProps.TotalTimeOn;

                return false;
            }
        }

        /// <summary>
        /// Raised when a property readonly state on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyReadOnlyChanged;

        /// <summary>
        /// Raises this object's PropertyReadOnlyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has changed his value and has triggered the change of readonly.</param>
        protected void OnPropertyReadOnlyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyReadOnlyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

            #endregion
        protected virtual String PerformValidation(String propertyName)
        {
#if !NET_STANDARD
            if (propertyName == "Name")
            {
                if (!Helpers.NameValidator.IsValidAlarmName(Name))
                {
                    return Properties.Resources.AlarmDefinitionNameInvalid;
                }
                else if (UFUAAlarmDefinitions != null)
                {
                    if ((from c in UFUAAlarmDefinitions.UFUAAlarmDefinitions/*.AsParallel()*/
                         where c != this
                         select c)
                            .AsEnumerable()
                            .Where(c => String.Compare(c.Name, Name, true) == 0)
                            .Any())
                    {
                        return Properties.Resources.AlarmDefinitionNameAlreadyExists;
                    }
                }
                else
                {
                    if ((from c in new XPQuery<UFUAModel.UFUAAlarmDefinition>(Session, true)/*.AsParallel()*/
                         where c != this && UFUAAlarmDefinitions == null && c.Name == Name
                         select c).ToList().Count > 0)
                    {
                        return Properties.Resources.AlarmDefinitionNameAlreadyExists;
                    }
                }
            }
            else if (propertyName == "ActivationLowValue" || propertyName == "ActivationValue")
            {
                if (AlarmType == UFUAModel.AlarmType.TripAlarm && ConditionType == UFUAModel.ConditionType.Between && ActivationLowValue >= ActivationValue)
                    return Properties.Resources.AlarmHighLowInvalid;
            }
            else if (propertyName == "HighHighLimit")
            {
                if (EnableHighHighLimit && EnableHighLimit && HighLimit >= HighHighLimit)
                    return Properties.Resources.AlarmHighLowInvalid;
            }
            else if (propertyName == "HighLimit")
            {
                if (EnableHighLimit && EnableHighHighLimit && HighLimit >= HighHighLimit)
                    return Properties.Resources.AlarmHighLowInvalid;
            }
            else if (propertyName == "LowLimit")
            {
                if (EnableLowLimit && EnableHighLimit && LowLimit >= HighLimit)
                    return Properties.Resources.AlarmHighLowInvalid;
            }
            else if (propertyName == "LowLowLimit")
            {
                if (EnableLowLowLimit && EnableLowLimit && LowLowLimit >= LowLimit)
                    return Properties.Resources.AlarmHighLowInvalid;
            }
            else if (propertyName == "Severity")
            {
                if (Severity < 0 || Severity > 1000)
                    return Properties.Resources.AlarmSeverityValueInvalid;
            }
            else if (propertyName == "TimeUnit")
            {
                if ((AlarmType == UFUAModel.AlarmType.ExclusiveRateOfChange || AlarmType == UFUAModel.AlarmType.NonExclusiveRateOfChange) && TimeUnit == TimeSpan.Zero)
                    return Properties.Resources.AlarmDelayTimeInvalid;
            }
            else if (propertyName == "StatisticData")
            {
                if (StatisticData == StatDef.StatProps.TotalTimeOn && AlarmType != UFUAModel.AlarmType.TripAlarm)
                    return Properties.Resources.AlarmStatisticTotalTimeOnNotSupported;
            }
#endif
            return null;
        }
    }
}
