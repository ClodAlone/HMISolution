// <copyright file="BrushModifier.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
#if WINDOWS_PHONE||WINDOWS_PHONE_7
using System.Windows.Data;
using System.Windows.Media;
namespace Syncfusion.WP.Converters
#else
#if SILVERLIGHT
using System.Windows.Data;
using System.Windows.Media;
namespace Syncfusion.Tools.Converters
#else
#if WPF
using System.Windows.Media;
using System.Windows.Data;
namespace Syncfusion.Windows.Converters
#else
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
namespace Syncfusion.UI.Xaml.Converters
#endif
#endif
#endif
{
    /// <summary>
    /// Represents a convertor that modifies the brush
    /// </summary>
    [ClassReference(IsReviewed = false, ShouldInclude = false)]
    public class BrushModifier : IValueConverter
    {
        /// <summary>
        /// Modifies the brush
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
            if (value == null)
                return null;

            if(parameter == null)
                return value;

            SolidColorBrush _brush = (SolidColorBrush)value;
            SolidColorBrush brush = new SolidColorBrush(_brush.Color);
            double opacity;
            Double.TryParse(parameter.ToString(),NumberStyles.Any,CultureInfo.InvariantCulture,out opacity);
            brush.Opacity = opacity;
            return brush;
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
