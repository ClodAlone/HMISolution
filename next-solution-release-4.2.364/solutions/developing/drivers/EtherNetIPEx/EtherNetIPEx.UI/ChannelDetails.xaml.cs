using System.Windows.Controls;

namespace EtherNetIP.UI
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
                //insert TCPChannelSettings setup
                MainStack.Children.Insert(0, new DriverCodeBaseEx.UI.Controls.TCPChannelSettingsUI() { DataContext = DataContext });
            };
        }
    }
}
