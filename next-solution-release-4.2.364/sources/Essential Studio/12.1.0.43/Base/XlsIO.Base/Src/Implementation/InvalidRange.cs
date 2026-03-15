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
using System.Collections;

#if ( WINRT )
using Windows.UI;
using Syncfusion.XlsIO.Implementation.WINRT;
#endif

#if  SILVERLIGHT 
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.Silverlight;
#elif WP
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.WP;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;


#endif

namespace Syncfusion.XlsIO.Implementation
{
  /// <summary>
  /// Represents range that references invalid worksheet.
  /// </summary>
  class InvalidRange : ICombinedRange
  {
    #region Members
    /// <summary>
    /// Parent object.
    /// </summary>
    private object m_parent;
    /// <summary>
    /// Application object.
    /// </summary>
    private IApplication m_application;
    /// <summary>
    /// First row index.
    /// </summary>
    private int m_iFirstRow;
    /// <summary>
    /// Last row index.
    /// </summary>
    private int m_iLastRow;
    /// <summary>
    /// First column index.
    /// </summary>
    private int m_iFirstColumn;
    /// <summary>
    /// Last column index.
    /// </summary>
    private int m_iLastColumn;
    #endregion

    #region ICombinedRange Members

    public string GetNewAddress( Dictionary<string, string> names, out string strSheetName )
    {
      throw new NotImplementedException();
    }

    public IRange Clone( object parent, Dictionary<string, string> hashNewNames, WorkbookImpl book )
    {
      throw new NotImplementedException();
    }

    public void ClearConditionalFormats()
    {
      throw new NotImplementedException();
    }

    public Rectangle[] GetRectangles()
    {
      throw new NotImplementedException();
    }

    public int GetRectanglesCount()
    {
      throw new NotImplementedException();
    }

    public int CellsCount
    {
      get { throw new NotImplementedException(); }
    }

    public string AddressGlobal2007
    {
      get { throw new NotImplementedException(); }
    }

    #endregion

    #region IRange Members

    public string Address
    {
      get { throw new NotImplementedException(); }
    }

    public string AddressLocal
    {
      get
      {
        return RangeImpl.GetAddressLocal( m_iFirstRow, m_iFirstColumn, m_iLastRow, m_iLastColumn );
        //throw new NotImplementedException();
      }
    }

    public string AddressGlobal
    {
      get { throw new NotImplementedException(); }
    }

    public string AddressR1C1
    {
      get { throw new NotImplementedException(); }
    }

    public string AddressR1C1Local
    {
      get { throw new NotImplementedException(); }
    }

    public bool Boolean
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

    public IBorders Borders
    {
      get { throw new NotImplementedException(); }
    }

    public IRange[] Cells
    {
      get { throw new NotImplementedException(); }
    }

    public int Column
    {
      get { throw new NotImplementedException(); }
    }

    public int ColumnGroupLevel
    {
      get { throw new NotImplementedException(); }
    }

    public double ColumnWidth
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

    public int Count
    {
      get { throw new NotImplementedException(); }
    }

    public DateTime DateTime
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

    public string DisplayText
    {
      get { throw new NotImplementedException(); }
    }

    public IRange End
    {
      get { throw new NotImplementedException(); }
    }

    public IRange EntireColumn
    {
      get { throw new NotImplementedException(); }
    }

    public IRange EntireRow
    {
      get { throw new NotImplementedException(); }
    }

    public string Error
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

    public string Formula
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

    public string FormulaArray
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

    public string FormulaArrayR1C1
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

    public bool FormulaHidden
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

    public DateTime FormulaDateTime
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

    public string FormulaR1C1
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

    public bool FormulaBoolValue
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

    public string FormulaErrorValue
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

    public bool HasDataValidation
    {
      get { throw new NotImplementedException(); }
    }

    public bool HasBoolean
    {
      get { throw new NotImplementedException(); }
    }

    public bool HasDateTime
    {
      get { throw new NotImplementedException(); }
    }

    public bool HasFormula
    {
      get { throw new NotImplementedException(); }
    }

    public bool HasFormulaArray
    {
      get { throw new NotImplementedException(); }
    }

    public bool HasNumber
    {
      get { throw new NotImplementedException(); }
    }

    public bool HasRichText
    {
      get { throw new NotImplementedException(); }
    }

    public bool HasString
    {
      get { throw new NotImplementedException(); }
    }

    public bool HasStyle
    {
      get { throw new NotImplementedException(); }
    }

    public ExcelHAlign HorizontalAlignment
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

    public IHyperLinks Hyperlinks
    {
      get { throw new NotImplementedException(); }
    }

    public int IndentLevel
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

    public bool IsBlank
    {
      get { throw new NotImplementedException(); }
    }

    public bool IsBoolean
    {
      get { throw new NotImplementedException(); }
    }

    public bool IsError
    {
      get { throw new NotImplementedException(); }
    }

