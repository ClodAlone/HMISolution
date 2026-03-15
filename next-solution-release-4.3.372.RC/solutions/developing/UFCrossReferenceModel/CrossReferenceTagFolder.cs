using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using UFInterfaces.Constants;
using System.ComponentModel;
using UFInterfaces;
using System.Windows.Media;
using DocumentManager.ComponentService;
using DevExpress.Xpo;
using System.Windows.Media.Imaging;

namespace UFCrossReferenceModel
{
    [DeferredDeletion(false)]
    public class UFCrossReferenceTagFolder : XPObject
    {
        #region Constructors
        public UFCrossReferenceTagFolder(Session session)
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

        [Association("CrossReferenceTagFolder-Entities"), Aggregated]
        public XPCollection<UFCrossReferenceEntity> Entities
        {
            get
            {
                return GetCollection<UFCrossReferenceEntity>("Entities");
            }
        }

        [Association("CrossReferenceTagFolder-Tags"), Aggregated]
        public XPCollection<UFCrossReferenceTag> Tags
        {
            get
            {
                return GetCollection<UFCrossReferenceTag>("Tags");
            }
        }

        private UFCrossReferenceTagFolder _UFUAFolder;
        [Association("UFCrossReferenceTagFolder-UFCrossReferenceTagFolders")]
        public UFCrossReferenceTagFolder UFUAFolderAss
        {
            get
            {
                return _UFUAFolder;
            }
            set
            {
                SetPropertyValue("UFUAFolderAss", ref _UFUAFolder, value);
            }
        }

        [Association("UFCrossReferenceTagFolder-UFCrossReferenceTagFolders"), Aggregated]
        public XPCollection<UFCrossReferenceTagFolder> UFUAFolders
        {
            get
            {
                return GetCollection<UFCrossReferenceTagFolder>("UFUAFolders");
            }
        }
              
        private string _TypeIcon;
        [ReadOnly(true)]
        public string TypeIcon
        {
            get
            {
                return _TypeIcon;
            }
            set
            {
                SetPropertyValue("TypeIcon", ref _TypeIcon, value);
            }
        }

        private string _TypeDefinition;
        [Size(SizeAttribute.Unlimited)]
        public string TypeDefinition
        {
            get
            {
                return _TypeDefinition;
            }
            set
            {
                _TypeDefinition = value;
            }
        }

        private bool _IsNotValid;
        //[ReadOnly(true)]
        public bool IsNotValid
        {
            get
            {
                return _IsNotValid;
            }
            set
            {
                SetPropertyValue("IsNotValid", ref _IsNotValid, value);
            }
        }

        private bool _IsNotInUse;
        public bool IsNotInUse
        {
            get
            {
                return _IsNotInUse;
            }
            set
            {
                SetPropertyValue("IsNotInUse", ref _IsNotInUse, value);
            }
        }

        private bool _HasPrototypeModel;
        public bool HasPrototypeModel
        {
            get
            {
                return _HasPrototypeModel;
            }
            set
            {
                SetPropertyValue("HasPrototypeModel", ref _HasPrototypeModel, value);
            }
        }
        #endregion

        #region Methods
        public string GetRelativeName()
        {
            string name = string.Format("{0}", Name);
            if (UFUAFolderAss != null)
            {
                name = string.Format("{0}\\{1}", UFUAFolderAss.GetRelativeName(), Name);
            }
            return name;
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

    }
}
