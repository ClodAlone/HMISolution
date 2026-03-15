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
using Syncfusion.XlsIO.Implementation.Charts;
using Syncfusion.XlsIO.Implementation.XmlSerialization.Constants;
using Syncfusion.XlsIO.Implementation.XmlSerialization.Shapes;

using Syncfusion.XlsIO.Implementation.Collections;
using System.IO;
using Syncfusion.XlsIO.Implementation.XmlReaders.Shapes;
using Syncfusion.XlsIO.Interfaces.Charts;
using Syncfusion.XlsIO.Implementation.PivotTables;

#if ( WINRT )
using Windows.UI;
using Rectangle = Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
using Syncfusion.XlsIO.Implementation.WINRT;
#endif

#if  (SILVERLIGHT)
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.Silverlight;
#elif WP
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.WP;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;


using Syncfusion.XlsIO.Parser.Biff_Records.Charts;
using Syncfusion.XlsIO.Implementation.Shapes;
#endif

namespace Syncfusion.XlsIO.Implementation.XmlSerialization.Charts
{
  /// <summary>
  /// This class is responsible for chart object serialization into XmlWriter in Excel 2007 SpreadsheetML format.
  /// </summary>
  public class ChartSerializator
  {
    #region Constants
    public const int DefaultExtentX = 8666049;
    public const int DefaultExtentY = 6293304;
    public int categoryFilter = 0;
    public bool findFilter;
    #endregion

    #region Delegates
    /// <summary>
    /// This delegate is used for series serialization.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize series into.</param>
    /// <param name="series">Series to serialize.</param>
    private delegate void SerializeSeriesDelegate( XmlWriter writer, ChartSerieImpl series );
    #endregion

    #region Methods
    /// <summary>
    /// Serializes chart inside XmlWriter.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="chart">Chart to serialize.</param>
    /// <param name="chartItemName">Name of the xml file containing chart item.</param>
    public void SerializeChart( XmlWriter writer, ChartImpl chart, string chartItemName )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      writer.WriteStartElement( ChartConstants.CPrefix, ChartConstants.ChartSpaceTag, ChartConstants.CNamespace );

      writer.WriteAttributeString( WorkbookXmlSerializator.DEF_XMLNS_PREF, Drawings.APreffix,
        null, Drawings.ANamespace );

      writer.WriteAttributeString( WorkbookXmlSerializator.DEF_XMLNS_PREF, Excel2007Serializator.RelationPrefix,
        null, Excel2007Serializator.RelationNamespace );

      if (chart.HasPlotArea && chart.PlotArea.IsBorderCornersRound)
      {
          ChartSerializatorCommon.SerializeBoolValueTag(writer, ChartConstants.RoundedCornersTag, true);
      }
      else
      {
          ChartSerializatorCommon.SerializeBoolValueTag(writer, ChartConstants.RoundedCornersTag, false);
      }

      if( chart.AlternateContent != null )
      {
        chart.AlternateContent.Position = 0;
        ShapeParser.WriteNodeFromStream( writer, chart.AlternateContent );
      }

      if( chart.Style > 0 )
        ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.ChartStyleTag, chart.Style.ToString() );

      SerializePivotSource( writer, chart );

      if( chart.InnerProtection != ExcelSheetProtection.None )
        writer.WriteElementString( "protection", ChartConstants.CNamespace, string.Empty );

      writer.WriteStartElement( ChartConstants.ChartTag, ChartConstants.CNamespace );

      WorksheetDataHolder sheetHolder = chart.DataHolder;
      FileDataHolder holder = sheetHolder.ParentHolder;
      RelationCollection relations = chart.Relations;

      if (chart.HasTitle)
          ChartSerializatorCommon.SerializeTextArea(writer, chart.ChartTitleArea, chart.ParentWorkbook, relations, ChartParser.DefaultTitleSize );
      
      if(chart.HasAutoTitle !=null)
      {
      writer.WriteStartElement(ChartConstants.AutoTitleDeletedTag, ChartConstants.CNamespace);
      writer.WriteAttributeString(ChartConstants.ValueAttribute,((bool)chart.HasAutoTitle)? "1":"0");
      writer.WriteEndElement();
      }

      

      SerializePivotFormats( writer, chart );

      if( chart.Series.Count > 0 || !chart.HasPivotSource )
      {
        SerializeView3D( writer, chart );
      }
      else
      {
        SerializePivotView3D( writer, chart );
      }

      if( chart.SupportWallsAndFloor )
      {
        // TODO: add walls/floor serialization
        if( chart.HasFloor )
          SerializeSurface( writer, chart.Floor, ChartConstants.FloorTag, chart );

        if (chart.HasWalls && (chart as WorksheetBaseImpl).ParentWorkbook.BeginVersion==1)
        {
            SerializeSurface(writer, chart.Walls, ChartConstants.SideWallTag, chart);
            SerializeSurface(writer, chart.Walls, ChartConstants.BackWallTag, chart);
        }
        else 
        {
            SerializeSurface(writer, chart.SideWall, ChartConstants.SideWallTag, chart);
            SerializeSurface(writer, chart.Walls, ChartConstants.BackWallTag, chart);
        }
      }

      SerializePlotArea( writer, chart, relations );

      if( chart.HasLegend )
        SerializeLegend( writer, chart.Legend, chart );

      // other tags: showDLblsOverMax
      if (chart.ShowPlotVisible || (chart.Workbook as WorkbookImpl).IsCreated)
          ChartSerializatorCommon.SerializeBoolValueTag( writer, ChartConstants.PlotVisibleOnlyTag, chart.PlotVisibleOnly );

