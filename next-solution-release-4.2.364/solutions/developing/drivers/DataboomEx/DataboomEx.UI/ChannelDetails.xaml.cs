using System.Windows.Controls;

namespace Databoom.UI
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
                DriverCodeBaseEx.UI.Controls.BaseChannelSettings baseDyn = new DriverCodeBaseEx.UI.Controls.BaseChannelSettings() { DataContext = DataContext };                
                baseDyn.PollNotUse.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.PollNotUseText.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.PollError.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.PollErrorText.Visibility = System.Windows.Visibility.Collapsed;
                MainStack.Children.Insert(0, baseDyn);
            };
        }
    }
}
