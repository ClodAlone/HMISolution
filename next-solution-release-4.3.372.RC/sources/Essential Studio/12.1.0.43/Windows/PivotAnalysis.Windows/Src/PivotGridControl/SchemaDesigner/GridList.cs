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
using System.Drawing;
using System.Windows.Forms;
using Syncfusion.Drawing;
using Syncfusion.PivotAnalysis.Base;
using System.Collections;
using System.Drawing.Drawing2D;
using System.ComponentModel;

namespace Syncfusion.Windows.Forms.PivotAnalysis
{
    [ToolboxItem(false)]
    public class GridList : GridControl
    {
        private Syncfusion.Windows.Forms.PopupControlContainer popupControlContainer1;
        private FilterHelper filterHelper;
        internal ContextMenuStrip cMenu = new ContextMenuStrip();
        DragDropHelper dragDropHelper;
        private GroupDragHelper headerDragHelper = null;
        private Point hiddenPoint = new Point(10000, 10000);
        internal Cursor cursor = null;
        private int sourceRow, sourceCol;
        private int colWidthOnRedArrowCreate = -1;
        private int rowHeightOnRedArrowCreate = -1;
        private bool wasDragging = false;
        GroupDragHelper redArrowIndicatorDragHelper = null;
        private Rectangle originBounds = new Rectangle();
        private bool isMouseDown = false;
        private GridStyleInfo style;
        private int checkRow, checkCol;
        private int locX, locY;

        /// <summary>
        /// The default contructor.
        /// </summary>
        public GridList()
            : base()
        {
            SetStyle(ControlStyles.ResizeRedraw, true);
            this.ShowColumnHeaders = false;
            this.RowCount = 1;
            this.ColCount = 1;
            this.RowHeights[0] = 0;
            this.ColWidths[0] = 0;
            this.DefaultGridBorderStyle = GridBorderStyle.None;
            this.Properties.BackgroundColor = this.TableStyle.BackColor;
            this.AllowDragSelectedRows = false;
            this.ResizeRowsBehavior = GridResizeCellsBehavior.None;
            this.ShowCurrentCellBorderBehavior = GridShowCurrentCellBorder.HideAlways;
            this.ActivateCurrentCellBehavior = GridCellActivateAction.None;
            this.selectionColor = this.AlphaBlendSelectionColor;
            this.AlphaBlendSelectionColor = Color.FromArgb(0, 0, 0, 0);
            this.GridVisualStyles = Syncfusion.Windows.Forms.GridVisualStyles.Office2007Blue;
            this.Office2007ScrollBars = true;
            this.ThemesEnabled = true;
            this.TableStyle.Themed = true;
            this.Height = 90;
            this.TableStyle.Borders.All = GridBorder.Empty;
            this.TableStyle.TextColor = Color.Black;
            this.ControllerOptions = GridControllerOptions.ClickCells;
            this.TableStyle.CellAppearance = GridCellAppearance.Flat;
            this.TableStyle.Font.Bold = false;
            this.TableStyle.CellType = "SchemaItem"; // was static
            this.Model.EnableLegacyStyle = false;
            this.CellModels.Add("SchemaItem", new SchemaItemCellModel(this.Model));
            this.CellButtonClicked += new GridCellButtonClickedEventHandler(GridList_CellButtonClicked);
            this.popupControlContainer1 = new Syncfusion.Windows.Forms.PopupControlContainer();
            this.VScrollBehavior = GridScrollbarMode.Automatic;
            this.HScrollBehavior = GridScrollbarMode.Automatic;
            #region ContextMenu


            ArrayList menuItemgrp1 = new ArrayList();
            menuItemgrp1.Add(PivotAnalysis.SR.GetString(PivotAnalysis.SR.MoveUp));
            menuItemgrp1.Add(PivotAnalysis.SR.GetString(PivotAnalysis.SR.MoveDown));
            menuItemgrp1.Add(PivotAnalysis.SR.GetString(PivotAnalysis.SR.MoveToBeginning));
            menuItemgrp1.Add(PivotAnalysis.SR.GetString(PivotAnalysis.SR.MoveToEnd));

            ArrayList menuItemgrp2 = new ArrayList();
            menuItemgrp2.Add(PivotAnalysis.SR.GetString(PivotAnalysis.SR.MoveToRowLabels));
            menuItemgrp2.Add(PivotAnalysis.SR.GetString(PivotAnalysis.SR.MoveToColumnLabels));
            menuItemgrp2.Add(PivotAnalysis.SR.GetString(PivotAnalysis.SR.MoveToReportFilter));
            menuItemgrp2.Add(PivotAnalysis.SR.GetString(PivotAnalysis.SR.MoveToValues));

            cMenu.RenderMode = ToolStripRenderMode.ManagerRenderMode;
            cMenu.Renderer = new ContextMenuRenderer(this);
            Bitmap bmp = FilterBitmaps.GetBitmap("col");
            ToolStripMenuItem[] items = new ToolStripMenuItem[menuItemgrp1.Count];
            for (int i = 0; i < menuItemgrp1.Count; i++)
            {
                items[i] = new ToolStripMenuItem();
                items[i].Text = menuItemgrp1[i].ToString();
                items[i].Name = items[i].Text;
            }

            ToolStripSeparator itemSep = new ToolStripSeparator();
            ToolStripSeparator itemSep2 = new ToolStripSeparator();
            ToolStripMenuItem removeField = new ToolStripMenuItem(PivotAnalysis.SR.GetString(PivotAnalysis.SR.RemoveField));
            removeField.Image = bmp;
            ToolStripMenuItem[] items2 = new ToolStripMenuItem[menuItemgrp2.Count];
            for (int i = 0; i < menuItemgrp1.Count; i++)
            {
                items2[i] = new ToolStripMenuItem();
                items2[i].Text = menuItemgrp2[i].ToString();
                items2[i].Name = items2[i].Text;
                items2[i].Image = bmp;
            }

            cMenu.Items.AddRange(items);
            cMenu.Items.Add(itemSep);
            cMenu.Items.AddRange(items2);
            cMenu.Items.Add(itemSep2);
            cMenu.Items.Add(removeField);
            cMenu.ItemClicked += new ToolStripItemClickedEventHandler(cMenu_ItemClicked);
            #endregion
        }

