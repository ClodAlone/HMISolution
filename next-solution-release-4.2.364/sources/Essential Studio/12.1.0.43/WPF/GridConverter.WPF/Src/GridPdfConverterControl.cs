#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Drawing;

namespace Syncfusion.Windows.Controls.Grid.Converter
{

    using System;
    using System.Windows;
    using Syncfusion.Pdf.Grid;
    using Syncfusion.Pdf.Graphics;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Documents;
    using System.Text;
    using Syncfusion.Pdf;

    /// <summary>
    /// Converter Base includes the basic properties and methods used for exporting Grid to PDF
    /// </summary>
    static class GridPdfConverterControl
    {

        /// <summary>
        /// Copies values and styles from GridStyleInfo to PdfGridCell
        /// </summary>
        /// <param name="gridModel">used to pass GridStyleInfo</param>
        /// <param name="pdfGrid"><see cref="PdfGrid"/></param>
        /// <param name="pdfGridCell">A PdfGridCell to which values and styles to be copied.</param>
        /// <param name="rowIndex"> RowIndex of the currently exporting Grid Cell </param>
        /// <param name="columnIndex">Column Index of the currently Exporting Grid Cell</param>
        /// <param name="pdfExportHandler"> Delegate Event handler which fires for every cell before exporting. </param>
        public static void GridCellToPdf(GridModel gridModel, PdfGrid pdfGrid, PdfGridCell pdfGridCell, int rowIndex, int columnIndex, GridCellExportToPdfHandler pdfExportHandler)
        {
            var exportingOptions = new ExportToPdfOptions {PdfExportHandler = pdfExportHandler};
            GridCellToPdf(gridModel, pdfGridCell, rowIndex, columnIndex, exportingOptions); 
        }

        /// <summary>
        /// Copies values and styles from GridStyleInfo to PdfGridCell
        /// </summary>
        /// <param name="gridModel">used to pass GridStyleInfo</param>
        /// <param name="pdfGridCell">A PdfGridCell to which values and styles to be copied.</param>
        /// <param name="rowIndex"> RowIndex of the currently exporting Grid Cell </param>
        /// <param name="columnIndex">Column Index of the currently Exporting Grid Cell</param>
        /// <param name="exportingOptions"> <see cref="ExportToPdfOptions"/> holds the Export To Pdf Options </param>
        public static void GridCellToPdf(GridModel gridModel, PdfGridCell pdfGridCell, int rowIndex, int columnIndex, ExportToPdfOptions exportingOptions)
        {
            var pdfExportHandler = exportingOptions.PdfExportHandler;
            var style = gridModel[rowIndex, columnIndex];
            var args = new ExportingToPdfEventArgs(style, pdfGridCell);

            if (pdfExportHandler != null)
            {
                pdfExportHandler(gridModel, args);
            }

            if (args.Handled)
                return;

            if (exportingOptions.ExportStyles)
            {
                CopyBackground(style, pdfGridCell);
                CopyFontStyle(pdfGridCell, style);
                CopyAlignment(pdfGridCell, style);

                if (gridModel.RowHeights[rowIndex] == 0 || gridModel.ColumnWidths[columnIndex] == 0)
#if !SILVERLIGHT
                    pdfGridCell.Style.Borders.All = new PdfPen(new PdfColor(System.Drawing.Color.Transparent), 0);
#else
                    pdfGridCell.Style.Borders.All = new PdfPen(new PdfColor(Colors.Transparent), 0);
#endif
                else
                    CopyBorders(style, pdfGridCell);

            }
            else
            {
#if !SILVERLIGHT
                pdfGridCell.Style.Borders.All = new PdfPen(new PdfColor(System.Drawing.Color.DarkGray), 0);
#else
                pdfGridCell.Style.Borders.All = new PdfPen(new PdfColor(Colors.Transparent), 0);
#endif
            }
            CopyCellValue(style, pdfGridCell);
            CopyCoveredCells(gridModel, pdfGridCell, rowIndex, columnIndex, exportingOptions);
        }

        #region Internal Fields

