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
    public class CRStringFolder
    {
        #region Constructors
        public CRStringFolder()
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

        List<CRString> _Screens;
        public List<CRString> Screens
        {
            get
            {
                if (_Screens == null)
                    _Screens = new List<CRString>();
                return _Screens;
            }
            set
            {
                _Screens = value;
            }
        }


        private CRStringFolder _UFUAFolder;
        public CRStringFolder UFUAFolderAss
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

        List<CRStringFolder> _UFUAFolders;
        public List<CRStringFolder> UFUAFolders
        {
            get
            {
                if (_UFUAFolders == null)
                    _UFUAFolders = new List<CRStringFolder>();
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
