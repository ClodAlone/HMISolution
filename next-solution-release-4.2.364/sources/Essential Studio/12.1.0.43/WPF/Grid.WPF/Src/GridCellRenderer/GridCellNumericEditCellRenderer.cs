#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Grid
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Syncfusion.Windows.Shared;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Input;
    using Syncfusion.Windows.Controls.Scroll;

    /// <summary>
    /// Implements the model part of a numeric edit cell.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GridCellNumericEditCellModel<T> : GridCellModel<T>
        where T : IGridCellRenderer, new()
    {
        /// <summary>
        /// Parses the display text and converts it into a cell value to be stored in the style object.
        /// </summary>
        /// <param name="style">Style information for the cell.</param>
        /// <param name="text">The input text to be parsed.</param>
        /// <returns>True if value was parsed correctly and saved in style object as <see cref="GridStyleInfo.CellValue"/>; False otherwise.</returns>
        public override bool ApplyFormattedText(GridStyleInfo style, string text, int textInfo)
        {
            var cellValue = this.ApplyFormattedValue(style, text);
            if (cellValue != null)
            {
                style.CellValue = cellValue;
                return true;
            }
            return false;
        }

        protected virtual object ApplyFormattedValue(GridStyleInfo style, string text)
        {
            if (text == string.Empty)
            {
                return 0;
            }

            return text;
        }

        /// <summary>
        /// This is called from GridStyleInfo.GetText (ignoring any <see cref="GridStyleInfo.Format"/> settings).
        /// </summary>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="value">The value to convert to a string.</param>
        /// <returns>The string that represents the given value.</returns>
        public override string GetText(GridStyleInfo style, object value)
        {
            return (value != null && !(value is DBNull)) ? value.ToString() : string.Empty;
        }

        /// <summary>
        /// Returns formatted text for the specified value.
        /// </summary>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="value">The value to format.</param>
        /// <param name="textInfo">TextInfo is a hint of who is calling, default is GridCellBaseTextInfo.DisplayText.</param>
        /// <returns>The formatted text for the given value.</returns>
        public override string GetFormattedText(GridStyleInfo style, object value, int textInfo)
        {
            if (!style.HasFormat)
            {
                string text = this.GetText(style, value);
                //var formattedText = base.GetFormattedText(style, value, textInfo);
                var numberFormat = style.HasNumberFormat ? style.NumberFormat : style.GetCulture(false).NumberFormat;
                if (style.NumberFormat != null && style.NumberFormat != style.GetCulture(false).NumberFormat)
                {
                    numberFormat = style.NumberFormat;
                }
                else
                {
                    numberFormat = style.GetCulture(false).NumberFormat;
                }
                var formattedText = this.GetFormattedText(text, numberFormat);
                return formattedText;
            }
            return base.GetFormattedText(style, value, textInfo);
        }

        protected virtual string GetFormattedText(string text, NumberFormatInfo numberFormatInfo)
        {
            return text;
        }
    }
}
