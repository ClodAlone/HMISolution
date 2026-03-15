#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

using System;
using System.Collections;

using Syncfusion.XlsIO.Implementation.Collections;
using System.Collections.Generic;
using Syncfusion.XlsIO.Implementation.Exceptions;

namespace Syncfusion.XlsIO.Implementation.Charts
{
  /// <summary>
  /// Represents collection of ChartLegendEntries in chart legend.
  /// </summary>
  public class ChartLegendEntriesColl
    : CommonObject
    , IChartLegendEntries
  {
    #region Class members
    /// <summary>
    /// Represents global storage for legend entries.
    /// Key - entry index; Value - legend entry.
    /// </summary>
    private Dictionary<int, ChartLegendEntryImpl> m_hashEntries = new Dictionary<int, ChartLegendEntryImpl>();
    /// <summary>
    /// Represents parent chart.
    /// </summary>
    private ChartImpl m_parentChart;
    #endregion

    #region Class constructors
    /// <summary>
    /// Creates new instance of Legend entry collection.
    /// </summary>
    /// <param name="application">Represents current application.</param>
    /// <param name="parent">Parent object.</param>
    public ChartLegendEntriesColl( IApplication application, object parent )
      : base( application, parent )
    {
      SetParents();
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Represents count of legend entries in collection. Read-only.
    /// </summary>
    public int Count
    {
      get
      {
        IChartSeries series = m_parentChart.Series;
        string strStartType = ChartFormatImpl.GetStartSerieType( m_parentChart.ChartType );
        bool bFlag = Array.IndexOf( ChartImpl.DEF_LEGEND_NEED_DATA_POINT, strStartType ) == -1;
        int iCount = 0;

        if( bFlag )
        {
          int result = series.Count;

          for( int i = 0, iLen = series.Count; i < iLen ;i++ )
          {
            IChartSerie serie = series[ i ];
            result += serie.TrendLines.Count;
          }

          iCount = result;
        }
        else
        {
          ChartSerieImpl curSerie = ( ChartSerieImpl )series[ 0 ];

          iCount = curSerie.PointNumber;
        }

        return iCount;
      }
    }
    /// <summary>
    /// Gets legend entry object by index. Read-only.
    /// </summary>
    public IChartLegendEntry this[ int iIndex ]
    {
      get
      {
        if( !m_parentChart.Loading && iIndex >= Count )
          throw new ArgumentOutOfRangeException( "iIndex" );

        if( m_hashEntries.ContainsKey( iIndex ) )
          return m_hashEntries[ iIndex ];

        return Add( iIndex );
      }
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Finds all parent objects.
    /// </summary>
    private void SetParents()
    {
      m_parentChart = ( ChartImpl )FindParent( typeof( ChartImpl ) );

      if( m_parentChart == null )
        throw new ApplicationException( "Can't find parent object." );
    }
    /// <summary>
    /// Adds legend to collection by index.
    /// </summary>
    /// <param name="iIndex">Represents index of new entry in collection.( Zero based )</param>
    /// <returns>Returns added entry.</returns>
    public ChartLegendEntryImpl Add( int iIndex )
    {
      if( m_hashEntries.ContainsKey( iIndex ) )
        return m_hashEntries[ iIndex ];

      ChartLegendEntryImpl entry = new ChartLegendEntryImpl( Application, this, iIndex );

      return Add( iIndex, entry );
    }
    /// <summary>
    /// Adds legend to collection by index.
    /// </summary>
    /// <param name="iIndex">Represents index of new entry in collection.( Zero based )</param>
    /// <param name="entry">Represents entry to add.</param>
    /// <returns>Returns added entry.</returns>
    public ChartLegendEntryImpl Add( int iIndex, ChartLegendEntryImpl entry )
    {
      if( !m_parentChart.Loading && iIndex >= Count )
        throw new ArgumentOutOfRangeException( "iIndex" );

      if( entry == null )
        throw new ArgumentNullException( "entry" );

      entry.Index = iIndex;

      if( !m_parentChart.Loading )
      {
        ExcelChartType type = m_parentChart.ChartType;
        string strType = ChartFormatImpl.GetStartSerieType( type );

        if( Array.IndexOf( ChartImpl.DEF_LEGEND_NEED_DATA_POINT, strType ) != -1 )
          entry.LegendEntityIndex = iIndex;
      }

      if( m_hashEntries.ContainsKey( iIndex ) )
      {
        m_hashEntries[ iIndex ] = entry;
      }
      else
      {
        m_hashEntries.Add( iIndex, entry );
      }

      return entry;
    }
    /// <summary>
    /// Checks for contain changed from default formatting legend entry by index.
    /// </summary>
    /// <param name="iIndex">Represents legend entry index.</param>
    /// <returns>Returns true if contains otherwise false.</returns>
    public bool Contains( int iIndex )
    {
      return m_hashEntries.ContainsKey( iIndex );
    }
    /// <summary>
    /// Checks before deleting legend entry.
    /// </summary>
    /// <param name="iIndex">Represents index in collection.</param>
    /// <returns>If true - can delete; otherwise false.</returns>
    public bool CanDelete( int iIndex )
    {
      if( m_hashEntries.Count != Count )
        return true;

      for( int i = 0, iLen = m_hashEntries.Count; i < iLen; i++ )
      {
        if( i == iIndex )
          continue;

        ChartLegendEntryImpl entry = null;
            
          if(  m_hashEntries.TryGetValue(i,out entry ))
          {

        if( !entry.IsDeleted )
          return true;
          }
      }

      return false;
    }
    /// <summary>
    /// Updates legend entries collection after removing series.
    /// </summary>
    /// <param name="iIndex">Index of legend entry to remove.</param>
    public void Remove( int iIndex )
    {
      int iCount = Count;

      string strType = ChartFormatImpl.GetStartSerieType( m_parentChart.ChartType );

      if( Array.IndexOf( ChartImpl.DEF_LEGEND_NEED_DATA_POINT, strType ) != - 1 )
        return;

      if( iIndex < 0 || iIndex >= iCount )
        throw new ArgumentOutOfRangeException( "iIndex" );

      if( m_hashEntries.ContainsKey( iIndex ) )
        m_hashEntries.Remove( iIndex );

      for( int i = iIndex + 1; i < iCount; i++ )
      {
        if( m_hashEntries.ContainsKey( i ) )
        {
          ChartLegendEntryImpl entry = m_hashEntries[ i ];

          entry.Index = i - 1;

          m_hashEntries.Add( i - 1, entry );
          m_hashEntries.Remove( i );
        }
      }
    }
    /// <summary>
    /// Clones current object.
    /// </summary>
    /// <param name="parent">Parent for cloned object.</param>
    /// <param name="dicIndexes">Represents list with new font indexes.</param>
    /// <param name="dicNewSheetNames">Dictionary with new worksheet names.</param>
    /// <returns>Returns cloned object.</returns>
    public ChartLegendEntriesColl Clone( object parent, Dictionary<int, int> dicIndexes,
      Dictionary<string, string> dicNewSheetNames )
    {
      if( parent == null )
        throw new ArgumentNullException( "parent" );

      ChartLegendEntriesColl result = ( ChartLegendEntriesColl )MemberwiseClone();
      result.SetParent( parent );
      result.SetParents();

      int iCount = m_hashEntries.Count;

      result.m_hashEntries = new Dictionary<int, ChartLegendEntryImpl>( iCount );

      if( iCount == 0 )
        return result;

      foreach( KeyValuePair<int, ChartLegendEntryImpl> entry in m_hashEntries )
      {
        ChartLegendEntryImpl legendEntry = entry.Value;

        legendEntry = ( ChartLegendEntryImpl )legendEntry.Clone( result, dicIndexes, dicNewSheetNames );
        result.m_hashEntries.Add( entry.Key, legendEntry );
      }

      return result;
    }
    /// <summary>
    /// Clears current collection.
    /// </summary>
    public void Clear()
    {
      m_hashEntries.Clear();
    }
    /// <summary>
    /// Adds value to entry indexes.
    /// </summary>
    /// <param name="entryIndex">Represents start entry index.</param>
    /// <param name="value">Represents value to add.</param>
    public void UpdateEntries( int entryIndex, int value )
    {
      int iCount = Count;

      for( int i = Count - 1; i >= entryIndex; i-- )
      {
        if( m_hashEntries.ContainsKey( i ) )
        {
          ChartLegendEntryImpl entry = m_hashEntries[ i ];

          entry.Index += value;

          m_hashEntries.Add( entry.Index, entry );
          m_hashEntries.Remove( i );
        }
      }
    }
    #endregion
  }
}
