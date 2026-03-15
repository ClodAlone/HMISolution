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
using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Parser.Biff_Records.Formula;

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
  class ExternalRange :
    IRange,
    INativePTG,
    ICombinedRange
  {
    #region Members
    /// <summary>
    /// Parent worksheet.
    /// </summary>
    private ExternWorksheetImpl m_sheet;
    private int m_iFirstRow;
    private int m_iFirstColumn;
    private int m_iLastRow;
    private int m_iLastColumn;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bIsNumReference;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bIsMultiReference;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bIsStringReference;
    #endregion

    #region IRange Members

    public string Address
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public string AddressLocal
    {
      get
      {
        return RangeImpl.GetAddressLocal( m_iFirstRow, m_iFirstColumn, m_iLastRow, m_iLastColumn );
      }
    }

    public string AddressGlobal
    {
      get
      {
        string cellAddress = RangeImpl.GetAddressLocal( m_iFirstRow, m_iFirstColumn, m_iLastRow, m_iLastColumn );
        return string.Format( "[{0}]{1}!{2}", m_sheet.Workbook.Index + 1, m_sheet.Name, cellAddress );
      }
    }

    public string AddressR1C1
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public string AddressR1C1Local
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public bool Boolean
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
      set
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public IBorders Borders
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public IRange[] Cells
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public int Column
    {
      get
      {
        return m_iFirstColumn;
      }
    }

    public int ColumnGroupLevel
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public double ColumnWidth
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
      set
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public int Count
    {
      get
      {
        return ( m_iLastColumn - m_iFirstColumn + 1 ) * ( m_iLastRow - m_iFirstRow + 1 );
      }
    }

    public DateTime DateTime
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
      set
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public string DisplayText
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public IRange End
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public IRange EntireColumn
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public IRange EntireRow
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public string Error
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
      set
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public string Formula
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
      set
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public string FormulaArray
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
      set
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public string FormulaArrayR1C1
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
      set
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public bool FormulaHidden
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
      set
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public DateTime FormulaDateTime
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
      set
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public string FormulaR1C1
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
      set
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public bool FormulaBoolValue
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
      set
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public string FormulaErrorValue
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
      set
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public bool HasDataValidation
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public bool HasBoolean
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public bool HasDateTime
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public bool HasFormula
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public bool HasFormulaArray
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public bool HasNumber
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public bool HasRichText
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public bool HasString
    {
      get
      {
        return m_sheet.CellRecords.GetCellType( Row, Column ) == WorksheetImpl.TRangeValueType.String;
      }
    }

    public bool HasStyle
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public ExcelHAlign HorizontalAlignment
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
      set
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public IHyperLinks Hyperlinks
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public int IndentLevel
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
      set
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public bool IsBlank
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public bool IsBoolean
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public bool IsError
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public bool IsGroupedByColumn
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public bool IsGroupedByRow
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public bool IsInitialized
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public int LastColumn
    {
      get
      {
        return m_iLastColumn;
      }
    }

    public int LastRow
    {
      get
      {
        return m_iLastRow;
      }
    }

    public double Number
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
      set
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public string NumberFormat
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
      set
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public int Row
    {
      get
      {
        return m_iFirstRow;
      }
    }

    public int RowGroupLevel
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public double RowHeight
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
      set
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public IRange[] Rows
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public IRange[] Columns
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public IStyle CellStyle
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
      set
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public string CellStyleName
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
      set
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public string Text
    {
      get
      {
        //throw new Exception( "The method or operation is not implemented." );
        long index = RangeImpl.GetCellIndex( Column, Row );
        return m_sheet.CellRecords.GetText( index );
      }
      set
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public TimeSpan TimeSpan
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
      set
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public string Value
    {
        get
        {
            long index = RangeImpl.GetCellIndex(m_iFirstRow, m_iFirstColumn);
            return IsSingleCell ?
              m_sheet.CellRecords.GetText(index) :
              null;
            //throw new Exception( "The method or operation is not implemented." );
        }
        set
        {
            throw new Exception("The method or operation is not implemented.");
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
        throw new Exception( "The method or operation is not implemented." );
      }
      set
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public ExcelVAlign VerticalAlignment
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
      set
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public IWorksheet Worksheet
    {
      get
      {
        return m_sheet;
      }
    }

    public IRange this[ int row, int column ]
    {
      get
      {
        return new ExternalRange( m_sheet, row, column );
      }
      set
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public IRange this[ int row, int column, int lastRow, int lastColumn ]
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public IRange this[ string name ]
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public IRange this[ string name, bool IsR1C1Notation ]
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public IConditionalFormats ConditionalFormats
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public IDataValidation DataValidation
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public string FormulaStringValue
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
      set
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public double FormulaNumberValue
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
      set
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public bool HasFormulaBoolValue
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public bool HasFormulaErrorValue
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public bool HasFormulaDateTime
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public bool HasFormulaNumberValue
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public bool HasFormulaStringValue
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public ICommentShape Comment
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public IRichTextString RichText
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public bool IsMerged
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public IRange MergeArea
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public bool WrapText
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
      set
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public bool HasExternalFormula
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public ExcelIgnoreError IgnoreErrorOptions
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
      set
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public bool? IsStringsPreserved
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
      set
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public BuiltInStyles? BuiltInStyle
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
      set
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public IRange Activate()
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    public IRange Activate(bool scroll)
    {
        throw new Exception("The method or operation is not implemented.");
    }

    public IRange Group( ExcelGroupBy groupBy )
    {
      throw new Exception( "The method or operation is not implemented." );
    }

    public IRange Group( ExcelGroupBy groupBy, bool bCollapsed )
    {
      throw new Exception( "The method or operation is not implemented." );
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
      throw new Exception( "The method or operation is not implemented." );
    }

    public void Merge( bool clearCells )
    {
      throw new Exception( "The method or operation is not implemented." );
    }

    public IRange Ungroup( ExcelGroupBy groupBy )
    {
      throw new Exception( "The method or operation is not implemented." );
    }

    public void UnMerge()
    {
      throw new Exception( "The method or operation is not implemented." );
    }

    public void FreezePanes()
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    public void Clear(ExcelClearOptions option)
    {
        throw new Exception("The method or operation is not implemented.");
    }

    public void Clear()
    {
      throw new Exception( "The method or operation is not implemented." );
    }

    public void Clear( bool isClearFormat )
    {
      throw new Exception( "The method or operation is not implemented." );
    }

    public void Clear( ExcelMoveDirection direction )
    {
      throw new Exception( "The method or operation is not implemented." );
    }

    public void Clear( ExcelMoveDirection direction, ExcelCopyRangeOptions options )
    {
      throw new Exception( "The method or operation is not implemented." );
    }

    public void MoveTo( IRange destination )
    {
      throw new Exception( "The method or operation is not implemented." );
    }

    public IRange CopyTo( IRange destination )
    {
      throw new Exception( "The method or operation is not implemented." );
    }

    public IRange CopyTo( IRange destination, ExcelCopyRangeOptions options )
    {
      throw new Exception( "The method or operation is not implemented." );
    }

    public IRange IntersectWith( IRange range )
    {
      throw new Exception( "The method or operation is not implemented." );
    }

    public IRange MergeWith( IRange range )
    {
      throw new Exception( "The method or operation is not implemented." );
    }

    public void AutofitRows()
    {
      throw new Exception( "The method or operation is not implemented." );
    }

    public void AutofitColumns()
    {
      throw new Exception( "The method or operation is not implemented." );
    }

    public ICommentShape AddComment()
    {
      throw new Exception( "The method or operation is not implemented." );
    }

    public IRange FindFirst( string findValue, ExcelFindType flags )
    {
      throw new Exception( "The method or operation is not implemented." );
    }

    public IRange FindFirst( double findValue, ExcelFindType flags )
    {
      throw new Exception( "The method or operation is not implemented." );
    }

    public IRange FindFirst( bool findValue )
    {
      throw new Exception( "The method or operation is not implemented." );
    }

    public IRange FindFirst( DateTime findValue )
    {
      throw new Exception( "The method or operation is not implemented." );
    }

    public IRange FindFirst( TimeSpan findValue )
    {
      throw new Exception( "The method or operation is not implemented." );
    }

    public IRange[] FindAll( string findValue, ExcelFindType flags )
    {
      throw new Exception( "The method or operation is not implemented." );
    }

    public IRange[] FindAll( double findValue, ExcelFindType flags )
    {
      throw new Exception( "The method or operation is not implemented." );
    }

    public IRange[] FindAll( bool findValue )
    {
      throw new Exception( "The method or operation is not implemented." );
    }

    public IRange[] FindAll( DateTime findValue )
    {
      throw new Exception( "The method or operation is not implemented." );
    }

    public IRange[] FindAll( TimeSpan findValue )
    {
      throw new Exception( "The method or operation is not implemented." );
    }

    public void BorderAround()
    {
      throw new Exception( "The method or operation is not implemented." );
    }

    public void BorderAround( ExcelLineStyle borderLine )
    {
      throw new Exception( "The method or operation is not implemented." );
    }

    public void BorderAround( ExcelLineStyle borderLine, Color borderColor )
    {
      throw new Exception( "The method or operation is not implemented." );
    }

    public void BorderAround( ExcelLineStyle borderLine, ExcelKnownColors borderColor )
    {
      throw new Exception( "The method or operation is not implemented." );
    }

    public void BorderInside()
    {
      throw new Exception( "The method or operation is not implemented." );
    }

    public void BorderInside( ExcelLineStyle borderLine )
    {
      throw new Exception( "The method or operation is not implemented." );
    }

    public void BorderInside( ExcelLineStyle borderLine, Color borderColor )
    {
      throw new Exception( "The method or operation is not implemented." );
    }

    public void BorderInside( ExcelLineStyle borderLine, ExcelKnownColors borderColor )
    {
      throw new Exception( "The method or operation is not implemented." );
    }

    public void BorderNone()
    {
      throw new Exception( "The method or operation is not implemented." );
    }

    public void CollapseGroup( ExcelGroupBy groupBy )
    {
      throw new Exception( "The method or operation is not implemented." );
    }

    public void ExpandGroup( ExcelGroupBy groupBy )
    {
      throw new Exception( "The method or operation is not implemented." );
    }

    public void ExpandGroup( ExcelGroupBy groupBy, ExpandCollapseFlags flags )
    {
      throw new Exception( "The method or operation is not implemented." );
    }

    /// <summary>
    /// Gets address global in the format required by Excel 2007.
    /// </summary>
    public string AddressGlobal2007
    {
      get
      {
        return AddressGlobal;
      }
    }
    #endregion

    #region IParentApplication Members

    public IApplication Application
    {
      get
      {
        return m_sheet.Application;
      }
    }

    public object Parent
    {
      get
      {
        return m_sheet;
      }
    }

    #endregion

    #region Methods
    /// <summary>
    /// Initializes new instance of the class.
    /// </summary>
    /// <param name="sheet">Parent external worksheet.</param>
    /// <param name="row">First row.</param>
    /// <param name="column">First column.</param>
    public ExternalRange( ExternWorksheetImpl sheet, int row, int column )
      : this( sheet, row, column, row, column )
    {
    }
    /// <summary>
    /// Initializes new instance of the class.
    /// </summary>
    /// <param name="sheet">Parent external worksheet.</param>
    /// <param name="row">First row.</param>
    /// <param name="column">First column.</param>
    /// <param name="lastRow">Last row.</param>
    /// <param name="lastColumn">Last column.</param>
    public ExternalRange( ExternWorksheetImpl sheet, int row, int column, int lastRow, int lastColumn )
    {
      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      m_sheet = sheet;
      m_iFirstRow = row;
      m_iFirstColumn = column;
      m_iLastRow = lastRow;
      m_iLastColumn = lastColumn;
    }
    #endregion

    #region INativePTG Members

    public Syncfusion.XlsIO.Parser.Biff_Records.Formula.Ptg[] GetNativePtg()
    {
      int iReferenceIndex = m_sheet.ReferenceIndex;
      Ptg result;

      if( IsSingleCell )
      {
        Ref3DPtg ref3D = ( Ref3DPtg )FormulaUtil.CreatePtg( FormulaToken.tRef3d1 );
        ref3D.RefIndex = ( ushort )iReferenceIndex;
        ref3D.RowIndex = m_iFirstRow - 1;
        ref3D.ColumnIndex = m_iFirstColumn - 1;
        result = ref3D;
      }
      else
      {
        Area3DPtg area3D = ( Area3DPtg )FormulaUtil.CreatePtg( FormulaToken.tArea3d1 );
        area3D.RefIndex = ( ushort )iReferenceIndex;
        area3D.FirstRow = m_iFirstRow - 1;
        area3D.FirstColumn = m_iFirstColumn - 1;
        area3D.LastRow = m_iLastRow - 1;
        area3D.LastColumn = m_iLastColumn - 1;
        result = area3D;
      }

      return new Ptg[] { result };
    }
    public bool IsSingleCell
    {
      get
      {
        return m_iFirstColumn == m_iLastColumn && m_iFirstRow == m_iLastRow;
      }
    }
    #endregion

    #region ICombinedRange Members

    public string GetNewAddress( Dictionary<string, string> names, out string strSheetName )
    {
      throw new Exception( "The method or operation is not implemented." );
    }

    public IRange Clone( object parent, Dictionary<string, string> hashNewNames, WorkbookImpl book )
    {
      int iBookIndex = m_sheet.Workbook.Index;
      ExternWorkbookImpl newBook = book.ExternWorkbooks[ iBookIndex ];
      ExternWorksheetImpl sheet = newBook.Worksheets[ m_sheet.Index ];
      return new ExternalRange( sheet, m_iFirstRow, m_iFirstColumn, m_iLastRow, m_iLastColumn );
    }

    public void ClearConditionalFormats()
    {
      throw new NotSupportedException();
    }

    public Rectangle[] GetRectangles()
    {
      return new Rectangle[]
      {
        Rectangle.FromLTRB( m_iFirstColumn, m_iFirstRow, m_iLastColumn, m_iLastRow )
      };
    }

    public int GetRectanglesCount()
    {
      return 1;
    }

    public int CellsCount
    {
      get
      {
        return Count;
      }
    }
    /// <summary>
    /// Gets name of the parent worksheet.
    /// </summary>
    public string WorksheetName
    {
      get
      {
        return Worksheet.Name;
      }
    }

    #endregion

    #region IEnumerable Members

    public System.Collections.IEnumerator GetEnumerator()
    {
        throw new NotImplementedException();
    }

    #endregion

    #region Class properties
    public ExternWorksheetImpl ExternSheet
    {
      get
      {
        return m_sheet;
      }
    }
    /// <summary>
    /// Gets or sets a value indicating whether this instance is num reference for chart axis.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is num reference; otherwise, <c>false</c>.
    /// </value>
    internal bool IsNumReference
    {
        get
        {
            return m_bIsNumReference;
        }
        set
        {
            m_bIsNumReference = value;
        }
    }
    /// <summary>
    /// Gets or sets a value indicating whether this instance is a string reference for chart axis.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is string reference; otherwise, <c>false</c>.
    /// </value>
    internal bool IsStringReference
    {
        get
        {
            return m_bIsStringReference;
        }
        set
        {
            m_bIsStringReference = value;
        }
    }
    /// <summary>
    /// Gets or sets a value indicating whether this instance is multi reference for chart axis.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is multi reference; otherwise, <c>false</c>.
    /// </value>
    internal bool IsMultiReference
    {
        get
        {
            return m_bIsMultiReference;
        }
        set
        {
            m_bIsMultiReference = value;
        }
    }
    #endregion
  }
}
