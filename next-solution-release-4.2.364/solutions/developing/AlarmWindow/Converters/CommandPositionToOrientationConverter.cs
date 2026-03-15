using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace AlarmWindow.Converters
{
    public class CommandPositionToOrientationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch ((Dock)value)
            {
                case Dock.Bottom:
                case Dock.Top:
                    {
                        return Orientation.Horizontal;
                    }
                case Dock.Left:
                case Dock.Right:
                    {
                        return Orientation.Vertical;
                    }
                default:
                    {
                        return Orientation.Horizontal;
                    }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
