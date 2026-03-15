using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;
using IpDriverCodeBaseEx;
using SerialDriverCodeBaseEx;
using System.IO.Ports;
using System.Text.RegularExpressions;

namespace ROCDriver
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class ROCDriverChannelSettings : ChannelSettings
    {
        #region Constructors

        public ROCDriverChannelSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        private ROCDriverChannelSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        
        #endregion

        public void CopyProperties(ROCDriverChannelSettings ch)
        {
            base.CopyProperties(ch);

            HostAddress = ch.HostAddress;
            HostGroup = ch.HostGroup;

            ChannelType = ch.ChannelType;

            CheckConnectionFrequency = ch.CheckConnectionFrequency;

            TcpChannelSettingsHostName = ch.TcpChannelSettingsHostName;
            TcpChannelSettingsHostPort = ch.TcpChannelSettingsHostPort;
            TcpChannelSettingsReadTimeout = ch.TcpChannelSettingsReadTimeout;
            TcpChannelSettingsWriteTimeout = ch.TcpChannelSettingsWriteTimeout;
            TcpChannelSettingsBackupHostName = ch.TcpChannelSettingsBackupHostName;
            TcpChannelSettingsSwitchHostTimeout = ch.TcpChannelSettingsSwitchHostTimeout;

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
            USBconnection = ch.USBconnection;

        }

        public void DefaultSettings()
        {
            base.DefaultSettings();

            HostAddress = 1;
            HostGroup = 1;

            ChannelType = ChannelTypes.Socket;
            CheckConnectionFrequency = 60;

            TcpChannelSettingsHostName = "";
            TcpChannelSettingsHostPort = 4000;
            TcpChannelSettingsReadTimeout = 2000;
            TcpChannelSettingsWriteTimeout = 2000;
            TcpChannelSettingsBackupHostName = "";
            TcpChannelSettingsSwitchHostTimeout = 10000;

            _CommPortName = "Com1";
            _CommPortName = string.Empty;
            _CommPortBaudRate = 9600;
            _CommPortDataBits = 8;
            _CommPortParity = (int)Parity.Odd;
            _CommPortStopBits = (int)StopBits.One;
            _CommPortHandshake = (int)Handshake.None;
            _CommPortRtsEnable = false;
            _CommPortDtrEnable = false;
            _CommPortReadTimeout = 5000;
            _CommPortWriteTimeout = 5000;
            _USBconnection = false;

        }

        /////////////////////////////

        #region General Properties

        /// <summary>
        /// Enter the channel Address (Unit)
        /// </summary>
        private Byte _HostAddress;
        public Byte HostAddress
        {
            get { return _HostAddress; }
            set
            {
                SetPropertyValue("HostAddress", ref _HostAddress, value);
            }
        }

        /// <summary>
        /// Enter the channel Group
        /// </summary>
        private Byte _HostGroup;
        public Byte HostGroup
        {
            get { return _HostGroup; }
            set
            {
                SetPropertyValue("HostGroup", ref _HostGroup, value);
            }
        }

        /// <summary>
        /// Enter the channel type
        /// </summary>
        private ChannelTypes _ChannelType;
        public ChannelTypes ChannelType
        {
            get { return _ChannelType; }
            set
            {
                if (SetPropertyValue("ChannelType", ref _ChannelType, value))
                {
                    RaisePropertyChangedEvent("CommPortName");
                }
            }
        }

        /// <summary>
        /// Enter the frequency of connection checks
        /// </summary>
        private uint _CheckConnectionFrequency;
        public uint CheckConnectionFrequency
        {
            get { return _CheckConnectionFrequency; }
            set
            {
                SetPropertyValue("CheckConnectionFrequency", ref _CheckConnectionFrequency, value);
            }
        }

        #endregion

        #region Tcp Properties

        /// <summary>
        /// Host name
        /// </summary>
        private string _TcpChannelSettingsHostName;
        public string TcpChannelSettingsHostName
        {
            get { return _TcpChannelSettingsHostName; }
            set
            {
                /*if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("Host name cannot be null");
                }*/

                SetPropertyValue("TcpChannelSettingsHostName", ref _TcpChannelSettingsHostName, value);
            }
        }

        /// <summary>
        /// Host port
        /// </summary>
        private int _TcpChannelSettingsHostPort;
        public int TcpChannelSettingsHostPort
        {
            get
            {
                return _TcpChannelSettingsHostPort;
            }
            set
            {
                SetPropertyValue("TcpChannelSettingsHostPort", ref _TcpChannelSettingsHostPort, value);
            }
        }

        /// <summary>  Backup Host name. </summary>
        private string _TcpChannelSettingsBackupHostName;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the name of the TCP channel settings backup host. </summary>
        ///
        /// <value> The name of the TCP channel settings backup host. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string TcpChannelSettingsBackupHostName
        {
            get { return _TcpChannelSettingsBackupHostName; }
            set
            {
                /*if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("Host name cannot be null");
                }*/

                SetPropertyValue("TcpChannelSettingsBackupHostName", ref _TcpChannelSettingsBackupHostName, value);
            }
        }

        /// <summary>   Switch Host timeout. </summary>
        private int _TcpChannelSettingsSwitchHostTimeout;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the TCP channel settings Switch Host timeout. </summary>
        ///
        /// <value> The TCP channel settings Switc hHost timeout. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public int TcpChannelSettingsSwitchHostTimeout
        {
            get
            {
                return _TcpChannelSettingsSwitchHostTimeout;
            }
            set
            {
                SetPropertyValue("TcpChannelSettingsSwitchHostTimeout", ref _TcpChannelSettingsSwitchHostTimeout, value);
            }
        }

        /// <summary>
        /// Read timeout
        /// </summary>
        private int _TcpChannelSettingsReadTimeout;
        public int TcpChannelSettingsReadTimeout
        {
            get
            {
                return _TcpChannelSettingsReadTimeout;
            }
            set
            {
                SetPropertyValue("TcpChannelSettingsReadTimeout", ref _TcpChannelSettingsReadTimeout, value);
            }
        }

        /// <summary>
        /// Write timeout
        /// </summary>
        private int _TcpChannelSettingsWriteTimeout;
        public int TcpChannelSettingsWriteTimeout
        {
            get
            {
                return _TcpChannelSettingsWriteTimeout;
            }
            set
            {
                SetPropertyValue("TcpChannelSettingsWriteTimeout", ref _TcpChannelSettingsWriteTimeout, value);
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

        /// <summary>
        /// USB connection
        /// </summary>
        private bool _USBconnection;
        public bool USBconnection
        {
            get
            {
                return _USBconnection;
            }
            set
            {
                SetPropertyValue("USBconnection", ref _USBconnection, value);
            }
        }

        #endregion



        #region IDataErrorInfo Members

        const string ValidIpAddressRegex = @"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$";
        const string ValidHostnameRegex = @"^(([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]*[a-zA-Z0-9])\.)*([A-Za-z0-9]|[A-Za-z0-9][A-Za-z0-9\-]*[A-Za-z0-9])$";
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
                        if (string.IsNullOrWhiteSpace(CommPortName))
                            return Properties.Resources.ErrorPortNameInvalid;

                        if (DriverSettings != null && ((from c in DriverSettings.ChannelSettings/*.AsParallel()*/
                                                        where c != this && (c as ROCDriverChannelSettings).CommPortName == CommPortName &&
                                                        (c as ROCDriverChannelSettings).ChannelType == ChannelTypes.Serial
                                                        select c).ToList().Count > 0))
                        {
                            return Properties.Resources.ErrorSerialPortAlreadyUsed;
                        }
                        break;
                    case "CommPortNameLinux":
                        if (!string.IsNullOrEmpty(CommPortNameLinux))
                        {
                            if (DriverSettings != null && ((from c in DriverSettings.ChannelSettings/*.AsParallel()*/
                                                            where c != this && (c as SerialChannelSettings).CommPortNameLinux == CommPortNameLinux
                                                            select c).ToList().Count > 0))
                            {
                                return Properties.Resources.ErrorSerialPortAlreadyUsed;
                            }
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
                    case "TcpChannelSettingsHostName":
                        break;
                }
            }
            else
            {
                switch(propertyName)
                {
                    case "TcpChannelSettingsHostName":
                        if (TcpChannelSettingsHostName != null && TcpChannelSettingsHostName.Length == 0)
                        {
                            return Properties.Resources.EnterHostName;
                        }

                        if (TcpChannelSettingsHostName != null &&
                            !Regex.IsMatch(TcpChannelSettingsHostName, ValidIpAddressRegex) &&
                            !Regex.IsMatch(TcpChannelSettingsHostName, ValidHostnameRegex))
                        {
                            return Properties.Resources.InvalidHostName;
                        }
                        break;
                    case "CommPortName":
                        break;
                    case "CommPortNameLinux":
                        break;
                    case "CommPortDataBits":
                        break;
                }

            }

            return null;
        }

        #endregion
        
        protected override void OnChanged(string propertyName, object oldValue, object newValue)
        {
            base.OnChanged(propertyName, oldValue, newValue);
            if (propertyName == "ChannelType")
            {
                RaisePropertyChangedEvent("TcpChannelSettingsHostName");
                RaisePropertyChangedEvent("CommPortName");
                RaisePropertyChangedEvent("CommPortNameLinux");
                RaisePropertyChangedEvent("CommPortDataBits");
            }
        }

    }

    public class ROCDriverTcpChannelSettings : TcpChannelSettings
    {
        #region Constructors

        protected ROCDriverTcpChannelSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public ROCDriverTcpChannelSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }


        #endregion

        public void CopyProperties(ROCDriverChannelSettings ch)
        {
            base.CopyProperties(ch);
            KeepOpened = true;
            TcpHostAddress = ch.HostAddress;
            TcpHostGroup = ch.HostGroup;
            TcpChannelSettingsHostName = ch.TcpChannelSettingsHostName;
            TcpChannelSettingsHostPort = ch.TcpChannelSettingsHostPort;
            TcpChannelSettingsBackupHostName = ch.TcpChannelSettingsBackupHostName;
            TcpChannelSettingsSwitchHostTimeout = ch.TcpChannelSettingsSwitchHostTimeout;
        }

        #region Properties
        /// <summary>
        /// TCP channel Address (Unit)
        /// </summary>
        private Byte _TcpHostAddress;
        public Byte TcpHostAddress
        {
            get { return _TcpHostAddress; }
            set
            {
                SetPropertyValue("HostAddress", ref _TcpHostAddress, value);
            }
        }

        /// <summary>
        /// TCP channel Group
        /// </summary>
        private Byte _TcpHostGroup;
        public Byte TcpHostGroup
        {
            get { return _TcpHostGroup; }
            set
            {
                SetPropertyValue("ChannelType", ref _TcpHostGroup, value);
            }
        }
        #endregion

    }

    public class ROCDriverSerialChannelSettings : SerialChannelSettings
    {
        #region Constructors

        protected ROCDriverSerialChannelSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public ROCDriverSerialChannelSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

        public void CopyProperties(ROCDriverChannelSettings ch)
        {
            base.CopyProperties(ch);
            SerialHostAddress = ch.HostAddress;
            SerialHostGroup = ch.HostGroup;
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


        #region Properties
        /// <summary>
        /// Serial channel Address (Unit)
        /// </summary>
        private Byte _SerialHostAddress;
        public Byte SerialHostAddress
        {
            get { return _SerialHostAddress; }
            set
            {
                SetPropertyValue("HostAddress", ref _SerialHostAddress, value);
            }
        }

        /// <summary>
        /// Serial channel Group
        /// </summary>
        private Byte _SerialHostGroup;
        public Byte SerialHostGroup
        {
            get { return _SerialHostGroup; }
            set
            {
                SetPropertyValue("ChannelType", ref _SerialHostGroup, value);
            }
        }
        #endregion
    }
}

