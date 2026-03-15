#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Diagnostics;
using System.Collections.Generic;

using Syncfusion.XlsIO.Implementation.Shapes;
using Syncfusion.XlsIO.Implementation.XmlSerialization.Shapes;
using Syncfusion.XlsIO.Implementation.XmlSerialization.Constants;
using Syncfusion.XlsIO.Implementation.XmlReaders;
using Syncfusion.XlsIO.Implementation.Charts;

using Syncfusion.XlsIO.Parser.Biff_Records.Charts;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser;

using Syncfusion.XlsIO.Interfaces.Charts;
using Syncfusion.XlsIO.Interfaces;
using System.Collections;
using Syncfusion.XlsIO.Implementation.XmlReaders.Shapes;
using Syncfusion.XlsIO.Parser.Biff_Records.Formula;
using System.Globalization;

#if ( WINRT )
using Windows.UI;
#endif

#if  (SILVERLIGHT || WP)
using System.Windows.Media;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;
#endif

namespace Syncfusion.XlsIO.Implementation.XmlSerialization.Charts
{
  /// <summary>
  /// This class is responsible for charts parsing.
  /// </summary>
  public class ChartParser
  {
    #region Constants
    internal const float DefaultTitleSize = 18;
    #endregion

    #region Fields
    private WorkbookImpl m_book;
    #endregion

    #region Constructor
    public ChartParser(WorkbookImpl book)
    {
        m_book = book;
    }
    #endregion
    #region Methods
    /// <summary>
    /// Extracts chart from XmlReader.
    /// </summary>
    /// <param name="reader">XmlReader to serialize into.</param>
    /// <param name="chart">Chart to serialize.</param>
    /// <param name="relations">Chart item relations.</param>
    public void ParseChart( XmlReader reader, ChartImpl chart, RelationCollection relations )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      while( reader.NodeType != XmlNodeType.Element )
        reader.Read();

      if( reader.LocalName != ChartConstants.ChartSpaceTag )
        throw new XmlException( "Unexpected xml tag." );

      reader.Read();

      IChartFrameFormat chartArea = chart.ChartArea;
      chartArea.Interior.UseAutomaticFormat = true;
      chartArea.Border.AutoFormat = true;

      while( reader.NodeType != XmlNodeType.EndElement && reader.LocalName != ChartConstants.ChartSpaceTag )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case ChartConstants.ChartTag:
              ParseChartElement( reader, chart, relations );
              break;

            case ChartConstants.RoundedCornersTag:
              chart.HasPlotArea = true;
              chart.PlotArea.IsBorderCornersRound = ChartParserCommon.ParseBoolValueTag( reader );
              break;

            case Drawings.ShapePropertiesTag:
              IChartFillObjectGetter objectGetter = new ChartFillObjectGetterAny(
                chartArea.Border as ChartBorderImpl,
                chartArea.Interior as ChartInteriorImpl,
                              chartArea.Fill as IInternalFill,
                              chartArea.Shadow as ShadowImpl,
                              chartArea.ThreeD as ThreeDFormatImpl);

              ChartParserCommon.ParseShapeProperties( reader, objectGetter,
                chart.ParentWorkbook.DataHolder, relations );
              break;

            case ChartConstants.ChartStyleTag:
              chart.Style = ChartParserCommon.ParseIntValueTag( reader );
              break;

            case ChartConstants.UserShapesTag:
              ParseUserShapes( reader, chart, relations );
              break;

            case ChartConstants.PivotSourceTag:
              ParsePivotSource( reader, chart );
              break;

            case ChartConstants.PrintSettings:
              ParsePrintSettings( reader, chart, relations );
              break;

            case Excel2007Serializator.Extensionlist:
              ParseExtensionList(reader, chart);
              break;

            case ChartConstants.AlternateContentTag:
              chart.AlternateContent = ShapeParser.ReadNodeAsStream( reader );
              break;
              
