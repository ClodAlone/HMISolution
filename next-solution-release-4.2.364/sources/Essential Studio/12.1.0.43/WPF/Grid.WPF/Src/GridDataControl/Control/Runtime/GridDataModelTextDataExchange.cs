#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Media;
using Syncfusion.Windows.Controls.Cells;
using System.Windows;
using Syncfusion.Windows.Data;

namespace Syncfusion.Windows.Controls.Grid
{
    public class GridDataModelTextDataExchange : GridModelTextDataExchange
    {
        public GridDataModelTextDataExchange(GridDataTableModel model) :
            base(model)
        {

        }

        /// <summary>
        /// Copy cellvalue to the buffer
        /// </summary>
        /// <param name="buffer">contain the formatted text for clipboard</param>
        /// <param name="rangeList">currently selected rangeList</param>
        /// <param name="nrowsdone">Number of rows affected</param>
        /// <param name="ncolsdone">Number of columns affected</param>
        /// <param name="gridControl">Reference for gridControl</param>
        /// <returns>returns true after successful copy the selected cell text value to the buffer</returns>
        public override bool CopyTextToBuffer(out string buffer, GridRangeInfoList rangeList, out int nrowsdone, out int ncolsdone, bool clear)
        {
            GridRangeInfoList rowRanges = rangeList.GetRowRanges(GridRangeInfoType.Cells | GridRangeInfoType.Rows);
            GridRangeInfoList colRanges = rangeList.GetColRanges(GridRangeInfoType.Cells | GridRangeInfoType.Cols);
            // bool IsSkip = false; The variable is assigned but it is never used.
            StringBuilder sb = new StringBuilder();
            nrowsdone = 0;
            ncolsdone = 0;

            string tabDelim = "\t";
            if (this.TabDelimiter != string.Empty)
            {
                tabDelim = this.TabDelimiter;
            }
            var _Model = this.Model as GridDataTableModel;
            
            for (int rowindex = 0; rowindex < rowRanges.Count; rowindex++)
            {
                var  rowbtom = rowRanges[rowindex].Bottom;
                var rowtop = rowRanges[rowindex].Top ;
                for (int nrow = rowtop; nrow <= rowbtom; nrow++)
                {
                    var recordIndex = 0;
                    bool isSummaryRow = false;
                    bool isCaptionRow = false;
                    if (this.Model is GridDataChildTableModel)
                        recordIndex = (Model as GridDataChildTableModel).ResolveIndexToRecordPosition(nrow);                    
                    else if (_Model.Table.HasNestedTables)
                    {
                        recordIndex = _Model.ResolveIndexToRecordPosition(nrow);
                        if (nrow != rowbtom)
                            nrow++;
                    }                   
                    else
                        recordIndex = _Model.ResolveIndexToRecordPosition(nrow);

                    if (recordIndex < 0)
                    {
                        var startIdx = _Model.ResolveStartIndexBasedOnPosition();
                        var counter0 = nrow - startIdx;
                        if (nrow > 0 && (_Model.Options.CopyPasteOption & CopyPaste.CopySummaries) == CopyPaste.CopySummaries
                            && _Model.Table.HasTableSummaries && _Model.TableProperties.TableSummaryPosition == Position.Top
                            && nrow <= _Model.TableProperties.TableSummaryRows.Count)
                                isSummaryRow = true;
                        else if (counter0 < 0)
                            continue;
                        else if (_Model.Table.GroupModel != null)
                        {
                            var displayEl = _Model.Table.GroupModel.DisplayElements[counter0];
                            if (displayEl is SummaryRecordEntry && (_Model.Options.CopyPasteOption & CopyPaste.CopySummaries) == CopyPaste.CopySummaries)
                                isSummaryRow = true;
                            else if (displayEl is Group && (_Model.Options.CopyPasteOption & CopyPaste.CopyCaptions) == CopyPaste.CopyCaptions)
                                isCaptionRow = true;
                        }
                        else
                            continue;
                    }

                    if ((_Model.Options.CopyPasteOption & CopyPaste.CopySummaries) == CopyPaste.CopySummaries 
                        && _Model.Table.HasTableSummaries && _Model.TableProperties.TableSummaryPosition == Position.Bottom
                        && nrow >= (rowbtom - (_Model.TableProperties.TableSummaryRows.Count-1)))
                            isSummaryRow = true;

                    if (nrowsdone > 0)
                    {
                        sb.Append(Environment.NewLine);
                    }

                    ncolsdone = 0;
                    bool firstCol;
                    firstCol = true;

                    for (int colindex = 0; colindex < colRanges.Count; colindex++)
                    {
                        for (int ncol = colRanges[colindex].Left; ncol <= colRanges[colindex].Right; ncol++)
                        {
                            var visibleColumnIndex = 0;
                            if (this.Model is GridDataChildTableModel)
                                visibleColumnIndex = (Model as GridDataChildTableModel).ResolvePositionToVisibleColumnIndex(ncol);
                            else
                                visibleColumnIndex = _Model.ResolvePositionToVisibleColumnIndex(ncol);
                            string text;
                            if (isSummaryRow || isCaptionRow)
                            {
                                var style = (_Model[nrow, ncol] as GridDataStyleInfo).CellIdentity.TableCellType;
                                if (style == GridDataTableCellType.EmptyCell || style == GridDataTableCellType.GroupCaptionPlusMinusCell
                                    || style == GridDataTableCellType.RecordPlusMinusCell || style == GridDataTableCellType.RowHeaderCell)
                                    continue;
                                else if (style == GridDataTableCellType.GroupCaptionCell)
                                {
                                    text = (_Model[nrow, ncol] as GridDataStyleInfo).GetFormattedText((_Model[nrow, ncol] as GridDataStyleInfo).CellValue);
                                    text = new StringBuilder(text).ToString().Trim();
                                    sb.Append(text);
                                    break;
                                }
                                else if (visibleColumnIndex > -1 && _Model.TableProperties.VisibleColumns[visibleColumnIndex].IsHidden)
                                    continue;
                            }
                            else if (visibleColumnIndex < 0 || visibleColumnIndex >= _Model.TableProperties.VisibleColumns.Count
                                || _Model.TableProperties.VisibleColumns[visibleColumnIndex].IsHidden)
                                continue;

                            //Only Record cells will enter this loop
                            //for (int rows = 0; rows <= this.Model.ColumnCount; rows++)
                            //{
                            //    if (this.Model[rows, ncol].CellType == "ExpandCollapseCell")
                            //    {
                            //        IsSkip = true;
                            //    }
                            //}

                            //if (IsSkip)
                            //{
                            //    IsSkip = false;
                            //    continue;
                            //}

                            if (!firstCol)
                            {
                                sb.Append(tabDelim);
                            }
                            if (isSummaryRow || isCaptionRow)
                            {
                                var style = _Model[nrow, ncol] as GridStyleInfo;
                                text = style.GetFormattedText(style.CellValue);
                            }
                            else
                                text = this.GetCopyTextRowCol(recordIndex, visibleColumnIndex, nrow, ncol, clear);

                            if (!rangeList.AnyRangeContains(GridRangeInfo.Cell(nrow, ncol)))
                            {
                                ncolsdone++;
                                continue;
                            }

                            text = new StringBuilder(text).ToString().Trim();
                            if (text == "")
                            {
                                text = "   ";
                            }
                            sb.Append(text);
                            firstCol = false;
                            //Added code to clear the Row in GetCopyTextRowCol function
                            //if (clear && !((this.Model.Options.CopyPasteOption & CopyPaste.CutCell) == CopyPaste.CutCell))
                            //{
                            //    GridStyleInfo style = this.Model[nrow, ncol];
                            //    style.ApplyFormattedText(string.Empty);
                            //}
                            ncolsdone++;
                        }
                    }//End of ColumnRanges for loop
                    nrowsdone++;
                }
            }//End of RowChanges for loop

            if (nrowsdone > 1 || ncolsdone > 1)
            {
                sb.Append(Environment.NewLine);
            }

            foreach (GridRangeInfo range in rangeList)
            {
                this.Model.InvalidateCell(range);
            }

            foreach (var grid in this.Model.Views)
            {
                if (grid.CurrentCell != null && grid.CurrentCell.IsEditing)
                {
                    grid.CurrentCell.EndEdit();
                }
            }
            buffer = sb.ToString();

            if ((this.Model.Options.CopyPasteOption & CopyPaste.IncludeHeaders) == CopyPaste.IncludeHeaders)
            {
                var bufferString = buffer;
                buffer = string.Empty;
                if (rangeList.Count > 0)
                {
                    for (int i = rangeList[0].Left; i <= rangeList[0].Right; i++)
                    {
                        var columnIndex = _Model.ResolvePositionToVisibleColumnIndex(i);
                        if (columnIndex < 0 || _Model.TableProperties.VisibleColumns[columnIndex].IsHidden)
                            continue;
                        if (columnIndex > -1)
                        {
                            buffer += _Model.TableProperties.VisibleColumns[columnIndex].HeaderText;
                        }
                        if (i != rangeList[0].Right)
                        {
                            buffer += this.TabDelimiter;
                        }
                    }
                }

                if (buffer.Length > 0)
                    buffer += Environment.NewLine;
                buffer += bufferString;
            }
            this.Model.InvalidateVisual(true);
            return true;
        }
        /// <summary>
        /// Calculates the Buffer dimentison as how many rows and cols does it required to paste.
        /// </summary>
        /// <param name="psz"></param>
        /// <param name="nRows"></param>
        /// <param name="nCols"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override bool CalcBufferDimension(string psz, out int nRows, out int nCols)
        {
            int nRowsDone = 0,
           nColsDone = 0;

            int colIndex = 0;
            bool canceled = false;
            //// (New line will start a new row, tab delimiter will
            //// move to the next column).
            string sTabDelim = "\t";
            if (!string.IsNullOrEmpty(this.TabDelimiter))
            {
                sTabDelim = this.TabDelimiter;
            }

            int size = psz.Length;

            for (int nIndex = 0, nLast = 0; !canceled && nIndex < size; nIndex++)
            {
                if (nIndex == size - 1 || "\r\n".IndexOf(psz[nIndex]) != -1 || sTabDelim[0] == psz[nIndex])
                {
                    if (nIndex == size - 1)
                    {
                        if (nIndex > nLast)
                        {
                            colIndex++;
                        }

                        break;
                    }
                    else if ("\r\n".IndexOf(psz[nIndex]) != -1)
                    {
                        //// Now that the value has been copied to the cell,
                        //// let's check if we should jump to a new row.

                        //// Yes, I found an end of line.
                        //// Let's increase rowIndex and reset colIndex to first column.
                        nRowsDone++;
                        colIndex = 0;

                        if (psz[nIndex] == '\r' && psz[nIndex + 1] == '\n')
                        {
                            nIndex++;
                        }

                        // Abort parsing the string if next char
                        // is an end-of-string.
                        if (nIndex == size - 1)
                        {
                            break;
                        }
                    }
                    else
                    {
                        //// Move to next column.
                        colIndex++;
                    }

                    //// Save index where the next cell's value starts.
                    nLast = nIndex + 1;

                    nColsDone = Math.Max(nColsDone, colIndex + 1);
                }
            }

            if (colIndex > 0)
            {
                nRowsDone++;
            }

            nRows = nRowsDone;
            nCols = Math.Max(nColsDone, 1);

            return true;
        }


