using System;
using System.Collections.Generic;
using System.Linq;

using System.Windows;
using System.Windows.Data;

namespace AlarmWindow.Converters
{
    public class HorizontalToAlignementConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(value is HorizontalAlignment)
            {
                switch ((HorizontalAlignment)value)
                {
                    case HorizontalAlignment.Left:
                        return TextAlignment.Left;
                    case HorizontalAlignment.Right:
                        return TextAlignment.Right;
                    case HorizontalAlignment.Stretch:
                        return TextAlignment.Justify;
                    case HorizontalAlignment.Center:
                        return TextAlignment.Center;
                }
            }
            return TextAlignment.Left;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
