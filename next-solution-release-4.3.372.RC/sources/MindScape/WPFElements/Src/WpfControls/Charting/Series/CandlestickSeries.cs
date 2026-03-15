using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// A series primarily used for plotting open-high-low-close data in the form of candle sticks.
  /// </summary>
  public class CandlestickSeries : StockSeriesBase
  {
    static CandlestickSeries()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(CandlestickSeries),
        new FrameworkPropertyMetadata(typeof(CandlestickSeries)));
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
      Candlestick candleStick = GetCandlestick(o, index, out point);
      PrepareDataPoint(candleStick, index);

      double spacing = MinDelta == 0 ? XAxis.ActualMajorTickMarkSpacing : MinDelta;
      double boxWidth = XAxis.ConvertLogicalToPhysicalSize(spacing) * 0.6;

      double logicalLow = GetLow(candleStick);
      double logicalHigh = GetHigh(candleStick);
      double logicalOpen = GetOpen(candleStick);
      double logicalClose = GetClose(candleStick);

      double low = YAxis.ConvertLogicalToPhysical(logicalLow);
      double high = YAxis.ConvertLogicalToPhysical(logicalHigh);
      double open = YAxis.ConvertLogicalToPhysical(logicalOpen);
      double close = YAxis.ConvertLogicalToPhysical(logicalClose);

      point.Y = logicalLow;
      candleStick.LogicalPoint = point;
      //candleStick.YObject = logicalLow;
      if (LowBinding != null)
      {
        //candleStick.SetBinding(CartesianDataPoint.YObjectProperty, LowBinding);
      }
      candleStick.OpenPosition = high - open;
      candleStick.ClosePosition = high - close;
      candleStick.BoxWidth = boxWidth;
      Point normalPoint = ConvertLogicalToPhysicalPoint(point);

      double size = Math.Max(0, high - low);

      candleStick.LowPosition = size;
      if (open <= close && PositiveStyle != null)
      {
        candleStick.VisualStyle = PositiveStyle;
        candleStick.BorderBrush = GetPositiveBrush();
        candleStick.Background = GetPositiveFillBrush();
      }
      if (open > close && NegativeStyle != null)
      {
        candleStick.VisualStyle = NegativeStyle;
        candleStick.BorderBrush = GetNegativeBrush();
        candleStick.Background = GetNegativeFillBrush();
      }
      Canvas.SetLeft(candleStick, normalPoint.X);
      Canvas.SetTop(candleStick, normalPoint.Y - size);
      Canvas.Children.Add(candleStick);
    }

    internal Candlestick GetCandlestick(object o, int index, out Point point)
    {
      Candlestick stick = GetDataPoint(o, index) as Candlestick;
      if (stick == null)
      {
        stick = new Candlestick(o);
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
