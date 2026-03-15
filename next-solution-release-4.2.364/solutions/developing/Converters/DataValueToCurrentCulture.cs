
using System;
using System.Windows.Data;
using System.IO;
using Utilities;
using System.Collections;
using System.Globalization;
using System.Windows;
using TranslationHelpers;
using System.Collections.Generic;

namespace Converters
{
    /// <summary>
    /// A Value converter
    /// </summary>
    public class DataValueToCurrentCulture : IMultiValueConverter
    {
        #region IMultiValueConverter Members
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {   try
            {
                if (values == null || values.Length < 3)
                    return null;
                string dataVlaue = values[0]?.ToString();
                string stringPlaceolder = values[1]?.ToString(); 
                string toRemove = values.Length >= 4 ? values[3]?.ToString() : null;
                if (!string.IsNullOrEmpty(toRemove) && dataVlaue.StartsWith(toRemove))
                    dataVlaue = dataVlaue.Replace(toRemove, "");
                string toUse = $"_{stringPlaceolder}_{dataVlaue}";
                return TranslationHelper.TranlslateText(toUse, Utilities.Converters.ResourceEnumConverter.StringTable, values[0]?.ToString());
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
