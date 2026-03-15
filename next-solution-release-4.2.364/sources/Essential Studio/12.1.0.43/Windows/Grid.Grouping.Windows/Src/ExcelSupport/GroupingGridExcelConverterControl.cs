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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text;
using Syncfusion.XlsIO;
using Syncfusion.XlsIO.Implementation;
using Syncfusion.Windows.Forms.Grid;
using Syncfusion.Drawing;
using Syncfusion.Grouping;
using Syncfusion.GridExcelConverter;
#if ASPNET
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Syncfusion.Web.UI.WebControls.Grid.Grouping;
#else 
using Syncfusion.Windows.Forms.Grid.Grouping;
#endif
#endregion

namespace Syncfusion.GroupingGridExcelConverter
{
  /// <summary>
  /// This controls provides support for Exporting datas from a GroupingGridControl into an Excel spreadsheet
  /// for verification and/or computation. This Control automatically Copies the Grid's Styles, Formats 
  /// to Excel. 
  /// The GroupingGridExcelConverter Control  is derived from 
  ///<see cref="GridExcelConverterBase"/>.
  /// </summary>
    [ToolboxItem(false)]
  public class GroupingGridExcelConverterControl : GridExcelConverterBase
  {
    #region Class constants
    /// <summary>
    /// 
    /// </summary>
    private static readonly DisplayElementKind[] DEF_ADJUSTGROUPLEVELIFEQUAL = new DisplayElementKind[]
    {
      DisplayElementKind.Caption,
      DisplayElementKind.Table,
      DisplayElementKind.NestedTable,
    };
    #endregion

