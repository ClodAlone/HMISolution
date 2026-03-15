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

#if !WinRT
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.Controls.Scroll;
using Syncfusion.Windows.Diagnostics;
using Syncfusion.Windows.GridCommon;
using Syncfusion.Windows.ComponentModel;
using System.Windows.Controls;
namespace Syncfusion.Windows.Controls.Grid
{
#else
namespace Syncfusion.WinRT.Controls.Cells
{
    using Syncfusion.WinRT.ComponentModel;
    using Windows.Devices.Input;
    using Syncfusion.WinRT.Controls.Scroll;
    using Syncfusion.WinRT.Controls.Grid;
    using Windows.Foundation;
    using Windows.UI.Xaml.Input;

#endif
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridDataObject
    {
        // Fields
        private object _data;

        // Methods
        public GridDataObject()
        {
        }

        public object GetData(string format)
        {
            return this._data;
        }

        public object GetData(Type format)
        {
            return this._data;
        }

        public bool GetDataPresent(string format)
        {
            return (this._data != null);
        }

        public bool GetDataPresent(Type format)
        {
            return (this._data != null);
        }

        public void SetData(object data)
        {
            this._data = data;
        }
    }

#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class SyncfusionDragEventArgs : SyncfusionEventArgs
    {
        private GridDataObject data;
        public GridDataObject Data
        {
            get
            {
                return this.data;
            }
            set
            {
                this.data = value;
            }
        }
#if !WinRT
        private MouseEventArgs sourceEventArgs;
        public MouseEventArgs SourceEventArgs
        {
            get
            {
                return this.sourceEventArgs;
            }
            set
            {
                this.sourceEventArgs = value;
            }
        }
#else
        private PointerRoutedEventArgs sourceEventArgs;
        public PointerRoutedEventArgs SourceEventArgs
        {
            get
            {
                return this.sourceEventArgs;
            }
            set
            {
                this.sourceEventArgs = value;
            }
        }
#endif
        private bool handled;
        public bool Handled
        {
            get
            {
                return this.handled;
            }
            set
            {
                this.handled = value;
            }
        }
#if !WinRT
        public SyncfusionDragEventArgs(MouseEventArgs args, GridDataObject dataObject, bool handled)
        {
            this.sourceEventArgs = args;
            this.data = dataObject;
            this.handled = handled;
        }
#else
        public SyncfusionDragEventArgs(PointerRoutedEventArgs args, GridDataObject dataObject, bool handled)
        {
            this.sourceEventArgs = args;
            this.data = dataObject;
            this.handled = handled;
        }
#endif

        public SyncfusionDragEventArgs()
        {

        }
    }

