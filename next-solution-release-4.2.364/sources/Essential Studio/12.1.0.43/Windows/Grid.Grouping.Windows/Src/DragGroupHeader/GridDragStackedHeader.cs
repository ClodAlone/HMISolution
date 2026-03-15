//-------------------------------------------------------------------------------------------------
// <copyright file="GridDragStackedHeader.cs" company="syncfusion">
// Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//
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
using System.Windows.Forms;

using Syncfusion.Diagnostics;
using Syncfusion.Drawing;
using Syncfusion.Grouping;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Grid;
using Syncfusion.Windows.Forms.Grid.Grouping;

namespace Syncfusion.Windows.Forms.Grid.Grouping
{
    /// <summary>
    /// This is an abstract base class used by <see cref="GridTableControlDragHeaderMouseController"/> and <see cref="GroupDropAreaDragHeaderMouseController"/>.
    /// It implements the IMouseController interface
    /// to be used with MouseControllerDispatcher and provides common functions for hit-testing and drag and drop
    /// functionality used by both derived classes.
    /// </summary>
    public class GroupDragStackedHeaderMouseController : IMouseController, IGridFocusHelper
    {
        #region Fields
        internal Rectangle headerSectionBounds = Rectangle.Empty;  //// area where default cursor is shown

        internal GridTableControl _grid;
        internal GridStackedHeaderSpan stackedHeaderSpan;
        internal bool wasDragged = false;  //// otherwise raise click event.
        internal Rectangle originBounds = Rectangle.Empty;

        private GroupDragHelper headerDragHelper = null;

        internal Bitmap dragHeaderBitmap = null;

        internal Point origin = Point.Empty;
        bool dragHeaderVisible = false;

        internal Cursor cursor = null;
        internal int _rowIndex, _colIndex;

        Point hiddenPoint = new Point(10000, 10000);
        GroupDragHelper redArrowIndicatorDragHelper = null;

        internal GridClickCellsMouseController clickCellsController;
        int clickCellsControllerHitTest = 0;

        DragStackedHeaderHitTestInfo hitTestInfo = null;
        GridRangeInfo _cellRange;

        #endregion

        /// <overload>
        /// Initializes the object
        /// </overload>
        /// <summary>
        /// Initializes the object
        /// </summary>
        public GroupDragStackedHeaderMouseController()
        {
        }

        /// <summary>
        /// Initializes the object with the grid it is bound to.
        /// </summary>
        /// <param name="grid">The table control.</param>
        public GroupDragStackedHeaderMouseController(GridTableControl grid)
        {
            this._grid = grid;
            clickCellsController = new GridClickCellsMouseController(grid);
        }

        #region Helper Methods

        /// <summary>
        /// Returns the grid it is bound to.
        /// </summary>
        /// <returns>The GridTableControl</returns>
        protected virtual GridTableControl GetGridTableControl()
        {
            return _grid;
        }

        internal GridTableControl grid
        {
            get
            {
                return GetGridTableControl();
            }
        }

        /// <summary>
        /// Gets the <see cref="GridClickCellsMouseController"/> which is used to forward mouse events
        /// to cell renderers.
        /// </summary>
        public GridClickCellsMouseController ClickCellsController
        {
            get
            {
                return clickCellsController;
            }
        }

        /// <summary>
        /// Clears out pending state of ClickCellsController, possibly sending MouseHoverLeave or CancelMode
        /// notification to cell renderer.
        /// </summary>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude]
        public void ResetClickCellsController()
        {
            if (this.entered)
            {
                clickCellsController.MouseHoverLeave(EventArgs.Empty);
            }
            else if (clickCellsControllerHitTest != 0)
            {
                clickCellsController.CancelMode();
            }

            entered = false;
            clickCellsControllerHitTest = 0;
        }

        /// <summary>
        /// The TableDescriptor
        /// </summary>
        public GridTableDescriptor TableDescriptor
        {
            get
            {
                return grid.Table.TableDescriptor;
            }
        }

