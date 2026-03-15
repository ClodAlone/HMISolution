// <copyright file="ToolTipVisibilityConverter.cs" company="Syncfusion">
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
#if !WINDOWS_PHONE_7
using System.Threading.Tasks;
#endif
#if !WINRT
using System.Windows.Data;
using System.Windows;
using System.Globalization;
#endif
#if WINDOWS_PHONE || WINDOWS_PHONE_7
namespace Syncfusion.WP.Converters

#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
namespace Syncfusion.UI.Xaml.Converters
#endif
{
    /// <summary>
    /// Converts the ToolTip visibility state
    /// </summary>
    [ClassReference(IsReviewed = false, ShouldInclude = false)]
    public class ToolTipVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Converts object into visibility
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
#if WINDOWS_PHONE||WINDOWS_PHONE_7||SILVERLIGHT
        public object Convert(object value, Type targetType, object parameter, CultureInfo language)
#else
        public object Convert(object value, Type targetType, object parameter, string language)
#endif
        {
            if ((bool)value)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        /// <summary>
        /// Converts Visibility back into object
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
#if WINDOWS_PHONE||WINDOWS_PHONE_7||SILVERLIGHT
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo language)
#else
        public object ConvertBack(object value, Type targetType, object parameter, string language)
#endif
        {
            throw new NotImplementedException();
        }
    }
}