        /// <summary>
        /// Copies the CellValue from GridStyleInfo to PdfGridCell based on the cell type.
        /// </summary>
        /// <param name="style">GridStyleInfo </param>
        /// <param name="pdfGridCell"> PdfGridCell to whcih the values and styles to be copied.</param>
        private static void CopyCellValue(GridStyleInfo style, PdfGridCell pdfGridCell)
        {
            
            if (style.CellType == "FormulaCell")
            {
                pdfGridCell.Value = style.FormattedText;
            }
#if !SILVERLIGHT
            else if (style.CellType == "RichText")
            {
                CopyRichText(style, pdfGridCell);
            }
#endif
            else if (style.CellType == "ImageCell")
            {
                CopyImageCells(style, pdfGridCell);
            }
            else if (style.CellType == "ExpandCollapseCell")
            {
#if !SILVERLIGHT
                var font = new PdfTrueTypeFont(new Font("Calibiri", 14));
#else
                PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 14);
#endif
                pdfGridCell.Style.Font = font;
                pdfGridCell.Value = (style.Text== "False" ? "  +" : "  -");
            }
            else
            {               
                pdfGridCell.Value = style.FormattedText;
            }
        }

        /// <summary>
        /// Copies RichTextBox from GridStyleInfo to PdfGridCell
        /// </summary>
        /// <param name="style"><see cref="GridStyleInfo"/> holds the RichTextBox values of Grid</param>
        /// <param name="pdfGridCell">PdfGridCell to which the RichTextBox to be copied.</param>
        private static void CopyRichText(GridStyleInfo style, PdfGridCell pdfGridCell)
        {
            if (style.CellType == "RichText")
            {
#if !SILVERLIGHT
                var document = style.CellValue as FlowDocument;                
                var textRange = new TextRange(document.ContentStart, document.ContentEnd);
                var text = textRange.Text;
                
                //Draw the image.
                pdfGridCell.Style.BackgroundImage = PdfImage.FromRtf(text, (float)100f, PdfImageType.Bitmap);
#endif
                pdfGridCell.Value = string.Empty;
            }
        }

        /// <summary>
        /// Sets Alignment details from GridStyleInfo to PdfGridCell
        /// </summary>
        /// <param name="style">GridStyleInfo </param>
        /// <param name="pdfGridCell"> PdfGridCell to whcih the Alignment to be copied.</param>
        private static void CopyAlignment(PdfGridCell pdfGridCell, GridStyleInfo style)
        {
            //For Horizontal Alignment of the cell
            if (style.HorizontalAlignment == HorizontalAlignment.Center)
                pdfGridCell.StringFormat.Alignment = PdfTextAlignment.Center;
            else if (style.HorizontalAlignment == HorizontalAlignment.Left)
                pdfGridCell.StringFormat.Alignment = PdfTextAlignment.Left;
            else if (style.HorizontalAlignment == HorizontalAlignment.Right)
                pdfGridCell.StringFormat.Alignment = PdfTextAlignment.Right;
            else if (style.HorizontalAlignment == HorizontalAlignment.Stretch)
                pdfGridCell.StringFormat.Alignment = PdfTextAlignment.Justify;
            else
                pdfGridCell.StringFormat.Alignment = PdfTextAlignment.Left;

            //For Vertical Alignment of the cell
            if (style.VerticalAlignment == VerticalAlignment.Center)
                pdfGridCell.StringFormat.LineAlignment = PdfVerticalAlignment.Middle;
            else if (style.VerticalAlignment == VerticalAlignment.Bottom)
                pdfGridCell.StringFormat.LineAlignment = PdfVerticalAlignment.Bottom;
            else if (style.VerticalAlignment == VerticalAlignment.Top)
                pdfGridCell.StringFormat.LineAlignment = PdfVerticalAlignment.Top;
            else
                pdfGridCell.StringFormat.LineAlignment = PdfVerticalAlignment.Middle;
        }

#if SILVERLIGHT
        private static void CopyBorderStyle(PdfPen border, BorderStyle borderStyle)
        {
            switch (borderStyle)
            {
                case BorderStyle.Dotted:
                    border.DashStyle = PdfDashStyle.Dot;
                    break;
                case BorderStyle.Dashed:
                    border.DashStyle = PdfDashStyle.Dash;
                    break;
                case BorderStyle.DashDot:
                    border.DashStyle = PdfDashStyle.DashDot;
                    break;
                case BorderStyle.DashDotDot:
                    border.DashStyle = PdfDashStyle.DashDotDot;
                    break;
                case BorderStyle.Custom:
                    border.DashStyle = PdfDashStyle.Custom;
                    break;
                case BorderStyle.None:
                    border = new PdfPen(Colors.Red, 0f);
                    break;
                default:
                    border.DashStyle = PdfDashStyle.Solid;
                    break;
            }
        }
#endif
              
