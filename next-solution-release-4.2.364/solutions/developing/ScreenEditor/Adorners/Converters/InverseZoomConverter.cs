using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ScreenManager.Adorners.Converters
{
    public class InverseZoomConverter : IValueConverter
    {
        FrameworkElement control;
        internal FrameworkElement Control
        {
            get
            {
                return control;
            }
            set
            {
                if (control == value)
                    return;
                control = value;
                if (Double.IsNaN(initialWidth))
                    initialWidth = control.Width;
            }
        }
        static double initialWidth = Double.NaN;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double)
            {
                var v = (double)value;
                if (v != 0)
                {
                    var ret =  1 / v;
                    if (control != null)
                    {
                        if (ret > 1)
                            control.Width = initialWidth * ret;
                        else
                            control.Width = initialWidth;
                    }

                    return ret;
                }
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
