#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Resources;
using System.Xml;
using Syncfusion.XlsIO.Implementation.Shapes;
using Syncfusion.XlsIO.Implementation.XmlSerialization;
using Syncfusion.XlsIO.Implementation.XmlSerialization.Shapes;
using Syncfusion.XlsIO.Implementation.Collections;
using System.IO;

#if ( WINRT )
using System.Reflection;
using Syncfusion.XlsIO;
#else
using System.Drawing;
#endif

namespace Syncfusion.XlsIO.Implementation.XmlReaders.Shapes
{
  /// <summary>
  /// Class used to parse Textbox.
  /// </summary>
  abstract class VmlTextBoxBaseParser : ShapeParser
  {
    #region Constants

    internal const int GradientHorizontal = 0;
    internal const int GradientVertical = -90;
    internal const int GradientDiagonalUp = -135;
    internal const int GradientDiagonalDownCornerCenter = -45;
    private const byte IndexedColor = 1;
    private const byte NamedColor = 2;
    private const byte HexColor = 3;
    internal const int GradientVariant_1 = 100;
    internal const int GradientVariant_3 = 50;
    internal const int GradientVariant_4 = -50;
    internal const string PatternPrefix = "pat_";
    private const string LineStylePrefix = "LINE_";
    private const string ResourcePatternPrefix = "Patt";
    private const string DefaultFillStyle = "solid";
    #endregion

    #region Static
    /// <summary>
    /// Hold the Enum shape line style and excel
    /// line style representation
    /// </summary>
    public static Dictionary<string, ExcelShapeLineStyle> m_excelShapeLineStyle;
    /// <summary>
    /// hold the enum dash style and excel
    /// dash style representation
    /// </summary>
    public static Dictionary<string, ExcelShapeDashLineStyle> m_excelDashLineStyle;
    #endregion

    #region Static Methods
    /// <summary>
    /// Adds list of Shape line style
    /// </summary>
    public static void InitShapeLineStyle()
    {
      m_excelShapeLineStyle = new Dictionary<string, ExcelShapeLineStyle>();
      m_excelShapeLineStyle.Add( "single", ExcelShapeLineStyle.Line_Single );
      m_excelShapeLineStyle.Add( "thinThin", ExcelShapeLineStyle.Line_Thin_Thin );
      m_excelShapeLineStyle.Add( "thinThick", ExcelShapeLineStyle.Line_Thin_Thick );
      m_excelShapeLineStyle.Add( "thickThin", ExcelShapeLineStyle.Line_Thick_Thin );
      m_excelShapeLineStyle.Add( "thickBetweenThin", ExcelShapeLineStyle.Line_Thick_Between_Thin );
    }
    /// <summary>
    /// Adds the list of shape dash style
    /// </summary>
    public static void InitDashLineStyle()
    {
      m_excelDashLineStyle = new Dictionary<string, ExcelShapeDashLineStyle>();
      m_excelDashLineStyle.Add( "solid", ExcelShapeDashLineStyle.Solid );
      m_excelDashLineStyle.Add( "1 1", ExcelShapeDashLineStyle.Dotted_Round );
      m_excelDashLineStyle.Add( "squareDot", ExcelShapeDashLineStyle.Dotted );
      m_excelDashLineStyle.Add( "dash", ExcelShapeDashLineStyle.Dashed );
      m_excelDashLineStyle.Add( "dashDot", ExcelShapeDashLineStyle.Dash_Dot );
      m_excelDashLineStyle.Add( "longDash", ExcelShapeDashLineStyle.Medium_Dashed );
      m_excelDashLineStyle.Add( "longDashDot", ExcelShapeDashLineStyle.Medium_Dash_Dot );
      m_excelDashLineStyle.Add( "longDashDotDot", ExcelShapeDashLineStyle.Dash_Dot_Dot );
    }
    #endregion

    #region Members
    /// <summary>
    /// Gradient center shadding option is parsed and serailized by the 
    /// "GradientRadial" attribute value, this is not in Enum ExcelGradientStyle
    /// so this property hold this value temporarly
    /// </summary>
    private bool m_isGradientShadingRadial;
    #endregion

    #region Properties
    /// <summary>
    /// Gradient center shadding option is parsed and serailized by the 
    /// "GradientRadial" attribute value, this is not in Enum ExcelGradientStyle
    /// so this property hold this value temporarly
    /// </summary>
    private bool IsGradientShadingRadial
    {
      get
      {
        return m_isGradientShadingRadial;
      }
      set
      {
        m_isGradientShadingRadial = value;
      }
    }
    #endregion

    #region Methods
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

      // Since clone method adds shape to the collection, we have to fill default
      // shape with correct values and then call clone.
      TextBoxShapeBase textBox = ( TextBoxShapeBase )defaultShape.Clone( defaultShape.Parent,
        null, null, false );

      ParseShapeId( reader, textBox );

      if( reader.MoveToAttribute( Vml.StyleAttribute ) )
        ParseStyle( reader, textBox );

      if( reader.MoveToAttribute( Vml.FilledAttribute ) )
      {
        textBox.HasFill = false;
       // textBox.Fill.BackColor = ColorExtension.Empty;
       // textBox.Fill.ForeColor = ColorExtension.Empty;
      }
      else
      {
        textBox.HasFill = true;
      }

      if( reader.MoveToAttribute( Vml.StrokedAttribute ) )
        textBox.HasLineFormat = reader.Value != ShapeSerializator.FalseAttributeValue;

