
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
    public class UTCToLocalConverterWithClientOffset : IMultiValueConverter
    {
        #region IMultiValueConverter Members
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string dateTimeFormat = null;
            try
            {
                if (values == null)
                    return null;
                if (values.Length >= 5 && !String.IsNullOrEmpty(values[4] as string))
                    dateTimeFormat = values[4] as string;
                if (values.Length >= 4 && values[3] as CultureInfo != null)
                    culture = values[3] as CultureInfo;
                if (values.Length < 3 ||
                    values[1] == DependencyProperty.UnsetValue ||
                    values[2] == DependencyProperty.UnsetValue)
                    return values[0];
                DateTime dateTime;
                if ((bool)values[2] && (double)values[1] != 0.0)
                    dateTime = ((DateTime)values[0]).AddMinutes((double)values[1]);
                else
                {
                    if (values.Length >= 6 && values[5] is Boolean && (bool)values[5])
                        dateTime = (DateTime)values[0];
                    else
                        dateTime = ((DateTime)values[0]).ToLocalTime();
                }
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
