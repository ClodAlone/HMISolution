//-------------------------------------------------------------------------------------------------
// <copyright file="ScheduleMouseController.cs" company="syncfusion">
// Copyright (c) syncfusion.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Security;
using System.Security.Permissions;
using System.Windows.Forms;
using Syncfusion.Windows.Forms.Grid;
using Syncfusion.Windows.Forms;
using Syncfusion.Schedule;
using Syncfusion.Drawing;

namespace Syncfusion.Windows.Forms.Schedule
{
    internal enum ResizingCellsMode
    {
        /// <summary>
        /// No resizing.
        /// </summary>
        None = 0,

        /// <summary>
        /// Resize row.
        /// </summary>
        ResizeRow = 1,

        /// <summary>
        /// Resize column.
        /// </summary>
        ResizeColumn = 2
    }

	/// <summary>
	/// Used to handle mouse interactions.
	/// </summary>
	internal class ScheduleResizeCellsMouseController : GridMouseController
	{
		private bool inResizingCells = false;
		private Rectangle resizeBounds = Rectangle.Empty;
		private Rectangle originalInvertBarBounds = Rectangle.Empty;
		private ResizingCellsMode resizingCellsMode = ResizingCellsMode.None;
		private int rowResizing = 0;
		private int colResizing = 0;
		private bool outlineBounds = false;
		private bool drawInvertLine = false;
		private bool outlineHeader = false;
		private Size originalSize = Size.Empty;
		private Rectangle invertBarBounds = Rectangle.Empty;
		private bool movedMarker = false;
		private bool defaultSize = false;
		private ResizeCellsHitTestInfo hitTestInfo = null;
		private GridControlBase grid;
		private int dragOutsideValue = 0;
		
		[ThreadStaticAttribute] private static bool inDrag = false;
		int dxDrag;
		int dyDrag;

		[ThreadStaticAttribute] private static GridBorder sizeIndicatorBorder = null;
		[ThreadStaticAttribute] private static GridBorder boundsIndicatorBorder = null;
		[ThreadStaticAttribute] internal static ScheduleDragLineWindow _dragWindow = null;
		[ThreadStaticAttribute] internal static ScheduleDragLineWindow _oldBoundsWindow = null;
        [ThreadStaticAttribute] internal static ScheduleDragLineWindow _headerWindow = null;
        [ThreadStaticAttribute] internal static ScheduleDragLineWindow _navCellWindow = null;
        [ThreadStaticAttribute] internal static Color dragColor = Color.Red;
        [ThreadStaticAttribute] internal static bool isRowSizing = false;

        internal static ScheduleDragLineWindow navCellWindow
        {
            get
            {
                if (_navCellWindow == null)
                {
                    _navCellWindow = new ScheduleDragLineWindow();
                    navCellWindow.TransparencyKey = navCellWindow.BackColor;
                }

                return _navCellWindow;
            }
        }

        internal static ScheduleDragLineWindow headerWindow
        {
            get
            {
                if (_headerWindow == null)
                {
                    _headerWindow = new ScheduleDragLineWindow();
                }

                return _headerWindow;
            }
        }

        internal static ScheduleDragLineWindow dragWindow
		{
			get
			{
				if (_dragWindow == null)
				{
					_dragWindow = new ScheduleDragLineWindow();
				}

				return _dragWindow;
			}
		}

		internal static ScheduleDragLineWindow oldBoundsWindow
		{
			get
			{
				if (_oldBoundsWindow == null)
				{
					_oldBoundsWindow = new ScheduleDragLineWindow();
				}

				return _oldBoundsWindow;
			}
		}

		/// <override/>
        /// <returns>True if the control should be focused.</returns>
		public override bool GetAllowFixFocus()
		{
			return !inResizingCells;
		}

        /// <summary>
		/// Initializes a new ScheduleResizeCellsMouseController and attaches it to a grid.
		/// </summary>
		/// <param name="grid">The grid control.</param>
		public ScheduleResizeCellsMouseController(GridControlBase grid)
			: base(grid)
		{
			this.grid = grid;
			this.dxDrag = SystemInformation.DragSize.Width;
			this.dyDrag = SystemInformation.DragSize.Height;
		}

		bool HasResizeColOptions(GridResizeCellsBehavior flags) 
		{ 
			return (grid.Model.Options.ResizeColsBehavior & flags) != GridResizeCellsBehavior.None;
		}

		bool HasResizeRowOptions(GridResizeCellsBehavior flags) 
		{ 
			return (grid.Model.Options.ResizeRowsBehavior & flags) != GridResizeCellsBehavior.None;
		}

		bool CanResizeRows
		{
			get
			{
				return HasResizeRowOptions(GridResizeCellsBehavior.ResizeSingle|GridResizeCellsBehavior.ResizeAll);
			}
		}

		bool CanResizeCols
		{
			get
			{
				return HasResizeColOptions(GridResizeCellsBehavior.ResizeSingle|GridResizeCellsBehavior.ResizeAll);
			}
		}

		private Point mouseDownPoint = Point.Empty;
		void LeftMouseDown(Point point, MouseEventArgs e, int ht, int rowIndex, int colIndex)
		{
			mouseDownPoint = new Point(e.X, e.Y);
			if (ht == GridHitTestContext.VerticalLine && CanResizeCols)
			{
                if (BeginResizing(rowIndex, colIndex, ResizingCellsMode.ResizeColumn, point)
                    && !grid.Capture && !grid.CurrentCell.StaticDrawing)
                {
                    grid.Capture = true;
                }
			}
			else if (ht == GridHitTestContext.HorizontalLine && CanResizeRows)
			{
                if (BeginResizing(rowIndex, colIndex, ResizingCellsMode.ResizeRow, point)
                    && !grid.Capture && !grid.CurrentCell.StaticDrawing)
                {
                    grid.Capture = true;
                }
			}
		}

        private int oldCalendarRow = -1;
        private int oldCalendarCol = -1;
		void LeftMouseMove(Point point, MouseEventArgs e)
		{
 			try
			{
				////check if need to drag a schedule item dragged
				if (!inDrag 
					&& this.hitTestInfo.hitTestResult != GridHitTestContext.HorizontalLine
					&& (Math.Abs(this.mouseDownPoint.X - e.X) > this.dxDrag 
					|| Math.Abs(this.mouseDownPoint.Y - e.Y) > this.dyDrag))
				{
					inDrag = true;
                    isRowSizing = false;

					int w = 0;
					////sets the dragrange
					RaiseResizingColumns(GridRangeInfo.Col(hitTestInfo.colIndex), ref w, GridResizeCellsReason.HitTest, hitTestInfo.point);
		
					////trigers the drag
					this.hitTestInfo.hitTestResult = GridHitTestContext.VerticalLine;
					this.LeftMouseDown(hitTestInfo.point, e, hitTestInfo.hitTestResult, hitTestInfo.rowIndex, hitTestInfo.colIndex);
					inResizingCells = true;
					return;
				}
				
				////otherwise do normal processing to possible move the visual feedback
				if (inResizingCells)
				{
					Rectangle r = this.resizeBounds;
					Size size;
                    if (grid.IsRightToLeft())
                    {
                        size = new Size(r.Right - point.X, point.Y - r.Y);
                    }
                    else
                    {
                        size = new Size(point.X - r.X, point.Y - r.Y);
                    }

                    if (this.resizingCellsMode == ResizingCellsMode.ResizeRow)
                    {
                        size.Width = 0;
                        isRowSizing = true;
                    }
                    else
                    {
                        isRowSizing = false;
                        size.Height = 0;

                        ////code to do hit testing on the NavCalendar
                        ScheduleGrid grd = Grid as ScheduleGrid;
                        Point pt = grd.Calendar.PointToClient(grd.PointToScreen(point));

                        int row, col;
                        if (grd.Calendar.CalenderGrid.PointToRowCol(pt, out row, out col)
                            && (row != oldCalendarRow || col != oldCalendarCol))
                        {
                            oldCalendarCol = col;
                            oldCalendarRow = row;
                            DrawCalendarCellInvertRect(grd.Calendar.CalenderGrid);
                        }
                    }

					MoveMarker(size, point);
				}
			}
			catch (Exception ex)
			{
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                {
                    throw ex;
                }
			}
		}

