#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if !WinRT
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using Syncfusion.Windows.ComponentModel;
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.Controls.Scroll;
using Syncfusion.Windows.Styles;
using System.Windows.Controls;

namespace Syncfusion.Windows.Controls.Grid
#else
using Syncfusion.WinRT.ComponentModel;
using Syncfusion.WinRT.Controls.Cells;
using Syncfusion.WinRT.Controls.Scroll;
using Syncfusion.WinRT.Styles;
using System;
using System.Globalization;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Syncfusion.WinRT.Controls.Grid
#endif
{

    // TODO: Maybe support cell renderers that are not bound
    // to any model?

    //public interface IGridModelBound
    //{
    //    GridModel GridModel { get; set; }
    //}

#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridCellModelBase : Disposable
    {
        GridModel gridModel;

        public GridCellModelBase()
        {
        }

        public GridModel GridModel
        {
            get { return gridModel; }
            set { gridModel = value; }
        }

        public virtual void OnCreated()
        {
        }

        public virtual IGridCellRenderer CreateRenderer()
        {
            throw new NotImplementedException("GridCellRenderer is not implemented in derived class");
            //return null;
        }

        IGridCellRenderer activeRenderer;

        public IGridCellRenderer ActiveRenderer
        {
            get { return activeRenderer; }
            set { activeRenderer = value; }
        }

        #region CellText

        public virtual string GetFormattedText(GridStyleInfo style, object value, int textInfo)
        {
            GridCellTextEventArgs ea = new GridCellTextEventArgs("", style, value, textInfo);
            style.GridModel.RaiseQueryCellFormattedText(ea);
            if (ea.Handled)
                return ea.Text;
            else
            {
                CultureInfo ci = style.CultureInfo;
                NumberFormatInfo nfi = ci != null ? ci.NumberFormat : null;
                //GridNumberFormatInfoStyleProperty ognfi = null;
                // TODO: the following code costs immense performance !
                //			ognfi = ((GridNumberFormatInfoStyleProperty) CustomStyleProperties[typeof(GridNumberFormatInfoStyleProperty)]);
                //			if (ognfi != null)
                //				nfi = (NumberFormatInfo) ognfi.GetFormat(typeof(NumberFormatInfo));
                return ValueConvert.FormatValue(value, style.CellValueType, style.Format, ci, nfi, style.FormatProvider);
            }
        }
        internal virtual string GetFormulaValue(GridStyleInfo style, object value, GridFormulaTag tag)
        {
            CultureInfo ci = style.CultureInfo;
            NumberFormatInfo nfi = ci != null ? ci.NumberFormat : null;
            return ValueConvert.FormatValue(value, style.CellValueType, style.Format, ci, nfi, style.FormatProvider);
        }

        /// <summary>
        /// Parses the display text and converts it into a cell value to be stored in the style object.
        /// GridStyleInfo.CultureInfo is used for parsing the string.
        /// </summary>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="text">The input text to be parsed.</param>
        /// <param name="textInfo">TextInfo is a hint of who is calling, default is GridCellBaseTextInfo.DisplayText</param>
        /// <returns>True if value was parsed correctly and saved in style object as <see cref="GridStyleInfo.CellValue"/>; False otherwise.</returns>
        public virtual bool ApplyFormattedText(GridStyleInfo style, string text, int textInfo)
        {
            GridCellTextEventArgs ea = new GridCellTextEventArgs(text, style, null, textInfo);
            style.GridModel.RaiseSaveCellFormattedText(ea);

            if (!ea.Handled)
            {
                style.GridModel.RaiseParseCommonFormats(ea);
            }

            if (!ea.Handled)
            {
                CultureInfo ci = style.GetCulture(true);
                IFormatProvider nfi = style.CellValueType == typeof(DateTime)
                    ? (IFormatProvider)ci.DateTimeFormat
                    : (IFormatProvider)ci.NumberFormat;

                try
                {
                    style.BeginUpdate();
                    Type valuetype = null;
                    if (style.CellValue != null && string.IsNullOrEmpty(style.CellValue.ToString()))
                        valuetype = style.CellValueType == null
                            ? style.CellValue.GetType()
                            : style.CellValueType;
#if!WinRT
                    if (valuetype == typeof(DBNull))
                        valuetype = null;
#endif
                    style.CellValue = style.ParseFormats == null
                                      ? ValueConvert.Parse(text, valuetype, nfi, style.Format)
                                      : ValueConvert.Parse(text, valuetype, nfi, style.ParseFormats, false);
                    style.ResetError();
                }

                catch (Exception ex)
                {
                    style.Error = ex.Message;
                    if (style.StrictValueType)
                        throw;

                    else if (ex is FormatException || ex.InnerException is FormatException)
                    {
                        style.CellValue = text;
                        // Possibly could also change CellValueType here based on input string.
                        // e.Style.CellValueType = typeof(string);
                    }
                    else
                        throw;
                }
                finally
                {
                    style.EndUpdate();
                }
            }
            return true;
        }

        public virtual string GetText(GridStyleInfo style, object value)
        {
            GridCellTextEventArgs ea = new GridCellTextEventArgs("", style, value, -1);
            style.GridModel.RaiseQueryCellText(ea);
            if (ea.Handled)
                return ea.Text;
            if (value == null)
                return "";
            return (string)ValueConvert.ChangeType(value, typeof(string), CultureInfo.CurrentCulture);
        }

        public virtual bool ApplyText(GridStyleInfo style, string text)
        {
            GridCellTextEventArgs ea = new GridCellTextEventArgs(text, style, null, -1);
            style.GridModel.RaiseSaveCellText(ea);
            if (!ea.Handled)
            {
                CultureInfo ci = CultureInfo.CurrentCulture;
                NumberFormatInfo nfi = ci != null ? ci.NumberFormat : null;
                try
                {
                    style.BeginUpdate();
                    style.CellValue = ValueConvert.Parse(text, style.CellValueType, nfi, "");
                    style.ResetError();
                }
                catch (Exception ex)
                {
                    style.Error = ex.Message;
                    if (style.StrictValueType)
                        throw;

                    else if (ex is FormatException || ex.InnerException is FormatException)
                    {
                        style.CellValue = text;
                        // Possibly could also change CellValueType here based on input string.
                        // e.Style.CellValueType = typeof(string);
                    }
                    else
                        throw;
                }
                finally
                {
                    style.EndUpdate();
                }
            }
            return true;
        }

        #endregion

#if !WinRT
        public virtual PropertyDescriptor GetPropertyDescriptor(GridStyleInfo style)
        {
            return style.PropertyDescriptor;
        }
#endif

        //public virtual TypeConverter GetTypeConverter(GridStyleInfo style)
        //{
        //    PropertyDescriptor pd = GetPropertyDescriptor(style);
        //    if (pd != null)
        //        return pd.Converter;

        //    Type type = style.CellValueType;
        //    if (type != null)
        //        return TypeDescriptor.GetConverter(type);

        //    return null;
        //}

        public void RaiseCurrentCellContentChanged()
        {
        }

        public virtual GridCellModelBase Clone()
        {
            return (GridCellModelBase)MemberwiseClone();
        }


        /// <overload>
        /// Calculates the preferred size of the cell based on its contents, including margins and any buttons.
        /// </overload>
        /// <summary>
        /// Calculates the preferred size of the cell based on its contents, including margins and any buttons.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> context of the canvas.</param>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="queryBounds">Vertical or horizontal</param>
        /// <returns>The optimal size of the cell.</returns>
        public virtual Size CalculatePreferredCellSize(int rowIndex, int colIndex, GridStyleInfo style, GridQueryBounds queryBounds)
        {
            Size clientSize = OnQueryPrefferedClientSize(rowIndex, colIndex, style, queryBounds);
            if (clientSize.IsEmpty)
                return Size.Empty;

            Size cellSize = AddBorderMargins(clientSize, style.BorderMargins.ToThickness());
            cellSize = AddBorderMargins(cellSize, style.TextMargins.ToThickness());
            cellSize.Width += 2;
            cellSize.Height += 2;
            //cellSize.Width += ButtonBarSize.Width;
            //int buttonHeight = ButtonBarSize.Height;
            //if (buttonHeight > 0 && buttonHeight < int.MaxValue)
            //    cellSize.Height = Math.Max(cellSize.Height, buttonHeight);
            return cellSize;
        }

        /// <summary>
        /// Add border margins to get cells client area.
        /// </summary>
        /// <param name="cellRect"></param>
        /// <param name="mi"></param>
        /// <returns></returns>
        public Size AddBorderMargins(Size cellRect, Thickness mi)
        {
            cellRect.Height += mi.Bottom + mi.Top;
            cellRect.Width += mi.Right + mi.Left;

            return cellRect;
        }
        /// <summary>
        /// Remove border margins to get cells client area.
        /// </summary>
        /// <param name="cellRect"></param>
        /// <param name="mi"></param>
        /// <returns></returns>
        public Size RemoveBorderMargins(Size cellRect, Thickness mi)
        {
            if ((cellRect.Height > mi.Bottom + mi.Top) && (cellRect.Width > mi.Right + mi.Left))
            {
                cellRect.Height -= mi.Bottom + mi.Top;
                cellRect.Width -= mi.Right + mi.Left;
            }

            return cellRect;
        }

        /// <summary>
        /// Calculates the preferred size of the cell based on its contents without margins and any buttons.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> context of the canvas.</param>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="queryBounds"></param>
        /// <returns>The optimal size of the cell.</returns>
        protected virtual Size OnQueryPrefferedClientSize(int rowIndex, int colIndex, GridStyleInfo style, GridQueryBounds queryBounds)
        {
            string text = style.FormattedText;

            if (string.IsNullOrEmpty(text))
                text = MeasureEmptyCellString;

            Size clientSize = GetCellClientSize(rowIndex, colIndex, style);

            Size textSize = this.MeasureText(clientSize, text, style, queryBounds); //clientSize;// GridTextBoxPaint.MeasureText(clientSize, text, style, queryBounds);

            return textSize;
        }


        private Size MeasureText(Size clientSize, string text, GridStyleInfo style, GridQueryBounds queryBounds)
        {
            var font = style.ReadOnlyFont;
            var textBlock = new TextBlock()
            {
                FontSize = font.FontSize,
                FontFamily = font.FontFamily,
                Text = text,
                TextWrapping = style.TextWrapping,
                TextTrimming = style.TextTrimming,
                FontStretch = font.FontStretch,
                FontWeight = font.FontWeight,
                FontStyle = font.FontStyle,
                HorizontalAlignment = style.HorizontalAlignment,
                Margin = style.TextMargins.ToThickness(),
                Padding = style.BorderMargins.ToThickness(),
#if !WinRT
                TextDecorations = font.TextDecorations,
#endif
                VerticalAlignment = style.VerticalAlignment
            };
            bool wrapText = style.TextWrapping != TextWrapping.NoWrap;
            var parentBorder = new Border() { Child = textBlock };
            if (queryBounds == GridQueryBounds.Height)
            {
                textBlock.MaxWidth = clientSize.Width;
                var totalHeight = this.GridModel.RowHeights.TotalExtent;
                double calculatedWidth = (textBlock.ActualWidth + textBlock.Padding.Right + textBlock.Padding.Left);
                //Previous Code if (wrapText) && (calculatedWidth) > clientSize.Width)
                //This code have changed like follows.Because If calculatedWidth less than clientSize.Width then this loop doent execute even WrapText==true.
                
                if (wrapText) 
                {
                    if ((calculatedWidth) > clientSize.Width)
                    {
                        var mod = calculatedWidth % clientSize.Width;
                        var increment = (int)(calculatedWidth / (clientSize.Width - style.TextMargins.Left));
                        textBlock.Height = textBlock.ActualHeight * (increment + 1 + (mod > 0 ? 1 : 0));
                    }
                    else
                        textBlock.Height = textBlock.ActualHeight; 
                    textBlock.Width = clientSize.Width;
                    return new Size(textBlock.Width, textBlock.Height);
                }
                parentBorder.MaxHeight = totalHeight;
                textBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                parentBorder.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                parentBorder.Arrange(new Rect(0, 0, textBlock.ActualWidth, textBlock.ActualHeight));
                // very odd, when query is for row heights, then just do arrange/measure on the parent border and then return the textblock size
                var reservedTextSize = new Size(textBlock.ActualWidth, textBlock.ActualHeight);
                return reservedTextSize;
            }
            else
            {
                textBlock.MaxHeight = clientSize.Height;
                var totalWidth = this.GridModel.ColumnWidths.TotalExtent;
                parentBorder.MaxWidth = totalWidth;
                var reservedTextSize = new Size(textBlock.ActualWidth, textBlock.ActualHeight);
                textBlock.Measure(new Size(double.MaxValue, double.MaxValue));
                parentBorder.Measure(new Size(double.MaxValue, double.MaxValue));
                parentBorder.Arrange(new Rect(0, 0, textBlock.ActualWidth, textBlock.ActualHeight));
                if (parentBorder.ActualWidth > 0 && parentBorder.ActualHeight > 0)
                {
                    // very odd, when query is for column widths, return the parent border's actual width and height
                    var finalSize = new Size(parentBorder.ActualWidth, parentBorder.ActualHeight);
                    return finalSize;
                }

                return reservedTextSize;
            }
        }


        /// <summary>
        /// This string is used when doing a resize to fit for cells with empty text.
        /// </summary>
        public static string MeasureEmptyCellString
        {
            get
            {
                return measureEmptyCellString;
            }
            set
            {
                measureEmptyCellString = value;
            }
        }

        static string measureEmptyCellString = "Wg";

        /// <summary>
        /// Gets the actual size of the cell including margins and cell buttons. 
        /// Spanned cells will return the size of the whole cell
        /// covering neighboring cells.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <returns>The actual <see cref="Size"/> of the cell including margins and cell buttons. </returns>
        public Size GetCellSize(int rowIndex, int colIndex)
        {
            CoveredCellInfo cc = gridModel.CoveredCells.GetCellSpan(rowIndex, colIndex);
            if (cc == null)
                cc = new CoveredCellInfo(rowIndex, colIndex, rowIndex, colIndex);
            double w = LineSizeUtil.GetTotal(gridModel.ColumnWidths, cc.Left, cc.Right);
            double h = LineSizeUtil.GetTotal(gridModel.RowHeights, cc.Top, cc.Bottom);
            return new Size(w, h);
        }

        /// <summary>
        /// Gets the actual size of the cell without margins and without cell buttons. 
        /// Spanned cells will return the size of the whole cell
        /// covering neighboring cells.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <returns>The actual <see cref="Size"/> of the cell without margins or cell buttons.</returns>
        public Size GetCellClientSize(int rowIndex, int colIndex, GridStyleInfo style)
        {
            Size size = GetCellSize(rowIndex, colIndex);
            size = RemoveBorderMargins(size, style.BorderMargins.ToThickness());
            //size.Width -= ButtonBarSize.Width;
            return size;
        }

    }


    /// <summary>
    /// Used by <see cref="GridCellModelBase.CalculatePreferredCellSize"/>.
    /// </summary>
    public enum GridQueryBounds
    {
        /// <summary>
        /// Queries height of cell.
        /// </summary>
        Height,

        /// <summary>
        /// Queries width of cell.
        /// </summary>
        Width
    }
}
