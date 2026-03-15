// <copyright file="DoubleToThicknessConverter.cs" company="Syncfusion">
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
using System.Windows.Data;
using System.Windows;
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
    /// Represents a convertor that converts Double to thickness
    /// </summary>
    [ClassReference(IsReviewed = false, ShouldInclude = false)]
    public class DoubleToThicknessConverter : IValueConverter
    {
        /// <summary>
        /// Converts Double to thickness
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
            if (value is double)
            {
                Thickness t = new Thickness((double)value);
                t.Left -= 2;
                t.Top -= 2;
                t.Right -= 2;
                t.Bottom -= 2;
                return t;
            }
            return value;
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
            throw new NotImplementedException();
        }
    }
}
