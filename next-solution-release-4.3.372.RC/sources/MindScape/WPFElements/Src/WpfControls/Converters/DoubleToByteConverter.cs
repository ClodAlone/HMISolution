using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Windows.Data;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Converts between doubles and bytes.
  /// </summary>
  [ValueConversion(typeof(Double), typeof(byte))]
  public class DoubleToByteConverter : IValueConverter
  {
    /// <summary>
    /// Converts a double value to a byte.
    /// </summary>
    /// <param name="value">The double value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>A byte of equal value to the double.  If the double is not convertible
    /// to a byte (according to the rules of System.Convert), an exception is thrown.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return System.Convert.ToByte((double)value);
    }

    /// <summary>
    /// Converts a byte value to a double.
    /// </summary>
    /// <param name="value">The byte value produced by the binding target.</param>
    /// <param name="targetType">The type to convert to.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>The equivalent double value.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      string text = value as string;
      if (text != null)
      {
        double result;
        bool isValid = Double.TryParse(text, NumberStyles.Number, CultureInfo.CurrentCulture, out result);
        if (isValid)
        {
          return result;
        }
        return value;  // return known bad data and let WPF data binding raise the validation errors
      }
      return System.Convert.ToDouble(value, CultureInfo.CurrentCulture);
    }
  }
}
