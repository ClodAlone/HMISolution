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

namespace Opc.Ua.Com.Client
{        
    /// <summary>
    /// A class that implements the IOPCShutdown interface.
    /// </summary>
    public class ShutdownCallback : OpcRcw.Comn.IOPCShutdown, IDisposable
    {
	    #region Constructors
	    /// <summary>
	    /// Initializes the object with the containing subscription object.
	    /// </summary>
	    public ShutdownCallback(object server, ServerShutdownEventHandler handler)
	    { 
		    try
		    {
                m_server  = server;
                m_handler = handler;

			    // create connection point.
			    m_connectionPoint = new ConnectionPoint(server, typeof(OpcRcw.Comn.IOPCShutdown).GUID);

			    // advise.
			    m_connectionPoint.Advise(this);
		    }
		    catch (Exception e)
		    {
			    throw new ServiceResultException(e, StatusCodes.BadOutOfService);
		    }
	    }
	    #endregion
        
        #region IDisposable Members
        /// <summary>
        /// Frees any unmanaged resources.
        /// </summary>
        public void Dispose()
        {   
            Dispose(true);
        }

        /// <summary>
        /// An overrideable version of the Dispose.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (m_connectionPoint != null)
            {
                m_connectionPoint.Dispose();
                m_connectionPoint = null;
            }
        }
        #endregion

	    #region IOPCShutdown Members
	    /// <summary>
	    /// Called when a data changed event is received.
	    /// </summary>
	    public void ShutdownRequest(string szReason)
	    {
		    try
		    {
                if (m_handler != null)
                {
                    m_handler(m_server, szReason);
                }
		    }
		    catch (Exception e) 
		    { 
                Utils.Trace(e, "Unexpected error processing callback.");
		    }
	    }
	    #endregion

	    #region Private Members
	    private object m_server;
	    private ServerShutdownEventHandler m_handler;
	    private ConnectionPoint m_connectionPoint;
	    #endregion
    }
    
    /// <summary>
    /// A delegate used to receive server shutdown events.
    /// </summary>
    public delegate void ServerShutdownEventHandler(object sender, string reason);
}
