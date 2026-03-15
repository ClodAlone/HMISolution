using System;
using System.Windows.Controls;
using System.ComponentModel;

namespace EtherNetIP.UI
{
    /// <summary>
    /// Interaction logic for StationDetails.xaml
    /// </summary>
    public partial class StationDetails : UserControl
    {
        bool bLoaded = false;
        public StationDetails()
        {
            InitializeComponent();

            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;
                bLoaded = true;
                var st = DataContext as EtherNetIPStationSettings;
                var baseStation = new DriverCodeBaseEx.UI.Controls.BaseStationSettings() { DataContext = DataContext };
                baseStation.CmbChannel.ItemsSource = st.DriverSettings.ChannelSettings;
                MainStack.Children.Insert(0,baseStation);
                //CmbChannel.ItemsSource = st.DriverSettings.ChannelSettings;
                CmbPlcType.ItemsSource = Enum.GetValues(typeof(PlcTypes));
                CmbLogix5550NonBlockPhAdd.ItemsSource = Enum.GetValues(typeof(PhisicalAddressesOptimizzations));

                CmbPlcType_TextChanged(null, new EventArgs());

                DependencyPropertyDescriptor descriptor =
                   DependencyPropertyDescriptor.FromProperty(ComboBox.TextProperty, typeof(ComboBox));
                descriptor.AddValueChanged(CmbPlcType, CmbPlcType_TextChanged);
            };
        }
        private void CmbPlcType_TextChanged(object sender, EventArgs e)
        {        
            if (!Enum.TryParse(CmbPlcType.Text, true, out PlcTypes plcType))
                return;

            switch (plcType)
            {
                case PlcTypes.Micro800_series:
                    CmbLogix5550NonBlockPhAdd.Visibility = System.Windows.Visibility.Collapsed;
                    TbLogix5550NonBlockPhAdd.Visibility = System.Windows.Visibility.Collapsed;
                    CmbLogix5550NonBlockPhAdd.SelectedValue = PhisicalAddressesOptimizzations.False;
                    CPUSlot.Visibility = System.Windows.Visibility.Collapsed;
                    TbCPUSlot.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                case PlcTypes.SLC500_MicroLogix:
                    CmbLogix5550NonBlockPhAdd.Visibility = System.Windows.Visibility.Collapsed;
                    TbLogix5550NonBlockPhAdd.Visibility = System.Windows.Visibility.Collapsed;
                    CmbLogix5550NonBlockPhAdd.SelectedValue = PhisicalAddressesOptimizzations.False;
                    CPUSlot.Visibility = System.Windows.Visibility.Visible;
                    TbCPUSlot.Visibility = System.Windows.Visibility.Visible;
                    break;
                case PlcTypes.PLC5:
                    CmbLogix5550NonBlockPhAdd.Visibility = System.Windows.Visibility.Collapsed;
                    TbLogix5550NonBlockPhAdd.Visibility = System.Windows.Visibility.Collapsed;
                    CmbLogix5550NonBlockPhAdd.SelectedValue = PhisicalAddressesOptimizzations.False;
                    CPUSlot.Visibility = System.Windows.Visibility.Visible;
                    TbCPUSlot.Visibility = System.Windows.Visibility.Visible;
                    break;
                default:
                    CmbLogix5550NonBlockPhAdd.Visibility = System.Windows.Visibility.Visible;
                    TbLogix5550NonBlockPhAdd.Visibility = System.Windows.Visibility.Visible;
                    CPUSlot.Visibility = System.Windows.Visibility.Visible;
                    TbCPUSlot.Visibility = System.Windows.Visibility.Visible;
                    break;
            }
        }
    }
}
