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

using ISummary = Syncfusion.Collections.BinaryTree.ITreeTableSummary;
#if ASPNET
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Syncfusion.Web.UI.WebControls.Grid.Grouping;
#else
using Syncfusion.Windows.Forms.Grid.Grouping;
using System.Collections.Generic;
using System.Globalization;
#endif
#endregion

namespace Syncfusion.GroupingGridExcelConverter
{
    /// <summary>
    /// GroupingGridExcelConverterControl class provides support for Exporting datas from a GroupingGridControl into an Excel spreadsheet
    /// for verification and/or computation. This Control automatically Copies the Grid's Styles, Formats 
    /// to Excel. 
    /// The GroupingGridExcelConverter Control  is derived from 
    ///<see cref="GridExcelConverterBase"/>.
    /// </summary>
    [ToolboxItem(false)]
    public class GroupingGridExcelConverterControl : GridExcelConverterBase
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public GroupingGridExcelConverterControl()
            : base()
        {
        }

        #region Maintain all GridTables in a hash table with hierarchy level
        GridTableDictionary tableDictionary = new GridTableDictionary();
        void GetGridTables(GridTable table)
        {
            tableDictionary.Add(table, 0);
            GetRelatedTables(table, 1);
        }

        void GetRelatedTables(GridTable table, int level)
        {
            foreach (GridTable relatedTable in table.RelatedTables)
            {
                bool isHierarchyTable = false;
                foreach (GridRelationDescriptor rd in table.TableDescriptor.Relations)
                {
                   if (rd.ChildTableDescriptor.Name == relatedTable.TableDescriptor.Name)
                    {
                        if(rd.RelationKind == RelationKind.RelatedMasterDetails || rd.RelationKind == RelationKind.UniformChildList)
                            isHierarchyTable = true;

                        break;
                    }
                }
                if (isHierarchyTable)
                {
                    tableDictionary.Add(relatedTable, level);
                    GetRelatedTables(relatedTable, ++level);
                    --level;
                }
            }
            
        }
        private bool applyExcelFilter = false;
        /// <summary>        
        /// This is specifically used to show the empty filter in excel sheet while exporting.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool ApplyExcelFilter
        {
            get
            {
                return applyExcelFilter;
            }
            set
            {
                if (applyExcelFilter != value)
                    applyExcelFilter = value;
            }
        }

        /// <summary>
        /// Maintain all GridTables in a hash table with hierarchy level.
        /// </summary>
        public class GridTableDictionary : Hashtable
        {
            Hashtable levels;
            /// <summary>
            /// Initializes a new hash table.
            /// </summary>
            public GridTableDictionary()
                : base()
            {
                 levels = new Hashtable();
            }

            /// <summary>
            /// Adds an element with the specified key and value into the [REDACTED].
            /// </summary>
            /// <param name="key">The key of the element to add.</param>
            /// <param name="value">The value of the element to add. The value can be null.</param>
            public new void Add(object key, object value)
            {
                GridTable table = key as GridTable;
                string tableName = table.TableDescriptor.Name;
                levels.Add(tableName, value);
                base.Add(tableName, table);
            }
            /// <summary>
            /// Gets the table level
            /// </summary>
            /// <param name="table">string</param>
            /// <returns>int</returns>
            public int GetTableLevel(Syncfusion.Grouping.Table table)
            {
                return GetTableLevel(table.TableDescriptor.Name);
            }
            /// <summary>
            /// Gets the table level
            /// </summary>
            /// <param name="tableName">string</param>
            /// <returns>int</returns>
            public int GetTableLevel(string tableName)
            {
                return (int)levels[tableName];
            }

            /// <summary>
            /// Retrieves the GridTable from the hashtable collection.
            /// </summary>
            /// <param name="tableName">Name of the table to be retrieved.</param>
            /// <returns>GridTable</returns>
            public GridTable GetGridTable(string tableName)
            {
                return (GridTable)base[tableName];
            }

        }
            #endregion

        #region Export Grouping Grid to Excel methods
        /// <summary>
        /// A method that converts grouping grid into excel file. Grid will accept .Net format Strings (Such as "F4" for a number with four decimal places)
        /// that are not valid with excel. Hence they will be exported as string instead of format like if you specify the format "F3", then 
        /// the cell will have the string "F3".
        /// </summary>
        /// <param name="grouping">Source grouping grid.</param>
        /// <param name="strFileName">Destination worksheet.</param>
        /// <param name="options">Convert options.</param>
        public void GroupingGridToExcel(GridGroupingControl grouping, string strFileName,
          ConverterOptions options)
        {
            if (grouping == null)
                throw new ArgumentNullException("grouping");

            if (strFileName == null)
                throw new ArgumentNullException("strFileName");

            if (strFileName.Length == 0)
                throw new ArgumentException("strFileName - string can not be empty");

            using (ExcelEngine engine = CreateEngine())
            {
                IWorkbook book = engine.Excel.Workbooks.Create(1);
                IWorksheet sheet = book.Worksheets[0];

                GroupingGridToExcel(grouping, sheet, options);

                book.Close(true, strFileName);
            }
        }
        /// <summary>
        /// A method that converts grouping grid into excel file.
        /// </summary>
        /// <param name="grouping">Source grouping grid.</param>
        /// <param name="sheet">Destination worksheet.</param>
        /// <param name="options">Convert options.</param>
        public void GroupingGridToExcel(GridGroupingControl grouping, IWorksheet sheet,
          ConverterOptions options)
        {
            if (grouping == null)
                throw new ArgumentNullException("grouping");

            if (sheet == null)
                throw new ArgumentNullException("sheet");

            GroupingGrid = grouping;
            this.options = options;
            GridTable table = grouping.Table;
            GetGridTables(table);

            sheet.PageSetup.IsSummaryRowBelow = false;

            if ((options & ConverterOptions.Visible) != 0)
            {
                //Fix: Collapsed (non visible) NestedTable elements also exported
                //Also made a change in the ExportRecordRow and ExportElements under switch case Caption
                //ExportElements(table.DisplayElements, sheet, 0, options, 1);
                ExportElements(table.NestedDisplayElements, sheet, 0, options, 1);

            }
            else
            {
                ElementsInTableCollection arrElements = table.Elements;
                bool bVisible = ((ConverterOptions.Visible & options) != 0);
                if (!bVisible)
                {
                    foreach (Element el in arrElements)
                    {
                        if (el is GridSummaryRow || el is GridCaptionRow)
                            if (!el.ParentGroup.IsExpanded)
                                if (!expandedList.ContainsKey(el))
                                    expandedList.Add(el, el.ParentGroup.IsExpanded);
                    }
                }
                ExportElements(arrElements, sheet, 0, options, 1);
                if (!bVisible)
                {
                    foreach (Element el in arrElements)
                    {
                        if (el is GridSummaryRow || el is GridCaptionRow)
                            if (expandedList.ContainsKey(el))
                            {
                                el.ParentGroup.IsExpanded = false;                                
                            }
                    }
                }
            }

            //Set the column width for all tables.
            GridTableModel tableModel = null;

            int lastColCount = 0;
            foreach (DictionaryEntry dEntry in tableDictionary)
            {

                GridTable t = (dEntry.Value as GridTable);
                tableModel = t.TableModel;

                GridTableDescriptor td = t.TableDescriptor;
                int colSetDiff = 0;
                foreach (GridColumnSetDescriptor columnSet in td.ColumnSets)
                    colSetDiff += columnSet.GetColCount() - 1;

                int tableLevel = tableDictionary.GetTableLevel(t);
                int colCount = td.VisibleColumns.Count + colSetDiff + tableLevel;
                int startColIndex = lastColCount < colCount ? (lastColCount - colSetDiff - tableLevel) : -1;
                if (startColIndex >= 0)
                {
                    CopyColumnWidthFromGrid(td, sheet, startColIndex, colCount, lastColCount + 1);
                    lastColCount = colCount;
                }
            }
        }

        /// <summary>
        /// Copies column width settings from grid model into excel worksheet.
        /// </summary>
        /// <param name="td">GridTableDescriptor.</param>
        /// <param name="sheet">Destination WorkSheet.</param>
        /// <param name="startColIndex">Starting grid column index from which width should be set.</param>
        /// <param name="colCount">Visible column count.</param>
		/// <param name="iStartIndex">Start column index in sheet.</param>
        private void CopyColumnWidthFromGrid(GridTableDescriptor td, IWorksheet sheet, int startColIndex, int colCount, int iStartIndex)
        {
            if (sheet == null)
                throw new ArgumentNullException("sheet");

            for (int i = startColIndex; i < td.VisibleColumns.Count; i++)
            {
                GridVisibleColumnDescriptor visibleColumn = td.VisibleColumns[i];
                if (td.ColumnSets.Contains(visibleColumn.Name))
                {
#if ASPNET
                    GridColumnSetDescriptor columnSet = td.ColumnSets.GetColumnSetDescriptor(visibleColumn.Name);
#else
                    GridColumnSetDescriptor columnSet = td.ColumnSets[visibleColumn.Name];
#endif
                    int[] widths;
                    GridColumnDescriptor[] columns = columnSet.GetWidthColumns(out widths);

                    foreach (GridColumnDescriptor column in columns)
                    {
                        double dWidth = sheet.PixelsToColumnWidth(column.Width);
                        sheet.SetColumnWidth(iStartIndex++, dWidth);
                    }

                }
                else
                {
#if ASPNET
                    GridColumnDescriptor column = td.Columns.GetColumnDescriptor(visibleColumn.Name);
#else
                    GridColumnDescriptor column = td.Columns[visibleColumn.Name];
#endif
                    if (column != null)
                    {
                        double dWidth = sheet.PixelsToColumnWidth(column.Width);
                        dWidth = i == 0 && (td.GroupedColumns.Count > 0 || td.Relations.Count > 0) ? dWidth + td.GroupedColumns.Count + td.Relations.Count + 2 : dWidth;
                        sheet.SetColumnWidth(iStartIndex++, dWidth);
                    }
                    else
                    {
                        double dWidth = 0.0;
                        sheet.SetColumnWidth(iStartIndex++, dWidth);
                    }
                }
            }
        }

