//-------------------------------------------------------------------------------------------------
// <copyright file="GridSortColumnHeaderCellRenderer.cs" company="syncfusion">
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
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.IO;
using Syncfusion.Windows.Forms.Grid;
using Syncfusion.Drawing;
using Syncfusion.PivotAnalysis.Base;

namespace Syncfusion.Windows.Forms.PivotAnalysis
{
    
    public class PivotGridSortColumnHeaderCellRenderer : GridHeaderCellRenderer
    {
        private GridCellButton pushButton, removeButton;
        private GridRangeInfo hoverRange = GridRangeInfo.Empty;
        GridRangeInfo range = null;
        bool mouseDown = false;
        bool inMouseDownRange = false;
        bool drawPressed = false;
        ThemedHeaderDrawing.HeaderState state = ThemedHeaderDrawing.HeaderState.Normal;
        private GridRangeInfo mouseDownRange = GridRangeInfo.Empty;
        static readonly BrushInfo defaultInterior1 = new BrushInfo(GradientStyle.Vertical, Color.FromArgb(203, 199, 184), Color.FromArgb(238, 234, 216));
        private PivotGridControl baseGrid;
        private GridControlBase gridBase;
        private Rectangle r1, r2;
        private int sortIconSize = 12;

        public PivotGridSortColumnHeaderCellRenderer(GridControlBase grid, GridCellModelBase cellModel)
            : base(grid, cellModel)
        {
            if (grid is GroupBar)
                baseGrid = (grid as GroupBar).GridControl;
            else if (grid is RowGroupBar)
                baseGrid = (grid as RowGroupBar).GridControl;
            else if (Grid is FilterBar)
                baseGrid = (grid as FilterBar).GridControl;

            this.gridBase = grid;
            AddButton(pushButton = new GridCellButton(this));
            if (gridBase is FilterBar)
            {
                AddButton(removeButton = new GridCellButton(this));
                removeButton.Text = "clear";
            }
        }

