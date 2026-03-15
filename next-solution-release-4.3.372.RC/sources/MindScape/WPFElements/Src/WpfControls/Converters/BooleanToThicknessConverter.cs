using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;

#if SILVERLIGHT
namespace Mindscape.SilverlightElements
#else
namespace Mindscape.WpfElements
#endif
{
  /// <summary>
  /// Selects a Thickness according to a boolean value.
  /// </summary>
  public class BooleanToThicknessConverter : IValueConverter
  {
    /// <summary>
    /// The Thickness to select when the source value is true.
    /// </summary>
    public Thickness TrueThickness { get; set; }

    /// <summary>
    /// The Thickness to select when the source value is false.
    /// </summary>
    public Thickness FalseThickness { get; set; }

    /// <summary>
    /// Selects a Thickness according to whether the input is true or false.
    /// </summary>
    /// <param name="value">The value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>The selected Thickness.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      bool boolean = (bool)value;
      return boolean ? TrueThickness : FalseThickness;
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
