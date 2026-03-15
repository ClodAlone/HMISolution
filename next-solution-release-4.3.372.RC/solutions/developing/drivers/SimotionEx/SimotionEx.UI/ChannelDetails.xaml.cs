using System.Windows.Controls;

namespace Simotion.UI
{
    /// <summary>
    /// Interaction logic for ChannelDetails.xaml
    /// </summary>
    public partial class ChannelDetails : UserControl
    {
        bool bLoaded = false;
        public DriverCodeBaseEx.UI.Controls.BaseChannelSettings baseDyn;
        public ChannelDetails()
        {
            InitializeComponent();
            baseDyn = new DriverCodeBaseEx.UI.Controls.BaseChannelSettings();
            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;
                bLoaded = true;
                baseDyn.DataContext = DataContext;
                baseDyn.TXBCommandVariable.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.DKPCommandVariable.Visibility = System.Windows.Visibility.Collapsed;
                MainStack.Children.Insert(0, baseDyn);
            };
        }
    }
}
