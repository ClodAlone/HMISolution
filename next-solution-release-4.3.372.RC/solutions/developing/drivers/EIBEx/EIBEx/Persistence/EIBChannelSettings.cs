using System;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.Text.RegularExpressions;

namespace EIB
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class EIBChannelSettings : ChannelSettings
    {
        #region Constructors

        public EIBChannelSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        private EIBChannelSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        
        #endregion

        public void DefaultSettings()
        {
            base.DefaultSettings();
            _MinBusInactivityTime = 100;
            base.WaitTime = (UInt16)_MinBusInactivityTime;
            _SerialCommPort = EIBProtocol.COMMPORT.Com1;
            _ConnectorType = EIBProtocol.ConnectorTypes.KnxIpRouting;
            _MulticastAddress = "224.0.23.12";
            _EIBNetFriendlyName = String.Empty;
            _EIBNetIpAddress = String.Empty;
            _LocalHostName = String.Empty;
            _EIBNetTcpPort = 3671;
            _ConnectionResetTime = 60;
        }
  
        public override void CopyProperties(ChannelSettings ch)
        {
            base.CopyProperties(ch);
            MinBusInactivityTime = ((EIBChannelSettings)ch).MinBusInactivityTime;
            base.WaitTime = (UInt16)((EIBChannelSettings)ch)._MinBusInactivityTime;
            SerialCommPort = ((EIBChannelSettings)ch).SerialCommPort;
            ConnectorType = ((EIBChannelSettings)ch).ConnectorType;
            MulticastAddress = ((EIBChannelSettings)ch).MulticastAddress;
            EIBNetFriendlyName = ((EIBChannelSettings)ch).EIBNetFriendlyName;
            LocalHostName = ((EIBChannelSettings)ch).LocalHostName;
            EIBNetIpAddress = ((EIBChannelSettings)ch).EIBNetIpAddress;
            EIBNetTcpPort = ((EIBChannelSettings)ch).EIBNetTcpPort;
            ConnectionResetTime = ((EIBChannelSettings)ch).ConnectionResetTime;
        }

        #region Properties

        /// <summary>
        /// Minimum Bus Inactivity Time
        /// </summary>
        private uint _MinBusInactivityTime;
        public uint MinBusInactivityTime
        {
            get
            {
                return _MinBusInactivityTime;
            }
            set
            {
                SetPropertyValue("MinBusInactivityTime", ref _MinBusInactivityTime, value);
            }
        }

        /// <summary>
        /// Connector Type
        /// </summary>
        private EIBProtocol.ConnectorTypes _ConnectorType;
        public EIBProtocol.ConnectorTypes ConnectorType
        {
            get { return _ConnectorType; }
            set
            {
                SetPropertyValue("ConnectorType", ref _ConnectorType, value);
                this.RaisePropertyChangedEvent("MulticastAddress");
                this.RaisePropertyChangedEvent("EIBNetIpAddress");
                this.RaisePropertyChangedEvent("LocalHostName");
            }
        }

        /// <summary>
        /// Multicast Address
        /// </summary>
        private string _MulticastAddress;
        public string MulticastAddress
        {
            get { return _MulticastAddress; }
            set
            {
                SetPropertyValue("MulticastAddress", ref _MulticastAddress, value);
            }
        }

        /// <summary>
        /// Serial Port Number
        /// </summary>
        private EIBProtocol.COMMPORT _SerialCommPort;
        public EIBProtocol.COMMPORT SerialCommPort
        {
            get { return _SerialCommPort; }
            set
            {
                SetPropertyValue("SerialCommPort", ref _SerialCommPort, value);
            }
        }

        /// <summary>
        /// Friendly Name of the TCP/EIB gateway
        /// </summary>
        private string _EIBNetFriendlyName;
        public string EIBNetFriendlyName
        {
            get
            {
                return _EIBNetFriendlyName;
            }
            set
            {
                SetPropertyValue("EIBNetFriendlyName", ref _EIBNetFriendlyName, value);
            }
        }

        /// <summary>
        /// The name of the channel Local host
        /// </summary>
        private string _LocalHostName;
        public string LocalHostName
        {
            get
            {
                return _LocalHostName;
            }
            set
            {
                SetPropertyValue("LocalHostName", ref _LocalHostName, value);
                this.RaisePropertyChangedEvent("ConnectorType");

            }
        }

        /// <summary>
        /// IP Address of the TCP/EIB gateway
        /// </summary>
        private string _EIBNetIpAddress;
        public string EIBNetIpAddress
        {
            get
            {
                return _EIBNetIpAddress;
            }
            set
            {
                SetPropertyValue("EIBNetIpAddress", ref _EIBNetIpAddress, value);
            }
        }

        /// <summary>
        ///  TCP Port of the TCP/EIB gateway
        /// </summary>
        private uint _EIBNetTcpPort;
        public uint EIBNetTcpPort
        {
            get
            {
                return _EIBNetTcpPort;
            }
            set
            {
                SetPropertyValue("EIBNetTcpPort", ref _EIBNetTcpPort, value);
            }
        }

        /// <summary>
        /// Connection Reset Time
        /// </summary>
        private uint _ConnectionResetTime;
        public uint ConnectionResetTime
        {
            get
            {
                return _ConnectionResetTime;
            }
            set
            {
                SetPropertyValue("ConnectionResetTime", ref _ConnectionResetTime, value);
            }
        }

        #endregion


        #region IDataErrorInfo Members

        #region IDataErrorInfo Members

        const string ValidIpAddressRegex = @"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$";
        const string ValidHostnameRegex = @"^(([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]*[a-zA-Z0-9])\.)*([A-Za-z0-9]|[A-Za-z0-9][A-Za-z0-9\-]*[A-Za-z0-9])$";
        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            switch (propertyName)
            {
                case "EIBNetIpAddress":
                    if (_ConnectorType == EIBProtocol.ConnectorTypes.KnxIpTunneling)
                    {
                        //validate IP address
                        if (!Regex.IsMatch(EIBNetIpAddress, ValidIpAddressRegex))
                            return Properties.Resources.InvalidIPAddress;
                    }
                    break;
                case "MulticastAddress":
                    if (_ConnectorType == EIBProtocol.ConnectorTypes.KnxIpRouting)
                    {
                        //validate IP address
                        if (!Regex.IsMatch(MulticastAddress, ValidIpAddressRegex))
                            return Properties.Resources.InvalidIPAddress;
                    }
                    break;
                //case "LocalHostName":
                //    if (_ConnectorType == ConnectorTypes.KnxIpRouting)
                //    {
                //        if (_LocalHostName.Length == 0)
                //            return Properties.Resources.EnterHostName;

                //        if (!Regex.IsMatch(_LocalHostName, ValidIpAddressRegex) && !Regex.IsMatch(_LocalHostName, ValidHostnameRegex))
                //            return Properties.Resources.InvalidHostName;
                //    }
                //    break;
            }

            return null;
        }

        #endregion

        #endregion
    }
}
