using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace PPI
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class PPIStationSettings : StationSettings
    {
        #region Constructors

        public PPIStationSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected PPIStationSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        #endregion

        public override void CopyProperties(StationSettings st)
        {
            base.CopyProperties(st);
            _StationAddress = ((PPIStationSettings)st)._StationAddress;
        }

        public void DefaultSettings()
        {
            base.DefaultSettings();
            _StationAddress = 2;
        }

        #region Properties
        private byte _StationAddress;
        public byte StationAddress
        {
            get { return _StationAddress; }
            set { SetPropertyValue("StationAddress", ref _StationAddress, value); ; }
        }
        #endregion

        #region IDataErrorInfo Members

        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            if (propertyName == "StationAddress")
            {
                if (_StationAddress > PPIProtocol.PPI_MAX_ID || _StationAddress < PPIProtocol.PPI_MIN_ID)
                    return Properties.Resources.ErrorInvalidPPIAddress;
            }

            return null;
        }

        #endregion

    }
}
