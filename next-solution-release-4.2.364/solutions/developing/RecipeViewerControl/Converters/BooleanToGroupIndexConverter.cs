using System;
using System.Globalization;
using System.Windows.Data;

namespace RecipeViewerControl.Converters
{
    internal class BooleanToGroupIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool && (bool)(value))
            {
                int index;
                if (parameter != null && int.TryParse(parameter as string, out index))
                    return index;
                return 0;
            }
            else
                return -1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
