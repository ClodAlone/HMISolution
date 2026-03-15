using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace FanucCNC.UI
{
    public partial class DynamicSettingsControl_cnc_statinfo : UserControl, IDisposable
    {
        private class FC
        {
            public FanucCNCDynTag_cnc_statinfo.FieldType Code { get; set; }
            public string Description { get; set; }

            public FC(FanucCNCDynTag_cnc_statinfo.FieldType code, string description)
            {
                Code = code;
                Description = description;
            }
        }

        bool bLoaded =false;
        public DynamicSettingsControl_cnc_statinfo()
        {
            InitializeComponent();
            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;
                bLoaded = true;

                List<FC> lst = new List<FC>();
                lst.Add(new FC(FanucCNCDynTag_cnc_statinfo.FieldType.Tmmode, FanucCNCDynTag_cnc_statinfo.FieldType.Tmmode.ToString()));
                lst.Add(new FC(FanucCNCDynTag_cnc_statinfo.FieldType.Aut, FanucCNCDynTag_cnc_statinfo.FieldType.Aut.ToString()));
                lst.Add(new FC(FanucCNCDynTag_cnc_statinfo.FieldType.Run, FanucCNCDynTag_cnc_statinfo.FieldType.Run.ToString()));
                lst.Add(new FC(FanucCNCDynTag_cnc_statinfo.FieldType.Edit, FanucCNCDynTag_cnc_statinfo.FieldType.Edit.ToString()));
                lst.Add(new FC(FanucCNCDynTag_cnc_statinfo.FieldType.Motion, FanucCNCDynTag_cnc_statinfo.FieldType.Motion.ToString()));
                lst.Add(new FC(FanucCNCDynTag_cnc_statinfo.FieldType.Mstb, FanucCNCDynTag_cnc_statinfo.FieldType.Mstb.ToString()));
                lst.Add(new FC(FanucCNCDynTag_cnc_statinfo.FieldType.Emergency, FanucCNCDynTag_cnc_statinfo.FieldType.Emergency.ToString()));
                lst.Add(new FC(FanucCNCDynTag_cnc_statinfo.FieldType.Alarm, FanucCNCDynTag_cnc_statinfo.FieldType.Alarm.ToString()));
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