        /// <summary>
        /// Get the value of the cell
        /// </summary>
        /// <param name="rowIndex">contain the row value</param>
        /// <param name="colIndex">contain the column value</param>
        /// <returns>the value of the cell</returns>
        public override string GetCopyTextRowCol(int rowIndex, int colIndex)
        {
            GridDataControl datagrid = null; 
            // bool state = false; The variable is assigned but it is never used.
            object data = "";
            int recordindex = 0;
            // Newly added code to affect the underlying business objects on pasting text from Clipboard
            foreach (GridControlBase grid in Model.Views)
            {
                if (grid != null)
                {
                    datagrid = grid.FindParentElementOfType<GridDataControl>();
                    if (datagrid != null)
                    {
                        int Expanderindex = 0;
                        // Copy the values in underlying collections is done seperatly for MAster Table and Child Table. 
                        var ChildModel = this.Model as GridDataChildTableModel;
                        // This gets the value from Child Table. 
                        if (ChildModel != null)
                        {
                            object record = ChildModel.Table.GetRecordFromRow(rowIndex);
                            if (record != null)
                            {
                                recordindex = ChildModel.View.Records.IndexOfRecord(record);
                            }
                            data = ChildModel.Table.GetValue(recordindex, (ChildModel.GetVisibleColumns()).ElementAt(colIndex).MappingName.ToString()) ?? "";
                        }
                        else
                        {
                            // This gets the value from Master Table. 
                            // Adjust hte column index based on the RowHeader
                            if (datagrid.ShowRowHeader)
                            {
                                Expanderindex = Expanderindex + 1;
                                colIndex = colIndex - 1;
                            }
                            // Adjust the column index based on the Expander Cells. 
                            if (datagrid.Model[rowIndex, Expanderindex].CellType == "ExpandCollapseCell")
                            {
                                colIndex = colIndex - 1;
                            }
                            if (datagrid.GroupedColumns.Count > 0)
                            {
                                colIndex = colIndex - datagrid.GroupedColumns.Count;
                            }
                            object record = datagrid.Model.Table.GetRecordFromRow(rowIndex);
                            if (record != null)
                            {
                                recordindex = datagrid.Model.View.Records.IndexOfRecord(record);
                            }
                            if (datagrid.VisibleColumns[colIndex].IsUnbound)
                            {
                                var style = datagrid.Model[rowIndex, colIndex] as GridStyleInfo;
                                data = style.GetFormattedText(style.CellValue);
                            }
                            else
                            {
                                data = datagrid.Model.Table.GetValue(recordindex, datagrid.VisibleColumns[colIndex].MappingName.ToString()) ?? "";
                            }

                        }
                    }
                }
            }
            return data.ToString();
        }

