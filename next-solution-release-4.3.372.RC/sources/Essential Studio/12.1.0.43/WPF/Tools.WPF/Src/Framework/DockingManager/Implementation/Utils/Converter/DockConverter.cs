// <copyright file="DockConverter.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// This class converts form DockSide to Dock and from Dock to DockSide.
    /// </summary>
    /// <exclude/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    [ValueConversion(typeof(DockSide), typeof(Dock))]
    public class DockConverter : IValueConverter
    {
        /// <summary>
        /// Converts from dock side to dock.
        /// </summary>
        /// <param name="value">Value to be converted.</param>
        /// <param name="targetType">Type, the value is to be converted to.</param>
        /// <param name="parameter">Does not matter.</param>
        /// <param name="culture">Currently used culture. Not used here.</param>
        /// <returns>Converted value.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DockSide side = (DockSide)value;
            Dock result = Dock.Left;

            switch (side)
            {
                case DockSide.Left:
                    result = Dock.Left;
                    break;
                case DockSide.Top:
                    result = Dock.Top;
                    break;
                case DockSide.Right:
                    result = Dock.Right;
                    break;
                case DockSide.Bottom:
                    result = Dock.Bottom;
                    break;
                case DockSide.Tabbed:
                    result = Dock.Left;
                    break;
                case DockSide.None:
                    result = Dock.Left;
                    break;
            }

            return result;
        }

        /// <summary>
        /// Converts from dock to dock side.
        /// </summary>
        /// <param name="value">Value to be converted.</param>
        /// <param name="targetType">Type, the value is to be converted to.</param>
        /// <param name="parameter">Does not matter.</param>
        /// <param name="culture">Currently used culture. Not used here.</param>
        /// <returns>Converted value.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Dock side = (Dock)value;
            DockSide result = DockSide.None;

            switch (side)
            {
                case Dock.Left:
                    result = DockSide.Left;
                    break;
                case Dock.Top:
                    result = DockSide.Top;
                    break;
                case Dock.Right:
                    result = DockSide.Right;
                    break;
                case Dock.Bottom:
                    result = DockSide.Bottom;
                    break;
            }

            return result;
        }
    }
}
