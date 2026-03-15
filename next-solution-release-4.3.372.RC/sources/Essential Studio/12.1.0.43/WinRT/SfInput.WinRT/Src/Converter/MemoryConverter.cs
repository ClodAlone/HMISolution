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
#if !(WINDOWS_PHONE_7 || Silverlight4)
using System.Threading.Tasks;

#endif
#if !WINRT
using System.Windows;
using System.Windows.Data;
#endif

#if WINDOWS_PHONE || WINDOWS_PHONE_7

namespace Syncfusion.WP.Controls.Input
#elif WPF

namespace Syncfusion.Windows.Controls.Input
#elif SILVERLIGHT

namespace Syncfusion.Tools.Controls.Input
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
namespace Syncfusion.UI.Xaml.Controls.Input
#endif
{
    /// <summary>
    /// Represents a class for converting values
    /// </summary>
    public class MemoryConverter : IValueConverter
    {
#if !WINRT
        /// <summary>
        /// Converts the value and stores in memory
        /// </summary>
        /// <value> Visibility is
        /// <c>Collapsed</c> if memory is 0; otherwise, <c>Visible</c>.
        /// </value>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
         public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
#else
        /// <summary>
        /// Converts the value and stores in memory
        /// </summary>
        /// <value> Visibility is
        /// <c>Collapsed</c> if memory is 0; otherwise, <c>Visible</c>.
        /// </value>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
         public object Convert(object value, Type targetType, object parameter, string language)
#endif
        {
            var memory = Decimal.Parse(value.ToString());
            if (memory == 0)
            {
                return Visibility.Collapsed;
            }
            else
            {
                return Visibility.Visible;
            }
        }

#if !WINRT
         /// <summary>
         /// Throws exception if cant be converted back
         /// </summary>
         /// <param name="value"></param>
         /// <param name="targetType"></param>
         /// <param name="parameter"></param>
         /// <param name="culture"></param>
         /// <returns></returns>
         public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
#else
         /// <summary>
         /// Throws exception if cant be converted back
         /// </summary>
         /// <param name="value"></param>
         /// <param name="targetType"></param>
         /// <param name="parameter"></param>
         /// <param name="language"></param>
         /// <returns></returns>
         public object ConvertBack(object value, Type targetType, object parameter, string language)
#endif
         {
            throw new NotImplementedException();
        }
    }
}
