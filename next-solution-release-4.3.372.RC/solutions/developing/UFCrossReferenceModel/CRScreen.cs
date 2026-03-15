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
    public class CRScreen  
    {
        #region Constructors
        public CRScreen()
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

        private CRScreenFolder _UFUAFolder;
        public CRScreenFolder UFUAFolder
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
        private string _ReadablePathProject;
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
                _ReadablePathProject = value;
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
        #endregion

        #region Methods
        #endregion

    }
}
