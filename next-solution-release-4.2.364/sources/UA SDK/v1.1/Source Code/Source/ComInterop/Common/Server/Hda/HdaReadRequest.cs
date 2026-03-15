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
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.InteropServices;
using Opc.Ua.Client;
using OpcRcw.Hda;

namespace Opc.Ua.Com.Server
{
    /// <summary>
    /// Stores information about an HDA read request.
    /// </summary>
    public class HdaReadRequest
    {
        /// <summary>
        /// The handle for the requested item.
        /// </summary>
        public HdaItemHandle Handle;

        /// <summary>
        /// The node id to read.
        /// </summary>
        public NodeId NodeId;

        /// <summary>
        /// The client handle.
        /// </summary>
        public int ClientHandle; 

        /// <summary>
        /// The attribute being read.
        /// </summary>
        public uint AttributeId;

        /// <summary>
        /// The aggregate used to calculate the results.
        /// </summary>
        public uint AggregateId;

        /// <summary>
        /// Any error associated with the item.
        /// </summary>
        public int Error;

        /// <summary>
        /// Any error associated with the item.
        /// </summary>
        public List<DaValue> Values;

        /// <summary>
        /// Metadata associated with the values.
        /// </summary>
        public List<ModificationInfo> ModificationInfos;

        /// <summary>
        /// A continuation point returned by the server.
        /// </summary>
        public byte[] ContinuationPoint;

        /// <summary>
        /// A flag that indicates that all data has been read.
        /// </summary>
        public bool IsComplete;
    }
}
