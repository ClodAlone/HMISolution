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
using Syncfusion.XlsIO.Implementation.XmlSerialization.Shapes;
using Syncfusion.XlsIO.Implementation.XmlSerialization.Constants;
using Syncfusion.XlsIO.Implementation.XmlSerialization.Charts;
using Syncfusion.XlsIO.Implementation.Shapes;
using Syncfusion.XlsIO.Interfaces;
using System.Diagnostics;
#if ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;
#endif
#if ( WINRT )
using Windows.UI;
using Rectangle=Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
#endif

namespace Syncfusion.XlsIO.Implementation.XmlReaders.Shapes
{
  /// <summary>
  /// This class is used to parse text box shape.
  /// </summary>
  class TextBoxShapeParser
  {
    #region Methods
    /// <summary>
    /// Extracts text box settings from XmlReader.
    /// </summary>
    /// <param name="textBox">TextBox to fill with settings.</param>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="parser">Parser used to help in parsing process.</param>
    public static void ParseTextBox( ITextBox textBox, XmlReader reader,
      Excel2007Parser parser )
    {
      if( textBox == null )
        throw new ArgumentNullException( "textBox" );

      if( reader == null )
        throw new ArgumentNullException( "reader" );

      reader.Read();
      ShapeImpl shape = ( textBox as ShapeImpl );
      shape.HasLineFormat = false;
      shape.HasFill = false;

      while( reader.NodeType != XmlNodeType.None )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case Drawings.TextBody:
              // On the current moment we don't support
              ParseRichText( reader, parser, textBox );
              break;

            case Drawings.ShapePropertiesTag:
              ParseShapeProperties( textBox, reader, parser );
              break;

            case Drawings.NonVisualShapeProperties:
              ParseNonVisualShapeProperties( textBox as IShape, reader, parser );
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
    }
    /// <summary>
    /// Parses non visual shape properties.
    /// </summary>
    /// <param name="shape">Shape to be parsed.</param>
    /// <param name="reader">XML reader to extract data from.</param>
    /// <param name="parser">Parser used to help in parsing process.</param>
    private static void ParseNonVisualShapeProperties( IShape shape, XmlReader reader, Excel2007Parser parser )
    {
      if( shape == null )
        throw new ArgumentNullException( "shape" );

      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( parser == null )
        throw new ArgumentNullException( "parser" );

      if( !reader.IsEmptyElement )
      {
        reader.Read();

        while( reader.NodeType != XmlNodeType.EndElement )
        {
          if( reader.NodeType == XmlNodeType.Element )
          {
            switch( reader.LocalName )
            {

              case Drawings.NVCanvasPropertiesTag:
                Excel2007Parser.ParseNVCanvasProperties( reader, shape );
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
      }

      reader.Read();
    }
    /// <summary>
    /// Parses shape properties.
    /// </summary>
    /// <param name="textBox">TextBox to be parsed.</param>
    /// <param name="reader">XML reader to extract data from.</param>
    /// <param name="parser">Parser used to help in parsing process.</param>
    private static void ParseShapeProperties( ITextBox textBox, XmlReader reader, Excel2007Parser parser )
    {
      reader.Read();
      TextBoxShapeImpl typedTextBox = ( TextBoxShapeImpl )textBox;

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case Drawings.SolidFillTag:
              IInternalFill fill = typedTextBox.Fill as IInternalFill;
              ChartParserCommon.ParseSolidFill( reader, parser, fill.ForeColorObject );
              break;

            case Drawings.LineTag:
              ShapeLineFormatImpl line = ( ShapeLineFormatImpl )typedTextBox.Line;
              TextBoxShapeParser.ParseLineProperties( reader, line, false, parser );
              break;

            case Drawings.Transform2DTag:
              if (reader.MoveToAttribute(Drawings.RotationAttribute))
                  typedTextBox.ShapeRotation = (int)(Convert.ToInt64(reader.Value) / 60000);
              typedTextBox.Coordinates2007 = ParseForm( reader );
              break;

            default:
              reader.Skip();
              break;
          }
        }
      }
      //throw new Exception( "The method or operation is not implemented." );
    }

