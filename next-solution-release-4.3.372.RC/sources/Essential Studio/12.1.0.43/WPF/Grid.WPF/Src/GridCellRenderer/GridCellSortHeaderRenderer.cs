#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using Syncfusion.Windows.Controls.Cells;
using System;

namespace Syncfusion.Windows.Controls.Grid
{
    /// <summary>
    /// Implements model part of a sort header cell. It displays a sort icon that is used to sort the grid at run time.
    /// </summary>
    public class GridCellSortHeaderModel : GridCellModel<GridCellSortHeaderRenderer>
    {
    }

    #region SortHeader Renderer
    /// <summary>
    /// CellRenderer that conditionally displays a SortHeader triangle.
    /// </summary>
    public class GridCellSortHeaderRenderer : GridCellStaticTextRenderer
    {
        private VerticalAlignment verticalGlyphAlignment = VerticalAlignment.Top;

        /// <summary>
        /// Gets or sets the vertical alignment of the sorting triangle.
        /// </summary>
        public VerticalAlignment VerticalGlyphAlignment
        {
            get { return verticalGlyphAlignment; }
            set { verticalGlyphAlignment = value; }
        }
        private HorizontalAlignment horizontalGlyphAlignment = HorizontalAlignment.Right;

        /// <summary>
        /// Gets or sets the horizontal alignment of the sorting triangle.
        /// </summary>
        public HorizontalAlignment HorizontalGlyphAlignment
        {
            get { return horizontalGlyphAlignment; }
            set { horizontalGlyphAlignment = value; }
        }

        protected override void OnRender(DrawingContext dc, RenderCellArgs rca, GridRenderStyleInfo style)
        {
            Rect textRectangle = rca.CellRect;
            //adjust the rectangle for text margines
            textRectangle.Y += style.TextMargins.Top;
            textRectangle.Height = Math.Max(0, textRectangle.Height - style.TextMargins.Top + style.TextMargins.Bottom);
            textRectangle.X += style.TextMargins.Left;
            textRectangle.Width = Math.Max(0, textRectangle.Width - style.TextMargins.Left + style.TextMargins.Right);
            if (style.HorizontalAlignment == HorizontalAlignment.Right)            
            {                
                textRectangle.X += style.TextMargins.Left + 4;
                textRectangle.Width = (Math.Max(0, textRectangle.Width - (style.TextMargins.Left+4) + style.TextMargins.Right));
            }
            string text = GetControlText(style);

            bool? descending = null;
            if (style.Tag is ListSortDirection)
            {
                ListSortDirection lsd = (ListSortDirection)style.Tag;
                descending = (lsd == ListSortDirection.Ascending) ? false : true;
            }
            if (descending.HasValue)
            {
                bool multiColumnSorting = this.GridControl is ISupportsSortStates && ((ISupportsSortStates)GridControl).SortStates.Count > 1;

                int multiColumnSortNumberWidth = 5;
                int w = 4;
                int h = 3;
                double xoffSet = 10;
                double yoffSet = 7;
                double pt0X, pt0Y;
                switch (HorizontalGlyphAlignment)
                {
                    case HorizontalAlignment.Left:
                        pt0X = rca.CellRect.Left + xoffSet - 2 * w;
                        break;
                    case HorizontalAlignment.Center:
                        pt0X = (rca.CellRect.Left + rca.CellRect.Right) / 2 - w;
                        break;
                    default:
                        pt0X = rca.CellRect.Right - xoffSet;
                        break;
                }
                if (multiColumnSorting)
                {
                    pt0X -= multiColumnSortNumberWidth;
                }
                switch (VerticalGlyphAlignment)
                {
                    case VerticalAlignment.Bottom:
                        pt0Y = rca.CellRect.Bottom - yoffSet - 2 * h;
                        break;
                    case VerticalAlignment.Center:
                        pt0Y = rca.CellRect.Bottom - rca.CellRect.Height / 2 - h;
                        break;
                    default:
                        if (HorizontalGlyphAlignment == HorizontalAlignment.Center)
                        {
                            pt0Y = rca.CellRect.Top;
                        }
                        else
                        {
                            pt0Y = rca.CellRect.Top + yoffSet;
                        }
                        break;
                }
                Point pt0 = new Point(pt0X, pt0Y);
                PathGeometry pg = new PathGeometry();
                PathFigure pfRight = new PathFigure();

                pfRight.IsClosed = true;
                pfRight.IsFilled = true;

                PolyLineSegment pls;
                if (descending.Value)
                {
                    pfRight.StartPoint = new Point(pt0.X, pt0.Y);
                    pls = new PolyLineSegment(new Point[]{
                                                                    new Point(pt0.X + 2 * w , pt0.Y),
                                                                    new Point(pt0.X + w, pt0.Y + 2 * h)
                                                                   }, true);
                }
                else
                {
                    pfRight.StartPoint = new Point(pt0.X + w, pt0.Y);
                    pls = new PolyLineSegment(new Point[]{
                                                                    new Point(pt0.X + 2 * w , pt0.Y + 2 * h),
                                                                    new Point(pt0.X , pt0.Y + 2 * h)
                                                                   }, true);
                }

                pfRight.Segments.Add(pls);
                pg.Figures.Add(pfRight);
                dc.DrawGeometry(SortWidgetBrush, null, pg);
                if (multiColumnSorting)
                {
                    string s = ((ISupportsSortStates)GridControl).PropertyNameFromColumnIndex(rca.ColumnIndex);
                    int loc = ((ISupportsSortStates)GridControl).FindPropertyNameInStates(s);
                    if (loc > -1)
                    {
                        Point p = new Point(pt0X + 2 * w, pt0Y - h / 2);
                        FormattedText formattedText = new FormattedText(loc.ToString(), style.GetCulture(true), style.FlowDirection, style.Font.Typeface, style.Font.FontSize - 3, SortWidgetBrush);
                        dc.DrawText(formattedText, p);
                    }
                }
            }
            // Draw the formatted text string to the DrawingContext of the control.
            GridTextBoxPaint.DrawText(dc, textRectangle, text, style);
        }

        private Brush sortWidgetBrush = null;

        /// <summary>
        /// Gets or sets the Brush that used to draw the sorting triangle.
        /// </summary>
        public Brush SortWidgetBrush
        {
            get
            {
                if (sortWidgetBrush == null)
                {
                    sortWidgetBrush = Brushes.Gray;
                }
                return sortWidgetBrush;
            }
            set { sortWidgetBrush = value; }
        }
    }
    #endregion
}
