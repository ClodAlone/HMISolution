using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;
using uPLibrary.Networking.M2Mqtt;
using System.Text.RegularExpressions;

namespace MQTTClient
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class MQTTClientChannelSettings : ChannelSettings
    {
                #region Constructors

        public MQTTClientChannelSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
            session.UpdateSchema(typeof(MQTTClientChannelSettings));
        }

        private MQTTClientChannelSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        
        #endregion

        public void DefaultSettings()
        {
            base.DefaultSettings();
            _MQTTServerHostName = String.Empty;
            _MQTTServerHostPort = 1883;
            _MQTTClientIdentifier = String.Empty;
            _MQTTClientUsername = String.Empty;
            _MQTTClientPassword = String.Empty;
            _MQTTClientUseSecureConnection = false;
            _MQTTClientProtocolForSecureConnection = MqttSslProtocols.None;
            _MQTTClientCACertificateFile = String.Empty;
            _MQTTClientClientCertificateFile = String.Empty;
            _MQTTClientCleanSession = true;
            _MQTTClientKeepAliveTime = 60;
            _MQTTClientMaxNumberOfPendingRequests = 5;
            //_MQTTClientUseAzureConnection = false;
        }

        //steve 080711
        public void CopyProperties(MQTTClientChannelSettings ch)
        {
            base.CopyProperties(ch);
            _MQTTServerHostName = ch.MQTTServerHostName;
            _MQTTServerHostPort = ch.MQTTServerHostPort;
            _MQTTClientIdentifier = ch.MQTTClientIdentifier;
            _MQTTClientUsername = ch.MQTTClientUsername;
            _MQTTClientPassword = ch.MQTTClientPassword;
            _MQTTClientUseSecureConnection = ch.MQTTClientUseSecureConnection;
            _MQTTClientProtocolForSecureConnection = ch.MQTTClientProtocolForSecureConnection;
            _MQTTClientCACertificateFile = ch.MQTTClientCACertificateFile;
            _MQTTClientClientCertificateFile = ch._MQTTClientClientCertificateFile;
            _MQTTClientCleanSession = ch.MQTTClientCleanSession;
            _MQTTClientKeepAliveTime = ch.MQTTClientKeepAliveTime;
            _MQTTClientMaxNumberOfPendingRequests = ch.MQTTClientMaxNumberOfPendingRequests;
            //_MQTTClientUseAzureConnection = ch.MQTTClientUseAzureConnection;
        }
        /////////////////////////////

        #region Properties

        /// <summary>
        /// Host Name of the MQTT server. Empty string = local machine. Example: 192.168.0.39
        /// </summary>
        private string _MQTTServerHostName;
        public string MQTTServerHostName
        {
            get
            {
                return _MQTTServerHostName;
            }
            set
            {
                SetPropertyValue("MQTTServerHostName", ref _MQTTServerHostName, value);
                RaisePropertyChangedEvent("MQTTServerHostPort");
                RaisePropertyChangedEvent("MQTTClientIdentifier");
            }
        }

        /// <summary>
        ///  Port number of the the MQTT server. Default value = 1883
        /// </summary>
        private uint _MQTTServerHostPort;
        public uint MQTTServerHostPort
        {
            get
            {
                return _MQTTServerHostPort;
            }
            set
            {
                SetPropertyValue("MQTTServerHostPort",
                                 ref _MQTTServerHostPort, value);
                RaisePropertyChangedEvent("MQTTServerHostName");
                RaisePropertyChangedEvent("MQTTClientIdentifier");
            }
        }

        /// <summary>
        /// Client Identifier.
        /// </summary>
        private string _MQTTClientIdentifier;
        public string MQTTClientIdentifier
        {
            get
            {
                return _MQTTClientIdentifier;
            }
            set
            {
                SetPropertyValue("MQTTClientIdentifier", ref _MQTTClientIdentifier, value);
                RaisePropertyChangedEvent("MQTTServerHostName");
                RaisePropertyChangedEvent("MQTTServerHostPort");
            }
        }

        /// <summary>
        /// User name for the connection to the broker (optional).
        /// </summary>
        private string _MQTTClientUsername;
        public string MQTTClientUsername
        {
            get
            {
                return _MQTTClientUsername;
            }
            set
            {
                SetPropertyValue("MQTTClientUsername", ref _MQTTClientUsername, value);
            }
        }

        /// <summary>
        /// Password for the connection to the broker (optional).
        /// </summary>
        private string _MQTTClientPassword;
        public string MQTTClientPassword
        {
            get
            {
                return _MQTTClientPassword;
            }
            set
            {
                SetPropertyValue("MQTTClientPassword", ref _MQTTClientPassword, value);
            }
        }

        /// <summary>
        /// Use secure connection (optional).
        /// </summary>
        private bool _MQTTClientUseSecureConnection;
        public bool MQTTClientUseSecureConnection
        {
            get
            {
                return _MQTTClientUseSecureConnection;
            }
            set
            {
                SetPropertyValue("MQTTClientUseSecureConnection", ref _MQTTClientUseSecureConnection, value);
                RaisePropertyChangedEvent("MQTTClientProtocolForSecureConnection");
            }
        }

        /// <summary>
        /// Protocol for secure connection (optional).
        /// </summary>
        private MqttSslProtocols _MQTTClientProtocolForSecureConnection;
        public MqttSslProtocols MQTTClientProtocolForSecureConnection
        {
            get
            {
                return _MQTTClientProtocolForSecureConnection;
            }
            set
            {
                SetPropertyValue("MQTTClientProtocolForSecureConnection", ref _MQTTClientProtocolForSecureConnection, value);
                RaisePropertyChangedEvent("MQTTClientUseSecureConnection");
            }
        }

        /// <summary>
        /// File of the CA Certificate.
        /// </summary>
        private string _MQTTClientCACertificateFile;
        public string MQTTClientCACertificateFile
        {
            get
            {
                return _MQTTClientCACertificateFile;
            }
            set
            {
                SetPropertyValue("MQTTClientCACertificateFile", ref _MQTTClientCACertificateFile, value);
            }
        }

        /// <summary>
        /// File of the Client Certificate.
        /// </summary>
        private string _MQTTClientClientCertificateFile;
        public string MQTTClientClientCertificateFile
        {
            get
            {
                return _MQTTClientClientCertificateFile;
            }
            set
            {
                SetPropertyValue("MQTTClientClientCertificateFile", ref _MQTTClientClientCertificateFile, value);
            }
        }

        /// <summary>
        /// Clean session (optional).
        /// </summary>
        private bool _MQTTClientCleanSession;
        public bool MQTTClientCleanSession
        {
            get
            {
                return _MQTTClientCleanSession;
            }
            set
            {
                SetPropertyValue("MQTTClientCleanSession", ref _MQTTClientCleanSession, value);
            }
        }

        /// <summary>
        ///  Keep Alive Time in seconds. Default value = 60
        /// </summary>
        private UInt16 _MQTTClientKeepAliveTime;
        public UInt16 MQTTClientKeepAliveTime
        {
            get
            {
                return _MQTTClientKeepAliveTime;
            }
            set
            {
                SetPropertyValue("MQTTClientKeepAliveTime",
                                 ref _MQTTClientKeepAliveTime, value);
            }
        }

        /// <summary>
        ///  Max. Number of Pending Requests. Default value = 5
        /// </summary>
        private uint _MQTTClientMaxNumberOfPendingRequests;
        public uint MQTTClientMaxNumberOfPendingRequests
        {
            get
            {
                return _MQTTClientMaxNumberOfPendingRequests;
            }
            set
            {
                SetPropertyValue("MQTTClientMaxNumberOfPendingRequests",
                                 ref _MQTTClientMaxNumberOfPendingRequests, value);
            }
        }

        ///// <summary>
        ///// Use Azure connection (optional).
        ///// </summary>
        //private bool _MQTTClientUseAzureConnection;
        //public bool MQTTClientUseAzureConnection
        //{
        //    get
        //    {
        //        return _MQTTClientUseAzureConnection;
        //    }
        //    set
        //    {
        //        SetPropertyValue("MQTTClientUseAzureConnection", ref _MQTTClientUseAzureConnection, value);
        //    }
        //}

        #endregion

        #region IDataErrorInfo Members

        const string ValidIpAddressRegex = @"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$";
        const string ValidHostnameRegex = @"^(([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]*[a-zA-Z0-9])\.)*([A-Za-z0-9]|[A-Za-z0-9][A-Za-z0-9\-]*[A-Za-z0-9])$";

        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            if(propertyName == "MQTTServerHostName")          
            {
                if(string.IsNullOrWhiteSpace(_MQTTServerHostName))
                {
                    return Properties.Resources.MQTTClientInvalidBrokerHostName;
                }

                if (!Regex.IsMatch(_MQTTServerHostName, ValidIpAddressRegex) && !Regex.IsMatch(_MQTTServerHostName, ValidHostnameRegex))
                {
                    return Properties.Resources.MQTTClientInvalidBrokerHostName;
                }
            }
            else if ((propertyName == "MQTTClientIdentifier") && string.IsNullOrWhiteSpace(_MQTTClientIdentifier))
            {
                return Properties.Resources.MQTTClientInvalidClientIdentifier;
            }

            switch (propertyName)
            {
                case "MQTTServerHostName":
                case "MQTTServerHostPort":
                case "MQTTClientIdentifier":
                    if (!string.IsNullOrWhiteSpace(_MQTTServerHostName) &&
                       !string.IsNullOrWhiteSpace(_MQTTClientIdentifier) &&
                       (_MQTTServerHostPort > 0))
                    {
                        if ((DriverSettings != null) &&
                           ((from c in DriverSettings.ChannelSettings
                             where ((c != this) &&
                                    (c as MQTTClientChannelSettings).MQTTServerHostName == _MQTTServerHostName &&
                                    (c as MQTTClientChannelSettings).MQTTServerHostPort == _MQTTServerHostPort &&
                                    (c as MQTTClientChannelSettings).MQTTClientIdentifier == _MQTTClientIdentifier)
                             select c).ToList().Count > 0))
                        {
                            return Properties.Resources.MQTTClientErrorDuplicatedChannel;
                        }
                    }
                    break;

                case "MQTTClientUseSecureConnection":
                case "MQTTClientProtocolForSecureConnection":
                    if ((_MQTTClientUseSecureConnection == true) && (MQTTClientProtocolForSecureConnection == MqttSslProtocols.None))
                    {
                        return Properties.Resources.MQTTClientInvalidProtocolForSecureConnection;
                    }
                    break;

                case "MQTTClientMaxNumberOfPendingRequests":
                    if((MQTTClientMaxNumberOfPendingRequests < 1) || (MQTTClientMaxNumberOfPendingRequests > 100))
                    {
                        return Properties.Resources.MQTTClientInvalidMaxNumberOfPendingRequests;
                    }
                    break;
            }

            return null;
        }

        #endregion
    
    }
}
