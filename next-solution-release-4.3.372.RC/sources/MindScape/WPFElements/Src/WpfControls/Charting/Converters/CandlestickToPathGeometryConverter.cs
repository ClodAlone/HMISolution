using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows;

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// Converts a <see cref="Candlestick"/> data point into a path geometry to create the standard candle stick chart graphics.
  /// </summary>
  public class CandlestickToPathGeometryConverter : IMultiValueConverter
  {
    /// <summary>
    /// Returns a <see cref="PathGeometry"/> for rendering a <see cref="Stick"/> based on the given values.
    /// The first value is the hight of the data point. (LowPosition)
    /// The second value is the distance from the top to the open value. (OpenPosition)
    /// The third value is the distance from the top to the close value. (ClosePosition)
    /// The fourth value is the width of the data point. (BoxWidth)
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
        double boxWidth = (double)values[3];
        double halfBoxWidth = boxWidth / 2.0;

        if (openPosition > closePosition)
        {
          double temp = openPosition;
          openPosition = closePosition;
          closePosition = temp;
        }

        PathFigure figure1 = new PathFigure();
        figure1.StartPoint = new Point(0, 0);
        LineSegment segment1 = new LineSegment() { Point = new Point(0, (int)openPosition) };
        figure1.Segments.Add(segment1);

        PathFigure figure2 = new PathFigure();
        figure2.StartPoint = new Point(0, (int)closePosition);
        LineSegment segment2 = new LineSegment() { Point = new Point(0, (int)height) };
        figure2.Segments.Add(segment2);

        PathFigure figure3 = new PathFigure();
        figure3.StartPoint = new Point(-halfBoxWidth, (int)openPosition);
        figure3.IsClosed = true;
        figure3.IsFilled = true;
        LineSegment segment3 = new LineSegment() { Point = new Point(halfBoxWidth, (int)openPosition) };
        LineSegment segment4 = new LineSegment() { Point = new Point(halfBoxWidth, (int)closePosition + ((int)openPosition - (int)closePosition == 0 ? 0.5 : 0)) };
        LineSegment segment5 = new LineSegment() { Point = new Point(-halfBoxWidth, (int)closePosition + ((int)openPosition - (int)closePosition == 0 ? 0.5 : 0)) };
        figure3.Segments.Add(segment3);
        figure3.Segments.Add(segment4);
        figure3.Segments.Add(segment5);

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
