using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Shapes;

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// Converts a <see cref="Stick"/> data point into a path geometry to create the standard stock chart graphics.
  /// </summary>
  public class StickToPathGeometryConverter : IMultiValueConverter
  {
    /// <summary>
    /// Returns a <see cref="PathGeometry"/> for rendering a <see cref="Stick"/> based on the given values.
    /// The first value is the hight of the data point. (LowPosition)
    /// The second value is the distance from the top to the open value. (OpenPosition)
    /// The third value is the distance from the top to the close value. (ClosePosition)
    /// The fourth value is the current style. (VisualStyle)
    /// </summary>
    /// <param name="values">The values produced by the binding sources.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>A <see cref="PathGeometry"/> for rendering a <see cref="Stick"/>.</returns>
    public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      PathGeometry geo = new PathGeometry();
      if (values[0] != DependencyProperty.UnsetValue && values[1] != DependencyProperty.UnsetValue
        && values[2] != DependencyProperty.UnsetValue && values[3] != DependencyProperty.UnsetValue)
      {
        double height = (double)values[0];
        double openPosition = (double)values[1];
        double closePosition = (double)values[2];
        Style visualStyle = (Style)values[3];

        double centerOffset = 0;
        if (visualStyle != null)
        {
          foreach (Setter setter in visualStyle.Setters)
          {
            if (setter.Property == Path.StrokeThicknessProperty)
            {
              centerOffset = (double)setter.Value / 2.0;
            }
          }
        }

        PathFigure figure1 = new PathFigure();
        figure1.StartPoint = new Point(0, 0);
        LineSegment segment1 = new LineSegment() { Point = new Point(0, (int)height) };
        figure1.Segments.Add(segment1);

        PathFigure figure2 = new PathFigure();
        figure2.StartPoint = new Point(centerOffset, (int)openPosition);
        LineSegment segment2 = new LineSegment() { Point = new Point(-5, (int)openPosition) };
        figure2.Segments.Add(segment2);

        PathFigure figure3 = new PathFigure();
        figure3.StartPoint = new Point(-centerOffset, (int)closePosition);
        LineSegment segment3 = new LineSegment() { Point = new Point(5, (int)closePosition) };
        figure3.Segments.Add(segment3);

        geo.Figures.Add(figure1);
        geo.Figures.Add(figure2);
        geo.Figures.Add(figure3);
      }
      return geo;
    }

    /// <summary>
    /// Converts a value from a binding target for writing to multiple binding source.
    /// </summary>
    /// <param name="value">The value produced by the binding target.</param>
    /// <param name="targetTypes">The types to convert to.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>This conversion direction is not implemented by this converter.</returns>
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
