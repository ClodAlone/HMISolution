using System.Windows.Controls;

namespace MTConnect.UI
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
                var st = DataContext as MTConnectStationSettings;
                var baseStation = new DriverCodeBaseEx.UI.Controls.BaseStationSettings() { DataContext = DataContext };
                baseStation.CmbChannel.ItemsSource = st.DriverSettings.ChannelSettings;
                baseStation.MaxRetry.Visibility = System.Windows.Visibility.Collapsed;
                baseStation.MaxRetryText.Visibility = System.Windows.Visibility.Collapsed;
                baseStation.TxBkAllowRewritingOfTheSameValue.Visibility = System.Windows.Visibility.Collapsed;
                baseStation.CheckAllowRewritingOfTheSameValue.Visibility = System.Windows.Visibility.Collapsed;
                //baseStation.CheckDisableQualityUpdateText.Visibility = System.Windows.Visibility.Collapsed;
                //baseStation.CheckDisableQualityUpdate.Visibility = System.Windows.Visibility.Collapsed;
                MainStack.Children.Insert(0,baseStation);
            };
        }
    }
}
