using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using UFInterfaces.Constants;
using UFUAModel;
using UFUAModel.Extensions;
using UFInterfaces.Editors;
using UFInterfaces.PropertyControl;
using OPCUAViewModel;
using Utilities;
#if !NET_STANDARD
using UFUAEditor.ComponentService;
#endif

namespace UFRecipeSettings.UFRecipeModel
{
    [DataContract(Name = "DataValueEntity", Namespace = Namespaces.UriProgea)]
#if !NET_STANDARD    
    [SvgValueConverter(typeof(ConvertUFDataValueEntity))]
#endif    
    public class UFDataValueEntity 
#if !NET_STANDARD
        : INotifyPropertyChanged, IDataErrorInfo, ICloneable, IDynamicSettingsEditing, INotifyPropertyVisibilityChanged, INotifyPropertyReadOnlyChanged
#endif
    {
#region Persistance

        [DataMember]
        Guid _NodeId;
        [DataMember]
        int _OID = -1;
        [DataMember]
        string _DataValueName;
        [DataMember]
        string _Description;
        [DataMember]
        UFUAModel.DataType _DataType = UFUAModel.DataType.Float;
        [DataMember]
        uint _ArrayDimension;
        [DataMember]
        String _DefaultValue;
        [DataMember]
        bool _AllowNull = true;
        [DataMember]
        bool _UseInCommunication = false;
        [DataMember]
        StringEncodingType _Encoding = StringEncodingType.ASCII;
        [DataMember]
        String startingAddress;
        [DataMember]
        int bytesStringValueSize = -1;
        [DataMember]
        OPCUAEntityReference tagDataValue;
        [DataMember]
        OPCUAEntityReference tagIODataValue;
        [DataMember]
        EditValueControlTypeEnum _EditControlType = EditValueControlTypeEnum.EditDisplay;
        [DataMember]
        String[] _EnumOptions;
        [DataMember]
        String _EngineeringUnit;
        [DataMember]
        String _UnitName;
        [DataMember]
        double _MinValue;
        [DataMember]
        double _MaxValue = 100.0;
        [DataMember]
        int _DecimalDigits = 2;
        [DataMember]
        int _MaxLength = -1;

        [OnDeserializing]
        private void PreInitialize(StreamingContext context)
        {
            _DataType = UFUAModel.DataType.Float;
            _Encoding = StringEncodingType.ASCII;
            _AllowNull = true;
            _UseInCommunication = false;
            bytesStringValueSize = -1;
            _EditControlType = EditValueControlTypeEnum.EditDisplay;
            _MaxValue = 100.0;
            _DecimalDigits = 2;
            _MaxLength = -1;
        }

#endregion

#region Constructors
#if !NET_STANDARD
        public UFDataValueEntity(UFDataValueEntity source)
        {
            if (source == null)
                return;

            CopyAll(source);
        }
#endif

        public UFDataValueEntity()
        {
        }

#endregion

#region Properties

        [Browsable(false)]
        public String Name { get { return DataValueName; } }

        [ReadOnly(true)]
        public Guid NodeId
        {
            get
            {
                return _NodeId;
            }
            set
            {
                if (_NodeId == value)
                    return;

                _NodeId = value;
#if !NET_STANDARD
                OnPropertyChanged("NodeId");
#endif
            }
        }

        [Browsable(false)]
        public int OID
        {
            get
            {
                return _OID;
            }
            set
            {
                if (_OID == value)
                    return;
                _OID = value;
#if !NET_STANDARD
                OnPropertyChanged("OID");
#endif
            }
        }
        
        [MergablePropertyAttribute(false)]
        public string DataValueName
        {
            get
            {
                return _DataValueName;
            }
            set
            {
                if (_DataValueName == value)
                    return;

                _DataValueName = value;
#if !NET_STANDARD
                OnPropertyChanged("DataValueName");
                OnPropertyChanged("Name");
#endif
            }
        }

        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                if (_Description == value)
                    return;

                _Description = value;
#if !NET_STANDARD
                OnPropertyChanged("Description");
#endif
            }
        }

        public UFUAModel.DataType DataType
        {
            get
            {
                return _DataType;
            }
            set
            {
                if (_DataType == value)
                    return;

                _DataType = value;
#if !NET_STANDARD
                OnPropertyChanged("DataType");

                if (value.IsMinMaxType())
                {
                    OnPropertyChanged("MinValue");
                    OnPropertyChanged("MaxValue");
                }
                else if (value.IsVariableLenght())
                {
                    OnPropertyChanged("BytesStringValueSize");
                }

                OnPropertyVisiblityChanged("DataType");
#endif
            }
        }

        public uint ArrayDimension
        {
            get
            {
                return _ArrayDimension;
            }
            set
            {
                if (_ArrayDimension == value)
                    return;

                _ArrayDimension = value;
#if !NET_STANDARD
                OnPropertyChanged("ArrayDimension");
#endif
            }
        }

        public String DefaultValue
        {
            get
            {
                return _DefaultValue;
            }
            set
            {
                if (_DefaultValue == value)
                    return;

                _DefaultValue = value;
#if !NET_STANDARD
                OnPropertyChanged("DefaultValue");
#endif
            }
        }

        public bool AllowNull
        {
            get
            {
                return _AllowNull;
            }
            set
            {
                if (_AllowNull == value)
                    return;

                _AllowNull = value;
#if !NET_STANDARD
                OnPropertyChanged("AllowNull");
                OnPropertyChanged("DefaultValue");
#endif
            }
        }

        public bool UseInCommunication
        {
            get
            {
                return _UseInCommunication;
            }
            set
            {
                if (_UseInCommunication == value)
                    return;

                _UseInCommunication = value;
#if !NET_STANDARD
                OnPropertyChanged("UseInCommunication");
                OnPropertyChanged("BytesStringValueSize");
                OnPropertyVisiblityChanged("UseInCommunication");
#endif
            }
        }

        public StringEncodingType Encoding
        {
            get
            {
                return _Encoding;
            }
            set
            {
                if (_Encoding == value)
                    return;

                _Encoding = value;
#if !NET_STANDARD
                OnPropertyChanged("Encoding");
#endif
            }
        }

        [DisplayName("StringSize")]
        public int BytesStringValueSize
        {
            get
            {
                var size = GetStringSizeFromStartingAddress();
                if (size > 0)
                    return size;

                return bytesStringValueSize;
            }
            set
            {
                if (bytesStringValueSize == value)
                    return;

                bytesStringValueSize = value;
#if !NET_STANDARD
                OnPropertyChanged("BytesStringValueSize");
#endif
            }
        }

        public String StartingAddress
        {
            get
            {
                return startingAddress;
            }
            set
            {
                if (value == startingAddress)
                    return;
                startingAddress = value;
#if !NET_STANDARD
                OnPropertyChanged("StartingAddress");
                OnPropertyChanged("BytesStringValueSize");
                OnPropertyChanged("LinkType");
                OnPropertyReadOnlyChanged("BytesStringValueSize");
#endif
            }
        }

        public OPCUAEntityReference TagDataValue
        {
            get { return tagDataValue; }
            set
            {
                if (value == tagDataValue)
                    return;
                tagDataValue = value;
#if !NET_STANDARD
                OnPropertyChanged("TagDataValue");
#endif
            }
        }

        public OPCUAEntityReference TagIODataValue
        {
            get { return tagIODataValue; }
            set
            {
                if (value == tagIODataValue)
                    return;
                tagIODataValue = value;
#if !NET_STANDARD
                OnPropertyChanged("TagIODataValue");
#endif
            }
        }

        [ReadOnly(true)]
        [Browsable(false)]
        public LinkType LinkType
        {
            get
            {
                if (!String.IsNullOrEmpty(StartingAddress))
                {
                    if (StartingAddress.Contains("LinkType=0"))
                        return LinkType.OnlyRead;
                    else if (StartingAddress.Contains("LinkType=1"))
                        return LinkType.ReadWrite;
                    else if (StartingAddress.Contains("LinkType=2") || StartingAddress.Contains("LinkType=3"))
                        return LinkType.OnlyWrite;
                }

                return LinkType.None;
            }
        }

        UFRecipeEntity _UFRecipeAss;
        [Browsable(false)]
        public UFRecipeEntity UFRecipeAss
        {
            get
            {
                return _UFRecipeAss;
            }
            set
            {
                if (_UFRecipeAss == value)
                    return;
                _UFRecipeAss = value;
#if !NET_STANDARD
                OnPropertyChanged("UFRecipeAss");
#endif
            }
        }

        UFGroupEntity _UFGroupAss;
        [Browsable(false)]
        public UFGroupEntity UFGroupAss
        {
            get
            {
                return _UFGroupAss;
            }
            set
            {
                if (_UFGroupAss == value)
                    return;
                _UFGroupAss = value;
#if !NET_STANDARD
                OnPropertyChanged("UFGroupAss");
#endif
            }
        }