		void LeftMouseUp(Point point, MouseEventArgs e)
		{
			inDrag = false;

			if (inResizingCells)
			{
				Rectangle r = this.resizeBounds;
				Size size;
                if (grid.IsRightToLeft())
                {
                    size = new Size(r.Right - point.X, point.Y - r.Y);
                }
                else
                {
                    size = new Size(point.X - r.X, point.Y - r.Y);
                }

                if (this.resizingCellsMode == ResizingCellsMode.ResizeRow)
                {
                    size.Width = 0;
                }
                else
                {
                    size.Height = 0;
                }
				
                EndResizing(size, point);
				grid.Capture = false;
			}
		}

		void LeftMouseDoubleClick(Point point, EventArgs e, int ht, int rowIndex, int colIndex)
		{
			int undefined = -1;
			
			//// check, if user clicked on a vertical line to resize or restore the column
			//// ... (behaviour is depending on Model.Options.ResizeColsBehavior and HeaderCount)

			if (ht == GridHitTestContext.VerticalLine && CanResizeCols)
			{
				GridRangeInfoList pSelList = grid.Model.Selections.Ranges;

				// Can column be resized?
				if (RaiseResizingColumns(GridRangeInfo.Col(colIndex), ref undefined, GridResizeCellsReason.DoubleClick, point))
				{
					const int _NDefault = -1;
					undefined = -1;

					if (colIndex > 0 && HasResizeColOptions(GridResizeCellsBehavior.ResizeAll))
					{
					}
					else if (grid.GetColHidden(colIndex)
						|| (colIndex < grid.Model.ColCount && grid.GetColHidden(colIndex+1)))
					{
						int fromColIndex = colIndex;
						int toColIndex = colIndex+1;
						int nCount = grid.Model.ColCount;
                        while (fromColIndex > 0 && grid.GetColHidden(fromColIndex - 1)
                            && RaiseResizingColumns(GridRangeInfo.Col(fromColIndex - 1), ref undefined, GridResizeCellsReason.ResetHide, point))
                        {
                            fromColIndex--;
                        }

                        while (toColIndex <= nCount && grid.GetColHidden(toColIndex)
                            && RaiseResizingColumns(GridRangeInfo.Col(toColIndex), ref undefined, GridResizeCellsReason.ResetHide, point))
                        {
                            toColIndex++;
                        }

                        if (toColIndex > fromColIndex &&
                            RaiseResizingColumns(GridRangeInfo.Cols(fromColIndex, toColIndex - 1), ref undefined, GridResizeCellsReason.ResetHide, point))
                        {
                            grid.Model.HideCols.SetRange(fromColIndex, toColIndex - 1, false);
                        }
					}
					else if (colIndex > 0 && pSelList.AnyRangeContains(GridRangeInfo.Col(colIndex)))
					{
						if (!pSelList.AnyRangeContains(GridRangeInfo.Table()))
						{
							GridRangeInfoList rl = pSelList.GetColRanges(GridRangeInfoType.Cols);

							//// TODO: WaitCursor?

							foreach (GridRangeInfo range in rl)
							{
								if (grid.GetColHidden(range.Left))
								{
                                    if (RaiseResizingColumns(range, ref undefined, GridResizeCellsReason.ResetHide, point))
                                    {
                                        grid.Model.HideCols.SetRange(range.Left, range.Right, false);
                                    }
								}
								else
								{
                                    if (RaiseResizingColumns(range, ref undefined, GridResizeCellsReason.ResetDefault, point))
                                    {
                                        grid.Model.ColWidths.SetRange(range.Left, range.Right, _NDefault);
                                    }
								}
							}
						}
					}
                    else if (RaiseResizingColumns(GridRangeInfo.Col(this.colResizing), ref undefined, GridResizeCellsReason.ResetDefault, point))
                    {
                        grid.Model.ColWidths[this.colResizing] = _NDefault;
                    }
				}
			}
			else if (ht == GridHitTestContext.HorizontalLine && CanResizeRows)
			{
                //// check, if user clicked on a horizontal line to resize or restore the row
                //// ... (behaviour is depending oh Model.Options.ResizeRowsBehavior and HeaderCount)
				GridRangeInfoList pSelList = grid.Model.Selections.Ranges;

				//// Can row be resized ?
				if (RaiseResizingRows(GridRangeInfo.Row(rowIndex), ref undefined, GridResizeCellsReason.DoubleClick, point))
				{
					const int _NDefault = -1;
					undefined = -1;

					if (rowIndex > 0 && HasResizeRowOptions(GridResizeCellsBehavior.ResizeAll))
					{
					}
					else if (grid.GetRowHidden(rowIndex)
						|| ((rowIndex < grid.Model.RowCount) && grid.GetRowHidden(rowIndex+1))
						&& RaiseResizingRows(GridRangeInfo.Row(rowIndex+1), ref undefined, GridResizeCellsReason.ResetHide, point))
					{
						int fromRowIndex = rowIndex;
						int toRowIndex = rowIndex+1;
						int nCount = grid.Model.RowCount;
                        while (fromRowIndex > 0 && grid.GetRowHidden(fromRowIndex - 1)
                            && RaiseResizingRows(GridRangeInfo.Row(fromRowIndex - 1), ref undefined, GridResizeCellsReason.ResetHide, point))
                        {
                            fromRowIndex--;
                        }

                        while (toRowIndex <= nCount && grid.GetRowHidden(toRowIndex)
                            && RaiseResizingRows(GridRangeInfo.Row(toRowIndex), ref undefined, GridResizeCellsReason.ResetHide, point))
                        {
                            toRowIndex++;
                        }

                        if (toRowIndex > fromRowIndex &&
                            RaiseResizingRows(GridRangeInfo.Rows(fromRowIndex, toRowIndex - 1), ref undefined, GridResizeCellsReason.ResetHide, point))
                        {
                            grid.Model.HideRows.SetRange(fromRowIndex, toRowIndex - 1, false);
                        }
					}
					else if (rowIndex > 0 && pSelList.AnyRangeContains(GridRangeInfo.Row(rowIndex)))
					{
						if (!pSelList.AnyRangeContains(GridRangeInfo.Table()))
						{
							GridRangeInfoList rl = pSelList.GetRowRanges(GridRangeInfoType.Rows);

							//// TODO: WaitCursor ?
							foreach (GridRangeInfo range in rl)
							{
								if (grid.GetRowHidden(range.Top))
								{
                                    if (RaiseResizingRows(range, ref undefined, GridResizeCellsReason.ResetHide, point))
                                    {
                                        grid.Model.HideRows.SetRange(range.Top, range.Bottom, false);
                                    }
								}
								else
								{
                                    if (RaiseResizingRows(range, ref undefined, GridResizeCellsReason.ResetDefault, point))
                                    {
                                        grid.Model.RowHeights.SetRange(range.Top, range.Bottom, _NDefault);
                                    }
								}
							}
						}
					}
                    else if (RaiseResizingRows(GridRangeInfo.Row(this.rowResizing), ref undefined, GridResizeCellsReason.ResetDefault, point))
                    {
                        grid.Model.RowHeights[this.rowResizing] = _NDefault;
                    }
				}
			}
			else 
			{
                // no hit - pass it back to the grid as a double click
				Point pt = this.grid.PointToClient(Control.MousePosition);
				GridRangeInfo range = this.grid.Model.CoveredRanges.FindRange(rowIndex, colIndex);
				if (!range.IsEmpty)
				{
					rowIndex = range.Top;
					colIndex = range.Left;
				}

				this.grid.RaiseCellDoubleClick(rowIndex, colIndex, new MouseEventArgs(MouseButtons.Left, 2, pt.X, pt.Y, 0));
			}
		}

