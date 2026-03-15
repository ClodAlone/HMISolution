using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace SQLDatabaseConfiguration.Converters
{
    [ValueConversion(typeof(String), typeof(Visibility))]
    public class ProjectPathToVisibilityConverter : IValueConverter
    {
        #region IValueConverter
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var projectPath = value as String;
            if (String.IsNullOrEmpty(projectPath))
                return Visibility.Collapsed;

            return XpoHelpers.XpoHelper.IsDataSource(projectPath) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
