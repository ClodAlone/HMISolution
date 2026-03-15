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
using Syncfusion.XlsIO.Implementation.XmlSerialization.Constants;
using Syncfusion.XlsIO.Implementation.XmlSerialization;
using Syncfusion.XlsIO.Implementation.Collections;

#if ( WINRT )
using Windows.UI;

#endif

#if  (SILVERLIGHT || WP)
using System.Windows.Media;
using System.Drawing;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;

#endif

namespace Syncfusion.XlsIO.Implementation.XmlReaders.Shapes
{
  /// <summary>
  /// This class is used for header/footer images parsing.
  /// </summary>
  public class HFImageParser : ShapeParser
  {
    /// <summary>
    /// Extracts shape type settings from the reader and creates shape with default settings.
    /// </summary>
    /// <param name="reader">XmlReader to get general shape settings from.</param>
    /// <param name="parent">Parent worksheet for the shape.</param>
    /// <returns>
    /// Shape with default settings without adding it to any collection
    /// </returns>
    public override ShapeImpl ParseShapeType( XmlReader reader, ShapeCollectionBase shapes )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( shapes == null )
        throw new ArgumentNullException( "shapes" );

      // This parser can only parse bitmap shapes and we don't support anything
      // from shape type, so we simply create empty bitmap shape and skip this tag.
      reader.Skip();

      BitmapShapeImpl result = new BitmapShapeImpl( shapes.Application, shapes );
      result.VmlShape = true;
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

      if( defaultShape == null )
        throw new ArgumentNullException( "defaultShape" );

      if( reader.LocalName != Vml.ShapeTagName )
        throw new XmlException( "Unexpected xml tag." );

      string strShapeId = null;
      int shapeId = -1;

      if( reader.MoveToAttribute( Vml.SpIdAttributeName, Vml.ONamespace ) )
      {
        strShapeId = reader.Value;
        shapeId = ParseShapeId( strShapeId );
      }

      if( reader.MoveToAttribute( Vml.ShapeIdAttributeName ) )
      {
        strShapeId = reader.Value;

        if( shapeId == -1 )
          shapeId = ParseShapeId( strShapeId );
      }
      BitmapShapeImpl result = ( BitmapShapeImpl )defaultShape.Clone( defaultShape.Parent,
        null, null, false );

      if (reader.MoveToAttribute(Vml.StrokedAttribute))
          result.HasBorder = true;
      else
          result.HasBorder = false;

      if (reader.MoveToAttribute(Vml.StyleAttribute))
          ParseStyle(reader, result);

      reader.Read();
      if( shapeId != -1 )
      {
          result.ShapeId = shapeId;
          result.Name = strShapeId;
      }


      // We don't support most of the shape settings, so we simply skip till image data.
      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case Vml.ImageDataTag:
              ParseImageData( reader, strShapeId, result, relations,
                parentItemPath );
              break;

            case Vml.ClientDataTagName:
              ParseClientData( reader, result );
              break;

