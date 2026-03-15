using System;
using DriverCodeBaseEx;
using DevExpress.Xpo;

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
            _snmpOid_Address = job.snmpOid_Address;
            _snmpCommunity = job.snmpCommunity;
            _snmpTrapOnly = job.snmpTrapOnly;
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
            _snmpOid_Address = String.Empty;
            _snmpCommunity = SNMPProtocol.SNMP_COMUNITY_PUBLIC;
            _snmpTrapOnly = false;
        }

        #region Properties

        private SNMPDATATYPE _snmpDataType;
        public SNMPDATATYPE snmpDataType
        {
            get { return _snmpDataType; }
            set { SetPropertyValue("snmpDataType", ref _snmpDataType, value); }
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

        private bool _snmpTrapOnly;
        public bool snmpTrapOnly
        {
            get { return _snmpTrapOnly; }
            set { SetPropertyValue("snmpTrapOnly", ref _snmpTrapOnly, value); }
        }
        #endregion
    }
}
