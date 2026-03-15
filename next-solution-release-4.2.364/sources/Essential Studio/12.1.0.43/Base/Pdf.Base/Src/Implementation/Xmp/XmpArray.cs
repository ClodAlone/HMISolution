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
using System.Xml;

namespace Syncfusion.Pdf.Xmp
{
    /// <summary>
    /// Represents Xmp array.
    /// </summary>
    public class XmpArray : XmpCollection
    {
        #region Constants
        /// <summary>
        /// Default date format.
        /// </summary>
        internal const string c_dateFormat = "yyyy-MM-dd'T'HH:mm:ss.ffzzz";
        #endregion

        #region Fields
        /// <summary>
        /// Type of the array.
        /// </summary>
        private XmpArrayType m_arrayType = XmpArrayType.Unknown;
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="XmpArray"/> class.
        /// </summary>
        /// <param name="xmp">The XMP.</param>
        /// <param name="parent">The parent.</param>
        /// <param name="prefix">The prefix.</param>
        /// <param name="localName">Name of the local.</param>
        /// <param name="namespaceURI">The namespace URI.</param>
        /// <param name="type">The type.</param>
        internal XmpArray(XmpMetadata xmp, XmlNode parent, string prefix, string localName, string namespaceURI, XmpArrayType type)
            : base(xmp, parent, prefix, localName, namespaceURI)
        {
            m_arrayType = type;

            Initialize();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets array of the array values.
        /// </summary>
        public string[] Items
        {
            get
            {
                string[] items = GetArrayValues();

                return items;
            }
        }

        /// <summary>
        /// Gets type of the array.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override XmpArrayType ArrayType
        {
            get
            {
                return m_arrayType;
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Adds string value to the array.
        /// </summary>
        /// <param name="value">Value to be added to the array.</param>
        public void Add(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            XmlElement item = CreateItem();
            XmpUtils.SetTextValue(item, value);
        }

        /// <summary>
        /// Adds integer value to the array.
        /// </summary>
        /// <param name="value">Value to be added to the array.</param>
        public void Add(int value)
        {
            XmlElement item = CreateItem();
            XmpUtils.SetIntValue(item, value);
        }

        /// <summary>
        /// Adds float value to the array.
        /// </summary>
        /// <param name="value">Value to be added to the array.</param>
        public void Add(float value)
        {
            XmlElement item = CreateItem();
            XmpUtils.SetRealValue(item, value);
        }

        /// <summary>
        /// Adds Date to the array.
        /// </summary>
        /// <param name="value">Value to be added to the array.</param>
        public void Add(DateTime value)
        {
            string date = value.ToString(c_dateFormat);

            Add(date);
        }

        /// <summary>
        /// Adds Date to the array.
        /// </summary>
        /// <param name="value">Value to be added to the array.</param>
        /// <param name="format">String format of the date.</param>
        public void Add(DateTime value, string format)
        {
            if (format == null)
            {
                throw new ArgumentNullException("format");
            }

            string date = value.ToString(format);

            Add(date);
        }

        /// <summary>
        /// Adds an XMP structure.
        /// </summary>
        /// <param name="structure">The structure.</param>
        public void Add(XmpStructure structure)
        {
            if (structure == null)
            {
                throw new ArgumentNullException("structure");
            }

            XmlElement item = CreateItem();
            XmpUtils.SetXmlValue(item, structure.XmlData);
            ChangeParent(item, structure);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Creates array item element.
        /// </summary>
        /// <returns>XmlElement of the array item.</returns>
        private XmlElement CreateItem()
        {
            XmlElement item = Xmp.CreateElement(XmpMetadata.c_rdfPrefix, c_itemName, XmpMetadata.c_rdfUri);

            ItemsContainer.AppendChild(item);

            return item;
        }

        /// <summary>
        /// Returns array of the array values.
        /// </summary>
        /// <returns>Array of the array values.</returns>
        private string[] GetArrayValues()
        {
            string[] values = new string[1];
            if (XmlData.InnerXml.Contains("rdf"))
            {
                XmlNodeList nodes = GetArrayItems();
                if (nodes.Count == 0)
                    values[0] = string.Empty;
                else
                {
                    values = new string[nodes.Count];
                    for (int i = 0, len = nodes.Count; i < len; i++)
                    {
                        XmlNode node = nodes[i];
                        values[i] = node.InnerXml;
                    }
                }
            }
            else
                values[0] = XmlData.InnerText;
            return values;
        }

        /// <summary>
        /// Changes parent of the entity.
        /// </summary>
        /// <param name="parent">New entity parent.</param>
        /// <param name="entity">Xmp entity.</param>
        private void ChangeParent(XmlNode parent, XmpEntityBase entity)
        {
            if (parent == null)
            {
                throw new ArgumentNullException("parent");
            }

            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            entity.SetXmlParent(parent);
        }
        #endregion
    }
}
