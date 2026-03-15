#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
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

namespace Syncfusion.UI.Xaml.Converters
#endif
#endif
#endif
{
    /// <summary>
    /// Conversion of RGB values
    /// </summary>
    public class RGBConverter : IValueConverter
    {
        /// <summary>
        /// Converts into RGB Value
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
            var color = (Color)value;
            if (parameter != null)
            {
                if (parameter.Equals("R"))
                {
                    return color.R.ToString();
                }
                else if (parameter.Equals("G"))
                {
                    return color.G.ToString();
                }
                else if (parameter.Equals("B"))
                {
                    return color.B.ToString();
                }
            }
            return "";
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
