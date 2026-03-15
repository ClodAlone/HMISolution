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
using System.Diagnostics;
using System.Xml;
using System.IO;
using System.Threading;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.IdentityModel.Tokens;
using Opc.Ua;
using Opc.Ua.Server;
using Opc.Ua.Gds;

namespace Opc.Ua.GdsServer
{
    /// <summary>
    /// A node manager for a server that exposes several variables.
    /// </summary>
    public class GdsNodeManager : CustomNodeManager2
    {
        #region Constructors
        /// <summary>
        /// Initializes the node manager.
        /// </summary>
        public GdsNodeManager(IServerInternal server, ApplicationConfiguration configuration)
        :
            base(server, configuration)
        {
            SystemContext.NodeIdFactory = this;

            // set one namespace for the type model and one names for dynamically created nodes.
            string[] namespaceUrls = new string[1];
            namespaceUrls[0] = Opc.Ua.Namespaces.OpcUaGds;
            SetNamespaces(namespaceUrls);

            // get the configuration for the node manager.
            m_configuration = configuration.ParseExtension<GdsServerConfiguration>();

            // use suitable defaults if no configuration exists.
            if (m_configuration == null)
            {
                m_configuration = new GdsServerConfiguration();
            }

            m_userRoleManager = new UserRoleManager(configuration.SecurityConfiguration.UserRoleDirectory);

            try
            {
                // create the system.
                SystemContext.SystemHandle = m_system = new Gds(server, m_configuration);
                m_system.LoadCertificateAuthority("http://opcfoundation.org/Authority/IOP", "OPC IOP Certificate Authority");
                m_system.LoadCertificateAuthority("http://opcfoundation.org/Authority/HTTPS", "OPC HTTPS Certificate Authority");
            }
            catch (Exception e)
            {
                Utils.Trace(e, "Could not initialize the GDS database.");
            }

            Utils.Trace((int)Utils.TraceMasks.Error, "Initialized the GDS Node Manager.");
        }
        #endregion
        
        #region IDisposable Members
        /// <summary>
        /// An overrideable version of the Dispose.
        /// </summary>
        protected override void Dispose(bool disposing)
        {  
            if (disposing)
            {
                // TBD
            }
        }
        #endregion

        #region INodeIdFactory Members
        /// <summary>
        /// Creates the NodeId for the specified node.
        /// </summary>
        public override NodeId New(ISystemContext context, NodeState node)
        {
            BaseInstanceState instance = node as BaseInstanceState;

            if (instance != null && instance.Parent != null)
            {
                ParsedNodeId pnd = ParsedNodeId.Parse(instance.Parent.NodeId);
                return pnd.Construct(instance.SymbolicName);
            }

            return node.NodeId;
        }
        #endregion

        #region Overridden Methods
        /// <summary>
        /// Loads a node set from a file or resource and addes them to the set of predefined nodes.
        /// </summary>
        protected override NodeStateCollection LoadPredefinedNodes(ISystemContext context)
        {
            NodeStateCollection predefinedNodes = new NodeStateCollection();
            predefinedNodes.LoadFromBinaryResource(context, "Opc.Ua.GdsServer.Model.Opc.Ua.Gds.PredefinedNodes.uanodes", null, true);
            return predefinedNodes;
        }
        #endregion

        #region INodeManager Members
        /// <summary>
        /// Does any initialization required before the address space can be used.
        /// </summary>
        public override void CreateAddressSpace(IDictionary<NodeId, IList<IReference>> externalReferences)
        {
            lock (Lock)
            {
                base.CreateAddressSpace(externalReferences);

                BaseObjectState passiveNode = (BaseObjectState)FindPredefinedNode(ExpandedNodeId.ToNodeId(Opc.Ua.Gds.ObjectIds.Directory, Server.NamespaceUris), typeof(BaseObjectState));
                
                // convert root node to a fully typed node.
                RootDirectoryEntryState root = new RootDirectoryEntryState(null);
                root.Create(SystemContext, passiveNode);
                AddPredefinedNode(SystemContext, root);

                root.RegisterApplication.OnCall = OnRegisterApplication;
                root.RequestCertificate.OnCall = OnRequestCertificate;
                root.RenewCertificate.OnCall = OnRenewCertificate;
                root.CheckRequestStatus.OnCall = OnCheckRequestStatus;
                root.GetTrustList.OnCall = OnGetTrustList;
                root.RevokeCertificate.OnCall = OnRevokeCertificate;
                root.QueryServers.OnCall = OnQueryServers;

                root.Applications.OnPopulateBrowser = m_system.OnPopulateBrowser;

                // load the existing requests.
                foreach (CertificateRequestState request in m_system.LoadCertificateRequests(SystemContext))
                {
                    request.OnRespond = OnRespond;

                    BaseObjectState folder = (BaseObjectState)FindPredefinedNode(new NodeId(Opc.Ua.Gds.Objects.Directory_CertificateRequests, NamespaceIndex), typeof(BaseObjectState));

                    request.AddNotifier(SystemContext, ReferenceTypeIds.HasEventSource, true, folder);
                    folder.AddNotifier(SystemContext, ReferenceTypeIds.HasEventSource, false, request);

                    AddPredefinedNode(SystemContext, request);
                }
            }
        }

