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
  /// Returns the margin to be used on the chart symbol for a given <see cref="LegendItem"/>.
  /// </summary>
  public class LegendItemToAreaChartSymbolMarginConverter : IValueConverter
  {
    /// <summary>
    /// Returns a <see cref="Thickness"/> to be used on the chart symbol for the given <see cref="LegendItem"/>.
    /// </summary>
    /// <param name="value">The <see cref="Thickness"/> value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>The <see cref="Thickness"/> for the chart symbol.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      LegendItem item = value as LegendItem;
      Thickness margin = new Thickness();
      if (item != null)
      {
        AreaSeries series = item.DataSeries as AreaSeries;
        if (series != null)
        {
          ChartSymbol symbol = new ChartSymbol();
          symbol.Style = series.SymbolStyle;
          margin = new Thickness(-1, symbol.Margin.Top, 1, symbol.Margin.Bottom);
        }
        else
        {
          RadarSeries polarSeries = item.PolarSeries as RadarSeries;
          if (polarSeries != null)
          {
            PolarChartSymbol symbol = new PolarChartSymbol();
            symbol.Style = polarSeries.SymbolStyle;
            margin = new Thickness(-1, symbol.Margin.Top, 1, symbol.Margin.Bottom);
          }
        }
      }
      return margin;
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
