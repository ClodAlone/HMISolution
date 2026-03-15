using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Microsoft.ServiceModel.Samples.Discovery;
using System.ServiceModel.Description;
using System.Xml;
using System.Diagnostics;

namespace LoggerSvc
{
    class LoggerSvc: ILoggerSvc
    {
        #region Declaration
        ServiceHost m_Host;
        #region Discovery
        List<ServiceProperties> publishedServices = new List<ServiceProperties>();
        ServicePublisher publisher = new ServicePublisher();
        #endregion
        #endregion

        protected override void OnStart(string[] args)
        {
            EventLog.WriteEntry(Properties.Resource.ServiceStarting,
                     System.Diagnostics.EventLogEntryType.Information);

            m_Host = new ServiceHost(typeof(LoggerSvc));

            NetNamedPipeBinding pipebinding = new NetNamedPipeBinding();
            m_Host.AddServiceEndpoint(typeof(LoggerSvc), pipebinding, MonitorNetPipeServiceAddress);

            BasicHttpBinding httpbinding = new BasicHttpBinding();
            m_Host.AddServiceEndpoint(typeof(LoggerSvc), httpbinding, MonitorBasicHttpServiceAddress);

            m_Host.Open();

            PublishService();
            EventLog.WriteEntry(Properties.Resource.ServiceStarted,
                     System.Diagnostics.EventLogEntryType.Information);
        }

        protected override void OnStop()
        {
            EventLog.WriteEntry(Properties.Resource.ServiceStopping,
                     System.Diagnostics.EventLogEntryType.Information);

            UnpublishService();
            m_Host.Close();

            EventLog.WriteEntry(Properties.Resource.ServiceStopped,
                     System.Diagnostics.EventLogEntryType.Information);
        }

        #region Publishing
        private void PublishService()
        {
            int serviceNumber = 1;
            ServiceProperties props = null;
            foreach (ServiceEndpoint endpoint in m_Host.Description.Endpoints)
            {
                props = new ServiceProperties(endpoint.Address);
                props.Types = new ContactTypeCollection();
                props.Types.Add(new XmlQualifiedName(endpoint.Contract.Name,
                    endpoint.Contract.Namespace));
                props.Scopes = new ScopeCollection();
                props.Scopes.Add(new Uri("http://myofice"));

                XmlDocument doc = new XmlDocument();
                XmlElement customElement = doc.CreateElement("CustomMetadata", "http://custom");
                XmlElement myScope = doc.CreateElement("CustomScope", "http://custom");
                XmlElement myNumber = doc.CreateElement("CustomNumber", serviceNumber.ToString());
                myScope.InnerText = "uri://custom/localnetwork/services/Echo";
                customElement.AppendChild(myScope);
                customElement.AppendChild(myNumber);

                props.AnyElements = new XmlElement[1];
                props.AnyElements[0] = customElement;

                publisher.Publish(props);
                publishedServices.Add(props);

                ++serviceNumber;
            }
        }

        private void UnpublishService()
        {
            foreach (ServiceProperties props in publishedServices)
            {
                publisher.Unpublish(props);
            }
        }
        #endregion
    }
}
