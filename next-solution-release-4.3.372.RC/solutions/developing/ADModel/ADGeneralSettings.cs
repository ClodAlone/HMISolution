using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using Opc.Ua;
using UFInterfaces.PropertyControl;

namespace ADModel
{
    // changed the table name for compatibility reason (see https://support.progea.com/Products/default.asp?9768)
    [Persistent("ADGeneralSettingsEx")]
    public class ADGeneralSettings : UFUAModel.ConfigurationBase, INotifyPropertyVisibilityChanged
    {
        #region Ctor
        public ADGeneralSettings(Session session)
            : base(session)
        {
            ServerConfiguration conf = ADServerInfo.ADServerInfo.GetCurrentServerConfiguration();

            DiagnosticEnabled = conf.DiagnosticsEnabled;
            PublishingResolution = conf.PublishingResolution;
            MinMetadataSamplingInterval = conf.MinMetadataSamplingInterval;
            MaxBrowseContinuationPoints = conf.MaxBrowseContinuationPoints;
            MaxHistoryContinuationPoints = conf.MaxHistoryContinuationPoints;
            MaxMessageQueueSize = conf.MaxMessageQueueSize;
            MaxNotificationQueueSize = conf.MaxNotificationQueueSize;
            MaxNotificationsPerPublish = conf.MaxNotificationsPerPublish;
            MinPublishingInterval = conf.MinPublishingInterval;
            MaxPublishingInterval = conf.MaxPublishingInterval;
            MaxRequestAge = conf.MaxRequestAge;
            MaxSessionCount = conf.MaxSessionCount;
            MinSessionTimeout = conf.MinSessionTimeout;
            MaxSessionTimeout = conf.MaxSessionTimeout;
            MaxSubscriptionLifetime = conf.MaxSubscriptionLifetime;
            UseLocalDateTime = false;
            NotifyEveryStateChange = true;
        }
        #endregion

        #region Properties Default Values
#if !NET_STANDARD
        const string defaultTransport = Opc.Ua.Utils.UriSchemeNetPipe;
#else
        const string defaultTransport = Opc.Ua.Utils.UriSchemeOpcTcp;
#endif

        const bool defaultCustomMessage = false;
        const bool defaultAddDateTime = true;
        const bool defaultAddAlarmState = true;
        const bool defaultAddNotificationText = true;
        const bool defaultAddServerText = true;
        const bool defaultAddNotificationName = true;
        const uint defaultErrorDelay = 0;

        protected override void EnsureDefaultValues()
        {
            if (!CustomMessage.HasValue)
                CustomMessage = defaultCustomMessage;
            if (!AddDateTime.HasValue)
                AddDateTime = defaultAddDateTime;
            if (!AddAlarmState.HasValue)
                AddAlarmState = defaultAddAlarmState;
            if (!AddNotificationText.HasValue)
                AddNotificationText = defaultAddNotificationText;
            if (!AddServerText.HasValue)
                AddServerText = defaultAddServerText;
            if (!AddNotificationName.HasValue)
                AddNotificationName = defaultAddNotificationName;
            if (!ErrorDelay.HasValue)
                ErrorDelay = defaultErrorDelay;

            base.EnsureDefaultValues();
        }

        #endregion

        #region Properties
        private bool _UseLocalDateTime;
        public bool UseLocalDateTime
        {
            get { return _UseLocalDateTime; }
            set { SetPropertyValue("UseLocalDateTime", ref _UseLocalDateTime, value); }
        }

        //private bool _EnableLog;
        //public bool EnableLog
        //{
        //    get
        //    {
        //        return _EnableLog;
        //    }
        //    set
        //    {
        //        SetPropertyValue("EnableLog", ref _EnableLog, value);
        //    }
        //}

        private int _ErrorThreshold;
        public int ErrorThreshold
        {
            get
            {
                return _ErrorThreshold;
            }
            set
            {
                SetPropertyValue("ErrorThreshold", ref _ErrorThreshold, value);
            }
        }

        private uint? _ErrorDelay;
        public uint? ErrorDelay
        {
            get
            {
                return _ErrorDelay;
            }
            set
            {
                SetPropertyValue("ErrorDelay", ref _ErrorDelay, value);
            }
        }

        private int _PriorityDelay0;
        public int PriorityDelay0
        {
            get { return _PriorityDelay0; }
            set
            {
                SetPropertyValue("PriorityDelay0", ref _PriorityDelay0, value);
            }
        }
        private int _PriorityDelay1;
        public int PriorityDelay1
        {
            get { return _PriorityDelay1; }
            set
            {
                SetPropertyValue("PriorityDelay1", ref _PriorityDelay1, value);
            }
        }
        private int _PriorityDelay2;
        public int PriorityDelay2
        {
            get { return _PriorityDelay2; }
            set
            {
                SetPropertyValue("PriorityDelay2", ref _PriorityDelay2, value);
            }
        }
        private int _PriorityDelay3;
        public int PriorityDelay3
        {
            get { return _PriorityDelay3; }
            set
            {
                SetPropertyValue("PriorityDelay3", ref _PriorityDelay3, value);
            }
        }
        private int _PriorityDelay4;
        public int PriorityDelay4
        {
            get { return _PriorityDelay4; }
            set
            {
                SetPropertyValue("PriorityDelay4", ref _PriorityDelay4, value);
            }
        }
        private int _PriorityDelay5;
        public int PriorityDelay5
        {
            get { return _PriorityDelay5; }
            set
            {
                SetPropertyValue("PriorityDelay5", ref _PriorityDelay5, value);
            }
        }
        private int _PriorityDelay6;
        public int PriorityDelay6
        {
            get { return _PriorityDelay6; }
            set
            {
                SetPropertyValue("PriorityDelay6", ref _PriorityDelay6, value);
            }
        }
        private int _PriorityDelay7;
        public int PriorityDelay7
        {
            get { return _PriorityDelay7; }
            set
            {
                SetPropertyValue("PriorityDelay7", ref _PriorityDelay7, value);
            }
        }
        private int _PriorityDelay8;
        public int PriorityDelay8
        {
            get { return _PriorityDelay8; }
            set
            {
                SetPropertyValue("PriorityDelay8", ref _PriorityDelay8, value);
            }
        }
        private int _PriorityDelay9;
        public int PriorityDelay9
        {
            get { return _PriorityDelay9; }
            set
            {
                SetPropertyValue("PriorityDelay9", ref _PriorityDelay9, value);
            }
        }

