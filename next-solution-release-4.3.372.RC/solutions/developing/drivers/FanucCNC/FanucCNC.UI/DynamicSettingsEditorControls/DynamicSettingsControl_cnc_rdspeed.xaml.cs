using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace FanucCNC.UI
{
    public partial class DynamicSettingsControl_cnc_rdspeed : UserControl, IDisposable
    {
        private class FC
        {
            public FanucCNCDynTag_cnc_rdspeed.ReadSpeedType Code { get; set; }
            public string Description { get; set; }

            public FC(FanucCNCDynTag_cnc_rdspeed.ReadSpeedType code, string description)
            {
                Code = code;
                Description = description;
            }
        }

        //FanucCNCDynTa thisTagSettings;
        public DynamicSettingsControl_cnc_rdspeed()
        {
            InitializeComponent();
            Loaded += (o, e) =>
            {
                List<FC> lst = new List<FC>();
                lst.Add(new FC(FanucCNCDynTag_cnc_rdspeed.ReadSpeedType.FeedRate, FanucCNCDynTag_cnc_rdspeed.ReadSpeedType.FeedRate.ToString()));
                lst.Add(new FC(FanucCNCDynTag_cnc_rdspeed.ReadSpeedType.SpindleSpeed, FanucCNCDynTag_cnc_rdspeed.ReadSpeedType.SpindleSpeed.ToString()));
                cmbField.ItemsSource = lst;
            };            
        }       
        #region IDisposable Members

        public void Dispose()
        {
            
        }
        #endregion
    }
}
