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
using System.IO;

namespace Syncfusion.XlsIO.Implementation.XmlSerialization.Shapes
{
  /// <summary>
  /// This is serializator for unknown/unsupported vml shapes that were parsed.
  /// </summary>
  class UnknownShapeSerializator : ShapeSerializator
  {
    #region Members
    /// <summary>
    /// Stream that contains xml data that describes shape type.
    /// </summary>
    private Stream m_shapeTypeStream;
    #endregion

    #region Methods
    /// <summary>
    /// Initializes a new instance of the UnknownShapeSerializator class.
    /// </summary>
    /// <param name="shapeTypeStream">Stream that contains shape type data.</param>
    public UnknownShapeSerializator( Stream shapeTypeStream )
    {
      if( shapeTypeStream == null || shapeTypeStream.Length == 0 )
        throw new ArgumentOutOfRangeException( "shapeTypeStream" );

      m_shapeTypeStream = shapeTypeStream;
    }
    /// <summary>
    /// This method serializes specified shape into specified writer.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize shape settings into.</param>
    /// <param name="shape">Shape to serialize.</param>
    /// <param name="holder">Parent worksheet data holder.</param>
    public override void Serialize( XmlWriter writer, ShapeImpl shape, WorksheetDataHolder holder,
      RelationCollection vmlRelations )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( shape == null )
        throw new ArgumentNullException( "shape" );

      if( holder == null )
        throw new ArgumentNullException( "holder" );

      Stream stream = shape.XmlDataStream;
      stream.Position = 0;
      XmlReader reader = UtilityMethods.CreateReader( stream );
      writer.WriteNode( reader, false );
      writer.Flush();
    }
    /// <summary>
    /// This method serializes general shape settings (shape type) into specified XmlWriter.
    /// </summary>
    /// <param name="writer">XmlWriter to write shape type into.</param>
    /// <param name="shapeType">Type of the shape that is going to be serialized.</param>
    public override void SerializeShapeType( XmlWriter writer, Type shapeType )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      m_shapeTypeStream.Position = 0;
      XmlReader reader = UtilityMethods.CreateReader( m_shapeTypeStream );
      writer.WriteNode( reader, false );
      writer.Flush();
    }
    #endregion
  }
}
