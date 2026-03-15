/* ========================================================================
 * Copyright (c) 2005-2010 The OPC Foundation, Inc. All rights reserved.
 *
 * OPC Foundation MIT License 1.00
 * 
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following
 * conditions:
 * 
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 *
 * The complete license agreement can be found here:
 * http://opcfoundation.org/License/MIT/1.00/
 * ======================================================================*/

using System;
using System.Collections.Generic;
using System.Text;
using Opc.Ua;
using Opc.Ua.Server;

namespace UFUAServerBase
{
    /// <summary>
    /// A class that builds NodeIds used by the DataAccess NodeManager
    /// </summary>
    public static class ModelUtils
    {
        //// <summary>
        /// Constructs the name identifier for a component.
        /// </summary>
        /// <param name="component">The component.</param>
        /// <returns>The name identifier for a component.</returns>
        public static String ConstructNameForComponent(NodeState component)
        {
            return ConstructName(component);
        }

        //// <summary>
        /// Constructs the name identifier for a component.
        /// </summary>
        /// <param name="component">The component.</param>
        /// <param name="separator">The separator to use for identify parent components.</param>
        /// <returns>The name identifier for a component.</returns>
        public static String ConstructNameForComponent(NodeState component, char separator)
        {
            return ConstructName(component, separator);
        }

        //// <summary>
        /// Constructs the name identifier for a component.
        /// </summary>
        /// <param name="component">The component.</param>
        /// <param name="nodeid">The NodeId where stopping the parent component search.</param>
        /// <returns>The name identifier for a component.</returns>
        public static String ConstructNameForComponent(NodeState component, NodeId nodeid)
        {
            return ConstructName(component, '.', nodeid);
        }

        //// <summary>
        /// Constructs the name identifier for a component.
        /// </summary>
        /// <param name="component">The component.</param>
        /// <param name="separator">The separator to use for identifying parent components.</param>
        /// <param name="nodeid">The NodeId where stopping the parent component search.</param>
        /// <returns>The name identifier for a component.</returns>
        public static String ConstructNameForComponent(NodeState component, char separator, NodeId nodeid)
        {
            return ConstructName(component, separator, nodeid);
        }

        private static String ConstructName(NodeState component, char separator = '.', NodeId nodeid = null)
        {
            if (component == null)
            {
                return String.Empty;
            }

            BaseInstanceState instance = component as BaseInstanceState;

            if (instance == null || instance.Parent == null)
            {
                return component.DisplayName.Text;
            }

            String ret = component.DisplayName.Text;
            while(instance.Parent != null)
            {
                if (nodeid != null && instance.Parent.NodeId == nodeid)
                    break;
                ret = String.Format("{0}{1}{2}", instance.Parent.DisplayName.Text, separator, ret);
                instance = instance.Parent as BaseInstanceState;
            }

            return ret;
        }

        //// <summary>
        /// Constructs the description for a component.
        /// </summary>
        /// <param name="component">The component.</param>
        /// <returns>The description for a component.</returns>
        public static String ConstructDescriptionForComponent(NodeState component)
        {
            return ConstructDescription(component);
        }

        //// <summary>
        /// Constructs the description for a component.
        /// </summary>
        /// <param name="component">The component.</param>
        /// <param name="separator">The separator to use for identify parent components.</param>
        /// <returns>The description for a component.</returns>
        public static String ConstructDescriptionForComponent(NodeState component, char separator)
        {
            return ConstructDescription(component, separator);
        }

        /// <summary>
        /// Constructs the description for a component. If the component has parents also the parents descriptions are considered.
        /// </summary>
        /// <param name="component">The component.</param>
        /// <param name="separator">The separator to use for identify parent components.</param>
        /// <returns>The description for a component.</returns>
        private static String ConstructDescription(NodeState component, char separator = '.')
        {
            if (component == null)
            {
                return String.Empty;
            }

            BaseInstanceState instance = component as BaseInstanceState;
            
            if (instance == null || instance.Parent == null)
                return component.Description.Text;
            String ret = component.Description.Text;

            while (instance.Parent != null)
            {
                if (instance.Parent.Description != null)
                    ret = String.Format("{0}{1}{2}", instance.Parent.Description.Text, separator, ret);
                instance = instance.Parent as BaseInstanceState;
            }

            return ret;
        }

        /// <summary>
        /// Constructs the node identifier for a component.
        /// </summary>
        /// <param name="component">The component.</param>
        /// <param name="namespaceIndex">Index of the namespace.</param>
        /// <returns>The node identifier for a component.</returns>
        public static NodeId ConstructIdForComponent(NodeState component, ushort namespaceIndex)
        {
            if (component == null)
            {
                return null;
            }

            // components must be instances with a parent.
            BaseInstanceState instance = component as BaseInstanceState;

            if (instance == null || instance.Parent == null)
            {
                return component.NodeId;
            }

            // parent must have a string identifier.
            string parentId = instance.Parent.NodeId.Identifier as string;
            
            if (parentId == null)
            {
                parentId = instance.Parent.NodeId.Identifier.ToString();
            }

            StringBuilder buffer = new StringBuilder();
            buffer.Append(parentId);
            
            // check if the parent is another component.
            int index = parentId.IndexOf('?');

            if (index < 0)
            {
                buffer.Append('?');
            }
            else
            {
                buffer.Append('/');
            }


            if (!NodeId.IsNull(component.NodeId) && component.NodeId.IdType == IdType.Guid)
                buffer.Append((Guid)component.NodeId.Identifier);
            else
                buffer.Append(component.SymbolicName);

            // return the node identifier.
            return new NodeId(buffer.ToString(), namespaceIndex);
        }
    }
}