        private bool? _CustomMessage;
        public bool? CustomMessage
        {
            get { return _CustomMessage; }
            set { 
                if(SetPropertyValue("CustomMessage", ref _CustomMessage, value))
                {
                    OnPropertyVisiblityChanged("CustomMessage");
                }
            }
        }

        private bool? _AddDateTime;
        public bool? AddDateTime
        {
            get { return _AddDateTime; }
            set { SetPropertyValue("AddDateTime", ref _AddDateTime, value); }
        }
        private bool? _AddAlarmState;
        public bool? AddAlarmState
        {
            get { return _AddAlarmState; }
            set { SetPropertyValue("AddAlarmState", ref _AddAlarmState, value); }
        }
        private bool? _AddNotificationText;
        public bool? AddNotificationText
        {
            get { return _AddNotificationText; }
            set { SetPropertyValue("AddNotificationText", ref _AddNotificationText, value); }
        }
        private bool? _AddServerText;
        public bool? AddServerText
        {
            get { return _AddServerText; }
            set { SetPropertyValue("AddServerText", ref _AddServerText, value); }
        }
        private bool? _AddNotificationName;
        public bool? AddNotificationName
        {
            get { return _AddNotificationName; }
            set { SetPropertyValue("AddNotificationName", ref _AddNotificationName, value); }
        }

        [Association("ADGeneralSettings-ADPlugins"), Aggregated]
        public XPCollection<ADPlugin> ADPlugins
        {
            get
            {
                return GetCollection<ADPlugin>("ADPlugins");
            }
        }

        /*
        [Association("ADGeneralSettings-ADNotifications")]
        public XPCollection<ADNotification> ADNotifications
        {
            get
            {
                return GetCollection<ADNotification>("ADNotifications");
            }
        }
        */

        private bool _NotifyEveryStateChange;
        public bool NotifyEveryStateChange
        {
            get { return _NotifyEveryStateChange; }
            set { SetPropertyValue("NotifyEveryStateChange", ref _NotifyEveryStateChange, value); }
        }
        #endregion

        #region Overrides

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

        public override int GetDefaultPort(string transport)
        {
            switch (transport)
            {
#if !NET_STANDARD
                case Opc.Ua.Utils.UriSchemeHttp:
                    return (int)ADModel.TransportDefaultPorts.UriSchemeHttpPort;
#endif
                case Opc.Ua.Utils.UriSchemeHttps:
                    return (int)ADModel.TransportDefaultPorts.UriSchemeHttpsPort;
#if !NET_STANDARD
                case Opc.Ua.Utils.UriSchemeNetPipe:
                    return (int)ADModel.TransportDefaultPorts.UriSchemeNetPipePort;
                case Opc.Ua.Utils.UriSchemeNetTcp:
                    return (int)ADModel.TransportDefaultPorts.UriSchemeNetTcpPort;
                case Opc.Ua.Utils.UriSchemeNoSecurityHttp:
                    return (int)ADModel.TransportDefaultPorts.UriSchemeNoSecurityHttpPort;
#endif
                case Opc.Ua.Utils.UriSchemeOpcTcp:
                    return (int)ADModel.TransportDefaultPorts.UriSchemeOpcTcpPort;
                default:
                    throw new ArgumentException(String.Format("Invalid transport scheme name '{0}'", transport));
            }
        }

        public override void EnsureDefaultBaseAddresses(Session session)
        {
            if (BaseAddresses.Count == 0)
            {
                var baseAddresses = ADServerInfo.ADServerInfo.GetCurrentApplicationBaseAddresses();
                if (baseAddresses.Count > 0)
                {
                    var uri = Opc.Ua.Utils.ParseUri(baseAddresses[0]);
                    if (uri != null)
                    {
                        BaseAddresses.Add(new ADModel.ADBaseAddress(session)
                        {
                            Enabled = true,
                            Transport = uri.Scheme,
                            Server = uri.Host,
                            Port = uri.Port
                        });
                    }
                }

                if (BaseAddresses.Count == 0)
                {
                    BaseAddresses.Add(new ADModel.ADBaseAddress(session)
                    {
                        Enabled = true,
                        Transport = defaultTransport,
                        Server = "localhost",
                        Port = GetDefaultPort(defaultTransport)
                    });
                }
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
#if !NET_STANDARD
                if (propertyName == "AddDateTime" || propertyName == "AddAlarmState" || propertyName == "AddNotificationText" ||
                    propertyName == "AddServerText" || propertyName == "AddNotificationName")
                {
                    
                    return (CustomMessage.HasValue && CustomMessage.Value);
                }
#endif
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
    }
}
