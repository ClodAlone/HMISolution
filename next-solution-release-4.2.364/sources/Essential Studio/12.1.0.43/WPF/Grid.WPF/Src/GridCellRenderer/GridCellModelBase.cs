#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using Syncfusion.Windows.ComponentModel;
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.Controls.Scroll;
using Syncfusion.Windows.Styles;

namespace Syncfusion.Windows.Controls.Grid
{

    // TODO: Maybe support cell renderers that are not bound
    // to any model?

    //public interface IGridModelBound
    //{
    //    GridModel GridModel { get; set; }
    //}

    /// <summary>
    /// Defines the model for a cell type.
    /// </summary>
    /// <remarks>
    /// You typically access cell models through the <see cref="GridModel.CellModels"/>
    /// property of the <see cref="GridModel"/> class.<para/>
    /// </remarks>
    public class GridCellModelBase : Disposable
    {
        GridModel gridModel;

        /// <summary>
        /// Initializes a new <see cref="GridCellModelBase"/>.
        /// </summary>
        public GridCellModelBase()
        {
        }

        /// <summary>
        /// Gets or sets the grid model for the cell.
        /// </summary>
        public GridModel GridModel
        {
            get { return gridModel; }
            set { gridModel = value; }
        }

        /// <summary>
        /// Occurs when the cell model is created.
        /// </summary>
        public virtual void OnCreated()
        {
        }

        /// <summary>
        /// Creates cell renderer.
        /// </summary>
        /// <returns>Throws Not Implemented exception.</returns>
        /// <remarks>You must override this method in your implementation of GridCellModelBase.</remarks>
        public virtual IGridCellRenderer CreateRenderer()
        {
            throw new NotImplementedException("GridCellRenderer is not implemented in derived class");
            //return null;
        }

        IGridCellRenderer activeRenderer;

        /// <summary>
        /// Returns the active cell renderer object.
        /// </summary>
        public IGridCellRenderer ActiveRenderer
        {
            get { return activeRenderer; }
            set { activeRenderer = value; }
        }

        #region CellText
        /// <summary>
        /// This is called from GridStyleInfo.GetFormattedText.
        /// GridStyleInfo.CultureInfo is used for conversion to string.
        /// </summary>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="value">The value to format.</param>
        /// <param name="textInfo">TextInfo is a hint of who is calling, default is GridCellBaseTextInfo.DisplayText.</param>
        /// <returns>The formatted text for the given value.</returns>
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
                if (nfi == null)
                {
                    nfi = style.NumberFormat != null ? style.NumberFormat : null;
                }
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
                        valuetype = style.CellValueType == null ? style.CellValue.GetType() : style.CellValueType;

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

        /// <summary>
        /// This is called from GridStyleInfo.GetText (ignoring any <see cref="GridStyleInfo.Format"/> settings).
        /// CultureInfo.CurrentText is used for conversion to string.
        /// </summary>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="value">The value to convert to a string.</param>
        /// <returns>The string that represents the given value.</returns>
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

        /// <summary>
        /// Parses the text and converts it into a cell value to be stored in the style object (ignoring any <see cref="GridStyleInfo.Format"/> settings).
        /// CultureInfo.CurrentText is used for parsing the string.
        /// </summary>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="text">The input text to be parsed.</param>
        /// <returns>True if value was parsed correctly and saved in style object as <see cref="GridStyleInfo.CellValue"/>; False otherwise.</returns>
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

        /// <summary>
        /// Returns GridStyleInfo.PropertyDescriptor.
        /// </summary>
        /// <param name="style">The style object</param>
        /// <returns>A PropertyDescriptor</returns>
        public virtual PropertyDescriptor GetPropertyDescriptor(GridStyleInfo style)
        {
            return style.PropertyDescriptor;
        }

        /// <summary>
        /// Returns a TypeConverter with type information about the style.CellValue.
        /// </summary>
        /// <param name="style">The style object</param>
        /// <returns>A TypeConverter</returns>
        public virtual TypeConverter GetTypeConverter(GridStyleInfo style)
        {
            PropertyDescriptor pd = GetPropertyDescriptor(style);
            if (pd != null)
                return pd.Converter;

            Type type = style.CellValueType;
            if (type != null)
                return TypeDescriptor.GetConverter(type);

            return null;
        }

        /// <summary>
        /// Signifies the current cell change.
        /// </summary>
        public void RaiseCurrentCellContentChanged()
        {
        }

        /// <summary>
        /// Creates a shallow copy of current <see cref="GridCellModelBase"/> object.
        /// </summary>
        /// <returns>A copy of current object.</returns>
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
            var margins = style.TextMargins.ToThickness();
            if (style.HasImageIndex)
            {
                margins = style.AdjustImageWidthAndHeightToMargin(margins, clientSize);
            }
            else
            {
                margins = style.ErrorInfo.AdjustErrorInfoMargin(margins, clientSize);
            }
            cellSize = AddBorderMargins(cellSize, margins);
            cellSize.Width = Math.Ceiling(cellSize.Width + 2);
            cellSize.Height = Math.Ceiling(cellSize.Height + 2);

            //cellSize.Width += ButtonBarSize.Width;
            //int buttonHeight = ButtonBarSize.Height;
            //if (buttonHeight > 0 && buttonHeight < int.MaxValue)
            //    cellSize.Height = Math.Max(cellSize.Height, buttonHeight);
            return cellSize;
        }

        /// <summary>
        /// Add border margins to get cells client area.
        /// </summary>
        /// <param name="cellRect">The cell rectangle</param>
        /// <param name="mi">Margins.</param>
        /// <returns>Size of the cell client area.</returns>
        public Size AddBorderMargins(Size cellRect, Thickness mi)
        {
            cellRect.Height += mi.Bottom + mi.Top;
            cellRect.Width += mi.Right + mi.Left;

            return cellRect;
        }
        /// <summary>
        /// Remove border margins to get cells client area.
        /// </summary>
        /// <param name="cellRect">The cell rectangle.</param>
        /// <param name="mi">Border margins</param>
        /// <returns>The cell client area.</returns>
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

            Size textSize = GridTextBoxPaint.MeasureText(clientSize, text, style, queryBounds);

            if (style.CellType == "ImageCell")
            {
                textSize = new Size { Width = style.HasImageWidth ? style.GetImageWidth() : textSize.Width, Height = style.HasImageHeight ? style.GetImageHeight() : textSize.Height };
            }

            return textSize;
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
        /// Gets the actual size of the cell including margins. 
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
        /// Gets the actual size of the cell without margins. 
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

            Thickness margins = style.TextMargins.ToThickness();
            if (style.HasImageIndex)
                margins = style.AdjustImageWidthAndHeightToMargin(margins, size);
            else
                margins = style.ErrorInfo.AdjustErrorInfoMargin(margins, size);

            margins.Left = Math.Max(margins.Left, 2);
            margins.Right = Math.Max(margins.Right, 2);

            if (style.HorizontalAlignment == HorizontalAlignment.Left && style.ErrorInfo.HasErrorMessage && style.ErrorInfo.ErrorContentAlignment == ImageContentAlignment.Left)
            {
                margins.Left = 20;
            }
            size = RemoveBorderMargins(size, margins);
            size = RemoveBorderMargins(size, style.Padding.ToThickness());
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
