#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Grid.Converter
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using Syncfusion.Windows.Controls.Cells;
    using Syncfusion.Windows.Controls.Grid;
    using Syncfusion.XlsIO;
    using Syncfusion.XlsIO.Implementation;
#if !SILVERLIGHT
    using System.Windows.Forms;
    using System.Data;
    using System.Windows.Media;
#else
    using System.Windows.Media;
#endif

    /// <summary>
    /// Base class for Excel Export
    /// </summary>
    public class GridExcelConverterBase
    {
        /// <summary>
        /// Maximum one-based index of the column in MS Excel.
        /// </summary>
        public const int DEFMAXCOLUMNONEINDEX = DEFMAXCOLUMNZEROINDEX + 1;

        /// <summary>
        /// Maximum zero-based index of the row in MS Excel.
        /// </summary>
        public const int DEFMAXROWZEROINDEX = ushort.MaxValue;

        /// <summary>
        /// Maximum one-based index of the row in MS Excel.
        /// </summary>
        public const int DEFMAXROWONEINDEX = DEFMAXROWZEROINDEX + 1;

        /// <summary>
        /// Currency format in the .Net.
        /// </summary>
        protected const char DEFCHARCURRENCY = 'C';

        /// <summary>
        /// Date format in the .Net.
        /// </summary>
        protected const char DEFCHARDATE = 'D';

        /// <summary>
        /// Sceintific format in the .Net.
        /// </summary>
        protected const char DEFCHARSCIENTIFIC = 'E';

        /// <summary>
        /// Fixed point format in the .Net.
        /// </summary>
        protected const char DEFCHARFIXEDPOINT = 'F';

        /// <summary>
        /// General format in the .Net.
        /// </summary>
        protected const char DEFCHARGENERAL = 'G';

        /// <summary>
        /// Number format in the .Net.
        /// </summary>
        protected const char DEFCHARNUMBER = 'N';

        /// <summary>
        /// Percent format in the .Net.
        /// </summary>
        protected const char DEFCHARPERCENT = 'P';

        /// <summary>
        /// Hexadecimal format in the .Net.
        /// </summary>
        protected const char DEFCHARHEX = 'X';

        /// <summary>
        /// Default horizontal alignment.
        /// </summary>
        protected const ExcelHAlign DEFDEFAULTHALIGNMENT = ExcelHAlign.HAlignLeft;

        /// <summary>
        /// Default vertical alignment.
        /// </summary>
        protected const ExcelVAlign DEFDEFAULTVALIGNMENT = ExcelVAlign.VAlignTop;

        /// <summary>
        /// Right angle value.
        /// </summary>
        protected const int DEFRIGHTANGLE = 90;

        /// <summary>
        /// Maximum possible angle value.
        /// </summary>
        protected const int DEFMAXIMUMANGLE = 360;

        /// <summary>
        /// Angle that corresponds to 90 rotation value in the XlsIO.
        /// </summary>
        protected const int DEFMINIMUMSUPPORTEDANGLE = DEFMAXIMUMANGLE - DEFRIGHTANGLE;

        /// <summary>
        /// General Format
        /// </summary>
        protected const string DEFGENERALFORMAT = "GENERAL";

        /// <summary>
        /// Excel Generatl Format
        /// </summary>
        protected const int DEFEXCELGENERALFORMAT = 0;

        /// <summary>
        /// Currency format index in the MS Excel.
        /// </summary>
        protected const int DEFEXCELCURRENCYFORMAT = 7;

        /// <summary>
        /// Decimal format index in the MS Excel.
        /// </summary>
        protected const int DEFEXCELDECIMALFORMAT = 4;

        /// <summary>
        /// Scientific format index in the MS Excel.
        /// </summary>
        protected const int DEFEXCELSCIENTIFICFORMAT = 11;

        /// <summary>
        /// Fixed-point format index in the MS Excel.
        /// </summary>
        protected const int DEFEXCELFIXEDPOINTFORMAT = 4;

        /// <summary>
        /// Number format index in the MS Excel.
        /// </summary>
        protected const int DEFEXCELNUMBERFORMAT = 4;

        /// <summary>
        /// Percent format index in the MS Excel.
        /// </summary>
        protected const int DEFEXCELPERCENTFORMAT = 9;

        /// <summary>
        /// Date format index in the MS Excel.
        /// </summary>
        protected const int DEFEXCELDATEFORMAT = 14;

        /// <summary>
        /// Hex format index in the MS Excel.
        /// </summary>
        protected const int DEFEXCELHEXFORMAT = 0;

        /// <summary>
        /// Maximum zero-based index of the column in MS Exce.
        /// </summary>
        protected const int DEFMAXCOLUMNZEROINDEX = 255;

        /// <summary>
        /// Color mask to remove Alpha component.
        /// </summary>
        private const int DEFCOLORMASK = 0xFFFFFF;

        /// <summary>
        /// Default color (white).
        /// </summary>
        private const int DEFAULTCOLOR = 0xFFFFFF;

        /// <summary>
        /// Copies row height settings from grid model into excel worksheet
        /// </summary>
        /// <param name="style">Source grid cell</param>
        /// <param name="range">Destination excel cell</param>
        protected void CopyRowHeightFromGrid(GridStyleInfo style, IRange range)
        {
            int height =Convert.ToInt32(style.GridModel.RowHeights[style.RowIndex]);
            if (0 < height && height < 409.5)
            {
                //range.RowHeight = height;
                range.Worksheet.SetRowHeightInPixels(range.Row, height);
                
            }
        }

        /// <summary>
        /// Copies column width settings from grid model into excel worksheet
        /// </summary>
        /// <param name="style">Source grid cell</param>
        /// <param name="range">Destination excel cell</param>
        protected void CopyColumnWidthFromGrid(GridStyleInfo style, IRange range)
        {
            int widthInPixels = Convert.ToInt32(style.GridModel.ColumnWidths[style.ColumnIndex]);
            double width = range.Worksheet.PixelsToColumnWidth(widthInPixels);
            //Width is applied as per the headers.
            if (!style.IsEmpty && (style as GridDataStyleInfo) != null)
            {
                if (width != 0 && ((style as GridDataStyleInfo).CellIdentity.TableCellType == GridDataTableCellType.ColumnHeaderCell) || (style as GridDataStyleInfo).CellIdentity.TableCellType == GridDataTableCellType.UnboundColumnHeaderCell)
                {
                    if (width > 250)
                    {
                        range.ColumnWidth = 255;//Maximun Width
                    }
                    else
                    {
                        range.ColumnWidth = width;
                    }
                }
            }
            else
            {
                range.ColumnWidth = width;
            }
        }

        /// <summary>
        /// Copies style from grid style into range style
        /// </summary>
        /// <param name="gridStyle">Grid style to copy</param>
        /// <param name="destRange">Destination range</param>
        protected void CopyStyle(GridStyleInfo gridStyle, IRange destRange, bool IsAsyncExport)
        {
            if (gridStyle == null)
            {
                throw new ArgumentNullException("gridStyle");
            }

            if (destRange == null)
            {
                throw new ArgumentNullException("destRange");
            }

            this.CopyBrush(gridStyle, destRange);
            this.CopyFont(gridStyle, destRange);
            this.CopyBorders(gridStyle, destRange, IsAsyncExport);
            this.CopyAlignment(gridStyle, destRange);
            ////this.SetNumberFormatIndex(destRange, gridStyle);
        }

        /// <summary>
        /// Copies font settings from grid cell into excel cell.
        /// </summary>
        /// <param name="gridCell">Source grid cell</param>
        /// <param name="range">Destination excel cell</param>
        private void CopyFont(GridStyleInfo gridCell, IRange range)
        {
            IStyle rangeStyle = range.CellStyle;
            IFont rangeFont = rangeStyle.Font;
            GridFontInfo font = gridCell.Font;
            rangeFont.FontName = font.FontFamily.ToString();
            rangeFont.Size = font.FontSize / 1.25 ;

            if (gridCell.Font.FontStyle.Equals(FontStyles.Italic))
            {
                rangeFont.Italic = true;
            }

            if (gridCell.Font.FontWeight.Equals(FontWeights.Bold))
            {
                rangeFont.Bold = true;
            }
#if !SILVERLIGHT
            // Strikethrough, underline
            if (gridCell.Font.TextDecorations != null)
            {
                foreach (TextDecoration textDecoration in gridCell.Font.TextDecorations)
                {
                    if (textDecoration.Location == TextDecorationLocation.Strikethrough)
                        rangeFont.Strikethrough = true;
                    if (textDecoration.Location == TextDecorationLocation.Underline)
                        rangeFont.Underline = ExcelUnderline.Single;
                    if (textDecoration.Location == TextDecorationLocation.Underline && textDecoration.Location == TextDecorationLocation.Baseline)
                        rangeFont.Underline = ExcelUnderline.Double;
                }
            }
#else
            // Strikethrough, underline
            if (gridCell.Font.TextDecorations != null)
            {
                gridCell.Font.TextDecorations.Equals(TextDecorations.Underline);
                    rangeFont.Underline = ExcelUnderline.Single;
            }
#endif

            if (gridCell.Foreground != null)
            {
#if !SILVERLIGHT
                //rangeFont.RGBColor = this.ColorFromString("#" + gridCell.Foreground.ToString().Substring(3));
                var color = (gridCell.Foreground as SolidColorBrush).Color;
                rangeFont.RGBColor = System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
#else
                rangeFont.RGBColor = this.GetColorFromBrush(gridCell.Foreground);
#endif
            }
            if (gridCell.NegativeForeground != null && gridCell.CellType == "DoubleEdit")
            {
                //This condition is checked for the cellvalue may come as null or empty and exception will throw while convert it to double.
                if (gridCell.CellValue != null && !string.IsNullOrEmpty(gridCell.CellValue.ToString()))
                {
                    // The Foreground color overrides only for Double edit cell has negative value and negative foreground value.
                    if (Convert.ToDouble(gridCell.CellValue) < 0)
                    {
#if !SILVERLIGHT
                        //rangeFont.RGBColor = this.ColorFromString("#" + gridCell.NegativeForeground.ToString().Substring(3));
                        var color = (gridCell.NegativeForeground as SolidColorBrush).Color;
                        rangeFont.RGBColor = System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
#else
                    rangeFont.RGBColor = this.GetColorFromBrush(gridCell.NegativeForeground);
#endif
                    }
                }
            }

           
        }

        /// <summary>
        /// Copies borders settings from grid cell into excel cell
        /// </summary>
        /// <param name="gridCell">Source grid cell</param>
        /// <param name="range">Destination excel cell</param>
        internal void CopyBorders(GridStyleInfo gridCell, IRange range, bool IsAsyncExport)
        {
            if (gridCell == null)
            {
                throw new ArgumentNullException("gridCell");
            }

            if (range == null)
            {
                throw new ArgumentNullException("range");
            }
#if !SILVERLIGHT
            if (IsAsyncExport)
            {
                System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
#endif
                    if (gridCell.Borders != null)
                    {
                        CellBordersInfo borders = gridCell.Borders;
                        IBorders rangeBorders = range.Borders;

#if !SILVERLIGHT
                        if (borders.Bottom != null)
                        {
                            this.CopyBorder(borders.Bottom, rangeBorders[ExcelBordersIndex.EdgeBottom], IsAsyncExport);
                        }

                        if (borders.Left != null)
                        {
                            this.CopyBorder(borders.Left, rangeBorders[ExcelBordersIndex.EdgeLeft], IsAsyncExport);
                        }

                        if (borders.Right != null)
                        {
                            this.CopyBorder(borders.Right, rangeBorders[ExcelBordersIndex.EdgeRight], IsAsyncExport);
                        }

                        if (borders.Top != null)
                        {
                            this.CopyBorder(borders.Top, rangeBorders[ExcelBordersIndex.EdgeTop], IsAsyncExport);
                        }
#else
                if (borders.Bottom != null)
                {
                    this.CopyBorder(borders.Bottom, rangeBorders[ExcelBordersIndex.EdgeBottom]);
                }

                if (borders.Left != null)
                {
                    this.CopyBorder(borders.Left, rangeBorders[ExcelBordersIndex.EdgeLeft]);
                }

                if (borders.Right != null)
                {
                    this.CopyBorder(borders.Right, rangeBorders[ExcelBordersIndex.EdgeRight]);
                }

                if (borders.Top != null)
                {
                    this.CopyBorder(borders.Top, rangeBorders[ExcelBordersIndex.EdgeTop]);
                }
#endif

                    }
#if !SILVERLIGHT
                }));
            }
            else
            {
                if (gridCell.Borders != null)
                {
                    CellBordersInfo borders = gridCell.Borders;
                    IBorders rangeBorders = range.Borders;

                    if (borders.Bottom != null)
                    {
                        this.CopyBorder(borders.Bottom, rangeBorders[ExcelBordersIndex.EdgeBottom], IsAsyncExport);
                    }

                    if (borders.Left != null)
                    {
                        this.CopyBorder(borders.Left, rangeBorders[ExcelBordersIndex.EdgeLeft], IsAsyncExport);
                    }

                    if (borders.Right != null)
                    {
                        this.CopyBorder(borders.Right, rangeBorders[ExcelBordersIndex.EdgeRight], IsAsyncExport);
                    }

                    if (borders.Top != null)
                    {
                        this.CopyBorder(borders.Top, rangeBorders[ExcelBordersIndex.EdgeTop], IsAsyncExport);
                    }
                }
            }
