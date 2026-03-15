using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;

namespace SNMP
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class SNMPCommJobSettings : CommJobSettings
    {
                #region Constructors

        public SNMPCommJobSettings(Session session, SNMPCommJob job)
            : base(session, job)
        {
            _snmpDataType = job.snmpDataType;
            _snmpDataSize = job.snmpDataSize;
            _snmpOid_Address = job.snmpOid_Address;
            _snmpCommunity = job.snmpCommunity;
        }
        
        public SNMPCommJobSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        protected SNMPCommJobSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

        public void DefaultSettings()
        {
            base.DefaultSettings();
            _snmpDataType = SNMPDATATYPE.Integer;
            _snmpDataSize = 4;
            _snmpOid_Address = String.Empty;
            _snmpCommunity = "public";
        }

        #region Properties

        private SNMPDATATYPE _snmpDataType;
        public SNMPDATATYPE snmpDataType
        {
            get { return _snmpDataType; }
            set { SetPropertyValue("snmpDataType", ref _snmpDataType, value); }
        }

        private UInt32 _snmpDataSize;
        public UInt32 snmpDataSize
        {
            get { return _snmpDataSize; }
            set { SetPropertyValue("snmpDataSize", ref _snmpDataSize, value); }
        }

        private string _snmpOid_Address;
        public string snmpOid_Address
        {
            get { return _snmpOid_Address; }
            set { SetPropertyValue("snmpOid_Address", ref _snmpOid_Address, value); }
        }

        private string _snmpCommunity;
        public string snmpCommunity
        {
            get { return _snmpCommunity; }
            set { SetPropertyValue("snmpCommunity", ref _snmpCommunity, value); }
        }

        #endregion


        #region IDataErrorInfo Members
        #endregion
    
    }
}
