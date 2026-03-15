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
    /// Represents Xmp Schema.
    /// </summary>
    public abstract class XmpSchema : XmpEntityBase
    {
        #region Constants
        /// <summary>
        /// Name of the schema tag.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        internal const string c_schemaTagName = "Description";

        /// <summary>
        /// Xpath to the Description element.
        /// </summary>
        private const string c_xPathDescription = XmpMetadata.c_xpathRdf + "/rdf:" + c_schemaTagName;
        #endregion

        #region Fields
        /// <summary>
        /// Parent XmpMetadata.
        /// </summary>
        private XmpMetadata m_xmp;

        /// <summary>
        /// Hashtable of the properties.
        /// </summary>
        private Hashtable m_properties;
        #endregion

        #region Properties
        /// <summary>
        /// Gets type of the schema.
        /// </summary>
        public abstract XmpSchemaType SchemaType { get; }

        /// <summary>
        /// Gets schema prefix.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected abstract string Prefix { get; }

        /// <summary>
        /// Gets name (URI) of the schema.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected abstract string Name { get; }

        /// <summary>
        /// Gets parent XmpMetadata.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected internal XmpMetadata Xmp
        {
            get
            {
                return m_xmp;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates new object.
        /// </summary>
        /// <param name="xmp">Parent XmpMetadata.</param>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected internal XmpSchema(XmpMetadata xmp)
            : base(xmp.Rdf, XmpMetadata.c_rdfPrefix, c_schemaTagName, XmpMetadata.c_rdfUri)
        {
            if (xmp == null)
                throw new ArgumentNullException("xmp");

            m_xmp = xmp;
            m_properties = new Hashtable();

            if (Prefix != null)
            {
                Initialize();
            }
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Creates schema xml.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void CreateEntity()
        {
            XmlElement description = Xmp.CreateElement(EntityPrefix, EntityName, EntityNamespaceURI);
            EntityParent.AppendChild(description);

            XmlAttribute about = Xmp.CreateAttribute(EntityPrefix, "about", EntityNamespaceURI, string.Empty);
            description.Attributes.Append(about);

            XmlAttribute xmlns = Xmp.CreateAttribute("xmlns:" + Prefix, Name);
            description.Attributes.Append(xmlns);

            Xmp.AddNamespace(Prefix, Name);
        }

        /// <summary>
        /// Gets Xml data of the entity.
        /// </summary>
        /// <returns>XmlElement containing entity data.</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override XmlElement GetEntityXml()
        {
            string xpath = "./" + EntityPrefix + ":" + EntityName;
            XmlNodeList list = EntityParent.SelectNodes(xpath, Xmp.NamespaceManager);
            XmlNode elm = null;

            for (int i = 0, len = list.Count; i < len; i++)
            {
                XmlNode node = list[i];
                XmlAttribute attr = node.Attributes[Prefix, XmpMetadata.c_xmlnsUri];

                if (attr != null && attr.Value.Equals(Name))
                {
                    elm = node;
                    break;
                }
            }
            return (elm as XmlElement);
        }
        #endregion

        #region Properties methods
        /// <summary>
        /// Creates simple property.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <returns>Simple property instance.</returns>
        protected XmpSimpleType CreateSimpleProperty(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            XmpSimpleType instance = new XmpSimpleType(Xmp, XmlData, Prefix, name, Name);

            return instance;
        }

        /// <summary>
        /// Gets property by its name.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <returns>Xmp property instance.</returns>
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
        /// Creates xmp array.
        /// </summary>
        /// <param name="name">Name of the array.</param>
        /// <param name="arrayType">Type of the array.</param>
        /// <returns>Created xmp array.</returns>
        protected XmpArray CreateArray(string name, XmpArrayType arrayType)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (arrayType == XmpArrayType.Unknown)
                throw new ArgumentException("Wrong array type", "arrayType");

            XmpArray array = new XmpArray(Xmp, XmlData, Prefix, name, Name, arrayType);

            return array;
        }

        /// <summary>
        /// Gets xmp array.
        /// </summary>
        /// <param name="name">Name of the array.</param>
        /// <param name="arrayType">Type of the array.</param>
        /// <returns>Xmp array.</returns>
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

        /// <summary>
        /// Creates xmp lang array.
        /// </summary>
        /// <param name="name">Name of the array.</param>
        /// <returns>Created xmp array.</returns>
        protected XmpLangArray CreateLangArray(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            XmpLangArray array = new XmpLangArray(Xmp, XmlData, Prefix, name, Name);

            return array;
        }

        /// <summary>
        /// Gets xmp lang array.
        /// </summary>
        /// <param name="name">Name of the array.</param>
        /// <returns>Xmp array.</returns>
        protected XmpLangArray GetLangArray(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            XmpLangArray obj = m_properties[name] as XmpLangArray;

            if (obj == null)
            {
                obj = CreateLangArray(name);
                m_properties[name] = obj;
            }

            return obj;
        }

        /// <summary>
        /// Gets the structure.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        protected XmpStructure GetStructure(string name, XmpStructureType type)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            XmpStructure obj = m_properties[name] as XmpStructure;

            if (obj == null)
            {
                obj = CreateStructure(name, type);
                m_properties[name] = obj;
            }

            return obj;
        }

        /// <summary>
        /// Creates the structure.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <returns>Structure object.</returns>
        protected XmpStructure CreateStructure(string name, XmpStructureType type)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            XmpStructure structure = null;
            bool bInsideArray = (name.Length == 0);

            switch (type)
            {
                case XmpStructureType.Dimensions:
                    structure = new XmpDimensionsStruct(Xmp, XmlData, Prefix, name, Name, bInsideArray);
                    break;

                case XmpStructureType.Font:
                    structure = new XmpFontStruct(Xmp, XmlData, Prefix, name, Name, bInsideArray);
                    break;

                case XmpStructureType.Colorant:
                    structure = new XmpColorantStruct(Xmp, XmlData, Prefix, name, Name, bInsideArray);
                    break;

                case XmpStructureType.Thumbnail:
                    structure = new XmpThumbnailStruct(Xmp, XmlData, Prefix, name, Name, bInsideArray);
                    break;

                case XmpStructureType.Job:
                    structure = new XmpJobStruct(Xmp, XmlData, Prefix, name, Name, bInsideArray);
                    break;
            }

            return structure;
        }

        /// <summary>
        /// Creates the structure.
        /// </summary>
        /// <param name="type">The type of the structure.</param>
        /// <returns>Structure object.</returns>
        public XmpStructure CreateStructure(XmpStructureType type)
        {
            XmpStructure structure = CreateStructure(string.Empty, type);

            return structure;
        }
        #endregion
    }
}
