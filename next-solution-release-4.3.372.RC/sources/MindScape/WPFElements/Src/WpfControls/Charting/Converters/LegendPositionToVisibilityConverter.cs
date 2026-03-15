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
  /// Converts a <see cref="LegendPosition"/> into a <see cref="Visibility"/>.
  /// </summary>
  public class LegendPositionToVisibilityConverter : IValueConverter
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="LegendPositionToVisibilityConverter"/> class.
    /// </summary>
    public LegendPositionToVisibilityConverter()
    {
      LeftVisibility = Visibility.Collapsed;
      RightVisibility = Visibility.Collapsed;
      TopVisibility = Visibility.Collapsed;
      BottomVisibility = Visibility.Collapsed;
      CenterVisibility = Visibility.Collapsed;
      NoneVisibility = Visibility.Collapsed;
    }

    /// <summary>
    /// Gets or sets the <see cref="Visibility"/> to return if the <see cref="LegendPosition"/> is left.
    /// </summary>
    public Visibility LeftVisibility { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="Visibility"/> to return if the <see cref="LegendPosition"/> is right.
    /// </summary>
    public Visibility RightVisibility { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="Visibility"/> to return if the <see cref="LegendPosition"/> is top.
    /// </summary>
    public Visibility TopVisibility { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="Visibility"/> to return if the <see cref="LegendPosition"/> is bottom.
    /// </summary>
    public Visibility BottomVisibility { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="Visibility"/> to return if the <see cref="LegendPosition"/> is center.
    /// </summary>
    public Visibility CenterVisibility { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="Visibility"/> to return if the <see cref="LegendPosition"/> is none.
    /// </summary>
    public Visibility NoneVisibility { get; set; }

    /// <summary>
    /// Returns a <see cref="Visibility"/> based on the given <see cref="LegendPosition"/>.
    /// </summary>
    /// <param name="value">The <see cref="LegendPosition"/> value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>A <see cref="Visibility"/> based on the given <see cref="LegendPosition"/>.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      LegendPosition lp = (LegendPosition)value;
      switch (lp)
      {
        case LegendPosition.Right:
          return RightVisibility;
        case LegendPosition.Left:
          return LeftVisibility;
        case LegendPosition.Top:
          return TopVisibility;
        case LegendPosition.Bottom:
          return BottomVisibility;
        case LegendPosition.Center:
          return CenterVisibility;
        case LegendPosition.None:
          return NoneVisibility;
      }
      return 0;
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
