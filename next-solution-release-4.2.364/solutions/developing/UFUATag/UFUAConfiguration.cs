using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Xpo;
using Opc.Ua;

namespace UFUAModel
{
    // changed the table name for compatibility reason (see https://support.progea.com/Products/default.asp?9768)
    [Persistent("UFUAConfigurationEx")]
    [DeferredDeletion(false)]
    public class UFUAConfiguration : ConfigurationBase
    {
        #region Ctor
        public UFUAConfiguration(Session session)
            : base(session)
        {
            ServerConfiguration conf = UFUAServerInfo.UFUAServerInfo.GetCurrentServerConfiguration();

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

        /// <summary>
        /// Adds inside this method the nullable property where you want handle a default value.
        /// </summary>
        private void EnsureDefaultValues()
        {
            // Examples of how to handle a default value
            //if (!_PropertyName.HasValue)
            //    _PropertyName = defaultPropertyName;
            //if (_TimeSpanPropertyName == TimeSpan.Zero)
            //    _TimeSpanPropertyName = TimeSpan.FromMinutes(1);
            //if (_DateTimePropertyName == DateTime.MinValue)
            //    _DateTimePropertyName = DateTime.UtcNow;
        }
        #endregion

        #region Properties

        [Association("UFUAConfiguration-CommDrivers"), Aggregated]
        public XPCollection<UFUACommunicationDriver> ComunicationDrivers
        {
            get
            {
                return GetCollection<UFUACommunicationDriver>("ComunicationDrivers");
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

        #region Overrides

        public override int GetDefaultPort(string transport)
        {
            switch (transport)
            {
#if !NET_STANDARD
                case Opc.Ua.Utils.UriSchemeHttp:
                    return (int)UFUAModel.TransportDefaultPorts.UriSchemeHttpPort;
#endif
                case Opc.Ua.Utils.UriSchemeHttps:
                    return (int)UFUAModel.TransportDefaultPorts.UriSchemeHttpsPort;
#if !NET_STANDARD                    
                case Opc.Ua.Utils.UriSchemeNetPipe:
                    return (int)UFUAModel.TransportDefaultPorts.UriSchemeNetPipePort;
                case Opc.Ua.Utils.UriSchemeNetTcp:
                    return (int)UFUAModel.TransportDefaultPorts.UriSchemeNetTcpPort;
                case Opc.Ua.Utils.UriSchemeNoSecurityHttp:
                    return (int)UFUAModel.TransportDefaultPorts.UriSchemeNoSecurityHttpPort;
#endif
                case Opc.Ua.Utils.UriSchemeOpcTcp:
                    return (int)UFUAModel.TransportDefaultPorts.UriSchemeOpcTcpPort;
                default:
                    throw new ArgumentException(String.Format("Invalid transport scheme name '{0}'", transport));
            }
        }

        public override void EnsureDefaultBaseAddresses(Session session)
        {
            if (BaseAddresses.Count == 0)
            {
                var baseAddresses = UFUAServerInfo.UFUAServerInfo.GetCurrentApplicationBaseAddresses();
                if (baseAddresses.Count > 0)
                {
                    var uri = Opc.Ua.Utils.ParseUri(baseAddresses[0]);
                    if (uri != null)
                    {
                        BaseAddresses.Add(new UFUAModel.UFUABaseAddress(session)
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
                    BaseAddresses.Add(new UFUAModel.UFUABaseAddress(session)
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
    }
}
