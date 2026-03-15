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

#if ( WINRT )
using Windows.UI;
using Rectangle = Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
#endif

#if (SILVERLIGHT)
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
#endregion

namespace Syncfusion.XlsIO.Implementation.Collections.Grouping
{
	/// <summary>
	/// Summary description for RangeGroup.
	/// </summary>
	public class RangeGroup
    : CommonObject
    , IRange
	{
    #region Class members
    /// <summary>
    /// First row.
    /// </summary>
    protected int m_iFirstRow;
    /// <summary>
    /// First column.
    /// </summary>
    protected int m_iFirstColumn;
    /// <summary>
    /// Last row.
    /// </summary>
    protected int m_iLastRow;
    /// <summary>
    /// Last column.
    /// </summary>
    protected int m_iLastColumn;
    /// <summary>
    /// Parent group of worksheets.
    /// </summary>
    private WorksheetGroup m_sheetGroup;
    /// <summary>
    /// Rich text string group.
    /// </summary>
    private RichTextStringGroup m_richText;
    /// <summary>
    /// Value of End property.
    /// </summary>
    private RangeGroup m_rangeEnd;
    /// <summary>
    /// Style group.
    /// </summary>
    protected StyleGroup m_style;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Creates new instance of the group.
    /// </summary>
    /// <param name="application">Application object for the new group.</param>
    /// <param name="parent">Parent object for the new group.</param>
    protected RangeGroup( IApplication application, object parent )
      : base( application, parent )
    {
      FindParents();
    }
    /// <summary>
    /// Creates new instance of the group.
    /// </summary>
    /// <param name="application">Application object for the new group.</param>
    /// <param name="parent">Parent object for the new group.</param>
    /// <param name="iFirstRow">The first row of the range.</param>
    /// <param name="iFirstColumn">The first column of the range.</param>
    public RangeGroup( IApplication application, object parent,
      int iFirstRow, int iFirstColumn )
      : this( application, parent, iFirstRow, iFirstColumn, iFirstRow, iFirstColumn )
    {
    }
    /// <summary>
    /// Creates new instance of the group.
    /// </summary>
    /// <param name="application">Application object for the new group.</param>
    /// <param name="parent">Parent object for the new group.</param>
    /// <param name="iFirstRow">The first row of the range.</param>
    /// <param name="iFirstColumn">The first column of the range.</param>
    /// <param name="iLastRow">The last row of the range.</param>
    /// <param name="iLastColumn">The last column of the range.</param>
    public RangeGroup( IApplication application, object parent,
      int iFirstRow, int iFirstColumn, int iLastRow, int iLastColumn )
      : this( application, parent )
    {
      m_iFirstRow = iFirstRow;
      m_iFirstColumn = iFirstColumn;
      m_iLastRow = iLastRow;
      m_iLastColumn = iLastColumn;
    }
    /// <summary>
    /// Creates new instance of the group.
    /// </summary>
    /// <param name="application">Application object for the new group.</param>
    /// <param name="parent">Parent object for the new group.</param>
    /// <param name="name">String representation of the range.</param>
    public RangeGroup( IApplication application, object parent, string name )
      : this( application, parent, name, false )
    {}
    /// <summary>
    /// Creates new instance of the group.
    /// </summary>
    /// <param name="application">Application object for the new group.</param>
    /// <param name="parent">Parent object for the new group.</param>
    /// <param name="name">String representation of the range.</param>
    /// <param name="IsR1C1Notation">Indicates is name in R1C1 notation.</param>
    public RangeGroup( IApplication application, object parent, string name, bool IsR1C1Notation )
      : this( application, parent )
    {
      WorksheetGroup sheetGroup = ( WorksheetGroup )parent;

      if( sheetGroup.IsEmpty )
        throw new NotSupportedException( "Sheets collection cannot be empty." );

      IRange range = sheetGroup[ 0 ].Range[ name, IsR1C1Notation ];
      m_iFirstRow = range.Row;
      m_iFirstColumn = range.Column;
      m_iLastRow = range.LastRow;
      m_iLastColumn = range.LastColumn;
    }
    /// <summary>
    /// Searches for all necessary parent objects.
    /// </summary>
    private void FindParents()
    {
      m_sheetGroup = FindParent( typeof( WorksheetGroup ) ) as WorksheetGroup;

      if( m_sheetGroup == null )
        throw new ArgumentOutOfRangeException( "parent", "Can't find parent worksheet group." );
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Returns range for specific sheet.
    /// </summary>
    /// <param name="iSheetIndex">Sheet index in the group.</param>
    /// <returns></returns>
    private IRange GetRange( int iSheetIndex )
    {
      IWorksheet sheet = m_sheetGroup[ iSheetIndex ];
      return sheet.Range[ m_iFirstRow, m_iFirstColumn, m_iLastRow, m_iLastColumn ];
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Returns number of ranges in the group. Read-only.
    /// </summary>
    public int Count
    {
      get
      {
        return m_sheetGroup.Count;
      }
    }
    /// <summary>
    /// Returns range from the specified worksheet in the range. Read-only.
    /// </summary>
    public IRange this[ int index ]
    {
      get
      {
        return GetRange( index );
      }
    }
    /// <summary>
    /// Returns parent workbook object. Read-only.
    /// </summary>
    public WorkbookImpl Workbook
    {
      get
      {
        return m_sheetGroup.ParentWorkbook;
      }
    }
    #endregion

    #region IRange Members
    /// <summary>
    /// Returns hyperlinks for this range group.
    /// </summary>
    public IHyperLinks Hyperlinks
    {
      get
      {
        throw new NotImplementedException( "Hyperlinks property" );
      }
    }
    /// <summary>
    /// Returns the range reference in the language of the macro.
    /// Read-only String.
    /// </summary>
    public string Address
    {
      get
      {
        throw new NotSupportedException();
      }
    }

    /// <summary>
    /// Returns the range reference for the specified range in the language
    /// of the user. Read-only String.
    /// </summary>
    public string AddressLocal
    {
      get
      {
        return RangeImpl.GetAddressLocal( m_iFirstRow, m_iFirstColumn, m_iLastRow, m_iLastColumn );
      }
    }

    /// <summary>
    /// Returns range Address in format "'Sheet1'!$A$1".
    /// </summary>
    public string AddressGlobal
    {
      get
      {
        throw new NotSupportedException();
      }
    }

    /// <summary>
    /// Returns the range reference using R1C1 notation.
    /// Read-only String.
    /// </summary>
    public string        AddressR1C1
    {
      get
      {
        throw new NotSupportedException();
      }
    }
    /// <summary>
    /// Returns the range reference using R1C1 notation.
    /// Read-only String.
    /// </summary>
    public string        AddressR1C1Local
    {
      get
      {
        return RangeImpl.GetAddressLocal( m_iFirstRow, m_iFirstColumn, m_iLastRow, m_iLastColumn, true );
      }
    }
    /// <summary>
    /// Gets / sets boolean value that is contained by this range.
    /// </summary>
    public bool Boolean
    {
      get
      {
        if( m_sheetGroup.IsEmpty ) return false;

        bool result = GetRange( 0 ).Boolean;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          bool curValue = GetRange( i ).Boolean;

          if( result != curValue )
          {
            return false;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          GetRange( i ).Boolean = value;
        }
      }
    }

    /// <summary>
    /// Returns a  Borders collection that represents the borders of a style
    /// or a range of cells (including a range defined as part of a
    /// conditional format).
    /// </summary>
    public IBorders Borders
    {
      get
      {
        return CellStyle.Borders;
      }
    }

    /// <summary>
    /// Returns a Range object that represents the cells in the specified range.
    /// Read-only.
    /// </summary>
    public IRange[] Cells
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    /// <summary>
    /// Returns the number of the first column in the first area in the specified
    /// range. Read-only.
    /// </summary>
    public int Column
    {
      get
      {
        return m_iFirstColumn;
      }
    }

    /// <summary>
    /// Column group level. Read-only.
    /// -1 - Not all columns in the range have same group level.
    /// 0 - No grouping,
    /// 1 - 7 - Group level.
    /// </summary>
    public int ColumnGroupLevel
    {
      get
      {
        if( m_sheetGroup.IsEmpty ) return int.MinValue;

        int result = GetRange( 0 ).ColumnGroupLevel;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          int curValue = GetRange( i ).ColumnGroupLevel;

          if( result != curValue )
          {
            return int.MinValue;
          }
        }

        return result;
      }
    }

    /// <summary>
    /// Returns or sets the width of all columns in the specified range.
    /// Read/write Double.
    /// </summary>
    public double ColumnWidth
    {
      get
      {
        if( m_sheetGroup.IsEmpty ) return double.MinValue;

        double result = GetRange( 0 ).ColumnWidth;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          double curValue = GetRange( i ).ColumnWidth;

          if( result != curValue )
          {
            return double.MinValue;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          GetRange( i ).ColumnWidth = value;
        }
      }
    }

    /// <summary>
    /// Returns the number of objects in the collection. Read-only.
    /// </summary>
    int IRange.Count
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    /// <summary>
    /// Gets / sets DateTime contained by this cell. Read-write DateTime.
    /// </summary>
    public DateTime DateTime
    {
      get
      {
        if( m_sheetGroup.IsEmpty ) return DateTime.MinValue;

        DateTime result = GetRange( 0 ).DateTime;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          DateTime curValue = GetRange( i ).DateTime;

          if( result != curValue )
          {
            return DateTime.MinValue;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          GetRange( i ).DateTime = value;
        }
      }
    }

    /// <summary>
    /// Returns cell value after number format application. Read-only.
    /// </summary>
    public string DisplayText
    {
      get
      {
        if( m_sheetGroup.IsEmpty ) return null;

        string result = GetRange( 0 ).DisplayText;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          string curValue = GetRange( i ).DisplayText;

          if( result != curValue )
          {
            return null;
          }
        }

        return result;
      }
    }

    /// <summary>
    /// Returns a Range object that represents the cell at the end of the
    /// region that contains the source range.
    /// </summary>
    public IRange End
    {
      get
      {
        if( m_rangeEnd == null )
        {
          m_rangeEnd = new RangeGroup( Application, this, m_iLastRow, m_iLastColumn );
        }

        return m_rangeEnd;
      }
    }

    /// <summary>
    /// Returns a Range object that represents the entire column (or
    /// columns) that contains the specified range. Read-only.
    /// </summary>
    public IRange EntireColumn
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    /// <summary>
    /// Returns a Range object that represents the entire row (or
    /// rows) that contains the specified range. Read-only.
    /// </summary>
    public IRange EntireRow
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    /// <summary>
    /// Gets / sets error value that is contained by this range.
    /// </summary>
    public string Error
    {
      get
      {
        if( m_sheetGroup.IsEmpty ) return null;

        string result = GetRange( 0 ).Error;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          string curValue = GetRange( i ).Error;

          if( result != curValue )
          {
            return null;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          GetRange( i ).Error = value;
        }
      }
    }

    /// <summary>
    /// Returns or sets the object's formula in A1-style notation and in
    /// the language of the macro. Read/write Variant.
    /// </summary>
    public string Formula
    {
      get
      {
        if( m_sheetGroup.IsEmpty ) return null;

        string result = GetRange( 0 ).Formula;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          string curValue = GetRange( i ).Formula;

          if( result != curValue )
          {
            return null;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          GetRange( i ).Formula = value;
        }
      }
    }
    /// <summary>
    /// Returns or sets the object's formula in R1C1-style notation and in
    /// the language of the macro. Read/write Variant.
    /// </summary>
    public string FormulaR1C1
    {
      get
      {
        if( m_sheetGroup.IsEmpty ) return null;

        string result = GetRange( 0 ).FormulaR1C1;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          string curValue = GetRange( i ).FormulaR1C1;

          if( result != curValue )
          {
            return null;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          GetRange( i ).FormulaR1C1 = value;
        }
      }
    }
    /// <summary>
    /// Represents array-entered formula.
    /// </summary>
    public string FormulaArray
    {
      get
      {
        if( m_sheetGroup.IsEmpty ) return null;

        string result = GetRange( 0 ).FormulaArray;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          string curValue = GetRange( i ).FormulaArray;

          if( result != curValue )
          {
            return null;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          GetRange( i ).FormulaArray = value;
        }
      }
    }

    /// <summary>
    /// Returns or sets the object's formula in R1C1-style notation and in
    /// the language of the macro. Read/write Variant.
    /// </summary>
    public string FormulaArrayR1C1
    {
      get
      {
        if( m_sheetGroup.IsEmpty ) return null;

        string result = GetRange( 0 ).FormulaArrayR1C1;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          string curValue = GetRange( i ).FormulaArrayR1C1;

          if( result != curValue )
          {
            return null;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          GetRange( i ).FormulaArrayR1C1 = value;
        }
      }
    }
    /// <summary>
    /// True if the formula will be hidden when the worksheet is protected.
    /// False if at least part of formula in the range is not hidden.
    /// </summary>
    public bool FormulaHidden
    {
      get
      {
        if( m_sheetGroup.IsEmpty ) return false;

        bool result = GetRange( 0 ).FormulaHidden;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          bool curValue = GetRange( i ).FormulaHidden;

          if( result != curValue )
          {
            return false;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          GetRange( i ).FormulaHidden = value;
        }
      }
    }

    /// <summary>
    /// Get / set formula DateTime value contained by this cell.
    /// DateTime.MinValue if not all cells of the range have same DateTime value.
    /// </summary>
    public DateTime FormulaDateTime
    {
      get
      {
        if( m_sheetGroup.IsEmpty ) return DateTime.MinValue;

        DateTime result = GetRange( 0 ).FormulaDateTime;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          DateTime curValue = GetRange( i ).FormulaDateTime;

          if( result != curValue )
          {
            return DateTime.MinValue;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          GetRange( i ).FormulaDateTime = value;
        }
      }
    }

    /// <summary>
    /// Indicates whether specified range object has data validation.
    /// If Range is not single cell, then returns true only if all cells have data validation. Read-only.
    /// </summary>
    public bool HasDataValidation
    {
      get
      {
        if( m_sheetGroup.IsEmpty ) return false;

        bool result = GetRange( 0 ).HasDataValidation;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          bool curValue = GetRange( i ).HasDataValidation;

          if( result != curValue )
          {
            result = false;
            break;
          }
        }

        return result;
      }
    }

