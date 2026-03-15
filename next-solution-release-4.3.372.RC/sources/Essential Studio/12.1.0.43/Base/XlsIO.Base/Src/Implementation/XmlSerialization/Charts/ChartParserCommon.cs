#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using Syncfusion.XlsIO.Implementation.XmlSerialization.Shapes;
using Syncfusion.XlsIO.Implementation.XmlSerialization.Constants;
using Syncfusion.XlsIO.Implementation.Charts;
using Syncfusion.XlsIO.Implementation.Shapes;
using Syncfusion.XlsIO.Implementation.XmlReaders;
using Syncfusion.XlsIO.Parser.Biff_Records.Charts;

using Syncfusion.XlsIO.Interfaces.Charts;
using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Implementation.XmlReaders.Shapes;


#if ( WINRT )
using Windows.UI;
using Syncfusion.XlsIO.Implementation.WINRT;
using Syncfusion.XlsIO.Silverlight.Implementation.Extensions;
#endif

#if  (SILVERLIGHT)
using System.Windows.Media;
using Syncfusion.XlsIO.Silverlight.Implementation.Extensions;
using Syncfusion.XlsIO.Implementation.Silverlight;
#elif WP
using System.Windows.Media;
using Syncfusion.XlsIO.WP.Implementation.Extensions;
using Syncfusion.XlsIO.Implementation.WP;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;
#endif


namespace Syncfusion.XlsIO.Implementation.XmlSerialization.Charts
{
  /// <summary>
  /// Class used for parsing charts.
  /// </summary>
  class ChartParserCommon
  {
    #region Constants
    private const string NullString = "null";
    private const int DefaultShadowSize = 100;
    private const int DefaultBlurValue = 0;
    private const int DefaultAngleValue = 0;
    private const int DefaultDistanceValue = 0;
    #endregion

    #region Members
    /// <summary>
    /// Contains chart line pattern values.
    /// </summary>
    private static Dictionary<KeyValuePair<string, string>, ExcelChartLinePattern> s_dicLinePatterns =
      new Dictionary<KeyValuePair<string, string>, ExcelChartLinePattern>();
        private static WorkbookImpl m_book;
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes static members of the ChartParserCommon class.
    /// </summary>
    static ChartParserCommon()
    {
      s_dicLinePatterns.Add( new KeyValuePair<string, string>( "solid", string.Empty ), ExcelChartLinePattern.Solid );
      s_dicLinePatterns.Add( new KeyValuePair<string, string>( "lgDash", string.Empty ), ExcelChartLinePattern.Dash );
      s_dicLinePatterns.Add( new KeyValuePair<string, string>( "sysDash", string.Empty ), ExcelChartLinePattern.Dot );
      s_dicLinePatterns.Add( new KeyValuePair<string, string>( "sysDot", string.Empty ), ExcelChartLinePattern.CircleDot );
      s_dicLinePatterns.Add( new KeyValuePair<string, string>( "dash", string.Empty ), ExcelChartLinePattern.Dash );
      s_dicLinePatterns.Add( new KeyValuePair<string, string>( "lgDashDot", string.Empty ), ExcelChartLinePattern.DashDot );
      s_dicLinePatterns.Add( new KeyValuePair<string, string>( "lgDashDotDot", string.Empty ), ExcelChartLinePattern.DashDotDot );
      s_dicLinePatterns.Add( new KeyValuePair<string, string>( "solid", "pct75" ), ExcelChartLinePattern.DarkGray );
      s_dicLinePatterns.Add( new KeyValuePair<string, string>( "solid", "pct50" ), ExcelChartLinePattern.MediumGray );
      s_dicLinePatterns.Add( new KeyValuePair<string, string>( "solid", "pct25" ), ExcelChartLinePattern.LightGray );
    }
    #endregion

    #region Methods
    //    /// <summary>
//    /// Serializes frame format.
//    /// </summary>
//    /// <param name="writer">XmlWriter to serialize into.</param>
//    /// <param name="format">Fill format to serialize.</param>
//    /// <param name="chart">Parent chart object.</param>
//    /// <param name="isRoundCorners">Indicates whether area corners should be rounded.</param>
//    public static void SerializeFrameFormat( XmlWriter writer, IChartFillBorder format,
//      ChartImpl chart, bool isRoundCorners )
//    {
//      if( writer == null )
//        throw new ArgumentNullException( "writer" );

//      if( chart == null )
//        throw new ArgumentNullException( "chart" );

//      if( format == null )
//        return;

//      WorksheetDataHolder sheetHolder = chart.DataHolder;
//      FileDataHolder holder = sheetHolder.ParentHolder;
//      RelationsCollection relations = chart.Relations;
//      SerializeFrameFormat( writer, format, holder, relations, isRoundCorners );
//    }
//    /// <summary>
//    /// Serializes frame format.
//    /// </summary>
//    /// <param name="writer">XmlWriter to serialize into.</param>
//    /// <param name="format">Fill format to serialize.</param>
//    /// <param name="holder">Parent file data holder.</param>
//    /// <param name="relations">Chart's relations collection</param>
//    /// <param name="isRoundCorners">Indicates whether area corners should be rounded.</param>
//    public static void ParseFrameFormat( XmlReader reader, IChartFillBorder format,
//      FileDataHolder holder, RelationsCollection relations, bool isRoundCorners )
//    {
//      if( reader == null )
//        throw new ArgumentNullException( "reader" );

//      //if( format == null )
//      //  return;

//      if( reader.LocalName != Drawings.ShapePropertiesTag )
//        throw new XmlException( "Unexpected xml tag." );

//      reader.Read();

//      while( reader.NodeType != XmlNodeType.EndElement )
//      {
//        if( reader.NodeType == XmlNodeType.Element )
//        {
//          switch( reader.LocalName )
//          {
//            case Drawings.GradientFillTag:
//              ParseGradientFill( writer, fill );
//              break;

//            case Drawings.PatternFillTag:
//              ParsePatternFill( writer, fill.ForeColor, false, fill.BackColor, false, fill.Pattern );
//              break;

//            case Drawings.SolidFillTag:
//              ParseSolidFill( writer, fill.ForeColor, false );
//              break;

//            case Drawings.BlipFillTagName:
//              ParsePictureFill( writer, fill.Picture, holder, relations, false );
//              break;

//            case Drawings.LineTag:
//              IChartBorder border = format.LineProperties;
//              ParseLineProperties( reader, border, isRoundCorners );
//              break;

//            default:
//              reader.Skip();
//              break;
//          }
//        }
//        else
//        {
//          reader.Skip();
//        }
//      }

//      reader.Read();
//    }

        public static void SetWorkbook(WorkbookImpl book)
        {
            m_book = book;
        }
    /// <summary>
    /// Parses text area.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="textArea">Text area to put extracted data into.</param>
    /// <param name="holder">Parent data holder object.</param>
    /// <param name="relations">Chart's relations.</param>
    public static void ParseTextArea( XmlReader reader, IInternalChartTextArea textArea,
      FileDataHolder holder, RelationCollection relations )
    {
      ParseTextArea( reader, textArea, holder, relations, null );
    }
    /// <summary>
    /// Parses text area.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="textArea">Text area to put extracted data into.</param>
    /// <param name="holder">Parent data holder object.</param>
    /// <param name="relations">Chart's relations.</param>
    /// <param name="defaultFontSize">Default font size.</param>
    public static void ParseTextArea( XmlReader reader, IInternalChartTextArea textArea,
      FileDataHolder holder, RelationCollection relations, float? defaultFontSize )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( textArea == null )
        throw new ArgumentNullException( "textArea" );

      //if( reader.LocalName != ChartConstants.TitleTag )
      //  throw new XmlException( "Unexpected xml tag." );
      if (!reader.IsEmptyElement)
      {
        reader.Read();

        while( reader.NodeType != XmlNodeType.EndElement )
        {
          ParseTextAreaTag( reader, textArea, relations, holder, defaultFontSize );
        }
      }

