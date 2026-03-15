#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Globalization;
using System.Windows.Data;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    ///
    /// </summary>
    public class ProgressConverter : IMultiValueConverter
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="values"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] != null && values[2] != null)
            {
                double value;
                double.TryParse(values[0].ToString(), out value);
                double actualWidth = (double)values[1];
                double maxValue;
                double.TryParse(values[2].ToString(), out maxValue);
                double percentage = (value * (actualWidth / maxValue));
                return percentage / actualWidth;
            }
            return 0d;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}