        /// <summary>
        /// <summary>
        /// The handler for the RenewCertificate method.
        /// </summary>
        private ServiceResult OnRenewCertificate(
            ISystemContext context,
            MethodState method,
            NodeId objectId,
            string applicationUri,
            string privateKeyFormat,
            string privateKeyPassword,
            ref NodeId requestId)
        {
            if (!CheckAdminAccess(context, false))
            {
                if (!CheckAccessForApplication(context, applicationUri))
                {
                    return StatusCodes.BadUserAccessDenied;
                }
            }

            return StatusCodes.BadNotImplemented;
        }
        
        /// <summary>
        /// The handler for the QueryServers method.
        /// </summary>
        private ServiceResult OnQueryServers(
            ISystemContext context,
            MethodState method,
            NodeId objectId,
            NodeId elementId,
            string applicationName,
            string machineName,
            string applicationUri,
            string productUri,
            ref ApplicationDescription[] servers)
        {
            return m_system.QueryServers(
                context,
                elementId,
                applicationName,
                machineName,
                applicationUri,
                productUri,
                out servers);
        }

        /// <summary>
        /// Checks if the current session has admin priviledges.
        /// </summary>
        private bool CheckAdminAccess(ISystemContext context, bool isGdsAdmin)
        {
            // no access if no identity.
            if (context.UserIdentity == null || context.UserIdentity.TokenType != UserTokenType.UserName)
            {
                return false;
            }

            // check that security is active.
            ServerSystemContext systemcontext = context as ServerSystemContext;

            if (systemcontext != null)
            {
                if (systemcontext.OperationContext.SecurityPolicyUri == SecurityPolicies.None)
                {
                    return false;
                }
            }

            SecurityToken securityToken = context.UserIdentity.GetSecurityToken();

            // check for a user name token.
            UserNameSecurityToken userNameToken = securityToken as UserNameSecurityToken;

            if (userNameToken == null)
            {
                return false;
            }
            
            // check for access to the user roles.
            ImpersonationContext impersonationContext = null;

            try
            {
                impersonationContext = UserIdentity.LogonUser(userNameToken, false);

                if (isGdsAdmin)
                {
                    if (m_userRoleManager.HasAccess("GdsAdministrator"))
                    {
                        return true;
                    }
                }
                else
                {
                    if (m_userRoleManager.HasAccess("ApplicationAdministrator"))
                    {
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                if (impersonationContext != null)
                {
                    impersonationContext.Dispose();
                }
            }

            return false;
        }

        /// <summary>
        /// The handler for the CreateApplication Method.
        /// </summary>
        private ServiceResult OnRegisterApplication(
            ISystemContext context,
            MethodState method,
            NodeId objectId,
            string applicationUri,
            string machineName,
            string applicationName,
            ApplicationType applicationType,
            string productUri,
            string gatewayServerUri,
            string[] discoveryUrls,
            ref string revisedApplicationUri)
        {
            if (!CheckAdminAccess(context, false))
            {
                return StatusCodes.BadUserAccessDenied;
            }

            return m_system.RegisterApplication(
                GetCertificateForContext(context),
                applicationUri,
                machineName,
                applicationName,
                applicationType,
                productUri,
                gatewayServerUri,
                discoveryUrls,
                out revisedApplicationUri);
        }

        /// <summary>
        /// The hander for the RequestCertificate method.
        /// </summary>
        private ServiceResult OnRequestCertificate(
            ISystemContext context,
            MethodState method,
            NodeId objectId,
            string applicationUri,
            string subjectName,
            string[] domainNames,
            string privateKeyFormat,
            string privateKeyPassword,
            bool createHttpsCertificate,
            ref NodeId requestId)
        {
            requestId = null;
            List<string> cancelledRequests = new List<string>();

            if (!CheckAdminAccess(context, false))
            {
                return StatusCodes.BadUserAccessDenied;
            }

            CertificateRequestState request = m_system.RequestCertificate(
                context as ServerSystemContext,
                GetCertificateForContext(context),
                applicationUri,
                subjectName,
                domainNames,
                privateKeyFormat,
                privateKeyPassword,
                createHttpsCertificate,
                cancelledRequests);

            lock (Lock)
            {
                for (int ii = 0; ii < cancelledRequests.Count; ii++)
                {
                    NodeId nodeId = ParsedNodeId.Construct(0, cancelledRequests[ii], NamespaceIndex);
                    DialogConditionState dialog = (DialogConditionState)FindPredefinedNode(nodeId, typeof(DialogConditionState));

                    if (dialog != null)
                    {
                        dialog.SetResponse(context, 1);
                        dialog.Retain.Value = false;
                        dialog.ReportEvent(context, dialog);

                        List<LocalReference> referencesToRemove = new List<LocalReference>();
                        RemovePredefinedNode(context, dialog, referencesToRemove);
                    }
                }

                BaseObjectState folder = (BaseObjectState)FindPredefinedNode(new NodeId(Opc.Ua.Gds.Objects.Directory_CertificateRequests, NamespaceIndex), typeof(BaseObjectState));

                request.AddNotifier(context, ReferenceTypeIds.HasEventSource, true, folder);
                folder.AddNotifier(context, ReferenceTypeIds.HasEventSource, false, request);

                request.OnRespond = OnRespond;

                request.ReportEvent(context, request);
                AddPredefinedNode(context, request);

                requestId = request.NodeId;

                return ServiceResult.Good;
            }
        }

        /// <summary>
        /// The handler for the response to the CertificateRequest Dialog Condition.
        /// </summary>
        private ServiceResult OnRespond(ISystemContext context, DialogConditionState dialog, int selectedResponse)
        {
            if (!CheckAdminAccess(context, true))
            {
                return StatusCodes.BadUserAccessDenied;
            }

            m_system.ApproveCertificateRequest(dialog as CertificateRequestState, null, selectedResponse == 0);
            
            lock (Lock)
            {
                dialog.SetResponse(context, selectedResponse);
                dialog.Retain.Value = false;
                dialog.ReportEvent(context, dialog);

                List<LocalReference> referencesToRemove = new List<LocalReference>();
                RemovePredefinedNode(context, dialog, referencesToRemove);

                return ServiceResult.Good;
            }
        }

        /// <summary>
        /// Returns the certificate for the context.
        /// </summary>
        private X509Certificate2 GetCertificateForContext(ISystemContext context)
        {
            ServerSystemContext systemcontext = context as ServerSystemContext;

            if (systemcontext != null)
            {
                if (systemcontext.OperationContext.SecurityPolicyUri == SecurityPolicies.None)
                {
                    return null;
                }

                if (systemcontext.OperationContext != null)
                {
                    if (systemcontext.OperationContext.Session != null)
                    {
                        return systemcontext.OperationContext.Session.ClientCertificate;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Checks if the certificate provides access to the method.
        /// </summary>
        private bool CheckAccessForRequest(ISystemContext context, NodeId requestId)
        {
            X509Certificate2 certificate = GetCertificateForContext(context);

            if (certificate == null)
            {
                return false;
            }

            return m_system.IsAuthorizedCertificate(requestId, certificate);
        }

        /// <summary>
        /// Checks if the certificate provides access to the method.
        /// </summary>
        private bool CheckAccessForApplication(ISystemContext context, string applicationUri)
        {
            X509Certificate2 certificate = GetCertificateForContext(context);

            if (certificate == null)
            {
                return false;
            }

            return m_system.IsAuthorizedCertificate(applicationUri, certificate);
        }

        /// <summary>
        /// The handler for the CheckCertificateRequestStatus method.
        /// </summary>
        private ServiceResult OnCheckRequestStatus(
            ISystemContext context,
            MethodState method,
            NodeId objectId,
            NodeId requestId,
            ref byte[] certificate,
            ref byte[] privateKey,
            ref byte[][] issuerCertificate)
        {
            if (!CheckAdminAccess(context, false))
            {
                if (!CheckAccessForRequest(context, requestId))
                {
                    return StatusCodes.BadUserAccessDenied;
                }
            }

            m_system.CheckRequestStatus(
                requestId,
                GetCertificateForContext(context),
                out certificate,
                out privateKey,
                out issuerCertificate);

            return ServiceResult.Good;
        }

        /// <summary>
        /// The handler for the GetTrustLists method.
        /// </summary>
        private ServiceResult OnGetTrustList(
            ISystemContext context,
            MethodState method,
            NodeId objectId,
            string applicationUri,
            bool returnHttpsLists,
            ref NodeId trustListId,
            ref byte[][] trustedCertificates,
            ref byte[][] trustedCertificateRevocationLists,
            ref byte[][] issuerCertificates,
            ref byte[][] issuerCertificateRevocationLists)
        {
            if (!CheckAdminAccess(context, false))
            {
                if (!CheckAccessForApplication(context, applicationUri))
                {
                    return StatusCodes.BadUserAccessDenied;
                }
            }

            m_system.GetTrustList(
                GetCertificateForContext(context),
                applicationUri,
                returnHttpsLists,
                out trustedCertificates,
                out trustedCertificateRevocationLists,
                out issuerCertificates,
                out issuerCertificateRevocationLists);

            return ServiceResult.Good;
        }
        
        /// <summary>
        /// The handler for the RevokeCertificate method.
        /// </summary>
        private ServiceResult OnRevokeCertificate(
            ISystemContext context,
            MethodState method,
            NodeId objectId,
            string thumbprint)
        {
            if (!CheckAdminAccess(context, false))
            {
                return StatusCodes.BadUserAccessDenied;
            }

            m_system.RevokeCertificate(thumbprint);

            return ServiceResult.Good;
        }

        /// <summary>
        /// Frees any resources allocated for the address space.
        /// </summary>
        public override void DeleteAddressSpace()
        {
            lock (Lock)
            {
                base.DeleteAddressSpace();
            }
        }

        /// <summary>
        /// Returns a unique handle for the node.
        /// </summary>
        protected override NodeHandle GetManagerHandle(ServerSystemContext context, NodeId nodeId, IDictionary<NodeId, NodeState> cache)
        {
            lock (Lock)
            {
                // quickly exclude nodes that are not in the namespace.
                if (!IsNodeIdInNamespace(nodeId))
                {
                    return null;
                }

                NodeState node = null;

                // check cache (the cache is used because the same node id can appear many times in a single request).
                if (cache != null)
                {
                    if (cache.TryGetValue(nodeId, out node))
                    {
                        return new NodeHandle(nodeId, node);
                    }
                }

                // look up predefined node.
                if (PredefinedNodes.TryGetValue(nodeId, out node))
                {
                    NodeHandle handle = new NodeHandle(nodeId, node);

                    if (cache != null)
                    {
                        cache.Add(nodeId, node);
                    }

                    return handle;
                }

                // parse the node id.
                ParsedNodeId pnd = ParsedNodeId.Parse(nodeId);

                if (pnd != null)
                {
                    NodeHandle handle = new NodeHandle();
                    handle.NodeId = nodeId;
                    handle.RootId = pnd.RootId;
                    handle.ComponentPath = pnd.ComponentPath;
                    handle.ParsedNodeId = pnd;
                    handle.Validated = false;
                    return handle;
                }

                return null;
            }
        }

        /// <summary>
        /// Verifies that the specified node exists.
        /// </summary>
        protected override NodeState ValidateNode(
            ServerSystemContext context,
            NodeHandle handle,
            IDictionary<NodeId, NodeState> cache)
        {
            // not valid if no root.
            if (handle == null)
            {
                return null;
            }

            // check if previously validated.
            if (handle.Validated)
            {
                return handle.Node;
            }

            // lookup in operation cache.
            NodeState target = FindNodeInCache(context, handle, cache);

            if (target == null)
            {
                target = m_system.CreateNode(context, handle.ParsedNodeId as ParsedNodeId);

                if (target == null)
                {
                    return null;
                }
            }

            return ValidationComplete(context, handle, target, cache);
        }
        #endregion

        #region Overridden Methods
        #endregion

        #region Private Fields
        private GdsServerConfiguration m_configuration;
        private Gds m_system;
        private UserRoleManager m_userRoleManager;
        #endregion
    }
}
