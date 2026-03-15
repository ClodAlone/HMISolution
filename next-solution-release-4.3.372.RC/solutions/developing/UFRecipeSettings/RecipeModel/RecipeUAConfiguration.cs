using System;
using System.ComponentModel;
using DevExpress.Xpo;
using Opc.Ua;
using UFInterfaces.PropertyControl;

namespace UFRecipeSettings.UFRecipeModel
{
    // changed the table name for compatibility reason (see https://support.progea.com/Products/default.asp?9768)
    [Persistent("RecipeUAConfiguration")]
    [DeferredDeletion(false)]
    public class RecipeUAConfiguration : UFUAModel.ConfigurationBase, INotifyPropertyVisibilityChanged
    {
        #region Ctor
        public RecipeUAConfiguration(Session session)
            : base(session)
        {
            ServerConfiguration conf = RecipeUAServerInfo.RecipeUAServerInfo.GetCurrentServerConfiguration();

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
            UseSharedRedundancyServers = true;            
        }
        #endregion

        #region Properties Default Values
        // List of constant default values for each property where you want handle a default value.
        //const int defaultPropertyName = -1;
#if !NET_STANDARD
        const string defaultTransport = Opc.Ua.Utils.UriSchemeNetPipe;
#else
        const string defaultTransport = Opc.Ua.Utils.UriSchemeOpcTcp;
#endif
        const uint defaultRedundancyPortNumber = 40100;
        #endregion

        #region Properties
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

        private TimeSpan? _TransactionLogMaxAge = new TimeSpan(10, 0, 0, 0);
        [Category("Redundancy")]
        public TimeSpan? TransactionLogMaxAge
        {
            get
            {
                return _TransactionLogMaxAge;
            }
            set
            {
                SetPropertyValue(nameof(TransactionLogMaxAge), ref _TransactionLogMaxAge, value);
            }
        }

        private bool _UseSharedRedundancyServers;
        [Category("Redundancy")]
        public bool UseSharedRedundancyServers
        {
            get
            {
                return _UseSharedRedundancyServers;
            }
            set
            {
                if (SetPropertyValue("UseSharedRedundancyServers", ref _UseSharedRedundancyServers, value))
                    OnPropertyVisiblityChanged(nameof(UseSharedRedundancyServers));
            }
        }

        private string _RecipeRedundancyServerList;
        [Browsable(false)]
        [NonPersistent]
        public string RecipeRedundancyServerList
        {
            get
            {
                return _RecipeRedundancyServerList;
            }
            set
            {
                if (_RecipeRedundancyServerList != value)
                {
                    _RecipeRedundancyServerList = value;
                    RaisePropertyChangedEvent(nameof(RedundancyKeepActiveServerActive));
                }
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

        protected override void EnsureDefaultValues()
        {
            if (!_RedundancyPortNumber.HasValue)
                _RedundancyPortNumber = defaultRedundancyPortNumber;
            if (_RedundancyTimeout == TimeSpan.Zero)
                _RedundancyTimeout = TimeSpan.FromSeconds(30);
            if (!_TransactionLogMaxAge.HasValue)
                _TransactionLogMaxAge = TimeSpan.FromDays(10);

            base.EnsureDefaultValues();
        }

        protected override string PerformValidation(string propertyName, string prototypeModel = null)
        {
            if (propertyName == "RedundancyKeepActiveServerActive")
            {
                if (RedundancyKeepActiveServerActive.Value && !String.IsNullOrEmpty(RecipeRedundancyServerList))
                {
                    var servers = RecipeRedundancyServerList.Split(new Char[] { ',' });
                    if (servers.Length > 2)
                        return Properties.Resources.InvalidKeepActiveServerActiveOption;
                    return null;
                }
            }
            return base.PerformValidation(propertyName, prototypeModel);
        }
        #endregion

        #region Overrides

        public override int GetDefaultPort(string transport)
        {
            switch (transport)
            {
#if !NET_STANDARD
                case Opc.Ua.Utils.UriSchemeHttp:
                    return (int)TransportDefaultPorts.UriSchemeHttpPort;
#endif
                case Opc.Ua.Utils.UriSchemeHttps:
                    return (int)TransportDefaultPorts.UriSchemeHttpsPort;
#if !NET_STANDARD                    
                case Opc.Ua.Utils.UriSchemeNetPipe:
                    return (int)TransportDefaultPorts.UriSchemeNetPipePort;
                case Opc.Ua.Utils.UriSchemeNetTcp:
                    return (int)TransportDefaultPorts.UriSchemeNetTcpPort;
                case Opc.Ua.Utils.UriSchemeNoSecurityHttp:
                    return (int)TransportDefaultPorts.UriSchemeNoSecurityHttpPort;
#endif
                case Opc.Ua.Utils.UriSchemeOpcTcp:
                    return (int)TransportDefaultPorts.UriSchemeOpcTcpPort;
                default:
                    throw new ArgumentException(String.Format("Invalid transport scheme name '{0}'", transport));
            }
        }

        public override void EnsureDefaultBaseAddresses(Session session)
        {
            if (BaseAddresses.Count == 0)
            {
                var baseAddresses = RecipeUAServerInfo.RecipeUAServerInfo.GetCurrentApplicationBaseAddresses();
                if (baseAddresses.Count > 0)
                {
                    var uri = Opc.Ua.Utils.ParseUri(baseAddresses[0]);
                    if (uri != null)
                    {
                        BaseAddresses.Add(new RecipeUABaseAddress(session)
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
                    BaseAddresses.Add(new RecipeUABaseAddress(session)
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
                if (propertyName == "ListRedundancyServers")
                    return !UseSharedRedundancyServers;
                return true;
            }
        }

        #endregion
    }
}
