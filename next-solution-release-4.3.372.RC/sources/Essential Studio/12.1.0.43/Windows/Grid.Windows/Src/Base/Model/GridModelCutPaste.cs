//-------------------------------------------------------------------------------------------------
// <copyright file="GridModelCutPaste.cs" company="syncfusion">
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
    /// This class manages cut, copy, and paste for a grid.
    /// </summary>
    /// <remarks>
    /// You access this class from a grid with the <see cref="GridModel.CutPaste"/>
    /// property of a <see cref="GridModel"/> instance.
    /// </remarks>
    public class GridModelCutPaste : GridModelBound
    {
        [NonSerialized]
        DataObject m_dataObject = null;
        internal int m_nClipboardFlags = GridDragDropFlags.Text | GridDragDropFlags.Styles | GridDragDropFlags.Compose;
        
        [NonSerialized]
        internal GridRangeInfoList m_CopyRangeList = null;
        
        [NonSerialized]
        internal bool m_bCut = false;
       
        [NonSerialized]
        internal bool m_bDirectCopyPaste = false;
       
        [NonSerialized]
        internal bool m_bDirectCutPaste = false;

        /// <summary>
        /// Initializes a <see cref="GridModelCutPaste"/> and associates it 
        /// with a <see cref="GridModel"/>.
        /// </summary>
        /// <param name="model">A reference to the parent <see cref="GridModel"/></param>.
        public GridModelCutPaste(GridModel model)
            : base(model)
        {
        }

        /// <summary>
        /// Gets or sets various <see cref="GridDragDropFlags"/> options that specify how clipboard cut, copy, and paste
        /// should be handled.
        /// </summary>
        /// <remarks>
        /// You can specify here if you only want top copy / paste text and if row or column header text should be included.
        /// </remarks>
        public int ClipboardFlags
        {
            get
            {
                return m_nClipboardFlags;
            }

            set
            {
                m_nClipboardFlags = value;
            }
        }

        /// <summary>
        /// Checks if there are selected ranges that can be copied to clipboard or if the current cell's contents can be copied.
        /// </summary>
        /// <returns>True if there is information available to be copied to clipboard.</returns>
        /// <remarks>
        /// Call this method for example to enable or gray out menu commands like "Copy Cells".
        /// </remarks>
        public bool CanCopy()
        {
            GridCutPasteEventArgs e = new GridCutPasteEventArgs(false, true, m_nClipboardFlags, Model.SelectedRanges);
            Model.RaiseClipboardCanCopy(e);
            this.m_nClipboardFlags = e.ClipboardFlags;
            if (e.Handled)
            {
                return e.Result;
            }

            if (Model.SelectedRanges.Count > 0)
            {
                return true;
            }

            if (!e.IgnoreCurrentCell)
            {
                GridCurrentCellInfo cci = Model.CurrentCellInfo;
                if (cci != null)
                {
                    return cci.CellView.CanCopy();
                }
            }

            return false;
        }

        /// <summary>
        /// Copies the contents of cells in selected to clipboard and the current cell's contents.
        /// </summary>
        /// <returns>True if information was available and copied to clipboard.</returns>
        /// <remarks>
        /// If there are no selected ranges, the current cell's <see cref="GridCellRendererBase.Copy"/> method is called.
        /// Otherwise, selected ranges will be copied. <para/>
        /// See the <see cref="ClipboardFlags"/> property how to customize the default behavior.
        /// </remarks>
        public bool Copy()
        {
            bool bRangeSel = Model.Selections.GetSelectedRanges(out m_CopyRangeList, false);

            GridCutPasteEventArgs e = new GridCutPasteEventArgs(false, true, m_nClipboardFlags, m_CopyRangeList);
            Model.RaiseClipboardCopy(e);
            this.m_CopyRangeList = e.RangeList;
            this.m_dataObject = e.DataObject;
            this.m_nClipboardFlags = e.ClipboardFlags;
            if (e.Handled)
            {
                return e.Result;
            }

            GridCellRendererBase cellRenderer = null;
            if (!e.IgnoreCurrentCell)
            {
                cellRenderer = Model.CurrentCellRenderer;
            }

            bool bActive = cellRenderer != null && cellRenderer.HasFocusControl;
            bool success = false;

            // If there are no selections, give current cell the
            // chance to copy its prefered clipboard format.
            if ((!bRangeSel || bActive) && cellRenderer != null && cellRenderer.Copy())
            {
                success = true;
            }               
            else if (bRangeSel || (!e.IgnoreCurrentCell && Model.Selections.GetSelectedRanges(out m_CopyRangeList, true)))
            {
                //// If there are no selections, add current cell.
                using (OperationFeedback op = new OperationFeedback(Model))
                {
                    // Loop through all selected ranges and expand them.
                    // Row and column headers will be excluded from expanded range.
                    int firstRow = 0;
                    int firstCol = 0;
                    int rowCount = Model.RowCount;
                    int colCount = Model.ColCount;

                    if (GridUtil.IsNotSet(m_nClipboardFlags, GridDragDropFlags.RowHeader))
                    {
                        firstCol = Model.Cols.HeaderCount + 1;
                    }

                    if (GridUtil.IsNotSet(m_nClipboardFlags, GridDragDropFlags.ColHeader))
                    {
                        firstRow = Model.Rows.HeaderCount + 1;
                    }

                    m_CopyRangeList = m_CopyRangeList.ExpandRanges(
                        firstRow,
                        firstCol,
                        rowCount,
                        colCount);

                    success = CopyRange(m_CopyRangeList);
                }
            }

            if (m_bCut)
            {
                m_bCut = false;
                Model.Refresh();
            }

            return success;
        }

        /// <overload>
        /// Copies the contents of a specified range of cells to clipboard.
        /// </overload>
        /// <summary>
        /// Copies the contents of a specified range of cells to clipboard.
        /// </summary>
        /// <param name="range">The range with cells to be copied.</param>
        /// <returns>True if information was available and copied to clipboard.</returns>
        /// <remarks>
        /// See the <see cref="ClipboardFlags"/> property how to customize the default behavior.
        /// </remarks>
        public bool CopyRange(GridRangeInfo range)
        {
            GridRangeInfoList rangeList = new GridRangeInfoList();
            rangeList.Add(range);
            return CopyRange(rangeList);
        }

        /// <summary>
        /// Copies the contents of a specified range of cells to clipboard.
        /// </summary>
        /// <param name="rangeList">The range list with cells to be copied.</param>
        /// <returns>True if information was available and copied to clipboard.</returns>
        /// <genoverload/>
        public bool CopyRange(GridRangeInfoList rangeList)
        {
            using (OperationFeedback op = new OperationFeedback(Model))
            {
                bool success = false;

                if (GridUtil.IsSet(m_nClipboardFlags, GridDragDropFlags.Text))
                {
                    op.SeriesCount = 2;
                }

                if (GridUtil.IsSet(m_nClipboardFlags, GridDragDropFlags.Text))
                {
                    success |= CopyTextToClipboard(rangeList);
                }

                if (op.ShouldCancel)
                {
                    return false;
                }

                if (GridUtil.IsSet(m_nClipboardFlags, GridDragDropFlags.Styles))
                {
                    success |= CopyCellsToClipboard(rangeList, false);
                }

                if (m_bDirectCopyPaste || m_bDirectCutPaste)
                {
                    try
                    {
                        if (m_dataObject == null)
                        {
                            m_dataObject = new DataObject();
                        }
                        Application.DoEvents();
                        m_dataObject.SetData(rangeList.Clone());  // Create an id and store it in Param. This
                        // unique id will later help identify that paste is done inside the same grid.
                        Clipboard.SetDataObject(m_dataObject);
                    }
                    catch (Exception ex)
                    {
                        TraceUtil.TraceExceptionCatched(ex);
                        if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                        {
                            throw;
                        }
                    }
                }

                m_dataObject = null;

                // Check if canceled by user or failed for other reasons.
                return success;
            }
        }

        /// <summary>
        /// Copies the formatted text of a specified range of cells to clipboard.
        /// </summary>
        /// <param name="rangeList">The range list with cells to be copied.</param>
        /// <returns>True if information was available and copied to clipboard.</returns>
        public bool CopyTextToClipboard(GridRangeInfoList rangeList)
        {
            string buffer;
            int rowCount, colCount;
            try
            {
                if (Model.TextDataExchange.CopyTextToBuffer(out buffer, rangeList, out rowCount, out colCount))
                {
                    if (m_dataObject == null)
                    {
                        m_dataObject = new DataObject();
                    }

                    m_dataObject.SetData(DataFormats.UnicodeText, buffer);
                    Clipboard.SetDataObject(m_dataObject);
                    return true;
                }
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                {
                    throw;
                }

                return false;
            }

            return false;
        }

        /// <summary>
        /// Copies the style information of a specified range of cells to clipboard.
        /// </summary>
        /// <param name="rangeList">The range list with cells to be copied.</param>
        /// <param name="bLoadBaseStyles">True if information from base styles should also be copied; False if only
        /// the settings that were initialized for the cells should be copied.</param>
        /// <returns>True if information was available and copied to clipboard.</returns>
        public bool CopyCellsToClipboard(GridRangeInfoList rangeList, bool bLoadBaseStyles)
        {
            GridData data;
            int rowCount, colCount;
            try
            {
                if (Model.DataExchange.CopyCellsToDataObject(out data, rangeList, bLoadBaseStyles, m_nClipboardFlags, out rowCount, out colCount))
                {
                    if (m_dataObject == null)
                    {
                        m_dataObject = new DataObject();
                    }
                    Application.DoEvents();
                    m_dataObject.SetData(data);
                    Clipboard.SetDataObject(m_dataObject);
                    return true;
                }
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                {
                    throw;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if there are selected ranges that can be cut and copied to clipboard or if the current cell's contents can be cut and copied.
        /// </summary>
        /// <returns>True if there is information available to be cut and copied to clipboard.</returns>
        /// <remarks>
        /// Call this method for example to enable or gray out menu commands like "Cut Cells".
        /// </remarks>
        public bool CanCut()
        {
            GridCutPasteEventArgs e = new GridCutPasteEventArgs(false, true, m_nClipboardFlags, Model.SelectedRanges);
            Model.RaiseClipboardCanCut(e);
            this.m_nClipboardFlags = e.ClipboardFlags;
            if (e.Handled)
            {
                return e.Result;
            }

            if (Model.SelectedRanges.Count > 0)
            {
                return true;
            }
            else if (e.IgnoreCurrentCell)
            {
                return false;
            }

            GridCellRendererBase cellRenderer = Model.CurrentCellRenderer;
            return cellRenderer != null && cellRenderer.CanCut();
        }

        /// <summary>
        /// Cuts and copies the contents of cells in selected to clipboard and the current cell's contents.
        /// </summary>
        /// <returns>True if information was available and copied to clipboard.</returns>
        /// <remarks>
        /// If there are no selected ranges, the current cell's <see cref="GridCellRendererBase.Cut"/> method is called.
        /// Otherwise, selected ranges will be copied. <para/>
        /// See the <see cref="ClipboardFlags"/> property how to customize the default behavior.
        /// </remarks>
        public bool Cut()
        {
            OperationFeedback op = new OperationFeedback(Model);
            try
            {
                bool bRangeSel = Model.Selections.GetSelectedRanges(out m_CopyRangeList, false);
                GridCutPasteEventArgs e = new GridCutPasteEventArgs(false, true, m_nClipboardFlags, m_CopyRangeList);
                Model.RaiseClipboardCut(e);
                this.m_CopyRangeList = e.RangeList;
                this.m_dataObject = e.DataObject;
                this.m_nClipboardFlags = e.ClipboardFlags;
                if (e.Handled)
                {
                    return e.Result;
                }

                GridCellRendererBase cellRenderer = null;
                if (!e.IgnoreCurrentCell)
                {
                    cellRenderer = Model.CurrentCellRenderer;
                }

                bool bActive = cellRenderer != null && cellRenderer.HasFocusControl;
                bool success = false;

                // If there are no selections, give current cell the
                // chance to copy its prefered clipboard format.
                if ((!bRangeSel || bActive) && cellRenderer != null && cellRenderer.Cut())
                {
                    success = true;
                }
                else if (m_bDirectCutPaste)
                {
                    // Copy will set up GetSelectedRanges.
                    // Data will be cut later when user gives Paste SyncfusionCommand.
                    Copy();
                    m_bCut = true;

                    // this will redraw the cells greyed out
                    Model.Refresh();

                    success = true;
                }
                else if (bRangeSel || (!e.IgnoreCurrentCell && Model.Selections.GetSelectedRanges(out m_CopyRangeList, true)))
                {
                    // Determine agian list of selections (if necessary add current cell).
                    Model.Selections.GetSelectedRanges(out m_CopyRangeList, true);

                    success = CutRange(m_CopyRangeList, GridUtil.IsSet(m_nClipboardFlags, GridDragDropFlags.Styles));
                }

                return success;
            }
            finally
            {
                op.Close();
            }
        }

        /// <overload>
        /// Cuts and copies the contents of a specified range of cells to clipboard.
        /// </overload>
        /// <summary>
        /// Cuts and copies the contents of a specified range of cells to clipboard.
        /// </summary>
        /// <param name="range">The range with cells to be cut and copied.</param>
        /// <param name="clearStyle">True if you want to clear all cell information; False if you only want to clear text.</param>
        /// <returns>True if information was available and copied to clipboard.</returns>
        /// <remarks>
        /// See the <see cref="ClipboardFlags"/> property how to customize the default behavior.
        /// </remarks>
        public bool CutRange(GridRangeInfo range, bool clearStyle /*=true*/)
        {
            GridRangeInfoList rangeList = new GridRangeInfoList();
            rangeList.Add(range);
            return CutRange(rangeList, clearStyle);
        }

        /// <summary>
        /// Cuts and copies the contents of a specified range of cells to clipboard.
        /// </summary>
        /// <param name="rangeList">The range list with cells to be copied.</param>
        /// <param name="clearStyle">True if you want to clear all cell information; False if you only want to clear text.</param>
        /// <returns>True if information was available and copied to clipboard.</returns>
        /// <genoverload/>
        public bool CutRange(GridRangeInfoList rangeList, bool clearStyle)
        {
            Model.CommandStack.BeginTrans(SR.GetString("GRID_IDM_CUTDATA"));

            // First, copy data.
            if (Copy() && Model.ClearCells(rangeList, clearStyle))
            {
                Model.CommandStack.CommitTrans();
                return true;
            }
            else
            {
                // If failed, rollback and do not clear any cells.
                Model.CommandStack.Rollback();
                return false;
            }
        }

        /// <summary>
        /// Checks if there is information on the clipboard that can be pasted into the grid.
        /// </summary>
        /// <returns>True if there is information available to be pasted into the grid.</returns>
        /// <remarks>
        /// Call this method for example to enable or gray out menu commands like "Paste Cells".
        /// </remarks>
        public bool CanPaste()
        {
            GridCutPasteEventArgs e = new GridCutPasteEventArgs(false, true, m_nClipboardFlags, Model.SelectedRanges);
            Model.RaiseClipboardCanPaste(e);
            this.m_nClipboardFlags = e.ClipboardFlags;
            if (e.Handled)
            {
                return e.Result;
            }

            if (Model.IsReadOnly && !Model.IgnoreReadOnly)
            {
                return false;
            }

            ////            if (m_CopyRangeList != null && m_CopyRangeList.Count > 0 && 
            ////                (m_bDirectCutPaste && m_bCut 
            ////                || m_bDirectCopyPaste && !m_bCut))
            ////                return true;

            return true;
        }

        ////        /// <summary>
        ////        /// Checks if there is information on the clipboard that can be pasted into the grid.
        ////        /// </summary>
        ////        public bool OnCheckClipboardFormat()
        ////        {
        ////            //// TODO: Provide a mechanism to customize this behavior.
        ////            return true;
        ////        }
        ////
        private string GetClipboardText()
        {
            string buffer = null;
            IDataObject iData = null;
            if (GridUtil.IsSet(m_nClipboardFlags, GridDragDropFlags.Styles | GridDragDropFlags.Text))
            {
                iData = Clipboard.GetDataObject();
            }
            if (GridUtil.IsSet(m_nClipboardFlags, GridDragDropFlags.Text)
                            && iData != null)
            {
                if (iData.GetDataPresent(DataFormats.UnicodeText))
                {
                    buffer = iData.GetData(DataFormats.UnicodeText) as string;
                }
                else if (iData.GetDataPresent(DataFormats.Text))
                {
                    buffer = iData.GetData(DataFormats.Text) as string;
                }
            }
            return buffer;
        }

        /// <summary>
        /// Paste information from the clipboard into the grid at the current selected range or current cell.
        /// </summary>
        /// <returns>True if information was available and pasted from the clipboard.</returns>
        /// <remarks>
        /// If there are no selected ranges, the contents are pasted starting at the current cell's position.
        /// Otherwise, contents will be pasted into the current selected range. <para/>
        /// See the <see cref="ClipboardFlags"/> property how to customize the default behavior.
        /// </remarks>
        public bool Paste()
        {
            if (!CanPaste())
            {
                return false;
            }

            GridCutPasteEventArgs e = new GridCutPasteEventArgs(false, true, m_nClipboardFlags, m_CopyRangeList);
            Model.RaiseClipboardPaste(e);
            this.m_CopyRangeList = e.RangeList;
            this.m_dataObject = e.DataObject;
            this.m_nClipboardFlags = e.ClipboardFlags;
            if (e.Handled)
            {
                if (e.Result)
                {
                    Model.RaiseClipboardPasted(e);
                }

                return e.Result;
            }

            // Verify that only one range or current cell is selected.
            GridRangeInfoList rangeList;
            if (!Model.Selections.GetSelectedRanges(out rangeList, true))
            {
                return false;
            }

            if (!e.IgnoreCurrentCell)
            {
                // Give the current cell a chance
                GridCellRendererBase cellRenderer = Model.CurrentCellRenderer;
                //if (cellRenderer != null && cellRenderer.CanPaste() && cellRenderer.Paste())
                if (cellRenderer != null && cellRenderer.CanPaste())
                {
                    bool proceedPaste = true;
                    bool pasteStatus = false; // initialized to check the paste status
                    string buffer = this.GetClipboardText();

                    if (buffer != null)
                    {
                        GridPasteCellTextEventArgs arg = new GridPasteCellTextEventArgs(model.currentRow, model.currentCol,
                            cellRenderer.StyleInfo, buffer, false);
                        Model.RaisePasteCellText(arg);

                        if (arg.Abort || arg.Cancel)
                            proceedPaste = false;
                    }
                    if (proceedPaste)
                    {
                        pasteStatus = cellRenderer.Paste();

                        // Added paste status check
                        if (pasteStatus)
                        {
                            Model.RaiseClipboardPasted(e);
                            return true;
                        }
                    }
                    else
                        return false;
                }
            }

            GridRangeInfo outerRange = rangeList.GetOuterRange(rangeList.ActiveRange);
            ////            if (rangeList.Count > 1)
            ////            {
            ////                //// only one selected range is allowed
            ////                MessageBox.Show(SR.GetString("Grid_IDM_PASTENOTMULTI"));
            ////                return false;
            ////            }

            int firstRow = 0;
            int firstCol = 0;
            int rowCount = Model.RowCount;
            int colCount = Model.ColCount;

            if (GridUtil.IsNotSet(m_nClipboardFlags, GridDragDropFlags.RowHeader))
            {
                firstRow = Model.Rows.HeaderCount + 1;
            }

            if (GridUtil.IsNotSet(m_nClipboardFlags, GridDragDropFlags.ColHeader))
            {
                firstCol = Model.Cols.HeaderCount + 1;
            }

            GridRangeInfo range = outerRange.ExpandRange(firstRow, firstCol, rowCount, colCount);

            // bool bPasteDirect = false;

            ////            //// Cut/Paste directly in sheet (no clipboard)
            ////            if (m_CopyRangeList != null && m_CopyRangeList.Count > 0 
            ////                && (m_bDirectCutPaste && m_bCut 
            ////                || m_bDirectCopyPaste && !m_bCut))
            ////            {
            ////                if (m_bDirectCutPaste && m_bCut)
            ////                    bPasteDirect = true;
            ////
            ////                /*else if (IsClipboardFormatAvailable(cfDirectCopy()))
            ////                            bPasteDirect = p1 == p2;
            ////                    }*/
            ////            }

            bool success = false;
            // if (bPasteDirect)
            // {
            //    success = OnPasteDirect(range);
            // }
            // else
            {
                //// If you want to support additional clipboard format,
                //// you should override the following method:
                success = OnPasteFromClipboard(range);
            }

            if (success)
            {
                Model.RaiseClipboardPasted(e);
            }

            return success;
        }

        /// <summary>
        /// Not yet implemented.
        /// </summary>
        /// <param name="range">The Grid Range</param>
        /// <returns>returns boolean value.</returns>
        internal bool OnPasteDirect(GridRangeInfo range)
        {
            // TODO: Direct copy paste from grid cells.
            return true;
        }

        internal bool OnPasteFromClipboard(GridRangeInfo rg)
        {
            int firstRow = 0;
            int firstCol = 0;

            if (GridUtil.IsNotSet(m_nClipboardFlags, GridDragDropFlags.RowHeader))
            {
                firstRow = Model.Rows.HeaderCount + 1;
            }

            if (GridUtil.IsNotSet(m_nClipboardFlags, GridDragDropFlags.ColHeader))
            {
                firstCol = Model.Cols.HeaderCount + 1;
            }

            GridRangeInfo range = GridRangeInfo.Cells(Math.Max(firstRow, rg.Top), Math.Max(firstCol, rg.Left), rg.Bottom, rg.Right);

            IDataObject iData = null;
            if (GridUtil.IsSet(m_nClipboardFlags, GridDragDropFlags.Styles | GridDragDropFlags.Text))
            {
                iData = Clipboard.GetDataObject();
            }

            if (GridUtil.IsSet(m_nClipboardFlags, GridDragDropFlags.Styles)
                && iData.GetDataPresent(typeof(GridData)))
            {
                GridData data = iData.GetData(typeof(GridData)) as GridData;
                if (data != null)
                {
                    return Model.DataExchange.PasteCellsFromDataObject(data, range, GridUtil.IsNotSet(m_nClipboardFlags, GridDragDropFlags.CheckRangeDim), m_nClipboardFlags);
                }
            }

            string buffer = GetClipboardText();
       
            if (buffer != null)
            {
                return Model.TextDataExchange.PasteTextFromBuffer(buffer, range, m_nClipboardFlags);
            }        

            return false;
        }

        internal bool OnPasteDiffRange()
        {
            // This method is called when the selected range of
            // cells differs from the range of cells currently
            // available on the clipboard.
            //
            // Override this method if you don't want that this
            // message box is displayed.

            // Return False if paste-operation should be canceled.
            if (this.model.ActiveGridView != null)
            {
                return MessageBox.Show(FindFormHelper.FindForm(this.model.ActiveGridView), SR.GetString("GRID_IDM_PASTEDIFFRANGE"), "Paste Cells", MessageBoxButtons.OKCancel) == DialogResult.OK;
            }
            else
            { 
                return true;
            }
        }
    }
}
