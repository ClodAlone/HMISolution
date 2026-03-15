using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;

namespace EIB
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class EIBCommJobSettings : CommJobSettings
    {
        #region Constructors

        public EIBCommJobSettings(Session session, EIBCommJob job)
            : base(session, job)
        {
            _EnablePolling = job.EnablePolling;
            //_EnableOnlyInitialPolling = job.EnableOnlyInitialPolling;
            _RetryInitialPolling = job.RetryInitialPolling;
            _PollingOnlyOnRequest = job.PollingOnlyOnRequest;
            _OutputOnlyOnRequest = job.OutputOnlyOnRequest;
            _AutoResetNewDataTime = job.AutoResetNewDataTime;
            _RetryOutput = job.RetryOutput;
            _InputGroups = job.InputGroups;
            _PollingGroup = job.PollingGroup;
            _OutputGroup = job.OutputGroup;
            _DataFormat = job.DataFormat;
            _PollingTime = job.PollingTime;
            _EnableOnlyInitialPolling = true;
            if(_EnablePolling && (_PollingTime > 0))
            {
                _EnableOnlyInitialPolling = false;
            }
        }
        
        public EIBCommJobSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        protected EIBCommJobSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

        public void DefaultSettings()
        {
            base.DefaultSettings();
            _EnablePolling = true;
            _EnableOnlyInitialPolling = true;
            _RetryInitialPolling = false;
            _PollingOnlyOnRequest = false;
            _OutputOnlyOnRequest = false;
            _AutoResetNewDataTime = 0;
            _RetryOutput = false;
            _InputGroups = String.Empty;
            _PollingGroup = String.Empty;
            _OutputGroup = String.Empty;
            _DataFormat = (int)EISDATAFORMAT.EISDFBit;
            _PollingTime = 0;
        }

        #region Properties

        /// <summary>
        /// Enable Polling
        /// </summary>
        private bool _EnablePolling;
        public bool EnablePolling
        {
            get
            {
                return _EnablePolling;
            }

            set
            {
                SetPropertyValue("EnablePolling", ref _EnablePolling, value);
            }
        }

        /// <summary>
        /// Enable Only Initial Polling
        /// </summary>
        private bool _EnableOnlyInitialPolling;
        public bool EnableOnlyInitialPolling
        {
            get
            {
                return _EnableOnlyInitialPolling;
            }

            set
            {
                SetPropertyValue("EnableOnlyInitialPolling", ref _EnableOnlyInitialPolling, value);
            }
        }

        /// <summary>
        /// Retry Initial Polling in Case of Error
        /// </summary>
        private bool _RetryInitialPolling;
        public bool RetryInitialPolling
        {
            get
            {
                return _RetryInitialPolling;
            }

            set
            {
                SetPropertyValue("RetryInitialPolling", ref _RetryInitialPolling, value);
            }
        }

        /// <summary>
        /// Polling Only On Request
        /// </summary>
        private bool _PollingOnlyOnRequest;
        public bool PollingOnlyOnRequest
        {
            get
            {
                return _PollingOnlyOnRequest;
            }

            set
            {
                SetPropertyValue("PollingOnlyOnRequest", ref _PollingOnlyOnRequest, value);
            }
        }

        /// <summary>
        /// Output Only On Request
        /// </summary>
        private bool _OutputOnlyOnRequest;
        public bool OutputOnlyOnRequest
        {
            get
            {
                return _OutputOnlyOnRequest;
            }

            set
            {
                SetPropertyValue("OutputOnlyOnRequest", ref _OutputOnlyOnRequest, value);
            }
        }

        /// <summary>
        /// Auto Reset "New Data" Notification Time
        /// </summary>
        private uint _AutoResetNewDataTime;
        public uint AutoResetNewDataTime
        {
            get
            {
                return _AutoResetNewDataTime;
            }

            set
            {
                SetPropertyValue("AutoResetNewDataTime", ref _AutoResetNewDataTime, value);
            }
        }

        /// <summary>
        /// Retry Output in Case of Error
        /// </summary>
        private bool _RetryOutput;
        public bool RetryOutput
        {
            get
            {
                return _RetryOutput;
            }

            set
            {
                SetPropertyValue("RetryOutput", ref _RetryOutput, value);
            }
        }

        /// <summary>
        /// Input Group List
        /// </summary>
        private string _InputGroups;
        public string InputGroups
        {
            get
            {
                return _InputGroups;
            }

            set
            {
                SetPropertyValue("InputGroups", ref _InputGroups, value);
            }
        }

        /// <summary>
        /// Polling Group
        /// </summary>
        private string _PollingGroup;
        public string PollingGroup
        {
            get
            {
                return _PollingGroup;
            }

            set
            {
                SetPropertyValue("PollingGroup", ref _PollingGroup, value);
            }
        }

        /// <summary>
        /// Output Group
        /// </summary>
        private string _OutputGroup;
        public string OutputGroup
        {
            get
            {
                return _OutputGroup;
            }

            set
            {
                SetPropertyValue("OutputGroup", ref _OutputGroup, value);
            }
        }

        /// <summary>
        /// Data Format EIS
        /// </summary>
        private /*EISDATAFORMAT*/int _DataFormat;
        public /*EISDATAFORMAT*/int DataFormat
        {
            get
            {
                return _DataFormat;
            }

            set
            {
                SetPropertyValue("DataFormat", ref _DataFormat, value);
            }
        }

        /// <summary>
        /// Polling Time
        /// </summary>
        private uint _PollingTime;
        public uint PollingTime
        {
            get
            {
                return _PollingTime;
            }

            set
            {
                SetPropertyValue("PollingTime", ref _PollingTime, value);
            }
        }

        #endregion

        #region IDataErrorInfo Members
        #endregion
    }
}