    #region Export Grouping Grid to Excel methods
    /// <summary>
    /// Converts grouping grid into excel file.
    /// </summary>
    /// <param name="grouping">Source grouping grid.</param>
    /// <param name="strFileName">Destination worksheet.</param>
    /// <param name="options">Convert options.</param>
    public void GroupingGridToExcel( GridGroupingControl grouping, string strFileName,
      ConverterOptions options )
    {
      if( grouping == null )
        throw new ArgumentNullException( "grouping" );

      if( strFileName == null )
        throw new ArgumentNullException( "strFileName" );

      if( strFileName.Length == 0 )
        throw new ArgumentException( "strFileName - string can not be empty" );

      ExcelEngine engine = new ExcelEngine();
      IWorkbook book = engine.Excel.Workbooks.Create( 1 );
      IWorksheet sheet = book.Worksheets[ 0 ];

      GroupingGridToExcel( grouping, sheet, options );

      book.Close( true, strFileName );
    }
    /// <summary>
    /// Converts grouping grid into excel file.
    /// </summary>
    /// <param name="grouping">Source grouping grid.</param>
    /// <param name="sheet">Destination worksheet.</param>
    /// <param name="options">Convert options.</param>
    public void GroupingGridToExcel( GridGroupingControl grouping, IWorksheet sheet,
      ConverterOptions options )
    {
      if( grouping == null )
        throw new ArgumentNullException( "grouping" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      sheet.PageSetup.IsSummaryRowBelow = false;
      GridTable table = grouping.Table;

      if( ( options & ConverterOptions.Visible ) != 0 )
      {
        ExportElements( table.DisplayElements, sheet, 0, options );
      }
      else
      {
        ExportElements( table.Elements, sheet, 0, options );
      }
      GridTableModel tableModel = null;
#if ASPNET
     tableModel = grouping.Table.TableModel;
#else
      tableModel = grouping.TableModel;
#endif
      CopyRowHeightFromGrid( tableModel, sheet );
      CopyColumnWidthFromGrid( tableModel, sheet, tableModel.GetColumnIndentCount() );
    }
    /// <summary>
    /// Exports collection of elements into worksheet.
    /// </summary>
    /// <param name="arrElements">Collection of elements to export.</param>
    /// <param name="sheet">Destination worksheet.</param>
    /// <param name="index">Starting zero-based index to the first row.</param>
    /// <param name="options">Convert options.</param>
    /// <returns>Index to the row after last exported element.</returns>
    private int ExportElements( IList arrElements, IWorksheet sheet, int index, ConverterOptions options )
    {
      if( arrElements == null )
        throw new ArgumentNullException( "arrElements" );

      if( sheet == null )
        throw new ArgumentNullException( "worksheet" );

      if( index < 0 )
        throw new ArgumentOutOfRangeException( "index", index, "Value can not be less 0" );

      if( arrElements.Count == 0 ) return 0;

      Element element = ( Element )arrElements[ 0 ];
      GridTableDescriptor tableDesc = ( GridTableDescriptor )element.ParentTable.TableDescriptor;
      int iColumnCount = tableDesc.Columns.Count;

      //for( int i = 0, len = arrElements.Count; i < len; i++ )
      int iIndexInGroup = 0;
      int i = 0;
      int iCount = arrElements.Count;

      Stack stackStartIndexes = new Stack();
      int iCurLevel = -1;

      while( i < iCount )
      {
        element = ( Element )arrElements[ i ];
        int iElementLevel = element.GroupLevel;

        if( iElementLevel != iCurLevel &&
          Array.IndexOf( DEF_ADJUSTGROUPLEVELIFEQUAL, element.Kind ) == -1 )
        {
          AdjustGroupingLevel( stackStartIndexes, ref iCurLevel, iElementLevel, sheet, index + i, false );
        }

        switch( element.Kind )
        {
          case DisplayElementKind.Table:
            GridTable table = ( GridTable )element;

            IList elements = ( ( options & ConverterOptions.Visible ) != 0 )
              ? elements = table.DisplayElements
              : elements = table.Elements;

            i += ExportElements( elements, sheet, index + i, options );
            break;

          case DisplayElementKind.Empty:
            GridEmptySection empty = ( GridEmptySection )element;
            i += ExportEmptyCells( sheet, index + i, options,
              empty.Appearance.EmptyCell, iColumnCount );
            break;

          case DisplayElementKind.AddNewRecord:
            RecordRow record = ( RecordRow )element;
            i += ExportRecordRow( record, sheet, index + i, options, iIndexInGroup );
            break;

          case DisplayElementKind.Record:
            i += ExportRecordRow( ( RecordRow )element, sheet, index + i, options, iIndexInGroup );
            iIndexInGroup++;
            break;

          case DisplayElementKind.Caption:
            iIndexInGroup = 0;
            GridCaptionRow captionRow = ( GridCaptionRow )element;
            i += ExportCaption( captionRow, sheet, index + i, options, ref iCurLevel, stackStartIndexes );
            break;

          case DisplayElementKind.Summary:
            GridSummaryRow summary = ( GridSummaryRow )element;
            i += ExportSummaryRow( summary, sheet, index + i, options );
            break;

          case DisplayElementKind.NestedTable:
            NestedTable nestedTable = ( NestedTable )element;
            i += ExportNestedTable( nestedTable, sheet, index + i, options );
            break;

          case DisplayElementKind.None:
          case DisplayElementKind.FilterBar:
          case DisplayElementKind.ColumnHeader:
          case DisplayElementKind.GroupHeader:
          case DisplayElementKind.GroupFooter:
          case DisplayElementKind.RecordPreview:
          case DisplayElementKind.GroupPreview:
            sheet.Range[ index + i + 1, 1 ].Text = element.Kind.ToString();
            sheet.Range[ index + i + 1, 2 ].Text = element.Info;
            i++;
            break;
        }
      }

      if( iCurLevel >= 0 )
      {
        AdjustGroupingLevel( stackStartIndexes, ref iCurLevel, 0, sheet, index + i, false );
      }

      //sheet.Range[ index + 1, 1, index + iCount, 1 ].Group( ExcelGroupBy.ByRows, false );
      return iCount;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="empty"></param>
    /// <param name="sheet"></param>
    /// <param name="index"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    private int ExportEmptyCells( IWorksheet sheet, int index,
      ConverterOptions options, GridTableCellStyleInfo style, int iColumnCount )
    {
      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( index < 0 )
        throw new ArgumentOutOfRangeException( "index", index, "Value can not be less 0" );

      IRange usedRange = sheet.Range;
      int iRowIndex = index + 1;
      IRange range = sheet.Range[ iRowIndex, 1, iRowIndex, iColumnCount ];
      CopyStyle( style, range );
      return 1;
    }
    /// <summary>
    /// Exports summary row into excel worksheet.
    /// </summary>
    /// <param name="summary">Summary to export.</param>
    /// <param name="sheet">Destination worksheet.</param>
    /// <param name="index">Zero-based index to first row of the summary.</param>
    /// <param name="options">Converter options.</param>
    private int ExportSummaryRow( GridSummaryRow summary, IWorksheet sheet, int index,
      ConverterOptions options )
    {
      if( summary == null )
        throw new ArgumentNullException( "summary" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( index < 0 )
        throw new ArgumentOutOfRangeException( "index", index, "Value can not be less 0" );

      int iRow = index + 1;
      GridSummaryRowDescriptor descriptor = summary.SummaryRowDescriptor;
      IRange range = sheet.Range[ iRow, 1 ];
      range.Text = descriptor.Title;
      CopyStyle( descriptor.Appearance.SummaryTitleCell, range );

      //GridSummaryColumnDescriptorCollection arrColumns = descriptor.SummaryColumns;
      GridTableDescriptor table = summary.ParentTableDescriptor;
      GridSummaryColumnDescriptorCollection arrColumns = descriptor.SummaryColumns;

      int iColumnsCount = table.Columns.Count;
      range = sheet.Range[ iRow, 2, iRow, iColumnsCount ];
      CopyStyle( descriptor.Appearance.SummaryEmptyCell, range );

      for( int i = 0, len = arrColumns.Count; i < len; i++ )
      {
        GridSummaryColumnDescriptor column = arrColumns[ i ];
        int iColumnIndex = column.ColInRecord;
        GridTableCellStyleInfo style = null;

        if( iColumnIndex == -1 )
        {
          range = sheet.Range[ iRow, 1 ];
        }
        else
        {
          range = sheet.Range[ iRow, iColumnIndex + 1 ];
          style = table.Appearance.SummaryFieldCell;
          CopyStyle( style, range );
        }

        range.Text = column.GetDisplayText( summary.ParentGroup );
      }

      return 1;
    }
    /// <summary>
    /// Performs all necessary operations (grouping necessary rows, etc.)
    /// to set current group level to the desired level.
    /// </summary>
    /// <param name="stackStartIndexes">Stack with starting index of previously found groups.</param>
    /// <param name="iCurLevel">Current group level.</param>
    /// <param name="iGroupLevel">Group level to set.</param>
    /// <param name="sheet">Destination worksheet.</param>
    /// <param name="index">Current row zero-based index.</param>
    /// <param name="bStartNewGroup">Indicates whether new group should be started at index.</param>
    private void AdjustGroupingLevel( Stack stackStartIndexes, ref int iCurLevel, int iGroupLevel,
      IWorksheet sheet, int index, bool bStartNewGroup )
    {
      if( stackStartIndexes == null )
        throw new ArgumentNullException( "stackStartIndexes" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( iCurLevel < 0 || iCurLevel < iGroupLevel )
      {
        iCurLevel = iGroupLevel;
        stackStartIndexes.Push( index );
      }
      else if( iCurLevel >= iGroupLevel )
      {
        int iLastLevel = bStartNewGroup ? iGroupLevel : iGroupLevel + 1;

        for( ; iCurLevel >= iLastLevel; iCurLevel-- )
        {
          int iStartIndex = ( int )stackStartIndexes.Pop() + 1;
          int iEndIndex = index;

          if( iStartIndex != iEndIndex )
          {
            sheet.Range[ iStartIndex + 1, 1, index, 1 ].Group( ExcelGroupBy.ByRows, false );
          }
        }

        if( bStartNewGroup )
        {
          iCurLevel++;
          stackStartIndexes.Push( index );
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="row"></param>
    /// <param name="sheet"></param>
    /// <param name="index"></param>
    /// <param name="options"></param>
    /// <param name="iIndexInGroup"></param>
    /// <returns></returns>
    private int ExportRecordRow( RecordRow row, IWorksheet sheet, int index, ConverterOptions options
      , int iIndexInGroup )
    {
      if( row == null )
        throw new ArgumentNullException( "row" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( index < 0 )
        throw new ArgumentOutOfRangeException( "index", index, "Value can not be less 0" );

      if( iIndexInGroup < 0 )
        throw new ArgumentOutOfRangeException( "iIndexInGroup", iIndexInGroup, "Value can not be less than 0" );

      FieldDescriptorCollection arrFields = row.ParentTable.TableDescriptor.Fields;
      int iRowIndex = index + 1;

      GridTableDescriptor table = ( GridTableDescriptor )row.ParentTableDescriptor;
      GridColumnDescriptorCollection arrColumns = table.Columns;

      for( int i = 0, len = arrColumns.Count; i < len; i++ )
      {
        GridColumnDescriptor column = arrColumns[ i ];
        FieldDescriptor field = arrFields[ column.Name ];

        IRange range = sheet.Range[ iRowIndex, i + 1 ];
        object value = row.ParentRecord.GetValue( field );
        range.Text = ( value == null ) ? string.Empty : value.ToString();
        
        if( ( options & ConverterOptions.Visible ) != 0 )
        {
          ExportStyle( row, field, range, options );
        }
        else
        {
          GridTableCellStyleInfo style = ( iIndexInGroup % 2 == 0 )
            ? table.Appearance.RecordFieldCell
            : table.Appearance.AlternateRecordFieldCell;

          CopyStyle( style, range );
        }
      }

      return 1;
    }
    /// <summary>
    /// Export element style.
    /// </summary>
    /// <param name="sourceElement">Source element to export.</param>
    /// <param name="sourceField">Source field descriptor.</param>
    /// <param name="destRange">Destination range.</param>
    /// <param name="options">Convert options.</param>
    private void ExportStyle( Element sourceElement, FieldDescriptor sourceField, IRange destRange,
      ConverterOptions options )
    {
      if( destRange == null )
        throw new ArgumentNullException( "destRange" );

      if( sourceElement == null )
        throw new ArgumentNullException( "sourceElement" );

      GridTable table = sourceElement.ParentTable as GridTable;

      if( table == null )
      {
        Debug.WriteLine( "Bad table type" );
        return;
      }

      GridTableCellStyleInfo style;

      //if( options == ConverterOptions.ConvertVisible )
    {
      string strFieldName = sourceField != null ? sourceField.Name : null;
      style = table.GetTableCellStyle( sourceElement, strFieldName );
    }
      //      else // if( options == ConverterOptions.ConvertAll )
      //      {
      //        style = table.Appearance.AnyRecordFieldCell;
      //      }

      CopyStyle( style, destRange );
    }
    /// <summary>
    /// Exports caption row.
    /// </summary>
    /// <param name="captionRow">Source caption row.</param>
    /// <param name="sheet">Destionation worksheet.</param>
    /// <param name="index">Capiton's zero-based row index in the worksheet.</param>
    /// <param name="options">Convert options.</param>
    /// <param name="iCurrentGroupLevel">Current group level.</param>
    /// <returns>Zero-based row index that point to the row after caption.</returns>
    private int ExportCaption( GridCaptionRow captionRow, IWorksheet sheet, int index,
      ConverterOptions options, ref int iCurrentGroupLevel, Stack stackStartIndexes )
    {
      if( captionRow == null )
        throw new ArgumentNullException( "captionRow" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( index < 0 )
        throw new ArgumentOutOfRangeException( "index", index, "Value can not be less 0" );

      IRange range = sheet.Range[ index + 1, 1 ];
      range.Text = GetCaptionText( captionRow );
 
      CopyStyle( captionRow.Appearance.GroupCaptionCell, range );
      GridTableModel tableModel = captionRow.ParentTable.TableModel;
      int iRealColCount = tableModel.ColCount - tableModel.GetColumnIndentCount();

      sheet.Range[ index + 1, 1, index + 1, iRealColCount ].Merge();

      int iGroupLevel = captionRow.GroupLevel;
      AdjustGroupingLevel( stackStartIndexes, ref iCurrentGroupLevel, iGroupLevel, sheet, index, true );
      
      return 1;
    }
    /// <summary>
    /// Exports group to the excel worksheet.
    /// </summary>
    /// <param name="group">Group to export.</param>
    /// <param name="sheet">Destination worksheet.</param>
    /// <param name="index">Zero-based row index.</Bparam>
    /// <returns>Zero-based row index that point to the row after group rows.</returns>
    private int ExportGroup( GridGroup group, IWorksheet sheet, int index )
    {
      if( group == null )
        throw new ArgumentNullException( "group" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( index < 0 )
        throw new ArgumentOutOfRangeException( "index", index, "Value can not be less 0" );

      RecordsInDetailsCollection arrRecords = group.Records;
      return ExportElements( arrRecords, sheet, index, ConverterOptions.Default );
    }
    /// <summary>
    /// Exports nested table.
    /// </summary>
    /// <param name="table">Source table to export.</param>
    /// <param name="sheet">Destination worksheet.</param>
    /// <param name="index">Capiton's zero-based row index in the worksheet.</param>
    /// <param name="options">Convert options.</param>
    /// <returns>Zero-based row index that point to the row after caption.</returns>
    private int ExportNestedTable( NestedTable table, IWorksheet sheet, int index, ConverterOptions options )
    {
      IList elements = null;
      ChildTable childTable = table.ChildTable;

      elements = ( ( options & ConverterOptions.Visible ) != 0 )
        ? ( IList )childTable.DisplayElements
        : ( IList )childTable.Elements;

      return ExportElements( elements, sheet, index, options );
    }
    #endregion

    #region Class static methods
    /// <summary>
    /// Retrieves caption text from caption row.
    /// </summary>
    /// <param name="caption">Caption row to get text from.</param>
    /// <returns>Caption text.</returns>
    public static string GetCaptionText( GridCaptionRow caption )
    {
      return GetGroupCaptionText( caption.ParentGroup );
    }
    /// <summary>
    /// Retrieves caption text for group.
    /// </summary>
    /// <param name="group">Group to get caption text from.</param>
    /// <returns>Caption text.</returns>
    public static string GetGroupCaptionText( Group group )
    {
      IGridGroupOptionsSource g = group as IGridGroupOptionsSource;
      string captionText;

        captionText = ( g != null )
          ? g.GroupOptions.CaptionText
          : captionText = "{CategoryCaption}: {Category} - {RecordCount} Items";
 
      return GetGroupCaptionDisplayText( group, captionText );
    }

    /// <summary>
    /// Gets caption text for a group.
    /// </summary>
    /// <param name="group">The group to get caption text for.</param>
    /// <param name="format">Caption format.</param>
    /// <returns>Caption text for the group.</returns>
    public static string GetGroupCaptionDisplayText( Group group, string format )
    {
      GridTableDescriptor tableDescriptor = ( GridTableDescriptor )group.ParentTableDescriptor;

      bool raiseException = false;
      ArrayList al = new ArrayList();
      int iOpenBracketPos = format.IndexOf( "{" );
      StringBuilder sb = new StringBuilder();
      int iCloseBracketPos = 0;

      if( iOpenBracketPos == -1 )
      {
        sb.Append( format );
      }
      else
      {
        sb.Append( format.Substring( 0, iOpenBracketPos + 1 ) );
      }
 
      while( iOpenBracketPos != -1 )
      {
        iCloseBracketPos = format.IndexOf( "}", iOpenBracketPos );

        if( iOpenBracketPos != -1 && iCloseBracketPos != -1 && iCloseBracketPos > iOpenBracketPos )
        {
          int n3 = format.IndexOfAny( new char[] { '}', ':' }, iOpenBracketPos );
          string name = format.Substring( iOpenBracketPos + 1, n3 - iOpenBracketPos - 1 );
  
          sb.Append( al.Count.ToString() );
          object obj = "";

          if( name == "TableName" )
          {
            obj = group.ParentTableDescriptor.Name;
          }
          else if( name == "CategoryName" )
          {
            obj = group.Name;
          }
          else if( name == "CategoryCaption" )
          {
            string fieldName = group.Name;

            GridColumnDescriptor cd = group.GroupLevel >= 0
              ? tableDescriptor.Columns.FindByMappingName( fieldName )
              : null;

            obj = ( cd != null ) ? cd.HeaderText : fieldName;
          }
          else if( name == "Category" )
          {
            obj = group.Category;
          }
          else if( name == "RecordCount" )
          {
            obj = group.GetFilteredRecordCount();
          }
          else
          {
            GridTableDescriptor td = ( ( GridTableDescriptor )group.ParentTableDescriptor );
            int summaryRowNum;
            int dot = name.IndexOf( '.' );

            if( dot != -1 )
            {
              string rowName = name.Substring( 0, dot );
              name = name.Substring( dot + 1 );
              summaryRowNum = td.SummaryRows.IndexOf(rowName);
            }
            else
            {
              summaryRowNum = td.SummaryRows.IndexOf( "GroupCaption" );
            }

            if( summaryRowNum != -1 )
            {
              GridSummaryColumnDescriptor scd=null;
#if ASPNET
       scd= td.SummaryRows[ summaryRowNum ].SummaryColumns.GetSummaryColDescriptor(name);
#else
              scd = td.SummaryRows[ summaryRowNum ].SummaryColumns[name];
#endif
              if( scd != null )
              {
                Group g = group;
                Syncfusion.Collections.BinaryTree.ITreeTableSummary[] sums =
                  g.GetSummaries( group.ParentTable );
                int ndx = scd.GetSummaryIndex();
                obj = ( scd.GetDisplayText( sums[ ndx ] ) );
              }
            }
          }
          al.Add( obj );
          iOpenBracketPos = format.IndexOf( "{", iCloseBracketPos );

          if( iOpenBracketPos == -1 )
          {
            sb.Append( format.Substring( n3 ) );
          }
          else
          {
            sb.Append( format.Substring( n3, iOpenBracketPos - n3 + 1 ) );
          }
        }
        else
        {
          if( raiseException )
          {
            throw new FormatException( "No closing char found: " + sb.ToString() );
          }
          else
          {
            break;
          }
        }
      }
      string formatString = sb.ToString();

      try
      {
        return String.Format(formatString, al.ToArray());
      }
      catch (Exception ex)
      {
        return formatString + ": " + ex.Message;
      }
    }

    #endregion
  }
}
