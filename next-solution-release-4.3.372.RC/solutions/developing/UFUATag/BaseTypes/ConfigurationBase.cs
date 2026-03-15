using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Xpo;
using DevExpress.Xpo.DB.Helpers;
using UFInterfaces.PropertyControl;

namespace UFUAModel
{
    public abstract class ConfigurationBase : XPObject, INotifyPropertyVisibilityChanged, IDataErrorInfo
    {
        protected ConfigurationBase(Session session)
            : base(session)
        {
        }

        #region Properties Default Values
        // List of constant default values for each property where you want handle a default value.
        const int defaultMaxConcurrentHistoricalAccess = 5;
        const uint defaultWaitHistoryRetryTime = 60;
        const byte defaultMaxHistoryErrorBeforeFlush = 3;
        const uint defaultMaxHistoryErrorCacheSize = 10000;
        const int defaultMinHistoryPendingEntities = 500000;
        const int defaultMaxHistoryPendingEntities = 600000;
        const int defaultMaxHistoryDeletingEntities = 10000;
        const int defaultMaxHistoryBackgroundProcess = 1;
        const long defaultMaxHistoryTotalSafelyFilesSize = 104857600; // 100 MBytes
        const int defaultMaxHistoryAlarmsBranches = 10;
        const int defaultRedundancyHistoryThreadPool = -1;
        const int defaultRedundancyMaxSyncEntities = 10000;
        const uint defaultRedundancyPortNumber = 40000;
        const long defaultRedundancyMaxReceivedMessageSize = 524288; // 512 KBytes
        const System.ServiceModel.SecurityMode defaultRedundancyTransportSecurityMode = System.ServiceModel.SecurityMode.None;
        const string defaultRedundancyUdpServiceIpAddress = "224.0.0.1";
        const string defaultRedundancyUdpClientIpAddress = "224.0.0.1";
        const bool defaultRedundancyKeepActiveServerActive = false;
        const int defaultRedundancyMaxRetransmitCount = 0;
        const int defaultRedundancyMaxPendingTransmitMessages = 100;

        const int defaultSlowDataIOSamplingInterval = 10000;
        const int defaultMediumDataIOSamplingInterval = 3000;
        const int defaultFastDataIOSamplingInterval = 500;

        /// <summary>
        /// Adds inside this method the nullable property where you want handle a default value.
        /// </summary>
        protected virtual void EnsureDefaultValues()
        {
            // Examples of how to handle a default value
            //if (!_PropertyName.HasValue)
            //    _PropertyName = defaultPropertyName;
            //if (_TimeSpanPropertyName == TimeSpan.Zero)
            //    _TimeSpanPropertyName = TimeSpan.FromMinutes(1);
            //if (_DateTimePropertyName == DateTime.MinValue)
            //    _DateTimePropertyName = DateTime.UtcNow;

            if (_BuildDate == DateTime.MinValue)
                _BuildDate = DateTime.UtcNow;
            if (!_EventMaxAge.HasValue)
                _EventMaxAge = TimeSpan.FromDays(365);
            if (!_MaxConcurrentHistoricalAccess.HasValue)
                _MaxConcurrentHistoricalAccess = defaultMaxConcurrentHistoricalAccess;
            if (!_WaitHistoryRetryTime.HasValue)
                _WaitHistoryRetryTime = defaultWaitHistoryRetryTime;
            if (!_MaxHistoryErrorBeforeFlush.HasValue)
                _MaxHistoryErrorBeforeFlush = defaultMaxHistoryErrorBeforeFlush;
            if (!_MaxHistoryErrorCacheSize.HasValue)
                _MaxHistoryErrorCacheSize = defaultMaxHistoryErrorCacheSize;
            if (!_MinHistoryPendingEntities.HasValue)
                _MinHistoryPendingEntities = defaultMinHistoryPendingEntities;
            if (!_MaxHistoryPendingEntities.HasValue)
                _MaxHistoryPendingEntities = defaultMaxHistoryPendingEntities;
            if (!_MaxHistoryDeletingEntities.HasValue)
                _MaxHistoryDeletingEntities = defaultMaxHistoryDeletingEntities;
            if (!_MaxHistoryDeleteProcess.HasValue)
                _MaxHistoryDeleteProcess = defaultMaxHistoryBackgroundProcess;
            if (!_MaxHistoryRestoreProcess.HasValue)
                _MaxHistoryRestoreProcess = defaultMaxHistoryBackgroundProcess;
            if (!_MaxHistoryTotalSafelyFilesSize.HasValue)
                _MaxHistoryTotalSafelyFilesSize = defaultMaxHistoryTotalSafelyFilesSize;
            if (!_MaxHistoryAlarmsBranches.HasValue)
                _MaxHistoryAlarmsBranches = defaultMaxHistoryAlarmsBranches;
            if (_RedundancyStartupTimeout == TimeSpan.Zero)
                _RedundancyStartupTimeout = TimeSpan.FromSeconds(5);
            if (_RedundancySynchronizeTimeout == TimeSpan.Zero)
                _RedundancySynchronizeTimeout = TimeSpan.FromSeconds(3);
            if (_RedundancyTimeout == TimeSpan.Zero)
                _RedundancyTimeout = TimeSpan.FromSeconds(10);
            if (_RedundancyFullSynchronizationStartTime != DateTime.MinValue)
                _RedundancyFullSynchronizationStartTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, _RedundancyFullSynchronizationStartTime.Hour, _RedundancyFullSynchronizationStartTime.Minute, _RedundancyFullSynchronizationStartTime.Second);
            if (!_RedundancyHistoryThreadPool.HasValue)
                _RedundancyHistoryThreadPool = defaultRedundancyHistoryThreadPool;
            if (!_RedundancyMaxSyncEntities.HasValue)
                _RedundancyMaxSyncEntities = defaultRedundancyMaxSyncEntities;
            if (!_RedundancyPortNumber.HasValue)
                _RedundancyPortNumber = defaultRedundancyPortNumber;
            if (!_RedundancyMaxReceivedMessageSize.HasValue)
                _RedundancyMaxReceivedMessageSize = defaultRedundancyMaxReceivedMessageSize;
            if (!_RedundancyTransportSecurityMode.HasValue)
                _RedundancyTransportSecurityMode = defaultRedundancyTransportSecurityMode;
            if (_RedundancyUdpServiceIpAddress == null)
                _RedundancyUdpServiceIpAddress = defaultRedundancyUdpServiceIpAddress;
            if (_RedundancyUdpClientIpAddress == null)
                _RedundancyUdpClientIpAddress = defaultRedundancyUdpClientIpAddress;
            if (!_RedundancyKeepActiveServerActive.HasValue)
                _RedundancyKeepActiveServerActive = defaultRedundancyKeepActiveServerActive;
            if (!_RedundancyMaxRetransmitCount.HasValue)
                _RedundancyMaxRetransmitCount = defaultRedundancyMaxRetransmitCount;
            if (!_RedundancyMaxPendingTransmitMessages.HasValue)
                _RedundancyMaxPendingTransmitMessages = defaultRedundancyMaxPendingTransmitMessages;