        Pen pen = new Pen(Color.Red);
        protected override void OnDraw(Graphics g, Rectangle clientRectangle, int rowIndex, int colIndex, GridStyleInfo style)
        {
            Color color = SystemColors.Window;
            switch (this.baseGrid.GridVisualStyles)
            {
                case GridVisualStyles.Office2007Blue:
                case GridVisualStyles.Office2010Blue:
                    color = Color.LightBlue;
                    break;
                
                case GridVisualStyles.Office2010Black:
                    color = Color.LightGray;
                    break;

                case GridVisualStyles.Office2007Silver:
                case GridVisualStyles.Office2010Silver:
                case GridVisualStyles.Office2007Black:
                    color = Color.Silver;
                    break;

            }
            style.Borders.All = new GridBorder(GridBorderStyle.Solid, color);

            base.OnDraw(g, clientRectangle, rowIndex, colIndex, style);
        }
        public override void OnPrepareViewStyleInfo(GridPrepareViewStyleInfoEventArgs e)
        {
            e.Style.Borders.All = new GridBorder(GridBorderStyle.Solid, Color.Gray, GridBorderWeight.Thin);
            //base.OnPrepareViewStyleInfo(e);
        }
        protected override System.Windows.Forms.Cursor OnGetCursor(int rowIndex, int colIndex)
        {
            // if over cell, return HandPointerCursor otherwise NoCursor...

            if (this.Grid.RectangleToScreen(r1).Contains(Control.MousePosition) || this.Grid.RectangleToScreen(r2).Contains(Control.MousePosition))
            {
                return Cursors.Hand;
            }
            else
                return base.OnGetCursor(rowIndex, colIndex);
        }
        /// <override/>
        protected override void OnDrawDisplayText(Graphics g, Rectangle textRectangle, int rowIndex, int colIndex, GridStyleInfo style)
        {
            style.Trimming = StringTrimming.EllipsisWord;
            if (Grid.Model.Options.GridVisualStyles == GridVisualStyles.Metro)
            {
                style.Font.Size = 9f;
                style.Font.Bold = true;
                style.Borders.All = new GridBorder(GridBorderStyle.Solid, Color.Gray, GridBorderWeight.Thin);
            }
            // No arrow needed when printing.
            object tag = style.Tag;
            
            if (!baseGrid.AllowSorting)
                tag = null;

            if (Grid.PrintingMode || !(tag is ListSortDirection))
            {
                tag = null;
            }

            ListSortDirection listSortDirection = ListSortDirection.Ascending;
            int margin = sortIconSize + 18;//18 represents Filter buttonWidth....
            Rectangle clientRect = this.Grid.RangeInfoToRectangle(GridRangeInfo.Cell(rowIndex, colIndex));
            if (tag != null)
            {
                listSortDirection = (ListSortDirection)tag;
            }

            bool isTextRightToLeft = (style.RightToLeft == RightToLeft.Inherit && Grid.IsRightToLeft()) || style.RightToLeft == RightToLeft.Yes;
            if (isTextRightToLeft)
            {
                GridUtil.OffsetLeft(ref textRectangle, margin);
            }
            else
            {
                textRectangle.Width -= margin;
            }

            base.OnDrawDisplayText(g, textRectangle, rowIndex, colIndex, style);
            
            if (tag != null)
            {
                Rectangle rect;
                if (isTextRightToLeft)
                {
                    rect = new Rectangle(textRectangle.Left - margin, textRectangle.Y, 10, textRectangle.Height);
                }
                else
                {
                    rect = new Rectangle(clientRect.Right - sortIconSize, textRectangle.Y, 10, textRectangle.Height);
                }

                rect = GridUtil.CenterInRect(rect, new Size(8, 8));

                Brush brush = null;
                Pen pen1 = null;

                this.Grid.Model.Options.GridVisualStylesDrawing.GetSortIconBrush(out brush, out pen1);

                ////g.FillRectangle(brush, rect);
                int i2 = Math.Max(0, (rect.Height - 3) / 2);
                rect.Inflate(-i2, -i2);
                ////Pen pen1 = new Pen(SystemColors.WindowFrame);
                Pen pen2 = new Pen(SystemColors.Control);
                GridTriangleDirection triangleDirection = listSortDirection == ListSortDirection.Ascending ? GridTriangleDirection.Up : GridTriangleDirection.Down;
                GridPaintTriangle.Paint(g, rect, triangleDirection, brush, pen1, true);
                pen1.Dispose();
                pen2.Dispose();
                brush.Dispose();
            }
        }
        protected override void OnMouseDown(int rowIndex, int colIndex, MouseEventArgs e)
        {
            GridRangeInfo range = GridRangeInfo.Cell(rowIndex, colIndex);
            if (!range.IsEmpty)
            {
                Grid.Model.GetSpannedRangeInfo(range.Top, range.Left, out this.mouseDownRange);
                GridStyleInfo style = this.Grid.Model[range.Top, range.Left];
                if (!style.Clickable || this.Grid.CellRenderers[style.CellType] != this)
                {
                    this.mouseDownRange = GridRangeInfo.Empty;
                }
            }

            this.mouseDown = !mouseDownRange.IsEmpty;
            this.hoverRange = GridRangeInfo.Empty;
            if (this.mouseDown)
            {
                this.inMouseDownRange = true;
                this.Grid.InvalidateRange(this.mouseDownRange);
            }
            base.OnMouseDown(rowIndex, colIndex, e);
        }

