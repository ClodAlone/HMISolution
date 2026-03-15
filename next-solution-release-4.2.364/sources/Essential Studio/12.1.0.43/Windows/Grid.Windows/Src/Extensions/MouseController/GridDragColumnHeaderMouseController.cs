//-------------------------------------------------------------------------------------------------
// <copyright file="GridDragColumnHeaderMouseController.cs" company="syncfusion">
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
using System.Windows.Forms;

using Syncfusion.ComponentModel;
using Syncfusion.Diagnostics;
using Syncfusion.Drawing;
using Syncfusion.Windows.Forms;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// GridDragColumnHeaderBitmaps is a helper class for drawing and caching bitmaps 
    /// </summary>
    /// <remarks>
    /// The bitmaps are loaded from the manifest and cached. The PaintIcon routine
    /// will substitute black pixels of the original bitmap and draw them with a 
    /// specified forecolor.<para/>
    /// Example:<para/>
    ///    GridDragColumnHeaderBitmaps.IconPainter.PaintIcon(g, rect, Point.Empty, bitmapName, Color.Black);
    /// </remarks>
    public class GridDragColumnHeaderBitmaps
    {
        [ThreadStaticAttribute]
        static IconPaint iconPainter;

        static string RootNamespace = AssemblyInfo.RootNamespace + @".Extensions.MouseController";

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public GridDragColumnHeaderBitmaps()
            : base()
        {
        }

        internal static IconPaint IconPainter
        {
            get
            {
                if (iconPainter == null)
                {
                    iconPainter = new IconPaint(RootNamespace + @".", typeof(GridDragColumnHeaderBitmaps).Assembly);
                }

                return iconPainter;
            }
        }

        /// <summary>
        /// Gets bitmap for moving columns.
        /// </summary>
        public static Bitmap RedDownBitmap
        {
            get
            {
                return IconPainter.GetBitmap("RedDownArrow.bmp");
            }
        }

        /// <summary>
        /// Gets bitmap for moving columns.
        /// </summary>
        public static Bitmap RedUpBitmap
        {
            get
            {
                return IconPainter.GetBitmap("RedUpArrow.bmp");
            }
        }
    }

    /// <summary>
    /// This is an abstract base class used by <see cref="GridDragColumnHeaderMouseController"/>.
    /// It implements the IMouseController interface to be used with MouseControllerDispatcher and provides common functions for 
    /// hit-testing and drag and drop functionality used by the derived class.
    /// </summary>
    public abstract class GridDragColumnHeaderMouseControllerBase : IMouseController, IGridFocusHelper
    {
        #region Fields
        internal GridControlBaseImp _grid;
        internal int column;
        internal bool wasDragged = false;  // otherwise raise click event.
        internal Rectangle originBounds = Rectangle.Empty;

        private GridDragColumnHeaderHelper headerDragHelper = null;

        internal Bitmap dragHeaderBitmap = null;

        internal Point origin = Point.Empty;
        bool dragHeaderVisible = false;

        internal Cursor cursor = null;
        internal int _rowIndex, _colIndex;

        Point hiddenPoint = new Point(10000, 10000);
        GridDragColumnHeaderHelper redArrowIndicatorDragHelper = null;

        internal const int GroupDropAreaRowIndex = 2;

        internal GridClickCellsMouseController clickCellsController;
        int clickCellsControllerHitTest = 0;
        Bitmap tempHeaderBitmap = null;
        Bitmap tempArrowBitmap = null;
        #endregion

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

        /// <overload>
        /// Initializes the object
        /// </overload>
        /// <summary>
        /// Initializes the object
        /// </summary>
        public GridDragColumnHeaderMouseControllerBase()
        {
        }

        /// <summary>
        /// Initializes the object with the grid it is bound to.
        /// </summary>
        public GridDragColumnHeaderMouseControllerBase(GridControlBaseImp grid)
        {
            this._grid = grid;
        }

        /// <summary>
        /// Returns the grid it is bound to.
        /// </summary>
        /// <returns>The GridControlBase</returns>
        protected virtual GridControlBaseImp GetGridControl()
        {
            return _grid;
        }

        internal GridControlBaseImp grid
        {
            get
            {
                return GetGridControl();
            }
        }

        #region Helper Methods

        /// <summary>
        /// Determines based on mouse position if remove cursor should be shown
        /// </summary>
        /// <returns>returns boolean value</returns>
        /// <internalonly/>
        public bool ShouldShowRemoveCursor()
        {
            if (grid == null)
            {
                return false;
            }

            Point pt = Control.MousePosition;
            pt = grid.GridPointToClient(pt);
            ////            bool b1 = groupAreaBounds.Contains(pt);
            ////            bool b2 = headerSectionBounds.Contains(pt) || headerSectionBounds.IsEmpty;
            return !grid.GridBounds.Contains(pt);
        }

        internal bool CheckMouseButtons(MouseEventArgs e)
        {
            return e.Button == MouseButtons.Left;
        }

        #endregion

        #region Column Header Section

        internal GridRangeInfo GetRangeOfColumnHeaderSection()
        {
            if (grid != null)
            {
                return GridRangeInfo.Rows(0, this.grid.Model.Rows.HeaderCount);
            }

            return GridRangeInfo.Empty;
        }

        /// <summary>
        /// The column descriptor under the mouse position. Will be null if there is no header drawn below mouse.
        /// </summary>
        /// <returns>returns mouse position</returns>
        /// <internalonly/>
        public int GetColumnAtMousePosition()
        {
            Point pt = Control.MousePosition;
            pt = grid.GridPointToClient(pt);
            return grid.GetCol(grid.ViewLayout.PointToClientCol(pt, GridCellSizeKind.ActualSize));
        }

        #endregion

        #region Red Arrow Indicator

        internal static bool SupportsTransparentForm()
        {
            // Check if this is 2000 (NT 5.0) or XP (NT 5.1)
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
            redArrowIndicatorDragHelper = new GridDragColumnHeaderHelper();
            tempArrowBitmap = bm;
            // There is always a small black rectangle at the lower right-corner. Drawing
            // over the previously painted BackgroundImage resolves this issue.
            redArrowIndicatorDragHelper.DragWindow.Paint += new PaintEventHandler(DragWindow_Paint);
            if (!GridDragColumnHeaderMouseControllerBase.SupportsTransparentForm())
            {
                GridRangeInfo rows = GetRangeOfColumnHeaderSection();
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
                redArrowIndicatorDragHelper.Dispose();
                redArrowIndicatorDragHelper = null;
            }
        }

        private Bitmap CreateRedArrowIndicatorBitmap()
        {
            Bitmap bm = null;
            Graphics g = null;
            GridRangeInfo rows = GetRangeOfColumnHeaderSection();
            int rowIndex = rows.Top;
            Bitmap downBitmap = GridDragColumnHeaderBitmaps.RedDownBitmap;
            Bitmap upBitmap = GridDragColumnHeaderBitmaps.RedUpBitmap;

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
            GridRangeInfo rows = GetRangeOfColumnHeaderSection();
            int rowIndex = rows.Top;
            Bitmap downBitmap = GridDragColumnHeaderBitmaps.RedDownBitmap;
            Bitmap upBitmap = GridDragColumnHeaderBitmaps.RedUpBitmap;

            Size size = new Size(downBitmap.Width, grid.GetRowHeight(rowIndex) + (downBitmap.Height * 2) - 1);
            Rectangle bounds = new Rectangle(Point.Empty, size);

            Color backColor = Color.Red;

            if (!GridDragColumnHeaderMouseControllerBase.SupportsTransparentForm())
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
        /// <returns>returns boolean value</returns>
        /// <internalonly/>
        public bool ShouldShowRedArrowIndicator()
        {
            if (ShouldShowRemoveCursor())
            {
                return false;
            }

            return targetColIndex > grid.Model.Cols.HeaderCount && this.column != targetColIndex;
        }

        /// <summary>
        /// Gets the red arrow indicator location.
        /// </summary>
        /// <param name="colIndex">Index of the col.</param>
        /// <returns>returns point</returns>
        /// <internalonly/>
        public Point GetRedArrowIndicatorLocation(int colIndex)
        {
            if (colIndex >= 0)
            {
                Rectangle r = grid.RangeInfoToRectangle(GridRangeInfo.Cell(0, colIndex));
                Point pt = grid.IsRightToLeft() ? new Point(r.Right, r.Top) : r.Location;
                return grid.GridPointToScreen(pt);
            }

            return Point.Empty;
        }

        ////internal bool redArrowGroupDropArea = false;
        ////internal int redArrowFieldNum = 0;
        internal int targetColIndex = 0;

        /// <internalonly/>
        /// <summary>
        /// Used internally.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void UpdateRedArrowIndicator()
        {
            Point pt = Control.MousePosition;

            pt = grid.GridPointToClient(pt);
            GridControlBase gridWindow = grid.GetGridWindow();
            if (pt.X < gridWindow.GridBounds.Left || pt.X > gridWindow.GridBounds.Right)
            {
                goto noArrow;
            }

            targetColIndex = grid.GetCol(grid.ViewLayout.PointToClientCol(pt, GridCellSizeKind.ActualSize));

            if (this.ShouldShowRedArrowIndicator())
            {
                Bitmap downBitmap = GridDragColumnHeaderBitmaps.RedDownBitmap;
                Bitmap upBitmap = GridDragColumnHeaderBitmaps.RedUpBitmap;

                Point p = Point.Empty;
                GridQueryDragColumnHeaderEventArgs ae = new GridQueryDragColumnHeaderEventArgs(this.grid, _colIndex, targetColIndex, GridQueryDragColumnHeaderReason.ShowRedArrowIndicator);
                this.grid.RaiseQueryAllowDragColumn(ae);
                if (!ae.AllowDrag)
                {
                    pt = hiddenPoint;
                }
                else
                {
                    p = GetRedArrowIndicatorLocation(targetColIndex);

                    p.X -= downBitmap.Width / 2;
                    p.Y -= downBitmap.Height;

                    pt = grid.GridPointToClient(p);
                }

                if (redArrowIndicatorDragHelper != null)
                {
                    if (pt.X < gridWindow.GridBounds.Left - (downBitmap.Width / 2) || pt.X > gridWindow.GridBounds.Right)
                    {
                        redArrowIndicatorDragHelper.DoDrag(hiddenPoint, DragDropEffects.Copy);
                    }
                    else
                    {
                        if(!this.redArrowIndicatorDragHelper.IsDragging)
                            redArrowIndicatorDragHelper.StartDrag(tempArrowBitmap, hiddenPoint, DragDropEffects.Move);

                        redArrowIndicatorDragHelper.DoDrag(p, DragDropEffects.Copy);
                        redArrowIndicatorDragHelper.DragWindow.ShowWindowTopMost();

                        //// Fixes an issue with transparency that occurs occasionally when shown
                        //// for the first time.
                        if (firstShowRedArrowIndicator)
                        {
                            firstShowRedArrowIndicator = false;
                            redArrowIndicatorDragHelper.DoDrag(hiddenPoint, DragDropEffects.Copy);
                            ////redArrowFieldNum = -1;
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
            ////redArrowFieldNum = -1;
        }
        #endregion

        #region Drag Header

        /// <summary>
        /// Gets the movement delta.
        /// </summary>
        /// <returns>returns point</returns>
        internal Point GetMovementDelta()
        {
            Point pt = Control.MousePosition;
            return new Point(pt.X - origin.X, pt.Y - origin.Y);
        }

        /// <summary>
        /// Shoulds the show drag bitmap.
        /// </summary>
        /// <returns>returns boolean value</returns>
        /// <internalonly/>
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
        /// <summary>
        /// Used internally.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
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
                    if (value && headerDragHelper == null)
                    {
                        OpenDragHeader();
                    }
                    else if (!value)
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
            tempHeaderBitmap = bm;
            headerDragHelper = new GridDragColumnHeaderHelper();
            cursor = Cursors.Default;
        }

        internal void CloseDragHeader()
        {
            if (headerDragHelper != null)
            {
                headerDragHelper.EndDrag();
                headerDragHelper.Dispose();
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
            if (!this.headerDragHelper.IsDragging)
                headerDragHelper.StartDrag(tempHeaderBitmap, Control.MousePosition, DragDropEffects.Move);
            this.headerDragHelper.DoDrag(pt, DragDropEffects.Move);
            if (redArrowIndicatorDragHelper != null)
            {
                redArrowIndicatorDragHelper.DragWindow.ShowWindowTopMost();
            }
        }

        private Bitmap CreateHeaderBitmap(GridControlBase grid, int rowIndex, int colIndex)
        {
            Graphics g = null;
            Size size = new Size(grid.GetColWidth(colIndex), grid.GetRowHeight(rowIndex));
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

        /// <summary>
        /// Implement this method in your <see cref="IMouseController"/> and return False if it would interfere with your
        /// controller's state when the current cell is focused and possibly scrolled into view.
        /// </summary>
        /// <returns>
        /// A <see cref="Boolean"/> that indicates if the grid is allowed to set the focus onto the current cells <see cref="Control"/>.
        /// </returns>
        /// <implement/>
        public virtual bool GetAllowFixFocus()
        {
            return false;
        }

        #endregion

        #region IMouseController implementation

        /// <implement/>
        ///<summary>
        /// The name of mouse controller.
        ///</summary>
        public virtual string Name
        {
            get
            {
                return "DragGroupHeader";
            }
        }

        /// <implement/>
        /// <summary>
        /// The cursor to be displayed.
        /// </summary>
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
        /// MouseHoverEnter is called when this controller signaled in HitTest that it wants to handle mouse events.
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
        ///<summary>
        /// MouseHover is called when this controller signaled in HitTest that it wants to handle mouse events.
        ///</summary>
        ///<param name="e">A <see cref="MouseEventArgs"/> holding the event data.</param>
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
        /// <param name="e">The event args.</param>
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
        /// <remarks>
        /// MouseDown is called and this controller will become the active controller and receive all subsequent mouse message
        /// until the mouse button is released or the mouse operation is cancelled.
        /// </remarks>
        /// <param name="e">A <see cref="MouseEventArgs"/> holding event data.</param>
        public virtual void MouseDown(MouseEventArgs e)
        {
            if (clickCellsControllerHitTest != 0)
            {
                clickCellsController.MouseDown(e);
            }
        }

        /// <implement/>
        /// <summary>
        /// MouseMove is called for the active controller after a MouseDown message when the user moves the mouse pointer.
        /// </summary>
        /// <param name="e">A <see cref="MouseEventArgs"/> holding event data.</param>
        public virtual void MouseMove(MouseEventArgs e)
        {
            if (clickCellsControllerHitTest != 0)
            {
                clickCellsController.MouseMove(e);
            }
        }

        /// <implement/>
        /// <summary>
        /// MouseUp is called for the active controller after a MouseDown message when the user releases the mouse button.
        /// </summary>
        /// <param name="e">A <see cref="MouseEventArgs"/> holding event data.</param>
        public virtual void MouseUp(MouseEventArgs e)
        {
            if (clickCellsControllerHitTest != 0)
            {
                clickCellsController.MouseUp(e);
            }
        }

        /// <implement/>
        /// <summary>
        /// CancelMode is called for the active controller after a MouseDown message when the mouse operation is cancelled.
        /// </summary>
        public virtual void CancelMode()
        {
            if (clickCellsControllerHitTest != 0)
            {
                clickCellsController.CancelMode();
            }
        }

        /// <summary>
        /// HitTest is called to determine whether your controller wants to handle the mouse events based current context.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <param name="controller">The controller.</param>
        /// <returns>returns int value to determine whether your controller wants to handle the mouse events based current context</returns>
        /// <implement/>
        public virtual int HitTest(MouseEventArgs e, IMouseController controller)
        {
            clickCellsControllerHitTest = clickCellsController.HitTest(e, controller);
            return clickCellsControllerHitTest;
        }

        #endregion
    }

    /// <summary>
    /// Mouse controller that provides support for dragging column headers within the
    /// grid.
    /// </summary>
    public class GridDragColumnHeaderMouseController : GridDragColumnHeaderMouseControllerBase
    {
        GridDragHeaderHitTestInfo hitTestInfo = null;
        GridRangeInfo _cellRange;

        /// <summary>
        /// Initializes the mouse controller with grid it operates on
        /// </summary>
        /// <param name="grid">The grid control.</param>
        public GridDragColumnHeaderMouseController(GridControlBaseImp grid)
            : base(grid)
        {
            clickCellsController = new GridClickCellsMouseController(grid);
        }

        #region MouseController Implementation

        bool doubleClick = false;

        /// <override/>
        /// /// <summary>
        /// MouseDown is called when this controller signaled in HitTest that it wants to handle mouse events and the
        /// user pressed the mouse button.
        /// </summary>
        /// <remarks>
        /// MouseDown is called and this controller will become the active controller and receive all subsequent mouse message
        /// until the mouse button is released or the mouse operation is cancelled.
        /// </remarks>
        /// <param name="e">A <see cref="MouseEventArgs"/> holding event data.</param>
        public override void MouseDown(MouseEventArgs e)
        {
            if (this.hitTestInfo == null)
            {
                base.MouseDown(e);
                return;
            }

            this.ResetClickCellsController();

            if (e.Clicks == 2)
            {
                int rowIndex, colIndex;
                grid.PointToRowCol(new Point(e.X, e.Y), out rowIndex, out colIndex);
                grid.RaiseCellDoubleClick(rowIndex, colIndex, e);
                doubleClick = true;
                return;
            }

            doubleClick = false;
            this.OpenDragHeader();
            this.OpenRedArrowIndicator();
            grid.PrepareViewStyleInfo += new GridPrepareViewStyleInfoEventHandler(grid_PrepareViewStyleInfo);
            grid.InvalidateRange(this._cellRange);
            grid.GetGridWindow().AutoScrolling = ScrollBars.Horizontal;
            grid.GetGridWindow().AutoScrollBounds = Rectangle.Empty;
        }

        /// <override/>
        /// <summary>
        /// MouseMove is called for the active controller after a MouseDown message when the user moves the mouse pointer.
        /// </summary>
        /// <param name="e">A <see cref="MouseEventArgs"/> holding event data.</param>
        public override void MouseMove(MouseEventArgs e)
        {
            if (this.hitTestInfo == null)
            {
                base.MouseMove(e);
                return;
            }

            this.ResetClickCellsController();

            if (this.ShouldShowRemoveCursor())
            {
                cursor = Cursors.Default; ////GridGroupingCursors.RemoveCursor;
            }
            else
            {
                cursor = Cursors.Default;
            }

            this.UpdateDragHeader();
            this.UpdateRedArrowIndicator();

            this.wasDragged |= DragHeaderVisible;
        }

        /// <override/>
        /// <summary>
        /// MouseUp is called for the active controller after a MouseDown message when the user releases the mouse button.
        /// </summary>
        /// <param name="e">A <see cref="MouseEventArgs"/> holding event data.</param>
        public override void MouseUp(MouseEventArgs e)
        {
            if (this.hitTestInfo == null)
            {
                base.MouseUp(e);
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
            grid.InvalidateRange(this._cellRange);

            if (!wasDragged)
            {
                if (!doubleClick)
                {
                    grid.RaiseCellClick(_rowIndex, _colIndex, e);
                }
            }
            else if (bv)
            {
                if (ShouldShowRedArrowIndicator())
                {
                    GridQueryDragColumnHeaderEventArgs ae = new GridQueryDragColumnHeaderEventArgs(this.grid, column, targetColIndex, GridQueryDragColumnHeaderReason.MouseUp);
                    this.grid.RaiseQueryAllowDragColumn(ae);
                    if (ae.AllowDrag)
                    {
                        if (targetColIndex > column)
                        {
                            targetColIndex--;
                        }

                        grid.Model.Cols.MoveRange(column, 1, targetColIndex);
                    }
                    ////grid.Update();
                    ////grid.LeftColIndex = grid.Model.Cols.FrozenCount+1;
                    Cursor.Current = Cursors.Default;
                }
            }
        }

        /// <override/>
        /// <summary>
        /// CancelMode is called for the active controller after a MouseDown message when the mouse operation is cancelled.
        /// </summary>
        public override void CancelMode()
        {
            base.CancelMode();

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
            grid.InvalidateRange(this._cellRange);
        }

        /// <override/>
        /// <summary>
        /// HitTest is called to determine whether your controller wants to handle the mouse events based current context.
        /// </summary>
        /// <param name="e">A MouseEventArgs holding event data.</param>
        /// <param name="controller">A mouse controller.</param>
        /// <returns>A non-zero value if the controller can and wants to handle the mouse event; 0 otherwise.</returns>
        public override int HitTest(MouseEventArgs e, IMouseController controller)
        {
            //// This HitTest code has higher priority than "SelectCells"
            Point pt = new Point(e.X, e.Y);
            hitTestInfo = null;

            ////if (!AllowDragColumns)
            ////    return 0;
 ////&& e.Clicks < 2
            if (CheckMouseButtons(e) && (controller == null || controller.Name != "ResizeCells"))
            {
                hitTestInfo = new GridDragHeaderHitTestInfo(grid, pt);
                if (hitTestInfo.hitTestResult == GridHitTestContext.None)
                {
                    hitTestInfo = null;
                }
                else
                {
                    GridQueryDragColumnHeaderEventArgs ae = new GridQueryDragColumnHeaderEventArgs(this.grid, hitTestInfo.colIndex, -1, GridQueryDragColumnHeaderReason.HitTest);
                    this.grid.RaiseQueryAllowDragColumn(ae);
                    if (ae.AllowDrag)
                    {
                        this._rowIndex = hitTestInfo.rowIndex;
                        this._colIndex = hitTestInfo.colIndex;
                        this._cellRange = hitTestInfo.cellRange;
                        this.originBounds = grid.GridRectangleToScreen(grid.RangeInfoToRectangle(_cellRange));
                        this.origin = this.grid.GetGridWindow().LastMousePosition;
                        this.column = hitTestInfo.colIndex;
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
                base.HitTest(e, controller);
                return hitTestInfo.hitTestResult;
            }

            return 0;
        }

        internal sealed class GridDragHeaderHitTestInfo
        {
            internal GridRangeInfo cellRange;
            internal int rowIndex;
            internal int colIndex;
            internal int hitTestResult = GridHitTestContext.None;
            internal Point point;

            internal GridDragHeaderHitTestInfo(GridControlBase grid, Point point)
            {
                if (grid.GridBounds.Contains(point))
                {
                    ////if (grid.Model.Table.TableLevel > -1)
                    ////    Trace.WriteLine(point);
                    cellRange = grid.PointToRangeInfo(point, -1);
                    rowIndex = cellRange.Top;
                    colIndex = cellRange.Left;
                    if (rowIndex <= grid.Model.Rows.HeaderCount)
                    {
                        this.point = point;
                        if (colIndex > grid.Model.Cols.HeaderCount)
                        {
                            hitTestResult = GridHitTestContext.Header;
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
    
    /// <internalonly/>
    /// <summary>For internal use.</summary>
    public class GridDragColumnHeaderHelper : IDisposable
    {
        // Fields
        private bool isDragging = false;
        internal DragDropEffects lastDragDropEffect = DragDropEffects.None;

        internal GridDragColumnHeaderWindow dragWindow = new GridDragColumnHeaderWindow();

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public GridDragColumnHeaderWindow DragWindow
        {
            get
            {
                return dragWindow;
            }
        }

        // Constructors

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        public GridDragColumnHeaderHelper()
        {
        }

        // Methods

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        public void StartDrag(Bitmap bmp, Point startPoint, DragDropEffects effects)
        {
            this.StopDrag();
            this.isDragging = true;
            this.lastDragDropEffect = effects;

            this.dragWindow.DragBitmap = bmp;
            this.dragWindow.Invalidate();
            this.dragWindow.StartDrag(startPoint);
            this.CheckDragCursor(effects);
        }

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected void CheckDragCursor(DragDropEffects e)
        {
            return;
        }

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected void StopDrag()
        {
            this.isDragging = false;
            this.lastDragDropEffect = DragDropEffects.None;
            this.dragWindow.StopDrag();
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        public void DoDrag(Point p, DragDropEffects e)
        {
            if (!this.isDragging)
            {
                return;
            }

            this.lastDragDropEffect = e;
            this.CheckDragCursor(e);
            this.dragWindow.MoveTo(p);
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        public void CancelDrag()
        {
            if (!this.isDragging)
            {
                return;
            }

            this.StopDrag();
        }
        
        /// <internalonly/>
        /// <summary>Used internally.</summary>
        public void EndDrag()
        {
            if (!this.isDragging)
            {
                return;
            }

            this.StopDrag();
        }
        
        // Properties

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public DragDropEffects LastDragDropEffect
        {
            get
            {
                return this.lastDragDropEffect;
            }
        }

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public bool IsDragging
        {
            get
            {
                return this.isDragging;
            }
        }

        #region IDisposable Members
        /// <summary>Used internally.</summary>
        public void Dispose()
        {
            this.dragWindow.Dispose();
            this.dragWindow = null;
        }

        #endregion
    }
    
    /// <internalonly/>
    /// <summary>Used internally.</summary>
    [ToolboxItem(false)]
    public class GridDragColumnHeaderWindow : DragWindow
    {
        // Fields
        private Bitmap dragBitmap = null;
        private bool isDragging = false;
        private Point origin = new Point(-30000, -30000);

        // Constructors

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        public GridDragColumnHeaderWindow()
        {
            if (GridDragColumnHeaderMouseControllerBase.SupportsTransparentForm())
            {
                this.TransparencyKey = Color.Red;
            }

            this.SetStyle(ControlStyles.Selectable, false);
            this.SetStyle(ControlStyles.UserMouse, true);
        }

        private void _Move(Point p)
        {
            this.Location = p;
            if (this.BackgroundImage != null)
            {
                this.Size = this.BackgroundImage.Size;
            }
            else
            {
                this.Size = this.MinimumSize;
            }
        }

        /// <summary>
        /// Used internally.
        /// </summary>
        /// <returns>returns boolean value</returns>
        /// <internalonly/>
        public new bool StartDrag(Point p)
        {
            if (this.BackgroundImage == null)
            {
                return false;
            }

            this.isDragging = true;
            this._Move(p);
            this.ShowWindowTopMost();
            return true;
        }

        /// <summary>
        /// Used internally.
        /// </summary>
        /// <returns>returns boolean value.</returns>
        /// <internalonly/>
        public new bool MoveTo(Point p)
        {
            if (!this.isDragging)
            {
                return false;
            }

            this._Move(p);
            return true;
        }

        /// <summary>
        /// Used internally.
        /// </summary>
        /// <returns>returns boolean value</returns>
        /// <internalonly/>
        public new bool StopDrag()
        {
            if (!this.isDragging)
            {
                return false;
            }

            this.BackgroundImage = null;
            this.isDragging = false;
            this.SuspendLayout();
            this.Visible = false;
            this.ResumeLayout();
            return true;
        }

        // Properties

        /// <internalonly/>
        /// <summary>Gets or sets DragBitmap. Used internally.</summary>
        public new Bitmap DragBitmap
        {
            get
            {
                return this.dragBitmap;
            }

            set
            {
                this.BackgroundImage = value;
                if (value == null)
                {
                    this.StopDrag();
                }
                else
                {
                    Size size = value.Size; ////new Size(value.Size.Width*2, value.Size.Height*2);/
                    this.origin = Point.Empty;

                    // Whidbey added a call to SetWindowPos in its Form.MinimumSize property setter. When this
                    // method is called it uses flags that change the z-order. This is not wanted for
                    // this DragWindow control.
                    //
                    // The problem can be avoided by destroying the window handle before setting the property.
                    // It will be recreated later automatically with correct z-order.
                    if (Environment.Version.Major >= 2)
                    {
                        this.DestroyHandle();
                    }

                    this.MinimumSize = size;
                    this.Size = size;
                }

                this.dragBitmap = value;
            }
        }
        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.dragBitmap = null;
            }

            base.Dispose(disposing);
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
        /// HitTest is occurring
        /// </summary>
        HitTest,

        /// <summary>
        /// RedArrowIndicator is displayed
        /// </summary>
        ShowRedArrowIndicator,

        /// <summary>
        /// Represents MouseUp
        /// </summary>
        MouseUp
    }

    /// <summary>
    /// Holds a reference to a <see cref="GridControlBase"/> that initiates the event and the column 
    /// that is affected.
    /// </summary>
    /// <remarks>
    /// Set <see cref="AllowDrag"/> to False if you do not want to allow the user 
    /// to drag the specified <see cref="Column"/>.
    /// </remarks>
    public sealed class GridQueryDragColumnHeaderEventArgs : SyncfusionEventArgs
    {
        GridControlBase grid;
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
        public GridQueryDragColumnHeaderEventArgs(GridControlBase grid, int column, int insertBeforeColumn, GridQueryDragColumnHeaderReason reason)
        {
            this.grid = grid;
            this.column = column;
            this.insertBeforeColumn = insertBeforeColumn;
            this.reason = reason;
        }

        /// <summary>
        /// Gets the table control.
        /// </summary>
        [TraceProperty(true)]
        public GridControlBase Grid
        {
            get
            {
                return grid;
            }
        }

        /// <summary>
        /// Gets Column Name. You can call TableDescriptor.Columns[Column] to get the GridColumnDescriptor.
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
        /// Gets Name of the column to insert at. You can call TableDescriptor.Columns[InsertBeforeColumn] to get the GridColumnDescriptor.
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
        /// Gets or sets reason why this event was raised (Show Red Indicator, MouseUp, or HitTest).
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
        /// Gets or sets a value indicating whether to allow drag. <see cref="AllowDrag"/> to False if you do not want to allow the user 
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
}