#region Edit Style
        [Category("Editing")]
        public EditValueControlTypeEnum EditControlType
        {
            get
            {
                return _EditControlType;
            }
            set
            {
                if (_EditControlType == value)
                    return;
                _EditControlType = value;
#if !NET_STANDARD
                OnPropertyChanged("EditControlType");
                OnPropertyVisiblityChanged("EditControlType");
#endif
            }
        }

        [Category("Editing")]
        public String[] EnumOptions
        {
            get
            {
                return _EnumOptions;
            }
            set
            {
                if (_EnumOptions == value)
                    return;
                _EnumOptions = value;
#if !NET_STANDARD
                OnPropertyChanged("EnumOptions");
#endif
            }
        }

        [Category("Editing")]
        public String EngineeringUnit
        {
            get
            {
                return _EngineeringUnit;
            }
            set
            {
                if (_EngineeringUnit == value)
                    return;
                _EngineeringUnit = value;
#if !NET_STANDARD
                OnPropertyChanged("EngineeringUnit");
#endif
            }
        }

        [Category("Editing")]
        public String UnitName
        {
            get
            {
                return _UnitName;
            }
            set
            {
                if (_UnitName == value)
                    return;
                _UnitName = value;
#if !NET_STANDARD
                OnPropertyChanged("UnitName");
#endif
            }
        }

        [Category("Editing")]
        public double MinValue
        {
            get
            {
                return _MinValue;
            }
            set
            {
                if (_MinValue == value)
                    return;
                _MinValue = value;
#if !NET_STANDARD
                OnPropertyChanged("MinValue");
#endif
            }
        }

        [Category("Editing")]
        public double MaxValue
        {
            get
            {
                return _MaxValue;
            }
            set
            {
                if (_MaxValue == value)
                    return;
                _MaxValue = value;
#if !NET_STANDARD
                OnPropertyChanged("MaxValue");
#endif
            }
        }

        [Category("Editing")]
        public int DecimalDigits
        {
            get
            {
                return _DecimalDigits;
            }
            set
            {
                if (_DecimalDigits == value)
                    return;
                _DecimalDigits = value;
#if !NET_STANDARD
                OnPropertyChanged("DecimalDigits");
#endif
            }
        }

        [Category("Editing")]
        public int MaxLength
        {
            get
            {
                return _MaxLength;
            }
            set
            {
                if (_MaxLength == value)
                    return;
                _MaxLength = value;
#if !NET_STANDARD
                OnPropertyChanged("MaxLength");
#endif
            }
        }
