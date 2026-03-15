#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.UI.Xaml.ScrollAxis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
#if WinRT
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
#endif

namespace Syncfusion.UI.Xaml.Grid
{
#if WinRT
    using MouseEventArgs = PointerRoutedEventArgs;
    using MouseButtonEventArgs = PointerRoutedEventArgs;
    using Windows.UI.Input;
    using Windows.UI.Core;
    using System.Diagnostics;
#endif
    public class GridColumnResizingController : IDisposable
    {
        #region Resizing Fields
        SfDataGrid dataGrid;
        private const double HitTestPrecision = 4.0;
        private const double HitTestHiddenColPrecision = 6.0;
        private bool isFirstColumnHidden;

        #endregion

        #region Internal property
        internal bool isHovering { get; set; }
        internal VisibleLineInfo dragLine { get; set; }
        #endregion

        public GridColumnResizingController()
        {

        }

        public GridColumnResizingController(SfDataGrid dataGrid)
        {
            this.dataGrid = dataGrid;
        }

        #region Resizing

        internal bool CanResizeColumn(GridColumn column)
        {
            if (column == null)
                return false;

            bool canResizeColumn = false;
            var resizeColumn = column.ReadLocalValue(GridColumn.AllowResizingProperty);
            if (resizeColumn != DependencyProperty.UnsetValue)
                canResizeColumn = column.AllowResizing;
            if ((resizeColumn == DependencyProperty.UnsetValue) && this.dataGrid.AllowResizingColumns)
                canResizeColumn = true;
            return canResizeColumn;
        }

        /// <summary>
        /// Ensures the VSM for hidden columns OnGridColumnCollectionChanged
        /// </summary>
        /// <param name="OldStartingIndex"></param>
        /// <param name="NewStartingIndex"></param>
        /// <remarks></remarks>
        internal void EnsureVSMOnColumnCollectionChanged(int OldStartingIndex, int NewStartingIndex)
        {
            if (OldStartingIndex != -1 && OldStartingIndex - 1 >= 0)
                this.ProcessResizeStateManager(this.dataGrid.Columns[OldStartingIndex - 1]);
            if (NewStartingIndex != -1 && NewStartingIndex - 1 >= 0)
                this.ProcessResizeStateManager(this.dataGrid.Columns[NewStartingIndex - 1]);
            if (OldStartingIndex != -1 && OldStartingIndex > 0 && OldStartingIndex < this.dataGrid.Columns.Count)
                this.ProcessResizeStateManager(this.dataGrid.Columns[OldStartingIndex]);
            if (NewStartingIndex != -1 && NewStartingIndex > 0 && NewStartingIndex < this.dataGrid.Columns.Count)
                this.ProcessResizeStateManager(this.dataGrid.Columns[NewStartingIndex]);
            if (OldStartingIndex != -1 && OldStartingIndex + 1 < this.dataGrid.Columns.Count)
                this.ProcessResizeStateManager(this.dataGrid.Columns[OldStartingIndex + 1]);
            if (NewStartingIndex != -1 && NewStartingIndex + 1 < this.dataGrid.Columns.Count)
                this.ProcessResizeStateManager(this.dataGrid.Columns[NewStartingIndex + 1]);
        }

        #region Resizing by mouse

