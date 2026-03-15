#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

using System;
using System.Collections;
using System.Collections.Generic;

using Syncfusion.XlsIO;
using Syncfusion.XlsIO.Implementation.Collections;
using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser.Biff_Records.Charts;
#if ( WINRT )
using Windows.UI;
using Syncfusion.XlsIO.Implementation.WINRT;
using Syncfusion.XlsIO.Implementation.Exceptions;
#endif
#if  (SILVERLIGHT)
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.Silverlight;
using Syncfusion.XlsIO.Implementation.Exceptions;
#elif (WP)
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.WP;
using Syncfusion.XlsIO.Implementation.Exceptions;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;
#endif

namespace Syncfusion.XlsIO.Implementation.Charts
{
  /// <summary>
  /// ChartSeriesCollection - collection of the chart series.
  /// </summary>
  public class ChartSeriesCollection
    : CollectionBaseEx<IChartSerie>
    , IChartSeries
    , ICloneParent
    , IList<IChartSerie>
  {
    #region Class constants
    /// <summary>
    /// Start of the default series name.
    /// </summary>
    public const string DEF_START_SERIE_NAME = "Serie";
    #endregion

    #region Class members
    /// <summary>
    /// Represents parent chart.
    /// </summary>
    private ChartImpl m_chart;
    /// <summary>
    /// Represents record storage for serialize error bars and trend lines. Use only for serialize.
    /// </summary>
    private IList<IBiffStorage> m_arrTrendError = new List<IBiffStorage>();
    /// <summary>
    /// Array that helps to serialize trendlines labels. Use only for serialize.
    /// </summary>
    private IList<IBiffStorage> m_arrTrendLabels = new List<IBiffStorage>();
    /// <summary>
    /// Represents summary index for error bars and trends. Use only for serialize.
    /// </summary>
    private int m_trendErrorBarsIndex;
    /// <summary>
    /// Represents trend line index.
    /// </summary>
    private int m_trendsIndex;
    private List<IChartSerie> m_additionOrder = new List<IChartSerie>();
    /// <summary>
    /// Returns True, when creating the chart serie.
    /// </summary>
    private bool m_bIsSerieCreating;
    #endregion

    #region Class constructors
    /// <summary>
    /// Creates collection.
    /// </summary>
    /// <param name="application">Application object for the collection.</param>
    /// <param name="parent">Parent object for the collection.</param>
    public ChartSeriesCollection( IApplication application, object parent )
      : base( application, parent )
    {
      m_chart = ( ChartImpl )FindParent( typeof( ChartImpl ) );

      if( m_chart == null )
        throw new ApplicationException( "cannot find parent chart." );
    }
    #endregion

    #region IChartSeries Members
    /// <summary>
    /// Returns a single Name object from a Names collection.
    /// </summary>
    public IChartSerie this[ int index ]
    {
      get
      {
        return (IChartSerie) List[ index ];
      }
      set
      {
        throw new NotSupportedException();
      }
    }

    /// <summary>
    /// Returns a single Name object from a Names collection.
    /// </summary>
    public IChartSerie this[ string name ]
    {
      get
      {
        for( int i = 0, iLen = Count; i < iLen; i++ )
        {
          IChartSerie serie = ( IChartSerie )List[ i ];

          if( serie.Name == name )
            return serie;
        }

        return null;
      }
    }
    /// <summary>
    /// Defines a new series.
    /// </summary>
    /// <returns> Returns a Series object.</returns>
    public IChartSerie Add()
    {
      ChartSerieImpl serie = new ChartSerieImpl( Application, this );
      //serie.SetDefaultName( GetDefSerieName() );
      serie.IsDefaultName = true;
      return Add( serie );
    }
    /// <summary>
    /// Defines a new series. Returns a Series object.
    /// </summary>
    /// <param name="name">Name of the new series.</param>
    /// <returns>Newly created series object.</returns>
    public IChartSerie Add( string name )
    {
      ChartSerieImpl serie = new ChartSerieImpl( Application, this );
      serie.Name = name;

      if ( m_chart.ChartTitle == null )
        m_chart.ChartTitle = name;

      return Add( serie );
    }
    /// <summary>
    /// Defines a new series. Returns a Series object.
    /// </summary>
    /// <param name="serieType">Type of new series.</param>
    /// <returns>Newly created series object.</returns>
    public IChartSerie Add( ExcelChartType serieType )
    {
      ChartSerieImpl serie = ( ChartSerieImpl )Add();

      serie.ChangeSeriesType( serieType, true );

      return serie;
    }
    /// <summary>
    /// Defines a new series. Returns a Series object.
    /// </summary>
    /// <param name="name">Name of the new series.</param>
    /// <param name="serieType">Type of new series.</param>
    /// <returns>Newly created series object.</returns>
    public IChartSerie Add( string name, ExcelChartType serieType )
    {
      IChartSerie serie = Add( name );

        IsSerieCreating = true;
        serie.SerieType = serieType;
        IsSerieCreating = false;
        return serie;
    }
    /// <summary>
    /// Removes Series object from the collection.
    /// </summary>
    /// <param name="index">Index of the series to remove.</param>
    new public void RemoveAt( int index )
    {
      int iCount = List.Count;

      if( index < 0 || index >= iCount )
        throw new ArgumentOutOfRangeException( "index" );

      ChartSerieImpl serie = ( ChartSerieImpl )this[ index ];

      //Updates legend entries.
      if( m_chart.HasLegend )
      {
        ChartLegendEntriesColl entries = ( ChartLegendEntriesColl )m_chart.Legend.LegendEntries;
        entries.Remove( index );
      }

      base.RemoveAt( index );
      bool bFlag = GetCountOfSeriesWithSameDrawingOrder( serie.ChartGroup ) == 0 && iCount != 1;

      if( bFlag )
        m_chart.RemoveFormat( serie.GetCommonSerieFormat() );

      UpdateSerieIndexAfterRemove( index );

      if (!HasSecondary())
      {
          m_chart.RemoveSecondaryAxes();
      }

    }
    /// <summary>
    /// Removes series by name.
    /// </summary>
    /// <param name="serieName">Series name to remove.</param>
    public void Remove( string serieName )
    {
      if( serieName == null )
        throw new ArgumentException( "serieName" );

      for( int i = 0, iLen = Count; i < iLen; i++ )
      {
        ChartSerieImpl serie = ( ChartSerieImpl )List[ i ];

        if( serie.Name == serieName )
        {
          RemoveAt( serie.Index );

          i--;
          iLen--;
        }
      }
    }
    #endregion

    #region Parse methods
    /// <summary>
    /// Parses ChartSiIndex records.
    /// </summary>
    /// <param name="data">Array of records containing ChartSiIndex records.</param>
    /// <param name="iPos">Position of the first ChartSiIndex record.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When specified record is not ChartSiIndex record.
    /// </exception>
    public void ParseSiIndex( IList<BiffRecordRaw> data, ref int iPos )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      BiffRecordRaw record = ( BiffRecordRaw )data[ iPos ];

      if( record.TypeCode != TBIFFRecord.ChartSiIndex )
        throw new ArgumentOutOfRangeException( "ChartSiIndex record was expected." );

      int siIndex = ( ( ChartSiIndexRecord )record ).NumIndex;
      iPos++;

      record = ( BiffRecordRaw )data[ iPos ];

      while( record.TypeCode == TBIFFRecord.Number
        || record.TypeCode == TBIFFRecord.Label
        || record.TypeCode == TBIFFRecord.Blank )
      {
        ICellPositionFormat format = ( ICellPositionFormat )record;
        iPos++;
        record = ( BiffRecordRaw )data[ iPos ];

        if( format.Column >= Count )
          continue;

        AddEnteredRecord( siIndex, format );
      }
    }
    #endregion

