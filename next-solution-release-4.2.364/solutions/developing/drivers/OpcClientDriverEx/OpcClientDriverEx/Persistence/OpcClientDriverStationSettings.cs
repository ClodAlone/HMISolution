using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace OpcClientDriver
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class OpcClientDriverStationSettings : StationSettings
    {
        #region Data
        const int defaultRemoveDisabledItemAfterSecs = 30;
        const int defaultMaxCleanCount = 2;
        const bool defaultUseAlwaysSecureConnections = false;
        const int defaultFastSamplingInterval = 500;
        const int defaultSlowSamplingInterval = 5000;
        const bool defaultDisableWhenNotUsed = false;
        const int defaultPublishingInterval = 1000;
        const int defaultConnectionTimeout = 10000;
        const bool defaultUserServerDiscovery = false;
        const bool defaultUseLocalTimestamp = false;
        #endregion

        #region Constructors

        public OpcClientDriverStationSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected OpcClientDriverStationSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

        #region Methods
        public void CopyProperties(OpcClientDriverStationSettings st)
        {
            base.CopyProperties(st);
        }

        public void DefaultSettings()
        {
            base.DefaultSettings();

            _RemoveDisabledItemAfterSecs = defaultRemoveDisabledItemAfterSecs;
            _MaxCleanCount = defaultMaxCleanCount;
            _UseAlwaysSecureConnections = defaultUseAlwaysSecureConnections;
            _FastSamplingInterval = defaultFastSamplingInterval;
            _SlowSamplingInterval = defaultSlowSamplingInterval;
            _DisableWhenNotUsed = defaultDisableWhenNotUsed;
            _PublishingInterval = defaultPublishingInterval;
            _ConnectionTimeout = defaultConnectionTimeout;
            _UseServerDiscovery = defaultUserServerDiscovery;
            _UseLocalTimestamp = defaultUseLocalTimestamp;
        }

        private void EnsureDefaultValues()
        {

            if (!_RemoveDisabledItemAfterSecs.HasValue)
                _RemoveDisabledItemAfterSecs = defaultRemoveDisabledItemAfterSecs;
            if (!_MaxCleanCount.HasValue)
                _MaxCleanCount = defaultMaxCleanCount;
            if (!_UseAlwaysSecureConnections.HasValue)
                _UseAlwaysSecureConnections = defaultUseAlwaysSecureConnections;
            if (!_FastSamplingInterval.HasValue)
                _FastSamplingInterval = defaultFastSamplingInterval;
            if (!_SlowSamplingInterval.HasValue)
                _SlowSamplingInterval = defaultSlowSamplingInterval;
            if (!_DisableWhenNotUsed.HasValue)
                _DisableWhenNotUsed = defaultDisableWhenNotUsed;
            if (!_PublishingInterval.HasValue)
                _PublishingInterval = defaultPublishingInterval;
            if (!_ConnectionTimeout.HasValue)
                _ConnectionTimeout = defaultConnectionTimeout;
            if (!_UseServerDiscovery.HasValue)
                _UseServerDiscovery = defaultUserServerDiscovery;
            if (!_UseLocalTimestamp.HasValue)
                _UseLocalTimestamp = defaultUseLocalTimestamp;
        }
        #endregion

        #region Override
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

        #region Properties

        private int? _RemoveDisabledItemAfterSecs;
        public int? RemoveDisabledItemAfterSecs
        {
            get
            {
                return _RemoveDisabledItemAfterSecs;
            }
            set
            {
                SetPropertyValue("RemoveDisabledItemAfterSecs", ref _RemoveDisabledItemAfterSecs, value);
            }
        }

        private int? _MaxCleanCount;
        public int? MaxCleanCount
        {
            get
            {
                return _MaxCleanCount;
            }
            set
            {
                SetPropertyValue("MaxCleanCount", ref _MaxCleanCount, value);
            }
        }

        private bool? _UseAlwaysSecureConnections;
        public bool? UseAlwaysSecureConnections
        {
            get
            {
                return _UseAlwaysSecureConnections;
            }
            set
            {
                SetPropertyValue("UseAlwaysSecureConnections", ref _UseAlwaysSecureConnections, value);
            }
        }

        private int? _FastSamplingInterval;
        public int? FastSamplingInterval
        {
            get
            {
                return _FastSamplingInterval;
            }
            set
            {
                SetPropertyValue("FastSamplingInterval", ref _FastSamplingInterval, value);
            }
        }

        private int? _SlowSamplingInterval;
        public int? SlowSamplingInterval
        {
            get
            {
                return _SlowSamplingInterval;
            }
            set
            {
                SetPropertyValue("SlowSamplingInterval", ref _SlowSamplingInterval, value);
            }
        }

        private bool? _DisableWhenNotUsed;
        public bool? DisableWhenNotUsed
        {
            get
            {
                return _DisableWhenNotUsed;
            }
            set
            {
                SetPropertyValue("DisableWhenNotUsed", ref _DisableWhenNotUsed, value);
            }
        }

        private int? _PublishingInterval;
        public int? PublishingInterval
        {
            get
            {
                return _PublishingInterval;
            }
            set
            {
                SetPropertyValue("PublishingInterval", ref _PublishingInterval, value);
            }
        }

        private bool _UsePollingRead;
        public bool UsePollingRead
        {
            get
            {
                return _UsePollingRead;
            }
            set
            {
                SetPropertyValue("UsePollingRead", ref _UsePollingRead, value);
            }
        }

        private int? _ConnectionTimeout;
        public int? ConnectionTimeout
        {
            get
            {
                return _ConnectionTimeout;
            }
            set
            {
                SetPropertyValue("ConnectionTimeout", ref _ConnectionTimeout, value);
            }
        }

        private string _User;
        public string User
        {
            get
            {
                return _User;
            }
            set
            {
                SetPropertyValue("User", ref _User, value);
            }
        }

        private string _Password;
        [ValueConverter(typeof(EncryptString.EncryptedValueConverter))]
        [Size(200)]
        public string Password
        {
            get
            {
                return _Password;
            }
            set
            {
                SetPropertyValue("Password", ref _Password, value);
            }
        }

        private bool? _UseServerDiscovery;

        public bool? UseServerDiscovery
        {
            get
            {
                return _UseServerDiscovery;
            }
            set
            {
                SetPropertyValue("UseServerDiscovery", ref _UseServerDiscovery, value);
            }
        }

        private bool? _UseLocalTimestamp;

        public bool? UseLocalTimestamp
        {
            get
            {
                return _UseLocalTimestamp;
            }
            set
            {
                SetPropertyValue("UseLocalTimestamp", ref _UseLocalTimestamp, value);
            }
        }
        #endregion



        #region IDataErrorInfo Members

        protected override String PerformValidation(String propertyName)
        {
            switch (propertyName)
            {
                case "ConnectionTimeout":
                    if (ConnectionTimeout > int.MaxValue || ConnectionTimeout < 0)
                        return string.Format(Properties.Resources.ConnectionTimeoutOutOfRange, int.MaxValue);
                    break;
                case "PublishingInterval":
                    if (PublishingInterval > int.MaxValue || PublishingInterval < 0)
                        return string.Format(Properties.Resources.PublishingIntervalOutOfRange, int.MaxValue);
                    break;
                case "MaxCleanCount":
                    if (MaxCleanCount > int.MaxValue || MaxCleanCount < 0)
                        return string.Format(Properties.Resources.MaxCleanCountOutOfRange, int.MaxValue);
                    break;
                case "FastSamplingInterval":
                    if (FastSamplingInterval > int.MaxValue || FastSamplingInterval < 0)
                        return string.Format(Properties.Resources.FastSamplingIntervalOutOfRange, int.MaxValue);
                    break;
                case "SlowSamplingInterval":
                    if (SlowSamplingInterval > int.MaxValue || SlowSamplingInterval < 0)
                        return string.Format(Properties.Resources.SlowSamplingIntervalOutOfRange, int.MaxValue);
                    break;
                case "RemoveDisabledItemAfterSecs":
                    if (RemoveDisabledItemAfterSecs > int.MaxValue || RemoveDisabledItemAfterSecs < 0)
                        return string.Format(Properties.Resources.RemoveDisabledItemAfterSecsOutOfRange, int.MaxValue);
                    break;
            }
        string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            return null;
        }

        #endregion

    }
}
