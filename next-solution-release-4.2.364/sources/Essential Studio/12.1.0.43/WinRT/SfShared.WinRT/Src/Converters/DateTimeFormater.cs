// <copyright file="DateTimeFormater.cs" company="Syncfusion">
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

namespace Syncfusion.WP.Converters
#else
#if SILVERLIGHT
using System.Windows.Data;
namespace Syncfusion.Tools.Converters
#else
using Windows.UI.Xaml.Data;
namespace Syncfusion.UI.Xaml.Converters
#endif
#endif
{
    /// <summary>
    /// Represents a class that formats the DateTime
    /// </summary>
    [ClassReference(IsReviewed = false, ShouldInclude = false)]
    public class DateTimeFormater : IValueConverter
    {
#if WINDOWS_PHONE||WINDOWS_PHONE_7|| SILVERLIGHT
        /// <summary>
        /// Converts the DateTime
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
#else
        /// <summary>
        /// Converts the DateTime
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
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
#if WINDOWS_PHONE||WINDOWS_PHONE_7|| SILVERLIGHT
        /// <summary>
        /// Converts back into default type
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
#else
        /// <summary>
        /// Converts back into default type
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

    /// <summary>
    /// Represents a class that formats the Calendar header
    /// </summary>
    public class CalendarHeaderFormatter : IValueConverter
    {
#if WINDOWS_PHONE||WINDOWS_PHONE_7|| SILVERLIGHT
        /// <summary>
        /// Formats the DateTime as header
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
#else
        /// <summary>
        /// Formats the DateTime as header
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, string language)
#endif
        {
            DateTime data;
            CultureInfo currentculture=parameter as CultureInfo;
            bool isvalid = DateTime.TryParse(value.ToString(), out data);
            if (isvalid)
            {
                return data.ToString("MMMM, yyyy", currentculture);
            }
            else
            {
                return null;
            }
        }

#if WINDOWS_PHONE||WINDOWS_PHONE_7|| SILVERLIGHT
        /// <summary>
        /// Converts back into default type
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
#else
        /// <summary>
        /// Converts back into default type
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
#endif
        {
            DateTime data;
            CultureInfo currentculture = parameter as CultureInfo;
            bool isvalid = DateTime.TryParse(value.ToString(), out data);
            if (isvalid)
            {
                return data.ToString("MMMM, yyyy", currentculture);
            }
            else
            {
                return null;
            }
        }
    }
}