        /// <summary>
        /// Copies the borders from GridStyleInfo to PdfGridCell.
        /// </summary>
        /// <param name="style"/><see cref="GridStyleInfo"/>
        /// <param name="pdfGridCell"/><see cref="PdfGridCell"/>
        private static void CopyBorders(GridStyleInfo style, PdfGridCell pdfGridCell)
        {
            GridStyleInfo checkPreviousCell = null;

#if !SILVERLIGHT
            var pdfCellBorder = pdfGridCell.Style.Borders;
#else
            var pdfCellBorder = new PdfBorders();
#endif
            if (style.Borders.Right != null)
                pdfCellBorder.Right = ConvertToPdfBorder(style.Borders.Right);
            else
                pdfCellBorder.Right = new PdfPen(PdfBrushes.Transparent, 0);

            if (style.Borders.Bottom != null)
                pdfCellBorder.Bottom = ConvertToPdfBorder(style.Borders.Bottom);
            else
                pdfCellBorder.Bottom = new PdfPen(PdfBrushes.Transparent, 0);

            if (style.RowIndex - 1 >= 0)
                checkPreviousCell = style.GridModel[style.RowIndex - 1, style.ColumnIndex];

            if (checkPreviousCell != null && checkPreviousCell.GridModel.RowHeights[checkPreviousCell.RowIndex] != 0)
            {
                if (checkPreviousCell.Borders.Bottom != null && style.Borders.Top == null)
                    pdfCellBorder.Top = ConvertToPdfBorder(checkPreviousCell.Borders.Bottom);
                else if (style.Borders.Top != null)
                    pdfCellBorder.Top = ConvertToPdfBorder(style.Borders.Top);
                else
                    pdfCellBorder.Top = new PdfPen(PdfBrushes.Transparent, 0);
            }
            else if (style.Borders.Top != null)
                pdfCellBorder.Top = ConvertToPdfBorder(style.Borders.Top);
            else
                pdfCellBorder.Top = new PdfPen(PdfBrushes.Transparent, 0);

            if (style.ColumnIndex - 1 >= 0)
                checkPreviousCell = style.GridModel[style.RowIndex, style.ColumnIndex - 1];

            if (checkPreviousCell != null && checkPreviousCell.GridModel.ColumnWidths[checkPreviousCell.ColumnIndex] != 0)
            {
                if (checkPreviousCell.Borders.Right != null && style.Borders.Left == null)
                    pdfCellBorder.Left = ConvertToPdfBorder(checkPreviousCell.Borders.Right);
                else if (style.Borders.Left != null)
                    pdfCellBorder.Left = ConvertToPdfBorder(style.Borders.Left);
                else
                    pdfCellBorder.Left = new PdfPen(PdfBrushes.Transparent, 0);
            }
            else if (style.Borders.Left != null)
                pdfCellBorder.Left = ConvertToPdfBorder(style.Borders.Left);
            else
                pdfCellBorder.Left = new PdfPen(PdfBrushes.Transparent, 0);
        }