#endif
        }

        /// <summary>
        /// Copies border settings from grid border into excel border
        /// </summary>
        /// <param name="cellBorder">Grid border to copy</param>
        /// <param name="rangeBorder">Destination excel border</param>
#if !SILVERLIGHT
        internal void CopyBorder(System.Windows.Media.Pen cellBorder, IBorder rangeBorder, bool IsAsyncExport)
#else
        internal void CopyBorder(Pen cellBorder, IBorder rangeBorder)
#endif
        {
            if (cellBorder == null)
            {
                throw new ArgumentNullException("cellBorder");
            }

            if (rangeBorder == null)
            {
                throw new ArgumentNullException("rangeBorder");
            }

#if !SILVERLIGHT
            if (IsAsyncExport)
            {
                System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
#endif
            if (cellBorder.Brush != null)
            {
#if !SILVERLIGHT
                        var c = this.ColorFromString("#" + cellBorder.Brush.ToString().Substring(3));

                        rangeBorder.ColorRGB = c;

                        if (cellBorder.Thickness < 2)
                        {
                            rangeBorder.LineStyle = ExcelLineStyle.Thin;
                        }
                        else if (cellBorder.Thickness < 4)
                        {
                            rangeBorder.LineStyle = ExcelLineStyle.Medium;
                        }
                        else
                        {
                            rangeBorder.LineStyle = ExcelLineStyle.Thick;
                        }
#else
                var c = this.GetColorFromBrush(cellBorder.Brush);

                rangeBorder.ColorRGB = c;

                if (cellBorder.Style == BorderStyle.None)
                {
                    rangeBorder.LineStyle = ExcelLineStyle.None;
                }
                else if (cellBorder.Style == BorderStyle.Dotted)
                {
                    rangeBorder.LineStyle = ExcelLineStyle.Dotted;
                }
                else if (cellBorder.Style == BorderStyle.Dashed)
                {
                    rangeBorder.LineStyle = ExcelLineStyle.Dashed;
                }
                else if (cellBorder.Style == BorderStyle.DashDot)
                {
                    rangeBorder.LineStyle = ExcelLineStyle.Dash_dot;
                }
                else if (cellBorder.Style == BorderStyle.DashDotDot)
                {
                    rangeBorder.LineStyle = ExcelLineStyle.Dash_dot_dot;
                }
                else if (cellBorder.Style == BorderStyle.Standard)
                {
                    if (cellBorder.Thickness < 2)
                    {
                        rangeBorder.LineStyle = ExcelLineStyle.Thin;
                    }
                    else if (cellBorder.Thickness < 4)
                    {
                        rangeBorder.LineStyle = ExcelLineStyle.Medium;
                    }
                    else
                    {
                        rangeBorder.LineStyle = ExcelLineStyle.Thick;
                    }
                }
                else
                {
                    rangeBorder.LineStyle = ExcelLineStyle.Thick;
                }

#endif
            }
#if !SILVERLIGHT
                }), null);
            }
            else
            {
                if (cellBorder.Brush != null)
                {
#if !SILVERLIGHT
                    var c = this.ColorFromString("#" + cellBorder.Brush.ToString().Substring(3));
#else
                    var c = this.GetColorFromBrush(cellBorder.Brush);
#endif
                    rangeBorder.ColorRGB = c;

                    if (cellBorder.Thickness < 2)
                    {
                        rangeBorder.LineStyle = ExcelLineStyle.Thin;
                    }
                    else if (cellBorder.Thickness < 4)
                    {
                        rangeBorder.LineStyle = ExcelLineStyle.Medium;
                    }
                    else
                    {
                        rangeBorder.LineStyle = ExcelLineStyle.Thick;
                    }
                }
            }