    /// <summary>
    /// Implements the datasource part of an OLE drag-and-drop operation in a grid control.
    /// </summary>    
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridExcelLikeDragDropMouseController : IMouseController
    {
        private GridControlBase grid;
        private GridDragDropFlags dragDropFlags;
        ExcelLikeDragDropHitTestInfo hitTestInfo = null;
        private GridDataObject dataObject { get; set; }
        public GridExcelLikeDragDropMouseController(GridControlBase grid)
        {
            this.grid = grid;
            this.dataObject = new GridDataObject();
        }

        public bool EnableExcelLikeDragDrop(GridDragDropFlags flags)
        {
            this.dragDropFlags = flags;
            return true;
        }

        #region IMouseController Members

        public string Name
        {
            get
            {
                return "ExcelLikeDragDrop";
            }
        }
#if !WinRT
        public Cursor Cursor
        {
            get
            {
                if (hitTestInfo != null && hitTestInfo.hitTestResult && grid.Model.Options.ExcelLikeSelectionFrame)
                {
                    return Cursors.Hand;
                }

                return null;
            }
        }
#endif
        public void MouseHoverEnter(MouseEventArgs e)
        {

        }

        public void MouseHover(MouseControllerEventArgs e)
        {

        }

        public void MouseHoverLeave(MouseEventArgs e)
        {

        }

        public void MouseDown(MouseControllerEventArgs e)
        {
            if (hitTestInfo != null
#if !WinRT
                && e.Button == System.Windows.Browser.MouseButtons.Left
#endif
                )
            {
                GridRangeInfo rgCovered;
                grid.Model.CoveredRanges.Find(hitTestInfo.rowIndex, hitTestInfo.colIndex, out rgCovered);
                if (e.ClickCount == 1 && grid.Model.Options.ExcelLikeSelectionFrame)
                {
                    this.StartDragDrop(rgCovered.Top, rgCovered.Left, e.SourceEventArgs);
                }
            }
        }

        public void MouseMove(MouseControllerEventArgs e)
        {
            this.grid.OleDragDropEventsTarget.OnDragOver(new SyncfusionDragEventArgs() { SourceEventArgs = e.SourceEventArgs });
        }

        public void MouseUp(MouseControllerEventArgs e)
        {
            this.grid.OleDragDropEventsTarget.OnDragDrop(new SyncfusionDragEventArgs() { SourceEventArgs = e.SourceEventArgs, Data = dataObject });
        }

        public void CancelMode()
        {

        }

        public void RestoreMode()
        {

        }

        public int HitTest(MouseControllerEventArgs e, IMouseController controller)
        {

            Point pt = new Point(e.Location.X, e.Location.Y);
            // This HitTest code has higher priority than "SelectCells"
            hitTestInfo = null;
            if (
#if !WinRT
                (e.Button == System.Windows.Browser.MouseButtons.Left || e.ClickCount == 1) && 
#endif
                this.grid.Model.Options.ExcelLikeSelectionFrame)
            {
                if (controller == null
                    || controller.Name == "SelectCellsMouseController")
                //|| this.grid.MouseControllerDispatcher.LastHitTestCode != GridHitTestContext.CellButtonElement)
                {
                    hitTestInfo = new ExcelLikeDragDropHitTestInfo(grid, pt);
                    if (!hitTestInfo.hitTestResult)
                    {
                        hitTestInfo = null;
                    }
                }
            }

            if (hitTestInfo != null && hitTestInfo.hitTestResult)
            {
                return 1;
            }

            return 0;
        }

        public bool SupportsCancelMouseCapture
        {
            get
            {
                return false;
            }
        }

        public bool SupportsMouseTracking
        {
            get
            {
                return false;
            }
        }

        #endregion


        internal sealed class ExcelLikeDragDropHitTestInfo
        {
            const int hitTestFrame = 8;
            internal bool hitTestResult = false;
            internal Point point;
            internal Rect cellBounds = Rect.Empty;
            internal int clientCol;
            internal int clientRow;
            internal int rowIndex;
            internal int colIndex;
            internal GridRangeInfoList pSelList;
            internal GridRangeInfo activeRange;
            internal bool bTableSel;

            internal ExcelLikeDragDropHitTestInfo(GridControlBase grid, Point point)
            {
                this.point = point;
                var currentRowColumnIndex = grid.PointToCellRowColumnIndex(point);
                clientCol = grid.GetClientCol(currentRowColumnIndex.ColumnIndex);//.ViewLayout.PointToClientCol(point, false, GridCellSizeKind.VisibleSize);
                clientRow = grid.GetClientRow(currentRowColumnIndex.RowIndex);//.ViewLayout.PointToClientRow(point, false, GridCellSizeKind.VisibleSize);
                if (clientCol >= 0 && clientRow >= 0)
                {
                    rowIndex = grid.GetRow(clientRow);
                    colIndex = grid.GetCol(clientCol);

                    if (//grid.HitTestSelectionEdge &&
                        //clientCol < grid.ViewLayout.VisibleCols && clientRow < grid.ViewLayout.VisibleRows &&
                        rowIndex <= grid.Model.RowCount && colIndex <= grid.Model.ColumnCount)
                    {
                        ////cellBounds = new Rectangle(grid.ViewLayout.ClientRowColToPoint(clientRow, clientCol), 
                        ////new Size(grid.GetColWidth(colIndex), grid.GetRowHeight(rowIndex)));
                        cellBounds = grid.RangeToClippedVisibleRect(GridRangeInfo.Cell(rowIndex, colIndex));//, GridCellSizeKind.VisibleSize);

                        int nEdge = 4;//HitTestSelectionEdge

                        bool bSelEdge = false;
                        pSelList = grid.Model.Selections.Ranges;
                        activeRange = GridRangeInfo.Empty;

                        //check if whole table is selected
                        bTableSel = pSelList.AnyRangeContains(GridRangeInfo.Table());

                        if (bTableSel)
                        {
                            bSelEdge = rowIndex == 0 && colIndex == 0
                                // check left border
                                && ((point.X >= cellBounds.Left && point.X - cellBounds.Left <= nEdge)
                                // check top border
                                || (point.Y >= cellBounds.Top && point.Y - cellBounds.Top <= nEdge));
                        }
                        else
                        {
                            activeRange = pSelList.ActiveRange;
                            if (activeRange.IsEmpty)
                            {
                                GridRangeInfo rg;
                                grid.Model.CoveredRanges.Find(rowIndex, colIndex, out rg);

                                if (rg != null && grid.CurrentCell.HasCurrentCellAt(rg.Top, rg.Left))
                                {
                                    activeRange = rg;
                                }
                            }

                            GridRangeInfo rgCell = GridRangeInfo.Cell(rowIndex, colIndex);

                            if (activeRange.Contains(rgCell))
                            {
                                // Check left border.
                                if (!bSelEdge && point.X >= cellBounds.Left && point.X - cellBounds.Left <= nEdge)
                                {
                                    bSelEdge = !activeRange.IsRows && activeRange.Left == colIndex;
                                }

                                // Check right border.
                                if (!bSelEdge && point.X <= cellBounds.Right && cellBounds.Right - point.X <= nEdge)
                                {
                                    bSelEdge = !activeRange.IsRows && activeRange.Right == colIndex;
                                }

                                // Check top border.
                                if (!bSelEdge && point.Y >= cellBounds.Top && point.Y - cellBounds.Top <= nEdge)
                                {
                                    bSelEdge = !activeRange.IsCols && activeRange.Top == rowIndex;
                                }

                                // Check bottom border.
                                if (!bSelEdge && point.Y <= cellBounds.Bottom && cellBounds.Bottom - point.Y <= nEdge)
                                {
                                    bSelEdge = !activeRange.IsCols && activeRange.Bottom == rowIndex;
                                }
                            }
                        }

                        if (bSelEdge && RaiseQueryCanOleDragRange(grid, activeRange))
                        {
                            hitTestResult = true;
                        }
                        else
                        {
                            hitTestResult = false;
                        }
                    }
                }
            }

            bool RaiseQueryCanOleDragRange(GridControlBase grid, GridRangeInfo range)
            {
                GridQueryCanDragRangeEventArgs e = new GridQueryCanDragRangeEventArgs(range);
                grid.RaiseQueryCanOleDragRange(e);
                return !e.Cancel;
            }
        }

#if !WinRT
        private bool StartDragDrop(int rowIndex, int colIndex, MouseEventArgs args)
        {
#else
        private bool StartDragDrop(int rowIndex, int colIndex, PointerRoutedEventArgs args)
        {
#endif
            // If there are no cells selected,
            // copy the current cell's coordinates.
            GridRangeInfoList selList;

            // If user did click into a selected cell (or current cell), start OLE drag-and-drop.
            if (grid.Model.Selections.GetSelectedRanges(out selList, true) && selList.AnyRangeContains(GridRangeInfo.Cell(rowIndex, colIndex)))
            {
                bool bMulti = (this.dragDropFlags & GridDragDropFlags.Multiple) != 0;

                if (!bMulti)
                {
                    // Build up a single-range list.
                    GridRangeInfo rg = selList.GetRangesContaining(GridRangeInfo.Cell(rowIndex, colIndex)).ActiveRange;
                    selList = new GridRangeInfoList();
                    selList.Add(rg);
                }

                grid.Model.DragDropData.dndForceDropCol = int.MaxValue - 1;
                grid.Model.DragDropData.dndForceDropRow = int.MaxValue - 1;

                // Check if we should exclude headers.
                int nFirstRow = 0;
                int nFirstCol = 0;

                if ((this.dragDropFlags & GridDragDropFlags.RowHeader) != 0)
                {
                    nFirstCol = grid.InternalGetHeaderCols() + 1;
                }

                if ((this.dragDropFlags & GridDragDropFlags.ColHeader) != 0)
                {
                    nFirstRow = grid.InternalGetHeaderRows() + 1;
                }

                // Loop through all selected ranges and expand them.
                // Row and column headers will be excluded from expanded range.
                foreach (GridRangeInfo rangeItem in selList)
                {
                    // If a whole row or column is dragged, allow
                    // it only to paste it as a whole row or column
                    // if pasted into the same grid.
                    if (rangeItem.IsTable)
                    {
                        grid.Model.DragDropData.dndForceDropCol = nFirstCol;
                        grid.Model.DragDropData.dndForceDropRow = nFirstRow;
                    }
                    else if (rangeItem.IsRows)
                    {
                        grid.Model.DragDropData.dndForceDropCol = nFirstCol;
                    }
                    else if (rangeItem.IsCols)
                    {
                        grid.Model.DragDropData.dndForceDropRow = nFirstRow;
                    }
                }

                selList = selList.ExpandRanges(
                    nFirstRow,
                    nFirstCol,
                    grid.Model.RowCount,
                    grid.Model.ColumnCount);

                grid.Model.DragDropData.dndCurrentCellText = false;
                grid.Model.DragDropData.dndCurrentCellControl = null;


                int dndRowsCopied;
                int dndColsCopied;

                // Raises OnQueryOleDataSourceData event.
                this.OnCacheGlobalData(dataObject, selList, out dndRowsCopied, out dndColsCopied);

                GridModel.InternalGridDragDropData.dndRowsCopied = dndRowsCopied;
                GridModel.InternalGridDragDropData.dndColsCopied = dndColsCopied;

                //// Store settings in parameter object so that when the
                //// user drops data into a different grid window
                //// which is bound to the same parameter object can use
                //// this information.

                grid.Model.DragDropData.dndSource = true;
                grid.Model.DragDropData.dndSelList = selList;
                grid.Model.DragDropData.dndStartRow = Math.Min(grid.Model.DragDropData.dndForceDropRow, Math.Max(rowIndex, nFirstRow));
                grid.Model.DragDropData.dndStartCol = Math.Min(grid.Model.DragDropData.dndForceDropCol, Math.Max(colIndex, nFirstCol));
                GridRangeInfo pRange = selList.ActiveRange;
                if (selList.Count == 1 && !pRange.IsEmpty)
                {
                    // Adjust start row / col to avoid that when
                    // the programmer selects a range of cells and
                    // drags the cells by selecting the lower right corner
                    // of the range that this corner becomes the upper left corner.
                    grid.Model.DragDropData.dndRowOffset = Math.Max(grid.Model.DragDropData.dndStartRow, pRange.Top) - pRange.Top;
                    grid.Model.DragDropData.dndColOffset = Math.Max(grid.Model.DragDropData.dndStartCol, pRange.Left) - pRange.Left;
                    grid.Model.DragDropData.dndStartRow = Math.Min(grid.Model.DragDropData.dndStartRow, pRange.Top);
                    grid.Model.DragDropData.dndStartCol = Math.Min(grid.Model.DragDropData.dndStartCol, pRange.Left);
                }
                else
                {
                    grid.Model.DragDropData.dndRowOffset = 0;
                    grid.Model.DragDropData.dndColOffset = 0;
                }

                // Show a drop cursor as soon as the user drags out
                // of the current cell.
                Rect cellBounds = grid.RangeToClippedVisibleRect(GridRangeInfo.Cell(rowIndex, colIndex));//, GridRangeOptions.MergeCoveredCells);
                //cellBounds = grid.GridRectangleToScreen(cellBounds);

                // Initiallize static member.
                GridModel.InternalGridDragDropData.dndGridSource = true; // data source is a GridControlBase grid
                GridModel.InternalGridDragDropData.dndGridTargetStyle = false; // Droptarget is a grid and style info is copied


                // Start the drag-and-drop operation.
                this.grid.OleDragDropEventsTarget.OnDragEnter(new SyncfusionDragEventArgs(args, dataObject, false));
                {
                    CancelEventArgs e = new CancelEventArgs();
                    grid.Model.RaiseQueryDragDropMoveClearCells(e);
                    if (!e.Cancel)
                    {
                        // dndCurrentCellText is set in OnDndCacheGlobalData when
                        // selected text from the current cell is copied to clipboard.
                        if (grid.Model.DragDropData.dndCurrentCellText)
                        {
                            IGridCellRenderer pCtrl = grid.Model.DragDropData.dndCurrentCellControl;
                            grid.CurrentCell.EndEdit();
                        }
                    }
                }

                return true; // Call was processed.
            }

            return false; // Grid shall continue to find another task to do.
        }

        bool OnCacheGlobalData(GridDataObject dataObject, GridRangeInfoList selList, out int nDndRowExt, out int nDndColExt)
        {
            nDndRowExt = nDndColExt = 0;

            GridQueryOleDataSourceDataEventArgs e = new GridQueryOleDataSourceDataEventArgs(dataObject, selList, this.dragDropFlags, 0, 0);
            grid.Model.RaiseQueryOleDataSourceData(e);
            this.dragDropFlags = e.DragDropFlags;
            nDndRowExt = e.RowCount;
            nDndColExt = e.ColCount;
            if (e.Handled)
            {
                return e.Result;
            }

            bool bText = (this.grid.Model.Options.DataObjectConsumerOptions & GridDataObjectConsumerOptions.Text) != 0;
            bText &= ((this.grid.Model.Options.DragDropDropTargetFlags & GridDragDropFlags.Text) != 0);
            bool bStyles = (this.grid.Model.Options.DataObjectConsumerOptions & GridDataObjectConsumerOptions.Styles) != 0;
            bStyles &= ((this.grid.Model.Options.DragDropDropTargetFlags & GridDragDropFlags.Styles) != 0);
            bool retVal = false;

            bool clear = false;//flag for clearing the data. Always set to false.

            // Copy text
            if (bText)
            {
                string s = string.Empty;
                if (grid.CurrentCell.Renderer != null)
                {
                    s = grid.CurrentCell.Renderer.ControlText;
                }

                int rowIndex, colIndex;

                bool bCCell = grid.CurrentCell.GetCurrentCell(out rowIndex, out colIndex) && !e.IgnoreCurrentCell;

                if (selList.Count == 1 && bCCell && selList[0] == GridRangeInfo.Cell(rowIndex, colIndex)
                    //&& grid.CurrentCell.HasControlFocus && grid.CurrentCell.Renderer.ge.GetSelectedText(out s)
                    && s.Length > 0)
                {
                    dataObject.SetData(s);
                    bStyles = false;
                    nDndRowExt = 1;
                    nDndColExt = 1;

                    // Mark this attribute if selected text from an edit control is dragged.
                    grid.Model.DragDropData.dndCurrentCellText = true;
                    grid.Model.DragDropData.dndCurrentCellControl = grid.CurrentCell.Renderer;
                }
                else if (grid.Model.TextDataExchange.CopyTextToBuffer(out s, selList, out nDndRowExt, out nDndColExt, clear))
                {
                    dataObject.SetData(s);
                }
            }

            //Copy styles.
            if (bStyles)
            {
                GridCellData data = null;
                grid.Model.CutPaste.CopyCellsToDataObject(out data, selList, clear, out nDndRowExt, out nDndColExt);//GridUtil.IsSet(this.dragDropFlags, GridDragDropFlags.Compose));
                dataObject.SetData(data);
                retVal = true;
            }

            return retVal;
        }


#if WinRT
        public void MouseHoverEnter(PointerRoutedEventArgs e)
        {
        }

        public Windows.UI.Core.CoreCursor Cursor
        {
            get
            {
                return new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 1);
            }
        }

        public void MouseHoverLeave(PointerRoutedEventArgs e)
        {
        }
#endif
    }

