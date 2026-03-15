#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Text;
using System.Xml;
using System.Collections;
using System.Globalization;
using Syncfusion.Layouting;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Xml.Schema;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using System.Net;
#if WINRT
using Syncfusion.DocIO.DLS;
using Font = Syncfusion.DocIO.DLS.Font;
#else
using System.Drawing;
using Font = System.Drawing.Font;
#endif
using Syncfusion.DocIO.DLS.Convertors;
namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// 
    /// </summary>
    internal class XDocument
    {
        XNode m_rootNode;
        public XNode RootNode
        {
            get
            {
                return m_rootNode;
            }
            set
            {
                m_rootNode = value;
            }
        }
        /// <summary>
        /// Load Xml Document
        /// </summary>
        /// <param name="reader"></param>
        internal void LoadXml(XmlReader reader)
        {

            while (reader.NodeType != XmlNodeType.Element)
                reader.Read();
            //Create root node
            m_rootNode = new XNode();
            m_rootNode.LocalName = reader.LocalName;
            //Loop upto next elemnt
            reader.Read();
            while (reader.LocalName != m_rootNode.LocalName)
            {
                if (reader.NodeType == XmlNodeType.Element)
                    AddNode(reader, m_rootNode, reader.NodeType);
                else
                {
                    reader.Read();
                }
                SkipWhitespaces(reader);
            }
        }
        /// <summary>
        /// Skip whitespaces and moves the reader to the next node.
        /// </summary>
        /// <param name="reader">The xml reader</param>
        private void SkipWhitespaces(XmlReader reader)
        {
            if (reader.NodeType == XmlNodeType.Element)
                return;

            while (reader.NodeType == XmlNodeType.Whitespace)
                reader.Read();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="parent"></param>
        /// <param name="nodeType"></param>
        private void AddNode(XmlReader reader, XNode parent, XmlNodeType nodeType)
        {
            XNode node = new XNode();
            node.Name = reader.LocalName;
            node.ParentNode = parent;
            node.NodeType = nodeType;
            node.InnerText = reader.Value;
            if (parent != null)
                parent.ChildNodes.Add(node);
            if (reader.AttributeCount > 0)
            {
                for (int i = 0; i < reader.AttributeCount; i++)
                {
                    XAttribute att = new XAttribute();
                    reader.MoveToAttribute(i);
                    att.Name = reader.LocalName;
                    att.Value = reader.Value;
                    node.Attributes.Add(att);
                }
                reader.MoveToElement();
            }
            
            node.InnerText = reader.Value;
            if (reader.IsEmptyElement)
                return;
            if (reader.NodeType == XmlNodeType.Element)
            {
                string endNode = reader.LocalName;
                reader.Read();
                while (reader.LocalName != endNode)
                {
                    AddNode(reader, node, reader.NodeType);
                    reader.Read();
                }
            }
        }

    }

    internal class XNode
    {
        #region Private members
        XNode m_parentNode;
        string m_name;
        string m_value;
        List<XNode> m_childNodes = new List<XNode>();
        List<XAttribute> m_attributes = new List<XAttribute>();
        XmlNodeType m_nodeType;
        #endregion
        /// <summary>
        /// Specifies the node type
        /// </summary>
        internal XmlNodeType NodeType
        {
            get
            {
                return m_nodeType;
            }
            set
            {
                m_nodeType = value;
            }
        }
        /// <summary>
        /// Instance specifies the node name
        /// </summary>
        internal string Name
        {
            get
            {
                return m_name;
            }
            set
            {
                m_name = value;
            }
        }
        /// <summary>
        /// Instance specifies the local name of the node
        /// </summary>
        internal string LocalName
        {
            get
            {
                return m_name;
            }
            set
            {
                m_name = value;
            }
        }
        /// <summary>
        /// Instance specifies the node value
        /// </summary>
        internal string Value
        {
            get
            {
                return m_value;
            }
            set
            {
                m_value = value;
            }
        }
        /// <summary>
        /// Instance specifies the inner text of the node
        /// </summary>
        internal string InnerText
        {
            get
            {
                return m_value;
            }
            set
            {
                m_value = value;
            }
        }
        /// <summary>
        /// Instance specifies the parent node
        /// </summary>
        internal XNode ParentNode
        {
            get
            {
                return m_parentNode;
            }
            set
            {
                m_parentNode = value;
            }
        }
        /// <summary>
        /// Instance specifies the nextsibling node
        /// </summary>
        internal XNode NextSibling
        {
            get
            {
                int currentIndex = m_parentNode.ChildNodes.IndexOf(this);

                if (currentIndex == m_parentNode.ChildNodes.Count - 1)
                    return null;

                return m_parentNode.ChildNodes[currentIndex + 1];
            }

        }
        /// <summary>
        /// Instance specifies the previoussibling node 
        /// </summary>
        internal XNode PreviousSibling
        {
            get
            {
                int currentIndex = m_parentNode.ChildNodes.IndexOf(this);

                if (currentIndex == 0)
                    return null;

                return m_parentNode.ChildNodes[currentIndex - 1];
            }

        }
        /// <summary>
        /// Instance specifies the childnodes
        /// </summary>
        internal List<XNode> ChildNodes
        {
            get
            {
                return m_childNodes;
            }
            set
            {
                m_childNodes = value;
            }
        }
        /// <summary>
        /// Instance specifies the attributes of the node
        /// </summary>
        internal List<XAttribute> Attributes
        {
            get
            {
                return m_attributes;
            }
            set
            {
                m_attributes = value;
            }
        }

    }
    /// <summary>
    /// 
    /// </summary>
    internal class XAttribute
    {
        #region Private members
        string m_name;
        string m_value;
        #endregion
        /// <summary>
        /// Instance specifies the local name of the attribute
        /// </summary>
        internal string LocalName
        {
            get
            {
                return m_name;
            }
            set
            {
                m_name = value;
            }
        }
        /// <summary>
        /// Instance specifies the name of the attribute
        /// </summary>
        internal string Name
        {
            get
            {
                return m_name;
            }
            set
            {
                m_name = value;
            }
        }
        /// <summary>
        /// Instance specifies the value of the attribute
        /// </summary>
        internal string Value
        {
            get
            {
                return m_value;
            }
            set
            {
                m_value = value;
            }
        }
    }
}
