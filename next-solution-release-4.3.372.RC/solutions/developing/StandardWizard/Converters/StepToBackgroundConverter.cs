using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows.Data;
using System.Windows;
using System.Windows.Media;

namespace StandardWizard
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    class StepToBackgroundConverter : IValueConverter
    {
        private Color _selectedItem = Color.FromRgb(0xED, 0x1C, 0x24);
        private Color _unselectedItem = Color.FromRgb(0xFF, 0x7F, 0x27);
        #region IValueConverter implementation
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value== null || parameter == null || value == DependencyProperty.UnsetValue || parameter == DependencyProperty.UnsetValue)
                return new SolidColorBrush(_unselectedItem);

            if ((int)value == int.Parse(parameter.ToString()))
                return new SolidColorBrush(_selectedItem);

            return new SolidColorBrush(_unselectedItem);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
        #endregion
}
