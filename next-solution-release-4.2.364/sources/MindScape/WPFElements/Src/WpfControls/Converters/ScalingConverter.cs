using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using System.ComponentModel;

namespace Mindscape.WpfElements.WpfPropertyGrid
{
  /// <summary>
  /// Scales a value.
  /// </summary>
  public class ScalingConverter : IValueConverter
  {
    private double _scaleFactor = 1;
    private bool _forceToInteger;

    /// <summary>
    /// Gets or sets the scale factor.
    /// </summary>
    public double ScaleFactor
    {
      get { return _scaleFactor; }
      set { _scaleFactor = value; }
    }

    /// <summary>
    /// Gets or sets whether the resulting scaled value is rounded up
    /// to the nearest integer.
    /// </summary>
    public bool ForceToInteger
    {
      get { return _forceToInteger; }
      set { _forceToInteger = value; }
    }

    /// <summary>
    /// Scales the specified value, optionally rounding up to the nearest integer.
    /// </summary>
    /// <param name="value">The value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>The scaled and optionally rounded value.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      double result = System.Convert.ToDouble(value, culture) * ScaleFactor;
      return (ForceToInteger ? Math.Ceiling(result) : result);
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
}
