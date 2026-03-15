using System;
using System.Linq;
using DevExpress.Xpo;
using Opc.Ua;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using UFInterfaces.Editors;
using System.Text.RegularExpressions;
using System.Reflection;
#if !NET_STANDARD
using DriverSettingsInterfaces;
using UFInterfaces.Scriptable;
using Utilities.Xpo.UndoRedo;
#endif
using UFInterfaces.PropertyControl;
using Utilities;
using System.Dynamic;

namespace UFUAModel
{
    public enum AccessLevels
    {
        /// <summary>
        /// The Variable value cannot be accessed and has no event history.
        /// </summary>
        None = 0x0,

        /// <summary>
        /// The current value of the Variable may be read.
        /// </summary>
        CurrentRead = 0x1,
        
        /// <summary>
        /// The current value of the Variable may be written.
        /// </summary>
        CurrentWrite = 0x2,
        
        /// <summary>
        /// The current value of the Variable may be read or written.
        /// </summary>
        CurrentReadOrWrite = 0x3
    };

    public enum SamplingIntervals
    {
        /// <summary>
        /// The Variable value cannot be accessed and has no event history.
        /// </summary>
        ClientBase = 0x0,

        /// <summary>
        /// The current value of the Variable may be read.
        /// </summary>
        Slow = 0x1,

        /// <summary>
        /// The current value of the Variable may be written.
        /// </summary>
        Medium = 0x2,

        /// <summary>
        /// The current value of the Variable may be read or written.
        /// </summary>
        Fast = 0x3
    };

    [Exportable(RequiredKeys = new string[]{ "PrototypeReferenceName", "FolderPath", "Name" }, ImportFolderInfo = "FolderPath", ImportFolderPrototype = "PrototypeReferenceName", AggregatedProperties = new string[] { "HistorianSettings", "Views", "AlarmList", "PrototypeReferenceName"}, ExternalRefInfo = "Name", ExternalRefValue = "NodeId", UseExternalRef = new string[] { "ScriptCode" }, ImportTagOwnerInfo = "TagOwnerPath", ImportTagProtoNameInfo = "PrototypeName")]
    [DeferredDeletion(false)]
    public class UFUATag : XPObject, IDataErrorInfo, IDynamicSettingsEditing, INotifyPropertyVisibilityChanged, INotifyPropertyReadOnlyChanged
#if !NET_STANDARD
        , IScriptable, IUndoRedoXpo
#endif
    {
        #region Ctor
        public UFUATag(Session session)
            : base(session)
        {
        }
        #endregion

        #region Properties Default Values
        // List of constant default values for each property where you want handle a default value.
        const int defaultMemberOrderId = -1;
        const DataType defaultDataType = UFUAModel.DataType.Float;
        const ModelType defaultModelType = UFUAModel.ModelType.Variable;
        const AccessLevels defaultAccessLevel = AccessLevels.CurrentReadOrWrite;
        const SamplingIntervals defaultSamplingInterval = SamplingIntervals.ClientBase;

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

            if (!_MemberOrderId.HasValue)
                _MemberOrderId = defaultMemberOrderId;
            if (!_DataType.HasValue)
                _DataType = defaultDataType;
            if (!_ModelType.HasValue)
                _ModelType = defaultModelType;
            if (!_AccessLevel.HasValue)
                _AccessLevel = defaultAccessLevel;
            if (!_SamplingInterval.HasValue)
                _SamplingInterval = defaultSamplingInterval;
            if (_SettingsTimeStamp == DateTime.MinValue)
                _SettingsTimeStamp = DateTime.UtcNow;
            if (!_UseShared.HasValue)
                _UseShared = true;
            if (!_AlwaysInUse.HasValue)
                _AlwaysInUse = false;
            if (!_IsRedundancyEnabled.HasValue)
                _IsRedundancyEnabled = true;
            if (!_MaxAuditAge.HasValue)
                _MaxAuditAge = TimeSpan.FromDays(1.0);
        }
        #endregion

        #region Not Persistence Properties

        [Browsable(false)]
        [NonPersistent]
        public bool IsScalar
        {
            get
            {
                return DataType >= UFUAModel.DataType.Boolean && DataType <= UFUAModel.DataType.Double;
            }
        }

        [Browsable(false)]
        [NonPersistent]
        public bool IsPrototypeMember
        {
            get 
            {
                return PrototypeReference != null;
            }
        }

        [Browsable(false)]
        [NonPersistent]
        public bool IsSubPrototypeMember
        {
            get
            {
                var prototype = PrototypeReference;
                return prototype != null && prototype.UFUATagOwner != null;
            }
        }

        bool isSubPrototypeMembersCreated;
        [Browsable(false)]
        [NonPersistent]
        public bool IsSubPrototypeMembersCreated
        {
            get
            {
                return isSubPrototypeMembersCreated;
            }
            set
            {
                if (isSubPrototypeMembersCreated == value)
                    return;
                isSubPrototypeMembersCreated = value;
                RaisePropertyChangedEvent("PrototypeModel");
            }
        }

        [Browsable(false)]
        [NonPersistent]
        public bool IsSharedMember
        {
            get
            {
                return IsSubPrototypeMember && UseShared.Value;
            }
        }

        [Browsable(false)]
        [NonPersistent]
        [Exportable]
        public string TagOwnerPath
        {
            get
            {
                string name = string.Empty;
                if (IsSubPrototypeMember)
                {
                    var prototype = PrototypeReference;
                    name = $"{ prototype.TagOwnerPath}";
                }
                return name;
            }
        }

        [Browsable(false)]
        [NonPersistent]
        public bool IsEngineeringUnitSupported
        {
            get
            {
                return DataType != UFUAModel.DataType.Boolean && DataType != UFUAModel.DataType.String &&
                    ModelType == UFUAModel.ModelType.Analog;
            }
        }

        [Browsable(false)]
        [NonPersistent]
        public UFUATagPrototype PrototypeReference
        {
            get
            {
                var prototype = UFUATagPrototype;
                if (UFUAFolder != null)
                {
                    var root = UFUAFolder;
                    while (root.UFUAFolderAss != null)
                        root = root.UFUAFolderAss;

                    prototype = root.UFUATagPrototype;
                }

                return prototype;
            }
        }

        [Browsable(false)]
        [NonPersistent]
        [Exportable]
        public string PrototypeReferenceName
        {
            get
            {
                if(!IsPrototypeMember)
                    return string.Empty;
                else
                {
                    return PrototypeReference.Name;
                }
            }
        }