    /// <summary>
    /// Drag-and-drop options used with EnableOleDropTarget
    /// </summary>    
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    [Flags]
    public enum GridDragDropFlags
    {
        /// <summary>
        /// Disable drop target.
        /// </summary>
        Disabled = 0x00,

        /// <summary>
        /// Also copy / move column header cells.
        /// </summary>
        ColHeader = 0x01,

        /// <summary>
        /// Also copy / move row header cells.
        /// </summary>
        RowHeader = 0x02,

        /// <summary>
        /// Allow dragging multiple selections.
        /// </summary>
        Multiple = 0x04,

        /// <summary>
        /// Force dragging of CF_TEXT clipboard format.
        /// </summary>
        Text = 0x08,

        /// <summary>
        /// Force dragging of internal styles format.
        /// </summary>
        Styles = 0x10,

        /// <summary>
        /// When copying internal styles, compose the full style of the cell and do not copy only the cell specific attributes.
        /// </summary>
        Compose = 0x20,

        /// <summary>
        ///  Enable autoscroll when user drags out of windows.
        /// </summary>
        AutoScroll = 0x40,

        /// <summary>
        /// Enable edgescroll when user drags to the corner of the window.
        /// </summary>
        EdgeScroll = 0x80,

        /// <summary>
        /// If the user pastes (or drops) more rows than currently available, don't append as many rows as needed.
        /// </summary>
        NoAppendRows = 0x200,

