using System;
using System.Linq;
using DevExpress.Xpo;
using System.ComponentModel;
using System.Collections.Generic;
using UFInterfaces.PropertyControl;
using System.Windows;
using System.Windows.Input;
using Utilities.Converters;
#if !NET_STANDARD
using Utilities.Xpo.UndoRedo;
#endif

namespace UFUAModel
{
    [DeferredDeletion(false)]
    public class UFUAAlarmThreshold : XPObject, IDataErrorInfo, INotifyPropertyVisibilityChanged, INotifyPropertyReadOnlyChanged
#if !NET_STANDARD
        , ICommandSource, IUndoRedoXpo
#endif
    {
        #region Constructors

        public UFUAAlarmThreshold(Session session)
            : base(session)
        {
        }

        public UFUAAlarmThreshold(UFUAAlarmDefinition alarm, Session session)
            : base(session)
        {
            _UFUAAlarmDefinitionRef = alarm;
            _UFUAAlarmDefinitionNodeIdRef = alarm.NodeId;
        }

        #endregion

        #region Properties Default Values
        // List of constant default values for each property where you want handle a default value.
        const bool defaultEnabled = true;
        const ThreeStateType defaultEnableQualityGood = UFUAModel.ThreeStateType.UseDefault;
        const ThreeStateType defaultBeep = UFUAModel.ThreeStateType.UseDefault;

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
            if (!_EnableQualityGood.HasValue)
                _EnableQualityGood = defaultEnableQualityGood;
            if (!_Beep.HasValue)
                _Beep = defaultBeep;
        }
        #endregion

        #region Not Persistence Properties

        [Browsable(false)]
        [NonPersistent]
        public string Name
        {
            get
            {
                if (!IsValid)
                    return Properties.Resources.InvalidThresholdText;

                var suffix = new System.Text.StringBuilder(UFUATagAss.Name);
                suffix.Append(GetFriendlyExpressionString());
                suffix.AppendFormat(":{0}", UFUAAlarmDefinitionRef.Name);

                if (ComposedAlarmText == suffix.ToString())
                    return suffix.ToString();
                else
                    return String.Format("{0} ({1})", suffix, ComposedAlarmText);
            }
        }

        [Browsable(false)]
        [NonPersistent]
        public string CompleteName
        {
            get
            {
                if (!IsValid)
                    return Properties.Resources.InvalidThresholdText;

                return String.Format("{0} - {1}", Name, UFUAAlarmDefinitionRef.SourcePath);
            }
        }

        [Browsable(false)]
        [NonPersistent]
        public bool IsValid
        {
            get
            {
                return UFUATagAss != null && UFUAAlarmDefinitionRef != null;
            }
        }

        [Browsable(false)]
        [NonPersistent]
        public bool IsBeepEnabled
        {
            get
            {
                if (UFUAAlarmDefinitionRef != null && Beep == ThreeStateType.UseDefault)
                    return UFUAAlarmDefinitionRef.Beep.Value;

                return Beep == ThreeStateType.ForceTrue;
            }
        }

        [Browsable(false)]
        [NonPersistent]
        public string SoundFile => UFUAAlarmDefinitionRef?.SoundFile;

        [Browsable(false)]
        [NonPersistent]
        public bool RepeatSoundContinuously => UFUAAlarmDefinitionRef?.RepeatSoundContinuously ?? false;

        [Browsable(false)]
        [NonPersistent]
        public int Severity => UFUAAlarmDefinitionRef?.Severity ?? 0;
        
        [Browsable(false)]
        [NonPersistent]
        public bool IsQualityGoodEnabled
        {
            get
            {
                if (UFUAAlarmDefinitionRef != null && EnableQualityGood == ThreeStateType.UseDefault)
                    return UFUAAlarmDefinitionRef.EnableQualityGood.Value;

                return EnableQualityGood == ThreeStateType.ForceTrue;
            }
        }

        [Browsable(false)]
        [NonPersistent]
        public bool HasCRCommands
        {
            get
            {
                return HasCommands || !String.IsNullOrEmpty(CommandsDbClick);
            }
        }

        [Browsable(false)]
        [NonPersistent]
        public bool HasCommands
        {
            get
            {
                return !String.IsNullOrEmpty(CommandsOn) ||
                    !String.IsNullOrEmpty(CommandsOff) ||
                    !String.IsNullOrEmpty(CommandsAck) ||
                    !String.IsNullOrEmpty(CommandsReset);
            }
        }

