using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;

namespace SNMP.UI
{
    /// <summary>
    /// Interaction logic for StationDetails.xaml
    /// </summary>
    public partial class StationDetails : UserControl
    {
        bool bLoaded = false;
        DriverCodeBaseEx.UI.Controls.BaseStationSettings baseStation = null;
        DependencyPropertyDescriptor channelDescriptor = null;
        DependencyPropertyDescriptor securityDescriptor = null;
        SNMPStationSettings st = null;
        public StationDetails()
        {
            InitializeComponent();

            Loaded += (o, e) =>
            {
                if (!bLoaded)
                {
                    //return;
                    bLoaded = true;
                    st = DataContext as SNMPStationSettings;
                    baseStation = new DriverCodeBaseEx.UI.Controls.BaseStationSettings() { DataContext = DataContext };
                    baseStation.CmbChannel.ItemsSource = st.DriverSettings.ChannelSettings;
                    MainStack.Children.Insert(0, baseStation);

                    channelDescriptor = DependencyPropertyDescriptor.FromProperty(ComboBox.TextProperty, typeof(ComboBox));
                }
                if (baseStation != null)
                {
                   channelDescriptor.AddValueChanged(baseStation.CmbChannel, CmbChannel_TextChanged);
                   if (st.DriverSettings.ChannelSettings != null)
                    {
                        var channels = (from ch in st.DriverSettings.ChannelSettings where ch.Name == st.Channel select ch);
                        if (channels != null && channels.Count() > 0)
                            st.snmpVersion = ((SNMPChannelSettings)channels.ElementAt(0)).snmpVersion;
                    }
                    CmbChannel_TextChanged(baseStation.CmbChannel, new EventArgs());
                     
                }
            };

            Unloaded += (o, e) =>
            {
                if (channelDescriptor != null)
                    channelDescriptor.RemoveValueChanged(baseStation.CmbChannel, CmbChannel_TextChanged);

                if (securityDescriptor != null)
                    securityDescriptor.RemoveValueChanged(CmbSecurityLevel, CmbSecurityLevel_TextChanged);
            };
        }

        /// <summary>
        /// CmbPlcType_TextChanged  Event linked to the PLC type combobox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CmbSecurityLevel_TextChanged(object sender, EventArgs e)
        {
            if (CmbSecurityLevel.SelectedValue != null)
            {
                switch ((SNMPProtocol.SecurityLevel)CmbSecurityLevel.SelectedValue)
                {
                    case SNMPProtocol.SecurityLevel.NoAuthNoPriv:
                        TBAuthenticationProtocol.Visibility = Visibility.Collapsed;
                        CmbAuthenticationProtocol.Visibility = Visibility.Collapsed;
                        TBAuthenticationPassword.Visibility = Visibility.Collapsed;
                        TXAuthenticationPassword.Visibility = Visibility.Collapsed;
                        TBPrivacyEncryptionProtocol.Visibility = Visibility.Collapsed;
                        CmbPrivacyEncryptionProtocol.Visibility = Visibility.Collapsed;
                        TBPrivacyEncryptionPassword.Visibility = Visibility.Collapsed;
                        TXPrivacyEncryptionPassword.Visibility = Visibility.Collapsed;
                        break;

                    case SNMPProtocol.SecurityLevel.AuthNoPriv:
                        TBAuthenticationProtocol.Visibility = Visibility.Visible;
                        CmbAuthenticationProtocol.Visibility = Visibility.Visible;
                        TBAuthenticationPassword.Visibility = Visibility.Visible;
                        TXAuthenticationPassword.Visibility = Visibility.Visible;
                        TBPrivacyEncryptionProtocol.Visibility = Visibility.Collapsed;
                        CmbPrivacyEncryptionProtocol.Visibility = Visibility.Collapsed;
                        TBPrivacyEncryptionPassword.Visibility = Visibility.Collapsed;
                        TXPrivacyEncryptionPassword.Visibility = Visibility.Collapsed;
                        break;

                    case SNMPProtocol.SecurityLevel.AuthPriv:
                        TBAuthenticationProtocol.Visibility = Visibility.Visible;
                        CmbAuthenticationProtocol.Visibility = Visibility.Visible;
                        TBAuthenticationPassword.Visibility = Visibility.Visible;
                        TXAuthenticationPassword.Visibility = Visibility.Visible;
                        TBPrivacyEncryptionProtocol.Visibility = Visibility.Visible;
                        CmbPrivacyEncryptionProtocol.Visibility = Visibility.Visible;
                        TBPrivacyEncryptionPassword.Visibility = Visibility.Visible;
                        TXPrivacyEncryptionPassword.Visibility = Visibility.Visible;
                        if (CmbPrivacyEncryptionProtocol.SelectedValue == null || ((SNMPProtocol.PrivacyEncryptionProtocol)CmbPrivacyEncryptionProtocol.SelectedValue) == SNMPProtocol.PrivacyEncryptionProtocol.None)
                            CmbPrivacyEncryptionProtocol.SelectedValue = SNMPProtocol.PrivacyEncryptionProtocol.Des;
                        break;
                }
            }
        }

