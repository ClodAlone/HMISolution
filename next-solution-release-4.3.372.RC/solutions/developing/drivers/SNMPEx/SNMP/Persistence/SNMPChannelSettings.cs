using System;
using IpDriverCodeBaseEx;
using DevExpress.Xpo;
using DriverCodeBaseEx;

namespace SNMP
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class SNMPChannelSettings : UdpChannelSettings
    {
                #region Constructors

        public SNMPChannelSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
            session.UpdateSchema(typeof(UdpChannelSettings));
        }

        private SNMPChannelSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        
        #endregion

        public void DefaultSettings()
        {
            base.DefaultSettings();
            UdpChannelSettingsHostPort = 161;
            _snmpVersion = SNMPVERSION.SNMPv1;
        }

        public override void CopyProperties(ChannelSettings ch)
        {
            base.CopyProperties(ch);
            _snmpVersion = ((SNMPChannelSettings)ch).snmpVersion;
        }
        /////////////////////////////

        #region Properties

        /// <summary>   Gets or sets the value of the version of the SNMP protocol to be used. </summary>
        private SNMPVERSION _snmpVersion;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the value of the version of the SNMP protocol to be used. </summary>
        ///
        /// <value> SNMPv1 or SNMPv2c or or SNMPv3.</value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public SNMPVERSION snmpVersion
        {
            get { return _snmpVersion; }
            set
            {
                SetPropertyValue("snmpVersion", ref _snmpVersion, value);
            }
        }

        #endregion

        #region IDataErrorInfo Members
        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            switch (propertyName)
            {
                case "snmpVersion":
                    if (!Enum.IsDefined(typeof(SNMPVERSION), snmpVersion))
                        return Properties.Resources.SNMPUnsupportedProtocolVersion;
                    break;
                //case "snmpMaxNumberOfAggregatedRequests":
                //    if (snmpMaxNumberOfAggregatedRequests > SNMPProtocol.MAX_REQUEST_NUMBER)
                //        return Properties.Resources.SNMPErrorMaximumNumberOfAggregatedRequests;
                //    break;
                case "snmpEnterpriseNumber":
                    break;
                    //    if (snmpMaxNumberOfAggregatedRequests > SNMPProtocol.MAX_REQUEST_NUMBER)
                    //        return Properties.Resources.SNMPErrorMaximumNumberOfAggregatedRequests;
            }

            return null;
        }

        #endregion
    
    }
}
