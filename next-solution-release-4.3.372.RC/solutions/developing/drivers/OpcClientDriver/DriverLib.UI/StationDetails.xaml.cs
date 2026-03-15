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

namespace OpcClientDriver.UI
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
                var st = DataContext as OpcClientDriverStationSettings;
                var baseStation = new DriverCodeBase.UI.Controls.BaseStationSettings() { DataContext = DataContext };
                baseStation.CmbChannel.ItemsSource = st.DriverSettings.ChannelSettings;
                baseStation.MaxRetry.Visibility = Visibility.Collapsed;
                baseStation.MaxRetryErr.Visibility = Visibility.Collapsed;
                baseStation.MaxRetryText.Visibility = Visibility.Collapsed;
                baseStation.TxBkAllowRewritingOfTheSameValue.Visibility = Visibility.Collapsed;
                baseStation.CheckAllowRewritingOfTheSameValue.Visibility = Visibility.Collapsed;
                MainStack.Children.Add(baseStation);
            };
        }
    }
}