        private GridRangeInfo oldCalendarCell = GridRangeInfo.Empty;
       
        void DrawCalendarCellInvertRect(GridControl grid1)
        {
            ////get point under mouse
            Point pt = grid1.PointToClient(Control.MousePosition);

            ////get range under point
            GridRangeInfo range = grid1.PointToRangeInfo(pt);
            
            if (oldCalendarCell != range)
            {
                oldCalendarCell = range;

                ////get rectangle of covered range
                Rectangle rect = grid1.RangeInfoToRectangle(range);

                Rectangle bounds = grid1.GridRectangleToScreen(rect);
                navCalendarCenterPoint = new Point(bounds.X + (bounds.Width / 2), bounds.Y + (bounds.Height / 2));
                navCellWindow.ShowAt(bounds, new GridBorder(GridBorderStyle.Solid, dragColor, GridBorderWeight.ExtraExtraThick));
            }
        }

        private GridRangeInfo oldHeaderRange = GridRangeInfo.Empty;
        internal static Point navCalendarCenterPoint = Point.Empty;
        
        void DrawHeaderInvertRect(bool ignoreOldHeaderRange)
        {
            if (this.hitTestInfo.hitTestResult == GridHitTestContext.VerticalLine && ((ScheduleGrid)this.Grid).numberPanels > 1)
            {
                ////get point under mouse
                Point pt = grid.PointToClient(Control.MousePosition);

               ////get range under point
                GridRangeInfo range = grid.PointToRangeInfo(pt);
                ////get coveredcell at top of column
                range = grid.Model.CoveredRanges.FindRange(1, range.Left);
                if ((ignoreOldHeaderRange || oldHeaderRange != range) && range.Left > 1)
                {
                    oldHeaderRange = range;

                    ////get rectangle of covered range
                    Rectangle rect = grid.RangeInfoToRectangle(grid.Model.CoveredRanges.FindRange(range.Top, range.Left));

                    ////move rectangle under the top row and set its height
                    rect.Y = grid.Model.RowHeights[0];
                    rect.Height = grid.Model.RowHeights[0] / 3;

                    Rectangle bounds = grid.GridRectangleToScreen(rect);
                    navCalendarCenterPoint = Point.Empty;
                    headerWindow.ShowAt(bounds, new GridBorder(GridBorderStyle.Solid, dragColor, GridBorderWeight.ExtraExtraThick));
                }
            }
        }

		void DrawInvertRect(Rectangle rect, bool setOrReset)
		{
			if (!setOrReset || sizeIndicatorBorder == null || sizeIndicatorBorder.Style == GridBorderStyle.None)
			{
				dragWindow.Fade();
                headerWindow.Fade();
                navCellWindow.Fade();
			}
			else
			{
                int x = sizeIndicatorBorder.Width-1;
				rect.Inflate(x/2, x/2);
				rect.Intersect(grid.RangeInfoToRectangle(grid.ViewLayout.VisibleCellsRange));
				rect.Intersect(grid.GetVisibleBounds());
				Rectangle bounds = grid.GridRectangleToScreen(rect);
                if (bounds == oldBoundsWindow.Bounds)
                {
                    dragWindow.Fade();
                }
                else
                {
                    dragWindow.ShowAt(bounds, sizeIndicatorBorder);
                }
			}
		}

		void DrawMarkerRect(Rectangle rect, bool setOrReset)
		{
			if (!setOrReset || boundsIndicatorBorder == null || boundsIndicatorBorder.Style == GridBorderStyle.None)
			{
				oldBoundsWindow.Fade();
			}
			else
			{
				int x = boundsIndicatorBorder.Width-1;

				rect.Inflate(x/2, x/2);
				rect.Intersect(grid.GridBounds);
				rect.Intersect(grid.GetVisibleBounds());
				Rectangle bounds = grid.GridRectangleToScreen(rect);
				oldBoundsWindow.ShowAt(bounds, boundsIndicatorBorder);
			}
		}
		
