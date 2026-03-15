using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace ROCDriver.UI
{
    /// <summary>
    /// Interaction logic for StationDetails.xaml
    /// </summary>
    public partial class StationDetails : UserControl
    {
        bool bLoaded = false;
        DependencyPropertyDescriptor descriptor = null;
        public StationDetails()
        {
            InitializeComponent();

            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;
                bLoaded = true;
                var st = DataContext as ROCDriverStationSettings;
                var baseStation = new DriverCodeBaseEx.UI.Controls.BaseStationSettings() { DataContext = DataContext };
                baseStation.CmbChannel.ItemsSource = st.DriverSettings.ChannelSettings;
                MainStack.Children.Insert(0,baseStation);
                textPingPointType.Visibility = Visibility.Collapsed;
                TbPingPointType.Visibility = Visibility.Collapsed;
                textPingLogicalNumber.Visibility = Visibility.Collapsed;
                TbPingLogicalNumber.Visibility = Visibility.Collapsed;
                textPingParameter.Visibility = Visibility.Collapsed;
                TbPingParameter.Visibility = Visibility.Collapsed;
                descriptor = DependencyPropertyDescriptor.FromProperty(CheckBox.IsCheckedProperty, typeof(CheckBox));
                descriptor.AddValueChanged(checkCheckDeviceConnection, CheckDeviceConnection_IsCheckedChanged);
                CheckDeviceConnection_IsCheckedChanged(checkCheckDeviceConnection);
            };

            Unloaded += (o, e) =>
            {
                descriptor.RemoveValueChanged(checkCheckDeviceConnection, CheckDeviceConnection_IsCheckedChanged);
            };
        }

        private void CheckDeviceConnection_IsCheckedChanged(object sender = null, EventArgs e = null)
        {
            var cb = (CheckBox)sender;
            if (cb != null)
            {
                if (cb.IsChecked == true)
                {
                    textPingPointType.Visibility = Visibility.Visible;
                    TbPingPointType.Visibility = Visibility.Visible;
                    textPingLogicalNumber.Visibility = Visibility.Visible;
                    TbPingLogicalNumber.Visibility = Visibility.Visible;
                    textPingParameter.Visibility = Visibility.Visible;
                    TbPingParameter.Visibility = Visibility.Visible;
                }
                else
                {
                    textPingPointType.Visibility = Visibility.Collapsed;
                    TbPingPointType.Visibility = Visibility.Collapsed;
                    textPingLogicalNumber.Visibility = Visibility.Collapsed;
                    TbPingLogicalNumber.Visibility = Visibility.Collapsed;
                    textPingParameter.Visibility = Visibility.Collapsed;
                    TbPingParameter.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}