    public bool IsGroupedByColumn
    {
      get { throw new NotImplementedException(); }
    }

    public bool IsGroupedByRow
    {
      get { throw new NotImplementedException(); }
    }

    public bool IsInitialized
    {
      get { throw new NotImplementedException(); }
    }

    public int LastColumn
    {
      get { throw new NotImplementedException(); }
    }

    public int LastRow
    {
      get { throw new NotImplementedException(); }
    }

    public double Number
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

    public string NumberFormat
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

    public int Row
    {
      get { throw new NotImplementedException(); }
    }

    public int RowGroupLevel
    {
      get { throw new NotImplementedException(); }
    }

    public double RowHeight
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

    public IRange[] Rows
    {
      get { throw new NotImplementedException(); }
    }

    public IRange[] Columns
    {
      get { throw new NotImplementedException(); }
    }

    public IStyle CellStyle
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

    public string CellStyleName
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

    public string Text
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

    public TimeSpan TimeSpan
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

    public string Value
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

    /// <summary>
    /// Returns the calculated value of a formula using the most current inputs.
    /// </summary>
    public string CalculatedValue
    {
        get
        {
            if (Parent is IWorksheet && ((IWorksheet)Parent).CalcEngine != null)
            {
                string cellRef = Syncfusion.Calculate.RangeInfo.GetAlphaLabel(Column) + Row.ToString();
                return ((IWorksheet)Parent).CalcEngine.PullUpdatedValue(cellRef);
            }
            return null;
        }
    }

    public object Value2
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

    public ExcelVAlign VerticalAlignment
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

    public IWorksheet Worksheet
    {
      get { throw new NotImplementedException(); }
    }

    public IRange this[ int row, int column ]
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

    public IRange this[ int row, int column, int lastRow, int lastColumn ]
    {
      get { throw new NotImplementedException(); }
    }

    public IRange this[ string name ]
    {
      get { throw new NotImplementedException(); }
    }

    public IRange this[ string name, bool IsR1C1Notation ]
    {
      get { throw new NotImplementedException(); }
    }

    public IConditionalFormats ConditionalFormats
    {
      get { throw new NotImplementedException(); }
    }

    public IDataValidation DataValidation
    {
      get { throw new NotImplementedException(); }
    }

    public string FormulaStringValue
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

    public double FormulaNumberValue
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

    public bool HasFormulaBoolValue
    {
      get { throw new NotImplementedException(); }
    }

    public bool HasFormulaErrorValue
    {
      get { throw new NotImplementedException(); }
    }

    public bool HasFormulaDateTime
    {
      get { throw new NotImplementedException(); }
    }

    public bool HasFormulaNumberValue
    {
        get { throw new NotImplementedException(); }
    }

    public bool HasFormulaStringValue
    {
        get { throw new NotImplementedException(); }
    }

    public ICommentShape Comment
    {
      get { throw new NotImplementedException(); }
    }

    public IRichTextString RichText
    {
      get { throw new NotImplementedException(); }
    }

    public bool IsMerged
    {
      get { throw new NotImplementedException(); }
    }

    public IRange MergeArea
    {
      get { throw new NotImplementedException(); }
    }

    public bool WrapText
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

    public bool HasExternalFormula
    {
      get { throw new NotImplementedException(); }
    }

    public ExcelIgnoreError IgnoreErrorOptions
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

    public bool? IsStringsPreserved
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

    public BuiltInStyles? BuiltInStyle
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

    public string WorksheetName
    {
      get
      {
        return "#REF";
      }
    }

    public IRange Activate()
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Activates a single cell, scroll to it and activates the corresponding sheet.
    /// To select a range of cells, use the Select method.
    /// </summary>
    /// <param name="scroll">True to scroll to the cell</param>
    /// <returns></returns>
    public IRange Activate(bool scroll)
    {
        throw new NotImplementedException();
    }

    public IRange Group( ExcelGroupBy groupBy )
    {
      throw new NotImplementedException();
    }

    public IRange Group( ExcelGroupBy groupBy, bool bCollapsed )
    {
      throw new NotImplementedException();
    }
    /// <summary>
    /// Creates Subtotal for the corresponding ranges
    /// </summary>
    /// <param name="groupBy">GroupBy</param>
    /// <param name="function">ConsolidationFunction</param>
    /// <param name="totalList">TotalList</param>
    public void SubTotal(int groupBy, ConsolidationFunction function, int[] totalList)
    {
        throw new NotSupportedException();
    }
    /// <summary>
    /// Creates SubTotal for the corresponding Ranges
    /// </summary>
    /// <param name="groupBy">GroupByGroupBy</param>
    /// <param name="function">ConsolidationFunction</param>
    /// <param name="totalList">TotalList</param>
    /// <param name="replace">Replace exisiting SubTotal</param>
    /// <param name="pageBreaks">Insert PageBreaks</param>
    /// <param name="summaryBelowData">SummaryBelowData</param>
    public void SubTotal(int groupBy, ConsolidationFunction function, int[] totalList, bool replace, bool pageBreaks, bool summaryBelowData)
    {
        throw new NotSupportedException();
    }

