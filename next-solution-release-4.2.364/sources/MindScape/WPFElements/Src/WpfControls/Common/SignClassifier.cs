using System.Windows.Data;
using System;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Classifies decimal values by sign.
  /// </summary>
  public class SignConverter : IValueConverter
  {
    /// <summary>
    /// Converts a decimal value to a sign indicator.
    /// </summary>
    /// <param name="value">The decimal value.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>A <see cref="Sign"/> indicating the sign of the value.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      decimal d = (decimal)value;
      if (d < 0)
      {
        return Sign.Negative;
      }
      else if (d > 0)
      {
        return Sign.Positive;
      }
      return Sign.Zero;
    }

    /// <summary>
    /// Converts a value from a binding target for writing to a binding source.
    /// </summary>
    /// <param name="value">The value produced by the binding target.</param>
    /// <param name="targetType">The type to convert to.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>This conversion direction is not implemented by this converter.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }



  /// <summary>
  /// The sign of a numeric value.
  /// </summary>
  public enum Sign
  {
    /// <summary>
    /// The value is positive (strictly greater than zero).
    /// </summary>
    Positive,

    /// <summary>
    /// The value is zero.
    /// </summary>
    Zero,

    /// <summary>
    /// The value is negative (strictly less than zero).
    /// </summary>
    Negative
  }
}
