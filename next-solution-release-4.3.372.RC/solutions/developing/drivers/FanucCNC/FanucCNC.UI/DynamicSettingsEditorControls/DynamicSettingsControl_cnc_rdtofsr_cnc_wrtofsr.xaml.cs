using System;
using System.Windows.Controls;

namespace FanucCNC.UI
{
    public partial class DynamicSettingsControl_cnc_rdtofsr_cnc_wrtofsr : UserControl, IDynamicSettingsChildrenControlRefresh, IDisposable
    {
        FanucCNCDynTag_cnc_rdtofsr_cnc_wrtofsr functionSettings;
        bool bLoaded = false;

        public DynamicSettingsControl_cnc_rdtofsr_cnc_wrtofsr()
        {
            InitializeComponent();
            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;

                bLoaded = true;

                functionSettings = DataContext as FanucCNCDynTag_cnc_rdtofsr_cnc_wrtofsr;

                var lst = functionSettings.GetListMemoryType();

                // check if code if compatible with selected list
                if (lst.Find(a => a.Code == functionSettings.OffsetMemoryType) == null)
                    functionSettings.OffsetMemoryType = lst[0].Code;

                cmbOffsetMemoryType.ItemsSource = lst;
                                                
                cmbOffsetMemoryType.SelectedValue = functionSettings.OffsetMemoryType;
            };
        }

        private void cmbOffsetMemoryType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = (ComboBox)sender;

            if (cb.SelectedIndex != -1)
            {
                FanucCNCDynTag_cnc_rdtofsr_cnc_wrtofsr.ToolsOffsetMemory v = ((FanucCNCDynTag_cnc_rdtofsr_cnc_wrtofsr.FC)cb.Items[cb.SelectedIndex]).Code;
                FillListOffsetType(v);
            }
        }

        private void FillListOffsetType(FanucCNCDynTag_cnc_rdtofsr_cnc_wrtofsr.ToolsOffsetMemory v)
        {
            var lst = functionSettings.GetListTypeParameter(v);
            cmbOffsetType.ItemsSource = lst;
            if (lst.Count > 0)
            {
                // check if code if compatible with selected list
                if (lst.Find(a => a.Code == functionSettings.OffsetType) == null)
                    functionSettings.OffsetType = lst[0].Code;

                cmbOffsetType.SelectedValue = functionSettings.OffsetType;
            }
        }

        public void interfaceDataChanged(FanucCNCProtocol.MachineSeries machineSerie)// , short cncPath)
        {
            functionSettings.SetChannelStationInfo(machineSerie);//, cncPath);
            var lst = functionSettings.GetListMemoryType();
            if (lst.Count > 0)
            {
                FillListOffsetType(lst[0].Code);

                cmbOffsetMemoryType.ItemsSource = lst;

                cmbOffsetMemoryType.SelectedIndex = 0;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            
        }
        #endregion
    }
}