		bool BeginResizing(int rowIndex, int colIndex, ResizingCellsMode nResizingCellsMode, Point point)
		{
            sizeIndicatorBorder = new GridBorder(
                GridBorderStyle.Dashed, 
                grid.Model.Properties.ResizingCellsLinesColor, 
                GridBorderWeight.Medium);
            boundsIndicatorBorder = new GridBorder(
                GridBorderStyle.Dotted,
                ////grid.Model.Properties.GridLineColor
                Color.Green,
                GridBorderWeight.Thick);

			this.dragOutsideValue = 0;

			// Check for headers in case of hidden columns.
            if (nResizingCellsMode == ResizingCellsMode.ResizeColumn
                && !((colIndex > grid.InternalGetHeaderCols()) || (!HasResizeColOptions(GridResizeCellsBehavior.IgnoreHeaders)))
                || nResizingCellsMode == ResizingCellsMode.ResizeRow
                && !((rowIndex > grid.InternalGetHeaderRows()) || (!HasResizeRowOptions(GridResizeCellsBehavior.IgnoreHeaders))))
            {
                return false;
            }

			//// Trigger events and check return values.
			int undefined = -1;
            if (nResizingCellsMode == ResizingCellsMode.ResizeColumn
                && (!RaiseResizingColumns(GridRangeInfo.Col(colIndex), ref undefined, GridResizeCellsReason.MouseDown, point))
                || (nResizingCellsMode == ResizingCellsMode.ResizeRow)
                && (!RaiseResizingRows(GridRangeInfo.Row(rowIndex), ref undefined, GridResizeCellsReason.MouseDown, point)))
            {
                return false;
            }

			//// Action is allowed.

            if (nResizingCellsMode == ResizingCellsMode.ResizeColumn)
            {
                colIndex = 2;
            }

            if (nResizingCellsMode == ResizingCellsMode.ResizeRow)
            {
                colIndex = 0;
            }

			Rectangle gridBounds = grid.GridBounds;
			Rectangle rectHit = grid.RangeInfoToRectangle(GridRangeInfo.Cell(rowIndex, colIndex));
			this.resizingCellsMode = nResizingCellsMode;
			this.rowResizing = rowIndex;
			this.colResizing = colIndex;

			rectHit.Offset(-1, -1);

			// ResizingCells line and bounding rectangle.
			if (this.resizingCellsMode == ResizingCellsMode.ResizeColumn)
			{
				// Cache the options.
				this.outlineBounds = grid.Model.Properties.DisplayVertLines && HasResizeColOptions(GridResizeCellsBehavior.OutlineBounds);
				this.drawInvertLine = true;
				this.outlineHeader = HasResizeColOptions(GridResizeCellsBehavior.OutlineHeaders);
				this.originalSize = new Size(grid.GetColWidth(colIndex), 0);

                if (this.HasResizeColOptions(GridResizeCellsBehavior.AllowDragOutside))
                {
                    this.dragOutsideValue = 1000;
                }

				// Check if column has an individual width
				this.defaultSize = !grid.Model.ColWidths.IsDefault(this.colResizing);

				// determine the bounding rectangle
				int yMax = grid.ViewLayout.Corner.Y; 
				if (grid.IsRightToLeft())
				{
					this.resizeBounds = Rectangle.FromLTRB(rectHit.Right-1, gridBounds.Top, rectHit.Right, yMax);
					this.originalInvertBarBounds = Rectangle.FromLTRB(rectHit.Left-1, gridBounds.Top, rectHit.Left, yMax);
				}
				else
				{
					this.resizeBounds = Rectangle.FromLTRB(rectHit.Left, rectHit.Top, rectHit.Left+1, rectHit.Bottom);
					this.originalInvertBarBounds = Rectangle.FromLTRB(rectHit.Right, rectHit.Top, rectHit.Right+1, rectHit.Bottom);
				}
			}
			else if (this.resizingCellsMode == ResizingCellsMode.ResizeRow)
			{
				// Cache the options.
				this.outlineBounds = grid.Model.Properties.DisplayHorzLines
					&& HasResizeRowOptions(GridResizeCellsBehavior.OutlineBounds);
				this.drawInvertLine = true;
				this.outlineHeader = HasResizeRowOptions(GridResizeCellsBehavior.OutlineHeaders);
				this.originalSize = new Size(0, grid.GetRowHeight(rowIndex));

                if (this.HasResizeRowOptions(GridResizeCellsBehavior.AllowDragOutside))
                {
                    this.dragOutsideValue = 1000;
                }

				// Check if row has an individual height.
				this.defaultSize = !grid.Model.RowHeights.IsDefault(this.rowResizing);

				// Determine the bounding rectangle.
				int xMax = grid.ViewLayout.Corner.X; 
				if (grid.IsRightToLeft())
				{
					this.resizeBounds = Rectangle.FromLTRB(xMax, rectHit.Top, gridBounds.Right, rectHit.Top+1);
					this.originalInvertBarBounds = Rectangle.FromLTRB(xMax, rectHit.Bottom, gridBounds.Right, rectHit.Bottom+1);
				}
				else
				{
					////this.resizeBounds = Rectangle.FromLTRB(gridBounds.Left, rectHit.Top, xMax, rectHit.Top+1);
					Rectangle topRect = this.Grid.RangeInfoToRectangle(GridRangeInfo.Row(this.Grid.TopRowIndex));

					this.resizeBounds = Rectangle.FromLTRB(gridBounds.Left, topRect.Top, xMax, topRect.Top+1);
					this.originalInvertBarBounds = Rectangle.FromLTRB(gridBounds.Left, rectHit.Bottom, xMax, rectHit.Bottom+1);
				}
			}

			//// Mark the line of the column which will be sized.
			if (this.outlineBounds)
			{
				this.DrawMarkerRect(originalInvertBarBounds, true);
			}

			this.movedMarker = false;   //// will be set true when user moved the line

			this.invertBarBounds = this.originalInvertBarBounds;

			inResizingCells = true;

            if (this.outlineHeader)
            {
                grid.InternalInvalidate(rectHit); //// Redraw button (in pressed state)
            }

			return true;
		}
		
		private int topRow = -1;
		private int topRowCol = -1;
		
        void MoveMarker(Size size, Point point)
		{
			this.movedMarker = true;    //// indicates that user has moved the cursor
			int rowIndex = this.rowResizing;
			int colIndex = this.colResizing;
			bool b = false;

			//// Trigger events.
			if (this.resizingCellsMode == ResizingCellsMode.ResizeColumn)
			{
                if (size.Width < (int)grid.Model.Options.MinResizeColSize)
                {
                    size.Width = (int)grid.Model.Options.MinResizeColSize;
                }

				int width = size.Width;
				b = RaiseResizingColumns(GridRangeInfo.Col(colIndex), ref width, GridResizeCellsReason.MouseMove, point);
				size.Width = width;
			}
			else if (this.resizingCellsMode == ResizingCellsMode.ResizeRow)
			{
				int height = size.Height;
				b = RaiseResizingRows(GridRangeInfo.Row(rowIndex), ref height, GridResizeCellsReason.MouseMove, point);
				size.Height = height;
			}

            if (!b)
            {
                return;
            }
			
			Rectangle gridBounds = grid.GridBounds;
			Rectangle resizeBounds = this.resizeBounds;
			Rectangle originalInvertBarBounds = this.originalInvertBarBounds;
			Size originalSize = this.originalSize;
			Point pt;
            if (this.resizingCellsMode == ResizingCellsMode.ResizeColumn)
            {
                pt = this.Grid.PointToClient(Control.MousePosition); ////resizeBounds.Location - size;
                pt.Offset(0, this.tOffSet);
                if (this.Grid.PointToRowCol(pt, out topRow, out topRowCol))
                {
                    pt = this.Grid.RangeInfoToRectangle(GridRangeInfo.Cell(topRow, topRowCol)).Location;
                    pt.Offset(1, 1);
                }
            }
            else
            {
                pt = resizeBounds.Location + size;
            }

			Rectangle oldRect = this.invertBarBounds;

			if (this.resizingCellsMode == ResizingCellsMode.ResizeColumn)
			{
				//// New grid line.
				Rectangle invertBarBounds;
				int numRows = this.returnedHeight / this.Grid.DefaultRowHeight;
				int	bOffSet = this.returnedHeight - tOffSet;
				
				invertBarBounds = Rectangle.FromLTRB(
					originalInvertBarBounds.Left - 4,
					Math.Min(gridBounds.Bottom+dragOutsideValue-7, pt.Y - tOffSet),
					////originalInvertBarBounds.Right,
					originalInvertBarBounds.Left,
					Math.Min(gridBounds.Bottom+dragOutsideValue-6, pt.Y + bOffSet));

				this.invertBarBounds = invertBarBounds;

                if (oldRect != invertBarBounds 
                    && this.Grid.ClientRectangle.Contains(this.Grid.PointToClient(Control.MousePosition)))
                {
                    if (this.drawInvertLine)
                    {
                        //// Undo previous inversion.
                        if (oldRect != originalInvertBarBounds)
                        {
                            DrawInvertRect(oldRect, false);
                        }
                        //// Show new size.
                        if (invertBarBounds != originalInvertBarBounds)
                        {
                            DrawInvertRect(invertBarBounds, true);
                        }
                        ////force a redraw of the header marker
                        DrawHeaderInvertRect(true);
                    }
                }
                else
                {
                    ////draw header marker if necessary
                    DrawHeaderInvertRect(false);
                }
			}
			else if (this.resizingCellsMode == ResizingCellsMode.ResizeRow)
			{
				//// Compute new grid line.
				Rectangle invertBarBounds = Rectangle.FromLTRB(
					originalInvertBarBounds.Left,
					Math.Min(gridBounds.Bottom+dragOutsideValue-7, Math.Max(pt.Y, resizeBounds.Top)),
					originalInvertBarBounds.Right,
					Math.Min(gridBounds.Bottom+dragOutsideValue-6, Math.Max(pt.Y+1, resizeBounds.Bottom)));

				this.invertBarBounds = invertBarBounds;

				if (oldRect != this.invertBarBounds)
				{
					// Undo previous inversion.
					if (this.drawInvertLine)
					{
                        if (oldRect != this.originalInvertBarBounds)
                        {
                            DrawInvertRect(oldRect, false);
                        }

						// Show new size.
                        if (this.drawInvertLine && this.invertBarBounds != this.originalInvertBarBounds)
                        {
                            DrawInvertRect(invertBarBounds, true);
                        }
					}
				}
			}
		}
		
