using System;
using System.Windows.Controls;
using System.ComponentModel;

namespace DICom.UI
{
    /// <summary>
    /// Interaction logic for StationDetails.xaml
    /// </summary>
    public partial class StationDetails : UserControl
    {
        bool bLoaded = false;
        DIComStationSettings thisStationSettings;
        public StationDetails()
        {
            InitializeComponent();

            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;
                bLoaded = true;
                thisStationSettings = DataContext as DIComStationSettings;
                var baseStation = new DriverCodeBase.UI.Controls.BaseStationSettings() { DataContext = DataContext };
                baseStation.CmbChannel.ItemsSource = thisStationSettings.DriverSettings.ChannelSettings;
                baseStation.TXBStateCommandVariable.Visibility = System.Windows.Visibility.Collapsed;
                baseStation.DKPStateCommandVariable.Visibility = System.Windows.Visibility.Collapsed;
                baseStation.CheckAllowRewritingOfTheSameValue.Visibility = System.Windows.Visibility.Collapsed;
                baseStation.TxBkAllowRewritingOfTheSameValue.Visibility = System.Windows.Visibility.Collapsed;
                baseStation.MaxRetry.Visibility = System.Windows.Visibility.Collapsed;
                baseStation.MaxRetryText.Visibility = System.Windows.Visibility.Collapsed;
                MainStack.Children.Insert(0, baseStation);
                //CmbChannel.ItemsSource = st.DriverSettings.ChannelSettings;
                //DependencyPropertyDescriptor descriptorHostName =
                //   DependencyPropertyDescriptor.FromProperty(TextBox.TextProperty, typeof(TextBox));
                //descriptorHostName.AddValueChanged(HostName, HostName_TextChanged);
                //HostName_TextChanged();
            };
        }
        //private void HostName_TextChanged(object sender, EventArgs e)
        //{
        //    HostName_TextChanged();
        //}

        //private void HostName_TextChanged()
        //{
        //    if (string.IsNullOrEmpty(thisStationSettings.DeviceHostName))
        //    {
        //        BackupHostName.Visibility = System.Windows.Visibility.Hidden;
        //        TxtBackupHostName.Visibility = System.Windows.Visibility.Hidden;
        //    }
        //    else
        //    {
        //        BackupHostName.Visibility = System.Windows.Visibility.Visible;
        //        TxtBackupHostName.Visibility = System.Windows.Visibility.Visible;
        //    }
        //}
    }
}
