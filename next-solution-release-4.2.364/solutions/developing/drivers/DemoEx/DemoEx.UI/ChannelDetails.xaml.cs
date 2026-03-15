using System.Windows.Controls;

namespace Demo.UI
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
                baseDyn.Timeout.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.TimeoutText.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.WaitTime.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.WaitTimeText.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.PollNotUse.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.PollNotUseText.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.PollError.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.PollErrorText.Visibility = System.Windows.Visibility.Collapsed;
                MainStack.Children.Insert(0, baseDyn);
            };
        }
    }
}
