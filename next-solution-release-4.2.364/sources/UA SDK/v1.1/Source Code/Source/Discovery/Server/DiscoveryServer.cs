/* ========================================================================
 * Copyright (c) 2005-2010 The OPC Foundation, Inc. All rights reserved.
 *
 * OPC Reciprocal Community Binary License ("RCBL") Version 1.00
 * 
 * Unless explicitly acquired and licensed from Licensor under another 
 * license, the contents of this file are subject to the Reciprocal 
 * Community Binary License ("RCBL") Version 1.00, or subsequent versions 
 * as allowed by the RCBL, and You may not copy or use this file in either 
 * source code or executable form, except in compliance with the terms and 
 * conditions of the RCBL.
 * 
 * All software distributed under the RCBL is provided strictly on an 
 * "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, 
 * AND LICENSOR HEREBY DISCLAIMS ALL SUCH WARRANTIES, INCLUDING WITHOUT 
 * LIMITATION, ANY WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR 
 * PURPOSE, QUIET ENJOYMENT, OR NON-INFRINGEMENT. See the RCBL for specific 
 * language governing rights and limitations under the RCBL.
 *
 * The complete license agreement can be found here:
 * http://opcfoundation.org/License/RCBL/1.00/
 * ======================================================================*/

using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace Opc.Ua.Discovery
{
    /// <summary>
    /// The standard implementation of a UA discovery server.
    /// </summary>
    public partial class DiscoveryServer : DiscoveryServerBase
    {
        #region Constructors
        /// <summary>
        /// Initializes the object with default values.
        /// </summary>
        public DiscoveryServer()
        {
            m_servers = new Dictionary<string, Registration>();
        }
        #endregion
                
        #region Overridden Methods
        /// <summary cref="ServerBase.StartApplication" />
        protected override void StartApplication(ApplicationConfiguration configuration)
        {
            base.StartApplication(configuration);
                                        
            lock (m_lock)
            {
                try
                {
                    m_saveFilePath = configuration.DiscoveryServerConfiguration.DiscoveryServerCacheFile;
                    m_serverNames  = new LocalizedTextCollection(configuration.DiscoveryServerConfiguration.ServerNames);

                    // trim the file name.
                    if (m_saveFilePath != null)
                    {
                        m_saveFilePath = m_saveFilePath.Trim();
                    }
                    
                    // check if caching is enabled.
                    if (!String.IsNullOrEmpty(m_saveFilePath))
                    {                    
                        string saveFilePath = Utils.GetAbsoluteFilePath(m_saveFilePath, true, false, false);

                        // load the cached servers from disk.
                        if (saveFilePath != null && File.Exists(saveFilePath))
                        {                       
                            LoadCachedServers(saveFilePath);
                        }
                    }

                    // add a registration for the discovery server itself.
                    Registration registration = new Registration();

                    registration.Server = new RegisteredServer();
                    registration.Server.IsOnline = false;
                    registration.Server.ProductUri = configuration.ProductUri;
                    registration.Server.ServerNames.Add(configuration.ApplicationName);
                    registration.Server.ServerType = ApplicationType.DiscoveryServer;
                    registration.Server.ServerUri = configuration.ApplicationUri;
                    registration.Server.DiscoveryUrls = GetDiscoveryUrls();

                    registration.DiscoveryUrls = new Uri[registration.Server.DiscoveryUrls.Count];

                    for (int ii = 0; ii < registration.Server.DiscoveryUrls.Count; ii++)
                    {
                        registration.DiscoveryUrls[ii] = new Uri(registration.Server.DiscoveryUrls[ii]);
                    }

                    registration.LastUpdateTime = DateTime.UtcNow;
                    m_servers[configuration.ApplicationUri] = registration;

                    ServerError = null;
                }
                catch (Exception e)
                {
                    ServerError = new ServiceResult(e);
                    throw new ServiceResultException(ServerError);
                }
            }
        }      
        
        /// <summary cref="ServerBase.InitializeServiceHosts" />
        protected override IList<ServiceHost> InitializeServiceHosts(
            ApplicationConfiguration          configuration, 
            BindingFactory                    bindingFactory,
            out ApplicationDescription        serverDescription,
            out EndpointDescriptionCollection endpoints)      
        {                        
            // ensure at least one security policy exists.
            if (configuration.DiscoveryServerConfiguration.SecurityPolicies.Count == 0)
            {   
                // must support an secured endpoint for registration.
                ServerSecurityPolicy policy = new ServerSecurityPolicy();

                policy.SecurityMode = MessageSecurityMode.Sign;
                policy.SecurityPolicyUri = SecurityPolicies.Basic128Rsa15;

                configuration.DiscoveryServerConfiguration.SecurityPolicies.Add(policy);
            }
            
            // create the server description.
            serverDescription = new ApplicationDescription();

            serverDescription.ApplicationUri = configuration.ApplicationUri;
            serverDescription.ApplicationName = configuration.ApplicationName;
            serverDescription.ApplicationType = configuration.ApplicationType;
            serverDescription.ProductUri = configuration.ProductUri;
            serverDescription.DiscoveryUrls = GetDiscoveryUrls();
            
            Dictionary<string,ServiceHost> hosts = new Dictionary<string,ServiceHost>();

            endpoints = new EndpointDescriptionCollection();
            IList<EndpointDescription> endpointsForHost = null;
            
            // create the discovery endpoints for the WS-* endpoints.
            List<Uri> webserviceUrls = new List<Uri>();

            for (int ii = 0; ii < serverDescription.DiscoveryUrls.Count; ii++)
            {
                int index = serverDescription.DiscoveryUrls[ii].LastIndexOf("/discovery");

                if (index != -1)
                {
                    serverDescription.DiscoveryUrls[ii] = serverDescription.DiscoveryUrls[ii].Substring(0, index);
                    webserviceUrls.Add(new Uri(serverDescription.DiscoveryUrls[ii]));
                }
            }

            if (webserviceUrls.Count > 0)
            {
                ServiceHost host = new ServiceHost(this, typeof(DiscoveryEndpoint), webserviceUrls.ToArray());

                foreach (Uri webserviceUrl in webserviceUrls)
                {
                    System.ServiceModel.Channels.Binding binding = bindingFactory.Create(webserviceUrl.Scheme, EndpointConfiguration.Create(configuration));
                    host.AddServiceEndpoint(typeof(IDiscoveryEndpoint), binding, webserviceUrl, webserviceUrl);
                }

                hosts[String.Empty] = host;
            }

            // create hosts for protocols that require one endpoints per security policy
            foreach (ServerSecurityPolicy securityPolicy in configuration.DiscoveryServerConfiguration.SecurityPolicies)
            {
                endpointsForHost = CreateSinglePolicyServiceHost(
                    hosts,
                    configuration,
                    bindingFactory,
                    configuration.DiscoveryServerConfiguration.BaseAddresses,
                    serverDescription,
                    securityPolicy.SecurityMode, 
                    securityPolicy.SecurityPolicyUri,
                    "/registration");

                for (int ii = 0; ii < endpointsForHost.Count; ii++)
                {
                    endpointsForHost[ii].SecurityLevel = securityPolicy.SecurityLevel;
                }

                endpoints.AddRange(endpointsForHost);
            }

            // create UA TCP host.
            endpointsForHost = CreateUaTcpServiceHost(
                hosts,
                configuration,
                bindingFactory,
                configuration.DiscoveryServerConfiguration.BaseAddresses,
                serverDescription,
                configuration.DiscoveryServerConfiguration.SecurityPolicies);

            endpoints.InsertRange(0, endpointsForHost);

            // create HTTPS host.
            endpointsForHost = CreateHttpsServiceHost(
                hosts,
                configuration,
                bindingFactory,
                configuration.DiscoveryServerConfiguration.BaseAddresses,
                serverDescription,
                configuration.DiscoveryServerConfiguration.SecurityPolicies);

            endpoints.AddRange(endpointsForHost);

            return new List<ServiceHost>(hosts.Values);
        }

        /// <summary>
        /// Creates an instance of the service host.
        /// </summary>
        protected override ServiceHost CreateServiceHost(ServerBase server, params Uri[] addresses)
        {
            return new ServiceHost(this, typeof(DiscoveryEndpoint), addresses);
        }

        /// <summary>
        /// Returns the service contract to use.
        /// </summary>
        protected override Type GetServiceContract()
        {
            return typeof(IRegistrationEndpoint);
        }

        /// <summary>
        /// Returns an instance of the endpoint to use.
        /// </summary>
        protected override EndpointBase GetEndpointInstance(ServerBase server)
        {
            return new DiscoveryEndpoint(server);
        }
        #endregion
                        
        #region IDiscoveryEndpoint Methods
        /// <summary cref="IDiscoveryServer.FindServers" />
        public override ResponseHeader FindServers(
            RequestHeader                        requestHeader, 
            string                               endpointUrl, 
            StringCollection                     localeIds, 
            StringCollection                     serverUris, 
            out ApplicationDescriptionCollection servers)
        {
            servers = null;
            
            ValidateRequest(requestHeader);
            
            lock (m_lock)
            { 
                servers = new ApplicationDescriptionCollection();
                Uri parsedEndpointUrl = Utils.ParseUri(endpointUrl);

                // find the servers that meet the criteria.
                foreach (Registration registration in m_servers.Values)
                {
                    RegisteredServer server = registration.Server;

                    // do not return out of date servers.
                    if (server.IsOnline)
                    {
                        if (registration.LastUpdateTime.AddMinutes(10) < DateTime.UtcNow)
                        {
                            continue;
                        }
                    }

                    // check semaphore file.
                    if (!String.IsNullOrEmpty(server.SemaphoreFilePath))
                    {
                        if (!File.Exists(server.SemaphoreFilePath))
                        {
                            continue;
                        }                        
                    }

                    // check client is filtering by server uri.
                    if (serverUris != null && serverUris.Count > 0)
                    {
                        if (!serverUris.Contains(server.ServerUri))
                        {
                            continue;
                        }
                    }

                    // create application description.
                    ApplicationDescription application = new ApplicationDescription();

                    application.ApplicationName     = Utils.SelectLocalizedText(localeIds, server.ServerNames, server.ServerNames[0]);
                    application.ApplicationUri      = server.ServerUri;
                    application.ApplicationType     = server.ServerType;
                    application.ProductUri          = server.ProductUri;
                    application.GatewayServerUri    = server.GatewayServerUri;
                    application.DiscoveryProfileUri = null;
                    application.DiscoveryUrls       = FilterAndFormatDiscoveryUrls(parsedEndpointUrl, registration.DiscoveryUrls);

                    // do not return servers that cannot be accessed.
                    if (application.DiscoveryUrls.Count == 0)
                    {
                        continue;
                    }
                    
                    // add to list of servers to return.
                    servers.Add(application);
                }
                    
                Utils.Trace("Discovery - FIND SERVERS. Returning {1} of {0} servers.", m_servers.Count, servers.Count);
            }
                            
            return CreateResponse(requestHeader, StatusCodes.Good);
        }


        /// <summary>
        /// Filters the list of addresses by the URL that the client provided.
        /// </summary>
        protected StringCollection FilterAndFormatDiscoveryUrls(Uri endpointUrl, Uri[] discoveryUrls)
        {
            StringCollection output = new StringCollection();

            // client gets all of the endpoints if it using a known variant of the hostname.
            if (endpointUrl == null || NormalizeHostname(endpointUrl.DnsSafeHost) == NormalizeHostname("localhost"))
            {
                for (int ii = 0; ii < discoveryUrls.Length; ii++)
                {
                    UriBuilder builder = new UriBuilder(discoveryUrls[ii]);

                    if (endpointUrl != null)
                    {
                        builder.Host = endpointUrl.Host;
                    }

                    output.Add(builder.ToString());
                }

                return output;
            }

            // client only gets alternate addresses that match the DNS name that it used.
            for (int ii = 0; ii < discoveryUrls.Length; ii++)
            {
                if (discoveryUrls[ii].DnsSafeHost == endpointUrl.DnsSafeHost)
                {
                    output.Add(discoveryUrls[ii].ToString());
                    continue;
                }
            }

            // no match on client DNS name. client gets only addresses that match the scheme.
            if (output.Count == 0)
            {
                for (int ii = 0; ii < discoveryUrls.Length; ii++)
                {
                    if (discoveryUrls[ii].Scheme == endpointUrl.Scheme)
                    {
                        UriBuilder builder = new UriBuilder(discoveryUrls[ii]);
                        builder.Host = endpointUrl.Host;
                        output.Add(builder.ToString());
                        continue;
                    }
                }
            }

            return output;
        }

        /// <summary cref="IDiscoveryServer.GetEndpoints" />
        public override ResponseHeader GetEndpoints(
            RequestHeader                     requestHeader, 
            string                            endpointUrl, 
            StringCollection                  localeIds, 
            StringCollection                  profileUris, 
            out EndpointDescriptionCollection endpoints)
        { 
            endpoints = null;
            
            ValidateRequest(requestHeader);

            lock (m_lock)
            {
                // filter by profile.
                IList<BaseAddress> baseAddresses = FilterByProfile(profileUris, BaseAddresses);

                // filter by endpoint urll.
                Uri parsedEndpointUrl = Utils.ParseUri(endpointUrl);

                if (parsedEndpointUrl != null)
                {
                    baseAddresses = FilterByEndpointUrl(parsedEndpointUrl, baseAddresses);
                }

                if (baseAddresses.Count != 0)
                {
                    // localize the application name if requested.
                    LocalizedText applicationName = this.ServerDescription.ApplicationName;

                    if (localeIds != null && localeIds.Count > 0)
                    {
                        applicationName = Utils.SelectLocalizedText(localeIds, m_serverNames, applicationName);
                    }

                    // get the application description.
                    ApplicationDescription application = TranslateApplicationDescription(
                        parsedEndpointUrl,
                        base.ServerDescription,
                        baseAddresses,
                        applicationName);

                    // get the application description.
                    endpoints = TranslateEndpointDescriptions(
                        parsedEndpointUrl,
                        baseAddresses,
                        this.Endpoints,
                        application);
                }
            }
                            
            return CreateResponse(requestHeader, StatusCodes.Good);
        }
        #endregion

        #region IRegistrationEndpoint
        /// <summary cref="IDiscoveryServer.RegisterServer" />
        public override ResponseHeader RegisterServer(RequestHeader requestHeader, RegisteredServer server)
        {
            ValidateRequest(requestHeader);

            // must verify that a secure channel is in place.
            bool secured = false;

            SecureChannelContext context = SecureChannelContext.Current;

            if (context != null && context.EndpointDescription != null)
            {
                EndpointDescription description = context.EndpointDescription;

                if (description.SecurityMode == MessageSecurityMode.Sign || description.SecurityMode == MessageSecurityMode.SignAndEncrypt)
                {
                    if (description.SecurityPolicyUri != SecurityPolicies.None)
                    {
                        secured = true;
                    }                    
                }
            }

            // throw an exception on error.
            if (!secured)
            {
                throw new ServiceResultException(StatusCodes.BadSecurityPolicyRejected);
            }

            lock (m_lock)
            {
                Utils.Trace("Discovery - REGISTER SERVER '{0}'", server.ServerUri);

                // validate server.
                ServiceResult error = ValidateRegisteredServer(server);

                if (ServiceResult.IsBad(error))
                {
                    Utils.Trace("Discovery - REGISTER SERVER FAILED. Error='{0}'", error);
                    throw new ServiceResultException(error);
                }

                // update server description.
                Registration registration = new Registration();

                registration.Server = server;
                registration.LastUpdateTime = DateTime.UtcNow;

                // cache list of discovery urls.
                registration.DiscoveryUrls = GetDiscoveryUrls(server);

                m_servers[server.ServerUri] = registration;

                // save offline registration to disk.
                if (!registration.Server.IsOnline)
                {
                    if (!String.IsNullOrEmpty(m_saveFilePath))
                    {
                        try
                        {
                            SaveCachedServers(m_saveFilePath);
                        }
                        catch (Exception e)
                        {
                            Utils.Trace(e, "Could not update file containing the cached servers.");
                        }
                    }
                }
            }
                            
            return CreateResponse(requestHeader, StatusCodes.Good);
        }
        #endregion
        
        #region Private Methods
        /// <summary>
        /// Gets the discovery urls.
        /// </summary>
        /// <param name="server">The server.</param>
        /// <returns></returns>
        private Uri[] GetDiscoveryUrls(RegisteredServer server)
        {
            // parse the discovery urls.
            List<Uri> discoveryUrls = new List<Uri>();

            for (int ii = 0; ii < server.DiscoveryUrls.Count; ii++)
            {
                Uri discoveryUrl = Utils.ParseUri(server.DiscoveryUrls[ii]);

                if (discoveryUrl != null)
                {
                    discoveryUrls.Add(discoveryUrl);
                }
            }

            // check if the discovery configuration file specifies addition registration information for the server.
            if (this.Configuration.DiscoveryServerConfiguration.ServerRegistrations == null)
            {
                return discoveryUrls.ToArray();
            }

            ServerRegistrationCollection registrations = this.Configuration.DiscoveryServerConfiguration.ServerRegistrations;

            for (int ii = 0; ii < registrations.Count; ii++)
            {
                if (registrations[ii].ApplicationUri != server.ServerUri)
                {
                    continue;
                }

                if (registrations[ii].AlternateDiscoveryUrls == null)
                {
                    continue;
                }

                // add the additional discovery urls to the list of discovery urls.
                bool found = false;

                for (int jj = 0; jj < registrations[ii].AlternateDiscoveryUrls.Count; jj++)
                {
                    Uri alternateUrl = Utils.ParseUri(registrations[ii].AlternateDiscoveryUrls[jj]);

                    if (alternateUrl == null)
                    {
                        continue;
                    }

                    for (int kk = 0; kk < discoveryUrls.Count; kk++)
                    {
                        if (String.Compare(discoveryUrls[ii].ToString(), alternateUrl.ToString(), StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        discoveryUrls.Add(alternateUrl);
                    }
                }
            }

            // return combined list.
            return discoveryUrls.ToArray();
        }

        /// <summary>
        /// Validates a registered server.
        /// </summary>
        private static ServiceResult ValidateRegisteredServer(RegisteredServer server)
        {
            // verify server uri.
            if (server == null || String.IsNullOrEmpty(server.ServerUri))
            {
                return new ServiceResult(StatusCodes.BadServerUriInvalid);
            }

            // verify server names.
            if (server.ServerNames == null || server.ServerNames.Count == 0)
            {
                return new ServiceResult(StatusCodes.BadBrowseNameInvalid);
            }
            
            // check semaphore file.
            if (!String.IsNullOrEmpty(server.SemaphoreFilePath))
            {
                if (!File.Exists(server.SemaphoreFilePath))
                {
                    return new ServiceResult(StatusCodes.BadSempahoreFileMissing);
                }                        
            }

            // check discovery urls
            if (server.DiscoveryUrls == null || server.DiscoveryUrls.Count == 0)
            {
                return new ServiceResult(StatusCodes.BadDiscoveryUrlMissing);
            }

            // check that the discovery urls are valid.
            foreach (string discoveryUrl in server.DiscoveryUrls)
            {
                if (Utils.ParseUri(discoveryUrl) == null)
                {
                    return new ServiceResult(StatusCodes.BadDiscoveryUrlMissing);
                }
            }

            // everything ok.
            return ServiceResult.Good;
        }

        /// <summary>
        /// Loads the servers from the cache file.
        /// </summary>
        private void LoadCachedServers(string filePath)
        {
			XmlTextReader reader = new XmlTextReader(File.Open(filePath, FileMode.Open, FileAccess.Read));

            try
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(RegisteredServerCollection));                
                RegisteredServerCollection servers = serializer.ReadObject(reader) as RegisteredServerCollection;

                m_servers.Clear();

                if (servers != null)
                {
                    for (int ii = 0; ii < servers.Count; ii++)
                    {
                        RegisteredServer server = servers[ii];

                        ServiceResult result = ValidateRegisteredServer(server);

                        if (ServiceResult.IsGood(result))
                        {
                            Registration registration = new Registration();

                            registration.Server = server;
                            registration.LastUpdateTime = DateTime.UtcNow;
                            registration.DiscoveryUrls = GetDiscoveryUrls(server);

                            m_servers[server.ServerUri] = registration;
                        }
                    }
                }
            }
            finally
            {
                reader.Close();
            }
        }

        /// <summary>
        /// SAves the servers from to the cache file.
        /// </summary>
        private void SaveCachedServers(string filePath)
        {
            filePath = Utils.GetAbsoluteFilePath(filePath, true, false, true);

            using (XmlTextWriter writer = new XmlTextWriter(File.Open(filePath, FileMode.Create, FileAccess.ReadWrite), System.Text.Encoding.UTF8))
            {
                writer.Formatting = Formatting.Indented;

                try
                {
                    RegisteredServerCollection servers = new RegisteredServerCollection();

                    foreach (Registration registration in m_servers.Values)
                    {
                        // only save offline registrations.
                        if (!registration.Server.IsOnline)
                        {
                            servers.Add(registration.Server);
                        }
                    }

                    DataContractSerializer serializer = new DataContractSerializer(typeof(RegisteredServerCollection));
                    serializer.WriteObject(writer, servers);
                }
                finally
                {
                    writer.Close();
                }
            }
        }
        #endregion

        #region Registration Class
        /// <summary>
        /// Stores information about a server.
        /// </summary>
        private class Registration
        {
            public RegisteredServer Server;
            public DateTime LastUpdateTime;
            public Uri[] DiscoveryUrls;
        }
        #endregion

        #region Private Fields
        private object m_lock = new object();
        private Dictionary<string,Registration> m_servers;
        private LocalizedTextCollection m_serverNames;
        private string m_saveFilePath;
        #endregion
    }
}
