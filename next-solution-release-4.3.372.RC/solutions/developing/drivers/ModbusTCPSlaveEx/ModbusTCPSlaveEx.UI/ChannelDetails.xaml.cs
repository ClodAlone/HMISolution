using System.Windows.Controls;

namespace ModbusTCPSlave.UI
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
                DriverCodeBaseEx.UI.Controls.TCPChannelSettingsUI baseDyn = new DriverCodeBaseEx.UI.Controls.TCPChannelSettingsUI() { DataContext = DataContext };
                baseDyn.baseDyn.WaitTimeText.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.baseDyn.WaitTime.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.baseDyn.PollNotUseText.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.baseDyn.PollNotUse.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.baseDyn.PollErrorText.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.baseDyn.PollError.Visibility = System.Windows.Visibility.Collapsed;                
                baseDyn.baseDyn.TXBStateVariable.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.baseDyn.DKPStateVariable.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.baseDyn.TXBCommandVariable.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.baseDyn.DKPCommandVariable.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.TxBHostName.Text = Properties.Resources.LocalHostName;
                MainStack.Children.Insert(0, baseDyn);                
            };
        }
    }
}
