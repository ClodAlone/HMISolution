using System.Collections.Generic;
using System.Windows.Controls;

namespace FanucCNC.UI
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
                var st = DataContext as FanucCNCStationSettings;
                var baseStation = new DriverCodeBase.UI.Controls.BaseStationSettings() { DataContext = DataContext };
                baseStation.CmbChannel.ItemsSource = st.DriverSettings.ChannelSettings;
                MainStack.Children.Add(baseStation);                
            };
        }
    }
}
