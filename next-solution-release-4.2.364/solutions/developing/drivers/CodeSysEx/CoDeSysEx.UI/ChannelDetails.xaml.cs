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
                MainStack.Children.Insert(0, new DriverCodeBaseEx.UI.Controls.BaseChannelSettings() { DataContext = DataContext });
                
                CmbConnectionType.ItemsSource = Enum.GetValues(typeof(CoDeSysChannelSettings.CONNECTION));

            };
        }
       
   
        private void CmbConnectionType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch ((CoDeSysChannelSettings.CONNECTION)CmbConnectionType.SelectedValue) 
            {
                case CoDeSysChannelSettings.CONNECTION.DIRECT:
                    lblDeviceName.Visibility = System.Windows.Visibility.Collapsed;
                    TxTDeviceName.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                case CoDeSysChannelSettings.CONNECTION.GATEWAY:
                    lblDeviceName.Visibility = System.Windows.Visibility.Visible;
                    TxTDeviceName.Visibility = System.Windows.Visibility.Visible;
                    TxTDeviceName.Text = string.Empty;
                    break;
            }
        }
    }
}
