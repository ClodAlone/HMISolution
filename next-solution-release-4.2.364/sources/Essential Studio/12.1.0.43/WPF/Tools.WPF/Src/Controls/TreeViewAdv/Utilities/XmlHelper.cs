// <copyright file="XmlHelper.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

#region file using

using System;
using System.Text;
using System.Xml;

#endregion file using

namespace Syncfusion.Windows.Tools
{
    /// <summary>
    /// Represents the XmlHelper class
    /// </summary>
    internal static class XmlHelper
    {
        #region Constants

        /// <summary>
        /// Presents xml name
        /// </summary>
        private const string C_SystemXmlName = "System.Xml";

        #endregion Constants

        #region Implementation

        /// <summary>
        /// Extracts the string.
        /// </summary>
        /// <param name="node">The node Xml Node.</param>
        /// <returns>string type </returns>
        internal static string ExtractString(XmlNode node)
        {
            string resultText = string.Empty;

            if (node.NodeType == XmlNodeType.Element)
            {
                StringBuilder stringBuilder = new StringBuilder();
                XmlNodeList nodeList = node.ChildNodes;

                for (int i = 0, cnt = nodeList.Count; i < cnt; i++)
                {
                    if (nodeList[i].NodeType == XmlNodeType.Text)
                    {
                        stringBuilder.Append(nodeList[i].Value);
                    }
                }

                resultText = stringBuilder.ToString();
            }
            else
            {
                resultText = node.Value;
            }

            return resultText;
        }

        /// <summary>
        /// Determines whether [is XML node] [the specified item].
        /// </summary>
        /// <param name="item">The item Xml Node.</param>
        /// <returns>
        /// true if [is XML node] [the specified item]; otherwise, false.
        /// </returns>
        internal static bool IsXmlNode(object item)
        {
            bool result = false;

            if (item != null)
            {
                Type type = item.GetType();
                string fullName = type.FullName;

                if (fullName.StartsWith(C_SystemXmlName, StringComparison.Ordinal))
                {
                    result = item is XmlNode;
                }
            }

            return result;
        }

        /// <summary>
        /// Selects the string value.
        /// </summary>
        /// <param name="parentNode">The parent node.</param>
        /// <param name="query">The query.</param>
        /// <returns>string type </returns>
        internal static string SelectStringValue(XmlNode parentNode, string query)
        {
            return SelectStringValue(parentNode, query, null);
        }

        /// <summary>
        /// Selects the string value.
        /// </summary>
        /// <param name="parentNode">The parent node.</param>
        /// <param name="query">The query.</param>
        /// <param name="namespaceManager">The namespace manager.</param>
        /// <returns> string type </returns>
        internal static string SelectStringValue(XmlNode parentNode, string query, XmlNamespaceManager namespaceManager)
        {
            XmlNode node = parentNode.SelectSingleNode(query, namespaceManager);
            string resultString = string.Empty;

            if (node != null)
            {
                resultString = ExtractString(node);
            }

            return resultString;
        }

        #endregion Implementation
    }

    public static class TreeViewAdvCloneManager
    {
        public static T Clone<T>(T source)
        {
            if (source.GetType().Name != "BitmapFrameDecode")
            {
                T cloned = (T)Activator.CreateInstance(source.GetType());
                return cloned;
            }
            else
            {
                return default(T);
            }
        }
    }
}