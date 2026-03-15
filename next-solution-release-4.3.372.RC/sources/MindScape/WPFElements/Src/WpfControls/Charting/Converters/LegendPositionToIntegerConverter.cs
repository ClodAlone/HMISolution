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
  /// Converts a <see cref="LegendPosition"/> into an integer value.
  /// This is useful for setting the Grid.Row and Grid.Column properties of the legend.
  /// </summary>
  public class LegendPositionToIntegerConverter : IValueConverter
  {
    /// <summary>
    /// Gets or sets the integer to be returned if the <see cref="LegendPosition"/> is left.
    /// </summary>
    public int LeftInteger { get; set; }

    /// <summary>
    /// Gets or sets the integer to be returned if the <see cref="LegendPosition"/> is right.
    /// </summary>
    public int RightInteger { get; set; }

    /// <summary>
    /// Gets or sets the integer to be returned if the <see cref="LegendPosition"/> is top.
    /// </summary>
    public int TopInteger { get; set; }

    /// <summary>
    /// Gets or sets the integer to be returned if the <see cref="LegendPosition"/> is bottom.
    /// </summary>
    public int BottomInteger { get; set; }

    /// <summary>
    /// Gets or sets the integer to be returned if the <see cref="LegendPosition"/> is center.
    /// </summary>
    public int CenterInteger { get; set; }

    /// <summary>
    /// Gets or sets the integer to be returned if the <see cref="LegendPosition"/> is none.
    /// </summary>
    public int NoneInteger { get; set; }

    /// <summary>
    /// Returns an integer based on the given <see cref="LegendPosition"/>.
    /// </summary>
    /// <param name="value">The <see cref="LegendPosition"/> value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>An integer based on the given <see cref="LegendPosition"/>.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      LegendPosition lp = (LegendPosition)value;
      switch(lp)
      {
        case LegendPosition.Right:
          return RightInteger;
        case LegendPosition.Left:
          return LeftInteger;
        case LegendPosition.Top:
          return TopInteger;
        case LegendPosition.Bottom:
          return BottomInteger;
        case LegendPosition.Center:
          return CenterInteger;
        case LegendPosition.None:
          return NoneInteger;
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
