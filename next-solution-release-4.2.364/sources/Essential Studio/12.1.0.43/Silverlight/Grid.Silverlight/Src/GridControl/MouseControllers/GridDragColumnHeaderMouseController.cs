#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Input;
using System.Runtime.InteropServices;
#if !WinRT
using Syncfusion.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using Syncfusion.Windows.ComponentModel;
using Syncfusion.Windows.Controls.Scroll;
using Syncfusion.Windows.Controls.Cells;
namespace Syncfusion.Windows.Controls.Grid
{
#else
using Syncfusion.WinRT.Controls.Scroll;
using Windows.Foundation;
using Syncfusion.WinRT.Controls.Cells;
using Windows.UI.Xaml;
using Windows.Devices.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Controls.Primitives;
using Syncfusion.WinRT.ComponentModel;

namespace Syncfusion.WinRT.Controls.Grid
{
#endif

    public class GridDragColumnHeaderMouseController : IMouseController
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
                    if (this.visibleRow != null && this.visibleRow.IsHeader)
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
            grid.Loaded += (s, e) =>
            {
#if !WinRT
                if (!DependencyObjectExtensions.GetEnableMousePosition(Application.Current.RootVisual))
                {
                    DependencyObjectExtensions.SetEnableMousePosition(Application.Current.RootVisual, true);
                }
#else
                if (!DependencyObjectExtensions.GetEnableMousePosition(Window.Current.Content))
                {
                    DependencyObjectExtensions.SetEnableMousePosition(Window.Current.Content, true);
                }
#endif

            };
            this.DragHeaderVisible = true;
            this.upIndicatorWindow = new PopupPositionWindow() { Child = new AnimatedGrid() { ArrowIndicatorDirection = ArrowIndicatorDirection.Up, Width = 15, Height = 10 } };
            this.downIndicatorWindow = new PopupPositionWindow() { Child = new AnimatedGrid() { ArrowIndicatorDirection = ArrowIndicatorDirection.Down, Width = 15, Height = 10 } };
        }

        public GridControlBase Grid
        {
            get;
            private set;
        }

