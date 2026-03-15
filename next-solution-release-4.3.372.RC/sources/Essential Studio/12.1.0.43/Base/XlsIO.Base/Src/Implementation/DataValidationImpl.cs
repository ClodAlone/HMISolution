#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using Syncfusion.XlsIO.Implementation.Collections;
using Syncfusion.XlsIO.Implementation.Exceptions;
using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser.Biff_Records.Formula;
using System.Runtime.CompilerServices;

#if ( WINRT )
using Windows.UI;
using Rectangle=Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
using Syncfusion.XlsIO.Implementation;
#endif

#if  (SILVERLIGHT)
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.Silverlight.Rectangle;
#elif WP
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.WP.Rectangle;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;
#endif

namespace Syncfusion.XlsIO.Implementation
{
  /// <summary>
  /// Contains a condition and the formatting attributes 
  /// applied to the cells, if the condition is met.
  /// Used for single-cell range.
  /// </summary>
  public class DataValidationImpl :
    IInternalDataValidation,
    IReparse,
    ICloneParent
  {
    #region Class constants
    /// <summary>
    /// 
    /// </summary>
    private readonly Type[] DATARANGETYPES = new Type[] 
    {
      typeof( Ref3DPtg ), typeof( RefPtg ), 
      typeof( AreaPtg ), typeof( Area3DPtg )
    };
    /// <summary>
    /// Maximum possible length of the string.
    /// </summary>
    private const int DEF_MAX_LIST_LENGTH = 256;
    private const int TitleLimit = 32;
    private const int TextLimit = 225;
    private const int InputTextLimit = 255;
    #endregion

    #region Class members
    /// <summary>
    /// 
    /// </summary>
    private DVRecord m_dvRecord;
    ///// <summary>
    ///// First formula string.
    ///// </summary>
    private string m_strFirstFormula = string.Empty;
    ///// <summary>
    ///// Second formula string.
    ///// </summary>
    private string m_strSecondFormula = string.Empty;
    /// <summary>
    /// 
    /// </summary>
    private DataValidationCollection m_DVCollection;
    /// <summary>
    /// 
    /// </summary>
    private RangesOperations m_cells = new RangesOperations();
    private FormulaUtil m_formulaUtil;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Creates DataValidation and set its Application and Parent
    /// properties to specified values.
    /// </summary>
    /// <param name="parent">Parent object for the DataValidation.</param>
    public DataValidationImpl( DataValidationCollection parent )
    {
      if( parent == null )
        throw new ArgumentNullException( "parent" );

      m_dvRecord = ( DVRecord )BiffRecordFactory.GetRecord( TBIFFRecord.DV );
      m_DVCollection = parent;
    }
    /// <summary>
    /// Creates DataValidation from an array of BiffRecords.
    /// </summary>
    /// <param name="parent">Parent object for the DataValidation.</param>
    /// <param name="dv">Base DVRecord.</param>
    [CLSCompliant( false )]
    public DataValidationImpl( DataValidationCollection parent
      , DVRecord dv )
      : this( parent )
    {
      m_dvRecord = ( DVRecord )dv.Clone();

      FillCells( m_dvRecord );

      try
      {
        Reparse();
      }
      catch( ParseException )
      {
        WorkbookImpl book = Workbook;

        if( book == null )
          throw new ArgumentNullException( "Can't find parent workbook" );

        if( book.Loading )
        {
          book.AddForReparse( this );
        }
        else
        {
          throw;
        }
      }
    }
    #endregion

    #region Class Helper Methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dv"></param>
    public void AddRange( DataValidationImpl dv )
    {
      List<Rectangle> lstCells = dv.m_cells.CellList;
      m_cells.AddCells( lstCells );
    }
    /// <summary>
    /// Adds range to the collection.
    /// </summary>
    /// <param name="range">Range to add.</param>
    public void AddRange( RangeImpl range )
    {
      m_cells.AddRange( range );
    }
    /// <summary>
    /// Adds range to the collection.
    /// </summary>
    /// <param name="tAddr">Cell range address.</param>
    [CLSCompliant( false )]
    public void AddRange( TAddr tAddr )
    {
      m_cells.AddRange( tAddr.GetRectangle() );
    }
    /// <summary>
    /// Adds range to the collection.
    /// </summary>
    /// <param name="range">Cell range to add.</param>
    [CLSCompliant( false )]
    public void AddRange( ICombinedRange range )
    {
      m_cells.AddRectangles( range.GetRectangles() );
    }
    /// <summary>
    /// Removes range from collection.
    /// </summary>
    /// <param name="range">Range to remove.</param>
    public void RemoveRange( RangeImpl range )
    {
      RemoveRange( range.GetRectangles() );
    }
    /// <summary>
    /// Removes ranges from collection.
    /// </summary>
    /// <param name="rectangles">Ranges to remove.</param>
    public void RemoveRange( Rectangle[] rectangles )
    {
      m_cells.Remove( rectangles );

      if( m_cells.CellList.Count == 0 )
      {
        m_DVCollection.Remove( this );
      }
    }
    /// <summary>
    /// 
    /// </summary>
    private void FillFromCells()
    {
      m_cells.OptimizeStorage();
      m_dvRecord.AddrList = new TAddr[] { };

      List<Rectangle> lstCells = m_cells.CellList;
      for( int i = 0, len = lstCells.Count; i < len; i++ )
      {
        Rectangle rect = lstCells[ i ];
        m_dvRecord.Add( new TAddr( rect ) );
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dv"></param>
    private void FillCells( DVRecord dv )
    {
      TAddr[] arrAddrList = dv.AddrList;

      for( int i = 0, len = arrAddrList.Length; i < len; i++ )
      {
        TAddr cur = arrAddrList[ i ];
        m_cells.AddRange( cur.GetRectangle() );
      }

    }
    /// <summary>
    /// Converts DateTime value into tokens array.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <returns>Converted array.</returns>
    private Ptg[] ConvertFromDateTime( DateTime value )
    {
      double dValue = value.ToOADate();
      DoublePtg ptg = ( DoublePtg )FormulaUtil.CreatePtg( FormulaToken.tNumber, dValue );
      ptg.Value = dValue;

      return new Ptg[] { ptg };
    }
    /// <summary>
    /// Tries to convert array of tokens into DateTime value.
    /// </summary>
    /// <param name="arrPtgs">Tokens to convert.</param>
    /// <returns>Converted value or DateTime.MinValue if conversion wasn't successful.</returns>
    private DateTime ConvertToDateTime( Ptg[] arrPtgs )
    {
      DateTime result = DateTime.MinValue;

      if( arrPtgs != null )
      {
        int iLength = arrPtgs.Length;

        if( iLength == 1 )
        {
          Ptg ptg = arrPtgs[ 0 ];

          if( ptg is IntegerPtg )
          {
            int iValue = ( ( IntegerPtg )ptg ).Value;
            result =
#if ( WINRT )
 DateTimeExtension.FromOADate(iValue);
#else
          DateTime.FromOADate( iValue );
#endif
          }
          else if( ptg is DoublePtg )
          {
            double dValue = ( ( DoublePtg )ptg ).Value;
            result =
#if ( WINRT )
 DateTimeExtension.FromOADate(dValue);
#else
          DateTime.FromOADate( dValue );
#endif
          }
        }
      }

      return result;
    }
    /// <summary>
    /// Converts formula string to Ptg array.
    /// </summary>
    /// <param name="value">Represents formula value.</param>
    /// <param name="formulaUtil">Object used for formula parsing.</param>
    /// <returns>Returns ptg array.</returns>
    public static Ptg[] GetFormulaPtg( ref string value, FormulaUtil formulaUtil, WorksheetImpl sheet,int row, int column )
    {
      if( value == null )
        throw new ArgumentNullException( "value" );

      if( value.Length == 0 )
        throw new ArgumentOutOfRangeException( "Value cannot be empty." );

      string strFormula = value;

      if( strFormula[ 0 ] == '=' )
      {
        strFormula = UtilityMethods.RemoveFirstCharUnsafe( strFormula );
      }

      double result;
      Ptg[] parsedFormula;

      NumberFormatInfo numberFormat = null;

      if( formulaUtil != null )
      {
        numberFormat = formulaUtil.NumberFormat;
      }

      if( double.TryParse( strFormula, System.Globalization.NumberStyles.Any, numberFormat, out result ) )
      {
        //parses as number.
        DoublePtg ptgRes = ( DoublePtg )FormulaUtil.CreatePtg( FormulaToken.tNumber );
        ptgRes.Value = result;
        parsedFormula = new Ptg[] { ptgRes };

        if( ptgRes.Value == 0 )
        {
          strFormula = "0";
        }
      }
      else
      {
        // parses as formula.
          parsedFormula = ParseFormula(strFormula, sheet, formulaUtil, row, column);
      }

      return parsedFormula;
    }
    /// <summary>
    /// Parses the string formula.
    /// </summary>
    /// <param name="value">Represents formula value.</param>
    /// <param name="formulaUtil">Object used for formula parsing.</param>
    /// <returns>Returns ptg array.</returns>
    public static Ptg[] ParseFormula(string strFormula, WorksheetImpl sheet, FormulaUtil formulaUtil, int row, int column)
    {
        Ptg[] parsedFormula;
        Dictionary<Type, ReferenceIndexAttribute> indexes = new Dictionary<Type, ReferenceIndexAttribute>();
        indexes.Add(typeof(AreaPtg), new ReferenceIndexAttribute(1));
        indexes.Add(typeof(RefNPtg), new ReferenceIndexAttribute(1));
        indexes.Add(typeof(RefPtg), new ReferenceIndexAttribute(1));
        indexes.Add(typeof(Area3DPtg), new ReferenceIndexAttribute(1));
        indexes.Add(typeof(Ref3DPtg), new ReferenceIndexAttribute(1));
        indexes.Add(typeof(NamePtg), new ReferenceIndexAttribute(1));

        WorkbookImpl book = sheet.ParentWorkbook;

        RegisterFunctions(true);
        if (formulaUtil == null)
        {
            parsedFormula = book.FormulaUtil.ParseString(strFormula, null, indexes,
              0, null, ExcelParseFormulaOptions.None, row, column);
        }
        else
        {
            parsedFormula = formulaUtil.ParseString(strFormula, sheet,
              indexes, 0, null, ExcelParseFormulaOptions.None, row, column);
        }
        RegisterFunctions(false);
        if (IsZeroValuePtg(parsedFormula))
        {
            parsedFormula = new Ptg[] { parsedFormula[0] };
            strFormula = "0";
        }
        return parsedFormula;
    }
      /// <summary>
      /// Registers and unregister the fuction with RefNPtg formula type.
      /// </summary>
      /// <param name="isRefNPtg">indicates whether the function is RefNPtg supported.</param>
#if !(WINRT )
    [MethodImpl(MethodImplOptions.Synchronized)]
#endif
    public static void RegisterFunctions(bool isRefNPtg)
    {
        //TODO: Add the RefNPtg supported function.
        if (isRefNPtg)
        {
            ReferenceIndexAttribute[] attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefNPtg ), 2 )
		  };
            FormulaUtil.EditRegisteredFunction("ISNUMBER", ExcelFunction.ISNUMBER, attributes, 1);
            FormulaUtil.EditRegisteredFunction("ISBLANK", ExcelFunction.ISBLANK, attributes, 1);
            FormulaUtil.EditRegisteredFunction("ISERR", ExcelFunction.ISERR, attributes, 1);
            FormulaUtil.EditRegisteredFunction("ISEVEN", ExcelFunction.ISEVEN, attributes, 1);
            FormulaUtil.EditRegisteredFunction("ISLOGICAL", ExcelFunction.ISLOGICAL, attributes, 1);
            FormulaUtil.EditRegisteredFunction("ISNA", ExcelFunction.ISNA, attributes, 1);
            FormulaUtil.EditRegisteredFunction("ISNONTEXT", ExcelFunction.ISNONTEXT, attributes, 1);
            FormulaUtil.EditRegisteredFunction("ISODD", ExcelFunction.ISODD, attributes, 1);
            FormulaUtil.EditRegisteredFunction("ISPMT", ExcelFunction.ISPMT, attributes, 1);
            FormulaUtil.EditRegisteredFunction("ISREF", ExcelFunction.ISREF, attributes, 1);
            FormulaUtil.EditRegisteredFunction("ISTEXT", ExcelFunction.ISTEXT, attributes, 1);
            //FormulaUtil.EditRegisteredFunction("AND", ExcelFunction.AND, attributes, -1);
            FormulaUtil.EditRegisteredFunction("OR", ExcelFunction.OR, attributes, -1);
            FormulaUtil.EditRegisteredFunction("MOD", ExcelFunction.MOD, attributes, -1);
            FormulaUtil.EditRegisteredFunction("MOD", ExcelFunction.MOD, attributes, -1);
        }
        else
        {
            ReferenceIndexAttribute[] attributes = new ReferenceIndexAttribute[]
            {
              new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
            };
            FormulaUtil.EditRegisteredFunction("ISNUMBER", ExcelFunction.ISNUMBER, attributes, 1);
            FormulaUtil.EditRegisteredFunction("ISBLANK", ExcelFunction.ISBLANK, attributes, 1);
            FormulaUtil.EditRegisteredFunction("ISERR", ExcelFunction.ISERR, attributes, 1);
            FormulaUtil.EditRegisteredFunction("ISEVEN", ExcelFunction.ISEVEN, attributes, 1);
            FormulaUtil.EditRegisteredFunction("ISLOGICAL", ExcelFunction.ISLOGICAL, attributes, 1);
            FormulaUtil.EditRegisteredFunction("ISNA", ExcelFunction.ISNA, attributes, 1);
            FormulaUtil.EditRegisteredFunction("ISNONTEXT", ExcelFunction.ISNONTEXT, attributes, 1);
            FormulaUtil.EditRegisteredFunction("ISODD", ExcelFunction.ISODD, attributes, 1);
            FormulaUtil.EditRegisteredFunction("ISPMT", ExcelFunction.ISPMT, attributes, 1);
            FormulaUtil.EditRegisteredFunction("ISREF", ExcelFunction.ISREF, attributes, 1);
            FormulaUtil.EditRegisteredFunction("ISTEXT", ExcelFunction.ISTEXT, attributes, 1);
           // FormulaUtil.EditRegisteredFunction("AND", ExcelFunction.AND, attributes, -1);
            FormulaUtil.EditRegisteredFunction("OR", ExcelFunction.OR, attributes, -1);
            FormulaUtil.EditRegisteredFunction("MOD", ExcelFunction.MOD, attributes, -1);
            FormulaUtil.EditRegisteredFunction("MOD", ExcelFunction.MOD, attributes, -1);
        }
    }
    /// <summary>
    /// Sets first and second formula values.
    /// </summary>
    /// <param name="value">Value to set.</param>
    /// <param name="formulaUtil">Formula util.</param>
    /// <param name="isFormulaOne">True - first formula, false - second formula.</param>
    public void SetFormulaOneTwoValue( string value, FormulaUtil formulaUtil, bool isFormulaOne )
    {
      Ptg[] parsedFormula = GetFormulaPtg( ref value, formulaUtil, m_DVCollection.Worksheet,0,0 );

      if( isFormulaOne )
      {
        m_dvRecord.FirstFormulaTokens = parsedFormula;
        m_strFirstFormula = value;
      }
      else
      {
        m_dvRecord.SecondFormulaTokens = parsedFormula;
        m_strSecondFormula = value;
      }
    }
    /// <summary>
    /// Sets first and second formula values in Xml Parser
    /// </summary>
    /// <param name="value">Value to set.</param>
    /// <param name="formulaUtil">Formula util.</param>
    /// <param name="taddr">Cell Range Address</param>
    /// <param name="isFormulaOne">True - first formula, false - second formula.</param>
    public void SetFormulaValue(string value, FormulaUtil formulaUtil, TAddr taddr, bool isFormulaOne)
    {
        Ptg[] parsedFormula = formulaUtil.ParseString(value, m_DVCollection.Worksheet, null, taddr.FirstRow,taddr.FirstCol,true);

        if (isFormulaOne)
        {
            m_dvRecord.FirstFormulaTokens = parsedFormula;
            m_strFirstFormula = value;
        }
        else
        {
            m_dvRecord.SecondFormulaTokens = parsedFormula;
            m_strSecondFormula = value;
        }
    }
    /// <summary>
    /// Defines whether Ptg array represents zero value.
    /// </summary>
    /// <param name="parsedFormula">Parsed formula Ptg array.</param>
    /// <returns>True if parsed formula represents zero value, otherwise false.</returns>
    private static bool IsZeroValuePtg( Ptg[] parsedFormula )
    {
      bool bIsZero = true;

      DoublePtg doublePtg = parsedFormula[ 0 ] as DoublePtg;
      if( doublePtg != null )
      {
        if( doublePtg.Value == 0 )
        {
          for( int i = 1, iLength = parsedFormula.Length; i < iLength; i++ )
          {
            FormulaToken formulaToken = parsedFormula[ i ].TokenCode;

            if( formulaToken != FormulaToken.tUnaryMinus && formulaToken != FormulaToken.tUnaryPlus )
            {
              bIsZero = false;
              break;
            }
          }
        }
      }
      else
      {
        bIsZero = false;
      }

      return bIsZero;
    }
    #endregion

