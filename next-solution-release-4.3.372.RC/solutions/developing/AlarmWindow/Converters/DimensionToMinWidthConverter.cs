using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace AlarmWindow.Converters
{
    public class DimensionToMinWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch ((Dimensions)value)
            {
                case Dimensions.Small:
                    {
                        return 60;
                    }
                case Dimensions.Medium:
                    {
                        return 90;
                    }
                case Dimensions.Large:
                    {
                        return 120;
                    }
                default:
                    {
                        return 90;
                    }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
