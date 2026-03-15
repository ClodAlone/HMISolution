using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Controls;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Selects a ControlTemplate for the RepeatButton part of a channel mixer
  /// in a <see cref="ColorPicker"/> control.
  /// </summary>
  public class ColorChannelToTemplateConverter : IValueConverter
  {
    /// <summary>
    /// The template for alpha channel repeat buttons.
    /// </summary>
    public ControlTemplate AlphaTemplate { get; set; }

    /// <summary>
    /// The template for red channel repeat buttons.
    /// </summary>
    public ControlTemplate RedTemplate { get; set; }

    /// <summary>
    /// The template for green channel repeat buttons.
    /// </summary>
    public ControlTemplate GreenTemplate { get; set; }

    /// <summary>
    /// The template for blue channel repeat buttons.
    /// </summary>
    public ControlTemplate BlueTemplate { get; set; }

    /// <summary>
    /// Selects a ControlTemplate according to a <see cref="ColorChannel"/> value.
    /// </summary>
    /// <param name="value">The ColorChannel value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>The selected ControlTemplate.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      ColorChannel ch = (ColorChannel)value;
      switch (ch)
      {
        case ColorChannel.Alpha: return AlphaTemplate;
        case ColorChannel.Blue: return BlueTemplate;
        case ColorChannel.Green: return GreenTemplate;
        case ColorChannel.Red: return RedTemplate;
      }

      return null;
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
