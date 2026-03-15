#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Windows.Controls.Grid;
using System.Windows.Media;
using Syncfusion.Windows.Controls.Cells;
using System.Windows;
using System;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Controls;
namespace Syncfusion.Windows.Controls.Grid
{   

    public class GridTreeExpanderCellRendererExt : GridTreeExpandCellRenderer
    {

        bool hooked = false;

        /// <summary>
        /// Called when [render].
        /// </summary>
        /// <param name="dc">The dc.</param>
        /// <param name="rca">The rca.</param>
        /// <param name="style">The style.</param>
        protected override void OnRender(DrawingContext dc, RenderCellArgs rca, GridRenderStyleInfo style)
        {
            if (!hooked)
            {
                this.GridControl.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(GridControl_PreviewMouseLeftButtonDown);
                hooked = true;
            }
            base.OnRender(dc, rca, style);
        }

        public double ImageWidth = 0.0;
        public override void OnInitializeContent(TextBox textBox, GridRenderStyleInfo style)
        {
            base.OnInitializeContent(textBox, style);
            var gridTree = this.GridControl.FindParentElementOfType<GridTreeControl>();
            BitmapImage image = null;
            double imageHeight = 0;
            ImageWidth = 0;
            if (gridTree.InternalGrid.SupportNodeImages)
            {
                var n = style.Tag as GridTreeNode;
                GridTreeRequestNodeImageEventArgs args = new GridTreeRequestNodeImageEventArgs(n.Item, GridTreeControlImpl.RequestNodeImageEvent, gridTree.InternalGrid);
                gridTree.InternalGrid.OnRequestNodeImage(args);
                image = args.NodeImage;
                if (image != null)
                {
                    ImageWidth = image.Width + 2;
                    imageHeight = image.Height;
                }
            }
        }

        /// <summary>
        /// Arranges the UI element.
        /// </summary>
        /// <param name="aca">The aca.</param>
        /// <param name="textBox">The text box.</param>
        /// <param name="style">The style.</param>
        protected override void ArrangeUIElement(ArrangeCellArgs aca, TextBox textBox, GridRenderStyleInfo style)
        {
            var n = style.Tag as GridTreeNode;
            Thickness margins = style.TextMargins.ToThickness();
            textBox.Margin = new Thickness(margins.Left + ImageWidth, 0, 0, 0);
            margins.Left = 0;
            if (style.TextWrapping != TextWrapping.NoWrap)
            {
                Rect bounds = aca.CellRect;
                if (bounds.Height < style.Font.GetLineHeightValue() * 2)
                    textBox.TextWrapping = TextWrapping.NoWrap;
            }
            if (style.Font.Orientation != 0)
            {
                textBox.RenderTransform = null;
            }

            if (style.Font.Orientation != 0)
            {
                textBox.RenderTransform = null;
            }
            
            // TextBoxView always seems to have this margin and I am not able to reset the margin.
            // Therefore I am also hard-codeing it here so that TextBox behavior is properly
            // emulated.
            margins.Left = Math.Max(0, margins.Left - 2);
            margins.Right = Math.Max(0, margins.Right - 2);
            if (style.HasImageIndex)
            {
                margins = style.AdjustImageWidthAndHeightToMargin(margins, style.GridControl);
            }
            else
            {
                margins = style.ErrorInfo.AdjustErrorInfoMarginOnEditing(margins, style.GridControl, style.CellRowColumnIndex);
            }
            textBox.Padding = margins;


            //Console.WriteLine(string.Format("Width : {0} \t Height :{0}", aca.CellRect.Width, aca.CellRect.Height));
            //Console.WriteLine(string.Format("required Width : {0} \t required Height :{0}", requiredRange.Width, requiredRange.Height));
            if (requiredRange.Width >= 2)
            {
                var width = aca.CellRect.Width * requiredRange.Width;
                aca.CellRect = new Rect(aca.CellRect.X, aca.CellRect.Y, width, aca.CellRect.Height);
            }

            if (requiredRange.Height >= 2)
            {
                var height = aca.CellRect.Height * requiredRange.Height;
                aca.CellRect = new Rect(aca.CellRect.X, aca.CellRect.Y, aca.CellRect.Width, height);
            }
            SetBounds(textBox, aca.CellRect, aca.ForceMeasure, false);
        }

