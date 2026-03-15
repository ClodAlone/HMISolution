using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// Creates a <see cref="LinearGradientBrush"/> for displaying a <see cref="HeatmapSeries"/> legend icon.
  /// </summary>
  public class HeatmapGradientConverter : IValueConverter
  {
    private Orientation _orientation = Orientation.Horizontal;

    /// <summary>
    /// Gets or sets the orientation of the gradient.
    /// </summary>
    public Orientation Orientation
    {
      get { return _orientation; }
      set
      {
        _orientation = value;
      }
    }

    /// <summary>
    /// Converts a <see cref="GradientBrush"/> into a <see cref="LinearGradientBrush"/> that can be used to display a <see cref="HeatmapSeries"/> legend icon.
    /// </summary>
    /// <param name="value">The <see cref="GradientBrush"/> object produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>A <see cref="LinearGradientBrush"/>.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value is GradientBrush)
      {
        GradientBrush brush = (GradientBrush)value;
        if (brush.GradientStops.Count > 0)
        {
          double minOffset = brush.GradientStops[0].Offset;
          double maxOffset = brush.GradientStops[brush.GradientStops.Count - 1].Offset;
          double factor = 1.0 / (maxOffset - minOffset);
          LinearGradientBrush result = new LinearGradientBrush();
          result.StartPoint = Orientation == Orientation.Horizontal ? new Point(0, 0) : new Point(0, 1);
          result.EndPoint = Orientation == Orientation.Horizontal ? new Point(1, 0) : new Point(0, 0);
          foreach (GradientStop stop in brush.GradientStops)
          {
            result.GradientStops.Add(new GradientStop(stop.Color, (stop.Offset - minOffset) * factor));
          }
          return result;
        }
      }
      return value;
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
