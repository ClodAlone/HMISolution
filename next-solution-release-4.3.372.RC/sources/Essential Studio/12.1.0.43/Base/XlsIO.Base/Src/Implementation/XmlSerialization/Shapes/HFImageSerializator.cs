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
#if ( WINRT )
using Windows.UI;
using Syncfusion.XlsIO;
#endif

#if  (SILVERLIGHT || WP)
using System.Windows.Media;
using System.Drawing;
#elif !(WINRT )
using System.Drawing;
using System.Drawing.Imaging;
#endif

namespace Syncfusion.XlsIO.Implementation.XmlSerialization.Shapes
{
  /// <summary>
  /// This class is responsible for header/footer image serialization.
  /// </summary>
  public class HFImageSerializator : ShapeSerializator
  {
    #region Statis members
    /// <summary>
    /// String that formulas for HF images.
    /// </summary>
    private static string[] s_arrFormulas = new string[]
    {
      "if lineDrawn pixelLineWidth 0", 
      "sum @0 1 0", 
      "sum 0 0 @1", 
      "prod @2 1 2", 
      "prod @3 21600 pixelWidth", 
      "prod @3 21600 pixelHeight", 
      "sum @0 0 1", 
      "prod @6 1 2", 
      "prod @7 21600 pixelWidth", 
      "sum @8 21600 0", 
      "prod @7 21600 pixelHeight", 
      "sum @10 21600 0", 
    };
    #endregion

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
      //shape.PrepareForSerialization();
      string strShapeType = '#' + string.Format( Vml.ShapeTypeIdFormat, shape.InnerSpRecord.Instance );

      writer.WriteStartElement( Vml.ShapeTagName, Vml.VNamespace );

      // TODO: Maybe we need to serialize spid too here.
      writer.WriteAttributeString( Vml.ShapeIdAttributeName, shape.Name );
      writer.WriteAttributeString( Vml.TypeAttributeName, strShapeType );

      BitmapShapeImpl bitmap = ( BitmapShapeImpl )shape;
      //Image picture = bitmap.Picture;
      PageSetupBaseImpl setup = bitmap.Worksheet.PageSetupBase;

      int iWidth = GetWidth( bitmap );
      int iHeight = GetHeight( bitmap );
      string strStyle;
      if (bitmap.PreserveStyleString != null && bitmap.PreserveStyleString.Length > 0)
          strStyle = bitmap.PreserveStyleString;
      else
      {
          strStyle = string.Format(System.Globalization.CultureInfo.InvariantCulture.NumberFormat, "width:{0}pt;height:{1}pt",
           ApplicationImpl.ConvertFromPixel(iWidth, MeasureUnits.Point),
           ApplicationImpl.ConvertFromPixel(iHeight, MeasureUnits.Point));
      }
      writer.WriteAttributeString( Vml.StyleAttribute, strStyle );

      SerializeImageData( writer, shape, holder, null, false, holder.HFDrawingsRelations );

