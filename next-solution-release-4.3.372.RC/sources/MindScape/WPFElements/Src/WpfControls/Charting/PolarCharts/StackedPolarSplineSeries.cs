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
  /// Plots a spline series on a <see cref="PolarChart"/> control.
  /// </summary>
  public class StackedPolarSplineSeries : PolarLineSeriesBase
  {
    private Dictionary<double, double> _dataCache;
    private Path _spline;

    static StackedPolarSplineSeries()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(StackedPolarSplineSeries),
        new FrameworkPropertyMetadata(typeof(StackedPolarSplineSeries)));
    }

    /// <summary>
    /// Plots the <see cref="StackedSplineSeries"/> on the chart canvas.
    /// </summary>
    protected override void BuildChartCore()
    {
      _spline = new Path();
      _spline.IsHitTestVisible = false;
      PointCollection points = new PointCollection();
      Canvas.Children.Add(_spline);

      if (LineStyle != null)
      {
        _spline.Style = LineStyle;
      }
      if (_spline.Stroke == null && SeriesBrush != null)
      {
        _spline.Stroke = SeriesBrush;
      }
      if (DashArray != null)
      {
        DoubleCollection collection = new DoubleCollection();
        foreach (double d in DashArray)
        {
          collection.Add(d);
        }
        _spline.StrokeDashArray = collection;
      }

      StackedPolarSplineSeries previousSeries = GetPreviousSeries();
      _dataCache = new Dictionary<double, double>();

      for (int index = 0; index < ItemsSource.Count; index++)
      {
        Object o = ItemsSource[index];
        PolarPoint point;
        PolarChartSymbol symbol = GetChartSymbol(o, index, out point);
        PrepareDataPoint(symbol, index);

        if (previousSeries != null)
        {
          point.Rho += previousSeries.GetData(point.Theta);
        }
        _dataCache[point.Theta] = point.Rho;

        Point normalPoint = ConvertLogicalToPhysicalPoint(point);
        points.Add(normalPoint);

        symbol.Style = SymbolStyle;
        if (SymbolStyle != null)
        {
          if (symbol.Background == null && SeriesBrush != null)
          {
            symbol.Background = SeriesBrush;
          }
          if (symbol.BorderBrush == null && SeriesBrush != null)
          {
            symbol.BorderBrush = SeriesBrush;
          }
          Canvas.SetLeft(symbol, normalPoint.X);
          Canvas.SetTop(symbol, normalPoint.Y);
          Canvas.Children.Add(symbol);
        }

        // Data Label:
        /*if (ShowDataLabels)
        {
          Point nextPoint = point;
          if (index < ItemsSource.Count - 1)
          {
            ChartSymbol nextSymbol = new ChartSymbol(ItemsSource[index + 1]);
            nextPoint = GetPoint(nextSymbol);
            if (previousSeries != null)
            {
              nextPoint.Y += previousSeries.GetData(nextPoint.X);
            }
          }
          AddDataLabel(index, point, nextPoint, _spline.Stroke);
        }*/
      }

      PathGeometry geo = SplineUtils.CreateSpline(points, 0.4, null, IsClosed, false, 0.5);
      _spline.Data = geo;
    }

    private double GetData(double x)
    {
      // TODO: include the case where the previous series doesn't contain data at the given x value.
      double data;
      _dataCache.TryGetValue(x, out data);
      return data;
    }

    private StackedPolarSplineSeries GetPreviousSeries()
    {
      int index = Series.IndexOf(this);
      for (int i = index - 1; i >= 0; i--)
      {
        if (Series[i] is StackedPolarSplineSeries)
        {
          return Series[i] as StackedPolarSplineSeries;
        }
      }
      return null;
    }

    internal override void AnalyseSeries()
    {
      StackedPolarSplineSeries series = GetPreviousSeries();
      _dataCache = new Dictionary<double, double>();
      StackedSeriesUtils.AnalyseStackedSeries(this, series == null ? null : series._dataCache, _dataCache);
    }
  }
}
