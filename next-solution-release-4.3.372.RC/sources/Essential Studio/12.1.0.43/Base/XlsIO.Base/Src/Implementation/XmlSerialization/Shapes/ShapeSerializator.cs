#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.IO;
using System.Collections.Generic;
using System.Xml;
using System.Resources;
#if ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;
#endif

using Syncfusion.XlsIO.Implementation.XmlReaders.Shapes;
using Syncfusion.XlsIO.Implementation.Shapes;
using Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing;

#if ( WINRT )
using Windows.UI;
using Syncfusion.XlsIO.Implementation.WINRT;
using System.Reflection;
#endif

#if  (SILVERLIGHT)
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.Silverlight;
#elif WP
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.WP;
#endif

namespace Syncfusion.XlsIO.Implementation.XmlSerialization.Shapes
{
  /// <summary>
  /// This is general interface for classes that are responsible for shape serialization.
  /// </summary>
  public abstract class ShapeSerializator
  {
    #region Constants
    public const string FalseAttributeValue = "f";
    public const string TrueAttributeValue = "t";
    #endregion

    #region Methods
    /// <summary>
    /// This method serializes specified shape into specified writer.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize shape settings into.</param>
    /// <param name="shape">Shape to serialize.</param>
    /// <param name="holder">Parent worksheet data holder.</param>
    public abstract void Serialize( XmlWriter writer, ShapeImpl shape, WorksheetDataHolder holder,
      RelationCollection relations );
    /// <summary>
    /// This method serializes general shape settings (shape type) into specified XmlWriter.
    /// </summary>
    /// <param name="writer">XmlWriter to write shape type into.</param>
    /// <param name="shapeType">Type of the shape that is going to be serialized.</param>
    public abstract void SerializeShapeType( XmlWriter writer, Type shapeType );
    /// <summary>
    /// Generates anchor from the shape and converts it into string representation for Excel 2007 format.
    /// </summary>
    /// <param name="shape">Shape to get anchor data from.</param>
    /// <returns>Generated anchor string.</returns>
    protected static string GetAnchorValue( ShapeImpl shape )
    {
      MsofbtClientAnchor anchor = shape.ClientAnchor;
      int iColumn = anchor.LeftColumn;
      int iOffset = shape.OffsetInPixels( iColumn + 1, anchor.LeftOffset, true );
      string strLeft = iColumn.ToString();
      string strLeftOffset = iOffset.ToString();

      iColumn = anchor.RightColumn;
      iOffset = shape.OffsetInPixels( iColumn + 1, anchor.RightOffset, true );
      string strRight = iColumn.ToString();
      string strRightOffset = iOffset.ToString();

      iColumn = anchor.TopRow;
      iOffset = shape.OffsetInPixels( iColumn + 1, anchor.TopOffset, false );
      string strTop = iColumn.ToString();
      string strTopOffset = iOffset.ToString();

      iColumn = anchor.BottomRow;
      iOffset = shape.OffsetInPixels( iColumn + 1, anchor.BottomOffset, false );
      string strBottom = iColumn.ToString();
      string strBottomOffset = iOffset.ToString();

      string strResult = string.Join( ", ", new string[]{ strLeft, strLeftOffset,
        strTop, strTopOffset, strRight, strRightOffset, strBottom, strBottomOffset } );

      return strResult;
    }
    /// <summary>
    /// Serializes client data tag of the shape.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="shape">Shape to serialize.</param>
    protected void SerializeClientData( XmlWriter writer, ShapeImpl shape, string shapeType )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( shape == null )
        throw new ArgumentNullException( "shape" );

      writer.WriteStartElement( Vml.ClientDataTagName, Vml.XNamespace );
      writer.WriteAttributeString( Vml.ObjectTypeAttribute, shapeType );
    if(!shape.IsMoveWithCell )
      writer.WriteElementString( Vml.MoveWithCellsTagName, Vml.XNamespace,null  );
    if (!shape.IsSizeWithCell)
    {
        writer.WriteStartElement(Vml.SizeWithCellsTagName,Vml .XNamespace );
        writer.WriteEndElement();
    }
      string strAnchorValue = GetAnchorValue( shape );
      writer.WriteElementString( Vml.AnchorTagName, Vml.XNamespace, strAnchorValue );

