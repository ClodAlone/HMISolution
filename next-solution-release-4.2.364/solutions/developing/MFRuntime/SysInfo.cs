using System;
using System.Collections;
using System.Threading;

using Microsoft.SPOT;
using Microsoft.SPOT.Input;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Shapes;
using Microsoft.SPOT.Net.NetworkInformation;
using System.Text;

namespace MFRuntime.UI
{
    internal sealed class SysInfo : PresentationWindow
    {
        // This member is the text scroller helper class defined above
        TextScrollViewer _viewer;

        public SysInfo(Program program)
            : base(program)
        {

            // Create a stack panel for the title and scroll view
            StackPanel panel = new StackPanel(Orientation.Vertical);

            Cpu.GlitchFilterTime = new TimeSpan(0, 0, 0, 0, 100); //100 ms
            float systemClock = Cpu.SystemClock / 1000000.0f;
            float slowClock = Cpu.SlowClock / 1000000.0f;
            float glitchFilterTimeMs = Cpu.GlitchFilterTime.Ticks /
            (float)TimeSpan.TicksPerMillisecond;

            NetworkInterface[] nifArray = NetworkInterface.GetAllNetworkInterfaces();
            
            float voltageVolt = Battery.ReadVoltage() / 1000.0f;
            float degreesCelsius = Battery.ReadTemperature() / 10.0f;
            String ScrollableText = SystemInfo.OEMString + "\r\n" +
                                    SystemInfo.Version.ToString() + "\r\n" +
                                    "*** Screen Info ***\r\n" +
                                    "Width: " + SystemMetrics.ScreenWidth.ToString() + "\r\n" +
                                    "Height:" + SystemMetrics.ScreenHeight.ToString() + "\r\n" +
                                    "BitsPerPixel:" + SystemMetrics.ScreenColorDepth.ToString() + "\r\n" +
                                    "\r\n*** CPU Info ***\r\n" +
                                    "System Clock: " + systemClock.ToString("F6") + " MHz\r\n" +
                                    "Slow Clock: " + slowClock.ToString("F6") + " MHz\r\n" +
                                    "Glitch Filter Time: " + glitchFilterTimeMs.ToString("F1") + " ms\r\n" +
                                    "\r\n*** Battery Info ***\r\n" +
                                    "State of Charge: " + Battery.StateOfCharge() + "%\r\n" +
                                    "Is fully charged: " + (Battery.IsFullyCharged() ? "Yes" : "No") + "\r\n" +
                                    "Voltage: " + voltageVolt.ToString("F3") + " Volt\r\n" +
                                    "Temperature: " + degreesCelsius.ToString("F1") + "° Celsius\r\n" + 
                                    "On Charger: " + (Battery.OnCharger() ? "Yes" : "No") + "\r\n" +
                                    "\r\n*** Network ***\r\n";

            foreach (NetworkInterface nif in nifArray)
            {
                if (nif.NetworkInterfaceType == 0)
                    continue;

                String dnscomposedstring = null;
                foreach (string dnsstirng in nif.DnsAddresses)
                {
                    if (dnscomposedstring == null)
                        dnscomposedstring = "";
                    else
                        dnscomposedstring += ", ";
                    dnscomposedstring += dnsstirng;
                }

                String sAdd = Utils.BufferToString(nif.PhysicalAddress);
                String s = "Network Type: " + nif.NetworkInterfaceType.ToString() + "\r\n" +
                           "IP Address: " + nif.IPAddress + "\r\n" +
                           "Subnet Mask: " + nif.SubnetMask + "\r\n" +
                           "Gateway Address: " + nif.GatewayAddress + "\r\n" +
                           "DNS Address: " + dnscomposedstring + "\r\n" +
                           "Is DHCP Enabled: " + (nif.IsDhcpEnabled ? "Yes" : "No") + "\r\n" +
                           "Is Dynamic DNS Enabled: " + (nif.IsDynamicDnsEnabled ? "Yes" : "No") + "\r\n" +
                           "Physical Address: " + sAdd + "\r\n";

                ScrollableText += s;
            }


            HardwareProvider hwp = HardwareProvider.HwProvider;
            ScrollableText += "\r\n***Hardware Provider***\r\n";
            ScrollableText += "GPIO Pin Count: " + hwp.GetPinsCount() + "\r\n";

            Cpu.PinUsage[] pins;
            int nPinCount;
            hwp.GetPinsMap(out pins, out nPinCount);
            int nPinCounter = 1;
            foreach (Cpu.PinUsage cpuPinUsage in pins)
            {
                String s = "GPIO Pin " + nPinCounter.ToString() + ": " + cpuPinUsage.ToString() + "\r\n";
                ScrollableText += s;
                ++nPinCounter;
            }


            ScrollableText += "***";

            // Create the text scroll view and set all of its properties
            _viewer = new TextScrollViewer(ScrollableText,
                                    Program.NinaBFont, Color.Black);
            _viewer.Width = this.Width;
            _viewer.Height = this.Height - 25;  // make room for the title bar
            // _viewer.HScrollHeight = 10;
            // _viewer.VScrollWidth = 10;
            _viewer.HorizontalAlignment = HorizontalAlignment.Left;
            _viewer.VerticalAlignment = VerticalAlignment.Top;

            // Create the title text
            Text title = new Text(Program.NinaBFont, Resources.GetString(Resources.StringResources.SysInfoTextTitle));
            title.ForeColor = Color.White;

            // Add the elements to the stack panel
            panel.Children.Add(title);
            panel.Children.Add(_viewer);

            // Add the stack panel to this window
            this.Child = panel;

            // Set the background color
            this.Background = new SolidColorBrush(ColorUtility.ColorFromRGB(64, 64, 255));
        }

        protected override void OnButtonDown(ButtonEventArgs e)
        {
            switch (e.Button)
            {
                case Button.VK_SELECT:

                    // Remove this window from the Window Manager
                    this.Close();

                    // When <Select> button is pressed, go back to the Home page
                    _program.GoHome();
                    break;

                case Button.VK_UP:
                    // Tell the viewer to scroll up one line
                    _viewer.LineUp();
                    break;

                case Button.VK_DOWN:
                    // Tell the viewer to scroll down one line
                    _viewer.LineDown();
                    break;

                case Button.VK_LEFT:
                    // Tell the viewer to scroll left
                    _viewer.LineLeft();
                    break;

                case Button.VK_RIGHT:
                    // Tell the viewer to scroll right
                    _viewer.LineRight();
                    break;
            }
        }
    }
}
