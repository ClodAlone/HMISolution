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
using System.Data.SqlClient;
using System.Data;
using Utilities;

namespace SQLDriver.UI
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
                var st = DataContext as SQLDriverStationSettings;
                var baseStation = new DriverCodeBaseEx.UI.Controls.BaseStationSettings() { DataContext = DataContext };
                baseStation.CmbChannel.ItemsSource = st.DriverSettings.ChannelSettings;
                baseStation.DKPStateVariable.Visibility = System.Windows.Visibility.Visible;
                baseStation.TXBStateVariable.Visibility = System.Windows.Visibility.Visible;
                baseStation.DKPCommandVariable.Visibility = System.Windows.Visibility.Visible;
                baseStation.TXBCommandVariable.Visibility = System.Windows.Visibility.Visible;
                //baseStation.MaxRetry.Visibility = System.Windows.Visibility.Collapsed;
                //baseStation.MaxRetryText.Visibility = System.Windows.Visibility.Collapsed;
                baseStation.CmbChannel.ItemsSource = st.DriverSettings.ChannelSettings;
                MainStack.Children.Insert(0,baseStation);
            };

        }

    }
}
