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
using Opc.Ua.Server;
using Opc.Ua.Com;
using Opc.Ua.Com.Client;
using OpcRcw.Ae;

namespace Opc.Ua.Com.Client
{
    /// <summary>
    /// A class that implements the IOPCEventSink interface.
    /// </summary>
    internal class ComAeEventSink : OpcRcw.Ae.IOPCEventSink, IDisposable
    {
	    #region Constructors
	    /// <summary>
	    /// Initializes the object with the containing subscription object.
	    /// </summary>
	    public ComAeEventSink(ComAeSubscriptionClient subscription)
	    { 
            // save group.
            m_subscription = subscription;

		    // create connection point.
		    m_connectionPoint = new ConnectionPoint(subscription.Unknown, typeof(IOPCEventSink).GUID);

		    // advise.
		    m_connectionPoint.Advise(this);
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
                if (disposing)
                {
                    m_connectionPoint.Dispose();
                    m_connectionPoint = null;
                }
            }
        }
        #endregion

	    #region Public Properties
        /// <summary>
        /// Whether the callback is connected.
        /// </summary>
        public bool Connected 
        {
            get 
            {
                return m_connectionPoint != null;
            }
        }

        /// <summary>
        /// Gets the cookie returned by the server.
        /// </summary>
        /// <value>The cookie.</value>
        public int Cookie
        {
            get
            {
                if (m_connectionPoint != null)
                {
                    return m_connectionPoint.Cookie;
                }

                return 0;
            }
        }
        #endregion

	    #region ComAeEventSink Members
        /// <summary>
        /// Called when one or events are produce by the server.
        /// </summary>
        public void OnEvent(
            int hClientSubscription,
            int bRefresh,
            int bLastRefresh,
            int dwCount,
            ONEVENTSTRUCT[] pEvents)
	    {
		    try
		    {
                if (bRefresh == 0)
                {
			        m_subscription.OnEvent(pEvents);
                }
                else
                {
			        m_subscription.OnRefresh(pEvents, bLastRefresh != 0);
                }
		    }
		    catch (Exception e) 
		    { 
                Utils.Trace(e, "Unexpected error processing OnDataChange callback.");
		    }
	    }
	    #endregion

	    #region Private Members
	    private ComAeSubscriptionClient m_subscription;
	    private ConnectionPoint m_connectionPoint;
	    #endregion
    }
}
