using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Opc.Ua;
using System.Windows;

namespace Sliders.Converters
{
    public class DimValueConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return new Thickness(0);
            try
            {
                double margin = double.Parse(value.ToString()) / 2;
                double top = 0.0;
                double bottom = 0.0;

                if (parameter != DependencyProperty.UnsetValue && parameter != null)
                {
                    if (parameter.ToString() == "1")
                        bottom = 1.0;
                    else if (parameter.ToString() == "2")
                        top = 1.0;
                }
                return new Thickness(margin, bottom, margin, top);
            }
            catch (Exception ex)
            {
                return new Thickness(0);
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
