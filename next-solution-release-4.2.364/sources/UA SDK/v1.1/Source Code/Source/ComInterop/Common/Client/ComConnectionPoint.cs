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
using System.Runtime.InteropServices;
using OpcRcw.Comn;

namespace Opc.Ua.Com.Client
{
	/// <summary>
	/// Manages a connection point with a COM server.
	/// </summary>
	public class ConnectionPoint : IDisposable
	{
		#region Constructors
		/// <summary>
		/// Initializes the object by finding the specified connection point.
		/// </summary>
		public ConnectionPoint(object server, Guid iid)
		{
            OpcRcw.Comn.IConnectionPointContainer cpc = server as OpcRcw.Comn.IConnectionPointContainer;

            if (cpc == null)
            {
                throw ServiceResultException.Create(StatusCodes.BadCommunicationError, "Server does not support the IConnectionPointContainer interface.");
            }

            cpc.FindConnectionPoint(ref iid, out m_server);
		}

		/// <summary>
		/// Sets private members to default values.
		/// </summary>
		private void Initialize()
		{
			m_server = null;
			m_cookie = 0;
			m_refs   = 0;
		}
		#endregion
    
	    #region IDisposable Members
        /// <summary>
        /// The finializer implementation.
        /// </summary>
        ~ConnectionPoint() 
        {
            Dispose(false);
        }
        
        /// <summary>
        /// Frees any unmanaged resources.
        /// </summary>
        public void Dispose()
        {   
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// An overrideable version of the Dispose.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            object server = System.Threading.Interlocked.Exchange(ref m_server, null);

            if (server != null)
            {
                ComUtils.ReleaseServer(server);
            }
        }
        #endregion
        
		#region Public Properties
        /// <summary>
        /// The cookie returned by the server.
        /// </summary>
        public int Cookie
        {
            get { return m_cookie; }
        }
        #endregion

		#region IConnectionPoint Members
		/// <summary>
		/// Establishes a connection, if necessary and increments the reference count.
		/// </summary>
		public int Advise(object callback)
		{
            if (m_refs++ == 0)
            {
                m_server.Advise(callback, out m_cookie);
            }

			return m_refs;
		}

		/// <summary>
		/// Decrements the reference count and closes the connection if no more references.
		/// </summary>
		public int Unadvise()
		{
			if (--m_refs == 0) m_server.Unadvise(m_cookie);
			return m_refs;
		}
		#endregion

		#region Private Members
        private OpcRcw.Comn.IConnectionPoint m_server;
		private int m_cookie;
		private int m_refs;
		#endregion
	}
}