        public virtual string GetCopyTextRowCol(int recordIndex, int visibleColIndex,int rowIndex,int colIndex, bool clear)
        {
            GridDataControl datagrid = null; 
            // bool state = false; Thhe variable is assigned but it is never used
            object data = "";
            // Newly added code to affect the underlying business objects on pasting text from Clipboard
            foreach (GridControlBase grid in Model.Views)
            {
                if (grid != null)
                {
                    datagrid = grid.FindParentElementOfType<GridDataControl>();
                    if (datagrid != null)
                    {
                        // int Expanderindex = 0; The variable is assigned but it is never used
                        // Copy the values in underlying collections is done seperatly for MAster Table and Child Table. 
                        var ChildModel = this.Model as GridDataChildTableModel;
                        // This gets the value from Child Table. 
                        if (ChildModel != null)
                        {
                            data = ChildModel.Table.GetValue(recordIndex, (ChildModel.GetVisibleColumns()).ElementAt(visibleColIndex).MappingName.ToString()) ?? "";
                            if (clear)
                                ChildModel.Table.SetValue(recordIndex, (ChildModel.GetVisibleColumns()).ElementAt(visibleColIndex).MappingName.ToString(), DefaultValue((ChildModel.GetVisibleColumns()).ElementAt(visibleColIndex).ColumnType));
                        }
                        else
                        {
                            if (datagrid.VisibleColumns[visibleColIndex].IsUnbound)
                            {                                
                               // Recordindex and VisibleColIndex can't be used for getting cellvalue in Unbound columns
                                var style = datagrid.Model[rowIndex, colIndex] as GridStyleInfo;
                                data = style.GetFormattedText(style.CellValue);
                            }
                            else
                            {
                                data = datagrid.Model.Table.GetValue(recordIndex, datagrid.VisibleColumns[visibleColIndex].MappingName.ToString()) ?? "";
                                if (clear)
                                {
                                    datagrid.Model.Table.SetValue(recordIndex, datagrid.VisibleColumns[visibleColIndex].MappingName.ToString(), DefaultValue(datagrid.VisibleColumns[visibleColIndex].ColumnType));
                                    datagrid.Model[rowIndex, colIndex].CellValue = DefaultValue(datagrid.VisibleColumns[visibleColIndex].ColumnType);
                                }
                                if (data.ToString().Contains("\r\n"))
                                {
                                     data = "\"" + data + Environment.NewLine;
                                }
                            }
                        }
                    }
                }
            }
            return data.ToString();
        }