        /// <summary>
        /// Check for the mouse button clicked.
        /// </summary>
        /// <param name="e">A <see cref="MouseEventArgs"/> holding the event data.</param>
        /// <returns>boolean value</returns>
        protected virtual bool CheckMouseButtons(MouseEventArgs e)
        {
            return e.Button != MouseButtons.None;
        }

        #endregion

        #region Column Header Section

        /// <summary>
        /// The Grid Table
        /// </summary>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude]
        public GridTable Table
        {
            get
            {
                if (grid != null && grid.Model != null)
                {
                    return grid.Model.Table;
                }

                return null;
            }
        }

        internal GridRangeInfo GetRangeOfStackedHeaderSection()
        {
            if (Table != null)
            {
                return Table.GetRangeOfStackedHeaderSection();
            }

            return GridRangeInfo.Empty;
        }

        /// <summary>
        /// The stackedHeaderSpan descriptor under the mouse position. Will be null if there is no header drawn below mouse.
        /// </summary>
        /// <returns>returns the GridStackedHeaderSpan</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude]
        internal GridStackedHeaderSpan GetStackedHeaderSpanAtMousePosition()
        {
            Point pt = Control.MousePosition;
            pt = grid.GridPointToClient(pt);
            return grid.GetStackedHeaderSpanAt(pt);
        }

        #endregion

        #region Red Arrow Indicator

        internal static bool SupportsTransparentForm()
        {
            //// Check if this is 2000 (NT 5.0) or XP (NT 5.1)
            if (Environment.OSVersion.Platform != PlatformID.Win32NT
                || Environment.OSVersion.Version.Major < 5)
            {
                return false;
            }

            return true;
        }

        internal void OpenRedArrowIndicator()
        {
            Bitmap bm = CreateRedArrowIndicatorBitmap();
            redArrowIndicatorDragHelper = new GroupDragHelper();
            redArrowIndicatorDragHelper.StartDrag(bm, hiddenPoint, DragDropEffects.Move);
            // There is always a small black rectangle at the lower right-corner. Drawing
            // over the previously painted BackgroundImage resolves this issue.
            redArrowIndicatorDragHelper.DragWindow.Paint += new PaintEventHandler(DragWindow_Paint);
            if (!GroupDragHeaderMouseControllerBase.SupportsTransparentForm())
            {
                GridRangeInfo rows = GetRangeOfStackedHeaderSection();
                int rowIndex = rows.Top;
                GridStyleInfo style = grid.Model[rowIndex, 0];
                Color backColor = Color.FromArgb(255, style.Interior.BackColor);
                redArrowIndicatorDragHelper.DragWindow.BackColor = backColor;
            }
        }

        internal void CloseRedArrowIndicator()
        {
            if (redArrowIndicatorDragHelper != null)
            {
                redArrowIndicatorDragHelper.EndDrag();
                redArrowIndicatorDragHelper.DragWindow.Paint -= new PaintEventHandler(DragWindow_Paint);
                redArrowIndicatorDragHelper = null;
            }
        }

        private Bitmap CreateRedArrowIndicatorBitmap()
        {
            Bitmap bm = null;
            Graphics g = null;
            GridRangeInfo rows = GetRangeOfStackedHeaderSection();
            int rowIndex = rows.Top;
            Bitmap downBitmap = GridGroupingBitmaps.RedDownBitmap;
            Bitmap upBitmap = GridGroupingBitmaps.RedUpBitmap;

            Size size = new Size(downBitmap.Width, grid.GetRowHeight(rowIndex) + (downBitmap.Height * 2) - 1);
            Rectangle bounds = new Rectangle(Point.Empty, size);

            try
            {
                bm = new Bitmap(size.Width, size.Height);
                g = Graphics.FromImage(bm);
                g.FillRectangle(new SolidBrush(Color.Red), new Rectangle(Point.Empty, size));
                g.DrawImageUnscaled(upBitmap, 0, bm.Height - upBitmap.Height);
                g.DrawImageUnscaled(downBitmap, 1, 0);
            }
            finally
            {
                if (g != null)
                {
                    g.Dispose();
                }
            }

            bm.MakeTransparent(Color.Red);
            return bm;
        }

