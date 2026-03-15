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
using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Parser.Biff_Records;
using System.Collections.Generic;

namespace Syncfusion.XlsIO.Implementation.Charts
{
	/// <summary>
	/// Represents collection of data points in the chart series.
	/// </summary>
	public class ChartDataPointsCollection
    : CommonObject
    , IChartDataPoints
	{
    #region Class members
    /// <summary>
    /// Format for data points without explicit format.
    /// </summary>
    private ChartDataPointImpl m_dataPointDefault;
    /// <summary>
    /// Collection of used data points, key - int index, value - data point.
    /// </summary>
    private Dictionary<int, ChartDataPointImpl> m_hashDataPoints = new Dictionary<int, ChartDataPointImpl>();
    /// <summary>
    /// Parent chart series.
    /// </summary>
    private ChartSerieImpl m_series;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes new instance of the collection and sets its parent and application properties.
    /// </summary>
    /// <param name="application">Application object.</param>
    /// <param name="parent">Parent object.</param>
    public ChartDataPointsCollection( IApplication application, object parent )
      : base( application, parent )
    {
      m_dataPointDefault = new ChartDataPointImpl( Application, this,
        ChartSerieImpl.DEF_FORMAT_ALLPOINTS_INDEX );

      SetParents();
    }
    /// <summary>
    /// Finds parent objects.
    /// </summary>
    private void SetParents()
    {
      m_series = FindParent( typeof( ChartSerieImpl ) ) as ChartSerieImpl;

      if( m_series == null )
        throw new ArgumentNullException( "Can't find parent series." );
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Returns single entry from the collection. Read-only.
    /// </summary>
    public IChartDataPoint this[ int index ]
    {
      get
      {
        if( index < 0 )
          throw new ArgumentOutOfRangeException( "index" );

        if( index == ChartSerieImpl.DEF_FORMAT_ALLPOINTS_INDEX )
        {
          return DefaultDataPoint;
        }

        if( !IsLoading )
        {
          int iCount = m_series.PointNumber;

          if( index >= iCount )
            throw new ArgumentOutOfRangeException( "index" );
        }

        ChartDataPointImpl result;

        if( m_hashDataPoints.ContainsKey( index ) )
        {
          result = m_hashDataPoints[ index ];
        }
        else
        {
          result = new ChartDataPointImpl( Application, this, index );
          Add( result );
        }

        ChartDataPointImpl defaultPoint = ( ChartDataPointImpl )DefaultDataPoint;
        ChartSerieDataFormatImpl format = defaultPoint.DataFormatOrNull;

        if( format != null && format.IsFormatted )
          result.CloneDataFormat( format );

        return result;
      }
    }
    /// <summary>
    /// Returns default data point which describes formats for data points without format. Read-only.
    /// </summary>
    public IChartDataPoint DefaultDataPoint
    {
      get
      {
        if( !m_series.InnerChart.Loading && !m_series.InnerChart.TypeChanging )
        {
          ChartFormatImpl format = m_series.GetCommonSerieFormat();
          m_dataPointDefault.CloneDataFormat( format.DataFormatOrNull );
        }

        return m_dataPointDefault;
      }
    }
    /// <summary>
    /// Indicates whether workbook is loading. Read-only.
    /// </summary>
    public bool IsLoading
    {
      get
      {
        return m_series.InnerWorkbook.Loading;
      }
    }
    /// <summary>
    /// Gets default data format or null. Read-only.
    /// </summary>
    public ChartSerieDataFormatImpl DefPointFormatOrNull
    {
      get
      {
        return m_dataPointDefault.DataFormatOrNull;
      }
    }
    #endregion

    #region Class serialization methods
    /// <summary>
    /// Serializes data labels data.
    /// </summary>
    /// <param name="records">List to serialize into.</param>
    [ CLSCompliant( false ) ]
    public void SerializeDataLabels( OffsetArrayList records )
    {
      foreach( ChartDataPointImpl point in m_hashDataPoints.Values )
      {
        point.SerializeDataLabels( records );
      }

      if( m_dataPointDefault != null )
      {
        m_dataPointDefault.SerializeDataLabels( records );
      }
    }
    /// <summary>
    /// Serializes all used data formats.
    /// </summary>
    /// <param name="records">List to serialize into.</param>
    [ CLSCompliant( false ) ]
    public void SerializeDataFormats( OffsetArrayList records )
    {
      foreach( ChartDataPointImpl point in m_hashDataPoints.Values )
      {
        point.SerializeDataFormat( records );
      }

      if( m_dataPointDefault != null )
      {
        m_dataPointDefault.SerializeDataFormat( records );
      }
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <param name="parent">Parent object for the cloned object.</param>
    /// <param name="book">Parent workbook.</param>
    /// <param name="dicFontIndexes">Dictionary with new font indexes.</param>
    /// <param name="dicNewSheetNames">Dictionary with new worksheet names.</param>
    /// <returns>A new object that is a copy of this instance.</returns>
    public object Clone( object parent, WorkbookImpl book, Dictionary<int, int> dicFontIndexes,
      Dictionary<string, string> dicNewSheetNames )
    {
      ChartDataPointsCollection result = ( ChartDataPointsCollection )MemberwiseClone();
      result.SetParent( parent );
      result.SetParents();

      int iLen = m_hashDataPoints.Count;
      result.m_hashDataPoints = new Dictionary<int, ChartDataPointImpl>( iLen );

      if( m_dataPointDefault != null )
      {
        result.m_dataPointDefault = ( ChartDataPointImpl )
          m_dataPointDefault.Clone( result, dicFontIndexes, dicNewSheetNames );
      }

      if( iLen > 0 )
      {
        foreach( ChartDataPointImpl point in m_hashDataPoints.Values )
        {
          ChartDataPointImpl pointClone = ( ChartDataPointImpl )
            point.Clone( result, dicFontIndexes, dicNewSheetNames );

          result.Add( pointClone );
        }
      }

      return result;
    }
    /// <summary>
    /// Adds new data point to the collection.
    /// </summary>
    /// <param name="point">Data point to add.</param>
    public void Add( ChartDataPointImpl point )
    {
      if( point == null )
        throw new ArgumentNullException( "point" );

      //int iCount = m_series.PointNumber;
      int iIndex = point.Index;

//      if( iIndex >= iCount || iIndex < 0 )
//        throw new ArgumentOutOfRangeException( "index" );

      m_hashDataPoints[ iIndex ] = point;
    }
    /// <summary>
    /// Removes all elements from the collection.
    /// </summary>
    public void Clear()
    {
      m_hashDataPoints.Clear();

      m_dataPointDefault = new ChartDataPointImpl( Application, this,
        ChartSerieImpl.DEF_FORMAT_ALLPOINTS_INDEX );

      m_dataPointDefault.DataFormat.BarShapeBase = ExcelBaseFormat.Rectangle;
      m_dataPointDefault.DataFormat.BarShapeTop = ExcelTopFormat.Straight;
    }
    /// <summary>
    /// Updates index of the parent Series.
    /// </summary>
    public void UpdateSerieIndex()
    {
      int iNewIndex = m_series.Index;
      m_dataPointDefault.UpdateSerieIndex();

      foreach( ChartDataPointImpl dataPoint in m_hashDataPoints.Values )
      {
        dataPoint.UpdateSerieIndex();
      }
    }
    /// <summary>
    /// Clears all series data formats.
    /// </summary>
    /// <param name="format">Represents format to update.</param>
    public void ClearDataFormats( ChartSerieDataFormatImpl format )
    {
      m_dataPointDefault.ClearDataFormats( format );

      if( m_hashDataPoints.Count == 0 )
        return;

      foreach( ChartDataPointImpl dataPoint in m_hashDataPoints.Values )
      {
        dataPoint.ClearDataFormats( format );
      }
    }
    /// <summary>
    /// Returns number of defined (created) data points. Read-only.
    /// </summary>
    public int DeninedDPCount
    {
      get
      {
        return m_hashDataPoints.Count;
      }
    }
    #endregion

    #region IEnumerable Members
    /// <summary>
    /// Returns an enumerator that iterates through a collection. 
    /// </summary>
    /// <returns>An IEnumerator object that can be used to iterate through the collection.</returns>
    public IEnumerator GetEnumerator()
    {
      return m_hashDataPoints.Values.GetEnumerator();
    }

    #endregion
  }
}
