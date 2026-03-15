using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ModBus.UI
{
    /// <summary>
    /// Interaction logic for ChannelDetails.xaml
    /// </summary>
    public partial class ChannelDetails : UserControl
    {
        bool bLoaded = false;
        public ChannelDetails()
        {
            

            InitializeComponent();

            Loaded += (o, e) =>
               {
                   if (bLoaded)
                       return;
                   bLoaded = true;

                   var baseControl = new DriverCodeBaseEx.UI.Controls.SerialChannelSettings() { DataContext = DataContext };
                   // show controls associated to "linux enviroment"
                   baseControl.CmbPortLinux.Visibility = Visibility.Visible;
                   baseControl.TxBPortLinux.Visibility = Visibility.Visible;
                   baseControl.baseDyn.TXBCommandVariable.Visibility = System.Windows.Visibility.Collapsed;
                   baseControl.baseDyn.DKPCommandVariable.Visibility = System.Windows.Visibility.Collapsed;

                   MainStack.Children.Insert(0, baseControl);
                   //Dictionary<int, string> l = new Dictionary<int, string>();
                   //l.Add(1, "Com1");
                   //l.Add(2, "Com2");
                   //l.Add(3, "Com3");
                   //l.Add(4, "Com4");
                   //l.Add(5, "Com5");
                   //l.Add(6, "Com6");
                   //l.Add(7, "Com7");
                   //l.Add(8, "Com8");
                   //l.Add(9, "Com9");
                   //l.Add(10, "Com10");
                   //CmbPort.ItemsSource = l;

                   //Dictionary<int, string> m = new Dictionary<int, string>();
                   //m.Add(1, "110");
                   //m.Add(2, "300");
                   //m.Add(3, "600");
                   //m.Add(4, "1200");
                   //m.Add(5, "2400");
                   //m.Add(6, "4800");
                   //m.Add(7, "9600");
                   //m.Add(8, "14400");
                   //m.Add(9, "19200");
                   //m.Add(10, "38400");
                   //m.Add(11, "56000");
                   //m.Add(12, "57600");
                   //m.Add(13, "115200");
                   //m.Add(14, "128000");
                   //m.Add(15, "256000");
                   //CmbBaud.ItemsSource = m;

                   //Dictionary<int, string> n = new Dictionary<int, string>();
                   //n.Add(0, "None");
                   //n.Add(1, "Odd");
                   //n.Add(2, "Even");
                   //n.Add(3, "Mark");
                   //n.Add(4, "Space");
                   //CmbParity.ItemsSource = n;
                   

                   //Dictionary<int, string> q = new Dictionary<int, string>();
                   //q.Add(0, "None");
                   //q.Add(1, "One");
                   //q.Add(2, "Two");
                   //q.Add(3, "One and a half");
                   //CmbStop.ItemsSource = q;

                   //Dictionary<int, string> p = new Dictionary<int, string>();
                   //p.Add(0, "None");
                   //p.Add(1, "XOnXOff");
                   //p.Add(2, "RequestToSend");
                   //p.Add(3, "RequestToSendXOnXOff");
                   //CmbHandShake.ItemsSource = p;

                   Dictionary<int, string> r = new Dictionary<int, string>();
                   r.Add(0, "RTU");
                   r.Add(1, "ASCII");
                   CmbFrame.ItemsSource = r;
               };
        }
    }
}
