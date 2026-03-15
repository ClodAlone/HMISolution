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
using System.Threading;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using Opc.Ua.Client;

namespace Opc.Ua.Com.Server
{
    /// <summary>
    /// Specifies the parameters used to create a DA group item.
    /// </summary>
    public class ComDaCreateItemRequest
    {
        #region Public Properties
        /// <summary>
        /// Gets or sets the item id.
        /// </summary>
        /// <value>The item id.</value>
        public string ItemId
        {
            get { return m_itemId; }
            set { m_itemId = value; }
        }

        /// <summary>
        /// Gets or sets the access path.
        /// </summary>
        /// <value>The access path.</value>
        public string AccessPath
        {
            get { return m_accessPath; }
            set { m_accessPath = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this item is active.
        /// </summary>
        /// <value><c>true</c> if active; otherwise, <c>false</c>.</value>
        public bool Active
        {
            get { return m_active; }
            set { m_active = value; }
        }

        /// <summary>
        /// Gets or sets the client handle.
        /// </summary>
        /// <value>The client handle.</value>
        public int ClientHandle
        {
            get { return m_clientHandle; }
            set { m_clientHandle = value; }
        }

        /// <summary>
        /// Gets or sets the requested data type.
        /// </summary>
        /// <value>The requested data type.</value>
        public short RequestedDataType
        {
            get { return m_requestedDataType; }
            set { m_requestedDataType = value; }
        }

        /// <summary>
        /// Gets or sets the server handle.
        /// </summary>
        /// <value>The server handle.</value>
        public int ServerHandle
        {
            get { return m_serverHandle; }
            set { m_serverHandle = value; }
        }

        /// <summary>
        /// Gets or sets the canonical data type for the item.
        /// </summary>
        /// <value>The canonical data type.</value>
        public short CanonicalDataType
        {
            get { return m_canonicalDataType; }
            set { m_canonicalDataType = value; }
        }

        /// <summary>
        /// Gets or sets the access rights for the item.
        /// </summary>
        /// <value>The access rights.</value>
        public int AccessRights
        {
            get { return m_accessRights; }
            set { m_accessRights = value; }
        }

        /// <summary>
        /// Gets or sets the error associated with the operation.
        /// </summary>
        /// <value>The error.</value>
        public int Error
        {
            get { return m_error; }
            set { m_error = value; }
        }
        #endregion

        #region Private Fields
        private string m_itemId;
        private string m_accessPath;
        private bool m_active;
        private int m_clientHandle;
        private short m_requestedDataType;
        private int m_serverHandle;
        private short m_canonicalDataType;
        private int m_accessRights;
        private int m_error;
        #endregion
    }
}
