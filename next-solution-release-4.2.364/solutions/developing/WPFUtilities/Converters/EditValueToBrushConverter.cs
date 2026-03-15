using System;
using System.Globalization;
using System.Windows.Media;
using System.Windows.Data;

namespace WPFUtilities.Converters
{
    /// <summary>
    /// This class simply converts a Boolean to a Visibility
    /// This class is kind of obsolete as there is a Standard 
    /// BooleanToVisibilityConverter within the System.Windows.Controls 
    /// namespace provided with the .NET framework, but you can not 
    /// debug that code. So this ValueConverter
    /// was provided in order that it could be debugger
    /// </summary>
    [ValueConversion(typeof(string), typeof(string))]
    public class EditValueToBrushConverter : IMultiValueConverter
    {
        #region IValueConverter implementation

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] == null || !(values[0] is double) || !(values[1] is Brush))
                return Binding.DoNothing;
            if ((double)values[0] < 0)
                return Brushes.Red;
            return values[1] as Brush;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
