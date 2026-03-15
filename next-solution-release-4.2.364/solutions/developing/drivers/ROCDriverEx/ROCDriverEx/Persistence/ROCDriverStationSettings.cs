using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace ROCDriver
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class ROCDriverStationSettings : StationSettings
    {
                #region Constructors

        public ROCDriverStationSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected ROCDriverStationSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        #endregion

        public void CopyProperties(ROCDriverStationSettings st)
        {
            base.CopyProperties(st);
            _StationID = st.StationID;
            _StationGroup = st.StationGroup;
            _CheckDeviceConnection = st.CheckDeviceConnection;
            _PingPointType = st.PingPointType;
            _PingLogicalNumber = st.PingLogicalNumber;
            _PingParameter = st.PingParameter;
        }

        public void DefaultSettings()
        {
            base.DefaultSettings();
            _StationID = 240;
            _StationGroup = 240;
            _CheckDeviceConnection = false;
            _PingPointType = 0;
            _PingLogicalNumber = 0;
            _PingParameter = 0;
        }

        #region Properties


        /// <summary>
        /// Enter the numeric station address (0..255)
        /// </summary>
        private byte _StationID;
        public byte StationID
        {
            get
            {
                return _StationID;
            }
            set
            {
                SetPropertyValue("StationID", ref _StationID, value);
            }
        }

        /// <summary>
        /// Enter the numeric station group (0..255)
        /// </summary>
        private byte _StationGroup;
        public byte StationGroup
        {
            get
            {
                return _StationGroup;
            }
            set
            {
                SetPropertyValue("StationGroup", ref _StationGroup, value);
            }
        }

        /// <summary>
        /// Check device connection (optional).
        /// </summary>
        private bool _CheckDeviceConnection;
        public bool CheckDeviceConnection
        {
            get
            {
                return _CheckDeviceConnection;
            }
            set
            {
                SetPropertyValue("CheckDeviceConnection", ref _CheckDeviceConnection, value);
                RaisePropertyChangedEvent("CheckDeviceConnection");
            }
        }

        /// <summary>
        /// Ping Point Type (optional)
        /// </summary>
        private byte _PingPointType;
        public byte PingPointType
        {
            get
            {
                return _PingPointType;
            }
            set
            {
                SetPropertyValue("PingPointType", ref _PingPointType, value);
            }
        }

        /// <summary>
        /// Ping Logical Number (optional)
        /// </summary>
        private byte _PingLogicalNumber;
        public byte PingLogicalNumber
        {
            get
            {
                return _PingLogicalNumber;
            }
            set
            {
                SetPropertyValue("PingLogicalNumber", ref _PingLogicalNumber, value);
            }
        }

        /// <summary>
        /// Ping Parameter (optional)
        /// </summary>
        private byte _PingParameter;
        public byte PingParameter
        {
            get
            {
                return _PingParameter;
            }
            set
            {
                SetPropertyValue("PingParameter", ref _PingParameter, value);
            }
        }

        #endregion



        #region IDataErrorInfo Members

        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            return null;
        }

        #endregion

    }
}
