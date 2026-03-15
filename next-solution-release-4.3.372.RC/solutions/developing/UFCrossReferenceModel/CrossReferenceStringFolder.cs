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
    public class UFCrossReferenceStringFolder : XPObject
    {
        #region Constructors
        public UFCrossReferenceStringFolder(Session session)
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

        [Association("CrossReferenceStringFolder-Screens"), Aggregated]
        public XPCollection<UFCrossReferenceString> Screens
        {
            get
            {
                return GetCollection<UFCrossReferenceString>("Screens");
            }
        }


        private UFCrossReferenceStringFolder _UFUAFolder;
        [Association("CrossReferenceStringFolder-CrossReferenceStringFolders")]
        public UFCrossReferenceStringFolder UFUAFolderAss
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

        [Association("CrossReferenceStringFolder-CrossReferenceStringFolders"), Aggregated]
        public XPCollection<UFCrossReferenceStringFolder> UFUAFolders
        {
            get
            {
                return GetCollection<UFCrossReferenceStringFolder>("UFUAFolders");
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

        private bool _IsNotValid;
        [ReadOnly(true)]
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