        /// <summary>
        /// CmbPlcType_TextChanged  Event linked to the PLC type combobox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CmbChannel_TextChanged(object sender, EventArgs e)
        {
            ComboBox cmbChannel = (ComboBox)sender;
            if (cmbChannel.SelectedValue != null)
            {
                var st = DataContext as SNMPStationSettings;
                if (st.snmpVersion == null)
                    st.snmpVersion = ((SNMPChannelSettings)cmbChannel.SelectedItem).snmpVersion;

                switch (st.snmpVersion)
                {
                    case SNMPVERSION.SNMPv1:
                    case SNMPVERSION.SNMPv2c:
                        TBContextName.Visibility = Visibility.Collapsed;
                        TXContextName.Visibility = Visibility.Collapsed;
                        TBUserName.Visibility = Visibility.Collapsed;
                        TXUserName.Visibility = Visibility.Collapsed;
                        TBSecurityLevel.Visibility = Visibility.Collapsed;
                        CmbSecurityLevel.Visibility = Visibility.Collapsed;
                        TBAuthenticationProtocol.Visibility = Visibility.Collapsed;
                        CmbAuthenticationProtocol.Visibility = Visibility.Collapsed;
                        TBAuthenticationPassword.Visibility = Visibility.Collapsed;
                        TXAuthenticationPassword.Visibility = Visibility.Collapsed;
                        TBPrivacyEncryptionProtocol.Visibility = Visibility.Collapsed;
                        CmbPrivacyEncryptionProtocol.Visibility = Visibility.Collapsed;
                        TBPrivacyEncryptionPassword.Visibility = Visibility.Collapsed;
                        TXPrivacyEncryptionPassword.Visibility = Visibility.Collapsed;
                        break;
                    case SNMPVERSION.SNMPv3:                        
                        CmbSecurityLevel.ItemsSource = Enum.GetValues(typeof(SNMPProtocol.SecurityLevel));
                        CmbAuthenticationProtocol.ItemsSource = Enum.GetValues(typeof(SNMPProtocol.AuthenticationProtocol));
                        CmbPrivacyEncryptionProtocol.ItemsSource = new List<SNMPProtocol.PrivacyEncryptionProtocol>()
                        {
                            SNMPProtocol.PrivacyEncryptionProtocol.Des
                        };

                        securityDescriptor = DependencyPropertyDescriptor.FromProperty(ComboBox.TextProperty, typeof(ComboBox));
                        securityDescriptor.AddValueChanged(CmbSecurityLevel, CmbSecurityLevel_TextChanged);
                        TBContextName.Visibility = Visibility.Visible;
                        TXContextName.Visibility = Visibility.Visible;
                        TBUserName.Visibility = Visibility.Visible;
                        TXUserName.Visibility = Visibility.Visible;
                        TBSecurityLevel.Visibility = Visibility.Visible;
                        CmbSecurityLevel.Visibility = Visibility.Visible;
                        CmbSecurityLevel_TextChanged(CmbSecurityLevel, new EventArgs());
                        break;
                }
            }
        }
    }
}
