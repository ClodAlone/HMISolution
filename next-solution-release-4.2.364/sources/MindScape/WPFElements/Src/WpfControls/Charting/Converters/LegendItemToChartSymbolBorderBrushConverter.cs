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
  /// Determins the border brush to be applied to a chart symbol for a given <see cref="LegendItem"/>.
  /// </summary>
  public class LegendItemToChartSymbolBorderBrushConverter : IValueConverter
  {
    /// <summary>
    /// Returns a border brush for a chart symbol for the given <see cref="LegendItem"/>.
    /// </summary>
    /// <param name="value">The <see cref="LegendItem"/> object produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>The border brush.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      LegendItem item = value as LegendItem;
      Brush brush = null;
      if (item != null)
      {
        PointSeriesBase series = item.DataSeries as PointSeriesBase;
        if (series != null)
        {
          ChartSymbol symbol = new ChartSymbol();
          symbol.Style = series.SymbolStyle;
          if (symbol.BorderBrush == null)
          {
            symbol.BorderBrush = series.SeriesBrush;
          }
          brush = symbol.BorderBrush;
        }
        else
        {
          PolarPointSeriesBase polarSeries = item.PolarSeries as PolarPointSeriesBase;
          if (polarSeries != null)
          {
            PolarChartSymbol symbol = new PolarChartSymbol();
            symbol.Style = polarSeries.SymbolStyle;
            if (symbol.BorderBrush == null)
            {
              symbol.BorderBrush = polarSeries.SeriesBrush;
            }
            brush = symbol.BorderBrush;
          }
        }
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
