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

namespace Syncfusion.XlsIO.Implementation.Charts
{
  /// <summary>
  /// Represents chart plot area object.
  /// </summary>
  public class ChartPlotAreaImpl
    : ChartFrameFormatImpl
    , IChartFrameFormat
  {
    #region Class members
    /// <summary>
    /// Represents chart plot area record.
    /// </summary>
    private ChartPlotAreaRecord m_plotArea;
    /// <summary>
    /// Represents Excel 2007 layout data
    /// </summary>
    private IChartLayout m_layout;
    #endregion

    #region Class Properties
    /// <summary>
    /// Gets or sets Excel 2007 layout data
    /// </summary>
    public IChartLayout Layout
    {
        get
        {
            if (m_layout == null)
                m_layout = new ChartLayoutImpl(Application, this, Parent);

            return m_layout;
        }
        set
        {
            m_layout = value;
        }
    }
    /// <summary>
    /// Return attached label layout plot area record. Read-only
    /// </summary>
    public ChartPlotAreaLayoutRecord PlotAreaLayout
    {
        get
        {
            if (m_plotAreaLayout == null)
            {
                m_plotAreaLayout = (ChartPlotAreaLayoutRecord)
                    BiffRecordFactory.GetRecord(TBIFFRecord.PlotAreaLayout);
            }
            return m_plotAreaLayout;
        }
    }
    #endregion

    #region Class initialize methods
    /// <summary>
    /// Creates new instance.
    /// </summary>
    /// <param name="application">Represents current application</param>
    /// <param name="parent">Represents parent object.</param>
    public ChartPlotAreaImpl( IApplication application, object parent )
      : base( application, parent )
    {
      m_plotArea = ( ChartPlotAreaRecord )BiffRecordFactory.GetRecord( TBIFFRecord.ChartPlotArea );
      Border.LinePattern = ExcelChartLinePattern.None;
    }
    /// <summary>
    /// Creates new instance.
    /// </summary>
    /// <param name="application">Represents current application</param>
    /// <param name="parent">Represents parent object.</param>
    /// <param name="type">Represents chart type.</param>
    public ChartPlotAreaImpl( IApplication application, object parent, ExcelChartType type )
      : this( application, parent )
    {
      bool bFlag = Array.IndexOf( ChartImpl.DEF_WALLS_OR_FLOOR_TYPES, type ) == -1;
      bFlag = bFlag && Array.IndexOf( ChartImpl.DEF_DONT_NEED_PLOT, type ) == -1;

      if( bFlag && Workbook.Version == ExcelVersion.Excel97to2003 )
      {
        Interior.ForegroundColorIndex = ExcelKnownColors.Grey_25_percent;
      }
      else
      {
        Interior.ForegroundColorIndex = ExcelKnownColors.WhiteCustom;
      }
    }
    /// <summary>
    /// Parses new instance from stg stream.
    /// </summary>
    /// <param name="application">Represents current application</param>
    /// <param name="parent">Represents parent object.</param>
    /// <param name="data">Represents record storage.</param>
    /// <param name="iPos">Represents position in storage.</param>
    public ChartPlotAreaImpl( IApplication application, object parent, IList<BiffRecordRaw> data, ref int iPos )
      : base( application, parent, false )
    {
      Parse( data, ref iPos );
    }
    #endregion

    #region Parse methods
    /// <summary>
    /// Parses from stg stream.
    /// </summary>
    /// <param name="data">Record storage.</param>
    /// <param name="iPos">Represents position in storage.</param>
    new public void Parse( IList<BiffRecordRaw> data, ref int iPos )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      BiffRecordRaw record = ( BiffRecordRaw )data[ iPos ];

      record.CheckTypeCode( TBIFFRecord.ChartPlotArea );
      m_plotArea = ( ChartPlotAreaRecord )record;

      iPos++;

      record = ( BiffRecordRaw )data[ iPos ];

      if( record.TypeCode == TBIFFRecord.ChartFrame )
        base.Parse( data, ref iPos );

      iPos--;
    }
    #endregion

    #region Serialize methods
    /// <summary>
    /// Serialize current records to stg stream.
    /// </summary>
    /// <param name="records">Represents list of records to serialize into.</param>
    [ CLSCompliant( false ) ]
    new public void Serialize( IList<IBiffStorage> records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      if( m_plotArea != null )
        records.Add( ( BiffRecordRaw )m_plotArea.Clone() );

      base.Serialize( records );

      if (m_plotAreaLayout != null)
          SerializeRecord(records, m_plotAreaLayout);
    }
    #endregion
  }
}