        [Browsable(false)]
        [NonPersistent]
        public AlarmType? AlarmType
        {
            get
            {
                if (UFUAAlarmDefinitionRef != null)
                    return UFUAAlarmDefinitionRef.AlarmType;

                return null;

            }
        }

        #endregion

        #region Properties
        DateTime _SettingsTimeStamp;
        [Browsable(false)]
        public DateTime SettingsTimeStamp
        {
            get
            {
                return _SettingsTimeStamp;
            }
            set
            {
                if (value != DateTime.MinValue)
                    SetPropertyValue("SettingsTimeStamp", ref _SettingsTimeStamp, value);
            }
        }

        private Guid _UFUAAlarmDefinitionNodeIdRef;
        [Browsable(false)]
        public Guid UFUAAlarmDefinitionNodeIdRef
        {
            get
            {
                return _UFUAAlarmDefinitionNodeIdRef;
            }
            set
            {
                SetPropertyValue("UFUAAlarmDefinitionNodeIdRef", ref _UFUAAlarmDefinitionNodeIdRef, value);
            }
        }

        private string _AlarmText;
        [Size(SizeAttribute.Unlimited)]
        public string AlarmText
        {
            get
            {
                return _AlarmText;
            }
            set
            {
                SetPropertyValue("AlarmText", ref _AlarmText, value);
                this.RaisePropertyChangedEvent("Name");
                this.RaisePropertyChangedEvent("ComposedAlarmText");
            }
        }

        [NonPersistent]
        public string ComposedAlarmText
        {
            get
            {
                return GetAlarmMessage(UFUATagAss != null ? UFUATagAss.GetRelativeName() : Name);
            }
        }

        private bool _AddTagDescription;
        public bool AddTagDescription
        {
            get
            {
                return _AddTagDescription;
            }
            set
            {
                SetPropertyValue("AddTagDescription", ref _AddTagDescription, value);
                this.RaisePropertyChangedEvent("ComposedAlarmText");
            }
        }

