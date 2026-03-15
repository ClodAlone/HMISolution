#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

using System;
using System.Collections;

using Syncfusion.XlsIO.Implementation.XmlSerialization.Constants;
using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser.Biff_Records.Charts;
using LinkIndex = Syncfusion.XlsIO.Parser.Biff_Records.Charts.ChartAIRecord.LinkIndex;
using ReferenceType = Syncfusion.XlsIO.Parser.Biff_Records.Charts.ChartAIRecord.ReferenceType;
using System.Collections.Generic;

namespace Syncfusion.XlsIO.Implementation.Charts
{
  /// <summary>
  /// Class used for Chart Series Axis implementation.
  /// </summary>
  public class ChartSeriesAxisImpl :
    ChartAxisImpl,
    IChartSeriesAxis,
    IScalable
  {
    #region Class constants
    /// <summary>
    /// Specifies maximum spacing value.
    /// </summary>
    private const int DEF_MAX_SPACING_VALUE = 31999;
    #endregion

    #region Class members
    /// <summary>
    /// Catser range record.
    /// </summary>
    private ChartCatserRangeRecord m_chartCatserRange;
    #endregion

    #region Class constructors
    /// <summary>
    /// Creates axis object.
    /// </summary>
    /// <param name="application">Application object for the axis.</param>
    /// <param name="parent">Parent object for the axis.</param>
    public ChartSeriesAxisImpl( IApplication application, object parent )
      : base( application, parent )
    {
      AxisId = ChartConstants.DefaultSeriesAxisId;
    }
    /// <summary>
    /// Creates primary axis of specified type.
    /// </summary>
    /// <param name="application">Application object for the axis.</param>
    /// <param name="parent">Parent object for the axis.</param>
    /// <param name="axisType">Type of the new axis.</param>
    public ChartSeriesAxisImpl( IApplication application, object parent, ExcelAxisType axisType )
      : this( application, parent, axisType, true )
    {
    }
    /// <summary>
    /// Creates axis of specified type and specified IsPrimary value.
    /// </summary>
    /// <param name="application">Application object for the axis.</param>
    /// <param name="parent">Parent object for the axis.</param>
    /// <param name="axisType">Type of the new axis.</param>
    /// <param name="bIsPrimary">
    /// True if primary axis should be created; otherwise False.
    /// </param>
    public ChartSeriesAxisImpl( IApplication application, object parent,
      ExcelAxisType axisType, bool bIsPrimary )
      : base( application, parent, axisType, bIsPrimary )
    {
      AxisId = ChartConstants.DefaultSeriesAxisId;
    }
    /// <summary>
    /// Extracts primary axis from the array of BiffRecords.
    /// </summary>
    /// <param name="application">Application object for the axis.</param>
    /// <param name="parent">Parent object for the axis.</param>
    /// <param name="data">Array of BiffRecords with axis data.</param>
    /// <param name="iPos">
    /// Position of the first axis record in the data array.
    /// </param>
    [ CLSCompliant( false ) ]
    public ChartSeriesAxisImpl( IApplication application, object parent
      , IList<BiffRecordRaw> data, ref int iPos )
      : this( application, parent, data, ref iPos, true )
    {
    }
    /// <summary>
    /// Extracts axis from the array of BiffRecords.
    /// </summary>
    /// <param name="application">Application object for the axis.</param>
    /// <param name="parent">Parent object for the axis.</param>
    /// <param name="data">Array of BiffRecords with axis data.</param>
    /// <param name="iPos">
    /// Position of the first axis record in the data array.
    /// </param>
    /// <param name="isPrimary">
    /// True if it is primary axis; otherwise False.
    /// </param>
    [ CLSCompliant( false ) ]
    public ChartSeriesAxisImpl( IApplication application, object parent
      , IList<BiffRecordRaw> data, ref int iPos, bool isPrimary )
      : base( application, parent, data, ref iPos, isPrimary )
    {
      AxisId = ChartConstants.DefaultSeriesAxisId;
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Represents the number of categories or series between tick-mark labels.
    /// </summary>
    public int LabelFrequency
    {
      get
      {
        return TickLabelSpacing;
      }
      set
      {
        TickLabelSpacing = value;
      }
    }
    /// <summary>
    /// Represents the number of categories or series between tick-mark labels.
    /// </summary>
    public int TickLabelSpacing
    {
      get
      {
        return m_chartCatserRange.LabelsFrequency;
      }
      set
      {
        if( value < 0 || value > DEF_MAX_SPACING_VALUE )
          throw new ArgumentOutOfRangeException( "Value must be great then 0 and less then 31999." );

        m_chartCatserRange.LabelsFrequency = ( ushort )value;
      }
    }

    /// <summary>
    /// Represents the number of categories or series between tick marks.
    /// </summary>
    public int TickMarksFrequency
    {
      get
      {
        return TickMarkSpacing;
      }
      set
      {
        TickMarkSpacing = value;
      }
    }
    /// <summary>
    /// Represents the number of categories or series between tick marks.
    /// </summary>
    public int TickMarkSpacing
    {
      get
      {
        return m_chartCatserRange.TickMarksFrequency;
      }
      set
      {
        if( value < 0 || value > DEF_MAX_SPACING_VALUE )
          throw new ArgumentOutOfRangeException( "Value must be great then 0 and less then 31999." );

        m_chartCatserRange.TickMarksFrequency = ( ushort )value;
      }
    }
    /// <summary>
    /// Display categories in reverse order.
    /// </summary>
    public override bool ReversePlotOrder
    {
      get
      {
        return m_chartCatserRange.IsReverse;
      }
      set
      {
        m_chartCatserRange.IsReverse = value;
      }
    }
    /// <summary>
    /// Returns title area. Read-only.
    /// </summary>
    [ CLSCompliant( false ) ]
    protected override ExcelObjectTextLink TextLinkType
    {
      get
      {
        return ExcelObjectTextLink.ZAxis;
      }
    }
    /// <summary>
    /// Represents the point on the axis another axis crosses it.
    /// </summary>
    public int CrossesAt
    {
      get
      {
        return m_chartCatserRange.CrossingPoint;
      }
      set
      {
        m_chartCatserRange.CrossingPoint = ( ushort )value;
      }
    }
    public bool IsBetween
    {
      get
      {
        return m_chartCatserRange.IsBetween;
      }
      set
      {
        m_chartCatserRange.IsBetween = value;
      }
    }
    #endregion

    #region Class parse methods
    /// <summary>
    /// Parse max cross.
    /// </summary>
    /// <param name="record">Represents MaxCross data.</param>
    private void ParseMaxCross( BiffRecordRaw record )
    {
      if( record == null )
        throw new ArgumentNullException( "record" );
       
      record.CheckTypeCode( TBIFFRecord.ChartCatserRange );
      m_chartCatserRange = ( ChartCatserRangeRecord )record;
    }
    /// <summary>
    /// Parses walls or floors.
    /// </summary>
    /// <param name="data">Represents record storage.</param>
    /// <param name="iPos">Represents position in storage.</param>
    protected override void ParseWallsOrFloor( IList<BiffRecordRaw> data, ref int iPos )
    {
      throw new NotSupportedException( "Current axis type doesn't support walls or floors" );
    }

    /// <summary>
    /// Parses data.
    /// </summary>
    /// <param name="record">Represents record to parse.</param>
    /// <param name="data">Represents records storage.</param>
    /// <param name="iPos">Represents position in storage.</param>
    [ CLSCompliant( false ) ]
    protected override void ParseData( BiffRecordRaw record, IList<BiffRecordRaw> data, ref int iPos )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      if( record == null )
        throw new ArgumentNullException( "record" );

      switch( record.TypeCode )
      {
        case TBIFFRecord.ChartCatserRange:
          ParseMaxCross( record );
          break;
      }
    }
    #endregion

    #region Class serialize methods
    /// <summary>
    /// Serializes axis.
    /// </summary>
    /// <param name="records">OffsetArrayList that will receive all records.</param>
    [ CLSCompliant( false ) ]
    public override void Serialize(OffsetArrayList records)
    {
      ChartAxisRecord axis = (ChartAxisRecord)
        BiffRecordFactory.GetRecord( TBIFFRecord.ChartAxis );
      axis.AxisType = ChartAxisRecord.ChartAxisType.SeriesAxis;

      records.Add( axis );
      records.Add( BiffRecordFactory.GetRecord( TBIFFRecord.Begin ) );
      records.Add( ( BiffRecordRaw )m_chartCatserRange.Clone() );

      SerializeTickRecord( records );
      SerializeNumberFormat( records );
      SerializeFont( records );
      SerializeAxisBorder( records );
      SerializeGridLines( records );

      records.Add( BiffRecordFactory.GetRecord( TBIFFRecord.End ) );
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// Initializes variables.
    /// </summary>
    protected override void InitializeVariables()
    {
      m_chartCatserRange = ( ChartCatserRangeRecord )BiffRecordFactory.GetRecord(
        TBIFFRecord.ChartCatserRange );

      base.InitializeVariables ();
    }

    /// <summary>
    /// Clone current object.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    /// <param name="dicFontIndexes">Dictionary with new font indexes.</param>
    /// <param name="dicNewSheetNames">Dictionary with new worksheet names.</param>
    /// <returns>Returns cloned object.</returns>
    public override ChartAxisImpl Clone( object parent, Dictionary<int, int> dicFontIndexes,
      Dictionary<string, string> dicNewSheetNames )
    {
      ChartSeriesAxisImpl result = ( ChartSeriesAxisImpl )
        base.Clone( parent, dicFontIndexes, dicNewSheetNames );

      if( m_chartCatserRange != null )
      {
        result.m_chartCatserRange = ( ChartCatserRangeRecord )m_chartCatserRange.Clone();
      }

      return result;
    }
    #endregion

    #region IScalable Members

    public bool IsLogScale
    {
      get
      {
        throw new NotImplementedException();
      }
      set
      {
        throw new NotImplementedException();
      }
    }

    public double MaximumValue
    {
      get
      {
        throw new NotImplementedException();
      }
      set
      {
        throw new NotImplementedException();
      }
    }

    public double MinimumValue
    {
      get
      {
        throw new NotImplementedException();
      }
      set
      {
        throw new NotImplementedException();
      }
    }

    #endregion
  }
}