        private static object DefaultValue(Type myType)
        {
            if (!myType.IsValueType)
                return null;
            else
                return Activator.CreateInstance(myType);
        }

        /// <summary>
        /// Paste Text From The Buffer
        /// </summary>
        /// <param name="buffer">For Holding final clipboard value</param>
        /// <param name="rangeList">conatin the selected range</param>
        /// <returns>return true after fill the cell value from the buffer</returns>
        public override bool PasteTextFromBuffer(string buffer, GridRangeInfoList rangeList)
        {
            var baseString = buffer;
            if (!buffer.Equals(string.Empty))
            {
                if ((this.Model.Options.CopyPasteOption & CopyPaste.PasteCell) == CopyPaste.PasteCell)
                {
                    return false;
                }
                if (rangeList == null)
                {
                    rangeList = new GridRangeInfoList();
                    RowColumnIndex rci = this.Model.CurrentCellState.CellRowColumnIndex;
                    rangeList.Add(GridRangeInfo.Cell(rci.RowIndex, rci.ColumnIndex));
                }
                if ((this.Model.Options.CopyPasteOption & CopyPaste.IncludeHeaders) == CopyPaste.IncludeHeaders)
                {
                    buffer =
                        new StringBuilder(buffer).Remove(0, new StringReader(buffer).ReadLine().Length + 2).ToString();
                    baseString = buffer;
                }
                string tabDelim = "\t";
                if (this.TabDelimiter != string.Empty)
                {
                    tabDelim = this.TabDelimiter;
                }

                if (!this.Model.CutPaste.PasteBoundCheck(rangeList, buffer, tabDelim))
                {
                    return false;
                }

                // string s = string.Empty; Unused local variable
                bool canceled = true;
                int rowIndex = rangeList[0].Top;
                int colIndex = rangeList[0].Left;

                rowIndex = rowIndex == 0 ? 1 : rowIndex;
                // int lastCol = colIndex; Unused local variable
                // int size = buffer.Length; Unused local variable

                // StringReader sr = new StringReader(buffer); Unused local variable
                int rangelColIndex = 0;
                string bufferCopy = buffer;
                int rowCount = 0;
                string temp = bufferCopy;
                StringReader bf = new StringReader(bufferCopy);
                while (temp != null)
                {
                    temp = bf.ReadLine();
                    if (temp != null)
                        rowCount++;
                }
                string[] rowCollect;
                if ((this.Model.Options.CopyPasteOption & CopyPaste.IncludeEmptyCells) == CopyPaste.IncludeEmptyCells)
                {
                    rowCollect = baseString.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                }
                else
                    rowCollect = baseString.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                rowCount = rowCollect.Length;
                GridDataControl datagrid = null;
                foreach (GridControlBase grid in Model.Views)
                {
                    if (grid != null)
                    {
                        datagrid = grid.FindParentElementOfType<GridDataControl>();
                        datagrid.Model.View.Suspend();
                    }
                }
                int pasterowCount = 0;
                foreach (var item in rowCollect)
                {
                    if (((this.Model.Options.CopyPasteOption & CopyPaste.IncludeEmptyCells) == CopyPaste.IncludeEmptyCells) && item == string.Empty && pasterowCount == rowCollect.Length - 1)
                    {
                        break;
                    }
                    pasterowCount++;
                    var colCollect = item.Split(new string[] { "\t" }, StringSplitOptions.None);
                    int tempcol = colIndex;
                    foreach (var colItem in colCollect)
                    {
                        rangelColIndex = colIndex;
                        if (this.Model[rowIndex, colIndex].CellType == "Static" ||
                                this.Model[rowIndex, colIndex].ReadOnly ||
                                this.Model[rowIndex, colIndex].CellType == "ReadOnly" ||
                                this.Model[rowIndex, colIndex].CellType == "static")
                        {
                            continue;
                        }
                        if (this.Model is GridDataTableModel)
                        {
                            int tempColIndex =
                                (colIndex - ((this.Model as GridDataTableModel).Grid.NavigateWithArrowKeysCellsRange.Left - (this.Model as GridDataTableModel).TableProperties.HeaderColumns));
                            while ((this.Model as GridDataTableModel).TableProperties.VisibleColumns[tempColIndex].IsHidden)
                            {
                                tempColIndex++;
                                colIndex++;
                                rangelColIndex = colIndex;
                                if (tempColIndex == (this.Model as GridDataTableModel).TableProperties.VisibleColumns.Count)
                                    break;
                            }
                        }
                        if (rowIndex <= this.Model.RowCount && colIndex <= this.Model.ColumnCount)
                            canceled = !this.PasteTextRowCol(rowIndex, colIndex, colItem);
                        colIndex++;
                    }
                    colIndex = tempcol;
                    rowIndex++;
                    if (datagrid.Model.Table.HasNestedTables)
                        rowIndex = rowIndex + datagrid.Model.TableProperties.Relations.Count;
                }
                /*foreach (var item in rowCollect)
                {
                    buffer = item;
                    size = buffer.Length;
                    for (int index = 0, last = 0; index <= size; index++)
                    {
                        //// Check for a delimiter.
                        if (size == 0)
                        {
                            this.PasteTextRowCol(rowIndex, colIndex, string.Empty);
                        }
                        else
                        {
                            if (this.Model[rowIndex, colIndex].CellType == "Static" ||
                                this.Model[rowIndex, colIndex].ReadOnly ||
                                this.Model[rowIndex, colIndex].CellType == "ReadOnly" ||
                                this.Model[rowIndex, colIndex].CellType == "static")
                            {
                                continue;
                            }
                            bool isDelimiter = (("\r\n".IndexOf(buffer[index]) != -1 || tabDelim[0] == buffer[index]) &&
                                                (buffer[index] != '\n'));
                            if (index == size - 1 || isDelimiter)
                            {
                                //// End of a string found, copy value to cell.

                                rangelColIndex = colIndex;
                                if ("\r\n".IndexOf(buffer[index]) != -1)
                                {
                                    if (buffer[index] == '\r' && buffer[index + 1] == '\n')
                                    {
                                        index++;
                                        colIndex = rangeList[0].Left;
                                        last = index + 1;
                                        continue;
                                    }
                                }

                                if (rowIndex <= this.Model.RowCount && colIndex <= this.Model.ColumnCount)
                                {
                                    s = isDelimiter
                                            ? (index != last ? buffer.Substring(last, index - last) : String.Empty)
                                            : buffer.Substring(last);

                                    //Remove double quotes start and end when new line is used in cell
                                    if (s.StartsWith("\""))
                                        s = s.Substring(1, s.Length - 2);
                                    s = s.Replace("\"\"", "\"");

                                    //// Give the control the chance to validate
                                    //// and change the pasted text.
                                    canceled = !this.PasteTextRowCol(rowIndex, colIndex, s);
                                }

                                if (!canceled && index != size - 1)
                                {
                                    if ("\r\n".IndexOf(buffer[index]) == -1)
                                    {
                                        colIndex++;
                                    }
                                    else
                                    {
                                        rowIndex++;
                                        colIndex = rangeList[0].Left;
                                        if (buffer[index] == '\r' && buffer[index + 1] == '\n')
                                        {
                                            index++;
                                        }

                                        //// Abort parsing the string if next char
                                        //// is an end-of-string.
                                        if (index == size - 1)
                                        {
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    break;
                                }

                                last = index + 1;
                                lastCol = Math.Max(colIndex, lastCol);
                            }
                        }
                    }
                    rowIndex++;
                    colIndex = rangeList[0].Left;
                }*/
                datagrid.Model.View.Resume();
                //  Since we paste the copied text to the underlying collection and not as the control text we no need of calling View.Refresh() method now. 
                //  Hence it is commented. To ensure it does not have any breaking it is kept as commented code still. 
                //  datagrid.Model.View.Refresh();
                var list = rangeList.Clone() as GridRangeInfoList;
                Model.Selections.Clear();
                if (list.Count > 0)
                {
                    Model.Selections.Add(GridRangeInfo.Cells(list[0].Top, list[0].Left, rowIndex - 1, rangelColIndex));
                    Model.InvalidateCell(GridRangeInfo.Cells(list[0].Top, list[0].Left, rowIndex - 1, rangelColIndex));
                }
                this.Model.InvalidateVisual(true);

                if (Model.CutPaste.CutFlag)
                {
                    try
                    {
#if !SILVERLIGHT
                        System.Windows.Clipboard.SetDataObject(string.Empty);
#else
                   System.Windows.Clipboard.SetText(string.Empty);
#endif
                    }
                    finally
                    {
                    }
                    Model.CutPaste.CutFlag = false;
                }
                return !canceled;
            }
            return false;
        }