        private bool? _Enabled;
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
                if (SetPropertyValue("EnableTag", ref _EnableTag, value))
                {
                    if (!IsLoading)
                    {
                        SettingsTimeStamp = DateTime.UtcNow;
                    }
                }
            }
        }

        private TagEntityReference _SeverityTag;
        /// <summary>
        /// Tag to set dynamically the alarm severity
        /// </summary>
        [ValueConverter(typeof(ConvertTagEntityReference))]
        [Size(SizeAttribute.Unlimited)]
        public TagEntityReference SeverityTag
        {
            get
            {
                return _SeverityTag;
            }
            set
            {
                if (SetPropertyValue("SeverityTag", ref _SeverityTag, value))
                {
                    this.RaisePropertyChangedEvent("SeverityTag");
                }
            }
        }

        private TagEntityReference _ActivationLowValueTag;
        [ValueConverter(typeof(ConvertTagEntityReference))]
        [Size(SizeAttribute.Unlimited)]
        public TagEntityReference ActivationLowValueTag
        {
            get
            {
                return _ActivationLowValueTag;
            }
            set
            {
                SetPropertyValue("ActivationLowValueTag", ref _ActivationLowValueTag, value);
            }
        }

        private TagEntityReference _ActivationValueTag;
        [ValueConverter(typeof(ConvertTagEntityReference))]
        [Size(SizeAttribute.Unlimited)]
        public TagEntityReference ActivationValueTag
        {
            get
            {
                return _ActivationValueTag;
            }
            set
            {
                SetPropertyValue("ActivationValueTag", ref _ActivationValueTag, value);
            }
        }

        private TagEntityReference _HighHighLimitTag;
        [ValueConverter(typeof(ConvertTagEntityReference))]
        [Size(SizeAttribute.Unlimited)]
        public TagEntityReference HighHighLimitTag
        {
            get
            {
                return _HighHighLimitTag;
            }
            set
            {
                SetPropertyValue("HighHighLimitTag", ref _HighHighLimitTag, value);
            }
        }

        private TagEntityReference _HighLimitTag;
        [ValueConverter(typeof(ConvertTagEntityReference))]
        [Size(SizeAttribute.Unlimited)]
        public TagEntityReference HighLimitTag
        {
            get
            {
                return _HighLimitTag;
            }
            set
            {
                SetPropertyValue("HighLimitTag", ref _HighLimitTag, value);
            }
        }

        private TagEntityReference _LowLimitTag;
        [ValueConverter(typeof(ConvertTagEntityReference))]
        [Size(SizeAttribute.Unlimited)]
        public TagEntityReference LowLimitTag
        {
            get
            {
                return _LowLimitTag;
            }
            set
            {
                SetPropertyValue("LowLimitTag", ref _LowLimitTag, value);
            }
        }

        private TagEntityReference _LowLowLimitTag;
        [ValueConverter(typeof(ConvertTagEntityReference))]
        [Size(SizeAttribute.Unlimited)]
        public TagEntityReference LowLowLimitTag
        {
            get
            {
                return _LowLowLimitTag;
            }
            set
            {
                SetPropertyValue("LowLowLimitTag", ref _LowLowLimitTag, value);
            }
        }

        Helpers.CustomXPCollection<XPTagEntityReference> _AliasTags;
        [Association("UFUAAlarmThreshold-AliasTags"), Aggregated]
        public Helpers.CustomXPCollection<XPTagEntityReference> AliasTags
        {
            get
            {
                if (_AliasTags == null)
                {
                    var member = base.ClassInfo.GetMember("AliasTags");
                    _AliasTags = new Helpers.CustomXPCollection<XPTagEntityReference>(base.Session, this, member);
                }

                return _AliasTags;
            }
            set
            {
                if (_AliasTags != null)
                {
                    while (_AliasTags.Count > 0)
                        _AliasTags[0].Delete();

                    if (value != null && value.Count > 0)
                    {
                        foreach (var item in value)
                            _AliasTags.Add(item);
                    }
                }
            }
        }

        private ThreeStateType? _EnableQualityGood;
        public ThreeStateType? EnableQualityGood
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

        private ThreeStateType? _Beep;
        public ThreeStateType? Beep
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

        private string _Expression;
        [Size(SizeAttribute.Unlimited)]
        public string Expression
        {
            get
            {
                return _Expression;
            }
            set
            {
                SetPropertyValue("Expression", ref _Expression, value);
                this.RaisePropertyChangedEvent("Name");
                this.RaisePropertyChangedEvent("ComposedAlarmText");
            }
        }

        private string _SeverityExpression;
        /// <summary>
        /// Expression associated to the Severity Tag
        /// </summary>
        [Size(SizeAttribute.Unlimited)]
        public string SeverityExpression
        {
            get
            {
                return _SeverityExpression;
            }
            set
            {
                SetPropertyValue("SeverityExpression", ref _SeverityExpression, value);
            }
        }

        private string _CommandsOn;
        [Category("Commands")]
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
        [Category("Commands")]
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

        private string _CommandsAck;
        [Category("Commands")]
        [Size(SizeAttribute.Unlimited)]
        public string CommandsAck
        {
            get
            {
                return _CommandsAck;
            }
            set
            {
                SetPropertyValue("CommandsAck", ref _CommandsAck, value);
            }
        }

        private string _CommandsReset;
        [Category("Commands")]
        [Size(SizeAttribute.Unlimited)]
        public string CommandsReset
        {
            get
            {
                return _CommandsReset;
            }
            set
            {
                SetPropertyValue("CommandsReset", ref _CommandsReset, value);
            }
        }

        private string _CommandsDbClick;
        [Category("Commands")]
        [Size(SizeAttribute.Unlimited)]
        public string CommandsDbClick
        {
            get
            {
                return _CommandsDbClick;
            }
            set
            {
                SetPropertyValue("CommandsDbClick", ref _CommandsDbClick, value);
            }
        }

        private UFUATag _UFUATagAss;
        [Association("UFUATag-UFUAAlarmThreshold")]
        [Browsable(false)]
        public UFUATag UFUATagAss
        {
            get
            {
                return _UFUATagAss;
            }
            set
            {
                SetPropertyValue("UFUATagAss", ref _UFUATagAss, value);
            }
        }

        private UFUAAlarmDefinition _UFUAAlarmDefinitionRef;
        [Browsable(false)]
        [Association("UFUAAlarmDefinition-UFUAAlarmThresholds")]
        public UFUAAlarmDefinition UFUAAlarmDefinitionRef
        {
            get
            {
                return _UFUAAlarmDefinitionRef;
            }
            set
            {
                SetPropertyValue("UFUAAlarmDefinitionRef", ref _UFUAAlarmDefinitionRef, value);
            }
        }

        [Browsable(false)]
        [NonPersistent]
        public String AlarmOptions
        {
            get
            {
                string alarmOptions = $"{Expression}#{AddTagDescription}#{Enabled}#{EnableTag}#{EnableQualityGood}#{ActivationLowValueTag}#{ActivationValueTag}#{Beep}#{Oid}#{HighHighLimitTag}#{HighLimitTag}#{LowLimitTag}#{LowLowLimitTag}#{SeverityTag}#{SeverityExpression}";
                if (AliasTags.Count > 0)
                    AliasTags.ToList().ForEach(alias => alarmOptions = $"{alarmOptions}#{alias.TagEntity}#{alias.Oid}");
                return alarmOptions;
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

        protected override void OnDeleted()
        {
            base.OnDeleted();

            if (_AliasTags != null)
                _AliasTags.Dispose();
        }

        #endregion

        #region Methods
        public string GetAlarmMessage(String startName)
        {
            if (!String.IsNullOrEmpty(AlarmText))
                return AlarmText;

            var suffix = new System.Text.StringBuilder(startName);
            suffix.Append(GetFriendlyExpressionString());
            suffix.AppendFormat(":{0}", UFUAAlarmDefinitionRef.Name);
            return suffix.ToString();
        }

        public String GetFriendlyExpressionString()
        {
            if (String.IsNullOrEmpty(Expression))
                return String.Empty;

            var type = ExpressionValueConverter.GetFormulaType(Expression);
            if (type == ExpressionType.Array ||
                type == ExpressionType.ArrayPlusBit ||
                type == ExpressionType.Bit)
                return Expression;
            else
                return String.Empty;
        }

        public String GetUniqueConditionName()
        {
            if (!IsValid)
                return null;

            Opc.Ua.NamespaceTable n = new Opc.Ua.NamespaceTable();
            UInt16 ns = (UInt16)(n.Count + 2 - 1);
            var instanceTag = UFUATagAss;
            var relativeName = instanceTag.GetRelativePath(ns, bRelative: true).Replace(String.Format("{0}:", ns), "").Replace('/', '.');
            if (instanceTag.IsSubPrototypeMember)
            {
                while (instanceTag.IsSubPrototypeMember)
                {
                    instanceTag = instanceTag.PrototypeReference.UFUATagOwner;
                    relativeName = String.Format("{0}.{1}", instanceTag.GetRelativePath(ns, bRelative: true).Replace(String.Format("{0}:", ns), "").Replace('/', '.'), relativeName);
                }
            }
            var relativePath = String.Format("{0}.{1}", UFUAServerInfo.UFUAServerInfo.GetTagRootName(), relativeName);
            var alarmDefinitionFullPath = UFUAAlarmDefinitionRef.FolderPath;
            if (String.IsNullOrEmpty(alarmDefinitionFullPath))
                alarmDefinitionFullPath = UFUAAlarmDefinitionRef.Name;
            else
                alarmDefinitionFullPath = String.Format("{0}{1}{2}", alarmDefinitionFullPath, UFUAArea.AreaSeparator, UFUAAlarmDefinitionRef.Name);
            if (!String.IsNullOrEmpty(Expression))
                return String.Format("{0}:{1}{2}", relativePath, alarmDefinitionFullPath, Expression);
            else
                return String.Format("{0}:{1}", relativePath, alarmDefinitionFullPath);
        }

        #endregion

#if !NET_STANDARD
        #region ICommandSource Members
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
        #endregion

        #region IUndoRedoXpo
        [Browsable(false)]
        [NonPersistent]
        public String PathIdentifier
        {
            get
            {
                if (UFUATagAss != null)
                    return String.Format("{0}\\{1}", UFUATagAss.PathIdentifier, Name);
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
                return GetUniqueConditionName();
            }
        }

        [Browsable(false)]
        [NonPersistent]
        public String ParentIdentifier
        {
            get
            {
                if (UFUATagAss != null)
                    return UFUATagAss.UniqueIdentifier;
                return String.Empty;
            }
        }

        [Browsable(false)]
        [NonPersistent]
        public String OwnerIdentifier
        {
            get
            {
                if (UFUATagAss != null)
                    return UFUATagAss.OwnerIdentifier;
                return String.Empty;
            }
        }

        [Browsable(false)]
        [NonPersistent]
        public String ParentOwnerIdentifier
        {
            get
            {
                if (UFUATagAss != null)
                {
                    var prototypeReference = UFUATagAss.PrototypeReference;
                    if (prototypeReference != null && prototypeReference.UFUATagOwner != null)
                        return prototypeReference.UFUATagOwner.UniqueIdentifier;
                }
                return String.Empty;
            }
        }
        #endregion
#endif

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
                if (propertyName == "ActivationLowValueTag")
                {
                    return UFUAAlarmDefinitionRef != null &&
                        UFUAAlarmDefinitionRef.AlarmType == UFUAModel.AlarmType.TripAlarm &&
                        UFUAAlarmDefinitionRef.ConditionType == ConditionType.Between;
                }
                else if (propertyName == "ActivationValueTag")
                {
                    return UFUAAlarmDefinitionRef != null && 
                        UFUAAlarmDefinitionRef.AlarmType == UFUAModel.AlarmType.TripAlarm;
                }
                else if (propertyName == "HighHighLimitTag")
                {
                    return UFUAAlarmDefinitionRef != null && 
                        UFUAAlarmDefinitionRef.AlarmType != UFUAModel.AlarmType.TripAlarm &&
                        UFUAAlarmDefinitionRef.EnableHighHighLimit;
                }
                else if (propertyName == "HighLimitTag")
                {
                    return UFUAAlarmDefinitionRef != null &&
                        UFUAAlarmDefinitionRef.AlarmType != UFUAModel.AlarmType.TripAlarm &&
                        UFUAAlarmDefinitionRef.EnableHighLimit;
                }
                else if (propertyName == "LowLimitTag")
                {
                    return UFUAAlarmDefinitionRef != null &&
                        UFUAAlarmDefinitionRef.AlarmType != UFUAModel.AlarmType.TripAlarm &&
                        UFUAAlarmDefinitionRef.EnableLowLimit;
                }
                else if (propertyName == "LowLowLimitTag")
                {
                    return UFUAAlarmDefinitionRef != null &&
                        UFUAAlarmDefinitionRef.AlarmType != UFUAModel.AlarmType.TripAlarm &&
                        UFUAAlarmDefinitionRef.EnableLowLowLimit;
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
                if (UFUATagAss != null && UFUATagAss.IsSubPrototypeMember && UFUATagAss.UseShared.Value)
                    return true;

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

        protected String PerformValidation(String propertyName)
        {
#if !NET_STANDARD
            if (propertyName == "EnableTag")
            {
                if (EnableTag != null && !EnableTag.IsEmpty())
                {
                    var listfound = (from tag in new XPQuery<UFUAModel.UFUATag>(Session, true)/*.AsParallel()*/
                                     where tag.NodeId == EnableTag.Guid
                                     select tag).ToList();
                    if (listfound.Count == 0)
                        return Properties.Resources.TagNotFound;
                    else if (listfound[0] == UFUATagAss)
                        return Properties.Resources.EnableThresholdTagIsRecursive;
                }
            }
            else if (propertyName == "ActivationLowValueTag")
            {
                if (ActivationLowValueTag != null && !ActivationLowValueTag.IsEmpty())
                {
                    var listfound = (from tag in new XPQuery<UFUAModel.UFUATag>(Session, true)/*.AsParallel()*/
                                     where tag.NodeId == ActivationLowValueTag.Guid
                                     select tag).ToList();
                    if (listfound.Count == 0)
                        return Properties.Resources.TagNotFound;
                    else if (listfound[0] == UFUATagAss)
                        return Properties.Resources.EnableThresholdTagIsRecursive;
                }
            }
            else if (propertyName == "ActivationValueTag")
            {
                if (ActivationValueTag != null && !ActivationValueTag.IsEmpty())
                {
                    var listfound = (from tag in new XPQuery<UFUAModel.UFUATag>(Session, true)/*.AsParallel()*/
                                     where tag.NodeId == ActivationValueTag.Guid
                                     select tag).ToList();
                    if (listfound.Count == 0)
                        return Properties.Resources.TagNotFound;
                    else if (listfound[0] == UFUATagAss)
                        return Properties.Resources.EnableThresholdTagIsRecursive;
                }
            }
            else if (propertyName == "HighHighLimitTag")
            {
                if (HighHighLimitTag != null && !HighHighLimitTag.IsEmpty())
                {
                    var listfound = (from tag in new XPQuery<UFUAModel.UFUATag>(Session, true)/*.AsParallel()*/
                                     where tag.NodeId == HighHighLimitTag.Guid
                                     select tag).ToList();
                    if (listfound.Count == 0)
                        return Properties.Resources.TagNotFound;
                    else if (listfound[0] == UFUATagAss)
                        return Properties.Resources.EnableThresholdTagIsRecursive;
                }
            }
            else if (propertyName == "HighLimitTag")
            {
                if (HighLimitTag != null && !HighLimitTag.IsEmpty())
                {
                    var listfound = (from tag in new XPQuery<UFUAModel.UFUATag>(Session, true)/*.AsParallel()*/
                                     where tag.NodeId == HighLimitTag.Guid
                                     select tag).ToList();
                    if (listfound.Count == 0)
                        return Properties.Resources.TagNotFound;
                    else if (listfound[0] == UFUATagAss)
                        return Properties.Resources.EnableThresholdTagIsRecursive;
                }
            }
            else if (propertyName == "LowLimitTag")
            {
                if (LowLimitTag != null && !LowLimitTag.IsEmpty())
                {
                    var listfound = (from tag in new XPQuery<UFUAModel.UFUATag>(Session, true)/*.AsParallel()*/
                                     where tag.NodeId == LowLimitTag.Guid
                                     select tag).ToList();
                    if (listfound.Count == 0)
                        return Properties.Resources.TagNotFound;
                    else if (listfound[0] == UFUATagAss)
                        return Properties.Resources.EnableThresholdTagIsRecursive;
                }
            }
            else if (propertyName == "LowLowLimitTag")
            {
                if (LowLowLimitTag != null && !LowLowLimitTag.IsEmpty())
                {
                    var listfound = (from tag in new XPQuery<UFUAModel.UFUATag>(Session, true)/*.AsParallel()*/
                                     where tag.NodeId == LowLowLimitTag.Guid
                                     select tag).ToList();
                    if (listfound.Count == 0)
                        return Properties.Resources.TagNotFound;
                    else if (listfound[0] == UFUATagAss)
                        return Properties.Resources.EnableThresholdTagIsRecursive;
                }
            }
            else if (propertyName == "Expression" )
            {
                if (!String.IsNullOrEmpty(Expression))
                {
                    if (UFUAAlarmDefinitionRef != null && UFUAAlarmDefinitionRef.StatisticData != StatDef.StatProps.None)
                    {
                        return Properties.Resources.AlarmThresholdExpressionNotSupportedOnStatistic;
                    }
                    else
                    {
                        var type = ExpressionValueConverter.GetFormulaType(Expression);
                        if (type == ExpressionType.none || type == ExpressionType.error)
                            return Properties.Resources.AlarmThresholdExpressionInvalid;
                        else if (type == ExpressionType.Expression)
                        {
                            using (var expressor = new ExpressionValueConverter(Expression))
                            {
                                expressor.ParseFormula();
                                var error = expressor.GetParserError();
                                if (!String.IsNullOrEmpty(error))
                                    return String.Format(Properties.Resources.AlarmThresholdExpressionError, error);
                                else
                                {
                                    var listVariables = expressor.GetFormulaVariables(Expression);
                                    if (listVariables.Count > 0)
                                        return Properties.Resources.AlarmThresholdExpressionInvalid;
                                }
                            }
                        }
                    }
                }
            }
            else if (propertyName == "SeverityTag")
            {
                if (SeverityTag != null && !SeverityTag.IsEmpty())
                {
                    var listfound = (from tag in new XPQuery<UFUATag>(Session, true)
                                     where tag.NodeId == SeverityTag.Guid
                                     select tag).ToList();
                    if (listfound.Count == 0)
                        return Properties.Resources.TagNotFound;
                }
            }
            else if (propertyName == "SeverityExpression")
            {
                if (!String.IsNullOrEmpty(SeverityExpression))
                {
                    var type = ExpressionValueConverter.GetFormulaType(SeverityExpression);
                    if (type == ExpressionType.none || type == ExpressionType.error)
                        return Properties.Resources.AlarmSeverityExpressionInvalid;
                    else if (type == ExpressionType.Expression)
                    {
                        using (var expressor = new ExpressionValueConverter(SeverityExpression))
                        {
                            expressor.ParseFormula();
                            var error = expressor.GetParserError();
                            if (!String.IsNullOrEmpty(error))
                                return String.Format(Properties.Resources.AlarmSeverityExpressionError, error);
                            else
                            {
                                var listVariables = expressor.GetFormulaVariables(SeverityExpression);
                                if (listVariables.Count > 0)
                                    return Properties.Resources.AlarmSeverityExpressionInvalid;
                            }
                        }
                    }
                }
            }
#endif
            return null;
        }
    }
}