#endif
        }

#if !SILVERLIGHT
        /// <summary>
        /// Get the color value from the string
        /// </summary>
        /// <param name="parseStr">string value</param>
        /// <returns>color value</returns>
        private System.Drawing.Color ColorFromString(string parseStr)
        {
            try
            {
                TypeConverter tc = TypeDescriptor.GetConverter(typeof(System.Drawing.Color));
                object o = tc.ConvertFrom(parseStr);

                if (o != null && o is System.Drawing.Color)
                {
                    return (System.Drawing.Color)o;
                }

                return System.Drawing.Color.Empty;
            }
            catch
            {
                return System.Drawing.Color.Empty;
            }
        }
#endif

        /// <summary>
        /// Copies brush settings from grid cell into excel cell
        /// </summary>
        /// <param name="gridCell">Source grid cell</param>
        /// <param name="range">Destination excel cell</param>
        private void CopyBrush(GridStyleInfo gridCell, IRange range)
        {
            IStyle rangeStyle = range.CellStyle;
            rangeStyle.FillPattern = ExcelPattern.Solid;

            if (gridCell.Foreground != null)
            {
#if !SILVERLIGHT
                var color = (gridCell.Foreground as SolidColorBrush).Color;
                rangeStyle.PatternColor = System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
#else
                var color = this.GetColorFromBrush(gridCell.Foreground);
                rangeStyle.PatternColor = color;
#endif
            }

            if (gridCell.Background != null)
            {
                if (gridCell.Background is System.Windows.Media.SolidColorBrush)
                {
#if !SILVERLIGHT
                    var color = (gridCell.Background as SolidColorBrush).Color;
                    rangeStyle.FillBackgroundRGB = System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
#else
                    var color = this.GetColorFromBrush(gridCell.Background);
                    rangeStyle.FillBackgroundRGB = color;
#endif
                    rangeStyle.Interior.FillPattern = ExcelPattern.Solid;
                }
                else if (gridCell.Background is System.Windows.Media.LinearGradientBrush)
                {
                    if (range.Worksheet.Workbook.Version == ExcelVersion.Excel2007)
                    {
                        System.Windows.Media.LinearGradientBrush linearGradient = gridCell.Background as System.Windows.Media.LinearGradientBrush;
                        rangeStyle.Interior.FillPattern = ExcelPattern.Gradient;
#if !SILVERLIGHT
                        var color = linearGradient.GradientStops[0].Color;
                        rangeStyle.Interior.Gradient.BackColor = System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
#else
                        var color = this.GetColorFromBrush(gridCell.Background);
                        rangeStyle.Interior.Gradient.BackColor = color;
#endif
                    }
                    else
                    {
                        System.Windows.Media.LinearGradientBrush linearGradient = gridCell.Background as System.Windows.Media.LinearGradientBrush;
#if !SILVERLIGHT
                        var color = linearGradient.GradientStops[0].Color;
                        rangeStyle.Color = System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
#else
                        var color = this.GetColorFromBrush(gridCell.Background);
                        rangeStyle.Color = color;
#endif
                    }
                }
            }
        }

        /// <summary>
        /// Copies alignment settings from grid cell into excel cell
        /// </summary>
        /// <param name="gridCell">Source grid cell</param>
        /// <param name="range">Destination excel cell</param>
        private void CopyAlignment(GridStyleInfo gridCell, IRange range)
        {
            IStyle rangeStyle = range.CellStyle;
            range.Worksheet.IsGridLinesVisible = false;
            rangeStyle.HorizontalAlignment = this.GetClosestExcelHorizontalAlignment(gridCell.HorizontalAlignment);
            rangeStyle.VerticalAlignment = this.GetClosestExcelVerticalAlignment(gridCell.VerticalAlignment);
            //rangeStyle.WrapText = true;
            //SD8520 - Text Wrap/NoWrap while exporting grid to excel.
#if !SILVERLIGHT
            rangeStyle.WrapText = gridCell.TextWrapping == TextWrapping.Wrap || gridCell.TextWrapping == TextWrapping.WrapWithOverflow ? true : false;
#else
             rangeStyle.WrapText = gridCell.TextWrapping == TextWrapping.Wrap ? true : false;
#endif
        }

        /// <summary>
        /// Converts GridHorizontalAlignment into ExcelHAlign.
        /// </summary>
        /// <param name="gridHAlign">GridHorizontalAlignment to convert</param>
        /// <returns>Corresponding ExcelHAlign</returns>
        private ExcelHAlign GetClosestExcelHorizontalAlignment(System.Windows.HorizontalAlignment gridHAlign)
        {
            switch (gridHAlign)
            {
                case System.Windows.HorizontalAlignment.Center: return ExcelHAlign.HAlignCenter;
                case System.Windows.HorizontalAlignment.Left: return ExcelHAlign.HAlignLeft;
                case System.Windows.HorizontalAlignment.Right: return ExcelHAlign.HAlignRight;
                case System.Windows.HorizontalAlignment.Stretch: return ExcelHAlign.HAlignJustify;
                default: throw new ArgumentOutOfRangeException("gridHAling");
            }
        }

        /// <summary>
        /// Converts GridVerticalAlignment into ExcelVAlign
        /// </summary>
        /// <param name="gridVAlign">GridVerticalAlignment to convert</param>
        /// <returns>Corresponding ExcelVAlign</returns>
        private ExcelVAlign GetClosestExcelVerticalAlignment(VerticalAlignment gridVAlign)
        {
            switch (gridVAlign)
            {
                case VerticalAlignment.Bottom: return ExcelVAlign.VAlignBottom;
                case VerticalAlignment.Center: return ExcelVAlign.VAlignCenter;
                case VerticalAlignment.Top: return ExcelVAlign.VAlignTop;
                case VerticalAlignment.Stretch: return ExcelVAlign.VAlignDistributed;
                default: throw new ArgumentOutOfRangeException("gridVAlign");
            }
        }

        /// <summary>
        /// Copies number format from excel style into grid cell
        /// </summary>
        /// <param name="range">Range to add format to</param>
        /// <param name="gridStyle">Style to get number format from</param>
        private void SetNumberFormatIndex(IRange range, GridStyleInfo gridStyle)
        {
            if (gridStyle.CellType != "CurrencyEdit" && gridStyle.CellType != "DateTimeEdit")
            {
                int formatIndex = this.ConvertFormatFromGridToExcel(range.Worksheet.Workbook, gridStyle.Format);
                range.CellStyle.NumberFormatIndex = formatIndex;
            }
        }

        /// <summary>
        /// Converts grid number format into excel number format
        /// </summary>
        /// <param name="book">Workbook Object</param>
        /// <param name="strGridFormat">Grid format to convert</param>
        /// <returns>Index to the excel format</returns>
        private int ConvertFormatFromGridToExcel(IWorkbook book, string strGridFormat)
        {
            if (strGridFormat == null || strGridFormat.Length == 0)
            {
                return DEFEXCELGENERALFORMAT;
            }

            switch (char.ToUpper(strGridFormat[0]))
            {
                case DEFCHARGENERAL:
                    return DEFEXCELGENERALFORMAT;

                case DEFCHARCURRENCY:
                    return DEFEXCELCURRENCYFORMAT;

                case DEFCHARDATE:
                    return DEFEXCELDATEFORMAT;

                case DEFCHARSCIENTIFIC:
                    return DEFEXCELSCIENTIFICFORMAT;

                case DEFCHARFIXEDPOINT:
                    return DEFEXCELFIXEDPOINTFORMAT;

                case DEFCHARNUMBER:
                    return DEFEXCELNUMBERFORMAT;

                case DEFCHARPERCENT:
                    return DEFEXCELPERCENTFORMAT;

                case DEFCHARHEX:
                    return DEFEXCELHEXFORMAT;

                default:
                    {
                        WorkbookImpl workbook = (WorkbookImpl)book;
                        int index = workbook.InnerFormats.CreateFormat(strGridFormat);
                        return index;
                    }
            }
        }

#if SILVERLIGHT
        private Color GetColorFromBrush(Brush brush)
        {
            Color c = Colors.Transparent;
            if (brush is SolidColorBrush)
            {
                var solidColorBrush = brush as SolidColorBrush;
                c = solidColorBrush.Color;
            }
            else if (brush is LinearGradientBrush)
            {
                var linearBrush = brush as LinearGradientBrush;
                c = linearBrush.GradientStops[0].Color;
            }

            return c;
        }
#endif
    }
}