      writer.WriteEndElement();
    }
    /// <summary>
    /// Gets width of the shape.
    /// </summary>
    /// <param name="bitmap"></param>
    /// <returns></returns>
    protected virtual int GetWidth( BitmapShapeImpl bitmap )
    {
      return bitmap.LeftColumn;
    }
    /// <summary>
    /// Gets height of the shape.
    /// </summary>
    /// <param name="bitmap"></param>
    /// <returns></returns>
    protected virtual int GetHeight( BitmapShapeImpl bitmap )
    {
      return bitmap.TopRow;
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

      /*
      - <v:shapetype id="_x0000_t75" coordsize="21600,21600" o:spt="75" o:preferrelative="t" path="m@4@5l@4@11@9@11@9@5xe" filled="f" stroked="f">
        <v:stroke joinstyle="miter" /> 
      - <v:formulas>
        <v:f eqn="if lineDrawn pixelLineWidth 0" /> 
        <v:f eqn="sum @0 1 0" /> 
        <v:f eqn="sum 0 0 @1" /> 
        <v:f eqn="prod @2 1 2" /> 
        <v:f eqn="prod @3 21600 pixelWidth" /> 
        <v:f eqn="prod @3 21600 pixelHeight" /> 
        <v:f eqn="sum @0 0 1" /> 
        <v:f eqn="prod @6 1 2" /> 
        <v:f eqn="prod @7 21600 pixelWidth" /> 
        <v:f eqn="sum @8 21600 0" /> 
        <v:f eqn="prod @7 21600 pixelHeight" /> 
        <v:f eqn="sum @10 21600 0" /> 
        </v:formulas>
        <v:path o:extrusionok="f" gradientshapeok="t" o:connecttype="rect" /> 
        <o:lock v:ext="edit" aspectratio="t" /> 
        </v:shapetype>

       */
      writer.WriteStartElement( Vml.ShapeTypeTagName, Vml.VNamespace );
      string strShapeTypeId = string.Format( Vml.ShapeTypeIdFormat, BitmapShapeImpl.ShapeInstance );
      writer.WriteAttributeString( Vml.ShapeIdAttributeName, strShapeTypeId );

      // TODO: find out what does this mean or/and move into constants.
      writer.WriteAttributeString( Vml.CoordSizeAttributeName, Vml.CommentCoordSize );
      writer.WriteAttributeString( Vml.SptAttriubteName, Vml.ONamespace,
        BitmapShapeImpl.ShapeInstance.ToString() );

      writer.WriteAttributeString( Vml.PreferRelative, Vml.ONamespace, "t" );
      writer.WriteAttributeString( Vml.PathAttributeName, Vml.BitmapPathValue );
      writer.WriteAttributeString( Vml.FilledAttribute, "f" );
      writer.WriteAttributeString( Vml.StrokedAttribute, "f" );
      // TODO: Add some additional values if they are necessary.
      writer.WriteStartElement( Vml.Stroke, Vml.VNamespace );
      writer.WriteAttributeString( Vml.JoinStyle, "miter" );
      writer.WriteEndElement();

      writer.WriteStartElement( Vml.FormulasTagName, Vml.VNamespace );

      for( int i = 0, len = s_arrFormulas.Length; i < len; i++ )
      {
        writer.WriteStartElement( Vml.SingleFormulaTagName, Vml.VNamespace );
        writer.WriteAttributeString( Vml.EquationTagName, s_arrFormulas[ i ] );
        writer.WriteEndElement();
      }

      writer.WriteEndElement();

      writer.WriteStartElement( Vml.ShapePathTagName, Vml.VNamespace );
      writer.WriteAttributeString( Vml.Extrusionok, Vml.ONamespace, "f" );
      writer.WriteAttributeString( Vml.GradientShapeOk, "t" );
      writer.WriteAttributeString( Vml.ConnectType, Vml.ONamespace, "rect" );
      writer.WriteEndElement();

      writer.WriteStartElement( Vml.Lock, Vml.ONamespace );
      writer.WriteAttributeString( Vml.Ext, Vml.VNamespace, "edit" );
      writer.WriteAttributeString( Vml.AspectRatio, "t" );
      writer.WriteEndElement();

      writer.WriteEndElement();
      //throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Serializes imageData xml tag.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="shape">Shape to get data to serialize from.</param>
    /// <param name="holder">Parent worksheet data holder.</param>
    protected  void SerializeImageData( XmlWriter writer, ShapeImpl shape, WorksheetDataHolder holder,
      string title, bool useRawFormat, RelationCollection relations )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( shape == null )
        throw new ArgumentNullException( "shape" );

      writer.WriteStartElement( Vml.ImageDataTag, Vml.VNamespace );
      string strRelationId = SerializePicture( shape, holder, useRawFormat, relations );
      writer.WriteAttributeString( Vml.RelationId, Vml.ONamespace, strRelationId );

      if( title != null )
        writer.WriteAttributeString( "title", Vml.ONamespace, title );

      writer.WriteEndElement();
    }
    /// <summary>
    /// This method is used to serialize picture item.
    /// </summary>
    /// <param name="shape">Shape to serialize.</param>
    /// <returns>String value containing relation id.</returns>
    /// <param name="holder">Parent worksheet data holder.</param>
    /// <returns>Relation id for the serialized shape</returns>
    protected virtual string SerializePicture( ShapeImpl shape, WorksheetDataHolder holder,
      bool useRawFormat, RelationCollection relations )
    {
      if( shape == null )
        throw new ArgumentNullException( "shape" );

      BitmapShapeImpl picture = shape as BitmapShapeImpl;
      Image image = picture.Picture;

      ImageFormat outputFormat =
#if !SILVERLIGHT && !WINRT && !WP
        useRawFormat ? image.RawFormat : ImageFormat.Png;
#else
        image.RawFormat;
#endif

      FileDataHolder fileHolder = holder.ParentHolder;

      string strExtension = holder.ParentHolder.RegisterContentTypes( outputFormat );

      //RelationCollection relations = holder.HFDrawingsRelations;
      string strItemName = fileHolder.SaveImage( image, outputFormat, null );
      string strRelationId = relations.GenerateRelationId();
      relations[ strRelationId ] = new Relation( '/' + strItemName, RelationTypes.Image );
      return strRelationId;
    }
    /// <summary>
    /// This method is used to serialize picture item.
    /// </summary>
    /// <param name="shape">Shape to serialize.</param>
    /// <returns>String value containing relation id.</returns>
    /// <param name="holder">Parent worksheet data holder.</param>
    /// <returns>Relation id for the serialized shape</returns>
    private string SerializePicture( ShapeImpl shape, WorksheetDataHolder holder )
    {
      if( shape == null )
        throw new ArgumentNullException( "shape" );

      return SerializePicture( shape, holder, false, holder.HFDrawingsRelations );
    }
  }
}
