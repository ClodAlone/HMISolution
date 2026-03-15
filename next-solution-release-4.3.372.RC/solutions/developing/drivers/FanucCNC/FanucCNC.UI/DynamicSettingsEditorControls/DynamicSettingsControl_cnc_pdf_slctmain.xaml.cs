using System;
using System.Windows.Controls;

namespace FanucCNC.UI
{
    public partial class DynamicSettingsControl_cnc_pdf_slctmain : UserControl, IDisposable
    {        
        public DynamicSettingsControl_cnc_pdf_slctmain()
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