      Excel2007ChartPlotEmpty plotEmpty = ( Excel2007ChartPlotEmpty )chart.DisplayBlanksAs;
      ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.DisplayBlanksAsTag,
        plotEmpty.ToString() );
      
      writer.WriteEndElement();

      if( chart.HasChartArea )
      {
        IChartFrameFormat format = chart.ChartArea;

        if( format != null )
        {
          ChartSerializatorCommon.SerializeFrameFormat( writer, format, chart, format.IsBorderCornersRound );
        }
      }


      SerializeDefaultTextProperties(writer, chart);

      if (chart.IsEmbeded)
      {
          SerializePrinterSettings(writer, chart, relations);
      }
      SerializeShapes(writer, chart, chartItemName);
      SerializePivotOptions(writer, chart);
      writer.WriteEndElement();
        }
      /// <summary>
      ///  Serializes the Chart default text properties.
      /// </summary>
      /// <param name="writer">XmlWriter to serializes into.</param>
      /// <param name="chart">chart to get the data to serialize.</param>
    private void SerializeDefaultTextProperties(XmlWriter writer, ChartImpl chart)
    {
        if (writer == null)
            throw new ArgumentNullException("writer");

        if (chart == null)
            throw new ArgumentNullException("chart");

        Stream stream = chart.DefaultTextProperty;
        if (stream != null)
        {
            stream.Position = 0;
            ShapeParser.WriteNodeFromStream(writer, stream);
        }
    }
        /// <summary>
        /// Serializes the pivot options.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="chart">The chart.</param>
        private void SerializePivotOptions(XmlWriter writer, ChartImpl chart)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");
            if (chart == null)
                throw new ArgumentNullException("chart");

          if( !chart.HasPivotSource )
              return;


            writer.WriteStartElement(ChartConstants.CPrefix, Excel2007Serializator.Extensionlist, ChartConstants.CNamespace);
            writer.WriteStartElement(ChartConstants.CPrefix, Excel2007Serializator.Extension, ChartConstants.CNamespace);
            writer.WriteAttributeString(Drawings.UriAttribute, ChartConstants.Pivoturi);
            writer.WriteAttributeString(WorkbookXmlSerializator.DEF_XMLNS_PREF, ChartConstants.ChartPrefix2010, null, ChartConstants.CNamespace2007);
            writer.WriteStartElement(ChartConstants.ChartPrefix2010, ChartConstants.PivotOptionsTag, ChartConstants.CNamespace2007);
            if (chart.ShowReportFilterFieldButtons)
            {
                writer.WriteStartElement(ChartConstants.ChartPrefix2010, ChartConstants.ShowZoneFilterTag, ChartConstants.CNamespace2007);
                writer.WriteAttributeString(ChartConstants.ValueAttribute, Excel2007Serializator.TrueValue);
                writer.WriteEndElement();
            }

            if (chart.ShowAxisFieldButtons)
            {
                writer.WriteStartElement(ChartConstants.ChartPrefix2010, ChartConstants.ShowZoneCategoryTag, ChartConstants.CNamespace2007);
                writer.WriteAttributeString(ChartConstants.ValueAttribute, Excel2007Serializator.TrueValue);
                writer.WriteEndElement();
            }
            if (chart.ShowValueFieldButtons)
            {
                writer.WriteStartElement(ChartConstants.ChartPrefix2010, ChartConstants.ShowZoneDataTag, ChartConstants.CNamespace2007);
                writer.WriteAttributeString(ChartConstants.ValueAttribute, Excel2007Serializator.TrueValue);
                writer.WriteEndElement();
            }
            if (chart.ShowLegendFieldButtons)
            {
                writer.WriteStartElement(ChartConstants.ChartPrefix2010, ChartConstants.ShowZoneSeriesTag, ChartConstants.CNamespace2007);
                writer.WriteAttributeString(ChartConstants.ValueAttribute, Excel2007Serializator.TrueValue);
                writer.WriteEndElement();
            }
            if (chart.ShowAllFieldButtons)
            {
                writer.WriteStartElement(ChartConstants.ChartPrefix2010, ChartConstants.ShowZoneVisibleTag, ChartConstants.CNamespace2007);
                writer.WriteAttributeString(ChartConstants.ValueAttribute, Excel2007Serializator.TrueValue);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();

    }
    /// <summary>
    /// Serializes pivot source tag if necessary.
    /// </summary>
    /// <param name="writer">XmlWriter to serilaize into.</param>
    /// <param name="chart">Chart to serialize pivot formats tag for.</param>
    private void SerializePivotFormats( XmlWriter writer, ChartImpl chart )
    {
      if( chart == null )
        throw new ArgumentNullException( "chart" );

      if( writer == null )
        throw new ArgumentNullException( "writer" );

      Stream pivotFormats = chart.PivotFormatsStream;

      if( pivotFormats != null )
      {
        pivotFormats.Position = 0;
        ShapeParser.WriteNodeFromStream( writer, pivotFormats );
      }
    }
    /// <summary>
    /// Serializes pivot source tag if necessary.
    /// </summary>
    /// <param name="writer">XmlWriter to serilaize into.</param>
    /// <param name="chart">Chart to serialize pivot source tag for.</param>
    private void SerializePivotSource( XmlWriter writer, ChartImpl chart )
    {
      if( chart == null )
        throw new ArgumentNullException( "chart" );

      if( writer == null )
        throw new ArgumentNullException( "writer" );

      IPivotTable pivotSource = chart.PivotSource;
      string strPivotSource = chart.PreservedPivotSource;

      if( pivotSource != null )
        strPivotSource = GetPivotSource( pivotSource );

      if( strPivotSource != null )
      {
        writer.WriteStartElement( ChartConstants.CPrefix, ChartConstants.PivotSourceTag, ChartConstants.CNamespace );
        writer.WriteStartElement( ChartConstants.CPrefix, ChartConstants.TrendlineNameTag, ChartConstants.CNamespace );
        writer.WriteString( strPivotSource );
        writer.WriteEndElement();
        writer.WriteStartElement(ChartConstants.CPrefix,ChartConstants.ChartFormatId,ChartConstants .CNamespace);
        writer.WriteAttributeString(ChartConstants .ValueAttribute, chart.FormatId.ToString () );
        writer.WriteEndElement();
        writer.WriteEndElement();
      }
    }
    /// <summary>
    /// Gets the pivot source.
    /// </summary>
    /// <param name="pivotSheet">The pivot sheet.</param>
    /// <param name="pivotTable">The pivot table.</param>
    /// <returns></returns>
    private static string GetPivotSource( IPivotTable pivotTable )
    {
      if( pivotTable == null )
        throw new ArgumentNullException( "pivotTable" );

      PivotTableImpl table = pivotTable as PivotTableImpl;
      IWorksheet pivotSheet = table.Worksheet;
      StringBuilder pivotString = new StringBuilder();
      pivotString.Append( "[0]" );
      pivotString.Append( pivotSheet.Name );
      pivotString.Append( '!' );
      pivotString.Append( pivotTable.Name );

      return pivotString.ToString();

    }
    /// <summary>
    /// Serializes shapes.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="chart">Chart to serialize shapes for.</param>
    /// <param name="chartItemName">Name of the xml file containing chart item.</param>
    private void SerializeShapes( XmlWriter writer, ChartImpl chart, string chartItemName )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      if( chart.Shapes.Count - chart.VmlShapesCount <= 0 )
        return;

      WorksheetDataHolder holder = chart.DataHolder;
      RelationCollection relations = chart.Relations;
      string strDrawingsId = holder.DrawingsId;

      if( strDrawingsId == null )
      {
        holder.DrawingsId = strDrawingsId = relations.GenerateRelationId();
        relations[ strDrawingsId ] = null;// reserve relation.
      }

      if( chart.DataHolder.SerializeDrawings( chart, relations, ref strDrawingsId, ContentTypes.ChartDrawings, RelationTypes.ChartDrawings ) )
      {
        writer.WriteStartElement( ChartConstants.UserShapesTag, ChartConstants.CNamespace );
        writer.WriteAttributeString( Drawings.IdAttributeName, Excel2007Serializator.RelationNamespace,
          strDrawingsId );
        writer.WriteEndElement();
      }
      else
      {
        relations.Remove( strDrawingsId );
      }

      //holder.SerializeRelations( relations, chartItemName );
      //throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Serializes chart printer settings.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="pageSetup">Object that stores settings to serialize.</param>
    private void SerializePrinterSettings( XmlWriter writer, ChartImpl chart, RelationCollection relations )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      IChartPageSetup pageSetup = chart.PageSetup;

      writer.WriteStartElement( ChartConstants.PrintSettings, ChartConstants.CNamespace );

      IPageSetupConstantsProvider constants = new ChartPageSetupConstants();
      Excel2007Serializator.SerializePrintSettings( writer, pageSetup, constants,true);
      Excel2007Serializator.SerializeVmlHFShapesWorksheetPart( writer, chart, constants, relations );
      chart.DataHolder.SerializeHeaderFooterImages( chart, relations );
      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes chart legend.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="legend">Chart legend to serialize.</param>
    /// <param name="chart">Parent chart object.</param>
    private void SerializeLegend( XmlWriter writer, IChartLegend legend, ChartImpl chart )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( legend == null )
        throw new ArgumentNullException( "legend" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      writer.WriteStartElement( ChartConstants.LegendTag, ChartConstants.CNamespace );
      ExcelLegendPosition legendPosition = legend.Position;

      if( legendPosition != ExcelLegendPosition.NotDocked )
      {
        Excel2007LegendPosition legendPos = ( Excel2007LegendPosition )legendPosition;
        ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.LegendPositionTag,
          legendPos.ToString() );
      }

      // legend entry
      IChartLegendEntries arrEntries = legend.LegendEntries;
      IWorkbook book = chart.Workbook;

      for( int i = 0, len = arrEntries.Count; i < len; i++ )
      {
        SerializeLegendEntry( writer, arrEntries[ i ], i, book );
      }
      // layout
      IChartLayout layout = ( legend as ChartLegendImpl ).Layout;
      if( layout != null )
          ChartSerializatorCommon.SerializeLayout(writer, (legend as ChartLegendImpl));

      if (!legend.IncludeInLayout)
          ChartSerializatorCommon.SerializeValueTag(writer, ChartConstants.OverlayTag, "1");
      else
          ChartSerializatorCommon.SerializeValueTag(writer, ChartConstants.OverlayTag, "0");


      // TODO: overlay - we don't support it.
      // spPr
      ChartSerializatorCommon.SerializeFrameFormat( writer, legend.FrameFormat, chart, false );
      // txPr
      (legend as ChartLegendImpl).IsChartTextArea = true;
      bool bHasDefault = ((IInternalChartTextArea)legend.TextArea).ParagraphType == ChartParagraphType.CustomDefault;
      if(bHasDefault )
        SerializeDefaultTextFormatting( writer, legend.TextArea, book, ChartAxisParser.DefaultFontSize );
      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes single legend entry.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize legend entry into.</param>
    /// <param name="legendEntry">Legend entry to serialize.</param>
    /// <param name="index">Legend entry index.</param>
    /// <param name="book">Parent workbook object.</param>
    private void SerializeLegendEntry( XmlWriter writer, IChartLegendEntry legendEntry,
      int index, IWorkbook book )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( legendEntry == null )
        throw new ArgumentNullException( "legendEntry" );

      if( legendEntry.IsDeleted || legendEntry.IsFormatted )
      {
        writer.WriteStartElement( ChartConstants.LegendEntryTag, ChartConstants.CNamespace );
        ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.IndexTag, index.ToString() );

        if( legendEntry.IsDeleted )
        {
          ChartSerializatorCommon.SerializeBoolValueTag( writer, ChartConstants.DeleteTag, true );
        }
        else
        {
          // TODO: serialize legend entry properties.
          // legendEntry.TextArea;
        }

        if( legendEntry.IsFormatted )
        {
          SerializeDefaultTextFormatting( writer, legendEntry.TextArea, book, ChartAxisParser.DefaultFontSize );
        }

        writer.WriteEndElement();
      }
    }

        private void SerializePivotView3D(XmlWriter writer, ChartImpl chart)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (chart == null)
                throw new ArgumentNullException("chart");

            if (!chart.IsPivotChart3D)
                return;
            string rotationX = chart.PivotChartType == ExcelChartType.Surface_3D
                               || chart.PivotChartType == ExcelChartType.Surface_NoColor_3D
                                                  ? "15"
                                                  : "90";
            string rotationY = chart.PivotChartType == ExcelChartType.Surface_3D
                             || chart.PivotChartType == ExcelChartType.Surface_NoColor_3D
                                                     ? "20"
                                                     : "0";
            string perspective = chart.PivotChartType == ExcelChartType.Surface_3D
                       || chart.PivotChartType == ExcelChartType.Surface_NoColor_3D
                                                ? "30"
                                                : "0";
            writer.WriteStartElement(ChartConstants.View3DTag, ChartConstants.CNamespace);
            ChartSerializatorCommon.SerializeValueTag(writer, ChartConstants.RotationXTag, rotationX);
            ChartSerializatorCommon.SerializeValueTag(writer, ChartConstants.RotationYTag, rotationY);
            ChartSerializatorCommon.SerializeBoolValueTag(writer, ChartConstants.RightAngleAxesTag, false);
            ChartSerializatorCommon.SerializeValueTag(writer, ChartConstants.PerspectiveTag, perspective);
            writer.WriteEndElement();

        }
    /// <summary>
    /// Serializes view 3D.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize legend entry into.</param>
    /// <param name="chart">Chart to serialize shapes for.</param>
    private void SerializeView3D( XmlWriter writer, ChartImpl chart )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      if( !chart.IsChart3D )
        return;

      writer.WriteStartElement( ChartConstants.View3DTag, ChartConstants.CNamespace );

      ChartFormatImpl format = chart.ChartFormat;

      if( !format.IsDefaultElevation )
        ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.RotationXTag, chart.Elevation.ToString() );

      if( chart.RightAngleAxes && !chart.AutoScaling )
      {
        ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.HeightPercentTag,
          chart.HeightPercent.ToString() );
      }

      if( !format.IsDefaultRotation )
        ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.RotationYTag, chart.Rotation.ToString() );

      ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.DepthPercentTag, chart.DepthPercent.ToString() );
      ChartSerializatorCommon.SerializeBoolValueTag( writer, ChartConstants.RightAngleAxesTag, chart.RightAngleAxes );
      ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.PerspectiveTag, ( chart.Perspective * 2 ).ToString() );
      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes error bars.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="errorBars">Error bars to serialize.</param>
    /// <param name="direction">Error bars direction (x or y).</param>
    /// <param name="book">Parent workbook.</param>
    private void SerializeErrorBars( XmlWriter writer, IChartErrorBars errorBars,
      string direction, IWorkbook book, ChartSerieImpl series )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( errorBars == null )
        return;

      if( direction == null || direction.Length == 0 )
        throw new ArgumentOutOfRangeException( "direction" );

      writer.WriteStartElement( ChartConstants.ErrorBarsTag, ChartConstants.CNamespace );
      ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.ErrorBarDirection, direction );

      ExcelErrorBarInclude include = errorBars.Include;
      string strErrorBarType = include.ToString().ToLower();
      ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.ErrorBarTypeTag, strErrorBarType );
      
      Excel2007ErrorBarType errorBarType = ( Excel2007ErrorBarType )errorBars.Type;
      ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.ErrorBarValueType,
        errorBarType.ToString() );
      ChartErrorBarsImpl errorBarImpl = errorBars as ChartErrorBarsImpl;
      ChartSerializatorCommon.SerializeBoolValueTag( writer, ChartConstants.ErrorBarsNoCap, !errorBars.HasCap );

      if( errorBarType == Excel2007ErrorBarType.cust )
      {
        if( include == ExcelErrorBarInclude.Plus || include == ExcelErrorBarInclude.Both )
        {

          writer.WriteStartElement( ChartConstants.ErrorBarPlusTag, ChartConstants.CNamespace );
          if (!errorBarImpl.IsPlusNumberLiteral)       
              SerializeNumReference(writer, errorBars.PlusRange, errorBarImpl.PlusRangeValues, series);         
          else          
              SerializeDirectlyEntered(writer, errorBarImpl.PlusRangeValues, false);          

          writer.WriteEndElement();
        }

        if( include == ExcelErrorBarInclude.Minus || include == ExcelErrorBarInclude.Both )
        {
          writer.WriteStartElement( ChartConstants.ErrorBarMinusTag, ChartConstants.CNamespace );
            if(!errorBarImpl.IsMinusNumberLiteral)
          SerializeNumReference(writer, errorBars.MinusRange, errorBarImpl.MinusRangeValues, series);
            else
                SerializeDirectlyEntered(writer, errorBarImpl.MinusRangeValues, false);
          writer.WriteEndElement();
        }
      }

      ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.ErrorBarValueTag,
        XmlConvert.ToString( errorBars.NumberValue ) );

      IChartBorder border = errorBars.Border;

      //element spPr { CT_ShapeProperties }?,
      // TODO: SerializeErrorBarProperties( writer, errorBars.Border );
      if( border != null && !border.AutoFormat )
      {
        writer.WriteStartElement( Drawings.ShapePropertiesTag, ChartConstants.CNamespace );
        ChartSerializatorCommon.SerializeLineProperties( writer, border, book );
        writer.WriteEndElement();
      }
      //element extLst { CT_ExtensionList }?
      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes trendlines collection.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="trendlines">Trendlines to serialize.</param>
    /// <param name="book">Parent workbook.</param>
    private void SerializeTrendlines( XmlWriter writer, IChartTrendLines trendlines, IWorkbook book )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( trendlines == null )
        return;

      for( int i = 0, len = trendlines.Count; i < len; i++ )
      {
        SerializeTrendline( writer, trendlines[ i ], book );
      }
    }
    /// <summary>
    /// Serializes trend line.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="trendline">Trend line to serialize.</param>
    /// <param name="book">Parent workbook.</param>
    private void SerializeTrendline( XmlWriter writer, IChartTrendLine trendline, IWorkbook book )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( trendline == null )
        throw new ArgumentNullException( "trendline" );

      writer.WriteStartElement( ChartConstants.TrendlineTag, ChartConstants.CNamespace );

      string strName = trendline.Name;

      if( strName != null && !trendline.NameIsAuto )
      {
        writer.WriteElementString( ChartConstants.TrendlineNameTag, ChartConstants.CNamespace, strName );
      }

      IChartBorder border = trendline.Border;

      if( border != null && !border.AutoFormat )
      {
        writer.WriteStartElement( Drawings.ShapePropertiesTag, ChartConstants.CNamespace );
        ChartSerializatorCommon.SerializeLineProperties( writer, trendline.Border, book );
        writer.WriteEndElement();
      }

      Excel2007TrendlineType trendlineType = ( Excel2007TrendlineType )trendline.Type;
      ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.TrendlineTypeTag,
        trendlineType.ToString() );

      string strOrderTag = null;
      if( trendline.Type == ExcelTrendLineType.Polynomial )
      {
        strOrderTag = ChartConstants.TrendlineOrderTag;
      }
      else if( trendline.Type == ExcelTrendLineType.Moving_Average )
      {
        strOrderTag = ChartConstants.TrendlinePeriodTag;
      }

      if( strOrderTag != null )
        ChartSerializatorCommon.SerializeValueTag( writer, strOrderTag, trendline.Order.ToString() );

      ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.TrendlineForwardTag,
        XmlConvert.ToString( trendline.Forward ) );

      ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.TrendlineBackwardTag,
        XmlConvert.ToString( trendline.Backward ) );

      if( !trendline.InterceptIsAuto )
      {
        ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.TrendlineIntercept,
          XmlConvert.ToString( trendline.Intercept ) );
      }

      ChartSerializatorCommon.SerializeBoolValueTag( writer, ChartConstants.DisplayRSquared,
        trendline.DisplayRSquared );

      ChartSerializatorCommon.SerializeBoolValueTag( writer, ChartConstants.DisplayEquation,
        trendline.DisplayEquation );

      if( trendline.DisplayRSquared || trendline.DisplayEquation )
        SerializeTrendlineLabel( writer, trendline.DataLabel );

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes trend line label settings.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="dataLabelFormat">Data label to serialize.</param>
    private void SerializeTrendlineLabel( XmlWriter writer, IChartTextArea dataLabelFormat )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( dataLabelFormat == null )
        throw new ArgumentNullException( "dataLabelFormat" );

      writer.WriteStartElement( ChartConstants.TrendlineLabelTag, ChartConstants.CNamespace );

      writer.WriteElementString( "layout", ChartConstants.CNamespace, string.Empty );
      writer.WriteStartElement( ChartConstants.NumberFormatTag, ChartConstants.CNamespace );
      writer.WriteAttributeString( ChartConstants.FormatCodeAttribute, "General" );
      writer.WriteAttributeString( ChartConstants.SourceLinkedAttribute, "0" );
      writer.WriteEndElement();

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes surface (wall or floor).
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="surface">Surface to serialize.</param>
    /// <param name="mainTagName">Name of the top xml tag to use.</param>
    /// <param name="chart">Parent chart object.</param>
    private void SerializeSurface( XmlWriter writer, IChartWallOrFloor surface,
      string mainTagName,
      ChartImpl chart )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( surface == null )
        throw new ArgumentNullException( "surface" );

      if( mainTagName == null || mainTagName.Length == 0 )
        throw new ArgumentOutOfRangeException( "mainTagName" );

      ChartWallOrFloorImpl wallFloorImpl=(ChartWallOrFloorImpl)surface;
      writer.WriteStartElement( mainTagName, ChartConstants.CNamespace );

      if (wallFloorImpl.Thickness != -1 || (chart.Workbook as WorkbookImpl).IsConverted)
          ChartSerializatorCommon.SerializeValueTag(writer, Drawings.ThicknessTag, wallFloorImpl.Thickness.ToString());

      // TODO: serialize some properties later.
      //ChartSerializatorCommon.SerializeValueTag( writer, "thickness", "1" );
      if(wallFloorImpl .HasShapeProperties || (chart.Workbook as WorkbookImpl).IsConverted)
         ChartSerializatorCommon.SerializeFrameFormat( writer, surface, chart, false );

     
      if (wallFloorImpl.PictureUnit ==ExcelChartPictureType.stack)
      {
          writer.WriteStartElement(Drawings.PictureoptionTag, Drawings.ANamespace);
          writer.WriteStartElement(Drawings.PictureformatTag, Drawings.ANamespace);
          writer.WriteAttributeString(Drawings.Valueattribite, wallFloorImpl.PictureUnit.ToString());
          writer.WriteEndElement();
          writer.WriteEndElement();
      }
        
      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes plotarea tag and everything inside it.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="chart">Chart to serialize.</param>
    private void SerializePlotArea( XmlWriter writer, ChartImpl chart, RelationCollection relations )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      writer.WriteStartElement( ChartConstants.PlotAreaTag, ChartConstants.CNamespace );

      if (chart.PlotArea != null)
      {
          IChartLayout layout = chart.PlotArea.Layout;
          if (layout != null)
              ChartSerializatorCommon.SerializeLayout(writer, chart.PlotArea);
      }

      //
      int iSeriesCount = chart.Series.Count;
      int iSerializedCount = 0;
      int iGroupIndex = 0;

      //if( chart.IsChartStock )
      //{
      //  SerializeStockChart( writer, chart, ( ChartSerieImpl )chart.Series[ 0 ] );
      //}
      //else
      {
        while( iSerializedCount != iSeriesCount )
        {
          iSerializedCount += SerializeMainChartTypeTag( writer, chart, iGroupIndex );
          iGroupIndex++;
        }

        if( iSerializedCount == 0 && !chart.HasPivotSource )
        {
          // serialize some default settings.
          writer.WriteStartElement( ChartConstants.BarChartTag, ChartConstants.CNamespace );
          ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.BarDirectionTag,
            ChartConstants.BarDirectionColumn );

          ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.BarGroupingTag,
            ChartConstants.Clustered );

          ChartAxisImpl axis = ( ChartAxisImpl )chart.PrimaryCategoryAxis;
          ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.AxisIdTag, axis.AxisId.ToString() );
          chart.SerializedAxisIds.Add(axis.AxisId);
          axis = ( ChartAxisImpl )chart.PrimaryValueAxis;
          ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.AxisIdTag, axis.AxisId.ToString() );
          chart.SerializedAxisIds.Add(axis.AxisId);
          writer.WriteEndElement();
        }
        else if( iSeriesCount == 0 && chart.HasPivotSource )
        {
          SerializePivotPlotArea( writer, chart, relations );
          return;
        }
      }

      SerializeAxes( writer, chart, relations );
      SerializeDataTable( writer, chart );

      if( chart.HasPlotArea )
      {
        IChartFrameFormat format = chart.PlotArea;
        ChartSerializatorCommon.SerializeFrameFormat( writer, format, chart,
          format.IsBorderCornersRound );
      }

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes plot area tag for pivot chart.
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="chart"></param>
    /// <param name="relations"></param>
    private void SerializePivotPlotArea( XmlWriter writer, ChartImpl chart, RelationCollection relations )
    {
      // serialize pivot chart 

      SerializeMainChartTypeTag( writer, chart );
      string pivotChartType = chart.PivotChartType.ToString();

      bool bPie = pivotChartType.Contains( ChartImpl.START_PIE );
      bool bDoughnut = pivotChartType.Contains( ChartImpl.START_DOUGHNUT );

      if( !( bPie || bDoughnut ) )
      {
        ChartAxisImpl axis = ( ChartAxisImpl )chart.PrimaryCategoryAxis;
        ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.AxisIdTag, axis.AxisId.ToString() );

        axis = ( ChartAxisImpl )chart.PrimaryValueAxis;
        ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.AxisIdTag, axis.AxisId.ToString() );

        if( pivotChartType.Contains( ChartImpl.START_SURFACE ) )
        {
          axis = ( ChartAxisImpl )chart.PrimarySerieAxis;
          ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.AxisIdTag, axis.AxisId.ToString() );
        }
      }

      writer.WriteEndElement();

      if( !( bPie || bDoughnut ) )
      {
        SerializePivotAxes( writer, chart, relations );
        SerializeDataTable( writer, chart );
      }

      if( chart.HasPlotArea )
      {
        IChartFrameFormat format = chart.PlotArea;
        ChartSerializatorCommon.SerializeFrameFormat( writer, format, chart,
          format.IsBorderCornersRound );
      }

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes the bar chart.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="chart">Chart to serialize.</param>
    private void SerializeBarChart( XmlWriter writer, ChartImpl chart )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      writer.WriteStartElement( ChartConstants.BarChartTag, ChartConstants.CNamespace );
      string strChartType = chart.PivotChartType.ToString();
      string strDirection = strChartType.Contains( ChartImpl.START_BAR ) ?
        ChartConstants.BarDirectionBar :
        ChartConstants.BarDirectionColumn;

      ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.BarDirectionTag, strDirection );
      SerializeChartGrouping( writer, chart );
      ChartSerializatorCommon.SerializeValueTag(writer, ChartConstants.OverlapTag, chart.OverLap .ToString());
    }
    /// <summary>
    /// Serializes bar chart.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="chart">Chart to serialize.</param>
    /// <param name="firstSeries">First series in the list of series with the
    /// same formatting to serialize.</param>
    /// <returns>Number of the serialized bar series.</returns>
    private int SerializeBarChart( XmlWriter writer, ChartImpl chart, ChartSerieImpl firstSeries )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      writer.WriteStartElement( ChartConstants.BarChartTag, ChartConstants.CNamespace );
      int iResult = SerializeBarChartShared( writer, chart, firstSeries );
      IChartFormat commonOptions = firstSeries.SerieFormat.CommonSerieOptions;

      int iGapWidth = 0;
      int iOverlap = 0;
      bool hasGapWidth = false;

      if (!(chart.Workbook as WorkbookImpl).IsCreated)
      {
          if (chart.GapWidth != 0 || chart.OverLap!=0)
          {
              iGapWidth = chart.GapWidth;
              iOverlap = chart.OverLap;
          }
          else
          {
              iGapWidth = (firstSeries as ChartSerieImpl).GapWidth;
              iOverlap = (firstSeries as ChartSerieImpl).Overlap;
              hasGapWidth = (firstSeries as ChartSerieImpl).ShowGapWidth;
          }
      }
      else
          iOverlap = chart.OverLap;

      if (iGapWidth == 0 & !hasGapWidth)
          iGapWidth = commonOptions.GapWidth;
      

      ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.GapWidthTag, iGapWidth.ToString() );

      if( iOverlap == ChartFormatImpl.DEF_BAR_STACKED )
        iOverlap = 100;

      if( iOverlap != 0 )
        ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.OverlapTag, iOverlap.ToString() );

      SerializeBarAxisId( writer, chart, firstSeries );

      writer.WriteEndElement();

      return iResult;
    }
    /// <summary>
    /// Serializes axis id's for bar chart.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="chart">Chart to serialize.</param>
    /// <param name="firstSeries">First series in the list of series with the
    /// same formatting to serialize.</param>
    private void SerializeBarAxisId( XmlWriter writer, ChartImpl chart, ChartSerieImpl firstSeries )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      if( firstSeries == null )
        throw new ArgumentNullException( "firstSeries" );

      bool bPrimary = firstSeries.UsePrimaryAxis;
      ChartAxisImpl axis = ( ChartAxisImpl )( bPrimary ?
        chart.PrimaryCategoryAxis :
        chart.SecondaryCategoryAxis );

      if( axis == null )
        throw new ArgumentNullException( "axis" );

      ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.AxisIdTag, axis.AxisId.ToString() );
      chart.SerializedAxisIds.Add(axis.AxisId);
      axis = ( ChartAxisImpl )( bPrimary ?
        chart.PrimaryValueAxis :
        chart.SecondaryValueAxis );

      ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.AxisIdTag, axis.AxisId.ToString() );
      chart.SerializedAxisIds.Add(axis.AxisId);
      if( chart.IsSeriesAxisAvail )
      {
        axis = ( ChartAxisImpl )chart.PrimarySerieAxis;
        ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.AxisIdTag, axis.AxisId.ToString() );
      }
    }
    /// <summary>
        /// Serializes the bar3 D chart.
        /// </summary>
        /// <param name="writer">XmlWriter to serialize into.</param>
        /// <param name="chart">Chart to serialize.</param>
        private void SerializeBar3DChart(XmlWriter writer, ChartImpl chart)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (chart == null)
                throw new ArgumentNullException("chart");

            ExcelBaseFormat baseFormat;
            ExcelTopFormat topFormat;

            writer.WriteStartElement(ChartConstants.Bar3DChartTag, ChartConstants.CNamespace);
            string strChartType = chart.PivotChartType.ToString();
            string strDirection = strChartType.Contains(ChartImpl.START_BAR) ?
              ChartConstants.BarDirectionBar :
              ChartConstants.BarDirectionColumn;

            ChartSerializatorCommon.SerializeValueTag(writer, ChartConstants.BarDirectionTag, strDirection);
            SerializeChartGrouping(writer, chart);
            if (strChartType.Contains(ChartImpl.START_CONE) || strChartType.Contains(ChartImpl.START_CYLINDER) || strChartType.Contains(ChartImpl.START_PYRAMID))
            {
                switch (chart.PivotChartType)
                {
                    case ExcelChartType.Cone_Bar_Clustered:
                    case ExcelChartType.Cone_Bar_Stacked:
                    case ExcelChartType.Cone_Bar_Stacked_100:
                    case ExcelChartType.Cone_Clustered:
                    case ExcelChartType.Cone_Clustered_3D:
                    case ExcelChartType.Cone_Stacked:
                    case ExcelChartType.Cone_Stacked_100:
                        baseFormat = ExcelBaseFormat.Circle;
                        topFormat = ExcelTopFormat.Sharp;
                        break;

                    case ExcelChartType.Pyramid_Bar_Clustered:
                    case ExcelChartType.Pyramid_Bar_Stacked:
                    case ExcelChartType.Pyramid_Bar_Stacked_100:
                    case ExcelChartType.Pyramid_Clustered:
                    case ExcelChartType.Pyramid_Clustered_3D:
                    case ExcelChartType.Pyramid_Stacked:
                    case ExcelChartType.Pyramid_Stacked_100:
                        baseFormat = ExcelBaseFormat.Rectangle;
                        topFormat = ExcelTopFormat.Sharp;
                        break;

                    case ExcelChartType.Cylinder_Bar_Clustered:
                    case ExcelChartType.Cylinder_Bar_Stacked:
                    case ExcelChartType.Cylinder_Bar_Stacked_100:
                    case ExcelChartType.Cylinder_Clustered:
                    case ExcelChartType.Cylinder_Clustered_3D:
                    case ExcelChartType.Cylinder_Stacked:
                    case ExcelChartType.Cylinder_Stacked_100:
                        baseFormat = ExcelBaseFormat.Circle;
                        topFormat = ExcelTopFormat.Straight;
                        break;

                    default:
                        throw new ArgumentException("type");
                }
                SerializeBarShape(writer, baseFormat, topFormat);
            }
        }
        /// <summary>
    /// Serialize bar3D chart.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="chart">Chart to serialize.</param>
    /// <param name="firstSeries">First series in the list of series with the
    /// same formatting to serialize.</param>
    /// <returns>Number of the serialized 3Dbar series.</returns>
    private int SerializeBar3DChart( XmlWriter writer, ChartImpl chart, ChartSerieImpl firstSeries )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      writer.WriteStartElement( ChartConstants.Bar3DChartTag, ChartConstants.CNamespace );
      int iResult = SerializeBarChartShared( writer, chart, firstSeries );

      IChartFormat commonOptions = firstSeries.SerieFormat.CommonSerieOptions;

      int iGapWidth = commonOptions.GapWidth;
      ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.GapWidthTag, iGapWidth.ToString() );
      SerializeGapDepth( writer, chart );
      SerializeBarShape( writer, chart, firstSeries );
      SerializeBarAxisId( writer, chart, firstSeries );

      writer.WriteEndElement();

      return iResult;
    }
    /// <summary>
    /// Serializes gap depth.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="chart">Chart to serialize gap depth for.</param>
    private void SerializeGapDepth( XmlWriter writer, ChartImpl chart )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.GapDepthTag, chart.GapDepth.ToString() );
    }
    /// <summary>
        /// Serializes the bar shape.
        /// </summary>
        /// <param name="writer">XmlWriter to serialize into.</param>
        /// <param name="baseFormat">The base format.</param>
        /// <param name="topFormat">The top format.</param>
        private void SerializeBarShape(XmlWriter writer, ExcelBaseFormat baseFormat, ExcelTopFormat topFormat)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            string strShapeType = null;
            switch (topFormat)
            {
                case ExcelTopFormat.Sharp:
                    strShapeType = (baseFormat == ExcelBaseFormat.Circle) ?
                      ChartConstants.BarShapeCone :
                      ChartConstants.BarShapePyramid;
                    break;

                case ExcelTopFormat.Trunc:
                    strShapeType = (baseFormat == ExcelBaseFormat.Circle) ?
                      ChartConstants.BarShapeConeToMax :
                      ChartConstants.BarShapePyramidToMax;
                    break;

                case ExcelTopFormat.Straight:
                    strShapeType = (baseFormat == ExcelBaseFormat.Circle) ?
                      ChartConstants.BarShapeCylinder :
                      ChartConstants.BarShapeBox;
                    break;
            }

            ChartSerializatorCommon.SerializeValueTag(writer, ChartConstants.BarShapeTag, strShapeType);
        }
        /// <summary>
    /// Serializes shape of the bar chart.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="chart">Chart to serialize.</param>
    /// <param name="firstSeries">First series in the list of series with the
    /// same formatting to serialize.</param>
    private void SerializeBarShape( XmlWriter writer, ChartImpl chart, ChartSerieImpl firstSeries )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      IChartSerieDataFormat dataFormat = firstSeries.SerieFormat;
      ExcelTopFormat topFormat = dataFormat.BarShapeTop;
      ExcelBaseFormat baseFormat = dataFormat.BarShapeBase;
      string strShapeType = null;
      switch( topFormat )
      {
        case ExcelTopFormat.Sharp:
          strShapeType = ( baseFormat == ExcelBaseFormat.Circle ) ?
            ChartConstants.BarShapeCone :
            ChartConstants.BarShapePyramid;
          break;

        case ExcelTopFormat.Trunc:
          strShapeType = ( baseFormat == ExcelBaseFormat.Circle ) ?
            ChartConstants.BarShapeConeToMax :
            ChartConstants.BarShapePyramidToMax;
          break;

        case ExcelTopFormat.Straight:
          strShapeType = ( baseFormat == ExcelBaseFormat.Circle ) ?
            ChartConstants.BarShapeCylinder :
            ChartConstants.BarShapeBox;
          break;
      }

      ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.BarShapeTag, strShapeType );
    }
    /// <summary>
    /// Serializes part of the bar chart that is common for all bar charts.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="chart">Chart to serialize.</param>
    /// <param name="firstSeries">First series in the list of series with the
    /// same formatting to serialize.</param>
    /// <returns>Number of the serialized bar series.</returns>
    private int SerializeBarChartShared( XmlWriter writer, ChartImpl chart, ChartSerieImpl firstSeries )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      string strChartType = firstSeries.SerieType.ToString();
      string strDirection = strChartType.Contains( ChartImpl.START_BAR ) ?
        ChartConstants.BarDirectionBar :
        ChartConstants.BarDirectionColumn;

      ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.BarDirectionTag, strDirection );
      
      SerializeChartGrouping(writer, firstSeries);

      // TODO: we support only one chart type on the current moment, later this code should be changed.
      SerializeVaryColors( writer, firstSeries );
      int iSeriesCount = SerializeChartSeries( writer, chart, firstSeries, SerializeBarSeries );

      //SerializeDataLabels( writer, chart.Series[ 0 ].DataPoints.DefaultDataPoint.DataLabels );
      return iSeriesCount;
    }
    /// <summary>
    /// Serializes chart series with the same formatting.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="chart">Chart to get series from.</param>
    /// <param name="firstSeries">First series in the list of the series to serialize.
    /// It is used to determine which series should be serialized.</param>
    /// <param name="serializator">Delegate used to serialize chart series.</param>
    /// <returns>Number of serialized series.</returns>
    private int SerializeChartSeries( XmlWriter writer, ChartImpl chart,
      ChartSerieImpl firstSeries, SerializeSeriesDelegate serializator )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      if( firstSeries == null )
        throw new ArgumentNullException( "firstSeries" );

      if( serializator == null )
        throw new ArgumentNullException( "serializator" );

      int iChartGroup = firstSeries.ChartGroup;
      IList<IChartSerie> arrAdditionOrder = ( chart.Series as ChartSeriesCollection ).AdditionOrder;
      IChartSeries arrSeries = chart.Series;
      IList<IChartSerie> arrOrderedSeries = ( arrAdditionOrder.Count == chart.Series.Count ) ?
        arrAdditionOrder :
        ( IList<IChartSerie> )chart.Series;

      int iSeriesFirstIndex = GetSeriesIndex( firstSeries, arrOrderedSeries, arrSeries );
      firstSeries = arrOrderedSeries[ iSeriesFirstIndex ] as ChartSerieImpl;
      List<ChartSerieImpl> filteredseries = new List<ChartSerieImpl>();
      if (!firstSeries.IsFiltered)
          serializator(writer, firstSeries);
      else
      {
          filteredseries.Add(firstSeries);
      }
      int iSeriesCount = 1;

      for( int i = iSeriesFirstIndex + 1, len = arrOrderedSeries.Count; i < len; i++ )
      {
        ChartSerieImpl series = ( ChartSerieImpl )arrOrderedSeries[ i ];

        if( series.ChartGroup == iChartGroup )
        {
            if (!series.IsFiltered)
                serializator(writer, series);
            else
            {
                filteredseries.Add(series);
            }
          iSeriesCount++;
        }
      }
      if (filteredseries.Count != 0)
      {
          writer.WriteStartElement(Excel2007Serializator.Extensionlist, ChartConstants.CNamespace);
          writer.WriteStartElement(ChartConstants.CPrefix, Excel2007Serializator.Extension, ChartConstants.CNamespace);
          writer.WriteAttributeString(Drawings.UriAttribute, ChartConstants.Filterseriesuri);
          writer.WriteAttributeString(WorkbookXmlSerializator.DEF_XMLNS_PREF, ChartConstants.c15tag, null, ChartConstants.xml15web);
          if (categoryFilter == 0)
          {
              findFilter = FindFiltered(filteredseries[0]);
              categoryFilter++;
              if (findFilter)
              {
                  UpdateCategoryLabel(filteredseries[0]);
                  UpdateFilteredValuesRange(filteredseries[0]);
              }
          }
          for (int i = 0; i < filteredseries.Count; i++)
          {              
              SerializeFilteredSeries(writer,filteredseries[i] );
          }
              writer.WriteEndElement();
              writer.WriteEndElement();
      }
      

      //SerializeDataLabels( writer, chart.Series[ 0 ].DataPoints.DefaultDataPoint.DataLabels );

      return iSeriesCount;
    }
    /// <summary>
    /// serialiae filtered serie
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="series"></param>
    private void SerializeFilteredSeries(XmlWriter writer, ChartSerieImpl series)
    {
        
        string filterseriesstring = GetSeriesType(series.StartType);        
        
        writer.WriteStartElement(ChartConstants.c15tag, filterseriesstring, ChartConstants.xml15web);
        writer.WriteStartElement(ChartConstants.c15tag, ChartConstants.SeriesTag, ChartConstants.xml15web);

        //serialize filtered series
        SerializeFilterSeries(writer, series);        
        
        writer.WriteEndElement();
        writer.WriteEndElement();
        
        

    }
    /// <summary>
    /// Serilaize filtered series
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="series"></param>
    private void SerializeFilterSeries(XmlWriter writer, ChartSerieImpl series)
    {
        writer.WriteStartElement(ChartConstants.CPrefix,ChartConstants.IndexTag,ChartConstants.CNamespace);
        writer.WriteAttributeString(ChartConstants.ValueAttribute, series.Number.ToString());
        writer.WriteEndElement();
        writer.WriteStartElement(ChartConstants.CPrefix, ChartConstants.SeriesOrderTag, ChartConstants.CNamespace);
        writer.WriteAttributeString(ChartConstants.ValueAttribute, series.Index.ToString());
        writer.WriteEndElement();
        if( (series.ParentChart as ChartImpl).SeriesNameLevel == ExcelSeriesNameLevel.SeriesNameLevelAll)
        {
            SerializeFilteredText(writer, series);
        }
        SerializeSeriesCommonWithoutEnd(writer, series, true);
        int iPercent = series.SerieFormat.Percent;
        if (iPercent != 0)
        {
            ChartSerializatorCommon.SerializeValueTag(writer, ChartConstants.PieExplosionTag,
              iPercent.ToString());
        }
        // serialize the data label
        if ((series.DataPoints.DefaultDataPoint as ChartDataPointImpl).HasDataLabels)
            SerializeDataLabels(writer, series.DataPoints.DefaultDataPoint.DataLabels, series);

        SerializeTrendlines(writer, series.TrendLines, series.ParentBook);
        SerializeErrorBars(writer, series);
        if ((series.ParentChart as ChartImpl).CategoryLabelLevel == ExcelCategoriesLabelLevel.CategoriesLabelLevelAll)
            SerializeFilteredCategory(writer, series, FindFiltered(series));
        SerializeFilteredValues(writer, series, FindFiltered(series));
        if (series.SerieType == ExcelChartType.Bubble || series.SerieType == ExcelChartType.Bubble_3D)
        {
            SerializeSeriesValues(writer, series.Bubbles, series.EnteredDirectlyBubbles, ChartConstants.BubbleSize, series);
        }
        if ((series.ParentChart as ChartImpl).CategoryLabelLevel != ExcelCategoriesLabelLevel.CategoriesLabelLevelAll || (series.ParentChart as ChartImpl).SeriesNameLevel != ExcelSeriesNameLevel.SeriesNameLevelAll)
        {
            writer.WriteStartElement(Excel2007Serializator.Extensionlist, ChartConstants.CNamespace);
            if ((series.ParentChart as ChartImpl).SeriesNameLevel!=ExcelSeriesNameLevel.SeriesNameLevelAll)
            {
                SerializeFilteredSeriesOrCategoryName(writer, series, true);
            }
            if (series.CategoryLabels != null && (series.ParentChart as ChartImpl).CategoryLabelLevel != ExcelCategoriesLabelLevel.CategoriesLabelLevelAll)
            {
                SerializeFilteredSeriesOrCategoryName(writer, series, false);
            }
            writer.WriteEndElement();
        }
    }
    /// <summary>
    /// Serialize Fitered Text
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="series"></param>
    private void SerializeFilteredText(XmlWriter writer,ChartSerieImpl series )
    {
        if (!series.IsDefaultName)
        {
            if( (series.ParentChart as ChartImpl).SeriesNameLevel != ExcelSeriesNameLevel.SeriesNameLevelAll)
            {
                writer.WriteStartElement(ChartConstants.CPrefix, ChartConstants.SeriesTextTag, ChartConstants.CNamespace);
            }
            else
            {
                writer.WriteStartElement(ChartConstants.c15tag, ChartConstants.SeriesTextTag, ChartConstants.xml15web);
            }
            string strName = series.NameOrFormula;
            if (strName.Length > 0 && strName[0] == '=')
            {
                SerializeFiltedStringReference(writer, strName, series);                
            }
            else
            {
                writer.WriteStartElement(ChartConstants.TextValueTag, ChartConstants.CNamespace);
                writer.WriteString(strName);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

        }
    } 
    /// <summary>
    /// Serialize filtered text values
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="range"></param>
    /// <param name="series"></param>
    private void SerializeFiltedStringReference(XmlWriter writer,string range,ChartSerieImpl series)
      {
          writer.WriteStartElement(ChartConstants.CPrefix,ChartConstants.StringReferenceTag, ChartConstants.CNamespace);
          writer.WriteStartElement(ChartConstants.CPrefix,Excel2007Serializator.Extensionlist, ChartConstants.CNamespace);
          writer.WriteStartElement(ChartConstants.CPrefix, Excel2007Serializator.Extension,ChartConstants.CNamespace);
          writer.WriteAttributeString(Drawings.UriAttribute, ChartConstants.Filterseriesuri);
          writer.WriteStartElement(ChartConstants.c15tag, ChartConstants.formulareference,ChartConstants.xml15web);
          
          if (range == null)
              throw new ArgumentNullException("range");

          if (range[0] == '=')
              range = UtilityMethods.RemoveFirstCharUnsafe(range);

          if (series.StrRefFormula != null)
              range = series.StrRefFormula;
          writer.WriteElementString(ChartConstants.SqureReference, ChartConstants.xml15web, range);
          
          writer.WriteEndElement();
          writer.WriteEndElement();
          //// TODO: we don't store cache and we don't support anything but formula here, this should be changed later.
          writer.WriteEndElement();
          writer.WriteEndElement();      
         }
    /// <summary>
    /// serialize filtered category
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="series"></param>
    /// <param name="categoryfilter"></param>
    private void SerializeFilteredCategory(XmlWriter writer, ChartSerieImpl series, bool categoryfilter)
      {
          if (series.CategoryLabels != null)
          {
              if (series.SerieType == ExcelChartType.Bubble || series.SerieType == ExcelChartType.Bubble_3D || series.StartType.Contains("Scatter"))
              {
                  writer.WriteStartElement(ChartConstants.CPrefix, ChartConstants.XValues, ChartConstants.CNamespace);
              }
              else
              {
                  writer.WriteStartElement(ChartConstants.CPrefix, ChartConstants.CategoryValuesTag, ChartConstants.CNamespace);
              }
              writer.WriteStartElement(ChartConstants.CPrefix, ChartConstants.StringReferenceTag, ChartConstants.CNamespace);
              writer.WriteStartElement(ChartConstants.CPrefix, Excel2007Serializator.Extensionlist, ChartConstants.CNamespace);
              writer.WriteStartElement(ChartConstants.CPrefix, Excel2007Serializator.Extension, ChartConstants.CNamespace);
              writer.WriteAttributeString(Drawings.UriAttribute, ChartConstants.Filterseriesuri);
              if (categoryfilter && series.IsFiltered)
              {
                  SerializeFilteredFullReference(writer, series, false);
                  writer.WriteStartElement(ChartConstants.c15tag, ChartConstants.formulareference, ChartConstants.xml15web);
                  writer.WriteElementString(ChartConstants.SqureReference, ChartConstants.xml15web, series.FilteredCategory);
                  writer.WriteEndElement();

              }
              else if (series.IsFiltered)
              {
                  writer.WriteStartElement(ChartConstants.c15tag, ChartConstants.formulareference, ChartConstants.xml15web);

                  string range = null;
                  if (series.CategoryLabels != null)
                  {
                      range = series.CategoryLabels.AddressGlobal;
                  }

                  writer.WriteElementString(ChartConstants.SqureReference, ChartConstants.xml15web, range);
                  
                  writer.WriteEndElement();
              }
              else
              {
                  SerializeFilteredFullReference(writer, series, false);
              }

              writer.WriteEndElement();

              writer.WriteEndElement();
              if (!series.IsFiltered && categoryfilter)
              {
                  writer.WriteElementString(ChartConstants.Formula, ChartConstants.CNamespace, series.FilteredCategory);
              }
              writer.WriteElementString(ChartConstants.StringCacheTag, ChartConstants.CNamespace, string.Empty);
              writer.WriteEndElement();
              writer.WriteEndElement();
          }
      }
    /// <summary>
    /// serialize filtered values
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="series"></param>
    /// <param name="categoryfilter"></param>
    private void SerializeFilteredValues(XmlWriter writer, ChartSerieImpl series,bool categoryfilter)
      {
          if (series.SerieType == ExcelChartType.Bubble || series.SerieType == ExcelChartType.Bubble_3D || series.StartType.Contains("Scatter"))
          {
              writer.WriteStartElement(ChartConstants.CPrefix, ChartConstants.YValues, ChartConstants.CNamespace); 
          }
          else
          {
              writer.WriteStartElement(ChartConstants.CPrefix, ChartConstants.ValueAttribute, ChartConstants.CNamespace);              
          }
          writer.WriteStartElement(ChartConstants.CPrefix, ChartConstants.NumberReferenceTag, ChartConstants.CNamespace);
          writer.WriteStartElement(ChartConstants.CPrefix, Excel2007Serializator.Extensionlist, ChartConstants.CNamespace);
          writer.WriteStartElement(ChartConstants.CPrefix, Excel2007Serializator.Extension, ChartConstants.CNamespace);
          writer.WriteAttributeString(Drawings.UriAttribute, ChartConstants.Filterseriesuri);
          if (categoryfilter && series.IsFiltered)
          {
              SerializeFilteredFullReference(writer, series, true);
              writer.WriteStartElement(ChartConstants.c15tag, ChartConstants.formulareference, ChartConstants.xml15web);
              writer.WriteElementString(ChartConstants.SqureReference, ChartConstants.xml15web, series.FilteredValue);
              writer.WriteEndElement();
          }
          else if (series.IsFiltered)
          {
              writer.WriteStartElement(ChartConstants.c15tag, ChartConstants.formulareference, ChartConstants.xml15web);
              string range = null;
              if (series.Values != null)
              {
                  range = series.Values.AddressGlobal;
              }

              writer.WriteElementString(ChartConstants.SqureReference, ChartConstants.xml15web, range);
              writer.WriteEndElement();
          }
          else
          {
              SerializeFilteredFullReference(writer, series, true);
          }
          writer.WriteEndElement();
          //// TODO: we don't store cache and we don't support anything but formula here, this should be changed later.
          writer.WriteEndElement();
          if (!series.IsFiltered && categoryfilter)
          {
              writer.WriteElementString(ChartConstants.Formula, ChartConstants.CNamespace, series.FilteredValue);
          }
          writer.WriteElementString(ChartConstants.NumberCacheTag, ChartConstants.CNamespace, string.Empty);          
          writer.WriteEndElement();          
          writer.WriteEndElement();          
      }
    /// <summary>
    /// Returns the series filter type
    /// </summary>
    /// <param name="Series"></param>
    /// <returns></returns>
    public string GetSeriesType(string Series)
      {
          String[] FilteredSeries = {"filteredBarSeries","filteredAreaSeries","filteredLineSeries","filteredPieSeries",
                                               "filteredRadarSeries","filteredScatterSeries","filteredSurfaceSeries","filteredBubbleSeries"};
    int count=0;
    for (count = 0; FilteredSeries.Length > count; count++)
    {
        if (FilteredSeries[count].Contains(Series))
            break;
    }

    return FilteredSeries[count%8];
      }
    private int GetSeriesIndex( ChartSerieImpl firstSeries, IList<IChartSerie> arrOrderedSeries, IChartSeries arrSeries )
    {
      int result = -1;

      if( arrSeries == arrOrderedSeries )
      {
        result = firstSeries.Index;
      }
      else
      {
        for( int i = 0, len = arrOrderedSeries.Count; i < len; i++ )
        {
          if( ( arrOrderedSeries[ i ] as ChartSerieImpl ).ChartGroup == firstSeries.ChartGroup )
          {
            result = i;
            break;
          }
        }
      }

      return result;
    }
    private void SerializeFilteredFullReference(XmlWriter writer, ChartSerieImpl series,bool catorval)
    {
        writer.WriteStartElement(ChartConstants.c15tag, ChartConstants.fullReference, ChartConstants.xml15web);
        string range = null;
        if (catorval)
        {
            if (series.Values != null)
            {
                range = series.Values.AddressGlobal;
            }
        }
        else
        {
            range = series.CategoryLabels.AddressGlobal;
        }

        writer.WriteElementString(ChartConstants.SqureReference, ChartConstants.xml15web, range);             
        writer.WriteEndElement();      

    }
    /// <summary>
    /// Serializes grouping tag.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="chart">Chart to serialize grouping for.</param>
    private void SerializeChartGrouping( XmlWriter writer, ChartSerieImpl firstSeries )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( firstSeries == null )
        throw new ArgumentNullException( "firstSeries" );

      string strGrouping;
      ExcelChartType seriesType = firstSeries.SerieType;

      if( ChartImpl.GetIsClustered( seriesType ) )
      {
        strGrouping = ChartConstants.Clustered;
      }
      else if( ChartImpl.GetIs100( seriesType ) )
      {
        strGrouping = ChartConstants.PercentStacked;
      }
      else if( ChartImpl.GetIsStacked( seriesType ) )
      {
        strGrouping = ChartConstants.Stacked;
      }
      else
      {
        strGrouping = ChartConstants.Standard;
      }

      ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.BarGroupingTag, strGrouping );
    }
    /// <summary>
        /// Serializes the chart grouping.
        /// </summary>
        /// <param name="writer">XmlWriter to serialize into.</param>
        /// <param name="chart">Chart to get series from.</param>
        private void SerializeChartGrouping(XmlWriter writer, ChartImpl chart)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (chart == null)
                throw new ArgumentNullException("firstSeries");

            string strGrouping;
            ExcelChartType pivotChartType = chart.PivotChartType;

            if (ChartImpl.GetIsClustered(pivotChartType))
            {
                strGrouping = ChartConstants.Clustered;
            }
            else if (ChartImpl.GetIs100(pivotChartType))
            {
                strGrouping = ChartConstants.PercentStacked;
            }
            else if (ChartImpl.GetIsStacked(pivotChartType))
            {
                strGrouping = ChartConstants.Stacked;
            }
            else
            {
                strGrouping = ChartConstants.Standard;
            }

            ChartSerializatorCommon.SerializeValueTag(writer, ChartConstants.BarGroupingTag, strGrouping);
        }

        /// <summary>
        /// Serializes the area3 D chart.
        /// </summary>
        /// <param name="writer">XmlWriter to serialize into.</param>
        /// <param name="chart">Chart to get series from.</param>
        private void SerializeArea3DChart(XmlWriter writer, ChartImpl chart)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (chart == null)
                throw new ArgumentNullException("chart");

            writer.WriteStartElement(ChartConstants.Area3DChartTag, ChartConstants.CNamespace);
            SerializeChartGrouping(writer, chart);
        }
        /// <summary>
    /// Serializes area3D chart.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="chart">Chart to serialize.</param>
    /// <param name="firstSeries">First series in the list of series with the
    /// same formatting to serialize.</param>
    /// <returns>Number of the serialized area3D series.</returns>
    private int SerializeArea3DChart( XmlWriter writer, ChartImpl chart, ChartSerieImpl firstSeries )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      writer.WriteStartElement( ChartConstants.Area3DChartTag, ChartConstants.CNamespace );
      int iResult = SerializeAreaChartCommon( writer, chart, firstSeries );
      SerializeBarAxisId( writer, chart, firstSeries );
      writer.WriteEndElement();

      return iResult;
    }
    /// <summary>
        /// Serializes the area chart.
        /// </summary>
        /// <param name="writer">XmlWriter to serialize into.</param>
        /// <param name="chart">Chart to get series from.</param>
        private void SerializeAreaChart(XmlWriter writer, ChartImpl chart)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (chart == null)
                throw new ArgumentNullException("chart");
            writer.WriteStartElement(ChartConstants.AreaChartTag, ChartConstants.CNamespace);
            SerializeChartGrouping(writer, chart);
        }
        /// <summary>
    /// Serializes area chart.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="chart">Chart to serialize.</param>
    /// <param name="firstSeries">First series in the list of series with the
    /// same formatting to serialize.</param>
    /// <returns>Number of the serialized area series.</returns>
    private int SerializeAreaChart( XmlWriter writer, ChartImpl chart, ChartSerieImpl firstSeries )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      writer.WriteStartElement( ChartConstants.AreaChartTag, ChartConstants.CNamespace );
      int iResult = SerializeAreaChartCommon( writer, chart, firstSeries );
      SerializeBarAxisId( writer, chart, firstSeries );
      writer.WriteEndElement();

      return iResult;
    }
    /// <summary>
    /// Serializes properties common to the area chart.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="chart">Chart to serialize.</param>
    /// <param name="firstSeries">First series in the list of series with the
    /// same formatting to serialize.</param>
    /// <returns>Number of the serialized series.</returns>
    private int SerializeAreaChartCommon( XmlWriter writer, ChartImpl chart, ChartSerieImpl firstSeries )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      SerializeChartGrouping( writer, firstSeries );
      SerializeVaryColors( writer, firstSeries );
      int iResult = SerializeChartSeries( writer, chart, firstSeries, SerializeAreaSeries );
      return iResult;
    }
    /// <summary>
    /// This method serializes common properties of the line charts.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="chart">Chart to serialize.</param>
    /// <param name="firstSeries">First series in the list of series with the
    /// same formatting to serialize.</param>
    /// <returns>Number of the serialized line series.</returns>
    private int SerializeLineChartCommon( XmlWriter writer, ChartImpl chart, ChartSerieImpl firstSeries )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      if( !chart.IsChartStock )
      {
        SerializeChartGrouping( writer, firstSeries );
        SerializeVaryColors( writer, firstSeries );
      }

      int iResult = SerializeChartSeries( writer, chart, firstSeries, SerializeLineSeries );
      return iResult;
    }
    /// <summary>
        /// Serializes the line3 D chart.
        /// </summary>
        /// <param name="writer">XmlWriter to serialize into.</param>
        /// <param name="chart">Chart to serialize.</param>
        private void SerializeLine3DChart(XmlWriter writer, ChartImpl chart)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (chart == null)
                throw new ArgumentNullException("chart");

            writer.WriteStartElement(ChartConstants.Line3DChartTag, ChartConstants.CNamespace);
           
                SerializeChartGrouping(writer, chart);
           
            ChartSerializatorCommon.SerializeValueTag(writer, ChartConstants.MarkerTag, "1");
        }
        /// <summary>
    /// Serialize line3DChart.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="chart">Chart to serialize.</param>
    /// <param name="firstSeries">First series in the list of series with the
    /// same formatting to serialize.</param>
    /// <returns>Number of the serialized line3D series.</returns>
    private int SerializeLine3DChart( XmlWriter writer, ChartImpl chart, ChartSerieImpl firstSeries )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      writer.WriteStartElement( ChartConstants.Line3DChartTag, ChartConstants.CNamespace );
      int iResult = SerializeLineChartCommon( writer, chart, firstSeries );
      SerializeBarAxisId( writer, chart, firstSeries );
      writer.WriteEndElement();

      return iResult;
    }
    /// <summary>
        /// Serializes the line chart.
        /// </summary>
        /// <param name="writer">XmlWriter to serialize into.</param>
        /// <param name="chart">Chart to serialize.</param>
        private void SerializeLineChart(XmlWriter writer, ChartImpl chart)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (chart == null)
                throw new ArgumentNullException("chart");

            string strMainTag = ChartConstants.LineChartTag;

            writer.WriteStartElement(strMainTag, ChartConstants.CNamespace);

            SerializeChartGrouping(writer, chart);

            ChartSerializatorCommon.SerializeValueTag(writer, ChartConstants.MarkerTag, "1");
        }
        /// <summary>
    /// Serializes line chart.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="chart">Chart to serialize.</param>
    /// <param name="firstSeries">First series in the list of series with the
    /// same formatting to serialize.</param>
    /// <returns>Number of the serialized line series.</returns>
    private int SerializeLineChart( XmlWriter writer, ChartImpl chart, ChartSerieImpl firstSeries )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      string strMainTag = chart.IsChartStock ?
        ChartConstants.StockChartTag :
        ChartConstants.LineChartTag;

      writer.WriteStartElement( strMainTag, ChartConstants.CNamespace );
      int iResult = SerializeLineChartCommon( writer, chart, firstSeries );
      ChartSerieDataFormatImpl dataFormat = (ChartSerieDataFormatImpl)firstSeries.SerieFormat;

      ChartFormatImpl format = (ChartFormatImpl)firstSeries.SerieFormat.CommonSerieOptions;

      if (format.IsChartChartLine && format.LineStyle == ExcelDropLineStyle.HiLow)
      {
          writer.WriteElementString(ChartConstants.HiLowLinesTag, ChartConstants.CNamespace, string.Empty);
      }

      SerializeUpDownBars( writer, chart, firstSeries );
      
      //TODO: Need to serialize from the properties instead of using stream
      if ((firstSeries != null) && (firstSeries.DropLinesStream != null))
      {
          firstSeries.DropLinesStream.Position = 0;
          ShapeParser.WriteNodeFromStream(writer, firstSeries.DropLinesStream, true);
      }

      if (dataFormat.IsMarker)
      {
          ChartSerializatorCommon.SerializeValueTag(writer, ChartConstants.MarkerTag, "1");
      }
      SerializeBarAxisId( writer, chart, firstSeries );
      writer.WriteEndElement();

      return iResult;
    }
    /// <summary>
    /// Serialize bubble chart.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize chart into.</param>
    /// <param name="chart">Chart to serialize.</param>
    /// <param name="firstSeries">First series in the list of series with the
    /// same formatting to serialize.</param>
    /// <returns>Number of the serialized bubble series.</returns>
    private int SerializeBubbleChart( XmlWriter writer, ChartImpl chart, ChartSerieImpl firstSeries )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      writer.WriteStartElement( ChartConstants.BubbleChartTag, ChartConstants.CNamespace );

      SerializeVaryColors( writer, firstSeries );
      int iResult = SerializeChartSeries( writer, chart, firstSeries, SerializeBubbleSeries );

      IChartFormat commonOptions = firstSeries.SerieFormat.CommonSerieOptions;
      int iBubbleScale = commonOptions.BubbleScale;

      if( iBubbleScale != ChartConstants.BubbleScaleDefault )
        ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.BubbleScaleTag, iBubbleScale.ToString() );

      if( commonOptions.ShowNegativeBubbles )
        ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.ShowNegativeBubbles, "1" );

      ExcelBubbleSize bubbleSize = commonOptions.SizeRepresents;
      string strBubbleSize = ( bubbleSize == ExcelBubbleSize.Area ) ?
        ChartConstants.BubbleSizeArea :
        ChartConstants.BubbleSizeWidth;

      ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.BubbleSizeRepresents, strBubbleSize );
      SerializeBarAxisId( writer, chart, firstSeries );
      writer.WriteEndElement();

      return iResult;
    }
    /// <summary>
        /// Serializes the surface chart.
        /// </summary>
        /// <param name="writer">XmlWriter to serialize into.</param>
        /// <param name="chart">Chart to serialize.</param>
        private void SerializeSurfaceChart(XmlWriter writer, ChartImpl chart)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (chart == null)
                throw new ArgumentNullException("chart");

            writer.WriteStartElement(ChartConstants.SurfaceChartTag, ChartConstants.CNamespace);
            ExcelChartType chartType = chart.PivotChartType;
            bool bWireFrame = (chartType == ExcelChartType.Surface_NoColor_3D
              || chartType == ExcelChartType.Surface_NoColor_Contour);

            if (bWireFrame)
                ChartSerializatorCommon.SerializeValueTag(writer, ChartConstants.WireframeTag, "1");
        }
        /// <summary>
    /// Serializes 2-D surface chart.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="chart">Chart to serialize.</param>
    /// <param name="firstSeries">First series in the list of series with the
    /// same formatting to serialize.</param>
    /// <returns>Number of the serialized series.</returns>
    private int SerializeSurfaceChart( XmlWriter writer, ChartImpl chart, ChartSerieImpl firstSeries )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      writer.WriteStartElement( ChartConstants.SurfaceChartTag, ChartConstants.CNamespace );
      int iResult = SerializeSurfaceCommon( writer, chart, firstSeries );
      SerializeBarAxisId( writer, chart, firstSeries );
      writer.WriteEndElement();

      return iResult;
    }
    /// <summary>
        /// Serializes the surface3 D chart.
        /// </summary>
        /// <param name="writer">XmlWriter to serialize into.</param>
        /// <param name="chart">Chart to serialize.</param>
        private void SerializeSurface3DChart(XmlWriter writer, ChartImpl chart)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (chart == null)
                throw new ArgumentNullException("chart");

            writer.WriteStartElement(ChartConstants.Surface3DChartTag, ChartConstants.CNamespace);
            ExcelChartType chartType = chart.PivotChartType;
            bool bWireFrame = (chartType == ExcelChartType.Surface_NoColor_3D
              || chartType == ExcelChartType.Surface_NoColor_Contour);

            if (bWireFrame)
                ChartSerializatorCommon.SerializeValueTag(writer, ChartConstants.WireframeTag, "1");
        }
        /// <summary>
    /// Serializes 3-D surface chart.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="chart">Chart to serialize.</param>
    /// <param name="firstSeries">First series in the list of series with the
    /// same formatting to serialize.</param>
    /// <returns>Number of the serialized series.</returns>
    private int SerializeSurface3DChart( XmlWriter writer, ChartImpl chart, ChartSerieImpl firstSeries )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      writer.WriteStartElement( ChartConstants.Surface3DChartTag, ChartConstants.CNamespace );
      int iResult = SerializeSurfaceCommon( writer, chart, firstSeries );
      SerializeBarAxisId( writer, chart, firstSeries );
      writer.WriteEndElement();

      return iResult;
    }
    /// <summary>
    /// Serializes common part of the surface charts.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="chart">Chart to serialize.</param>
    /// <param name="firstSeries">First series in the list of series with the
    /// same formatting to serialize.</param>
    /// <returns>Number of the serialized series.</returns>
    private int SerializeSurfaceCommon( XmlWriter writer, ChartImpl chart, ChartSerieImpl firstSeries )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      ExcelChartType chartType = firstSeries.SerieType;
      bool bWireFrame = ( chartType == ExcelChartType.Surface_NoColor_3D
        || chartType == ExcelChartType.Surface_NoColor_Contour );

      if( bWireFrame )
        ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.WireframeTag, "1" );

      int result = SerializeChartSeries( writer, chart, firstSeries, SerializeBarSeries );
      SerializeBandFormats( writer, chart );
      return result;
    }
    /// <summary>
    /// Serializes preserved band formats if necessary.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="chart">Chart to serialize.</param>
    private void SerializeBandFormats( XmlWriter writer, ChartImpl chart )
    {
      Stream stream = chart.PreservedBandFormats;

      if( stream != null && stream.Length > 0 )
      {
        stream.Position = 0;
        ShapeParser.WriteNodeFromStream( writer, stream );
        //Excel2007Serializator
      }
    }
    /// <summary>
    /// Serializes main chart tag.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="chart">Chart to serialize.</param>
    /// <param name="groupIndex">Index of the series group to serialize.</param>
    /// <returns>Number of the serialized series.</returns>
    private int SerializeMainChartTypeTag( XmlWriter writer, ChartImpl chart, int groupIndex )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      // TODO: we don't support multi type charts, we should add it later.
      // 1. Find first series with that index
      IChartSeries arrSeries = chart.Series;
      ChartSerieImpl firstSeries = null;

      for( int i = 0, len = arrSeries.Count; i < len; i++ )
      {
        ChartSerieImpl series = ( ChartSerieImpl )arrSeries[ i ];

        if( series.ChartGroup == groupIndex )
        {
          firstSeries = series;
          break;
        }
      }


      int iResult = 0;
      if( firstSeries != null )
      {
        switch( firstSeries.SerieType )
        {
          case ExcelChartType.Column_Clustered:
          case ExcelChartType.Column_Stacked:
          case ExcelChartType.Column_Stacked_100:
          case ExcelChartType.Bar_Clustered:
          case ExcelChartType.Bar_Stacked:
          case ExcelChartType.Bar_Stacked_100:
            iResult = SerializeBarChart( writer, chart, firstSeries );
            break;

          case ExcelChartType.Column_Clustered_3D:
          case ExcelChartType.Column_Stacked_3D:
          case ExcelChartType.Column_Stacked_100_3D:
          case ExcelChartType.Column_3D:
          case ExcelChartType.Bar_Clustered_3D:
          case ExcelChartType.Bar_Stacked_3D:
          case ExcelChartType.Bar_Stacked_100_3D:
          case ExcelChartType.Cylinder_Clustered:
          case ExcelChartType.Cylinder_Stacked:
          case ExcelChartType.Cylinder_Stacked_100:
          case ExcelChartType.Cylinder_Bar_Clustered:
          case ExcelChartType.Cylinder_Bar_Stacked:
          case ExcelChartType.Cylinder_Bar_Stacked_100:
          case ExcelChartType.Cylinder_Clustered_3D:
          case ExcelChartType.Cone_Clustered:
          case ExcelChartType.Cone_Stacked:
          case ExcelChartType.Cone_Stacked_100:
          case ExcelChartType.Cone_Bar_Clustered:
          case ExcelChartType.Cone_Bar_Stacked:
          case ExcelChartType.Cone_Bar_Stacked_100:
          case ExcelChartType.Cone_Clustered_3D:
          case ExcelChartType.Pyramid_Clustered:
          case ExcelChartType.Pyramid_Stacked:
          case ExcelChartType.Pyramid_Stacked_100:
          case ExcelChartType.Pyramid_Bar_Clustered:
          case ExcelChartType.Pyramid_Bar_Stacked:
          case ExcelChartType.Pyramid_Bar_Stacked_100:
          case ExcelChartType.Pyramid_Clustered_3D:
            iResult = SerializeBar3DChart( writer, chart, firstSeries );
            break;

          case ExcelChartType.Line:
          case ExcelChartType.Line_Stacked:
          case ExcelChartType.Line_Stacked_100:
          case ExcelChartType.Line_Markers:
          case ExcelChartType.Line_Markers_Stacked:
          case ExcelChartType.Line_Markers_Stacked_100:
            iResult = SerializeLineChart( writer, chart, firstSeries );
            break;

          case ExcelChartType.Line_3D:
            iResult = SerializeLine3DChart( writer, chart, firstSeries );
            break;

          case ExcelChartType.Pie:
          case ExcelChartType.Pie_Exploded:
            iResult = SerializePieChart( writer, chart, firstSeries );
            break;

          case ExcelChartType.Pie_3D:
          case ExcelChartType.Pie_Exploded_3D:
            iResult = SerializePie3DChart( writer, chart, firstSeries );
            break;

          case ExcelChartType.PieOfPie:
          case ExcelChartType.Pie_Bar:
            iResult = SerializeOfPieChart( writer, chart, firstSeries );
            break;

          case ExcelChartType.Scatter_Markers:
          case ExcelChartType.Scatter_SmoothedLine_Markers:
          case ExcelChartType.Scatter_SmoothedLine:
          case ExcelChartType.Scatter_Line_Markers:
          case ExcelChartType.Scatter_Line:
            iResult = SerializeScatterChart( writer, chart, firstSeries );
            break;

          case ExcelChartType.Area:
          case ExcelChartType.Area_Stacked:
          case ExcelChartType.Area_Stacked_100:
            iResult = SerializeAreaChart( writer, chart, firstSeries );
            break;

          case ExcelChartType.Area_3D:
          case ExcelChartType.Area_Stacked_3D:
          case ExcelChartType.Area_Stacked_100_3D:
            iResult = SerializeArea3DChart( writer, chart, firstSeries );
            break;

          case ExcelChartType.Doughnut:
          case ExcelChartType.Doughnut_Exploded:
            iResult = SerializeDoughnutChart( writer, chart, firstSeries );
            break;

          case ExcelChartType.Radar:
          case ExcelChartType.Radar_Markers:
          case ExcelChartType.Radar_Filled:
            iResult = SerializeRadarChart( writer, chart, firstSeries );
            break;

          case ExcelChartType.Surface_3D:
          case ExcelChartType.Surface_NoColor_3D:
            iResult = SerializeSurface3DChart( writer, chart, firstSeries );
            break;

          case ExcelChartType.Surface_Contour:
          case ExcelChartType.Surface_NoColor_Contour:
            iResult = SerializeSurfaceChart( writer, chart, firstSeries );
            break;

          case ExcelChartType.Bubble:
          case ExcelChartType.Bubble_3D:
            iResult = SerializeBubbleChart( writer, chart, firstSeries );
            break;

          case ExcelChartType.Stock_HighLowClose:
          case ExcelChartType.Stock_OpenHighLowClose:
          case ExcelChartType.Stock_VolumeHighLowClose:
          case ExcelChartType.Stock_VolumeOpenHighLowClose:
            iResult = SerializeStockChart( writer, chart, firstSeries );
            break;
        }
      }

      return iResult;
    }
    /// <summary>
        /// Serializes main chart tag.
        /// </summary>
        /// <param name="writer">XmlWriter to serialize into.</param>
        /// <param name="chart">Chart to serialize.</param>
        /// <param name="groupIndex">Index of the series group to serialize.</param>
        /// <returns>Number of the serialized series.</returns>
        private void SerializeMainChartTypeTag(XmlWriter writer, ChartImpl chart)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (chart == null)
                throw new ArgumentNullException("chart");

            int iResult = 0;
            if( chart.HasPivotSource )
            {
                switch (chart.PivotChartType)
                {
                    case ExcelChartType.Column_Clustered:
                    case ExcelChartType.Column_Stacked:
                    case ExcelChartType.Column_Stacked_100:
                    case ExcelChartType.Bar_Clustered:
                    case ExcelChartType.Bar_Stacked:
                    case ExcelChartType.Bar_Stacked_100:
                        SerializeBarChart(writer, chart);
                        break;

                    case ExcelChartType.Column_Clustered_3D:
                    case ExcelChartType.Column_Stacked_3D:
                    case ExcelChartType.Column_Stacked_100_3D:
                    case ExcelChartType.Column_3D:
                    case ExcelChartType.Bar_Clustered_3D:
                    case ExcelChartType.Bar_Stacked_3D:
                    case ExcelChartType.Bar_Stacked_100_3D:
                    case ExcelChartType.Cylinder_Clustered:
                    case ExcelChartType.Cylinder_Stacked:
                    case ExcelChartType.Cylinder_Stacked_100:
                    case ExcelChartType.Cylinder_Bar_Clustered:
                    case ExcelChartType.Cylinder_Bar_Stacked:
                    case ExcelChartType.Cylinder_Bar_Stacked_100:
                    case ExcelChartType.Cylinder_Clustered_3D:
                    case ExcelChartType.Cone_Clustered:
                    case ExcelChartType.Cone_Stacked:
                    case ExcelChartType.Cone_Stacked_100:
                    case ExcelChartType.Cone_Bar_Clustered:
                    case ExcelChartType.Cone_Bar_Stacked:
                    case ExcelChartType.Cone_Bar_Stacked_100:
                    case ExcelChartType.Cone_Clustered_3D:
                    case ExcelChartType.Pyramid_Clustered:
                    case ExcelChartType.Pyramid_Stacked:
                    case ExcelChartType.Pyramid_Stacked_100:
                    case ExcelChartType.Pyramid_Bar_Clustered:
                    case ExcelChartType.Pyramid_Bar_Stacked:
                    case ExcelChartType.Pyramid_Bar_Stacked_100:
                    case ExcelChartType.Pyramid_Clustered_3D:
                        SerializeBar3DChart(writer, chart);
                        break;

                    case ExcelChartType.Line:
                    case ExcelChartType.Line_Stacked:
                    case ExcelChartType.Line_Stacked_100:
                    case ExcelChartType.Line_Markers:
                    case ExcelChartType.Line_Markers_Stacked:
                    case ExcelChartType.Line_Markers_Stacked_100:
                        SerializeLineChart(writer, chart);
                        break;

                    case ExcelChartType.Line_3D:
                        SerializeLine3DChart(writer, chart);
                        break;

                    case ExcelChartType.Pie:
                    case ExcelChartType.Pie_Exploded:
                        SerializePieChart(writer, chart);
                        break;

                    case ExcelChartType.Pie_3D:
                    case ExcelChartType.Pie_Exploded_3D:
                        SerializePie3DChart(writer, chart);
                        break;

                    case ExcelChartType.PieOfPie:
                    case ExcelChartType.Pie_Bar:
                        SerializeOfPieChart(writer, chart);
                        break;

                    case ExcelChartType.Area:
                    case ExcelChartType.Area_Stacked:
                    case ExcelChartType.Area_Stacked_100:
                        SerializeAreaChart(writer, chart);
                        break;

                    case ExcelChartType.Area_3D:
                    case ExcelChartType.Area_Stacked_3D:
                    case ExcelChartType.Area_Stacked_100_3D:
                        SerializeArea3DChart(writer, chart);
                        break;

                    case ExcelChartType.Doughnut:
                    case ExcelChartType.Doughnut_Exploded:
                        SerializeDoughnutChart(writer, chart);
                        break;

                    case ExcelChartType.Radar:
                    case ExcelChartType.Radar_Markers:
                    case ExcelChartType.Radar_Filled:
                        SerializeRadarChart(writer, chart);
                        break;

                    case ExcelChartType.Surface_3D:
                    case ExcelChartType.Surface_NoColor_3D:
                        SerializeSurface3DChart(writer, chart);
                        break;

                    case ExcelChartType.Surface_Contour:
                    case ExcelChartType.Surface_NoColor_Contour:
                        SerializeSurfaceChart(writer, chart);
                        break;

                }
            }

        }
        /// <summary>
        /// Serializes the radar chart.
        /// </summary>
        /// <param name="writer">XmlWriter to serialize into.</param>
        /// <param name="chart">Chart to serialize.</param>
        private void SerializeRadarChart(XmlWriter writer, ChartImpl chart)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (chart == null)
                throw new ArgumentNullException("chart");

            writer.WriteStartElement(ChartConstants.RadarChartTag, ChartConstants.CNamespace);
            Excel2007RadarStyle radarStyle = (Excel2007RadarStyle)chart.PivotChartType;
            ChartSerializatorCommon.SerializeValueTag(writer, ChartConstants.RadarStyleTag, radarStyle.ToString());
        }
        /// <summary>
    /// Serializes radar chart.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="chart">Chart to serialize.</param>
    /// <param name="firstSeries">First series in the list of series with the
    /// same formatting to serialize.</param>
    /// <returns>Number of the serialized series.</returns>
    private int SerializeRadarChart( XmlWriter writer, ChartImpl chart, ChartSerieImpl firstSeries )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      writer.WriteStartElement( ChartConstants.RadarChartTag, ChartConstants.CNamespace );
      if (chart.RadarStyle != null)
          ChartSerializatorCommon.SerializeValueTag(writer, ChartConstants.RadarStyleTag, chart.RadarStyle);
      else
      {
          Excel2007RadarStyle radarStyle = (Excel2007RadarStyle)firstSeries.SerieType;
          ChartSerializatorCommon.SerializeValueTag(writer, ChartConstants.RadarStyleTag, radarStyle.ToString());
          SerializeVaryColors(writer, firstSeries);
      }
      int iResult = SerializeChartSeries( writer, chart, firstSeries, SerializeRadarSeries );
      SerializeBarAxisId( writer, chart, firstSeries );

      writer.WriteEndElement();

      return iResult;
    }
    /// <summary>
    /// Serializes scatter chart.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="chart">Chart to serialize.</param>
    /// <param name="firstSeries">First series in the list of series with the
    /// same formatting to serialize.</param>
    /// <returns>Number of the serialized series.</returns>
    private int SerializeScatterChart( XmlWriter writer, ChartImpl chart, ChartSerieImpl firstSeries )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      writer.WriteStartElement( ChartConstants.ScatterChartTag, ChartConstants.CNamespace );

      Excel2007ScatterStyle scatterStyle = ( Excel2007ScatterStyle )firstSeries.SerieType;
      ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.ScatterStyleTag, scatterStyle.ToString() );
      SerializeVaryColors( writer, firstSeries );
      int iResult = SerializeChartSeries( writer, chart, firstSeries, SerializeScatterSeries );

      SerializeUpDownBars( writer, chart, firstSeries );
      SerializeBarAxisId( writer, chart, firstSeries );

      writer.WriteEndElement();

      return iResult;
    }
    /// <summary>
        /// Serializes the pie chart.
        /// </summary>
        /// <param name="writer">XmlWriter to serialize into.</param>
        /// <param name="chart">Chart to serialize.</param>
        private void SerializePieChart(XmlWriter writer, ChartImpl chart)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (chart == null)
                throw new ArgumentNullException("chart");
            writer.WriteStartElement(ChartConstants.PieChartTag, ChartConstants.CNamespace);
            ChartSerializatorCommon.SerializeBoolValueTag(writer, ChartConstants.VaryColorsTag, true);
        }
        /// <summary>
    /// Serializes pie chart.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="chart">Chart to serialize.</param>
    /// <param name="firstSeries">First series in the list of series with the
    /// same formatting to serialize.</param>
    /// <returns>Number of the serialized series.</returns>
    private int SerializePieChart( XmlWriter writer, ChartImpl chart, ChartSerieImpl firstSeries )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      writer.WriteStartElement( ChartConstants.PieChartTag, ChartConstants.CNamespace );
      int iResult = SerializePieCommon( writer, chart, firstSeries );
      IChartFormat commonOptions = firstSeries.SerieFormat.CommonSerieOptions;

      int iFirstSliceAngle = commonOptions.FirstSliceAngle;
      ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.FirstSliceAngleTag,
        iFirstSliceAngle.ToString() );

      writer.WriteEndElement();
      return iResult;
    }
    /// <summary>
        /// Serializes the pie3 D chart.
        /// </summary>
        /// <param name="writer">XmlWriter to serialize into.</param>
        /// <param name="chart">Chart to serialize.</param>
        private void SerializePie3DChart(XmlWriter writer, ChartImpl chart)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (chart == null)
                throw new ArgumentNullException("chart");

            writer.WriteStartElement(ChartConstants.Pie3DChartTag, ChartConstants.CNamespace);
            ChartSerializatorCommon.SerializeBoolValueTag(writer, ChartConstants.VaryColorsTag, true);
        }
        /// <summary>
    /// Serializes 3-D pie chart.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="chart">Chart to serialize.</param>
    /// <param name="firstSeries">First series in the list of series with the
    /// same formatting to serialize.</param>
    /// <returns>Number of the serialized series.</returns>
    private int SerializePie3DChart( XmlWriter writer, ChartImpl chart, ChartSerieImpl firstSeries )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      writer.WriteStartElement( ChartConstants.Pie3DChartTag, ChartConstants.CNamespace );
      int iResult = SerializePieCommon( writer, chart, firstSeries );
      writer.WriteEndElement();

      return iResult;
    }
    /// <summary>
        /// Serializes the of pie chart.
        /// </summary>
        /// <param name="writer">XmlWriter to serialize into.</param>
        /// <param name="chart">Chart to serialize.</param>
        private void SerializeOfPieChart(XmlWriter writer, ChartImpl chart)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (chart == null)
                throw new ArgumentNullException("chart");

            writer.WriteStartElement(ChartConstants.OfPieChartTag, ChartConstants.CNamespace);
            string strOfPieType = (chart.PivotChartType == ExcelChartType.PieOfPie) ?
              ChartConstants.OfPieTypePie :
              ChartConstants.OfPieTypeBar;

            ChartSerializatorCommon.SerializeValueTag(writer, ChartConstants.OfPieTypeTag, strOfPieType);
            ChartSerializatorCommon.SerializeBoolValueTag(writer, ChartConstants.VaryColorsTag, true);
        }
        /// <summary>
    /// Serializes pie of pie or pie of bar chart.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="chart">Chart to serialize.</param>
    /// <param name="firstSeries">First series in the list of series with the
    /// same formatting to serialize.</param>
    /// <returns>Number of the serialized series.</returns>
    private int SerializeOfPieChart( XmlWriter writer, ChartImpl chart, ChartSerieImpl firstSeries )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      writer.WriteStartElement( ChartConstants.OfPieChartTag, ChartConstants.CNamespace );
      string strOfPieType = ( firstSeries.SerieType == ExcelChartType.PieOfPie ) ?
        ChartConstants.OfPieTypePie :
        ChartConstants.OfPieTypeBar;

      ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.OfPieTypeTag, strOfPieType );

      int iResult = SerializePieCommon( writer, chart, firstSeries );

      IChartFormat commonOptions = firstSeries.SerieFormat.CommonSerieOptions;
      ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.GapWidthTag,
        commonOptions.GapWidth.ToString() );

      int iSplitValue = commonOptions.SplitValue;

      if( iSplitValue != 0 )
      {
        Excel2007SplitType splitType = ( Excel2007SplitType )commonOptions.SplitType;
        ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.SplitTypeTag,
          splitType.ToString() );

        ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.SplitPosTag,
          iSplitValue.ToString() );
      }
      // custSplit
      ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.SecondPieSizeTag,
        commonOptions.PieSecondSize.ToString() );
      // serlines

      writer.WriteElementString( ChartConstants.SeriesLinesTag, ChartConstants.CNamespace, string.Empty );
      writer.WriteEndElement();

      return iResult;
    }
    /// <summary>
    /// Serializes stock chart.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="chart">Chart to serialize.</param>
    /// <param name="firstSeries">First series in the list of series with the
    /// same formatting to serialize.</param>
    /// <returns>Number of the serialized series.</returns>
    private int SerializeStockChart( XmlWriter writer, ChartImpl chart, ChartSerieImpl firstSeries )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      writer.WriteStartElement( ChartConstants.StockChartTag, ChartConstants.CNamespace );

      IChartSeries arrSeries = chart.Series;
      for( int i = 0, len = arrSeries.Count; i < len; i++ )
      {
        ChartSerieImpl series = ( ChartSerieImpl )arrSeries[ i ];
        SerializeLineSeries( writer, series );
      }

      ChartFormatImpl format = ( ChartFormatImpl )chart.PrimaryFormats[ 0 ];

      if( format.IsChartChartLine && format.LineStyle == ExcelDropLineStyle.HiLow )
      {
        writer.WriteElementString( ChartConstants.HiLowLinesTag, ChartConstants.CNamespace, string.Empty );
      }

      SerializeUpDownBars( writer, chart, firstSeries );
      SerializeBarAxisId( writer, chart, firstSeries );

      writer.WriteEndElement();

      return arrSeries.Count;
    }
    /// <summary>
        /// Serializes the doughnut chart.
        /// </summary>
        /// <param name="writer">XmlWriter to serialize into.</param>
        /// <param name="chart">Chart to serialize.</param>
        private void SerializeDoughnutChart(XmlWriter writer, ChartImpl chart)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (chart == null)
                throw new ArgumentNullException("chart");

            writer.WriteStartElement(ChartConstants.DoughnutChartTag, ChartConstants.CNamespace);
            ChartSerializatorCommon.SerializeBoolValueTag(writer, ChartConstants.VaryColorsTag, true);
            ChartSerializatorCommon.SerializeValueTag(writer, ChartConstants.DoughnutHoleSizeTag,
              "50");
        }
        /// <summary>
    /// Serializes doughnut chart.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="chart">Chart to serialize.</param>
    /// <param name="firstSeries">First series in the list of series with the
    /// same formatting to serialize.</param>
    /// <returns>Number of the serialized series.</returns>
    private int SerializeDoughnutChart( XmlWriter writer, ChartImpl chart, ChartSerieImpl firstSeries )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      writer.WriteStartElement( ChartConstants.DoughnutChartTag, ChartConstants.CNamespace );
      int iResult = SerializePieCommon( writer, chart, firstSeries );
      IChartFormat commonOptions = firstSeries.SerieFormat.CommonSerieOptions;

      int iFirstSliceAngle = commonOptions.FirstSliceAngle;
      ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.FirstSliceAngleTag,
        iFirstSliceAngle.ToString() );

      int iHoleSize = commonOptions.DoughnutHoleSize;
      ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.DoughnutHoleSizeTag,
        iHoleSize.ToString() );

      writer.WriteEndElement();

      return iResult;
    }
    /// <summary>
    /// Serializes common properties of pie charts.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="chart">Chart to serialize.</param>
    /// <param name="firstSeries">First series in the list of series with the
    /// same formatting to serialize.</param>
    /// <returns>Number of the serialized series.</returns>
    private int SerializePieCommon( XmlWriter writer, ChartImpl chart, ChartSerieImpl firstSeries )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      SerializeVaryColors( writer, firstSeries );

      return SerializeChartSeries( writer, chart, firstSeries, SerializePieSeries );
    }
    /// <summary>
    /// Serializes data lables.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="dataLabels">DataLabels to serialize.</param>
    /// <param name="series">Parent series.</param>
    private void SerializeDataLabels( XmlWriter writer, IChartDataLabels dataLabels,
      ChartSerieImpl series )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( dataLabels == null )
        throw new ArgumentNullException( "dataLabels" );

      if( series == null )
        throw new ArgumentNullException( "series" );

      writer.WriteStartElement( ChartConstants.DataLabelsTag, ChartConstants.CNamespace );

      ChartImpl parentChart = series.ParentChart;
      ChartDataPointsCollection dataPoints = ( ChartDataPointsCollection )series.DataPoints;

      if( dataPoints.DeninedDPCount > 0 )
      {
        foreach( ChartDataPointImpl dataPoint in dataPoints )
        {
          if( !dataPoint.IsDefault && dataPoint.HasDataLabels )
          {
            SerializeDataLabel( writer, dataPoint.DataLabels, dataPoint.Index, parentChart );
          }
        }
      }

      if ((series.DataPoints.DefaultDataPoint.DataLabels as ChartDataLabelsImpl).NumberFormat != null)
          SerializeNumFormat(writer, series);

      SerializeDataLabelSettings( writer, dataLabels, parentChart, true );
      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes number format.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="numFormats">Number format serialize.</param>
    /// <param name="series">Parent series.</param>
    private void SerializeNumFormat(XmlWriter writer, ChartSerieImpl series)
    {
        if (writer == null)
            throw new ArgumentNullException("writer");

        if (series == null)
            throw new ArgumentNullException("series");

        writer.WriteStartElement(ChartConstants.NumberFormatTag, ChartConstants.CNamespace);

        writer.WriteAttributeString(ChartConstants.FormatCodeAttribute, (series.DataPoints.DefaultDataPoint.DataLabels as ChartDataLabelsImpl).NumberFormat);

        writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes data label for single data point.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="dataLabels">Data labels to serialize.</param>
    /// <param name="index">Data point index.</param>
    /// <param name="chart">Parent chart object.</param>
    private void SerializeDataLabel( XmlWriter writer, IChartDataLabels dataLabels,
      int index, ChartImpl chart )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( dataLabels == null )
        throw new ArgumentNullException( "dataLabels" );

      writer.WriteStartElement( ChartConstants.DataLabelTag, ChartConstants.CNamespace );
      ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.IndexTag, index.ToString() );

      // layout for data labels
      IChartLayout layout = null;
      if ((dataLabels as ChartDataLabelsImpl) != null)
      {
          layout = (dataLabels as ChartDataLabelsImpl).Layout;
          if (layout != null)
          {
              IChartManualLayout manualLayout = layout.ManualLayout;

              if ((manualLayout != null) && 
                  ((manualLayout.LayoutTarget != LayoutTargets.auto) ||
                   (manualLayout.LeftMode != LayoutModes.auto) ||
                   (manualLayout.TopMode != LayoutModes.auto) ||
                   (manualLayout.Left != 0) ||
                   (manualLayout.Top != 0) ||
                   (manualLayout.WidthMode != LayoutModes.auto) ||
                   (manualLayout.HeightMode != LayoutModes.auto) ||
                   (manualLayout.Width != 0) ||
                   (manualLayout.Height != 0)))
              {
                  ChartSerializatorCommon.SerializeLayout(writer, (dataLabels as ChartDataLabelsImpl));
              }
          }
      }

      IInternalChartTextArea textArea = dataLabels as IInternalChartTextArea;

      if( !string.IsNullOrEmpty( textArea.Text ) )
      {
        //ChartSerializatorCommon.SerializeTextArea( writer, textArea, chart.ParentWorkbook, relations );
        WorkbookImpl book = chart.ParentWorkbook;

        if ((textArea as ChartTextAreaImpl) != null)
        {
            layout = (textArea as ChartTextAreaImpl).Layout;
            if (layout != null)
                ChartSerializatorCommon.SerializeLayout(writer, (textArea as ChartTextAreaImpl));
        }

        ChartSerializatorCommon.SerializeTextAreaText( writer, textArea, book, ChartAxisParser.DefaultFontSize );
      }

      SerializeDataLabelSettings( writer, dataLabels, chart, false );
      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes data labels settings.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="dataLabels">Data labels to serialize.</param>
    /// <param name="chart">Parent chart.</param>
    private void SerializeDataLabelSettings( XmlWriter writer, IChartDataLabels dataLabels, ChartImpl chart, bool SerializeLeaderLines )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( dataLabels == null )
        throw new ArgumentNullException( "dataLabels" );

      if ((dataLabels as ChartDataLabelsImpl).ShowTextProperties)
      {
          ChartSerializatorCommon.SerializeFrameFormat(writer, dataLabels.FrameFormat, chart, false);
          bool bIsDefault = ((ChartDataLabelsImpl)dataLabels).ParagraphType == ChartParagraphType.CustomDefault;
          if (bIsDefault)
              SerializeDefaultTextFormatting(writer, dataLabels, chart.Workbook, ChartAxisParser.DefaultFontSize);
      }
      else if ((dataLabels as ChartDataLabelsImpl).IsDelete)
      {
          ChartSerializatorCommon.SerializeBoolValueTag(writer, ChartConstants.DeleteTag, true);
      }

      ExcelDataLabelPosition labelPosition = dataLabels.Position;

      if( labelPosition != ExcelDataLabelPosition.Automatic && labelPosition!=ExcelDataLabelPosition.Moved )
      {
        Excel2007DataLabelPos position = ( Excel2007DataLabelPos )labelPosition;
        ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.DataLabelPosTag, position.ToString() );
      }
      if ((chart as WorksheetBaseImpl).Workbook.Version == ExcelVersion.Excel2007)          
      {
          if ((dataLabels as ChartDataLabelsImpl).m_bHasLegendKeyOption || dataLabels.IsLegendKey || chart.DestinationType == ExcelChartType.Scatter_Line_Markers)
              ChartSerializatorCommon.SerializeBoolValueTag(writer, ChartConstants.ShowLegendKeyTag, dataLabels.IsLegendKey);
          if ((dataLabels as ChartDataLabelsImpl).m_bHasValueOption || dataLabels.IsValue || chart.DestinationType == ExcelChartType.Scatter_Line_Markers)
              ChartSerializatorCommon.SerializeBoolValueTag(writer, ChartConstants.ShowValueTag, dataLabels.IsValue);
          if ((dataLabels as ChartDataLabelsImpl).m_bHasCategoryOption || dataLabels.IsCategoryName || chart.DestinationType == ExcelChartType.Scatter_Line_Markers)
              ChartSerializatorCommon.SerializeBoolValueTag(writer, ChartConstants.ShowCategoryTag, dataLabels.IsCategoryName);
          if ((dataLabels as ChartDataLabelsImpl).m_bHasSeriesOption || dataLabels.IsSeriesName || chart.DestinationType == ExcelChartType.Scatter_Line_Markers)
              ChartSerializatorCommon.SerializeBoolValueTag(writer, ChartConstants.ShowSeriesNameTag, dataLabels.IsSeriesName);
          if ((dataLabels as ChartDataLabelsImpl).m_bHasPercentageOption || dataLabels.IsPercentage || chart.DestinationType == ExcelChartType.Scatter_Line_Markers)
              ChartSerializatorCommon.SerializeBoolValueTag(writer, ChartConstants.ShowPercentageTag, dataLabels.IsPercentage);
          if ((dataLabels as ChartDataLabelsImpl).m_bHasBubbleSizeOption || dataLabels.IsBubbleSize || chart.DestinationType == ExcelChartType.Scatter_Line_Markers)
              ChartSerializatorCommon.SerializeBoolValueTag(writer, ChartConstants.ShowBubbleSizeTag, dataLabels.IsBubbleSize);
      }
      else
      {
          if (dataLabels.IsLegendKey || dataLabels.IsValue || dataLabels.IsCategoryName || dataLabels.IsSeriesName || dataLabels.IsPercentage || dataLabels.IsBubbleSize || chart.DestinationType == ExcelChartType.Scatter_Line_Markers)
          {
              ChartSerializatorCommon.SerializeBoolValueTag(writer, ChartConstants.ShowLegendKeyTag, dataLabels.IsLegendKey);
              ChartSerializatorCommon.SerializeBoolValueTag(writer, ChartConstants.ShowValueTag, dataLabels.IsValue);
              ChartSerializatorCommon.SerializeBoolValueTag(writer, ChartConstants.ShowCategoryTag, dataLabels.IsCategoryName);
              ChartSerializatorCommon.SerializeBoolValueTag(writer, ChartConstants.ShowSeriesNameTag, dataLabels.IsSeriesName);
              ChartSerializatorCommon.SerializeBoolValueTag(writer, ChartConstants.ShowPercentageTag, dataLabels.IsPercentage);
              ChartSerializatorCommon.SerializeBoolValueTag(writer, ChartConstants.ShowBubbleSizeTag, dataLabels.IsBubbleSize);
          }
      }
      string strDelimiter = dataLabels.Delimiter;
      if( strDelimiter != null )
        writer.WriteElementString( ChartConstants.DataLabelsSeparatorTag, ChartConstants.CNamespace, strDelimiter );

      if (chart.IsChartPie && SerializeLeaderLines)
          ChartSerializatorCommon.SerializeBoolValueTag(writer, ChartConstants.ShowLeaderLineTag, chart.ChartFormat.ShowLeaderLines);
    }
    /// <summary>
    /// Serializes default text formatting.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="textFormatting">Text formatting to serialize.</param>
    /// <param name="book">Parent workbook.</param>
    private void SerializeDefaultTextFormatting( XmlWriter writer,
      IChartTextArea textFormatting, IWorkbook book, double defaultFontSize )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( book == null )
        throw new ArgumentNullException( "book" );

      if( textFormatting == null )
        return;

      writer.WriteStartElement( ChartConstants.TextPropertiesTag, ChartConstants.CNamespace );

      writer.WriteStartElement( Drawings.TextBodyPropertiesTag, Drawings.ANamespace );
      if (textFormatting.TextRotationAngle != 0 )
      {
          int iAngle = textFormatting.TextRotationAngle  * ChartAxisSerializator.TextRotationMultiplier;
          writer.WriteAttributeString(Drawings.TextRotationAttribute, iAngle.ToString());
      }
      writer.WriteEndElement();

      writer.WriteStartElement( Drawings.ListStylesTag, Drawings.ANamespace );
      writer.WriteEndElement();

      writer.WriteStartElement( Drawings.Paragraphs, Drawings.ANamespace );
      writer.WriteStartElement( Drawings.ParagraphProperties, Drawings.ANamespace );

      ChartSerializatorCommon.SerializeParagraphRunProperites( writer, textFormatting,
        Drawings.DefaultParagraphProperites, book, defaultFontSize );

      writer.WriteEndElement();
      writer.WriteEndElement();

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes IsVaryColors option.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="firstSeries">First series in the list of series with the
    /// same formatting to serialize vary colors option for.</param>
    private void SerializeVaryColors( XmlWriter writer, ChartSerieImpl firstSeries )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( firstSeries == null )
        throw new ArgumentNullException( "firstSeries" );

      bool bVaryColor = firstSeries.SerieFormat.CommonSerieOptions.IsVaryColor;
      ChartSerializatorCommon.SerializeBoolValueTag( writer, ChartConstants.VaryColorsTag, bVaryColor );
    }
    /// <summary>
    /// Serializes single chart series for bar chart.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="series">Series to serialize.</param>
    private void SerializeBarSeries( XmlWriter writer, ChartSerieImpl series )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( series == null )
        throw new ArgumentNullException( "series" );
      
      SerializeSeriesCommonWithoutEnd( writer, series ,series.IsFiltered);

      if( ( series.DataPoints.DefaultDataPoint as ChartDataPointImpl ).HasDataLabels )
        SerializeDataLabels( writer, series.DataPoints.DefaultDataPoint.DataLabels, series );

      SerializeTrendlines( writer, series.TrendLines, series.ParentBook );
      SerializeErrorBars( writer, series );
      if (categoryFilter == 0)
      {
          findFilter = FindFiltered(series);
          categoryFilter++;
          if (findFilter)
          {
              UpdateCategoryLabel(series);
              UpdateFilteredValuesRange(series);
          }
      }
      if (!findFilter)
      {
          if ((series.ParentChart as ChartImpl).CategoryLabelLevel == ExcelCategoriesLabelLevel.CategoriesLabelLevelAll)
          {
              SerializeSeriesCategory(writer, series);
          }
          SerializeSeriesValues(writer, series);
      }
      else
      {
          if ((series.ParentChart as ChartImpl).CategoryLabelLevel == ExcelCategoriesLabelLevel.CategoriesLabelLevelAll)
          {
              SerializeFilteredCategory(writer, series, findFilter);
          }
          SerializeFilteredValues(writer, series, findFilter);
      }
      if ((series.ParentChart as ChartImpl).CategoryLabelLevel != ExcelCategoriesLabelLevel.CategoriesLabelLevelAll || (series.ParentChart as ChartImpl).SeriesNameLevel != ExcelSeriesNameLevel.SeriesNameLevelAll)
      {
          writer.WriteStartElement(Excel2007Serializator.Extensionlist, ChartConstants.CNamespace);
          if( (series.ParentChart as ChartImpl).SeriesNameLevel != ExcelSeriesNameLevel.SeriesNameLevelAll)
          {
              SerializeFilteredSeriesOrCategoryName(writer, series, true);
          }
          if ((series.ParentChart as ChartImpl).CategoryLabelLevel != ExcelCategoriesLabelLevel.CategoriesLabelLevelAll)
          {
              SerializeFilteredSeriesOrCategoryName(writer, series, false);
          }
          writer.WriteEndElement();
      }
         
      writer.WriteEndElement();
    }
    /// <summary>
    /// Find the category filter
    /// </summary>
    /// <param name="series"></param>
    /// <returns></returns>
    private bool FindFiltered(ChartSerieImpl series)
    {
        ChartImpl chart = series.ParentChart as ChartImpl;
        IChartCategories category = chart.Categories;
        for (int i = 0; i < category.Count; i++)
        {
            if (category[i].IsFiltered)
                return true;
        }
        return false; ;
    }
    /// <summary>
    /// To update the filtered value range
    /// </summary>
    /// <param name="series"></param>
    private void UpdateFilteredValuesRange(ChartSerieImpl series)
    {
        ChartImpl chart = series.ParentChart as ChartImpl;
        if ((chart.Categories[0] as ChartCategory).Filter_customize)
        {
            WorksheetBaseImpl sheet = chart.DataRange.Worksheet as WorksheetBaseImpl;
            IChartSeries seriescollection = chart.Series;
            IWorksheet sheet1 = sheet as IWorksheet;
            IChartCategories categories = chart.Categories as ChartCategoryCollection;
            string[] categotistring = new string[series.Values.Count * categories.Count];
            int startrow = 0;
            int endrow = 0;
            string valuesranges = string.Empty;
            int startcolumn = 0;
            int endcolumn = 0;
            int count = 0;
            IRange range = null;
            for (int i = 0; i < categories[0].Values.Count; i++)
            {
                for (int j = 0; j < categories.Count; j++)
                {
                    if (!categories[j].IsFiltered && startrow == 0)
                    {
                        startrow = categories[j].Values.Cells[i].Row;
                        startcolumn = categories[j].Values.Cells[i].Column;
                    }
                    else if (startrow != 0 && categories[j].IsFiltered)
                    {
                        endcolumn = categories[j].Values.Cells[i].Column;
                        endrow = categories[j].Values.Cells[i].Row;
                    }
                    if (startrow != 0 && endrow != 0)
                    {

                        if (startrow == endrow)
                        {
                            range = sheet1.Range[startrow, startcolumn, endrow, endcolumn - 1];
                        }
                        else
                        {
                            range = sheet1.Range[startrow, startcolumn, endrow - 1, endcolumn];
                        }
                        //categotistring[count]=range.AddressGlobal;
                        valuesranges = valuesranges == string.Empty ? "(" + range.AddressGlobal : (valuesranges + "," + range.AddressGlobal);
                        count++;
                        startrow = 0;
                        endrow = 0;
                    }
                }
                if (startrow != 0)
                {
                    endcolumn = categories[categories.Count - 1].Values.Cells[i].Column;
                    endrow = categories[categories.Count - 1].Values.Cells[i].Row;
                    if (startrow == endrow)
                    {
                        range = sheet1.Range[startrow, startcolumn, endrow, endcolumn];
                        startrow = 0;
                        endrow = 0;
                    }
                    else
                    {
                        range = sheet1.Range[startrow, startcolumn, endrow, endcolumn];
                        startrow = 0;
                        endrow = 0;
                    }
                    
                    valuesranges = valuesranges == string.Empty ? "(" + range.AddressGlobal : (valuesranges + "," + range.AddressGlobal);
                    count++;
                }
                valuesranges = valuesranges + ")";
                (seriescollection[i] as ChartSerieImpl).FilteredValue = valuesranges.Replace("\'", "");
                valuesranges = string.Empty;

            }
        }

    }
    /// <summary>
    /// Update the category filterlabel range
    /// </summary>
    /// <param name="series"></param>
    private void UpdateCategoryLabel(ChartSerieImpl series)
    {
        ChartImpl chart = series.ParentChart as ChartImpl;
        if ((chart.Categories[0] as ChartCategory).Filter_customize && series.CategoryLabels!=null)
        {
            WorksheetBaseImpl sheet = chart.DataRange.Worksheet as WorksheetBaseImpl;
            IChartSeries seriescollection = chart.Series;
            IWorksheet sheet1 = sheet as IWorksheet;
            IChartCategories categories = chart.Categories as ChartCategoryCollection;
            string[] categotistring = new string[series.Values.Count * categories.Count];
            int startrow = 0;
            int endrow = 0;
            string valuesranges = string.Empty;
            int startcolumn = 0;
            int endcolumn = 0;
            int count = 0;
            IRange range = null;
            {
                for (int j = 0; j < categories.Count; j++)
                {
                    if (!categories[j].IsFiltered && startrow == 0)
                    {
                        startrow = categories[j].CategoryLabel.Cells[j].Row;
                        startcolumn = categories[j].CategoryLabel.Cells[j].Column;
                    }
                    else if (startrow != 0 && categories[j].IsFiltered)
                    {
                        endcolumn = categories[j].CategoryLabel.Cells[j].Column;
                        endrow = categories[j].CategoryLabel.Cells[j].Row;
                    }
                    if (startrow != 0 && endrow != 0)
                    {

                        if (startrow == endrow)
                        {
                            range = sheet1.Range[startrow, startcolumn, endrow, endcolumn - 1];
                        }
                        else
                        {
                            range = sheet1.Range[startrow, startcolumn, endrow - 1, endcolumn];
                        }
                        //categotistring[count]=range.AddressGlobal;
                        valuesranges = valuesranges == string.Empty ? "(" + range.AddressGlobal : (valuesranges + "," + range.AddressGlobal);
                        count++;
                        startrow = 0;
                        endrow = 0;
                    }
                }
                if (startrow != 0)
                {
                    endcolumn = categories[categories.Count - 1].CategoryLabel.Cells[categories.Count - 1].Column;
                    endrow = categories[categories.Count - 1].CategoryLabel.Cells[categories.Count - 1].Row;
                    if (startrow == endrow)
                    {
                        range = sheet1.Range[startrow, startcolumn, endrow, endcolumn];
                        startrow = 0;
                        endrow = 0;
                    }
                    else
                    {
                        range = sheet1.Range[startrow, startcolumn, endrow, endcolumn];
                        startrow = 0;
                        endrow = 0;
                    }
                    
                    valuesranges = valuesranges == string.Empty ? "(" + range.AddressGlobal : (valuesranges + "," + range.AddressGlobal);
                    count++;
                }
                valuesranges = valuesranges + ")";
                valuesranges = valuesranges.Replace("\'", "");
            }
            for (int scount = 0; scount < seriescollection.Count; scount++)
            {
                (seriescollection[scount] as ChartSerieImpl).FilteredCategory = valuesranges;
            }
        }
    }
    /// <summary>
    /// serialize the filtered category or series name
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="series"></param>
    /// <param name="seriesOrcategory"></param>
    private void SerializeFilteredSeriesOrCategoryName(XmlWriter writer,ChartSerieImpl series,bool seriesOrcategory)
    {       
        
        if (seriesOrcategory && !series.IsDefaultName)
        {
            writer.WriteStartElement(ChartConstants.CPrefix, Excel2007Serializator.Extension, ChartConstants.CNamespace);
            writer.WriteAttributeString(Drawings.UriAttribute, ChartConstants.Filterseriesuri);
            writer.WriteAttributeString(WorkbookXmlSerializator.DEF_XMLNS_PREF, ChartConstants.c15tag, null, ChartConstants.xml15web);
            writer.WriteStartElement(ChartConstants.c15tag, ChartConstants.FilteredSeriesTitle, ChartConstants.xml15web);
            SerializeFilteredText(writer, series);
            writer.WriteEndElement();
            writer.WriteEndElement();
        }
        else  if(series.CategoryLabels!=null)
        {
            writer.WriteStartElement(ChartConstants.CPrefix, Excel2007Serializator.Extension, ChartConstants.CNamespace);
            writer.WriteAttributeString(Drawings.UriAttribute, ChartConstants.Filterseriesuri);
            writer.WriteAttributeString(WorkbookXmlSerializator.DEF_XMLNS_PREF, ChartConstants.c15tag, null, ChartConstants.xml15web);
            writer.WriteStartElement(ChartConstants.c15tag, ChartConstants.FilteredCategoryTitle, ChartConstants.xml15web);
            SerializeFilteredCategoryName(writer, series);
            writer.WriteEndElement();
            writer.WriteEndElement();
        }                 
        
    }
    /// <summary>
    /// serilaize the category name
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="series"></param>
    private void SerializeFilteredCategoryName(XmlWriter writer, ChartSerieImpl series)
    {
        if (series.CategoryLabels != null)
        {
            writer.WriteStartElement(ChartConstants.c15tag, ChartConstants.CategoryValuesTag, ChartConstants.xml15web);
            writer.WriteStartElement(ChartConstants.CPrefix, ChartConstants.StringReferenceTag, ChartConstants.CNamespace);
            writer.WriteStartElement(ChartConstants.CPrefix, Excel2007Serializator.Extensionlist, ChartConstants.CNamespace);
            writer.WriteStartElement(ChartConstants.CPrefix, Excel2007Serializator.Extension, ChartConstants.CNamespace);
            writer.WriteAttributeString(Drawings.UriAttribute, ChartConstants.Filterseriesuri);
            writer.WriteStartElement(ChartConstants.c15tag, ChartConstants.formulareference, ChartConstants.xml15web);


            writer.WriteElementString(ChartConstants.SqureReference, ChartConstants.xml15web, series.CategoryLabels.AddressGlobal);
            //writer.WriteStartElement(ChartConstants.c15tag ,ChartConstants.SqureReference, ChartConstants.CNamespace);
            //writer.WriteElementString(ChartConstants.Formula, ChartConstants.xml15web, range);
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
            //// TODO: we don't store cache and we don't support anything but formula here, this should be changed later.
            writer.WriteEndElement();
            writer.WriteEndElement();
        }
    }
    /// <summary>
    /// Serializes single chart series for pie chart.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="series">Series to serialize.</param>
    private void SerializePieSeries( XmlWriter writer, ChartSerieImpl series )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( series == null )
        throw new ArgumentNullException( "series" );

      SerializeSeriesCommonWithoutEnd( writer, series ,false);
      int iPercent = series.SerieFormat.Percent;

      if( iPercent != 0 )
      {
        ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.PieExplosionTag,
          iPercent.ToString() );
      }

      if( ( series.DataPoints.DefaultDataPoint as ChartDataPointImpl ).HasDataLabels )
        SerializeDataLabels( writer, series.DataPoints.DefaultDataPoint.DataLabels, series );

      SerializeTrendlines( writer, series.TrendLines, series.ParentBook );
      SerializeErrorBars( writer, series );
      if (categoryFilter == 0)
      {
          findFilter = FindFiltered(series);
          categoryFilter++;
          if (findFilter)
          {
              UpdateCategoryLabel(series);
              UpdateFilteredValuesRange(series);
          }
      }
      if (!findFilter)
      {
          if ((series.ParentChart as ChartImpl).CategoryLabelLevel == ExcelCategoriesLabelLevel.CategoriesLabelLevelAll)
          {
              SerializeSeriesCategory(writer, series);
          }
          SerializeSeriesValues(writer, series);
      }
      else
      {
          if ((series.ParentChart as ChartImpl).CategoryLabelLevel == ExcelCategoriesLabelLevel.CategoriesLabelLevelAll)
          {
              SerializeFilteredCategory(writer, series, findFilter);
          }
          SerializeFilteredValues(writer, series, findFilter);
      }
      if ((series.ParentChart as ChartImpl).CategoryLabelLevel != ExcelCategoriesLabelLevel.CategoriesLabelLevelAll || (series.ParentChart as ChartImpl).SeriesNameLevel != ExcelSeriesNameLevel.SeriesNameLevelAll)
      {
          writer.WriteStartElement(Excel2007Serializator.Extensionlist, ChartConstants.CNamespace);
          if ((series.ParentChart as ChartImpl).SeriesNameLevel != ExcelSeriesNameLevel.SeriesNameLevelAll)
          {
              SerializeFilteredSeriesOrCategoryName(writer, series, true);
          }
          if ((series.ParentChart as ChartImpl).CategoryLabelLevel != ExcelCategoriesLabelLevel.CategoriesLabelLevelAll)
          {
              SerializeFilteredSeriesOrCategoryName(writer, series, false);
          }
          writer.WriteEndElement();
      }
      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes error bars.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="series">Series to serialize.</param>
    private void SerializeErrorBars( XmlWriter writer, ChartSerieImpl series )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( series == null )
        throw new ArgumentNullException( "series" );

      IWorkbook book = series.ParentBook;

      if( series.HasErrorBarsX )
        SerializeErrorBars( writer, series.ErrorBarsX, ChartConstants.ErrorBarX, book, series );

      if( series.HasErrorBarsY )
        SerializeErrorBars( writer, series.ErrorBarsY, ChartConstants.ErrorBarY, book, series );
    }
    /// <summary>
    /// Serialize line series.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="series">Chart line series to serialize.</param>
    private void SerializeLineSeries( XmlWriter writer, ChartSerieImpl series )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( series == null )
        throw new ArgumentNullException( "series" );

      SerializeSeriesCommonWithoutEnd( writer, series,false );

      //IChartBorder border = series.SerieFormat.LineProperties;
      //if( !border.AutoFormat )
      //{
      //  writer.WriteStartElement( Drawings.ShapePropertiesTag, ChartConstants.CNamespace );
      //  ChartSerializatorCommon.SerializeLineProperties( writer, series.SerieFormat.LineProperties );
      //  writer.WriteEndElement();
      //}

      //SerializeMarker( writer, series ); 

      if( ( series.DataPoints.DefaultDataPoint as ChartDataPointImpl ).HasDataLabels )
        SerializeDataLabels( writer, series.DataPoints.DefaultDataPoint.DataLabels, series );

      SerializeTrendlines( writer, series.TrendLines, series.ParentBook );
      SerializeErrorBars( writer, series );
      if (categoryFilter == 0)
      {
          findFilter = FindFiltered(series);
          categoryFilter++;
          if (findFilter)
          {
              UpdateCategoryLabel(series);
              UpdateFilteredValuesRange(series);
          }
      }
      if (!findFilter)
      {
          if ((series.ParentChart as ChartImpl).CategoryLabelLevel == ExcelCategoriesLabelLevel.CategoriesLabelLevelAll)
          {
              SerializeSeriesCategory(writer, series);
          }
          SerializeSeriesValues(writer, series);
      }
      else
      {
          if ((series.ParentChart as ChartImpl).CategoryLabelLevel == ExcelCategoriesLabelLevel.CategoriesLabelLevelAll)
          {
              SerializeFilteredCategory(writer, series, findFilter);
          }
          SerializeFilteredValues(writer, series, findFilter);
      }
      if ((series.ParentChart as ChartImpl).CategoryLabelLevel != ExcelCategoriesLabelLevel.CategoriesLabelLevelAll || (series.ParentChart as ChartImpl).SeriesNameLevel != ExcelSeriesNameLevel.SeriesNameLevelAll)
      {
          writer.WriteStartElement(Excel2007Serializator.Extensionlist, ChartConstants.CNamespace);
          if ((series.ParentChart as ChartImpl).SeriesNameLevel != ExcelSeriesNameLevel.SeriesNameLevelAll)
          {
              SerializeFilteredSeriesOrCategoryName(writer, series, true);
          }
          if ((series.ParentChart as ChartImpl).CategoryLabelLevel != ExcelCategoriesLabelLevel.CategoriesLabelLevelAll)
          {
              SerializeFilteredSeriesOrCategoryName(writer, series, false);
          }
          writer.WriteEndElement();
      }
      ChartSerieDataFormatImpl serieFormat = ( ChartSerieDataFormatImpl )series.SerieFormat;
      ChartSerializatorCommon.SerializeBoolValueTag( writer, ChartConstants.SmoothTag, serieFormat.IsSmoothed );

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes scatter series.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="series">Chart scatter series to serialize.</param>
    private void SerializeScatterSeries( XmlWriter writer, ChartSerieImpl series )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( series == null )
        throw new ArgumentNullException( "series" );

      SerializeSeriesCommonWithoutEnd( writer, series,false );
      //SerializeMarker( writer, series );

      if ((series.DataPoints.DefaultDataPoint.DataLabels != null) &&
          (series.DataPoints.DefaultDataPoint as ChartDataPointImpl).HasDataLabels)
      {
          SerializeDataLabels(writer, series.DataPoints.DefaultDataPoint.DataLabels, series);
      }

      SerializeTrendlines( writer, series.TrendLines, series.ParentBook );
      SerializeErrorBars( writer, series );

      SerializeSeriesCategory( writer, series, ChartConstants.XValues );
      SerializeSeriesValues( writer, series, ChartConstants.YValues );

      ChartSerieDataFormatImpl serieFormat = ( ChartSerieDataFormatImpl )series.SerieFormat;

      //if( serieFormat.IsSmoothed )
        ChartSerializatorCommon.SerializeBoolValueTag( writer, ChartConstants.SmoothTag, serieFormat.IsSmoothed );
        if ((series.ParentChart as ChartImpl).CategoryLabelLevel != ExcelCategoriesLabelLevel.CategoriesLabelLevelAll || (series.ParentChart as ChartImpl).SeriesNameLevel != ExcelSeriesNameLevel.SeriesNameLevelAll)
        {
            writer.WriteStartElement(Excel2007Serializator.Extensionlist, ChartConstants.CNamespace);
            if( (series.ParentChart as ChartImpl).SeriesNameLevel != ExcelSeriesNameLevel.SeriesNameLevelAll)
            {
                SerializeFilteredSeriesOrCategoryName(writer, series, true);
            }
            if ((series.ParentChart as ChartImpl).CategoryLabelLevel != ExcelCategoriesLabelLevel.CategoriesLabelLevelAll)
            {
                SerializeFilteredSeriesOrCategoryName(writer, series, false);
            }
            writer.WriteEndElement();
        }
      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes chart radar series.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="series">Chart series to serialize.</param>
    private void SerializeRadarSeries( XmlWriter writer, ChartSerieImpl series )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( series == null )
        throw new ArgumentNullException( "series" );

      SerializeSeriesCommonWithoutEnd( writer, series ,false);
      //SerializeMarker( writer, series );

      if( ( series.DataPoints.DefaultDataPoint as ChartDataPointImpl ).HasDataLabels )
        SerializeDataLabels( writer, series.DataPoints.DefaultDataPoint.DataLabels, series );

      if (categoryFilter == 0)
      {
          findFilter = FindFiltered(series);
          categoryFilter++;
          if (findFilter)
          {
              UpdateCategoryLabel(series);
              UpdateFilteredValuesRange(series);
          }
      }
      if (!findFilter)
      {
          if ((series.ParentChart as ChartImpl).CategoryLabelLevel == ExcelCategoriesLabelLevel.CategoriesLabelLevelAll)
          {
              SerializeSeriesCategory(writer, series);
          }
          SerializeSeriesValues(writer, series);
      }
      else
      {
          if ((series.ParentChart as ChartImpl).CategoryLabelLevel == ExcelCategoriesLabelLevel.CategoriesLabelLevelAll)
          {
              SerializeFilteredCategory(writer, series, findFilter);
          }
          SerializeFilteredValues(writer, series, findFilter);
      }
      if ((series.ParentChart as ChartImpl).CategoryLabelLevel != ExcelCategoriesLabelLevel.CategoriesLabelLevelAll || (series.ParentChart as ChartImpl).SeriesNameLevel != ExcelSeriesNameLevel.SeriesNameLevelAll)
      {
          writer.WriteStartElement(Excel2007Serializator.Extensionlist, ChartConstants.CNamespace);
          if ((series.ParentChart as ChartImpl).SeriesNameLevel != ExcelSeriesNameLevel.SeriesNameLevelAll)
          {
              SerializeFilteredSeriesOrCategoryName(writer, series, true);
          }
          if ((series.ParentChart as ChartImpl).CategoryLabelLevel != ExcelCategoriesLabelLevel.CategoriesLabelLevelAll)
          {
              SerializeFilteredSeriesOrCategoryName(writer, series, false);
          }
          writer.WriteEndElement();
      }
      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes single chart series for bubble chart.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="series">Series to serialize.</param>
    private void SerializeBubbleSeries( XmlWriter writer, ChartSerieImpl series )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( series == null )
        throw new ArgumentNullException( "series" );

      SerializeSeriesCommonWithoutEnd( writer, series,false );

      if( ( series.DataPoints.DefaultDataPoint as ChartDataPointImpl ).HasDataLabels )
        SerializeDataLabels( writer, series.DataPoints.DefaultDataPoint.DataLabels, series );

      SerializeTrendlines( writer, series.TrendLines, series.ParentBook );
      SerializeErrorBars( writer, series );

      SerializeSeriesCategory( writer, series, ChartConstants.XValues );
      SerializeSeriesValues( writer, series, ChartConstants.YValues );
      SerializeSeriesValues( writer, series.Bubbles, series.EnteredDirectlyBubbles, ChartConstants.BubbleSize, series );

      if( series.SerieType == ExcelChartType.Bubble_3D )
        ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.Bubble3DTag, "1" );
      if ((series.ParentChart as ChartImpl).CategoryLabelLevel != ExcelCategoriesLabelLevel.CategoriesLabelLevelAll || (series.ParentChart as ChartImpl).SeriesNameLevel != ExcelSeriesNameLevel.SeriesNameLevelAll)
      {
          writer.WriteStartElement(Excel2007Serializator.Extensionlist, ChartConstants.CNamespace);
          if( (series.ParentChart as ChartImpl).SeriesNameLevel != ExcelSeriesNameLevel.SeriesNameLevelAll)
          {
              SerializeFilteredSeriesOrCategoryName(writer, series, true);
          }
          if ((series.ParentChart as ChartImpl).CategoryLabelLevel != ExcelCategoriesLabelLevel.CategoriesLabelLevelAll)
          {
              SerializeFilteredSeriesOrCategoryName(writer, series, false);
          }
          writer.WriteEndElement();
      }
      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes single chart series for area chart.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="series">Series to serialize.</param>
    private void SerializeAreaSeries( XmlWriter writer, ChartSerieImpl series )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( series == null )
        throw new ArgumentNullException( "series" );

      SerializeSeriesCommonWithoutEnd( writer, series,false );

      if( ( series.DataPoints.DefaultDataPoint as ChartDataPointImpl ).HasDataLabels )
        SerializeDataLabels( writer, series.DataPoints.DefaultDataPoint.DataLabels, series );

      SerializeTrendlines( writer, series.TrendLines, series.ParentBook );
      SerializeErrorBars( writer, series );
      if (categoryFilter == 0)
      {
          findFilter = FindFiltered(series);
          categoryFilter++;
          if (findFilter)
          {
              UpdateCategoryLabel(series);
              UpdateFilteredValuesRange(series);
          }
      }
      if (!findFilter)
      {
          if ((series.ParentChart as ChartImpl).CategoryLabelLevel == ExcelCategoriesLabelLevel.CategoriesLabelLevelAll)
          {
              SerializeSeriesCategory(writer, series);
          }
          SerializeSeriesValues(writer, series);
      }
      else
      {
          if ((series.ParentChart as ChartImpl).CategoryLabelLevel == ExcelCategoriesLabelLevel.CategoriesLabelLevelAll)
          {
              SerializeFilteredCategory(writer, series, findFilter);
          }
          SerializeFilteredValues(writer, series, findFilter);
      }
      if ((series.ParentChart as ChartImpl).CategoryLabelLevel != ExcelCategoriesLabelLevel.CategoriesLabelLevelAll || (series.ParentChart as ChartImpl).SeriesNameLevel != ExcelSeriesNameLevel.SeriesNameLevelAll)
      {
          writer.WriteStartElement(Excel2007Serializator.Extensionlist, ChartConstants.CNamespace);
          if ((series.ParentChart as ChartImpl).SeriesNameLevel != ExcelSeriesNameLevel.SeriesNameLevelAll)
          {
              SerializeFilteredSeriesOrCategoryName(writer, series, true);
          }
          if ((series.ParentChart as ChartImpl).CategoryLabelLevel != ExcelCategoriesLabelLevel.CategoriesLabelLevelAll)
          {
              SerializeFilteredSeriesOrCategoryName(writer, series, false);
          }
          writer.WriteEndElement();
      }
      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes common part of the series.
    /// WARNING: this method doesn't call last WriteEndElement(), so this call
    /// must be made by parent item after series serialization complete.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="series">Series to serialize.</param>
    private void SerializeSeriesCommonWithoutEnd( XmlWriter writer, ChartSerieImpl series,bool isFiltered )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( series == null )
        throw new ArgumentNullException( "series" );
      if (!isFiltered)
      {
          writer.WriteStartElement(ChartConstants.SeriesTag, ChartConstants.CNamespace);
          ChartSerializatorCommon.SerializeValueTag(writer, ChartConstants.IndexTag, series.Number.ToString());
          ChartSerializatorCommon.SerializeValueTag(writer, ChartConstants.SeriesOrderTag, series.Index.ToString());

          series.CheckLimits();

          if (!series.IsDefaultName && ((series.ParentChart as ChartImpl).SeriesNameLevel==ExcelSeriesNameLevel.SeriesNameLevelAll))
          {
              string strName = series.NameOrFormula;

              writer.WriteStartElement(ChartConstants.SeriesTextTag, ChartConstants.CNamespace);

              if (strName.Length > 0 && strName[0] == '=')
              {
                  SerializeStringReference(writer, strName, series, true);
              }
              else
              {
                  writer.WriteStartElement(ChartConstants.TextValueTag, ChartConstants.CNamespace);
                  writer.WriteString(strName);
                  writer.WriteEndElement();
              }

              writer.WriteEndElement();
          }
      }
      ChartDataPointImpl defaultDataPoint = ( ChartDataPointImpl )series.DataPoints.DefaultDataPoint;
      ChartSerieDataFormatImpl dataFormat = defaultDataPoint.DataFormatOrNull;

      if( dataFormat != null )
      {
        ChartSerializatorCommon.SerializeFrameFormat( writer, dataFormat,
          series.ParentChart, false, true );
      }

      if (series.InvertIfNegative != null) 
      {
          string value = (series.InvertIfNegative == true) ? "1" : "0";
          ChartSerializatorCommon.SerializeValueTag(writer, ChartConstants.InvertIfNegative, value);
      }

      SerializeMarker( writer, series );

          ChartDataPointsCollection dataPoints = (ChartDataPointsCollection)series.DataPoints;

          // 1 - because default data point is always there.
          if (dataPoints.DeninedDPCount > 0)
          {
              foreach (ChartDataPointImpl dataPoint in dataPoints)
              {
                  if ((!dataPoint.IsDefault) || (dataPoint.HasDataPoint))
                  {
                      SerializeDataPoint(writer, dataPoint, series);
                  }
              }
          }
    }
    /// <summary>
    /// Serializes single data point.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="dataPoint">Data point to serialize.</param>
    private void SerializeDataPoint( XmlWriter writer, ChartDataPointImpl dataPoint, ChartSerieImpl series )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( dataPoint == null )
        throw new ArgumentNullException( "dataPoint" );

      ChartSerieDataFormatImpl dataFormat = dataPoint.DataFormatOrNull;

      if( dataFormat == null || !dataFormat.IsFormatted && !dataFormat.IsParsed)
        return;

      writer.WriteStartElement( ChartConstants.DataPointTag, ChartConstants.CNamespace );
      ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.IndexTag,
        dataPoint.Index.ToString() );
      ChartSerializatorCommon.SerializeBoolValueTag(writer, ChartConstants.Bubble3DTag, dataPoint.Bubble3D);
      if(dataPoint.HasExplosion)
          ChartSerializatorCommon.SerializeValueTag(writer, ChartConstants.PieExplosionTag, dataPoint.Explosion.ToString());
      if (dataFormat.IsSupportFill && series.ParentChart.IsParsed)
        ChartSerializatorCommon.SerializeBoolValueTag(writer, ChartConstants.InvertIfNegative, (dataPoint.DataFormat.Fill as ChartFillImpl).InvertIfNegative);
        if(dataFormat.HasInterior||dataFormat.HasShadowProperties)
      ChartSerializatorCommon.SerializeFrameFormat( writer, dataFormat,
        dataFormat.ParentChart, false );

      if(dataPoint.IsDefaultmarkertype)
        SerializeMarker(writer, dataFormat);

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes series category.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="series">Series to serialize category labels for.</param>
    private void SerializeSeriesCategory( XmlWriter writer, ChartSerieImpl series, string tagName )
    {
      SerializeSeriesValues( writer, series.CategoryLabels, series.EnteredDirectlyCategoryLabels,
        tagName, series );
    }
    /// <summary>
    /// Serializes series category.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="series">Series to serialize category labels for.</param>
    private void SerializeSeriesCategory( XmlWriter writer, ChartSerieImpl series )
    {
      SerializeSeriesValues( writer, series.CategoryLabels, series.EnteredDirectlyCategoryLabels,
        ChartConstants.CategoryValuesTag, series );
    }
    /// <summary>
    /// Serializes series values.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="series">Series to serialize values for.</param>
    private void SerializeSeriesValues( XmlWriter writer, ChartSerieImpl series )
    {
      SerializeSeriesValues( writer, series.Values, series.EnteredDirectlyValues,
        ChartConstants.SeriesValuesTag, series );
    }
    /// <summary>
    /// Serializes series values.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="series">Series to serialize values for.</param>
    private void SerializeSeriesValues( XmlWriter writer, ChartSerieImpl series, string tagName )
    {
      SerializeSeriesValues( writer, series.Values, series.EnteredDirectlyValues,
        tagName, series );
    }
    /// <summary>
    /// Serializes series values.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="range">Range to serialize values for.</param>
    /// <param name="tagName">Name of the xml tag to use.</param>
    private void SerializeSeriesValues( XmlWriter writer, IRange range, object[] values, string tagName, ChartSerieImpl series )
    {
      if( range == null && values == null &&
          series.NumRefFormula == null && series.StrRefFormula == null && series.MulLvlStrRefFormula == null)
        return;

      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( tagName == null || tagName.Length == 0 )
        throw new ArgumentOutOfRangeException( "tagName" );

      WorkbookImpl book = null;

      if (range != null && range.Worksheet !=null)
      {
          book = range.Worksheet.Workbook as WorkbookImpl;
      }
      writer.WriteStartElement( tagName, ChartConstants.CNamespace );

      if (book != null)
      {
         if (book.IsCreated || book.IsConverted)
          {
              SerializeNormalReference(writer, range, values, tagName, series);
          }        
         else if (book.IsLoaded || book.Loading)
         {
             bool firstExpression = false;
             bool secondExpression = false;

             if (range is ExternalRange)
             {
                 firstExpression = (range as ExternalRange).IsNumReference || (range as ExternalRange).IsStringReference;
                 secondExpression = (range as ExternalRange).IsMultiReference;
             }
             else if (range is RangeImpl)
             {
                 firstExpression = (range as RangeImpl).IsNumReference || (range as RangeImpl).IsStringReference;
                 secondExpression = (range as RangeImpl).IsMultiReference;
             }
             else if (range is NameImpl)
             {
                 firstExpression = (range as NameImpl).IsNumReference || (range as NameImpl).IsStringReference;
                 secondExpression = (range as NameImpl).IsMultiReference;
             }

             if (range != null && !firstExpression && !secondExpression)
             {
                 SerializeNormalReference(writer,range, values, tagName, series);
             }
             else if (range != null && firstExpression)
             {
                 SerializeReference(writer, range, values, series,tagName);
             }
             else if (range != null && secondExpression)
             {
                 SerializeMultiLevelStringReference(writer, range, values);
             }
             else if (values != null)
             {
                 SerializeDirectlyEntered(writer, values, false);
             }
         }          
       }
      else if (range != null && tagName != ChartConstants.CategoryValuesTag)
      {
          SerializeReference(writer, range, values, series,tagName);
      }
      else if (range != null && tagName == ChartConstants.CategoryValuesTag)
      {
          SerializeMultiLevelStringReference(writer, range, values);
      }

      else if (values != null)
      {
          SerializeDirectlyEntered(writer, values, false);
      }
      else if (series.StrRefFormula != null)
      {
          SerializeFormula(writer, ChartConstants.StringReferenceTag, series.StrRefFormula);
      }
      else if (series.NumRefFormula != null)
      {
          SerializeFormula(writer, ChartConstants.NumberReferenceTag, series.NumRefFormula);
      }
      else if (series.MulLvlStrRefFormula != null)
      {
          SerializeFormula(writer, ChartConstants.MultiLevelStringReferenceTag, series.MulLvlStrRefFormula);
      }

      writer.WriteEndElement();
    }

    /// <summary>
    /// Serializes the normal reference.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="range">The range.</param>
    /// <param name="values">The values.</param>
    /// <param name="tagName">Name of the tag.</param>
    private void SerializeNormalReference(XmlWriter writer,IRange range, object[] values, string tagName, ChartSerieImpl series)
    {
        if (range != null && tagName != ChartConstants.CategoryValuesTag)
        {
            SerializeReference(writer, range, values, series,tagName);
        }
        else if (range != null && tagName == ChartConstants.CategoryValuesTag)
        {
            //SerializeMultiLevelStringReference(writer, range, values);
            SerializeStringReference(writer, range, series);
        }
        else if (values != null)
        {
            SerializeDirectlyEntered(writer, values, false);
        }
    }
    /// <summary>
    /// Serializes number or string reference.
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="range"></param>
    private void SerializeReference( XmlWriter writer, IRange range ,object[] rangeValues, ChartSerieImpl series,string tagName)
    {
      if( range == null )
        throw new ArgumentNullException( "range" );

      //IRange topCell = range[ range.Row, range.Column ];(
      bool isString = range.HasString;
      if (range.Worksheet != null)
      {
          isString = GetStringReference(range);            
      }
      if(isString && tagName != ChartConstants.YValues && tagName != ChartConstants.ValueAttribute)      
      {
        SerializeStringReference( writer, range, series );
      }
      else
      {
        SerializeNumReference( writer, range ,rangeValues, series);
      }
    }

    /// <summary>
    /// Gets the string reference.
    /// </summary>
    /// <param name="range">The range.</param>
    /// <returns></returns>
    private bool GetStringReference(IRange range)
    {
        WorkbookImpl book = range.Worksheet.Workbook as WorkbookImpl;
        if (book.IsCreated || book.IsConverted)
        {
            return range.HasString;
        }
        else
        {
            if (range is ExternalRange)
                return (range as ExternalRange).IsStringReference;
            else if (range is RangeImpl)
                return (range as RangeImpl).IsStringReference;
            else if (range is NameImpl)
                return (range as NameImpl).IsStringReference;
        }
        return false;
    }

    
    /// <summary>
    /// Serializes number references.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="range">Range to serialize values for.</param>
    /// <param name="rangeValues">Range values to serialize cache values for.</param>
    private void SerializeNumReference( XmlWriter writer, IRange range, object[] rangeValues, ChartSerieImpl series)
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( range == null )
        throw new ArgumentNullException( "range" );

      ICombinedRange combinedRange = range as ICombinedRange;

      string address = ( combinedRange != null ) ?
        combinedRange.AddressGlobal2007 :
        range.AddressGlobal;

      if (combinedRange!=null && !(combinedRange is ExternalRange))
          rangeValues = null;

      if (series.NumRefFormula != null)
          address = series.NumRefFormula;

      writer.WriteStartElement( ChartConstants.NumberReferenceTag, ChartConstants.CNamespace );
      writer.WriteElementString( ChartConstants.Formula, ChartConstants.CNamespace, address );
      if (rangeValues == null)
          writer.WriteElementString(ChartConstants.NumberCacheTag, ChartConstants.CNamespace, string.Empty);
      else
          SerializeDirectlyEntered(writer, rangeValues, true);
      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes string references.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="range">Range to serialize values for.</param>
    private void SerializeStringReference( XmlWriter writer, IRange range, ChartSerieImpl series )
    {
      SerializeStringReference( writer, range.AddressGlobal, series, false );
    }
    /// <summary>
    /// Serializes String references.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="range">Range to serialize values for.</param>
    private void SerializeStringReference( XmlWriter writer, string range, ChartSerieImpl series, bool hasSeriesName )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( range == null )
        throw new ArgumentNullException( "range" );

      if( range[ 0 ] == '=' )
        range = UtilityMethods.RemoveFirstCharUnsafe( range );

      if ((!hasSeriesName) && (series.StrRefFormula != null))
          range = series.StrRefFormula;

      writer.WriteStartElement( ChartConstants.StringReferenceTag, ChartConstants.CNamespace );
      writer.WriteElementString( ChartConstants.Formula, ChartConstants.CNamespace, range );
      writer.WriteElementString( ChartConstants.StringCacheTag, ChartConstants.CNamespace, string.Empty );
      // TODO: we don't store cache and we don't support anything but formula here, this should be changed later.
      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes Multi level string references.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="range">Range to serialize values for.</param>
    /// <param name="rangeValues">Range values to serialize cache values for.</param>
    private void SerializeMultiLevelStringReference(XmlWriter writer, IRange range, object[] rangeValues)
    {
        if (writer == null)
            throw new ArgumentNullException("writer");

        if (range == null)
            throw new ArgumentNullException("range");

        ICombinedRange combinedRange = range as ICombinedRange;

        string address = (combinedRange != null) ?
          combinedRange.AddressGlobal2007 :
          range.AddressGlobal;
        if(rangeValues==null)
        writer.WriteStartElement(ChartConstants.MultiLevelStringReferenceTag, ChartConstants.CNamespace);
        else
            writer.WriteStartElement(ChartConstants.NumberReferenceTag, ChartConstants.CNamespace);
        writer.WriteElementString(ChartConstants.Formula, ChartConstants.CNamespace, address);
        if (rangeValues == null)
            writer.WriteElementString(ChartConstants.MultiLevelStringCacheTag, ChartConstants.CNamespace, string.Empty);
        else
            SerializeDirectlyEntered(writer, rangeValues, true);
        writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes the formula if the range is null
    /// </summary>
    /// <param name="writer">XMLWriter to serialize into</param>
    /// <param name="tag">Tag to serialize values for</param>
    /// <param name="formula">Formula to serialize for the tag</param>
    private void SerializeFormula(XmlWriter writer, string tag, string formula)
    {
        if (writer == null)
            throw new ArgumentNullException("writer");

        if (tag == null)
            throw new ArgumentNullException("tag");

        if (formula == null)
            throw new ArgumentNullException("formula");

        writer.WriteStartElement(tag, ChartConstants.CNamespace);
        writer.WriteElementString(ChartConstants.Formula, ChartConstants.CNamespace, formula);
        writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes chart axes.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="chart">Chart to serializes axes of.</param>
    private void SerializeAxes( XmlWriter writer, ChartImpl chart, RelationCollection relations )
    {
      // TODO: add support of secondary axes.

      ChartAxisSerializator serializator = new ChartAxisSerializator();

      if( chart.IsCategoryAxisAvail && Array.IndexOf(chart.SerializedAxisIds.ToArray(),(chart.PrimaryCategoryAxis as ChartAxisImpl).AxisId)>=0 )
        serializator.SerializeAxis( writer, chart.PrimaryCategoryAxis, relations );

      if (chart.IsValueAxisAvail && Array.IndexOf(chart.SerializedAxisIds.ToArray(), (chart.PrimaryValueAxis as ChartAxisImpl).AxisId) >= 0)
        serializator.SerializeAxis( writer, chart.PrimaryValueAxis, relations );

      if (chart.IsSecondaryCategoryAxisAvail && Array.IndexOf(chart.SerializedAxisIds.ToArray(), (chart.SecondaryCategoryAxis as ChartAxisImpl).AxisId) >= 0)
        serializator.SerializeAxis( writer, chart.SecondaryCategoryAxis, relations );

      if (chart.IsSecondaryValueAxisAvail && Array.IndexOf(chart.SerializedAxisIds.ToArray(), (chart.SecondaryValueAxis as ChartAxisImpl).AxisId) >= 0)
        serializator.SerializeAxis( writer, chart.SecondaryValueAxis, relations );

      if( chart.IsSeriesAxisAvail )
        serializator.SerializeAxis( writer, chart.PrimarySerieAxis, relations );

        }
        /// <summary>
        /// Serializes the pivot axes.
        /// </summary>
        /// <param name="writer">XmlWriter to serialize into.</param>
        /// <param name="chart">Chart to serialize.</param>
        /// <param name="relations">The relations.</param>
        private void SerializePivotAxes(XmlWriter writer, ChartImpl chart, RelationCollection relations)
        {
            // TODO: add support of secondary axes.

            ChartAxisSerializator serializator = new ChartAxisSerializator();

            if (chart.IsCategoryAxisAvail)
                serializator.SerializeAxis(writer, chart.PrimaryCategoryAxis, relations);

            if (chart.IsValueAxisAvail)
                serializator.SerializeAxis(writer, chart.PrimaryValueAxis, relations);

            if (chart.IsPivotChart3D)
                serializator.SerializeAxis(writer, chart.PrimarySerieAxis, relations);

    }
    /// <summary>
    /// Serializes series marker if necessary.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="series">Series to serialize marker for.</param>
    private void SerializeMarker( XmlWriter writer, ChartSerieImpl series )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( series == null )
        throw new ArgumentNullException( "series" );

      ChartSerieDataFormatImpl serieFormat = ( ChartSerieDataFormatImpl )series.SerieFormat;
      SerializeMarker( writer, serieFormat );
    }
    /// <summary>
    /// Serializes series marker if necessary.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="series">Series to serialize marker for.</param>
    private void SerializeMarker( XmlWriter writer, ChartSerieDataFormatImpl serieFormat )
    {
       if( serieFormat.IsMarkerSupported && serieFormat.IsMarker )
      {
        if( !serieFormat.IsAutoMarker )
        {
          writer.WriteStartElement( ChartConstants.MarkerTag, ChartConstants.CNamespace );
          Excel2007ChartMarkerType markerType = ( Excel2007ChartMarkerType )serieFormat.MarkerStyle;
          ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.MarkerStyleTag, markerType.ToString() );
          ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.MarkerSizeTag, serieFormat.MarkerSize.ToString() );

          // TODO: here we have to serialize marker fill, but we don't support it on the current moment.
          if( !serieFormat.MarkerFormat.IsAutoColor )
          {
            writer.WriteStartElement( Drawings.ShapePropertiesTag, ChartConstants.CNamespace );
            IWorkbook book = serieFormat.ParentChart.ParentWorkbook;

          
            if(serieFormat.EffectListStream==null && !serieFormat.MarkerFormat.IsNotShowInt )
            {
              if( serieFormat.MarkerGradient != null )
              {
                GradientSerializator serializator = new GradientSerializator();
                serializator.Serialize( writer, serieFormat.MarkerGradient, book );
              }
              else
              {
                double transparency = serieFormat.IsSupportFill ? serieFormat.Fill.Transparency : 0;
#if !SILVERLIGHT && !WINRT && !WP
                transparency = serieFormat.MarkerBackgroundColor == Color.Transparent ? 0 : 1 - transparency;
#else
                transparency = serieFormat.MarkerBackgroundColor == Color.FromArgb(0,255,255,255) ? 0 : 1 - transparency;
#endif
                ChartSerializatorCommon.SerializeSolidFill( writer,
                  serieFormat.MarkerBackgroundColor, false, book, transparency );
              }
            }
            else if (serieFormat.MarkerFormat.IsNotShowInt)
            {
              writer.WriteElementString( Drawings.NoFillTag, Drawings.ANamespace, string.Empty );
            }

            if( serieFormat.MarkerLineStream != null )
            {
              serieFormat.MarkerLineStream.Position = 0;
              ShapeParser.WriteNodeFromStream( writer, serieFormat.MarkerLineStream );
            }
            else if(serieFormat.MarkerFormat.HasLineProperties )
            {
              SerializeLineSettings( writer, serieFormat.MarkerForegroundColor, book, serieFormat.MarkerFormat.IsNotShowBrd, serieFormat.MarkerTransparency );
            }

            if (serieFormat.EffectListStream != null)
            {
                ShapeParser.WriteNodeFromStream(writer, serieFormat.EffectListStream);
            }
            writer.WriteEndElement();
          }

          writer.WriteEndElement();
        }
      }
      else
           if (serieFormat.IsMarkerSupported &&
              (serieFormat as ChartSerieDataFormatImpl).HasMarkerProperties )
      {
        writer.WriteStartElement( ChartConstants.MarkerTag, ChartConstants.CNamespace );
        ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.MarkerStyleTag, Excel2007ChartMarkerType.none.ToString() );
        writer.WriteEndElement();
      }
    }
    /// <summary>
    /// Serializes line with the specified color.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="color">Line color.</param>
    /// <param name="book">Parent workbook.</param>
    internal static void SerializeLineSettings( XmlWriter writer, Color color, IWorkbook book )
    {
      SerializeLineSettings( writer, color, book, false );
    }
    /// <summary>
    /// Serializes line with the specified color.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="color">Line color.</param>
    /// <param name="book">Parent workbook.</param>
    internal static void SerializeLineSettings( XmlWriter writer, Color color, IWorkbook book, bool bNoFill )
    {
      SerializeLineSettings( writer, color, book, bNoFill, 1.0 );
    }
    /// <summary>
    /// Serializes line with the specified color.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="color">Line color.</param>
    /// <param name="book">Parent workbook.</param>
    internal static void SerializeLineSettings( XmlWriter writer, Color color, IWorkbook book, bool bNoFill, double transparency )
    {
      writer.WriteStartElement( Drawings.LineTag, Drawings.ANamespace );

      if( !bNoFill )
      {
        ChartSerializatorCommon.SerializeSolidFill( writer,
          color, false, book, transparency );
      }
      else
      {
        writer.WriteElementString( Drawings.NoFillTag, Drawings.ANamespace, string.Empty );
      }

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes up/down bars.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="chart">Chart to serialize.</param>
    /// <param name="firstSeries">First series in the list of series with same
    /// formatting to serialize up/down bars for.</param>
    private void SerializeUpDownBars( XmlWriter writer, ChartImpl chart, ChartSerieImpl firstSeries )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      ChartFormatImpl chartFormat = ( ChartFormatImpl )firstSeries.SerieFormat.CommonSerieOptions;

      if( chartFormat.IsDropBar )
      {
        IChartDropBar upBar = chartFormat.FirstDropBar;
        IChartDropBar downBar = chartFormat.SecondDropBar;

        writer.WriteStartElement( ChartConstants.UpDownBarsTag, ChartConstants.CNamespace );

        ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.GapWidthTag, upBar.Gap.ToString() );

        SerializeDropBar( writer, upBar, ChartConstants.UpBarsTag, chart );
        SerializeDropBar( writer, downBar, ChartConstants.DownBarsTag, chart );
        writer.WriteEndElement();
      }
    }
    /// <summary>
    /// Serializes single drop bar (up or down bar).
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="dropBar">Drop bar to serialize.</param>
    /// <param name="tagName">Name of the main tag.</param>
    /// <param name="chart">Chart to serialize dropbar for.</param>
    private void SerializeDropBar( XmlWriter writer, IChartDropBar dropBar,
      string tagName, ChartImpl chart )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( dropBar == null )
        throw new ArgumentNullException( "dropBar" );

      if( tagName == null || tagName.Length == 0 )
        throw new ArgumentOutOfRangeException( "tagName" );

      writer.WriteStartElement( tagName, ChartConstants.CNamespace );

      ChartSerializatorCommon.SerializeFrameFormat( writer, dropBar, chart, false );

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes chartsheet into specified writer.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="chart">Chart to serialize.</param>
    /// <param name="drawingRelation">Id of the drawing relation with chart object..</param>
    internal void SerializeChartsheet( XmlWriter writer, ChartImpl chart, string drawingRelation )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      if( drawingRelation == null || drawingRelation.Length == 0 )
        throw new ArgumentOutOfRangeException( "drawingRelation" );

      writer.WriteStartDocument();
      writer.WriteStartElement( ChartConstants.ChartsheetTag, Excel2007Serializator.XmlNamespaceMain );
      writer.WriteAttributeString( WorkbookXmlSerializator.DEF_XMLNS_PREF,
        Excel2007Serializator.RelationPrefix, null, Excel2007Serializator.RelationNamespace );

      // TODO: add chart page setup serialization.
      writer.WriteElementString( Excel2007Serializator.SheetLevelPropertiesTagName, string.Empty );
  //<sheetViews>
  //  <sheetView tabSelected="1" zoomScale="118" workbookViewId="0" zoomToFit="1"/>
  //</sheetViews>
      writer.WriteStartElement( Excel2007Serializator.SheetViewsTag );
      writer.WriteStartElement( Excel2007Serializator.SheetViewTag );
      //writer.WriteAttributeString( "tabSelected", "1" );
      Excel2007Serializator.SerializeAttribute( writer, Excel2007Serializator.SheetZoomScale, chart.Zoom, 100 );
      writer.WriteAttributeString( Excel2007Serializator.WorkbookViewIdAttribute, "0" );
      Excel2007Serializator.SerializeAttribute( writer, ChartConstants.ZoomToFit, chart.ZoomToFit, false );
      writer.WriteEndElement();
      writer.WriteEndElement();

      WorkbookImpl book = chart.ParentWorkbook;
      Excel2007Serializator serializator = book.DataHolder.Serializator;
      serializator.SerializeSheetProtection( writer, chart );

      IPageSetupConstantsProvider constants = new WorksheetPageSetupConstants();
      Excel2007Serializator.SerializePageMargins( writer, chart.PageSetup, constants );
      Excel2007Serializator.SerializePageSetup( writer, chart.PageSetup, constants );
      Excel2007Serializator.SerializeHeaderFooter( writer, chart.PageSetupBase, constants );

      writer.WriteStartElement( Drawings.DrawingTagName );

      writer.WriteAttributeString( Excel2007Serializator.RelationshipIdAttributeName,
        Excel2007Serializator.RelationNamespace, drawingRelation );

      writer.WriteEndElement();

      serializator.SerializeVmlShapesWorksheetPart( writer, chart );
      Excel2007Serializator.SerializeVmlHFShapesWorksheetPart( writer, chart, new ChartPageSetupConstants(), null );

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes chartsheet drawing part.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="chart">Chart to serialize drawing for.</param>
    /// <param name="strRelationId">Relation id of the drawing part.</param>
    public void SerializeChartsheetDrawing( XmlWriter writer, ChartImpl chart,
      string strRelationId )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      writer.WriteStartDocument( true );
      writer.WriteStartElement( Drawings.XdrPreffix, Drawings.WorksheetDrawings, Drawings.XdrNamespace );

      //writer.WriteAttributeString( WorkbookXmlSerializator.DEF_XMLNS_PREF, Drawings.XdrPreffix,
      //  null, Drawings.XdrNamespace );

      writer.WriteAttributeString( WorkbookXmlSerializator.DEF_XMLNS_PREF, Drawings.APreffix,
        null, Drawings.ANamespace );

      SerializeAbsoluteAnchorChart(writer, chart, strRelationId);

      writer.WriteEndElement();
    }
    internal static void SerializeAbsoluteAnchorChart(XmlWriter writer, ChartImpl chart, string strRelationId)
    {
        writer.WriteStartElement(Drawings.AbsoluteAnchorTag, Drawings.XdrNamespace);

        writer.WriteStartElement(Drawings.PositionTag, Drawings.XdrNamespace);

        double xPos=(chart.Workbook as WorkbookImpl).IsConverted ? 0 :chart.XPos;
        double yPos = (chart.Workbook as WorkbookImpl).IsConverted ? 0 : chart.YPos;
        writer.WriteAttributeString(Drawings.XAttributeName, xPos.ToString());
        writer.WriteAttributeString(Drawings.YAttributeName, yPos.ToString());
        writer.WriteEndElement();

        writer.WriteStartElement(Drawings.Extents, Drawings.XdrNamespace);
        
        if (chart.Width <= 0)
            writer.WriteAttributeString(Drawings.CXAttributeName, ChartSerializator.DefaultExtentX.ToString());
        else
            writer.WriteAttributeString(Drawings.CXAttributeName, chart.EMUWidth.ToString());


        if (chart.Height <= 0)
            writer.WriteAttributeString(Drawings.CYAttributeName, ChartSerializator.DefaultExtentY.ToString());
        else
            writer.WriteAttributeString(Drawings.CYAttributeName, chart.EMUHeight.ToString());
        
        writer.WriteEndElement();

        writer.WriteStartElement(Drawings.GraphicFrame, Drawings.XdrNamespace);
        writer.WriteAttributeString(Drawings.MacroAttribute, string.Empty);

        writer.WriteStartElement(Drawings.NonVisualGraphicFramePr, Drawings.XdrNamespace);

        writer.WriteStartElement(Drawings.NVCanvasPropertiesTag, Drawings.XdrNamespace);
        writer.WriteAttributeString(Drawings.IdAttributeName, "2");
        writer.WriteAttributeString(Drawings.NameAttributeName, chart.Name);
        writer.WriteEndElement();

        writer.WriteStartElement(Drawings.CNVGraphicFramePr, Drawings.XdrNamespace);
        writer.WriteStartElement(Drawings.GraphicFrameLocksTag, Drawings.ANamespace);
        writer.WriteAttributeString(Drawings.NoShapeGrouping, "1");
        writer.WriteEndElement();
        writer.WriteEndElement();

        writer.WriteEndElement();

        DrawingShapeSerializator.SerializeForm(writer, Drawings.XdrNamespace,
          Drawings.ANamespace, 0, 0, 0, 0);

        writer.WriteStartElement(Drawings.GraphicTag, Drawings.ANamespace);
        writer.WriteStartElement(Drawings.GraphicDataTag, Drawings.ANamespace);
        writer.WriteAttributeString(Drawings.UriAttribute, ChartConstants.CNamespace);

        writer.WriteStartElement(ChartConstants.CPrefix, ChartConstants.ChartTag, ChartConstants.CNamespace);

        writer.WriteAttributeString(Drawings.IdAttributeName, Excel2007Serializator.RelationNamespace,
          strRelationId);

        writer.WriteEndElement();
        writer.WriteEndElement();
        writer.WriteEndElement();
        writer.WriteEndElement();

        writer.WriteElementString(Drawings.ClientDataTagName, Drawings.XdrNamespace, string.Empty);

        writer.WriteEndElement();
    }
    /// <summary>
    /// This method serializes chart data table.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="chart">Chart to serialize data table for.</param>
    private void SerializeDataTable( XmlWriter writer, ChartImpl chart )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      if( chart.HasDataTable )
      {
        IChartDataTable dataTable = chart.DataTable;
        writer.WriteStartElement( ChartConstants.DataTableTag, ChartConstants.CNamespace );

        ChartSerializatorCommon.SerializeBoolValueTag( writer,
          ChartConstants.ShowHorizontalBorder, dataTable.HasHorzBorder );

        ChartSerializatorCommon.SerializeBoolValueTag( writer,
          ChartConstants.ShowVerticalBorder, dataTable.HasVertBorder );

        ChartSerializatorCommon.SerializeBoolValueTag( writer,
          ChartConstants.ShowOutline, dataTable.HasBorders );

        ChartSerializatorCommon.SerializeBoolValueTag( writer,
          ChartConstants.ShowSeriesKeys, dataTable.ShowSeriesKeys );
        
        bool bHasDefault = ((IInternalChartTextArea)dataTable.TextArea).ParagraphType != ChartParagraphType.CustomDefault;
        if (!bHasDefault)
        {
            WorkbookImpl book = (((dataTable as ChartDataTableImpl).Parent) as ChartImpl).ParentWorkbook;
            SerializeDefaultTextFormatting(writer, dataTable.TextArea, book as IWorkbook, ChartAxisParser.DefaultFontSize );
        }

        writer.WriteEndElement();
      }
    }
    /// <summary>
    /// Serializes series value that were entered directly.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="values">Values to serialize.</param>
    private void SerializeDirectlyEntered( XmlWriter writer, object[] values,bool isCache )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( values == null )
        throw new ArgumentNullException( "values" );

      string mainTag = null;

      if (isCache)
          mainTag = (values[0] is string) ? ChartConstants.StringCacheTag :
       ChartConstants.NumberCacheTag;
      else
          mainTag = (values[0] is string) ? ChartConstants.StringLiteral :
         ChartConstants.NumberLiteral;

      writer.WriteStartElement( mainTag, ChartConstants.CNamespace );

      int iCount = values.Length;
      ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.PointCount, iCount.ToString() );

      for( int i = 0; i < iCount; i++ )
      {
        writer.WriteStartElement( ChartConstants.NumericPoint, ChartConstants.CNamespace );
        writer.WriteAttributeString( ChartConstants.IndexTag, i.ToString() );
        writer.WriteStartElement( ChartConstants.NumbericValue, ChartConstants.CNamespace );
        writer.WriteString( ToXmlString( values[ i ] ) );
        writer.WriteEndElement();
        writer.WriteEndElement();
      }

      writer.WriteEndElement();
    }
    /// <summary>
    /// Converts object into xml string.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <returns>Converted value.</returns>
    private string ToXmlString( object value )
    {
      string result = null;

      if( value is double )
      {
        result = XmlConvert.ToString( ( double )value );
      }
      else if( value is float )
      {
        result = XmlConvert.ToString( ( float )value );
      }
      else
      {
        result = value.ToString();
      }

      return result;
    }
    #endregion
  }
}
