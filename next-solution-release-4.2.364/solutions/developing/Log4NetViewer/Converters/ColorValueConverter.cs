using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace Log4NetViewer.Converters
{
    public class ColorValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                switch ((string)value)
                {
                    case "ERROR":
                        return new SolidColorBrush(Colors.Red);
                    case "INFO":
                        return new SolidColorBrush(Color.FromRgb(0x26, 0x2E, 0x43));
                    case "DEBUG":
                        return new SolidColorBrush(Color.FromRgb(0x36, 0x36, 0x36));
                    case "WARN":
                        return new SolidColorBrush(Colors.OrangeRed);
                    case "FATAL":
                        return new SolidColorBrush(Color.FromRgb(0x99, 0x00, 0x00));
                    case "COMPRESSION":
                        return new SolidColorBrush(Colors.DimGray);
                    default:
                        return new SolidColorBrush(Colors.DarkGray);
                }
            }
            catch (Exception ex)
            {
               return new SolidColorBrush(Colors.White);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
