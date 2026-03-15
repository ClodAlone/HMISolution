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
  /// Returns a dash array to be used in the given <see cref="LegendItem"/>.
  /// </summary>
  public class LegendItemToDashArrayConverter : IValueConverter
  {
    /// <summary>
    /// Returns a dash array for the given <see cref="LegendItem"/>.
    /// </summary>
    /// <param name="value">The <see cref="LegendItem"/> object produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>The dash array.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      LegendItem item = value as LegendItem;
      DoubleCollection dashArray = new DoubleCollection();
      if (item != null)
      {
        LineAreaSeriesBase series = item.DataSeries as LineAreaSeriesBase;
        if (series != null && series.DashArray != null)
        {
          foreach (double d in series.DashArray)
          {
            dashArray.Add(d);
          }
        }
        else
        {
          PolarLineAreaSeriesBase polarSeries = item.PolarSeries as PolarLineAreaSeriesBase;
          if (polarSeries != null && polarSeries.DashArray != null)
          {
            foreach (double d in polarSeries.DashArray)
            {
              dashArray.Add(d);
            }
          }
        }
      }
      return dashArray;
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