              case ChartConstants.TextPropertiesTag:
              ParseDefaultTextProperties(reader, chart);
              break;
            default:
              reader.Read();
              break;
          }
        }
        else
        {
          reader.Skip();
        }
      }

      
      chart.DetectIsInRowOnParsing();          
      if (chart.DataRange != null && chart.Categories.Count > 0)
        {
            IRange values = null;
          
            chart.IsSeriesInRows = DetectIsInRow(chart.Series[0].Values);
            GetSerieOrAxisRange(chart.DataRange, chart.IsSeriesInRows, out values);
            GetSerieOrAxisRange(values, !chart.IsSeriesInRows, out values);
            int count = values.Count / chart.Series[0].Values.Count;
            for (int i = 0; i < chart.Series[0].Values.Count; i++)
            {
                IRange cate_range = ChartImpl.GetCategoryRange(values, out values, count, chart.IsSeriesInRows);
                (chart.Categories[i] as ChartCategory).CategoryLabel = chart.Series[0].CategoryLabels;
                (chart.Categories[i] as ChartCategory).Values = cate_range;
                if (chart.Categories[0].CategoryLabel != null)
                    (chart.Categories[i] as ChartCategory).Name = chart.Categories[0].CategoryLabel.Cells[i].Text;
                else
                {
                    (chart.Categories[i] as ChartCategory).Name = (i + 1).ToString();
                    if (chart.Legend != null &&
                        chart.Legend.LegendEntries != null && 
                        chart.Legend.LegendEntries.Count > i &&
                        chart.Legend.LegendEntries[i].TextArea != null)
                    {
                        chart.Legend.LegendEntries[i].TextArea.Text = (chart.Categories[i] as ChartCategory).Name;
                        chart.Legend.LegendEntries[i].IsFormatted = false;
                    }
                }
            }
            
        }
      else
      {
          bool hasSeries=chart.Series != null && chart.Series.Count > 0 && chart.Series[0].Values != null ;
          
          if (hasSeries && chart.Series[0].CategoryLabels != null && chart.Categories.Count > 0)
          {
              IRange catLabelRange = chart.Series[0].CategoryLabels;
              int categoryCount = catLabelRange.Count;
              int startRow = catLabelRange.Row;
              int startColumn = catLabelRange.Column;
              IRange range = null;
              bool hasText = false;
              
              if (catLabelRange != null && (catLabelRange.GetType() != typeof(ExternalRange)) && (catLabelRange.Worksheet != null))
              {
                  hasText = true;
                  //Need to find the needed ranges for assign the category names
                  if (!chart.IsSeriesInRows && catLabelRange.LastRow >= startRow + chart.Categories.Count - 1)
                      range = catLabelRange[startRow, startColumn, startRow + chart.Categories.Count - 1, startColumn];
                  else if (catLabelRange.LastColumn >= startColumn + chart.Categories.Count - 1)
                      range = catLabelRange[startRow, startColumn, startRow, startColumn + chart.Categories.Count - 1];
              }

              for (int i = 0; i < chart.Categories.Count; i++)
              {
                  (chart.Categories[i] as ChartCategory).CategoryLabel = catLabelRange;
                  (chart.Categories[i] as ChartCategory).Values = chart.Series[0].Values;
                  if (hasText && range != null && i < categoryCount)
                  {
                      (chart.Categories[i] as ChartCategory).Name = range.Cells[i].DisplayText;
                  }
              }
          }
      }

      if (chart.Series.Count!=0 &&  (chart.Series[0] as ChartSerieImpl).FilteredValue != null )
          {
              FindFilter(chart.Categories, (chart.Series[0] as ChartSerieImpl).FilteredValue, chart.Series[0].Values.AddressGlobal, chart.Series[0], chart.IsSeriesInRows);
          }
      
      reader.Read();
    }
    /// <summary>
    /// Parses the Chart default text properties.
    /// </summary>
    /// <param name="reader">The Xml reader to parse from.</param>
    /// <param name="chart">The chart to put the extracted data. </param>
    private void ParseDefaultTextProperties(XmlReader reader, ChartImpl chart)
    {
        if (reader == null)
            throw new ArgumentNullException("reader");
        
        if (reader.LocalName != ChartConstants.TextPropertiesTag)
            throw new XmlException("Unexpected tag name");

        if (chart == null)
            throw new ArgumentNullException("chart");

        chart.DefaultTextProperty = ShapeParser.ReadNodeAsStream(reader);
        chart.DefaultTextProperty.Position = 0;

        XmlReader chartTextReader = UtilityMethods.CreateReader(chart.DefaultTextProperty);

        if (chartTextReader.LocalName != ChartConstants.TextPropertiesTag)
            throw new XmlException();


        if (!chartTextReader.IsEmptyElement)
        {
            chartTextReader.Read();
            while (chartTextReader.NodeType != XmlNodeType.EndElement)
            {
                if (chartTextReader.NodeType == XmlNodeType.Element)
                {
                    switch (chartTextReader.LocalName)
                    {
                        case Drawings.TextBodyPropertiesTag:
                            ParseChartBodyProperties(chartTextReader, chart);
                            break;
                        case Drawings.Paragraphs:
                            ParserChartParagraphs(chartTextReader, chart);
                            break;
                        default:
                            chartTextReader.Skip();
                            break;
                     }
                 } 
                else
                {
                    chartTextReader.Skip();
                }
            }
        }
        chartTextReader.Read();
    }
    /// <summary>
    /// Parses chart body properties
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="chart"></param>
    private void ParseChartBodyProperties(XmlReader reader, ChartImpl chart)
    {
        if (reader == null)
            throw new ArgumentNullException("reader");

        if (chart == null)
            throw new ArgumentNullException("chart");

        if (reader.LocalName != Drawings.TextBodyPropertiesTag)
            throw new XmlException();
        //TODO:
        //It does not contain the specific elements of the Body properties so skipping the records.
        
        reader.MoveToElement();
        reader.Skip();
    }
    /// <summary>
    /// Parses Chart Paragraph Properties
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="chart"></param>
    private void ParserChartParagraphs(XmlReader reader, ChartImpl chart)
    {
        WorksheetDataHolder sheetHolder = chart.DataHolder;
        FileDataHolder holder = sheetHolder.ParentHolder;                

        while (!(reader.LocalName == Drawings.Paragraphs && reader.NodeType == XmlNodeType.EndElement))
        {
            if (reader.NodeType == XmlNodeType.Element && reader.LocalName == Drawings.DefaultParagraphProperites)
            {
                TextSettings textSettngs = ChartParserCommon.ParseDefaultParagraphProperties(reader, holder.Parser);
                ChartParserCommon.CopyDefaultSettings(chart.Font as IInternalFont, textSettngs);
                CheckDefaultTextSettings(chart);
            }
            else
            {
                reader.Read();
            }
        }                      
    }
    ///<summary>
    ///Checks Default Text Settings for the axis.
    ///</summary>
    private void CheckDefaultTextSettings(ChartImpl chart)
    {        
        List<ChartAxisImpl> axisList = new List<ChartAxisImpl>();

        axisList.Add((ChartAxisImpl)chart.PrimaryCategoryAxis);
        axisList.Add((ChartAxisImpl)chart.PrimaryValueAxis);        
        axisList.Add((ChartAxisImpl)chart.SecondaryValueAxis);
        axisList.Add((ChartAxisImpl)chart.SecondaryCategoryAxis);

        foreach (ChartAxisImpl axis in axisList)
        {

                        if (axis != null && axis.IsDefaultTextSettings == true)
                        {
                            axis.IsChartFont = true;
                            axis.Font = (FontWrapper)((FontWrapper)chart.Font).Clone(chart);
                            axis.IsChartFont = false;
                        }
                   
        }

        
     }
        /// <summary>
        /// Parses the extension list.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="chart">The chart.</param>
        private void ParseExtensionList(XmlReader reader, ChartImpl chart)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");
            if (chart == null)
                throw new ArgumentNullException("chart");

              if (reader.IsEmptyElement)
              {
                  reader.Read();
                  return;
              }

            reader.Read();
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case Excel2007Serializator.Extension:
                            ParseExtension(reader, chart);
                            break;
                    }
                }
            }
            reader.Read();
        }

        /// <summary>
        /// Parses the extension.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="chart">The chart.</param>
        private void ParseExtension(XmlReader reader, ChartImpl chart)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");
            if (chart == null)
                throw new ArgumentNullException("chart");

            reader.Read();

            while (reader.NodeType != XmlNodeType.EndElement)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case ChartConstants.PivotOptionsTag:
                            ParsePivotOptions(reader, chart);
                            break;
                    }
                }
            }
            reader.Read();
        }

        /// <summary>
        /// Parses the pivot options.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="chart">The chart.</param>
        private void ParsePivotOptions(XmlReader reader, ChartImpl chart)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");
            if (chart == null)
                throw new ArgumentNullException("chart");

            if (reader.LocalName != ChartConstants.PivotOptionsTag)
                throw new XmlException("Unexpected tag name");

            reader.Read();


            while (reader.NodeType != XmlNodeType.EndElement)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case ChartConstants.ShowZoneCategoryTag:
                            if (reader.MoveToAttribute(ChartConstants.ValueAttribute))
                            {
                                chart.ShowAxisFieldButtons = true;
                            }
                            break;
                        case ChartConstants.ShowZoneDataTag:
                            if (reader.MoveToAttribute(ChartConstants.ValueAttribute))
                            {
                                chart.ShowValueFieldButtons = true;
                            }
                            break;
                        case ChartConstants.ShowZoneFilterTag:
                            if (reader.MoveToAttribute(ChartConstants.ValueAttribute))
                            {
                                chart.ShowReportFilterFieldButtons = true;
                            }
                            break;
                        case ChartConstants.ShowZoneSeriesTag:
                            if (reader.MoveToAttribute(ChartConstants.ValueAttribute))
                            {
                                chart.ShowLegendFieldButtons = true;
                            }
                            break;
                        case ChartConstants.ShowZoneVisibleTag:
                            if (reader.MoveToAttribute(ChartConstants.ValueAttribute))
                            {
                                chart.ShowAllFieldButtons = true;
                            }
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
            if(reader.LocalName==ChartConstants.PivotOptionsTag)
                reader.Read();
        }
        /// <summary>
    /// Extracts chart print settings from the reader.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="chart">Chart to put extracted data into.</param>
    /// <param name="relations">Chart relations.</param>
    private void ParsePrintSettings( XmlReader reader, ChartImpl chart, RelationCollection relations )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      if( reader.LocalName != ChartConstants.PrintSettings )
        throw new XmlException();

      ChartPageSetupConstants constants = new ChartPageSetupConstants();

      if( !reader.IsEmptyElement )
      {
        reader.Read();
        IPageSetupBase pageSetup = chart.PageSetup;

        while( reader.NodeType != XmlNodeType.EndElement )
        {
          if( reader.NodeType == XmlNodeType.Element )
          {
            switch( reader.LocalName )
            {
              case PageSetup.PrintOptionsTag:
                Excel2007Parser.ParsePrintOptions( reader, pageSetup );
                break;

              case ChartConstants.PageMarginsTag:
                Excel2007Parser.ParsePageMargins( reader, pageSetup, constants );
                break;

              case PageSetup.PageSetupTag:
                Excel2007Parser.ParsePageSetup( reader, ( PageSetupBaseImpl )pageSetup );
                break;

              case PageSetup.HeaderFooterTag:
                Excel2007Parser.ParseHeaderFooter( reader, ( PageSetupBaseImpl )pageSetup );
                break;

              case Vml.LegacyDrawingHF:
                Excel2007Parser.ParseLegacyDrawingHF( reader, chart, relations );
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
    /// Extracts pivot source.
    /// </summary>
    /// <param name="reader">XmlReader to get pivot source from.</param>
    /// <param name="chart">Chart to put extracted pivot source into.</param>
    private void ParsePivotSource( XmlReader reader, ChartImpl chart )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      if( reader.LocalName != ChartConstants.PivotSourceTag )
        throw new XmlException( "Unexpected xml tag" );

      reader.Read();
      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case ChartConstants.PivotSourceNameTag:
              string value = reader.ReadElementContentAsString();
              chart.PivotSource = GetPivotTable( chart.Workbook, value );
              chart.PreservedPivotSource = value;

              if( value != null )
              {
                chart.ShowAllFieldButtons = false;
                chart.ShowAxisFieldButtons = false;
                chart.ShowLegendFieldButtons = false;
                chart.ShowReportFilterFieldButtons = false;
                chart.ShowValueFieldButtons = false;
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

    private IPivotTable GetPivotTable( IWorkbook book, string pivotSourceName )
    {
      int pivotNameStart = pivotSourceName.LastIndexOf( '!' );

      if( pivotNameStart < 0 )
        return null;

      string pivotName = pivotSourceName.Substring( pivotNameStart + 1 );
      pivotSourceName = pivotSourceName.Substring( 0, pivotNameStart );

      int closeSquareIndex = pivotSourceName.IndexOf( ']' );

      if( closeSquareIndex < 0 )
        return null;

      string sheetName = pivotSourceName.Substring( closeSquareIndex + 1 );

      // TODO: we can add support of external data sources here.
      return book.Worksheets[ sheetName ].PivotTables[ pivotName ];
    }
    /// <summary>
    /// Parses user shapes.
    /// </summary> 
    /// <param name="reader">XmlReader to serialize into.</param>
    /// <param name="chart">Chart to serialize.</param>
    /// <param name="relations">Chart item relations.</param>
    private void ParseUserShapes( XmlReader reader, ChartImpl chart, RelationCollection relations )
    {
      reader.MoveToAttribute( "id", Excel2007Serializator.RelationNamespace );
      string id = reader.Value;
      Dictionary<string, object> toRemove = new Dictionary<string,object>();
      Relation relation = relations[ id ];
      chart.DataHolder.ParseDrawings( chart, relation, toRemove );
        //ParentHolder.Parser.ParseDrawings( reader, chart, path, lstRelationIds, dictItemsToRemove );
      //throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Parses main chart xml tag.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="chart">Chart to put extracted data into.</param>
    /// <param name="relations">Chart's relations.</param>
    private void ParseChartElement( XmlReader reader, ChartImpl chart, RelationCollection relations )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      if( reader.LocalName != ChartConstants.ChartTag )
        throw new XmlException( "Unexpected xml tag." );

      reader.Read();

      WorksheetDataHolder sheetHolder = chart.DataHolder;
      FileDataHolder holder = sheetHolder.ParentHolder;
      //RelationsCollection relations = chart.Relations;
      Chart3DRecord chart3D = null;

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case ChartConstants.View3DTag:
              chart3D = ParseView3D( reader, chart );
              break;

            case ChartConstants.PlotAreaTag:
              ParsePlotArea( reader, chart, relations, holder.Parser );
              break;

            case ChartConstants.LegendTag:
              chart.HasLegend = true;
              ParseLegend( reader, chart.Legend, chart, relations );
              break;

            case ChartConstants.FloorTag:
              ParseSurface( reader, chart.Floor, holder, relations );
              break;

            case ChartConstants.SideWallTag:
              ParseSurface( reader, chart.SideWall, holder, relations );
              break;

            case ChartConstants.BackWallTag:
              ParseSurface( reader, chart.Walls, holder, relations );              
              break;

            //case ChartConstants.PlotVisibleOnlyTag:
            //  //ChartSerializatorCommon.SerializeBoolValueTag( writer, ChartConstants.PlotVisibleOnlyTag,
            //  //  chart.PlotVisibleOnly );
            //  break;

            case ChartConstants.DisplayBlanksAsTag:
              if (reader.MoveToAttribute(ChartConstants.ValueAttribute))
#if (SILVERLIGHT)
                  chart.DisplayBlanksAs = (ExcelChartPlotEmpty)Enum.Parse(typeof(Excel2007ChartPlotEmpty), reader.Value.ToString(), true);
#else
                  chart.DisplayBlanksAs = (ExcelChartPlotEmpty)Enum.Parse(typeof(Excel2007ChartPlotEmpty), reader.Value.ToString().ToLower());
#endif
              break;

            case ChartConstants.TitleTag:
            //  //ChartSerializatorCommon.SerializeTextArea( writer, chart.ChartTitleArea, holder, relations );
              IInternalChartTextArea textArea = chart.ChartTitleArea as IInternalChartTextArea;
              ChartParserCommon.SetWorkbook(m_book);
              ChartParserCommon.ParseTextArea( reader, textArea, holder, relations, DefaultTitleSize );
              break;
            case ChartConstants.AutoTitleDeletedTag:
              ParseAutoTitleDeleted(reader, chart);
              break;
            case ChartConstants.PivotFormats:
              ParsePivotFormats( reader, chart );
              break;
            
            case ChartConstants.PlotVisibleOnlyTag:
              chart.ShowPlotVisible = true;
              reader.Skip();
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


      //SerializePivotFormats( writer, chart );

      // other tags: showDLblsOverMax

      reader.Read();
      Set3DSettings( chart, chart3D );
    }

    private void ParseAutoTitleDeleted(XmlReader reader, ChartImpl chart)
    {
        if (reader == null)
            throw new ArgumentNullException("reader");

        if (chart == null)
            throw new ArgumentNullException("chart");

        if (reader.MoveToAttribute(ChartConstants.ValueAttribute))
            chart.HasAutoTitle = XmlConvert.ToBoolean(reader.Value);

        reader.Read();
        
    }
    /// <summary>
    /// Extracts pivot formats.
    /// </summary>
    /// <param name="reader">XmlReader to get pivot formats from.</param>
    /// <param name="chart">Chart to put extracted pivot formats into.</param>
    private void ParsePivotFormats( XmlReader reader, ChartImpl chart )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      chart.PivotFormatsStream = ShapeParser.ReadNodeAsStream( reader );
    }
    /// <summary>
    /// Copies 3D settings from Chart3DRecord into chart.
    /// </summary>
    /// <param name="chart">Chart to copy data into.</param>
    /// <param name="chart3D">Record to copy data from.</param>
    private void Set3DSettings( ChartImpl chart, Chart3DRecord chart3D )
    {
      if( chart == null )
        throw new ArgumentNullException( "chart" );

      if( chart3D == null || chart.Series.Count == 0 /*&& chart.PivotSource == null*/ )
        return;

      if( !chart3D.IsDefaultElevation )
        chart.Elevation = chart3D.ElevationAngle;

      chart.AutoScaling = chart3D.IsAutoScaled;
      chart.HeightPercent = chart3D.Height;

      if( !chart3D.IsDefaultRotation )
        chart.Rotation = chart3D.RotationAngle;

      chart.DepthPercent = chart3D.Depth;
      chart.RightAngleAxes = chart3D.IsPerspective;
      chart.Perspective = chart3D.DistanceFromEye;
    }
    ///// <summary>
    ///// Serializes chart printer settings.
    ///// </summary>
    ///// <param name="writer">XmlWriter to serialize into.</param>
    ///// <param name="pageSetup">Object that stores settings to serialize.</param>
    //private void SerializePrinterSettings( XmlWriter writer, IChartPageSetup pageSetup )
    //{
    //  if( writer == null )
    //    throw new ArgumentNullException( "writer" );

    //  if( pageSetup == null )
    //    throw new ArgumentNullException( "pageSetup" );
    //  throw new Exception( "The method or operation is not implemented." );
    //}
    /// <summary>
    /// Extracts chart legend from XmlReader.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="legend">Chart legend to put extracted data into.</param>
    /// <param name="chart">Chart object that stores specified legend.</param>
    /// <param name="relations">Chart relations collection.</param>
    private void ParseLegend( XmlReader reader, IChartLegend legend,
      ChartImpl chart, RelationCollection relations )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( legend == null )
        throw new ArgumentNullException( "legend" );

      if( reader.LocalName != ChartConstants.LegendTag )
        throw new XmlException( "Unexpected xml tag." );

      bool bEmpty = reader.IsEmptyElement;

      reader.Read();
      Excel2007Parser parser = chart.ParentWorkbook.DataHolder.Parser;
      (legend as ChartLegendImpl).IsChartTextArea = true;
      legend.TextArea.FontName = ChartAxisParser.DefaultFont;
      legend.TextArea.Size = ChartAxisParser.DefaultFontSize;
      (legend as ChartLegendImpl).IsChartTextArea = false;

      if( !bEmpty )
      {
        while( reader.NodeType != XmlNodeType.EndElement )
        {
          if( reader.NodeType == XmlNodeType.Element )
          {
            switch( reader.LocalName )
            {
              case ChartConstants.LegendPositionTag:
                string strLegendPos = ChartParserCommon.ParseValueTag( reader );
                legend.Position = ( ExcelLegendPosition )( Excel2007LegendPosition )Enum.Parse(
                  typeof( Excel2007LegendPosition ), strLegendPos, false );
                break;

              case ChartConstants.LegendEntryTag:
                ParseLegendEntry( reader, legend, parser );
                break;

              case ChartConstants.TextPropertiesTag:
                (legend as ChartLegendImpl).IsChartTextArea = true;
                IInternalChartTextArea textArea = legend.TextArea as IInternalChartTextArea;
                ParseDefaultTextFormatting( reader, textArea, parser );
                (legend as ChartLegendImpl).IsChartTextArea = false;
                break;

              case Drawings.ShapePropertiesTag:
                IChartFrameFormat frameFormat = legend.FrameFormat;
                ChartFillObjectGetterAny getter = new ChartFillObjectGetterAny(
                  frameFormat.Border as ChartBorderImpl,
                  frameFormat.Interior as ChartInteriorImpl,
                                  frameFormat.Fill as IInternalFill,
                                  frameFormat.Shadow as ShadowImpl,
                                  frameFormat.ThreeD as ThreeDFormatImpl);
                FileDataHolder dataHolder = chart.ParentWorkbook.DataHolder;
                ChartParserCommon.ParseShapeProperties( reader, getter, dataHolder, relations );
                break;

              case ChartConstants.LayoutTag:
                (legend as ChartLegendImpl).Layout = new ChartLayoutImpl(m_book.Application, (legend as ChartLegendImpl), chart);
                ChartParserCommon.ParseChartLayout(reader, (legend as ChartLegendImpl).Layout);
                break;

              case ChartConstants.OverlayTag:
                legend.IncludeInLayout = !ChartParserCommon.ParseBoolValueTag(reader);
                break;

              default:
                // layout
                // TODO: overlay - we don't support it.
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
    /// Extracts legend entry from specified reader.
    /// </summary>
    /// <param name="reader">XmlReader to extract legend entry from.</param>
    /// <param name="legend">Legend to put extracted legend entry into.</param>
    /// <param name="parser">Excel2007Parser object to use if necessary.</param>
    private void ParseLegendEntry( XmlReader reader, IChartLegend legend, Excel2007Parser parser )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( legend == null )
        throw new ArgumentNullException( "legend" );

      if( reader.LocalName != ChartConstants.LegendEntryTag )
        throw new XmlException( "Unexpected xml tag." );

      reader.Read();
      int index = 0;

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case ChartConstants.IndexTag:
              string strIndex = ChartParserCommon.ParseValueTag( reader );
              index = int.Parse( strIndex );
              break;

            case ChartConstants.DeleteTag:
              bool bDeleted = ChartParserCommon.ParseBoolValueTag( reader );
              legend.LegendEntries[ index ].IsDeleted = bDeleted;
              break;

            case ChartConstants.TextPropertiesTag:
              IInternalChartTextArea textArea = legend.LegendEntries[ index ].TextArea as IInternalChartTextArea;
              ParseDefaultTextFormatting( reader, textArea, parser );
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
    /// Extracts 3-D view options from XmlReader.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="chart">Chart to put extracted options into.</param>
    /// <returns>Record with 3D view settings.</returns>
    private Chart3DRecord ParseView3D( XmlReader reader, ChartImpl chart )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      if( reader.LocalName != ChartConstants.View3DTag )
        throw new XmlException( "Unexpected xml tag." );

      reader.Read();
      Chart3DRecord chart3D = ( Chart3DRecord )BiffRecordFactory.GetRecord( TBIFFRecord.Chart3D );

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case ChartConstants.RotationXTag:
              string strRotationX = ChartParserCommon.ParseValueTag( reader );
              chart3D.ElevationAngle = short.Parse( strRotationX );
              break;

            case ChartConstants.HeightPercentTag:
              chart3D.IsAutoScaled = false;
              string strHeightPercent = ChartParserCommon.ParseValueTag( reader );
              chart3D.Height = ushort.Parse( strHeightPercent );
              break;

            case ChartConstants.RotationYTag:
              string strRotation = ChartParserCommon.ParseValueTag( reader );
              chart3D.RotationAngle = ushort.Parse( strRotation );
              break;

            case ChartConstants.DepthPercentTag:
              string strDepth = ChartParserCommon.ParseValueTag( reader );
              chart3D.Depth = ushort.Parse( strDepth );
              break;

            case ChartConstants.RightAngleAxesTag:
              string strRightAngleAxes = ChartParserCommon.ParseValueTag( reader );
              chart3D.IsPerspective = XmlConvert.ToBoolean( strRightAngleAxes );
              break;

            case ChartConstants.PerspectiveTag:
              string strPerspective = ChartParserCommon.ParseValueTag( reader );
              chart3D.DistanceFromEye = ( ushort )( int.Parse( strPerspective ) / 2 );
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
      return chart3D;
    }
    /// <summary>
    /// Serializes error bars.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="series">Series to put extracted error bars into.</param>
    /// <param name="relations">Chart item relations.</param>
    private void ParseErrorBars( XmlReader reader, ChartSerieImpl series, RelationCollection relations )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( series == null )
        throw new ArgumentNullException( "series" );

      if( reader.LocalName != ChartConstants.ErrorBarsTag )
        throw new XmlException( "Unexpected xml tag." );

      reader.Read();
      IChartErrorBars errorBars = null;
      WorkbookImpl book = series.ParentBook;
      FileDataHolder dataHolder = book.DataHolder;
      object[] values = null;
      ChartErrorBarsImpl errorBarImpl = null;
      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case ChartConstants.ErrorBarDirection:
              string strDirection = ChartParserCommon.ParseValueTag( reader );

              if( strDirection == ChartConstants.ErrorBarX )
              {
                series.HasErrorBarsX = true;
                errorBars = series.ErrorBarsX;
              }
              else
              {
                series.HasErrorBarsY = true;
                errorBars = series.ErrorBarsY;
              }
              errorBarImpl = errorBars as ChartErrorBarsImpl;
              break;

            case ChartConstants.ErrorBarTypeTag:
              string strInclude = ChartParserCommon.ParseValueTag( reader );

              if( errorBars == null )
              {
                // TODO: maybe Y error bars is default not for all chart types.
                series.HasErrorBarsY = true;
                errorBars = series.ErrorBarsY;
              }

              errorBars.Include = ( ExcelErrorBarInclude )Enum.Parse(
                typeof( ExcelErrorBarInclude ), strInclude, true );
              break;

            case ChartConstants.ErrorBarValueType:
              string strErrorBar = ChartParserCommon.ParseValueTag( reader );
              Excel2007ErrorBarType errorBarType = ( Excel2007ErrorBarType )Enum.Parse(
                typeof( Excel2007ErrorBarType ), strErrorBar, false );
              errorBars.Type = ( ExcelErrorBarType )errorBarType;
              break;

            case ChartConstants.ErrorBarsNoCap:
              errorBars.HasCap = !ChartParserCommon.ParseBoolValueTag( reader );
              break;

            case ChartConstants.ErrorBarPlusTag:
              IRange plusrange = ParseErrorBarRange(reader, book, out values, errorBars);
              if (!((ChartErrorBarsImpl)errorBars).IsPlusNumberLiteral)
                  errorBars.PlusRange = plusrange;
              if (errorBarImpl == null & errorBars != null)
                  errorBarImpl = errorBars as ChartErrorBarsImpl;
              errorBarImpl.PlusRangeValues = values;
              break;

            case ChartConstants.ErrorBarMinusTag:
              IRange minusrange = ParseErrorBarRange(reader, book, out values, errorBars);
              if (!((ChartErrorBarsImpl)errorBars).IsPlusNumberLiteral)
                  errorBars.MinusRange = minusrange;
              errorBarImpl.MinusRangeValues = values;
              break;

            case ChartConstants.ErrorBarValueTag:
              errorBars.NumberValue = ChartParserCommon.ParseDoubleValueTag( reader );
              break;

            case Drawings.ShapePropertiesTag:
              ChartFillObjectGetterAny getter = new ChartFillObjectGetterAny( 
                errorBars.Border as ChartBorderImpl, null, null,
                errorBars.Shadow as ShadowImpl,errorBars.Chart3DOptions as ThreeDFormatImpl);
              ChartParserCommon.ParseShapeProperties( reader, getter, dataHolder, relations );
              break;

            default:
              //element extLst { CT_ExtensionList }?
              reader.Skip();
              break;
          }
        }
        else
        {
          reader.Skip();
        }
      }
      CheckCustomErrorBarType(errorBars);
      reader.Read();
    }

private void CheckCustomErrorBarType(IChartErrorBars errorBars)
{
    if (errorBars.Type != ExcelErrorBarType.Custom)
        return;
    if (!(((ChartErrorBarsImpl)errorBars).IsPlusNumberLiteral && ((ChartErrorBarsImpl)errorBars).IsMinusNumberLiteral))
    {
        if ((errorBars.MinusRange == null && errorBars.PlusRange == null) || (((ChartErrorBarsImpl)errorBars).PlusRangeValues.Length == 0 && ((ChartErrorBarsImpl)errorBars).MinusRangeValues.Length == 0))
            throw new NotSupportedException("Custom value can be set only if minus range or plus range is set.");
        else if (errorBars.MinusRange != null && errorBars.MinusRange != null)
            errorBars.Include = ExcelErrorBarInclude.Both;
        else
            errorBars.Include = errorBars.MinusRange == null ?
                ExcelErrorBarInclude.Plus :
                ExcelErrorBarInclude.Minus;
    }
        
}

              
    /// <summary>
    /// Extracts error bar range.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="book">Represents Workbook.</param>
    /// <returns>Extracted range.</returns>
    private IRange ParseErrorBarRange( XmlReader reader, IWorkbook book,out object[] values ,IChartErrorBars errorBars)
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );
      bool isPlusTag = false;
      if (reader.LocalName == ChartConstants.ErrorBarPlusTag)
          isPlusTag = true;

      reader.Read();
      string result = null;
      values = null;
      while( reader.NodeType != XmlNodeType.EndElement )
      {
          if (reader.NodeType == XmlNodeType.Element && reader.LocalName == ChartConstants.NumberReferenceTag )
              result = ParseNumReference(reader, out values);
          else if (reader.LocalName == ChartConstants.NumberLiteral)
          {
              if (isPlusTag)
                  ((ChartErrorBarsImpl)errorBars).IsPlusNumberLiteral = true;
              else
                  ((ChartErrorBarsImpl)errorBars).IsMinusNumberLiteral = true;
              

              string formatCode=string.Empty;
              values = ParseDirectlyEnteredValues(reader);
              
          }
      }

      reader.Read();

      if (result != null)
      {
          WorkbookImpl bookImpl = book as WorkbookImpl;
          FormulaUtil formulaUtil = bookImpl.DataHolder.Parser.FormulaUtil;
          Ptg[] formula = formulaUtil.ParseString(result);
          IRangeGetter rangeHolder = formula[0] as IRangeGetter;
          return rangeHolder.GetRange(book, book.Worksheets[0]);
      }
      
      return null;
    }
    /// <summary>
    /// Extracts trendlines collection.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="series">Series to put extracted data into.</param>
    /// <param name="relations">Chart item relations.</param>
    private void ParseTrendlines( XmlReader reader, ChartSerieImpl series, RelationCollection relations )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( series == null )
        throw new ArgumentNullException( "series" );

      while( reader.LocalName == ChartConstants.TrendlineTag )
      {
        ParseTrendline( reader, series, relations );

        while( reader.NodeType != XmlNodeType.EndElement && reader.NodeType != XmlNodeType.Element )
          reader.Read();
      }
    }
    /// <summary>
    /// Extracts trend line.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="series">Series to put extracted data into.</param>
    /// <param name="relations">Chart item relations.</param>
    private void ParseTrendline( XmlReader reader, ChartSerieImpl series, RelationCollection relations )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( series == null )
        throw new ArgumentNullException( "series" );

      if( reader.LocalName != ChartConstants.TrendlineTag )
        throw new XmlException( "Unexpected xml tag." );

      reader.Read();
      IChartTrendLine trendLine = series.TrendLines.Add();
      FileDataHolder dataHolder = series.ParentBook.DataHolder;

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case ChartConstants.TrendlineNameTag:
              trendLine.Name = reader.ReadElementContentAsString();
              break;

            case Drawings.ShapePropertiesTag:
              ChartFillObjectGetterAny getter = new ChartFillObjectGetterAny(
                trendLine.Border as ChartBorderImpl, null, null,
                trendLine.Shadow as ShadowImpl, trendLine.Chart3DOptions as ThreeDFormatImpl );
              ChartParserCommon.ParseShapeProperties( reader, getter, dataHolder, relations );
              break;

            case ChartConstants.TrendlineTypeTag:
              string strValue = ChartParserCommon.ParseValueTag( reader );
              Excel2007TrendlineType trendlineType = ( Excel2007TrendlineType )Enum.Parse(
                typeof( Excel2007TrendlineType ), strValue, false );
              trendLine.Type = ( ExcelTrendLineType )trendlineType;
              break;

            case ChartConstants.TrendlineOrderTag:
            case ChartConstants.TrendlinePeriodTag:
              trendLine.Order = ChartParserCommon.ParseIntValueTag( reader );
              break;

            case ChartConstants.TrendlineForwardTag:
              trendLine.Forward = ChartParserCommon.ParseDoubleValueTag( reader );
              break;

            case ChartConstants.TrendlineBackwardTag:
              trendLine.Backward = ChartParserCommon.ParseDoubleValueTag( reader );
              break;

            case ChartConstants.TrendlineIntercept:
              trendLine.Intercept = ChartParserCommon.ParseDoubleValueTag( reader );
              break;

            case ChartConstants.DisplayRSquared:
              trendLine.DisplayRSquared = ChartParserCommon.ParseBoolValueTag( reader );
              break;

            case ChartConstants.DisplayEquation:
              trendLine.DisplayEquation = ChartParserCommon.ParseBoolValueTag( reader );
              break;

            case ChartConstants.TrendlineLabelTag:
              ParseTrendlineLabel( reader, trendLine );
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
    /// Extracts trend line label settings.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="trendline">Data label to serialize.</param>
    private void ParseTrendlineLabel( XmlReader reader, IChartTrendLine trendline )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( trendline == null )
        throw new ArgumentNullException( "trendline" );

      if( reader.LocalName != ChartConstants.TrendlineLabelTag )
        throw new XmlException( "Unexpected xml tag." );

      reader.Read();

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          reader.Skip();
          //writer.WriteElementString( "layout", ChartConstants.CNamespace, string.Empty );
          //writer.WriteStartElement( ChartConstants.NumberFormatTag, ChartConstants.CNamespace );
          //writer.WriteAttributeString( ChartConstants.FormatCodeAttribute, "General" );
          //writer.WriteAttributeString( ChartConstants.SourceLinkedAttribute, "0" );
          //writer.WriteEndElement();
        }
        else
        {
          reader.Skip();
        }
      }

      reader.Read();
    }
    /// <summary>
    /// Serializes surface (wall or floor).
    /// </summary>
    /// <param name="reader">XmlWriter to serialize into.</param>
    /// <param name="surface">Surface to serialize.</param>
    /// <param name="dataHolder">Parent data holder.</param>
    /// <param name="relations">Drawing's relations.</param>
    private void ParseSurface( XmlReader reader, IChartWallOrFloor surface,
      FileDataHolder dataHolder,
      RelationCollection relations )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( surface == null )
        throw new ArgumentNullException( "surface" );

      ((ChartWallOrFloorImpl)surface).HasShapeProperties = false;
      if( !reader.IsEmptyElement )
      {
        reader.Read();

        while( reader.NodeType != XmlNodeType.EndElement )
        {
          if( reader.NodeType == XmlNodeType.Element )
          {
            switch( reader.LocalName )
            {
              case Drawings.ShapePropertiesTag:
                ((ChartWallOrFloorImpl)surface).HasShapeProperties = true;
                IChartFillObjectGetter getter = new ChartFillObjectGetterAny(
                  surface.LineProperties as ChartBorderImpl,
                  surface.Interior as ChartInteriorImpl,
                                  surface.Fill as IInternalFill,
                                  surface.Shadow as ShadowImpl,
                                  surface.ThreeD as ThreeDFormatImpl );
                ChartParserCommon.ParseShapeProperties( reader, getter, dataHolder, relations );
                break;
              case Drawings.ThicknessTag:
                string value= ChartParserCommon.ParseValueTag(reader);
                ((ChartWallOrFloorImpl)surface).Thickness = (uint)Int32.Parse(value);
                break;
              case Drawings.PictureoptionTag:
                reader.Read();
                if (reader.LocalName == Drawings.PictureformatTag) ;
                string format = ChartParserCommon.ParseValueTag(reader);
                if (format == ExcelChartPictureType.stack.ToString())
                    ((ChartWallOrFloorImpl)surface).PictureUnit = ExcelChartPictureType.stack;
                reader.Skip();
                break;
              default:
                reader.Skip();
                break;
            }
            // TODO: serialize some properties later.
            //ChartSerializatorCommon.SerializeValueTag( writer, "thickness", "1" );
            //writer.WriteElementString( "spPr", ChartConstants.CNamespace, string.Empty );
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
    /// Extracts plotarea tag from XmlReader.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="chart">Chart to put extracted data into.</param>
    /// <param name="relations">Chart item relations.</param>
    private void ParsePlotArea( XmlReader reader, ChartImpl chart, RelationCollection relations,
      Excel2007Parser parser )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      if( reader.LocalName != ChartConstants.PlotAreaTag )
        throw new XmlException( "Unexpected xml tag" );

      // We are preserving plot area in order to change parsing sequence.
      // On the first stage we have to parse axis and then parse series,
      // since some of them could refer secondary settings and store some
      // data for all series on the axis.
      Stream streamPlotArea = ShapeParser.ReadNodeAsStream( reader );

      streamPlotArea.Position = 0;
      reader = UtilityMethods.CreateReader( streamPlotArea );
      //reader.Read(); // move to the first tag - PlotArea tag.
      reader.Read(); // move inside PlotArea tag.

      ParsePlotAreaAxes( reader, chart, relations, parser );

      streamPlotArea.Position = 0;
      reader = UtilityMethods.CreateReader( streamPlotArea );
      //reader.Read(); // move to the first tag - PlotArea tag.
      reader.Read(); // move inside PlotArea tag.

      ParsePlotAreaGeneral( reader, chart, relations, parser );

      //ChartAxisParser axisParser = new ChartAxisParser();
      //IChartFrameFormat plotArea = chart.PlotArea;
      //FileDataHolder dataHolder = chart.DataHolder.ParentHolder;
      ////plotArea.Interior.UseAutomaticFormat = true;
      ////plotArea.Border.AutoFormat = true;
      //Dictionary<int, int> dictSeriesAxis = new Dictionary<int, int>();
      //int iAxisCount = 0;
      //bool bPrimary;

      //reader.Read();
    }

    private void ParsePlotAreaGeneral( XmlReader reader, ChartImpl chart,
      RelationCollection relations, Excel2007Parser parser )
    {
      IChartFrameFormat plotArea = chart.PlotArea;
      FileDataHolder dataHolder = chart.DataHolder.ParentHolder;
      //plotArea.Interior.UseAutomaticFormat = true;
      //plotArea.Border.AutoFormat = true;
      Dictionary<int, int> dictSeriesAxis = new Dictionary<int, int>();
      bool isCornerBorder = chart.PlotArea != null ? chart.PlotArea.IsBorderCornersRound : false;
      chart.HasPlotArea = false;
      while( reader.NodeType != XmlNodeType.EndElement && reader.NodeType != XmlNodeType.None)
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case ChartConstants.LayoutTag:
              if (chart.PlotArea == null)
                  chart.PlotArea = new ChartPlotAreaImpl(m_book.Application, chart);
              chart.PlotArea.Layout = new ChartLayoutImpl(m_book.Application, chart.PlotArea, chart);
              ChartParserCommon.ParseChartLayout(reader, chart.PlotArea.Layout);
              break;

            case ChartConstants.DataTableTag:
              ParseDataTable( reader, chart );
              break;

            case Drawings.ShapePropertiesTag:
              chart.HasPlotArea = true;
              plotArea = chart.PlotArea;
              chart.PlotArea.IsBorderCornersRound = isCornerBorder;
              ChartFillObjectGetterAny getter = new ChartFillObjectGetterAny(
                plotArea.Border as ChartBorderImpl,
                plotArea.Interior as ChartInteriorImpl,
                              plotArea.Fill as IInternalFill,
                              plotArea.Shadow as ShadowImpl,
                              plotArea.ThreeD as ThreeDFormatImpl);
              ChartParserCommon.ParseShapeProperties( reader, getter, dataHolder, relations );
              break;

            case ChartConstants.BarChartTag:
              ParseBarChart( reader, chart, relations, dictSeriesAxis );
              break;

            case ChartConstants.Bar3DChartTag:
              ParseBar3DChart( reader, chart, relations, dictSeriesAxis );
              break;

            case ChartConstants.AreaChartTag:
              ParseAreaChart( reader, chart, relations, dictSeriesAxis );
              break;

            case ChartConstants.Area3DChartTag:
              ParseArea3DChart( reader, chart, relations, dictSeriesAxis );
              break;

            case ChartConstants.LineChartTag:
              ParseLineChart( reader, chart, relations, dictSeriesAxis, parser );
              break;

            case ChartConstants.Line3DChartTag:
              ParseLine3DChart( reader, chart, relations, dictSeriesAxis, parser );
              break;

            case ChartConstants.BubbleChartTag:
            case ChartConstants.Bubble3DTag:
              ParseBubbleChart( reader, chart, relations, dictSeriesAxis );
              break;

            case ChartConstants.SurfaceChartTag:
              ParseSurfaceChart( reader, chart, relations, dictSeriesAxis );
              break;

            case ChartConstants.Surface3DChartTag:
              ParseSurfaceChart( reader, chart, relations, dictSeriesAxis );
              break;

            case ChartConstants.RadarChartTag:
              ParseRadarChart( reader, chart, relations, dictSeriesAxis, parser );
              break;

            case ChartConstants.ScatterChartTag:
              ParseScatterChart( reader, chart, relations, dictSeriesAxis, parser );
              break;

            case ChartConstants.PieChartTag:
              ParsePieChart( reader, chart, relations, dictSeriesAxis );
              break;

            case ChartConstants.Pie3DChartTag:
              ParsePie3DChart( reader, chart, relations, dictSeriesAxis );
              break;

            case ChartConstants.DoughnutChartTag:
              ParseDoughnutChart( reader, chart, relations, dictSeriesAxis );
              break;

            case ChartConstants.OfPieChartTag:
              ParseOfPieChart( reader, chart, relations, dictSeriesAxis );
              break;

            case ChartConstants.StockChartTag:
              ParseStockChart( reader, chart, relations, dictSeriesAxis, parser );
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

      ChartSeriesCollection arrSeries = ( ChartSeriesCollection )chart.Series;
      
      arrSeries.ResortSeries( dictSeriesAxis );
    }

    private void ParsePlotAreaAxes( XmlReader reader, ChartImpl chart, RelationCollection relations,
      Excel2007Parser parser )
    {
      ChartAxisParser axisParser = new ChartAxisParser(m_book);
      IChartFrameFormat plotArea = chart.PlotArea;
      FileDataHolder dataHolder = chart.DataHolder.ParentHolder;
      //plotArea.Interior.UseAutomaticFormat = true;
      //plotArea.Border.AutoFormat = true;
      Dictionary<int, int> dictSeriesAxis = new Dictionary<int, int>();
      int iAxisCount = 0;
      bool bPrimary;
      ExcelChartType chartType = chart.ChartType;

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case ChartConstants.ValueAxisTag:
              bPrimary = ( iAxisCount <= 1 );
              chart.CreateNecessaryAxes( bPrimary );
              ChartValueAxisImpl valueAxis = ( ChartValueAxisImpl )(
                ( bPrimary ) ?
                chart.PrimaryValueAxis :
                chart.SecondaryValueAxis );

              axisParser.ParseValueAxis( reader, valueAxis, relations, chartType, parser );
              iAxisCount++;
              break;

            case ChartConstants.SeriesAxisTag:
              chart.CreateNecessaryAxes( true );
              ChartSeriesAxisImpl seriesAxis = ( ChartSeriesAxisImpl )chart.PrimarySerieAxis;

              if( seriesAxis == null )
                seriesAxis = chart.CreatePrimarySeriesAxis();

              axisParser.ParseSeriesAxis( reader, seriesAxis, relations, chartType, parser );
              //iAxisCount++;
              break;

            case ChartConstants.CategoryAxisTag:
              bPrimary = ( iAxisCount <= 1 );
              chart.CreateNecessaryAxes( bPrimary );
              ChartCategoryAxisImpl categoryAxis = ( ChartCategoryAxisImpl )(
                ( iAxisCount <= 1 ) ?
                chart.PrimaryCategoryAxis :
                chart.SecondaryCategoryAxis );

              axisParser.ParseCategoryAxis( reader, ( ChartCategoryAxisImpl )categoryAxis, relations, chartType, parser );
              iAxisCount++;
              break;

            case ChartConstants.DateAxisTag:
              bPrimary = ( iAxisCount <= 1 );
              chart.CreateNecessaryAxes( bPrimary );
              ChartCategoryAxisImpl category = ( ChartCategoryAxisImpl )( ( bPrimary ) ?
                chart.PrimaryCategoryAxis : 
                chart.SecondaryCategoryAxis );

              axisParser.ParseDateAxis( reader, category, relations, chartType, parser );
              iAxisCount++;
              break;

            case ChartConstants.BubbleChartTag:
              chartType = ExcelChartType.Bubble;
              reader.Skip();
              break;

            case ChartConstants.ScatterChartTag:
              chartType = ExcelChartType.Scatter_Markers;
              reader.Skip();
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
    /// <summary>
    /// Parses bar chart.
    /// </summary>
    /// <param name="reader">XmlReader to extract chart from.</param>
    /// <param name="chart">Chart to put extracted data into.</param>
    /// <param name="relations">Chart item relations.</param>
    /// <param name="dictSeriesAxis">Dictionary with axis id, key - series index, value - axis index (category or value).</param>
    private void ParseBarChart( XmlReader reader, ChartImpl chart,
      RelationCollection relations, Dictionary<int, int> dictSeriesAxis )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      if( reader.LocalName != ChartConstants.BarChartTag )
        throw new XmlException( "Unexpected xml tag." );

      reader.Read();

      //writer.WriteStartElement( ChartConstants.BarChartTag, ChartConstants.CNamespace );
      List<ChartSerieImpl> lstSeries = new List<ChartSerieImpl>();
      string shape = null;
      IChartSerie firstSeries = ParseBarChartShared( reader, chart, relations, false, lstSeries,out shape );

      int? gapWidth = null;
      int? overlap = null;
      bool secaxisid=false;
      bool primaryaxisid = false;
      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element)
        {
          switch( reader.LocalName )
          {
            case ChartConstants.GapWidthTag:
              string strGapWidth = ChartParserCommon.ParseValueTag( reader );
              gapWidth = int.Parse( strGapWidth );
              if (firstSeries != null)
              {
                  (firstSeries as ChartSerieImpl).GapWidth = (int)gapWidth;
                  (firstSeries as ChartSerieImpl).ShowGapWidth = true;
              }
              break;

            case ChartConstants.OverlapTag:
              string strOverlap = ChartParserCommon.ParseValueTag( reader );
              overlap = int.Parse( strOverlap );
              if (firstSeries != null)
                  (firstSeries as ChartSerieImpl).Overlap = (int) overlap;
              chart.OverLap = int.Parse(strOverlap);
              if( overlap == 100 )
                overlap = ChartFormatImpl.DEF_BAR_STACKED;
              break;

            case ChartConstants.AxisIdTag:
             secaxisid= ParseAxisId( reader, lstSeries, dictSeriesAxis );
             if (secaxisid)
                 primaryaxisid = true;
              break;
            case Excel2007Serializator.Extensionlist:
              IChartSerie serie = ParseFilteredSeries(reader, chart, relations, true,firstSeries.SerieType,primaryaxisid);                            
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

      // If firstSeries is null then it means that we have no series at all - empty chart.
      // In this case we have simply skip all tags.
      IChartFormat commonOptions = ( firstSeries != null ) ?
        firstSeries.SerieFormat.CommonSerieOptions :
        null;

      if (gapWidth != null && firstSeries != null)
        commonOptions.GapWidth = ( int )gapWidth;

      if (overlap != null && firstSeries != null)
          commonOptions.Overlap = (int)overlap;

      reader.Read();
    }
    /// <summary>
    /// Parse the Filtered Series
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="chart"></param>
    /// <param name="relations"></param>
    /// <param name="is3D"></param>
    /// <param name="SeriesType"></param>
    /// <param name="secondary"></param>
    /// <returns></returns>
    private IChartSerie ParseFilteredSeries(XmlReader reader, ChartImpl chart,
    RelationCollection relations, bool is3D, ExcelChartType SeriesType,bool secondary)
    {
        WorksheetDataHolder sheetHolder = chart.DataHolder;
        FileDataHolder holder = sheetHolder.ParentHolder;
        if (reader == null)
            throw new ArgumentNullException("reader");
        string shape = string.Empty;
        if (chart == null)
            throw new ArgumentNullException("chart");
        string nn = string.Empty;
        if (reader.LocalName != Excel2007Serializator.Extensionlist)
            throw new XmlException("Unexpected xml tag.");
        reader.Read();
        ChartSerieImpl serie = null;
        if (reader.LocalName == Excel2007Serializator.Extension)
        {
            reader.Read();

            while (reader.LocalName != Excel2007Serializator.Extensionlist && reader.NodeType != XmlNodeType.EndElement)
            {
                reader.Read();
                if (reader.LocalName == ChartConstants.SeriesTag)
                {
                    switch (SeriesType)
                    {
                        case ExcelChartType.Area:
                        case ExcelChartType.Area_3D:
                        case ExcelChartType.Area_Stacked:
                        case ExcelChartType.Area_Stacked_100:
                        case ExcelChartType.Area_Stacked_100_3D:
                        case ExcelChartType.Area_Stacked_3D:
                            serie = ParseAreaSeries(reader, chart, SeriesType, relations, !secondary);
                            break;
                        case ExcelChartType.Line:
                        case ExcelChartType.Line_3D:
                        case ExcelChartType.Line_Markers:
                        case ExcelChartType.Line_Markers_Stacked:
                        case ExcelChartType.Line_Markers_Stacked_100:
                        case ExcelChartType.Line_Stacked:
                        case ExcelChartType.Line_Stacked_100:
                            serie=ParseLineSeries(reader,chart,SeriesType,relations,holder.Parser);
                            break;
                        case ExcelChartType.Pie:
                        case ExcelChartType.Pie_3D:
                        case ExcelChartType.Pie_Bar:
                        case ExcelChartType.Pie_Exploded:
                        case ExcelChartType.Pie_Exploded_3D:
                        case ExcelChartType.PieOfPie:
                            serie = ParsePieSeries(reader, chart, SeriesType, relations);
                            break;
                        case ExcelChartType.Radar:
                        case ExcelChartType.Radar_Filled:
                        case ExcelChartType.Radar_Markers:
                            serie = ParseRadarSeries(reader, chart, SeriesType, relations, holder.Parser);
                            break;
                        case ExcelChartType.Scatter_Line:
                        case ExcelChartType.Scatter_Line_Markers:
                        case ExcelChartType.Scatter_Markers:
                        case ExcelChartType.Scatter_SmoothedLine:
                        case ExcelChartType.Scatter_SmoothedLine_Markers:
                            serie = ParseScatterSeries(reader, chart, SeriesType, relations, holder.Parser);
                            break;
                        case ExcelChartType.Surface_3D:
                        case ExcelChartType.Surface_Contour:
                        case ExcelChartType.Surface_NoColor_3D:
                        case ExcelChartType.Surface_NoColor_Contour:
                            serie = ParseSurfaceSeries(reader, chart, SeriesType, relations);
                            break;
                        case ExcelChartType.Bubble:
                        case ExcelChartType.Bubble_3D:
                            serie = ParseBubbleSeries(reader, chart, relations);
                            break;
                        default:
                            serie = ParseBarSeries(reader, chart, SeriesType, relations);
                            break;
                    }
                    
                }
                if (secondary)
                serie.UsePrimaryAxis = !secondary;

                serie.IsFiltered = true;

                reader.Skip();
                //reader.Skip();
                //reader.Read();
            }
            reader.Read();
            reader.Read();
        }
        return serie;

    }
    /// <summary>
    /// Stores extracted axis id inside structures for future use.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="lstSeries">Series list.</param>
    /// <param name="dictSeriesAxis">Dictionary with axis id, key - series index, value - axis index (category or value).</param>
    private bool ParseAxisId( XmlReader reader, List<ChartSerieImpl> lstSeries, Dictionary<int, int> dictSeriesAxis )
    {
      int iAxisId = ChartParserCommon.ParseIntValueTag( reader );
      int iCount = lstSeries.Count;
      bool bSecondaryAxis = false;

      if( iCount > 0 )
      {
        ChartSerieImpl firstSerie = lstSeries[ 0 ];
        ChartImpl chart = firstSerie.ParentChart;
        int iValueId = ( chart.PrimaryValueAxis as ChartAxisImpl ).AxisId;
        int iCategoryId = ( chart.PrimaryCategoryAxis as ChartAxisImpl ).AxisId;
        bSecondaryAxis = iAxisId != iValueId && iAxisId != iCategoryId;

        for( int i = 0; i < iCount; i++ )
        {
          ChartSerieImpl series = lstSeries[ i ];
          int index = series.Index;
          dictSeriesAxis[ index ] = iAxisId;

          //if( bSecondaryAxis )
          //  series.UsePrimaryAxis = false;
        }
      }

      lstSeries.Clear();
      return bSecondaryAxis;
    }
    /// <summary>
    /// Parses bar 3D chart.
    /// </summary>
    /// <param name="reader">XmlReader to extract chart from.</param>
    /// <param name="chart">Chart to put extracted data into.</param>
    /// <param name="relations">Chart item relations.</param>
    /// <param name="dictSeriesAxis">Dictionary with axis id, key - series index, value - axis index (category or value).</param>
    public void ParseBar3DChart( XmlReader reader, ChartImpl chart,
      RelationCollection relations, Dictionary<int, int> dictSeriesAxis )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      if( reader.LocalName != ChartConstants.Bar3DChartTag )
        throw new XmlException( "Unexpected xml tag." );

      reader.Read();

      //writer.WriteStartElement( ChartConstants.BarChartTag, ChartConstants.CNamespace );

      List<ChartSerieImpl> lstSeries = new List<ChartSerieImpl>();
      string shape = null;
      ChartSerieImpl firstSeries = ParseBarChartShared( reader, chart, relations, true, lstSeries,out shape );
      int? gapWidth = null;

      if(shape!=null)
      ParseBarShape(shape, firstSeries);

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element && firstSeries != null )
        {
          switch( reader.LocalName )
          {
            case ChartConstants.GapWidthTag:
              string strGapWidth = ChartParserCommon.ParseValueTag( reader );
              gapWidth = int.Parse( strGapWidth );
              chart.GapWidth = (int) gapWidth;
              chart.ShowGapWidth = true;
              break;

            case ChartConstants.GapDepthTag:
              chart.GapDepth = ChartParserCommon.ParseIntValueTag( reader );
              break;

            case ChartConstants.BarShapeTag:
              ParseBarShape( reader, firstSeries );
              break;

            case ChartConstants.AxisIdTag:
              ParseAxisId( reader, lstSeries, dictSeriesAxis );
              break;

            case Excel2007Serializator.Extensionlist:

              IChartSerie serie = ParseFilteredSeries(reader, chart, relations, true, firstSeries.SerieType,false);
              break;

            //SerializeBarAxisId( writer, chart, firstSeries );

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

      if( gapWidth != null )
      {
        IChartFormat commonOptions = firstSeries.SerieFormat.CommonSerieOptions;
        commonOptions.GapWidth = ( int )gapWidth;
      }

      reader.Read();
    }
    ///// <summary>
    ///// Serializes axis id's for bar chart.
    ///// </summary>
    ///// <param name="writer">XmlWriter to serialize into.</param>
    ///// <param name="chart"></param>
    ///// <param name="firstSeries">First series in the list of series with the
    ///// same formatting to serialize.</param>
    //private void SerializeBarAxisId( XmlWriter writer, ChartImpl chart, ChartSerieImpl firstSeries )
    //{
    //  if( writer == null )
    //    throw new ArgumentNullException( "writer" );

    //  if( chart == null )
    //    throw new ArgumentNullException( "chart" );

    //  if( firstSeries == null )
    //    throw new ArgumentNullException( "firstSeries" );

    //  bool bPrimary = firstSeries.UsePrimaryAxis;
    //  ChartAxisImpl axis = ( ChartAxisImpl )( bPrimary ?
    //    chart.PrimaryCategoryAxis :
    //    chart.SecondaryCategoryAxis );

    //  if( axis == null )
    //    throw new ArgumentNullException( "axis" );

    //  ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.AxisIdTag, axis.AxisId.ToString() );

    //  axis = ( ChartAxisImpl )( bPrimary ?
    //    chart.PrimaryValueAxis :
    //    chart.SecondaryValueAxis );

    //  ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.AxisIdTag, axis.AxisId.ToString() );

    //  if( chart.IsSeriesAxisAvail )
    //  {
    //    axis = ( ChartAxisImpl )chart.PrimarySerieAxis;
    //    ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.AxisIdTag, axis.AxisId.ToString() );
    //  }
    //}

    /// <summary>
    /// Extracts shape of the bar chart.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="firstSeries">First series in the list of series with the
    /// same formatting.</param>
    private void ParseBarShape( XmlReader reader, ChartSerieImpl firstSeries )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( firstSeries == null )
        throw new ArgumentNullException( "firstSeries" );


      
      string strValue = ChartParserCommon.ParseValueTag( reader );

      ParseBarShape(strValue, firstSeries);

      ////ChartSerieImpl series = ( ChartSerieImpl )firstSeries;
      //ChartSeriesCollection arrSeries = firstSeries.ParentSeries;

      //for( int i = firstSeries.Index, len = arrSeries.Count; i < len; i++ )
      //{
      //  ChartSerieImpl currentSeries = ( ChartSerieImpl )arrSeries[ i ];

      //  if( currentSeries.ChartGroup == firstSeries.ChartGroup )
      //  {
      //    ( ( ChartSerieDataFormatImpl )currentSeries.SerieFormat ).UpdateBarFormat( true );
      //    ( ( ChartSerieDataFormatImpl )currentSeries.SerieFormat ).UpdateBarFormat( false );
      //    currentSeries.DetectSerieType();
      //  }
      //}
    }
    /// <summary>
    /// Extracts shape of the bar chart.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="firstSeries">First series in the list of series with the
    /// same formatting.</param>
    private void ParseBarShape(string value, ChartSerieImpl firstSeries)
    {
        if (value == null)
            return;

        if (firstSeries == null)
            throw new ArgumentNullException("firstSeries");


        ChartFormatImpl format = firstSeries.GetCommonSerieFormat();
        IChartSerieDataFormat dataFormat = firstSeries.SerieFormat;        

        switch (value)
        {
            case ChartConstants.BarShapeCone:
                dataFormat.BarShapeTop = ExcelTopFormat.Sharp;
                dataFormat.BarShapeBase = ExcelBaseFormat.Circle;
                break;

            case ChartConstants.BarShapePyramid:
                dataFormat.BarShapeTop = ExcelTopFormat.Sharp;
                dataFormat.BarShapeBase = ExcelBaseFormat.Rectangle;
                break;

            case ChartConstants.BarShapeConeToMax:
                dataFormat.BarShapeTop = ExcelTopFormat.Trunc;
                dataFormat.BarShapeBase = ExcelBaseFormat.Circle;
                break;

            case ChartConstants.BarShapePyramidToMax:
                dataFormat.BarShapeTop = ExcelTopFormat.Trunc;
                dataFormat.BarShapeBase = ExcelBaseFormat.Rectangle;
                break;

            case ChartConstants.BarShapeCylinder:
                dataFormat.BarShapeTop = ExcelTopFormat.Straight;
                dataFormat.BarShapeBase = ExcelBaseFormat.Circle;
                //chartType = firstSeries.SerieType;
                break;

            case ChartConstants.BarShapeBox:
                dataFormat.BarShapeTop = ExcelTopFormat.Straight;
                dataFormat.BarShapeBase = ExcelBaseFormat.Rectangle;
                //chartType = firstSeries.SerieType;
                break;

            default:
                throw new XmlException();
        }

        ////ChartSerieImpl series = ( ChartSerieImpl )firstSeries;
        //ChartSeriesCollection arrSeries = firstSeries.ParentSeries;

        //for( int i = firstSeries.Index, len = arrSeries.Count; i < len; i++ )
        //{
        //  ChartSerieImpl currentSeries = ( ChartSerieImpl )arrSeries[ i ];

        //  if( currentSeries.ChartGroup == firstSeries.ChartGroup )
        //  {
        //    ( ( ChartSerieDataFormatImpl )currentSeries.SerieFormat ).UpdateBarFormat( true );
        //    ( ( ChartSerieDataFormatImpl )currentSeries.SerieFormat ).UpdateBarFormat( false );
        //    currentSeries.DetectSerieType();
        //  }
        //}
    }
    /// <summary>
    /// Extracts part of the bar chart that is common for all bar charts.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="chart">Chart to put extracted data into.</param>
    /// <param name="relations">Chart item relations.</param>
    /// <param name="is3D">Indicates whether we are parsing 3D chart.</param>
    /// <param name="lstSeries">List that will get extracted series.</param>
    /// <returns>First extracted series.</returns>
    private ChartSerieImpl ParseBarChartShared( XmlReader reader, ChartImpl chart,
      RelationCollection relations, bool is3D, List<ChartSerieImpl> lstSeries,out string shape )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      bool bContinue = true;
      ChartSerieImpl series = null;
      string strDirection = null;
      bool bVaryColors = true;
      string strGrouping = null;
      shape = null;
      MemoryStream seriesData = new MemoryStream();
      XmlWriter writer = UtilityMethods.CreateWriter( seriesData, Encoding.UTF8 );
      writer.WriteStartElement( "root" );
      ChartSerieImpl filteredseries = null;
      while( bContinue )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case ChartConstants.BarDirectionTag:
              strDirection = ChartParserCommon.ParseValueTag( reader );
              break;

            case ChartConstants.BarGroupingTag:
              strGrouping = ChartParserCommon.ParseValueTag( reader );
              break;

            case ChartConstants.VaryColorsTag:
              bVaryColors = ChartParserCommon.ParseBoolValueTag( reader );
              break;

            case ChartConstants.BarShapeTag:
              shape = ChartParserCommon.ParseValueTag( reader );
              break;

            case ChartConstants.SeriesTag:
              writer.WriteNode( reader, false );
              //ExcelChartType seriesType = GetBarSeriesType( strDirection, strGrouping, is3D, null );
              //ChartSerieImpl currentSeries = ParseBarSeries( reader, chart, seriesType, relations );

              //if( series == null )
              //  series = currentSeries;

              //lstSeries.Add( currentSeries );
              break;

            case ChartConstants.DataLabelsTag:
              //ParseDataLabels( reader, chart.Series[ 0 ] );
              //break;
              reader.Skip();
              break;

            default:
              if (seriesData.Length == 0 && !chart.HasPivotSource)
              {

                  ExcelChartType seriesType = GetPivotBarSeriesType(strDirection, strGrouping, shape, is3D);
                  filteredseries = ParseFilterSecondaryAxis(reader, seriesType, is3D, lstSeries, chart, relations, ref series);

              }
              bContinue = false;
                  break;
          }
        }
        else
        {
          reader.Skip();
        }
      }

      writer.WriteEndElement();
      writer.Flush();

      seriesData.Position = 0;
      XmlReader seriesReader = UtilityMethods.CreateReader( seriesData );

      if( !seriesReader.IsEmptyElement )
      {
        seriesReader.Read();
        ParseSeries( seriesReader, strDirection, strGrouping, shape, is3D, lstSeries, chart, relations, ref series );
        if (series != null)
            series.Grouping = strGrouping;
      }

