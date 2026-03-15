#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.Controls.Scroll;
using Syncfusion.Windows.GridCommon;

namespace Syncfusion.Windows.Controls.Grid
{
    /// <summary>
    /// Implements the model part of a static cell. A static cell is also a base class for many
    /// other cell types that display text.
    /// </summary>
    public class GridStaticCellModel : GridCellModel<GridCellStaticTextRenderer>
    {
    }

    /// <summary>
    /// Defines the renderer part of a static cell. A static cell renderer is also a base class for many
    /// other cell types and provides the inactive cell rendering for cell types that support editing (such as
    /// a text box or combo box).
    /// </summary>
    public class GridCellStaticTextRenderer : GridCellRendererBase
    {
        /// <summary>
        /// Initializes a new <see cref="GridCellStaticTextRenderer"/>.
        /// </summary>
        public GridCellStaticTextRenderer()
        {
            IsControlTextShown = true;
            this.IsEditable = false;
            this.SupportsRenderOptimization = true;
        }

        protected override void OnRender(DrawingContext dc, RenderCellArgs rca, GridRenderStyleInfo style)
        {
            Thickness margins = style.TextMargins.ToThickness();
            if (style.HasImageIndex)
            {
                margins = style.AdjustImageWidthAndHeightToMargin(margins, rca.CellRect.Size);
            }
            else
            {
                margins = style.ErrorInfo.AdjustErrorInfoMargin(margins, style.GridControl, style.CellRowColumnIndex);
            }
            Rect textRectangle = rca.SubtractBorderMargins(rca.CellRect, margins);

            string text = GetControlText(style);

            double dValue = 0;

            if (style.CellValue != null && style.HasNegativeForeground)
            {
                if (double.TryParse(style.CellValue.ToString(), out dValue))
                {
                    if (dValue < 0)
                        style.Foreground = style.HasNegativeForeground ? style.NegativeForeground : style.Foreground;
                }
            }
            // Draw the formatted text string to the DrawingContext of the control.
            GridTextBoxPaint.DrawText(dc, textRectangle, text, style);

            base.OnRender(dc, rca, style);
        }
    }

}