    private static Rectangle ParseForm( XmlReader reader )
    {
      Rectangle result = Rectangle.Empty;

      if( !reader.IsEmptyElement )
      {
        reader.Read();
        int x = 0;
        int y = 0;
        int cx = 0;
        int cy = 0;

        while( reader.NodeType != XmlNodeType.EndElement )
        {
          if( reader.NodeType == XmlNodeType.Element )
          {
            switch( reader.LocalName )
            {
              case Drawings.Offset:
                if( reader.MoveToAttribute( Drawings.XAttributeName ) )
                  x = XmlConvert.ToInt32( reader.Value );

                if( reader.MoveToAttribute( Drawings.YAttributeName ) )
                  y = XmlConvert.ToInt32( reader.Value );
                break;

              case Drawings.Extents:
                if( reader.MoveToAttribute( Drawings.CXAttributeName ) )
                  cx = XmlConvert.ToInt32( reader.Value );

                if( reader.MoveToAttribute( Drawings.CYAttributeName ) )
                  cy = XmlConvert.ToInt32( reader.Value );
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

        result = new Rectangle( x, y, cx, cy );
      }

      reader.Read();
      return result;
    }
    #endregion

    #region Duplicated methods - please remove one of those copies
    /// <summary>
    /// Parses rich text.
    /// </summary>
    /// <param name="reader">XmlReader to read rich text from.</param>
    /// <param name="parser">Parser object that helps to extract data.</param>
    /// <param name="textBox">TextBox to be parsed.</param>
    private static void ParseRichText( XmlReader reader, Excel2007Parser parser,
      ITextBox textBox )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( textBox == null )
        throw new ArgumentNullException( "textBox" );

      RichTextString textArea = textBox.RichText as RichTextString;

      if( reader.MoveToAttribute( Drawings.LockTextAttribute ) )
        textBox.IsTextLocked = XmlConvert.ToBoolean( reader.Value );

      reader.Read();

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case Drawings.TextBodyPropertiesTag:
              ParseBodyProperties( reader, textArea, textBox );
              break;

            case Drawings.ListStylesTag:
              ParseListStyles( reader, textArea );
              break;

            case Drawings.Paragraphs:
              ParseParagraphs( reader, textBox, parser );
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

      textArea.TextObject.Defragment();
      reader.Read();
    }
    /// <summary>
    /// Parses text area body properties.
    /// </summary>
    /// <param name="reader">XmlReader to read body properties from.</param>
    /// <param name="textArea">Text area to put body properties into.</param>
    /// <param name="textBox">Textbox to be parsed.</param>
    private static void ParseBodyProperties( XmlReader reader, RichTextString textArea, ITextBox textBox )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( textArea == null )
        throw new ArgumentNullException( "textArea" );

      if( reader.LocalName != Drawings.TextBodyPropertiesTag )
        throw new XmlException( "Unexpected xml tag." );

      // TODO: on the current moment we don't support body properties.
      if( reader.HasAttributes )
      {
        Dictionary<string, string> unknownAttributes = new Dictionary<string, string>();
        reader.MoveToFirstAttribute();

        for( int i = 0, len = reader.AttributeCount; i < len; i++ )
        {
          unknownAttributes[ reader.LocalName ] = reader.Value;
          reader.MoveToNextAttribute();
        }

        if( unknownAttributes.ContainsKey( Drawings.AnchorAttribute ) )
        {
          ParseAnchor( unknownAttributes[ Drawings.AnchorAttribute ], textBox );
          unknownAttributes.Remove( Drawings.AnchorAttribute );
        }

        if( unknownAttributes.ContainsKey( Drawings.TextBoxRotationAttribute ) )
        {
          ParseTextRotation( unknownAttributes[ Drawings.TextBoxRotationAttribute ], textBox );
          unknownAttributes.Remove( Drawings.TextBoxRotationAttribute );
        }

        unknownAttributes.Remove( "a" );

        ( textBox as TextBoxShapeBase ).UnknownBodyProperties = unknownAttributes;
      }

      reader.MoveToElement();
      reader.Skip();
    }
    /// <summary>
    /// Parses text rotation value.
    /// </summary>
    /// <param name="rotationValue">Rotation value to parse.</param>
    /// <param name="textBox">TextBox to put extracted value into.</param>
    private static void ParseTextRotation( string rotationValue, ITextBox textBox )
    {
      Excel2007TextRotation rotation = ( Excel2007TextRotation )Enum.Parse(
        typeof( Excel2007TextRotation ), rotationValue, false );

      textBox.TextRotation = ( ExcelTextRotation )rotation;
    }
    /// <summary>
    /// Parses anchor (vertical alignment).
    /// </summary>
    /// <param name="anchorValue">Anchor value to parse.</param>
    /// <param name="textBox">TextBox to put extracted value into.</param>
    private static void ParseAnchor( string anchorValue, ITextBox textBox )
    {
      Excel2007CommentVAlign valign = ( Excel2007CommentVAlign )Enum.Parse(
        typeof( Excel2007CommentVAlign ), anchorValue, false );

      textBox.VAlignment = ( ExcelCommentVAlign )valign;
    }
    /// <summary>
    /// Extracts list styles for a text area.
    /// </summary>
    /// <param name="reader">XmlReader to extract list styles from.</param>
    /// <param name="textArea">Text area that will get extracted settings.</param>
    private static void ParseListStyles( XmlReader reader, RichTextString textArea )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( textArea == null )
        throw new ArgumentNullException( "textArea" );

      if( reader.LocalName != Drawings.ListStylesTag )
        throw new XmlException( "Unexpected xml tag." );

      reader.Skip();
    }
    /// <summary>
    /// Parses paragraph.
    /// </summary>
    /// <param name="reader">XmlReader to get paragraph tag from.</param>
    /// <param name="textBox">Text box that needs its paragraph text to be parsed.</param>
    /// <param name="parser">Parser object that helps to extract data.</param>
    private static void ParseParagraphs( XmlReader reader, ITextBox textBox, Excel2007Parser parser )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( textBox == null )
        throw new ArgumentNullException( "textBox" );

