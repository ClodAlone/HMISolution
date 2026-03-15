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
using System.Reflection;

namespace Opc.Ua
{
	/// <summary>
	/// A class that defines constants used by UA applications.
	/// </summary>
    /// <exclude /> 
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.CodeGenerator", "1.0.0.0")]
	public static partial class Attributes
	{
        /// <summary>
        /// The canonical identifier for the node.
        /// </summary>
        public const uint NodeId = 1;

        /// <summary>
        /// The class of the node.
        /// </summary>
        public const uint NodeClass = 2;

        /// <summary>
        /// A non-localized, human readable name for the node.
        /// </summary>
        public const uint BrowseName = 3;

        /// <summary>
        /// A localized, human readable name for the node.
        /// </summary>
        public const uint DisplayName = 4;

        /// <summary>
        /// A localized description for the node.
        /// </summary>
        public const uint Description = 5;

        /// <summary>
        /// Indicates which attributes are writeable.
        /// </summary>
        public const uint WriteMask = 6;

        /// <summary>
        /// Indicates which attributes are writeable by the current user.
        /// </summary>
        public const uint UserWriteMask = 7;

        /// <summary>
        /// Indicates that a type node may not be instantiated.
        /// </summary>
        public const uint IsAbstract = 8;

        /// <summary>
        /// Indicates that forward and inverse references have the same meaning.
        /// </summary>
        public const uint Symmetric = 9;

        /// <summary>
        /// The browse name for an inverse reference.
        /// </summary>
        public const uint InverseName = 10;

        /// <summary>
        /// Indicates that following forward references within a view will not cause a loop.
        /// </summary>
        public const uint ContainsNoLoops = 11;

        /// <summary>
        /// Indicates that the node can be used to subscribe to events.
        /// </summary>
        public const uint EventNotifier = 12;

        /// <summary>
        /// The value of a variable.
        /// </summary>
        public const uint Value = 13;

        /// <summary>
        /// The node id of the data type for the variable value.
        /// </summary>
        public const uint DataType = 14;

        /// <summary>
        /// The number of dimensions in the value.
        /// </summary>
        public const uint ValueRank = 15;

        /// <summary>
        /// The length for each dimension of an array value.
        /// </summary>
        public const uint ArrayDimensions = 16;

        /// <summary>
        /// How a variable may be accessed.
        /// </summary>
        public const uint AccessLevel = 17;

        /// <summary>
        /// How a variable may be accessed after taking the user's access rights into account.
        /// </summary>
        public const uint UserAccessLevel = 18;

        /// <summary>
        /// Specifies (in milliseconds) how fast the server can reasonably sample the value for changes.
        /// </summary>
        public const uint MinimumSamplingInterval = 19;

        /// <summary>
        /// Specifies whether the server is actively collecting historical data for the variable.
        /// </summary>
        public const uint Historizing = 20;

        /// <summary>
        /// Whether the method can be called.
        /// </summary>
        public const uint Executable = 21;

        /// <summary>
        /// Whether the method can be called by the current user.
        /// </summary>
        public const uint UserExecutable = 22;
    }
}
