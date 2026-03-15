using System;
using System.Windows.Controls;

namespace FanucCNC.UI
{
    public partial class DynamicSettingsControl_cnc_rdzofs_cnc_wrzofs : UserControl, IDisposable
    {        
        public DynamicSettingsControl_cnc_rdzofs_cnc_wrzofs()
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