        //[Browsable(false)]
        [NonPersistent]
        [Exportable]
        public String PrototypeName
        {
            get
            {
                var prototypeModel = PrototypeModel;
                if (!String.IsNullOrEmpty(prototypeModel))
                {
                    Guid guid;
                    if (Guid.TryParse(prototypeModel, out guid))
                    {
                        var list = (from protoype in new XPQuery<UFUAModel.UFUATagPrototype>(Session, true)/*.AsParallel()*/
                                    where protoype.NodeId == guid && protoype.UFUATagOwner == null
                                    select protoype.Name).ToList();
                        if (list.Count > 0)
                            return list[0];

                    }
                    else
                    {
                        var list = (from protoype in new XPQuery<UFUAModel.UFUATagPrototype>(Session, true)/*.AsParallel()*/
                                    where protoype.Name == prototypeModel && protoype.UFUATagOwner == null
                                    select protoype.Name).ToList();
                        if (list.Count > 0)
                            return list[0];
                    }
                }

                return prototypeModel;
            }
            set 
            {
                if (value == PrototypeModel)
                    return;

                if (!String.IsNullOrEmpty(value))
                {
                    var list = (from protoype in new XPQuery<UFUAModel.UFUATagPrototype>(Session, true)/*.AsParallel()*/
                                where protoype.Name == value && protoype.UFUATagOwner == null
                                select protoype.NodeId).ToList();
                    if (list.Count > 0)
                        PrototypeModel = list[0].ToString();
                    else
                        PrototypeModel = null;
                }
                else
                    PrototypeModel = null;

                this.RaisePropertyChangedEvent("PrototypeModel");
                this.RaisePropertyChangedEvent("PrototypeName");
            }
        }

        [Browsable(false)]
        [NonPersistent]
        [Exportable]
        public String FolderPath
        {
            get
            {
                String ret = String.Empty;
                UFUAFolder folder = UFUAFolder;
                while (folder != null)
                {
                    if (String.IsNullOrEmpty(ret))
                        ret = folder.Name;
                    else
                        ret = String.Format("{0}\\{1}", folder.Name, ret);
                    folder = folder.UFUAFolderAss;
                }

                return ret;
            }
        }

        [Browsable(false)]
        [NonPersistent]
        public String OriginalFolderPath
        {
            get
            {
                String ret = String.Empty;
                UFUAFolder folder = UFUAFolder;
                while (folder != null)
                {
                    if (String.IsNullOrEmpty(ret))
                        ret = folder.OriginalName;
                    else
                        ret = String.Format("{0}\\{1}", folder.OriginalName, ret);
                    folder = folder.UFUAFolderAss;
                }

                return ret;
            }
        }

        [Browsable(false)]
        [NonPersistent]
        [Exportable]
        public String EnumStringsFlat
        {
            get
            {
                String ret = null;
                if (EnumStrings.Count > 0)
                {
                    var orderedEnumStrings = (from c in EnumStrings orderby c.Oid select c).ToList();
                    foreach (var value in orderedEnumStrings)
                    { 
                        if (String.IsNullOrEmpty(ret))
                            ret = value.Data;
                        else
                            ret = String.Format("{0}|{1}", ret, value.Data);
                    }
                }
                
                return ret;
            }
            set
            {
                if (EnumStringsFlat != value)
                {

                    while (EnumStrings.Count > 0)
                    {
                        var member = EnumStrings[0];
                        EnumStrings.Remove(member);
                        member.Delete();
                    }

                    if (!string.IsNullOrEmpty(value))
                    {
                        var values = value.Split('|');
                        foreach (var element in values)
                        {
                            EnumStrings.Add(new UFUAEnumString(this.Session) { Data = element });
                        }
                    }
                    this.OnChanged("EnumStringsFlat");
                }
            }
        }

        #endregion

        #region Properties

        private int? _MemberOrderId;
        [Custom("Generate", "Null")]
        [Browsable(false)]
        [Exportable]
        public int? MemberOrderId
        {
            get
            {
                return _MemberOrderId;
            }
            set 
            {
                SetPropertyValue("MemberOrderId", ref _MemberOrderId, value);
            }
        }

        
        private bool? _AlwaysInUse;
        [Exportable]
        public bool? AlwaysInUse
        {
            get
            {
                return _AlwaysInUse;
            }
            set
            {
                SetPropertyValue("AlwaysInUse", ref _AlwaysInUse, value);
            }
        }


        private bool? _UseShared;
        [Exportable]
        public bool? UseShared
        {
            get
            {
                return _UseShared;
            }
            set
            {
                var bChanged = _UseShared != value;
                if (SetPropertyValue("UseShared", ref _UseShared, value))
                {
                    if (!IsLoading && IsSubPrototypeMember)
                    {
                        if (value == false && UFUATagPrototype != null && UFUATagPrototype.UFUATagOwner != null && 
                            UFUATagPrototype.UFUATagOwner.IsSubPrototypeMember)
                            UFUATagPrototype.UFUATagOwner.UseShared = false;
                        else if (value == true)
                        {
                            if (SubPrototypeMembers != null && SubPrototypeMembers.Count > 0)
                            {
                                var members = SubPrototypeMembers[0].GetTagMembers();
                                foreach (var member in members)
                                    member.UseShared = true;
                            }
                        }
                    }

                    OnPropertyReadOnlyChanged("UseShared");
                    RaisePropertyChangedEvent("IsSharedMember");
                }
                if (IsLoading && bChanged)
                    RaisePropertyChangedEvent("UseShared");
            }
        }