        protected override void OnMouseUp(int rowIndex, int colIndex, MouseEventArgs e)
        {
            if (this.mouseDown)
            {
                this.mouseDown = false;
                this.Grid.InvalidateRange(this.mouseDownRange);
                this.mouseDownRange = GridRangeInfo.Empty;
            }

            base.OnMouseUp(rowIndex, colIndex, e);
        }
        /// <override/>
        protected override void OnClick(int rowIndex, int colIndex, MouseEventArgs e)
        {
            // Suppress click event - don't move current cell.
        }
        protected override void OnDrawCellButton(GridCellButton button, Graphics g, int rowIndex, int colIndex, bool bActive, GridStyleInfo style)
        {

            if (this.gridBase is FilterBar || baseGrid.AllowFiltering)
            {
                //button.Draw(g, rowIndex, colIndex, false, style);
                Point ptOffset = new Point(1, 1); ;// Point.Empty;
                Bitmap bm = null;
                GridProperties propertyObject = Grid.Model.Properties;
                if (!(this.gridBase is FilterBar))
                {
                    if (baseGrid.GridVisualStyles == GridVisualStyles.Office2010Black)
                    {
                        if (!FilterDropDown.FilterDimensions.Contains(style.Text))
                            bm = FilterBitmaps.GetBitmap("sch_filter_white");
                        else
                            bm = FilterBitmaps.GetBitmap("filtered_white");
                    }
                    else
                    {
                        if (!FilterDropDown.FilterDimensions.Contains(style.Text))
                            bm = FilterBitmaps.GetBitmap("sch_filter");
                        else
                            bm = FilterBitmaps.GetBitmap("filtered_gray");
                    }

                    if (this.baseGrid.GridVisualStyles == GridVisualStyles.Metro)
                    {
                        Color shadow = SystemColors.ControlDarkDark;
                        if (IsHeaderHot(rowIndex, colIndex, style))
                        {
                            if (!FilterDropDown.FilterDimensions.Contains(style.Text))
                                bm = FilterBitmaps.GetBitmap("sch_filter_white");
                            else
                                bm = FilterBitmaps.GetBitmap("filtered_white");
                        }
                        else
                            if (IsHeaderPressed(rowIndex, colIndex, style))
                            {
                                state = ThemedHeaderDrawing.HeaderState.Pressed;
                                if (!FilterDropDown.FilterDimensions.Contains(style.Text))
                                    bm = FilterBitmaps.GetBitmap("sch_filter_white");
                                else
                                    bm = FilterBitmaps.GetBitmap("filtered_white");
                            }
                    }
                    else if (this.baseGrid.GridVisualStyles == GridVisualStyles.Office2010Black)
                    {
                        Color shadow = SystemColors.ControlDarkDark;
                        if (IsHeaderHot(rowIndex, colIndex, style))
                        {
                            if (!FilterDropDown.FilterDimensions.Contains(style.Text))
                                bm = FilterBitmaps.GetBitmap("sch_filter");
                            else
                                bm = FilterBitmaps.GetBitmap("filtered_gray");
                        }
                        else
                            if (IsHeaderPressed(rowIndex, colIndex, style))
                            {
                                state = ThemedHeaderDrawing.HeaderState.Pressed;
                                if (!FilterDropDown.FilterDimensions.Contains(style.Text))
                                    bm = FilterBitmaps.GetBitmap("sch_filter");
                                else
                                    bm = FilterBitmaps.GetBitmap("filtered_gray");
                            }
                    }

                    FilterBitmaps.IconPainter.PaintIcon(g, button.Bounds, ptOffset, bm, Color.Blue);
                    button.Text = "";
                }
                else
                {
                    if (button.Text == "clear")
                    {
                        if (baseGrid.GridVisualStyles == GridVisualStyles.Office2010Black)
                        {
                            if (IsHeaderHot(rowIndex, colIndex, style) || (IsHeaderPressed(rowIndex, colIndex, style)))
                                bm = FilterBitmaps.GetBitmap("clear");
                            else
                                bm = FilterBitmaps.GetBitmap("clear_white");
                        }
                        else if (this.baseGrid.GridVisualStyles == GridVisualStyles.Metro)
                        {
                            if (IsHeaderHot(rowIndex, colIndex, style) || ((IsHeaderPressed(rowIndex, colIndex, style))))
                                bm = FilterBitmaps.GetBitmap("clear_white");
                            else
                                bm = FilterBitmaps.GetBitmap("clear");
                        }
                        else
                            bm = FilterBitmaps.GetBitmap("clear");
                    }
                    else
                    {
                        if (baseGrid.GridVisualStyles == GridVisualStyles.Office2010Black)
                        {
                            if (!FilterDropDown.FilterDimensions.Contains(style.Text))
                                bm = FilterBitmaps.GetBitmap("sch_filter_white");
                            else
                                bm = FilterBitmaps.GetBitmap("filtered_white");
                        }
                        else
                        {
                            if (!FilterDropDown.FilterDimensions.Contains(style.Text))
                                bm = FilterBitmaps.GetBitmap("sch_filter");
                            else
                                bm = FilterBitmaps.GetBitmap("filtered_gray");
                        }
   
                        if (this.baseGrid.GridVisualStyles == GridVisualStyles.Metro)
                        {
                            if (IsHeaderHot(rowIndex, colIndex, style))
                            {
                                if (!FilterDropDown.FilterDimensions.Contains(style.Text))
                                    bm = FilterBitmaps.GetBitmap("sch_filter_white");
                                else
                                    bm = FilterBitmaps.GetBitmap("filtered_white");
                                state = ThemedHeaderDrawing.HeaderState.Hot;
                            }
                            else
                                if (IsHeaderPressed(rowIndex, colIndex, style))
                                {
                                    if (!FilterDropDown.FilterDimensions.Contains(style.Text))
                                        bm = FilterBitmaps.GetBitmap("sch_filter_white");
                                    else
                                        bm = FilterBitmaps.GetBitmap("filtered_white");
                                    state = ThemedHeaderDrawing.HeaderState.Hot;
                                }
                        }
                        else if (this.baseGrid.GridVisualStyles == GridVisualStyles.Office2010Black)
                        {
                            if (IsHeaderHot(rowIndex, colIndex, style))
                            {
                                if (!FilterDropDown.FilterDimensions.Contains(style.Text))
                                    bm = FilterBitmaps.GetBitmap("sch_filter");
                                else
                                    bm = FilterBitmaps.GetBitmap("filtered_white");
                                state = ThemedHeaderDrawing.HeaderState.Hot;
                            }
                            else
                                if (IsHeaderPressed(rowIndex,colIndex,style))
                                {
                                    if (!FilterDropDown.FilterDimensions.Contains(style.Text))
                                        bm = FilterBitmaps.GetBitmap("sch_filter");
                                    else
                                        bm = FilterBitmaps.GetBitmap("filtered_gray");
                                    state = ThemedHeaderDrawing.HeaderState.Hot;
                                }
                        }
                        FilterBitmaps.IconPainter.PaintIcon(g, button.Bounds, ptOffset, bm, Color.Blue);
                    }

                }
                FilterBitmaps.IconPainter.PaintIcon(g, button.Bounds, ptOffset, bm, Color.Blue);
                Rectangle faceRect = button.Bounds;
                faceRect.Inflate(-2, -1);
            }
        }