#endregion

#endregion

#region ICloneable Members
#if !NET_STANDARD
        public UFDataValueEntity CreateSnapshot()
        {
            var cloneDataValue = Clone() as UFDataValueEntity;
            cloneDataValue.ClearAllSubscriptions();

            if (tagDataValue != null)
                cloneDataValue.tagDataValue = tagDataValue.Clone() as OPCUAEntityReference;
            if (tagIODataValue != null)
                cloneDataValue.tagIODataValue = tagIODataValue.Clone() as OPCUAEntityReference;

            return cloneDataValue;
        }

        public object Clone()
        {
            return MemberwiseClone() as UFDataValueEntity;
        }
#endif
#endregion ICloneable Members

#region INotifyPropertyChanged Members
#if !NET_STANDARD

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }
#endif
#endregion

#region IDataErrorInfo
#if !NET_STANDARD
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
#endif
#endregion

#region IDynamicSettingsEditing Members
#if !NET_STANDARD
        [Browsable(false)]
        public bool IsMethod
        {
            get { return false; }
        }

        [Browsable(false)]
        public string FolderPath
        {
            get { return string.Empty; }
        }
        [Browsable(false)]
        public string TagOwnerPath
        {
            get { return string.Empty; }
        }
        [Browsable(false)]
        public bool IsObjectType
        {
            get { return false; }
        }

        [Browsable(false)]
        public string DynamicSettingsForEditing
        {
            get
            {
                return StartingAddress;
            }
            set
            {
                if (StartingAddress == value)
                    return;

                StartingAddress = value;
                OnPropertyChanged("DynamicSettingsForEditing");
            }
        }

        [Browsable(false)]
        int IDynamicSettingsEditing.DataType
        {
            get 
            { 
                if (UseInCommunication && (IsReadable() || IsWritable()))
                    return (int)DataType;

                return -1;
            }
            set 
            {
                if (DataType != (UFUAModel.DataType)value)
                    _DataType = (UFUAModel.DataType)value; 
            }
        }

        [Browsable(false)]
        public IList<IDynamicSettingsEditing> Members
        {
            get
            {
                return null;
            }
        }
#endif
#endregion

