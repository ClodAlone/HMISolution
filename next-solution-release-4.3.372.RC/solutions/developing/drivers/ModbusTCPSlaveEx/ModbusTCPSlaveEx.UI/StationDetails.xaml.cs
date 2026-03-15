using System;
using System.Windows.Controls;
using System.ComponentModel;

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
                var baseStation = new DriverCodeBaseEx.UI.Controls.BaseStationSettings() { DataContext = DataContext };
                baseStation.CmbChannel.ItemsSource = thisStationSettings.DriverSettings.ChannelSettings;
                baseStation.MaxRetryText.Visibility = System.Windows.Visibility.Collapsed;
                baseStation.MaxRetry.Visibility = System.Windows.Visibility.Collapsed;
                baseStation.TxBkAllowRewritingOfTheSameValue.Visibility = System.Windows.Visibility.Collapsed;
                baseStation.CheckAllowRewritingOfTheSameValue.Visibility = System.Windows.Visibility.Collapsed;
                baseStation.TXBStateVariable.Visibility = System.Windows.Visibility.Collapsed;
                baseStation.DKPStateVariable.Visibility = System.Windows.Visibility.Collapsed;
                baseStation.TXBCommandVariable.Visibility = System.Windows.Visibility.Collapsed;
                baseStation.DKPCommandVariable.Visibility = System.Windows.Visibility.Collapsed;
                MainStack.Children.Insert(0, baseStation);
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
                BackupHostName.Visibility = System.Windows.Visibility.Collapsed;
                TxtBackupHostName.Visibility = System.Windows.Visibility.Collapsed;
                ChID.Visibility = System.Windows.Visibility.Visible;
                TxtChID.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                BackupHostName.Visibility = System.Windows.Visibility.Visible;
                TxtBackupHostName.Visibility = System.Windows.Visibility.Visible;
                ChID.Visibility = System.Windows.Visibility.Collapsed;
                TxtChID.Visibility = System.Windows.Visibility.Collapsed;
            }
        }
    }
}
