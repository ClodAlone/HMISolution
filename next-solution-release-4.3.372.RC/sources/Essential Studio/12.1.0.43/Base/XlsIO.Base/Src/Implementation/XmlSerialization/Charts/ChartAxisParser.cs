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
using System.Diagnostics;

using Syncfusion.XlsIO.Implementation.Charts;
using Syncfusion.XlsIO.Implementation.XmlSerialization.Constants;
using Syncfusion.XlsIO.Implementation.XmlSerialization.Shapes;
using Syncfusion.XlsIO.Implementation.XmlReaders;
using Syncfusion.XlsIO.Implementation.Shapes;
using Syncfusion.XlsIO.Interfaces.Charts;
using Syncfusion.XlsIO.Interfaces;
using System.IO;
using Syncfusion.XlsIO.Implementation.XmlReaders.Shapes;

namespace Syncfusion.XlsIO.Implementation.XmlSerialization.Charts
{
  /// <summary>
  /// This class is responsible for chart axis parsing.
  /// </summary>
  public class ChartAxisParser
  {
    #region Constants
    /// <summary>
    /// Default font name.
    /// </summary>
    public const string DefaultFont = "Calibri";
    /// <summary>
    /// Default axis font size.
    /// </summary>
    public const float DefaultFontSize = 10;
    #endregion

    #region Members
    //Removed the static keyword to avoid threading issue. Also this contains only 4 elements not need to implement 
    // wait or lock the thread for little memory management.
    /// <summary>
    /// Represents dictionary for Ticklabel to Attribute value.
    /// </summary>
    private Dictionary<string, ExcelTickLabelPosition> s_dictTickLabelToAttributeValue = new Dictionary<string, ExcelTickLabelPosition>(4);
    /// <summary>
    /// Represents dictionary for TickMark to Attribute value.
    /// </summary>
    private Dictionary<string, ExcelTickMark> s_dictTickMarkToAttributeValue = new Dictionary<string, ExcelTickMark>( 4 );
    private WorkbookImpl m_book;
    #endregion

    #region Delegates
    private delegate void AxisTagsParser( XmlReader reader, ChartAxisImpl axis, RelationCollection relations );
    #endregion

    #region Methods
    /// <summary>
    /// Initializes static members of the ChartAxisParser class.
    /// </summary>
    public ChartAxisParser()
    {
        
        if (s_dictTickLabelToAttributeValue.Count == 0)
    {
      s_dictTickLabelToAttributeValue.Add( ChartConstants.TickLabelHigh,
        ExcelTickLabelPosition.TickLabelPosition_High );

      s_dictTickLabelToAttributeValue.Add( ChartConstants.TickLabelLow,
        ExcelTickLabelPosition.TickLabelPosition_Low );

      s_dictTickLabelToAttributeValue.Add( ChartConstants.TickLabelNextTo,
        ExcelTickLabelPosition.TickLabelPosition_NextToAxis );

      s_dictTickLabelToAttributeValue.Add( ChartConstants.TickLabelNone,
        ExcelTickLabelPosition.TickLabelPosition_None );

      s_dictTickMarkToAttributeValue.Add( ChartConstants.TickMarkNone, ExcelTickMark.TickMark_None );
      s_dictTickMarkToAttributeValue.Add( ChartConstants.TickMarkInside, ExcelTickMark.TickMark_Inside );
      s_dictTickMarkToAttributeValue.Add( ChartConstants.TickMarkOutside, ExcelTickMark.TickMark_Outside );
      s_dictTickMarkToAttributeValue.Add( ChartConstants.TickMarkCross, ExcelTickMark.TickMark_Cross );
    }
    }
    public ChartAxisParser(WorkbookImpl book) 
        :this()       
    {
        m_book = book;
        ChartParserCommon.SetWorkbook(book);
    }
    ///// <summary>
    ///// Serializes chart axis.
    ///// </summary>
    ///// <param name="writer">XmlWriter to serialize into.</param>
    ///// <param name="axis">Axis to serialize.</param>
    //public void SerializeAxis( XmlWriter writer, IChartAxis axis )
    //{
    //  if( writer == null )
    //    throw new ArgumentNullException( "writer" );

    //  if( axis == null )
    //    return;

    //  switch( axis.AxisType )
    //  {
    //    case ExcelAxisType.Category:

    //      ChartCategoryAxisImpl categoryAxis = ( ChartCategoryAxisImpl )axis;
    //      if( !categoryAxis.ParentChart.IsChartBubble )
    //      {
    //        if( categoryAxis.CategoryType == ExcelCategoryType.Time )
    //        {
    //          SerializeDateAxis( writer, categoryAxis );
    //        }
    //        else
    //        {
    //          SerializeCategoryAxis( writer, categoryAxis );
    //        }
    //      }
    //      else
    //      {
    //        SerializeValueAxis( writer, ( ChartValueAxisImpl )axis );
    //      }
    //      break;

