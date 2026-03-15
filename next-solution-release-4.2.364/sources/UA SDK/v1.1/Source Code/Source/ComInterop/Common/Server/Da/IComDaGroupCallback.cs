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
    public interface IComDaGroupCallback : IDisposable
    {
        /// <summary>
        /// Called when a data change or read complete occurs.
        /// </summary>
        /// <param name="groupHandle">The group handle.</param>
        /// <param name="isRefresh">If set to <c>true</c> it is a response to a refresh request.</param>
        /// <param name="cancelId">The cancel id.</param>
        /// <param name="transactionId">The transaction id.</param>
        /// <param name="clientHandles">The client handles.</param>
        /// <param name="values">The values.</param>
        void ReadCompleted(
            int groupHandle,
            bool isRefresh,
            int cancelId,
            int transactionId,
		    int[] clientHandles,
		    DaValue[] values);

        /// <summary>
        /// Called when a write complete occurs.
        /// </summary>
        /// <param name="groupHandle">The group handle.</param>
        /// <param name="transactionId">The transaction id.</param>
        /// <param name="clientHandles">The client handles.</param>
        /// <param name="errors">The errors.</param>
	    void WriteCompleted(
		    int groupHandle,
		    int transactionId,
		    int[] clientHandles,
            int[] errors);

        /// <summary>
        /// Called when a cancel succeeds.
        /// </summary>
        /// <param name="groupHandle">The group handle.</param>
        /// <param name="transactionId">The transaction id.</param>
	    void CancelSucceeded(
		    int groupHandle,
		    int transactionId);
    }
}
