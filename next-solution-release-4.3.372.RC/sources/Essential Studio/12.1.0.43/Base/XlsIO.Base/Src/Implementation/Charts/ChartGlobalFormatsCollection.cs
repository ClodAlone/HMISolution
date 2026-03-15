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

using Syncfusion.XlsIO.Parser.Biff_Records.Charts;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Implementation.Exceptions;
using System.Collections.Generic;
#endregion

namespace Syncfusion.XlsIO.Implementation.Charts
{
	/// <summary>
	/// Contains primary parent axis, secondary parent axis.
	/// </summary>
  public class ChartGlobalFormatsCollection
  {
    #region Class constants
    /// <summary>
    /// 
    /// </summary>
    public static readonly ExcelChartType[] DEF_MABY_COMBINATION_TYPES =
    {
      ExcelChartType.Scatter_Markers,
      ExcelChartType.Scatter_Line_Markers,
      ExcelChartType.Scatter_Line,
      ExcelChartType.Scatter_SmoothedLine_Markers,
      ExcelChartType.Scatter_SmoothedLine,
      ExcelChartType.Line,
      ExcelChartType.Line_3D,
      ExcelChartType.Line_Markers,
      ExcelChartType.Line_Markers_Stacked,
      ExcelChartType.Line_Markers_Stacked_100,
      ExcelChartType.Line_Stacked,
      ExcelChartType.Line_Stacked_100,
      ExcelChartType.Bubble,
      ExcelChartType.Bubble_3D,
      ExcelChartType.Radar_Markers,
      ExcelChartType.Radar
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly string[] DEF_MABY_COMBINATION_TYPES_START =
    {
      ChartImpl.START_SCATTER,
      ChartImpl.START_LINE,
      ChartImpl.START_BUBBLE,
      ChartImpl.START_RADAR,
    };
    #endregion

    #region Class members
    /// <summary>
    /// Represents primary parent axis.
    /// </summary>
    private ChartFormatCollection m_primary;
    /// <summary>
    /// Represents secondary parent axis.
    /// </summary>
    private ChartFormatCollection m_secondary;
    #endregion

    #region Class constructors
    /// <summary>
    /// Default constructor.
    /// </summary>
    public ChartGlobalFormatsCollection()
    {}
    /// <summary>
    /// Creates object and initializes collections.
    /// </summary>
    /// <param name="application">Application object.</param>
    /// <param name="primaryParent">Parent object for primary collection.</param>
    /// <param name="secondaryParent">Parent object for secondary collection.</param>
    public ChartGlobalFormatsCollection( IApplication application
      , ChartParentAxisImpl primaryParent, ChartParentAxisImpl secondaryParent )
    {
      m_primary = new ChartFormatCollection( application, primaryParent );
      m_secondary = new ChartFormatCollection( application, secondaryParent );
    }
    #endregion

    #region Parse methods
    /// <summary>
    /// Parses ChartAxisParent record.
    /// </summary>
    /// <param name="data">Array of records containing record.</param>
    /// <param name="iPos">Position of the record to parse.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When specified record is not ChartAxisParent record.
    /// </exception>
    public void Parse( IList data, ref int iPos )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

    }
    #endregion

