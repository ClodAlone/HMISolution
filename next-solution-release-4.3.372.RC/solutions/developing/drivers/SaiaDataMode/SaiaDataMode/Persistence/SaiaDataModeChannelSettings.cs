using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;
using IpDriverCodeBase;
using SerialDriverCodeBase;
using System.IO.Ports;

namespace SaiaDataMode
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class SaiaDataModeChannelSettings : ChannelSettings
    {
                #region Constructors

        public SaiaDataModeChannelSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
            session.UpdateSchema(typeof(SaiaDataModeChannelSettings));
        }

        private SaiaDataModeChannelSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        
        #endregion

        public void CopyProperties(SaiaDataModeChannelSettings ch)
        {
            base.CopyProperties(ch);

            ChannelType = ch.ChannelType;

            UdpChannelSettingsHostName = ch.UdpChannelSettingsHostName;
            UdpChannelSettingsHostPort = ch.UdpChannelSettingsHostPort;
            UdpChannelSettingsReadTimeout = ch.UdpChannelSettingsReadTimeout;
            UdpChannelSettingsWriteTimeout = ch.UdpChannelSettingsWriteTimeout;

            CommPortName = ch.CommPortName;
            CommPortNameLinux = ch.CommPortNameLinux;
            CommPortBaudRate = ch.CommPortBaudRate;
            CommPortDataBits = ch.CommPortDataBits;
            CommPortParity = ch.CommPortParity;
            CommPortStopBits = ch.CommPortStopBits;
            CommPortHandshake = ch.CommPortHandshake;
            CommPortRtsEnable = ch.CommPortRtsEnable;
            CommPortDtrEnable = ch.CommPortDtrEnable;
            CommPortReadTimeout = ch.CommPortReadTimeout;
            CommPortWriteTimeout = ch.CommPortWriteTimeout;
        }

        public void DefaultSettings()
        {
            base.DefaultSettings();
            ChannelType = ChannelTypes.Socket;

            UdpChannelSettingsHostName = "";
            UdpChannelSettingsHostPort = 5050;
            UdpChannelSettingsReadTimeout = 2000;
            UdpChannelSettingsWriteTimeout = 2000;

            _CommPortName = "Com1";
            _CommPortNameLinux = string.Empty;
            _CommPortBaudRate = 9600;
            _CommPortDataBits = 8;
            _CommPortParity = (int)Parity.None;
            _CommPortStopBits = (int)StopBits.One;
            _CommPortHandshake = (int)Handshake.None;
            _CommPortRtsEnable = false;
            _CommPortDtrEnable = false;
            _CommPortReadTimeout = 5000;
            _CommPortWriteTimeout = 5000;
        }

        /////////////////////////////

        #region General Properties

        /// <summary>
        /// Enter the channel type
        /// </summary>
        private ChannelTypes _ChannelType;
        public ChannelTypes ChannelType
        {
            get { return _ChannelType; }
            set
            {
                if(SetPropertyValue("ChannelType", ref _ChannelType, value))
                {
                    RaisePropertyChangedEvent("CommPortName");
                }
            }
        }

        #endregion

        #region Udp Properties

        /// <summary>
        /// Host name
        /// </summary>
        private string _UdpChannelSettingsHostName;
        public string UdpChannelSettingsHostName
        {
            get { return _UdpChannelSettingsHostName; }
            set
            {
                /*if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("Host name cannot be null");
                }*/

                SetPropertyValue("UdpChannelSettingsHostName", ref _UdpChannelSettingsHostName, value);
            }
        }

        /// <summary>
        /// Host port
        /// </summary>
        private int _UdpChannelSettingsHostPort;
        public int UdpChannelSettingsHostPort
        {
            get
            {
                return _UdpChannelSettingsHostPort;
            }
            set
            {
                SetPropertyValue("UdpChannelSettingsHostPort", ref _UdpChannelSettingsHostPort, value);
            }
        }

        /// <summary>
        /// Read timeout
        /// </summary>
        private int _UdpChannelSettingsReadTimeout;
        public int UdpChannelSettingsReadTimeout
        {
            get
            {
                return _UdpChannelSettingsReadTimeout;
            }
            set
            {
                SetPropertyValue("UdpChannelSettingsReadTimeout", ref _UdpChannelSettingsReadTimeout, value);
            }
        }

        /// <summary>
        /// Write timeout
        /// </summary>
        private int _UdpChannelSettingsWriteTimeout;
        public int UdpChannelSettingsWriteTimeout
        {
            get
            {
                return _UdpChannelSettingsWriteTimeout;
            }
            set
            {
                SetPropertyValue("UdpChannelSettingsWriteTimeout", ref _UdpChannelSettingsWriteTimeout, value);
            }
        }

        #endregion

        #region serial Properties

        /// <summary>
        /// Port Name
        /// </summary>
        private string _CommPortName;
        public string CommPortName
        {
            get { return _CommPortName; }
            set
            {
                //if (String.IsNullOrEmpty(value))
                //{
                //    throw new ArgumentException("Port name cannot be null");
                //}

                SetPropertyValue("CommPortName", ref _CommPortName, value);
            }
        }

        /// <summary>
        /// Port Name Linux
        /// </summary>
        private string _CommPortNameLinux;
        public string CommPortNameLinux
        {
            get { return _CommPortNameLinux; }
            set
            {
                //if (String.IsNullOrEmpty(value))
                //{
                //    throw new ArgumentException("Port name cannot be null");
                //}

                SetPropertyValue("CommPortNameLinux", ref _CommPortNameLinux, value);
            }
        }

        /// <summary>
        /// Baud rate
        /// </summary>
        private int _CommPortBaudRate;
        public int CommPortBaudRate
        {
            get
            {
                return _CommPortBaudRate;
            }
            set
            {
                SetPropertyValue("CommPortBaudRate", ref _CommPortBaudRate, value);
            }
        }

        /// <summary>
        /// Data bits
        /// </summary>
        private int _CommPortDataBits;
        public int CommPortDataBits
        {
            get
            {
                return _CommPortDataBits;
            }
            set
            {
                SetPropertyValue("CommPortDataBits", ref _CommPortDataBits, value);
            }
        }

        /// <summary>
        /// Parity
        /// </summary>
        private int _CommPortParity;
        public int CommPortParity
        {
            get
            {
                return _CommPortParity;
            }
            set
            {
                SetPropertyValue("CommPortParity", ref _CommPortParity, value);
            }
        }

        /// <summary>
        /// Stop bits
        /// </summary>
        private int _CommPortStopBits;
        public int CommPortStopBits
        {
            get
            {
                return _CommPortStopBits;
            }
            set
            {
                SetPropertyValue("CommPortStopBits", ref _CommPortStopBits, value);
            }
        }

        /// <summary>
        /// Handshake
        /// </summary>
        private int _CommPortHandshake;
        public int CommPortHandshake
        {
            get
            {
                return _CommPortHandshake;
            }
            set
            {
                SetPropertyValue("CommPortHandshake", ref _CommPortHandshake, value);
            }
        }

        /// <summary>
        /// RTS enabled
        /// </summary>
        private bool _CommPortRtsEnable;
        public bool CommPortRtsEnable
        {
            get
            {
                return _CommPortRtsEnable;
            }
            set
            {
                SetPropertyValue("CommPortRtsEnable", ref _CommPortRtsEnable, value);
            }
        }

        /// <summary>
        /// DTR enabled
        /// </summary>
        private bool _CommPortDtrEnable;
        public bool CommPortDtrEnable
        {
            get
            {
                return _CommPortDtrEnable;
            }
            set
            {
                SetPropertyValue("CommPortDtrEnable", ref _CommPortDtrEnable, value);
            }
        }

        /// <summary>
        /// Read timeout
        /// </summary>
        private int _CommPortReadTimeout;
        public int CommPortReadTimeout
        {
            get
            {
                return _CommPortReadTimeout;
            }
            set
            {
                SetPropertyValue("CommPortReadTimeout", ref _CommPortReadTimeout, value);
            }
        }

        /// <summary>
        /// Write timeout
        /// </summary>
        private int _CommPortWriteTimeout;
        public int CommPortWriteTimeout
        {
            get
            {
                return _CommPortWriteTimeout;
            }
            set
            {
                SetPropertyValue("CommPortWriteTimeout", ref _CommPortWriteTimeout, value);
            }
        }

        #endregion



        #region IDataErrorInfo Members

        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            if (_ChannelType == ChannelTypes.Serial)
            {
                switch (propertyName)
                {
                    case "CommPortName":
                        if (DriverSettings != null && ((from c in DriverSettings.ChannelSettings/*.AsParallel()*/
                                                        where c != this && (c as SaiaDataModeChannelSettings).CommPortName == CommPortName &&
                                                        (c as SaiaDataModeChannelSettings).ChannelType == ChannelTypes.Serial
                                                        select c).ToList().Count > 0))
                        {
                            return Properties.Resources.ErrorSerialPortAlreadyUsed;
                        }
                        break;
                    case "CommPortBaudRate":
                        break;
                    case "CommPortDataBits":
                        if (CommPortDataBits != 7 && CommPortDataBits != 8)
                        {
                            return Properties.Resources.ErrorDataBits;
                        }
                        break;
                    case "CommPortParity":
                        break;
                    case "CommPortStopBits":
                        break;
                    case "CommPortHandshake":
                        break;
                    case "CommPortRtsEnable":
                        break;
                    case "CommPortDtrEnable":
                        break;
                    case "CommPortReadTimeout":
                        break;
                    case "CommPortWriteTimeout":
                        break;
                }
            }

            return null;
        }

        #endregion

    }

    public class SaiaDataModeUdpChannelSettings : UdpChannelSettings
    {
        #region Constructors

        protected SaiaDataModeUdpChannelSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public SaiaDataModeUdpChannelSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }


        #endregion

        public void CopyProperties(SaiaDataModeChannelSettings ch)
        {
            base.CopyProperties(ch);
            KeepOpened = true;
            UdpChannelSettingsHostName = ch.UdpChannelSettingsHostName;
            UdpChannelSettingsHostPort = ch.UdpChannelSettingsHostPort;
        }



    }

    public class SaiaDataModeSerialChannelSettings : SerialChannelSettings
    {
        #region Constructors

        protected SaiaDataModeSerialChannelSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public SaiaDataModeSerialChannelSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

        public void CopyProperties(SaiaDataModeChannelSettings ch)
        {
            base.CopyProperties(ch);
            CommPortName = ch.CommPortName;
            CommPortNameLinux = ch.CommPortNameLinux;
            CommPortBaudRate = ch.CommPortBaudRate;
            CommPortDataBits = ch.CommPortDataBits;
            CommPortParity = ch.CommPortParity;
            CommPortStopBits = ch.CommPortStopBits;
            CommPortHandshake = ch.CommPortHandshake;
            CommPortRtsEnable = ch.CommPortRtsEnable;
            CommPortDtrEnable = ch.CommPortDtrEnable;
            CommPortReadTimeout = ch.CommPortReadTimeout;
            CommPortWriteTimeout = ch.CommPortWriteTimeout;
        }

    }
}

