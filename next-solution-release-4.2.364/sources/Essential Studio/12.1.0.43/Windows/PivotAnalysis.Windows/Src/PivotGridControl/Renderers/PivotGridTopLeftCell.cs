#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Any infringement will be prosecuted under
//  applicable laws. 
//
#endregion

namespace Syncfusion.Windows.Forms.PivotAnalysis
{
	using System;
	using System.Drawing;
	using System.Collections;
    using System.Collections.Generic;
	using System.ComponentModel;
	using System.Windows.Forms;
	using System.Data;
	using System.Runtime.Serialization;
    using System.Linq;
	using Syncfusion.Diagnostics;
	using Syncfusion.Windows.Forms;
	using Syncfusion.Windows.Forms.Grid;
    using Syncfusion.PivotAnalysis.Base;
    using Syncfusion.Drawing;

	/// <summary>
	/// Summary description for TopLeftCell.
	/// </summary>
	public class TopLeftCellModel : GridGenericControlCellModel
	{
		protected TopLeftCellModel(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

        public TopLeftCellModel(GridModel grid)
			: base(grid)
		{
			AllowFloating = false;
		}
	
		public override GridCellRendererBase CreateRenderer(GridControlBase control)
		{
            return new TopLeftCellRenderer(control, this);
		}

	}

	
	public class TopLeftCellRenderer: GridGenericControlCellRenderer
	{
		private RowGroupBar activeGrid;

        public TopLeftCellRenderer(GridControlBase grid, GridCellModelBase cellModel)
			: base(grid, cellModel)
		{
			this.SupportsFocusControl = true;
		}

		protected override void OnDraw(System.Drawing.Graphics g, System.Drawing.Rectangle clientRectangle, int rowIndex, int colIndex, Syncfusion.Windows.Forms.Grid.GridStyleInfo style)
		{
			if (this.ShouldDrawFocused(rowIndex, colIndex))
			{
				if (style.Control is RowGroupBar)
                    activeGrid = (RowGroupBar)style.Control;
					
				base.OnDraw(g, clientRectangle, rowIndex, colIndex, style);
			}
			else
			{
				// Draw a static grid
                if (style.Control is RowGroupBar)
				{
                    RowGroupBar grid = (RowGroupBar)style.Control;
					grid.DrawGrid(g, clientRectangle, true);
				}
			}
		}

        protected override void OnOutlineCurrentCell(Graphics g, Rectangle r)
        {
            // Suppress this action
        }
        protected override void OnClick(int rowIndex, int colIndex, MouseEventArgs e)
        {
            // Suppress this action
        }

		protected override bool ProcessKeyEventArgs(ref Message m)  
		{
			TraceUtil.TraceCurrentMethodInfo(m.ToString());

			// forward keyboard events to child grid that would otherwise 
			// be handled by parent grid (right arrow, page down etc.)
            if (activeGrid != null && activeGrid.Focused)
                return activeGrid.InitiateProcessKeyEventArgs(ref m);

			return base.ProcessKeyEventArgs(ref m);
		}
	}

