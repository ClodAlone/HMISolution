using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppNameSettingService;

namespace OPCUAViewModel
{
    public class AppNameSettings: Utilities.Observable, IAppNameSettings, IDataErrorInfo
    {
        public AppNameSettings()
        {
        }

        public AppNameSettings(SessionSettings settings)
        {
            RemoveDisabledItemAfterSecs = settings.RemoveDisabledItemAfterSecs;
            MaxCleanCount = settings.MaxCleanCount;
            UseAlwaysSecureConnections = settings.UseAlwaysSecureConnections;
            FastSamplingInterval = settings.FastSamplingInterval;
            SlowSamplingInterval = settings.SlowSamplingInterval;
            DisableWhenNotUsed = settings.DisableWhenNotUsed;
            PublishingInterval = settings.PublishingInterval;
        }

        String overriddenString;
        public void SetOverriddenString(String s)
        {
            overriddenString = s;
        }

        public bool HasOverriddenString()
        {
            return !String.IsNullOrEmpty(overriddenString);
        }

        public override string ToString()
        {
            if (!String.IsNullOrEmpty(overriddenString))
                return overriddenString;
            return base.ToString();
        }

        private int _RemoveDisabledItemAfterSecs = 30;
        [Display(Name = "RemoveDisabledItemAfterSecs", ResourceType = typeof(Properties.AppNameSettingsResource))]
        public int RemoveDisabledItemAfterSecs
        {
            get { return _RemoveDisabledItemAfterSecs; }
            set
            {
                Set(ref _RemoveDisabledItemAfterSecs, value, "RemoveDisabledItemAfterSecs");
            }
        }

        private int _MaxCleanCount = 2;
        [Display(Name = "MaxCleanCount", ResourceType = typeof(Properties.AppNameSettingsResource))]
        public int MaxCleanCount
        {
            get { return _MaxCleanCount; }
            set
            {
                Set(ref _MaxCleanCount, value, "MaxCleanCount");
            }
        }

        private bool _UseAlwaysSecureConnections = false;
        [Display(Name = "UseAlwaysSecureConnections", ResourceType = typeof(Properties.AppNameSettingsResource))]
        public bool UseAlwaysSecureConnections
        {
            get { return _UseAlwaysSecureConnections; }
            set
            {
                Set(ref _UseAlwaysSecureConnections, value, "UseAlwaysSecureConnections");
            }
        }

        private bool _UseSecurityWhenNotLocal;
        [Display(Name = "UseSecurityWhenNotLocal", ResourceType = typeof(Properties.AppNameSettingsResource))]
        public bool UseSecurityWhenNotLocal
        {
            get { return _UseSecurityWhenNotLocal; }
            set
            {
                Set(ref _UseSecurityWhenNotLocal, value, "UseSecurityWhenNotLocal");
            }
        }
        
        int _fastSamplingInterval = 250;
        [Display(Name = "FastSamplingInterval", ResourceType = typeof(Properties.AppNameSettingsResource))]
        public int FastSamplingInterval
        {
            get
            {
                return _fastSamplingInterval;
            }
            set
            {
                Set(ref _fastSamplingInterval, value, "FastSamplingInterval");
            }
        }

        int _slowSamplingInterval = 30000;
        [Display(Name = "SlowSamplingInterval", ResourceType = typeof(Properties.AppNameSettingsResource))]
        public int SlowSamplingInterval
        {
            get
            {
                return _slowSamplingInterval;
            }
            set
            {
                Set(ref _slowSamplingInterval, value, "SlowSamplingInterval");
            }
        }

        private bool _DisableWhenNotUsed = true;
        [Display(Name = "DisableWhenNotUsed", ResourceType = typeof(Properties.AppNameSettingsResource))]
        public bool DisableWhenNotUsed
        {
            get { return _DisableWhenNotUsed; }
            set
            {
                Set(ref _DisableWhenNotUsed, value, "DisableWhenNotUsed");
            }
        }

        int _publishingInterval = 250;
        [Display(Name = "PublishingInterval", ResourceType = typeof(Properties.AppNameSettingsResource))]
        public int PublishingInterval
        {
            get
            {
                return _publishingInterval;
            }
            set
            {
                Set(ref _publishingInterval, value, "PublishingInterval");
            }
        }

        bool _usePollingRead = false;
        [Display(Name = "UsePollingRead", ResourceType = typeof(Properties.AppNameSettingsResource))]
        public bool UsePollingRead
        {
            get
            {
                return _usePollingRead;
            }
            set
            {
                Set(ref _usePollingRead, value, "UsePollingRead");
            }
        }

        private string _AppNameRenamed;
        [Display(Name = "AppNameRenamed", ResourceType = typeof(Properties.AppNameSettingsResource))]
        public string AppNameRenamed
        {
            get { return _AppNameRenamed; }
            set
            {
                Set(ref _AppNameRenamed, value, "AppNameRenamed");
            }
        }

        private string _HostNameRenamed;
        [Display(Name = "HostNameRenamed", ResourceType = typeof(Properties.AppNameSettingsResource))]
        public string HostNameRenamed
        {
            get { return _HostNameRenamed; }
            set
            {
                Set(ref _HostNameRenamed, value, "HostNameRenamed");
            }
        }

        private string _EndpointRenamed;
        [Display(Name = "EndpointRenamed", ResourceType = typeof(Properties.AppNameSettingsResource))]
        public string EndpointRenamed
        {
            get { return _EndpointRenamed; }
            set
            {
                Set(ref _EndpointRenamed, value, "EndpointRenamed");
            }
        }

        private bool _AlwaysDiscoverEndpoint;
        [Display(Name = "AlwaysDiscoverEndpoint", ResourceType = typeof(Properties.AppNameSettingsResource))]
        public bool AlwaysDiscoverEndpoint
        {
            get { return _AlwaysDiscoverEndpoint; }
            set
            {
                Set(ref _AlwaysDiscoverEndpoint, value, "AlwaysDiscoverEndpoint");
            }
        }

        private string _BackupAppName;
        [Display(Name = "BackupAppName", ResourceType = typeof(Properties.AppNameSettingsResource))]
        public string BackupAppName
        {
            get { return _BackupAppName; }
            set
            {
                Set(ref _BackupAppName, value, "BackupAppName");
            }
        }

        private string _BackupHostName;
        [Display(Name = "BackupHostName", ResourceType = typeof(Properties.AppNameSettingsResource))]
        public string BackupHostName
        {
            get { return _BackupHostName; }
            set
            {
                Set(ref _BackupHostName, value, "BackupHostName");
            }
        }

        public string Error => throw new NotImplementedException();

        public string this[string columnName]
        {
            get { var s = PerformValidation(); return s; }
        }

        private string PerformValidation()
        {
            return String.Empty;
        }
    }
}
