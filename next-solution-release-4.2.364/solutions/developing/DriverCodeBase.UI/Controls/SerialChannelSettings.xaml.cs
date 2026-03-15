using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Windows.Controls;

namespace DriverCodeBase.UI.Controls
{
    /// <summary>
    /// Interaction logic for BaseSettings.xaml
    /// </summary>
    public partial class SerialChannelSettings : UserControl
    {
        private enum OS
        {
            Windows,
            Linux
        }

        bool bLoaded = false;
        public DriverCodeBase.UI.Controls.BaseChannelSettings baseDyn;

        public SerialChannelSettings()
        {
            InitializeComponent();
            baseDyn = new DriverCodeBase.UI.Controls.BaseChannelSettings();
            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;
                bLoaded = true;
                baseDyn.DataContext = DataContext;
                SerialStack.Children.Insert(0, baseDyn);

                CmbPort.ItemsSource = SplitCommPortsList(OS.Windows, Properties.Settings.Default.CommPorts);

                CmbPortLinux.ItemsSource = SplitCommPortsList(OS.Linux, Properties.Settings.Default.CommPortsLinux);

                CmbBaud.ItemsSource = new Dictionary<int, string>()
                {
                    { 110, Properties.Resources.BaudRateN1 },
                    { 300, Properties.Resources.BaudRateN2 },
                    { 600, Properties.Resources.BaudRateN3 },
                    { 1200, Properties.Resources.BaudRateN4 },
                    { 2400, Properties.Resources.BaudRateN5 },
                    { 4800, Properties.Resources.BaudRateN6 },
                    { 9600, Properties.Resources.BaudRateN7 },
                    { 14400, Properties.Resources.BaudRateN8 },
                    { 19200, Properties.Resources.BaudRateN9 },
                    { 38400, Properties.Resources.BaudRateN10 },
                    { 56000, Properties.Resources.BaudRateN11 },
                    { 57600, Properties.Resources.BaudRateN12 },
                    { 115200, Properties.Resources.BaudRateN13 },
                    { 128000, Properties.Resources.BaudRateN14 },
                    { 256000, Properties.Resources.BaudRateN15 },
                };

                CmbParity.ItemsSource = new Dictionary<int, string>()
                {
                    { (int)(Parity.None), Properties.Resources.ParityNone },
                    { (int)(Parity.Odd), Properties.Resources.ParityOdd },
                    { (int)(Parity.Even), Properties.Resources.ParityEven },
                    { (int)(Parity.Mark), Properties.Resources.ParityMark },
                    { (int)(Parity.Space), Properties.Resources.ParitySpace }
                };

                CmbStop.ItemsSource = new Dictionary<int, string>()
                {
                    { (int)(StopBits.One), Properties.Resources.StopOne },
                    { (int)(StopBits.Two), Properties.Resources.StopTwo },
                    { (int)(StopBits.OnePointFive), Properties.Resources.StopOneHalf }
                };

                CmbHandShake.ItemsSource = new Dictionary<int, string>()
                {
                    { (int)(Handshake.None), Properties.Resources.HandshakeNone },
                    { (int)(Handshake.XOnXOff), Properties.Resources.HandshakeX },
                    { (int)(Handshake.RequestToSend), Properties.Resources.HandshekeRTS },
                    { (int)(Handshake.RequestToSendXOnXOff), Properties.Resources.HandshakeRTSX }
                };
            };
        }

        private List<string> SplitCommPortsList(OS os,string commPortsList)
        {
            if (string.IsNullOrWhiteSpace(commPortsList))
            {
                switch (os)
                {
                    case OS.Windows:
                        return new List<string>() { "Com1" };
                    case OS.Linux:
                        return new List<string>() { "/dev/ttyS0" };
                    default:
                        return new List<string>();
                }
            }
            else
            {
                return commPortsList.Split(';').ToList();
            }
        }
    }
}
