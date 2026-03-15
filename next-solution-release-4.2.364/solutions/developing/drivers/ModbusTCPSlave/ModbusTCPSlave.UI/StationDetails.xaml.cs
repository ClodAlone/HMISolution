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
using System.ComponentModel;
using ModbusTCPSlave;

namespace ModbusTCPSlave.UI
{
    /// <summary>
    /// Interaction logic for StationDetails.xaml
    /// </summary>
    public partial class StationDetails : UserControl
    {
        bool bLoaded = false;
        ModbusTCPSlaveStationSettings thisStationSettings;
        public StationDetails()
        {
            InitializeComponent();

            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;
                bLoaded = true;
                thisStationSettings = DataContext as ModbusTCPSlaveStationSettings;
                var baseStation = new DriverCodeBase.UI.Controls.BaseStationSettings() { DataContext = DataContext };
                baseStation.CmbChannel.ItemsSource = thisStationSettings.DriverSettings.ChannelSettings;
                baseStation.TXBStateCommandVariable.Visibility = System.Windows.Visibility.Collapsed;
                baseStation.DKPStateCommandVariable.Visibility = System.Windows.Visibility.Collapsed;
                MainStack.Children.Add(baseStation);
                //CmbChannel.ItemsSource = st.DriverSettings.ChannelSettings;
                DependencyPropertyDescriptor descriptorHostName =
                   DependencyPropertyDescriptor.FromProperty(TextBox.TextProperty, typeof(TextBox));
                descriptorHostName.AddValueChanged(HostName, HostName_TextChanged);
                HostName_TextChanged();
            };
        }
        private void HostName_TextChanged(object sender, EventArgs e)
        {
            HostName_TextChanged();
        }

        private void HostName_TextChanged()
        {
            if (string.IsNullOrEmpty(thisStationSettings.DeviceHostName))
            {
                BackupHostName.Visibility = System.Windows.Visibility.Hidden;
                TxtBackupHostName.Visibility = System.Windows.Visibility.Hidden;
                ChID.Visibility = System.Windows.Visibility.Visible;
                TxtChID.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                BackupHostName.Visibility = System.Windows.Visibility.Visible;
                TxtBackupHostName.Visibility = System.Windows.Visibility.Visible;
                ChID.Visibility = System.Windows.Visibility.Hidden;
                TxtChID.Visibility = System.Windows.Visibility.Hidden;
            }

        }

    }
}
