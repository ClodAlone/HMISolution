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
using OpcRcw.Hda;

namespace Opc.Ua.Com.Client
{
    /// <summary>
    /// A class that implements the IOPCHDA_DataCallback interface.
    /// </summary>
    internal class ComHdaDataCallback : OpcRcw.Hda.IOPCHDA_DataCallback, IDisposable
    {
	    #region Constructors
	    /// <summary>
	    /// Initializes the object with the containing subscription object.
	    /// </summary>
        public ComHdaDataCallback(ComHdaClient server)
	    { 
            // save group.
            m_server = server;

		    // create connection point.
            m_connectionPoint = new ConnectionPoint(server.Unknown, typeof(OpcRcw.Hda.IOPCHDA_DataCallback).GUID);

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
        #endregion

	    #region IOPCDataCallback Members
        /// <summary>
        /// Called when a data change arrives.
        /// </summary>
        public void OnDataChange(
            int dwTransactionID,
            int hrStatus,
            int dwNumItems,
            OPCHDA_ITEM[] pItemValues,
            int[] phrErrors)
        {
            try
            {
                // TBD
            }
            catch (Exception e)
            {
                Utils.Trace(e, "Unexpected error processing OnDataChange callback.");
            }
        }

        /// <summary>
        /// Called when an async read completes.
        /// </summary>
        public void OnReadComplete(
            int dwTransactionID, 
            int hrStatus,
            int dwNumItems,  
            OPCHDA_ITEM[] pItemValues,
            int[] phrErrors)
        {
            try
            {
                // TBD
            }
            catch (Exception e)
            {
                Utils.Trace(e, "Unexpected error processing OnReadComplete callback.");
            }
        }

        /// <summary>
        /// Called when an async read modified completes.
        /// </summary>
        public void OnReadModifiedComplete(
            int dwTransactionID, 
            int hrStatus,
            int dwNumItems, 
            OPCHDA_MODIFIEDITEM[] pItemValues,
            int[] phrErrors)
        {
            try
            {
                // TBD
            }
            catch (Exception e)
            {
                Utils.Trace(e, "Unexpected error processing OnReadModifiedComplete callback.");
            }
        }

        /// <summary>
        /// Called when an async read attributes completes.
        /// </summary>
        public void OnReadAttributeComplete(
            int dwTransactionID, 
            int hrStatus,
            int hClient, 
            int dwNumItems, 
            OPCHDA_ATTRIBUTE[] pAttributeValues,
            int[] phrErrors)
        {
            try
            {
                // TBD
            }
            catch (Exception e)
            {
                Utils.Trace(e, "Unexpected error processing OnReadAttributeComplete callback.");
            }
        }
        
        /// <summary>
        /// Called when an async read annotations completes.
        /// </summary>
        public  void OnReadAnnotations(
            int dwTransactionID, 
            int hrStatus,
            int dwNumItems, 
            OPCHDA_ANNOTATION[] pAnnotationValues,
            int[] phrErrors)
        {
            try
            {
                // TBD
            }
            catch (Exception e)
            {
                Utils.Trace(e, "Unexpected error processing OnReadAnnotations callback.");
            }
        }

        /// <summary>
        /// Called when an async insert annotations completes.
        /// </summary>
        public void OnInsertAnnotations (
            int dwTransactionID, 
            int hrStatus,
            int dwCount, 
            int[] phClients, 
            int[] phrErrors)
        {
            try
            {
                // TBD
            }
            catch (Exception e)
            {
                Utils.Trace(e, "Unexpected error processing OnInsertAnnotations callback.");
            }
        }
        
        /// <summary>
        /// Called when a playback result arrives.
        /// </summary>
        public void OnPlayback (
            int dwTransactionID, 
            int hrStatus,
            int dwNumItems, 
            IntPtr ppItemValues, 
            int[] phrErrors)
        {
            try
            {
                // TBD
            }
            catch (Exception e)
            {
                Utils.Trace(e, "Unexpected error processing OnPlayback callback.");
            }
        }
        
        /// <summary>
        /// Called when a async update completes.
        /// </summary>
        public void OnUpdateComplete (
            int dwTransactionID, 
            int hrStatus,
            int dwCount, 
            int[] phClients, 
            int[] phrErrors)
        {
            try
            {
                // TBD
            }
            catch (Exception e)
            {
                Utils.Trace(e, "Unexpected error processing OnUpdateComplete callback.");
            }
        }

        /// <summary>
        /// Called when a async opeartion cancel completes.
        /// </summary>
        public void OnCancelComplete(
            int dwCancelID)
        {
            try
            {
                // TBD
            }
            catch (Exception e)
            {
                Utils.Trace(e, "Unexpected error processing OnCancelComplete callback.");
            }
        }
	    #endregion

	    #region Private Members
	    private ComHdaClient m_server;
	    private ConnectionPoint m_connectionPoint;
	    #endregion
    }
}