		void CancelResizing()
		{
			inResizingCells = false;

			//// Redraw the marked grid line.
			DrawMarkerRect(Rectangle.Empty, false);
			DrawInvertRect(Rectangle.Empty, false);

			Rectangle gridBounds = grid.GridBounds;
			int rowIndex = this.rowResizing;
			int colIndex = this.colResizing;

            //// Undo previous inversion.
            ////			Rectangle invertBarBounds = this.invertBarBounds;
            ////			Rectangle originalInvertBarBounds = this.originalInvertBarBounds;
            ////			if (this.drawInvertLine && invertBarBounds != originalInvertBarBounds)
            ////			{
            ////				grid.Invalidate(invertBarBounds);
            ////				//InvertRect(invertBarBounds);
            ////			}
               
			Size originalSize = this.originalSize;

			int undefined = -1;
			if (this.resizingCellsMode == ResizingCellsMode.ResizeColumn)
			{
				RaiseResizingColumns(GridRangeInfo.Col(this.colResizing), ref undefined, GridResizeCellsReason.CancelMode, Point.Empty);

                if (this.outlineHeader)
                {
                    grid.InvalidateRange(GridRangeInfo.Cell(0, this.colResizing));
                }
			}
			else if (this.resizingCellsMode == ResizingCellsMode.ResizeRow)
			{
				RaiseResizingRows(GridRangeInfo.Row(this.rowResizing), ref undefined, GridResizeCellsReason.CancelMode, Point.Empty);

				//// Redraw the Cells
                if (this.outlineHeader)
                {
                    grid.InvalidateRange(GridRangeInfo.Cell(this.rowResizing, 0));
                }
			}
		}
		
		void EndResizing(Size size, Point point)
		{
            if (this.movedMarker)
            {
                MoveMarker(size, point);
            }

			DrawMarkerRect(Rectangle.Empty, false);
			DrawInvertRect(Rectangle.Empty, false);

			int rowIndex = this.rowResizing;
			int colIndex = this.colResizing;
			
			if (this.resizingCellsMode == ResizingCellsMode.ResizeColumn)
			{
                if (size.Width < (int)grid.Model.Options.MinResizeColSize)
                {
                    size.Width = (int)grid.Model.Options.MinResizeColSize;
                }
			}
			else if (this.resizingCellsMode == ResizingCellsMode.ResizeRow)
			{
                if (size.Height < (int)grid.Model.Options.MinResizeRowSize)
                {
                    size.Height = (int)grid.Model.Options.MinResizeRowSize;
                }
			}

			Rectangle resizeBounds = this.resizeBounds;
			Size originalSize = this.originalSize;
			Point pt;
            if (grid.IsRightToLeft() && this.resizingCellsMode == ResizingCellsMode.ResizeColumn)
            {
                pt = resizeBounds.Location - size;
            }
            else
            {
                pt = resizeBounds.Location + size;
            }

			Rectangle gridBounds = grid.GridBounds;

			GridRangeInfoList pSelList = grid.Model.Selections.Ranges;

			inResizingCells = false;

			if ((this.resizingCellsMode & ResizingCellsMode.ResizeColumn) != 0)
			{
				//// Change the column widths and update the display.
				Rectangle invertBarBounds = this.invertBarBounds;
				Rectangle originalInvertBarBounds = this.originalInvertBarBounds;
				int dx = invertBarBounds.Left - originalInvertBarBounds.Left;
                if (grid.IsRightToLeft())
                {
                    dx = -dx;
                }

				int newWidth = dx + grid.GetColWidth(this.colResizing);
				
				//// Trigger event and check return value.
				if (RaiseResizingColumns(GridRangeInfo.Col(colIndex), ref newWidth, GridResizeCellsReason.MouseUp, point)
					&& Math.Abs(dx) > 1 && this.movedMarker)
				{
					if (this.colResizing > 0 && HasResizeColOptions(GridResizeCellsBehavior.ResizeAll))
					{
                        if (RaiseResizingColumns(GridRangeInfo.Table(), ref newWidth, GridResizeCellsReason.MouseUp, point))
                        {
                            grid.Model.Cols.DefaultSize = newWidth;
                        }
					}
					else if (this.colResizing > 0 && pSelList.AnyRangeContains(GridRangeInfo.Col(this.colResizing)))
					{
						if (pSelList.AnyRangeContains(GridRangeInfo.Table()))
						{
                            if (RaiseResizingColumns(GridRangeInfo.Table(), ref newWidth, GridResizeCellsReason.MouseUp, point))
                            {
                                grid.Model.Cols.DefaultSize = newWidth;
                            }
						}
						else
						{
							GridRangeInfoList rl = pSelList.GetColRanges(GridRangeInfoType.Cols);

							//// TODO: WaitCursor?

							foreach (GridRangeInfo range in rl)
							{
								if (RaiseResizingColumns(range, ref newWidth, GridResizeCellsReason.MouseUp, point))
								{
                                    if (newWidth > 0)
                                    {
                                        grid.Model.ColWidths.SetRange(range.Left, range.Right, newWidth);
                                    }
                                    else
                                    {
                                        grid.Model.HideCols.SetRange(range.Left, range.Right, true);
                                    }
								}
							}
						}
					}
					else
					{
                        if (newWidth > 0)
                        {
                            grid.Model.ColWidths[this.colResizing] = newWidth;
                        }
                        else
                        {
                            grid.Model.HideCols[this.colResizing] = true;
                        }
					}
				}
				else
				{
					grid.InternalInvalidate(grid.RangeInfoToRectangle(GridRangeInfo.Cell(0, this.colResizing)));
					int nOldWidth = grid.GetColWidth(this.colResizing);
					UpdateColWidths(this.colResizing, this.colResizing, new int[] { nOldWidth });
				}

				//// Just in some rare case that column is not in view area.
				grid.ScrollCellInView(grid.TopRowIndex, this.colResizing, GridScrollCurrentCellReason.ResizedCells);
			}
			else
			{
				// Change the row heights and update the display.
				Rectangle invertBarBounds = this.invertBarBounds;
				Rectangle originalInvertBarBounds = this.originalInvertBarBounds;
				int dy = invertBarBounds.Top - originalInvertBarBounds.Top;
				
				int newHeight = dy + grid.GetRowHeight(this.rowResizing);

				// Trigger event and check return value.
				if (RaiseResizingRows(GridRangeInfo.Row(rowIndex), ref newHeight, GridResizeCellsReason.MouseUp, point)
					&& Math.Abs(dy) > 1 && this.movedMarker)
				{
					if (this.rowResizing > 0 && HasResizeRowOptions(GridResizeCellsBehavior.ResizeAll))
					{
                        if (RaiseResizingRows(GridRangeInfo.Table(), ref newHeight, GridResizeCellsReason.MouseUp, point))
                        {
                            grid.Model.Rows.DefaultSize = newHeight;
                        }
					}
					else if (this.rowResizing > 0 && pSelList.AnyRangeContains(GridRangeInfo.Row(this.rowResizing)))
					{
						if (pSelList.AnyRangeContains(GridRangeInfo.Table()))
						{
                            if (RaiseResizingRows(GridRangeInfo.Table(), ref newHeight, GridResizeCellsReason.MouseUp, point))
                            {
                                grid.Model.Rows.DefaultSize = newHeight;
                            }
						}
						else
						{
							GridRangeInfoList rl = pSelList.GetRowRanges(GridRangeInfoType.Rows);

							//// TODO: WaitCursor? 

							foreach (GridRangeInfo range in rl)
							{
								if (RaiseResizingRows(range, ref newHeight, GridResizeCellsReason.MouseUp, point))
								{
                                    if (newHeight > 0)
                                    {
                                        grid.Model.RowHeights.SetRange(range.Top, range.Bottom, newHeight);
                                    }
                                    else
                                    {
                                        grid.Model.HideRows.SetRange(range.Top, range.Bottom, true);
                                    }
								}
							}
						}
					}
					else
					{
                        if (newHeight > 0)
                        {
                            grid.Model.RowHeights[this.rowResizing] = newHeight;
                        }
                        else
                        {
                            grid.Model.HideRows[this.rowResizing] = true;
                        }
					}
				}
				else
				{
					grid.InternalInvalidate(grid.RangeInfoToRectangle(GridRangeInfo.Cell(this.rowResizing, 0)));
					int nOldHeight = grid.GetRowHeight(this.rowResizing);
					UpdateRowHeights(this.rowResizing, this.rowResizing, new int[] { nOldHeight });
				}

				//// just in some rare case that row is not in view area
				grid.ScrollCellInView(this.rowResizing, grid.LeftColIndex, GridScrollCurrentCellReason.ResizedCells);
			}
		}

