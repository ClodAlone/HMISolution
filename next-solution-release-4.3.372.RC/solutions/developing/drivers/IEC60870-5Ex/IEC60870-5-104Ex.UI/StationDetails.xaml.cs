////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	StationDetails.xaml.cs
//
// summary:	Implements the station details.xaml class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Windows.Controls;

namespace IEC60870_5_104.UI
{
    /// <summary>   Interaction logic for StationDetails.xaml. </summary>
    public partial class StationDetails : UserControl
    {
        bool bLoaded = false;
        /// <summary>   Default constructor. </summary>
        public StationDetails()
        {
            InitializeComponent();

            //executed at loaded
            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;
                bLoaded = true;
                var st = DataContext as IEC60870_5_104StationSettings;
                var baseStation = new DriverCodeBaseEx.UI.Controls.BaseStationSettings() { DataContext = DataContext };
                //insert Channel in ComboBox CmbChannel
                baseStation.CmbChannel.ItemsSource = st.DriverSettings.ChannelSettings;
                MainStack.Children.Insert(0, baseStation);
            };
        }
    }
}
