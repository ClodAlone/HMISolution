
using System;
using System.Windows.Data;
using System.IO;
using Utilities;
using System.Collections;
using System.Globalization;
using System.Windows;

namespace Converters
{
    /// <summary>
    /// A Value converter
    /// </summary>
    public class DateTimeCurrentCultureConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string dateTimeFormat = null;
            try
            {
                if (values == null)
                    return null;
                if (values.Length >= 3 && !String.IsNullOrEmpty(values[2] as string))
                    dateTimeFormat = values[2] as string;
                if (values.Length >= 2 && values[1] as CultureInfo != null)
                    culture = values[1] as CultureInfo;
                
                DateTime dateTime = (DateTime)values[0];

                if (dateTimeFormat != null)
                    return dateTime.ToString(dateTimeFormat, culture);
                return dateTime.ToString(culture);
            }
            catch
            {
                return values[0]?.ToString();
            }
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
