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
using Opc.Ua;

namespace Opc.Ua.Com.Client
{
    /// <summary>
    /// Stores the history of an HDA item.
    /// </summary>
    public class HdaItemHistoryData
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="HdaItemHistoryData"/> class.
        /// </summary>
        public HdaItemHistoryData()
        {
        }
        #endregion

        #region Public Members
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
        /// Gets or sets the error.
        /// </summary>
        /// <value>The error.</value>
        public int Error
        {
            get { return m_error; }
            set { m_error = value; }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public object[] Values
        {
            get { return m_values; }
            set { m_values = value; }
        }

        /// <summary>
        /// Gets or sets the qualities.
        /// </summary>
        /// <value>The qualities.</value>
        public int[] Qualities
        {
            get { return m_qualities; }
            set { m_qualities = value; }
        }

        /// <summary>
        /// Gets or sets the timestamp.
        /// </summary>
        /// <value>The timestamp.</value>
        public DateTime[] Timestamps
        {
            get { return m_timestamps; }
            set { m_timestamps = value; }
        }

        /// <summary>
        /// Gets or sets the modifications.
        /// </summary>
        /// <value>The modifications.</value>
        public ModificationInfo[] Modifications
        {
            get { return m_modifications; }
            set { m_modifications = value; }
        }
        #endregion

        #region Private Fields
        private int m_serverHandle;
        private object[] m_values;
        private int[] m_qualities;
        private DateTime[] m_timestamps;
        private ModificationInfo[] m_modifications;
        private int m_error;
        #endregion
    }
}
