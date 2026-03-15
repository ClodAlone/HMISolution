#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;

#if ( WINRT )
using Windows.UI;
using Rectangle=Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
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

using Syncfusion.XlsIO.Implementation.Collections;
using Syncfusion.XlsIO.Parser.Biff_Records.Formula;
using Syncfusion.XlsIO.Interfaces;

namespace Syncfusion.XlsIO.Implementation
{
  /// <summary>
  /// Contains a condition and the formatting attributes 
  /// applied to the cell, if the condition is met.
  /// Used for multiple-cells range.
  /// </summary>
  public class DataValidationArray :
    CommonWrapper,
    IInternalDataValidation
  {
    #region Class members
    /// <summary>
    /// Parent range
    /// </summary>
    private IRange m_range;
    /// <summary>
    /// List with cached data validation.
    /// </summary>
    private List<IDataValidation> m_arrValidationList = new List<IDataValidation>();
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Creates conditional format and sets its Application and Parent properties
    /// </summary>
    /// <param name="parent">Parent object for the format.</param>
    public DataValidationArray( IRange parent )
    {
      m_range = parent;
    }
    #endregion

    #region IDataValidation Members
    /// <summary>
    /// Title of the prompt box.
    /// </summary>
    public string PromptBoxTitle
    {
      get
      {
        string result =
          m_range.Cells[ 0 ].DataValidation.PromptBoxTitle;

        IRange[] cells = m_range.Cells;

        for( int i = 0, iLen = cells.Length; i < iLen; i++ )
        {
          IRange cell = cells[ i ];
          if( cell.DataValidation.PromptBoxTitle != result )
          {
            return null;
          }
        }

        return result;
      }
      set
      {
        IterateDVs( SetPromptBoxTitle, value );
      }
    }
    /// <summary>
    /// Prompt box message.
    /// </summary>
    public string PromptBoxText
    {
      get
      {
        string result =
          m_range.Cells[ 0 ].DataValidation.PromptBoxText;

        IRange[] cells = m_range.Cells;

        for( int i = 0, iLen = cells.Length; i < iLen; i++ )
        {
          IRange cell = cells[ i ];
          if( cell.DataValidation.PromptBoxText != result )
          {
            return null;
          }
        }

        return result;
      }
      set
      {
        IterateDVs( SetPromptBoxText, value );
      }
    }
    /// <summary>
    /// Title of the error box.
    /// </summary>
    public string ErrorBoxTitle
    {
      get
      {
        string result =
          m_range.Cells[ 0 ].DataValidation.ErrorBoxTitle;

        IRange[] cells = m_range.Cells;

        for( int i = 0, iLen = cells.Length; i < iLen; i++ )
        {
          IRange cell = cells[ i ];
          if( cell.DataValidation.ErrorBoxTitle != result )
          {
            return null;
          }
        }

        return result;
      }
      set
      {
        IterateDVs( SetErrorBoxTitle, value );
      }
    }
    /// <summary>
    /// Error box message.
    /// </summary>
    public string ErrorBoxText
    {
      get
      {
        string result =
          m_range.Cells[ 0 ].DataValidation.ErrorBoxText;

        IRange[] cells = m_range.Cells;

        for( int i = 0, iLen = cells.Length; i < iLen; i++ )
        {
          IRange cell = cells[ i ];
          if( cell.DataValidation.ErrorBoxText != result )
          {
            return null;
          }
        }

        return result;
      }
      set
      {
        IterateDVs( SetErrorBoxText, value );
      }
    }
    /// <summary>
    /// First formula's DateTime value.
    /// </summary>
    public string FirstFormula
    {
      get
      {
        string result = m_range[ m_range.Row, m_range.Column ].DataValidation.FirstFormula;

        for( int iRow = m_range.Row, iLastRow = m_range.LastRow; iRow <= iLastRow; iRow++ )
        {
          for( int iColumn = m_range.Column, iLastColumn = m_range.LastColumn; iColumn <= iLastColumn; iColumn++ )
          {
            if( m_range[ iRow, iColumn ].DataValidation.FirstFormula != result )
            {
              return null;
            }
          }
        }

        return result;
      }
      set
      {
        WorkbookImpl book = ( WorkbookImpl )m_range.Worksheet.Workbook;
        FormulaUtil formulaUtil = book.FormulaUtil;

        if( value != null && value.Length > 0 && value[ 0 ] == '=' )
          value = UtilityMethods.RemoveFirstCharUnsafe( value );

        Ptg[] tokens = DataValidationImpl.GetFormulaPtg( ref value, null,
          ( WorksheetImpl )m_range.Worksheet,0,0 );

        IterateDVs( SetFirstFormulaTokens, tokens );
        //IterateCells( SetFirstFormula, value );
      }
    }
    /// <summary>
    /// First formula's DateTime value.
    /// </summary>
    public DateTime FirstDateTime
    {
      get
      {
        DateTime result =
          m_range.Cells[ 0 ].DataValidation.FirstDateTime;

        IRange[] cells = m_range.Cells;

        for( int i = 0, iLen = cells.Length; i < iLen; i++ )
        {
          IRange cell = cells[ i ];

          if( cell.DataValidation.FirstDateTime != result )
          {
            return DateTime.MinValue;
          }
        }

        return result;
      }
      set
      {
        IterateDVs( SetFirstDateTime, value );
      }
    }
    /// <summary>
    /// Second formula.
    /// </summary>
    public string SecondFormula
    {
      get
      {
        string result =
          m_range.Cells[ 0 ].DataValidation.SecondFormula;

        IRange[] cells = m_range.Cells;

        for( int i = 0, iLen = cells.Length; i < iLen; i++ )
        {
          IRange cell = cells[ i ];
          if( cell.DataValidation.SecondFormula != result )
          {
            return null;
          }
        }

        return result;
      }
      set
      {
        WorkbookImpl book = ( WorkbookImpl )m_range.Worksheet.Workbook;
        FormulaUtil formulaUtil = book.FormulaUtil;

        if( value != null && value.Length > 0 && value[ 0 ] == '=' )
          value = UtilityMethods.RemoveFirstCharUnsafe( value );

        Ptg[] tokens = DataValidationImpl.GetFormulaPtg( ref value, null,
          ( WorksheetImpl )m_range.Worksheet,0,0 );

        IterateDVs( SetSecondFormulaTokens, tokens );
        //IterateDVs( SetSecondFormula, value );
      }
    }
    /// <summary>
    /// Second formula's DateTime value.
    /// </summary>
    public DateTime SecondDateTime
    {
      get
      {
        DateTime result =
          m_range.Cells[ 0 ].DataValidation.SecondDateTime;

        IRange[] cells = m_range.Cells;

        for( int i = 0, iLen = cells.Length; i < iLen; i++ )
        {
          IRange cell = cells[ i ];

          if( cell.DataValidation.SecondDateTime != result )
          {
            return DateTime.MinValue;
          }
        }

        return result;
      }
      set
      {
        IterateDVs( SetSecondDateTime, value );
      }
    }
    /// <summary>
    /// Type of the allowed data.
    /// </summary>
    public ExcelDataType AllowType
    {
      get
      {
        ExcelDataType result =
          m_range.Cells[ 0 ].DataValidation.AllowType;

        IRange[] cells = m_range.Cells;

        for( int i = 0, iLen = cells.Length; i < iLen; i++ )
        {
          IRange cell = cells[ i ];
          if( cell.DataValidation.AllowType != result )
          {
            return ExcelDataType.Any;
          }
        }

        return result;
      }
      set
      {
        IterateDVs( SetAllowType, value );
      }
    }
    /// <summary>
    /// Comparison operator between two formula values.
    /// </summary>
    public ExcelDataValidationComparisonOperator CompareOperator
    {
      get
      {
        ExcelDataValidationComparisonOperator result =
          m_range.Cells[ 0 ].DataValidation.CompareOperator;

        IRange[] cells = m_range.Cells;

        for( int i = 0, iLen = cells.Length; i < iLen; i++ )
        {
          IRange cell = cells[ i ];
          if( cell.DataValidation.CompareOperator != result )
          {
            return ExcelDataValidationComparisonOperator.Between;
          }
        }

        return result;
      }
      set
      {
        IterateDVs( SetCompareOperator, value );
      }
    }
    /// <summary>
    /// Indicates whether there is list of values in the first formula.
    /// </summary>
    public bool IsListInFormula
    {
      get
      {
        bool result =
          m_range.Cells[ 0 ].DataValidation.IsListInFormula;

        IRange[] cells = m_range.Cells;

        for( int i = 0, iLen = cells.Length; i < iLen; i++ )
        {
          IRange cell = cells[ i ];
          if( cell.DataValidation.IsListInFormula != result )
          {
            return false;
          }
        }

        return result;
      }
      set
      {
        IterateDVs( SetIsListInFormula, value );
      }
    }
    /// <summary>
    /// Indicates whether empty cells are valid.
    /// </summary>
    public bool IsEmptyCellAllowed
    {
      get
      {
        bool result =
          m_range.Cells[ 0 ].DataValidation.IsEmptyCellAllowed;

        IRange[] cells = m_range.Cells;

        for( int i = 0, iLen = cells.Length; i < iLen; i++ )
        {
          IRange cell = cells[ i ];
          if( cell.DataValidation.IsEmptyCellAllowed != result )
          {
            return false;
          }
        }

        return result;
      }
      set
      {
        IterateDVs( SetIsEmptyCellAllowed, value );
      }
    }
    /// <summary>
    /// Indicates whether to hide drop-down arrow for list of values.
    /// </summary>
    public bool IsSuppressDropDownArrow
    {
      get
      {
        bool result =
          m_range.Cells[ 0 ].DataValidation.IsSuppressDropDownArrow;

        IRange[] cells = m_range.Cells;

        for( int i = 0, iLen = cells.Length; i < iLen; i++ )
        {
          IRange cell = cells[ i ];
          if( cell.DataValidation.IsSuppressDropDownArrow != result )
          {
            return false;
          }
        }

        return result;
      }
      set
      {
        IterateDVs( SetIsSuppressDropDownArrow, value );
      }
    }
    /// <summary>
    /// Indicates whether to show prompt box.
    /// </summary>
    public bool ShowPromptBox
    {
      get
      {
        bool result =
          m_range.Cells[ 0 ].DataValidation.ShowPromptBox;

        IRange[] cells = m_range.Cells;

        for( int i = 0, iLen = cells.Length; i < iLen; i++ )
        {
          IRange cell = cells[ i ];
          if( cell.DataValidation.ShowPromptBox != result )
          {
            return false;
          }
        }

        return result;
      }
      set
      {
        IterateDVs( SetShowPromptBox, value );
      }
    }
    /// <summary>
    /// Indicates whether to show error box.
    /// </summary>
    public bool ShowErrorBox
    {
      get
      {
        bool result =
          m_range.Cells[ 0 ].DataValidation.ShowErrorBox;

        IRange[] cells = m_range.Cells;

        for( int i = 0, iLen = cells.Length; i < iLen; i++ )
        {
          IRange cell = cells[ i ];
          if( cell.DataValidation.ShowErrorBox != result )
          {
            return false;
          }
        }

        return result;
      }
      set
      {
        IterateDVs( SetShowErrorBox, value );
      }
    }
    /// <summary>
    /// Horizontal position of the prompt box.
    /// </summary>
    public int PromptBoxHPosition
    {
      get
      {
        int result =
          m_range.Cells[ 0 ].DataValidation.PromptBoxHPosition;

        IRange[] cells = m_range.Cells;

        for( int i = 0, iLen = cells.Length; i < iLen; i++ )
        {
          IRange cell = cells[ i ];
          if( cell.DataValidation.PromptBoxHPosition != result )
          {
            return int.MinValue;
          }
        }

        return result;
      }
      set
      {
        IterateDVs( SetPromptBoxHPosition, value );
      }
    }
    /// <summary>
    /// Vertical position of the prompt box.
    /// </summary>
    public int PromptBoxVPosition
    {
      get
      {
        int result =
          m_range.Cells[ 0 ].DataValidation.PromptBoxVPosition;

        IRange[] cells = m_range.Cells;

        for( int i = 0, iLen = cells.Length; i < iLen; i++ )
        {
          IRange cell = cells[ i ];
          if( cell.DataValidation.PromptBoxVPosition != result )
          {
            return int.MinValue;
          }
        }

        return result;
      }
      set
      {
        IterateDVs( SetPromptBoxVPosition, value );
      }
    }
    /// <summary>
    /// Indicates whether prompt box is visible.
    /// </summary>
    public bool IsPromptBoxVisible
    {
      get
      {
        bool result =
          m_range.Cells[ 0 ].DataValidation.IsPromptBoxVisible;

        IRange[] cells = m_range.Cells;

        for( int i = 0, iLen = cells.Length; i < iLen; i++ )
        {
          IRange cell = cells[ i ];
          if( cell.DataValidation.IsPromptBoxVisible != result )
          {
            return false;
          }
        }

        return result;
      }
      set
      {
        IterateDVs( SetIsPromptBoxVisible, value );
      }
    }
    /// <summary>
    /// Indicates whether position of the prompt box is fixed.
    /// </summary>
    public bool IsPromptBoxPositionFixed
    {
      get
      {
        bool result =
          m_range.Cells[ 0 ].DataValidation.IsPromptBoxPositionFixed;

        IRange[] cells = m_range.Cells;

        for( int i = 0, iLen = cells.Length; i < iLen; i++ )
        {
          IRange cell = cells[ i ];
          if( cell.DataValidation.IsPromptBoxPositionFixed != result )
          {
            return false;
          }
        }

        return result;
      }
      set
      {
        IterateDVs( SetIsPromptBoxPositionFixed, value );
      }
    }
    /// <summary>
    /// Style of the error.
    /// </summary>
    public ExcelErrorStyle ErrorStyle
    {
      get
      {
        ExcelErrorStyle result =
          m_range.Cells[ 0 ].DataValidation.ErrorStyle;

        IRange[] cells = m_range.Cells;

        for( int i = 0, iLen = cells.Length; i < iLen; i++ )
        {
          IRange cell = cells[ i ];
          if( cell.DataValidation.ErrorStyle != result )
          {
            return ExcelErrorStyle.Stop;
          }
        }

        return result;
      }
      set
      {
        IterateDVs( SetErrorStyle, value );
      }
    }
    /// <summary>
    /// List of allowed values (only for ExcelDataType.User).
    /// </summary>
    public string[] ListOfValues
    {
      get
      {
        string[] result =
          m_range.Cells[ 0 ].DataValidation.ListOfValues;

        if( result == null )
          return null;

        if( this.FirstFormula == m_range.Cells[ 0 ].DataValidation.FirstFormula )
        {
          return result;
        }

        return null;
      }
      set
      {
        IterateDVs( SetListOfValues, value );
      }
    }
    /// <summary>
    /// Range of allowed values (only for ExcelDataType.User).
    /// </summary>
    public IRange DataRange
    {
      get
      {
        IRange result =
          m_range.Cells[ 0 ].DataValidation.DataRange;

        if( result == null )
          return null;

        if( this.FirstFormula == m_range.Cells[ 0 ].DataValidation.FirstFormula )
        {
          return result;
        }

        return null;
      }
      set
      {
        IterateDVs( SetDataRange, value );
      }
    }
    /// <summary>
    /// Application object for this object.
    /// </summary>
    public IApplication Application
    {
      get
      {
        return m_range.Application;
      }
    }
    /// <summary>
    /// Parent object for this object.
    /// </summary>
    public object Parent
    {
      get
      {
        return m_range;
      }
    }
    #endregion

    #region IInternalDataValidation
    /// <summary>
    /// 
    /// </summary>
    public Ptg[] FirstFormulaTokens
    {
      get
      {
        throw new NotImplementedException();
        //return m_dataValidation.FirstFormulaTokens;
      }
      set
      {
        IterateDVs( SetFirstFormulaTokens, value );
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public Ptg[] SecondFormulaTokens
    {
      get
      {
        throw new NotImplementedException();
        //return m_dataValidation.SecondFormulaTokens;
      }
      set
      {
        IterateDVs( SetSecondFormulaTokens, value );
      }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Copies data validation settings from the first cell to the rest of the range.
    /// </summary>
    private void CopyFromFirstCell()
    {
      ICombinedRange range = m_range as ICombinedRange;
      WorksheetImpl sheet = m_range.Worksheet as WorksheetImpl;
      DataValidationWrapper wrapper = sheet[ m_range.Row, m_range.Column ].DataValidation as DataValidationWrapper;
      DataValidationImpl dataValidation = wrapper.Wrapped;
      DataValidationTable dvTable = sheet.InnerDVTable;
      Rectangle[] rectangles = range.GetRectangles();
      // Step 1. Clear all data validation objects.
      if( range.Row != range.LastRow || range.Column != range.LastColumn )
      {
        dvTable.Remove( rectangles );
      }

      // Step 2. Copy from the first one.
      //dataValidation.RemoveRange( rectangles );
      dataValidation.AddRange( range );
      // Step 3. If dataValidation got removed in the Step 1, we have to re-add it.
      if( dvTable.FindDataValidation( range.Row, range.Column ) == null )
      {
        dataValidation.ParentCollection.Add( dataValidation );
      }
      //if( dvTable.Cont
      //sheet.InnerDVTable;
    }
    /// <summary>
    /// This method should be called before several updates to the object will take place.
    /// </summary>
    public override void BeginUpdate()
    {
      // Default mode.
      if( BeginCallsCount == 0 )
      {
        IterateCells( BeginUpdate );
      }

      base.BeginUpdate();
      //WorksheetImpl sheet = m_range.Worksheet as WorksheetImpl;
      //ICombinedRange range = m_range as ICombinedRange;
      //DataValidationTable dvTable = sheet.InnerDVTable;
      //dvTable.Remove( range.GetRectangles() );
    }
    /// <summary>
    /// This method should be called after several updates to the object took place.
    /// </summary>
    public override void EndUpdate()
    {
      //IterateCells( EndUpdate );

      base.EndUpdate();

      if( BeginCallsCount == 0 )
      {
        for( int i = 0, len = m_arrValidationList.Count; i < len; i++ )
        {
          m_arrValidationList[ i ].EndUpdate();
        }

        m_arrValidationList.Clear();
      }
    }
    /// <summary>
    /// Iterates through all cells.
    /// </summary>
    /// <param name="method">Method to call for each cell.</param>
    private void IterateCells( CellMethod method )
    {
      Rectangle[] rectangles = ( m_range as ICombinedRange ).GetRectangles();

      for( int i = 0, len = rectangles.Length; i < len; i++ )
      {
        Rectangle rect = rectangles[ i ];
        IterateRectangle( rect, method );
      }
    }
    private void IterateRectangle( Rectangle rect, CellMethod method )
    {
      int iTop = rect.Top + 1;
      int iBottom = rect.Bottom + 1;
      int iLeft = rect.Left + 1;
      int iRight = rect.Right + 1;

      if( rect.Height >= rect.Width )
      {
        for( int iColumn = iLeft; iColumn <= iRight; iColumn++ )
        {
          for( int iRow = iTop; iRow <= iBottom; iRow++ )
          {
            method( iRow, iColumn );
          }
        }
      }
      else
      {
        for( int iRow = iTop; iRow <= iBottom; iRow++ )
        {
          for( int iColumn = iLeft; iColumn <= iRight; iColumn++ )
          {
            method( iRow, iColumn );
          }
        }
      }
    }
    private void BeginUpdate( int row, int column )
    {
      IDataValidation validation = m_range.Worksheet[ row, column ].DataValidation;
      validation.BeginUpdate();
      m_arrValidationList.Add( validation );
    }
    private void EndUpdate( int row, int column )
    {
      //m_range.Worksheet[ row, column ].DataValidation.EndUpdate();
    }
    /// <summary>
    /// Iterates through all data validations using cells.
    /// </summary>
    /// <param name="method">Method to call for each data validation.</param>
    /// <param name="value">Value to pass to the data validation method.</param>
    private void IterateCells( DVMethod method, object value )
    {
      IRange[] cells = m_range.Cells;

      for( int i = 0, iLen = cells.Length; i < iLen; i++ )
      {
        IRange cell = cells[ i ];
        method( cell.DataValidation, value );
      }
    }
    /// <summary>
    /// Iterates through all cached data validation objects.
    /// </summary>
    /// <param name="method">Method to call for each data validation.</param>
    /// <param name="value">Value to pass to the data validation method.</param>
    private void IterateDVs( DVMethod method, object value )
    {
      BeginUpdate();
      for( int i = 0, iLen = m_arrValidationList.Count; i < iLen; i++ )
      {
        IDataValidation dv = m_arrValidationList[ i ];
        method( dv, value );
      }
      EndUpdate();
    }
    /// <summary>
    /// Sets PromptBoxTitle in the specified data validation.
    /// </summary>
    /// <param name="dv">Data validation to set value for.</param>
    /// <param name="value">Value to set.</param>
    private void SetPromptBoxTitle( IDataValidation dv, object value )
    {
      dv.PromptBoxTitle = value as string;
    }
    /// <summary>
    /// Sets PromptBoxText in the specified data validation.
    /// </summary>
    /// <param name="dv">Data validation to set value for.</param>
    /// <param name="value">Value to set.</param>
    private void SetPromptBoxText( IDataValidation dv, object value )
    {
      dv.PromptBoxText = value as string;
    }
    /// <summary>
    /// Sets ErrorBoxTitle in the specified data validation.
    /// </summary>
    /// <param name="dv">Data validation to set value for.</param>
    /// <param name="value">Value to set.</param>
    private void SetErrorBoxTitle( IDataValidation dv, object value )
    {
      dv.ErrorBoxTitle = value as string;
    }
    /// <summary>
    /// Sets ErrorBoxText in the specified data validation.
    /// </summary>
    /// <param name="dv">Data validation to set value for.</param>
    /// <param name="value">Value to set.</param>
    private void SetErrorBoxText( IDataValidation dv, object value )
    {
      dv.ErrorBoxText = value as string;
    }
    /// <summary>
    /// Sets FirstFormula in the specified data validation.
    /// </summary>
    /// <param name="dv">Data validation to set value for.</param>
    /// <param name="value">Value to set.</param>
    private void SetFirstFormula( IDataValidation dv, object value )
    {
      dv.FirstFormula = value as string;
    }
    /// <summary>
    /// Sets FirstDateTime in the specified data validation.
    /// </summary>
    /// <param name="dv">Data validation to set value for.</param>
    /// <param name="value">Value to set.</param>
    private void SetFirstDateTime( IDataValidation dv, object value )
    {
      dv.FirstDateTime = ( DateTime )value;
    }
    /// <summary>
    /// Sets SecondFormula in the specified data validation.
    /// </summary>
    /// <param name="dv">Data validation to set value for.</param>
    /// <param name="value">Value to set.</param>
    private void SetSecondFormula( IDataValidation dv, object value )
    {
      dv.SecondFormula = value as string;
    }
    /// <summary>
    /// Sets SecondDateTime in the specified data validation.
    /// </summary>
    /// <param name="dv">Data validation to set value for.</param>
    /// <param name="value">Value to set.</param>
    private void SetSecondDateTime( IDataValidation dv, object value )
    {
      dv.SecondDateTime = ( DateTime )value;
    }
    /// <summary>
    /// Sets AllowType in the specified data validation.
    /// </summary>
    /// <param name="dv">Data validation to set value for.</param>
    /// <param name="value">Value to set.</param>
    private void SetAllowType( IDataValidation dv, object value )
    {
      dv.AllowType = ( ExcelDataType )value;
    }
    /// <summary>
    /// Sets CompareOperator in the specified data validation.
    /// </summary>
    /// <param name="dv">Data validation to set value for.</param>
    /// <param name="value">Value to set.</param>
    private void SetCompareOperator( IDataValidation dv, object value )
    {
      dv.CompareOperator = ( ExcelDataValidationComparisonOperator )value;
    }
    /// <summary>
    /// Sets IsListInFormula in the specified data validation.
    /// </summary>
    /// <param name="dv">Data validation to set value for.</param>
    /// <param name="value">Value to set.</param>
    private void SetIsListInFormula( IDataValidation dv, object value )
    {
      dv.IsListInFormula = ( bool )value;
    }
    /// <summary>
    /// Sets IsEmptyCellAllowed in the specified data validation.
    /// </summary>
    /// <param name="dv">Data validation to set value for.</param>
    /// <param name="value">Value to set.</param>
    private void SetIsEmptyCellAllowed( IDataValidation dv, object value )
    {
      dv.IsEmptyCellAllowed = ( bool )value;
    }
    /// <summary>
    /// Sets IsSuppressDropDownArrow in the specified data validation.
    /// </summary>
    /// <param name="dv">Data validation to set value for.</param>
    /// <param name="value">Value to set.</param>
    private void SetIsSuppressDropDownArrow( IDataValidation dv, object value )
    {
      dv.IsSuppressDropDownArrow = ( bool )value;
    }
    /// <summary>
    /// Sets ShowPromptBox in the specified data validation.
    /// </summary>
    /// <param name="dv">Data validation to set value for.</param>
    /// <param name="value">Value to set.</param>
    private void SetShowPromptBox( IDataValidation dv, object value )
    {
      dv.ShowPromptBox = ( bool )value;
    }
    /// <summary>
    /// Sets ShowErrorBox in the specified data validation.
    /// </summary>
    /// <param name="dv">Data validation to set value for.</param>
    /// <param name="value">Value to set.</param>
    private void SetShowErrorBox( IDataValidation dv, object value )
    {
      dv.ShowErrorBox = ( bool )value;
    }
    /// <summary>
    /// Sets PromptBoxHPosition in the specified data validation.
    /// </summary>
    /// <param name="dv">Data validation to set value for.</param>
    /// <param name="value">Value to set.</param>
    private void SetPromptBoxHPosition( IDataValidation dv, object value )
    {
      dv.PromptBoxHPosition = ( int )value;
    }
    /// <summary>
    /// Sets PromptBoxVPosition in the specified data validation.
    /// </summary>
    /// <param name="dv">Data validation to set value for.</param>
    /// <param name="value">Value to set.</param>
    private void SetPromptBoxVPosition( IDataValidation dv, object value )
    {
      dv.PromptBoxVPosition = ( int )value;
    }
    /// <summary>
    /// Sets IsPromptBoxVisible in the specified data validation.
    /// </summary>
    /// <param name="dv">Data validation to set value for.</param>
    /// <param name="value">Value to set.</param>
    private void SetIsPromptBoxVisible( IDataValidation dv, object value )
    {
      dv.IsPromptBoxVisible = ( bool )value;
    }
    /// <summary>
    /// Sets IsPromptBoxPositionFixed in the specified data validation.
    /// </summary>
    /// <param name="dv">Data validation to set value for.</param>
    /// <param name="value">Value to set.</param>
    private void SetIsPromptBoxPositionFixed( IDataValidation dv, object value )
    {
      dv.IsPromptBoxPositionFixed = ( bool )value;
    }
    /// <summary>
    /// Sets ErrorStyle in the specified data validation.
    /// </summary>
    /// <param name="dv">Data validation to set value for.</param>
    /// <param name="value">Value to set.</param>
    private void SetErrorStyle( IDataValidation dv, object value )
    {
      dv.ErrorStyle = ( ExcelErrorStyle )value;
    }
    /// <summary>
    /// Sets ListOfValues in the specified data validation.
    /// </summary>
    /// <param name="dv">Data validation to set value for.</param>
    /// <param name="value">Value to set.</param>
    private void SetListOfValues( IDataValidation dv, object value )
    {
      dv.ListOfValues = ( string[] )value;
    }
    /// <summary>
    /// Sets DataRange in the specified data validation.
    /// </summary>
    /// <param name="dv">Data validation to set value for.</param>
    /// <param name="value">Value to set.</param>
    private void SetDataRange( IDataValidation dv, object value )
    {
      dv.DataRange = value as IRange;
    }
    /// <summary>
    /// Sets FirstFormulaTokens in the specified data validation.
    /// </summary>
    /// <param name="dv">Data validation to set value for.</param>
    /// <param name="value">Value to set.</param>
    private void SetFirstFormulaTokens( IDataValidation dv, object value )
    {
      ( ( IInternalDataValidation )dv ).FirstFormulaTokens = value as Ptg[];
    }
    /// <summary>
    /// Sets SecondFormulaTokens in the specified data validation.
    /// </summary>
    /// <param name="dv">Data validation to set value for.</param>
    /// <param name="value">Value to set.</param>
    private void SetSecondFormulaTokens( IDataValidation dv, object value )
    {
      ( ( IInternalDataValidation )dv ).SecondFormulaTokens = value as Ptg[];
    }
    /// <summary>
    /// Delegate specified method that receives cell's coordinates to process.
    /// </summary>
    /// <param name="row">One-based row index.</param>
    /// <param name="column">One-based column index.</param>
    private delegate void CellMethod( int row, int column );
    /// <summary>
    /// Method to process single data validation.
    /// </summary>
    /// <param name="dv">Data validation object to process.</param>
    /// <param name="value">Additional value.</param>
    private delegate void DVMethod( IDataValidation dv, object value );
    #endregion
  }
}
