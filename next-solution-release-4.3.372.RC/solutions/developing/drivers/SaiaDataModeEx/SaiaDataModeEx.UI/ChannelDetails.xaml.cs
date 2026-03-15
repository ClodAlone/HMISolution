using System;
using System.Windows.Controls;
using System.ComponentModel;
using DriverCodeBaseEx.UI;

namespace SaiaDataMode.UI
{
    /// <summary>
    /// Interaction logic for ChannelDetails.xaml
    /// </summary>
    public partial class ChannelDetails : UserControl
    {
        bool bLoaded = false;
        DriverCodeBaseEx.UI.Controls.UDPChannelSettingsUI baseDynUdp;
        DriverCodeBaseEx.UI.Controls.SerialChannelSettings baseDynSerial;
        public ChannelDetails()
        {


            InitializeComponent();

            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;
                bLoaded = true;

                DriverCodeBaseEx.UI.Controls.BaseChannelSettings b = new DriverCodeBaseEx.UI.Controls.BaseChannelSettings() { DataContext = DataContext };
                b.ChName.Visibility = System.Windows.Visibility.Visible;
                b.TXBCommandVariable.Visibility = System.Windows.Visibility.Collapsed;
                b.DKPCommandVariable.Visibility = System.Windows.Visibility.Collapsed;

                baseDynUdp = new DriverCodeBaseEx.UI.Controls.UDPChannelSettingsUI(false) { DataContext = DataContext };
                baseDynSerial = new DriverCodeBaseEx.UI.Controls.SerialChannelSettings(false) { DataContext = DataContext };
                MainStack.Children.Insert(0, b);
                MainStack.Children.Add(baseDynUdp);
                MainStack.Children.Add(baseDynSerial);

                CmbChannelType.ItemsSource = Enum.GetValues(typeof(ChannelTypes));

                DependencyPropertyDescriptor descriptor =
                   DependencyPropertyDescriptor.FromProperty(ComboBox.TextProperty, typeof(ComboBox));
                descriptor.AddValueChanged(CmbChannelType, CmbChannelType_TextChanged);
                CmbChannelType_TextChanged();
            };
        }

        private void CmbChannelType_TextChanged(object sender = null, EventArgs e = null)
        {
            if (CmbChannelType.Text == ChannelTypes.Serial.ToString())
            {
                baseDynUdp.Visibility = System.Windows.Visibility.Collapsed;
                baseDynSerial.Visibility = System.Windows.Visibility.Visible;
                // show controls associated to "linux enviroment"
                baseDynSerial.CmbPortLinux.Visibility = System.Windows.Visibility.Visible;
                baseDynSerial.TxBPortLinux.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                baseDynUdp.Visibility = System.Windows.Visibility.Visible;
                baseDynUdp.LocalHostName.Visibility = System.Windows.Visibility.Collapsed;
                baseDynUdp.TxBLocalHostName.Visibility = System.Windows.Visibility.Collapsed;
                baseDynUdp.LocalHostPort.Visibility = System.Windows.Visibility.Collapsed;
                baseDynUdp.TxBLocalHostPort.Visibility = System.Windows.Visibility.Collapsed;
                baseDynSerial.Visibility = System.Windows.Visibility.Collapsed;
                // show controls associated to "linux enviroment"
                baseDynSerial.CmbPortLinux.Visibility = System.Windows.Visibility.Collapsed;
                baseDynSerial.TxBPortLinux.Visibility = System.Windows.Visibility.Collapsed;
            }
        }
    }
}
