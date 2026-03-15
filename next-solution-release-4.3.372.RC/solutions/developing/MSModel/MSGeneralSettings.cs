using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using Opc.Ua;

namespace MSModel
{
    // changed the table name for compatibility reason (see https://support.progea.com/Products/default.asp?9768)
    [Persistent("MSGeneralSettingsEx")]
    public class MSGeneralSettings : UFUAModel.ConfigurationBase
    {
        #region Ctor
        public MSGeneralSettings(Session session)
            : base(session)
        {
            ServerConfiguration conf = MSServerInfo.MSServerInfo.GetCurrentServerConfiguration();

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
        #endregion

        #region Properties

        private string _UserConnectionString;
        [Size(SizeAttribute.Unlimited)]
        public string UserConnectionString
        {
            get
            {
                return _UserConnectionString;
            }
            set
            {
                SetPropertyValue("UserConnectionString", ref _UserConnectionString, value);
            }
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

        #endregion

        #region Overrides

        public override int GetDefaultPort(string transport)
        {
            switch (transport)
            {
#if !NET_STANDARD
                case Opc.Ua.Utils.UriSchemeHttp:
                    return (int)MSModel.TransportDefaultPorts.UriSchemeHttpPort;
#endif
                case Opc.Ua.Utils.UriSchemeHttps:
                    return (int)MSModel.TransportDefaultPorts.UriSchemeHttpsPort;
#if !NET_STANDARD
                case Opc.Ua.Utils.UriSchemeNetPipe:
                    return (int)MSModel.TransportDefaultPorts.UriSchemeNetPipePort;
                case Opc.Ua.Utils.UriSchemeNetTcp:
                    return (int)MSModel.TransportDefaultPorts.UriSchemeNetTcpPort;
                case Opc.Ua.Utils.UriSchemeNoSecurityHttp:
                    return (int)MSModel.TransportDefaultPorts.UriSchemeNoSecurityHttpPort;
#endif
                case Opc.Ua.Utils.UriSchemeOpcTcp:
                    return (int)MSModel.TransportDefaultPorts.UriSchemeOpcTcpPort;
                default:
                    throw new ArgumentException(String.Format("Invalid transport scheme name '{0}'", transport));
            }
        }

        public override void EnsureDefaultBaseAddresses(Session session)
        {
            if (BaseAddresses.Count == 0)
            {
                var baseAddresses = MSServerInfo.MSServerInfo.GetCurrentApplicationBaseAddresses();
                if (baseAddresses.Count > 0)
                {
                    var uri = Opc.Ua.Utils.ParseUri(baseAddresses[0]);
                    if (uri != null)
                    {
                        BaseAddresses.Add(new MSBaseAddress(session)
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
                    BaseAddresses.Add(new MSBaseAddress(session)
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
