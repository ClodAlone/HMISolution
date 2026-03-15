// <copyright file="RandomBrushConverter.cs" company="Syncfusion">
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
#if !Silverlight4
using System.Threading.Tasks;
#endif
#if !(SILVERLIGHT||WPF)
using Windows.UI;
#endif
#if WINDOWS_PHONE
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
using System.Windows.Data;
using System.Windows.Media;
using System.Globalization;
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
    /// Brush convertor
    /// </summary>
    [ClassReference(IsReviewed = false, ShouldInclude = false)]
    public class RandomBrushConverter : IValueConverter
    {
        /// <summary>
        /// Converts into brush
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
            if (value == null || !(value is Brush))
            {
                return new SolidColorBrush(Colors.Transparent);
            }
            return GetRandomBrush(value as Brush);
        }

        private Brush GetRandomBrush(Brush brush)
        {
            Random random = new Random();
            int value = random.Next(2);
            if (value == 0)
            {
                brush.Opacity = 1;
            }
            else if (value == 1)
            {
                brush.Opacity = 0.7;
            }
            else
            {
                brush.Opacity = 0.3;
            }
            return brush;
        }

        /// <summary>
        /// Converts the value back into the object
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