        /// <summary>
        /// Pastes the text value to the cell
        /// </summary>
        /// <param name="rowIndex">Row value for the cell</param>
        /// <param name="colIndex">Column value for the cell</param>
        /// <param name="text">New String value for the cell</param>
        /// <returns>successful paste returns true</returns>
        public override bool PasteTextRowCol(int rowIndex, int colIndex, string text)
        {
            int celRowIdx = rowIndex;
            int celColIdx = colIndex;
            int recordindex = 0;
            // Newly added code to affect the underlying business objects on pasting text from Clipboard
            foreach (GridControlBase grid in Model.Views)
            {
                if (grid != null)
                {
                    var gridModel = this.Model as GridDataTableModel;
                    if (gridModel != null)
                    {
                        colIndex -= (gridModel.Grid.NavigateWithArrowKeysCellsRange.Left - gridModel.TableProperties.HeaderColumns);
                        object record = gridModel.Table.GetRecordFromRow(rowIndex);
                        if (record != null)
                        {
                            recordindex = gridModel.Table.HasGroups ? gridModel.View.TopLevelGroup.DisplayElements.IndexOf(record as RecordEntry) : gridModel.View.Records.IndexOfRecord(record);
                        }
                        if (gridModel.TableProperties.VisibleColumns[colIndex].IsUnbound)
                        {
                            // var style = gridModel[rowIndex, colIndex] as GridStyleInfo; Unused local variable
                            gridModel[rowIndex, colIndex].CellValue = text;
                        }
                        else
                        {
                            if (!gridModel.TableProperties.VisibleColumns[colIndex].IsReadOnly && recordindex < gridModel.SourceListCount)
                            {
                                gridModel.Table.SetValue(recordindex,
                                                                   gridModel.TableProperties.VisibleColumns[colIndex].MappingName.ToString(), text);
                                gridModel[celRowIdx, celColIdx].CellValue = text;
                            }
                        }

                        if (gridModel.Table.HasTableSummaries)
                        {
                            gridModel.InvalidateCell(gridModel.GetRangeOfTableSummaryRows());
                        }
                        gridModel.InvalidateCell(GridRangeInfo.Row(rowIndex));
                    }
                }
            }
            return true;
        }
#if !SILVERLIGHT

