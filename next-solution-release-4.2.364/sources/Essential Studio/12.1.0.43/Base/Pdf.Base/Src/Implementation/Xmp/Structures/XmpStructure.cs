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
using System.Collections;
using System.Xml;

namespace Syncfusion.Pdf.Xmp
{
    /// <summary>
    /// Represents Xmp Structure.
    /// </summary>
    public abstract class XmpStructure : XmpType
    {
        #region Fields
        /// <summary>
        /// Hashtable of the properties.
        /// </summary>
        private Hashtable m_properties;
        /// <summary>
        /// Indicates whether structure is inside of the array or not.
        /// </summary>
        private bool m_bInsideArray;
        /// <summary>
        /// Indicates whether we have to suspend initialization.
        /// </summary>
        private bool m_bSuspend = true;
        /// <summary>
        /// Indicate swhether structure is initialized.
        /// </summary>
        private bool m_bInitialized;
        #endregion

        #region Properties
        /// <summary>
        /// Gets inner xml data.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected internal XmlElement InnerXmlData
        {
            get
            {
                XmlElement elm = null;

                if (m_bInsideArray)
                {
                    elm = XmlData;
                }
                else
                {
                    elm = GetDescriptionElement();
                }

                if (elm == null)
                    throw new ArgumentNullException("elm");

                return elm;
            }
        }
        /// <summary>
        /// Gets prefix of the structure.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected abstract string StructurePrefix { get; }
        /// <summary>
        /// Gets name pf the structure.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected abstract string StructureURI { get; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates xmp simple type instance.
        /// </summary>
        /// <param name="xmp">Parent XmpMetadata.</param>
        /// <param name="parent">Parent xml node.</param>
        /// <param name="prefix">Namespace prefix.</param>
        /// <param name="localName">Name of the tag.</param>
        /// <param name="namespaceURI">Namespace URI.</param>
        internal XmpStructure(XmpMetadata xmp, XmlNode parent, string prefix, string localName, string namespaceURI)
            : this(xmp, parent, prefix, localName, namespaceURI, false)
        {
        }
        /// <summary>
        /// Creates xmp simple type instance.
        /// </summary>
        /// <param name="xmp">Parent XmpMetadata.</param>
        /// <param name="parent">Parent xml node.</param>
        /// <param name="prefix">Namespace prefix.</param>
        /// <param name="localName">Name of the tag.</param>
        /// <param name="namespaceURI">Namespace URI.</param>
        /// <param name="insideArray">if it is inside an array, set to <c>true</c>.</param>
        internal XmpStructure(XmpMetadata xmp, XmlNode parent, string prefix, string localName, string namespaceURI, bool insideArray)
            : base(xmp, parent, prefix, localName, namespaceURI)
        {
            m_bInsideArray = insideArray;
            m_bSuspend = false;
            m_properties = new Hashtable();

            Initialize();
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Creates structure.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void CreateEntity()
        {
            if (m_properties != null)
            {
                Xmp.AddNamespace(StructurePrefix, StructureURI);

                // Create outer tag of the property.
                if (!m_bInsideArray)
                {
                    base.CreateEntity();
                }

                CreateStructureContent();

                // Initialize entities.
                InitializeEntities();
                m_bInitialized = true;
            }
        }
        /// <summary>
        /// Gets Xml data of the entity.
        /// </summary>
        /// <returns>XmlElement containing entity data.</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override XmlElement GetEntityXml()
        {
            XmlElement elm = null;

            if (!m_bInsideArray)
            {
                elm = base.GetEntityXml();
            }
            else
            {
                elm = GetDescriptionElement();
            }

            if (elm == null)
                throw new ArgumentNullException("elm");

            return elm;
        }
        /// <summary>
        /// Gets value indicating whether we have to suspend initialization.
        /// </summary>
        /// <returns>Value indicating whether we have to suspend initialization.</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override bool GetSuspend()
        {
            return m_bSuspend;
        }
        /// <summary>
        /// Checks whether entity already exists in the parent.
        /// </summary>
        /// <returns>True - if exists, False otherwise.</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override bool CheckIfExists()
        {
            bool result = false;

            if (m_bInitialized)
            {
                result = base.CheckIfExists();
            }

            return result;
        }
        /// <summary>
        /// Initializes internal entries.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected abstract void InitializeEntities();
        #endregion

        #region Properties methods
        /// <summary>
        /// Creates simple property.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <returns>Simple property instance.</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected XmpSimpleType CreateSimpleProperty(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            XmpSimpleType instance = CreateSimpleProperty(name, InnerXmlData);

            return instance;
        }
        /// <summary>
        /// Gets property by its name.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <returns>Xmp property instance.</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected XmpSimpleType GetSimpleProperty(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            XmpSimpleType obj = m_properties[name] as XmpSimpleType;

