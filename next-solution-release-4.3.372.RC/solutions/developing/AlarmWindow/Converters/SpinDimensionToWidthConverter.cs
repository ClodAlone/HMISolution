using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace AlarmWindow.Converters
{
    public class SpinDimensionToWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch ((int)value)
            {
                case 0:
                    {
                        return (int)15;
                    }
                case 1:
                    {
                        return (int)20;
                    }
                case 2:
                    {
                        return (int)25;
                    }
                default:
                    {
                        return (int)15;
                    }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
