// <copyright file="BooleanToVisibilityConverter.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if WINDOWS_PHONE||WINDOWS_PHONE_7
using System.Windows.Data;
using System.Windows;
using System.Globalization;
namespace Syncfusion.WP.Converters
#else
#if SILVERLIGHT
using System.Windows;
using System.Windows.Data;
using System.Globalization;
namespace Syncfusion.Tools.Converters
#else
#if WPF
using System.Windows;
using System.Windows.Data;
using System.Globalization;
namespace Syncfusion.Windows.Converters
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
namespace Syncfusion.UI.Xaml.Converters
#endif
#endif
#endif
{
    /// <summary>
    /// Represents the converter that converts Boolean values to and
    /// from visibility enumeration values
    /// </summary>
    [ClassReference(IsReviewed = false, ShouldInclude = false)]
    public class BooleanToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Converts bool variable to visibility
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
#if !WINRT
        public object Convert(object value, Type targetType, object parameter, CultureInfo language)
#else
        public object Convert(object value, Type targetType, object parameter, string language)
#endif
        {
            bool flag = (bool)value;
            if (flag)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Converts back into default type
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
#if !WINRT
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo language)
#else
        public object ConvertBack(object value, Type targetType, object parameter, string language)
#endif
        {
            Visibility visibility = (Visibility)value;
            if (visibility == Visibility.Collapsed)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    /// <summary>
    /// Represents the converter that converts InverseBoolean values to and
    /// from visibility enumeration values
    /// </summary>
    [ClassReference(IsReviewed = false, ShouldInclude = false)]
    public class InverseBooleanToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Converts Boolean to visibility
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
#if !WINRT
        public object Convert(object value, Type targetType, object parameter, CultureInfo language)
#else
        public object Convert(object value, Type targetType, object parameter, string language)
#endif
        {
            bool flag = (bool)value;
            if (flag)
            {
                return Visibility.Collapsed;
            }
            else
            {
                return Visibility.Visible;
            }
        }

        /// <summary>
        /// Converts back into default type
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
#if !WINRT
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo language)
#else
        public object ConvertBack(object value, Type targetType, object parameter, string language)
#endif
        {
            Visibility visibility = (Visibility)value;
            if (visibility == Visibility.Collapsed)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