        /// <summary>
        /// Returns the VisibleLine on the pointer hitting point
        /// </summary>
        /// <param name="point"></param>
        /// <param name="cursor"></param>
        /// <returns></returns>
        /// <remarks></remarks>
#if WinRT
        internal VisibleLineInfo HitTest(Point point,out CoreCursorType cursor)
#else
        internal VisibleLineInfo HitTest(Point point, out Cursor cursor)
#endif
        {
            VisibleLineInfo info = point.X == 0.0 ? this.dataGrid.VisualContainer.ScrollColumns.GetVisibleLineAtPoint(point.X)
                                                  : dataGrid.VisualContainer.ScrollColumns.GetLineNearCorner(point.X, HitTestPrecision);
#if WinRT
            if (info != null && info.LineIndex > this.dataGrid.ResolveToStartColumnIndex() - 1)
                cursor = CoreCursorType.SizeWestEast;             
            else            
                cursor = CoreCursorType.Arrow;            
#else
            cursor = Cursors.SizeWE;
#endif
            if (dataGrid.AllowResizingHiddenColumns && info == null)
            {
                var lineInfo = dataGrid.VisualContainer.ScrollColumns.GetLineNearCorner(point.X, HitTestHiddenColPrecision, CornerSide.Bottom);
                if (lineInfo == null)
                {
                    var lines = this.dataGrid.VisualContainer.ScrollColumns.GetVisibleLines();
                    var visibleLine = lines.GetVisibleLineAtPoint(point.X);
                    if (visibleLine != null)
                    {
                        var count = 0;
                        if (this.dataGrid.VisualContainer.ColumnWidths.GetHidden(0, out count))
                        {
                            var d = visibleLine.ClippedOrigin - point.X;
                            if (Math.Abs(d) <= HitTestHiddenColPrecision)
                            {
                                isFirstColumnHidden = true;
                                lineInfo = visibleLine;
                            }
                        }
                    }
                }
                if (lineInfo != null)
                {
                    var lineIndex = lineInfo.LineIndex;
                    lineIndex = (isFirstColumnHidden && lineIndex > 0) ? lineIndex - 1 : lineIndex + 1;
                    int rc;

                    if (dataGrid.VisualContainer.ColumnWidths.GetHidden(lineIndex, out rc) || dataGrid.VisualContainer.ColumnWidths[lineIndex] == 0.0)
#if WinRT
                        cursor = CoreCursorType.SizeNorthwestSoutheast;
#else
                        cursor = Cursors.SizeNWSE;
#endif
                        info = lineInfo;
                }
            }

            var indentCellCount = this.dataGrid.ResolveToScrollColumnIndex(0);
            if (info != null && (info.LineIndex < indentCellCount && !dataGrid.AllowResizingHiddenColumns))
                return null;
            return info;
        }

        /// <summary>
        /// Computing width on resizing with Minimum & MaximumWidth constrains
        /// </summary>
        /// <param name="column"></param>
        /// <param name="Width"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private double ComputeResizingWidth(GridColumn column, double Width)
        {
            var computedWidth = 0d;
            var colIndex = this.dataGrid.ResolveToScrollColumnIndex(this.dataGrid.Columns.IndexOf(column));
            if (!double.IsNaN(column.ExtendedWidth))
            {
                return Width;
            }
            if (!double.IsNaN(column.MinimumWidth) || !double.IsNaN(column.MaximumWidth))
            {
                if (!double.IsNaN(column.MinimumWidth) && !double.IsNaN(column.MaximumWidth))
                {
                    if (column.MinimumWidth < Width && column.MaximumWidth > Width)
                    {
                        computedWidth = Width;
                    }
                    else if (column.MinimumWidth < Width)
                    {
                        computedWidth = column.MaximumWidth;
                    }
                    else
                        computedWidth = column.MinimumWidth;
                }
                else if (!double.IsNaN(column.MinimumWidth) && double.IsNaN(column.MaximumWidth))
                {
                    if (column.MinimumWidth < Width)
                        computedWidth = Width;
                    else
                        computedWidth = column.MinimumWidth;
                }
                else if(double.IsNaN(column.MinimumWidth) && !double.IsNaN(column.MaximumWidth))
                {
                    if (column.MaximumWidth > Width)
                    {
                        computedWidth = Width;
                    }
                    else
                    {
                        computedWidth = column.MaximumWidth;
                    }
                }
            }
            else
            {
                computedWidth = Width;
            }
            return computedWidth;
        }

