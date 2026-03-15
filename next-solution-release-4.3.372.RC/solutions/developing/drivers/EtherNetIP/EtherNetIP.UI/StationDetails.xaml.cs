using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EtherNetIP;
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
                var baseStation = new DriverCodeBase.UI.Controls.BaseStationSettings() { DataContext = DataContext };
                baseStation.CmbChannel.ItemsSource = st.DriverSettings.ChannelSettings;
                MainStack.Children.Add(baseStation);
                //CmbChannel.ItemsSource = st.DriverSettings.ChannelSettings;
                CmbPlcType.ItemsSource = Enum.GetValues(typeof(PlcTypes));
                CmbLogix5550NonBlockPhAdd.ItemsSource = Enum.GetValues(typeof(PhisicalAddressesOptimizzations));

                CmbPlcType_TextChanged();

                DependencyPropertyDescriptor descriptor =
                   DependencyPropertyDescriptor.FromProperty(ComboBox.TextProperty, typeof(ComboBox));
                descriptor.AddValueChanged(CmbPlcType, CmbPlcType_TextChanged);
            };
        }
        private void CmbPlcType_TextChanged(object sender, EventArgs e)
        {
            CmbPlcType_TextChanged();
        }

        private void CmbPlcType_TextChanged()
        {
            if (CmbPlcType.Text == String.Empty)
                return;
            if (CmbPlcType.Text == PlcTypes.Micro800_series.ToString())
            {
                CmbLogix5550NonBlockPhAdd.Visibility = System.Windows.Visibility.Collapsed;
                TbLogix5550NonBlockPhAdd.Visibility = System.Windows.Visibility.Collapsed;
                CPUSlot.Visibility = System.Windows.Visibility.Collapsed;
                TbCPUSlot.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                CmbLogix5550NonBlockPhAdd.Visibility = System.Windows.Visibility.Visible;
                TbLogix5550NonBlockPhAdd.Visibility = System.Windows.Visibility.Visible;
                CPUSlot.Visibility = System.Windows.Visibility.Visible;
                TbCPUSlot.Visibility = System.Windows.Visibility.Visible;
            }

        }
    }
}
