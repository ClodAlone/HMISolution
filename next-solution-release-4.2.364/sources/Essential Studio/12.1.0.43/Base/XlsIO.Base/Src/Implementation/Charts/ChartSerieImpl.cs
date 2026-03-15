#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#region file using directives
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;

using Syncfusion.XlsIO;
using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser.Biff_Records.Charts;
using Syncfusion.XlsIO.Parser.Biff_Records.Formula;
using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Implementation.Collections;
using Syncfusion.XlsIO.Implementation.Exceptions;

using LinkIndex = Syncfusion.XlsIO.Parser.Biff_Records.Charts.ChartAIRecord.LinkIndex;
using ReferenceType = Syncfusion.XlsIO.Parser.Biff_Records.Charts.ChartAIRecord.ReferenceType;
using System.Collections.Generic;

#if ( WINRT )
using Windows.UI;
using Rectangle = Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
#endif

#if  (SILVERLIGHT)
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.Silverlight.Rectangle;
#elif (WP)
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.WP.Rectangle;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;
#endif

#endregion

namespace Syncfusion.XlsIO.Implementation.Charts
{
  /// <summary>
  /// This class represents ChartSeries object.
  /// </summary>
  public class ChartSerieImpl
    : CommonObject
    , IChartSerie
    , ISerializableNamedObject
    , IReparse
  {
    #region Class constants
    /// <summary>
    /// Index for format that corresponds to all points in the Series.
    /// </summary>
    public const int DEF_FORMAT_ALLPOINTS_INDEX = 65535;
    /// <summary>
    /// Represents default point number for surface chart type.
    /// </summary>
    private const int DEF_SURFACE_POINT_NUMBER = ushort.MaxValue - 3;
    /// <summary>
    /// Represents default chart group.
    /// </summary>
    public const int DEF_CHART_GROUP = -1;
    /// <summary>
    /// Represents start radar type.
    /// </summary>
    private const string DEF_RADAR_START_TYPE = "Radar";
    /// <summary>
    /// Represents true as string.
    /// </summary>
    public const string DEF_TRUE = "TRUE";
    /// <summary>
    /// Represents false as string.
    /// </summary>
    public const string DEF_FALSE = "FALSE";
    #endregion

    #region Events
    /// <summary>
    /// This event is raised when ValueRange was changed.
    /// </summary>
    public event ValueChangedEventHandler ValueRangeChanged;
    #endregion

    #region Class members
    /// <summary>
    /// Values range for the series.
    /// </summary>
    private IRange m_ValueRange;
    /// <summary>
    /// Category labels for the series.
    /// </summary>
    private IRange m_CategoryRange;
    /// <summary>
    /// Bubble sizes for the series.
    /// </summary>
    private IRange m_BubbleRange;
    /// <summary>
    /// Name of the series.
    /// </summary>
    private string m_strName;
    private Ptg[] m_nameTokens;
    /// <summary>
    /// Dictionary IndexIdentifier-to-ChartAiRecord
    /// </summary>
    private Dictionary<ChartAIRecord.LinkIndex, ChartAIRecord> m_hashAi = new Dictionary<ChartAIRecord.LinkIndex, ChartAIRecord>();
    /// <summary>
    /// Index of the chart group this axis belongs to.
    /// </summary>
    private int m_iChartGroup;
    /// <summary>
    /// Parent workbook for the series.
    /// </summary>
    private WorkbookImpl m_book;
    /// <summary>
    /// ChartSeries record describing this series.
    /// </summary>
    private ChartSeriesRecord m_series;
    /// <summary>
    /// Parent chart for the series.
    /// </summary>
    private ChartImpl m_chart;
    /// <summary>
    /// Parent series collection.
    /// </summary>
    private ChartSeriesCollection m_seriesColl;
    /// <summary>
    /// Index of the series.
    /// </summary>
    private int m_iIndex;
    /// <summary>
    /// Drawing order of the series.
    /// </summary>
    private int m_iOrder;
    /// <summary>
    /// Indicates whether series name has default value.
    /// </summary>
    private bool m_bDefaultName = true;
    /// <summary>
    /// Data points collection.
    /// </summary>
    private ChartDataPointsCollection m_dataPoints;
    /// <summary>
    /// Represents Series type.
    /// </summary>
    private ExcelChartType m_serieType;
    /// <summary>
    /// Represents array of number or label records for value range.
    /// </summary>
    private List<BiffRecordRaw> m_valueEnteredDirectly = new List<BiffRecordRaw>();
    /// <summary>
    /// Represents array of number or label records for category range.
    /// </summary>
    private List<BiffRecordRaw> m_categoryEnteredDirectly = new List<BiffRecordRaw>();
    /// <summary>
    /// Represents array of number or label records for bubble range.
    /// </summary>
    private List<BiffRecordRaw> m_bubbleEnteredDirectly = new List<BiffRecordRaw>();
    /// <summary>
    /// Represents array of values for value range.
    /// </summary>
    private object[] m_enteredDirectlyValue;
    /// <summary>
    /// Represents array of values for category range.
    /// </summary>
    private object[] m_enteredDirectlyCategory;
    /// <summary>
    /// Represents array of values for bubble range.
    /// </summary>
    private object[] m_enteredDirectlyBubble;
    /// <summary>
    /// Represents series name range.
    /// </summary>
    private IRange m_nameRange;
    /// <summary>
    /// Represents not default series text for first Series on parsing.
    /// </summary>
    private string m_seriesText;
    /// <summary>
    /// Represents Y error bar.
    /// </summary>
    private ChartErrorBarsImpl m_errorBarY;
    /// <summary>
    /// Represents X error bar.
    /// </summary>
    private ChartErrorBarsImpl m_errorBarX;
    /// <summary>
    /// Represents trend line collection.
    /// </summary>
    private ChartTrendLineCollection m_trendLines;
    /// <summary>
    /// This element specifies the series
    /// to invert its colors if the value is negative.
    /// </summary>
    private bool? m_bInvertIfNegative=null;
    /// <summary>
    /// Represents the string reference formula
    /// </summary>
    private string m_strRefFormula;
    /// <summary>
    /// Represents the number reference formula
    /// </summary>
    private string m_numRefFormula;
    /// <summary>
    /// Represents the number reference formula
    /// </summary>
    private string m_MulLvlStrRefFormula;
    /// <summary>
    /// Preserve the drop lines
    /// </summary>
    private Stream m_dropLinesStream;
    /// <summary>
    /// Reprsent the Filter option
    /// </summary>
    private bool m_IsFiltered;
    /// <summary>
    /// Reprsent the category filter range
    /// </summary>
    private string m_categoryFilteredRange;
    /// <summary>
    /// Reprsent the category value Range
    /// </summary>
    private string m_categoryValue;
    /// <summary>
    /// Specifies the kind of grouping for a column, line or area chart
    /// </summary>
    private string m_grouping;
    /// <summary>
    /// Represents chart gapWidth for the first series
    /// </summary>
    private int m_gapWidth;
    /// <summary>
    /// Represents chart overlap for the first series
    /// </summary>
    private int m_overlap;
    /// <summary>
    /// Represents whether to serialize gapwidth 
    /// </summary>
    private bool m_bShowGapWidth;
    #endregion
    
    #region Class constructors
    /// <summary>
    /// Creates series and set its Application and Parent
    /// properties to specified values.
    /// </summary>
    /// <param name="application">Application object for the chart.</param>
    /// <param name="parent">Parent object for the chart.</param>
    public ChartSerieImpl( IApplication application, object parent )
      : base( application, parent )
    {
      SetParents();
      InitializeCollections();
    }
    /// <summary>
    /// Creates series from the array of BiffRecords.
    /// </summary>
    /// <param name="application">Application object for the series.</param>
    /// <param name="parent">Parent object for the series.</param>
    /// <param name="data">Array of BiffRecords with series data.</param>
    /// <param name="iPos">Position of the first series record.</param>
    [ CLSCompliant( false ) ]
    public ChartSerieImpl( IApplication application, object parent,
      IList<BiffRecordRaw> data, ref int iPos )
      : this( application, parent )
    {
      Parse( data, ref iPos );
    }
    #endregion

    #region IChartSerie Members
    /// <summary>
    /// Name of the series.
    /// </summary>
    public string Name
    {
      get
      {
        return DetectSerieName();
      }
      set
      {
        if( m_strName != value )
        {
          OnNameChanged( value );
          m_strName = value;
          m_bDefaultName = false;

          if( value != null && value.Length>0)
          {             
           if ( value[ 0 ] == '=' )
          {
            m_nameTokens = GetNameTokens();
          }
          }

          if( !m_chart.Loading && ( m_chart.ChartTitle == null || m_chart.ChartTitle.Length == 0 ) )
          {
            string strType = ChartFormatImpl.GetStartSerieType( m_chart.ChartType );

            if( !m_bDefaultName && ( strType == ChartImpl.START_PIE
              || strType == ChartImpl.START_DOUGHNUT ) )
            {
              m_chart.ChartTitle = Name;
            }
          }
        }
      }
    }
    /// <summary>
    /// Series Name range for the series.
    /// </summary>
    /// <value></value>
    public IRange NameRange
    {
        get
        {
            //if (m_nameRange == null)
            //    throw new ArgumentNullException("Name Range");

            this.DetectSerieName();
            return m_nameRange;
        }
    }
    /// <summary>
    /// Values range for the series.
    /// </summary>
    public IRange Values
    {
      get
      {
        return m_ValueRange;
      }
      set
      {
        if( m_ValueRange != value )
        {
          m_valueEnteredDirectly.Clear();

          ValueChangedEventArgs args = new ValueChangedEventArgs( m_ValueRange,
            value, "ValueRange" );

          m_ValueRange = value;
          OnValueRangeChanged( args );
        }
      }
    }

    /// <summary>
    /// Category labels for the series.
    /// </summary>
    public IRange CategoryLabels
    {
      get
      {
        return m_CategoryRange;
      }
      set
      {
        if( m_CategoryRange != value )
        {
          m_categoryEnteredDirectly.Clear();

          m_CategoryRange = value;
          OnCategoryRangeChanged();
        }
      }
    }

    /// <summary>
    /// Bubble sizes for the series.
    /// </summary>
    public IRange Bubbles
    {
      get
      {
        return m_BubbleRange;
      }
      set
      {
        if( m_BubbleRange != value )
        {
          m_bubbleEnteredDirectly.Clear();

          m_BubbleRange = value;
          OnBubbleRangeChanged();
        }
      }
    }
    /// <summary>
    /// Synonym for Index property.
    /// </summary>
    public int RealIndex
    {
      get
      {
        return Index;
      }
      set
      {
        Index = value;
      }
    }
    /// <summary>
    /// Returns data points collection for the chart series. Read-only.
    /// </summary>
    public IChartDataPoints DataPoints
    {
      get
      {
        if( m_dataPoints == null )
          m_dataPoints = new ChartDataPointsCollection( Application, this );

        return m_dataPoints;
      }
    }
    /// <summary>
    /// Returns format of current Series.
    /// </summary>
    public IChartSerieDataFormat SerieFormat
    {
      get
      {
        return m_dataPoints.DefaultDataPoint.DataFormat;
      }
    }
    /// <summary>
    /// Represents Series type.
    /// </summary>
    public ExcelChartType SerieType
    {
      get
      {
        return DetectSerieType();
      }
      set
      {
        ChangeSeriesType( value, false );
        (SerieFormat as ChartSerieDataFormatImpl).HasMarkerProperties = true;
      }
    }
    /// <summary>
    /// Indicates whether to use primary axis for series drawing.
    /// </summary>
    public bool UsePrimaryAxis
    {
      get
      {
        ChartFormatImpl format = GetCommonSerieFormat();
        return format.IsPrimaryAxis;
      }
      set
      {
        if( Array.IndexOf( ChartImpl.DEF_CHANGE_SERIE, SerieType ) == -1 )
          throw new NotSupportedException( "Property not supported for current serie type" );

        if( value != UsePrimaryAxis )
        {
          ChangeAxis( value );
          m_chart.IsManuallyFormatted = true;
        }

        if( !value )
          m_chart.SecondaryParentAxis.UpdateSecondaryAxis( true );

        if (!m_seriesColl.HasSecondary())
            m_chart.RemoveSecondaryAxes();
      }
    }
    /// <summary>
    /// Represents value as entered directly.
    /// </summary>
    public object[] EnteredDirectlyValues
    {
      get
      {
        if( m_enteredDirectlyValue == null )
        {
          m_enteredDirectlyValue = GetEnteredDirectlyValues( m_valueEnteredDirectly );
        }

        return m_enteredDirectlyValue;
      }
      set
      {
        if( value == null || value.Length == 0 )
          throw new ArgumentNullException( "value" );
          
        bool bIsNumber = GetEnteredDirectlyType( value );
        m_valueEnteredDirectly = GetArrayRecordsByValues( bIsNumber, value );
        m_enteredDirectlyValue = value;
      }
    }
    /// <summary>
    /// Represents category values as entered directly.
    /// </summary>
    public object[] EnteredDirectlyCategoryLabels
    {
      get
      {
        if( m_enteredDirectlyCategory == null )
        {
          m_enteredDirectlyCategory = GetEnteredDirectlyValues( m_categoryEnteredDirectly );
        }

        return m_enteredDirectlyCategory;
      }
      set
      {
        if( value == null || value.Length == 0 )
          throw new ArgumentNullException( "value" );
        bool bIsNumber = GetEnteredDirectlyType( value );
        m_categoryEnteredDirectly = GetArrayRecordsByValues( bIsNumber, value );
        m_enteredDirectlyCategory = value;
      }
    }
    /// <summary>
    /// Represents bubble values as entered directly.
    /// </summary>
    public object[] EnteredDirectlyBubbles
    {
      get
      {
        if( m_enteredDirectlyBubble == null )
        {
          m_enteredDirectlyBubble = GetEnteredDirectlyValues( m_bubbleEnteredDirectly );
        }

        return m_enteredDirectlyBubble;
      }
      set
      {
        if( value == null || value.Length == 0 )
          throw new ArgumentNullException( "value" );

        Bubbles = null;
        bool bIsNumber = GetEnteredDirectlyType( value );
        m_bubbleEnteredDirectly = GetArrayRecordsByValues( bIsNumber, value );
        m_enteredDirectlyBubble = value;
      }
    }
    /// <summary>
    /// Represents Y error bars. Read-only.
    /// </summary>
    public IChartErrorBars ErrorBarsY
    {
      get
      {
        if( m_errorBarY == null )
          throw new ApplicationException( "Use HasErrorBarsY property to create error bars." );

        return m_errorBarY;
      }
    }
    /// <summary>
    /// Indicates if Series contains Y error bars.
    /// </summary>
    public bool HasErrorBarsY
    {
      get
      {
        return m_errorBarY != null;
      }
      set
      {
        if( HasErrorBarsY != value )
        {
          if( !value )
          {
            m_errorBarY = null;
          }
          else
          {
            string strStart = ChartFormatImpl.GetStartSerieType( SerieType );

            if( m_chart.IsChart3D || Array.IndexOf( ChartImpl.DEF_SUPPORT_ERROR_BARS, strStart ) == -1 )
              throw new NotSupportedException( "Current serie doesnot support Y error bars." );

            if( m_errorBarY == null )
              m_errorBarY = new ChartErrorBarsImpl( Application, this, true );
          }
        }
      }
    }
    /// <summary>
    /// Represents X error bars. Read-only.
    /// </summary>
    public IChartErrorBars ErrorBarsX
    {
      get
      {
        if( m_errorBarX == null )
          throw new ApplicationException( "Use HasErrorBarsX property to create error bars." );

        return m_errorBarX;
      }
    }
    /// <summary>
    /// Indicates if Series contains X error bars.
    /// </summary>
    public bool HasErrorBarsX
    {
      get
      {
        return m_errorBarX != null;
      }
      set
      {
        if( HasErrorBarsX != value )
        {
          if( !value )
          {
            m_errorBarX = null;
          }
          else
          {
            if( ChartFormatImpl.GetStartSerieType( SerieType ) != ChartImpl.START_SCATTER 
              && ChartFormatImpl.GetStartSerieType( SerieType ) != ChartImpl.START_BUBBLE )
              throw new NotSupportedException( "Current serie doesnot support X error bars." );

            if( m_errorBarX == null )
              m_errorBarX = new ChartErrorBarsImpl( Application, this, false );
          }
        }
      }
    }
    /// <summary>
    /// Represents Series trend lines collection. Read-only.
    /// </summary>
    public IChartTrendLines TrendLines
    {
      get
      {
        return m_trendLines;
      }
    }
    /// <summary>
    /// Gets or sets the value that represents the kind of grouping for a column, line or area chart
    /// </summary>
    internal string Grouping
    {
        get
        {
            return m_grouping;
        }
        set
        {
            m_grouping = value;
        }
    }
    /// <summary>
    /// Gets or sets the value of chart gapWidth of first series
    /// </summary>
    internal int GapWidth
    {
        get
        {
            return m_gapWidth;
        }
        set
        {
            m_gapWidth = value;
        }
    }
    /// <summary>
    /// Gets or sets the value of chart overlap of first series
    /// </summary>
    internal int Overlap
    {
        get
        {
            return m_overlap;
        }
        set
        {
            m_overlap = value;
        }
    }
    /// <summary>
    /// Represents whether to serialize gapwidth 
    /// </summary>
    internal bool ShowGapWidth
    {
        get
        {
            return m_bShowGapWidth;
        }
        set
        {
            m_bShowGapWidth = value;
        }
    }
    #endregion

    #region IChartSerie methods
    /// <summary>
    /// Creates error bar object.
    /// </summary>
    /// <param name="bIsY">If true - on Y axis; otherwise on X axis.</param>
    /// <returns>Return error bar object.</returns>
    public IChartErrorBars ErrorBar( bool bIsY )
    {
      return ErrorBar( bIsY, ExcelErrorBarInclude.Both );
    }
    /// <summary>
    /// Creates error bar object.
    /// </summary>
    /// <param name="bIsY">If true - on Y axis; otherwise on X axis.</param>
    /// <param name="include">Represents include type.</param>
    /// <returns>Return error bar object.</returns>
    public IChartErrorBars ErrorBar( bool bIsY, ExcelErrorBarInclude include )
    {
      return ErrorBar( bIsY, include, ExcelErrorBarType.Fixed );
    }
    /// <summary>
    /// Creates error bar object.
    /// </summary>
    /// <param name="bIsY">If true - on Y axis; otherwise on X axis.</param>
    /// <param name="include">Represents include type.</param>
    /// <param name="type">Represents error bar type.</param>
    /// <returns>Return error bar object.</returns>
    public IChartErrorBars ErrorBar( bool bIsY, ExcelErrorBarInclude include
      , ExcelErrorBarType type )
    {
      double value = ( bIsY )
        ? ChartErrorBarsImpl.DEF_NUMBER_Y_VALUE
        : ChartErrorBarsImpl.DEF_NUMBER_X_VALUE;

      return ErrorBar( bIsY, include, type, value );
    }
    /// <summary>
    /// Creates error bar object.
    /// </summary>
    /// <param name="bIsY">If true - on Y axis; otherwise on X axis.</param>
    /// <param name="include">Represents include type.</param>
    /// <param name="type">Represents error bar type.</param>
    /// <param name="numberValue">Represents number value.</param>
    /// <returns>Returns error bar object.</returns>
    public IChartErrorBars ErrorBar( bool bIsY, ExcelErrorBarInclude include
      , ExcelErrorBarType type, double numberValue )
    {
      if( type == ExcelErrorBarType.Custom )
        throw new ArgumentException( "For sets custom type use another overload method" );

      ChartErrorBarsImpl bar = null;

      if( bIsY )
      {
        HasErrorBarsY = true;
        bar = m_errorBarY;
      }
      else
      {
        HasErrorBarsX = true;
        bar = m_errorBarX;
      }

      bar.Type = type;
      bar.Include = include;
      bar.NumberValue = numberValue;
      bar.Border.AutoFormat = true;
      bar.HasCap = true;

      return bar;
    }
    /// <summary>
    /// Sets custom error bar type.
    /// </summary>
    /// <param name="bIsY">If true - on Y axis; otherwise on X axis.</param>
    /// <param name="plusRange">Represents plus range.</param>
    /// <param name="minusRange">Represents minus range.</param>
    /// <returns>Returns error bar object.</returns>
    public IChartErrorBars ErrorBar( bool bIsY, IRange plusRange, IRange minusRange )
    {
      bool bIsPlus = plusRange != null;
      bool bisMinus = minusRange != null;

      if( !bIsPlus && !bisMinus )
        throw new ArgumentException( "Plus range and minus range are null referance." );

      ChartErrorBarsImpl bar = null;

      if( bIsY )
      {
        HasErrorBarsY = true;
        bar = m_errorBarY;
        bar.NumberValue = ChartErrorBarsImpl.DEF_NUMBER_Y_VALUE;
      }
      else
      {
        HasErrorBarsX = true;
        bar = m_errorBarX;
        bar.NumberValue = ChartErrorBarsImpl.DEF_NUMBER_X_VALUE;
      }

      if( bIsPlus )
        bar.PlusRange = plusRange;

      if( bisMinus )
        bar.MinusRange = minusRange;

      bar.Border.AutoFormat = true;
      bar.HasCap = true;

      return bar;
    }
    #endregion

    #region Parse methods
    /// <summary>
    /// Parses series data.
    /// </summary>
    /// <param name="data">Array of biff records that contains series data.</param>
    /// <param name="iPos">Position of the ChartSeries record in the array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// Specified record is not ChartSeries record
    /// or if next record is not Begin record.
    /// </exception>
    private void Parse( IList<BiffRecordRaw> data, ref int iPos )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      BiffRecordRaw record = data[ iPos ];
      record.CheckTypeCode( TBIFFRecord.ChartSeries );

      ParseSeriesRecord( ( ChartSeriesRecord )data[ iPos ] );
      iPos++;

      record = data[ iPos ];
      if( record.TypeCode != TBIFFRecord.Begin )
        throw new ArgumentOutOfRangeException( "Begin record was expected." );
      
      iPos++;
      
      m_hashAi.Clear();
      m_dataPoints.Clear();

      // TODO: implement
      record = data[ iPos ];

      while( record.TypeCode != TBIFFRecord.End )
      {
        switch( record.TypeCode )
        {
          case TBIFFRecord.ChartAI:
            ParseAIRecord( data, ref iPos );
            break;

          case TBIFFRecord.ChartDataFormat:
            m_iOrder = ( ( ChartDataFormatRecord )record ).SeriesNumber;
            ChartSerieDataFormatImpl dataFormat = new ChartSerieDataFormatImpl( Application, this );
            iPos = dataFormat.Parse( data, iPos );
            int iIndex = dataFormat.DataFormat.PointNumber;

            SetDataFormat( dataFormat );
            break;

          case TBIFFRecord.ChartSertocrt:
            ParseSertoCrt( data, ref iPos );
            break;

          case TBIFFRecord.ChartLegendxn:
            ParseLegendEntries( data, ref iPos );
            break;

          case TBIFFRecord.ChartSeriesText:
            if( m_seriesColl.Count == 0 )
              m_seriesText = ( ( ChartSeriesTextRecord )record ).Text;

            iPos++;
            break;

          default:
            iPos++;
            break;
        }

        record = data[ iPos ];
      }

      m_seriesColl.TrendIndex++;
      // Move after EndRecord
      iPos++;
      Reparse();
      //m_book.AddForReparse( this );
    }
    /// <summary>
    /// Parses ChartSeries record.
    /// </summary>
    /// <param name="series">Record to parse.</param>
    private void ParseSeriesRecord( ChartSeriesRecord series )
    {
      // TODO: implement
      m_series = series;
      //      m_xDataType = series.StdX;
      //      m_yDataType = series.StdY;
      //      m_bubbleDataType = series.StdY;
      //
      //      m_iValuesCount = series.ValuesCount;
      //      m_iCategoriesCount = series.CategoriesCount;
      //      m_iBubbleSeriesCount = series.BubbleSeriesCount;
    }
    /// <summary>
    /// Parses ChartAI record.
    /// </summary>
    /// <param name="data">Array of BiffRecords that contains ChartAi record.</param>
    /// <param name="iPos">Position of the record in the data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When specified record is not ChartAi record.
    /// </exception>
    /// <exception cref="System.ArgumentException">
    /// When ChartAI record with the same IndexIdentifier was
    /// already added to the collection.
    /// </exception>
    private void ParseAIRecord( IList<BiffRecordRaw> data, ref int iPos )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      BiffRecordRaw record = data[ iPos ];

