//-------------------------------------------------------------------------------------------------
// <copyright file="PivotGridSortColumnHeaderCellRenderer.cs" company="syncfusion">
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
using System.Runtime.Serialization;

namespace Syncfusion.Windows.Forms.PivotAnalysis
{

    public class SchemaItemCellRenderer : GridHeaderCellRenderer
    {
        private GridCellButton pushButton;
        private GridCellButton removeButton;
        private GridRangeInfo hoverRange = GridRangeInfo.Empty;
        GridRangeInfo range = null;
        bool mouseDown = false;
        bool inMouseDownRange = false;
        ThemedHeaderDrawing.HeaderState state = ThemedHeaderDrawing.HeaderState.Normal;
        GridRangeInfo mouseDownRange = GridRangeInfo.Empty;
        static readonly BrushInfo defaultInterior1 = new BrushInfo(GradientStyle.Vertical, Color.FromArgb(203, 199, 184), Color.FromArgb(238, 234, 216));
        PivotGridControl baseGrid;
        GridList grid;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public SchemaItemCellRenderer(GridControlBase grid, GridCellModelBase cellModel)
            : base(grid, cellModel)
        {
            if (grid is GridList)
            {
                baseGrid = (grid as GridList).PivotGridControl;
                this.grid = grid as GridList;
            }


            AddButton(pushButton = new SchemaCellButton(this));
            AddButton(removeButton = new SchemaCellButton(this));
        }

