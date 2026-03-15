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
    using System.Linq;
    using System.Net;
    using Syncfusion.Windows.Controls.Scroll;
    using Syncfusion.Windows.Controls.Cells;
    using System.Windows;

    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Controls;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    using Syncfusion.Windows;


    using System.ComponentModel;

    enum GridDataGroupingControllerMode
    {
        GroupDrag,
        GroupDrop
    }

#if SILVERLIGHT
    // Summary:
    //     Defines constants that specify the content flow direction for text and user
    //     interface (UI) elements.
    enum FlowDirection
    {
        // Summary:
        //     Indicates that content should flow from left to right.
        LeftToRight = 0,
        //
        // Summary:
        //     Indicates that content should flow from right to left.
        RightToLeft = 1,
    }
#endif

    class GridDataGroupAreaBaseController : IMouseController
    {
        private PopupDragWindow dragWindow;
        private PopupPositionWindow upIndicatorWindow;
        private PopupPositionWindow downIndicatorWindow;

        /// <summary>
        /// Initializes a new <see cref="GridDragColumnHeaderMouseController"/>
        /// </summary>
        /// <param name="grid">The grid.</param>
        public GridDataGroupAreaBaseController(GridControlBase grid)
        {
            this.Grid = grid;
            grid.Loaded += (s, e) =>
            {
                if (!DependencyObjectExtensions.GetEnableMousePosition(Application.Current.RootVisual))
                {
                    DependencyObjectExtensions.SetEnableMousePosition(Application.Current.RootVisual, true);
                }
            };
            this.DragHeaderVisible = true;
            this.upIndicatorWindow = new PopupPositionWindow() { Child = new AnimatedGrid() { ArrowIndicatorDirection = ArrowIndicatorDirection.Up, Width = 15, Height = 10 } };
            this.downIndicatorWindow = new PopupPositionWindow() { Child = new AnimatedGrid() { ArrowIndicatorDirection = ArrowIndicatorDirection.Down, Width = 15, Height = 10 } };
        }

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

        protected int GroupedColumnIndex
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

        protected bool WasDragged
        {
            get;
            set;
        }

        private GridDataGroupDropAreaGridImpl groupDropAreaGrid;
        public GridDataGroupDropAreaGridImpl GroupDropAreaGrid
        {
            get
            {
                if (this.groupDropAreaGrid == null && this.Grid != null)
                {
                    var dataGrid = this.Grid.FindParentElementOfType<GridDataControl>();
                    if (dataGrid != null)
                    {
                        this.groupDropAreaGrid = dataGrid.GroupDropAreaGrid;
                        if (!DependencyObjectExtensions.GetEnableMousePosition(this.groupDropAreaGrid))
                        {
                            DependencyObjectExtensions.SetEnableMousePosition(this.groupDropAreaGrid, true);
                        }
                    }
                }

                return this.groupDropAreaGrid;
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

        protected GridDataGroupingControllerMode Mode
        {
            get;
            set;
        }

        #region IMouseController Members

        public virtual string Name
        {
            get { return string.Empty; }
        }

        public virtual Cursor Cursor
        {
            get { return Cursors.Hand; }
        }

        #region Unused methods
        /// <summary>
        /// MouseHoverEnter is called when this controller signaled in HitTest that it wants to handle mouse events. MouseHoverEnter
        /// is called before the MouseHover is called for the first time.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        public void MouseHoverEnter(MouseEventArgs e)
        {
        }

        /// <summary>
        /// MouseHover is called when this controller signaled in HitTest that it wants to handle mouse events. MouseHover
        /// is called after MouseHoverEnter.
        /// </summary>
        /// <param name="e">The <see cref="T:Syncfusion.Windows.Controls.Scroll.MouseControllerEventArgs"/> instance containing the event data.</param>
        public void MouseHover(MouseControllerEventArgs e)
        {
        }

        /// <summary>
        /// MouseHoverLeave is called when hovering ends either because user dragged mouse out of the hit-test area or
        /// when context changes (e.g. user pressed the mouse button).
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        public void MouseHoverLeave(System.Windows.Input.MouseEventArgs e)
        {
        }

        #endregion

        private Point mouseDownPoint;
        /// <summary>
        /// MouseDown is called when this controller signaled in HitTest that it wants to handle mouse events and the
        /// user pressed the mouse button.
        /// </summary>
        /// <param name="e">The <see cref="T:Syncfusion.Windows.Controls.Scroll.MouseControllerEventArgs"/> instance containing the event data.</param>
        /// <remarks>
        /// MouseDown is called and this controller will become the active controller and receive all subsequent mouse messages
        /// until the mouse button is released or the mouse operation is cancelled.
        /// </remarks>
        public virtual void MouseDown(MouseControllerEventArgs e)
        {
            this.mouseDownPoint = e.Location;
            if (this.IsMouseInGroupDropArea)
            {
                Point groupDropAreaLocation = DependencyObjectExtensions.PointFromRootVisual(this.GroupDropAreaGrid);
                var rowColIndex = this.GroupDropAreaGrid.PointToCellRowColumnIndex(groupDropAreaLocation);
                if (rowColIndex != null)
                    this.GroupedColumnIndex = this.GroupDropAreaGrid.Model.ColIndexToField(rowColIndex.ColumnIndex);
            }
        }

        /// <summary>
        /// MouseMove is called for the active controller after a MouseDown message when the user moves the mouse pointer.
        /// </summary>
        /// <param name="e">The <see cref="T:Syncfusion.Windows.Controls.Scroll.MouseControllerEventArgs"/> instance containing the event data.</param>
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
            //this.mouseDownPoint = e.Location;
        }

        /// <summary>
        /// MouseUp is called for the active controller after a MouseDown message when the user releases the mouse button.
        /// </summary>
        /// <param name="e">The <see cref="T:Syncfusion.Windows.Controls.Scroll.MouseControllerEventArgs"/> instance containing the event data.</param>
        public virtual void MouseUp(MouseControllerEventArgs e)
        {
        }

        /// <summary>
        /// CancelMode is called for the active controller after a MouseDown message when the mouse operation is cancelled.
        /// </summary>
        public virtual void CancelMode()
        {
        }

        /// <summary>
        /// RestoreMode is called when a controller should be reactivated. Prevoius state can be restored if it was backed up earlier when CancelMode was called.
        /// </summary>
        public virtual void RestoreMode()
        {
        }

        /// <summary>
        /// Hits the test.
        /// </summary>
        /// <param name="colIndex">Index of the col.</param>
        /// <returns></returns>
        protected VisibleLineInfo HitTest(int colIndex)
        {
            if (this.Mode == GridDataGroupingControllerMode.GroupDrag)
            {
                return this.Grid.ScrollColumns.GetVisibleLineAtLineIndex(colIndex);
            }
            else
            {
                return this.GroupDropAreaGrid.ScrollColumns.GetVisibleLineAtLineIndex(colIndex);
            }
        }

        /// <summary>
        /// HitTest is called to determine whether your controller wants to handle the mouse events based current context.
        /// </summary>
        /// <param name="mouseEventArgs">The <see cref="T:Syncfusion.Windows.Controls.Scroll.MouseControllerEventArgs"/> instance containing the event data.</param>
        /// <param name="controller">The controller.</param>
        /// <returns>
        /// A value not equal to 0 indicates that the controller wants to handle the mouse input; otherwise if equal to 0 the controller is not handling it.
        /// </returns>
        /// <remarks>
        /// The current winner of the vote is specified through the controller parameter. Your implementation of HitTest
        /// can decide if it wants to override the existing vote or leave it.
        /// </remarks>
        public virtual int HitTest(MouseControllerEventArgs mouseEventArgs, IMouseController controller)
        {
            return 0;
        }

        /// <summary>
        /// Gets a value indicating whether this controller supports the cancel mouse capture feature and the context of the mouse operation can be changed
        /// while the user drags the pressed mouse.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if supports the cancel mouse capture feature; otherwise, <c>false</c>.
        /// </value>
        public virtual bool SupportsCancelMouseCapture
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether the controller supports the mouse
        /// tracking feature which allows MouseControllerDispatcher to emulate
        /// a pressed mouse operation similar
        /// to the way a combobox selects the item in a dropped list box while
        /// hovering the mouse over the dropped listbox and simulating
        /// a MouseUp when the user presses the mouse.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the controller supports mouse tracking; otherwise, <c>false</c>.
        /// </value>
        public virtual bool SupportsMouseTracking
        {
            get { return false; }
        }

        #endregion

        /// <summary>
        /// Determines whether [is mouse over group drop area].
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [is mouse over group drop area]; otherwise, <c>false</c>.
        /// </returns>
        protected bool IsMouseOverGroupDropArea()
        {
            if (this.Grid == null || this.TableModel == null || this.Table == null || !this.TableProperties.ShowGroupDropArea)
            {
                return false;
            }

            var mousePos = DependencyObjectExtensions.GetMousePosition(Application.Current.RootVisual);
            var point = DependencyObjectExtensions.PointFromRootVisual(this.GroupDropAreaGrid);
            mousePos.X -= point.X;
            mousePos.Y -= point.Y;
            var rect = this.GroupDropAreaGrid.GetClipRect(ScrollAxisRegion.Body, ScrollAxisRegion.Body);
            var result = rect.Contains(mousePos);
            return result;
        }

        #region Helper methods

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

        protected bool IsMouseInGroupDropArea
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

        protected void OpenDragIndicators(Point location, IGridDataDragHeaderHitTestInfo hitTestInfo)
        {
            this.indicatorsShown = true;
            Rect clip = Rect.Empty;
            var pt = this.GetArrowIndicatorLocation(ref clip, false, location, hitTestInfo, ref this.isEventHandled);
            if (clip != Rect.Empty)
            {
                this.UpIndicator.IndicatorVisibility = Visibility.Visible;
                this.DownIndicator.IndicatorVisibility = Visibility.Visible;
            }
        }

        private bool indicatorsShown = false;
        protected Point GetArrowIndicatorLocation(ref Rect clip, bool raiseEvent, Point location, IGridDataDragHeaderHitTestInfo hitTestInfo, ref bool isEventHandled)
        {
            this.IsMouseInGroupDropArea = this.IsMouseOverGroupDropArea();
            if (!this.IsMouseInGroupDropArea)
            {
                var rowColIndex = this.Grid.PointToCellRowColumnIndex(location);
                if (rowColIndex != RowColumnIndex.Empty)
                {
                    this.TargetDragColumnIndex = rowColIndex.ColumnIndex;
                    if (raiseEvent)
                    {
                        if (!this.Grid.RaiseQueryAllowDragColumn(hitTestInfo.RowColumnIndex.ColumnIndex, this.TargetDragColumnIndex, GridQueryDragColumnHeaderReason.MouseMove))
                        {
                            clip = Rect.Empty;
                            isEventHandled = true;
                            return new Point(-1, -1);
                        }
                    }
                    var headerIndex = this.Grid.Model.HeaderRows - 1;
                    Rect r = this.Grid.RangeToRect(ScrollAxisRegion.Header, ScrollAxisRegion.Body, GridRangeInfo.Cell(headerIndex, this.TargetDragColumnIndex), false, true);
                    clip = r;
                    Point pt;
                    if (location.X >= (clip.X + (clip.Width / 2)))
                    {
                        pt = new Point(r.Right, r.Top);
                        if (hitTestInfo.RowColumnIndex.ColumnIndex != this.TargetDragColumnIndex)
                            this.TargetDragColumnIndex++;
                    }
                    else
                    {
                        pt = new Point(r.Left, r.Top);
                        if (hitTestInfo.RowColumnIndex.ColumnIndex == this.TargetDragColumnIndex - 1)
                            this.TargetDragColumnIndex--;
                    }
                    var matrixTransform = (MatrixTransform)this.Grid.TransformToVisual(Application.Current.RootVisual);
                    pt = matrixTransform.Transform(pt);
                    return pt;
                }
            }
            else
            {
                var groupDropAreaLocation = DependencyObjectExtensions.GetMousePosition(Application.Current.RootVisual);
                var point = DependencyObjectExtensions.PointFromRootVisual(this.GroupDropAreaGrid);
                groupDropAreaLocation.X -= point.X;
                groupDropAreaLocation.Y -= point.Y;
                var rowColIndex = this.GroupDropAreaGrid.PointToCellRowColumnIndex(groupDropAreaLocation);
                if (rowColIndex != RowColumnIndex.Empty)
                {
                    var targetColIndex = this.TargetDragGroupColumnIndex = this.GroupDropAreaGrid.Model.ColIndexToField(rowColIndex.ColumnIndex);
                    var isInsideHeaderColIndex = this.GroupDropAreaGrid.Model.IsHeaderColIndex(rowColIndex.ColumnIndex);
                    if (!isInsideHeaderColIndex)
                    {
                        // simply adjust the columnindex to the offset value
                        rowColIndex.ColumnIndex = (targetColIndex + 2) * 2;
                    }

                    Rect r = this.GroupDropAreaGrid.RangeToRect(ScrollAxisRegion.Body, ScrollAxisRegion.Body, GridRangeInfo.Cell(0, rowColIndex.ColumnIndex), false, true);
                    clip = r;
                    Point pt = this.GroupDropAreaGrid.FlowDirection == System.Windows.FlowDirection.RightToLeft ? new Point(r.Right, r.Top) : new Point(r.Left, r.Top);
                    var matrixTransform = (MatrixTransform)this.GroupDropAreaGrid.TransformToVisual(Application.Current.RootVisual);
                    pt = matrixTransform.Transform(pt);
                    return pt;
                }
            }

            return new Point(-1, -1);
        }


        protected void StartDragHeaderPopup(IGridDataDragHeaderHitTestInfo hitTestInfo)
        {
            if (this.dragWindow == null)
            {
                this.dragWindow = new PopupDragWindow();
            }

            var element = this.CreateClonedHeader(hitTestInfo);
            if (element != null)
            {
                this.dragWindow.ProvideVisual(element);
                this.dragWindow.StartDrag();
            }
        }

        protected void CloseDragIndicators()
        {
            this.indicatorsShown = false;
            this.UpIndicator.IndicatorVisibility = Visibility.Collapsed;
            this.DownIndicator.IndicatorVisibility = Visibility.Collapsed;
            this.upIndicatorWindow.Hide();
            this.downIndicatorWindow.Hide();
        }

        Point offSetPoint;
        private FrameworkElement CreateClonedHeader(IGridDataDragHeaderHitTestInfo hitTestInfo)
        {
            if (hitTestInfo == null)
            {
                return null;
            }

            var gridControl = hitTestInfo.Grid;
            this.offSetPoint = hitTestInfo.Point;
            this.offSetPoint = this.offSetPoint.Offset(-hitTestInfo.VisibleColumn.Origin, -hitTestInfo.VisibleRow.Origin);

            var rowIdx = hitTestInfo.RowColumnIndex.RowIndex;
            var colIdx = hitTestInfo.RowColumnIndex.ColumnIndex;


            var style = gridControl.Model[rowIdx, colIdx];
            var renderer = gridControl.CellRenderers[style.CellType];

            var visibleRow = hitTestInfo.VisibleRow;
            var visibleColumn = hitTestInfo.VisibleColumn;
            Rect cellRect = new Rect(0, 0, visibleColumn.Size, visibleRow.Size);
            var renderStyle = gridControl.GetRenderStyleInfo(hitTestInfo.RowColumnIndex);
            ArrangeCellArgs aca = new ArrangeCellArgs(gridControl, visibleRow, visibleColumn, cellRect, renderStyle, true);
            var cellRenderer = ((GridRenderStyleInfo)aca.CellInfo).CellRenderer;
            aca.CloneUIElement = true;
            cellRenderer.PrepareUIElements(aca, aca.CellUIElements.UIElements, null);
            cellRenderer.Arrange(aca);
            var grid = new System.Windows.Controls.Grid();
            //this.RenderBackground
            Rectangle rectangleShape = new Rectangle();
            rectangleShape.Fill = renderStyle.Background;
            rectangleShape.Height = cellRect.Height;
            rectangleShape.Width = cellRect.Width;
            grid.Children.Add(rectangleShape);
            var element = (FrameworkElement)aca.CellUIElements.UIElements[0];
            grid.Children.Add(element);
            return grid;
        }

        private Point GetDragWindowLocation()
        {
            Point pt = DependencyObjectExtensions.GetMousePosition(Application.Current.RootVisual);
            pt = pt.Offset(-offSetPoint.X, -offSetPoint.Y);
            return pt;
        }

        protected void UpdateDragIndicators(Point location, IGridDataDragHeaderHitTestInfo hitTestInfo)
        {
            Rect clip = Rect.Empty;
            this.IsEventHandled = false;
            var pt = this.GetArrowIndicatorLocation(ref clip, true, location, hitTestInfo, ref this.isEventHandled);
            if (this.isEventHandled)
            {
                this.DragHeaderVisible = false;
                this.MoveWindowButCloseIndicators();
                return;
            }

            if (this.Mode == GridDataGroupingControllerMode.GroupDrag)
            {
                if (!this.IsMouseInGroupDropArea)
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
            else if (this.Mode == GridDataGroupingControllerMode.GroupDrop)
            {
                if (this.IsMouseInGroupDropArea)
                {
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
                if (!this.IsMouseInGroupDropArea && this.Grid != null && this.Grid.Model != null)
                {
                    // set value for up indicator arrow
                    double headerHeight = 0;
                    if (this.Grid != null && this.Grid.Model != null)
                        headerHeight = this.Grid.Model.RowHeights[this.Grid.Model.HeaderRows - 1];

                    pt.Y += headerHeight;
                    
                }
                else
                {
                    pt.Y += this.GroupDropAreaGrid.Model.RowHeights[0] + this.GroupDropAreaGrid.Model.RowHeights[1];
                    
                }
                this.upIndicatorWindow.Move(pt);

                if (!this.IsMouseInGroupDropArea && this.Grid != null && this.Grid.Model != null)
                {
                    // set value for down indicator arrow
                    double headerHeight = 0;
                    if(this.Grid != null && this.Grid.Model != null)
                        headerHeight = this.Grid.Model.RowHeights[this.Grid.Model.HeaderRows - 1] + 10;
                        
                    pt.Y -= headerHeight;
                    
                }
                else
                {
                    pt.Y -= this.GroupDropAreaGrid.Model.RowHeights[1] + this.GroupDropAreaGrid.Model.RowHeights[0];
                }
                this.downIndicatorWindow.Move(pt);

                var windowPoint = this.GetDragWindowLocation();
                this.dragWindow.MoveTo(windowPoint);
            }
            else
            {
                // we are out of bounds so close the indicator popup and stop the animation
                this.CloseDragIndicators();
            }
        }


        private void MoveWindowButCloseIndicators()
        {
            var windowPoint = this.GetDragWindowLocation();
            this.dragWindow.MoveTo(windowPoint);
            this.CloseDragIndicators();
        }

        #endregion

    }

    internal interface IGridDataDragHeaderHitTestInfo
    {
        GridControlBase Grid
        {
            get;
        }

        bool IsHeader
        {
            get;
        }

        Point Point
        {
            get;
        }

        RowColumnIndex RowColumnIndex
        {
            get;
        }

        VisibleLineInfo VisibleColumn
        {
            get;
        }

        VisibleLineInfo VisibleRow
        {
            get;
        }
    }

    class GridDataGroupDragMouseController : GridDataGroupAreaBaseController
    {
        internal sealed class GridDataGroupDragHeaderHitTestInfo : IGridDataDragHeaderHitTestInfo
        {
            GridDataGroupDragHeaderHitTestInfo()
            {
                this.rowColumnIndex = RowColumnIndex.Empty;
            }

            internal GridDataGroupDragHeaderHitTestInfo(GridControlBase grid, Point point)
                : this()
            {
                this.rowColumnIndex = grid.PointToCellRowColumnIndex(point, true);
                if (this.rowColumnIndex != RowColumnIndex.Empty)
                {
                    this.grid = grid;
                    this.visibleColumn = grid.ScrollColumns.GetVisibleLineAtLineIndex(this.RowColumnIndex.ColumnIndex);
                    this.visibleRow = grid.ScrollRows.GetVisibleLineAtLineIndex(this.RowColumnIndex.RowIndex);
                    if (this.VisibleRow != null && this.VisibleRow.LineIndex < grid.Model.HeaderRows)
                    {
                        this.isHeader = true;
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

        public const string MouseControllerName = "GridDataGroupDropAreaMouseController";

        public GridDataGroupDragMouseController(GridControlBase grid)
            : base(grid)
        {
            this.Mode = GridDataGroupingControllerMode.GroupDrag;
        }

        public override string Name
        {
            get
            {
                return GridDataGroupDragMouseController.MouseControllerName;
            }
        }

        private IGridDataDragHeaderHitTestInfo hitTestInfo;
        public override int HitTest(MouseControllerEventArgs mouseEventArgs, IMouseController controller)
        {
            //System.Diagnostics.Debug.WriteLine("Button {0} / IsPressed {1} / ClickCount {2}", mouseEventArgs.Button, mouseEventArgs.IsPressed, mouseEventArgs.ClickCount);
            if (mouseEventArgs.Button == System.Windows.Browser.MouseButtons.Left && mouseEventArgs.IsPressed && mouseEventArgs.ClickCount == 1 && !mouseEventArgs.IsMouseOverChildElement)
            {
                var point = mouseEventArgs.Location;
                this.hitTestInfo = null;
                var rowColIndex = this.Grid.PointToCellRowColumnIndex(point);
                // when we are near to the corner, Columns resizer will try to get the hittest, then we dont have to drag, 4.0 is the precision used in the Columns resizer.
                var headerIndex = this.Grid.Model.HeaderRows - 1;
                if (rowColIndex.RowIndex == headerIndex)
                {
                    var hitTestPrecision = 4.0;

                    var mc = controller as GridResizeColumnsMouseController;

                    if (mc != null)
                    {
                        hitTestPrecision = mc.GetHitTestPrecision();
                    }

                    var cornerHit = this.Grid.ScrollColumns.GetLineNearCorner(point.X, hitTestPrecision);

                    var hit = cornerHit == null ? this.HitTest(rowColIndex.ColumnIndex) : null;
                    if (hit != null)
                    {
                        this.hitTestInfo = new GridDataGroupDragHeaderHitTestInfo(this.Grid, point);

                        if (!this.hitTestInfo.IsHeader)
                        {
                            this.hitTestInfo = null;
                        }
                        else
                        {
                            if (!this.Grid.RaiseQueryAllowDragColumn(this.hitTestInfo.RowColumnIndex.ColumnIndex, -1, GridQueryDragColumnHeaderReason.HitTest))
                            {
                                this.hitTestInfo = null;
                            }
                        }
                    }
                }
            }

            if (this.hitTestInfo != null)
            {
                this.Mode = GridDataGroupingControllerMode.GroupDrag;
                return 1;
            }

            return 0;
        }

        public override void MouseDown(MouseControllerEventArgs e)
        {
            base.MouseDown(e);
            if (this.hitTestInfo == null)
            {
                return;
            }

            if (this.hitTestInfo != null)
            {
                // check column can drag?
                var colIndex = this.TableModel.ResolvePositionToVisibleColumnIndex(this.hitTestInfo.RowColumnIndex.ColumnIndex);
                var visibleCol = colIndex > -1 && colIndex < this.TableProperties.VisibleColumns.Count ? this.TableProperties.VisibleColumns[colIndex] : null;
                if (visibleCol != null && !visibleCol.AllowDrag)
                {
                    return;
                }
                if (this.Grid.CurrentCell != null)
                {
                    var currentCellRenderer = this.Grid.CurrentCell.Renderer;
                    if (currentCellRenderer != null && (currentCellRenderer is GridDataHeaderCellRenderer))
                    {
                        var headerCell = currentCellRenderer.CurrentCellUIElement as GridDataHeaderCellControl;
                        if (headerCell != null)
                        {
                            headerCell.CloseFilterDropDown();
                            headerCell.CloseColumnOptionsDropDown();
                        }
                    }
                }
            }

            this.WasDragged = false;
            this.IsEventHandled = false;
            this.StartDragHeaderPopup(this.hitTestInfo);
            this.OpenDragIndicators(e.Location, this.hitTestInfo);
        }

        public override void MouseMove(MouseControllerEventArgs e)
        {
            base.MouseMove(e);
            if (this.DragWindow != null && this.DragWindow.IsDragging)
            {
                this.UpdateDragIndicators(e.Location, this.hitTestInfo);
                this.WasDragged |= this.DragHeaderVisible;
            }
        }

        public override void MouseUp(MouseControllerEventArgs e)
        {
            base.MouseUp(e);
            if (this.hitTestInfo == null)
            {
                return;
            }

            if (this.IsEventHandled)
            {
                this.Close();
                return;
            }

            //Cannot Ungroup the columns if All Columns are grouped. So here i check the hidden columns count and disable the grouping of last column.

            var lastColumn = this.TableProperties.VisibleColumns.Where(c => c.IsHidden == false);
            if (lastColumn.Count() == 1 && this.TableProperties.HideColumnsWhenGrouped)
            {
                this.Close();
                return;
            }

            if (!this.WasDragged)
            {
                var dataGrid = this.Grid.FindParentElementOfType<GridDataControl>();
                this.Grid.RaiseGridCellClick(this.hitTestInfo.RowColumnIndex.RowIndex, this.hitTestInfo.RowColumnIndex.ColumnIndex);
                if (dataGrid.ListBoxSelectionMode==GridSelectionMode.None && (this.Grid.Model.Options.AllowSelection & GridSelectionFlags.Column) == GridSelectionFlags.Column)
                {
                    this.AllowSelectionColumn(e.Location);
                }
            }
            else
            {
                if (!this.IsMouseInGroupDropArea)
                {
                    var sameColMoved = this.hitTestInfo.RowColumnIndex.ColumnIndex == this.TargetDragColumnIndex;
                    if (!sameColMoved)
                    {
                        // move columns if columns are dragged
                        if (this.Grid.RaiseQueryAllowDragColumn(this.hitTestInfo.RowColumnIndex.ColumnIndex, this.TargetDragColumnIndex, GridQueryDragColumnHeaderReason.MouseUp))
                        {
                            this.TableModel.IsInSourceListChanged = true;
                            int target = this.TargetDragColumnIndex;
                            if (this.TargetDragColumnIndex > this.hitTestInfo.RowColumnIndex.ColumnIndex)//this is correct the drag columns when the current column is greater than targeted column.
                                target = this.TargetDragColumnIndex > 0 ? this.TargetDragColumnIndex - 1 : this.TargetDragColumnIndex;
                            this.TableModel.MoveColumns(this.hitTestInfo.RowColumnIndex.ColumnIndex, 1, target);
                            this.TableModel.IsInSourceListChanged = false;
                        }
                    }
                }
                else
                {
                    // move columns to group drop area
                    var visibleColIndex = this.GroupDropAreaGrid.Model.ResolvePositionToVisibleColumnIndex(this.hitTestInfo.RowColumnIndex.ColumnIndex);
                    var column = visibleColIndex < this.TableProperties.VisibleColumns.Count ? this.TableProperties.VisibleColumns[visibleColIndex] : null;
                    if (column != null && column.AllowGroup)
                    {
                        var groupedColumn = this.TableProperties.GroupedColumns.FirstOrDefault(o => o.ColumnName == column.MappingName);
                        if (groupedColumn == null)
                        {
                            this.TableModel.Table.HideAllUIRows();
                            this.TableModel.IsInGroup = true;
                            this.TableModel.View.BeginInit();
                            var col = new GridDataGroupColumn() { ColumnName = column.MappingName };
                            this.TableProperties.GroupedColumns.Insert(this.TargetDragGroupColumnIndex, col);
                            var sortColumn = this.TableProperties.GetSortColumnForGroup(col);
                            if (sortColumn == null)
                            {
                                var sortCol = new GridDataSortColumn() { ColumnName = column.MappingName };
                                this.TableProperties.SortColumns.Insert(this.TableProperties.SortColumns.Count, sortCol);
                            }
                            else
                            {
                                this.TableProperties.SortColumns.Remove(sortColumn);
                                this.TableProperties.SortColumns.Add(sortColumn);
                            }
                            
                            this.TableModel.View.EndInit();
                            
                        }

                        this.TableModel.UpdateSelectedRanges();

                        this.TableModel.IsInGroup = false;
                        
                        // refresh the group drop area grid
                        this.GroupDropAreaGrid.Refresh();
                        this.TableModel.InvalidateDisplay();
                    }
                }
            }

            this.Close();
        }

        #region Properties

        /// <summary>
        /// Gets the selections.
        /// </summary>
        /// <value>The selections.</value>
        internal GridModelSelections Selections
        {
            get { return this.Grid.Model.Selections; }
        }

        #endregion

        #region Helper methods
        
        /// <summary>
        /// Allows the selection column.
        /// </summary>
        /// <param name="e">The e.</param>
        internal void AllowSelectionColumn(Point e)
        {
            Point point = e;
            RowColumnIndex end = this.Grid.PointToCellRowColumnIndex(point);

            int rowIndex = end.RowIndex;
            int colIndex = end.ColumnIndex;

            GridRangeInfo newRange = GridRangeInfo.Empty;
            if (colIndex >= this.Grid.InternalGetHeaderCols())
                newRange = GridRangeInfo.Col(colIndex);
            else
                newRange = GridRangeInfo.Empty;

            if (
                newRange.IsTable && (this.Grid.Model.Options.AllowSelection & GridSelectionFlags.Table) != GridSelectionFlags.None
                || newRange.IsRows && (this.Grid.Model.Options.AllowSelection & GridSelectionFlags.Row) != GridSelectionFlags.None
                || newRange.IsCols && (this.Grid.Model.Options.AllowSelection & GridSelectionFlags.Column) != GridSelectionFlags.None
                || newRange.IsCells && (this.Grid.Model.Options.AllowSelection & GridSelectionFlags.Cell) != GridSelectionFlags.None)
            {
                if (this.Grid.Model.Options.ExcelLikeCurrentCell && !Selections.Ranges.AnyRangeContains(newRange))
                {
                    Selections.Clear();
                    Selections.Add(newRange);
                }
            }

        }

        private void Close()
        {
            if (this.DragWindow != null && this.DragWindow.IsDragging)
            {
                this.DragWindow.StopDrag();
                this.CloseDragIndicators();
            }

            this.DragWindow = null;
            this.hitTestInfo = null;
        }

        #endregion
    }

    class GridDataGroupDropMouseController : GridDataGroupAreaBaseController
    {
        internal sealed class GridDataGroupDropHeaderHitTestInfo : IGridDataDragHeaderHitTestInfo
        {
            GridDataGroupDropHeaderHitTestInfo()
            {
                this.rowColumnIndex = RowColumnIndex.Empty;
            }

            internal GridDataGroupDropHeaderHitTestInfo(GridControlBase grid, Point point)
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

        public const string MouseControllerName = "GridDataGroupDropMouseController";

        public GridDataGroupDropMouseController(GridControlBase grid)
            : base(grid)
        {
            this.Mode = GridDataGroupingControllerMode.GroupDrop;
        }

        public override string Name
        {
            get
            {
                return GridDataGroupDropMouseController.MouseControllerName;
            }
        }

        private IGridDataDragHeaderHitTestInfo hitTestInfo = null;
        public override int HitTest(MouseControllerEventArgs mouseEventArgs, IMouseController controller)
        {
            if (mouseEventArgs.Button == System.Windows.Browser.MouseButtons.Left && mouseEventArgs.IsPressed && mouseEventArgs.ClickCount == 1 && !mouseEventArgs.IsMouseOverChildElement)
            {
                var point = mouseEventArgs.Location;
                this.hitTestInfo = null;
                var rowColIndex = this.GroupDropAreaGrid.PointToCellRowColumnIndex(point);
                var hit = this.HitTest(rowColIndex.ColumnIndex);
                if (hit != null)
                {
                    var isMouseOverGroupDropArea = this.IsMouseOverGroupDropArea();
                    if (isMouseOverGroupDropArea)
                    {
                        this.hitTestInfo = new GridDataGroupDropHeaderHitTestInfo(this.GroupDropAreaGrid, point);
                        if (this.hitTestInfo.IsHeader)
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

        public override void MouseDown(MouseControllerEventArgs e)
        {
            base.MouseDown(e);
            if (this.hitTestInfo == null || this.TableModel == null)
            {
                return;
            }

            if (this.TableProperties.GroupedColumns.Count > 0)
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

        public override void MouseMove(MouseControllerEventArgs e)
        {
            base.MouseMove(e);

            if (this.hitTestInfo == null)
            {
                return;
            }

            if (this.DragWindow.IsDragging)
            {
                this.UpdateDragIndicators(e.Location, this.hitTestInfo);
                this.WasDragged |= this.DragHeaderVisible;
            }
        }

        public override void MouseUp(MouseControllerEventArgs e)
        {
            base.MouseUp(e);
            if (this.hitTestInfo == null)
            {
                return;
            }

            var groupIndex = this.GroupDropAreaGrid.Model.ColIndexToField(this.hitTestInfo.RowColumnIndex.ColumnIndex);
            var groupColumn = groupIndex < this.TableProperties.GroupedColumns.Count ? this.TableProperties.GroupedColumns[groupIndex] : null;
            if (groupColumn != null)
            {
                if (!this.WasDragged || !this.DragHeaderVisible)
                {
                    this.TableModel.IsInSort = true;

                    var sortColumn = this.TableProperties.GetSortColumnForGroup(groupColumn);
                    var visibleColumn = this.TableProperties.VisibleColumns[groupColumn.ColumnName];
                    if (visibleColumn != null && !visibleColumn.AllowSort)
                    {
                        return;
                    }
                    this.TableModel.View.BeginInit();
                    if (sortColumn != null)
                    {
                        sortColumn.SortDirection = sortColumn.SortDirection == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
                        var sortIndex = this.TableProperties.SortColumns.IndexOf(sortColumn);
                        this.TableProperties.SortColumns[sortIndex] = sortColumn;
                    }
                    this.TableModel.View.EndInit();
                    this.GroupDropAreaGrid.Refresh();
                    this.TableModel.IsInSort = false;
                    this.TableModel.InvalidateDisplay();
                }
                else
                {
                    if (!IsMouseInGroupDropArea)
                    {
                        // hotfix - CurrentCell.MoveTo(-1,-1);
                        this.Grid.CurrentCell.Deactivate();
                        var visibleColumn = this.TableProperties.VisibleColumns.FirstOrDefault(o => o.MappingName == groupColumn.ColumnName);

                        if (visibleColumn == null)
                        {
                            visibleColumn = new GridDataVisibleColumn()
                            {
                                MappingName = groupColumn.ColumnName,
                                AllowDrag = this.TableProperties.AllowDragColumns,
                                AllowResize = this.TableProperties.AllowResizeColumns,
                                ShowColumnOptions = this.TableProperties.ShowColumnOptions,
                                AllowFilter = this.TableProperties.ShowFilters,
                                AllowGroup = true,
                                AllowSort = this.TableProperties.AllowSort
                            };
                            this.TableProperties.VisibleColumns.Add(visibleColumn);
                        }

                        // get the maxlevel to know the indent level before it is removed, once it is removed, the indent levels will be refreshed
                        var indentLevels = this.Table.GroupModel.GetMaxLevel();
                        var indentEqualsTotalGroups = this.TableProperties.GroupedColumns.Count == indentLevels;

                        this.TableModel.IsInGroup = true;
                        this.TableModel.View.BeginInit();
                        this.TableProperties.GroupedColumns.Remove(groupColumn);
                        var sortColumn = this.TableProperties.GetSortColumnForGroup(groupColumn);
                        if (sortColumn != null)
                        {
                            this.TableProperties.SortColumns.Remove(sortColumn);
                        }
                        this.TableModel.View.EndInit();
                        this.TableModel.IsInGroup = false;
                        var visibleColIndex = this.TableProperties.VisibleColumns.IndexOf(visibleColumn);
                        var actualGridColIndex = this.TableModel.ResolveVisibleColumnIndexToPosition(visibleColIndex);
                        var toGridColIndex = this.TargetDragColumnIndex;
                        // when column was hidden then check as below
                        if (this.TableProperties.HideColumnsWhenGrouped && toGridColIndex <= actualGridColIndex)
                        {
                            toGridColIndex += 1; // the target drag col would be one less since the visible line would be hidden for this column
                        }
                        var isNoGroup = this.TableProperties.GroupedColumns.Count == 0;
                        if (isNoGroup && indentLevels > 0)
                        {
                            toGridColIndex -= indentLevels;
                        }
                        else if (indentLevels > 1)
                        {
                            if (!indentEqualsTotalGroups)
                            {
                                actualGridColIndex -= 1;
                            }
                            toGridColIndex -= 1;
                        }
                        if (actualGridColIndex > -1 && toGridColIndex > -1)
                        {
                            this.TableModel.IsInSourceListChanged = true;
                            this.Grid.Model.MoveColumns(actualGridColIndex, 1, toGridColIndex);
                            this.TableModel.IsInSourceListChanged = false;
                            this.GroupDropAreaGrid.Refresh();
                        }
                        // }
                    }
                    else
                    {
                        if (this.GroupedColumnIndex == this.TargetDragGroupColumnIndex || this.GroupedColumnIndex + 1 == this.TargetDragGroupColumnIndex)//|| this.GroupedColumnIndex -1 == this.TargetDragGroupColumnIndex)
                            this.Table.RefreshHiddenColumns();
                        else
                        {
                            int target = this.TargetDragGroupColumnIndex;
                            if (this.GroupedColumnIndex <= this.TargetDragGroupColumnIndex)
                                target -= 1;

                            this.TableProperties.GroupedColumns.Remove(groupColumn);
                            if (target >= this.TableProperties.GroupedColumns.Count)
                            {
                                this.TableProperties.GroupedColumns.Add(groupColumn);
                            }
                            else
                            {
                                this.TableProperties.GroupedColumns.Insert(target, groupColumn);
                            }
                        }                        
                    }
                }

                if(this.WasDragged)
                    this.TableModel.IsInGroup = true;

                this.TableModel.UpdateSelectedRanges();

                this.TableModel.IsInGroup = false;

                this.TableModel.InvalidateDisplay();

            }

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
