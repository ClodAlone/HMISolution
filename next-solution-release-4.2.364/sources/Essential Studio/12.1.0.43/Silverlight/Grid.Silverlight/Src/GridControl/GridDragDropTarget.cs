#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;
using System.Collections;

#if!WinRT
using System.Windows.Controls;
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.Controls.Scroll;
using Syncfusion.Windows.Diagnostics;
using Syncfusion.Windows.GridCommon;
using Syncfusion.Windows.ComponentModel;

#if SILVERLIGHT
using ArrayList = System.Collections.Generic.List<object>;
using System.Collections.Generic;
#endif
namespace Syncfusion.Windows.Controls.Grid
{
#else
using Syncfusion.WinRT.Controls.Cells;
using Windows.Foundation;
using Syncfusion.WinRT.ComponentModel;
using System.Collections.Generic;
using Syncfusion.WinRT.Controls.Scroll;
namespace Syncfusion.WinRT.Controls.Grid
{
#endif
    /// <summary>
    /// Interface to create ObjectConsumer for IDataObject
    /// </summary>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
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
        bool QueryAcceptData(GridDataObject dataObject, IGridDataObjectConsumer consumer);

        /// <summary>
        /// Queries the dimension in rows and columns of the data object.
        /// </summary>
        /// <param name="dataObject">The data object.</param>
        /// <returns>returns size</returns>
        Size DetermineRowColCount(GridDataObject dataObject);

        /// <summary>
        /// Paste the contents of the data object at the specified cell coordinates.
        /// </summary>
        /// <param name="dataObject">Provides data to be consumed.</param>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <returns>True if successful; False otherwise.</returns>
        bool DropAtRowCol(GridDataObject dataObject, int rowIndex, int colIndex);
    }

    /// <summary>
    /// A base class for objects that are associated with a <see cref="GridControlBase"/>
    /// </summary>  
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridSubComponent : NonFinalizeDisposable
    {
        GridControlBase grid;

        /// <summary>
        /// Initializes a <see cref="GridSubComponent"/> and associates it with a grid.
        /// </summary>
        /// <param name="grid">The grid control this object is associated with.</param>
        protected GridSubComponent(GridControlBase grid)
        {
            this.grid = grid;
        }

        /// <summary>
        /// Gets the grid control this object is associated with.
        /// </summary>
        public GridControlBase Grid
        {
            get
            {
                return grid;
            }
        }
    }


    /// <summary>
    /// Implements a DataObject consumer for text data. Will handle data provided in DataFormats.Text and DataFormats.UnicodeText format.
    /// </summary>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
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
        public bool QueryAcceptData(GridDataObject dataObject, IGridDataObjectConsumer consumer)
        {
            if ((Grid.Model.Options.DragDropDropTargetFlags & GridDragDropFlags.Text) == 0)
            {
                return false;
            }

            //if (!Grid.Model.IgnoreReadOnly && Grid.Model.ReadOnly)
            //{
            //    return false;
            //}

            return consumer == null && (dataObject.GetDataPresent(typeof(string)));
        }