        /// <summary>
        /// Converts Pen to PdfPen
        /// </summary>
        /// <param name="border">Value of Type Pen<see cref="System.Windows.Media.Pen"/></param>
        /// <returns>Value of Type PdfPen<see cref="PdfPen"/></returns>
        private static PdfPen ConvertToPdfBorder(Pen border)
        {
#if SILVERLIGHT
            Color _color;
#else
            System.Drawing.Color _color;
#endif

            var _borderBrush = border.Brush != null ? border.Brush as SolidColorBrush : new SolidColorBrush(Colors.Transparent);

#if SILVERLIGHT
            _color = _borderBrush.Color;
#else            
            _color = System.Drawing.Color.FromArgb(_borderBrush.Color.R, _borderBrush.Color.G, _borderBrush.Color.B);
#endif
            var borderThickness = (float)border.Thickness;

#if SILVERLIGHT
            if (border.Style == BorderStyle.None)
                borderThickness = 0f;
#endif
            var pdfBorder = new PdfPen(_color, borderThickness);

#if !SILVERLIGHT
            if (border.DashStyle == DashStyles.Dash)
                pdfBorder.DashStyle = PdfDashStyle.Dash;
            else if (border.DashStyle == DashStyles.DashDot)
                pdfBorder.DashStyle = PdfDashStyle.DashDot;
            else if (border.DashStyle == DashStyles.DashDotDot)
                pdfBorder.DashStyle = PdfDashStyle.DashDotDot;
            else if (border.DashStyle == DashStyles.Dot)
                pdfBorder.DashStyle = PdfDashStyle.Dot;
            else if (border.DashStyle == DashStyles.Solid)
                pdfBorder.DashStyle = PdfDashStyle.Solid;

            if (border.DashCap == PenLineCap.Flat)
                pdfBorder.LineCap = PdfLineCap.Flat;
            else if (border.DashCap == PenLineCap.Round)
                pdfBorder.LineCap = PdfLineCap.Round;
            else if (border.DashCap == PenLineCap.Square)
                pdfBorder.LineCap = PdfLineCap.Square;

            if (border.LineJoin == PenLineJoin.Bevel)
                pdfBorder.LineJoin = PdfLineJoin.Bevel;
            else if (border.LineJoin == PenLineJoin.Miter)
                pdfBorder.LineJoin = PdfLineJoin.Miter;
            else if (border.LineJoin == PenLineJoin.Round)
                pdfBorder.LineJoin = PdfLineJoin.Round;

            pdfBorder.MiterLimit = (float)border.MiterLimit;
#endif
            return pdfBorder;
        }

        /// <summary>
        /// Copies the covered cells from GridModel to PdfGridCell.
        /// </summary>
        /// <param name="gridModel"><see cref="GridModel"/>holds the covered cells Info</param>
        /// <param name="pdfGridCell">PdfGridCell to which the column spanning is set.</param>
        /// <param name="rowIndex">Row Index of the Grid Cell</param>
        /// <param name="columnIndex">ColumnIndex of the Grid Cell</param>
        /// <param name="exportingOptions"><see cref="ExportToPdfOptions"/> Used to check the Exclude Columns while setting the Column Span</param>
        private static void CopyCoveredCells(GridModel gridModel, PdfGridCell pdfGridCell, int rowIndex, int columnIndex,ExportToPdfOptions exportingOptions)
        {
            GridRangeInfo range, overlappingRange;
            var isCoveredCell = gridModel.CoveredCells.Find(rowIndex, columnIndex, out range);
            var isOverlappingCell = gridModel.OverlappingCells.Find(rowIndex, columnIndex, out overlappingRange);
            if (isCoveredCell)
            {
                var rightRange = range.Right > exportingOptions.EndColumnIndex ? exportingOptions.EndColumnIndex : range.Right;
                var leftRange = range.Left < exportingOptions.StartColumnIndex ? exportingOptions.StartColumnIndex : range.Left;

                if (range.Top == rowIndex && leftRange == columnIndex)
                {
                    var excludeColumnCount = 0;
                    if (exportingOptions.ExcludeColumns.Count > 0)
                    {
                        for (int i = leftRange + 1; i <= rightRange; i++)
                        {
                            if (exportingOptions.ExcludeColumnsIndexList.Contains(i))
                                excludeColumnCount++;
                        }
                    }
                    pdfGridCell.ColumnSpan = (rightRange - leftRange) > 0 ? (rightRange - leftRange-excludeColumnCount) + 1 : 1;
                    pdfGridCell.RowSpan = (range.Bottom - range.Top) > 0 ? (range.Bottom - range.Top) + 1 : 1;
                }
                else if(leftRange<columnIndex && exportingOptions.ExcludeColumnsIndexList.Contains(leftRange))
                {
                    var copyFromPreviuosCell = true;
                    var leftRangeDifference = columnIndex - leftRange;
                    for (int i = leftRange; i < columnIndex; i++)
                    {
                        if (!exportingOptions.ExcludeColumnsIndexList.Contains(i))
                        {
                            copyFromPreviuosCell = false;
                            break;
                        }
                    }
                    if (copyFromPreviuosCell)
                    {
                        pdfGridCell.ColumnSpan = (rightRange - leftRange - leftRangeDifference) + 1;
                        pdfGridCell.Value = gridModel[rowIndex, leftRange].CellValue;
                    }
                }
            }
            if (isOverlappingCell)
            {
                if (overlappingRange.Top == rowIndex && overlappingRange.Left == columnIndex)
                {
                    pdfGridCell.ColumnSpan = (overlappingRange.Right - overlappingRange.Left) > 0
                                                 ? (overlappingRange.Right - overlappingRange.Left) + 1
                                                 : 1;
                    pdfGridCell.RowSpan = (overlappingRange.Bottom - overlappingRange.Top) > 0
                                              ? (overlappingRange.Bottom - overlappingRange.Top) + 1
                                              : 1;
                }
            }
        }        