      if( textBox.HasLineFormat )
      {
        if( reader.MoveToAttribute( Vml.StrokeColorAttribute ) )
        {
          ColorObject colorObject = ExtractColor( reader.Value );
          textBox.Line.BackColor = colorObject.GetRGB( textBox.Workbook );
          textBox.Line.DashStyle = ExcelShapeDashLineStyle.Solid;
          textBox.Line.HasPattern = false;
          textBox.Line.Style = ExcelShapeLineStyle.Line_Single;
          textBox.Line.Weight = 0.5;
        }

        if (reader.MoveToAttribute(Vml.StrokeWeightAttribute))
        {
            string weight = reader.Value;
            if (weight.Contains("pt"))
                textBox.Line.Weight = Convert.ToDouble(weight.Split('p')[0]);
            else if (weight.Contains("mm"))
                textBox.Line.Weight = Convert.ToDouble(weight.Split('m')[0]);
        }
      }

      if( textBox.HasFill && reader.MoveToAttribute( Vml.FillColorAttribute ) )
      {
        ColorObject colorObject = this.ExtractColor( reader.Value );
        textBox.FillColor = colorObject.GetRGB( textBox.Workbook );
        textBox.Fill.ForeColor = textBox.FillColor;
      }

      if( reader.MoveToAttribute( Vml.AlternateTextAttribute ) )
        textBox.AlternativeText = reader.Value.Split( Vml.RGBColorPrefixChar )[ 0 ];

      if( reader.MoveToAttribute( Vml.SpIdAttributeName, Vml.ONamespace ) && reader.MoveToAttribute( Vml.ShapeIdAttributeName ) )
      {
        textBox.Name = reader.Value;
      }

      reader.MoveToElement();
      reader.Read();
      bool bResult = true;
      bool bClientDataPresent = false;
      string shapeType = null;

      while( reader.NodeType != XmlNodeType.EndElement && bResult )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case Vml.ClientDataTagName:
              bClientDataPresent = true;
              bResult = ParseClientData( reader, textBox, out shapeType );
              break;

            case Vml.TextBoxTagName:
              ParseTextBox( reader, textBox );
              break;

            case Vml.FillTag:
              //if( textBox.HasFill )
                ParseFillStyle( reader, textBox, relations, parentItemPath );
              break;

            case Vml.Stroke:
              if( textBox.HasLineFormat )
              {
                ParseLine( reader, textBox, relations, parentItemPath );
              }
              else
              {
                goto default;
              }
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

      if (bResult && bClientDataPresent && shapeType != "Shape")
          RegisterShape(textBox);

      return bResult;
    }

    public static void ParseShapeId( XmlReader reader, ShapeImpl shape )
    {
        string fullId = null;
        int index = 0;

        if (reader.MoveToAttribute(Vml.SpIdAttributeName, Vml.ONamespace))
        {
            //shape o:spid="_x0000_s33851"
            string tmpId = reader.Value;
            index = tmpId.IndexOf("_s");

            if (index >= 0)
                fullId = tmpId;
        }
        if (fullId==null && reader.MoveToAttribute(Vml.ShapeIdAttributeName))
        {
            //shape id="_x0000_s33851"
            string tmpId = reader.Value;
            index = tmpId.IndexOf("_s");

            if (index >= 0)
                fullId = tmpId;
        }
        
        if(fullId!=null)
        {
          string shortId = fullId.Substring( index + 2 );
          int shapeId;

          if( int.TryParse( shortId, out shapeId ) )
          {
            ShapesCollection shapes = shape.Worksheet.InnerShapes;

            if( shapes.StartId == 0 )
            {
              shapes.StartId = shapeId;
            }

            ShapeImpl oldShape = shapes.GetShapeById( ( shapeId ) ) as ShapeImpl;
            shape.ShapeId = shapeId;

            if( oldShape != null && oldShape.EnableAlternateContent )
            {
              shape.EnableAlternateContent = true;
              shape.XmlDataStream = oldShape.XmlDataStream;
              if (oldShape.Name != null && oldShape.Name.Length > 0)
              {
                  shape.Name = oldShape.Name;
              }
              oldShape.Remove();
            }
          }
        }
      
      //throw new NotImplementedException();
    }
    /// <summary>
    /// Parses text box name and divisions
    /// </summary>
    /// <param name="reader">Reader to get data from.</param>
    /// <param name="textBox">Text box to parse.</param>
    private void ParseTextBox( XmlReader reader, TextBoxShapeBase textBox )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( textBox == null )
        throw new ArgumentNullException( "textBox" );

      if( reader.LocalName != Vml.TextBoxTagName )
        throw new XmlException( "Unexcpected xml tag." );

      if( reader.MoveToAttribute( Vml.StyleAttribute ) )
      {
        string strStyle = reader.Value;
        Dictionary<string, string> dictProperties = SplitStyle( strStyle );
        ParseTextDirection( textBox, dictProperties );
      }

      reader.MoveToElement();