        #region Event Handlers
        GridStyleInfo contextMenuInvokedCellStyle;
        void GridList_CellButtonClicked(object sender, GridCellButtonClickedEventArgs e)
        {
            if (e.ButtonIndex == 0)
            {
                GridRangeInfo range = GridRangeInfo.Cell(e.RowIndex, e.ColIndex);
                contextMenuInvokedCellStyle = this[e.RowIndex, e.ColIndex];
                contextMenuInvokedCellStyle.Tag = this.Tag;
                QueryContextMenuItems(this.cMenu, contextMenuInvokedCellStyle);
                this.cMenu.Show(PointToScreen(e.Button.Bounds.Location));//Control.MousePosition);
            }
            else if(e.ButtonIndex==1)
            {
                if (!this.Tag.ToString().Equals("PivotFilters"))
                {
                    if (this.RowCount > 1)
                    {
                        contextMenuInvokedCellStyle = this[e.RowIndex, e.ColIndex];
                        
                        dragDropHelper.Remove(this.Tag.ToString(), contextMenuInvokedCellStyle);
                    }
                }
                else
                {
                    this.popupControlContainer1.RightToLeft = this.PivotGridControl.TableControl.IsRightToLeft() ? RightToLeft.Yes : RightToLeft.No;
                    filterHelper = new FilterHelper(this.pivotGridControl.TableControl);
                    PropertyDescriptor descriptor = filterHelper.GetPropertyDescriptor(this[e.RowIndex, e.ColIndex].Text);
                    FilterExpression fItem = this.PivotGridControl.TableControl.PivotFilters.Where(f => f.DimensionName == this[e.RowIndex, e.ColIndex].Text).FirstOrDefault();
                    FilterItemsCollection filteritem = new FilterItemsCollection();
                    FilterDropDown filter = new FilterDropDown(this.PivotGridControl);

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
                    Point location = this.PointToScreen(new Point(locX,locY));
                    if (popupControlContainer1.IsShowing())
                        popupControlContainer1.HidePopup();
                    else
                        popupControlContainer1.ShowPopup(location);
              
                }
            }
        }

        void cMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ContextMenuStrip str = sender as ContextMenuStrip;

            if (contextMenuInvokedCellStyle!=null)
            {
                GridStyleInfo selectedItemstyle = contextMenuInvokedCellStyle;// this[this.CurrentCell.RowIndex, this.CurrentCell.ColIndex];
                this.ApplyAction(selectedItemstyle, this.Tag, e.ClickedItem.Text);
            }
        }
        
#endregion

        internal bool isFieldsGrid = false;

        /// <summary>
        /// Gets either the GridStyleInfo.Description property or the GridStyleInfo.Text property for a particular
        /// row depending upon the value of the isFieldsGrid property. 
        /// </summary>
        /// <param name="index">The 1-based row index.</param>
        /// <returns>A string holding either the Description or the Text for the cell in this row.</returns>
        public string this[int index]
        {
            get
            {
                if (!(index > 0 && index < this.Model.RowCount))
                    throw new IndexOutOfRangeException(this.ToString() + " invalid 'row index'");
                else
                {
                    if (isFieldsGrid)
                        return this[index, 0].Description;
                    else
                        return this[index, 0].Text;
                }

            }
        }

        #region properties
        PivotGridControl pivotGridControl = null;
        internal PivotGridControl PivotGridControl
        {
            get
            {
                return pivotGridControl;
            }
            set
            {
                this.pivotGridControl = value;
                this.GridVisualStyles = this.PivotGridControl.GridVisualStyles;
                SetSelectionColor(this.GridVisualStyles);
                dragDropHelper = new DragDropHelper(this.pivotGridControl);
            }
        }


