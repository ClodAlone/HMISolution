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
using System.ComponentModel;

namespace IEC61850.UI
{
    /// <summary>
    /// Interaction logic for StationDetails.xaml
    /// </summary>
    public partial class StationDetails : UserControl
    {
        DependencyPropertyDescriptor descriptor = null;
        bool bLoaded = false;
        public StationDetails()
        {
            InitializeComponent();

            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;
                bLoaded = true;
                var st = DataContext as IEC61850StationSettings;
                var baseStation = new DriverCodeBase.UI.Controls.BaseStationSettings() { DataContext = DataContext };
                baseStation.CmbChannel.ItemsSource = st.DriverSettings.ChannelSettings;
                MainStack.Children.Add(baseStation);
                TxtAuthenticationPassword.Visibility = Visibility.Collapsed;
                stAuthenticationPassword.Visibility = Visibility.Collapsed;
                descriptor = DependencyPropertyDescriptor.FromProperty(CheckBox.IsCheckedProperty, typeof(CheckBox));
                descriptor.AddValueChanged(stAuthenticationEnabled, stAuthenticationEnabled_IsCheckedChanged);
                stAuthenticationEnabled_IsCheckedChanged(stAuthenticationEnabled);
            };

            Unloaded += (o, e) =>
            {
                if (bLoaded)
                {
                    bLoaded = false;
                    descriptor.RemoveValueChanged(stAuthenticationEnabled, stAuthenticationEnabled_IsCheckedChanged);
                }
            };
        }

        private void stAuthenticationEnabled_IsCheckedChanged(object sender = null, EventArgs e = null)
        {
            var cb = (CheckBox)sender;
            if (cb != null)
            {
                if (cb.IsChecked == true)
                {
                    TxtAuthenticationPassword.Visibility = Visibility.Visible;
                    stAuthenticationPassword.Visibility = Visibility.Visible;
                }
                else
                {
                    TxtAuthenticationPassword.Visibility = Visibility.Collapsed;
                    stAuthenticationPassword.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}
