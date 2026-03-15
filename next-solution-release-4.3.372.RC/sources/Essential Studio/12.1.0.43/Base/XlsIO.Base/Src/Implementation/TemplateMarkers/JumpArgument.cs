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

using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Collections.Generic;

#if ( WINRT )
using Windows.UI;
using Syncfusion.XlsIO.Implementation.WINRT;
#endif

#if  (SILVERLIGHT)
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

namespace Syncfusion.XlsIO.Implementation.TemplateMarkers
{
  /// <summary>
  /// JumpArgument for marker. Specifies jump position after applying
  /// single template marker element.
  /// </summary>
  [ TemplateMarker ]
  public class JumpArgument : MarkerArgument
  {
    #region Class constants
    /// <summary>
    /// Name of the group in the regular expression for row index.
    /// </summary>
    protected const string DEF_ROW_INDEX_GROUP = "RowIndex";
    /// <summary>
    /// Name of the group in the regular expression for column index .
    /// </summary>
    protected const string DEF_COLUMN_INDEX_GROUP = "ColumnIndex";
    /// <summary>
    /// Copy styles group name.
    /// </summary>
    protected const string DEF_COPY_STYLES_GROUP = "CopyStyles";
    /// <summary>
    /// Start of the jump argument.
    /// </summary>
    private const string DEF_JUMP = "jump";
    /// <summary>
    /// Value indicating that styles must be copied after jump operation.
    /// </summary>
    protected const string DEF_COPY_STYLES = "copystyles";
    /// <summary>
    /// Index of the group in the regular expression to check whether row index is relative.
    /// </summary>
    protected const int DEF_ROW_RELATIVE_GROUP = 1;
    /// <summary>
    /// Index of the group in the regular expression to check whether column index is relative.
    /// </summary>
    protected const int DEF_COLUMN_RELATIVE_GROUP = 2;
    /// <summary>
    /// Argument priority.
    /// </summary>
    protected const int DEF_PRIORITY = 2;
    /// <summary>
    /// R1C1 cell detection regular expression
    /// </summary>
    protected const string DEF_R1C1_CELL_REGEX = @"R(\[)?(?<"
      + DEF_ROW_INDEX_GROUP + @">[\-]?[\0-9]{0,5})(?(1)\])C(\[)?(?<"
      + DEF_COLUMN_INDEX_GROUP + @">[\-]?[0-9]{0,3})(?(2)\])";
    #endregion

    #region Class static members
    /// <summary>
    /// Regular expression to check cell.
    /// </summary>
    private static readonly Regex s_cellRegex = new Regex( DEF_R1C1_CELL_REGEX,
#if !SILVERLIGHT && !WINRT && !WP
      RegexOptions.Compiled
#else
      RegexOptions.None
#endif
      );
    #endregion

