using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IpDriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;
using DriverCodeBaseEx;

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

        public override void CopyProperties(ChannelSettings ch)
        {
            base.CopyProperties(ch);
            _SourceNetworkAddress = ((OmronFinsEthernetChannelSettings)ch).SourceNetworkAddress;
            _SourceNode = ((OmronFinsEthernetChannelSettings)ch).SourceNode;
            _SourceUnit = ((OmronFinsEthernetChannelSettings)ch).SourceUnit;
            UdpChannelSettingsLocalHostPort = ((OmronFinsEthernetChannelSettings)ch).UdpChannelSettingsLocalHostPort;
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
        /// <summary>
        /// Verify is more channel has the same setting when local udp port set : this is not allowed 
        /// </summary>
        /// <param name="localHostName"></param>
        /// <param name="localUdpPort"></param>
        /// <param name="hostName"></param>
        /// <param name="udpPort"></param>
        /// <returns></returns>
        private bool AnyChannelsWithLocalPortSetHaveSameSettings(string localHostName, int localUdpPort, string hostName, int udpPort)
        {
            IEnumerable<OmronFinsEthernetChannelSettings> Channels = (DriverSettings as OmronFinsEthernetDriverSettings).ChannelSettings.ToList().Cast<OmronFinsEthernetChannelSettings>();

            return Channels.Count(ch => UdpChannelSettingsLocalHostPort != 0 && ch.UdpChannelSettingsLocalHostName == localHostName && ch.UdpChannelSettingsLocalHostPort == localUdpPort && ch.UdpChannelSettingsHostName == hostName && ch.UdpChannelSettingsLocalHostPort == localUdpPort) > 1;
        }     

        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            switch (propertyName)
            {
                case "SourceNetworkAddress":
                    if (_SourceNetworkAddress < 0 || _SourceNetworkAddress > 127)
                        return Properties.Resources.NetworkAddressOutOfRange;
                    break;
                case "SourceNode":
                    if (_SourceNode < 0 || _SourceNode > 254)
                        return Properties.Resources.NodeOutOfRange;
                    break;

                case "SourceUnit":
                    if (_SourceUnit < 0 || _SourceUnit > 255)
                        return Properties.Resources.UnitOutOfRange;
                    break;
            
                case "UdpChannelSettingsLocalHostName":
                    // check if other channel have same local host name and port
                    if (DriverSettings != null)
                    {
                        if (AnyChannelsWithLocalPortSetHaveSameSettings(UdpChannelSettingsLocalHostName, UdpChannelSettingsLocalHostPort, UdpChannelSettingsHostName, UdpChannelSettingsHostPort))
                            return Properties.Resources.ErrorManyChannlesShareTheSameSettingsWtihLocalPortSet;
                    }
                    break;

                case "UdpChannelSettingsLocalHostPort":
                    // check if other channel have same local host name and port
                    if (DriverSettings != null)
                    {
                        if (AnyChannelsWithLocalPortSetHaveSameSettings(UdpChannelSettingsLocalHostName, UdpChannelSettingsLocalHostPort, UdpChannelSettingsHostName, UdpChannelSettingsHostPort))
                            return Properties.Resources.ErrorManyChannlesShareTheSameSettingsWtihLocalPortSet;
                    }
                    break;

                case "UdpChannelSettingsHostName":
                    // check if other channel have same local host name and port
                    if (DriverSettings != null)
                    {
                        if (AnyChannelsWithLocalPortSetHaveSameSettings(UdpChannelSettingsLocalHostName, UdpChannelSettingsLocalHostPort, UdpChannelSettingsHostName, UdpChannelSettingsHostPort))
                            return Properties.Resources.ErrorManyChannlesShareTheSameSettingsWtihLocalPortSet;
                    }
                    break;

                case "UdpChannelSettingsHostPort":
                    // check if other channel have same local host name and port
                    if (DriverSettings != null)
                    {
                        if (AnyChannelsWithLocalPortSetHaveSameSettings(UdpChannelSettingsLocalHostName, UdpChannelSettingsLocalHostPort, UdpChannelSettingsHostName, UdpChannelSettingsHostPort))
                            return Properties.Resources.ErrorManyChannlesShareTheSameSettingsWtihLocalPortSet;
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
