using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IpDriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace OmronFinsEthernet
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class OmronFinsEthernetChannelSettings : UdpChannelSettings
    {
                #region Constructors

        public OmronFinsEthernetChannelSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        private OmronFinsEthernetChannelSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        
        #endregion

        public void DefaultSettings()
        {
            base.DefaultSettings();
            UdpChannelSettingsHostPort = 9600;
            _SourceNetworkAddress = 0;
            _SourceNode = 50;
            _SourceUnit = 0;
            UdpChannelSettingsLocalHostPort = 0;
        }

        public void CopyProperties(OmronFinsEthernetChannelSettings ch)
        {
            base.CopyProperties(ch);
            _SourceNetworkAddress = ch.SourceNetworkAddress;
            _SourceNode = ch.SourceNode;
            _SourceUnit = ch.SourceUnit;
        }
        /////////////////////////////

        #region Properties

        /// <summary>
        /// Enter the Source Network Address Number(0...127 )
        /// </summary>
        private byte _SourceNetworkAddress;
        public byte SourceNetworkAddress
        {
            get
            {
                return _SourceNetworkAddress;
            }
            set
            {
                SetPropertyValue("SourceNetworkAddress", ref _SourceNetworkAddress, value);
            }
        }

        /// <summary>
        /// Enter the Source Node Number(0...254 )
        /// </summary>
        private byte _SourceNode;
        public byte SourceNode
        {
            get
            {
                return _SourceNode;
            }
            set
            {
                SetPropertyValue("SourceNode", ref _SourceNode, value);
            }
        }

        /// <summary>
        /// Source Unit Number(0...255 )
        /// </summary>
        private byte _SourceUnit;
        public byte SourceUnit
        {
            get
            {
                return _SourceUnit;
            }
            set
            {
                SetPropertyValue("SourceUnit", ref _SourceUnit, value);
            }
        }

        #endregion


        #region IDataErrorInfo Members
        private bool AnyChannelsHaveSameNameAndPortHost(string HostName, int UdpPort , int localUdpPort)
        {
            IEnumerable<OmronFinsEthernetChannelSettings> Channels = (DriverSettings as OmronFinsEthernetDriverSettings).ChannelSettings.ToList().Cast<OmronFinsEthernetChannelSettings>();

            return Channels.Count(ch => UdpChannelSettingsLocalHostPort != 0 && ch.UdpChannelSettingsHostPort == UdpPort && ch.UdpChannelSettingsLocalHostPort == localUdpPort) > 1;
        }
        private bool AnyChannelsHaveSameNameAndPort(string localHostName, int localUdpPort, int UdpPort)
        {
            IEnumerable<OmronFinsEthernetChannelSettings> Channels = (DriverSettings as OmronFinsEthernetDriverSettings).ChannelSettings.ToList().Cast<OmronFinsEthernetChannelSettings>();

            return Channels.Count(ch => UdpChannelSettingsLocalHostPort != 0 && ch.UdpChannelSettingsLocalHostPort == localUdpPort && ch.UdpChannelSettingsHostPort == UdpPort) > 1;
        }

        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            if (propertyName == "SourceNetworkAddress")
            {
                if (_SourceNetworkAddress < 0 || _SourceNetworkAddress > 127)
                    return Properties.Resources.NetworkAddressOutOfRange;

            }
            if (propertyName == "SourceNode")
            {
                if (_SourceNode < 0 || _SourceNode > 254)
                    return Properties.Resources.NodeOutOfRange;

            }

            if (propertyName == "SourceUnit")
            {
                if (_SourceUnit < 0 || _SourceUnit > 255)
                    return Properties.Resources.UnitOutOfRange;

            }

            switch (propertyName)
            {
                case "UdpChannelSettingsLocalHostName":
                    // check if other channel have same local host name and port
                    if (DriverSettings != null /*&& !((OmronFinsEthernetDriverSettings)DriverSettings).AllowOtherBACnetClients.Value*/)
                    {
                        if (AnyChannelsHaveSameNameAndPort(UdpChannelSettingsLocalHostName, UdpChannelSettingsLocalHostPort, UdpChannelSettingsHostPort))
                            return string.Format(Properties.Resources.AOBCLocalHostNameAndPortAlreadyUsed, Properties.Resources.CaptionAllowOtherOmronFinsEthernetClients);
                    }
                    break;

                case "UdpChannelSettingsLocalHostPort":
                    // check if other channel have same local host name and port
                    if (DriverSettings != null /*&& !((OmronFinsEthernetDriverSettings)DriverSettings).AllowOtherBACnetClients.Value*/)
                    {
                        if (AnyChannelsHaveSameNameAndPort(UdpChannelSettingsLocalHostName, UdpChannelSettingsLocalHostPort, UdpChannelSettingsHostPort))
                        {
                            return string.Format(Properties.Resources.AOBCLocalHostNameAndPortAlreadyUsed, Properties.Resources.CaptionAllowOtherOmronFinsEthernetClients);
                        }
                    }
                    break;

                case "UdpChannelSettingsHostName":
                    // check if other channel have same local host name and port
                    if (DriverSettings != null /*&& !((OmronFinsEthernetDriverSettings)DriverSettings).AllowOtherBACnetClients.Value*/)
                    {
                        if (AnyChannelsHaveSameNameAndPortHost(UdpChannelSettingsHostName, UdpChannelSettingsHostPort, UdpChannelSettingsLocalHostPort))
                        {
                            return string.Format(Properties.Resources.AOBCLocalHostNameAndPortAlreadyUsed, Properties.Resources.CaptionAllowOtherOmronFinsEthernetClients);
                        }
                    }
                    break;

                case "UdpChannelSettingsHostPort":
                    // check if other channel have same local host name and port
                    if (DriverSettings != null /*&& !((OmronFinsEthernetDriverSettings)DriverSettings).AllowOtherBACnetClients.Value*/)
                    {
                        if (AnyChannelsHaveSameNameAndPortHost(UdpChannelSettingsHostName, UdpChannelSettingsHostPort, UdpChannelSettingsLocalHostPort))
                        {
                            return string.Format(Properties.Resources.AOBCLocalHostNameAndPortAlreadyUsed, Properties.Resources.CaptionAllowOtherOmronFinsEthernetClients);
                        }
                    }
                    break;
            }

            return null;
        }

        protected override void OnChanged(string propertyName, object oldValue, object newValue)
        {
            base.OnChanged(propertyName, oldValue, newValue);
            switch (propertyName)
            {
                case "UdpChannelSettingsHostName":
                    RaisePropertyChangedEvent("UdpChannelSettingsLocalHostPort");
                    RaisePropertyChangedEvent("UdpChannelSettingsHostPort");
                    break;

                case "UdpChannelSettingsLocalHostPort":
                    RaisePropertyChangedEvent("UdpChannelSettingsHostName");
                    RaisePropertyChangedEvent("UdpChannelSettingsHostPort");
                    break;

                case "UdpChannelSettingsHostPort":
                    RaisePropertyChangedEvent("UdpChannelSettingsHostName");
                    RaisePropertyChangedEvent("UdpChannelSettingsHostPort");
                    break;

            }
        }

        #endregion

    }
}