      if( !reader.IsEmptyElement )
      {
        reader.Read();

        while( reader.NodeType != XmlNodeType.EndElement )
        {
          if( reader.NodeType == XmlNodeType.Element )
          {
            switch( reader.LocalName )
            {
              case Vml.DivTagName:
                ParseDiv( reader, textBox );
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
      }

      reader.Skip();
    }
    /// <summary>
    /// Parses division element
    /// </summary>
    /// <param name="reader">Reader to get data from.</param>
    /// <param name="textBox">Text box to parse.</param>
    private void ParseDiv( XmlReader reader, TextBoxShapeBase textBox )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( textBox == null )
        throw new ArgumentNullException( "textBox" );

      if( !reader.IsEmptyElement )
      {
        reader.Read();

        while( reader.NodeType != XmlNodeType.EndElement )
        {
          if( reader.NodeType == XmlNodeType.Element )
          {
            switch( reader.LocalName )
            {
              case Vml.FontTag:
                ParseFormattingRun( reader, textBox );
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
      }

      reader.Read();
    }
    /// <summary>
    /// Extracts single formatting run (text + font) from the reader.
    /// </summary>
    /// <param name="reader">Reader to get text data from.</param>
    /// <param name="textBox">Text box shape to put extracted data into.</param>
    private void ParseFormattingRun( XmlReader reader, TextBoxShapeBase textBox )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( textBox == null )
        throw new ArgumentNullException( "textBox" );
      Stream stream = ShapeParser.ReadNodeAsStream(reader);
      stream.Position = 0;
      XmlReader fontReader = UtilityMethods.CreateReader(stream);
      bool fontHasValue = CheckFontElement(fontReader);
      stream.Position = 0;
      fontReader = UtilityMethods.CreateReader(stream);
      IFont font = textBox.Workbook.CreateFont();

      if (fontReader.MoveToAttribute(Vml.Face))
          font.FontName = fontReader.Value;

      if (fontReader.MoveToAttribute(Vml.Size))
          font.Size = XmlConvert.ToInt32(fontReader.Value) / 20.0;

      fontReader.MoveToElement();
      string text = string.Empty;

      if (fontHasValue)
      {
          text = fontReader.ReadElementContentAsString();
          IRichTextString richText = textBox.RichText;
          int iCurrentpos = richText.Text.Length;
          richText.Append(text, font);
      }
      else
          fontReader.Skip();
    }

    /// <summary>
    /// Checks the font element.
    /// </summary>
    /// <param name="fontReader">The font reader.</param>
    /// <returns></returns>
    private bool CheckFontElement(XmlReader fontReader)
    {
        if (fontReader == null)
            throw new ArgumentNullException();

        if (fontReader.LocalName != "font")
            throw new XmlException();

        fontReader.Read(); 
        fontReader.Read();
        if (fontReader.LocalName == "font" && fontReader.NodeType == XmlNodeType.EndElement)
            return true;

        return false;

    }
    /// <summary>
    /// Registers shape in all necessary collections.
    /// </summary>
    /// <param name="textBox">Shape to register.</param>
    protected virtual void RegisterShape( TextBoxShapeBase textBox )
    {
      if( textBox == null )
        throw new ArgumentNullException( "comment" );

      WorksheetImpl sheet = ( WorksheetImpl )textBox.Worksheet;
      sheet.InnerShapes.AddShape( textBox );
    }
    /// <summary>
    /// Parses client data tag and all its internal tags.
    /// </summary>
    /// <param name="reader">Reader to get necessary values from.</param>
    /// <param name="textBox">Shape to parse client data for.</param>
    private bool ParseClientData( XmlReader reader, TextBoxShapeBase textBox, out string shapeType )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( textBox == null )
        throw new ArgumentNullException( "textBox" );

      if( reader.LocalName != Vml.ClientDataTagName )
        throw new XmlException( "Unexpected xml token" );

      shapeType = null;

      if( reader.MoveToAttribute( Vml.ObjectTypeAttribute ) )
      {
        shapeType = reader.Value;

        if( shapeType == Vml.PictureObjectTypeAttributeValue )
          return false;
      }

      reader.Read();
      textBox.IsMoveWithCell = true;
      textBox.IsSizeWithCell = true;

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case Vml.MoveWithCellsTagName:
              textBox.IsMoveWithCell = !ParseBoolOrEmpty( reader, true );
              break;

            case Vml.SizeWithCellsTagName:
              textBox.IsSizeWithCell = !ParseBoolOrEmpty( reader, true );
              break;

            case Vml.AnchorTagName:
              ParseAnchor( reader, textBox );
              break;

            case Vml.TextHAlign:
              textBox.HAlignment = ( ExcelCommentHAlign )Enum.Parse(
                typeof( ExcelCommentHAlign ), reader.ReadElementContentAsString(), false );
              break;

            case Vml.TextVAlign:
              textBox.VAlignment = ( ExcelCommentVAlign )Enum.Parse(
                typeof( ExcelCommentVAlign ), reader.ReadElementContentAsString(), false );
              break;

            case Vml.LockText:
              string strValue = reader.ReadElementContentAsString();
              textBox.IsTextLocked = XmlConvert.ToBoolean( strValue.ToLower() );
              break;

            default:
              ParseUnknownClientDataTag( reader, textBox );
              break;

          }
        }

        reader.Read();
      }

      reader.Read();