    #region Serialize methods
    /// <summary>
    /// Serializes chart axes.
    /// </summary>
    /// <param name="records">
    /// OffsetArrayList that will receive all records.
    /// </param>
    [ CLSCompliant( false ) ]
    public void Serialize( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Represents primary format collection. Read-only.
    /// </summary>
    public ChartFormatCollection PrimaryFormats
    {
      get
      {
        return m_primary;
      }
    }
    /// <summary>
    /// Represents secondary format collection. Read-only.
    /// </summary>
    public ChartFormatCollection SecondaryFormats
    {
      get
      {
        return m_secondary;
      }
    }
    #endregion

    #region Class methods
    /// <summary>
    /// If can - removes format.
    /// </summary>
    /// <param name="format">Format to remove.</param>
    public void Remove( ChartFormatImpl format )
    {
      if( format == null )
        throw new ArgumentNullException( "format" );

      int iOrder = format.DrawingZOrder;

      if( m_primary.ContainsIndex( iOrder ) )
      {
        if( m_primary.Count == 1 )
        {
          ChangeCollections();
          m_secondary.Remove( format );

          return;
        }

        m_primary.Remove( format );

        return;
      }

      if( m_secondary.ContainsIndex( iOrder ) ) m_secondary.Remove( format );
    }
    /// <summary>
    /// Creates format collection.
    /// </summary>
    /// <param name="application">Application object.</param>
    /// <param name="parent">Parent object for collection.</param>
    /// <param name="bIsPrimary">If true - primary collection; otherwise - secondary.</param>
    public void CreateCollection( IApplication application, object parent, bool bIsPrimary )
    {
      if( bIsPrimary )
      {
        m_primary = new ChartFormatCollection( application, parent );
      }
      else
      {
        m_secondary = new ChartFormatCollection( application, parent );
      }
    }
    /// <summary>
    /// Changes primary and secondary format collections.
    /// </summary>
    public void ChangeCollections()
    {
      ChartFormatCollection primary = m_primary;
      object primaryParent = m_primary.Parent;
      object secondaryParent = m_secondary.Parent;

      m_primary = m_secondary;
      m_primary.SetParent( primaryParent );
      m_primary.SetParents();

      m_secondary = primary;
      m_secondary.SetParent( secondaryParent );
      m_secondary.SetParents();
    }
    /// <summary>
    /// Helper methods for adding new formats.
    /// </summary>
    /// <param name="formatToAdd">Format to add.</param>
    /// <param name="order">Format order.</param>
    /// <param name="index">Format index.</param>
    /// <param name="isPrimary">If true - adds in primary format; otherwise - in secondary.</param>
    /// <returns></returns>
    public ChartFormatImpl AddFormat( ChartFormatImpl formatToAdd, int order
      , int index, bool isPrimary )
    {
      ChartFormatCollection coll = GetCurrentCollection( isPrimary );

      if( !( m_primary.ContainsIndex( order ) || m_secondary.ContainsIndex( order ) ) )
      {
        coll.SetIndex( order, index );

        return formatToAdd;
      }

      for( int i = ChartFormatCollection.DEF_ARRAY_CAPACITY - 1; i >= order; i-- )
      {
        if( m_primary.ContainsIndex( i ) )
        {
          m_primary.UpdateFormatsOnAdding( i );
        }
        else if( m_secondary.ContainsIndex( i ) )
        {
          m_secondary.UpdateFormatsOnAdding( i );
        }
      }

      coll.SetIndex( order, index );

      return formatToAdd;
    }
    /// <summary>
    /// Removes format from collection.
    /// </summary>
    /// <param name="indexToRemove">Index to remove.</param>
    /// <param name="iOrder">Order to remove.</param>
    /// <param name="isPrimary">If true - removes in primary collection; otherwise - secondary.</param>
    public void RemoveFormat( int indexToRemove, int iOrder, bool isPrimary )
    {
      ChartFormatCollection coll = GetCurrentCollection( isPrimary );

      if( isPrimary )
      {
        m_primary.UpdateIndexesAfterRemove( indexToRemove );
      }
      else
      {
        m_secondary.UpdateIndexesAfterRemove( indexToRemove );
      }

      for( int i = iOrder + 1; i < ChartFormatCollection.DEF_ARRAY_CAPACITY; i++ )
      {
        if( m_primary.ContainsIndex( i ) )
        {
          m_primary.UpdateFormatsOnRemoving( i );
        }
        else if( m_secondary.ContainsIndex( i ) )
        {
          m_secondary.UpdateFormatsOnRemoving( i );
        }
      }
    }
    /// <summary>
    /// Returns collection by parameter.
    /// </summary>
    /// <param name="isPrimary">If true  - returns primary collection; otherwise - secondary.</param>
    /// <returns>Returns one of collection.</returns>
    private ChartFormatCollection GetCurrentCollection( bool isPrimary )
    {
      return ( isPrimary ) ? m_primary : m_secondary;
    }
    /// <summary>
    /// Clones current instance.
    /// </summary>
    /// <param name="parent">Parent for primary collection.</param>
    /// <returns>Returns just cloned method.</returns>
    public ChartGlobalFormatsCollection CloneForPrimary( object parent )
    {
      ChartGlobalFormatsCollection result = new ChartGlobalFormatsCollection();

      if( m_primary != null )
        result.m_primary = ( ChartFormatCollection )m_primary.Clone( parent );
      
      return result;
    }
    /// <summary>
    /// Clones secondary collection.
    /// </summary>
    /// <param name="result">Global collection.</param>
    /// <param name="parent">Parent object.</param>
    public void CloneForSecondary( ChartGlobalFormatsCollection result, object parent )
    {
      if( result == null )
        throw new ArgumentNullException( "result" );

      if( m_secondary != null )
        result.m_secondary = ( ChartFormatCollection )m_secondary.Clone( parent );
    }
    /// <summary>
    /// Detects chart type.
    /// </summary>
    /// <param name="series">Chart series collection.</param>
    /// <returns>Returns chart type.</returns>
    public ExcelChartType DetectChartType( ChartSeriesCollection series )
    {
      if( series == null )
        throw new ArgumentNullException( "series" );

      int iPrimaryCount = m_primary.Count;
      int iSeriesCount = series.Count;

      ExcelChartType result = ExcelChartType.Combination_Chart;

      if( iPrimaryCount == 0 )
      {
        result = ChartImpl.DEFAULT_CHART_TYPE;
      }
      else if( iPrimaryCount > 1 )
      {
        result = ExcelChartType.Combination_Chart;
      }
      else if( m_secondary.Count == 0 )
      {
        result = DetectTypeForPrimaryCollOnly( series );
      }
      else
      {
        if( iSeriesCount >= 4 && iSeriesCount <= 5 && SecondaryFormats.ContainsIndex( 1 ) )
        {
          ChartFormatImpl format = SecondaryFormats[ 1 ];

          if( format != null && format.IsChartChartLine && format.FormatRecordType == TBIFFRecord.ChartLine
               && format.LineStyle == ExcelDropLineStyle.HiLow )
          {
            result = format.IsDropBar ? ExcelChartType.Stock_VolumeOpenHighLowClose
              : ExcelChartType.Stock_VolumeHighLowClose;
          }
        }
      }

      return result;
    }
    /// <summary>
    /// Detects chart type by primary formats only.
    /// </summary>
    /// <param name="series">Chart series collection.</param>
    /// <returns>Returns chart type.</returns>
    private ExcelChartType DetectTypeForPrimaryCollOnly( ChartSeriesCollection series )
    {
      if( series == null )
        throw new ArgumentNullException( "series" );

      if( m_secondary.Count != 0 )
        throw new ApplicationException( "Can't detect chart type" );

      int iSeriesCount = series.Count;
      ChartFormatImpl format = ( ChartFormatImpl )m_primary[ 0 ];

      bool flag = iSeriesCount >= 3 && iSeriesCount <= 4 && format.FormatRecordType ==
        TBIFFRecord.ChartLine && format.IsChartLineFormat && format.IsChartChartLine
        && format.LineStyle == ExcelDropLineStyle.HiLow;

      if( flag )
      {
        return ( format.IsDropBar )
          ? ExcelChartType.Stock_OpenHighLowClose : ExcelChartType.Stock_HighLowClose;
      }

      ChartSerieImpl firstSerie = series[ 0 ] as ChartSerieImpl;
      string strTypeStart = firstSerie.DetectSerieTypeStart();//SerieType;
      string strTypeString = firstSerie.DetectSerieTypeString();
      ExcelChartType result = ( ExcelChartType )(-1);

      if( ( Array.IndexOf( DEF_MABY_COMBINATION_TYPES_START, strTypeStart ) != -1 ) )
      {
        for( int i = 0; i < iSeriesCount; i++ )
        {
          // TODO: investigate this
          //ChartDataPointsCollection coll = ( ChartDataPointsCollection )series[ i ].DataPoints;
          //ChartSerieDataFormatImpl dataFormat = coll.DefPointFormatOrNull;

          //if( dataFormat != null && dataFormat.ContainsLineProperties )
          //  return ExcelChartType.Combination_Chart;

          ChartSerieImpl currentSerie = series[ i ] as ChartSerieImpl;

          if( currentSerie.ChartGroup != firstSerie.ChartGroup ||
            currentSerie.DetectSerieTypeString() != strTypeString )
          {
            result = ExcelChartType.Combination_Chart;
            break;
          }
        }
      }

      if( ( int )result == -1 )
      {
        result = firstSerie.SerieType;
      }

      return result;
    }
    /// <summary>
    /// Clears all format collections.
    /// </summary>
    public void Clear()
    {
      m_primary.Clear();
      m_secondary.Clear();
    }
    /// <summary>
    /// Changes not intimate types.
    /// </summary>
    /// <param name="typeToChange">Type to change.</param>
    /// <param name="serieType">Current Series type.</param>
    /// <param name="application">Application object.</param>
    /// <param name="chart">Chart object.</param>
    /// <returns>Returns format for current type.</returns>
    public ChartFormatImpl ChangeNotIntimateSerieType( ExcelChartType typeToChange
      , ExcelChartType serieType, IApplication application, ChartImpl chart, ChartSerieImpl serieToChange )
    {
      if( application == null )
        throw new ArgumentNullException( "application" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      bool bNeedSecondary = Array.IndexOf( ChartImpl.DEF_NEED_SECONDARY_AXIS, typeToChange ) != -1;
      ChartSeriesCollection seriesColl = ( ChartSeriesCollection )chart.Series;

      ChartFormatImpl format = bNeedSecondary
        ? new ChartFormatImpl( application, m_secondary )
        : new ChartFormatImpl( application, m_primary );

      format.ChangeSerieType( typeToChange, false );
      format.DrawingZOrder = seriesColl.FindOrderByType( typeToChange );

      // 1. Find series that have proposed z-order
      ChartSeriesCollection chartSeries = chart.Series as ChartSeriesCollection;
      List<ChartSerieImpl> series = chartSeries.GetSeriesWithDrawingOrder( format.DrawingZOrder );
      
      // 2. If not only one series that is being changed has it then
      bool bCanReplace = ( series.Count == 1 && series[ 0 ] == serieToChange );
      {
        // 3. we should check whether we won't replace anything by adding new format
        // 4. if we are replacing something then we should update existing z-order table (m_arrOrders)
      }

      // 5. Add format to the table.

      if( bNeedSecondary )
      {
        chart.IsSecondaryAxes = true;

        m_secondary.Add( format, bCanReplace );
      }
      else
      {
        if( Array.IndexOf( ChartImpl.DEF_NEED_SECONDARY_AXIS, serieType ) != -1 )
        {
          chart.ChangePrimaryAxis( false );
        }

        m_primary.Add( format, bCanReplace );
      }

      return format;
    }

    /// <summary>
    /// Change format in axis.
    /// </summary>
    /// <param name="bToPrimary">If true - changes format to primary. otherwise - to secondary.</param>
    /// <param name="iOrder">Format order.</param>
    /// <param name="bAdd">If true - standard add; otherwise - shallow.</param>
    /// <param name="iNewOrder">Represents new order.</param>
    public void ChangeShallowAxis( bool bToPrimary, int iOrder, bool bAdd, int iNewOrder )
    {
      if( bToPrimary )
      {
        ChangeInAxis( m_secondary, m_primary, iOrder, iNewOrder, bAdd );
      }
      else
      {
        ChangeInAxis( m_primary, m_secondary, iOrder, iNewOrder, bAdd );
      }
    }
    /// <summary>
    /// Changes format in axis collection.
    /// </summary>
    /// <param name="from">Collection from format gets.</param>
    /// <param name="to">Collection to format sets.</param>
    /// <param name="iOrder">Formats order.</param>
    /// <param name="iNewOrdr">New order to set.</param>
    /// <param name="bAdd">IF true - standard add; otherwise - shallow.</param>
    private void ChangeInAxis( ChartFormatCollection from, ChartFormatCollection to
      , int iOrder, int iNewOrder, bool bAdd )
    {
      ChartFormatImpl format = from.GetFormat( iOrder, !bAdd );
      ChartFormatImpl newFormat = ( ChartFormatImpl )format.Clone( to );

      if( bAdd )
      {
        newFormat.DrawingZOrder = iNewOrder;
        to.Add( newFormat, false );
      }
      else
      {
        to.AddFormat( newFormat );
      }
    }
    #endregion
	}
}
