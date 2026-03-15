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
using System.Collections.Generic;

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// plots a stacked bar series on a <see cref="PolarChart"/> control.
  /// </summary>
  public class StackedRoseSeries : RoseSeries
  {
    private Dictionary<double, double> _dataCache;

    static StackedRoseSeries()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(StackedRoseSeries),
        new FrameworkPropertyMetadata(typeof(StackedRoseSeries)));
    }

    /// <summary>
    /// Plots the <see cref="StackedRoseSeries"/> on the chart canvas.
    /// </summary>
    protected override void BuildChartCore()
    {
      double logicalSpacing = BarSizeFactor;
      if (ItemsSource.Count > 1)
      {
        PolarPoint p1, p2;
        GetBar(ItemsSource[0], 0, out p1);
        GetBar(ItemsSource[1], 1, out p2);
        logicalSpacing *= (p2.Theta - p1.Theta);
      }
      double logicalRange = ThetaAxis.Maximum - ThetaAxis.Minimum;
      double ratio = logicalSpacing / logicalRange;
      double barWidthAngle = 360 * ratio;

      StackedRoseSeries previousSeries = GetPreviousSeries();
      _dataCache = new Dictionary<double, double>();

      int index = 0;
      foreach (object o in ItemsSource)
      {
        PolarPoint point;
        PolarBar bar = GetBar(o, index, out point);
        PrepareDataPoint(bar, index);

        double previousData = 0;
        if (previousSeries != null)
        {
          previousData = previousSeries.GetData(point.Theta);
          point.Rho += previousData;
        }
        _dataCache[point.Theta] = point.Rho;

        Point normalPoint = ConvertLogicalToPhysicalPoint(point);
        //Point previousPoint = ConvertLogicalToPhysicalPoint(new PolarPoint(0, previousData));
        //previousPoint.Y = Math.Round(previousPoint.Y);
        //normalPoint.Y = Math.Ceiling(normalPoint.Y);
        //normalPoint.X = Math.Ceiling(normalPoint.X);

        //double barLength = Math.Abs(previousPoint.Y - normalPoint.Y) + (previousData == 0 ? 1 : 0);

        bar.Style = BarStyle;
        if (bar.Background == null && SeriesBrush != null)
        {
          bar.Background = SeriesBrush;
        }
        if (bar.BorderBrush == null && SeriesBrush != null)
        {
          bar.BorderBrush = SeriesBrush;
        }

        double currentHeight = Math.Round(RhoAxis.ConvertLogicalToPhysical(point.Rho));
        double previousHeight = Math.Round(RhoAxis.ConvertLogicalToPhysical(previousData));

        double height = currentHeight - previousHeight;
        bar.Height = height + 1;
        double angle = ThetaAxis.ConvertLogicalToPhysical(point.Theta);
        RotateTransform rotation = new RotateTransform();
        rotation.Angle = angle;
        bar.RenderTransform = rotation;

        bar.UpdatePathData(currentHeight, previousHeight, barWidthAngle);

        Canvas.SetLeft(bar, normalPoint.X);
        Canvas.SetTop(bar, normalPoint.Y);
        Canvas.SetZIndex(bar, 1);
        Canvas.Children.Add(bar);

        // Data Label:
        //AddDataLabel(index, symbol.Background);
        index++;
      }
    }

    private double GetData(double x)
    {
      // TODO: include the case where the previous series doesn't contain data at the given x value.
      double length;
      _dataCache.TryGetValue(x, out length);
      return length;
    }

    private StackedRoseSeries GetPreviousSeries()
    {
      int index = Series.IndexOf(this);
      for (int i = index - 1; i >= 0; i--)
      {
        if (Series[i] is StackedRoseSeries)
        {
          return Series[i] as StackedRoseSeries;
        }
      }
      return null;
    }

    internal override void AnalyseSeries()
    {
      StackedRoseSeries series = GetPreviousSeries();
      _dataCache = new Dictionary<double, double>();
      StackedSeriesUtils.AnalyseStackedSeries(this, series == null ? null : series._dataCache, _dataCache);
    }
  }
}
