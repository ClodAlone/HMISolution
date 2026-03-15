/* ========================================================================
 * Copyright (c) 2005-2011 The OPC Foundation, Inc. All rights reserved.
 *
 * OPC Foundation MIT License 1.00
 * 
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following
 * conditions:
 * 
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 *
 * The complete license agreement can be found here:
 * http://opcfoundation.org/License/MIT/1.00/
 * ======================================================================*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using Opc.Ua.Server;
using Opc.Ua.Gds;

namespace Opc.Ua.GdsServer
{
    public class Gds
    {
        #region Public Interface
        /// <summary>
        /// Constructs the object.
        /// </summary>
        public Gds(IServerInternal server, GdsServerConfiguration configuration)
        {
            m_configuration = configuration;

            // check that the parameters are valid.
            Utils.GetAbsoluteDirectoryPath(m_configuration.CertificateAuthorityStorePath, true, true, true);
            Utils.GetAbsoluteDirectoryPath(m_configuration.DefaultTrustListStorePath, true, true, true);
            Utils.GetAbsoluteDirectoryPath(m_configuration.DefaultIssuerListStorePath, true, true, true);

            // cache the namespace index.
            NamespaceIndex = (ushort)server.NamespaceUris.GetIndex(Opc.Ua.Namespaces.OpcUaGds);
        }

        /// <summary>
        /// The namespace index of the Opc.Ua namespace.
        /// </summary>
        public ushort NamespaceIndex { get; set; }

        /// <summary>
        /// Creates a new certificate authority.
        /// </summary>
        public void LoadCertificateAuthority(string authorityUri, string authorityName)
        {
            lock (m_lock)
            {
                // load the dataset.
                Load();
                                
                GdsDataSet.CertificateAuthoritiesRow authority = m_dataset.CertificateAuthorities.FindByAuthorityUri(authorityUri);

                if (authority == null)
                {
                    // create a new authority.
                    CreateCertificateAuthority(authorityUri, authorityName);

                    // save updates.
                    Save();
                }
            }
        }

        /// <summary>
        /// Loads all of the outstanding certificate requests.
        /// </summary>
        public CertificateRequestState[] LoadCertificateRequests(ServerSystemContext context)
        {
            lock (m_lock)
            {
                // load the dataset.
                Load();

                // select the active requests.
                List<CertificateRequestState> requests = new List<CertificateRequestState>();

                for (int ii = 0; ii < m_dataset.CertificateRequests.Count; ii++)
                {
                    CertificateRequestState request = CreateRequest(context, m_dataset.CertificateRequests[ii]);
                    requests.Add(request);
                }

                // return array.
                return requests.ToArray();
            }
        }

        /// <summary>
        /// Returns id of the specified element.
        /// </summary>
        public NodeState CreateNode(ISystemContext context, ParsedNodeId pnd)
        {
            lock (m_lock)
            {
                // load the dataset.
                Load();

                // find the element.
                GdsDataSet.SystemElementsRow element = FindSystemElement(pnd);

                if (element == null)
                {
                    return null;
                }

                // create the correct object type.
                NodeState node = null;

                switch (element.ElementType)
                {
                    case Opc.Ua.Gds.BrowseNames.AddressableElementType:
                    {
                        node = new Opc.Ua.Gds.AddressableElementState(null);
                        break;
                    }

                    case Opc.Ua.Gds.BrowseNames.ApplicationElementType:
                    {
                        node = new Opc.Ua.Gds.ApplicationElementState(null);
                        break;
                    }

                    default:
                    {
                        node = new Opc.Ua.Gds.SystemElementState(null);
                        break;
                    }
                }

                // build the object.
                ParsedNodeId nodeId = new ParsedNodeId();
                nodeId.NamespaceIndex = NamespaceIndex;
                nodeId.RootType = 0;
                nodeId.RootId = element.ElementId.ToString();

                node.Create(
                    context,
                    pnd.Construct(),
                    new QualifiedName(element.ElementName, NamespaceIndex),
                    null,
                    true);

                node.OnPopulateBrowser = OnPopulateBrowser;

                // fill in custom properties.
                switch (element.ElementType)
                {
                    case Opc.Ua.Gds.BrowseNames.ApplicationElementType:
                    {
                        UpdateApplicationElement(context, element.ApplicationUri, node);
                        break;
                    }
                }

                // find the component node if requested.
                if (!String.IsNullOrEmpty(pnd.ComponentPath))
                {
                    return node.FindChildBySymbolicName(context, pnd.ComponentPath);
                }

                return node;
            }
        }

        /// <summary>
        /// Returns the references from the specified element.
        /// </summary>
        public List<IReference> GetReferences(ISystemContext context, NodeId parentId, BrowseDirection direction)
        {
            List<IReference> references = new List<IReference>();

            lock (m_lock)
            {
                // load the dataset.
                Load();

                // find the parent element.
                GdsDataSet.SystemElementsRow parent = FindSystemElement(parentId);

                if (parent == null)
                {
                    // check if browsing the root.
                    if (parentId != new NodeId(Opc.Ua.Gds.Objects.Directory_Applications, NamespaceIndex))
                    {
                        return null;
                    }
                }

                // get the forward references.
                if (direction != BrowseDirection.Inverse)
                {
                    // search for child elements.
                    DataView view = new DataView(m_dataset.SystemElements);

                    if (parent == null)
                    {
                        view.RowFilter = "ParentElementId IS NULL";
                    }
                    else
                    {
                        view.RowFilter = "ParentElementId = " + parent.ElementId.ToString();
                    }

                    foreach (DataRowView row in view)
                    {
                        GdsDataSet.SystemElementsRow child = row.Row as GdsDataSet.SystemElementsRow;

                        if (child == null)
                        {
                            continue;
                        }

                        // build the object.
                        ParsedNodeId pnd = new ParsedNodeId();
                        pnd.NamespaceIndex = NamespaceIndex;
                        pnd.RootType = 0;
                        pnd.RootId = child.ElementId.ToString();

                        BaseObjectState target = new BaseObjectState(null);
                        target.NodeId = pnd.Construct();
                        target.BrowseName = new QualifiedName(child.ElementName, NamespaceIndex);
                        target.DisplayName = target.BrowseName.Name;
                        target.ReferenceTypeId = Opc.Ua.ReferenceTypeIds.Organizes;
                        target.TypeDefinitionId = GetTypeDefinitionId(child);

                        // add the reference.
                        references.Add(new NodeStateReference(Opc.Ua.ReferenceTypeIds.Organizes, false, target));
                    }
                }

                // get the inverse references.
                if (direction != BrowseDirection.Forward)
                {
                    if (parent != null)
                    {
                        NodeId targetId = null;

                        if (parent.IsParentElementIdNull())
                        {
                            targetId = new NodeId(Opc.Ua.Gds.Objects.Directory_Applications, NamespaceIndex);
                        }
                        else
                        {
                            ParsedNodeId pnd = new ParsedNodeId();
                            pnd.NamespaceIndex = NamespaceIndex;
                            pnd.RootType = 0;
                            pnd.RootId = parent.ParentElementId.ToString();
                            targetId = pnd.Construct();
                        }

                        // add the reference.
                        references.Add(new NodeStateReference(Opc.Ua.ReferenceTypeIds.Organizes, true, targetId));
                    }
                }
            }

            return references;
        }

        /// <summary>
        /// Used to fill in the list of references during browsing.
        /// </summary>
        public void OnPopulateBrowser(
            ISystemContext context,
            NodeState node,
            NodeBrowser browser)
        {
            if (browser.IsRequired(Opc.Ua.ReferenceTypeIds.Organizes, false) || browser.IsRequired(Opc.Ua.ReferenceTypeIds.Organizes, true))
            {
                foreach (IReference target in GetReferences(context, node.NodeId, browser.BrowseDirection))
                {
                    browser.Add(target);
                }
            }
        }

        /// <summary>
        /// Returns the ApplicationDescriptions for the servers that match the criteria.
        /// </summary>
        public ServiceResult QueryServers(
            ISystemContext context,
            NodeId elementId,
            string applicationName,
            string machineName,
            string applicationUri,
            string productUri,
            out ApplicationDescription[] servers)
        {
            servers = null;
            List<ApplicationDescription> descriptions = new List<ApplicationDescription>();

            lock (m_lock)
            {
                // load the dataset.
                Load();

                // find the parent element.
                GdsDataSet.SystemElementsRow element = null;

                if (!NodeId.IsNull(elementId))
                {
                    element = FindSystemElement(elementId);

                    if (element == null)
                    {
                        // check if browsing the root.
                        if (elementId != new NodeId(Opc.Ua.Gds.Objects.Directory_Applications, NamespaceIndex))
                        {
                            return StatusCodes.BadNodeIdUnknown;
                        }
                    }
                }

                GdsDataSet.ApplicationsDataTable table = new GdsDataSet.ApplicationsDataTable();

                // get all applications.
                if (element == null)
                {
                    foreach (GdsDataSet.ApplicationsRow application in m_dataset.Applications.Rows)
                    {
                        table.ImportRow(application);
                    }
                }

                // get the children of the current element.
                else
                {
                    GetApplicationDescriptions(context, element, table);
                }

                // check if anything to do.
                if (table.Rows.Count == 0)
                {
                    return ServiceResult.Good;
                }

                // build the query string.
                StringBuilder query = new StringBuilder();

                if (!String.IsNullOrEmpty(applicationName))
                {
                    query.Append("(ApplicationName LIKE '");
                    query.Append(applicationName);
                    query.Append("')");
                }

                if (!String.IsNullOrEmpty(machineName))
                {
                    if (query.Length > 0)
                    {
                        query.Append(" AND ");
                    }

                    query.Append("(MachineName LIKE '");
                    query.Append(machineName);
                    query.Append("')");
                }

                if (!String.IsNullOrEmpty(applicationUri))
                {
                    if (query.Length > 0)
                    {
                        query.Append(" AND ");
                    }

                    query.Append("(ApplicationUri LIKE '");
                    query.Append(applicationUri);
                    query.Append("')");
                }

                if (!String.IsNullOrEmpty(productUri))
                {
                    if (query.Length > 0)
                    {
                        query.Append(" AND ");
                    }

                    query.Append("(ProductUri LIKE '");
                    query.Append(productUri);
                    query.Append("')");
                }

                DataView view = new DataView(table);
                view.RowFilter = query.ToString();

                // check if anything to do.
                if (view.Count == 0)
                {
                    return StatusCodes.Good;
                }

                // create application decriptions.
                foreach (DataRowView row in view)
                {
                    GdsDataSet.ApplicationsRow application = row.Row as GdsDataSet.ApplicationsRow;

                    if (application == null)
                    {
                        continue;
                    }

                    descriptions.Add(GetApplicationDescription(application));
                }

                // return the servers.
                servers = descriptions.ToArray();

                return ServiceResult.Good;
            }
        }

        /// <summary>
        /// Creates a new application.
        /// </summary>
        public ServiceResult RegisterApplication(
            X509Certificate2 agentCertificate,
            string applicationUri,
            string machineName,
            string applicationName,
            ApplicationType applicationType,
            string productUri,
            string gatewayServerUri,
            string[] discoveryUrls,
            out string revisedApplicationUri)
        {
            revisedApplicationUri = null;

            // validate parameters.
            if (!String.IsNullOrEmpty(applicationUri))
            {
                Uri uri = Utils.ParseUri(applicationUri);

                if (uri == null)
                {
                    return ServiceResult.Create(StatusCodes.BadInvalidArgument, "The ApplicationUri must be a valid URI.");
                }

                if (applicationUri.IndexOf(machineName, 0, StringComparison.InvariantCultureIgnoreCase) < 0)
                {
                    return ServiceResult.Create(StatusCodes.BadInvalidArgument, "The MachineName must be part of the ApplicationUri.");
                }
            }

            if (String.IsNullOrEmpty(applicationName))
            {
                return ServiceResult.Create(StatusCodes.BadInvalidArgument, "The ApplicationName must be specified.");
            }

            if (applicationType < ApplicationType.Server || applicationType > ApplicationType.DiscoveryServer)
            {
                return ServiceResult.Create(StatusCodes.BadInvalidArgument, "The ApplicationType is not valid.");
            }

            if (String.IsNullOrEmpty(machineName))
            {
                return ServiceResult.Create(StatusCodes.BadInvalidArgument, "The MachineName must be specified.");
            }

            if (String.Compare(machineName, "localhost", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                return ServiceResult.Create(StatusCodes.BadInvalidArgument, "Cannot use localhost as the MachineName.");
            }

            if (applicationType == ApplicationType.Server || applicationType == ApplicationType.ClientAndServer)
            {
                if (discoveryUrls == null || discoveryUrls.Length == 0)
                {
                    return ServiceResult.Create(StatusCodes.BadInvalidArgument, "The DiscoveryUrls must be specified.");
                }
            }

            if (!String.IsNullOrEmpty(productUri))
            {
                Uri uri = Utils.ParseUri(productUri);

                if (uri == null)
                {
                    return ServiceResult.Create(StatusCodes.BadInvalidArgument, "The ProductUri must be a valid URI.");
                }

                if (productUri.IndexOf(machineName, 0, StringComparison.InvariantCultureIgnoreCase) >= 0)
                {
                    return ServiceResult.Create(StatusCodes.BadInvalidArgument, "The MachineName must NOT be the part of the ProductUri.");
                }
            }

            if (!String.IsNullOrEmpty(gatewayServerUri))
            {
                Uri uri = Utils.ParseUri(gatewayServerUri);

                if (uri == null)
                {
                    return ServiceResult.Create(StatusCodes.BadInvalidArgument, "The GatewayServerUri must be a valid URI.");
                }
            }

            lock (m_lock)
            {
                // load the dataset.
                Load();

                GdsDataSet.ApplicationsRow row = null;

                // check uri.
                if (String.IsNullOrEmpty(applicationUri))
                {
                    applicationUri = Utils.Format("urn:{0}:{1}", machineName, applicationName);
                }
                else
                {
                    if (applicationUri.Length < 20)
                    {
                        return ServiceResult.Create(StatusCodes.BadInvalidArgument, "The ApplicationUri must be at least 20 characters long.");
                    }
                }

                // look up existing record.
                row = m_dataset.Applications.FindByApplicationUri(applicationUri);

                if (row == null)
                {
                    row = m_dataset.Applications.NewApplicationsRow();
                }

                // update row.
                row.ApplicationUri = applicationUri;
                row.MachineName = machineName.ToUpperInvariant();
                row.ApplicationName = applicationName;
                row.ApplicationType = applicationType.ToString();
                row.ProductUri = productUri;
                row.GatewayServerUri = gatewayServerUri;

                if (discoveryUrls != null)
                {
                    StringBuilder buffer = new StringBuilder();

                    for (int ii = 0; ii < discoveryUrls.Length; ii++)
                    {
                        if (!String.IsNullOrEmpty(discoveryUrls[ii]))
                        {
                            if (buffer.Length > 0)
                            {
                                buffer.Append("\r\n");
                            }

                            buffer.Append(discoveryUrls[ii]);
                        }
                    }

                    row.DiscoveryUrls = buffer.ToString();
                }

                row.SetCertificateIdNull();
                row.AgentCertificateId = agentCertificate.Thumbprint;

                if (row.RowState == DataRowState.Detached)
                {
                    m_dataset.Applications.AddApplicationsRow(row);
                }

                // find the system element for the application.
                DataView view = new DataView(m_dataset.SystemElements);

                StringBuilder filter = new StringBuilder();

                filter = new StringBuilder();
                filter.Append("(ApplicationUri = '");
                filter.Append(applicationUri);
                filter.Append("')");

                view.RowFilter = filter.ToString();

                GdsDataSet.SystemElementsRow applicationElement = null;

                if (view.Count == 0)
                {
                    // find the system element for the machine.
                    GdsDataSet.SystemElementsRow machineElement = null;

                    filter.Length = 0;
                    filter.Append("(ElementName = '");
                    filter.Append(machineName);
                    filter.Append("') AND ");
                    filter.Append("(ElementType = '");
                    filter.Append(Opc.Ua.Gds.BrowseNames.AddressableElementType);
                    filter.Append("')");

                    view.RowFilter = filter.ToString();

                    if (view.Count == 0)
                    {
                        machineElement = m_dataset.SystemElements.NewSystemElementsRow();
                        machineElement.ElementName = machineName;
                        machineElement.ElementType = Opc.Ua.Gds.BrowseNames.AddressableElementType;
                        m_dataset.SystemElements.Rows.Add(machineElement);
                    }
                    else
                    {
                        machineElement = view[0].Row as GdsDataSet.SystemElementsRow;
                    }

                    // add the application element.
                    applicationElement = m_dataset.SystemElements.NewSystemElementsRow();

                    applicationElement.ElementName = applicationName;
                    applicationElement.ParentElementId = machineElement.ElementId;
                    applicationElement.ElementType = Opc.Ua.Gds.BrowseNames.ApplicationElementType;
                    applicationElement.ApplicationUri = applicationUri;

                    m_dataset.SystemElements.Rows.Add(applicationElement);
                }
                else
                {
                    applicationElement = view[0].Row as GdsDataSet.SystemElementsRow;
                }

                // update the application name.
                applicationElement.ElementName = applicationName;

                // save updates.
                Save();
            }

            revisedApplicationUri = applicationUri;
                                  
            return ServiceResult.Good;
        }

        /// <summary>
        /// Requests a new certificate.
        /// </summary>
        public CertificateRequestState RequestCertificate(
            ServerSystemContext context,
            X509Certificate2 agentCertificate,
            string applicationUri,
            string subjectName,
            string[] domainNames,
            string privateKeyFormat,
            string privateKeyPassword,
            bool createHttpsCertificate,
            List<string> cancelledRequests)
        {
            lock (m_lock)
            {
                // load the dataset.
                Load();

                // find the application.
                GdsDataSet.ApplicationsRow application = m_dataset.Applications.FindByApplicationUri(applicationUri);

                if (application == null)
                {
                    throw new ServiceResultException(StatusCodes.BadInvalidArgument, "Application does not exist.");
                }

                // delete existing requests.
                m_dataset.CertificateRequests.DefaultView.RowFilter = Utils.Format("ApplicationUri = '{0}'", applicationUri);

                foreach (DataRowView viewRow in m_dataset.CertificateRequests.DefaultView)
                {
                    GdsDataSet.CertificateRequestsRow row = (GdsDataSet.CertificateRequestsRow)viewRow.Row;
                    cancelledRequests.Add(row.RequestId);

                    if (!row.IsCertificateIdNull())
                    {
                        // delete the certificate.
                        GdsDataSet.CertificatesRow row2 = m_dataset.Certificates.FindByCertificateId(row.CertificateId);

                        if (row2 != null)
                        {
                            DeleteCertificate(row2.CertificateAuthoritiesRow.StoreType, row2.CertificateAuthoritiesRow.StorePath, row.CertificateId);
                            row2.Delete();
                        }
                    }

                    row.Delete();
                }

                // create the new request.
                GdsDataSet.CertificateRequestsRow request = m_dataset.CertificateRequests.NewCertificateRequestsRow();

                request.RequestId = Guid.NewGuid().ToString();
                request.RequestTime = DateTime.UtcNow;
                request.ApplicationRow = application;
                request.SubjectName = subjectName;
                request.DomainNames = null;
                request.IsHttpsCertificate = createHttpsCertificate;
                request.PrivateKeyFormat = privateKeyFormat;
                request.PrivateKeyPassword = privateKeyPassword;
                request.AgentCertificateId = agentCertificate.Thumbprint;

                // check if the CommonName is the MachineName.
                if (createHttpsCertificate && !String.IsNullOrEmpty(subjectName))
                {
                    List<string> names = Utils.ParseDistinguishedName(subjectName);

                    foreach (string name in names)
                    {
                        if (name.StartsWith("CN="))
                        {
                            string commonName = name.Substring(3);

                            if (String.Compare(commonName, application.MachineName, StringComparison.InvariantCultureIgnoreCase) != 0)
                            {
                                throw ServiceResultException.Create(
                                    StatusCodes.BadInvalidArgument, 
                                    "An HTTPS certificate must have the machine name ({0}) as the common name ({1}).",
                                    application.MachineName,
                                    commonName);
                            }
                        }
                    }
                }

                // check that an HTTPS endpoint exists.
                if (request.IsHttpsCertificate)
                {
                    bool httpsUrlFound = false;

                    if (!application.IsDiscoveryUrlsNull() && application.DiscoveryUrls != null)
                    {
                        string[] discoveryUrls = application.DiscoveryUrls.Split(new char[] {'\r','\n'}, StringSplitOptions.RemoveEmptyEntries);

                        foreach (string discoveryUrl in discoveryUrls)
                        {
                            Uri url = Utils.ParseUri(discoveryUrl);

                            if (url != null && url.Scheme == Utils.UriSchemeHttps)
                            {
                                if (String.Compare(url.DnsSafeHost, application.MachineName, StringComparison.InvariantCultureIgnoreCase) == 0)
                                {
                                    httpsUrlFound = true;
                                    break;
                                }
                            }
                        }
                    }

                    if (!httpsUrlFound)
                    {
                        throw new ServiceResultException(StatusCodes.BadInvalidArgument, "Cannot request an HTTPS certificate for an application without an HTTPS endpoint that matches the requested domain.");
                    }
                }

                // build list of domains.
                if (domainNames != null)
                {
                    StringBuilder buffer = new StringBuilder();

                    for (int ii = 0; ii < domainNames.Length; ii++)
                    {
                        if (buffer.Length > 1)
                        {
                            buffer.Append(" ");
                        }

                        buffer.Append(domainNames[ii]);
                    }

                    request.DomainNames = buffer.ToString();
                }

                m_dataset.CertificateRequests.AddCertificateRequestsRow(request);
                m_dataset.AcceptChanges();

                // save updates.
                Save();

                // create the new node.
                return CreateRequest(context, request);
            }
        }

        /// <summary>
        /// Approves a certificate request.
        /// </summary>>
        public void ApproveCertificateRequest(CertificateRequestState request, string authorityUri, bool accept)
        {
            lock (m_lock)
            {
                // load the dataset.
                Load();

                // check for valid node id.
                ParsedNodeId pnd = ParsedNodeId.Parse(request.NodeId);

                if (pnd == null)
                {
                    throw new ServiceResultException(StatusCodes.BadInvalidArgument, "Request does not exist.");
                }

                // find the request.
                GdsDataSet.CertificateRequestsRow row = m_dataset.CertificateRequests.FindByRequestId(pnd.RootId);

                if (row == null)
                {
                    throw new ServiceResultException(StatusCodes.BadInvalidArgument, "Request does not exist.");
                }

                // handle rejection.
                if (!accept)
                {
                    row.Delete();
                    Save();
                    return;
                }

                // select the authority.
                GdsDataSet.CertificateAuthoritiesRow authority = SelectCertificateAuthority(row.IsHttpsCertificate);

                if (!String.IsNullOrEmpty(authorityUri))
                {
                    // find the authority.
                    authority = m_dataset.CertificateAuthorities.FindByAuthorityUri(authorityUri);

                    if (authority == null)
                    {
                        throw new ServiceResultException(StatusCodes.BadInvalidArgument, "Certificate authority does not exist.");
                    }
                }

                // get authority key file.
                CertificateIdentifier id = new CertificateIdentifier();
                
                id.StoreType = authority.StoreType;
                id.StorePath = authority.StorePath;
                id.Thumbprint = authority.Thumbprint;

                string filePath = id.GetPrivateKeyFilePath();

                string[] domainNames = null;

                if (!row.IsDomainNamesNull() && !String.IsNullOrEmpty(row.DomainNames))
                {
                    domainNames = row.DomainNames.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                }

                // issue the certificate.
                X509Certificate2 certificate = CertificateFactory.CreateCertificate(
                    authority.StoreType,
                    authority.StorePath,
                    String.IsNullOrEmpty(row.PrivateKeyPassword)?null:row.PrivateKeyPassword,
                    row.ApplicationRow.ApplicationUri,
                    row.ApplicationRow.ApplicationName,
                    row.SubjectName,
                    domainNames,
                    1024,
                    60,
                    false,
                    row.PrivateKeyFormat == "PEM",
                    filePath,
                    authority.CertificatePassword);

                GdsDataSet.ApplicationsRow application = row.ApplicationRow;

                // create the new certificate.
                GdsDataSet.CertificatesRow row2 = m_dataset.Certificates.NewCertificatesRow();

                row2.CertificateId = certificate.Thumbprint;
                row2.Certificate = certificate.RawData;
                row2.AuthorityUri = authority.AuthorityUri;
                row2.IsRevoked = false;
                m_dataset.Certificates.AddCertificatesRow(row2);

                row.CertificateId = certificate.Thumbprint;
                m_dataset.AcceptChanges();

                // save updates.
                Save();
            }
        }

        /// <summary>
        /// Check if the certificate is an authorized certificates for the request.
        /// </summary>>
        public bool IsAuthorizedCertificate(NodeId requestId, X509Certificate2 certificate)
        {
            lock (m_lock)
            {
                // load the dataset.
                Load();

                // check for valid node id.
                ParsedNodeId pnd = ParsedNodeId.Parse(requestId);

                if (pnd == null)
                {
                    return true;
                }

                // find the request.
                GdsDataSet.CertificateRequestsRow row = m_dataset.CertificateRequests.FindByRequestId(pnd.RootId);

                if (row == null)
                {
                    return true;
                }

                // check the agent that requested the certificate.
                if (!row.IsAgentCertificateIdNull())
                {
                    if (certificate != null && certificate.Thumbprint == row.AgentCertificateId)
                    {
                        return true;
                    }
                }

                // check the existing certificate id.
                GdsDataSet.ApplicationsRow application = row.ApplicationRow;

                if (application != null)
                {
                    if (!application.IsCertificateIdNull())
                    {
                        if (certificate != null && certificate.Thumbprint == application.CertificateId)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Check if the certificate is an authorized certificates for the application.
        /// </summary>>
        public bool IsAuthorizedCertificate(string applicationUri, X509Certificate2 certificate)
        {
            lock (m_lock)
            {
                // load the dataset.
                Load();

                // find the request.
                GdsDataSet.ApplicationsRow row = m_dataset.Applications.FindByApplicationUri(applicationUri);

                if (row != null)
                {
                    // check the agent for the application.
                    if (!row.IsAgentCertificateIdNull())
                    {
                        if (certificate != null && certificate.Thumbprint == row.AgentCertificateId)
                        {
                            return true;
                        }
                    }

                    // check the existing certificate id.
                    if (!row.IsCertificateIdNull())
                    {
                        if (certificate != null && certificate.Thumbprint == row.CertificateId)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Revokes an existing certificate.
        /// </summary>
        public void RevokeCertificate(string thumbprint)
        {
            // find the certificate.
            GdsDataSet.CertificatesRow row = m_dataset.Certificates.FindByCertificateId(thumbprint);

            if (row == null)
            {
                return;
            }

            row.IsRevoked = true;

            if (row.CertificateAuthoritiesRow == null || row.IsCertificateNull())
            {
                return;
            }

            // add the certificate to the revocation list.
            CertificateStoreIdentifier id = new CertificateStoreIdentifier();
            id.StoreType = row.CertificateAuthoritiesRow.StoreType;
            id.StorePath = row.CertificateAuthoritiesRow.StorePath;

            ICertificateStore store = id.OpenStore();
            string privateKeyPath = store.GetPrivateKeyFilePath(row.CertificateAuthoritiesRow.Thumbprint);

            CertificateFactory.RevokeCertificate(
                id.StorePath,
                new X509Certificate2(row.Certificate),
                privateKeyPath,
                row.CertificateAuthoritiesRow.CertificatePassword);
        }
        
        /// <summary>
        /// Gets the trust lists for the application.
        /// </summary>>
        public void GetTrustList(
            X509Certificate2 agentCertificate,
            string applicationUri,
            bool returnHttpsLists,
            out byte[][] trustedCertificates,
            out byte[][] trustedCertificateRevocationLists,
            out byte[][] issuerCertificates,
            out byte[][] issuerCertificateRevocationLists)
        {
            trustedCertificates = null;
            trustedCertificateRevocationLists = null;
            issuerCertificates = null;
            issuerCertificateRevocationLists = null;
            
            lock (m_lock)
            {
                // load the dataset.
                Load();

                // find the application.
                GdsDataSet.ApplicationsRow application = m_dataset.Applications.FindByApplicationUri(applicationUri);

                if (application == null)
                {
                    throw new ServiceResultException(StatusCodes.BadInvalidArgument, "Application does not exist.");
                }

                // check if HTTPS trust lists are being requested.
                if (returnHttpsLists)
                {
                    trustedCertificates = new byte[0][];
                    trustedCertificateRevocationLists = new byte[0][];
                    issuerCertificates = new byte[0][];
                    issuerCertificateRevocationLists = new byte[0][];

                    GdsDataSet.CertificateAuthoritiesRow authority = SelectCertificateAuthority(true);

                    if (authority != null)
                    {
                        X509Certificate2 certificate = GetCertificate(authority.StoreType, authority.StorePath, authority.Thumbprint);

                        if (certificate != null)
                        {
                            trustedCertificates = new byte[][] { certificate.RawData };
                            trustedCertificateRevocationLists = AddCertificateAuthorityCRLs(trustedCertificates, trustedCertificateRevocationLists);
                        }
                    }

                    return;                   
                }

                // find the trust list.
                string trustListStorePath = m_configuration.DefaultTrustListStorePath;
                
                if (!application.IsTrustListStorePathNull())
                {
                    trustListStorePath = application.TrustListStorePath;
                }

                trustedCertificates = GetCertificatesInStore(CertificateStoreType.Directory, trustListStorePath);
                trustedCertificateRevocationLists = GetCRLsInStore(CertificateStoreType.Directory, trustListStorePath);
                trustedCertificateRevocationLists = AddCertificateAuthorityCRLs(trustedCertificates, trustedCertificateRevocationLists);

                // find the issuer list.
                string issuerListStorePath = m_configuration.DefaultIssuerListStorePath;

                if (!application.IsIssuerListStorePathNull())
                {
                    issuerListStorePath = application.IssuerListStorePath;
                }

                issuerCertificates = GetCertificatesInStore(CertificateStoreType.Directory, issuerListStorePath);
                issuerCertificateRevocationLists = GetCRLsInStore(CertificateStoreType.Directory, issuerListStorePath);
                issuerCertificateRevocationLists = AddCertificateAuthorityCRLs(issuerCertificates, issuerCertificateRevocationLists);
            }
        }

        /// <summary>
        /// Adds the CRLs for the authorities to the list.
        /// </summary>
        private byte[][] AddCertificateAuthorityCRLs(byte[][] certificates, byte[][] crls)
        {
            List<byte[]> authorityCrls = new List<byte[]>(crls);

            DataView view = new DataView(m_dataset.CertificateAuthorities);

            foreach (byte[] bytes in certificates)
            {
                X509Certificate2 certificate = new X509Certificate2(bytes);
                view.RowFilter = "Thumbprint = '" + certificate.Thumbprint + "'";

                if (view.Count > 0)
                {
                    GdsDataSet.CertificateAuthoritiesRow authority = view[0].Row as GdsDataSet.CertificateAuthoritiesRow;

                    if (authority != null)
                    {
                        GetCRLsForIssuer(certificate, authority.StoreType, authority.StorePath, authorityCrls);
                    }
                }
            }

            return authorityCrls.ToArray();
        }
        
        /// <summary>
        /// Gets the result of a certificate request.
        /// </summary>>
        public void CheckRequestStatus(
            NodeId nodeId,
            X509Certificate2 agentCertificate,
            out byte[] certificate,
            out byte[] privateKey,
            out byte[][] issuerCertificates)
        {
            certificate = null;
            privateKey = null;
            issuerCertificates = null;

            lock (m_lock)
            {
                // load the dataset.
                Load();

                // check for valid node id.
                ParsedNodeId pnd = ParsedNodeId.Parse(nodeId);

                if (pnd == null)
                {
                    throw new ServiceResultException(StatusCodes.BadNoEntryExists, "Request does not exist.");
                }
                
                // find the request.
                GdsDataSet.CertificateRequestsRow row = m_dataset.CertificateRequests.FindByRequestId(pnd.RootId);

                if (row == null)
                {
                    throw new ServiceResultException(StatusCodes.BadNoEntryExists, "Request does not exist.");
                }

                if (row.IsCertificateIdNull())
                {
                    throw new ServiceResultException(StatusCodes.BadNothingToDo, "Request is not complete.");
                }

                // find the certificate.
                GdsDataSet.CertificatesRow row2 = m_dataset.Certificates.FindByCertificateId(row.CertificateId);

                if (row2 == null)
                {
                    throw new ServiceResultException(StatusCodes.BadInvalidArgument, "Certificate does not exist.");
                }

                // update the application record.
                GdsDataSet.ApplicationsRow application = row.ApplicationRow;

                if (application != null)
                {
                    application.AgentCertificateId = agentCertificate.Thumbprint;
                }

                string storeType = row2.CertificateAuthoritiesRow.StoreType;
                string storePath = row2.CertificateAuthoritiesRow.StorePath;

                // get the certificate.
                X509Certificate2 target = GetCertificate(storeType, storePath, row.CertificateId);

                if (target == null)
                {
                    throw new ServiceResultException(StatusCodes.BadInternalError, "Cannot load certificate.");
                }

                certificate = target.RawData;

                // get the private key.
                string privateKeyPath = GetPrivateKeyPath(storeType, storePath, row.CertificateId);
                
                if (privateKeyPath == null)
                {
                    throw new ServiceResultException(StatusCodes.BadInternalError, "Cannot find private key.");
                }

                privateKey = System.IO.File.ReadAllBytes(privateKeyPath);

                // build list of issuers.
                List<byte[]> issuers = new List<byte[]>();

                while (!Utils.CompareDistinguishedName(target.Subject, target.Issuer))
                {
                    X509Certificate2 issuer = GetCertificateByIssuer(storeType, storePath, target);

                    if (issuer == null)
                    {
                        throw new ServiceResultException(StatusCodes.BadInternalError, "Cannot find issuer certificate.");
                    }

                    AddToApplicationIssuerStore(application, issuer);
                    issuers.Add(issuer.RawData);
                    target = issuer;
                }

                issuerCertificates = issuers.ToArray();
                
                // delete the certificate from the store.
                DeleteCertificate(storeType, storePath, row.CertificateId);

                // delete the request.
                bool isHttpsCertificate = row.IsHttpsCertificate;
                row.Delete();

                // revoke the existing certificate.
                if (isHttpsCertificate)
                {
                    if (!application.IsCertificateIdNull())
                    {
                        RevokeCertificate(application.CertificateId);
                    }

                    application.CertificateId = row2.CertificateId;
                }
                else
                {
                    if (!application.IsHttpsCertificateIdNull())
                    {
                        RevokeCertificate(application.HttpsCertificateId);
                    }

                    application.HttpsCertificateId = row2.CertificateId;
                }

                // save updates.
                Save();
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Finds the system element with the specified node id.
        /// </summary>
        private GdsDataSet.SystemElementsRow FindSystemElement(NodeId nodeId)
        {
            ParsedNodeId pnd = ParsedNodeId.Parse(nodeId);

            if (pnd == null)
            {
                return null;
            }

            return FindSystemElement(pnd);
        }

        /// <summary>
        /// Finds the system element with the specified parsed node id.
        /// </summary>
        private GdsDataSet.SystemElementsRow FindSystemElement(ParsedNodeId pnd)
        {
            int id = -1;

            try
            {
                id = Convert.ToInt32(pnd.RootId);
            }
            catch (Exception)
            {
                return null;
            }

            return m_dataset.SystemElements.FindByElementId(id);
        }

        /// <summary>
        /// Updates the application element with additional metadata.
        /// </summary>
        private void UpdateApplicationElement(ISystemContext context, string applicationUri, NodeState node)
        {
            // find the application.
            GdsDataSet.ApplicationsRow application = m_dataset.Applications.FindByApplicationUri(applicationUri);

            if (application == null)
            {
                return;
            }

            // update properties.
            Opc.Ua.Gds.ApplicationElementState anode = node as Opc.Ua.Gds.ApplicationElementState;

            anode.ApplicationUri.Value = application.ApplicationUri;
            anode.ApplicationType.Value = application.ApplicationType;
            anode.MachineName.Value = application.MachineName;

            if (application.DiscoveryUrls != null)
            {
                anode.DiscoveryUrls.Value = application.DiscoveryUrls.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        /// <summary>
        /// Returns the type definition for the element type.
        /// </summary>
        private NodeId GetTypeDefinitionId(GdsDataSet.SystemElementsRow element)
        {
            switch (element.ElementType)
            {
                case Opc.Ua.Gds.BrowseNames.AddressableElementType:
                {
                    return new NodeId(Opc.Ua.Gds.ObjectTypes.AddressableElementType, NamespaceIndex);
                }

                case Opc.Ua.Gds.BrowseNames.ApplicationElementType:
                {
                    return new NodeId(Opc.Ua.Gds.ObjectTypes.ApplicationElementType, NamespaceIndex);
                }

                default:
                {
                    return new NodeId(Opc.Ua.Gds.ObjectTypes.SystemElementType, NamespaceIndex);
                }
            }
        }

        /// <summary>
        /// Returns the application type.
        /// </summary>
        private ApplicationType GetApplicationType(GdsDataSet.ApplicationsRow application)
        {
            if (application != null && !application.IsApplicationTypeNull() && application.ApplicationType != null)
            {
                switch (application.ApplicationType)
                {
                    case "Server":
                    {
                        return ApplicationType.Server;
                    }

                    case "Client":
                    {
                        return ApplicationType.Client;
                    }

                    case "ClientAndServer":
                    {
                        return ApplicationType.ClientAndServer;
                    }

                    case "DiscoveryServer":
                    {
                        return ApplicationType.DiscoveryServer;
                    }
                }
            }

            return ApplicationType.Server;
        }

        /// <summary>
        /// Returns the application description.
        /// </summary>
        private ApplicationDescription GetApplicationDescription(GdsDataSet.ApplicationsRow application)
        {
            ApplicationDescription description = new ApplicationDescription();

            description.ApplicationUri = application.ApplicationUri;

            if (!application.IsApplicationTypeNull())
            {
                description.ApplicationType = GetApplicationType(application);
            }

            if (!application.IsApplicationNameNull())
            {
                description.ApplicationName = application.ApplicationName;
            }

            if (!application.IsProductUriNull())
            {
                description.ProductUri = application.ProductUri;
            }

            if (!application.IsGatewayServerUriNull())
            {
                description.GatewayServerUri = application.GatewayServerUri;
            }

            description.DiscoveryProfileUri = null;

            if (!application.IsDiscoveryUrlsNull() && application.DiscoveryUrls != null)
            {
                description.DiscoveryUrls = application.DiscoveryUrls.Split(new char[] {'\r','\n'}, StringSplitOptions.RemoveEmptyEntries);
            }

            return description;
        }

        /// <summary>
        /// Returns all applications which are children of the specified ndoe.
        /// </summary>
        private void GetApplicationDescriptions(
            ISystemContext context,
            GdsDataSet.SystemElementsRow element,
            GdsDataSet.ApplicationsDataTable table)
        {
            if (!element.IsApplicationUriNull() && !String.IsNullOrEmpty(element.ApplicationUri))
            {
                // find the application.
                GdsDataSet.ApplicationsRow application = m_dataset.Applications.FindByApplicationUri(element.ApplicationUri);

                if (application != null)
                {
                    table.ImportRow(application);
                }
            }

            // search for child elements.
            DataView view = new DataView(m_dataset.SystemElements);
            view.RowFilter = "ParentElementId = " + element.ElementId.ToString();

            foreach (DataRowView row in view)
            {
                GdsDataSet.SystemElementsRow child = row.Row as GdsDataSet.SystemElementsRow;

                if (child == null)
                {
                    continue;
                }

                GetApplicationDescriptions(context, child, table);
            }
        }

        /// <summary>
        /// Creates a new certificate authority.
        /// </summary>
        private GdsDataSet.CertificateAuthoritiesRow CreateCertificateAuthority(string authorityUri, string authorityName)
        {
            lock (m_lock)
            {
                string privateKeyPath = null;
                string privateKeyPassword = null;

                // get the master authority.
                if (m_configuration.UseSingleRootCertificateAuthority)
                {
                    GdsDataSet.CertificateAuthoritiesRow master = m_dataset.CertificateAuthorities.FindByAuthorityUri("http://opcfoundation.org/UA/GDS");

                    if (master == null)
                    {
                        master = m_dataset.CertificateAuthorities.NewCertificateAuthoritiesRow();

                        master.AuthorityUri = "http://opcfoundation.org/UA/GDS";
                        master.AuthorityName = "OPC UA GDS Certificate Authority";
                        master.StoreType = CertificateStoreType.Directory;
                        master.StorePath = m_configuration.CertificateAuthorityStorePath;
                        master.CertificatePassword = Guid.NewGuid().ToString();

                        X509Certificate2 masterCertificate = CertificateFactory.CreateCertificate(
                            master.StoreType,
                            master.StorePath,
                            master.CertificatePassword,
                            master.AuthorityUri,
                            master.AuthorityName,
                            null,
                            null,
                            2048,
                            60,
                            true,
                            null,
                            null);

                        master.Thumbprint = masterCertificate.Thumbprint;
                        m_dataset.AcceptChanges();

                        AddCertificate(CertificateStoreType.Directory, m_configuration.DefaultIssuerListStorePath, new X509Certificate2(masterCertificate.RawData));
                    }

                    // get the master authority certificate file.
                    privateKeyPath = GetPrivateKeyPath(master.StoreType, master.StorePath, master.Thumbprint);
                    privateKeyPassword = master.CertificatePassword;
                }

                // create the new authority.
                GdsDataSet.CertificateAuthoritiesRow row = m_dataset.CertificateAuthorities.NewCertificateAuthoritiesRow();

                row.AuthorityUri = authorityUri;
                row.AuthorityName = authorityName;
                row.StoreType = CertificateStoreType.Directory;
                row.StorePath = m_configuration.CertificateAuthorityStorePath;
                row.CertificatePassword = Guid.NewGuid().ToString();

                m_dataset.CertificateAuthorities.AddCertificateAuthoritiesRow(row);
                m_dataset.AcceptChanges();

                X509Certificate2 certificate = CertificateFactory.CreateCertificate(
                    row.StoreType,
                    row.StorePath,
                    row.CertificatePassword,
                    authorityUri,
                    authorityName,
                    null,
                    null,
                    2048,
                    60,
                    true,
                    privateKeyPath,
                    privateKeyPassword);

                row.Thumbprint = certificate.Thumbprint;
                m_dataset.AcceptChanges();

                privateKeyPassword = row.CertificatePassword;
                privateKeyPath = GetPrivateKeyPath(row.StoreType, row.StorePath, certificate.Thumbprint);

                try
                {
                    // issue a certificate to revoke.
                    X509Certificate2 certificateToRevoke = CertificateFactory.CreateCertificate(
                        row.StoreType,
                        row.StorePath,
                        null,
                        "uri:placeholder.com",
                        "revoked certificate",
                        null,
                        null,
                        1024,
                        1,
                        false,
                        false,
                        privateKeyPath,
                        privateKeyPassword);

                    // revoke it (ensures a CRL exists for the authority).
                    CertificateFactory.RevokeCertificate(
                        row.StorePath,
                        certificateToRevoke,
                        privateKeyPath,
                        privateKeyPassword);

                    // delete the certificate.
                    DeleteCertificate(row.StoreType, row.StorePath, certificateToRevoke.Thumbprint);
                }
                catch (Exception e)
                {
                    Utils.Trace((int)Utils.TraceMasks.Error, "Could not initialize the CRL for the new authority. {0}", e.Message);
                }

                return row;
            }
        }

        /// <summary>
        /// Selects the certificate authority to use.
        /// </summary>
        private GdsDataSet.CertificateAuthoritiesRow SelectCertificateAuthority(bool isHttpsCertificate)
        {
            foreach (GdsDataSet.CertificateAuthoritiesRow authority in m_dataset.CertificateAuthorities)
            {
                if (isHttpsCertificate)
                {
                    if (!authority.AuthorityUri.Contains("HTTPS"))
                    {
                        continue;
                    }

                    return authority;
                }

                else
                {
                    if (authority.AuthorityUri.Contains("HTTPS"))
                    {
                        continue;
                    }

                    return authority;
                }
            }

            throw ServiceResultException.Create(StatusCodes.BadConfigurationError, "No certificate authorities have been defined.");
        }

        /// <summary>
        /// Adds the CRLs for the issuer to the list.
        /// </summary>
        private void GetCRLsForIssuer(X509Certificate2 issuer, string storeType, string storePath, List<byte[]> crls)
        {
            CertificateStoreIdentifier id = new CertificateStoreIdentifier();
            id.StoreType = storeType;
            id.StorePath = storePath;

            ICertificateStore store = id.OpenStore();

            try
            {
                List<X509CRL> issuerCrls = store.EnumerateCRLs(issuer);

                foreach (X509CRL issuerCrl in issuerCrls)
                {
                    crls.Add(issuerCrl.RawData);
                }
            }
            finally
            {
                store.Close();
            }
        }

        /// <summary>
        /// Finds the certificate matching the thumbprint.
        /// </summary>
        private X509Certificate2 GetCertificate(string storeType, string storePath, string thumprint)
        {
            CertificateStoreIdentifier id = new CertificateStoreIdentifier();
            id.StoreType = storeType;
            id.StorePath = storePath;

            ICertificateStore store = id.OpenStore();

            try
            {
                return store.FindByThumbprint(thumprint);
            }
            finally
            {
                store.Close();
            }
        }

        /// <summary>
        /// Deletes the certificate matching the thumbprint.
        /// </summary>
        private void DeleteCertificate(string storeType, string storePath, string thumprint)
        {
            CertificateStoreIdentifier id = new CertificateStoreIdentifier();
            id.StoreType = storeType;
            id.StorePath = storePath;

            ICertificateStore store = id.OpenStore();

            try
            {
                store.Delete(thumprint);
            }
            finally
            {
                store.Close();
            }
        }

        /// <summary>
        /// Finds the certificate matching the issuer.
        /// </summary>
        private X509Certificate2 GetCertificateByIssuer(string storeType, string storePath, X509Certificate2 target)
        {
            X509AuthorityKeyIdentifierExtension keyId = FindAuthorityKeyIdentifier(target);

            CertificateStoreIdentifier id = new CertificateStoreIdentifier();
            id.StoreType = storeType;
            id.StorePath = storePath;

            ICertificateStore store = id.OpenStore();

            try
            {
                foreach (X509Certificate2 certificate in store.Enumerate())
                {
                    if (MatchKeyIdentifier(certificate, target.Issuer, keyId))
                    {
                        return certificate;
                    }
                }
            }
            finally
            {
                store.Close();
            }

            return null;
        }

        /// <summary>
        /// Adds an application to application issuer store.
        /// </summary>
        private void AddToApplicationIssuerStore(GdsDataSet.ApplicationsRow application, X509Certificate2 issuer)
        {
            CertificateStoreIdentifier id = new CertificateStoreIdentifier();
            id.StoreType = CertificateStoreType.Directory;
            // id.StorePath = (application.IsIssuerListStorePathNull()) ? m_configuration.DefaultIssuerListStorePath : application.IssuerListStorePath;
            id.StorePath = (application.IsTrustListStorePathNull()) ? m_configuration.DefaultTrustListStorePath : application.TrustListStorePath;

            ICertificateStore store = id.OpenStore();

            try
            {
                if (store.FindByThumbprint(issuer.Thumbprint) == null)
                {
                    store.Add(issuer);
                }
            }
            finally
            {
                store.Close();
            }
        }

        /// <summary>
        /// Returns all certificates in a store.
        /// </summary>
        private byte[][] GetCertificatesInStore(string storeType, string storePath)
        {
            if (storeType != CertificateStoreType.Directory)
            {
                throw new ServiceResultException(StatusCodes.BadNotSupported);
            }

            List<byte[]> certificates = new List<byte[]>();

            storePath += "//certs";
            storePath = Utils.GetAbsoluteDirectoryPath(storePath, false, false, false);
            
            if (storePath != null)
            {
                foreach (string filePath in Directory.GetFiles(storePath, "*.der"))
                {
                    byte[] bytes = File.ReadAllBytes(filePath);
                    certificates.Add(bytes);
                }
            }

            return certificates.ToArray();
        }

        /// <summary>
        /// Returns all CRLs in a store.
        /// </summary>
        private byte[][] GetCRLsInStore(string storeType, string storePath)
        {
            if (storeType != CertificateStoreType.Directory)
            {
                throw new ServiceResultException(StatusCodes.BadNotSupported);
            }

            List<byte[]> certificates = new List<byte[]>();

            storePath += "//crl";
            storePath = Utils.GetAbsoluteDirectoryPath(storePath, false, false, false);

            if (storePath != null)
            {
                foreach (string filePath in Directory.GetFiles(storePath, "*.crl"))
                {
                    byte[] bytes = File.ReadAllBytes(filePath);
                    certificates.Add(bytes);
                }
            }

            return certificates.ToArray();
        }

        /// <summary>
        /// Returns true if the certificate matches the key identifier.
        /// </summary>
        private bool MatchKeyIdentifier(
            X509Certificate2 target,
            string subjectName,
            X509AuthorityKeyIdentifierExtension keyId)
        {
            // check for null.
            if (target == null)
            {
                return false;
            }

            // check for subject name match.
            if (!Utils.CompareDistinguishedName(target.SubjectName.Name, subjectName))
            {
                return false;
            }

            if (keyId != null)
            {
                // check for serial number match.
                if (!String.IsNullOrEmpty(keyId.SerialNumber))
                {
                    if (target.SerialNumber != keyId.SerialNumber)
                    {
                        return false;
                    }
                }
            }

            // found match.
            return true;
        }
        
        /// <summary>
        /// Returns the authority key identifier in the certificate.
        /// </summary>
        private X509AuthorityKeyIdentifierExtension FindAuthorityKeyIdentifier(X509Certificate2 certificate)
        {
            for (int ii = 0; ii < certificate.Extensions.Count; ii++)
            {
                X509Extension extension = certificate.Extensions[ii];

                switch (extension.Oid.Value)
                {
                    case X509AuthorityKeyIdentifierExtension.AuthorityKeyIdentifierOid:
                    case X509AuthorityKeyIdentifierExtension.AuthorityKeyIdentifier2Oid:
                    {
                        return new X509AuthorityKeyIdentifierExtension(extension, extension.Critical);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Adds the certificate to the store.
        /// </summary>
        private void AddCertificate(string storeType, string storePath, X509Certificate2 certificate)
        {
            CertificateStoreIdentifier id = new CertificateStoreIdentifier();
            id.StoreType = storeType;
            id.StorePath = storePath;

            ICertificateStore store = id.OpenStore();

            try
            {
                if (store.FindByThumbprint(certificate.Thumbprint) == null)
                {
                    store.Add(certificate);
                }
            }
            finally
            {
                store.Close();
            }
        }

        /// <summary>
        /// Gets the path to the certificate private key.
        /// </summary>
        private string GetPrivateKeyPath(string storeType, string storePath, string thumbprint)
        {
            CertificateStoreIdentifier id = new CertificateStoreIdentifier();
            id.StoreType = storeType;
            id.StorePath = storePath;

            ICertificateStore store = id.OpenStore();

            try
            {
                return store.GetPrivateKeyFilePath(thumbprint);
            }
            finally
            {
                store.Close();
            }
        }

        /// <summary>
        /// Creates a request from an application record.
        /// </summary>
        private CertificateRequestState CreateRequest(ServerSystemContext context, GdsDataSet.CertificateRequestsRow request)
        {
            ushort namespaceIndex = (ushort)context.NamespaceUris.GetIndex(Opc.Ua.Namespaces.OpcUaGds);

            string applicationName = request.RequestId;

            if (request.ApplicationRow != null)
            {
                applicationName = request.ApplicationRow.ApplicationName;
            }

            CertificateRequestState node = new CertificateRequestState(null);

            node.Create(
                context,
                ParsedNodeId.Construct(0, request.RequestId, namespaceIndex),
                new QualifiedName(applicationName, namespaceIndex),
                null,
                true);

            node.SetEnableState(context, true);
            node.Activate(context);

            if (request.ApplicationRow != null)
            {
                node.ConditionName.Value = request.ApplicationRow.ApplicationName;
                node.ApplicationUri.Value = request.ApplicationRow.ApplicationUri;
                node.ApplicationType.Value = request.ApplicationRow.ApplicationType;
            }

            node.SubjectName.Value = request.SubjectName;
            node.IsHttpsCertificate.Value = request.IsHttpsCertificate;

            if (node.DomainNames != null)
            {
                node.DomainNames.Value = request.DomainNames.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            }

            node.DefaultResponse.Value = 0;
            node.OkResponse.Value = 0;
            node.CancelResponse.Value = 1;
            node.ResponseOptionSet.Value = new LocalizedText[] { "Accept", "Reject" };
            node.Prompt.Value = "Accept request for new certificate?";

            node.EventId.Value = Guid.NewGuid().ToByteArray();
            node.EventType.Value = new NodeId(Opc.Ua.Gds.ObjectTypes.CertificateRequestType, namespaceIndex);
            node.SourceName.Value = Opc.Ua.Gds.BrowseNames.CertificateRequests;
            node.SourceNode.Value = new NodeId(Opc.Ua.Gds.Objects.RootDirectoryEntryType_CertificateRequests, namespaceIndex);
            node.Severity.Value = 100;
            node.Time.Value = request.RequestTime;
            node.ReceiveTime.Value = request.RequestTime;
            node.Message.Value = node.Prompt.Value;
            node.ConditionClassId.Value = Opc.Ua.ObjectTypeIds.BaseConditionClassType;
            node.ConditionClassName.Value = Opc.Ua.BrowseNames.BaseConditionClassType;
            node.Retain.Value = true;

            return node;
        }
 
        /// <summary>
        /// Loads the file from disk.
        /// </summary>
        private void Load()
        {
            GdsDataSet dataset = new GdsDataSet();

            string filePath = Utils.GetAbsoluteFilePath(m_configuration.DatabasePath, true, false, false);

            if (filePath != null)
            {
                FileInfo info = new FileInfo(filePath);

                if (info.LastWriteTimeUtc <= m_lastModifiedTime)
                {
                    return;
                }

                dataset.ReadXml(info.FullName);
                m_lastModifiedTime = info.LastWriteTimeUtc;
            }

            m_dataset = dataset;
        }

        /// <summary>
        /// Saves the file to disk.
        /// </summary>
        private void Save()
        {
            m_dataset.AcceptChanges();
            string filePath = Utils.GetAbsoluteFilePath(m_configuration.DatabasePath, true, false, true);
            m_dataset.WriteXml(filePath);
            FileInfo info = new FileInfo(m_configuration.DatabasePath);
            m_lastModifiedTime = info.LastWriteTimeUtc;
        }
        #endregion

        #region Private Fields
        private object m_lock = new object();
        GdsServerConfiguration m_configuration;
        private GdsDataSet m_dataset;
        private DateTime m_lastModifiedTime;
        #endregion
    }
}