        /// <summary>
        /// performs mouse move action on GridHeaderCellControl
        /// </summary>
        /// <param name="e">An <see cref="T:Windows.UI.Xaml.Input.PointerRoutedEventArgs">PointerRoutedEventArgs</see> that contains the event data.</param>
        /// <param name="headerCell"></param>
        /// <remarks></remarks>
#if WinRT
        internal void DoActionOnMouseMove(PointerPoint pp, GridHeaderCellControl headerCell)
#else
        internal void DoActionOnMouseMove(Point pp, GridHeaderCellControl headerCell)
#endif
        {
            bool isLastColumnHidden = false;
#if WinRT
            double hScrollChange = 0;

            for (int i = 0; i < this.dataGrid.VisualContainer.FrozenColumns; i++)
            {
                hScrollChange += this.dataGrid.VisualContainer.ColumnWidths[i];
            }
			// since panel x moves to negative the resizing not properly working, so Abs removed for x calculation WRT-1311, skipping for negative shrinking for first column.
            var pointerPoint = new Point((pp.Position.X - (dataGrid.VisualContainer.HScrollBar.Value - hScrollChange)), Math.Abs(pp.Position.Y - dataGrid.VisualContainer.VScrollBar.Value));
            if (this.isHovering && pp.Properties.IsLeftButtonPressed && this.dragLine != null)
            {
                var delta = 0.0d;
                int repeatCount;
                bool isHidden;
                isHidden = isFirstColumnHidden ? dataGrid.VisualContainer.ColumnWidths.GetHidden(dragLine.LineIndex - 1, out repeatCount)
                                               : dataGrid.VisualContainer.ColumnWidths.GetHidden(dragLine.LineIndex + 1, out repeatCount);
                delta = isFirstColumnHidden ? pointerPoint.X
                                            : pointerPoint.X - dragLine.Corner;
#else
#if WinRT
            pp=new Point(Math.Abs(pp.X-(dataGrid.VisualContainer.HScrollBar.Value)), Math.Abs(pp.Y));
#endif
#if SILVERLIGHT
            if (this.isHovering && this.dragLine != null)
#else
            if (this.isHovering && this.dragLine != null && (headerCell.isMouseLeftButtonPressed || headerCell.isTouchPressed))
#endif
            {
                var delta = 0.0d;
                int repeatCount;
                bool isHidden;
                isHidden = isFirstColumnHidden ? dataGrid.VisualContainer.ColumnWidths.GetHidden(dragLine.LineIndex - 1, out repeatCount)
                                               : dataGrid.VisualContainer.ColumnWidths.GetHidden(dragLine.LineIndex + 1, out repeatCount);
                delta = isFirstColumnHidden ? pp.X
                                            : pp.X - dragLine.Corner;
#endif
                double width = Math.Max(0, dragLine.Size + delta);
                var args = new ResizingColumnsEventArgs(this.dataGrid) { ColumnIndex = dragLine.LineIndex, Width = width };
                if (isHidden && dataGrid.AllowResizingHiddenColumns && 
#if WinRT
                    Window.Current.CoreWindow.PointerCursor.Type == CoreCursorType.SizeNorthwestSoutheast
#else
                    headerCell.Cursor == Cursors.SizeNWSE
#endif
                    )
                {
                    if (delta <= 2)
                        return;
                    var hiddenLineIndex = dragLine.LineIndex + 1;
                    if (isFirstColumnHidden)
                        hiddenLineIndex = dragLine.LineIndex - repeatCount;
                    var columnIndex = this.dataGrid.ResolveToGridVisibleColumnIndex(hiddenLineIndex);

                    dataGrid.Columns[columnIndex].IsHidden = false;
                    this.dataGrid.VisualContainer.ColumnWidths[hiddenLineIndex] = 10;
                    isLastColumnHidden = (columnIndex + 1 == dataGrid.Columns.Count);
#if WinRT
                    var cursor = CoreCursorType.Arrow;
                    dragLine = HitTest(new Point(pointerPoint.X + HitTestPrecision, pointerPoint.Y),out cursor);
                    if (cursor != CoreCursorType.Arrow)
                        SetPointerCursor(cursor);
#else
                    var cursor = headerCell.Cursor;
                    if (dragLine != null && dragLine.LineIndex != hiddenLineIndex)
                    {
                        dragLine = dataGrid.VisualContainer.ScrollColumns.GetVisibleLineAtLineIndex(hiddenLineIndex);
                        cursor = Cursors.SizeWE;
                    }
                    SetPointerCursor(cursor, headerCell);
#endif
                    if (!double.IsNaN(headerCell.Column.MinimumWidth))
                        delta = Math.Max(delta, headerCell.Column.MinimumWidth);
                    if (!double.IsNaN(headerCell.Column.MaximumWidth))
                        delta = Math.Min(delta, headerCell.Column.MaximumWidth);
                    if (dragLine != null)
                        args = new ResizingColumnsEventArgs(this.dataGrid) { ColumnIndex = dragLine.LineIndex, Width = delta };
#if !WinRT
                    if (isLastColumnHidden)
                        dragLine = HitTest(new Point(pp.X + HitTestPrecision, pp.Y), out cursor);
                    SetPointerCursor(cursor, headerCell);
#endif
                }
                if (dragLine != null && !this.dataGrid.RaiseResizingColumnsEvent(args))
                {
                    var headerRow = dataGrid.RowGenerator.Items.FirstOrDefault(row => row.RowIndex == dataGrid.GetHeaderIndex());
                    var columnBase = headerRow.VisibleColumns.FirstOrDefault(col => col.ColumnIndex == (dragLine.LineIndex));
                    if (columnBase != null && columnBase.ColumnElement is GridHeaderCellControl)
                    {
                        var column = (columnBase.ColumnElement as GridHeaderCellControl).Column;
                        if (dragLine.LineIndex > dataGrid.FrozenColumnCount)
                            dataGrid.VisualContainer.ScrollColumns.SetLineResize(dragLine.LineIndex, ComputeResizingWidth(column, args.Width != 0 ? args.Width : 1));
                        else
                            column.Width = ComputeResizingWidth(column, args.Width != 0 ? args.Width : 1);
                    }
                    isFirstColumnHidden = false;
                }
            }
            else
            {
                this.isHovering = false;
#if WinRT
                Point point = pointerPoint;
                var cursor = CoreCursorType.SizeWestEast;
#else
                Point point = pp;
                var cursor = headerCell.Cursor;
#endif

#if WinRT
                var hit = HitTest(pointerPoint,out cursor);
                if(hit != null && cursor != CoreCursorType.Arrow)  
#else
                var hit= HitTest(pp,out cursor);               
#if !WP
                if (hit != null && (cursor == Cursors.SizeNWSE || hit.LineIndex > ((this.dataGrid.GroupColumnDescriptions.Count - 1) + (this.dataGrid.ShowRowHeader ? 1 : 0) + (this.dataGrid.DetailsViewManager.HasDetailsView ? 1 : 0))))
#elif WP
                if(hit!=null)
#endif                                                                                            
#endif        
                {
                    var lineIndex = hit.LineIndex;
#if !WinRT
                    if (cursor == Cursors.SizeNWSE)
#else
                    if(cursor == CoreCursorType.SizeNorthwestSoutheast)
#endif
                    {
                        lineIndex++;
                        int count;
                        if (this.dataGrid.VisualContainer.ColumnWidths.GetHidden(lineIndex, out count))
                            lineIndex += count - 1;
                        else
                            lineIndex--;
                    }
                    var index = this.dataGrid.ResolveToGridVisibleColumnIndex(lineIndex);
                    if (index >= 0 && index < this.dataGrid.Columns.Count)
                    {
                        GridColumn column = this.dataGrid.Columns[index];
                        var lineindex = this.dataGrid.ResolveToScrollColumnIndex(this.dataGrid.Columns.IndexOf(column));                       
                        if (!this.CanResizeColumn(column))
                            return;
#if !WinRT
                    this.SetPointerCursor(cursor, headerCell);
#else
                        this.SetPointerCursor(cursor);
#endif
                    }
                }
                bool edgeHit = point.X < 5.0 && point.X > 0;
                if (hit == null && edgeHit)
                {
                    hit = new VisibleLineInfo(0, 0, 0, point.X, 0, true, false);
                }
                if (hit != null || edgeHit)
                {                          
                    var row = this.dataGrid.VisualContainer.ScrollRows.GetVisibleLineAtPoint(point.Y);
                    if (row != null && row.IsHeader)
                        this.isHovering = true;
                }
#if WinRT
                if (!isHovering && Window.Current.CoreWindow.PointerCursor.Type != CoreCursorType.Arrow)
                    SetPointerCursor(CoreCursorType.Arrow);
#else
                if (!isHovering && headerCell.Cursor != Cursors.Arrow && (!headerCell.isMouseLeftButtonPressed || !headerCell.isTouchPressed))
                    SetPointerCursor(Cursors.Arrow,headerCell);
#endif
            }
        }

        /// <summary>
        /// Perfoms MouseUp operation on GridHeaderCellControl
        /// </summary>
        /// <param name="e">An <see cref="T:System.Windows.Input.MouseButtonEventArgs">MouseButtonEventArgs</see> that contains the event data.</param>
        /// <param name="headerCell"></param>
        /// <remarks></remarks>
        internal void DoActionOnMouseUp(MouseButtonEventArgs e, GridHeaderCellControl headerCell)
        {
#if WinRT
            PointerPoint pp = e.GetCurrentPoint(this.dataGrid.VisualContainer);
            double hScrollChange = 0;

            for (int i = 0; i < this.dataGrid.VisualContainer.FrozenColumns; i++)
            {
                hScrollChange += this.dataGrid.VisualContainer.ColumnWidths[i];
            }
            var pointerPoint = new Point(pp.Position.X - (dataGrid.VisualContainer.HScrollBar.Value - hScrollChange), pp.Position.Y - dataGrid.VisualContainer.VScrollBar.Value);
            double delta = pointerPoint.X - dragLine.Corner;
#else
            Point pp = e.GetPosition(this.dataGrid.VisualContainer);
            double delta = pp.X - dragLine.Corner;
#endif
            var columnIndex = this.dataGrid.ResolveToGridVisibleColumnIndex(dragLine.LineIndex);
            double width = Math.Max(0, dragLine.Size + delta);
            if (!(columnIndex >= 0 && columnIndex < this.dataGrid.Columns.Count))
                return;
            width = ComputeResizingWidth(this.dataGrid.Columns[columnIndex], width);
            this.dataGrid.VisualContainer.ScrollColumns.ResetLineResize();
            var args = new ResizingColumnsEventArgs(this.dataGrid) { ColumnIndex = dragLine.LineIndex, Width = width };
            if (columnIndex >= 0 && !dataGrid.RaiseResizingColumnsEvent(args))
            {
                this.dataGrid.Columns[columnIndex].IsHidden = (args.Width == 0 || args.Width == 1);
                if ((args.Width == 0) || (args.Width == 1))
                {
                    this.dataGrid.Columns[columnIndex].Width = Math.Round(args.Width);
                }
                else
                {
                    this.dataGrid.Columns[columnIndex].Width = Math.Round(args.Width) == 0 ? 20 : Math.Round(args.Width);
                }
            }
            if ((args.Width == 0 || columnIndex >= 0 && dataGrid.Columns[columnIndex].IsHidden) && dataGrid.AllowResizingHiddenColumns && dataGrid.VisualContainer.ColumnCount == this.dataGrid.ResolveToGridVisibleColumnIndex(dragLine.LineIndex + 1))
            {
                var headerRow = dataGrid.RowGenerator.Items.FirstOrDefault(row => row.RowIndex == dataGrid.GetHeaderIndex());
                var columnBase = headerRow.VisibleColumns.FirstOrDefault(column => column.ColumnIndex == dragLine.LineIndex + 1);
                var prevHiddenIndex = columnIndex - 1;
                while (dataGrid.Columns[prevHiddenIndex].IsHidden)
                    prevHiddenIndex--;
                if (columnBase != null)
                {
                    var prevColumnHeader = (columnBase.ColumnElement as GridHeaderCellControl);
                    if (prevHiddenIndex != columnIndex)
                    {
                        columnBase = headerRow.VisibleColumns.FirstOrDefault(column => column.ColumnIndex == prevHiddenIndex);
                        prevColumnHeader = columnBase.ColumnElement as GridHeaderCellControl;
                    }
                    dataGrid.VisualContainer.ColumnWidths.SetHidden(dragLine.LineIndex, dragLine.LineIndex, true);
                    dataGrid.Columns[dataGrid.ResolveToGridVisibleColumnIndex(dragLine.LineIndex)].IsHidden = true;
                    dataGrid.Columns[dataGrid.ResolveToGridVisibleColumnIndex(dragLine.LineIndex)].Width = 0.0;
                }
            }
            dragLine = null;
#if WinRT
            headerCell.ReleasePointerCaptures();
            if (Window.Current.CoreWindow.PointerCursor.Type != CoreCursorType.Arrow)
            {
                Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 1);
            }
#else
            headerCell.isMouseLeftButtonPressed = false;
            headerCell.ReleaseMouseCapture();
            if (headerCell.Cursor != Cursors.Arrow)
                SetPointerCursor(Cursors.Arrow, headerCell);
#endif
        }