        PivotSchemaDesigner pivotSchemaDesigner = null;
        internal PivotSchemaDesigner PivotSchemaDesigner
        {
            get
            {
                return pivotSchemaDesigner;
            }
            set
            {
                this.pivotSchemaDesigner = value;
            }
        }

        private Color selectionColor;

        /// <summary>
        /// Gets or sets the Selection color.
        /// </summary>
        public Color SelectionColor
        {
            get { return selectionColor; }
            set { selectionColor = value; }
        }

        private bool hasCheckBox = false;

        /// <summary>
        /// Gets or sets whether this grid has a checkbox displayed in it single cell on each row.
        /// </summary>
        public bool HasCheckBox
        {
            get { return hasCheckBox; }
            set { hasCheckBox = value; }
        }

        private bool hasPaddedRow = true;

        /// <summary>
        /// Gets or sets whether there is a hidden padded row at the top of the grid.
        /// </summary>
        public bool HasPaddedRow
        {
            get { return hasPaddedRow; }
            set { hasPaddedRow = value; }
        }

        #endregion

        #region Helpers

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Delete)
                return true;
            else
                return base.ProcessCmdKey(ref msg, keyData);
        }

        /// <summary>
        /// Oveeridden to manage selections and display bold checked values.
        /// </summary>
        /// <param name="e">The event argument.</param>
        protected override void OnPrepareViewStyleInfo(GridPrepareViewStyleInfoEventArgs e)
        {
            base.OnPrepareViewStyleInfo(e);
            if (e.ColIndex == 1 && e.RowIndex > 0 && e.Style.CellValue != null
                && e.Style.CellType == "CheckBox" && e.Style.Text == "1")
            {
                e.Style.Font.Bold = true;

            }
            if (this.Focused && e.RowIndex == this.CurrentCell.RowIndex)
                e.Style.BackColor = this.SelectionColor;
            ////else
            ////    e.Style.BackColor = this.TableStyle.BackColor;
            if ((this.GridVisualStyles == GridVisualStyles.Office2010Black || this.GridVisualStyles == GridVisualStyles.Metro) && this.Focused && e.RowIndex == this.CurrentCell.RowIndex)
                e.Style.TextColor = Color.White;
            else
                e.Style.TextColor = Color.Black;

            e.Style.Borders.All = GridBorder.Empty;
        }

        /// <summary>
        /// To set the selection color based on the themes applied
        /// </summary>
        /// <param name="vStyles"></param>
        private void SetSelectionColor(GridVisualStyles vStyles)
        {
            switch (vStyles)
            {
                case GridVisualStyles.Office2007Blue:
                case GridVisualStyles.Office2010Blue:
                    this.SelectionColor = Color.FromArgb(153, 204, 255);//(224,235,246);//(153, 204, 255);
                    break;
                
                case GridVisualStyles.Office2007Black:
                case GridVisualStyles.Office2007Silver:
                case GridVisualStyles.Office2010Black:
                case GridVisualStyles.Office2010Silver:
                    this.SelectionColor = Color.FromArgb(180, 180, 180);//(238,238,238);//(180, 180, 180);
                    break;
                
                case GridVisualStyles.Metro:
                    this.SelectionColor = Color.FromArgb(42, 191, 241);
                    break;
            }
        }

       
        #endregion

        #region Drag Drop

        #region Arrow Indicator
        /// <summary>
        /// Creates the arrow indicator which used for indicating the dropping location
        /// </summary>
        /// <returns>bitmap</returns>
        private Bitmap CreateRedArrowIndicatorBitmap(Point location)
        {
            Bitmap bm = null;
            Graphics g = null;
            int colIndex = checkCol;
            int rowIndex = checkRow;
            Bitmap leftBitmap = FilterBitmaps.RedLeftBitmap;
            Bitmap rightBitmap = FilterBitmaps.RedRightBitmap;

            // to avoid the longer width for arrow bitmap, use the width of pivot sub collection grid's instead of table field list
            colWidthOnRedArrowCreate = this.GetColWidth(colIndex);
            if (this.Tag.ToString() == "TableFieldList")
                colWidthOnRedArrowCreate = this.PivotGridControl.pivotSchemaDesigner.pivotColLists.GetColWidth(colIndex);

            rowHeightOnRedArrowCreate = this.DefaultRowHeight;

            Size size = new Size(leftBitmap.Width * 2 + colWidthOnRedArrowCreate - 1, rowHeightOnRedArrowCreate);

            Rectangle bounds = new Rectangle(Point.Empty, size);

            try
            {
                bm = new Bitmap(size.Width, size.Height);
                g = Graphics.FromImage(bm);
                g.FillRectangle(new SolidBrush(Color.Red), new Rectangle(Point.Empty, size));
                g.DrawImageUnscaled(leftBitmap, bm.Width - leftBitmap.Width, 0);
                g.DrawImageUnscaled(rightBitmap, 0, 1);
            }
            finally
            {
                if (g != null)
                    g.Dispose();
            }

            bm.MakeTransparent(Color.Red);
            return bm;
        }

        /// <summary>
        /// opens the created arrow bitmap and intiates the painting in drag operation
        /// </summary>
        internal void OpenRedArrowIndicator(Point location)
        {
            Bitmap bm = CreateRedArrowIndicatorBitmap(location);
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

            Point droppedPoint = this.pivotGridControl.pivotSchemaDesigner.splitter.Panel2.PointToClient(Control.MousePosition);
            Rectangle tableFieldBounds = this.pivotGridControl.pivotSchemaDesigner.gridColumnList.Bounds;
            Point arrowPoint = hiddenPoint; GridRangeInfo p2;

            if (this.pivotGridControl.pivotSchemaDesigner.pivotRowLists.Bounds.Contains(droppedPoint))
            {
                p2 = this.PivotGridControl.pivotSchemaDesigner.pivotRowLists.PointToRangeInfo(Control.MousePosition);
                Rectangle cellRectangle = this.PivotGridControl.pivotSchemaDesigner.pivotRowLists.RangeInfoToRectangle(p2);
                arrowPoint = new Point(cellRectangle.X - FilterBitmaps.RedRightBitmap.Width, cellRectangle.Y + cellRectangle.Height);
                arrowPoint = this.PivotGridControl.pivotSchemaDesigner.pivotRowLists.PointToScreen(arrowPoint);
            }
            if (this.pivotGridControl.pivotSchemaDesigner.pivotColLists.Bounds.Contains(droppedPoint))
            {
                p2 = this.PivotGridControl.pivotSchemaDesigner.pivotColLists.PointToRangeInfo(Control.MousePosition);
                Rectangle cellRectangle = this.PivotGridControl.pivotSchemaDesigner.pivotColLists.RangeInfoToRectangle(p2);
                arrowPoint = new Point(cellRectangle.X - FilterBitmaps.RedRightBitmap.Width, cellRectangle.Y + cellRectangle.Height);
                arrowPoint = this.PivotGridControl.pivotSchemaDesigner.pivotColLists.PointToScreen(arrowPoint);
            }
            if (this.pivotGridControl.pivotSchemaDesigner.pivotCalcLists.Bounds.Contains(droppedPoint))
            {
                p2 = this.PivotGridControl.pivotSchemaDesigner.pivotCalcLists.PointToRangeInfo(Control.MousePosition);
                Rectangle cellRectangle = this.PivotGridControl.pivotSchemaDesigner.pivotCalcLists.RangeInfoToRectangle(p2);
                arrowPoint = new Point(cellRectangle.X - FilterBitmaps.RedRightBitmap.Width, cellRectangle.Y + cellRectangle.Height);
                arrowPoint = this.PivotGridControl.pivotSchemaDesigner.pivotCalcLists.PointToScreen(arrowPoint);
            }
            if (this.pivotGridControl.pivotSchemaDesigner.pivotFilterLists.Bounds.Contains(droppedPoint))
            {
                p2 = this.PivotGridControl.pivotSchemaDesigner.pivotFilterLists.PointToRangeInfo(Control.MousePosition);
                Rectangle cellRectangle = this.PivotGridControl.pivotSchemaDesigner.pivotFilterLists.RangeInfoToRectangle(p2);
                arrowPoint = new Point(cellRectangle.X - FilterBitmaps.RedRightBitmap.Width, cellRectangle.Y + cellRectangle.Height);
                arrowPoint = this.PivotGridControl.pivotSchemaDesigner.pivotFilterLists.PointToScreen(arrowPoint);
            }

            redArrowIndicatorDragHelper.DoDrag(arrowPoint, DragDropEffects.Copy);
            redArrowIndicatorDragHelper.DragWindow.ShowWindowTopMost();
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
                wasDragging = false;
            }
        }

        #endregion

        #region DragHeader

        /// <summary>
        /// Creates a header bitmap which used in dragwindow 
        /// </summary>
        private Bitmap CreateHeaderBitmap(GridControlBase grid, int rowIndex, int colIndex)
        {
            Graphics g = null;
            Size size = new Size(/*grid.GetColWidth(colIndex)*/90, grid.GetRowHeight(rowIndex));
            Rectangle bounds = new Rectangle(Point.Empty, size);
            string buffer = style.Text;
            if (style.CellType == "CheckBox")
            {
                style.Text = style.Description;
            }
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
            if (style.CellType == "CheckBox")
                style.Text = buffer;
            if (this.Model.Options.GridVisualStyles == Forms.GridVisualStyles.Metro)
            {
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
                            bm.SetPixel(X, Y, System.Drawing.Color.LightGray);
                        }
                    }
                }
            }
            return bm;
        }

        /// <summary>
        /// Opens the created drag window and initiates the painting in drag operation
        /// </summary>
        internal void OpenDragHeader()
        {
            Bitmap bm;
            bm = CreateHeaderBitmap(this, sourceRow, sourceCol); // source row col 
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
            //return new Point(srcLocation.X + pt.X - origin.X, srcLocation.Y + pt.Y - origin.Y);
            //return new Point(srcLocation.X + pt.X /*- origin.X*/, srcLocation.Y + pt.Y /*- origin.Y*/);
            return new Point(pt.X, pt.Y);

        }
        #endregion

        #region Overrides
        
        protected override void OnMouseEnter(EventArgs e)
        {
            if (isMouseDown)
            {
            }
            base.OnMouseEnter(e);
        }
       
        protected override void OnMouseUp(MouseEventArgs e)
        {
            isMouseDown = false;
            started = false;
            CloseDragHeader();

            if (wasDragging)
            {
                CloseRedArrowIndicator();
                wasDragging = false;
                
                Point droppedPoint = this.pivotGridControl.pivotSchemaDesigner.splitter.Panel2.PointToClient(Control.MousePosition);
                Rectangle tableFieldBounds = this.pivotGridControl.pivotSchemaDesigner.gridColumnList.Bounds;
               
                if (this.pivotGridControl.pivotSchemaDesigner.pivotRowLists.Bounds.Contains(droppedPoint))
                {
                    if (GetContainerControl().ActiveControl is GridList)
                    {
                        GridList list = (GridList)GetContainerControl().ActiveControl;
                        style.Tag = list.Tag;
                        dragDropHelper.DropinPivotRows(style, GetControlBase(style.Tag.ToString()));
                    }
                }

                if (this.pivotGridControl.pivotSchemaDesigner.pivotColLists.Bounds.Contains(droppedPoint))
                {
                    if (GetContainerControl().ActiveControl is GridList)
                    {
                        GridList list = (GridList)GetContainerControl().ActiveControl;
                        style.Tag = list.Tag;
                        dragDropHelper.DropinPivotColumns(style, GetControlBase(style.Tag.ToString()));
                    }
                }
                if (this.pivotGridControl.pivotSchemaDesigner.pivotCalcLists.Bounds.Contains(droppedPoint))
                {
                    if (GetContainerControl().ActiveControl is GridList)
                    {
                        GridList list = (GridList)GetContainerControl().ActiveControl;
                        style.Tag = list.Tag;
                        dragDropHelper.DropinPivotCalculations(style, GetControlBase(style.Tag.ToString()));
                    }
                }
                if (this.pivotGridControl.pivotSchemaDesigner.pivotFilterLists.Bounds.Contains(droppedPoint))
                {
                    if (GetContainerControl().ActiveControl is GridList)
                    {
                        GridList list = (GridList)GetContainerControl().ActiveControl;
                        style.Tag = list.Tag;
                        dragDropHelper.DropinPivotFilters(style, GetControlBase(style.Tag.ToString()));
                    }
                }
                if (this.pivotGridControl.pivotSchemaDesigner.RectangleToScreen(tableFieldBounds).Contains(Control.MousePosition))
                {
                    if (GetContainerControl().ActiveControl is GridList)
                    {
                        GridList list = (GridList)GetContainerControl().ActiveControl;
                        style.Tag = list.Tag;
                        dragDropHelper.DropInTableFieldList(style, GetControlBase(style.Tag.ToString()));
                    }
                }
                Point p = Control.MousePosition;
                Rectangle groupbar = new Rectangle(this.PivotGridControl.TableControl.GroupDropArea.Location.X, this.PivotGridControl.TableControl.GroupDropArea.Location.Y, this.PivotGridControl.TableControl.GroupDropArea.Size.Width, this.PivotGridControl.TableControl.GroupDropArea.Size.Height);
                Rectangle groupbarBounds = this.pivotGridControl.RectangleToScreen(groupbar);

                if (groupbarBounds.Contains(Control.MousePosition))
                {
                    GridRangeInfo dropRange = this.pivotGridControl.TableControl.GroupDropArea.PointToRangeInfo(this.pivotGridControl.TableControl.GroupDropArea.PointToClient(PointToScreen(e.Location)));
                    GridStyleInfo styleee = this.pivotGridControl.TableControl.GroupDropArea[dropRange.Bottom, dropRange.Left];


                    int meanrange = (2 * this.pivotGridControl.PivotCalculations.Count) + 1;
                    int colrange = meanrange + ((2 * this.pivotGridControl.PivotColumns.Count) - 1);
                    if (dropRange.Left > meanrange && dropRange.Left <= colrange)
                    {
                        if (dropRange.Left % 2 == 0)
                        {
                            style.Tag = this.Tag;
                            dragDropHelper.DropinPivotColumns(style, GetControlBase(style.Tag.ToString()));
                        }
                    }
                    else if (dropRange.Left < meanrange)
                    {
                        if (dropRange.Left % 2 == 0)
                        {
                            style.Tag = this.Tag;
                            dragDropHelper.DropinPivotCalculations(style, GetControlBase(style.Tag.ToString()));
                        }
                    }
                }
                Rectangle topLeftCell3 = new Rectangle(this.PivotGridControl.TableControl.Bounds.Location.X, this.PivotGridControl.TableControl.Bounds.Location.Y, this.PivotGridControl.TableControl.RowGroupDropArea.Width, this.PivotGridControl.TableControl.RowGroupDropArea.Height);
                if (this.PivotGridControl.RectangleToScreen(topLeftCell3).Contains(PointToScreen(e.Location)))
                {
                    GridRangeInfo dropRange = this.pivotGridControl.TableControl.RowGroupDropArea.PointToRangeInfo(this.pivotGridControl.TableControl.RowGroupDropArea.PointToClient(PointToScreen(e.Location)));
                    GridStyleInfo styleee2 = this.pivotGridControl.TableControl.RowGroupDropArea[dropRange.Bottom, dropRange.Left];
                    if (dropRange.Left % 2 == 0)
                    {
                        style.Tag = this.Tag;
                        dragDropHelper.DropinPivotRows(style, GetControlBase(style.Tag.ToString()));
                    }
                }
                if (this.PivotGridControl.RectangleToScreen(this.PivotGridControl.TableControl.FilterArea.Bounds).Contains(PointToScreen(e.Location)))
                {
                    style.Tag = this.Tag;
                    dragDropHelper.DropinPivotFilters(style, GetControlBase(style.Tag.ToString()));
                }
                this.PivotGridControl.ResumeLayout(true);
            }
           
            this.ScrollCellInView(GridRangeInfo.Row(this.RowCount));
            base.OnMouseUp(e);
        }

        bool started = false;
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (checkRow < 0 || checkCol < 0)
                isMouseDown = false;
            if (isMouseDown)
            {
                if (!started)
                {
                    GridRangeInfo range = this.PointToRangeInfo(e.Location);
                    style = this[range.Top, range.Left];
                    sourceRow = range.Top;
                    sourceCol = range.Left;
                    GridRangeInfo testRange = GridRangeInfo.Rows(1,this.RowCount);
                    
                    if ((this.Tag.ToString() == "PivotFilters" || this.RowCount > 1) && this.RectangleToScreen(this.RangeInfoToRectangle(testRange)).Contains(Control.MousePosition))
                    {
                        OpenDragHeader();
                        OpenRedArrowIndicator(e.Location);
                        started = true;
                    }
                }
                if (started)
                {
                    if (this.Tag.ToString() == "PivotFilters" || this.RowCount > 1)
                        UpdateDragHeader();
                    UpdateRedArrowIndicatorBitmap(e.Location);
                }
            }
        }
       
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                isMouseDown = true;
            if (e.Clicks == 2)
                isMouseDown = false;
            this.PointToRowCol(e.Location, out checkRow, out checkCol);
            base.OnMouseDown(e);
            locX = e.X;
            locY = e.Y;
           
        }

        protected override void OnQueryColWidth(GridRowColSizeEventArgs e)
        {
            if (e.Index == 1)
            {
                e.Size = this.ClientSize.Width;
                e.Handled = true;
            }
            base.OnQueryColWidth(e);
        }

        protected override void OnCellDoubleClick(GridCellClickEventArgs e)
        {

            if (this.Tag.ToString().Equals("PivotCalculations"))
            {
                PivotCompInfo compForm = new PivotCompInfo();
                compForm.WireCompInfo(this[e.RowIndex, e.ColIndex], this.PivotGridControl);
                compForm.ShowDialog();
            }
            if (isMouseDown)
                isMouseDown = false;

            //base.OnCellDoubleClick(e);
        }
       
        #endregion

        #endregion

        #region Helper Functions

        /// <summary>
        /// Updates the items of the context menu items
        /// </summary>
        private ContextMenuStrip QueryContextMenuItems(ContextMenuStrip cMenu, GridStyleInfo sty)
        {
            PivotItem pivotItem;
            PivotComputationInfo compItem; int index;
            FilterExpression exp;
            foreach (ToolStripItem item in cMenu.Items)
            {
                if (!item.Enabled)
                    item.Enabled = true;
               
            }
            switch (Tag.ToString())
            {
                case "PivotRows":
                    if (this.pivotGridControl.pivotSchemaDesigner.pivotRowCollList.TryGetValue(sty.Text, out pivotItem))
                    {
                        index = this.PivotGridControl.PivotRows.IndexOf(pivotItem);
                        cMenu.Items[PivotAnalysis.SR.GetString(PivotAnalysis.SR.MoveToRowLabels)].Enabled = false;
                        if (index == 0)
                        {
                            cMenu.Items[PivotAnalysis.SR.GetString(PivotAnalysis.SR.MoveUp)].Enabled = false;
                            cMenu.Items[PivotAnalysis.SR.GetString(PivotAnalysis.SR.MoveToBeginning)].Enabled = false;
                        }
                        else if (index == this.PivotGridControl.PivotRows.Count - 1)
                        {
                            cMenu.Items[PivotAnalysis.SR.GetString(PivotAnalysis.SR.MoveDown)].Enabled = false;
                            cMenu.Items[PivotAnalysis.SR.GetString(PivotAnalysis.SR.MoveToBeginning)].Enabled = false;
                        }

                        if (this.PivotGridControl.PivotRows.Count == 1)
                        {
                            foreach (ToolStripItem item in cMenu.Items)
                            {
                                item.Enabled = false;
                            }
                        }

                    }
                    break;
                case "PivotColumns":
                    if (this.pivotGridControl.pivotSchemaDesigner.pivotColumnCollList.TryGetValue(sty.Text, out pivotItem))
                    {
                        index = this.PivotGridControl.PivotColumns.IndexOf(pivotItem);
                        cMenu.Items[PivotAnalysis.SR.GetString(PivotAnalysis.SR.MoveToColumnLabels)].Enabled = false;
                        if (index == 0)
                        {
                            cMenu.Items[PivotAnalysis.SR.GetString(PivotAnalysis.SR.MoveUp)].Enabled = false;
                            cMenu.Items[PivotAnalysis.SR.GetString(PivotAnalysis.SR.MoveToBeginning)].Enabled = false;
                        }
                        else if (index == this.PivotGridControl.PivotColumns.Count - 1)
                        {
                            cMenu.Items[PivotAnalysis.SR.GetString(PivotAnalysis.SR.MoveDown)].Enabled = false;
                            cMenu.Items[PivotAnalysis.SR.GetString(PivotAnalysis.SR.MoveToEnd)].Enabled = false;
                        }

                        if (this.PivotGridControl.PivotColumns.Count == 1)
                        {
                            foreach (ToolStripItem item in cMenu.Items)
                            {
                                item.Enabled = false;
                            }
                        }
                    }
                    break;
                case "PivotFilters":
                    if (this.pivotGridControl.pivotSchemaDesigner.pivotFilterCollList.TryGetValue(sty.Text, out exp))
                    {
                        index = this.PivotGridControl.Filters.IndexOf(exp);
                        cMenu.Items[PivotAnalysis.SR.GetString(PivotAnalysis.SR.MoveToReportFilter)].Enabled = false;
                        cMenu.Items[PivotAnalysis.SR.GetString(PivotAnalysis.SR.MoveUp)].Visible = false;
                        cMenu.Items[PivotAnalysis.SR.GetString(PivotAnalysis.SR.MoveToBeginning)].Visible = false;

                        cMenu.Items[PivotAnalysis.SR.GetString(PivotAnalysis.SR.MoveDown)].Visible = false;
                        cMenu.Items[PivotAnalysis.SR.GetString(PivotAnalysis.SR.MoveToEnd)].Visible = false;


                    }
                    break;
                case "PivotCalculations":
                    if (this.pivotGridControl.pivotSchemaDesigner.pivotCalcCollList.TryGetValue(sty.Text, out compItem))
                    {
                        index = this.PivotGridControl.PivotCalculations.IndexOf(compItem);
                        cMenu.Items[PivotAnalysis.SR.GetString(PivotAnalysis.SR.MoveToValues)].Enabled = false;
                        if (index == 0)
                        {
                            cMenu.Items[PivotAnalysis.SR.GetString(PivotAnalysis.SR.MoveUp)].Enabled = false;
                            cMenu.Items[PivotAnalysis.SR.GetString(PivotAnalysis.SR.MoveToBeginning)].Enabled = false;
                        }
                        else if (index == this.PivotGridControl.PivotCalculations.Count - 1)
                        {
                            cMenu.Items[PivotAnalysis.SR.GetString(PivotAnalysis.SR.MoveDown)].Enabled = false;
                            cMenu.Items[PivotAnalysis.SR.GetString(PivotAnalysis.SR.MoveToEnd)].Enabled = false;
                        }

                        if (this.PivotGridControl.PivotCalculations.Count == 1)
                        {
                            foreach (ToolStripItem item in cMenu.Items)
                            {
                                item.Enabled = false;
                            }
                        }
                    }
                    break;
            }
            return cMenu;
        }

        /// <summary>
        /// Applies the corresponding action triggered by the context menu
        /// </summary>
        public void ApplyAction(GridStyleInfo selectedItemStyle, object tag, string action)
        {
            if (action == PivotAnalysis.SR.GetString(PivotAnalysis.SR.MoveUp))
                dragDropHelper.NavigateUp(tag.ToString(), selectedItemStyle);
            else if (action == PivotAnalysis.SR.GetString(PivotAnalysis.SR.MoveDown))
                dragDropHelper.NavigateDown(tag.ToString(), selectedItemStyle);
            else if (action == PivotAnalysis.SR.GetString(PivotAnalysis.SR.MoveToBeginning))
                dragDropHelper.NavigateBeginning(tag.ToString(), selectedItemStyle);
            else if (action == PivotAnalysis.SR.GetString(PivotAnalysis.SR.MoveToEnd))
                dragDropHelper.NavigateEnd(tag.ToString(), selectedItemStyle);
            else if (action == PivotAnalysis.SR.GetString(PivotAnalysis.SR.RemoveField))
                dragDropHelper.Remove(tag.ToString(), selectedItemStyle);
            else if (action == PivotAnalysis.SR.GetString(PivotAnalysis.SR.MoveToRowLabels))
                dragDropHelper.DropinPivotRows(selectedItemStyle, GetControlBase(tag.ToString()));
            else if (action == PivotAnalysis.SR.GetString(PivotAnalysis.SR.MoveToColumnLabels))
                dragDropHelper.DropinPivotColumns(selectedItemStyle, GetControlBase(tag.ToString()));
            else if (action == PivotAnalysis.SR.GetString(PivotAnalysis.SR.MoveToReportFilter))
                dragDropHelper.DropinPivotFilters(selectedItemStyle, GetControlBase(tag.ToString()));
            else if (action == PivotAnalysis.SR.GetString(PivotAnalysis.SR.MoveToValues))
                dragDropHelper.DropinPivotCalculations(selectedItemStyle, GetControlBase(tag.ToString()));

        }

        /// <summary>
        /// Returns the control base for the corresponding tags
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        private GridControlBase GetControlBase(string tag)
        {
            GridControlBase grid = null;
            switch (tag)
            {
                case "PivotFilters":
                    grid = this.PivotGridControl.TableControl.FilterArea;
                    break;
                case "PivotColumns":
                    grid = this.PivotGridControl.TableControl.GroupDropArea;
                    break;
                case "PivotRows":
                    grid = this.PivotGridControl.TableControl.RowGroupDropArea;
                    break;
                case "PivotCalculations":
                    grid = this.PivotGridControl.TableControl.GroupDropArea;
                    break;
                default:
                    return null;
            }
            return grid;
        }

        #endregion
    }

    #region ContextMenuRenderer
    public class ContextMenuRenderer : ToolStripProfessionalRenderer
    {
        GridControlBase Grid;
        public ContextMenuRenderer(GridControlBase grid)
        {
            this.Grid = grid;
        }
        protected override void OnRenderItemImage(ToolStripItemImageRenderEventArgs e)
        {
            Bitmap bmp;
            if (this.Grid.Model.Options.GridVisualStyles == GridVisualStyles.Office2010Black || this.Grid.Model.Options.GridVisualStyles == GridVisualStyles.Metro)
            {
                if (e.Item.Text == PivotAnalysis.SR.GetString(PivotAnalysis.SR.MoveToRowLabels))
                    bmp = FilterBitmaps.GetBitmap("row");
                else if (e.Item.Text == PivotAnalysis.SR.GetString(PivotAnalysis.SR.MoveToColumnLabels))
                    bmp = FilterBitmaps.GetBitmap("col");
                else if (e.Item.Text == PivotAnalysis.SR.GetString(PivotAnalysis.SR.MoveToReportFilter))
                    bmp = FilterBitmaps.GetBitmap("filt");
                else if (e.Item.Text == PivotAnalysis.SR.GetString(PivotAnalysis.SR.MoveToValues))
                    bmp = FilterBitmaps.GetBitmap("sum");
                else if (e.Item.Text == PivotAnalysis.SR.GetString(PivotAnalysis.SR.RemoveField))
                    bmp = FilterBitmaps.GetBitmap("clear");
                else bmp = FilterBitmaps.GetBitmap("up_white");
                e.Item.Image = bmp;
            }

            else
            {
                if (e.Item.Text == PivotAnalysis.SR.GetString(PivotAnalysis.SR.MoveToRowLabels))
                    bmp = FilterBitmaps.GetBitmap("row");
                else if (e.Item.Text == PivotAnalysis.SR.GetString(PivotAnalysis.SR.MoveToColumnLabels))
                    bmp = FilterBitmaps.GetBitmap("col");
                else if (e.Item.Text == PivotAnalysis.SR.GetString(PivotAnalysis.SR.MoveToReportFilter))
                    bmp = FilterBitmaps.GetBitmap("filt");
                else if (e.Item.Text == PivotAnalysis.SR.GetString(PivotAnalysis.SR.MoveToValues))
                    bmp = FilterBitmaps.GetBitmap("sum");
                else if (e.Item.Text == PivotAnalysis.SR.GetString(PivotAnalysis.SR.RemoveField))
                    bmp = FilterBitmaps.GetBitmap("clear");
                else
                    bmp = FilterBitmaps.GetBitmap("up");
            }
            e.Item.Image = bmp;
            base.OnRenderItemImage(e);
        }

        protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
        {
            Rectangle marginRect = e.AffectedBounds;
            SolidBrush backBrush; LinearGradientBrush brush;
            ColorBlend cb = new ColorBlend();
            backBrush = new SolidBrush(SystemColors.Control);
            brush = new LinearGradientBrush(marginRect, Color.FromArgb(253, 253, 253), Color.FromArgb(253, 253, 253), 0f, true);
            using (brush)
                e.Graphics.FillRectangle(brush, marginRect);
        }

    }
    #endregion
}
