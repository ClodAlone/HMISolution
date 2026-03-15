using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Core;

namespace NextAR.View
{

    ///<summary>
    /// The ViewModel with dependency properties intended for the use within a DXPage.
    /// When the ViewModel is used as a DataContext of a DXPage, you can use the
    /// ISupportSaveLoadState interface methods to save/load the
    /// ViewModel's state when the application is suspended/resumed.
    ///</summary>
    public class QRReaderViewModel : BindableBase, ISupportSaveLoadState
    {
        public QRReaderViewModel()
        {
            Header = "Header";
        }

        private string _header;
        public string Header
        {
            get { return _header; }
            set
            {
                SetProperty<string>(ref _header, value, "Header");
            }
        }

        #region ISupportSaveLoadState Members

        public void LoadState(object navigationParameter, PageStateStorage pageState)
        {
        }

        public void SaveState(PageStateStorage pageState)
        {
        }

        #endregion
    }
}
