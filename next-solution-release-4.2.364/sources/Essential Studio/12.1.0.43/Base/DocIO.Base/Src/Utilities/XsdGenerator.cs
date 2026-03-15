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

#if !SILVERLIGHT

#region file using directives
using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Xml;
using System.Xml.Schema;
using Syncfusion.Documentation;
#endregion

namespace Syncfusion.DocIO.DLS
{
    enum ModeType
    {
        Enum,
        Pattern,
        Space
    }

    /// <summary>
    /// Summary description for XSDSchemeGenerator.
    /// </summary>
    [DocumentationExclude()]
    public class XsdGenerator
    {
        #region Class constants
        protected const string DEF_DLS_RESOURCES = "Syncfusion.DocIO.DLS.Resources";

        /// <summary>
        /// 
        /// </summary>
        private const string DEF_SCHEME_NS = "http://www.w3.org/2001/XMLSchema";
        private const string DEF_META_NS = "http://tempuri.org/DLSMetaSchema.xsd";
        private readonly string[] DEF_STANDARD_TYPES = new string[]
      {
       "string", "float", "boolean", "int", "datetime", "base64Binary"
      };
        #endregion

        #region Class members
        private XmlSchema m_schema;
        private XmlNamespaceManager m_mngr;
        private XmlElement m_metaElement;
        #endregion

        #region Class public methods
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="reader"></param>
        ////public static void ValidateBySchema(XmlReader reader, ValidationEventHandler validationHandler)
        ////{
        ////    XmlValidatingReader valReader = new XmlValidatingReader(reader);

        ////    valReader.ValidationType = ValidationType.Schema;
        ////    valReader.ValidationEventHandler += validationHandler;

        ////    while (valReader.Read())
        ////    {
        ////        // reads full document
        ////    }
        ////}

        /// <summary>
        /// Gets the DLS local schema.
        /// </summary>
        /// <returns>Returns the DLS local schema.</returns>
        public static XmlSchema GetDLSLocalSchema()
        {
            Stream stream = GetDLSResourceStream("dls-schema.xsd");
            return XmlSchema.Read(stream, new ValidationEventHandler(OnValidation));
        }

        /// <summary>
        /// Generates the DLS schema.
        /// </summary>
        /// <returns>Returns the DLS schema.</returns>
        public XmlSchema GenerateDLSSchema()
        {
            Stream stream = GetDLSResourceStream("dls-meta-schema.xml");
            XmlDocument dlsMetaSchema = LoadXmlDocument(stream);
            return GenerateSchema(dlsMetaSchema);
        }