    #region Serialize methods
    /// <summary>
    /// Serializes collection.
    /// </summary>
    /// <param name="records">OffsetArrayList that will receive all records.</param>
    /// <exception cref="System.ArgumentNullException">
    /// When specified OffsetArrayList is NULL.
    /// </exception>
    [ CLSCompliant( false ) ]
    public void Serialize( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      m_arrTrendError.Clear();
      m_arrTrendLabels.Clear();
      m_trendErrorBarsIndex = Count;
      m_trendsIndex = Count;

      foreach( ChartSerieImpl serie in List )
      {
        serie.Serialize( records );
      }

      records.AddRange( m_arrTrendError );
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

      for( int i = 0, len = Count; i < len; i++ )
      {
        ChartSerieImpl series = ( ChartSerieImpl )InnerList[ i ];
        series.SerializeDataLabels( records );
      }
    }
    #endregion
  
    #region Class methods
    /// <summary>
    /// Adds series to the collection.
    /// </summary>
    /// <param name="serieToAdd">Series that should be added to the collection.</param>
    /// <returns>Series that was added.</returns>
    public IChartSerie Add( ChartSerieImpl serieToAdd )
    {
      if( serieToAdd == null )
        throw new ArgumentNullException( "serieToAdd" );

      //if( serieToAdd.Name == null || serieToAdd.Name.Length == 0 )
      if( serieToAdd.IsDefaultName )
        serieToAdd.SetDefaultName( GetDefSerieName() );

      base.Add( serieToAdd );
      serieToAdd.Index = List.Count - 1;

      if( !m_chart.ParentWorkbook.Loading )
      {
        serieToAdd.Number = serieToAdd.Index;
        m_additionOrder.Clear();
      }
      else
      {
        m_additionOrder.Add( serieToAdd );
      }

      return serieToAdd;
    }
    /// <summary>
    /// Performs additional operations before the Clear method.
    /// </summary>
    protected override void OnClear()
    {
      base.OnClear ();
    }

