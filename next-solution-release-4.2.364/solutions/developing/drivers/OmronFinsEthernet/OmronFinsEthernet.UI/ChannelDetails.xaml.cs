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

namespace OmronFinsEthernet.UI
{
    /// <summary>
    /// Interaction logic for ChannelDetails.xaml
    /// </summary>
    public partial class ChannelDetails : UserControl
    {
        bool bLoaded = false;
        public ChannelDetails()
        {
            InitializeComponent();
            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;
                bLoaded = true;
                //insert UDPChannelSettings setup
                DriverCodeBase.UI.Controls.UDPChannelSettingsUI baseSettings = new DriverCodeBase.UI.Controls.UDPChannelSettingsUI() { DataContext = DataContext };
                baseSettings.TxBLocalHostName.Visibility = System.Windows.Visibility.Collapsed;
                baseSettings.LocalHostName.Visibility = System.Windows.Visibility.Collapsed;
                baseSettings.TxBLocalHostPort.Visibility = System.Windows.Visibility.Collapsed;
                baseSettings.LocalHostPort.Visibility = System.Windows.Visibility.Collapsed;
                MainStack.Children.Insert(0, baseSettings);
            };
        }
    }
}
