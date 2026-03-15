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
    public class CRScreenFolder
    {
        #region Constructors
        public CRScreenFolder()
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

        List<CRScreen> _Screens;
        public List<CRScreen> Screens
        {
            get
            {
                if (_Screens == null)
                    _Screens = new List<CRScreen>();
                return _Screens;
            }
            set
            {
                _Screens = value;
            }
        }


        private CRScreenFolder _UFUAFolder;
        public CRScreenFolder UFUAFolderAss
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
        List<CRScreenFolder> _UFUAFolders;
        public List<CRScreenFolder> UFUAFolders
        {
            get
            {
                if (_UFUAFolders == null)
                    _UFUAFolders = new List<CRScreenFolder>();
                return _UFUAFolders;
            }
            set
            {
                _UFUAFolders = value;
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
