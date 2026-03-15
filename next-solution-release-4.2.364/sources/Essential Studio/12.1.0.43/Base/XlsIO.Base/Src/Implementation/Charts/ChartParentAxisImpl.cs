#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

using System;
using System.Collections;

using Syncfusion.XlsIO.Implementation.Exceptions;
using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser.Biff_Records.Charts;
using System.Collections.Generic;

namespace Syncfusion.XlsIO.Implementation.Charts
{
	/// <summary>
	/// Represents ChartParentAxis record and subrecords.
	/// </summary>
	public class ChartParentAxisImpl : CommonObject
	{
    #region Class members
    /// <summary>
    /// Represents chartAxisParent record.
    /// </summary>
    private ChartAxisParentRecord m_parentAxis;
    /// <summary>
    /// Represents chart pos record.
    /// </summary>
    private ChartPosRecord m_position;
    /// <summary>
    /// Represents chart category axis.
    /// </summary>
    private ChartCategoryAxisImpl m_categoryAxis;
    /// <summary>
    /// Represents Chart value axis.
    /// </summary>
    private ChartValueAxisImpl m_valueAxis;
    /// <summary>
    /// Represents Chart series axis.
    /// </summary>
    private ChartSeriesAxisImpl m_seriesAxis;
    /// <summary>
    /// Represents parent chart.
    /// </summary>
    internal ChartImpl m_parentChart;
    /// <summary>
    /// Represents formats coll.
    /// </summary>
    private ChartGlobalFormatsCollection m_globalFormats;
    #endregion

    #region Class initialize methods
    /// <summary>
    /// Creates new instance of chart parent axis.
    /// </summary>
    /// <param name="application">Application object.</param>
    /// <param name="parent">Parent object.</param>
		public ChartParentAxisImpl( IApplication application, Object parent )
      : this( application, parent, true )
		{
		}
    /// <summary>
    /// Creates new instance of chart parent axis.
    /// </summary>
    /// <param name="application">Application object.</param>
    /// <param name="parent">Parent object.</param>
    /// <param name="isPrimary">If true - creates primary axis; otherwise - secondary.</param>
    public ChartParentAxisImpl( IApplication application, Object parent, bool isPrimary )
      : base( application, parent )
    {
      m_parentAxis = ( ChartAxisParentRecord )
        BiffRecordFactory.GetRecord( TBIFFRecord.ChartAxisParent );

      if( isPrimary )
      {
        m_categoryAxis = new ChartCategoryAxisImpl( application, this,
          ExcelAxisType.Category, IsPrimary );

        m_valueAxis = new ChartValueAxisImpl( application, this,
          ExcelAxisType.Value, IsPrimary );
      }

      if( !IsPrimary )
      {
        m_parentAxis.AxesIndex = 1;
      }
//      else
//      {
//        m_seriesAxis = new ChartSeriesAxisImpl( application, this,
//          ExcelAxisType.Serie, IsPrimary );
//      }

      SetParents();
    }
    /// <summary>
    /// Finds all parent objects.
    /// </summary>
    private void SetParents()
    {
      m_parentChart = ( ChartImpl )FindParent( typeof( ChartImpl ) );

      if( m_parentChart == null )
        throw new ArgumentException( "Can't find parent objects." );
    }
    #endregion

    #region Parse methods
    /// <summary>
    /// Parses parent axis impl.
    /// </summary>
    /// <param name="data">Record storage.</param>
    /// <param name="iPos">Position in storage.</param>
    [ CLSCompliant( false ) ]
    public void Parse( IList<BiffRecordRaw> data, ref int iPos )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      m_parentAxis = ( ChartAxisParentRecord )data[ iPos ];
      iPos++;

      BiffRecordRaw record = data[ iPos ];

      record.CheckTypeCode( TBIFFRecord.Begin );
      iPos++;

      int iCount = 1;

