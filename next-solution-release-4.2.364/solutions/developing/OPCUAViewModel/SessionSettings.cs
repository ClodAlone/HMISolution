using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCUAViewModel
{
    public class SessionSettings
    {
        private string _ParentTitle;
        public string ParentTitle
        {
            get { return _ParentTitle; }
            set
            {
                _ParentTitle = value;
            }
        }

        public string UserName { get; set; }

        public string Password { get; set; }

        private bool _ConnectItemsAtStartup = false;
        public bool ConnectItemsAtStartup
        {
            get { return _ConnectItemsAtStartup; }
            set
            {
                _ConnectItemsAtStartup = value;
            }
        }

        private int _RemoveDisabledItemAfterSecs = 30;
        public int RemoveDisabledItemAfterSecs
        {
            get { return _RemoveDisabledItemAfterSecs; }
            set
            {
                _RemoveDisabledItemAfterSecs = value;
            }
        }

        private int _MaxCleanCount = 2;
        public int MaxCleanCount
        {
            get { return _MaxCleanCount; }
            set
            {
                _MaxCleanCount = value;
            }
        }

        private bool _UseAlwaysSecureConnections = false;
        public bool UseAlwaysSecureConnections
        {
            get { return _UseAlwaysSecureConnections; }
            set
            {
                _UseAlwaysSecureConnections = value;
            }
        }

        int _fastSamplingInterval = 250;
        public int FastSamplingInterval
        {
            get
            {
                return _fastSamplingInterval;
            }
            set
            {
                _fastSamplingInterval = value;
            }
        }

        int _slowSamplingInterval = 30000;
        public int SlowSamplingInterval
        {
            get
            {
                return _slowSamplingInterval;
            }
            set
            {
                _slowSamplingInterval = value;
            }
        }

        private bool _DisableWhenNotUsed = true;
        public bool DisableWhenNotUsed
        {
            get { return _DisableWhenNotUsed; }
            set
            {
                _DisableWhenNotUsed = value;
            }
        }

        int _publishingInterval = 250;
        public int PublishingInterval
        {
            get
            {
                return _publishingInterval;
            }
            set
            {
                _publishingInterval = value;
            }
        }

        String[] _serverArray;
        public String[] ServerArray
        {
            get
            {
                return _serverArray;
            }
            set
            {
                _serverArray = value;
            }
        }

        readonly Dictionary<String, AppNameSettings> mapAppNameSettings = new Dictionary<String, AppNameSettings>();
        public Dictionary<String, AppNameSettings> MapAppNameSettings
        {
            get
            {
                return mapAppNameSettings;
            }
            set
            {
                mapAppNameSettings.Clear();
                foreach(var entry in value.Keys)
                    mapAppNameSettings.Add(entry, value[entry]);
            }
        }
    }
}