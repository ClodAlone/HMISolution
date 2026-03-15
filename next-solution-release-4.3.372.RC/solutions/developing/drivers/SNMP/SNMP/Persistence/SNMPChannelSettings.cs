using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IpDriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;

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
            _snmpMaxNumberOfAggregatedRequests = 1;
        }

        public void CopyProperties(SNMPChannelSettings ch)
        {
            base.CopyProperties(ch);
            _snmpVersion = ch.snmpVersion;
            snmpMaxNumberOfAggregatedRequests = ch.snmpMaxNumberOfAggregatedRequests;
        }
        /////////////////////////////

        #region Properties

        /// <summary>   Gets or sets the value of the version of the SNMP protocol to be used. </summary>
        private SNMPVERSION _snmpVersion;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the value of the version of the SNMP protocol to be used. </summary>
        ///
        /// <value> SNMPv1 or SNMPv2c. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public SNMPVERSION snmpVersion
        {
            get { return _snmpVersion; }
            set
            {
                SetPropertyValue("snmpVersion", ref _snmpVersion, value);
            }
        }

        /// <summary>
        /// Maximum number of aggregated read/write requests (<= 500).
        /// </summary>
        private uint _snmpMaxNumberOfAggregatedRequests;
        public uint snmpMaxNumberOfAggregatedRequests
        {
            get
            {
                return _snmpMaxNumberOfAggregatedRequests;
            }
            set
            {
                SetPropertyValue("snmpMaxNumberOfAggregatedRequests",
                                 ref _snmpMaxNumberOfAggregatedRequests,
                                 value);
            }
        }

        #endregion


        #region IDataErrorInfo Members

        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            if (propertyName == "snmpVersion")
            {
                if ((_snmpVersion != SNMPVERSION.SNMPv1) && (_snmpVersion != SNMPVERSION.SNMPv2c))
                {
                    return Properties.Resources.SNMPUnsupportedProtocolVersion;
                }
            }
            else if (propertyName == "snmpMaxNumberOfAggregatedRequests")
            {
                if (snmpMaxNumberOfAggregatedRequests > SNMPChannel.MAX_REQUEST_NUMBER)
                {
                    return Properties.Resources.SNMPErrorMaximumNumberOfAggregatedRequests;
                }
            }

            return null;
        }

        #endregion
    
    }
}