      while( iCount != 0 )
      {
        record = data[ iPos ];

        switch( record.TypeCode )
        {
          case TBIFFRecord.Begin:
            iPos = BiffRecordRaw.SkipBeginEndBlock( data, iPos ) - 1;
            break;

          case TBIFFRecord.End:
            iCount--;
            break;

          case TBIFFRecord.ChartPos:
            m_position = ( ChartPosRecord )record;
            break;

          case TBIFFRecord.ChartAxis:
            ParseAxes( data, ref iPos );
            break;

          case TBIFFRecord.ChartChartFormat:
            ParseChartFormat( data, ref iPos );
            break;

          case TBIFFRecord.ChartPlotArea:
            m_parentChart.PlotArea = new ChartPlotAreaImpl( Application, m_parentChart
              , data, ref iPos );
            break;

          case TBIFFRecord.ChartText:
            ParseChartText( data, ref iPos );
            break;

          case ( TBIFFRecord )2131:
            break;

          default:
            break;
        }

        iPos++;
      }
    }
    /// <summary>
    /// Parsers category, value, series axis.
    /// </summary>
    /// <param name="data">Records storage.</param>
    /// <param name="iPos">Position in storage.</param>
    private void ParseAxes( IList<BiffRecordRaw> data, ref int iPos )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      BiffRecordRaw record = ( BiffRecordRaw )data[ iPos ];

      record.CheckTypeCode( TBIFFRecord.ChartAxis );

      ChartAxisRecord axisRecord = ( ChartAxisRecord )data[ iPos ];

      switch( axisRecord.AxisType )
      {
        case ChartAxisRecord.ChartAxisType.CategoryAxis:
          m_categoryAxis = new ChartCategoryAxisImpl( Application, this, data
            , ref iPos, m_parentAxis.AxesIndex == 0 );
          break;

        case ChartAxisRecord.ChartAxisType.ValueAxis:
          m_valueAxis = new ChartValueAxisImpl( Application, this, data
            , ref iPos, m_parentAxis.AxesIndex == 0 );
          break;

        case ChartAxisRecord.ChartAxisType.SeriesAxis:
          m_seriesAxis = new ChartSeriesAxisImpl( Application, this, data
            , ref iPos, m_parentAxis.AxesIndex == 0 );
          break;

        default:
          throw new ArgumentOutOfRangeException( "Unknown chart axis type" );
      }

