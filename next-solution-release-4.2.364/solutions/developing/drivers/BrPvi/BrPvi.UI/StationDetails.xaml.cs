using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace BrPvi.UI
{
    /// <summary>
    /// Interaction logic for StationDetails.xaml
    /// </summary>
    public partial class StationDetails : UserControl
    {
        bool bLoaded = false;
        public StationDetails()
        {
            InitializeComponent();
            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;
                bLoaded = true;
                var st = DataContext as BrPviStationSettings;
                var baseStation = new DriverCodeBase.UI.Controls.BaseStationSettings() { DataContext = DataContext };
                baseStation.CmbChannel.ItemsSource = st.DriverSettings.ChannelSettings;

                //hide this parameter (driver force value = 0 on constructor) because other values (>0) create problem on detect station error status
                baseStation.MaxRetryText.Visibility = Visibility.Collapsed;
                baseStation.MaxRetry.Visibility = Visibility.Collapsed;

                MainStack.Children.Insert(0,baseStation);
                CmbPviInterfaceType.ItemsSource = Enum.GetValues(typeof(InterfaceTypes));

                Dictionary<int, string> baudrates = new Dictionary<int, string>();
                baudrates.Add(1, "110");
                baudrates.Add(2, "300");
                baudrates.Add(3, "600");
                baudrates.Add(4, "1200");
                baudrates.Add(5, "2400");
                baudrates.Add(6, "4800");
                baudrates.Add(7, "9600");
                baudrates.Add(8, "14400");
                baudrates.Add(9, "19200");
                baudrates.Add(10, "38400");
                baudrates.Add(11, "56000");
                baudrates.Add(12, "57600");
                baudrates.Add(13, "115200");
                baudrates.Add(14, "128000");
                baudrates.Add(15, "256000");
                CmbPviSerialBaudrate.ItemsSource = baudrates;

                Dictionary<int, string> parity = new Dictionary<int, string>();
                parity.Add(0, "None");
                parity.Add(1, "Odd");
                parity.Add(2, "Even");
                parity.Add(3, "Mark");
                parity.Add(4, "Space");

                CmbPviSerialParity.ItemsSource = parity;

                Dictionary<int, string> flowcontrols = new Dictionary<int, string>();
                flowcontrols.Add(0, "None");
                flowcontrols.Add(1, "RtsOff");
                flowcontrols.Add(2, "RS232Mode");
                flowcontrols.Add(3, "RS422Mode");
                CmbPviSerialFlowControl.ItemsSource = flowcontrols;

                DependencyPropertyDescriptor descriptor =
                   DependencyPropertyDescriptor.FromProperty(ComboBox.TextProperty, typeof(ComboBox));
                descriptor.AddValueChanged(CmbPviInterfaceType, CmbPviInterfaceType_TextChanged);
                CmbPviInterfaceType_TextChanged();
            };
        }
        private void CmbPviInterfaceType_TextChanged(object sender = null, EventArgs e = null)
        {
            CmbPviInterfaceType_TextChanged();
        }

        private void CmbPviInterfaceType_TextChanged()
        {
            if (CmbPviInterfaceType.Text == InterfaceTypes.Ethernet.ToString())
            {
                TextPviSourceStationPort.Visibility = Visibility.Visible;
                PviSourceStationport.Visibility = Visibility.Visible;
                TextPviSourceStationID.Visibility = Visibility.Visible;
                PviSourceStationID.Visibility = Visibility.Visible;
                TextPviDestinationStationIPAddress.Visibility = Visibility.Visible;
                PviDestinationStationIPAddress.Visibility = Visibility.Visible;
                TextPviDestinationStationPort.Visibility = Visibility.Visible;
                PviSourceDestinationport.Visibility = Visibility.Visible;
                TextPviDestinationStationID.Visibility = Visibility.Visible;
                PviDestinationStationID.Visibility = Visibility.Visible;
                TextPviSerialPort.Visibility = Visibility.Collapsed;
                PviSerialPort.Visibility = Visibility.Collapsed;
                TextPviSerialBaudrate.Visibility = Visibility.Collapsed;
                CmbPviSerialBaudrate.Visibility = Visibility.Collapsed;
                TextPviSerialParity.Visibility = Visibility.Collapsed;
                CmbPviSerialParity.Visibility = Visibility.Collapsed;
                TextPviSerialFlowControl.Visibility = Visibility.Collapsed;
                CmbPviSerialFlowControl.Visibility = Visibility.Collapsed;
            }
            else
            {
                TextPviSourceStationPort.Visibility = Visibility.Collapsed;
                PviSourceStationport.Visibility = Visibility.Collapsed;
                TextPviSourceStationID.Visibility = Visibility.Collapsed;
                PviSourceStationID.Visibility = Visibility.Collapsed;
                TextPviDestinationStationIPAddress.Visibility = Visibility.Collapsed;
                PviDestinationStationIPAddress.Visibility = Visibility.Collapsed;
                TextPviDestinationStationPort.Visibility = Visibility.Collapsed;
                PviSourceDestinationport.Visibility = Visibility.Collapsed;
                TextPviDestinationStationID.Visibility = Visibility.Collapsed;
                PviDestinationStationID.Visibility = Visibility.Collapsed;
                TextPviSerialPort.Visibility = Visibility.Visible;
                PviSerialPort.Visibility = Visibility.Visible;
                TextPviSerialBaudrate.Visibility = Visibility.Visible;
                CmbPviSerialBaudrate.Visibility = Visibility.Visible;
                TextPviSerialParity.Visibility = Visibility.Visible;
                CmbPviSerialParity.Visibility = Visibility.Visible;
                TextPviSerialFlowControl.Visibility = Visibility.Visible;
                CmbPviSerialFlowControl.Visibility = Visibility.Visible;
            }
        }
    }
}