    //    case ExcelAxisType.Value:
    //      SerializeValueAxis( writer, ( ChartValueAxisImpl )axis );
    //      break;

    //    case ExcelAxisType.Serie:
    //      SerializeSeriesAxis( writer, ( ChartSeriesAxisImpl )axis );
    //      break;

    //    default:
    //      throw new NotSupportedException();
    //  }
    //}
    /// <summary>
    /// Extracts date axis from XmlReader.
    /// </summary>
    /// <param name="reader">XmlReader to serialize into.</param>
    /// <param name="axis">Axis to serialize.</param>
    /// <param name="relations">Chart item relations.</param>
    /// <param name="chartType">Prognosed type of the chart (or chart part) that is being parsed.</param>
    public void ParseDateAxis( XmlReader reader, ChartCategoryAxisImpl axis,
        RelationCollection relations, ExcelChartType chartType, Excel2007Parser parser )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( axis == null )
        throw new ArgumentNullException( "axis" );

      if( reader.LocalName != ChartConstants.DateAxisTag )
        throw new XmlException( "Unexpected xml tag." );

      reader.Read();

      //writer.WriteStartElement( ChartConstants.DateAxisTag, ChartConstants.CNamespace );
      axis.CategoryType = ExcelCategoryType.Time;
      ParseAxisCommon( reader, axis, relations, chartType, parser, DateAxisTagParsing );

      reader.Read();
    }
    ///// <summary>
    ///// 
    ///// </summary>
    ///// <param name="baseUnit"></param>
    ///// <returns></returns>
    //private string ConvertDateUnitToString( ExcelChartBaseUnit baseUnit )
    //{
    //  return baseUnit.ToString().ToLower() + 's';
    //}
    /// <summary>
    /// Serializes category axis.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="axis">Axis to serialize.</param>
    /// <param name="relations">Chart item relations.</param>
    /// <param name="chartType">Prognosed type of the chart (or chart part) that is being parsed.</param>
    public void ParseCategoryAxis( XmlReader reader, ChartCategoryAxisImpl axis,
      RelationCollection relations, ExcelChartType chartType, Excel2007Parser parser )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( axis == null )
        throw new ArgumentNullException( "axis" );

      if( reader.LocalName != ChartConstants.CategoryAxisTag )
        throw new XmlException( "reader" );

      reader.Read();

      axis.IsAutoMajor = true;
      axis.IsAutoMinor = true;
      axis.CategoryType = ExcelCategoryType.Category;

      ParseAxisCommon( reader, axis, relations, chartType, parser, CategoryAxisTagParsing );

