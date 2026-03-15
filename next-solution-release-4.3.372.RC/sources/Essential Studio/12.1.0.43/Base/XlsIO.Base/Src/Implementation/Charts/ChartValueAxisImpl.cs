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
  /// Class used for Chart Value Axis implementation.
  /// </summary>
  public class ChartValueAxisImpl
    : ChartAxisImpl
    , IChartValueAxis
    , IScalable
  {
    #region Class static members
    /// <summary>
    /// Represents array of display units values.
    /// Index - display units index, value - double value.
    /// </summary>
    public static readonly double[] DEF_DISPLAY_UNIT_VALUES =
    {
      0,
      100,
      1000,
      10000,
      100000,
      1000000,
      10000000,
      100000000,
      1000000000,
      1000000000000,
    };
    #endregion

    #region Class members
    /// <summary>
    /// Indicates is has display unit label.
    /// </summary>
    private bool m_bHasDisplayUnitLabel;
    /// <summary>
    /// Chart value record.
    /// </summary>
    private ChartValueRangeRecord m_chartValueRange;
    /// <summary>
    /// Represents custom unit to display.
    /// </summary>
    private double m_displayUnitCustom = 1;
    /// <summary>
    /// Represents display unit.
    /// </summary>
    private ExcelChartDisplayUnit m_displayUnit;
    /// <summary>
    /// Represents display Unit label.
    /// </summary>
    private ChartWrappedTextAreaImpl m_displayUnitLabel;
    /// <summary>
    /// Indicates whether tick label spacing value is automatically evaluated.
    /// </summary>
    private bool m_bAutoTickLabelSpacing = true;
    #endregion

    #region Class constructors
    /// <summary>
    /// Creates axis object.
    /// </summary>
    /// <param name="application">Application object for the axis.</param>
    /// <param name="parent">Parent object for the axis.</param>
    public ChartValueAxisImpl( IApplication application, object parent )
      : base( application, parent )
    {
    }
    /// <summary>
    /// Creates primary axis of specified type.
    /// </summary>
    /// <param name="application">Application object for the axis.</param>
    /// <param name="parent">Parent object for the axis.</param>
    /// <param name="axisType">Type of the new axis.</param>
    public ChartValueAxisImpl( IApplication application, object parent, ExcelAxisType axisType )
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
    public ChartValueAxisImpl( IApplication application, object parent,
      ExcelAxisType axisType, bool bIsPrimary )
      : base( application, parent, axisType, bIsPrimary )
    {
      AxisId = IsPrimary ?
        ChartConstants.DefaultValueAxisId :
        ChartConstants.DefaultSecondaryValueAxisId;
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
    public ChartValueAxisImpl( IApplication application, object parent
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
    public ChartValueAxisImpl( IApplication application, object parent
      , IList<BiffRecordRaw> data, ref int iPos, bool isPrimary )
      : base( application, parent, data, ref iPos, isPrimary )
    {
      AxisId = IsPrimary ?
        ChartConstants.DefaultValueAxisId :
        ChartConstants.DefaultSecondaryValueAxisId;
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Maximum value on axis.
    /// </summary>
    public double MinimumValue
    {
      get
      {
        CheckValueRangeRecord();
        return m_chartValueRange.NumMin;
      }
      set
      {
        if( !IsAutoMax && value >= MaximumValue )
          throw new ArgumentOutOfRangeException( "MinimumValue" );

        CheckValueRangeRecord();
        m_chartValueRange.NumMin = value;
        IsAutoMin = false;
      }
    }
    /// <summary>
    /// Maximum value on axis.
    /// </summary>
    public double MaximumValue
    {
      get
      {
        CheckValueRangeRecord();
        return m_chartValueRange.NumMax;
      }
      set
      {
        if( !IsAutoMin && value <= MinimumValue )
          throw new ArgumentOutOfRangeException( "MaximumValue" );

        CheckValueRangeRecord();
        m_chartValueRange.NumMax = value;
        IsAutoMax = false;
      }
    }
    /// <summary>
    /// Value of major increment.
    /// </summary>
    public virtual double MajorUnit
    {
      get
      {
        CheckValueRangeRecord();
        return m_chartValueRange.NumMajor;
      }
      set
      {
        SetMajorUnit( value );
      }
    }
    /// <summary>
    /// Value of minor increment.
    /// </summary>
    public virtual double MinorUnit
    {
      get
      {
        CheckValueRangeRecord();
        return m_chartValueRange.NumMinor;
      }
      set
      {
        SetMinorUnit( value );
      }
    }
    /// <summary>
    /// Represents the point on the axis another axis crosses it.
    /// </summary>
    public double CrossValue
    {
      get
      {
        return CrossesAt;
      }
      set
      {
        CrossesAt = value;
      }
    }
    /// <summary>
    /// Represents the point on the axis another axis crosses it.
    /// </summary>
    public virtual double CrossesAt
    {
      get
      {
        return m_chartValueRange.NumCross;
      }
      set
      {
        m_chartValueRange.NumCross = value;
        IsAutoCross = false;
      }
    }
    /// <summary>
    /// Automatic minimum selected.
    /// </summary>
    public virtual bool IsAutoMin
    {
      get
      {
        return ( CheckValueRangeRecord( false ) ) ?
          m_chartValueRange.IsAutoMin :
          true;
      }
      set
      {
        SetAutoMin( true, value );
      }
    }
    protected void SetAutoMin( bool check, bool value )
    {
      CheckValueRangeRecord( check );
      m_chartValueRange.IsAutoMin = value;
    }
    /// <summary>
    /// Automatic maximum selected.
    /// </summary>
    public virtual bool IsAutoMax
    {
      get
      {
        return ( CheckValueRangeRecord( false ) ) ?
          m_chartValueRange.IsAutoMax :
          true;
      }
      set
      {
        SetAutoMax( true, value );
      }
    }
    protected void SetAutoMax( bool check, bool value )
    {
      CheckValueRangeRecord( check );
      m_chartValueRange.IsAutoMax = value;
    }
    /// <summary>
    /// Represents whether the label spacing is automatic or not
    /// </summary>
    public bool AutoTickLabelSpacing
    {
        get
        {
            return (IsAutoMajor && IsAutoMax && IsAutoMin && IsAutoMinor);
        }
        set
        {
            m_bAutoTickLabelSpacing = value;
        }
    }
    /// <summary>
    /// Automatic major selected.
    /// </summary>
    public virtual bool IsAutoMajor
    {
      get
      {
        return m_chartValueRange.IsAutoMajor;
      }
      set
      {
        m_chartValueRange.IsAutoMajor = value;
      }
    }
    /// <summary>
    /// Automatic minor selected.
    /// </summary>
    public virtual bool IsAutoMinor
    {
      get
      {
        return m_chartValueRange.IsAutoMinor;
      }
      set
      {
        m_chartValueRange.IsAutoMinor = value;
      }
    }
    /// <summary>
    /// Automatic category crossing point selected.
    /// </summary>
    public virtual bool IsAutoCross
    {
      get
      {
        return ( CheckValueRangeRecord( false ) ) ?
          m_chartValueRange.IsAutoCross :
          true;
      }
      set
      {
        CheckValueRangeRecord();
        m_chartValueRange.IsAutoCross = value;
      }
    }
    /// <summary>
    /// Logarithmic scale.
    /// </summary>
    public bool IsLogScale
    {
      get
      {
        return ( CheckValueRangeRecord( false ) ) ?
          m_chartValueRange.IsLogScale :
          false;
      }
      set
      {
        CheckValueRangeRecord();
        m_chartValueRange.IsLogScale = value;

        if( value )
        {
          if( !m_chartValueRange.IsAutoMin && m_chartValueRange.NumMin < 1 )
            m_chartValueRange.NumMin = 1;

          if( !m_chartValueRange.IsAutoMax && m_chartValueRange.NumMax < 1 )
            m_chartValueRange.NumMax = 1;
        }

      }
    }
    /// <summary>
    /// True if plots data points from last to first.
    /// </summary>
    public override bool ReversePlotOrder
    {
      get
      {
        return m_chartValueRange.IsReverse;
      }
      set
      {
        m_chartValueRange.IsReverse = value;
      }
    }
    /// <summary>
    /// Category axis to cross at maximum value.
    /// </summary>
    public virtual bool IsMaxCross
    {
      get
      {
        return m_chartValueRange.IsMaxCross;
      }
      set
      {
        m_chartValueRange.IsMaxCross = value;
      }
    }
    /// <summary>
    /// Gets or sets the ChartValueRangeRecord.
    /// </summary>
    [ CLSCompliant( false ) ]
    protected ChartValueRangeRecord ChartValueRange
    {
      get
      {
        return m_chartValueRange;
      }
      set
      {
        m_chartValueRange = value;
      }
    }

    /// <summary>
    /// Represents custom unit to display.
    /// </summary>
    public double DisplayUnitCustom
    {
      get
      {
        CheckValueRangeRecord();

        return m_displayUnitCustom;
      }
      set
      {
        CheckValueRangeRecord();

        if( value <= 0 )
          throw new ArgumentOutOfRangeException( "The value must be large than zero." );

        m_displayUnitCustom = value;
        DisplayUnit = ExcelChartDisplayUnit.Custom;
      }
    }
    /// <summary>
    /// Returns or sets the unit label for the specified axis.
    /// </summary>
    public ExcelChartDisplayUnit DisplayUnit
    {
      get
      {
        CheckValueRangeRecord();

        return m_displayUnit;
      }
      set
      {
        CheckValueRangeRecord();
        m_displayUnit = value;

        if( value == ExcelChartDisplayUnit.None )
        {
          m_bHasDisplayUnitLabel = false;
          m_displayUnitCustom = 1;
          m_displayUnitLabel = null;
        }
        else
        {
          int iIndex = ( int )value;

          if( iIndex < DEF_DISPLAY_UNIT_VALUES.Length )
            m_displayUnitCustom = DEF_DISPLAY_UNIT_VALUES[ iIndex ];

          if( !ParentAxis.ParentChart.ParentWorkbook.Loading )
            HasDisplayUnitLabel = true;
        }
      }
    }
    /// <summary>
    /// True if the label is displayed on the specified axis.
    /// </summary>
    public bool HasDisplayUnitLabel
    {
      get
      {
        CheckValueRangeRecord();

        return m_bHasDisplayUnitLabel;
      }
      set
      {
        CheckValueRangeRecord();

        if( m_displayUnit == ExcelChartDisplayUnit.None )
          throw new NotSupportedException( "Doesnot support display unit label in DisplayUnit None mode." );

        if( value && m_displayUnitLabel == null )
          CreateDispalayUnitLabel();

        m_bHasDisplayUnitLabel = value;
      }
    }
    /// <summary>
    /// Returns the DisplayUnitLabel object for the specified axis.
    /// Returns Null if the HasDisplayUnitLabel property is set to False. Read-only.
    /// </summary>
    public IChartTextArea DisplayUnitLabel
    {
      get
      {
        CheckValueRangeRecord();

        if( !HasDisplayUnitLabel )
          return null;

        return m_displayUnitLabel;
      }
    }
    #endregion

    #region Class parse methods
    /// <summary>
    /// Parses max cross.
    /// </summary>
    /// <param name="record">Represents max cross data to parse.</param>
    [ CLSCompliant( false ) ]
    protected virtual void ParseMaxCross( BiffRecordRaw record )
    {
      if( record == null )
        throw new ArgumentNullException( "record" );

      record.CheckTypeCode( TBIFFRecord.ChartValueRange );
      m_chartValueRange = ( ChartValueRangeRecord )record;
    }
    /// <summary>
    /// Parses walls or floor.
    /// </summary>
    /// <param name="data">Record storage.</param>
    /// <param name="iPos">Position in storage.</param>
    protected override void ParseWallsOrFloor( IList<BiffRecordRaw> data, ref int iPos )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      ParentChart.Floor = new ChartWallOrFloorImpl( Application, ParentChart, false
        , data, ref iPos );
    }
    /// <summary>
    /// Parses data.
    /// </summary>
    /// <param name="record">Represents current record to parse.</param>
    /// <param name="data">Represents records storage.</param>
    /// <param name="iPos">Represents position in storage.</param>
    [ CLSCompliant( false ) ]
    protected override void ParseData( BiffRecordRaw record, IList<BiffRecordRaw> data, ref int iPos )
    {
      if( record == null )
        throw new ArgumentNullException( "record" );

      if( data == null )
        throw new ArgumentNullException( "data" );

      switch( record.TypeCode )
      {
        case TBIFFRecord.ChartValueRange:
          ParseMaxCross( record );
          break;

        case TBIFFRecord.ChartAxisDisplayUnits:
          ParseDisplayUnits( ( ChartAxisDisplayUnitsRecord )record );
          break;

        case TBIFFRecord.ChartBegDispUnit:
          ParseDisplayUnitLabel( data, ref iPos );
          break;
      }
    }
    /// <summary>
    /// Parses display unit record.
    /// </summary>
    /// <param name="record">Represents record to parse.</param>
    private void ParseDisplayUnits( ChartAxisDisplayUnitsRecord record )
    {
      if( record == null )
        throw new ArgumentNullException( "record" );

      m_displayUnit = ( ExcelChartDisplayUnit )record.DisplayUnit;
      m_displayUnitCustom = record.DisplayUnitValue;
      m_bHasDisplayUnitLabel = record.IsShowLabels;
    }
    /// <summary>
    /// Parses display unit label.
    /// </summary>
    /// <param name="data">Record storage.</param>
    /// <param name="iPos">Position in storage.</param>
    private void ParseDisplayUnitLabel( IList<BiffRecordRaw> data, ref int iPos )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      BiffRecordRaw record = ( BiffRecordRaw )data[ iPos ];
      record.CheckTypeCode( TBIFFRecord.ChartBegDispUnit );

      iPos++;

      record = ChartTextAreaImpl.UnwrapRecord( ( BiffRecordRaw )data[ iPos ] );

      while( record.TypeCode != TBIFFRecord.ChartEndDispUnit )
      {
        switch( record.TypeCode )
        {
          case TBIFFRecord.ChartText:
            m_displayUnitLabel = new ChartWrappedTextAreaImpl( Application, this, data, ref iPos );
            iPos--;
            break;
        }

        iPos++;
        record = ChartTextAreaImpl.UnwrapRecord( ( BiffRecordRaw )data[ iPos ] );
      }
    }
    #endregion

    #region Class serialization methods
    /// <summary>
    /// Serializes axis.
    /// </summary>
    /// <param name="records">OffsetArrayList that will receive all records.</param>
    [ CLSCompliant( false ) ]
    public override void Serialize( OffsetArrayList records )
    {
      Serialize( records, ChartAxisRecord.ChartAxisType.ValueAxis );
    }
    /// <summary>
    /// Serializes axis.
    /// </summary>
    /// <param name="records">OffsetArrayList that will receive all records.</param>
    /// <param name="axisType">Represents axis type.</param>
    [ CLSCompliant( false ) ]
    protected void Serialize( OffsetArrayList records, ChartAxisRecord.ChartAxisType axisType )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      ChartAxisRecord axis = (ChartAxisRecord)
        BiffRecordFactory.GetRecord( TBIFFRecord.ChartAxis );
      axis.AxisType = axisType;

      records.Add( axis );
      records.Add( BiffRecordFactory.GetRecord( TBIFFRecord.Begin ) );

      records.Add( ( BiffRecordRaw )m_chartValueRange.Clone() );
      SerializeDisplayUnits( records );

      SerializeNumberFormat( records );
      SerializeTickRecord( records );
      SerializeFont( records );

      SerializeAxisBorder( records );

      if( IsPrimary )
      {
        SerializeGridLines( records );
        SerializeWallsOrFloor( records );
      }

      records.Add( BiffRecordFactory.GetRecord( TBIFFRecord.End ) );
    }
    /// <summary>
    /// Serializes walls or floor.
    /// </summary>
    /// <param name="records">Record storage.</param>
    [ CLSCompliant( false ) ]
    protected virtual void SerializeWallsOrFloor( OffsetArrayList records )
    {
      ParentChart.SerializeFloor( records );
    }
    /// <summary>
    /// Serialize display units.
    /// </summary>
    /// <param name="records">Represents records storage.</param>
    private void SerializeDisplayUnits( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      if( m_displayUnit == ExcelChartDisplayUnit.None )
        return;

      ChartAxisDisplayUnitsRecord record = ( ChartAxisDisplayUnitsRecord )BiffRecordFactory.GetRecord(
        TBIFFRecord.ChartAxisDisplayUnits );

      record.IsShowLabels = m_bHasDisplayUnitLabel;
      record.DisplayUnitValue = m_displayUnitCustom;
      record.DisplayUnit = m_displayUnit;
      records.Add( record );

      ChartBegDispUnitRecord begRec = ( ChartBegDispUnitRecord )BiffRecordFactory.GetRecord(
        TBIFFRecord.ChartBegDispUnit );
      begRec.IsShowLabels = m_bHasDisplayUnitLabel;
      records.Add( begRec );

      if( m_bHasDisplayUnitLabel )
        m_displayUnitLabel.Serialize( records );

      ChartEndDispUnitRecord endRec = ( ChartEndDispUnitRecord )BiffRecordFactory.GetRecord(
        TBIFFRecord.ChartEndDispUnit );
      endRec.IsShowLabels = m_bHasDisplayUnitLabel;
      records.Add( endRec );
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// Initializes internal variables.
    /// </summary>
    protected override void InitializeVariables()
    {
      m_chartValueRange = ( ChartValueRangeRecord )BiffRecordFactory.GetRecord(
        TBIFFRecord.ChartValueRange );

      base.InitializeVariables ();
    }

    /// <summary>
    /// Checks if everything is ok with ChartValueRangeRecord.
    /// </summary>
    /// <returns>True if check succeeded.</returns>
    protected bool CheckValueRangeRecord()
    {
      // Exception throwing was disable on loading because sometimes we are detecting chart type after axis were parsed.
      return CheckValueRangeRecord(! ( ParentWorkbook.Loading ||  ParentWorkbook.Saving || ParentWorkbook.IsCreated) );
    }
    /// <summary>
    /// Checks if everything is ok with ChartValueRangeRecord.
    /// </summary>
    /// <param name="throwException">Indicates whether we should throw an exception in the case of check failed.</param>
    /// <returns>True if check succeeded.</returns>
    protected virtual bool CheckValueRangeRecord( bool throwException )
    {
      return true;
    }
    /// <summary>
    /// Gets text link for this axis.
    /// </summary>
    [ CLSCompliant( false ) ]
    protected override ExcelObjectTextLink TextLinkType
    {
      get
      {
        return ExcelObjectTextLink.YAxis;
      }
    }
    /// <summary>
    /// Clone current object.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    /// <param name="dicFontIndexes">Dictionary with new indexes.</param>
    /// <param name="dicNewSheetNames">Dictionary with new worksheet names.</param>
    /// <returns>Returns cloned object.</returns>
    public override ChartAxisImpl Clone( object parent, Dictionary<int, int> dicFontIndexes,
      Dictionary<string, string> dicNewSheetNames )
    {
      ChartValueAxisImpl result = ( ChartValueAxisImpl )base.Clone( parent, dicFontIndexes, dicNewSheetNames );

      if( m_chartValueRange != null )
      {
        result.m_chartValueRange = ( ChartValueRangeRecord )m_chartValueRange.Clone();
      }

      if( m_displayUnitLabel != null )
        result.m_displayUnitLabel = ( ChartWrappedTextAreaImpl )
          m_displayUnitLabel.Clone( result, dicFontIndexes, dicNewSheetNames );

      return result;
    }
    /// <summary>
    /// Creates display unit label.
    /// </summary>
    private void CreateDispalayUnitLabel()
    {
      m_displayUnitLabel = new ChartWrappedTextAreaImpl( Application, this, ExcelObjectTextLink.DisplayUnit );
      m_displayUnitLabel.TextRecord.IsAutoText = true;
      m_displayUnitLabel.Bold = true;
      m_displayUnitLabel.IsAutoMode = true;

      if( AxisType == ExcelAxisType.Value )
        m_displayUnitLabel.TextRotationAngle = 90;
    }

    public void SetMajorUnit( double value )
    {
      if( value <= 0 || ( !IsAutoMinor && value < MinorUnit ) )
        throw new ArgumentOutOfRangeException( "MajorUnit" );

      m_chartValueRange.NumMajor = value;
      IsAutoMajor = false;
    }
    public void SetMinorUnit( double value )
    {
      if( value <= 0 || ( !IsAutoMajor && value > m_chartValueRange.NumMajor ) )
        throw new ArgumentOutOfRangeException( "MinorUnit" );

      m_chartValueRange.NumMinor = value;
      IsAutoMinor = false;
    }
    #endregion
  }
}