        protected override void OnDraw(Graphics g, Rectangle clientRectangle, int rowIndex, int colIndex, GridStyleInfo style)
        {
            base.OnDraw(g, clientRectangle, rowIndex, colIndex, style);

            

            Color color = SystemColors.Window;
            if (this.baseGrid != null)
            {
                switch (this.baseGrid.GridVisualStyles)
                {
                    case GridVisualStyles.Office2007Blue:
                    case GridVisualStyles.Office2010Blue:
                    case GridVisualStyles.Metro:
                        color = Color.LightGray;
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
            }
            style.Borders.All = new GridBorder(GridBorderStyle.Solid, color);
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
            //return Cursors.SizeAll;
        }
        /// <override/>
        protected override void OnMouseDown(int rowIndex, int colIndex, MouseEventArgs e)
        {
            base.OnMouseDown(rowIndex, colIndex, e);
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
            //Suppress the color change while hovering
            base.OnMouseHover(rowIndex, colIndex, e);
        }
        protected override void OnMouseHoverLeave(int rowIndex, int colIndex, EventArgs e)
        {
            hoverRange = GridRangeInfo.Empty;
            base.OnMouseHoverLeave(rowIndex, colIndex, e);
        }
        public override void OnPrepareViewStyleInfo(GridPrepareViewStyleInfoEventArgs e)
        {
            
           // base.OnPrepareViewStyleInfo(e);
        }
        /// <override/>
        protected override void OnClick(int rowIndex, int colIndex, MouseEventArgs e)
        {
            // Suppress click event - don't move current cell.
        }
        protected override void OnDrawCellButton(GridCellButton button, Graphics g, int rowIndex, int colIndex, bool bActive, GridStyleInfo style)
        {
            Point ptOffset = new Point(1, 1); 
            if (button.Bounds == r1) // context menu button
            {
                style.Description = "-";
            }
            else if (button.Bounds == r2) // remove button
            {
                if (this.grid.Tag != null && this.grid.Tag.ToString().Equals("PivotFilters"))
                {
                    style.Description = "@";
                }
                else
                    style.Description = "+";
            }
            button.Draw(g, rowIndex, colIndex, false, style);
            IconPaint iconPainter = new IconPaint(AssemblyInfo.RootNamespace + @".Resources.", AssemblyInfo.Assembly);
            GridProperties propertyObject = Grid.Model.Properties;
            string bm = null;
            string value = style.Description;
            if (style.CellAppearance == GridCellAppearance.Flat && propertyObject.Buttons3D)
            {
                Color shadow = SystemColors.ControlDarkDark;
                if (!Grid.PrintingMode)
                {
                    GridRangeInfo cellRange = GridRangeInfo.Cell(rowIndex, colIndex);
                    Point mouseClientPosition = Grid.GetWindow().PointToClient(Control.MousePosition);
                    Rectangle cr = button.Bounds;
                    cr.Inflate(1, 1);
                    if (this.baseGrid.GridVisualStyles == GridVisualStyles.Metro)
                    {
                        if (this.hoverRange.Contains(cellRange))
                        {
                            bm = value == "-" ? "WhiteD.png" : value == "@" ? "sch_filter_white.png" : "clear_white.png";

                            state = ThemedHeaderDrawing.HeaderState.Hot;

                            iconPainter.PaintIcon(g, button.Bounds, Point.Empty, bm, Color.Black);
                        }
                        else
                        {
                            bm = value == "-" ? "ClickedD.png" : value == "@" ? "sch_filter.png" : "clear.png";
                            iconPainter.PaintIcon(g, button.Bounds, Point.Empty, bm, Color.Black);
                        }
                    }
                }
            }

            Rectangle faceRect = button.Bounds;
            faceRect.Inflate(-2, -1);
        }

        Rectangle r1, r2;
        protected override Rectangle OnLayout(int rowIndex, int colIndex, GridStyleInfo style, Rectangle innerBounds, Rectangle[] buttonsBounds)
        {
            Rectangle buttonArea, buttonArea2;
            int buttonWidth = 18;
            bool isTextRightToLeft = (style.RightToLeft == RightToLeft.Inherit && Grid.IsRightToLeft()) || style.RightToLeft == RightToLeft.Yes;

            if (!isTextRightToLeft)
            {
                buttonArea = Rectangle.FromLTRB(((innerBounds.Right - (buttonWidth * 2))), innerBounds.Top, (innerBounds.Right - buttonWidth), innerBounds.Bottom);
                buttonArea2 = Rectangle.FromLTRB(buttonArea.Right + 2, innerBounds.Top, innerBounds.Right, innerBounds.Bottom);
            }
            else
            {
                buttonArea2 = Rectangle.FromLTRB(innerBounds.Location.X, innerBounds.Location.Y, innerBounds.Left + buttonWidth, innerBounds.Bottom);
                buttonArea = Rectangle.FromLTRB(buttonArea2.Right, innerBounds.Top, buttonArea2.Right + buttonWidth, innerBounds.Bottom);
            }

            buttonsBounds[0] = GridUtil.CenterInRect(buttonArea, new Size(buttonWidth, 20));
            r1 = buttonArea;
            buttonsBounds[1] = GridUtil.CenterInRect(buttonArea2, new Size(buttonWidth, 20));
            r2 = buttonArea2;
            return innerBounds;
        }
        protected override void OnDrawDisplayText(Graphics g, Rectangle textRectangle, int rowIndex, int colIndex, GridStyleInfo style)
        {
            if(this.baseGrid.GridVisualStyles==GridVisualStyles.Metro)
            style.Font.Size = 9f;
            base.OnDrawDisplayText(g, textRectangle, rowIndex, colIndex, style);
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

    public class SchemaItemCellModel : GridHeaderCellModel
    {
        /// <overload>
        /// Initializes a new <see cref="GridSortColumnHeaderCellModel"/> object.
        /// </overload>
        /// <summary>
        /// Initializes a new <see cref="GridSortColumnHeaderCellModel"/> object 
        /// and stores a reference to the <see cref="GridModel"/> this cell belongs to.
        /// </summary>
        /// <param name="grid">The <see cref="GridModel"/> for this cell model.</param>    
        /// <remarks>
        /// You typically access cell models through the <see cref="GridModel.CellModels"/>
        /// property of the <see cref="GridModel"/> class.
        /// </remarks>
        public SchemaItemCellModel(GridModel grid)
            : base(grid)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="GridSortColumnHeaderCellModel"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected SchemaItemCellModel(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ////TraceUtil.TraceCurrentMethodInfoIf(Switches.Serialization.TraceVerbose, info.FullTypeName, info.MemberCount);
        }

        /// <summary>
        /// This is called from GridStyleInfo.GetFormattedText. 
        /// </summary>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="value">The value to format.</param>
        /// <param name="textInfo">textInfo is a hint who is calling, default is GridCellBaseTextInfo.DisplayText.</param>
        /// <returns>The formatted test for the gives value.</returns>
        public override string GetFormattedText(GridStyleInfo style, object value, int textInfo)
        {
            GridStyleInfo styleColumn = Grid.ColStyles[((GridStyleInfoIdentity)style.Identity).ColIndex];
            string desc = styleColumn.Description;
            if (desc.Length > 0)
            {
                return desc;
            }

            return base.GetFormattedText(style, value, textInfo);
        }

        /// <override/>
        /// <summary>Creates a renderer for this cell model.</summary>
        /// <param name="control">The grid control.</param>
        /// <returns>Cell renderer.</returns>
        public override GridCellRendererBase CreateRenderer(GridControlBase control)
        {
            return new SchemaItemCellRenderer(control, this);
        }

        /// <summary>
        /// Calculates the preferred size of the cell based on its contents without margins and any buttons.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> context of the canvas.</param>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="queryBounds">grsphical bounds</param>
        /// <returns>The optimal size of the cell.</returns>
        /// <override/>
        protected override Size OnQueryPrefferedClientSize(Graphics g, int rowIndex, int colIndex, GridStyleInfo style, GridQueryBounds queryBounds)
        {
            Size size = base.OnQueryPrefferedClientSize(g, rowIndex, colIndex, style, queryBounds);

            size.Width += 12; // for sort triangle
            return size;
        }
    }
}