#region INotifyPropertyVisibilityChanged Members
#if !NET_STANDARD

        /// <summary>
        /// Gets the visibility state for the property with the given name.
        /// </summary>
        /// <param name="propertyName">The property name that you want konw the current visibility state.</param>
        /// <returns></returns>
        bool INotifyPropertyVisibilityChanged.this[string propertyName]
        {
            get 
            {
                if (propertyName == "StartingAddress" || propertyName == "LinkType")
                {
                    return UseInCommunication;
                }
                else if (propertyName == "BytesStringValueSize")
                {
                    return UseInCommunication && DataType == UFUAModel.DataType.String;
                }
                else if (propertyName == "Encoding")
                {
                    return UseInCommunication && DataType.IsVariableLenght();
                }
                else if (propertyName == "EngineeringUnit" || propertyName == "UnitName" || propertyName == "MinValue" || propertyName == "MaxValue")
                {
                    return EditControlType == EditValueControlTypeEnum.EditDisplay && DataType.IsMinMaxType();
                }
                else if (propertyName == "DecimalDigits")
                {
                    return EditControlType == EditValueControlTypeEnum.EditDisplay && DataType.IsDecimalType();
                }
                else if (propertyName == "EnumOptions")
                {
                    if (EditControlType != EditValueControlTypeEnum.ComboBox && EditControlType != EditValueControlTypeEnum.RadioButton && EditControlType != EditValueControlTypeEnum.ListView)
                        return false;
                }
                else if (propertyName == "MaxLength")
                {
                    return EditControlType == EditValueControlTypeEnum.EditDisplay && DataType.IsVariableLenght();
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
#endif
#endregion

#region INotifyPropertyReadOnlyChanged Members
#if !NET_STANDARD
        /// <summary>
        /// Gets the readonly state for the property with the given name.
        /// </summary>
        /// <param name="propertyName">The property name that you want konw the current readonly state.</param>
        /// <returns></returns>

        bool INotifyPropertyReadOnlyChanged.this[string propertyName]
        {
            get
            {
                if (propertyName == "BytesStringValueSize")
                {
                    return GetStringSizeFromStartingAddress() != -1;
                }

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
#endif
#endregion

#region Methods
#if !NET_STANDARD

        public Dictionary<string, object> ToDictionary()
        {
            Dictionary<string, object> res = new Dictionary<string, object>();
            res.Add("Name", Name);
            res.Add("Description", Description);
            res.Add("UFGroupAss", UFGroupAss?.Name);
            res.Add("DataType", DataType);
            res.Add("ArrayDimension", ArrayDimension);
            res.Add("DefaultValue", DefaultValue);
            res.Add("AllowNull", AllowNull);
            res.Add("UseInCommunication", UseInCommunication);
            res.Add("DecimalDigits", DecimalDigits);
            res.Add("EditControlType", EditControlType);
            res.Add("EngineeringUnit", EngineeringUnit);
            res.Add("UnitName", UnitName);
            res.Add("MinValue", MinValue);
            res.Add("MaxValue", MaxValue);
            res.Add("EnumOptions", EnumOptions);
            return res;
        }

        public void CopyAll(UFDataValueEntity source)
        {
            _Description = source._Description;
            _DataType = source._DataType;
            _ArrayDimension = source._ArrayDimension;
            _DefaultValue = source._DefaultValue;
            _AllowNull = source._AllowNull;
            _UseInCommunication = source._UseInCommunication;
            _Encoding = source._Encoding;
            startingAddress = source.startingAddress;
            bytesStringValueSize = source.bytesStringValueSize;
            if (source.tagDataValue != null)
                tagDataValue = source.tagDataValue.Clone() as OPCUAEntityReference;
            else
                tagDataValue = null;
            if (source.tagIODataValue != null)
                tagIODataValue = source.tagIODataValue.Clone() as OPCUAEntityReference;
            else
                tagIODataValue = null;
            _EditControlType = source._EditControlType;
            _EnumOptions = source._EnumOptions;
            _EngineeringUnit = source._EngineeringUnit;
            _UnitName = source._UnitName;
            _MinValue = source._MinValue;
            _MaxValue = source._MaxValue;
            _DecimalDigits = source._DecimalDigits;
            _MaxLength = source._MaxLength;
        }
#endif

        public bool IsWritable()
        {
            if (!UseInCommunication)
                return false;

            if (String.IsNullOrEmpty(StartingAddress))
                return UFGroupAss == null || String.IsNullOrEmpty(UFGroupAss.StartingAddress);
            else
                return LinkType == UFRecipeModel.LinkType.OnlyWrite || LinkType == UFRecipeModel.LinkType.ReadWrite;
        }

        public bool IsReadable()
        {
            if (!UseInCommunication)
                return false;

            if (String.IsNullOrEmpty(StartingAddress))
                return UFGroupAss == null || String.IsNullOrEmpty(UFGroupAss.StartingAddress);
            else
                return LinkType == UFRecipeModel.LinkType.OnlyRead || LinkType == UFRecipeModel.LinkType.ReadWrite;
        }

        public bool IsTagReferenceValid()
        {
            return TagDataValue != null && TagDataValue.IsValid;
        }

        public bool IsTagIOReferenceValid()
        {
            return TagIODataValue != null && TagIODataValue.IsValid;
        }

        int GetStringSizeFromStartingAddress()
        {
            if (String.IsNullOrEmpty(StartingAddress))
                return -1;

            var index = StartingAddress.LastIndexOf(':');
            if (index != -1)
            {
                int bytes = -1;
                if (int.TryParse(StartingAddress.Substring(index + 1), out bytes))
                    return bytes;
            }

            return -1;
        }

#if !NET_STANDARD
        protected String PerformValidation(String propertyName)
        {
            if (propertyName == "DataValueName")
            {
                if (String.IsNullOrWhiteSpace(DataValueName))
                {
                    return Properties.Resources.ObjectNameMissing;
                }
                else if (UFRecipeAss != null)
                {
                    if ((from c in UFRecipeAss.DataValues.AsParallel()
                         where c != this && c.DataValueName == DataValueName && c.NodeId != NodeId
                         select c).ToList().Count > 0)
                    {
                        return Properties.Resources.DataValueNameAlreadyExists;
                    }
                }
                else if (UFGroupAss != null)
                {
                    if ((from c in UFGroupAss.DataValues.AsParallel()
                         where c != this && c.DataValueName == DataValueName && c.NodeId != NodeId
                         select c).ToList().Count > 0)
                    {
                        return Properties.Resources.DataValueNameAlreadyExists;
                    }

                }
            }
            else if (propertyName == "DefaultValue")
            {
                if (!AllowNull && String.IsNullOrEmpty(DefaultValue))
                    return Properties.Resources.EmptyDefaultValue;
                else if (!String.IsNullOrEmpty(DefaultValue))
                {
                    var value = DataTypeExtensions.ChangeType(DefaultValue, DataType, (int)ArrayDimension);
                    if ((ArrayDimension > 0 || !DataType.IsVariableLenght()) && value == null)
                        return Properties.Resources.InvalidDefaultValue;
                }
            }
            else if (propertyName == "BytesStringValueSize")
            {
                var dynamicAddress = StartingAddress;
                if (String.IsNullOrEmpty(dynamicAddress) && UFGroupAss != null)
                    dynamicAddress = UFGroupAss.StartingAddress;
                if (String.IsNullOrEmpty(dynamicAddress) && UFRecipeAss != null)
                    dynamicAddress = UFRecipeAss.StartingAddress;

                if (UseInCommunication && !String.IsNullOrEmpty(dynamicAddress) && DataType.IsVariableLenght() && BytesStringValueSize <= 0)
                    return Properties.Resources.MissingStringSize;
            }
            else if (propertyName == "EngineeringUnit")
            {
                if (UFRecipeAss != null && UFRecipeAss.Document != null && !String.IsNullOrEmpty(EngineeringUnit))
                {
                    var ufuaEditorService = UFRecipeAss.Document.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
                    var ue = ufuaEditorService.GetEngineeringUnit(UFRecipeAss.Document, EngineeringUnit);
                    if (ue == null)
                        return Properties.Resources.MissingEngineeringUnit;
                }
                
            }

            return null;
        }

        internal void ClearAllSubscriptions()
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                var delegates = handler.GetInvocationList();
                foreach (var delegateItem in delegates)
                    PropertyChanged -= (PropertyChangedEventHandler)delegateItem;
            }
        }
#endif
#endregion
    }

#if !NET_STANDARD
    internal class ConvertUFDataValueEntity : CustomValueConverter
    {
        public override object ConvertFromStorageType(object value, object sender)
        {
            throw new NotImplementedException();
        }

        public override object ConvertToStorageType(object value, object sender, object document, object property)
        {
            var entity = value as UFDataValueEntity;
            if (entity == null)
                return (new UFDataValueEntity()).ToDictionary();

            return entity.ToDictionary();
        }
        public override Type StorageType
        {
            get
            {
                return typeof(Dictionary<string, object>);
            }
        }
    }
#endif    
}