        int iCurLevel = -1;
        bool allowSwapSummaryGroupPreview;
        private Dictionary<Element, bool> expandedList = new Dictionary<Element, bool>();
        /// <summary>
        /// Exports collection of elements into worksheet.
        /// </summary>
        /// <param name="arrElements">Collection of elements to export.</param>
        /// <param name="sheet">Destination worksheet.</param>
        /// <param name="index">Starting zero-based index to the first row.</param>
        /// <param name="options">Convert options.</param>
        /// <param name="iGroupLevel">Group Level.</param>
        /// <returns>Index to the row after last exported element.</returns>
        private int ExportElements(IList arrElements, IWorksheet sheet, int index,
    ConverterOptions options, int iGroupLevel)
        {
            if (arrElements == null)
                throw new ArgumentNullException("arrElements");

            if (sheet == null)
                throw new ArgumentNullException("worksheet");

            if (index < 0)
                throw new ArgumentOutOfRangeException("index", index, "Value can not be less 0");

            int iCount = arrElements.Count;

            if (iCount == 0) return 0;

            Element elementFirst = (Element)arrElements[0];
            GridTableDescriptor tableDesc = (GridTableDescriptor)elementFirst.ParentTable.TableDescriptor;
            int iColumnCount = tableDesc.Columns.Count;

            int iIndexInGroup = 0;
            //int i = 0;

            Stack stackStartIndexes = new Stack();
            //int iCurLevel = -1;
            int iRow = 0;
            int iStartRow = iRow;
            int iSkipCount = 0;

            allowSwapSummaryGroupPreview = true;
            int iSummaryRowCount = 0;
            foreach (GridSummaryRowDescriptor srd in tableDesc.SummaryRows)
                if (srd.Visible || (options & ConverterOptions.Default) == 0)
                    iSummaryRowCount++;

            DateTime start = DateTime.Now;
            foreach (Element element in arrElements)
            {
                if (DateTime.Now.Subtract(start).TotalSeconds > 5)
                {
                    Application.DoEvents();
                    start = DateTime.Now;
                }                                                    

                if (iSkipCount > 0)
                {
                    iSkipCount--;
                    continue;
                }

                //Added for: collapsed (non visible) NestedTable elements also exported fix.
                if ((options & ConverterOptions.Visible) == ConverterOptions.Visible)
                    iGroupLevel = tableDictionary.GetTableLevel(element.ParentTable) + 1;

                GridExportElementEventArgs e = new GridExportElementEventArgs(element, iRow);
                RaiseExportElement(e);
                if (e.Cancel)
                {
                    if (e.Element.ParentGroup.FilteredChildNodeCount == 0)
                        iRow++;
                    //dont sawp summary row and group preview row when summary row
                    //or group preview row export is canceled...
                    //look at under GroupPreview case and in ExportSummaryRow method.
                    if (e.Element.Kind == DisplayElementKind.Summary || e.Element.Kind == DisplayElementKind.GroupPreview)
                        allowSwapSummaryGroupPreview = false;
                    continue;
                }
                bool iVisible = ((ConverterOptions.Visible & options) != 0);
                if (!iVisible)
                    element.ParentGroup.IsExpanded = true;
                switch (element.Kind)
                {

                    case DisplayElementKind.Table:
                        GridTable table = (GridTable)element;
                        table = (GridTable)element;

                        IList elements = ((options & ConverterOptions.Visible) != 0)
                          ? elements = table.DisplayElements
                          : elements = table.Elements;

                        iRow += ExportElements(elements, sheet, index + iRow, options, iGroupLevel);
                        break;

                    case DisplayElementKind.Empty:
                        GridEmptySection empty = (GridEmptySection)element;
                        if (!(element is GridHiddenSection && element.ParentGroup.IsTopLevelGroup))
                        {
                            iRow += ExportEmptyCells(sheet, index + iRow, options,
                              empty.Appearance.EmptyCell, iColumnCount, iGroupLevel);
                        }
                        break;

                    case DisplayElementKind.AddNewRecord:
                        bool newRow = false;
                        if (iGroupLevel > 1)
                            newRow = groupingGrid.NestedTableGroupOptions.ShowAddNewRecordBeforeDetails | groupingGrid.NestedTableGroupOptions.ShowAddNewRecordAfterDetails;
                        if(element.ChildTableGroupLevel == 0)
                            newRow = groupingGrid.TopLevelGroupOptions.ShowAddNewRecordBeforeDetails | groupingGrid.TopLevelGroupOptions.ShowAddNewRecordAfterDetails;
                        else if (element.ChildTableGroupLevel > 0)
                            newRow = groupingGrid.ChildGroupOptions.ShowAddNewRecordBeforeDetails | groupingGrid.ChildGroupOptions.ShowAddNewRecordAfterDetails;

                        if (groupingGrid.TableDescriptor.AllowNew && newRow)
                        {
                            RecordRow record;
                            if (element is RecordRow)
                            {
                                record = (RecordRow)element;
                            }
                            else
                            {
                                record = new RecordRow(new RecordRowsPart(element.GetRecord()));
                            }

                            iRow += ExportRecordRow(record, sheet, index + iRow, options,
                              iIndexInGroup, iGroupLevel, out iSkipCount);
                        }
                        break;

                    case DisplayElementKind.Record:
                        RecordRow row;
                        if (element is GridRecord)
                        {
                            GridRecord rec = element as GridRecord;
                            row = new RecordRow(new RecordRowsPart(rec.GetRecord()));
                        }
                        else
                            row = element as RecordRow;


                        iRow += ExportRecordRow(row, sheet, index + iRow, options,
                          iIndexInGroup, iGroupLevel, out iSkipCount);
                        iIndexInGroup++;
                        break;

                    ////TODO: SummaryInCaption should be considered.
                    case DisplayElementKind.Caption:
                        iIndexInGroup = 0;
                        GridCaptionRow captionRow;
                        if (element is GridCaptionSection)
                        {
                            GridCaptionSection cs = element as GridCaptionSection;
                            captionRow = new GridCaptionRow(cs);
                        }
                        else
                            captionRow = (GridCaptionRow)element;

                        //Added addtional code for: collapsed (non visible) NestedTable elements also exported fix.
                        //if ((options & ConverterOptions.Visible) == ConverterOptions.Visible)
                        //    iGroupLevel = tableCollection.IndexOf(element.ParentTable) + 1;
                        bool ShowCaption = (element.ChildTableGroupLevel == 0) ? groupingGrid.TopLevelGroupOptions.ShowCaption : true;
                        if (ExportCaptionSummary)
                        {
                            bool showCaptionSummary = false;
                            int level = 0;
                            if (iGroupLevel > 1)
                                showCaptionSummary = groupingGrid.NestedTableGroupOptions.ShowCaptionSummaryCells;
                            if (element.ChildTableGroupLevel == 0)
                                showCaptionSummary = groupingGrid.GetTableDescriptor(tableDesc.Name).TopLevelGroupOptions.ShowCaptionSummaryCells;
                            else if (element.ChildTableGroupLevel > 0)
                            {
                                showCaptionSummary = groupingGrid.GetTableDescriptor(tableDesc.Name).ChildGroupOptions.ShowCaptionSummaryCells;
                                level = groupingGrid.ChildGroupOptions.ShowCaptionPlusMinus ? 2 : 1;
                            }

                            string name = tableDesc.Name;
                            if (ShowCaption && showCaptionSummary)
                            {
                                if (element.ParentChildTable != null && !element.ParentChildTable.IsTopLevelGroup)
                                    name = element.ParentChildTable.Name;
                                GridRangeInfo rowRange = this.GroupingGrid.GetTable(name).GetElementRangeInfo(element);
                                GridStyleInfo style = this.GroupingGrid.GetTableControl(name).GetViewStyleInfo(rowRange.Top, rowRange.Left + iGroupLevel + level, false);
                                iRow += ExportCaptionSummaryRow(captionRow, sheet, index + iRow, options, /*ref iCurLevel*/
                                  iGroupLevel, stackStartIndexes, iSummaryRowCount, element.ChildTableGroupLevel, style);
                            }
                            else if (ShowCaption)
                            {
                                iRow += ExportCaption(captionRow, sheet, index + iRow, options, /*ref iCurLevel*/
                                  iGroupLevel, stackStartIndexes, iSummaryRowCount);
                            }
                        }
                        break;

                    case DisplayElementKind.Summary:
                        GridSummaryRow summary = (GridSummaryRow)element;                        
                        GridRangeInfo range = this.GroupingGrid.GetTable(tableDesc.Name).GetElementRangeInfo(summary);
                        GridStyleInfo cellStyle = this.GroupingGrid.GetTable(tableDesc.Name).GetTableCellStyle(summary.GetRowIndex(), range.Left + iGroupLevel + summary.GroupLevel);
                        if (cellStyle == null)
                            cellStyle = new GridStyleInfo();
                        iRow += ExportSummaryRow(summary, sheet, index + iRow, options, iGroupLevel, cellStyle);
                        break;
                    case DisplayElementKind.ColumnHeader:
                        GridColumnHeaderRow headerRow;
                        if (element is GridColumnHeaderSection)
                        {
                            GridColumnHeaderSection hs = element as GridColumnHeaderSection;
                            headerRow = new GridColumnHeaderRow(hs);
                        }
                        else
                            headerRow = (GridColumnHeaderRow)element;

                        iRow += ExportColumnHeader(headerRow, sheet, element, index + iRow, options, iGroupLevel, out iSkipCount);
                        if (headerRow.ParentTableDescriptor.IsExcelFilterWired && ApplyExcelFilter)
                            sheet.AutoFilters.FilterRange = sheet.Range[iRow, 1, iRow, tableDesc.Fields.Count];
                        break;
                    case DisplayElementKind.StackedHeader:
                        GridStackedHeaderRow stackedHeaderRow = element as GridStackedHeaderRow;
                        iRow += ExportStackedHeader(stackedHeaderRow, sheet, iGroupLevel, iRow);
                        break;
                    case DisplayElementKind.FilterBar:
                        int ix = index;

                        //Swap the Summary Row and the GroupPreview Row when both are exported
                        //Also look into the ExportSummaryRow method...
                        if (allowSwapSummaryGroupPreview && ((options & ConverterOptions.Visible) != ConverterOptions.Visible ||
                            element.ParentGroup.IsExpanded))
                            ix += iSummaryRowCount;
                        if (element is GridFilterBarRow && !ApplyExcelFilter)
                            iRow += ExportPreviewRow(element, sheet, ix, iRow);
                        break;
                    case DisplayElementKind.None:
                    case DisplayElementKind.GroupHeader:
                    case DisplayElementKind.GroupFooter:
                    case DisplayElementKind.GroupPreview:

                        int inx = index;

                        //Swap the Summary Row and the GroupPreview Row when both are exported
                        //Also look into the ExportSummaryRow method...
                        if(allowSwapSummaryGroupPreview && ((options & ConverterOptions.Visible) != ConverterOptions.Visible ||
                            element.ParentGroup.IsExpanded))
                            inx += iSummaryRowCount;
                        iRow += ExportPreviewRow(element, sheet, inx, iRow);
                        break;

                    case DisplayElementKind.RecordPreview:
                        if (element.ParentRecord is GridRecord)
                            iRow += ExportPreviewRow(element, sheet, index, iRow);

                        //sheet.Range[ index + iRow, 1 ].Text = element.Kind.ToString();
                        //sheet.Range[ index + iRow, 1 ].Text = element.Info;
                        break;
                }
            }

            if (ExportGroupPlusMinus || ExportRecordPlusMinus)
                if (iCurLevel >= 0)
                    AdjustGroupingLevel(stackStartIndexes, ref iCurLevel/*ref iCurrentGroupLevel*/, 0, sheet, index + iRow, false, iSummaryRowCount);

            bool isVisible = ((ConverterOptions.Visible & options) != 0);

            return iRow - iStartRow;
        }

        private int ExportRowHeader(GridTableDescriptor table, IWorksheet sheet, int row, int col)
        {
            if (sheet == null)
                throw new ArgumentNullException("sheet");

            if (row < 0 )
                throw new ArgumentOutOfRangeException("index", row, "Value can not be less 0");

            if (col < 0)
                throw new ArgumentOutOfRangeException("index", col, "Value can not be less 0");

            sheet.SetColumnWidthInPixels(col, table.TableOptions.RowHeaderWidth);
            IRange range = sheet.Range[row, col];
            range.Text = table.Appearance.RowHeaderCell.Text;
            col++;

            if (ExportStyle || hasHeaderBKColor)
            {
                GridStyleInfo style = new GridStyleInfo();

                if (ExportStyle)
                {
                    //Combine ColumnHeader Styles.
                    CombineStyles(style, table.Appearance.RowHeaderCell);

                    //Set default column header alignmnet                 
                    if (!style.HasVerticalAlignment)
                        style.VerticalAlignment = GridVerticalAlignment.Middle;
                    if (!style.HasHorizontalAlignment)
                        style.HorizontalAlignment = GridHorizontalAlignment.Center;

                    //Set Solid Boders for Header cell
                    if (ExportBorders)
                        style.Borders.All = new GridBorder(GridBorderStyle.Solid);
                }

                if (hasHeaderBKColor)
                    style.BackColor = HeaderBackColor;

                CopyStyle(style, range);
            }
            GridStyleInfo gridCell = table.Appearance.RowHeaderCell;
            GridImportExportCellInfoEventArgs e = new GridImportExportCellInfoEventArgs(range, gridCell, GridConverterAction.Export, row, col);
            RaiseQueryImportExportCellInfo(e);
            if(e.Handled)
                CopyStyle(gridCell, range);

            return col;
        }

        static int colHeaderRowIndex = 0;
        /// <summary>
        /// Exports column header.
        /// </summary>
        /// <param name="headerRow">Header row to export.</param>
        /// <param name="sheet">Destination worksheet.</param>
        /// <param name="element">Element to export.</param>
        /// <param name="index">One-based row index.</param>
        /// <param name="options">Convert options.</param>
        /// <param name="iGroupLevel">Group level - one-based column index.</param>
        /// <param name="iSkipCount">Number of elements to skip.</param>
        /// <returns>Number of exported (used) rows.</returns>
        private int ExportColumnHeader(GridColumnHeaderRow headerRow, IWorksheet sheet,
      Element element, int index, ConverterOptions options, int iGroupLevel, out int iSkipCount)
        {
            if (headerRow == null)
                throw new ArgumentNullException("headerRow");

            if (sheet == null)
                throw new ArgumentNullException("sheet");

            if (index < 0)
                throw new ArgumentOutOfRangeException("index", index, "Value can not be less 0");

            int iRowIndex = index + 1;

            GridTableDescriptor table = (GridTableDescriptor)headerRow.ParentTableDescriptor;
            GridColumnDescriptorCollection arrColumns = table.Columns;
            GridVisibleColumnDescriptorCollection arrVisibleColumns = table.VisibleColumns;
            GridColumnSetDescriptorCollection arrColumnSets = table.ColumnSets;

            GridTableOptionsStyleInfo tableStyleInfo = table.TableOptions;
            sheet.SetRowHeightInPixels(iRowIndex, tableStyleInfo.ColumnHeaderRowHeight);

            int iRowCount = 1;

            for (int i = 0, iColumnIndex = iGroupLevel, len = arrVisibleColumns.Count; i < len; i++, iColumnIndex++)
            {
                GridVisibleColumnDescriptor column = arrVisibleColumns[i];
                string strColumnName = column.Name;

                if (arrColumnSets.Contains(strColumnName))
                {
#if ASPNET
                    GridColumnSetDescriptor columnSet = arrColumnSets.GetColumnSetDescriptor(strColumnName);
#else
                    GridColumnSetDescriptor columnSet = arrColumnSets[strColumnName];
#endif
                    int iRows = ExportColumnSet(arrColumns, columnSet, sheet, iRowIndex, ref iColumnIndex, table);

                    if (iRows > iRowCount) iRowCount = iRows;
                }
                else
                {

#if ASPNET
                    GridColumnDescriptor gridColumn = arrColumns.GetColumnDescriptor(column.Name);
#else
                    GridColumnDescriptor gridColumn = arrColumns[column.Name];
#endif
                    if (gridColumn != null)
                    {
                        if (((options & ConverterOptions.RowHeaders) == ConverterOptions.RowHeaders) && (colHeaderRowIndex != index))
                            iColumnIndex = ExportRowHeader(table, sheet, iRowIndex, iColumnIndex);
                        colHeaderRowIndex = index;

                        IRange range = sheet.Range[iRowIndex, iColumnIndex];
                        range.Text = gridColumn.HeaderText;//column.Name;

                        if (ExportStyle || hasHeaderBKColor)
                        {
                            GridStyleInfo style = new GridStyleInfo();

                            if (ExportStyle)
                            {
                                GridStyleInfo headerStyle = this.GroupingGrid.TableControl.GetViewStyleInfo(element.GetRowIndex(), table.FieldToColIndex(table.NameToField(gridColumn.Name)));
                                //Combine ColumnHeader Styles.
                                CombineStyles(style, headerStyle);

                                //Force Header Text Color before exporting
                                style.TextColor = headerStyle.TextColor;

                                //Set default column header alignmnet                 
                                if (!style.HasVerticalAlignment)
                                    style.VerticalAlignment = GridVerticalAlignment.Middle;
                                if (!style.HasHorizontalAlignment)
                                    style.HorizontalAlignment = GridHorizontalAlignment.Center;

                                //Set Solid Boders for Header cell
                                if (ExportBorders)
                                    style.Borders.All = new GridBorder(GridBorderStyle.Solid);
                            }

                            if (hasHeaderBKColor)
                                style.BackColor = HeaderBackColor;

                            CopyStyle(style, range);
                        }
                        GridStyleInfo gridCell = gridColumn.Appearance.ColumnHeaderCell;

                        if (gridColumn.HeaderImage != null && ExportImage && ExportStyle)
                            ExportImageToExcelCell(range, gridColumn.HeaderImage);

                        GridImportExportCellInfoEventArgs e = new GridImportExportCellInfoEventArgs(range, gridCell, GridConverterAction.Export, iRowIndex, iColumnIndex);
                        RaiseQueryImportExportCellInfo(e);
                        if (e.Handled)
                            CopyStyle(gridCell, range);
                    }
                }
            }
            iSkipCount = iRowCount - 1;
            return iRowCount;
        }