#if ( WINRT )
      seriesReader.Dispose();
      writer.Dispose();
#else
        seriesReader.Close();
      writer.Close();
#endif

      if (chart.HasPivotSource)
          chart.PivotChartType = GetPivotBarSeriesType(strDirection, strGrouping, shape, is3D);

      return series;
    }
    /// <summary>
    /// Parse Secondary Axis Filter
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="seriesType"></param>
    /// <param name="is3D"></param>
    /// <param name="lstSeries"></param>
    /// <param name="chart"></param>
    /// <param name="relations"></param>
    /// <param name="series"></param>
    /// <returns></returns>
    private ChartSerieImpl ParseFilterSecondaryAxis(XmlReader reader, ExcelChartType seriesType, bool is3D,
      List<ChartSerieImpl> lstSeries, ChartImpl chart, RelationCollection relations, ref ChartSerieImpl series)
    {
        ChartSerieImpl secondaryfilter = null;
        int? gapWidth = null;
        bool secaxisid = false;
        bool primaryaxisid = false;
        Dictionary<int, int> dictseries = new Dictionary<int, int>();
        IChartSerie serie=null;     
                      
        while (reader.NodeType != XmlNodeType.EndElement)
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                switch (reader.LocalName)
                {
                    case ChartConstants.GapWidthTag:
                        string strGapWidth = ChartParserCommon.ParseValueTag(reader);
                        gapWidth = int.Parse(strGapWidth);
                        chart.GapWidth = (int) gapWidth;
                        chart.ShowGapWidth = true;
                        break;

                    case ChartConstants.OverlapTag:
                        string strOverlap = ChartParserCommon.ParseValueTag(reader);                        
                        chart.OverLap = int.Parse(strOverlap);                         
                        break;

                    case ChartConstants.AxisIdTag:
                        //secaxisid = ParseAxisId(reader, lstSeries, dictseries);
                        //if (secaxisid)
                        //    primaryaxisid = true;
                        reader.Skip();
                        break;
                    case Excel2007Serializator.Extensionlist:
                         serie= ParseFilteredSeries(reader, chart, relations, true, seriesType,true);                        
                            serie.UsePrimaryAxis = false;
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }
        }

        series = serie as ChartSerieImpl;

        return secondaryfilter;
    }

    private void ParseSeries( XmlReader reader, string strDirection, string strGrouping, string shape, bool is3D,
      List<ChartSerieImpl> lstSeries, ChartImpl chart, RelationCollection relations, ref ChartSerieImpl series )
    {
      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element && reader.LocalName == ChartConstants.SeriesTag )
        {
          ExcelChartType seriesType = GetPivotBarSeriesType( strDirection, strGrouping, shape, is3D );//GetBarSeriesType( strDirection, strGrouping, is3D, null );
          ChartSerieImpl currentSeries = ParseBarSeries( reader, chart, seriesType, relations );

          if( series == null )
            series = currentSeries;

          lstSeries.Add( currentSeries );
        }
        else
        {
          reader.Read();
        }
      }
      
      
    }
    /// <summary>
    /// When we parsing the categoryfiltered chart, we need to find the filtered category
    /// </summary>
    /// <param name="categories"></param>
    /// <param name="filteredcategory"></param>
    /// <param name="fullreference"></param>
    /// <param name="series1"></param>
    /// <param name="isseries"></param>
    private void FindFilter(IChartCategories categories, string filteredcategory,string fullreference,IChartSerie series1,bool isseries)
    {
        string strValue = fullreference;
        IRange chartvalues = FindRange(series1,fullreference);
        int start = isseries ? chartvalues.Column : chartvalues.Row;
        int end = isseries ? chartvalues.LastColumn : chartvalues.LastRow;
       filteredcategory= filteredcategory.Trim('(');
       filteredcategory= filteredcategory.Trim(')');
        string[] unfiltered = filteredcategory.Split(',');
        int[] categories_length = new int[chartvalues.Count];
        int k = 0;
        for (int i = 0; i < unfiltered.Length; i++)
        {
            IRange check = FindRange(series1, unfiltered[i]);
            int start1 = isseries ? check.Column : check.Row;
            int end1 = isseries ? check.LastColumn : check.LastRow;
            for (int j = start1; j <= end1; j++)
            {
                categories_length[k] = j;
                k++;
            }
        }
        k = 0;
        int kk = 0;
        for (int m = start; m <= end; m++)
        {
            if (m != categories_length[k] && categories_length[k]!=0)
            {
                categories[kk].IsFiltered = true;               
            }
            if (m == categories_length[k]&& categories_length[k]!=0)
            {
                k++;
            }
            else
            {
                categories[kk].IsFiltered = true; 
            }
            kk++;
        }
      
    }
    /// <summary>
    /// Supporting method for Find Filter
    /// </summary>
    /// <param name="series1"></param>
    /// <param name="strValue"></param>
    /// <returns></returns>
    private IRange FindRange(IChartSerie series1, string strValue)
    {
        IRange result = null;
        bool numRef = true;
        bool strRef = false;
        bool mulRef = false;
        ChartSerieImpl series = series1 as ChartSerieImpl;
        IWorkbook book = series.ParentBook;
        if (strValue != null)
        {

            WorkbookImpl bookImpl = series.ParentBook as WorkbookImpl;
            FormulaUtil formulaUtil = bookImpl.DataHolder.Parser.FormulaUtil;
            Ptg[] formula = formulaUtil.ParseString(strValue);
            IRangeGetter rangeHolder = formula[0] as IRangeGetter;
            result = rangeHolder.GetRange(bookImpl, bookImpl.Worksheets[0]);
            if (result != null)
            {
                if (result is ExternalRange)
                {
                    (result as ExternalRange).IsNumReference = numRef;
                    (result as ExternalRange).IsStringReference = strRef;
                    (result as ExternalRange).IsMultiReference = mulRef;
                }
                else if (result is RangeImpl)
                {
                    (result as RangeImpl).IsNumReference = numRef;
                    (result as RangeImpl).IsStringReference = strRef;
                    (result as RangeImpl).IsMultiReference = mulRef;
                }
                else if (result is NameImpl)
                {
                    (result as NameImpl).IsNumReference = numRef;
                    (result as NameImpl).IsStringReference = strRef;
                    (result as NameImpl).IsMultiReference = mulRef;
                }
            }
         }
        return result;
    }
    /// <summary>
    /// Find the series or category Range
    /// </summary>
    /// <param name="range"></param>
    /// <param name="bIsInRow"></param>
    /// <param name="serieRange"></param>
    /// <returns></returns>
    public IRange GetSerieOrAxisRange(IRange range, bool bIsInRow, out IRange serieRange)
    {
        if (range == null)
            throw new ArgumentNullException("range");

        int iFirstLen = bIsInRow ? range.Row : range.Column;
        int iRowColumn = bIsInRow ? range.LastRow : range.LastColumn;

        int iFirsCount = bIsInRow ? range.Column : range.Row;
        int iLastCount = bIsInRow ? range.LastColumn : range.LastRow;

        int iIndex = -1;

        bool bIsName = false;

        for (int i = iFirsCount; i < iLastCount && !bIsName; i++)
        {
            IRange curRange = bIsInRow ? range[iRowColumn, i] : range[i, iRowColumn];

            bIsName = curRange.HasNumber || curRange.IsBlank || curRange.HasFormula;

            if (!bIsName)
                iIndex = i;
        }

        if (iIndex == -1)
        {
            serieRange = range;
            return null;
        }

        IRange result = (bIsInRow)
          ? range[iFirstLen, iFirsCount, iRowColumn, iIndex]
          : range[iFirsCount, iFirstLen, iIndex, iRowColumn];

        serieRange = (bIsInRow)
          ? range[range.Row, result.LastColumn + 1, range.LastRow, range.LastColumn]
          : range[result.LastRow + 1, range.Column, range.LastRow, range.LastColumn];

        return result;
    }
    /// <summary>
    /// Find series in Row or Column.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    private bool DetectIsInRow(IRange range)
    {
        if (range == null)
            return true;

        int iRowCount = range.LastRow - range.Row;
        int iColCount = range.LastColumn - range.Column;

        return iRowCount <= iColCount;
    }
    /// <summary>
    /// Converts direction and grouping values into type of bar series.
    /// </summary>
    /// <param name="direction">Represents direction of bar series.</param>
    /// <param name="grouping">Represents series grouping value.</param>
    /// <param name="is3D">Indicates whether we are parsing 3D chart.</param>
    /// <returns>Extracted chat type.</returns>
    private ExcelChartType GetBarSeriesType( string direction, string grouping, bool is3D, string shape )
    {
      // direction - bar or column
      // grouping - ChartConstants.Clustered, ChartConstants.PercentStacked (100%),
      // ChartConstants.Stacked, ChartConstants.Standard
      string strChartTypeName = null;

      if( !is3D )
      {
        strChartTypeName = ( direction == ChartConstants.BarDirectionBar ) ?
          ChartImpl.START_BAR :
          ChartImpl.START_COLUMN;
      }
      else
      {
        strChartTypeName = ( shape == "box" ) ? ChartImpl.START_COLUMN : ChartImpl.START_CONE;

        if( direction == ChartConstants.BarDirectionBar )
          strChartTypeName += '_' + ChartImpl.START_BAR;
      }

      switch( grouping )
      {
        case ChartConstants.Clustered:
          strChartTypeName += "_Clustered";
          break;

        case ChartConstants.PercentStacked:
          strChartTypeName += "_Stacked_100";
          break;

        case ChartConstants.Stacked:
          strChartTypeName += "_Stacked";
          break;

        default:
          if( is3D )
          {
            strChartTypeName += "_Clustered_3D";
          }
          else
          {
            strChartTypeName += "_Clustered";
          }
          break;
      }

      ExcelChartType result = ( ExcelChartType )Enum.Parse( typeof( ExcelChartType ), strChartTypeName, false );
      return result;
    }
    /// <summary>
        /// Converts direction and grouping values into type of bar series.
        /// </summary>
        /// <param name="direction">Represents direction of bar series.</param>
        /// <param name="grouping">Represents series grouping value.</param>
        /// <param name="is3D">Indicates whether we are parsing 3D chart.</param>
        /// <returns>Extracted chat type.</returns>
        private ExcelChartType GetPivotBarSeriesType(string direction, string grouping, string shape, bool is3D)
        {
            // direction - bar or column
            // grouping - ChartConstants.Clustered, ChartConstants.PercentStacked (100%),
            // ChartConstants.Stacked, ChartConstants.Standard
            string strChartTypeName = null;
            string[] shapeTypes = new string[]
            {
                ChartImpl.START_CONE,
                ChartImpl.START_CYLINDER,
                ChartImpl.START_PYRAMID
            };
            if (shape != null)
            {
                if (Array.IndexOf(shapeTypes, shape) == -1)
                    strChartTypeName = (direction == ChartConstants.BarDirectionBar) ?
                                            ChartImpl.START_BAR
                                            : ChartImpl.START_COLUMN;
            }
            else
            {
                strChartTypeName = (direction == ChartConstants.BarDirectionBar) ?
                                                 shape == null ?
                                                 ChartImpl.START_BAR
                                                 : shape + "_" + ChartImpl.START_BAR
                                                 : (direction == ChartConstants.BarDirectionColumn
                                                && shape != null) ?
                                                         shape :
                                                         ChartImpl.START_COLUMN;

            }

            switch (grouping)
            {
                case ChartConstants.Clustered:
                    if (is3D && (shape == null||Array.IndexOf(shapeTypes,shape)==-1))
                        strChartTypeName += "_Clustered_3D";
                    else
                        strChartTypeName += "_Clustered";
                    break;

                case ChartConstants.Standard:
                    if (is3D && (shape == null || Array.IndexOf(shapeTypes, shape) == -1))
                        strChartTypeName += "_3D";
                    else
                        strChartTypeName += "_Clustered_3D";
                    break;

                case ChartConstants.PercentStacked:
                    if (is3D && (shape == null || Array.IndexOf(shapeTypes, shape) == -1))
                        strChartTypeName += "_Stacked_100_3D";
                    else
                        strChartTypeName += "_Stacked_100";
                    break;

                case ChartConstants.Stacked:
                    if (is3D && (shape == null || Array.IndexOf(shapeTypes, shape) == -1))
                        strChartTypeName += "_Stacked_3D";
                    else
                        strChartTypeName += "_Stacked";
                    break;

                default:
                    if (is3D)
                    {
                        strChartTypeName += "_Clustered_3D";
                    }
                    else
                    {
                        strChartTypeName += "_Clustered";
                    }
                    break;
            }

            ExcelChartType result = (ExcelChartType)Enum.Parse(typeof(ExcelChartType), strChartTypeName, true);
            return result;
        }
        /// <summary>
    /// Converts grouping value into type of area series.
    /// </summary>
    /// <param name="grouping">Area series grouping value.</param>
    /// <param name="is3D">Indicates whether we are parsing 3D chart.</param>
    /// <returns>Area chart type that corresponds to grouping and is3D values.</returns>
    private ExcelChartType GetAreaSeriesType( string grouping, bool is3D )
    {
      // grouping - ChartConstants.Clustered, ChartConstants.PercentStacked (100%),
      // ChartConstants.Stacked, ChartConstants.Standard
      string strChartTypeName = ChartImpl.START_AREA;

      switch( grouping )
      {
        case ChartConstants.Clustered:
        default:
          //if( is3D )
          //  strChartTypeName += "_Clustered";
          break;

        case ChartConstants.PercentStacked:
          strChartTypeName += "_Stacked_100";
          break;

        case ChartConstants.Stacked:
          strChartTypeName += "_Stacked";
          break;
      }

      if( is3D )
        strChartTypeName += ChartImpl.PREFIX_3D;

      ExcelChartType result = ( ExcelChartType )Enum.Parse( typeof( ExcelChartType ), strChartTypeName, false );
      return result;
    }
    /// <summary>
    /// Converts grouping value into type of line series.
    /// </summary>
    /// <param name="grouping">Line series grouping value.</param>
    /// <param name="is3D">Indicates whether we are parsing 3D chart.</param>
    /// <returns>Line chart type that corresponds to grouping and is3D values.</returns>
    private ExcelChartType GetLineSeriesType( string grouping, bool is3D )
    {
      // grouping - ChartConstants.Clustered, ChartConstants.PercentStacked (100%),
      // ChartConstants.Stacked, ChartConstants.Standard
      string strChartTypeName = ChartImpl.START_LINE;
      ExcelChartType result;

      if( !is3D )
      {
        //if( bMarkers )
        //  strChartTypeName += "_Markers";

        switch( grouping )
        {
          //case ChartConstants.Clustered:
          //default:
          //  if( b3D )
          //    strChartTypeName += "_Clustered";
          //  break;

          case ChartConstants.PercentStacked:
            strChartTypeName += "_Stacked_100";
            break;

          case ChartConstants.Stacked:
            strChartTypeName += "_Stacked";
            break;
        }

        result = ( ExcelChartType )Enum.Parse( typeof( ExcelChartType ), strChartTypeName, false );
      }
      else
      {
        result = ExcelChartType.Line_3D;
      }
      return result;
    }
    ///// <summary>
    ///// Serializes chart series with the same formatting.
    ///// </summary>
    ///// <param name="writer">XmlWriter to serialize into.</param>
    ///// <param name="chart">Chart to get series from.</param>
    ///// <param name="firstSeries">First series in the list of the series to serialize.
    ///// It is used to determine which series should be serialized.</param>
    ///// <param name="serializator">Delegate used to serialize chart series.</param>
    ///// <returns>Number of serialized series.</returns>
    //private int SerializeChartSeries( XmlWriter writer, ChartImpl chart,
    //  ChartSerieImpl firstSeries, SerializeSeriesDelegate serializator )
    //{
    //  if( writer == null )
    //    throw new ArgumentNullException( "writer" );

    //  if( chart == null )
    //    throw new ArgumentNullException( "chart" );

    //  if( firstSeries == null )
    //    throw new ArgumentNullException( "firstSeries" );

    //  if( serializator == null )
    //    throw new ArgumentNullException( "serializator" );

    //  int iChartGroup = firstSeries.ChartGroup;
    //  serializator( writer, firstSeries );
    //  int iSeriesCount = 1;
    //  IChartSeries arrSeries = chart.Series;

    //  for( int i = firstSeries.Index + 1, len = arrSeries.Count; i < len; i++ )
    //  {
    //    ChartSerieImpl series = ( ChartSerieImpl )arrSeries[ i ];

    //    if( series.ChartGroup == iChartGroup )
    //    {
    //      serializator( writer, series );
    //      iSeriesCount++;
    //    }
    //  }

    //  return iSeriesCount;
    //}
    ///// <summary>
    ///// Serializes grouping tag.
    ///// </summary>
    ///// <param name="writer">XmlWriter to serialize into.</param>
    ///// <param name="chart">Chart to serialize grouping for.</param>
    //private void SerializeChartGrouping( XmlWriter writer, ChartImpl chart )
    //{
    //  if( writer == null )
    //    throw new ArgumentNullException( "writer" );

    //  if( chart == null )
    //    throw new ArgumentNullException( "chart" );

    //  string strGrouping;

    //  if( chart.IsClustered )
    //  {
    //    strGrouping = ChartConstants.Clustered;
    //  }
    //  else if( chart.IsChart_100 )
    //  {
    //    strGrouping = ChartConstants.PercentStacked;
    //  }
    //  else if( chart.IsStacked )
    //  {
    //    strGrouping = ChartConstants.Stacked;
    //  }
    //  else
    //  {
    //    strGrouping = ChartConstants.Standard;
    //  }

    //  ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.BarGroupingTag, strGrouping );
    //}
    /// <summary>
    /// Extracts area3D chart.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="chart">Chart to serialize.</param>
    /// <param name="relations">Chart item relations.</param>
    /// <param name="dictSeriesAxis">Dictionary with axis id, key - series index, value - axis index (category or value).</param>
    private void ParseArea3DChart( XmlReader reader, ChartImpl chart, RelationCollection relations,
      Dictionary<int, int> dictSeriesAxis )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      if( reader.LocalName != ChartConstants.Area3DChartTag )
        throw new XmlException( "Unexpected xml tag." );

      reader.Read();
      List<ChartSerieImpl> lstSeries = new List<ChartSerieImpl>();
      ParseAreaChartCommon( reader, chart, true, relations, lstSeries, true );
      bool secaxisid = false;
      bool primaryaxisid = false;
      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case ChartConstants.AxisIdTag:
              ParseAxisId( reader, lstSeries, dictSeriesAxis );              
              break;
            case Excel2007Serializator.Extensionlist:
              IChartSerie serie = ParseFilteredSeries(reader, chart, relations, true, chart.ChartType,false);              
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
    }
    /// <summary>
    /// Extracts area chart from XmlReader.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="chart">Chart to put extracted data into.</param>
    /// <param name="relations">Chart item relations.</param>
    /// <param name="dictSeriesAxis">Dictionary with axis id, key - series index, value - axis index (category or value).</param>
    private void ParseAreaChart( XmlReader reader, ChartImpl chart,
      RelationCollection relations, Dictionary<int, int> dictSeriesAxis )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      if( reader.LocalName != ChartConstants.AreaChartTag )
        throw new XmlException( "Unexpected xml tag." );

      reader.Read();
      bool primaryaxisid = false;
      List<ChartSerieImpl> lstSeries = new List<ChartSerieImpl>();

      bool bSecondary = GetAxisType( chart, ref reader );

      ParseAreaChartCommon( reader, chart, false, relations, lstSeries, !bSecondary );

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.LocalName == ChartConstants.AxisIdTag )
        {
          bSecondary = ParseAxisId( reader, lstSeries, dictSeriesAxis );
          if (bSecondary)
              primaryaxisid = true;
        }
        else if (reader.LocalName == Excel2007Serializator.Extensionlist)
        {IChartSerie serie=null;
           
            if(chart.Series[0]!=null)
             serie = ParseFilteredSeries(reader, chart, relations, true, chart.Series[0].SerieType,primaryaxisid);            
        }
        else
        {
          reader.Skip();
        }
      }
    }

    private bool GetAxisType( ChartImpl chart, ref XmlReader reader )
    {
      MemoryStream temp = new MemoryStream();
      XmlWriter writer = UtilityMethods.CreateWriter( temp, Encoding.UTF8 );
      writer.WriteStartElement( Excel2007Serializator.TemporaryRoot );
      bool bSecondary = false;

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.LocalName == ChartConstants.AxisIdTag )
        {
          string tagName = reader.LocalName;
          int iAxisId = ChartParserCommon.ParseIntValueTag( reader );
          int iValueId = ( chart.PrimaryValueAxis as ChartAxisImpl ).AxisId;
          int iCategoryId = ( chart.PrimaryCategoryAxis as ChartAxisImpl ).AxisId;
          bSecondary = iAxisId != iValueId && iAxisId != iCategoryId;
          ChartSerializatorCommon.SerializeValueTag( writer, tagName, iAxisId.ToString() );
          //bSecondary = ParseAxisId( reader, lstSeries, dictSeriesAxis );
        }
        else if( reader.NodeType == XmlNodeType.Element )
        {
          writer.WriteNode( reader, false );
        }
        else
        {
          reader.Skip();
        }
      
      }

      writer.WriteEndElement();
      writer.Flush();
      reader.Read();

      temp.Position = 0;
      reader = UtilityMethods.CreateReader( temp );
      reader.Read();

      return bSecondary;
    }
    /// <summary>
    /// Extracts properties common to the area chart.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="chart">Chart to put extracted data into.</param>
    /// <param name="b3D">Indicates whether we are parsing 3D chart.</param>
    /// <param name="relations">Chart item relations.</param>
    /// <param name="lstSeries">List that will get extracted series.</param>
    private void ParseAreaChartCommon( XmlReader reader, ChartImpl chart, bool b3D,
      RelationCollection relations, List<ChartSerieImpl> lstSeries, bool isPrimary )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      bool bContinue = true;
      string strGrouping = null;
      bool bVaryColors = false;
      ChartSerieImpl filteredserseries = null;
      ChartSerieImpl series = null;
      while( reader.NodeType != XmlNodeType.EndElement && bContinue )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case ChartConstants.BarGroupingTag:
              strGrouping = ChartParserCommon.ParseValueTag( reader );
              break;

            case ChartConstants.VaryColorsTag:
              bVaryColors = ChartParserCommon.ParseBoolValueTag( reader );
              break;

            case ChartConstants.SeriesTag:
              ExcelChartType seriesType = GetAreaSeriesType( strGrouping, b3D );
              series = ParseAreaSeries( reader, chart, seriesType, relations, isPrimary );
              lstSeries.Add( series );
              break;

            case ChartConstants.DataLabelsTag:
