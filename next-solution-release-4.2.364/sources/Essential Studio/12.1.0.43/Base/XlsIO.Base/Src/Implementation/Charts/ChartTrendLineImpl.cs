#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;

using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser.Biff_Records.Charts;
using LinkIndex = Syncfusion.XlsIO.Parser.Biff_Records.Charts.ChartAIRecord.LinkIndex;
using Syncfusion.XlsIO.Parser.Biff_Records.Formula;
using System.Collections.Generic;

namespace Syncfusion.XlsIO.Implementation.Charts
{
  /// <summary>
  /// Represents chart trend line.
  /// </summary>
  public class ChartTrendLineImpl
    : CommonObject
    , IChartTrendLine
  {
    #region Class constants
    /// <summary>
    /// Represents max order value for polynomial trend line type.
    /// </summary>
    private const int DEF_ORDER_MAX_VALUE = 6;
    #endregion

    #region Class static members
    /// <summary>
    /// Represents name hash table
    /// Key - trend line type, Value - legend string.
    /// </summary>
    private static Dictionary<ExcelTrendLineType, string> m_hashNames = new Dictionary<ExcelTrendLineType, string>( 6 );
    #endregion

    #region Class members
    /// <summary>
    /// Represents main trend line record.
    /// </summary>
    private ChartSerAuxTrendRecord m_record;
    /// <summary>
    /// Represents Shadow
    /// </summary>
    private ShadowImpl m_shadow;
    /// <summary>
    /// Represents border object.
    /// </summary>
    private ChartBorderImpl m_border;
    /// <summary>
    /// Represents parent series.
    /// </summary>
    private ChartSerieImpl m_serie;
    /// <summary>
    /// Represents trend line type.
    /// </summary>
    private ExcelTrendLineType m_type = ExcelTrendLineType.Linear;
    /// <summary>
    /// Indicates if name is auto.
    /// </summary>
    private bool m_isAutoName = true;
    /// <summary>
    /// Represents custom name.
    /// </summary>
    private string m_strName = "";
    /// <summary>
    /// Represents text area for data label.
    /// </summary>
    private ChartTextAreaImpl m_textArea;
    /// <summary>
    /// Represents some unique ID.
    /// </summary>
    private int m_iIndex;
        /// <summary>
        /// Represents the 3D features
        /// </summary>
        private ThreeDFormatImpl m_3D;
    #endregion

    #region Class static constructors
    /// <summary>
    /// Initialize all static members.
    /// </summary>
    static ChartTrendLineImpl()
    {
      m_hashNames.Add( ExcelTrendLineType.Exponential, "Expon. " );
      m_hashNames.Add( ExcelTrendLineType.Linear, "Linear " );
      m_hashNames.Add( ExcelTrendLineType.Logarithmic, "Log. " );
      m_hashNames.Add( ExcelTrendLineType.Moving_Average, " per. Mov. Avg. " );
      m_hashNames.Add( ExcelTrendLineType.Polynomial, "Poly. " );
      m_hashNames.Add( ExcelTrendLineType.Power, "Power " );
    }
    #endregion

    #region Class constructors
    /// <summary>
    /// Creates new instance of object.
    /// </summary>
    /// <param name="application">Represents current application.</param>
    /// <param name="parent">Represents parent object.</param>
    public ChartTrendLineImpl( IApplication application, object parent )
      : base( application, parent )
    {
      FindParents();

      m_border = new ChartBorderImpl( application, this );
      m_border.HasLineProperties = true;
      m_record = ( ChartSerAuxTrendRecord )
        BiffRecordFactory.GetRecord( TBIFFRecord.ChartSerAuxTrend );
    }
    /// <summary>
    /// Finds parent objects.
    /// </summary>
    private void FindParents()
    {
      m_serie = ( ChartSerieImpl )FindParent( typeof( ChartSerieImpl ) );

      if( m_serie == null )
        throw new NotSupportedException( "Cannot find parent objects" );
    }
    /// <summary>
    /// Creates new instance of trend object by parsing from stream.
    /// </summary>
    /// <param name="application">Represents application object.</param>
    /// <param name="parent">Represents parent object.</param>
    /// <param name="data">Represents data holder.</param>
    /// <param name="iPos">Represents position in stream.</param>
    /// <param name="entry">Represents parsed legend entry, or null.</param>
    public ChartTrendLineImpl( IApplication application, object parent, IList<BiffRecordRaw> data, ref int iPos
      , out ChartLegendEntryImpl entry )
      : base( application, parent )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      FindParents();
      entry = null;

      Parse( data, ref iPos, ref entry );
    }
    #endregion

