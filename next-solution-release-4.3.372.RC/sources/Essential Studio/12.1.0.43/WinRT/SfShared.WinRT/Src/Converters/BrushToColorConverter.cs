// <copyright file="BrushToColorConverter.cs" company="Syncfusion">
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
using System.Windows.Media;
using System.Globalization;

namespace Syncfusion.WP.Converters
#else
#if SILVERLIGHT
using System.Windows.Data;
using System.Windows.Media;
using System.Globalization;
namespace Syncfusion.Tools.Converters
#else
#if WPF
using System.Windows.Media;
using System.Windows.Data;
using System.Globalization;
namespace Syncfusion.Windows.Converters
#else
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
namespace Syncfusion.UI.Xaml.Converters
#endif
#endif
#endif
{
    /// <summary>
    /// Represents a convertor that converts brush to color
    /// </summary>
    [ClassReference(IsReviewed = false, ShouldInclude = false)]
    public class BrushToColorConverter : IValueConverter
    {
        /// <summary>
        /// Converts brush to color
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
            SolidColorBrush brush = value as SolidColorBrush;

            if (brush == null && parameter != null && parameter.ToString() == "AccentBrushnull")
                return new SolidColorBrush(Colors.Transparent);
            else if (brush == null && parameter != null && parameter.ToString() == "ContentBrushnull")
                return Colors.Black;
            else if (brush != null && parameter != null && parameter.ToString() == "ContentBrushnull")
                return brush.Color;
            else if (brush != null && parameter != null && parameter.ToString() == "AccentBrushnull")
            {
                Color color = Color.FromArgb((byte)(brush.Color.A), brush.Color.R, brush.Color.G, brush.Color.B);
                return new SolidColorBrush(color);
            }
            else if (brush == null)
                return Colors.Transparent;
            else
                return Color.FromArgb((byte)(brush.Color.A),
                                      brush.Color.R,
                                      brush.Color.G,
                                      brush.Color.B
                                     );
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
            return null;
        }
    }

}