		void UpdateRowHeights(int fromRowIndex, int toRowIndex, int[] savedHeights)
		{
			grid.ViewLayout.Reset();
			GridRangeInfo rowRange = GridRangeInfo.Rows(fromRowIndex, toRowIndex);
			GridRangeInfo range = grid.ViewLayout.CombineSpannedRanges(rowRange);
            if (grid.ViewLayout.IsRangeVisible(range))
            {
                grid.InternalInvalidate(grid.ViewLayout.RectangleBottomOfRow(range.Top, GridCellSizeKind.VisibleSize));
            }
		}

		void UpdateColWidths(int fromColIndex, int toColIndex, int[] savedWidths)
		{
			grid.ViewLayout.Reset();
			Rectangle bounds = grid.GridBounds;

			GridRangeInfo colRange = GridRangeInfo.Cols(fromColIndex, toColIndex);
			
			GridRangeInfo range = grid.ViewLayout.CombineSpannedRanges(colRange);
			if (grid.Model.Options.FloatCellsMode != GridFloatCellsMode.None)
			{
				grid.Model.FloatingCells.DelayFloatCells(range);
				grid.InternalInvalidate(grid.GridBounds);
			}
            else if (grid.ViewLayout.IsRangeVisible(range))
            {
                grid.InternalInvalidate(grid.ViewLayout.RectangleRightOfCol(range.Left, GridCellSizeKind.VisibleSize));
            }
		}

		[Syncfusion.Documentation.DocumentationExclude()]
			internal sealed class ResizeCellsHitTestInfo
		{
			const int _HitTestFrame = 4;
			internal int hitTestResult = GridHitTestContext.None;
			internal Point point;
			internal Rectangle cellBounds = Rectangle.Empty;
			internal int clientCol;
			internal int clientRow;
			internal int rowIndex;
			internal int colIndex;
			internal ScheduleResizeCellsMouseController resizeUI;
			static int dxDrag = SystemInformation.DragSize.Width;
			static int dyDrag = SystemInformation.DragSize.Height;