            if (!SlowDataIOSamplingInterval.HasValue)
                SlowDataIOSamplingInterval = defaultSlowDataIOSamplingInterval;
            if (!MediumDataIOSamplingInterval.HasValue)
                MediumDataIOSamplingInterval = defaultMediumDataIOSamplingInterval;
            if (!FastDataIOSamplingInterval.HasValue)
                FastDataIOSamplingInterval = defaultFastDataIOSamplingInterval;
        }
        #endregion

        #region Properties

        private Guid _ConfigurationId;
        [Custom("Generate", "Guid")]
        public Guid ConfigurationId
        {
            get
            {
                return _ConfigurationId;
            }
            set
            {
                SetPropertyValue("ConfigurationId", ref _ConfigurationId, value);
            }
        }

        private string _ParentApplicationName;
        [Size(SizeAttribute.Unlimited)]
        public string ParentApplicationName
        {
            get
            {
                return _ParentApplicationName;
            }
            set
            {
                SetPropertyValue("ParentApplicationName", ref _ParentApplicationName, value);
            }
        }

        private int _DefaultAccessMask;
        public int DefaultAccessMask
        {
            get
            {
                return _DefaultAccessMask;
            }
            set
            {
                SetPropertyValue("DefaultAccessMask", ref _DefaultAccessMask, value);
            }
        }

        private int _DefaultAccessLevel;
        public int DefaultAccessLevel
        {
            get
            {
                return _DefaultAccessLevel;
            }
            set
            {
                SetPropertyValue("DefaultAccessLevel", ref _DefaultAccessLevel, value);
            }
        }

        private string _ApplicationName;
        [Size(SizeAttribute.Unlimited)]
        public string ApplicationName
        {
            get
            {
                if (_ApplicationName != null)
                    return _ApplicationName.Trim();
                
                return null;
            }
            set
            {
                SetPropertyValue("ApplicationName", ref _ApplicationName, value);
            }
        }

        private string _ManufacturerName;
        [Size(SizeAttribute.Unlimited)]
        public string ManufacturerName
        {
            get
            {
                return _ManufacturerName;
            }
            set
            {
                SetPropertyValue("ManufacturerName", ref _ManufacturerName, value);
            }
        }

        private string _ProductName;
        [Size(SizeAttribute.Unlimited)]
        public string ProductName
        {
            get
            {
                return _ProductName;
            }
            set
            {
                SetPropertyValue("ProductName", ref _ProductName, value);
            }
        }