        /// <summary>
        /// Exports header column set into excel.
        /// </summary>
        /// <param name="arrColumns">Columns collection.</param>
        /// <param name="columnSet">Column set to export.</param>
        /// <param name="sheet">Worksheet to export data into.</param>
        /// <param name="iRowIndex">Row index to export to.</param>
        /// <param name="iColumnIndex">Style of the exported cells.</param>
        /// <param name="table">GridTableDescriptor.</param>
        /// <returns>Number of rows used by column set.</returns>
        private int ExportColumnSet(GridColumnDescriptorCollection arrColumns,
          GridColumnSetDescriptor columnSet, IWorksheet sheet,
          int iRowIndex, ref int iColumnIndex, GridTableDescriptor table)
        {
            GridColumnSpanDescriptorCollection arrColumnSpans = columnSet.ColumnSpans;
            int iRowCount = 1;
            int iMaxColumn = 0;

            for (int i = 0, len = arrColumnSpans.Count; i < len; i++)
            {
                GridColumnSpanDescriptor columnSpan = arrColumnSpans[i];
                GridRangeInfo gridRange = columnSpan.Range;

                int iSpanRowIndex = iRowIndex + gridRange.Top;
                int iSpanColumnIndex = iColumnIndex + gridRange.Left;

                //IRange range = sheet.Range[iSpanRowIndex, iSpanColumnIndex];
                IRange range = sheet.Range[iSpanRowIndex, iSpanColumnIndex,
                            iRowIndex + gridRange.Bottom, iColumnIndex + gridRange.Right];
                range.Merge();

                string strColumnName = columnSpan.Name;

#if ASPNET
                    GridColumnDescriptor gridColumn = arrColumns.GetColumnDescriptor(strColumnName);
#else
                GridColumnDescriptor gridColumn = arrColumns[strColumnName];
#endif

                range.Text = gridColumn.HeaderText;

                if (ExportStyle || hasHeaderBKColor)
                {
                    GridStyleInfo style = new GridStyleInfo();
                if (ExportStyle)
                {
                //Combine ColumnHeader Styles.
                CombineStyles(style, gridColumn.Appearance.ColumnHeaderCell);

                //Set default column header alignmnet                 
                if (!style.HasVerticalAlignment)
                    style.VerticalAlignment = GridVerticalAlignment.Middle;
                if (!style.HasHorizontalAlignment)
                    style.HorizontalAlignment = GridHorizontalAlignment.Center;

                    //Set Solid Border for Header cell
                    if (ExportBorders)
                        style.Borders.All = new GridBorder(GridBorderStyle.Solid);
                    }
                    if (hasHeaderBKColor)
                        style.BackColor = HeaderBackColor;
                CopyStyle(style, range);
                }

                GridStyleInfo gridCell = gridColumn.Appearance.ColumnHeaderCell;
                GridImportExportCellInfoEventArgs e = new GridImportExportCellInfoEventArgs(range, gridCell, GridConverterAction.Export, iRowIndex, iColumnIndex);
                RaiseQueryImportExportCellInfo(e);
                if (e.Handled)
                    CopyStyle(gridCell, range);
                iRowCount = Math.Max(gridRange.Bottom + 1, iRowCount);
                iMaxColumn = Math.Max(gridRange.Right, iMaxColumn);
            }

            iColumnIndex += iMaxColumn;
            return iRowCount;
        }

        /// <summary>
        /// Export StackedHeader.
        /// </summary>
        /// <param name="stackHeaderRow">StackedHeaderRow to export.</param>
        /// <param name="sheet">WorkSheet to export data into.</param>
        /// <param name="iGroupLevel">GroupLevel of the StackHeaderRow element.</param>
        /// <param name="iRowIndex">Row index to export to.</param>
        /// <returns>No. of row exported.</returns>
        private int ExportStackedHeader(GridStackedHeaderRow stackHeaderRow, IWorksheet sheet, int iGroupLevel, int iRowIndex)
        {
            if (stackHeaderRow == null)
                throw new ArgumentNullException("stackHeaderRow");

            if (sheet == null)
                throw new ArgumentNullException("sheet");

            if (iRowIndex < 0)
                throw new ArgumentOutOfRangeException("iRowIndex", iRowIndex, "Value can not be less 0");

            if (iGroupLevel <= 0)
                throw new ArgumentOutOfRangeException("iGroupLevel", iGroupLevel, "Value can not be less than or equal 0");

            iRowIndex++;
            GridTableDescriptor table = (GridTableDescriptor)stackHeaderRow.ParentTableDescriptor;
            GridVisibleColumnDescriptorCollection arrVisibleColumns = table.VisibleColumns;
            GridTableOptionsStyleInfo tableStyleInfo = table.TableOptions;
            sheet.SetRowHeightInPixels(iRowIndex, tableStyleInfo.ColumnHeaderRowHeight);
            GridStackedHeaderRowDescriptor shd = stackHeaderRow.StackedHeaderRowDescriptor;
            
            for(int i = 0; i < arrVisibleColumns.Count; i++)
            {
                GridStackedHeaderSpan sp = shd.GetStackedHeaderSpanAt(i);
                IRange range = sheet.Range[iRowIndex, sp.FirstCol + iGroupLevel, iRowIndex, sp.LastCol + iGroupLevel];
                range.Merge();
                range.Text = sp.Header.HeaderText;
                i = sp.LastCol;

                if (ExportStyle || hasHeaderBKColor)
                {
                    GridStyleInfo style = new GridStyleInfo();

                    if (ExportStyle)
                    {
                        //Combine StackHeader Styles.
                        CombineStyles(style, shd.Appearance.StackedHeaderCell);

                        //Set default column header alignmnet                 
                        if (!style.HasVerticalAlignment)
                            style.VerticalAlignment = GridVerticalAlignment.Middle;
                        if (!style.HasHorizontalAlignment)
                            style.HorizontalAlignment = GridHorizontalAlignment.Center;

                        //Set Solid Boders for Header cell
                        if (ExportBorders)
                            style.Borders.All = new GridBorder(GridBorderStyle.Solid);
                    }

                    if (hasHeaderBKColor)
                        style.BackColor = HeaderBackColor;

                    CopyStyle(style, range);
                }
                GridStyleInfo gridCell = shd.Appearance.StackedHeaderCell;
                GridImportExportCellInfoEventArgs e = new GridImportExportCellInfoEventArgs(range, gridCell, GridConverterAction.Export, iRowIndex, sp.FirstCol + iGroupLevel);
                RaiseQueryImportExportCellInfo(e);
                if(e.Handled)
                    CopyStyle(gridCell, range);
            }

            return 1;
        }

        /// <summary>
        /// Exports empty cells.
        /// </summary>
        /// <param name="sheet">Sheet to export cells into.</param>
        /// <param name="index">Zero-based row index.</param>
        /// <param name="options">Converter options.</param>
        /// <param name="style">Style of the cells to export.</param>
        /// <param name="iColumnCount">Number of columns to export.</param>
        /// <param name="iGroupLevel">Group level.</param>
        /// <returns>Number of used rows.</returns>
        private int ExportEmptyCells(IWorksheet sheet, int index,
          ConverterOptions options, GridTableCellStyleInfo style, int iColumnCount,
          int iGroupLevel)
        {
            if (sheet == null)
                throw new ArgumentNullException("sheet");

            if (index < 0)
                throw new ArgumentOutOfRangeException("index", index, "Value can not be less 0");

            IRange usedRange = sheet.Range;
            int iRowIndex = index + 1;

            if (iColumnCount > 0)
            {
                IRange range = sheet.Range[iRowIndex, iGroupLevel, iRowIndex, iColumnCount + iGroupLevel - 1];
                if (ExportStyle)
                {
                    if (ExportBorders)
                        CombineGridTableBorders(style);
                CopyStyle(style, range);
                }
            }

            sheet.SetRowHeightInPixels(iRowIndex, 0);
            return 1;
        }