        /// <summary>
        /// Queries the dimension in rows and columns of the data object.
        /// </summary>
        /// <param name="dataObject">Data object.</param>
        /// <returns>returns Dimension.</returns>
        public Size DetermineRowColCount(GridDataObject dataObject)
        {
            int rowCount = 0;
            int colCount = 0;

            if (dataObject != null)
            {
                string buffer = null;
                if (dataObject.GetDataPresent(typeof(string)))
                {
                    buffer = dataObject.GetData(typeof(string)) as string;
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
        public bool DropAtRowCol(GridDataObject dataObject, int rowIndex, int colIndex)
        {
            if (dataObject != null)
            {
                string buffer = null;
                if (dataObject.GetDataPresent(typeof(string)))
                {
                    buffer = dataObject.GetData(typeof(string)) as string;
                }

                if (buffer != null)
                {
                    var rangeInfoList = new GridRangeInfoList();
                    rangeInfoList.Add(GridRangeInfo.Cell(rowIndex, colIndex));
                    return Grid.Model.TextDataExchange.PasteTextFromBuffer(buffer, rangeInfoList);//, Grid.Model.Options.DragDropDropTargetFlags);
                }
            }

            return false;
        }
    }

    /// <summary>
    /// Implements a DataObject consumer for serialized GridCellData. Checks if the provided data are of type <see cref="GridCellData"/>.
    /// </summary>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
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
            get { return "GridCellData"; }
        }

        /// <summary>
        /// Queries the DataObject consumer if it knows how to handle the IDataObject.
        /// </summary>
        /// <param name="dataObject">Provides data to be consumed.</param>
        /// <param name="consumer">Another consumer that is capable of reading the data. Might be NULL.</param>
        /// <param name="options">Reserved for future use.</param>
        /// <returns>True if this consumer is able to read the data from <paramref name="dataObject"/>.</returns>
        public bool QueryAcceptData(GridDataObject dataObject, IGridDataObjectConsumer consumer)
        {
            if ((grid.Model.Options.DragDropDropTargetFlags & GridDragDropFlags.Styles) == 0)
            {
                return false;
            }

            //if (!grid.Model.IgnoreReadOnly && grid.Model.ReadOnly)
            //{
            //    return false;
            //}

            // Let other consumers other than "Text" have higher priority than this one.
            if (consumer != null && consumer.Name != "Text")
            {
                return false;
            }

            return dataObject.GetDataPresent(typeof(GridCellData));
        }

        /// <summary>
        /// Queries the dimension in rows and columns of the data object.
        /// </summary>
        /// <param name="dataObject">The data object.</param>
        /// <returns>returns Dimension.</returns>
        public Size DetermineRowColCount(GridDataObject dataObject)
        {
            int rowCount = 0;
            int colCount = 0;

            if (dataObject.GetDataPresent(typeof(GridCellData)))
            {
                GridCellData data = (GridCellData)dataObject.GetData(typeof(GridCellData));
                rowCount = data.Rows.Count;
                colCount = data.Rows[0].Cells.Count;
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
        public bool DropAtRowCol(GridDataObject dataObject, int rowIndex, int colIndex)
        {
            if (dataObject.GetDataPresent(typeof(GridCellData)))
            {
                GridModel.InternalGridDragDropData.dndGridTargetStyle = true;
                GridCellData data = (GridCellData)dataObject.GetData(typeof(GridCellData));
                var rangeList = new GridRangeInfoList();
                rangeList.Add(GridRangeInfo.Cells(rowIndex, colIndex, rowIndex + data.Rows.Count, colIndex + data.Rows[0].Cells.Count));
                return grid.Model.CutPaste.PasteCellsFromStyle(data, rangeList);//, true, grid.Model.Options.DragDropDropTargetFlags);
            }

            return false;
        }
    }

    /// <summary>
    /// Implements the DropTarget part for OLE drag-and-drop operations. You can add support for additional data formats
    /// by creating a class that implements IGridDataObjectConsumer.
    /// </summary> 
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridOleDropTarget : GridSubComponent, IGridOleDragDropEventsTarget
    {
        // Attributes - initialized from EnableOleDropTarget
        // private int dragDropDropTargetFlags;  

        // Attributes - Focus Rectangle, Cursor
        private int dragDropLastRow;
        private int dragDropLastCol;
        private Rect dragDropLastRect;
        private int dragDropRowExt;
        private int dragDropColExt;

        private List<object> dataObjectConsumerOptions = new List<object>();
        private IGridDataObjectConsumer activeConsumer = null;
        private GridControlBase grid;

        /// <summary>
        /// Initializes a new <see cref="GridOleDropTarget"/>.
        /// </summary>
        /// <param name="grid">The grid control.</param>
        public GridOleDropTarget(GridControlBase grid)
            : base(grid)
        {
            this.grid = grid;
            grid.OleDragDropEventsTarget = this;
        }

        /// <summary>
        /// Sets grid.Model.Options.DragDropDropTargetFlags 
        /// </summary>
        /// <param name="flags">Value for DragDropDropTargetFlags.</param>
        public void Register(GridDragDropFlags flags)
        {
            grid.Model.Options.DragDropDropTargetFlags = flags;
        }

        /// <summary>
        /// Disposes objects on release
        /// </summary>
        /// <param name="disposing">Should Dispose</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (grid.OleDragDropEventsTarget == this)
                {
                    grid.OleDragDropEventsTarget = null;
                }
            }

            base.Dispose(disposing);
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
        public virtual void OnDragEnter(SyncfusionDragEventArgs drgevent)
        {

            //Trace.WriteLineIf(Switches.DragDrop.TraceVerbose, "Effect: " + drgevent.Effect.ToString() + " / AllowedEffect: " + drgevent.AllowedEffect.ToString());
            // Callback, check if grid understands the data.

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
                rowExt = (int)size.Height;
                colExt = (int)size.Width;
            }

            this.dragDropRowExt = rowExt;
            this.dragDropColExt = colExt;

            if (this.dragDropRowExt == 0 && this.dragDropColExt == 0)
            {
                // Empty data - return immediately.
                activeConsumer = null;
                return;
            }


            // Initialize AutoScrolling for the grid. GridDragLeave will reset Autoscrolling.
            if ((grid.Model.Options.DragDropDropTargetFlags & GridDragDropFlags.EdgeScroll) != 0)
            {
                EnableEdgeScrolling(AutoScrollOrientation.Horizontal | AutoScrollOrientation.Vertical);
            }

            // GridDragOver will draw focus rectangle.
            OnDragOver(drgevent);

        }

        /// <exclude/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected void EnableEdgeScrolling(AutoScrollOrientation sb)
        {
            var rect = this.Grid.GetClipRect(ScrollAxisRegion.Body, ScrollAxisRegion.Body);
            rect.Inflate(10, 10);
            this.Grid.AutoScroller.AutoScrollBounds = rect;
            this.Grid.AutoScroller.AutoScrolling = sb;
        }

        /// <summary>
        /// Queries the accept data.
        /// </summary>
        /// <param name="dataObject">The data object.</param>
        /// <returns>returns IGridDataObjectConsumer</returns>
        /// <exclude/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected virtual IGridDataObjectConsumer QueryAcceptData(GridDataObject dataObject)
        {
            IGridDataObjectConsumer consumer = null;
            bool acceptsData;

            // Get a vote which consumer should get MouseMoveOver message.
            for (int n = 0; n < dataObjectConsumerOptions.Count; n++)
            {
                IGridDataObjectConsumer mc = dataObjectConsumerOptions[n] as IGridDataObjectConsumer;
                acceptsData = mc.QueryAcceptData(dataObject, consumer);
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
        public virtual void OnDragDrop(SyncfusionDragEventArgs drgevent)
        {
            if ((this.dragDropRowExt == 0 && this.dragDropColExt == 0)
                || this.dragDropLastRow >= int.MaxValue - 1)
            {
                // Empty data - user might have custom OLE drag-and-drop implementation - return immediately.
                return;
            }

            // Reset Autoscolling.
            if ((grid.Model.Options.DragDropDropTargetFlags & GridDragDropFlags.EdgeScroll) != 0)
            {
                grid.AutoScroller.AutoScrolling = AutoScrollOrientation.None;
            }

            // Erase the focus rectangle.
#if !WinRT
            Point pt = drgevent.SourceEventArgs.GetPosition(grid);
#else
            Point pt = drgevent.SourceEventArgs.GetCurrentPoint(grid).Position;
#endif
            this.OutlineDropTargetRange(pt, true, false);

            // Check if the source grid is also the target grid and
            // drop cell is the same as the start cell.
#if !WinRT
            RowColumnIndex targetCell = grid.PointToCellRowColumnIndex(drgevent.SourceEventArgs.GetPosition(this.grid));
#else
            RowColumnIndex targetCell = grid.PointToCellRowColumnIndex(drgevent.SourceEventArgs.GetCurrentPoint(grid).Position);
#endif

            int rowIndex = targetCell.RowIndex;
            int colIndex = targetCell.ColumnIndex;

            // Adjust row / col if cursor is on a header cell
            // and pasting data onto a header is not allowed.
            // Also check if a whole row or column should be pasted.
            int nFirstRow = ((grid.Model.Options.DragDropDropTargetFlags & GridDragDropFlags.RowHeader) == 0) ? 0 : grid.InternalGetHeaderRows();
            int nFirstCol = ((grid.Model.Options.DragDropDropTargetFlags & GridDragDropFlags.ColHeader) == 0) ? 0 : grid.InternalGetHeaderCols();

            if (grid.Model.DragDropData.dndSource)
            {
                rowIndex = Math.Max(rowIndex, grid.Model.DragDropData.dndRowOffset) - grid.Model.DragDropData.dndRowOffset;
                colIndex = Math.Max(colIndex, grid.Model.DragDropData.dndColOffset) - grid.Model.DragDropData.dndColOffset;
                rowIndex = Math.Min(grid.Model.DragDropData.dndForceDropRow, rowIndex);
                colIndex = Math.Min(grid.Model.DragDropData.dndForceDropCol, colIndex);
            }

            rowIndex = Math.Max(rowIndex, nFirstRow);
            colIndex = Math.Max(colIndex, nFirstCol);

            if ((grid.Model.Options.DragDropDropTargetFlags & GridDragDropFlags.NoAppendRows) == 0)
            {
                int nLastRow = grid.Model.RowCount - this.dragDropRowExt + 1;
                rowIndex = Math.Min(rowIndex, nLastRow);
            }

            if ((grid.Model.Options.DragDropDropTargetFlags & GridDragDropFlags.NoAppendCols) == 0)
            {
                int nLastCol = grid.Model.ColumnCount - this.dragDropColExt + 1;
                colIndex = Math.Min(colIndex, nLastCol);
            }

            if (grid.Model.DragDropData.dndSource)
            {
                if (grid.Model.DragDropData.dndStartRow == rowIndex && grid.Model.DragDropData.dndStartCol == colIndex)
                {
                    return;
                }

                try
                {
                    if (grid.Model.DragDropData.directDragDrop && !grid.Model.DragDropData.dndCurrentCellText
                        && grid.Model.DragDropData.dndSelList.Count == 1)
                    {
                        // OnDndDropData will directly copy or move cells in sheet.
                        DropDataAtRowCol(drgevent.Data, rowIndex, colIndex);
                    }
                    else
                    {

                        bool bSuccess = true;

                        try
                        {

                            // DndCurrentCellText is set in OnDndCacheGlobalData when
                            // selected text from the current cell is copied to clipboard
                            if (grid.Model.DragDropData.dndCurrentCellText)
                            {
                                IGridCellRenderer pCtrl = grid.Model.DragDropData.dndCurrentCellControl;
                                ////                            if (pCtrl.IsEditing && !grid.Model.IsReadOnly && !pCtrl.IsReadOnly())
                                ////                                pCtrl.ReplaceSel(String.Empty);
                                grid.CurrentCell.EndEdit();
                                bSuccess = true;
                            }
                            else
                            {
                                //Clear styles when we copied styles.
                                //If only text was copied, clear only the text from the cells.
                                if (drgevent.Data.GetDataPresent(typeof(GridCellData))
                                && (grid.Model.Options.DataObjectConsumerOptions & GridDataObjectConsumerOptions.Styles) != 0)
                                {
                                    bSuccess = grid.Model.ClearCells(grid.Model.DragDropData.dndSelList, true);
                                }
                                else
                                {
                                    bSuccess = grid.Model.ClearCells(grid.Model.DragDropData.dndSelList, false);
                                }
#if !WPF
                                grid.Model.InvalidateCell(grid.Model.DragDropData.dndSelList.ActiveRange);
                                grid.Model.InvalidateVisual(true);
#endif
                            }

                            // Make sure calling function does not
                            // call ClearCells again.
                            if (bSuccess)
                            {
                                bSuccess = DropDataAtRowCol(drgevent.Data, rowIndex, colIndex);
                            }
                        }
                        finally
                        { }
                    }
                }
                finally
                { }
            }
            else
            {


                activeConsumer.DropAtRowCol(drgevent.Data, rowIndex, colIndex);
            }

            this.dragDropRowExt = 0;
            this.dragDropColExt = 0;

            GridModel.InternalGridDragDropData.dndGridSource = false;
            GridModel.InternalGridDragDropData.dndGridTargetStyle = false;

            // Reset settings in parameter object.
            grid.Model.DragDropData.dndSource = false;
            grid.Model.DragDropData.dndStartRow = 0;
            grid.Model.DragDropData.dndStartCol = 0;
            grid.Model.DragDropData.dndSelList = null;
            grid.Model.DragDropData.dndCurrentCellText = false;
            grid.Model.DragDropData.dndCurrentCellControl = null;
        }

        /// <implement/>
        /// <summary>Occurs when an object is dragged out of the control's bounds.</summary>
        /// <param name="e">An EventArgs that holds the event data.</param>
        public virtual void OnDragLeave(EventArgs e)
        {
            ////Reset Autoscrolling.
            if ((grid.Model.Options.DragDropDropTargetFlags & GridDragDropFlags.EdgeScroll) != 0)
            {
                grid.AutoScroller.AutoScrolling = AutoScrollOrientation.None;
            }

            // Erase the focus rectangle.
            this.OutlineDropTargetRange(new Point(), true, false);
            this.dragDropRowExt = 0;
            this.dragDropColExt = 0;
            this.activeConsumer = null;
        }

        /// <implement/>
        /// <summary>Occurs when an object is dragged over the control's bounds.</summary>
        /// <param name="drgevent">A <see cref="DragEventArgs"/> holding event data.</param>
        public virtual void OnDragOver(SyncfusionDragEventArgs drgevent)
        {
            // Outline the cells which will be replaced.
#if !WinRT
            Point pt = new Point(drgevent.SourceEventArgs.GetPosition(this.grid).X, drgevent.SourceEventArgs.GetPosition(this.grid).Y);
#else
            Point pt = drgevent.SourceEventArgs.GetCurrentPoint(grid).Position;
#endif
            this.OutlineDropTargetRange(pt, true, true);

            if (this.dragDropLastRow == int.MaxValue)
            {
                return;
            }

            // Check for force copy.
            //if ((ModifierKeys.Control & Key.RightCtrl) != Key.None)
            //{
            //    drgevent.Effects = DragDropEffects.Copy; // force copy data
            //}
            //else
            //{
            //    // move data is default
            //    drgevent.Effects = DragDropEffects.Move;
            //}
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
        protected virtual bool DropDataAtRowCol(GridDataObject dataObject, int rowIndex, int colIndex)
        {
            try
            {
                GridOleDropAtRowColEventArgs ge = new GridOleDropAtRowColEventArgs(dataObject, rowIndex, colIndex, false);
                grid.Model.RaiseOleDropAtRowCol(ge);
                if (ge.Handled)
                {
                    return ge.Result;
                }

                //if (!grid.Model.IgnoreReadOnly && grid.Model.ReadOnly)
                //{
                //    return false;
                //}

                try
                {
                    bool bSuccess = activeConsumer.DropAtRowCol(dataObject, rowIndex, colIndex);

                    if (bSuccess)
                    {

                        grid.CurrentCell.MoveTo(rowIndex, colIndex);//, GridSetCurrentCellOptions.ScrollInView | GridSetCurrentCellOptions.NoSelectRange);
                        if (this.dragDropRowExt > 1 || this.dragDropColExt > 1)
                        {
                            int nRowExt = Math.Min(rowIndex + this.dragDropRowExt - 1, grid.Model.RowCount);
                            int nColExt = Math.Min(colIndex + this.dragDropColExt - 1, grid.Model.ColumnCount);
                            GridRangeInfo range = GridRangeInfo.Cells(rowIndex, colIndex, nRowExt, nColExt);

                            if (grid.Model.DragDropData.dndSource)
                            {
                                // Check if full rows or columns have been dragged.
                                if (grid.Model.DragDropData.dndForceDropCol < int.MaxValue - 1)
                                {
                                    if (grid.Model.DragDropData.dndForceDropRow < int.MaxValue - 1)
                                    {
                                        range = GridRangeInfo.Table();
                                    }
                                    else
                                    {
                                        range = GridRangeInfo.Rows(rowIndex, nRowExt);
                                    }
                                }
                                else if (grid.Model.DragDropData.dndForceDropRow != int.MaxValue - 1)
                                {
                                    range = GridRangeInfo.Cols(colIndex, nColExt);
                                }
                            }
                            grid.Model.Selections.Clear();
                            grid.Model.Selections.Add(range);
                        }

                        grid.Model.RaiseOleDroppedData(ge);
                    }

                    return bSuccess;
                }
                catch (Exception ex)
                {
#if !WinRT
                    MessageBox.Show(ex.Message);
#endif
                    return false;
                }
            }
            finally
            {
            }
        }

        /// <exclude/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected virtual void OutlineDropTargetRange(Point point, bool bEraseOld, bool bDrawNew)
        {
            if (grid == null || (this.dragDropRowExt == 0 && this.dragDropColExt == 0))
            {
                return; // invalid area
            }

            Rect oldDrawRect = this.dragDropLastRect;

            // If both parameters are False, simply invalidate the rect.
            if (!bEraseOld && !bDrawNew && !oldDrawRect.IsEmpty)
            {
                this.dragDropLastRect = oldDrawRect = Rect.Empty;
            }

            // If there is no old rectangle, we can't erase it.
            if (bEraseOld && !bDrawNew && oldDrawRect.IsEmpty)
            {
                return;
            }

            // Determine top-left row / col for rectangle.
            RowColumnIndex targetCell = grid.PointToCellRowColumnIndex(point);//, 1);
            int rowIndex = this.dragDropColExt > 0 ? targetCell.RowIndex : 0;
            int colIndex = this.dragDropRowExt > 0 ? targetCell.ColumnIndex : 0;

            if (rowIndex == int.MaxValue || (rowIndex > grid.Model.RowCount && ((grid.Model.Options.DragDropDropTargetFlags & GridDragDropFlags.NoAppendRows) == 0)
                || colIndex == int.MaxValue || (colIndex > grid.Model.ColumnCount && ((grid.Model.Options.DragDropDropTargetFlags & GridDragDropFlags.NoAppendCols) == 0))))
            {
                rowIndex = colIndex = int.MaxValue;
                bDrawNew = false;
            }
            else
            {
                // Adjust row / col if cursor is on a header cell
                // and pasting data onto a header is not allowed.
                // Also check if a whole row or column should be pasted.
                int nFirstRow = ((grid.Model.Options.DragDropDropTargetFlags & GridDragDropFlags.RowHeader) == 0) ? 0 : grid.InternalGetHeaderRows();
                int nFirstCol = ((grid.Model.Options.DragDropDropTargetFlags & GridDragDropFlags.ColHeader) == 0) ? 0 : grid.InternalGetHeaderCols();

                if (grid.Model.DragDropData.dndSource)
                {
                    rowIndex = Math.Max(rowIndex, grid.Model.DragDropData.dndRowOffset) - grid.Model.DragDropData.dndRowOffset;
                    colIndex = Math.Max(colIndex, grid.Model.DragDropData.dndColOffset) - grid.Model.DragDropData.dndColOffset;
                    rowIndex = Math.Min(grid.Model.DragDropData.dndForceDropRow, rowIndex);
                    colIndex = Math.Min(grid.Model.DragDropData.dndForceDropCol, colIndex);
                }

                rowIndex = Math.Max(rowIndex, nFirstRow);
                colIndex = Math.Max(colIndex, nFirstCol);

                if ((grid.Model.Options.DragDropDropTargetFlags & GridDragDropFlags.NoAppendRows) == 0)
                {
                    int nLastRow = grid.Model.RowCount - this.dragDropRowExt + 1;
                    rowIndex = Math.Min(rowIndex, nLastRow);
                }

                if ((grid.Model.Options.DragDropDropTargetFlags & GridDragDropFlags.NoAppendCols) == 0)
                {
                    int nLastCol = grid.Model.ColumnCount - this.dragDropColExt + 1;
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

            if (bEraseOld)
            {
                if (!oldDrawRect.IsEmpty)
                {
                    this.grid.RenderExcelLikeBorder(oldDrawRect);
                }

                this.dragDropLastRect = Rect.Empty;
                this.dragDropLastRow = int.MaxValue - 1;
                this.dragDropLastCol = int.MaxValue - 1;
            }

            if (bDrawNew)
            {
                // Check here if the bounds have been set.
                int actualBottomRow = rowIndex + Math.Max(this.dragDropRowExt, 1) - 1;
                int actualRightCol = colIndex + Math.Max(this.dragDropColExt, 1) - 1;
                int nDndRowBounds = Math.Min(grid.Model.RowCount + 1, actualBottomRow);
                int nDndColBounds = Math.Min(grid.Model.ColumnCount + 1, actualRightCol);

                Rect rectDraw;
                GridRangeInfo cellRange = GridRangeInfo.Cells(rowIndex, colIndex, nDndRowBounds, nDndColBounds);
                //int extraWidth = 0, extraHeight = 0;
                //if (this.dragDropColExt == 0)
                //{
                //    GridRangeInfo rg = grid.ViewLayout.VisibleCellsRange;
                //    rg = GridRangeInfo.Cells(0, rg.Left, rg.Bottom, rg.Right + 1);
                //    cellRange = cellRange.IntersectRange(rg);
                //    rectDraw = this.grid.RenderExcelRangeBorder(cellRange);
                //    rectDraw.Width = 0;
                //}
                //else if (this.dragDropRowExt == 0)
                //{
                //    GridRangeInfo rg = grid.ViewLayout.VisibleCellsRange;
                //    rg = GridRangeInfo.Cells(rg.Top, 0, rg.Bottom + 1, rg.Right);
                //    cellRange = cellRange.IntersectRange(rg);
                //    rectDraw = grid.RangeInfoToRectangle(cellRange);
                //    rectDraw.Height = 0;
                //}


                rectDraw = this.grid.RenderExcelRangeBorder(cellRange);
                this.grid.RenderExcelLikeBorder(rectDraw);
                // Save state.
                this.dragDropLastRect = rectDraw;
                this.dragDropLastRow = rowIndex;
                this.dragDropLastCol = colIndex;
            }
        }
    }
}
