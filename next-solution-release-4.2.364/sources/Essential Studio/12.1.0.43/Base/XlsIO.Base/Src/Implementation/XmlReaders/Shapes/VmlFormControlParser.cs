#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Syncfusion.XlsIO.Implementation.Shapes;
using Syncfusion.XlsIO.Implementation.XmlSerialization;
using System.IO;
using Syncfusion.XlsIO.Implementation.Collections;

namespace Syncfusion.XlsIO.Implementation.XmlReaders.Shapes
{
  /// <summary>
  /// This class is used for vml form controls parsing.
  /// </summary>
  class VmlFormControlParser : ShapeParser
  {
    #region Members
    /// <summary>
    /// Dictionary of all found shape types. Key - shape type id, value - shape type stream.
    /// </summary>
    private Dictionary<string, Stream> m_dictShapeTypes = new Dictionary<string, Stream>();
    /// <summary>
    /// Dictinary with all supported shape parsers. Key - shape type, Value - shape parser to parse shape.
    /// </summary>
    private static Dictionary<string, ShapeParser> m_dictShapeParser = new Dictionary<string, ShapeParser>();
    #endregion

    #region Methods
    /// <summary>
    /// Initializes static members.
    /// </summary>
    static VmlFormControlParser()
    {
      m_dictShapeParser.Add( Vml.Checkbox, new CheckBoxShapeParser() );
	  m_dictShapeParser.Add( Vml.OptionButton, new OptionButtonShapeParser() );
      m_dictShapeParser.Add( Vml.Drop, new ComboBoxShapeParser() );
    }
    /// <summary>
    /// Extracts shape type settings from the reader and creates shape with default settings.
    /// </summary>
    /// <param name="reader">XmlReader to get general shape settings from.</param>
    /// <param name="parent">Parent worksheet for the shape.</param>
    /// <returns>
    /// Shape with default settings without adding it to any collection.
    /// </returns>
    public override ShapeImpl ParseShapeType( XmlReader reader, ShapeCollectionBase shapes )
    {
      if( !reader.MoveToAttribute( Vml.ShapeIdAttributeName ) )
        throw new XmlException();

      string strShapeId = reader.Value;
      reader.MoveToElement();

      Stream stream = ShapeParser.ReadNodeAsStream( reader );
      m_dictShapeTypes[ strShapeId ] = stream; 
      ShapeImpl result = new ShapeImpl( shapes.Application, shapes );
      result.XmlTypeStream = stream;
      return result;
    }
    /// <summary>
    /// Parses shape and adds it to all necessary shapes collections.
    /// </summary>
    /// <param name="reader">XmlReader to get shape from.</param>
    /// <param name="defaultShape">Default shape that must be cloned to get resulting shape.</param>
    /// <param name="relations">Corresponding relations collection.</param>
    /// <param name="parentItemPath">Path to the parent item (item which holds all these xml tags).</param>
    public override bool ParseShape( XmlReader reader, ShapeImpl defaultShape,
      RelationCollection relations, string parentItemPath )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      Stream stream = ReadNodeAsStream( reader );

      stream.Position = 0;
      reader = UtilityMethods.CreateReader( stream );

      reader.MoveToAttribute( Vml.TypeAttributeName );
      string strShapeType = reader.Value;

      reader.MoveToElement();
      reader.Read();

      string strObjectType = null;

      while( reader.NodeType != XmlNodeType.EndElement && strObjectType == null )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case Vml.ClientDataTagName:
              if( reader.MoveToAttribute( Vml.ObjectTypeAttribute ) )
                strObjectType = reader.Value;
              break;

            default:
              reader.Skip();
              break;
          }
        }
        else
        {
          reader.Skip();
        }
      }

      ShapeParser parser;
      bool bResult = false;
      if (strObjectType != null)
      {
          if (m_dictShapeParser.TryGetValue(strObjectType, out parser))
          {
              strShapeType = UtilityMethods.RemoveFirstCharUnsafe(strShapeType);
              Stream shapeTypeStream = m_dictShapeTypes[strShapeType];
              shapeTypeStream.Position = 0;
              XmlReader shapeTypeReader = UtilityMethods.CreateReader(shapeTypeStream);
              defaultShape = parser.ParseShapeType(reader, defaultShape.ParentShapes);

              stream.Position = 0;
              reader = UtilityMethods.CreateReader(stream);

              bResult = parser.ParseShape(reader, defaultShape, relations, parentItemPath);
          }
      }
      return bResult;
    }
    #endregion
  }
}