        private string _ProductUri;
        [Size(SizeAttribute.Unlimited)]
        public string ProductUri
        {
            get
            {
                return _ProductUri;
            }
            set
            {
                SetPropertyValue("ProductUri", ref _ProductUri, value);
            }
        }

        private string _SoftwareVersion;
        [Size(SizeAttribute.Unlimited)]
        public string SoftwareVersion
        {
            get
            {
                return _SoftwareVersion;
            }
            set
            {
                SetPropertyValue("SoftwareVersion", ref _SoftwareVersion, value);
            }
        }

        private string _BuildNumber;
        [Size(SizeAttribute.Unlimited)]
        public string BuildNumber
        {
            get
            {
                return _BuildNumber;
            }
            set
            {
                SetPropertyValue("BuildNumber", ref _BuildNumber, value);
            }
        }

        private DateTime _BuildDate;
        public DateTime BuildDate
        {
            get
            {
                return _BuildDate;
            }
            set
            {
                if (value != DateTime.MinValue)
                    SetPropertyValue("BuildDate", ref _BuildDate, value);
            }
        }

        private string _AliasRoot;
        [Size(SizeAttribute.Unlimited)]
        public string AliasRoot
        {
            get
            {
                return _AliasRoot;
            }
            set
            {
                SetPropertyValue("AliasRoot", ref _AliasRoot, value);
            }
        }

        private string _NamespaceUri;
        [Size(SizeAttribute.Unlimited)]
        public string NamespaceUri
        {
            get
            {
                return _NamespaceUri;
            }
            set
            {
                SetPropertyValue("NamespaceUri", ref _NamespaceUri, value);
            }
        }

        private string _HistorianDefaultConnection;
        [Size(SizeAttribute.Unlimited)]
        public string HistorianDefaultConnection
        {
            get
            {
                return _HistorianDefaultConnection;
            }
            set
            {
                SetPropertyValue("HistorianDefaultConnection", ref _HistorianDefaultConnection, value);
            }
        }

        private string _EventDefaultConnection;
        [Size(SizeAttribute.Unlimited)]
        public string EventDefaultConnection
        {
            get
            {
                return _EventDefaultConnection;
            }
            set
            {
                SetPropertyValue("EventDefaultConnection", ref _EventDefaultConnection, value);
            }
        }

        private string _AuditTraceDefaultConnection;
        [Size(SizeAttribute.Unlimited)]
        public string AuditTraceDefaultConnection
        {
            get
            {
                return _AuditTraceDefaultConnection;
            }
            set
            {
                SetPropertyValue("AuditTraceDefaultConnection", ref _AuditTraceDefaultConnection, value);
            }
        }

        private TimeSpan? _EventMaxAge;
        public TimeSpan? EventMaxAge
        {
            get
            {
                return _EventMaxAge;
            }
            set
            {
                SetPropertyValue("EventMaxAge", ref _EventMaxAge, value);
            }
        }

        private bool _EnableEventDataProtection;
        public bool EnableEventDataProtection
        {
            get
            {
                return _EnableEventDataProtection;
            }
            set
            {
                if (SetPropertyValue("EnableEventDataProtection", ref _EnableEventDataProtection, value))
                {
                    RaisePropertyChangedEvent("HistorianDefaultConnection");
                    RaisePropertyChangedEvent("EventDefaultConnection");
                    RaisePropertyChangedEvent("AuditTraceDefaultConnection");
                    OnPropertyVisiblityChanged("EnableEventDataProtection");
                }
            }
        }

        /// <summary>
        /// Max values of concurrent historical access from OPC Client UA.
        /// When this value is reached a new OPC Client UA must wait for retrieving data from server.
        /// </summary>
        private int? _MaxConcurrentHistoricalAccess;
        public int? MaxConcurrentHistoricalAccess
        {
            get
            {
                return _MaxConcurrentHistoricalAccess;
            }
            set
            {
                SetPropertyValue("MaxConcurrentHistoricalAccess", ref _MaxConcurrentHistoricalAccess, value);
            }
        }

        /// <summary>
        /// Time to wait for retring to perform a write operation
        /// </summary>
        private uint? _WaitHistoryRetryTime;
        public uint? WaitHistoryRetryTime
        {
            get
            {
                return _WaitHistoryRetryTime;
            }
            set
            {
                SetPropertyValue("WaitHistoryRetryTime", ref _WaitHistoryRetryTime, value);
            }
        }

        /// <summary>
        /// Max number of errors before flush safely to file
        /// </summary>
        private byte? _MaxHistoryErrorBeforeFlush;
        public byte? MaxHistoryErrorBeforeFlush
        {
            get
            {
                return _MaxHistoryErrorBeforeFlush;
            }
            set
            {
                SetPropertyValue("MaxHistoryErrorBeforeFlush", ref _MaxHistoryErrorBeforeFlush, value);
            }
        }

