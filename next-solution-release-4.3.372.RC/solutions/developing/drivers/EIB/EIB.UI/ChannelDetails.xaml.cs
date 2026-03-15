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
using EIB;
using DriverCodeBase;
using DevExpress.Xpo;
using System.Reflection;
using DriverCodeBase.Enumerators;
using System.ComponentModel;

namespace EIB.UI
{
    /// <summary>
    /// Interaction logic for ChannelDetails.xaml
    /// </summary>
    public partial class ChannelDetails : UserControl
    {
        bool alreadyLoaded = false;
        public DriverCodeBase.UI.Controls.BaseChannelSettings baseDyn;

        public ChannelDetails()
        {
            InitializeComponent();

            baseDyn = new DriverCodeBase.UI.Controls.BaseChannelSettings();
            Loaded += (o, e) =>
            {
                if (alreadyLoaded)
                    return;
                alreadyLoaded = true;
                baseDyn.DataContext = DataContext;
                var cs = DataContext as ChannelSettings;
                MainStack.Children.Insert(0, baseDyn);
                baseDyn.WaitTimeText.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.WaitTime.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.PollNotUse.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.PollNotUseText.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.PollError.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.PollErrorText.Visibility = System.Windows.Visibility.Collapsed;

                CmbConnectorType.ItemsSource = Enum.GetValues(typeof(ConnectorTypes));

                CmbConnectorType_TextChanged();

                DependencyPropertyDescriptor descriptor =
                   DependencyPropertyDescriptor.FromProperty(ComboBox.TextProperty, typeof(ComboBox));
                descriptor.AddValueChanged(CmbConnectorType, CmbConnectorType_TextChanged);

            };
        }

        private void CmbConnectorType_TextChanged(object sender, EventArgs e)
        {
            CmbConnectorType_TextChanged();
        }

        private void CmbConnectorType_TextChanged()
        {
            if (CmbConnectorType.SelectedItem == null)
            {
                TextEIBNetIPAddress.Visibility = System.Windows.Visibility.Hidden;
                EIBNetIPAddress.Visibility = System.Windows.Visibility.Hidden;
                TextMulticastAddress.Visibility = System.Windows.Visibility.Hidden;
                MulticastAddress.Visibility = System.Windows.Visibility.Hidden;
                TextLocalHostName.Visibility = System.Windows.Visibility.Hidden;
                LocalHostName.Visibility = System.Windows.Visibility.Hidden;
                LocalHostName.Text = string.Empty;
                return;
            }

            switch ((ConnectorTypes)CmbConnectorType.SelectedItem)
            {
                case ConnectorTypes.KnxIpRouting:
                    {
                        TextEIBNetIPAddress.Visibility = System.Windows.Visibility.Hidden;
                        EIBNetIPAddress.Visibility = System.Windows.Visibility.Hidden;
                        TextConnectionResetTime.Visibility = System.Windows.Visibility.Hidden;
                        ConnectionResetTime.Visibility = System.Windows.Visibility.Hidden;
                        TextMulticastAddress.Visibility = System.Windows.Visibility.Visible;
                        MulticastAddress.Visibility = System.Windows.Visibility.Visible;
                        TextLocalHostName.Visibility = System.Windows.Visibility.Visible;
                        LocalHostName.Visibility = System.Windows.Visibility.Visible;
                        break;
                    }
                case ConnectorTypes.KnxIpTunneling:
                    {
                        TextEIBNetIPAddress.Visibility = System.Windows.Visibility.Visible;
                        EIBNetIPAddress.Visibility = System.Windows.Visibility.Visible;
                        TextConnectionResetTime.Visibility = System.Windows.Visibility.Visible;
                        ConnectionResetTime.Visibility = System.Windows.Visibility.Visible;
                        TextMulticastAddress.Visibility = System.Windows.Visibility.Hidden;
                        MulticastAddress.Visibility = System.Windows.Visibility.Hidden;
                        TextLocalHostName.Visibility = System.Windows.Visibility.Hidden;
                        LocalHostName.Visibility = System.Windows.Visibility.Hidden;
                        LocalHostName.Text = string.Empty;
                        break;
                    }
            }
        }
    }
}