        /// <summary>
        /// Copy the selected cells in XmlDocument format
        /// </summary>
        /// <param name="buffer">Returns the Xml content of copied ranges</param>
        /// <param name="rangeList">currently selected rangeList</param>
        /// <param name="nrowsdone">Number of rows affected</param>
        /// <param name="ncolsdone">Number of columns affected</param>
        /// <param name="clear">Clear the selected ranges.</param>
        /// <returns>returns true after successful copy the selected cell text value to the buffer</returns>
        public override bool CopyXmlToBuffer(out string buffer, GridRangeInfoList rangeList, out int nrowsdone, out int ncolsdone, bool clear)
        {
            buffer = null;
            nrowsdone = 0;
            ncolsdone = 0;
            GridRangeInfoList rowRanges = rangeList.GetRowRanges(GridRangeInfoType.Cells | GridRangeInfoType.Rows);
            GridRangeInfoList colRanges = rangeList.GetColRanges(GridRangeInfoType.Cells | GridRangeInfoType.Cols);
            int nrows = 0;
            foreach (GridRangeInfo range in rowRanges)
            {
                nrows += range.Height;
            }
            int ncols = 0;

            foreach (GridRangeInfo range in colRanges)
            {
                ncols += range.Width;
            }

            int top = rangeList[0].Top;
            int left = rangeList[0].Left;
            int right = rangeList[0].Right;
            int bottom = rangeList[0].Bottom;
            int rowcount = (bottom - top) + 1;
            int colcount = (right - left) + 1;
            StringBuilder sb = new StringBuilder();
            GridStyleInfo style;

            // Header section " 
            string header = GetXmlHeaderContent();
            sb.Append(header);

            // Styles Section: 
            sb.Append("<Styles>");

            // Append Default Style
            string defaultstyle = GetDefaultXmlStyle();
            sb.Append(defaultstyle);

            // Append Cell Styles
            for (int row = 0; row < rowcount; row++)
            {
                for (int col = 0; col < colcount; col++)
                {
                    style = Model[row + top, col + left];
                    string cellstyles = CreateXmlCellStyles(style);
                    sb.Append(cellstyles);
                }
            }
            sb.Append("  </Styles><Worksheet ss:Name=\"Sheet1\">");

            //Append Table
            sb.Append("<Table ss:ExpandedColumnCount=\"" + colcount +
                      "\" ss:ExpandedRowCount=\"" + rowcount + "\" x:FullColumns=\"1\" x:FullRows=\"1\" ss:DefaultRowHeight=\"13.2\">");

            // Append Rows
            for (int row = 0; row < rowcount; row++)
            {
                sb.Append(" <Row>");
                for (int col = 0; col < colcount; col++)
                {
                    //Append Cells
                    style = this.Model[row + top, col + left];
                    string cellcontent = CreateXmlCellContent(style);
                    sb.Append(cellcontent);
                }
                sb.Append(" </Row>");
            }
            sb.Append("</Table>");
            sb.Append("</Worksheet>");
            sb.Append("</Workbook>");
            buffer = sb.ToString();

            Stream xmlStream = new MemoryStream();
            xmlStream.Write(Encoding.ASCII.GetBytes(buffer), 0, buffer.Length);
            Clipboard.SetData("XML Spreadsheet", xmlStream);
            return true;
        }

