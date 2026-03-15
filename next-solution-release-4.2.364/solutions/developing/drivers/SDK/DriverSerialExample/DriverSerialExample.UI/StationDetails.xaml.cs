////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	StationDetails.xaml.cs
//
// summary:	Implements the station details.xaml class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Windows.Controls;

namespace DriverSerialExample.UI
{
    /// <summary>   Interaction logic for StationDetails.xaml. </summary>
    public partial class StationDetails : UserControl
    {
        /// <summary>   Default constructor. </summary>
        public StationDetails()
        {
            bool bLoaded = false;
            InitializeComponent();

            Loaded += (o, e) =>
               {
                   if (bLoaded)
                       return;
                   bLoaded = true;
                   var st = DataContext as DriverSerialExampleStationSettings;
                   var baseStation = new DriverCodeBase.UI.Controls.BaseStationSettings() { DataContext = DataContext };
                   //insert Channe in ComboBox CmbChannel
                   baseStation.CmbChannel.ItemsSource = st.DriverSettings.ChannelSettings;
                   MainStack.Children.Add(baseStation);

               };
        }
    }
}
