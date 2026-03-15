#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

using System;
using System.Collections;

using Syncfusion.XlsIO.Implementation.Charts;
using Syncfusion.XlsIO.Implementation.Exceptions;
using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser.Biff_Records.Charts;
using System.Collections.Generic;
using Syncfusion.XlsIO.Implementation.Shapes;
using System.IO;
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

namespace Syncfusion.XlsIO.Implementation.Charts
{
  /// <summary>
  /// ChartSerieDataFormatImpl class.
  /// </summary>
  public class ChartSerieDataFormatImpl
    : CommonObject
    , IChartSerieDataFormat
    , IFillColor
  {
    #region Class constants
    /// <summary>
    /// Represents None color index.
    /// </summary>
    private const ushort DEF_NONE_COLOR = 78;
    /// <summary>
    /// Represents marker size mull prefix.
    /// </summary>
    private const int DEF_MARKER_SIZE_MUL = 20;
    /// <summary>
    /// Represents start color.
    /// </summary>
    public const int DEF_MARKER_START_COLOR = ( int )ExcelKnownColors.Custom16;//32;
    /// <summary>
    /// Represents start pie type.
    /// </summary>
    private const string DEF_PIE_START_TYPE = "Pie";
    /// <summary>
    /// Represents start doughnut type.
    /// </summary>
    private const string DEF_DOUGHNUT_START_TYPE = "Doughnut";
    /// <summary>
    /// Represents start surface type.
    /// </summary>
    private const string DEF_SURFACE_START_TYPE = "Surface";
    /// <summary>
    /// Represents start line type.
    /// </summary>
    public const string DEF_LINE_START_TYPE = "Line";
    /// <summary>
    /// Represents start scatter type.
    /// </summary>
    public const string DEF_SCATTER_START_TYPE = "Scatter";
    /// <summary>
    /// Represents default line size in marker record.
    /// </summary>
    private const int DEF_MARKER_LINE_SIZE = 60;
    /// <summary>
    /// Represents default line size in marker record.
    /// </summary>
    private const int DEF_LINE_SIZE = 5;
    /// <summary>
    /// Represents default line color.
    /// </summary>
    private const int DEF_LINE_COLOR = 0x800000;
    /// <summary>
    /// Represents default index for marker record.
    /// </summary>
    private const int DEF_MARKER_INDEX = 32;
    /// <summary>
    /// Represents default color in marker record.
    /// </summary>
    private const ExcelKnownColors DEF_MARKER_COLOR_INDEX = ( ExcelKnownColors )77;
    /// <summary>
    /// Represents types of chart that support data format properties.
    /// </summary>
    public static readonly ExcelChartType[] DEF_SUPPORT_DATAFORMAT_PROPERTIES =
    {
      ExcelChartType.Bar_Clustered_3D,
      ExcelChartType.Bar_Stacked_100_3D,
      ExcelChartType.Bar_Stacked_3D,
      ExcelChartType.Column_3D,
      ExcelChartType.Column_Clustered_3D,
      ExcelChartType.Column_Stacked_100_3D,
      ExcelChartType.Column_Stacked_3D,
      ExcelChartType.Cone_Bar_Clustered,
      ExcelChartType.Cone_Bar_Stacked,
      ExcelChartType.Cone_Bar_Stacked_100,
      ExcelChartType.Cone_Clustered,
      ExcelChartType.Cone_Clustered_3D,
      ExcelChartType.Cone_Stacked,
      ExcelChartType.Cone_Stacked_100,
      ExcelChartType.Cylinder_Bar_Clustered,
      ExcelChartType.Cylinder_Bar_Stacked,
      ExcelChartType.Cylinder_Bar_Stacked_100,
      ExcelChartType.Cylinder_Clustered,
      ExcelChartType.Cylinder_Clustered_3D,
      ExcelChartType.Cylinder_Stacked,
      ExcelChartType.Cylinder_Stacked_100,
      ExcelChartType.Pyramid_Bar_Clustered,
      ExcelChartType.Pyramid_Bar_Stacked,
      ExcelChartType.Pyramid_Bar_Stacked_100,
      ExcelChartType.Pyramid_Clustered,
      ExcelChartType.Pyramid_Clustered_3D,
      ExcelChartType.Pyramid_Stacked,
      ExcelChartType.Pyramid_Bar_Stacked
    };
    #endregion

    #region Class members
    /// <summary>
    /// Main series data format record.
    /// </summary>
    private ChartDataFormatRecord m_dataFormat = ( ChartDataFormatRecord )
      BiffRecordFactory.GetRecord( TBIFFRecord.ChartDataFormat );
    /// <summary>
    /// 3-D data format.
    /// </summary>
    private Chart3DDataFormatRecord m_3DDataFormat;
    /// <summary>
    /// Pie format.
    /// </summary>
    private ChartPieFormatRecord m_pieFormat;
    /// <summary>
        /// Represents the 3D features
        /// </summary>
        private ThreeDFormatImpl m_3D;
        /// <summary>
        /// Represents the Chart Shadow
        /// </summary>
        private ShadowImpl m_shadow;
        /// <summary>
    /// Marker format.
    /// </summary>
    private ChartMarkerFormatRecord m_markerFormat;
    /// <summary>
    /// Attached label.
    /// </summary>
    private ChartAttachedLabelRecord m_attachedLabel;
    /// <summary>
    /// Represents the beginning of a collection of records
    /// </summary>
    private UnknownRecord m_startBlock;
    /// <summary>
    /// Represents the shape formatting properties for chart elements
    /// </summary>
    private UnknownRecord m_shapePropsStream;
    /// <summary>
    /// Represents the end of a collection of records
    /// </summary>
    private UnknownRecord m_endBlock;
    /// <summary>
    /// Attached label layout
    /// </summary>
    private ChartAttachedLabelLayoutRecord m_attachedLabelLayout;
    /// <summary>
    /// Series format.
    /// </summary>
    private ChartSerFmtRecord m_seriesFormat;
    /// <summary>
    /// Represents default data point.
    /// </summary>
    private ChartDataPointImpl m_dataPoint;
    /// <summary>
    /// Parent series.
    /// </summary>
    private ChartSerieImpl m_serie;
    /// <summary>
    /// Parent chart format.
    /// </summary>
    private ChartFormatImpl m_format;
    /// <summary>
    /// Parent chart.
    /// </summary>
    private ChartImpl m_chart;
    /// <summary>
    /// Represents border.
    /// </summary>
    private ChartBorderImpl m_border;
    /// <summary>
    /// Represents chart area properties.
    /// </summary>
    private ChartInteriorImpl m_interior;
    /// <summary>
    /// Represents if DataFormat is Formatted.
    /// </summary>
    private bool m_bFormatted;
    /// <summary>
    /// Represents fill properties.
    /// </summary>
    private ChartFillImpl m_fill;
    /// <summary>
    /// Object that holds marker background color.
    /// </summary>
    private ColorObject m_markerBackColor;
    /// <summary>
    /// Object that holds marker foreground color.
    /// </summary>
    private ColorObject m_markerForeColor;
    /// <summary>
    /// Preserved marker gradient data.
    /// </summary>
    private GradientStops m_markerGradient;
    private double m_markerTransparency = 1.0;//ShapeFillImpl.MaxValue;
    private Stream m_markerLineStream;
    private Stream m_markerEffectList;
    /// <summary>
    /// Represents whether the marker properties exists or not
    /// </summary>
    private bool m_HasMarkerProperties;
    /// <summary>
    /// Represents whether the data point is parsed or not
    /// </summary>
    private bool m_bIsParsed;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Creates series and set its Application and Parent
    /// properties to specified values.
    /// </summary>
    /// <param name="application">Application object for the chart.</param>
    /// <param name="parent">Parent object for the chart.</param>
    public ChartSerieDataFormatImpl( IApplication application, object parent )
      : base( application, parent )
    {
      SetParents();

      m_fill = new ChartFillImpl( application, this );

      if( !m_chart.ParentWorkbook.Loading )
        SetDefault3DDataFormat();

      InitializeColors();
    }
    /// <summary>
    /// Initializes marker color variables.
    /// </summary>
    private void InitializeColors()
    {
      m_markerForeColor = new ColorObject( ColorExtension.Empty );
      m_markerForeColor.AfterChange += MarkerForeColorChanged;

      m_markerBackColor = new ColorObject( ColorExtension.Empty );
      m_markerBackColor.AfterChange += MarkerBackColorChanged;
    }
    /// <summary>
    /// Searches for all necessary parents.
    /// </summary>
    internal void SetParents()
    {
      m_chart = FindParent( typeof( ChartImpl ) ) as ChartImpl;

      Type[] types = { typeof( ChartSerieImpl ), typeof( ChartFormatImpl ) };
      object parentObject = FindParent( types );

      if( parentObject == null || m_chart == null )
        throw new ArgumentNullException( "Can't find parent objects." );

      m_serie = parentObject as ChartSerieImpl;
      m_format = parentObject as ChartFormatImpl;
      m_dataPoint = FindParent( typeof( ChartDataPointImpl ) ) as ChartDataPointImpl;

      if( m_format != null && !m_chart.TypeChanging )
      {
        UpdateSerieFormat();
      }
    }
    #endregion

    #region Class parse / serialization methods
    /// <summary>
    /// Parses data format.
    /// </summary>
    /// <param name="arrData">Array with data format records.</param>
    /// <param name="iPos">Position of the first data format record.</param>
    /// <returns>Position after the last data format record.</returns>
    [ CLSCompliant( false ) ]
    public int Parse( IList<BiffRecordRaw> arrData, int iPos )
    {
      if( arrData == null )
        throw new ArgumentNullException( "arrData" );

      if( iPos < 0 || iPos > arrData.Count )
        throw new ArgumentOutOfRangeException( "iPos", "Value cannot be less than 0 and greater than arrData.Length" );

      BiffRecordRaw record = ( BiffRecordRaw )arrData[ iPos ];
      record.CheckTypeCode( TBIFFRecord.ChartDataFormat );
      m_dataFormat = ( ChartDataFormatRecord )record;
      iPos++;

      record = arrData[ iPos++ ];
      record.CheckTypeCode( TBIFFRecord.Begin );

      record = arrData[ iPos ];

      int iCount = 1;

      while( iCount > 0 )
      {
        switch( record.TypeCode )
        {
          case TBIFFRecord.Begin:
            iCount++;
            break;

          case TBIFFRecord.End:
            iCount--;
            break;

          case TBIFFRecord.Chart3DDataFormat:
            m_3DDataFormat = ( Chart3DDataFormatRecord )record;
            m_bFormatted = ChackDataRecord( m_3DDataFormat );
            break;

          case TBIFFRecord.ChartLineFormat:
            m_border = new ChartBorderImpl( Application, this, ( ChartLineFormatRecord )record );
            m_bFormatted = true;
            break;

          case TBIFFRecord.ChartAreaFormat:
            m_interior = new ChartInteriorImpl( Application, this, ( ChartAreaFormatRecord )record );
            m_bFormatted = true;
            break;

          case TBIFFRecord.ChartPieFormat:
            m_pieFormat = ( ChartPieFormatRecord )record;
            m_bFormatted = true;
            break;

          case TBIFFRecord.ChartMarkerFormat:
            m_markerFormat = ( ChartMarkerFormatRecord )record;
            m_bFormatted = true;
            break;

          case TBIFFRecord.ChartGelFrame:
            m_fill = new ChartFillImpl( Application, this, ( ChartGelFrameRecord )record );
            break;

          case TBIFFRecord.ChartAttachedLabel:
            m_attachedLabel = ( ChartAttachedLabelRecord )record;
            break;

          case TBIFFRecord.ChartAttachedLabelLayout:
            m_attachedLabelLayout = (ChartAttachedLabelLayoutRecord)record;
            break;

          case TBIFFRecord.ChartSerFmt:
            m_seriesFormat = ( ChartSerFmtRecord )record;
            m_bFormatted = true;
            break;

          case TBIFFRecord.StartBlock:
            m_startBlock = (UnknownRecord)record;
            break;

          case TBIFFRecord.ShapePropsStream:
            m_shapePropsStream = (UnknownRecord)record;
            break;

          case TBIFFRecord.EndBlock:
            m_endBlock = (UnknownRecord)record;
            break;

        }

        iPos++;
        record = ( BiffRecordRaw )arrData[ iPos ];
      }

      return iPos;
    }
    /// <summary>
    /// Serializes data format.
    /// </summary>
    /// <param name="records">Represents record list to serialize into.</param>
    [ CLSCompliant( false ) ]
    public void Serialize( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      IChartDataLabels dataLabels =  ( m_dataPoint != null && m_dataPoint.HasDataLabels ) ?
        m_dataPoint.DataLabels :
        null;

      bool bNeedFormat = ( dataLabels != null ) ?
        dataLabels.IsSeriesName || dataLabels.IsCategoryName :
        false;

      records.Add( m_dataFormat );
      records.Add( BiffRecordFactory.GetRecord( TBIFFRecord.Begin ) );

      if( m_3DDataFormat != null )
        records.Add( ( BiffRecordRaw )m_3DDataFormat.Clone() );

      if( m_border != null )
      {
        m_border.Serialize( records );
      }
      else if( bNeedFormat && IsBorderSupported )
      {
        BiffRecordRaw record = BiffRecordFactory.GetRecord( TBIFFRecord.ChartLineFormat );
        records.Add( record );
      }

      if( m_interior != null )
      {
        m_interior.Serialize( records );
      }
      else if( bNeedFormat && IsInteriorSupported )
      {
        BiffRecordRaw record = BiffRecordFactory.GetRecord( TBIFFRecord.ChartAreaFormat );
        records.Add( record );
      }

      if( m_pieFormat != null )
        records.Add( ( BiffRecordRaw )m_pieFormat.Clone() );

      if( m_seriesFormat != null )
        records.Add( ( BiffRecordRaw )m_seriesFormat.Clone() );

      if( ( m_serie == null || m_serie.StartType != ChartImpl.START_SCATTER )
        && IsInteriorSupported )
      {
        m_fill.Serialize( records );
      }

      if (m_markerFormat != null && IsMarkerSupported)
        records.Add( ( BiffRecordRaw )m_markerFormat.Clone() );

      if( m_attachedLabel != null )
        records.Add( ( BiffRecordRaw )m_attachedLabel.Clone() );

      if (m_startBlock != null)
          records.Add((UnknownRecord)m_startBlock.Clone());

      if (m_shapePropsStream != null)
          records.Add((UnknownRecord)m_shapePropsStream.Clone());

      if (m_endBlock != null)
          records.Add((UnknownRecord)m_endBlock.Clone());

      records.Add( BiffRecordFactory.GetRecord( TBIFFRecord.End ) );
    }
    /// <summary>
    /// Sets default values.
    /// </summary>
    public void SetDefaultValues()
    {
      SetFieldsToNull();
      m_dataFormat.SeriesIndex = ( ushort )m_serie.Index;
      m_dataFormat.SeriesNumber = ( ushort )m_serie.Number;
      
      m_3DDataFormat = m_serie.Get3DDataFormat();

      ChartImpl parentChart = m_serie.InnerChart;

      if( parentChart.IsChartStock && parentChart.Series[ parentChart.Series.Count - 1 ] == m_serie )
      {
        LineProperties.LinePattern = parentChart.DefaultLinePattern;
        m_border.AutoFormat = false;

        m_pieFormat = ( ChartPieFormatRecord )
          BiffRecordFactory.GetRecord( TBIFFRecord.ChartPieFormat );

        MarkerFormat.MarkerType = ExcelChartMarkerType.DowJones;
        m_markerFormat.LineSize = DEF_MARKER_LINE_SIZE;
      }
      else
      {
        if( m_serie.ChartGroup > 0 )
        {
          LineProperties.LineColor = ColorExtension.FromArgb( DEF_LINE_COLOR );

          m_pieFormat = ( ChartPieFormatRecord )
            BiffRecordFactory.GetRecord( TBIFFRecord.ChartPieFormat );

          MarkerFormat.MarkerType = ExcelChartMarkerType.Diamond;
          m_markerFormat.BorderColorIndex = DEF_MARKER_INDEX;
          m_markerFormat.FillColorIndex = DEF_MARKER_INDEX;
          m_markerFormat.LineSize = DEF_LINE_SIZE * 20;
          m_markerFormat.IsAutoColor = true;
          m_border.AutoFormat = true;
        }
      }
    }
    /// <summary>
    /// Sets 3D data format to the default state.
    /// </summary>
    private void SetDefault3DDataFormat()
    {
      m_3DDataFormat = ( Chart3DDataFormatRecord )
        BiffRecordFactory.GetRecord( TBIFFRecord.Chart3DDataFormat );

      switch( m_chart.ChartType )
      {
        case ExcelChartType.Cylinder_Clustered:
        case ExcelChartType.Cylinder_Stacked:
        case ExcelChartType.Cylinder_Stacked_100:
        case ExcelChartType.Cylinder_Bar_Clustered:
        case ExcelChartType.Cylinder_Bar_Stacked:
        case ExcelChartType.Cylinder_Bar_Stacked_100:
        case ExcelChartType.Cylinder_Clustered_3D:
          m_3DDataFormat.DataFormatBase = ExcelBaseFormat.Circle;
          m_3DDataFormat.DataFormatTop = ExcelTopFormat.Straight;
          break;

        case ExcelChartType.Cone_Stacked_100:
        case ExcelChartType.Cone_Bar_Stacked_100:
          m_3DDataFormat.DataFormatBase = ExcelBaseFormat.Circle;
          m_3DDataFormat.DataFormatTop = ExcelTopFormat.Trunc;
          break;

        case ExcelChartType.Cone_Clustered:
        case ExcelChartType.Cone_Stacked:
        case ExcelChartType.Cone_Bar_Clustered:
        case ExcelChartType.Cone_Bar_Stacked:
        case ExcelChartType.Cone_Clustered_3D:
          m_3DDataFormat.DataFormatBase = ExcelBaseFormat.Circle;
          m_3DDataFormat.DataFormatTop = ExcelTopFormat.Sharp;
          break;

        case ExcelChartType.Pyramid_Stacked_100:
        case ExcelChartType.Pyramid_Bar_Stacked_100:
          m_3DDataFormat.DataFormatBase = ExcelBaseFormat.Rectangle;
          m_3DDataFormat.DataFormatTop = ExcelTopFormat.Trunc;
          break;

        case ExcelChartType.Pyramid_Clustered:
        case ExcelChartType.Pyramid_Stacked:
        case ExcelChartType.Pyramid_Bar_Clustered:
        case ExcelChartType.Pyramid_Bar_Stacked:
        case ExcelChartType.Pyramid_Clustered_3D:
          m_3DDataFormat.DataFormatBase = ExcelBaseFormat.Rectangle;
          m_3DDataFormat.DataFormatTop = ExcelTopFormat.Sharp;
          break;
      }
    }
    /// <summary>
    /// Sets format fields to Null.
    /// </summary>
    private void SetFieldsToNull()
    {
      m_dataFormat = null;
      m_3DDataFormat = null;
      m_border = null;
      m_interior = null;
      m_pieFormat = null;
      m_markerFormat = null;
    }
    #endregion

    #region Class Helper Methods
    /// <summary>
    /// Clone current instance.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    /// <returns>Return cloned object.</returns>
    public ChartSerieDataFormatImpl Clone( object parent )
    {
      ChartSerieDataFormatImpl result = ( ChartSerieDataFormatImpl )MemberwiseClone();

      result.SetParent( parent );
      result.SetParents();

      result.m_dataFormat = ( ChartDataFormatRecord )CloneUtils.CloneCloneable( m_dataFormat );
      result.m_3DDataFormat = ( Chart3DDataFormatRecord )CloneUtils.CloneCloneable( m_3DDataFormat );

      if( m_border != null )
        result.m_border = m_border.Clone( result );

      if( m_interior != null )
        result.m_interior = m_interior.Clone( result );

      result.m_pieFormat = ( ChartPieFormatRecord )CloneUtils.CloneCloneable( m_pieFormat );
      result.m_markerFormat = ( ChartMarkerFormatRecord )CloneUtils.CloneCloneable( m_markerFormat );
      result.m_attachedLabel = ( ChartAttachedLabelRecord )CloneUtils.CloneCloneable( m_attachedLabel );
      result.m_attachedLabelLayout = (ChartAttachedLabelLayoutRecord)CloneUtils.CloneCloneable(m_attachedLabelLayout);
      result.m_seriesFormat = ( ChartSerFmtRecord )CloneUtils.CloneCloneable( m_seriesFormat );
      result.m_fill = ( ChartFillImpl )m_fill.Clone( result );

      if (!m_chart.TypeChanging && !m_chart.IsParsed && IsInteriorSupported && result.IsSupportFill)
      {
          if (result.m_fill.ForeColorObject != m_fill.ForeColorObject)
              result.m_fill.ForeColorObject.CopyFrom(m_fill.ForeColorObject, false);
          if (result.m_fill.BackColorObject != m_fill.BackColorObject)
              result.m_fill.BackColorObject.CopyFrom(m_fill.BackColorObject, false);
      }
      result.InitializeColors();
      result.m_markerBackColor.CopyFrom( m_markerBackColor, false );
      result.m_markerForeColor.CopyFrom( m_markerForeColor, false );

      return result;
    }
    /// <summary>
    /// Updates Series index.
    /// </summary>
    public void UpdateSerieIndex()
    {
      m_dataFormat.SeriesIndex = ( ushort )m_serie.Index;
      m_dataFormat.SeriesNumber = ( ushort )m_serie.Number;
    }
    /// <summary>
    /// Updates data format in data points.
    /// </summary>
    public void UpdateDataFormatInDataPoint()
    {
      if( ParentSerie == null )
        throw new ArgumentException( "Parent serie" );

      m_dataFormat.SeriesIndex = ( ushort )ParentSerie.Index;
      m_dataFormat.SeriesNumber = ( ushort )ParentSerie.Number;
    }
    /// <summary>
    /// Changes data format to create radar chart.
    /// </summary>
    /// <param name="type">Type to change.</param>
    public void ChangeRadarDataFormat( ExcelChartType type )
    {
      if( type == ExcelChartType.Radar )
      {
        MarkerForegroundColorIndex = DEF_MARKER_COLOR_INDEX;
        MarkerBackgroundColorIndex = DEF_MARKER_COLOR_INDEX;
        LineProperties.AutoFormat = true;
        m_border.IsAutoLineColor = true;
        IsAutoMarker = false;
        MarkerStyle = ExcelChartMarkerType.None;
      }

      if( type == ExcelChartType.Radar_Markers )
      {
        LineProperties.AutoFormat = false;
      }
    }
    /// <summary>
    /// Changes data format to create scatter chart.
    /// </summary>
    /// <param name="type">Type to change.</param>
    public void ChangeScatterDataFormat( ExcelChartType type )
    {
      if( type == ExcelChartType.Scatter_Line_Markers )
      {
        LineProperties.LinePattern = ExcelChartLinePattern.None;
        m_border.AutoFormat = true;

        return;
      }
      
      bool isSerieCreation = ((ChartSeriesCollection)m_chart.Series).IsSerieCreating;
      if (isSerieCreation)
        m_markerForeColor.SetIndexed(DEF_MARKER_COLOR_INDEX, !isSerieCreation);
      
      MarkerSize = DEF_LINE_SIZE;
      MarkerStyle = ExcelChartMarkerType.None;
      LineProperties.AutoFormat = true;

      if( type == ExcelChartType.Scatter_SmoothedLine
        || type == ExcelChartType.Scatter_SmoothedLine_Markers )
        IsSmoothedLine = true;

      if( type == ExcelChartType.Scatter_SmoothedLine_Markers
        || type == ExcelChartType.Scatter_Markers )
      {
        MarkerStyle = ExcelChartMarkerType.Diamond;
        IsAutoMarker = true;
      }

      if( type == ExcelChartType.Scatter_Markers )
      {
          if (!isSerieCreation)
               m_border.LinePattern = ExcelChartLinePattern.None;
        m_markerFormat = null;
      }
    }
    /// <summary>
    /// Changes data format to create line chart.
    /// </summary>
    /// <param name="type">Type to change.</param>
    public void ChangeLineDataFormat( ExcelChartType type )
    {
      if( type == ExcelChartType.Line
        || type == ExcelChartType.Line_Stacked
        || type == ExcelChartType.Line_Stacked_100 )
      {
        IsAutoMarker = false;
        //LineProperties.AutoFormat = true;
        //m_border.IsAutoLineColor = true;
        MarkerStyle = ExcelChartMarkerType.None;
      }

      if( type == ExcelChartType.Line_Markers
        || type == ExcelChartType.Line_Markers_Stacked
        || type == ExcelChartType.Line_Markers_Stacked_100 )
      {
        LineProperties.AutoFormat = false;
      }
    }
    /// <summary>
    /// Updates bar column properties in series and chartformat points.
    /// </summary>
    /// <param name="bIsDataTop">If true updates data format top property; otherwise data format base.</param>
    internal void UpdateBarFormat( bool bIsDataTop )
    {
      if( m_serie == null )
      {
        IChartSeries series = m_chart.Series;
        for( int i = 0, iLen = series.Count; i < iLen; i++ )
        {
          IChartSerieDataFormat format = series[ i ].DataPoints.DefaultDataPoint.DataFormat;

          if( bIsDataTop )
          {
            format.BarShapeTop = BarShapeTop;
          }
          else
          {
            format.BarShapeBase = BarShapeBase;
          }
        }

        return;
      }
    }
    /// <summary>
    /// Updates line color for line, radar, skater Series data format.
    /// </summary>
    /// <returns>Returns updated color index or -1.</returns>
    public int UpdateLineColor()
    {
      ExcelChartType type = SerieType;
      string strType = ChartFormatImpl.GetStartSerieType( type );

      bool bFlag = type == ExcelChartType.Radar_Markers || type == ExcelChartType.Radar
        || strType == ChartSerieDataFormatImpl.DEF_LINE_START_TYPE || strType ==
        ChartSerieDataFormatImpl.DEF_LINE_START_TYPE;

      if( !bFlag )
        return -1;

      return UpdateColor( m_serie, m_dataPoint );
    }
    /// <summary>
    /// Updates color for line, markers and series.
    /// </summary>
    /// <param name="serie">Represents series to update color.</param>
    /// <param name="dataPoint">Represents data point to update color.</param>
    /// <returns>Returns updated color index.</returns>
    public static int UpdateColor( ChartSerieImpl serie, ChartDataPointImpl dataPoint )
    {
      if( serie == null )
        return DEF_MARKER_START_COLOR;
      
      int iSerieIndex = serie.SerieFormat.CommonSerieOptions.IsVaryColor ?
        dataPoint.Index :
        serie.Index;

      if( iSerieIndex <= 30 )
        return iSerieIndex + DEF_MARKER_START_COLOR;

      iSerieIndex = iSerieIndex - 30;
      return iSerieIndex % 55 + 7;
    }
    /// <summary>
    /// Updates series formats for chartformat object.
    /// </summary>
    public void UpdateSerieFormat()
    {
      if( m_3DDataFormat == null )
        m_3DDataFormat = ( Chart3DDataFormatRecord )BiffRecordFactory.GetRecord( TBIFFRecord.Chart3DDataFormat );

      if( m_pieFormat == null )
        m_pieFormat = ( ChartPieFormatRecord )BiffRecordFactory.GetRecord( TBIFFRecord.ChartPieFormat );

      if( m_markerFormat == null && !m_chart.ParentWorkbook.Loading )
        m_markerFormat = ( ChartMarkerFormatRecord )BiffRecordFactory.GetRecord( TBIFFRecord.ChartMarkerFormat );

      if( m_seriesFormat == null )
        m_seriesFormat = ( ChartSerFmtRecord )BiffRecordFactory.GetRecord( TBIFFRecord.ChartSerFmt );

      bool bTypeChanging = m_chart.TypeChanging;
      bool bLoading = m_chart.ParentWorkbook.Loading;
      ExcelChartType destinationType = m_chart.DestinationType;

      bool bCreateCondition = !bLoading && ( bTypeChanging && GetIsBorderSupported( destinationType )
        || !bTypeChanging && IsBorderSupported );

      if( m_border == null && bCreateCondition )
      {
        m_border = new ChartBorderImpl( Application, this );
      }
      //else if( !bCreateCondition )
      //{
      //  m_border = null;
      //}

      bCreateCondition = !bLoading && ( bTypeChanging && GetIsInteriorSupported( destinationType )
        || !bTypeChanging && IsInteriorSupported );

      if( m_interior == null && bCreateCondition )
      {
        m_interior = new ChartInteriorImpl( Application, this );
      }
      //else if( !bCreateCondition )
      //{
      //  m_interior = null;
      //}

      m_bFormatted = true;
    }
    /// <summary>
    /// Checks for default record.
    /// </summary>
    /// <param name="record">Record to check.</param>
    /// <returns>Returns true if not equal; otherwise false.</returns>
    private bool ChackDataRecord( Chart3DDataFormatRecord record )
    {
      if( record == null )
        throw new ArgumentNullException( "record" );

      bool bResult = record.DataFormatBase != ExcelBaseFormat.Rectangle
        || record.DataFormatTop != ExcelTopFormat.Straight;

      return bResult;
    }
    /// <summary>
    /// Clears sub data formats on property change.
    /// </summary>
    public void ClearOnPropertyChange()
    {
      if( m_chart.Loading )
        return;

      if( m_format != null )
      {
        ChartSeriesCollection series = ( ChartSeriesCollection )m_chart.Series;
        series.ClearDataFormats( this );
      }
      else if( m_dataPoint != null && m_dataPoint.Index == ChartSerieImpl.DEF_FORMAT_ALLPOINTS_INDEX )
      {
        m_dataPoint.ClearDataFormats( this );
      }
    }
    /// <summary>
    /// Validate marker properties.
    /// </summary>
    /// <returns>If true than marker properties valid for this chart type; otherwise false.</returns>
    private bool ValidateMarkerProprties()
    {
      ExcelChartType type = SerieType;
      string strType = ChartFormatImpl.GetStartSerieType( type );

      bool bFlag = strType == ChartImpl.START_LINE || strType == ChartImpl.START_RADAR
        || strType == ChartImpl.START_SCATTER;

      if (type == ExcelChartType.Radar || strType == ChartImpl.START_SCATTER)
      {          
          HasMarkerProperties = true;
      }

      return bFlag && type != ExcelChartType.Line_3D && type != ExcelChartType.Radar_Filled; 
    }
    /// <summary>
    /// Indicates whether interior object is supported. Read-only.
    /// </summary>
    /// <param name="chartType">Chart type to check whether interior object is supported.</param>
    /// <returns>Value indicating whether interior object is supported.</returns>
    private static bool GetIsInteriorSupported( ExcelChartType chartType )
    {
      string strType = ChartFormatImpl.GetStartSerieType( chartType );

      bool bFlag = ( strType == DEF_LINE_START_TYPE && chartType != ExcelChartType.Line_3D )
        || chartType == ExcelChartType.Radar || chartType == ExcelChartType.Radar_Markers;

      return !( strType == DEF_SURFACE_START_TYPE || strType == DEF_SCATTER_START_TYPE || bFlag );
    }
    /// <summary>
    /// Indicates whether border object is supported. Read-only.
    /// </summary>
    /// <param name="chartType">Chart type to check whether border is supported.</param>
    /// <returns>Vale indicating whether border is supported.</returns>
    private static bool GetIsBorderSupported( ExcelChartType chartType )
    {
      string strType = ChartFormatImpl.GetStartSerieType( chartType );
      return true;
    }
    /// <summary>
    /// Event handler for marker foreground color change.
    /// </summary>
    private void MarkerForeColorChanged()
    {
      IsAutoMarker = false;
      ExcelKnownColors value = m_markerForeColor.GetIndexed( m_chart.Workbook );
      MarkerFormat.BorderColorIndex = ( ushort )value;
      MarkerFormat.IsNotShowBrd = ( value == ExcelKnownColors.None ) ? true : false;
      m_markerGradient = null;

      if( !m_chart.ParentWorkbook.Loading )
        m_markerLineStream = null;

      ClearOnPropertyChange();
    }
    /// <summary>
    /// Event handler for marker background color change.
    /// </summary>
    private void MarkerBackColorChanged()
    {
      IsAutoMarker = false;
      ExcelKnownColors value = m_markerBackColor.GetIndexed( m_chart.Workbook );
      MarkerFormat.FillColorIndex = ( ushort )value;
      MarkerFormat.IsNotShowInt = ( value == ExcelKnownColors.None ) ? true : false;
      ClearOnPropertyChange();
    }
    #endregion

    #region IChartSerieDataFormat properties
    /// <summary>
    /// Gets/sets value indicating whether line properties are created.
    /// </summary>
    public bool HasLineProperties
    {
      get
      {
        return m_border != null;
      }
      internal set
      {
        if( m_border == null && value )
          m_border = new ChartBorderImpl( Application, this );       
            
      }
    }
        /// <summary>
        /// Gets a value indicating whether this instance has shadow properties.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has shadow properties; otherwise, <c>false</c>.
        /// </value>
        public bool HasShadowProperties
        {
            get
            {
                return m_shadow != null;
            }
            internal set
            {
                if (value)
                {
                    IShadow shadow = Shadow;
                }
                else
                {
                    m_shadow = null;
                }
            }
        }
        /// <summary>
        /// Gets the chart3 D options.
        /// </summary>
        /// <value>The chart3 D options.</value>
        public IThreeDFormat ThreeD
        {
            get
            {
                if (m_3D == null)
                    m_3D = new ThreeDFormatImpl(Application, this);

                return m_3D;
            }
        }
        /// <summary>
        /// This property Indicates whether the Shadow object has been created(which includes the 3D properties)
        /// </summary>
        public bool Has3dProperties
        {
            get
            {
                return m_3D != null;
            }
            internal set
            {
                if (value)
                {
                    IThreeDFormat Threed = ThreeD;
                }
                else
                {
                    m_3D = null;
                }
            }
        }
    /// <summary>
    /// Gets/sets value indicating whether interior object was created.
    /// </summary>
    public bool HasInterior
    {
      get
      {
        if( !IsInteriorSupported )
          m_interior = null;

        return m_interior != null;
      }
      internal set
      {
        if( m_interior == null && value )
          m_interior = new ChartInteriorImpl( Application, this );
      }
    }
        public IShadow Shadow
        {
            get
            {
               
                if (m_shadow == null)
                    m_shadow = new ShadowImpl(Application, this);
                //if (m_shadow.HasCustomShadowStyle == true)
                //    throw new NotSupportedException("It is not supported when Custom Shadow style is set to true");
                return m_shadow;
            }
        }
    /// <summary>
    /// Returns object, that represents line properties. Read-only.
    /// </summary>
    public IChartBorder LineProperties
    {
      get
      {
        if( !m_chart.TypeChanging && !IsBorderSupported )
          throw new NotSupportedException( "This property dosn't support in this chart type" );

        if( m_chart.ParentWorkbook.Loading )
        {
          HasLineProperties = true;
        }
        else
        {
          UpdateSerieFormat();          
        }

        m_bFormatted = true;

        return m_border;
      }
    }
    /// <summary>
    /// Returns object, that represents area properties. Read-only.
    /// </summary>
    public IChartInterior AreaProperties
    {
      get
      {
        if( !m_chart.TypeChanging && m_chart.IsParsed && !IsInteriorSupported )
          throw new NotSupportedException( "This property dosn't support in this chart type" );

        UpdateSerieFormat();

        m_bFormatted = true;

        if( !m_chart.TypeChanging && m_chart.IsParsed && m_interior.UseAutomaticFormat == true )
        {
          ExcelKnownColors color = ( ExcelKnownColors )UpdateColor( m_serie, m_dataPoint );
          m_interior.ForegroundColorObject.SetIndexed( color );
          m_interior.UseAutomaticFormat = true;
        }

        return m_interior;
      }
    }
    /// <summary>
    /// Represents the base data format.
    /// </summary>
    public ExcelBaseFormat BarShapeBase
    {
      get
      {
        return Serie3DDataFormat.DataFormatBase;
      }
      set
      {
        if( value != BarShapeBase )
        {
          bool isLoding = m_chart.Loading;

          if( !isLoding && Array.IndexOf( DEF_SUPPORT_DATAFORMAT_PROPERTIES, SerieType ) == -1 )
            throw new NotSupportedException( "This property is not supported in current chart type." );

          Serie3DDataFormat.DataFormatBase = value;

          if( !isLoding )
            UpdateBarFormat( false );

          m_bFormatted = true;
          ClearOnPropertyChange();
        }
      }
    }
    /// <summary>
    /// Represents the top data format.
    /// </summary>
    public ExcelTopFormat BarShapeTop
    {
      get
      {
        return Serie3DDataFormat.DataFormatTop;
      }
      set
      {
        if( value != BarShapeTop )
        {
          bool isLoding = m_chart.Loading;

          if( !isLoding && Array.IndexOf( DEF_SUPPORT_DATAFORMAT_PROPERTIES, SerieType ) == -1 )
            throw new NotSupportedException( "This property is not supported in current chart type." );

          Serie3DDataFormat.DataFormatTop = value;

          if( !isLoding )
            UpdateBarFormat( true );

          m_bFormatted = true;
          ClearOnPropertyChange();
        }
      }
    }
    /// <summary>
    /// Foreground color: RGB value (high byte = 0).
    /// </summary>
    public Color MarkerBackgroundColor
    {
      get
      {
        return m_markerBackColor.GetRGB( m_chart.Workbook );
        //return m_chart.Workbook.GetPaletteColor( MarkerForegroundColorIndex );
      }
      set
      {
        //MarkerBackgroundColorIndex = m_chart.Workbook.GetNearestColor( value );
        //MarkerFormat.ForeColor = value.ToArgb() & 0xffffff;
         
        m_markerBackColor.SetRGB( value );
      }
    }
    /// <summary>
    /// Background color: RGB value (high byte = 0).
    /// </summary>
    public Color MarkerForegroundColor
    {
      get
      {
        return m_markerForeColor.GetRGB( m_chart.Workbook );
        //return m_chart.Workbook.GetPaletteColor( MarkerBackgroundColorIndex );
      }
      set
      {
          if (((WorkbookImpl)m_chart.Workbook).IsCreated|| Parent is ChartDataPointImpl)
              this.MarkerFormat.HasLineProperties = true;

        m_markerForeColor.SetRGB( value );
        //MarkerForegroundColorIndex = m_chart.Workbook.GetNearestColor( value );
        //MarkerFormat.BackColor = value.ToArgb() & 0xffffff;
      }
    }
    /// <summary>
    /// Type of marker.
    /// </summary>
    public ExcelChartMarkerType MarkerStyle
    {
      get
      {
        if( !m_chart.Loading && !m_chart.TypeChanging && !ValidateMarkerProprties() )
          throw new NotSupportedException( "This property is not supported in this chart type." );

        return MarkerFormat.MarkerType;
      }
      set
      {
        if( MarkerStyle != value )
        {
          MarkerFormat.MarkerType = value;
          IsAutoMarker = false;
          HasMarkerProperties = true;

          if( !m_chart.TypeChanging )
            ClearOnPropertyChange();
        }
      }
    }
    /// <summary>
    /// Index to color of marker border.
    /// </summary>
    public ExcelKnownColors MarkerForegroundColorIndex
    {
      get
      {
        if( !m_chart.Loading && !m_chart.TypeChanging && !ValidateMarkerProprties() )
          throw new NotSupportedException( "This property is not supported in this chart type." );

        return ( ExcelKnownColors )MarkerFormat.BorderColorIndex;
      }
      set
      {
        if( MarkerForegroundColorIndex != value )
        {
          m_markerForeColor.SetIndexed( value );
        }
      }
    }
    /// <summary>
    /// Index to color of marker fill.
    /// </summary>
    public ExcelKnownColors MarkerBackgroundColorIndex
    {
      get
      {
        if( !m_chart.Loading && !m_chart.TypeChanging && !ValidateMarkerProprties() )
          throw new NotSupportedException( "This property is not supported in this chart type." );

        return ( ExcelKnownColors )MarkerFormat.FillColorIndex;
      }
      set
      {
        if( MarkerBackgroundColorIndex != value )
        {
          m_markerBackColor.SetIndexed( value );
        }
      }
    }
    /// <summary>
    /// Size of markers.
    /// </summary>
    public int MarkerSize
    {
      get
      {
        if( !m_chart.Loading && !m_chart.TypeChanging && !ValidateMarkerProprties() )
          throw new NotSupportedException( "This property is not supported in this chart type." );

        return MarkerFormat.LineSize / DEF_MARKER_SIZE_MUL;
      }
      set
      {
        if( value != MarkerSize )
        {
          if( value < 2 || value > 72 )
            throw new ArgumentOutOfRangeException( "MarkerSize" );

          MarkerFormat.LineSize = value * DEF_MARKER_SIZE_MUL;

          if( MarkerFormat.IsAutoColor == true )
            MarkerFormat.MarkerType = ExcelChartMarkerType.Square;

          IsAutoMarker = false;

          ClearOnPropertyChange();
        }
      }
    }
    /// <summary>
    /// Automatic color.
    /// </summary>
    public bool IsAutoMarker
    {
      get
      {
        if( !m_chart.Loading && !m_chart.TypeChanging && !ValidateMarkerProprties() )
          throw new NotSupportedException( "This property is not supported in this chart type." );

        return MarkerFormat.IsAutoColor;
      }
      set
      {
        if( value != IsAutoMarker )
        {
          MarkerFormat.IsAutoColor = value;

          if( !value )
          {
            int result = UpdateColor( m_serie, m_dataPoint );

            MarkerFormat.FillColorIndex = ( ushort )( result );
            MarkerFormat.BorderColorIndex = ( ushort )( result );
          }

          if( !m_chart.TypeChanging )
            ClearOnPropertyChange();
        }
      }
    }
    /// <summary>
    /// True = "background = none".
    /// </summary>
    public bool IsNotShowInt
    {
      get
      {
        return MarkerFormat.IsNotShowInt;
      }
      set
      {
        MarkerFormat.IsNotShowInt = value;
      }
    }
    /// <summary>
    /// True = "foreground = none".
    /// </summary>
    public bool IsNotShowBrd
    {
      get
      {
        return MarkerFormat.IsNotShowBrd;
      }
      set
      {
        MarkerFormat.IsNotShowBrd = value;
      }
    }
    /// <summary>
    /// Distance of pie slice from center of pie.
    /// </summary>
    public int Percent
    {
      get
      {
        return PieFormat.Percent;
      }
      set
      {
        if( !m_chart.TypeChanging )
        {
          string strType = ChartFormatImpl.GetStartSerieType( SerieType );

          if( strType != DEF_PIE_START_TYPE && strType != DEF_DOUGHNUT_START_TYPE )
            throw new NotSupportedException( "This property is not supported in current chart type." );
        }

        PieFormat.Percent = ( ushort )value;

        ClearOnPropertyChange();
      }
    }
    /// <summary>
    /// True if the line series has a smoothed line.
    /// </summary>
    public bool IsSmoothedLine
    {
      get
      {
        return SerieFormat.IsSmoothedLine;
      }
      set
      {
        SerieFormat.IsSmoothedLine = value;
      }
    }
    /// <summary>
    /// True to draw bubbles with 3D effects.
    /// </summary>
    public bool Is3DBubbles
    {
      get
      {
        return SerieFormat.Is3DBubbles;
      }
      set
      {
        if( Is3DBubbles != value )
        {
          ExcelChartType type = SerieType;

          if( type != ExcelChartType.Bubble && type != ExcelChartType.Bubble_3D )
            throw new NotSupportedException( "This property is not supported in this chart type." );

          SerieFormat.Is3DBubbles = value;

          ClearOnPropertyChange();
        }
      }
    }
    /// <summary>
    /// True if this series has a shadow.
    /// </summary>
    public bool IsArShadow
    {
      get
      {
        return SerieFormat.IsArShadow;
      }
      set
      {
        SerieFormat.IsArShadow = value;
      }
    }
    /// <summary>
    /// Gets or sets value indicating whether to show label active value.
    /// </summary>
    public bool ShowActiveValue
    {
      get
      {
        return AttachedLabel.ShowActiveValue;
      }
      set
      {
        AttachedLabel.ShowActiveValue = value;
      }
    }
    /// <summary>
    /// Show value as a percent of the total. This bit applies only to pie charts.
    /// </summary>
    public bool ShowPieInPercents
    {
      get
      {
        return AttachedLabel.ShowPieInPercents;
      }
      set
      {
        AttachedLabel.ShowPieInPercents = value;
      }
    }
    /// <summary>
    /// Show category label and value as a percentage (pie charts only).
    /// </summary>
    public bool ShowPieCategoryLabel
    {
      get
      {
        return AttachedLabel.ShowPieCategoryLabel;
      }
      set
      {
        AttachedLabel.ShowPieCategoryLabel = value;
      }
    }
    /// <summary>
    /// Show smoothed line.
    /// </summary>
    public bool SmoothLine
    {
      get
      {
        return AttachedLabel.SmoothLine;
      }
      set
      {
        AttachedLabel.SmoothLine = value;
      }
    }
    /// <summary>
    /// Show category label.
    /// </summary>
    public bool ShowCategoryLabel
    {
      get
      {
        return AttachedLabel.ShowCategoryLabel;
      }
      set
      {
        AttachedLabel.ShowCategoryLabel = value;
      }
    }
    /// <summary>
    /// Show bubble sizes.
    /// </summary>
    public bool ShowBubble
    {
      get
      {
        return AttachedLabel.ShowBubble;
      }
      set
      {
        AttachedLabel.ShowBubble = value;
      }
    }
    /// <summary>
    /// Represents fill options. Read-only.
    /// </summary>
    public IFill Fill
    {
      get
      {
        if( !m_chart.TypeChanging && m_chart.IsParsed )
        {
          if( !IsSupportFill )
            throw new NotSupportedException( "This property isn't supported in this chart type" );

          UpdateSerieFormat();
          //IsAutomaticFormat = false;
        }

        return m_fill;
      }
    }
    /// <summary>
    /// Gets value indicating whether chart supports transparency.
    /// </summary>
    public bool IsSupportFill
    {
      get
      {
          ExcelChartType type = SerieType;
          string strType = ChartFormatImpl.GetStartSerieType( type );

          bool bFlag = ( strType == DEF_LINE_START_TYPE && type != ExcelChartType.Line_3D )
            || type == ExcelChartType.Radar || type == ExcelChartType.Radar_Markers;

          return !( strType == DEF_SURFACE_START_TYPE || strType == DEF_SCATTER_START_TYPE || bFlag );
      }
    }
    /// <summary>
    /// Gets common Series options. Read-only.
    /// </summary>
    public IChartFormat CommonSerieOptions
    {
      get
      {
        if( m_serie == null )
          throw new NotSupportedException( "Cannot get series options." );

        return m_serie.GetCommonSerieFormat();
      }
    }
    /// <summary>
    /// Indicates whether marker is supported by this chart/series.
    /// </summary>
    public bool IsMarkerSupported
    {
      get
      {
        return ValidateMarkerProprties();
      }
    }
    /// <summary>
    /// Returns object, that represents area properties. Read-only.
    /// </summary>
    public IChartInterior Interior
    {
      get
      {
        return AreaProperties;
      }
    }
    /// <summary>
    /// Indicates whether interior object is supported. Read-only.
    /// </summary>
    public bool IsInteriorSupported
    {
      get
      {
        return GetIsInteriorSupported( SerieType );
      }
    }
    /// <summary>
    /// Indicates whether border object is supported. Read-only.
    /// </summary>
    public bool IsBorderSupported
    {
      get
      {
        return GetIsBorderSupported( SerieType );
      }
    }
    /// <summary>
    /// Indicates whether the marker properties exists or not
    /// </summary>
    internal bool HasMarkerProperties
    {
        get
        {
            return m_HasMarkerProperties;
        }
        set
        {
            m_HasMarkerProperties = value;
        }
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Returns parent Series. Read-only.
    /// </summary>
    public ChartSerieImpl ParentSerie
    {
      get
      {
        return m_serie;
      }
    }
    /// <summary>
    /// Returns data format main record.
    /// </summary>
    [ CLSCompliant( false ) ]
    public ChartDataFormatRecord DataFormat
    {
      get
      {
        return m_dataFormat;
      }
      set
      {
        m_dataFormat = value;
      }
    }
    /// <summary>
    /// Returns pie format record. Read-only.
    /// </summary>
    [ CLSCompliant( false ) ]
    public ChartPieFormatRecord PieFormat
    {
      get
      {
        if( m_pieFormat == null )
        {
          m_pieFormat = ( ChartPieFormatRecord )
            BiffRecordFactory.GetRecord( TBIFFRecord.ChartPieFormat );
        }

        m_bFormatted = true;

        return m_pieFormat;
      }
    }
    /// <summary>
    /// Returns marker format main record. Read-only.
    /// </summary>
    [ CLSCompliant( false ) ]
    public ChartMarkerFormatRecord MarkerFormat
    {
      get
      {
        if( m_markerFormat == null )
        {
          m_markerFormat = ( ChartMarkerFormatRecord )
            BiffRecordFactory.GetRecord( TBIFFRecord.ChartMarkerFormat );

          m_bFormatted = true;

          m_markerFormat.IsAutoColor = true;
        }

        return m_markerFormat;
      }
    }
    /// <summary>
    /// Returns 3dData format main record. Read-only.
    /// </summary>
    [ CLSCompliant( false ) ]
    public Chart3DDataFormatRecord Serie3DDataFormat
    {
      get
      {
        if( m_3DDataFormat == null )
        {
          m_3DDataFormat = ( Chart3DDataFormatRecord )
            BiffRecordFactory.GetRecord( TBIFFRecord.Chart3DDataFormat );
        }

        return m_3DDataFormat;
      }
    }
    /// <summary>
    /// Returns Series format main record. Read-only.
    /// </summary>
    [ CLSCompliant( false ) ]
    public ChartSerFmtRecord SerieFormat
    {
      get
      {
        if( m_seriesFormat == null )
        {
          m_seriesFormat = ( ChartSerFmtRecord )
            BiffRecordFactory.GetRecord( TBIFFRecord.ChartSerFmt );
        }

        m_bFormatted = true;

        return m_seriesFormat;
      }
    }
    /// <summary>
    /// Returns attached label record. Read-only.
    /// </summary>
    [ CLSCompliant( false ) ]
    public ChartAttachedLabelRecord AttachedLabel
    {
      get
      {
        if( m_attachedLabel == null )
        {
          UpdateSerieFormat();
          m_attachedLabel = ( ChartAttachedLabelRecord )
            BiffRecordFactory.GetRecord( TBIFFRecord.ChartAttachedLabel );
        }

        return m_attachedLabel;
      }
    }
    /// <summary>
    /// Return attached label layout record. Read-only
    /// </summary>
    [ CLSCompliant( false ) ]
    public ChartAttachedLabelLayoutRecord AttachedLabelLayout
    {
        get
        {
            if (m_attachedLabelLayout == null)
            {
                m_attachedLabelLayout = ( ChartAttachedLabelLayoutRecord )
                    BiffRecordFactory.GetRecord( TBIFFRecord.ChartAttachedLabelLayout );
            }
            return m_attachedLabelLayout;
        }
    }
    /// <summary>
    /// If line not null - returns true; otherwise - false. Read-only.
    /// </summary>
    public bool ContainsLineProperties
    {
      get
      {
        return m_border != null;
      }
    }
    /// <summary>
    /// If marker is not null returns marker format main record otherwise null. Read-only.
    /// </summary>
    [ CLSCompliant( false ) ]
    public ChartMarkerFormatRecord MarkerFormatOrNull
    {
      get
      {
        return m_markerFormat;
      }
    }
    /// <summary>
    /// If 3dData not Null returns 3DData format main record; otherwise - returns null. Read-only.
    /// </summary>
    [ CLSCompliant( false ) ]
    public Chart3DDataFormatRecord Serie3DdDataFormatOrNull
    {
      get
      {
        return m_3DDataFormat;
      }
    }
    /// <summary>
    /// if SerieFormat not Null returns Series format main record; otherwise - returns null. Read-only.
    /// </summary>
    [ CLSCompliant( false ) ]
    public ChartSerFmtRecord SerieFormatOrNull
    {
      get
      {
        return m_seriesFormat;
      }
    }
    /// <summary>
    /// Returns pie format or null. Read-only.
    /// </summary>
    [ CLSCompliant( false ) ]
    public ChartPieFormatRecord PieFormatOrNull
    {
      get
      {
        return m_pieFormat;
      }
    }
    /// <summary>
    /// Gets or sets Series number.
    /// </summary>
    public int SeriesNumber
    {
      get
      {
        return m_dataFormat.SeriesNumber;
      }
      set
      {
        m_dataFormat.SeriesNumber = ( ushort )value;
      }
    }
    /// <summary>
    /// If true - format has marker; otherwise false. Read-only.
    /// </summary>
    public bool IsMarker
    {
      get
      {
        return !( MarkerFormatOrNull != null && MarkerFormatOrNull.MarkerType == ExcelChartMarkerType.None );
      }
    }
    /// <summary>
    /// If true - format has line; otherwise false. Read-only.
    /// </summary>
    public bool IsLine
    {
      get
      {
        return !( m_border != null && !m_border.AutoFormat
          && m_border.LinePattern == ExcelChartLinePattern.None );
      }
    }
    /// <summary>
    /// If true - format has smoothed line; otherwise false. Read-only.
    /// </summary>
    public bool IsSmoothed
    {
      get
      {
        return SerieFormatOrNull != null && IsSmoothedLine;
      }
    }
    /// <summary>
    /// Gets Series type. Read-only.
    /// </summary>
    private ExcelChartType SerieType
    {
      get
      {
        if( m_serie != null )
          return m_serie.SerieType;

        ChartSeriesCollection series = ( ChartSeriesCollection )m_chart.Series;

        return series.GetTypeByOrder(m_dataFormat.SeriesIndex );
      }
    }
    
    /// <summary>
    /// Indicate if data format is formatted.
    /// </summary>
    public bool IsFormatted
    {
      get
      {
        return m_bFormatted;
      }
    }
    /// <summary>
    /// Represents parent chart. Read-only.
    /// </summary>
    public ChartImpl ParentChart
    {
      get
      {
        return m_chart;
      }
    }
    /// <summary>
    /// Gets object that holds marker background color.
    /// </summary>
    public ColorObject MarkerBackColorObject
    {
      get
      {
        return m_markerBackColor;
      }
    }
    /// <summary>
    /// Gets object that holds marker foreground color.
    /// </summary>
    public ColorObject MarkerForeColorObject
    {
      get
      {
        return m_markerForeColor;
      }
    }
    internal GradientStops MarkerGradient
    {
      get
      {
        return m_markerGradient;
      }
      set
      {
        m_markerGradient = value;
      }
    }
    /// <summary>
    /// Gets or sets the transparency of the line marker.
    /// </summary>
    /// <value>The transparency.</value>
    public double MarkerTransparency
    {
      get
      {
        return m_markerTransparency;
      }
      set
      {
        m_markerTransparency = value;
      }
    }
    public Stream MarkerLineStream
    {
      get
      {
        return m_markerLineStream;
      }
      set
      {
        m_markerLineStream = value;
      }
    }
    internal Stream EffectListStream
    {
        get
        {
            return m_markerEffectList;
        }
        set
        {
            m_markerEffectList = value;
        }
    }
    /// <summary>
    /// Gets or sets whether the data point is parsed or not. 
    /// If parsed, it will be serialized.
    /// </summary>
    internal bool IsParsed
    {
        get
        {
            return m_bIsParsed;
        }
        set
        {
            m_bIsParsed = value;
        }
    }
    #endregion

    #region IFillColor properties
    /// <summary>
    /// Represents foreground color.
    /// </summary>
    public ColorObject ForeGroundColorObject
    {
      get
      {
          if (AreaProperties == null)
              return null;
          else
              return (AreaProperties as ChartInteriorImpl).ForegroundColorObject;
      }
    }
    /// <summary>
    /// Represents background color.
    /// </summary>
    public ColorObject BackGroundColorObject
    {
      get
      {
          if (AreaProperties == null)
              return null;
          else
              return ( AreaProperties as ChartInteriorImpl ).BackgroundColorObject;
      }
    }
    /// <summary>
    /// Represents pattern.
    /// </summary>
    public ExcelPattern Pattern
    {
      get
      {
        return AreaProperties.Pattern;
      }
      set
      {
        AreaProperties.Pattern = value;
      }
    }
    /// <summary>
    /// Indicates, if automatic format is used for area.
    /// </summary>
    public bool IsAutomaticFormat
    {
      get
      {
        return AreaProperties.UseAutomaticFormat;
      }
      set
      {
        AreaProperties.UseAutomaticFormat = value;
      }
    }
    /// <summary>
    /// Represents visibility.
    /// </summary>
    public bool Visible
    {
      get
      {
        return AreaProperties.Pattern != ExcelPattern.None;
      }
      set
      {
        if( value )
        {
          if( AreaProperties.Pattern == ExcelPattern.None )
            AreaProperties.Pattern = ExcelPattern.Solid;
        }
        else
        {
          AreaProperties.Pattern = ExcelPattern.None;
        }
      }
    }
    #endregion
  }
}
