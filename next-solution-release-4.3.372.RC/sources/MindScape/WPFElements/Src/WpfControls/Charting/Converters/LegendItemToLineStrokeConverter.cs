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
  /// Determins the stroke brush to be applied to a path for a given <see cref="LegendItem"/>.
  /// </summary>
  public class LegendItemToLineStrokeConverter : IValueConverter
  {
    /// <summary>
    /// Returns a line stroke to be applied to a path for the given <see cref="LegendItem"/>.
    /// </summary>
    /// <param name="value">The <see cref="LegendItem"/> object produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>The line stroke.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      LegendItem item = value as LegendItem;
      Brush brush = null;
      if (item != null)
      {
        Path path = new Path();
        LineAreaSeriesBase series = item.DataSeries as LineAreaSeriesBase;
        if (series != null)
        {
          path.Style = series.LineStyle;
          if (path.Stroke == null)
          {
            path.Stroke = series.SeriesBrush;
          }
        }
        else
        {
          PolarLineAreaSeriesBase polarSeries = item.PolarSeries as PolarLineAreaSeriesBase;
          if (polarSeries != null)
          {
            path.Style = polarSeries.LineStyle;
            if (path.Stroke == null)
            {
              path.Stroke = polarSeries.SeriesBrush;
            }
          }
          else
          {
            StockSeriesBase stockSeries = item.DataSeries as StockSeriesBase;
            if (stockSeries != null)
            {
              path.Style = stockSeries.PositiveStyle;
              if (path.Stroke == null)
              {
                path.Stroke = stockSeries.SeriesBrush;
              }
            }
            else
            {
              BoxplotSeries boxplotSeries = item.DataSeries as BoxplotSeries;
              if (boxplotSeries != null)
              {
                path.Stroke = boxplotSeries.SeriesBrush;
              }
            }
          }
        }

        brush = path.Stroke;
      }
      return brush;
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
