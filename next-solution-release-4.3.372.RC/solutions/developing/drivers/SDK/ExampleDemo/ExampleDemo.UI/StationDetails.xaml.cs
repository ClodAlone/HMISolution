
using System.Windows.Controls;

namespace ExampleDemo.UI
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
                var st = DataContext as ExampleDemoStationSettings;
                CmbChannel.ItemsSource = st.DriverSettings.ChannelSettings;

                if (CmbChannel.SelectedIndex == -1 && CmbChannel.Items.Count > 0)
                {
                    CmbChannel.SelectedIndex = CmbChannel.Items.Count - 1;
                }
            };
        }
    }
}