#if DEBUG && !(SILVERLIGHT)
              Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, "Data labels tag is not supported here" );
#endif
              reader.Skip();
              break;
              //throw new NotImplementedException();
              //reader.Skip();
              //break;

            case ChartConstants.DropLinesTag:
              // NOTE: we don't support it yet.
              reader.Skip();
              break;

            default:
              if (lstSeries.Count != 0)
              {
                  bContinue = false;
              }
              else
              {
                  seriesType = GetLineSeriesType(strGrouping, b3D);
                  filteredserseries = ParseFilterSecondaryAxis(reader, seriesType
                      , b3D, lstSeries, chart, relations, ref series);
                  bContinue = false;
              }
                  break;
              //return SerializeChartSeries( writer, chart, firstSeries, SerializeAreaSeries );
          }
        }
      }

      if( chart.HasPivotSource )
        chart.PivotChartType = GetAreaSeriesType( strGrouping, b3D );
    }
    /// <summary>
    /// This method extracts common properties of the line charts.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="chart">Chart to put extracted data into.</param>
    /// <param name="is3D">Indicates whether we are parsing 3D chart.</param>
    /// <param name="relations">Chart item relations.</param>
    /// <param name="lstSeries">List that will get extracted series.</param>
    /// <returns>One of the extracted series.</returns>
    private ChartSerieImpl ParseLineChartCommon( XmlReader reader, ChartImpl chart,
      bool is3D, RelationCollection relations, List<ChartSerieImpl> lstSeries, Excel2007Parser parser )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      bool bContinue = true;
      string strGrouping = null;
      bool bVaryColors = false;
      ChartSerieImpl result = null;
      ChartSerieImpl filteredserseries = null;
      ChartSerieImpl series = null;
      ExcelChartType seriesType = ExcelChartType.Line;
      while( reader.NodeType != XmlNodeType.EndElement && bContinue )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case ChartConstants.BarGroupingTag:
              strGrouping = ChartParserCommon.ParseValueTag( reader );
              break;

            case ChartConstants.VaryColorsTag:
              bVaryColors = ChartParserCommon.ParseBoolValueTag( reader );
              break;

            case ChartConstants.SeriesTag:
              seriesType = GetLineSeriesType( strGrouping, is3D );
                series = ParseLineSeries( reader, chart, seriesType, relations, parser );
              lstSeries.Add( series );

              if( result == null )
                result = series;
              break;

            case ChartConstants.DataLabelsTag:
              //ParseDataLables( reader, chart );
              //break;
              reader.Skip();
              //throw new NotImplementedException();
              //Debug.Fail( "Pasing is not implemented yet" );
              break;

            case ChartConstants.DropLinesTag:
              //TODO: Need to provide parsing support instead of preserving
              if (result != null)
                  result.DropLinesStream = ShapeParser.ReadNodeAsStream(reader, true);
              else
                  reader.Skip();
              break;

            default:
              if (lstSeries.Count != 0)
              {
                  bContinue = false;
              }
              else
              {
                  seriesType = GetLineSeriesType(strGrouping, is3D);
                  filteredserseries = ParseFilterSecondaryAxis(reader, seriesType
                      , is3D, lstSeries, chart, relations, ref series);
                  bContinue = false;
              }
                  break;
          }
        }
      }

      if( chart.HasPivotSource )
        chart.PivotChartType = GetLineSeriesType( strGrouping, is3D );

      return result;
    }
    /// <summary>
    /// Extracts line3DChart from XmlReader.
    /// </summary>
    /// <param name="reader">XmlReader to extract line 3d chart from.</param>
    /// <param name="chart">Chart to put extracted data into.</param>
    /// <param name="relations">Chart item relations.</param>
    /// <param name="dictSeriesAxis">Dictionary with axis id, key - series index, value - axis index (category or value).</param>
    private void ParseLine3DChart( XmlReader reader, ChartImpl chart,
      RelationCollection relations, Dictionary<int, int> dictSeriesAxis, Excel2007Parser parser )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      if( reader.LocalName != ChartConstants.Line3DChartTag )
        throw new XmlException( "Unexpected xml tag." );

      reader.Read();
      List<ChartSerieImpl> lstSeries = new List<ChartSerieImpl>();
      ParseLineChartCommon( reader, chart, true, relations, lstSeries, parser );

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.LocalName == ChartConstants.AxisIdTag )
        {
          ParseAxisId( reader, lstSeries, dictSeriesAxis );
        }
        else if (reader.LocalName == Excel2007Serializator.Extensionlist)
        {
            IChartSerie serie = ParseFilteredSeries(reader, chart, relations, true, ExcelChartType.Line_3D,false);
        }
        else
        {
          reader.Skip();
        }
      }

      reader.Read();
    }
    /// <summary>
    /// Extracts line chart.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="chart">Chart to put extracted data into.</param>
    /// <param name="relations">Chart item relations.</param>
    /// <param name="dictSeriesAxis">Dictionary with axis id, key - series index, value - axis index (category or value).</param>
    private void ParseLineChart( XmlReader reader, ChartImpl chart,
      RelationCollection relations, Dictionary<int, int> dictSeriesAxis, Excel2007Parser parser )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      //string strMainTag = chart.IsChartStock ?
      //  ChartConstants.StockChartTag :
      //  ChartConstants.LineChartTag;

      //writer.WriteStartElement( strMainTag, ChartConstants.CNamespace );
      reader.Read();
      List<ChartSerieImpl> lstSeries = new List<ChartSerieImpl>();
      ChartSerieImpl firstSeries = ParseLineChartCommon( reader, chart, false, relations, lstSeries, parser );
      bool bMarker = false;
      bool secaxisid = false;
      bool primaryaxisid = false;
      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element)
        {
          switch( reader.LocalName )
          {
            case ChartConstants.UpDownBarsTag:
              ParseUpDownBars( reader, firstSeries, relations );
              break;

            case ChartConstants.MarkerTag:
              bMarker = ChartParserCommon.ParseBoolValueTag( reader );

              if( bMarker )
              {
                // access to the marker to create necessary records.
                ChartSerieDataFormatImpl dataFormat = ( ChartSerieDataFormatImpl )firstSeries.SerieFormat;
                ChartMarkerFormatRecord markerRecord = dataFormat.MarkerFormat;
              }
              break;

            case ChartConstants.HiLowLinesTag:
              ParseHiLowLines( reader, firstSeries );
              break;

            case ChartConstants.AxisIdTag:
              secaxisid=ParseAxisId(reader, lstSeries, dictSeriesAxis);
              if (secaxisid)
                  primaryaxisid = true;
              break;
            case Excel2007Serializator.Extensionlist:
              IChartSerie serie = ParseFilteredSeries(reader, chart, relations, true, firstSeries.SerieType,primaryaxisid);              
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
    /// Extracts bubble chart.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="chart">Chart to put extracted data into.</param>
    /// <param name="relations">Chart item relations.</param>
    /// <param name="dictSeriesAxis">Dictionary with axis id, key - series index, value - axis index (category or value).</param>
    private void ParseBubbleChart( XmlReader reader, ChartImpl chart,
      RelationCollection relations, Dictionary<int, int> dictSeriesAxis )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      if( reader.LocalName != ChartConstants.BubbleChartTag )
        throw new XmlException( "Unexpected xml tag." );

      reader.Read();
      bool bVaryColors = false;
      ChartSerieImpl series = null;
      IChartFormat commonOptions;
      List<ChartSerieImpl> lstSeries = new List<ChartSerieImpl>();

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case ChartConstants.VaryColorsTag:
              bVaryColors = ChartParserCommon.ParseBoolValueTag( reader );
              break;

            case ChartConstants.SeriesTag:
              series = ParseBubbleSeries( reader, chart, relations );
              lstSeries.Add( series );
              break;

            case ChartConstants.BubbleScaleTag:
              int iBubbleScale = ChartParserCommon.ParseIntValueTag( reader );
              commonOptions = series.SerieFormat.CommonSerieOptions;
              commonOptions.BubbleScale = iBubbleScale;
              break;

            case ChartConstants.ShowNegativeBubbles:
              bool bShowNegative = ChartParserCommon.ParseBoolValueTag( reader );
              commonOptions = series.SerieFormat.CommonSerieOptions;
              commonOptions.ShowNegativeBubbles = bShowNegative;
              break;

            case ChartConstants.BubbleSizeRepresents:
              string strBubbleSize = ChartParserCommon.ParseValueTag( reader );
              commonOptions = series.SerieFormat.CommonSerieOptions;
              commonOptions.SizeRepresents = ( strBubbleSize == ChartConstants.BubbleSizeArea ) ?
                ExcelBubbleSize.Area :
                ExcelBubbleSize.Width;
              break;

            case ChartConstants.AxisIdTag:
              ParseAxisId( reader, lstSeries, dictSeriesAxis );
              break;
            case Excel2007Serializator.Extensionlist:
              IChartSerie serie = ParseFilteredSeries(reader, chart, relations, false, series.SerieType, false);
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
    /// Extracts 2-D surface chart from XmlReader.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="chart">Chart to put extracted data into.</param>
    /// <param name="relations">Chart item relations.</param>
    /// <param name="dictSeriesAxis">Dictionary with axis id, key - series index, value - axis index (category or value).</param>
    private void ParseSurfaceChart( XmlReader reader, ChartImpl chart,
      RelationCollection relations, Dictionary<int, int> dictSeriesAxis )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      bool b3D;

      if( reader.LocalName == ChartConstants.SurfaceChartTag )
      {
        b3D = false;
      }
      else if( reader.LocalName == ChartConstants.Surface3DChartTag )
      {
        b3D = true;
      }
      else
      {
        throw new XmlException( "Unexpected xml tag." );
      }

      reader.Read();

      List<ChartSerieImpl> lstSeries = new List<ChartSerieImpl>();
      ParseSurfaceCommon( reader, chart, b3D, relations, lstSeries );

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.LocalName == ChartConstants.AxisIdTag )
        {
          ParseAxisId( reader, lstSeries, dictSeriesAxis );
        }
        else if (reader.LocalName == Excel2007Serializator.Extensionlist)
        {
            IChartSerie serie = ParseFilteredSeries(reader, chart, relations, b3D, chart.ChartType,false);
            
        }
        else
        {
          reader.Skip();
        }
      }

      reader.Skip();
    }
    ///// <summary>
    ///// Serializes 3-D surface chart.
    ///// </summary>
    ///// <param name="writer">XmlWriter to serialize into.</param>
    ///// <param name="chart">Chart to serialize.</param>
    ///// <param name="firstSeries">First series in the list of series with the
    ///// same formatting to serialize.</param>
    //private int SerializeSurface3DChart( XmlWriter writer, ChartImpl chart, ChartSerieImpl firstSeries )
    //{
    //  if( writer == null )
    //    throw new ArgumentNullException( "writer" );

    //  if( chart == null )
    //    throw new ArgumentNullException( "chart" );

    //  writer.WriteStartElement( ChartConstants.Surface3DChartTag, ChartConstants.CNamespace );
    //  int iResult = SerializeSurfaceCommon( writer, chart, firstSeries );
    //  SerializeBarAxisId( writer, chart, firstSeries );
    //  writer.WriteEndElement();

    //  return iResult;
    //}
    /// <summary>
    /// Extracts common part of the surface charts.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="chart">Chart to put extracted data into.</param>
    /// <param name="is3D">Indicates whether we are parsing 3D chart.</param>
    /// <param name="relations">Chart item relations.</param>
    /// <param name="lstSeries">List that will get extracted series.</param>
    private void ParseSurfaceCommon( XmlReader reader, ChartImpl chart, bool is3D,
      RelationCollection relations, List<ChartSerieImpl> lstSeries )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );
      ExcelChartType seriesType = ExcelChartType.Surface_3D;
      bool bContinue = true;
      bool bWireframe = false;

      while( reader.NodeType != XmlNodeType.EndElement && bContinue )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case ChartConstants.WireframeTag:
              //ExcelChartType chartType = firstSeries.SerieType;
              //bool bWireFrame = ( chartType == ExcelChartType.Surface_Contour
              //  || chartType == ExcelChartType.Surface_NoColor_Contour );
              bWireframe = ChartParserCommon.ParseBoolValueTag( reader );
              break;

            case ChartConstants.SeriesTag:
              seriesType = GetSurfaceSeriesType( bWireframe, is3D );
              ChartSerieImpl series = ParseSurfaceSeries( reader, chart, seriesType, relations );
              lstSeries.Add( series );
              break;

            case ChartConstants.BandFormats:
              ParseBandFormats( reader, chart );
              break;
            case Excel2007Serializator.Extensionlist:
              IChartSerie serie = ParseFilteredSeries(reader, chart, relations, true, seriesType,false);
              break;
            default:
              bContinue = false;
              break;
          }
        }
        else
        {
          reader.Skip();
        }
      }

      if( chart.HasPivotSource )
        chart.PivotChartType = GetSurfaceSeriesType( bWireframe, is3D );
    }
    /// <summary>
    /// Extracts band formats from the stream.
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="chart"></param>
    private void ParseBandFormats( XmlReader reader, ChartImpl chart )
    {
      chart.PreservedBandFormats = ShapeParser.ReadNodeAsStream( reader );
    }
    /// <summary>
    /// Returns type of the series to create.
    /// </summary>
    /// <param name="bWireframe">Indicates whether surface is wireframe or not.</param>
    /// <param name="is3D">Indicates whether we are parsing 3D chart.</param>
    /// <returns>Surface chart type.</returns>
    private ExcelChartType GetSurfaceSeriesType( bool bWireframe, bool is3D )
    {
      ExcelChartType result;

      if( bWireframe )
      {
        result = ( is3D ) ?
          ExcelChartType.Surface_NoColor_3D :
          ExcelChartType.Surface_NoColor_Contour;
      }
      else
      {
        result = ( is3D ) ?
          ExcelChartType.Surface_3D :
          ExcelChartType.Surface_Contour;
      }

      return result;
    }
    ///// <summary>
    ///// Serializes main chart tag.
    ///// </summary>
    ///// <param name="writer">XmlWriter to serialize into.</param>
    ///// <param name="chart">Chart to serialize.</param>
    ///// <param name="groupIndex">Index of the series group to serialize.</param>
    //private int SerializeMainChartTypeTag( XmlWriter writer, ChartImpl chart, int groupIndex )
    //{
    //  if( writer == null )
    //    throw new ArgumentNullException( "writer" );

    //  if( chart == null )
    //    throw new ArgumentNullException( "chart" );

    //  // TODO: we don't support multi type charts, we should add it later.
    //  // 1. Find first series with that index
    //  IChartSeries arrSeries = chart.Series;
    //  ChartSerieImpl firstSeries = null;

    //  for( int i = 0, len = arrSeries.Count; i < len; i++ )
    //  {
    //    ChartSerieImpl series = ( ChartSerieImpl )arrSeries[ i ];

    //    if( series.ChartGroup == groupIndex )
    //    {
    //      firstSeries = series;
    //      break;
    //    }
    //  }


    //  int iResult = 0;
    //  if( firstSeries != null )
    //  {
    //    switch( firstSeries.SerieType )
    //    {
    //      case ExcelChartType.Column_Clustered:
    //      case ExcelChartType.Column_Stacked:
    //      case ExcelChartType.Column_Stacked_100:
    //      case ExcelChartType.Bar_Clustered:
    //      case ExcelChartType.Bar_Stacked:
    //      case ExcelChartType.Bar_Stacked_100:
    //        iResult = SerializeBarChart( writer, chart, firstSeries );
    //        break;

    //      case ExcelChartType.Column_Clustered_3D:
    //      case ExcelChartType.Column_Stacked_3D:
    //      case ExcelChartType.Column_Stacked_100_3D:
    //      case ExcelChartType.Column_3D:
    //      case ExcelChartType.Bar_Clustered_3D:
    //      case ExcelChartType.Bar_Stacked_3D:
    //      case ExcelChartType.Bar_Stacked_100_3D:
    //      case ExcelChartType.Cylinder_Clustered:
    //      case ExcelChartType.Cylinder_Stacked:
    //      case ExcelChartType.Cylinder_Stacked_100:
    //      case ExcelChartType.Cylinder_Bar_Clustered:
    //      case ExcelChartType.Cylinder_Bar_Stacked:
    //      case ExcelChartType.Cylinder_Bar_Stacked_100:
    //      case ExcelChartType.Cylinder_Clustered_3D:
    //      case ExcelChartType.Cone_Clustered:
    //      case ExcelChartType.Cone_Stacked:
    //      case ExcelChartType.Cone_Stacked_100:
    //      case ExcelChartType.Cone_Bar_Clustered:
    //      case ExcelChartType.Cone_Bar_Stacked:
    //      case ExcelChartType.Cone_Bar_Stacked_100:
    //      case ExcelChartType.Cone_Clustered_3D:
    //      case ExcelChartType.Pyramid_Clustered:
    //      case ExcelChartType.Pyramid_Stacked:
    //      case ExcelChartType.Pyramid_Stacked_100:
    //      case ExcelChartType.Pyramid_Bar_Clustered:
    //      case ExcelChartType.Pyramid_Bar_Stacked:
    //      case ExcelChartType.Pyramid_Bar_Stacked_100:
    //      case ExcelChartType.Pyramid_Clustered_3D:
    //        iResult = SerializeBar3DChart( writer, chart, firstSeries );
    //        break;

    //      case ExcelChartType.Line:
    //      case ExcelChartType.Line_Stacked:
    //      case ExcelChartType.Line_Stacked_100:
    //      case ExcelChartType.Line_Markers:
    //      case ExcelChartType.Line_Markers_Stacked:
    //      case ExcelChartType.Line_Markers_Stacked_100:
    //        iResult = SerializeLineChart( writer, chart, firstSeries );
    //        break;

    //      case ExcelChartType.Line_3D:
    //        iResult = SerializeLine3DChart( writer, chart, firstSeries );
    //        break;

    //      case ExcelChartType.Pie:
    //      case ExcelChartType.Pie_Exploded:
    //        iResult = SerializePieChart( writer, chart, firstSeries );
    //        break;

    //      case ExcelChartType.Pie_3D:
    //      case ExcelChartType.Pie_Exploded_3D:
    //        iResult = SerializePie3DChart( writer, chart, firstSeries );
    //        break;

    //      case ExcelChartType.PieOfPie:
    //      case ExcelChartType.Pie_Bar:
    //        iResult = SerializeOfPieChart( writer, chart, firstSeries );
    //        break;

    //      case ExcelChartType.Scatter_Markers:
    //      case ExcelChartType.Scatter_SmoothedLine_Markers:
    //      case ExcelChartType.Scatter_SmoothedLine:
    //      case ExcelChartType.Scatter_Line_Markers:
    //      case ExcelChartType.Scatter_Line:
    //        iResult = SerializeScatterChart( writer, chart, firstSeries );
    //        break;

    //      case ExcelChartType.Area:
    //      case ExcelChartType.Area_Stacked:
    //      case ExcelChartType.Area_Stacked_100:
    //        iResult = SerializeAreaChart( writer, chart, firstSeries );
    //        break;

    //      case ExcelChartType.Area_3D:
    //      case ExcelChartType.Area_Stacked_3D:
    //      case ExcelChartType.Area_Stacked_100_3D:
    //        iResult = SerializeArea3DChart( writer, chart, firstSeries );
    //        break;

    //      case ExcelChartType.Doughnut:
    //      case ExcelChartType.Doughnut_Exploded:
    //        iResult = SerializeDoughnutChart( writer, chart, firstSeries );
    //        break;

    //      case ExcelChartType.Radar:
    //      case ExcelChartType.Radar_Markers:
    //      case ExcelChartType.Radar_Filled:
    //        iResult = SerializeRadarChart( writer, chart, firstSeries );
    //        break;

    //      case ExcelChartType.Surface_3D:
    //      case ExcelChartType.Surface_NoColor_3D:
    //        iResult = SerializeSurface3DChart( writer, chart, firstSeries );
    //        break;

    //      case ExcelChartType.Surface_Contour:
    //      case ExcelChartType.Surface_NoColor_Contour:
    //        iResult = SerializeSurfaceChart( writer, chart, firstSeries );
    //        break;

    //      case ExcelChartType.Bubble:
    //      case ExcelChartType.Bubble_3D:
    //        iResult = SerializeBubbleChart( writer, chart, firstSeries );
    //        break;

    //      case ExcelChartType.Stock_HighLowClose:
    //      case ExcelChartType.Stock_OpenHighLowClose:
    //      case ExcelChartType.Stock_VolumeHighLowClose:
    //      case ExcelChartType.Stock_VolumeOpenHighLowClose:
    //        iResult = SerializeStockChart( writer, chart, firstSeries );
    //        break;
    //    }
    //  }

    //  return iResult;
    //}
    /// <summary>
    /// Extracts radar chart.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="chart">Chart to put extracted data into.</param>
    /// <param name="relations">Chart item relations.</param>
    /// <param name="dictSeriesAxis">Dictionary with axis id, key - series index, value - axis index (category or value).</param>
    private void ParseRadarChart( XmlReader reader, ChartImpl chart,
      RelationCollection relations, Dictionary<int, int> dictSeriesAxis, Excel2007Parser parser )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      if( reader.LocalName != ChartConstants.RadarChartTag )
        throw new XmlException( "Unexpected xml tag." );

      reader.Read();

      ExcelChartType seriesType = ExcelChartType.Radar;
      bool bVaryColors = false;
      List<ChartSerieImpl> lstSeries = new List<ChartSerieImpl>();
      bool secaxisid = false;
      bool primaryaxisid = false;
      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case ChartConstants.RadarStyleTag:
              string strRadarStype = ChartParserCommon.ParseValueTag( reader );
              Excel2007RadarStyle radarStyle = ( Excel2007RadarStyle )Enum.Parse(
                typeof( Excel2007RadarStyle ), strRadarStype, false );
              seriesType = ( ExcelChartType )radarStyle;
              chart.RadarStyle = strRadarStype;
              break;

            case ChartConstants.VaryColorsTag:
              bVaryColors = ChartParserCommon.ParseBoolValueTag( reader );
              break;

            case ChartConstants.SeriesTag:
              ChartSerieImpl series = ParseRadarSeries( reader, chart, seriesType, relations, parser );
              lstSeries.Add( series );

              if( bVaryColors )
                series.SerieFormat.CommonSerieOptions.IsVaryColor = true;
              break;

            case ChartConstants.AxisIdTag:
              secaxisid=ParseAxisId(reader, lstSeries, dictSeriesAxis);
              if (secaxisid)
                  primaryaxisid = true;
              break;

            case Excel2007Serializator.Extensionlist:
              IChartSerie serie = ParseFilteredSeries(reader, chart, relations, false, seriesType, primaryaxisid);
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

      if( chart.HasPivotSource )
        chart.PivotChartType = seriesType;

      reader.Read();
    }
    /// <summary>
    /// Extracts scatter chart.
    /// </summary>
    /// <param name="reader">XmlReader to extract chart from.</param>
    /// <param name="chart">Chart to serialize.</param>
    /// <param name="relations">Chart item relations.</param>
    /// <param name="dictSeriesAxis">Dictionary with axis id, key - series index, value - axis index (category or value).</param>
    private void ParseScatterChart( XmlReader reader, ChartImpl chart,
      RelationCollection relations, Dictionary<int, int> dictSeriesAxis, Excel2007Parser parser )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      if( reader.LocalName != ChartConstants.ScatterChartTag )
        throw new XmlException( "Unexpected xml tag." );

      reader.Read();
      ExcelChartType seriesType = ExcelChartType.Scatter_Markers;
      bool bVaryColors = true;
      bool secaxisid = false;
      bool primaryaxisid = false;
      ChartSerieImpl series = null;
      List<ChartSerieImpl> lstSeries = new List<ChartSerieImpl>();

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case ChartConstants.ScatterStyleTag:
              string strScatterStyle = ChartParserCommon.ParseValueTag( reader );
              Excel2007ScatterStyle scatterStyle = ( Excel2007ScatterStyle )Enum.Parse(
                typeof( Excel2007ScatterStyle ), strScatterStyle, false );
              seriesType = ( ExcelChartType )scatterStyle;
              break;

            case ChartConstants.VaryColorsTag:
              bVaryColors = ChartParserCommon.ParseBoolValueTag( reader );
              break;

            case ChartConstants.SeriesTag:
              series = ParseScatterSeries( reader, chart, seriesType, relations, parser );
              lstSeries.Add( series );

              if( bVaryColors )
                series.SerieFormat.CommonSerieOptions.IsVaryColor = true;
              break;

            case ChartConstants.UpDownBarsTag:
              ParseUpDownBars( reader, series, relations );
              break;

            case ChartConstants.AxisIdTag:
              secaxisid=ParseAxisId(reader, lstSeries, dictSeriesAxis);
              if (secaxisid)
                  primaryaxisid = true;
              break;
            case Excel2007Serializator.Extensionlist:
              IChartSerie serie = ParseFilteredSeries(reader, chart, relations, false, seriesType, primaryaxisid);
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
    /// Extracts pie chart.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="chart">Chart to put extracted data into.</param>
    /// <param name="relations">Chart item relations.</param>
    /// <param name="dictSeriesAxis">Dictionary with axis id, key - series index, value - axis index (category or value).</param>
    private void ParsePieChart( XmlReader reader, ChartImpl chart,
      RelationCollection relations, Dictionary<int, int> dictSeriesAxis )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      if( reader.LocalName != ChartConstants.PieChartTag )
        throw new XmlException( "Unexpected xml tag." );

      reader.Read();
      List<ChartSerieImpl> lstSeries = new List<ChartSerieImpl>();
      ChartSerieImpl series = ParsePieCommon( reader, chart, ExcelChartType.Pie, relations, lstSeries );

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element && series !=null )
        {
          switch( reader.LocalName )
          {
            case ChartConstants.FirstSliceAngleTag:
              IChartFormat commonOptions = series.SerieFormat.CommonSerieOptions;
              commonOptions.FirstSliceAngle = ChartParserCommon.ParseIntValueTag( reader );
              break;

            case ChartConstants.AxisIdTag:
              ParseAxisId( reader, lstSeries, dictSeriesAxis );
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
    /// Extracts 3-D pie chart.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="chart">Chart to put extracted data into.</param>
    /// <param name="relations">Chart item relations.</param>
    /// <param name="dictSeriesAxis">Dictionary with axis id, key - series index, value - axis index (category or value).</param>
    private void ParsePie3DChart( XmlReader reader, ChartImpl chart,
      RelationCollection relations, Dictionary<int, int> dictSeriesAxis )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      if( reader.LocalName != ChartConstants.Pie3DChartTag )
        throw new XmlException( "Unexpected xml tag." );

      reader.Read();
      List<ChartSerieImpl> lstSeries = new List<ChartSerieImpl>();
      ParsePieCommon( reader, chart, ExcelChartType.Pie_3D, relations, lstSeries );

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.LocalName == ChartConstants.AxisIdTag )
        {
          ParseAxisId( reader, lstSeries, dictSeriesAxis );
        }
        else
        {
          reader.Skip();
        }
      }

      reader.Read();
    }
    /// <summary>
    /// Extracts pie of pie or pie of bar chart.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="chart">Chart to put extracted data into.</param>
    /// <param name="relations">Chart item relations.</param>
    /// <param name="dictSeriesAxis">Dictionary with axis id, key - series index, value - axis index (category or value).</param>
    private void ParseOfPieChart( XmlReader reader, ChartImpl chart,
      RelationCollection relations, Dictionary<int, int> dictSeriesAxis )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      if( reader.LocalName != ChartConstants.OfPieChartTag )
        throw new XmlException( "Unexpected xml tag." );

      reader.Read();
      ExcelChartType seriesType = ExcelChartType.PieOfPie;
      ChartSerieImpl series = null;
      IChartFormat commonOptions;
      List<ChartSerieImpl> lstSeries = new List<ChartSerieImpl>();
      int? gapWidth = null;

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case ChartConstants.OfPieTypeTag:
              string strType = ChartParserCommon.ParseValueTag( reader );
              seriesType = ( strType == ChartConstants.OfPieTypePie ) ?
                ExcelChartType.PieOfPie :
                ExcelChartType.Pie_Bar;
              break;

            case ChartConstants.VaryColorsTag:
            case ChartConstants.SeriesTag:
            case ChartConstants.DataLabelsTag:
              series = ParsePieCommon( reader, chart, seriesType, relations, lstSeries );
              break;

            case ChartConstants.GapWidthTag:
              gapWidth = ChartParserCommon.ParseIntValueTag( reader );
              break;

            case ChartConstants.SplitTypeTag:
              string strSplitType = ChartParserCommon.ParseValueTag( reader );
              Excel2007SplitType splitType = ( Excel2007SplitType )Enum.Parse(
                typeof( Excel2007SplitType ), strSplitType, false );
              commonOptions = series.SerieFormat.CommonSerieOptions;
              commonOptions.SplitType = ( ExcelSplitType )splitType;
              break;

            case ChartConstants.SplitPosTag:
              commonOptions = series.SerieFormat.CommonSerieOptions;
              commonOptions.SplitValue = ChartParserCommon.ParseIntValueTag( reader );
              break;

            case ChartConstants.SecondPieSizeTag:
              // custSplit
              commonOptions = series.SerieFormat.CommonSerieOptions;
              commonOptions.PieSecondSize = ChartParserCommon.ParseIntValueTag( reader );
              break;

            case ChartConstants.AxisIdTag:
              ParseAxisId( reader, lstSeries, dictSeriesAxis );
              break;

            case ChartConstants.SeriesLinesTag:
            //writer.WriteElementString( ChartConstants.SeriesLinesTag, ChartConstants.CNamespace, string.Empty );
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

      if( gapWidth != null )
        series.SerieFormat.CommonSerieOptions.GapWidth = ( int )gapWidth;

      if( chart.HasPivotSource )
        chart.PivotChartType = seriesType;

      reader.Read();
    }
    /// <summary>
    /// Extracts stock chart.
    /// </summary>
    /// <param name="reader">XmlReader to extract from.</param>
    /// <param name="chart">Chart to put extracted data into.</param>
    /// <param name="relations">Chart item relations.</param>
    /// <param name="dictSeriesAxis">Dictionary with axis id, key - series index, value - axis index (category or value).</param>
    private void ParseStockChart( XmlReader reader, ChartImpl chart,
      RelationCollection relations, Dictionary<int, int> dictSeriesAxis, Excel2007Parser parser )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      if( reader.LocalName != ChartConstants.StockChartTag )
        throw new XmlException( "Unexpected xml tag." );

      reader.Read();
      ChartSerieImpl series = null;
      List<ChartSerieImpl> lstSeries = new List<ChartSerieImpl>();

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case ChartConstants.SeriesTag:
              series = ParseLineSeries( reader, chart, ExcelChartType.Line, relations, parser );
              lstSeries.Add( series );
              //IChartSeries arrSeries = chart.Series;
              //for( int i = 0, len = arrSeries.Count; i < len; i++ )
              //{
              //  ChartSerieImpl series = ( ChartSerieImpl )arrSeries[ i ];
              //  SerializeLineSeries( writer, series );
              //}
              break;

            case ChartConstants.HiLowLinesTag:
              ParseHiLowLines( reader, series );
              break;

            case ChartConstants.UpDownBarsTag:
              ParseUpDownBars( reader, series, relations );
              break;

            case ChartConstants.AxisIdTag:
              ParseAxisId( reader, lstSeries, dictSeriesAxis );
              break;
            case Excel2007Serializator.Extensionlist:
              IChartSerie serie = ParseFilteredSeries(reader, chart, relations, true, series.SerieType,false);
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
    /// Extracts hi-low lines object from xml reader.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="series">Series object to extract.</param>
    private void ParseHiLowLines( XmlReader reader, ChartSerieImpl series )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( series == null )
        throw new ArgumentNullException( "series" );

      ChartImpl chart = series.ParentChart;
      ChartFormatImpl format = ( ChartFormatImpl )chart.PrimaryFormats[ series.ChartGroup ];
      format.LineStyle = ExcelDropLineStyle.HiLow;

      // NOTE: we don't support any formatting.
      reader.Skip();
    }
    /// <summary>
    /// Extracts doughnut chart.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="chart">Chart to put extracted data into.</param>
    /// <param name="relations">Chart item relations.</param>
    /// <param name="dictSeriesAxis">Dictionary with axis id, key - series index, value - axis index (category or value).</param>
    private void ParseDoughnutChart( XmlReader reader, ChartImpl chart,
      RelationCollection relations, Dictionary<int, int> dictSeriesAxis )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      if( reader.LocalName != ChartConstants.DoughnutChartTag )
        throw new XmlException( "Unexpected xml tag." );

      reader.Read();
      List<ChartSerieImpl> lstSeries = new List<ChartSerieImpl>();
      IChartFormat commonOptions = null;
      ChartSerieImpl series = ParsePieCommon( reader, chart, ExcelChartType.Doughnut, relations, lstSeries );

      if (series != null)
      {
        commonOptions = (reader.NodeType != XmlNodeType.EndElement) ?
        series.SerieFormat.CommonSerieOptions :
        null;
      }

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element && series != null )
        {
          switch( reader.LocalName )
          {
            case ChartConstants.FirstSliceAngleTag:
              commonOptions.FirstSliceAngle = ChartParserCommon.ParseIntValueTag( reader );
              break;

            case ChartConstants.DoughnutHoleSizeTag:
              commonOptions.DoughnutHoleSize = ChartParserCommon.ParseIntValueTag( reader );
              break;

            case ChartConstants.AxisIdTag:
              ParseAxisId( reader, lstSeries, dictSeriesAxis );
              break;
            case Excel2007Serializator.Extensionlist:
              IChartSerie serie = ParseFilteredSeries(reader, chart, relations, true, series.SerieType,false);
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

      if( chart.HasPivotSource )
        chart.PivotChartType = ExcelChartType.Doughnut;

      reader.Read();
    }
    /// <summary>
    /// Extracts common properties of a pie charts.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="chart">Chart to put extracted data into.</param>
    /// <param name="seriesType">Type of the series to create.</param>
    /// <param name="relations">Chart item relations.</param>
    /// <param name="lstSeries">List that will get extracted series.</param>
    /// <returns>One of the parsed series.</returns>
    private ChartSerieImpl ParsePieCommon( XmlReader reader, ChartImpl chart,
      ExcelChartType seriesType, RelationCollection relations, List<ChartSerieImpl> lstSeries )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      bool bContinue = true;
      ChartSerieImpl series = null;
      bool bVaryColors = false;

      while( reader.NodeType != XmlNodeType.EndElement && bContinue )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case ChartConstants.VaryColorsTag:
              bVaryColors = ChartParserCommon.ParseBoolValueTag( reader );
              break;

            case ChartConstants.SeriesTag:
              series = ParsePieSeries( reader, chart, seriesType, relations );
              series.SerieFormat.CommonSerieOptions.IsVaryColor = bVaryColors;
              lstSeries.Add( series );
              break;

            case ChartConstants.DataLabelsTag:
              if (series != null)
                  ParseDataLabels(reader, series);
              else
                  reader.Skip();
              break;

            default:
              bContinue = false;
              break;
          }
        }
        else
        {
          reader.Skip();
        }
      }

      if( chart.HasPivotSource )
        chart.PivotChartType = seriesType;

      return series;
    }
    /// <summary>
    /// Extracts data lables.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="series">Parent series.</param>
    private void ParseDataLabels( XmlReader reader, ChartSerieImpl series )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( series == null )
        throw new ArgumentNullException( "series" );

      if( reader.LocalName != ChartConstants.DataLabelsTag )
        throw new XmlException( "Unexpected xml tag." );

      reader.Read();

      //ChartDataPointsCollection dataPoints = ( ChartDataPointsCollection )series.DataPoints;

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case ChartConstants.DataLabelTag:
              ParseDataLabel( reader, series );
              break;

            default:
              IChartDataLabels dataLabels = series.DataPoints.DefaultDataPoint.DataLabels;
              FileDataHolder holder = series.ParentBook.DataHolder;
              Excel2007Parser parser = holder.Parser;
              ParseDataLabelSettings( reader, dataLabels, parser, holder );
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
    /// Serializes data label for single data point.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="series">Chart series to put extracted data into.</param>
    private void ParseDataLabel( XmlReader reader, ChartSerieImpl series )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( series == null )
        throw new ArgumentNullException( "series" );

      if( reader.LocalName != ChartConstants.DataLabelTag )
        throw new XmlException( "Unexpeced xml tag." );

      reader.Read();
      IChartDataLabels dataLabels = null;

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case ChartConstants.IndexTag:
              int index = ChartParserCommon.ParseIntValueTag( reader );
              dataLabels = series.DataPoints[ index ].DataLabels;
              (dataLabels as ChartDataLabelsImpl).ShowTextProperties = false;
              break;

            case ChartConstants.LayoutTag:
              (dataLabels as ChartDataLabelsImpl).Layout = new ChartLayoutImpl(m_book.Application, (dataLabels as ChartDataLabelsImpl), series.Parent);
              ChartParserCommon.ParseChartLayout(reader, (dataLabels as ChartDataLabelsImpl).Layout);
              break;

            case ChartConstants.DeleteTag:
              bool isDelete = ChartParserCommon.ParseBoolValueTag(reader);
              (dataLabels as ChartDataLabelsImpl).IsDelete = isDelete;
              break;

            default:
              FileDataHolder holder = series.ParentBook.DataHolder;
              Excel2007Parser parser = holder.Parser;
              ParseDataLabelSettings( reader, dataLabels, parser, holder );
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
    /// Extracts data labels settings.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="dataLabels">Data labels to put extracted data into.</param>
    /// <param name="parser">Excel2007Parser to use if necessary.</param>
    private void ParseDataLabelSettings( XmlReader reader, IChartDataLabels dataLabels,
      Excel2007Parser parser, FileDataHolder holder )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( dataLabels == null )
        throw new ArgumentNullException( "dataLabels" );

      IInternalChartTextArea labelTextArea = dataLabels as IInternalChartTextArea;

      labelTextArea.Size = ChartAxisParser.DefaultFontSize;

      (dataLabels as ChartDataLabelsImpl).ShowTextProperties = false;

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case ChartConstants.DataLabelPosTag:
              string strPosition = ChartParserCommon.ParseValueTag( reader );
              Excel2007DataLabelPos position = ( Excel2007DataLabelPos )Enum.Parse(
                typeof( Excel2007DataLabelPos ), strPosition, false );

              dataLabels.Position = ( ExcelDataLabelPosition )position;
              break;

            case ChartConstants.ShowLegendKeyTag:
              if (!dataLabels.IsLegendKey)
                  dataLabels.IsLegendKey = ChartParserCommon.ParseBoolValueTag(reader);
              else
                  reader.Skip();
              break;

            case ChartConstants.ShowLeaderLineTag:
              bool value  = ChartParserCommon.ParseBoolValueTag( reader );

              if( value )
                dataLabels.ShowLeaderLines = value;
              break;

            case ChartConstants.ShowValueTag:
              if (!dataLabels.IsValue)
                  dataLabels.IsValue = ChartParserCommon.ParseBoolValueTag(reader);
              else
                  reader.Skip();
              break;

            case ChartConstants.ShowCategoryTag:
              if (!dataLabels.IsCategoryName)
                  dataLabels.IsCategoryName = ChartParserCommon.ParseBoolValueTag(reader);
              else
                  reader.Skip();
              break;

            case ChartConstants.ShowPercentageTag:
              if (!dataLabels.IsPercentage)
                  dataLabels.IsPercentage = ChartParserCommon.ParseBoolValueTag(reader);
              else
                  reader.Skip();
              break;

            case ChartConstants.ShowBubbleSizeTag:
              if (!dataLabels.IsBubbleSize)
                  dataLabels.IsBubbleSize = ChartParserCommon.ParseBoolValueTag(reader);
              else
                  reader.Skip();
              break;

            case ChartConstants.ShowSeriesNameTag:
              if (!dataLabels.IsSeriesName)
                  dataLabels.IsSeriesName = ChartParserCommon.ParseBoolValueTag(reader);
              else
                  reader.Skip();
              break;

            case ChartConstants.DataLabelsSeparatorTag:
              dataLabels.Delimiter = reader.ReadElementContentAsString();//ChartParserCommon.ParseValueTag( reader );
              break;

            case ChartConstants.TextPropertiesTag:
              IInternalChartTextArea textArea = dataLabels as IInternalChartTextArea;
              ParseDefaultTextFormatting( reader, textArea, parser );
              (dataLabels as ChartDataLabelsImpl).ShowTextProperties = true;
              IChartDataPoint parentDataPoint = (IChartDataPoint)(dataLabels as ChartDataLabelsImpl).Parent;
              if ((parentDataPoint.IsDefault) && ((parentDataPoint.DataLabels as ChartDataLabelsImpl).TextArea).Text != null)
              {
                  ChartTextAreaImpl defaultTextArea = (parentDataPoint.DataLabels as ChartDataLabelsImpl).TextArea;
                  ChartDataPointsCollection dataPoints = (ChartDataPointsCollection)parentDataPoint.Parent;
                  foreach (ChartDataPointImpl dataPoint in dataPoints)
                  {
                      if (dataPoint.HasDataLabels && (dataPoint.DataLabels as ChartDataLabelsImpl).ParagraphType != ChartParagraphType.CustomDefault)
                          (dataPoint.DataLabels as ChartDataLabelsImpl).TextArea = defaultTextArea;
                  }
              }             
              break;

            case ChartConstants.NumberFormatTag:
              string numberFormat = ChartParserCommon.ParseNumberFormat(reader);

              if (numberFormat != "")
                  (dataLabels as ChartDataLabelsImpl).NumberFormat = numberFormat;
              break;

            case ChartConstants.DeleteTag:
              bool isDelete = ChartParserCommon.ParseBoolValueTag(reader);
              (dataLabels as ChartDataLabelsImpl).IsDelete = isDelete;
              break;

              
            default:
              RelationCollection relations = new RelationCollection();
              ChartParserCommon.ParseTextAreaTag( reader,
                dataLabels as IInternalChartTextArea, relations, holder, 10.0F );
              //reader.Skip();
              break;
          }
        }
        else
        {
          reader.Skip();
        }
      }
    }
    ///// <summary>
    ///// Serializes IsVaryColors option.
    ///// </summary>
    ///// <param name="writer">XmlWriter to serialize into.</param>
    ///// <param name="firstSeries">First series in the list of series with the
    ///// same formatting to serialize vary colors option for.</param>
    //private void SerializeVaryColors( XmlWriter writer, ChartSerieImpl firstSeries )
    //{
    //  if( writer == null )
    //    throw new ArgumentNullException( "writer" );

    //  if( firstSeries == null )
    //    throw new ArgumentNullException( "firstSeries" );

    //  bool bVaryColor = firstSeries.SerieFormat.CommonSerieOptions.IsVaryColor;
    //  ChartSerializatorCommon.SerializeBoolValueTag( writer, ChartConstants.VaryColorsTag, bVaryColor );
    //}
    /// <summary>
    /// Parses single chart series of a bar chart.
    /// </summary>
    /// <param name="reader">XmlReader to extract series from.</param>
    /// <param name="chart">Parent chart object.</param>
    /// <param name="seriesType">Type of the series to create.</param>
    /// <returns>Extracted chart series.</returns>
    /// <param name="relations">Chart item relations.</param>
    private ChartSerieImpl ParseBarSeries( XmlReader reader, ChartImpl chart,
      ExcelChartType seriesType, RelationCollection relations )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      ChartSerieImpl series = ( ChartSerieImpl )chart.Series.Add( seriesType );
      ParseSeriesCommonWithoutEnd( reader, series, relations );
      object[] values;

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case ChartConstants.DataLabelsTag:
              ParseDataLabels( reader, series );
              break;

            case ChartConstants.TrendlineTag:
              ParseTrendlines( reader, series, relations );
              break;

            case ChartConstants.ErrorBarsTag:
              ParseErrorBars( reader, series, relations );
              break;

            case ChartConstants.CategoryValuesTag:
              series.CategoryLabels = ParseSeriesValues( reader, series, out values,false );

              if( values != null )
                series.EnteredDirectlyCategoryLabels = values;
              break;

            case ChartConstants.SeriesValuesTag:
              series.Values = ParseSeriesValues( reader, series, out values , true);

              if (values != null && series.Values == null)
                series.EnteredDirectlyValues = values;
              break;

            case ChartConstants.DataPointTag:
              ParseDataPoint( reader, series, relations );
              break;
              case Excel2007Serializator.Extensionlist:              
              ParseFilteredSeriesOrCategoryName(reader,series);             
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
      return series;
    }
    /// <summary>
    /// Parse Series or category name filter
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="series"></param>
    private void ParseFilteredSeriesOrCategoryName(XmlReader reader  ,ChartSerieImpl series)
    {
        if (reader == null)
            throw new ArgumentNullException("reader");
        object[] values;
        if (reader.LocalName != Excel2007Serializator.Extensionlist)
            throw new XmlException("Unexpected xml tag.");
        reader.Read();
        if (reader.LocalName == Excel2007Serializator.Extension)
        {
            while (reader.LocalName == Excel2007Serializator.Extension && reader.NodeType != XmlNodeType.EndElement)
            {
                if (reader.LocalName == Excel2007Serializator.Extension)
                {
                    reader.Read();

                    reader.Read();

                    if (reader.LocalName == ChartConstants.SeriesTextTag)
                    {
                        ParseSeriesText(reader, series);
                        //(series.ParentChart as ChartImpl).IsSeriesNameFiltered = true;
                        (series.ParentChart as ChartImpl).SeriesNameLevel = ExcelSeriesNameLevel.SeriesNameLevelNone;
                        reader.Skip();
                        reader.Read();
                    }
                    else if (reader.LocalName == ChartConstants.CategoryValuesTag)
                    {
                        series.CategoryLabels = ParseSeriesValues(reader, series, out values,false);

                        if (values != null)
                            series.EnteredDirectlyCategoryLabels = values;
                        //(series.ParentChart as ChartImpl).IsCategoryNameFiltered = true;
                        (series.ParentChart as ChartImpl).CategoryLabelLevel = ExcelCategoriesLabelLevel.CategoriesLabelLevelNone;
                        reader.Skip();
                        reader.Read();
                    }
                    //reader.Skip();
                    //reader.Read();
                }

            }
            reader.Read();
        }
        
    }
    /// <summary>
    /// Parses single chart series of a surface chart.
    /// </summary>
    /// <param name="reader">XmlReader to extract series from.</param>
    /// <param name="chart">Parent chart object.</param>
    /// <param name="seriesType">Type of the series to create.</param>
    /// <returns>Extracted chart series.</returns>
    /// <param name="relations">Chart item relations.</param>
    private ChartSerieImpl ParseSurfaceSeries( XmlReader reader, ChartImpl chart,
      ExcelChartType seriesType, RelationCollection relations )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      ChartSerieImpl series = ( ChartSerieImpl )chart.Series.Add( seriesType );
      ParseSeriesCommonWithoutEnd( reader, series, relations );
      object[] values;

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case ChartConstants.CategoryValuesTag:
              series.CategoryLabels = ParseSeriesValues( reader, series, out values , false);

              if( values != null )
                series.EnteredDirectlyCategoryLabels = values;
              break;

            case ChartConstants.SeriesValuesTag:
              series.Values = ParseSeriesValues( reader, series, out values, true);

              if( values != null && series.Values==null )
                series.EnteredDirectlyValues = values;
              break;
            case Excel2007Serializator.Extensionlist:              
              ParseFilteredSeriesOrCategoryName(reader, series);
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
      return series;
    }
    /// <summary>
    /// Extracts single chart series for pie chart.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="chart">Chart to put extracted data into.</param>
    /// <param name="seriesType">Type of the series to extract.</param>
    /// <param name="relations">Chart item relations.</param>
    /// <returns>Extracted series.</returns>
    private ChartSerieImpl ParsePieSeries( XmlReader reader, ChartImpl chart,
      ExcelChartType seriesType, RelationCollection relations )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      ChartSerieImpl series = ( ChartSerieImpl )chart.Series.Add( seriesType );
      ParseSeriesCommonWithoutEnd( reader, series, relations );
      object[] values;

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case ChartConstants.PieExplosionTag:
              series.SerieFormat.Percent = ChartParserCommon.ParseIntValueTag( reader );
              break;

            case ChartConstants.DataLabelsTag:
              ParseDataLabels( reader, series );
              break;

            case ChartConstants.TrendlineTag:
              ParseTrendlines( reader, series, relations );
              break;

            case ChartConstants.ErrorBarsTag:
              ParseErrorBars( reader, series, relations );
              break;

            case ChartConstants.CategoryValuesTag:
              series.CategoryLabels = ParseSeriesValues( reader, series, out values, false );

              if( values != null )
                series.EnteredDirectlyCategoryLabels = values;
              break;

            case ChartConstants.SeriesValuesTag:
              series.Values = ParseSeriesValues( reader, series, out values, true );

              if( values != null && series.Values==null)
                series.EnteredDirectlyValues = values;
              break;

            case ChartConstants.DataPointTag:
              ParseDataPoint( reader, series, relations );
              break;
            case Excel2007Serializator.Extensionlist:              
              ParseFilteredSeriesOrCategoryName(reader, series);
              break;
            default:
              reader.Skip();
              break;
          }
        }
      }

      reader.Read();
      return series;
    }
    /// <summary>
    /// Extracts line series.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="chart">Chart to put extracted series into.</param>
    /// <param name="seriesType">Type of the extracted series.</param>
    /// <param name="relations">Chart item relations.</param>
    /// <returns>Extracted series.</returns>
    private ChartSerieImpl ParseLineSeries( XmlReader reader, ChartImpl chart,
      ExcelChartType seriesType, RelationCollection relations, Excel2007Parser parser )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      ChartSerieImpl series = ( ChartSerieImpl )chart.Series.Add( seriesType );
      ParseSeriesCommonWithoutEnd( reader, series, relations );
      FileDataHolder dataHolder = series.ParentBook.DataHolder;
      object[] values;

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case Drawings.ShapePropertiesTag:
              ChartFillObjectGetterAny getter = new ChartFillObjectGetterAny(
                series.SerieFormat.LineProperties as ChartBorderImpl, null, null,
                series.SerieFormat.Shadow as ShadowImpl, series.SerieFormat.ThreeD as ThreeDFormatImpl);
              ChartParserCommon.ParseShapeProperties( reader, getter, dataHolder, relations );
              break;

            case ChartConstants.MarkerTag:
              ParseMarker( reader, series, parser );
              break;

            case ChartConstants.DataLabelsTag:
              ParseDataLabels( reader, series );
              break;

            case ChartConstants.TrendlineTag:
              ParseTrendlines( reader, series, relations );
              break;

            case ChartConstants.ErrorBarsTag:
              ParseErrorBars( reader, series, relations );
              break;

            case ChartConstants.CategoryValuesTag:
              series.CategoryLabels = ParseSeriesValues( reader, series, out values, false );

              if( values != null )
                series.EnteredDirectlyCategoryLabels = values;
              break;

            case ChartConstants.SeriesValuesTag:
              series.Values = ParseSeriesValues( reader, series, out values, true );

              if( values != null && series.Values==null )
                series.EnteredDirectlyValues = values;
              break;

            case ChartConstants.DataPointTag:
              ParseDataPoint( reader, series, relations );
              break;
            case Excel2007Serializator.Extensionlist:              
              ParseFilteredSeriesOrCategoryName(reader, series);
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

      return series;
    }
    /// <summary>
    /// Extracts scatter series.
    /// </summary>
    /// <param name="reader">XmlReader to extract series data from.</param>
    /// <param name="chart">Chart to put extracted series into.</param>
    /// <param name="seriesType">Type of the series to extract.</param>
    /// <param name="relations">Chart item relations.</param>
    /// <returns>Extracted series.</returns>
    private ChartSerieImpl ParseScatterSeries( XmlReader reader, ChartImpl chart,
      ExcelChartType seriesType, RelationCollection relations, Excel2007Parser parser )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      ChartSerieImpl series = ( ChartSerieImpl )chart.Series.Add( seriesType );
      ParseSeriesCommonWithoutEnd( reader, series, relations );
      object[] values;

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case Drawings.ShapePropertiesTag:
              ChartSerieDataFormatImpl dataFormat = ( ChartSerieDataFormatImpl )series.SerieFormat;
              FileDataHolder dataHolder = series.ParentBook.DataHolder;
              ChartFillObjectGetter getter = new ChartFillObjectGetter( dataFormat );
              ChartParserCommon.ParseShapeProperties( reader, getter, dataHolder, relations );
              break;

            case ChartConstants.MarkerTag:
              ParseMarker( reader, series, parser );
              break;

            case ChartConstants.DataLabelsTag:
              ParseDataLabels( reader, series );
              break;

            case ChartConstants.TrendlineTag:
              ParseTrendlines( reader, series, relations );
              break;

            case ChartConstants.ErrorBarsTag:
              ParseErrorBars( reader, series, relations );
              break;

            case ChartConstants.XValues:
              series.CategoryLabels = ParseSeriesValues( reader, series, out values, false );

              if( values != null )
                series.EnteredDirectlyCategoryLabels = values;
              break;

            case ChartConstants.YValues:
              series.Values = ParseSeriesValues(reader, series, out values, false);

              if( values != null )
                series.EnteredDirectlyValues = values;
              break;

            case ChartConstants.SmoothTag:
              bool bSmoothed = ChartParserCommon.ParseBoolValueTag( reader );
              ChartSerieDataFormatImpl serieFormat = ( ChartSerieDataFormatImpl )series.SerieFormat;
              serieFormat.IsSmoothedLine = bSmoothed;
              break;

            case ChartConstants.DataPointTag:
              ParseDataPoint( reader, series, relations );
              break;
            case Excel2007Serializator.Extensionlist:              
              ParseFilteredSeriesOrCategoryName(reader, series);
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
      return series;
    }
    /// <summary>
    /// Extracts radar series.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="chart">Chart to put extracted data into.</param>
    /// <param name="seriesType">Type of the series to create.</param>
    /// <param name="relations">Chart item relations.</param>
    /// <returns>Extracted series.</returns>
    private ChartSerieImpl ParseRadarSeries( XmlReader reader, ChartImpl chart,
      ExcelChartType seriesType, RelationCollection relations, Excel2007Parser parser )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      ChartSerieImpl series = ( ChartSerieImpl )chart.Series.Add( seriesType );
      ParseSeriesCommonWithoutEnd( reader, series, relations );
      object[] values;

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case ChartConstants.MarkerTag:
              ParseMarker( reader, series, parser );
              break;

            case ChartConstants.DataLabelsTag:
              ParseDataLabels( reader, series );
              break;

            case ChartConstants.CategoryValuesTag:
              series.CategoryLabels = ParseSeriesValues( reader, series, out values, false );

              if( values != null )
                series.EnteredDirectlyCategoryLabels = values;
              break;

            case ChartConstants.SeriesValuesTag:
              series.Values = ParseSeriesValues( reader, series, out values, true);

              if( values != null && series.Values==null )
                series.EnteredDirectlyValues = values;
              break;

            case ChartConstants.DataPointTag:
              ParseDataPoint( reader, series, relations );
              break;
            case Excel2007Serializator.Extensionlist:              
              ParseFilteredSeriesOrCategoryName(reader, series);
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
      return series;
    }
    /// <summary>
    /// Extracts single chart series for bubble chart.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="chart">Chart to put extracted series into.</param>
    /// <returns>Extracted series.</returns>
    /// <param name="relations">Chart item relations.</param>
    private ChartSerieImpl ParseBubbleSeries( XmlReader reader, ChartImpl chart,
      RelationCollection relations )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      if( reader.LocalName != ChartConstants.SeriesTag )
        throw new XmlException( "Unexpected xml tag." );

      //reader.Read();
      ChartSerieImpl series = ( ChartSerieImpl )chart.Series.Add( ExcelChartType.Bubble );
      ParseSeriesCommonWithoutEnd( reader, series, relations );
      object[] values;

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case ChartConstants.DataLabelsTag:
              ParseDataLabels( reader, series );
              break;

            case ChartConstants.TrendlineTag:
              ParseTrendlines( reader, series, relations );
              break;

            case ChartConstants.ErrorBarsTag:
              ParseErrorBars( reader, series, relations );
              break;

            case ChartConstants.XValues:
              series.CategoryLabels = ParseSeriesValues( reader, series, out values, false );

              if( values != null )
                series.EnteredDirectlyCategoryLabels = values;
              break;

            case ChartConstants.YValues:
              series.Values = ParseSeriesValues(reader, series, out values, false);

              if( values != null )
                series.EnteredDirectlyValues = values;
              break;

            case ChartConstants.BubbleSize:
              series.Bubbles = ParseSeriesValues(reader, series, out values, false);

              if( values != null )
                series.EnteredDirectlyBubbles = values;
              break;

            case ChartConstants.Bubble3DTag:
              bool b3D = ChartParserCommon.ParseBoolValueTag( reader );

              if( b3D )
              {
                //series.SerieType = ExcelChartType.Bubble_3D;
                series.SerieFormat.Is3DBubbles = true;
              }
              break;

            case ChartConstants.DataPointTag:
              ParseDataPoint( reader, series, relations );
              break;
            case Excel2007Serializator.Extensionlist:              
              ParseFilteredSeriesOrCategoryName(reader, series);
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
      return series;
    }
    /// <summary>
    /// Extracts single chart series for area chart.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="chart">Chart to put extracted series into.</param>
    /// <param name="seriesType">Type of the series to extract.</param>
    /// <param name="relations">Chart item relations.</param>
    /// <returns>Extracted series.</returns>
    private ChartSerieImpl ParseAreaSeries( XmlReader reader, ChartImpl chart,
      ExcelChartType seriesType, RelationCollection relations, bool isPrimary )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      ExcelChartType chartType = chart.ChartType;

      if( chartType == seriesType || chartType == ExcelChartType.Combination_Chart )
        chartType = seriesType;

      ChartSerieImpl series = ( ChartSerieImpl )chart.Series.Add( chartType );

      if (!isPrimary & series.ParentSeries.Count > 1)
      {
        series.UsePrimaryAxis = isPrimary;
      }

      if( chartType != seriesType )
        series.SerieType = seriesType;

      ParseSeriesCommonWithoutEnd( reader, series, relations );
      object[] values;

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case ChartConstants.DataLabelsTag:
              ParseDataLabels( reader, series );
              break;

            case ChartConstants.TrendlineTag:
              ParseTrendlines( reader, series, relations );
              break;

            case ChartConstants.ErrorBarsTag:
              ParseErrorBars( reader, series, relations );
              break;

            case ChartConstants.CategoryValuesTag:
              series.CategoryLabels = ParseSeriesValues( reader, series, out values, false );

              if( values != null )
                series.EnteredDirectlyCategoryLabels = values;
              break;

            case ChartConstants.SeriesValuesTag:
              series.Values = ParseSeriesValues( reader, series, out values, true );

              if( values != null && series.Values==null )
                series.EnteredDirectlyValues = values;
              break;

            case ChartConstants.DataPointTag:
              ParseDataPoint( reader, series, relations );
              break;
            case Excel2007Serializator.Extensionlist:              
              ParseFilteredSeriesOrCategoryName(reader, series);
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
      return series;
    }
    /// <summary>
    /// Parses common part of the series.
    /// WARNING: this method doesn't call last Read(), so this call
    /// must be made by parent item after series parsing complete.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="series">Series to put extracted data into.</param>
    /// <param name="relations">Chart item relations.</param>
    private void ParseSeriesCommonWithoutEnd( XmlReader reader, ChartSerieImpl series,
      RelationCollection relations )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( series == null )
        throw new ArgumentNullException( "series" );

      if( reader.LocalName != ChartConstants.SeriesTag )
        throw new XmlException( "Unexpected xml tag." );

      reader.Read();
      bool bContinue = true;

      while( reader.NodeType != XmlNodeType.EndElement && bContinue )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case ChartConstants.IndexTag:
              string strIndex = ChartParserCommon.ParseValueTag( reader );
              //series.Index = int.Parse( strIndex );
              series.Number = int.Parse( strIndex );
              break;

            case ChartConstants.SeriesOrderTag:
              string strOrder = ChartParserCommon.ParseValueTag( reader );
              series.Index = int.Parse( strOrder );
              //series.Order = int.Parse( strOrder );
              break;

            case ChartConstants.SeriesTextTag:
              ParseSeriesText( reader, series );
              break;

            case Drawings.ShapePropertiesTag:
              ParseSeriesProperties( reader, series, relations );
              break;

            case ChartConstants.InvertIfNegative:
                if (reader.MoveToAttribute(ChartConstants.ValueAttribute))
                    series.InvertIfNegative = XmlConvert.ToBoolean(reader.Value);
                break;

            default:
              bContinue = false;
              break;
            //case ChartConstants.DataPointTag:
          }

          //ChartDataPointsCollection dataPoints = ( ChartDataPointsCollection )series.DataPoints;

          //// 1 - because default data point is always there.
          //if( dataPoints.DeninedDPCount > 0 )
          //{
          //  foreach( ChartDataPointImpl dataPoint in dataPoints )
          //  {
          //    if( !dataPoint.IsDefault )
          //    {
          //      SerializeDataPoint( writer, dataPoint );
          //    }
          //  }
          //}
        }
        else
        {
          reader.Skip();
        }
      }
    }
    /// <summary>
    /// Parses name of the series.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="series">Series to put extracted name into.</param>
    private void ParseSeriesText( XmlReader reader, ChartSerieImpl series )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( series == null )
        throw new ArgumentNullException( "series" );

      if( reader.LocalName != ChartConstants.SeriesTextTag )
        throw new XmlException( "Unexpected xml tag." );

      if( !reader.IsEmptyElement )
      {
        reader.Read();

        while( reader.NodeType != XmlNodeType.EndElement )
        {
          if( series.IsDefaultName && reader.NodeType == XmlNodeType.Element )
          {
            switch( reader.LocalName )
            {
              case ChartConstants.TextValueTag:
                series.Name = reader.ReadElementContentAsString();
                break;

              case ChartConstants.StringReferenceTag:
                series.Name = "=" + ParseStringReference( reader );
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
    /// Parses single data point.
    /// </summary>
    /// <param name="reader">XmlReader to extract from.</param>
    /// <param name="series">Series to put data point into.</param>
    /// <param name="relations">Parent relations.</param>
    private void ParseDataPoint( XmlReader reader, ChartSerieImpl series, RelationCollection relations )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( series == null )
        throw new ArgumentNullException( "series" );

      if( reader.LocalName != ChartConstants.DataPointTag )
        throw new XmlException();

      if( !reader.IsEmptyElement )
      {
        FileDataHolder dataHolder = series.ParentChart.ParentWorkbook.DataHolder;

        reader.Read();
        IChartDataPoint dataPoint = null;

        while( reader.NodeType != XmlNodeType.EndElement )
        {
          if( reader.NodeType == XmlNodeType.Element )
          {
            switch( reader.LocalName )
            {
              case ChartConstants.IndexTag:
                string strIndex = ChartParserCommon.ParseValueTag( reader );
                int index = int.Parse( strIndex );
                dataPoint = series.DataPoints[ index ];
                (dataPoint as ChartDataPointImpl).HasDataPoint = true;
                break;

              case Drawings.ShapePropertiesTag:
                {
                  if( dataPoint == null )
                    throw new XmlException();

                  ChartSerieDataFormatImpl dataFormat = dataPoint.DataFormat as ChartSerieDataFormatImpl;
                  dataFormat.HasLineProperties = true;
                  dataFormat.HasInterior = true;
                  dataFormat.IsParsed = true;
                  ChartFillObjectGetterAny getter = new ChartFillObjectGetterAny(
                    dataFormat.LineProperties as ChartBorderImpl,
                    dataFormat.Interior as ChartInteriorImpl,
                    dataFormat.Fill as IInternalFill,
                    dataFormat.Shadow as ShadowImpl,
                    dataFormat.ThreeD as ThreeDFormatImpl );
                  ChartParserCommon.ParseShapeProperties( reader, getter, dataHolder, relations );

                  //ChartSerializatorCommon.SerializeFrameFormat( writer, dataFormat,
                  //  dataFormat.ParentChart, false );
                }
                break;

              case ChartConstants.MarkerTag:
                {
                  ChartSerieDataFormatImpl dataFormat = dataPoint.DataFormat as ChartSerieDataFormatImpl;
                  ParseMarker( reader, dataFormat, dataHolder.Parser );
                  dataPoint.IsDefaultmarkertype = true;
                  dataFormat.IsParsed = true;
                }
                break;
              case ChartConstants.InvertIfNegative:
                if (reader.MoveToAttribute(ChartConstants.ValueAttribute))
                {
                    ChartSerieDataFormatImpl dataFormat = dataPoint.DataFormat as ChartSerieDataFormatImpl;
                    series.ParentChart.IsParsed = true;
                    (dataFormat.Fill as ChartFillImpl).InvertIfNegative = XmlConvert.ToBoolean(reader.Value);
                    series.ParentChart.IsParsed = false;
                    dataFormat.IsParsed = true;
                }
                break;

              case ChartConstants.Bubble3DTag:
                {
                    (dataPoint as ChartDataPointImpl).Bubble3D = ChartParserCommon.ParseBoolValueTag(reader);
                    ChartSerieDataFormatImpl dataFormat = dataPoint.DataFormat as ChartSerieDataFormatImpl;
                    dataFormat.IsParsed = true;
                }
                break;

              case ChartConstants.PieExplosionTag:
                {
                    (dataPoint as ChartDataPointImpl).Explosion = ChartParserCommon.ParseIntValueTag(reader);
                    ChartSerieDataFormatImpl dataFormat = dataPoint.DataFormat as ChartSerieDataFormatImpl;
                    dataFormat.IsParsed = true;
                }
                break;

              default:
                // TODO: add marker parsing here.
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
    ///// <summary>
    ///// Serializes series values.
    ///// </summary>
    ///// <param name="writer">XmlWriter to serialize into.</param>
    ///// <param name="series">Series to serialize values for.</param>
    //private void SerializeSeriesValues( XmlWriter writer, ChartSerieImpl series )
    //{
    //  SerializeSeriesValues( writer, series.Values, ChartConstants.SeriesValuesTag );
    //}
    /// <summary>
    /// Extracts series values.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="series">Series to parse values for.</param>
    /// <param name="values">Array of series values.</param>
    /// <param name="isValueAxis">Indicates whether axis is ValueAxis or CategoryAxis.</param>
    /// <returns>Range referencing series values.</returns>
    private IRange ParseSeriesValues(XmlReader reader, ChartSerieImpl series, out object[] values,bool isValueAxis)
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( series == null )
        throw new ArgumentNullException( "series" );

      reader.Read();
      string strValue = null;
      string FilteredRange = null;
      string filteredvalue = null;
      values = null;
      bool numRef=false;
      bool strRef=false;
      bool mulRef=false;

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case ChartConstants.NumberReferenceTag:
              numRef = true;
              strValue = ParseNumReference(reader, out values,out filteredvalue);
              if (strValue.Split(',').Length > 1)
                  series.NumRefFormula = strValue;
              series.FilteredValue = filteredvalue;
              break;

            case ChartConstants.StringReferenceTag:
              strRef = true;                  
              strValue = ParseStringReference( reader ,out FilteredRange);
              if (strValue.Split(',').Length > 1)
                  series.StrRefFormula = strValue;
              series.FilteredCategory = FilteredRange;
              break;

            case ChartConstants.MultiLevelStringReferenceTag:
              mulRef = true;
              strValue = ParseMultiLevelStringReference( reader );
              if (strValue != null)
                  series.MulLvlStrRefFormula = strValue;
              break;

            case ChartConstants.NumberLiteral:
              values = ParseDirectlyEnteredValues( reader );
              break;

            case ChartConstants.StringLiteral:
              values = ParseDirectlyEnteredValues( reader );
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

      if( strValue != null && strValue.StartsWith( "(" ) && strValue.EndsWith( ")" ) )
        strValue = strValue.Substring( 1, strValue.Length - 2 );

      WorksheetImpl sheet = series.ParentBook.Worksheets[ 0 ] as WorksheetImpl;

      IRange result = null;

      if (strValue != null)
      {
          WorkbookImpl bookImpl = series.ParentBook as WorkbookImpl;
          FormulaUtil formulaUtil = bookImpl.DataHolder.Parser.FormulaUtil;
          Ptg[] formula = formulaUtil.ParseString(strValue);
          IRangeGetter rangeHolder = formula[0] as IRangeGetter;
          result= rangeHolder.GetRange(bookImpl, bookImpl.Worksheets[0]);
          if (result != null)
          {
              if (result is ExternalRange)
              {
                  (result as ExternalRange).IsNumReference = numRef;
                  (result as ExternalRange).IsStringReference = strRef;
                  (result as ExternalRange).IsMultiReference = mulRef;
              }
              else if(result is RangeImpl)
              {
                  (result as RangeImpl).IsNumReference = numRef;
                  (result as RangeImpl).IsStringReference = strRef;
                  (result as RangeImpl).IsMultiReference = mulRef;
              }
              else if (result is NameImpl)
              {
                  (result as NameImpl).IsNumReference = numRef;
                  (result as NameImpl).IsStringReference = strRef;
                  (result as NameImpl).IsMultiReference = mulRef;
              }
          }
      }
      if (result != null && isValueAxis)
      {
          ChartImpl chart = series.ParentChart as ChartImpl;
          int count = chart.Categories.Count;
          if (count < result.Count)
          {
              for (int i = count; i < result.Count; i++)
              {
                  (chart.Categories as ChartCategoryCollection).Add();
              }
          }
      }
      return result;
    }
    /// <summary>
    /// Extracts null reference tag.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <returns>String which is enclosed inside the formula tag.</returns>
    private string ParseNumReference( XmlReader reader,out object[] values )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( reader.LocalName != ChartConstants.NumberReferenceTag )
        throw new XmlException( "Unexpected xml tag." );

      reader.Read();
      string strResult = null;
      values = null;
      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case ChartConstants.Formula:
                  if (strResult == null)
                  {
                      strResult = reader.ReadElementContentAsString();
                  }
                  else
                  {
                      reader.Skip();
                  }
              break;
            case ChartConstants.NumberCacheTag:
              values = ParseDirectlyEnteredValues(reader);
              if(values.Length ==0)
                 values=null;             
              break;
            case Excel2007Serializator.Extensionlist:
                reader.Read();
                        if (reader.LocalName == Excel2007Serializator.Extension)
                        {
                            reader.Read();                            
                            if (reader.LocalName == ChartConstants.fullReference)
                            {
                                reader.Read();
                                if (reader.LocalName == ChartConstants.SqureReference)
                                {
                                    strResult = reader.ReadElementContentAsString();
                                    reader.Skip();
                                }
                                reader.Skip();
                            }
                            if (reader.LocalName == ChartConstants.formulareference)
                            {
                                reader.Read();
                                if (reader.LocalName == ChartConstants.SqureReference)
                                {
                                    strResult = reader.ReadElementContentAsString();
                                    reader.Skip();
                                }
                                reader.Skip();
                            }
                            reader.Read();
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
        // TODO: we don't store cache and we don't support anything but formula here, this should be changed later.
      }

      reader.Read();
      return strResult;
    }
    /// <summary>
    /// Filtered Number Reference
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="values"></param>
    /// <param name="filteredvalue"></param>
    /// <returns></returns>
    private string ParseNumReference(XmlReader reader, out object[] values, out string filteredvalue)
    {
        if (reader == null)
            throw new ArgumentNullException("reader");

        if (reader.LocalName != ChartConstants.NumberReferenceTag)
            throw new XmlException("Unexpected xml tag.");

        reader.Read();
        string strResult = null;
        filteredvalue = null;
        values = null;
        while (reader.NodeType != XmlNodeType.EndElement)
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                switch (reader.LocalName)
                {
                    case ChartConstants.Formula:
                        if (strResult == null)
                        {
                            strResult = reader.ReadElementContentAsString();
                        }
                        else
                        {
                            filteredvalue = reader.ReadElementContentAsString();
                        }
                        break;
                    case ChartConstants.NumberCacheTag:
                        values = ParseDirectlyEnteredValues(reader);
                        if (values.Length == 0)
                            values = null;
                        break;
                    case Excel2007Serializator.Extensionlist:
                        reader.Read();
                        if (reader.LocalName == Excel2007Serializator.Extension)
                        {
                            reader.Read();
                            
                            if (reader.LocalName == ChartConstants.fullReference)
                            {
                                reader.Read();
                                if (reader.LocalName == ChartConstants.SqureReference)
                                {
                                    strResult = reader.ReadElementContentAsString();
                                    reader.Skip();
                                }
                                if (reader.LocalName != ChartConstants.formulareference)
                                reader.Skip();
                            }
                            if (reader.LocalName == ChartConstants.formulareference)
                            {
                                reader.Read();
                                if (reader.LocalName == ChartConstants.SqureReference)
                                {
                                    if (strResult == null)
                                    {
                                        strResult = reader.ReadElementContentAsString();
                                    }
                                    else
                                    {
                                        filteredvalue = reader.ReadElementContentAsString();
                                    }
                                    reader.Skip();
                                }
                                reader.Skip();
                            }
                            reader.Read();
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
            // TODO: we don't store cache and we don't support anything but formula here, this should be changed later.
        }

        reader.Read();
        return strResult;
    }
    /// <summary>
    /// Extracts string reference tag.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <returns>String which is enclosed inside the formula tag.</returns>
    private string ParseStringReference( XmlReader reader )
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
                  if (strResult == null)
                  {
                      strResult = reader.ReadElementContentAsString();
                  }
                  else
                  {
                      reader.Skip();
                  }
              break;
              case Excel2007Serializator.Extensionlist:
                  reader.Read();
                  if (reader.LocalName == Excel2007Serializator.Extension)
                  {
                      reader.Read();
                      
                      if (reader.LocalName == ChartConstants.fullReference)
                      {
                          reader.Read();
                          if (reader.LocalName == ChartConstants.SqureReference)
                          {
                              strResult = reader.ReadElementContentAsString();
                              reader.Skip();
                          }
                          reader.Skip();
                      }
                      if (reader.LocalName == ChartConstants.formulareference)
                      {
                          reader.Read();
                          if (reader.LocalName == ChartConstants.SqureReference)
                          {
                              strResult = reader.ReadElementContentAsString();
                              reader.Skip();
                          }
                          reader.Skip();
                      }                      
                      reader.Read();
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
        // TODO: we don't store cache and we don't support anything but formula here, this should be changed later.
      }

      reader.Read();
      return strResult;
    }
    /// <summary>
    /// Filtered series range
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="Filteredcategory"></param>
    /// <returns></returns>
    private string ParseStringReference(XmlReader reader,out string Filteredcategory)
    {
        if (reader == null)
            throw new ArgumentNullException("reader");

        if (reader.LocalName != ChartConstants.StringReferenceTag)
            throw new XmlException("Unexpected xml tag.");
        Filteredcategory = string.Empty;
        reader.Read();
        string strResult = null;

        while (reader.NodeType != XmlNodeType.EndElement)
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                switch (reader.LocalName)
                {
                    case ChartConstants.Formula:
                        if (strResult == null)
                        {
                            strResult = reader.ReadElementContentAsString();
                        }
                        else
                        {
                            
                            Filteredcategory = reader.ReadElementContentAsString();
                        }
                        break;
                    case Excel2007Serializator.Extensionlist:
                        reader.Read();
                        if (reader.LocalName == Excel2007Serializator.Extension)
                        {
                            reader.Read();
                            
                            if (reader.LocalName == ChartConstants.fullReference)
                            {
                                reader.Read();
                                if (reader.LocalName == ChartConstants.SqureReference)
                                {
                                    strResult = reader.ReadElementContentAsString();
                                    reader.Skip();
                                }
                                if(reader.LocalName!=ChartConstants.formulareference)
                                reader.Skip();
                            }
                            if (reader.LocalName == ChartConstants.formulareference)
                            {
                                reader.Read();
                                if (reader.LocalName == ChartConstants.SqureReference)
                                {
                                    if (strResult == null)
                                    {
                                        strResult = reader.ReadElementContentAsString();
                                    }
                                    else
                                    {
                                        Filteredcategory = reader.ReadElementContentAsString();
                                    }                                    
                                    reader.Skip();
                                }
                                reader.Skip();
                            }
                            reader.Read();
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
            // TODO: we don't store cache and we don't support anything but formula here, this should be changed later.
        }

        reader.Read();
        return strResult;
    }    
    /// <summary>
    /// Extracts multi level string reference tag.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <returns>String which is enclosed inside the formula tag.</returns>
    private string ParseMultiLevelStringReference(XmlReader reader)
    {
        if (reader == null)
            throw new ArgumentNullException("reader");

        if (reader.LocalName != ChartConstants.MultiLevelStringReferenceTag)
            throw new XmlException("Unexpected xml tag.");

        reader.Read();
        string strResult = null;

        while (reader.NodeType != XmlNodeType.EndElement)
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                switch (reader.LocalName)
                {
                    case ChartConstants.Formula:
                        strResult = reader.ReadElementContentAsString();
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
        return strResult;
    }
    ///// <summary>
    ///// Serializes chart axes.
    ///// </summary>
    ///// <param name="writer">XmlWriter to serialize into.</param>
    ///// <param name="chart">Chart to serializes axes of.</param>
    //private void SerializeAxes( XmlWriter writer, ChartImpl chart )
    //{
    //  // TODO: add support of secondary axes.

    //  ChartAxisSerializator serializator = new ChartAxisSerializator();

    //  if( chart.IsCategoryAxisAvail )
    //    serializator.SerializeAxis( writer, chart.PrimaryCategoryAxis );

    //  if( chart.IsValueAxisAvail )
    //    serializator.SerializeAxis( writer, chart.PrimaryValueAxis );

    //  if( chart.IsSecondaryValueAxisAvail )
    //    serializator.SerializeAxis( writer, chart.SecondaryValueAxis );

    //  if( chart.IsSecondaryCategoryAxisAvail )
    //    serializator.SerializeAxis( writer, chart.SecondaryCategoryAxis );

    //  if( chart.IsSeriesAxisAvail )
    //    serializator.SerializeAxis( writer, chart.PrimarySerieAxis );

    //}
    /// <summary>
    /// Extracts series marker from XmlReader.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="series">Series to serialize marker for.</param>
    private void ParseMarker( XmlReader reader, ChartSerieImpl series, Excel2007Parser parser )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( series == null )
        throw new ArgumentNullException( "series" );

      if( reader.LocalName != ChartConstants.MarkerTag )
        throw new XmlException( "Unexpected xml tag." );

      IChartSerieDataFormat dataFormat = series.SerieFormat;
      ParseMarker( reader, dataFormat, parser );
    }
    private void ParseMarker( XmlReader reader, IChartSerieDataFormat dataFormat, Excel2007Parser parser )
    {
        bool checkShapeproperties = true;
      if( !reader.IsEmptyElement )
      {
        reader.Read();
        Excel2007ChartMarkerType markerType = Excel2007ChartMarkerType.none;
       
        while( reader.NodeType != XmlNodeType.EndElement )
        {
          if( reader.NodeType == XmlNodeType.Element )
          {
            switch( reader.LocalName )
            {
              case ChartConstants.MarkerStyleTag:
                string strMarkerType = ChartParserCommon.ParseValueTag( reader );
                markerType = ( Excel2007ChartMarkerType )Enum.Parse(
                  typeof( Excel2007ChartMarkerType ), strMarkerType, false );

                //if( markerType != Excel2007ChartMarkerType.none )
                {
                  dataFormat.MarkerStyle = ( ExcelChartMarkerType )markerType;
                }
                (dataFormat as ChartSerieDataFormatImpl).HasMarkerProperties = true;
                checkShapeproperties = (dataFormat as ChartSerieDataFormatImpl).IsAutoMarker;
                break;

              case ChartConstants.MarkerSizeTag:
                dataFormat.MarkerSize = ChartParserCommon.ParseIntValueTag( reader );
                break;

              case Drawings.ShapePropertiesTag:
                checkShapeproperties = false;
                ParseMarkerFill( reader, dataFormat, parser );
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
      ((ChartSerieDataFormatImpl)dataFormat).MarkerFormat.IsAutoColor = checkShapeproperties;
      reader.Read();
    }
    /// <summary>
    /// Parser marker fill.
    /// </summary>
    /// <param name="reader">Reader to get fill information from.</param>
    /// <param name="serieDataFormat">Data format to put extracted marker information into.</param>
    /// <param name="parser">Parser to help with color parsing.</param>
    private void ParseMarkerFill( XmlReader reader, IChartSerieDataFormat serieDataFormat, Excel2007Parser parser )
    {
      string tagName = reader.LocalName;

      if( !reader.IsEmptyElement )
      {
        reader.Read();

        ChartSerieDataFormatImpl dataFormat = serieDataFormat as ChartSerieDataFormatImpl;

        while( reader.NodeType != XmlNodeType.EndElement && reader.LocalName != tagName )
        {
          if( reader.NodeType == XmlNodeType.Element )
          {
            switch( reader.LocalName )
            {
              case Drawings.SolidFillTag:
                ChartParserCommon.ParseSolidFill( reader, parser, dataFormat.MarkerBackColorObject );
                //ParseMarkerFill( reader, dataFormat.MarkerBackColorObject, parser );
                break;

              case Drawings.GradientFillTag:
                GradientStops fill = ChartParserCommon.ParseGradientFill( reader, parser );
                ChartMarkerFormatRecord markerFormat = dataFormat.MarkerFormat;
                markerFormat.IsAutoColor = false;
                markerFormat.IsNotShowInt = false;
                markerFormat.IsNotShowBrd = false;
                dataFormat.MarkerBackColorObject.CopyFrom( fill[ 0 ].ColorObject, true );
                dataFormat.MarkerGradient = fill;
                break;

              case Drawings.LineTag:
                dataFormat.MarkerFormat.HasLineProperties = true;
                dataFormat.MarkerFormat.IsNotShowBrd = !ParseMarkerLine( reader, dataFormat.MarkerForeColorObject, parser, dataFormat );
                break;

              case Drawings.NoFillTag:
                dataFormat.MarkerFormat.IsNotShowInt = true;
                reader.Read();
                break;

              case Drawings.EffectListTag:
                dataFormat.EffectListStream = ShapeParser.ReadNodeAsStream(reader);
                dataFormat.EffectListStream.Position = 0;
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
      //ChartParserCommon.ParseShapeProperties( series.SerieFormat.
      //if( !serieFormat.MarkerFormat.IsAutoColor )
      //{
      //  writer.WriteStartElement( Drawings.ShapePropertiesTag, ChartConstants.CNamespace );
      //  ChartSerializatorCommon.SerializeSolidFill( writer, serieFormat.MarkerForegroundColor, false );

      //  writer.WriteStartElement( Drawings.LineTag, Drawings.ANamespace );
      //  ChartSerializatorCommon.SerializeSolidFill( writer, serieFormat.MarkerBackgroundColor, false );
      //  writer.WriteEndElement();

      //  writer.WriteEndElement();
      //}
      //break;
      //throw new NotImplementedException();
      //Debug.Fail( "Method is not implemented" );
      //reader.Skip();
      //throw new Exception( "The method or operation is not implemented." );
    }

    private bool ParseMarkerLine( XmlReader reader, ColorObject color, Excel2007Parser parser ,ChartSerieDataFormatImpl format)
    {
      bool bResult = false;
      int alpha = ShapeFillImpl.MaxValue;
      Stream markerLineStream = ShapeParser.ReadNodeAsStream( reader );
      markerLineStream.Position = 0;
      reader = UtilityMethods.CreateReader( markerLineStream );
      format.MarkerLineStream = markerLineStream;

      if( !reader.IsEmptyElement )
      {
        reader.Read();

        while( reader.NodeType != XmlNodeType.EndElement )
        {
          if( reader.NodeType == XmlNodeType.Element )
          {
            switch( reader.LocalName )
            {
              case Drawings.SolidFillTag:
                ChartParserCommon.ParseSolidFill( reader, parser, color, out alpha );
                format.MarkerTransparency = alpha / ( float )ShapeFillImpl.MaxValue;
                bResult = true;
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
      return bResult;
    }
    /// <summary>
    /// Extracts up/down bars.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="series">Series to put up/down bars into.</param>
    /// <param name="relations">Chart item relations.</param>
    private void ParseUpDownBars( XmlReader reader, ChartSerieImpl series,
      RelationCollection relations )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( series == null )
        throw new ArgumentNullException( "series" );

      if( reader.LocalName != ChartConstants.UpDownBarsTag )
        throw new XmlException( "Unexpected xml tag." );

      ChartFormatImpl chartFormat = ( ChartFormatImpl )series.SerieFormat.CommonSerieOptions;
      FileDataHolder dataHolder = series.ParentBook.DataHolder;

      reader.Read();
      //int iGapWidth = 150;

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        //if( chartFormat.IsDropBar )
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            //case ChartConstants.GapWidthTag:
            //  iGapWidth = ChartParserCommon.ParseIntValueTag( reader );
            //  break;

            case ChartConstants.UpBarsTag:
              IChartDropBar upBar = chartFormat.FirstDropBar;
              ParseDropBar( reader, upBar, dataHolder, relations );
              break;

            case ChartConstants.DownBarsTag:
              IChartDropBar downBar = chartFormat.SecondDropBar;
              ParseDropBar( reader, downBar, dataHolder, relations );
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
    /// Extracts single drop bar (up or down bar).
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="dropBar">Drop bar to put extracted data into.</param>
    /// <param name="dataHolder">Parent file data holder.</param>
    /// <param name="relations">Chart item relations.</param>
    private void ParseDropBar( XmlReader reader, IChartDropBar dropBar,
      FileDataHolder dataHolder, RelationCollection relations )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( dropBar == null )
        throw new ArgumentNullException( "dropBar" );

      if( !reader.IsEmptyElement )
      {
        reader.Read();
        //writer.WriteStartElement( tagName, ChartConstants.CNamespace );

        while( reader.NodeType != XmlNodeType.EndElement )
        {
          if( reader.NodeType == XmlNodeType.Element && reader.LocalName == Drawings.ShapePropertiesTag )
          {
            ChartFillObjectGetterAny getter = new ChartFillObjectGetterAny(
              dropBar.LineProperties as ChartBorderImpl,
              dropBar.Interior as ChartInteriorImpl,
              dropBar.Fill as IInternalFill,
              dropBar.Shadow as ShadowImpl,
              dropBar.ThreeD as ThreeDFormatImpl );
            ChartParserCommon.ParseShapeProperties( reader, getter, dataHolder, relations );
          }
          else
          {
            reader.Skip();
          }
        }

      }

      reader.Read();
    }
    ///// <summary>
    ///// Serializes chartsheet into specified writer.
    ///// </summary>
    ///// <param name="writer">XmlWriter to serialize into.</param>
    ///// <param name="chart">Chart to serialize.</param>
    ///// <param name="drawingRelation">Id of the drawing relation with chart object..</param>
    //internal void SerializeChartsheet( XmlWriter writer, ChartImpl chart, string drawingRelation )
    //{
    //  if( writer == null )
    //    throw new ArgumentNullException( "writer" );

    //  if( chart == null )
    //    throw new ArgumentNullException( "chart" );

    //  if( drawingRelation == null || drawingRelation.Length == 0 )
    //    throw new ArgumentOutOfRangeException( "drawingRelation" );

    //  writer.WriteStartDocument();
    //  writer.WriteStartElement( ChartConstants.ChartsheetTag, Excel2007Serializator.XmlNamespaceMain );
    //  writer.WriteAttributeString( WorkbookXmlSerializator.DEF_XMLNS_PREF,
    //    Excel2007Serializator.RelationPrefix, null, Excel2007Serializator.RelationNamespace );

    //  // TODO: add chart page setup serialization.
    //  writer.WriteElementString( Excel2007Serializator.SheetLevelPropertiesTagName, string.Empty );
    //  //<sheetViews>
    //  //  <sheetView tabSelected="1" zoomScale="118" workbookViewId="0" zoomToFit="1"/>
    //  //</sheetViews>
    //  writer.WriteStartElement( Excel2007Serializator.SheetViewsTag );
    //  writer.WriteStartElement( Excel2007Serializator.SheetViewTag );
    //  //writer.WriteAttributeString( "tabSelected", "1" );
    //  writer.WriteAttributeString( Excel2007Serializator.WorkbookViewIdAttribute, "0" );
    //  writer.WriteEndElement();
    //  writer.WriteEndElement();

    //  Excel2007Serializator serializator = new Excel2007Serializator( ( WorkbookImpl )chart.Workbook );
    //  serializator.SerializePageMargins( writer, chart.PageSetup, new WorksheetPageSetupConstants() );

    //  writer.WriteStartElement( Drawings.DrawingTagName );

    //  writer.WriteAttributeString( Excel2007Serializator.RelationshipIdAttributeName,
    //    Excel2007Serializator.RelationNamespace, drawingRelation );

    //  writer.WriteEndElement();

    //  writer.WriteEndElement();
    //}
    ///// <summary>
    ///// Serializes chartsheet drawing part.
    ///// </summary>
    ///// <param name="writer">XmlWriter to serialize into.</param>
    ///// <param name="chart">Chart to serialize drawing for.</param>
    ///// <param name="strRelationId">Relation id of the drawing part.</param>
    //public void SerializeChartsheetDrawing( XmlWriter writer, ChartImpl chart,
    //  string strRelationId )
    //{
    //  if( writer == null )
    //    throw new ArgumentNullException( "writer" );

    //  if( chart == null )
    //    throw new ArgumentNullException( "chart" );

    //  writer.WriteStartDocument( true );
    //  writer.WriteStartElement( Drawings.XdrPreffix + ':' + Drawings.WorksheetDrawings );

    //  writer.WriteAttributeString( WorkbookXmlSerializator.DEF_XMLNS_PREF, Drawings.XdrPreffix,
    //    null, Drawings.XdrNamespace );

    //  writer.WriteAttributeString( WorkbookXmlSerializator.DEF_XMLNS_PREF, Drawings.APreffix,
    //    null, Drawings.ANamespace );

    //  writer.WriteStartElement( Drawings.AbsoluteAnchorTag, Drawings.XdrNamespace );

    //  writer.WriteStartElement( Drawings.PositionTag, Drawings.XdrNamespace );
    //  writer.WriteAttributeString( Drawings.XAttributeName, "0" );
    //  writer.WriteAttributeString( Drawings.YAttributeName, "0" );
    //  writer.WriteEndElement();

    //  writer.WriteStartElement( Drawings.Extents, Drawings.XdrNamespace );
    //  writer.WriteAttributeString( Drawings.CXAttributeName, "9298983" );
    //  writer.WriteAttributeString( Drawings.CYAttributeName, "6078242" );
    //  writer.WriteEndElement();

    //  writer.WriteStartElement( Drawings.GraphicFrame, Drawings.XdrNamespace );
    //  writer.WriteAttributeString( Drawings.MacroAttribute, string.Empty );

    //  writer.WriteStartElement( Drawings.NonVisualGraphicFramePr, Drawings.XdrNamespace );

    //  writer.WriteStartElement( Drawings.NVCanvasPropertiesTag, Drawings.XdrNamespace );
    //  writer.WriteAttributeString( Drawings.IdAttributeName, "2" );
    //  writer.WriteAttributeString( Drawings.NameAttributeName, chart.Name );
    //  writer.WriteEndElement();

    //  writer.WriteStartElement( Drawings.CNVGraphicFramePr, Drawings.XdrNamespace );
    //  writer.WriteStartElement( Drawings.GraphicFrameLocksTag, Drawings.ANamespace );
    //  writer.WriteAttributeString( Drawings.NoShapeGrouping, "1" );
    //  writer.WriteEndElement();
    //  writer.WriteEndElement();

    //  writer.WriteEndElement();

    //  DrawingShapeSerializator.SerializeForm( writer, Drawings.XdrNamespace,
    //    Drawings.ANamespace, 0, 0, 0, 0 );

    //  writer.WriteStartElement( Drawings.GraphicTag, Drawings.ANamespace );
    //  writer.WriteStartElement( Drawings.GraphicDataTag, Drawings.ANamespace );
    //  writer.WriteAttributeString( Drawings.UriAttribute, ChartConstants.CNamespace );
    //  writer.WriteStartElement( ChartConstants.CPrefix, Drawings.ChartTag, ChartConstants.CNamespace );

    //  writer.WriteAttributeString( Drawings.IdAttributeName, Excel2007Serializator.RelationNamespace,
    //    strRelationId );

    //  writer.WriteEndElement();
    //  writer.WriteEndElement();
    //  writer.WriteEndElement();
    //  writer.WriteEndElement();

    //  writer.WriteElementString( Drawings.ClientDataTagName, Drawings.XdrNamespace, string.Empty );

    //  writer.WriteEndElement();

    //  writer.WriteEndElement();
    //}
    /// <summary>
    /// This method extracts chart data table.
    /// </summary>
    /// <param name="reader">XmlReader to extract data table from.</param>
    /// <param name="chart">Chart to put extracted data into.</param>
    private void ParseDataTable( XmlReader reader, ChartImpl chart )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      if( reader.LocalName != ChartConstants.DataTableTag )
        throw new XmlException( "Unexpected xml tag." );

      Excel2007Parser parser = chart.ParentWorkbook.DataHolder.Parser;
      chart.HasDataTable = true;

      if( !reader.IsEmptyElement )
      {
        reader.Read();
        IChartDataTable dataTable = chart.DataTable;

        while( reader.NodeType != XmlNodeType.EndElement )
        {
          if( reader.NodeType == XmlNodeType.Element )
          {
            switch( reader.LocalName )
            {
              case ChartConstants.ShowHorizontalBorder:
                dataTable.HasHorzBorder = ChartParserCommon.ParseBoolValueTag( reader );
                break;

              case ChartConstants.ShowVerticalBorder:
                dataTable.HasVertBorder = ChartParserCommon.ParseBoolValueTag( reader );
                break;

              case ChartConstants.ShowOutline:
                dataTable.HasBorders = ChartParserCommon.ParseBoolValueTag( reader );
                break;

              case ChartConstants.ShowSeriesKeys:
                dataTable.ShowSeriesKeys = ChartParserCommon.ParseBoolValueTag( reader );
                break;

              case ChartConstants.TextPropertiesTag:
                IInternalChartTextArea textArea = dataTable.TextArea as IInternalChartTextArea;
                ParseDefaultTextFormatting(reader, textArea, parser);
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
    /// Extracts series formatting options.
    /// </summary>
    /// <param name="reader">XmlReader to extract formatting from.</param>
    /// <param name="series">Series to extract formatting from.</param>
    /// <param name="relations">Chart item relations.</param>
    private void ParseSeriesProperties( XmlReader reader, ChartSerieImpl series, RelationCollection relations )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( series == null )
        throw new ArgumentNullException( "series" );

      if( reader.LocalName != Drawings.ShapePropertiesTag )
        throw new XmlException( "Unexpected xml tag" );

      ChartSerieDataFormatImpl dataFormat = ( ChartSerieDataFormatImpl )series.SerieFormat;
      FileDataHolder dataHolder = series.ParentChart.DataHolder.ParentHolder;
      ChartFillObjectGetter objectGetter = new ChartFillObjectGetter( dataFormat );
      ChartParserCommon.ParseShapeProperties( reader, objectGetter, dataHolder, relations );
    }
    /// <summary>
    /// Extracts default text formatting.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="textFormatting">Object with text formatting.</param>
    /// <param name="parser">Excel2007Parser to use if necessary.</param>
    private void ParseDefaultTextFormatting( XmlReader reader,
      IInternalChartTextArea textFormatting, Excel2007Parser parser )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( textFormatting == null )
        throw new ArgumentNullException( "textFormatting" );

      if( reader.LocalName != ChartConstants.TextPropertiesTag )
        throw new XmlException( "Unexpected xml tag" );
      textFormatting.ParagraphType = ChartParagraphType.CustomDefault;
      reader.Read();
      while (reader.NodeType != XmlNodeType.EndElement)
      {
          if (reader.NodeType == XmlNodeType.Element)
          {
              switch (reader.LocalName)
              {
                  case Drawings.TextBodyPropertiesTag:
                      if (reader.MoveToAttribute(Drawings.TextRotationAttribute))
                          textFormatting.TextRotationAngle = XmlConvert.ToInt32(reader.Value) / ChartAxisSerializator.TextRotationMultiplier;                      
                      reader.Skip();
                      break;
                  case Drawings.DefaultParagraphProperites:
                      ChartParserCommon.ParseParagraphRunProperites(reader, textFormatting, parser, null);
                      while (reader.LocalName != ChartConstants.TextPropertiesTag)
                          reader.Read();
                      break;
                  case Drawings.Paragraphs:
                      while (reader.NodeType != XmlNodeType.EndElement
                              && reader.LocalName != ChartConstants.TextPropertiesTag
                              && reader.LocalName != Drawings.DefaultParagraphProperites)
                      {
                          reader.Read();
                      }
                      if (reader.LocalName == Drawings.DefaultParagraphProperites)
                      {
                          ChartParserCommon.ParseParagraphRunProperites(reader, textFormatting, parser, null);

                          while (reader.LocalName != ChartConstants.TextPropertiesTag)
                              reader.Read();
                      }

                      break;

                  default:
                      reader.Skip();
                      break;
              }
          }
          else
              reader.Read();
      }      

      reader.Read();
    }
    private object[] ParseDirectlyEnteredValues( XmlReader reader )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      // Skip top level tag.
      reader.Read();
      List<object> list = new List<object>();
      if (reader.NodeType == XmlNodeType.EndElement)
          return list.ToArray();
      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case ChartConstants.NumericPoint:
              AddNumericPoint( reader, list );
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

      // Skip end tag.
      reader.Read();
      return list.ToArray();
    } 

    private void AddNumericPoint( XmlReader reader, List<object> list )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( list == null )
        throw new ArgumentNullException( "list" );

      if( reader.LocalName != ChartConstants.NumericPoint )
        throw new XmlException();

      if( reader.MoveToAttribute( ChartConstants.IndexTag ) )
      {
        int index = XmlConvert.ToInt32( reader.Value );
      }

      if( !reader.IsEmptyElement )
      {
        reader.Read();

        while( reader.NodeType != XmlNodeType.EndElement )
        {
          if( reader.NodeType == XmlNodeType.Element )
          {
            switch( reader.LocalName )
            {
              case ChartConstants.NumbericValue:
                // NOTE: on the current moment we ignore index.
                list.Add( ReadXmlValue( reader ) );
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
    /// Extracts number or string value from the reader.
    /// </summary>
    /// <param name="reader">XmlReader to get value from.</param>
    /// <returns>Extracted object.</returns>
    private object ReadXmlValue( XmlReader reader )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      string strValue = reader.ReadElementContentAsString();

      double dResult;
      object result;

      System.Globalization.NumberStyles style = NumberStyles.Number;

      if (double.TryParse(strValue, style, CultureInfo.InvariantCulture, out dResult))
      {
          result = dResult;
      }
      else
      {
          result = strValue;
      }

      return result;
    }
    #endregion
  }
}