      if( reader.LocalName != Drawings.Paragraphs )
        throw new XmlException( "Unexpected xml tag." );

      RichTextString textArea = textBox.RichText as RichTextString;
      string strCurrentText = textArea.Text;

      if( strCurrentText != null && strCurrentText.Length != 0 && !strCurrentText.EndsWith( "\n" ) )
        textArea.AddText( "\n", textArea.GetFont( strCurrentText.Length - 1 ) );

      //writer.WriteStartElement( Drawings.Paragraphs, Drawings.ANamespace );
      reader.Read();
      bool bDataAdded = false;

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case Drawings.ParagraphProperties:
              ParseParagraphProperites( reader, textBox );
              break;

            case Drawings.ParagraphRun:
              bDataAdded = true;
              ParseParagraphRun( reader, textArea, parser );
              break;

            case Drawings.ParagraphEndProperties:
              ParseParagraphEnd( reader, textArea, parser );
              break;

            case "fld":
              ParseTextField(reader, textBox);
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

      //if( !bDataAdded )
      //  textArea.AddText( "\n", textArea.DefaultFont );

      reader.Read();
    }

    private static void ParseTextField(XmlReader reader, ITextBox textBox)
    {
        TextBoxShapeImpl textBoxShape = textBox as TextBoxShapeImpl;
        if (textBoxShape != null)
        {
            if (reader.MoveToAttribute(Drawings.IdAttributeName))
                textBoxShape.FieldId = reader.Value;

            if (reader.MoveToAttribute(SparkConstants.SparklineTypeAttribute))
                textBoxShape.FieldType = reader.Value;

            reader.Skip();
            
        }
    }

    internal static void ParseParagraphEnd( XmlReader reader, RichTextString textArea, Excel2007Parser parser )
    {
      IFont font = ParseParagraphRunProperites( reader, textArea, parser );
      textArea.AddText( "\n", font );
    }
    /// <summary>
    /// Extracts paragraph properties
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="textBox">TextBox to put data into.</param>
    private static void ParseParagraphProperites( XmlReader reader, ITextBox textBox )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( textBox == null )
        throw new ArgumentNullException( "textBox" );

      if( reader.MoveToAttribute( Drawings.HorizontalAlignment ) )
      {
        Excel2007CommentHAlign align = ( Excel2007CommentHAlign )Enum.Parse(
          typeof( Excel2007CommentHAlign ), reader.Value, false );

        textBox.HAlignment = ( ExcelCommentHAlign )align;
      }