        /// <summary>
        /// Gets or sets a value indicating whether [click out side cell].
        /// </summary>
        /// <value><c>true</c> if [click out side cell]; otherwise, <c>false</c>.</value>
        internal bool ClickOutSideCell
        {
            get;
            set;
        }

        /// <summary>
        /// Handles the PreviewMouseLeftButtonDown event of the GridControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        void GridControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            GridTreeControl gridTree = this.GridControl.FindParentElementOfType<GridTreeControl>();
            RowColumnIndex cell = this.GridControl.PointToCellRowColumnIndex(e);
            GridTreeNode node;
            GridTreeRowType rowType = gridTree.InternalGrid.GetGridRowType(cell.RowIndex, out node);

            if (rowType != GridTreeRowType.Header && rowType != GridTreeRowType.UnboundRow && cell.ColumnIndex == 1 && cell.RowIndex > 0)
            {
                GridStyleInfo style = GridControl.GetRenderStyleInfo(cell);
                GridTreeNode n = style.Tag as GridTreeNode;
                bool MouseInExpander = false;
                bool mouseInOutside = false;
                if (gridTree.InternalGrid.ShowRowHeader)
                {
                    var width = gridTree.InternalGrid.ColumnWidths[0];
                    if (ExpandGlyphType == GridTreeExpandGlyph.PlusMinus || ExpandGlyphType == GridTreeExpandGlyph.PlusMinusLines)
                    {
                        MouseInExpander = lastPoint.X - width < style.TextMargins.Left && lastPoint.X - width > style.TextMargins.Left - NodeColumnWidth - 8;
                    }
                    else
                    {
                        MouseInExpander = lastPoint.X - width < style.TextMargins.Left && lastPoint.X - width > style.TextMargins.Left - 10;
                    }

                    var width1 = gridTree.InternalGrid.GetTextIndent(n.Level);
                    mouseInOutside = lastPoint.X - width < width1 && lastPoint.X - width > 0;
                }
                else
                {
                    var width = gridTree.InternalGrid.GetTextIndent(n.Level);
                    if (ExpandGlyphType == GridTreeExpandGlyph.PlusMinus || ExpandGlyphType == GridTreeExpandGlyph.PlusMinusLines)
                    {
                        MouseInExpander = lastPoint.X < style.TextMargins.Left && lastPoint.X > style.TextMargins.Left - NodeColumnWidth - 8;
                    }
                    else
                    {
                        MouseInExpander = lastPoint.X < style.TextMargins.Left && lastPoint.X > style.TextMargins.Left - 10;
                    }
                    mouseInOutside = lastPoint.X < width && lastPoint.X > 0;
                }

                if (mouseInOutside)
                {
                    this.ClickOutSideCell = true;
                }
                else if (cell == CurrentCell.CellRowColumnIndex)
                {
                    if (e.ClickCount == 1 && this.GridControl.Model.Options.ActivateCurrentCellBehavior == GridCellActivateAction.ClickOnCell)
                    {
                        this.CurrentCell.BeginEdit();
                    }
                    else if (e.ClickCount == 2 && this.GridControl.Model.Options.ActivateCurrentCellBehavior == GridCellActivateAction.DblClickOnCell)
                    {
                        this.CurrentCell.BeginEdit();
                    }
                }
                if (MouseInExpander)
                {
                    gridTree.InternalGrid.ExpandCollapseOnExpanderClick();
                    e.Handled = true;
                }
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            this.GridControl.PreviewMouseLeftButtonDown -= new MouseButtonEventHandler(GridControl_PreviewMouseLeftButtonDown);
            base.Dispose(disposing);
        }
    }
}