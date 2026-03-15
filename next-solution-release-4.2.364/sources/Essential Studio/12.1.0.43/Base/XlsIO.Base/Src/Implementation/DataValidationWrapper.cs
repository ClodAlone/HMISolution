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

using Syncfusion.XlsIO.Implementation.Collections;
using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser.Biff_Records.Formula;
using Syncfusion.XlsIO.Interfaces;

namespace Syncfusion.XlsIO.Implementation
{
  /// <summary>
  /// Wrapper over DatavalidationImpl.
  /// </summary>
  public class DataValidationWrapper :
    CommonWrapper,
    IInternalDataValidation
  {
    #region Class members
    /// <summary>
    ///
    /// </summary>
    private DataValidationImpl m_dataValidation;
    /// <summary>
    /// Parent range.
    /// </summary>
    private RangeImpl m_range;
    /// <summary>
    /// Old DataValidationImpl. Should be not null between OnBeforeWrapperChange
    /// and OnAfterWrapperChange.
    /// </summary>
    private DataValidationImpl m_dvOld;
    #endregion

    #region Class internal properties
    /// <summary>
    ///
    /// </summary>
    internal DataValidationImpl Wrapped
    {
      get
      {
        return m_dataValidation;
      }
      set
      {
        if( value != m_dataValidation )
        {
          m_dataValidation = value;
        }
      }
    }
    #endregion

    #region Class initialize/finilize methods
    /// <summary>
    ///
    /// </summary>
    public DataValidationWrapper( RangeImpl range, DataValidationImpl wrap )
    {
      if( wrap == null )
      {
        WorksheetImpl worksheet = range.InnerWorksheet;
        DValRecord dval = ( DValRecord )BiffRecordFactory.GetRecord( TBIFFRecord.DVal );
        DataValidationCollection dvCollection = worksheet.DVTable.Add( dval );
        DVRecord dv = ( DVRecord )BiffRecordFactory.GetRecord( TBIFFRecord.DV );
        wrap = dvCollection.AddDVRecord( dv );
        wrap.AddRange( range );
      }

      m_dataValidation = wrap;
      m_range = range;
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
        return m_dataValidation.PromptBoxTitle;
      }
      set
      {
        if( PromptBoxTitle != value )
        {
          BeginUpdate();
          m_dataValidation.PromptBoxTitle = value;
          EndUpdate();
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
        return m_dataValidation.PromptBoxText;
      }
      set
      {
        if( PromptBoxText != value )
        {
          BeginUpdate();
          m_dataValidation.PromptBoxText = value;
          EndUpdate();
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
        return m_dataValidation.ErrorBoxTitle;
      }
      set
      {
        if( ErrorBoxTitle != value )
        {
          BeginUpdate();
          m_dataValidation.ErrorBoxTitle = value;
          EndUpdate();
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
        return m_dataValidation.ErrorBoxText;
      }
      set
      {
        if( ErrorBoxText != value )
        {
          BeginUpdate();
          m_dataValidation.ErrorBoxText = value;
          EndUpdate();
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
        WorkbookImpl book = m_dataValidation.Workbook;
        FormulaUtil formulaUtil = book.FormulaUtil;

        return formulaUtil.ParsePtgArray( FirstFormulaTokens, m_range.Row, m_range.Column, false, false );
        //return m_dataValidation.FirstFormulaTokens;
      }
      set
      {
        if( FirstFormula != value )
        {
          BeginUpdate();

          //WorkbookImpl book = m_dataValidation.Workbook;
          //FormulaUtil formulaUtil = book.FormulaUtil;

          //if( value != null && value.Length > 0 && value[ 0 ] == '=' )
          //  value = UtilityMethods.RemoveFirstCharUnsafe( value );
          //Ptg[] tokens = formulaUtil.ParseString( value );
          //tokens = formulaUtil.ConvertTokensToShared( tokens, m_range.Row, m_range.Column, book );
          WorksheetImpl sheetImpl = m_range.Worksheet as WorksheetImpl;
          m_dataValidation.DVRecord.FirstFormulaTokens = DataValidationImpl.GetFormulaPtg(ref value, null, sheetImpl, m_range.Row-1, m_range.Column-1);
          m_dataValidation.FirstFormula = value;
          //m_dataValidation.FirstFormulaTokens = tokens;
          EndUpdate();
        }
      }
    }
    /// <summary>
    /// First formula's DateTime value.
    /// </summary>
    public DateTime FirstDateTime
    {
      get
      {
        return m_dataValidation.FirstDateTime;
      }
      set
      {
        if( FirstDateTime != value || FirstDateTime == DateTime.MinValue )
        {
          BeginUpdate();
          m_dataValidation.FirstDateTime = value;
          EndUpdate();
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public string SecondFormula
    {
      get
      {
          WorkbookImpl book = m_dataValidation.Workbook;
          FormulaUtil formulaUtil = book.FormulaUtil;

          return formulaUtil.ParsePtgArray(SecondFormulaTokens, m_range.Row, m_range.Column, false, false);
      }
      set
      {
        if( SecondFormula != value )
        {
          BeginUpdate();

          //WorkbookImpl book = m_dataValidation.Workbook;
          //FormulaUtil formulaUtil = book.FormulaUtil;
          //Ptg[] tokens = formulaUtil.ParseString( value );
          //tokens = formulaUtil.ConvertTokensToShared( tokens, m_range.Row, m_range.Column, book );
          WorksheetImpl sheetImpl = m_range.Worksheet as WorksheetImpl;
          m_dataValidation.DVRecord.SecondFormulaTokens = DataValidationImpl.GetFormulaPtg(ref value, null, sheetImpl, m_range.Row-1, m_range.Column-1);
          m_dataValidation.SecondFormula = value;
          //m_dataValidation.SecondFormulaTokens = tokens;
          EndUpdate();
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
        return m_dataValidation.SecondDateTime;
      }
      set
      {
        if( SecondDateTime != value || FirstDateTime == DateTime.MinValue )
        {
          BeginUpdate();
          m_dataValidation.SecondDateTime = value;
          EndUpdate();
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public ExcelDataType AllowType
    {
      get
      {
        // TODO: Add DataValidationWrapper.AllowType getter implementation.
        return m_dataValidation.AllowType;
      }
      set
      {
        if( AllowType != value )
        {
          BeginUpdate();
          m_dataValidation.AllowType = value;

          if( m_dataValidation.IsListInFormula && value != ExcelDataType.User )
            m_dataValidation.IsListInFormula = false;

          EndUpdate();
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public ExcelDataValidationComparisonOperator CompareOperator
    {
      get
      {
        return m_dataValidation.CompareOperator;
      }
      set
      {
        if( CompareOperator != value )
        {
          BeginUpdate();
          m_dataValidation.CompareOperator = value;
          EndUpdate();
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public bool IsListInFormula
    {
      get
      {
        return m_dataValidation.IsListInFormula;
      }
      set
      {
        if( IsListInFormula != value )
        {
          BeginUpdate();
          m_dataValidation.IsListInFormula = value;
          EndUpdate();
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public bool IsEmptyCellAllowed
    {
      get
      {
        return m_dataValidation.IsEmptyCellAllowed;
      }
      set
      {
        if( IsEmptyCellAllowed != value )
        {
          BeginUpdate();
          m_dataValidation.IsEmptyCellAllowed = value;
          EndUpdate();
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public bool IsSuppressDropDownArrow
    {
      get
      {
        return m_dataValidation.IsSuppressDropDownArrow;
      }
      set
      {
        if( IsSuppressDropDownArrow != value )
        {
          BeginUpdate();
          m_dataValidation.IsSuppressDropDownArrow = value;
          EndUpdate();
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public bool ShowPromptBox
    {
      get
      {
        return m_dataValidation.ShowPromptBox;
      }
      set
      {
        if( ShowPromptBox != value )
        {
          BeginUpdate();
          m_dataValidation.ShowPromptBox = value;
          EndUpdate();
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public bool ShowErrorBox
    {
      get
      {
        return m_dataValidation.ShowErrorBox;
      }
      set
      {
        if( ShowErrorBox != value )
        {
          BeginUpdate();
          m_dataValidation.ShowErrorBox = value;
          EndUpdate();
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public int PromptBoxHPosition
    {
      get
      {
        return m_dataValidation.PromptBoxHPosition;
      }
      set
      {
        if( PromptBoxHPosition != value )
        {
          OnBeforeCollectionChange();
          m_dataValidation.PromptBoxHPosition = value;
          OnAfterCollectionChange();
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public int PromptBoxVPosition
    {
      get
      {
        return m_dataValidation.PromptBoxVPosition;
      }
      set
      {
        if( PromptBoxVPosition != value )
        {
          OnBeforeCollectionChange();
          m_dataValidation.PromptBoxVPosition = value;
          OnAfterCollectionChange();
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public bool IsPromptBoxVisible
    {
      get
      {
        return m_dataValidation.IsPromptBoxVisible;
      }
      set
      {
        if( IsPromptBoxVisible != value )
        {
          OnBeforeCollectionChange();
          m_dataValidation.IsPromptBoxVisible = value;
          OnAfterCollectionChange();
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public bool IsPromptBoxPositionFixed
    {
      get
      {
        return m_dataValidation.IsPromptBoxPositionFixed;
      }
      set
      {
        if( IsPromptBoxPositionFixed != value )
        {
          OnBeforeCollectionChange();
          m_dataValidation.IsPromptBoxPositionFixed = value;
          OnAfterCollectionChange();
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public ExcelErrorStyle ErrorStyle
    {
      get
      {
        return m_dataValidation.ErrorStyle;
      }
      set
      {
        if( ErrorStyle != value )
        {
          BeginUpdate();
          m_dataValidation.ErrorStyle = value;
          EndUpdate();
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public string[] ListOfValues
    {
      get
      {
        return m_dataValidation.ListOfValues;
      }
      set
      {
        BeginUpdate();
        m_dataValidation.ListOfValues = value;
        EndUpdate();
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public IRange DataRange
    {
      get
      {
        return m_dataValidation.DataRange;
      }
      set
      {
        if( DataRange != value )
        {
          BeginUpdate();
          m_dataValidation.DataRange = value;
          EndUpdate();
        }
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
        return m_dataValidation.FirstFormulaTokens;
      }
      set
      {
        if( FirstFormulaTokens != value )
        {
          BeginUpdate();
          m_dataValidation.FirstFormulaTokens = value;
          EndUpdate();
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public Ptg[] SecondFormulaTokens
    {
      get
      {
        return m_dataValidation.SecondFormulaTokens;
      }
      set
      {
        if( SecondFormulaTokens != value )
        {
          BeginUpdate();
          m_dataValidation.SecondFormulaTokens = value;
          EndUpdate();
        }
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
        return m_dataValidation.Application;
      }
    }
    /// <summary>
    /// Parent object for this object.
    /// </summary>
    public object Parent
    {
      get
      {
        return m_dataValidation.Parent;
      }
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// 
    /// </summary>
    public override void BeginUpdate()
    {
      // If only this range refers to that DataValidationImpl, we can freely change it.
      // Any other way, we have to create new and change it.
      if( BeginCallsCount == 0 )
      {
        DataValidationCollection parentCol = m_dataValidation.ParentCollection;

        DataValidationImpl newDV = new DataValidationImpl( parentCol, m_dataValidation.DVRecord );

        newDV.AddRange( m_range );
        m_dvOld = m_dataValidation;
        m_dataValidation = newDV;
      }

      base.BeginUpdate();
    }
    /// <summary>
    /// 
    /// </summary>
    public override void EndUpdate()
    {
      base.EndUpdate();

      if( BeginCallsCount == 0 )
      {
        m_dataValidation = m_dataValidation.ParentCollection.Add( m_dataValidation );

        if( m_dvOld != null && m_dvOld != m_dataValidation )
        {
          m_dvOld.RemoveRange( m_range );
        }

        m_dvOld = null;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnBeforeCollectionChange()
    {
      //      DataValidationCollection parentCol = m_dataValidation.ParentCollection;
      //      DataValidationTable parentTable = parentCol.ParentTable;
      //
      //      //if( parentTable.Count > 1 )
      //      {
      //        DataValidationCollection newDVCol = new DataValidationCollection( Application,
      //          parentTable, parentCol.Record );
      //
      //        DataValidationImpl newDV = new DataValidationImpl( Application, newDVCol,
      //          m_dataValidation.DVRecord );
      //
      //        newDV.AddRange( m_range );
      //        m_dvOld = m_dataValidation;
      //        m_dataValidation = newDV;
      //      }
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnAfterCollectionChange()
    {
      //      if( m_dvOld != null )
      //      {
      //        m_dvOld.RemoveRange( m_range );
      //
      //        DataValidationCollection oldParentCol = m_dvOld.ParentCollection;
      //        DataValidationCollection newParentCol = m_dataValidation.ParentCollection;
      //        DataValidationTable parentTable = newParentCol.ParentTable;
      //
      //        newParentCol = parentTable.Add( newParentCol );
      //        DataValidationImpl newDV = newParentCol.Add( m_dataValidation );
      //
      //        m_dataValidation = newDV;
      //        m_dataValidation.ParentCollection = newParentCol;
      //
      //        m_dvOld = null;
      //      }
    }
    #endregion
  }
}