            if (obj == null)
            {
                obj = CreateSimpleProperty(name);
                m_properties[name] = obj;
            }

            return obj;
        }
        /// <summary>
        /// Creates simple property.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <param name="parent">Parent XmlNode.</param>
        /// <returns>Simple property instance.</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected XmpSimpleType CreateSimpleProperty(string name, XmlNode parent)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (parent == null)
                throw new ArgumentNullException("parent");

            XmpSimpleType instance = new XmpSimpleType(Xmp, parent, StructurePrefix, name, StructureURI);

            return instance;
        }
        /// <summary>
        /// Gets property by its name.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <param name="parent">Parent XmlNode.</param>
        /// <returns>Xmp property instance.</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected XmpSimpleType GetSimpleProperty(string name, XmlNode parent)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (parent == null)
                throw new ArgumentNullException("parent");

            XmpSimpleType obj = m_properties[name] as XmpSimpleType;

            if (obj == null)
            {
                obj = CreateSimpleProperty(name, parent);
                m_properties[name] = obj;
            }

            return obj;
        }
        /// <summary>
        /// Creates xmp array.
        /// </summary>
        /// <param name="name">Name of the array.</param>
        /// <param name="arrayType">Type of the array.</param>
        /// <returns>Created xmp array.</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected XmpArray CreateArray(string name, XmpArrayType arrayType)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (arrayType == XmpArrayType.Unknown)
                throw new ArgumentException("Wrong array type", "arrayType");

            XmpArray array = new XmpArray(Xmp, InnerXmlData, StructurePrefix, name, StructureURI, arrayType);

            return array;
        }
        /// <summary>
        /// Gets xmp array.
        /// </summary>
        /// <param name="name">Name of the array.</param>
        /// <param name="arrayType">Type of the array.</param>
        /// <returns>Xmp array.</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected XmpArray GetArray(string name, XmpArrayType arrayType)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            XmpArray obj = m_properties[name] as XmpArray;

            if (obj == null)
            {
                obj = CreateArray(name, arrayType);
                m_properties[name] = obj;
            }

            return obj;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Creates structure inner content.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected void CreateStructureContent()
        {
            XmlNode parent = GetContentParent();

            XmlElement description = Xmp.CreateElement(XmpMetadata.c_rdfPrefix, XmpSchema.c_schemaTagName, XmpMetadata.c_rdfUri);
            parent.AppendChild(description);

            XmlAttribute xmlns = Xmp.CreateAttribute("xmlns:" + StructurePrefix, StructureURI);
            description.Attributes.Append(xmlns);
        }
        /// <summary>
        /// Gets Xml element of the description tag.
        /// </summary>
        /// <returns>Xml data.</returns>
        private XmlElement GetDescriptionElement()
        {
            XmlNode parent = GetContentParent();
            string xpath = "./" + XmpMetadata.c_rdfPrefix + ":" + XmpSchema.c_schemaTagName;
            XmlNode node = parent.SelectSingleNode(xpath, Xmp.NamespaceManager);

            return (node as XmlElement);
        }
        /// <summary>
        /// Gets parent of the structure content.
        /// </summary>
        /// <returns>Parent of the structure content.</returns>
        private XmlNode GetContentParent()
        {
            XmlNode node = (m_bInsideArray) ? EntityParent : XmlData;

            return node;
        }
        #endregion
    }
}
