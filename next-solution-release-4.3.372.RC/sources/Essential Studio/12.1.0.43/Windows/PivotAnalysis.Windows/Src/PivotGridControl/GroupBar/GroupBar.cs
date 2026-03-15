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
    public class GroupBar : GridControl
    {
        private PivotGridControlBase gridControl; FilterHelper filterHelper;
        private int topLeftCellWidth;
        private int maxRange = 0, meanRange = 0, colRange = 0;
        private Syncfusion.Windows.Forms.PopupControlContainer popupControlContainer1;
        internal Cursor cursor = null;
        private DragDropHelper dragDropHelper;
        private GroupDragHelper headerDragHelper = null;
        private Point hiddenPoint = new Point(10000, 10000);
        private bool wasDragging = false;
        private int dropRow, dropCol, index = 0;
        private int sourceIndex = 0;
        private Rectangle originBounds = new Rectangle();
        private GridStyleInfo localStyle;
        private int targetindex = 0;
        private GridStyleInfo sourceItem;
        private GroupDragHelper redArrowIndicatorDragHelper = null;
        private int cellMinSize = 40, cellMaxSize = 55;
        private int defaultWidth = 60;
        private int checkRow, checkCol;
        private int locX, locY;
        private bool isMouseDown = false;
        private bool started = false;
        private FilterExpression fItem = null;
        private FilterDropDown filter = null;
        /// <summary>
        /// Constructor
        /// </summary>
        public GroupBar()
        {

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
            this.AllowSelection = GridSelectionFlags.None;
            this.ShowCurrentCellBorderBehavior = GridShowCurrentCellBorder.HideAlways;
            this.AllowDrop = false;
            this.DefaultRowHeight = 25;
            this.AllowDragSelectedCols = false;
            this.AllowDragSelectedRows = false;
            this.ControllerOptions = ~GridControllerOptions.OleDataSource;
            CellModels.Add("PivotGridHeaderCell", new PivotGridHeaderCellModel(this.Model));
            CellModels.Add("PivotGridExpandCell", new PivotGridExpandCellCellModel(this.Model));
            CellModels.Add("TemplateCell", new PivotGridTemplateCellModel(this.Model));
            CellModels.Add("ColumnHeaderCell", new PivotGridSortColumnHeaderCellModel(this.Model));

            GridStyleInfo standard = BaseStylesMap["Standard"].StyleInfo;
            standard.Borders.All = GridBorder.Empty;
            standard.CellType = "Static";
            standard.Themed = true;
            this.Model.CoveredRanges.Add(GridRangeInfo.Cells(1, 1, 1, this.ColCount));

            this.popupControlContainer1 = new Syncfusion.Windows.Forms.PopupControlContainer();
            this.gridControl.PivotCalculations.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(PivotCalculations_CollectionChanged);
            this.CellButtonClicked += new GridCellButtonClickedEventHandler(GroupBar_CellButtonClicked);
            this.CellClick += new GridCellClickEventHandler(GroupBar_CellClick);
            this.popupControlContainer1.CloseUp += new PopupClosedEventHandler(popupControlContainer1_CloseUp);
            filterHelper = new FilterHelper(this.gridControl);
            dragDropHelper = new DragDropHelper(this.GridControl);
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
        public GroupBar(PivotGridControlBase tableControl)
        {
            if (tableControl != null)
            {
                this.gridControl = tableControl;
                this.gridControl.GroupDropArea = this;
                WireGrid();
            }


        }

        #region Filter
        class PivotFilterElementComparer : IComparer<FilterItemElement>
        {
            IComparer comparer = null;
            public PivotFilterElementComparer(IComparer comparer)
            {
                this.comparer = comparer;
            }
            public int Compare(FilterItemElement x, FilterItemElement y)
            {
                return comparer.Compare(x.Key, y.Key);
            }
        }
        int filterIndex = 0;
        void GroupBar_CellButtonClicked(object sender, GridCellButtonClickedEventArgs e)
        {
            if (this.gridControl.AllowFiltering)
            {
                for (int i = 0; i < this.gridControl.PivotColumns.Count; i++)
                {
                    string fieldName = this.gridControl.PivotColumns[i].FieldHeader != null ? this.gridControl.PivotColumns[i].FieldHeader : this.gridControl.PivotColumns[i].FieldMappingName;
                    if (this.Model[e.RowIndex, e.ColIndex].Text == fieldName)
                    {
                        filterIndex = i;
                        break;
                    }
                }
                this.popupControlContainer1.RightToLeft = this.gridControl.IsRightToLeft() ? RightToLeft.Yes : RightToLeft.No;
                PivotItem pivotItem = this.gridControl.PivotColumns[filterIndex];
                PropertyDescriptor descriptor = filterHelper.GetPropertyDescriptor(pivotItem.FieldMappingName);
                if (pivotItem.Comparer == null && descriptor != null && pivotItem.Format == null)
                {
                    pivotItem.Comparer = (IComparer)this.gridControl.PivotEngine.AddComparers(descriptor.PropertyType);
                }

                fItem = this.gridControl.PivotFilters.Where(f => f.Name == pivotItem.FieldMappingName).FirstOrDefault();
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

                    if (ftempCollection.FilteredValues.Count>0)
                    {
                        foreach (var temp in ftempCollection)
                        {
                            if (ftempCollection.FilteredValues.Contains(temp.Key))
                                temp.IsSelected = true;
                            else
                                temp.IsSelected = false;
                        }
                    }
                    else
                        ftempCollection = filterHelper.GetFilterItem(descriptor);
                    if (pivotItem.Comparer != null)
                    {
                        var temp = ftempCollection[0];// remove (ALL) before sorting...
                        ftempCollection.RemoveAt(0);
                        ftempCollection.Sort(new PivotFilterElementComparer(pivotItem.Comparer));
                        ftempCollection.Insert(0, temp); //Insert (ALL) back...
                    }
                    filter.FilterList = ftempCollection;
                }
                popupControlContainer1.Controls.Add(filter);
                Point location = this.PointToScreen(new Point(locX, locY));
                if (popupControlContainer1.IsShowing())
                    popupControlContainer1.HidePopup(PopupCloseType.Done);
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

        #region Sort
        void GroupBar_CellClick(object sender, GridCellClickEventArgs e)
        {
            colRange = (this.gridControl.PivotColumns.Count + this.gridControl.PivotColumns.Count) + meanRange;
            for (int i = 0; i < this.gridControl.PivotColumns.Count; i++)
            {
                string name =  this.gridControl.PivotColumns[i].FieldHeader != null ? this.gridControl.PivotColumns[i].FieldHeader:this.gridControl.PivotColumns[i].FieldMappingName;
                if (this.Model[e.RowIndex, e.ColIndex].Text == name)
                {
                    index = i;
                    break;
                }
            }
            if (e.ColIndex <= colRange && e.ColIndex > -1 && e.ColIndex > meanRange)
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
            isSortCursorOnMove = false;
        }
        bool isSortCursorOnMove = false;
        private bool asc = false;
        private void Sort(int rowIndex, int colIndex, int index)
        {

            if (colIndex > meanRange)
            {
                PivotItem pivotItems = this.GridControl.PivotColumns[index];
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

        #region Overrides

        #region DragDrop
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
            if (isMouseDown)
            {
                if (!started)
                {
                    Point cursorPos = new Point(e.X, e.Y);
                    int rowIndex, colIndex = 0;
                    this.PointToRowCol(cursorPos, out rowIndex, out colIndex);
                    if (rowIndex != checkRow || colIndex != checkCol)
                    {
                        if (!string.IsNullOrEmpty(localStyle.Text) && checkRow == 2)
                        {
                            if (checkCol < meanRange)
                            {
                                if (this.gridControl.PivotCalculations.Count > 1)
                                {
                                    this.OpenDragHeader();
                                    this.OpenRedArrowIndicator();
                                }
                            }
                            else if (checkCol <= colRange)
                            {
                                if (this.gridControl.PivotColumns.Count > 1)
                                {
                                    this.OpenDragHeader();
                                    this.OpenRedArrowIndicator();
                                }
                            }
                            else
                            {
                                this.OpenDragHeader();
                                this.OpenRedArrowIndicator();
                            }
                        }
                        started = true;
                    }
                }
                if (started)
                {
                    if (((this.gridControl.PivotCalculations.Count > 1) || (this.gridControl.PivotColumns.Count > 1)) && this.isMouseDown && !string.IsNullOrEmpty(localStyle.Text) && checkRow == 2)
                    {
                        this.originBounds = this.GetGridWindow().GridRectangleToScreen(this.RangeInfoToRectangle(GridRangeInfo.Cell(dropRow, dropCol)));

                        int n1 = meanRange + 1;

                        if (checkCol == 2 || checkCol < meanRange)
                        {
                            if (this.gridControl.PivotCalculations.Count > 1)
                            {
                                UpdateDragHeader();
                                UpdateRedArrowIndicatorBitmap(e.Location);
                            }
                        }
                        else if (checkCol == meanRange + 1)
                        {
                            if (this.gridControl.PivotColumns.Count > 1)
                            {
                                UpdateDragHeader();
                                UpdateRedArrowIndicatorBitmap(e.Location);
                            }
                        }
                        else
                        {
                            UpdateDragHeader();
                            UpdateRedArrowIndicatorBitmap(e.Location);
                        }
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
            dragDropHelper = new DragDropHelper(this.GridControl);

            if (wasDragging)
            {
                isSortCursorOnMove = true;
                targetindex = checkCol;
                wasDragging = false;
                GridStyleInfo dragCellStyle = this[2, sourceIndex];
                Rectangle topLeftCell3 = new Rectangle(this.gridControl.Bounds.Location.X, this.gridControl.Bounds.Location.Y, this.gridControl.RowGroupDropArea.Width, this.gridControl.RowGroupDropArea.Height);
                #region Within PivotGridControl
                if (sourceIndex != targetindex && targetindex != -1 && sourceIndex < colRange && sourceIndex != -1)
                {
                    if (checkRow == 2)
                    {
                        if (targetindex < meanRange || targetindex == meanRange)
                            dragDropHelper.DropinPivotCalculations(dragCellStyle, sourceIndex, targetindex, this.gridControl.GroupDropArea);
                        else if (targetindex > meanRange)
                            dragDropHelper.DropinPivotColumns(dragCellStyle, sourceIndex, targetindex, this.gridControl.GroupDropArea);
                    }
                }
                else if (this.GridControl.RectangleToScreen(topLeftCell3).Contains(PointToScreen(e.Location)))
                {
                    GridRangeInfo cRange = this.gridControl.RowGroupDropArea.PointToRangeInfo(e.Location);
                    dragDropHelper.DropinPivotRows(dragCellStyle, sourceIndex, cRange.Left, this.gridControl.GroupDropArea);
                }
                else if (this.GridControl.RectangleToScreen(this.gridControl.FilterArea.Bounds).Contains(PointToScreen(e.Location)))
                {
                    if (sourceIndex < meanRange)
                        dragDropHelper.DropinPivotFilters(dragCellStyle, sourceIndex, this.gridControl.GroupDropArea);
                    else if (sourceIndex > meanRange && sourceIndex <= colRange)
                        dragDropHelper.DropinPivotFilters(dragCellStyle, sourceIndex, this.gridControl.GroupDropArea);
                }
                else
                {
                    if (this.GridControl.pivotSchemaDesigner != null)
                    {
                        switch (dragDropHelper.GetSchemaContainer(e.Location))
                        {
                            case "PivotRows":
                                dragDropHelper.DropinPivotRows(localStyle, this.gridControl.GroupDropArea);
                                break;
                            case "PivotColumns":
                                dragDropHelper.DropinPivotColumns(localStyle, this.gridControl.GroupDropArea);
                                break;
                            case "PivotCalculations":
                                dragDropHelper.DropinPivotCalculations(localStyle, this.gridControl.GroupDropArea);
                                break;
                            case "TableFieldList":
                                dragDropHelper.DropInTableFieldList(localStyle, sourceIndex, this.gridControl.GroupDropArea);
                                break;
                            case "PivotFilters":
                                dragDropHelper.DropinPivotFilters(localStyle);
                                break;
                        }
                    }
                }

                #endregion

                if (this.GridControl.pivotSchemaDesigner != null)
                {
                    this.GridControl.pivotSchemaDesigner.ResetItemCollectionLists(true);
                    this.GridControl.pivotSchemaDesigner.PopulatePivotItems();
                }
            }
            base.OnMouseUp(e);
        }
        #endregion

        protected override void OnQueryRowHeight(GridRowColSizeEventArgs e)
        {
            if (e.Index == 1)
            {
                e.Size = 2;
                e.Handled = true;
            }

            base.OnQueryRowHeight(e);
        }

        protected override void OnDrawCellDisplayText(GridDrawCellDisplayTextEventArgs e)
        {
            if (this.Model.Options.GridVisualStyles == GridVisualStyles.Metro)
            {
                e.Style.Font.Bold = true;
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
                if (this.GridVisualStyles == Forms.GridVisualStyles.Metro)
                {
                    standard.BackColor = Color.FromArgb(246, 247, 247);//clrBack;
                    this.Properties.BackgroundColor = Color.FromArgb(246, 247, 247);// clrBack;
                    standard.Font.Size = 9f;
                    standard.Font.Bold = true;
                }
                else
                {
                    standard.BackColor = clrBack;
                    this.Properties.BackgroundColor = clrBack;
                    e.Style.Font.Bold = false;
                    if (this.GridVisualStyles == Forms.GridVisualStyles.Office2010Black)
                        e.Style.TextColor = Color.White;
                }                    
            
            }
            if (e.RowIndex == 2)
            {
                e.Style.VerticalAlignment = GridVerticalAlignment.Middle;
            }
            if (e.RowIndex == 3)
            {
                e.Style.BackColor = Color.DimGray;
            }
            #endregion

            e.Style.Trimming = StringTrimming.EllipsisCharacter;
            base.OnQueryCellInfo(e);
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
        /// gets the location of the drag window
        /// </summary>
        internal Point GetDragWindowLocation(Point srcLocation)
        {
            Point pt = Control.MousePosition;
            return new Point(pt.X, pt.Y);
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
            Rectangle r1 = RectangleToScreen(this.gridControl.GroupDropArea.Bounds);
            int height;
            GridRangeInfo p2 = this.PointToRangeInfo(location);
            Rectangle cellRectangle; Point arrowPoint = Point.Empty;

            Rectangle topLeftCell = new Rectangle(this.gridControl.Bounds.Location.X, this.gridControl.Bounds.Location.Y, this.gridControl.RowGroupDropArea.Width, this.gridControl.RowGroupDropArea.Height);
            Rectangle topLeftCell2 = new Rectangle(this.gridControl.RowGroupDropArea.Location.X, this.gridControl.RowGroupDropArea.Location.Y, this.gridControl.RowGroupDropArea.Size.Width, this.gridControl.RowGroupDropArea.Size.Height);
            // Fix : Drag drop  -  original topleftcell bounds
            Rectangle topLeftCell3 = new Rectangle(this.gridControl.Bounds.Location.X, this.gridControl.Bounds.Location.Y, this.gridControl.RowGroupDropArea.Width, this.gridControl.RowGroupDropArea.Height);

            if (this.GridControl.RectangleToScreen(topLeftCell3).Contains(PointToScreen(location)))
            {
                p2 = this.gridControl.RowGroupDropArea.PointToRangeInfo(location);
                if (p2.Bottom == 1 && p2.Left <= (this.gridControl.PivotRows.Count * 2))
                {
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
                    else if (p2.Left == ((this.gridControl.PivotRows.Count * 2) - 1))//&& this.gridControl.RowGroupDropArea.Model.ColWidths[p2.Left] == 5)
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
                colRange = meanRange + (this.gridControl.PivotColumns.Count * 2);
                if (p2.Bottom == 2 && p2.Left != meanRange && p2.Left <= colRange && p2.Left > 1 && this.Model.ColWidths[p2.Left] != 5)
                {

                    cellRectangle = this.RangeInfoToRectangle(p2);
                    arrowPoint = new Point(cellRectangle.X, cellRectangle.Y);

                    height = this.GetRowHeight(p2.Bottom);
                    arrowPoint = this.PointToScreen(arrowPoint);
                    arrowPoint = new Point(arrowPoint.X - 1, arrowPoint.Y - ((downBitmap.Height)));
                    redArrowIndicatorDragHelper.DoDrag(arrowPoint, DragDropEffects.Copy);
                    redArrowIndicatorDragHelper.DragWindow.ShowWindowTopMost();
                }
                else if (p2.Bottom == 2 && p2.Left != meanRange && p2.Left >= colRange && p2.Left > 1 && this.Model.ColWidths[p2.Left] != 5)
                {
                    GridRangeInfo lastColRange = GridRangeInfo.Cell(p2.Bottom, colRange - 1);
                    cellRectangle = this.gridControl.GroupDropArea.RangeInfoToRectangle(lastColRange);
                    arrowPoint = new Point(cellRectangle.X + cellRectangle.Width, cellRectangle.Y);
                    arrowPoint = this.gridControl.GroupDropArea.PointToScreen(arrowPoint);
                    arrowPoint = new Point(arrowPoint.X - 1, arrowPoint.Y - ((downBitmap.Height)));
                    redArrowIndicatorDragHelper.DoDrag(arrowPoint, DragDropEffects.Copy);
                    redArrowIndicatorDragHelper.DragWindow.ShowWindowTopMost();
                }
                else if (p2.Bottom == 2 && p2.Left == meanRange)//&& p2.Left > 1 && this.Model.ColWidths[p2.Left] != 5))
                {
                    GridRangeInfo lastColRange = GridRangeInfo.Cell(p2.Bottom, meanRange - 1);
                    cellRectangle = this.gridControl.GroupDropArea.RangeInfoToRectangle(lastColRange);
                    arrowPoint = new Point(cellRectangle.X + cellRectangle.Width, cellRectangle.Y);
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

        #region ApplyItems and ApplySize

        void PivotCalculations_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ApplySize();
            ApplyItems();
        }

        /// <summary>
        /// Helper function which used to apply the size of the cells in the GroupBar. An alter to avoid events 
        /// </summary>
        internal void ApplySize()
        {
            maxRange = this.gridControl.PivotCalculations.Count > 0 ? (this.gridControl.PivotCalculations.Count + this.gridControl.PivotCalculations.Count) : 2;
            meanRange = maxRange + 1;
            colRange = maxRange + (this.gridControl.PivotColumns.Count + this.gridControl.PivotColumns.Count);
            if (this.gridControl.ShowCalculationsAsColumns)
            {
                topLeftCellWidth = gridControl.PivotRows.Count * 90;
            }
            else
            {

                if (this.gridControl.PivotCalculations.Count > 1)
                {
                    topLeftCellWidth = (gridControl.PivotRows.Count + 1) * 90;
                }
                else
                {
                    topLeftCellWidth = gridControl.PivotRows.Count * 90;
                }
            }

            int diff2 = topLeftCellWidth - defaultWidth;
            int diff = diff2 - ((maxRange / 2) * 5);
            int cellSize = diff / (maxRange / 2);

            int itemCount = maxRange / 2;
            int emptyCount = maxRange / 2;
            int proposedWidth = (itemCount * cellMaxSize) + (emptyCount * 5);
            int exceedWidth; int presentWidth;

            for (int j = 1; j <= 2 * (this.gridControl.PivotRows.Count); j++)
            {
                if (j % 2 == 0)
                    this.gridControl.RowGroupDropArea.Model.ColWidths[j] = this.gridControl.RowGroupDropArea.DefaultColWidth - 5;
            }
            for (int k = 1; k <= this.gridControl.PivotRows.Count; k++)
            {
                this.gridControl.Model.ColWidths[k] = this.gridControl.DefaultColWidth;
            }

            if (proposedWidth < topLeftCellWidth)
            {
                if (this.gridControl.PivotRows.Count > 1)
                {
                    cellSize = cellMaxSize;
                    presentWidth = (itemCount * cellSize) + (emptyCount * 5);
                    defaultWidth = topLeftCellWidth - proposedWidth;
                }
                else
                {
                    cellSize = cellMaxSize;
                    defaultWidth = topLeftCellWidth - proposedWidth;
                }
            }
            else if (proposedWidth >= topLeftCellWidth)
            {

                if (this.gridControl.PivotRows.Count > 1)
                {
                    cellSize = cellMinSize;
                    presentWidth = (itemCount * cellSize) + (emptyCount * 5);
                    if (presentWidth < topLeftCellWidth)
                    {
                        defaultWidth = topLeftCellWidth - presentWidth;
                    }
                    else
                    {
                        defaultWidth = 20;
                        for (int j = 1; j <= 2 * (this.gridControl.PivotRows.Count); j++)
                        {
                            if (this.gridControl.RowGroupDropArea.Model.ColWidths[j] != 5)
                                this.gridControl.RowGroupDropArea.Model.ColWidths[j] += 10;
                        }
                        for (int k = 1; k <= this.gridControl.PivotRows.Count; k++)
                        {
                            this.gridControl.Model.ColWidths[k] += 10;
                        }
                    }
                }
                else
                {
                    cellSize = cellMinSize;
                    presentWidth = (itemCount * cellSize) + (emptyCount * 5);
                    if (presentWidth < topLeftCellWidth)
                    {
                        defaultWidth = topLeftCellWidth - presentWidth;
                    }
                    else if (presentWidth == topLeftCellWidth)
                    {
                        defaultWidth = 20;
                        for (int j = 1; j <= 2 * (this.gridControl.PivotRows.Count); j++)
                        {
                            if (this.gridControl.RowGroupDropArea.Model.ColWidths[j] != 5)
                                this.gridControl.RowGroupDropArea.Model.ColWidths[j] += 10;
                        }
                        for (int k = 1; k <= this.gridControl.PivotRows.Count; k++)
                        {
                            this.gridControl.Model.ColWidths[k] += 10;
                        }
                    }
                    else
                    {

                        exceedWidth = presentWidth - topLeftCellWidth;
                        defaultWidth = 20;
                        this.gridControl.Model.ColWidths[1] = presentWidth + defaultWidth;
                        this.gridControl.RowGroupDropArea.Model.ColWidths[2] = presentWidth + defaultWidth;

                    }

                }
            }
            else
            {
            }

            for (int i = 1; i < Model.ColCount; i++)
            {
                if (i > 0 && i % 2 != 0 && i <= meanRange)
                {
                    this.ColWidths.SetRange(i, i, 5);

                }
                if (i > meanRange && i % 2 != 0)
                {
                    this.ColWidths.SetRange(i, i, 5);

                }
                if (i > meanRange && i % 2 == 0 && Model.ColWidths[i] != 5)
                {
                    this.Model.ColWidths[i] = 90;
                }
                if (i <= maxRange && (i % 2) == 0)
                {
                    if (i < meanRange)
                    {
                        if (this.gridControl.PivotCalculations.Count > 1)
                        {
                            if (diff > 0)
                                this.Model.ColWidths[i] = cellSize;//diff / (maxRange / 2);
                            else
                                this.Model.ColWidths[i] = 30;
                        }
                        else
                        {
                            this.Model.ColWidths[i] = 80;
                        }
                    }
                    else
                    {
                        this.Model.ColWidths[i] = 80;
                        defaultWidth = (diff / (maxRange / 2));

                    }
                }

                int differ = (this.DefaultColWidth * (maxRange / 2)) + (5 * (maxRange / 2));//maxRange * 33;
                if (i == meanRange)
                {
                    if (this.gridControl.PivotCalculations.Count > 1)
                    {
                        this.Model.ColWidths[i] = defaultWidth;
                    }
                    else
                    {
                        int n1 = defaultWidth - (this.gridControl.PivotCalculations.Count * 30);
                        if (n1 > 0)
                            this.Model.ColWidths[i] = n1;
                        else
                            this.Model.ColWidths[i] = 5; // as a minimum size to set for the mean range cell
                    }
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
            colRange = maxRange + (this.gridControl.PivotColumns.Count + this.gridControl.PivotColumns.Count);
            sourceItem = this[2, sourceIndex];

            PivotItem draggedItem = new PivotItem { FieldMappingName = sourceItem.Text };
            if (this.gridControl.ShowCalculationsAsColumns)
            {
                topLeftCellWidth = gridControl.PivotRows.Count * 90;
            }
            else
            {
                if (this.gridControl.PivotCalculations.Count > 1)
                {
                    topLeftCellWidth = (gridControl.PivotRows.Count + 1) * 90;
                }
                else
                {
                    topLeftCellWidth = gridControl.PivotRows.Count * 90;
                }
            }

            for (int i = 2; i <= maxRange; i++)
            {
                if (i / 2 != 0 && this.ColWidths[i] != 5)
                {
                    try
                    {
                        this[2, i].CellType = "TemplateCell";
                        this[2, i].VerticalAlignment = GridVerticalAlignment.Middle;
                        int index = (i) / 2;
                        string fieldHeaderName = this.gridControl.PivotCalculations[index - 1].FieldHeader;
                        string fieldName = this.gridControl.PivotCalculations[index - 1].FieldName;
                        this[2, i].Text = (!string.IsNullOrEmpty(fieldHeaderName)) ? fieldHeaderName : fieldName;
                        this[2, i].Description = (fieldHeaderName != fieldName) ? fieldHeaderName : fieldName;
                        this[2, i].Enabled = true;
                    }
                    catch
                    {
                    }
                }

            }
            for (int j = meanRange; j <= colRange; j++)
            {
                if (j > meanRange && j <= colRange)
                {
                    int num = ColIndexToField(j);
                    if (this.ColWidths[num] != 5)
                    {
                        try
                        {
                            this[2, j].CellType = "ColumnHeaderCell";
                            this[2, j].VerticalAlignment = GridVerticalAlignment.Middle;
                            int index = (j - maxRange) / 2;
                            string fieldHeaderName = this.gridControl.PivotColumns[index - 1].FieldHeader;
                            string fieldMappingName = this.gridControl.PivotColumns[index - 1].FieldMappingName;
                            this[2, j].Text = (!string.IsNullOrEmpty(fieldHeaderName)) ? fieldHeaderName : fieldMappingName;
                            this[2, j].Description = (!string.IsNullOrEmpty(fieldHeaderName)) ? fieldHeaderName : fieldMappingName;
                            if (string.IsNullOrEmpty(fieldHeaderName))
                                this.gridControl.PivotColumns[index - 1].FieldHeader = fieldMappingName;
                        }
                        catch
                        {
                            if (this.ColWidths[j] != 5)
                            {
                                this[2, j].CellType = "Static";
                                this[2, j].Text = "";
                            }
                        }
                    }
                }
            }
            // better reduce the colcount to the required amount
            for (int i = colRange + 1; i <= this.ColCount; i++)
            {
                this[2, i].CellType = "Static";
                this[2, i].Text = "";
            }
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

        internal PivotGridGroupDragWindow dragWindow = new PivotGridGroupDragWindow();

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        public PivotGridGroupDragWindow DragWindow
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
    public class PivotGridGroupDragWindow : DragWindow
    {
        private Bitmap dragBitmap = null;
        private bool isDragging = false;
        private Point origin = new Point(-30000, -30000);

        //// Constructor for GridGroupDragWindow

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        public PivotGridGroupDragWindow()
        {
            this.TransparencyKey = Color.Red;
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
            this.Visible = false;
            return true;
        }


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
                    Size size = value.Size;
                    this.origin = Point.Empty;

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
