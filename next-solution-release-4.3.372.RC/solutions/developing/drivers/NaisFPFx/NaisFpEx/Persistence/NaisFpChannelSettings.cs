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

namespace NaisFp
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class NaisFpChannelSettings : ChannelSettings
    {
        #region Constructors

        public NaisFpChannelSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        private NaisFpChannelSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        
        #endregion

        public override void CopyProperties(ChannelSettings ch)
        {
            base.CopyProperties(ch);

            ChannelType = ((NaisFpChannelSettings)ch).ChannelType;
            SupervisorID = ((NaisFpChannelSettings)ch).SupervisorID;

            TcpChannelSettingsHostName = ((NaisFpChannelSettings)ch).TcpChannelSettingsHostName;
            TcpChannelSettingsHostPort = ((NaisFpChannelSettings)ch).TcpChannelSettingsHostPort;
            TcpChannelSettingsReadTimeout = ((NaisFpChannelSettings)ch).TcpChannelSettingsReadTimeout;
            TcpChannelSettingsWriteTimeout = ((NaisFpChannelSettings)ch).TcpChannelSettingsWriteTimeout;
            TcpChannelSettingsBackupHostName = ((NaisFpChannelSettings)ch).TcpChannelSettingsBackupHostName;
            TcpChannelSettingsSwitchHostTimeout = ((NaisFpChannelSettings)ch).TcpChannelSettingsSwitchHostTimeout;
            FP2ETLAN = ((NaisFpChannelSettings)ch).FP2ETLAN;

            CommPortName = ((NaisFpChannelSettings)ch).CommPortName;
            CommPortNameLinux = ((NaisFpChannelSettings)ch).CommPortNameLinux;
            CommPortBaudRate = ((NaisFpChannelSettings)ch).CommPortBaudRate; 
            CommPortDataBits = ((NaisFpChannelSettings)ch).CommPortDataBits;
            CommPortParity = ((NaisFpChannelSettings)ch).CommPortParity;
            CommPortStopBits = ((NaisFpChannelSettings)ch).CommPortStopBits; 
            CommPortHandshake = ((NaisFpChannelSettings)ch).CommPortHandshake;
            CommPortRtsEnable = ((NaisFpChannelSettings)ch).CommPortRtsEnable;
            CommPortDtrEnable = ((NaisFpChannelSettings)ch).CommPortDtrEnable;
            CommPortReadTimeout = ((NaisFpChannelSettings)ch).CommPortReadTimeout;   
            CommPortWriteTimeout = ((NaisFpChannelSettings)ch).CommPortWriteTimeout;
            USBconnection = ((NaisFpChannelSettings)ch).USBconnection;

        }

        public void DefaultSettings()
        {
            base.DefaultSettings();
            ChannelType = ChannelTypes.Socket;
            SupervisorID = 0;

            TcpChannelSettingsHostName = "";
            TcpChannelSettingsHostPort = 5050;
            TcpChannelSettingsReadTimeout = 2000;
            TcpChannelSettingsWriteTimeout = 2000;
            TcpChannelSettingsBackupHostName = "";
            TcpChannelSettingsSwitchHostTimeout = 10000;
            FP2ETLAN = false;

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

        /// <summary>
        /// Enter the numeric ID for the PC
        /// </summary>
        private byte _SupervisorID;
        public byte SupervisorID
        {
            get
            {
                return _SupervisorID;
            }
            set
            {
                SetPropertyValue("SupervisorID", ref _SupervisorID, value);
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

        /// <summary>
        /// FP2 ET-LAN unit
        /// </summary>
        private bool _FP2ETLAN;
        public bool FP2ETLAN
        {
            get
            {
                return _FP2ETLAN;
            }
            set
            {
                SetPropertyValue("FP2ETLAN", ref _FP2ETLAN, value);
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

        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            if (propertyName == "SupervisorID")
            {
                if (SupervisorID > 99)
                    return Properties.Resources.SupervisorIDOutOfRange;

            }

            if (_ChannelType == ChannelTypes.Serial)
            {
                switch (propertyName)
                {
                    case "CommPortName":
                        if (DriverSettings != null && ((from c in DriverSettings.ChannelSettings/*.AsParallel()*/
                                                        where c != this && (c as NaisFpChannelSettings).CommPortName == CommPortName &&
                                                        (c as NaisFpChannelSettings).ChannelType == ChannelTypes.Serial
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

    public class NaisFpTcpChannelSettings : TcpChannelSettings
    {
        #region Constructors

        protected NaisFpTcpChannelSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public NaisFpTcpChannelSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }


        #endregion

        public override void CopyProperties(ChannelSettings ch)
        {
            base.CopyProperties(ch);
            KeepOpened = true;
            TcpChannelSettingsHostName = ((NaisFpChannelSettings)ch).TcpChannelSettingsHostName;
            TcpChannelSettingsHostPort = ((NaisFpChannelSettings)ch).TcpChannelSettingsHostPort;
            TcpChannelSettingsBackupHostName = ((NaisFpChannelSettings)ch).TcpChannelSettingsBackupHostName;
            TcpChannelSettingsSwitchHostTimeout = ((NaisFpChannelSettings)ch).TcpChannelSettingsSwitchHostTimeout;
        }
    }

    public class NaisFpSerialChannelSettings : SerialChannelSettings
    {
        #region Constructors

        protected NaisFpSerialChannelSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public NaisFpSerialChannelSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

        public override void CopyProperties(ChannelSettings ch)
        {
            base.CopyProperties(ch);
            CommPortName = ((NaisFpChannelSettings)ch).CommPortName;
            CommPortNameLinux = ((NaisFpChannelSettings)ch).CommPortNameLinux;
            CommPortBaudRate = ((NaisFpChannelSettings)ch).CommPortBaudRate;
            CommPortDataBits = ((NaisFpChannelSettings)ch).CommPortDataBits;
            CommPortParity = ((NaisFpChannelSettings)ch).CommPortParity;
            CommPortStopBits = ((NaisFpChannelSettings)ch).CommPortStopBits;
            CommPortHandshake = ((NaisFpChannelSettings)ch).CommPortHandshake;
            CommPortRtsEnable = ((NaisFpChannelSettings)ch).CommPortRtsEnable;
            CommPortDtrEnable = ((NaisFpChannelSettings)ch).CommPortDtrEnable;
            CommPortReadTimeout = ((NaisFpChannelSettings)ch).CommPortReadTimeout;
            CommPortWriteTimeout = ((NaisFpChannelSettings)ch).CommPortWriteTimeout;
        }

    }
}