    #region Class Properties
    /// <summary>
    /// Parent workbook.
    /// </summary>
    public WorkbookImpl Workbook
    {
      get
      {
        return m_DVCollection.Workbook;
      }
    }
    /// <summary>
    /// Parent worksheet.
    /// </summary>
    public WorksheetImpl Worksheet
    {
      get
      {
        return m_DVCollection.Worksheet;
      }
    }
    /// <summary>
    /// Returns internal CFRecord. Read-only.
    /// </summary>
    [CLSCompliant( false )]
    public DVRecord DVRecord
    {
      get
      {
        return m_dvRecord;
      }
    }
    /// <summary>
    /// Gets / sets parent data validation collection.
    /// </summary>
    public DataValidationCollection ParentCollection
    {
      get
      {
        return m_DVCollection;
      }
      set
      {
        if( value == null )
          throw new ArgumentNullException( "value" );

        if( m_DVCollection != value )
        {
          m_DVCollection = value;
        }
      }
    }
    /// <summary>
    /// Gets ranges for this data validation.
    /// </summary>
    public string[] DVRanges
    {
      get
      {
        if( m_dvRecord.AddrList.Length == 0 )
        {
          FillFromCells();
        }

        List<string> dvRanges = new List<string>();

        TAddr[] tAddrArray = m_dvRecord.AddrList;
        int iCount = m_dvRecord.AddrListSize;

        for( int i = 0; i < iCount; i++ )
        {
          TAddr tAddr = tAddrArray[ i ];
          string strRange = RangeImpl.GetAddressLocal( tAddr.FirstRow + 1, tAddr.FirstCol + 1, tAddr.LastRow + 1, tAddr.LastCol + 1 );
          dvRanges.Add( strRange );
        }

        return dvRanges.ToArray();
      }
    }
    /// <summary>
    /// Gets number of required shapes objects.
    /// </summary>
    public int ShapesCount
    {
      get
      {
        return m_cells.CellList.Count;
      }
    }
    #endregion

