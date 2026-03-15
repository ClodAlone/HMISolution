using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;
using System.Text.RegularExpressions;


namespace CoDeSys
{

    [MapInheritance(MapInheritanceType.ParentTable)]
    public class CoDeSysChannelSettings : ChannelSettings
    {
        public enum CONNECTION : uint
        {
            GATEWAY,
            DIRECT
        };


        #region Constructors

        public CoDeSysChannelSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
            session.UpdateSchema(typeof(CoDeSysChannelSettings));
        }

        private CoDeSysChannelSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }


        #endregion

        public void DefaultSettings()
        {
            base.DefaultSettings();

            _DeviceName = string.Empty;
            _DeviceAddress = string.Empty;
            _PlcVersion = CoDeSysProtocol.PlcVersion.IDS_PLC_VER_30;
            _PlcPort = CoDeSysProtocol.DEFAULT_PORT;
            _CyclicListUdateRate = 100;
            _Motorola = false;
            _BufferSize = 0;
            //_LogIn = false;
            _Protocol = CoDeSysProtocol.Protocol.IDS_PROTOCOL_L2_ROUTE;
            _UseDirectReading = true;
            _MaxDynamicAggregation = 50;
            _UsePing = false;
            _ConnectionType = CONNECTION.DIRECT;
            _UserPLC = string.Empty;
            _PasswordPLC = string.Empty;
            _PasswordGateWay = string.Empty;

        }

        public void CopyProperties(CoDeSysChannelSettings ch)
        {
            base.CopyProperties(ch);

            DeviceName = ch.DeviceName;
            DeviceAddress = ch.DeviceAddress;
            PlcVersion = ch.PlcVersion;
            PlcPort = ch.PlcPort;
            CyclicListUdateRate = ch.CyclicListUdateRate;
            Motorola = ch.Motorola;
            BufferSize = ch.BufferSize;
            //LogIn = ch.LogIn;
            Protocol = ch.Protocol;
            UseDirectReading = ch.UseDirectReading;
            MaxDynamicAggregation = ch.MaxDynamicAggregation;
            UsePing = false;
            ConnectionType = ch.ConnectionType;
            UserPLC = ch.UserPLC;
            PasswordPLC = ch.PasswordPLC;
            PasswordGateWay = ch.PasswordGateWay;
        }
        /////////////////////////////

        #region Properties
        /// <summary>
        /// Connection Type  Example: Gateway or Direct
        /// </summary>
        private CONNECTION _ConnectionType;
        public CONNECTION ConnectionType
        {
            get
            {
                return _ConnectionType;
            }
            set
            {
                SetPropertyValue("ConnectionType", ref _ConnectionType, value);
                //OnPropertyChanged(new PropertyChangedEventArgs("DeviceName"));
                RaisePropertyChangedEvent("DeviceName");
            }
        }

        /// <summary>
        /// IP Address or name of the CoDeSys server. Empty string = local machine. Example: 192.168.0.39
        /// </summary>
        private string _DeviceName;
        public string DeviceName
        {
            get
            {
                return _DeviceName;
            }
            set
            {
                SetPropertyValue("DeviceName", ref _DeviceName, value);
            }
        }

        /// <summary>
        /// Device Address
        /// </summary>
        private string _DeviceAddress;
        public string DeviceAddress
        {
            get { return _DeviceAddress; }
            set { SetPropertyValue("DeviceAddress", ref _DeviceAddress, value); }
        }

        /// <summary>
        /// Enter the CoDeSys PLC Version
        /// </summary>
        private CoDeSysProtocol.PlcVersion _PlcVersion;
        public CoDeSysProtocol.PlcVersion PlcVersion
        {
            get { return _PlcVersion; }
            set
            {
                SetPropertyValue("PlcVersion", ref _PlcVersion, value);
                //RaisePropertyChangedEvent("Channel");
                //RaisePropertyChangedEvent("CPUSlot");
            }
        }

        /// <summary>
        /// Port Number
        /// </summary>
        private uint _PlcPort;
        public uint PlcPort
        {
            get { return _PlcPort; }
            set
            {
                SetPropertyValue("PlcPort", ref _PlcPort, value);
                //RaisePropertyChangedEvent("Channel");
                //RaisePropertyChangedEvent("PlcType");
            }
        }

        /// <summary>
        /// User PLC. Empty string or Example: PlC1
        /// </summary>
        private string _UserPLC;
        public string UserPLC
        {
            get
            {
                return _UserPLC;
            }
            set
            {
                SetPropertyValue("UserPLC", ref _UserPLC, value);
            }
        }

        /// <summary>
        /// Password PLC. Empty string or Example: Administrator
        /// </summary>
        private string _PasswordPLC;
        public string PasswordPLC
        {
            get
            {
                return _PasswordPLC;
            }
            set
            {
                SetPropertyValue("PasswordPLC", ref _PasswordPLC, value);
            }
        }

        /// <summary>
        /// Password GateWay. Empty string or Example: Administrator
        /// </summary>
        private string _PasswordGateWay;
        public string PasswordGateWay
        {
            get
            {
                return _PasswordGateWay;
            }
            set
            {
                SetPropertyValue("PasswordGateWay", ref _PasswordGateWay, value);
            }
        }

        /// <summary>
        /// Cycling update rate
        /// </summary>
        private uint _CyclicListUdateRate;
        public uint CyclicListUdateRate
        {
            get { return _CyclicListUdateRate; }
            set { SetPropertyValue("CyclicListUdateRate", ref _CyclicListUdateRate, value); }
        }

        /// <summary>
        /// Motorola byte order = default value = False
        /// </summary>
        private bool _Motorola;
        public bool Motorola
        {
            get { return _Motorola; }
            set { SetPropertyValue("Motorola", ref _Motorola, value); }
        }

        /// <summary>
        /// Communication BufferSize
        /// </summary>
        private uint _BufferSize;
        public uint BufferSize
        {
            get { return _BufferSize; }
            set { SetPropertyValue("BufferSize", ref _BufferSize, value); }
        }

        /// <summary>
        /// LogIn to PLC
        /// </summary>
        //private bool _LogIn;
        //public bool LogIn
        //{
        //    get { return _LogIn; }
        //    set { SetPropertyValue("LogIn", ref _LogIn, value); }
        //}

        /// <summary>
        /// Protocol Type
        /// </summary>
        private CoDeSysProtocol.Protocol _Protocol;
        public CoDeSysProtocol.Protocol Protocol
        {
            get { return _Protocol; }
            set { SetPropertyValue("Protocol", ref _Protocol, value); }
        }

        /// <summary>
        /// Enable to retrieve data from PLC polling device
        /// </summary>
        private bool _UseDirectReading;
        public bool UseDirectReading
        {
            get { return _UseDirectReading; }
            set { SetPropertyValue("UseDirectReading", ref _UseDirectReading, value); }
        }

        /// <summary>
        /// Task aggregation threshold limit
        /// </summary>
        private uint _MaxDynamicAggregation;
        public uint MaxDynamicAggregation
        {
            get { return _MaxDynamicAggregation; }
            set { SetPropertyValue("MaxDynamicAggregation", ref _MaxDynamicAggregation, value); }
        }

        /// <summary>
        /// Use ping to test presence of PLC 
        /// </summary>
        private bool _UsePing;
        public bool UsePing
        {
            get { return _UsePing; }
            set { SetPropertyValue("UsePing", ref _UsePing, value); }
        }


        #endregion


        #region IDataErrorInfo Members
        #endregion

        #region Overrides
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Performs the validation action. </summary>
        ///
        /// <param name="propertyName" type="String">   Name of the property. </param>
        ///
        /// <returns>   A String. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        const string ValidIpAddressRegex = @"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$";
        const string ValidHostnameRegex = @"^(([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-\_]*[a-zA-Z0-9])\.)*([A-Za-z0-9]|[A-Za-z0-9][A-Za-z0-9\-\_]*[A-Za-z0-9])$";
        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            switch (propertyName)
            {
                case "DeviceName":
                    if (_ConnectionType == CONNECTION.GATEWAY)
                    {
                        if (_DeviceName.Length == 0)
                            return Properties.Resources.EnterHostName;

                        if (!Regex.IsMatch(_DeviceName, ValidIpAddressRegex) && !Regex.IsMatch(_DeviceName, ValidHostnameRegex))
                            return Properties.Resources.InvalidHostName;
                    }
                    break;

                case "DeviceAddress":// PLC Address
                    if (string.IsNullOrEmpty(_DeviceAddress))
                        return Properties.Resources.EnterPlcAddress;
                    break;

                case "MaxDynamicAggregation":
                    if (_MaxDynamicAggregation < 0 || _MaxDynamicAggregation > 10000)
                        return Properties.Resources.MaxDynamicAggregationOutOfRange;
                    break;

                case "CyclicListUdateRate":
                    if (_CyclicListUdateRate < 10 || CyclicListUdateRate > 60000)
                        return Properties.Resources.CyclicListUdateRateOutOfRange;
                    break;
            }

            return null;
        }
        #endregion
    }
}
