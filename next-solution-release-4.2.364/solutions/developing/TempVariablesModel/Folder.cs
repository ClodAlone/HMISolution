using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Xpo;

namespace TempVariablesModel
{
    public class Folder : XPObject, IDataErrorInfo, XpoHelpers.UndoRedoIXPSimpleObjectHelper.IUniqueIdentifier
    {
        #region Constructors
        public Folder(Session session)
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
            //if (!PropertyName.HasValue)
            //    PropertyName = defaultPropertyName;
            //if (TimeSpanPropertyName == TimeSpan.Zero)
            //    TimeSpanPropertyName = TimeSpan.FromMinutes(1);
            //if (DateTimePropertyName == DateTime.MinValue)
            //    DateTimePropertyName = DateTime.UtcNow;

            if (!MemberOrderId.HasValue)
                MemberOrderId = defaultMemberOrderId;
        }
        #endregion
        
        #region Not Persistence Properties
        #endregion

        #region Properties
        private int? _MemberOrderId;
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

        [Association("Folder-Variables"), Aggregated]
        public XPCollection<Variable> Variables
        {
            get
            {
                return GetCollection<Variable>("Variables");
            }
        }

        private Folder _Folder;
        [Association("Folder-Folders")]
        public Folder FolderAss
        {
            get
            {
                return _Folder;
            }
            set
            {
                SetPropertyValue("FolderAss", ref _Folder, value);
            }
        }

        [Association("Folder-Folders"), Aggregated]
        public XPCollection<Folder> Folders
        {
            get
            {
                return GetCollection<Folder>("Folders");
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
                if (FolderAss != null)
                {
                    path = string.Format("{1}:{0}", Name, ns);
                }
                return path;
            }
            else
            {
                string path = string.Format("{0}:{1}&{0}:{2}", ns, Properties.Resources.Tags, Name);
                if (FolderAss != null)
                {
                    path = string.Format("{0}&{2}:{1}", FolderAss.GetRelativePath(ns, bRelative), Name, ns);
                }
                return path;
            }
        }

        public string GetRelativeName()
        {
            string name = string.Format("{0}", Name);
            if (FolderAss != null)
            {
                name = string.Format("{0}&{1}", FolderAss.GetRelativeName(), Name);
            }
            return name;
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

        protected String PerformValidation(String propertyName)
        {
            if (propertyName == "Name")
            {
                if (!UFUAModel.Helpers.NameValidator.IsValidName(Name))
                {
                    return Properties.Resources.FolderNameInvalid;
                }
                else if (FolderAss != null)
                {
                    if ((from c in FolderAss.Folders/*.AsParallel()*/
                         where c != this && c.Name == Name
                         select c).ToList().Count > 0)
                    {
                        return Properties.Resources.FolderNameAlreadyExists;
                    }
                }
                else
                {
                    if ((from c in new XPQuery<Folder>(Session, true)/*.AsParallel()*/
                         where c != this && c.FolderAss == null && c.Name == Name
                         select c).ToList().Count > 0)
                    {
                        return Properties.Resources.FolderNameAlreadyExists;
                    }
                }
            }

            return null;
        }
    }
}