			internal ResizeCellsHitTestInfo(ScheduleResizeCellsMouseController resizeUI, GridControlBase grid, Point point, GridResizeCellsReason reason)
			{
				this.resizeUI = resizeUI;
				this.point = point;
				clientCol = grid.ViewLayout.PointToClientCol(point, false, GridCellSizeKind.VisibleSize);
				
				clientRow = grid.ViewLayout.PointToClientRow(point, false, GridCellSizeKind.VisibleSize);
				if (clientCol >= 0 && clientRow >= 0)
				{
					rowIndex = grid.GetRow(clientRow);
					colIndex = grid.GetCol(clientCol);

					//// special case for last row and column
					if (colIndex == grid.Model.ColCount+1 && rowIndex == grid.Model.RowCount+1)
					{
						cellBounds = grid.RangeInfoToRectangle(GridRangeInfo.Cell(rowIndex-1, colIndex-1));
						cellBounds.Offset(grid.Model.ColWidths[colIndex-1], grid.Model.RowHeights[rowIndex-1]);
					}
					else if (colIndex == grid.Model.ColCount+1)
					{
						cellBounds = grid.RangeInfoToRectangle(GridRangeInfo.Cell(rowIndex, colIndex-1));
						cellBounds.Offset(grid.Model.ColWidths[colIndex-1], 0);
					}
                    else if (rowIndex == grid.Model.RowCount + 1)
                    {
                        cellBounds = grid.RangeInfoToRectangle(GridRangeInfo.Cell(rowIndex - 1, colIndex));
                        cellBounds.Offset(0, grid.Model.RowHeights[rowIndex - 1]);
                    }
                    else
                    {
                        cellBounds = grid.RangeInfoToRectangle(GridRangeInfo.Cell(rowIndex, colIndex));
                    }

					if (clientCol <= grid.ViewLayout.VisibleCols && clientRow <= grid.ViewLayout.VisibleRows
						&& rowIndex <= grid.Model.RowCount+1 && colIndex <= grid.Model.ColCount+1)
					{
						bool shouldResizeCol = resizeUI.CanResizeCols
							&& (clientRow == 0 || resizeUI.HasResizeColOptions(GridResizeCellsBehavior.InsideGrid));

						bool shouldResizeRow = resizeUI.CanResizeRows
							&& (clientCol == 0 || resizeUI.HasResizeRowOptions(GridResizeCellsBehavior.InsideGrid));

						if (shouldResizeRow && Math.Abs(cellBounds.Bottom-point.Y) <= _HitTestFrame/2)
						{
                            if (clientRow > grid.InternalGetHeaderRows() || !resizeUI.HasResizeRowOptions(GridResizeCellsBehavior.IgnoreHeaders))
                            {
                                hitTestResult = GridHitTestContext.HorizontalLine;
                            }
						}
						else if (shouldResizeRow && Math.Abs(cellBounds.Top-point.Y) <= _HitTestFrame/2 && clientRow > 0)
						{
							if (clientRow > grid.InternalGetHeaderRows()+1 || !resizeUI.HasResizeRowOptions(GridResizeCellsBehavior.IgnoreHeaders))
							{
								while (clientRow > 0 && grid.GetRowHeight(grid.GetRow(--clientRow)) == 0)
								{
                                }

                                if (clientRow > 0 || grid.ShouldDisplayHeaderRow())
                                {
                                    hitTestResult = GridHitTestContext.HorizontalLine;
                                }

								rowIndex = grid.GetRow(clientRow);
								cellBounds = grid.RangeInfoToRectangle(GridRangeInfo.Cell(rowIndex, colIndex));
							}
						}
						else 
						{
								hitTestResult = GridHitTestContext.VerticalLine;
						}

						//// special case for last row and column when mouse pointer is right of last column 
                        if (rowIndex > grid.Model.RowCount || colIndex > grid.Model.ColCount)
                        {
                            hitTestResult = GridHitTestContext.None;
                        }
					}

					int undefined = -1;
					
					if (hitTestResult == GridHitTestContext.VerticalLine)
					{ 
						////if you are not over a covered column, no hit
                        if (!resizeUI.RaiseResizingColumns(GridRangeInfo.Col(colIndex), ref undefined, reason, point))
                        {
                            hitTestResult = GridHitTestContext.None;
                        }
					}
					else if (hitTestResult == GridHitTestContext.HorizontalLine)
					{ 
						////if you are not over a covered row, no hit
                        if (!resizeUI.RaiseResizingRows(GridRangeInfo.Row(rowIndex), ref undefined, reason, point))
                        {
                            hitTestResult = GridHitTestContext.None;
                        }
					}
				}

                if (hitTestResult == GridHitTestContext.HorizontalLine)
                {
                    if (resizeUI.IsSpanItemAtPoint(new Point(point.X, point.Y + _HitTestFrame))
                        || resizeUI.IsSpanItemAtPoint(new Point(point.X, point.Y - _HitTestFrame)))
                    {
                        hitTestResult = GridHitTestContext.Cell;
                    }
                }
			}
		}

		//// IMouseController implementation 

		/// <override/>
		public override string Name 
		{ 
			get
			{
				return "ResizeCells"; 
			}
		}

		/// <override/>
		public override Cursor Cursor 
		{ 
			get
			{
                if (hitTestInfo != null)
                {
                    return (hitTestInfo.hitTestResult == GridHitTestContext.Cell || this.isSpanHit) ? Cursors.Default : (hitTestInfo.hitTestResult == GridHitTestContext.HorizontalLine ? GridCursors.RowHeightCursor : Cursors.SizeAll); ////GridCursors.ColumnWidthCursor); 
                }

				return null;
			}
		}

		/// <override/>
		public override void MouseHoverEnter()
		{
		}

		/// <override/>
		public override void MouseHover(MouseEventArgs e)
		{
		}

		/// <override/>
		public override void MouseHoverLeave(EventArgs e)
		{
		}

		/// <override/>
		public override void MouseDown(MouseEventArgs e)
		{
			if (hitTestInfo != null && e.Button == MouseButtons.Left)
			{
				if (e.Clicks == 1 && !isSpanHit)
				{
					this.LeftMouseDown(hitTestInfo.point, e, hitTestInfo.hitTestResult, hitTestInfo.rowIndex, hitTestInfo.colIndex);
				}
                else if (e.Clicks == 2)
                {
                    this.LeftMouseDoubleClick(hitTestInfo.point, e, hitTestInfo.hitTestResult, hitTestInfo.rowIndex, hitTestInfo.colIndex);
                }
			}
		}

		/// <override/>
		public override void MouseMove(MouseEventArgs e)
		{
            if (Control.ModifierKeys != Keys.None)
            {
                grid.NotifyCancelMode();
            }
            else if (!isSpanHit)
            {
                Point point = new Point(e.X, e.Y);
                this.LeftMouseMove(point, e);
            }
		}

		/// <override/>
		public override void MouseUp(MouseEventArgs e)
		{
			Point point = new Point(e.X, e.Y);
			this.LeftMouseUp(point, e);
		}

		/// <override/>
		public override void CancelMode()
		{
			this.CancelResizing();
		}

        internal bool isSpanHit = false;

        /// <returns>True if there is an item at the given point.</returns>
        internal bool IsSpanItemAtPoint(Point point)
        {
            ItemHitType mouseDownHitType;
            IScheduleAppointment item = ((ScheduleGrid)this.Grid).GetItemAtPoint(point, out mouseDownHitType);
            return RecurrenceSupport.IsSpanItem(item);
        }

		/// <override/>
        /// <summary>
        /// Returns a value indicating the context at the given mouse position.
        /// </summary>
        /// <param name="e">A <see cref="MouseEventArgs"/>.</param>
        /// <param name="controller">Mouse controller.</param>
        /// <returns>A value indicating the context at the given mouse position.</returns>
		public override int HitTest(MouseEventArgs e, IMouseController controller)
		{
			// This HitTest code has higher priority than "SelectCells" and "DragCells".
			Point pt = new Point(e.X, e.Y);
			hitTestInfo = null;

            if (IsSpanItemAtPoint(pt))
            {
                isSpanHit = true;
            }
            else
            {
                isSpanHit = false;
            }

			if (ScheduleResizeCellsMouseController.inDrag ||
				(e.Button == MouseButtons.Left && 
				(controller == null || 
				controller.Name == "SelectCells" || 
				controller.Name == "DragCells")))
			{
				hitTestInfo = new ResizeCellsHitTestInfo(this, grid, pt, GridResizeCellsReason.HitTest);
				if (hitTestInfo.hitTestResult == GridHitTestContext.None)
				{
					ItemHitType mouseDownHitType;
                    if (((ScheduleGrid)this.Grid).GetItemAtPoint(new Point(e.X, e.Y), out mouseDownHitType) != null)
                    {
                        hitTestInfo.hitTestResult = GridHitTestContext.Cell;
                    }
				}
                else if (hitTestInfo.hitTestResult == GridHitTestContext.VerticalLine)
                {
                    hitTestInfo.hitTestResult = GridHitTestContext.Cell;
                }
			}
			
			return hitTestInfo != null ? hitTestInfo.hitTestResult : 0;
		}

		#region handle autoscrolling during drag

		/// <summary>
		/// Time interval in milliseconds of the between scrolls during a ScheduleAppointment
		/// drag when the item is at a visible border. 
		/// </summary>
		[ThreadStaticAttribute] public static int AutoScrollTimeInterval = 200;
		private Timer autoScrollTimer = null;
		private bool inAutoScroll = false;
		
