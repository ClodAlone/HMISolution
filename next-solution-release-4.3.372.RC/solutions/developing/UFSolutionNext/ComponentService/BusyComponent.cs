using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UFInterfaces;
using UFInterfaces.CoreHostComponents;
using UFSolutionNext.ViewModel;
using DevExpress.Mvvm.POCO;

namespace UFSolution
{
    class BusyComponent : ComponentBase<IBusyComponent>, IBusyComponent
    {
        #region Declarations
        MainVM mainVM;
        int busyCounter = 0;
        #endregion

        #region Constructors
        public BusyComponent()
        {
            Initialize();
        }
        #endregion

        #region IBusyComponent
        public void Initialize()
        {
            if (mainVM == null)
                mainVM = ViewModelSource.Create(() => new MainVM());
        }

        public bool IsBusy
        {
            get 
            {
                return mainVM.IsBusy;
            }
            set
            {
                if (value)
                {
                    if (++busyCounter >= 1)
                        mainVM.IsBusy = true;
                }
                else if (--busyCounter <= 0)
                {
                    mainVM.IsBusy = false;
                    BusyContent = null;
                    OnClosed(this);
                }
            }
        }

        public string BusyContent
        {
            get
            {
                return mainVM.WaitIndicatorText;
            }
            set
            {
                if (mainVM.WaitIndicatorText != value)
                    mainVM.WaitIndicatorText = value;
            }
        }

        public void ResetBusy()
        {
            mainVM.IsBusy = false;
        }

        public void RestoreBusy()
        {
            if (busyCounter >= 1)
                mainVM.IsBusy = true;
        }

        public void SetProgress(double value, double maxValue = 100.0)
        {
            if (DevExpress.Xpf.Core.DXSplashScreen.IsActive)
                DevExpress.Xpf.Core.DXSplashScreen.Progress(value, maxValue);
        }

        public void ResetProgress()
        {
            if (DevExpress.Xpf.Core.DXSplashScreen.IsActive)
                DevExpress.Xpf.Core.DXSplashScreen.Progress(0);
        }

        public event EventHandler Closed;
        void OnClosed(Object sender)
        {
            Closed?.Invoke(sender, EventArgs.Empty);
        }
        #endregion

        #region Properties
        public MainVM Instance
        {
            get
            {
                return mainVM;
            }
        }
        #endregion

        #region Overrides
        protected override void Dispose(bool disposing)
        {
            try
            {
                DevExpress.Xpf.Core.DXSplashScreen.Close();
            }
            catch { };
            busyCounter = 0;
            mainVM.IsBusy = false;

            base.Dispose(disposing);
        }
        #endregion
    }
}
