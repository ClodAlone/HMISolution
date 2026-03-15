#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.Controls.Scroll;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Controls.Grid
{
    /// <summary>
    /// Implements the model part of a text block cell.
    /// </summary>
    public class GridCellTextBlockModel : GridCellModel<GridCellTextBlockRenderer>
    {
    }

    /// <summary>
    /// Renders a TextBlock control inside a grid cell.
    /// </summary>
    public class GridCellTextBlockRenderer : GridVirtualizingCellRenderer<Border>
    {
        /// <summary>
        /// Initializes a new <see cref="GridCellTextBlockRenderer"/>.
        /// </summary>
        public GridCellTextBlockRenderer()
        {
            SupportsRenderOptimization = true;
            AllowRecycle = true;
            IsControlTextShown = true;
        }

        protected override void OnRender(DrawingContext dc, RenderCellArgs rca, GridRenderStyleInfo style)
        {
            if (rca.CellUIElements != null)
                return;

            // Will only get hit if SupportsRenderOptimization is true, otherwise rca.CellUIElements is never null.
            var margins = style.TextMargins.ToThickness();
            if (style.HasImageIndex)
            {
                margins = style.AdjustImageWidthAndHeightToMargin(margins, rca.CellRect.Size);
            }
            else
            {
                margins = style.ErrorInfo.AdjustErrorInfoMargin(margins, rca.CellRect.Size);
            }
            Rect textRectangle = rca.SubtractBorderMargins(rca.CellRect, margins);
            if (textRectangle.IsEmpty)
                return;

            string text = GetControlText(style);

            // Draw the formatted text string to the DrawingContext of the control.
            GridTextBoxPaint.DrawText(dc, textRectangle, text, style);
        }

        /// <summary>
        /// Initializes the content of the cell
        /// using the information from the cell style (value, text,
        /// behavior etc.).
        /// </summary>
        /// <param name="textBlock">The border.</param>
        /// <param name="style">The cell style info.</param>
        public override void OnInitializeContent(Border border, GridRenderStyleInfo style)
        {
            base.OnInitializeContent(border, style);
            /*Padding and Margins do not affect in TextBlock, since it works only when there is a parent content to host it*/
            Thickness margins = style.TextMargins.ToThickness();
            if (style.HasImageIndex)
            {
                margins = style.AdjustImageWidthAndHeightToMargin(margins, style.GridControl);
            }
            else
            {
                margins = style.ErrorInfo.AdjustErrorInfoMargin(margins, style.GridControl, style.CellRowColumnIndex);
            }
            border.SetValue(TextBox.MarginProperty, margins);
            var textBlock = new TextBlock();
            textBlock.Text = this.GetControlText(style);
            textBlock.VerticalAlignment = VerticalAlignment.Center;
            border.Child = textBlock;
            border.Padding = margins;
            if (style.FlowDirection == FlowDirection.RightToLeft)
                {
                textBlock.FlowDirection = style.FlowDirection;
                textBlock.TextWrapping = TextWrapping.NoWrap;
                double m11 = -1;
                double m22 = 1;
                double offsetX = textBlock.Width;
                double offsetY = 0;
                textBlock.LayoutTransform = new MatrixTransform(m11, 0, 0, m22, offsetX, offsetY); //make sure this does not leak...
                }
            else
                {
                textBlock.LayoutTransform = MatrixTransform.Identity;
                }
            VisualContainer.SetWantsMouseInput(textBlock, false);
        }

        public override void CreateRendererElement(Border border, GridRenderStyleInfo style)
        {
            Thickness margins = style.TextMargins.ToThickness();
            if (style.HasImageIndex)
            {
                margins = style.AdjustImageWidthAndHeightToMargin(margins, style.GridControl);
            }
            else
            {
                margins = style.ErrorInfo.AdjustErrorInfoMargin(margins, style.GridControl, style.CellRowColumnIndex);
            }
            border.SetValue(TextBox.MarginProperty, margins);
            var textBlock = new TextBlock();
            textBlock.Text = this.GetControlText(style);
            textBlock.VerticalAlignment = VerticalAlignment.Center;
            border.Child = textBlock;
            border.Padding = margins;
            VisualContainer.SetWantsMouseInput(textBlock, false);
            base.CreateRendererElement(border, style);
        }

        protected override string GetControlTextFromEditorCore(Border border)
        {
            var textBlock = border.Child as TextBlock;
            if (textBlock != null)
            {
                return textBlock.Text;
            }

            return string.Empty;
        }
    }

}