        /// <summary>
        /// Copies the image from GridStyleInfo to PdfGridCell
        /// </summary>
        /// <param name="style"><see cref="GridStyleInfo"/>holds the ImagePath or Stream details</param>
        /// <param name="pdfGridCell">PdfGridCell to which the Image is to be copied.</param>
        private static void CopyImageCells(GridStyleInfo style, PdfGridCell pdfGridCell)
        {
            if (style.CellValue is BitmapImage)
            {
#if !SILVERLIGHT
                if ((style.CellValue as BitmapImage).StreamSource != null)
                    pdfGridCell.Style.BackgroundImage = PdfImage.FromStream((style.CellValue as BitmapImage).StreamSource);
                else
                    pdfGridCell.Style.BackgroundImage = PdfImage.FromFile(style.CellValue.ToString());
#endif
                pdfGridCell.Value = string.Empty;
            }
        }
       
        /// <summary>
        /// Copies the Background from GridStyleInfo to PdfGridCell.
        /// </summary>
        /// <param name="cellStyle"><see cref="GridStyleInfo"/> holds the Background details.</param>
        /// <param name="pdfGridCell">PdfGridCell to which the background is to be copied.</param>
        private static void CopyBackground(GridStyleInfo cellStyle, PdfGridCell pdfGridCell)
        {
            if (cellStyle.Background is SolidColorBrush)
            {
                var brush = cellStyle.Background as SolidColorBrush;
                Color color = brush.Color;
#if SILVERLIGHT
                pdfGridCell.Style.BackgroundBrush = new PdfSolidBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
#else
                pdfGridCell.Style.BackgroundBrush = new PdfSolidBrush(new PdfColor(System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B)));
#endif
            }
            else if (cellStyle.GridModel.HeaderStyle.Background is LinearGradientBrush)
            {
                //Code here to set the Row/Column header Background
                var brush = cellStyle.GridModel.HeaderStyle.Background as LinearGradientBrush;
#if !SILVERLIGHT
                var lr = new LinearGradientBrush(brush.GradientStops[0].Color, brush.GradientStops[1].Color,
                                                 new Point(0, 0), new Point(1000, 1000));
#else
                    LinearGradientBrush lr = new LinearGradientBrush();
                    lr.StartPoint = new Point(0, 0);
                    lr.EndPoint = new Point(700, 10);
                    GradientStop g1 = new GradientStop();
                    g1.Color = brush.GradientStops[0].Color;
                    lr.GradientStops.Add(g1);
                    GradientStop g2 = new GradientStop();
                    g2.Color = brush.GradientStops[1].Color;
                    lr.GradientStops.Add(g2);
#endif
                var p1 = new PointF((float) lr.StartPoint.X, (float) lr.StartPoint.Y);
                var p2 = new PointF((float) lr.EndPoint.X, (float) lr.EndPoint.Y);
#if SILVERLIGHT
                var linear = new PdfLinearGradientBrush(p1, p2, new PdfColor(lr.GradientStops[0].Color.R, lr.GradientStops[0].Color.G, lr.GradientStops[0].Color.B), new PdfColor(lr.GradientStops[1].Color.R, lr.GradientStops[1].Color.G, lr.GradientStops[1].Color.B));
#else
                var linear = new PdfLinearGradientBrush(p1, p2,
                                                        new PdfColor(lr.GradientStops[0].Color.R,
                                                                     lr.GradientStops[0].Color.G,
                                                                     lr.GradientStops[0].Color.B),
                                                        new PdfColor(lr.GradientStops[1].Color.R,
                                                                     lr.GradientStops[1].Color.G,
                                                                     lr.GradientStops[1].Color.B));
#endif
                pdfGridCell.Style.BackgroundBrush = linear;
            }
        }

        /// <summary>
        /// Copies the font style.
        /// </summary>
        /// <param name="pdfGridCell">The PDF grid cell to which the font is to be copied.</param>
        /// <param name="cellStyle"><see cref="GridStyleInfo"/> holds the font informations.</param>
        private static void CopyFontStyle(PdfGridCell pdfGridCell, GridStyleInfo cellStyle)
        {
            var gridfont = cellStyle.Font;

            var isBold = false;
            var isItalic = false;
            var isunderline = false;
            var isstrikethrough = false;
#if SILVERLIGHT
            if (gridfont.FontWeight == FontWeights.Bold || gridfont.FontWeight == FontWeights.ExtraBold|| gridfont.FontWeight == FontWeights.SemiBold)
#else 
                if (gridfont.FontWeight == FontWeights.Bold || gridfont.FontWeight == FontWeights.DemiBold || gridfont.FontWeight == FontWeights.ExtraBold
                || gridfont.FontWeight == FontWeights.SemiBold || gridfont.FontWeight == FontWeights.UltraBold)
#endif
            {
                isBold = true;
            }
            if (gridfont.FontStyle == FontStyles.Italic)
                isItalic = true;

            if (gridfont.HasTextDecorations && gridfont.TextDecorations!=null)
            {
#if !SILVERLIGHT
                foreach (var textdecoration in gridfont.TextDecorations)
                {
                    if (textdecoration.Location == TextDecorationLocation.Strikethrough)
                        isstrikethrough = true;
                    if (textdecoration.Location == TextDecorationLocation.Underline)
                        isunderline = true;
                }
#else
                if (gridfont.TextDecorations == TextDecorations.Underline)
                    isunderline = true;
#endif
            }

            var fontSize = (float)Math.Round(gridfont.FontSize / 1.4);
            var fontName = gridfont.FontFamily.ToString();
            // The following code is to convert the Grid's font to Pdf font(Can convert all fonts)
#if ! SILVERLIGHT
            PdfTrueTypeFont pdfFont = null;
            var font = new Font(fontName, fontSize, (isBold ? System.Drawing.FontStyle.Bold : System.Drawing.FontStyle.Regular) |
                                                                    (isItalic ? System.Drawing.FontStyle.Italic : System.Drawing.FontStyle.Regular) |
                                                                    (isunderline ? System.Drawing.FontStyle.Underline : System.Drawing.FontStyle.Regular) |
                                                                    (isstrikethrough ? System.Drawing.FontStyle.Strikeout : System.Drawing.FontStyle.Regular));

            var value = cellStyle.CellValue != null ? cellStyle.CellValue.ToString() : "";
            var unicode = Encoding.UTF8.GetByteCount(value) != value.Length;
            pdfFont = new PdfTrueTypeFont(font, unicode);
#else
            PdfFont pdfFont = new PdfStandardFont(PdfFontFamily.Helvetica, fontSize);
#endif

            pdfGridCell.Style.Font = pdfFont;
            if (cellStyle.Foreground is SolidColorBrush)
            {
                var brush = cellStyle.Foreground as SolidColorBrush;
                Color color = brush.Color;
#if SILVERLIGHT
                pdfGridCell.Style.TextBrush = new PdfSolidBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
#else
                pdfGridCell.Style.TextBrush = new PdfSolidBrush(new PdfColor(System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B)));
#endif
            }
            if (cellStyle.Foreground is LinearGradientBrush)
            {
                var brush = cellStyle.Foreground as LinearGradientBrush;
                if (brush != null)
                {
                    PointF p1 = new PointF((float)brush.StartPoint.X, (float)brush.StartPoint.Y);
                    PointF p2 = new PointF((float)brush.EndPoint.X, (float)brush.EndPoint.Y);
#if SILVERLIGHT
                    var linear = new PdfLinearGradientBrush(p1, p2, brush.GradientStops[0].Color, brush.GradientStops[1].Color);
#else
                    var linear = new PdfLinearGradientBrush(p1, p2, new PdfColor(brush.GradientStops[0].Color.R, brush.GradientStops[0].Color.G, brush.GradientStops[0].Color.B), new PdfColor(brush.GradientStops[1].Color.R, brush.GradientStops[1].Color.G, brush.GradientStops[1].Color.B));
#endif
                    pdfGridCell.Style.TextBrush = linear;
                }
            }
        }

        #endregion
    }
}