        /// <summary>
        /// If the user pastes (or drops) more columns than currently available, don't append as many columns as needed.
        /// </summary>
        NoAppendCols = 0x400,

        /// <summary>
        /// By default if the user drags multiple rows to the bottom of the grid in an ole drag operation the
        /// outlined rectangle will be clipped at the bottom of the current available rows. If you specify
        /// this option the new rows will be outlined below the last visible row.
        /// </summary>
        OutlineAppendRows = 0x800,

        /// <summary>
        /// By default if the user drags multiple columns to the right of the grid in an ole drag operation the
        /// outlined rectangle will be clipped at the right of the current available columns. If you specify
        /// this option the new columns will be outlined below the last visible column.
        /// </summary>
        OutlineAppendCols = 0x1000,

        /// <summary>
        /// Later ... Paste only: Display "Selected Range is Different"-Dialog.
        /// </summary>
        CheckRangeDim = 0x100
    }


    /// <summary>
    /// Defines an interface for an object that handles Ole Drag Drop events raised by <see cref="GridControlBase"/> objects.
    /// </summary> 
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public interface IGridOleDragDropEventsTarget
    {
        /// <summary>
        /// Occurs when a drag-and-drop operation is completed and before <see cref="Control.DragDrop"/> event is raised.
        /// </summary>
        /// <param name="e"><see cref="SyncfusionDragEventArgs"/> containing data related to this event</param>
        void OnDragDrop(SyncfusionDragEventArgs e);

        /// <summary>
        /// Occurs when an object is dragged into the control's bounds and before <see cref="Control.DragEnter"/> event is raised.
        /// </summary>
        /// <param name="e"><see cref="SyncfusionDragEventArgs"/> containing data related to this event</param>
        void OnDragEnter(SyncfusionDragEventArgs e);

        /// <summary>
        /// Occurs when an object is dragged out of the control's bounds and before <see cref="Control.DragLeave"/> event is raised.
        /// </summary>
        /// <param name="e"><see cref="SyncfusionDragEventArgs"/> containing data related to this event</param>
        void OnDragLeave(EventArgs e);


        /// <summary>
        /// Occurs when an object is dragged over the control's bounds and before <see cref="Control.DragOver"/> event is raised.
        /// </summary>
        /// <param name="e"><see cref="SyncfusionDragEventArgs"/> containing data related to this event</param>
        void OnDragOver(SyncfusionDragEventArgs e);
    }