    /// <summary>
    /// Clone current instance.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    /// <param name="hashNewNames">Hash table with new Worksheet names.</param>
    /// <param name="dicFontIndexes">Dictionary with new font indexes.</param>
    /// <returns>Returns cloned instance.</returns>
    public ChartSeriesCollection Clone( object parent, Dictionary<string, string> hashNewNames,
      Dictionary<int, int> dicFontIndexes )
    {
      ChartSeriesCollection result = new ChartSeriesCollection( Application, parent );

      for( int i = 0, len = InnerList.Count; i < len; i++ )
      {
        ChartSerieImpl toClone = InnerList[ i ] as ChartSerieImpl;
        ChartSerieImpl impl = toClone.Clone( result, hashNewNames, dicFontIndexes );

        result.Add( impl );
      }

      return result;
    }
    /// <summary>
    /// Clone current instance.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    /// <returns>Returns cloned instance.</returns>
    public override object Clone( object parent )
    {
      ChartSeriesCollection result = new ChartSeriesCollection( Application, parent );

      List<IChartSerie>list = InnerList;
      for( int i = 0, len = list.Count; i < len; i++ )
      {
        ChartSerieImpl toClone = list[ i ] as ChartSerieImpl;
        ChartSerieImpl impl = toClone.Clone( result, null, null );
        result.Add( impl );
      }

      return result;
    }
    /// <summary>
    /// Gets count of series that has same chart group index.
    /// </summary>
    /// <param name="order">Parameter to check.</param>
    /// <returns>Returns count of found series.</returns>
    public int GetCountOfSeriesWithSameDrawingOrder( int order )
    {
      int result = 0;

      for( int i = 0, iLen = List.Count; i < iLen; i++ )
      {
        if( ( ( ChartSerieImpl )List[ i ] ).ChartGroup == order )
          result++;
      }

      return result;
    }
    /// <summary>
    /// Gets series that has same chart group index.
    /// </summary>
    /// <param name="order">Parameter to check.</param>
    /// <returns>Returns list of found series.</returns>
    public List<ChartSerieImpl> GetSeriesWithDrawingOrder( int order )
    {
      List<ChartSerieImpl> result = new List<ChartSerieImpl>();

      for( int i = 0, iLen = List.Count; i < iLen; i++ )
      {
        ChartSerieImpl series = ( ChartSerieImpl )List[ i ];

        if( series.ChartGroup == order )
          result.Add( series );
      }

      return result;
    }
    /// <summary>
    /// Gets count of series with same type in collection.
    /// </summary>
    /// <param name="type">Current type.</param>
    /// <returns>Returns count of same types.</returns>
    public int GetCountOfSeriesWithSameType( ExcelChartType type, bool usePrimaryAxis )
    {
      int result = 0;

      for( int i = 0, iLen = List.Count; i < iLen; i++ )
      {
        ChartSerieImpl series = ( ChartSerieImpl )List[ i ];

        if( series.SerieType == type && series.UsePrimaryAxis == usePrimaryAxis )
          result++;
      }

      return result;
    }
    /// <summary>
    /// Gets count of series with same type in collection.
    /// </summary>
    /// <param name="type">Current type.</param>
    /// <returns>Returns count of same start types.</returns>
    public int GetCountOfSeriesWithSameStartType( ExcelChartType type )
    {
      string strType = ChartFormatImpl.GetStartSerieType( type );
      int result = 0;

      for( int i = 0, iLen = List.Count; i < iLen; i++ )
      {
        if( ( ( ChartSerieImpl )List[ i ] ).StartType == strType )
          result++;
      }

      return result;
    }
    /// <summary>
    /// Sets for default all series in chart.
    /// </summary>
    public void ClearSeriesForChangeChartType()
    {
      for( int i = 0; i < Count; i++ )
      {
        ChartSerieImpl serie = ( ( ChartSerieImpl )List[ i ] );

        serie.ChartGroup = 0;
        ( ( ChartDataPointsCollection )serie.DataPoints ).Clear();
      }
    }
    /// <summary>
    /// Finds order by series type.
    /// </summary>
    /// <param name="type">Type to find.</param>
    /// <returns>Returns order.</returns>
    public int FindOrderByType( ExcelChartType type )
    {
      Dictionary<string, object> hashFormats = new Dictionary<string, object>();
      Dictionary<int, object> hashSerieOrders = new Dictionary<int, object>( 5 );
      string strStartType;

      for( int i = 0; i < Count; i++ )
      {
        ChartSerieImpl serie = ( ChartSerieImpl )List[ i ];
        int iSerieOrder = serie.ChartGroup;

        if( !hashSerieOrders.ContainsKey( iSerieOrder ) )
        {
          hashSerieOrders.Add( iSerieOrder, null );
          strStartType = ChartFormatImpl.GetStartSerieType( serie.SerieType );
          hashFormats[ strStartType ] = null;
        }
      }

      strStartType =  ChartFormatImpl.GetStartSerieType( type );

      int result = 0;

      for( int i = 0, iLen = ChartImpl.DEF_PRIORITY_START_TYPES.Length; i < iLen; i++ )
      {
        string strStartPriority = ChartImpl.DEF_PRIORITY_START_TYPES[ i ];

        if( strStartType == strStartPriority )
          return result;

        if( hashFormats.ContainsKey( strStartPriority ) )
          result++;
      }

      throw new ApplicationException( "Cannot find order." );
    }
    /// <summary>
    /// Updates data points for create one of Pyramid, Cylinder, Cone chart types.
    /// </summary>
    /// <param name="baseFormat">Base format for update.</param>
    /// <param name="topFormat">Top format for update.</param>
    public void UpdateDataPointForCylConePurChartType( ExcelBaseFormat baseFormat
      , ExcelTopFormat topFormat )
    {
      for( int i = 0; i < Count; i++ )
      {
        IChartSerieDataFormat dataFormat =
          ( ( ChartSerieImpl )List[ i ] ).DataPoints.DefaultDataPoint.DataFormat;

        dataFormat.BarShapeBase = baseFormat;
        dataFormat.BarShapeTop = topFormat;
      }
    }
    /// <summary>
    /// Adds record in some collection in some series.
    /// </summary>
    /// <param name="siIndex">Collection index.</param>
    /// <param name="record">Record to add.</param>
    private void AddEnteredRecord( int siIndex, ICellPositionFormat record )
    {
      if( siIndex > 3 || siIndex < 1 )
        throw new ArgumentOutOfRangeException( "siIndex" );

      if( record == null )
        throw new ArgumentNullException( "record" );

      ChartSerieImpl serie = ( ChartSerieImpl )List[ record.Column ];

      serie.AddEnteredRecord( siIndex, record );
    }
    /// <summary>
    /// Returns array of entered records.
    /// </summary>
    /// <param name="siIndex">Si record index.</param>
    /// <returns>Returns array of entered records.</returns>
    public List<BiffRecordRaw> GetEnteredRecords( int siIndex )
    {
      if( siIndex > 3 || siIndex < 1 )
        throw new ArgumentOutOfRangeException( "siIndex" );

      List<BiffRecordRaw> result = new List<BiffRecordRaw>();
      List<List<BiffRecordRaw>> arrays = GetArrays( siIndex );

      if( arrays == null )
        return null;

      int count = arrays[ 0 ].Count;

      for( int i = 1, iLen = arrays.Count; i < iLen; i++ )
      {
        count = Math.Max( count, arrays[ i ].Count );
      }

      for( int i = 0; i < count; i++ )
      {
        for( int j = 0, iLen = arrays.Count; j < iLen; j++ )
        {
          List<BiffRecordRaw> list = arrays[ j ];

          if( list.Count > i )
            result.Add( list[ i ] );
        }
      }

      return result;
    }
    /// <summary>
    /// Gets array by si index.
    /// </summary>
    /// <param name="siIndex">Si index.</param>
    /// <returns>Returns array of arrays by si index.</returns>
    private List<List<BiffRecordRaw>> GetArrays( int siIndex )
    {
      List<List<BiffRecordRaw>> result = new List<List<BiffRecordRaw>>();

      for( int i = 0; i < Count; i++ )
      {
        ChartSerieImpl serie = ( ChartSerieImpl )List[ i ];
        List<BiffRecordRaw> list = serie.GetArray( siIndex );

        if( list != null )
          result.Add( list );
      }

      if( result.Count == 0 )
        return null;

      return result;
    }
    /// <summary>
    /// Updates series index after remove
    /// </summary>
    /// <param name="iRemoveIndex">Remove index.</param>
    public void UpdateSerieIndexAfterRemove( int iRemoveIndex )
    {
      if( iRemoveIndex < 0 || iRemoveIndex > List.Count )
        throw new ArgumentOutOfRangeException( "iRemoveIndex" );

      for( int i = iRemoveIndex, ilen = List.Count; i < ilen; i++ )
      {
        ChartSerieImpl serie = ( ChartSerieImpl )List[ i ];

        serie.Index -= 1;
      }
    }
    /// <summary>
    /// Gets series type by order.
    /// </summary>
    /// <param name="iOrder">Current order.</param>
    /// <returns>Returns found type.</returns>
    public ExcelChartType GetTypeByOrder( int iOrder )
    {
      for( int i = 0, iLen = Count; i < iLen; i++ )
      {
        ChartSerieImpl serie = ( ChartSerieImpl )List[ i ];

        if( serie.ChartGroup == iOrder )
          return serie.SerieType;
      }

      throw new ArgumentOutOfRangeException( "iOrder" );
    }
    /// <summary>
    /// Clears all series data formats.
    /// </summary>
    /// <param name="format">Represents format to update.</param>
    public void ClearDataFormats( ChartSerieDataFormatImpl format )
    {
      for( int i = 0, iLen = List.Count; i < iLen; i++ )
      {
        ChartSerieImpl serie = ( ChartSerieImpl )List[ i ];
        ChartDataPointsCollection points = ( ChartDataPointsCollection )serie.DataPoints;

        points.ClearDataFormats( format );
      }
    }
    /// <summary>
    /// Gets default series name.
    /// </summary>
    /// <returns>Returns default series name.</returns>
    public string GetDefSerieName()
    {
      return GenerateDefaultName( List, DEF_START_SERIE_NAME );
    }
    /// <summary>
    /// Gets default series name.
    /// </summary>
    /// <param name="iSerieIndex">Represents series index in collections.</param>
    /// <returns>Returns default series name.</returns>
    public string GetDefSerieName( int iSerieIndex )
    {
      int iCount = List.Count;

      if( iSerieIndex > iCount || iSerieIndex < 0 )
        throw new ArgumentOutOfRangeException( "iSerieIndex" );

      IList<IChartSerie> arrList;

      if( iSerieIndex == List.Count )
      {
        arrList = List;
      }
      else
      {
       arrList = new List<IChartSerie>( iSerieIndex );

        for( int i = 0; i < iSerieIndex; i++ )
        {
          arrList.Add( List[ i ] );
        }
      }

      return GenerateDefaultName( arrList, DEF_START_SERIE_NAME );
    }
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
      List<IChartSerie> list = InnerList;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        ChartSerieImpl serie = ( ChartSerieImpl )list[ i ];
        serie.UpdateFormula( iCurIndex, iSourceIndex, sourceRect, iDestIndex, destRect );
      }
    }
    /// <summary>
    /// Returns legend offset.
    /// </summary>
    /// <param name="iSerIndex">Represents series index.</param>
    /// <returns>Returns legend offset.</returns>
    public int GetLegendEntryOffset( int iSerIndex )
    {
      if( iSerIndex >= Count )
        throw new ArgumentOutOfRangeException( "iSerIndex" );

      int result = 0;

      for( int i = 0; i < iSerIndex; i++ )
      {
        ChartSerieImpl serie = ( ChartSerieImpl )List[ i ];
        result += serie.TrendLines.Count;
      }

      return result + Count;
    }
    /// <summary>
    /// Assign trend data label.
    /// </summary>
    /// <param name="area">Represents data label.</param>
    public void AssignTrendDataLabel( ChartTextAreaImpl area )
    {
      if( area == null )
        throw new ArgumentNullException( "area" );

      for( int i = 0, iLen = Count; i < iLen; i++ )
      {
        IChartSerie serie = ( IChartSerie )List[ i ];
        IChartTrendLines trends = serie.TrendLines;

        for( int j = 0, iCount = trends.Count; j < iCount; j++ )
        {
          ChartTrendLineImpl trend = ( ChartTrendLineImpl )trends[ j ];

          if( trend.Index == area.ObjectLink.SeriesNumber )
          {
            trend.SetDataLabel( area );

            return;
          }
        }
      }
    }
    /// <summary>
    /// Clears error bars.
    /// </summary>
    internal void ClearErrorBarsAndTrends()
    {
      for( int i = 0, iLen = Count; i < iLen; i++ )
      {
        IChartSerie serie = ( IChartSerie )List[ i ];
        IChartTrendLines trends = serie.TrendLines;

        serie.HasErrorBarsX = false;
        serie.HasErrorBarsY = false;

        trends.Clear();
      }
    }
    /// <summary>
    /// Puts all series into correct order based on their index.
    /// </summary>
    /// <param name="dictSeriesAxis">Dictionary containing series axis.</param>
    internal void ResortSeries( Dictionary<int, int> dictSeriesAxis )
    {
      int iCount = Count;

#if DEBUG
      ChartImpl chart = m_chart;
#endif

      if( iCount > 1 )
      {
        List<IChartSerie> list = InnerList;
        SortedList<int, ChartSerieImpl> sortedList =
          new SortedList<int, ChartSerieImpl>();

        for( int i = 0; i < iCount; i++ )
        {
          ChartSerieImpl series = ( ChartSerieImpl )list[ i ];
          int iCorrectIndex = series.Index;
          sortedList.Add( iCorrectIndex, series );
        }

        IList<ChartSerieImpl> values = sortedList.Values;
        Dictionary<int, int> dictIndexesChange = new Dictionary<int, int>();
        int iOldIndex;

        for( int i = 0; i < iCount; i++ )
        {
          //sortedList.Values
          ChartSerieImpl series = values[ i ];
          list[ i ] = series;

          iOldIndex = series.Index;
          dictIndexesChange[ i ] = iOldIndex;
          series.Index = i;
        }

        for( int i = 0; i < iCount; i++ )
        {
          iOldIndex = dictIndexesChange[ i ];

          int iAxisId;

          if( dictSeriesAxis.TryGetValue( iOldIndex, out iAxisId ) )
          {
            ChartAxisImpl category = m_chart.PrimaryCategoryAxis as ChartAxisImpl;
            ChartAxisImpl value = m_chart.PrimaryValueAxis as ChartAxisImpl;

            if( iAxisId != category.AxisId && iAxisId != value.AxisId )
            {
              ChartSerieImpl series = ( ChartSerieImpl )list[ i ];
              series.UsePrimaryAxis = false;
            }
          }
        }
      }
    }
    /// <summary>
    /// Sets items with used reference indexes to true.
    /// </summary>
    /// <param name="usedItems">Array to mark used references in.</param>
    public void MarkUsedReferences( bool[] usedItems )
    {
      List<IChartSerie> list = InnerList;

      for( int i = 0, len = Count; i < len; i++ )
      {
        ChartSerieImpl series = ( ChartSerieImpl )list[ i ];
        series.MarkUsedReferences( usedItems );
      }
    }
    /// <summary>
    /// Updates reference indexes.
    /// </summary>
    /// <param name="arrUpdatedIndexes">Array with updated indexes.</param>
    public void UpdateReferenceIndexes( int[] arrUpdatedIndexes )
    {
      List<IChartSerie> list = InnerList;

      for( int i = 0, len = Count; i < len; i++ )
      {
        ChartSerieImpl series = ( ChartSerieImpl )list[ i ];
        series.UpdateReferenceIndexes( arrUpdatedIndexes );
      }
    }
    internal List<IChartSerie> AdditionOrder
    {
      get
      {
        return m_additionOrder;
      }
    }
    internal bool HasSecondary()
    {
        for (int i = 0; i < Count; i++)
            if (!this[i].UsePrimaryAxis)
                return true;
        return false;
    }
    #endregion

    #region Class helper properties
    /// <summary>
    /// Represents record storage for serialize error bars and trend lines. Use only for serialize.
    /// </summary>
    internal IList<IBiffStorage> TrendErrorList
    {
      get
      {
        return m_arrTrendError;
      }
    }
    /// <summary>
    /// Represents summary index for error bars and trends. Use only for serialize.
    /// </summary>
    internal int TrendErrorBarIndex
    {
      get
      {
        return m_trendErrorBarsIndex;
      }
      set
      {
        m_trendErrorBarsIndex = value;
      }
    }
    /// <summary>
    /// Represents record storage for serialize trend labels. Use only for serialize. Read-only.
    /// </summary>
    internal IList<IBiffStorage> TrendLabels
    {
      get
      {
        return m_arrTrendLabels;
      }
    }
    /// <summary>
    /// Represents count of trends + series count.
    /// </summary>
    internal int TrendIndex
    {
      get
      {
        return m_trendsIndex;
      }
      set
      {
        m_trendsIndex = value;
      }
    }
    /// <summary>
    /// Returns True, when creating the chart serie.
    /// </summary>
    internal bool IsSerieCreating
    {
        get
        {
            return m_bIsSerieCreating;
        }
        set
        {
            m_bIsSerieCreating = value;
        }
    }


    #endregion
  }
}
