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
    public class UFCrossReferenceScreen : XPObject
    {
        #region Constructors
        public UFCrossReferenceScreen(Session session)
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

        #region Non Persistence Values
        [Browsable(false)]
        [NonPersistent]
        public string ReadablePathWithoutExtension
        {
            get
            {
                if (!String.IsNullOrEmpty(ReadablePath))
                    return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(ReadablePath), System.IO.Path.GetFileNameWithoutExtension(ReadablePath));
                else
                    return ReadablePath;
            }
        }

        [Browsable(false)]
        [NonPersistent]
        public string ResourceType
        {
            get
            {
                string resourceType = String.Empty;
                var folder = UFUAFolder;
                while (folder != null)
                {
                    resourceType = folder.Name;
                    folder = folder.UFUAFolderAss;
                }

                return resourceType;
            }
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

        private string _ReadablePath;
        [Size(SizeAttribute.Unlimited)]
        public string ReadablePath
        {
            get
            {
                if (_ReadablePath == null)
                    return string.Empty;
                return _ReadablePath;
            }
            set
            {
                if (SetPropertyValue("ReadablePath", ref _ReadablePath, value))
                    RaisePropertyChangedEvent(nameof(ReadablePathWithoutExtension));
            }
        }

        [Association("CrossReferenceScreen-Entities"), Aggregated]
        public XPCollection<UFCrossReferenceEntity> Entities
        {
            get
            {
                return GetCollection<UFCrossReferenceEntity>("Entities");
            }
        }

        private UFCrossReferenceScreenFolder _UFUAFolder;
        [Association("CrossReferenceScreenFolder-Screens"), Aggregated]
        public UFCrossReferenceScreenFolder UFUAFolder
        {
            get
            {
                return _UFUAFolder;
            }
            set
            {
                SetPropertyValue("UFUAFolder", ref _UFUAFolder, value);
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
        private string _ReadablePathProject;
        [Size(SizeAttribute.Unlimited)]
        public string ReadablePathProject
        {
            get
            {
                if (_ReadablePathProject == null)
                    return string.Empty;
                return _ReadablePathProject;
            }
            set
            {
                SetPropertyValue("ReadablePathProject", ref _ReadablePathProject, value);
            }
        }
        private bool _IsInitialized;
        //[ReadOnly(true)]
        public bool IsInitialized
        {
            get
            {
                return _IsInitialized;
            }
            set
            {
                SetPropertyValue("IsInitialized", ref _IsInitialized, value);
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
        #endregion

    }
}
