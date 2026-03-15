#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;
using System.Globalization;
using System.Xml;

namespace Syncfusion.Pdf.Xmp
{
    /// <summary>
    /// Base class for the arrays.
    /// </summary>
    public abstract class XmpCollection : XmpType
    {
        #region Constants
        /// <summary>
        /// Name of the array item tag.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected const string c_itemName = "li";
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="XmpCollection"/> class.
        /// </summary>
        /// <param name="xmp">Parent XmpMetadata.</param>
        /// <param name="parent">Parent xml node.</param>
        /// <param name="prefix">Namespace prefix.</param>
        /// <param name="localName">Name of the tag.</param>
        /// <param name="namespaceURI">Namespace URI.</param>
        internal XmpCollection(XmpMetadata xmp, XmlNode parent, string prefix, string localName, string namespaceURI)
            : base(xmp, parent, prefix, localName, namespaceURI)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets count of the items in the collection.
        /// </summary>
        public int Count
        {
            get
            {
                return GetItemsCount();
            }
        }

        /// <summary>
        /// Gets type of the collection.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected abstract XmpArrayType ArrayType { get; }

        /// <summary>
        /// Gets items container element.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected XmlElement ItemsContainer
        {
            get
            {
                string xpath = "./" + XmpMetadata.c_rdfPrefix + ":" + GetArrayName();
                XmlNode node = XmlData.SelectSingleNode(xpath, Xmp.NamespaceManager);
                if (node == null)
                {
                    throw new ArgumentNullException("node");
                }

                return node as XmlElement;
            }
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Creates entity in the parent.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void CreateEntity()
        {
            // NOTE: We can't create array until we know it's type.
            if (ArrayType != XmpArrayType.Unknown)
            {
                // Create base element.
                base.CreateEntity();

                // Depending of the array type - create the inner element.
                string arrayName = GetArrayName();
                XmlElement container = Xmp.CreateElement(XmpMetadata.c_rdfPrefix, arrayName, XmpMetadata.c_rdfUri);

                XmlData.AppendChild(container);
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Gets list of the array items.
        /// </summary>
        /// <returns>List of the array items.</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected XmlNodeList GetArrayItems()
        {
            string xpath = "./" + XmpMetadata.c_rdfPrefix + ":" + c_itemName;
            XmlNodeList nodes = ItemsContainer.SelectNodes(xpath, Xmp.NamespaceManager);

            return nodes;
        }

        /// <summary>
        /// Gets prefix of the array depending on the array type.
        /// </summary>
        /// <returns>prefix</returns>
        private string GetArrayName()
        {
            string prefix = XmpArrayType.Bag.ToString();
            if (XmlData.InnerXml.Contains("rdf:Seq"))
                prefix = XmpArrayType.Seq.ToString();
            else if (XmlData.InnerXml.Contains("rdf:Alt"))
                prefix = XmpArrayType.Alt.ToString();

            return prefix;
        }

        /// <summary>
        /// Gets count of the items in the collection.
        /// </summary>
        /// <returns>Count of the items in the collection.</returns>
        private int GetItemsCount()
        {
            XmlNodeList nodes = GetArrayItems();

            return nodes.Count;
        }
        #endregion
    }
}