    /// <summary>
    /// Indicates whether range contains bool value. Read-only.
    /// </summary>
    public bool HasBoolean
    {
      get
      {
        if( m_sheetGroup.IsEmpty )
          return false;

        bool result = GetRange( 0 ).HasBoolean;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          bool curValue = GetRange( i ).HasBoolean;

          if( result != curValue )
          {
            return false;
          }
        }

        return result;
      }
    }
    /// <summary>
    /// Indicates whether range contains DateTime value. Read-only.
    /// </summary>
    public bool HasDateTime
    {
      get
      {
        if( m_sheetGroup.IsEmpty ) return false;

        bool result = GetRange( 0 ).HasDateTime;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          bool curValue = GetRange( i ).HasDateTime;

          if( result != curValue )
          {
            return false;
          }
        }

        return result;
      }
    }

    /// <summary>
    /// Indicates if current range has formula bool value. Read-only.
    /// </summary>
    public bool HasFormulaBoolValue
    {
      get
      {
        if( m_sheetGroup.IsEmpty ) return false;

        bool result = GetRange( 0 ).HasFormulaBoolValue;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          bool curValue = GetRange( i ).HasFormulaBoolValue;

          if( result != curValue )
          {
            return false;
          }
        }

        return result;
      }
    }
    /// <summary>
    /// Indicates if current range has formula error value. Read-only.
    /// </summary>
    public bool HasFormulaErrorValue
    {
      get
      {
        if( m_sheetGroup.IsEmpty ) return false;

        bool result = GetRange( 0 ).HasFormulaErrorValue;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          bool curValue = GetRange( i ).HasFormulaErrorValue;

          if( result != curValue )
          {
            return false;
          }
        }

        return result;
      }
    }
    /// <summary>
    /// Indicates if current range has formula value formatted as DateTime. Read-only.
    /// </summary>
    public bool HasFormulaDateTime
    {
      get
      {
        if( m_sheetGroup.IsEmpty ) return false;

        bool result = GetRange( 0 ).HasFormulaDateTime;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          bool curValue = GetRange( i ).HasFormulaDateTime;

          if( result != curValue )
          {
            result = false;
            break;
          }
        }

        return result;
      }
    }
    /// <summary>
    /// Indicates if the current range has formula number value. Read-only.
    /// </summary>
    public bool HasFormulaNumberValue
    {
      get
      {
        if( m_sheetGroup.IsEmpty ) return false;

        bool result = GetRange( 0 ).HasFormulaNumberValue;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          bool curValue = GetRange( i ).HasFormulaNumberValue;

          if( result != curValue )
          {
            result = false;
            break;
          }
        }

        return result;
      }
    }
    /// <summary>
    /// Indicates if the current range has formula string value. Read-only.
    /// </summary>
    public bool HasFormulaStringValue
    {
      get
      {
        if( m_sheetGroup.IsEmpty ) return false;

        bool result = GetRange( 0 ).HasFormulaStringValue;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          bool curValue = GetRange( i ).HasFormulaStringValue;

          if( result != curValue )
          {
            result = false;
            break;
          }
        }

        return result;
      }
    }
    /// <summary>
    /// True if all cells in the range contain formulas; False if
    /// at least one of the cells in the range doesn't contain a formula.
    /// Read-only Boolean.
    /// </summary>
    public bool HasFormula
    {
      get
      {
        if( m_sheetGroup.IsEmpty ) return false;

        bool result = GetRange( 0 ).HasFormula;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          bool curValue = GetRange( i ).HasFormula;

          if( result != curValue )
          {
            return false;
          }
        }

        return result;
      }
    }

    /// <summary>
    /// Indicates whether range contains array-entered formula. Read-only.
    /// </summary>
    public bool HasFormulaArray
    {
      get
      {
        if( m_sheetGroup.IsEmpty ) return false;

        bool result = GetRange( 0 ).HasFormulaArray;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          bool curValue = GetRange( i ).HasFormulaArray;

          if( result != curValue )
          {
            return false;
          }
        }

        return result;
      }
    }

    /// <summary>
    /// Indicates whether the range contains number. Read-only.
    /// </summary>
    public bool HasNumber
    {
      get
      {
        if( m_sheetGroup.IsEmpty ) return false;

        bool result = GetRange( 0 ).HasNumber;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          bool curValue = GetRange( i ).HasNumber;

          if( result != curValue )
          {
            return false;
          }
        }

        return result;
      }
    }

    /// <summary>
    /// Indicates whether cell contains formatted rich text string.
    /// </summary>
    public bool HasRichText
    {
      get
      {
        if( m_sheetGroup.IsEmpty ) return false;

        bool result = GetRange( 0 ).HasRichText;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          bool curValue = GetRange( i ).HasRichText;

          if( result != curValue )
          {
            return false;
          }
        }

        return result;
      }
    }

    /// <summary>
    /// Indicates whether the range contains String. Read-only.
    /// </summary>
    public bool HasString
    {
      get
      {
        if( m_sheetGroup.IsEmpty ) return false;

        bool result = GetRange( 0 ).HasString;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          bool curValue = GetRange( i ).HasString;

          if( result != curValue )
          {
            return false;
          }
        }

        return result;
      }
    }

    /// <summary>
    /// Indicates whether range has default style. False means default style.
    /// Read-only.
    /// </summary>
    public bool HasStyle
    {
      get
      {
        if( m_sheetGroup.IsEmpty ) return false;

        bool result = GetRange( 0 ).HasStyle;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          bool curValue = GetRange( i ).HasStyle;

          if( result != curValue )
          {
            return false;
          }
        }

        return result;
      }
    }

    /// <summary>
    /// Returns or sets the horizontal alignment for the specified object.
    /// Read/write ExcelHAlign.
    /// </summary>
    public ExcelHAlign HorizontalAlignment
    {
      get
      {
        if( m_sheetGroup.IsEmpty ) return ExcelHAlign.HAlignGeneral;

        ExcelHAlign result = GetRange( 0 ).HorizontalAlignment;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          ExcelHAlign curValue = GetRange( i ).HorizontalAlignment;

          if( result != curValue )
          {
            return ExcelHAlign.HAlignGeneral;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          GetRange( i ).HorizontalAlignment = value;
        }
      }
    }

    /// <summary>
    /// Returns or sets the indent level for the cell or range. Can be an integer
    /// from 0 to 15 for Excel 97-2003 and 250 for Excel 2007. Read/write Integer.
    /// </summary>
    public int IndentLevel
    {
      get
      {
        if( m_sheetGroup.IsEmpty ) return int.MinValue;

        int result = GetRange( 0 ).IndentLevel;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          int curValue = GetRange( i ).IndentLevel;

          if( result != curValue )
          {
            return int.MinValue;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          GetRange( i ).IndentLevel = value;
        }
      }
    }

    /// <summary>
    /// Indicates whether the range is blank. Read-only.
    /// </summary>
    public bool IsBlank
    {
      get
      {
        if( m_sheetGroup.IsEmpty ) return false;

        bool result = GetRange( 0 ).IsBlank;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          bool curValue = GetRange( i ).IsBlank;

          if( result != curValue )
          {
            return false;
          }
        }

        return result;
      }
    }

    /// <summary>
    /// Indicates whether range contains boolean value. Read-only.
    /// </summary>
    public bool IsBoolean
    {
      get
      {
        if( m_sheetGroup.IsEmpty ) return false;

        bool result = GetRange( 0 ).IsBoolean;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          bool curValue = GetRange( i ).IsBoolean;

          if( result != curValue )
          {
            return false;
          }
        }

        return result;
      }
    }

    /// <summary>
    /// Indicates whether range contains error value.
    /// </summary>
    public bool IsError
    {
      get
      {
        if( m_sheetGroup.IsEmpty ) return false;

        bool result = GetRange( 0 ).IsError;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          bool curValue = GetRange( i ).IsError;

          if( result != curValue )
          {
            return false;
          }
        }

        return result;
      }
    }

    /// <summary>
    /// Indicates whether this range is grouped by column. Read-only.
    /// </summary>
    public bool IsGroupedByColumn
    {
      get
      {
        if( m_sheetGroup.IsEmpty ) return false;

        bool result = GetRange( 0 ).IsGroupedByColumn;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          bool curValue = GetRange( i ).IsGroupedByColumn;

          if( result != curValue )
          {
            return false;
          }
        }

        return result;
      }
    }

    /// <summary>
    /// Indicates whether this range is grouped by row. Read-only.
    /// </summary>
    public bool IsGroupedByRow
    {
      get
      {
        if( m_sheetGroup.IsEmpty ) return false;

        bool result = GetRange( 0 ).IsGroupedByRow;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          bool curValue = GetRange( i ).IsGroupedByRow;

          if( result != curValue )
          {
            return false;
          }
        }

        return result;
      }
    }

    /// <summary>
    /// Indicates whether cell is initialized. Read-only.
    /// </summary>
    public bool IsInitialized
    {
      get
      {
        if( m_sheetGroup.IsEmpty ) return false;

        bool result = GetRange( 0 ).IsInitialized;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          bool curValue = GetRange( i ).IsInitialized;

          if( result != curValue )
          {
            return false;
          }
        }

        return result;
      }
    }

    /// <summary>
    /// Returns last column of the range. Read-only.
    /// </summary>
    public int    LastColumn
    {
      get
      {
        return m_iLastColumn;
      }
    }
    /// <summary>
    /// Returns last row of the range. Read-only.
    /// </summary>
    public int    LastRow
    {
      get
      {
        return m_iLastRow;
      }
    }
    /// <summary>
    /// Gets / sets double value of the range.
    /// </summary>
    public double Number
    {
      get
      {
        if( m_sheetGroup.IsEmpty ) return double.MinValue;

        double result = GetRange( 0 ).Number;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          double curValue = GetRange( i ).Number;

          if( result != curValue )
          {
            return double.MinValue;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
            if (BitConverter.DoubleToInt64Bits(value) == BitConverter.DoubleToInt64Bits(-0.0))
                value = 0;

          GetRange( i ).Number = value;
        }
      }
    }

    /// <summary>
    /// Format of current cell. Analog of Style.NumberFormat property.
    /// </summary>
    public string NumberFormat
    {
      get
      {
        if( m_sheetGroup.IsEmpty ) return null;

        string result = GetRange( 0 ).NumberFormat;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          string curValue = GetRange( i ).NumberFormat;

          if( result != curValue )
          {
            return null;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          GetRange( i ).NumberFormat = value;
        }
      }
    }

    /// <summary>
    /// Returns the number of the first row of the first area in
    /// the range. Read-only Long.
    /// </summary>
    public int Row
    {
      get
      {
        return m_iFirstRow;
      }
    }

    /// <summary>
    /// Row group level. Read-only.
    /// -1 - Not all rows in the range have same group level.
    /// 0 - No grouping,
    /// 1 - 7 - Group level.
    /// </summary>
    public int RowGroupLevel
    {
      get
      {
        if( m_sheetGroup.IsEmpty ) return int.MinValue;

        int result = GetRange( 0 ).RowGroupLevel;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          int curValue = GetRange( i ).RowGroupLevel;

          if( result != curValue )
          {
            return int.MinValue;
          }
        }

        return result;
      }
    }

    /// <summary>
    /// Returns the height of all the rows in the range specified,
    /// measured in points. Returns Double.MinValue if the rows in the specified range
    /// aren't all the same height. Read / write Double.
    /// </summary>
    public double RowHeight
    {
      get
      {
        if( m_sheetGroup.IsEmpty ) return double.MinValue;

        double result = GetRange( 0 ).RowHeight;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          double curValue = GetRange( i ).RowHeight;

          if( result != curValue )
          {
            return double.MinValue;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          GetRange( i ).RowHeight = value;
        }
      }
    }

    /// <summary>
    /// For a Range object, returns an array of Range objects that represent the
    /// rows in the specified range.
    /// </summary>
    public IRange[] Rows
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    /// <summary>
    /// For a Range object, returns an array of Range objects that represent the
    /// columns in the specified range.
    /// </summary>
    public IRange[] Columns
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    /// <summary>
    /// Returns a Style object that represents the style of the specified
    /// range. Read/write IStyle.
    /// </summary>
    public IStyle CellStyle
    {
      get
      {
        if( m_style == null )
        {
          m_style = new StyleGroup( Application, this );
        }

        return m_style;
      }
      set
      {
        throw new NotImplementedException();
      }
    }

    /// <summary>
    /// Returns name of the Style object that represents the style of the specified
    /// range. Read/write String.
    /// </summary>
    public string CellStyleName
    {
      get
      {
        if( m_sheetGroup.IsEmpty ) return null;

        string result = GetRange( 0 ).CellStyleName;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          string curValue = GetRange( i ).CellStyleName;

          if( result != curValue )
          {
            return null;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          GetRange( i ).CellStyleName = value;
        }
      }
    }

    /// <summary>
    /// Gets / sets string value of the range.
    /// </summary>
    public string Text
    {
      get
      {
        if( m_sheetGroup.IsEmpty ) return null;

        string result = GetRange( 0 ).Text;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          string curValue = GetRange( i ).Text;

          if( result != curValue )
          {
            return null;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          GetRange( i ).Text = value;
        }
      }
    }

    /// <summary>
    /// Gets / sets time value of the range.
    /// </summary>
    public TimeSpan TimeSpan
    {
      get
      {
        if( m_sheetGroup.IsEmpty ) return TimeSpan.MinValue;

        TimeSpan result = GetRange( 0 ).TimeSpan;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          TimeSpan curValue = GetRange( i ).TimeSpan;

          if( result != curValue )
          {
            return TimeSpan.MinValue;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          GetRange( i ).TimeSpan = value;
        }
      }
    }

    /// <summary>
    /// Returns or sets the value of the specified range.
    /// Read/write Variant.
    /// </summary>
    public string Value
    {
      get
      {
        if( m_sheetGroup.IsEmpty ) return null;

        string result = GetRange( 0 ).Value;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          string curValue = GetRange( i ).Value;

          if( result != curValue )
          {
            return null;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          GetRange( i ).Value = value;
        }
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

    /// <summary>
    /// Returns or sets the cell value. Read/write Variant.
    /// The only difference between this property and the Value property is
    /// that the Value2 property doesn't use the Currency and Date data types.
    /// </summary>
    public object Value2
    {
      get
      {
        if( m_sheetGroup.IsEmpty ) return null;

        object result = GetRange( 0 ).Value2;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          object curValue = GetRange( i ).Value2;

          if( result != curValue )
          {
            return null;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          GetRange( i ).Value2 = value;
        }
      }
    }

    /// <summary>
    /// Returns or sets the vertical alignment of the specified object.
    /// Read/write ExcelVAlign.
    /// </summary>
    public ExcelVAlign VerticalAlignment
    {
      get
      {
        if( m_sheetGroup.IsEmpty ) return ExcelVAlign.VAlignTop;

        ExcelVAlign result = GetRange( 0 ).VerticalAlignment;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          ExcelVAlign curValue = GetRange( i ).VerticalAlignment;

          if( result != curValue )
          {
            return ExcelVAlign.VAlignTop;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          GetRange( i ).VerticalAlignment = value;
        }
      }
    }

    /// <summary>
    /// Returns a Worksheet object that represents the worksheet
    /// containing the specified range. Read-only.
    /// </summary>
    public IWorksheet Worksheet
    {
      get
      {
        return m_sheetGroup;
      }
    }

    /// <summary>
    /// Gets / sets cell by row and column index. Row and column indexes are one-based.
    /// </summary>
    public IRange this[ int row, int column ]
    {
      get
      {
        return new RangeGroup( Application, m_sheetGroup, row, column, row, column );
      }
      set
      {
        throw new NotSupportedException();
      }
    }

    /// <summary>
    /// Get cell range. Row and column indexes are one-based. Read-only.
    /// </summary>
    public IRange this[ int row, int column, int lastRow, int lastColumn ]
    {
      get
      {
        return new RangeGroup( Application, m_sheetGroup, row, column, lastRow, lastColumn );
      }
    }

    /// <summary>
    /// Get cell range. Read-only.
    /// </summary>
    public IRange this[ string name ]
    {
      get
      {
        return this[ name, false ];
      }
    }
    /// <summary>
    /// Get cell range. Read-only.
    /// </summary>
    public IRange this[ string name, bool IsR1C1Notation ]
    {
      get
      {
        return new RangeGroup( Application, m_sheetGroup, name, IsR1C1Notation );
      }
    }
    /// <summary>
    /// Collection of conditional formats.
    /// </summary>
    public IConditionalFormats ConditionalFormats
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    /// <summary>
    /// Data validation for the range.
    /// </summary>
    public IDataValidation DataValidation
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    /// <summary>
    /// Gets / sets string value evaluated by formula.
    /// </summary>
    public string FormulaStringValue
    {
      get
      {
        if( m_sheetGroup.IsEmpty ) return null;

        string result = GetRange( 0 ).FormulaStringValue;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          string curValue = GetRange( i ).FormulaStringValue;

          if( result != curValue )
          {
            return null;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          GetRange( i ).FormulaStringValue = value;
        }
      }
    }

    /// <summary>
    /// Gets / sets number value evaluated by formula.
    /// </summary>
    public double FormulaNumberValue
    {
      get
      {
        if( m_sheetGroup.IsEmpty ) return double.MinValue;

        double result = GetRange( 0 ).FormulaNumberValue;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          double curValue = GetRange( i ).FormulaNumberValue;

          if( result != curValue )
          {
            return double.MinValue;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          GetRange( i ).FormulaNumberValue = value;
        }
      }
    }
    /// <summary>
    /// Gets / sets number value evaluated by formula.
    /// </summary>
    public bool FormulaBoolValue
    {
      get
      {
        if( m_sheetGroup.IsEmpty ) return false;

        bool result = GetRange( 0 ).FormulaBoolValue;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          bool curValue = GetRange( i ).FormulaBoolValue;

          if( result != curValue )
          {
            return false;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          GetRange( i ).FormulaBoolValue = value;
        }
      }
    }
    /// <summary>
    /// Returns the calculated value of the formula as a string.
    /// </summary>
    public string FormulaErrorValue
    {
      get
      {
        if( m_sheetGroup.IsEmpty ) return null;

        string result = GetRange( 0 ).FormulaErrorValue;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          string curValue = GetRange( i ).FormulaStringValue;

          if( result != curValue )
          {
            return null;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          GetRange( i ).FormulaErrorValue = value;
        }
      }
    }
    /// <summary>
    /// Comment assigned to the range. Read-only.
    /// </summary>
    public ICommentShape Comment
    {
      get
      {
        throw new NotSupportedException();
      }
    }

    /// <summary>
    /// String with rich text formatting. Read-only.
    /// </summary>
    public IRichTextString RichText
    {
      get
      {
        if( m_richText == null )
        {
          m_richText = new RichTextStringGroup( Application, this );
        }

        return m_richText;
      }
    }

    /// <summary>
    /// Indicates whether this range is part of merged range. Read-only.
    /// </summary>
    public bool IsMerged
    {
      get
      {
        if( m_sheetGroup.IsEmpty ) return false;

        bool result = GetRange( 0 ).IsMerged;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          bool curValue = GetRange( i ).IsMerged;

          if( result != curValue )
          {
            return false;
          }
        }

        return result;
      }
    }

    /// <summary>
    /// Returns a Range object that represents the merged range containing
    /// the specified cell. If the specified cell isn�t in a merged range,
    /// this property returns NULL. Read-only.
    /// </summary>
    public IRange MergeArea
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    /// <summary>
    /// True if Microsoft Excel wraps the text in the object.
    /// Read/write Boolean.
    /// </summary>
    public bool WrapText
    {
      get
      {
        if( m_sheetGroup.IsEmpty ) return false;

        bool result = GetRange( 0 ).WrapText;

        for( int i = 1, len = m_sheetGroup.Count; i < len; i++ )
        {
          bool curValue = GetRange( i ).WrapText;

          if( result != curValue )
          {
            return false;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          GetRange( i ).WrapText = value;
        }
      }
    }

    /// <summary>
    /// Indicates is current range has external formula. Read-only.
    /// </summary>
    public bool HasExternalFormula
    {
      get
      {
        if( m_sheetGroup.IsEmpty ) return false;

        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          if( !GetRange( i ).HasExternalFormula )
          {
            return false;
          }
        }

        return true;
      }
    }
    /// <summary>
    /// Represents ignore error options. If not single cell returs concatenateed flags.
    /// </summary>
    public ExcelIgnoreError IgnoreErrorOptions
    {
      get
      {
        if( m_sheetGroup.IsEmpty ) return ExcelIgnoreError.None;

        ExcelIgnoreError result = ExcelIgnoreError.All;

        for( int i = 0, len = m_sheetGroup.Count; i < len && result != ExcelIgnoreError.None; i++ )
        {
          result &= GetRange( i ).IgnoreErrorOptions;
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          GetRange( i ).IgnoreErrorOptions = value;
        }
      }
    }
    /// <summary>
    /// Indicates whether all values in the range are preserved as strings.
    /// </summary>
    public bool? IsStringsPreserved
    {
      get
      {
        return m_sheetGroup.GetStringPreservedValue( this );
      }
      set
      {
        m_sheetGroup.SetStringPreservedValue( this, value );
      }
    }
    /// <summary>
    /// Gets/sets built in style.
    /// </summary>
    public BuiltInStyles? BuiltInStyle
    {
      get
      {
        if( m_sheetGroup.IsEmpty )
          return null;

        BuiltInStyles? result = GetRange( 0 ).BuiltInStyle;

        for( int i = 1, len = m_sheetGroup.Count; i < len && result != null; i++ )
        {
          BuiltInStyles? current = GetRange( i ).BuiltInStyle;

          if( current != result )
          {
            result = null;
            break;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
        {
          GetRange( i ).BuiltInStyle = value;
        }
      }
    }
    #endregion

    #region IRange methods
    /// <summary>
    /// Activates a single cell, which must be inside the current selection.
    /// To select a range of cells, use the Select method.
    /// </summary>
    /// <returns></returns>
    public IRange Activate()
    {
      throw new NotSupportedException();
    }
        /// <summary>
        /// Activates a single cell, scroll to it and activates the corresponding sheet.
        /// To select a range of cells, use the Select method.
        /// </summary>
    /// <param name="scroll">True to scroll to the cell</param>
        /// <returns></returns>
    public IRange Activate(bool scroll)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// This method groups current range.
    /// </summary>
    /// <param name="groupBy">
    /// This parameter specifies whether the grouping should be performed by rows or by columns. 
    /// </param>
    /// <returns>Current range after grouping.</returns>
    public IRange Group( ExcelGroupBy groupBy )
    {
      throw new NotSupportedException();
    }

    /// <summary>
    /// This method groups current range.
    /// </summary>
    /// <param name="groupBy">
    /// This parameter specifies whether grouping should
    /// be performed by rows or by columns. 
    /// </param>
    /// <param name="bCollapsed">Indicates whether group should be collapsed.</param>
    /// <returns>Current range after grouping.</returns>
    public IRange Group( ExcelGroupBy groupBy, bool bCollapsed )
    {
      throw new NotSupportedException();
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

    /// <summary>
    /// Creates a merged cell from the specified Range object.
    /// </summary>
    public void Merge()
    {
      for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
      {
        GetRange( i ).Merge();
      }
    }

    /// <summary>
    /// Creates a merged cell from the specified Range object.
    /// </summary>
    /// <param name="clearCells">Indicates whether to clear unnecessary cells.</param>
    public void Merge( bool clearCells )
    {
      for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
      {
        GetRange( i ).Merge( clearCells );
      }
    }

    /// <summary>
    /// Ungroups current range.
    /// </summary>
    /// <param name="groupBy">Indicates type of ungrouping. Ungroup by columns or by rows.</param>
    /// <returns>Current range after ungrouping.</returns>
    public IRange Ungroup( ExcelGroupBy groupBy )
    {
      throw new NotSupportedException();
    }

    /// <summary>
    /// Separates a merged area into individual cells.
    /// </summary>
    public void UnMerge()
    {
      for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
      {
        GetRange( i ).UnMerge();
      }
    }

    /// <summary>
    /// Freezes pane at the current range.
    /// </summary>
    public void FreezePanes()
    {
      throw new NotSupportedException();
    }

    /// <summary>
    /// Clear the contents of the Range.
    /// </summary>
    public void Clear()
    {
      for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
      {
        GetRange( i ).Clear();
      }
    }

    /// <summary>
    /// Clear the contents of the Range with formatting.
    /// </summary>
    /// <param name="isClearFormat">True if formatting should also be cleared.</param>
    public void Clear( bool isClearFormat )
    {
      for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
      {
        GetRange( i ).Clear( isClearFormat );
      }
    }

    /// <summary>
    /// Clear the contents of the Range and shifts the cells Up or Left
    /// without formula or merged ranges update.
    /// </summary>
    /// <param name="direction">Cells shift direction Up/Left.</param>
    public void Clear( ExcelMoveDirection direction )
    {
      for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
      {
        GetRange( i ).Clear( direction );
      }
    }

    /// <summary>
    /// Clears the contents, formats and comments of the cell, based on clear options.
    /// </summary>
    /// <param name="option"></param>
    public void Clear(ExcelClearOptions option)
    {
        for (int i = 0, len = m_sheetGroup.Count; i < len; i++)
        {
            GetRange(i).Clear(option);
        }
    }

    /// <summary>
    /// Clear the contents of the Range and shifts the cells Up or Left.
    /// </summary>
    /// <param name="direction">Cells shift direction Up/Left.</param>
    /// <param name="options">Cells shifting options.</param>
    public void Clear( ExcelMoveDirection direction, ExcelCopyRangeOptions options )
    {
      for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
      {
        GetRange( i ).Clear( direction, options );
      }
    }

    /// <summary>
    /// Moves the cells to the specified Range (without updating formulas).
    /// </summary>
    /// <param name="destination">Destination Range.</param>
    public void MoveTo( IRange destination )
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Moves the cells to the specified Range.
    /// </summary>
    /// <param name="destination">Destination Range</param>
    /// <param name="bUpdateFormula">Indicates whether to update formula after move operation.</param>
    public void MoveTo( IRange destination, bool bUpdateFormula )
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Copies the range to the specified destination Range (without updating formulas).
    /// </summary>
    /// <param name="destination">Destination range.</param>
    /// <returns>Range were this range was copied.</returns>
    public IRange CopyTo( IRange destination )
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Copies this range into another location.
    /// </summary>
    /// <param name="destination">Destination range.</param>
    /// <param name="bUpdateFormula">Indicates whether update formula during copy.</param>
    public IRange CopyTo( IRange destination, bool bUpdateFormula )
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Copies this range into another location.
    /// </summary>
    /// <param name="destination">Destination range.</param>
    /// <param name="options">Copy range options.</param>
    /// <returns>Destination range.</returns>
    public IRange CopyTo( IRange destination, ExcelCopyRangeOptions options )
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Returns intersection of this range with the specified one.
    /// </summary>
    /// <param name="range">The Range with which to intersect.</param>
    /// <returns>Range intersection; if there is no intersection, NULL is returned.</returns>
    public IRange IntersectWith( IRange range )
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Returns merge of this range with the specified one.
    /// </summary>
    /// <param name="range">The Range to merge with.</param>
    /// <returns>Merged ranges or NULL if wasn't able to merge ranges.</returns>
    public IRange MergeWith( IRange range )
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Autofits all rows in the range.
    /// </summary>
    public void AutofitRows()
    {
      for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
      {
        GetRange( i ).AutofitRows();
      }
    }

    /// <summary>
    /// Autofits all columns in the range.
    /// </summary>
    public void AutofitColumns()
    {
      for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
      {
        GetRange( i ).AutofitColumns();
      }
    }

    /// <summary>
    /// Adds comment to the range.
    /// </summary>
    /// <returns>Range's comment.</returns>
    public ICommentShape AddComment()
    {
      throw new NotSupportedException();
    }

    /// <summary>
    /// This method searches for the first cell with specified string value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Flag that represent type of search.</param>
    /// <returns></returns>
    public IRange FindFirst(string findValue, Syncfusion.XlsIO.ExcelFindType flags)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// This method searches for the first cell with specified double value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Flag that represent type of search.</param>
    /// <returns>First found cell, or Null if value was not found. </returns>    
    IRange Syncfusion.XlsIO.IRange.FindFirst(double findValue, Syncfusion.XlsIO.ExcelFindType flags)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// This method searches for the first cell with specified bool value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <returns>First found cell, or Null if value was not found.</returns>
    IRange Syncfusion.XlsIO.IRange.FindFirst(bool findValue)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// This method searches for the first cell with specified DateTime value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <returns>First found cell, or Null if value was not found.</returns>
    IRange Syncfusion.XlsIO.IRange.FindFirst(DateTime findValue)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// This method searches for the first cell with specified TimeSpan value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <returns>First found cell, or Null if value was not found.</returns>
    public IRange FindFirst( TimeSpan findValue )
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// This method searches for the all cells with specified string value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Flag that represent type of search.</param>
    /// <returns>All found cells, or Null if value was not found.</returns>    
    public IRange[] FindAll( string findValue, ExcelFindType flags )
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// This method searches for the all cells with specified double value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Flag that represent type of search.</param>
    /// <returns>All found cells, or Null if value was not found.</returns>    
    public IRange[] FindAll( double findValue, ExcelFindType flags )
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// This method searches for the all cells with specified bool value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <returns>All found cells, or Null if value was not found.</returns>
    public IRange[] FindAll( bool findValue )
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// This method searches for the all cells with specified DateTime value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <returns>All found cells, or Null if value was not found.</returns>
    public IRange[] FindAll( DateTime findValue )
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// This method searches for the all cells with specified TimeSpan value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <returns>All found cells, or Null if value was not found.</returns>
    public IRange[] FindAll( TimeSpan findValue )
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Copies range to the clipboard.
    /// </summary>
    public void CopyToClipboard()
    {
      throw new NotSupportedException();
    }
    /// <summary>
    /// Sets around border for current range.
    /// </summary>
    public void BorderAround()
    {
      BorderAround( ExcelLineStyle.Thin );
    }
    /// <summary>
    /// Sets around border for current range.
    /// </summary>
    /// <param name="borderLine">Represents border line.</param>
    public void BorderAround( ExcelLineStyle borderLine )
    {
      BorderAround( borderLine, ExcelKnownColors.Black );
    }
    /// <summary>
    /// Sets around border for current range.
    /// </summary>
    /// <param name="borderLine">Represents border line.</param>
    /// <param name="borderColor">Represents border color.</param>
    public void BorderAround( ExcelLineStyle borderLine, Color borderColor )
    {
      ExcelKnownColors color = Workbook.GetNearestColor( borderColor );

      BorderAround( borderLine, color );
    }
    /// <summary>
    /// Sets around border for current range.
    /// </summary>
    /// <param name="borderLine">Represents border line.</param>
    /// <param name="borderColor">Represents border color as ExcelKnownColors.</param>
    public void BorderAround( ExcelLineStyle borderLine, ExcelKnownColors borderColor )
    {
      for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
      {
        GetRange( i ).BorderAround( borderLine, borderColor );
      }
    }
    /// <summary>
    /// Sets inside border for current range.
    /// </summary>
    public void BorderInside()
    {
      BorderInside( ExcelLineStyle.Thin );
    }
    /// <summary>
    /// Sets inside border for current range.
    /// </summary>
    /// <param name="borderLine">Represents border line.</param>
    public void BorderInside( ExcelLineStyle borderLine )
    {
      BorderInside( borderLine, ExcelKnownColors.Black );
    }
    /// <summary>
    /// Sets inside border for current range.
    /// </summary>
    /// <param name="borderLine">Represents border line.</param>
    /// <param name="borderColor">Represents border color.</param>
    public void BorderInside( ExcelLineStyle borderLine, Color borderColor )
    {
      ExcelKnownColors color = Workbook.GetNearestColor( borderColor );

      BorderInside( borderLine, color );
    }
    /// <summary>
    /// Sets inside border for current range.
    /// </summary>
    /// <param name="borderLine">Represents border line.</param>
    /// <param name="borderColor">Represents border color as ExcelKnownColors.</param>
    public void BorderInside( ExcelLineStyle borderLine, ExcelKnownColors borderColor )
    {
      for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
      {
        GetRange( i ).BorderInside( borderLine, borderColor );
      }
    }
    /// <summary>
    /// Sets none border for current range.
    /// </summary>
    public void BorderNone()
    {
      for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
      {
        GetRange( i ).BorderNone();
      }
    }
    /// <summary>
    /// Collapses current group.
    /// </summary>
    /// <param name="groupBy">
    /// This parameter specifies whether the grouping should be performed by rows or by columns. 
    /// </param>
    public void CollapseGroup( ExcelGroupBy groupBy )
    {
      for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
      {
        GetRange( i ).CollapseGroup( groupBy );
      }
    }
    /// <summary>
    /// Expands current group.
    /// </summary>
    /// <param name="groupBy">
    /// This parameter specifies whether the grouping should be performed by rows or by columns. 
    /// </param>
    public void ExpandGroup( ExcelGroupBy groupBy )
    {
      for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
      {
        GetRange( i ).ExpandGroup( groupBy );
      }
    }
    /// <summary>
    /// Expands current group.
    /// </summary>
    /// <param name="groupBy">
    /// This parameter specifies whether the grouping should be performed by rows or by columns. 
    /// </param>
    /// <param name="flags">Additional option flags.</param>
    public void ExpandGroup( ExcelGroupBy groupBy, ExpandCollapseFlags flags )
    {
      for( int i = 0, len = m_sheetGroup.Count; i < len; i++ )
      {
        GetRange( i ).ExpandGroup( groupBy, flags );
      }
    }
    #endregion

    #region IEnumerable Members

    public System.Collections.IEnumerator GetEnumerator()
    {
        throw new NotImplementedException();
    }

    #endregion
    }
}