    /// <summary>
    /// Specifies which default data consumers should be enabled for the grid.
    /// <para/>
    ///    This enumeration has a <see cref="System.FlagsAttribute"/> attribute that allows a bitwise combination of its member values.
    /// </summary>
    /// <remarks>
    /// You can customize the mouse controller's behavior at run-time while the user is performing the action 
    /// by creating an object that implements <see cref="IGridDataObjectConsumer"/> and register this object
    /// with <see cref="GridControlBaseImp.RegisterDataObjectConsumer"/>. This allows you to add support for additional
    /// clipboard formats to be dragged into the grid from outside applications.
    /// <para/>
    /// You can change various options with <see cref="GridControlBaseImp.EnableOleDropTarget"/>. 
    /// <para/>
    /// When you change 
    /// <see cref="GridModelOptions.ControllerOptions"/> in <see cref="GridModelOptions"/>,
    /// this will actually end up calling <see cref="GridControlBaseImp.EnableOleDropTarget"/>
    /// for each associated view.
    /// </remarks>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    [Flags]
    public enum GridDataObjectConsumerOptions
    {
        /// <summary>
        /// No default data objects supported.
        /// </summary>
        None = 0,

        /// <summary>
        /// Enable styles (internal) data objects. This allows you to drag / copy / paste complete cell information.
        /// </summary>
        Styles = 1,

        /// <summary>
        /// Enable text data format. This allows you to drag cell values.
        /// </summary>
        Text = 2,

        /// <summary>
        /// Enable support for all default data objects.
        /// </summary>
#if !WinRT
        [Browsable(false)]
#endif
        All = 0x3
    }

}
