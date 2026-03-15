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

namespace LacbusPC.UI
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
                var st = DataContext as LacbusPCStationSettings;
                var baseStation = new DriverCodeBase.UI.Controls.BaseStationSettings() { DataContext = DataContext };
                baseStation.CmbChannel.ItemsSource = st.DriverSettings.ChannelSettings;
                MainStack.Children.Add(baseStation);
                CmbProtocolType.ItemsSource = Enum.GetValues(typeof(LacbusPcUnderlyingProtocols));
                //CommunicationDurationText.Visibility = Visibility.Collapsed;
                //CommunicationDuration.Visibility = Visibility.Collapsed;
                AutomaticPollRTU_IsCheckedChanged();
                DependencyPropertyDescriptor descriptor =
                   DependencyPropertyDescriptor.FromProperty(CheckBox.IsCheckedProperty, typeof(CheckBox));
                descriptor.AddValueChanged(AutomaticPollRTU, AutomaticPollRTU_IsCheckedChanged);
            };
        }

        private void AutomaticPollRTU_IsCheckedChanged(object sender, EventArgs e)
        {
            AutomaticPollRTU_IsCheckedChanged();
        }

        private void AutomaticPollRTU_IsCheckedChanged()
        {
            if(AutomaticPollRTU.IsChecked == true)
            {
                AutomaticPollRTUFrequencyText.Visibility = Visibility.Visible;
                AutomaticPollRTUFrequency.Visibility = Visibility.Visible;
            }
            else
            {
                AutomaticPollRTUFrequencyText.Visibility = Visibility.Collapsed;
                AutomaticPollRTUFrequency.Visibility = Visibility.Collapsed;
            }
        }
    }
}
