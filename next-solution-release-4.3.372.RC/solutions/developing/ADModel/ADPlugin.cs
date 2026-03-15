using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using System.ComponentModel;
using UFUAModel;

namespace ADModel
{
    public class ADPlugin : XPObject, IDataErrorInfo, XpoHelpers.UndoRedoIXPSimpleObjectHelper.IUniqueIdentifier
    {
        #region Constructor
        public ADPlugin(Session session)
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
        [ReadOnly(true)]
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

        private string _AssemblyName;
        [ReadOnly(true)]
        [Size(SizeAttribute.Unlimited)]
        public string AssemblyName
        {
            get
            {
                return _AssemblyName;
            }
            set
            {
                SetPropertyValue("AssemblyName", ref _AssemblyName, value);
            }
        }
        
        private string _Description;
        [ReadOnly(true)]
        [Size(SizeAttribute.Unlimited)]
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

        private ADGeneralSettings _ADGeneralSettings;
        [Association("ADGeneralSettings-ADPlugins")]
        [Browsable(false)]
        public ADGeneralSettings ADGeneralSettings
        {
            get
            {
                return _ADGeneralSettings;
            }
            set
            {
                SetPropertyValue("ADGeneralSettings", ref _ADGeneralSettings, value);
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
                return Name;
            }
        }
        #endregion


        #region IDataErrorInfo Members
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
            /*if (propertyName == "Name")
            {
                if (UFUAFolderAss != null)
                {
                    if ((from c in UFUAFolderAss.UFUAFolders.AsParallel()
                         where c != this && c.Name == Name
                         select c).ToList().Count > 0)
                    {
                        return Properties.Resources.FolderNameAlreadyExists;
                    }
                }
                else if (UFUATagPrototype != null)
                {
                    if ((from c in UFUATagPrototype.Folders.AsParallel()
                         where c != this && c.Name == Name
                         select c).ToList().Count > 0)
                    {
                        return Properties.Resources.FolderNameAlreadyExists;
                    }
                }
                else
                {
                    if ((from c in new XPQuery<UFUAModel.UFUAFolder>(Session, true).AsParallel()
                         where c != this && c.UFUAFolderAss == null && c.UFUATagPrototype == null && c.Name == Name
                         select c).ToList().Count > 0)
                    {
                        return Properties.Resources.FolderNameAlreadyExists;
                    }
                }
            }*/

            return null;
        }
    }
}