        /// <summary>
        /// Max number of pending errors before flush safely to file
        /// </summary>
        private uint? _MaxHistoryErrorCacheSize;
        public uint? MaxHistoryErrorCacheSize
        {
            get
            {
                return _MaxHistoryErrorCacheSize;
            }
            set
            {
                SetPropertyValue("MaxHistoryErrorCacheSize", ref _MaxHistoryErrorCacheSize, value);
            }
        }

        /// <summary>
        /// Number of pending entities to wait before exiting to fulsh safely to file.
        /// </summary>
        private int? _MinHistoryPendingEntities;
        public int? MinHistoryPendingEntities
        {
            get
            {
                return _MinHistoryPendingEntities;
            }
            set
            {
                SetPropertyValue("MinHistoryPendingEntities", ref _MinHistoryPendingEntities, value);
            }
        }

        /// <summary>
        /// Number of pending entities before starting to fulsh safely to file.
        /// </summary>
        private int? _MaxHistoryPendingEntities;
        public int? MaxHistoryPendingEntities
        {
            get
            {
                return _MaxHistoryPendingEntities; 
            }
            set
            {
                SetPropertyValue("MaxHistoryPendingEntities", ref _MaxHistoryPendingEntities, value);
            }
        }

        /// <summary>
        /// Maximum amount of group data to load from datalayer for deleting old data.
        /// </summary>
        private int? _MaxHistoryDeletingEntities;
        public int? MaxHistoryDeletingEntities
        {
            get
            {
                return _MaxHistoryDeletingEntities;
            }
            set
            {
                SetPropertyValue("MaxHistoryDeletingEntities", ref _MaxHistoryDeletingEntities, value);
            }
        }

        /// <summary>
        /// Maximum number of background process to run for deleting the old records.
        /// </summary>
        private int? _MaxHistoryDeleteProcess;
        public int? MaxHistoryDeleteProcess
        {
            get
            {
                return _MaxHistoryDeleteProcess;
            }
            set
            {
                SetPropertyValue("MaxHistoryDeleteProcess", ref _MaxHistoryDeleteProcess, value);
            }
        }

        /// <summary>
        /// Maximum number of background process to run for restoring the flushed records.
        /// </summary>
        private int? _MaxHistoryRestoreProcess;
        public int? MaxHistoryRestoreProcess
        {
            get
            {
                return _MaxHistoryRestoreProcess;
            }
            set
            {
                SetPropertyValue("MaxHistoryRestoreProcess", ref _MaxHistoryRestoreProcess, value);
            }
        }

        /// <summary>
        /// Maximum size in bytes of safely file recorded when a database broken connection is active.
        /// </summary>
        private long? _MaxHistoryTotalSafelyFilesSize;
        public long? MaxHistoryTotalSafelyFilesSize
        {
            get
            {
                return _MaxHistoryTotalSafelyFilesSize;
            }
            set
            {
                SetPropertyValue("MaxHistoryTotalSafelyFilesSize", ref _MaxHistoryTotalSafelyFilesSize, value);
            }
        }

        /// <summary>
        /// Maximum number of branches allowed for any alarms.
        /// </summary>
        private int? _MaxHistoryAlarmsBranches;
        public int? MaxHistoryAlarmsBranches
        {
            get
            {
                return _MaxHistoryAlarmsBranches;
            }
            set
            {
                SetPropertyValue("MaxHistoryAlarmsBranches", ref _MaxHistoryAlarmsBranches, value);
            }
        }