      reader.MoveToElement();
      reader.Skip();
    }
    /// <summary>
    /// Parses paragraph run.
    /// </summary>
    /// <param name="reader">XmlReader to get paragraph run from.</param>
    /// <param name="textArea">Text area to put extracted properties into.</param>
    /// <param name="parser">Parser object that helps to extract data.</param>
    internal static void ParseParagraphRun( XmlReader reader, RichTextString textArea, Excel2007Parser parser )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( textArea == null )
        throw new ArgumentNullException( "textArea" );

      if( reader.LocalName != Drawings.ParagraphRun )
        throw new XmlException( "Unexpected xml tag." );

      reader.Read();
      string text = null;
      IFont font = null;

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        // TODO: our text area support only one formatting, so we are serializing only one paragraph run.
        //ChartTextAreaImpl textAreaImpl = ( ChartTextAreaImpl )textArea;

        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case Drawings.TextRunProperites:
              font = ParseParagraphRunProperites( reader, textArea, parser );
              break;

            case Drawings.ParagraphText:
              string textPart = reader.ReadElementContentAsString();//ReadElementString();
              text = textPart;
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

      if( text == null || text.Length == 0 )
        text = "\n";

      textArea.AddText( text, font );
      reader.Read();
    }
    /// <summary>
    /// Parses paragraph run.
    /// </summary>
    /// <param name="reader">XmlReader to extract paragraph tag from.</param>
    /// <param name="textArea">Text area that will get paragraph run information (formatting and text).</param>
    /// <param name="parser">Parser object that helps to extract data.</param>
    /// <returns>Font attributes of the paragraph run.</returns>
    private static FontImpl ParseParagraphRunProperites( XmlReader reader,
      RichTextString textArea, Excel2007Parser parser )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( textArea == null )
        throw new ArgumentNullException( "textArea" );

      //if( reader.LocalName != Drawings.TextRunProperites )
      //  throw new XmlException( "Unexpected xml tag." );

      WorkbookImpl book = textArea.Workbook;
      FontImpl font = ( FontImpl )book.CreateFont( book.InnerFonts[ 0 ], false );

      if( reader.MoveToAttribute( Drawings.FontBoldAttribute ) )
        font.Bold = XmlConvert.ToBoolean( reader.Value );

      if( reader.MoveToAttribute( Drawings.FontItalicAttribute ) )
        font.Italic = XmlConvert.ToBoolean( reader.Value );

      if( reader.MoveToAttribute( Drawings.FontStrikeAttribute ) )
        font.Strikethrough = reader.Value != ChartConstants.StrikeThroughNone;

      if( reader.MoveToAttribute( Drawings.FontSizeAttribute ) )
        font.Size = int.Parse( reader.Value ) / 100.0;

      if( reader.MoveToAttribute( Drawings.FontUnterlineAttribute ) )
      {
        if( reader.Value == ChartConstants.UnderlineSingle )
        {
          font.Underline = ExcelUnderline.Single;
        }
        else if( reader.Value == ChartConstants.UnderlineDouble )
        {
          font.Underline = ExcelUnderline.Double;
        }
      }

      font.showFontName = false;

      // TODO: add color parsing later.

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
              case Drawings.LatinTag:
                if( reader.MoveToAttribute( Drawings.TypefaceTag ) )
                {
                  font.FontName = reader.Value;
                  reader.MoveToElement();
                }
                font.showFontName = true;
                reader.Skip();
                break;

              case Drawings.SolidFillTag:
                Syncfusion.XlsIO.Implementation.XmlSerialization.Charts.ChartParserCommon.ParseSolidFill( reader, parser, font.ColorObject );
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

      return font;
    }
    /// <summary>
    /// Serialize line properties.
    /// </summary>
    /// <param name="reader">XmlReader to serialize into.</param>
    /// <param name="border">Chart line properties to serialize.</param>
    /// <param name="bRoundCorners">Indicates whether border is rounded or not</param>
    /// <param name="parser">Excel2007Parser to help in extraction process.</param>
    internal static void ParseLineProperties( XmlReader reader, ShapeLineFormatImpl border,
      bool bRoundCorners, Excel2007Parser parser )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( border == null )
        throw new ArgumentNullException( "border" );

      if( reader.LocalName != Drawings.LineTag )
        throw new XmlException( "Unexpected xml tag" );

      //reader.Read();
      bool bEmpty = reader.IsEmptyElement;

      if( reader.MoveToAttribute( Drawings.LineWidthAttribute ) )
      {
        int iLineWieght = ( int )Math.Round( int.Parse( reader.Value ) / ShapeImpl.LineWieghtMultiplier );
        border.Weight = iLineWieght;
      }

      if( reader.MoveToAttribute( Drawings.CompoundLineTypeAttribute ) )
      {
        Excel2007ShapeLineStyle lineStyle = ( Excel2007ShapeLineStyle )Enum.Parse(
          typeof( Excel2007ShapeLineStyle ), reader.Value, false );
        border.Style = ( ExcelShapeLineStyle )lineStyle;
      }

      bool bSolid = false;
      string strPresetDash = null;

      if( !bEmpty )
      {
        reader.Read();

        while( reader.NodeType != XmlNodeType.EndElement )
        {
          if( reader.NodeType == XmlNodeType.Element )
          {
            switch( reader.LocalName )
            {
              case Drawings.NoFillTag:
                border.Weight = 0;
                //border.Pattern = ExcelGradientPattern.None;
                reader.Read();
                break;

              case Drawings.RoundTag:
                border.IsRound = true;
                reader.Skip();
                break;
                throw new NotImplementedException();
              //break;

              case Drawings.SolidFillTag:
                ColorObject color = new ColorObject( ExcelKnownColors.None );
                ChartParserCommon.ParseSolidFill( reader, parser, color );
                border.ForeColor = color.GetRGB( border.Workbook );
                bSolid = true;
                break;

              case Drawings.PresetDashTag:
                strPresetDash = ChartParserCommon.ParseValueTag( reader );
                break;

              case Drawings.PatternFillTag:
                //Debug.Fail( "Pattern fill parsing is not implemented" );
                reader.Skip();
                break;

                case Drawings.HeadEnd:
                ParseArrowSettings(reader, border, true);
                break;

                case Drawings.TailEnd:
                ParseArrowSettings(reader, border, false);
                break;

              default:
                //throw new NotImplementedException();
                //Debug.Fail( "Unsupported tag name " + reader.LocalName );
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
      else
      {
        bSolid = true;
      }

      if( bSolid )
      {
        //border.Style = ExcelShapeLineStyle.Line_Single;//ExcelChartLinePattern.Solid;
        border.DashStyle = ExcelShapeDashLineStyle.Solid;

        if (strPresetDash != null)
        {            
            ExcelShapeDashLineStyle linePattern;

            if (border.AppImplementation.StringEnum.LineDashTypeXmltoEnum.TryGetValue(strPresetDash,out linePattern));
            {
                border.DashStyle = linePattern;
            }

        }
      }

      reader.Read();
    }

    private static void ParseArrowSettings(XmlReader reader, ShapeLineFormatImpl border,bool isHead)
    {
        if (reader.HasAttributes)
        {
            if (reader.MoveToAttribute("len"))
            {
                if (isHead)
                {
                    border.BeginArrowheadLength = GetHeadLength(reader.Value);
                }
                else
                {
                    border.EndArrowheadLength = GetHeadLength(reader.Value);
                }
            }
            if (reader.MoveToAttribute("type"))
            {
                if (isHead)
                {
                    border.BeginArrowHeadStyle = GetHeadStyle(reader.Value);
                }
                else
                {
                    border.EndArrowHeadStyle = GetHeadStyle(reader.Value);
                }
            }
            if (reader.MoveToAttribute("w"))
            {
                if (isHead)
                {
                    border.BeginArrowheadWidth = GetHeadWidth(reader.Value);
                }
            }
        }
        reader.Skip();
    }

    private static ExcelShapeArrowWidth GetHeadWidth(string value)
    {
        switch (value)
        {
            case "lg":
                return ExcelShapeArrowWidth.ArrowHeadWide;

            case "med":
                return ExcelShapeArrowWidth.ArrowHeadMedium;

            case "sm":
                return ExcelShapeArrowWidth.ArrowHeadNarrow;
        }
        return ExcelShapeArrowWidth.ArrowHeadMedium;
    }

    private static ExcelShapeArrowStyle GetHeadStyle(string value)
    {
        switch (value)
        {
            case "arrow":
                return ExcelShapeArrowStyle.LineArrowOpen;
            case "diamond":
                return ExcelShapeArrowStyle.LineArrowDiamond;
            case "none":
                return ExcelShapeArrowStyle.LineNoArrow;
            case "oval":
                return ExcelShapeArrowStyle.LineArrowOval;
            case "stealth":
                return ExcelShapeArrowStyle.LineArrowStealth;
            case "triangle":
                return ExcelShapeArrowStyle.LineArrow;
        }
        return ExcelShapeArrowStyle.LineNoArrow;
    }

    private static ExcelShapeArrowLength GetHeadLength(string value)
    {
        switch (value)
        {
            case "lg":
                return ExcelShapeArrowLength.ArrowHeadLong;

            case "med":
                return ExcelShapeArrowLength.ArrowHeadMedium;

            case "sm":
                return ExcelShapeArrowLength.ArrowHeadShort;
        }
        return ExcelShapeArrowLength.ArrowHeadMedium;
    }
    #endregion
  }
}