        static int summaryColIndex = 0;
        /// <summary>
        /// Exports summary row into excel worksheet.
        /// </summary>
        /// <param name="summary">Summary to export.</param>
        /// <param name="sheet">Destination worksheet.</param>
        /// <param name="index">Zero-based index to first row of the summary.</param>
        /// <param name="options">Converter options.</param>
        /// <param name="iGroupLevel">GroupLevel.</param>
        /// <param name="style">Summary row style.</param>
        private int ExportSummaryRow(GridSummaryRow summary, IWorksheet sheet, int index,
          ConverterOptions options, int iGroupLevel, GridStyleInfo style)
        {
            if (summary == null)
                throw new ArgumentNullException("summary");

            if (sheet == null)
                throw new ArgumentNullException("sheet");

            if (index < 0)
                throw new ArgumentOutOfRangeException("index", index, "Value can not be less 0");

            int iRow = index + 1;

            //Swap the Summary Row and the GroupPreview Row when both are exported
            //Also look into the ExportElements method under switch case GroupPreview
            if (allowSwapSummaryGroupPreview && GroupingGrid.ChildGroupOptions.ShowGroupPreview && summary.ParentGroup != summary.ParentChildTable
                && (options & ConverterOptions.Visible) != ConverterOptions.Visible)
                iRow--;
            
            int iColIndex = iGroupLevel;
            if((summaryColIndex != index) && ((options & ConverterOptions.RowHeaders) == ConverterOptions.RowHeaders))
                iColIndex = ExportRowHeader(GroupingGrid.TableDescriptor, sheet, iRow, iColIndex);
            summaryColIndex = index;

            GridSummaryRowDescriptor descriptor = summary.SummaryRowDescriptor;
            IRange range = sheet.Range[iRow, iColIndex];
            range.Text = style.Text;
            int GroupCount = groupingGrid.TableDescriptor.GroupedColumns.Count;
            if(ExportStyle)
            {
                if (ExportBorders)
                {
                    if (!style.Borders.HasTop)
                        style.Borders.Top = new GridBorder(GridBorderStyle.Solid, Color.Blue, GridBorderWeight.Medium);
                    CombineGridTableBorders(style);
                }
                CopyStyle(style, range);
            }
            ExportFormats(style, range);
            GridStyleInfo gridCell = style;
            GridImportExportCellInfoEventArgs e = new GridImportExportCellInfoEventArgs(range, gridCell, GridConverterAction.Export, iRow, iColIndex);
            RaiseQueryImportExportCellInfo(e);
            if (e.Handled)
                CopyStyle(gridCell, range);

            GridTableDescriptor table = summary.ParentTableDescriptor;
            GridSummaryColumnDescriptorCollection arrColumns = descriptor.SummaryColumns;

            int iColumnsCount = table.Engine.TableModel.ColCount;

            GridTableOptionsStyleInfo tableStyleInfo = table.TableOptions;
            sheet.SetRowHeightInPixels(iRow, tableStyleInfo.RecordRowHeight);
            for (int i = 0; i < table.Columns.Count; i++)
            {
                int colIndex = groupingGrid.TableModel.NameToColIndex(table.Columns[i].MappingName);
                GridTableCellStyleInfo summaryTitleStyle = null;
                if (colIndex >= 0)
                    summaryTitleStyle = groupingGrid.GetTable(table.Name).GetTableCellStyle(summary.GetRowIndex(), summary.GroupLevel + colIndex - table.GroupedColumns.Count);
                if (summaryTitleStyle != null && summaryTitleStyle.TableCellIdentity.TableCellType == GridTableCellType.SummaryTitleCell)
                {
                    range = sheet.Range[iRow, iColIndex];
                    if (ExportStyle)
                    {
                        if (!style.Borders.HasTop)
                            style.Borders.Top = new GridBorder(GridBorderStyle.Solid, Color.Blue, GridBorderWeight.Medium);
                        CombineGridTableBorders(style);
                    }
                    ExportFormats(summaryTitleStyle, range);
                    CopyStyle(summaryTitleStyle, range);
                    e = new GridImportExportCellInfoEventArgs(range, summaryTitleStyle, GridConverterAction.Export, iRow, iColIndex);
                    RaiseQueryImportExportCellInfo(e);
                    if (e.Handled)
                        CopyStyle(summaryTitleStyle, range);
                }
                else
                    for (int j = 0; j < arrColumns.Count; j++)
                    {
                        GridSummaryColumnDescriptor column = arrColumns[j];
                        string colName = column.DisplayColumn != string.Empty ? column.DisplayColumn : column.DataMember;
                        if (table.Columns[i].MappingName.Equals(colName))
                        {
                            int iColumnIndex = column.ColInRecord;
                            if (iColumnIndex == -1)
                            {
                                range = sheet.Range[iRow, iColIndex];
                            }
                            else                           
                                range = sheet.Range[iRow, iColIndex + iColumnIndex];
                            if (ExportStyle)
                            {
                                style = new GridStyleInfo();
                                style = groupingGrid.GetTable(table.Name).GetTableCellStyle(summary, colName);
                                if (ExportBorders)
                                {
                                    if (!style.Borders.HasTop)
                                        style.Borders.Top = new GridBorder(GridBorderStyle.Solid, Color.Blue, GridBorderWeight.Medium);
                                    CombineGridTableBorders(style);
                                }
                                CopyStyle(style, range);
                                if (string.IsNullOrEmpty(column.Appearance.SummaryFieldCell.Format) && string.IsNullOrEmpty(style.Format) && !string.IsNullOrEmpty(column.Format))
                                {
                                    string strFormat = null;
                                    string[] strArr = null;
                                    char[] splitchar = { ':' };
                                    strArr = column.Format.Split(splitchar);
                                    if (!(column.Format.IndexOf(":") > 0))
                                    {
                                        strFormat = "0";
                                    }
                                    else
                                        strFormat = strArr[1].Replace("}", "");
                                    style.Format = strFormat;
                                }
                                else
                                    style.Text = column.GetDisplayText(summary.ParentGroup);
                            }
                            style.CellValueType = column.GetDisplayText(summary.ParentGroup).GetType();

                            ExportFormats(style, range);
                            gridCell = style;
                            e = new GridImportExportCellInfoEventArgs(range, gridCell, GridConverterAction.Export, iRow, iColIndex + iColumnIndex);
                            RaiseQueryImportExportCellInfo(e);
                            if (e.Handled)
                                CopyStyle(gridCell, range);
                            if (!ExportStyle)
                                range.Text = column.GetDisplayText(summary.ParentGroup);
                        }
                        else
                        {
                            GridTableCellStyleInfo cellStyle = groupingGrid.GetTable(table.Name).GetTableCellStyle(summary, table.Columns[i].MappingName);
                            if (colIndex == -1)
                            {
                                range = sheet.Range[iRow, iColIndex, iRow, iColIndex - colIndex];
                            }
                            else
                                range = sheet.Range[iRow, colIndex - table.GroupedColumns.Count];
                            if (ExportStyle)
                            {
                                if (ExportBorders)
                                {
                                    if (!cellStyle.Borders.HasTop)
                                        cellStyle.Borders.Top = new GridBorder(GridBorderStyle.Solid, Color.Blue, GridBorderWeight.Medium);
                                    CombineGridTableBorders(cellStyle);
                                }
                                CopyStyle(cellStyle, range);
                            }
                            cellStyle.CellValueType = column.GetDisplayText(summary.ParentGroup).GetType();

                            ExportFormats(cellStyle, range);
                            e = new GridImportExportCellInfoEventArgs(range, cellStyle, GridConverterAction.Export, summary.GetRowIndex(), colIndex - groupingGrid.TableDescriptor.GroupedColumns.Count);
                            RaiseQueryImportExportCellInfo(e);
                            if (e.Handled)
                                CopyStyle(cellStyle, range);                           
                        }

                    }
            }

            return 1;
        }

