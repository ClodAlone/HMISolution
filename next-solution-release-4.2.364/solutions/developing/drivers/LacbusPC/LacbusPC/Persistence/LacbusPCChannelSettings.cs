using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;

namespace LacbusPC
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class LacbusPCChannelSettings : ChannelSettings
    {
        #region Constructors

        public LacbusPCChannelSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        private LacbusPCChannelSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        #endregion

        public void DefaultSettings()
        {
            base.DefaultSettings();
            _LacbusPCHostName = String.Empty;
            _LacbusPCHostPort = 1503;
            _LacbusPCConnectionTimeout = 10000;
            _LacbusPCCommunicationTimeout = 0;
            _LacbusPCMaxNumberOfAggregatedRequests = 1;
            _LacbusPCPCNumber = 1;
            _LacbusPCBackupHostName = String.Empty;
            ScheduleTimeJobsList = 10;
            PollingTimeInError = 30000;
            _LacbusPCTimeZone = String.Empty;
            LacbusPCMaxPendingRTUPollReq = 3;
        }

        public void CopyProperties(LacbusPCChannelSettings ch)
        {
            base.CopyProperties(ch);
            LacbusPCHostName = ch.LacbusPCHostName;
            LacbusPCHostPort = ch.LacbusPCHostPort;
            LacbusPCConnectionTimeout = ch.LacbusPCConnectionTimeout;
            LacbusPCCommunicationTimeout = ch.LacbusPCCommunicationTimeout;
            LacbusPCMaxNumberOfAggregatedRequests = ch.LacbusPCMaxNumberOfAggregatedRequests;
            LacbusPCPCNumber = ch.LacbusPCPCNumber;
            LacbusPCHostName = ch.LacbusPCHostName;
            LacbusPCBackupHostName = ch.LacbusPCBackupHostName;
            LacbusPCTimeZone = ch.LacbusPCTimeZone;
            LacbusPCMaxPendingRTUPollReq = ch.LacbusPCMaxPendingRTUPollReq;
        }

        #region Properties

        /// <summary>
        /// Host name (or IP address) of the LacbusPC server. Empty string = local machine. Example: 192.168.0.39
        /// </summary>
        private string _LacbusPCHostName;
        public string LacbusPCHostName
        {
            get
            {
                return _LacbusPCHostName;
            }
            set
            {
                SetPropertyValue("LacbusPCHostName", ref _LacbusPCHostName, value);
            }
        }

        /// <summary>
        ///  Port number of the the LacbusPC server. Default value = 801
        /// </summary>
        private uint _LacbusPCHostPort;
        public uint LacbusPCHostPort
        {
            get
            {
                return _LacbusPCHostPort;
            }
            set
            {
                SetPropertyValue("LacbusPCHostPort",
                                 ref _LacbusPCHostPort, value);
            }
        }

        /// <summary>
        ///  Timeout for the connection to the server. Unit in ms.
        /// </summary>
        private uint _LacbusPCConnectionTimeout;
        public uint LacbusPCConnectionTimeout
        {
            get
            {
                return _LacbusPCConnectionTimeout;
            }
            set
            {
                SetPropertyValue("LacbusPCConnectionTimeout",
                                 ref _LacbusPCConnectionTimeout, value);
            }
        }

        /// <summary>
        ///  Timeout for the ADS communication. Unit in ms.
        /// </summary>
        private uint _LacbusPCCommunicationTimeout;
        public uint LacbusPCCommunicationTimeout
        {
            get
            {
                return _LacbusPCCommunicationTimeout;
            }
            set
            {
                SetPropertyValue("LacbusPCCommunicationTimeout",
                                 ref _LacbusPCCommunicationTimeout, value);
            }
        }

        /// <summary>
        /// Maximum number of aggregated read/write requests (<= 500).
        /// </summary>
        private uint _LacbusPCMaxNumberOfAggregatedRequests;
        public uint LacbusPCMaxNumberOfAggregatedRequests
        {
            get
            {
                return _LacbusPCMaxNumberOfAggregatedRequests;
            }
            set
            {
                SetPropertyValue("LacbusPCMaxNumberOfAggregatedRequests",
                                 ref _LacbusPCMaxNumberOfAggregatedRequests,
                                 value);
            }
        }

        /// <summary>
        ///PC Number
        /// </summary>
        private byte _LacbusPCPCNumber;
        public byte LacbusPCPCNumber
        {
            get
            {
                return _LacbusPCPCNumber;
            }
            set
            {
                SetPropertyValue("LacbusPCPCNumber", ref _LacbusPCPCNumber, value);
            }
        }

        /// <summary>
        /// Host name (or IP address) of the LacbusPC backup server. Empty string = local machine. Example: 192.168.0.39
        /// </summary>
        private string _LacbusPCBackupHostName;
        public string LacbusPCBackupHostName
        {
            get
            {
                return _LacbusPCBackupHostName;
            }
            set
            {
                SetPropertyValue("LacbusPCBackupHostName", ref _LacbusPCBackupHostName, value);
            }
        }

        /// <summary>
        /// Time Zone for this channel 
        /// </summary>
        private string _LacbusPCTimeZone;
        [Category("Device Data")]
        [Description("LacbusPCTimeZone")]
        [Size(SizeAttribute.Unlimited)]
        public string LacbusPCTimeZone
        {
            get
            {
                return _LacbusPCTimeZone;
            }
            set
            {
                SetPropertyValue("LacbusPCTimeZone", ref _LacbusPCTimeZone, value);
            }
        }

        /// <summary>
        /// Maximum number of pending "POLL RTU" requests
        /// </summary>
        private byte _LacbusPCMaxPendingRTUPollReq;
        public byte LacbusPCMaxPendingRTUPollReq
        {
            get
            {
                return _LacbusPCMaxPendingRTUPollReq;
            }
            set
            {
                SetPropertyValue("LacbusPCMaxPendingRTUPollReq", ref _LacbusPCMaxPendingRTUPollReq, value);
            }
        }

        #endregion

        #region IDataErrorInfo Members

        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            if (propertyName == "LacbusPCPCNumber")
            {
                if ((LacbusPCPCNumber > 4) || (LacbusPCPCNumber < 1))
                {
                    return Properties.Resources.ErrorInvalidPCNumber;
                }
            }
            //else if (propertyName == "LacbusPCHostPort")
            //{
            //    if (LacbusPCHostPort == 0)
            //    {
            //        return Properties.Resources.ErrorPortNumber;
            //    }
            //}
            //else if(propertyName == "LacbusPCConnectionTimeout")
            //{
            //    if(LacbusPCConnectionTimeout < 5000)
            //    {
            //        return Properties.Resources.ErrorConnectionTimeout;
            //    }
            //}

            return null;
        }

        #endregion

    }
}