        /// <summary>
        /// Sets the Cursor for the Pointer
        /// </summary>
        /// <param name="cursor"></param>
        /// <param name="headerCell"></param>
        /// <remarks></remarks>
#if WinRT
        internal void SetPointerCursor(CoreCursorType cursorType)
#else
        internal void SetPointerCursor(Cursor cursor, GridHeaderCellControl headerCell)
#endif
        {
#if WinRT
            Window.Current.CoreWindow.PointerCursor = new CoreCursor(cursorType, 1);
#else
            headerCell.Cursor = cursor;
#endif
        }

        #endregion

        #region Popup Resizing

        /// <summary>
        /// Calls on popup resizing by touch
        /// </summary>
        /// <param name="IsinLeft">If set to <see langword="true"/>, then ; otherwise, .</param>
        /// <param name="delta"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        internal bool OnPopupContentResizing(bool IsinLeft, double delta)
        {
            if (this.dataGrid.GridColumnDragDropController == null)
                return false;
            if (this.dataGrid.GridColumnDragDropController.DraggablePopup.IsOpen)
            {
                double width;
                if (IsinLeft && this.dataGrid.GridColumnDragDropController.DragLeftLine != null)
                {
                    width = Math.Max(0, this.dataGrid.GridColumnDragDropController.PreviousLeftLineSize + delta);
                    if (ResizingColumn(this.dataGrid.GridColumnDragDropController.DragLeftLine.LineIndex, width))
                    {
                        this.dataGrid.GridColumnDragDropController.PreviousLeftLineSize = width;
                        this.dataGrid.GridColumnDragDropController.needToUpdatePosition = true;
                        return true;
                    }
                    return false;
                }
                else if (this.dataGrid.GridColumnDragDropController.DragRightLine != null)
                {
                    width = Math.Max(0, this.dataGrid.GridColumnDragDropController.PreviousRightLineSize + delta);
                    if (ResizingColumn(this.dataGrid.GridColumnDragDropController.DragRightLine.LineIndex, width))
                    {
                        this.dataGrid.GridColumnDragDropController.PreviousRightLineSize = width;
                        dataGrid.GridColumnDragDropController.needToUpdatePosition = true;
                        return true;
                    }
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// calls popup resized by touch
        /// </summary>
        /// <param name="IsinLeft">If set to <see langword="true"/>, then ; otherwise, .</param>
        /// <param name="delta"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        internal bool OnPopupContentResized(bool IsinLeft, double delta)
        {
            if (this.dataGrid.GridColumnDragDropController == null)
                return false;
            if (this.dataGrid.GridColumnDragDropController.DraggablePopup.IsOpen)
            {
                double width;
                if (IsinLeft && this.dataGrid.GridColumnDragDropController.DragLeftLine != null)
                {
                    width = Math.Max(0, dataGrid.GridColumnDragDropController.PreviousLeftLineSize);
                    var result = SetColumnWidth(dataGrid.GridColumnDragDropController.DragLeftLine.LineIndex, width);
                    dataGrid.GridColumnDragDropController.needToUpdatePosition = true;
                    return result;
                }
                else if (this.dataGrid.GridColumnDragDropController.DragRightLine != null)
                {
                    width = Math.Max(0, dataGrid.GridColumnDragDropController.PreviousRightLineSize);
                    var result = SetColumnWidth(dataGrid.GridColumnDragDropController.DragRightLine.LineIndex, width);
                    dataGrid.GridColumnDragDropController.needToUpdatePosition = true;
                    return result;
                }
            }
            return false;
        }

        /// <summary>
        /// perform Resizing column for given index and sets the width 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private bool ResizingColumn(int index, double width)
        {
            if (width <= 0)
                return false;
            int columnindex = this.dataGrid.ResolveToGridVisibleColumnIndex(index);
            var gridcolumn = this.dataGrid.Columns[columnindex];

            var args = new ResizingColumnsEventArgs(this.dataGrid) { ColumnIndex = index, Width = width };
            if (this.dataGrid.RaiseResizingColumnsEvent(args))
                return false;

            width = ComputeResizingWidth(gridcolumn, args.Width);
            this.dataGrid.VisualContainer.ScrollColumns.SetLineResize(index, ComputeResizingWidth(gridcolumn, width));
            if (width != args.Width)
                return false;
            return true;
        }

        /// <summary>
        /// sets the width for column for given column index
        /// </summary>
        /// <param name="index"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private bool SetColumnWidth(int index, double width)
        {
            int columnindex = this.dataGrid.ResolveToGridVisibleColumnIndex(index);
            var gridcolumn = this.dataGrid.Columns[columnindex];

            var args = new ResizingColumnsEventArgs(this.dataGrid) { ColumnIndex = index, Width = Math.Round(width, 2) };
            if (this.dataGrid.RaiseResizingColumnsEvent(args))
                return false;

            width = args.Width;

            this.dataGrid.VisualContainer.ScrollColumns.ResetLineResize();
            gridcolumn.Width = width;
            return true;
        }

        #endregion

        #region Hidden Resizinig VSM

        /// <summary>
        /// Applies the Hidden State VSM if the Column in hidden.
        /// </summary>
        /// <param name="column">The column.</param>

        internal void ProcessResizeStateManager(GridColumn column)
        {
            var headerRow = dataGrid.RowGenerator.Items.FirstOrDefault(row => row.RowIndex == dataGrid.GetHeaderIndex());
            if (headerRow == null)
                return;
            var lastColumn = this.dataGrid.Columns.LastOrDefault(col => !col.IsHidden);
            var lastColumnBase = headerRow.VisibleColumns.FirstOrDefault(col => col.GridColumn == lastColumn);
            if (!(this.dataGrid.AllowResizingColumns && this.dataGrid.AllowResizingHiddenColumns))
            {
                var columnbase = headerRow.VisibleColumns.FirstOrDefault(col => col.GridColumn == column);
                if (columnbase == null) return;
                (columnbase.ColumnElement as GridHeaderCellControl).hiddenResizingVisualState = VisualStateManager.GoToState((GridHeaderCellControl)columnbase.ColumnElement, "Normal", true) ? string.Empty : "Normal";
                return;
            }
            if (column.IsHidden)
            {
                if (lastColumn != this.dataGrid.Columns.LastOrDefault())
                {
                    string lastColVisualState = this.dataGrid.Columns[this.dataGrid.Columns.IndexOf(lastColumn) - 1 >= 0 ? this.dataGrid.Columns.IndexOf(lastColumn) - 1 : 0].IsHidden ? "HiddenState" : "LastColumnHidden";
                    if (lastColumnBase == null || !(lastColumnBase.ColumnElement is GridHeaderCellControl)) return;
                    (lastColumnBase.ColumnElement as GridHeaderCellControl).hiddenResizingVisualState = VisualStateManager.GoToState((GridHeaderCellControl)lastColumnBase.ColumnElement, lastColVisualState, true) ? string.Empty : lastColVisualState;
                }
                var columnIndex = dataGrid.Columns.IndexOf(column);
                var index = dataGrid.ResolveToScrollColumnIndex(columnIndex);
                var visualState = lastColumn != column ? "PreviousColumnHidden" : "LastColumnHidden";
                index = lastColumn != column ? index + 1 : index - 1;
                var columnBase = headerRow.VisibleColumns.FirstOrDefault(col => col.ColumnIndex == index);
                if (this.dataGrid.ResolveToGridVisibleColumnIndex(index) < this.dataGrid.Columns.Count)
                    if (this.dataGrid.Columns[this.dataGrid.ResolveToGridVisibleColumnIndex(index)].IsHidden) return;
                if (columnBase == null || !(columnBase.ColumnElement is GridHeaderCellControl)) return;
                if (column.IsHidden && !(columnBase.ColumnElement as GridHeaderCellControl).Column.IsHidden)
                    (columnBase.ColumnElement as GridHeaderCellControl).hiddenResizingVisualState = VisualStateManager.GoToState((GridHeaderCellControl)columnBase.ColumnElement, visualState, true) ? string.Empty : visualState;
                else
                    VisualStateManager.GoToState((GridHeaderCellControl)columnBase.ColumnElement, "Normal", true);
                if (lastColumnBase == columnBase && lastColumn != this.dataGrid.Columns.LastOrDefault())
                    (columnBase.ColumnElement as GridHeaderCellControl).hiddenResizingVisualState = VisualStateManager.GoToState((GridHeaderCellControl)columnBase.ColumnElement, "HiddenState", true) ? string.Empty : "HiddenState";
            }
            else
            {
                var columnIndex = dataGrid.Columns.IndexOf(column);                
                var index = dataGrid.ResolveToScrollColumnIndex(columnIndex);
                index = lastColumn != column ? index + 1 : index - 1;
                var columnBase = headerRow.VisibleColumns.FirstOrDefault(col => col.ColumnIndex == index);               
                if (columnBase == null || !(columnBase.ColumnElement is GridHeaderCellControl)) return;
                (columnBase.ColumnElement as GridHeaderCellControl).hiddenResizingVisualState = VisualStateManager.GoToState((GridHeaderCellControl)columnBase.ColumnElement, "Normal", true) ? string.Empty : "Normal";
                if (lastColumn != this.dataGrid.Columns.LastOrDefault())
                {
                    string lastColVisualState = this.dataGrid.Columns[this.dataGrid.Columns.IndexOf(lastColumn) - 1].IsHidden ? "HiddenState" : "LastColumnHidden";
                    if (lastColumnBase == null || !(lastColumnBase.ColumnElement is GridHeaderCellControl)) return;
                    (lastColumnBase.ColumnElement as GridHeaderCellControl).hiddenResizingVisualState = VisualStateManager.GoToState((GridHeaderCellControl)lastColumnBase.ColumnElement, lastColVisualState, true) ? string.Empty : lastColVisualState;
                    Debug.WriteLine("LastColumnHidden apply for " + (lastColumnBase.ColumnElement as GridHeaderCellControl).Column.MappingName);
                }
                var newIndex = columnIndex - 1;
                if (newIndex >= 0 && newIndex < this.dataGrid.Columns.Count && dataGrid.Columns[newIndex].IsHidden)
                {
                    columnBase = headerRow.VisibleColumns.FirstOrDefault(col => col.GridColumn == column);
                    if (columnBase == null || !(columnBase.ColumnElement is GridHeaderCellControl)) return;
                    (columnBase.ColumnElement as GridHeaderCellControl).hiddenResizingVisualState = VisualStateManager.GoToState((GridHeaderCellControl)columnBase.ColumnElement, "PreviousColumnHidden", true) ? string.Empty : "PreviousColumnHidden";
                }
            }
        }
        #endregion

        #endregion

        public void Dispose()
        {
            dragLine = null;
            dataGrid = null;
        }
    }
}
