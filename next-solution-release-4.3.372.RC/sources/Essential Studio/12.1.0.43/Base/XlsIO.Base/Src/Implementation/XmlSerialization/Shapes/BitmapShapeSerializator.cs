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
#if ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;
#endif
using System.IO;

using Syncfusion.XlsIO.Implementation.Shapes;
using Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing;
using Syncfusion.XlsIO.Implementation.XmlSerialization.Constants;

#if !SILVERLIGHT && !WINRT && !WP
using System.Drawing.Imaging;
#endif


namespace Syncfusion.XlsIO.Implementation.XmlSerialization.Shapes
{
  /// <summary>
  /// This class is used for picture shapes serialization.
  /// </summary>
  public class BitmapShapeSerializator : DrawingShapeSerializator
  {
    #region Methods
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

      BitmapShapeImpl picture = shape as BitmapShapeImpl;

      if( picture == null )
        throw new ArgumentOutOfRangeException( "picture" );

      FileDataHolder fileHolder = holder.ParentHolder;

      ////string strExtension = RegisterContentTypes( fileHolder.DefaultContentTypes, picture );
      string strRelationId = SerializePictureFile( holder, picture, vmlRelations );

      bool bRelative = shape.Worksheet as WorksheetImpl == null;

      string mainNamespace;
      string startTag;
      
      if( bRelative )
      {
        startTag = ChartConstants.RelativeSizeAnchorTag;
        mainNamespace = Drawings.CdrNamespace;
      }
      else
      {
        startTag = Drawings.TwoCellAnchorTagName;
        mainNamespace = Drawings.XdrNamespace;
      }


      writer.WriteStartElement( startTag, mainNamespace );

      if( !bRelative )
        writer.WriteAttributeString( Drawings.EditAsAttribute, GetEditAsValue( picture ) );

      SerializeAnchorPoint( writer, Drawings.FromTagName,
        shape.LeftColumn, shape.LeftColumnOffset,
        shape.TopRow, shape.TopRowOffset, shape.Worksheet,
        mainNamespace );

      SerializeAnchorPoint( writer, Drawings.ToTagName,
        shape.RightColumn, shape.RightColumnOffset,
        shape.BottomRow, shape.BottomRowOffset, shape.Worksheet,
        mainNamespace );

      SerializePicture( writer, picture, strRelationId, holder, mainNamespace );

      writer.WriteEndElement();
      //throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Serializes picture.
    /// </summary>
    /// <param name="writer">Writer to serialize into.</param>
    /// <param name="picture">Picture to serialize.</param>
    /// <param name="relationId">Relation id of the picture file.</param>
    /// <param name="holder">Object that stores data of the parent worksheet.</param>
    private void SerializePicture( XmlWriter writer, BitmapShapeImpl picture,
      string relationId, WorksheetDataHolder holder, string mainNamespace )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( picture == null )
        throw new ArgumentNullException( "picture" );

      if( relationId == null || relationId.Length == 0 )
        throw new ArgumentOutOfRangeException( "relationId" );

      writer.WriteStartElement( Drawings.PictureTagName, mainNamespace );

      string strMacro = picture.Macro;

      if( strMacro != null )
        writer.WriteAttributeString( Drawings.MacroAttribute, strMacro );

      SerializeNonVisualProperties( writer, picture, holder, mainNamespace );
      SerializeBlipFill( writer, picture, relationId, mainNamespace );
      SerializeShapeProperties( writer, picture, mainNamespace );
      writer.WriteEndElement();

      if( mainNamespace == Drawings.XdrNamespace )
      {
        writer.WriteStartElement( Drawings.ClientDataTagName, Drawings.XdrNamespace );
        writer.WriteEndElement();
      }

    }
    /// <summary>
    /// This method serializes shape properties.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="picture">Picture to get properties from.</param>
    private void SerializeShapeProperties( XmlWriter writer, BitmapShapeImpl picture, string mainNamespace )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( picture == null )
        throw new ArgumentNullException( "picture" );

