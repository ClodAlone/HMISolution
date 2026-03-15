using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace AlarmWindow.Converters
{
    public class DimensionToMinHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch ((Dimensions)value)
            {
                case Dimensions.Small:
                    {
                        return 35;
                    }
                case Dimensions.Medium:
                    {
                        return 45;
                    }
                case Dimensions.Large:
                    {
                        return 55;
                    }
                default:
                    {
                        return 35;
                    }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
