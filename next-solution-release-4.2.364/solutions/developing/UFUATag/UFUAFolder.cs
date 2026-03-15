using System;
using System.Linq;
using DevExpress.Xpo;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UFInterfaces.PropertyControl;
#if !NET_STANDARD
using Utilities.Xpo.UndoRedo;
#endif

namespace UFUAModel
{
    [DeferredDeletion(false)]
    public class UFUAFolder : XPObject, IDataErrorInfo, INotifyPropertyReadOnlyChanged
#if !NET_STANDARD
        , IUndoRedoXpo
#endif
    {
        #region Constructors
        public UFUAFolder(Session session)
            : base(session)
        { }
        #endregion

        #region Properties Default Values
        // List of constant default values for each property where you want handle a default value.
        const int defaultMemberOrderId = -1;

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
        }
        #endregion

        #region Not Persistence Properties

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

        [Browsable(false)]
        [NonPersistent]
        public UFUATagPrototype PrototypeReference
        {
            get
            {
                var prototype = UFUATagPrototype;
                if (UFUAFolderAss != null)
                {
                    var root = UFUAFolderAss;
                    while (root.UFUAFolderAss != null)
                        root = root.UFUAFolderAss;

                    prototype = root.UFUATagPrototype;
                }

                return prototype;
            }
        }
        #endregion

        #region Properties
        private int? _MemberOrderId;
        [Custom("Generate", "Null")]
        [Browsable(false)]
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

        private string _User;
        [Size(SizeAttribute.Unlimited)]
        public string User
        {
            get
            {
                return _User;
            }
            set
            {
                SetPropertyValue("User", ref _User, value);
            }
        }

        [Association("UFUAFolder-UFUATags"), Aggregated]
        public XPCollection<UFUATag> UFUATags
        {
            get
            {
                return GetCollection<UFUATag>("UFUATags");
            }
        }

        private UFUAFolder _UFUAFolder;
        [Association("UFUAFolder-UFUAFolders")]
        public UFUAFolder UFUAFolderAss
        {
            get
            {
                if (_UFUAFolder == this)
                    return null;

                return _UFUAFolder;
            }
            set
            {
                if (_UFUAFolder == this)
                    return;

                SetPropertyValue("UFUAFolderAss", ref _UFUAFolder, value);
            }
        }

        [Association("UFUAFolder-UFUAFolders"), Aggregated]
        public XPCollection<UFUAFolder> UFUAFolders
        {
            get
            {
                return GetCollection<UFUAFolder>("UFUAFolders");
            }
        }
       
        private UFUATagPrototype _UFUATagPrototype;
        [Association("UFUATagPrototype-Folders")]
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

        #region Methods

        public string GetRelativePath(UInt16 ns, bool bRelative = false)
        {
            if (bRelative)
            {
                string path = string.Format("{0}:{1}", ns, Name);
                if (UFUAFolderAss != null)
                {
                    path = string.Format("{0}/{2}:{1}", UFUAFolderAss.GetRelativePath(ns, bRelative), Name, ns);
                }
                return path;
            }
            else
            {
                string path = string.Format("{0}:{1}/{0}:{2}", ns, UFUAServerInfo.UFUAServerInfo.GetTagRootName(), Name);
                if (UFUAFolderAss != null)
                {
                    path = string.Format("{0}/{2}:{1}", UFUAFolderAss.GetRelativePath(ns, bRelative), Name, ns);
                }
                return path;
            }
        }

        public string GetFullName()
        {
            string name = string.Format("{0}", Name);
            if (UFUAFolderAss != null)
            {
                name = string.Format("{0}/{1}", UFUAFolderAss.GetFullName(), Name);
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
            if (UFUAFolderAss != null && (!bUseSubPrototypeMember || UFUAFolderAss.IsSubPrototypeMember))
            {
                name = string.Format("{0}/{1}", UFUAFolderAss.GetRelativeName(bUseSubPrototypeMember), Name);
            }
            else if (bUseSubPrototypeMember && IsSubPrototypeMember)
            {
                name = string.Format("{0}/{1}", PrototypeReference.UFUATagOwner.GetRelativeName(bUseSubPrototypeMember), Name);
            }
            return name;
        }

        public List<UFUAModel.UFUATag> GetTagMembers(bool bSorted = false)
        {
            List<UFUAModel.UFUATag> members;
            if (bSorted)
                members = (from c in UFUATags orderby c.MemberOrderId select c).ToList();
            else
                members = new List<UFUAModel.UFUATag>(UFUATags);
            foreach (var folder in UFUAFolders)
                members.AddRange(folder.GetTagMembers(bSorted));

            return members;
        }

        public List<UFUAModel.UFUATag> GetTagMembersSorted()
        {
            var members = new List<UFUAModel.UFUATag>(UFUATags);
            foreach (var folder in UFUAFolders)
                members.AddRange(folder.GetTagMembers());

            return members;
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
                if (IsSubPrototypeMember)
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

        #region IUndoRedoXpo
        [Browsable(false)]
        [NonPersistent]
        public String PathIdentifier
        {
            get
            {
                if (UFUAFolderAss != null)
                    return String.Format("{0}\\{1}", UFUAFolderAss.PathIdentifier, Name);
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
                if (UFUAFolderAss != null)
                    return UFUAFolderAss.UniqueIdentifier;
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
#endif
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
            if (propertyName == "Name")
            {
                if (!Helpers.NameValidator.IsValidName(Name))
                {
                    return Properties.Resources.FolderNameInvalid;
                }
                else if (UFUAFolderAss != null)
                {
                    if ((from c in UFUAFolderAss.UFUAFolders/*.AsParallel()*/
                         where c != this && c.Name == Name
                         select c).ToList().Count > 0)
                    {
                        return Properties.Resources.FolderNameAlreadyExists;
                    }
                }
                else if (UFUATagPrototype != null)
                {
                    if ((from c in UFUATagPrototype.Folders/*.AsParallel()*/
                         where c != this && c.Name == Name
                         select c).ToList().Count > 0)
                    {
                        return Properties.Resources.FolderNameAlreadyExists;
                    }
                }
                else
                {
                    var aliasRoot = UFUAServerInfo.UFUAServerInfo.GetDefaultAliasRootName();
                    var config = (from tag in new XPQuery<UFUAModel.UFUAConfiguration>(Session, true).AsParallel() select tag).FirstOrDefault();
                    if (config != null && !String.IsNullOrEmpty(config.AliasRoot))
                        aliasRoot = config.AliasRoot;
                    if (aliasRoot == Name)
                        return Properties.Resources.FolderNameReserved;

                    if ((from c in new XPQuery<UFUAModel.UFUAFolder>(Session, true)/*.AsParallel()*/
                         where c != this && c.UFUAFolderAss == null && c.UFUATagPrototype == null && c.Name == Name
                         select c).ToList().Count > 0)
                    {
                        return Properties.Resources.FolderNameAlreadyExists;
                    }
                }
            }
            else if (propertyName == "MemberOrderId")
            {
                if (MemberOrderId < 0)
                    return Properties.Resources.MemberMinPositionError;
                else
                {
                    if (UFUAFolderAss != null)
                    {
                        if (MemberOrderId > UFUAFolderAss.UFUATags.Count - 1)
                            return Properties.Resources.MemberMaxPositionError;
                    }
                    else if (MemberOrderId > UFUATagPrototype.Members.Count - 1)
                        return Properties.Resources.MemberMaxPositionError;
                }
            }
#endif
            return null;
        }
    }
}
