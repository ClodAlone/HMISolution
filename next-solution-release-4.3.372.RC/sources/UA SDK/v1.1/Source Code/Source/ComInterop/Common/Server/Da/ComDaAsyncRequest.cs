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
    /// Specifies the parameters for an async request.
    /// </summary>
    public class ComDaAsnycRequest
    {
        #region Public Properties
        /// <summary>
        /// Gets or sets the cancel id.
        /// </summary>
        /// <value>The cancel id.</value>
        public int CancelId
        {
            get { return m_cancelId; }
            set { m_cancelId = value; }
        }

        /// <summary>
        /// Gets or sets a flag indicating that the request was cancelled.
        /// </summary>
        /// <value>True if cancelled.</value>
        public bool Cancelled
        {
            get { return m_cancelled; }
            set { m_cancelled = value; }
        }

        /// <summary>
        /// Gets or sets the transaction id.
        /// </summary>
        /// <value>The transaction id.</value>
        public int TransactionId
        {
            get { return m_transactionId; }
            set { m_transactionId = value; }
        }

        /// <summary>
        /// Gets or sets the server handles.
        /// </summary>
        /// <value>The server handles.</value>
        public int[] ServerHandles
        {
            get { return m_serverHandles; }
            set { m_serverHandles = value; }
        }

        /// <summary>
        /// Gets or sets the client handles.
        /// </summary>
        /// <value>The client handles.</value>
        public int[] ClientHandles
        {
            get { return m_clientHandles; }
            set { m_clientHandles = value; }
        }
        #endregion

        #region Private Fields
        private int m_transactionId;
        private int m_cancelId;
        private bool m_cancelled;
        private int[] m_serverHandles;
        private int[] m_clientHandles;
        #endregion
    }

    /// <summary>
    /// Specifies the parameters for an async read request.
    /// </summary>
    public class ComDaAsnycReadRequest : ComDaAsnycRequest
    {
        #region Public Properties
        /// <summary>
        /// Gets or sets the max age.
        /// </summary>
        /// <value>The max age.</value>
        public uint MaxAge
        {
            get { return m_maxAge; }
            set { m_maxAge = value; }
        }

        /// <summary>
        /// Gets or sets a flag indicating that it is a read request.
        /// </summary>
        /// <value>The refresh flag.</value>
        public bool IsRefresh
        {
            get { return m_isRefresh; }
            set { m_isRefresh = value; }
        }

        /// <summary>
        /// Gets or sets a flag indicating that it is the first update after activation.
        /// </summary>
        public bool IsFirstUpdate { get; set; }
        #endregion

        #region Private Fields
        private uint m_maxAge;
        private bool m_isRefresh;
        #endregion
    }

    /// <summary>
    /// Specifies the parameters for an async write request.
    /// </summary>
    public class ComDaAsnycWriteRequest : ComDaAsnycRequest
    {
        #region Public Properties
        /// <summary>
        /// Gets or sets the results.
        /// </summary>
        /// <value>The results.</value>
        public DaValue[] Values
        {
            get { return m_values; }
            set { m_values = value; }
        }
        #endregion

        #region Private Fields
        private DaValue[] m_values;
        #endregion
    }
}
