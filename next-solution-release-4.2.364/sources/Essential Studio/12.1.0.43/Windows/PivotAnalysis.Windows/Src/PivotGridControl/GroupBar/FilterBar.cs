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
using Syncfusion.Windows.Forms.Grid;
using Syncfusion.Diagnostics;
using System.ComponentModel;
using System.Windows.Forms;
using Syncfusion.PivotAnalysis.Base;
using Syncfusion.Windows.Forms;
using System.Drawing;
using System.Data;
using System.Collections;
using Syncfusion.Drawing;

namespace Syncfusion.Windows.Forms.PivotAnalysis
{
    [ToolboxItem(false)]
    public class FilterBar : GridControl
    {
        private Syncfusion.Windows.Forms.PopupControlContainer popupControlContainer1; FilterHelper filterHelper;
        private Syncfusion.Windows.Forms.PivotAnalysis.PivotGridControlBase gridControl;
        private DragDropHelper helper;
        private int meanRange = 0, colRange = 0;
        private int locX, locY;
        private int sourceIndex = 0; int dropRow, dropCol;
        private Rectangle originBounds = new Rectangle();
        private GridStyleInfo localStyle;
        private int checkRow, checkCol;
        private bool isMouseDown = false;
        private bool started = false;
        private GroupDragHelper redArrowIndicatorDragHelper = null;
        private GroupDragHelper headerDragHelper = null;
        private Point hiddenPoint = new Point(10000, 10000);
        internal Cursor cursor = null;
        private FilterExpression fItem = null; 
        private FilterDropDown filter = null;
        private bool wasDragging = false;

        /// <summary>
        /// Constructor
        /// </summary>
        public FilterBar()
        {

        }

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


        [Syncfusion.Documentation.DocumentationExclude]
        public override void Invalidate()
        {
            Model.ResetVolatileData();
            base.Invalidate();
        }
        public FilterBar(PivotGridControlBase tableControl)
        {
            if (tableControl != null)
            {
                this.gridControl = tableControl;
                this.gridControl.FilterArea = this;
                WireGrid();
            }


        }

        /// <summary>
        /// Wire GroupBar in the Grid
        /// </summary>
        void WireGrid()
        {
            SetStyle(ControlStyles.Selectable, false);
            Model.ActiveGridView = this;
            RowCount = 2;
            ColCount = 20;
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
            this.ShowCurrentCellBorderBehavior = GridShowCurrentCellBorder.HideAlways;
            this.DefaultRowHeight = 24;
            this.DefaultColWidth = 80;
            this.ControllerOptions = ~GridControllerOptions.OleDataSource;
            this.AllowDragSelectedCols = false;
            this.AllowDragSelectedRows = false;
            this.AllowSelection = GridSelectionFlags.None;
            this.QueryCellInfo += FilterBar_QueryCellInfo;
            this.gridControl.Filters.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Filters_CollectionChanged);

            CellModels.Add("ColumnHeaderCell", new PivotGridSortColumnHeaderCellModel(this.Model));
            CellModels.Add("PivotGridHeaderCell", new PivotGridHeaderCellModel(this.Model));
            CellModels.Add("PivotGridExpandCell", new PivotGridExpandCellCellModel(this.Model));

            GridStyleInfo standard = BaseStylesMap["Standard"].StyleInfo;
            standard.Borders.All = GridBorder.Empty;
            standard.CellType = "Static";
            standard.Themed = true;
            this[2, 1].Text = PivotAnalysis.SR.GetString(PivotAnalysis.SR.DropPivotFieldsheretoFilterBy);
            this.Model.RowHeights[1] = 5;

            this.Model.CoveredRanges.Add(GridRangeInfo.Cells(1, 1, 1, this.ColCount));
            this.Model.CoveredRanges.Add(GridRangeInfo.Cells(2, 1, 2, this.ColCount));

            this.popupControlContainer1 = new Syncfusion.Windows.Forms.PopupControlContainer();

            filterHelper = new FilterHelper(this.gridControl);
            helper = new DragDropHelper(this.GridControl);
            this.CellButtonClicked += new GridCellButtonClickedEventHandler(FilterBar_CellButtonClicked);
        }