    #region Class members
    /// <summary>
    /// Destination row index.
    /// </summary>
    protected int m_iRow;
    /// <summary>
    /// Destination column index.
    /// </summary>
    protected int m_iColumn;
    /// <summary>
    /// Indicates whether row index is relative.
    /// </summary>
    protected bool m_bRowRelative;
    /// <summary>
    /// Indicates whether column index is relative.
    /// </summary>
    protected bool m_bColumnRelative;
    /// <summary>
    /// Indicates whether styles must be copied.
    /// </summary>
    protected bool m_bCopyStyles;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes a new instance of the JumpArgument class.
    /// </summary>
    public JumpArgument()
    {
    }
    /// <summary>
    /// Initializes a new instance of the JumpArgument class.
    /// </summary>
    /// <param name="iRow">Destination row index.</param>
    /// <param name="iColumn">Destination column index.</param>
    /// <param name="bRowRelative">Indicates whether row index is relative.</param>
    /// <param name="bColumnRelative">Indicates whether column index is relative.</param>
    public JumpArgument( int iRow, int iColumn, bool bRowRelative, bool bColumnRelative )
    {
      m_iRow = iRow;
      m_iColumn = iColumn;
      m_bRowRelative = bRowRelative;
      m_bColumnRelative = bColumnRelative;
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Tries to parse argument string.
    /// </summary>
    /// <param name="strArgument">Argument to parse.</param>
    /// <returns>Parsed argument if possible; null otherwise.</returns>
    public override MarkerArgument TryParse( string strArgument )
    {
      if( strArgument == null || strArgument.Length == 0 ) return null;

      string[] arrParts = strArgument.Split( DEF_PARTS_SEPARATOR );
      int iLength = arrParts.Length;

      if( arrParts[ 0 ].ToLower() != DEF_JUMP ) return null;

      if( iLength > 1 )
      {
        m_bCopyStyles = arrParts[ iLength - 1 ].ToLower() == DEF_COPY_STYLES;

        if( m_bCopyStyles ) iLength--;
      }

      int iCellIndex = iLength - 1;

      if( iCellIndex > 0 )
      {
        if( !TryParseCell( arrParts[ iCellIndex ], out m_iRow, out m_iColumn,
          out m_bRowRelative, out m_bColumnRelative ) ) return null;
      }

      return ( MarkerArgument )Clone();
    }
    /// <summary>
    /// Applies argument to the marker at the specified position
    /// and to the list of cells with markers.
    /// </summary>
    /// <param name="sheet">Worksheet that contains cell to apply marker argument to.</param>
    /// <param name="pOldPosition">Previous cell position.</param>
    /// <param name="iRow">One-based row index of the cell to apply marker argument to.</param>
    /// <param name="iColumn">One-based column index of the cell to apply marker argument to.</param>
    /// <param name="arrMarkerCells">List of cells with markers.</param>
    /// <param name="options">Marker options.</param>
    public override void ApplyArgument( IWorksheet sheet, Point pOldPosition,
      ref int iRow, ref int iColumn, IList<long> arrMarkerCells, MarkerOptionsImpl options,int count )
    {
      int iOriginalRow = iRow;
      int iOriginalColumn = iColumn;
      Point newPosition = GetCellLocation( pOldPosition, options.Workbook );
      iRow = newPosition.X;
      iColumn = newPosition.Y;

      if( m_bCopyStyles && iColumn != 0 && iRow != 0 )
      {
        // Here we have to copy style from the original cell.
        if( sheet == null )
          throw new ArgumentNullException( "sheet" );

        WorksheetImpl worksheet = ( WorksheetImpl )sheet;
        worksheet.CellRecords.CopyStyle( iOriginalRow, iOriginalColumn, iRow, iColumn );
      }
    }
    /// <summary>
    /// Evaluates new position of the cell.
    /// </summary>
    /// <param name="pointStart">Original cell position.</param>
    /// <param name="book">Workbook where operation is performed.</param>
    /// <returns>Point that contains updated cell position.</returns>
    protected Point GetCellLocation( Point pointStart, IWorkbook book )
    {
      return GetCellLocation( pointStart, m_iRow, m_iColumn, m_bRowRelative, m_bColumnRelative, book );
    }
    #endregion

    #region Class static methods
    /// <summary>
    /// Tries to convert string into int value.
    /// </summary>
    /// <param name="strToParse">String to parse.</param>
    /// <param name="iResult">Result integer.</param>
    /// <returns>True if conversion succeeded; false otherwise.</returns>
    private static bool TryParseInt( string strToParse, out int iResult )
    {
      if( strToParse.Length == 0 )
      {
        iResult = 0;
        return true;
      }

      iResult = 0;
      double dResult;

      if( !double.TryParse( strToParse, NumberStyles.Integer, null, out dResult )
        || Math.Abs( dResult ) > int.MaxValue )
      {
        return false;
      }

      iResult = ( int )dResult;
      return true;
    }
    /// <summary>
    /// Converts regular expressions match into row and column coordinates.
    /// </summary>
    /// <param name="strToParse">Value to parse.</param>
    /// <param name="iRow">Resulting row.</param>
    /// <param name="iColumn">Resulting column.</param>
    /// <param name="bRowRelative">Indicates whether row index is relative.</param>
    /// <param name="bColumnRelative">Indicates whether column index is relative.</param>
    /// <returns>True if parsing succeeded.</returns>
    protected static bool TryParseCell( string strToParse, out int iRow, out int iColumn,
      out bool bRowRelative, out bool bColumnRelative )
    {
      iRow = iColumn = 0;
      bRowRelative = bColumnRelative = false;

      if( strToParse == null || strToParse.Length == 0 )
        return false;

      Match m = s_cellRegex.Match( strToParse );
      bool bResult = ( m.Success && m.Length == strToParse.Length );

      if( bResult )
      {
        bRowRelative = ( m.Groups[ DEF_ROW_RELATIVE_GROUP ].Value.Length > 0 );
        bColumnRelative = ( m.Groups[ DEF_COLUMN_RELATIVE_GROUP ].Value.Length > 0 );
        string strValue = m.Groups[ DEF_ROW_INDEX_GROUP ].Value;

        if( !TryParseInt( strValue, out iRow ) ) return false;

        if( iRow == 0 ) bRowRelative = true;

        strValue = m.Groups[ DEF_COLUMN_INDEX_GROUP ].Value;

        if( !TryParseInt( strValue, out iColumn ) ) return false;

        if( iColumn == 0 ) bColumnRelative = true;
      }

      return bResult;
    }
    /// <summary>
    /// Evaluates new position of the cell.
    /// </summary>
    /// <param name="pointStart">Original cell position.</param>
    /// <param name="iRow">Destination row index.</param>
    /// <param name="iColumn">Destination column index.</param>
    /// <param name="bRowRelative">Indicates whether row index is relative.</param>
    /// <param name="bColumnRelative">Indicates whether column index is relative.</param>
    /// <param name="book">Workbook where operation is performed.</param>
    /// <returns>Point that contains updated cell position.</returns>
    protected static Point GetCellLocation( Point pointStart, int iRow, int iColumn,
      bool bRowRelative, bool bColumnRelative, IWorkbook book )
    {
      int iNewColumn = bColumnRelative ? pointStart.Y + iColumn : iColumn;
      int iNewRow = bRowRelative ? pointStart.X + iRow : iRow;

      if( iNewColumn < 1 || iNewColumn > book.MaxColumnCount ) iNewColumn = 0;
      if( iNewRow < 1 || iNewRow > book.MaxRowCount ) iNewRow = 0;

      return new Point( iNewRow, iNewColumn );
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Returns priority of the argument. Read-only.
    /// </summary>
    public override int Priority
    {
      get
      {
        return DEF_PRIORITY;
      }
    }
    /// <summary>
    /// Indicates whether marker should be applies for each marker value. Read-only.
    /// </summary>
    public override bool IsApplyable
    {
      get
      {
        return true;
      }
    }

    #endregion
  }
}