      iPos--;
    }
    /// <summary>
    /// Parses chart text.
    /// </summary>
    /// <param name="data">Record storage.</param>
    /// <param name="iPos">Position in storage.</param>
    private void ParseChartText( IList<BiffRecordRaw> data, ref int iPos )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      ChartTextAreaImpl text = new ChartTextAreaImpl( Application, this );

      iPos = text.Parse( data, iPos ) - 1;

      switch( text.ObjectLink.LinkObject )
      {
        case ExcelObjectTextLink.XAxis:
          if( m_categoryAxis != null )
            m_categoryAxis.SetTitle( text );
          break;

        case ExcelObjectTextLink.YAxis:
          if( m_valueAxis != null )
            m_valueAxis.SetTitle( text );
          break;

        case ExcelObjectTextLink.ZAxis:
          if( m_seriesAxis != null )
            m_seriesAxis.SetTitle( text );
          break;
      }
    }
    /// <summary>
    /// Parses chart format.
    /// </summary>
    /// <param name="data">Record storage.</param>
    /// <param name="iPos">Position in storage.</param>
    private void ParseChartFormat( IList<BiffRecordRaw> data, ref int iPos )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      ChartFormatCollection formats;

      formats = ( IsPrimary ) ?  m_globalFormats.PrimaryFormats : m_globalFormats.SecondaryFormats;

      ChartFormatImpl format = new ChartFormatImpl( Application, formats );
      format.Parse( data, ref iPos );

      formats.Add( format, false );

      //m_chartFormats.Add( format );
      iPos--;
    }
    #endregion

    #region Serialize methods
    /// <summary>
    /// Serialize current object.
    /// </summary>
    /// <param name="records">Record storage to serialize.</param>
    [ CLSCompliant( false ) ]
    public void Serialize( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      if( m_parentAxis == null )
        return;

      bool bFlag = Array.IndexOf( ChartImpl.DEF_SUPPORT_SERIES_AXIS, m_parentChart.ChartType ) != -1;

      records.Add( ( BiffRecordRaw )m_parentAxis.Clone() );
      records.Add( BiffRecordFactory.GetRecord( TBIFFRecord.Begin ) );

      if(m_position != null && IsPrimary)
        records.Add( ( BiffRecordRaw )m_position.Clone() );

      if( m_categoryAxis != null )
        m_categoryAxis.Serialize( records );

      if( m_valueAxis != null )
        m_valueAxis.Serialize( records );

      if( m_seriesAxis != null && m_parentAxis.AxesIndex == 0 && bFlag )
        m_seriesAxis.Serialize( records );

      if( m_categoryAxis != null )
        m_categoryAxis.SerializeAxisTitle( records );

      if( m_valueAxis != null )
        m_valueAxis.SerializeAxisTitle( records );

      if( m_seriesAxis != null && m_parentAxis.AxesIndex == 0 && bFlag )
        m_seriesAxis.SerializeAxisTitle( records );

      if( IsPrimary )
      {
        m_parentChart.SerializePlotArea( records );
        m_globalFormats.PrimaryFormats.Serialize( records );
      }
      else
      {
        m_globalFormats.SecondaryFormats.Serialize( records );
      }

      records.Add( BiffRecordFactory.GetRecord( TBIFFRecord.End ) );
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Represents chartAxisParent record.
    /// </summary>
    internal ChartAxisParentRecord ParentAxisRecord
    {
        get
        {
            if(m_parentAxis==null)
                m_parentAxis = (ChartAxisParentRecord)
        BiffRecordFactory.GetRecord(TBIFFRecord.ChartAxisParent);
            return m_parentAxis;
        }
    }
    /// <summary>
    /// Returns collection that contain chart format. Read-only.
    /// </summary>
    public ChartFormatCollection ChartFormats
    {
      get
      {
        if( IsPrimary )
        {
          return m_globalFormats.PrimaryFormats;
        }
        else
        {
          return m_globalFormats.SecondaryFormats;
        }
      }
    }
    /// <summary>
    /// If true this axis is primary axis; otherwise false. Read-only.
    /// </summary>
    public bool IsPrimary
    {
      get
      {
        return ( m_parentAxis.AxesIndex == 0 ) ? true : false;
      }
    }
    /// <summary>
    /// Gets or sets ChartCategoryAxis.
    /// </summary>
    public ChartCategoryAxisImpl CategoryAxis
    {
      get
      {
        return m_categoryAxis;
      }
      set
      {
        m_categoryAxis = value;
      }
    }
    /// <summary>
    /// Gets or sets ValueAxis.
    /// </summary>
    public ChartValueAxisImpl ValueAxis
    {
      get
      {
        return m_valueAxis;
      }
      set
      {
        m_valueAxis = value;
      }
    }
    /// <summary>
    /// Gets or sets series axis.
    /// </summary>
    public ChartSeriesAxisImpl SeriesAxis
    {
      get
      {
        return m_seriesAxis;
      }
      set
      {
        m_seriesAxis = value;
      }
    }
    /// <summary>
    /// Returns parent chart. Read-only.
    /// </summary>
    public ChartImpl ParentChart
    {
      get
      {
        object parent = FindParent( typeof( ChartImpl ) );

        if( parent == null )
          throw new ArgumentException( "cannot find parent object." );

        return parent as ChartImpl;
      }
    }
    /// <summary>
    /// Represents formats collection. Read-only.
    /// </summary>
    public ChartGlobalFormatsCollection Formats
    {
      get
      {
        return m_globalFormats;
      }
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Creates global format collection.
    /// </summary>
    public void CreatePrimaryFormats()
    {
      ChartGlobalFormatsCollection coll = new ChartGlobalFormatsCollection( Application, 
        ParentChart.PrimaryParentAxis, ParentChart.SecondaryParentAxis );

      ParentChart.PrimaryParentAxis.m_globalFormats = coll;
      ParentChart.SecondaryParentAxis.m_globalFormats = coll;

      if( !m_parentChart.ParentWorkbook.Loading )
      {
        ChartFormatImpl format = new ChartFormatImpl( Application, coll.PrimaryFormats );

        coll.PrimaryFormats.Add( format, false );
      }
    }
    /// <summary>
    /// Using for creating  secondary axis in Series type change.
    /// </summary>
    /// <param name="bCreateAxis">Value indicating whether to create new axis.</param>
    public void UpdateSecondaryAxis( bool bCreateAxis )
    {
      m_parentAxis = ( ChartAxisParentRecord )
        BiffRecordFactory.GetRecord( TBIFFRecord.ChartAxisParent );

      m_parentAxis.AxesIndex = 1;

      m_position = ( ChartPosRecord )
        BiffRecordFactory.GetRecord( TBIFFRecord.ChartPos );

      if( bCreateAxis )
      {
        if( m_categoryAxis == null )
          m_categoryAxis = new ChartCategoryAxisImpl( Application, this, ExcelAxisType.Category, false );

        if( m_valueAxis == null )
          m_valueAxis = new ChartValueAxisImpl( Application, this, ExcelAxisType.Value, false );
      }
    }
    /// <summary>
    /// Clones current object.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    /// <param name="dicFontIndexes">Dictionary with new font indexes.</param>
    /// <param name="dicNewSheetNames">Dictionary with new sheet names.</param>
    /// <returns>Returns cloned object.</returns>
    public ChartParentAxisImpl Clone( object parent, Dictionary<int, int> dicFontIndexes,
      Dictionary<string, string> dicNewSheetNames )
    {
      ChartParentAxisImpl result = ( ChartParentAxisImpl )MemberwiseClone();
      result.SetParent( parent );
      result.SetParents();

      result.m_parentAxis = ( ChartAxisParentRecord )CloneUtils.CloneCloneable( m_parentAxis );
      result.m_position = ( ChartPosRecord )CloneUtils.CloneCloneable( m_position );

      if( IsPrimary )
      {
        if( m_globalFormats != null )
          result.m_globalFormats = m_globalFormats.CloneForPrimary( result );
      }
      else if( m_globalFormats != null )
      {
        result.m_globalFormats = result.ParentChart.PrimaryParentAxis.Formats;
        m_globalFormats.CloneForSecondary( result.m_globalFormats, result );
      }

      if( m_seriesAxis != null )
      {
        result.m_seriesAxis = ( ChartSeriesAxisImpl )
          m_seriesAxis.Clone( result, dicFontIndexes, dicNewSheetNames );
      }

      if( m_valueAxis != null )
      {
        result.m_valueAxis = ( ChartValueAxisImpl )
          m_valueAxis.Clone( result, dicFontIndexes, dicNewSheetNames );
      }

      if( m_categoryAxis != null )
      {
        result.m_categoryAxis = ( ChartCategoryAxisImpl )
          m_categoryAxis.Clone( result, dicFontIndexes, dicNewSheetNames );
      }

      return result;
    }
    /// <summary>
    /// Clears all walls, floor, gridLines.
    /// </summary>
    public void ClearGridLines()
    {
      if( m_categoryAxis != null )
      {
        m_categoryAxis.HasMajorGridLines = false;
        m_categoryAxis.HasMinorGridLines = false;
      }

      if( m_valueAxis != null )
      {
        m_categoryAxis.HasMajorGridLines = false;
        m_categoryAxis.HasMinorGridLines = false;
      }

      if( m_seriesAxis != null )
      {
        m_categoryAxis.HasMajorGridLines = false;
        m_categoryAxis.HasMinorGridLines = false;
      }
    }
    /// <summary>
    /// Sets items with used reference indexes to true.
    /// </summary>
    /// <param name="usedItems">Array to mark used references in.</param>
    public void MarkUsedReferences( bool[] usedItems )
    {
      if( m_valueAxis != null )
        m_valueAxis.MarkUsedReferences( usedItems );

      if( m_categoryAxis != null )
        m_categoryAxis.MarkUsedReferences( usedItems );

      if( m_seriesAxis != null )
        m_seriesAxis.MarkUsedReferences( usedItems );
    }
    /// <summary>
    /// Updates reference indexes.
    /// </summary>
    /// <param name="arrUpdatedIndexes">Array with updated indexes.</param>
    public void UpdateReferenceIndexes( int[] arrUpdatedIndexes )
    {
      if( m_valueAxis != null )
        m_valueAxis.UpdateReferenceIndexes( arrUpdatedIndexes );

      if( m_categoryAxis != null )
        m_categoryAxis.UpdateReferenceIndexes( arrUpdatedIndexes );

      if( m_seriesAxis != null )
        m_seriesAxis.UpdateReferenceIndexes( arrUpdatedIndexes );
    }
    internal void RemoveAxis(bool isCategory)
    {
        if (isCategory)
            m_categoryAxis = null;
        else
            m_valueAxis = null;
    }


    #endregion
  }
}