        public bool DragHeaderVisible
        {
            get;
            set;
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
#if !WinRT
        public Cursor Cursor
        {
            get { return Cursors.Hand; }
        }
#endif
        #region Unused methods
        public void MouseHoverEnter(MouseEventArgs e)
        {
        }

        public void MouseHover(MouseControllerEventArgs e)
        {
        }

        public void MouseHoverLeave(MouseEventArgs e)
        {
        }

        #endregion
        private Point mouseDownPoint;
        public void MouseDown(MouseControllerEventArgs e)
        {
            this.mouseDownPoint = e.Location;
            if (this.hitTestInfo == null)
            {
                return;
            }
            this.WasDragged = false;
            // init the drag providers
            // start drag header popup
            this.StartDragHeaderPopup();
            // show arrow indicators
            this.OpenDragIndicators(e.Location);

            //this.Grid.AutoScroller.AutoScrollBounds = this.Grid.GetClipRect(ScrollAxisRegion.Header, ScrollAxisRegion.Header);
            //if (this.Grid.AutoScroller.InsideScrollBounds.Contains(e.Location))
            //{
            //    this.Grid.AutoScroller.AutoScrolling = AutoScrollOrientation.Horizontal;
            //}
        }

        #region Helper methods

        private void OpenDragIndicators(Point location)
        {
            this.indicatorsShown = true;
            Rect clip = Rect.Empty;
            var pt = this.GetArrowIndicatorLocation(ref clip, false, location);
            if (clip != Rect.Empty)
            {
                this.UpIndicator.IndicatorVisibility = Visibility.Visible;
                this.DownIndicator.IndicatorVisibility = Visibility.Visible;
            }
        }

        private bool indicatorsShown = false;
        private int targetColIndex = -1;

        private FlowDirection MouseFlowDirection
        {
            get;
            set;
        }
        private Point GetArrowIndicatorLocation(ref Rect clip, bool raiseEvent, Point location)
        {
#if !WinRT
            GridTreeControlImpl gridTree = this.Grid as GridTreeControlImpl;
#endif
            var rowColIndex = this.Grid.PointToCellRowColumnIndex(location);

            if (rowColIndex != RowColumnIndex.Empty)
            {
                this.targetColIndex = rowColIndex.ColumnIndex;
                if (raiseEvent)
                {
                    if (!this.Grid.RaiseQueryAllowDragColumn(this.hitTestInfo.rowColIndex.ColumnIndex, this.targetColIndex, GridQueryDragColumnHeaderReason.MouseMove))
                    {
                        clip = Rect.Empty;
                        return new Point(-1, -1);
                    }
                }
                Rect r;
#if !WinRT
                if (gridTree != null && this.MouseFlowDirection == FlowDirection.RightToLeft)
                {
                    this.targetColIndex += 1;
                    r = this.Grid.RangeToRect(ScrollAxisRegion.Header, ScrollAxisRegion.Body, GridRangeInfo.Cell(0, this.targetColIndex-1), false, true);
                    if (targetColIndex > gridTree.Columns.Count)
                        targetColIndex -= 1;
                }
                else
#endif
                r = this.Grid.RangeToRect(ScrollAxisRegion.Header, ScrollAxisRegion.Body, GridRangeInfo.Cell(0, this.targetColIndex), false, true);
                clip = r;
                var pt = new Point(r.Right, r.Top);
                var frameworkelement = this.Grid.Parent as FrameworkElement;
                if (r.Right > frameworkelement.ActualWidth)
                {
                    pt.X = frameworkelement.ActualWidth;
                    if (frameworkelement is ScrollableContentViewer)
                    {
                        pt.X -= ((ScrollableContentViewer)frameworkelement).ComputedVerticalScrollBarVisibility == Visibility.Visible ? 19 : 0;
                    }
                }
#if !WinRT
                var matrixTransform = (MatrixTransform)this.Grid.TransformToVisual(Application.Current.RootVisual);
                pt = matrixTransform.Transform(pt);

#else
                var matrixTransform = (MatrixTransform)this.Grid.TransformToVisual(Window.Current.Content);
                pt = matrixTransform.TransformPoint(pt);
#endif
                return pt;
            }

            return new Point(-1, -1);
        }

        private void StartDragHeaderPopup()
        {
            if (this.dragWindow == null)
            {
                this.dragWindow = new PopupDragWindow();
            }
            var element = this.CreateClonedHeader();
            this.dragWindow.ProvideVisual(element);
            this.dragWindow.StartDrag();
        }

        private void CloseDragIndicators()
        {
            this.indicatorsShown = false;
            this.UpIndicator.IndicatorVisibility = Visibility.Collapsed;
            this.DownIndicator.IndicatorVisibility = Visibility.Collapsed;
            this.upIndicatorWindow.Hide();
            this.downIndicatorWindow.Hide();
        }

        Point offSetPoint;
        private FrameworkElement CreateClonedHeader()
        {
            if (this.hitTestInfo == null)
            {
                return null;
            }

            this.offSetPoint = hitTestInfo.point;
            this.offSetPoint = this.offSetPoint.Offset(-hitTestInfo.visibleColumn.Origin, -hitTestInfo.visibleRow.Origin);

            var rowIdx = this.hitTestInfo.rowColIndex.RowIndex;
            var colIdx = this.hitTestInfo.rowColIndex.ColumnIndex;
            var style = this.Grid.Model[rowIdx, colIdx];
            var renderer = this.Grid.CellRenderers[style.CellType];
            var visibleRow = this.hitTestInfo.visibleRow;
            var visibleColumn = this.hitTestInfo.visibleColumn;
            Rect cellRect = new Rect(0, 0, visibleColumn.Size, visibleRow.Size);
            var renderStyle = this.Grid.GetRenderStyleInfo(this.hitTestInfo.rowColIndex);
            ArrangeCellArgs aca = new ArrangeCellArgs(this.Grid, visibleRow, visibleColumn, cellRect, renderStyle, true);
            var cellRenderer = ((GridRenderStyleInfo)aca.CellInfo).CellRenderer;
            aca.CloneUIElement = true;
            cellRenderer.PrepareUIElements(aca, aca.CellUIElements.UIElements, null);
            cellRenderer.Arrange(aca);
#if !WinRT
            var grid = new System.Windows.Controls.Grid();
#else
            var grid = new Windows.UI.Xaml.Controls.Grid();
#endif
            //this.RenderBackground
            Rectangle rectangleShape = new Rectangle();
            rectangleShape.Fill = renderStyle.Background;
            rectangleShape.Height = cellRect.Height;
            rectangleShape.Width = cellRect.Width;
            grid.Children.Add(rectangleShape);
            var element = (FrameworkElement)aca.CellUIElements.UIElements[0];
            grid.Children.Add(element);
            return grid;
            //this.RenderBorderShape(grid, cellRect, cellRect, CellBorderSide.Top, renderStyle.Borders.Top);
            //this.RenderBorderShape(grid, cellRect, cellRect, CellBorderSide.Left, renderStyle.Borders.Left);
            //this.RenderBorderShape(grid, cellRect, cellRect, CellBorderSide.Right, renderStyle.Borders.Right);
            //this.RenderBorderShape(grid, cellRect, cellRect, CellBorderSide.Bottom, renderStyle.Borders.Bottom);
        }

        /*private void RenderBorderShape(Grid grid, Rect cellRect, Rect clipRect, CellBorderSide borderSide, Pen pen)
        {
            if (pen == null) return;
            bool solidLine = true;

            Line line = new Line();
            cellRect = clipRect;
            line.Stroke = pen.Brush;
            line.StrokeThickness = pen.Thickness;

            double d = 0;//solidLine ? pen.Thickness : pen.Thickness / 2;
            switch (borderSide)
            {
                case CellBorderSide.Top:
                    line.X1 = cellRect.Left;
                    line.X2 = cellRect.Right;
                    line.Y1 = cellRect.Top;
                    line.Y2 = cellRect.Top + d;
                    break;
                case CellBorderSide.Bottom:
                    line.X1 = cellRect.Left;
                    line.X2 = cellRect.Right;
                    line.Y1 = cellRect.Bottom - d;
                    line.Y2 = cellRect.Bottom;
                    break;
                case CellBorderSide.Left:
                    line.X1 = cellRect.Left;
                    line.X2 = cellRect.Left + d;
                    line.Y1 = cellRect.Top;
                    line.Y2 = cellRect.Bottom;
                    break;
                case CellBorderSide.Right:
                    line.X1 = cellRect.Right - d;
                    line.X2 = cellRect.Right;
                    line.Y1 = cellRect.Top;
                    line.Y2 = cellRect.Bottom;
                    break;
            }
            grid.Children.Add(line);
        }*/

        private Point GetDragWindowLocation()
        {
#if !WinRT
            Point pt = DependencyObjectExtensions.GetMousePosition(Application.Current.RootVisual);
#else
            Point pt = DependencyObjectExtensions.GetMousePosition(Window.Current.Content);
#endif
            pt = pt.Offset(-offSetPoint.X, -offSetPoint.Y);
            return pt;
        }

        private void UpdateDragIndicators(Point location)
        {
            Rect clip = Rect.Empty;
            var pt = this.GetArrowIndicatorLocation(ref clip, true, location);
            // we need to identify that the current mouse point is not in the origin column
            //this.DragHeaderVisible = !clip.Contains(this.hitTestInfo.point);
            //if (!this.DragHeaderVisible)
            //{
            //    return;
            //}

            if (pt.X >= 0 && pt.Y >= 0)
            {
                if (!this.indicatorsShown)
                {
                    // animations would have been stopped, so restart it here
                    this.UpIndicator.IndicatorVisibility = Visibility.Visible;
                    this.DownIndicator.IndicatorVisibility = Visibility.Visible;
                    this.indicatorsShown = true;
                }
                // adjust X
                pt.X -= 8;

                // set value for up indicator arrow
                pt.Y += 25;
                this.upIndicatorWindow.Move(pt);

                // set value for down indicator arrow
                pt.Y -= 30;
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
        #endregion

        public void MouseMove(MouseControllerEventArgs e)
        {
            if (e.Location.X >= this.mouseDownPoint.X)
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

                //this.Grid.AutoScroller.AutoScrollBounds = this.Grid.GetClipRect(ScrollAxisRegion.Body, ScrollAxisRegion.Body);
                //if (this.Grid.AutoScroller.InsideScrollBounds.Contains(e.Location))
                //{
                //    this.Grid.AutoScroller.AutoScrolling = AutoScrollOrientation.Horizontal;
                //}
            }
        }

        public void MouseUp(MouseControllerEventArgs e)
        {
            if (this.hitTestInfo == null)
            {
                return;
            }

            if (!this.WasDragged)
            {
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
                if (this.Grid.RaiseQueryAllowDragColumn(this.hitTestInfo.rowColIndex.ColumnIndex, this.targetColIndex, GridQueryDragColumnHeaderReason.MouseUp))
                {
                    var headercolumns = this.Grid.Model.HeaderColumns;
                    if (!(this.hitTestInfo.rowColIndex.ColumnIndex <= headercolumns - 1))
                    {
                        if (this.targetColIndex <= headercolumns - 1)
                        {
                            this.Grid.Model.MoveColumns(this.hitTestInfo.rowColIndex.ColumnIndex, 1, headercolumns);
                        }

                        else if (this.targetColIndex > headercolumns - 1)
                        {
                            this.Grid.Model.MoveColumns(this.hitTestInfo.rowColIndex.ColumnIndex, 1, this.targetColIndex);
                        }
                    }
                }
            }

            this.dragWindow = null;
            this.hitTestInfo = null;
            this.Grid.AutoScroller.AutoScrolling = AutoScrollOrientation.None;
        }

        public void CancelMode()
        {
        }

        public void RestoreMode()
        {
        }

        private GridDragHeaderHitTestInfo hitTestInfo;
        /// <summary>
        /// HitTest is called to determine whether your controller wants to handle the mouse events based current context.
        /// </summary>
        /// <remarks>
        /// The current winner of the vote is specified through the controller paramter. Your implementation of HitTest
        /// can decide if it wants to override the existing vote or leave it.
        /// </summary>
        /// <param name="mouseEventArgs">A <see cref="MouseControllerEventArgs"/> holding event data.</param>
        /// <param name="controller">A <see cref="IMouseController"/> that has indicated to handle the mouse event.</param>
        /// <returns>A non-zero value if the button can and wants to handle the mouse event; 0 if the
        /// mouse event is unrelated for this button.</returns>
        public int HitTest(MouseControllerEventArgs mouseEventArgs, IMouseController controller)
        {
#if WPF
            if (mouseEventArgs.SourceEventArgs.LeftButton == MouseButtonState.Pressed && mouseEventArgs.ClickCount == 1 && !mouseEventArgs.IsMouseOverChildElement)
#endif
#if SILVERLIGHT
            //Debug.WriteLine("Button {0} / ClickCount = {1} / IsMouseOverChildElement {2}", mouseEventArgs.Button, mouseEventArgs.ClickCount, mouseEventArgs.IsMouseOverChildElement);
            if (mouseEventArgs.Button == System.Windows.Browser.MouseButtons.Left && mouseEventArgs.IsPressed && mouseEventArgs.ClickCount == 1 && !mouseEventArgs.IsMouseOverChildElement)
#endif
#if WinRT
            if (mouseEventArgs.IsPressed && mouseEventArgs.ClickCount == 1 && !mouseEventArgs.IsMouseOverChildElement)
#endif
            {
                var point = mouseEventArgs.Location;
                this.hitTestInfo = null;
                var rowColIndex = this.Grid.PointToCellRowColumnIndex(point);
                // when we are near to the corner, Columns resizer will try to get the hittest, then we dont have to drag, 4.0 is the precision used in the Columns resizer.
                var cornerHit = this.Grid.ScrollColumns.GetLineNearCorner(point.X, 4.0);
                var hit = cornerHit == null ? this.HitTest(rowColIndex.ColumnIndex) : null;
                if (hit != null)
                {
                    this.hitTestInfo = new GridDragHeaderHitTestInfo(this.Grid, point);
                    if (!this.hitTestInfo.isHeader)
                    //|| this.Grid.Model[hitTestInfo.rowColIndex.RowIndex, hitTestInfo.rowColIndex.ColumnIndex].CellType != "Static")
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

                    if (hitTestInfo != null && this.hitTestInfo.rowColIndex.RowIndex >= this.Grid.Model.HeaderRows)
                    {
                        this.hitTestInfo = null;
                    }

                }
            }

            if (this.hitTestInfo != null)
            {
                return 1;
            }

            return 0;
        }

        private VisibleLineInfo HitTest(int colIndex)
        {
            return this.Grid.ScrollColumns.GetVisibleLineAtLineIndex(colIndex);
        }

        public bool SupportsCancelMouseCapture
        {
            get { return false; }
        }

        public bool SupportsMouseTracking
        {
            get { return false; }
        }

        #endregion

#if WinRT
        public void MouseHoverEnter(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public Windows.UI.Core.CoreCursor Cursor
        {
            get { throw new NotImplementedException(); }
        }

        public void MouseHoverLeave(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
        }
#endif
    }

    /// <summary>
    /// For internal use.
    /// </summary>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class PopupDragWindow : IDisposable
    {
        public Popup popupWindow;

        /// <summary>
        /// Constructor.
        /// </summary>
        public PopupDragWindow()
        {
            this.popupWindow = new Popup() { IsHitTestVisible = false };
            this.DestroyChildOnStopDrag = true;
            this.HorizontalOffset = 0d;
            this.VerticalOffset = 0d;
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
            this.popupWindow.Child = null;
        }

        public UIElement Child
        {
            get
            {
                if (this.popupWindow != null)
                {
                    return this.popupWindow.Child;
                }

                return null;
            }
        }

        public double HorizontalOffset
        {
            get;
            set;
        }

        public double VerticalOffset
        {
            get;
            set;
        }

        private bool isPopupUnderMousePoint = false;
        public bool IsPopupUnderMousePoint
        {
            get
            {
                return this.isPopupUnderMousePoint;
            }
            set
            {
                if (this.isPopupUnderMousePoint != value)
                {
                    //#if !WinRT
                    //                    if (value)
                    //                    {
                    //                        Application.Current.RootVisual.MouseMove += new MouseEventHandler(RootVisual_MouseMove);
                    //                    }
                    //                    else
                    //                    {
                    //                        Application.Current.RootVisual.MouseMove += new MouseEventHandler(RootVisual_MouseMove);
                    //                    }
#if WinRT
                    if (value)
                    {
                        Window.Current.Content.PointerMoved += Content_PointerMoved;
                    }
                    else
                    {
                        Window.Current.Content.PointerMoved -= Content_PointerMoved;
                    }
#endif
                }
            }
        }



        /// <summary>
        /// For internal use.
        /// </summary>
        public void ProvideVisual(FrameworkElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            this.popupWindow.Child = element;
            this.popupWindow.Width = element.Width > 0 ? element.Width : 100;
            this.popupWindow.Height = element.Height > 0 ? element.Height : 100;
        }
        //#if !WinRT
        //        void RootVisual_MouseMove(object sender, MouseEventArgs e)
        //        {
        //            if (!this.IsShowing)
        //            {
        //                var p = e.GetPosition(Application.Current.RootVisual);
        //                this.popupWindow.HorizontalOffset = p.X + this.HorizontalOffset;
        //                this.popupWindow.VerticalOffset = p.Y + this.VerticalOffset;
        //            }
        //        }
#if WinRT

        void Content_PointerMoved(object sender, global::Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (!this.IsShowing)
            {
                var p = e.GetCurrentPoint(Window.Current.Content).Position;
                this.popupWindow.HorizontalOffset =p.X + this.HorizontalOffset;
                this.popupWindow.VerticalOffset = p.Y + this.VerticalOffset;
            }
        }
#endif

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
                this.popupWindow.Visibility = Visibility.Visible;
                this.popupWindow.IsOpen = true;
                this.isShown = true;
            }
        }

        /// <summary>
        /// For internal use.
        /// </summary>
        public bool StartDrag()
        {
            if (this.popupWindow.Child == null)
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
            this.popupWindow.HorizontalOffset = p.X;
            this.popupWindow.VerticalOffset = p.Y;
            //this.PlacementRectangle = new Rect(p.X, p.Y, this.Width, this.Height);
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
                this.popupWindow.Visibility = Visibility.Collapsed;
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
            this.popupWindow.IsOpen = false;
            this.IsDragging = false;
            this.isShown = false;
        }

        public void Dispose()
        {
            //#if !WinRT
            //            Application.Current.RootVisual.MouseMove -= new MouseEventHandler(RootVisual_MouseMove);
#if WinRT
            Window.Current.Content.PointerMoved -= Content_PointerMoved;
#endif
            this.popupWindow = null;
        }
    }

