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
using Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing;
using Syncfusion.XlsIO.Implementation.Collections;

namespace Syncfusion.XlsIO.Implementation.XmlReaders.Shapes
{
  /// <summary>
  /// This class is responsible for vml shape and shape type parsing.
  /// </summary>
  public abstract class ShapeParser
  {
    /// <summary>
    /// Extracts shape type settings from the reader and creates shape with default settings.
    /// </summary>
    /// <param name="reader">XmlReader to get general shape settings from.</param>
    /// <param name="parent">Parent worksheet for the shape.</param>
    /// <returns>
    /// Shape with default settings without adding it to any collection.
    /// </returns>
    public abstract ShapeImpl ParseShapeType( XmlReader reader, ShapeCollectionBase shapes );
    /// <summary>
    /// Parses shape and adds it to all necessary shapes collections.
    /// </summary>
    /// <param name="reader">XmlReader to get shape from.</param>
    /// <param name="defaultShape">Default shape that must be cloned to get resulting shape.</param>
    /// <param name="relations">Corresponding relations collection.</param>
    /// <param name="parentItemPath">Path to the parent item (item which holds all these xml tags).</param>
    public abstract bool ParseShape( XmlReader reader, ShapeImpl defaultShape,
      RelationCollection relations, string parentItemPath );
    /// <summary>
    /// Saves current node into stream.
    /// </summary>
    /// <param name="reader">Reader to get node from.</param>
    /// <returns>Stream with current node's data.</returns>
    public static Stream ReadNodeAsStream( XmlReader reader )
    {
      return ReadNodeAsStream( reader, false );
    }
    /// <summary>
    /// Saves current node into stream.
    /// </summary>
    /// <param name="reader">Reader to get node from.</param>
    /// <returns>Stream with current node's data.</returns>
    public static Stream ReadNodeAsStream( XmlReader reader, bool writeNamespaces )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      MemoryStream stream = new MemoryStream();
      XmlWriter writer = UtilityMethods.CreateWriter( stream, Encoding.UTF8 );
      writer.WriteNode( reader, writeNamespaces );
      writer.Flush();

      return stream;
    }
    /// <summary>
    /// Reads node from the stream and writes it into XmlWriter.
    /// </summary>
    /// <param name="writer">Writer to write node into.</param>
    /// <param name="stream">Stream to get node from.</param>
    public static void WriteNodeFromStream( XmlWriter writer, Stream stream )
    {
      WriteNodeFromStream( writer, stream, false );
    }
    /// <summary>
    /// Reads node from the stream and writes it into XmlWriter.
    /// </summary>
    /// <param name="writer">Writer to write node into.</param>
    /// <param name="stream">Stream to get node from.</param>
    public static void WriteNodeFromStream( XmlWriter writer, Stream stream, bool writeNamespaces )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( stream == null )
        throw new ArgumentNullException( "stream" );

      XmlReader reader = UtilityMethods.CreateReader( stream );

      writer.WriteNode( reader, writeNamespaces );
      writer.Flush();
    }
    /// <summary>
    /// Parses shape's anchor.
    /// </summary>
    /// <param name="reader">XmlReader to get anchor information from.</param>
    /// <param name="shape">Shape to set anchor to.</param>
    protected void ParseAnchor( XmlReader reader, ShapeImpl shape )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( shape == null )
        throw new ArgumentNullException( "shape" );

      string strAnchor = reader.ReadElementContentAsString();//reader.Value;
      string[] arrAnchor = strAnchor.Split( ',' );

      if( arrAnchor.Length != 8 )
        throw new XmlException( "Wrong anchor format" );

      for( int i = 0, len = arrAnchor.Length; i < len; i++ )
      {
        arrAnchor[ i ] = arrAnchor[ i ].Trim();
      }

      MsofbtClientAnchor anchor = shape.ClientAnchor;
      int iRowColumn = int.Parse( arrAnchor[ 0 ] );
      int iOffset = int.Parse( arrAnchor[ 1 ] );
      anchor.LeftColumn = iRowColumn;
      anchor.LeftOffset = shape.PixelsInOffset( iRowColumn + 1, iOffset, true );
      shape.CheckLeftOffset();
      iRowColumn = int.Parse( arrAnchor[ 2 ] );
      iOffset = int.Parse( arrAnchor[ 3 ] );
      anchor.TopRow = iRowColumn;
      anchor.TopOffset = shape.PixelsInOffset( iRowColumn + 1, iOffset, false );

      iRowColumn = int.Parse( arrAnchor[ 4 ] );
      iOffset = int.Parse( arrAnchor[ 5 ] );
      anchor.RightColumn = iRowColumn;
      anchor.RightOffset = shape.PixelsInOffset( iRowColumn + 1, iOffset, true );

      iRowColumn = int.Parse( arrAnchor[ 6 ] );
      iOffset = int.Parse( arrAnchor[ 7 ] );
      anchor.BottomRow = iRowColumn;
      anchor.BottomOffset = shape.PixelsInOffset( iRowColumn + 1, iOffset, false );

      shape.UpdateHeight();
      shape.UpdateWidth();
    }
    /// <summary>
    /// Splits style into properties dictionary.
    /// </summary>
    /// <param name="styleValue">Value to split.</param>
    /// <returns>Dictionary with properties, key - property name, value - property value.</returns>
    protected Dictionary<string, string> SplitStyle( string styleValue )
    {
      Dictionary<string, string> dictProperties = new Dictionary<string, string>();

      if( styleValue != null )
      {
        string[] arrStyle = styleValue.Split( ';' );

        // We don't use most of style properties, so they will be skipped.
        for( int i = 0, len = arrStyle.Length; i < len; i++ )
        {
          string strStyleProperty = arrStyle[ i ];
          int iValueIndex = strStyleProperty.IndexOf( ':' );

          if( iValueIndex >= 0 )
          {
            string strPropertyName = strStyleProperty.Substring( 0, iValueIndex ).Trim();
            string strPropertyValue = strStyleProperty.Substring( iValueIndex + 1,
              strStyleProperty.Length - iValueIndex - 1 ).Trim();
            if (!dictProperties.ContainsKey(strPropertyName)) 
            dictProperties.Add( strPropertyName, strPropertyValue );
          }
        }
      }

      return dictProperties;
    }
  }
}