      if( picture.ShapePropertiesStream != null )
      {
        Excel2007Serializator.SerializeStream( writer, picture.ShapePropertiesStream, null );
      }
      else
      {
        writer.WriteStartElement( Drawings.ShapePropertiesTag, mainNamespace );
        SerializeForm( writer, Drawings.ANamespace, Drawings.ANamespace, 0, 1, 2076450, 1557338,picture as IShape );
        //writer.WriteStartElement( Drawings.Transform2DTag, Drawings.ANamespace );
        //writer.WriteStartElement( Drawings.Offset, Drawings.ANamespace );
        //// TODO: change this.
        //writer.WriteAttributeString( Drawings.XAttributeName, "0" );
        //writer.WriteAttributeString( Drawings.YAttributeName, "1" );
        //writer.WriteEndElement();

        //writer.WriteStartElement( Drawings.Extents, Drawings.ANamespace );
        //// TODO: change this.
        //writer.WriteAttributeString( Drawings.CXAttributeName, "2076450" );
        //writer.WriteAttributeString( Drawings.CYAttributeName, "1557338" );
        //writer.WriteEndElement();

        //writer.WriteEndElement();

        SerializePresetGeometry( writer );

        writer.WriteEndElement();
      }
    }
    /// <summary>
    /// Serializes non visual properties.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="picture">Picture to serialize.</param>
    /// <param name="holder">Object that stores data of the parent worksheet.</param>
    private void SerializeNonVisualProperties( XmlWriter writer, BitmapShapeImpl picture,
      WorksheetDataHolder holder, string mainNamespace )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( picture == null )
        throw new ArgumentNullException( "picture" );

      writer.WriteStartElement( Drawings.NVPicturePropertiesTag, mainNamespace );
      SerializeNVCanvasProperties( writer, picture, holder, mainNamespace );
      SerializeNVPictureCanvasProperties( writer, picture, mainNamespace );

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes non visual picture canvas properties.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="picture">Picture to serialize.</param>
    private void SerializeNVPictureCanvasProperties( XmlWriter writer, BitmapShapeImpl picture,
      string mainNamespace )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( picture == null )
        throw new ArgumentNullException( "picture" );

      writer.WriteStartElement( Drawings.NVPictureCanvasPropertiesTag, mainNamespace );
      writer.WriteStartElement( Drawings.PictureLocksTag, Drawings.ANamespace );
      writer.WriteAttributeString( Drawings.NoChangeAspectAttribute, "1" );
      writer.WriteEndElement();
      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes blip fill.
    /// </summary>
    /// <param name="writer">XmlWriter to save blip fill into.</param>
    /// <param name="picture">Picture to get blip fill options from.</param>
    /// <param name="relationId">Relation id of the picture zip archive item.</param>
    private void SerializeBlipFill( XmlWriter writer, BitmapShapeImpl picture, string relationId,
      string mainNamespace )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( picture == null )
        throw new ArgumentNullException( "picture" );

      if( relationId == null || relationId.Length == 0 )
        throw new ArgumentOutOfRangeException( "relationId" );

      writer.WriteStartElement( Drawings.BlipFillTagName, mainNamespace );

      writer.WriteStartElement( Drawings.BlipTagName, Drawings.ANamespace );
      writer.WriteAttributeString( Drawings.EmbeddedPicture, Excel2007Serializator.RelationNamespace,
        relationId );

      if( picture.BlipSubNodesStream != null )
      {
        Excel2007Serializator.SerializeStream( writer, picture.BlipSubNodesStream,
          Excel2007Serializator.TemporaryRoot );
      }

      writer.WriteEndElement();

      if (picture.SourceRectStream != null)
      {
          Excel2007Serializator.SerializeStream(writer, picture.SourceRectStream,
            Excel2007Serializator.TemporaryRoot);
      }
      else
      {
          writer.WriteStartElement(Drawings.SourceRectangleTagName, Drawings.ANamespace);

          if(picture.CropBottomOffset !=0)

          writer.WriteAttributeString(Drawings.BottomAttribute,picture.CropBottomOffset.ToString());
          if(picture.CropLeftOffset!=0)
              writer.WriteAttributeString(Drawings.LeftAttribute,picture.CropLeftOffset.ToString());
          if(picture.CropRightOffset!=0)
              writer.WriteAttributeString(Drawings.RightAttribute,picture.CropRightOffset.ToString());
          if(picture.CropTopOffset!=0)
              writer.WriteAttributeString(Drawings.TopAttribute,picture.CropTopOffset.ToString());

              writer.WriteEndElement();
      }

      writer.WriteStartElement( Drawings.StretchTagName, Drawings.ANamespace );
      writer.WriteElementString( Drawings.FillRectTagName, Drawings.ANamespace, string.Empty );
      writer.WriteEndElement();

      writer.WriteEndElement();
    }
    /// <summary>
    /// This method serializes general shape settings (shape type) into specified XmlWriter.
    /// </summary>
    /// <param name="writer">XmlWriter to write shape type into.</param>
    /// <param name="shapeType">Type of the shape that is going to be serialized.</param>
    public override void SerializeShapeType( XmlWriter writer, Type shapeType )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Serializes picture file and creates necessary relations.
    /// </summary>
    /// <param name="holder">Object that holds file data.</param>
    /// <param name="picture">Picture to save into file.</param>
    /// <returns>Relation id of the generated picture item.</returns>
    public string SerializePictureFile( WorksheetDataHolder holder, BitmapShapeImpl picture,
      RelationCollection vmlRelations )
    {
      if( holder == null )
        throw new ArgumentNullException( "holder" );

      if( picture == null )
        throw new ArgumentNullException( "picture" );

      RelationCollection drawingsRelations = holder.DrawingsRelations;
      string strItemName = '/' + holder.ParentHolder.GetImageItemName( ( int )picture.BlipId - 1 );
      string strRelationId = drawingsRelations.FindRelationByTarget( strItemName );

      if( strRelationId == null )
      {
        strRelationId = drawingsRelations.GenerateRelationId();

        // TODO: maybe we should correct item name.
        drawingsRelations[ strRelationId ] = new Relation( strItemName, RelationTypes.Image );
      }

      return strRelationId;
    }
    #endregion
  }
}