    #region IChartTrendLine properties
    /// <summary>
    /// Represents border object. Read-only.
    /// </summary>
    public IChartBorder Border
    {
      get
      {
        return m_border;
      }
    }
    /// <summary>
    /// Represents the Shadow.Read-only
    /// </summary>
    public IShadow Shadow
    {
      get
      {
        if( m_shadow == null )
          m_shadow = new ShadowImpl( Application, this );

        return m_shadow;
      }
    }
    /// <summary>
    /// This property indicates whether the shadow object has been created 
    /// </summary>
    public bool HasShadowProperties
    {
      get
      {
        return m_shadow != null;
      }
      internal set
      {
        if( value )
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
    public IThreeDFormat Chart3DOptions
    {
      get
      {
        if( m_3D == null )
          m_3D = new ThreeDFormatImpl( Application, this );

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
        if( value )
        {
          IThreeDFormat Threed = Chart3DOptions;
        }
        else
        {
          m_3D = null;
        }
      }
    }
    /// <summary>
    /// Represents number of periods that the trend line extends backward.
    /// </summary>
    public double Backward
    {
      get
      {
        return m_record.NumBackcast;
      }
      set
      {
        if( value != Backward )
        {
          CheckRecordProprties();
          CheckBackward( value );

          m_record.NumBackcast = value;
        }
      }
    }
    /// <summary>
    ///Represents number of periods that the trend line extends forward.
    /// </summary>
    public double Forward
    {
      get
      {
        return m_record.NumForecast;
      }
      set
      {
        if( Forward != value )
        {
          CheckRecordProprties();

          if( value < 0 )
            throw new ArgumentOutOfRangeException( "Forward" );

          m_record.NumForecast = value;
        }
      }
    }
    /// <summary>
    /// True if the equation for the trend line is displayed on the chart.
    /// </summary>
    public bool DisplayEquation
    {
      get
      {
        return m_record.IsEquation;
      }
      set
      {
        if( value != DisplayEquation )
        {
          CheckRecordProprties();

          m_record.IsEquation = value;
          UpdateDataLabels( value );
        }
      }
    }
    /// <summary>
    /// True if the R-squared value of the trend line is displayed on the chart.
    /// </summary>
    public bool DisplayRSquared
    {
      get
      {
        return m_record.IsRSquared;
      }
      set
      {
        if( value != DisplayRSquared )
        {
          CheckRecordProprties();

          m_record.IsRSquared = value;
          UpdateDataLabels( value );
        }
      }
    }
    /// <summary>
    /// Represents point where the trend line crosses the value axis.
    /// </summary>
    public double Intercept
    {
      get
      {
        return m_record.NumIntercept;
      }
      set
      {
        if( Intercept != value )
        {
          CheckRecordProprties();
          CheckIntercept();

          if( Type == ExcelTrendLineType.Exponential && value <= 0 )
            throw new ArgumentOutOfRangeException( "Intercept" );

          m_record.NumIntercept = value;
        }
      }
    }
    /// <summary>
    /// True if the point where the trend line crosses the value
    ///  axis is automatically determined by the regression.
    /// </summary>
    public bool InterceptIsAuto
    {
      get
      {
        return Double.IsNaN( Intercept );
      }
      set
      {
        if( InterceptIsAuto != value )
        {
          CheckRecordProprties();
          CheckIntercept();

          if( value )
          {
            Intercept = ChartSerAuxTrendRecord.DEF_NAN_VALUE;
          }
          else
          {
            Intercept = ( Type == ExcelTrendLineType.Exponential ) ? 1 : 0;
          }
        }
      }
    }
    /// <summary>
    /// Represents trend line type.
    /// </summary>
    public ExcelTrendLineType Type
    {
      get
      {
        return m_type;
      }
      set
      {
        m_type = value;

        OnTypeChanging( value );
      }
    }
    /// <summary>
    /// Represents for Moving Average and Polynomial trend line type order value.
    /// </summary>
    public int Order
    {
      get
      {
        return m_record.Order;
      }
      set
      {
        if( value <= 0 )
          throw new ArgumentOutOfRangeException( "Order" );

        m_record.Order = ( byte )value;
      }
    }
    /// <summary>
    /// Indicates if name is default.
    /// </summary>
    public bool NameIsAuto
    {
      get
      {
        return m_isAutoName;
      }
      set
      {
        if( NameIsAuto != value )
        {
          if( value )
            m_strName = string.Empty;

          m_isAutoName = value;
        }
      }
    }
    /// <summary>
    /// Represents trend line name.
    /// </summary>
    public string Name
    {
      get
      {
        if( !NameIsAuto )
          return m_strName;

        ExcelTrendLineType type = Type;
        string result = m_hashNames[ type ];

        if( type == ExcelTrendLineType.Moving_Average )
          result = Order.ToString() + result;

        result += "(" + m_serie.Name + ")";

        return result;
      }
      set
      {
        if( value == null )
          throw new ArgumentNullException( "Name" );

        m_strName = value;
        NameIsAuto = false;
      }
    }
    /// <summary>
    /// Returns data label. Read-only.
    /// </summary>
    public IChartTextArea DataLabel
    {
      get
      {
        if( m_textArea == null )
          throw new NotSupportedException( "Cannot return data label." );

        return m_textArea;
      }
    }
    #endregion

    #region IChartTrendLine methods
    /// <summary>
    /// Clears current trend line.
    /// </summary>
    public void ClearFormats()
    {
      m_border.AutoFormat = true;
      m_type = ExcelTrendLineType.Linear;
      m_isAutoName = true;
      m_strName = "";

      m_record.Order = 1;
      m_record.IsRSquared = false;
      m_record.IsEquation = false;
      m_textArea = null;
      m_record.NumForecast = 0;
      m_record.NumBackcast = 0;
      m_record.NumIntercept = ChartSerAuxTrendRecord.DEF_NAN_VALUE;
    }
    /// <summary>
    /// Sets items with used reference indexes to true.
    /// </summary>
    /// <param name="usedItems">Array to mark used references in.</param>
    public void MarkUsedReferences( bool[] usedItems )
    {
      if( m_textArea != null )
        m_textArea.MarkUsedReferences( usedItems );
    }
    /// <summary>
    /// Updates reference indexes.
    /// </summary>
    /// <param name="arrUpdatedIndexes">Array with updated indexes.</param>
    public void UpdateReferenceIndexes( int[] arrUpdatedIndexes )
    {
      if( m_textArea != null )
        m_textArea.UpdateReferenceIndexes( arrUpdatedIndexes );
    }
    #endregion

    #region Class parse methods
    /// <summary>
    /// Parse trend object from stream.
    /// </summary>
    /// <param name="data">Represents record holder.</param>
    /// <param name="iPos">Represents position in storage.</param>
    /// <param name="entry">Represents legend entry.</param>
    private void Parse( IList<BiffRecordRaw> data, ref int iPos, ref ChartLegendEntryImpl entry )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      BiffRecordRaw record = ( BiffRecordRaw )data[ iPos ];

      if( record.TypeCode != TBIFFRecord.ChartSeries )
        throw new ArgumentOutOfRangeException( "iPos" );

      iPos += 2;
      int iCount = 1;
      ChartImpl chart = m_serie.ParentChart;
      bool bHasLegend = chart.HasLegend;
      ChartLegendEntriesColl coll = null;

      if( bHasLegend )
        coll = ( ChartLegendEntriesColl )chart.Legend.LegendEntries;

      while( iCount > 0 )
      {
        record = ( BiffRecordRaw )data[ iPos ];

        switch( record.TypeCode )
        {
          case TBIFFRecord.Begin:
            iCount++;
            break;

          case TBIFFRecord.End:
            iCount--;
            break;

          case TBIFFRecord.ChartSerAuxTrend:
            m_record = ( ChartSerAuxTrendRecord )record;
            break;

          case TBIFFRecord.ChartDataFormat:
            m_iIndex = ( ( ChartDataFormatRecord )record ).SeriesIndex;
            break;

          case TBIFFRecord.ChartLineFormat:
            m_border = new ChartBorderImpl( Application, this, ( ChartLineFormatRecord )record );
            break;

          case TBIFFRecord.ChartLegendxn:
            if( bHasLegend )
            {
              int iTrendIndex = m_serie.ParentSeries.TrendIndex;
              entry = new ChartLegendEntryImpl( Application, coll, iTrendIndex, data, ref iPos );
              iPos--;
            }
            break;

          case TBIFFRecord.ChartSeriesText:
            ChartSeriesTextRecord text = ( ChartSeriesTextRecord )record;
            Name = text.Text;
            break;
        }

        iPos++;
      }

      iPos--;
      UpdateType();
    }
    #endregion

    #region Class serialize methods
    /// <summary>
    /// Serialize records to biff stream
    /// </summary>
    /// <param name="records">Represents record holder.</param>
    [ CLSCompliant( false ) ]
    public void Serialize( IList<IBiffStorage> records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      int iSerieIndex = m_serie.Index;
      ChartSeriesCollection series = m_serie.ParentSeries;

      ChartErrorBarsImpl.SerializeSerieRecord( records, 0 );

      BiffRecordRaw record = BiffRecordFactory.GetRecord( TBIFFRecord.Begin );
      records.Add( record );

      SerializeChartAi( records );

      ChartErrorBarsImpl.SerializeDataFormatRecords( records, m_border, iSerieIndex
        , series.TrendErrorBarIndex, null );

      ChartSerParentRecord serRecord = ( ChartSerParentRecord )
        BiffRecordFactory.GetRecord( TBIFFRecord.ChartSerParent );

      SerializeDataLabels( series.TrendLabels );
      serRecord.Series = ( ushort )( iSerieIndex + 1 );
      records.Add( serRecord );

      m_record.UpdateType( m_type );
      records.Add( m_record );

      SerializeLegendEntry( records );
      series.TrendIndex++;
      series.TrendErrorBarIndex++;

      record = BiffRecordFactory.GetRecord( TBIFFRecord.End );
      records.Add( record );
    }
    /// <summary>
    /// Serialize chart ai records.
    /// </summary>
    /// <param name="records">Represents record holder.</param>
    private void SerializeChartAi( IList<IBiffStorage> records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      ChartAIRecord recordAi = ( ChartAIRecord )BiffRecordFactory.GetRecord( TBIFFRecord.ChartAI );
      recordAi.IndexIdentifier = LinkIndex.LinkToTitleOrText;
      recordAi.Reference = ChartAIRecord.ReferenceType.EnteredDirectly;
      records.Add( recordAi );

      if( !NameIsAuto )
      {
        ChartSeriesTextRecord textRecord = ( ChartSeriesTextRecord )
          BiffRecordFactory.GetRecord( TBIFFRecord.ChartSeriesText );

        textRecord.Text = m_strName;
        records.Add( textRecord );
      }

      recordAi = ( ChartAIRecord )recordAi.Clone();
      recordAi.IndexIdentifier = LinkIndex.LinkToValues;
      records.Add( recordAi );

      recordAi = ( ChartAIRecord )recordAi.Clone();
      recordAi.IndexIdentifier = LinkIndex.LinkToCategories;
      records.Add( recordAi );

      recordAi = ( ChartAIRecord )recordAi.Clone();
      recordAi.IndexIdentifier = LinkIndex.LinkToBubbles;
      records.Add( recordAi );
    }
    /// <summary>
    /// Serialize legend entry.
    /// </summary>
    /// <param name="records">Represents record storage.</param>
    private void SerializeLegendEntry( IList<IBiffStorage> records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      ChartImpl chart = m_serie.ParentChart;

      if( !chart.HasLegend )
        return;

      int iTrendIndex = m_serie.ParentSeries.TrendIndex;
      IChartLegendEntries entrys = chart.Legend.LegendEntries;
      ChartLegendEntryImpl entry = ( ChartLegendEntryImpl )entrys[ iTrendIndex ];

      entry.Serialize( records );
    }
    /// <summary>
    /// Serialize data labels.
    /// </summary>
    /// <param name="records">Represents records holder.</param>
    private void SerializeDataLabels( IList<IBiffStorage> records )
    {
      if( m_textArea == null )
        return;

      m_textArea.ObjectLink.SeriesNumber = ( ushort )( m_serie.ParentSeries.TrendErrorBarIndex );
      m_textArea.Serialize( records );
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// Checks some records properties.
    /// </summary>
    private void CheckRecordProprties()
    {
      if( Type == ExcelTrendLineType.Moving_Average )
        throw new NotSupportedException( "This property doesnot support on current trend line type." );
    }
    /// <summary>
    /// Check if current trend support intercept property, if failed throws exception.
    /// </summary>
    private void CheckIntercept()
    {
      ExcelTrendLineType type = Type;

      if( type == ExcelTrendLineType.Logarithmic || type == ExcelTrendLineType.Power )
        throw new NotSupportedException( "This property doesnot support in current trend line type." );
    }
    /// <summary>
    /// Check if current trend support backward property, if failed throws exception.
    /// </summary>
    /// <param name="value">Represents value.</param>
    private void CheckBackward( double value )
    {
      if( value < 0 )
        throw new ArgumentOutOfRangeException( "Backward" );

      string strType = ChartFormatImpl.GetStartSerieType( m_serie.SerieType );

      bool bFlag = strType == ChartImpl.START_BAR || strType == ChartImpl.START_COLUMN
        || strType == ChartImpl.START_LINE;

      if( bFlag && value > 0.5 )
        throw new ArgumentOutOfRangeException( "The value must be between zero and 0,5" );

      if( strType == ChartImpl.START_AREA )
        throw new NotSupportedException( "This property doesnot supported on current trendline object." );        
    }
    /// <summary>
    /// Changes trend line type.
    /// </summary>
    /// <param name="type">Represents new trend line type.</param>
    private void OnTypeChanging( ExcelTrendLineType type )
    {
      bool bFlag = type == ExcelTrendLineType.Moving_Average;

      if( bFlag && m_serie.PointNumber < 3 && !m_serie.ParentBook.Loading )
        throw new NotSupportedException( "This trendline type is supported only if data points count is greater than 2" );

      m_record.Order = ( byte )( ( bFlag || type == ExcelTrendLineType.Polynomial ) ? 2 : 1 );
      m_record.NumIntercept = ChartSerAuxTrendRecord.DEF_NAN_VALUE;
    }
    /// <summary>
    /// Check if current trend support backward property, if failed throws exception.
    /// </summary>
    ///<param name="value">Represents order value.</param>
    private void CheckOrder( int value )
    {
      if( m_type == ExcelTrendLineType.Polynomial || m_type == ExcelTrendLineType.Moving_Average )
      {
        int iTop = DEF_ORDER_MAX_VALUE;

        if( m_type != ExcelTrendLineType.Moving_Average )
          iTop = m_serie.PointNumber - 1;

        if( value < 2 || value > iTop )
          throw new ArgumentOutOfRangeException( "Order" );
      }
      else
      {
        throw new NotSupportedException( "This property doesnot support in current trendline type." );
      }
    }
    /// <summary>
    /// Updates data labels
    /// </summary>
    /// <param name="value">Represents flag for updates.</param>
    private void UpdateDataLabels( bool value )
    {
      if( value && m_textArea == null )
      {
        m_textArea = new ChartTextAreaImpl( Application, this );
        m_textArea.IsTrend = true;

        return;
      }

      if( DisplayEquation == false && DisplayRSquared == false )
        m_textArea = null;
    }
    /// <summary>
    /// Updates trend type on parsing.
    /// </summary>
    private void UpdateType()
    {
      bool bFlag = m_record.RegressionType == ChartSerAuxTrendRecord.TRegression.Polynomial;
      ChartSeriesCollection series = m_serie.ParentSeries;

      if( bFlag && m_record.Order < 2 )
      {
        m_type = ExcelTrendLineType.Linear;
      }
      else
      {
        m_type = ( ExcelTrendLineType )m_record.RegressionType;
      }

      series.TrendIndex++;
    }
    /// <summary>
    /// Sets current data label.
    /// </summary>
    /// <param name="area">Represents data label.</param>
    public void SetDataLabel( ChartTextAreaImpl area )
    {
      if( area == null )
        throw new ArgumentNullException( "area" );

      m_textArea = area;
      m_textArea.IsTrend = true;
    }
    #endregion

    #region Class helper properties
    /// <summary>
    /// Represents some unique id.
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
      }
    }
    #endregion

    #region Class clone methods
    /// <summary>
    /// Clones current object.
    /// </summary>
    /// <param name="parent">Represents parent object for new cloned instance.</param>
    /// <param name="dicFontIndexes">Represents new font indexes.</param>
    /// <param name="dicNewSheetNames">Dictionary with new worksheet names.</param>
    /// <returns>Returns cloned object.</returns>
    public ChartTrendLineImpl Clone( object parent, Dictionary<int, int> dicFontIndexes,
      Dictionary<string, string> dicNewSheetNames )
    {
      if( parent == null )
        throw new ArgumentNullException( "parent" );

      ChartTrendLineImpl result = ( ChartTrendLineImpl )MemberwiseClone();

      result.SetParent( parent );
      result.FindParents();

      result.m_border = m_border.Clone( result );

      if( m_record != null )
        result.m_record = ( ChartSerAuxTrendRecord )CloneUtils.CloneCloneable( m_record );

      if( m_textArea != null )
        result.m_textArea = ( ChartTextAreaImpl )m_textArea.Clone( result, dicFontIndexes, dicNewSheetNames );

      return result;
    }
    #endregion
  }
}