        private int _MaxSessionTimeout;
        public int MaxSessionTimeout
        {
            get
            {
                return _MaxSessionTimeout;
            }
            set
            {
                SetPropertyValue("MaxSessionTimeout", ref _MaxSessionTimeout, value);
            }
        }
        private int _MinSessionTimeout;
        public int MinSessionTimeout
        {
            get
            {
                return _MinSessionTimeout;
            }
            set
            {
                SetPropertyValue("MinSessionTimeout", ref _MinSessionTimeout, value);
            }
        }
        private bool _DiagnosticEnabled;
        public bool DiagnosticEnabled
        {
            get
            {
                return _DiagnosticEnabled;
            }
            set
            {
                SetPropertyValue("DiagnosticEnabled", ref _DiagnosticEnabled, value);
            }
        }
        private bool _SpeechEnabled = true;
        public bool SpeechEnabled
        {
            get
            {
                return _SpeechEnabled;
            }
            set
            {
                SetPropertyValue("SpeechEnabled", ref _SpeechEnabled, value);
            }
        }
        private String _SpeechVoiceName;
        [Size(SizeAttribute.Unlimited)]
        public String SpeechVoiceName
        {
            get
            {
                return _SpeechVoiceName;
            }
            set
            {
                SetPropertyValue("SpeechVoiceName", ref _SpeechVoiceName, value);
            }
        }
        private int _MaxSessionCount;
        public int MaxSessionCount
        {
            get
            {
                return _MaxSessionCount;
            }
            set
            {
                SetPropertyValue("MaxSessionCount", ref _MaxSessionCount, value);
            }
        }
        private int _MaxBrowseContinuationPoints;
        public int MaxBrowseContinuationPoints
        {
            get
            {
                return _MaxBrowseContinuationPoints;
            }
            set
            {
                SetPropertyValue("MaxBrowseContinuationPoints", ref _MaxBrowseContinuationPoints, value);
            }
        }
        private int _MaxHistoryContinuationPoints;
        public int MaxHistoryContinuationPoints
        {
            get
            {
                return _MaxHistoryContinuationPoints;
            }
            set
            {
                SetPropertyValue("MaxHistoryContinuationPoints", ref _MaxHistoryContinuationPoints, value);
            }
        }
        private int _MaxRequestAge;
        public int MaxRequestAge
        {
            get
            {
                return _MaxRequestAge;
            }
            set
            {
                SetPropertyValue("MaxRequestAge", ref _MaxRequestAge, value);
            }
        }
        private int _MinPublishingInterval;
        public int MinPublishingInterval
        {
            get
            {
                return _MinPublishingInterval;
            }
            set
            {
                SetPropertyValue("MinPublishingInterval", ref _MinPublishingInterval, value);
            }
        }
        private int _MaxPublishingInterval;
        public int MaxPublishingInterval
        {
            get
            {
                return _MaxPublishingInterval;
            }
            set
            {
                SetPropertyValue("MaxPublishingInterval", ref _MaxPublishingInterval, value);
            }
        }
        private int _PublishingResolution;
        public int PublishingResolution
        {
            get
            {
                return _PublishingResolution;
            }
            set
            {
                SetPropertyValue("PublishingResolution", ref _PublishingResolution, value);
            }
        }
        private int _MaxSubscriptionLifetime;
        public int MaxSubscriptionLifetime
        {
            get
            {
                return _MaxSubscriptionLifetime;
            }
            set
            {
                SetPropertyValue("MaxSubscriptionLifetime", ref _MaxSubscriptionLifetime, value);
            }
        }
        private int _MaxMessageQueueSize;
        public int MaxMessageQueueSize
        {
            get
            {
                return _MaxMessageQueueSize;
            }
            set
            {
                SetPropertyValue("MaxMessageQueueSize", ref _MaxMessageQueueSize, value);
            }
        }
        private int _MaxNotificationQueueSize;
        public int MaxNotificationQueueSize
        {
            get
            {
                return _MaxNotificationQueueSize;
            }
            set
            {
                SetPropertyValue("MaxNotificationQueueSize", ref _MaxNotificationQueueSize, value);
            }
        }
        private int _MaxNotificationsPerPublish;
        public int MaxNotificationsPerPublish
        {
            get
            {
                return _MaxNotificationsPerPublish;
            }
            set
            {
                SetPropertyValue("MaxNotificationsPerPublish", ref _MaxNotificationsPerPublish, value);
            }
        }
        private int _MinMetadataSamplingInterval;
        public int MinMetadataSamplingInterval
        {
            get
            {
                return _MinMetadataSamplingInterval;
            }
            set
            {
                SetPropertyValue("MinMetadataSamplingInterval", ref _MinMetadataSamplingInterval, value);
            }
        }
        private int _DefaultDataIOSamplingInterval;
        public int DefaultDataIOSamplingInterval
        {
            get
            {
                return _DefaultDataIOSamplingInterval;
            }
            set
            {
                SetPropertyValue("DefaultDataIOSamplingInterval", ref _DefaultDataIOSamplingInterval, value);
            }
        }

        private int? _SlowDataIOSamplingInterval;
        public int? SlowDataIOSamplingInterval
        {
            get
            {
                return _SlowDataIOSamplingInterval;
            }
            set
            {
                SetPropertyValue("SlowDataIOSamplingInterval", ref _SlowDataIOSamplingInterval, value);
            }
        }

        private int? _MediumDataIOSamplingInterval;
        public int? MediumDataIOSamplingInterval
        {
            get
            {
                return _MediumDataIOSamplingInterval;
            }
            set
            {
                SetPropertyValue("MediumDataIOSamplingInterval", ref _MediumDataIOSamplingInterval, value);
            }
        }

        private int? _FastDataIOSamplingInterval;
        public int? FastDataIOSamplingInterval
        {
            get
            {
                return _FastDataIOSamplingInterval;
            }
            set
            {
                SetPropertyValue("FastDataIOSamplingInterval", ref _FastDataIOSamplingInterval, value);
            }
        }

        #region Redundancy
        private String _ListRedundancyServers;
        [Size(SizeAttribute.Unlimited)]
        [Category("Redundancy")]
        public String ListRedundancyServers
        {
            get
            {
                return _ListRedundancyServers;
            }
            set
            {
                if (SetPropertyValue("ListRedundancyServers", ref _ListRedundancyServers, value))
                {
                    RaisePropertyChangedEvent("RedundancyKeepActiveServerActive");
                }
            }
        }

