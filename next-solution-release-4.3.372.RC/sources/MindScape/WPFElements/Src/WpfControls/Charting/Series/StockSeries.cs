using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// A data series primarily used for plotting open-high-low-close data in the form of a
  /// vertical line with an open tick to the left and a close tick to the right.
  /// </summary>
  public class StockSeries : StockSeriesBase
  {
    static StockSeries()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(StockSeries),
        new FrameworkPropertyMetadata(typeof(StockSeries)));
    }

    /// <summary>
    /// Gets whether or not this <see cref="BarSeries"/> supports data sampling.
    /// </summary>
    protected override bool SupportsDataSampling
    {
      get
      {
        return IsDataOrdered;
      }
    }

    internal override void PlotDataPoint(int index)
    {
      object o = ItemsSource[index];
      Point point;
      Stick stick = GetStick(o, index, out point);
      if (YBinding == null && LowBinding != null)
      {
        stick.SetBinding(CartesianDataPoint.YObjectProperty, LowBinding);
      }
      PrepareDataPoint(stick, index);

      double logicalLow = GetLow(stick);
      double logicalHigh = GetHigh(stick);
      double logicalOpen = GetOpen(stick);
      double logicalClose = GetClose(stick);

      double low = YAxis.ConvertLogicalToPhysical(logicalLow);
      double high = YAxis.ConvertLogicalToPhysical(logicalHigh);
      double open = YAxis.ConvertLogicalToPhysical(logicalOpen);
      double close = YAxis.ConvertLogicalToPhysical(logicalClose);

      point.Y = logicalLow;
      stick.LogicalPoint = point;
      //stick.YObject = logicalLow;
      stick.OpenPosition = high - open;
      stick.ClosePosition = high - close;
      Point normalPoint = ConvertLogicalToPhysicalPoint(point);

      double size = Math.Max(0, high - low);

      stick.LowPosition = size;
      if (open <= close && PositiveStyle != null)
      {
        stick.VisualStyle = PositiveStyle;
        stick.BorderBrush = GetPositiveBrush();
      }
      if (open > close && NegativeStyle != null)
      {
        stick.VisualStyle = NegativeStyle;
        stick.BorderBrush = GetNegativeBrush();
      }
      Canvas.SetLeft(stick, normalPoint.X);
      Canvas.SetTop(stick, normalPoint.Y - size);
      Canvas.Children.Add(stick);
    }

    internal Stick GetStick(object o, int index, out Point point)
    {
      Stick stick = GetDataPoint(o, index) as Stick;
      if (stick == null)
      {
        stick = new Stick(o);
        point = GetPoint(stick, index);
      }
      else
      {
        point = stick.LogicalPoint;
      }
      return stick;
    }
  }
}
