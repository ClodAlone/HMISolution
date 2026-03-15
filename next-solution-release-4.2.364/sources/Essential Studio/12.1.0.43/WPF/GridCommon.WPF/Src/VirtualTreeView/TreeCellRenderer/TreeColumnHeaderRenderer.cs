#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Windows;
using System.Windows.Media;
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.GridCommon;


namespace Syncfusion.Windows.Controls.VirtualTreeView
{
    /// <summary>
    /// A renderer for displaying column header cells.
    /// </summary>
    /// <exclude/>
    public class TreeColumnHeaderRenderer : TreeCellRenderer
    {
        /// <summary>
        /// Called from <see cref="ICellRenderer.Render"/> to manually 
        /// render graphics that do not belong to live controls (e.g. static text).
        /// </summary>
        /// <param name="dc">The drawing context.</param>
        /// <param name="rca">The render cell layout information.</param>
        /// <param name="style">The cell style info.</param>
        protected override void OnRender(DrawingContext dc, RenderCellArgs rca, TreeRenderStyleInfo style)
        {
            Rect textRectangle = rca.CellRect;
            string text = style.CellValue.ToString();
            TextBoxPaint rt = new TextBoxPaint(new Typeface("Segoe UI Bold"), 12, Brushes.Black);
            rt.Trimming = TextTrimming.None;
            rt.HorizontalAlignment = TextAlignment.Center;
            rt.VerticalAlignment = VerticalAlignment.Center;

            // Draw the formatted text string to the DrawingContext of the control.
            rt.DrawText(dc, textRectangle, text);

            base.OnRender(dc, rca, style);
        }

    }

}
