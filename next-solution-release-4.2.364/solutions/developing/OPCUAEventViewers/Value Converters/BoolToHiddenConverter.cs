using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace OPCUAEventViewers
{
    /// <summary>
    ///This class simply converts a Boolean to a Visibility
    ///This is simliar to the BoolToVisibilityConverter that 
    ///is also part of the Sonic project, it is just this one
    ///returns Visibility.Collapsed whilst the other one returns
    ///Visibility.Hidden
    /// </summary>
    [ValueConversion(typeof(Boolean), typeof(Visibility))]
    public class BoolToHiddenConverter : IValueConverter
    {
        #region IValueConverter implementation
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Boolean invert = Boolean.Parse(parameter.ToString());
            Boolean input = (Boolean)value;

            if (invert)
                input = !input;

            if (input)
                return Visibility.Visible;
            return Visibility.Collapsed;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("This method is intentionally not implemented");
        }
        #endregion
    }
}
