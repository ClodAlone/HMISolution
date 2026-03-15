////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	StationDetails.xaml.cs
//
// summary:	Implements the station details.xaml class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Windows.Controls;

namespace DriverTcpExample.UI
{
    /// <summary>   Interaction logic for StationDetails.xaml. </summary>
    public partial class StationDetails : UserControl
    {
        /// <summary>   Default constructor. </summary>
        public StationDetails()
        {
            bool bLoaded = false;

            InitializeComponent();

            //executed at loaded
            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;
                bLoaded = true;
                var st = DataContext as DriverTcpExampleStationSettings;
                var baseStation = new DriverCodeBase.UI.Controls.BaseStationSettings() { DataContext = DataContext };
                //insert Channel in ComboBox CmbChannel
                baseStation.CmbChannel.ItemsSource = st.DriverSettings.ChannelSettings;
                MainStack.Children.Add(baseStation);
            };
        }
    }
}