        private TimeSpan _RedundancyStartupTimeout;
        [Category("Redundancy")]
        public TimeSpan RedundancyStartupTimeout
        {
            get
            {
                return _RedundancyStartupTimeout;
            }
            set
            {
                SetPropertyValue("RedundancyStartupTimeout", ref _RedundancyStartupTimeout, value);
            }
        }

        private TimeSpan _RedundancySynchronizeTimeout;
        [Category("Redundancy")]
        public TimeSpan RedundancySynchronizeTimeout
        {
            get
            {
                return _RedundancySynchronizeTimeout;
            }
            set
            {
                SetPropertyValue("RedundancySynchronizeTimeout", ref _RedundancySynchronizeTimeout, value);
            }
        }

        protected TimeSpan _RedundancyTimeout;
        [Category("Redundancy")]
        public TimeSpan RedundancyTimeout
        {
            get
            {
                return _RedundancyTimeout;
            }
            set
            {
                SetPropertyValue("RedundancyTimeout", ref _RedundancyTimeout, value);
            }
        }

        private DateTime _RedundancyFullSynchronizationStartTime;
        [Category("Redundancy")]
        public DateTime RedundancyFullSynchronizationStartTime
        {
            get
            {
                return _RedundancyFullSynchronizationStartTime;
            }
            set
            {
                SetPropertyValue("RedundancyFullSynchronizationStartTime", ref _RedundancyFullSynchronizationStartTime, value);
            }
        }

        private TimeSpan _RedundancyFullSynchronizationTimeSpan;
        [Category("Redundancy")]
        public TimeSpan RedundancyFullSynchronizationTimeSpan
        {
            get
            {
                return _RedundancyFullSynchronizationTimeSpan;
            }
            set
            {
                SetPropertyValue("RedundancyFullSynchronizationTimeSpan", ref _RedundancyFullSynchronizationTimeSpan, value);
            }
        }

        private int? _RedundancyHistoryThreadPool;
        [Category("Redundancy")]
        public int? RedundancyHistoryThreadPool
        {
            get
            {
                return _RedundancyHistoryThreadPool;
            }
            set
            {
                SetPropertyValue("RedundancyHistoryThreadPool", ref _RedundancyHistoryThreadPool, value);
            }
        }

        private int? _RedundancyMaxSyncEntities;
        [Category("Redundancy")]
        public int? RedundancyMaxSyncEntities
        {
            get
            {
                return _RedundancyMaxSyncEntities;
            }
            set
            {
                SetPropertyValue("RedundancyMaxSyncEntities", ref _RedundancyMaxSyncEntities, value);
            }
        }

        protected uint? _RedundancyPortNumber;
        [Category("Redundancy")]
        public uint? RedundancyPortNumber
        {
            get
            {
                return _RedundancyPortNumber;
            }
            set
            {
                SetPropertyValue("RedundancyPortNumber", ref _RedundancyPortNumber, value);
            }
        }

        private long? _RedundancyMaxReceivedMessageSize;
        [Category("Redundancy")]
        public long? RedundancyMaxReceivedMessageSize
        {
            get
            {
                return _RedundancyMaxReceivedMessageSize;
            }
            set
            {
                SetPropertyValue("RedundancyMaxReceivedMessageSize", ref _RedundancyMaxReceivedMessageSize, value);
            }
        }

        private int? _RedundancyMaxRetransmitCount;
        [Category("Redundancy")]
        public int? RedundancyMaxRetransmitCount
        {
            get
            {
                return _RedundancyMaxRetransmitCount;
            }
            set
            {
                SetPropertyValue("RedundancyMaxRetransmitCount", ref _RedundancyMaxRetransmitCount, value);
            }
        }

        private int? _RedundancyMaxPendingTransmitMessages;
        [Category("Redundancy")]
        public int? RedundancyMaxPendingTransmitMessages
        {
            get
            {
                return _RedundancyMaxPendingTransmitMessages;
            }
            set
            {
                SetPropertyValue("RedundancyMaxPendingTransmitMessages", ref _RedundancyMaxPendingTransmitMessages, value);
            }
        }

        private bool? _RedundancyKeepActiveServerActive;
        [Category("Redundancy")]
        public bool? RedundancyKeepActiveServerActive
        {
            get
            {
                return _RedundancyKeepActiveServerActive;
            }
            set
            {
                SetPropertyValue("RedundancyKeepActiveServerActive", ref _RedundancyKeepActiveServerActive, value);
            }
        }