        private void InitialAutoScrollOnDemand()
		{
			if (autoScrollTimer == null)
			{
				autoScrollTimer = new Timer();
				autoScrollTimer.Interval = AutoScrollTimeInterval;
				autoScrollTimer.Tick += new EventHandler(t_Tick);
				autoScrollTimer.Start();
			}
		}

		private void AutoTimerCleanUp()
		{
			if (autoScrollTimer != null)
			{
				autoScrollTimer.Stop();
				autoScrollTimer.Tick -= new EventHandler(t_Tick);
				autoScrollTimer.Dispose();
				autoScrollTimer = null;
			}
		}
		
        private void t_Tick(object sender, EventArgs e)
		{
			////finished
			if (Control.MouseButtons != MouseButtons.Left)
			{
				this.AutoTimerCleanUp();
				return;
			}

			if (!inAutoScroll)
			{
				inAutoScroll = true;
				CheckIfScrollNeeded(this.grid.PointToClient(Control.MousePosition));
				inAutoScroll = false;
			}
		}
		
        private void CheckIfScrollNeeded(Point pt)
		{
			////check if off top
			int row, col; 
			pt.Offset(0, -tOffSet);
			if (this.grid.PointToRowCol(pt, out row, out col)
				&& row <= this.grid.TopRowIndex 
				&& row > this.Grid.Model.Rows.FrozenCount + 1)
			{
				this.grid.TopRowIndex = row - 1;
				InitialAutoScrollOnDemand();
			}
			else
			{
				////check if off bottom
				pt.Offset(0, this.returnedHeight);
				if (this.grid.PointToRowCol(pt, out row, out col)
					&& row >= this.grid.ViewLayout.LastVisibleRow
					&& this.grid.ViewLayout.LastVisibleRow <= this.Grid.Model.RowCount)
				{
					this.grid.TopRowIndex += 1;
					InitialAutoScrollOnDemand();
					////handle last row not scrolling problem
                    if (this.grid.ViewLayout.LastVisibleRow == this.Grid.Model.RowCount - 1
                        && this.grid.ViewLayout.HasPartialVisibleRows)
                    {
                        this.grid.TopRowIndex += 1;
                    }
				}
			}
		}
		#endregion

		private int returnedHeight = 0;
		private int tOffSet = 0;
		private Point lastScrolledMousePosition = Point.Empty;
		
        private bool RaiseResizingColumns(GridRangeInfo columns, ref int width, GridResizeCellsReason reason, Point point)
		{
            if (isSpanHit)
            {
                return false;
            }

			GridResizingColumnsEventArgs e = new GridResizingColumnsEventArgs(columns, width, reason, ScheduleResizeCellsMouseController.sizeIndicatorBorder, ScheduleResizeCellsMouseController.boundsIndicatorBorder, point);
			if (reason == GridResizeCellsReason.MouseUp)
			{
				e.Width = topRow;
			}

			grid.RaiseResizingColumns(e);
			width = e.Width;

			////check if need to scroll
			if (reason == GridResizeCellsReason.MouseMove && !((ScheduleGrid)this.grid).scrollLocked)
			{
				CheckIfScrollNeeded(point);
			}
			else if (reason == GridResizeCellsReason.MouseDown)
			{
				lastScrolledMousePosition = Point.Empty;
				returnedHeight = width / 100;
				Point pt = this.grid.RangeInfoToRectangle(GridRangeInfo.Row(width % 100)).Location;
				
				int row, col; 
				this.grid.PointToRowCol(point, out row, out col);
				
				point = this.grid.RangeInfoToRectangle(GridRangeInfo.Row(row)).Location;
				tOffSet = point.Y - pt.Y;
                if (row == 1)
                {
                    tOffSet = 0;
                }
			}
			else if (reason == GridResizeCellsReason.MouseUp
				|| reason == GridResizeCellsReason.CancelMode)
			{
				AutoTimerCleanUp();
			}
		
			ScheduleResizeCellsMouseController.sizeIndicatorBorder = e.SizeIndicatorBorder;
			ScheduleResizeCellsMouseController.boundsIndicatorBorder = e.BoundsIndicatorBorder;
			return !e.Cancel;
		}
		
		bool RaiseResizingRows(GridRangeInfo rows, ref int height, GridResizeCellsReason reason, Point point)
		{
            if (isSpanHit)
            {
                return false;
            }

			GridResizingRowsEventArgs e = new GridResizingRowsEventArgs(rows, height, reason, ScheduleResizeCellsMouseController.sizeIndicatorBorder, ScheduleResizeCellsMouseController.boundsIndicatorBorder, point);
			grid.RaiseResizingRows(e);
			height = e.Height;
			ScheduleResizeCellsMouseController.sizeIndicatorBorder = e.SizeIndicatorBorder;
			ScheduleResizeCellsMouseController.boundsIndicatorBorder = e.BoundsIndicatorBorder;
			return !e.Cancel;
		}
	}

	internal class ScheduleDragLineWindow : TopLevelWindow
	{
		GridBorder border;

		public ScheduleDragLineWindow()
		{
			this.SetStyle(ControlStyles.AllPaintingInWmPaint|Syncfusion.Windows.Forms.WhidbeyCompatibleControlStyles.DoubleBuffer, true);
			this.SetStyle(ControlStyles.Selectable, false);
			this.TopLevel = true;  //// Must be TopLevel to allow transparency
			this.TransparencyKey = Color.White;  //// White will also look good on Win98 even though it is not transparent.
		}

		/// <override/>
		protected override CreateParams CreateParams
		{
			[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
			get
			{
				CreateParams cp = base.CreateParams;
				cp.ExStyle = cp.ExStyle | 0x80/*WS_EX_TOOLWINDOW*/;
				return cp;
			}
		}

        /// <override/>
		protected override void OnPaint(PaintEventArgs pe)
		{
            if (border == null)
            {
                return;
            }

			Graphics g = pe.Graphics;

            if (Height > Width || (border.Style == GridBorderStyle.Solid && ScheduleResizeCellsMouseController.navCalendarCenterPoint.IsEmpty))
            {
                using (Brush b = new SolidBrush(ScheduleResizeCellsMouseController.dragColor))
                {
                    g.FillRectangle(b, ClientRectangle);
                }
            }
            else if (border.Style == GridBorderStyle.Solid)
            {
                Rectangle r = ClientRectangle;
                r.Inflate(-1, -1);
                using (Pen p = new Pen(ScheduleResizeCellsMouseController.dragColor))
                {
                    g.DrawRectangle(p, r);
                }
            }
            else
            {
                GridBorderPaint.DrawRectangle(g, border, ClientRectangle, Color.White, GridBorderSide.Top);
            }
     	}

		public void ShowAt(Rectangle bounds, GridBorder border)
		{
			this.Visible = false;
			this.border = border;
			this.Location = new Point(10000, 10000);
			this.Size = bounds.Size;
			this.ShowWindowTopMost();
			this.Update();
			this.Bounds = bounds;
            this.ShowWindowTopMost();
		}

		public void Fade()
		{
			this.Location = new Point(10000, 10000);
			this.Hide();
		}

		public GridBorder Border
		{
			get
			{
				return border;
			}

			set
			{
				border = value;
			}
		}
	}
}