      reader.Read();
    }
    /// <summary>
    /// Extracts value axis.
    /// </summary>
    /// <param name="reader">XmlReader to extract axis from.</param>
    /// <param name="valueAxis">Axis to put extracted data into.</param>
    /// <param name="relations">Chart item relations.</param>
    /// <param name="chartType">Prognosed type of the chart (or chart part) that is being parsed.</param>
    public void ParseValueAxis( XmlReader reader, ChartValueAxisImpl valueAxis,
      RelationCollection relations, ExcelChartType chartType, Excel2007Parser parser )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( valueAxis == null )
        throw new ArgumentNullException( "valueAxis" );

      if( reader.LocalName != ChartConstants.ValueAxisTag )
        throw new XmlException( "Unexpected xml tag." );

      reader.Read();
      ParseAxisCommon( reader, valueAxis, relations, chartType, parser, ValueAxisTagParsing );

      reader.Read();
    }
    /// <summary>
    /// Extracts series axis data.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="seriesAxis">Axis to put extracted data into.</param>
    /// <param name="relations">Chart item relations.</param>
    /// <param name="chartType">Prognosed type of the chart (or chart part) that is being parsed.</param>
    public void ParseSeriesAxis( XmlReader reader, ChartSeriesAxisImpl seriesAxis,
      RelationCollection relations, ExcelChartType chartType, Excel2007Parser parser )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( seriesAxis == null )
        throw new ArgumentNullException( "seriesAxis" );

      if( reader.LocalName != ChartConstants.SeriesAxisTag )
        throw new XmlException( "Unexpected xml tag." );

      reader.Read();

      seriesAxis.AutoTickLabelSpacing = true;
      seriesAxis.AutoTickMarkSpacing = true;

      ParseAxisCommon( reader, seriesAxis, relations, chartType, parser, SeriesAxisTagParsing );

      reader.Read();
    }
    /// <summary>
    /// Extracts common part of the axis.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="axis">Axis to serialize.</param>
    /// <param name="chartItemRelations">Relations collection for chart item that is being parsed.</param>
    /// <param name="chartType">Prognosed type of the chart (or chart part) that is being parsed.</param>
    private void ParseAxisCommon( XmlReader reader, ChartAxisImpl axis,
      RelationCollection chartItemRelations, ExcelChartType chartType,
      Excel2007Parser parser, AxisTagsParser unknownTagParser )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( axis == null )
        throw new ArgumentNullException( "axis" );

      ChartImpl chart = axis.ParentChart;
      WorksheetDataHolder sheetHolder = chart.DataHolder;
      FileDataHolder holder = sheetHolder.ParentHolder;
      RelationCollection relations = chart.Relations;
      bool bContinue = true;
      ChartAxisScale scaling = null;
      axis.Visible = true;

      int iAxisId = -1;
      bool? bVisible = null;
      axis.Visible = true;
      axis.Font.FontName = DefaultFont;
      axis.Font.Size = DefaultFontSize;

      while( reader.NodeType != XmlNodeType.EndElement && bContinue )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case ChartConstants.AxisIdTag:
              iAxisId = ChartParserCommon.ParseIntValueTag( reader );
              break;

            case ChartConstants.ScalingTag:
              scaling = ParseScaling( reader );
              break;

            case ChartConstants.DeleteTag:
              //reader.Skip();
              bVisible = !ChartParserCommon.ParseBoolValueTag( reader );
              break;

            case ChartConstants.AxisPositionTag:
              string strPosition = ParseAxisPosition( reader, axis );
              axis.AxisPosition = ( ChartAxisPos )Enum.Parse( typeof( ChartAxisPos ), strPosition, false );

              //if( axis.ParentChart.IsChartBar && strPosition == ChartConstants.AxisPosBottom || strPosition == ChartConstants.AxisPosTop )
              //if( ( axis.ParentChart.IsChartScatter || axis.ParentChart.IsChartBubble ) && 
              if( ( chartType == ExcelChartType.Scatter_Markers || chartType == ExcelChartType.Bubble ) &&
                ( strPosition == ChartConstants.AxisPosBottom || strPosition == ChartConstants.AxisPosTop ) )
              {
                // switch to category axis (on the current moment primary).
                axis = ( ( axis.IsPrimary ) ?
                  axis.ParentChart.PrimaryCategoryAxis :
                  axis.ParentChart.SecondaryCategoryAxis ) as ChartValueAxisImpl;

                axis.Visible = true;
              }
              break;

            case ChartConstants.MajorGridlinesTag:
              axis.HasMajorGridLines = true;
              ParseGridlines( reader, axis.MajorGridLines, holder, chartItemRelations );
              break;

            case ChartConstants.MinorGridlinesTag:
              axis.HasMinorGridLines = true;
              ParseGridlines( reader, axis.MinorGridLines, holder, chartItemRelations );
              break;

            case ChartConstants.TitleTag:
              IInternalChartTextArea textArea = axis.TitleArea as IInternalChartTextArea;
              ChartParserCommon.ParseTextArea( reader, textArea, holder, relations );
              if (axis.IsAutoTextRotation && axis.AxisPosition == ChartAxisPos.l)
              {
                  textArea.TextRotationAngle = -90;
              }
              break;

            case ChartConstants.NumberFormatTag:
              ParseNumberFormat( reader, axis );
              break;

            case ChartConstants.MajorTickMarkTag:
              axis.MajorTickMark = ParseTickMark( reader );
              break;

            case ChartConstants.MinorTickMarkTag:
              axis.MinorTickMark = ParseTickMark( reader );
              break;

            case ChartConstants.TickLabelPositionTag:
              ParseTickLabel( reader, axis );
              break;

            case ChartConstants.CrossAxisTag:
              ParseCrossAxis( reader, axis );
              break;

            case ChartConstants.CrossesTag:
              ParseCrossesTag( reader, axis );
              break;

            case ChartConstants.CrossesAtTag:
              ChartValueAxisImpl valueAxis = ChartAxisSerializator.GetPairAxis( axis ) as ChartValueAxisImpl;

              if( valueAxis != null )
              {
                valueAxis.CrossesAt = ChartParserCommon.ParseDoubleValueTag( reader );
              }
              else
              {
                ( axis as ChartSeriesAxisImpl ).CrossesAt = ChartParserCommon.ParseIntValueTag( reader );
              }
              break;

            case Drawings.ShapePropertiesTag:
              ChartInteriorImpl interior = axis.FrameFormat.Interior as ChartInteriorImpl;
              interior.UseAutomaticFormat = false;
              ChartFillImpl fill = axis.FrameFormat.Fill as ChartFillImpl;
              IChartFillObjectGetter getter = new ChartFillObjectGetterAny( axis.FrameFormat.Border as ChartBorderImpl, interior, fill,
                axis.ShadowProperties as ShadowImpl,axis.FrameFormat.ThreeD as ThreeDFormatImpl );
              ChartParserCommon.ParseShapeProperties( reader, getter, holder, chartItemRelations );
              break;

            case ChartConstants.TextPropertiesTag:
              axis.ParagraphType = ChartParagraphType.CustomDefault;
              Stream textStream = ShapeParser.ReadNodeAsStream(reader);
              textStream.Position = 0;
              axis.TextStream = textStream;
              XmlReader axisTextReader= UtilityMethods.CreateReader(textStream);
              ParseTextSettings( axisTextReader, axis, parser );
              break;

            case ChartConstants.LabelAlignment:
              axis.LabelAlign = ChartParserCommon.ParseValueTag( reader );
              break;

            //case ChartConstants.CrossBetweenTag:
            //  bContinue = false;
            //  break;

            default:
              if( unknownTagParser != null )
              {
                unknownTagParser( reader, axis, relations );
              }
              else
              {
                reader.Skip();
              }
              break;
          }
          // (crosses | crossesAt)?
        }
        else
        {
          reader.Skip();
        }
      }

      axis.AxisId = iAxisId;

      if( bVisible != null )
        axis.Deleted = !( bool )bVisible;

      //if( bVisible != null )
      //  axis.Visible = ( bool )bVisible;

      if( scaling != null )
        scaling.CopyTo( axis as IScalable );
    }
    /// <summary>
    /// Extracts chart axis text properties from the reader.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="axis">Axis to put extracted data into.</param>
    private void ParseTextSettings( XmlReader reader, ChartAxisImpl axis, Excel2007Parser parser )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( axis == null )
        throw new ArgumentNullException( "axis" );

      if( reader.LocalName != ChartConstants.TextPropertiesTag )
        throw new XmlException();
      //axis.Font.Size = DefaultFontSize;
      if( !reader.IsEmptyElement )
      {
        reader.Read();

        while( reader.NodeType != XmlNodeType.EndElement )
        {
          if( reader.NodeType == XmlNodeType.Element )
          {
            switch( reader.LocalName )
            {
              case Drawings.TextBodyPropertiesTag:
                ParseBodyProperties( reader, axis );
                break;

              case Drawings.Paragraphs:
                ParseAxisParagraphs( reader, axis, parser );
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

    private void ParseAxisParagraphs( XmlReader reader, ChartAxisImpl axis, Excel2007Parser parser )
    {
      while( !( reader.LocalName == Drawings.Paragraphs && reader.NodeType == XmlNodeType.EndElement ) )
      {
        if( reader.NodeType == XmlNodeType.Element && reader.LocalName == Drawings.DefaultParagraphProperites )
        {
            if (reader.IsEmptyElement)
            {
                axis.IsDefaultTextSettings = true;
            }
            TextSettings textSettings = ChartParserCommon.ParseDefaultParagraphProperties( reader, parser, DefaultFontSize );
            ChartParserCommon.CopyDefaultSettings( axis.Font as IInternalFont, textSettings );
        }
        else
        {
          reader.Read();
        }
      }
      //writer.WriteStartElement( Drawings.ParagraphProperties, Drawings.ANamespace );
      //  ChartSerializatorCommon.SerializeParagraphRunProperites( writer, axis.Font,
      //    Drawings.DefaultParagraphProperites, axis.ParentChart.ParentWorkbook );
      //  //writer.WriteStartElement( Drawings.DefaultParagraphProperites, Drawings.ANamespace );
      //  //int iFontSize = ( int )( axis.Font.Size * 100 );
      //  //writer.WriteAttributeString( Drawings.FontSizeAttribute, iFontSize.ToString() );
      //  //writer.WriteEndElement();
      //  writer.WriteEndElement();

      reader.Read();
    }
    /// <summary>
    /// Extracts text body properties
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="axis">Axis to put extracted data into.</param>
    private void ParseBodyProperties( XmlReader reader, ChartAxisImpl axis )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( axis == null )
        throw new ArgumentNullException( "axis" );

      if( reader.LocalName != Drawings.TextBodyPropertiesTag )
        throw new XmlException();

      if( reader.MoveToAttribute( Drawings.TextRotationAttribute ) )
      {
        axis.TextRotationAngle = XmlConvert.ToInt32( reader.Value ) / ChartAxisSerializator.TextRotationMultiplier;
      }

      reader.MoveToElement();
      reader.Skip();
    }
    /// <summary>
    /// Parses crosses xml tag.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="axis">Axis to put extracted data into.</param>
    private void ParseCrossesTag( XmlReader reader, ChartAxisImpl axis )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( axis == null )
        throw new ArgumentNullException( "axis" );

      if( reader.LocalName != ChartConstants.CrossesTag )
        throw new XmlException( "Unexpected xml tag." );

      string strCross = ChartParserCommon.ParseValueTag( reader );

      if( strCross == ChartConstants.CrossesMaximum )
      {
        ChartValueAxisImpl valueAxis = ChartAxisSerializator.GetPairAxis( axis ) as ChartValueAxisImpl;
        valueAxis.IsMaxCross = true;
      }
    }
    /// <summary>
    /// Parses cross axis tag.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="axis">Axis to put extracted data into.</param>
    private void ParseCrossAxis( XmlReader reader, ChartAxisImpl axis )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( axis == null )
        throw new ArgumentNullException( "axis" );

      if( reader.LocalName != ChartConstants.CrossAxisTag )
        throw new XmlException( "Unexpected xml tag." );

      // NOTE: this functionality is not quite supported, so we can simply skip this tag.
      reader.Skip();
      //ChartImpl chart = axis.ParentChart;
      //bool bPrimary = axis.IsPrimary;
      //ChartAxisImpl crossAxis;
      //// category axis crosses value
      //// series crosses value?
      //// value crosses category
      //// we have no support for secondary axis yet.
      //switch( axis.AxisType )
      //{
      //  case ExcelAxisType.Category:
      //    crossAxis = ( ChartAxisImpl )( bPrimary ?
      //      chart.PrimaryValueAxis :
      //      chart.SecondaryValueAxis );
      //    break;

      //  case ExcelAxisType.Serie:
      //    crossAxis = ( ChartAxisImpl )chart.PrimaryValueAxis;
      //    break;

      //  case ExcelAxisType.Value:
      //    crossAxis = ( ChartAxisImpl )( bPrimary ?
      //      chart.PrimaryCategoryAxis :
      //      chart.SecondaryCategoryAxis );
      //    break;

      //  default:
      //    throw new InvalidOperationException();
      //}

      //ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.CrossAxisTag,
      //  crossAxis.AxisId.ToString() );
    }
    /// <summary>
    /// Extracts tick mark.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <returns>Extracted value.</returns>
    private ExcelTickMark ParseTickMark( XmlReader reader )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      string strTickValue = ChartParserCommon.ParseValueTag( reader );
      return s_dictTickMarkToAttributeValue[ strTickValue ];
    }
    /// <summary>
    /// Extracts tick label position.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="axis">Axis to set tick label position.</param>
    private void ParseTickLabel( XmlReader reader, ChartAxisImpl axis )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( axis == null )
        throw new ArgumentNullException( "axis" );

      if( reader.LocalName != ChartConstants.TickLabelPositionTag )
        throw new XmlException( "Unexpected xml tag." );

      string strValue = ChartParserCommon.ParseValueTag( reader );
      axis.TickLabelPosition = s_dictTickLabelToAttributeValue[ strValue ];
    }
    /// <summary>
    /// Extracts number format used by the axis.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="axis">Axis to put extracted number format into.</param>
    private void ParseNumberFormat( XmlReader reader, ChartAxisImpl axis )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( axis == null )
        throw new ArgumentNullException( "axis" );

      if( reader.LocalName != ChartConstants.NumberFormatTag )
        throw new XmlException( "Unexpected xml tag." );

      if( reader.MoveToAttribute( ChartConstants.FormatCodeAttribute ) )
        axis.NumberFormat = reader.Value;

      if (reader.MoveToAttribute(ChartConstants.SourceLinkedAttribute))
          axis.IsSourceLinked = XmlConvert.ToBoolean(reader.Value);

      reader.Read();
    }
    /// <summary>
    /// Extracts single gridline object.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="gridLines">Gridlines to put extracted data into.</param>
    /// <param name="dataHolder">Parent file data holder.</param>
    /// <param name="relations">Chart item relations.</param>
    private void ParseGridlines( XmlReader reader, IChartGridLine gridLines,
      FileDataHolder dataHolder, RelationCollection relations )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( gridLines == null )
        throw new ArgumentNullException( "gridLines" );

      if( !reader.IsEmptyElement )
      {
        reader.Read();

        while( reader.NodeType != XmlNodeType.EndElement )
        {
          if( reader.NodeType == XmlNodeType.Element && reader.LocalName == Drawings.ShapePropertiesTag )
          {
            ChartFillObjectGetterAny getter = new ChartFillObjectGetterAny( gridLines.Border as ChartBorderImpl, null, null,
              gridLines.Shadow as ShadowImpl,gridLines.ThreeD as ThreeDFormatImpl );
            ChartParserCommon.ParseShapeProperties( reader, getter, dataHolder, relations );
            ////reader.Read();

            //while( reader.NodeType != XmlNodeType.EndElement )
            //{
            //  if( reader.NodeType == XmlNodeType.Element && reader.LocalName == Drawings.ShapePropertiesTag )
            //  {
            //    //IChartBorder line = gridLines.Border;
            //    //ChartParserCommon.ParseLineProperties( reader, line );
            //    //throw new NotImplementedException();
            //    //Debug.Assert( false, "Parsing is not implemented yet" );
            //    //reader.Skip();
            //  }
            //  else
            //  {
            //    reader.Skip();
            //  }
            //}

            //reader.Read();
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
    /// Extracts axis position.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="axis">Axis to serialize.</param>
    /// <returns>Extracted Axis position.</returns>
    private string ParseAxisPosition( XmlReader reader, ChartAxisImpl axis )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( axis == null )
        throw new ArgumentNullException( "valueAxis" );

      // TODO: we don't support any positioning on the current moment, we should add it later.

      string strPosition = ChartParserCommon.ParseValueTag( reader );
      //reader.Skip();
      //string strPosition = null;

      //switch( axis.AxisType )
      //{
      //  case ExcelAxisType.Category:
      //    strPosition = axis.IsPrimary ? ChartConstants.AxisPosBottom : ChartConstants.AxisPosTop;
      //    break;

      //  case ExcelAxisType.Value:
      //    strPosition = axis.IsPrimary ? ChartConstants.AxisPosLeft : ChartConstants.AxisPosRight;
      //    break;

      //  case ExcelAxisType.Serie:
      //    strPosition = axis.IsPrimary ? ChartConstants.AxisPosBottom : ChartConstants.AxisPosTop;
      //    break;
      //}

      //if( ChartConstants.AxisPositionTag != null )
      //{
      //  ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.AxisPositionTag,
      //    strPosition );
      //}
      return strPosition;
    }
    /// <summary>
    /// Extracts scaling tag and all necessary child tags.
    /// </summary>
    /// <param name="reader">XmlReader to serialize into.</param>
    /// <returns>Extracted Axis scale.</returns>
    private ChartAxisScale ParseScaling( XmlReader reader )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( reader.LocalName != ChartConstants.ScalingTag )
        throw new XmlException( "Unexpected xml tag." );

      reader.Read();

      ChartAxisScale scaleSettings = new ChartAxisScale();

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case ChartConstants.LogBaseTag:
              reader.Read();
              scaleSettings.LogScale = true;
              break;

            case ChartConstants.AxisOrientationTag:
              string strOrientation = ChartParserCommon.ParseValueTag( reader );

              if( strOrientation == ChartConstants.MaxMinOrientation )
                scaleSettings.Reversed = true;
              break;

            case ChartConstants.MaximumTag:
              scaleSettings.MaximumValue = ChartParserCommon.ParseDoubleValueTag( reader );
              break;

            case ChartConstants.MinimumTag:
              scaleSettings.MinimumValue = ChartParserCommon.ParseDoubleValueTag( reader );
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
      return scaleSettings;
    }
    /// <summary>
    /// Extracts display unit from XmlReader.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="valueAxis">Axis to put extracted data into.</param>
    private void ParseDisplayUnit( XmlReader reader, ChartValueAxisImpl valueAxis,
      RelationCollection relations )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( valueAxis == null )
        throw new ArgumentNullException( "valueAxis" );

      if( reader.LocalName != ChartConstants.DisplayUnitsTag )
        throw new XmlException();

      if( !reader.IsEmptyElement )
      {
        reader.Read();

        while( reader.NodeType != XmlNodeType.EndElement )
        {
          if( reader.NodeType == XmlNodeType.Element )
          {
            switch( reader.LocalName )
            {
              case ChartConstants.BuiltInUnitTag:
                ParseBuiltInDisplayUnit( reader, valueAxis );
                break;

              case ChartConstants.DisplayUnitsLabel:
                valueAxis.HasDisplayUnitLabel = true;
                IInternalChartTextArea unitsLabel = valueAxis.DisplayUnitLabel as IInternalChartTextArea;
                FileDataHolder holder = valueAxis.ParentChart.ParentWorkbook.DataHolder;
                ChartParserCommon.ParseTextArea( reader, unitsLabel, holder, relations );
                //reader.Skip();
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
        //writer.WriteStartElement( ChartConstants.DisplayUnitsTag, ChartConstants.CNamespace );

        //if( displayUnit != ExcelChartDisplayUnit.Custom )
        //{
        //  string strUnit = ( ( Excel2007ChartDisplayUnit )displayUnit ).ToString();
        //  ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.BuiltInUnitTag, strUnit );
        //}
        //else
        //{
        //  ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.CustomUnitTag, unitValue.ToString() );
        //}

        //writer.WriteEndElement();
      }

      reader.Read();
    }
    /// <summary>
    /// Extracts built-in display unit.
    /// </summary>
    /// <param name="reader">XmlReader to get value from.</param>
    /// <param name="valueAxis">Value axis to put extracted value into.</param>
    private void ParseBuiltInDisplayUnit( XmlReader reader, ChartValueAxisImpl valueAxis )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( valueAxis == null )
        throw new ArgumentNullException( "valueAxis" );

      string strDisplayUnit = ChartParserCommon.ParseValueTag( reader );
      Excel2007ChartDisplayUnit displayUnit = ( Excel2007ChartDisplayUnit )Enum.Parse(
        typeof( Excel2007ChartDisplayUnit ), strDisplayUnit, false );

      valueAxis.DisplayUnit = ( ExcelChartDisplayUnit )displayUnit;
    }
    /// <summary>
    /// Parses additional tags of category axis.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="axis">Axis to put data into.</param>
    /// <param name="relations">Chart relations.</param>
    private void CategoryAxisTagParsing( XmlReader reader, ChartAxisImpl axis, RelationCollection relations )
    {
      if( reader.NodeType == XmlNodeType.Element )
      {
        ChartCategoryAxisImpl category = axis as ChartCategoryAxisImpl;

        switch( reader.LocalName )
        {
          case ChartConstants.LabelOffsetTag:
            category.Offset = ChartParserCommon.ParseIntValueTag( reader );
            break;

          case ChartConstants.TickLabelSkip:
            category.TickLabelSpacing = ChartParserCommon.ParseIntValueTag( reader );
            break;

          case ChartConstants.TickMarkSkip:
            category.TickMarkSpacing = ChartParserCommon.ParseIntValueTag( reader );
            break;

          case ChartConstants.AutoCategoryAxis:
            category.CategoryType = ( ChartParserCommon.ParseBoolValueTag( reader ) ) ?
              ExcelCategoryType.Automatic :
              ExcelCategoryType.Category;
            break;

          case ChartConstants.MajorUnitTag:
            category.MajorUnit = ChartParserCommon.ParseDoubleValueTag( reader );
            break;

          case ChartConstants.MinorUnitTag:
            category.MinorUnit = ChartParserCommon.ParseDoubleValueTag( reader );
            break;

          case ChartConstants.NoMultiLvlLblTag:
            category.NoMultiLevelLabel = ChartParserCommon.ParseBoolValueTag( reader );
            category.m_showNoMultiLvlLbl = true;
            break;

          default:
            // TODO: finish implementation
            reader.Skip();
            break;
        }
      }
      else
      {
        reader.Skip();
      }
    }
    /// <summary>
    /// Parses additional tags of date axis.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="axis">Axis to put data into.</param>
    /// <param name="relations">Chart relations.</param>
    private void DateAxisTagParsing( XmlReader reader, ChartAxisImpl axis, RelationCollection relations )
    {
      if( reader.NodeType == XmlNodeType.Element )
      {
        ChartCategoryAxisImpl category = axis as ChartCategoryAxisImpl;
        switch( reader.LocalName )
        {
          case ChartConstants.LabelOffsetTag:
            category.Offset = ChartParserCommon.ParseIntValueTag( reader );
            break;

          case ChartConstants.MajorUnitTag:
            category.MajorUnit = ChartParserCommon.ParseDoubleValueTag(reader);
            break;

          case ChartConstants.MinorUnitTag:
            category.MinorUnit = ChartParserCommon.ParseDoubleValueTag(reader);
            break;

          case ChartConstants.MinorTimeUnit:
            string unitScale = ChartParserCommon.ParseValueTag( reader );
            ( ( ChartCategoryAxisImpl )axis ).MinorUnitScale = GetChartBaseUnitFromString( unitScale );
            ((ChartCategoryAxisImpl)axis).MinorUnitScaleIsAuto = false;
            break;

          case ChartConstants.MajorTimeUnit:
            unitScale = ChartParserCommon.ParseValueTag( reader );
            ( ( ChartCategoryAxisImpl )axis ).MajorUnitScale = GetChartBaseUnitFromString( unitScale );
            ((ChartCategoryAxisImpl)axis).MajorUnitScaleIsAuto = false;
            break;

          case ChartConstants.BaseTimeUnitTag:
            unitScale = ChartParserCommon.ParseValueTag( reader );
            ( ( ChartCategoryAxisImpl )axis ).BaseUnit = GetChartBaseUnitFromString( unitScale );
            ((ChartCategoryAxisImpl)axis).BaseUnitIsAuto = false;
            break;

#if DEBUG_NOTIMPLEMENTED
          case ChartConstants.BaseTimeUnitTag:
            //if( !axis.BaseUnitIsAuto )
            //{
            //  string strBaseUnit = ConvertDateUnitToString( axis.BaseUnit );
            //  ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.BaseTimeUnitTag,
            //    strBaseUnit );
            //}
            //break;
            throw new NotImplementedException();

          case ChartConstants.MajorUnitTag:
            //if( !axis.IsAutoMajor )
            //{
            //  ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.MajorUnitTag,
            //    XmlConvert.ToString( axis.MajorUnit ) );
            //}
            //break;
            throw new NotImplementedException();


          case ChartConstants.MinorUnitTag:
            //if( !axis.IsAutoMinor )
            //{
            //  ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.MinorUnitTag,
            //    XmlConvert.ToString( axis.MinorUnit ) );
            //}
            //break;
            throw new NotImplementedException();

          case ChartConstants.MajorTimeUnit:
            //string strUnit = ConvertDateUnitToString( axis.MajorUnitScale );
            //ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.MajorTimeUnit, strUnit );
            //break;
            throw new NotImplementedException();

          case ChartConstants.MinorTimeUnit:
            //strUnit = ConvertDateUnitToString( axis.MinorUnitScale );
            //ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.MinorTimeUnit, strUnit );
            //break;
            throw new NotImplementedException();
#endif

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
    /// <summary>
    /// Parses additional tags of value axis.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="axis">Axis to put data into.</param>
    /// <param name="relations">Chart relations.</param>
    private void ValueAxisTagParsing( XmlReader reader, ChartAxisImpl axis, RelationCollection relations )
    {
      if( reader.NodeType == XmlNodeType.Element )
      {
        //TODO: implement crossBetween tag support.
        ChartValueAxisImpl valueAxis = axis as ChartValueAxisImpl;
        switch( reader.LocalName )
        {
          case ChartConstants.CrossBetweenTag:
            //ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.CrossBetweenTag,
            //  ChartConstants.BetweenValue );
            string crossBetweenValue = ChartParserCommon.ParseValueTag( reader );
            ChartImpl chart = valueAxis.ParentChart;
            IChartCategoryAxis catergoryAxis = valueAxis.IsPrimary ?
              chart.PrimaryCategoryAxis :
              chart.SecondaryCategoryAxis;

            catergoryAxis.IsBetween = ( crossBetweenValue == ChartConstants.BetweenValue );
            break;

          case ChartConstants.MajorUnitTag:
            valueAxis.SetMajorUnit( ChartParserCommon.ParseDoubleValueTag( reader ) );
            break;

          case ChartConstants.MinorUnitTag:
            valueAxis.SetMinorUnit( ChartParserCommon.ParseDoubleValueTag( reader ) );
            break;

          case ChartConstants.DisplayUnitsTag:
            ParseDisplayUnit( reader, valueAxis, relations );
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
    /// <summary>
    /// Parses additional tags of series axis.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="axis">Axis to put data into.</param>
    /// <param name="relations">Chart relations.</param>
    private void SeriesAxisTagParsing( XmlReader reader, ChartAxisImpl axis, RelationCollection relations )
    {
      if( reader.NodeType == XmlNodeType.Element )
      {
        ChartSeriesAxisImpl seriesAxis = axis as ChartSeriesAxisImpl;

        switch( reader.LocalName )
        {
          case ChartConstants.TickLabelSkip:
            seriesAxis.TickLabelSpacing = ChartParserCommon.ParseIntValueTag( reader );
            break;

          case ChartConstants.TickMarkSkip:
            seriesAxis.TickMarkSpacing = ChartParserCommon.ParseIntValueTag( reader );
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

    private ExcelChartBaseUnit GetChartBaseUnitFromString( string baseUnitScale )
    {
      baseUnitScale = PrepareBaseUnitScale( baseUnitScale );
      return ( ExcelChartBaseUnit )Enum.Parse( typeof( ExcelChartBaseUnit ), baseUnitScale, false );
    }
    private string PrepareBaseUnitScale( string baseUnitScale )
    {
      baseUnitScale = RemoveCharUnSafeAtLast( baseUnitScale );
      char firstChar = baseUnitScale[ 0 ];
      return Char.ToUpper( firstChar ) + baseUnitScale.Substring( 1 );
    }
    private string RemoveCharUnSafeAtLast( string baseUnitScale )
    {
      return baseUnitScale.Substring( 0, baseUnitScale.Length - 1 );
    }
    #endregion
  }
}