        private void DrawRedArrowIndicator(Graphics g)
        {
            // There is always a small black rectangle at the lower right-corner. Drawing
            // over the previously painted BackgroundImage resolves this issue.
            GridRangeInfo rows = GetRangeOfStackedHeaderSection();
            int rowIndex = rows.Top;
            Bitmap downBitmap = GridGroupingBitmaps.RedDownBitmap;
            Bitmap upBitmap = GridGroupingBitmaps.RedUpBitmap;

            Size size = new Size(downBitmap.Width, grid.GetRowHeight(rowIndex) + (downBitmap.Height * 2) - 1);
            Rectangle bounds = new Rectangle(Point.Empty, size);

            Color backColor = Color.Red;

            if (!GroupDragHeaderMouseControllerBase.SupportsTransparentForm())
            {
                GridStyleInfo style = grid.Model[rowIndex, 0];
                backColor = Color.FromArgb(255, style.Interior.BackColor);
            }

            try
            {
                g.FillRectangle(new SolidBrush(backColor), new Rectangle(Point.Empty, size));
                g.DrawImageUnscaled(upBitmap, 0, size.Height - upBitmap.Height);
                g.DrawImageUnscaled(downBitmap, 0, 0);
            }
            finally
            {
            }
        }

        private void DragWindow_Paint(object sender, PaintEventArgs e)
        {
            this.DrawRedArrowIndicator(e.Graphics);
        }

        static bool firstShowRedArrowIndicator = true;

