using System;
using System.Windows.Controls;
using System.ComponentModel;

namespace CoDeSys.UI
{
    /// <summary>
    /// Interaction logic for ChannelDetails.xaml
    /// </summary>
    public partial class ChannelDetails : UserControl
    {
        bool alreadyLoaded = false;
        public ChannelDetails()
        {
            InitializeComponent();
            Loaded += (o, e) =>
            {
                if (alreadyLoaded)
                    return;
                alreadyLoaded = true;
                MainStack.Children.Insert(0, new DriverCodeBase.UI.Controls.BaseChannelSettings() { DataContext = DataContext });
                
                CmbConnectionType.ItemsSource = Enum.GetValues(typeof(CoDeSysChannelSettings.CONNECTION));

            };
        }
       
   
        private void CmbConnectionType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((CoDeSysChannelSettings.CONNECTION)CmbConnectionType.SelectedValue) == CoDeSysChannelSettings.CONNECTION.DIRECT)
            {
                lblDeviceName.Visibility = System.Windows.Visibility.Collapsed;
                TxTDeviceName.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                lblDeviceName.Visibility = System.Windows.Visibility.Visible;
                TxTDeviceName.Visibility = System.Windows.Visibility.Visible;
            }
        }
    }
}
