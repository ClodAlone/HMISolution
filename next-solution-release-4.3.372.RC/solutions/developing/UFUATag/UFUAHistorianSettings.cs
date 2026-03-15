using System;
using System.Linq;
using DevExpress.Xpo;
using Opc.Ua;
using System.ComponentModel;
using System.Collections.Generic;
using Utilities;
using UFInterfaces.PropertyControl;
#if !NET_STANDARD
using Utilities.Xpo.UndoRedo;
#endif

namespace UFUAModel
{
    [Exportable(RequiredKeys = new string[] { "Name" })]
    [DeferredDeletion(false)]
    public class UFUAHistorianSettings : XPObject, INotifyPropertyVisibilityChanged, IDataErrorInfo
#if !NET_STANDARD
        , IUndoRedoXpo
#endif
    {
        #region Constructors
        public UFUAHistorianSettings(Session session)
            : base(session)
        { }
        #endregion

        #region Properties Default Values
        // List of constant default values for each property where you want handle a default value.
        const bool defaultEnabled = true;
        const DeviationType defaultExceptionDeviationFormat = UFUAModel.DeviationType.AbsoluteValue;
        public const uint defaultWaitRetryTime = 60;
        public const byte defaultMaxErrorBeforeFlush = 3;
        public const uint defaultMaxErrorCacheSize = 10000;

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

            if (!_Enabled.HasValue)
                _Enabled = defaultEnabled;
            if (!_MaxAge.HasValue)
                _MaxAge = TimeSpan.FromDays(1.0);
            if (!_ExceptionDeviationFormat.HasValue)
                _ExceptionDeviationFormat = defaultExceptionDeviationFormat;
            if (!_WaitRetryTime.HasValue)
                _WaitRetryTime = defaultWaitRetryTime;
            if (!_MaxErrorBeforeFlush.HasValue)
                _MaxErrorBeforeFlush = defaultMaxErrorBeforeFlush;
            if (!_MaxErrorCacheSize.HasValue)
                _MaxErrorCacheSize = defaultMaxErrorCacheSize;
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
                this.RaiseChangeEvent(new ObjectChangeEventArgs(Session, this, "Name", _Name, value));
                SetPropertyValue("Name", ref _Name, value);
            }
        }

        private string _TableName;
        //[Indexed(Unique = false)]
        //[MergablePropertyAttribute(false)]
        [Size(SizeAttribute.Unlimited)]
        [Exportable]
        public string TableName
        {
            get
            {
                return _TableName;
            }
            set
            {
                SetPropertyValue("TableName", ref _TableName, value);
            }
        }

        private bool? _Enabled;
        [Exportable]
        public bool? Enabled
        {
            get
            {
                return _Enabled;
            }
            set
            {
                SetPropertyValue("Enabled", ref _Enabled, value);
            }
        }

        private bool _EnableDataProtection;
        [Exportable]
        public bool EnableDataProtection
        {
            get
            {
                return _EnableDataProtection;
            }
            set
            {
                if (SetPropertyValue("EnableDataProtection", ref _EnableDataProtection, value))
                {
                    RaisePropertyChangedEvent("ConnectionSettings");
                    OnPropertyVisiblityChanged("EnableEventDataProtection");
                }
            }
        }

        private TagEntityReference _EnableTag;
        [ValueConverter(typeof(ConvertTagEntityReference))]
        [Size(SizeAttribute.Unlimited)]
        public TagEntityReference EnableTag
        {
            get
            {
                return _EnableTag;
            }
            set
            {
                SetPropertyValue("EnableTag", ref _EnableTag, value);
            }
        }
        /// <summary>
        /// True in case the tag used to enable the historian is not valid
        /// </summary>
        public bool invalidEnableTag;

        private string _ConnectionSettings;
        [Size(SizeAttribute.Unlimited)]
        [Exportable]
        public string ConnectionSettings
        {
            get
            {
                return _ConnectionSettings;
            }
            set
            {
                SetPropertyValue("ConnectionSettings", ref _ConnectionSettings, value);
            }
        }

        private TimeSpan? _MaxAge;
        [Exportable]
        public TimeSpan? MaxAge
        {
            get
            {
                return _MaxAge;
            }
            set
            {
                SetPropertyValue("MaxAge", ref _MaxAge, value);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("This method is obsolete. Please use MaxAge property instead.")]
        public void set_MaxAge(TimeSpan value)
        {
            MaxAge = value;
        }

        private double _ExceptionDeviation;
        [Exportable]
        public double ExceptionDeviation
        {
            get
            {
                return _ExceptionDeviation;
            }
            set
            {
                SetPropertyValue("ExceptionDeviation", ref _ExceptionDeviation, value);
            }
        }

        private DeviationType? _ExceptionDeviationFormat;
        [Exportable]
        public DeviationType? ExceptionDeviationFormat
        {
            get
            {
                return _ExceptionDeviationFormat;
            }
            set
            {
                SetPropertyValue("ExceptionDeviationFormat", ref _ExceptionDeviationFormat, value);
            }
        }

        private string _Definition;
        [Size(SizeAttribute.Unlimited)]
        [Exportable]
        public string Definition
        {
            get
            {
                return _Definition;
            }
            set
            {
                SetPropertyValue("Definition", ref _Definition, value);
            }
        }

        /// <summary>
        /// Min time to wait for writing records
        /// </summary>
        private TimeSpan _MinTimeInterval;
        [Exportable]
        public TimeSpan MinTimeInterval
        {
            get
            {
                return _MinTimeInterval;
            }
            set
            {
                SetPropertyValue("MinTimeInterval", ref _MinTimeInterval, value);
                this.RaisePropertyChangedEvent("MaxAge");
            }
        }

        /// <summary>
        /// Max time after that a new records will be added anyway
        /// </summary>
        private TimeSpan _MaxTimeInterval;
        [Exportable]
        public TimeSpan MaxTimeInterval
        {
            get
            {
                return _MaxTimeInterval;
            }
            set
            {
                SetPropertyValue("MaxTimeInterval", ref _MaxTimeInterval, value);
            }
        }

        private TagEntityReference _ResettingTag;
        /// <summary>
        /// The UFUATag to use for delete all records from table.
        /// </summary>
        /// <remarks>
        /// The tag is automatically reset after the operation has been performed.
        /// </remarks>
        [ValueConverter(typeof(ConvertTagEntityReference))]
        [Size(SizeAttribute.Unlimited)]
        public TagEntityReference ResettingTag
        {
            get
            {
                return _ResettingTag;
            }
            set
            {
                SetPropertyValue("ResettingTag", ref _ResettingTag, value);
            }
        }
        /// <summary>
        /// True in case the tag used to reset the historian is not valid
        /// </summary>
        public bool invalidResetTag;

        /// <summary>
        /// Time to wait for retring to perform a write operation
        /// </summary>
        private uint? _WaitRetryTime;
        [Exportable]
        public uint? WaitRetryTime
        {
            get
            {
                return _WaitRetryTime;
            }
            set
            {
                SetPropertyValue("WaitRetryTime", ref _WaitRetryTime, value);
            }
        }

        /// <summary>
        /// Max number of errors before flush safely to file
        /// </summary>
        private byte? _MaxErrorBeforeFlush;
        [Exportable]
        public byte? MaxErrorBeforeFlush
        {
            get
            {
                return _MaxErrorBeforeFlush;
            }
            set
            {
                SetPropertyValue("MaxErrorBeforeFlush", ref _MaxErrorBeforeFlush, value);
            }
        }

        /// <summary>
        /// Max number of pending errors before flush safely to file
        /// </summary>
        private uint? _MaxErrorCacheSize;
        [Exportable]
        public uint? MaxErrorCacheSize
        {
            get
            {
                return _MaxErrorCacheSize;
            }
            set
            {
                SetPropertyValue("MaxErrorCacheSize", ref _MaxErrorCacheSize, value);
            }
        }

        private bool _RecordOnlyOnQualityGood;
        [Exportable]
        public bool RecordOnlyOnQualityGood
        {
            get
            {
                return _RecordOnlyOnQualityGood;
            }
            set
            {
                SetPropertyValue("RecordOnlyOnQualityGood", ref _RecordOnlyOnQualityGood, value);
            }
        }

        private bool _Stepped;
        [Category("Aggregation")]
        [Exportable]
        public bool Stepped
        {
            get
            {
                return _Stepped;
            }
            set
            {
                SetPropertyValue("Stepped", ref _Stepped, value);
            }
        }

        private byte _PercentDataGood;
        [Category("Aggregation")]
        [Exportable]
        public byte PercentDataGood
        {
            get
            {
                return _PercentDataGood;
            }
            set
            {
                SetPropertyValue("PercentDataGood", ref _PercentDataGood, value);
            }
        }

        private byte _PercentDataBad;
        [Category("Aggregation")]
        [Exportable]
        public byte PercentDataBad
        {
            get
            {
                return _PercentDataBad;
            }
            set
            {
                SetPropertyValue("PercentDataBad", ref _PercentDataBad, value);
            }
        }

        private bool _UseSlopedExtrapolation;
        [Category("Aggregation")]
        [Exportable]
        public bool UseSlopedExtrapolation
        {
            get
            {
                return _UseSlopedExtrapolation;
            }
            set
            {
                SetPropertyValue("UseSlopedExtrapolation", ref _UseSlopedExtrapolation, value);
            }
        }

        private bool _TreatUncertainAsBad;
        [Category("Aggregation")]
        [Exportable]
        public bool TreatUncertainAsBad
        {
            get
            {
                return _TreatUncertainAsBad;
            }
            set
            {
                SetPropertyValue("TreatUncertainAsBad", ref _TreatUncertainAsBad, value);
            }
        }

        /* managed by report document editor
        [Delayed(true)]
        public byte[] ReportTemplate
        {
            get { return GetDelayedPropertyValue<byte[]>("ReportTemplate"); }
            set { SetDelayedPropertyValue<byte[]>("ReportTemplate", value); }
        }
        */

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
                if (propertyName == "EnableEventDataProtection" || propertyName == "AuditTraceDefaultConnection")
                {
                    return !String.IsNullOrEmpty(UFUAServerInfo.UFUAServerInfo.GetCFR21UserName());
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

#if !NET_STANDARD
        #region IUndoRedoXpo
        [Browsable(false)]
        [NonPersistent]
        public String PathIdentifier
        {
            get
            {
                return Name;
            }
        }

        [Browsable(false)]
        [NonPersistent]
        public String UniqueIdentifier
        {
            get
            {
                return Name;
            }
        }

        [Browsable(false)]
        [NonPersistent]
        public String ParentIdentifier
        {
            get
            {
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
        #endregion
#endif

        protected String PerformValidation(String propertyName)
        {
#if !NET_STANDARD
            if (propertyName == "Name")
            {
                if (String.IsNullOrWhiteSpace(Name))
                {
                    return Properties.Resources.ObjectNameMissing;
                }
                else if ((from c in new XPQuery<UFUAModel.UFUAHistorianSettings>(Session, true)/*.AsParallel()*/
                          where c != this
                          select c)
                            .AsEnumerable()
                            .Where(c => String.Compare(c.Name, Name, true) == 0)
                            .Any())
                {
                    return Properties.Resources.HistoricalSettingsNameAlreadyExists;
                }
            }
            else if (propertyName == "TableName")
            {
                if (!String.IsNullOrEmpty(TableName) && !Helpers.NameValidator.IsValidTableName(TableName))
                {
                    return Properties.Resources.HistoricalSettingsTableNameInvalid;
                }
            }
            else if (propertyName == "EnableTag")
            {
                if (EnableTag != null && !EnableTag.IsEmpty())
                {
                    var listfound = (from tag in new XPQuery<UFUAModel.UFUATag>(Session, true)/*.AsParallel()*/
                                     where tag.NodeId == EnableTag.Guid
                                     select tag).ToList();
                    if (listfound.Count == 0)
                        return Properties.Resources.TagNotFound;
                }
            }
            else if (propertyName == "MaxAge")
            {
                if (MaxAge.HasValue && MaxAge != TimeSpan.Zero && MaxAge <= MinTimeInterval)
                    return Properties.Resources.MaxAgeTooLow;
            }
            else if (propertyName == "MinTimeInterval" || propertyName == "MaxTimeInterval")
            {
                var minTime = TimeSpan.FromMilliseconds(50.0);
                if (MinTimeInterval == MaxTimeInterval &&
                    MinTimeInterval != TimeSpan.Zero && MinTimeInterval < minTime)
                    return Properties.Resources.MinMaxTimeIntervalTooLow;
                else if (propertyName == "MaxTimeInterval" &&
                    MaxTimeInterval != TimeSpan.Zero && MaxTimeInterval < TimeSpan.FromMilliseconds(50.0))
                    return Properties.Resources.MinMaxTimeIntervalTooLow;
            }
            else if (propertyName == "ConnectionSettings")
            {
                if (EnableDataProtection)
                {
                    if (String.IsNullOrEmpty(ConnectionSettings))
                    {
                        var configuration = (from tag in new XPQuery<UFUAModel.UFUAConfiguration>(Session, true).AsParallel() select tag).FirstOrDefault();
                        if (configuration != null)
                        {
                            if (!XpoHelpers.XpoHelper.IsMSSQlDataProvider(configuration.HistorianDefaultConnection))
                                return Properties.Resources.DataProtectionXpoProviderNotSupported;
                        }
                    }
                    else if (!XpoHelpers.XpoHelper.IsMSSQlDataProvider(ConnectionSettings))
                        return Properties.Resources.DataProtectionXpoProviderNotSupported;
                }
            }
            else if (propertyName == "ResettingTag")
            {
                if (ResettingTag != null && !ResettingTag.IsEmpty())
                {
                    var listfound = (from tag in new XPQuery<UFUAModel.UFUATag>(Session, true)/*.AsParallel()*/
                                     where tag.NodeId == ResettingTag.Guid
                                     select tag).ToList();
                    if (listfound.Count == 0)
                        return Properties.Resources.TagEntityReferenceNotFound;
                }
            }
#endif
            return null;
        }
    }
}
