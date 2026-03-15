using System;
using System.Windows.Controls;

namespace FanucCNC.UI
{
    public partial class DynamicSettingsControl_pmc_rdpmcrng_pmc_wrpmcrng : UserControl, IDisposable
    {        
        public DynamicSettingsControl_pmc_rdpmcrng_pmc_wrpmcrng()
        {
            InitializeComponent();
            Loaded += (o, e) =>
            {
            };            
        }       

        #region IDisposable Members

        public void Dispose()
        {
            
        }
        #endregion
    }
}
