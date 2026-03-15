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

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// Plots a scatter series on a <see cref="PolarChart"/> control.
  /// </summary>
  public class PolarScatterSeries : PolarPointSeriesBase
  {
    static PolarScatterSeries()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(PolarScatterSeries),
        new FrameworkPropertyMetadata(typeof(PolarScatterSeries)));
    }

    /// <summary>
    /// Plots the <see cref="PolarScatterSeries"/> on the chart canvas.
    /// </summary>
    protected override void BuildChartCore()
    {
      int index = 0;
      foreach (object o in ItemsSource)
      {
        PolarPoint point;
        PolarChartSymbol symbol = GetChartSymbol(o, index, out point);
        PrepareDataPoint(symbol, index);
        Point normalPoint = ConvertLogicalToPhysicalPoint(point);

        symbol.Style = SymbolStyle;
        if (symbol.Background == null && SeriesBrush != null)
        {
          symbol.Background = SeriesBrush;
        }
        if (symbol.BorderBrush == null && SeriesBrush != null)
        {
          symbol.BorderBrush = SeriesBrush;
        }
        Canvas.SetLeft(symbol, Math.Round(normalPoint.X));
        Canvas.SetTop(symbol, Math.Round(normalPoint.Y));
        Canvas.SetZIndex(symbol, 1);
        Canvas.Children.Add(symbol);

        // Data Label:
        //AddDataLabel(index, symbol.Background);
        index++;
      }
    }
  }
}