        /// <summary>
        /// Combines Borders from entire style hierarchy.
        /// </summary>
        /// <param name="style">Destination Style</param>
        private void CombineGridTableBorders(GridStyleInfo style)
        {
            if (GroupingGrid.TableOptions.HasGridLineBorder)
            {
                GridBorder border = GroupingGrid.TableOptions.GridLineBorder;
                if (!style.Borders.HasTop)
                    style.Borders.Top = border;
                if (!style.Borders.HasLeft)
                    style.Borders.Left = border;
                if (!style.Borders.HasBottom)
                    style.Borders.Bottom = border;
                if (!style.Borders.HasRight)
                    style.Borders.Right = border;
            }

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
        /// /// <param name="bStartNewGroup">Indicates whether new group should be started at index.</param>
        /// <param name="summaryRowCount">SummaryRowCount of the parent table.</param>
        private void AdjustGroupingLevel(Stack stackStartIndexes, ref int iCurLevel, int iGroupLevel,
          IWorksheet sheet, int index, bool bStartNewGroup, int summaryRowCount)
        {
            if (stackStartIndexes == null)
                throw new ArgumentNullException("stackStartIndexes");

            if (sheet == null)
                throw new ArgumentNullException("sheet");

            if (iCurLevel < 0 || iCurLevel < iGroupLevel)
            {
                iCurLevel = iGroupLevel;
                GroupInfo gi = new GroupInfo(index, summaryRowCount);
                stackStartIndexes.Push(gi);
                //stackStartIndexes.Push( index );
            }
            else if (iCurLevel >= iGroupLevel)
            {
                int iLastLevel = bStartNewGroup ? iGroupLevel : iGroupLevel + 1;
                bool groupPreview = GroupingGrid.ChildGroupOptions.ShowGroupPreview;
                for (; iCurLevel >= iLastLevel; iCurLevel--)
                {
                    GroupInfo gi = (GroupInfo)stackStartIndexes.Pop();
                    //int iStartIndex = ( int )stackStartIndexes.Pop() + 1;
                    int iStartIndex = (int)gi.StartIndex + 1;

                    int iEndIndex = index;

                    if (iStartIndex != iEndIndex)
                    {
                        int ix = index;
                        if (iGroupLevel == 0)
                            ix = index - gi.SummaryRowCount * iCurLevel;
                        else
                            if (iCurLevel < iLastLevel)
                                ix = index - gi.SummaryRowCount;
                        if (groupPreview)
                            ix--;

                        // Fix:3852 ArgumentOutOfRangeException thrown on exporting groupedgrid with CounterLogic set to options other than All
                        if (ix >= iStartIndex + 1)
                            sheet.Range[iStartIndex + 1, 1, ix/*index*/, 1].Group(ExcelGroupBy.ByRows, false);
                    }
                }

                if (bStartNewGroup)
                {
                    iCurLevel++;
                    GroupInfo gi = new GroupInfo(index, summaryRowCount);
                    stackStartIndexes.Push(gi);
                    //stackStartIndexes.Push( index );
                }
            }
        }

        /// <summary>
        /// Holds information about summary row count in a group that need to be calculated
        /// while grouping in rows in worksheet.
        /// </summary>
        internal class GroupInfo
        {
            int startIndex;
            int summaryRowCount;
            public GroupInfo(int startIndex, int summaryRowCount)
            {
                this.startIndex = startIndex;
                this.summaryRowCount = summaryRowCount;
            }

            public int StartIndex
            {
                get { return startIndex; }
            }
            public int SummaryRowCount
            {
                get { return summaryRowCount; }
            }
        }

        static int recRowIndex = 0;
        /// <summary>
        /// Exports record row into excel worksheet.
        /// </summary>
        /// <param name="row">RecordRow to export.</param>
        /// <param name="sheet">Destination worksheet.</param>
        /// <param name="index">Zero-based index to first row of the record.</param>
        /// <param name="options">Converter options.</param>
        /// <param name="iIndexInGroup"></param>
        /// <param name="iGroupLevel">GroupLevel.</param>
        /// <param name="iSkipRecordRows">Skiped RecordRow Count.</param>
        /// <returns>Number of exported records.</returns>
        private int ExportRecordRow(RecordRow row, IWorksheet sheet, int index, ConverterOptions options
          , int iIndexInGroup, int iGroupLevel, out int iSkipRecordRows)
        {
            if (row == null)
                throw new ArgumentNullException("row");

            if (sheet == null)
                throw new ArgumentNullException("sheet");

            if (index < 0)
                throw new ArgumentOutOfRangeException("index", index, "Value can not be less 0");

            if (iIndexInGroup < 0)
                throw new ArgumentOutOfRangeException("iIndexInGroup", iIndexInGroup, "Value can not be less than 0");

            FieldDescriptorCollection arrFields = row.ParentTable.TableDescriptor.Fields;
            int iRowIndex = index + 1;

            GridTableDescriptor table = (GridTableDescriptor)row.ParentTableDescriptor;
            GridColumnDescriptorCollection arrColumns = table.Columns;
            GridVisibleColumnDescriptorCollection arrVisibleColumns = table.VisibleColumns;
            GridColumnSetDescriptorCollection arrColumnSets = table.ColumnSets;
            Record rec = Element.GetRecord(row);

            GridTableOptionsStyleInfo tableStyleInfo = table.TableOptions;
            sheet.SetRowHeightInPixels(iRowIndex, tableStyleInfo.RecordRowHeight);
            
            //GridTableCellStyleInfo style = ( iIndexInGroup % 2 == 0 )
            //  ? table.Appearance.RecordFieldCell
            //  : table.Appearance.AlternateRecordFieldCell;

            int iRowCount = 1;

            for (int i = 0,iColumnIndex = iGroupLevel, len = arrVisibleColumns.Count; i < len; i++, iColumnIndex++)
            {
                //int iColumnIndex = iGroupLevel + i;
                GridVisibleColumnDescriptor column = arrVisibleColumns[i];
                string strColumnName = column.Name;

                if (arrColumnSets.Contains(strColumnName))
                {
#if ASPNET
                    GridColumnSetDescriptor columnSet = arrColumnSets.GetColumnSetDescriptor(strColumnName);
#else
                    GridColumnSetDescriptor columnSet = arrColumnSets[strColumnName];
#endif
                    int iUsedRows = ExportColumnSet(columnSet, sheet, iIndexInGroup,
                       iRowIndex, ref iColumnIndex, arrFields, row, table);

                    if (iUsedRows > iRowCount) iRowCount = iUsedRows;
                }
                else
                {

#if ASPNET
                    GridColumnDescriptor gridColumn = arrColumns.GetColumnDescriptor(strColumnName);
#else
                    GridColumnDescriptor gridColumn = arrColumns[strColumnName];
#endif
                    if (gridColumn != null)
                    {
                        FieldDescriptor field = arrFields[gridColumn.MappingName];
                        if (field != null)
                        {
                            if (((options & ConverterOptions.RowHeaders) == ConverterOptions.RowHeaders) && (recRowIndex != index))
                                iColumnIndex = ExportRowHeader(table, sheet, iRowIndex, iColumnIndex);
                            recRowIndex = index;

                            IRange range = sheet.Range[iRowIndex, iColumnIndex];
                            object value = rec.GetValue(field);

                            //Code to export StyleInfo Format set
                            GridTableCellStyleInfo style1 = tableDictionary.GetGridTable(table.Name).GetTableCellStyle(rec.GetRecordDisplayElement(), field.Name);
                            

                            ////Export DisplayMember value
                            if (gridColumn.Appearance.AnyRecordFieldCell.DisplayMember != string.Empty)
                            {
                                GridTableModel tm = tableDictionary.GetGridTable(table.Name).TableModel;
                                GridTableCellStyleInfo cellStyle = gridColumn.Appearance.AnyRecordFieldCell;
                                value = tm.CellModels[cellStyle.CellType].GetFormattedText(cellStyle, value, GridCellBaseTextInfo.DisplayText);
                            } //ForeignKeyCell
                            else if (gridColumn.Appearance.AnyRecordFieldCell.CellType == "ForeignKeyCell")
                                value = GetForeignKeyCellText(gridColumn.Appearance.AnyRecordFieldCell, field, rec, value);
                            else if (style1.CellType == GridCellTypeName.CheckBox || style1.CellType == GridCellTypeName.RadioButton || style1.CellType == GridCellTypeName.PushButton)
                                value = style1.Description;
							else
                                value = (value == null) ? string.Empty : value;

                            try
                            {
                                if (gridColumn.Appearance.AnyRecordFieldCell.CellType == GridCellTypeName.TextBox ||
                                    gridColumn.Appearance.AnyRecordFieldCell.CellType == GridCellTypeName.OriginalTextBox)
                                {
                                    double result = 0;
                                    if (Double.TryParse(value.ToString(), out result))
                                        range.Value2 = result;
                                    else
                                        range.Value2 = value;

                                    if (!range.HasNumber)
                                    {
                                        range.Text = Convert.ToString(value);
                                    }
                                    else if (range.HasNumber && string.IsNullOrEmpty(style1.Format)
                                        && style1.CellValueType == typeof(string) && value.ToString().ToUpper().IndexOf('E') != -1 || value.ToString().StartsWith("+"))
                                    {
                                        range.NumberFormat = "text";
                                        range.Text = Convert.ToString(value);
                                        range.IgnoreErrorOptions = ExcelIgnoreError.NumberAsText; //ignore error
                                    }
                                    else
                                    {
                                        if (style1.Format != string.Empty)
                                        {
                                            if (style1.Format.StartsWith("P") && style1.FormattedText.Contains("%"))
                                            {
                                                range.NumberFormat += "%";
                                            }
                                            System.Globalization.NumberFormatInfo nfi = (style1.CurrencyEdit != null) ? style1.CurrencyEdit.NumberFormatInfoObject : System.Globalization.CultureInfo.CurrentCulture.NumberFormat;
                                            range.Value2 = result;
                                        }
                                    }
                                }
                                else
                                {
                                    range.Value2 = value;
                                }
                                if (style1.Format != string.Empty)
                                {
                                    if (style1.Format.StartsWith("N"))
                                    {
                                        string substr = style1.Format.Replace("N", string.Empty);
                                        double digit;
                                        if (double.TryParse(substr, out digit))
                                        {
                                            string format = "0.";
                                            for (int j = 0; j < digit; j++)
                                                format = string.Concat(format, "0");
                                            range.NumberFormat = format;
                                        }
                                    }
                                    else
                                        range.NumberFormat = style1.Format;
                                }
                            }
                            catch
                            {
                                Debug.WriteLine("Exporting Cell: [" + iRowIndex + "]" + "[" + iColumnIndex + "]" + " failed.");
                                range.Value2 = "#VALUE!";
                            }
                            if (ExportStyle)
                            {
                                GridStyleInfo newStyle = new GridStyleInfo();
                                //Combine Column Styles.
                                newStyle = tableDictionary.GetGridTable(table.Name).GetTableCellStyle(rec, field.Name);

                                if (ExportBorders)
                                    CombineGridTableBorders(newStyle);

                                ////Do any cell specific customizations:
                                if (newStyle.CellType == "Currency")
                                {
                                    System.Globalization.NumberFormatInfo nfi = (newStyle != null) ? newStyle.CurrencyEdit.NumberFormatInfoObject : System.Globalization.CultureInfo.CurrentCulture.NumberFormat;
                                    Double val;
                                    if (newStyle.CellValue != null && newStyle.CellValue.ToString() != string.Empty)
                                    {
                                        val = Convert.ToDouble(newStyle.CellValue, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                                        range.Number = val;
                                    }
                                    else
                                        val = 0;

                                    string valString = val.ToString("C", nfi);

                                    String FormatString = "#,##0.";
                                    for (int digits = 0; digits < nfi.CurrencyDecimalDigits; digits++)
                                        FormatString += "0";
                                    if (Char.IsNumber(valString, 0))
                                        FormatString += " [$" + nfi.CurrencySymbol + "-411]";
                                    else
                                        FormatString = "[$" + nfi.CurrencySymbol + "-411]" + FormatString;
                                    if (nfi.CurrencyGroupSizes.Length == 2 &&
                                        nfi.CurrencyGroupSizes[0] == 3 &&
                                        nfi.CurrencyGroupSizes[1] == 2)
                                    {
                                        range.CellStyle.NumberFormat = CustomFormatString(range.Number, nfi);
                                    }
                                    else
                                    {
                                        range.CellStyle.NumberFormat = FormatString;
                                    }
                                }

                                CopyStyle(newStyle, range);
                            }
                            GridStyleInfo gridCell = tableDictionary.GetGridTable(table.Name).GetTableCellStyle(rec, field.Name);

                            if (gridCell.CellType == GridCellTypeName.Image && ExportImage && ExportStyle)
                            {
                                ExportImageToExcelCell(range, gridCell);
                            }
                            GridImportExportCellInfoEventArgs e = new GridImportExportCellInfoEventArgs(range, gridCell, GridConverterAction.Export, iRowIndex, iColumnIndex);
                            RaiseQueryImportExportCellInfo(e);
                            if (e.Handled)
                                CopyStyle(gridCell, range);
                        }
                    }
                }
            }

            iSkipRecordRows = iRowCount - 1;

            
            bool bVisible = ((ConverterOptions.Visible & options) != 0);

            //Fix: Collapsed (non visible) NestedTable elements also exported.
            //if (bVisible & !rec.IsExpanded) return iRowCount;
            if (bVisible) return iRowCount;

            NestedTablesCollection nestedTables = rec.NestedTables;
            int iCount = nestedTables.Count;

            if (iCount > 0) iGroupLevel++;

            for (int i = 0; i < iCount; i++)
            {
                //        NestedTable nestedTable = nestedTables[ i ];
                //        IList elements = nestedTable.ChildTable.NestedDisplayElements;
                //        iRowCount += ExportElements( elements, sheet, index + iRowCount, options );
                GridNestedTable nestedTable = (GridNestedTable)nestedTables[i];

                IList elements = nestedTable.ChildTable.Elements;

                int startIndex = index + iRowCount + 1;

                int cur = iCurLevel;

                iCurLevel = -1;
               
                if (!bVisible)
                {
                    foreach (Element el in elements)
                    {
                        if (el is GridSummaryRow || el is GridCaptionRow)
                            if (!el.ParentGroup.IsExpanded)
                                if (!expandedList.ContainsKey(el))
                                    expandedList.Add(el, el.ParentGroup.IsExpanded);
                    }
                }
                iRowCount += ExportElements(elements, sheet, index + iRowCount, options, iGroupLevel);
                if (!bVisible)
                {
                    foreach (Element el in elements)
                    {
                        if (el is GridSummaryRow || el is GridCaptionRow)
                            if (expandedList.ContainsKey(el))
                            {
                                el.ParentGroup.IsExpanded = false;
                            }
                    }
                }
                iCurLevel = cur;

                //Export Nested Tables as Excel Groups
                if (ExportRecordPlusMinus)
                    sheet.Range[startIndex, 1, index + iRowCount, 1].Group(ExcelGroupBy.ByRows, false);
            }

            return iRowCount;
        }

        string CustomFormatString(double number, System.Globalization.NumberFormatInfo nfi)
        {
            string format = "";
            int length = number.ToString().IndexOf(".") != -1 ? number.ToString().IndexOf(".") : number.ToString().Length;
            format = "\"" + nfi.CurrencySymbol + "\"";
            if (length > 3)
            {
                length -= 3;
                if (length % 2 != 0)
                {
                    format += @"#\" + nfi.CurrencyGroupSeparator;
                    length -= 4;
                }
                for (int i = 0; i < length - 1; i += 2)
                {
                    format += @"##\" + nfi.CurrencyGroupSeparator;
                }
                format += "##0.";
                for (int digits = 0; digits < nfi.CurrencyDecimalDigits; digits++)
                    format += "0";
            }
            return format;
        }

        /// <summary>
        /// Exports column set into excel.
        /// </summary>
        /// <param name="columnSet">Column set to export.</param>
        /// <param name="sheet">Worksheet to export data into.</param>
        /// <param name="iIndexInGroup"></param>
        /// <param name="iRowIndex">Row index to export to.</param>
        /// <param name="iColumnIndex">Column index to export to.</param>
        /// <param name="arrFields">FieldDescriptorCollections.</param>
        /// <param name="row">RecordRow.</param>
        /// <param name="table">GridTableDescriptor.</param>
        /// <returns>Number of rows used by column set.</returns>
        private int ExportColumnSet(GridColumnSetDescriptor columnSet, IWorksheet sheet,
          int iIndexInGroup, int iRowIndex, ref int iColumnIndex,FieldDescriptorCollection arrFields,
            RecordRow row, GridTableDescriptor table)
        {
            GridColumnSpanDescriptorCollection arrColumnSpans = columnSet.ColumnSpans;
            Record rec = Element.GetRecord(row);
            int iRowCount = 1;
            int iMaxColumn = 0;

            for (int i = 0, len = arrColumnSpans.Count; i < len; i++)
            {
                GridColumnSpanDescriptor columnSpan = arrColumnSpans[i];
                GridRangeInfo gridRange = columnSpan.Range;

                int iSpanRowIndex = iRowIndex + gridRange.Top;
                int iSpanColumnIndex = iColumnIndex + gridRange.Left;

                //IRange range = sheet.Range[iSpanRowIndex, iSpanColumnIndex];
                IRange range = sheet.Range[iSpanRowIndex, iSpanColumnIndex,
                      iRowIndex + gridRange.Bottom, iColumnIndex + gridRange.Right];
                range.Merge();

                string strColumnName = columnSpan.Name;
#if ASPNET
                GridColumnDescriptor gridColumn = table.Columns.GetColumnDescriptor(strColumnName);
#else
                GridColumnDescriptor gridColumn = table.Columns[strColumnName];
#endif
                FieldDescriptor field = arrFields[gridColumn.MappingName];

                object value = rec.GetValue(field);

                ////Export DisplayMember value
                if (gridColumn.Appearance.AnyRecordFieldCell.DisplayMember != string.Empty)
                {
                    GridTableModel tm = tableDictionary.GetGridTable(table.Name).TableModel;
                    GridTableCellStyleInfo cellStyle = gridColumn.Appearance.AnyRecordFieldCell;
                    range.Value2 = tm.CellModels[cellStyle.CellType].GetFormattedText(cellStyle, value, GridCellBaseTextInfo.DisplayText);
                } //ForeignKeyCell
                else if (gridColumn.Appearance.AnyRecordFieldCell.CellType == "ForeignKeyCell")
                {
                    range.Value2 = value == null ? string.Empty :
                                   GetForeignKeyCellText(gridColumn.Appearance.AnyRecordFieldCell, field, rec, value);
                }
                else
                    range.Value2 = (value == null) ? string.Empty : value;


                if (ExportStyle)
                {
                ////Set Style...
                GridStyleInfo style = new GridStyleInfo();

                //Combine Column Styles.
                    if (iIndexInGroup % 2 == 0)
                        CombineStyles(style, gridColumn.Appearance.RecordFieldCell);
                    else
                        CombineStyles(style, gridColumn.Appearance.AlternateRecordFieldCell);
                    if (ExportBorders)
                        CombineGridTableBorders(style);
                CopyStyle(style, range);
                }
                GridStyleInfo gridCell = tableDictionary.GetGridTable(table.Name).GetTableCellStyle(rec, field.Name);
                GridImportExportCellInfoEventArgs e = new GridImportExportCellInfoEventArgs(range, gridCell, GridConverterAction.Export, iRowIndex, iColumnIndex);
                RaiseQueryImportExportCellInfo(e);
                if (e.Handled)
                    CopyStyle(gridCell, range);
                iRowCount = Math.Max(gridRange.Bottom + 1, iRowCount);
                iMaxColumn = Math.Max(gridRange.Right, iMaxColumn);
            }

            iColumnIndex += iMaxColumn;
            return iRowCount;
        }


        /// <summary>
        /// Export cell style.
        /// </summary>
        /// <param name="sourceElement">Source element to export.</param>
        /// <param name="sourceField">Source field descriptor.</param>
        /// <param name="destRange">Destination range.</param>
        /// <param name="options">Convert options.</param>
        private void ExportCellStyle(Element sourceElement, FieldDescriptor sourceField, IRange destRange,
          ConverterOptions options)
        {
            if (destRange == null)
                throw new ArgumentNullException("destRange");

            if (sourceElement == null)
                throw new ArgumentNullException("sourceElement");

            GridTable table = sourceElement.ParentTable as GridTable;

            if (table == null)
            {
                Debug.WriteLine("Bad table type");
                return;
            }

            GridTableCellStyleInfo style;

            //if( options == ConverterOptions.ConvertVisible )
            {
                string strFieldName = sourceField != null ? sourceField.Name : string.Empty;
                style = table.GetTableCellStyle(sourceElement, strFieldName);
            }
            //      else // if( options == ConverterOptions.ConvertAll )
            //      {
            //        style = table.Appearance.AnyRecordFieldCell;
            //      }

            CopyStyle(style, destRange);
        }
        /// <summary>
        /// Exports caption row.
        /// </summary>
        /// <param name="captionRow">Source caption row.</param>
        /// <param name="sheet">Destionation worksheet.</param>
        /// <param name="index">Capiton's zero-based row index in the worksheet.</param>
        /// <param name="options">Convert options.</param>
        /// <param name="iCurrentGroupLevel">Current group level.</param>
        /// <param name="stackStartIndexes">A stack object to maintain the start group indexes.</param>
        /// <param name="summaryRowCount">SummaryRowCount.</param>
        /// <returns>Zero-based row index that point to the row after caption.</returns>
        private int ExportCaption(GridCaptionRow captionRow, IWorksheet sheet, int index,
          ConverterOptions options, int iCurrentGroupLevel, Stack stackStartIndexes, int summaryRowCount)
        {
            if (captionRow == null)
                throw new ArgumentNullException("captionRow");

            if (sheet == null)
                throw new ArgumentNullException("sheet");

            if (index < 0)
                throw new ArgumentOutOfRangeException("index", index, "Value can not be less 0");

            IRange range = sheet.Range[index + 1, iCurrentGroupLevel];

            string s = GetCaptionText(captionRow);

            GridQueryCaptionTextEventArgs e = new GridQueryCaptionTextEventArgs(captionRow, s);
            RaiseQueryCaptionText(e);

            range.Text = e.Text;
            // range.Text = GetCaptionText(captionRow);


            ////Fix: caption in the inner most nested table has an additional column merged.
            //GridTableModel tableModel = captionRow.ParentTable.TableModel;
            //int iRealColCount = tableModel.ColCount - tableModel.GetColumnIndentCount();
            int iRowIndex = index + 1;

            GridTableDescriptor table = (GridTableDescriptor)captionRow.ParentTableDescriptor;

            GridTableOptionsStyleInfo tableStyleInfo = table.TableOptions;
            sheet.SetRowHeightInPixels(iRowIndex, tableStyleInfo.CaptionRowHeight);

            if (ExportStyle || hasCaptionBKColor)
            {
                GridStyleInfo style = new GridStyleInfo();
                if (ExportStyle)
                style.CopyFrom(captionRow.Appearance.GroupCaptionCell);
            //Check for custom export CaptionBack Color
            if (hasCaptionBKColor)
                style.BackColor = CaptionBackColor;
                else
                    style.BackColor = captionRow.Appearance.GroupCaptionCell.BackColor;

                style.Borders.All = new GridBorder(GridBorderStyle.None);
                CopyStyle(style, range);
            }
            
            
            int colSetDiff = 0;
            foreach (GridColumnSetDescriptor columnSet in table.ColumnSets)
                colSetDiff += columnSet.GetColCount() - 1;

            //int colSetDiff = (table.ColumnSets.Count > 0 ? 1 : 0) - 1;
            int iRealColCount = table.VisibleColumns.Count + colSetDiff -1;
            if ((options & ConverterOptions.RowHeaders) == ConverterOptions.RowHeaders)
                iRealColCount++;

            if (iRealColCount > 0)
            {
                sheet.Range[iRowIndex, iCurrentGroupLevel, iRowIndex, iCurrentGroupLevel + iRealColCount].Merge();
            }

            //Export Groups as Excel Groups
            if (ExportGroupPlusMinus)
            {
                int iGroupLevel = captionRow.GroupLevel;
                AdjustGroupingLevel(stackStartIndexes, ref iCurLevel/*ref iCurrentGroupLevel*/, iGroupLevel, sheet, index, true, summaryRowCount);
            }

            return 1;
        }

        /// <summary>
        /// Exports caption summary row.
        /// </summary>
        /// <param name="captionRow">Source caption row.</param>
        /// <param name="sheet">Destionation worksheet.</param>
        /// <param name="index">Capiton's zero-based row index in the worksheet.</param>
        /// <param name="options">Convert options.</param>
        /// <param name="iCurrentGroupLevel">Current group level.</param>
        /// <param name="stackStartIndexes">A stack object to maintain the start group indexes.</param>
        /// <param name="summaryRowCount">SummaryRowCount.</param>
        /// <param name="childTableGroupLevel">Child Table Group level.</param>
        /// <param name="style">Caption summary row style.</param>
        /// <returns>Zero-based row index that point to the row after caption.</returns>
        private int ExportCaptionSummaryRow(GridCaptionRow captionRow, IWorksheet sheet, int index,
          ConverterOptions options, int iCurrentGroupLevel, Stack stackStartIndexes, int summaryRowCount, int childTableGroupLevel, GridStyleInfo style)
        {
            if (captionRow == null)
                throw new ArgumentNullException("captionSummaryRow");

            if (sheet == null)
                throw new ArgumentNullException("sheet");

            if (index < 0)
                throw new ArgumentOutOfRangeException("index", index, "Value can not be less 0");

            int iRowIndex = index + 1;

            GridTableDescriptor table = (GridTableDescriptor)captionRow.ParentTableDescriptor;

            GridTableOptionsStyleInfo tableStyleInfo = table.TableOptions;
            sheet.SetRowHeightInPixels(iRowIndex, tableStyleInfo.CaptionRowHeight);
            #pragma warning disable
            IRange range = null,captionTextRange = null,captionRange = null;
            #pragma warning enable
            string summaryRow = groupingGrid.GetTableDescriptor(table.Name).TopLevelGroupOptions.CaptionSummaryRow;
            if (iCurrentGroupLevel > 1)
                summaryRow = groupingGrid.NestedTableGroupOptions.CaptionSummaryRow;
            if(childTableGroupLevel > 0)
                summaryRow = groupingGrid.GetTableDescriptor(table.Name).ChildGroupOptions.CaptionSummaryRow;

            GridTableDescriptor tblDescriptor = captionRow.ParentTableDescriptor;
            GridSummaryRowDescriptor sumRowDescriptor = tblDescriptor.SummaryRows[summaryRow];
            for (int i = 1; i < tblDescriptor.VisibleColumns.Count; i++)
            {
                GridSummaryColumnDescriptor sumColDescriptor = null;
                string colName = tblDescriptor.VisibleColumns[i].Name;
                int capIndex = groupingGrid.TableModel.NameToColIndex(colName);                                
                GridStyleInfo stylecell = groupingGrid.GetTable(table.Name).GetTableCellStyle(captionRow, colName);
                if (sumRowDescriptor != null && string.IsNullOrEmpty(stylecell.Format))
                {
                    foreach (GridSummaryColumnDescriptor sumColDesc in sumRowDescriptor.SummaryColumns)
                    {
                        if (sumColDesc.DisplayColumn.Equals(colName))
                        {
                            string strFormat = null;
                            string[] strArr = null;
                            char[] splitchar = { ':' };
                            strArr = sumColDesc.Format.Split(splitchar);
                            if (!(sumColDesc.Format.IndexOf(":") > 0))
                            {
                                strFormat = "0";
                            }
                            else
                                strFormat = strArr[1].Replace("}", "");
                            stylecell.Format = strFormat;
                        }
                    }
                }
                range = sheet.Range[iRowIndex, capIndex - tblDescriptor.GroupedColumns.Count - (tblDescriptor.Columns.IndexOf(colName) - tblDescriptor.VisibleColumns.IndexOf(colName))];//ColIndex based on Hidden columns.
                ExportFormats(stylecell, range);
                CopyStyle(stylecell, range);
                  
                GridStyleInfo styleinfo = groupingGrid.GetTable(table.Name).GetTableCellStyle(captionRow.GetRowIndex(), iCurrentGroupLevel + captionRow.GroupLevel);
                range = sheet.Range[iRowIndex, iCurrentGroupLevel];
                ExportFormats(styleinfo, range);               
                if (ExportStyle)
                {
                    styleinfo.Borders.All = new GridBorder(GridBorderStyle.None);                    
                }  
                CopyStyle(styleinfo, range); 
                GridImportExportCellInfoEventArgs e = new GridImportExportCellInfoEventArgs(range, stylecell, GridConverterAction.Export, iRowIndex, capIndex);
                RaiseQueryImportExportCellInfo(e);
                if (e.Handled)
                    CopyStyle(stylecell, range);
            }

            //Export Groups as Excel Groups
            if (ExportGroupPlusMinus)
            {
                int iGroupLevel = captionRow.GroupLevel;
                AdjustGroupingLevel(stackStartIndexes, ref iCurLevel/*ref iCurrentGroupLevel*/, iGroupLevel, sheet, index, true, summaryRowCount);
            }

            return 1;
        }

        /// <summary>
        /// Sets number format index for a range from specified grid style.
        /// </summary>
        /// <param name="range">Range to add format to.</param>
        /// <param name="style">Style to get number format from.</param>
        private void ExportFormats(GridStyleInfo style, IRange range)
        {
            switch (style.CellType)
            {
                case "Image":
                    if (ExportImage && ExportStyle)
                        ExportImageToExcelCell(range, style);
                    break;
                case "PushButton":
                case "CheckBox":
                case "RadioButton":
                    range.Text = style.Description;
                    break;
                case "ComboBox":
                case "RichText":
                    range.Text = style.FormattedText;
                    break;

                case "ProgressBar":
                    GridProgressBarInfo progressBar = style.ProgressBar;
                    int iValue = progressBar.ProgressValue;

                    if (progressBar.TextStyle == Syncfusion.Windows.Forms.Tools.ProgressBarTextStyles.Percentage)
                    {

                        int iMin = progressBar.Minimum;
                        int iMax = progressBar.Maximum;
                        int iRange = iMax - iMin;
                        int iPercent = Math.Min(100, iValue * 100 / iRange);
                        range.Number = iPercent / 100.0;
                        range.NumberFormat = "0%";
                    }
                    else
                    {
                        range.Number = iValue;
                    }
                    break;

                case "Currency":

                    System.Globalization.NumberFormatInfo nfi = (style != null) ? style.CurrencyEdit.NumberFormatInfoObject : System.Globalization.CultureInfo.CurrentCulture.NumberFormat;
                    Double val;
                    if (style.CellValue.ToString() != string.Empty)
                    {
                        val = double.Parse(style.CellValue.ToString(), NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint);
                        range.Number = val;
                    }
                    else
                        val = 0;

                    string valString = val.ToString("C", nfi);

                    String FormatString = "#,##0.";
                    for (int i = 0; i < nfi.CurrencyDecimalDigits; i++)
                        FormatString += "0";
                    if (Char.IsNumber(valString, 0))
                        FormatString += " [$" + nfi.CurrencySymbol + "-411]";
                    else
                        FormatString = "[$" + nfi.CurrencySymbol + "-411]" + FormatString;
                    range.CellStyle.NumberFormat = FormatString;
                    break;

                default:
                    object objValue = style.CellValue;

                    if (objValue is DateTime)
                    {
                        range.DateTime = (DateTime)objValue;
                    }
                    else if (objValue is string || !string.IsNullOrEmpty(style.Format ))
                    {
                        string strText = style.Text;
                        double value = 0;

                        if (style.CellType == GridCellTypeName.FormulaCell && strText.Length != 0
                    && style.Text[0] == '=')
                        {
                            range.Formula = strText;
                        }
                        else if (double.TryParse(strText, out value) && string.IsNullOrEmpty(style.Format) && strText.ToUpper().IndexOf('E') != -1)
                        {
                            range.NumberFormat = "text";
                            range.Text = strText;
                            range.IgnoreErrorOptions = ExcelIgnoreError.NumberAsText; //ignore error
                        }
                        else if (!string.IsNullOrEmpty(style.Format) && (style.CellValueType==null||style.CellValueType!=null))
                        {
                            if (style.Format.StartsWith("P"))
                            {
                                string substr = style.Format.Replace("P", string.Empty);
                                double digit;
                                if (double.TryParse(substr, out digit))
                                {
                                    range.NumberFormat = FormatConcat(digit, range, style)+"%";
                                    range.Value2 = style.Text;
                                } 
                                else
                                {
                                    range.Value2 = objValue;
                                    range.NumberFormat = "0.00%";                                   
                                }                              
                               
                            }
                            else if (style.Format.StartsWith("#") && style.Format.Contains("%"))
                            {
                                string substr = style.Format.Replace("#", string.Empty);
                                double digit;
                                if (double.TryParse(substr, out digit))
                                {
                                    range.NumberFormat = FormatConcat(digit, range, style) + "%";
                                    range.Value2 = style.Text;
                                }
                                else
                                {
                                    range.Value2 = objValue;
                                    range.NumberFormat = "0%";                                    
                                }

                            }
                            else if (style.Format.StartsWith("C"))
                            {
                                System.Globalization.NumberFormatInfo nfo = (style != null) ? style.CurrencyEdit.NumberFormatInfoObject : System.Globalization.CultureInfo.CurrentCulture.NumberFormat;
                                string substr = style.Format.Replace("C", string.Empty);
                                double digit;
                                if (double.TryParse(substr, out digit))
                                {
                                    range.NumberFormat =  "[$" + nfo.CurrencySymbol + "-411]" + FormatConcat(digit, range, style);
                                    range.Value2 = style.Text;
                                }
                                else
                                {
                                    range.Value2 = objValue;
                                    range.NumberFormat = "[$" + nfo.CurrencySymbol + "-411]" + ".00";                                    
                                }                                
                            }
                            else if (style.Format.StartsWith("N"))
                            {
                                string substr = style.Format.Replace("N", string.Empty);
                                double digit;
                                if (double.TryParse(substr, out digit))
                                {                                    
                                    range.NumberFormat = FormatConcat(digit, range, style);
                                    range.Value2 = objValue;
                                }
                                else
                                {
                                    range.Value2 = objValue;
                                    range.NumberFormat = "0";
                                }

                            }
                            else if (style.Format.StartsWith("E"))
                            {
                                string substr = style.Format.Replace("E", string.Empty);
                                double digit;
                                if (double.TryParse(substr, out digit))
                                {
                                    range.NumberFormat = FormatConcat(digit, range, style) + "E+000";
                                    range.Value2 = objValue;
                                }
                                else
                                {
                                    range.Value2 = objValue;
                                    range.NumberFormat = "0.0000E+00";
                                }
                            }
                            else
                            {
                                range.Value2 = objValue;
                                range.NumberFormat = style.Format;
                            }
                        }
                        else
                        {
                            if (style.CellValueType == null)
                                range.Text = strText;
                            else
                                range.Value2 = objValue;
                        }
                    }
                    else if (objValue is ulong || objValue is ushort)
                    {
                        range.Text = objValue.ToString();
                        range.IgnoreErrorOptions = ExcelIgnoreError.NumberAsText;
                    }                    
                    else if ((style.CellValueType == typeof(double) || style.CellValueType == typeof(decimal) || 
                        style.CellValueType == typeof(int) || style.CellValueType == typeof(Int16) || style.CellValueType == typeof(Int32) || style.CellValueType == typeof(Int64) || style.CellValueType == typeof(long)) && string.IsNullOrEmpty(style.Format))
                    {
                        range.Value2 = objValue;
                        range.NumberFormat = "0";
                    }
                    else
                        range.Value2 = objValue;
                    break;
            }
        }
        private string FormatConcat(double digit, IRange range, GridStyleInfo style)
        {
            string format = string.Empty;
            if (digit > 0)
                format = "0.";
            else
                format = "0";
            for (int j = 0; j < digit; j++)
                format = string.Concat(format, "0");
            return format;
        }
        /// <summary>
        /// Exports group to the excel worksheet.
        /// </summary>
        /// <param name="group">Group to export.</param>
        /// <param name="sheet">Destination worksheet.</param>
        /// <param name="index">Zero-based row index.</param>
        /// <param name="iGroupLevel">Element GroupLevel.</param>
        /// <returns>Zero-based row index that point to the row after group rows.</returns>
        private int ExportGroup(GridGroup group, IWorksheet sheet, int index, int iGroupLevel)
        {
            if (group == null)
                throw new ArgumentNullException("group");

            if (sheet == null)
                throw new ArgumentNullException("sheet");

            if (index < 0)
                throw new ArgumentOutOfRangeException("index", index, "Value can not be less 0");

            RecordsInDetailsCollection arrRecords = group.Records;
            return ExportElements(arrRecords, sheet, index, ConverterOptions.Default, iGroupLevel);
        }
        /// <summary>
        /// Exports nested table.
        /// </summary>
        /// <param name="table">Source table to export.</param>
        /// <param name="sheet">Destination worksheet.</param>
        /// <param name="index">Capiton's zero-based row index in the worksheet.</param>
        /// <param name="options">Convert options.</param>
        /// <param name="iGroupLevel">GroupLevel.</param>
        /// <returns>Zero-based row index that point to the row after caption.</returns>
        private int ExportNestedTable(NestedTable table, IWorksheet sheet, int index,
          ConverterOptions options, int iGroupLevel)
        {
            IList elements = null;
            ChildTable childTable = table.ChildTable;

            elements = ((options & ConverterOptions.Visible) != 0)
              ? (IList)childTable.DisplayElements
              : (IList)childTable.Elements;

            return ExportElements(elements, sheet, index, options, iGroupLevel);
        }


        /// <summary>
        /// Exports preview row into excel worksheet.
        /// </summary>
        /// <param name="element">PreviewRow element to export.</param>
        /// <param name="sheet">Destination worksheet.</param>
        /// <param name="index">Index of the preview row element in the Elements collection.</param>
        /// <param name="iRowIndex">Row index to export to.</param>
        private int ExportPreviewRow(Element element, IWorksheet sheet, int index, int iRowIndex)
        {

            GridTable table = element.ParentTable as GridTable;
            //System.Reflection.FieldInfo fi = typeof(Syncfusion.Grouping.Table).GetField("tableId", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            int tableLevel = tableDictionary.GetTableLevel(table) + 1;//(int)fi.GetValue(table);
            int rowIndex = table.Elements.IndexOf(element);
            iRowIndex++;
            //int colSetDiff = (table.TableDescriptor.ColumnSets.Count > 0 ? 1 : 0) - 1;
            int colSetDiff = 0;
            foreach (GridColumnSetDescriptor columnSet in table.TableDescriptor.ColumnSets)
                colSetDiff += columnSet.GetColCount() - 1;

            IRange previewRange = sheet.Range[index + iRowIndex, tableLevel, index + iRowIndex, table.TableDescriptor.VisibleColumns.Count + tableLevel + colSetDiff - 1];
            if (element.Kind != DisplayElementKind.FilterBar)
                previewRange.Merge();
            if (!ExportPreviewRows && (element.Engine.CounterLogic == EngineCounters.All) && !element.IsFilterBar())
            {
                sheet.SetRowHeightInPixels(index + iRowIndex, 0);
                return 1;
            }
            GridStyleInfo paraStyle = new GridStyleInfo();
            if ((options & ConverterOptions.Visible) == ConverterOptions.Visible)
            {
                rowIndex = table.DisplayElements.IndexOf(element);
                if (ExportStyle)
                paraStyle = table.GetTableCellStyle(rowIndex, element.GroupLevel + 1);
            }
            else
            {
                if(ExportStyle)
                paraStyle.CopyFrom(table.Appearance.AnyPreviewCell);
            }

            //if (ExportBorders && GroupingGrid.TableOptions.HasGridLineBorder)
            //    paraStyle.Borders.All = GroupingGrid.TableOptions.GridLineBorder;

            GroupingGridExportPreviewRowQueryInfoEventArgs e = new GroupingGridExportPreviewRowQueryInfoEventArgs(element, paraStyle);
            RaiseQueryExportPreviewRowInfo(e);
            if (e.Handled)
            {
                if (e.Result)
                {
                    sheet.SetRowHeightInPixels(index + iRowIndex, table.TableOptions.GroupPreviewSectionHeight);
                    previewRange.Text = e.Style.Text;
                }
                else
                {

                    sheet.SetRowHeightInPixels(index + iRowIndex, 0);
                    return 1;
                }
                if (ExportStyle)
                    CopyStyle(e.Style, previewRange);
            }
            else
            {
                if ((options & ConverterOptions.Visible) == ConverterOptions.Visible)
                {
                    sheet.SetRowHeightInPixels(index + iRowIndex, table.TableOptions.GroupPreviewSectionHeight);
                    previewRange.Text = paraStyle.Text;
                    if (ExportStyle)
                        CopyStyle(paraStyle, previewRange);
                }
                else
                {
                    //Old Export Style
                    previewRange.UnMerge();

                    sheet.Range[index + iRowIndex, 1].Text = element.Kind.ToString();
                    sheet.Range[index + iRowIndex, 2].Text = element.Info;

                    //previewRange.Text = "ElementKind: " + element.Kind.ToString() + "  Info: " + element.Info;
                }
            }
            GridImportExportCellInfoEventArgs args = new GridImportExportCellInfoEventArgs(previewRange, paraStyle, GridConverterAction.Export, iRowIndex, element.GroupLevel + 1);
            RaiseQueryImportExportCellInfo(args);
            if (args.Handled)
                CopyStyle(paraStyle, previewRange);

            return 1;
        }


        /// <summary>
        /// Gets the ForeignKeyCell Formatted Text.
        /// </summary>
        /// <param name="style">Column Style.</param>
        /// <param name="field">FieldDescriptor.</param>
        /// <param name="element">Record.</param>
        /// <param name="value">Field Value.</param>
        /// <returns>ForeignKeyCell Formatted Text.</returns>
        private string GetForeignKeyCellText(GridStyleInfo style, FieldDescriptor field, Element element, object value)
        {
            System.Globalization.NumberFormatInfo nfi = null;
            System.Globalization.CultureInfo ci = style.CultureInfo;
            
            FieldDescriptor columnFieldDescriptor = field;
            FieldDescriptor relatedFieldDescriptor = columnFieldDescriptor.GetRelatedDescriptor();
            FieldDescriptor nestedRelatedFieldDescriptor = columnFieldDescriptor.GetNestedRelatedDescriptor();
            Type propertyType = relatedFieldDescriptor.GetPropertyType();
            RelationDescriptor rd = columnFieldDescriptor.GetRelation();

            if (relatedFieldDescriptor == null
                || (rd.RelationKind == RelationKind.ForeignKeyReference || rd.RelationKind == RelationKind.ForeignKeyKeyWords)
                && (propertyType == typeof(System.Byte[]) || rd.RelationKeys.Count == 0)
                )							// ForeignListItems
                return "";

            // Replace the last relation key (the foreign key id with the value that was passed in).
            Record record = Record.GetParentRecord(element);

            //if (columnFieldDescriptor.IsComplexPropertyField())
            //{
            //    value = columnFieldDescriptor.GetValueFromDataRow(value);
            //    propertyType = columnFieldDescriptor.GetPropertyType();
            //}
            //else 
            if (rd.RelationKind == RelationKind.ForeignKeyKeyWords)
            {
                ChildTable childTable = record.GetRelatedChildTable(rd);

                if (childTable == null)
                    return "";

                // Now get the value from the related childTable.
                Syncfusion.Grouping.Table relatedTable = childTable.ParentTable;
                Syncfusion.Collections.BinaryTree.ITreeTableSummary[] summaries = childTable.GetSummaries(relatedTable);
                SummaryDescriptor sd = columnFieldDescriptor.GetRelatedSummaryDescriptor();
                if (sd == null)
                {
                    Console.WriteLine("Could not find summary for " + columnFieldDescriptor.Name + " in " + relatedTable.ParentTableDescriptor.Name);
                }
                int index = relatedTable.TableDescriptor.Summaries.IndexOf(sd);
                if (index != -1)
                {
                    VectorSummary summary = summaries[index] as VectorSummary;

                    StringBuilder sb = new StringBuilder();
                    for (int n = 0; n < summary.Values.Length; n++)
                    {
                        if (n > 0)
                            sb.Append(", ");
                        sb.Append(summary.Values[n] == null || summary.Values[n] is DBNull ? "null" : summary.Values[n].ToString());
                    }
                    value = sb.ToString();
                }
                propertyType = typeof(string);
            }
            // ForeignListItems
            ////by default Record.GetValue() returns formated vlaue...
            //else 
            //{
            //    Record relatedRecord = record.GetRelatedRecord(rd, rd.RelationKeys[rd.RelationKeys.Count - 1].ParentKeyField, value);

            //    if (relatedRecord == null)
            //        return "";

            //    // Now get the value from the related record.
            //    value = relatedRecord.GetValue(relatedFieldDescriptor);
            //    propertyType = nestedRelatedFieldDescriptor.GetPropertyType();
            //}

            return GridCellValueConvert.FormatValue(value, propertyType, style.Format, ci, nfi);
        }

        /// <summary>
        /// Initiates call to OnQueryExportPreviewRowInfo
        /// </summary>
        /// <param name="e">The event arguments.</param>
        private void RaiseQueryExportPreviewRowInfo(GroupingGridExportPreviewRowQueryInfoEventArgs e)
        {
            try
            {
                OnQueryExportPreviewRowInfo(e);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Raises the QueryExportPreviewRowInfo event.
        /// </summary>
        /// <param name="e">An GroupingGridExportPreviewRowInfoEventArgs that contains the event data.</param>
        private void OnQueryExportPreviewRowInfo(GroupingGridExportPreviewRowQueryInfoEventArgs e)
        {
            if (this.QueryExportPreviewRowInfo != null)
            {
                this.QueryExportPreviewRowInfo(this, e);
            }
        }

        /// <summary>
        /// Initiates call to OnExportElement.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        private void RaiseExportElement(GridExportElementEventArgs e)
        {
            try
            {
                OnExportElement(e);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        
        /// <summary>
        /// Raises the ExportElement event.
        /// </summary>
        /// <param name="e">An GridExportElementEventArgs that contains the event data.</param>
        private void OnExportElement(GridExportElementEventArgs e)
        {
            if (this.ExportElement != null)
            {        
                this.ExportElement(this, e);
            }
        }

        /// <summary>
        /// Initiates call to OnQueryCaptionText.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        private void RaiseQueryCaptionText(GridQueryCaptionTextEventArgs e)
        {
            try
            {
                OnQueryCaptionText(e);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// Raises the QueryCaptionText event.
        /// </summary>
        /// <param name="e">An GridQueryCaptionTextEventArgs that contains the event data.</param>
        private void OnQueryCaptionText(GridQueryCaptionTextEventArgs e)
        {
            if (this.QueryCaptionText != null)
            {
                this.QueryCaptionText(this, e);
            }
        }

        #endregion

        #region Class static methods
        /// <summary>
        /// Retrieves caption text from caption row.
        /// </summary>
        /// <param name="caption">Caption row to get text from.</param>
        /// <returns>Caption text.</returns>
        public static string GetCaptionText(GridCaptionRow caption)
        {
            return GetGroupCaptionText(caption.ParentGroup);
        }
        /// <summary>
        /// Retrieves caption text for group.
        /// </summary>
        /// <param name="group">Group to get caption text from.</param>
        /// <returns>Caption text.</returns>
        public static string GetGroupCaptionText(Group group)
        {
            IGridGroupOptionsSource g = group as IGridGroupOptionsSource;
            string captionText;

            captionText = (g != null)
              ? g.GroupOptions.CaptionText
              : captionText = "{CategoryCaption}: {Category} - {RecordCount} Items";

            return GetGroupCaptionDisplayText(group, captionText);
        }

        /// <summary>
        /// Gets caption text for a group.
        /// </summary>
        /// <param name="group">The group to get caption text for.</param>
        /// <param name="format">Caption format.</param>
        /// <returns>Caption text for the group.</returns>
        public static string GetGroupCaptionDisplayText(Group group, string format)
        {
            GridTableDescriptor tableDescriptor = (GridTableDescriptor)group.ParentTableDescriptor;

            bool raiseException = false;
            ArrayList al = new ArrayList();
            int iOpenBracketPos = format.IndexOf("{");
            StringBuilder sb = new StringBuilder();
            int iCloseBracketPos = 0;

            if (iOpenBracketPos == -1)
            {
                sb.Append(format);
            }
            else
            {
                sb.Append(format.Substring(0, iOpenBracketPos + 1));
            }

            while (iOpenBracketPos != -1)
            {
                iCloseBracketPos = format.IndexOf("}", iOpenBracketPos);

                if (iOpenBracketPos != -1 && iCloseBracketPos != -1 && iCloseBracketPos > iOpenBracketPos)
                {
                    int n3 = format.IndexOfAny(new char[] { '}', ':' }, iOpenBracketPos);
                    string name = format.Substring(iOpenBracketPos + 1, n3 - iOpenBracketPos - 1);

                    sb.Append(al.Count.ToString());
                    object obj = "";

                    if (name == "TableName")
                    {
                        obj = group.ParentTableDescriptor.Name;
                    }
                    else if (name == "CategoryName")
                    {
                        obj = group.Name;
                    }
                    else if (name == "CategoryCaption")
                    {
                        string fieldName = group.Name;

                        GridColumnDescriptor cd = group.GroupLevel >= 0
                          ? tableDescriptor.Columns.FindByMappingName(fieldName)
                          : null;

                        obj = (cd != null) ? cd.HeaderText : fieldName;
                    }
                    else if (name == "Category")
                    {
                        obj = group.Category;
                    }
                    else if (name == "RecordCount")
                    {
                        obj = group.GetFilteredRecordCount();
                    }
                    else
                    {
                        GridTableDescriptor td = ((GridTableDescriptor)group.ParentTableDescriptor);
                        int summaryRowNum;
                        int dot = name.IndexOf('.');

                        if (dot != -1)
                        {
                            string rowName = name.Substring(0, dot);
                            name = name.Substring(dot + 1);
                            summaryRowNum = td.SummaryRows.IndexOf(rowName);
                        }
                        else
                        {
                            summaryRowNum = td.SummaryRows.IndexOf("GroupCaption");
                        }

                        if (summaryRowNum != -1)
                        {
                            GridSummaryColumnDescriptor scd = null;
#if ASPNET
                            scd = td.SummaryRows[summaryRowNum].SummaryColumns.GetSummaryColDescriptor(name);
#else
                            scd = td.SummaryRows[summaryRowNum].SummaryColumns[name];
#endif
                            if (scd != null)
                            {
                                Group g = group;
                                Syncfusion.Collections.BinaryTree.ITreeTableSummary[] sums =
                                  g.GetSummaries(group.ParentTable);
                                int ndx = scd.GetSummaryIndex();
                                obj = (scd.GetDisplayText(sums[ndx]));
                            }
                        }
                    }
                    al.Add(obj);
                    iOpenBracketPos = format.IndexOf("{", iCloseBracketPos);

                    if (iOpenBracketPos == -1)
                    {
                        sb.Append(format.Substring(n3));
                    }
                    else
                    {
                        sb.Append(format.Substring(n3, iOpenBracketPos - n3 + 1));
                    }
                }
                else
                {
                    if (raiseException)
                    {
                        throw new FormatException("No closing char found: " + sb.ToString());
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
        #region Class properties
        /// <summary>
        /// Indicates whether default alignment setting should be used when alignment
        /// wasn't set. Read-only.
        /// </summary>
        public override bool UseDefaultAlignment
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// A property that gets or sets the caption cell's back color in the worksheet.
        /// </summary>
        public Color CaptionBackColor
        {
            get { return captionBKColor; }
            set
            {
                hasCaptionBKColor = true;
                captionBKColor = value;
            }
        }

        /// <summary>
        /// A property that gets or sets whether the caption summary row if enabled, can be exported to excel. Default is true.
        /// </summary>
        public bool ExportCaptionSummary
        {
            get 
            {
                return expCaptionSummary;
            }
            set
            {
                if (expCaptionSummary != value)
                    expCaptionSummary = value;
            }
        }

        /// <summary>
        /// A property that gets or sets a value indicating whether the Record with related tables should be exported as Excel Group.
        /// </summary>
        public bool ExportRecordPlusMinus
        {

            get { return expRecordPLUSMINUS && (options & ConverterOptions.Visible) != ConverterOptions.Visible; }
            set 
            { 
                expRecordPLUSMINUS = value;
                hasExpRecordPLUSMINUS = true;
            }
        }

        /// <summary>
        /// A property that gets or sets a value indicating whether the  Grouping.Group should be exported as Excel Group.
        /// </summary>
        public bool ExportGroupPlusMinus
        {
            get { return expGroupPLUSMINUS && (options & ConverterOptions.Visible) != ConverterOptions.Visible; }
            set 
            { 
                expGroupPLUSMINUS = value;
                hasExpRecordPLUSMINUS = true;
            }
        }

        /// <summary>
        /// A property that gets or sets a value indicating whether the  PreviewRows should be exported.
        /// The default value is false.
        /// </summary>
        /// <remarks>
        /// When you are not exporting with <see cref="Syncfusion.GridExcelConverter.ConverterOptions.Visible"/> option
        /// then, the <see cref="QueryExportPreviewRowInfo"/> event must be handled.        
        /// </remarks>
        public bool ExportPreviewRows
        {
            get { return exportPreviewRow; }
            set { exportPreviewRow = value; }
        }

        /// <override/>
        /// <summary>
        /// An Overridden property that gets or sets the export style.
        /// </summary>
        public override bool ExportStyle
        {
            get
            {
                return base.ExportStyle;
            }
            set
            {
                base.ExportStyle = value;
                if(!hasExpRecordPLUSMINUS)
                    expRecordPLUSMINUS = value;
                if(!hasExpGroupPLUSMINUS)
                    expGroupPLUSMINUS = value;
                }
            }

        /// <summary>
        /// An Overridden readonly property that gets the value whether it is a grouping grid.
        /// </summary>
        protected override bool IsGroupingGrid
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// A property that gets or sets the grouping grid.
        /// </summary>
        protected GridGroupingControl GroupingGrid
        {
            get { return groupingGrid; }
            set
            {
                groupingGrid = value;
                Grid = groupingGrid.Table.TableModel;
            }
        }

        #endregion

        /// <summary>
        /// Represents a method that handles an event with <see cref="GroupingGridExportElementQueryInfoEventArgs"/>.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        public delegate void GroupingGridExportPreviewRowQueryInfoEventHandler(object sender, GroupingGridExportPreviewRowQueryInfoEventArgs e);

        /// <summary>
        /// Represents a method that handles an event with <see cref="GridExportElementEventArgs"/>.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        public delegate void GridExportElementEventHandler(object sender, GridExportElementEventArgs e);

        /// <summary>
        /// Represents a method that handles an event with <see cref="GridQueryCaptionTextEventArgs"/>.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        public delegate void GridQueryCaptionTextEventHandler(object sender, GridQueryCaptionTextEventArgs e);

        
        /// <summary>
        /// Occurs for each preview row element before the <see cref="GroupingGridExcelConverterControl"/>
        /// exports and lets users customize the preview row.
        /// </summary>
        public event GroupingGridExportPreviewRowQueryInfoEventHandler QueryExportPreviewRowInfo;

        /// <summary>
        /// Occurs for each element before the <see cref="GroupingGridExcelConverterControl"/>
        /// exports and lets users to cancel exporting the element.
        /// </summary>
        public event GridExportElementEventHandler ExportElement;

        /// <summary>
        /// Occurs for each CaptionRow before the <see cref="GroupingGridExcelConverterControl"/>
        /// exports and lets users customize the CaptionText.
        /// </summary>
        public event GridQueryCaptionTextEventHandler QueryCaptionText;

        Color captionBKColor;
        bool expRecordPLUSMINUS = true;
        bool expGroupPLUSMINUS = true;
        bool exportPreviewRow = false;//true;

        bool hasCaptionBKColor = false;
        bool hasExpRecordPLUSMINUS = false;
        bool hasExpGroupPLUSMINUS = true;
        private bool expCaptionSummary = true;

        ConverterOptions options;
        GridGroupingControl groupingGrid;

    }
    /// <summary>
    /// Provides data about the QueryCaptionEvent event.
    /// </summary>
    public sealed class GridQueryCaptionTextEventArgs : Syncfusion.ComponentModel.SyncfusionEventArgs
    {
        /// <summary>
        /// Initializes a new <see cref="GridQueryCaptionTextEventArgs"/>
        /// </summary>
        /// <param name="caption">The CaptionRow to be exported.</param>
        /// <param name="text">The text to be displayed in CaptionRow.</param>
        public GridQueryCaptionTextEventArgs(CaptionRow caption, string text)
        {
            this.caption = caption;
            this.text = text;
        }

        CaptionRow caption;

        /// <summary>
        /// A readonly property that gets the CaptionRow whose display text you can modify.
        /// </summary>
        public CaptionRow Caption
        {
            get { return caption; }
        }
        string text;

        /// <summary>
        /// A property that gets or sets the string that will be exported for the CaptionRow.
        /// </summary>
        public string Text
        {
            get { return text; }
            set { text = value; }
        }
    }
    /// <summary>
    /// Provides data about the ExportElement event.
    /// </summary>
    public sealed class GridExportElementEventArgs : Syncfusion.ComponentModel.SyncfusionCancelEventArgs
    {
        /// <summary>
        /// Initializes a new <see cref="GridExportElementEventArgs"/>
        /// </summary>
        /// <param name="element">The element to be exported.</param>
        /// <param name="rowIndex">The rowindex of the element.</param>
        public GridExportElementEventArgs( Element element, int rowIndex)
        {
            this.element = element;
            this.rowIndex = rowIndex;
        }

        // Properties

        /// <summary>
        /// The element to get exported to the worksheet.
        /// </summary>
        public Element Element
        {
            get
            {
                return element;
            }

        }
        /// <summary>
        /// A readonly property that gets Worksheet rowindex to which the Element will get exported.
        /// </summary>
        public int RowIndex
        {
            get { return rowIndex; }
        }

        // Fields
        private Element element;
        private int rowIndex;
    }

    /// <summary>
    /// A sealed class that defines data about the GroupingGridExportPreviewRowQueryInfo event.
    /// </summary>
    public sealed class GroupingGridExportPreviewRowQueryInfoEventArgs
    {
        /// <summary>
        ///  Initializes the object with Element to export and the GridStyleInfo.
        /// </summary>
        /// <param name="el">Element to export.</param>
        /// <param name="style">Style Object.</param>
        public GroupingGridExportPreviewRowQueryInfoEventArgs(Element el, GridStyleInfo style)
        {
            if (style == null)
                this.style = new GridStyleInfo();
            else
                this.style = style;

            element = el;

        }

        // Properties

        /// <summary>
        /// A readonly property that gets the Style object.
        /// </summary>
        public GridStyleInfo Style
        {
            get
            {
                return style;
            }
        }

        /// <summary>
        /// A readonly property to get the element to export.
        /// </summary>
        public Element Element
        {
            get { return element; }
        }

        /// <summary>
        /// A property that gets or sets the result. True if the preview row should get exported, false otherwise.
        /// </summary>
        public bool Result
        {
            get { return result; }
            set { result = value; }
        }

        /// <summary>
        /// A property that gets or sets whether the event has been handled and no further processing of this event should happen.
        /// </summary>
        public bool Handled
        {
            get { return handled; }
            set { handled = value; }
        }

        // Fields
        private GridStyleInfo style;
        private Element element;
        private bool result = true;
        private bool handled = false;
    }
}
