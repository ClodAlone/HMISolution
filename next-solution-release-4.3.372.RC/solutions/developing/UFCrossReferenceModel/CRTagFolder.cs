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
    public class CRTagFolder
    {
        #region Constructors
        public CRTagFolder()
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

        List<CRTag> _Tags;
        public List<CRTag> Tags
        {
            get
            {
                if (_Tags == null)
                    _Tags = new List<CRTag>();
                return _Tags;
            }
            set
            {
                _Tags = value;
            }
        }

        private CRTagFolder _UFUAFolder;
        public CRTagFolder UFUAFolderAss
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
        List<CRTagFolder> _UFUAFolders;
        public List<CRTagFolder> UFUAFolders
        {
            get
            {
                if (_UFUAFolders == null)
                    _UFUAFolders = new List<CRTagFolder>();
                return _UFUAFolders;
            }
            set
            {
                _UFUAFolders = value;
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
    }
}
