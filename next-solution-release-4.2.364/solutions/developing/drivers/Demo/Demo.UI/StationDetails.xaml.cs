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

namespace Demo.UI
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
                var st = DataContext as DemoStationSettings;
                var baseStation = new DriverCodeBase.UI.Controls.BaseStationSettings() { DataContext = DataContext };
                CmbChannel.ItemsSource = st.DriverSettings.ChannelSettings;
                baseStation.TxBkAllowRewritingOfTheSameValue.Visibility = System.Windows.Visibility.Collapsed;
                baseStation.CheckAllowRewritingOfTheSameValue.Visibility = System.Windows.Visibility.Collapsed;

                if (CmbChannel.SelectedIndex == -1 && CmbChannel.Items.Count > 0)
                {
                    CmbChannel.SelectedIndex = CmbChannel.Items.Count - 1;
                }

                //var st = DataContext as DemoStationSettings;
                //var baseStation = new DriverCodeBase.Controls.BaseStationSettings() 
                //    { DataContext = DataContext };
                //baseStation.CmbChannel.ItemsSource = st.DriverSettings.ChannelSettings;
                //MainStack.Children.Insert(0, baseStation);
            };
        }
    }
}
