using System.Collections.Generic;
using System.Windows.Controls;

namespace Fatek.UI
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
                var st = DataContext as FatekStationSettings;
                var baseStation = new DriverCodeBase.UI.Controls.BaseStationSettings() { DataContext = DataContext };
                baseStation.CmbChannel.ItemsSource = st.DriverSettings.ChannelSettings;
                MainStack.Children.Add(baseStation);
                //Dictionary<int, string> addressTypes = new Dictionary<int, string>();
                //addressTypes.Add((int)AddressTypes.ZeroBased, Properties.Resources.AddressTypes_ZeroBased);
                //addressTypes.Add((int)AddressTypes.OneBased, Properties.Resources.AddressTypes_OneBased);
                //CmbAddressType.ItemsSource = addressTypes;
                //CmbChannel.ItemsSource = st.DriverSettings.ChannelSettings;
            };
        }
    }
}
