using System.Windows.Controls;

namespace ModbusTCP.UI
{
    /// <summary>
    /// Interaction logic for ChannelDetails.xaml
    /// </summary>
    public partial class ChannelDetails : UserControl
    {
        public ChannelDetails()
        {
            bool bLoaded = false;

            InitializeComponent();

            Loaded += (o, e) =>
            {
                if (bLoaded)
                   return;
                bLoaded = true;
                MainStack.Children.Insert(0, new DriverCodeBase.UI.Controls.TCPChannelSettingsUI() { DataContext = DataContext });
            };
        }
    }
}
