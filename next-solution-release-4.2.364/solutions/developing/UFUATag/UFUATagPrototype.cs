using System;
using System.Linq;
using DevExpress.Xpo;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Utilities;
#if !NET_STANDARD
using Utilities.Xpo.UndoRedo;
#endif

namespace UFUAModel
{
    [Exportable(RequiredKeys = new string[] { "Name" })]
    [DeferredDeletion(false)]
    public class UFUATagPrototype : XPObject, IDataErrorInfo
#if !NET_STANDARD
        , IUndoRedoXpo
#endif
    {
        #region Ctor
        public UFUATagPrototype(Session session)
            : base(session)
        { }
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
            //if (!_PropertyName.HasValue)
            //    _PropertyName = defaultPropertyName;
            //if (_TimeSpanPropertyName == TimeSpan.Zero)
            //    _TimeSpanPropertyName = TimeSpan.FromMinutes(1);
            //if (_DateTimePropertyName == DateTime.MinValue)
            //    _DateTimePropertyName = DateTime.UtcNow;

            if (_CreateDate == DateTime.MinValue)
                _CreateDate = DateTime.UtcNow;
        }
        #endregion

        #region Not Persistence Properties
        [Browsable(false)]
        [NonPersistent]
        public String TagOwnerPath
        {
            get
            {
                String ret = String.Empty;
                if(UFUATagOwner != null)
                {
                    ret = UFUATagOwner.GetFullName();
                }

                return ret;
            }
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

        [Association("UFUATagPrototype-Members"), Aggregated]
        public XPCollection<UFUATag> Members
        {
            get
            {
                return GetCollection<UFUATag>("Members");
            }
        }

        [Association("UFUATagPrototype-Folders"), Aggregated]
        public XPCollection<UFUAFolder> Folders
        {
            get
            {
                return GetCollection<UFUAFolder>("Folders");
            }
        }

        private UFUATag _UFUATagOwner;
        [Association("UFUATag-SubPrototypeMembers")]
        [Browsable(false)]
        public UFUATag UFUATagOwner
        {
            get
            {
                return _UFUATagOwner;
            }
            set
            {
                SetPropertyValue("UFUATagOwner", ref _UFUATagOwner, value);
            }
        }

        private DateTime _CreateDate;
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

        #region Public Methods
        public List<UFUAModel.UFUATag> GetTagMembers(bool bSorted = false)
        {
            List<UFUAModel.UFUATag> members;
            if (bSorted)
                members = (from c in Members orderby c.MemberOrderId select c).ToList();
            else
                members = new List<UFUAModel.UFUATag>(Members);
            foreach (var folder in Folders)
                members.AddRange(folder.GetTagMembers(bSorted));

            return members;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Check the members order id in order to ensure a unique identifier for each member
        /// </summary>
        /// 
        bool bEnsuredUniqueMembersOrderId;
        public void EnsureUniqueMembersOrderId()
        {
            if (bEnsuredUniqueMembersOrderId)
                return;
            bEnsuredUniqueMembersOrderId = true;
            int oid = 0;
            var members = Members.OrderBy(o => GetMemberOrderId(o.MemberOrderId));

            foreach (var member in members)
                member.MemberOrderId = oid++;

            var folders = this.Folders.OrderBy(o=> GetMemberOrderId(o.MemberOrderId));
            oid = 0;
            foreach (var folder in folders)
            {
                folder.MemberOrderId = oid++;
                EnsureUniqueMembersOrderId(folder);
            }
        }

        private void EnsureUniqueMembersOrderId(UFUAModel.UFUAFolder folder)
        {
            int oid = 0;
            var members = folder.UFUATags.OrderBy(o => GetMemberOrderId(o.MemberOrderId));
            foreach (var member in members)
                member.MemberOrderId = oid++;

            var folders = folder.UFUAFolders.OrderBy(o => GetMemberOrderId(o.MemberOrderId));
            oid = 0;
            foreach (var subfolder in folders)
            {
                subfolder.MemberOrderId = oid++;
                EnsureUniqueMembersOrderId(subfolder);
            }
        }

        int GetMemberOrderId(int? id)
        {
            if (!id.HasValue || id.Value == -1)
                return int.MaxValue;
            else
                return id.Value;
        }

        public void InvalidateMembersOrderId()
        {
            bEnsuredUniqueMembersOrderId = false;
            RaisePropertyChangedEvent("InvalidateMembersOrderId");
        }

        #endregion

        #region IUndoRedoXpo
        [Browsable(false)]
        [NonPersistent]
        public String PathIdentifier
        {
            get
            {
                if (UFUATagOwner != null)
                    return String.Format("{0}\\{1}", UFUATagOwner.PathIdentifier, Name);
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
                return String.Empty;
            }
        }

        [Browsable(false)]
        [NonPersistent]
        public String OwnerIdentifier
        {
            get
            {
                if (UFUATagOwner != null)
                    return UFUATagOwner.UniqueIdentifier;
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
                    return Properties.Resources.TagNameInvalid;
                }
                else if ((from c in new XPQuery<UFUAModel.UFUATagPrototype>(Session, true)/*.AsParallel()*/
                          where c != this && c.Name == Name && c.UFUATagOwner == null
                          select c).ToList().Count > 0)
                {
                    return Properties.Resources.PrototypeNameAlreadyExists;
                }
            }
#endif
            return null;
        }
    }
}
