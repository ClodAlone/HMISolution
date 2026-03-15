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

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// Converts a value.
  /// </summary>
  public class OrientationToVisibilityConverter : IValueConverter
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="OrientationToVisibilityConverter"/> class.
    /// </summary>
    public OrientationToVisibilityConverter()
    {
      VerticalVisibility = Visibility.Visible;
      HorizontalVisibility = Visibility.Collapsed;
    }

    /// <summary>
    /// Gets or sets the <see cref="Visibility"/> to be returned if the orientation is vertical. The default is Visibility.Visible.
    /// </summary>
    public Visibility VerticalVisibility { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="Visibility"/> to be returned if the orientation is horizontal. The default is Visibility.Collapsed.
    /// </summary>
    public Visibility HorizontalVisibility { get; set; }

    /// <summary>
    /// Converts an <see cref="Orientation"/> value from a binding source to a <see cref="Visibility"/> for use by a binding target.
    /// </summary>
    /// <param name="value">The Orientation produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns><see cref="VerticalVisibility"/> if the value is Orientation.Vertical; otherwise <see cref="HorizontalVisibility"/>.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      Orientation orientation = (Orientation)value;
      return orientation == Orientation.Horizontal ? HorizontalVisibility : VerticalVisibility;
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
