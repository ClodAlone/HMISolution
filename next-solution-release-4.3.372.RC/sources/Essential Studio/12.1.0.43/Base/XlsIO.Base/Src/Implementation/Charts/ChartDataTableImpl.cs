#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

using System;
using System.Collections;

using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser.Biff_Records.Charts;
using System.Collections.Generic;
using Syncfusion.XlsIO.Implementation.XmlSerialization.Charts;

namespace Syncfusion.XlsIO.Implementation.Charts
{
  /// <summary>
  /// Class used for Chart DataTable implementation.
  /// </summary>
  public class ChartDataTableImpl
    : CommonObject
    , IChartDataTable
  {
    #region Class members
    /// <summary>
    /// This record stores options for the chart data table.
    /// </summary>
    private ChartDatRecord m_chartDat;
    /// <summary>
    /// Records that were read (if data table was not created but loaded).
    /// </summary>
    private List<BiffRecordRaw> m_arrRecords = new List<BiffRecordRaw>();
    /// <summary>
    /// Represents chart text record and sub records.
    /// </summary>
    private ChartTextAreaImpl m_text;
    #endregion

    #region Class constructors
    /// <summary>
    /// Creates default data table.
    /// </summary>
    /// <param name="application">Application object for the new data table.</param>
    /// <param name="parent">Parent object for the new data table.</param>
    public ChartDataTableImpl( IApplication application, object parent )
      : base( application, parent )
    {
      m_chartDat = ( ChartDatRecord )BiffRecordFactory.GetRecord( TBIFFRecord.ChartDat );
      m_chartDat.HasBorders = true;
      m_chartDat.HasHorizontalBorders = true;
      m_chartDat.HasVerticalBorders = true;
    }
    /// <summary>
    /// Extracts data table's data from the array of BiffRecords.
    /// </summary>
    /// <param name="application">Application object for the new data table.</param>
    /// <param name="parent">Parent object for the new data table.</param>
    /// <param name="data">Array of BiffRecords that contains the data table's data.</param>
    /// <param name="iPos">Position of the ChartData record.</param>
    [ CLSCompliant( false ) ]
    public ChartDataTableImpl( IApplication application, object parent
      , IList<BiffRecordRaw> data, ref int iPos )
      : base( application, parent )
    {
      Parse( data, ref iPos );
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Extracts data table from the array of BiffRecords.
    /// </summary>
    /// <param name="data">BiffRecords with data table records.</param>
    /// <param name="iPos">Position of the ChartData record.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When specified record is not ChartData record
    /// or when next record is not Begin record.
    /// </exception>
    private void Parse( IList<BiffRecordRaw> data, ref int iPos )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      BiffRecordRaw record = ( BiffRecordRaw )data[ iPos ];
      record.CheckTypeCode( TBIFFRecord.ChartDat );
      m_chartDat = ( ChartDatRecord )record;
      iPos++;

      record = ( BiffRecordRaw )data[ iPos ];
      record.CheckTypeCode( TBIFFRecord.Begin );
      m_arrRecords.Add( record );
      iPos++;

      int level = 1;

      while( level != 0 )
      {
      record = ( BiffRecordRaw )data[ iPos ];

             if( record.TypeCode == TBIFFRecord.End   ) level--;
        else if( record.TypeCode == TBIFFRecord.Begin ) level++;
        
        m_arrRecords.Add( record );
        iPos++;
      }

    }
    /// <summary>
    /// Serializes data table.
    /// </summary>
    /// <param name="records">
    /// OffsetArrayList that will receive all data table records.
    /// </param>
    /// <exception cref="System.ArgumentNullException">
    /// When specified OffsetArrayList is NULL.
    /// </exception>
    [ CLSCompliant( false ) ]
    public void Serialize( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      records.Add( m_chartDat );
      
      if( m_arrRecords.Count == 0 )
      {
        m_arrRecords.Add( BiffRecordFactory.GetRecord( TBIFFRecord.Begin ) );

        ChartLegendRecord legend = (ChartLegendRecord)
          BiffRecordFactory.GetRecord( TBIFFRecord.ChartLegend );
        
        legend.IsVerticalLegend = true;
        legend.ContainsDataTable = true;
        legend.Position = ExcelLegendPosition.NotDocked;
        legend.Spacing = ExcelLegendSpacing.Medium;

        m_arrRecords.Add( legend );
        m_arrRecords.Add( BiffRecordFactory.GetRecord( TBIFFRecord.Begin ) );

        ChartPosRecord chartPos = ( ChartPosRecord )
          BiffRecordFactory.GetRecord( TBIFFRecord.ChartPos );
        chartPos.TopLeft = 3;

        m_arrRecords.Add( chartPos );

        ChartTextRecord chartText = ( ChartTextRecord )BiffRecordFactory.GetRecord( TBIFFRecord.ChartText );
        chartText.Options2 = 10816;
        m_arrRecords.Add( chartText );
        m_arrRecords.Add( BiffRecordFactory.GetRecord( TBIFFRecord.Begin ) );

        chartPos = ( ChartPosRecord )BiffRecordFactory.GetRecord( TBIFFRecord.ChartPos );
        chartPos.TopLeft = 2;
        chartPos.BottomRight = 2;
        m_arrRecords.Add( chartPos );

        m_arrRecords.Add( BiffRecordFactory.GetRecord( TBIFFRecord.ChartFontx ) );
        m_arrRecords.Add( BiffRecordFactory.GetRecord( TBIFFRecord.ChartAI ) );
        m_arrRecords.Add( BiffRecordFactory.GetRecord( TBIFFRecord.End ) );

        m_arrRecords.Add( BiffRecordFactory.GetRecord( TBIFFRecord.End ) );
       
        m_arrRecords.Add( BiffRecordFactory.GetRecord( TBIFFRecord.End ) );
      }

      records.AddList(m_arrRecords );
    }
    /// <summary>
    /// Clone current Record.
    /// </summary>
    /// <param name="parent">Parent object for create new instance.</param>
    /// <returns>Returns clone of current object.</returns>
    public ChartDataTableImpl Clone( object parent )
    {
      ChartDataTableImpl result = new ChartDataTableImpl( Application, parent );
      
      result.m_bIsDisposed = m_bIsDisposed;

      if( m_arrRecords != null )
      {
        List<BiffRecordRaw> list = new List<BiffRecordRaw>();

        for( int i = 0, len = m_arrRecords.Count; i < len; i++ )
        {
          BiffRecordRaw record = ( BiffRecordRaw )m_arrRecords[ i ].Clone();
          list.Add( record );
        }

        result.m_arrRecords = list;
      }

      if( m_chartDat != null )
      {
        result.m_chartDat = ( ChartDatRecord )m_chartDat.Clone();
      }

      return result;
    }
    #endregion

    #region Class properties
    /// <summary>
    /// True if data table has horizontal border.
    /// </summary>
    public bool HasHorzBorder
    {
      get
      {
        return m_chartDat.HasHorizontalBorders;
      }
      set
      {
        m_chartDat.HasHorizontalBorders = value;
      }
    }
    /// <summary>
    /// True if data table has vertical border.
    /// </summary>
    public bool HasVertBorder
    {
      get
      {
        return m_chartDat.HasVerticalBorders;
      }
      set
      {
        m_chartDat.HasVerticalBorders = value;
      }
    }
    /// <summary>
    /// True if data table has borders.
    /// </summary>
    public bool HasBorders
    {
      get
      {
        return m_chartDat.HasBorders;
      }
      set
      {
        m_chartDat.HasBorders = value;
      }
    }
    /// <summary>
    /// True if there is series keys in the data table.
    /// </summary>
    public bool ShowSeriesKeys
    {
      get
      {
        return m_chartDat.ShowSeriesKeys;
      }
      set
      {
        m_chartDat.ShowSeriesKeys = value;
      }
    }
    /// <summary>
    /// Return text area of data table.
    /// </summary>
    public IChartTextArea TextArea
    {
        get
        {
            if (m_text == null)
            {
                m_text = new ChartTextAreaImpl(Application, this);
                m_text.FontName = ChartAxisParser.DefaultFont;
            }
            else
                ChartParserCommon.CheckDefaultSettings(m_text);
            return m_text; ;
        }
    }
    #endregion
  }
}
