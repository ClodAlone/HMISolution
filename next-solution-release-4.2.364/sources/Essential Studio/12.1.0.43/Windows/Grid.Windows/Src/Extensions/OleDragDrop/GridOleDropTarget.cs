//-------------------------------------------------------------------------------------------------
// <copyright file="GridOleDropTarget.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using Syncfusion.Diagnostics;
using Syncfusion.Styles;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Grid;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Not implemented. Reserved for future use - possibly return options how to handle operation, e.g. EdgeScroll
    ///  or implement this behavior itself.
    /// </summary>
    public class GridQueryAcceptDataOptions
    {
        // Reserved for later use.
        internal int dummy = 0;

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public GridQueryAcceptDataOptions()
            : base()
        {
        }
    }

    /// <summary>
    /// Implement this interface in order to participate as a possible DataObject consumer. See <see cref="GridControlBaseImp.RegisterDataObjectConsumer"/>.
    /// </summary>
    public interface IGridDataObjectConsumer
    {
        /// <summary>
        /// Gets the name of the DataObject consumer.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Queries the DataObject consumer if it knows how to handle the IDataObject.
        /// </summary>
        /// <param name="dataObject">Provides data to be consumed.</param>
        /// <param name="consumer">Another consumer that is capable of reading the data. Might be NULL.</param>
        /// <param name="options">Reserved for future use.</param>
        /// <returns>True if this consumer is able to read the data from <paramref name="dataObject"/>.</returns>
        bool QueryAcceptData(IDataObject dataObject, IGridDataObjectConsumer consumer, GridQueryAcceptDataOptions options);

        /// <summary>
        /// Queries the dimension in rows and columns of the data object.
        /// </summary>
        /// <param name="dataObject">The data object.</param>
        /// <returns>returns size</returns>
        Size DetermineRowColCount(IDataObject dataObject);

        /// <summary>
        /// Paste the contents of the data object at the specified cell coordinates.
        /// </summary>
        /// <param name="dataObject">Provides data to be consumed.</param>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <returns>True if successful; False otherwise.</returns>
        bool DropAtRowCol(IDataObject dataObject, int rowIndex, int colIndex);
    }

    /// <summary>
    /// Implements a DataObject consumer for serialized GridData. Checks if the provided data are of type <see cref="GridData"/>.
    /// </summary>
    public class GridDataDataObjectConsumer : GridSubComponent, IGridDataObjectConsumer
    {
        GridControlBase grid;

        /// <summary>
        /// Initializes a new <see cref="GridDataDataObjectConsumer"/> object and associates it with a <see cref="GridControlBase"/>.
        /// </summary>
        /// <param name="grid">The <see cref="GridControlBase"/> this object should be associated with.</param>
        public GridDataDataObjectConsumer(GridControlBase grid)
            : base(grid)
        {
            this.grid = grid;
        }

        /// <summary>
        /// Gets the name of the DataObject consumer.
        /// </summary>
        public string Name
        {
            get { return "GridData"; }
        }

        /// <summary>
        /// Queries the DataObject consumer if it knows how to handle the IDataObject.
        /// </summary>
        /// <param name="dataObject">Provides data to be consumed.</param>
        /// <param name="consumer">Another consumer that is capable of reading the data. Might be NULL.</param>
        /// <param name="options">Reserved for future use.</param>
        /// <returns>True if this consumer is able to read the data from <paramref name="dataObject"/>.</returns>
        public bool QueryAcceptData(IDataObject dataObject, IGridDataObjectConsumer consumer, GridQueryAcceptDataOptions options)
        {
            if (!GridUtil.IsSet(grid.Model.Options.DragDropDropTargetFlags, GridDragDropFlags.Styles))
            {
                return false;
            }

            if (!grid.Model.IgnoreReadOnly && grid.Model.ReadOnly)
            {
                return false;
            }

            // Let other consumers other than "Text" have higher priority than this one.
            if (consumer != null && consumer.Name != "Text")
            {
                return false;
            }

            return dataObject.GetDataPresent(typeof(GridData));
        }

        /// <summary>
        /// Queries the dimension in rows and columns of the data object.
        /// </summary>
        /// <param name="dataObject">The data object.</param>
        /// <returns>returns Dimension.</returns>
        public Size DetermineRowColCount(IDataObject dataObject)
        {
            int rowCount = 0;
            int colCount = 0;

            if (dataObject.GetDataPresent(typeof(GridData)))
            {
                GridData data = (GridData)dataObject.GetData(typeof(GridData));
                rowCount = data.RowCount;
                colCount = data.ColCount;
            }

            return new Size(colCount, rowCount);
        }

        /// <summary>
        /// Paste the contents of the data object at the specified cell coordinates.
        /// </summary>
        /// <param name="dataObject">Provides data to be consumed.</param>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <returns>True if successful; False otherwise.</returns>
        public bool DropAtRowCol(IDataObject dataObject, int rowIndex, int colIndex)
        {
            if (dataObject.GetDataPresent(typeof(GridData)))
            {
                GridModel.InternalGridDragDropData.dndGridTargetStyle = true;
                GridData data = (GridData)dataObject.GetData(typeof(GridData));
                return grid.Model.DataExchange.PasteCellsFromDataObject(data, GridRangeInfo.Cell(rowIndex, colIndex), true, grid.Model.Options.DragDropDropTargetFlags);
            }

            return false;
        }
    }

    /// <summary>
    /// Implements a DataObject consumer for text data. Will handle data provided in DataFormats.Text and DataFormats.UnicodeText format.
    /// </summary>
    public class GridTextDataObjectConsumer : GridSubComponent, IGridDataObjectConsumer
    {
        /// <summary>
        /// Initializes a new <see cref="GridTextDataObjectConsumer"/> object and associates it with a <see cref="GridControlBase"/>.
        /// </summary>
        /// <param name="grid">The <see cref="GridControlBase"/> this object should be associated with.</param>
        public GridTextDataObjectConsumer(GridControlBase grid)
            : base(grid)
        {
        }

        /// <summary>
        /// Gets the name of the DataObject consumer.
        /// </summary>
        public string Name
        {
            get { return "Text"; }
        }

        /// <summary>
        /// Queries the DataObject consumer if it knows how to handle the IDataObject.
        /// </summary>
        /// <param name="dataObject">Provides data to be consumed.</param>
        /// <param name="consumer">Another consumer that is capable of reading the data. Might be NULL.</param>
        /// <param name="options">Reserved for future use.</param>
        /// <returns>True if this consumer is able to read the data from <paramref name="dataObject"/>.</returns>
        public bool QueryAcceptData(IDataObject dataObject, IGridDataObjectConsumer consumer, GridQueryAcceptDataOptions options)
        {
            if (!GridUtil.IsSet(Grid.Model.Options.DragDropDropTargetFlags, GridDragDropFlags.Text))
            {
                return false;
            }

            if (!Grid.Model.IgnoreReadOnly && Grid.Model.ReadOnly)
            {
                return false;
            }

            // Let other consumers have higher precedence than this one.
            return consumer == null && (dataObject.GetDataPresent(DataFormats.Text) || dataObject.GetDataPresent(DataFormats.UnicodeText));
        }

        /// <summary>
        /// Queries the dimension in rows and columns of the data object.
        /// </summary>
        /// <param name="dataObject">Data object.</param>
        /// <returns>returns Dimension.</returns>
        public Size DetermineRowColCount(IDataObject dataObject)
        {
            int rowCount = 0;
            int colCount = 0;

            if (dataObject != null)
            {
                string buffer = null;
                if (dataObject.GetDataPresent(DataFormats.UnicodeText))
                {
                    buffer = dataObject.GetData(DataFormats.UnicodeText) as string;
                }
                else if (dataObject.GetDataPresent(DataFormats.Text))
                {
                    buffer = dataObject.GetData(DataFormats.Text) as string;
                }

                if (buffer != null)
                {
                    Grid.Model.TextDataExchange.CalcBufferDimension(buffer, out rowCount, out colCount);
                }
            }

            return new Size(colCount, rowCount);
        }

        /// <summary>
        /// Paste the contents of the data object at the specified cell coordinates.
        /// </summary>
        /// <param name="dataObject">Provides data to be consumed.</param>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <returns>True if successful; false otherwise.</returns>
        public bool DropAtRowCol(IDataObject dataObject, int rowIndex, int colIndex)
        {
            if (dataObject != null)
            {
                string buffer = null;
                if (dataObject.GetDataPresent(DataFormats.UnicodeText))
                {
                    buffer = dataObject.GetData(DataFormats.UnicodeText) as string;
                }
                else if (dataObject.GetDataPresent(DataFormats.Text))
                {
                    buffer = dataObject.GetData(DataFormats.Text) as string;
                }

                if (buffer != null)
                {
                    return Grid.Model.TextDataExchange.PasteTextFromBuffer(buffer, GridRangeInfo.Cell(rowIndex, colIndex), Grid.Model.Options.DragDropDropTargetFlags);
                }
            }

            return false;
        }
    }

    /// <summary>
    /// Implements the DropTarget part for OLE drag-and-drop operations. You can add support for additional data formats
    /// by creating a class that implements IGridDataObjectConsumer and register an instance of this with <see cref="GridControlBaseImp.RegisterDataObjectConsumer"/>.
    /// </summary>
    public class GridOleDropTarget : GridSubComponent, IGridOleDragDropEventsTarget
    {
        // Attributes - initialized from EnableOleDropTarget
        // private int dragDropDropTargetFlags;  

        // Attributes - Focus Rectangle, Cursor
        private int dragDropLastRow;
        private int dragDropLastCol;
        private Rectangle dragDropLastRect;
        private int dragDropRowExt;
        private int dragDropColExt;
        private DragDropEffects dragDropDropeffect;
        private ArrayList dataObjectConsumerOptions = new ArrayList();
        private IGridDataObjectConsumer activeConsumer = null;
        private GridControlBase grid;
        private bool mergeConflict = false;
        
        /// <summary>
        /// Initializes a new <see cref="GridOleDropTarget"/>.
        /// </summary>
        /// <param name="grid">The grid control.</param>
        public GridOleDropTarget(GridControlBase grid)
            : base(grid)
        {
            this.grid = grid;
            grid.OleDragDropEventsTarget = this;
            grid.LeftColChanging += new GridRowColIndexChangingEventHandler(GridBeforeScrolling);
            grid.TopRowChanging += new GridRowColIndexChangingEventHandler(GridBeforeScrolling);
        }

        /// <summary>
        /// Sets grid.Model.Options.DragDropDropTargetFlags 
        /// </summary>
        /// <param name="flags">Value for DragDropDropTargetFlags.</param>
        public void Register(int flags)
        {
            grid.Model.Options.DragDropDropTargetFlags = flags;
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (grid.OleDragDropEventsTarget == this)
                {
                    grid.OleDragDropEventsTarget = null;
                }

                grid.LeftColChanging -= new GridRowColIndexChangingEventHandler(GridBeforeScrolling);
                grid.TopRowChanging -= new GridRowColIndexChangingEventHandler(GridBeforeScrolling);
            }

            base.Dispose(disposing);
        }

        /// <exclude/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected virtual void GridBeforeScrolling(object sender, GridRowColIndexChangingEventArgs e)
        {
            OutlineDropTargetRange(Point.Empty, true, false);
        }

        /// <summary>
        /// Adds a IGridDataObjectConsumer.
        /// </summary>
        /// <param name="consumer">A <see cref="IGridDataObjectConsumer"/> object.</param>
        public void RegisterConsumer(IGridDataObjectConsumer consumer)
        {
            if (!dataObjectConsumerOptions.Contains(consumer))
            {
                this.dataObjectConsumerOptions.Add(consumer);
            }
        }

        /// <summary>
        /// Removes a IGridDataObjectConsumer.
        /// </summary>
        /// <param name="consumer">A <see cref="IGridDataObjectConsumer"/> object.</param>
        public void UnregisterConsumer(IGridDataObjectConsumer consumer)
        {
            if (dataObjectConsumerOptions.Contains(consumer))
            {
                this.dataObjectConsumerOptions.Remove(consumer);
            }
        }

        /// <implement/>
        /// <summary>Occurs when an object is dragged into the control's bounds.</summary>
        /// <param name="drgevent">A <see cref="DragEventArgs"/> holding the event data.</param>
        public virtual void OnDragEnter(DragEventArgs drgevent)
        {
            Trace.WriteLineIf(Switches.DragDrop.TraceVerbose, "Effect: " + drgevent.Effect.ToString() + " / AllowedEffect: " + drgevent.AllowedEffect.ToString());
            // Callback, check if grid understands the data.
            this.dragDropDropeffect = DragDropEffects.None;
            activeConsumer = QueryAcceptData(drgevent.Data);
            if (activeConsumer == null)
            {
                return;
            }

            // Check if another grid was the datasource.
            int rowExt, colExt;
            if (GridModel.InternalGridDragDropData.dndGridSource)
            {
                // Reuse information from datasource.
                rowExt = GridModel.InternalGridDragDropData.dndRowsCopied;
                colExt = GridModel.InternalGridDragDropData.dndColsCopied;
            }
            else            
            {
                // Calculate the size of the focus rectangle.
                Size size = activeConsumer.DetermineRowColCount(drgevent.Data);
                rowExt = size.Height;
                colExt = size.Width;
            }

            this.dragDropRowExt = rowExt;
            this.dragDropColExt = colExt;

            if (this.dragDropRowExt == 0 && this.dragDropColExt == 0)
            {
                // Empty data - return immediately.
                this.dragDropDropeffect = DragDropEffects.Scroll;
                drgevent.Effect = DragDropEffects.None;
                activeConsumer = null;
                return;
            }

            // Activate form.
            Form form = GridUtil.GetParentFrame(grid);
            if (form != null)
            {
                form.Activate();
                grid.Refresh();
            }

            // Initialize AutoScrolling for the grid. GridDragLeave will reset Autoscrolling.
            if (GridUtil.IsSet(grid.Model.Options.DragDropDropTargetFlags, GridDragDropFlags.EgdeScroll))
            {
                EnableEdgeScrolling(ScrollBars.Horizontal | ScrollBars.Vertical);
            }

            // GridDragOver will draw focus rectangle.
            OnDragOver(drgevent);
            this.dragDropDropeffect = DragDropEffects.None;
        }

        /// <exclude/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected void EnableEdgeScrolling(ScrollBars sb)
        {
            Rectangle r = grid.GridBounds;
            int dy = grid.ViewLayout.GetRowRangeHeight(0, grid.InternalGetFrozenRows(), GridCellSizeKind.VisibleSize);
            int dx = grid.ViewLayout.GetColRangeWidth(0, grid.InternalGetFrozenCols(), GridCellSizeKind.VisibleSize);
            grid.AutoScrollBounds = Rectangle.FromLTRB(dx, dy, r.Right, r.Bottom);
            grid.AutoScrolling = sb;
        }

        /// <summary>
        /// Queries the accept data.
        /// </summary>
        /// <param name="dataObject">The data object.</param>
        /// <returns>returns IGridDataObjectConsumer</returns>
        /// <exclude/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected virtual IGridDataObjectConsumer QueryAcceptData(IDataObject dataObject)
        {
            IGridDataObjectConsumer consumer = null;
            bool acceptsData;

            // Get a vote which consumer should get MouseMoveOver message.
            for (int n = 0; n < dataObjectConsumerOptions.Count; n++)
            {
                IGridDataObjectConsumer mc = dataObjectConsumerOptions[n] as IGridDataObjectConsumer;
                acceptsData = mc.QueryAcceptData(dataObject, consumer, null);
                if (acceptsData)
                {
                    consumer = mc;
                }
            }

            return consumer;
        }

        /// <implement/>
        /// <summary>
        /// Occurs when a drag-drop operation is completed.
        /// </summary>
        /// <param name="drgevent">A <see cref="DragEventArgs"/> holding the event data.</param>
        public virtual void OnDragDrop(DragEventArgs drgevent)
        {
            this.dragDropDropeffect = DragDropEffects.None;

            if ((this.dragDropRowExt == 0 && this.dragDropColExt == 0)
                || this.dragDropLastRow >= GridConstants.MaxRowCol)
            {
                // Empty data - user might have custom OLE drag-and-drop implementation - return immediately.
                // drgevent.Effect = DragDropEffects.None;
                return;
            }

            // Reset Autoscolling.
            if (GridUtil.IsSet(grid.Model.Options.DragDropDropTargetFlags, GridDragDropFlags.EgdeScroll))
            {
                grid.AutoScrolling = ScrollBars.None;
            }

            // Erase the focus rectangle.
            Point pt = new Point(drgevent.X, drgevent.Y);
            pt = grid.PointToClient(pt);
            this.OutlineDropTargetRange(pt, true, false);

// Something is wrong here, but at least we cleaned up everything.
            if (drgevent.Effect == DragDropEffects.None)            
            {
                return;
            }

            DragDropEffects dropEffect = drgevent.Effect;

            // Check if the source grid is also the target grid and
            // drop cell is the same as the start cell.
            GridRangeInfo targetCell = grid.PointToRangeInfo(pt);
            int rowIndex = targetCell.Top;
            int colIndex = targetCell.Left;

            // Adjust row / col if cursor is on a header cell
            // and pasting data onto a header is not allowed.
            // Also check if a whole row or column should be pasted.
            int nFirstRow = GridUtil.IsSet(grid.Model.Options.DragDropDropTargetFlags, GridDragDropFlags.RowHeader) ? 0 : grid.InternalGetHeaderRows() + 1;
            int nFirstCol = GridUtil.IsSet(grid.Model.Options.DragDropDropTargetFlags, GridDragDropFlags.ColHeader) ? 0 : grid.InternalGetHeaderCols() + 1;

            if (grid.Model.DragDropData.dndSource)
            {
                rowIndex = Math.Max(rowIndex, grid.Model.DragDropData.dndRowOffset) - grid.Model.DragDropData.dndRowOffset;
                colIndex = Math.Max(colIndex, grid.Model.DragDropData.dndColOffset) - grid.Model.DragDropData.dndColOffset;
                rowIndex = Math.Min(grid.Model.DragDropData.dndForceDropRow, rowIndex);
                colIndex = Math.Min(grid.Model.DragDropData.dndForceDropCol, colIndex);
            }

            rowIndex = Math.Max(rowIndex, nFirstRow);
            colIndex = Math.Max(colIndex, nFirstCol);

            if (GridUtil.IsSet(grid.Model.Options.DragDropDropTargetFlags, GridDragDropFlags.NoAppendRows))
            {
                int nLastRow = grid.Model.RowCount - this.dragDropRowExt + 1;
                rowIndex = Math.Min(rowIndex, nLastRow);
            }

            if (GridUtil.IsSet(grid.Model.Options.DragDropDropTargetFlags, GridDragDropFlags.NoAppendCols))
            {
                int nLastCol = grid.Model.ColCount - this.dragDropColExt + 1;
                colIndex = Math.Min(colIndex, nLastCol);
            }

            if (grid.Model.DragDropData.dndSource)
            {
                if (grid.Model.DragDropData.dndStartRow == rowIndex && grid.Model.DragDropData.dndStartCol == colIndex)
                {
                    drgevent.Effect = DragDropEffects.None;
                    return;
                }

                if (cellRange != GridRangeInfo.Empty)
                {
                    if (this.Grid.Model.CoveredRanges.Ranges.AnyRangeIntersects(cellRange))
                    {
                        GridRangeInfoList list = this.grid.Model.CoveredRanges.Ranges.GetRangesIntersecting(cellRange);
                        foreach (GridRangeInfo range in list)
                        {
                            mergeConflict = false;
                            if (range.Left >= cellRange.Left && range.Top >= cellRange.Top && range.Bottom <= cellRange.Bottom && range.Right <= cellRange.Right)
                            {
                                continue;
                            }
                            else
                            {
                                mergeConflict = true;
                                break;
                            }
                        }
                        if (mergeConflict)
                        {
                            dropEffect = DragDropEffects.None;
                            drgevent.Effect = dropEffect;
                            if (this.grid.ShowMessageBoxOnDrop)
                                MessageBoxAdv.Show(SR.GetString(SR.Cannotchangepartofamergedcell));
                            return;
                        }
                        else
                        {
                            if (this.grid.ShowMessageBoxOnDrop)
                            {
                                if (MessageBoxAdv.Show(SR.GetString(SR.DoYouWantToReplaceTheContentsOfTheDestinationCellsIfAny), SR.GetString(SR.Request), MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                                {
                                    dropEffect = DragDropEffects.None;
                                    drgevent.Effect = dropEffect;
                                    return;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (this.grid.ShowMessageBoxOnDrop)
                        {
                            if (MessageBoxAdv.Show(SR.GetString(SR.DoYouWantToReplaceTheContentsOfTheDestinationCellsIfAny), SR.GetString(SR.Request), MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                            {
                                dropEffect = DragDropEffects.None;
                                drgevent.Effect = dropEffect;
                                return;
                            }
                        }
                    }
                }

                try
                {
                    this.dragDropDropeffect = dropEffect;

                    if (grid.Model.DragDropData.directDragDrop && !grid.Model.DragDropData.dndCurrentCellText
                        && grid.Model.DragDropData.dndSelList.Count == 1)
                    {
                        // OnDndDropData will directly copy or move cells in sheet.
                        DropDataAtRowCol(drgevent.Data, rowIndex, colIndex);
                    }
                    else
                    {
                        grid.NotifySelectionFrameChanging(null);
                        grid.Model.BeginUpdate(BeginUpdateOptions.InvalidateAndScroll);

                        bool bSuccess = true;

                        try
                        {
                            if ((dropEffect & DragDropEffects.Copy) == DragDropEffects.Copy)
                            {
                                grid.Model.CommandStack.BeginTrans(SR.GetString("GRID_IDM_DRAGDROP_COPY"));
                            }
                            else
                            {
                                grid.Model.CommandStack.BeginTrans(SR.GetString("GRID_IDM_DRAGDROP_MOVE"));

                                // m_bDndCurrentCellText is set in OnDndCacheGlobalData when
                                // selected text from the current cell is copied to clipboard
                                if (grid.Model.DragDropData.dndCurrentCellText)
                                {
                                    GridCellRendererBase pCtrl = grid.Model.DragDropData.dndCurrentCellControl;
                                    ////                            if (pCtrl.IsEditing && !grid.Model.IsReadOnly && !pCtrl.IsReadOnly())
                                    ////                                pCtrl.ReplaceSel(String.Empty);
                                    grid.CurrentCell.EndEdit();
                                    bSuccess = true;
                                }
                                else
                                {
                                    // Clear styles when we copied styles.
                                    // If only text was copied, clear only the text from the cells.
                                    if (drgevent.Data.GetDataPresent(typeof(GridData))
                                        && GridUtil.IsSet(grid.Model.Options.DragDropDropTargetFlags, GridDragDropFlags.Styles))
                                    {
                                        bSuccess = grid.Model.ClearCells(grid.Model.DragDropData.dndSelList, true);
                                    }
                                    else
                                    {
                                        bSuccess = grid.Model.ClearCells(grid.Model.DragDropData.dndSelList, false);
                                    }
                                }

                                // Make sure calling function does not
                                // call ClearCells again.
                                dropEffect = DragDropEffects.Copy;
                            }

                            if (bSuccess)
                            {
                                bSuccess = DropDataAtRowCol(drgevent.Data, rowIndex, colIndex);
                            }

                            // Paste data.
                            if (bSuccess)
                            {
                                grid.Model.CommandStack.CommitTrans();
                            }
                            else
                            {
                                grid.Model.CommandStack.Rollback();
                            }
                        }
                        finally
                        {
                            grid.Model.EndUpdate();
                        }
                    }
                }
                finally
                {
                }

                this.dragDropDropeffect = DragDropEffects.None;

                // Let DoDragSource return DragDropEffects.None
                // so that grid does not clear cells again.
                dropEffect = DragDropEffects.None;
            }
            else
            {
                this.dragDropDropeffect = dropEffect;
                activeConsumer.DropAtRowCol(drgevent.Data, rowIndex, colIndex);
                this.dragDropDropeffect = DragDropEffects.None;
            }

            this.dragDropRowExt = 0;
            this.dragDropColExt = 0;

            drgevent.Effect = dropEffect;
            this.dragDropDropeffect = DragDropEffects.None;
        }

        /// <implement/>
        /// <summary>Occurs when an object is dragged out of the control's bounds.</summary>
        /// <param name="e">An EventArgs that holds the event data.</param>
        public virtual void OnDragLeave(EventArgs e)
        {
            // Reset Autoscrolling.
            if (GridUtil.IsSet(grid.Model.Options.DragDropDropTargetFlags, GridDragDropFlags.EgdeScroll))
            {
                grid.AutoScrolling = ScrollBars.None;
            }

            // Erase the focus rectangle.
            this.OutlineDropTargetRange(Point.Empty, true, false);

            this.dragDropDropeffect = DragDropEffects.None;
            this.dragDropRowExt = 0;
            this.dragDropColExt = 0;
            this.activeConsumer = null;
        }

        /// <implement/>
        /// <summary>Occurs when an object is dragged over the control's bounds.</summary>
        /// <param name="drgevent">A <see cref="DragEventArgs"/> holding event data.</param>
        public virtual void OnDragOver(DragEventArgs drgevent)
        {
#if DEBUG
            if (Switches.DragDrop.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo();
            }
#endif
            if (this.dragDropDropeffect != DragDropEffects.None)
            {
                drgevent.Effect = this.dragDropDropeffect;
                return;
            }

            // Outline the cells which will be replaced.
            Point pt = new Point(drgevent.X, drgevent.Y);
            pt = grid.PointToClient(pt);
            this.OutlineDropTargetRange(pt, true, true);

            if (this.dragDropLastRow == GridConstants.Undefined)
            {
                // No good - something went wrong in OutlineDropTargetRange.
                if (this.dragDropDropeffect != DragDropEffects.None)
                {
                    drgevent.Effect = DragDropEffects.None;
                }

                return;
            }

            // Check for force copy.
            if ((Control.ModifierKeys & Keys.Control) != Keys.None)
            {
                drgevent.Effect = DragDropEffects.Copy; // force copy data
            }
            else 
            {
                // move data is default
                drgevent.Effect = DragDropEffects.Move;
            }
        }

        /// <summary>
        /// Drops the data at row col.
        /// </summary>
        /// <param name="dataObject">The data object.</param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="colIndex">Index of the col.</param>
        /// <returns>returns boolean value</returns>
        /// <exclude/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected virtual bool DropDataAtRowCol(IDataObject dataObject, int rowIndex, int colIndex)
        {
            try
            {
                GridOleDropAtRowColEventArgs ge = new GridOleDropAtRowColEventArgs(dataObject, rowIndex, colIndex, false);
                grid.Model.RaiseOleDropAtRowCol(ge);
                if (ge.Handled)
                {
                    return ge.Result;
                }

                if (!grid.Model.IgnoreReadOnly && grid.Model.ReadOnly)
                {
                    return false;
                }

                grid.BeginUpdate(BeginUpdateOptions.Invalidate);

                try
                {
                    bool bSuccess = activeConsumer.DropAtRowCol(dataObject, rowIndex, colIndex);

                    if (bSuccess)
                    {
                        grid.Selections.Clear();
                        grid.CurrentCell.MoveTo(rowIndex, colIndex, GridSetCurrentCellOptions.ScrollInView | GridSetCurrentCellOptions.NoSelectRange);
                        if (this.dragDropRowExt > 1 || this.dragDropColExt > 1)
                        {
                            int nRowExt = Math.Min(rowIndex + this.dragDropRowExt - 1, grid.Model.RowCount);
                            int nColExt = Math.Min(colIndex + this.dragDropColExt - 1, grid.Model.ColCount);
                            GridRangeInfo range = GridRangeInfo.Cells(rowIndex, colIndex, nRowExt, nColExt);

                            if (grid.Model.DragDropData.dndSource)
                            {
                                // Check if full rows or columns have been dragged.
                                if (grid.Model.DragDropData.dndForceDropCol < GridConstants.MaxRowCol)
                                {
                                    if (grid.Model.DragDropData.dndForceDropRow < GridConstants.MaxRowCol)
                                    {
                                        range = GridRangeInfo.Table();
                                    }
                                    else
                                    {
                                        range = GridRangeInfo.Rows(rowIndex, nRowExt);
                                    }
                                }
                                else if (grid.Model.DragDropData.dndForceDropRow != GridConstants.MaxRowCol)
                                {
                                    range = GridRangeInfo.Cols(colIndex, nColExt);
                                }
                            }

                            grid.Model.Selections.Add(range);
                        }

                        grid.Model.RaiseOleDroppedData(ge);
                    }

                    return bSuccess;
                }
                catch (Exception ex)
                {
                    TraceUtil.TraceExceptionCatched(ex);
                    if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                    {
                        throw;
                    }

                    MessageBoxAdv.Show(FindFormHelper.FindForm(grid), ex.Message);
                    return false;
                }
            }
            finally
            {
                grid.EndUpdate(true);
            }
        }

        private GridRangeInfo cellRange = GridRangeInfo.Empty;

        /// <exclude/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected virtual void OutlineDropTargetRange(Point point, bool bEraseOld, bool bDrawNew)
        {
            if (grid == null || (this.dragDropRowExt == 0 && this.dragDropColExt == 0))
            {
                return; // invalid area
            }

            Rectangle oldDrawRect = this.dragDropLastRect;

            // If both parameters are False, simply invalidate the rect.
            if (!bEraseOld && !bDrawNew && !oldDrawRect.IsEmpty)
            {
                grid.InternalInvalidate(oldDrawRect);
                grid.Update();
                this.dragDropLastRect = oldDrawRect = Rectangle.Empty;
            }

            // If there is no old rectangle, we can't erase it.
            if (bEraseOld && !bDrawNew && oldDrawRect.IsEmpty)
            {
                return;
            }

            // Determine top-left row / col for rectangle.
            GridRangeInfo targetCell = grid.PointToRangeInfo(point, 1);
            int rowIndex = this.dragDropColExt > 0 ? targetCell.Top : 0;
            int colIndex = this.dragDropRowExt > 0 ? targetCell.Left : 0;

            if (rowIndex == GridConstants.Undefined || (rowIndex > grid.Model.RowCount && GridUtil.IsSet(grid.Model.Options.DragDropDropTargetFlags, GridDragDropFlags.NoAppendRows))
                || colIndex == GridConstants.Undefined || (colIndex > grid.Model.ColCount && GridUtil.IsSet(grid.Model.Options.DragDropDropTargetFlags, GridDragDropFlags.NoAppendCols)))
            {
                rowIndex = colIndex = GridConstants.Undefined;
                bDrawNew = false;
            }
            else
            {
                // Adjust row / col if cursor is on a header cell
                // and pasting data onto a header is not allowed.
                // Also check if a whole row or column should be pasted.
                int nFirstRow = GridUtil.IsSet(grid.Model.Options.DragDropDropTargetFlags, GridDragDropFlags.RowHeader) ? 0 : grid.InternalGetHeaderRows() + 1;
                int nFirstCol = GridUtil.IsSet(grid.Model.Options.DragDropDropTargetFlags, GridDragDropFlags.ColHeader) ? 0 : grid.InternalGetHeaderCols() + 1;

                if (grid.Model.DragDropData.dndSource)
                {
                    rowIndex = Math.Max(rowIndex, grid.Model.DragDropData.dndRowOffset) - grid.Model.DragDropData.dndRowOffset;
                    colIndex = Math.Max(colIndex, grid.Model.DragDropData.dndColOffset) - grid.Model.DragDropData.dndColOffset;
                    rowIndex = Math.Min(grid.Model.DragDropData.dndForceDropRow, rowIndex);
                    colIndex = Math.Min(grid.Model.DragDropData.dndForceDropCol, colIndex);
                }

                rowIndex = Math.Max(rowIndex, nFirstRow);
                colIndex = Math.Max(colIndex, nFirstCol);

                if (GridUtil.IsSet(grid.Model.Options.DragDropDropTargetFlags, GridDragDropFlags.NoAppendRows))
                {
                    int nLastRow = grid.Model.RowCount - this.dragDropRowExt + 1;
                    rowIndex = Math.Min(rowIndex, nLastRow);
                }

                if (GridUtil.IsSet(grid.Model.Options.DragDropDropTargetFlags, GridDragDropFlags.NoAppendCols))
                {
                    int nLastCol = grid.Model.ColCount - this.dragDropColExt + 1;
                    colIndex = Math.Min(colIndex, nLastCol);
                }
            }

            // If rectangle is already drawn at this row / col, there is no need to draw it again.
            if (bEraseOld && bDrawNew
                && rowIndex == this.dragDropLastRow && colIndex == this.dragDropLastCol
                && !oldDrawRect.IsEmpty)
            {
                return;
            }

            bEraseOld &= !oldDrawRect.IsEmpty;
            bDrawNew &= rowIndex != this.dragDropLastRow || colIndex != this.dragDropLastCol || oldDrawRect.IsEmpty;

            if (!bEraseOld && !bDrawNew)
            {
                return; // nothing to do
            }

            Graphics g = grid.CreateGridGraphics();
            g.IntersectClip(grid.GridBounds);

            if (bEraseOld)
            {
                if (!oldDrawRect.IsEmpty)
                {
                    GridUtil.DrawDragRectHelper(g, oldDrawRect, grid.GridBounds);
                }

                this.dragDropLastRect = Rectangle.Empty;
                this.dragDropLastRow = GridConstants.Undefined;
                this.dragDropLastCol = GridConstants.Undefined;
            }

            if (bDrawNew)
            {
                // Check here if the bounds have been set.
                int actualBottomRow = rowIndex + Math.Max(this.dragDropRowExt, 1) - 1;
                int actualRightCol = colIndex + Math.Max(this.dragDropColExt, 1) - 1;
                int nDndRowBounds = Math.Min(grid.Model.RowCount + 1, actualBottomRow);
                int nDndColBounds = Math.Min(grid.Model.ColCount + 1, actualRightCol);

                Rectangle rectDraw;
                cellRange = GridRangeInfo.Cells(rowIndex, colIndex, nDndRowBounds, nDndColBounds);
                int extraWidth = 0, extraHeight = 0;
                if (this.dragDropColExt == 0)
                {
                    GridRangeInfo rg = grid.ViewLayout.VisibleCellsRange;
                    rg = GridRangeInfo.Cells(0, rg.Left, rg.Bottom, rg.Right + 1);
                    cellRange = cellRange.IntersectRange(rg);
                    rectDraw = grid.RangeInfoToRectangle(cellRange);
                    rectDraw.Width = 0;
                }
                else if (this.dragDropRowExt == 0)
                {
                    GridRangeInfo rg = grid.ViewLayout.VisibleCellsRange;
                    rg = GridRangeInfo.Cells(rg.Top, 0, rg.Bottom + 1, rg.Right);
                    cellRange = cellRange.IntersectRange(rg);
                    rectDraw = grid.RangeInfoToRectangle(cellRange);
                    rectDraw.Height = 0;
                }
                else
                {
                    GridRangeInfo rg = grid.ViewLayout.VisibleCellsRange;
                    if (GridUtil.IsSet(grid.Model.Options.DragDropDropTargetFlags, GridDragDropFlags.OutlineAppendRows))
                    {
                        if (actualBottomRow > rg.Bottom)
                        {
                            extraHeight = (actualBottomRow - rg.Bottom) * grid.Model.Rows.DefaultSize;
                        }
                    }

                    if (GridUtil.IsSet(grid.Model.Options.DragDropDropTargetFlags, GridDragDropFlags.OutlineAppendCols))
                    {
                        if (actualRightCol > rg.Right)
                        {
                            extraWidth = (actualRightCol - rg.Right) * grid.Model.Cols.DefaultSize;
                        }
                    }

                    cellRange = cellRange.IntersectRange(rg);
                    rectDraw = grid.RangeInfoToRectangle(cellRange);
                    rectDraw.Width += extraWidth;
                    rectDraw.Height += extraHeight;

                    if (grid.Model.Properties.DisplayVertLines)
                    {
                        rectDraw.Width--;
                    }
                    else if (grid.Model.Properties.DisplayHorzLines)
                    {
                        rectDraw.Height--;
                    }
                }

                GridUtil.DrawDragRectHelper(g, rectDraw, grid.GridBounds);

                // Save state.
                this.dragDropLastRect = rectDraw;
                this.dragDropLastRow = rowIndex;
                this.dragDropLastCol = colIndex;
            }

            g.Dispose();
        }
    }
}

namespace Syncfusion.Windows.Forms.Grid.GridInternal
{
    /// <summary>
    /// This should implement internal drag-and-drop inside a grid. Not implemented yet, since grid itself does not provide
    /// CopyCells and MoveCells functionality yet.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class GridInternalDataObjectConsumer : GridSubComponent, IGridDataObjectConsumer
    {
        public GridInternalDataObjectConsumer(GridControlBase grid)
            : base(grid)
        {
        }

        public string Name
        {
            get { return "Internal"; }
        }

        public bool QueryAcceptData(IDataObject dataObject, IGridDataObjectConsumer consumer, GridQueryAcceptDataOptions options)
        {
            // Not yet implemented.
            return false;
        }

        public Size DetermineRowColCount(IDataObject dataObject)
        {
            int rowCount = 0;
            int colCount = 0;
            return new Size(colCount, rowCount);
        }

        public bool DropAtRowCol(IDataObject dataObject, int rowIndex, int colIndex)
        {
            return false;
            // TODO: directly copy / move cells within grid
            /*
            if (grid.Model.DragDropData.directDragDrop && !grid.Model.DragDropData.dndCurrentCellText
                && grid.Model.DragDropData.dndSelList.Count == 1)
            {
                GridRangeInfo range = grid.Model.DragDropData.dndSelList.ActiveRange;
                //                    if (this.dragDropDropeffect == DragDropEffects.Copy)
                //                        grid.CopyCells(range, rowIndex, colIndex);
                //                    else
                //                        grid.MoveCells(range, rowIndex, colIndex, null);

                bSuccess = true;
            }
            */
        }
    }
}
