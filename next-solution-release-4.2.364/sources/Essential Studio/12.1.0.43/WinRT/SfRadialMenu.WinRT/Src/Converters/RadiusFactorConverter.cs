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
#if WINDOWS_PHONE||WINDOWS_PHONE_7
using System.Windows.Data;
using System.Globalization;

namespace Syncfusion.WP.Controls.Navigation
#elif SILVERLIGHT
using System.Windows.Data;
using System.Globalization;

namespace Syncfusion.Tools.Controls.Navigation
#elif WPF
using System.Windows.Data;
using System.Globalization;
namespace Syncfusion.Windows.Controls.Navigation
#else
using Windows.UI.Xaml.Data;

namespace Syncfusion.UI.Xaml.Controls.Navigation
#endif
{
    /// <summary>
    /// Represents a convertor that converts the radius factor
    /// </summary>
    public class RadiusFactorConverter : IValueConverter
    {
        /// <summary>
        /// Converts the radius factor
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
            double factor = (double)value;
            double size = 0d;
            if (parameter != null)
                size = (double)parameter;           
            return size * factor;
           
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
