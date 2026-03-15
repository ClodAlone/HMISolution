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
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Opc.Ua;
using Opc.Ua.Server;
using Opc.Ua.Com;
using OpcRcw.Da;

namespace Opc.Ua.Com.Client
{
    /// <summary>
    /// Manages the DA COM connections used by the UA server.
    /// </summary>
    public class ComDaClientManager : ComClientManager
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ComDaClientManager"/> class.
        /// </summary>
        public ComDaClientManager()
        {
        }
        #endregion

        #region Public Members
        /// <summary>
        /// Selects the DA COM client to use for the current context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="useDefault">True if the the default instance should be returned.</param>
        /// <returns>A DA COM client instance.</returns>
        public new ComDaClient SelectClient(ServerSystemContext context, bool useDefault)
        {
            return (ComDaClient)base.SelectClient(context, useDefault);
        }
        #endregion

        #region Protected Members
        /// <summary>
        /// Gets or sets the default COM client instance.
        /// </summary>
        /// <value>The default client.</value>
        protected new ComDaClient DefaultClient
        {
            get { return base.DefaultClient as ComDaClient; }
            set { base.DefaultClient = value; }
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>The configuration.</value>
        protected new ComDaClientConfiguration Configuration
        {
            get { return base.Configuration as ComDaClientConfiguration; }
        }

        /// <summary>
        /// Creates a new client object.
        /// </summary>
        protected override ComClient CreateClient()
        {
            return new ComDaClient(Configuration);
        }

        /// <summary>
        /// Updates the status node.
        /// </summary>
        protected override bool UpdateStatus()
        {
            // get the status from the server.
            ComDaClient client = DefaultClient;
            OPCSERVERSTATUS? status = client.GetStatus();

            // check the client has been abandoned.
            if (!Object.ReferenceEquals(client, DefaultClient))
            {
                return false;
            }

            // update the server status.
            lock (StatusNodeLock)
            {
                StatusNode.ServerUrl.Value = Configuration.ServerUrl;

                if (status != null)
                {
                    StatusNode.SetStatusCode(DefaultSystemContext, StatusCodes.Good, DateTime.UtcNow);

                    StatusNode.ServerState.Value = Utils.Format("{0}", status.Value.dwServerState);
                    StatusNode.CurrentTime.Value = ComUtils.GetDateTime(status.Value.ftCurrentTime);
                    StatusNode.LastUpdateTime.Value = ComUtils.GetDateTime(status.Value.ftLastUpdateTime);
                    StatusNode.StartTime.Value = ComUtils.GetDateTime(status.Value.ftStartTime);
                    StatusNode.VendorInfo.Value = status.Value.szVendorInfo;
                    StatusNode.SoftwareVersion.Value = Utils.Format("{0}.{1}.{2}", status.Value.wMajorVersion, status.Value.wMinorVersion, status.Value.wBuildNumber);
                }
                else
                {
                    StatusNode.SetStatusCode(DefaultSystemContext, StatusCodes.BadOutOfService, DateTime.UtcNow);
                }

                StatusNode.ClearChangeMasks(DefaultSystemContext, true);
                return status != null;
            }
        }
        #endregion

        #region Private Fields
        #endregion
    }
}
