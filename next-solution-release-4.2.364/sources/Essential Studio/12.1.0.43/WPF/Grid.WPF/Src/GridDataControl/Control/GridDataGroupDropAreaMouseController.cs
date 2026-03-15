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
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Syncfusion.Windows.Controls.Scroll;
    using Syncfusion.Windows.Controls.Cells;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Controls;
    using System.Windows.Media.Imaging;
    using Syncfusion.Linq;
    using Syncfusion.Windows.GridCommon;
    using System.Windows.Media.Animation;
    using System.ComponentModel;
    using System.Collections.Specialized;
    using System.IO;
    using System.Windows.Data;

    enum GridDataGroupingControllerMode
    {
        GroupDrag,
        GroupDrop
    }


    class GridDataGroupAreaBaseController : IMouseController, IDisposable
    {

        #region Variables

        private GridDataGroupDropAreaGridImpl groupDropAreaGrid;
        private GridDataColumnChooserWindow columnChooserGrid;
        private PopupDragWindow dragWindow;
        private PopupPositionWindow upIndicatorWindow;
        private PopupPositionWindow downIndicatorWindow;

        #endregion

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="GridDataGroupAreaBaseController"/> class.
        /// </summary>
        /// <param name="grid">The grid.</param>
        public GridDataGroupAreaBaseController(GridControlBase grid)
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
                    this._cursor = this.Grid.RaiseGridCellCursor();
                }
                else
                {
                    this._cursor = Cursors.No;
                }
            }
            else
            {
                this._cursor = this.Grid.RaiseGridCellCursor();
            }
        }

        

        void grid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                this.CloseDragIndicators();
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the grid.
        /// </summary>
        /// <value>The grid.</value>
        public GridControlBase Grid
        {
            get;
            private set;
        }


        /// <summary>
        /// Gets the group drop area grid.
        /// </summary>
        /// <value>The group drop area grid.</value>
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
                    }
                }

                return this.groupDropAreaGrid;
            }
        }


        /// <summary>
        /// Gets the column chooser grid.
        /// </summary>
        /// <value>The column chooser grid.</value>
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

        /// <summary>
        /// Gets the table model.
        /// </summary>
        /// <value>The table model.</value>
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

        /// <summary>
        /// Gets the table.
        /// </summary>
        /// <value>The table.</value>
        public GridDataTable Table
        {
            get
            {
                var table = this.TableModel != null ? this.TableModel.Table : null;
                return table;
            }
        }

        /// <summary>
        /// Gets the table properties.
        /// </summary>
        /// <value>The table properties.</value>
        public GridDataTableProperties TableProperties
        {
            get
            {
                var tableProperties = this.TableModel != null ? this.TableModel.TableProperties : null;
                return tableProperties;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [drag header visible].
        /// </summary>
        /// <value><c>true</c> if [drag header visible]; otherwise, <c>false</c>.</value>
        public bool DragHeaderVisible
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets up indicator.
        /// </summary>
        /// <value>Up indicator.</value>
        public AnimatedGrid UpIndicator
        {
            get
            {
                return this.upIndicatorWindow.Child as AnimatedGrid;
            }
        }

        /// <summary>
        /// Gets down indicator.
        /// </summary>
        /// <value>Down indicator.</value>
        public AnimatedGrid DownIndicator
        {
            get
            {
                return this.downIndicatorWindow.Child as AnimatedGrid;
            }
        }

        #endregion

        #region Protected Properties

        /// <summary>
        /// Gets or sets a value indicating whether [was dragged].
        /// </summary>
        /// <value><c>true</c> if [was dragged]; otherwise, <c>false</c>.</value>
        protected bool WasDragged
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the mode.
        /// </summary>
        /// <value>The mode.</value>
        protected GridDataGroupingControllerMode Mode
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets the drag window.
        /// </summary>
        /// <value>The drag window.</value>
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

        #endregion

        #region IMouseController Members

        /// <summary>
        /// Returns the name of this mouse controller.
        /// </summary>
        /// <value></value>
        public virtual string Name
        {
            get { return string.Empty; }
        }

         private Cursor _cursor;

         /// <summary>
         /// Returns the cursor to be displayed.
         /// </summary>
         /// <value></value>
         public virtual System.Windows.Input.Cursor Cursor
         {
             get
             {
                 Cursor currentCursor = this.Grid.RaiseGridCellCursor();
                 if (this.WasDragged)
                 {
                     if (this.DragHeaderVisible)
                     {
                         if (currentCursor == Cursors.Arrow)
                         {
                             return _cursor;
                         }
                         else
                         {
                             return currentCursor;
                         }
                     }
                     else
                     {
                         return Cursors.No;
                     }
                 }
                 else
                 {
                     if (currentCursor == Cursors.Arrow)
                     {
                         return _cursor;
                     }
                     else
                     {
                         return currentCursor;
                     }
                 }
             }
             set
             {
                 _cursor = value;
             }           
         }

        #region Hidden functions

         /// <summary>
         /// MouseHoverEnter is called when this controller signaled in HitTest that it wants to handle mouse events. MouseHoverEnter
         /// is called before the MouseHover is called for the first time.
         /// </summary>
         /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        public void MouseHoverEnter(System.Windows.Input.MouseEventArgs e)
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
        public GridQueryDragColumnHeaderAction action;
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
            if (IsMouseOverGroupDropArea())
            {
                action = GridQueryDragColumnHeaderAction.ColumnUnGrouping;
                Point groupDropAreaLocation = Mouse.GetPosition(this.GroupDropAreaGrid);
                var rowColIndex = this.GroupDropAreaGrid.PointToCellRowColumnIndex(groupDropAreaLocation);
                if (rowColIndex != null)
                    this.GroupedColumnIndex = this.GroupDropAreaGrid.Model.ColIndexToField(rowColIndex.ColumnIndex);
            }
            else
                action = GridQueryDragColumnHeaderAction.ColumnMoving;
        }

        /// <summary>
        /// MouseMove is called for the active controller after a MouseDown message when the user moves the mouse pointer.
        /// </summary>
        /// <param name="e">The <see cref="T:Syncfusion.Windows.Controls.Scroll.MouseControllerEventArgs"/> instance containing the event data.</param>
        public virtual void MouseMove(MouseControllerEventArgs e)
        {
            this.mouseDownPoint = e.Location;
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
            get
            {
                return false;
            }
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
            get
            {
                return false;
            }
        }

        #region Helper methods
        /// <summary>
        /// Determines whether [is mouse over group drop area].
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [is mouse over group drop area]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsMouseOverGroupDropArea()
        {
            if (this.Grid == null || this.TableModel == null || this.Table == null || !this.TableProperties.ShowGroupDropArea || this.GroupDropAreaGrid == null)
            {
                return false;
            }

            var mousePos = Mouse.GetPosition(this.GroupDropAreaGrid);
            var rect = this.GroupDropAreaGrid.GetClipRect(ScrollAxisRegion.Body, ScrollAxisRegion.Body);
            var result = rect.Contains(mousePos);
            return result;
        }

        /// <summary>
        /// Determines whether [is mouse over column chooser area].
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [is mouse over column chooser area]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsMouseOverColumnChooserArea()
        {
            var result = false;
            if (this.Grid == null || this.TableModel == null || this.Table == null)
            {
                return false;
            }

            var mousePos = Mouse.GetPosition(this.ColumnChooserGrid.Grid);
            Rect rect = new Rect(0, 0, this.columnChooserGrid.Grid.ActualWidth, this.columnChooserGrid.Grid.ActualHeight);
            if (rect.Contains(mousePos))
            {
                return true;
            }
            return result;
        }

        /// <summary>
        /// Opens the drag indicators.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="hitTestInfo">The hit test info.</param>
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
        /// <summary>
        /// Gets or sets a value indicating whether this instance is event handled.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is event handled; otherwise, <c>false</c>.
        /// </value>
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

        /// <summary>
        /// Updates the drag indicators.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="hitTestInfo">The hit test info.</param>
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
                    this.DragHeaderVisible = this.TargetDragColumnIndex != hitTestInfo.RowColumnIndex.ColumnIndex;//!clip.Contains(hitTestInfo.Point);
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
            if (pt.X > 0 && pt.Y > 0)
            {
                // var grid = hitTestInfo.Grid.FindElementOfType<GridDataControl>(); Unused local variable
                if (!this.indicatorsShown)
                {
                    // animations would have been stopped, so restart it here
                    this.UpIndicator.Begin();
                    this.DownIndicator.Begin();
                    this.indicatorsShown = true;
                }
                // adjust X
                if (SystemParameters.MenuDropAlignment)
                    pt.X += 30;
                else
                    pt.X += 10;
                double y = 0;
                if (this.GroupDropAreaGrid != null && this.GroupDropAreaGrid.Model != null)
                    y = this.GroupDropAreaGrid.Model.RowHeights[1];

                if (!this.IsMouseInGroupDropArea)
                {
                    // set value for up indicator arrow
                    var headerHeight = this.Grid.Model.RowHeights[this.Grid.Model.HeaderRows - 1];
                    pt.Y += headerHeight;
                    //pt.Y += 20;
                }
                else
                {
                    pt.Y += y;
                    pt.X -= 25; //this is included to update the drag indicator in correct position.
                }
                this.upIndicatorWindow.Move(pt);

                if (!this.IsMouseInGroupDropArea)
                {
                    // set value for down indicator arrow
                    //if (grid != null && grid.Model != null)
                    {
                        //int index = .Model.TableProperties.StackedHeaderRows != null ? grid.Model.TableProperties.StackedHeaderRows.Count : 0;
                        var headerHeight = this.Grid.Model.RowHeights[this.Grid.Model.HeaderRows - 1];
                        pt.Y -= headerHeight + 20;
                    }
                    //else
                        //pt.Y -= 40;
                }
                else
                {
                    // adjust values if we are in group drop area
                    pt.Y -= y + this.GroupDropAreaGrid.Model.RowHeights[0] - 2;
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

        /// <summary>
        /// Moves the window but close indicators.
        /// </summary>
        private void MoveWindowButCloseIndicators()
        {
            var windowPoint = this.GetDragWindowLocation();
            this.dragWindow.MoveTo(windowPoint);
            this.CloseDragIndicators();
        }

        private bool indicatorsShown = false;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is mouse in group drop area.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is mouse in group drop area; otherwise, <c>false</c>.
        /// </value>
        protected bool IsMouseInGroupDropArea
        {
            get;
            private set;
        }

        protected int GroupedColumnIndex
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the index of the target drag column.
        /// </summary>
        /// <value>The index of the target drag column.</value>
        protected int TargetDragColumnIndex
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the index of the target drag group column.
        /// </summary>
        /// <value>The index of the target drag group column.</value>
        protected int TargetDragGroupColumnIndex
        {
            get;
            private set;
        }

        protected Point GetArrowIndicatorLocation(ref Rect clip, bool raiseEvent, Point location, IGridDataDragHeaderHitTestInfo hitTestInfo, ref bool isEventHandled)
        {
            this.IsMouseInGroupDropArea = this.IsMouseOverGroupDropArea();
            if (!this.IsMouseInGroupDropArea)
            {
                var rowColIndex = this.Grid.PointToCellRowColumnIndex(location);
                var GroupDropAreaRowIndex = hitTestInfo.RowColumnIndex;
                var groupIndex = (this.GroupDropAreaGrid != null && this.GroupDropAreaGrid.Model != null) ? this.GroupDropAreaGrid.Model.ColIndexToField(hitTestInfo.RowColumnIndex.ColumnIndex) : -1;
                var groupColumn = ( groupIndex >= 0 && groupIndex < this.TableProperties.GroupedColumns.Count) ? this.TableProperties.GroupedColumns[groupIndex] : null;
                var VisibleColumn = groupColumn != null ? this.TableProperties.VisibleColumns.FirstOrDefault(v => v.MappingName.Equals(groupColumn.ColumnName)) : null;
                int VisibleColumnIndex = this.TableProperties.VisibleColumns.IndexOf(VisibleColumn);
                int ActualColIndex = (action == GridQueryDragColumnHeaderAction.ColumnUnGrouping && groupColumn != null) ? this.TableModel.ResolveVisibleColumnIndexToPosition(VisibleColumnIndex) : hitTestInfo.RowColumnIndex.ColumnIndex;
                if (rowColIndex != RowColumnIndex.Empty)
                {
                    this.TargetDragColumnIndex = rowColIndex.ColumnIndex;
                    var targetIndex = this.TargetDragColumnIndex;
                    var headerIndex = this.Grid.Model.HeaderRows - 1;
                    Rect r = this.Grid.RangeToRect(ScrollAxisRegion.Header, ScrollAxisRegion.Body, GridRangeInfo.Cell(headerIndex, this.TargetDragColumnIndex), false, true);
                    clip = r;
                    Point pt;
                    if (location.X >= (clip.X + (clip.Width / 2)))
                        targetIndex++;
                    if (raiseEvent)
                    {
                        if (!this.Grid.RaiseGridDataQueryAllowDragColumn(ActualColIndex, targetIndex, GridQueryDragColumnHeaderReason.MouseMove,action))
                        {
                            clip = Rect.Empty;
                            isEventHandled = true;
                            return new Point(0, 0);
                        }
                    }
                    if (location.X >= (clip.X + (clip.Width / 2)))
                    {
                        pt = new Point(r.Right - 20, r.Top);
                        if (hitTestInfo.RowColumnIndex.ColumnIndex != this.TargetDragColumnIndex)
                            this.TargetDragColumnIndex++;
                    }
                    else
                    {
                        pt = new Point(r.Left - 20, r.Top);
                        if (hitTestInfo.RowColumnIndex.ColumnIndex == this.TargetDragColumnIndex - 1)
                            this.TargetDragColumnIndex--;
                    }
                    pt = this.Grid.PointToScreen(pt);
                    return pt;
                }
            }
            else
            {
                Point groupDropAreaLocation = Mouse.GetPosition(this.GroupDropAreaGrid);
                var rowColIndex = this.GroupDropAreaGrid.PointToCellRowColumnIndex(groupDropAreaLocation);
                if (rowColIndex != RowColumnIndex.Empty)
                {
                    var targetColIndex = this.TargetDragGroupColumnIndex = this.GroupDropAreaGrid.Model.ColIndexToField(rowColIndex.ColumnIndex);
                    var isInsideHeaderColIndex = this.GroupDropAreaGrid.Model.IsHeaderColIndex(rowColIndex.ColumnIndex);
#if DEBUG
                    Console.WriteLine("Target : " + this.TargetDragGroupColumnIndex);
#endif
                    if (!isInsideHeaderColIndex)
                    {
                        // simply adjust the columnindex to the offset value
                        rowColIndex.ColumnIndex = (targetColIndex + 2) * 2;
                    }
                    //if (isInsideHeaderColIndex)
                    //{
                    Rect r = this.GroupDropAreaGrid.RangeToRect(ScrollAxisRegion.Body, ScrollAxisRegion.Body, GridRangeInfo.Cell(0, rowColIndex.ColumnIndex), false, true);
                    clip = r;
                    Point pt = this.GroupDropAreaGrid.FlowDirection == FlowDirection.RightToLeft ? new Point(r.Right, r.Top) : new Point(r.Left, r.Top);
                    pt = this.GroupDropAreaGrid.PointToScreen(pt);
                    return pt;
                    //}
                }
            }
            return new Point(0, 0);
        }

        /// <summary>
        /// Starts the drag header popup.
        /// </summary>
        /// <param name="hitTestInfo">The hit test info.</param>
        protected void StartDragHeaderPopup(IGridDataDragHeaderHitTestInfo hitTestInfo)
        {
            if (this.dragWindow == null)
            {
                this.dragWindow = new PopupDragWindow();
            }

            var element = this.CreateClonedHeader(hitTestInfo);
            if (element != null)
            {
                this.dragWindow.ProvideElement(element);
                this.dragWindow.AllowsTransparency = true;
                this.dragWindow.StartDrag();
            }
        }

        /// <summary>
        /// Closes the drag indicators.
        /// </summary>
        protected void CloseDragIndicators()
        {
            this.indicatorsShown = false;
            this.UpIndicator.Stop();
            this.DownIndicator.Stop();
            this.upIndicatorWindow.Hide();
            this.downIndicatorWindow.Hide();
        }

        void dragWindow_LostFocus(object sender, RoutedEventArgs e)
        {
            this.CloseDragIndicators();
        }

        /// <summary>
        /// Gets the drag window location.
        /// </summary>
        /// <returns></returns>
        private Point GetDragWindowLocation()
        {
            Point pt = Mouse.GetPosition(this.Grid);
            pt = this.Grid.PointToScreen(pt);
            if (SystemParameters.MenuDropAlignment)
                pt.Offset(+offSetPoint.X, -offSetPoint.Y);
            else
                pt.Offset(-offSetPoint.X, -offSetPoint.Y);
            
            return pt;
        }

        private Point OffsetPoint(Point pt, double offsetX, double offsetY)
        {
            pt.X += offsetX;
            pt.Y += offsetY;
            return pt;
        }

        #region Cell To Image Renderer

        Point offSetPoint;
        /// <summary>
        /// Creates the header image.
        /// </summary>
        /// <param name="hitTestInfo">The hit test info.</param>
        /// <returns></returns>
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

            //dc.DrawRectangle(renderStyle.Background, new Pen(Brushes.Red, 5d), cellRect);

            //this.RenderBorder(dc, cellRect, cellRect, CellBorderSide.Top, renderStyle.Borders.Top);
            //this.RenderBorder(dc, cellRect, cellRect, CellBorderSide.Left, renderStyle.Borders.Left);
            //this.RenderBorder(dc, cellRect, cellRect, CellBorderSide.Right, renderStyle.Borders.Right);
            //this.RenderBorder(dc, cellRect, cellRect, CellBorderSide.Bottom, renderStyle.Borders.Bottom);

            //this.RenderBorder(dc, cellRect, cellRect, CellBorderSide.Top, new Pen(Brushes.Red, 13d));
            //this.RenderBorder(dc, cellRect, cellRect, CellBorderSide.Left, new Pen(Brushes.Red, 13d));
            //this.RenderBorder(dc, cellRect, cellRect, CellBorderSide.Right, new Pen(Brushes.Red, 13d));
            //this.RenderBorder(dc, cellRect, cellRect, CellBorderSide.Bottom, new Pen(Brushes.Red, 13d));
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

            PngBitmapEncoder png = new PngBitmapEncoder();
            png.Frames.Add(BitmapFrame.Create(bmp));
            using (Stream stm = File.Create(@"D:\new.png"))
            {
                png.Save(stm);
            }

            if (renderStyle.FlowDirection == FlowDirection.RightToLeft)
            {
                System.Windows.Controls.Image image = new Image
                                                          {
                                                              Source = bmp,
                                                              Width = rca.CellRect.Width,
                                                              Height = rca.CellRect.Height,
                                                              LayoutTransform = new ScaleTransform(-1, 1)
                                                          };
                return image;
            }
            else
            {
                Image img = new Image
                                      {
                                          Source = bmp,
                                          Width = rca.CellRect.Width,
                                          Height = rca.CellRect.Height
                                      };

                return img;
            }

        }

        protected FrameworkElement CreateClonedHeader(IGridDataDragHeaderHitTestInfo hitTestInfo)
        {
            if (hitTestInfo == null)
            {
                return null;
            }

            var gridControl = hitTestInfo.Grid;
            this.offSetPoint = hitTestInfo.Point;
            this.offSetPoint = this.OffsetPoint(this.offSetPoint, -hitTestInfo.VisibleColumn.Origin, -hitTestInfo.VisibleRow.Origin);

            var rowIdx = hitTestInfo.RowColumnIndex.RowIndex;
            var colIdx = hitTestInfo.RowColumnIndex.ColumnIndex;


            // var style = gridControl.Model[rowIdx, colIdx];
            // var renderer = gridControl.CellRenderers[style.CellType]; Unused local variable

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
            System.Windows.Shapes.Rectangle rectangleShape = new System.Windows.Shapes.Rectangle();
            rectangleShape.FlowDirection = renderStyle.FlowDirection;
            rectangleShape.Fill = renderStyle.Background;
            rectangleShape.Height = cellRect.Height;
            rectangleShape.Width = cellRect.Width;
            grid.Children.Add(rectangleShape);
            FrameworkElement element = aca.CellUIElements.UIElements.Count > 0 ? (FrameworkElement)aca.CellUIElements.UIElements[0] : null;
            if (element != null)
            {
                element.FlowDirection = renderStyle.FlowDirection;
                //grid.FlowDirection = renderStyle.FlowDirection;
                Console.WriteLine(renderStyle.FlowDirection);
                grid.Children.Add(element);
            }
            return grid;
        }

        protected Border GetBorder(IGridDataDragHeaderHitTestInfo hitTestInfo)
        {
            // var renderStyle = hitTestInfo.Grid.GetRenderStyleInfo(hitTestInfo.RowColumnIndex); Unused local variable
            Border border = new Border();
            //border.BorderThickness = new Thickness(renderStyle.Borders.Left.Thickness,renderStyle.Borders.Top.Thickness,renderStyle.Borders.Right.Thickness,renderStyle.Borders.Bottom.Thickness);
            //border.BorderBrush = renderStyle.Borders.Bottom.Brush;
            return border;
        }

        /// <summary>
        /// Renders the border.
        /// </summary>
        /// <param name="dc">The dc.</param>
        /// <param name="cellRect">The cell rect.</param>
        /// <param name="clipRect">The clip rect.</param>
        /// <param name="borderSide">The border side.</param>
        /// <param name="pen">The pen.</param>
        private void RenderBorder(DrawingContext dc, Rect cellRect, Rect clipRect, CellBorderSide borderSide, Pen pen)
        {
            if (cellRect.Width == 0)
                return;

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

        public virtual void Dispose()
        {
            if (this.dragWindow != null)
            {
                this.dragWindow.Dispose();
                this.dragWindow = null;
            }
            if (this.upIndicatorWindow != null)
            {
                this.upIndicatorWindow.Dispose();
                this.upIndicatorWindow = null;
            }
            if (this.downIndicatorWindow != null)
            {
                this.downIndicatorWindow.Dispose();
                this.downIndicatorWindow = null;
            }            
        }
    }

    internal interface IGridDataDragHeaderHitTestInfo
    {

        #region Properties

        /// <summary>
        /// Gets the grid.
        /// </summary>
        /// <value>The grid.</value>
        GridControlBase Grid
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is header.
        /// </summary>
        /// <value><c>true</c> if this instance is header; otherwise, <c>false</c>.</value>
        bool IsHeader
        {
            get;
        }

        /// <summary>
        /// Gets the point.
        /// </summary>
        /// <value>The point.</value>
        Point Point
        {
            get;
        }

        /// <summary>
        /// Gets the index of the row column.
        /// </summary>
        /// <value>The index of the row column.</value>
        RowColumnIndex RowColumnIndex
        {
            get;
        }

        /// <summary>
        /// Gets the visible column.
        /// </summary>
        /// <value>The visible column.</value>
        VisibleLineInfo VisibleColumn
        {
            get;
        }

        /// <summary>
        /// Gets the visible row.
        /// </summary>
        /// <value>The visible row.</value>
        VisibleLineInfo VisibleRow
        {
            get;
        }

        #endregion
        
    }

    class GridDataGroupDragMouseController : GridDataGroupAreaBaseController, IDisposable
    {
        internal sealed class GridDataGroupDragHeaderHitTestInfo : IGridDataDragHeaderHitTestInfo
        {

            #region Variables

            private GridControlBase grid;
            private bool isHeader = false;
            private Point point;
            private RowColumnIndex rowColumnIndex;
            private VisibleLineInfo visibleColumn;
            private VisibleLineInfo visibleRow;

            #endregion

            #region ctor

            /// <summary>
            /// Initializes a new instance of the <see cref="GridDataGroupDragHeaderHitTestInfo"/> class.
            /// </summary>
            GridDataGroupDragHeaderHitTestInfo()
            {
                this.rowColumnIndex = RowColumnIndex.Empty;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="GridDataGroupDragHeaderHitTestInfo"/> class.
            /// </summary>
            /// <param name="grid">The grid.</param>
            /// <param name="point">The point.</param>
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

            #endregion

            #region Public Properties

            /// <summary>
            /// Gets the grid.
            /// </summary>
            /// <value>The grid.</value>
            public GridControlBase Grid
            {
                get
                {
                    return this.grid;
                }
            }


            /// <summary>
            /// Gets a value indicating whether this instance is header.
            /// </summary>
            /// <value><c>true</c> if this instance is header; otherwise, <c>false</c>.</value>
            public bool IsHeader
            {
                get
                {
                    return this.isHeader;
                }
            }


            /// <summary>
            /// Gets the point.
            /// </summary>
            /// <value>The point.</value>
            public Point Point
            {
                get
                {
                    return this.point;
                }
            }


            /// <summary>
            /// Gets the index of the row column.
            /// </summary>
            /// <value>The index of the row column.</value>
            public RowColumnIndex RowColumnIndex
            {
                get
                {
                    return this.rowColumnIndex;
                }
            }


            /// <summary>
            /// Gets the visible column.
            /// </summary>
            /// <value>The visible column.</value>
            public VisibleLineInfo VisibleColumn
            {
                get
                {
                    return this.visibleColumn;
                }
            }


            /// <summary>
            /// Gets the visible row.
            /// </summary>
            /// <value>The visible row.</value>
            public VisibleLineInfo VisibleRow
            {
                get
                {
                    return this.visibleRow;
                }
            }

            #endregion
            
        }

        #region Variables

        private IGridDataDragHeaderHitTestInfo hitTestInfo;

        #endregion

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="GridDataGroupDragMouseController"/> class.
        /// </summary>
        /// <param name="grid">The grid.</param>
        public GridDataGroupDragMouseController(GridControlBase grid)
            : base(grid)
        {
            this.Mode = GridDataGroupingControllerMode.GroupDrag;
        }

        #endregion

        #region Const

        public const string MouseControllerName = "GridDataGroupDropAreaMouseController";

        #endregion

        #region Overrides

        /// <summary>
        /// Returns the name of this mouse controller.
        /// </summary>
        /// <value></value>
        public override string Name
        {
            get
            {
                return GridDataGroupDragMouseController.MouseControllerName;
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
        public override int HitTest(MouseControllerEventArgs mouseEventArgs, IMouseController controller)
        {
            if (mouseEventArgs.SourceEventArgs.LeftButton == MouseButtonState.Pressed && mouseEventArgs.ClickCount == 1 && !mouseEventArgs.IsMouseOverChildElement)
            {
                var point = mouseEventArgs.Location;
                this.hitTestInfo = null;
                var rowColIndex = this.Grid.PointToCellRowColumnIndex(point);
                // when we are near to the corner, Columns resizer will try to get the hittest, then we dont have to drag, 4.0 is the precision used in the Columns resizer.
                var headerIndex = this.Grid.Model.HeaderRows - 1;
                if (rowColIndex.RowIndex == headerIndex)
                {
                    var cornerHit = this.Grid.ScrollColumns.GetLineNearCorner(point.X, 4.0);
                    if (cornerHit == null && this.Grid.Model.Options.AllowExcelLikeResizing)
                    {
                        cornerHit = this.Grid.ScrollColumns.GetLineNearCorner(point.X, 6.0, CornerSide.Right);
                        if(cornerHit==null &&point.X < 5 && point.X >= 1 && rowColIndex.ColumnIndex>0)
                        {         
                            cornerHit = new VisibleLineInfo(0, rowColIndex.ColumnIndex - 1, 0, point.X, 0, true,false);
                        }                    
                    }
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
                            if (!this.Grid.RaiseGridDataQueryAllowDragColumn(this.hitTestInfo.RowColumnIndex.ColumnIndex, -1, GridQueryDragColumnHeaderReason.HitTest,action))
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

        /// <summary>
        /// MouseDown is called when this controller signaled in HitTest that it wants to handle mouse events and the
        /// user pressed the mouse button.
        /// </summary>
        /// <param name="e">The <see cref="T:Syncfusion.Windows.Controls.Scroll.MouseControllerEventArgs"/> instance containing the event data.</param>
        /// <remarks>
        /// MouseDown is called and this controller will become the active controller and receive all subsequent mouse messages
        /// until the mouse button is released or the mouse operation is cancelled.
        /// </remarks>
        public override void MouseDown(MouseControllerEventArgs e)
        {
            if (Mouse.GetPosition(this.Grid).X > this.Grid.ScrollColumns.ViewSize)
                return;
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
                if (ColumnChooserGrid != null)
                {
                    this.ColumnChooserGrid.DragColIndex = colIndex;
                    this.ColumnChooserGrid.MappingName = visibleCol.MappingName;
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

            this.Grid.AutoScroller.AutoScrollBounds = this.Grid.GetClipRect(ScrollAxisRegion.Header, ScrollAxisRegion.Header);
            if (this.Grid.AutoScroller.InsideScrollBounds.Left <= e.Location.X || this.Grid.AutoScroller.InsideScrollBounds.Right >= e.Location.X)
            {
                this.Grid.AutoScroller.AutoScrolling = AutoScrollOrientation.Horizontal;
            }
        }

        /// <summary>
        /// MouseMove is called for the active controller after a MouseDown message when the user moves the mouse pointer.
        /// </summary>
        /// <param name="e">The <see cref="T:Syncfusion.Windows.Controls.Scroll.MouseControllerEventArgs"/> instance containing the event data.</param>
        public override void MouseMove(MouseControllerEventArgs e)
        {
            base.MouseMove(e);
			 if (e.Button == null || this.hitTestInfo==null)
            {
                this.Close();
            }
           

            if (this.DragWindow != null && this.DragWindow.IsDragging)
            {
                this.UpdateDragIndicators(e.Location, this.hitTestInfo);
                this.WasDragged |= this.DragHeaderVisible;
                if (this.ColumnChooserGrid != null)
                {
                    if (this.WasDragged || this.IsMouseOverColumnChooserArea())
                    {
                        this.ColumnChooserGrid.Draggingwindow = this.DragWindow.IsDragging;
                        this.TableProperties.CanDropOnColumnChooser = this.DragWindow.IsDragging;
                        this.TableProperties.DraggingColumnName = this.ColumnChooserGrid.MappingName;
                        this.TableProperties.DragColumnIndex = this.ColumnChooserGrid.DragColIndex;
                    }
                    else
                    {
                        this.ColumnChooserGrid.Draggingwindow = false;
                        this.TableProperties.CanDropOnColumnChooser = false;
                        this.TableProperties.DraggingColumnName = "";
                        this.TableProperties.DragColumnIndex = -1;
                    }
                }

                this.Grid.AutoScroller.AutoScrollBounds = this.Grid.GetClipRect(ScrollAxisRegion.Header, ScrollAxisRegion.Header);
                if (this.Grid.AutoScroller.InsideScrollBounds.Left <= e.Location.X || this.Grid.AutoScroller.InsideScrollBounds.Right >= e.Location.X)
                {
                    this.Grid.AutoScroller.AutoScrolling = AutoScrollOrientation.Horizontal;
                }
            }
        }

        /// <summary>
        /// MouseUp is called for the active controller after a MouseDown message when the user releases the mouse button.
        /// </summary>
        /// <param name="e">The <see cref="T:Syncfusion.Windows.Controls.Scroll.MouseControllerEventArgs"/> instance containing the event data.</param>
        public override void MouseUp(MouseControllerEventArgs e)
        {
            base.MouseUp(e);

            if (this.hitTestInfo == null)
            {
                return;
            }

            if (this.IsEventHandled || e.ClickCount == 2)
            {                
                this.Close();                
                return;
            }

            //Cannot Ungroup the columns if All Columns are grouped. So here i check the hidden columns count and disable the grouping of last column.                       
            var lastColumn =this.TableProperties.VisibleColumns.Where(c => c.IsHidden == false);
            if (lastColumn.Count() == 1 && this.TableProperties.HideColumnsWhenGrouped)
            {
                this.Close();
                return;
            }
            
            if (!this.WasDragged)
            {
                var dataGrid=this.Grid.FindParentElementOfType<GridDataControl>();
                this.Grid.RaiseGridCellClick(this.hitTestInfo.RowColumnIndex.RowIndex, this.hitTestInfo.RowColumnIndex.ColumnIndex);
                if (dataGrid.ListBoxSelectionMode==GridSelectionMode.None && (this.Grid.Model.Options.AllowSelection & GridSelectionFlags.Column)==GridSelectionFlags.Column)
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
                        int rowcol = 0;
                        if (ColumnChooserGrid != null && this.IsMouseOverColumnChooserArea())
                        {
                            var rowcolindex = this.ColumnChooserGrid.Grid.PointToCellRowColumnIndex(e.Location);
                            rowcol = rowcolindex.RowIndex;
                        }
                        else
                        {
                            this.TableProperties.CanDropOnColumnChooser = false;
                            this.TableProperties.DraggingColumnName = "";
                            this.TableProperties.DragColumnIndex = -1;
                            if (ColumnChooserGrid != null)
                                this.ColumnChooserGrid.Draggingwindow = false;
                        }
                        if (rowcol == 0)
                        {
                            if (this.Grid.RaiseGridDataQueryAllowDragColumn(this.hitTestInfo.RowColumnIndex.ColumnIndex, this.TargetDragColumnIndex, GridQueryDragColumnHeaderReason.MouseUp,action))
                            {
                                this.TableModel.IsInSourceListChanged = true;
                                int target = this.TargetDragColumnIndex;
                                if (this.TargetDragColumnIndex > this.hitTestInfo.RowColumnIndex.ColumnIndex)//this is correct the drag columns when the current column is greater than targeted column.
                                    target = this.TargetDragColumnIndex > 0 ? this.TargetDragColumnIndex - 1 : this.TargetDragColumnIndex;
                                this.TableModel.MoveColumns(this.hitTestInfo.RowColumnIndex.ColumnIndex, 1, target);
                                this.TableModel.IsInSourceListChanged = false;
                                if (this.ColumnChooserGrid != null)
                                {
                                    this.ColumnChooserGrid.Draggingwindow = false;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (ColumnChooserGrid != null && !this.IsMouseOverColumnChooserArea())
                        {
                            this.TableProperties.DragColumnIndex = -1;
                        }
                    }
                }
                else
                {
                    // move columns to group drop area
                    var visibleColIndex = this.GroupDropAreaGrid.Model.ResolvePositionToVisibleColumnIndex(this.hitTestInfo.RowColumnIndex.ColumnIndex);
                    var column = visibleColIndex < this.TableProperties.VisibleColumns.Count ? this.TableProperties.VisibleColumns[visibleColIndex] : null;
                    var isNotLegacyDataTableAndUnbound = column.IsUnbound && this.TableModel.IsLegacyDataTable;
                    if (column != null && !isNotLegacyDataTableAndUnbound /*&& !column.IsUnbound*/ && column.AllowGroup)
                    {
                        var groupedColumn = this.TableProperties.GroupedColumns.FirstOrDefault(o => o.ColumnName == column.MappingName);
                        if (groupedColumn == null)
                        {
                            this.TableModel.Table.HideAllUIRows();
                            this.TableModel.IsInGroup = true;
                            this.TableModel.View.BeginInit();
                            var col = new GridDataGroupColumn() { ColumnName = column.MappingName };

                            this.TableProperties.GroupedColumns.Insert(this.TargetDragGroupColumnIndex, col);
                            if (this.TableProperties.SortWhenGrouped)
                            {
                                this.TableModel.IsInSort = true;
                                var sortColumn = this.TableProperties.GetSortColumnForGroup(col);
                                if (sortColumn == null)
                                {
                                    var sortCol = new GridDataSortColumn() { ColumnName = column.MappingName };
                                    if (this.TableProperties.Model.Table.RaiseSortColumnsChanging(new List<GridDataSortColumn>() { sortCol }, null, NotifyCollectionChangedAction.Add)) //Adding new column for sort
                                    {
                                        var count = this.TableProperties.SortColumns.Count;
                                        this.TableProperties.SortColumns.Insert(count, sortCol);
                                        this.TableProperties.Model.Table.RaiseSortColumnsChanged(new List<GridDataSortColumn>() { sortCol }, null, NotifyCollectionChangedAction.Add);
                                    }
                                }
                                else
                                {                                    
                                    if (this.TableProperties.Model.Table.RaiseSortColumnsChanging(new List<GridDataSortColumn>() { sortColumn }, new List<GridDataSortColumn>(), NotifyCollectionChangedAction.Add))
                                    {
                                        this.TableProperties.SortColumns.Remove(sortColumn);
                                        this.TableProperties.SortColumns.Add(sortColumn);
                                        this.TableProperties.Model.Table.RaiseSortColumnsChanged(new List<GridDataSortColumn>() { sortColumn }, new List<GridDataSortColumn>(), NotifyCollectionChangedAction.Add);
                                    }
                                }
                                this.TableModel.IsInSort = false;
                            }
                            this.TableModel.View.EndInit();                            
                            //while Grouping the columns via GroupDropArea, Last Group wont get expand because in OnGroupCollectionChanged event GroupModel will be null, so in order to expand last group the below code is added.
                            if (this.TableProperties.ExpandGroupsWhenGrouped)
                                this.TableModel.Table.ExpandAllGroups();
                            
                            this.TableModel.UpdateSelectedRanges();
                            this.TableModel.IsInGroup = false;
                        }
                        if (this.ColumnChooserGrid != null)
                        {
                            this.ColumnChooserGrid.DragColIndex = -1;
                        }

                        // refresh the group drop area grid
                        this.GroupDropAreaGrid.Refresh();
                    }
                }
            }

            this.Grid.AutoScroller.AutoScrolling = AutoScrollOrientation.None;
            this.Close();
        }

        #endregion

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

        #region Helper Methods

        /// <summary>
        /// Allows the selection column.
        /// </summary>
        /// <param name="e">The e.</param>
        internal void AllowSelectionColumn(Point e)
        {
            Point point = e;
            RowColumnIndex end = this.Grid.PointToCellRowColumnIndex(point);

            // int rowIndex = end.RowIndex; Unused local variable
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

        /// <summary>
        /// Closes this instance.
        /// </summary>
        private void Close()
        {
            if (this.DragWindow != null && this.DragWindow.IsDragging)
            {
                this.DragWindow.StopDrag();
                this.CloseDragIndicators();
            }
            if (this.DragWindow != null)
            {
                this.DragWindow.Dispose();
                this.DragWindow = null;
            }
            this.hitTestInfo = null;
        }

        #endregion


        public override void Dispose()
        {
            base.Dispose();
        }
    }

    class GridDataGroupDropMouseController : GridDataGroupAreaBaseController, IDisposable
    {

        internal sealed class GridDataGroupDropHeaderHitTestInfo : IGridDataDragHeaderHitTestInfo
        {

            #region Variables

            private GridControlBase grid;
            private bool isHeader = false;
            private Point point;
            private RowColumnIndex rowColumnIndex;
            private VisibleLineInfo visibleColumn;
            private VisibleLineInfo visibleRow;

            #endregion

            #region ctor

            /// <summary>
            /// Initializes a new instance of the <see cref="GridDataGroupDropHeaderHitTestInfo"/> class.
            /// </summary>
            GridDataGroupDropHeaderHitTestInfo()
            {
                this.rowColumnIndex = RowColumnIndex.Empty;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="GridDataGroupDropHeaderHitTestInfo"/> class.
            /// </summary>
            /// <param name="grid">The grid.</param>
            /// <param name="point">The point.</param>
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

            #endregion

            #region Properties

            /// <summary>
            /// Gets the grid.
            /// </summary>
            /// <value>The grid.</value>
            public GridControlBase Grid
            {
                get
                {
                    return this.grid;
                }
            }


            /// <summary>
            /// Gets a value indicating whether this instance is header.
            /// </summary>
            /// <value><c>true</c> if this instance is header; otherwise, <c>false</c>.</value>
            public bool IsHeader
            {
                get
                {
                    return this.isHeader;
                }
            }


            /// <summary>
            /// Gets the point.
            /// </summary>
            /// <value>The point.</value>
            public Point Point
            {
                get
                {
                    return this.point;
                }
            }


            /// <summary>
            /// Gets the index of the row column.
            /// </summary>
            /// <value>The index of the row column.</value>
            public RowColumnIndex RowColumnIndex
            {
                get
                {
                    return this.rowColumnIndex;
                }
            }


            /// <summary>
            /// Gets the visible column.
            /// </summary>
            /// <value>The visible column.</value>
            public VisibleLineInfo VisibleColumn
            {
                get
                {
                    return this.visibleColumn;
                }
            }


            /// <summary>
            /// Gets the visible row.
            /// </summary>
            /// <value>The visible row.</value>
            public VisibleLineInfo VisibleRow
            {
                get
                {
                    return this.visibleRow;
                }
            }

            #endregion
            
        }

        #region Variables

        private IGridDataDragHeaderHitTestInfo hitTestInfo = null;

        #endregion

        #region Const

        public const string MouseControllerName = "GridDataGroupDropMouseController";

        #endregion

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="GridDataGroupDropMouseController"/> class.
        /// </summary>
        /// <param name="grid">The grid.</param>
        public GridDataGroupDropMouseController(GridControlBase grid)
            : base(grid)
        {
            this.Mode = GridDataGroupingControllerMode.GroupDrop;
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Returns the name of this mouse controller.
        /// </summary>
        /// <value></value>
        public override string Name
        {
            get
            {
                return GridDataGroupDropMouseController.MouseControllerName;
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
        public override int HitTest(MouseControllerEventArgs mouseEventArgs, IMouseController controller)
        {
            if (mouseEventArgs.SourceEventArgs.LeftButton == MouseButtonState.Pressed && mouseEventArgs.ClickCount == 1 && !mouseEventArgs.IsMouseOverChildElement)
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

        /// <summary>
        /// MouseDown is called when this controller signaled in HitTest that it wants to handle mouse events and the
        /// user pressed the mouse button.
        /// </summary>
        /// <param name="e">The <see cref="T:Syncfusion.Windows.Controls.Scroll.MouseControllerEventArgs"/> instance containing the event data.</param>
        /// <remarks>
        /// MouseDown is called and this controller will become the active controller and receive all subsequent mouse messages
        /// until the mouse button is released or the mouse operation is cancelled.
        /// </remarks>
        public override void MouseDown(MouseControllerEventArgs e)
        {
            base.MouseDown(e);

            if (this.hitTestInfo == null || this.TableModel == null)
            {
                return;
            }   

            if (this.TableProperties.GroupedColumns.Count > 0)
            {
                var point = Mouse.GetPosition(this.GroupDropAreaGrid);
                var rowcolIndx = this.GroupDropAreaGrid.PointToCellRowColumnIndex(point);
                if (this.hitTestInfo.Grid.Model[rowcolIndx.RowIndex, rowcolIndx.ColumnIndex].CellType == "SortableHeaderCell")
                {
                    this.WasDragged = false;
                    this.StartDragHeaderPopup(this.hitTestInfo);
                    this.OpenDragIndicators(e.Location, this.hitTestInfo);
                }
            }
            else
            {
                this.hitTestInfo = null;
            }
        }

        /// <summary>
        /// MouseMove is called for the active controller after a MouseDown message when the user moves the mouse pointer.
        /// </summary>
        /// <param name="e">The <see cref="T:Syncfusion.Windows.Controls.Scroll.MouseControllerEventArgs"/> instance containing the event data.</param>
        public override void MouseMove(MouseControllerEventArgs e)
        {
            base.MouseMove(e);

            if (this.hitTestInfo == null)
            {
                return;
            }

            if (this.DragWindow != null && this.DragWindow.IsDragging)
            {
                this.UpdateDragIndicators(e.Location, this.hitTestInfo);
                this.WasDragged |= this.DragHeaderVisible;
            }
        }

        /// <summary>
        /// MouseUp is called for the active controller after a MouseDown message when the user releases the mouse button.
        /// </summary>
        /// <param name="e">The <see cref="T:Syncfusion.Windows.Controls.Scroll.MouseControllerEventArgs"/> instance containing the event data.</param>
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
                    this.Table.HideAllUIRows();

                    var sortColumn = this.TableProperties.GetSortColumnForGroup(groupColumn);
                    var visibleColumn = this.TableProperties.VisibleColumns[groupColumn.ColumnName];
                    if (visibleColumn != null && !visibleColumn.AllowSort)
                    {
                        return;
                    }

                    this.TableModel.View.BeginInit();
                    if (sortColumn != null)
                    {
                        int sortIndex = -1;
                        if (sortColumn.SortDirection == ListSortDirection.Descending && this.TableProperties.EnableTriStateSorting)
                        {
                            if(this.TableProperties.Model.Table.RaiseSortColumnsChanging(new List<GridDataSortColumn>(),new List<GridDataSortColumn>() { sortColumn }, NotifyCollectionChangedAction.Remove)) //Remove Column from sorting
                            {
                                sortIndex = this.TableProperties.SortColumns.IndexOf(sortColumn);
                                this.TableProperties.SortColumns.RemoveAt(sortIndex);
                                this.TableProperties.Model.Table.RaiseSortColumnsChanged(new List<GridDataSortColumn>(), new List<GridDataSortColumn>() { sortColumn }, NotifyCollectionChangedAction.Remove);
                            }                            
                        }
                        else
                        {
                            sortColumn.SortDirection = sortColumn.SortDirection == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
                            sortIndex = this.TableProperties.SortColumns.IndexOf(sortColumn);

                            this.TableProperties.SortColumns.RemoveAt(sortIndex);
                            if (visibleColumn == null || visibleColumn.AllowSort)
                            {
                                var sortCol = new GridDataSortColumn() { ColumnName = sortColumn.ColumnName, SortDirection = sortColumn.SortDirection };
                                if (this.TableProperties.Model.Table.RaiseSortColumnsChanging(new List<GridDataSortColumn>() { sortCol }, new List<GridDataSortColumn>(), NotifyCollectionChangedAction.Replace)) //Replace the column for sorting
                                {
                                    this.TableProperties.SortColumns.Insert(sortIndex, sortCol);
                                    this.TableProperties.Model.Table.RaiseSortColumnsChanged(new List<GridDataSortColumn>() { sortCol }, new List<GridDataSortColumn>(), NotifyCollectionChangedAction.Replace);
                                }
                            }
                        }
                    }
                    else
                    {

                        if (visibleColumn == null || visibleColumn.AllowSort)
                        {
                            var sortCol = new GridDataSortColumn() { ColumnName = groupColumn.ColumnName, SortDirection = ListSortDirection.Ascending };
                            if (this.TableProperties.Model.Table.RaiseSortColumnsChanging(new List<GridDataSortColumn>() { sortCol }, new List<GridDataSortColumn>(), NotifyCollectionChangedAction.Add)) // Add new column for sorting
                            {
                                this.TableProperties.SortColumns.Add(sortCol);
                                this.TableProperties.Model.Table.RaiseSortColumnsChanged(new List<GridDataSortColumn>() { sortCol }, new List<GridDataSortColumn>(), NotifyCollectionChangedAction.Add);
                            }
                        }
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
                        this.Grid.CurrentCell.Deactivate();
                        var visibleColumn = this.TableProperties.VisibleColumns.FirstOrDefault(o => o.MappingName == groupColumn.ColumnName);
                        // var isVisibleColumnNull = visibleColumn == null; Unused local variable
                        if (visibleColumn == null)
                        {
                            visibleColumn = new GridDataVisibleColumn()
                            {
                                MappingName = groupColumn.ColumnName,
                                AllowDrag = this.TableProperties.AllowDragColumns,
                                AllowResize = this.TableProperties.AllowResizeColumns,
                                ShowColumnOptions = this.TableProperties.ShowColumnOptions,
                                AllowFilter = this.TableProperties.ShowFilters,
                                AllowGroup = this.TableProperties.AllowGroup,
                                AllowSort = this.TableProperties.AllowSort
                            };
                            this.TableProperties.VisibleColumns.Add(visibleColumn);
                        }
                        var ActualColumnIndex = this.TableModel.ResolveVisibleColumnIndexToPosition(this.TableProperties.VisibleColumns.IndexOf(visibleColumn));
                        
                        if (this.Grid.RaiseGridDataQueryAllowDragColumn(ActualColumnIndex, this.TargetDragColumnIndex, GridQueryDragColumnHeaderReason.MouseUp, action))
                        {
                            // get the maxlevel to know the indent level before it is removed, once it is removed, the indent levels will be refreshed
                            var indentLevels = this.Table.GroupModel.GetMaxLevel();
                            var indentEqualsTotalGroups = this.TableProperties.GroupedColumns.Count == indentLevels;

                            this.TableModel.IsInGroup = true;
                            this.TableModel.View.BeginInit();
                            this.TableProperties.GroupedColumns.Remove(groupColumn);
                            var sortColumn = this.TableProperties.GetSortColumnForGroup(groupColumn);
                            if (sortColumn != null)
                            {
                                if (!this.TableProperties.RetainSortWhenUnGrouped)
                                {
                                    this.TableProperties.SortColumns.Remove(sortColumn);
                                }
                            }

                            //Resets nested lines in ILizeSizeHost
                            if (this.Table.HasNestedTables || this.Table.HasDetailsView)
                            {
                                var lines = this.TableModel.RowHeights as LineSizeCollection;
                                lines.ResetNestedLines();

                                //Clearing the DetailsViewRow cache to avoid rending of Details view cell on refreshing rows
                                this.TableModel.DetailsViewRows.Clear();
                            }

                            this.TableModel.View.EndInit();
                            this.TableModel.IsInGroup = false;
                            
                            var visibleColIndex = this.TableProperties.VisibleColumns.IndexOf(visibleColumn);
                            var actualGridColIndex = this.TableModel.ResolveVisibleColumnIndexToPosition(visibleColIndex);
                            var toGridColIndex = this.TargetDragColumnIndex;

                            //SD16705 - Fix for the targetted column is greater than the current column index.
                            if (this.TargetDragColumnIndex > this.hitTestInfo.RowColumnIndex.ColumnIndex)
                                toGridColIndex--;

                            // when column was hidden then check as below
                            //if (this.TableProperties.HideColumnsWhenGrouped && toGridColIndex <= actualGridColIndex)
                            //{
                            //    toGridColIndex += 1; // the target drag col would be one less since the visible line would be hidden for this column
                            //}

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
                        }
                    }
                    else
                    {
                        //Resets nested lines in ILizeSizeHost
                        if (this.Table.HasNestedTables || this.Table.HasDetailsView)
                        {
                            var lines = this.TableModel.RowHeights as LineSizeCollection;
                            lines.ResetNestedLines();
                            //Clearing the DetailsViewRow cache to avoid rending of Details view cell on refreshing rows
                            this.TableModel.DetailsViewRows.Clear();
                        }
                        if (this.GroupedColumnIndex == this.TargetDragGroupColumnIndex || this.GroupedColumnIndex+1 == this.TargetDragGroupColumnIndex )//|| this.GroupedColumnIndex -1 == this.TargetDragGroupColumnIndex)
                            this.Table.RefreshHiddenColumns();
                        else
                        {
                            int target = this.TargetDragGroupColumnIndex;
                            if (this.GroupedColumnIndex <= this.TargetDragGroupColumnIndex)
                                target -= 1;

                            this.TableModel.IsInGroup = true;
                            if(this.TableProperties.Model.View != null)
                                this.TableProperties.Model.View.BeginInit();
                            this.TableProperties.GroupedColumns.Remove(groupColumn);
                            if (target >= this.TableProperties.GroupedColumns.Count)
                            {
                                this.TableProperties.GroupedColumns.Add(groupColumn);
                            }
                            else
                            {
                                this.TableProperties.GroupedColumns.Insert(target, groupColumn);
                            }
                            if (this.TableProperties.Model.View != null)
                                this.TableProperties.Model.View.EndInit();
                            this.Table.RefreshHiddenColumns();
                        }
                    }
                }
            }

            if (this.WasDragged)
                this.TableModel.IsInGroup = true;

            this.TableModel.UpdateSelectedRanges();
            this.TableModel.IsInGroup = false;

            this.TableModel.InvalidateDisplay();

            if (this.DragWindow != null && this.DragWindow.IsDragging)
            {
                this.DragWindow.StopDrag();
                this.CloseDragIndicators();
            }
            if (this.DragWindow != null)
            {
                this.DragWindow.Dispose();
                this.DragWindow = null;
            }
            this.hitTestInfo = null;
        }

        #endregion


        public override void Dispose()
        {
            base.Dispose();
        }
    }
}