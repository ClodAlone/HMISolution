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
using Syncfusion.XlsIO.Implementation.Shapes;
using Syncfusion.XlsIO.Implementation.XmlSerialization;
using System.Xml;
using System.IO;
using Syncfusion.XlsIO.Implementation.XmlSerialization.Shapes;
using Syncfusion.XlsIO.Implementation.Collections;

namespace Syncfusion.XlsIO.Implementation.XmlReaders.Shapes
{
  /// <summary>
  /// This is parser of unknown vml shapes.
  /// </summary>
  class UnknownVmlShapeParser : ShapeParser
  {
    #region Methods
    /// <summary>
    /// Extracts shape type settings from the reader and creates shape with default settings.
    /// </summary>
    /// <param name="reader">XmlReader to get general shape settings from.</param>
    /// <param name="parent">Parent worksheet for the shape.</param>
    /// <returns>Shape with default settings without adding it to any collection</returns>
    public override ShapeImpl ParseShapeType( XmlReader reader, ShapeCollectionBase shapes )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( shapes == null )
        throw new ArgumentNullException( "shapes" );

      if( !reader.MoveToAttribute( Vml.SptAttriubteName, Vml.ONamespace ) )
        throw new XmlException();

      int iShapeInstance = int.Parse( reader.Value );
      reader.MoveToElement();

      AddNewSerializator( iShapeInstance, reader, shapes );
      ShapeImpl shape = new ShapeImpl( shapes.Application, shapes );
      return shape;
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
      ShapeImpl shape = ( ShapeImpl )defaultShape.Clone( defaultShape.Parent );
      MemoryStream shapeDataStream = new MemoryStream();
      XmlWriter writer = UtilityMethods.CreateWriter( shapeDataStream, Encoding.UTF8 );
      writer.WriteNode( reader, false );
      writer.Flush();
      shape.XmlDataStream = shapeDataStream;

      shapeDataStream.Position = 0;
      reader = UtilityMethods.CreateReader( shapeDataStream );

      reader.Read();

      while( reader.NodeType != XmlNodeType.None )
      {
        if( reader.NodeType == XmlNodeType.Element && reader.LocalName == Vml.ImageDataTag )
        {
          if( reader.MoveToAttribute( Vml.RelationIDAttribute, Vml.ONamespace ) )
          {
            string relationId = reader.Value;
            shape.ImageRelation = ( Relation )relations[ relationId ].Clone();
            shape.ImageRelationId = relationId;
          }

          break;
        }
        else
        {
          reader.Read();
        }
      }

      shapeDataStream.Position = 0;

      return true;
    }
    /// <summary>
    /// Creates new Unknown shapes serializator.
    /// </summary>
    /// <param name="shapeInstance">Shape instance used to choose correct shape serializator.</param>
    /// <param name="reader">XmlReader to get shape from.</param>
    /// <param name="sheet">Represents current worksheet.</param>
    private void AddNewSerializator( int shapeInstance, XmlReader reader, ShapeCollectionBase shapes )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( shapes == null )
        throw new ArgumentNullException( "shapes" );

      MemoryStream shapeTypeStream = new MemoryStream();
      XmlWriter writer = UtilityMethods.CreateWriter( shapeTypeStream, Encoding.UTF8 );
      writer.WriteNode( reader, false );
      writer.Flush();

      WorksheetBaseImpl sheet = shapes.WorksheetBase;
      UnknownShapeSerializator serializator = new UnknownShapeSerializator( shapeTypeStream );
      sheet.DataHolder.ParentHolder.Serializator.VmlSerializators[ shapeInstance ] = serializator;
      sheet.UnknownVmlShapes = true;
    }
    #endregion
  }
}
