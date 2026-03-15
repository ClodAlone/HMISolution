using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using UFRecipeEditor.CsvHelper;

namespace UFRecipeEditor.Converters
{
    [ValueConversion(typeof(ImportExportSeparatorOptions), typeof(Visibility))]
    public class SeparatorOptionToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(Visibility))
                throw new InvalidOperationException("The target must be a System.Windows.Visibility");

            if (!(value is ImportExportSeparatorOptions))
                return Visibility.Collapsed;

            var option = (ImportExportSeparatorOptions)value;
            return option != ImportExportSeparatorOptions.Custom ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
