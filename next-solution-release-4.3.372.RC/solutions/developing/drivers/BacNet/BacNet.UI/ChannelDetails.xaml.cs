////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	ChannelDetails.xaml.cs
//
// summary:	Implements the channel details.xaml class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Windows.Controls;

namespace BACnet.UI
{
    /// <summary>   Interaction logic for ChannelDetails.xaml. </summary>
    public partial class ChannelDetails : UserControl
    {
        /// <summary>   Default constructor. </summary>
        public ChannelDetails()
        {


            bool bLoaded = false;
            InitializeComponent();

            //executed at loaded
            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;
                bLoaded = true;
                //insert UDPChannelSettings setup
                DriverCodeBase.UI.Controls.UDPChannelSettingsUI baseDynUdp = new DriverCodeBase.UI.Controls.UDPChannelSettingsUI() { DataContext = DataContext };
                baseDynUdp.TxBHostName.Visibility = System.Windows.Visibility.Collapsed;
                baseDynUdp.HostName.Visibility = System.Windows.Visibility.Collapsed;
                baseDynUdp.TxBHostPort.Visibility = System.Windows.Visibility.Collapsed;
                baseDynUdp.HostPort.Visibility = System.Windows.Visibility.Collapsed;
                MainStack.Children.Insert(0, baseDynUdp);
            };
        }
    }
}