        /// <summary>
        /// Paste the  XML formatted text in Grid Control. 
        /// </summary>
        /// <param name="xDocument"> Xml document contains the copied data. </param>
        /// <param name="rangeList">Current cell range to paste the copied data</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override bool PasteXmlFromBuffer(System.Xml.XmlDocument xDocument, GridRangeInfoList rangeList)
        {
            int rowcount = 0;
            int columncount = 0;
            if ((this.Model.Options.CopyPasteOption & CopyPaste.PasteCell) == CopyPaste.PasteCell)
            {
                return false;
            }
            if (rangeList == null)
            {
                rangeList = new GridRangeInfoList();
                RowColumnIndex rci = this.Model.CurrentCellState.CellRowColumnIndex;
                rangeList.Add(GridRangeInfo.Cell(rci.RowIndex, rci.ColumnIndex));
            }
            if (!this.Model.CutPaste.PasteBoundCheck(rangeList))
            {
                return false;
            }
            if (Clipboard.ContainsData("XML Spreadsheet"))
            {
                var tablenodes = xDocument.GetElementsByTagName("Table");
                var styles = xDocument.GetElementsByTagName("Style");
                var rownodes = xDocument.GetElementsByTagName("Row");
                if (tablenodes.Count > 0)
                {
                    rowcount = Convert.ToInt32(tablenodes[0].Attributes["ss:ExpandedRowCount"].Value);
                    columncount = Convert.ToInt32(tablenodes[0].Attributes["ss:ExpandedColumnCount"].Value);
                }
                for (int rowindex = 0; rowindex < rowcount; rowindex++)
                {
                    var rownode = rownodes.Item(rowindex);
                    if (rownode != null)
                        for (int colindex = 0; colindex < rownode.ChildNodes.Count; colindex++)
                        {
                            var cell = rownode.ChildNodes[colindex];
                            string data = rownode.ChildNodes[colindex].InnerText;
                            GridStyleInfo style = this.Model[rowindex + rangeList[0].Top, colindex + rangeList[0].Left];
                            PasteTextRowCol(rowindex + rangeList[0].Top, colindex + rangeList[0].Left, data);
                            if (cell.Attributes != null)
                                for (int index = 0; index < cell.Attributes.Count; index++)
                                {
                                    string attribute = cell.Attributes[index].Name;
                                    if (attribute == "ss:Formula")
                                    {
                                        style.Text = cell.Attributes["ss:Formula"].Value;
                                        style.CellType = "FormulaCell";
                                    }
                                    if (attribute == "ss:StyleID")
                                    {
                                        for (int styleindex = 0; styleindex < styles.Count; styleindex++)
                                        {
                                            if (cell.Attributes[index].Value == styles[styleindex].Attributes["ss:ID"].Value)
                                            {
                                                var cellstyles = styles[styleindex];
                                                for (int attributeindex = 0;
                                                     attributeindex < cellstyles.ChildNodes.Count;
                                                     attributeindex++)
                                                {
                                                    var innerstyle = cellstyles.ChildNodes[attributeindex];
                                                    if (innerstyle.LocalName == "Font")
                                                    {
                                                        for (int fontstyle = 0;
                                                             fontstyle < innerstyle.Attributes.Count;
                                                             fontstyle++)
                                                        {
                                                            if (innerstyle.Attributes[fontstyle].Name == "ss:FontName")
                                                            {
                                                                style.Font.FontFamily =
                                                                    new FontFamily(
                                                                        innerstyle.Attributes[fontstyle].Value);
                                                            }
                                                            if (innerstyle.Attributes[fontstyle].Name == "ss:Size")
                                                            {
                                                                style.Font.FontSize =
                                                                    Convert.ToDouble(
                                                                        innerstyle.Attributes[fontstyle].Value);
                                                            }

                                                            if (innerstyle.Attributes[fontstyle].Name == "ss:Color")
                                                            {
                                                                string forecolor = innerstyle.Attributes[fontstyle].Value;
                                                                var conv = new BrushConverter();
                                                                var brush = conv.ConvertFromString("#FF" + forecolor.Substring(1)) as SolidColorBrush;
                                                                if (brush != null)
                                                                {
                                                                    style.Foreground = brush;
                                                                }
                                                            }
                                                            if (innerstyle.Attributes[fontstyle].Name == "ss:Bold")
                                                            {
                                                                if (innerstyle.Attributes[fontstyle].Value == "1")
                                                                {
                                                                    style.Font.FontWeight = FontWeights.Bold;
                                                                }
                                                            }
                                                            if (innerstyle.Attributes[fontstyle].Name == "ss:Italic")
                                                            {
                                                                if (innerstyle.Attributes[fontstyle].Value == "1")
                                                                {
                                                                    style.Font.FontStyle = FontStyles.Italic;
                                                                }
                                                            }
                                                            if (innerstyle.Attributes[fontstyle].Name == "ss:Underline")
                                                            {
                                                                style.Font.TextDecorations = TextDecorations.Underline;
                                                            }
                                                        }
                                                    }
                                                    if (innerstyle.LocalName == "Interior")
                                                    {
                                                        for (int fontstyle = 0;
                                                             fontstyle < innerstyle.Attributes.Count;
                                                             fontstyle++)
                                                        {
                                                            if (innerstyle.Attributes[fontstyle].Name == "ss:Color")
                                                            {
                                                                string color = innerstyle.Attributes[fontstyle].Value;
                                                                var conv = new BrushConverter();
                                                                var brush =
                                                                    conv.ConvertFromString("#FF" + color.Substring(1)) as SolidColorBrush;
                                                                if (brush != null)
                                                                {
                                                                    style.Background = brush;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                        }
                }
                GridRangeInfo pastedrange = GridRangeInfo.Cells(rangeList[0].Top, rangeList[0].Left,
                                                                rangeList[0].Top + (rowcount - 1),
                                                                rangeList[0].Left + (columncount - 1));
                this.Model.InvalidateCell(pastedrange);
                this.Model.SelectedRanges.Clear();
                this.Model.SelectedRanges.Add(pastedrange);
            }
            this.Model.InvalidateVisual(true);
            return true;
        }
#endif

    }
}
