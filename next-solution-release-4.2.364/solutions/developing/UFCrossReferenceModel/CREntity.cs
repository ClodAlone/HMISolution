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
    public class CREntity 
    {
        #region Constructors
        public CREntity()
        { }
        #endregion

        #region Properties

        private string _Name;
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }

        public string TagName
        {
            get
            {
                if (CrossReferenceTag != null)
                    return CrossReferenceTag.Name;
                else if (CrossReferenceScreen != null)
                    return CrossReferenceScreen.Name;
                else
                    return string.Empty; 
            }
        }

        public string EntityReadablePath
        {
            get
            {
                if (CrossReferenceTag != null)
                    return CrossReferenceTag.ReadablePath;
                else if (CrossReferenceScreen != null)
                    return CrossReferenceScreen.ReadablePath;
                else if (CrossReferenceConnection != null)
                    return $"{CrossReferenceConnection.ReadablePath} ({DynamicSettings?.Split('|').LastOrDefault()})";
                else
                    return string.Empty;
            }
        }

        private string _EndpointUrl;
        public string EndpointUrl
        {
            get
            {
                return _EndpointUrl;
            }
            set
            {
                _EndpointUrl = value;
            }
        }
        
        public bool IsNotRefUsed
        {
            get
            {
                if (CrossReferenceTag != null)
                    return CrossReferenceTag.IsNotInUse;
                else 
                    return false;
            }
        }

        public bool IsNotRefValid
        {
            get
            {
                if (CrossReferenceTag != null)
                    return CrossReferenceTag.IsNotValid;
                else if (CrossReferenceScreen != null)
                    return CrossReferenceScreen.IsNotValid;
                else
                    return false;
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
                _HasPrototypeModel = value;
            }
        }

        private CRTag _CrossReferenceTag;
        public CRTag CrossReferenceTag
        {
            get
            {
                return _CrossReferenceTag;
            }
            set
            {
                _CrossReferenceTag = value;
            }
        }

        private CRScreen _CrossReferenceScreen;
        public CRScreen CrossReferenceScreen
        {
            get
            {
                return _CrossReferenceScreen;
            }
            set
            {
                _CrossReferenceScreen = value;
            }
        }

        private CRConnection _CrossReferenceConnection;
        public CRConnection CrossReferenceConnection
        {
            get
            {
                return _CrossReferenceConnection;
            }
            set
            {
                _CrossReferenceConnection = value;
            }
        }

        private CRString _CrossReferenceString;
        public CRString CrossReferenceString
        {
            get
            {
                return _CrossReferenceString;
            }
            set
            {
                _CrossReferenceString = value;
            }
        }

        private string _DynamicSettings;
        public string DynamicSettings
        {
            get
            {
                return _DynamicSettings;
            }
            set
            {
                _DynamicSettings = value;
            }
        }

        private string _Container;
        public string Container
        {
            get
            {
                return _Container;
            }
            set
            {
                _Container = value;
            }
        }

        private string _TypeIcon;
        public string TypeIcon
        {
            get
            {
                return _TypeIcon;
            }
            set
            {
                _TypeIcon = value;
            }
        }

        private string _ReferencedNodeID;
        public string ReferencedNodeID
        {
            get
            {
                return _ReferencedNodeID;
            }
            set
            {
                _ReferencedNodeID = value;
            }
        }

        #endregion
    }
}