        private bool _RedundancySkipHistoryDataSynchronization;
        [Category("Redundancy")]
        public bool RedundancySkipHistoryDataSynchronization
        {
            get
            {
                return _RedundancySkipHistoryDataSynchronization;
            }
            set
            {
                SetPropertyValue("RedundancySkipHistoryDataSynchronization", ref _RedundancySkipHistoryDataSynchronization, value);
            }
        }

        private System.ServiceModel.SecurityMode? _RedundancyTransportSecurityMode;
        [Category("Redundancy")]
        public System.ServiceModel.SecurityMode? RedundancyTransportSecurityMode
        {
            get
            {
                return _RedundancyTransportSecurityMode;
            }
            set
            {
                SetPropertyValue("RedundancyTransportSecurityMode", ref _RedundancyTransportSecurityMode, value);
            }
        }

        private string _RedundancyUdpServiceIpAddress;
        [Category("Redundancy")]
        public string RedundancyUdpServiceIpAddress
        {
            get
            {
                return _RedundancyUdpServiceIpAddress;
            }
            set
            {
                SetPropertyValue("RedundancyUdpServiceIpAddress", ref _RedundancyUdpServiceIpAddress, value);
            }
        }

        private string _RedundancyUdpClientIpAddress;
        [Category("Redundancy")]
        public string RedundancyUdpClientIpAddress
        {
            get
            {
                return _RedundancyUdpClientIpAddress;
            }
            set
            {
                SetPropertyValue("RedundancyUdpClientIpAddress", ref _RedundancyUdpClientIpAddress, value);
            }
        }
        #endregion

        [Association("UFUAConfiguration-BaseAddresses"), Aggregated]
        public XPCollection<AddressBase> BaseAddresses
        {
            get
            {
                return GetCollection<AddressBase>("BaseAddresses");
            }
        }
        #endregion

        #region Override Methods

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

        #region IDataErrorInfo

