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
using Syncfusion.Windows.Controls.Scroll;
using System.Windows;
using Syncfusion.Windows.Controls.Cells;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Syncfusion.Windows.Controls.Grid
{
    enum GridDataColumnChooserControllerMode
    {
        ColumnDrag,
        ColumnDrop
    }
    class GridDataColumnChooserMouseController : IMouseController
    {
        public GridDataColumnChooserMouseController(GridControlBase grid)
        {
            this.Grid = grid;
            this.DragHeaderVisible = true;
            this.upIndicatorWindow = new PopupPositionWindow()
            {
                Child = new AnimatedGrid()
                {
                    Direction = Direction.Up
                },
                Width = 20,
                Height = 25,
                AllowsTransparency = true,
            };

            this.downIndicatorWindow = new PopupPositionWindow()
            {
                Child = new AnimatedGrid()
                {
                    Direction = Direction.Down
                },
                Width = 20,
                Height = 25,
                AllowsTransparency = true
            };

            if (this.WasDragged)
            {
                if (this.DragHeaderVisible)
                {
                    this._cursor = Cursors.Arrow;
                }
                else
                {
                    this._cursor = Cursors.No;
                }
            }
            else
            {
                this._cursor = Cursors.Arrow;
            }

        }

        public const int OffsetColumn = 4;

        public int ResolveVisibleColumnIndexToPosition(int colIdx)
        {
            if (this.TableProperties == null)
            {
                return -1;
            }

            if (this.Table.HasNestedTables)
            {
                colIdx = this.TableProperties.ShowRecordPlusMinus ? colIdx + 1 : colIdx;
            }

            if (this.Table.HasGroups && this.TableProperties.ShowGroupCaptionPlusMinus)
            {
                var maxLevel = this.Table.GroupModel.GetMaxLevel();
                colIdx = colIdx + maxLevel;
            }

            //// adjust the value of colIndex to get the exact column
            colIdx = this.TableProperties.ShowRowHeader ? colIdx + 1 : colIdx;
            return colIdx;
        }

        public int ResolvePositionToVisibleColumnIndex(int colIdx)
        {
            if (this.TableProperties == null)
            {
                return -1;
            }

            if (this.Table.HasNestedTables)
            {
                colIdx = this.TableProperties.ShowRecordPlusMinus ? colIdx - 1 : colIdx;
            }

            if (this.Table.HasGroups && this.TableProperties.ShowGroupCaptionPlusMinus)
            {
                var maxLevel = this.Table.GroupModel.GetMaxLevel();
                colIdx = colIdx - maxLevel;
            }

            //// adjust the value of colIndex to get the exact column
            colIdx = this.TableProperties.ShowRowHeader ? colIdx - 1 : colIdx;
            return colIdx;
        }

        public GridControlBase Grid
        {
            get;
            private set;
        }


        public int DragColIndex = -1;

        private GridDataColumnChooserWindow columnChooserGrid;
        public GridDataColumnChooserWindow ColumnChooserGrid
        {
            get
            {
                if (this.columnChooserGrid == null && this.Grid != null)
                {
                    var dataGrid = this.Grid.FindParentElementOfType<GridDataControl>();
                    if (dataGrid != null)
                    {
                        this.columnChooserGrid = dataGrid.ColumnChooserGrid;
                    }
                }

                return this.columnChooserGrid;
            }
        }

        public GridDataTableModel TableModel
        {
            get
            {
                var tableModel = this.Grid.Model as GridDataTableModel;
                if (tableModel != null)
                {
                    return tableModel;
                }
                return null;
            }
        }

        public GridDataTable Table
        {
            get
            {
                var table = this.TableModel != null ? this.TableModel.Table : null;
                return table;
            }
        }

        public GridDataTableProperties TableProperties
        {
            get
            {
                var tableProperties = this.TableModel != null ? this.TableModel.TableProperties : null;
                return tableProperties;
            }
        }

        public bool DragHeaderVisible
        {
            get;
            private set;
        }

        protected bool WasDragged
        {
            get;
            set;
        }

        protected GridDataColumnChooserControllerMode Mode
        {
            get;
            set;
        }

        private PopupDragWindow dragWindow;
        protected PopupDragWindow DragWindow
        {
            get
            {
                return this.dragWindow;
            }

            set
            {
                this.dragWindow = value;
            }
        }

        private PopupPositionWindow upIndicatorWindow;
        private PopupPositionWindow downIndicatorWindow;
        public AnimatedGrid UpIndicator
        {
            get
            {
                return this.upIndicatorWindow.Child as AnimatedGrid;
            }
        }

        public AnimatedGrid DownIndicator
        {
            get
            {
                return this.downIndicatorWindow.Child as AnimatedGrid;
            }
        }

        #region IMouseController Members

        public virtual string Name
        {
            get { return "ColumnChooserMouseController"; }
        }

        private Cursor _cursor;

        public System.Windows.Input.Cursor Cursor
        {
            get
            {
                return _cursor;
            }
            set
            {
                _cursor = value;
            }
        }

        #region Hidden functions

        public virtual void MouseHoverEnter(System.Windows.Input.MouseEventArgs e)
        {
        }

        public virtual void MouseHover(MouseControllerEventArgs e)
        {
        }

        public virtual void MouseHoverLeave(System.Windows.Input.MouseEventArgs e)
        {
        }

        #endregion

        private Point mouseDownPoint;
        public virtual void MouseDown(MouseControllerEventArgs e)
        {
            this.mouseDownPoint = e.Location;
        }

        public virtual void MouseMove(MouseControllerEventArgs e)
        {
            if (e.Location.X >= this.mouseDownPoint.X)
            {
                this.MouseFlowDirection = FlowDirection.RightToLeft;
            }
            else
            {
                this.MouseFlowDirection = FlowDirection.LeftToRight;
            }
        }

        public virtual void MouseUp(MouseControllerEventArgs e)
        {
        }

        public virtual void CancelMode()
        {
        }

        public virtual void RestoreMode()
        {
        }

        protected VisibleLineInfo HitTest(int colIndex)
        {
            if (this.Mode == GridDataColumnChooserControllerMode.ColumnDrag)
            {
                return this.Grid.ScrollColumns.GetVisibleLineAtLineIndex(colIndex);
            }
            else
            {
                return this.ColumnChooserGrid.Grid.ScrollColumns.GetVisibleLineAtLineIndex(colIndex);
            }
        }

        public virtual int HitTest(MouseControllerEventArgs mouseEventArgs, IMouseController controller)
        {
            return 0;
        }

        public virtual bool SupportsCancelMouseCapture
        {
            get
            {
                return false;
            }
        }

        public virtual bool SupportsMouseTracking
        {
            get
            {
                return false;
            }
        }

        #region Helper methods
        public bool IsMouseOverColumnChooserArea()
        {
            if (this.Grid == null || this.TableModel == null || this.Table == null)
            {
                return false;
            }
            var mousePos = Mouse.GetPosition(this.ColumnChooserGrid.ParentGrid);//Temp Added
            var rect = this.ColumnChooserGrid.ParentGrid.GetClipRect(ScrollAxisRegion.Body, ScrollAxisRegion.Body);//Temp Added            
            if (mousePos.X < 0 || mousePos.Y < 0 || mousePos.X > this.ColumnChooserGrid.ParentGrid.ActualWidth || mousePos.Y > this.ColumnChooserGrid.ParentGrid.ActualHeight)
            {
                return true;
            }
            //var mousePos = Mouse.GetPosition(this.ColumnChooserGrid.Grid);
            //var rect = this.ColumnChooserGrid.Grid.GetClipRect(ScrollAxisRegion.Header, ScrollAxisRegion.Footer);
            var result = rect.Contains(mousePos);
            return result;
        }

        public bool IsMouseOverGrid()
        {
            if (this.Grid == null || this.TableModel == null || this.Table == null)
            {
                return false;
            }

            var mousePos = Mouse.GetPosition(this.ColumnChooserGrid.ParentGrid);
            Rect rect;
            if (this.ColumnChooserGrid.TableModel.TableProperties.VisibleColumns.Count == 1)
                rect = this.ColumnChooserGrid.ParentGrid.GetClipRect(ScrollAxisRegion.Header, ScrollAxisRegion.Footer);
            else
                rect = this.ColumnChooserGrid.ParentGrid.GetClipRect(ScrollAxisRegion.Body, ScrollAxisRegion.Body);
            rect = this.ColumnChooserGrid.ParentGrid.RangeToClippedVisibleRect(GridRangeInfo.Table());
            var result = rect.Contains(mousePos);
            return result;
        }

        protected void OpenDragIndicators(Point location, IGridDataDragHeaderHitTestInfo hitTestInfo)
        {
            this.indicatorsShown = true;
            Rect clip = Rect.Empty;
            this.GetArrowIndicatorLocation(ref clip, false, location, hitTestInfo, ref this.isEventHandled);
            if (clip != Rect.Empty)
            {
                this.UpIndicator.Begin();
                this.DownIndicator.Begin();
            }
        }

        private bool isEventHandled = false;
        protected bool IsEventHandled
        {
            get
            {
                return this.isEventHandled;
            }

            set
            {
                this.isEventHandled = value;
            }
        }

        protected void UpdateDragIndicators(Point location, IGridDataDragHeaderHitTestInfo hitTestInfo)
        {
            Rect clip = Rect.Empty;
            this.IsEventHandled = false;
            var pt = this.GetArrowIndicatorLocation(ref clip, true, location, hitTestInfo, ref this.isEventHandled);
            //if (this.isEventHandled)
            //{
            //    this.DragHeaderVisible = false;
            //    this.MoveWindowButCloseIndicators();
            //    return;
            //}

            if (this.Mode == GridDataColumnChooserControllerMode.ColumnDrag)
            {
                if (!this.IsMouseInColumnChooserArea)
                {
                    // we need to identify that the current mouse point is not in the origin column
                    this.DragHeaderVisible = !clip.Contains(hitTestInfo.Point);
                    if (!this.DragHeaderVisible)
                    {
                        this.MoveWindowButCloseIndicators();
                        return;
                    }
                }
                else
                {
                    this.DragHeaderVisible = true;
                }
            }
            else if (this.Mode == GridDataColumnChooserControllerMode.ColumnDrop)
            {
                if (this.IsMouseInColumnChooserArea)
                {
                    this.DragHeaderVisible = !clip.Contains(hitTestInfo.Point);
                    if (!this.DragHeaderVisible)
                    {
                        this.MoveWindowButCloseIndicators();
                        return;
                    }
                    else
                    {
                        this.MoveWindowButCloseIndicators();
                        return;
                    }
                }
                else
                {
                    this.DragHeaderVisible = true;
                }
            }

            if (pt.X >= 0 && pt.Y >= 0)
            {
                if (!this.indicatorsShown)
                {
                    // animations would have been stopped, so restart it here
                    this.UpIndicator.Begin();
                    this.DownIndicator.Begin();
                    this.indicatorsShown = true;
                }
                // adjust X
                pt.X -= 10;
                double y = 0;
                if (this.ColumnChooserGrid.ParentGrid != null)
                {
                    y = this.ColumnChooserGrid.ParentGrid.RowHeights[0];
                }

                if (!this.IsMouseInColumnChooserArea)
                {
                    // set value for up indicator arrow
                    pt.Y += 20;
                }
                else
                {
                    pt.Y += y + 10;
                }
                this.upIndicatorWindow.Move(pt);

                if (!this.IsMouseInColumnChooserArea)
                {
                    // set value for down indicator arrow
                    pt.Y -= 40;
                }
                else
                {
                    // adjust values if we are in group drop area
                    pt.Y -= y + 20;
                }
                this.downIndicatorWindow.Move(pt);
                var windowPoint = this.GetDragWindowLocation();
                this.DragWindow.MoveTo(windowPoint);
            }
            else
            {
                // we are out of bounds so close the indicator popup and stop the animation
                this.CloseDragIndicators();
                Mouse.OverrideCursor = Cursors.No;
            }
        }

        private void MoveWindowButCloseIndicators()
        {
            var windowPoint = this.GetDragWindowLocation();
            this.DragWindow.MoveTo(windowPoint);
            this.CloseDragIndicators();
        }

        private bool indicatorsShown = false;

        protected bool IsMouseInColumnChooserArea
        {
            get;
            private set;
        }

        protected int TargetDragColumnIndex
        {
            get;
            private set;
        }

        protected int TargetDragGroupColumnIndex
        {
            get;
            private set;
        }

        protected FlowDirection MouseFlowDirection
        {
            get;
            private set;
        }

        protected Point GetArrowIndicatorLocation(ref Rect clip, bool raiseEvent, Point location, IGridDataDragHeaderHitTestInfo hitTestInfo, ref bool isEventHandled)
        {
            this.IsMouseInColumnChooserArea = this.IsMouseOverColumnChooserArea();
            if (!this.IsMouseInColumnChooserArea)
            {
                var rowColIndex = this.ColumnChooserGrid.ParentGrid.PointToCellRowColumnIndex(location);
                if (rowColIndex != RowColumnIndex.Empty)
                {
                    this.TargetDragColumnIndex = rowColIndex.ColumnIndex;
                    var headerIndex = this.ColumnChooserGrid.ParentGrid.Model.HeaderRows - 1;
                    Rect r;
                    if (IsMouseOverGrid())
                    {
                        r = this.Grid.RangeToRect(ScrollAxisRegion.Header, ScrollAxisRegion.Body, GridRangeInfo.Cell(headerIndex, this.TargetDragColumnIndex), false, true);
                    }
                    else
                    {
                        r = this.Grid.RangeToRect(ScrollAxisRegion.Header, ScrollAxisRegion.Body, GridRangeInfo.Cell(headerIndex, this.TargetDragColumnIndex + 1), false, true);
                    }
                    clip = r;
                    Point pt = /*this.MouseFlowDirection == FlowDirection.RightToLeft ? r.TopRight : */r.Location;
                    pt = this.ColumnChooserGrid.ParentGrid.PointToScreen(pt);
                    return pt;
                }
            }
            else
            {
                Point columnChooserLocation = Mouse.GetPosition(this.ColumnChooserGrid.Grid);
                var rowColIndex = this.ColumnChooserGrid.Grid.PointToCellRowColumnIndex(columnChooserLocation);
                if (rowColIndex != RowColumnIndex.Empty)
                {
                    Rect r = this.ColumnChooserGrid.Grid.RangeToRect(ScrollAxisRegion.Body, ScrollAxisRegion.Body, GridRangeInfo.Cell(0, rowColIndex.ColumnIndex), false, true);
                    clip = r;
                    Point pt = /*this.ColumnChooserGrid.Grid.FlowDirection == FlowDirection.RightToLeft ? new Point(r.Right, r.Top) : */r.Location;
                    pt = this.ColumnChooserGrid.Grid.PointToScreen(pt);
                    return pt;
                }
            }
            return new Point(0, 0);
        }

        protected void StartDragHeaderPopup(IGridDataDragHeaderHitTestInfo hitTestInfo)
        {
            if (this.DragWindow == null)
            {
                this.DragWindow = new PopupDragWindow();
            }

            var image = this.CreateHeaderImage(hitTestInfo);
            if (image != null)
            {
                this.dragWindow.ProvideImage(image);
                this.dragWindow.StartDrag();
            }
        }

        protected void CloseDragIndicators()
        {
            this.indicatorsShown = false;
            this.UpIndicator.Stop();
            this.DownIndicator.Stop();
            this.upIndicatorWindow.Hide();
            this.downIndicatorWindow.Hide();
        }

        private Point GetDragWindowLocation()
        {
            Point pt = Mouse.GetPosition(this.ColumnChooserGrid.ParentGrid);
            pt = this.ColumnChooserGrid.ParentGrid.PointToScreen(pt);
            pt.Offset(-offSetPoint.X, -offSetPoint.Y);
            //pt.X += 5;
            //pt.Y += 5;
            return pt;
        }

        #region Cell To Image Renderer

        Point offSetPoint;
        protected Image CreateHeaderImage(IGridDataDragHeaderHitTestInfo hitTestInfo)
        {
            if (hitTestInfo == null)
            {
                return null;
            }

            var grid = hitTestInfo.Grid;
            var rowIdx = hitTestInfo.RowColumnIndex.RowIndex;
            var colIdx = hitTestInfo.RowColumnIndex.ColumnIndex;
            var style = grid.Model[rowIdx, colIdx];
            var renderer = grid.CellRenderers[style.CellType];

            var visibleRow = hitTestInfo.VisibleRow;
            var visibleColumn = hitTestInfo.VisibleColumn;
            Rect cellRect = new Rect(0, 0, visibleColumn.Size, visibleRow.Size);
            var renderStyle = grid.GetRenderStyleInfo(hitTestInfo.RowColumnIndex);
            RenderCellArgs rca = new RenderCellArgs(grid, visibleRow, hitTestInfo.VisibleColumn, cellRect, renderStyle);
            offSetPoint = hitTestInfo.Point;
            offSetPoint.Offset(-hitTestInfo.VisibleColumn.Origin, -hitTestInfo.VisibleRow.Origin);
            var visual = new DrawingVisual();
            var dc = visual.RenderOpen();
            dc.DrawRectangle(renderStyle.Background, null, cellRect);
            this.RenderBorder(dc, cellRect, cellRect, CellBorderSide.Top, renderStyle.Borders.Top);
            this.RenderBorder(dc, cellRect, cellRect, CellBorderSide.Left, renderStyle.Borders.Left);
            this.RenderBorder(dc, cellRect, cellRect, CellBorderSide.Right, renderStyle.Borders.Right);
            this.RenderBorder(dc, cellRect, cellRect, CellBorderSide.Bottom, renderStyle.Borders.Bottom);
            if (rca.CellUIElements != null)
            {
                foreach (UIElement el in rca.CellUIElements.UIElements)
                {
                    VisualBrush vb = new VisualBrush(el);
                    Rect r = GridCellTextBoxRenderer.GetBounds(el);
                    r = new Rect(r.X - visibleColumn.Origin, r.Y - visibleRow.Origin, r.Width, r.Height);
                    dc.DrawRectangle(vb, null, r);
                }
            }
            renderer.Render(dc, rca);
            dc.Close();

            RenderTargetBitmap bmp = new RenderTargetBitmap((int)rca.CellRect.Width, (int)rca.CellRect.Height, 96, 96, PixelFormats.Pbgra32);
            bmp.Render(visual);
            return new Image()
            {
                Source = bmp,
                Width = rca.CellRect.Width,
                Height = rca.CellRect.Height
            };
        }

        private void RenderBorder(DrawingContext dc, Rect cellRect, Rect clipRect, CellBorderSide borderSide, Pen pen)
        {
            if (cellRect.Width == 0)
                return;

            //cellRect = clipRect;

            switch (borderSide)
            {
                case CellBorderSide.Top:
                    dc.DrawLine(pen, cellRect.TopLeft, cellRect.TopRight);
                    break;
                case CellBorderSide.Bottom:
                    dc.DrawLine(pen, cellRect.BottomLeft, cellRect.BottomRight);
                    break;
                case CellBorderSide.Left:
                    dc.DrawLine(pen, cellRect.TopLeft, cellRect.BottomLeft);
                    break;
                case CellBorderSide.Right:
                    dc.DrawLine(pen, cellRect.TopRight, cellRect.BottomRight);
                    break;
            }
        }

        #endregion

        #endregion

        #endregion
    }

    //class GridDataColumnDragMouseController : GridDataColumnChooserMouseController
    //{
    //    internal sealed class GridDataColumnDragHeaderHitTestInfo : IGridDataDragHeaderHitTestInfo
    //    {
    //        GridDataColumnDragHeaderHitTestInfo()
    //        {
    //            this.rowColumnIndex = RowColumnIndex.Empty;
    //        }

    //        internal GridDataColumnDragHeaderHitTestInfo(GridControlBase grid, Point point)
    //            : this()
    //        {
    //            this.rowColumnIndex = grid.PointToCellRowColumnIndex(point, true);
    //            if (this.rowColumnIndex != RowColumnIndex.Empty)
    //            {
    //                this.grid = grid;
    //                this.visibleColumn = grid.ScrollColumns.GetVisibleLineAtLineIndex(this.RowColumnIndex.ColumnIndex);
    //                this.visibleRow = grid.ScrollRows.GetVisibleLineAtLineIndex(this.RowColumnIndex.RowIndex);
    //                if (this.VisibleRow != null && this.VisibleRow.LineIndex < grid.Model.HeaderRows)
    //                {
    //                    this.isHeader = true;
    //                    this.point = point;
    //                }
    //            }
    //        }

    //        private GridControlBase grid;
    //        public GridControlBase Grid
    //        {
    //            get
    //            {
    //                return this.grid;
    //            }
    //        }

    //        private bool isHeader = false;
    //        public bool IsHeader
    //        {
    //            get
    //            {
    //                return this.isHeader;
    //            }
    //        }

    //        private Point point;
    //        public Point Point
    //        {
    //            get
    //            {
    //                return this.point;
    //            }
    //        }

    //        private RowColumnIndex rowColumnIndex;
    //        public RowColumnIndex RowColumnIndex
    //        {
    //            get
    //            {
    //                return this.rowColumnIndex;
    //            }
    //        }

    //        private VisibleLineInfo visibleColumn;
    //        public VisibleLineInfo VisibleColumn
    //        {
    //            get
    //            {
    //                return this.visibleColumn;
    //            }
    //        }

    //        private VisibleLineInfo visibleRow;
    //        public VisibleLineInfo VisibleRow
    //        {
    //            get
    //            {
    //                return this.visibleRow;
    //            }
    //        }
    //    }

    //    public const string MouseControllerName = "GridDataColumnChooserMouseController";

    //    public GridDataColumnDragMouseController(GridControlBase grid)
    //        : base(grid)
    //    {
    //        this.Mode = GridDataColumnChooserControllerMode.ColumnDrag;
    //    }

    //    public override string Name
    //    {
    //        get
    //        {
    //            return GridDataColumnDragMouseController.MouseControllerName;
    //        }
    //    }

    //    private IGridDataDragHeaderHitTestInfo hitTestInfo;
    //    public override int HitTest(MouseControllerEventArgs mouseEventArgs, IMouseController controller)
    //    {
    //        if (mouseEventArgs.SourceEventArgs.LeftButton == MouseButtonState.Pressed && mouseEventArgs.ClickCount == 1 && !mouseEventArgs.IsMouseOverChildElement)
    //        {
    //            var point = mouseEventArgs.Location;
    //            this.hitTestInfo = null;
    //            var rowColIndex = this.Grid.PointToCellRowColumnIndex(point);
    //            // when we are near to the corner, Columns resizer will try to get the hittest, then we dont have to drag, 4.0 is the precision used in the Columns resizer.
    //            var headerIndex = this.Grid.Model.HeaderRows - 1;
    //            if (rowColIndex.RowIndex == headerIndex)
    //            {
    //                var cornerHit = this.Grid.ScrollColumns.GetLineNearCorner(point.X, 4.0);
    //                var hit = cornerHit == null ? this.HitTest(rowColIndex.ColumnIndex) : null;
    //                if (hit != null)
    //                {
    //                    this.hitTestInfo = new GridDataColumnDragHeaderHitTestInfo(this.Grid, point);

    //                    if (!this.hitTestInfo.IsHeader)
    //                    {
    //                        this.hitTestInfo = null;
    //                    }
    //                }
    //            }
    //        }

    //        if (this.hitTestInfo != null)
    //        {
    //            this.Mode = GridDataColumnChooserControllerMode.ColumnDrag;
    //            //return 1;
    //        }

    //        return 0;
    //    }

    //    public override void MouseDown(MouseControllerEventArgs e)
    //    {
    //        base.MouseDown(e);
    //        if (this.hitTestInfo == null)
    //        {
    //            return;
    //        }

    //        if (this.hitTestInfo != null)
    //        {
    //            var colIndex = this.TableModel.ResolvePositionToVisibleColumnIndex(this.hitTestInfo.RowColumnIndex.ColumnIndex);
    //            var visibleCol = colIndex > -1 && colIndex < this.TableProperties.VisibleColumns.Count ? this.TableProperties.VisibleColumns[colIndex] : null;
    //            this.ColumnChooserGrid.DragColIndex = colIndex;
    //        }
    //        this.WasDragged = false;
    //        this.IsEventHandled = false;
    //        this.StartDragHeaderPopup(this.hitTestInfo);
    //        this.CloseDragIndicators();
    //    }

    //    public override void MouseMove(MouseControllerEventArgs e)
    //    {
    //        base.MouseMove(e);
    //        if (this.DragWindow != null && this.DragWindow.IsDragging)
    //        {
    //            this.UpdateDragIndicators(e.Location, this.hitTestInfo);
    //            if (IsMouseInColumnChooserArea)
    //                this.CloseDragIndicators();
    //            this.WasDragged |= this.DragHeaderVisible;
    //            //this.ColumnChooserGrid.draggingwindow = this.DragWindow.IsDragging;
    //        }
    //        if (this.IsMouseOverGroupDropArea())
    //        {
    //            //this.hitTestInfo = null;                
    //            var controller = this.ColumnChooserGrid.ParentGrid.MouseControllerDispatcher.Find("GridDataGroupDropAreaMouseController") as GridDataGroupDragMouseController;
    //            //this.ColumnChooserGrid.ParentGrid.MouseControllerDispatcher.ActiveController = controller;
    //        }
    //    }

    //    public override void MouseHoverEnter(MouseEventArgs e)
    //    {
    //        base.MouseHoverEnter(e);
    //    }

    //    public override void MouseUp(MouseControllerEventArgs e)
    //    {
    //        base.MouseUp(e);

    //        if (this.hitTestInfo == null)
    //        {
    //            return;
    //        }

    //        if (this.IsEventHandled || e.ClickCount == 2)
    //        {
    //            this.Close();
    //            return;
    //        }
    //        if (!this.WasDragged)
    //        {
    //            this.Grid.RaiseGridCellClick(this.hitTestInfo.RowColumnIndex.RowIndex, this.hitTestInfo.RowColumnIndex.ColumnIndex);
    //        }
    //        this.Close();
    //    }

    //    private void Close()
    //    {
    //        if (this.DragWindow != null && this.DragWindow.IsDragging)
    //        {
    //            this.DragWindow.StopDrag();
    //            this.CloseDragIndicators();
    //        }

    //        this.DragWindow = null;
    //        this.hitTestInfo = null;
    //    }
    //}

    class GridDataColumnDropMouseController : GridDataColumnChooserMouseController
    {
        internal sealed class GridDataColumnDropHeaderHitTestInfo : IGridDataDragHeaderHitTestInfo
        {
            GridDataColumnDropHeaderHitTestInfo()
            {
                this.rowColumnIndex = RowColumnIndex.Empty;
            }

            internal GridDataColumnDropHeaderHitTestInfo(GridControlBase grid, Point point)
                : this()
            {
                this.rowColumnIndex = grid.PointToCellRowColumnIndex(point, true);
                if (this.rowColumnIndex != RowColumnIndex.Empty)
                {
                    this.grid = grid;
                    this.visibleColumn = grid.ScrollColumns.GetVisibleLineAtLineIndex(this.RowColumnIndex.ColumnIndex);
                    this.visibleRow = grid.ScrollRows.GetVisibleLineAtLineIndex(this.RowColumnIndex.RowIndex);
                    if (this.VisibleRow != null && !this.VisibleRow.IsHeader)
                    {
                        this.isHeader = false;
                        this.point = point;
                    }
                }
            }

            private GridControlBase grid;
            public GridControlBase Grid
            {
                get
                {
                    return this.grid;
                }
            }

            private bool isHeader = false;
            public bool IsHeader
            {
                get
                {
                    return this.isHeader;
                }
            }

            private Point point;
            public Point Point
            {
                get
                {
                    return this.point;
                }
            }

            private RowColumnIndex rowColumnIndex;
            public RowColumnIndex RowColumnIndex
            {
                get
                {
                    return this.rowColumnIndex;
                }
            }

            private VisibleLineInfo visibleColumn;
            public VisibleLineInfo VisibleColumn
            {
                get
                {
                    return this.visibleColumn;
                }
            }

            private VisibleLineInfo visibleRow;
            public VisibleLineInfo VisibleRow
            {
                get
                {
                    return this.visibleRow;
                }
            }
        }

        public const string MouseControllerName = "GridDataColumnDropMouseController";

        public GridDataColumnDropMouseController(GridControlBase grid)
            : base(grid)
        {
            this.Mode = GridDataColumnChooserControllerMode.ColumnDrop;
        }

        public override string Name
        {
            get
            {
                return GridDataColumnDropMouseController.MouseControllerName;
            }
        }

        private IGridDataDragHeaderHitTestInfo hitTestInfo = null;
        public override int HitTest(MouseControllerEventArgs mouseEventArgs, IMouseController controller)
        {
            var point = mouseEventArgs.Location;
            this.hitTestInfo = null;
            var rowColIndex = this.ColumnChooserGrid.Grid.PointToCellRowColumnIndex(point);
            var hit = this.HitTest(rowColIndex.ColumnIndex);
            if (hit != null)
            {
                this.hitTestInfo = new GridDataColumnDropHeaderHitTestInfo(this.ColumnChooserGrid.Grid, point);
            }
            if (this.hitTestInfo != null)
            {
                return 1;
            }

            return 0;
        }

        public override void MouseHoverEnter(MouseEventArgs e)
        {
            this.ColumnChooserGrid.Draggingwindow = this.TableProperties.CanDropOnColumnChooser;
            this.ColumnChooserGrid.MappingName = this.TableProperties.DraggingColumnName;
            this.ColumnChooserGrid.DragColIndex = this.TableProperties.DragColumnIndex;
            base.MouseHoverEnter(e);
            // consider as mouse up when using grid in a window            
            this.CloseDragIndicators();
            if (this.ColumnChooserGrid.Draggingwindow && this.ColumnChooserGrid.MappingName != string.Empty && this.ColumnChooserGrid.MappingName.Length >= 1)
            {
                var rowcolindex = this.ColumnChooserGrid.Grid.PointToCellRowColumnIndex(e.GetPosition(this.ColumnChooserGrid.Grid));
                if (this.ColumnChooserGrid.DragColIndex >= 0)
                {
                    var groupedColumn = this.TableProperties.GroupedColumns.FirstOrDefault(o => o.ColumnName == this.ColumnChooserGrid.MappingName);
                    if (groupedColumn == null)
                    {
                        var column = this.TableProperties.VisibleColumns.FirstOrDefault(v => v.MappingName == this.ColumnChooserGrid.MappingName);
                        if (column != null && column.MappingName != null)
                        {
                            if ((this.ColumnChooserGrid.newColumnList.Count - 1) == rowcolindex.RowIndex)
                                this.ColumnChooserGrid.newColumnList.Insert(rowcolindex.RowIndex + 1, column);
                            else
                                this.ColumnChooserGrid.newColumnList.Insert(rowcolindex.RowIndex, column);
                            this.ColumnChooserGrid.DragColIndex = -1;
                            this.TableProperties.VisibleColumns.Remove(column);
                            column = null;
                        }
                    }
                    this.TableModel.InvalidateDisplay();
                }
                this.ColumnChooserGrid.Draggingwindow = false;
            }
        }
        public int colindex = -1;

        public override void MouseDown(MouseControllerEventArgs e)
        {
            base.MouseDown(e);

            if (this.hitTestInfo == null || this.TableModel == null)
            {
                return;
            }
            var point = e.Location;
            var rowcolidex = this.ColumnChooserGrid.Grid.PointToCellRowColumnIndex(point);
            colindex = rowcolidex.RowIndex;
            if (this.ColumnChooserGrid.newColumnList.Count >= 0)
            {
                this.WasDragged = false;
                this.StartDragHeaderPopup(this.hitTestInfo);
                this.OpenDragIndicators(e.Location, this.hitTestInfo);
            }
            else
            {
                this.hitTestInfo = null;
            }
        }

        bool candrop = false;

        public override void MouseMove(MouseControllerEventArgs e)
        {
            base.MouseMove(e);

            if (this.hitTestInfo == null)
            {
                return;
            }
            if (this.DragWindow != null && this.DragWindow.IsDragging)
            {
                this.UpdateDragIndicators(Mouse.GetPosition(this.ColumnChooserGrid.ParentGrid), this.hitTestInfo);
                this.WasDragged |= this.DragHeaderVisible;
                candrop = true;
            }
        }

        public override void MouseUp(MouseControllerEventArgs e)
        {
            base.MouseUp(e);

            if (this.hitTestInfo == null)
            {
                return;
            }
            if (!IsMouseOverColumnChooserArea() && !this.ColumnChooserGrid.Grid.IsMouseOver)
            {
                if (IsMouseOverGrid() && candrop)
                {

                    if (colindex >= 0 && this.ColumnChooserGrid.newColumnList.Count >= 1)
                    {
                        var column = this.ColumnChooserGrid.newColumnList.ElementAt(colindex);
                        if (column != null)
                        {
                            var index = this.ColumnChooserGrid.ParentGrid.PointToCellRowColumnIndex(Mouse.GetPosition(this.ColumnChooserGrid.ParentGrid));
                            this.TableProperties.VisibleColumns.Add(column);
                            var visibleColIndex = this.TableProperties.VisibleColumns.IndexOf(column);
                            var actualGridColIndex = this.TableModel.ResolveVisibleColumnIndexToPosition(visibleColIndex);
                            var toGridColIndex = index.ColumnIndex;
                            if (actualGridColIndex > -1 && toGridColIndex > -1)
                            {
                                this.TableModel.IsInSourceListChanged = true;
                                if (this.TargetDragColumnIndex == toGridColIndex)
                                    this.ColumnChooserGrid.ParentGrid.Model.MoveColumns(actualGridColIndex, 1, this.TargetDragColumnIndex);
                                else
                                    this.ColumnChooserGrid.ParentGrid.Model.MoveColumns(actualGridColIndex, 1, this.TargetDragColumnIndex + 1);

                                this.TableModel.IsInSourceListChanged = false;
                            }
                            this.TableProperties.DragColumnIndex = -1;
                            this.ColumnChooserGrid.newColumnList.Remove(column);
                        }
                    }

                }
                else
                {
                    //if (this.ColumnChooserGrid.TableModel.TableProperties.VisibleColumns.Count == 1 || this.ColumnChooserGrid.TableModel.TableProperties.VisibleColumns.Count == 0)
                    if (this.ColumnChooserGrid.TableModel.TableProperties.VisibleColumns.Count <= (this.Grid.Model.ColumnCount + ColumnChooserGrid.newColumnList.Count))
                    {
                        var column = this.ColumnChooserGrid.newColumnList.ElementAt(colindex);
                        if (column != null)
                        {
                            var index = this.ColumnChooserGrid.ParentGrid.PointToCellRowColumnIndex(Mouse.GetPosition(this.ColumnChooserGrid.ParentGrid));
                            this.TableProperties.VisibleColumns.Add(column);
                            var visibleColIndex = this.TableProperties.VisibleColumns.IndexOf(column);
                            var actualGridColIndex = this.TableModel.ResolveVisibleColumnIndexToPosition(visibleColIndex);
                            var toGridColIndex = index.ColumnIndex;
                            if (actualGridColIndex > -1 && toGridColIndex > -1)
                            {
                                this.TableModel.IsInSourceListChanged = true;
                                if (this.TargetDragColumnIndex == toGridColIndex)
                                    this.ColumnChooserGrid.ParentGrid.Model.MoveColumns(actualGridColIndex, 1, this.TargetDragColumnIndex + 1);
                                else
                                    this.ColumnChooserGrid.ParentGrid.Model.MoveColumns(actualGridColIndex, 1, this.TargetDragColumnIndex);

                                this.TableModel.IsInSourceListChanged = false;
                            }
                            this.TableProperties.DragColumnIndex = -1;
                            this.ColumnChooserGrid.newColumnList.Remove(column);
                        }
                    }
                }
            }
            this.TableModel.InvalidateDisplay();
            this.TableModel.InvalidateVisual();
            if (this.DragWindow != null && this.DragWindow.IsDragging)
            {
                this.DragWindow.StopDrag();
                this.CloseDragIndicators();
            }

            this.DragWindow = null;
            this.hitTestInfo = null;

        }
    }
}