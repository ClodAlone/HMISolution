// <copyright file="TimeFormater.cs" company="Syncfusion">
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
using System.Threading.Tasks;
#if SILVERLIGHT
using System.Windows.Data;
using System.Globalization;
#else
using Windows.UI.Xaml.Data;
#endif
namespace Syncfusion.UI.Xaml.Converters
{
    /// <summary>
    /// Format the time
    /// </summary>
    [ClassReference(IsReviewed = false, ShouldInclude = false)]
    public class TimeFormater : IValueConverter
    {
        /// <summary>
        /// Converts the object
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
#if WINDOWS_PHONE||SILVERLIGHT
        public object Convert(object value, Type targetType, object parameter, CultureInfo language)
#else
        public object Convert(object value, Type targetType, object parameter, string language)
#endif
        {
            if (parameter == null)
            {
                return value;
            }

            DateTime data = System.Convert.ToDateTime(value);
            return data.ToString(parameter.ToString());
        }

        /// <summary>
        /// Converts the value back into the object
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
#if WINDOWS_PHONE||SILVERLIGHT
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo language)
#else
        public object ConvertBack(object value, Type targetType, object parameter, string language)
#endif
        {
            throw new NotImplementedException();
        }
    }
}
