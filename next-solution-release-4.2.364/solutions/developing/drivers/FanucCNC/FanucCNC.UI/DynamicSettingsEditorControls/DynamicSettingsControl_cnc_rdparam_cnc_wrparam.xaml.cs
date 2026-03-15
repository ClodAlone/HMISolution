using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace FanucCNC.UI
{
    public partial class DynamicSettingsControl_cnc_rdparam_cnc_wrparam : UserControl, IDisposable
    {
        private class FC
        {
            public FanucCNCDynTag_cnc_rdparam_cnc_wrparam.ParameterSize Code { get; set; }
            public string Description { get; set; }

            public FC(FanucCNCDynTag_cnc_rdparam_cnc_wrparam.ParameterSize code, string description)
            {
                Code = code;
                Description = description;
            }
        }

        bool bLoaded = false;
        public DynamicSettingsControl_cnc_rdparam_cnc_wrparam()
        {
            InitializeComponent();
            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;
                bLoaded = true;

                List<FC> lst = new List<FC>();
                lst.Add(new FC(FanucCNCDynTag_cnc_rdparam_cnc_wrparam.ParameterSize.ByteSize, "1-byte data is stored (byte)"));
                lst.Add(new FC(FanucCNCDynTag_cnc_rdparam_cnc_wrparam.ParameterSize.WordSize, "2-byte data is stored (Int16)"));
                lst.Add(new FC(FanucCNCDynTag_cnc_rdparam_cnc_wrparam.ParameterSize.DoubleWordSize, "4-byte data is stored (Int32)"));
                lst.Add(new FC(FanucCNCDynTag_cnc_rdparam_cnc_wrparam.ParameterSize.RealSize, "8-byte data is stored (Int64 or Floating point)"));

                cmbPSize.ItemsSource = lst;
            };
        }

        #region IDisposable Members

        public void Dispose()
        {
            
        }
        #endregion
    }
}