      if( record.TypeCode != TBIFFRecord.ChartAI )
        throw new ArgumentOutOfRangeException( "ChartAI record was expected." );

      ChartAIRecord recordAi = ( ChartAIRecord )record;
      iPos++;
      
      if( m_hashAi.ContainsKey( recordAi.IndexIdentifier ) )
        throw new ArgumentException( "AI record with such IndexIdentifier was already read." );

      if( recordAi.IndexIdentifier == ChartAIRecord.LinkIndex.LinkToTitleOrText )
        GetTitle( recordAi, data, ref iPos );

      m_hashAi.Add( recordAi.IndexIdentifier, recordAi );
    }
    /// <summary>
    /// Parses ChartSertocrt record.
    /// </summary>
    /// <param name="data">Array of BiffRecords that contains necessary record.</param>
    /// <param name="iPos">
    /// Position of the ChartSertocrt record in the data array.
    /// </param>
    private void ParseSertoCrt( IList<BiffRecordRaw> data, ref int iPos )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      BiffRecordRaw record = data[ iPos ];

      record.CheckTypeCode( TBIFFRecord.ChartSertocrt );
      m_iChartGroup = ( ( ChartSertocrtRecord )record ).ChartGroup;
      iPos++;
    }
    /// <summary>
    /// Parses series title.
    /// </summary>
    /// <param name="recordAi">ChartAi record describing chart title.</param>
    /// <param name="data">Array of BiffRecords that contains necessary records.</param>
    /// <param name="iPos">Position of the ChartAi record in the data array.</param>
    private void GetTitle( ChartAIRecord recordAi, IList<BiffRecordRaw> data, ref int iPos )
    {
      if( recordAi.Reference == ReferenceType.EnteredDirectly || 
        recordAi.Reference == ReferenceType.NotUsed )
      {
        BiffRecordRaw record = ( BiffRecordRaw )data[ iPos ];

        if( record.TypeCode == TBIFFRecord.ChartSeriesText )
        {
          m_strName = ( ( ChartSeriesTextRecord )record ).Text;
          m_seriesText = m_strName;
          m_bDefaultName = false;
          iPos++;
        }
      }

      if( recordAi.Reference == ReferenceType.Worksheet )
      {
        Ptg[] expression = recordAi.ParsedExpression;
        m_strName = "=" + m_book.FormulaUtil.ParsePtgArray( expression );
        m_nameTokens = expression;
        m_bDefaultName = false;
      }
    }
    /// <summary>
    /// Parses legend entry from biff stream.
    /// </summary>
    /// <param name="data">Array of BiffRecords that contains ChartAi record.</param>
    /// <param name="iPos">Position of the record in the data array.</param>
    private void ParseLegendEntries( IList<BiffRecordRaw> data, ref int iPos )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      m_chart.HasLegend = true;
      ChartLegendEntriesColl entries = ( ChartLegendEntriesColl )m_chart.Legend.LegendEntries;

      ChartLegendEntryImpl legendEntry = new ChartLegendEntryImpl( Application, entries, 0 );

      legendEntry.Parse( data, ref iPos );

      int iIndex = ( legendEntry.LegendEntityIndex == DEF_FORMAT_ALLPOINTS_INDEX )
        ? m_seriesColl.Count : legendEntry.LegendEntityIndex;

      entries.Add( iIndex, legendEntry );
    }
    /// <summary>
    /// Parses error bars.
    /// </summary>
    /// <param name="data">Represents data holder.</param>
    public void ParseErrorBars( IList<BiffRecordRaw> data )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      ChartErrorBarsImpl bar = new ChartErrorBarsImpl( Application, this, data );

      if( bar.IsY )
      {
        UpdateErrorBar( bar, ref m_errorBarY );
      }
      else
      {
        UpdateErrorBar( bar, ref m_errorBarX );
      }
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// Searches for all needed parent objects.
    /// </summary>
    /// <exception cref="System.ArgumentNullException">
    /// When parents cannot be found.
    /// </exception>
    private void SetParents()
    {
      object parent = FindParent( typeof( WorkbookImpl ) );

      if( parent == null )
        throw new ArgumentNullException( "Can't find parent workbook." );

      m_book = (WorkbookImpl) parent;

      parent = FindParent( typeof( ChartImpl ) );

      if( parent == null )
        throw new ArgumentNullException( "Can't find parent chart." );

      m_chart = (ChartImpl) parent;

      parent = FindParent( typeof( ChartSeriesCollection ) );

      if( parent == null )
        throw new ArgumentNullException( "Can't find parent series collection." );

      m_seriesColl = (ChartSeriesCollection) parent;
    }
    /// <summary>
    /// Initializes m_hashAI member.
    /// </summary>
    private void InitializeHashAIMember()
    {
      ChartAIRecord chartAi = (ChartAIRecord) BiffRecordFactory.GetRecord( TBIFFRecord.ChartAI );
      chartAi.IndexIdentifier = LinkIndex.LinkToTitleOrText;
      chartAi.Reference = ReferenceType.NotUsed;
      m_hashAi.Add( chartAi.IndexIdentifier, chartAi );

      chartAi = ( ChartAIRecord )chartAi.Clone();
      chartAi.IndexIdentifier = LinkIndex.LinkToCategories;
      chartAi.Reference = ReferenceType.DefaultCategories;
      m_hashAi.Add( chartAi.IndexIdentifier, chartAi );

      chartAi = ( ChartAIRecord )chartAi.Clone();
      chartAi.IndexIdentifier = LinkIndex.LinkToValues;
      chartAi.Reference = ReferenceType.NotUsed;
      //chartAi.NumberFormatIndex = 2;
      m_hashAi.Add( chartAi.IndexIdentifier, chartAi );

      chartAi = ( ChartAIRecord )chartAi.Clone();
      chartAi.IndexIdentifier = LinkIndex.LinkToBubbles;
      chartAi.Reference = ReferenceType.NotUsed;
      m_hashAi.Add( chartAi.IndexIdentifier, chartAi );
    }
    /// <summary>
    /// Initializes all internal collections.
    /// </summary>
    private void InitializeCollections()
    {
      InitializeHashAIMember();

      m_series = ( ChartSeriesRecord )
        BiffRecordFactory.GetRecord( TBIFFRecord.ChartSeries );

      ChartSerieDataFormatImpl dataFormat = new ChartSerieDataFormatImpl( Application, this );
      dataFormat.DataFormat.PointNumber = ( ushort )DEF_FORMAT_ALLPOINTS_INDEX;
      SetDataFormat( dataFormat );

      m_trendLines = new ChartTrendLineCollection( Application, this );
    }
    /// <summary>
    /// Sets data format.
    /// </summary>
    /// <param name="dataFormat">Data format to set.</param>
    private void SetDataFormat( ChartSerieDataFormatImpl dataFormat )
    {
      if( dataFormat == null )
        throw new ArgumentNullException( "dataFormat" );

      int iIndex = dataFormat.DataFormat.PointNumber;
      ChartDataPointImpl dataPoint = ( ChartDataPointImpl )DataPoints[ iIndex ];
      dataPoint.InnerDataFormat = dataFormat;
      dataFormat.SetParent( dataPoint );
    }
    /// <summary>
    /// Return Chart3DDataFormat record for this series.
    /// </summary>
    /// <returns>Chart3DDataFormat record for this series.</returns>
    [ CLSCompliant( false ) ]
    public Chart3DDataFormatRecord Get3DDataFormat()
    {
      Chart3DDataFormatRecord result = (Chart3DDataFormatRecord)
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
          result.DataFormatBase = ExcelBaseFormat.Circle;
          result.DataFormatTop = ExcelTopFormat.Straight;
          break;

        case ExcelChartType.Cone_Stacked_100:
        case ExcelChartType.Cone_Bar_Stacked_100:
          result.DataFormatBase = ExcelBaseFormat.Circle;
          result.DataFormatTop = ExcelTopFormat.Trunc;
          break;

        case ExcelChartType.Cone_Clustered:
        case ExcelChartType.Cone_Stacked:
        case ExcelChartType.Cone_Bar_Clustered:
        case ExcelChartType.Cone_Bar_Stacked:
        case ExcelChartType.Cone_Clustered_3D:
          result.DataFormatBase = ExcelBaseFormat.Circle;
          result.DataFormatTop = ExcelTopFormat.Sharp;
          break;

        case ExcelChartType.Pyramid_Stacked_100:
        case ExcelChartType.Pyramid_Bar_Stacked_100:
          result.DataFormatBase = ExcelBaseFormat.Rectangle;
          result.DataFormatTop = ExcelTopFormat.Trunc;
          break;

        case ExcelChartType.Pyramid_Clustered:
        case ExcelChartType.Pyramid_Stacked:
        case ExcelChartType.Pyramid_Bar_Clustered:
        case ExcelChartType.Pyramid_Bar_Stacked:
        case ExcelChartType.Pyramid_Clustered_3D:
          result.DataFormatBase = ExcelBaseFormat.Rectangle;
          result.DataFormatTop = ExcelTopFormat.Sharp;
          break;
      }

      return result;
    }
    /// <summary>
    /// This method is called when the name of the series needs to be changed.
    /// </summary>
    /// <param name="value">New name of the series.</param>
    private void OnNameChanged( string value )
    {
      if( value != null && value.Length > 0 && value[ 0 ] == '=' )
      {
        value = value.Substring( 1 );

        try
        {
          Ptg[] exspression = m_book.FormulaUtil.ParseString( value );
          IRangeGetter getter = ( IRangeGetter )exspression[ 0 ];
          m_nameRange = getter.GetRange( m_book, null );
        }
        catch
        {
          throw new ArgumentException( "Not valid formula string" );
        }

        if( m_nameRange != null && m_nameRange.Row != m_nameRange.LastRow && m_nameRange.Column != m_nameRange.LastColumn )
          throw new NotSupportedException( "Referance must be a single cell, row, or column." );
      }
      else
      {
        m_nameRange = null;
      }
    }
    /// <summary>
    /// Raises ValueRangeChanged event and sets new value to the ValueRange.
    /// </summary>
    private void OnValueRangeChanged( ValueChangedEventArgs e )
    {
      SetAIRange( m_hashAi[ LinkIndex.LinkToValues ],
        m_ValueRange, ReferenceType.EnteredDirectly );

      if( ValueRangeChanged != null )
      {
        ValueRangeChanged( this, e );
      }
    }
    /// <summary>
    /// Sets new value to the CategoryRange.
    /// </summary>
    private void OnCategoryRangeChanged()
    {
      SetAIRange( ( ChartAIRecord )m_hashAi[ LinkIndex.LinkToCategories ]
        , m_CategoryRange, ReferenceType.DefaultCategories );
    }
    /// <summary>
    /// Sets new value to the BubbleRange.
    /// </summary>
    private void OnBubbleRangeChanged()
    {
      SetAIRange( ( ChartAIRecord )m_hashAi[ LinkIndex.LinkToBubbles ]
        , m_BubbleRange, ReferenceType.DefaultCategories );
    }
    /// <summary>
    /// Clone current instance.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    /// <param name="hashNewNames">Dictionary with new worksheet names.</param>
    /// <param name="dicFontIndexes">Dictionary with new font indexes.</param>
    /// <returns>Cloned series.</returns>
    public ChartSerieImpl Clone( object parent, Dictionary<string, string> hashNewNames, Dictionary<int, int> dicFontIndexes )
    {
      ChartSerieImpl result = new ChartSerieImpl( Application, parent );

      result.m_bDefaultName = m_bDefaultName;
      result.m_bIsDisposed = m_bIsDisposed;

      result.m_hashAi = new Dictionary<ChartAIRecord.LinkIndex, ChartAIRecord>();

      result.InitializeHashAIMember();
      IRange range = GetSerieNameRange();

      if( range != null )
      {
        result.m_nameRange = ( ( ICombinedRange )m_nameRange ).Clone( result
          , hashNewNames, result.m_book );
      }

      if( m_ValueRange != null )
      {
        range = ( ( ICombinedRange )m_ValueRange ).Clone( result
          , hashNewNames, result.m_book );
        result.Values = range;
      }

      if( m_BubbleRange != null )
      {
        result.Bubbles = ( ( ICombinedRange )m_BubbleRange ).Clone( result
          , hashNewNames, result.m_book );
      }

      if( m_CategoryRange != null )
      {
        result.CategoryLabels = ( ( ICombinedRange )m_CategoryRange ).Clone( result
          , hashNewNames, result.m_book );
      }

      if( m_nameRange != null )
      {
        result.m_nameRange = ( ( ICombinedRange )m_nameRange ).Clone( result
          , hashNewNames, result.m_book );
      }

      if( m_dataPoints != null )
      {
        result.m_dataPoints = ( ChartDataPointsCollection )
          m_dataPoints.Clone( result, result.m_book, dicFontIndexes, hashNewNames );
      }

      result.m_valueEnteredDirectly = CloneUtils.CloneCloneable( m_valueEnteredDirectly );
      result.m_categoryEnteredDirectly = CloneUtils.CloneCloneable( m_categoryEnteredDirectly );
      result.m_bubbleEnteredDirectly = CloneUtils.CloneCloneable( m_bubbleEnteredDirectly );

      result.m_enteredDirectlyValue = CloneUtils.CloneArray( m_enteredDirectlyValue );
      result.m_enteredDirectlyCategory = CloneUtils.CloneArray( m_enteredDirectlyCategory );
      result.m_enteredDirectlyBubble = CloneUtils.CloneArray( m_enteredDirectlyBubble );

      result.NumRefFormula = m_numRefFormula;

      result.m_iChartGroup = m_iChartGroup;
      result.m_iIndex = m_iIndex;
      result.m_iOrder = m_iOrder;
      result.m_series = ( ChartSeriesRecord )m_series.Clone();
      result.m_strName = m_strName;
      result.m_trendLines = m_trendLines.Clone( result, dicFontIndexes, hashNewNames );

      if( m_errorBarX != null )
        result.m_errorBarX = m_errorBarX.Clone( result, hashNewNames );

      if( m_errorBarY != null )
        result.m_errorBarY = m_errorBarY.Clone( result, hashNewNames );

      return result;
    }
    /// <summary>
    /// Gets worksheet name.
    /// </summary>
    /// <param name="strAddress">Address of Range.</param>
    /// <returns>Returns name of parent worksheet.</returns>
    private string GetWorkSheetNameByAddress( string strAddress )
    {
      if( strAddress == null )
        throw new ArgumentNullException( "strAddress" );

      int iEndSheetName = strAddress.IndexOf( "'!" );

      return strAddress.Substring( 1, iEndSheetName - 1 );
    }
    /// <summary>
    /// Changes axis.
    /// </summary>
    /// <param name="bToPrimary">If true - to primary; otherwise - to secondary.</param>
    private void ChangeAxis( bool bToPrimary )
    {
      if( !bToPrimary && m_seriesColl.Count == 1 )
        throw new ArgumentException( "Can't set current serie to secondary axis" );

      int iOrder = ChartGroup;
      ChartFormatImpl format = GetCommonSerieFormat();
      int newOrder = GetNewOrder( bToPrimary );
      int iOrderCount = m_seriesColl.GetCountOfSeriesWithSameDrawingOrder( iOrder );
      int iStartTypeCount = m_seriesColl.GetCountOfSeriesWithSameStartType( SerieType );

      if( newOrder < 0 )
        throw new ApplicationException( "Can't set current serie to secondary axis" );

      if( iOrderCount != iStartTypeCount )
      {
        ExcelChartType type = SerieType;
        //ChartGroup = newOrder;

        ChartFormatCollection formats = bToPrimary ?
          m_chart.PrimaryFormats :
          m_chart.SecondaryFormats;

        ChartFormatImpl newFormat = ( ChartFormatImpl )format.Clone( formats );
        newFormat = formats.FindOrAdd( newFormat );
        ChartGroup = newFormat.DrawingZOrder;

        if( iOrderCount == 1 )
          m_chart.PrimaryParentAxis.Formats.Remove( format );

        if( Array.IndexOf( ChartImpl.DEF_CHANGE_INTIMATE, type ) != -1 )
        {
          SerieType = type;
        }
        else if( m_chart.ChartType == ExcelChartType.Combination_Chart )
        {
          ChangeSeriesType( type, false, true );
        }
      }
      else
      {
        //if( ( !bToPrimary && m_chart.SecondaryFormats.Count != 0 )
        //  || ( bToPrimary && Array.IndexOf( ChartImpl.DEF_NEED_SECONDARY_AXIS, SerieType ) != -1 ) )
        //{
        //  throw new ApplicationException( "Can't set current serie to secondary axis" );
        //}

        bool bAdd = iOrderCount != 1;

        ChartGlobalFormatsCollection formats = m_chart.PrimaryParentAxis.Formats;
        formats.ChangeShallowAxis( bToPrimary, iOrder, bAdd, newOrder );

        if( bAdd )
          ChartGroup = newOrder;
      }
    }
    /// <summary>
    /// Returns new series order.
    /// </summary>
    /// <param name="bToPrimary">If true - to primary; otherwise - to secondary.</param>
    /// <returns>A new series order.</returns>
    private int GetNewOrder( bool bToPrimary )
    {
      ChartGlobalFormatsCollection formats = m_chart.PrimaryParentAxis.Formats;
      ChartFormatCollection primaryFormats = formats.PrimaryFormats;
      ChartFormatCollection secondaryFormats = formats.SecondaryFormats;
      //ChartFormatCollection formats = ( bToPrimary) ? primaryFormats : secondaryFormats;

      // 1. Check whether same format is already there
      // 2. Get new Z-Order.
      int iResult = -1;

      for( int i = 0; i < ChartFormatCollection.DEF_ARRAY_CAPACITY; i++ )
      {
        if( !primaryFormats.ContainsIndex( i ) && !secondaryFormats.ContainsIndex( i ) )
        {
          iResult = i;
          break;
        }
      }

      return iResult;
    }

    /// <summary>
    /// Add entered record by si index.
    /// </summary>
    /// <param name="siIndex">si index.</param>
    /// <param name="record">Record to add.</param>
    [ CLSCompliant( false ) ]
    public void AddEnteredRecord( int siIndex, ICellPositionFormat record )
    {
      if( siIndex > 3 || siIndex < 1 )
        throw new ArgumentOutOfRangeException( "siIndex" );

      if( record == null )
        throw new ArgumentNullException( "record" );

      switch( siIndex )
      {
        case ChartImpl.DEF_SI_VALUE:
          if( m_ValueRange == null )
            m_valueEnteredDirectly.Add( record as BiffRecordRaw );
          break;

        case ChartImpl.DEF_SI_CATEGORY:
          if( m_CategoryRange == null )
            m_categoryEnteredDirectly.Add( record as BiffRecordRaw );
          break;

        case ChartImpl.DEF_SI_BUBBLE:
          if( m_BubbleRange == null )
            m_bubbleEnteredDirectly.Add( record as BiffRecordRaw );
          break;
      }
    }

    /// <summary>
    /// Gets array by siIndex.
    /// </summary>
    /// <param name="siIndex">Si index.</param>
    /// <returns>Returns array by si index.</returns>
    public List<BiffRecordRaw> GetArray( int siIndex )
    {
      if( siIndex > 3 || siIndex < 1 )
        throw new ArgumentOutOfRangeException( "siIndex" );

      List<BiffRecordRaw> result = null;

      switch( siIndex )
      {
        case ChartImpl.DEF_SI_VALUE:
           result = m_valueEnteredDirectly;
           break;

         case ChartImpl.DEF_SI_CATEGORY:
          result = m_categoryEnteredDirectly;
          break;

        case ChartImpl.DEF_SI_BUBBLE:
          result = m_bubbleEnteredDirectly;
          break;
      }

      return ( result != null && result.Count > 0 ) ? result : null;
    }

    /// <summary>
    /// Gets values of entered directly values.
    /// </summary>
    /// <param name="array">Record storage.</param>
    /// <returns>Returns just created array of values.</returns>
    public object[] GetEnteredDirectlyValues( List<BiffRecordRaw> array )
    {
      if( array == null )
        throw new ArgumentNullException( "array" );

      int count = array.Count;

      if( count == 0 )
        return null;

      List<object> list = new List<object>( count );

      for( int i = 0; i < count; i++ )
      {
        BiffRecordRaw record = array[ i ];
        object o = null;

        switch( record.TypeCode )
        {
          case TBIFFRecord.Number:
            o = ( ( NumberRecord )record ).Value;
            break;

          case TBIFFRecord.Label:
            o = ( ( LabelRecord )record ).Label;
            break;

          default:
            Debug.WriteLine( "Unknown ChartSi record type:" + record.TypeCode );
            break;
        }

        list.Add( o );
      }

      return list.ToArray();
    }
    /// <summary>
    /// Gets type of Entered directly values.
    /// </summary>
    /// <param name="array">Array with entered directly values.</param>
    /// <returns>If true - type is Number; otherwise label.</returns>
    private bool GetEnteredDirectlyType( object[] array )
    {
      if( array == null )
        throw new ArgumentNullException( "array" );

      for( int i = 0, iLen = array.Length; i < iLen; i++ )
      {
        if( array[ i ] is string )
          return false;
      }

      return true;
    }
    /// <summary>
    /// Gets array of records by values.
    /// </summary>
    /// <param name="bIsNumber">If true - values type is number; otherwise - string.</param>
    /// <param name="values">Array list with values.</param>
    /// <returns>Returns created array with records.</returns>
    private List<BiffRecordRaw> GetArrayRecordsByValues( bool bIsNumber, object[] values )
    {
      if( values == null )
        throw new ArgumentNullException( "values" );

      int iLen = values.Length;
      List<BiffRecordRaw> result = new List<BiffRecordRaw>( iLen );

      for( int i = 0; i < iLen; i++ )
      {
        object o = values[ i ];
        ICellPositionFormat format;

        if( o == null )
          throw new ApplicationException( "Null referance value in values array at " + i.ToString() + " position" );

        if( o is bool && !bIsNumber )
          o = ( ( bool )o ) ? DEF_TRUE : DEF_FALSE;

        if( bIsNumber )
        {
          NumberRecord record = ( NumberRecord )BiffRecordFactory.GetRecord( TBIFFRecord.Number );

#if !(WINRT )
          if( !( o is IConvertible ) )
            throw new ApplicationException( "Bad value in values array at " + i.ToString() + " position" );
#endif
          record.Value = Convert.ToDouble( o );
          format = record;
        }
        else
        {
          LabelRecord record = ( LabelRecord )BiffRecordFactory.GetRecord( TBIFFRecord.Label );

#if !(WINRT )
          if( !( o is IConvertible ) )
            throw new ApplicationException( "Bad value in values array at " + i.ToString() + " position" );
#endif
          record.Label = Convert.ToString( o );
          format = record;
        }

        format.Column = ( ushort )Index;
        format.Row = ( ushort )i;

        result.Add( ( BiffRecordRaw )format );
      }

      return result;
    }
    /// <summary>
    /// Updates in entered directly values Series index.
    /// </summary>
    /// <param name="arrayToUpdate">Array to update.</param>
    private void UpdateSerieIndexesInEnteredDirectlyValues( List<BiffRecordRaw> arrayToUpdate )
    {
      if( arrayToUpdate == null )
        throw new ArgumentNullException( "arrayToUpdate" );

      int index = Index;

      for( int i = 0, iLen = arrayToUpdate.Count; i <iLen; i++ )
      {
        ICellPositionFormat format = ( ICellPositionFormat )arrayToUpdate[ i ];
        format.Column = ( ushort )index;
      }
    }
    /// <summary>
    /// Detects name.
    /// </summary>
    /// <returns>Returns detected Series name.</returns>
    private string DetectSerieName()
    {
      if( m_strName == null || m_strName.Length == 0 || m_strName[ 0 ] != '=' )
        return m_strName;

      string result = null;

      if( m_nameRange != null )
      {
        result = GetTextRangeValue( m_nameRange );
      }
      else
      {
        string value = m_strName.Substring( 1 );

        try
        {
          //Ptg[] exspression = m_book.FormulaUtil.ParseString( value );
          IRangeGetter getter = ( IRangeGetter )m_nameTokens[ 0 ];//exspression[ 0 ];
          m_nameRange = getter.GetRange( m_book, null );

          if( m_nameRange == null )
          {
            result = RefErrorPtg.ReferenceError;
          }
          else
          {
            result = GetTextRangeValue( m_nameRange );
          }
        }
        catch
        {
          result = m_strName = RefErrorPtg.ReferenceError;
        }
      }

      return result;
    }
    /// <summary>
    /// Gets text range value for Series name that supports cell, or row, or column.
    /// </summary>
    /// <param name="range">Range to gets text value.</param>
    /// <returns>Value representing text range.</returns>
    private string GetTextRangeValue( IRange range )
    {
      if( range == null )
        throw new ArgumentNullException( "range" );

      string result = "";
      int firstColumn = range.Column;
      int lastColumn = range.LastColumn;
      int firstRow = range.Row;
      int lastRow = range.LastRow;

      if ((range.Worksheet as WorksheetImpl) != null)
      {
          IMigrantRange migrantRange = range.Worksheet.MigrantRange;

          for (int i = firstColumn, iLen = lastColumn; i <= iLen; i++)
          {
              for (int j = firstRow, iCount = lastRow; j <= iCount; j++)
              {
                  migrantRange.ResetRowColumn(j, i);
                  string strValue = migrantRange.Value;

                  if (strValue != null && strValue.Length > 0)
                      result += strValue + " ";
              }
          }
      }
      else
      {
          for (int i = firstColumn, iLen = lastColumn; i <= iLen; i++)
          {
              for (int j = firstRow, iCount = lastRow; j <= iCount; j++)
              {
                  string strValue = range[j, i].Value;

                  if (strValue != null && strValue.Length > 0)
                      result += strValue + " ";
              }
          }
      }

      return ( result == "" ) ? "" : result.Substring( 0, result.Length - 1 );
    }
    /// <summary>
    /// Sets default Series name.
    /// </summary>
    /// <param name="strName">Represents Series name.</param>
    public void SetDefaultName( string strName )
    {
      if( strName == null || strName.Length == 0 )
        throw new ArgumentNullException( "strName" );

      m_strName = strName;
      m_bDefaultName = true;
    }
    /// <summary>
    /// Gets Series name range.
    /// </summary>
    /// <returns>Returns range, that represents Series name range. Can be null.</returns>
    public IRange GetSerieNameRange()
    {
      string strName = Name;

      return m_nameRange;
    }

    #region Helper methods for detect serie type
    /// <summary>
    /// Detects Series type.
    /// </summary>
    /// <returns>Extracted series type.</returns>
    public ExcelChartType DetectSerieType()
    {
      string strTypeName = DetectSerieTypeString();

      return ( ExcelChartType )Enum.Parse( typeof( ExcelChartType ), strTypeName, true );
    }
    /// <summary>
    /// Detects Series type.
    /// </summary>
    /// <returns>Detected string representationg of the series type.</returns>
    public string DetectSerieTypeString()
    {
      ChartFormatCollection primaryFormats = m_chart.PrimaryFormats;

      if( m_chart.Loading && primaryFormats.Count == 0 )
        return "Column_Clustered";

      ChartFormatImpl format = GetCommonSerieFormat();

      switch ( format.FormatRecordType )
      {
        case TBIFFRecord.ChartBar:
          return DetectBarSerie( format );

        case TBIFFRecord.ChartLine:
          return DetectLineSerie( format );

        case TBIFFRecord.ChartPie:
          return DetectPieSerie( format );

        case TBIFFRecord.ChartArea:
          return DetectAreaSerie( format );

        case TBIFFRecord.ChartScatter:
          return DetectScatterSerie( format );

        case TBIFFRecord.ChartSurface:
          return DetectSurfaceSerie( format );

        case TBIFFRecord.ChartRadar:
          return DetectRadarSerie( format );

        case TBIFFRecord.ChartRadarArea:
          return "Radar_Filled";

        case TBIFFRecord.ChartBoppop:
          return DetectBoppopSerie( format );

        default:
          throw new ArgumentOutOfRangeException( "Can't detect serie type." );
      }
    }
    /// <summary>
    /// Detects Series type.
    /// </summary>
    /// <returns>Extracted series type.</returns>
    public string DetectSerieTypeStart()
    {
      ChartFormatCollection primaryFormats = m_chart.PrimaryFormats;

      if( m_chart.Loading && primaryFormats.Count == 0 )
        return ChartImpl.START_COLUMN;

      ChartFormatImpl format = GetCommonSerieFormat();

      switch( format.FormatRecordType )
      {
        case TBIFFRecord.ChartBar:
          return GetBarStartString( format );

        case TBIFFRecord.ChartLine:
          return ChartImpl.START_LINE;

        case TBIFFRecord.ChartPie:
          return ( format.DoughnutHoleSize != 0 )
            ? ChartImpl.START_DOUGHNUT
            : ChartImpl.START_PIE;

        case TBIFFRecord.ChartArea:
          return ChartImpl.START_AREA;

        case TBIFFRecord.ChartScatter:
          return ( format.DataFormatOrNull != null && format.DataFormatOrNull.SerieFormatOrNull != null &&
            format.DataFormatOrNull.Is3DBubbles || format.IsBubbles ) ?
              ChartImpl.START_BUBBLE :
              ChartImpl.START_SCATTER;

        case TBIFFRecord.ChartSurface:
          return ChartImpl.START_SURFACE;

        case TBIFFRecord.ChartRadar:
        case TBIFFRecord.ChartRadarArea:
          return ChartImpl.START_RADAR;

        case TBIFFRecord.ChartBoppop:
          return ChartImpl.START_PIE;

        default:
          throw new ArgumentOutOfRangeException( "Can't detect serie type." );
      }
    }

    /// <summary>
    /// Detects the type of the series when describing record is ChartBar.
    /// </summary>
    /// <param name="format">Series format.</param>
    /// <returns>Returns series type.</returns>
    private string DetectBarSerie( ChartFormatImpl format )
    {
      if( format == null )
        throw new ArgumentNullException( "format" );

      if( format.FormatRecordType != TBIFFRecord.ChartBar )
        throw new ArgumentException( "format" );

      string strSerieType = string.Empty;

      strSerieType = GetBarStartString( format );

      if( format.StackValuesBar )
      {
        strSerieType += ChartImpl.PREFIX_STACKED;
      }
      else
      {
        if( strSerieType == ChartImpl.START_COLUMN && format.Is3D
          && !format.RightAngleAxes && !format.IsClustered )
        {
          return "Column_3D";
        }
        else
        {
          strSerieType += ChartImpl.PREFIX_CLUSTERED;
        }
      }

      if( format.ShowAsPercentsBar ) strSerieType += ChartImpl.PREFIX_SHOW_PERCENT;

      bool flag = ( strSerieType.IndexOf( ChartImpl.START_CONE ) != -1 )
        || ( strSerieType.IndexOf( ChartImpl.START_CYLINDER ) != -1 )
        || ( strSerieType.IndexOf( "Pyramid" ) != -1 );

      if( format.Is3D && !flag )
        strSerieType += ChartImpl.PREFIX_3D;

      if( flag && !format.IsClustered && !format.StackValuesBar )
      {
        strSerieType += ChartImpl.PREFIX_3D;
      }

      return strSerieType;//( ExcelChartType )Enum.Parse( typeof( ExcelChartType ), strSerieType ); 
    }

    /// <summary>
    /// Gets bar start string.
    /// </summary>
    /// <param name="format">Format for detect start string.</param>
    /// <returns>Returns start string.</returns>
    private string  GetBarStartString( ChartFormatImpl format )
    {
      if( format == null )
        throw new ArgumentNullException( "format" );

      if( format.FormatRecordType != TBIFFRecord.ChartBar )
        throw new ArgumentException( "format" );

      string result = null;

      if( format.DataFormatOrNull != null
        && format.DataFormatOrNull.Serie3DdDataFormatOrNull != null )
      {
        ChartSerieDataFormatImpl dataFormat = format.DataFormatOrNull;

        if( dataFormat.BarShapeBase == ExcelBaseFormat.Circle )
        {
          result = ( dataFormat.BarShapeTop == ExcelTopFormat.Straight )
            ? ChartImpl.START_CYLINDER : ChartImpl.START_CONE;
 
          if( format.IsHorizontalBar )
            result += ChartImpl.PREFIX_BAR;
        }
        else
        {
          if( dataFormat.BarShapeTop == ExcelTopFormat.Straight )
          {
            result =  format.IsHorizontalBar ? ChartImpl.START_BAR : ChartImpl.START_COLUMN;
          }
          else
          {
            result = ChartImpl.START_PYRAMID;

            if( format.IsHorizontalBar )
              result += ChartImpl.PREFIX_BAR;
          }
        }
      }
      else
      {
        result = format.IsHorizontalBar ? ChartImpl.START_BAR : ChartImpl.START_COLUMN;
      }

      return result;
    }
    /// <summary>
    /// Detects the type of the series when describing record is ChartPie.
    /// </summary>
    /// <param name="format">Series format.</param>
    /// <returns>Returns series type.</returns>
    private string DetectPieSerie( ChartFormatImpl format )
    {
      if( format == null )
        throw new ArgumentNullException( "format" );

      if( format.FormatRecordType != TBIFFRecord.ChartPie )
        throw new ArgumentException( "format" );

      string strSerieType = ( format.DoughnutHoleSize != 0 )
        ? ChartImpl.START_DOUGHNUT
        : ChartImpl.START_PIE;

      if( format.DataFormatOrNull != null )
      {
        ChartSerieDataFormatImpl dataFormat = format.DataFormatOrNull;

        if( dataFormat.PieFormatOrNull != null && dataFormat.Percent > 0 )
          strSerieType += ChartImpl.PREFIX_EXPLODED;
      }

      if( format.Is3D == true ) strSerieType += ChartImpl.PREFIX_3D;

      return strSerieType;//( ExcelChartType )Enum.Parse( typeof( ExcelChartType ), strSerieType );
    }

    /// <summary>
    /// Detects the type of the series when describing record is ChartArea.
    /// </summary>
    /// <param name="format">Series format.</param>
    /// <returns>Returns series type.</returns>
    private string DetectAreaSerie( ChartFormatImpl format )
    {
      if( format == null )
        throw new ArgumentNullException( "format" );

      if( format.FormatRecordType != TBIFFRecord.ChartArea )
        throw new ArgumentException( "format" );

      if( format.Is3D == true && !format.RightAngleAxes && !format.IsStacked )
        return "Area_3D";

      string strSerieType = ChartImpl.START_AREA;
      
      if( format.IsStacked ) strSerieType += ChartImpl.PREFIX_STACKED;
      if( format.IsCategoryBrokenDown ) strSerieType += ChartImpl.PREFIX_SHOW_PERCENT;
      if( format.Is3D == true ) strSerieType += ChartImpl.PREFIX_3D;

      return strSerieType;//( ExcelChartType )Enum.Parse( typeof( ExcelChartType ), strSerieType );
    }
    /// <summary>
    /// Detects the type of the series when describing record is ChartSurface.
    /// </summary>
    /// <param name="format">Series format.</param>
    /// <returns>Returns series type.</returns>
    private string DetectSurfaceSerie( ChartFormatImpl format )
    {
      if( format == null )
        throw new ArgumentNullException( "format" );

      if( format.FormatRecordType != TBIFFRecord.ChartSurface )
        throw new ArgumentException( "format" );

      string strSerieType = ChartImpl.START_SURFACE;
      
      if( !format.IsFillSurface ) strSerieType += ChartImpl.PREFIX_NOCOLOR;

      if ( format.Rotation == 0 && format.Elevation == 90
        && format.Perspective == 0 )
      {
        strSerieType += ChartImpl.PREFIX_CONTOUR;
      }
      else
      {
        strSerieType += ChartImpl.PREFIX_3D;
      }

      return strSerieType;//( ExcelChartType )Enum.Parse( typeof( ExcelChartType ), strSerieType );
    }
    /// <summary>
    /// Detects the type of the series when describing record is ChartBoppop.
    /// </summary>
    /// <param name="format">Series format.</param>
    /// <returns>Returns series type.</returns>
    private string DetectBoppopSerie( ChartFormatImpl format )
    {
      if( format == null )
        throw new ArgumentNullException( "format" );

      if( format.FormatRecordType != TBIFFRecord.ChartBoppop )
        throw new ArgumentException( "format" );

      switch( format.PieChartType )
      {
        case ExcelPieType.Normal:
          return "Pie";

        case ExcelPieType.Bar:
          return "Pie_Bar";

        case ExcelPieType.Pie:
          return "PieOfPie";

        default:
          throw new ApplicationException( "Can't detect boppop serie type." );
      }
    }
    /// <summary>
    /// Detects the type of the series when describing record is ChartRadar.
    /// </summary>
    /// <param name="format">Series format.</param>
    /// <returns>Returns series type.</returns>
    private string DetectRadarSerie( ChartFormatImpl format )
    {
      if( format == null )
        throw new ArgumentNullException( "format" );

      if( format.FormatRecordType != TBIFFRecord.ChartRadar )
        throw new ArgumentException( "format" );

      string strSerieType = ChartImpl.START_RADAR;

      if( format.IsMarker )
        strSerieType += ChartImpl.PREFIX_MARKERS;

      ChartSerieDataFormatImpl dataFormat = ( ChartSerieDataFormatImpl )
        ( ( ChartDataPointsCollection )DataPoints ).DefPointFormatOrNull;

      if( dataFormat != null && dataFormat.ContainsLineProperties )
      {
        strSerieType = ChartImpl.START_RADAR;

        if( dataFormat.IsMarker )
          strSerieType += ChartImpl.PREFIX_MARKERS;
      }

      return strSerieType;//( ExcelChartType )Enum.Parse( typeof( ExcelChartType ), strSerieType );
    }
    /// <summary>
    /// Detects the type of the series when describing record is Chartline.
    /// </summary>
    /// <param name="format">Series format.</param>
    /// <returns>Returns series type.</returns>
    private string DetectLineSerie( ChartFormatImpl format )
    {
      if( format == null )
        throw new ArgumentNullException( "format" );

      if( format.FormatRecordType != TBIFFRecord.ChartLine )
        throw new ArgumentException( "format" );

      if( format.Is3D == true )
        return "Line_3D";

      bool bWasDataPoint = false;

      ChartSerieDataFormatImpl dataFormat = ( ChartSerieDataFormatImpl )
        ( ( ChartDataPointsCollection )DataPoints ).DefPointFormatOrNull;

      if( dataFormat != null && dataFormat.ContainsLineProperties )
        bWasDataPoint = true;

      string strChartType = ChartImpl.START_LINE;

      if( ( !bWasDataPoint && format.IsMarker ) || ( bWasDataPoint && dataFormat.IsMarker ) )
        strChartType += ChartImpl.PREFIX_MARKERS;

      if( format.StackValuesLine )    strChartType += ChartImpl.PREFIX_STACKED;
      if( format.ShowAsPercentsLine ) strChartType += ChartImpl.PREFIX_SHOW_PERCENT;

      return strChartType;//( ExcelChartType )Enum.Parse( typeof( ExcelChartType ), strChartType );
    }

    /// <summary>
    /// Detects the type of the series when describing record is ChartScatter.
    /// </summary>
    /// <param name="format">Series format.</param>
    /// <returns>Returns series type.</returns>
    private string DetectScatterSerie( ChartFormatImpl format )
    {
      if( format == null )
        throw new ArgumentNullException( "format" );

      if( format.FormatRecordType != TBIFFRecord.ChartScatter )
        throw new ArgumentException( "format" );

      if( format.DataFormatOrNull != null )
      {
        ChartSerieDataFormatImpl primaryDataFormat = format.DataFormatOrNull;

        if( primaryDataFormat.SerieFormatOrNull != null && primaryDataFormat.Is3DBubbles )
          return "Bubble_3D";
      }

      if( format.IsBubbles )
        return "Bubble";

      ChartSerieDataFormatImpl dataFormat = ( ChartSerieDataFormatImpl )
        ( ( ChartDataPointsCollection )DataPoints ).DefPointFormatOrNull;

      bool bWasDataPoint = dataFormat.ContainsLineProperties;

      string strSerieType = ChartImpl.START_SCATTER;

      if( ( bWasDataPoint && dataFormat.IsSmoothed ) || ( !bWasDataPoint && format.IsSmoothed ) )
      {
        strSerieType += ChartImpl.PREFIX_SMOOTHEDLINE;
      }
      else
      {
        if( ( bWasDataPoint && dataFormat.IsLine ) || ( !bWasDataPoint && format.IsLine ) )
        {
          strSerieType += ChartImpl.PREFIX_LINE;
        }
      }

      if( ( bWasDataPoint && dataFormat.IsMarker ) || ( !bWasDataPoint && format.IsMarker ) )
      {
        strSerieType += ChartImpl.PREFIX_MARKERS;
      }

      if( strSerieType == ChartImpl.START_SCATTER )
      {
        strSerieType = ExcelChartType.Scatter_Line.ToString();
      }

      return strSerieType;//( ExcelChartType )Enum.Parse( typeof( ExcelChartType ), strSerieType );
    }
    #endregion

    #region Helper methods for change serie type
    /// <summary>
    /// This method changes series type.
    /// </summary>
    /// <param name="seriesType">Series type to change to.</param>
    /// <param name="isSeriesCreation">Indicates whether this change happens because of series creation process.</param>
    internal void ChangeSeriesType( ExcelChartType seriesType, bool isSeriesCreation )
    {
      ChangeSeriesType( seriesType, isSeriesCreation, false );
    }
    /// <summary>
    /// This method changes series type.
    /// </summary>
    /// <param name="seriesType">Series type to change to.</param>
    /// <param name="isSeriesCreation">Indicates whether this change happens because of series creation process.</param>
    internal void ChangeSeriesType( ExcelChartType seriesType, bool isSeriesCreation, bool forceChange )
    {
      if( m_book.Loading || SerieType != seriesType || forceChange )
      {
        m_chart.TypeChanging = true;

        OnSerieTypeChange( seriesType, isSeriesCreation );
        m_serieType = seriesType;

        m_chart.TypeChanging = false;
      }
    }
    /// <summary>
    /// Calls before series type changed.
    /// </summary>
    /// <param name="type">Type to change.</param>
    /// <param name="isSeriesCreation">Indicates whether we are in the process of series creation.</param>
    private void OnSerieTypeChange( ExcelChartType type, bool isSeriesCreation )
    {
      if( isSeriesCreation || type != SerieType )
        m_dataPoints.Clear();

      string startType = ChartFormatImpl.GetStartSerieType( type );

      if( !ChartImpl.CheckDataTablePossibility( startType, false ) )
        m_chart.HasDataTable = false;

      HasErrorBarsX = false;
      HasErrorBarsY = false;
      m_trendLines.Clear();

      ExcelChartType chartType = m_chart.ChartType;

      if( Array.IndexOf( ChartImpl.DEF_COMBINATION_CHART, chartType ) != -1 )
      {
        ChangeCombinationType( type );
        return;
      }

      if( Array.IndexOf( ChartImpl.DEF_CHANGE_SERIE, chartType ) == -1
        || m_chart.Series.Count == 1 )
      {
        //m_chart.ChartType = type;
        m_chart.ChangeChartType( type, isSeriesCreation );
        return;
      }

      if( Array.IndexOf( ChartImpl.DEF_CHANGE_SERIE, type ) == -1 )
        throw new ArgumentException( "Cannot change serie type." );

      ChartFormatImpl format = GetCommonSerieFormat();

      if( ChangeIntimateType( format, type ) )
        return;

      if( type == ExcelChartType.Bubble || type == ExcelChartType.Bubble_3D
        || chartType == ExcelChartType.Bubble_3D || chartType == ExcelChartType.Bubble )
      {
        throw new ArgumentException( "Cannot change serie type." );
      }

      if( Array.IndexOf( ChartImpl.DEF_NOT_SUPPORT_GRIDLINES, type ) != -1 )
        m_chart.PrimaryParentAxis.ClearGridLines();

      ChangeNotIntimateType( type );
    }
    /// <summary>
    /// Changes not intimate type.
    /// </summary>
    /// <param name="type">Type to change.</param>
    private void ChangeNotIntimateType( ExcelChartType type )
    {
      m_chart.IsManuallyFormatted = true;

      ChartFormatImpl format = m_chart.PrimaryParentAxis.Formats.ChangeNotIntimateSerieType( type
        , SerieType, Application, m_chart, this );

      ChartGroup = format.DrawingZOrder;
    }
    /// <summary>
    /// Checks is types are intimate. If intimate then change chart type.
    /// </summary>
    /// <param name="format">Format to change.</param>
    /// <param name="TypeToChange">Type to change.</param>
    /// <returns>Returns true if changed type. otherwise false.</returns>
    private bool ChangeIntimateType( ChartFormatImpl format, ExcelChartType TypeToChange )
    {
      if( format == null )
        throw new ArgumentNullException( "format" );

      string strSerieType = ChartFormatImpl.GetStartSerieType( SerieType );
      string strTypeToChange = ChartFormatImpl.GetStartSerieType( TypeToChange );

      if( strSerieType != strTypeToChange )
        return false;

      strTypeToChange = TypeToChange.ToString();

      ChartDataPointImpl dataPoint = ( ChartDataPointImpl )m_dataPoints.DefaultDataPoint;
      ChartSerieDataFormatImpl dataFormat = ( ChartSerieDataFormatImpl )dataPoint.DataFormatOrNull;

      if( strSerieType == ChartImpl.START_RADAR && SerieType != ExcelChartType.Radar_Filled 
        && TypeToChange != ExcelChartType.Radar_Filled )
      {
        dataFormat.ChangeRadarDataFormat( TypeToChange );
        return true;
      }

      switch( strSerieType )
      {
        case ChartImpl.START_SCATTER:
          dataFormat.ChangeScatterDataFormat( TypeToChange );
          return true;

        case ChartImpl.START_BUBBLE:
          dataPoint.ChangeIntimateBuble( TypeToChange );
          return true;

        case ChartImpl.START_LINE:
          ChangeIntimateLine( format, TypeToChange, strTypeToChange );
          return true;
      }

      if( SerieType != TypeToChange )
        format.ChangeSerieType( TypeToChange, false );

      return true;
    }
    /// <summary>
    /// Changes intimate line series.
    /// </summary>
    /// <param name="format">Format to change.</param>
    /// <param name="TypeToChange">Type to change.</param>
    /// <param name="strTypeToChange">TypeToChange string representation.</param>
    private void ChangeIntimateLine( ChartFormatImpl format, ExcelChartType TypeToChange
      , string strTypeToChange )
    {
      bool bIsContainMarketInSerieType =
        strTypeToChange.IndexOf( ChartImpl.PREFIX_MARKERS ) != -1;

      if( format.IsMarker == bIsContainMarketInSerieType )
      {
        format.ChangeSerieType( TypeToChange, false );

        return;
      }

      Dictionary<ExcelChartType, ExcelChartType> hashToChange = new Dictionary<ExcelChartType, ExcelChartType>( 7 );
      InitalizeChangeLineTypeHash( hashToChange );

      format.ChangeSerieType( hashToChange[ TypeToChange ], false );

      ChartDataPointImpl dataPoint = ( ChartDataPointImpl )DataPoints.DefaultDataPoint;

      ( ( ChartSerieDataFormatImpl )dataPoint.DataFormat ).ChangeLineDataFormat( TypeToChange );
    }
    /// <summary>
    /// Initialize hash table for change intimate line type.
    /// </summary>
    /// <param name="hashToInit">HashTable to initialize.</param>
    private void InitalizeChangeLineTypeHash( Dictionary<ExcelChartType, ExcelChartType> hashToInit )
    {
      if( hashToInit == null )
        throw new ArgumentNullException( "hashToInit" );

      hashToInit.Add( ExcelChartType.Line, ExcelChartType.Line_Markers );
      hashToInit.Add( ExcelChartType.Line_Stacked, ExcelChartType.Line_Markers_Stacked );
      hashToInit.Add( ExcelChartType.Line_Stacked_100, ExcelChartType.Line_Markers_Stacked_100 );
      hashToInit.Add( ExcelChartType.Line_Markers_Stacked, ExcelChartType.Line_Stacked );
      hashToInit.Add( ExcelChartType.Line_Markers_Stacked_100, ExcelChartType.Line_Stacked_100 );
      hashToInit.Add( ExcelChartType.Line_Markers, ExcelChartType.Line );
    }
    /// <summary>
    /// Changes series in combination charts.
    /// </summary>
    /// <param name="type">Type to change.</param>
    private void ChangeCombinationType( ExcelChartType type )
    {
      ChartFormatImpl format = FindIntimateFormatByType( type, UsePrimaryAxis, true );
      ChartGlobalFormatsCollection globalFormats = m_chart.PrimaryParentAxis.Formats;
      ChartFormatImpl oldFormat = null;// = GetCommonSerieFormat();
      //m_chart.GlobalFormats
      int iOldOrder = ChartGroup;//oldFormat.DrawingZOrder;

      if( globalFormats.PrimaryFormats.ContainsIndex( iOldOrder ) || globalFormats.SecondaryFormats.ContainsIndex( iOldOrder ) )
        oldFormat = GetCommonSerieFormat();

      if( format != null )
      {
        ChartGroup = format.DrawingZOrder;

        if( m_seriesColl.GetCountOfSeriesWithSameDrawingOrder( iOldOrder ) == 0 )
        {
          if( oldFormat != null )
            m_chart.PrimaryParentAxis.Formats.Remove( oldFormat );
        }

        ChangeIntimateType( format, type );

        if( m_seriesColl.GetCountOfSeriesWithSameType( type, UsePrimaryAxis ) == m_seriesColl.Count )
        {
          m_chart.ChartType = SerieType;
        }

        return;
      }

      bool bNeedDelete = m_seriesColl.GetCountOfSeriesWithSameDrawingOrder( iOldOrder ) == 1;
      bool bNeedSecondartyAxis = Array.IndexOf( ChartImpl.DEF_NEED_SECONDARY_AXIS, type ) != -1;
      int iSecondaryAxisCount = m_chart.SecondaryFormats.Count;

      if( bNeedSecondartyAxis && iSecondaryAxisCount != 0 )
        throw new ArgumentException( "cannot change serie type." );

      ChartFormatCollection formats;

      if( bNeedSecondartyAxis )
      {
        formats = m_chart.SecondaryFormats;

        m_chart.SecondaryParentAxis.UpdateSecondaryAxis( true );
      }
      else
      {
        formats = m_chart.PrimaryFormats;
      }

      format = new ChartFormatImpl( Application, formats );

      if( bNeedDelete )
      {
        ChartGroup = ( ChartGroup == 0 ) ? 1 : 0;

        m_chart.PrimaryParentAxis.Formats.Remove( oldFormat );
      }

      format.ChangeSerieType( type, false );
      format.DrawingZOrder = m_seriesColl.FindOrderByType( type );
      formats.Add( format, false );

      ChartGroup = format.DrawingZOrder;
    }
    /// <summary>
    /// Finds intimate format by type.
    /// </summary>
    /// <param name="type">Type to find.</param>
    /// <returns>Returns found format or null.</returns>
    public ChartFormatImpl FindIntimateFormatByType( ExcelChartType type, bool bPrimaryAxis, bool bPreferSameAxis )
    {
      string strStartType = ChartFormatImpl.GetStartSerieType( type );
      List<int> list = new List<int>( 6 );
      ChartFormatImpl result = null;

      for( int i = 0, iLen = m_seriesColl.Count; i < iLen; i++ )
      {
        ChartSerieImpl serie = ( ChartSerieImpl )m_seriesColl[ i ];

        int iGroup = serie.ChartGroup;

        if( !list.Contains( iGroup ) )
        {
          if( strStartType == ChartFormatImpl.GetStartSerieType( serie.SerieType ) )
          {
              if (strStartType != DEF_RADAR_START_TYPE)
                  result = serie.GetCommonSerieFormat();

            if( bPreferSameAxis && bPrimaryAxis == serie.UsePrimaryAxis || !bPreferSameAxis )
            {
              break;
            }
          }

          list.Add( iGroup );
        }
      }

      return result;
    }
    #endregion

    /// <summary>
    /// Updates formulas after copy operation.
    /// </summary>
    /// <param name="iCurIndex">Current worksheet index.</param>
    /// <param name="iSourceIndex">Source worksheet index.</param>
    /// <param name="sourceRect">Source rectangle.</param>
    /// <param name="iDestIndex">Destination worksheet index.</param>
    /// <param name="destRect">Destination rectangle.</param>
    public void UpdateFormula( int iCurIndex, int iSourceIndex,
      Rectangle sourceRect, int iDestIndex, Rectangle destRect )
    {
      ChartAIRecord chartAI;

      if( m_ValueRange != null )
      {
        chartAI = ( ChartAIRecord )m_hashAi[ LinkIndex.LinkToValues ];
        Values = UpdateRange( chartAI, iCurIndex, iSourceIndex, sourceRect, iDestIndex, destRect );
      }

      if( m_CategoryRange != null )
      {
        chartAI = ( ChartAIRecord )m_hashAi[ LinkIndex.LinkToCategories ];
        CategoryLabels = UpdateRange( chartAI, iCurIndex, iSourceIndex, sourceRect, iDestIndex, destRect );
      }

      if( m_BubbleRange != null )
      {
        chartAI = ( ChartAIRecord )m_hashAi[ LinkIndex.LinkToBubbles ];
        Bubbles = UpdateRange( chartAI, iCurIndex, iSourceIndex, sourceRect, iDestIndex, destRect );
      }
    }
    /// <summary>
    /// Updates formulas after copy operation.
    /// </summary>
    /// <param name="chartAI">Record with range info to update.</param>
    /// <param name="iCurIndex">Current worksheet index.</param>
    /// <param name="iSourceIndex">Source worksheet index.</param>
    /// <param name="sourceRect">Source rectangle.</param>
    /// <param name="iDestIndex">Destination worksheet index.</param>
    /// <param name="destRect">Destination rectangle.</param>
    /// <returns>Updated range.</returns>
    private IRange UpdateRange( ChartAIRecord chartAI, int iCurIndex, int iSourceIndex,
      Rectangle sourceRect, int iDestIndex, Rectangle destRect )
    {
      if( chartAI == null ) return null;

      Ptg[] arrExpression = chartAI.ParsedExpression;
      List<Ptg> arrNewExpression = new List<Ptg>();

      for( int i = 0, len = arrExpression.Length; i < len; i++ )
      {
        Ptg token = arrExpression[ i ];
        Ptg[] arrNewTokens = UpdateToken( token, iCurIndex, iSourceIndex,
          sourceRect, iDestIndex, destRect );
        arrNewExpression.AddRange( arrNewTokens );
      }

      // Here we have to update range. First of all we have to find out whether
      //throw new NotImplementedException();
      chartAI.ParsedExpression = arrNewExpression.ToArray();
      return GetRange( chartAI );
    }
    /// <summary>
    /// Updates formulas after copy operation.
    /// </summary>
    /// <param name="token">Formula token to update.</param>
    /// <param name="iCurIndex">Current worksheet index.</param>
    /// <param name="iSourceIndex">Source worksheet index.</param>
    /// <param name="sourceRect">Source rectangle.</param>
    /// <param name="iDestIndex">Destination worksheet index.</param>
    /// <param name="destRect">Destination rectangle.</param>
    /// <returns>Updated token.</returns>
    private Ptg[] UpdateToken( Ptg token, int iCurIndex, int iSourceIndex,
      Rectangle sourceRect, int iDestIndex, Rectangle destRect )
    {
      if( token == null )
        throw new ArgumentNullException( "token" );

      IReference reference = token as IReference;

      if( reference == null )
        return new Ptg[] { token };
      int iRefIndex = reference.RefIndex;

      if( iRefIndex != iSourceIndex && iRefIndex == iDestIndex )
        return new Ptg[]{ token };

      IRectGetter rangeGetter = token as IRectGetter;

      if( rangeGetter == null ) return new Ptg[]{ token };

      Rectangle rectRange = rangeGetter.GetRectangle();
      bool bChanged;

      if( iRefIndex == iSourceIndex )
      {
        if( sourceRect.Contains( rectRange ) )
        {
          return new Ptg[]{ token.Offset( iCurIndex, -1, -1, iSourceIndex, sourceRect,
                            iDestIndex, destRect, out bChanged, m_book ) };
        }

        if( iSourceIndex == iDestIndex && UtilityMethods.Intersects( sourceRect, rectRange ) )
        {
          // Part of the range was moved.
          Ptg result = PartialTokenMove( token, iSourceIndex, rectRange, sourceRect, destRect );
          return new Ptg[]{ result };
        }
      }

      if( iRefIndex == iDestIndex )
      {
        if( destRect.Contains( rectRange ) )
        {
          // Unfortunately we have to remove this token.
          return new Ptg[]{};
        }

        if( UtilityMethods.Intersects( destRect, rectRange ) )
        {
          // Token was partially overlapped - we have to remove part of the token.
          Ptg result = PartialRemove( token, iRefIndex, rectRange, sourceRect, destRect );
          return new Ptg[]{ result };
        }
      }

      return new Ptg[]{ token };
    }
    /// <summary>
    /// Updates formula token when only part of it was moved.
    /// </summary>
    /// <param name="token">Original token.</param>
    /// <param name="iSourceIndex">Source worksheet index.</param>
    /// <param name="rectRange">Rectangle with token range.</param>
    /// <param name="sourceRect">Source rectangle.</param>
    /// <param name="destRect">Destination rectangle.</param>
    /// <returns>Token after move operation.</returns>
    private Ptg PartialTokenMove( Ptg token, int iSourceIndex, Rectangle rectRange,
      Rectangle sourceRect, Rectangle destRect )
    {
      bool bHeadMoved = UtilityMethods.Contains( sourceRect, rectRange.Left, rectRange.Top );
      bool bTailMoved = UtilityMethods.Contains( sourceRect, rectRange.Right, rectRange.Bottom );

      if( !bHeadMoved && !bTailMoved ) return token;

      int iDeltaCol = destRect.Left - sourceRect.Left;
      int iDeltaRow = destRect.Top - sourceRect.Top;

      if( rectRange.Height == 0 && iDeltaRow == 0 )
      {
        int iNewStartCol;
        int iNewEndCol;

        if( bHeadMoved )
        {
          iNewStartCol = Math.Min( rectRange.Left + iDeltaCol, sourceRect.Right + 1 );
          iNewEndCol = Math.Max( rectRange.Left + iDeltaCol, rectRange.Right );
        }
        else
        {
          iNewStartCol = Math.Min( rectRange.Right + iDeltaCol, rectRange.Left );
          iNewEndCol = Math.Max( rectRange.Right + iDeltaCol, sourceRect.Left - 1 );
        }

        return FormulaUtil.CreatePtg( token.TokenCode, iSourceIndex,
          rectRange.Top, iNewStartCol, rectRange.Bottom, iNewEndCol, ( byte )0, ( byte )0 );
      }
      else if( rectRange.Width == 0 && iDeltaCol == 0 )
      {
        int iNewStartRow;
        int iNewEndRow;

        if( bHeadMoved )
        {
          iNewStartRow = Math.Min( rectRange.Top + iDeltaRow, sourceRect.Bottom  + 1);
          iNewEndRow = Math.Max( rectRange.Top + iDeltaRow, rectRange.Bottom );
        }
        else
        {
          iNewStartRow = Math.Min( rectRange.Bottom + iDeltaRow, rectRange.Top );
          iNewEndRow = Math.Max( rectRange.Bottom + iDeltaRow, sourceRect.Top - 1 );
        }

        return FormulaUtil.CreatePtg( token.TokenCode, iSourceIndex,
          iNewStartRow, rectRange.Left, iNewEndRow, rectRange.Right, ( byte )0, ( byte )0 );
      }

      return token;
    }
    /// <summary>
    /// Partially removes token.
    /// </summary>
    /// <param name="token">Token to shrink</param>
    /// <param name="iSheetIndex">Sheet index.</param>
    /// <param name="rectRange">Range rectangle.</param>
    /// <param name="sourceRect">Source rectangle.</param>
    /// <param name="destRect">Destination rectangle.</param>
    /// <returns>Token after shrink operation.</returns>
    private Ptg PartialRemove( Ptg token, int iSheetIndex, Rectangle rectRange,
      Rectangle sourceRect, Rectangle destRect )
    {
      if( token == null )
        throw new ArgumentNullException( "token" );

      bool bHeadRemoved = UtilityMethods.Contains( destRect, rectRange.Left, rectRange.Top );
      bool bTailRemoved = UtilityMethods.Contains( destRect, rectRange.Right, rectRange.Bottom );

      if( !bHeadRemoved && !bTailRemoved ) return token;

      int iNewStartRow = rectRange.Top;
      int iNewEndRow = rectRange.Bottom;
      int iNewStartCol = rectRange.Left;
      int iNewEndCol = rectRange.Right;

      if( rectRange.Left == rectRange.Right )
      {
        if( bHeadRemoved )
        {
          iNewStartRow = destRect.Bottom + 1;
        }
        else
        {
          iNewEndRow = destRect.Top - 1;
        }
      }
      else if( rectRange.Top == rectRange.Bottom )
      {
        if( bHeadRemoved )
        {
          iNewStartCol = destRect.Right + 1;
        }
        else
        {
          iNewEndCol = destRect.Left - 1;
        }
      }

      return FormulaUtil.CreatePtg( token.TokenCode, iSheetIndex,
        iNewStartRow, iNewStartCol, iNewEndRow, iNewEndCol, ( byte )0, ( byte )0 );
    }
    /// <summary>
    /// Updates error bars.
    /// </summary>
    /// <param name="bar">Represents current error bar.</param>
    /// <param name="barToUpdate">Represents error bar to update.</param>
    private void UpdateErrorBar( ChartErrorBarsImpl bar, ref ChartErrorBarsImpl barToUpdate )
    {
      if( bar == null )
        throw new ArgumentNullException( "bar" );

      if( /*m_errorBarY*/barToUpdate == null )
      {
        barToUpdate = bar;
      }
      else
      {
        if( bar.Type == ExcelErrorBarType.Custom )
        {
          if( barToUpdate.Type != ExcelErrorBarType.Custom )
            throw new ApplicationException( "Cannot parse error bars. Different types." );

          IRange plusRange = bar.PlusRange;
          IRange minusRange = bar.MinusRange;

          if( minusRange != null )
            barToUpdate.MinusRange = minusRange;

          if( plusRange != null )
            barToUpdate.PlusRange = plusRange;

          if( bar.Include == ExcelErrorBarInclude.Minus && barToUpdate.Include == ExcelErrorBarInclude.Plus ||
            bar.Include == ExcelErrorBarInclude.Plus && barToUpdate.Include == ExcelErrorBarInclude.Minus )
          {
            barToUpdate.Include = ExcelErrorBarInclude.Both;
          }
        }
        else
        {
          barToUpdate.Include = ExcelErrorBarInclude.Both;
        }
      }
    }
    /// <summary>
    /// Gets common series format.
    /// </summary>
    /// <returns>Returns common series format.</returns>
    public ChartFormatImpl GetCommonSerieFormat()
    {
      if( m_chart.SecondaryFormats.ContainsIndex( m_iChartGroup ) )
        return m_chart.SecondaryFormats[ m_iChartGroup ];

      return m_chart.PrimaryFormats[ m_iChartGroup ];
    }
    /// <summary>
    /// Sets items with used reference indexes to true.
    /// </summary>
    /// <param name="usedItems">Array to mark used references in.</param>
    public void MarkUsedReferences( bool[] usedItems )
    {
      foreach( ChartAIRecord chartAi in m_hashAi.Values )
      {
        FormulaUtil.MarkUsedReferences( chartAi.ParsedExpression, usedItems );
      }

      if( m_errorBarX != null )
        m_errorBarX.MarkUsedReferences( usedItems );

      if( m_errorBarY != null )
        m_errorBarY.MarkUsedReferences( usedItems );

      if( m_trendLines != null )
        m_trendLines.MarkUsedReferences( usedItems );
    }
    /// <summary>
    /// Updates reference indexes.
    /// </summary>
    /// <param name="arrUpdatedIndexes">Array with updated indexes.</param>
    public void UpdateReferenceIndexes( int[] arrUpdatedIndexes )
    {
      foreach( ChartAIRecord chartAi in m_hashAi.Values )
      {
        Ptg[] tokens = chartAi.ParsedExpression;

        if( FormulaUtil.UpdateReferenceIndexes( tokens, arrUpdatedIndexes ) )
          chartAi.ParsedExpression = tokens;
      }

      if( m_errorBarX != null )
        m_errorBarX.UpdateReferenceIndexes( arrUpdatedIndexes );

      if( m_errorBarY != null )
        m_errorBarY.UpdateReferenceIndexes( arrUpdatedIndexes );

      if( m_trendLines != null )
        m_trendLines.UpdateReferenceIndexes( arrUpdatedIndexes );
    }
    #endregion

    #region Serialize methods
    /// <summary>
    /// Serializes series to the OffsetArrayList.
    /// </summary>
    /// <param name="records">OffsetArrayList that will receive records.</param>
    /// <exception cref="System.ArgumentNullException">
    /// When specified OffsetArrayList is NULL.
    /// </exception>
    [ CLSCompliant( false ) ]
    public void Serialize( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      SerializeSerie( records );

      records.Add( BiffRecordFactory.GetRecord( TBIFFRecord.Begin ) );

      SerializeChartAi( records );
      m_dataPoints.SerializeDataFormats( records );

      ChartSertocrtRecord sertocrt = (ChartSertocrtRecord)
        BiffRecordFactory.GetRecord( TBIFFRecord.ChartSertocrt );
      sertocrt.ChartGroup = (ushort) ChartGroup;

      records.Add( sertocrt );

      SerializeLegendEntries( records );

      records.Add( BiffRecordFactory.GetRecord( TBIFFRecord.End ) );

      if( m_valueEnteredDirectly.Count != 0 )
        UpdateSerieIndexesInEnteredDirectlyValues( m_valueEnteredDirectly );

      if( m_categoryEnteredDirectly.Count != 0 )
        UpdateSerieIndexesInEnteredDirectlyValues( m_categoryEnteredDirectly );

      if( m_bubbleEnteredDirectly.Count != 0 )
        UpdateSerieIndexesInEnteredDirectlyValues( m_bubbleEnteredDirectly );

      if( HasErrorBarsY )
        m_errorBarY.Serialize( m_seriesColl.TrendErrorList );

      if( HasErrorBarsX )
        m_errorBarX.Serialize( m_seriesColl.TrendErrorList );

      m_trendLines.Serialize( m_seriesColl.TrendErrorList );
    }
    /// <summary>
    /// Serializes ChartAi records.
    /// </summary>
    /// <param name="records">OffsetArrayList that will receive records.</param>
    private void SerializeChartAi( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      SeriealizeSerieName( records );

      ChartAIRecord chartAi = ( ChartAIRecord )m_hashAi[ LinkIndex.LinkToValues ];
      SetAIRange( chartAi, m_ValueRange, ReferenceType.EnteredDirectly );

      if( m_ValueRange != null ) chartAi.NumberFormatIndex = 2;

      if( m_valueEnteredDirectly.Count != 0 )
      {
        chartAi.ParsedExpression = null;
        chartAi.Reference = ReferenceType.EnteredDirectly;
      }

      records.Add( ( BiffRecordRaw )chartAi.Clone() );

      chartAi = ( ChartAIRecord )m_hashAi[ LinkIndex.LinkToCategories ];
      SetAIRange( chartAi, m_CategoryRange, ReferenceType.DefaultCategories );

      if( m_categoryEnteredDirectly.Count != 0 )
      {
        chartAi.ParsedExpression = null;
        chartAi.Reference = ReferenceType.EnteredDirectly;
      }

      records.Add( ( BiffRecordRaw )chartAi.Clone() );

      chartAi = ( ChartAIRecord )m_hashAi[ LinkIndex.LinkToBubbles ];
      SetAIRange( chartAi, m_BubbleRange, ReferenceType.EnteredDirectly );

      if( m_bubbleEnteredDirectly.Count != 0 )
      {
        chartAi.ParsedExpression = null;
        chartAi.Reference = ReferenceType.EnteredDirectly;
      }

      records.Add( ( BiffRecordRaw )chartAi.Clone() );
    }
    /// <summary>
    /// Sets range value into the ChartAi record.
    /// </summary>
    /// <param name="record">ChartAi record that will contain specified range.</param>
    /// <param name="range">Range that will be assigned.</param>
    /// <param name="onNull">ReferenceType that should be written if range is NULL.</param>
    /// <exception cref="System.ArgumentNullException">
    /// When specified ChartAi record is NULL.
    /// </exception>
    private void SetAIRange( ChartAIRecord record, IRange range, ReferenceType onNull )
    {
      if( record == null )
        throw new ArgumentNullException( "record" );

      if( range == null )
      {
        record.Reference = onNull;
        record.ParsedExpression = null;
      }
      else
      {
        record.Reference = ReferenceType.Worksheet;  
          if(range.GetType()== typeof(RangeImpl))
        ((RangeImpl)range).SetWorkbook(this.m_book);         
        record.ParsedExpression = ( ( INativePTG )range ).GetNativePtg();
      }
    }
    /// <summary>
    /// Fills and serializes ChartSeries record.
    /// </summary>
    /// <param name="records">OffsetArrayList that will receive records.</param>
    private void SerializeSerie( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      m_series.StdX = ChartSeriesRecord.DataType.Numeric;
      m_series.StdY = ChartSeriesRecord.DataType.Numeric;
      m_series.BubbleDataType = ChartSeriesRecord.DataType.Numeric;

      if( m_valueEnteredDirectly.Count == 0 )
      {
        m_series.ValuesCount = ( m_ValueRange != null )
          ? ( ushort )m_ValueRange.Count//Cells.Length
          : ( ushort )0;
      }
      else
      {
        m_series.ValuesCount = ( ushort )m_valueEnteredDirectly.Count;
        
        if( m_valueEnteredDirectly[ 0 ] is LabelRecord )
          m_series.StdY = ChartSeriesRecord.DataType.Text;
      }

      if( m_categoryEnteredDirectly.Count == 0 )
      {
        m_series.CategoriesCount = ( m_CategoryRange != null )
          ? ( ushort )m_CategoryRange.Count//Cells.Length
          : m_series.ValuesCount;
      }
      else
      {
        m_series.CategoriesCount = ( ushort )m_categoryEnteredDirectly.Count;

        if( m_categoryEnteredDirectly[ 0 ] is LabelRecord )
          m_series.StdX = ChartSeriesRecord.DataType.Text;
      }

      if( m_bubbleEnteredDirectly.Count == 0 )
      {
        m_series.BubbleSeriesCount = ( m_BubbleRange != null )
          ? ( ushort )m_BubbleRange.Count//Cells.Length
          : ( ushort )0;
      }
      else
      {
        m_series.BubbleSeriesCount = ( ushort )m_bubbleEnteredDirectly.Count;

        if( m_bubbleEnteredDirectly[ 0 ] is LabelRecord )
          m_series.BubbleDataType = ChartSeriesRecord.DataType.Text;
      }

      CheckLimits();

      records.Add( ( BiffRecordRaw )m_series.Clone() );
    }
    /// <summary>
    /// Checks whether number of items inside single series is correct.
    /// </summary>
    public void CheckLimits()
    {
      int iCount;
      if( m_categoryEnteredDirectly.Count == 0 )
      {
        iCount = ( m_CategoryRange != null ) ?
          ( ushort )m_CategoryRange.Count :
          m_series.ValuesCount;
      }
      else
      {
        iCount = m_categoryEnteredDirectly.Count;
      }

      ExcelVersion version = m_book.Version;

      if( ( version == ExcelVersion.Excel97to2003 || version == ExcelVersion.Excel2007 ) &&
        ( ( m_chart.IsChart3D && iCount > 4000 ) || iCount > 32000 ) )
      {
        throw new ApplicationException( "The maximum number of data points you can use in a data series for a 2-D chart is 32000, for a 3-D chart is 4000." +
          "If you want to use more data points, you must create two or more series or use Excel 2010." );
      }
    }
    /// <summary>
    /// Serializes data labels data.
    /// </summary>
    /// <param name="records">List of biff records to serialize into.</param>
    [ CLSCompliant( false ) ]
    public void SerializeDataLabels( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      if( m_dataPoints != null )
      {
        m_dataPoints.SerializeDataLabels( records );
      }
    }
    /// <summary>
    /// Serializes legend entries.
    /// </summary>
    /// <param name="records">Record storage.</param>
    private void SerializeLegendEntries( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      if( !m_chart.HasLegend )
        return;

      string strStartType = ChartFormatImpl.GetStartSerieType( m_chart.ChartType );
      ChartLegendEntriesColl legendEntries = ( ChartLegendEntriesColl )m_chart.Legend.LegendEntries;

      if( Array.IndexOf( ChartImpl.DEF_LEGEND_NEED_DATA_POINT, strStartType ) == -1 )
      {
        if( legendEntries.Contains( Index ) )
        {
          ChartLegendEntryImpl entry = ( ChartLegendEntryImpl )legendEntries[ Index ];
          
          entry.Serialize( records );
        }
      }
      else if( Index == 0 )
      {
        for( int i = 0, ilen = legendEntries.Count; i < ilen; i++ )
        {
          if( legendEntries.Contains( i ) )
          {
            ChartLegendEntryImpl entry = ( ChartLegendEntryImpl )legendEntries[ i ];
          
            entry.Serialize( records );
          }
        }
      }
    }
    /// <summary>
    /// Serialize series name.
    /// </summary>
    /// <param name="records">Represents record storage.</param>
    private void SeriealizeSerieName( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      string strName = Name;
      ChartAIRecord chartAi = ( ChartAIRecord )m_hashAi[ LinkIndex.LinkToTitleOrText ];

      if( ( m_strName == null || m_strName.Length == 0 ) && m_nameRange == null || m_strName[ 0 ] != '=' )
      {
        chartAi.ParsedExpression = null;
        chartAi.Reference = ReferenceType.EnteredDirectly;
      }
      else
      {
        Ptg[] tokens = /*m_nameTokens;/*/GetNameTokens();
        chartAi.ParsedExpression = tokens;
        chartAi.Reference = ReferenceType.Worksheet;
      }

      records.Add( ( BiffRecordRaw )chartAi.Clone() );

      if( !m_bDefaultName )
      {
        ChartSeriesTextRecord seriesText = ( ChartSeriesTextRecord )
          BiffRecordFactory.GetRecord( TBIFFRecord.ChartSeriesText );
        seriesText.Text = ( strName == null ) ? "" : strName;
        records.Add( seriesText );
      }
    }
    private Ptg[] GetNameTokens()
    {
      Ptg[] tokens;
      if( m_nameRange == null && m_strName != null && m_strName[ 0 ] == '=' )
      {
        string formula = UtilityMethods.RemoveFirstCharUnsafe( m_strName );
        tokens = m_book.FormulaUtil.ParseString( formula );
      }
      else
      {
        tokens = ( ( INativePTG )m_nameRange ).GetNativePtg();
      }

      return tokens;
    }
    #endregion

    #region Events
    /// <summary>
    /// This event is raised when name of the series changes.
    /// </summary>
    public event ValueChangedEventHandler NameChanged;
    #endregion

    #region Implementation properties
    /// <summary>
    /// Represents index of the series.
    /// </summary>
    public int Index
    {
      get
      {
        return m_iIndex;
      }
      set
      {
        m_iIndex = value;
        m_dataPoints.UpdateSerieIndex();
      }
    }
    /// <summary>
    /// Represent series is Filter
    /// </summary>
    public bool IsFiltered
    {
        get
        {
            return m_IsFiltered;
        }
        set
        {
            m_IsFiltered = value;
        }
    }
    /// <summary>
    /// Series drawing/settings order.//Number of the series.
    /// </summary>
    public int Number
    {
      get
      {
        //return Index;
        return m_iOrder;
      }
      set
      {
        //Index = value;
        m_iOrder = value;
      }
    }
    /// <summary>
    /// Index of the chart group.
    /// </summary>
    public int ChartGroup
    {
      get
      {
        return m_iChartGroup;
      }
      set
      {
        m_iChartGroup = value;
      }
    }
    /// <summary>
    /// Returns parent chart. Read-only.
    /// </summary>
    public ChartImpl InnerChart
    {
      get
      {
        return m_chart;
      }
    }
    /// <summary>
    /// Indicates whether series has default title.
    /// </summary>
    public bool IsDefaultName
    {
      get
      {
        return m_bDefaultName;
      }
      set
      {
        m_bDefaultName = value;
      }
    }
    /// <summary>
    /// Returns number of points in the series. Read-only.
    /// </summary>
    public int PointNumber
    {
      get
      {
        string strStartType = ChartFormatImpl.GetStartSerieType( m_chart.ChartType );

        if( ChartImpl.START_SURFACE == strStartType )
          return DEF_SURFACE_POINT_NUMBER;

        return ( m_ValueRange != null ) ? ( ( ICombinedRange )m_ValueRange ).CellsCount : 0;
      }
    }
    /// <summary>
    /// Returns parent workbook. Read-only.
    /// </summary>
    public WorkbookImpl InnerWorkbook
    {
      get
      {
        return m_book;
      }
    }
    /// <summary>
    /// Represent the filtered category label range
    /// </summary>
    public string FilteredCategory
    {
        get
        {
            return m_categoryFilteredRange;
        }
        set
        {
            m_categoryFilteredRange = value;
        }
    }
    /// <summary>
    /// Represent the Filtered Category value
    /// </summary>
    public string FilteredValue
    {
        get
        {
            return m_categoryValue;
        }
        set
        {
            m_categoryValue = value;
        }
    }
    /// <summary>
    /// Returns series start type. Read-only.
    /// </summary>
    public string StartType
    {
      get
      {
        return DetectSerieTypeStart();//ChartFormatImpl.GetStartSerieType( SerieType );
      }
    }
    /// <summary>
    /// Represents not default series text for first series. Read-only.
    /// </summary>
    public string ParseSerieNotDefaultText
    {
      get
      {
        return m_seriesText;
      }
    }
    /// <summary>
    /// Gets parent series collection. Read - only.
    /// </summary>
    public ChartSeriesCollection ParentSeries
    {
      get
      {
        return m_seriesColl;
      }
    }
    /// <summary>
    /// Represents parent chart. Read - only.
    /// </summary>
    public ChartImpl ParentChart
    {
      get
      {
        return m_chart;
      }
    }
    /// <summary>
    /// Gets parent workbook. Read - only.
    /// </summary>
    internal WorkbookImpl ParentBook
    {
      get
      {
        return m_book;
      }
    }
    /// <summary>
    /// Indicates whether this is pie series.
    /// </summary>
    public bool IsPie
    {
      get
      {
        return ChartImpl.GetIsChartPie( SerieType );
      }
    }
    /// <summary>
    /// Gets series name or formula value (not actual string value).
    /// </summary>
    public string NameOrFormula
    {
      get
      {
          if (m_nameRange != null)
          {
              if (m_strName.Substring(1) != m_nameRange.AddressGlobal)
                  m_strName = "=" + m_nameRange.AddressGlobal;
          }

        return ( m_strName != null && m_strName.Length>0 ) ?
            (m_strName[ 0 ] == '=' && m_nameTokens != null)?
          '=' + m_book.FormulaUtil.ParsePtgArray( m_nameTokens ) :
          m_strName:
          m_strName;
      }
    }
    /// <summary>
    /// This element specifies the series
    /// to invert its colors if the value is negative.
    /// </summary>
    public bool? InvertIfNegative
    {
        get
        {
            return m_bInvertIfNegative;
        }
        set
        {
            m_bInvertIfNegative = value;
        }
    }
    /// <summary>
    /// Gets or sets the string reference formula
    /// </summary>
    public string StrRefFormula
    {
        get
        {
            return m_strRefFormula;
        }
        set
        {
            m_strRefFormula = value;
        }
    }
    /// <summary>
    /// Gets or sets the number reference formula
    /// </summary>
    internal string NumRefFormula
    {
        get
        {
            return m_numRefFormula;
        }
        set
        {
            m_numRefFormula = value;
        }
    }      
    /// <summary>
    /// Gets or sets the number reference formula
    /// </summary>
    internal string MulLvlStrRefFormula
    {
        get
        {
            return m_MulLvlStrRefFormula;
        }
        set
        {
            m_MulLvlStrRefFormula = value;
        }
    }
    /// <summary>
    /// Gets or sets the chart's drop lines property as stream.
    /// TODO: Need to add parsing support for drop lines. Should be removed after adding parsing support.
    /// </summary>
    internal Stream DropLinesStream
    {
        get
        {
            return m_dropLinesStream;
        }
        set
        {
            m_dropLinesStream = value;
        }
    }
    #endregion

    #region IReparse Members
    /// <summary>
    /// Reparses method.
    /// </summary>
    public void Reparse()
    {
      ChartAIRecord chartAi = ( ChartAIRecord )m_hashAi[ LinkIndex.LinkToValues ];
      m_ValueRange = GetRange( chartAi );

      chartAi = ( ChartAIRecord )m_hashAi[ LinkIndex.LinkToBubbles ];
      m_BubbleRange = GetRange( chartAi );

      chartAi = ( ChartAIRecord )m_hashAi[ LinkIndex.LinkToCategories ];
      m_CategoryRange = GetRange( chartAi );
    }
    /// <summary>
    /// Returns range from ChartAi record.
    /// </summary>
    /// <param name="chartAi">ChartAi record that contains range.</param>
    /// <returns>Parsed range.</returns>
    /// <exception cref="System.ArgumentNullException">
    /// When chartAi is NULL.
    /// </exception>
    [ CLSCompliant( false ) ]
    public IRange GetRange( ChartAIRecord chartAi )
    {
      if( chartAi == null )
        throw new ArgumentNullException( "chartAi" );

      if( chartAi.ParsedExpression == null || chartAi.ParsedExpression.Length <= 1 )
      {
        if( chartAi.Reference == ReferenceType.Worksheet || chartAi.ParsedExpression != null )
        {
          Ptg reference = chartAi.ParsedExpression[ 0 ];

          return GetRangeFromOnePTG( reference );
        }
      }
      else
      {
        IRanges ranges = null; //m_book.ActiveSheet.CreateRangesCollection();

        for( int i = 0, iLen = chartAi.ParsedExpression.Length; i < iLen; i++ )
        {
          Ptg reference = chartAi.ParsedExpression[ i ];

          if( reference is IRangeGetter )
          {
            IRange range = GetRangeFromOnePTG( reference );

            if( range != null )
            {
              if( ranges == null )
              {
                IWorksheet sheet = range.Worksheet;

                if( sheet != null )
                  ranges = range.Worksheet.CreateRangesCollection();
              }

              if( ranges != null )
                ranges.Add( range );
            }
          }
        }

        return ( ranges == null || ranges.Count == 0 ) ? null : ranges;
      }

      return null;
    }
    /// <summary>
    /// Gets IRange from current ptg.
    /// </summary>
    /// <param name="currentPtg">Current ptg.</param>
    /// <returns>Returns ptg.</returns>
    private IRange GetRangeFromOnePTG( Ptg currentPtg )
    {
      if( currentPtg == null )
        throw new ArgumentNullException( "currentPtg" );

      if( !( currentPtg is IRangeGetter ) )
        throw new ParseException( "currentPtg" );

      if( !currentPtg.IsOperation && currentPtg.ToString( m_book.FormulaUtil, 0, 0
        , false ).IndexOf( "#REF" ) != -1 )
      {
        return null;
      }

      IRangeGetter getter = ( IRangeGetter )currentPtg;

      return getter.GetRange( m_book, null );
    }
    #endregion
  }
}