      SerializeClientDataAdditional( writer, shape );

      //TODO: maybe we have to add some more values here.
      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes additional tag into ClientData tag.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="shape">Shape to serialize for.</param>
    protected virtual void SerializeClientDataAdditional( XmlWriter writer, ShapeImpl shape )
    {
    }
    /// <summary>
    /// Serialize the fill style of textbox
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="shape">Shape to serialize style for</param>
    /// <param name="holder">Parent worksheet data holder.</param>
    /// <param name="vmlRelations">Resource relation collection</param>
    protected virtual void SerializeFill( XmlWriter writer, ShapeImpl shape, WorksheetDataHolder holder,
      RelationCollection vmlRelations )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( shape == null )
        throw new ArgumentNullException( "textBox" );

      if( !shape.HasFill )
        return;

      TextBoxShapeBase textBox = shape as TextBoxShapeBase;
      FileDataHolder fileDataHolder;
      writer.WriteStartElement( Vml.FillTag, Vml.VNamespace ); //<fill>

      switch( textBox.Fill.FillType )
      {
        case ExcelFillType.SolidColor:
          SerializeSolidFill( writer, textBox );
          break;

        case ExcelFillType.Gradient:
          SerializeGradientFill( writer, textBox );
          break;

        case ExcelFillType.Texture:
          writer.WriteAttributeString( Vml.TypeAttributeName, Vml.TextureAttributeValue );
          fileDataHolder = holder.ParentHolder;
          SerializeTextureFill( writer, textBox, fileDataHolder, vmlRelations );
          SerializeFillCommon( writer, textBox );
          break;

        case ExcelFillType.Pattern:
          fileDataHolder = holder.ParentHolder;
          writer.WriteAttributeString( Vml.TypeAttributeName, Vml.PatternAttributeValue );
          writer.WriteAttributeString( Vml.Color, GenerateHexColor( textBox.Fill.BackColor ) );
          writer.WriteAttributeString( Vml.Color2Attribute, GenerateHexColor( textBox.Fill.ForeColor ) );
          SerializePatternFill( writer, textBox, fileDataHolder, vmlRelations );
          SerializeFillCommon( writer, textBox );
          break;

        case ExcelFillType.Picture:
          writer.WriteAttributeString( Vml.TypeAttributeName, Vml.PictureAttributeValue );
          fileDataHolder = holder.ParentHolder;
          SerializePictureFill( writer, textBox, fileDataHolder, vmlRelations );
          break;
      }

