//-------------------------------------------------------------------------------------------------
// <copyright file="GridDragGroupHeader.cs" company="syncfusion">
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
    public abstract class GroupDragHeaderMouseControllerBase : IMouseController, IGridFocusHelper
    {
        #region Fields
        internal bool isGroupAreaOrigin = false;   // true if GroupArea is initiating action
        internal Rectangle groupAreaBounds = Rectangle.Empty;  // area where default cursor is shown
        internal Rectangle headerSectionBounds = Rectangle.Empty;  // area where default cursor is shown

        internal GridTableControl _grid;
        internal GridGroupDropArea groupDropArea;
        internal GridColumnDescriptor column;
        internal bool wasDragged = false;  // otherwise raise click event.
        internal bool cancelRemove = false;
        internal Rectangle originBounds = Rectangle.Empty;

        private GroupDragHelper headerDragHelper = null;

        internal Bitmap dragHeaderBitmap = null;

        internal Point origin = Point.Empty;
        bool dragHeaderVisible = false;

        internal Cursor cursor = null;
        internal int _rowIndex, _colIndex;

        Point hiddenPoint = new Point(10000, 10000);
        GroupDragHelper redArrowIndicatorDragHelper = null;

        internal const int GroupDropAreaRowIndex = 2;
        internal const int GroupDropAreaColIndex = 2;
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

        /// <overload>
        /// Initializes the object
        /// </overload>
        /// <summary>
        /// Initializes the object
        /// </summary>
        public GroupDragHeaderMouseControllerBase()
        {
        }

        /// <summary>
        /// Initializes the object with the grid it is bound to.
        /// </summary>
        /// <param name="grid">The grid table control.</param>
        public GroupDragHeaderMouseControllerBase(GridTableControl grid)
        {
            this._grid = grid;
        }

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

        #region Helper Methods
        /// <summary>
        /// The GridGroupDropArea above the grid
        /// </summary>
        public GridGroupDropArea GridGroupDropArea
        {
            get
            {
                if (groupDropArea == null)
                {
                    groupDropArea = grid.GroupDropArea;
                }

                return groupDropArea;
            }
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
        /// Determines based on mouse position if remove cursor should be shown
        /// </summary>
        /// <returns>True if remove cursor should be shown.</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude]
        public bool ShouldShowRemoveCursor()
        {
            if (grid == null || grid.Table == null || grid.Table.Engine.ParentControl == null || !this.grid.Table.Engine.ParentControl.ShowGroupDropArea)
            {
                return false;
            }

            Point pt = Control.MousePosition;
            bool b1 = groupAreaBounds.Contains(pt);
            bool b2 = headerSectionBounds.Contains(pt) || headerSectionBounds.IsEmpty;
            return !(b1 || b2);
        }

        /// <summary>
        /// Determines based on mouse position if mouse is over GroupDropArea
        /// </summary>
        /// <returns>True if mouse is over GroupDropArea.</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude]
        public bool IsMouseOverGroupDropArea()
        {
            if (grid == null || grid.Table == null || grid.Table.Engine.ParentControl == null || !this.grid.Table.Engine.ParentControl.ShowGroupDropArea)
            {
                return false;
            }

            Point pt = Control.MousePosition;
            bool b1 = groupAreaBounds.Contains(pt);
            return b1;
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

        internal GridRangeInfo GetRangeOfColumnHeaderSection()
        {
            if (Table != null)
            {
                return Table.GetRangeOfColumnHeaderSection();
            }

            return GridRangeInfo.Empty;
        }

        /// <summary>
        /// The column descriptor under the mouse position. Will be null if there is no header drawn below mouse.
        /// </summary>
        /// <returns>The column descriptor.</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude]
        public GridColumnDescriptor GetColumnDescriptorAtMousePosition()
        {
            Point pt = Control.MousePosition;
            if (this.IsMouseOverGroupDropArea())
            {
                pt = GridGroupDropArea.GridPointToClient(pt);
                return GridGroupDropArea.GetHeaderColumnDescriptorAt(pt);
            }
            else
            {
                pt = grid.GridPointToClient(pt);
                return grid.GetHeaderColumnDescriptorAt(pt);
            }
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
            tempArrowBitmap = bm;
            //// There is always a small black rectangle at the lower right-corner. Drawing
            //// over the previously painted BackgroundImage resolves this issue.
            redArrowIndicatorDragHelper.DragWindow.Paint += new PaintEventHandler(DragWindow_Paint);
            if (!GroupDragHeaderMouseControllerBase.SupportsTransparentForm())
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
                redArrowIndicatorDragHelper.DragWindow.Paint -= new PaintEventHandler(DragWindow_Paint);
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
            GridRangeInfo rows = GetRangeOfColumnHeaderSection();
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
                if (!grid.Model.HierarchicalGroupDropArea)
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
        /// <returns>True if it should be shown.</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude]
        public bool ShouldShowRedArrowIndicator()
        {
            if (ShouldShowRemoveCursor())
            {
                return false;
            }

            if (this.TableDescriptor.ColumnSets.Count > 0
                && !this.IsMouseOverGroupDropArea())
            {
                return false;
            }

            GridColumnDescriptor cd = GetColumnDescriptorAtMousePosition();

            return cd != column;
            /*
            if (IsMouseOverGroupDropArea())
            {
                Point pt = GridGroupDropArea.GridPointToClient(Control.MousePosition);
                GridColumnDescriptor cd = GridGroupDropArea.GetHeaderColumnDescriptorAt(pt);
                if (cd == null)
                    return false;
                int srcIndex = TableDescriptor.GroupedColumns.IndexOf(column.MappingName);
                int dest = TableDescriptor.GroupedColumns.IndexOf(cd.MappingName);
                if (dest == srcIndex || dest == srcIndex+1)
                    return false;
                return true;
            }
            else
            {
                Point pt = grid.GridPointToClient(Control.MousePosition);
                GridColumnDescriptor cd = grid.GetHeaderColumnDescriptorAt(pt);
                if (cd == null)
                    return false;
                int srcIndex = TableDescriptor.Columns.IndexOf(column);
                int dest = TableDescriptor.Columns.IndexOf(cd);
                if (dest == srcIndex || dest == srcIndex+1)
                    return false;
                return true;
            }
            */
        }

        /// <summary>
        /// Used internally.
        /// </summary>
        /// <param name="inGroupDropArea">if set to <c>true</c> [in group drop area].</param>
        /// <param name="cd">The GridColumnDescriptor.</param>
        /// <returns>returns the point of RedArrowIndicatorLocation</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude]
        public Point GetRedArrowIndicatorLocation(bool inGroupDropArea, GridColumnDescriptor cd)
        {
            if (inGroupDropArea)
            {
                int num = 0;
                if (cd != null)
                {
                    num = TableDescriptor.GroupedColumns.IndexOf(cd.MappingName);
                }

                int colIndex = GridGroupDropArea.Model.FieldToColIndex(num);
                Rectangle r = GridGroupDropArea.RangeInfoToRectangle(GridRangeInfo.Cell(GroupDropAreaRowIndex, colIndex));
                Point pt = grid.IsRightToLeft() ? new Point(r.Right, r.Top) : r.Location;
                return GridGroupDropArea.GridPointToScreen(pt);
            }
            else
            {
                if (cd != null && redArrowRowIndex != -1)
                {
                    GridRangeInfo range = grid.Table.GetRangeOfHeaderColumnDescriptor(cd);
                    if (!range.IsEmpty)
                    {
                        int colIndex = range.Left;

                        Rectangle r = grid.RangeInfoToRectangle(GridRangeInfo.Cell(range.Top, colIndex));
                        GridControlBase gridWindow = grid.GetGridWindow();

                        // Support for dragging columns in child group column headers.
                        r.Y = gridWindow.ViewLayout.RowColToPoint(redArrowRowIndex, 0, GridCellSizeKind.VisibleSize).Y;

                        Point pt = grid.IsRightToLeft() ? new Point(r.Right, r.Top) : r.Location;
                        return grid.GridPointToScreen(pt);
                    }
                }

                return Point.Empty;
            }
        }

        internal bool redArrowGroupDropArea = false;
        internal int redArrowFieldNum = 0;

        internal int redArrowRowIndex = 0;

        /// <internalonly/>
        /// <summary>Internal only.</summary>
        [Syncfusion.Documentation.DocumentationExclude]
        public void UpdateRedArrowIndicator()
        {
            Point pt = Control.MousePosition;
            if (!IsMouseOverGroupDropArea())
            {
                redArrowGroupDropArea = false;
                if (this.isGroupAreaOrigin)
                {
                    return;
                }

                pt = grid.GridPointToClient(pt);
                GridControlBase gridWindow = grid.GetGridWindow();
                if (pt.X < gridWindow.GridBounds.Left || pt.X > gridWindow.GridBounds.Right)
                {
                    goto noArrow;
                }

                GridColumnDescriptor cd = grid.GetHeaderColumnDescriptorAt(pt);

                if (this.ShouldShowRedArrowIndicator() && cd != null && cd != column)
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
                    if (!td.VisibleColumns.IsModified)
                    {
                        int n = td.Columns.IndexOf(cd);
                        int srcIndex = td.Columns.IndexOf(column);
                        if (n == srcIndex + 1)
                        {
                            if (n + 1 < td.Columns.Count)
                            {
                                cd = td.Columns[++n];
                            }
                            else
                            {
                                goto noArrow;
                            }
                        }

                        redArrowFieldNum = n;
                    }
                    else
                    {
                        int n = td.VisibleColumns.IndexOf(cd.Name);
                        int srcIndex = td.VisibleColumns.IndexOf(column.Name);
                        if (n == srcIndex + 1)
                        {
                            if (n + 1 < td.VisibleColumns.Count)
                            {
                                cd = td.Columns[td.VisibleColumns[++n].Name];
                            }
                            else
                            {
                                goto noArrow;
                            }
                        }

                        redArrowFieldNum = td.Columns.IndexOf(cd.Name);
                    }

                    redArrowRowIndex = gridWindow.GetRow(gridWindow.ViewLayout.PointToClientRow(pt, GridCellSizeKind.ActualSize));

                    Point p = Point.Empty;
                    GridQueryAllowDragColumnEventArgs ae = new GridQueryAllowDragColumnEventArgs(this.grid, column.Name, cd.Name, GridQueryAllowDragColumnReason.ShowRedArrowIndicator);
                    this.grid.RaiseQueryAllowDragColumn(ae);
                    if (!ae.AllowDrag)
                    {
                        cd = null;
                        pt = hiddenPoint;
                    }
                    else
                    {
                        p = GetRedArrowIndicatorLocation(false, cd);

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
                            if(!redArrowIndicatorDragHelper.IsDragging)
                                redArrowIndicatorDragHelper.StartDrag(tempArrowBitmap,Control.MousePosition, DragDropEffects.Move);
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
            else
            {
                Bitmap downBitmap = GridGroupingBitmaps.RedDownBitmap;
                Bitmap upBitmap = GridGroupingBitmaps.RedUpBitmap;

                redArrowGroupDropArea = true;
                pt = GridGroupDropArea.GridPointToClient(pt);
                GridRangeInfo cell = GridGroupDropArea.PointToRangeInfo(pt);
                int fieldNum = GridGroupDropArea.Model.ColIndexToField(cell.Left);
                GridGroupingControl groupingGrid = this.Table.Engine.ParentControl as GridGroupingControl;
                if (groupingGrid.GroupDropAreaAlignment == GridGroupDropAreaAlignment.Left || groupingGrid.GroupDropAreaAlignment == GridGroupDropAreaAlignment.Right)
                {
                    fieldNum = GridGroupDropArea.Model.ColIndexToField(cell.Top);
                }
                int fnum = GridGroupDropArea.Model.ColIndexToField(_colIndex);
                int frow = GridGroupDropArea.Model.ColIndexToField(_rowIndex);
                bool flag = false;
                if (redArrowIndicatorDragHelper != null)
                {
                    if (!(this.Table.TableModel.GroupDropAlign == GridGroupDropAreaAlignment.Left || this.Table.TableModel.GroupDropAlign == GridGroupDropAreaAlignment.Right))
                    {
                        if (this.isGroupAreaOrigin && (!this.ShouldShowDragBitmap() || (fieldNum >= fnum && fieldNum <= fnum + 1) && !flag))
                        {
                            redArrowIndicatorDragHelper.DoDrag(hiddenPoint, DragDropEffects.Copy);
                        }
                        else
                        {
                            redArrowFieldNum = fieldNum;

                            string insertBeforeColumnName = string.Empty;
                            if (redArrowFieldNum < TableDescriptor.GroupedColumns.Count)
                            {
                                insertBeforeColumnName = TableDescriptor.GroupedColumns[redArrowFieldNum].Name;
                                GridColumnDescriptor cd = TableDescriptor.Columns.FindByMappingName(insertBeforeColumnName);
                                if (cd != null)
                                {
                                    insertBeforeColumnName = cd.Name;
                                }
                            }

                            GridQueryAllowGroupByColumnEventArgs ae = new GridQueryAllowGroupByColumnEventArgs(this.grid, column.Name, insertBeforeColumnName, GridQueryAllowDragColumnReason.ShowRedArrowIndicator, column.AllowGroupByColumn);
                            this.grid.RaiseQueryAllowGroupByColumn(ae);
                            if (ae.AllowGroupByColumn)
                            {
                                Rectangle r = new Rectangle();
                                int colIndex = GridGroupDropArea.Model.FieldToColIndex(fieldNum);
                                if (this.grid.Table.TableModel.GroupDropAlign == GridGroupDropAreaAlignment.Left || this.grid.Table.TableModel.GroupDropAlign == GridGroupDropAreaAlignment.Right)
                                    r = GridGroupDropArea.RangeInfoToRectangle(GridRangeInfo.Cell(colIndex, GroupDropAreaColIndex));
                                else
                                    r = GridGroupDropArea.RangeInfoToRectangle(GridRangeInfo.Cell(GroupDropAreaRowIndex, colIndex));

                                pt = grid.IsRightToLeft() ? new Point(r.Right, r.Top) : r.Location;
                                pt = GridGroupDropArea.GridPointToScreen(pt);
                                pt.X -= downBitmap.Width / 2;
                                pt.Y -= downBitmap.Height;
                                if (!redArrowIndicatorDragHelper.IsDragging)
                                    redArrowIndicatorDragHelper.StartDrag(tempArrowBitmap, Control.MousePosition, DragDropEffects.Move);
                                redArrowIndicatorDragHelper.DoDrag(pt, DragDropEffects.Copy);
                                redArrowIndicatorDragHelper.DragWindow.ShowWindowTopMost();
                            }
                        }
                    }
                    else
                    {
                        if (this.isGroupAreaOrigin && (!this.ShouldShowDragBitmap() || (fieldNum >= frow && fieldNum <= frow + 1)))
                        {
                            redArrowIndicatorDragHelper.DoDrag(hiddenPoint, DragDropEffects.Copy);
                        }
                        else
                        {
                            redArrowFieldNum = fieldNum;

                            string insertBeforeColumnName = string.Empty;
                            if (redArrowFieldNum < TableDescriptor.GroupedColumns.Count)
                            {
                                insertBeforeColumnName = TableDescriptor.GroupedColumns[redArrowFieldNum].Name;
                                GridColumnDescriptor cd = TableDescriptor.Columns.FindByMappingName(insertBeforeColumnName);
                                if (cd != null)
                                {
                                    insertBeforeColumnName = cd.Name;
                                }
                            }

                            GridQueryAllowGroupByColumnEventArgs ae = new GridQueryAllowGroupByColumnEventArgs(this.grid, column.Name, insertBeforeColumnName, GridQueryAllowDragColumnReason.ShowRedArrowIndicator, column.AllowGroupByColumn);
                            this.grid.RaiseQueryAllowGroupByColumn(ae);
                            if (ae.AllowGroupByColumn)
                            {
                                Rectangle r = new Rectangle();
                                int colIndex = GridGroupDropArea.Model.FieldToColIndex(fieldNum);
                                if (this.grid.Table.TableModel.GroupDropAlign == GridGroupDropAreaAlignment.Left || this.grid.Table.TableModel.GroupDropAlign == GridGroupDropAreaAlignment.Right)
                                    r = GridGroupDropArea.RangeInfoToRectangle(GridRangeInfo.Cell(colIndex, GroupDropAreaColIndex));
                                else
                                    r = GridGroupDropArea.RangeInfoToRectangle(GridRangeInfo.Cell(GroupDropAreaRowIndex, colIndex));

                                pt = grid.IsRightToLeft() ? new Point(r.Right, r.Top) : r.Location;
                                pt = GridGroupDropArea.GridPointToScreen(pt);
                                pt.X -= downBitmap.Width / 2;
                                pt.Y -= downBitmap.Height;
                                if (!redArrowIndicatorDragHelper.IsDragging)
                                    redArrowIndicatorDragHelper.StartDrag(tempArrowBitmap, Control.MousePosition, DragDropEffects.Move);
                                redArrowIndicatorDragHelper.DoDrag(pt, DragDropEffects.Copy);
                                redArrowIndicatorDragHelper.DragWindow.ShowWindowTopMost();
                            }
                        }
                    }
                
                }
            }
        }

        #endregion

        #region Drag Header

        internal Point GetMovementDelta()
        {
            Point pt = Control.MousePosition;
            return new Point(pt.X - origin.X, pt.Y - origin.Y);
        }

        /// <summary>
        /// Used internally.
        /// </summary>
        /// <returns>returns boolean value to indicate ShouldShowDragBitmap</returns>
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
            if (!this.isGroupAreaOrigin)
            {
                bm = CreateHeaderBitmap(grid, _rowIndex, _colIndex);
            }
            else
            {
                bm = CreateHeaderBitmap(GridGroupDropArea, _rowIndex, _colIndex);
            }

            headerDragHelper = new GroupDragHelper();
            cursor = Cursors.Default;
            GridGroupingControl groupingGrid = this.Table.Engine.ParentControl as GridGroupingControl;
            tempHeaderBitmap = bm;
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
                this.headerDragHelper.StartDrag(tempHeaderBitmap, Control.MousePosition, DragDropEffects.Move);
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

            if (this.grid.Table.TableOptions.GridVisualStyles == Forms.GridVisualStyles.Metro)
                bm = this.ProcessBitmap(bm);
            return bm;
        }
        /// <summary>
        /// Used Internally to process BorderColor of bitmap.
        /// </summary>
        /// <param name="bm">Gets the bitmap to be processed</param>
        /// <returns>bitmap</returns>
        internal Bitmap ProcessBitmap(Bitmap bm)
        {
            Color borderColor = System.Drawing.Color.FromArgb(27, 161, 226);
            for (int i = 0; i < 2; i++)
            {
                for (int X = 0; X < bm.Width; X++)
                {
                    for (int Y = 0; Y < bm.Height; Y++)
                    {
                        if (i == 1)
                        {
                            if (X == 1)
                                X = bm.Width - 1;
                        }
                        else
                        {
                            if (Y == 1)
                                Y = bm.Height - 1;
                        }
                        bm.SetPixel(X, Y, borderColor);
                    }
                }
            }
            return bm;
        }
        #endregion

        #region IGridFocusHelper Implementation

        /// <implement/>
        /// <summary>Gets the Allow fix focus.</summary>
        /// <returns>returns False.</returns>
        public virtual bool GetAllowFixFocus()
        {
            return false;
        }

        #endregion

        #region IMouseController implementation

        /// <implement/>
        /// <summary>Returns the name of the mouse controller.</summary>
        public virtual string Name
        {
            get
            {
                return "DragGroupHeader";
            }
        }

        /// <implement/>
        /// <summary>Gets the cursor to be displayed.</summary>
        public virtual Cursor Cursor
        {
            get
            {
                if (cursor != null)
                {
                    return cursor;
                }
                else if (clickCellsController.Cursor == null)
                {
                    return Cursors.Default;
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
            clickCellsControllerHitTest = clickCellsController.HitTest(e, controller);
            return clickCellsControllerHitTest;
        }

        #endregion
    }

    /// <summary>
    /// Mouse controller that provides support for dragging column headers within the
    /// GridTableControl or dragging them to the GroupDropArea.
    /// </summary>
    public class GridTableControlDragHeaderMouseController : GroupDragHeaderMouseControllerBase
    {
        DragGroupHeaderHitTestInfo hitTestInfo = null;
        GridRangeInfo _cellRange;

        /// <summary>
        /// Initializes the mouse controller with grid it operates on
        /// </summary>
        /// <param name="grid">The grid table control.</param>
        public GridTableControlDragHeaderMouseController(GridTableControl grid)
            : base(grid)
        {
            this.groupDropArea = grid.GroupDropArea;
            isGroupAreaOrigin = false;
            clickCellsController = new GridClickCellsMouseController(grid);
        }

        #region MouseController Implementation

        /// <override/>
        /// <summary>
        /// MouseDown is called when this controller signaled in HitTest that it wants to handle mouse events and the
        /// user pressed the mouse button.
        /// </summary>
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
                return;
            }

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
                cursor = GridGroupingCursors.RemoveCursor;
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
                grid.RaiseCellClick(_rowIndex, _colIndex, e);
            }
            else if (bv)
            {
                if (redArrowGroupDropArea)
                {
                    if (!grid.CurrentCell.Deactivate(false))
                    {
                        return;
                    }

                    int indexOfGroupedColumn = TableDescriptor.GroupedColumns.IndexOf(column.MappingName);
                    if (indexOfGroupedColumn != -1)
                    {
                        if (indexOfGroupedColumn < redArrowFieldNum)
                        {
                            redArrowFieldNum--;
                        }
                    }

                    if (redArrowFieldNum >= 0)
                    {
                        string insertBeforeColumnName = string.Empty;
                        if (redArrowFieldNum < TableDescriptor.GroupedColumns.Count)
                        {
                            insertBeforeColumnName = TableDescriptor.GroupedColumns[redArrowFieldNum].Name;
                            GridColumnDescriptor cd = TableDescriptor.Columns.FindByMappingName(insertBeforeColumnName);
                            if (cd != null)
                            {
                                insertBeforeColumnName = cd.Name;
                            }
                        }

                        GridQueryAllowGroupByColumnEventArgs ae = new GridQueryAllowGroupByColumnEventArgs(this.grid, column.Name, insertBeforeColumnName, GridQueryAllowDragColumnReason.MouseUp, column.AllowGroupByColumn);
                        this.grid.RaiseQueryAllowGroupByColumn(ae);
                        if (ae.AllowGroupByColumn)
                        {
                            Cursor.Current = Cursors.WaitCursor;
                            if (grid.ControlEndEdit(true))
                            {
                                if (indexOfGroupedColumn != -1)
                                {
                                    TableDescriptor.GroupedColumns.RemoveAt(indexOfGroupedColumn);
                                }

                                TableDescriptor.GroupedColumns.Insert(redArrowFieldNum, new SortColumnDescriptor(column.MappingName));
                                try
                                {
                                    grid.Update();
                                }
                                catch (Exception ex)
                                {
                                    TraceUtil.TraceExceptionCatched(ex);
                                    grid.Table.TopLevelGroup.InvalidateCounterTopDown(true);
                                    grid.Invalidate();
                                }

                                GridGroupDropArea.Refresh();
                                grid.LeftColIndex = grid.Model.Cols.FrozenCount + 1;
                            }

                            Cursor.Current = Cursors.Default;
                        }
                    }
                }
                else
                {
                    if (ShouldShowRedArrowIndicator() && !headerSectionBounds.IsEmpty)
                    {
                        if (redArrowFieldNum >= 0)
                        {
                            string redArrowColumnName = string.Empty;
                            redArrowColumnName = TableDescriptor.Columns[redArrowFieldNum].Name;
                            GridQueryAllowDragColumnEventArgs ae = new GridQueryAllowDragColumnEventArgs(this.grid, column.Name, redArrowColumnName, GridQueryAllowDragColumnReason.MouseUp);
                            this.grid.RaiseQueryAllowDragColumn(ae);
                            if (ae.AllowDrag)
                            {
                                Table.RaiseDisplayElementChanging(Table, -1, -1, true, true, true, false);
                                if (TableDescriptor.VisibleColumns.IsModified)
                                {
                                    int n = TableDescriptor.VisibleColumns.IndexOf(column.Name);
                                    int nredarrow = TableDescriptor.VisibleColumns.IndexOf(TableDescriptor.Columns[redArrowFieldNum].Name);
                                    if (n >= 0)
                                    {
                                        if (n < nredarrow)
                                        {
                                            nredarrow--;
                                        }
                                    }

                                    Cursor.Current = Cursors.WaitCursor;
                                    if (n >= 0 && redArrowFieldNum != -1)
                                    {
                                        TableDescriptor.VisibleColumns.Move(n, nredarrow);
                                        ////                        else if (redArrowFieldNum == -1)
                                        ////                            TableDescriptor.Columns.RemoveAt(n);
                                        ////                        else
                                        ////                            TableDescriptor.Columns.Insert(redArrowFieldNum, sd);
                                        Table.RaiseDisplayElementChanged(Table, -1, -1, true, true, true, false);
                                    }
                                }
                                else
                                {
                                    int n = TableDescriptor.Columns.IndexOf(column);
                                    GridColumnDescriptor sd = column.Clone();
                                    if (n >= 0)
                                    {
                                        if (n < redArrowFieldNum)
                                        {
                                            redArrowFieldNum--;
                                        }
                                    }

                                    Cursor.Current = Cursors.WaitCursor;
                                    if (n >= 0 && redArrowFieldNum != -1)
                                    {
                                        TableDescriptor.Columns.Move(n, redArrowFieldNum);
                                        ////                        else if (redArrowFieldNum == -1)
                                        ////                            TableDescriptor.Columns.RemoveAt(n);
                                        ////                        else
                                        ////                            TableDescriptor.Columns.Insert(redArrowFieldNum, sd);
                                        Table.RaiseDisplayElementChanged(Table, -1, -1, true, true, true, false);
                                    }
                                }
                            }
                        }

                        ////grid.Update();
                        ////grid.LeftColIndex = grid.Model.Cols.FrozenCount+1;
                        if (GridGroupDropArea != null)
                        {
                            GridGroupDropArea.Refresh();
                        }

                        Cursor.Current = Cursors.Default;
                    }
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
        /// <param name="e">A <see cref="MouseEventArgs"/> holding event data.</param>
        /// <param name="controller">A <see cref="IMouseController"/> that has indicated to handle the mouse event.</param>
        /// <returns>A non-zero value if the button can and wants to handle the mouse event; 0 if the
        /// mouse event is unrelated for this button.</returns>
        public override int HitTest(MouseEventArgs e, IMouseController controller)
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
                hitTestInfo = new DragGroupHeaderHitTestInfo(grid, pt);
                if (hitTestInfo.hitTestResult == GridHitTestContext.None)
                {
                    hitTestInfo = null;
                }
                else
                {
                    GridQueryAllowDragColumnEventArgs ae = new GridQueryAllowDragColumnEventArgs(this.grid, hitTestInfo.columnDescriptor, GridQueryAllowDragColumnReason.HitTest, e);
                    ae.AllowDrag = e.Button == MouseButtons.Left;
                    this.grid.RaiseQueryAllowDragColumn(ae);
                    if (ae.AllowDrag)
                    {
                        this._rowIndex = hitTestInfo.rowIndex;
                        this._colIndex = hitTestInfo.colIndex;
                        this._cellRange = hitTestInfo.cellRange;
                        this.originBounds = grid.GridRectangleToScreen(grid.RangeInfoToRectangle(_cellRange));
                        this.origin = this.grid.GetGridWindow().LastMousePosition;
                        if (GridGroupDropArea != null)
                        {
                            Rectangle bnds = this.GridGroupDropArea.GridBounds;
                            Control c = this.GridGroupDropArea;
                            this.groupAreaBounds = c.RectangleToScreen(bnds);
                        }

                        if (grid.Table.TableDescriptor.ColumnSets.Count > 0)
                        {
                            this.headerSectionBounds = Rectangle.Empty;
                        }
                        else
                        {
                            this.headerSectionBounds = grid.GridRectangleToScreen(grid.RangeInfoToRectangle(GridRangeInfo.Row(_rowIndex)));
                        }

                        this.column = hitTestInfo.columnDescriptor;
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

        [Syncfusion.Documentation.DocumentationExclude()]
        internal sealed class DragGroupHeaderHitTestInfo
        {
            internal GridRangeInfo cellRange;
            internal int rowIndex;
            internal int colIndex;
            internal int hitTestResult = GridHitTestContext.None;
            internal Point point;
            internal GridColumnDescriptor columnDescriptor;
            ////internal int fieldNum;

            internal DragGroupHeaderHitTestInfo(GridTableControl grid, Point point)
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
                        if (el.ParentTable == grid.Model.Table && (el is ColumnHeaderRow || el is ColumnHeaderSection))
                        {
                            this.point = point;
                            ////fieldNum = grid.Model.ColIndexToField(colIndex);

                            GridTableCellStyleInfo style = grid.Model[rowIndex, colIndex];
                            columnDescriptor = style.TableCellIdentity.Column;

                            if (columnDescriptor != null && el != null)
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

    /// <summary>
    /// Mouse controller that provides support for dragging headers from the
    /// GroupDropArea.
    /// </summary>
    public class GroupDropAreaDragHeaderMouseController : GroupDragHeaderMouseControllerBase
    {
        /// <summary>
        /// Initializes the mouse controller with GridGroupDropArea it operates on
        /// </summary>
        /// <param name="grid">The group drop area.</param>
        public GroupDropAreaDragHeaderMouseController(GridGroupDropArea grid)
        {
            this.groupDropArea = grid;
            isGroupAreaOrigin = true;
            clickCellsController = new GridClickCellsMouseController(grid);
        }

        /// <summary>
        /// Returns the grid it is bound to.
        /// </summary>
        /// <returns>The GridTableControl</returns>
        /// <override/>
        protected override GridTableControl GetGridTableControl()
        {
            return (GridTableControl)groupDropArea.Model.GridTableModel.ActiveGridView;
        }

        private Bitmap CreateHeaderBitmap(int rowIndex, int colIndex)
        {
            Graphics g = null;
            Size size = new Size(GridGroupDropArea.GetColWidth(colIndex), GridGroupDropArea.GetRowHeight(rowIndex));
            Rectangle bounds = new Rectangle(Point.Empty, size);
            GridStyleInfo style = GridGroupDropArea.Model[rowIndex, colIndex];
            GridCellRendererBase headerCellRenderer = grid.CellRenderers[style.CellType];
            Bitmap bm = new Bitmap(size.Width, size.Height);

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

        #region MouseController Implementation

        /// <override/>
        /// <summary>
        /// MouseDown is called when this controller signaled in HitTest that it wants to handle mouse events and the
        /// user pressed the mouse button.
        /// </summary>
        /// <param name="e">A <see cref="MouseEventArgs"/> holding event data.</param>
        public override void MouseDown(MouseEventArgs e)
        {
            if (this.column == null)
            {
                base.MouseDown(e);
                return;
            }

            this.ResetClickCellsController();
            this.OpenDragHeader();
            this.OpenRedArrowIndicator();
            GridGroupDropArea.Capture = true;
        }

        /// <override/>
        /// <summary>
        /// MouseMove is called for the active controller after a MouseDown message when the user moves the mouse pointer.
        /// </summary>
        /// <param name="e">A <see cref="MouseEventArgs"/> holding event data.</param>
        public override void MouseMove(MouseEventArgs e)
        {
            if (this.column == null)
            {
                base.MouseMove(e);
                return;
            }

            this.ResetClickCellsController();

            if (!this.IsMouseOverGroupDropArea())
            {
                GridQueryAllowDragColumnEventArgs ae = new GridQueryAllowDragColumnEventArgs(this.grid, column, GridQueryAllowDragColumnReason.Remove, e);
                this.grid.RaiseQueryAllowDragColumn(ae);
                if (ae.AllowDrag)
                {
                    cursor = GridGroupingCursors.RemoveCursor;
                    cancelRemove = false;
                }
                else
                {
                    cursor = Cursors.Default;
                    cancelRemove = true;
                }
            }
            else
            {
                cursor = Cursors.Default;
            }

            this.UpdateRedArrowIndicator();
            this.UpdateDragHeader();

            this.wasDragged |= DragHeaderVisible;
        }

        /// <override/>
        /// <summary>
        /// MouseUp is called for the active controller after a MouseDown message when the user releases the mouse button.
        /// </summary>
        /// <param name="e">A <see cref="MouseEventArgs"/> holding event data.</param>
        public override void MouseUp(MouseEventArgs e)
        {
            if (this.column == null)
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

            GridGroupDropArea.Capture = false;

            if (!wasDragged)
            {
                GridGroupDropArea.RaiseCellClick(_rowIndex, _colIndex, e);
            }
            else if (bv)
            {
                int targetRow, targetCol;
                this.GridGroupDropArea.PointToRowCol(new Point(e.X, e.Y), out targetRow, out targetCol);
                if (redArrowGroupDropArea)
                {
                    GridQueryAllowGroupByColumnEventArgs ae = new GridQueryAllowGroupByColumnEventArgs(this.grid, column.Name, this.GridGroupDropArea.Model[targetRow, targetCol].Text, GridQueryAllowDragColumnReason.MouseUp, column.AllowGroupByColumn);
                    this.grid.RaiseQueryAllowGroupByColumn(ae);
                    if (ae.AllowGroupByColumn)
                    {
                        if (grid.ControlEndEdit(true))
                        {
                            int n = TableDescriptor.GroupedColumns.IndexOf(column.MappingName);
                            if (n != -1)
                            {
                                if (n < redArrowFieldNum)
                                {
                                    redArrowFieldNum--;
                                }

                                TableDescriptor.GroupedColumns.RemoveAt(n);
                            }

                            Cursor.Current = Cursors.WaitCursor;
                            if (redArrowFieldNum < TableDescriptor.GroupedColumns.Count)
                            {
                                TableDescriptor.GroupedColumns.Insert(redArrowFieldNum, new SortColumnDescriptor(column.MappingName));
                            }
                            else
                            {
                                TableDescriptor.GroupedColumns.Add(new SortColumnDescriptor(column.MappingName));
                            }

                            grid.Refresh();
                            GridGroupDropArea.Refresh();
                            grid.LeftColIndex = grid.Model.Cols.FrozenCount + 1;
                        }
                    }
                    Cursor.Current = Cursors.Default;
                }
                else if (!cancelRemove)
                {
                    Cursor.Current = Cursors.WaitCursor;
                    if (grid.ControlEndEdit(true))
                    {
                        GridQueryAllowGroupByColumnEventArgs ae = new GridQueryAllowGroupByColumnEventArgs(this.grid, column.Name, this.GridGroupDropArea.Model[targetRow, targetCol].Text, GridQueryAllowDragColumnReason.Remove, column.AllowGroupByColumn);
                        this.grid.RaiseQueryAllowGroupByColumn(ae);
                        if (ae.AllowGroupByColumn)
                        {
                            TableDescriptor.GroupedColumns.Remove(column.MappingName);
                            grid.Refresh();
                            grid.LeftColIndex = grid.Model.Cols.FrozenCount + 1;
                        }
                    }

                    GridGroupDropArea.Refresh();
                    Cursor.Current = Cursors.Default;
                }
            }
        }

        /// <override/>
        ///  <summary>
        /// CancelMode is called for the active controller after a MouseDown message when the mouse operation is cancelled.
        /// </summary>
        public override void CancelMode()
        {
            base.CancelMode();

            if (this.column == null)
            {
                return;
            }

            cursor = null;
            DragHeaderVisible = false;
            this.CloseDragHeader();
            this.CloseRedArrowIndicator();

            GridGroupDropArea.Capture = false;
        }
        
        /// <override/>
        /// <summary>
        /// HitTest is called to determine whether your controller wants to handle the mouse events based current context.
        /// </summary>
        /// <param name="e">A <see cref="MouseEventArgs"/> holding event data.</param>
        /// <param name="controller">A <see cref="IMouseController"/> that has indicated to handle the mouse event.</param>
        /// <returns>A non-zero value if the button can and wants to handle the mouse event; 0 if the
        /// mouse event is unrelated for this button.</returns>
        public override int HitTest(MouseEventArgs e, IMouseController controller)
        {
            int baseHitTest = base.HitTest(e, controller);
            if (grid != null && this.GridGroupDropArea != null && this.GridGroupDropArea.Model != null && this.GridGroupDropArea.Model.HasTable)
            {
                Point pt = new Point(e.X, e.Y);
                ////&& e.Clicks < 2
                if (CheckMouseButtons(e))
                {
                    this.groupAreaBounds = this.GridGroupDropArea.GridRectangleToScreen(this.GridGroupDropArea.GridBounds);
                    this.headerSectionBounds = grid.GridRectangleToScreen(grid.RangeInfoToRectangle(this.GetRangeOfColumnHeaderSection()));

                    if (this.IsMouseOverGroupDropArea())
                    {
                        pt = this.GridGroupDropArea.LastMousePosition;
                        pt = GridGroupDropArea.GridPointToClient(pt);
                        GridRangeInfo _cellRange = GridGroupDropArea.PointToRangeInfo(pt);
                        this._rowIndex = _cellRange.Top;
                        this._colIndex = _cellRange.Left;
                        column = GridGroupDropArea.Model.GetHeaderColumnDescriptorAt(_rowIndex, _colIndex);
                        this.originBounds = GridGroupDropArea.GridRectangleToScreen(GridGroupDropArea.RangeInfoToRectangle(_cellRange));
                        this.origin = this.GridGroupDropArea.LastMousePosition; ////this.GridGroupDropArea.LastMousePosition;
                        wasDragged = false;
                    }

                    if (column != null)
                    {
                        return GridHitTestContext.Header;
                    }
                }
            }

            return GridHitTestContext.Header;
        }
        #endregion
    }

    /// <internalonly/>
    /// <summary>Used internally.</summary>
    [Syncfusion.Documentation.DocumentationExclude]
    public class GroupDragHelper : IDisposable
    {
        // Fields
        private bool isDragging = false;
        internal DragDropEffects lastDragDropEffect = DragDropEffects.None;

        internal GridGroupDragWindow dragWindow = new GridGroupDragWindow();

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        public GridGroupDragWindow DragWindow
        {
            get
            {
                return dragWindow;
            }
        }

        //// Constructor for GroupDragHelper

        /// <summary>Used internally.</summary>
        /// <internalonly/>        
        public GroupDragHelper()
        {
        }

        ////Methods
        
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
        /// <summary>Used internally.</summary>
        protected void CheckDragCursor(DragDropEffects e)
        {
            return;
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        protected void StopDrag()
        {
            this.isDragging = false;
            this.lastDragDropEffect = DragDropEffects.None;
            this.dragWindow.StopDrag();
        }

        /// <internalonly/>
        /// <summary>Drags the group to the specified point.</summary>
        /// <param name="p">The specified point.</param>
        /// <param name="e">Specifies the effects of a Drag-Drop operation.</param>
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
        /// <summary>Cancels the dragging.</summary>
        public void CancelDrag()
        {
            if (!this.isDragging)
            {
                return;
            }

            this.StopDrag();
        }

        /// <internalonly/>
        /// <summary>Stops dragging.</summary>
        public void EndDrag()
        {
            if (!this.isDragging)
            {
                return;
            }

            this.StopDrag();
        }

        //// Properties

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        public DragDropEffects LastDragDropEffect
        {
            get
            {
                return this.lastDragDropEffect;
            }
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        public bool IsDragging
        {
            get
            {
                return this.isDragging;
            }
        }

        #region IDisposable Members
        /// <summary>
        /// Disposes the current object.
        /// </summary>
        public void Dispose()
        {
            this.dragWindow.Dispose();
            this.dragWindow = null;
        }
        #endregion
    }

    /// <internalonly/>
    /// <summary>Used internally.</summary>
    [ToolboxItem(false),
    Syncfusion.Documentation.DocumentationExclude()]
    public class GridGroupDragWindow : DragWindow
    {
        //// Fields
        private Bitmap dragBitmap = null;
        private bool isDragging = false;
        private Point origin = new Point(-30000, -30000);

        //// Constructor for GridGroupDragWindow

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        public GridGroupDragWindow()
        {
            if (GroupDragHeaderMouseControllerBase.SupportsTransparentForm())
            {
                this.TransparencyKey = Color.Red;
            }
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
        /// <param name="p">The point value.</param>
        /// <returns>returns boolean value to indicate start drag</returns>
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
        /// <param name="p">The point value.</param>
        /// <returns>returns the boolean value to indicate MoveTo</returns>
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
        /// <returns>returns boolean value to indicate stop drag</returns>
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

        //// Properties

        /// <internalonly/>
        /// <summary>Used internally.</summary>
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
                    Size size = value.Size; ////new Size(value.Size.Width*2, value.Size.Height*2); 
                    this.origin = Point.Empty;

                    //// Whidbey added a call to SetWindowPos in its Form.MinimumSize property setter. When this
                    //// method is called it uses flags that change the z-order. This is not wanted for
                    //// this DragWindow control.
                    ////
                    //// The problem can be avoided by destroying the window handle before setting the property.
                    //// It will be recreated later automatically with correct z-order.
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
        /// <summary>
        /// Determine the dispose.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.dragBitmap = null;
            }

            base.Dispose(disposing);
        }
    }
}