      reader.Read();
    }
    /// <summary>
    /// Parses text area tag.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="textArea">Text area to put extracted data into.</param>
    /// <param name="holder">Parent data holder object.</param>
    /// <param name="relations">Chart's relations.</param>
    /// <param name="defaultFontSize">Default font size.</param>
    public static void ParseTextAreaTag( XmlReader reader, IInternalChartTextArea textArea,
      RelationCollection relations, FileDataHolder holder, float? defaultFontSize )
    {
      if( reader.NodeType == XmlNodeType.Element )
      {
        switch( reader.LocalName )
        {
          case ChartConstants.ChartTextTag:
            ParseTextAreaText( reader, textArea, holder.Parser, defaultFontSize );
            break;

          case ChartConstants.LayoutTag:
            (textArea as ChartTextAreaImpl).Layout = new ChartLayoutImpl(m_book.Application, (textArea as ChartTextAreaImpl), textArea.Parent);
            ParseChartLayout(reader, (textArea as ChartTextAreaImpl).Layout);
            break;

          //ParseLayout( writer, textArea );
          //ParseOverlay( writer, textArea );

          case Drawings.ShapePropertiesTag:
            IChartFrameFormat frameFormat = textArea.FrameFormat;
            IChartFillObjectGetter getter = new ChartFillObjectGetterAny(
              frameFormat.Border as ChartBorderImpl,
              frameFormat.Interior as ChartInteriorImpl,
                            frameFormat.Fill as IInternalFill,
                            frameFormat.Shadow as ShadowImpl,
                            frameFormat.ThreeD as ThreeDFormatImpl
                            );

            ParseShapeProperties( reader, getter, holder, relations );
            break;
            case ChartConstants.TextPropertiesTag:
            ((ChartTextAreaImpl)textArea).ParagraphType= ChartParagraphType.CustomDefault;
            ChartParserCommon.ParseDefaultTextFormatting(reader,textArea,holder.Parser,defaultFontSize);
            break;
          //ParseTextProperties( writer, textArea );
            case ChartConstants.OverlayTag:
            Stream overlayStream = ShapeParser.ReadNodeAsStream(reader);
            ((ChartTextAreaImpl)textArea).OverlayStream = overlayStream;
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
    //}
    //??
    //reader.Read();
  }
    /// <summary>
    /// Extracts default text formatting.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="textFormatting">Object with text formatting.</param>
    /// <param name="parser">Excel2007Parser to use if necessary.</param>
    private static void ParseDefaultTextFormatting(XmlReader reader,
      IInternalChartTextArea textFormatting, Excel2007Parser parser,double? defaultFontSize)
    {
        if (reader == null)
            throw new ArgumentNullException("reader");

        if (textFormatting == null)
            throw new ArgumentNullException("textFormatting");

        if (reader.LocalName != ChartConstants.TextPropertiesTag)
            throw new XmlException("Unexpected xml tag");

        reader.Read();

        while (reader.NodeType != XmlNodeType.EndElement
          && reader.LocalName != ChartConstants.TextPropertiesTag
          && reader.LocalName != Drawings.DefaultParagraphProperites)
        {
            switch (reader.LocalName)
            {
                case Drawings.TextBodyPropertiesTag:
                    if (reader.MoveToAttribute(Drawings.TextRotationAttribute))
                        textFormatting.TextRotationAngle = XmlConvert.ToInt32(reader.Value) / ChartAxisSerializator.TextRotationMultiplier;
                    reader.Skip();
                    break;

                default:
                    reader.Read();
                    break;
            }
        }
        if(defaultFontSize!=null)
            textFormatting.Size = (double)defaultFontSize;
        if (reader.LocalName == Drawings.DefaultParagraphProperites)
        {
            ChartParserCommon.ParseParagraphRunProperites(reader, textFormatting, parser, null);

            while (reader.LocalName != ChartConstants.TextPropertiesTag)
                reader.Read();
        }

        reader.Read();
    }
//    /// <summary>
//    /// Serialize xml tag that contains value attribute with tag value.
//    /// </summary>
//    /// <param name="writer">XmlWriter to serialize into.</param>
//    /// <param name="tagName">Tag name to serialize.</param>
//    /// <param name="value">Value to serialize.</param>
//    public static void SerializeValueTag( XmlWriter writer, string tagName, string value )
//    {
//      SerializeValueTag( writer, tagName, ChartConstants.CNamespace, value );
//    }
    /// <summary>
    /// Extracts value from XmlReader..
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <returns>Extracted value.</returns>
    public static string ParseValueTag( XmlReader reader )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      string strResult;

      if( reader.MoveToAttribute( ChartConstants.ValueAttribute ) )
      {
        strResult = reader.Value;
      }
      else
      {
        throw new XmlException();
      }

      reader.Read();
      return strResult;
    }
    /// <summary>
    /// Extracts boolean value from xml tag.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <returns>Extracted value.</returns>
    public static bool ParseBoolValueTag( XmlReader reader )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      string strValue = ParseValueTag( reader );
      return XmlConvert.ToBoolean( strValue );
    }
    /// <summary>
    /// Extracts int value from xml tag.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <returns>Extracted value.</returns>
    public static int ParseIntValueTag( XmlReader reader )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      string strValue = ParseValueTag( reader );
      return XmlConvert.ToInt32( strValue );
    }
    /// <summary>
    /// Extracts double value from xml tag.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <returns>Extracted value.</returns>
    public static double ParseDoubleValueTag( XmlReader reader )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      string strValue = ParseValueTag( reader );
      return XmlConvert.ToDouble( strValue );
    }
    /// <summary>
    /// Extracts Line properties of chart.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="border">Chart line properties to parse.</param>
    /// <param name="parser">Excel2007Parser to help in extraction process.</param>
    public static void ParseLineProperties( XmlReader reader, ChartBorderImpl border, Excel2007Parser parser )
    {
      ParseLineProperties( reader, border, false, parser );
    }
    /// <summary>
    /// Extracts pattern fill.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="fill">Fill to put extracted data into.</param>
    /// <param name="parser">Excel2007Parser to help in extraction process.</param>
    public static void ParsePatternFill( XmlReader reader, IFill fill, Excel2007Parser parser )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( reader.LocalName != Drawings.PatternFillTag )
        throw new XmlException( "Unexpected xml tag." );

      string strPreset = reader.MoveToAttribute( Drawings.PresetPattern ) ?
        reader.Value :
        null;

      Excel2007GradientPattern pattern2007 = ( strPreset != null ) ?
        ( Excel2007GradientPattern )Enum.Parse( typeof( Excel2007GradientPattern ), strPreset, false ) :
        Excel2007GradientPattern.dashDnDiag;

      ExcelGradientPattern pattern = ( ExcelGradientPattern )pattern2007;
      fill.Pattern = pattern;

      reader.MoveToElement();
      int iTransparency;
      int iTint;
      int iShade;
      Color color;

      if( !reader.IsEmptyElement )
      {
        reader.Read();

        while( reader.NodeType != XmlNodeType.EndElement )
        {
          if( reader.NodeType == XmlNodeType.Element )
          {
            switch( reader.LocalName )
            {
              case Drawings.ForegroundColorTag:
                reader.Read();
                color = ReadColor( reader, out iTransparency, out iTint, out iShade, parser );
                fill.ForeColor = color;
                reader.Read();
                break;

              case Drawings.BackgroundColorTag:
                reader.Read();
                color = ReadColor( reader, out iTransparency, out iTint, out iShade, parser );
                fill.BackColor = color;
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
      }

      reader.Read();
    }
    /// <summary>
    /// Extracts solid fill.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="interior">Interior to put extracted data into.</param>
    /// <param name="parser">Excel2007Parser to help in extraction process.</param>
    public static void ParseSolidFill( XmlReader reader, ChartInteriorImpl interior, Excel2007Parser parser, out int Alpha )
    {
      if( interior == null )
        throw new ArgumentNullException( "interior" );

      ParseSolidFill( reader, parser, interior.ForegroundColorObject, out Alpha );
      interior.UseAutomaticFormat = false;
    }
    /// <summary>
    /// Extracts solid fill.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="interior">Interior to put extracted data into.</param>
    /// <param name="parser">Excel2007Parser to help in extraction process.</param>
    public static void ParseSolidFill( XmlReader reader, ChartInteriorImpl interior, Excel2007Parser parser )
    {
      if( interior == null )
        throw new ArgumentNullException( "interior" );

      int Alpha;
      ParseSolidFill( reader, parser, interior.ForegroundColorObject, out Alpha );
    }
    /// <summary>
    /// Extracts solid fill.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="parser">Object that is used to help in color parsing.</param>
    /// <param name="color">Color object to put extracted color into.</param>
    public static void ParseSolidFill( XmlReader reader, Excel2007Parser parser, ColorObject color )
    {
      int alpha = ShapeFillImpl.MaxValue;
      ParseSolidFill( reader, parser, color, out alpha );
    }
    /// <summary>
    /// Extracts solid fill.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="parser">Object that is used to help in color parsing.</param>
    /// <param name="color">Color object to put extracted color into.</param>
    public static void ParseSolidFill( XmlReader reader, Excel2007Parser parser, ColorObject color, out int Alpha )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( reader.LocalName != Drawings.SolidFillTag )
        throw new XmlException( "Unexpected xml tag." );

      Alpha = ShapeFillImpl.MaxValue;
      int Tint;
      int Shade;

      if( !reader.IsEmptyElement )
      {
        reader.Read();

        while( reader.NodeType != XmlNodeType.EndElement )
        {
          if( reader.NodeType == XmlNodeType.Element )
          {
            switch( reader.LocalName )
            {
              case Drawings.SRGBColorTag:
                color.SetRGB( ParseSRgbColor( reader, out Alpha, out Tint, out Shade, parser ) );
                break;

              case Drawings.SchemeColorTag:
                color.SetRGB( ParseSchemeColor( reader, out Alpha, parser ) );
                break;

              case Drawings.SystemColorTag:
                color.SetRGB(ReadColor(reader, out Alpha, out Tint, out Shade, parser));
                break;

              case Drawings.PresetcolorTag:
                color.SetRGB(ParsePresetColor(reader, out Alpha, parser));
                break;

              default:
                //Debug.Fail( "Not implemented xml tag " + reader.LocalName );
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
    /// Extracts rgb color.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <returns>Extracted color.</returns>
    public static Color ParseSRgbColor( XmlReader reader, Excel2007Parser parser )
    {
      int iAlpha;
      int iTint;
      int iShade;

      return ParseSRgbColor( reader, out iAlpha, out iTint, out iShade, parser );
    }
    /// <summary>
    /// Extracts rgb color.
    /// </summary>
    /// <param name="reader">XmlReder to extract data from.</param>
    /// <param name="alpha">Alpha component of the extracted color (0-100000).</param>
    /// <param name="tint">Tint part of the extracted color (0-100000) or -1 if no tint part was present.</param>
    /// <param name="shade">Shade part of the extracted color (0-100000) or -1 if no shade part was present.</param>
    /// <returns>Extracted color.</returns>
    public static Color ParseSRgbColor( XmlReader reader, out int alpha, out int tint, out int shade, Excel2007Parser parser )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( reader.LocalName != Drawings.SRGBColorTag )
        throw new XmlException( "Unexpeced xml tag." );

      bool bEmpty = reader.IsEmptyElement;
      Color result = ColorExtension.Empty;
      alpha = ShapeFillImpl.MaxValue;
      tint = -1;
      shade = -1;

      if( reader.MoveToAttribute( ChartConstants.ValueAttribute ) )
      {
        string strColor = reader.Value;
        int iColor = int.Parse( strColor, System.Globalization.NumberStyles.HexNumber, null );
        result = ColorExtension.FromArgb( iColor );          
      }
      else
      {
        throw new XmlException();
      }

      reader.MoveToElement();
      reader.Read();

      if( !bEmpty )
      {
        while( reader.NodeType != XmlNodeType.EndElement )
        {
          if( reader.NodeType == XmlNodeType.Element )
          {
            switch( reader.LocalName )
            {
              case Drawings.AlphaTag:
                alpha = ParseIntValueTag( reader );
                break;

              case Drawings.GammaTag:
                reader.Skip();
                break;

              case Drawings.InverseGammaTag:
                reader.Skip();
                break;

              case Drawings.TintTag:
                tint = ParseIntValueTag( reader );
                break;

              case Drawings.ShadeTag:
                shade = ParseIntValueTag( reader );
                break;

              default:
                result = ParseColorUpdater( reader, result, parser, out alpha );
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
     
      return result;
    }
    /// <summary>
    /// Extracts scheme color and converts it into rgb.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="parser">Excel2007Parser to help in extraction process.</param>
    /// <returns>Extracted color.</returns>
    public static Color ParseSchemeColor( XmlReader reader, Excel2007Parser parser )
    {
      int iAlpha;
      return ParseSchemeColor( reader, out iAlpha, parser );
    }
    /// <summary>
    /// Extracts scheme color and converts it into rgb.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="alpha">Output alpha value.</param>
    /// <param name="parser">Excel2007Parser to help in extraction process.</param>
    /// <returns>Extracted color.</returns>
    public static Color ParseSchemeColor( XmlReader reader, out int alpha, Excel2007Parser parser )
    {
      if( reader == null )
        throw new ArgumentNullException();

      if( reader.LocalName != Drawings.SchemeColorTag )
        throw new XmlException( "Unexpected xml tag" );

      bool bEmpty = reader.IsEmptyElement;

      alpha = ShapeFillImpl.MaxValue;
      string strColorName = null;

      if( reader.MoveToAttribute( ChartConstants.ValueAttribute ) )
        strColorName = reader.Value;

      Color result = parser.GetThemeColor( strColorName );
      reader.MoveToElement();

      if( !bEmpty )
      {
        reader.Read();

        double dHue;
        double dLuminance;
        double dSaturation;

        while( reader.NodeType != XmlNodeType.EndElement )
        {
          if( reader.NodeType == XmlNodeType.Element )
          {
            result = ParseColorUpdater( reader, result, parser, out alpha );
          }
          else
          {
            reader.Skip();
          }
        }
      }

      reader.Read();

      return result;
    }
    /// <summary>
    /// Extracts preset color and converts it into rgb.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="alpha">Output alpha value.</param>
    /// <param name="parser">Excel2007Parser to help in extraction process.</param>
    /// <returns>Extracted color.</returns>
    public static Color ParsePresetColor(XmlReader reader, out int alpha, Excel2007Parser parser)
    {
        if (reader == null)
            throw new ArgumentNullException();

        if (reader.LocalName != Drawings.PresetcolorTag)
            throw new XmlException("Unexpected xml tag");

        bool bEmpty = reader.IsEmptyElement;

        alpha = ShapeFillImpl.MaxValue;
        string strColorName = null;

        if (reader.MoveToAttribute(ChartConstants.ValueAttribute))
            strColorName = reader.Value;

#if (!SILVERLIGHT && !WINRT && !WP)
        Color result = Color.FromName(strColorName);
#else
        Color result = ColorExtension.FromName(strColorName);
#endif
        reader.MoveToElement();

        if (!bEmpty)
        {
            reader.Read();

            while (reader.NodeType != XmlNodeType.EndElement)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    result = ParseColorUpdater(reader, result, parser, out alpha);
                }
                else
                {
                    reader.Skip();
                }
            }
        }

        reader.Read();

        return result;
    }
    /// <summary>
    /// Extracts system color and converts it into rgb.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="alpha">Output alpha value.</param>
    /// <param name="parser">Excel2007Parser to help in extraction process.</param>
    /// <returns>Extracted color.</returns>
    public static Color ParseSystemColor( XmlReader reader, out int alpha, Excel2007Parser parser )
    {
      if( reader == null )
        throw new ArgumentNullException();

      if( reader.LocalName != Drawings.SystemColorTag )
        throw new XmlException( "Unexpected xml tag" );

      alpha = ShapeFillImpl.MaxValue;
      string strColorName = null;
      Color result;

      if( reader.MoveToAttribute( Drawings.SystemColorHexAttribute ) )
      {
        string strColor = reader.Value;
        int iColor = int.Parse( strColor, System.Globalization.NumberStyles.HexNumber, null );
        result = ColorExtension.FromArgb( iColor );
      }
      else
       {
        result = ColorExtension.Empty;
      }

      reader.MoveToElement();

      if( !reader.IsEmptyElement )
      {
        reader.Read();

        while( reader.NodeType != XmlNodeType.EndElement )
        {
          if( reader.NodeType == XmlNodeType.Element )
          {
            result = ParseColorUpdater( reader, result, parser, out alpha );
          }
          else
          {
            reader.Skip();
          }
        }
      }

      reader.Read();

      return result;
    }
    private static Color ParseColorUpdater( XmlReader reader, Color result, Excel2007Parser parser, out int alpha )
    {
      double dHue;
      double dLuminance;
      double dSaturation;
      alpha = ShapeFillImpl.MaxValue;

      switch( reader.LocalName )
      {
        case Drawings.LuminanceModulation:
          int iLuminanceMod = ParseIntValueTag( reader );
          Excel2007Parser.ConvertRGBtoHLS( result, out dHue, out dLuminance, out dSaturation );
          dLuminance *= iLuminanceMod / ( double )ShapeFillImpl.MaxValue;
          result = Excel2007Parser.ConvertHLSToRGB( dHue, dLuminance, dSaturation );
          break;

        case Drawings.LuminanceOffset:
          int iLuminanceOff = ParseIntValueTag( reader );
          Excel2007Parser.ConvertRGBtoHLS( result, out dHue, out dLuminance, out dSaturation );
          //int iCurrentLuminance = ( int )( dLuminance * ShapeFillImpl.MaxValue );
          dLuminance += Excel2007Parser.HLSMax * iLuminanceOff / ( double )ShapeFillImpl.MaxValue;
          result = Excel2007Parser.ConvertHLSToRGB( dHue, dLuminance, dSaturation );
          break;

        case Drawings.SaturationModulation:
          int iSaturationMod = ParseIntValueTag( reader );
          Excel2007Parser.ConvertRGBtoHLS( result, out dHue, out dLuminance, out dSaturation );
          dSaturation *= iSaturationMod / ( double )ShapeFillImpl.MaxValue;
          result = Excel2007Parser.ConvertHLSToRGB( dHue, dLuminance, dSaturation );
          break;

        case Drawings.TintTag:
          double dTint = ParseDoubleValueTag( reader );
          result = Excel2007Parser.ConvertColorByTint( result, dTint );
          break;

        case Drawings.ShadeTag:
          int iShade = ParseIntValueTag( reader );
          double dShade = iShade / ( double )ShapeFillImpl.MaxValue;
          result = parser.ConvertColorByShade( result, dShade );
          break;

        case Drawings.AlphaTag:
          alpha = ParseIntValueTag( reader );
          break;

        default:
          Debug.Assert( false, "Unknown color tag " + reader.LocalName );
          reader.Skip();
          break;
      }

      return result;
    }
//    /// <summary>
//    /// Serializes rgb color.
//    /// </summary>
//    /// <param name="writer">XmlWriter to serialize into.</param>
//    /// <param name="color">Color to serialize.</param>
//    /// <param name="alpha">Alpha component of the color to serialize, 0-100000.</param>
//    /// <param name="tint">Tint value of the color to serialize, 0-100000.</param>
//    /// <param name="shade">Shape value of the color to serialize, 0-100000.</param>
//    public static void SerializeRgbColor( XmlWriter writer, Color color,
//      int alpha, int tint, int shade )
//    {
//      if( writer == null )
//        throw new ArgumentNullException( "writer" );

//      writer.WriteStartElement( Drawings.SRGBColorTag, Drawings.ANamespace );
//      writer.WriteAttributeString( ChartConstants.ValueAttribute, ( color.ToArgb() & 0xFFFFFF ).ToString( "X6" ) );

//      if( alpha >= 0 )
//      {
//        writer.WriteStartElement( Drawings.AlphaTag, Drawings.ANamespace );
//        writer.WriteAttributeString( ChartConstants.ValueAttribute, alpha.ToString() );
//        writer.WriteEndElement();
//      }

//      if( shade >= 0 )
//      {
//        writer.WriteElementString( Drawings.GammaTag, Drawings.ANamespace, string.Empty );
//        writer.WriteStartElement( Drawings.ShadeTag, Drawings.ANamespace );
//        writer.WriteAttributeString( ChartConstants.ValueAttribute, shade.ToString() );
//        writer.WriteEndElement();
//        writer.WriteElementString( Drawings.InverseGammaTag, Drawings.ANamespace, string.Empty );
//      }
//      else if( tint >= 0 )
//      {
//        writer.WriteElementString( Drawings.GammaTag, Drawings.ANamespace, string.Empty );
//        writer.WriteStartElement( Drawings.TintTag, Drawings.ANamespace );
//        writer.WriteAttributeString( ChartConstants.ValueAttribute, tint.ToString() );
//        writer.WriteEndElement();
//        writer.WriteElementString( Drawings.InverseGammaTag, Drawings.ANamespace, string.Empty );
//      }

//      writer.WriteEndElement();
//    }
    /// <summary>
    /// Serialize line properties.
    /// </summary>
    /// <param name="reader">XmlReader to serialize into.</param>
    /// <param name="border">Chart line properties to serialize.</param>
    /// <param name="bRoundCorners">Indicates whether border is rounded or not</param>
    /// <param name="parser">Excel2007Parser to help in extraction process.</param>
    private static void ParseLineProperties( XmlReader reader, ChartBorderImpl border,
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
      bool isSkipped = false;
      if( reader.MoveToAttribute( Drawings.LineWidthAttribute ) )
      {
        int iLineWieght = ( int )( int.Parse( reader.Value ) / 12700.0 ) - 1;
        border.LineWeightString = reader.Value;
        border.LineWeight = ( ExcelChartLineWeight )iLineWieght;
        //( iLineWieght > 2 ) ?
        //( ExcelChartLineWeight )iLineWieght :
        //ExcelChartLineWeight.Hairline;
      }
      else
      {
        border.LineWeight = ExcelChartLineWeight.Hairline;
      }

      int Alpha = ShapeFillImpl.MaxValue;
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
                border.LinePattern = ExcelChartLinePattern.None;
                border.HasLineProperties = true;
                reader.Read();
                break;

              case Drawings.RoundTag:
                border.JoinType = Excel2007BorderJoinType.Round;
                reader.Read();
                break;

              case Drawings.MiterJoinTag:
                border.JoinType = Excel2007BorderJoinType.Mitter;
                reader.Read();
                break;

              case Drawings.BevelJoinTag:
                border.JoinType = Excel2007BorderJoinType.Bevel;
                reader.Read();
                break;

              case Drawings.SolidFillTag:
                border.Color.AfterChange += border.ClearAutoColor;
                ParseSolidFill( reader, parser, border.Color, out Alpha );
                border.Transparency = 1 - Alpha / ( float )ShapeFillImpl.MaxValue;
                border.Color.AfterChange -= border.ClearAutoColor;
                border.AutoFormat = false;
                bSolid = true;
                break;

              case Drawings.PresetDashTag:  
                strPresetDash = ParseValueTag( reader );
                break;

              case Drawings.PatternFillTag:
                //Debug.Fail( "Pattern fill parsing is not implemented" );
                reader.Skip();
                break;

              case Drawings.GradientFillTag:
                border.AutoFormat = false;
                GradientStops gradientStops = ParseGradientFill( reader, parser );
                ConvertGradientStopsToProperties( gradientStops, border.Fill );
                border.Fill.PreservedGradient = gradientStops;
                border.HasLineProperties = true;
                break;

              case Drawings.HeadEnd:
                reader.Skip();
                break;

              case Drawings.TailEnd:
                reader.Skip();
                break;

              default:
                throw new NotImplementedException();
              //reader.Skip();
              //break;
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
        border.LinePattern = ExcelChartLinePattern.Solid;
          if(!bEmpty)
        border.HasLineProperties = true;
        if( strPresetDash != null )
        {
          KeyValuePair<string, string> pair = new KeyValuePair<string, string>( strPresetDash, string.Empty );
          ExcelChartLinePattern linePattern;

          if( s_dicLinePatterns.TryGetValue( pair, out linePattern ) )
          {
            border.LinePattern = linePattern;
          }
          
        }
      }
      //ExcelChartLinePattern linePattern = border.LinePattern;

      //if( linePattern == ExcelChartLinePattern.None )
      //{
      //  writer.WriteElementString( Drawings.NoFillTag, Drawings.ANamespace, string.Empty );
      //}
      //else if( linePattern == ExcelChartLinePattern.Solid )
      //{
      //  SerializeSolidFill( writer, border.LineColor, border.IsAutoLineColor );
      //}
      //else
      //{
      //  KeyValuePair<string, string> pair = s_dicLinePatterns[ linePattern ];
      //  string strDash2007 = pair.Key;
      //  string strPreset = pair.Value;
      //  SerializePatternFill( writer, border.LineColor, border.IsAutoLineColor, strDash2007, strPreset );
      //  //throw new NotImplementedException();
      //}

      //if( bRoundCorners )
      //  writer.WriteElementString( Drawings.RoundTag, Drawings.ANamespace );

      reader.Read();
    }
    /// <summary>
    /// Extracts picture fill.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="fill">Fill to put extracted data into.</param>
    /// <param name="relations">Chart item relations.</param>
    /// <param name="holder">Parent file data holder.</param>
    public static void ParsePictureFill( XmlReader reader, IFill fill,
      RelationCollection relations, FileDataHolder holder )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( fill == null )
        throw new ArgumentNullException( "fill" );

      if( reader.LocalName != Drawings.BlipFillTagName )
        throw new XmlException( "Unexpected xml tag." );

      //if( holder == null )
      //  throw new ArgumentNullException( "holder" );

      //if( relations == null )
      //  throw new ArgumentNullException( "relations" );

      //string strLocation = holder.SaveImage( image, null );
      //string strRelationId = relations.GenerateRelationId();
      //relations[ strRelationId ] = new Relation( '/' + strLocation, RelationTypes.Image );

      reader.Read();

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case Drawings.BlipTagName:
              if( reader.MoveToAttribute( Drawings.EmbeddedPicture, Excel2007Serializator.RelationNamespace ) )
              {
                string strRelationId = reader.Value;
                Relation relation = relations[ strRelationId ];
                string strPath = relations.ItemPath;
                int index = strPath.LastIndexOf( '/' );
                strPath = strPath.Substring( 0, index );
                index = strPath.LastIndexOf( '/' );
                strPath = strPath.Substring( 0, index );
                strPath = FileDataHolder.CombinePath( strPath, relation.Target );
#if (SILVERLIGHT || WP)
                System.Drawing.Image image = holder.GetImage( strPath );
#else
                   Image image = holder.GetImage( strPath );
#endif
                fill.UserPicture( image, "image" );
              }
              reader.Read();
              while (reader.LocalName != Drawings.SourceRectangleTagName && reader.LocalName != Drawings.TileTagName && reader.LocalName != Drawings.StretchTagName)
              {
                  if (reader.LocalName == Drawings.AlphaModFixTag)
                  {
                      int Transparency = (reader.MoveToAttribute(Drawings.AlphaModFixattribute)) ?
                      int.Parse(reader.Value) :
                      0;
                      (fill as IInternalFill).TransparencyColor = (float)1-(float)Transparency / 100000;
                      reader.Read();

                      }
                      
                  reader.Read();
              }
             
              break;

            case Drawings.TileTagName:
              ////<a:tile tx="0" ty="0" sx="100000" sy="100000" flip="none" algn="tl" /> 
              //writer.WriteStartElement( Drawings.TileTagName, Drawings.ANamespace );
              //writer.WriteAttributeString( "tx", "0" );
              //writer.WriteAttributeString( "ty", "0" );
              //writer.WriteAttributeString( "sx", "100000" );
              //writer.WriteAttributeString( "sy", "100000" );
              //writer.WriteAttributeString( "flip", "none" );
              //writer.WriteAttributeString( "algn", "tl" );                         
              if (reader.NodeType != XmlNodeType.EndElement)
                  {
                     
                      if (reader.MoveToAttribute(Drawings.HorizontalOffsetTag))
                          (fill as IInternalFill).TextureOffsetX = float.Parse(reader.Value)/12700;
                      if (reader.MoveToAttribute(Drawings.VerticalOffsetTag))
                          (fill as IInternalFill).TextureOffsetY = float.Parse(reader.Value)/12700;
                      if (reader.MoveToAttribute(Drawings.HorizontalRatioTag))
                          (fill as IInternalFill).TextureHorizontalScale = float.Parse(reader.Value)/ 100000;
                      if (reader.MoveToAttribute(Drawings.VerticalRatioTag))
                          (fill as IInternalFill).TextureVerticalScale = float.Parse(reader.Value) / 100000;                      
                      if (reader.MoveToAttribute(Drawings.TileFlippingTag))
                          (fill as IInternalFill).TileFlipping = reader.Value;
                      if (reader.MoveToAttribute(Drawings.AlignmentTag))
                      {
                          (fill as IInternalFill).Tile = true;
                          (fill as IInternalFill).Alignment = reader.Value;
                      }
                  }        
              reader.Read();             
              break;

            case Drawings.StretchTagName:
              reader.Read();
              if (reader.LocalName == Drawings.FillRectTagName) ;
              int iLeft = (reader.MoveToAttribute(Drawings.LeftAttribute)) ?
                int.Parse(reader.Value) :
                0;

              int iTop = (reader.MoveToAttribute(Drawings.TopAttribute)) ?
                int.Parse(reader.Value) :
                0;

              int iRight = reader.MoveToAttribute(Drawings.RightAttribute) ?
                int.Parse(reader.Value) :
                0;

              int iBottom = (reader.MoveToAttribute(Drawings.BottomAttribute)) ?
                int.Parse(reader.Value) :
                0;
              (fill as ShapeFillImpl).FillRect = Rectangle.FromLTRB(iLeft, iTop, iRight, iBottom);
              reader.Read();
              reader.Read();
              break;

              case Drawings.SourceRectangleTagName:
                  int rLeft = (reader.MoveToAttribute(Drawings.LeftAttribute)) ?
            int.Parse(reader.Value) :
            0;

              int rTop = (reader.MoveToAttribute(Drawings.TopAttribute)) ?
                int.Parse(reader.Value) :
                0;

              int rRight = reader.MoveToAttribute(Drawings.RightAttribute) ?
                int.Parse(reader.Value) :
                0;

              int rBottom = (reader.MoveToAttribute(Drawings.BottomAttribute)) ?
                int.Parse(reader.Value) :
                0;
              (fill as ShapeFillImpl).SourceRect = Rectangle.FromLTRB(rLeft, rTop, rRight, rBottom);
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
//    /// <summary>
//    /// Serializes texture fill.
//    /// </summary>
//    /// <param name="writer">XmlWriter to serialize into.</param>
//    /// <param name="fill">Fill object that contains settings to serialize.</param>
//    /// <param name="holder">Parent file data holder object.</param>
//    /// <param name="relations">Relations collection to add relation to.</param>
//    private static void SerializeTextureFill( XmlWriter writer, IFill fill,
//      FileDataHolder holder, RelationsCollection relations )
//    {
//      if( writer == null )
//        throw new ArgumentNullException( "writer" );

//      if( fill == null )
//        throw new ArgumentNullException( "fill" );

//      if( holder == null )
//        throw new ArgumentNullException( "holder" );

//      if( relations == null )
//        throw new ArgumentNullException( "relations" );

//      Image picture;
//      ExcelTexture texture = fill.Texture;

//      if( texture != ExcelTexture.User_Defined )
//      {
//        byte[] arrData = ShapeFillImpl.GetResData( ShapeFillImpl.DEF_TEXTURE_PREFIX + ( ( int )texture ).ToString() );
//        byte[] arrPicture = new byte[ arrData.Length - 25 ];

//        Array.Copy( arrData, 25, arrPicture, 0, arrPicture.Length );
//        MemoryStream ms = new MemoryStream();

//        ShapeFillImpl.UpdateBitMapHederToStream( ms, arrData );
//        ms.Write( arrPicture, 0, arrPicture.Length );

//        picture = Image.FromStream( ms, true, false );
//      }
//      else
//      {
//        picture = fill.Picture;
//      }

//      SerializePictureFill( writer, picture, holder, relations, true );
//    }
//    /// <summary>
//    /// Serializes gradient fill.
//    /// </summary>
//    /// <param name="writer">XmlWriter to serialize into.</param>
//    /// <param name="fill">Fill to serialize.</param>
//    private static void SeializeGradientFill( XmlWriter writer, IFill fill )
//    {
//      // TODO: add required property to the interface.
//      ShapeFillImpl shapeFill = ( ShapeFillImpl )fill;
//      GradientStops gradientStops = shapeFill.GradientStops;
//      GradientSerializator serializator = new GradientSerializator();
//      serializator.Serialize( writer, gradientStops );
//    }
//    /// <summary>
//    /// Parses text properties.
//    /// </summary>
//    /// <param name="reader">XmlReader to extract data from.</param>
//    /// <param name="textArea">Text area to put extracted data into.</param>
//    private void ParseTextProperties( XmlReader reader, IChartTextArea textArea )
//    {
//      throw new Exception( "The method or operation is not implemented." );
//    }
    /// <summary>
    /// Extracts text from XmlReader and places it into text area.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="textArea">Text area to put extracted data into.</param>
    /// <param name="parser">Parser object that helps to extract data.</param>
    private static void ParseTextAreaText( XmlReader reader, IInternalChartTextArea textArea,
      Excel2007Parser parser, float? defaultFontSize )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( textArea == null )
        throw new ArgumentNullException( "textArea" );

      if( reader.LocalName != ChartConstants.ChartTextTag )
        throw new XmlException( "Unexpected xml tag." );

      reader.Read();

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case ChartConstants.RichTextTag:
              List<ChartAlrunsRecord.TRuns> tRuns = null;
              if ((textArea as ChartTextAreaImpl) != null && (textArea as ChartTextAreaImpl).ChartAlRuns != null)
                  tRuns = new List<ChartAlrunsRecord.TRuns>((textArea as ChartTextAreaImpl).ChartAlRuns.Runs);
              else if ((textArea as ChartDataLabelsImpl) != null && (textArea as ChartDataLabelsImpl).TextArea.ChartAlRuns != null)
                  tRuns = new List<ChartAlrunsRecord.TRuns>((textArea as ChartDataLabelsImpl).TextArea.ChartAlRuns.Runs);

              ParseRichText( reader, textArea, parser, defaultFontSize, tRuns );
              break;

            case ChartConstants.StringReferenceTag:
              ParseStringReference(reader, textArea);
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
    /// Extracts layout from XmlReader and places it into text area.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="textArea">Layout to put extracted data into.</param>
    /// <param name="parser">Parser object that helps to extract data.</param>
    public static void ParseChartLayout(XmlReader reader, IChartLayout layout)
    {
        if (reader == null)
            throw new ArgumentNullException("reader");

        if (layout == null)
            throw new ArgumentNullException("layout");

        if (reader.LocalName != ChartConstants.LayoutTag)
            throw new XmlException("Unexpected xml tag.");

        if (!reader.IsEmptyElement)
        {
            reader.Read();

            IChartManualLayout manualLayout = null;
            if (reader.IsStartElement() && reader.LocalName == ChartConstants.ManualLayoutTag)
                manualLayout = layout.ManualLayout;

            if (manualLayout != null)
            {
                while (reader.NodeType != XmlNodeType.EndElement)
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        switch (reader.LocalName)
                        {
                            case ChartConstants.ManualLayoutTag:
                                ParseManualLayout(reader, manualLayout);
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
        }
        else
            reader.Skip();
    }
    /// <summary>
    /// Parses manual layout.
    /// </summary>
    /// <param name="reader">XmlReader to read rich text from.</param>
    /// <param name="manualLayout">manualLayout that will get extracted the manual position settings.</param>
    /// <param name="parser">Parser object that helps to extract data.</param>
    private static void ParseManualLayout(XmlReader reader, IChartManualLayout manualLayout)
    {
        if (reader == null)
            throw new ArgumentNullException("reader");

        if (manualLayout == null)
            throw new ArgumentNullException("manualLayout");

        if (reader.LocalName != ChartConstants.ManualLayoutTag)
            throw new XmlException("Unexpected xml tag.");

        if (!reader.IsEmptyElement)
        {
            reader.Read();

            while (reader.NodeType != XmlNodeType.EndElement)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case ChartConstants.LayoutTargetTag:
                            manualLayout.LayoutTarget = (LayoutTargets)Enum.Parse(typeof(LayoutTargets), ParseValueTag(reader), true);
                            break;

                        case ChartConstants.LeftModeTag:
                            manualLayout.LeftMode = (LayoutModes)Enum.Parse(typeof(LayoutModes), ParseValueTag(reader), true);
                            break;

                        case ChartConstants.TopModeTag:
                            manualLayout.TopMode = (LayoutModes)Enum.Parse(typeof(LayoutModes), ParseValueTag(reader), true);
                            break;

                        case ChartConstants.LeftTag:
                            manualLayout.Left = ParseDoubleValueTag(reader);
                            break;

                        case ChartConstants.TopTag:
                            manualLayout.Top = ParseDoubleValueTag(reader);
                            break;

                        case ChartConstants.dXTag:
                            reader.Skip();
                            break;

                        case ChartConstants.dYTag:
                            reader.Skip();
                            break;

                        case ChartConstants.WidthModeTag:
                            manualLayout.WidthMode = (LayoutModes)Enum.Parse(typeof(LayoutModes), ParseValueTag(reader), true);
                            break;

                        case ChartConstants.HeightModeTag:
                            manualLayout.HeightMode = (LayoutModes)Enum.Parse(typeof(LayoutModes), ParseValueTag(reader), true);
                            break;

                        case ChartConstants.WidthTag:
                            manualLayout.Width = ParseDoubleValueTag(reader);
                            break;

                        case ChartConstants.HeightTag:
                            manualLayout.Height = ParseDoubleValueTag(reader);
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
        else
            reader.Skip();
    }
    private static void ParseStringReference(XmlReader reader, IInternalChartTextArea textArea)
    {
        if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( reader.LocalName != ChartConstants.StringReferenceTag )
        throw new XmlException( "Unexpected xml tag." );

      reader.Read();
      string strResult = null;

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case ChartConstants.Formula:
              if (textArea is ChartTextAreaImpl)
                  (textArea as ChartTextAreaImpl).IsFormula = true;
              else if (textArea is ChartDataLabelsImpl)
                  (textArea as ChartDataLabelsImpl).IsFormula = true;

              textArea.Text = reader.ReadElementContentAsString();
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
        // TODO: we don't store cache and we don't support anything but formula here, this should be changed later.
      }

      reader.Read();     
    
    }
    /// <summary>
    /// Parses rich text.
    /// </summary>
    /// <param name="reader">XmlReader to read rich text from.</param>
    /// <param name="textArea">Text area that will get extracted rich text.</param>
    /// <param name="parser">Parser object that helps to extract data.</param>
    private static void ParseRichText( XmlReader reader, IInternalChartTextArea textArea,
      Excel2007Parser parser, float? defaultFontSize, List<ChartAlrunsRecord.TRuns> tRuns )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( textArea == null )
        throw new ArgumentNullException( "textArea" );

      if( reader.LocalName != ChartConstants.RichTextTag )
        throw new XmlException( "Unexpected xml tag." );

      reader.Read();
      bool bFirstParagraph = true;

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case Drawings.TextBodyPropertiesTag:
              ParseBodyProperties( reader, textArea );
              break;

            case Drawings.ListStylesTag:
              ParseListStyles( reader, textArea );
              break;

            case Drawings.Paragraphs:

              if( !bFirstParagraph )
              {
                textArea.Text += '\n';
              }
              else
              {
                bFirstParagraph = false;
              }

              tRuns = ParseParagraphs( reader, textArea, parser, defaultFontSize, tRuns );
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
    /// Parses text area body properties.
    /// </summary>
    /// <param name="reader">XmlReader to read body properties from.</param>
    /// <param name="textArea">Text area to put body properties into.</param>
    private static void ParseBodyProperties( XmlReader reader, IChartTextArea textArea )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( textArea == null )
        throw new ArgumentNullException( "textArea" );

      if( reader.LocalName != Drawings.TextBodyPropertiesTag )
        throw new XmlException( "Unexpected xml tag." );

      // TODO: on the current moment we don't support body properties.

      if( reader.MoveToAttribute( Drawings.TextRotationAttribute ) )
      {
        int iAngle = XmlConvert.ToInt32( reader.Value );
        textArea.TextRotationAngle = iAngle / ChartAxisSerializator.TextRotationMultiplier;
        if (reader.MoveToAttribute(Drawings.TextBoxRotationAttribute))
        {
            (textArea as ChartTextAreaImpl).TextRotation = (Excel2007TextRotation)Enum.Parse(
        typeof(Excel2007TextRotation), reader.Value, false);
        }
        reader.MoveToElement();
      }

      reader.Skip();
    }
    /// <summary>
    /// Extracts list styles for a text area.
    /// </summary>
    /// <param name="reader">XmlReader to extract list styles from.</param>
    /// <param name="textArea">Text area that will get extracted settings.</param>
    private static void ParseListStyles( XmlReader reader, IChartTextArea textArea )
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
    /// <param name="textArea">Text area that will get paragraph information (formatting and text).</param>
    /// <param name="parser">Parser object that helps to extract data.</param>
    private static List<ChartAlrunsRecord.TRuns> ParseParagraphs(XmlReader reader, IInternalChartTextArea textArea,
      Excel2007Parser parser, float? defaultFontSize, List<ChartAlrunsRecord.TRuns> tRuns )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( textArea == null )
        throw new ArgumentNullException( "textArea" );

      if( reader.LocalName != Drawings.Paragraphs )
        throw new XmlException( "Unexpected xml tag." );

      //writer.WriteStartElement( Drawings.Paragraphs, Drawings.ANamespace );
      reader.Read();
      TextSettings defaultSettings = null;

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case Drawings.ParagraphProperties:
              defaultSettings = ParseParagraphProperties( reader, parser, defaultFontSize );
              break;

            case Drawings.ParagraphRun:
              tRuns = ParseParagraphRun( reader, textArea, parser, defaultSettings, tRuns );
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

      return tRuns;
    }
    /// <summary>
    /// Parses paragraph properties.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    private static TextSettings ParseParagraphProperties( XmlReader reader,
      Excel2007Parser parser, float? defaultFontSize )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      TextSettings result = null;

      if( !reader.IsEmptyElement )
      {
        reader.Read();

        while( reader.NodeType != XmlNodeType.EndElement )
        {
          if( reader.NodeType == XmlNodeType.Element )
          {
            switch( reader.LocalName )
            {
              case Drawings.DefaultParagraphProperites:
                result = ParseDefaultParagraphProperties( reader, parser, defaultFontSize );
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
      return result;
    }
    /// <summary>
    /// Extracts default paragraph properties from the reader.
    /// </summary>
    /// <param name="reader">Reader to get properties from.</param>
    /// <param name="parser">Instance of Excel2007Parser that helps in parsing process.</param>
    /// <returns>Default paragraph properties.</returns>
    internal static TextSettings ParseDefaultParagraphProperties( XmlReader reader,
      Excel2007Parser parser )
    {
      return ParseDefaultParagraphProperties( reader, parser, null );
    }
    /// <summary>
    /// Extracts default paragraph properties from the reader.
    /// </summary>
    /// <param name="reader">Reader to get properties from.</param>
    /// <param name="parser">Instance of Excel2007Parser that helps in parsing process.</param>
    /// <param name="defaultFontSize">Default font size.</param>
    /// <returns>Default paragraph properties.</returns>
    internal static TextSettings ParseDefaultParagraphProperties( XmlReader reader,
      Excel2007Parser parser, float? defaultFontSize )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      TextSettings result = new TextSettings();
      result.FontSize = defaultFontSize;

      if( reader.MoveToAttribute( Drawings.FontBoldAttribute ) )
        result.Bold = XmlConvert.ToBoolean( reader.Value );

      if( reader.MoveToAttribute( Drawings.FontItalicAttribute ) )
        result.Italic = XmlConvert.ToBoolean( reader.Value );

      if (reader.MoveToAttribute(Drawings.FontSizeAttribute))
      {
          result.FontSize = XmlConvert.ToSingle(reader.Value) / 100.0f;
          result.ShowSizeProperties = true;
      }

      if( reader.MoveToAttribute( Drawings.FontStrikeAttribute ) )
        result.Striked = reader.Value != ChartConstants.StrikeThroughNone;

      if( reader.MoveToAttribute( Drawings.FontLanguage ) )
        result.Language = reader.Value;

      if (reader.MoveToAttribute(Drawings.Baseline))
      {
          int value = XmlConvert.ToInt32(reader.Value);
          result.Baseline = value;
      }

      // TODO: add underline support.
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
                                result.HasLatin = true;
                if( reader.MoveToAttribute( Drawings.TypefaceTag ) )
                {
                                    result.ActualFontName = reader.Value;
                                    string fontName = CheckValue(reader.Value.ToString());
                                    result.FontName = fontName;
                }

                reader.MoveToElement();
                reader.Skip();
                break;

              case Drawings.SolidFillTag:
                ParseDefaultFontColor( reader, result, parser );
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

      return result;
    }

        public static string CheckValue(string strValue)
        {

            FontImpl font = null;
            if (strValue == "+mj-lt"|| strValue=="+mj-cs" || strValue == "+mj-ea")
            {
                string[] split = strValue.Split('-');
                if (split[0] == "+mj")
                {
                    switch (split[1])
                    {
                        case "lt":
                            m_book.MajorFonts.TryGetValue("lt", out font);
                            break;
                        case "ea":
                            m_book.MajorFonts.TryGetValue("ea", out font);
                            break;
                        case "cs":
                            m_book.MajorFonts.TryGetValue("cs", out font);
                            break;
                    }
                }
                return font.FontName;
            }
            return strValue;
        }
    /// <summary>
    /// Extracts color of the default paragraph.
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="result"></param>
    private static void ParseDefaultFontColor( XmlReader reader, TextSettings result, Excel2007Parser parser )
    {
      if( !reader.IsEmptyElement )
      {
        reader.Read();

        while( reader.NodeType != XmlNodeType.EndElement )
        {
          if( reader.NodeType == XmlNodeType.Element )
          {
            switch( reader.LocalName )
            {
              case Drawings.SRGBColorTag:
                result.FontColor = ParseSRgbColor( reader, parser );
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
    /// Parses paragraph run.
    /// </summary>
    /// <param name="reader">XmlReader to get paragraph run from.</param>
    /// <param name="textArea">Text area to put extracted properties into.</param>
    /// <param name="parser">Parser object that helps to extract data.</param>
    /// <param name="defaultSettings">Default text settings.</param>
    private static List<ChartAlrunsRecord.TRuns> ParseParagraphRun(XmlReader reader, IInternalChartTextArea textArea, 
        Excel2007Parser parser, TextSettings defaultSettings, List<ChartAlrunsRecord.TRuns> tRuns )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( textArea == null )
        throw new ArgumentNullException( "textArea" );

      if( reader.LocalName != Drawings.ParagraphRun )
        throw new XmlException( "Unexpected xml tag." );

      reader.Read();
      CopyDefaultSettings( textArea, defaultSettings );

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case Drawings.TextRunProperites:
                  ParseParagraphRunProperites(reader, textArea, parser, defaultSettings);
              break;

            case Drawings.ParagraphText:
              ushort firstCharIndex = 0;

              if (textArea.Text != null)
                  firstCharIndex = (ushort) textArea.Text.Length;

              textArea.Text += reader.ReadElementContentAsString();//ReadElementString();

              if (tRuns != null)
              {
                  tRuns.Add(new ChartAlrunsRecord.TRuns(firstCharIndex, (ushort)textArea.Font.Index));

                  if ((textArea as ChartTextAreaImpl) != null && (textArea as ChartTextAreaImpl).ChartAlRuns != null)
                  {
                      (textArea as ChartTextAreaImpl).ChartAlRuns.Runs = tRuns.ToArray();
                  }
                  else if ((textArea as ChartDataLabelsImpl) != null && (textArea as ChartDataLabelsImpl).TextArea.ChartAlRuns != null)
                  {
                      (textArea as ChartDataLabelsImpl).TextArea.ChartAlRuns.Runs = tRuns.ToArray();
                  }
              }

              if (textArea.Text.Contains("\r"))
                  textArea.Text = textArea.Text.Remove(textArea.Text.IndexOf('\r'), 1);
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

      return tRuns;
    }
    /// <summary>
    /// Parses paragraph run.
    /// </summary>
    /// <param name="reader">XmlReader to extract paragraph tag from.</param>
    /// <param name="textArea">Text area that will get paragraph run information (formatting and text).</param>
    /// <param name="parser">Parser object that helps to extract data.</param>
    /// <param name="defaultSettings">Default text settings.</param>
    public static void ParseParagraphRunProperites( XmlReader reader,
      IInternalChartTextArea textArea, Excel2007Parser parser, TextSettings defaultSettings)
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( textArea == null )
        throw new ArgumentNullException( "textArea" );

      //if( reader.LocalName != Drawings.TextRunProperites )
      //  throw new XmlException( "Unexpected xml tag." );
      CopyDefaultSettings( textArea, defaultSettings );

      if (reader.MoveToAttribute(Drawings.FontLanguage))
      {
          textArea.Font.Language = reader.Value;
          FontImpl defaultFontImpl = null;
          if (m_book.MinorFonts.TryGetValue(Drawings.LatinTag, out defaultFontImpl))
          {
              textArea.FontName = defaultFontImpl.FontName;
          }
      }
      if (defaultSettings != null && defaultSettings.FontName != textArea.FontName && defaultSettings.FontName!=null)
          textArea.FontName = defaultSettings.FontName;

      if (reader.MoveToAttribute(Drawings.FontBoldAttribute))
      {
          textArea.Bold = XmlConvert.ToBoolean(reader.Value);
          if ((textArea as ChartDataLabelsImpl) != null)
              (textArea as ChartDataLabelsImpl).ShowBoldProperties = true;
          else if ((textArea as ChartTextAreaImpl) != null)
              (textArea as ChartTextAreaImpl).ShowBoldProperties = true;
      }

      if( reader.MoveToAttribute( Drawings.FontItalicAttribute ) )
        textArea.Italic = XmlConvert.ToBoolean( reader.Value );

      if( reader.MoveToAttribute( Drawings.FontStrikeAttribute ) )
      {
        textArea.Strikethrough = reader.Value != ChartConstants.StrikeThroughNone;
        //writer.WriteAttributeString( Drawings.FontStrikeAttribute, ChartConstants.StrikeThroughSingle );
      }

      if (reader.MoveToAttribute(Drawings.FontSizeAttribute))
      {
          textArea.Size = int.Parse(reader.Value) / 100.0;
          if ((textArea as ChartDataLabelsImpl) != null)
              (textArea as ChartDataLabelsImpl).ShowSizeProperties = true;
          else if ((textArea as ChartTextAreaImpl) != null)
              (textArea as ChartTextAreaImpl).ShowSizeProperties = true;
      }

      if( reader.MoveToAttribute( Drawings.FontUnterlineAttribute ) )
      {
        if( reader.Value == ChartConstants.UnderlineSingle )
        {
          textArea.Underline = ExcelUnderline.Single;
        }
        else if( reader.Value == ChartConstants.UnderlineDouble )
        {
          textArea.Underline = ExcelUnderline.Double;
        }
      }

      if ( reader.MoveToAttribute( Drawings.Baseline ) )
      {
          int baseLine = int.Parse(reader.Value);
          
          textArea.Font.BaseLine = baseLine;

          if (baseLine > 0)
              textArea.Superscript = true;
          else if (baseLine < 0)
              textArea.Subscript = true;
          else
          {
              textArea.Superscript = false;
              textArea.Subscript = false;
          }
      }
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
                                textArea.Font.HasLatin = true;
                if( reader.MoveToAttribute( Drawings.TypefaceTag ) )
                {
                    textArea.Font.ActualFontName = reader.Value;
                    textArea.FontName = CheckValue(reader.Value);
                    reader.MoveToElement();
                }
                reader.Skip();
                break;

              case Drawings.SolidFillTag:
                ParseSolidFill( reader, parser, textArea.ColorObject );
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

    public static void CopyDefaultSettings( IInternalFont textArea, TextSettings defaultSettings )
    {
      if( defaultSettings != null )
      {
        if (defaultSettings.Bold != null)
              textArea.Bold = (bool)defaultSettings.Bold;

        if( defaultSettings.Italic != null )
          textArea.Italic = ( bool )defaultSettings.Italic;

        if (defaultSettings.FontSize != null)
            textArea.Size = (float)defaultSettings.FontSize;

        if( defaultSettings.FontName != null )
          textArea.FontName = defaultSettings.FontName;

        if( defaultSettings.Striked != null )
          textArea.Strikethrough = ( bool )defaultSettings.Striked;

        if ( defaultSettings.Baseline != null && (textArea as FontWrapper) != null )
            (textArea as FontWrapper).Baseline = defaultSettings.Baseline;

        if (defaultSettings.Baseline > 0)
            textArea.Superscript = true;
        else if (defaultSettings.Baseline < 0)
            textArea.Subscript = true;
        else
        {
            textArea.Subscript = false;
            textArea.Superscript = false;
        }

        if (defaultSettings.Language != null)
        {
            textArea.Font.Language = defaultSettings.Language;
            FontImpl defaultFontImpl=null;
            if (m_book.MinorFonts.TryGetValue(Drawings.LatinTag, out defaultFontImpl))
            {
                textArea.FontName = defaultFontImpl.FontName;
            }
        }

        textArea.Font.m_textSettings = defaultSettings;

        if( defaultSettings.FontColor != null )
          textArea.RGBColor = ( Color )defaultSettings.FontColor;

                if (defaultSettings.HasLatin != null)
                    textArea.Font.HasLatin = (bool)defaultSettings.HasLatin;

                if (defaultSettings.HasComplexScripts != null)
                    textArea.Font.HasComplexScripts = (bool)defaultSettings.HasComplexScripts;

                if (defaultSettings.HasEastAsianFont != null)
                    textArea.Font.HasEastAsianFont = (bool)defaultSettings.HasEastAsianFont;

                if (defaultSettings.ActualFontName != null)
                    textArea.Font.ActualFontName = defaultSettings.ActualFontName.ToString();
                
          if (defaultSettings.ShowSizeProperties != null)
          {
              if ((textArea as ChartTextAreaImpl) != null && defaultSettings.ShowSizeProperties != null)
              {
                  ChartTextAreaImpl titles = (textArea as ChartTextAreaImpl);
                  titles.ShowSizeProperties = (bool)defaultSettings.ShowSizeProperties;
              }
              else if ((textArea as ChartDataLabelsImpl) != null && defaultSettings.ShowSizeProperties != null)
              {
                  ChartDataLabelsImpl datalabels = (textArea as ChartDataLabelsImpl);
                  datalabels.ShowSizeProperties = (bool)defaultSettings.ShowSizeProperties;
              }
          }
      }
    }
//    /// <summary>
//    /// Extract string reference settings.
//    /// </summary>
//    /// <param name="reader">XmlReader to extract data from.</param>
//    /// <param name="textArea">Text area to put settings into.</param>
//    private static void ParseStringReference( XmlReader reader, IChartTextArea textArea )
//    {
//      throw new Exception( "The method or operation is not implemented." );
//    }
//    /// <summary>
//    /// Extracts text area layout settings.
//    /// </summary>
//    /// <param name="reader">XmlReader to get layout from.</param>
//    /// <param name="textArea">Text area to put layout settings into.</param>
//    private static void ParseLayout( XmlReader reader, IChartTextArea textArea )
//    {
//      throw new Exception( "The method or operation is not implemented." );
//    }
//    /// <summary>
//    /// Parses overlay setting of the text area.
//    /// </summary>
//    /// <param name="reader">XmlReader to extract overlay from.</param>
//    /// <param name="textArea">Text area to place overlay settings into.</param>
//    private static void ParseOverlay( XmlReader reader, IChartTextArea textArea )
//    {
//      throw new Exception( "The method or operation is not implemented." );
//    }
    /// <summary>
    /// Extracts gradient stops collection from the specified reader.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="parser">Excel2007Parser to help in extraction process.</param>
    /// <returns>Extracted gradient stops.</returns>
    public static GradientStops ParseGradientFill( XmlReader reader, Excel2007Parser parser )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( reader.LocalName != Drawings.GradientFillTag )
        throw new XmlException( "Unexpected xml tag." );

      GradientStops result = null;
      reader.Read();

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case Drawings.GradientStopsTag:
              result = ParseGradientStops( reader, parser );
              break;

            case Drawings.GradientLiniarTag:
              result.GradientType = GradientType.Liniar;

              reader.MoveToAttribute( Drawings.GradientAngleAttribute );
              result.Angle = int.Parse( reader.Value );
              reader.Read();
              break;

            case Drawings.GradientPathTag:
              ParseGradientPath( reader, result );
              break;
              case Drawings.GradientTailTag:
                 int iLeft = ( reader.MoveToAttribute( Drawings.LeftAttribute ) ) ?
            int.Parse( reader.Value ) :
            0;

          int iTop = ( reader.MoveToAttribute( Drawings.TopAttribute ) ) ?
            int.Parse( reader.Value ) :
            0;

          int iRight = reader.MoveToAttribute( Drawings.RightAttribute ) ?
            int.Parse( reader.Value ) :
            0;

          int iBottom = ( reader.MoveToAttribute( Drawings.BottomAttribute ) ) ?
            int.Parse( reader.Value ) :
            0;
          result.TileRect = Rectangle.FromLTRB(iLeft, iTop, iRight, iBottom);
              reader.Read();
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
      return result;
    }
    /// <summary>
    /// Extracts gradient path.
    /// </summary>
    /// <param name="reader">XmlReader to extract from.</param>
    /// <param name="result">Resulting gradient stop collection</param>
    private static void ParseGradientPath( XmlReader reader, GradientStops result )
    {
      bool bEmptyElement = reader.IsEmptyElement;
      reader.MoveToAttribute( Drawings.GradientPathAttribute );
      result.GradientType = ( GradientType )Enum.Parse( typeof( GradientType ), reader.Value, true );

      if( !bEmptyElement )
      {
        reader.Read();

        if( reader.LocalName == Drawings.FillToRectTag )
        {
          int iLeft = ( reader.MoveToAttribute( Drawings.LeftAttribute ) ) ?
            int.Parse( reader.Value ) :
            0;

          int iTop = ( reader.MoveToAttribute( Drawings.TopAttribute ) ) ?
            int.Parse( reader.Value ) :
            0;

          int iRight = reader.MoveToAttribute( Drawings.RightAttribute ) ?
            int.Parse( reader.Value ) :
            0;

          int iBottom = ( reader.MoveToAttribute( Drawings.BottomAttribute ) ) ?
            int.Parse( reader.Value ) :
            0;

          result.FillToRect = Rectangle.FromLTRB( iLeft, iTop, iRight, iBottom );
          reader.Read();
          reader.Read();
        }
      }
    }
    /// <summary>
    /// Extracts GradientStops collection.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="parser">Parser object that helps to extract data.</param>
    /// <returns>Extracted collection.</returns>
    private static GradientStops ParseGradientStops( XmlReader reader, Excel2007Parser parser )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( reader.LocalName != Drawings.GradientStopsTag )
        throw new XmlException( "Unexpected xml tag." );

      reader.Read();
      GradientStops result = new GradientStops();

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case Drawings.GradientStopTag:
              GradientStopImpl gradientStop = ParseGradientStop( reader, parser );
              result.Add( gradientStop );
              break;

            default:
              reader.Read();
              break;
          }
        }
        else
        {
          reader.Read();
        }
      }

      reader.Read();

      return result;
    }
    /// <summary>
    /// Extracts single gradient stop settings.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="parser">Excel parser to help in extraction process.</param>
    /// <returns>Extracted gradient stop.</returns>
    private static GradientStopImpl ParseGradientStop( XmlReader reader, Excel2007Parser parser )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( reader.LocalName != Drawings.GradientStopTag )
        throw new XmlException( "Unexpected xml tag." );

      int iPos = -1;

      if( reader.MoveToAttribute( Drawings.GradientPositionAttribute ) )
        iPos = XmlConvert.ToInt32( reader.Value );

      reader.Read();      
      int iTransparecy=0;
      int iTint;
      int iShade;
      Color color;
      GradientStopImpl result;
      if (reader.LocalName == Drawings.SchemeColorTag && reader.NodeType != XmlNodeType.EndElement)
      {
          string strColorName = null;
          if (reader.MoveToAttribute(ChartConstants.ValueAttribute))
              strColorName = reader.Value;
          color = parser.GetThemeColor(strColorName);
          reader.MoveToElement();
          result = new GradientStopImpl(color, iPos, iTransparecy);
          ColorObject colorObj = result.ColorObject;
          colorObj.IsSchemeColor = true;
          colorObj.SchemaName = strColorName;
          ParseSchemeColor(reader, parser, result);
          reader.Read();
          reader.Read();
      }
      else
      {
          color = ReadColor(reader, out iTransparecy, out iTint, out iShade, parser);
          reader.Read();
          result = new GradientStopImpl(color, iPos, iTransparecy, iTint, iShade);
      }      
      return result;
    }
    /// <summary>
    /// Parse the Scheme color.
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="parser"></param>
    /// <param name="stop"></param>
    private static void ParseSchemeColor(XmlReader reader, Excel2007Parser parser, GradientStopImpl stop)
    {
        if (reader.LocalName != Drawings.SchemeColorTag)
            throw new ArgumentException("Invaild Tag");
        if (!reader.IsEmptyElement)
        {
            reader.Read();
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                ColorObject colorObj = stop.ColorObject;
                switch (reader.LocalName)
                {
                    case Drawings.LuminanceModulation:
                        colorObj.Luminance = ParseIntValueTag(reader);
                        break;

                    case Drawings.LuminanceOffset:
                        colorObj.LuminanceOffSet = ParseIntValueTag(reader);
                        break;

                    case Drawings.SaturationModulation:
                        colorObj.Saturation = ParseIntValueTag(reader);
                        break;

                    case Drawings.TintTag:
                        colorObj.Tint = ParseIntValueTag(reader);
                        break;

                    case Drawings.ShadeTag:
                        stop.Shade = ParseIntValueTag(reader);
                        break;

                    case Drawings.AlphaTag:
                        stop.Transparency = ParseIntValueTag(reader);
                        break;

                    default:
                        Debug.Assert(false, "Unknown color tag " + reader.LocalName);
                        reader.Skip();
                        break;
                }

            }
        }
    }
    /// <summary>
    /// Extracts color settings.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="transparecy">Transparency part of the extracted color (0-100000) or -1 if no transparecy part was present.</param>
    /// <param name="tint">Tint part of the extracted color (0-100000) or -1 if no tint part was present.</param>
    /// <param name="shade">Shade part of the extracted color (0-100000) or -1 if no shade part was present.</param>
    /// <param name="parser">Excel parser to help in extraction process.</param>
    /// <returns>Color extracted.</returns>
    private static Color ReadColor( XmlReader reader, out int transparecy, out int tint,
      out int shade, Excel2007Parser parser )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      //int iColorValue = -1;
      Color color = ColorExtension.Empty;
      transparecy = -1;
      tint = -1;
      shade = -1;

      switch( reader.LocalName )
      {
        case Drawings.SRGBColorTag:
          color = ParseSRgbColor( reader, out transparecy, out tint, out shade, parser );
          break;

        case Drawings.SchemeColorTag:
          color = ParseSchemeColor( reader, out transparecy, parser );
          break;

        case Drawings.SystemColorTag:
          color = ParseSystemColor( reader, out transparecy, parser );
          break;

        default:
          //throw new XmlException( "Unexpected xml tag." );
          //Debug.Fail( "Unexpected xml tag." );
          reader.Skip();
          break;
      }

      return color;
    }
    /// <summary>
    /// Converts Excel 2007 gradient stops collection into set of properties used by Excel 97-2003.
    /// </summary>
    /// <param name="gradientStops">Gradient stops collection to convert.</param>
    /// <param name="fill">Fill object to put extracted properties into.</param>
    private static void ConvertGradientStopsToProperties( GradientStops gradientStops, IInternalFill fill )
    {
      if( gradientStops == null )
        throw new ArgumentNullException( "gradientStops" );

      if( fill == null )
        throw new ArgumentNullException( "fill" );

      ExcelGradientPreset preset = ExcelGradientPreset.Grad_Brass;
      bool isInverted = false;

      // We have following possible choices
      // 1. One color gradient
      // 2. Two color gradient
      ExcelGradientColor gradientColor = DetectGradientColor( gradientStops );
      fill.IsGradientSupported = true;

      // 3. Preset colors
      if( ( int )gradientColor < 0 )
      {
        preset = FindPreset( gradientStops, out isInverted );

        if( ( int )preset >= 0 )
        {
          gradientColor = ExcelGradientColor.Preset;
          fill.PresetGradient( preset );
        }
      }

      // 4. Others - cannot be converted into Excel 97 format.
      if( ( int )gradientColor < 0 )
      {
        // Type wasn't detected in this case we are taking first and last colors and using two color gradient.
        gradientColor = ExcelGradientColor.TwoColor;
        fill.IsGradientSupported = false;
      }
      
      if( gradientColor != ExcelGradientColor.Preset )
      {
        CopyGradientColor( fill.ForeColorObject, gradientStops[ 0 ] );
        CopyGradientColor( fill.BackColorObject, gradientStops[ gradientStops.Count - 1 ] );
        fill.FillType = ExcelFillType.Gradient;
        fill.GradientColorType = gradientColor;
        //fill.GradientStyle = ExcelGradientStyle.
      }

      ExcelGradientStyle gradientStyle;
      fill.FillType = ExcelFillType.Gradient;
      fill.GradientStyle = gradientStyle = DetectGradientStyle( gradientStops );
      fill.GradientVariant = DetectGradientVariant( gradientStops, gradientStyle, gradientColor, isInverted );

      SetGradientDegree( gradientStops, gradientColor, fill );
    }
    /// <summary>
    /// To check the default settings of text area
    /// </summary>
    /// <param name="textArea">Text area to check the default values</param>
    internal static void CheckDefaultSettings(ChartTextAreaImpl textArea)
    {
        bool bIsDefaultlanguage = (textArea.Font.Language != null);
        bool bIsDefaultFont = (textArea.FontName != ChartAxisParser.DefaultFont);
        bool bIsDefualtFontSize = (textArea.Size != ChartAxisParser.DefaultFontSize);
        bool bIsDefaultBold = textArea.Bold != false;
        bool bIsDefaultItlaic = textArea.Italic != false;
        bool bIsDefaultUnderline = textArea.Underline != ExcelUnderline.None;
        bool bIsDefaultSuperScript = textArea.Superscript != false;
        bool bIsDefualtSubScript = textArea.Subscript != false;
        bool bIsDefualtStrikethrough = textArea.Strikethrough != false;
        bool bHasLatin = textArea.Font.HasLatin != false;
        bool bIsDefaultColor = textArea.IsAutoColor != true;

        if (bIsDefaultlanguage || bIsDefaultFont || bIsDefualtFontSize || bIsDefaultBold || bIsDefaultItlaic || bIsDefaultUnderline
            || bIsDefaultSuperScript || bIsDefualtSubScript || bIsDefualtStrikethrough || bHasLatin || bIsDefaultColor)
        {
            (textArea as IInternalChartTextArea).ParagraphType = ChartParagraphType.CustomDefault;
        }
    }
    /// <summary>
    /// Copies color data from gradient stop.
    /// </summary>
    /// <param name="colorObject">Color object to copy data into.</param>
    /// <param name="gradientStop">Gradient stop to copy color data from.</param>
    private static void CopyGradientColor( ColorObject colorObject, GradientStopImpl gradientStop )
    {
      ColorObject source = gradientStop.ColorObject;

      int iTint = gradientStop.Tint;

      if( iTint >= 0 )
      {
        double dTint = iTint / ( double )ShapeFillImpl.MaxValue;
        source = Excel2007Parser.ConvertColorByTint( source.GetRGB( null ), dTint );
      }

      colorObject.CopyFrom( source, true );
    }
    /// <summary>
    /// Extracts shape properties.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="objectGetter">Object that provides access to filling objects.</param>
    /// <param name="dataHolder">FileDataHolder of the document that is parsed.</param>
    /// <param name="relations">Chart item relations.</param>
    public static void ParseShapeProperties( XmlReader reader, IChartFillObjectGetter objectGetter,
      FileDataHolder dataHolder,
      RelationCollection relations )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( reader.LocalName != Drawings.ShapePropertiesTag )
        throw new XmlException( "Unexpected xml tag." );

      if( objectGetter == null )
        throw new ArgumentNullException( "objectGetter" );

      IChartInterior interior = objectGetter.Interior;
      int Alpha = ShapeFillImpl.MaxValue;
      if( interior != null )
      {
        interior.Pattern = ExcelPattern.None;
        interior.UseAutomaticFormat = true;
      }

      Excel2007Parser parser = dataHolder.Parser;

      if( !reader.IsEmptyElement )
      {
        reader.Read();
        objectGetter.Border.AutoFormat = true;

        while( reader.NodeType != XmlNodeType.EndElement )
        {
          if( reader.NodeType == XmlNodeType.Element )
          {
            switch( reader.LocalName )
            {
              case Drawings.LineTag:
                ChartParserCommon.ParseLineProperties( reader, objectGetter.Border, parser );
                break;

              case Drawings.SolidFillTag:
                ChartParserCommon.ParseSolidFill( reader, objectGetter.Interior, parser, out Alpha );
                objectGetter.Fill.Transparency = 1 - Alpha / ( float )ShapeFillImpl.MaxValue;
                break;

              case Drawings.PatternFillTag:
                ChartParserCommon.ParsePatternFill( reader, objectGetter.Fill, parser );
                break;

              case Drawings.GradientFillTag:
                GradientStops gradientStops = ParseGradientFill( reader, parser );
                ConvertGradientStopsToProperties( gradientStops, objectGetter.Fill );
                objectGetter.Fill.PreservedGradient = gradientStops;
                objectGetter.Interior.UseAutomaticFormat = false;
                break;

              case Drawings.BlipFillTagName:
                ChartParserCommon.ParsePictureFill( reader, objectGetter.Fill, relations, dataHolder );
                break;

              case Drawings.NoFillTag:
                if( objectGetter.Fill != null )
                {
                  objectGetter.Fill.FillType = ExcelFillType.Pattern;
                  objectGetter.Fill.Pattern = ( ExcelGradientPattern )0;
                  objectGetter.Interior.Pattern = ExcelPattern.None;
                }
                reader.Skip();
                break;
///???

              case Drawings.EffectListTag:
                ChartParserCommon.ParseShadowproperties( reader, objectGetter.Shadow, relations, dataHolder, parser );
                break;


              case Drawings.Scene3DTag:
                ChartParserCommon.ParseLighting( reader, objectGetter.ThreeD, relations, dataHolder );
                break;

                            case Drawings.Special3DTag:
                                string material = NullString;
                                if (reader.MoveToAttribute("prstMaterial"))
                                {
                                    material = reader.Value;
                                    objectGetter.ThreeD.Material = ChartParserCommon.Check(material, reader);
                                }
                                else
                                {
                                    objectGetter.ThreeD.Material = Excel2007ChartMaterialProperties.NoEffect;
                                }
                                reader.MoveToElement();
                                if (!reader.IsEmptyElement)
                                {
                                    reader.Read();
                                    while (reader.NodeType != XmlNodeType.EndElement)
                                    {
                                        if (reader.NodeType == XmlNodeType.Element)
                                        {
                                          string Linewidth = NullString;
                                          string LineHeight = NullString;
                                          string PresetShape = NullString;

                                            switch (reader.LocalName)
                                            {
                                                case Drawings.BevelTopTag:
                                                    if (reader.MoveToAttribute(Drawings.LineWidthAttribute))
                                                    {
                                                        Linewidth = reader.Value;
                                                    }
                                                    if (reader.MoveToAttribute(Drawings.LineHeightAttribute))
                                                    {
                                                        LineHeight = reader.Value;
                                                    }
                                                    if (reader.MoveToAttribute(Drawings.PresetShapeAttribute))
                                                    {
                                                        PresetShape = reader.Value;
                                                    }
                                                    objectGetter.ThreeD.BevelTop = ChartParserCommon.Check(Linewidth, LineHeight, PresetShape, reader);
                                                    break;
                                                case Drawings.BevelBottomTag:
                                                    if (reader.MoveToAttribute(Drawings.LineWidthAttribute))
                                                    {
                                                        Linewidth = reader.Value;
                                                    }
                                                    if (reader.MoveToAttribute(Drawings.LineHeightAttribute))
                                                    {
                                                        LineHeight = reader.Value;
                                                    }
                                                    if (reader.MoveToAttribute(Drawings.PresetShapeAttribute))
                                                    {
                                                        PresetShape = reader.Value;
                                                    }
                                                    objectGetter.ThreeD.BevelBottom = ChartParserCommon.Check(Linewidth, LineHeight, PresetShape, reader);
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

                                break;


              default:
                //throw new NotImplementedException();
                //Debug.Fail( "Unexpected xml tag " + reader.LocalName );
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
        /// Parses the lighting.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="shadow">The shadow.</param>
        /// <param name="relations">The relations.</param>
        /// <param name="holder">The holder.</param>
        private static void ParseLighting(XmlReader reader, ThreeDFormatImpl Three_D, RelationCollection relations, FileDataHolder holder)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");
            if (reader.LocalName != Drawings.Scene3DTag)
                throw new XmlException("Unexpected xml tag.");
            if (!reader.IsEmptyElement)
            {
                reader.Read();
                while (reader.NodeType != XmlNodeType.EndElement)
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        string Lighttype;
                        switch (reader.LocalName)
                        {
                            case Drawings.LightingTag:
                                reader.MoveToAttribute(Drawings.LightingRightTag);
                                Lighttype = reader.Value;
                                Three_D.Lighting = ChartParserCommon.Check(Lighttype);
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
        /// Checks the specified lighttype.
        /// </summary>
        /// <param name="lighttype">The lighttype.</param>
        /// <returns></returns>
        public static Excel2007ChartLightingProperties Check(string lighttype)
        {
            Excel2007ChartLightingProperties temp = Excel2007ChartLightingProperties.ThreePoint;

            for (int i = 0; i < ChartSerializatorCommon.LightingProperties.GetLength(0); i++)
            {
                if (lighttype.Equals(ChartSerializatorCommon.LightingProperties[i][0]))
                {
                    temp = (Excel2007ChartLightingProperties)i;
                    break;
                }
            }
            return temp;
        }
        /// <summary>
        /// This method tries to get the Bevel properties read from the XML
        /// </summary>
        /// <param name="LineWidth">Linewidth mentions the Width of the line and represents the 'W' tag</param>
        /// <param name="LineHeight">LineHeight mentions the Heigth and represents the 'H' tag</param>
        /// <param name="PresetShape">Presetshape mentions the shape of the 3D feature</param>
        /// <param name="reader">XmlReader to extract data from.</param>
        /// <returns>it returns the Excel2007Chartbevel properties or Noangle will be returned(which means 0)</returns>
        public static Excel2007ChartBevelProperties Check(string LineWidth, string LineHeight, string PresetShape, XmlReader reader)
        {
            Excel2007ChartBevelProperties temp = Excel2007ChartBevelProperties.NoAngle;
            if( ( LineWidth.Equals( NullString ) ) && ( LineHeight.Equals( NullString ) ) && ( PresetShape.Equals( NullString ) ) )
            {
                temp = Excel2007ChartBevelProperties.Circle;
                reader.Skip();
            }
            else
            {
                for (int i = 0; i < ChartSerializatorCommon.BevelProperties.GetLength(0); i++)
                {
                    if ((LineWidth.Equals(ChartSerializatorCommon.BevelProperties[i][0])) && (LineHeight.Equals(ChartSerializatorCommon.BevelProperties[i][1])) && (PresetShape.Equals(ChartSerializatorCommon.BevelProperties[i][2])))
                    {
                        temp = (Excel2007ChartBevelProperties)i;
                        break;
                    }
                }
            }
            return temp;
        }

        /// <summary>
        /// This method tries to get the material properties
        /// </summary>
        /// <param name="material">Material is denotes the type of the Material properties read from the XML</param>
        /// <param name="reader">XmlReader to extract data from.</param>
        /// <returns>Excel 2007 chartmaterial properties or Noeffect value will be returned(which means 0)</returns>
        public static Excel2007ChartMaterialProperties Check(string material, XmlReader reader)
        {
            Excel2007ChartMaterialProperties temp = Excel2007ChartMaterialProperties.NoEffect;
            for (int i = 0; i < ChartSerializatorCommon.MaterialProperties.GetLength(0); i++)
            {
                if (material.Equals(ChartSerializatorCommon.MaterialProperties[i][0]))
                {
                    int x = i + 1;
                    temp = (Excel2007ChartMaterialProperties)x;
                    break;
                }
            }
            return temp;

        }
        /// <summary>
        /// Parses the Shadow Properties
        /// </summary>
        /// <param name="reader">XmlReader to extract data from.</param>
        /// <param name="shadow">Shadow object to access the properties</param>
        /// <param name="objectGetter">Object that provides access to filling objects.</param>
        /// <param name="Holder">FileDataHolder of the document that is parsed.</param>
        /// <param name="relations">Chart item relations.</param>
        /// <param name="parser">Excel 2007 Parser</param>
        private static void ParseShadowproperties(XmlReader reader, ShadowImpl shadow, RelationCollection relations, FileDataHolder holder, Excel2007Parser parser)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            if (reader.LocalName != Drawings.EffectListTag)
                throw new XmlException("Unexpected xml tag.");

            if (!reader.IsEmptyElement)
            {
                reader.Read();
                while (reader.NodeType != XmlNodeType.EndElement)
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                      string blurval = NullString;
                      string sizex = NullString;
                      string sizey = NullString;
                      string disttag = NullString;
                      string dirtag = NullString;
                      string align = NullString;
                      string rot = NullString;
                      string Kxtag = NullString;

                        switch (reader.LocalName)
                        {
                            case Drawings.OuterShadowTag:
                                if (reader.MoveToAttribute(Drawings.BlurRadiusTag))
                                {
                                    blurval = reader.Value;
                                }
                                if (reader.MoveToAttribute(Drawings.SizeX))
                                {
                                    sizex = reader.Value;
                                }
                                if (reader.MoveToAttribute(Drawings.SizeY))
                                {
                                    sizey = reader.Value;
                                }
                                if (reader.MoveToAttribute(Drawings.KXTag))
                                {
                                    Kxtag = reader.Value;
                                }
                                if (reader.MoveToAttribute(Drawings.DistanceTag))
                                {
                                    disttag = reader.Value;
                                }
                                if (reader.MoveToAttribute(Drawings.DirectionTag))
                                {
                                    dirtag = reader.Value;
                                }
                                if (reader.MoveToAttribute(Drawings.AlignmentTag))
                                {
                                    align = reader.Value;
                                }
                                if (reader.MoveToAttribute(Drawings.RotationwithShapeTag))
                                {
                                    rot = reader.Value;
                                }

                                if (((Kxtag == NullString) && (blurval.Equals("50800"))) || ((Kxtag == NullString)
                                  && (blurval.Equals("63500")) && (!sizex.Equals(NullString))))
                                {
                                    shadow.ShadowOuterPresets = ChartParserCommon.Check(blurval, sizex, sizey, disttag, dirtag, align, rot, shadow, reader, parser);
                                }
                                else if ((!Kxtag.Equals(NullString)) || ((sizex.Equals("90000")) && (Kxtag.Equals(NullString))))
                                {
                                    shadow.ShadowPrespectivePresets = (Excel2007ChartPresetsPrespective)ChartParserCommon.Check(blurval, sizex, sizey, Kxtag, disttag, dirtag, align, rot);
                                }
                                else
                                {
                                    shadow.ShadowOuterPresets = ChartParserCommon.Check(blurval, sizex, sizey, disttag, dirtag, align, rot, shadow, reader, parser);
                                }
                                break;
                            case Drawings.InnerShadowTag:
                                if (reader.MoveToAttribute(Drawings.BlurRadiusTag))
                                {
                                    blurval = reader.Value;
                                }
                                if (reader.MoveToAttribute(Drawings.DistanceTag))
                                {
                                    disttag = reader.Value;
                                }
                                if (reader.MoveToAttribute(Drawings.DirectionTag))
                                {
                                    dirtag = reader.Value;
                                }
                                shadow.ShadowInnerPresets = (Excel2007ChartPresetsInner)ChartParserCommon.Check(blurval, disttag, dirtag, reader, shadow, parser);
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
            else
            {
                shadow.HasCustomShadowStyle = true;
            }
            reader.Read();


        }
        ///<summary>
        //This method tries to get the Outer Shadow values
        /// <summary>
        /// 
        /// </summary>
        /// <param name="blurval">It gets the Blur Radius Tag value </param>
        /// <param name="sizex">It gets the Sizex Tag value</param>
        /// <param name="sizey">It gets the Sizey Tag value</param>
        /// <param name="disttag">It gets the Distance Tag Tag value</param>
        /// <param name="dirtag">It gets the Direction Tag value</param>
        /// <param name="align">It gets the Alignment Tag value</param>
        /// <param name="rot">It gets the Rotationwithshape Tag value</param>
        /// <param name="parser">Excel 2007 Parser</param>
        /// <param name="Shadow">Shadow object to access the properties</param>
        /// <param name="reader">Xml reader to extract data from.</param>
        /// <returns>The Excel2007chartpresetsouter value or Noshadow value will be returned(Which means 0)</returns>

        public static Excel2007ChartPresetsOuter Check(string blurval, string sizex, string sizey, string disttag, string dirtag, string align, string rot, ShadowImpl Shadow, XmlReader reader, Excel2007Parser parser)
        {
            int incre = 0;
            Excel2007ChartPresetsOuter temp = Excel2007ChartPresetsOuter.NoShadow;
            if( blurval.Equals( NullString ) && ( sizex.Equals( NullString ) ) && ( sizey.Equals( NullString ) )
              && ( disttag.Equals( NullString ) ) && ( dirtag.Equals( NullString ) ) && ( align.Equals( NullString ) ) )
            {
                temp = Excel2007ChartPresetsOuter.NoShadow;
            }
            else
            {
                for (int i = 0; i < ChartSerializatorCommon.OuterAttributeArray.GetLength(0); i++)
                {
                    incre++;
                    if ((blurval.Equals(ChartSerializatorCommon.OuterAttributeArray[i][0])) && (sizex.Equals(ChartSerializatorCommon.OuterAttributeArray[i][1])) && (sizey.Equals(ChartSerializatorCommon.OuterAttributeArray[i][2])) && (disttag.Equals(ChartSerializatorCommon.OuterAttributeArray[i][3])) && (dirtag.Equals(ChartSerializatorCommon.OuterAttributeArray[i][4])) && (align.Equals(ChartSerializatorCommon.OuterAttributeArray[i][5])) && (rot.Equals(ChartSerializatorCommon.OuterAttributeArray[i][6])))
                    {
                        int x = i + 1;
                        temp = (Excel2007ChartPresetsOuter)x;
                        break;
                    }
                }
                if (incre == ChartSerializatorCommon.OuterAttributeArray.GetLength(0))
                {
                    temp = ChartParserCommon.Check(blurval, sizex, disttag, dirtag, align, rot, Shadow, reader, parser);
                }
            }
            return temp;
        }
        /// <summary>
        /// Checks the specified Custom Outer Shadow .
        /// </summary>
        /// <param name="blurval">The blurval.</param>
        /// <param name="sizex">The sizex.</param>
        /// <param name="disttag">The disttag.</param>
        /// <param name="dirtag">The dirtag.</param>
        /// <param name="align">The align.</param>
        /// <param name="rot">The rot.</param>
        /// <param name="Shadow">The shadow.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="parser">The parser.</param>
        /// <returns></returns>
        public static Excel2007ChartPresetsOuter Check(string blurval, string sizex, string disttag, string dirtag, string align, string rot, ShadowImpl Shadow, XmlReader reader, Excel2007Parser parser)
        {
            Excel2007ChartPresetsOuter temp = Excel2007ChartPresetsOuter.NoShadow;

            for (int i = 0; i < ChartSerializatorCommon.OuterAttributeArray.GetLength(0); i++)
            {
                if (align.Equals(ChartSerializatorCommon.OuterAttributeArray[i][5]))
                {
                    int x = i + 1;
                    temp = (Excel2007ChartPresetsOuter)x;
                    break;
                }
            }

            Shadow.HasCustomShadowStyle = true;
            Shadow.Blur = ( blurval != NullString ) ? Convert.ToInt32(blurval) / ChartConstants.BlurValue : DefaultBlurValue;
            Shadow.Size = ( sizex != NullString ) ? Convert.ToInt32(sizex) / ChartConstants.SizeValue : DefaultShadowSize;
            Shadow.Distance = ( disttag != NullString ) ? Convert.ToInt32( disttag ) / ChartConstants.DistanceValue : DefaultDistanceValue;
            Shadow.Angle = ( dirtag != NullString ) ? Convert.ToInt32(dirtag) / ChartConstants.Anglevalue : DefaultAngleValue;

            reader.Read();
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case Drawings.PresetcolorTag:
                            if (reader.MoveToAttribute("val"))
                            {
                                string strColorName = reader.Value;
                                Shadow.ShadowColor = parser.GetThemeColor(strColorName);
                            }
                            reader.MoveToElement();
                            ChartParserCommon.ParseShadowAlpha(reader, Shadow);
                            break;
                        case Drawings.SchemeColorTag:
                            if (reader.MoveToAttribute("val"))
                            {
                                string strColorName = reader.Value;
                                Shadow.ShadowColor = parser.GetThemeColor(strColorName);
                            }
                            reader.MoveToElement();
                            ChartParserCommon.ParseShadowAlpha(reader, Shadow);
                            break;
                        case Drawings.SRGBColorTag:

                            if (reader.MoveToAttribute("val"))
                            {
                                string strColor = reader.Value;
                                int iColor = int.Parse(strColor, System.Globalization.NumberStyles.HexNumber, null);
                                Shadow.ShadowColor = ColorExtension.FromArgb( iColor );
                            }
                            reader.MoveToElement();
                            if (!reader.IsEmptyElement)
                            {
                                reader.Read();
                                while (reader.NodeType != XmlNodeType.EndElement)
                                {
                                    if (reader.NodeType == XmlNodeType.Element)
                                    {
                                        switch (reader.LocalName)
                                        {
                                            case Drawings.AlphaTag:
                                                if (reader.MoveToAttribute("val"))
                                                {
                                                    Shadow.Transparency = 100 - (Convert.ToInt32(reader.Value) / 1000);
                                                }
                                                else
                                                {
                                                    Shadow.Transparency = ShapeFillImpl.MaxValue;
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
                            }
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
            return temp;
        }
        /// <summary>
        /// Parses the shadow alpha.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="Shadow">The shadow.</param>
        public static void ParseShadowAlpha(XmlReader reader, ShadowImpl Shadow)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");
            if ((reader.LocalName != Drawings.PresetcolorTag) && (reader.LocalName != Drawings.SchemeColorTag))
                throw new XmlException("Unexpected xml tag.");
            reader.Read();
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case Drawings.AlphaTag:
                            if (reader.MoveToAttribute("val"))
                            {
                                Shadow.Transparency = 100 - (XmlConvert.ToInt32(reader.Value) / 1000);
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

        }
        /// <summary>
        /// Extracts value from XmlReader..
        /// </summary>
        /// <param name="reader">XmlReader to extract data from.</param>
        /// <returns>Extracted value.</returns>
        public static string ParseNumberFormat(XmlReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            string strResult = null; 

            if (reader.MoveToAttribute(ChartConstants.FormatCodeAttribute))
            {
                strResult = reader.Value;
            }

            return strResult;
        }
        /// <summary>
        /// This Method tries to get the value of perspective shadow
        /// </summary>
        /// <param name="blurval">It gets the Blur Radius Tag value</param>
        /// <param name="sizex">It gets the Sizex Tag value</param>
        /// <param name="sizey">It gets the Sizey Tag value</param>
        /// <param name="kxtag">It gets the Kx Tag value</param>
        /// <param name="disttag">It gets the Distance Tag Tag value</param>
        /// <param name="dirtag">It gets the Direction Tag value</param>
        /// <param name="align">It gets the Alignment Tag value</param>
        /// <param name="rot">It gets the Rotationwithshape Tag value</param>
        /// <returns>the Excel2007Chartpresetsperspective value or NoShadow value will be returned(which means 0)</returns>
        public static Excel2007ChartPresetsPrespective Check(string blurval, string sizex, string sizey, string kxtag, string disttag, string dirtag, string align, string rot)
        {
            Excel2007ChartPresetsPrespective temp = Excel2007ChartPresetsPrespective.NoShadow;
            for (int i = 0; i < ChartSerializatorCommon.PerspectiveAttributeArray.GetLength(0); i++)
            {
                if ((blurval.Equals(ChartSerializatorCommon.PerspectiveAttributeArray[i][0])) && (sizex.Equals(ChartSerializatorCommon.PerspectiveAttributeArray[i][4])) && (sizey.Equals(ChartSerializatorCommon.PerspectiveAttributeArray[i][3])) && (disttag.Equals(ChartSerializatorCommon.PerspectiveAttributeArray[i][2])) && (dirtag.Equals(ChartSerializatorCommon.PerspectiveAttributeArray[i][1])) && (kxtag.Equals(ChartSerializatorCommon.PerspectiveAttributeArray[i][5])) && (align.Equals(ChartSerializatorCommon.PerspectiveAttributeArray[i][6])) && (rot.Equals(ChartSerializatorCommon.PerspectiveAttributeArray[i][7])))
                {
                    temp = (Excel2007ChartPresetsPrespective)i;
                    break;
                }
            }
            return temp;
        }
        /// <summary>
        /// This method tries to get the Inner shadow values based on the parsed Info
        /// </summary>
        /// <param name="blurval">It gets the Blur Radius Tag value</param>
        /// <param name="disttag">It gets the Distance Tag Tag value</param>
        /// <param name="dirtag">It gets the Direction Tag value</param>
        /// <returns>This returns the Excel2007chartPresetsInner value or No shadow (which means 0)</returns>
        public static Excel2007ChartPresetsInner Check(string blurval, string disttag, string dirtag, XmlReader reader, ShadowImpl Shadow, Excel2007Parser parser)
        {
            int incre = 0;
            Excel2007ChartPresetsInner temp = Excel2007ChartPresetsInner.NoShadow;
            if( blurval.Equals( NullString ) && disttag.Equals( NullString ) && dirtag.Equals( NullString ) )
            {
                temp = Excel2007ChartPresetsInner.NoShadow;
            }
            else
            {
                for (int i = 0; i < ChartSerializatorCommon.InnerAttributeArray.GetLength(0); i++)
                {
                    incre++;
                    if ((blurval.Equals(ChartSerializatorCommon.InnerAttributeArray[i][0])) && (disttag.Equals(ChartSerializatorCommon.InnerAttributeArray[i][1])) && (dirtag.Equals(ChartSerializatorCommon.InnerAttributeArray[i][2])))
                    {
                        int x = i;
                        x++;
                        temp = (Excel2007ChartPresetsInner)x;
                        break;
                    }
                }
                if (incre == ChartSerializatorCommon.InnerAttributeArray.GetLength(0))
                {
                    temp = ChartParserCommon.Check(blurval, disttag, dirtag, Shadow, reader, true, parser);
                }
            }
            return temp;
        }
        /// <summary>
        /// Checks the Custom Inner Shadow.
        /// </summary>
        /// <param name="blurval">The blurval.</param>
        /// <param name="disttag">The disttag.</param>
        /// <param name="dirtag">The dirtag.</param>
        /// <param name="Shadow">The shadow.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="m_HasShadowStyle">if set to <c>true</c> [Current format has Custom shadow style].</param>
        /// <returns></returns>
        public static Excel2007ChartPresetsInner Check(string blurval, string disttag, string dirtag, ShadowImpl Shadow, XmlReader reader, bool m_HasShadowStyle, Excel2007Parser parser)
        {
            Excel2007ChartPresetsInner temp = Excel2007ChartPresetsInner.InsideBottom;

            Shadow.HasCustomShadowStyle = m_HasShadowStyle;
            Shadow.Blur = Convert.ToInt32(blurval) / ChartConstants.BlurValue;
            Shadow.Distance = Convert.ToInt32(disttag) / ChartConstants.DistanceValue;
            Shadow.Angle = Convert.ToInt32(dirtag) / ChartConstants.Anglevalue;

            reader.Read();
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case Drawings.PresetcolorTag:
                            if (reader.MoveToAttribute("val"))
                            {
                                string strColorName = reader.Value;
                                Shadow.ShadowColor = parser.GetThemeColor(strColorName);
                            }
                            reader.MoveToElement();
                            ChartParserCommon.ParseShadowAlpha(reader, Shadow);
                            break;
                        case Drawings.SchemeColorTag:
                            if (reader.MoveToAttribute("val"))
                            {
                                string strColorName = reader.Value;
                                Shadow.ShadowColor = parser.GetThemeColor(strColorName);
                            }
                            reader.MoveToElement();
                            ChartParserCommon.ParseShadowAlpha(reader, Shadow);
                            break;
                        case Drawings.SRGBColorTag:

                            if (reader.MoveToAttribute("val"))
                            {
                                string strColor = reader.Value;
                                int iColor = int.Parse(strColor, System.Globalization.NumberStyles.HexNumber, null);
                                Shadow.ShadowColor = ColorExtension.FromArgb( iColor );

                            }
                            reader.MoveToElement();
                            if (!reader.IsEmptyElement)
                            {
                                reader.Read();
                                while (reader.NodeType != XmlNodeType.EndElement)
                                {
                                    if (reader.NodeType == XmlNodeType.Element)
                                    {
                                        switch (reader.LocalName)
                                        {
                                            case Drawings.AlphaTag:
                                                if (reader.MoveToAttribute("val"))
                                                {
                                                    Shadow.Transparency = 100 - (Convert.ToInt32(reader.Value) / 1000);
                                                }
                                                else
                                                {
                                                    Shadow.Transparency = ShapeFillImpl.MaxValue;
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
                            }

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
            return temp;
    }
    /// <summary>
    /// This method tries to detect gradient color settings (One or Two color gradient).
    /// </summary>
    /// <param name="gradientStops">Gradient stops to detect gradient color</param>
    /// <returns>Detected gradient color or -1 if it is neither one nor two color gradient.</returns>
    private static ExcelGradientColor DetectGradientColor( GradientStops gradientStops )
    {
      if( gradientStops == null )
        throw new ArgumentNullException( "gradientStops" );

      ExcelGradientColor gradientColor = ( ExcelGradientColor )( -1 );
      int iCount = gradientStops.Count;

      if( iCount == 2 )
      {
        GradientStopImpl gradientStop1 = gradientStops[ 0 ];
        GradientStopImpl gradientStop2 = gradientStops[ 1 ];

        gradientColor = ( gradientStop1.ColorObject == gradientStop2.ColorObject ) ?
          ExcelGradientColor.OneColor :
          ExcelGradientColor.TwoColor;
      }
      else if( iCount == 3 )
      {
        GradientStopImpl gradientStop1 = gradientStops[ 0 ];
        GradientStopImpl gradientStop2 = gradientStops[ 1 ];
        GradientStopImpl gradientStop3 = gradientStops[ 2 ];

        if( gradientStop1.ColorObject == gradientStop3.ColorObject )
        {
          gradientColor = ( gradientStop1.ColorObject == gradientStop2.ColorObject ) ?
            ExcelGradientColor.OneColor :
            ExcelGradientColor.TwoColor;
        }
      }

      return gradientColor;
    }
    /// <summary>
    /// Detects gradient variant type.
    /// </summary>
    /// <param name="gradientStops">Gradient stops to detect variant type for.</param>
    /// <param name="gradientStyle">Already detected gradient style.</param>
    /// <param name="gradientColor">Already detected gradient color.</param>
    /// <param name="isPresetInverted">Indicates whether gradient stops for preset
    /// gradient are in inverted order. This argument is used in the case of ExcelGradientColor.Preset.</param>
    /// <returns>Detected gradient variant type.</returns>
    private static ExcelGradientVariants DetectGradientVariant( GradientStops gradientStops,
      ExcelGradientStyle gradientStyle, ExcelGradientColor gradientColor, bool isPresetInverted )
    {
      if( gradientStops == null )
        throw new ArgumentNullException( "gradientStops" );

      ExcelGradientVariants result = ExcelGradientVariants.ShadingVariants_1;
      bool bInverted = IsInverted( gradientStops, gradientColor, isPresetInverted );
      bool bDoubled = gradientStops.IsDoubled;//IsDoubled

      switch( gradientStyle )
      {
        case ExcelGradientStyle.Diagonl_Up:
        case ExcelGradientStyle.Horizontal:
        case ExcelGradientStyle.Vertical:
          result = DetectStandardVariant( bInverted, bDoubled );
          break;

        case ExcelGradientStyle.Diagonl_Down:
          result = DetectDiagonalDownVariant( bInverted, bDoubled );
          break;

        case ExcelGradientStyle.From_Corner:
          result = DetectGradientVariantCorner( gradientStops.FillToRect );
          break;

        case ExcelGradientStyle.From_Center:
          result = bInverted ?
            ExcelGradientVariants.ShadingVariants_1 :
            ExcelGradientVariants.ShadingVariants_2;
          break;
      }

      return result;
    }
    /// <summary>
    /// Detects diagonal gradient variant.
    /// </summary>
    /// <param name="bInverted">Checks whether gradient stops in the collection are in inverted order or not.</param>
    /// <param name="bDoubled">Represents double order variant.</param>
    /// <returns>Detected gradient variant.</returns>
    private static ExcelGradientVariants DetectDiagonalDownVariant( bool bInverted, bool bDoubled )
    {
      ExcelGradientVariants result;

      if( bInverted && bDoubled )
      {
        result = ExcelGradientVariants.ShadingVariants_4;
      }
      else if( bDoubled )
      {
        result = ExcelGradientVariants.ShadingVariants_3;
      }
      else if( bInverted )
      {
        result = ExcelGradientVariants.ShadingVariants_1;
      }
      else
      {
        result = ExcelGradientVariants.ShadingVariants_2;
      }

      return result;
    }
    /// <summary>
    /// Detects gradient variant.
    /// </summary>
    /// <param name="bInverted">Checks whether gradient stops in the collection are in inverted order or not.</param>
    /// <param name="bDoubled">Represents double ordered variant.</param>
    /// <returns>Detected gradient variant.</returns>
    private static ExcelGradientVariants DetectStandardVariant( bool bInverted, bool bDoubled )
    {
      ExcelGradientVariants result;

      if( bInverted && bDoubled )
      {
        result = ExcelGradientVariants.ShadingVariants_4;
      }
      else if( bDoubled )
      {
        result = ExcelGradientVariants.ShadingVariants_3;
      }
      else if( bInverted )
      {
        result = ExcelGradientVariants.ShadingVariants_2;
      }
      else
      {
        result = ExcelGradientVariants.ShadingVariants_1;
      }

      return result;
    }
    /// <summary>
    /// Detects gradient variant for FromCorner gradient style.
    /// </summary>
    /// <param name="rectangle">FillToRect used by gradient stops collection.</param>
    /// <returns>Detected gradient variant.</returns>
    private static ExcelGradientVariants DetectGradientVariantCorner( Rectangle rectangle )
    {
      Rectangle[] arrRectangles = ShapeFillImpl.RectanglesCorner;
      ExcelGradientVariants result = ExcelGradientVariants.ShadingVariants_1;

      for( int i = 0, len = arrRectangles.Length; i < len; i++ )
      {
        if( arrRectangles[ i ] == rectangle )
        {
          result = ( ExcelGradientVariants )i;
          break;
        }
      }

      return result;
    }
    /// <summary>
    /// Checks whether gradient stops in the collection are in inverted order or not.
    /// </summary>
    /// <param name="gradientStops">Gradient stops to check.</param>
    /// <param name="gradientColor">Gradient color type.</param>
    /// <param name="isPresetInverted">Indicates whether gradient stops for preset
    /// gradient are in inverted order. This argument is used in the case of ExcelGradientColor.Preset.</param>
    /// <returns>True if gradient stops are inverted.</returns>
    private static bool IsInverted( GradientStops gradientStops, ExcelGradientColor gradientColor, bool isPresetInverted )
    {
      if( gradientStops == null )
        throw new ArgumentNullException( "gradientStops" );

      bool bResult = false;

      switch( gradientColor )
      {
        case ExcelGradientColor.OneColor:
          if( gradientStops[ 0 ].Shade > 0 || gradientStops[ 0 ].Tint > 0 )
            bResult = true;
          break;

        case ExcelGradientColor.TwoColor:
          bResult = false;
          break;

        case ExcelGradientColor.Preset:
          bResult = isPresetInverted;
          break;
      }

      return bResult;
    }
    /// <summary>
    /// Detect Gradient style.
    /// </summary>
    /// <param name="gradientStops">Gradient stops to check.</param>
    /// <returns>Detected Gradient style.</returns>
    private static ExcelGradientStyle DetectGradientStyle( GradientStops gradientStops )
    {
      if( gradientStops == null )
        throw new ArgumentNullException( "gradientStops" );

      ExcelGradientStyle result = ExcelGradientStyle.Horizontal;

      switch( gradientStops.GradientType )
      {
        case GradientType.Liniar:
          result = GetLiniarGradientStyle( gradientStops );
          break;

        case GradientType.Rect:
          result = GetRectGradientStyle( gradientStops );
          break;
      }

      return result;
    }
    /// <summary>
    /// Gets gradient style for rectangular gradient.
    /// </summary>
    /// <param name="gradientStops">Gradient stops collection to get gradient style for.</param>
    /// <returns>Converted gradient style.</returns>
    private static ExcelGradientStyle GetRectGradientStyle( GradientStops gradientStops )
    {
      if( gradientStops == null )
        throw new ArgumentNullException( "gradientStops" );

      return ( gradientStops.FillToRect == ShapeFillImpl.RectangleFromCenter ) ?
        ExcelGradientStyle.From_Center :
        ExcelGradientStyle.From_Corner;
    }
    /// <summary>
    /// Gets gradient style for linear gradient.
    /// </summary>
    /// <param name="gradientStops">Gradient stops collection to get gradient style for.</param>
    /// <returns>Converted gradient style.</returns>
    private static ExcelGradientStyle GetLiniarGradientStyle( GradientStops gradientStops )
    {
      if( gradientStops == null )
        throw new ArgumentNullException( "gradientStops" );

      ExcelGradientStyle result;
      int iAngle = gradientStops.Angle;

      if( iAngle == ShapeFillImpl.VerticalAngle )
      {
        result = ExcelGradientStyle.Vertical;
      }
      else if( iAngle <= ShapeFillImpl.HorizontalAngle )
      {
        result = ExcelGradientStyle.Horizontal;
      }
      else if( iAngle <= ShapeFillImpl.DiagonalDownAngle )
      {
        result = ExcelGradientStyle.Diagonl_Down;
      }
      else //if( iAngle > ShapeFillImpl.HorizontalAngle )
      {
        result = ExcelGradientStyle.Diagonl_Up;
      }

      return result;
    }
    /// <summary>
    /// Tries to find preset gradient corresponding to the specified collection.
    /// </summary>
    /// <param name="gradientStops">Gradient stops collection to analyze.</param>
    /// <param name="isInverted">Indicates whether gradient stops are in inverted order.</param>
    /// <returns>Preset value or ( ExcelGradientPreset )( -1 ) if there is no corresponding preset value found.</returns>
    private static ExcelGradientPreset FindPreset( GradientStops gradientStops, out bool isInverted )
    {
      if( gradientStops == null )
        throw new ArgumentNullException( "gradientStops" );

      ExcelGradientPreset[] arrPresets =
#if !SILVERLIGHT && !WINRT && !WP
        ( ExcelGradientPreset[] )Enum.GetValues( typeof( ExcelGradientPreset ) );
#else
        ExcelGradientPresetExtension.GetValues();
#endif

      if( gradientStops.IsDoubled )
        gradientStops = gradientStops.ShrinkGradientStops();

      isInverted = false;
      ExcelGradientPreset result = ( ExcelGradientPreset )( -1 );

      //MemoryStream stream = new MemoryStream();
      //gradientStops.Serialize( stream );
      //byte[] arrBuffer = stream.GetBuffer();
      //int iLength = ( int )stream.Length;

      GradientStops gradientStops2 = gradientStops.Clone();
      gradientStops2.InvertGradientStops();

      //stream = new MemoryStream();
      //gradientStops2.Serialize( stream );
      //byte[] arrBuffer2 = stream.GetBuffer();
      //int iLength2 = ( int )stream.Length;

      // What if collection is doubled or inverted ?


      for( int i = 0, len = arrPresets.Length; i < len; i++ )
      {
        ExcelGradientPreset preset = arrPresets[ i ];
        GradientStops presetStops = ShapeFillImpl.GetPresetGradientStops( preset );

        //byte[] arrData = ShapeFillImpl.GetPresetGradientStopsData( preset );

        if( presetStops.EqualColors( gradientStops ) )
        //if( BiffRecordRaw.CompareArrays( arrData, 0, arrBuffer, 0, iLength ) )
        {
          result = preset;
          break;
        }

        if( presetStops.EqualColors( gradientStops2 ) )
        //if( BiffRecordRaw.CompareArrays( arrData, 0, arrBuffer2, 0, iLength2 ) )
        {
          isInverted = true;
          result = preset;
          break;
        }
      }

      return result;
    }
    /// <summary>
    /// Sets GradientDegree property if necessary.
    /// </summary>
    /// <param name="gradientStops">Gradient stops to detect degree from.</param>
    /// <param name="gradientColor">Detected gradient color.</param>
    /// <param name="fill">Fill to set gradient degree for.</param>
    private static void SetGradientDegree( GradientStops gradientStops, ExcelGradientColor gradientColor, IFill fill )
    {
      if( gradientStops == null )
        throw new ArgumentNullException( "gradientStops" );

      if( fill == null )
        throw new ArgumentNullException( "fill" );

      if( gradientColor == ExcelGradientColor.OneColor )
      {
        int iTint = Math.Max( gradientStops[ 0 ].Tint, gradientStops[ 1 ].Tint );
        int iShade = Math.Max( gradientStops[ 0 ].Shade, gradientStops[ 1 ].Shade );
        double degree;
        // shade = degree * 100000/255

        if( iShade > 0 )
        {
          degree = iShade / ( double )ShapeFillImpl.MaxValue;
        }
        else if( iTint > 0 )
        {
          degree = 1 - iTint / ( double )ShapeFillImpl.MaxValue;
        }
        else
        {
          degree = 0.5;
        }

        fill.GradientDegree = degree;
      }
    }
    #endregion
  }
}