        /// <summary>
        /// Generates the schema.
        /// </summary>
        /// <param name="metaSchema">The meta schema.</param>
        /// <returns>Returns the XmlSchema.</returns>
        public XmlSchema GenerateSchema(XmlDocument metaSchema)
        {
            m_mngr = new XmlNamespaceManager(metaSchema.NameTable);
            m_mngr.AddNamespace("m", DEF_META_NS);
            m_metaElement = metaSchema.DocumentElement;

            // Add includes
            XmlNodeList includeList = m_metaElement.SelectNodes("m:include", m_mngr);

            foreach (XmlNode includeNode in includeList)
            {
                string resName = includeNode.Attributes["name"].Value;
                string resSource = includeNode.Attributes["namespace"].Value;
                Stream stream = GetResourceStream(resName, resSource);
                XmlDocument inclDoc = LoadXmlDocument(stream);
                MergeWithInclude(metaSchema, inclDoc);
            }

            m_schema = new XmlSchema();

            // Add root element <xs:element name="{Name}" type="{Type}"/>
            XmlElement root = m_metaElement["m:root"];
            XmlSchemaElement rootElement = new XmlSchemaElement();
            rootElement.Name = root.Attributes["name"].Value;
            rootElement.SchemaTypeName = new XmlQualifiedName(root.Attributes["type"].Value);

            m_schema.Items.Add(rootElement);

            // Add complexType <xs:complexType name="{TypeName}">
            XmlNodeList typeList = m_metaElement.SelectNodes("m:type", m_mngr);

            foreach (XmlNode typeNode in typeList)
            {
                ParseType(typeNode);
            }

            return m_schema;
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Gets the resource stream.
        /// </summary>
        /// <param name="resName">Name of the res.</param>
        /// <param name="resNamespace">The res namespace.</param>
        /// <returns></returns>
        protected virtual Stream GetResourceStream(string resName, string resNamespace)
        {
            if (resNamespace == "Syncfusion.DocIO.DLS")
            {
                return GetDLSResourceStream(resName);
            }

            return null;
        }

        /// <summary>
        /// Gets the DLS resource stream.
        /// </summary>
        /// <param name="resName">Name of the res.</param>
        /// <returns></returns>
        protected static Stream GetDLSResourceStream(string resName)
        {
            Assembly execAssm = Assembly.GetExecutingAssembly();
            return execAssm.GetManifestResourceStream(DEF_DLS_RESOURCES + "." + resName);
        }

        /// <summary>
        /// Loads the XML document.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        protected static XmlDocument LoadXmlDocument(Stream stream)
        {
            StreamReader reader = new StreamReader(stream);
            string strXml = reader.ReadToEnd();

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(strXml);
            return xmlDocument;
        }

        /// <summary>
        /// Called when [validation].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Xml.Schema.ValidationEventArgs"/> instance containing the event data.</param>
        protected static void OnValidation(object sender, ValidationEventArgs args)
        {
            throw new DLSException(args.Message);
        }

        /// <summary>
        /// Parses the type.
        /// </summary>
        /// <param name="typeNode">The type node.</param>
        private void ParseType(XmlNode typeNode)
        {
            XmlAttribute attrMode = typeNode.Attributes["mode"];

            if (attrMode != null)
            {
                switch (attrMode.Value)
                {
                    case "choice":
                        // Add Type sub sequence
                        //   <xs:complexType name="{TypeName}">
                        //     <xs:choice>
                        ParseComplexType(new XmlSchemaChoice(), typeNode, false);
                        break;
                    case "grouping":
                        // Add Type sub sequence
                        //   <xs:complexType name="{TypeName}">
                        //     <xs:sequence>   
                        ParseComplexType(new XmlSchemaSequence(), typeNode, true);
                        break;
                    case "enum":
                        // Add Type sub sequence
                        //   <xs:simpleType name="{TypeName}">
                        ParseSimpleType(typeNode, ModeType.Enum);
                        break;
                    case "pattern":
                        ParseSimpleType(typeNode, ModeType.Pattern);
                        break;
                    case "space":
                        ParseSimpleType(typeNode, ModeType.Space);
                        break;
                }
            }
            else
            {
                // Add Type sub sequence
                //   <xs:complexType name="{TypeName}">
                //     <xs:sequence>   
                ParseComplexType(new XmlSchemaChoice(), typeNode, false);
            }
        }

        /// <summary>
        /// Parses the type of the simple.
        /// </summary>
        /// <param name="typeNode">The type node.</param>
        /// <param name="mode">The mode.</param>
        private void ParseSimpleType(XmlNode typeNode, ModeType mode)
        {
            ////   <xs:simpleType name="{TypeName}">
            XmlSchemaSimpleType simpleType = new XmlSchemaSimpleType();
            simpleType.Name = typeNode.Attributes["name"].Value;

            ////     <xs:restriction base="xs:string">
            XmlSchemaSimpleTypeRestriction simpleTypeRestriction = new XmlSchemaSimpleTypeRestriction();
            simpleTypeRestriction.BaseTypeName = new XmlQualifiedName("string", DEF_SCHEME_NS);
            simpleType.Content = simpleTypeRestriction;

            switch (mode)
            {
                case ModeType.Enum:
                    XmlNodeList enumList = typeNode.SelectNodes("m:enum", m_mngr);
                    foreach (XmlNode enumNode in enumList)
                    {
                        XmlSchemaEnumerationFacet enumFacet = new XmlSchemaEnumerationFacet();
                        enumFacet.Value = enumNode.Attributes["value"].Value;
                        simpleTypeRestriction.Facets.Add(enumFacet);
                    }

                    break;

                case ModeType.Pattern:
                    XmlNode pattern = typeNode.SelectSingleNode("m:pattern", m_mngr);
                    XmlSchemaPatternFacet patternFacet = new XmlSchemaPatternFacet();
                    patternFacet.Value = pattern.Attributes["value"].Value;
                    simpleTypeRestriction.Facets.Add(patternFacet);
                    break;

                case ModeType.Space:
                    XmlNode whitespace = typeNode.SelectSingleNode("m:whitespace", m_mngr);
                    XmlSchemaWhiteSpaceFacet whiteSpaceFacet = new XmlSchemaWhiteSpaceFacet();
                    whiteSpaceFacet.Value = whitespace.Attributes["value"].Value;
                    simpleTypeRestriction.Facets.Add(whiteSpaceFacet);
                    break;
            }

            m_schema.Items.Add(simpleType);
        }

        /// <summary>
        /// Parses the type of the complex.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="typeNode">The type node.</param>
        /// <param name="isGrouping">if set to <c>true</c> [is grouping].</param>
        private void ParseComplexType(XmlSchemaGroupBase group, XmlNode typeNode, bool isGrouping)
        {
            XmlSchemaComplexType complexType = new XmlSchemaComplexType();
            complexType.Name = typeNode.Attributes["name"].Value;
            if (isGrouping)
            {
                string groupName = complexType.Name + "Group";
                XmlSchemaGroupRef refGroup = new XmlSchemaGroupRef();
                refGroup.RefName = new XmlQualifiedName(groupName);
                complexType.Particle = refGroup;

                XmlSchemaGroup extGroup = new XmlSchemaGroup();
                extGroup.Name = groupName;
                extGroup.Particle = group;
                m_schema.Items.Add(extGroup);
            }
            else
            {
                complexType.Particle = group;
                complexType.Particle.MaxOccursString = "unbounded";
                complexType.Particle.MinOccurs = 0;
            }

            m_schema.Items.Add(complexType);

            // Add Type elements:
            //       <xs:element name="{GroupName}">
            //         <xs:sequence>
            //         <xs:complexType>
            //             <xs:element name="{Name}" type="{Type}">
            XmlNodeList elementList = typeNode.SelectNodes("m:element", m_mngr);
            foreach (XmlNode elementNode in elementList)
            {
                ParseElement(elementNode, group);
            }

            // Add Type sub groups:
            //       <xs:element name="{GroupName}">
            //         <xs:complexType>
            //           <xs:sequence>
            //             <xs:element name="{ItemName}" type="{ItemType}">
            XmlNodeList groupList = typeNode.SelectNodes("m:group", m_mngr);
            foreach (XmlNode groupNode in groupList)
            {
                XmlAttribute attrRef = groupNode.Attributes["ref"];
                if (attrRef != null)
                {
                    XmlNode groupNodeRef = m_metaElement.SelectSingleNode(
                      "m:group[@name='" + attrRef.Value + "']", m_mngr);

                    ParseGroup(groupNodeRef, group);
                }
                else
                {
                    ParseGroup(groupNode, group);
                }
            }

            XmlSchemaObjectCollection attributes = complexType.Attributes;
            if (isGrouping)
            {
                string groupName = complexType.Name + "AttrGroup";
                XmlSchemaAttributeGroupRef refGroup = new XmlSchemaAttributeGroupRef();
                refGroup.RefName = new XmlQualifiedName(groupName);
                complexType.Attributes.Add(refGroup);

                XmlSchemaAttributeGroup extGroup = new XmlSchemaAttributeGroup();
                extGroup.Name = groupName;
                m_schema.Items.Add(extGroup);

                attributes = extGroup.Attributes;
            }

            // Add attributes:
            //       <xs:element name="{GroupName}">
            //         <xs:complexType>
            //           <xs:attribute name="{Name}" type="{Type}">
            XmlNodeList attrList = typeNode.SelectNodes("m:attribute", m_mngr);
            foreach (XmlNode attrNode in attrList)
            {
                ParseAttribute(attrNode, attributes);
            }

            //// Add "id" attribute
            XmlSchemaAttribute attrID = new XmlSchemaAttribute();
            attrID.Name = "id";
            attrID.SchemaTypeName = new XmlQualifiedName("int", DEF_SCHEME_NS);
            complexType.Attributes.Add(attrID);
        }

        /// <summary>
        /// Parses the element.
        /// </summary>
        /// <param name="elementNode">The element node.</param>
        /// <param name="group">The group.</param>
        private void ParseElement(XmlNode elementNode, XmlSchemaGroupBase group)
        {
            ////       <xs:element name="{Name}" type="{Type}">
            XmlSchemaElement element = new XmlSchemaElement();
            element.Name = elementNode.Attributes["name"].Value;
            XmlAttribute attrType = elementNode.Attributes["type"];

            if (((IList)DEF_STANDARD_TYPES).Contains(attrType.Value))
            {
                element.SchemaTypeName = new XmlQualifiedName(attrType.Value, DEF_SCHEME_NS);
            }
            else
            {
                element.SchemaTypeName = new XmlQualifiedName(attrType.Value);
            }

            group.Items.Add(element);
        }

        /// <summary>
        /// Parses the group.
        /// </summary>
        /// <param name="groupNode">The group node.</param>
        /// <param name="group">The group.</param>
        private void ParseGroup(XmlNode groupNode, XmlSchemaGroupBase group)
        {
            ////       <xs:element name="{GroupName}">
            XmlSchemaElement groupElement = new XmlSchemaElement();
            groupElement.Name = groupNode.Attributes["name"].Value;
            group.Items.Add(groupElement);

            ////         <xs:complexType>
            XmlSchemaComplexType groupComplexType = new XmlSchemaComplexType();
            groupElement.SchemaType = groupComplexType;

            ////           <xs:sequence>
            XmlSchemaSequence groupSeq = new XmlSchemaSequence();
            groupComplexType.Particle = groupSeq;
            groupComplexType.Particle.MaxOccursString = "unbounded";
            groupComplexType.Particle.MinOccurs = 0;

            ////             <xs:element name="{ItemName}" type="{ItemType}">
            XmlSchemaElement itemElement = new XmlSchemaElement();
            itemElement.Name = groupNode.Attributes["item"].Value;
            itemElement.SchemaTypeName = new XmlQualifiedName(groupNode.Attributes["type"].Value);

            groupSeq.Items.Add(itemElement);
        }

        /// <summary>
        /// Parses the attribute.
        /// </summary>
        /// <param name="attrNode">The attr node.</param>
        /// <param name="attributes">The attributes.</param>
        private void ParseAttribute(XmlNode attrNode, XmlSchemaObjectCollection attributes)
        {
            ////           <xs:attribute name="{Name}" type="{Type}">
            XmlSchemaAttribute attr = new XmlSchemaAttribute();
            attr.Name = attrNode.Attributes["name"].Value;
            XmlAttribute attrType = attrNode.Attributes["type"];
            XmlAttribute attrFixed = attrNode.Attributes["fixed"];

            ////XmlNode typeNode = m_metaElement.SelectSingleNode( 
            ////  "m:type[@name='" + attrType+ "']", m_mngr );

            if (((IList)DEF_STANDARD_TYPES).Contains(attrType.Value))
            {
                attr.SchemaTypeName = new XmlQualifiedName(attrType.Value, DEF_SCHEME_NS);
            }
            else
            {
                attr.SchemaTypeName = new XmlQualifiedName(attrType.Value);
            }

            if (attrFixed != null)
            {
                attr.FixedValue = attrFixed.Value;
            }

            attributes.Add(attr);
        }

        /// <summary>
        /// Merges the with include.
        /// </summary>
        /// <param name="metaSchema">The meta schema.</param>
        /// <param name="includeSchema">The include schema.</param>
        private void MergeWithInclude(XmlDocument metaSchema, XmlDocument includeSchema)
        {
            MergeWithInclude(includeSchema, metaSchema, "m:type");
            MergeWithInclude(includeSchema, metaSchema, "m:group");
        }

        /// <summary>
        /// Merges the with include.
        /// </summary>
        /// <param name="includeSchema">The include schema.</param>
        /// <param name="metaSchema">The meta schema.</param>
        /// <param name="tagsName">Name of the tags.</param>
        private void MergeWithInclude(XmlDocument includeSchema, XmlDocument metaSchema, string tagsName)
        {
            XmlNodeList includeNodes = includeSchema.DocumentElement.SelectNodes(tagsName, m_mngr);

            foreach (XmlNode includeNode in includeNodes)
            {
                string name = includeNode.Attributes["name"].Value;
                XmlNode selTypeNode = metaSchema.DocumentElement.SelectSingleNode(tagsName + "[@name='" + name + "']", m_mngr);

                if (selTypeNode == null)
                {
                    XmlNode newNode = metaSchema.CreateNode(XmlNodeType.Element, "temp", string.Empty);
                    newNode.InnerXml = includeNode.OuterXml;
                    metaSchema.DocumentElement.AppendChild(newNode.FirstChild);
                }
                else
                {
                    string oldInner = selTypeNode.InnerXml;
                    selTypeNode.InnerXml = oldInner + includeNode.InnerXml;
                }
            }
        }
        #endregion
    }
}

#endif