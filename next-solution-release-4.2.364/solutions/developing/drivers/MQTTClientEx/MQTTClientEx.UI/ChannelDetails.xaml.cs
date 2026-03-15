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
using uPLibrary.Networking.M2Mqtt;

namespace MQTTClient.UI
{
    /// <summary>
    /// Interaction logic for ChannelDetails.xaml
    /// </summary>
    public partial class ChannelDetails : UserControl
    {
        DependencyPropertyDescriptor descriptor = null;
        bool alreadyLoaded = false;

        public ChannelDetails()
        {

            InitializeComponent();

            Loaded += (o, e) =>
            {
                if (alreadyLoaded)
                    return;
                alreadyLoaded = true;
                DriverCodeBaseEx.UI.Controls.BaseChannelSettings baseDyn = new DriverCodeBaseEx.UI.Controls.BaseChannelSettings() { DataContext = DataContext };
                baseDyn.PollNotUse.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.PollNotUseText.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.PollError.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.PollErrorText.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.WaitTime.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.WaitTimeText.Visibility = System.Windows.Visibility.Collapsed;
                MainStack.Children.Insert(0, baseDyn);
                CmbProtocolForSecureConnection.ItemsSource = Enum.GetValues(typeof(MqttSslProtocols));
                TxtMqttCACertificateFile.Visibility = Visibility.Collapsed;
                MqttCACertificateFile.Visibility = Visibility.Collapsed;
                //textUseAzureConnection.Visibility = Visibility.Collapsed;
                //checkUseAzureConnection.Visibility = Visibility.Collapsed;
                descriptor = DependencyPropertyDescriptor.FromProperty(CheckBox.IsCheckedProperty, typeof(CheckBox));
                descriptor.AddValueChanged(checkUseSecureConnection, checkUseSecureConnection_IsCheckedChanged);
                checkUseSecureConnection_IsCheckedChanged(checkUseSecureConnection);
            };

            Unloaded += (o, e) =>
            {
                descriptor.RemoveValueChanged(checkUseSecureConnection, checkUseSecureConnection_IsCheckedChanged);                
            };
        }

        private void checkUseSecureConnection_IsCheckedChanged(object sender = null, EventArgs e = null)
        {
            var cb = (CheckBox)sender;
            if (cb != null)
            {
                if (cb.IsChecked == true)
                {
                    TxtProtocolSecureConnection.Visibility = Visibility.Visible;
                    CmbProtocolForSecureConnection.Visibility = Visibility.Visible;
                    CmbProtocolForSecureConnection.SelectedItem = MqttSslProtocols.TLSv1_2;
                    TxtMqttCACertificateFile.Visibility = Visibility.Collapsed;
                    MqttCACertificateFile.Visibility = Visibility.Collapsed;
                    TxtMqttClientCertificateFile.Visibility = Visibility.Visible;
                    MqttClientCertificateFile.Visibility = Visibility.Visible;
                    //textUseAzureConnection.Visibility = Visibility.Visible;
                    //checkUseAzureConnection.Visibility = Visibility.Visible;
                }
                else
                {
                    TxtProtocolSecureConnection.Visibility = Visibility.Collapsed;
                    CmbProtocolForSecureConnection.Visibility = Visibility.Collapsed;
                    CmbProtocolForSecureConnection.SelectedItem = MqttSslProtocols.None;
                    TxtMqttCACertificateFile.Visibility = Visibility.Collapsed;
                    MqttCACertificateFile.Visibility = Visibility.Collapsed;
                    TxtMqttClientCertificateFile.Visibility = Visibility.Collapsed;
                    MqttClientCertificateFile.Visibility = Visibility.Collapsed;
                    //textUseAzureConnection.Visibility = Visibility.Collapsed;
                    //checkUseAzureConnection.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void GenerateNewClientID_Click(object sender, RoutedEventArgs e)
        {
            var nDC = DataContext as MQTTClientChannelSettings;
            nDC.MQTTClientIdentifier = Guid.NewGuid().ToString();
        }
    }
}