    [ToolboxItem(false)]
    public class RowGroupBar : GridControl
    {
        private int maxRange = 0; int meanRange = 0; int colRange = 0, topLeftCellWidth;
        private PivotGridControlBase gridControl; FilterHelper filterHelper;
        private Syncfusion.Windows.Forms.PopupControlContainer popupControlContainer1;
        private DragDropHelper dragDropHelper;
        private int dropRow, dropCol; int index = 0;
        private int targetindex = 0;
        private int sourceIndex = 0;
        private Rectangle originBounds = new Rectangle();
        private bool started = false;
        private GridStyleInfo localStyle;
        private GroupDragHelper headerDragHelper = null;
        private Point hiddenPoint = new Point(10000, 10000);
        internal Cursor cursor = null;
        private bool wasDragging = false;
        private int checkRow, checkCol, locX, locY;
        private bool isMouseDown = false;
        private int defaultWidth = 5;
        GroupDragHelper redArrowIndicatorDragHelper = null;
        private FilterExpression fItem = null;
        private FilterDropDown filter = null;
        [ReadOnly(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public PivotGridControl GridControl
        {
            get
            {
                Control c = Parent;
                while (c != null)
                {
                    if (c is PivotGridControl)
                    {
                        return (PivotGridControl)c;
                    }

                    c = c.Parent;
                }

                return null;
            }
        }


        bool inWndProc = false;

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude]
        protected override void WndProc(ref Message msg)
        {
            bool oldInWndProc = inWndProc;
            inWndProc = true;
            base.WndProc(ref msg);
            inWndProc = oldInWndProc;
            ////            if (!inWndProc && !ParentDesignMode)
            ////            {
            ////                GridTableControl tableControl = Model.ActiveGridView as GridTableControl;
            ////                if (tableControl != null)
            ////                    tableControl.SynchronizeGridWithEngine();
            ////            }
        }
        [Syncfusion.Documentation.DocumentationExclude]
        public override void Invalidate()
        {
            Model.ResetVolatileData();
            base.Invalidate();
        }
        
        /// <summary>
        /// Constructor
        /// </summary>
        public RowGroupBar(PivotGridControlBase tableControl)
        {
            if (tableControl != null)
            {
                
                this.gridControl = tableControl;
                this.gridControl.RowGroupDropArea = this;
                WireGrid();
            }
        }

        /// <summary>
        /// Wire GroupBar in the Grid
        /// </summary>
        void WireGrid()
        {
            Model.ActiveGridView = this;
            RowCount = 1;
            ColCount = 100;

            this.VScrollBehavior = GridScrollbarMode.Disabled;
            this.HScrollBehavior = GridScrollbarMode.Disabled;
            this.ThemesEnabled = true;
            this.WantKeys = false;
            this.Model.Options.WrapCell = false;
            this.Model.Options.WrapCellBehavior = GridWrapCellBehavior.None;
            this.TableStyle.WrapText = false;
            this.Model.Properties.ColHeaders = false;
            this.Model.Properties.RowHeaders = false;
            this.Model.Options.GridVisualStyles = Syncfusion.Windows.Forms.GridVisualStyles.Office2007Blue;
            this.AllowSelection = GridSelectionFlags.None;
            this.ShowCurrentCellBorderBehavior = GridShowCurrentCellBorder.HideAlways;
            this.AllowDrop = false;
            this.ControllerOptions =  ~GridControllerOptions.OleDataSource;
            this.DefaultColWidth = 88;
            this.DefaultRowHeight = 25;
            this.AllowDragSelectedCols = false;
            this.AllowDragSelectedRows = false;
            CellModels.Add("PivotGridHeaderCell", new PivotGridHeaderCellModel(this.Model));
            CellModels.Add("PivotGridExpandCell", new PivotGridExpandCellCellModel(this.Model));
            CellModels.Add("ColumnHeaderCell", new PivotGridSortColumnHeaderCellModel(this.Model));

            this.popupControlContainer1 = new Syncfusion.Windows.Forms.PopupControlContainer();
            
            GridStyleInfo standard = BaseStylesMap["Standard"].StyleInfo;
            standard.Borders.All = GridBorder.Empty;
            standard.CellType = "Static";
            this.FillSplitterPane = false;
            
            this.gridControl.PivotCalculations.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(PivotCalculations_CollectionChanged);
            this.CellButtonClicked += new GridCellButtonClickedEventHandler(RowGroupBar_CellButtonClicked);
            this.CellClick += new GridCellClickEventHandler(RowGroupBar_CellClick);
            this.popupControlContainer1.CloseUp += new PopupClosedEventHandler(popupControlContainer1_CloseUp);
            filterHelper = new FilterHelper(this.gridControl);
            dragDropHelper = new DragDropHelper(this.GridControl);
        }

        #region Filter
        int filterIndex = 0;
        void RowGroupBar_CellButtonClicked(object sender, GridCellButtonClickedEventArgs e)
        {
            if (this.gridControl.AllowFiltering)
            {
                for (int i = 0; i < this.gridControl.PivotRows.Count; i++)
                {
                    if (this.Model[e.RowIndex, e.ColIndex].Text == this.gridControl.PivotRows[i].FieldMappingName)
                    {
                        filterIndex = i;
                        break;
                    }
                    else if (this.gridControl.PivotRows[i].FieldHeader != null)
                    {
                        if (this.Model[e.RowIndex, e.ColIndex].Text == this.gridControl.PivotRows[i].FieldHeader)
                        {
                            filterIndex = i;
                            break;
                        }
                    }
                }
                this.popupControlContainer1.RightToLeft = this.gridControl.IsRightToLeft() ? RightToLeft.Yes : RightToLeft.No;
                PropertyDescriptor descriptor = filterHelper.GetPropertyDescriptor(this.gridControl.PivotRows[filterIndex].FieldMappingName);
                fItem = this.gridControl.PivotFilters.Where(f => f.Name == this.gridControl.PivotRows[filterIndex].FieldMappingName).FirstOrDefault();
                FilterItemsCollection filteritem = new FilterItemsCollection();
                filter = new FilterDropDown(this.GridControl);
                Color clrBack, headerBorderTop, headerBorderLeft;
                this.Model.Options.GridVisualStylesDrawing.GetGroupDropAreaColors(out clrBack, out headerBorderTop, out headerBorderLeft);
                this.popupControlContainer1.BackColor = clrBack;
                this.popupControlContainer1.Size = filter.Size;
                if (popupControlContainer1.Controls.Count > 0)
                    popupControlContainer1.Controls.Clear();
                if (fItem != null && fItem.Tag != null)
                {
                    FilterItemsCollection filterList = fItem.Tag as FilterItemsCollection;
                    if (filterList.FilteredValues.Count > 0)
                    {
                        foreach (var item in filterList)
                        {
                            if (filterList.FilteredValues.Contains(item.Key))
                            {
                                item.IsSelected = true;
                            }
                            else
                                item.IsSelected = false;
                        }
                    }
                    filter.FilterList = filterList;
                }
                else
                {
                    FilterItemsCollection ftempCollection = new FilterItemsCollection();
                    ftempCollection = filterHelper.RequireFilterItem(descriptor);
                    FilterItemsCollection fTemp = filterHelper.GetFilterItem(descriptor);

                    foreach (var item in fTemp)
                    {
                        ftempCollection.FilteredValues.Add(item.Key.ToString());
                    }

                    if (ftempCollection.FilteredValues.Count > 0)
                    {
                        foreach (var temp in ftempCollection)
                        {
                            if (ftempCollection.FilteredValues.Contains(temp.Key))
                                temp.IsSelected = true;
                            else
                                temp.IsSelected = false;
                        }
                        filter.FilterList = ftempCollection;
                    }
                    else
                        filter.FilterList = filterHelper.GetFilterItem(descriptor);
                }

                popupControlContainer1.Controls.Add(filter);

                Point location = this.PointToScreen(new Point(locX, locY));
                if (popupControlContainer1.IsShowing())
                    popupControlContainer1.HidePopup();
                else
                    popupControlContainer1.ShowPopup(location);
                this.popupControlContainer1.Focus();

            }
        }

        void popupControlContainer1_CloseUp(object sender, PopupClosedEventArgs e)
        {
            if (e.PopupCloseType == PopupCloseType.Done)
            {
                if (filter != null && filter.FilterList.AllFilterItem.SelectedState == true)
                {
                    if (fItem != null && this.gridControl.Filters.Contains(fItem))
                        this.gridControl.Filters.Remove(fItem);
                }
            }
        }

        #endregion
        bool extendedClick = false;
        #region sort
        void RowGroupBar_CellClick(object sender, GridCellClickEventArgs e)
        {
            for (int i = 0; i < this.gridControl.PivotRows.Count; i++)
            {
                if (this.Model[e.RowIndex, e.ColIndex].Text == this.gridControl.PivotRows[i].FieldMappingName)
                {
                    index = i;
                    break;
                }
            }
            if (e.ColIndex <= colRange && e.ColIndex > -1)
            {
                if (this.gridControl.AllowSorting && !isSortCursorOnMove)
                {
                    Sort(e.RowIndex, e.ColIndex, index);
                    if (asc)
                        this.Model[e.RowIndex, e.ColIndex].Tag = ListSortDirection.Ascending;
                    else if (!asc)
                        this.Model[e.RowIndex, e.ColIndex].Tag = ListSortDirection.Descending;
                }
            }
            if (e.ColIndex > colRange || e.RowIndex != 1)
                extendedClick = true;
            isSortCursorOnMove = false;
        }
        bool asc = false;
        bool isSortCursorOnMove = false;
        private void Sort(int rowIndex, int colIndex, int index)
        {

            if (this.Model.ColWidths[colIndex] != 5)
            {
                PivotItem pivotItems = this.GridControl.PivotRows[index];
                if (pivotItems.Comparer == null || pivotItems.Comparer.GetType().ToString() == "Syncfusion.PivotAnalysis.Base.IntComparer"
                    || pivotItems.Comparer.GetType().ToString() == "Syncfusion.PivotAnalysis.Base.DoubleComparer"
                    || pivotItems.Comparer.GetType().ToString() == "Syncfusion.PivotAnalysis.Base.DecimalComparer"
                    || pivotItems.Comparer.GetType().ToString() == "Syncfusion.PivotAnalysis.Base.DateComparer")
                {
                    pivotItems.Comparer = new ReverseOrderComparer();
                    this.gridControl.UpdateGridLayout = true;
                    this.Refresh(true);
                    this.gridControl.PivotEngine.CoveredRanges.Clear();
                    this.gridControl.PivotEngine.Populate();
                    this.gridControl.InternalRefresh();
                    this.gridControl.Refresh();
                    asc = true;
                }
                else if (pivotItems.Comparer is ReverseOrderComparer)
                {
                    pivotItems.Comparer = null;
                    this.gridControl.UpdateGridLayout = true;
                    this.Refresh(true);
                    this.gridControl.PivotEngine.CoveredRanges.Clear();
                    this.gridControl.PivotEngine.Populate();
                    this.gridControl.InternalRefresh();
                    this.gridControl.Refresh();
                    asc = false;

                }
            }
        }
        #endregion

        #region DragHeader

        /// <summary>
        /// Creates a header bitmap which used in dragwindow 
        /// </summary>
        /// <param name="grid">groupbar</param>
        /// <param name="rowIndex">rowindex</param>
        /// <param name="colIndex">colindex</param>
        /// <returns></returns>
        private Bitmap CreateHeaderBitmap(GridControlBase grid, int rowIndex, int colIndex)
        {
            Graphics g = null;
            Size size = new Size(this.GetColWidth(colIndex), this.GetRowHeight(rowIndex));
            Rectangle bounds = new Rectangle(Point.Empty, size);
            GridStyleInfo style = this.Model[rowIndex, colIndex];
            GridCellRendererBase headerCellRenderer = grid.CellRenderers["Header"];
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

            if (this.Model.Options.GridVisualStyles == Forms.GridVisualStyles.Metro)
                bm = this.gridControl.ProcessBitmap(bm);
            return bm;
        }

        /// <summary>
        /// Opens the created drag window and initiates the painting in drag operation
        /// </summary>
        internal void OpenDragHeader()
        {
            Bitmap bm;

            bm = CreateHeaderBitmap(this, checkRow, checkCol);

            headerDragHelper = new GroupDragHelper();
            cursor = Cursors.Default;
            headerDragHelper.StartDrag(bm, Control.MousePosition, DragDropEffects.Move);
        }

        /// <summary>
        /// Update the DragWindow to the current mouse position
        /// </summary>
        internal void UpdateDragHeader()
        {
            Point mousePos = Control.MousePosition;

            this.headerDragHelper.DragWindow.WindowCursor = Cursor;
            Point pt = GetDragWindowLocation(originBounds.Location);
            pt = new Point(pt.X, pt.Y);
            this.headerDragHelper.DoDrag(pt, DragDropEffects.Move);

            wasDragging = true;
        }

        /// <summary>
        /// Close the drag window which usually done in mouse up
        /// </summary>
        internal void CloseDragHeader()
        {
            if (headerDragHelper != null)
            {
                headerDragHelper.EndDrag();
                headerDragHelper.Dispose();
                headerDragHelper = null;
            }
        }

        /// <summary>
        /// gets the location of the drag window
        /// </summary>
        internal Point GetDragWindowLocation(Point srcLocation)
        {
            Point pt = Control.MousePosition;
            return new Point(pt.X, pt.Y);

        }

        /// <summary>
        /// returns the corresponding field index of the column
        /// </summary>
        /// <param name="colIndex"></param>
        /// <returns></returns>
        public int ColIndexToField(int colIndex)
        {
            int num = Math.Min(colRange, colIndex);
            return num;
        }

        #endregion

        #region RedArrowIndicator

        /// <summary>
        /// Creates the arrow indicator which used for indicating the dropping location
        /// </summary>
        /// <returns>bitmap</returns>
        private Bitmap CreateRedArrowIndicatorBitmap()
        {
            Bitmap bm = null;
            Graphics g = null;
            int rowIndex = checkRow;
            Bitmap downBitmap = FilterBitmaps.RedDownBitmap;
            Bitmap upBitmap = FilterBitmaps.RedUpBitmap;

            Size size = new Size(downBitmap.Width, this.GetRowHeight(rowIndex) + (downBitmap.Height * 2) - 1);
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

        /// <summary>
        /// opens the created arrow bitmap and intiates the painting in drag operation
        /// </summary>
        internal void OpenRedArrowIndicator()
        {
            Bitmap bm = CreateRedArrowIndicatorBitmap();
            redArrowIndicatorDragHelper = new GroupDragHelper();
            redArrowIndicatorDragHelper.StartDrag(bm, Control.MousePosition, DragDropEffects.Move);
        }

        /// <summary>
        /// Updates the arrow bitmap to the current location of drag window
        /// </summary>
        internal void UpdateRedArrowIndicatorBitmap(Point location)
        {
            Point pt = Control.MousePosition;
            pt = this.GridPointToClient(pt);
            GridControlBase gridWindow = this.GetGridWindow();
            Bitmap downBitmap = FilterBitmaps.RedDownBitmap;
            Bitmap upBitmap = FilterBitmaps.RedUpBitmap;

            GridRangeInfo p2 = this.PointToRangeInfo(location);

            Rectangle topLeftCell = new Rectangle(this.gridControl.RowGroupDropArea.Bounds.X, this.gridControl.RowGroupDropArea.Bounds.Y, this.gridControl.RowGroupDropArea.Bounds.Width, this.gridControl.RowGroupDropArea.Bounds.Height);

            Rectangle groupBarBounds = new Rectangle(this.gridControl.Bounds.Location.X, this.gridControl.Bounds.Location.Y, this.gridControl.GroupDropArea.Width, this.gridControl.GroupDropArea.Height);

            #region [implement here]
            if (topLeftCell.Contains(location))
            {
                p2 = this.PointToRangeInfo(location);
                if (p2.Bottom == 1 && this.Model.ColWidths[p2.Left] != 5 && p2.Left <= (this.gridControl.PivotRows.Count * 2))
                {
                    Rectangle cellRectangle = this.RangeInfoToRectangle(p2);
                    Point arrowPoint = new Point(cellRectangle.X, cellRectangle.Y);
                    arrowPoint = this.PointToScreen(arrowPoint);
                    arrowPoint = new Point(arrowPoint.X - 1, arrowPoint.Y - ((downBitmap.Height)));
                    redArrowIndicatorDragHelper.DoDrag(arrowPoint, DragDropEffects.Copy);
                    redArrowIndicatorDragHelper.DragWindow.ShowWindowTopMost();
                }
                else
                {
                    redArrowIndicatorDragHelper.DoDrag(hiddenPoint, DragDropEffects.Copy);
                }
            }
            else if (this.GridControl.RectangleToScreen(this.gridControl.GroupDropArea.Bounds).Contains(PointToScreen(location)))
            {
                location.Y = location.Y + groupBarBounds.Height;
                p2 = this.gridControl.GroupDropArea.PointToRangeInfo(location);
                meanRange = (2 * this.gridControl.PivotCalculations.Count) + 1;
                colRange = meanRange + (this.gridControl.PivotColumns.Count * 2);
                if (p2.Bottom == 2 && p2.Left != meanRange && p2.Left <= colRange && p2.Left > 1 && this.Model.ColWidths[p2.Left] != 5 && p2.Left % 2 == 0)
                {
                    Rectangle cellRectangle = this.gridControl.GroupDropArea.RangeInfoToRectangle(p2);
                    Point arrowPoint = new Point(cellRectangle.X, cellRectangle.Y);
                    arrowPoint = this.gridControl.GroupDropArea.PointToScreen(arrowPoint);
                    arrowPoint = new Point(arrowPoint.X - 1, arrowPoint.Y - ((downBitmap.Height)));
                    redArrowIndicatorDragHelper.DoDrag(arrowPoint, DragDropEffects.Copy);
                    redArrowIndicatorDragHelper.DragWindow.ShowWindowTopMost();
                }

                else if (p2.Bottom == 2 && p2.Left != meanRange && p2.Left >= colRange && p2.Left > 1 && this.Model.ColWidths[p2.Left] != 5)
                {
                    GridRangeInfo lastColRange = GridRangeInfo.Cell(p2.Bottom, colRange - 1);
                    Rectangle cellRectangle = this.gridControl.GroupDropArea.RangeInfoToRectangle(lastColRange);
                    Point arrowPoint = new Point(cellRectangle.X + cellRectangle.Width, cellRectangle.Y);
                    arrowPoint = this.gridControl.GroupDropArea.PointToScreen(arrowPoint);
                    arrowPoint = new Point(arrowPoint.X - 1, arrowPoint.Y - ((downBitmap.Height)));
                    redArrowIndicatorDragHelper.DoDrag(arrowPoint, DragDropEffects.Copy);
                    redArrowIndicatorDragHelper.DragWindow.ShowWindowTopMost();
                }
                else if (p2.Bottom == 2 && p2.Left == meanRange)
                {
                    GridRangeInfo lastColRange = GridRangeInfo.Cell(p2.Bottom, meanRange - 1);
                    Rectangle cellRectangle = this.gridControl.GroupDropArea.RangeInfoToRectangle(lastColRange);
                    Point arrowPoint = new Point(cellRectangle.X + cellRectangle.Width, cellRectangle.Y);
                    arrowPoint = this.gridControl.GroupDropArea.PointToScreen(arrowPoint);
                    arrowPoint = new Point(arrowPoint.X - 1, arrowPoint.Y - ((downBitmap.Height)));
                    redArrowIndicatorDragHelper.DoDrag(arrowPoint, DragDropEffects.Copy);
                    redArrowIndicatorDragHelper.DragWindow.ShowWindowTopMost();
                }
                else
                {
                    redArrowIndicatorDragHelper.DoDrag(hiddenPoint, DragDropEffects.Copy);
                }
            }
            else
            {
                redArrowIndicatorDragHelper.DoDrag(hiddenPoint, DragDropEffects.Copy);
            }
            #endregion


        }

        /// <summary>
        /// To draw the arrow indicators , invoked at CreateRedArrowIndicator()
        /// </summary>
        /// <param name="g"></param>
        private void DrawRedArrowIndicator(Graphics g)
        {
            int rowIndex = checkRow;
            Bitmap downBitmap = FilterBitmaps.RedDownBitmap;
            Bitmap upBitmap = FilterBitmaps.RedUpBitmap;

            Size size = new Size(downBitmap.Width, this.GetRowHeight(rowIndex) + (downBitmap.Height * 2) - 1);
            Rectangle bounds = new Rectangle(Point.Empty, size);

            Color backColor = Color.Red;

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

        /// <summary>
        /// Closes the indicator which usually done at mouse up
        /// </summary>
        internal void CloseRedArrowIndicator()
        {
            if (redArrowIndicatorDragHelper != null)
            {
                redArrowIndicatorDragHelper.EndDrag();
                redArrowIndicatorDragHelper.Dispose();
                redArrowIndicatorDragHelper = null;
            }
        }
       
        #endregion

        #region To Resize the items in RowGroupbar [Alternate for events to invoke this whenever it is needed]
        
        void PivotCalculations_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ApplySize();
            ApplyItems();
        }

        /// <summary>
        /// Helper function which used to apply the size of the cells in the RowGroupBar. An alter to avoid events 
        /// </summary>
        internal void ApplySize()
        {
            int topLeftCellWidth;
            if (this.gridControl.ShowCalculationsAsColumns)
            {
                topLeftCellWidth = gridControl.PivotRows.Count * 88;
                colRange = 2 * this.gridControl.PivotRows.Count;
            }
            else
            {
                if (this.gridControl.PivotCalculations.Count > 1)
                {
                    topLeftCellWidth = (gridControl.PivotRows.Count + 1) * 88;
                    colRange = (2 * this.gridControl.PivotRows.Count) + 1;
                }
                else
                {
                    topLeftCellWidth = gridControl.PivotRows.Count * 88;
                    colRange = 2 * this.gridControl.PivotRows.Count;
                }
            }

            meanRange = colRange + 1;
            for (int i = 1; i <= meanRange; i++)
            {
                if (i % 2 != 0)
                {
                    this.Model.ColWidths[i] = 5;
                }
                if (this.gridControl.PivotRows.Count > 1)
                {
                    if (i == meanRange)
                    {
                        this.Model.ColWidths[i] = defaultWidth;
                    }
                }
                else
                {
                }
            }
        }

        /// <summary>
        /// Helper function which used to apply the value to the cells in the GroupBar. An alter to avoid events 
        /// </summary>
        internal void ApplyItems()
        {
            maxRange = (this.gridControl.PivotCalculations.Count + this.gridControl.PivotCalculations.Count);
            meanRange = maxRange + 1;

            if (this.gridControl.ShowCalculationsAsColumns)
            {
                topLeftCellWidth = gridControl.PivotRows.Count * 88;
                colRange = 2 * this.gridControl.PivotRows.Count;
            }
            else
            {
                if (this.gridControl.PivotCalculations.Count > 1)
                {
                    topLeftCellWidth = (gridControl.PivotRows.Count + 1) * 88;
                    colRange = (2 * this.gridControl.PivotRows.Count) + 1;
                }
                else
                {
                    topLeftCellWidth = gridControl.PivotRows.Count * 88;
                    colRange = 2 * this.gridControl.PivotRows.Count;
                }
            }

            for (int j = 1; j <= colRange; j++)
            {
                if (j <= colRange)
                {
                    if (j == 7)
                    {
                    }
                    if (this.ColWidths[j] != 5 && (j % 2 == 0))
                    {
                        this[1, j].CellType = "ColumnHeaderCell";
                        this[1, j].VerticalAlignment = GridVerticalAlignment.Middle;
                        int index = (j) / 2;
                        if (!string.IsNullOrEmpty(this.gridControl.PivotRows[index - 1].FieldHeader))
                            this[1, j].Text = this.gridControl.PivotRows[index - 1].FieldHeader;
                        else
                        {
                            this[1, j].Text = this.gridControl.PivotRows[index - 1].FieldMappingName;
                            this.gridControl.PivotRows[index - 1].FieldHeader = this.gridControl.PivotRows[index - 1].FieldMappingName;
                        }
                    }
                }
            }

            for (int i = colRange + 1; i <= this.ColCount; i++)
            {
                this[1, i].CellType = "Static";
                this[1, i].Text = "";
            }

        }

        #endregion

        #region Overrides
      
        #region For DragDrop
        protected override void OnMouseMove(MouseEventArgs e)
        {
            this.PointToRowCol(new Point(e.X, e.Y), out dropRow, out dropCol);
            localStyle = this[checkRow, checkCol];
            if (this.isMouseDown && checkCol != -1 && checkRow != -1 && checkCol % 2 == 0)
            {
                if (!started)
                {
                    Point cursorPos = new Point(e.X, e.Y);
                    int rowIndex, colIndex = 0;
                    this.PointToRowCol(cursorPos, out rowIndex, out colIndex);
                    if (rowIndex != checkRow || colIndex != checkCol)
                    {
                        if ((this.gridControl.PivotRows.Count > 1) || checkCol > 2)
                        {
                            this.OpenDragHeader();
                            this.OpenRedArrowIndicator();
                        }
                        started = true;
                    }
                }
                if (started)
                {
                    this.originBounds = this.GetGridWindow().GridRectangleToScreen(this.RangeInfoToRectangle(GridRangeInfo.Cell(dropRow, dropCol)));
                    if ((this.gridControl.PivotRows.Count > 1) || checkCol > 2)
                    {
                        UpdateDragHeader();
                        UpdateRedArrowIndicatorBitmap(e.Location);
                    }
                }
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            this.PointToRowCol(e.Location, out checkRow, out checkCol);
            if (checkCol <= colRange)
            {
                isMouseDown = false;
                started = false;
                CloseDragHeader();
                CloseRedArrowIndicator();
                dragDropHelper = new DragDropHelper(this.GridControl);
                Rectangle topLeftCell = new Rectangle(this.gridControl.RowGroupDropArea.Location.X, this.gridControl.RowGroupDropArea.Location.Y, this.gridControl.RowGroupDropArea.Size.Width, this.gridControl.RowGroupDropArea.Size.Height);
                if (wasDragging)
                {
                    isSortCursorOnMove = true;
                    targetindex = checkCol;
                    wasDragging = false;
                    GridStyleInfo dragCellStyle = this[1, sourceIndex];
                    Rectangle filterAreaBounds = this.gridControl.FilterArea.Bounds;
                    Rectangle groupAreaBounds = this.gridControl.GroupDropArea.Bounds;
                    if (IsRightToLeft())
                    {
                        if (this.GridControl.ShowPivotTableFieldList)
                        {
                            filterAreaBounds.Width += this.GridControl.PivotSchemaDesigner.Width;
                            groupAreaBounds.Width += this.GridControl.PivotSchemaDesigner.Width;
                        }
                    }
                    if (topLeftCell.Contains(e.Location))
                        dragDropHelper.DropinPivotRows(dragCellStyle, sourceIndex, targetindex, null);
                    else if (this.GridControl.RectangleToScreen(filterAreaBounds).Contains(PointToScreen(e.Location)))
                    {
                        dragDropHelper.DropinPivotFilters(dragCellStyle, -1, this.gridControl.RowGroupDropArea);
                    }
                    else if (this.GridControl.RectangleToScreen(groupAreaBounds).Contains(PointToScreen(e.Location)))
                    {
                        GridRangeInfo rangeinGroupArea = this.gridControl.GroupDropArea.PointToRangeInfo(e.Location);
                        int groupBarMeanRange = (2 * this.gridControl.PivotCalculations.Count) + 1;
                        int maxRange = groupBarMeanRange + (this.gridControl.PivotColumns.Count * 2);
                        if (rangeinGroupArea.Left <= groupBarMeanRange && this.gridControl.GroupDropArea.Model.ColWidths[rangeinGroupArea.Left] != 5)
                        {
                            dragDropHelper.DropinPivotCalculations(dragCellStyle, -1, rangeinGroupArea.Left, this.gridControl.RowGroupDropArea);
                        }
                        else if (rangeinGroupArea.Left > groupBarMeanRange && this.gridControl.GroupDropArea.Model.ColWidths[rangeinGroupArea.Left] != 5)
                        {
                            dragDropHelper.DropinPivotColumns(dragCellStyle, -1, rangeinGroupArea.Left, this.gridControl.RowGroupDropArea);
                        }
                    }
                    else
                    {
                        {
                            if (this.GridControl.pivotSchemaDesigner != null)
                            {
                                // Well its dropped in schema
                                switch (dragDropHelper.GetSchemaContainer(e.Location))
                                {
                                    case "PivotRows":
                                        dragDropHelper.DropinPivotRows(localStyle, this.gridControl.RowGroupDropArea);
                                        break;
                                    case "PivotColumns":
                                        dragDropHelper.DropinPivotColumns(localStyle, this.gridControl.RowGroupDropArea);
                                        break;
                                    case "PivotCalculations":
                                        dragDropHelper.DropinPivotCalculations(localStyle, this.gridControl.RowGroupDropArea);
                                        break;
                                    case "TableFieldList":
                                        dragDropHelper.DropInTableFieldList(localStyle, sourceIndex, this.gridControl.RowGroupDropArea);
                                        break;
                                    case "PivotFilters":
                                        break;
                                }
                            }
                        }
                    }
                    if (this.GridControl.pivotSchemaDesigner != null)
                    {
                        this.GridControl.pivotSchemaDesigner.ResetItemCollectionLists(true);
                        this.GridControl.pivotSchemaDesigner.PopulatePivotItems();
                    }
                }
                base.OnMouseUp(e);
            }
        }
       
        protected override void OnMouseDown(MouseEventArgs e)
        {
            isMouseDown = true;
            locX = e.X;
            locY = e.Y;
            this.PointToRowCol(e.Location, out checkRow, out checkCol);
            if (checkCol > this.gridControl.PivotRows.Count * 2)
                isMouseDown = false;
            sourceIndex = checkCol;
            base.OnMouseDown(e);

        }
        #endregion

        #region Usual overrides
        protected override void OnQueryRowHeight(GridRowColSizeEventArgs e)
        {
            if (e.Index == 1 && !extendedClick)
            {
                e.Size = 28;
                e.Handled = true;
            }
            extendedClick = false;

            base.OnQueryRowHeight(e);
        }
        protected override void OnDrawCellDisplayText(GridDrawCellDisplayTextEventArgs e)
        {
            if (this.Model.Options.GridVisualStyles == Forms.GridVisualStyles.Metro)
                e.Style.Font.Bold = true;
            else
            {
                if (this.GridVisualStyles == Forms.GridVisualStyles.Office2010Black)
                    e.Style.TextColor = Color.White;
            }

            base.OnDrawCellDisplayText(e);
        }
        protected override void OnQueryCellInfo(GridQueryCellInfoEventArgs e)
        {
            #region color settings
            Color clrBack = Color.Empty;
            Color headerBorderTop = Color.Empty;
            Color headerBorderLeft = Color.Empty;

            GridStyleInfo standard = BaseStylesMap["Standard"].StyleInfo;

            if (this.Model.Options.GridVisualStylesDrawing.GetGroupDropAreaColors(out clrBack, out headerBorderTop, out headerBorderLeft))
            {
                if (this.Model.Options.GridVisualStyles == Forms.GridVisualStyles.Metro)
                {
                    standard.BackColor = Color.FromArgb(246, 247, 247);
                    this.Properties.BackgroundColor = Color.FromArgb(246, 247, 247);
                    this.BorderStyle = System.Windows.Forms.BorderStyle.None;
                }
                else
                {
                    standard.BackColor = clrBack;
                    this.Properties.BackgroundColor = clrBack;
                }
            }

            #endregion
            e.Style.Borders.All = GridBorder.Empty;

            base.OnQueryCellInfo(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            TraceUtil.TraceCurrentMethodInfo(e.KeyCode);
            base.OnKeyDown(e);
        }

        internal bool InitiateProcessKeyEventArgs(ref Message m)
        {
            return base.ProcessKeyEventArgs(ref m);
        }
        #endregion

        #endregion

    }
}
