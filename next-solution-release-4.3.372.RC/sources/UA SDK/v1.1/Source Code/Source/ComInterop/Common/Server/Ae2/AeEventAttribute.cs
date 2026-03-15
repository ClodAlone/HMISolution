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
    /// Stores an instance declaration fetched from the server.
    /// </summary>
    public class AeEventAttribute
    {
        /// <summary>
        /// The proxy assigned identifier for the attribute.
        /// </summary>
        public uint LocalId;

        /// <summary>
        /// The type that the declaration belongs to.
        /// </summary>
        public NodeId RootTypeId { get; set; }

        /// <summary>
        /// The browse path to the instance declaration.
        /// </summary>
        public QualifiedNameCollection BrowsePath { get; set; }

        /// <summary>
        /// The browse path to the instance declaration.
        /// </summary>
        public string BrowsePathDisplayText { get; set; }

        /// <summary>
        /// A localized path to the instance declaration.
        /// </summary>
        public string DisplayPath { get; set; }

        /// <summary>
        /// The node id for the instance declaration.
        /// </summary>
        public NodeId NodeId { get; set; }

        /// <summary>
        /// The node class of the instance declaration.
        /// </summary>
        public NodeClass NodeClass { get; set; }

        /// <summary>
        /// The browse name for the instance declaration.
        /// </summary>
        public QualifiedName BrowseName { get; set; }

        /// <summary>
        /// The display name for the instance declaration.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// The description for the instance declaration.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The modelling rule for the instance declaration (i.e. Mandatory or Optional).
        /// </summary>
        public NodeId ModellingRule { get; set; }

        /// <summary>
        /// The data type for the instance declaration.
        /// </summary>
        public NodeId DataType { get; set; }

        /// <summary>
        /// The value rank for the instance declaration.
        /// </summary>
        public int ValueRank { get; set; }

        /// <summary>
        /// The built-in type parent for the data type.
        /// </summary>
        public BuiltInType BuiltInType { get; set; }

        /// <summary>
        /// An instance declaration that has been overridden by the current instance.
        /// </summary>
        public AeEventAttribute OverriddenDeclaration { get; set; }

        /// <summary>
        /// The attribute is not visible to clients.
        /// </summary>
        public bool Hidden { get; set; }
    }
}
