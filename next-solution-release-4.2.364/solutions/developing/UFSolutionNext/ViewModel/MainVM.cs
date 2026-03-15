using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shell;
using WPFUtilities;

namespace UFSolutionNext.ViewModel
{
    public class MainVM
    {
        public MainVM()
        {
           
        }

        #region Wait Indicator 
        public virtual bool IsBusy { get; set; }
        protected void OnIsBusyChanged()
        {
            if (IsBusy)
            {
                if (DevExpress.Xpf.Core.DXSplashScreen.IsActive)
                    DevExpress.Xpf.Core.DXSplashScreen.Close();

                DevExpress.Xpf.Core.DXSplashScreen.Show<WaitWindow>();
                if (!String.IsNullOrEmpty(WaitIndicatorText))
                    DevExpress.Xpf.Core.DXSplashScreen.SetState(WaitIndicatorText);
            }
            else
            {
                if (DevExpress.Xpf.Core.DXSplashScreen.IsActive)
                    DevExpress.Xpf.Core.DXSplashScreen.Close();

                WaitIndicatorText = savedWaitIndicatorText;
                savedWaitIndicatorText = String.Empty;
            }
        }

        String savedWaitIndicatorText;
        public virtual String WaitIndicatorText { get; set; }
        protected void OnWaitIndicatorTextChanged()
        {
            if (String.IsNullOrEmpty(savedWaitIndicatorText) && !String.IsNullOrEmpty(WaitIndicatorText))
                savedWaitIndicatorText = WaitIndicatorText;
            if (DevExpress.Xpf.Core.DXSplashScreen.IsActive)
                DevExpress.Xpf.Core.DXSplashScreen.SetState(WaitIndicatorText);
        }
        #endregion

        #region Taskbar Service
        public virtual TaskbarItemProgressState TaskbarProgressState { get; set; }

        public virtual double TaskbarProgressValue { get; set; }

        public virtual String TaskbarDescription { get; set; }

        public virtual ImageSource TaskbarOverlayIcon { get; set; }

        protected virtual ITaskbarButtonService TaskbarButtonService { get { return null; } }
        #endregion
    }
}
