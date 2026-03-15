using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;
using DriverCodeBaseEx;

namespace ModbusTCP
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class ModbusTCPStationSettings : StationSettings
    {
                #region Constructors

        public ModbusTCPStationSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected ModbusTCPStationSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        #endregion

        public override void CopyProperties(StationSettings st)
        {
            base.CopyProperties(st);
            StationID = ((ModbusTCPStationSettings)st).StationID;
            AddressType = ((ModbusTCPStationSettings)st).AddressType;
        }

        public void DefaultSettings()
        {
            base.DefaultSettings();
            _StationID = 1;
            _AddressType = (int)AddressTypes.ZeroBased;
        }

        #region Properties

        /// <summary>
        /// Enter the numeric station address (0..247)
        /// </summary>
        private uint _StationID;
        public uint StationID
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
        /// Enter the address type (0-Based or 1-Based)
        /// </summary>
        private int _AddressType;
        public int AddressType
        {
            get
            {
                return _AddressType;
            }
            set
            {
                SetPropertyValue("AddressType", ref _AddressType, value);
            }
        }

        #endregion



        #region IDataErrorInfo Members

        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            if (propertyName == "StationID")
            {
                if(StationID < 0 || StationID > 255)
                    return Properties.Resources.StationIDOutOfRange;

            }

            return null;
        }

        #endregion

    }
}