        void FilterBar_QueryCellInfo(object sender, GridQueryCellInfoEventArgs e)
        {
            if (this.Model.Options.GridVisualStyles == Forms.GridVisualStyles.Metro)
            {
                e.Style.BackColor = Color.FromArgb(35, 130, 195);
                e.Style.Font.Bold = true;
                if (e.Style.Text == PivotAnalysis.SR.GetString(PivotAnalysis.SR.DropPivotFieldsheretoFilterBy))
                    e.Style.TextColor = Color.White;
                e.Style.Font.Size = 9f;
            }
            else
            {
                if (this.GridVisualStyles == Forms.GridVisualStyles.Office2010Black)
                    e.Style.TextColor = Color.White;
            }
        }

        #region Filter

        void FilterBar_CellButtonClicked(object sender, GridCellButtonClickedEventArgs e)
        {
            if (e.ButtonIndex == 0 && this.gridControl.AllowFiltering)
            {
                this.popupControlContainer1.RightToLeft = this.gridControl.IsRightToLeft() ? RightToLeft.Yes : RightToLeft.No;
                PropertyDescriptor descriptor = filterHelper.GetPropertyDescriptor(this[e.RowIndex, e.ColIndex].Text);
                fItem = this.gridControl.PivotFilters.Where(f => f.DimensionName == this[e.RowIndex, e.ColIndex].Text).FirstOrDefault();
                FilterItemsCollection filteritem = new FilterItemsCollection();
                filter = new FilterDropDown(this.GridControl);
                Color clrBack, headerBorderTop, headerBorderLeft;
                this.Model.Options.GridVisualStylesDrawing.GetGroupDropAreaColors(out clrBack, out headerBorderTop, out headerBorderLeft);
                this.popupControlContainer1.BackColor = clrBack;
                filter.BackColor = clrBack;
                int count = this.popupControlContainer1.Controls.Count;
                if (this.popupControlContainer1.Controls.Count > 0)
                    this.popupControlContainer1.Controls.Clear();
                this.popupControlContainer1.Size = filter.Size;
                filter.Size = this.popupControlContainer1.Size;
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
                    popupControlContainer1.HidePopup(PopupCloseType.Done);
                else
                    popupControlContainer1.ShowPopup(location);

            }
            else if (e.ButtonIndex == 1)
            {
                helper = new DragDropHelper(this.GridControl);
                helper.Remove("PivotFilters", this[e.RowIndex, e.ColIndex]);
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

        #endregion

        #region ArrowIndicator

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
            Rectangle r1 = RectangleToScreen(this.gridControl.GroupDropArea.Bounds);
            int height;
            GridRangeInfo p2 = this.PointToRangeInfo(location);

            Rectangle topLeftCell = new Rectangle(this.gridControl.Bounds.Location.X, this.gridControl.Bounds.Location.Y, this.gridControl.RowGroupDropArea.Width, this.gridControl.RowGroupDropArea.Height);
            Rectangle topLeftCell2 = new Rectangle(this.gridControl.RowGroupDropArea.Location.X, this.gridControl.RowGroupDropArea.Location.Y, this.gridControl.RowGroupDropArea.Size.Width, this.gridControl.RowGroupDropArea.Size.Height);
            Rectangle topLeftCell3 = new Rectangle(this.gridControl.Bounds.Location.X, this.gridControl.Bounds.Location.Y, this.gridControl.RowGroupDropArea.Width, this.gridControl.RowGroupDropArea.Height);
            meanRange = (2 * this.gridControl.PivotCalculations.Count) + 1;
            if (this.GridControl.RectangleToScreen(topLeftCell3).Contains(PointToScreen(location)))
            {
                p2 = this.gridControl.RowGroupDropArea.PointToRangeInfo(location);
                if (p2.Bottom == 1 && p2.Left <= (this.gridControl.PivotRows.Count * 2))
                {
                    Rectangle cellRectangle; Point arrowPoint = Point.Empty;
                    cellRectangle = this.gridControl.RowGroupDropArea.RangeInfoToRectangle(p2);
                    bool done = false;
                    if (this.gridControl.RowGroupDropArea.Model.ColWidths[p2.Left] != 5 && p2.Left != (this.gridControl.PivotRows.Count * 2))
                    {
                        arrowPoint = new Point(cellRectangle.X, cellRectangle.Y);
                        done = true;
                    }
                    else if (this.gridControl.RowGroupDropArea.Model.ColWidths[p2.Left] != 5 && p2.Left == (this.gridControl.PivotRows.Count * 2))
                    {
                        arrowPoint = new Point(cellRectangle.X + (cellRectangle.Width - 5), cellRectangle.Y);
                        done = true;
                    }
                    else if (p2.Left == ((this.gridControl.PivotRows.Count * 2) - 1) && this.gridControl.RowGroupDropArea.Model.ColWidths[p2.Left] == 5)
                    {
                        GridRangeInfo testRange = GridRangeInfo.Cell(p2.Bottom, p2.Left + 1);
                        cellRectangle = this.gridControl.RowGroupDropArea.RangeInfoToRectangle(testRange);
                        arrowPoint = new Point(cellRectangle.X, cellRectangle.Y);
                        done = true;
                    }

                    if (done)
                    {
                        height = this.gridControl.GetRowHeight(p2.Bottom);
                        arrowPoint = this.gridControl.RowGroupDropArea.PointToScreen(arrowPoint);
                        arrowPoint = new Point(arrowPoint.X - 1, arrowPoint.Y - ((downBitmap.Height)));
                        redArrowIndicatorDragHelper.DoDrag(arrowPoint, DragDropEffects.Copy);
                        redArrowIndicatorDragHelper.DragWindow.ShowWindowTopMost();
                    }
                }
                else
                {
                    redArrowIndicatorDragHelper.DoDrag(hiddenPoint, DragDropEffects.Copy);
                }
            }
            else if (this.GridControl.RectangleToScreen(this.gridControl.GroupDropArea.Bounds).Contains(PointToScreen(location)))
            {
                colRange = meanRange + (this.gridControl.PivotColumns.Count * 2) - 1;
                p2 = this.gridControl.GroupDropArea.PointToRangeInfo(location);
                if (p2.Bottom == 2 && p2.Left != meanRange && p2.Left <= colRange && p2.Left > 1 && this.Model.ColWidths[p2.Left] != 5 && p2.Left % 2 == 0)
                {
                    Rectangle cellRectangle = this.gridControl.GroupDropArea.RangeInfoToRectangle(p2);
                    Point arrowPoint = new Point(cellRectangle.X, cellRectangle.Y);
                    height = this.gridControl.GetRowHeight(p2.Bottom);
                    arrowPoint = this.gridControl.GroupDropArea.PointToScreen(arrowPoint);
                    arrowPoint = new Point(arrowPoint.X - 1, arrowPoint.Y - ((downBitmap.Height)));
                    redArrowIndicatorDragHelper.DoDrag(arrowPoint, DragDropEffects.Copy);
                    redArrowIndicatorDragHelper.DragWindow.ShowWindowTopMost();
                }
                else if (p2.Bottom == 2 && p2.Left != meanRange && p2.Left >= colRange + 1 && p2.Left > 1 && this.Model.ColWidths[p2.Left] != 5)
                {
                    GridRangeInfo lastColRange = GridRangeInfo.Cell(p2.Bottom, colRange);
                    Rectangle cellRectangle = this.gridControl.GroupDropArea.RangeInfoToRectangle(lastColRange);
                    Point arrowPoint = new Point(cellRectangle.X + cellRectangle.Width, cellRectangle.Y);
                    arrowPoint = this.gridControl.GroupDropArea.PointToScreen(arrowPoint);
                    arrowPoint = new Point(arrowPoint.X - 1, arrowPoint.Y - ((downBitmap.Height)));
                    redArrowIndicatorDragHelper.DoDrag(arrowPoint, DragDropEffects.Copy);
                    redArrowIndicatorDragHelper.DragWindow.ShowWindowTopMost();
                }
                else if (p2.Bottom == 2 && p2.Left == meanRange)//&& p2.Left > 1 && this.Model.ColWidths[p2.Left] != 5))
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
            else if (this.GridControl.RectangleToScreen(this.gridControl.FilterArea.Bounds).Contains(PointToScreen(location)))
            {
                redArrowIndicatorDragHelper.DoDrag(hiddenPoint, DragDropEffects.Copy);
            }
            else
            {
                redArrowIndicatorDragHelper.DoDrag(hiddenPoint, DragDropEffects.Copy);
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

        #region Overrides
        protected override void OnQueryCellInfo(GridQueryCellInfoEventArgs e)
        {
            #region color settings

            Color clrBack = Color.Empty;
            Color headerBorderTop = Color.Empty;
            Color headerBorderLeft = Color.Empty;

            GridStyleInfo standard = BaseStylesMap["Standard"].StyleInfo;
            if (e.RowIndex == 2 && e.ColIndex == 1 && e.Style.Text == string.Empty)
                e.Style.Text = PivotAnalysis.SR.GetString(PivotAnalysis.SR.DropPivotFieldsheretoFilterBy);

            if (this.Model.Options.GridVisualStylesDrawing.GetGroupDropAreaColors(out clrBack, out headerBorderTop, out headerBorderLeft))
            {
                standard.BackColor = clrBack;
                switch (this.Model.Options.GridVisualStyles)
                {
                    case Forms.GridVisualStyles.Metro:
                        this.Properties.BackgroundColor = Color.FromArgb(35, 130, 195);
                        break;
                    case Forms.GridVisualStyles.Office2007Blue:
                    case Forms.GridVisualStyles.Office2010Blue:
                        this.Properties.BackgroundColor = Color.FromArgb(227, 239, 255);
                        break;
                    case Forms.GridVisualStyles.Office2010Black:
                        this.Properties.BackgroundColor = Color.FromArgb(100, 100, 110);
                        break;
                    case Forms.GridVisualStyles.Office2007Black:
                    case Forms.GridVisualStyles.Office2007Silver:
                        this.Properties.BackgroundColor = Color.FromArgb(240, 241, 242);
                        break;
                    case Forms.GridVisualStyles.Office2010Silver:
                        this.Properties.BackgroundColor = Color.FromArgb(223, 227, 222);
                        break;
                    default:
                        this.Properties.BackgroundColor = Color.FromArgb(240, 241, 242);
                        break;
                }
            }
            #endregion
            base.OnQueryCellInfo(e);
        }

        #region re DragDrop
        protected override void OnMouseDown(MouseEventArgs e)
        {
            Graphics g = CreateGraphics();
            Pen p1 = new Pen(Color.Red);
            isMouseDown = true;
            locX = e.X;
            locY = e.Y;
            this.PointToRowCol(e.Location, out checkRow, out checkCol);
            sourceIndex = checkCol;
            base.OnMouseDown(e);


        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            this.PointToRowCol(new Point(e.X, e.Y), out dropRow, out dropCol);
            localStyle = this[checkRow, checkCol];
            if (this.isMouseDown && checkCol != -1 && checkRow != -1 && localStyle.CellValue.ToString() != string.Empty)
            {
                Point cursorPos = new Point(e.X, e.Y);
                int rowIndex, colIndex = 0;
                this.PointToRowCol(cursorPos, out rowIndex, out colIndex);
                if (rowIndex != checkRow || colIndex != checkCol)
                {
                    if (!started)
                    {
                        if (checkCol > 1)
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
                    if (checkCol > 1)
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
            isMouseDown = false;
            started = false;
            CloseDragHeader();
            CloseRedArrowIndicator();
            helper = new DragDropHelper(this.GridControl);
            if (wasDragging)
            {
                wasDragging = false;
                GridStyleInfo dragCellStyle = this[2, sourceIndex];
                Rectangle topLeftCell3 = new Rectangle(this.gridControl.Bounds.Location.X, this.gridControl.Bounds.Location.Y, this.gridControl.RowGroupDropArea.Width, this.gridControl.RowGroupDropArea.Height);
                Rectangle groupAreaBounds = this.gridControl.GroupDropArea.Bounds;
                if (IsRightToLeft())
                {
                    if (this.GridControl.ShowPivotTableFieldList)
                    {
                        topLeftCell3.Width += this.GridControl.pivotSchemaDesigner.Width;
                        groupAreaBounds.Width += this.GridControl.PivotSchemaDesigner.Width;
                    }
                }
                if (this.GridControl.RectangleToScreen(topLeftCell3).Contains(PointToScreen(e.Location)))
                {
                    GridRangeInfo rangeinRowGroupArea = this.gridControl.RowGroupDropArea.PointToRangeInfo(e.Location);
                    helper.DropinPivotRows(dragCellStyle, -1, rangeinRowGroupArea.Left, this.gridControl.FilterArea);
                }
                else if (this.GridControl.RectangleToScreen(groupAreaBounds).Contains(PointToScreen(e.Location)))
                {
                    GridRangeInfo rangeinGroupArea = this.gridControl.GroupDropArea.PointToRangeInfo(e.Location);
                    meanRange = (2 * this.gridControl.PivotCalculations.Count) + 1;
                    colRange = meanRange + (this.gridControl.PivotColumns.Count * 2) - 1;
                    if (rangeinGroupArea.Left <= meanRange && this.gridControl.GroupDropArea.Model.ColWidths[rangeinGroupArea.Left] != 5)
                    {
                        helper.DropinPivotCalculations(dragCellStyle, -1, rangeinGroupArea.Left, this.gridControl.FilterArea);
                    }
                    else if (rangeinGroupArea.Left > meanRange && this.gridControl.GroupDropArea.Model.ColWidths[rangeinGroupArea.Left] != 5)
                    {
                        helper.DropinPivotColumns(dragCellStyle, -1, rangeinGroupArea.Left, this.gridControl.FilterArea);
                    }
                }
                else
                {
                    if (this.GridControl.pivotSchemaDesigner != null)
                    {
                        switch (helper.GetSchemaContainer(e.Location))
                        {
                            case "PivotRows":
                                helper.DropinPivotRows(localStyle);
                                break;
                            case "PivotColumns":
                                helper.DropinPivotColumns(localStyle);
                                break;
                            case "PivotCalculations":
                                helper.DropinPivotCalculations(localStyle);
                                break;
                            case "TableFieldList":
                                helper.DropInTableFieldList(localStyle);
                                break;
                            case "PivotFilters":
                                break;
                        }
                    }
                }
            }
            base.OnMouseUp(e);
        }
        #endregion

        #endregion

        #region Helper Methods
        private void ResetCoveredCells()
        {
            int startIndex = this.gridControl.Filters.Count==0?1:((2*this.gridControl.Filters.Count)+1);
            this.CoveredRanges.Clear();
            this.CoveredRanges.ResetCache();
            this.CoveredRanges.Add(GridRangeInfo.Cells(2, startIndex, 2, this.ColCount));
        }
        #endregion

        #region To Resize the items in FilterBar [Alternate for events to invoke this whenever it is needed]
        void Filters_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ApplySize();
            ApplyItems();
        }

        /// <summary>
        /// Helper function which used to apply the size of the cells in the FilterBar. An alter to avoid events 
        /// </summary>
        internal void ApplySize()
        {
            int maxRange = this.gridControl.Filters.Count + this.gridControl.Filters.Count;
            for (int i = 1; i < maxRange; i++)
            {
                if (i % 2 != 0)
                {
                    this.Model.ColWidths[i] = 5;
                }
            }
        }

        /// <summary>
        /// Helper function which used to apply the value to the cells in the FilterBar. An alter to avoid events 
        /// </summary>
        internal void ApplyItems()
        {
            ResetCoveredCells();
            int maxRange = (this.gridControl.Filters.Count + this.gridControl.Filters.Count);
            if (this.gridControl.Filters.Count > 0)
            {
                for (int j = 1; j <= maxRange; j++)
                {
                    if (j <= maxRange)
                    {
                        if (this.ColWidths[j] != 5 && (j % 2 == 0))
                        {
                            this[2, j].CellType = "ColumnHeaderCell";
                            this[2, j].VerticalAlignment = GridVerticalAlignment.Middle;
                            int index = (j) / 2;
                            if (this.gridControl.collectionTablelist.ContainsKey(this.gridControl.Filters[index - 1].DimensionName))
                            {
                                var item = this.gridControl.collectionTablelist.Where(f => f.Key == this.gridControl.Filters[index - 1].DimensionName).FirstOrDefault();
                                if (!string.IsNullOrEmpty(item.Value.FieldHeader))
                                {
                                    this[2, j].Text = item.Value.FieldHeader;
                                    this.gridControl.Filters[index - 1].DimensionHeader = item.Value.FieldHeader;
                                }
                            }
                            if (string.IsNullOrEmpty(this[2, j].Text))
                                this[2, j].Text = (!string.IsNullOrEmpty(this.gridControl.Filters[index - 1].DimensionHeader)) ? (this.gridControl.Filters[index - 1].DimensionHeader) : (this.gridControl.Filters[index - 1].DimensionName);
                        }
                    }
                }
            }
            else
            {
                this[2, 1].Text = "Drop Filter Fields here";
            }
        }
        #endregion
    }
}
