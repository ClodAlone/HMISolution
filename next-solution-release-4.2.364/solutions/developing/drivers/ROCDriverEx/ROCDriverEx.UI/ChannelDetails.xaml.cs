using System;
using System.Windows.Controls;
using System.ComponentModel;

namespace ROCDriver.UI
{
    /// <summary>
    /// Interaction logic for ChannelDetails.xaml
    /// </summary>
    public partial class ChannelDetails : UserControl
    {
        bool bLoaded = false;
        DriverCodeBaseEx.UI.Controls.TCPChannelSettingsUI baseDynTcp;
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

                baseDynTcp = new DriverCodeBaseEx.UI.Controls.TCPChannelSettingsUI(false) { DataContext = DataContext };                
                baseDynSerial = new DriverCodeBaseEx.UI.Controls.SerialChannelSettings(false) { DataContext = DataContext };
                MainStack.Children.Insert(0, b);
                MainStack.Children.Add(baseDynTcp);
                MainStack.Children.Add(baseDynSerial);

                CmbChannelType.ItemsSource = Enum.GetValues(typeof(ChannelTypes));

                DependencyPropertyDescriptor descriptor =
                   DependencyPropertyDescriptor.FromProperty(ComboBox.TextProperty, typeof(ComboBox));
                descriptor.AddValueChanged(CmbChannelType, CmbChannelType_TextChanged);
                CmbChannelType_TextChanged();
            };
        }

        private void CmbChannelType_TextChanged(object sender, EventArgs e)
        {
            CmbChannelType_TextChanged();
        }
        private void CmbChannelType_TextChanged()
        {
            if (CmbChannelType.Text == ChannelTypes.Serial.ToString())
            {
                baseDynTcp.Visibility = System.Windows.Visibility.Collapsed;
                baseDynSerial.Visibility = System.Windows.Visibility.Visible;
                baseDynSerial.CmbPortLinux.Visibility = System.Windows.Visibility.Visible;
                baseDynSerial.TxBPortLinux.Visibility = System.Windows.Visibility.Visible;

                //TxBUSBconnection.Visibility = System.Windows.Visibility.Visible;
                //CkUSBconnection.Visibility = System.Windows.Visibility.Visible;
                TxBUSBconnection.Visibility = System.Windows.Visibility.Collapsed;
                CkUSBconnection.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {                
                baseDynSerial.Visibility = System.Windows.Visibility.Collapsed;
                baseDynSerial.CmbPortLinux.Visibility = System.Windows.Visibility.Collapsed;
                baseDynSerial.TxBPortLinux.Visibility = System.Windows.Visibility.Collapsed;
                baseDynTcp.Visibility = System.Windows.Visibility.Visible;

                TxBUSBconnection.Visibility = System.Windows.Visibility.Collapsed;
                CkUSBconnection.Visibility = System.Windows.Visibility.Collapsed;
            }
        }
    }
}