    public void Merge()
    {
      throw new NotImplementedException();
    }

    public void Merge( bool clearCells )
    {
      throw new NotImplementedException();
    }

    public IRange Ungroup( ExcelGroupBy groupBy )
    {
      throw new NotImplementedException();
    }

    public void UnMerge()
    {
      throw new NotImplementedException();
    }

    public void FreezePanes()
    {
      throw new NotImplementedException();
    }

    public void Clear()
    {
      throw new NotImplementedException();
    }

    public void Clear( bool isClearFormat )
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Clears the Content, formats, comments based on clear option.
    /// </summary>
    /// <param name="option"></param>
    public void Clear(ExcelClearOptions option)
    {
        throw new NotImplementedException();
    }

    public void Clear( ExcelMoveDirection direction )
    {
      throw new NotImplementedException();
    }

    public void Clear( ExcelMoveDirection direction, ExcelCopyRangeOptions options )
    {
      throw new NotImplementedException();
    }

    public void MoveTo( IRange destination )
    {
      throw new NotImplementedException();
    }

    public IRange CopyTo( IRange destination )
    {
      throw new NotImplementedException();
    }

    public IRange CopyTo( IRange destination, ExcelCopyRangeOptions options )
    {
      throw new NotImplementedException();
    }

    public IRange IntersectWith( IRange range )
    {
      throw new NotImplementedException();
    }

    public IRange MergeWith( IRange range )
    {
      throw new NotImplementedException();
    }

    public void AutofitRows()
    {
      throw new NotImplementedException();
    }

    public void AutofitColumns()
    {
      throw new NotImplementedException();
    }

    public ICommentShape AddComment()
    {
      throw new NotImplementedException();
    }

    public IRange FindFirst( string findValue, ExcelFindType flags )
    {
      throw new NotImplementedException();
    }

    public IRange FindFirst( double findValue, ExcelFindType flags )
    {
      throw new NotImplementedException();
    }

    public IRange FindFirst( bool findValue )
    {
      throw new NotImplementedException();
    }

    public IRange FindFirst( DateTime findValue )
    {
      throw new NotImplementedException();
    }

    public IRange FindFirst( TimeSpan findValue )
    {
      throw new NotImplementedException();
    }

    public IRange[] FindAll( string findValue, ExcelFindType flags )
    {
      throw new NotImplementedException();
    }

    public IRange[] FindAll( double findValue, ExcelFindType flags )
    {
      throw new NotImplementedException();
    }

    public IRange[] FindAll( bool findValue )
    {
      throw new NotImplementedException();
    }

    public IRange[] FindAll( DateTime findValue )
    {
      throw new NotImplementedException();
    }

    public IRange[] FindAll( TimeSpan findValue )
    {
      throw new NotImplementedException();
    }

    public void BorderAround()
    {
      throw new NotImplementedException();
    }

    public void BorderAround( ExcelLineStyle borderLine )
    {
      throw new NotImplementedException();
    }

    public void BorderAround( ExcelLineStyle borderLine, Color borderColor )
    {
      throw new NotImplementedException();
    }

    public void BorderAround( ExcelLineStyle borderLine, ExcelKnownColors borderColor )
    {
      throw new NotImplementedException();
    }

    public void BorderInside()
    {
      throw new NotImplementedException();
    }

    public void BorderInside( ExcelLineStyle borderLine )
    {
      throw new NotImplementedException();
    }

    public void BorderInside( ExcelLineStyle borderLine, Color borderColor )
    {
      throw new NotImplementedException();
    }

    public void BorderInside( ExcelLineStyle borderLine, ExcelKnownColors borderColor )
    {
      throw new NotImplementedException();
    }

    public void BorderNone()
    {
      throw new NotImplementedException();
    }

    public void CollapseGroup( ExcelGroupBy groupBy )
    {
      throw new NotImplementedException();
    }

    public void ExpandGroup( ExcelGroupBy groupBy )
    {
      throw new NotImplementedException();
    }

    public void ExpandGroup( ExcelGroupBy groupBy, ExpandCollapseFlags flags )
    {
      throw new NotImplementedException();
    }

    #endregion

    #region IEnumerable Members

      public IEnumerator GetEnumerator()
      {
          throw new NotImplementedException();
      }

      #endregion

    #region IParentApplication Members

    public IApplication Application
    {
      get
      {
        return m_application;
      }
    }

    public object Parent
    {
      get
      {
        return m_parent;
      }
    }

    #endregion

    #region Methods
    public InvalidRange( object parent, IRange range )
    {
      m_parent = parent;
      m_application = range.Application;
      m_iFirstColumn = range.Column;
      m_iFirstRow = range.Row;
      m_iLastRow = range.LastRow;
      m_iLastColumn = range.LastColumn;
    }
    #endregion
  }
}
