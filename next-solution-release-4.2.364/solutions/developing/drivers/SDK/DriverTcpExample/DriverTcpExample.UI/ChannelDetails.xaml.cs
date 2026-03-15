////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	ChannelDetails.xaml.cs
//
// summary:	Implements the channel details.xaml class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Windows.Controls;

namespace DriverTcpExample.UI
{
    /// <summary>   Interaction logic for ChannelDetails.xaml. </summary>
    public partial class ChannelDetails : UserControl
    {
        bool bLoaded = false;
        /// <summary>   Default constructor. </summary>
        public ChannelDetails()
        {


            InitializeComponent();

            //executed at loaded
            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;
                bLoaded = true;
                //insert TCPChannelSettings setup
                MainStack.Children.Insert(0, new DriverCodeBase.UI.Controls.TCPChannelSettingsUI() { DataContext = DataContext });
            };
        }
    }
}
