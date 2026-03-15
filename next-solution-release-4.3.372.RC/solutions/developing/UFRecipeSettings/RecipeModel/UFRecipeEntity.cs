using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
#if !NET_STANDARD
using System.Windows.Media;
using System.Windows.Controls;
using UFInterfaces.PropertyControl;
#endif
using DevExpress.Xpo.DB.Helpers;
using UFInterfaces;
using UFInterfaces.Constants;
using UFInterfaces.Editors;
using UFRecipeSettings.Documents;
using OPCUAViewModel;
using Utilities;

namespace UFRecipeSettings.UFRecipeModel
{
    [DataContract(Name = "RecipeEntity", Namespace = Namespaces.UriProgea)]
    public class UFRecipeEntity : IEntityReference
#if !NET_STANDARD
        , IDynamicSettingsEditing, INotifyPropertyChanged, INotifyPropertyVisibilityChanged, IDataErrorInfo, ICloneable
#endif
    {
#region Declarations

        public UFRecipeDocument Document;

#endregion

#region Persistance

        [DataMember]
        Guid _NodeId;
        [DataMember]
        string _TableName;
        [DataMember]
        string _Description;
        [DataMember]
        int _MaxLength = 255;
        [DataMember]
        bool skipCheckColumnsType;
        [DataMember]
        String startingAddress;
        [DataMember]
        OPCUAEntityReference tagRecipeList;
        [DataMember]
        OPCUAEntityReference tagRecipeIndex;
        [DataMember]
        OPCUAEntityReference tagRecipeState;
        [DataMember]
        OPCUAEntityReference tagRecipeLoad;
        [DataMember]
        OPCUAEntityReference tagRecipeSave;
        [DataMember]
        OPCUAEntityReference tagRecipeDelete;
        [DataMember]
        OPCUAEntityReference tagRecipeWrite;
        [DataMember]
        OPCUAEntityReference tagRecipeRead;
        [DataMember]
        string _DataProvider;
        [DataMember]
        string _ConnectionString;
        [DataMember]
        string _DataProviderDisplayName;
        [DataMember]
        string _DataProviderShortDisplayName;
        [DataMember]
        string _DataProviderDescription;
        [DataMember]
        string _DataSourceName;
        [DataMember]
        string _DataSourceDisplayName;
        //[DataMember]
        //LinkType _LinkType = LinkType.ReadWrite;
        [DataMember]
        int userAccessLevel;
        [DataMember]
        bool auditTraceEnabled;
        [DataMember]
        bool enterCommentOnAudit;
        [DataMember]
        bool enterPasswordOnAudit;
        [DataMember]
        int minAccessLevelRequiredOnAudit;

        /*
        [DataMember]
        OPCUAEntityReference opcuaEntityReferenceRead;
        [DataMember]
        OPCUAEntityReference opcuaEntityReferenceWrite;
        */

        [DataMember]
        List<UFDataValueEntity> _DataValues;
        [DataMember]
        List<UFGroupEntity> _Groups;

        [OnDeserializing]
        private void PreInitialize(StreamingContext context)
        {
            _MaxLength = 255;
            //_LinkType = LinkType.ReadWrite;
        }

        [OnDeserialized]
        private void PostInitialize(StreamingContext context)
        {
            // ensure a valid recipe association value and oid value
            int oid = 0;
            var values = DataValues.OrderBy(o => o.OID);
            foreach (var value in values)
            {
                value.UFRecipeAss = this;
                value.OID = oid++;
            }

            oid = 0;
            var groups = Groups.OrderBy(o => o.OID);
            foreach (var group in groups)
            {
                group.UFRecipeAss = this;
                group.OID = oid++;
            }
        }

#endregion

#region Properties

        [Browsable(false)]
        public String Name { get { return RecipeName; } }

        //int _DefaultMaxLenght;
        //public int DefaultMaxLenght
        //{
        //    get
        //    {
        //        return _DefaultMaxLenght;
        //    }
        //    set
        //    {
        //        if (_DefaultMaxLenght == value)
        //            return;

        //        _DefaultMaxLenght = value;

        //    }
        //}

        [Browsable(false)]
        public String ReadableConnectionString
        {
            get
            {
                if (String.IsNullOrEmpty(DataProvider) || String.IsNullOrEmpty(ConnectionString))
                    return String.Empty;

                return String.Format("DataProvider={0};{1}", String.IsNullOrEmpty(DataProviderDisplayName) ? DataProvider : DataProviderDisplayName, ConnectionString);
            }
            set 
            {
                if (ReadableConnectionString == value)
                    return;

                if (String.IsNullOrEmpty(value))
                {
                    ConnectionString =
                        DataProvider =
                        DataProviderDisplayName =
                        DataProviderDescription =
                        DataProviderShortDisplayName =
                        DataSourceName =
                        DataSourceDisplayName = String.Empty;
                }
                else
                {
                    ConnectionStringParser helper = new ConnectionStringParser(value);
                    //if (helper.PartExists("DataProvider"))
                    //{
                    //    var dataProvider = helper.GetPartByName("DataProvider");
                    //    helper.RemovePartByName("DataProvider");
                    //    if (DataProvider != dataProvider)
                    //        DataProvider = dataProvider;
                    //}

                    helper.RemovePartByName("DataProvider");
                    var connectionString = helper.GetConnectionString();
                    if (ConnectionString != connectionString)
                        ConnectionString = connectionString;
                }
           }
        }

        ConnectionSourceReference connectionSource;
        [Browsable(true)]
        public ConnectionSourceReference ConnectionSource
        {
            get
            {
                if (connectionSource == null)
                    connectionSource = new ConnectionSourceReference(Document);

                return connectionSource;
            }
            set
            {
                if (connectionSource == value)
                    return;
                connectionSource = value;
#if !NET_STANDARD
                OnPropertyChanged("ConnectionSource");
#endif
            }
        }

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

        string _RecipeName;
        [ReadOnly(true)]
        public string RecipeName
        {
            get
            {
                return _RecipeName;
            }
            set
            {
                if (_RecipeName == value)
                    return;

                _RecipeName = value;
#if !NET_STANDARD
                OnPropertyChanged("RecipeName");
                OnPropertyChanged("Name");
#endif
            }
        }

        public string TableName
        {
            get
            {
                return _TableName;
            }
            set
            {
                if (_TableName == value)
                    return;

                _TableName = value;
#if !NET_STANDARD
                OnPropertyChanged("TableName");
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

        public bool SkipCheckColumnsType
        {
            get
            {
                return skipCheckColumnsType;
            }
            set
            {
                if (skipCheckColumnsType == value)
                    return;

                skipCheckColumnsType = value;
#if !NET_STANDARD
                OnPropertyChanged("SkipCheckColumnsType");
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
                if (startingAddress == value)
                    return;
                startingAddress = value;
#if !NET_STANDARD
                OnPropertyChanged("StartingAddress");
                OnPropertyChanged("LinkType");
#endif
            }
        }

        [DisplayNameExtension]
        public OPCUAEntityReference TagRecipeList
        {
            get { return tagRecipeList; }
            set
            {
                if (value == tagRecipeList)
                    return;
                tagRecipeList = value;
#if !NET_STANDARD
                OnPropertyChanged("TagRecipeList");
#endif
            }
        }

        [DisplayNameExtension]
        public OPCUAEntityReference TagRecipeIndex
        {
            get { return tagRecipeIndex; }
            set
            {
                if (value == tagRecipeIndex)
                    return;
                tagRecipeIndex = value;
#if !NET_STANDARD
                OnPropertyChanged("TagRecipeIndex");
#endif
            }
        }

        [DisplayNameExtension]
        public OPCUAEntityReference TagRecipeState
        {
            get { return tagRecipeState; }
            set
            {
                if (value == tagRecipeState)
                    return;
                tagRecipeState = value;
#if !NET_STANDARD
                OnPropertyChanged("TagRecipeState");
#endif
            }
        }

        [DisplayNameExtension]
        public OPCUAEntityReference TagRecipeLoad
        {
            get { return tagRecipeLoad; }
            set
            {
                if (value == tagRecipeLoad)
                    return;
                tagRecipeLoad = value;
#if !NET_STANDARD
                OnPropertyChanged("TagRecipeLoad");
#endif
            }
        }

        [DisplayNameExtension]
        public OPCUAEntityReference TagRecipeSave
        {
            get { return tagRecipeSave; }
            set
            {
                if (value == tagRecipeSave)
                    return;
                tagRecipeSave = value;
#if !NET_STANDARD
                OnPropertyChanged("TagRecipeSave");
#endif
            }
        }

        [DisplayNameExtension]
        public OPCUAEntityReference TagRecipeDelete
        {
            get { return tagRecipeDelete; }
            set
            {
                if (value == tagRecipeDelete)
                    return;
                tagRecipeDelete = value;
#if !NET_STANDARD
                OnPropertyChanged("TagRecipeDelete");
#endif
            }
        }

        [DisplayNameExtension]
        public OPCUAEntityReference TagRecipeWrite
        {
            get { return tagRecipeWrite; }
            set
            {
                if (value == tagRecipeWrite)
                    return;
                tagRecipeWrite = value;
#if !NET_STANDARD
                OnPropertyChanged("TagRecipeWrite");
#endif
            }
        }

        [DisplayNameExtension]
        public OPCUAEntityReference TagRecipeRead
        {
            get { return tagRecipeRead; }
            set
            {
                if (value == tagRecipeRead)
                    return;
                tagRecipeRead = value;
#if !NET_STANDARD
                OnPropertyChanged("TagRecipeRead");
#endif
            }
        }

        [Category("User Access")]
        public int UserAccessLevel
        {
            get
            {
                return userAccessLevel;
            }
            set
            {
                if (userAccessLevel == value)
                    return;

                userAccessLevel = value;
#if !NET_STANDARD
                OnPropertyChanged("UserAccessLevel");
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
                    else 
                        return LinkType.WrongLink;
                }

                return LinkType.None;
            }
        }

        [Browsable(false)]
        public string DataProvider
        {
            get
            {
                return _DataProvider;
            }
            set
            {
                if (_DataProvider == value)
                    return;

                _DataProvider = value;
#if !NET_STANDARD
                OnPropertyChanged("DataProvider");
#endif
            }
        }

        [Browsable(false)]
        public string ConnectionString
        {
            get
            {
                return _ConnectionString;
            }
            set
            {
                if (_ConnectionString == value)
                    return;

                _ConnectionString = value;
#if !NET_STANDARD
                OnPropertyChanged("ConnectionString");
#endif
            }
        }

        [Browsable(false)]
        public string DataProviderDisplayName
        {
            get
            {
                return _DataProviderDisplayName;
            }
            set
            {
                if (_DataProviderDisplayName == value)
                    return;

                _DataProviderDisplayName = value;
#if !NET_STANDARD
                OnPropertyChanged("DataProviderDisplayName");
#endif
            }
        }

        [Browsable(false)]
        public string DataProviderShortDisplayName
        {
            get
            {
                return _DataProviderShortDisplayName;
            }
            set
            {
                if (_DataProviderShortDisplayName == value)
                    return;

                _DataProviderShortDisplayName = value;
#if !NET_STANDARD
                OnPropertyChanged("DataProviderShortDisplayName");
#endif
            }
        }

        [Browsable(false)]
        public string DataProviderDescription
        {
            get
            {
                return _DataProviderDescription;
            }
            set
            {
                if (_DataProviderDescription == value)
                    return;

                _DataProviderDescription = value;
#if !NET_STANDARD
                OnPropertyChanged("DataProviderDescription");
#endif
            }
        }

        [Browsable(false)]
        public string DataSourceName
        {
            get
            {
                return _DataSourceName;
            }
            set
            {
                if (_DataSourceName == value)
                    return;

                _DataSourceName = value;
#if !NET_STANDARD
                OnPropertyChanged("DataSourceName");
#endif
            }
        }

        [Browsable(false)]
        public string DataSourceDisplayName
        {
            get
            {
                return _DataSourceDisplayName;
            }
            set
            {
                if (_DataSourceDisplayName == value)
                    return;

                _DataSourceDisplayName = value;
#if !NET_STANDARD
                OnPropertyChanged("DataSourceDisplayName");
#endif
            }
        }

        public bool AuditTraceEnabled
        {
            get
            {
                return auditTraceEnabled;
            }
            set
            {
                if (auditTraceEnabled == value)
                    return;

                auditTraceEnabled = value;
#if !NET_STANDARD
                OnPropertyChanged("AuditTraceEnabled");
                OnPropertyVisiblityChanged("AuditTraceEnabled");
#endif
            }
        }
        
        public bool EnterCommentOnAudit
        {
            get
            {
                return enterCommentOnAudit;
            }
            set
            {
                if (enterCommentOnAudit == value)
                    return;

                enterCommentOnAudit = value;
#if !NET_STANDARD
                OnPropertyChanged("EnterCommentOnAudit");
                OnPropertyVisiblityChanged("EnterCommentOnAudit");
#endif
            }
        }
        
        public bool EnterPasswordOnAudit
        {
            get
            {
                return enterPasswordOnAudit;
            }
            set
            {
                if (enterPasswordOnAudit == value)
                    return;

                enterPasswordOnAudit = value;
#if !NET_STANDARD
                OnPropertyChanged("EnterPasswordOnAudit");
#endif
            }
        }

        
        public int MinAccessLevelRequiredOnAudit
        {
            get
            {
                return minAccessLevelRequiredOnAudit;
            }
            set
            {
                if (minAccessLevelRequiredOnAudit == value)
                    return;

                minAccessLevelRequiredOnAudit = value;
#if !NET_STANDARD
                OnPropertyChanged("MinAccessLevelRequiredOnAudit");
#endif
            }
        }

        /*
        [Browsable(false)]
        public OPCUAEntityReference OpcuaEntityReferenceRead
        {
            get
            {
                return opcuaEntityReferenceRead;
            }
            set
            {
                if (opcuaEntityReferenceRead == value)
                    return;
                opcuaEntityReferenceRead = value;
                OnPropertyChanged("OpcuaEntityReferenceRead");
            }
        }

        [Browsable(false)]
        public OPCUAEntityReference OpcuaEntityReferenceWrite
        {
            get
            {
                return opcuaEntityReferenceWrite;
            }
            set
            {
                if (opcuaEntityReferenceWrite == value)
                    return;
                opcuaEntityReferenceWrite = value;
                OnPropertyChanged("OpcuaEntityReferenceWrite");
            }
        }
        */

        [Browsable(false)]
        public List<UFDataValueEntity> DataValues
        {
            get
            {
                if (_DataValues == null)
                    _DataValues = new List<UFDataValueEntity>();

                return _DataValues;
            }
        }

        [Browsable(false)]
        public List<UFGroupEntity> Groups
        {
            get
            {
                if (_Groups == null)
                    _Groups = new List<UFGroupEntity>();

                return _Groups;
            }
        }

#endregion

#region ICloneable Members
#if !NET_STANDARD
        public UFRecipeEntity CreateSnapshot()
        {
            var clone = Clone() as UFRecipeEntity;
            clone.ClearAllSubscriptions();

            foreach (var dataValue in DataValues)
            {
                var cloneDataValue = dataValue.Clone() as UFDataValueEntity;
                cloneDataValue.ClearAllSubscriptions();
                cloneDataValue.UFRecipeAss = clone;
                clone.DataValues.Add(cloneDataValue);
            }

            foreach (var group in Groups)
            {
                var cloneGroup = group.Clone() as UFGroupEntity;
                cloneGroup.ClearAllSubscriptions();
                cloneGroup.UFRecipeAss = clone;
                clone.Groups.Add(cloneGroup);

                foreach (var dataValue in group.DataValues)
                {
                    var cloneDataValue = dataValue.Clone() as UFDataValueEntity;
                    cloneDataValue.ClearAllSubscriptions();
                    cloneDataValue.UFGroupAss = cloneGroup;
                    cloneGroup.DataValues.Add(cloneDataValue);
                }
            }

            if (tagRecipeList != null)
                clone.tagRecipeList = tagRecipeList.Clone() as OPCUAEntityReference;
            if (tagRecipeIndex != null)
                clone.tagRecipeIndex = tagRecipeIndex.Clone() as OPCUAEntityReference;
            if (tagRecipeState != null)
                clone.tagRecipeState = tagRecipeState.Clone() as OPCUAEntityReference;
            if (tagRecipeLoad != null)
                clone.tagRecipeLoad = tagRecipeLoad.Clone() as OPCUAEntityReference;
            if (tagRecipeSave != null)
                clone.tagRecipeSave = tagRecipeSave.Clone() as OPCUAEntityReference;
            if (tagRecipeDelete != null)
                clone.tagRecipeDelete = tagRecipeDelete.Clone() as OPCUAEntityReference;
            if (tagRecipeWrite != null)
                clone.tagRecipeWrite = tagRecipeWrite.Clone() as OPCUAEntityReference;
            if (tagRecipeRead != null)
                clone.tagRecipeRead = tagRecipeRead.Clone() as OPCUAEntityReference;

            return clone;
        }

        public object Clone()
        {
            var clone = MemberwiseClone() as UFRecipeEntity;
            clone._DataValues = null;
            clone._Groups = null;
            return clone;
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
                //DispatcherObject dispatcherObject = handler.Target as DispatcherObject;

                var e = new PropertyChangedEventArgs(propertyName);
                //// If the subscriber is a DispatcherObject and different thread
                //if (dispatcherObject != null && dispatcherObject.CheckAccess() == false)
                //{
                //    // Invoke handler in the target dispatcher's thread
                //    dispatcherObject.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, handler, this, e);
                //}
                //else // Execute handler as is
                handler(this, e);
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
                if (propertyName == "EnterCommentOnAudit")
                {
                    return AuditTraceEnabled;
                }
                else if (propertyName == "EnterPasswordOnAudit" || propertyName == "MinAccessLevelRequiredOnAudit")
                {
                    return AuditTraceEnabled && EnterCommentOnAudit;
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

        [Browsable(false)]
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

#region Methods

        public List<UFDataValueEntity> GetFlatDataValuesCollection()
        {
            var values = (from c in DataValues orderby c.OID select c).ToList();
            var groups = (from c in Groups orderby c.OID select c).ToList();
            foreach (var groupitem in groups)
                values.AddRange((from c in groupitem.DataValues orderby c.OID select c).ToList());

            return values;
        }

        public int GetFirstValidDataValueOrderId(UFRecipeModel.UFGroupEntity root = null)
        {
            var datavalues = new List<UFRecipeModel.UFDataValueEntity>();
            if (root != null)
                datavalues.AddRange(root.DataValues);
            else
                datavalues.AddRange(DataValues);

            var values = (from c in datavalues where c.OID >= 0 orderby c.OID descending select c.OID).ToList();
            if (values.Count > 0)
                return values[0];
            else
                return -1;
        }

        public int GetFirstValidGroupOrderId()
        {
            var values = (from c in Groups where c.OID >= 0 orderby c.OID descending select c.OID).ToList();
            if (values.Count > 0)
                return values[0];
            else
                return -1;
        }

        public bool IsWritable()
        {
            return !String.IsNullOrEmpty(StartingAddress) &&
                    (LinkType == UFRecipeModel.LinkType.OnlyWrite || LinkType == UFRecipeModel.LinkType.ReadWrite);
        }

        public bool IsReadable()
        {
            return !String.IsNullOrEmpty(StartingAddress) &&
                    (LinkType == UFRecipeModel.LinkType.OnlyRead || LinkType == UFRecipeModel.LinkType.ReadWrite);
        }

        protected String PerformValidation(String propertyName)
        {
            if (propertyName == "RecipeName")
            {
                // RecipeName cannot be changed
            }
            else if (propertyName == "TableName")
            {
                if (!String.IsNullOrEmpty(TableName) && !Helpers.DataSetHelper.IsValidTableName(TableName))
                {
                    return Properties.Resources.RecipeTableNameInvalidChars;
                }
            }

            return null;
        }

#if !NET_STANDARD
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
        public int DataType
        {
            get 
            {
                var writabledatavalues = (from c in GetFlatDataValuesCollection()
                                          where c.UseInCommunication && (c.IsWritable() || c.IsReadable())
                                          select c).ToList();
                if (writabledatavalues.Count > 0)
                    DataType = (int)writabledatavalues[0].DataType;

                return -1; 
            }
            set 
            { 
                // do nothing.
            }
        }

        // FOGBUGZ 9836
        private uint _ArrayDimension = 0;
        [Browsable(false)]
        public uint ArrayDimension
        {
            get { return _ArrayDimension; }
            set { _ArrayDimension = value; }
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

#region IEntityReference Members

        [Browsable(false)]
        public ImageSource CollapsedImageSource
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        public ImageSource ExpandedImageSource
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        public ContextMenu contextMenu
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        public object Tooltip
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        public object ContainedObject
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        public object EntityParent
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        public string TypeDefinitionString
        {
            get
            {
                return null;
            }
        }

#endregion
    }
}
