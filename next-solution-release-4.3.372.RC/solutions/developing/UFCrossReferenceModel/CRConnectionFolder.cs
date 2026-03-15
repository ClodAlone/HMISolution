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
    public class CRConnectionFolder
    {
        #region Constructors
        public CRConnectionFolder()
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

        List<CRConnection> _Screens;
        public List<CRConnection> Screens
        {
            get
            {
                if (_Screens == null)
                    _Screens = new List<CRConnection>();
                return _Screens;
            }
            set
            {
                _Screens = value;
            }
        }


        private CRConnectionFolder _UFUAFolder;
        public CRConnectionFolder UFUAFolderAss
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

        List<CRConnectionFolder> _UFUAFolders;
        public List<CRConnectionFolder> UFUAFolders
        {
            get
            {
                if (_UFUAFolders == null)
                    _UFUAFolders = new List<CRConnectionFolder>();
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
