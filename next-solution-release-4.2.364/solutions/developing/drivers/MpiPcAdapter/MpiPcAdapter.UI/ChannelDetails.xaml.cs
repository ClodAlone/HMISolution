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

namespace MpiPcAdapter.UI
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
                   //MainStack.Children.Insert(0, new DriverCodeBase.UI.Controls.SerialChannelSettings() { DataContext = DataContext});
                   var baseControl = new DriverCodeBase.UI.Controls.SerialChannelSettings() { DataContext = DataContext };
                   // show controls associated to "linux enviroment"
                   baseControl.CmbPortLinux.Visibility = Visibility.Visible;
                   baseControl.TxBPortLinux.Visibility = Visibility.Visible;

                   MainStack.Children.Insert(0, baseControl);

                   CmbMpiNetworkBitRate.ItemsSource = Enum.GetValues(typeof(MpiNetworkBitRates));

               };
        }
    }
}