      writer.WriteEndElement(); //</fill>
    }
    /// <summary>
    /// Serialize Solid fill of textbox
    /// </summary>
    /// <param name="writer">Xmlwriter to serialize into</param>
    /// <param name="textBox">textbox shape for serialize</param>
    protected virtual void SerializeSolidFill( XmlWriter writer, TextBoxShapeBase textBox )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );
      if( textBox == null )
        throw new ArgumentNullException( "textBox" );


      if( !IsEmptyColor( textBox.Fill.ForeColor ) )
      {
        string color = GenerateHexColor( textBox.Fill.ForeColor );
        writer.WriteAttributeString( Vml.Color, color );
      }
      //  <v:fill opacity="43909f" color2="#fc0 [51]" recolor="t" rotate="t"/>
      SerializeFillCommon( writer, textBox );
    }
    /// <summary>
    /// Serialize the Gradient Fill
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="textBox">Shape to serialize style for</param>
    protected virtual void SerializeGradientFill( XmlWriter writer, TextBoxShapeBase textBox )
    {
      //<v:fill opacity="43909f" color2="#760000" o:opacity2="26214f" recolor="t"
      //   rotate="t" angle="-135" focusposition=".5,.5" focussize="" focus="100%"
      // type="gradient"/>

      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( textBox == null )
        throw new ArgumentNullException( "textBox" );

      switch( textBox.Fill.GradientColorType )
      {
        case ExcelGradientColor.OneColor:
          writer.WriteAttributeString( Vml.Color, GenerateHexColor( textBox.Fill.BackColor ) );
          writer.WriteAttributeString( Vml.Color2Attribute, PrepareGradientDegree( textBox.Fill.GradientDegree ) );
          break;

        case ExcelGradientColor.TwoColor:
          writer.WriteAttributeString( Vml.Color, GenerateHexColor( textBox.Fill.BackColor ) );
          writer.WriteAttributeString( Vml.Color2Attribute, GenerateHexColor( textBox.Fill.ForeColor ) );
          break;

        case ExcelGradientColor.Preset:
          writer.WriteAttributeString( Vml.MethodAttribute, Vml.MethodNoneValue );
          writer.WriteAttributeString( Vml.ColorsAttribute, GetPresetString( textBox.Fill.PresetGradientType ) );
          break;

      }

      SerializeGradientFillCommon( writer, textBox );
    }
    /// <summary>
    /// Serialize Texture Fill 
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="textBox">textBox Shape to serialize style for</param>
    /// <param name="holder">Parent worksheet data holder.</param>
    /// <param name="vmlRelations">Resource relation collection</param>
    protected virtual void SerializeTextureFill( XmlWriter writer, TextBoxShapeBase textBox, FileDataHolder holder,
      RelationCollection vmlRelations )
    {
      //<v:fill o:relid="rId1" o:title="Paper bag"

      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( textBox == null )
        throw new ArgumentNullException( "textBox" );

      if( holder == null )
        throw new ArgumentNullException( "holder" );

      if( vmlRelations == null )
        throw new ArgumentNullException( "relations" );

      Image image;
      ExcelTexture texture = textBox.Fill.Texture;

      if( texture != ExcelTexture.User_Defined )
      {
#if ( WINRT )
          ResourceHandler resource=new ResourceHandler();
          byte[] arrData = resource.TextureArray[ShapeFillImpl.DEF_TEXTURE_PREFIX + ((int)texture).ToString()];
#else
        byte[] arrData = ShapeFillImpl.GetResData( ShapeFillImpl.DEF_TEXTURE_PREFIX + ( ( int )texture ).ToString() );
#endif
        byte[] arrPicture = new byte[ arrData.Length - 25 ];

        Array.Copy( arrData, 25, arrPicture, 0, arrPicture.Length );
        MemoryStream ms = new MemoryStream();

        ShapeFillImpl.UpdateBitMapHederToStream( ms, arrData );
        ms.Write( arrPicture, 0, arrPicture.Length );

        image = Image.FromStream( ms, true, true);

        string imageName = texture.ToString();
        imageName = imageName.Replace( '_', ' ' ).Trim();
        string strLocation = holder.SaveImage( image, null );
        string strRelationId = vmlRelations.GenerateRelationId();
        vmlRelations[ strRelationId ] = new Relation( '/' + strLocation, RelationTypes.Image );
        writer.WriteAttributeString( Vml.RelationIDAttribute, Vml.ONamespace, strRelationId );
        writer.WriteAttributeString( Vml.TitleAttibute, Vml.ONamespace, imageName );
      }
      else // if Predefined Texture
      {
        image = textBox.Fill.Picture;
        string imageName = textBox.Fill.PictureName;

        textBox.Fill.UserTexture( image, imageName );
        SerializeUserPicture( writer, textBox, holder, vmlRelations );
      }

    }
    /// <summary>
    /// Serialize pattern Fill
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="textBox">textBox Shape to serialize style for</param>
    /// <param name="holder">Parent worksheet data holder.</param>
    /// <param name="vmlRelations">Resource relation collection</param>
    protected virtual void SerializePatternFill( XmlWriter writer, TextBoxShapeBase textBox, FileDataHolder holder,
      RelationCollection vmlRelations )
    {
      throw new NotImplementedException( "Pattern" );
    }
    /// <summary>
    /// Serialize Picture Fill
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="textBox">textBox Shape to serialize style for</param>
    /// <param name="holder">Parent worksheet data holder.</param>
    /// <param name="vmlRelations">Resource relation collection</param>
    protected virtual void SerializePictureFill( XmlWriter writer, TextBoxShapeBase textBox,
      FileDataHolder holder, RelationCollection relations )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( holder == null )
        throw new ArgumentNullException( "holder" );

      if( relations == null )
        throw new ArgumentNullException( "relations" );

      if( textBox == null )
        throw new ArgumentNullException( "textBox" );

      Image image = textBox.Fill.Picture;
      string imageName = textBox.Fill.PictureName;

      ExcelFillType oldFillType = textBox.Fill.FillType;
      textBox.Fill.FillType = ExcelFillType.SolidColor;
      writer.WriteAttributeString( Vml.OpacityAttribute, GetOpacityFormat( textBox.Fill.Transparency ) );
      textBox.Fill.UserPicture( image, imageName );
      SerializeUserPicture( writer, textBox, holder, relations );

    }
    /// <summary>
    /// Serialize user Picture
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="textBox ">textBox Shape to serialize style for</param>
    /// <param name="holder">Parent worksheet data holder.</param>
    /// <param name="vmlRelations">Resource relation collection</param>
    protected virtual void SerializeUserPicture( XmlWriter writer, TextBoxShapeBase textBox,
      FileDataHolder holder, RelationCollection relations )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( holder == null )
        throw new ArgumentNullException( "holder" );

      if( relations == null )
        throw new ArgumentNullException( "relations" );

      if( textBox == null )
        throw new ArgumentNullException( "textBox" );

      Image image = textBox.Fill.Picture;

      string imageName = textBox.Fill.PictureName;
      //image.Save( imageName, System.Drawing.Imaging.ImageFormat.Png );
      string strLocation = holder.SaveImage( image, null );
      string strRelationId = relations.GenerateRelationId();
      relations[ strRelationId ] = new Relation( '/' + strLocation, RelationTypes.Image );
      writer.WriteAttributeString( Vml.RelationIDAttribute, Vml.ONamespace, strRelationId );
      writer.WriteAttributeString( Vml.TitleAttibute, Vml.ONamespace, imageName );
    }
    /// <summary>
    /// Serilaize common Gradient fill attribute
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="textBox ">textBox Shape to serialize style for</param>
    protected virtual void SerializeGradientFillCommon( XmlWriter writer, TextBoxShapeBase textBox )
    {

      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( textBox == null )
        throw new ArgumentNullException( "textBox" );
      writer.WriteAttributeString( Vml.OpacityAttribute, GetOpacityFormat( textBox.Fill.TransparencyFrom ) );
      writer.WriteAttributeString( Vml.ReColorAttribute, TrueAttributeValue );
      writer.WriteAttributeString( Vml.RotateAttribute, TrueAttributeValue );
      writer.WriteAttributeString( Vml.Opacity2Attribute, Vml.ONamespace, GetOpacityFormat( textBox.Fill.TransparencyTo ) );

      double angle = 0.0;
      switch( textBox.Fill.GradientStyle )
      {
        case ExcelGradientStyle.Horizontal:
          angle = 0.0;
          writer.WriteAttributeString( Vml.TypeAttributeName, Vml.GradientTypeTagValue );
          break;
        case ExcelGradientStyle.Vertical:
          angle = VmlTextBoxBaseParser.GradientVertical;
          writer.WriteAttributeString( Vml.TypeAttributeName, Vml.GradientTypeTagValue );
          writer.WriteAttributeString( Vml.AngleAttribute, angle.ToString() );
          break;
        case ExcelGradientStyle.Diagonl_Up:
          angle = VmlTextBoxBaseParser.GradientDiagonalUp;
          writer.WriteAttributeString( Vml.AngleAttribute, angle.ToString() );
          writer.WriteAttributeString( Vml.TypeAttributeName, Vml.GradientTypeTagValue );
          break;

        case ExcelGradientStyle.Diagonl_Down:
          angle = VmlTextBoxBaseParser.GradientDiagonalDownCornerCenter;
          writer.WriteAttributeString( Vml.AngleAttribute, angle.ToString() );
          writer.WriteAttributeString( Vml.TypeAttributeName, Vml.GradientTypeTagValue );
          break;
        case ExcelGradientStyle.From_Center:
          angle = VmlTextBoxBaseParser.GradientDiagonalDownCornerCenter;
          writer.WriteAttributeString( Vml.AngleAttribute, angle.ToString() );
          writer.WriteAttributeString( Vml.TypeAttributeName, Vml.GradientRadialTypeTagValue );
          break;
        case ExcelGradientStyle.From_Corner:
          angle = VmlTextBoxBaseParser.GradientDiagonalDownCornerCenter;
          writer.WriteAttributeString( Vml.AngleAttribute, angle.ToString() );
          writer.WriteAttributeString( Vml.TypeAttributeName, Vml.GradientRadialTypeTagValue );
          //Start <o:fill Tag
          writer.WriteStartElement( Vml.FillTag, Vml.ONamespace );
          writer.WriteAttributeString( Vml.Ext, Vml.VNamespace, "view" );
          writer.WriteAttributeString( Vml.TypeAttributeName, Vml.GradientCenterTypeTagValue );
          writer.WriteEndElement();// end <o:fill> tag

          break;
      }
      switch( textBox.Fill.GradientVariant )
      {
        case ExcelGradientVariants.ShadingVariants_1:
          writer.WriteAttributeString( Vml.FocusAttribute, VmlTextBoxBaseParser.GradientVariant_1 + "%" );
          break;
        case ExcelGradientVariants.ShadingVariants_2:
          // attribute not avaialble 
          break;
        case ExcelGradientVariants.ShadingVariants_3:
          writer.WriteAttributeString( Vml.FocusAttribute, VmlTextBoxBaseParser.GradientVariant_3 + "%" );
          break;
        case ExcelGradientVariants.ShadingVariants_4:
          writer.WriteAttributeString( Vml.FocusAttribute, VmlTextBoxBaseParser.GradientVariant_4 + "%" );
          break;
      }
    }
    /// <summary>
    /// Serialize Common fill attributes
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="textBox ">textBox Shape to serialize style for</param>
    protected virtual void SerializeFillCommon( XmlWriter writer, TextBoxShapeBase textBox )
    {

      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( textBox == null )
        throw new ArgumentNullException( "textBox" );

      // Temporary Block to support Opacity attribute
      ExcelFillType oldFillType = textBox.Fill.FillType;
      textBox.Fill.FillType = ExcelFillType.SolidColor;
      writer.WriteAttributeString( Vml.OpacityAttribute, GetOpacityFormat( textBox.Fill.Transparency ) );

      textBox.Fill.FillType = oldFillType;

      writer.WriteAttributeString( Vml.ReColorAttribute, TrueAttributeValue );
      writer.WriteAttributeString( Vml.RotateAttribute, TrueAttributeValue );
    }
    /// <summary>
    /// Seialize Line for the Shape
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="textBox">textBox Shape to serialize style for</param>
    /// <param name="holder">Parent worksheet data holder.</param>
    /// <param name="vmlRelations">Resource relation collection</param>
    protected virtual void SerializeLine( XmlWriter writer, TextBoxShapeBase textBox,
      FileDataHolder holder, RelationCollection relations )
    {

      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( textBox == null )
        throw new ArgumentNullException( "textBox" );

      if( !textBox.HasLineFormat )
        return;

      writer.WriteStartElement( Vml.Stroke, Vml.VNamespace ); //<v:stroke> start
      //<v:shape strokecolor="#330 [59]  stroked=�f or false�>
      //<v:stroke dashStyle=�1 1�  linestyle="thinThick" weight="4pt">
      //<v:stroke o:relid="rId1" o:title="" color2="yellow [13]" filltype="pattern"/>
      if( textBox.Line.HasPattern )
      {
        SerializePatternLine( writer, textBox, holder, relations );
        writer.WriteAttributeString( Vml.Color2Attribute, Vml.RGBColorPrefixChar + GenerateHexColor( textBox.Line.ForeColor ) );
      }
      else
      {
        writer.WriteAttributeString( Vml.DashStyleAttribute, GetDashStyle( textBox.Line.DashStyle ) );
        writer.WriteAttributeString( Vml.LineStyleAttribute, GetLineStyle( textBox.Line.Style ) );
        writer.WriteAttributeString( Vml.Color2Attribute, Vml.RGBColorPrefixChar + GenerateHexColor( textBox.Line.ForeColor ) );
      }

      writer.WriteEndElement(); //</stroke> end element
    }
    /// <summary>
    /// Serialize Pattern Line
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="textBox">textBox Shape to serialize style for</param>
    /// <param name="holder">Parent worksheet data holder.</param>
    /// <param name="vmlRelations">Resource relation collection</param>
    protected virtual void SerializePatternLine( XmlWriter writer, TextBoxShapeBase textBox,
        FileDataHolder holder, RelationCollection relations )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( textBox == null )
        throw new ArgumentNullException( "textBox" );

      if( holder == null )
        throw new ArgumentNullException( "holder" );

      if( relations == null )
        throw new ArgumentNullException( "relations" );

      Image image;
      ExcelGradientPattern pattern = textBox.Line.Pattern;

      //User Define pattern Line not Implemented.
      //if (pattern != ExcelGradientPattern.User_Defined)
      {
#if ( WINRT )
          ResourceHandler resource = new ResourceHandler();
          byte[] arrData = resource.PatternArray[ShapeFillImpl.DEF_PATTERN_PREFIX + ((int)pattern).ToString()];
#else
        byte[] arrData = ShapeFillImpl.GetResData( ShapeFillImpl.DEF_PATTERN_PREFIX + ( ( int )pattern ).ToString() );
#endif
        byte[] arrPicture = new byte[ arrData.Length - 25 ];

        Array.Copy( arrData, 25, arrPicture, 0, arrPicture.Length );
        MemoryStream ms = new MemoryStream();

        ShapeFillImpl.UpdateBitMapHederToStream( ms, arrData );

        ms.Write( arrPicture, 0, arrPicture.Length );

        image = ApplicationImpl.CreateImage( ms );

        string imageName = pattern.ToString();
        imageName = GeneratePatternName( pattern );
        string strLocation = holder.SaveImage( image, null );
        string strRelationId = relations.GenerateRelationId();
        relations[ strRelationId ] = new Relation( '/' + strLocation, RelationTypes.Image );
        writer.WriteAttributeString( Vml.RelationIDAttribute, Vml.ONamespace, strRelationId );
        writer.WriteAttributeString( Vml.TitleAttibute, Vml.ONamespace, imageName );

      }
    }
    /// <summary>
    /// double degree value to string value with some calculations
    /// </summary>
    /// <param name="degree">degree in double</param>
    /// <returns></returns>
    protected string PrepareGradientDegree( double degree )
    {
      string degreeAttributeValue = null;
      if( degree > 0.5 )
      {
        degreeAttributeValue = Vml.GradientDarkFillValue + "(";
        degreeAttributeValue += ( int )( degree * Vml.DegreeDivider + Vml.DarkLimit );
        degreeAttributeValue += ")";
      }
      else
      {
        degreeAttributeValue = Vml.GradientLightFillValue + "(";
        degreeAttributeValue += ( int )( degree * Vml.DegreeDivider );
        degreeAttributeValue += ")";
      }

      return degreeAttributeValue;
    }
    /// <summary>
    /// Generate Hexdecimal color from the Color
    /// </summary>
    /// <param name="color">color object</param>
    /// <returns></returns>
    protected string GenerateHexColor( Color color )
    {
      return Vml.RGBColorPrefixChar + RemovePrecedingZeroes( ( color.ToArgb() & 0xFFFFFF ).ToString( "X6" ) );
    }
    /// <summary>
    /// Get the String value from the resource 
    /// </summary>
    /// <param name="excelGradientPreset">Excel Gradient Preset</param>
    /// <returns></returns>
    protected string GetPresetString( ExcelGradientPreset excelGradientPreset )
    {
#if !(WINRT )
      ResourceManager manager = new ResourceManager( "Syncfusion.XlsIO.VMLPresetGradientFills", typeof( VmlTextBoxBaseSerializator ).Assembly );
#else
        ResourceManager manager = new ResourceManager( "Syncfusion.XlsIO.VMLPresetGradientFills",  typeof( VmlTextBoxBaseSerializator ).GetTypeInfo().Assembly );
#endif
      return manager.GetString( excelGradientPreset.ToString(), System.Globalization.CultureInfo.CurrentCulture );
    }
    /// <summary>
    /// Enum pattern to string
    /// </summary>
    /// <param name="pattern">enum pattern to string</param>
    /// <returns>execel equalent name</returns>
    protected string GeneratePatternName( ExcelGradientPattern pattern )
    {
      string excelGradientPattern = null;
      excelGradientPattern = pattern.ToString();
      excelGradientPattern = excelGradientPattern.Remove( 0, VmlTextBoxBaseParser.PatternPrefix.ToString().Length - 1 );
      excelGradientPattern = excelGradientPattern.Replace( '_', ' ' );
      return excelGradientPattern.Trim();
    }
    /// <summary>
    /// opacity double to string with some calculations
    /// </summary>
    /// <param name="opacity">opacity to convert</param>
    /// <returns>opacity in excel format</returns>
    protected string GetOpacityFormat( double opacity )
    {
      //if( opacity == 0.0 )
      //  opacity = 1.0;
      opacity = 1.0 - opacity;
      return ( ( int )( opacity * Vml.OpacityDegree ) ) + "f";
    }
    /// <summary>
    ///  Remove Preceding zeroes in the color
    /// </summary>
    /// <param name="color">string color to remove zeroes</param>
    /// <returns>removed preceding zeroes</returns>
    protected string RemovePrecedingZeroes( string color )
    {
      for( int i = 0; i < color.Length; i++ )
      {
        if( color.StartsWith( "0" ) )
          color = color.Remove( 0, 1 );
        else
          break;
      }
      return color;
    }
    /// <summary>
    /// Get the Excel matching Dash Style
    /// </summary>
    /// <param name="dashStyle">dashstyle to string</param>
    /// <returns>dashstyle to excel format</returns>
    protected string GetDashStyle( ExcelShapeDashLineStyle dashStyle )
    {
      if( VmlTextBoxBaseParser.m_excelDashLineStyle == null )
        VmlTextBoxBaseParser.InitDashLineStyle();

      Dictionary<string, ExcelShapeDashLineStyle> excelDashStyle = VmlTextBoxBaseParser.m_excelDashLineStyle;
      IEnumerator<string> keys = excelDashStyle.Keys.GetEnumerator();
      IEnumerator<ExcelShapeDashLineStyle> values = excelDashStyle.Values.GetEnumerator();

      while( keys.MoveNext() )
      {
        values.MoveNext();
        if( values.Current == dashStyle )
        {
          break;
        }
      }
      return keys.Current;
    }
    /// <summary>
    /// Get the line style from the dictionary
    /// </summary>
    /// <param name="lineStyle">linestyle to string</param>
    /// <returns>line style in excel format</returns>
    protected string GetLineStyle( ExcelShapeLineStyle lineStyle )
    {
      if( VmlTextBoxBaseParser.m_excelShapeLineStyle == null )
        VmlTextBoxBaseParser.InitShapeLineStyle();

      Dictionary<string, ExcelShapeLineStyle> excelLineStyle = VmlTextBoxBaseParser.m_excelShapeLineStyle;
      IEnumerator<string> keys = excelLineStyle.Keys.GetEnumerator();
      IEnumerator<ExcelShapeLineStyle> values = excelLineStyle.Values.GetEnumerator();

      while( keys.MoveNext() )
      {
        values.MoveNext();
        if( values.Current == lineStyle )
        {
          break;
        }
      }
      return keys.Current;
    }
    public static bool IsEmptyColor( Color color )
    {
      return color == ColorExtension.Empty;
    }
 
    #endregion
  }
}
