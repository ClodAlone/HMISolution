using System.Windows.Controls;

namespace S7TCP.UI
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
                DriverCodeBaseEx.UI.Controls.TCPChannelSettingsUI baseDyn = new DriverCodeBaseEx.UI.Controls.TCPChannelSettingsUI() { DataContext = DataContext };
                MainStack.Children.Insert(0, baseDyn);
            };
        }
    }
}
