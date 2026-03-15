#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;

using Syncfusion.XlsIO.Implementation.Collections;
using Syncfusion.XlsIO.Parser.Biff_Records;
using System.Collections.Generic;

using Syncfusion.XlsIO.Implementation.Exceptions;

namespace Syncfusion.XlsIO.Implementation.Charts
{
  /// <summary>
  /// Represents Trend Line Collection.
  /// </summary>
  public class ChartTrendLineCollection
    : CollectionBaseEx<IChartTrendLine>
    , IChartTrendLines
  {
    #region Class members
    /// <summary>
    /// Represents parent series.
    /// </summary>
    private ChartSerieImpl m_parentSerie;
    #endregion

    #region Class initialize methods
    /// <summary>
    /// Creates new instance of collection.
    /// </summary>
    /// <param name="application">Represents current application.</param>
    /// <param name="parent">Represents parent object.</param>
    public ChartTrendLineCollection( IApplication application, object parent )
      : base( application, parent )
    {
      m_parentSerie = ( ChartSerieImpl )FindParent( typeof( ChartSerieImpl ) );

      if( m_parentSerie == null )
        throw new ApplicationException( "Cannot find parent objects." );
    }
    #endregion

    #region IChartTrendLines properties
    /// <summary>
    /// Gets single trend line by index. Read-only
    /// </summary>
    public new IChartTrendLine this[ int iIndex ]
    {
      get
      {
        if( iIndex >= List.Count || iIndex < 0 )
          throw new ArgumentOutOfRangeException( "Index is out of bounds of collection." );

        CheckSeriesType();

        IChartTrendLine line = ( IChartTrendLine )List[ iIndex ];

        if( !IsParsed )
          CheckNegativeValues( line.Type );

        return line;
      }
    }
    #endregion

    #region IChartTrendLines methods
    /// <summary>
    /// Adds new instance of trend line to collection.
    /// </summary>
    /// <returns>Returns added trend line object.</returns>
    public IChartTrendLine Add()
    {
      return Add( ExcelTrendLineType.Linear );
    }
    /// <summary>
    /// Adds new instance of trend line to collection.
    /// </summary>
    /// <param name="type">Represents type of trend line.</param>
    /// <returns>Returns added trend line object.</returns>
    public IChartTrendLine Add( ExcelTrendLineType type )
    {
      CheckSeriesType();
      CheckNegativeValues( type );

      ChartTrendLineImpl trendline = new ChartTrendLineImpl( Application, this );

      trendline.Type = type;
      base.Add( trendline );

      return trendline;
    }
    /// <summary>
    /// Removes trend line object from collection.
    /// </summary>
    /// <param name="index">Represents </param>
    new public void RemoveAt( int index )
    {
      if( index < 0 || index >= Count )
        throw new ArgumentOutOfRangeException( "index" );

      CheckSeriesType();

      base.RemoveAt( index );
    }
    #endregion

    #region Class serialize methods
    /// <summary>
    /// Serialize all trend lines.
    /// </summary>
    /// <param name="records">Represents record holder.</param>
    [ CLSCompliant( false ) ]
    public void Serialize( IList<IBiffStorage> records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      for( int i = 0, iLen = Count; i < iLen; i++ )
      {
        ChartTrendLineImpl trendLine = ( ChartTrendLineImpl )List[ i ];

        trendLine.Serialize( records );
      }
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// Checks if current trend line supported negative values.
    /// </summary>
    /// <param name="type">Represents trend line types.</param>
    private void CheckNegativeValues( ExcelTrendLineType type )
    {
      if( type != ExcelTrendLineType.Power && type != ExcelTrendLineType.Exponential )
        return;

      IRange range = m_parentSerie.Values;
      IRange[] ranges = range.Cells;

      for( int i = 0, iLen = ranges.Length; i < iLen; i++ )
      {
        IRange curRange = ranges[ i ];

        if( curRange.HasNumber && curRange.Number <= 0 )
        {
          throw new NotSupportedException( "Cannot perform current operation becouse one of"
            + "series values is less or equal zero." );
        }
      }
    }
    /// <summary>
    /// Adds trend object to trends collection.
    /// </summary>
    /// <param name="trend">Represents trend object to add.</param>
    public void Add( ChartTrendLineImpl trend )
    {
      if( trend == null )
        throw new ArgumentNullException( "trend" );

      base.Add( trend );
    }
    /// <summary>
    /// Checks series type. If current type does not support trendlines throw exception.
    /// </summary>
    public void CheckSeriesType()
    {
      if( !IsParsed )
      {
        if( Array.IndexOf( ChartImpl.DEF_SUPPORT_TREND_LINES, m_parentSerie.SerieType ) == -1 )
          throw new ArgumentNullException( "Current serie type doesnot support trend lines." );
      }
    }
    /// <summary>
    /// Sets items with used reference indexes to true.
    /// </summary>
    /// <param name="usedItems">Array to mark used references in.</param>
    public void MarkUsedReferences( bool[] usedItems )
    {
      List<IChartTrendLine> list = InnerList;
      for( int i = 0, len = list.Count; i < len; i++ )
      {
        ChartTrendLineImpl trendLine = ( ChartTrendLineImpl )list[ i ];
        trendLine.MarkUsedReferences( usedItems );
      }
    }
    /// <summary>
    /// Updates reference indexes.
    /// </summary>
    /// <param name="arrUpdatedIndexes">Array with updated indexes.</param>
    public void UpdateReferenceIndexes( int[] arrUpdatedIndexes )
    {
      List<IChartTrendLine> list = InnerList;
      for( int i = 0, len = list.Count; i < len; i++ )
      {
        ChartTrendLineImpl trendLine = ( ChartTrendLineImpl )list[ i ];
        trendLine.UpdateReferenceIndexes( arrUpdatedIndexes );
      }
    }
    #endregion

    #region Class propeties
    /// <summary>
    /// Indicates whether object was parsed.
    /// </summary>
    private bool IsParsed
    {
      get
      {
        return m_parentSerie.ParentChart.IsParsed;
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
    public ChartTrendLineCollection Clone( object parent, Dictionary<int, int> dicFontIndexes,
      Dictionary<string, string> dicNewSheetNames )
    {
      if( parent == null )
        throw new ArgumentNullException( "parent" );

      ChartTrendLineCollection result = new ChartTrendLineCollection( Application, parent );

      for( int i = 0, iLen = Count; i < iLen; i++ )
      {
        ChartTrendLineImpl trend = ( ChartTrendLineImpl )List[ i ];

        result.Add( trend.Clone( result, dicFontIndexes, dicNewSheetNames ) );
      }

      return result;
    }
    #endregion
  }
}
