// <copyright file="RadiusToWidthConverter.cs" company="Syncfusion">
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
using System.Globalization;

namespace Syncfusion.WP.Converters
#else
#if SILVERLIGHT
using System.Windows.Data;
using System.Globalization;
namespace Syncfusion.Tools.Converters
#else
#if WPF
using System.Windows.Data;
using System.Globalization;
namespace Syncfusion.Windows.Converters
#else
using Windows.UI.Xaml.Data;

namespace Syncfusion.UI.Xaml.Converters
#endif
#endif
#endif
{
    /// <summary>
    /// Conversion of radius to width
    /// </summary>
    [ClassReference(IsReviewed = false, ShouldInclude = false)]
    public class RadiusToWidthConverter : IValueConverter
    {
        /// <summary>
        /// Converts the Raddius to width
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
            return (double)value * 2;
        }

        /// <summary>
        /// Converts the width back into radius
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
