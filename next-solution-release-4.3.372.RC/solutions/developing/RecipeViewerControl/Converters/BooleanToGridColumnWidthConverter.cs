using System;
using System.Globalization;
using System.Windows.Data;

namespace RecipeViewerControl.Converters
{
    internal class BooleanToGridColumnWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool && (bool)(value))
                return "*";
            else
                return "0";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