            default:
              reader.Skip();
              break;
          }
        }
        else
        {
          reader.Read();
        }
      }

      reader.Read();
      return true;
    }
    /// <summary>
    /// Parses style attribute of the comment shape.
    /// </summary>
    /// <param name="reader">Reader to get attribute data from.</param>
    /// <param name="textBox">Comment shape to set values to.</param>
    private void ParseStyle(XmlReader reader, BitmapShapeImpl result)
    {
        if (result == null)
            throw new ArgumentNullException("reader");

        if (result == null)
            throw new ArgumentNullException("textBox");

        result.PreserveStyleString = reader.Value;
       
    }
    /// <summary>
    /// Parses style properties.
    /// </summary>
    /// <param name="textBox">Textbox to put properties into.</param>
    /// <param name="styleProperties">String representation of the style properties
    /// (key - property name, value - property value).</param>
    protected virtual void ParseStyle(BitmapShapeImpl result, Dictionary<string, string> styleProperties)
    {
        string height;
        string width;

        if (styleProperties.TryGetValue(Vml.Height, out height))
        {
            result.Height =(int)result.AppImplementation.ConvertUnits(Convert.ToDouble(height.Substring(0,height.Length - 2)),MeasureUnits.Point,MeasureUnits.Pixel);
        }
        if (styleProperties.TryGetValue(Vml.Width, out width))
        {
            result.Width = (int)result.AppImplementation.ConvertUnits(Convert.ToDouble(width.Substring(0, width.Length - 2)), MeasureUnits.Point, MeasureUnits.Pixel);
        }
    }
    /// <summary>
    /// Parses shape id
    /// </summary>
    /// <returns>Extracted shapeid or -1 if format was incorrect.</returns>
    private int ParseShapeId( string shapeId )
    {
      int index = shapeId.IndexOf( "_s" );
      int result = -1;

      if( index >= 0 )
      {
        string id = shapeId.Substring( index + 2 );

        if( int.TryParse( id, out index ) )
        {
          result = index;
        }

      }

      return result;
    }
    private void ParseClientData( XmlReader reader, BitmapShapeImpl shape )
    {
      if( reader.LocalName != Vml.ClientDataTagName )
        throw new XmlException( "Unexpected xml token" );

      if( shape == null )
        throw new ArgumentNullException( "shape" );

      reader.Read();
      shape.IsMoveWithCell = true;
      shape.IsSizeWithCell = true;

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case Vml.MoveWithCellsTagName:
              shape.IsMoveWithCell = VmlTextBoxBaseParser.ParseBoolOrEmpty( reader, true );
              break;

            case Vml.SizeWithCellsTagName:
              shape.IsSizeWithCell = VmlTextBoxBaseParser.ParseBoolOrEmpty( reader, true );
              break;

            case Vml.AnchorTagName:
              ParseAnchor( reader, shape );
              break;

            case "DDE":
              shape.IsDDE = true;
              reader.Read();
              break;

            case "Camera":
              shape.IsCamera = true;
              reader.Read();
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

      reader.Read();
    }
    /// <summary>
    /// Parses image data.
    /// </summary>
    /// <param name="reader">XmlReader to get required information from.</param>
    /// <param name="shapeName">Name of the new shape.</param>
    /// <param name="sheet">Parent worksheet to place new shape into.</param>
    /// <param name="relations">Corresponding relations collection.</param>
    /// <param name="parentItemPath">Path to the parent item.</param>
    private void ParseImageData( XmlReader reader, string shapeName, BitmapShapeImpl shape,
      RelationCollection relations, string parentItemPath )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( shape == null )
        throw new ArgumentNullException( "shape" );

      if( shapeName == null || shapeName.Length == 0 )
        throw new ArgumentOutOfRangeException( "shapeName" );

      if( parentItemPath == null || parentItemPath.Length == 0 )
        throw new ArgumentOutOfRangeException( "parentItemPath" );

      if( relations == null )
        throw new ArgumentNullException( "relations" );

      if( reader.LocalName != Vml.ImageDataTag )
        throw new XmlException( "Unexpected xml tag." );

      if( reader.MoveToAttribute( Vml.RelationId, Vml.ONamespace ) )
      {
        string strRelatinId = reader.Value;
        // Here we have to 
        // 1. get Image item from the archive.
        Relation relation = relations[ strRelatinId ];

        if( relation == null )
          throw new XmlException( "Cannot find required relation." );

        WorksheetBaseImpl sheet = shape.Worksheet;
        string strFullPath = FileDataHolder.CombinePath( parentItemPath, relation.Target );
        Image picture = sheet.DataHolder.ParentHolder.GetImage( strFullPath );
        // 2. Create picture item using such code

        if( shape.ParentShapes is HeaderFooterShapeCollection )
        {
          sheet.HeaderFooterShapes.SetPicture( shapeName, picture, -1, false,shape.PreserveStyleString );
        }
        else
        {
          shape.Picture = picture;
          sheet.InnerShapes.Add( shape );
        }
      }
      else
      {
        throw new XmlException( "Wrong xml format." );
      }

      reader.Skip();
    }
  }
}
