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
    public class CRTag
    {
        #region Constructors
        public CRTag()
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

        private string _ReadablePath;
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
                _ReadablePath = value;
            }
        }

        private bool _IsReferencedTag;
        public bool IsOtherEntitiesReferencedBy
        {
            get
            {
                return _IsReferencedTag;
            }
            set
            {
                _IsReferencedTag = value;
            }
        }

        List<CREntity> _Entities;
        public List<CREntity> Entities
        {
            get
            {
                if (_Entities == null)
                    _Entities = new List<CREntity>();
                return _Entities;
            }
            set
            {
                _Entities = value;
            }
        }
        private CRTagFolder _UFUAFolder;
        public CRTagFolder UFUAFolder
        {
            get
            {
                return _UFUAFolder;
            }
            set
            {
                _UFUAFolder = value;
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
        private bool _IsNotValid;
        public bool IsNotValid
        {
            get
            {
                return _IsNotValid;
            }
            set
            {
                _IsNotValid = value;
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
                _IsNotInUse = value;
            }
        }

        private bool _IsInitialized;
        public bool IsInitialized
        {
            get
            {
                return _IsInitialized;
            }
            set
            {
                _IsInitialized = value;
            }
        }
        private string _AppName;
        public string AppName
        {
            get
            {
                return _AppName;
            }
            set
            {
                _AppName = value;
            }
        }

        private string _ReadablePathNoProject;
        public string ReadablePathNoProject
        {
            get
            {
                if (_ReadablePathNoProject == null)
                    return string.Empty;
                return _ReadablePathNoProject;
            }
            set
            {
                _ReadablePathNoProject = value;
            }
        }

        private string _EndpointUrl;
        public string EndpointUrl
        {
            get
            {
                if (_EndpointUrl == null)
                    return string.Empty;
                return _EndpointUrl;
            }
            set
            {
                _EndpointUrl = value;
            }
        }

        private string _TypeDefinition;
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

        private CRTagFolder _PrototypeFolder;
        public CRTagFolder PrototypeFolder
        {
            get
            {
                return _PrototypeFolder;
            }
            set
            {
                _PrototypeFolder = value;
            }
        }
        #endregion
    }
}
