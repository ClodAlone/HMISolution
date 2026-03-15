using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// Determins the size of a chart symbol for a given <see cref="LegendItem"/>.
  /// </summary>
  public class LegendItemToChartSymbolSizeConverter : IValueConverter
  {
    /// <summary>
    /// Returns the size for a chart symbol for the given <see cref="LegendItem"/>.
    /// </summary>
    /// <param name="value">The <see cref="LegendItem"/> object produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>A size as a double value.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      LegendItem item = (LegendItem)value;
      double size = Double.NaN;
      PointSeriesBase series = item.DataSeries as PointSeriesBase;
      if (series != null)
      {
        if (series.SymbolSize != 0)
        {
          size = series.SymbolSize;
        }
        else
        {
          ChartSymbol symbol = new ChartSymbol();
          symbol.Style = series.SymbolStyle;
          size = symbol.Width;
        }
      }
      return size;
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
