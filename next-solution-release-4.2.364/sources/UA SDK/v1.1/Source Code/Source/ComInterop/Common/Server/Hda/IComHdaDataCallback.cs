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
    /// Used to report asynchronous events produced by an HDA server.
    /// </summary>
    public interface IComHdaDataCallback : IDisposable
    {
        /// <summary>
        /// Called when a data change operation completes.
        /// </summary>
	    void OnDataChange(
		    int transactionId, 
		    List<HdaReadRequest> results);

        /// <summary>
        /// Called when a read operation completes.
        /// </summary>
	    void OnReadComplete(
		    int transactionId, 
		    List<HdaReadRequest> results);

        /// <summary>
        /// Called when a read modified operation completes.
        /// </summary>
	    void OnReadModifiedComplete(
		    int transactionId, 
		    List<HdaReadRequest> results);

        /// <summary>
        /// Called when a read attributes operation completes.
        /// </summary>
	    void OnReadAttributeComplete(
		    int transactionId, 
		    List<HdaReadRequest> results);

        /// <summary>
        /// Called when a read annotations operation completes.
        /// </summary>
	    void OnReadAnnotations(
		    int transactionId, 
		    List<HdaReadRequest> results);

        /// <summary>
        /// Called when an insert annotations operation completes.
        /// </summary>
	    void OnInsertAnnotations(
            int transactionId,
            List<HdaUpdateRequest> results); 

        /// <summary>
        /// Called when a update operation completes.
        /// </summary>
	    void OnUpdateComplete(
		    int transactionId, 
		    List<HdaUpdateRequest> results); 

        /// <summary>
        /// Called when a cancel operation completes.
        /// </summary>
	    void OnCancelComplete(int transactionId);
    }
}
