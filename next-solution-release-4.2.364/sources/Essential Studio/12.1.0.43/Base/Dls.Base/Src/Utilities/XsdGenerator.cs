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

#region file using directives
using System;
using System.Collections;
using System.Xml;
using System.Xml.Schema;
using System.Reflection;
using System.Resources;
using System.IO;
using Syncfusion.Documentation;
#endregion

namespace Syncfusion.DLS
{
  /// <summary>
  /// Summary description for XSDSchemeGenerator.
  /// </summary>
  [ DocumentationExclude() ]
  public class XsdGenerator
  {
    #region Class constants
    /// <summary>
    /// 
    /// </summary>
    private const string DEF_SCHEME_NS = "http://www.w3.org/2001/XMLSchema";
    private const string DEF_META_NS = "http://tempuri.org/DLSMetaSchema.xsd";
    private readonly string[] DEF_STANDARD_TYPES = new string[]
      {
       "string", "float", "boolean", "int", "datetime"
      };
    protected const string DEF_DLS_RESOURCES = "Syncfusion.DLS.Resources";
    #endregion

    #region Class members
    private XmlSchema m_schema;
    private XmlNamespaceManager m_mngr;
    private XmlElement m_metaElement;
    #endregion

    #region Class public methods
//    /// <summary>
//    /// 
//    /// </summary>
//    /// <param name="reader"></param>
//    public static void ValidateBySchema( XmlReader reader, ValidationEventHandler validationHandler  )
//    {
//      XmlValidatingReader valReader = new XmlValidatingReader( reader );
//      
//      valReader.ValidationType = ValidationType.Schema;
//      valReader.ValidationEventHandler += validationHandler;
//      
//      while( valReader.Read() )
//      {
//        // reads full document
//      }
//    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static XmlSchema GetDLSLocalSchema()
    {
      Stream stream = GetDLSResourceStream("dls-schema.xsd");
      return XmlSchema.Read(stream, new ValidationEventHandler(OnValidation) );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public XmlSchema GenerateDLSSchema()
    {
      Stream stream = GetDLSResourceStream("dls-meta-schema.xml");
      XmlDocument dlsMetaSchema = LoadXmlDocument( stream );
      return GenerateSchema( dlsMetaSchema );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="metaSchema"></param>
    /// <returns></returns>
    public XmlSchema GenerateSchema( XmlDocument metaSchema )
    {
      m_mngr = new XmlNamespaceManager( metaSchema.NameTable );
      m_mngr.AddNamespace( "m", DEF_META_NS );
      m_metaElement = metaSchema.DocumentElement;
      
      // Add includes
      XmlNodeList includeList = m_metaElement.SelectNodes("m:include", m_mngr);
      
      foreach(XmlNode includeNode in includeList )
      {
        string resName = includeNode.Attributes["name"].Value;
        string resSource = includeNode.Attributes["namespace"].Value;
        Stream stream = GetResourceStream( resName, resSource );
        XmlDocument inclDoc = LoadXmlDocument(stream);
        MergeWithInclude( metaSchema, inclDoc );
      }
      
      m_schema = new XmlSchema();

      // Add root element <xs:element name="{Name}" type="{Type}"/>
      XmlElement root = m_metaElement[ "m:root" ];
      XmlSchemaElement rootElement = new XmlSchemaElement();
      rootElement.Name = root.Attributes[ "name" ].Value;
      rootElement.SchemaTypeName = new XmlQualifiedName( root.Attributes[ "type" ].Value );

      m_schema.Items.Add( rootElement );

      // Add complexType <xs:complexType name="{TypeName}">
      XmlNodeList typeList = m_metaElement.SelectNodes( "m:type", m_mngr );

      foreach (XmlNode typeNode in typeList)
      {
        ParseType( typeNode );
      }
      return m_schema;
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="resName"></param>
    /// <param name="resNamespace"></param>
    /// <returns></returns>
    protected virtual Stream GetResourceStream( string resName, string resNamespace )
    {
      if( resNamespace == "Syncfusion.DLS" )
      {
        return GetDLSResourceStream( resName );
      }
      
      return null;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="resName"></param>
    /// <returns></returns>
    protected static Stream GetDLSResourceStream( string resName )
    {
      Assembly execAssm = Assembly.GetExecutingAssembly();
      return execAssm.GetManifestResourceStream( DEF_DLS_RESOURCES + "." + resName );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    protected static XmlDocument LoadXmlDocument( Stream stream )
    {
      StreamReader reader = new StreamReader(stream);
      string strXml = reader.ReadToEnd();

      XmlDocument xmlDocument = new XmlDocument();
      xmlDocument.LoadXml( strXml );
      return xmlDocument;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    protected static void OnValidation( object sender, ValidationEventArgs args )
    {
      throw new DLSException(args.Message);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="typeNode"></param>
    private void ParseType( XmlNode typeNode )
    {
      XmlAttribute attrMode = typeNode.Attributes[ "mode" ];
      
      if( attrMode != null)
      {
        switch( attrMode.Value )
        {
          case "choice":
            // Add Type sub sequence
            //   <xs:complexType name="{TypeName}">
            //     <xs:sequence>   
            ParseComplexType( new XmlSchemaChoice(), typeNode, false );
            break;
          case "grouping":
            // Add Type sub sequence
            //   <xs:complexType name="{TypeName}">
            //     <xs:sequence>   
            ParseComplexType( new XmlSchemaSequence(), typeNode, true );
            break;
          case "enum":
            // Add Type sub sequence
            //   <xs:simpleType name="{TypeName}">
            ParseSimpleType( typeNode, true );
            break;
          case "pattern":
            ParseSimpleType( typeNode, false );
            break;
        }
      }
      else
      {
        // Add Type sub sequence
        //   <xs:complexType name="{TypeName}">
        //     <xs:sequence>   
        ParseComplexType( new XmlSchemaSequence(), typeNode, false );
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="typeNode"></param>
    /// <param name="isEnumOrPattern"></param>
    private void ParseSimpleType( XmlNode typeNode, bool isEnumOrPattern )
    {
      //   <xs:simpleType name="{TypeName}">
      XmlSchemaSimpleType simpleType = new XmlSchemaSimpleType();
      simpleType.Name = typeNode.Attributes[ "name" ].Value;
      
      //     <xs:restriction base="xs:string">
      XmlSchemaSimpleTypeRestriction simpleTypeRestriction = new XmlSchemaSimpleTypeRestriction();
      simpleTypeRestriction.BaseTypeName = new XmlQualifiedName( "string", DEF_SCHEME_NS );
      simpleType.Content = simpleTypeRestriction;
      
      if( isEnumOrPattern )
      {
        XmlNodeList enumList = typeNode.SelectNodes( "m:enum", m_mngr );
        foreach (XmlNode enumNode in enumList)
        {
          XmlSchemaEnumerationFacet enumFacet = new XmlSchemaEnumerationFacet();
          enumFacet.Value = enumNode.Attributes[ "value" ].Value;
          simpleTypeRestriction.Facets.Add(enumFacet);
        }
      }
      else
      {
        XmlNode pattern = typeNode.SelectSingleNode( "m:pattern", m_mngr );
        XmlSchemaPatternFacet patternFacet = new XmlSchemaPatternFacet();
        patternFacet.Value = pattern.Attributes["value"].Value;
        simpleTypeRestriction.Facets.Add(patternFacet);
      }
      m_schema.Items.Add( simpleType );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="group"></param>
    /// <param name="typeNode"></param>
    /// <param name="isGrouping"></param>
    private void ParseComplexType( XmlSchemaGroupBase group, XmlNode typeNode, bool isGrouping )
    {
      XmlSchemaComplexType complexType = new XmlSchemaComplexType();
      complexType.Name = typeNode.Attributes[ "name" ].Value;
      
      
      if( isGrouping )
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
      }
      m_schema.Items.Add( complexType );
      
      // Add Type elements:
      //       <xs:element name="{GroupName}">
      //         <xs:sequence>
      //         <xs:complexType>
      //             <xs:element name="{Name}" type="{Type}">
      XmlNodeList elementList = typeNode.SelectNodes( "m:element", m_mngr );
      foreach (XmlNode elementNode in elementList)
      {
        ParseElement( elementNode, group );
      }
      
      // Add Type sub groups:
      //       <xs:element name="{GroupName}">
      //         <xs:complexType>
      //           <xs:sequence>
      //             <xs:element name="{ItemName}" type="{ItemType}">
      XmlNodeList groupList = typeNode.SelectNodes( "m:group", m_mngr );
      foreach (XmlNode groupNode in groupList)
      {
        XmlAttribute attrRef = groupNode.Attributes[ "ref" ];
        if( attrRef != null )
        {
          XmlNode groupNodeRef = m_metaElement.SelectSingleNode( 
            "m:group[@name='" + attrRef.Value+ "']", m_mngr );
        
          ParseGroup( groupNodeRef, group );
        }
        else
        {
          ParseGroup( groupNode, group );
        }
      }
      
      XmlSchemaObjectCollection attributes = complexType.Attributes;
      if( isGrouping )
      {
        string groupName = complexType.Name + "AttrGroup";
        XmlSchemaAttributeGroupRef refGroup = new XmlSchemaAttributeGroupRef();
        refGroup.RefName = new XmlQualifiedName(groupName);
        complexType.Attributes.Add( refGroup );
               
        XmlSchemaAttributeGroup extGroup = new XmlSchemaAttributeGroup();
        extGroup.Name = groupName;
        m_schema.Items.Add(extGroup);
        
        attributes = extGroup.Attributes;
      }
      
      // Add attributes:
      //       <xs:element name="{GroupName}">
      //         <xs:complexType>
      //           <xs:attribute name="{Name}" type="{Type}">
      XmlNodeList attrList = typeNode.SelectNodes( "m:attribute", m_mngr );
      foreach (XmlNode attrNode in attrList)
      {
        ParseAttribute( attrNode, attributes );
      }
      
      // Processes extentions
      XmlNodeList extList = typeNode.SelectNodes( "m:extention", m_mngr );
      foreach (XmlNode extNode in extList)
      {
        ParseExtention( extNode, group, attributes );
      }
      
      // Add "id" attribute
      XmlSchemaAttribute attrID = new XmlSchemaAttribute();
      attrID.Name = "id";
      attrID.SchemaTypeName = new XmlQualifiedName( "int", DEF_SCHEME_NS );
      complexType.Attributes.Add(attrID);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="extNode"></param>
    /// <param name="group"></param>
    /// <param name="attributes"></param>
    private void ParseExtention(XmlNode extNode, XmlSchemaGroupBase group, XmlSchemaObjectCollection attributes )
    {
      string extRef = extNode.Attributes["ref"].Value;
      XmlSchemaGroupRef groupRef = new XmlSchemaGroupRef();
      groupRef.RefName = new XmlQualifiedName(extRef + "Group");
      group.Items.Add(groupRef);
      
      XmlSchemaAttributeGroupRef attrGroup = new XmlSchemaAttributeGroupRef();
      attrGroup.RefName = new XmlQualifiedName(extRef + "AttrGroup");
      attributes.Add(attrGroup);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="elementNode"></param>
    /// <param name="group"></param>
    private void ParseElement( XmlNode elementNode, XmlSchemaGroupBase group )
    {
      //       <xs:element name="{Name}" type="{Type}">
      XmlSchemaElement element = new XmlSchemaElement();
      element.Name = elementNode.Attributes[ "name" ].Value;
      XmlAttribute attrType = elementNode.Attributes[ "type" ];
      
      if( ((IList)DEF_STANDARD_TYPES).Contains( attrType.Value ) )
      {
        element.SchemaTypeName = new XmlQualifiedName( attrType.Value, DEF_SCHEME_NS );
      }
      else
      {
        element.SchemaTypeName = new XmlQualifiedName( attrType.Value );
      }
      group.Items.Add( element );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="groupNode"></param>
    /// <param name="group"></param>
    private void ParseGroup( XmlNode groupNode, XmlSchemaGroupBase group )
    {
      //       <xs:element name="{GroupName}">
      XmlSchemaElement groupElement = new XmlSchemaElement();
      groupElement.Name = groupNode.Attributes[ "name" ].Value;
      group.Items.Add( groupElement );

      //         <xs:complexType>
      XmlSchemaComplexType groupComplexType = new XmlSchemaComplexType();
      groupElement.SchemaType = groupComplexType;

      //           <xs:sequence>
      XmlSchemaSequence groupSeq = new XmlSchemaSequence();
      groupComplexType.Particle = groupSeq;

      //             <xs:element name="{ItemName}" type="{ItemType}">
      XmlSchemaElement itemElement = new XmlSchemaElement();
      itemElement.Name = groupNode.Attributes[ "item" ].Value;
      itemElement.SchemaTypeName = new XmlQualifiedName( groupNode.Attributes[ "type" ].Value );

      groupSeq.Items.Add( itemElement );
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="attrNode"></param>
    /// <param name="attributes"></param>
    private void ParseAttribute( XmlNode attrNode, XmlSchemaObjectCollection attributes )
    {
      //           <xs:attribute name="{Name}" type="{Type}">
      XmlSchemaAttribute attr = new XmlSchemaAttribute();
      attr.Name = attrNode.Attributes[ "name" ].Value;
      XmlAttribute attrType = attrNode.Attributes[ "type" ];
      XmlAttribute attrFixed = attrNode.Attributes[ "fixed" ];
      
      //XmlNode typeNode = m_metaElement.SelectSingleNode( 
      //  "m:type[@name='" + attrType+ "']", m_mngr );
      
      if( ((IList)DEF_STANDARD_TYPES).Contains( attrType.Value ) )
      {
        attr.SchemaTypeName = new XmlQualifiedName( attrType.Value, DEF_SCHEME_NS );
      }
      else
      {
        attr.SchemaTypeName = new XmlQualifiedName( attrType.Value );
      }

      if( attrFixed != null )
      {
        attr.FixedValue = attrFixed.Value;
      }
      attributes.Add(attr);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="metaSchema"></param>
    /// <param name="includeSchema"></param>
    private void MergeWithInclude( XmlDocument metaSchema, XmlDocument includeSchema )
    {
      MergeWithInclude(includeSchema, metaSchema, "m:type");
      MergeWithInclude(includeSchema, metaSchema, "m:group");
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="includeSchema"></param>
    /// <param name="metaSchema"></param>
    /// <param name="tagsName"></param>
    private void MergeWithInclude(XmlDocument includeSchema, XmlDocument metaSchema, string tagsName )
    {
      XmlNodeList includeNodes = includeSchema.DocumentElement.SelectNodes(tagsName, m_mngr);
      
      foreach( XmlNode includeNode in includeNodes)
      {
        string name = includeNode.Attributes["name"].Value;
        XmlNode selTypeNode = metaSchema.DocumentElement.SelectSingleNode(tagsName + "[@name='" + name + "']", m_mngr);
       
        if( selTypeNode == null)
        {
          XmlNode newNode = metaSchema.CreateNode(XmlNodeType.Element, "temp", "");
          newNode.InnerXml = includeNode.OuterXml;
          metaSchema.DocumentElement.AppendChild( newNode.FirstChild );
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