        private bool IsHeaderHot(int rowIndex, int colIndex, GridStyleInfo style)
        {
            GridProperties propertyObject = Grid.Model.Properties;          
            {
                if (style.CellAppearance == GridCellAppearance.Flat && propertyObject.Buttons3D)
                {
                    if (!Grid.PrintingMode)
                    {
                        GridRangeInfo cellRange = GridRangeInfo.Cell(rowIndex, colIndex);

                        if (this.hoverRange.Contains(cellRange))
                            return true;
                    }
                }
            }
            return false;
        }

        private bool IsHeaderPressed(int rowIndex, int colIndex, GridStyleInfo style)
        {
            GridProperties propertyObject = Grid.Model.Properties;
            {
                if (style.CellAppearance == GridCellAppearance.Flat && propertyObject.Buttons3D)
                {
                    if (!Grid.PrintingMode)
                    {
                        GridRangeInfo cellRange = GridRangeInfo.Cell(rowIndex, colIndex);

                        if (drawPressed || (this.inMouseDownRange && mouseDownRange.Contains(cellRange)))
                            return true;
                    }
                }
            }
            return false;
        }
        protected override void OnMouseHoverEnter(int rowIndex, int colIndex)
        {
            base.OnMouseHoverEnter(rowIndex, colIndex);
        }
        protected override void OnMouseHover(int rowIndex, int colIndex, MouseEventArgs e)
        {
            bool isInvalidated = false;
            range = GridRangeInfo.Cell(rowIndex, colIndex);
            if (!range.IsEmpty)
            {
                Grid.Model.GetSpannedRangeInfo(range.Top, range.Left, out range);
            }

            GridStyleInfo style = this.Grid.Model[range.Top, range.Left];
            if (!style.Clickable || this.Grid.CellRenderers[style.CellType] != this)
            {
                range = GridRangeInfo.Empty;
            }

            if (!mouseDown)
            {
                if (IntelliMouseDragScroll.ActiveIntelliMouseDragScroll != null)
                {
                    range = GridRangeInfo.Empty;
                }

                if (!hoverRange.Equals(range))
                {
                    if (!hoverRange.IsEmpty)
                    {
                        this.Grid.InvalidateRange(this.hoverRange);
                        isInvalidated = true;
                    }

                    this.hoverRange = range;
                    if (!hoverRange.IsEmpty)
                    {
                        this.Grid.InvalidateRange(this.hoverRange);
                        isInvalidated = true;
                    }
                }
            }
            else
            {
                if (this.inMouseDownRange != mouseDownRange.Equals(range))
                {
                    this.inMouseDownRange = !inMouseDownRange;
                    this.Grid.InvalidateRange(this.mouseDownRange);
                    isInvalidated = true;
                }
            }

            base.OnMouseHover(rowIndex, colIndex, e);
            if (isInvalidated)
            {
                Grid.GetWindow().Update();
            }

            isInvalidated = false;
            base.OnMouseHover(rowIndex, colIndex, e);
        }
        protected override void OnMouseHoverLeave(int rowIndex, int colIndex, EventArgs e)
        {
            this.hoverRange = GridRangeInfo.Empty ;
            base.OnMouseHoverLeave(rowIndex, colIndex, e);
        }
        protected override Rectangle OnLayout(int rowIndex, int colIndex, GridStyleInfo style, Rectangle innerBounds, Rectangle[] buttonsBounds)
        {
            Rectangle buttonArea;
            int buttonWidth = 18;
           
            bool isTextRightToLeft = (style.RightToLeft == RightToLeft.Inherit && Grid.IsRightToLeft()) || style.RightToLeft == RightToLeft.Yes;
            //Button area customization...........
            if (!isTextRightToLeft)
            {
                buttonArea = Rectangle.FromLTRB(innerBounds.Location.X + sortIconSize, innerBounds.Location.Y, innerBounds.Right + buttonWidth + sortIconSize, innerBounds.Bottom);
            }
            else if (baseGrid.AllowSorting)
            {
                buttonArea = Rectangle.FromLTRB(innerBounds.Location.X + sortIconSize, innerBounds.Location.Y, innerBounds.Left + buttonWidth + sortIconSize, innerBounds.Bottom);
            }
            else
            {
                buttonArea = Rectangle.FromLTRB(innerBounds.Location.X, innerBounds.Location.Y, innerBounds.Left + buttonWidth, innerBounds.Bottom);
            }
            buttonsBounds[0] = GridUtil.CenterInRect(buttonArea, new Size(buttonWidth, 20));
            r1 = buttonArea;

            if (this.gridBase is FilterBar)
            {
                 Rectangle buttonArea2;
                 if (!isTextRightToLeft)
                 {
                     buttonArea = Rectangle.FromLTRB(innerBounds.Location.X + sortIconSize, innerBounds.Location.Y, innerBounds.Right + buttonWidth, innerBounds.Bottom);
                     buttonsBounds[0] = GridUtil.CenterInRect(buttonArea, new Size(buttonWidth, 20));
                     buttonArea2 = Rectangle.FromLTRB(buttonsBounds[0].Right, innerBounds.Top, innerBounds.Right, innerBounds.Bottom);
                     buttonsBounds[1] = GridUtil.CenterInRect(buttonArea2, new Size(buttonWidth, 20));
                 }
                 else
                 {
                     buttonArea = Rectangle.FromLTRB(innerBounds.Location.X, innerBounds.Location.Y, innerBounds.Left + buttonWidth, innerBounds.Bottom);
                     buttonsBounds[0] = GridUtil.CenterInRect(buttonArea, new Size(buttonWidth, 20));
                     buttonArea2 = Rectangle.FromLTRB(buttonArea.X - sortIconSize, innerBounds.Location.Y, buttonArea.X, innerBounds.Bottom);
                     buttonsBounds[1] = GridUtil.CenterInRect(buttonArea2, new Size(buttonWidth, 20));
                 }
                 r2 = buttonArea2;
            }
            return innerBounds;
        }
        /// <override/>
        protected override void OnButtonClicked(int rowIndex, int colIndex, int button)
        {
            base.OnButtonClicked(rowIndex, colIndex, button);
            OnPushButtonClick(rowIndex, colIndex);
        }
        /// <summary>
        /// Raises <see cref="GridControlBase.PushButtonClick"/> event when the user presses the PushButton.
        /// </summary>
        /// <param name="rowIndex">Specifies the row id.</param>
        /// <param name="colIndex">Specifies the column id.</param>
        protected virtual void OnPushButtonClick(int rowIndex, int colIndex)
        {
            Grid.RaisePushButtonClick(rowIndex, colIndex);
        }
    }
}
