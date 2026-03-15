// <copyright file="PrecisionToVisibilityConverter.cs" company="Syncfusion">
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
#if !(Silverlight4 || WINDOWS_PHONE_7)
using System.Threading.Tasks;
#endif
#if !WINRT
using System.Windows.Data;
using System.Windows;
using System.Globalization;
#endif
#if WINDOWS_PHONE || WINDOWS_PHONE_7
using Syncfusion.WP.Primitives;
namespace Syncfusion.WP.Converters
#elif SILVERLIGHT
using Syncfusion.Tools.Primitives;
namespace Syncfusion.Tools.Converters
#else
#if WPF
using Syncfusion.Windows.Primitives;
namespace Syncfusion.Windows.Converters
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Syncfusion.UI.Xaml.Primitives;
namespace Syncfusion.UI.Xaml.Converters
#endif
#endif
{
    /// <summary>
    /// Conversion of precision to visibility
    /// </summary>
    [ClassReference(IsReviewed = false, ShouldInclude = false)]
    public class PrecisionToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Converts precision to visibility
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
#if !WINRT
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
#else
        public object Convert(object value, Type targetType, object parameter, string culture)
#endif
        {
            if ((Precision)value == Precision.Standard)
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
        /// <param name="culture"></param>
        /// <returns></returns>
#if !WINRT
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
#else
        public object ConvertBack(object value, Type targetType, object parameter, string culture)
#endif
        {
            throw new NotImplementedException();
        }
    }
}
