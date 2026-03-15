//-------------------------------------------------------------------------------------------------
// <copyright file="GridModelTextDataExchange.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Security;
using System.Security.Permissions;

using Syncfusion.Collections;
using Syncfusion.ComponentModel;
using Syncfusion.Diagnostics;
using Syncfusion.Drawing;
using Syncfusion.Styles;
using Syncfusion.Windows.Forms;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Manages text data exchange for the grid. Lets you copy cell text to a stream or clipboard and recreate the
    /// cell text at a later time.
    /// </summary>
    public class GridModelTextDataExchange : GridModelBound
    {
        /// <summary>
        /// Initializes a new <see cref="GridModelStyleDataExchange"/> object and associates it 
        /// with a <see cref="GridModel"/>.
        /// </summary>
        /// <param name="model">A reference to the parent <see cref="GridModel"/>.</param>
        public GridModelTextDataExchange(GridModel model)
            : base(model)
        {
        }

        private string m_sImportTabDelim = null;  // default is '\t', used for separating columns when importing text file
        private string m_sExportTabDelim = null;  // default is '\t', used for separating columns when exporting text file

        /// <summary>
        /// Gets or sets the character that is used for separating columns when importing text file.
        /// </summary>
        public string ImportTabDelim
        {
            get
            {
                return m_sImportTabDelim;
            }

            set
            {
                m_sImportTabDelim = value;
            }
        }

        /// <summary>
        /// Gets or sets that is used for separating columns when exporting text file.
        /// </summary>
        public string ExportTabDelim
        {
            get
            {
                return m_sExportTabDelim;
            }

            set
            {
                m_sExportTabDelim = value;
            }
        }

        /// <summary>
        /// Creates a  <see cref="System.String"/> object and initializes it with style objects and covered cell information of a range of cells in the grid.
        /// </summary>
        /// <param name="buffer">A placeholder for the string buffer that is created by the method.</param>
        /// <param name="rangeList">The list with range of cells to be copied.</param>
        /// <param name="nRowsDone">A placeholder where the number of copied rows is returned.</param>
        /// <param name="nColsDone">A placeholder where the number of copied columns is returned.</param>
        /// <returns>True if the operation completed successfully; False otherwise.</returns>
        public bool CopyTextToBuffer(out string buffer, GridRangeInfoList rangeList, out int nRowsDone, out int nColsDone)
        {
            // Store rows / columns indexes to process in an array.
            GridRangeInfoList rowRanges = rangeList.GetRowRanges(GridRangeInfoType.Cells | GridRangeInfoType.Rows);
            GridRangeInfoList colRanges = rangeList.GetColRanges(GridRangeInfoType.Cells | GridRangeInfoType.Cols);

            // Determine number of rows / cols to process.
            int nRows = 0;
            foreach (GridRangeInfo range in rowRanges)
            {
                nRows += range.Height;
            }

            int nCols = 0;
            foreach (GridRangeInfo range in colRanges)
            {
                nCols += range.Width;
            }

            int dwSize = nRows * nCols;

            nRowsDone = 0;
            nColsDone = 0;

            // Status message, let the user abort the operation.
            using (OperationFeedback op = new OperationFeedback(Model))
            {
                op.Description = SR.GetString("GRID_IDM_COPYTEXT");
                op.AllowCancel = true;

                bool canceled = false;
                bool bAnyChar = false;

                string sTabDelim = "\t";
                if (m_sExportTabDelim != null && m_sExportTabDelim.Length > 0)
                {
                    sTabDelim = m_sExportTabDelim;
                }

#if DEBUG
                if (sTabDelim.Length > 1)
                {
                    Trace.WriteLineIf(Switches.DataExchange.TraceWarning, "Warning: the length of m_sExportTabDelim in CopyTextToFile is more than one character!");
                    Trace.WriteLineIf(Switches.DataExchange.TraceWarning, String.Format("Columns will be separated with the following string: {0}", m_sExportTabDelim));
                }
#endif

                StringBuilder sb = new StringBuilder();

                try
                {
                    DateTime start = DateTime.Now;
                    // fill pOldCellsArray row by row
                    for (int rowindex = 0; !canceled && rowindex < rowRanges.Count; rowindex++)
                    {
                        for (int nRow = rowRanges[rowindex].Top; nRow <= rowRanges[rowindex].Bottom; nRow++)
                        {
                            if (DateTime.Now.Subtract(start).TotalSeconds > 5)
                            {
                                Application.DoEvents();
                                start = DateTime.Now;
                            }
                            if (nRowsDone > 0)
                            {
                                sb.Append(Environment.NewLine);
                            }

                            bool firstCol;
                            firstCol = true;
                            nColsDone = 0;

                            for (int colindex = 0; !canceled && colindex < colRanges.Count; colindex++)
                            {
                                for (int nCol = colRanges[colindex].Left; nCol <= colRanges[colindex].Right; nCol++)
                                {
                                    // Store styles in array, but allow user to abort
                                    int dwIndex = (nRowsDone * nCols) + nColsDone;

                                    if (!firstCol)
                                    {
                                        sb.Append(sTabDelim);
                                    }

                                    // Get text from control (and give control the chance
                                    // to convert value into unformatted text or vice versa).
                                    string sText = GetCopyTextRowCol(nRow, nCol);
                                    string UNIQUESTRINGMARKER = ((char)127).ToString();
                                    sText = new StringBuilder(sText)
                                        .Replace(Environment.NewLine, UNIQUESTRINGMARKER)
                                        .Replace("\r", " ")
                                        .Replace("\n", UNIQUESTRINGMARKER)
                                        .ToString()
                                        .Trim();

                                    sb.Append(sText);

                                    bAnyChar |= sText.Length > 0;
                                    firstCol = false;

                                    // check, if user pressed ESC to cancel
                                    if (dwSize > 0)
                                    {
                                        op.PercentComplete = (int)(dwIndex * 100 / dwSize);
                                    }

                                    if (op.ShouldCancel)
                                    {
                                        throw new GridUserCanceledException();
                                    }

                                    nColsDone++;
                                }
                            }

                            nRowsDone++;
                        }
                    }
                }
                catch (GridUserCanceledException ex)
                {
                    TraceUtil.TraceExceptionCatched(ex);
                    if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                    {
                        throw;
                    }

                    canceled = true;
                }

                if (nRowsDone > 1 || nColsDone > 1)
                {
                    sb.Append(Environment.NewLine);
                }

                buffer = sb.ToString();
                ClipboardCopyToBufferEventArgs cb = new ClipboardCopyToBufferEventArgs(buffer,rangeList);
                Model.RaiseClipboardCopyToBuffer(cb);
                if (cb.Handled)
                    buffer = cb.CopyText;
                return !canceled && bAnyChar;
            }
        }

        /// <summary>
        /// Gets the formatted text from the specified row and column.
        /// </summary>
        /// <param name="rowIndex">The row index</param>
        /// <param name="colIndex">The colindex</param>
        /// <returns>returns formatted text</returns>
        internal string GetCopyTextRowCol(int rowIndex, int colIndex)
        {
            // TODO: make customizable
            GridStyleInfo style = Model[rowIndex, colIndex];
            return style.GetFormattedText(style.CellValue, GridCellBaseTextInfo.CopyText);
        }

        /// <summary>
        /// Pastes text from a string buffer with tab-delimited text into a range of cells.
        /// </summary>
        /// <param name="psz">The string buffer with tab-delimited cell's text.</param>
        /// <param name="range">The destination range where text should be pasted.</param>
        /// <param name="dragDropFlags"><see cref="GridDragDropFlags"/> options let you specify if rows or columns can be appended. 
        /// See <see cref="GridDragDropFlags.NoAppendRows"/> and <see cref="GridDragDropFlags.NoAppendCols"/>
        /// of the <see cref="GridDragDropFlags"/> class.</param>
        /// <returns>True if operation completed successfully; False otherwise.</returns>
        public bool PasteTextFromBuffer(string psz, GridRangeInfo range, int dragDropFlags)
        {
            bool canceled = false;

            Model.BeginUpdate(BeginUpdateOptions.InvalidateAndScroll, "PasteTextFromBuffer");
            OperationFeedback op = new OperationFeedback(Model);
            try
            {
                op.AllowRollback = true;
                op.Description = SR.GetString("GRID_IDM_PASTINGDATA");
                op.AllowNestedProgress = false;

                string s;
                int rowIndex, colIndex;

                //// Paste text into cells
                //// (new line will start a new row, tab delimiter will
                //// move to the next column).

                string sTabDelim = "\t";
                if (m_sImportTabDelim != null && m_sImportTabDelim.Length > 0)
                {
                    sTabDelim = m_sImportTabDelim;
                }

                //// Store and deactivate current cell.
                Model.ConfirmChanges();

                Model.CommandStack.BeginTrans(SR.GetString("GRID_IDM_PASTEDATA"));

                rowIndex = range.Top;
                colIndex = range.Left;

                string sTabDelim2 = sTabDelim + sTabDelim;
                psz = psz.Replace(sTabDelim2 + Environment.NewLine, Environment.NewLine);

                int nLastCol = colIndex;
                int size = psz.Length;

                try
                {
                    //// Parse buffer.
                    for (int nIndex = 0, nLast = 0; !canceled && nIndex < size; nIndex++)
                    {
                        //// Check for a delimiter.
                        bool isDelimiter = "\r\n".IndexOf(psz[nIndex]) != -1 || sTabDelim[0] == psz[nIndex];
                        if (nIndex == size - 1 || isDelimiter)
                        {
                            //// End of a string found, copy value to cell.
                            if (rowIndex > Model.RowCount)
                            {
                                if (GridUtil.IsNotSet(dragDropFlags, GridDragDropFlags.NoAppendRows))
                                {
                                    Model.RowCount = rowIndex;
                                }
                                else
                                {
                                    break;
                                }
                            }

                            if (colIndex > Model.ColCount)
                            {
                                if (GridUtil.IsNotSet(dragDropFlags, GridDragDropFlags.NoAppendCols))
                                {
                                    Model.ColCount = colIndex;
                                }
                                else if ("\r\n".IndexOf(psz[nIndex]) != -1)
                                {
                                    if (psz[nIndex] == '\r' && psz[nIndex + 1] == '\n')
                                    {
                                        nIndex++;
                                        colIndex = range.Left;
                                        nLast = nIndex + 1;
                                        rowIndex++;
                                        continue;
                                    }
                                }
                                else
                                {
                                    continue;
                                }
                            }

                            if (rowIndex <= Model.RowCount && colIndex <= Model.ColCount)
                            {
                                if (isDelimiter)
                                {
                                    if (nIndex != nLast)
                                    {
                                        s = psz.Substring(nLast, nIndex - nLast);
                                    }
                                    else
                                    {
                                        s = String.Empty;
                                    }
                                }
                                else
                                {
                                    s = psz.Substring(nLast);
                                }

                                ////GridStyleInfo style = null;
                                ////Model.ComposeTextRowCol(rowIndex, colIndex, ref style);
                                ////style = Model[rowIndex, colIndex];

                                //// Give the control the chance to validate
                                //// and change the pasted text.
                                canceled = !PasteTextRowCol(rowIndex, colIndex, s);
                            }

                            //// Abort parsing the string if next char
                            //// is an end-of-string or if canceled.
                            if (canceled || nIndex == size - 1)
                            {
                                break;
                            }                               
                            else if ("\r\n".IndexOf(psz[nIndex]) != -1)
                            { 
                                //// Now that the value has been copied to the cell,
                            //// let's check if we should jump to a new row.

                                //// Yes, I found an end of line.
                                //// Let's increase rowIndex and reset colIndex to first column.
                                rowIndex++;
                                colIndex = range.Left;
                                if (psz[nIndex] == '\r' && psz[nIndex + 1] == '\n')
                                {
                                    nIndex++;
                                }

                                //// Abort parsing the string if next char
                                //// is an end-of-string.
                                if (nIndex == size - 1)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                colIndex++;  // Move to next column.
                            }

                            //// Save index where the next cell's value starts.
                            nLast = nIndex + 1;

                            nLastCol = Math.Max(colIndex, nLastCol);

                            //// Check, if user pressed ESC to cancel.
                            if (size > 0)
                            {
                                op.PercentComplete = (int)(nIndex * 100 / size);
                            }
                           
                            if (op.ShouldCancel)
                            {
                                throw new GridUserCanceledException();
                            }
                        }
                    }
                }
                catch (GridUserCanceledException ex)
                {
                    TraceUtil.TraceExceptionCatched(ex);
                    if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                    {
                        throw;
                    }

                    canceled = true;
                }

                if (canceled && op.RollbackConfirmed)
                {
                    Model.CommandStack.Rollback();
                }
                else
                {
                    Model.CommandStack.CommitTrans();
                }

                ////Model.Invalidate(range.Top, range.Left, rowIndex, nLastCol, GridSetCurrentCellOptions.ForceUpdate, true);

                //// Also formula refresh cells that have references to the pasted cells.
                Model.Refresh();

                return !canceled;
            }
            finally
            {
                op.Close();
                Model.EndUpdate();
            }
        }

        /// <summary>
        /// Raises a <see cref="GridModel.PasteCellText"/> event and calls GridStyleInfo.ApplyFormattedText for
        /// the specific cell.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="text">The text to be pasted.</param>
        /// <returns>True if text could be pasted; False otherwise.</returns>
        public bool PasteTextRowCol(int rowIndex, int colIndex, string text)
        {
            GridStyleInfo style = Model[rowIndex, colIndex];
            GridPasteCellTextEventArgs e = new GridPasteCellTextEventArgs(rowIndex, colIndex, style, text, false);
            Model.RaisePasteCellText(e);
            if (e.Abort)
            {
                return false;
            }

            if (e.Cancel)
            {
                return true;
            }

            if (!Model.IgnoreReadOnly && (Model.ReadOnly || style.ReadOnly))
            {
                return false;
            }
            string  UNIQUESTRINGMARKER = ((char)127).ToString();
            if (e.Text.IndexOf(UNIQUESTRINGMARKER) != -1)
            {
                e.Text = e.Text.Replace(UNIQUESTRINGMARKER, Environment.NewLine);
            }

            return style.ApplyFormattedText(e.Text, GridCellBaseTextInfo.PasteText);
        }

        /// <summary>
        /// Creates an ArrayList of rows where each row is an ArrayList of values that belong to that row 
        /// from a tab-delimited input string.
        /// </summary>
        /// <param name="psz">Tab separated values as string. Newline characters indicate a new row.</param>
        /// <param name="nRows">The number of rows extracted from the string.</param>
        /// <param name="nCols">The number of columns extracted from the string</param>
        /// <param name="op">An OperationFeedback object.</param>
        /// <returns>The resulting ArrayList with rows and values for each row.</returns>
        public ArrayList CreateTableFromCVSBuffer(string psz, out int nRows, out int nCols, OperationFeedback op)
        {
            bool canceled = false;
            ArrayList array = new ArrayList();

            try
            {
                string s;
                //// Paste text into cells
                //// (new line will start a new row, tab delimiter will
                //// move to the next column).

                string sTabDelim = "\t";
                if (m_sImportTabDelim != null && m_sImportTabDelim.Length > 0)
                {
                    sTabDelim = m_sImportTabDelim;
                }

                int rowIndex = 0;
                int colIndex = 0;

                int nLastCol = colIndex;
                int size = psz.Length;

                ArrayList rowArray = new ArrayList();

                //// Parse buffer. 
                for (int nIndex = 0, nLast = 0; !canceled && nIndex < size; nIndex++)
                {
                    //// Check for a delimiter.
                    bool isDelimiter = "\r\n".IndexOf(psz[nIndex]) != -1 || sTabDelim[0] == psz[nIndex];
                    if (nIndex == size - 1 || isDelimiter)
                    {
                        //// End of a string found, copy value to cell.
                        if (isDelimiter)
                        {
                            if (nIndex != nLast)
                            {
                                s = psz.Substring(nLast, nIndex - nLast);
                            }
                            else
                            {
                                s = String.Empty;
                            }
                        }
                        else
                        {
                            s = psz.Substring(nLast);
                        }

                        rowArray.Add(s);

                        // Abort parsing the string if next char
                        // is an end-of-string or if canceled.
                        if (canceled || nIndex == size - 1)
                        {
                            break;
                        }
                        else if ("\r\n".IndexOf(psz[nIndex]) != -1)
                        {
                            // Now that the value has been copied to the cell,
                        // let's check if we should jump to a new row.

                            // Yes, I found an end of line.
                            // Let's increase rowIndex and reset colIndex to first column.
                            array.Add(rowArray);
                            rowArray = new ArrayList(nLastCol);
                            rowIndex++;
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
                            colIndex++; // move to next column
                        }

                        // Save index where the next cell's value starts.
                        nLast = nIndex + 1;

                        nLastCol = Math.Max(colIndex, nLastCol);

                        // Check, if user pressed ESC to cancel.
                        if (op != null)
                        {
                            if (size > 0)
                            {
                                op.PercentComplete = (int)(nIndex * 100 / size);
                            }

                            if (op.ShouldCancel)
                            {
                                throw new GridUserCanceledException();
                            }
                        }
                    }
                }

                if (colIndex > 0 || rowArray.Count > 0)
                {
                    array.Add(rowArray);
                }

                nRows = array.Count;
                nCols = nRows > 0 ? nLastCol + 1 : 0;
            }
            finally
            {
            }

            return array;
        }

        /// <summary>
        /// Determines the number of rows and columns in a tab-delimited string buffer.
        /// </summary>
        /// <param name="psz">The tab-delimited string buffer.</param>
        /// <param name="nRows">A placeholder where the calculated number of rows is returned.</param>
        /// <param name="nCols">A placeholder where the calculated number of columns is returned.</param>
        /// <returns>True if the passed in buffer is a valid tab-delimited text; False otherwise.</returns>
        public bool CalcBufferDimension(string psz, out int nRows, out int nCols)
        {
            int nRowsDone = 0,
                nColsDone = 0;

            int colIndex = 0;
            bool canceled = false;

            //// (New line will start a new row, tab delimiter will
            //// move to the next column).

            string sTabDelim = "\t";
            if (m_sImportTabDelim != null && m_sImportTabDelim.Length > 0)
            {
                sTabDelim = m_sImportTabDelim;
            }

            int size = psz.Length;

            for (int nIndex = 0, nLast = 0; !canceled && nIndex < size; nIndex++)
            {
                //// Check for a delimiter
                if (nIndex == size - 1 || "\r\n".IndexOf(psz[nIndex]) != -1 || sTabDelim[0] == psz[nIndex])
                {
                    //// Abort parsing the string if next char
                    //// is an end-of-string.
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
    }
}
