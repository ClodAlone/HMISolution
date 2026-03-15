using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Xpo;
using System.ComponentModel;
using Opc.Ua;
using OPCUAViewModel;
using System.ComponentModel;
using DevExpress.Xpo.Metadata;
using UFUAModel;
using UFInterfaces.PropertyControl;
using Utilities;

namespace ADModel
{
 
    #region Converters for XPObject

    public class ConvertNodeId : ValueConverter
    {

        public override object ConvertFromStorageType(object value)
        {
            var id = value as String;
            if (id == null)
                throw new ArgumentException("Parameters must be a string value");

            return NodeId.Parse((String)value);
        }

        public override object ConvertToStorageType(object value)
        {
            if (value == null)
                return String.Empty;
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
    #endregion

    public class ADNotification : XPObject, IDataErrorInfo, INotifyPropertyVisibilityChanged, XpoHelpers.UndoRedoIXPSimpleObjectHelper.IUniqueIdentifier
    {
        #region Ctor
        public ADNotification(Session session)
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
        const double defaultActivationValue = 1;
        const DeviationType defaultDeviationType = UFUAModel.DeviationType.AbsoluteValue;
        const int defaultSeverity = 100;
        const bool defaultEnableON = true;
        public static char delimiter = ';';

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

            if (!AlarmType.HasValue)
                AlarmType = defaultAlarmType;
            if (!HighHighLimit.HasValue)
                HighHighLimit = defaultHighHighLimit;
            if (!HighLimit.HasValue)
                HighLimit = defaultHighLimit;
            if (!LowLimit.HasValue)
                LowLimit = defaultLowLimit;
            if (!LowLowLimit.HasValue)
                LowLowLimit = defaultLowLowLimit;
            if (!ConditionType.HasValue)
                ConditionType = defaultConditionType;
            if (!ActivationValue.HasValue)
                ActivationValue = defaultActivationValue;
            if (!DeviationType.HasValue)
                DeviationType = defaultDeviationType;
            if (!Severity.HasValue)
                Severity = defaultSeverity;
            if (TimeUnit == TimeSpan.Zero)
                TimeUnit = TimeSpan.FromMinutes(1);
            if (!EnableON.HasValue)
                EnableON = defaultEnableON;
        }
        #endregion

        #region Properties
        private string _Name;
        [MergablePropertyAttribute(false)]
        //[Indexed(Unique = false)]
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

        private NotificationTypes _NotificationType;
        public NotificationTypes NotificationType
        {
            get { return _NotificationType; }
            set
            {
                if(value != _NotificationType)
                {
                    if (!string.IsNullOrEmpty(NotificationItem))
                    {
                        OPCUAEntityReference item = NotificationItem.FromXml<OPCUAEntityReference>();
                        if (value == NotificationTypes.Local && !string.IsNullOrEmpty(item.EndpointUrl) ||
                            value == NotificationTypes.Server && !string.IsNullOrEmpty(item.ReadablePath))
                        {
                            NotificationItem = string.Empty;
                            AlarmName = string.Empty;
                        }
                    }

                }
                SetPropertyValue("NotificationType", ref _NotificationType, value);
                this.RaisePropertyChangedEvent("Severity");
                this.OnPropertyVisiblityChanged("NotificationType");
            }
        }
        private Guid _AlarmNodeId;
        public Guid AlarmNodeId
        {
            get { return _AlarmNodeId; }
            set { SetPropertyValue("AlarmNodeId", ref _AlarmNodeId, value); }
        }

        private string _AlarmName;
        [Size(SizeAttribute.Unlimited)]
        public string AlarmName
        {
            get
            {
                return _AlarmName;
            }
            set
            {
                SetPropertyValue("AlarmName", ref _AlarmName, value);
            }
        }

        private string _Message;
        [Size(SizeAttribute.Unlimited)]
        public string Message
        {
            get
            {
                return _Message;
            }
            set
            {
                SetPropertyValue("Message", ref _Message, value);
            }
        }

        private AlarmType? _AlarmType;
        public AlarmType? AlarmType
        {
            get
            {
                return _AlarmType;
            }
            set
            {
                SetPropertyValue("AlarmType", ref _AlarmType, value);
                this.RaisePropertyChangedEvent("TimeUnit");
                this.OnPropertyVisiblityChanged("AlarmType");
            }
        }

        private bool _EnableHighHighLimit;
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
        public double? HighHighLimit
        {
            get
            {
                return _HighHighLimit;
            }
            set
            {
                SetPropertyValue("HighHighLimit", ref _HighHighLimit, value);
                this.RaisePropertyChangedEvent("HighLimit");
                this.RaisePropertyChangedEvent("LowLimit");
                this.RaisePropertyChangedEvent("LowLowLimit");
            }
        }

        private bool _EnableHighLimit;
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
        public ConditionType? ConditionType
        {
            get
            {
                return _ConditionType;
            }
            set
            {
                SetPropertyValue("ConditionType", ref _ConditionType, value);
            }
        }

        private double? _ActivationValue;
        public double? ActivationValue
        {
            get
            {
                return _ActivationValue;
            }
            set
            {
                SetPropertyValue("ActivationValue", ref _ActivationValue, value);
            }
        }

        private DeviationType? _DeviationType;
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
        public int? Severity
        {
            get
            {
                return _Severity;
            }
            set
            {
                SetPropertyValue("Severity", ref _Severity, value);
            }
        }

        private TimeSpan _TimeUnit;
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

        private TimeSpan _DelayTimeOn;
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

        private TimeSpan _DelayTimeOff;
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

        private string _Recipient;
        [Size(SizeAttribute.Unlimited)]
        public string Recipient
        {
            get
            {
                return _Recipient;
            }
            set
            {
                SetPropertyValue("Recipient", ref _Recipient, value);
            }
        }

        NodeId _RecipientID;
        [ValueConverter(typeof(ConvertNodeId))]
        [ReadOnly(true)]
        [Browsable(false)]
        public NodeId RecipientID
        {
            get
            {
                return _RecipientID;
            }
            set
            {
                SetPropertyValue("RecipientID", ref _RecipientID, value);
            }
        }

        string _MultiRecipientID;
        //[ValueConverter(typeof(ConvertNodeId))]
        [ReadOnly(true)]
        [Browsable(false)]
        [Size(SizeAttribute.Unlimited)]
        public string MultiRecipientID
        {
            get
            {
                return _MultiRecipientID;
            }
            set
            {
                SetPropertyValue("MultiRecipientID", ref _MultiRecipientID, value);
            }
        }

        #region OpcUaEntityReference
        private string _NotificationItem;
        [Size(SizeAttribute.Unlimited)]
        public string NotificationItem
        {
            get { return _NotificationItem; }
            set
            {
                try
                {
                    if(!string.IsNullOrEmpty(value))
                    {
                        OPCUAEntityReference item = value.FromXml<OPCUAEntityReference>();
                        if (item == null)
                            NotificationItemName = string.Empty;
                        else
                        {
                            Opc.Ua.NamespaceTable n = new Opc.Ua.NamespaceTable();
                            UInt16 ns = (UInt16)(n.Count + 2 - 1);
                            string oldChars = string.Format("{0}:", ns);
                            if (!string.IsNullOrEmpty(AlarmName))
                                NotificationItemName = string.Format("{0} ({1})", (item.EndpointUrl).Replace(oldChars, ""), item.AppName)/*HumanReadable*/;
                            else
                                NotificationItemName = string.Format("{0} ({1})", (item.ReadablePath).Replace(oldChars, ""), item.AppName)/*HumanReadable*/;

                        }
                    }
                    else
                    {
                        NotificationItemName = string.Empty;
                    }
                }
                catch (Exception)
                {
                    NotificationItemName = string.Empty;
                }

                SetPropertyValue("NotificationItem", ref _NotificationItem, value);
            }
        }

        private string _NotificationItemName;
        [Browsable(false)]
        [Size(SizeAttribute.Unlimited)]
        public string NotificationItemName
        {
            get
            {
                return _NotificationItemName;
            }
            set
            {
                SetPropertyValue("NotificationItemName", ref _NotificationItemName, value);
            }
        }
        #endregion

        private string _Attachments;
        [Size(SizeAttribute.Unlimited)]
        public string Attachments
        {
            get
            {
                return _Attachments;
            }
            set
            {
                SetPropertyValue("Attachments", ref _Attachments, value);
            }
        }

        private string _PluginName;
        [Size(SizeAttribute.Unlimited)]
        public string PluginName
        {
            get
            {
                return _PluginName;
            }
            set
            {
                SetPropertyValue("PluginName", ref _PluginName, value);
                this.OnPropertyVisiblityChanged("NotificationType");
            }
        }
        private bool _ADGroupMessage;
        [Size(SizeAttribute.Unlimited)]
        [Browsable(true)]
        public bool ADGroupMessage
        {
            get
            {
                return _ADGroupMessage;
            }
            set
            {
                SetPropertyValue("ADGroupMessage", ref _ADGroupMessage, value);
            }
        }
        private NodeId _PluginID;
        [ValueConverter(typeof(ConvertNodeId))]
        [ReadOnly(true)]
        public NodeId PluginID
        {
            get
            {
                return _PluginID;
            }
            set
            {
                SetPropertyValue("PluginID", ref _PluginID, value);
            }
        }
        private int _Priority;
        public int Priority
        {
            get
            {
                return _Priority;
            }
            set
            {
                SetPropertyValue("Priority", ref _Priority, value);
            }
        }

        private ADFolder _ADFolder;
        [Association("ADFolder-ADNotifications")]
        [Browsable(false)]
        public ADFolder ADFolder
        {
            get
            {
                return _ADFolder;
            }
            set
            {
                SetPropertyValue("ADFolder", ref _ADFolder, value);
            }
        }

        private bool? _EnableON;
        public bool? EnableON
        {
            get { return _EnableON; }
            set { SetPropertyValue("EnableON", ref _EnableON, value); }
        }
        private bool _EnableOFF;
        public bool EnableOFF
        {
            get { return _EnableOFF; }
            set { SetPropertyValue("EnableOFF", ref _EnableOFF, value); }
        }
        private bool _EnableACK;
        public bool EnableACK
        {
            get { return _EnableACK; }
            set { SetPropertyValue("EnableACK", ref _EnableACK, value); }
        }
        private bool _EnableCONFIRMED;
        public bool EnableCONFIRMED
        {
            get { return _EnableCONFIRMED; }
            set { SetPropertyValue("EnableCONFIRMED", ref _EnableCONFIRMED, value); }
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
                    return AlarmType != UFUAModel.AlarmType.TripAlarm && NotificationType == NotificationTypes.Local;
                }
                else if (propertyName == "AlarmType")
                {
                    return NotificationType == NotificationTypes.Local;
                }
                else if (propertyName == "ConditionType" || propertyName == "ActivationValue")
                {
                    return AlarmType == UFUAModel.AlarmType.TripAlarm && NotificationType == NotificationTypes.Local;
                }
                else if (propertyName == "AlarmName")
                {
                    return false;
                }
                else if (propertyName == "DeviationType")
                {
                    return (AlarmType == UFUAModel.AlarmType.ExclusiveDeviation ||
                        AlarmType == UFUAModel.AlarmType.ExclusiveRateOfChange ||
                        AlarmType == UFUAModel.AlarmType.NonExclusiveDeviation ||

                        AlarmType == UFUAModel.AlarmType.NonExclusiveRateOfChange) && NotificationType == NotificationTypes.Local;
                }
                else if (propertyName == "TimeUnit")
                {
                    return AlarmType == UFUAModel.AlarmType.ExclusiveRateOfChange ||
                        AlarmType == UFUAModel.AlarmType.NonExclusiveRateOfChange;
                }
                else if (propertyName == "DelayTimeOn" || propertyName == "DelayTimeOff")
                {
                    return AlarmType != UFUAModel.AlarmType.ExclusiveRateOfChange &&
                        AlarmType != UFUAModel.AlarmType.NonExclusiveRateOfChange && NotificationType == NotificationTypes.Local;
                }
                else if (propertyName == "EnableACK" || propertyName == "EnableCONFIRMED")
                {
                    return NotificationType != NotificationTypes.Local;
                }
                else if (propertyName == "Attachments")
                {
                    return PluginName == "Smtp mail sender";
                }
                else if (propertyName == "ADGroupMessage")
                {
                    return PluginName == "Telegram sender" && this.ADGroupMessage;
                }
                else if (propertyName == "Severity")
                {
                    return NotificationType == NotificationTypes.Local;
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

        public const int MAXPriority = 9;
        protected String PerformValidation(String propertyName)
        {
            if (propertyName == "Name")
            {
                if(String.IsNullOrWhiteSpace(Name))
                {
                    return Properties.Resources.ItemNameNull;
                }
                else if (ADFolder != null)
                {
                    if ((from c in ADFolder.ADNotifications.AsParallel()
                         where c != this && c.Name == Name select c).ToList().Count > 0)
                    {
                        return Properties.Resources.NotificationNameAlreadyExists;
                    }
                }
                else
                {
                    if ((from c in new XPQuery<ADModel.ADNotification>(Session, true).AsParallel()
                         where c != this && ADFolder == null && c.Name == Name select c).ToList().Count > 0)
                    {
                        return Properties.Resources.NotificationNameAlreadyExists;
                    }
                }
            }
            else if (propertyName == "RecipientID" || propertyName == "Recipient" ||propertyName == "MultiRecipientID")
            {
                if (string.IsNullOrEmpty(Recipient))
                    return Properties.Resources.RecipientIDNull;
            }
            else if (propertyName == "PluginId" || propertyName == "PluginName")
            {
                if (PluginID == null || string.IsNullOrEmpty(PluginName))
                    return Properties.Resources.PluginIDNull;
            }
            else if (propertyName == "NotificationItemName" || propertyName == "NotificationItem")
            {
                if(string.IsNullOrEmpty(NotificationItem))
                    return Properties.Resources.NotificationItemNull;
            }
            //else if (propertyName == "ActivationValue")
            //{
            //    if ((AlarmType == AlarmType.ExclusiveDeviation || AlarmType == AlarmType.NonExclusiveDeviation)
            //        && DeviationType == DeviationType.AbsoluteValue && (ActivationValue > HighLimit || ActivationValue < LowLimit))
            //        return Properties.Resources.AlarmActivationValueInvalid;
            //}
            else if (propertyName == "HighHighLimit")
            {
                if (EnableHighHighLimit && HighLimit >= HighHighLimit)
                    return Properties.Resources.AlarmHighLowInvalid;
            }
            else if (propertyName == "HighLimit")
            {
                if (EnableHighLimit && HighLimit >= HighHighLimit)
                    return Properties.Resources.AlarmHighLowInvalid;
            }
            else if (propertyName == "LowLimit")
            {
                if (EnableLowLimit && LowLimit >= HighLimit)
                    return Properties.Resources.AlarmHighLowInvalid;
            }
            else if (propertyName == "LowLowLimit")
            {
                if (EnableLowLowLimit && LowLowLimit >= LowLimit)
                    return Properties.Resources.AlarmHighLowInvalid;
            }
            else if (propertyName == "Severity")
            {
                if (Severity < 1 || Severity > 1000)
                    return Properties.Resources.AlarmSeverityValueInvalid;
            }
            else if (propertyName == "TimeUnit")
            {
                if ((AlarmType == UFUAModel.AlarmType.ExclusiveRateOfChange || AlarmType == UFUAModel.AlarmType.NonExclusiveRateOfChange) && TimeUnit == TimeSpan.Zero)
                    return Properties.Resources.AlarmDelayTimeInvalid;
            }
            else if(propertyName == "Priority")
            {
                if (Priority < 0 || Priority > MAXPriority)
                    return string.Format(Properties.Resources.PriorityOutOfRange, MAXPriority);
            }

            return null;
        }

    }
}
