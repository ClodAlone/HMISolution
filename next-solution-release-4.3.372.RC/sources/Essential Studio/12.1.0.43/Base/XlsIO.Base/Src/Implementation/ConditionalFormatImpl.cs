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

using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser.Biff_Records.Formula;

using Syncfusion.XlsIO.Implementation.Collections;
using Syncfusion.XlsIO.Interfaces;
using System.Collections.Generic;

#if ( WINRT )
using Windows.UI;
using Rectangle = Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
using System.Globalization;
#endif

#if  (SILVERLIGHT)
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.Silverlight.Rectangle;
using System.Globalization;
#elif WP
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.WP.Rectangle;
using System.Globalization;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;
using System.Globalization;

#endif
#endregion

namespace Syncfusion.XlsIO.Implementation
{
  /// <summary>
  /// Contains a condition and the formatting attributes 
  /// applied to the cells, if the condition is met.
  /// Used for single-cell range
  /// </summary>
  public class ConditionalFormatImpl
    : CommonObject
    , IInternalConditionalFormat
    , IConditionalFormat
    , ICloneParent
  {
    #region Class constants
    /// <summary>
    /// Represents constant that not contain font color.
    /// </summary>
    private const uint DEF_NOT_CONTAIN_FONT_COLOR = 4294967295;
    /// <summary>
    /// Default Formula for Blank conditional Formatting type
    /// </summary>
    private const string DefaultBlankFormula = "LEN(TRIM({0}))=0";
    /// <summary>
    ///  Default Formula for No Blank conditional Formatting type
    /// </summary>
    private const string DefaultNoBlankFormula = "LEN(TRIM({0}))>0";
    /// <summary>
    ///  Default Formula for Error conditional Formatting type
    /// </summary>
    private const string DefaultErrorFormula = "ISERROR({0})";
    /// <summary>
    ///  Default Formula for NoError conditional Formatting type
    /// </summary>
    private const string DefaultNotErrorFormula = "NOT(ISERROR({0}))";
    /// <summary>
    ///   Default Formula for Begins With Specific Text conditional Formatting type
    /// </summary>
    private const string DefaultBeginsWithFormula = "LEFT({0},LEN({1}))={1}";
    /// <summary>
    ///   Default Formula for Ends With Specific Text conditional Formatting type
    /// </summary>
    private const string DefaultEndsWithFormula = "RIGHT({0},LEN({1}))={1}";
    /// <summary>
    ///    Default Formula for Contains Text Specific Text conditional Formatting type
    /// </summary>
    private const string DefaultContainsTextFormula = "NOT(ISERROR(SEARCH({0},{1})))";
    /// <summary>
    ///    Default Formula for NotContains Text Specific Text conditional Formatting type
    /// </summary>
    private const string DefaultNotContainsTextFormula = "ISERROR(SEARCH({0},{1}))";
    /// <summary>
    /// Default formula for yesterday's time period type
    /// </summary>
    private const string DefaultYesterdayTimePeriodFormula = "FLOOR({0},1)=TODAY()-1";
    /// <summary>
    /// Default formula for today's time period ytype
    /// </summary>
    private const string DefaultTodayTimePeriodFormula = "FLOOR({0},1)=TODAY()";
    /// <summary>
    /// Default formula for tomorrow's time period type
    /// </summary>
    private const string DefaultTomorrowTimePeriodFormula = "FLOOR({0},1)=TODAY()+1";
    /// <summary>
    /// Default formula for last seven days time period type
    /// </summary>
    private const string DefaultLastSevenDaysTimePeriodFormula = "AND(TODAY()-FLOOR({0},1)<=6,FLOOR({0},1)<=TODAY())";
    /// <summary>
    /// Default formula for last week time period type
    /// </summary>
    private const string DefaultLastWeekTimePeriodFormula = "AND(TODAY()-ROUNDDOWN({0},0)>=(WEEKDAY(TODAY())),TODAY()-ROUNDDOWN({0},0)<(WEEKDAY(TODAY())+7))";
    /// <summary>
    /// Default formula for this week time period type
    /// </summary>
    private const string DefaultThisWeekTimePeriodFormula = "AND(TODAY()-ROUNDDOWN({0},0)<=WEEKDAY(TODAY())-1,ROUNDDOWN({0},0)-TODAY()<=7-WEEKDAY(TODAY()))";
    /// <summary>
    ///  Default formula for next week time period type
    /// </summary>
    private const string DefaultNextWeekTimePeriodFormula = "AND(ROUNDDOWN({0},0)-TODAY()>(7-WEEKDAY(TODAY())),ROUNDDOWN({0},0)-TODAY()<(15-WEEKDAY(TODAY())))";
    /// <summary>
    ///  Default formula for last month time period type
    /// </summary>
    private const string DefaultLastMonthTimePeriodFormula = "AND(MONTH({0})=MONTH(EDATE(TODAY(),0-1)),YEAR({0})=YEAR(EDATE(TODAY(),0-1)))";
    /// <summary>
    ///  Default formula for this month time period type
    /// </summary>
    private const string DefaultThisMonthTimePeriodFormula = "AND(MONTH({0})=MONTH(TODAY()),YEAR({0})=YEAR(TODAY()))";
    /// <summary>
    ///  Default formula for next month time period type
    /// </summary>
    private const string DefaultNextMonthTimePeriodFormula = "AND(MONTH({0})=MONTH(EDATE(TODAY(),0+1)),YEAR({0})=YEAR(EDATE(TODAY(),0+1)))";
    #endregion

    #region Class members
    /// <summary>
    /// Record that contains conditional format data.
    /// </summary>
    private CFRecord      m_formatRecord;
    /// <summary>
    /// CFEx record.
    /// </summary>
    private CFExRecord m_cfExRecord;
    /// <summary>
    /// CF12 record.
    /// </summary>
    private CF12Record m_cf12Record;
    /// <summary>
    /// Parent workbook.
    /// </summary>
    private WorkbookImpl  m_book;
    /// <summary>
    /// Color object that stores cf color.
    /// </summary>
    private ColorObject m_color;
    /// <summary>
    /// Color object that stores cf background color.
    /// </summary>
    private ColorObject m_backColor;
    /// <summary>
    /// Color object that stores cf top border color.
    /// </summary>
    private ColorObject m_topBorderColor;
    /// <summary>
    /// Color object that stores cf bottom border color.
    /// </summary>
    private ColorObject m_bottomBorderColor;
    /// <summary>
    /// Color object that stores cf left border color.
    /// </summary>
    private ColorObject m_leftBorderColor;
    /// <summary>
    /// Color object that stores cf right border color.
    /// </summary>
    private ColorObject m_rightBorderColor;
    /// <summary>
    /// Color object that stores cf font color color.
    /// </summary>
    private ColorObject m_fontColor;
    /// <summary>
    /// Data bar settings.
    /// </summary>
    private DataBarImpl m_dataBar;
    /// <summary>
    /// Icon set settings.
    /// </summary>
    private IconSetImpl m_iconSet;
    /// <summary>
    /// Color scale settings.
    /// </summary>
    private ColorScaleImpl m_colorScale;
    private string m_asteriskRange;
    private IRange m_range;
    /// <summary>
    /// Represents the text value.
    /// </summary>
    private string m_text;
    /// <summary>
    /// Represents the range reference value
    /// </summary>
    private string m_rangeReference;
    /// <summary>
    /// Preserves the conditional formatting custom functions.
    /// </summary>
    internal string m_customFunction = string.Empty;
    /// <summary>
    /// Represents whether the conditional format has extension list or not
    /// </summary>
    private bool m_cfHasExtensionList;
    /// <summary>
    /// Represents the GUID for the conditional format extension list
    /// </summary>
    internal string ST_GUID;
    /// <summary>
    /// Represents the Time Period conditional formatting types.
    /// </summary> 
    private CFTimePeriods m_CFTimePeriod;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Creates ConditionalFormat and set its Application and Parent
    /// properties to specified values.
    /// </summary>
    /// <param name="application">Application object for the ConditionalFormat.</param>
    /// <param name="parent">Parent object for the ConditionalFormat.</param>
    public ConditionalFormatImpl( IApplication application, object parent )
      : base( application, parent )
    {
      SetParents();
      m_formatRecord = ( CFRecord )BiffRecordFactory.GetRecord( TBIFFRecord.CF );
      InitializeColors();

      m_cf12Record = (CF12Record)BiffRecordFactory.GetRecord(TBIFFRecord.CF12);
      m_cfExRecord = (CFExRecord)BiffRecordFactory.GetRecord(TBIFFRecord.CFEx);
    }
    /// <summary>
    /// Creates ConditionalFormat from array of BiffRecords.
    /// </summary>
    /// <param name="application">Application object for the ConditionalFormat.</param>
    /// <param name="parent">Parent object for the ConditionalFormat.</param>
    /// <param name="data">Array of BiffRecords.</param>
    /// <param name="iPos">Position of the corresponding CFRecord in the array.</param>
    [ CLSCompliant( false ) ]
    public ConditionalFormatImpl( IApplication application, object parent
      , BiffRecordRaw[] data, ref int iPos )
      : this( application, parent )
    {
      Parse( data, ref iPos );
    }
    /// <summary>
    /// Creates ConditionalFormat from CFRecord.
    /// </summary>
    /// <param name="application">Application object for the ConditionalFormat.</param>
    /// <param name="parent">Parent object for the ConditionalFormat.</param>
    /// <param name="cf">CFRecord to parse.</param>
    [ CLSCompliant( false ) ]
    public ConditionalFormatImpl( IApplication application, object parent, CFRecord cf )
      : this( application, parent )
    {
      m_formatRecord = ( CFRecord ) cf.Clone();
      ParseRecord();
    }
    /// <summary>
    /// Creates ConditionalFormat from CF12Record.
    /// </summary>
    /// <param name="application">Application object for the ConditionalFormat.</param>
    /// <param name="parent">Parent object for the ConditionalFormat.</param>
    /// <param name="cf12">CF12Record to parse.</param>
    [CLSCompliant(false)]
    public ConditionalFormatImpl(IApplication application, object parent, CF12Record cf12)
        : this(application, parent)
    {
        m_cf12Record = (CF12Record)cf12.Clone();
    }
    /// <summary>
    /// Creates ConditionalFormat from CFExRecord.
    /// </summary>
    /// <param name="application">Application object for the ConditionalFormat.</param>
    /// <param name="parent">Parent object for the ConditionalFormat.</param>
    /// <param name="cfEx">CFExRecord to parse.</param>
    [CLSCompliant(false)]
    public ConditionalFormatImpl(IApplication application, object parent, CFExRecord cfEx)
        : this(application, parent)
    {
        m_cfExRecord = (CFExRecord)cfEx.Clone();
    }
    #endregion

    #region Class Serialization Methods
    /// <summary>
    /// Parses Conditional format's data from an array of BiffRecords.
    /// </summary>
    /// <param name="data">Array of BiffRecords.</param>
    /// <param name="iPos">Position of the corresponding CFRecord in the array.</param>
    [ CLSCompliant( false ) ]
    public void Parse( BiffRecordRaw[] data, ref int iPos )
    {
      data[ iPos ].CheckTypeCode( TBIFFRecord.CF );

      m_formatRecord = ( CFRecord ) data[ iPos ];
      ParseRecord();

      data[iPos].CheckTypeCode(TBIFFRecord.CF12);
      m_cf12Record = (CF12Record)data[iPos];
      
      data[iPos].CheckTypeCode(TBIFFRecord.CFEx);
      m_cfExRecord = (CFExRecord)data[iPos];

      iPos++;
    }
    /// <summary>
    /// Initializes color objects.
    /// </summary>
    private void InitializeColors()
    {
      m_color = new ColorObject( ( ExcelKnownColors )ExtendedFormatRecord.DEF_DEFAULT_PATTERN_COLOR_INDEX );
      m_color.AfterChange += UpdateColor;

      m_backColor = new ColorObject( ( ExcelKnownColors )ExtendedFormatRecord.DEF_DEFAULT_COLOR_INDEX );
      m_backColor.AfterChange += UpdateBackColor;

      m_topBorderColor = new ColorObject( ExcelKnownColors.None );
      m_topBorderColor.AfterChange += UpdateTopBorderColor;

      m_bottomBorderColor = new ColorObject( ExcelKnownColors.None );
      m_bottomBorderColor.AfterChange += UpdateBottomBorderColor;
      
      m_leftBorderColor = new ColorObject( ExcelKnownColors.None );
      m_leftBorderColor.AfterChange += UpdateLeftBorderColor;

      m_rightBorderColor = new ColorObject( ExcelKnownColors.None );
      m_rightBorderColor.AfterChange += UpdateRightBorderColor;

      m_fontColor = new ColorObject( ExcelKnownColors.None );
      m_fontColor.AfterChange += UpdateFontColor;
    }
    /// <summary>
    /// Updates color objects and sets color values to zero (to make comparison correct).
    /// </summary>
    private void UpdateColorObjects()
    {
      m_color.SetIndexedNoEvent( ( ExcelKnownColors )m_formatRecord.PatternColorIndex );
      m_backColor.SetIndexedNoEvent( ( ExcelKnownColors )m_formatRecord.PatternBackColor );
      m_topBorderColor.SetIndexedNoEvent( ( ExcelKnownColors )m_formatRecord.TopBorderColorIndex );
      m_bottomBorderColor.SetIndexedNoEvent( ( ExcelKnownColors )m_formatRecord.BottomBorderColorIndex );
      m_leftBorderColor.SetIndexedNoEvent( ( ExcelKnownColors )m_formatRecord.LeftBorderColorIndex );
      m_rightBorderColor.SetIndexedNoEvent( ( ExcelKnownColors )m_formatRecord.RightBorderColorIndex );
      m_fontColor.SetIndexedNoEvent( ( ExcelKnownColors )m_formatRecord.FontColorIndex );

      //ZeroRecordColors();
    }
    /// <summary>
    /// Sets all color indexes to zero.
    /// </summary>
    private void ZeroRecordColors()
    {
      //m_formatRecord.PatternColorIndex = 0;
      //m_formatRecord.PatternBackColor = 0;
      //m_formatRecord.TopBorderColorIndex = 0;
      //m_formatRecord.BottomBorderColorIndex = 0;
      //m_formatRecord.LeftBorderColorIndex = 0;
      //m_formatRecord.RightBorderColorIndex = 0;
      //m_formatRecord.FontColorIndex = 0;
    }
    /// <summary>
    /// Parses format record.
    /// </summary>
    private void ParseRecord()
    {
      //FormulaUtil formulaParser = m_book.FormulaUtil;
      //Ptg[] arrPtgs = FormulaUtil.ParseExpression( m_formatRecord.FirstFormulaBytes );
      //m_strFirstFormula = formulaParser.ParsePtgArray( arrPtgs, 0, 0, false );

      //arrPtgs = FormulaUtil.ParseExpression( m_formatRecord.SecondFormulaBytes );
      //m_strSecondFormula = formulaParser.ParsePtgArray( arrPtgs, 0, 0, false );
      UpdateColorObjects();
    }
    /// <summary>
    /// Serializes conditional format's data into OffsetArrayList.
    /// </summary>
    /// <param name="records">OffsetArrayList that will get all data.</param>
    [ CLSCompliant( false ) ]
    public void Serialize( OffsetArrayList records )
    {
      UpdateColors();
      records.Add( m_formatRecord );
      ZeroRecordColors();
    }
    /// <summary>
    /// Serializes conditional format's data into OffsetArrayList.
    /// </summary>
    /// <param name="records">OffsetArrayList that will get all data.</param>
    [CLSCompliant(false)]
    public void SerializeCF12(OffsetArrayList records)
    {
        records.Add(m_cf12Record);
    }    
    /// <summary>
    /// Updates font color inside record.
    /// </summary>
    private void UpdateFontColor()
    {
      m_formatRecord.FontColorIndex = ( ushort )m_fontColor.GetIndexed( m_book );
      m_formatRecord.IsFontFormatPresent = true;
    }
    /// <summary>
    /// Update color value.
    /// </summary>
    private void UpdateColor()
    {
      m_formatRecord.PatternColorIndex = ( ushort )m_color.GetIndexed( m_book );
      m_formatRecord.IsPatternColorModified = true;
      m_formatRecord.IsPatternFormatPresent = true;
    }
    /// <summary>
    /// Updates back color value.
    /// </summary>
    private void UpdateBackColor()
    {
      m_formatRecord.PatternBackColor = ( ushort )m_backColor.GetIndexed( m_book );
      m_formatRecord.IsPatternBackColorModified = true;
      m_formatRecord.IsPatternFormatPresent = true;
    }
    /// <summary>
    /// Updates left border color.
    /// </summary>
    private void UpdateLeftBorderColor()
    {
      m_formatRecord.LeftBorderColorIndex = ( uint )m_leftBorderColor.GetIndexed( m_book );
      IsLeftBorderModified = true;
    }
    /// <summary>
    /// Updates right border color.
    /// </summary>
    private void UpdateRightBorderColor()
    {
      m_formatRecord.RightBorderColorIndex = ( uint )m_rightBorderColor.GetIndexed( m_book );
      IsRightBorderModified = true;
    }
    /// <summary>
    /// Updates top border color.
    /// </summary>
    private void UpdateTopBorderColor()
    {
      m_formatRecord.TopBorderColorIndex = ( uint )m_topBorderColor.GetIndexed( m_book );
      IsTopBorderModified = true;
    }
    /// <summary>
    /// Updates bottom border color.
    /// </summary>
    private void UpdateBottomBorderColor()
    {
      m_formatRecord.BottomBorderColorIndex = ( uint )m_bottomBorderColor.GetIndexed( m_book );
      IsBottomBorderModified = true;
    }
    #endregion

    #region Class Helper Methods
    /// <summary>
    /// Sets items with used reference indexes to true.
    /// </summary>
    /// <param name="usedItems">Array to mark used references in.</param>
    public void MarkUsedReferences( bool[] usedItems )
    {
      FormulaUtil.MarkUsedReferences( m_formatRecord.FirstFormulaPtgs, usedItems );
      FormulaUtil.MarkUsedReferences( m_formatRecord.SecondFormulaPtgs, usedItems );
    }
    /// <summary>
    /// Updates reference indexes.
    /// </summary>
    /// <param name="arrUpdatedIndexes">Array with updated indexes.</param>
    public void UpdateReferenceIndexes( int[] arrUpdatedIndexes )
    {
      Ptg[] tokens = m_formatRecord.FirstFormulaPtgs;

      if( FormulaUtil.UpdateReferenceIndexes( tokens, arrUpdatedIndexes ) )
        m_formatRecord.FirstFormulaPtgs = tokens;

      tokens = m_formatRecord.SecondFormulaPtgs;

      if( FormulaUtil.UpdateReferenceIndexes( tokens, arrUpdatedIndexes ) )
        m_formatRecord.SecondFormulaPtgs = tokens;
    }
    /// <summary>
    /// Searches for necessary parents of the conditional format.
    /// </summary>
    private void SetParents()
    {
      object parent = FindParent( typeof( WorkbookImpl ) );

      if( parent == null )
        throw new ArgumentNullException( "Can't find parent workbook" );

      m_book = ( WorkbookImpl )parent;
    }
    #endregion

    #region IConditionalFormat Members
    /// <summary>
    /// Color of the left line.
    /// </summary>
    public ExcelKnownColors         LeftBorderColor
    {
      get
      {
        return m_leftBorderColor.GetIndexed( m_book );
      }
      set
      {
        value = BorderImpl.NormalizeColor( value );
        m_leftBorderColor.SetIndexed( value );
      }
    }
    /// <summary>
    /// Color of the left line.
    /// </summary>
    public Color                    LeftBorderColorRGB
    {
      get
      {
        return m_leftBorderColor.GetRGB( m_book );
      }
      set
      {
        m_leftBorderColor.SetRGB( value, m_book );
      }
    }
    /// <summary>
    /// Left border line style.
    /// </summary>
    public ExcelLineStyle           LeftBorderStyle
    {
      get
      {
        return m_formatRecord.LeftBorderStyle;
      }
      set
      {
        m_formatRecord.LeftBorderStyle = value;
        IsLeftBorderModified = true;
        m_formatRecord.IsBorderFormatPresent = true;
      }
    }

    /// <summary>
    /// Color of the right line.
    /// </summary>
    public ExcelKnownColors         RightBorderColor
    {
      get
      {
        return m_rightBorderColor.GetIndexed( m_book );
      }
      set
      {
        value = BorderImpl.NormalizeColor( value );
        m_rightBorderColor.SetIndexed( value );
      }
    }
    /// <summary>
    /// Color of the right line.
    /// </summary>
    public Color                    RightBorderColorRGB
    {
      get
      {
        return m_rightBorderColor.GetRGB( m_book );
      }
      set
      {
        m_rightBorderColor.SetRGB( value, m_book );
      }
    }
    /// <summary>
    /// Right border line style.
    /// </summary>
    public ExcelLineStyle           RightBorderStyle
    {
      get
      {
        return m_formatRecord.RightBorderStyle;
      }
      set
      {
        m_formatRecord.RightBorderStyle = value;
        IsRightBorderModified = true;
        m_formatRecord.IsBorderFormatPresent = true;
      }
    }

    /// <summary>
    /// Color of the top line.
    /// </summary>
    public ExcelKnownColors         TopBorderColor
    {
      get
      {
        return m_topBorderColor.GetIndexed( m_book );
      }
      set
      {
        value = BorderImpl.NormalizeColor( value );
        m_topBorderColor.SetIndexed( value );
      }
    }
    /// <summary>
    /// Color of the top line.
    /// </summary>
    public Color                    TopBorderColorRGB
    {
      get
      {
        return m_topBorderColor.GetRGB( m_book );
      }
      set
      {
        m_topBorderColor.SetRGB( value, m_book );
      }
    }
    /// <summary>
    /// Top border line style.
    /// </summary>
    public ExcelLineStyle           TopBorderStyle
    {
      get
      {
        return m_formatRecord.TopBorderStyle;
      }
      set
      {
        m_formatRecord.TopBorderStyle = value;
        IsTopBorderModified = true;
        m_formatRecord.IsBorderFormatPresent = true;
      }
    }

    /// <summary>
    /// Color of the bottom line.
    /// </summary>
    public ExcelKnownColors         BottomBorderColor
    {
      get
      {
        return m_bottomBorderColor.GetIndexed( m_book );
      }
      set
      {
        value = BorderImpl.NormalizeColor( value );
        m_bottomBorderColor.SetIndexed( value );
      }
    }
    /// <summary>
    /// Color of the bottom line.
    /// </summary>
    public Color                    BottomBorderColorRGB
    {
      get
      {
        return m_bottomBorderColor.GetRGB( m_book );
      }
      set
      {
        m_bottomBorderColor.SetRGB( value, m_book );
      }
    }
    /// <summary>
    /// Bottom border line style.
    /// </summary>
    public ExcelLineStyle           BottomBorderStyle
    {
      get
      {
        return m_formatRecord.BottomBorderStyle;
      }
      set
      {
        m_formatRecord.BottomBorderStyle = value;
        IsBottomBorderModified = true;
        m_formatRecord.IsBorderFormatPresent = true;
      }
    }

    /// <summary>
    /// First formula.
    /// </summary>
    public string                   FirstFormula
    {
      get
      {
        return GetFirstSecondFormula( m_book.FormulaUtil, true );
        //return m_strFirstFormula;
      }
      set
      {
        if( value[ 0 ] == '=' )
        {
          value = value.Substring( 1 );
        }
        if ((Workbook.Version >= ExcelVersion.Excel2010) && Text == null && !CFHasExtensionList && FormatType == ExcelCFType.SpecificText
            && value.Length <3)
        {
            string m_text = value.ToString();
            string str_formula = string.Empty;
            RangeImpl range = (this.Parent as ConditionalFormats).sheet.Range[m_text.ToString()] as RangeImpl;
            m_range = range;
           
             str_formula=SetSpecificTextFormula(Operator,range);

             value = str_formula;
        }
        if( FirstFormula != value )
        {
            DataValidationImpl.RegisterFunctions(true);
            DataValidationImpl.RegisterFunctions(true);
            Ptg[] ptgs = m_book.FormulaUtil.ParseString(value);
            DataValidationImpl.RegisterFunctions(false);
            DataValidationImpl.RegisterFunctions(false);
          
          m_formatRecord.FirstFormulaPtgs = ptgs;
          if (FormatType != ExcelCFType.ColorScale && FormatType != ExcelCFType.DataBar && FormatType != ExcelCFType.IconSet)
              m_cf12Record.FirstFormulaPtgs = ptgs;
        }
      }
    }
    /// <summary>
    /// First formula in R1C1 notation. Read-only.
    /// </summary>
    public string                   FirstFormulaR1C1
    {
      get
      {
          Ptg[] arrPtg = m_formatRecord.FirstFormulaPtgs;

          if (arrPtg == null) return null;

          ConditionalFormats formats = (ConditionalFormats)this.Parent;
          IRange range = formats.sheet.Range[formats.Address];

           return m_book.FormulaUtil.ParsePtgArray(arrPtg, 0, 0, true, false);
      }
        set
        {
            if (value[0] == '=')
            {
                value = value.Substring(1);
            }

            if (FirstFormulaR1C1 != value)
            {
                DataValidationImpl.RegisterFunctions(true);
                Ptg[] ptgs = m_book.FormulaUtil.ParseString(value,m_range.Worksheet,null,m_range.Row-1,m_range.Column-1,true);
                DataValidationImpl.RegisterFunctions(false);
                m_formatRecord.FirstFormulaPtgs = ptgs;
            }

        }
    }
    /// <summary>
    /// Second formula.
    /// </summary>
    public string                   SecondFormula
    {
      get
      {
        return GetFirstSecondFormula( m_book.FormulaUtil, false );
        //return m_strSecondFormula;
      }
      set
      {
        if( value[ 0 ] == '=' )
        {
          value = value.Substring( 1 );
        }

        if( SecondFormula != value )
        {
          Ptg[] ptgs = m_book.FormulaUtil.ParseString( value );
          m_formatRecord.SecondFormulaPtgs = ptgs;
            if(FormatType!=ExcelCFType.ColorScale && FormatType!=ExcelCFType.DataBar && FormatType!=ExcelCFType.IconSet)
                m_cf12Record.SecondFormulaPtgs = ptgs;
        }
      }
    }
    /// <summary>
    /// Second formula in R1C1 notation. Read-only.
    /// </summary>
    public string                   SecondFormulaR1C1
    {
      get
      {
        Ptg[] arrPtg = m_formatRecord.SecondFormulaPtgs;

        if( arrPtg == null ) return null;

        return m_book.FormulaUtil.ParsePtgArray( arrPtg, 0, 0, true, false );
      }
        set
        {
            if (value[0] == '=')
            {
                value = value.Substring(1);
            }

            if (SecondFormulaR1C1 != value)
            {
                Ptg[] ptgs = m_book.FormulaUtil.ParseString(value, m_range.Worksheet, null, m_range.Row - 1, m_range.Column - 1, true);
                m_formatRecord.SecondFormulaPtgs = ptgs;
            }
        }
    }
    /// <summary>
    /// Type of the conditional format.
    /// </summary>
    public ExcelCFType              FormatType
    {
      get
      {
        return m_formatRecord.FormatType;
      }
      set
      {
        if( m_formatRecord.FormatType != value )
        {
          m_formatRecord.FormatType = value;

          m_dataBar = null;
          m_iconSet = null;
          m_colorScale = null;

          switch( value )
          {
            case ExcelCFType.CellValue:
              Operator = ExcelComparisonOperator.Between;
              break;

            case ExcelCFType.Blank:
              ConditionalFormats parentFormats = Parent as ConditionalFormats;
              FirstFormula = string.Format(DefaultBlankFormula, parentFormats.Address);
              break;

            case ExcelCFType.NoBlank:
              parentFormats = Parent as ConditionalFormats;
              FirstFormula = string.Format(DefaultNoBlankFormula, parentFormats.Address);
              break;

            case ExcelCFType.Formula:
              Operator = ExcelComparisonOperator.None;
              break;

            case ExcelCFType.SpecificText:
              Operator = ExcelComparisonOperator.ContainsText;
              break;

            case ExcelCFType.TimePeriod:
                Operator = ExcelComparisonOperator.None;
                FirstFormula = SetTimePeriodFormula(m_CFTimePeriod);
                break;

            case ExcelCFType.ContainsErrors:
                parentFormats = Parent as ConditionalFormats;
                FirstFormula = string.Format(DefaultErrorFormula, parentFormats.Address);
                break;

            case ExcelCFType.NotContainsErrors:
                parentFormats = Parent as ConditionalFormats;
                FirstFormula = string.Format(DefaultNotErrorFormula, parentFormats.Address);
                break;
              
            case ExcelCFType.ColorScale:
              m_colorScale = new ColorScaleImpl();
              if (CF12Record.FormatType == ExcelCFType.ColorScale)
                  m_colorScale = (ColorScaleImpl)CF12Record.ColorScaleCF12.ColorScaleImpl;
              break;

            case ExcelCFType.DataBar:
              m_dataBar = new DataBarImpl();
              if (CF12Record.FormatType == ExcelCFType.DataBar)
                  m_dataBar = (DataBarImpl)CF12Record.DataBarCF12.DataBarImpl;;
              break;
              
            case ExcelCFType.IconSet:
              m_iconSet = new IconSetImpl();
              if (CF12Record.FormatType == ExcelCFType.IconSet)
                  m_iconSet = (IconSetImpl)CF12Record.IconSetCF12.IconsetImpl;
              break;
          }
        }
      }
    }
    /// <summary>
    /// Represents the type of time period.
    /// </summary>
    public CFTimePeriods TimePeriodType
    {
        get
        {
            return m_CFTimePeriod;
        }
        set
        {
            if (FormatType == ExcelCFType.TimePeriod)
                m_CFTimePeriod = value;
            FirstFormula = SetTimePeriodFormula(m_CFTimePeriod);
        }
    }
    /// <summary>
    /// Type of the comparison operator.
    /// </summary>
    public ExcelComparisonOperator  Operator
    {
      get
      {
        return m_formatRecord.ComparisonOperator;
      }
      set
      {
          if (value == ExcelComparisonOperator.BeginsWith || value == ExcelComparisonOperator.ContainsText ||
              value== ExcelComparisonOperator.NotContainsText || value== ExcelComparisonOperator.EndsWith)
              FormatType = ExcelCFType.SpecificText;

        m_formatRecord.ComparisonOperator = value;
      }
    }

    /// <summary>
    /// Indicates whether font is bold.
    /// </summary>
    public bool                     IsBold
    {
      get
      {
        return ( m_formatRecord.FontWeight >= FontImpl.FONTBOLD );
      }
      set
      {
        if( value )
        {
          m_formatRecord.FontWeight = FontImpl.FONTBOLD;
        }
        else
        {
          m_formatRecord.FontWeight = FontImpl.FONTNORMAL;
        }

        m_formatRecord.IsFontFormatPresent = true;
        m_formatRecord.IsFontStyleModified = true;
      }
    }
    /// <summary>
    /// Indicates whether font is italic.
    /// </summary>
    public bool                     IsItalic
    {
      get
      {
        return m_formatRecord.FontPosture;
      }
      set
      {
        m_formatRecord.FontPosture = value;
        m_formatRecord.IsFontFormatPresent = true;
        m_formatRecord.IsFontStyleModified = true;
      }
    }
    /// <summary>
    /// Font color.
    /// </summary>
    public ExcelKnownColors         FontColor
    {
      get
      {
        return ( ExcelKnownColors )m_fontColor.GetIndexed( m_book );
      }
      set
      {
        m_fontColor.SetIndexed( value );
        //m_formatRecord.FontColorIndex = ( uint ) value;
        //m_formatRecord.IsFontFormatPresent = true;
      }
    }
    /// <summary>
    /// Font color.
    /// </summary>
    public Color                    FontColorRGB
    {
      get
      {
        return m_fontColor.GetRGB( m_book );
      }
      set
      {
        m_fontColor.SetRGB( value );
      }
    }
    /// <summary>
    /// Underline type.
    /// </summary>
    public ExcelUnderline           Underline
    {
      get
      {
        return m_formatRecord.FontUnderline;
      }
      set
      {
        m_formatRecord.FontUnderline = value;
        m_formatRecord.IsFontFormatPresent = true;
        m_formatRecord.IsFontUnderlineModified = true;
      }
    }
    /// <summary>
    /// Indicates whether font is strike through.
    /// </summary>
    public bool                     IsStrikeThrough
    {
      get
      {
        return m_formatRecord.FontCancellation;
      }
      set
      {
        m_formatRecord.FontCancellation = value;
        m_formatRecord.IsFontFormatPresent = true;
        m_formatRecord.IsFontCancellationModified = true;
      }
    }
    /// <summary>
    /// Indicates whether font is superscript.
    /// </summary>
    public bool                     IsSuperScript
    {
      get
      {
        return ( m_formatRecord.FontEscapment == ExcelFontVertialAlignment.Superscript );
      }
      set
      {
        if( value )
        {
          m_formatRecord.FontEscapment = ExcelFontVertialAlignment.Superscript;
        }
        else if( IsSuperScript )
        {
          m_formatRecord.FontEscapment = ExcelFontVertialAlignment.Baseline;
        }

        m_formatRecord.IsFontEscapmentModified = true;
        m_formatRecord.IsFontFormatPresent = true;
      }
    }
    /// <summary>
    /// Indicates whether font is subscript.
    /// </summary>
    public bool                     IsSubScript
    {
      get
      {
        return ( m_formatRecord.FontEscapment == ExcelFontVertialAlignment.Subscript );
      }
      set
      {
        if( value )
        {
          m_formatRecord.FontEscapment = ExcelFontVertialAlignment.Subscript;
        }
        else if( IsSubScript )
        {
          m_formatRecord.FontEscapment = ExcelFontVertialAlignment.Baseline;
        }

        m_formatRecord.IsFontEscapmentModified = true;
        m_formatRecord.IsFontFormatPresent = true;
      }
    }
    /// <summary>
    /// Pattern foreground color.
    /// </summary>
    public ExcelKnownColors         Color
    {
      get
      {
        return m_color.GetIndexed( m_book );
      }
      set
      {
        m_color.SetIndexed( value );
      }
    }
    /// <summary>
    /// Pattern foreground color.
    /// </summary>
    public Color                    ColorRGB
    {
      get
      {
        return m_color.GetRGB( m_book );
      }
      set
      {
        m_color.SetRGB( value, m_book );
      }
    }
    /// <summary>
    /// Pattern background color.
    /// </summary>
    public ExcelKnownColors         BackColor
    {
      get
      {
        return m_backColor.GetIndexed( m_book );
      }
      set
      {
        m_backColor.SetIndexed( value );
      }
    }
    /// <summary>
    /// Pattern background color.
    /// </summary>
    public Color                    BackColorRGB
    {
      get
      {
        return m_backColor.GetRGB( m_book );
      }
      set
      {
        m_backColor.SetRGB( value, m_book );
      }
    }
    /// <summary>
    /// Fill pattern style.
    /// </summary>
    public ExcelPattern             FillPattern
    {
      get
      {
        return m_formatRecord.PatternStyle;
      }
      set
      {
        if( value != m_formatRecord.PatternStyle )
        {
          m_formatRecord.PatternStyle = value;
          m_formatRecord.IsPatternStyleModified = true;
          m_formatRecord.IsPatternFormatPresent = true;
        }
      }
    }

    /// <summary>
    /// True if contains font formatting.
    /// </summary>
    public bool                     IsFontFormatPresent
    {
      get
      {
        return m_formatRecord.IsFontFormatPresent;
      }
      set
      {
        m_formatRecord.IsFontFormatPresent = value;
      }
    }
    /// <summary>
    /// True if contains border formatting.
    /// </summary>
    public bool                     IsBorderFormatPresent
    {
      get
      {
        return m_formatRecord.IsBorderFormatPresent;
      }
      set
      {
        m_formatRecord.IsBorderFormatPresent = value;
      }
    }

    /// <summary>
    /// True if contains pattern formatting.
    /// </summary>
    public bool                     IsPatternFormatPresent
    {
      get
      {
        return m_formatRecord.IsPatternFormatPresent;
      }
      set
      {
        m_formatRecord.IsPatternFormatPresent = value;
      }
    }
    /// <summary>
    /// If true - format color present. otherwise - false.
    /// </summary>
    public bool                     IsFontColorPresent
    {
      get
      {
        return m_formatRecord.FontColorIndex != DEF_NOT_CONTAIN_FONT_COLOR;
      }
      set
      {
        if( value )
        {
          m_formatRecord.FontColorIndex = ( int )ExcelKnownColors.Black;
        }
        else
        {
          m_formatRecord.FontColorIndex = DEF_NOT_CONTAIN_FONT_COLOR;
        }
      }
    }
    /// <summary>
    /// If true - pattern color present, otherwise - false.
    /// </summary>
    public bool IsPatternColorPresent
    {
      get
      {
        return m_formatRecord.IsPatternColorModified;
      }
      set
      {
        m_formatRecord.IsPatternColorModified = value;

        if( value )
          m_formatRecord.IsPatternFormatPresent = value;
      }
    }
    /// <summary>
    /// If true - background color present. otherwise - false.
    /// </summary>
    public bool                     IsBackgroundColorPresent
    {
      get
      {
        return m_formatRecord.IsPatternBackColorModified;
      }
      set
      {
        m_formatRecord.IsPatternBackColorModified = value;
        
        if( value )
          m_formatRecord.IsPatternFormatPresent = value;
      }
    }
    /// <summary>
    /// If true - Number format is present. otherwise - false.
    /// </summary>
    public bool HasNumberFormatPresent
    {
        get
        {
            return m_formatRecord.IsNumberFormatModified;
        }
        set
        {
            m_formatRecord.IsNumberFormatModified = value;

            if (value)
                m_formatRecord.IsNumberFormatPresent = value;
        }
    }
    /// <summary>
    /// Gets or Sets whether the Conditional format has Extension list or not
    /// </summary>
    internal bool CFHasExtensionList
    {
        get
        {
            if (ST_GUID == null)
            {
                ST_GUID = "{" + Guid.NewGuid().ToString() + "}";
            }
            return m_cfHasExtensionList;
        }
        set
        {
            m_cfHasExtensionList = value;
 
            if (ST_GUID == null)
            {
                ST_GUID = "{" + Guid.NewGuid().ToString() + "}";
            }
        }
    }
    /// <summary>
    /// True if left border style and color are modified.
    /// </summary>
    public bool                     IsLeftBorderModified
    {
      get
      {
        return m_formatRecord.IsLeftBorderModified;
      }
      set
      {
        m_formatRecord.IsLeftBorderModified = value;

        if( value )
          m_formatRecord.IsBorderFormatPresent = value;
      }
    }

    /// <summary>
    /// True if right border style and color modified.
    /// </summary>
    public bool                     IsRightBorderModified
    {
      get
      {
        return m_formatRecord.IsRightBorderModified;
      }
      set
      {
        m_formatRecord.IsRightBorderModified = value;

        if( value )
          m_formatRecord.IsBorderFormatPresent = value;
      }
    }
    /// <summary>
    /// True if top border style and color are modified.
    /// </summary>
    public bool                     IsTopBorderModified
    {
      get
      {
        return m_formatRecord.IsTopBorderModified;
      }
      set
      {
        m_formatRecord.IsTopBorderModified = value;

        if( value )
          m_formatRecord.IsBorderFormatPresent = value;
      }
    }
    /// <summary>
    /// True if bottom border style and color are modified.
    /// </summary>
    public bool                     IsBottomBorderModified
    {
      get
      {
        return m_formatRecord.IsBottomBorderModified;
      }
      set
      {
        m_formatRecord.IsBottomBorderModified = value;

        if( value )
          m_formatRecord.IsBorderFormatPresent = value;
      }
    }
    /// <summary>
    /// Number format index.
    /// </summary>
    public ushort NumberFormatIndex
    {
        get
        {
            return m_formatRecord.NumberFormatIndex;
        }
        set
        {
            if (!m_book.InnerFormats.Contains(value))
            {
                throw new ArgumentOutOfRangeException("Unknown format index");
            }

            HasNumberFormatPresent = true;
            m_formatRecord.NumberFormatIndex = (ushort)value;            
        }
    }
    /// <summary>
    /// Returns or sets the format code for the object. Read/write String.
    /// </summary>
    public string NumberFormat
    {
        get
        {
            return ((FormatImpl)m_book.InnerFormats[NumberFormatIndex]).FormatString;
        }
        set
        {
            NumberFormatIndex = (ushort)m_book.InnerFormats.FindOrCreateFormat(value);
        }
    }
    /// <summary>
    /// The text value in a Specific Text conditional formatting rule. 
    /// Valid only for type =contains Text, notContainsText,beginsWith,endsWith. The default value is null.
    /// </summary>
    public string Text
    {
        get 
        {
            return m_text;
        }
        set
        {
            if (value == null || value==string.Empty)
            {
                throw new ArgumentNullException("Argument cannot be null or empty.");
            }
            m_text = value;
            if (FormatType == ExcelCFType.SpecificText)
            {
                SetSpecificTextString(Operator, value);
            }
        }
    }
    /// <summary>
    /// TODO:The issue with IRange for Conditional cell value type while range having the "Astersik " (*) symbol. the breaking issue id:DefectID_SD15510.xlsx,
    /// The breaks occurs in the FR implementation "SF7880-Support for specific Text conditional formatting" .
    /// Remove this property while fixing the break issue of CF range with Asterisk.
    /// </summary>
    internal string AsteriskRange
    {
        get
        {
            return m_asteriskRange;
        }
        set
        {
            m_asteriskRange = value;
        }
    }
    ///<summary>
    /// Lower priority conditional formatting rules are evaluated.
    ///</summary>
    public bool StopIfTrue
    {
        get
        {
            return m_cfExRecord.StopIfTrue;
        }
        set
        {
            m_cfExRecord.StopIfTrue = value;
        }
    }
    ///<summary>
    /// Represents the priority of the CF.
    ///</summary>
    internal int Priority
    {
        get
        {
            return m_cfExRecord.Priority;
        }
        set
        {
            m_cfExRecord.Priority =(ushort) value;
        }
    }
    ///<summary>
    /// Conditionl format template.
    ///</summary>
    public ConditionalFormatTemplate Template
    {
        get
        {
            return m_cf12Record.Template;
        }
        set
        {
            m_cf12Record.Template = value;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public IDataBar DataBar
    {
      get
      {
        return m_dataBar;
      }
    }
    /// <summary>
    /// Returns iconset settings. Valid only if FormatType is set to IconSet. Read-only.
    /// </summary>
    public IIconSet IconSet
    {
      get
      {
        return m_iconSet;
      }
    }
    /// <summary>
    /// Returns color scale object. Read-only.
    /// </summary>
    public IColorScale ColorScale
    {
      get
      {
        return m_colorScale;
      }
    }
    #endregion

    #region Class Properties
    /// <summary>
    /// Returns internal CFRecord. Read-only.
    /// </summary>
    [ CLSCompliant( false ) ]
    public CFRecord Record
    {
      get
      {
        return m_formatRecord;
      }
    }
    /// <summary>
    /// Returns internal CF12Record. Read-only.
    /// </summary>
    [CLSCompliant(false)]
    public CF12Record CF12Record
    {
        get
        {
            return m_cf12Record;
        }
    }
    /// <summary>
    /// Returns internal CFExRecord. Read-only.
    /// </summary>
    [CLSCompliant(false)]
    public CFExRecord CFExRecord
    {
        get
        {
            return m_cfExRecord;
        }
    }
    /// <summary>
    /// <summary>
    /// Returns parent workbook. Read-only.
    /// </summary>
    public WorkbookImpl Workbook
    {
      get
      {
        return m_book;
      }
    }
    /// <summary>
    /// Returns data bar object. Read-only.
    /// </summary>
    internal DataBarImpl InnerDataBar
    {
      get
      {
        return m_dataBar;
      }
    }
    /// <summary>
    /// Represents the range refernce of Conditional formatting
    /// </summary>
    internal string RangeRefernce
    {
        get 
        {
            return m_rangeReference;
        }
        set 
        {
            m_rangeReference = value;
        }
    }
    internal IRange Range
    {
        get
        {
            return m_range;
        }
        set
        {
            m_range = value;
        }
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Set the text value for the Specfic Text formula
    /// </summary>
    /// <param name="compOperator">value for Excel comparision operator</param>
    /// <param name="value">string value </param>
    public void SetSpecificTextString(ExcelComparisonOperator compOperator, string value)
    {
        ConditionalFormats parentFormats = Parent as ConditionalFormats;
        m_cfExRecord.Template = ConditionalFormatTemplate.ContainsText;

        this.AppImplementation.IsFormulaParsed = false;
        CultureInfo currentCulture = this.AppImplementation.CheckAndApplySeperators();
        this.m_book.SetSeparators(Convert.ToChar(currentCulture.TextInfo.ListSeparator), this.AppImplementation.RowSeparator);

        switch (compOperator)
        {
            case ExcelComparisonOperator.BeginsWith:            
                FirstFormula = string.Format(DefaultBeginsWithFormula, parentFormats.Address, "\"" + value.ToString() + "\"");                
                m_cfExRecord.m_cfExTextParam.TextRuleType = CFTextRuleType.TextBeginsWith;
                break;
            case ExcelComparisonOperator.EndsWith:                
                FirstFormula = string.Format(DefaultEndsWithFormula, parentFormats.Address, "\"" + value.ToString() + "\"");
                m_cfExRecord.m_cfExTextParam.TextRuleType = CFTextRuleType.TextEndsWith;
                break;
            case ExcelComparisonOperator.ContainsText:
                FirstFormula = string.Format(DefaultContainsTextFormula, "\"" + value.ToString() + "\"", parentFormats.Address);
                m_cfExRecord.m_cfExTextParam.TextRuleType = CFTextRuleType.TextContains;
                break;
            case ExcelComparisonOperator.NotContainsText:
                FirstFormula = string.Format(DefaultNotContainsTextFormula, "\"" + value.ToString() + "\"", parentFormats.Address);               
                m_cfExRecord.m_cfExTextParam.TextRuleType = CFTextRuleType.TextNotContains;
                break;
            default:
                throw new ArgumentException("Invalid Opertor:", "compOperator");
        }

        this.AppImplementation.IsFormulaParsed = true;
        currentCulture = this.AppImplementation.CheckAndApplySeperators();
        this.m_book.SetSeparators(Convert.ToChar(currentCulture.TextInfo.ListSeparator), this.AppImplementation.RowSeparator);
    }

    /// <summary>
    /// Set the range value for the Specfic Text formula
    /// </summary>
    /// <param name="compOperator">value for Excel comparision operator </param>
    /// <param name="range"> Range value</param>
    public string SetSpecificTextFormula(ExcelComparisonOperator compOperator,RangeImpl range)
    {
        ConditionalFormats parentFormats = Parent as ConditionalFormats;
        string str_formula = string.Empty;

        this.AppImplementation.IsFormulaParsed = false;
        CultureInfo currentCulture = this.AppImplementation.CheckAndApplySeperators();
        this.m_book.SetSeparators(Convert.ToChar(currentCulture.TextInfo.ListSeparator), this.AppImplementation.RowSeparator);

        switch (compOperator)
        {
            case ExcelComparisonOperator.BeginsWith:               
                str_formula = string.Format(DefaultBeginsWithFormula, parentFormats.Address, "\"" + range.AddressGlobalWithoutSheetName.ToString() + "\"");
                CFHasExtensionList = true;
                break;
            case ExcelComparisonOperator.EndsWith:                
                str_formula = string.Format(DefaultEndsWithFormula, parentFormats.Address, "\"" + range.AddressGlobalWithoutSheetName.ToString() + "\"");
                CFHasExtensionList = true;
                break;
            case ExcelComparisonOperator.ContainsText:
                str_formula = string.Format(DefaultContainsTextFormula, "\"" + range.AddressGlobalWithoutSheetName.ToString() + "\"", parentFormats.Address);
                CFHasExtensionList = true;
                break;
            case ExcelComparisonOperator.NotContainsText:
                str_formula = string.Format(DefaultNotContainsTextFormula, "\"" + range.AddressGlobalWithoutSheetName.ToString() + "\"", parentFormats.Address);
                CFHasExtensionList = true;
                break;
            default:
                throw new ArgumentException("Invalid Opertor:", "compOperator");
        }

        this.AppImplementation.IsFormulaParsed = true;
        currentCulture = this.AppImplementation.CheckAndApplySeperators();
        this.m_book.SetSeparators(Convert.ToChar(currentCulture.TextInfo.ListSeparator), this.AppImplementation.RowSeparator);

        return str_formula;
    }

    /// <summary>
    /// Sets the formula for time period types.
    /// </summary>
    /// <param name="cfTimePeriods"></param>
    /// <returns></returns>
    private string SetTimePeriodFormula(CFTimePeriods cfTimePeriods)
    {
        string formula = string.Empty;
        ConditionalFormats parentFormats = Parent as ConditionalFormats;
        switch (cfTimePeriods)
        {
            case CFTimePeriods.Today:
                formula = string.Format(DefaultTodayTimePeriodFormula, parentFormats.Address);
                m_cfExRecord.m_cfExDateParam.DateComparisonOperator = (ushort)ConditionalFormatTemplate.Today;
                m_cfExRecord.Template = ConditionalFormatTemplate.Today;
                break;
            case CFTimePeriods.Tomorrow:
                formula = string.Format(DefaultTomorrowTimePeriodFormula, parentFormats.Address);
                m_cfExRecord.m_cfExDateParam.DateComparisonOperator = (ushort)ConditionalFormatTemplate.Tomorrow;
                m_cfExRecord.Template = ConditionalFormatTemplate.Tomorrow;
                break;
            case CFTimePeriods.Yesterday:
                formula = string.Format(DefaultYesterdayTimePeriodFormula,parentFormats.Address);
                m_cfExRecord.m_cfExDateParam.DateComparisonOperator = (ushort)ConditionalFormatTemplate.Yesterday;
                m_cfExRecord.Template = ConditionalFormatTemplate.Yesterday;
                break;
            case CFTimePeriods.Last7Days:
                formula = string.Format(DefaultLastSevenDaysTimePeriodFormula, parentFormats.Address);
                m_cfExRecord.m_cfExDateParam.DateComparisonOperator = (ushort)ConditionalFormatTemplate.Last7Days;
                m_cfExRecord.Template = ConditionalFormatTemplate.Last7Days;
                break;
            case CFTimePeriods.LastWeek:
                formula = string.Format(DefaultLastWeekTimePeriodFormula,parentFormats.Address);
                m_cfExRecord.m_cfExDateParam.DateComparisonOperator = (ushort)ConditionalFormatTemplate.LastWeek;
                m_cfExRecord.Template = ConditionalFormatTemplate.LastWeek;
                break;
            case CFTimePeriods.ThisWeek:
                formula = string.Format(DefaultThisWeekTimePeriodFormula,parentFormats.Address);
                m_cfExRecord.m_cfExDateParam.DateComparisonOperator = (ushort)ConditionalFormatTemplate.ThisWeek;
                m_cfExRecord.Template = ConditionalFormatTemplate.ThisWeek;
                break;
            case CFTimePeriods.NextWeek:
                formula = string.Format(DefaultNextWeekTimePeriodFormula, parentFormats.Address);
                m_cfExRecord.m_cfExDateParam.DateComparisonOperator = (ushort)ConditionalFormatTemplate.NextWeek;
                m_cfExRecord.Template = ConditionalFormatTemplate.NextWeek;
                break;
            case CFTimePeriods.LastMonth:
                formula = string.Format(DefaultLastMonthTimePeriodFormula,parentFormats.Address);
                m_cfExRecord.m_cfExDateParam.DateComparisonOperator = (ushort)ConditionalFormatTemplate.LastMonth;
                m_cfExRecord.Template = ConditionalFormatTemplate.LastMonth;
                break;
            case CFTimePeriods.ThisMonth:
                formula = string.Format(DefaultThisMonthTimePeriodFormula, parentFormats.Address);
                m_cfExRecord.m_cfExDateParam.DateComparisonOperator = (ushort)ConditionalFormatTemplate.ThisMonth;
                m_cfExRecord.Template = ConditionalFormatTemplate.ThisMonth;
                break;
            case CFTimePeriods.NextMonth:
                formula = string.Format(DefaultNextMonthTimePeriodFormula, parentFormats.Address);
                m_cfExRecord.m_cfExDateParam.DateComparisonOperator = (ushort)ConditionalFormatTemplate.NextMonth;
                m_cfExRecord.Template = ConditionalFormatTemplate.NextMonth;
                break;
            default:
                throw new ArgumentException("Invalid time period type:", "cfTimePeriods");
        }
        return formula;
    }
    /// <summary>
    /// Sets first or second formula value.
    /// </summary>
    /// <param name="formulaUtil">Formula util.</param>
    /// <param name="strFormula">Formula string.</param>
    /// <param name="bIsFirstFormula">Defines first or second formula.</param>
    public void SetFirstSecondFormula( FormulaUtil formulaUtil, string strFormula, bool bIsFirstFormula )
    {
      Ptg[] ptgs = formulaUtil.ParseString( strFormula );

      if( bIsFirstFormula )
      {
        m_formatRecord.FirstFormulaPtgs = ptgs;
      }
      else
      {
        m_formatRecord.SecondFormulaPtgs = ptgs;
      }

      if (m_book.ThrowOnUnknownNames && FormatType==ExcelCFType.Formula)
          m_customFunction = strFormula;
    }
    /// <summary>
    /// Returns first or second formula string value.
    /// </summary>
    /// <param name="formulaUtil">Formula util.</param>
    /// <param name="bIsFirstFormula">Defines first or second formula.</param>
    /// <returns>First/second formula string value.</returns>
    public string GetFirstSecondFormula( FormulaUtil formulaUtil, bool bIsFirstFormula )
    { 
      Ptg[] arrPtg = ( bIsFirstFormula ) ? m_formatRecord.FirstFormulaPtgs : m_formatRecord.SecondFormulaPtgs;

      if( arrPtg == null )
        return null;

      ConditionalFormats parentFormats = Parent as ConditionalFormats;
      Rectangle cell = Range !=null ?
                       GetCellRectangle(parentFormats)
                       : parentFormats.CellRectangles[0];
        

      return formulaUtil.ParsePtgArray(arrPtg, cell.Top , cell.Left,
        m_book.CalculationOptions.R1C1ReferenceMode, false );
    }
    /// <summary>
    /// Gets the cell rectangle.
    /// </summary>
    /// <param name="parentFormats">The parent formats.</param>
    /// <returns></returns>
    private Rectangle GetCellRectangle(ConditionalFormats parentFormats)
    {
        string MinimumRange;        
        if (parentFormats != null && parentFormats.CellsList.Length > 0)
        {
            MinimumRange = GetMinimumRange(parentFormats.CellsList);
            for (int index = 0; index < parentFormats.CellsList.Length; index++)
            {                
                IRange cellRange = this.Workbook.ActiveSheet[parentFormats.CellsList[index]];                
                if (Range.Row >= cellRange.Row && Range.Column >= cellRange.Column || Range.LastRow <= cellRange.LastRow && Range.LastColumn <= cellRange.LastColumn)
                    return new Rectangle(this.Workbook.ActiveSheet[MinimumRange].Column, this.Workbook.ActiveSheet[MinimumRange].Row, 0, 0);
            }
        }
        return new Rectangle(0, 0, 0, 0);
    }

    /// <summary>
    /// Gets the minimum range.
    /// </summary>
    /// <param name="sortedList">The sorted list.</param>
    /// <returns></returns>
    private string GetMinimumRange(string[] sortedList)
    {
        List<string> splitList = new List<string>();
        int index;
        foreach (string str in sortedList)
        {
            if (str.Contains(":"))
                splitList.AddRange(str.Split(':'));
            else
                splitList.Add(str);
        }

        for (int rangeIndex = 1; rangeIndex < splitList.Count; rangeIndex++)
        {
            string destindex = splitList[rangeIndex];
            IRange firstRange = this.Workbook.ActiveSheet[splitList[rangeIndex]];
            index = rangeIndex;
            while ((index > 0) && (this.Workbook.ActiveSheet[splitList[index - 1]].Row <= firstRange.Row && this.Workbook.ActiveSheet[splitList[index - 1]].Column <= firstRange.Column))
            {
                splitList[index] = splitList[index - 1];
                index = index - 1;
            }
            splitList[index] = destindex;
        }
        return splitList[splitList.Count - 1];
    }
    /// <summary>
    /// This method should be called before several updates to the object will take place.
    /// </summary>
    public void BeginUpdate()
    {
      throw new NotImplementedException();
    }
    /// <summary>
    /// This method should be called after several updates to the object took place.
    /// </summary>
    public void EndUpdate()
    {
      throw new NotImplementedException();
    }
    /// <summary>
    /// Updates color indexes.
    /// </summary>
    private void UpdateColors()
    {
      m_formatRecord.PatternColorIndex = ( ushort )m_color.GetIndexed( m_book );
      m_formatRecord.PatternBackColor = ( ushort )m_backColor.GetIndexed( m_book );
      m_formatRecord.TopBorderColorIndex = ( ushort )m_topBorderColor.GetIndexed( m_book );
      m_formatRecord.BottomBorderColorIndex = ( ushort )m_bottomBorderColor.GetIndexed( m_book );
      m_formatRecord.LeftBorderColorIndex = ( ushort )m_leftBorderColor.GetIndexed( m_book );
      m_formatRecord.RightBorderColorIndex = ( ushort )m_rightBorderColor.GetIndexed( m_book );
    }
    /// <summary>
    /// Updates conditional format formulas.
    /// </summary>
    /// <param name="iCurIndex">Current worksheet index.</param>
    /// <param name="iSourceIndex">Source worksheet index.</param>
    /// <param name="sourceRect">Source rectangle.</param>
    /// <param name="iDestIndex">Destination worksheet index.</param>
    /// <param name="destRect">Destination rectangle.</param>
    public void UpdateFormula( int iCurIndex, int iSourceIndex, Rectangle sourceRect,
      int iDestIndex, Rectangle destRect, int row, int column )
    {
      Ptg[] formula = m_formatRecord.FirstFormulaPtgs;

      if( formula != null && formula.Length > 0 )
      {
        formula = m_book.FormulaUtil.UpdateFormula( formula, iCurIndex, iSourceIndex,
          sourceRect, iDestIndex, destRect, row, column );

        m_formatRecord.FirstFormulaPtgs = formula;
      }

      formula = m_formatRecord.SecondFormulaPtgs;

      if( formula != null && formula.Length > 0 )
      {
        formula = m_book.FormulaUtil.UpdateFormula( formula, iCurIndex, iSourceIndex,
          sourceRect, iDestIndex, destRect, row, column );

        m_formatRecord.SecondFormulaPtgs = formula;
      }
    }
    #endregion

    #region Class overrides
    /// <summary>
    /// Serves as a hash function for a particular type, suitable for use in
    /// hashing algorithms and data structures like a hash table.
    /// </summary>
    /// <returns>A hash code for the current Object.</returns>
    public override int GetHashCode()
    {
      return m_formatRecord.GetHashCode() ^ m_cf12Record.GetHashCode() ^ m_cfExRecord.GetHashCode();
    }
    /// <summary>
    /// A hash code for the current Object without taking cell list into account.
    /// </summary>
    /// <param name="obj">The Object to compare with the current Object.</param>
    /// <returns></returns>
    public override bool Equals( object obj )
    {
      ConditionalFormatImpl toCompare = obj as ConditionalFormatImpl;

      if( toCompare == null ) return false;

      return m_formatRecord.Equals( toCompare.m_formatRecord ) &&
        m_cfExRecord.Equals(toCompare.CFExRecord) &&
        m_cf12Record.Equals(toCompare.m_cf12Record) &&
        m_color == toCompare.m_color &&
        m_backColor == toCompare.m_backColor &&
        m_topBorderColor == toCompare.m_topBorderColor &&
        m_bottomBorderColor == toCompare.m_bottomBorderColor &&
        m_leftBorderColor == toCompare.m_leftBorderColor &&
        m_rightBorderColor == toCompare.m_rightBorderColor &&
        m_fontColor == toCompare.m_fontColor &&
        m_dataBar == toCompare.m_dataBar &&
        m_iconSet == toCompare.m_iconSet &&
        m_colorScale == toCompare.m_colorScale;
    }
    #endregion

    #region ICloneParent Members
    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <param name="parent">Parent object for a copy of this instance.</param>
    /// <returns>A new object that is a copy of this instance.</returns>
    public object Clone( object parent )
    {
      ConditionalFormatImpl result = ( ConditionalFormatImpl )MemberwiseClone();
      result.SetParent( parent );
      result.SetParents();
      result.m_formatRecord = ( CFRecord )CloneUtils.CloneCloneable( m_formatRecord );

      result.InitializeColors();
      result.m_color.CopyFrom( m_color, false );
      result.m_backColor.CopyFrom( m_backColor, false );
      result.m_fontColor.CopyFrom( m_fontColor, false );
      result.m_leftBorderColor.CopyFrom( m_leftBorderColor, false );
      result.m_rightBorderColor.CopyFrom( m_rightBorderColor, false );
      result.m_topBorderColor.CopyFrom( m_topBorderColor, false );
      result.m_bottomBorderColor.CopyFrom( m_bottomBorderColor, false );

      result.m_formatRecord = ( CFRecord )CloneUtils.CloneCloneable( m_formatRecord );
      result.m_cf12Record = (CF12Record)CloneUtils.CloneCloneable(m_cf12Record);
      result.m_cfExRecord = (CFExRecord)CloneUtils.CloneCloneable(m_cfExRecord);
      result.m_dataBar = ( DataBarImpl )CloneUtils.CloneCloneable( m_dataBar );
      result.m_iconSet = ( IconSetImpl )CloneUtils.CloneCloneable( m_iconSet );

      return result;
    }

    #endregion

    #region IInternalConditionalFormat Members
    /// <summary>
    /// Conditional format color. Read-only.
    /// </summary>
    public ColorObject ColorObject
    {
      get
      {
        return m_color;
      }
    }
    /// <summary>
    /// Conditional format background color. Read-only.
    /// </summary>
    public ColorObject BackColorObject
    {
      get
      {
        return m_backColor;
      }
    }
    /// <summary>
    /// Conditional format top border color. Read-only.
    /// </summary>
    public ColorObject TopBorderColorObject
    {
      get
      {
        return m_topBorderColor;
      }
    }
    /// <summary>
    /// Conditional format bottom border color. Read-only.
    /// </summary>
    public ColorObject BottomBorderColorObject
    {
      get
      {
        return m_bottomBorderColor;
      }
    }
    /// <summary>
    /// Conditional format left border color. Read-only.
    /// </summary>
    public ColorObject LeftBorderColorObject
    {
      get
      {
        return m_leftBorderColor;
      }
    }
    /// <summary>
    /// Conditional format right border color. Read-only.
    /// </summary>
    public ColorObject RightBorderColorObject
    {
      get
      {
        return m_rightBorderColor;
      }
    }
    /// <summary>
    /// Conditional format font color. Read-only.
    /// </summary>
    public ColorObject FontColorObject
    {
      get
      {
        return m_fontColor;
      }
    }
    /// <summary>
    /// Indicates whether pattern style was modified.
    /// </summary>
    public bool IsPatternStyleModified
    {
      get
      {
        return m_formatRecord.IsPatternStyleModified;
      }
      set
      {
        m_formatRecord.IsPatternStyleModified = value;
      }
    }
    /// <summary>
    /// Returns parsed tokens of the first formula.
    /// </summary>
    Ptg[] IInternalConditionalFormat.FirstFormulaPtgs
    {
      get
      {
        return m_formatRecord.FirstFormulaPtgs;
      }
    }
    /// <summary>
    /// Returns parsed tokens of the second formula.
    /// </summary>
    Ptg[] IInternalConditionalFormat.SecondFormulaPtgs
    {
      get
      {
        return m_formatRecord.SecondFormulaPtgs;
      }
    }
    #endregion

    internal void ClearAll()
    {
        if(m_fontColor!=null)
            m_fontColor.Dispose();
        if(m_color!=null)
            m_color.Dispose();
        if(m_cf12Record!=null)
            m_cf12Record.ClearAll();
        m_cfExRecord.ClearAll();
        m_backColor.Dispose();
        m_fontColor.Dispose();
        m_book = null;
    }
  }
}
