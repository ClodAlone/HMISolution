using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Xpo;

namespace UFEventModel
{
    public class UFEventFolder : XPObject, IDataErrorInfo, XpoHelpers.UndoRedoIXPSimpleObjectHelper.IUniqueIdentifier
    {
        #region Ctor
        public UFEventFolder(DevExpress.Xpo.Session session)
            : base(session)
        {

        }
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
            //if (!PropertyName.HasValue)
            //    PropertyName = defaultPropertyName;
            //if (TimeSpanPropertyName == TimeSpan.Zero)
            //    TimeSpanPropertyName = TimeSpan.FromMinutes(1);
            //if (DateTimePropertyName == DateTime.MinValue)
            //    DateTimePropertyName = DateTime.UtcNow;
        }
        #endregion

        #region Properties
        private string _Name;
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

        [Association("UFEventFolder-UFEventObjects"), Aggregated]
        public XPCollection<UFEventObject> UFEventObjects
        {
            get
            {
                return GetCollection<UFEventObject>("UFEventObjects");
            }
        }

        private UFEventFolder _UFEventFolder;
        [Association("UFEventFolder-UFEventFolders")]
        public UFEventFolder UFEventFolderAss
        {
            get
            {
                return _UFEventFolder;
            }
            set
            {
                SetPropertyValue("UFEventFolderAss", ref _UFEventFolder, value);
            }
        }

        [Association("UFEventFolder-UFEventFolders"), Aggregated]
        public XPCollection<UFEventFolder> UFEventFolders
        {
            get
            {
                return GetCollection<UFEventFolder>("UFEventFolders");
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
                else if (UFEventFolderAss != null)
                {
                    if ((from c in UFEventFolderAss.UFEventFolders/*.AsParallel()*/
                         where c != this && c.Name == Name
                         select c).ToList().Count > 0)
                    {
                        return Properties.Resources.FolderNameAlreadyExists;
                    }
                }
                else
                {
                    if ((from c in new XPQuery<UFEventFolder>(Session, true)/*.AsParallel()*/
                         where c != this && c.UFEventFolderAss == null && c.Name == Name
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