    #region IParentApplication Members
    /// <summary>
    /// Application object for this object.
    /// </summary>
    public IApplication Application
    {
      get
      {
        return m_DVCollection.Application;
      }
    }
    /// <summary>
    /// Parent object for this object.
    /// </summary>
    public object Parent
    {
      get
      {
        return m_DVCollection;
      }
    }
    #endregion

    #region IDataValidation Members
    /// <summary>
    /// 
    /// </summary>
    public string PromptBoxTitle
    {
      get
      {
        return m_dvRecord.PromtBoxTitle;
      }
      set
      {
        CheckLimit( "PromptBoxTitle", value, TitleLimit );

        if( m_dvRecord.PromtBoxTitle != value )
        {
          m_dvRecord.PromtBoxTitle = value;
        }
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public string PromptBoxText
    {
      get
      {
        return m_dvRecord.PromtBoxText;
      }
      set
      {
        CheckLimit( "PromptBoxText", value, InputTextLimit );

        if( m_dvRecord.PromtBoxText != value )
        {
          m_dvRecord.PromtBoxText = value;
        }
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public string ErrorBoxTitle
    {
      get
      {
        return m_dvRecord.ErrorBoxTitle;
      }
      set
      {
        CheckLimit( "ErrorBoxTitle", value, TitleLimit );

        if( m_dvRecord.ErrorBoxTitle != value )
        {
          m_dvRecord.ErrorBoxTitle = value;
        }
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public string ErrorBoxText
    {
      get
      {
        return m_dvRecord.ErrorBoxText;
      }
      set
      {
        CheckLimit( "ErrorBoxText", value, TextLimit );

        if( m_dvRecord.ErrorBoxText != value )
        {
          m_dvRecord.ErrorBoxText = value;
        }
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public string FirstFormula
    {
      get
      {
        return m_strFirstFormula;
      }
      set
      {
        if( m_strFirstFormula != value )
        {
          //m_dvRecord.FirstFormulaTokens = GetFormulaPtg( ref value, null, m_DVCollection.Worksheet );
          //FstCondFormula = FormulaUtil.PtgArrayToByteArray( parsedFormula );
          m_strFirstFormula = value;
        }
      }
    }

    /// <summary>First formula's DateTime value.
    /// </summary>
    public DateTime FirstDateTime
    {
      get
      {
        return ConvertToDateTime( m_dvRecord.FirstFormulaTokens );
      }
      set
      {
        Ptg[] arrPtgs = ConvertFromDateTime( value );
        m_dvRecord.FirstFormulaTokens = arrPtgs;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public string SecondFormula
    {
      get
      {
        return m_strSecondFormula;
      }
      set
      {
        if( m_strSecondFormula != value )
        {
        //  m_dvRecord.SecondFormulaTokens = GetFormulaPtg( ref value, null, m_DVCollection.Worksheet );
          m_strSecondFormula = value;
        }
      }
    }

    /// <summary>
    /// Second formula's DateTime value.
    /// </summary>
    public DateTime SecondDateTime
    {
      get
      {
        return ConvertToDateTime( m_dvRecord.SecondFormulaTokens );
      }
      set
      {
        Ptg[] arrPtgs = ConvertFromDateTime( value );
        m_dvRecord.SecondFormulaTokens = arrPtgs;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public ExcelDataType AllowType
    {
      get
      {
        return m_dvRecord.DataType;
      }
      set
      {
        m_dvRecord.DataType = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public ExcelDataValidationComparisonOperator CompareOperator
    {
      get
      {
        return m_dvRecord.Condition;
      }
      set
      {
        m_dvRecord.Condition = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool IsListInFormula
    {
      get
      {
        return m_dvRecord.IsStrListExplicit;
      }
      set
      {
        m_dvRecord.IsStrListExplicit = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool IsEmptyCellAllowed
    {
      get
      {
        return m_dvRecord.IsEmptyCell;
      }
      set
      {
        m_dvRecord.IsEmptyCell = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool IsSuppressDropDownArrow
    {
      get
      {
        return m_dvRecord.IsSuppressArrow;
      }
      set
      {
        m_dvRecord.IsSuppressArrow = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool ShowPromptBox
    {
      get
      {
        return m_dvRecord.IsShowPromptBox;
      }
      set
      {
        m_dvRecord.IsShowPromptBox = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool ShowErrorBox
    {
      get
      {
        return m_dvRecord.IsShowErrorBox;
      }
      set
      {
        m_dvRecord.IsShowErrorBox = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public int PromptBoxHPosition
    {
      get
      {
        return m_DVCollection.PromptBoxHPosition;
      }
      set
      {
        m_DVCollection.PromptBoxHPosition = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public int PromptBoxVPosition
    {
      get
      {
        return m_DVCollection.PromptBoxVPosition;
      }
      set
      {
        m_DVCollection.PromptBoxVPosition = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool IsPromptBoxVisible
    {
      get
      {
        return m_DVCollection.IsPromptBoxVisible;
      }
      set
      {
        m_DVCollection.IsPromptBoxVisible = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool IsPromptBoxPositionFixed
    {
      get
      {
        return m_DVCollection.IsPromptBoxPositionFixed;
      }
      set
      {
        m_DVCollection.IsPromptBoxPositionFixed = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public ExcelErrorStyle ErrorStyle
    {
      get
      {
        return m_dvRecord.ErrorStyle;
      }
      set
      {
        m_dvRecord.ErrorStyle = value;
      }
    }
    /// <summary>
    /// If this is data validation list, then gets / sets array of all possible values.
    /// </summary>
    public string[] ListOfValues
    {
      get
      {
        if( IsListInFormula == true && AllowType == ExcelDataType.User )
        {
          Ptg[] expression = m_dvRecord.FirstFormulaTokens;

          if( expression != null && expression.Length == 1 && expression[ 0 ] is StringConstantPtg )
          {
            StringConstantPtg stringPtg = expression[ 0 ] as StringConstantPtg;
            string[] values = stringPtg.Value.Split( '\0' );

            if( values != null && values.Length > 0 )
            {
              return values;
            }
          }
        }
        else if (!IsListInFormula && AllowType == ExcelDataType.User)
        {
            Ptg[] expression = m_dvRecord.FirstFormulaTokens;

            //Only one area range can be added with List Data validation

            if (expression != null && expression.Length == 1 && expression[0] is AreaPtg)
            {
                AreaPtg areaPtg = expression[0] as AreaPtg;
                IRange areaRange=areaPtg.GetRange(this.Workbook as IWorkbook, this.Worksheet as IWorksheet);
               
                //The cell refernces of an data validation list should be single column or single row
                int firstRow = areaRange.Row;
                int firstColumn = areaRange.Column;
                int lastRow = areaRange.LastRow;
                int lastColumn = areaRange.LastColumn;

                bool isSingleRow = (firstRow == lastRow);
                bool isSingleColumn = (firstColumn == lastColumn);

                string[] values = null;
                int index=0;
                if (isSingleRow)
                {
                    values = new string[(lastColumn - firstColumn) + 1];
                    for (int col = firstColumn; col <= lastColumn; col++)
                    {
                        values[index] = this.Worksheet[firstRow, col].Value;
                        index++;
                    }
                }
                else if (isSingleColumn)
                { 
                    values = new string[(lastRow - firstRow) + 1];
                    for (int row = firstRow; row <= lastRow; row++)
                    {
                        values[index] = this.Worksheet[row, firstColumn].Value;
                        index++;
                    }
                }                               
                if (values.Length > 0)
                {
                    return values;
                }
            }
        }

        return null;
      }
      set
      {
        if( value == null )
          throw new ArgumentNullException( "value" );

        StringBuilder builder = new StringBuilder( "\"" );

        for( int i = 0, len = value.Length; i < len; i++ )
        {
          builder.Append( ( i == len ) ? value[ i ] : value[ i ] + "\0" );

        }

        if( builder.Length > DEF_MAX_LIST_LENGTH )
        {
          throw new ArgumentOutOfRangeException( "value", "Too many strings in the array or strings are too long." );
        }

        builder.Append( "\"" );
        
        SetFormulaOneTwoValue(builder.ToString(), new FormulaUtil(m_DVCollection.Workbook.Application, m_DVCollection.Workbook, NumberFormatInfo.InvariantInfo,
        ApplicationImpl.DEF_ARGUMENT_SEPARATOR, ApplicationImpl.DEF_ROW_SEPARATOR), true);

        IsListInFormula = true;
        IsSuppressDropDownArrow = false;
        AllowType = ExcelDataType.User;
        CompareOperator = ExcelDataValidationComparisonOperator.NotEqual;
        ErrorStyle = ExcelErrorStyle.Stop;
        ShowErrorBox = true;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public IRange DataRange
    {
      get
      {
        if( AllowType == ExcelDataType.User )
        {
          Ptg[] expression = m_dvRecord.FirstFormulaTokens;
          WorkbookImpl book = Workbook;

          if( expression != null && expression.Length == 1
            && Array.IndexOf( DATARANGETYPES, expression[ 0 ].GetType() ) != -1 )
          {
            bool bR1C1 = book.CalculationOptions.R1C1ReferenceMode;
            string strAddress = expression[ 0 ].ToString( Workbook.FormulaUtil, 0, 0, bR1C1 );
            return Workbook.Worksheets[ 0 ].Range[ strAddress ];
          }
        }

        return null;
      }
      set
      {
        if( value == null )
          throw new ArgumentNullException( "DataRange" );

        if( !Workbook.Allow3DRangesInDataValidation && value.Worksheet != Worksheet )
          throw new ArgumentException( "Data range should be from same worksheet" );

        RangeImpl range = ( RangeImpl )value;

        if( value.Worksheet == Worksheet )
        {
          FirstFormula = range.AddressGlobalWithoutSheetName;
        }
        else
        {
          FirstFormula = range.AddressGlobal;
        }
        SetFormulaOneTwoValue(FirstFormula, new FormulaUtil(m_DVCollection.Workbook.Application, m_DVCollection.Workbook, NumberFormatInfo.InvariantInfo,
      ApplicationImpl.DEF_ARGUMENT_SEPARATOR, ApplicationImpl.DEF_ROW_SEPARATOR), true);
        SecondFormula = "";
        CompareOperator = ExcelDataValidationComparisonOperator.NotEqual;
        AllowType = ExcelDataType.User;
        ShowPromptBox = true;
        ShowErrorBox = true;
        IsListInFormula = false;//true;
        IsEmptyCellAllowed = true;
        IsSuppressDropDownArrow = false;
      }
    }
    #endregion

    #region IInternalDataValidation members
    /// <summary>
    /// 
    /// </summary>
    public Ptg[] FirstFormulaTokens
    {
      get
      {
        return m_dvRecord.FirstFormulaTokens;
      }
      set
      {
        m_dvRecord.FirstFormulaTokens = value;
        //FstCondFormula = FormulaUtil.PtgArrayToByteArray( parsedFormula );
         //m_strFirstFormula = value;
        //UpdateFirstFormulaString();
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public Ptg[] SecondFormulaTokens
    {
      get
      {
        return m_dvRecord.SecondFormulaTokens;
      }
      set
      {
        m_dvRecord.SecondFormulaTokens = value;
        //FstCondFormula = FormulaUtil.PtgArrayToByteArray( parsedFormula );
        //m_strFirstFormula = value;
        //UpdateSecondFormulaString();
      }
    }
    /// <summary>
    /// Updates first formula string based on the formula tokens.
    /// </summary>
    private void UpdateFirstFormulaString()
    {
      WorkbookImpl book = Workbook;
      FormulaUtil formulaUtils = book.FormulaUtil;
      Ptg[] ptgs = m_dvRecord.FirstFormulaTokens;

      int iRow;
      int iColumn ;

      GetRowColumn( out iRow, out iColumn );


      if( ptgs != null && ptgs.Length > 0 )
      {
        m_strFirstFormula = formulaUtils.ParsePtgArray( ptgs, iRow, iColumn, false, false );
      }
    }
    /// <summary>
    /// Updates second formula string based on the formula tokens.
    /// </summary>
    private void UpdateSecondFormulaString()
    {
      WorkbookImpl book = Workbook;
      FormulaUtil formulaParser = book.FormulaUtil;
      Ptg[] ptgs = m_dvRecord.SecondFormulaTokens;
      int iRow;
      int iColumn;
      GetRowColumn( out iRow, out iColumn );

      if( ptgs != null && ptgs.Length > 0 )
      {
        m_strSecondFormula = formulaParser.ParsePtgArray( ptgs, iRow, iColumn, false, false );
      }
    }
    /// <summary>
    /// Gets row and column indexes of the first cell.
    /// </summary>
    /// <param name="iRow"></param>
    /// <param name="iColumn"></param>
    private void GetRowColumn( out int iRow, out int iColumn )
    {
      iRow = 0;
      iColumn = 0;

      if( m_cells != null && m_cells.CellList.Count > 0 )
      {
        Rectangle firstRect = m_cells.CellList[ 0 ];
        iRow = firstRect.Top + 1;
        iColumn = firstRect.Left + 1;
      }
    }

    #endregion

    #region IReparse Members
    /// <summary>
    /// 
    /// </summary>
    public void Reparse()
    {
      UpdateFirstFormulaString();
      UpdateSecondFormulaString();
    }
    #endregion

    #region Class Public Methods
    /// <summary>
    /// Returns data validation first or second formula string value.
    /// </summary>
    /// <param name="formulaUtil">Formula util.</param>
    /// <param name="bIsFirstFormula">Is first formula.</param>
    /// <returns>First or second formula string value.</returns>
    public string GetFirstSecondFormula( FormulaUtil formulaUtil, bool bIsFirstFormula )
    {
      //string strResult = string.Empty;

      Ptg[] arrTokens = ( bIsFirstFormula ) ? m_dvRecord.FirstFormulaTokens : m_dvRecord.SecondFormulaTokens;

      /*
      if( bIsFirstFormula )
      {
        strResult = formulaUtil.ParsePtgArray( m_dvRecord.FirstFormulaTokens );
      }
      else
      {
        strResult = formulaUtil.ParsePtgArray( m_dvRecord.SecondFormulaTokens );
      }

      return strResult;
      */
      List<Rectangle> lstRectangles = m_cells.CellList;
      Rectangle rect = ( lstRectangles.Count > 0 ) ?
        lstRectangles[ 0 ] :
        Rectangle.Empty ;

      return ( arrTokens != null ) ?
        formulaUtil.ParsePtgArray( arrTokens, rect.Top + 1, rect.Left + 1, false, false ) :
        string.Empty;
    }
    /// <summary>
    /// Returns data validation first or second formula string value in R1C1 format
    /// </summary>
    /// <param name="bIsFirstFormula">Is first formula.</param>
    /// <returns>First or second formula string value.</returns>
    public string GetR1C1FirstSecondFormula(FormulaUtil formulaUtil, bool bIsFirstFormula)
    {
        //string strResult = string.Empty;

        Ptg[] arrTokens = (bIsFirstFormula) ? m_dvRecord.FirstFormulaTokens : m_dvRecord.SecondFormulaTokens;

        /*
        if( bIsFirstFormula )
        {
          strResult = formulaUtil.ParsePtgArray( m_dvRecord.FirstFormulaTokens );
        }
        else
        {
          strResult = formulaUtil.ParsePtgArray( m_dvRecord.SecondFormulaTokens );
        }

        return strResult;
        */
        List<Rectangle> lstRectangles = m_cells.CellList;
        Rectangle rect = (lstRectangles.Count > 0) ?
          lstRectangles[0] :
          Rectangle.Empty;

        return (arrTokens != null) ?
          formulaUtil.ParsePtgArray(arrTokens, rect.Top + 1, rect.Left + 1, true, false) :
          string.Empty;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="records"></param>
    [CLSCompliant( false )]
    public void Serialize( OffsetArrayList records )
    {
      FillFromCells();
      records.Add( m_dvRecord );
    }
    /// <summary>
    /// Clones current instance.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    /// <returns>Returns just cloned object.</returns>
    public object Clone( object parent )
    {
      DataValidationImpl result = ( DataValidationImpl )MemberwiseClone();
      result.SetParent( parent as DataValidationCollection );

      result.m_dvRecord = ( DVRecord )CloneUtils.CloneCloneable( m_dvRecord );
      result.m_cells = m_cells.Clone();

      return result;
    }
    /// <summary>
    /// Sets parent collection object.
    /// </summary>
    /// <param name="dataValidationCollection">Parent object.</param>
    private void SetParent( DataValidationCollection dataValidationCollection )
    {
      if( dataValidationCollection == null )
        throw new ArgumentNullException( "dataValidationCollection" );

      m_DVCollection = dataValidationCollection;
    }
    /// <summary>
    /// Indicates whether this object contains data validation settings for cell with specified index.
    /// </summary>
    /// <param name="lCellIndex">Cell index to search.</param>
    /// <returns>True if this object contains data validation settings for cell with specified index.</returns>
    public bool ContainsCell( long lCellIndex )
    {
      int iFirstRow = RangeImpl.GetRowFromCellIndex( lCellIndex );
      int iFirstColumn = RangeImpl.GetColumnFromCellIndex( lCellIndex );
      Rectangle rect = Rectangle.FromLTRB( iFirstColumn - 1, iFirstRow - 1, iFirstColumn - 1, iFirstRow - 1 );
      return m_cells.Contains( rect );
    }
    /// <summary>
    /// Updates indexes to named ranges.
    /// </summary>
    /// <param name="arrNewIndex">New indexes.</param>
    public void UpdateNamedRangeIndexes( int[] arrNewIndex )
    {
      if( arrNewIndex == null )
        throw new ArgumentNullException( "arrNewIndex" );

      FormulaUtil parser = Workbook.FormulaUtil;
      parser.UpdateNameIndex( m_dvRecord.FirstFormulaTokens, arrNewIndex );
      parser.UpdateNameIndex( m_dvRecord.SecondFormulaTokens, arrNewIndex );
    }
    /// <summary>
    /// Updates indexes to named ranges.
    /// </summary>
    /// <param name="dicNewIndex">New indexes.</param>
    public void UpdateNamedRangeIndexes( IDictionary<int, int> dicNewIndex )
    {
      if( dicNewIndex == null )
        throw new ArgumentNullException( "dicNewIndex" );

      FormulaUtil parser = Workbook.FormulaUtil;
      parser.UpdateNameIndex( m_dvRecord.FirstFormulaTokens, dicNewIndex );
      parser.UpdateNameIndex( m_dvRecord.SecondFormulaTokens, dicNewIndex );
    }
    /// <summary>
    /// This method should be called before several updates to the object will take place.
    /// </summary>
    public void BeginUpdate()
    {
    }
    /// <summary>
    /// This method should be called after several updates to the object took place.
    /// </summary>
    public void EndUpdate()
    {
    }
    /// <summary>
    /// Sets items with used reference indexes to true.
    /// </summary>
    /// <param name="usedItems">Array to mark used references in.</param>
    public void MarkUsedReferences( bool[] usedItems )
    {
      FormulaUtil.MarkUsedReferences( m_dvRecord.FirstFormulaTokens, usedItems );
      FormulaUtil.MarkUsedReferences( m_dvRecord.SecondFormulaTokens, usedItems );
    }
    /// <summary>
    /// Updates reference indexes.
    /// </summary>
    /// <param name="arrUpdatedIndexes">Array with updated indexes.</param>
    public void UpdateReferenceIndexes( int[] arrUpdatedIndexes )
    {
      Ptg[] tokens = m_dvRecord.FirstFormulaTokens;

      if( FormulaUtil.UpdateReferenceIndexes( tokens, arrUpdatedIndexes ) )
        m_dvRecord.FirstFormulaTokens = tokens;

      tokens = m_dvRecord.SecondFormulaTokens;

      if( FormulaUtil.UpdateReferenceIndexes( tokens, arrUpdatedIndexes ) )
        m_dvRecord.SecondFormulaTokens = tokens;

    }

    internal DataValidationImpl Clone( DataValidationCollection dataValidationCollection,
      int iSourceRow, int iSourceColumn, int iRowDelta, int iColumnDelta, int iRowCount, int iColumnCount )
    {
      DataValidationImpl result = ( DataValidationImpl )Clone( m_DVCollection );
      // TODO: this operation can be optimized.
      WorkbookImpl book = result.Workbook;

      Rectangle[] toRemove = new Rectangle[]
      {
        Rectangle.FromLTRB( 0, 0, book.MaxColumnCount - 1, iSourceRow - 2 ),
        Rectangle.FromLTRB( 0, iSourceRow - 1, iSourceColumn - 2, iSourceRow + iRowCount - 1 ),
        Rectangle.FromLTRB( 0, iSourceRow + iRowCount - 1, book.MaxColumnCount - 1, book.MaxRowCount - 1 ),
        Rectangle.FromLTRB( iSourceColumn + iColumnCount - 1, iSourceRow - 1,
          book.MaxColumnCount - 1, iSourceRow + iRowCount - 1 ),
      };

      result.RemoveRange( toRemove );
      result.m_cells.Offset( iRowDelta, iColumnDelta, result.m_DVCollection.Workbook );
      return result;
    }
    /// <summary>
    /// Checks whether specified value is inside requested limit.
    /// </summary>
    /// <param name="propertyName">Property name.</param>
    /// <param name="value">Property value.</param>
    /// <param name="limit">Length limit</param>
    private void CheckLimit( string propertyName, string value, int limit )
    {
      if( value != null && value.Length > limit )
        throw new ArgumentOutOfRangeException( string.Format( "{0} cannot exceed {1} characters.", propertyName, limit ) );
    }

    #endregion
  }
}
