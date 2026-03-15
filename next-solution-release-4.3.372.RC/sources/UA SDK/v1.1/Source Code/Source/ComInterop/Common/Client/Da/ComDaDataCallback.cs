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
using OpcRcw.Da;

namespace Opc.Ua.Com.Client
{
    /// <summary>
    /// A class that implements the IOPCDataCallback interface.
    /// </summary>
    internal class ComDaDataCallback : OpcRcw.Da.IOPCDataCallback, IDisposable
    {
	    #region Constructors
	    /// <summary>
	    /// Initializes the object with the containing subscription object.
	    /// </summary>
	    public ComDaDataCallback(ComDaGroup group)
	    { 
            // save group.
            m_group = group;

		    // create connection point.
		    m_connectionPoint = new ConnectionPoint(group.Unknown, typeof(OpcRcw.Da.IOPCDataCallback).GUID);

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
	    /// Called when a data changed event is received.
	    /// </summary>
	    public void OnDataChange(
		    int                  dwTransid,
		    int                  hGroup,
		    int                  hrMasterquality,
		    int                  hrMastererror,
		    int                  dwCount,
		    int[]                phClientItems,
		    object[]             pvValues,
		    short[]              pwQualities,
		    System.Runtime.InteropServices.ComTypes.FILETIME[] pftTimeStamps,
		    int[]                pErrors)
	    {
		    try
		    {
			    // unmarshal item values.
			    DaValue[] values = ComDaGroup.GetItemValues(
				    dwCount,
				    pvValues, 
				    pwQualities, 
				    pftTimeStamps, 
				    pErrors);

			    // invoke the callback.
			    m_group.OnDataChange(phClientItems, values);
		    }
		    catch (Exception e) 
		    { 
                Utils.Trace(e, "Unexpected error processing OnDataChange callback.");
		    }
	    }

	    /// <summary>
	    /// Called when an asynchronous read operation completes.
	    /// </summary>
	    public void OnReadComplete(
		    int                  dwTransid,
		    int                  hGroup,
		    int                  hrMasterquality,
		    int                  hrMastererror,
		    int                  dwCount,
		    int[]                phClientItems,
		    object[]             pvValues,
		    short[]              pwQualities,
		    System.Runtime.InteropServices.ComTypes.FILETIME[] pftTimeStamps,
		    int[]                pErrors)
	    {
		    try
		    {
			    // unmarshal item values.
			    DaValue[] values = ComDaGroup.GetItemValues(
				    dwCount,
				    pvValues, 
				    pwQualities, 
				    pftTimeStamps, 
				    pErrors);

			    // invoke the callback.
                m_group.OnReadComplete(dwTransid, phClientItems, values);
		    }
		    catch (Exception e) 
		    { 
                Utils.Trace(e, "Unexpected error processing OnReadComplete callback.");
		    }
	    }
        
	    /// <summary>
	    /// Called when an asynchronous write operation completes.
	    /// </summary>
	    public void OnWriteComplete(
		    int   dwTransid,
		    int   hGroup,
		    int   hrMastererror,
		    int   dwCount,
		    int[] phClientItems,
		    int[] pErrors)
	    {
		    try
		    {
                m_group.OnWriteComplete(dwTransid, phClientItems, pErrors);
		    }
		    catch (Exception e) 
		    { 
                Utils.Trace(e, "Unexpected error processing OnWriteComplete callback.");
		    }
	    }
                    
	    /// <summary>
	    /// Called when an asynchronous operation is cancelled.
	    /// </summary>
	    public void OnCancelComplete(
		    int dwTransid,
		    int hGroup)
	    {
	    }
	    #endregion

	    #region Private Members
	    private ComDaGroup m_group;
	    private ConnectionPoint m_connectionPoint;
	    #endregion
    }
}
