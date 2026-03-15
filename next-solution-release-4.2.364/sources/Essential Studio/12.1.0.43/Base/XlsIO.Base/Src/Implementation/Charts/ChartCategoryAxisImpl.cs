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

using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser.Biff_Records.Charts;

using LinkIndex = Syncfusion.XlsIO.Parser.Biff_Records.Charts.ChartAIRecord.LinkIndex;
using ReferenceType = Syncfusion.XlsIO.Parser.Biff_Records.Charts.ChartAIRecord.ReferenceType;
using Syncfusion.XlsIO.Implementation.XmlSerialization.Constants;
using System.Collections.Generic;
using Syncfusion.XlsIO.Implementation.Exceptions;
#endregion

namespace Syncfusion.XlsIO.Implementation.Charts
{
  /// <summary>
  /// Class used for Chart Category Axis implementation.
  /// </summary>
  public class ChartCategoryAxisImpl
    : ChartValueAxisImpl
    , IChartCategoryAxis
  {
    #region Class constants
    /// <summary>
    /// Error message for not supported property.
    /// </summary>
    private const string DEF_NOTSUPPORTED_PROPERTY = "This property is not supported for the current chart type";
    /// <summary>
    /// Represents default offset.
    /// </summary>
    private const int DEF_AXIS_OFFSET = 100;
    /// <summary>
    /// Represents month count.
    /// </summary>
    private const int DEF_MONTH_COUNT = 12;
    /// <summary>
    /// Represents min axis data.
    /// </summary>
    private static readonly DateTime DEF_MIN_DATE = new DateTime( 1900, 1, 1 );
    #endregion

    #region Class members
    /// <summary>
    /// ChartCatserRangeRecord that describes this axis.
    /// </summary>
    private ChartCatserRangeRecord m_chartCatser;
     /// <summary>
    /// UnknownRecord that describes label intervals of this axis.
    /// </summary>
    private UnknownRecord m_chartMlFrt;
    /// <summary>
    /// Represents time scale data.
    /// </summary>
    private ChartAxcextRecord m_axcetRecord = ( ChartAxcextRecord )
      BiffRecordFactory.GetRecord( TBIFFRecord.ChartAxcext );
    /// <summary>
    /// Represents category axis type.
    /// </summary>
    private ExcelCategoryType m_categoryType = ExcelCategoryType.Automatic;
    /// <summary>
    /// Represents default axis offset.
    /// </summary>
    private int m_iOffset = 100;
    /// <summary>
    /// Represents the crosses in axis
    /// </summary>
    private bool m_isMaxCross = false;
    /// <summary>
    /// Indicates whether tick label spacing value is automatically evaluated.
    /// </summary>
    private bool m_bAutoTickLabelSpacing = true;
    /// <summary>
    /// Indicates whether multi level label is allowed or not
    /// </summary>
    private bool m_bnoMultiLvlLbl = false;
    /// <summary>
    /// Represents to serialize NoMultiLvlLbl attribute or not
    /// </summary>
    internal bool m_showNoMultiLvlLbl = false;
    /// <summary>
    /// Represents to serialize majorUnitScale is Auto
    /// </summary>
    private bool m_majorUnitIsAuto;
    /// <summary>
    /// Represents to serialize minorUnitScale is Auto
    /// </summary>
    private bool m_minorUnitIsAuto;
    #endregion

    #region Class constructors
    /// <summary>
    /// Creates axis object.
    /// </summary>
    /// <param name="application">Application object for the axis.</param>
    /// <param name="parent">Parent object for the axis.</param>
    public ChartCategoryAxisImpl( IApplication application, object parent )
      : base( application, parent )
    {
      AxisId = IsPrimary ?
        ChartConstants.DefaultCategoryAxisId :
        ChartConstants.DefaultSecondaryCategoryAxisId;
      m_majorUnitIsAuto = true;
      m_minorUnitIsAuto = true;
    }
    /// <summary>
    /// Creates primary axis of specified type.
    /// </summary>
    /// <param name="application">Application object for the axis.</param>
    /// <param name="parent">Parent object for the axis.</param>
    /// <param name="axisType">Type of the new axis.</param>
    public ChartCategoryAxisImpl( IApplication application, object parent, ExcelAxisType axisType )
      : this( application, parent, axisType, true )
    {
        m_majorUnitIsAuto = true;
        m_minorUnitIsAuto = true;
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
    public ChartCategoryAxisImpl( IApplication application, object parent,
      ExcelAxisType axisType, bool bIsPrimary )
      : base( application, parent, axisType, bIsPrimary )
    {
      AxisId = IsPrimary ?
        ChartConstants.DefaultCategoryAxisId :
        ChartConstants.DefaultSecondaryCategoryAxisId;

      if( !IsPrimary )
        Visible = false;
      m_majorUnitIsAuto = true;
      m_minorUnitIsAuto = true;
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
    public ChartCategoryAxisImpl( IApplication application, object parent
      , IList<BiffRecordRaw> data, ref int iPos )
      : this( application, parent, data, ref iPos, true )
    {
        m_majorUnitIsAuto = true;
        m_minorUnitIsAuto = true;
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
    public ChartCategoryAxisImpl( IApplication application, object parent
      , IList<BiffRecordRaw> data, ref int iPos, bool isPrimary )
      : base( application, parent, data, ref iPos, isPrimary )
    {
      AxisId = IsPrimary ?
        ChartConstants.DefaultCategoryAxisId :
        ChartConstants.DefaultSecondaryCategoryAxisId;
      m_majorUnitIsAuto = true;
      m_minorUnitIsAuto = true;
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Value axis crosses at the far right category (in a line, bar, 
    /// column, scatter, or area chart; 2D charts only).
    /// </summary>
    public override bool IsMaxCross
    {
      get
      {
          return m_isMaxCross;
      }
      set
      {
        if( IsChartBubbleOrScatter )
        {
           base.IsMaxCross = value;
        }
        else 
        {
          m_chartCatser.IsMaxCross = value;
        }

        m_isMaxCross = value;
      }
    }

    /// <summary>
    /// Represents the point on the axis another axis crosses it.
    /// </summary>
    public override double CrossesAt
    {
      get
      {
        return ( IsChartBubbleOrScatter )
          ? base.CrossesAt
          : m_chartCatser.CrossingPoint;
      }
      set
      {
        if( IsChartBubbleOrScatter )
        {
          base.CrossesAt = value;
        }
        else 
        {
          if( ( value < 1 || value > 31999 ) && !ParentWorkbook.Loading )
            throw new ArgumentOutOfRangeException( "For current chart type valid number must be between 1 to 3199" );

          m_chartCatser.CrossingPoint = ( ushort )value;

          //if( !IsCategoryType )
            IsAutoCross = false;
        }
      }
    }
    /// <summary>
    /// Represents whether the label spacing is automatic or not
    /// </summary>
    public bool AutoTickLabelSpacing
    {
        get
        {
            if (TickLabelSpacing != 1)
                return false;
            else
                return m_bAutoTickLabelSpacing;
        }
        set
        {
            if (value == true)
                TickLabelSpacing = 1;
            m_bAutoTickLabelSpacing = value;           
        }
    }
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
        return m_chartCatser.LabelsFrequency;
      }
      set
      {
        if( value < 0 )
          throw new ArgumentOutOfRangeException( "value", "Value cannot be less than 0" );

        m_chartCatser.LabelsFrequency = ( ushort )value;
        AutoTickLabelSpacing = false;
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
        AutoTickMarkSpacing = false;
      }
    }
    /// <summary>
    /// Represents the number of categories or series between tick marks.
    /// </summary>
    public int TickMarkSpacing
    {
      get
      {
        return m_chartCatser.TickMarksFrequency;
      }
      set
      {
        if( value < 0 )
          throw new ArgumentOutOfRangeException( "value", "Value cannot be less than 0" );

        AutoTickMarkSpacing = false;
        m_chartCatser.TickMarksFrequency = ( ushort )value;
      }
    }
    /// <summary>
    /// Creates title area. Read-only.
    /// </summary>
    [ CLSCompliant( false ) ]
    protected override ExcelObjectTextLink TextLinkType
    {
      get
      {
        return ExcelObjectTextLink.XAxis;
      }
    }

    /// <summary>
    /// If false - cuts unused plot area. Default for area, surface charts.
    /// </summary>
    public bool IsBetween
    {
      get
      {
        return CatserRecord.IsBetween;
      }
      set
      {
        CatserRecord.IsBetween = value;
      }
    }
    /// <summary>
    /// True if plots data points from last to first.
    /// </summary>
    public override bool ReversePlotOrder
    {
      get
      {
        if( IsChartBubbleOrScatter )
          return base.ReversePlotOrder;

        return CatserRecord.IsReverse;
      }
      set
      {
        if( IsChartBubbleOrScatter )
        {
          base.ReversePlotOrder = value;
        }
        else
        {
          CatserRecord.IsReverse = value;
          base.ReversePlotOrder = value;
        }
      }
    }
    /// <summary>
    /// Category labels for the chart.
    /// </summary>
    public IRange CategoryLabels
    {
      get
      {
        return ParentChart.Series[ 0 ].CategoryLabels;
      }
      set
      {
        ChartSeriesCollection coll = ( ChartSeriesCollection )ParentChart.Series;

        for( int i = 0, iLen = coll.Count; i < iLen; i ++ )
        {
          coll[ i ].CategoryLabels = value;
        }
      }
    }

    /// <summary>
    /// Entered directly category labels for the chart.
    /// </summary>
    public object[] EnteredDirectlyCategoryLabels
    {
      get
      {
        return ParentChart.Series[ 0 ].EnteredDirectlyCategoryLabels;
      }
      set
      {
        ChartSeriesCollection coll = ( ChartSeriesCollection )ParentChart.Series;

        for( int i = 0, iLen = coll.Count; i < iLen; i ++ )
        {
          coll[ i ].EnteredDirectlyCategoryLabels = value;
        }
      }
    }

    /// <summary>
    /// Represents axis category type.
    /// </summary>
    public ExcelCategoryType CategoryType
    {
      get
      {
        return m_categoryType;
      }
      set
      {
        m_categoryType = value;
      }
    }
    /// <summary>
    /// Represents distance between the labels and axis line.
    /// The value can be from 0 through 1000.
    /// </summary>
    public int Offset
    {
      get
      {
        return m_iOffset;
      }
      set
      {
        if( value < 0 || value > 1000 )
          throw new ArgumentOutOfRangeException( "The value can be from 0 through 1000." );

        m_iOffset = value;
      }
    }
    /// <summary>
    /// Represents base unit for the specified category axis.
    /// </summary>
    public ExcelChartBaseUnit BaseUnit
    {
      get
      {
        CheckTimeScaleProperties();

        return m_axcetRecord.BaseUnits;
      }
      set
      {
        CheckTimeScaleProperties();

        m_axcetRecord.BaseUnits = value;
        m_axcetRecord.UseDefaultBaseUnits = false;
      }
    }
    /// <summary>
    /// True if use automatic base units for the specified category axis.
    /// </summary>
    public bool BaseUnitIsAuto
    {
      get
      {
        CheckTimeScaleProperties();

        return m_axcetRecord.UseDefaultBaseUnits;
      }
      set
      {
        CheckTimeScaleProperties();

        m_axcetRecord.UseDefaultBaseUnits = value;
      }
    }
    /// <summary>
    /// True if use automatic major units for the specified category axis.
    /// </summary> 
    internal bool MajorUnitScaleIsAuto
    {
        get
        {
            return m_majorUnitIsAuto;
        }
        set
        {
            m_majorUnitIsAuto = false;
        }
    }
    /// <summary>
    /// True if use automatic major units for the specified category axis.
    /// </summary> 
    internal bool MinorUnitScaleIsAuto
    {
        get
        {
            return m_minorUnitIsAuto;
        }
        set
        {
            m_minorUnitIsAuto = false;
        }
    }
    /// <summary>
    /// Automatic major selected.
    /// </summary>
    public override bool IsAutoMajor
    {
      get
      {
        if( IsChartBubbleOrScatter )
        {
          return base.IsAutoMajor;
        }
        else
        {
          if( IsCategoryType )
          {
            return true;//throw new NotSupportedException( DEF_NOTSUPPORTED_PROPERTY );
          }
          else
          {
            return m_axcetRecord.UseDefaultMajorUnits;
          }
        }
      }
      set
      {
        if( !IsChartBubbleOrScatter && !IsCategoryType && !ParentWorkbook.Loading &&!ParentWorkbook.IsLoaded && !ParentWorkbook.IsCreated && value != IsAutoMajor )
          throw new NotSupportedException( DEF_NOTSUPPORTED_PROPERTY );

        base.IsAutoMajor = value;
        m_axcetRecord.UseDefaultMajorUnits = value;
        //if( IsChartBubbleOrScatter )
        //{
        //  base.IsAutoMajor = value;
        //}
        //else
        //{
        //  if( IsCategoryType )
        //  {
        //    throw new NotSupportedException( DEF_NOTSUPPORTED_PROPERTY );
        //  }
        //  else
        //  {
        //    m_axcetRecord.UseDefaultMajorUnits = value;
        //  }
        //}
      }
    } 
    /// <summary>
    /// Automatic minor selected.
    /// </summary>
    public override bool IsAutoMinor
    {
      get
      {
        if( IsChartBubbleOrScatter )
        {
          return base.IsAutoMinor;
        }
        else
        {
          if( IsCategoryType )
          {
            return true;//throw new NotSupportedException( DEF_NOTSUPPORTED_PROPERTY );
          }
          else
          {
            return m_axcetRecord.UseDefaultMinorUnits;
          }
        }
      }
      set
      {
          if( !IsChartBubbleOrScatter && !IsCategoryType && !ParentWorkbook.Loading &&!ParentWorkbook.IsLoaded && value != IsAutoMinor )
          throw new NotSupportedException( DEF_NOTSUPPORTED_PROPERTY );

        base.IsAutoMinor = value;
        m_axcetRecord.UseDefaultMinorUnits = value;
        //if( IsChartBubbleOrScatter )
        //{
        //  base.IsAutoMinor = value;
        //}
        //else
        //{
        //  if( IsCategoryType )
        //  {
        //    throw new NotSupportedException( DEF_NOTSUPPORTED_PROPERTY );
        //  }
        //  else
        //  {
        //    m_axcetRecord.UseDefaultMinorUnits = value;
        //  }
        //}
      }
    }
    /// <summary>
    /// Automatic category crossing point selected.
    /// </summary>
    public override bool IsAutoCross
    {
      get
      {
        if( IsChartBubbleOrScatter )
        {
          return base.IsAutoCross;
        }
        else
        {
          //if( IsCategoryType )
          //{
          //  return true;//throw new NotSupportedException( DEF_NOTSUPPORTED_PROPERTY );
          //}
          //else
          {
            return m_axcetRecord.UseDefaultCrossPoint;
          }
        }
      }
      set
      {
        base.IsAutoCross = value;

        if( !IsChartBubbleOrScatter )
        {
          //if( IsCategoryType )
          //{
          //  throw new NotSupportedException( DEF_NOTSUPPORTED_PROPERTY );
          //}
          //else
          {
            m_axcetRecord.UseDefaultCrossPoint = value;
            base.IsAutoCross = value;
          }
        }
      }
    }
    /// <summary>
    /// Automatic maximum selected.
    /// </summary>
    public override bool IsAutoMax
    {
      get
      {
        if( IsChartBubbleOrScatter )
        {
          return base.IsAutoMax;
        }
        else
        {
          if( IsCategoryType )
          {
            return true;// throw new NotSupportedException( DEF_NOTSUPPORTED_PROPERTY );
          }
          else
          {
            return m_axcetRecord.UseDefaultMaximum;
          }
        }
      }
      set
      {
        SetAutoMax( false, value );
        m_axcetRecord.UseDefaultMaximum = value;

        if( !IsChartBubbleOrScatter )
        {
          if( IsCategoryType )
          {
            throw new NotSupportedException( DEF_NOTSUPPORTED_PROPERTY );
          }
        }
      }
    }
    /// <summary>
    /// Automatic minimum selected.
    /// </summary>
    public override bool IsAutoMin
    {
      get
      {
        if( IsChartBubbleOrScatter )
        {
          return base.IsAutoMin;
        }
        else
        {
          if( IsCategoryType )
          {
            return true;//throw new NotSupportedException( DEF_NOTSUPPORTED_PROPERTY );
          }
          else
          {
            return m_axcetRecord.UseDefaultMinimum;
          }
        }
      }
      set
      {
        SetAutoMin( false, value );
        m_axcetRecord.UseDefaultMinimum = value;

        if( !IsChartBubbleOrScatter )
        {
          if( IsCategoryType )
          {
            throw new NotSupportedException( DEF_NOTSUPPORTED_PROPERTY );
          }
        }
      }
    }
    /// <summary>
    /// Value of major increment.
    /// </summary>
    public override double MajorUnit
    {
      get
      {
        if( IsChartBubbleOrScatter )
        {
          return base.MajorUnit;
        }
        else
        {
          if( IsCategoryType )
          {
            throw new NotSupportedException( DEF_NOTSUPPORTED_PROPERTY );
          }
          else
          {
            return m_axcetRecord.Major;
          }
        }
      }
      set
      {
        if( IsChartBubbleOrScatter )
        {
          base.MajorUnit = value;
        }
        else
        {
          if( IsCategoryType )
          {
            throw new NotSupportedException( DEF_NOTSUPPORTED_PROPERTY );
          }
          else
          {
            if( value < 1 || ( !IsAutoMinor && value < MinorUnit ) )
              throw new ArgumentOutOfRangeException( "MajorUnit" );
            m_majorUnitIsAuto = false;
            m_axcetRecord.Major = ( ushort )value;
            m_axcetRecord.UseDefaultMajorUnits = false;
          }
        }
      }
    }

    /// <summary>
    /// Value of minor increment.
    /// </summary>
    public override double MinorUnit
    {
      get
      {
        if( IsChartBubbleOrScatter )
        {
          return base.MinorUnit;
        }
        else
        {
          if( IsCategoryType )
          {
            throw new NotSupportedException( DEF_NOTSUPPORTED_PROPERTY );
          }
          else
          {
            return m_axcetRecord.Minor;
          }
        }
      }
      set
      {
        if( IsChartBubbleOrScatter )
        {
          base.MinorUnit = value;
        }
        else
        {
          if( IsCategoryType )
          {
            throw new NotSupportedException( DEF_NOTSUPPORTED_PROPERTY );
          }
          else
          {
            if( ( value < 1 || ( !IsAutoMajor && value > MajorUnit ) ) && !ParentWorkbook.Loading )
              throw new ArgumentOutOfRangeException( "MinorUnit" );

            m_axcetRecord.Minor = ( ushort )value;
            m_axcetRecord.UseDefaultMinorUnits = false;
            m_minorUnitIsAuto = false;
          }
        }
      }    
    }

    /// <summary>
    /// Represents the major unit scale value for the category axis
    ///  when the CategoryType property is set to TimeScale.
    /// </summary>
    public ExcelChartBaseUnit MajorUnitScale
    {
      get
      {
        CheckTimeScaleProperties();

        return m_axcetRecord.MajorUnits;
      }
      set
      {
        CheckTimeScaleProperties();

        if( !IsAutoMinor && ( int )value < ( int )MinorUnitScale )
          throw new ArgumentOutOfRangeException( DEF_NOTSUPPORTED_PROPERTY );
        m_majorUnitIsAuto = false;
        m_axcetRecord.MajorUnits = value;
      }
    }
    /// <summary>
    /// Represents the minor unit scale value for the category axis
    ///  when the CategoryType property is set to TimeScale.
    /// </summary>
    public ExcelChartBaseUnit MinorUnitScale
    {
      get
      {
        CheckTimeScaleProperties();

        return m_axcetRecord.MinorUnits;
      }
      set
      {
        CheckTimeScaleProperties();

        if( !IsAutoMajor && ( int )MajorUnitScale < ( int )value )
          throw new ArgumentOutOfRangeException( DEF_NOTSUPPORTED_PROPERTY );
        m_minorUnitIsAuto = false;
        m_axcetRecord.MinorUnits = value;
      }
    }
    /// <summary>
    /// Represents whether axis labels allow multi level string or not
    /// </summary>
    public bool NoMultiLevelLabel
    {
        get
        {
            return m_bnoMultiLvlLbl;
        }
        set
        {
            m_bnoMultiLvlLbl = value;
        }
    }
    #endregion

    #region Class parse methods
    /// <summary>
    /// Parses data.
    /// </summary>
    /// <param name="record">Represents data to parse.</param>
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
        case TBIFFRecord.ChartAxcext:
          m_axcetRecord = ( ChartAxcextRecord )record;
          ParseCategoryType( m_axcetRecord );
          break;

        case TBIFFRecord.ChartAxisOffset:
          m_iOffset = ( ( ChartAxisOffsetRecord )record ).Offset;
          break;

        case TBIFFRecord.ChartCatserRange:
        case TBIFFRecord.ChartValueRange:
          ParseMaxCross( record );
          break;

        case TBIFFRecord.ChartMlFrt:
          m_chartMlFrt = ( UnknownRecord ) record;
          break;

        default:
          base.ParseData( record, data, ref iPos );
          break;
      }
    }
    /// <summary>
    /// Parses max cross.
    /// </summary>
    /// <param name="record">Represents max cross data to parse.</param>
    [ CLSCompliant( false ) ]
    protected override void ParseMaxCross( BiffRecordRaw record )
    {
      if( record == null )
        throw new ArgumentNullException( "record" );

      switch( record.TypeCode )
      {
        case TBIFFRecord.ChartCatserRange:
          m_chartCatser = ( ChartCatserRangeRecord )record;
          break;

        case TBIFFRecord.ChartValueRange:
          ChartValueRange = ( ChartValueRangeRecord )record;
          break;

        default:
          throw new ApplicationException( "Unknown record type" );
      }
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

      ParentChart.Walls = new ChartWallOrFloorImpl( Application, ParentChart, true, data, ref iPos );
    }
    /// <summary>
    /// Parses category axis type.
    /// </summary>
    /// <param name="record">Represents record for parsing.</param>
    private void ParseCategoryType( ChartAxcextRecord record )
    {
      if( record == null )
        throw new ArgumentNullException( "record" );

      if( record.UseDefaultDateSettings )
      {
        m_categoryType = ExcelCategoryType.Automatic;
      }
      else
      {
        m_categoryType = ( record.IsDateAxis )
          ? ExcelCategoryType.Time
          : ExcelCategoryType.Category;
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
      if( records == null )
        throw new ArgumentNullException( "records" );

      if( IsChartBubbleOrScatter )
      {
        base.Serialize( records, ChartAxisRecord.ChartAxisType.CategoryAxis );
      }
      else
      {
        SerializeCategory( records );
      }
    }
    /// <summary>
    /// Serializes primary standard category axis.
    /// </summary>
    /// <param name="records">OffsetArrayList that will receive all records.</param>
    private void SerializeCategory( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      ChartAxisRecord axis = (ChartAxisRecord)
        BiffRecordFactory.GetRecord( TBIFFRecord.ChartAxis );
      axis.AxisType = ChartAxisRecord.ChartAxisType.CategoryAxis;

      records.Add( axis );
      records.Add( BiffRecordFactory.GetRecord( TBIFFRecord.Begin ) );

      records.Add( ( BiffRecordRaw )m_chartCatser.Clone() );

      SerializeAxcetRecord( records );
      SerializeNumberFormat( records );

      //// This record is required by Excel 2007 to display category labels correctly.
      //UnknownRecord unknown = new UnknownRecord();
      //unknown.RecordCode = 0x856;
      //unknown.m_data = new byte[]
      //{
      //  0x56, 0x08, 0x00, 0x00, 0x64, 0x00, 0x02, 0x00, 0xEC, 0x2B, 0x00, 0x00
      //};
      //unknown.Length = unknown.m_data.Length;
      //records.Add( unknown );

      //if( m_iOffset != DEF_AXIS_OFFSET )
      {
        ChartAxisOffsetRecord rec = ( ChartAxisOffsetRecord )BiffRecordFactory.GetRecord(
          TBIFFRecord.ChartAxisOffset );

        rec.Offset = Offset;
        records.Add( rec );
      }

      SerializeTickRecord( records );
      SerializeFont( records );
      SerializeAxisBorder( records );

      if (m_chartMlFrt != null)
          records.Add(m_chartMlFrt);
      else if (AutoTickLabelSpacing)
      {
          // This record is required by Excel 2003 to display the axis label intervals as automatic by default.
          UnknownRecord crtMlFrt = new UnknownRecord();
          crtMlFrt.RecordCode = 0x89E;
          crtMlFrt.m_data = new byte[]
            {
                0x09E, 0x008, 0x000, 0x000, 0x000, 0x000, 0x000, 0x000,
                0x000, 0x000, 0x000, 0x000, 0x00C, 0x000, 0x000, 0x000,
                0x000, 0x000, 0x004, 0x000, 0x004, 0x000, 0x051, 0x000,
                0x001, 0x000, 0x000, 0x000, 0x000, 0x000, 0x000, 0x000
            };
          crtMlFrt.Length = crtMlFrt.m_data.Length;
          records.Add(crtMlFrt);
      }
      else
      {
          UnknownRecord crtMlFrt = new UnknownRecord();
          crtMlFrt.RecordCode = 0x89E;
          crtMlFrt.m_data = new byte[]
            {
                0x09E, 0x008, 0x000, 0x000, 0x000, 0x000, 0x000, 0x000,
                0x000, 0x000, 0x000, 0x000, 0x00C, 0x000, 0x000, 0x000,
                0x000, 0x000, 0x004, 0x000, 0x004, 0x000, 0x052, 0x000,
                0x001, 0x000, 0x000, 0x000, 0x000, 0x000, 0x000, 0x000
            };
          crtMlFrt.Length = crtMlFrt.m_data.Length;
          records.Add(crtMlFrt);
      }
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
    protected override void SerializeWallsOrFloor( OffsetArrayList records )
    {
      ParentChart.SerializeWalls( records );
    }
    /// <summary>
    /// Serialize category axis type.
    /// </summary>
    /// <param name="records">Represents record storage.</param>
    private void SerializeAxcetRecord( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      m_axcetRecord.UseDefaultDateSettings = ( m_categoryType == ExcelCategoryType.Automatic )
        ? true
        : false;

      m_axcetRecord.IsDateAxis = ( m_categoryType == ExcelCategoryType.Time )
        ? true
        : false;

      records.Add( m_axcetRecord );
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// Initializes internal variables.
    /// </summary>
    protected override void InitializeVariables()
    {
      m_chartCatser = ( ChartCatserRangeRecord )BiffRecordFactory.GetRecord(
        TBIFFRecord.ChartCatserRange );

      ChartValueRange = ( ChartValueRangeRecord )BiffRecordFactory.GetRecord(
        TBIFFRecord.ChartValueRange );

      base.InitializeVariables();
    }
    /// <summary>
    /// Checks if everything is ok with ChartValueRangeRecord.
    /// </summary>
    /// <param name="throwException">Indicates whether we should throw an exception in the case of check failed.</param>
    /// <returns>True if check succeeded.</returns>
    protected override bool CheckValueRangeRecord( bool throwException )
    {
      bool bResult = IsChartBubbleOrScatter;

      if( throwException && !bResult )
      {
        throw new NotSupportedException( DEF_NOTSUPPORTED_PROPERTY );
      }

      return bResult;
    }
    /// <summary>
    /// Clones current object.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    /// <param name="dicFontIndexes">Dictionary with new indexes.</param>
    /// <param name="dicNewSheetNames">Dictionary with new worksheet names.</param>
    /// <returns>Returns cloned object.</returns>
    public override ChartAxisImpl Clone( object parent, Dictionary<int, int> dicFontIndexes,
      Dictionary<string, string> dicNewSheetNames )
    {
      ChartCategoryAxisImpl result = ( ChartCategoryAxisImpl )
        base.Clone( parent, dicFontIndexes, dicNewSheetNames );

      if( m_chartCatser != null )
        result.m_chartCatser = ( ChartCatserRangeRecord )m_chartCatser.Clone();

      if( m_axcetRecord != null )
        result.m_axcetRecord = ( ChartAxcextRecord )m_axcetRecord.Clone();

      return result;
    }
    /// <summary>
    /// Gets start chart type only for series type.
    /// </summary>
    /// <returns>Returns start chart type.</returns>
    private string GetStartChartType()
    {
      IChartSeries series = ParentChart.Series;

      if( series.Count == 0 )
        return ChartFormatImpl.GetStartSerieType( ParentChart.ChartType );

      string strStartType = ( series[ 0 ] as ChartSerieImpl ).StartType;//ChartFormatImpl.GetStartSerieType( series[ 0 ].SerieType );
      //add optimization later.

      for( int i = 1, iLen = series.Count; i < iLen; i++ )
      {
        ChartSerieImpl currentSerie = ( ChartSerieImpl )series[ i ];
        string strCurType = currentSerie.StartType;

        if( strCurType != strStartType )
          return ChartFormatImpl.GetStartSerieType( ExcelChartType.Combination_Chart );
      }

      return strStartType;
    }
    /// <summary>
    /// Checks for time scale axis mode. Otherwise rise exception.
    /// </summary>
    private void CheckTimeScaleProperties()
    {
      if( IsCategoryType || IsChartBubbleOrScatter )
        throw new NotSupportedException( "Current chart doesnot support this property." );
    }
    #endregion

    #region Class helper properties
    /// <summary>
    /// Returns ChartCatserRangeRecord record. Read-only.
    /// </summary>
    private ChartCatserRangeRecord CatserRecord
    {
      get
      {
        if( m_chartCatser == null )
        {
          m_chartCatser = ( ChartCatserRangeRecord )
            BiffRecordFactory.GetRecord( TBIFFRecord.ChartCatserRange );
        }

        return m_chartCatser;
      }
    }
    /// <summary>
    /// Indicates is chart bubble or scatter. Using series start type.
    /// </summary>
    internal bool IsChartBubbleOrScatter
    {
      get
      {
        string strStartType = GetStartChartType();

        return strStartType == ChartImpl.START_BUBBLE || strStartType == ChartImpl.START_SCATTER;
      }
    }
    /// <summary>
    /// Indicates is category axis type is category. Read-only.
    /// </summary>
    private bool IsCategoryType
    {
      get
      {
        return  m_categoryType == ExcelCategoryType.Category;
      }
    }
    #endregion
  }
}
