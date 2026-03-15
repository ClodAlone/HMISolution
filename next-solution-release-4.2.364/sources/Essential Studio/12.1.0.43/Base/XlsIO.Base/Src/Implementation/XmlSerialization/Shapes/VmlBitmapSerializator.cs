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
using System.Xml;
#if ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;
#endif

#if !(SILVERLIGHT) && !(WINRT) && !(WP)
using System.Drawing.Imaging;
#endif

namespace Syncfusion.XlsIO.Implementation.XmlSerialization.Shapes
{
  public class VmlBitmapSerializator : HFImageSerializator
  {
    public override void Serialize( XmlWriter writer, ShapeImpl shape, WorksheetDataHolder holder,
      RelationCollection vmlRelations )
    {
 // <v:shape id="_x0000_s1026" type="#_x0000_t75" style='position:absolute;
 //   margin-left:204pt;margin-top:122.25pt;width:630.75pt;height:446.25pt;
 //   z-index:2' filled="t" fillcolor="window [65]" stroked="t" strokecolor="windowText [64]"
 //   o:insetmode="auto">
 //   <v:fill color2="window [65]"/>
 //   <v:imagedata o:relid="rId1" o:title=""/>
 //   <x:ClientData ObjectType="Pict">
 //    <x:SizeWithCells/>
 //    <x:Anchor>
 //     4, 16, 8, 3, 17, 25, 37, 18</x:Anchor>
 //    <x:CF>Pict</x:CF>
 //    <x:AutoPict/>
 //   </x:ClientData>
 //</v:shape>
      //base.Serialize( writer, shape, holder );
      writer.WriteStartElement( Vml.ShapeTagName, Vml.VNamespace );
      string strShapeType = '#' + string.Format( Vml.ShapeTypeIdFormat, shape.InnerSpRecord.Instance );
      string strShapeId = string.Format( Vml.ShapeIdFormat, shape.ShapeId );

      writer.WriteAttributeString( Vml.ShapeIdAttributeName, strShapeId );
      writer.WriteAttributeString( Vml.TypeAttributeName, strShapeType );

      List<string> arrStyleProperties = new List<string>();
      PrepareStyleProperties( arrStyleProperties, shape );
      VmlTextBoxBaseSerializator.SerializeStyle( writer, arrStyleProperties );

      writer.WriteAttributeString( "filled", "t" );
      writer.WriteAttributeString( "fillcolor", "window [65]" );
      if(shape.HasBorder)
          writer.WriteAttributeString( Vml.StrokedAttribute,"t");
      writer.WriteAttributeString( "strokecolor", "windowText [64]" );
      writer.WriteAttributeString( "o:insetmode", "auto" );
      //SerializeShapeStyle( writer, shape );

      //<v:fill color2="window [65]"/>
      writer.WriteStartElement( "v", "fill", null );
      writer.WriteAttributeString( "color2", "window [65]" );
      writer.WriteEndElement();

      SerializeImageData( writer, shape, holder, string.Empty, true, vmlRelations );
      SerializeClientData( writer, shape, "Pict" );
      writer.WriteEndElement();
    }
    protected override void SerializeClientDataAdditional( XmlWriter writer, ShapeImpl shape )
    {
      base.SerializeClientDataAdditional( writer, shape );

      writer.WriteElementString( "CF", Vml.XNamespace, "Pict" );
      writer.WriteElementString( "AutoPict", Vml.XNamespace, string.Empty );

      BitmapShapeImpl bitmap = shape as BitmapShapeImpl;

      if( bitmap.IsDDE )
        writer.WriteElementString( "DDE", Vml.XNamespace, null );

      if( bitmap.IsCamera )
        writer.WriteElementString( "Camera", Vml.XNamespace, null );
    }
    private void PrepareStyleProperties( List<string> styleProperties, ShapeImpl shape )
    {
      styleProperties.Add( "position:absolute" );

      AddMeasurement( styleProperties, Vml.MarginLeft, shape.Left );
      AddMeasurement( styleProperties, Vml.MarginTop, shape.Top );
      AddMeasurement( styleProperties, Vml.Width, shape.Width );
      AddMeasurement( styleProperties, Vml.Height, shape.Height );
      //styleProperties.Add( " position:absolute" );
    }

    private void AddMeasurement( List<string> styleProperties, string tagName, int size )
    {
      const string dimensionFormat = "{0}:{1}pt";
      double sizeInPoints = Math.Round( ApplicationImpl.ConvertFromPixel( size, MeasureUnits.Point ), 2 );
      styleProperties.Add( string.Format( dimensionFormat, tagName, sizeInPoints ) );
    }
    /// <summary>
    /// This method is used to serialize picture item.
    /// </summary>
    /// <param name="shape">Shape to serialize.</param>
    /// <returns>String value containing relation id.</returns>
    /// <param name="holder">Parent worksheet data holder.</param>
    /// <returns>Relation id for the serialized shape</returns>
    protected override string SerializePicture( ShapeImpl shape, WorksheetDataHolder holder,
      bool useRawFormat, RelationCollection relations )
    {
      if( shape == null )
        throw new ArgumentNullException( "shape" );

      BitmapShapeImpl picture = shape as BitmapShapeImpl;
      Image image = picture.Picture;
      int imageIndex = ( int )picture.BlipId;

      ImageFormat outputFormat =
#if !SILVERLIGHT && !WINRT && !WP
 useRawFormat ? image.RawFormat : ImageFormat.Png;
#else
        image.RawFormat;
#endif

      FileDataHolder fileHolder = holder.ParentHolder;

      string strExtension = holder.ParentHolder.RegisterContentTypes( outputFormat );

      //RelationCollection relations = holder.HFDrawingsRelations;
      string strItemName = fileHolder.GetImageItemName( imageIndex - 1 );//SaveImage( image, outputFormat, null );
      string strRelationId = relations.GenerateRelationId();
      relations[ strRelationId ] = new Relation( '/' + strItemName, RelationTypes.Image );
      return strRelationId;
    }
  }
}
