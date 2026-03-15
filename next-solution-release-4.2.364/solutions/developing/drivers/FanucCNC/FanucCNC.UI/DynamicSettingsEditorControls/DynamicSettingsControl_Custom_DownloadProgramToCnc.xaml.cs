using System;
using System.Windows.Controls;

namespace FanucCNC.UI
{
    public partial class DynamicSettingsControl_Custom_DownloadProgramToCnc : UserControl, IDisposable
    {        
        public DynamicSettingsControl_Custom_DownloadProgramToCnc()
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
