using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace TwinCAT
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class TwinCATChannelSettings : ChannelSettings
    {
        #region Constructors

        public TwinCATChannelSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        private TwinCATChannelSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        #endregion

        public void DefaultSettings()
        {
            base.DefaultSettings();
            _TwinCATAdsAmsNetId = String.Empty;
            _TwinCATAdsAmsPortNum = 801;
            _TwinCATCommunicationTimeout = 0;
            _TwinCATMaxNumberOfAggregatedRequests = TwinCATProtocol.DEFAULT_REQUEST_NUMBER;
            _TwinCATVersion = (byte)TwinCATVersions.Version2x;
        }

        public void CopyProperties(TwinCATChannelSettings ch)
        {
            base.CopyProperties(ch);
            TwinCATAdsAmsNetId = ch.TwinCATAdsAmsNetId;
            TwinCATAdsAmsPortNum = ch.TwinCATAdsAmsPortNum;
            TwinCATCommunicationTimeout = ch.TwinCATCommunicationTimeout;
            TwinCATMaxNumberOfAggregatedRequests = ch.TwinCATMaxNumberOfAggregatedRequests;
            TwinCATVersion = ch.TwinCATVersion;
        }

        #region Properties

        /// <summary>
        /// Ams Net Id of the TwinCAT server. Empty string = local machine. Example: 192.168.0.39.1.1
        /// </summary>
        private string _TwinCATAdsAmsNetId;
        public string TwinCATAdsAmsNetId
        {
            get
            {
                return _TwinCATAdsAmsNetId;
            }
            set
            {
                SetPropertyValue("TwinCATAdsAmsNetId", ref _TwinCATAdsAmsNetId, value);
            }
        }

        /// <summary>
        ///  Port number of the the TwinCAT server. Default value = 801
        /// </summary>
        private uint _TwinCATAdsAmsPortNum;
        public uint TwinCATAdsAmsPortNum
        {
            get
            {
                return _TwinCATAdsAmsPortNum;
            }
            set
            {
                SetPropertyValue("TwinCATAdsAmsPortNum",
                                 ref _TwinCATAdsAmsPortNum, value);
            }
        }

        /// <summary>
        ///  Timeout for the ADS communication. Unit in ms.
        /// </summary>
        private uint _TwinCATCommunicationTimeout;
        public uint TwinCATCommunicationTimeout
        {
            get
            {
                return _TwinCATCommunicationTimeout;
            }
            set
            {
                SetPropertyValue("TwinCATCommunicationTimeout",
                                 ref _TwinCATCommunicationTimeout, value);
            }
        }

        /// <summary>
        /// Maximum number of aggregated read/write requests (<= 500).
        /// </summary>
        private uint _TwinCATMaxNumberOfAggregatedRequests;
        public uint TwinCATMaxNumberOfAggregatedRequests
        {
            get
            {
                return _TwinCATMaxNumberOfAggregatedRequests;
            }
            set
            {
                SetPropertyValue("TwinCATMaxNumberOfAggregatedRequests",
                                 ref _TwinCATMaxNumberOfAggregatedRequests,
                                 value);
            }
        }

        /// <summary>
        /// Enter the version of TwinCAT system installed on the server
        /// </summary>
        private byte _TwinCATVersion;
        public byte TwinCATVersion
        {
            get
            {
                return _TwinCATVersion;
            }
            set
            {
                SetPropertyValue("TwinCATVersion", ref _TwinCATVersion, value);
            }
        }

        #endregion

        #region IDataErrorInfo Members

        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            if (propertyName == "TwinCATMaxNumberOfAggregatedRequests")
            {
                if (TwinCATMaxNumberOfAggregatedRequests > TwinCATProtocol.MAX_REQUEST_NUMBER)
                {
                    return Properties.Resources.ErrorMaximumNumberOfAggregatedRequests;
                }
            }
            else if (propertyName == "TwinCATAdsAmsPortNum")
            {
                if (TwinCATAdsAmsPortNum == 0)
                {
                    return Properties.Resources.ErrorPortNumber;
                }
            }
            else if(propertyName == "TwinCATVersion")
            {

            }

            return null;
        }

        #endregion

    }
}
