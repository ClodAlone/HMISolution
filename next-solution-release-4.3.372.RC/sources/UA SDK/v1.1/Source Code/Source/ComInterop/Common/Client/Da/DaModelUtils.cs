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
using System.Text;
using Opc.Ua;
using Opc.Ua.Server;
using Opc.Ua.Com;

namespace Opc.Ua.Com.Client
{
    /// <summary>
    /// A class that builds NodeIds used by the DataAccess NodeManager
    /// </summary>
    public static class DaModelUtils
    {
        /// <summary>
        /// The RootType for a DA Branch or Item.
        /// </summary>
        public const int DaElement = 0;

        /// <summary>
        /// The RootType for a DA Property identified by its property id.
        /// </summary>
        public const int DaProperty = 2;

        /// <summary>
        /// The RootType for a node defined by the UA server.
        /// </summary>
        public const int InternalNode = 3;

        /// <summary>
        /// Constructs a NodeId from the BrowseName of an internal node.
        /// </summary>
        /// <param name="browseName">The browse name.</param>
        /// <param name="namespaceIndex">Index of the namespace.</param>
        /// <returns>The node id.</returns>
        public static NodeId ConstructIdForInternalNode(QualifiedName browseName, ushort namespaceIndex)
        {
            ParsedNodeId parsedNodeId = new ParsedNodeId();

            parsedNodeId.RootId = browseName.Name;
            parsedNodeId.NamespaceIndex = namespaceIndex;
            parsedNodeId.RootType = InternalNode;

            return parsedNodeId.Construct();
        }

        /// <summary>
        /// Constructs a NodeId from the ItemId for a DA branch.
        /// </summary>
        /// <param name="itemId">The item id.</param>
        /// <param name="propertyId">The property id.</param>
        /// <param name="namespaceIndex">Index of the namespace.</param>
        /// <returns>The node id.</returns>
        public static NodeId ConstructIdForDaElement(string itemId, int propertyId, ushort namespaceIndex)
        {
            DaParsedNodeId parsedNodeId = new DaParsedNodeId();

            parsedNodeId.RootId = itemId;
            parsedNodeId.NamespaceIndex = namespaceIndex;
            parsedNodeId.RootType = DaElement;

            if (propertyId >= 0)
            {
                parsedNodeId.PropertyId = propertyId;
                parsedNodeId.RootType = DaProperty;
            }

            return parsedNodeId.Construct();
        }

        /// <summary>
        /// Constructs the node identifier for a component.
        /// </summary>
        /// <param name="component">The component.</param>
        /// <param name="namespaceIndex">Index of the namespace.</param>
        /// <returns>The node identifier for a component.</returns>
        public static NodeId ConstructIdForComponent(NodeState component, ushort namespaceIndex)
        {
            return ParsedNodeId.ConstructIdForComponent(component, namespaceIndex);
        }

        /// <summary>
        /// Constructs a branch or item node from a DaElement returned from the COM server.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="element">The element.</param>
        /// <param name="namespaceIndex">Index of the namespace for the NodeId.</param>
        /// <returns>The node.</returns>
        public static NodeState ConstructElement(ISystemContext context, DaElement element, ushort namespaceIndex)
        {
            if (element.ElementType == DaElementType.Branch)
            {
                return new DaBranchState(context, element, namespaceIndex);
            }

            return new DaItemState(context, element, namespaceIndex);
        }

        /// <summary>
        /// Constructs a property node for a DA property.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="parentId">The parent id.</param>
        /// <param name="property">The property.</param>
        /// <param name="namespaceIndex">Index of the namespace.</param>
        /// <returns>The property node.</returns>
        public static PropertyState ConstructProperty(ISystemContext context, string parentId, DaProperty property, ushort namespaceIndex)
        {
            return new DaPropertyState(context, parentId, property, namespaceIndex);
        }
    }
}