        [Browsable(false)]
        public string Error
        {
            get
            {
                var context = new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null);
                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

                return !System.ComponentModel.DataAnnotations.Validator.TryValidateObject(this, context, results)
                    ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                    : null;
            }
        }

        public string this[string propertyName]
        {
            get
            {
                String s = PerformValidation(propertyName);
                if (!String.IsNullOrEmpty(s))
                    return s;
                var context = new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null)
                {
                    MemberName = propertyName
                };

                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
                var propertyInfo = GetType().GetProperty(propertyName);
                if (propertyInfo != null)
                {
                    var value = propertyInfo.GetValue(this, null);

                    return !System.ComponentModel.DataAnnotations.Validator.TryValidateProperty(value, context, results)
                        ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                        : null;
                }

                return null;
            }
        }
        #endregion

        #region INotifyPropertyVisibilityChanged Members

        /// <summary>
        /// Gets the visibility state for the property with the given name.
        /// </summary>
        /// <param name="propertyName">The property name that you want konw the current visibility state.</param>
        /// <returns></returns>
        bool INotifyPropertyVisibilityChanged.this[string propertyName]
        {
            get
            {
                if (propertyName == "EnableEventDataProtection" || propertyName == "AuditTraceDefaultConnection")
                {
                    return !String.IsNullOrEmpty(UFUAServerInfo.UFUAServerInfo.GetCFR21UserName());
                }

                return true;
            }
        }

        /// <summary>
        /// Raised when a property visibility state on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyVisiblityChanged;

        /// <summary>
        /// Raises this object's PropertyVisiblityChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has changed his value and has triggered the change of visibility.</param>
        protected void OnPropertyVisiblityChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyVisiblityChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        #endregion

        #region Methods

        private static readonly String DataSourceHeader = "data source";
        private static readonly String CatalogSourceHeader = "initial catalog";
        public virtual void EnsureDefaultSettings(string title)
        {
            if (String.IsNullOrEmpty(ApplicationName))
            {
                ApplicationName = title;
            }
            if (ConfigurationId == Guid.Empty)
                ConfigurationId = Guid.NewGuid();

            if (String.IsNullOrEmpty(HistorianDefaultConnection) || 
                String.IsNullOrEmpty(EventDefaultConnection))
            {
                var helper = new ConnectionStringParser(UFUAServerInfo.UFUAServerInfo.GetDefaultConnectionString());
                if (helper.PartExists(CatalogSourceHeader))
                    helper.UpdatePartByName(CatalogSourceHeader, ApplicationName);
                else if (helper.PartExists(DataSourceHeader))
                {
                    var dataSource = helper.GetPartByName(DataSourceHeader);
                    var filename = System.IO.Path.GetFileNameWithoutExtension(dataSource.Replace(Utilities.Properties.Settings.Default.ProjectRootPlaceholder, String.Empty));
                    dataSource = dataSource.Replace(filename, ApplicationName);
                    helper.UpdatePartByName(DataSourceHeader, dataSource);
                }

                if (String.IsNullOrEmpty(HistorianDefaultConnection))
                    HistorianDefaultConnection = helper.GetConnectionString();
                if (String.IsNullOrEmpty(EventDefaultConnection))
                    EventDefaultConnection = helper.GetConnectionString();
            }

            if (String.IsNullOrEmpty(AuditTraceDefaultConnection))
            {
                var helper = new ConnectionStringParser(UFUAServerInfo.UFUAServerInfo.GetDefaultConnectionString());
                if (helper.PartExists(CatalogSourceHeader))
                    helper.UpdatePartByName(CatalogSourceHeader, String.Format("{0}_{1}", ApplicationName, UFUAServerInfo.UFUAServerInfo.GetAuditTraceSuffix()));
                else if (helper.PartExists(DataSourceHeader))
                {
                    var dataSource = helper.GetPartByName(DataSourceHeader);
                    var filename = System.IO.Path.GetFileNameWithoutExtension(dataSource.Replace(Utilities.Properties.Settings.Default.ProjectRootPlaceholder, String.Empty));
                    dataSource = dataSource.Replace(filename, String.Format("{0}_{1}", ApplicationName, UFUAServerInfo.UFUAServerInfo.GetAuditTraceSuffix()));
                    helper.UpdatePartByName(DataSourceHeader, dataSource);
                }

                AuditTraceDefaultConnection = helper.GetConnectionString();
            }

            EnsureDefaultBaseAddresses(Session);
        }

        public void NormalizeConnectionStrings(string projectRoot)
        {
            HistorianDefaultConnection = XpoHelpers.XpoHelper.NormalizeConnectionString(HistorianDefaultConnection, projectRoot);
            EventDefaultConnection = XpoHelpers.XpoHelper.NormalizeConnectionString(EventDefaultConnection, projectRoot);
            AuditTraceDefaultConnection = XpoHelpers.XpoHelper.NormalizeConnectionString(AuditTraceDefaultConnection, projectRoot);
        }

        public void EnsureTrustedConnectionStrings()
        {
            HistorianDefaultConnection = XpoHelpers.XpoHelper.EnsureTrustedConnectionStrings(HistorianDefaultConnection);
            EventDefaultConnection = XpoHelpers.XpoHelper.EnsureTrustedConnectionStrings(EventDefaultConnection);
            AuditTraceDefaultConnection = XpoHelpers.XpoHelper.EnsureTrustedConnectionStrings(AuditTraceDefaultConnection);
        }

        public abstract void EnsureDefaultBaseAddresses(Session session);

        public abstract int GetDefaultPort(string transport);

        protected virtual String PerformValidation(String propertyName, String prototypeModel = null)
        {
            if (propertyName == "ApplicationName")
            {
                if (String.IsNullOrEmpty(ApplicationName))
                    return Properties.Resources.ConfigurationApplicationNameIsEmpty;
            }
            else if (propertyName == "RedundancyUdpServiceIpAddress")
            {
                System.Net.IPAddress ipAddress;
                if (!System.Net.IPAddress.TryParse(RedundancyUdpServiceIpAddress, out ipAddress))
                    return Properties.Resources.InvalidIpAddress;
            }
            else if (propertyName == "RedundancyUdpClientIpAddress")
            {
                System.Net.IPAddress ipAddress;
                if (!System.Net.IPAddress.TryParse(RedundancyUdpClientIpAddress, out ipAddress))
                    return Properties.Resources.InvalidIpAddress;
            }
            else if (propertyName == "RedundancyKeepActiveServerActive")
            {
                if (RedundancyKeepActiveServerActive.Value && !String.IsNullOrEmpty(ListRedundancyServers))
                {
                    var servers = ListRedundancyServers.Split(new Char[] { ',' });
                    if (servers.Length > 2)
                        return Properties.Resources.InvalidKeepActiveServerActiveOption;
                }
            }
            else if (propertyName == "EventMaxAge")
            {
                if (!EventMaxAge.HasValue || EventMaxAge == TimeSpan.MinValue)
                    return Properties.Resources.ErrorInvalidTimeSpan;
            }
            else if (propertyName == "HistorianDefaultConnection")
            {
                if (EnableEventDataProtection && !XpoHelpers.XpoHelper.IsMSSQlDataProvider(HistorianDefaultConnection))
                    return Properties.Resources.DataProtectionXpoProviderNotSupported;
            }
            else if (propertyName == "EventDefaultConnection")
            {
                if (EnableEventDataProtection && !XpoHelpers.XpoHelper.IsMSSQlDataProvider(EventDefaultConnection))
                    return Properties.Resources.DataProtectionXpoProviderNotSupported;
            }
            else if (propertyName == "AuditTraceDefaultConnection")
            {
                if (EnableEventDataProtection && !XpoHelpers.XpoHelper.IsMSSQlDataProvider(AuditTraceDefaultConnection))
                    return Properties.Resources.DataProtectionXpoProviderNotSupported;
            }

            return null;
        }

        #endregion
        
    }
}
