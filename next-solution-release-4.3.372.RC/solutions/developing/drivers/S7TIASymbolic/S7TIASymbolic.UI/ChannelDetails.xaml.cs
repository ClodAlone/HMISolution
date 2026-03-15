using System.Windows.Controls;

namespace S7TIASymbolic.UI
{
    /// <summary>
    /// Interaction logic for ChannelDetails.xaml
    /// </summary>
    public partial class ChannelDetails : UserControl
    {
        bool bLoaded = false;
        public DriverCodeBase.UI.Controls.BaseChannelSettings baseDyn;
        public ChannelDetails()
        {
            InitializeComponent();
            baseDyn = new DriverCodeBase.UI.Controls.BaseChannelSettings();
            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;
                bLoaded = true;
                baseDyn.DataContext = DataContext;
                MainStack.Children.Insert(0, baseDyn);
            };
        }
    }
}
