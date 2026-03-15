using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace BrPvi
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class BrPviChannelSettings : ChannelSettings
    {
        #region Constructors

        public BrPviChannelSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
            session.UpdateSchema(typeof(BrPviChannelSettings));
        }

        private BrPviChannelSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        #endregion

        public void DefaultSettings()
        {
            base.DefaultSettings();
            _BrPviServerAddress = String.Empty;
            _BrPviServerPort = 20000;
            _BrPviServerCommunicationTimeout = 10;
            _BrPviRetryTime = 0;
            //_BrPviMaxNumberOfAggregatedRequests = 1;
            _BrPviVersion = (byte)BrPviProcol.BrPviVersions.Version2x;
            _BrPviMaxNumberSubscriptions = 10;
        }

        public void CopyProperties(BrPviChannelSettings ch)
        {
            base.CopyProperties(ch);
            BrPviServerAddress = ch.BrPviServerAddress;
            BrPviServerPort = ch.BrPviServerPort;
            BrPviServerCommunicationTimeout = ch.BrPviServerCommunicationTimeout;
            BrPviRetryTime = ch.BrPviRetryTime;
            //BrPviMaxNumberOfAggregatedRequests = ch.BrPviMaxNumberOfAggregatedRequests;
            BrPviVersion = ch.BrPviVersion;
            BrPviMaxNumberSubscriptions = ch.BrPviMaxNumberSubscriptions;
        }

        #region Properties

        /// <summary>
        /// IP Address or name of the PVI server. Empty string = local machine. Example: 192.168.0.39
        /// </summary>
        private string _BrPviServerAddress;
        public string BrPviServerAddress
        {
            get
            {
                return _BrPviServerAddress;
            }
            set
            {
                SetPropertyValue("BrPviServerAddress", ref _BrPviServerAddress, value);
            }
        }

        /// <summary>
        ///  Port number of the PVI server server. Default value = 20000
        /// </summary>
        private uint _BrPviServerPort;
        public uint BrPviServerPort
        {
            get
            {
                return _BrPviServerPort;
            }
            set
            {
                SetPropertyValue("BrPviServerPort",
                                 ref _BrPviServerPort, value);
            }
        }

        /// <summary>
        ///  Timeout for the communication with the PVI server. Unit is sec.
        /// </summary>
        private uint _BrPviServerCommunicationTimeout;
        public uint BrPviServerCommunicationTimeout
        {
            get
            {
                return _BrPviServerCommunicationTimeout;
            }
            set
            {
                SetPropertyValue("BrPviServerCommunicationTimeout",
                                 ref _BrPviServerCommunicationTimeout, value);
            }
        }

        /// <summary>
        ///  Retry Time for user messages. Unit is sec.
        /// </summary>
        private uint _BrPviRetryTime;
        public uint BrPviRetryTime
        {
            get
            {
                return _BrPviRetryTime;
            }
            set
            {
                SetPropertyValue("BrPviRetryTime",
                                 ref _BrPviRetryTime, value);
            }
        }

        ///// <summary>
        ///// Maximum number of aggregated read/write requests (<= 500).
        ///// </summary>
        //private uint _BrPviMaxNumberOfAggregatedRequests;
        //public uint BrPviMaxNumberOfAggregatedRequests
        //{
        //    get
        //    {
        //        return _BrPviMaxNumberOfAggregatedRequests;
        //    }
        //    set
        //    {
        //        SetPropertyValue("BrPviMaxNumberOfAggregatedRequests",
        //                         ref _BrPviMaxNumberOfAggregatedRequests,
        //                         value);
        //    }
        //}

        /// <summary>
        /// Enter the version of BrPvi system installed on the server
        /// </summary>
        private byte _BrPviVersion;
        public byte BrPviVersion
        {
            get
            {
                return _BrPviVersion;
            }
            set
            {
                SetPropertyValue("BrPviVersion", ref _BrPviVersion, value);
            }
        }

        /// <summary>
        /// Nr of tag's subscription at time
        /// </summary>
        private uint? _BrPviMaxNumberSubscriptions;
        public uint? BrPviMaxNumberSubscriptions
        {
            get
            {
                return _BrPviMaxNumberSubscriptions;
            }
            set
            {
                SetPropertyValue("BrPviMaxNumberSubscriptions", ref _BrPviMaxNumberSubscriptions, value);
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
                case "BrPviServerPort":
                    if (_BrPviServerPort == 0)
                    {
                        return Properties.Resources.ErrorPortNumber;
                    }
                    break;
                case "BrPviServerCommunicationTimeout":
                    if (_BrPviServerCommunicationTimeout > 3600)
                    {
                        return Properties.Resources.ErrorInvalidTimeout;
                    }
                    break;
                case "BrPviRetryTime":                    
                    if (_BrPviRetryTime > 3600)
                    {
                        return Properties.Resources.ErrorInvalidRetryTime;
                    }
                    break;
                case "BrPviMaxNumberSubscriptions":
                    if (_BrPviMaxNumberSubscriptions <=0 || _BrPviMaxNumberSubscriptions>100)
                    {
                        return string.Format(Properties.Resources.ErroroMaxNumberSubscriptions, 100);
                    }
                    break;
            }
            return null;
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();

            EnsureDefaultValues();
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();

            EnsureDefaultValues();
        }

        #endregion

        #region Properties Default Values
        /// <summary>
        /// Adds inside this method the nullable property where you want handle a default value.
        /// </summary>
        private void EnsureDefaultValues()
        {
            if (!_BrPviMaxNumberSubscriptions.HasValue)
                _BrPviMaxNumberSubscriptions = 5;
        }
        #endregion

    }
}
