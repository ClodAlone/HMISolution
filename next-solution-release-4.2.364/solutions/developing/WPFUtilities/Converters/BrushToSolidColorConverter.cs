using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace WPFUtilities.Converters
{
    public class BrushToSolidColorConverter : IValueConverter
    {
        static SolidColorBrush AlphaBlack = new SolidColorBrush(Colors.Black) { Opacity = 0.5 };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is VisualBrush || value is SolidColorBrush)
                return AlphaBlack;
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
