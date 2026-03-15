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
    public class UFCrossReferenceEntity : XPObject
    {
        #region Constructors
        public UFCrossReferenceEntity(Session session)
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
        public string ResourceType
        {
            get
            {
                return CrossReferenceScreen?.ResourceType;
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

        [ReadOnly(true)]
        [Size(SizeAttribute.Unlimited)]
        public string FolderName
        {
            get
            {
                if (CrossReferenceTagFolder != null)
                    return CrossReferenceTagFolder.Name;
                else
                    return string.Empty;
            }
        }

        [ReadOnly(true)]
        [Size(SizeAttribute.Unlimited)]
        public string TagName
        {
            get
            {
                if (CrossReferenceTag != null)
                    return CrossReferenceTag.Name;
                else if (CrossReferenceScreen != null)
                    return CrossReferenceScreen.Name;
                else if (CrossReferenceString != null)
                    return CrossReferenceString.Name;
                else
                    return string.Empty; 
            }
        }

        [ReadOnly(true)]
        [Size(SizeAttribute.Unlimited)]
        public string EntityReadablePath
        {
            get
            {
                if (CrossReferenceTag != null)
                    return CrossReferenceTag.ReadablePath;
                else if (CrossReferenceScreen != null)
                    return CrossReferenceScreen.ReadablePathWithoutExtension;
                else if (CrossReferenceConnection != null)
                    return $"{CrossReferenceConnection.ReadablePath} ({DynamicSettings?.Split('|').LastOrDefault()})";
                else if (CrossReferenceString != null)
                    return CrossReferenceString.ReadablePath;
                else
                    return string.Empty;
            }
        }

        private string _EndpointUrl;
        [ReadOnly(true)]
        [Size(SizeAttribute.Unlimited)]
        public string EndpointUrl
        {
            get
            {
                return _EndpointUrl;
            }
            set
            {
                SetPropertyValue("EndpointUrl", ref _EndpointUrl, value);
            }
        }
        
        [ReadOnly(true)]
        [Size(SizeAttribute.Unlimited)]
        public bool IsNotRefUsed
        {
            get
            {
                if (CrossReferenceTag != null)
                    return CrossReferenceTag.IsNotInUse;
                if (CrossReferenceString != null)
                    return CrossReferenceString.IsNotInUse;
                else 
                    return false;
            }
        }
        [ReadOnly(true)]
        [Size(SizeAttribute.Unlimited)]
        public bool IsNotRefValid
        {
            get
            {
                if (CrossReferenceTag != null)
                    return CrossReferenceTag.IsNotValid;
                else if (CrossReferenceScreen != null)
                    return CrossReferenceScreen.IsNotValid;
                if (CrossReferenceString != null)
                    return CrossReferenceString.IsNotValid;
                else
                    return false;
            }
        }

        private UFCrossReferenceTagFolder _CrossReferenceTagFolder;
        [Association("CrossReferenceTagFolder-Entities")]
        public UFCrossReferenceTagFolder CrossReferenceTagFolder
        {
            get
            {
                return _CrossReferenceTagFolder;
            }
            set
            {
                SetPropertyValue("CrossReferenceTagFolder", ref _CrossReferenceTagFolder, value);
            }
        }

        private UFCrossReferenceTag _CrossReferenceTag;
        [Association("CrossReferenceTag-Entities")]
        public UFCrossReferenceTag CrossReferenceTag
        {
            get
            {
                return _CrossReferenceTag;
            }
            set
            {
                SetPropertyValue("CrossReferenceTag", ref _CrossReferenceTag, value);
            }
        }

        private UFCrossReferenceScreen _CrossReferenceScreen;
        [Association("CrossReferenceScreen-Entities")]
        public UFCrossReferenceScreen CrossReferenceScreen
        {
            get
            {
                return _CrossReferenceScreen;
            }
            set
            {
                SetPropertyValue("CrossReferenceScreen", ref _CrossReferenceScreen, value);
            }
        }

        private UFCrossReferenceConnection _CrossReferenceConnection;
        [Association("CrossReferenceConnection-Entities")]
        public UFCrossReferenceConnection CrossReferenceConnection
        {
            get
            {
                return _CrossReferenceConnection;
            }
            set
            {
                SetPropertyValue("CrossReferenceConnection", ref _CrossReferenceConnection, value);
            }
        }

        private UFCrossReferenceString _CrossReferenceString;
        [Association("CrossReferenceString-Entities")]
        public UFCrossReferenceString CrossReferenceString
        {
            get
            {
                return _CrossReferenceString;
            }
            set
            {
                SetPropertyValue("CrossReferenceString", ref _CrossReferenceString, value);
            }
        }

        private string _DynamicSettings;
        [ReadOnly(true)]
        [Size(SizeAttribute.Unlimited)]
        public string DynamicSettings
        {
            get
            {
                return _DynamicSettings;
            }
            set
            {
                SetPropertyValue("DynamicSettings", ref _DynamicSettings, value);
            }
        }

        private string _Container;
        [ReadOnly(true)]
        [Size(SizeAttribute.Unlimited)]
        public string Container
        {
            get
            {
                return _Container;
            }
            set
            {
                SetPropertyValue("Container", ref _Container, value);
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

        private string _ReferencedNodeID;
        [Size(SizeAttribute.Unlimited)]
        public string ReferencedNodeID
        {
            get
            {
                return _ReferencedNodeID;
            }
            set
            {
                SetPropertyValue("ReferencedNodeID", ref _ReferencedNodeID, value);
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
    }
}
