#region Copyright Syncfusion Inc. 2001 - 2014

//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Xml;

namespace Syncfusion.Pdf.Xmp
{
    /// <summary>
    /// Represents Xmp Alt Lang array.
    /// </summary>
    public class XmpLangArray : XmpCollection
    {
        #region Constants
        /// <summary>
        /// Default language name.
        /// </summary>
        private const string c_langName = "x-default";

        /// <summary>
        /// Language attribute.
        /// </summary>
        private const string c_langAttribute = "lang";
        #endregion

        #region Constructors
        /// <summary>
        /// Creates new object.
        /// </summary>
        /// <param name="xmp">Parent XmpMetadata.</param>
        /// <param name="parent">Parent XmlElement of the array.</param>
        /// <param name="prefix">Prefix of the array.</param>
        /// <param name="localName">Name of the tag.</param>
        /// <param name="namespaceURI">Namespace URI.</param>
        internal XmpLangArray(XmpMetadata xmp, XmlNode parent, string prefix, string localName, string namespaceURI)
            : base(xmp, parent, prefix, localName, namespaceURI)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the default text.
        /// </summary>
        /// <value>The default text.</value>
        public string DefaultText
        {
            get
            {
                if (XmlData.InnerXml.Contains("rdf"))
                {
                    XmlElement def = this.GetItem(c_langName);
                    if (def == null)
                        def = this.CreateItem(c_langName);
                    return def.InnerXml;
                }
                else
                    return XmlData.InnerText;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("DefaultText");
                }

                XmlElement def = this.GetItem(c_langName);
                if (def == null)
                {
                    def = this.CreateItem(c_langName);
                }

                XmpUtils.SetTextValue(def, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="System.String"/> with the specified lang.
        /// </summary>
        /// <value>value</value>
        public string this[string lang]
        {
            get
            {
                string val = null;
                XmlElement elm = this.GetItem(lang);

                if (elm != null)
                {
                    val = elm.InnerXml;
                }

                return val;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                XmlElement elm = this.GetItem(lang);

                if (elm == null)
                {
                    elm = this.CreateItem(lang);
                }

                XmpUtils.SetTextValue(elm, value);
            }
        }

        /// <summary>
        /// Gets type of the lang array.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override XmpArrayType ArrayType
        {
            get
            {
                return XmpArrayType.Alt;
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Adds item to the array.
        /// </summary>
        /// <param name="lang">Language code.</param>
        /// <param name="value">Text value.</param>
        public void Add(string lang, string value)
        {
            if (lang == null)
            {
                throw new ArgumentNullException("lang");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            XmlElement item = this.CreateItem(lang);
            XmpUtils.SetTextValue(item, value);
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Creates entity in the parent.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void CreateEntity()
        {
            base.CreateEntity();

            // Add default language item.
            this.CreateItem(c_langName);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Creates array item element.
        /// </summary>
        /// <param name="lang">Language code.</param>
        /// <returns>XmlElement of the array item.</returns>
        private XmlElement CreateItem(string lang)
        {
            if (lang == null)
            {
                throw new ArgumentNullException(c_langAttribute);
            }

            XmlElement item = Xmp.CreateElement(XmpMetadata.c_rdfPrefix, c_itemName, XmpMetadata.c_rdfUri);
            ItemsContainer.AppendChild(item);

            // Add language attribute.
            XmlAttribute attr = Xmp.CreateAttribute(XmpMetadata.c_xmlPefix, c_langAttribute, XmpMetadata.c_xmlUri, lang);
            item.Attributes.Append(attr);

            return item;
        }

        /// <summary>
        /// Searches for a item with the specified language.
        /// </summary>
        /// <param name="lang">Language name.</param>
        /// <returns>Item with the specified language.</returns>
        private XmlElement GetItem(string lang)
        {
            if (lang == null)
            {
                throw new ArgumentNullException("lang");
            }

            string xpath = "./" + XmpMetadata.c_rdfPrefix + ":" +
                c_itemName + "[@xml:" + c_langAttribute + "=\"" + lang + "\"]";

            XmlNode node = ItemsContainer.SelectSingleNode(xpath, Xmp.NamespaceManager);

            return (node as XmlElement);
        }
        #endregion
    }
}
