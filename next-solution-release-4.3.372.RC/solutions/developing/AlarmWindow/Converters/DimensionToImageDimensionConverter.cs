using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace AlarmWindow.Converters
{
    public class DimensionToImageDimensionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch ((Dimensions)value)
            {
                case Dimensions.Small:
                    {
                        return 15;
                    }
                case Dimensions.Medium:
                    {
                        return 18;
                    }
                case Dimensions.Large:
                    {
                        return 21;
                    }
                default:
                    {
                        return 12;
                    }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
