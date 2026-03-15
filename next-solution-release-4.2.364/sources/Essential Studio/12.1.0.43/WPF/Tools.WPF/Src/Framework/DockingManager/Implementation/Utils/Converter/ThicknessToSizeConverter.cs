// <copyright file="ThicknessToSizeConverter.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// This class converts thickness of the window to size.
    /// </summary>
    /// <exclude/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ThicknessToSizeConverter : IValueConverter
    {
        #region Constants
        /// <summary>
        /// Specifies the up horizontal letter.
        /// </summary>
        private const char C_UPHorizontalLetter = 'H';

        /// <summary>
        /// Specifies the down horizontal letter.
        /// </summary>
        private const char C_LOWHorizontalLetter = 'h';

        /// <summary>
        /// Specifies the up vertical letter.
        /// </summary>
        private const char C_UPVerticalLetter = 'V';

        /// <summary>
        /// Specifies the low vertical letter.
        /// </summary>
        private const char C_LOWVerticalLetter = 'v';
        #endregion

        #region Implemenation
        /// <summary>
        /// Converts thickness of the window to size.
        /// </summary>
        /// <param name="value">Value to be converted.</param>
        /// <param name="targetType">Type, the value is to be converted to.</param>
        /// <param name="parameter">Does not matter.</param>
        /// <param name="culture">Currently used culture. Not used here.</param>
        /// <returns>Converted value.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double result = 0;
            Thickness thickness = (Thickness)value;
            string strParemetr = parameter.ToString();

            if (!string.IsNullOrEmpty(strParemetr))
            {
                char firstLetter = strParemetr[0];
                strParemetr = strParemetr.Remove(0, 1);
                double.TryParse(strParemetr, out result);

                switch (firstLetter)
                {
                    case C_UPHorizontalLetter:
                    case C_LOWHorizontalLetter:
                        result += thickness.Top + thickness.Bottom;
                        break;

                    case C_UPVerticalLetter:
                    case C_LOWVerticalLetter:
                        result += thickness.Left + thickness.Right;
                        break;

                    default:
                        throw new ArgumentException("Incorrect parameter set in Binding");
                }
            }

            return result;
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>Rises exception.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }
        #endregion
    }
}
