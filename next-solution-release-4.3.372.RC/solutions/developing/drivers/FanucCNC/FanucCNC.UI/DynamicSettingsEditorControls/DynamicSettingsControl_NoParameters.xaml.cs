using System;
using System.Windows.Controls;

namespace FanucCNC.UI
{
    public partial class DynamicSettingsControl_NoParameters : UserControl, IDisposable
    {
        public DynamicSettingsControl_NoParameters()
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
