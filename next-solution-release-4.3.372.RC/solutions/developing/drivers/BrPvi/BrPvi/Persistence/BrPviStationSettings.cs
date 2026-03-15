using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;

namespace BrPvi
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class BrPviStationSettings : StationSettings
    {
        #region Constructors

        public BrPviStationSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected BrPviStationSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        #endregion

        public void CopyProperties(BrPviStationSettings st)
        {
            base.CopyProperties(st);
            _BrPviRoutingPath = st.BrPviRoutingPath;
            _BrPviResponseTimeout = st.BrPviResponseTimeout;
            _BrPviInterfaceType = st._BrPviInterfaceType;
            _BrPviSourceStationPort = st.BrPviSourceStationPort;
            _BrPviSourceStationID = st.BrPviSourceStationID;
            _BrPviDestinationStationIPAddress = st.BrPviDestinationStationIPAddress;
            _BrPviDestinationStationPort = st._BrPviDestinationStationPort;
            _BrPviDestinationStationID = st.BrPviDestinationStationID;
            _BrPviSerialPort = st.BrPviSerialPort;
            _BrPviSerialBaudrate = st.BrPviSerialBaudrate;
            _BrPviSerialParity = st.BrPviSerialParity;
            _BrPviSerialFlowControl = st.BrPviSerialFlowControl;
        }

        public void DefaultSettings()
        {
            base.DefaultSettings();
            _BrPviRoutingPath = String.Empty;
            _BrPviResponseTimeout = 275;
            _BrPviInterfaceType = InterfaceTypes.Ethernet;
            _BrPviSourceStationPort = 11159;
            _BrPviSourceStationID = 1;
            _BrPviDestinationStationIPAddress = String.Empty;
            _BrPviDestinationStationPort = 11159;
            _BrPviDestinationStationID = 2;
            _BrPviSerialPort = 1;
            _BrPviSerialBaudrate = 57600;
            _BrPviSerialParity = 2;
            _BrPviSerialFlowControl = 2;
            //set as default value 0 so station will go on error immediatly; this driver recive event of change (state/value) from PVI Server (like OPC Client) and not polling directly device
            base.MaxRetriesBeforeError = 0;
        }

        #region Properties

        /// <summary>
        /// Enter the Routing Path 
        /// </summary>
        private string _BrPviRoutingPath;
        public string BrPviRoutingPath
        {
            get
            {
                return _BrPviRoutingPath;
            }
            set
            {
                SetPropertyValue("BrPviRoutingPath", ref _BrPviRoutingPath, value);
            }
        }

        /// <summary>
        /// Enter the Response Timeout 
        /// </summary>
        private int _BrPviResponseTimeout;
        public int BrPviResponseTimeout
        {
            get
            {
                return _BrPviResponseTimeout;
            }
            set
            {
                SetPropertyValue("BrPviResponseTimeout", ref _BrPviResponseTimeout, value);
            }
        }

        /// <summary>
        /// Select the Interface Type 
        /// </summary>
        private InterfaceTypes _BrPviInterfaceType;
        public InterfaceTypes BrPviInterfaceType
        {
            get
            {
                return _BrPviInterfaceType;
            }
            set
            {
                SetPropertyValue("BrPviInterfaceType", ref _BrPviInterfaceType, value);
            }
        }

        /// <summary>
        /// Enter the Source Station Port 
        /// </summary>
        private short _BrPviSourceStationPort;
        public short BrPviSourceStationPort
        {
            get
            {
                return _BrPviSourceStationPort;
            }
            set
            {
                SetPropertyValue("BrPviSourceStationPort", ref _BrPviSourceStationPort, value);
            }
        }

        /// <summary>
        /// Enter the Source Station ID 
        /// </summary>
        private byte _BrPviSourceStationID;
        public byte BrPviSourceStationID
        {
            get
            {
                return _BrPviSourceStationID;
            }
            set
            {
                SetPropertyValue("BrPviSourceStationID", ref _BrPviSourceStationID, value);
            }
        }

        /// <summary>
        /// Enter the Destination Station IP Address 
        /// </summary>
        private string _BrPviDestinationStationIPAddress;
        public string BrPviDestinationStationIPAddress
        {
            get
            {
                return _BrPviDestinationStationIPAddress;
            }
            set
            {
                SetPropertyValue("BrPviDestinationStationIPAddress", ref _BrPviDestinationStationIPAddress, value);
            }
        }

        /// <summary>
        /// Enter the Destination Station Port 
        /// </summary>
        private short _BrPviDestinationStationPort;
        public short BrPviDestinationStationPort
        {
            get
            {
                return _BrPviDestinationStationPort;
            }
            set
            {
                SetPropertyValue("BrPviDestinationStationPort", ref _BrPviDestinationStationPort, value);
            }
        }

        /// <summary>
        /// Enter the Destination Station ID 
        /// </summary>
        private byte _BrPviDestinationStationID;
        public byte BrPviDestinationStationID
        {
            get
            {
                return _BrPviDestinationStationID;
            }
            set
            {
                SetPropertyValue("BrPviDestinationStationID", ref _BrPviDestinationStationID, value);
            }
        }

        /// <summary>
        /// Enter the Serial Port 
        /// </summary>
        private byte _BrPviSerialPort;
        public byte BrPviSerialPort
        {
            get
            {
                return _BrPviSerialPort;
            }
            set
            {
                SetPropertyValue("BrPviSerialPort", ref _BrPviSerialPort, value);
            }
        }

        /// <summary>
        /// Select the Serial Baudrate 
        /// </summary>
        private int _BrPviSerialBaudrate;
        public int BrPviSerialBaudrate
        {
            get
            {
                return _BrPviSerialBaudrate;
            }
            set
            {
                SetPropertyValue("BrPviSerialBaudrate", ref _BrPviSerialBaudrate, value);
            }
        }

        /// <summary>
        /// Select the Serial Parity 
        /// </summary>
        private int _BrPviSerialParity;
        public int BrPviSerialParity
        {
            get
            {
                return _BrPviSerialParity;
            }
            set
            {
                SetPropertyValue("BrPviSerialParity", ref _BrPviSerialParity, value);
            }
        }

        /// <summary>
        /// Select the Serial Flow Control 
        /// </summary>
        private int _BrPviSerialFlowControl;
        public int BrPviSerialFlowControl
        {
            get
            {
                return _BrPviSerialFlowControl;
            }
            set
            {
                SetPropertyValue("BrPviSerialFlowControl", ref _BrPviSerialFlowControl, value);
            }
        }

        #endregion

        #region IDataErrorInfo Members

        protected String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            return null;
        }

        #endregion

    }
}