    /// <summary>
    /// For internal use.
    /// </summary>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class PopupPositionWindow
    {
        private Popup popupWindow;
        /// <summary>
        /// Constructor.
        /// </summary>
        public PopupPositionWindow()
        {
            this.popupWindow = new Popup() { IsHitTestVisible = false };
        }

        public FrameworkElement Child
        {
            get
            {
                return (FrameworkElement)this.popupWindow.Child;
            }
            set
            {
                var element = value;
                if (element == null)
                {
                    throw new ArgumentNullException("element");
                }

                this.popupWindow.Child = element;
                this.popupWindow.Width = element.Width > 0 ? element.Width : 100;
                this.popupWindow.Height = element.Height > 0 ? element.Height : 100;
            }
        }

        /// <summary>
        /// Hides the popup.
        /// </summary>
        public void Hide()
        {
            this.popupWindow.IsOpen = false;
            this.popupWindow.Visibility = Visibility.Collapsed;
        }

        private void Show()
        {
            this.popupWindow.IsOpen = true;
            this.popupWindow.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Moves the popup to the specified point.
        /// </summary>
        /// <param name="p">The target point.</param>
        public void Move(Point p)
        {
            this.Show();
            this.popupWindow.HorizontalOffset = p.X;
            this.popupWindow.VerticalOffset = p.Y;
        }
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
    /// Holds a reference to a <see cref="GridControlBase"/> that initiates the event and the column 
    /// that is affected.
    /// </summary>
    /// <remarks>
    /// Set <see cref="AllowDrag"/> to False if you do not want to allow the user 
    /// to drag the specified <see cref="Column"/>.
    /// </remarks>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public sealed class GridQueryDragColumnHeaderEventArgs : SyncfusionRoutedEventArgs
    {
        int column;
        bool allowDrag = true;

        private int insertBeforeColumn;
        private GridQueryDragColumnHeaderReason reason;

        /// <summary>
        /// Initializes the event args.
        /// </summary>
        /// <param name="grid">The table control.</param>
        /// <param name="column">Column Name.</param>
        /// <param name="insertBeforeColumn">Name of the column to insert at. You can call TableDescriptor.Columns[InsertBeforeColumn] to get the GridColumnDescriptor.</param>
        /// <param name="reason">Reason why this event was raised (Show Red Indicator, MouseUp, or HitTest).</param>
        public GridQueryDragColumnHeaderEventArgs(int column, int insertBeforeColumn, GridQueryDragColumnHeaderReason reason)
        {
            this.column = column;
            this.insertBeforeColumn = insertBeforeColumn;
            this.reason = reason;
        }

        /// <summary>
        /// Column Name. You can call TableDescriptor.Columns[Column] to get the GridColumnDescriptor.
        /// </summary>
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

    /// <summary>
    /// For internal use.
    /// </summary>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
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

    internal static class PointExtensions
    {
        public static Point Offset(this Point pt, double offsetX, double offsetY)
        {
            pt.X += offsetX;
            pt.Y += offsetY;
            return pt;
        }
    }
}