      return true;
    }
    /// <summary>
    /// Tries to parse unknown client data tag.
    /// </summary>
    /// <param name="reader">Reader to get data from.</param>
    /// <param name="textBox">Shape to put data into.</param>
    protected virtual void ParseUnknownClientDataTag( XmlReader reader, TextBoxShapeBase textBox )
    {
      reader.Skip();
    }
    /// <summary>
    /// Extracts boolean value from the reader. When element is empty - default value is used.
    /// </summary>
    /// <param name="reader">XmlReader to get value from.</param>
    /// <param name="defaultValue">Default value to use.</param>
    /// <returns>Value containing parsed boolean value if there is any or default one if tag is empty.</returns>
    public static bool ParseBoolOrEmpty( XmlReader reader, bool defaultValue )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      bool bResult = defaultValue;

      if( !reader.IsEmptyElement )
      {
        string strValue = reader.ReadElementContentAsString();

        if( strValue.Length != 0 )
        {
          bResult = bool.Parse( strValue );
        }
      }
      else
      {
        reader.Read();
      }

      return bResult;
    }
    /// <summary>
    /// This method parses fill color types
    /// </summary>
    /// <param name="reader">XmlReader to parse fill color from.</param>
    /// <param name="textBox">TextBox to set fill color to.</param>
    /// <param name="relations">relation Collection of the item</param>
    /// <param name="parentItemPath"> path of the item</param>
    private void ParseFillStyle( XmlReader reader, TextBoxShapeBase textBox,
      RelationCollection relations, string parentItemPath )
    {

      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( textBox == null )
        throw new ArgumentNullException( "textBox" );

      ExcelFillType fillType;
      fillType = reader.MoveToAttribute( Vml.TypeAttributeName ) ?
        GetExcelFillType( reader.Value ) :
        GetExcelFillType( DefaultFillStyle );

      if( reader.MoveToAttribute( Vml.OpacityAttribute ) )
        textBox.Fill.Transparency = 1 - ExtractOpacity( reader.Value );

      if( reader.MoveToAttribute( Vml.ColorAttribute ) )
      {
        ColorObject colorObject = ExtractColor( reader.Value );
        textBox.FillColor = colorObject.GetRGB( textBox.Workbook );

        if( textBox.FillColor == ColorExtension.Empty )
          textBox.HasFill = false;

        textBox.Fill.ForeColor = textBox.FillColor;
      }
      switch( fillType )
      {
        case ExcelFillType.SolidColor:
          ParseSolidFill( reader, textBox );
          break;

        case ExcelFillType.Gradient:
          ParseGradientFill( reader, textBox );
          break;

        case ExcelFillType.Texture:
          ParseTextureFill( reader, textBox, relations );
          break;

        case ExcelFillType.Pattern:
          ParsePatternFill( reader, textBox, relations );
          break;

        case ExcelFillType.Picture:
          ParsePictureFill( reader, textBox, relations );
          break;
      }

    }

    /// <summary>
    /// Parse the solid color
    /// </summary>
    /// <param name="reader">XmlReader to parse fill color from.</param>
    /// <param name="textBox">TextBox to set fill color to.</param>
    private void ParseSolidFill( XmlReader reader, TextBoxShapeBase textBox )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( textBox == null )
        throw new ArgumentNullException( "textBox" );

      textBox.Fill.FillType = ExcelFillType.SolidColor;

      if( reader.MoveToAttribute( Vml.OpacityAttribute ) )
      {
        textBox.Fill.Transparency = 1 - ExtractOpacity( reader.Value );
      }

      reader.MoveToElement();
      reader.Skip();
    }
    /// <summary>
    /// Parse the Graident fills One Color,Two Color and Presets 
    /// </summary>
    /// <param name="reader">XmlReader to parse fill color from.</param>
    /// <param name="textBox">TextBox to set fill color to.</param>

    private void ParseGradientFill( XmlReader reader, TextBoxShapeBase textBox )
    {

      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( textBox == null )
        throw new ArgumentNullException( "textBox" );

      textBox.Fill.FillType = ExcelFillType.Gradient;
      string color = null;
      textBox.Fill.GradientColorType = ExtractGradientColorType( reader, out color );

      switch( textBox.Fill.GradientColorType )
      {
        case ExcelGradientColor.OneColor:
          //Color2 attribute contains the darkness of the 
          // one color
          textBox.Fill.BackColor = textBox.FillColor;
          textBox.Fill.GradientDegree = ExtractDegree( color );
          break;

        case ExcelGradientColor.TwoColor:
          //color2 attribute contains the second fill color
          ColorObject colorObject = ExtractColor( color );
          textBox.Fill.BackColor = textBox.FillColor;
          textBox.Fill.ForeColor = colorObject.GetRGB( textBox.Workbook );
          break;

        case ExcelGradientColor.Preset:
          //colors attribute contains the color combination 
          // of preset colors and it is seprated with semicolons.
          textBox.Fill.PresetGradient( ExtractPreset( color ) );
          break;
      }

      ParseGradientCommon( reader, textBox );
    }
    /// <summary>
    /// This method parses Texture fill  attribute.
    /// </summary>
    /// <param name="reader">XmlReader to extract fill color from.</param>
    /// <param name="textBox">TextBox to set fill color to.</param>
    /// <param name="relations">relation Collection of the item</param>
    /// <param name="parentItemPath"> path of the item</param>
    private void ParseTextureFill( XmlReader reader, TextBoxShapeBase textBox,
        RelationCollection relations )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( textBox == null )
        throw new ArgumentNullException( "textBox" );

      if( relations == null )
        throw new ArgumentNullException( "relation collection" );

      string title = "image";

      if( reader.MoveToAttribute( Vml.TitleAttibute, Vml.ONamespace ) )
        title = reader.Value;

      textBox.Fill.FillType = ExcelFillType.Texture;
      textBox.Fill.Texture = ExtractTexture( title );

      if( textBox.Fill.Texture == ExcelTexture.User_Defined && reader.MoveToAttribute( Vml.RelationIDAttribute, Vml.ONamespace ) )
      {
        FileDataHolder holder = textBox.ParentWorkbook.DataHolder;
        string strRelationId = reader.Value;
        Relation relation = relations[ strRelationId ];
        string strPath = relations.ItemPath;
        int index = strPath.LastIndexOf( '/' );
        strPath = strPath.Substring( 0, index );
        index = strPath.LastIndexOf( '/' );
        strPath = strPath.Substring( 0, index );
        strPath = FileDataHolder.CombinePath( strPath, relation.Target );
        Image image = holder.GetImage( strPath );
        textBox.Fill.UserTexture( image, title );
      }
      else
      {
        textBox.Fill.PresetTextured( ExtractTexture( title ) );
      }
    }
    /// <summary>
    /// This method parses Pattern fill  attribute.
    /// </summary>
    /// <param name="reader">XmlReader to extract fill color from.</param>
    /// <param name="textBox">TextBox to set fill color to.</param>
    /// <param name="relations">relation Collection of the item</param>
    /// <param name="parentItemPath"> path of the item</param>
    private void ParsePatternFill( XmlReader reader, TextBoxShapeBase textBox,
      RelationCollection relations )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( textBox == null )
        throw new ArgumentNullException( "textBox" );

      if( relations == null )
        throw new ArgumentNullException( "relation collection" );

      string title = "image";

      if( reader.MoveToAttribute( Vml.TitleAttibute, Vml.ONamespace ) )
        title = reader.Value;

      if( reader.MoveToAttribute( Vml.Color2Attribute ) )
      {
        ColorObject colorObject = ExtractColor( reader.Value );
        textBox.Fill.BackColor = textBox.FillColor;
        textBox.Fill.ForeColor = colorObject.GetRGB( textBox.Workbook );
        textBox.Fill.FillType = ExcelFillType.Pattern;
        textBox.Fill.Pattern = ExtractPattern( title );
      }

      if( reader.MoveToAttribute( Vml.RelationIDAttribute, Vml.ONamespace ) )
      {
        FileDataHolder holder = textBox.ParentWorkbook.DataHolder;
        string strRelationId = reader.Value;
        Relation relation = relations[ strRelationId ];
        string strPath = relations.ItemPath;
        int index = strPath.LastIndexOf( '/' );
        strPath = strPath.Substring( 0, index );
        index = strPath.LastIndexOf( '/' );
        strPath = strPath.Substring( 0, index );
        strPath = FileDataHolder.CombinePath( strPath, relation.Target );
        Image image = holder.GetImage( strPath );
        textBox.Fill.Patterned( textBox.Fill.Pattern );
      }
    }
    /// <summary>
    /// This method parses Picture fill  attribute.
    /// </summary>
    /// <param name="reader">XmlReader to extract fill color from.</param>
    /// <param name="textBox">TextBox to set fill color to.</param>
    /// <param name="relations">relation Collection of the item</param>
    /// <param name="parentItemPath"> path of the item</param>
    private void ParsePictureFill( XmlReader reader, TextBoxShapeBase textBox,
    RelationCollection relations )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( textBox == null )
        throw new ArgumentNullException( "textBox" );

      if( relations == null )
        throw new ArgumentNullException( "relation collection" );

      string title = "image";

      if( reader.MoveToAttribute( Vml.TitleAttibute, Vml.ONamespace ) )
        title = reader.Value;

      if( reader.MoveToAttribute( Vml.RelationIDAttribute, Vml.ONamespace ) )
      {
        FileDataHolder holder = textBox.ParentWorkbook.DataHolder;
        string strRelationId = reader.Value;
        Relation relation = relations[ strRelationId ];
        string strPath = relations.ItemPath;
        int index = strPath.LastIndexOf( '/' );
        strPath = strPath.Substring( 0, index );
        index = strPath.LastIndexOf( '/' );
        strPath = strPath.Substring( 0, index );
        strPath = FileDataHolder.CombinePath( strPath, relation.Target );
        Image image = holder.GetImage( strPath );
        textBox.Fill.UserPicture( image, title );
      }
    }

    /// <summary>
    /// Parse Gradient Shading, Variants and Transparency
    /// </summary>
    /// <param name="reader">XmlReader to parse fill color from.</param>
    /// <param name="textBox">TextBox to set fill color to.</param>
    private void ParseGradientCommon( XmlReader reader, TextBoxShapeBase textBox )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( textBox == null )
        throw new ArgumentNullException( "textBox" );

      int angle = 0;

      if( reader.MoveToAttribute( Vml.OpacityAttribute ) )
        textBox.Fill.TransparencyFrom = ExtractOpacity( reader.Value );

      if( reader.MoveToAttribute( Vml.OPreffix + ":" + Vml.Opacity2Attribute ) )
        textBox.Fill.TransparencyTo = ExtractOpacity( reader.Value );

      if( reader.MoveToAttribute( Vml.FocusAttribute ) )
      {
        textBox.Fill.GradientVariant = ExtractShadingVariant( reader.Value );
      }

      if( reader.MoveToAttribute( Vml.AngleAttribute ) )
        angle = reader.ReadContentAsInt();

      switch( angle )
      {
        case GradientHorizontal:
          textBox.Fill.GradientStyle = ExcelGradientStyle.Horizontal;
          break;
        case GradientVertical:
          textBox.Fill.GradientStyle = ExcelGradientStyle.Vertical;
          break;
        case GradientDiagonalUp:
          textBox.Fill.GradientStyle = ExcelGradientStyle.Diagonl_Up;
          break;
        case GradientDiagonalDownCornerCenter:
          if( IsGradientShadingRadial )
          {
            reader.Read();

            if( reader.NodeType == XmlNodeType.EndElement ||
                reader.NodeType == XmlNodeType.Whitespace )
            {
              textBox.Fill.GradientStyle = ExcelGradientStyle.From_Center;
            }
            else
            {
              textBox.Fill.GradientStyle = ExcelGradientStyle.From_Corner;
            }
          }
          else
          {
            textBox.Fill.GradientStyle = ExcelGradientStyle.Diagonl_Down;
          }
          break;
      }


    }
    /// <summary>
    /// Parse the border Line of the shape
    /// </summary>
    /// <param name="reader">XmlReader to parse fill color from.</param>
    /// <param name="textBox">TextBox to set fill color to.</param>
    /// <param name="relations">relation Collection of the item</param>
    /// <param name="parentItemPath"> path of the item</param>
    private void ParseLine( XmlReader reader, TextBoxShapeBase textBox,
      RelationCollection relations, string parentItemPath )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( textBox == null )
        throw new ArgumentNullException( "textBox" );


      if( reader.MoveToAttribute( Vml.FillTypeAttribute ) )
      {
        textBox.Line.HasPattern = true;
        ParsePatternLine( reader, textBox, relations, parentItemPath );
      }
      else
      {
        if( reader.MoveToAttribute( Vml.DashStyleAttribute ) )
        {
          textBox.Line.DashStyle = ExtractDashStyle( reader.Value );
        }

        if( reader.MoveToAttribute( Vml.LineStyleAttribute ) )
        {
          textBox.Line.Style = ExtractLineStyle( reader.Value );
        }

        reader.MoveToElement();
        reader.Skip();
      }
    }
    /// <summary>
    /// Parse Pattern Line
    /// </summary>
    /// <param name="reader">XmlReader to parse fill color from.</param>
    /// <param name="textBox">TextBox to set fill color to.</param>
    /// <param name="relations">relation Collection of the item</param>
    /// <param name="parentItemPath"> path of the item</param>
    private void ParsePatternLine( XmlReader reader, TextBoxShapeBase textBox,
    RelationCollection relations, string parentItemPath )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( textBox == null )
        throw new ArgumentNullException( "textBox" );

      if( relations == null )
        throw new ArgumentNullException( "relation collection" );

      if( parentItemPath == null )
        throw new ArgumentNullException( "resource path" );

      if( reader.MoveToAttribute( Vml.RelationIDAttribute, Vml.ONamespace ) )
      {
        // Why do we need to extract image?
        FileDataHolder holder = textBox.ParentWorkbook.DataHolder;
        string strRelationId = reader.Value;
        Relation relation = relations[ strRelationId ];
        string strPath = relations.ItemPath;
        int index = strPath.LastIndexOf( '/' );
        strPath = strPath.Substring( 0, index );
        index = strPath.LastIndexOf( '/' );
        strPath = strPath.Substring( 0, index );
        strPath = FileDataHolder.CombinePath( strPath, relation.Target );
        Image image = holder.GetImage( strPath );

        ExtractLinePattern( holder.GetData( relation, parentItemPath, false ).Length );
      }
    }
    /// <summary>
    /// Parses style attribute of the comment shape.
    /// </summary>
    /// <param name="reader">Reader to get attribute data from.</param>
    /// <param name="textBox">Comment shape to set values to.</param>
    private void ParseStyle( XmlReader reader, TextBoxShapeBase textBox )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( textBox == null )
        throw new ArgumentNullException( "textBox" );

      string strStyle = reader.Value;
     textBox.StyleProperties = SplitStyle( strStyle );
      ParseStyle( textBox, textBox.StyleProperties );
    }
    /// <summary>
    /// Parses style properties.
    /// </summary>
    /// <param name="textBox">Textbox to put properties into.</param>
    /// <param name="styleProperties">String representation of the style properties
    /// (key - property name, value - property value).</param>
    protected virtual void ParseStyle( TextBoxShapeBase textBox, Dictionary<string, string> styleProperties )
    {
      string visibility;

      if( styleProperties.TryGetValue( Vml.VisibilityAttribute, out visibility ) && visibility == Vml.VisibilityHiddenValue )
      {
        textBox.IsShapeVisible = false;
      }
    }
    /// <summary>
    /// Parses TextDirection property comment style property.
    /// </summary>
    /// <param name="textBox">TextBox to set TextRotation for.</param>
    /// <param name="dictProperties">Dictionary with comment properties.</param>
    private void ParseTextDirection( TextBoxShapeBase textBox, Dictionary<string, string> dictProperties )
    {
      if( textBox == null )
        throw new ArgumentNullException( "textBox" );

      if( dictProperties == null )
        throw new ArgumentNullException( "dictProperties" );

      string strMsoLayoutFlow;
      string strLayoutFlow;

      dictProperties.TryGetValue( Vml.MsoLayoutFlow, out strMsoLayoutFlow );
      dictProperties.TryGetValue( Vml.LayoutFlow, out strLayoutFlow );

      if( strMsoLayoutFlow != null || strLayoutFlow == Vml.LayoutFlowVertical )
      {
        ExcelTextRotation textRotation;

        if( strMsoLayoutFlow == Vml.MsoLayoutFlowTopToBottom )
        {
          textRotation = ExcelTextRotation.TopToBottom;
        }
        else if( strMsoLayoutFlow == Vml.MsoLayoutFlowBottomToTop )
        {
          textRotation = ExcelTextRotation.CounterClockwise;
        }
        else
        {
          textRotation = ExcelTextRotation.Clockwise;
        }

        textBox.TextRotation = textRotation;
      }
    }
    #endregion

    #region Helper Methods
    private ExcelFillType GetExcelFillType( string excelFillType )
    {
      System.Collections.Generic.Dictionary<string, ExcelFillType> fillType;
      fillType = new Dictionary<string, ExcelFillType>();
      fillType.Add( Vml.GradientTypeTagValue, ExcelFillType.Gradient );
      fillType.Add( Vml.GradientRadialTypeTagValue, ExcelFillType.Gradient );
      fillType.Add( Vml.PatternAttributeValue, ExcelFillType.Pattern );
      fillType.Add( Vml.PictureAttributeValue, ExcelFillType.Picture );
      fillType.Add( Vml.TextureAttributeValue, ExcelFillType.Texture );
      fillType.Add( Vml.SolidFillTypeAttributeValue, ExcelFillType.SolidColor );
      if( excelFillType.Equals( Vml.GradientRadialTypeTagValue ) )
        IsGradientShadingRadial = true;
      return fillType[ excelFillType ];
    }
    /// <summary>
    /// Checks whether the given color is indexed or not
    /// </summary>
    /// <param name="color">color</param>
    /// <returns>color representation </returns>
    private byte GetColorType( string color )
    {
      if( color.Contains( Vml.IndexedColorPrefix.ToString() ) )
        return IndexedColor;
      else if( color.Contains( Vml.RGBColorPrefixChar.ToString() ) )
        return HexColor;
      else
        return NamedColor;
    }
    /// <summary>
    /// Extract the string color to ColorObject
    /// </summary>
    /// <param name="color">color</param>
    /// <returns>colorobject from string</returns>
    private ColorObject ExtractColor( string color )
    {
      switch( GetColorType( color ) )
      {
        case IndexedColor:
          return new ColorObject( ColorType.Indexed, Convert.ToInt32( color.Split( '[' )[ 1 ].Split( ']' )[ 0 ] ) );
          break;
        case HexColor:
          return new ColorObject( ColorType.RGB, int.Parse( RemoveCharUnSafeAt( color, false ), System.Globalization.NumberStyles.HexNumber, null ) );
          break;
        case NamedColor:
          return new ColorObject( ColorExtension.FromName( color ) );
          break;
        default:
          return new ColorObject( ColorType.RGB, ( int )ExcelKnownColors.White );
      }

    }
    /// <summary>
    /// Extract the opacity double value from string value
    /// </summary>
    /// <param name="opacity">opacity value in string</param>
    /// <returns>double value from excel opacity</returns>
    private double ExtractOpacity( string opacity )
    {
      if( opacity.EndsWith( "f" ) )
      {
        opacity = RemoveCharUnSafeAt( opacity, true );
        return Convert.ToDouble( opacity ) / Vml.OpacityDegree;
      }
      else
      {
        return Convert.ToDouble( opacity );
      }
    }
    /// <summary>
    /// Removes char at the first or last of the given string
    /// </summary>
    /// <param name="source">source string</param>
    /// <param name="isLast">indicates whether remove char at last or first</param>
    /// <returns>remove character at given position</returns>
    private string RemoveCharUnSafeAt( string source, bool isLast )
    {
      return ( isLast ) ? source.Remove( source.Length - 1 ) : source.Remove( 0, 1 );
    }
    /// <summary>
    /// Extract the Gradient Color Type
    /// </summary>
    /// <param name="reader">reader to parse</param>
    /// <returns>enum gradient color</returns>
    private ExcelGradientColor ExtractGradientColorType( XmlReader reader, out string color )
    {
      if( reader.MoveToAttribute( Vml.ColorsAttribute ) )
      {
        color = reader.Value;
        return ExcelGradientColor.Preset;
      }
      else if( reader.MoveToAttribute( Vml.Color2Attribute ) )
      {
        color = reader.Value;
        if( color.StartsWith( Vml.GradientOneColorAttributeValueStart ) )
          return ExcelGradientColor.OneColor;
        else
          return ExcelGradientColor.TwoColor;
      }
      color = Vml.GradientDarkFillValue + "(0)";
      return ExcelGradientColor.OneColor;
    }
    /// <summary>
    /// Extract the darkness or lightness value of one color gradient
    /// for one color2 attribute contains darkness or lightness value
    /// </summary>
    /// <param name="degree">string color2 attribute value</param>
    /// <returns>degree double from string</returns>
    private double ExtractDegree( string degree )
    {
      if( degree == null )
        throw new ArgumentNullException( "degree" );
      double d = XmlConvert.ToDouble( degree.Split( '(' )[ 1 ].Split( ')' )[ 0 ] );

      if( degree.Contains( Vml.GradientLightFillValue ) )
      {
        d = d / Vml.DegreeDivider;
        //d = Vml.DarkLimit + d / Vml.DegreeDivider;
      }
      else
      {
        //d = d / Vml.DegreeDivider;
        d = ( d - Vml.DarkLimit ) / Vml.DegreeDivider;
      }
      return d;
    }
    /// <summary>
    /// Extract the preset value from the resource file.
    /// </summary>
    /// <param name="preset">preset in excel format</param>
    /// <returns>preset in Enum </returns>
    private ExcelGradientPreset ExtractPreset( string preset )
    {
      ExcelGradientPreset[] arrPresets =
        new ExcelGradientPreset[]
      {
        ExcelGradientPreset.Grad_Early_Sunset,
        ExcelGradientPreset.Grad_Late_Sunset,
        ExcelGradientPreset.Grad_Nightfall,
        ExcelGradientPreset.Grad_Daybreak,
        ExcelGradientPreset.Grad_Horizon,
        ExcelGradientPreset.Grad_Desert,
        ExcelGradientPreset.Grad_Ocean,
        ExcelGradientPreset.Grad_Calm_Water,
        ExcelGradientPreset.Grad_Fire,
        ExcelGradientPreset.Grad_Fog,
        ExcelGradientPreset.Grad_Moss,
        ExcelGradientPreset.Grad_Peacock,
        ExcelGradientPreset.Grad_Wheat,
        ExcelGradientPreset.Grad_Parchment,
        ExcelGradientPreset.Grad_Mahogany,
        ExcelGradientPreset.Grad_Rainbow,
        ExcelGradientPreset.Grad_RainbowII,
        ExcelGradientPreset.Grad_Gold,
        ExcelGradientPreset.Grad_GoldII,
        ExcelGradientPreset.Grad_Brass,
        ExcelGradientPreset.Grad_Chrome,
        ExcelGradientPreset.Grad_ChromeII,
        ExcelGradientPreset.Grad_Silver,
        ExcelGradientPreset.Grad_Sapphire,
      };

      ResourceManager manager = new ResourceManager( "Syncfusion.XlsIO.VMLPresetGradientFills", typeof( VmlTextBoxBaseParser )
#if ( WINRT )
         .GetTypeInfo().Assembly
#else
          .Assembly 
#endif
         );

      for( int i = 0, len = arrPresets.Length; i < len; i++ )
      {
        if( manager.GetString( arrPresets[ i ].ToString() ).Equals( preset ) )
          return arrPresets[ i ];
      }
      throw new IndexOutOfRangeException( "Presets" );
    }
    /// <summary>
    /// Extract the Pattern value from the resource file.
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    private ExcelPattern ExtractLinePattern( long length )
    {
      ///title attribute empty only relid attribute available

      //string[] arrPatterns = Enum.GetNames(typeof(ExcelGradientPattern));
      //ResourceManager manager = new ResourceManager("Syncfusion.XlsIO.TexturePatternGradient", typeof(VmlTextBoxBaseParser).Assembly);
      //long lengthOfImage;
      //for (int i = 1, len = arrPatterns.Length; i < len; i++)
      //{

      //    lengthOfImage = ((byte [])manager.GetObject(RESOURCE_PATTERN_PREFIX+i)).Length;
      //    if (lengthOfImage == length)
      //        break;
      //}
      //    return ExcelPattern.Angle; 
      throw new NotImplementedException( "pattern" );
    }
    /// <summary>
    /// Extract the Shading Variants
    /// </summary>
    /// <param name="focus">focus is an angle in string</param>
    /// <returns>Variant in Enum</returns>
    private ExcelGradientVariants ExtractShadingVariant( string focus )
    {
      int focusValue = Convert.ToInt32( RemoveCharUnSafeAt( focus, true ) );
      switch( focusValue )
      {
        case GradientVariant_1:
          return ExcelGradientVariants.ShadingVariants_1;
        case GradientVariant_3:
          return ExcelGradientVariants.ShadingVariants_3;
        case GradientVariant_4:
          return ExcelGradientVariants.ShadingVariants_4;
        default:
          return ExcelGradientVariants.ShadingVariants_2;
      }
    }
    /// <summary>
    /// Extract the Texture Name from the title string
    /// </summary>
    /// <param name="title">Texture name in string</param>
    /// <returns>texture in enum</returns>
    private ExcelTexture ExtractTexture( string title )
    {
      title = title.Replace( ' ', '_' );
      try
      {
        return ( ExcelTexture )Enum.Parse( typeof( ExcelTexture ), title, true );
      }
      catch( Exception )
      {
        return ExcelTexture.User_Defined;
      }
    }
    /// <summary>
    /// Extract the Pattern Name 
    /// </summary>
    /// <param name="title">Patterns color string</param>
    /// <returns>pattern in enum</returns>
    private ExcelGradientPattern ExtractPattern( string title )
    {
      title = PatternPrefix + title.Replace( ' ', '_' );
      try
      {
        return ( ExcelGradientPattern )Enum.Parse( typeof( ExcelGradientPattern ), title, true );
      }
      catch( Exception )
      {
        return ExcelGradientPattern.Pat_10_Percent;
      }
    }
    /// <summary>
    /// Extract Dash style from enum
    /// </summary>
    /// <param name="dashStyle">dash style to enum</param>
    /// <returns>dash style in enum</returns>
    private ExcelShapeDashLineStyle ExtractDashStyle( string dashStyle )
    {
      if( m_excelDashLineStyle == null )
        InitDashLineStyle();
      if( !Char.IsDigit( dashStyle[ 0 ] ) )
      {
        return m_excelDashLineStyle[ dashStyle ];
      }
      else
        return ExcelShapeDashLineStyle.Dotted_Round;
    }
    /// <summary>
    /// Exctract the line style from enum
    /// </summary>
    /// <param name="lineStyle">line style to extract</param>
    /// <returns>line style in enum</returns>
    private ExcelShapeLineStyle ExtractLineStyle( string lineStyle )
    {
      if( m_excelShapeLineStyle == null )
        InitShapeLineStyle();
      return m_excelShapeLineStyle[ lineStyle ];
    }
    #endregion
  }
}
