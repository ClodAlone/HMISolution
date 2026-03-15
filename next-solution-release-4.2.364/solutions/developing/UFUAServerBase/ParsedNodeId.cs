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
using System.Text;
using Opc.Ua;

namespace UFUAServerBase
{
    /// <summary>
    /// Stores the elements of a NodeId after it is parsed.
    /// </summary>
    /// <remarks>
    /// The NodeIds used by the samples are strings with an optional path appended.
    /// The RootType identifies the type of Root Node. The RootId is the unique identifier
    /// for the Root Node. The ComponentPath is constructed from the SymbolicNames
    /// of one or more children of the Root Node. 
    /// </remarks>
    public class ParsedNodeId
    {
        #region Public Interface
        /// <summary>
        /// The namespace index that qualified the NodeId.
        /// </summary>
        public ushort NamespaceIndex
        {
            get { return m_namespaceIndex; }
            set { m_namespaceIndex = value; }
        }

        /// <summary>
        /// The identifier for the root of the NodeId.
        /// </summary>
        public NodeId RootId
        {
            get { return m_rootId; }
            set { m_rootId = value; }
        }

        /// <summary>
        /// The relative path to the component identified by the NodeId.
        /// </summary>
        public string ComponentPath
        {
            get { return m_componentPath; }
            set { m_componentPath = value; }
        }

        /// <summary>
        /// Parses the specified node identifier.
        /// </summary>
        /// <param name="nodeId">The node identifier.</param>
        /// <returns>The parsed node identifier. Null if the identifier cannot be parsed.</returns>
        public static ParsedNodeId Parse(NodeId nodeId)
        {
            // can only parse non-null string node identifiers.
            if (NodeId.IsNull(nodeId))
            {
                return null;
            }

            string identifier = nodeId.Identifier as string;

            if (String.IsNullOrEmpty(identifier))
            {
                identifier = nodeId.Identifier.ToString();
            }

            ParsedNodeId parsedNodeId = new ParsedNodeId { NamespaceIndex = nodeId.NamespaceIndex };

            // extract any component path.
            StringBuilder buffer = new StringBuilder();

            int index = 0;
            int end = identifier.Length;

            bool escaped = false;

            while (index < end)
            {
                char ch = identifier[index++];

                // skip any escape character but keep the one after it.
                if (ch == '&')
                {
                    escaped = true;
                    continue;
                }

                if (!escaped && ch == '?')
                {
                    end = index;
                    break;
                }

                buffer.Append(ch);
                escaped = false;
            }

            // extract any component.
            Guid guid;
            if (Guid.TryParse(buffer.ToString(), out guid))
                parsedNodeId.RootId = new NodeId(guid, parsedNodeId.NamespaceIndex);
            else
                parsedNodeId.RootId = new NodeId(buffer.ToString(), parsedNodeId.NamespaceIndex);

            if (end < identifier.Length)
                parsedNodeId.ComponentPath = identifier.Substring(end);
            else
                parsedNodeId.ComponentPath = null;

            return parsedNodeId;
        }

        #endregion

        #region Private Fields
        private ushort m_namespaceIndex;
        private NodeId m_rootId;
        private string m_componentPath;
        #endregion
    }
}
