#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Grid
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Interop;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Media.Imaging;
    using Syncfusion.Windows.Controls.Cells;
    using Syncfusion.Windows.Controls.Scroll;
    using Syncfusion.Windows.GridCommon;
    using Syncfusion.Windows.Shared;
    using Syncfusion.Windows.ComponentModel;
    using System.Windows.Data;

    /// <summary>
    /// Implements a mouse controller for column drag operation.
    /// </summary>
    class GridDragColumnHeaderMouseController : IMouseController, IDisposable
    {
        internal sealed class GridDragHeaderHitTestInfo
        {
            internal bool isHeader = false;
            internal Point point;
            internal RowColumnIndex rowColIndex;
            internal VisibleLineInfo visibleColumn;
            internal VisibleLineInfo visibleRow;

            internal GridDragHeaderHitTestInfo(GridControlBase grid, Point point)
            {
                if (grid.PointToCellRowColumnIndex(point, true) != RowColumnIndex.Empty)
                {
                    this.rowColIndex = grid.PointToCellRowColumnIndex(point, true);
                    this.visibleColumn = grid.ScrollColumns.GetVisibleLineAtLineIndex(this.rowColIndex.ColumnIndex);
                    this.visibleRow = grid.ScrollRows.GetVisibleLineAtLineIndex(this.rowColIndex.RowIndex);
                    if (this.visibleRow != null && this.visibleRow.IsHeader && grid.InternalGetHeaderRows() > 0)
                    {
                        this.isHeader = true;
                        this.point = point;
                    }
                }
            }
        }

        private PopupDragWindow dragWindow;
        private PopupPositionWindow upIndicatorWindow;
        private PopupPositionWindow downIndicatorWindow;

        /// <summary>
        /// Initializes a new <see cref="GridDragColumnHeaderMouseController"/>
        /// </summary>
        /// <param name="grid">The grid.</param>
        public GridDragColumnHeaderMouseController(GridControlBase grid)
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
        }

        /// <summary>
        /// The base grid.
        /// </summary>
        public GridControlBase Grid
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the Up Drag Indicator.
        /// </summary>
        public AnimatedGrid UpIndicator
        {
            get
            {
                return this.upIndicatorWindow.Child as AnimatedGrid;
            }
        }

        /// <summary>
        /// Gets the Down Drag Indicator.
        /// </summary>
        public AnimatedGrid DownIndicator
        {
            get
            {
                return this.downIndicatorWindow.Child as AnimatedGrid;
            }
        }

        /// <summary>
        /// When true, makes the drag header visible.
        /// </summary>
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

        #region IMouseController Members
        public const string GridDragColumnHeaderMouseControllerName = "GridDragColumnHeaderMouseController";

        /// <summary>
        /// Returns the name of the mouse controller.
        /// </summary>
        public string Name
        {
            get { return GridDragColumnHeaderMouseController.GridDragColumnHeaderMouseControllerName; }
        }

        private Cursor _cursor;

        /// <summary>
        /// Returns the cell cursor.
        /// </summary>
        /// 
        public Cursor Cursor
        {
            get
            {
                Cursor Currentcursor = this.Grid.RaiseGridCellCursor();
                if (Currentcursor == Cursors.Arrow)
                {
                    return _cursor;
                }
                else
                {
                    return Currentcursor;
                }
            }
            set
            {
                _cursor = value;
            }
        }


        protected FlowDirection MouseFlowDirection
        {
            get;
            private set;
        }

        private Point mouseDownPoint;




        /// <summary>
        /// MouseHoverEnter is called when this controller signaled in HitTest that it wants to handle mouse events. MouseHoverEnter
        /// is called before the first time MouseHover is called.
        /// </summary>
        /// <param name="e">The <see cref="MouseEventArgs"/> that contains the event data.</param>
        public void MouseHoverEnter(System.Windows.Input.MouseEventArgs e)
        {
        }

        /// <summary>
        /// MouseHover is called when this controller signaled in HitTest that it wants to handle mouse events. MouseHover
        /// is called after MouseHoverEnter.
        /// </summary>
        /// <param name="e">The <see cref="MouseControllerEventArgs"/> that contains the event data.</param>
        public void MouseHover(MouseControllerEventArgs e)
        {
        }

        /// <summary>
        /// MouseHoverLeave is called when hovering ends either because user dragged mouse out of the hit-test area or
        /// when context changes (e.g. user pressed the mouse button).
        /// </summary>
        /// <param name="e">The <see cref="MouseEventArgs"/> that contains the event data.</param>
        public void MouseHoverLeave(System.Windows.Input.MouseEventArgs e)
        {
        }

        /// <summary>
        /// MouseDown is called when this controller signaled in HitTest that it wants to handle mouse events and the
        /// user pressed the mouse button.
        /// </summary>
        /// <param name="e">The <see cref="MouseControllerEventArgs"/> that contains the event data.</param>
        public void MouseDown(MouseControllerEventArgs e)
        {
            if (this.hitTestInfo == null)
            {
                return;
            }
            this.mouseDownPoint = e.Location;
            this.WasDragged = false;
            // init the drag providers
            // start drag header popup
            this.StartDragHeaderPopup();
            // show arrow indicators
            this.OpenDragIndicators(e.Location);

            this.Grid.AutoScroller.AutoScrollBounds = this.Grid.GetClipRect(ScrollAxisRegion.Header, ScrollAxisRegion.Footer);
            if (this.Grid.AutoScroller.InsideScrollBounds.Left <= e.Location.X || this.Grid.AutoScroller.InsideScrollBounds.Right >= e.Location.X)
            {
                this.Grid.AutoScroller.AutoScrolling = AutoScrollOrientation.Horizontal;
            }
        }

        private void OpenDragIndicators(Point location)
        {
            this.indicatorsShown = true;
            Rect clip = Rect.Empty;
            this.GetArrowIndicatorLocation(ref clip, false, location);
            if (clip != Rect.Empty)
            {
                this.UpIndicator.Begin();
                this.DownIndicator.Begin();
            }
        }
        // This just close the drag indicator and move the header cell anole... This will invoked when the header cell is dragged in inside the column
        private void MoveWindowButCloseIndicators()
        {
            var windowPoint = this.GetDragWindowLocation();
            this.dragWindow.MoveTo(windowPoint);
            this.CloseDragIndicators();
        }
        private void UpdateDragIndicators(Point location)
        {
            Rect clip = Rect.Empty;
            var pt = this.GetArrowIndicatorLocation(ref clip, true, location);
            // we need to identify that the current mouse point is not in the origin column
            this.DragHeaderVisible = !clip.Contains(this.hitTestInfo.point);
            GridTreeControlImpl gridTree = this.Grid as GridTreeControlImpl;
            if (gridTree != null)
                if (this.MouseFlowDirection == FlowDirection.LeftToRight && this.hitTestInfo.rowColIndex.ColumnIndex + 1 == this.targetColIndex)
                    this.DragHeaderVisible = false;
            if (!this.DragHeaderVisible)
            {
                this.MoveWindowButCloseIndicators();
                return;
            }

            if (pt.X > 0 && pt.Y > 0)
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
                // set value for up indicator arrow
                pt.Y += 20;
                this.upIndicatorWindow.Move(pt);
                // set value for down indicator arrow
                pt.Y -= 40;
                this.downIndicatorWindow.Move(pt);
            }

            var windowPoint = this.GetDragWindowLocation();
            this.dragWindow.MoveTo(windowPoint);

            //Code commented to allow drag column over 1st column too
            //else
            //{
            //    // we are out of bounds so close the indicator popup and stop the animation
            //    this.CloseDragIndicators();
            //}
        }

        /// <summary>
        /// MouseMove is called for the active controller after a MouseDown message when the user moves the mouse pointer.
        /// </summary>
        /// <param name="e">The <see cref="MouseControllerEventArgs"/> that holds the event data.</param>
        public void MouseMove(MouseControllerEventArgs e)
        {
            // Sets the mouse move direction from the mouse down point. 
            if (e.Location.X <= this.mouseDownPoint.X)
            {
                this.MouseFlowDirection = FlowDirection.RightToLeft;
            }
            else
            {
                this.MouseFlowDirection = FlowDirection.LeftToRight;
            }

            if (this.dragWindow.IsDragging)
            {
                this.UpdateDragIndicators(e.Location);
                this.WasDragged |= this.DragHeaderVisible;

                this.Grid.AutoScroller.AutoScrollBounds = this.Grid.GetClipRect(ScrollAxisRegion.Body, ScrollAxisRegion.Body);
                if (this.Grid.AutoScroller.InsideScrollBounds.Left <= e.Location.X || this.Grid.AutoScroller.InsideScrollBounds.Right >= e.Location.X)
                {
                    this.Grid.AutoScroller.AutoScrolling = AutoScrollOrientation.Horizontal;
                }
            }
        }

        /// <summary>
        /// MouseUp is called for the active controller after a MouseDown message when the user releases the mouse button.
        /// </summary>
        /// <param name="e">The <see cref="MouseControllerEventArgs"/> that holds the event data.</param>
        public void MouseUp(MouseControllerEventArgs e)
        {
            GridTreeControlImpl gridTree = this.Grid as GridTreeControlImpl;
            if (this.hitTestInfo == null)
            {
                return;
            }
            //var rowColIndex = this.Grid.PointToCellRowColumnIndex(e.Location);
            //if (rowColIndex != RowColumnIndex.Empty)
            //{
            //    if (rowColIndex.ColumnIndex > this.hitTestInfo.rowColIndex.ColumnIndex)
            //    {
            //        this.targetColIndex = rowColIndex.ColumnIndex - 1;
            //    }
            //}

            if (!this.WasDragged)
            {
                if (this.dragWindow.IsDragging)
                {
                    this.dragWindow.StopDrag();
                    this.CloseDragIndicators();
                }
                this.Grid.RaiseGridCellClick(this.hitTestInfo.rowColIndex.RowIndex, this.hitTestInfo.rowColIndex.ColumnIndex);
            }
            else
            {
                if (this.dragWindow.IsDragging)
                {
                    this.dragWindow.StopDrag();
                    this.CloseDragIndicators();
                }

                // raise allow drag events
                if (this.Grid.RaiseQueryAllowDragColumn(this.hitTestInfo.rowColIndex.ColumnIndex, gridTree != null ? this.targetColIndex : this.targetColIndex, GridQueryDragColumnHeaderReason.MouseUp))
                {

                    if (this.hitTestInfo.rowColIndex.ColumnIndex != this.targetColIndex)
                        this.Grid.Model.MoveColumns(this.hitTestInfo.rowColIndex.ColumnIndex, 1, gridTree != null ? this.targetColIndex : this.targetColIndex);
                    //When we drag and drop the header column, the header styles are not applied correctly. So here we need to invalidate the header column
                    if (this.targetColIndex < this.Grid.Model.HeaderColumns || this.hitTestInfo.rowColIndex.ColumnIndex < this.Grid.Model.HeaderColumns)
                    {
                        this.Grid.InvalidateCell(GridRangeInfo.Col(this.targetColIndex));
                        if (this.hitTestInfo.rowColIndex.ColumnIndex < this.Grid.Model.HeaderColumns)
                            this.Grid.InvalidateCell(GridRangeInfo.Col(this.hitTestInfo.rowColIndex.ColumnIndex));
                        if (this.targetColIndex < this.Grid.Model.HeaderColumns)
                            this.Grid.InvalidateCell(GridRangeInfo.Col(this.targetColIndex + 1));
                    }
                }
            }
            this.dragWindow = null;
            this.hitTestInfo = null;
            this.Grid.AutoScroller.AutoScrolling = AutoScrollOrientation.None;
        }

        /// <summary>
        /// Cancels the drag operation.
        /// </summary>
        public void CancelMode()
        {
            if (this.hitTestInfo == null)
            {
                return;
            }

            if (this.dragWindow.IsDragging)
            {
                this.dragWindow.StopDrag();
                this.CloseDragIndicators();
                this.dragWindow = null;
                this.hitTestInfo = null;
            }
        }

        /// <summary>
        /// Restores the drag action.
        /// </summary>
        public void RestoreMode()
        {
        }

        private VisibleLineInfo HitTest(int colIndex)
        {
            return this.Grid.ScrollColumns.GetVisibleLineAtLineIndex(colIndex);
        }

        private GridDragHeaderHitTestInfo hitTestInfo;
        /// <summary>
        /// HitTest is called to determine whether this controller wants to handle the mouse events.
        /// It also raises <see cref="GridControlBase.QueryAllowDragColum"/> event if the mouse hits
        /// over a column header.
        /// </summary>
        /// <param name="mouseEventArgs">A <see cref="MouseControllerEventArgs"/> holding event data.</param>
        /// <param name="controller">A <see cref="IMouseController"/> that has indicated to handle the mouse event.</param>
        /// <returns>A non-zero value if the grid can and wants to handle the mouse event; 0 if the
        /// mouse event is unrelated for this grid.</returns>
        public int HitTest(MouseControllerEventArgs mouseEventArgs, IMouseController controller)
        {
            if (mouseEventArgs.SourceEventArgs.LeftButton == MouseButtonState.Pressed && mouseEventArgs.ClickCount == 1 && !mouseEventArgs.IsMouseOverChildElement)
            {
                var point = mouseEventArgs.Location;
                this.hitTestInfo = null;
                var rowColIndex = this.Grid.PointToCellRowColumnIndex(point);

                //This code added to select the Frozen Rows
                if (rowColIndex.RowIndex >= this.Grid.Model.HeaderRows)
                    return 0;

                // when we are near to the corner, Columns resizer will try to get the hittest, then we dont have to drag, 4.0 is the precision used in the Columns resizer.
                var cornerHit = this.Grid.ScrollColumns.GetLineNearCorner(point.X, 4.0);
                var hit = cornerHit == null ? this.HitTest(rowColIndex.ColumnIndex) : null;
                if (hit != null)
                {
                    this.hitTestInfo = new GridDragHeaderHitTestInfo(this.Grid, point);
                    if (!this.hitTestInfo.isHeader)
                    {
                        this.hitTestInfo = null;
                    }
                    else
                    {


                        if (!this.Grid.RaiseQueryAllowDragColumn(this.hitTestInfo.rowColIndex.ColumnIndex, -1, GridQueryDragColumnHeaderReason.HitTest))
                        {
                            this.hitTestInfo = null;
                        }
                    }
                }
            }

            if (this.hitTestInfo != null)
            {
                return 1;
            }

            return 0;
        }

        /// <summary>
        /// Allows to cancel the mouse capture.
        /// </summary>
        public bool SupportsCancelMouseCapture
        {
            get { return false; }
        }

        /// <summary>
        /// Allows to track the mouse.
        /// </summary>
        public bool SupportsMouseTracking
        {
            get { return false; }
        }

        #region Helper methods
        private bool indicatorsShown = false;
        private int targetColIndex = -1;
        private Point GetArrowIndicatorLocation(ref Rect clip, bool raiseEvent, Point location)
        {
            //var target = this.Grid.ScrollColumns.GetVisibleLineAtPoint(Mouse.GetPosition(this.Grid).X);
            //if (target != null)
            //{
            //    this.targetColIndex = target.LineIndex;
            GridTreeControlImpl gridTree = this.Grid as GridTreeControlImpl;
            var rowColIndex = this.Grid.PointToCellRowColumnIndex(location);
            if (rowColIndex != RowColumnIndex.Empty)
            {
                this.targetColIndex = rowColIndex.ColumnIndex;
                if (raiseEvent)
                {

                    if (!this.Grid.RaiseQueryAllowDragColumn(this.hitTestInfo.rowColIndex.ColumnIndex, this.targetColIndex, GridQueryDragColumnHeaderReason.MouseMove))
                    {
                        clip = Rect.Empty;
                        return new Point(0, 0);
                    }
                }
                // In Grid Tree Control the column  drag indication should not should not show in the right side of Same column header when moved to Left to Right
                if (gridTree != null && this.MouseFlowDirection == FlowDirection.LeftToRight)
                    this.targetColIndex += 1;
                Rect r = this.Grid.RangeToRect(ScrollAxisRegion.Header, ScrollAxisRegion.Body, GridRangeInfo.Cell(0, this.targetColIndex), false, true);
                clip = r;
                Point pt = this.Grid.FlowDirection == FlowDirection.RightToLeft ? new Point(r.Right, r.Top) : r.Location;
                pt = this.Grid.PointToScreen(pt);
                return pt;
            }

            return new Point(0, 0);
        }

        private void StartDragHeaderPopup()
        {
            if (this.dragWindow == null)
            {
                this.dragWindow = new PopupDragWindow();
            }
            var image = this.CreateHeaderImage();
            this.dragWindow.ProvideImage(image);
            //var pt = this.GetDragWindowLocation();
            this.dragWindow.StartDrag();
        }

        private void CloseDragIndicators()
        {
            this.indicatorsShown = false;
            this.UpIndicator.Stop();
            this.DownIndicator.Stop();
            this.upIndicatorWindow.Hide();
            this.downIndicatorWindow.Hide();
        }

        Point offSetPoint;
        private Image CreateHeaderImage()
        {
            if (this.hitTestInfo == null)
            {
                return null;
            }

            var rowIdx = this.hitTestInfo.rowColIndex.RowIndex;
            var colIdx = this.hitTestInfo.rowColIndex.ColumnIndex;
            var style = this.Grid.Model[rowIdx, colIdx];
            var renderer = this.Grid.CellRenderers[style.CellType];

            var visibleRow = this.hitTestInfo.visibleRow;
            var visibleColumn = this.hitTestInfo.visibleColumn;

            if (visibleColumn == null)
            {
                return new Image();
            }

            Rect cellRect = new Rect(0, 0, visibleColumn.Size, visibleRow.Size);
            var renderStyle = this.Grid.GetRenderStyleInfo(this.hitTestInfo.rowColIndex);
            RenderCellArgs rca = new RenderCellArgs(this.Grid, visibleRow, this.hitTestInfo.visibleColumn, cellRect, renderStyle);
            offSetPoint = hitTestInfo.point;
            offSetPoint.Offset(-hitTestInfo.visibleColumn.Origin, -hitTestInfo.visibleRow.Origin);
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

            RenderTargetBitmap bmp = new RenderTargetBitmap((int)rca.CellRect.Width, (int)rca.CellRect.Height, 96, 96, PixelFormats.Default);
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

        private Point GetDragWindowLocation()
        {
            Point pt = Mouse.GetPosition(this.Grid);
            pt = this.Grid.PointToScreen(pt);
            pt.Offset(-offSetPoint.X, -offSetPoint.Y);
            //  pt.X += 5;
            //  pt.Y += 5;
            return pt;
        }

        #endregion

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (this.dragWindow != null)
            {
                this.dragWindow.Dispose();
                this.dragWindow = null;
            }
        }

        #endregion
    }

    /// <summary>
    /// Defines the direction of the drag indicator.
    /// </summary>
    public enum Direction
    {
        Up,
        Down
    }

    class AnimatedGrid : Grid, IDisposable
    {
        public AnimatedGrid()
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                this.InnerBrush = GridUtil.GetXamlConvertedValue<Brush>("#FFFFFFFF");
                this.OuterBrush = GridUtil.GetXamlConvertedValue<Brush>("#FF000000");
            }
            this.Direction = Direction.Up;
        }

        private Brush outerBrush;
        public Brush OuterBrush
        {
            get
            {
                return this.outerBrush;
            }

            set
            {
                if (value != null && this.outerBrush != value)
                {
                    this.outerBrush = value;
                    if (this.border != null)
                    {
                        // refresh
                        this.border.Background = this.GetBrush();
                    }
                }
            }
        }

        private Brush innerBrush;
        public Brush InnerBrush
        {
            get
            {
                return this.innerBrush;
            }

            set
            {
                if (value != null && this.innerBrush != value)
                {
                    this.innerBrush = value;
                    if (this.border != null)
                    {
                        // refresh
                        this.border.Background = this.GetBrush();
                    }
                }
            }
        }

        public Direction Direction
        {
            get;
            set;
        }

        private Brush GetBrush()
        {
            Brush arrowBrush = null;
            if (this.Direction == Direction.Up)
            {
                arrowBrush = new DrawingBrush()
                {
                    Drawing = new DrawingGroup()
                    {
                        Children = new DrawingCollection()
                    {
                        new GeometryDrawing()
                        {
                            Brush = this.OuterBrush,
                            Geometry = GridUtil.GetXamlConvertedValue<Geometry>("F1 M 411.741,305.165L 407.204,300.027L 402.667,294.889L 398.129,300.027L 393.592,305.165L 398,305.165L 398,310.444L 406.667,310.444L 406.667,305.165L 411.741,305.165 Z ")
                        },
                        new GeometryDrawing()
                        {
                            Brush = this.InnerBrush,
                            Geometry = GridUtil.GetXamlConvertedValue<Geometry>("F1 M 405.332,309.112L 399.335,309.112L 399.335,305.164L 399.335,303.83L 398.001,303.83L 396.547,303.83L 399.126,300.909L 402.667,296.903L 406.204,300.909L 408.784,303.83L 406.665,303.83L 405.332,303.83L 405.332,305.164L 405.332,309.112 Z ")
                        }
                    }
                    }
                };
            }
            else if (this.Direction == Direction.Down)
            {
                arrowBrush = new DrawingBrush()
                {
                    Drawing = new DrawingGroup()
                    {
                        Children = new DrawingCollection()
                    {
                        new GeometryDrawing()
                        {
                            Brush = this.OuterBrush,
                            Geometry = GridUtil.GetXamlConvertedValue<Geometry>("F1 M 411.741,300.168L 407.204,305.306L 402.667,310.444L 398.129,305.306L 393.592,300.168L 398,300.168L 398,294.889L 406.667,294.889L 406.667,300.168L 411.741,300.168 Z ")
                        },
                        new GeometryDrawing()
                        {
                            Brush = this.InnerBrush,
                            Geometry = GridUtil.GetXamlConvertedValue<Geometry>("F1 M 405.332,296.222L 399.335,296.222L 399.335,300.169L 399.335,301.503L 398.001,301.503L 396.547,301.503L 399.126,304.424L 402.667,308.43L 406.204,304.424L 408.784,301.503L 406.665,301.503L 405.332,301.503L 405.332,300.169L 405.332,296.222 Z ")
                        }
                    }
                    }
                };
            }
            return arrowBrush;
        }

        private Border border = null;

        private Storyboard storyBoard = null;

        private void CreateStoryBoard()
        {
            var grid = new Grid()
            {
                Width = 12,
                Height = 12
            };
            border = new Border()
            {
                RenderTransform = new TransformGroup()
                {
                    Children = new TransformCollection()
                    {
                        new ScaleTransform()
                        {
                            ScaleX = 1,
                            ScaleY = 1
                        },
                        new SkewTransform()
                        {
                            AngleX = 0,
                            AngleY = 0
                        },
                        new RotateTransform()
                        {
                            Angle = 0
                        },
                        new TranslateTransform()
                        {
                            X = 0,
                            Y = 0
                        }
                    }
                }
            };

            border.Background = this.GetBrush();
            grid.Children.Add(border);
            this.Children.Add(grid);
            DoubleAnimationUsingKeyFrames frame1 = null;
            if (this.Direction == Direction.Up)
            {
                frame1 = new DoubleAnimationUsingKeyFrames()
                {
                    BeginTime = GridUtil.GetXamlConvertedValue<TimeSpan>("00:00:00.1"),
                    KeyFrames = new DoubleKeyFrameCollection()
                    {
                        new SplineDoubleKeyFrame()
                        {
                            KeyTime = GridUtil.GetXamlConvertedValue<KeyTime>("00:00:00.5"),
                            Value = -5d
                        },
                        new SplineDoubleKeyFrame()
                        {
                            KeyTime = GridUtil.GetXamlConvertedValue<KeyTime>("00:00:01"),
                            Value = 0
                        }
                    }
                };
                Storyboard.SetTarget(frame1, border);
                Storyboard.SetTargetProperty(frame1, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.Y)"));
            }
            else if (this.Direction == Direction.Down)
            {
                frame1 = new DoubleAnimationUsingKeyFrames()
                {
                    BeginTime = GridUtil.GetXamlConvertedValue<TimeSpan>("00:00:00.1"),
                    KeyFrames = new DoubleKeyFrameCollection()
                    {
                        new SplineDoubleKeyFrame()
                        {
                            KeyTime = GridUtil.GetXamlConvertedValue<KeyTime>("00:00:00.5"),
                            Value = 5d
                        },
                        new SplineDoubleKeyFrame()
                        {
                            KeyTime = GridUtil.GetXamlConvertedValue<KeyTime>("00:00:01"),
                            Value = -5d
                        }
                    }
                };
                Storyboard.SetTarget(frame1, border);
                Storyboard.SetTargetProperty(frame1, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.Y)"));
            }
            storyBoard = new Storyboard()
            {
                RepeatBehavior = RepeatBehavior.Forever,
                AutoReverse = true,
                Children = new TimelineCollection()
                {
                    frame1
                }
            };
        }

        public void Begin()
        {
            if (this.storyBoard == null)
            {
                this.CreateStoryBoard();
            }

            this.storyBoard.Begin();
        }

        public void Stop()
        {
            if (this.storyBoard == null)
            {
                return;
            }

            this.storyBoard.Stop();
        }

        public void Dispose()
        {
            this.innerBrush = null;
            this.outerBrush = null;
            this.border = null;
            this.storyBoard = null;
        }
    }

    /// <summary>
    /// For internal use.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class PopupDragWindow : Popup, IDisposable
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public PopupDragWindow()
        {
            if (!BrowserInteropHelper.IsBrowserHosted)
            {
                this.Placement = PlacementMode.Relative;
            }
            else
            {
                this.Placement = PlacementMode.Mouse;
                this.PlacementTarget = Application.Current.MainWindow;
            }

            this.PlacementRectangle = new Rect(1, 1, 1, 1);
            DependencyPropertyDescriptor propDesc = DependencyPropertyDescriptor.FromProperty(PlacementRectangleProperty, typeof(PopupDragWindow));
            propDesc.AddValueChanged(this, OnPlacementRectangleChanged);
            this.DestroyChildOnStopDrag = true;
        }
        /// <summary>
        /// For internal use.
        /// </summary>
        public bool DestroyChildOnStopDrag
        {
            get;
            set;
        }
        /// <summary>
        /// For internal use.
        /// </summary>
        public void DestroyChild()
        {
            if (this.Child != null)
            {
                var imageSource = this.Child as Image;
                if (imageSource != null)
                {
                    imageSource.Source = null;
                    imageSource.Width = 0;
                    imageSource.Height = 0;
                    imageSource = null;
                }
            }
            this.Child = null;
        }
        /// <summary>
        /// For internal use.
        /// </summary>
        public void ProvideImage(Image image)
        {
            this.Child = image;
        }

        /// <summary>
        /// For internal use.
        /// </summary>
        public void ProvideElement(FrameworkElement element)
        {

            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            this.Child = element;
        }

        /// <summary>
        /// For internal use.
        /// </summary>
        public void ProvideVisual(FrameworkElement visual)
        {
            var width = visual.Width;
            if (DoubleUtil.IsNaN(width))
            {
                width = visual.ActualWidth;
            }

            var height = visual.Height;
            if (DoubleUtil.IsNaN(height))
            {
                height = visual.ActualHeight;
            }

            visual.Measure(new Size(width, height));
            visual.Arrange(new Rect(0, 0, width, height));
            visual.UpdateLayout();

            RenderTargetBitmap bmp = new RenderTargetBitmap((int)width, (int)height, 96, 96, PixelFormats.Default);
            bmp.Render(visual);
            this.Child = new Image()
            {
                Source = bmp,
                Width = width,
                Height = height
            };
        }

        /// <summary>
        /// Updates width and haight of the window using the size from the PlacementRectangle.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPlacementRectangleChanged(object sender, EventArgs e)
        {
            if (!this.PlacementRectangle.IsEmpty)
            {
                this.Width = this.PlacementRectangle.Width;
                this.Height = this.PlacementRectangle.Height;
            }
            else
            {
                this.Width = 150;
                this.Height = 100;
            }
        }
        /// <summary>
        /// For internal use.
        /// </summary>
        public bool IsDragging
        {
            get;
            private set;
        }

        private bool isShown = false;
        /// <summary>
        /// For internal use.
        /// </summary>
        public bool IsShowing
        {
            get
            {
                return this.isShown;
            }
        }
        /// <summary>
        /// For internal use.
        /// </summary>
        public void Show()
        {
            if (!this.isShown)
            {
                this.Visibility = Visibility.Visible;
                this.IsOpen = true;
                this.isShown = true;
            }
        }
        /// <summary>
        /// For internal use.
        /// </summary>
        public bool StartDrag()
        {
            if (this.Child == null)
            {
                return false;
            }
            else
            {
                this.StopDrag();
            }

            //this.Visibility = Visibility.Visible;
            this.IsDragging = true;
            //this.IsOpen = true;
            //this.Move(p);
            return true;
        }

        private void Move(Point p)
        {
            this.Show();
            this.PlacementRectangle = new Rect(p.X, p.Y, this.Width, this.Height);
        }

        /// <summary>
        /// For internal use.
        /// </summary>
        public bool MoveTo(Point p)
        {
            if (!this.IsDragging)
            {
                return false;
            }

            this.Move(p);
            return true;
        }
        /// <summary>
        /// For internal use.
        /// </summary>
        public bool StopDrag()
        {
            if (this.IsDragging)
            {
                this.Hide();
                this.Visibility = Visibility.Hidden;
                if (this.DestroyChildOnStopDrag)
                {
                    this.DestroyChild();
                }
                return true;
            }
            return false;
        }
        /// <summary>
        /// For internal use.
        /// </summary>
        public void Hide()
        {
            this.IsOpen = false;
            this.IsDragging = false;
            this.isShown = false;
        }

        private void SetWindowOnTopSecure()
        {
            if (!PermissionHelper.HasUnmanagedCodePermission)
            {
                return;
            }

            HwndSource src = (HwndSource)PresentationSource.FromVisual(Child);

            if (src != null)
            {
                IntPtr hwnd = src.Handle;

                NativeMethods.SetWindowPos(hwnd, NativeConstants.HWND_TOP, 0, 0, 0, 0,
                    NativeConstants.SWP_NOSIZE | NativeConstants.SWP_NOMOVE | NativeConstants.SWP_NOACTIVATE);
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            DependencyPropertyDescriptor propDesc = DependencyPropertyDescriptor.FromProperty(PlacementRectangleProperty, typeof(PopupDragWindow));
            propDesc.RemoveValueChanged(this, OnPlacementRectangleChanged);
            this.PlacementTarget = null;

            //Below the code for resolve this issue SD17083 Indicator is still on screen when remove the grid from VisualChild 
            if(this.IsOpen)
                this.IsOpen = false;
        }

        #endregion
    }

    /// <summary>
    /// For internal use.
    /// </summary>
    public class DoubleUtil
    {
        [StructLayout(LayoutKind.Explicit)]
        private struct NanUnion
        {
            [FieldOffset(0)]
            internal double DoubleValue;
            [FieldOffset(0)]
            internal ulong UintValue;
        }

        /// <summary>
        /// For internal use.
        /// </summary>
        public static bool IsNaN(double value)
        {
            NanUnion t = new NanUnion();
            t.DoubleValue = value;

            ulong exp = t.UintValue & 0xfff0000000000000;
            ulong man = t.UintValue & 0x000fffffffffffff;

            return (exp == 0x7ff0000000000000 || exp == 0xfff0000000000000) && (man != 0);
        }
    }

    /// <summary>
    /// For internal use.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class PopupPositionWindow : Popup, IDisposable
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public PopupPositionWindow()
        {
            if (!BrowserInteropHelper.IsBrowserHosted)
            {
                this.Placement = PlacementMode.Relative;
            }
            else
            {
                this.Placement = PlacementMode.Mouse;
                this.PlacementTarget = Application.Current.MainWindow;
            }
            this.PlacementRectangle = new Rect(1, 1, 1, 1);
            DependencyPropertyDescriptor propDesc = DependencyPropertyDescriptor.FromProperty(PlacementRectangleProperty, typeof(PopupDragWindow));
            propDesc.AddValueChanged(this, OnPlacementRectangleChanged);
        }

        /// <summary>
        /// Hides the popup.
        /// </summary>
        public void Hide()
        {
            this.IsOpen = false;
            this.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Updates width and haight of the window using the size from the PlacementRectangle.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPlacementRectangleChanged(object sender, EventArgs e)
        {
            if (!this.PlacementRectangle.IsEmpty)
            {
                this.Width = this.PlacementRectangle.Width;
                this.Height = this.PlacementRectangle.Height;
            }
            else
            {
                this.Width = 150;
                this.Height = 100;
            }
        }

        private void Show()
        {
            this.IsOpen = true;
            this.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Moves the popup to the specified point.
        /// </summary>
        /// <param name="p">The target point.</param>
        public void Move(Point p)
        {
            this.Show();
            this.PlacementRectangle = new Rect(p.X, p.Y, this.Width, this.Height);
        }

        private void SetWindowOnTopSecure()
        {
            if (!PermissionHelper.HasUnmanagedCodePermission)
            {
                return;
            }

            HwndSource src = (HwndSource)PresentationSource.FromVisual(Child);

            if (src != null)
            {
                IntPtr hwnd = src.Handle;

                NativeMethods.SetWindowPos(hwnd, NativeConstants.HWND_TOP, 0, 0, 0, 0,
                    NativeConstants.SWP_NOSIZE | NativeConstants.SWP_NOMOVE | NativeConstants.SWP_NOACTIVATE);
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            DependencyPropertyDescriptor propDesc = DependencyPropertyDescriptor.FromProperty(PlacementRectangleProperty, typeof(PopupDragWindow));
            propDesc.RemoveValueChanged(this, OnPlacementRectangleChanged);
            if (this.Child != null)
            {
                AnimatedGrid child = this.Child as AnimatedGrid;
                if (Child != null)
                {
                    child.Dispose();
                    child = null;
                }
                this.Child = null;
            }
        }

        #endregion
    }

    /// <summary>
    /// Represents a method that handles events with <see cref="GridQueryDragColumnHeaderEventArgs"/> that are raised when the user hovers over a column
    /// header  or drags column header with the mouse.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Contains the event data.</param>
    public delegate void GridQueryDragColumnHeaderEventHandler(object sender, GridQueryDragColumnHeaderEventArgs e);


    /// <summary>
    /// Reason why QueryAllowDragColumnHeader event was raised (Show Red Indicator, MouseUp or HitTest).
    /// </summary>
    public enum GridQueryDragColumnHeaderReason
    {
        /// <summary>
        /// HitTest is occuring
        /// </summary>
        HitTest,
        /// <summary>
        /// MouseUp
        /// </summary>
        MouseUp,
        MouseMove
    }

    /// <summary>
    /// Action when QueryAllowDragColumnHeader event was raised.
    /// </summary>
    public enum GridQueryDragColumnHeaderAction
    {
        ColumnMoving,
        ColumnUnGrouping
    }

    /// <summary>
    /// Holds a reference to a <see cref="GridControlBase"/> that initiates the event and the column 
    /// that is affected.
    /// </summary>
    /// <remarks>
    /// Set <see cref="AllowDrag"/> to False if you do not want to allow the user 
    /// to drag the specified <see cref="Column"/>.
    /// </remarks>
    public class GridQueryDragColumnHeaderEventArgs : SyncfusionRoutedEventArgs
    {
        internal int column;
        internal bool allowDrag = true;

        internal int insertBeforeColumn;
        internal GridQueryDragColumnHeaderReason reason;
        
        /// <summary>
        /// Initializes the event args.
        /// </summary>
        /// <param name="grid">The table control.</param>
        /// <param name="column">Column Name.</param>
        /// <param name="insertBeforeColumn">Name of the column to insert at. You can call TableDescriptor.Columns[InsertBeforeColumn] to get the GridColumnDescriptor.</param>
        /// <param name="reason">Reason why this event was raised (Show Red Indicator, MouseUp, or HitTest).</param>
        public GridQueryDragColumnHeaderEventArgs(int column, int insertBeforeColumn, GridQueryDragColumnHeaderReason reason, RoutedEvent routedEvent, object source)
            : base(routedEvent, source)
        {
            this.column = column;
            this.insertBeforeColumn = insertBeforeColumn;
            this.reason = reason;
        }

        /// <summary>
        /// Column Name. You can call TableDescriptor.Columns[Column] to get the GridColumnDescriptor.
        /// </summary>
        [TraceProperty(true)]
        public int Column
        {
            get
            {
                return column;
            }
        }

        /// <summary>
        /// Name of the column to insert at. You can call TableDescriptor.Columns[InsertBeforeColumn] to get the GridColumnDescriptor.
        /// </summary>
        [TraceProperty(true)]
        public int InsertBeforeColumn
        {
            get
            {
                return this.insertBeforeColumn;
            }
        }


        /// <summary>
        /// Reason why this event was raised (Show Red Indicator, MouseUp, or HitTest).
        /// </summary>
        [TraceProperty(true)]
        public GridQueryDragColumnHeaderReason Reason
        {
            get
            {
                return this.reason;
            }
            set
            {
                this.reason = value;
            }
        }

        /// <summary>
        /// Set <see cref="AllowDrag"/> to False if you do not want to allow the user 
        /// to drag the specified <see cref="Column"/>.
        /// </summary>
        [TraceProperty(true)]
        public bool AllowDrag
        {
            get
            {
                return allowDrag;
            }
            set
            {
                allowDrag = value;
            }
        }
    }

    public sealed class GridDataQueryDragColumnHeaderEventArgs : GridQueryDragColumnHeaderEventArgs
    {
        private GridQueryDragColumnHeaderAction action;
        /// <summary>
        /// Initializes the event args.
        /// </summary>
        /// <param name="grid">The table control.</param>
        /// <param name="column">Column Name.</param>
        /// <param name="insertBeforeColumn">Name of the column to insert at. You can call TableDescriptor.Columns[InsertBeforeColumn] to get the GridColumnDescriptor.</param>
        /// <param name="reason">Reason why this event was raised (Show Red Indicator, MouseUp, or HitTest).</param>
        public GridDataQueryDragColumnHeaderEventArgs(int column, int insertBeforeColumn, GridQueryDragColumnHeaderReason reason, GridQueryDragColumnHeaderAction action, RoutedEvent routedEvent, object source)
            : base(column, insertBeforeColumn, reason, routedEvent, source)
        {
            this.column = column;
            this.insertBeforeColumn = insertBeforeColumn;
            this.reason = reason;
            this.action = action;
        }
            
        /// <summary>
        /// Action when this event was raised.
        /// </summary>
        [TraceProperty(true)]
        public GridQueryDragColumnHeaderAction Action
        {
            get
            {
                return this.action;
            }
            set
            {
                this.action = value;
            }
        }

    }
}