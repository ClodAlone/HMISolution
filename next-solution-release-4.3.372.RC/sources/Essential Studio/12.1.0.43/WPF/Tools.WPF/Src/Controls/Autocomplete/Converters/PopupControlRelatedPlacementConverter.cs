// <copyright file="PopupControlRelatedPlacementConverter.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>
using System;
using System.Globalization;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace Syncfusion.Windows.Tools
{
    /// <summary>
    /// Converts <see cref="Syncfusion.Windows.Tools.PopupPlacement"/> objects
    /// to <see cref="System.Windows.Controls.Primitives.PlacementMode"/> objects.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif

    public class PopupControlRelatedPlacementConverter : IValueConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PopupControlRelatedPlacementConverter"/> class.
        /// </summary>
        public PopupControlRelatedPlacementConverter()
        {
        }

        #region IValueConverter Members

        /// <summary>
        /// Converts <see cref="Syncfusion.Windows.Tools.PopupPlacement"/> objects
        /// to <see cref="System.Windows.Controls.Primitives.PlacementMode"/> objects.
        /// </summary>
        /// <param name="value">The <see cref="Syncfusion.Windows.Tools.PopupPlacement"/> object to be converted.</param>
        /// <param name="targetType">The type of the binding data target property.</param>
        /// <param name="parameter">Does not matter.</param>
        /// <param name="culture">Currently used culture.</param>
        /// <returns>A converted value.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            PopupPlacement? placement = value as PopupPlacement?;
            if (placement == null)
            {
                throw new ArgumentException("Incorrect value type is used.");
            }

            PlacementMode mode = PlacementMode.Bottom;

            if (placement == PopupPlacement.Top)
            {
                mode = PlacementMode.Top;
            }
            else if (placement == PopupPlacement.Right)
            {
                mode = PlacementMode.Right;
            }
            else if (placement == PopupPlacement.Left)
            {
                mode = PlacementMode.Left;
            }

            return mode;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding data target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new Exception("It's only one way converter.");
        }

        #endregion IValueConverter Members
    }
}