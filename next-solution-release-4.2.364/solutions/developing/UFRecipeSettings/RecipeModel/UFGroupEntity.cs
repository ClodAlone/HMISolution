using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using UFInterfaces.Constants;
using UFInterfaces.Editors;
using Utilities;

namespace UFRecipeSettings.UFRecipeModel
{
    [DataContract(Name = "GroupEntity", Namespace = Namespaces.UriProgea)]
    public class UFGroupEntity
#if !NET_STANDARD
        : INotifyPropertyChanged, IDataErrorInfo, ICloneable, IDynamicSettingsEditing
#endif
    {
                
#region Persistance

        [DataMember]
        Guid _NodeId;
        [DataMember]
        int _OID = -1;
        [DataMember]
        string _GroupName;
        [DataMember]
        string _Description;
        [DataMember]
        String startingAddress;
        //[DataMember]
        //LinkType _LinkType = LinkType.ReadWrite;

        [DataMember]
        List<UFDataValueEntity> _DataValues;

        [OnDeserializing]
        private void PreInitialize(StreamingContext context)
        {
            //_LinkType = LinkType.ReadWrite;
        }

        [OnDeserialized]
        private void PostInitialize(StreamingContext context)
        {
            int oid = 0;
            var values = DataValues.OrderBy(o => o.OID);
            foreach (var value in values)
            {
                value.UFGroupAss = this;
                value.OID = oid++;
            }
        }

#endregion

#region Constructors
#if !NET_STANDARD
        public UFGroupEntity(UFGroupEntity source)
        {
            if (source == null)
                return;

            CopyAll(source);
        }
#endif

        public UFGroupEntity()
        {
        }

#endregion

#region Properties

        [Browsable(false)]
        public String Name { get { return GroupName; } }

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
        public string GroupName
        {
            get
            {
                return _GroupName;
            }
            set
            {
                if (_GroupName == value)
                    return;

                _GroupName = value;
#if !NET_STANDARD
                OnPropertyChanged("GroupName");
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
                OnPropertyChanged("LinkType");
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

        #endregion

        #region ICloneable Members
#if !NET_STANDARD
        public UFGroupEntity CreateSnapshot()
        {
            var cloneGroup = Clone() as UFGroupEntity;
            cloneGroup.ClearAllSubscriptions();
            foreach (var dataValue in DataValues)
            {
                var cloneDataValue = dataValue.Clone() as UFDataValueEntity;
                cloneDataValue.ClearAllSubscriptions();
                cloneDataValue.UFGroupAss = cloneGroup;
                cloneGroup.DataValues.Add(cloneDataValue);
            }

            return cloneGroup;
        }

        public object Clone()
        {
            var clone = MemberwiseClone() as UFGroupEntity;
            clone._DataValues = null;
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
        public int DataType
        {
            get 
            {
                if (DataValues.Count > 0)
                {
                    var writabledatavalues = (from c in DataValues
                                  where c.UseInCommunication
                                  orderby c.OID ascending
                                  select c).ToList();

                    if (writabledatavalues.Count > 0)
                        return (int)writabledatavalues[0].DataType;
                }

                return -1; 
            }
            set 
            { 
                // do nothing
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

#region Methods
#if !NET_STANDARD
        public void CopyAll(UFGroupEntity source)
        {
            _Description = source._Description;
            startingAddress = source.startingAddress;
            //_LinkType = source._LinkType;
            if (source._DataValues != null)
            {
                var sourceString = source._DataValues.ToXml();
                _DataValues = sourceString.FromXml<List<UFDataValueEntity>>();
                _DataValues.ForEach((datavalue) => datavalue.NodeId = Guid.NewGuid());
            }
            else
                _DataValues = null;
        }
#endif

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

#if !NET_STANDARD
        protected String PerformValidation(String propertyName)
        {
            if (propertyName == "GroupName")
            {
                if (String.IsNullOrWhiteSpace(GroupName))
                {
                    return Properties.Resources.ObjectNameMissing;
                }
                else if (UFRecipeAss != null)
                {
                    if ((from c in UFRecipeAss.Groups.AsParallel()
                         where c != this && c.GroupName == GroupName && c.NodeId != NodeId
                         select c).ToList().Count > 0)
                    {
                        return Properties.Resources.GroupNameAlreadyExists;
                    }
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
}