        /// <summary>
        /// Determines based on mouse position if red arrow indicator should be shown
        /// </summary>
        /// <returns>True if the indicator should be shown.</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude]
        public bool ShouldShowRedArrowIndicator()
        {
            GridStackedHeaderSpan cd = GetStackedHeaderSpanAtMousePosition();

            GridStackedHeaderRowDescriptor parentRow = stackedHeaderSpan.parentRow;
            if (cd != null)
            {
                if (cd.header == null || (cd.parentRow == parentRow && cd != stackedHeaderSpan))
                {
                    if (cd.index != stackedHeaderSpan.index + 1)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Used internally.
        /// </summary>
        /// <param name="cd">The GridStackedHeaderSpan.</param>
        /// <returns>returns the point for RedArrowIndicatorLocation</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude]
        public Point GetRedArrowIndicatorLocation(GridStackedHeaderSpan cd)
        {
            if (cd != null)
            {
                GridRangeInfo range = grid.Table.GetRangeOfStackedHeaderSpan(cd);
                if (!range.IsEmpty)
                {
                    int colIndex = range.Left;
                    Rectangle r = grid.RangeInfoToRectangle(GridRangeInfo.Cell(redArrowRowIndex, colIndex));
                    Point pt = grid.IsRightToLeft() ? new Point(r.Right, r.Top) : r.Location;
                    return grid.GridPointToScreen(pt);
                }
            }

            return Point.Empty;
        }

        internal int redArrowFieldNum = 0;
        internal int redArrowRowIndex = 0;

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude]
        public void UpdateRedArrowIndicator()
        {
            Point pt = Control.MousePosition;

            pt = grid.GridPointToClient(pt);
            GridControlBase gridWindow = grid.GetGridWindow();
            if (pt.X < gridWindow.GridBounds.Left || pt.X > gridWindow.GridBounds.Right)
            {
                goto noArrow;
            }

            GridStackedHeaderSpan cd = grid.GetStackedHeaderSpanAt(pt);

            if (this.ShouldShowRedArrowIndicator())
            {
                ////                    if (pt.X > gridWindow.Right)
                ////                    {
                ////                        int delta = pt.X + gridWindow.Right + 5;
                ////                        gridWindow.SetCurrentHScrollPixelPos(gridWindow.GetCurrentHScrollPixelPos() + delta);
                ////                        pt.X -= delta;
                ////                    }

                Bitmap downBitmap = GridGroupingBitmaps.RedDownBitmap;
                Bitmap upBitmap = GridGroupingBitmaps.RedUpBitmap;

                GridTableDescriptor td = Table.TableDescriptor;

                redArrowFieldNum = cd.firstCol;
                redArrowRowIndex = gridWindow.GetRow(gridWindow.ViewLayout.PointToClientRow(pt, GridCellSizeKind.ActualSize));

                Point p = Point.Empty;
                ////GridQueryAllowDragColumnEventArgs ae = new GridQueryAllowDragColumnEventArgs(this.grid, stackedHeaderSpan.Name, cd.Name, GridQueryAllowDragColumnReason.ShowRedArrowIndicator);
                ////this.grid.RaiseQueryAllowDragColumn(ae);
                ////if (!ae.AllowDrag)
                ////{
                ////    cd = null;
                ////    pt = hiddenPoint;
                ////}
                ////else
                {
                    p = GetRedArrowIndicatorLocation(cd);

                    p.X -= downBitmap.Width / 2;
                    p.Y -= downBitmap.Height;

                    pt = grid.GridPointToClient(p);
                }

                if (redArrowIndicatorDragHelper != null)
                {
                    if (pt.X < (gridWindow.GridBounds.Left - (downBitmap.Width / 2)) || pt.X > gridWindow.GridBounds.Right)
                    {
                        redArrowIndicatorDragHelper.DoDrag(hiddenPoint, DragDropEffects.Copy);
                    }
                    else
                    {
                        redArrowIndicatorDragHelper.DoDrag(p, DragDropEffects.Copy);
                        redArrowIndicatorDragHelper.DragWindow.ShowWindowTopMost();

                        //// Fixes an issue with transparency that occurs occasionally when shown
                        //// for the first time.
                        if (firstShowRedArrowIndicator)
                        {
                            firstShowRedArrowIndicator = false;
                            redArrowIndicatorDragHelper.DoDrag(hiddenPoint, DragDropEffects.Copy);
                            redArrowFieldNum = -1;
                            UpdateRedArrowIndicator();
                        }
                    }
                }

                return;
            }

        noArrow:
            if (redArrowIndicatorDragHelper != null)
            {
                redArrowIndicatorDragHelper.DoDrag(hiddenPoint, DragDropEffects.Copy);
            }

            redArrowFieldNum = -1;
        }

        #endregion

        #region Drag Header

        /// <summary>
        /// Gets the movement delta.
        /// </summary>
        /// <returns>returns the point</returns>
        internal Point GetMovementDelta()
        {
            Point pt = Control.MousePosition;
            return new Point(pt.X - origin.X, pt.Y - origin.Y);
        }

        /// <summary>
        /// Used internally.
        /// </summary>
        /// <returns>return boolean value to indicate ShouldShowDragBitmap</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude]
        public bool ShouldShowDragBitmap()
        {
            return !originBounds.Contains(Control.MousePosition);
        }

        internal Point GetDragWindowLocation(Point srcLocation)
        {
            Point pt = Control.MousePosition;
            return new Point(srcLocation.X + pt.X - origin.X, srcLocation.Y + pt.Y - origin.Y);
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude]
        public bool DragHeaderVisible
        {
            get
            {
                return dragHeaderVisible;
            }

            set
            {
                if (dragHeaderVisible != value)
                {
                    dragHeaderVisible = value;
                    if (value)
                    {
                        OpenDragHeader();
                    }
                    else
                    {
                        CloseDragHeader();
                    }
                }
            }
        }

        internal void OpenDragHeader()
        {
            Bitmap bm;
            bm = CreateHeaderBitmap(grid, _rowIndex, _colIndex);

            headerDragHelper = new GroupDragHelper();
            cursor = Cursors.Default;
            headerDragHelper.StartDrag(bm, hiddenPoint, DragDropEffects.Move);
        }

        internal void CloseDragHeader()
        {
            if (headerDragHelper != null)
            {
                headerDragHelper.EndDrag();
                headerDragHelper = null;
            }
        }

        internal void UpdateDragHeader()
        {
            Point mousePos = Control.MousePosition;
            DragHeaderVisible = !this.originBounds.Contains(mousePos);

            if (!DragHeaderVisible)
            {
                return;
            }

            this.headerDragHelper.DragWindow.WindowCursor = Cursor;
            Point pt = GetDragWindowLocation(originBounds.Location);
            this.headerDragHelper.DoDrag(pt, DragDropEffects.Move);
            if (redArrowIndicatorDragHelper != null)
            {
                redArrowIndicatorDragHelper.DragWindow.ShowWindowTopMost();
            }
        }

        private Bitmap CreateHeaderBitmap(GridControlBase grid, int rowIndex, int colIndex)
        {
            Graphics g = null;
            GridRangeInfo range = grid.Model.CoveredRanges.FindRange(rowIndex, colIndex);
            Size size = new Size(
                grid.ViewLayout.GetColRangeWidth(range.Left, range.Right, GridCellSizeKind.ActualSize),
                grid.GetRowHeight(rowIndex));
            Rectangle bounds = new Rectangle(Point.Empty, size);
            GridStyleInfo style = grid.Model[rowIndex, colIndex];
            GridCellRendererBase headerCellRenderer = grid.CellRenderers[style.CellType];
            Bitmap bm = new Bitmap(Math.Max(1, size.Width), Math.Max(1, size.Height));

            try
            {
                g = Graphics.FromImage(bm);
                BrushPaint.FillRectangle(g, bounds, style.Interior);
                headerCellRenderer.Draw(g, bounds, rowIndex, colIndex, style);
            }
            finally
            {
                if (g != null)
                {
                    g.Dispose();
                }
            }

            return bm;
        }
        
        #endregion

        #region IGridFocusHelper Implementation

        /// <implement/>
        /// <summary>Gets allow fix focus.</summary>
        /// <returns>returns False.</returns>
        public virtual bool GetAllowFixFocus()
        {
            return false;
        }

        #endregion

        #region IMouseController implementation

        /// <implement/>
        /// <summary>Gets the name of the mouse controller.</summary>
        public virtual string Name
        {
            get
            {
                return "DragStackedHeader";
            }
        }

        /// <implement/>
        /// <summary>Specifies the cursor to be displayed.</summary>
        public virtual Cursor Cursor
        {
            get
            {
                if (cursor != null)
                {
                    return cursor;
                }

                return clickCellsController.Cursor;
            }
        }

        bool entered = false;

        /// <implement/>
        /// <summary>
        /// Called before the first time MouseHover is called.
        /// </summary>
        public virtual void MouseHoverEnter()
        {
            if (clickCellsControllerHitTest != 0)
            {
                entered = true;
                clickCellsController.MouseHoverEnter();
            }
        }

        /// <implement/>
        /// <summary>
        /// Called after MouseHoverEnter is called.
        /// </summary>
        /// <param name="e">A <see cref="MouseEventArgs"/> holding the event data.</param>
        public virtual void MouseHover(MouseEventArgs e)
        {
            if (clickCellsControllerHitTest != 0)
            {
                clickCellsController.MouseHover(e);
            }
        }

        /// <implement/>
        /// <summary>
        /// MouseHoverLeave is called when hovering ends either because user dragged mouse out of the hit-test area or
        /// when context changes (e.g. user pressed the mouse button).
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> holding event data.</param>
        public virtual void MouseHoverLeave(EventArgs e)
        {
            if (entered)
            {
                clickCellsController.MouseHoverLeave(e);
                entered = false;
            }
        }

        /// <implement/>
        /// <summary>
        /// MouseDown is called when this controller signaled in HitTest that it wants to handle mouse events and the
        /// user pressed the mouse button.
        /// </summary>
        /// <param name="e">A <see cref="MouseEventArgs"/> holding event data.</param>
        public virtual void MouseDown(MouseEventArgs e)
        {
            if (this.hitTestInfo == null)
            {
                if (clickCellsControllerHitTest != 0)
                {
                    clickCellsController.MouseDown(e);
                }

                return;
            }

            this.ResetClickCellsController();

            if (e.Clicks == 2)
            {
                int rowIndex, colIndex;
                grid.PointToRowCol(new Point(e.X, e.Y), out rowIndex, out colIndex);
                grid.RaiseCellDoubleClick(rowIndex, colIndex, e);
                return;
            }

            this.OpenDragHeader();
            this.OpenRedArrowIndicator();
            grid.PrepareViewStyleInfo += new GridPrepareViewStyleInfoEventHandler(grid_PrepareViewStyleInfo);
            grid.InvalidateRange(this._cellRange);
            grid.GetGridWindow().AutoScrolling = ScrollBars.Horizontal;
            grid.GetGridWindow().AutoScrollBounds = Rectangle.Empty;
        }

        /// <implement/>
        /// <summary>
        /// MouseMove is called for the active controller after a MouseDown message when the user moves the mouse pointer.
        /// </summary>
        /// <param name="e">A <see cref="MouseEventArgs"/> holding event data.</param>
        public virtual void MouseMove(MouseEventArgs e)
        {
            if (this.hitTestInfo == null)
            {
                if (clickCellsControllerHitTest != 0)
                {
                    clickCellsController.MouseMove(e);
                }

                return;
            }

            this.ResetClickCellsController();

            cursor = Cursors.Default;

            this.UpdateDragHeader();
            this.UpdateRedArrowIndicator();

            this.wasDragged |= DragHeaderVisible;
        }

        /// <implement/>
        /// <summary>
        /// MouseUp is called for the active controller after a MouseDown message when the user releases the mouse button.
        /// </summary>
        /// <param name="e">A <see cref="MouseEventArgs"/> holding event data.</param>
        public virtual void MouseUp(MouseEventArgs e)
        {
            if (this.hitTestInfo == null)
            {
                if (clickCellsControllerHitTest != 0)
                {
                    clickCellsController.MouseUp(e);
                }

                return;
            }

            this.ResetClickCellsController();

            cursor = null;
            bool bv = DragHeaderVisible;

            DragHeaderVisible = false;
            this.CloseDragHeader();
            this.CloseRedArrowIndicator();

            grid.GetGridWindow().AutoScrolling = ScrollBars.None;
            grid.GetGridWindow().Capture = false;
            grid.PrepareViewStyleInfo -= new GridPrepareViewStyleInfoEventHandler(grid_PrepareViewStyleInfo);
            grid.InvalidateRange(this.GetRangeOfStackedHeaderSection());

            if (!wasDragged)
            {
                grid.RaiseCellClick(_rowIndex, _colIndex, e);
            }
            else if (bv)
            {
                if (ShouldShowRedArrowIndicator() && !headerSectionBounds.IsEmpty)
                {
                    string redArrowColumnName = string.Empty;
                    GridStackedHeaderSpan targetSpan = null;
                    if (redArrowFieldNum >= 0)
                    {
                        targetSpan = this.stackedHeaderSpan.parentRow.GetStackedHeaderSpanAt(redArrowFieldNum);
                        if (targetSpan.header != null)
                        {
                            redArrowColumnName = targetSpan.header.Name;
                        }
                    }
                    string columnName = stackedHeaderSpan.VisibleColumns[0].ToString();
                    string targetColumnName = targetSpan.VisibleColumns[0].ToString();
                    if (columnName != null)
                    {
                        GridQueryAllowDragColumnEventArgs ae = new GridQueryAllowDragColumnEventArgs(this.grid, columnName, stackedHeaderSpan.Header.Name, targetColumnName, GridQueryAllowDragColumnReason.MouseUp);
                        this.grid.RaiseQueryAllowDragColumn(ae);
                        if (ae.AllowDrag)
                        {
                            Table.RaiseDisplayElementChanging(Table, -1, -1, true, true, true, false);
                            int targetCol = targetSpan.firstCol;
                            int sourceCol = stackedHeaderSpan.firstCol;
                            if (sourceCol < targetCol)
                            {
                                targetCol--;
                            }

                            Cursor.Current = Cursors.WaitCursor;
                            int count = stackedHeaderSpan.lastCol - stackedHeaderSpan.firstCol + 1;
                            for (int n = 0; n < count; n++)
                            {
                                if (TableDescriptor.VisibleColumns.IsModified)
                                {
                                    TableDescriptor.VisibleColumns.Move(sourceCol, targetCol);
                                }
                                else
                                {
                                    TableDescriptor.Columns.Move(sourceCol, targetCol);
                                }

                                if (targetCol < sourceCol)
                                {
                                    sourceCol++;
                                    targetCol++;
                                }
                            }

                            Table.RaiseDisplayElementChanged(Table, -1, -1, true, true, true, false);
                        }
                    }
                    grid.Update();
                    grid.LeftColIndex = grid.Model.Cols.FrozenCount + 1;
                    Cursor.Current = Cursors.Default;
                }
            }
        }

        /// <implement/>
        ///  <summary>
        /// CancelMode is called for the active controller after a MouseDown message when the mouse operation is cancelled.
        /// </summary>
        public virtual void CancelMode()
        {
            if (clickCellsControllerHitTest != 0)
            {
                clickCellsController.CancelMode();
            }

            if (this.hitTestInfo == null)
            {
                return;
            }

            cursor = null;
            DragHeaderVisible = false;
            this.CloseDragHeader();
            this.CloseRedArrowIndicator();

            grid.GetGridWindow().AutoScrolling = ScrollBars.None;
            grid.GetGridWindow().Capture = false;
            grid.GetGridWindow().AutoScrolling = ScrollBars.None;
            grid.GetGridWindow().Capture = false;
            grid.PrepareViewStyleInfo -= new GridPrepareViewStyleInfoEventHandler(grid_PrepareViewStyleInfo);
            grid.InvalidateRange(this.GetRangeOfStackedHeaderSection());
        }

        /// <implement/>
        /// <summary>
        /// HitTest is called to determine whether your controller wants to handle the mouse events based current context.
        /// </summary>
        /// <param name="e">A <see cref="MouseEventArgs"/> holding event data.</param>
        /// <param name="controller">A <see cref="IMouseController"/> that has indicated to handle the mouse event.</param>
        /// <returns>A non-zero value if the button can and wants to handle the mouse event; 0 if the
        /// mouse event is unrelated for this button.</returns>
        public virtual int HitTest(MouseEventArgs e, IMouseController controller)
        {
            // This HitTest code has higher priority than "SelectCells"
            Point pt = new Point(e.X, e.Y);
            hitTestInfo = null;

            if (!Table.TableOptions.AllowDragColumns)
            {
                return 0;
            }

            ////&& e.Clicks < 2
            if (CheckMouseButtons(e) && (controller == null || controller.Name != "ResizeCells"))
            {
                hitTestInfo = new DragStackedHeaderHitTestInfo(grid, pt);
                if (hitTestInfo.hitTestResult == GridHitTestContext.None
                    || hitTestInfo.stackedHeaderSpan.header == null)
                {
                    hitTestInfo = null;
                }
                else
                {
                    string columnName = null;
                    int row = -1, col = -1;
                    if (this.grid.PointToRowCol(e.Location, out row, out col))
                    {
                        GridTableCellStyleInfo style = this.grid.GetTableViewStyleInfo(row, col);
                        if (style.TableCellIdentity.DisplayElement.Kind == DisplayElementKind.StackedHeader && hitTestInfo.stackedHeaderSpan.Header.VisibleColumns.Count > 0)
                        {
                            columnName = hitTestInfo.stackedHeaderSpan.Header.VisibleColumns[0].Name;
                        }
                    }
                    if (columnName == null)
                    {
                        hitTestInfo = null;
                        return 0;
                    }
                    GridQueryAllowDragColumnEventArgs ae = new GridQueryAllowDragColumnEventArgs(this.grid, this.grid.TableDescriptor.Columns[columnName],hitTestInfo.stackedHeaderSpan.Header, GridQueryAllowDragColumnReason.HitTest, e);
                    ae.AllowDrag = e.Button == MouseButtons.Left;
                    this.grid.RaiseQueryAllowDragColumn(ae);
                    if (ae.AllowDrag)
                    {
                        this._rowIndex = hitTestInfo.rowIndex;
                        this._colIndex = hitTestInfo.colIndex;
                        this._cellRange = hitTestInfo.cellRange;
                        this.originBounds = grid.GridRectangleToScreen(grid.RangeInfoToRectangle(_cellRange));
                        this.origin = this.grid.GetGridWindow().LastMousePosition;
                        this.headerSectionBounds = grid.GridRectangleToScreen(grid.RangeInfoToRectangle(this.GetRangeOfStackedHeaderSection()));
                        this.stackedHeaderSpan = hitTestInfo.stackedHeaderSpan;
                        wasDragged = false;
                    }
                    else
                    {
                        hitTestInfo = null;
                    }
                }
            }

            if (hitTestInfo != null)
            {
                clickCellsControllerHitTest = clickCellsController.HitTest(e, controller);
                return hitTestInfo.hitTestResult;
            }

            return 0;
        }
        
        [Syncfusion.Documentation.DocumentationExclude()]
        internal sealed class DragStackedHeaderHitTestInfo
        {
            internal GridRangeInfo cellRange;
            internal int rowIndex;
            internal int colIndex;
            internal int hitTestResult = GridHitTestContext.None;
            internal Point point;
            internal GridStackedHeaderSpan stackedHeaderSpan;
            ////internal int fieldNum;

            internal DragStackedHeaderHitTestInfo(GridTableControl grid, Point point)
            {
                if (grid.HasTable && (grid.Table.IsNewUniformChildListRelation() || grid.Table.SourceList != null) && grid.GridBounds.Contains(point))
                {
                    ////if (grid.Model.Table.TableLevel > -1)
                    ////    Trace.WriteLine(point);
                    cellRange = grid.PointToRangeInfo(point, -1);
                    rowIndex = cellRange.Top;
                    colIndex = cellRange.Left;
                    if (rowIndex < grid.Model.Table.NestedDisplayElements.Count)
                    {
                        Element el = grid.Model.Table.NestedDisplayElements[rowIndex];
                        if (el.ParentTable == grid.Model.Table && (el is GridStackedHeaderRow || el is GridStackedHeaderSection))
                        {
                            this.point = point;
                            ////fieldNum = grid.Model.ColIndexToField(colIndex);

                            stackedHeaderSpan = grid.Table.GetStackedHeaderSpanAt(el, colIndex);

                            if (stackedHeaderSpan != null && el != null)
                            {
                                hitTestResult = GridHitTestContext.Header;
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Grid Event Handlers

        private void grid_PrepareViewStyleInfo(object sender, GridPrepareViewStyleInfoEventArgs e)
        {
            ////if (this.hitTestInfo == null)
            ////    return;

            if (e.ColIndex == this._colIndex && e.RowIndex == this._rowIndex)
            {
                e.Style.CellAppearance = GridCellAppearance.Sunken;
            }
        }

        #endregion
    }
}