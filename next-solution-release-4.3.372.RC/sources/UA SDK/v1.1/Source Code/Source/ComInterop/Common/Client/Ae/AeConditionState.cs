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
using System.Xml;
using System.IO;
using System.Reflection;
using System.Threading;
using Opc.Ua;
using Opc.Ua.Server;
using OpcRcw.Ae;

namespace Opc.Ua.Com.Client
{        
    /// <summary>
    /// A object which represents a COM AE condition.
    /// </summary>
    public partial class AeConditionState : AlarmConditionState
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="AeConditionState"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="handle">The handle.</param>
        /// <param name="acknowledgeMethod">The acknowledge method.</param>
        public AeConditionState(ISystemContext context, NodeHandle handle, AddCommentMethodState acknowledgeMethod)
        :
            base(null)
        {
            AeParsedNodeId parsedNodeId = (AeParsedNodeId)handle.ParsedNodeId;

            this.NodeId = handle.NodeId;

            this.TypeDefinitionId = AeParsedNodeId.Construct(
                Constants.CONDITION_EVENT, 
                parsedNodeId.CategoryId, 
                parsedNodeId.ConditionName, 
                parsedNodeId.NamespaceIndex);

            this.Acknowledge = acknowledgeMethod;
            this.AddChild(acknowledgeMethod);
        }
        #endregion

        #region Public Properties
        #endregion

        #region Private Fields
        #endregion
    }
}
