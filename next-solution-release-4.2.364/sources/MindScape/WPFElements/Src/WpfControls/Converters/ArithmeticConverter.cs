using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Modifies the input number using multiplication and addition.
  /// </summary>
  public class ArithmeticConverter : IValueConverter
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ArithmeticConverter"/> class.
    /// </summary>
    public ArithmeticConverter()
    {
      Multiplier = 1;
    }

    /// <summary>
    /// Gets or sets the amount by which to scale the input. The default is 1.
    /// </summary>
    public double Multiplier { get; set; }

    /// <summary>
    /// Gets or sets the value added to the input. The default is 0.
    /// </summary>
    public double Additive { get; set; }

    /// <summary>
    /// Converts a double value by multiplying by the <see cref="Multiplier"/> and adding with the <see cref="Additive"/>.
    /// </summary>
    /// <param name="value">The value to be modified.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>The modified value.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return Multiplier * NumericalUtils.ConvertToDouble(value) + Additive;
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
      return (NumericalUtils.ConvertToDouble(value) - Additive) / Multiplier;
    }
  }
}