        private string _Name;
        //[Indexed(Unique = false)]
        [Exportable]
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
                if (Oid != -1 && String.IsNullOrEmpty(OriginalName) && _Name != value)
                    OriginalName = _Name;
                SetPropertyValue("Name", ref _Name, value);
            }
        }

        private string _OriginalName;
        //[Indexed(Unique = false)]
        [MergablePropertyAttribute(false)]
        [Size(SizeAttribute.Unlimited)]
        [Custom("Generate", "Null")]
        [Browsable(false)]
        public string OriginalName
        {
            get
            {
                return _OriginalName;
            }
            set
            {
                SetPropertyValue("OriginalName", ref _OriginalName, value);
            }
        }

        private string _Description;
        [Size(SizeAttribute.Unlimited)]
        [Exportable]
        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                SetPropertyValue("Description", ref _Description, value);
            }
        }

        private ModelType? _ModelType;
        [Exportable]
        public ModelType? ModelType
        {
            get
            {
                return _ModelType;
            }
            set
            {
                if (SetPropertyValue("ModelType", ref _ModelType, value))
                {
                    if (!IsLoading && SubPrototypeMembers != null)
                    {
                        while (SubPrototypeMembers.Count > 0)
                        {
                            var member = SubPrototypeMembers[0];
                            SubPrototypeMembers.Remove(member);
                            member.Delete();
                        }
                    }

                    this.OnChanged("PrototypeModel");
                    this.OnChanged("PrototypeName");
                    this.OnChanged("EnumStringsFlat");
                    this.RaisePropertyChangedEvent("DynamicSettings");
                    OnPropertyVisiblityChanged("ModelType");
                }
            }
        }

        private DataType? _DataType;
        [Exportable]
        public DataType? DataType
        {
            get
            {
                if (ModelType == UFUAModel.ModelType.Digital)
                    return UFUAModel.DataType.Boolean;
                else if (ModelType == UFUAModel.ModelType.Enumerated)
                    return UFUAModel.DataType.UInt32;
                else if (ModelType == UFUAModel.ModelType.Method || ModelType == UFUAModel.ModelType.ObjectType)
                    return null;
                else
                    return _DataType;
            }
            set
            {
                var bChanged = _DataType != value;
                if (SetPropertyValue("DataType", ref _DataType, value))
                {
                    this.RaisePropertyChangedEvent("DynamicSettings");
                    OnPropertyVisiblityChanged("DataType");
                }

                if (IsLoading && bChanged)
                    this.RaisePropertyChangedEvent("DataType");
            }
        }

        private uint _ArrayDimension;
        [Exportable]
        public uint ArrayDimension
        {
            get
            {
                return _ArrayDimension;
            }
            set
            {
                if (SetPropertyValue("ArrayDimension", ref _ArrayDimension, value))
                {
                    this.RaisePropertyChangedEvent("DynamicSettings");
                    OnPropertyVisiblityChanged("ArrayDimension");
                }
            }
        }

        private bool _ExcludeDynamicSettings;
        [Exportable]
        public bool ExcludeDynamicSettings
        {
            get
            {
                return _ExcludeDynamicSettings;
            }
            set
            {
                if (SetPropertyValue("ExcludeDynamicSettings", ref _ExcludeDynamicSettings, value))
                    OnPropertyReadOnlyChanged("ExcludeDynamicSettings");
            }
        }

        private string _DynamicSettings;
        [Size(SizeAttribute.Unlimited)]
        [Exportable]
        public string DynamicSettings
        {
            get
            {
                return _DynamicSettings;
            }
            set
            {
                SetPropertyValue("DynamicSettings", ref _DynamicSettings, value);
            }
        }

        PhysicalAddresses _DynamicSettingsFlat;
        [NonPersistent]
        public PhysicalAddresses DynamicSettingsFlat
        {
            get
            {
                if (_DynamicSettingsFlat == null)
                {
                    _DynamicSettingsFlat = new PhysicalAddresses();
                    var dictionary = _DynamicSettingsFlat as IDictionary<string, object>;                    
                    var mapDynamic = GetMapDynamicSettings();

                    var drivers = (from p in new XPQuery<UFUAModel.UFUACommunicationDriver>(Session, true).AsParallel()
                                   orderby p.FriendlyName
                                   select p).ToList();
                    foreach (var driver in drivers)
                    {
                        if (mapDynamic.ContainsKey(driver.Name))
                        {
                            if (!dictionary.ContainsKey(driver.FriendlyName))
                                dictionary.Add(driver.FriendlyName, new DriverDynamicSettings(this, driver.Name, mapDynamic[driver.Name]));
                            else
                                dictionary.Add(driver.Name, new DriverDynamicSettings(this, driver.Name, mapDynamic[driver.Name]));
                            mapDynamic.Remove(driver.Name);
                        }
                        else if (!dictionary.ContainsKey(driver.FriendlyName))
                            dictionary.Add(driver.FriendlyName, new DriverDynamicSettings(this, driver.Name));
                        else
                            dictionary.Add(driver.Name, new DriverDynamicSettings(this, driver.Name));
                    }

                    foreach (var driverName in mapDynamic.Keys)
                    {
                        var baseName = mapDynamic[driverName];
                        if (baseName.Contains('.'))
                            baseName = baseName.Substring(0, baseName.IndexOf('.'));
                        dictionary.Add(driverName, new DriverDynamicSettings(this, baseName, mapDynamic[driverName]));
                    }
                }

                return _DynamicSettingsFlat;
            }
            set
            {
                DynamicSettings = null;
                _DynamicSettingsFlat = null;
            }
        }

        private bool _IsRetentive;
        [Exportable]
        public bool IsRetentive
        {
            get
            {
                return _IsRetentive;
            }
            set
            {
                SetPropertyValue("IsRetentive", ref _IsRetentive, value);
            }
        }

        private bool _EnableStatistics;
        [Exportable]
        public bool EnableStatistics
        {
            get
            {
                return _EnableStatistics;
            }
            set
            {
                SetPropertyValue("EnableStatistics", ref _EnableStatistics, value);
            }
        }

        private bool? _IsRedundancyEnabled;
        [Exportable]
        public bool? IsRedundancyEnabled
        {
            get
            {
                return _IsRedundancyEnabled;
            }
            set
            {
                SetPropertyValue("IsRedundancyEnabled", ref _IsRedundancyEnabled, value);
            }
        }

        private bool _UseSeparateThreadScriptExecuter;
        [Exportable]
        public bool UseSeparateThreadScriptExecuter
        {
            get
            {
                return _UseSeparateThreadScriptExecuter;
            }
            set
            {
                SetPropertyValue("UseSeparateThreadScriptExecuter", ref _UseSeparateThreadScriptExecuter, value);
            }
        }

        #region Audit
        private bool _AuditTraceEnabled;
        [Category("Audit")]
        [Exportable]
        public bool AuditTraceEnabled
        {
            get
            {
                return _AuditTraceEnabled;
            }
            set
            {
                if (SetPropertyValue("AuditTraceEnabled", ref _AuditTraceEnabled, value))
                {
                    OnPropertyVisiblityChanged("AuditTraceEnabled");
                }
            }
        }

        private bool _EnterCommentOnAudit;
        [Category("Audit")]
        [Exportable]
        public bool EnterCommentOnAudit
        {
            get
            {
                return _EnterCommentOnAudit;
            }
            set
            {
                if (SetPropertyValue("EnterCommentOnAudit", ref _EnterCommentOnAudit, value))
                {
                    OnPropertyVisiblityChanged("EnterCommentOnAudit");
                }
            }
        }

        private bool _EnterPasswordOnAudit;
        [Category("Audit")]
        [Exportable]
        public bool EnterPasswordOnAudit
        {
            get
            {
                return _EnterPasswordOnAudit;
            }
            set
            {
                SetPropertyValue("EnterPasswordOnAudit", ref _EnterPasswordOnAudit, value);
            }
        }

        private int _MinAccessLevelRequiredOnAudit;
        [Category("Audit")]
        [Exportable]
        public int MinAccessLevelRequiredOnAudit
        {
            get
            {
                return _MinAccessLevelRequiredOnAudit;
            }
            set
            {
                SetPropertyValue("MinAccessLevelRequiredOnAudit", ref _MinAccessLevelRequiredOnAudit, value);
            }
        }

        private TimeSpan? _MaxAuditAge;
        [Category("Audit")]
        [Exportable]
        public TimeSpan? MaxAuditAge
        {
            get
            {
                return _MaxAuditAge;
            }
            set
            {
                SetPropertyValue("MaxAuditAge", ref _MaxAuditAge, value);
            }
        }
        #endregion

        private string _InitialValue;
        [Size(SizeAttribute.Unlimited)]
        [Exportable]
        public string InitialValue
        {
            get
            {
                return _InitialValue;
            }
            set
            {
                SetPropertyValue("InitialValue", ref _InitialValue, value);
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

        private AccessLevels? _AccessLevel;
        [Exportable]
        public AccessLevels? AccessLevel
        {
            get
            {
                return _AccessLevel;
            }
            set
            {
                SetPropertyValue("AccessLevel", ref _AccessLevel, value);
            }
        }

        private SamplingIntervals? _SamplingInterval;
        [Exportable]
        public SamplingIntervals? SamplingInterval
        {
            get
            {
                return _SamplingInterval;
            }
            set
            {
                SetPropertyValue("SamplingInterval", ref _SamplingInterval, value);
            }
        }

        private DateTime _SettingsTimeStamp;
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

        private Guid _NodeId;
        [Custom("Generate", "Guid")]
        [Exportable]
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

        private String _NodeIdToPublish;
        [Custom("Generate", "Null")]
        [ReadOnly(true)]
        public String NodeIdToPublish
        {
            get
            {
                return _NodeIdToPublish;
            }
            set
            {
                SetPropertyValue("NodeIdToPublish", ref _NodeIdToPublish, value);
            }
        }

        private String _PrototypeModel;
        [Browsable(false)]
        public String PrototypeModel
        {
            get
            {
                if (ModelType != UFUAModel.ModelType.ObjectType)
                    return String.Empty;

                return _PrototypeModel;
            }
            set
            {
                var bChanged = (_PrototypeModel ?? String.Empty) != (value ?? String.Empty);
                if (SetPropertyValue("PrototypeModel", ref _PrototypeModel, value))
                {
                    if (!IsLoading && SubPrototypeMembers != null)
                    {
                        while (SubPrototypeMembers.Count > 0)
                        {
                            var member = SubPrototypeMembers[0];
                            SubPrototypeMembers.Remove(member);
                            member.Delete();
                        }
                    }
                }
                if (IsLoading && bChanged)
                    RaisePropertyChangedEvent("PrototypeModel");
            }
        }

        Helpers.CustomXPCollection<UFUAEnumString> _EnumStrings;
        [Association("UFUATag-EnumStrings"), Aggregated]
        public Helpers.CustomXPCollection<UFUAEnumString> EnumStrings
        {
            get
            {
                if (_EnumStrings == null)
                {
                    var member = base.ClassInfo.GetMember("EnumStrings");
                    _EnumStrings = new Helpers.CustomXPCollection<UFUAEnumString>(base.Session, this, member);
                }

                return _EnumStrings;
            }
            set
            {
                if (_EnumStrings != null)
                {
                    while (_EnumStrings.Count > 0)
                    {
                        var member = _EnumStrings[0];
                        _EnumStrings.Remove(member);
                        member.Delete();
                    }

                    if (value != null && value.Count > 0)
                    {
                        foreach (var item in value)
                            _EnumStrings.Add(item);
                    }
                }
            }
        }

        private UFUAFolder _UFUAFolder;
        [Association("UFUAFolder-UFUATags")]
        [Browsable(false)]
        public UFUAFolder UFUAFolder
        {
            get
            {
                return _UFUAFolder;
            }
            set
            {
                SetPropertyValue("UFUAFolder", ref _UFUAFolder, value);
            }
        }

        private UFUATagPrototype _UFUATagPrototype;
        [Association("UFUATagPrototype-Members")]
        public UFUATagPrototype UFUATagPrototype
        {
            get
            {
                return _UFUATagPrototype;
            }
            set
            {
                SetPropertyValue("UFUATagPrototype", ref _UFUATagPrototype, value);
            }
        }

        private String _UFUAEngineeringUnit;
        [Size(SizeAttribute.Unlimited)]
        [Exportable]
        public String UFUAEngineeringUnit
        {
            get
            {
                if (!IsEngineeringUnitSupported)
                    return String.Empty;

                return _UFUAEngineeringUnit;
            }
            set
            {
                bool bChanged = (_UFUAEngineeringUnit ?? String.Empty) != (value ?? String.Empty);
                SetPropertyValue("UFUAEngineeringUnit", ref _UFUAEngineeringUnit, value);
                if (IsLoading && bChanged)
                    RaisePropertyChangedEvent("UFUAEngineeringUnit");
            }
        }

        private String _HistorianSettings;
        [Size(SizeAttribute.Unlimited)]
        [Exportable(ExportPropertyName = "AssignedHistorian")]
        public String HistorianSettings
        {
            get
            {
                return _HistorianSettings;
            }
            set
            {
                bool bChanged = (_HistorianSettings ?? String.Empty) != (value ?? String.Empty);
                SetPropertyValue("HistorianSettings", ref _HistorianSettings, value);
                if (IsLoading && bChanged)
                    RaisePropertyChangedEvent("HistorianSettings");
            }
        }

        //[Association("UFUATag-UFUAAlarmDefinition")]
        //public XPCollection<UFUAAlarmDefinition> UFUAAlarmDefinition
        //{
        //    get
        //    {
        //        return GetCollection<UFUAAlarmDefinition>("UFUAAlarmDefinition");
        //    }
        //}

        [Association("UFUATag-UFUAAlarmThreshold"), Aggregated]
        public XPCollection<UFUAAlarmThreshold> UFUAAlarmThresholds
        {
            get
            {
                return GetCollection<UFUAAlarmThreshold>("UFUAAlarmThresholds");
            }
        }

        [MergablePropertyAttribute(false)]
        [Browsable(false)]
        [Custom("DisableCheckCustomAttributes", "")]
        [Association("UFUATag-SubPrototypeMembers"), Aggregated]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public XPCollection<UFUATagPrototype> SubPrototypeMembers
        {
            get
            {
                return GetCollection<UFUATagPrototype>("SubPrototypeMembers");
            }
        }

        [Browsable(false)]
        [NonPersistent]
        public String Alarms
        {
            get
            {
                return ToString(UFUAAlarmThresholds);
            }
        }

        [Browsable(false)]
        [NonPersistent]
        [Exportable]
        public String AlarmList
        {
            get
            {
                String alarms = String.Empty;
                var list = UFUAAlarmThresholds?.ToList();
                if (list != null)
                    list.ForEach(alarm =>
                    {
                        string alarmOptions = alarm.AlarmOptions;
                        if (alarm.UFUAAlarmDefinitionRef != null)
                        {
                            string text = string.Format("{0}/{1}#{2}@{3}", alarm.UFUAAlarmDefinitionRef.SourcePath, alarm.UFUAAlarmDefinitionRef.Name, alarm.AlarmText, alarmOptions);
                            if (String.IsNullOrEmpty(alarms))
                                alarms = string.Format("{0}", text);
                            else
                                alarms = String.Format("{0}|{1}", alarms, text);
                        }
                        else
                            alarms = String.Format("{0}|{1}@{2}", alarms, alarm.CompleteName, alarmOptions);
                    });
                return alarms;
            }
        }

        [Association("UFUATag-UFUAView")]
        public XPCollection<UFUAView> UFUAViews
        {
            get
            {
                return GetCollection<UFUAView>("UFUAViews");
            }
        }

        [Browsable(false)]
        [NonPersistent]
        [Exportable]
        public String Views
        {
            get
            {
                return ToString(UFUAViews);
            }
            set { }
        }

        private DateTime _CreateDate = DateTime.UtcNow;
        [Custom("Generate", "DateTime")]
        [ReadOnly(true)]
        public DateTime CreateDate
        {
            get
            {
                return _CreateDate;
            }
            set
            {
                if (value != DateTime.MinValue)
                    SetPropertyValue("CreateDate", ref _CreateDate, value);
            }
        }

        private String _ListProcedures;
        [Size(SizeAttribute.Unlimited)]
        [Exportable]
        public String ListProcedures
        {
            get
            {
                return _ListProcedures;
            }
            set
            {
                SetPropertyValue("ListProcedures", ref _ListProcedures, value);
            }
        }

        private String _ScriptCode;
        [Size(SizeAttribute.Unlimited)]
        [Exportable]
        public String ScriptCode
        {
            get
            {
                if (IsSaving && _ScriptCode != null)
                {
                    return _ScriptCode.Replace("\r", "\n");
                }
                else
                    return _ScriptCode;
            }
            set
            {
                if (IsLoading && value != null)
                {
                    var code = value.Replace("\n\n", Environment.NewLine);
                    SetPropertyValue("ScriptCode", ref _ScriptCode, code);
                }
                else
                    SetPropertyValue("ScriptCode", ref _ScriptCode, value);
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

            if (_EnumStrings != null)
                _EnumStrings.Dispose();
        }

        #endregion

        #region Methods
        public string GetRelativePath(UInt16 ns, bool bRelative = false)
        {
            if (bRelative)
            {
                string path = string.Format("{0}:{1}", ns, Name);
                if (UFUAFolder != null)
                {
                    path = string.Format("{0}/{2}:{1}", UFUAFolder.GetRelativePath(ns, bRelative), Name, ns);
                }
                return path;
            }
            else
            {
                string path = string.Format("{0}:{1}/{0}:{2}", ns, UFUAServerInfo.UFUAServerInfo.GetTagRootName(), Name);
                if (UFUAFolder != null)
                {
                    path = string.Format("{0}/{2}:{1}", UFUAFolder.GetRelativePath(ns, bRelative), Name, ns);
                }
                return path;
            }
        }

        public string GetFullName()
        {
            string name = string.Format("{0}", Name);
            if (UFUAFolder != null)
            {
                name = string.Format("{0}/{1}", UFUAFolder.GetFullName(), Name);
            }
            else if (IsSubPrototypeMember)
            {
                name = string.Format("{0}/{1}", PrototypeReference.UFUATagOwner.GetFullName(), Name);
            }
            return name;
        }

        public string GetRelativeName(bool bUseSubPrototypeMember = false)
        {
            string name = string.Format("{0}", Name);
            if (UFUAFolder != null && (!bUseSubPrototypeMember || UFUAFolder.IsSubPrototypeMember))
            {
                name = string.Format("{0}/{1}", UFUAFolder.GetRelativeName(bUseSubPrototypeMember), Name);
            }
            else if (bUseSubPrototypeMember && IsSubPrototypeMember)
            {
                string nodeId = string.Format("{0}", NodeId);
                if (ModelType == UFUAModel.ModelType.ObjectType)
                    nodeId = Name;
                name = string.Format("{0}/{1}", PrototypeReference.UFUATagOwner.GetRelativeName(bUseSubPrototypeMember), nodeId);
            }
            return name;
        }

        public string GetRelativeNodeId(bool bUseSubPrototypeMember = false)
        {
            string nodeId = string.Format("{0}", NodeId);
            if (ModelType == UFUAModel.ModelType.ObjectType)
                nodeId = Name;
            if (UFUAFolder != null && (!bUseSubPrototypeMember || UFUAFolder.IsSubPrototypeMember))
            {
                nodeId = string.Format("{0}/{1}", UFUAFolder.GetRelativeName(bUseSubPrototypeMember), nodeId);
            }
            else if (bUseSubPrototypeMember && IsSubPrototypeMember)
            {
                nodeId = string.Format("{0}/{1}", PrototypeReference.UFUATagOwner.GetRelativeName(bUseSubPrototypeMember), nodeId);
            }
            return nodeId;
        }

        public NodeId GetResolvedNodeId(UInt16 ns)
        {
            var instanceTag = PrototypeReference.UFUATagOwner;
            while (instanceTag.IsSubPrototypeMember)
                instanceTag = instanceTag.PrototypeReference.UFUATagOwner;

            var relativeName = GetRelativeNodeId(bUseSubPrototypeMember: true);
            relativeName = String.Format("{0}?{1}",
                instanceTag.NodeId, relativeName.Remove(0, instanceTag.Name.Length + 1));
            return new NodeId(relativeName, ns);
        }

        public byte GetBitsNumber()
        {
            if (!DataType.HasValue)
                return 0;

            if (DataType.Value == UFUAModel.DataType.Boolean)
                return 1;
            else if (DataType.Value == UFUAModel.DataType.Byte ||
                DataType.Value == UFUAModel.DataType.SByte)
                return 8;
            else if (DataType.Value == UFUAModel.DataType.UInt16 ||
                DataType.Value == UFUAModel.DataType.Int16)
                return 16;
            else if (DataType.Value == UFUAModel.DataType.UInt32 ||
                DataType.Value == UFUAModel.DataType.Int32 ||
                DataType.Value == UFUAModel.DataType.Float)
                return 32;
            else if (DataType.Value == UFUAModel.DataType.UInt64 ||
                DataType.Value == UFUAModel.DataType.Int64 ||
                DataType.Value == UFUAModel.DataType.Double)
                return 64;
            else
                return 0;
        }

        public List<string> GetDynamicSettings()
        {
            if (!String.IsNullOrEmpty(DynamicSettings))
                return DynamicSettings.Split(UFUAServerInfo.UFUAServerInfo.GetDynSettingsSeparator()).ToList();

            return null;
        }

        Dictionary<string, string> GetMapDynamicSettings()
        {
            // force case insensitive key search for back compatibility with Movicon's project contain Driver's Name different from Assembly Name
            var ret = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            if (!String.IsNullOrEmpty(DynamicSettings))
            {
                var dynamics = DynamicSettings.Split(UFUAServerInfo.UFUAServerInfo.GetDynSettingsSeparator()).ToList();
                foreach (var dyn in dynamics)
                {
                    var driverName = dyn;
                    if (driverName.Contains('.'))
                        driverName = driverName.Substring(0, driverName.IndexOf('.'));

                    if (!String.IsNullOrEmpty(driverName))
                    {
                        int index = 1;
                        var baseName = driverName;
                        while (ret.ContainsKey(driverName))
                            driverName = string.Format("{0}({1})", baseName, ++index);
                        ret.Add(driverName, dyn);
                    }
                }
            }

            return ret;
        }

        public bool IsAlarmDefinitionAssigned(UFUAAlarmDefinition alarm)
        {
            return (from c in UFUAAlarmThresholds where c.UFUAAlarmDefinitionRef == alarm select c).ToList().Count > 0;
        }

        public bool RemoveAlarmDefinition(UFUAAlarmDefinition alarm)
        {
            var list = (from c in UFUAAlarmThresholds where c.UFUAAlarmDefinitionRef == alarm select c).ToList();
            list.ForEach((o) => { UFUAAlarmThresholds.Remove(o); });
            return list.Count > 0;
        }

        public bool IsMemberOf(UFUAModel.UFUATagPrototype prototype)
        {
            var parent = UFUATagPrototype;
            if (UFUAFolder != null)
            {
                var root = UFUAFolder;
                while (root.UFUAFolderAss != null)
                    root = root.UFUAFolderAss;

                parent = root.UFUATagPrototype;
            }

            return parent == prototype;
        }

        public bool IsMemberOf(UFUAModel.UFUAFolder folder)
        {
            UFUAFolder parent = UFUAFolder;
            while (parent != null)
            {
                if (parent == folder)
                    return true;
                parent = parent.UFUAFolderAss;
            }

            return false;
        }

        public void NotifyPropertyChanged(string property)
        {
            this.RaisePropertyChangedEvent(property);
        }

        #endregion

        #region Static Methods
        public static string ToString(DevExpress.Xpo.XPCollection<UFUAModel.UFUAView> views, char separator = ',')
        {
            StringBuilder ret = new StringBuilder();
            if (views != null)
            {
                var list = (from c in views orderby c.Name ascending select c).ToList();
                foreach (var view in list)
                {
                    if (ret.Length == 0)
                        ret.Append(view.Name);
                    else
                        ret.AppendFormat("{0} {1}", separator, view.Name);
                }
            }
            return ret.ToString();
        }

        public static string ToString(DevExpress.Xpo.XPCollection<UFUAModel.UFUAAlarmThreshold> alarms, char separator = ',')
        {
            StringBuilder ret = new StringBuilder();
            if (alarms != null)
            {
                var list = (from c in alarms orderby c.Name ascending select c).ToList();
                foreach (var alarm in list)
                {
                    if (ret.Length == 0)
                        ret.Append(alarm.Name);
                    else
                        ret.AppendFormat("{0} {1}", separator, alarm.Name);
                }
            }
            return ret.ToString();
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
#if !NET_STANDARD
                if (propertyName == "DataType")
                {
                    return (ModelType == UFUAModel.ModelType.Variable || ModelType == UFUAModel.ModelType.Analog);
                }
                else if (propertyName == "ArrayDimension")
                {
                    return ModelType != UFUAModel.ModelType.Method && ModelType != UFUAModel.ModelType.ObjectType;
                }
                else if (propertyName == "DynamicSettings")
                {
                    if (IsMultipleDriverInstalled() ||
                        (DynamicSettings != null && DynamicSettings.Contains(UFUAServerInfo.UFUAServerInfo.GetDynSettingsSeparator())))
                        return false;
                    else
                        return !IsPrototypeMember || IsSubPrototypeMember || ModelType == UFUAModel.ModelType.Method;
                }
                else if (propertyName == "DynamicSettingsFlat")
                {
                    if (!IsMultipleDriverInstalled() &&
                        (DynamicSettings == null || !DynamicSettings.Contains(UFUAServerInfo.UFUAServerInfo.GetDynSettingsSeparator())))
                        return false;
                    else
                        return !IsPrototypeMember || IsSubPrototypeMember || ModelType == UFUAModel.ModelType.Method;
                }
                else if (propertyName == "IsRetentive")
                {
                    return !IsPrototypeMember && ModelType != UFUAModel.ModelType.Method;
                }
                else if (propertyName == "EnableStatistics")
                {
                    return !IsPrototypeMember && ModelType != UFUAModel.ModelType.Method && IsScalar && ArrayDimension == 0;
                }
                else if (propertyName == "InitialValue")
                {
                    return (!IsPrototypeMember || IsSubPrototypeMember) && ModelType != UFUAModel.ModelType.Method && ModelType != UFUAModel.ModelType.ObjectType;
                }
                else if (propertyName == "PrototypeName")
                {
                    return ModelType == UFUAModel.ModelType.ObjectType;
                }
                else if (propertyName == "EnumStrings")
                {
                    return ModelType == UFUAModel.ModelType.Digital || ModelType == UFUAModel.ModelType.Enumerated;
                }
                else if (propertyName == "UFUAEngineeringUnit")
                {
                    return ModelType == UFUAModel.ModelType.Analog && DataType != UFUAModel.DataType.Boolean && DataType != UFUAModel.DataType.String;
                }
                else if (propertyName == "HistorianSettings")
                {
                    return ModelType != UFUAModel.ModelType.Method && ModelType != UFUAModel.ModelType.ObjectType;
                }
                else if (propertyName == "UFUAAlarmThresholds")
                {
                    return ModelType != UFUAModel.ModelType.Method && ModelType != UFUAModel.ModelType.ObjectType;
                }
                else if (propertyName == "UFUAViews")
                {
                    return !IsPrototypeMember;
                }
                else if (propertyName == "UseShared")
                {
                    return PrototypeReference != null && PrototypeReference.UFUATagOwner != null;
                }
                else if (propertyName == "IsRedundancyEnabled")
                {
                    return ModelType != UFUAModel.ModelType.Method && ModelType != UFUAModel.ModelType.ObjectType;
                }
                else if (propertyName == "AuditTraceEnabled")
                {
                    return ModelType != UFUAModel.ModelType.Method && ModelType != UFUAModel.ModelType.ObjectType;
                }
                else if (propertyName == "EnterCommentOnAudit" ||
                    propertyName == "MaxAuditAge")
                {
                    return AuditTraceEnabled && ModelType != UFUAModel.ModelType.Method && ModelType != UFUAModel.ModelType.ObjectType;
                }
                else if (propertyName == "EnterPasswordOnAudit" ||
                    propertyName == "MinAccessLevelRequiredOnAudit")
                {
                    return AuditTraceEnabled && EnterCommentOnAudit && ModelType != UFUAModel.ModelType.Method && ModelType != UFUAModel.ModelType.ObjectType;
                }
#endif
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
                if (propertyName != "UseShared" && UseShared.Value &&
                    PrototypeReference != null && PrototypeReference.UFUATagOwner != null)
                    return true;

                if (propertyName == "Name" || propertyName == "ModelType" || 
                    propertyName == "DataType" || propertyName == "ArrayDimension" || 
                    propertyName == "IsRetentive" || propertyName == "PrototypeName" ||
                    propertyName == "EnableStatistics" || propertyName == "UFUAViews")
                {
                    return IsSubPrototypeMember;
                }

                if (propertyName == "DynamicSettings" || propertyName == "DynamicSettingsFlat")
                    return ExcludeDynamicSettings;

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

        #region Private Methods
        protected String PerformValidation(String propertyName, String prototypeName = null)
        {
#if !NET_STANDARD
            if (propertyName == "Name")
            {
                if (!Helpers.NameValidator.IsValidName(Name))
                {
                    return Properties.Resources.TagNameInvalid;
                }
                else if (UFUAFolder != null)
                {
                    if ((from c in UFUAFolder.UFUATags/*.AsParallel()*/
                        where c != this && c.Name == Name
                        select c).ToList().Count > 0)
                        return Properties.Resources.TagNameAlreadyExists;
                }
                else if (UFUATagPrototype != null)
                {
                    if ((from c in UFUATagPrototype.Members/*.AsParallel()*/
                        where c != this && c.Name == Name
                        select c).ToList().Count > 0)
                        return Properties.Resources.TagNameAlreadyExists;
                }
                else
                {
                    var list = (from tag in new XPQuery<UFUAModel.UFUATag>(Session, true)/*.AsParallel()*/
                                where tag != this && tag.UFUAFolder == this.UFUAFolder &&
                                tag.UFUATagPrototype == this.UFUATagPrototype &&
                                tag.Name == Name
                                select tag).ToList();
                    if (list.Count >= 1)
                        return Properties.Resources.TagNameAlreadyExists;
                }
            }
            else if (propertyName == "PrototypeName")
            {
                if (ModelType == UFUAModel.ModelType.ObjectType)
                {
                    prototypeName = prototypeName ?? PrototypeName;
                    if (String.IsNullOrEmpty(prototypeName))
                        return Properties.Resources.PrototypeDefinitionIsEmpty;

                    var listprototypes = (from protoype in new XPQuery<UFUAModel.UFUATagPrototype>(Session, true)/*.AsParallel()*/
                                          where protoype.Name == prototypeName && protoype.UFUATagOwner == null
                                          select protoype).ToList();
                    if (listprototypes.Count == 0)
                        return Properties.Resources.PrototypeDefinitionNotFound;

                    if (PrototypeReference != null && PrototypeReference.Name == prototypeName)
                        return Properties.Resources.PrototypeDefinitionIsRecursive;
                    else if (PrototypeReference != null)
                    {
                        var listtagstocheck = (from c in new XPQuery<UFUAModel.UFUATag>(Session, true)/*.AsParallel()*/
                                               where ((c.UFUATagPrototype != null && c.UFUATagPrototype != PrototypeReference) ||
                                               (c.UFUAFolder != null && c.UFUAFolder.UFUATagPrototype != null && c.UFUAFolder.UFUATagPrototype != PrototypeReference) ||
                                               (c.UFUAFolder != null && c.UFUAFolder.UFUAFolderAss != null)) &&
                                               c != this && c.ModelType == UFUAModel.ModelType.ObjectType
                                               select c).ToList();

                        var listPrototypeName = new List<String>();
                        foreach (var tag in listtagstocheck/*.AsParallel()*/)
                        {
                            if (tag.PrototypeReference == null)
                                continue;

                            // here we konw that tag is a member of a different prototype
                            if (tag.PrototypeReference.Name == prototypeName &&
                                tag.PrototypeName == PrototypeReference.Name)
                                return Properties.Resources.PrototypeDefinitionIsRecursive;
                            else if (tag.PrototypeReference.Name == prototypeName)
                                listPrototypeName.Add(tag.PrototypeName);
                        }

                        if (listPrototypeName.Contains(PrototypeName))
                            return Properties.Resources.PrototypeDefinitionIsRecursive;

                        foreach (var name in listPrototypeName/*.AsParallel()*/)
                        {
                            String s = PerformValidation(propertyName, name);
                            if (!String.IsNullOrEmpty(s))
                                return s;
                        }
                    }
                }
            }
            else if (propertyName == "UFUAEngineeringUnit")
            {
                if (!String.IsNullOrEmpty(UFUAEngineeringUnit))
                {
                    var listengunits = (from engunit in new XPQuery<UFUAModel.UFUAEngineeringUnit>(Session, true)/*.AsParallel()*/
                                        where engunit.Name == UFUAEngineeringUnit
                                        select engunit).ToList();
                    if (listengunits.Count == 0)
                        return Properties.Resources.EngineeringUnitNotFound;
                }
            }
            else if (propertyName == "InitialValue")
            {
                if (!String.IsNullOrEmpty(InitialValue))
                {
                    if (ArrayDimension > 0)
                    {
                        if (InitialValue[0] != '{' || InitialValue[InitialValue.Length - 1] != '}')
                            return Properties.Resources.TagInitialValueInvalidSyntaxForArray;
                        else if (ArrayDimension != InitialValue.Substring(1, InitialValue.Length - 2).Split('|').Length)
                            return Properties.Resources.TagInitialValueInvalidNumberOfElements;
                    }
                }
            }
            else if (propertyName == "EnumStringsFlat")
            {
                if (ModelType == UFUAModel.ModelType.Digital)
                {
                    if (String.IsNullOrEmpty(EnumStringsFlat))
                        return Properties.Resources.EnumeratedStringsMissingTwoValue;

                    var values = EnumStringsFlat.Split('|');
                    var empty = (from s in values where String.IsNullOrEmpty(s) select s).ToList().Count;
                    if ((values.Length - empty) != 2)
                        return Properties.Resources.EnumeratedStringsMissingTwoValue;
                }
                else if (ModelType == UFUAModel.ModelType.Enumerated)
                {
                    if (String.IsNullOrEmpty(EnumStringsFlat))
                        return Properties.Resources.EnumeratedStringsMissingAnyValue;

                    var values = EnumStringsFlat.Split('|');
                    var empty = (from s in values where String.IsNullOrEmpty(s) select s).ToList().Count;
                    if ((values.Length - empty) == 0)
                        return Properties.Resources.EnumeratedStringsMissingAnyValue;
                }
            }
            else if (propertyName == "DynamicSettings")
            {
                String error = DynSettingsValidation();
                if (!String.IsNullOrEmpty(error))
                    return error;
            }
            else if (propertyName == "MemberOrderId")
            {
                if (MemberOrderId < 0)
                    return Properties.Resources.MemberMinPositionError;
                else
                {
                    if (UFUAFolder != null)
                    {
                        if (MemberOrderId > UFUAFolder.UFUATags.Count - 1)
                            return Properties.Resources.MemberMaxPositionError;
                    }
                    else if (MemberOrderId > PrototypeReference.Members.Count - 1)
                        return Properties.Resources.MemberMaxPositionError;
                }
            }
#endif
            return null;
        }

#if !NET_STANDARD
        protected String DynSettingsValidation()
        {
            var dynamicSettings = GetDynamicSettings();
            if (dynamicSettings != null && dynamicSettings.Count > 0)
            {
                var listConf = (from tag in new XPQuery<UFUAModel.UFUAConfiguration>(Session, true)/*.AsParallel()*/ select tag).ToList();
                if (listConf.Count == 0)
                    return null;

                foreach (var dynamic in dynamicSettings)
                {
                    var error = DynSettingsValidation(dynamic, listConf[0]);
                    if (!String.IsNullOrEmpty(error))
                        return error;
                }
            }
            return null;
        }

        internal String DynSettingsValidation(string dynamic, UFUAModel.UFUAConfiguration configuration = null)
        {
            if (configuration == null)
                configuration = (from tag in new XPQuery<UFUAModel.UFUAConfiguration>(Session, true)/*.AsParallel()*/ select tag).FirstOrDefault();
            if (configuration == null)
                return null;

            var driverName = dynamic;
            if (driverName.Contains('.'))
                driverName = driverName.Substring(0, driverName.IndexOf('.'));

            string assemblyName;
            try
            {
                assemblyName = (from c in configuration.ComunicationDrivers
                                where c.Name.ToLower() == driverName.ToLower()
                                select c.AssemblyName).Single();
            }
            catch (Exception ex)
            {
                // Driver not installed in the project.
                return String.Format(Properties.Resources.InvalidDynamcSettingsDriverNoLoaded, driverName);
            }

            string rootPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            var uidll = UFUAServerInfo.UFUAServerInfo.GetDriversUIName(String.Format("{0}\\Drivers\\{1}", rootPath, assemblyName));
            try
            {
                string error = null;

                var types = Assembly.LoadFile(uidll).GetTypes();
                var list2 = (from t in types/*.AsParallel()*/
                            where !t.IsAbstract && typeof(ICommunicationDriverWpfEditing2).IsAssignableFrom(t)
                            select (ICommunicationDriverWpfEditing2)Activator.CreateInstance(t)).ToList();
                if (list2.Count > 0)
                {
                    error = list2[0].CheckDynamicAddress(dynamic, this);
                    if (!string.IsNullOrEmpty(error))
                        return error;
                }
                else
                {
                    var list = (from t in types/*.AsParallel()*/
                                 where !t.IsAbstract && typeof(ICommunicationDriverWpfEditing).IsAssignableFrom(t)
                                 select (ICommunicationDriverWpfEditing)Activator.CreateInstance(t)).ToList();
                    if (list.Count > 0)
                    {
                        if (ModelType == UFUAModel.ModelType.ObjectType)
                            error = list[0].CheckDynamicAddress(dynamic, unchecked((uint)(-1)), 0);
                        else if (ModelType == UFUAModel.ModelType.Method)
                            error = list[0].CheckDynamicAddress(dynamic, unchecked((uint)(-2)), 0);
                        else
                            error = list[0].CheckDynamicAddress(dynamic, DataType.HasValue ? (uint)DataType.Value : (uint)UFUAModel.DataType.Byte, ArrayDimension);
                        if (!string.IsNullOrEmpty(error))
                            return error;
                    } 
                    else
                    {
                        return String.Format(Properties.Resources.InvalidDynamcSettingsDriverNoLoaded, driverName);
                    }
                }
            }
            catch (Exception e)
            {
                return String.Format(Properties.Resources.InvalidDynamcSettingsDriverNoLoaded, driverName);
            }

            return null;
        }

        bool IsMultipleDriverInstalled()
        {
            var configuration = (from tag in new XPQuery<UFUAModel.UFUAConfiguration>(Session, true)/*.AsParallel()*/ select tag).FirstOrDefault();
            return configuration != null && configuration.ComunicationDrivers.Count > 1;
        }
#endif
#endregion

        #region IDynamicSettingsEditing Members

        [Browsable(false)]
        [NonPersistent]
        public bool IsMethod
        {
            get
            {
                return ModelType == UFUAModel.ModelType.Method;
            }
        }

        [Browsable(false)]
        [NonPersistent]
        public bool IsObjectType
        {
            get
            {
                return ModelType == UFUAModel.ModelType.ObjectType;
            }
        }

        [Browsable(false)]
        [NonPersistent]
        public string DynamicSettingsForEditing
        {
            get
            {
                return DynamicSettings;
            }
            set
            {
                if (DynamicSettings != value)
                    DynamicSettings = value;
            }
        }

        [Browsable(false)]
        [NonPersistent]
        int IDynamicSettingsEditing.DataType
        {
            get
            {
                if (!DataType.HasValue)
                    return -1;

                return (int)DataType;
            }
            set
            {
                if (DataType != (UFUAModel.DataType)value)
                    DataType = (UFUAModel.DataType)value;
            }
        }

        // FOGBUGZ 9836
        [Browsable(false)]
        [NonPersistent]
        uint IDynamicSettingsEditing.ArrayDimension
        {
            get
            {
                return ArrayDimension;
            }
            set
            {
                if (ArrayDimension != (uint)value)
                    ArrayDimension = (uint)value;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IList<IDynamicSettingsEditing> Members
        {
            get
            {
                var prototypeName = PrototypeName;
                if (!String.IsNullOrEmpty(prototypeName))
                {
                    var prototype = (from item in new XPQuery<UFUAModel.UFUATagPrototype>(Session, true)
                                     where item.Name == prototypeName && item.UFUATagOwner == null
                                     select item).FirstOrDefault();
                    if (prototype != null)
                        return prototype.GetTagMembers(bSorted: true).ToList<IDynamicSettingsEditing>();
                }

                return null;
            }
        }

        string IDynamicSettingsEditing.FolderPath
        {
            get
            {
                return FolderPath;
            }
        }

        string IDynamicSettingsEditing.TagOwnerPath
        {
            get
            {
                return TagOwnerPath;
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
                if (UFUAFolder != null)
                    return String.Format("{0}\\{1}", UFUAFolder.PathIdentifier, Name);
                else if (UFUATagPrototype != null)
                    return String.Format("{0}\\{1}", UFUATagPrototype.PathIdentifier, Name);
                else
                    return Name;
            }
        }

#if !NET_STANDARD
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
                if (UFUAFolder != null)
                    return UFUAFolder.UniqueIdentifier;
                return String.Empty;
            }
        }

        [Browsable(false)]
        [NonPersistent]
        public String OwnerIdentifier
        {
            get
            {
                var prototypeReference = PrototypeReference;
                if (prototypeReference != null)
                {
                    if (prototypeReference != null && prototypeReference.UFUATagOwner != null)
                        return String.Format("{0}|{1}", prototypeReference.UniqueIdentifier, prototypeReference.UFUATagOwner.UniqueIdentifier);
                    else
                        return prototypeReference.UniqueIdentifier;
                }
                return String.Empty;
            }
        }

        [Browsable(false)]
        [NonPersistent]
        public String ParentOwnerIdentifier
        {
            get
            {
                var prototypeReference = PrototypeReference;
                if (prototypeReference != null && prototypeReference.UFUATagOwner != null)
                    return prototypeReference.UFUATagOwner.UniqueIdentifier;
                return String.Empty;
            }
        }
#endif
        #endregion

#if !NET_STANDARD
        #region IScriptable Members

        [Browsable(false)]
        [NonPersistent]
        String IScriptable.Name
        {
            get
            {
                return Name;
            }
        }

        [Browsable(false)]
        [NonPersistent]
        public String Code
        {
            get
            {
                return ScriptCode;
            }
            set
            {
                if (value.Length >= 32767)
                {
                    throw new ArgumentException("Code Lenght cannot be greater than 32767");
                }

                ScriptCode = value;
            }
        }

        private int[] breakpoints;
        [Browsable(false)]
        [ValueConverter(typeof(ArrayConverter))]
        public int[] Breakpoints
        {
            get
            {
                return breakpoints;
            }
            set
            {
                SetPropertyValue("Breakpoints", ref breakpoints, value);
            }
        }

        [Browsable(false)]
        [NonPersistent]
        public bool CanReadMacro
        {
            get
            {
                return false;
            }
        }

        [Browsable(false)]
        [NonPersistent]
        public bool CanEdit
        {
            get
            {
                return !IsSubPrototypeMember || !UseShared.Value;
            }
        }
        
        const String separator = "|";
        public void SetListProcedures(IList<String> list)
        {
            if (list == null)
            {
                ListProcedures = null;
                return;
            }

            var buffer = new StringBuilder();
            foreach (var str in list)
            {
                if (buffer.Length != 0)
                    buffer.Append(separator);
                buffer.Append(str);
            }
            ListProcedures = buffer.ToString();
        }

        public String[] GetListProcedures()
        {
            if (ListProcedures == null)
                return null;
            return ListProcedures.Split(new String[] { separator }, StringSplitOptions.RemoveEmptyEntries);
        }

        public IList GetQuickReferenceList()
        {
            return null;
        }

        public IList GetReferenceList()
        {
            var list = new List<Object>();

            var opcua = typeof(DataValue).Assembly;
            list.Add(opcua);

            BaseInstanceState instance = null;
            switch (ModelType)
            {
                case UFUAModel.ModelType.Variable: instance = new DataItemState(null); break;
                case UFUAModel.ModelType.Digital: instance = new TwoStateDiscreteState(null); break;
                case UFUAModel.ModelType.Enumerated: instance = new MultiStateDiscreteState(null); break;
                case UFUAModel.ModelType.Method: instance = new MethodState(null); break;
                case UFUAModel.ModelType.ObjectType: instance = new BaseObjectState(null); break;
                default: instance = new AnalogItemState(null); break;
            }

            var manager = new ServerScriptManager.ServerScriptManager(instance, new Opc.Ua.Server.StandardServer(), null, null, null);
            list.Add(manager);
            list.Add(manager.Instance);
            return list;
        }

        public String GetReferenceName(Object var)
        {
            var opcua = typeof(DataValue).Assembly;
            if (var == opcua)
                return "#";
            
            var title = var.GetType().UnderlyingSystemType.Name;
            return title;
        }

        #endregion
#endif
    }
}