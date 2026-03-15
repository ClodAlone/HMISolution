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
  /// plots a spline area series on a <see cref="PolarChart"/> control.
  /// </summary>
  public class StackedPolarSplineAreaSeries : RadarSeries
  {
    private Dictionary<double, double> _dataCache;
    private PolyLineSegment _lineCache;
    private Path _spline;

    static StackedPolarSplineAreaSeries()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(StackedPolarSplineAreaSeries),
        new FrameworkPropertyMetadata(typeof(StackedPolarSplineAreaSeries)));
    }

    /// <summary>
    /// Plots the <see cref="StackedPolarSplineAreaSeries"/> on the chart canvas.
    /// </summary>
    protected override void BuildChartCore()
    {
      Path area = new Path();
      area.IsHitTestVisible = false;
      if (AreaStyle != null)
      {
        area.Style = AreaStyle;
      }
      if (area.Fill == null && SeriesBrush != null)
      {
        area.Fill = GetAreaBrush();
      }
      Canvas.Children.Add(area);

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

      StackedPolarSplineAreaSeries previousSeries = GetPreviousSeries();
      _dataCache = new Dictionary<double, double>();

      double min = Double.MaxValue;
      double max = Double.MinValue;
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
        min = Math.Min(min, point.Theta);
        max = Math.Max(max, point.Theta);
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
          Canvas.SetZIndex(symbol, 100);
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

      PathGeometry geo = SplineUtils.CreateSpline(points, 0.4, null, true, false, 0.5);
      _spline.Data = geo;
      _lineCache = geo.Figures[0].Segments[0] as PolyLineSegment;

      //Point minPoint = ConvertLogicalToPhysicalPoint(new PolarPoint(min, 0));
      //Point maxPoint = ConvertLogicalToPhysicalPoint(new PolarPoint(max, 0));
      PathGeometry pathGeo = SplineUtils.CreateSpline(points, 0.4, null, true, false, 0.5);
      PolyLineSegment pathSegment = pathGeo.Figures[0].Segments[0] as PolyLineSegment;
      if (previousSeries != null)
      {
        /*pathSegment.Points.Add(ConvertLogicalToPhysicalPoint(new PolarPoint(max, previousSeries.GetData(max))));
        for (int i = previousSeries._lineCache.Points.Count - 1; i >= 0; i--)
        {
          pathSegment.Points.Add(previousSeries._lineCache.Points[i]);
        }
        pathSegment.Points.Add(ConvertLogicalToPhysicalPoint(new PolarPoint(min, previousSeries.GetData(min))));*/
        pathSegment.Points.Add(ConvertLogicalToPhysicalPoint(new PolarPoint(0, GetData(0))));
        foreach (Point point in previousSeries._lineCache.Points)
        {
          pathSegment.Points.Add(point);
        }
        if (previousSeries._lineCache.Points.Count > 0)
        {
          pathSegment.Points.Add(previousSeries._lineCache.Points[0]);
        }
      }
      /*else
      {
        pathSegment.Points.Add(maxPoint);
        pathSegment.Points.Add(minPoint);
      }*/
      pathGeo.Figures[0].IsClosed = true;
      pathGeo.Figures[0].IsFilled = true;
      area.Data = pathGeo;
    }

    private double GetData(double x)
    {
      // TODO: include the case where the previous series doesn't contain data at the given x value.
      double data;
      _dataCache.TryGetValue(x, out data);
      return data;
    }

    private StackedPolarSplineAreaSeries GetPreviousSeries()
    {
      int index = Series.IndexOf(this);
      for (int i = index - 1; i >= 0; i--)
      {
        if (Series[i] is StackedPolarSplineAreaSeries)
        {
          return Series[i] as StackedPolarSplineAreaSeries;
        }
      }
      return null;
    }

    internal override void AnalyseSeries()
    {
      StackedPolarSplineAreaSeries series = GetPreviousSeries();
      _dataCache = new Dictionary<double, double>();
      StackedSeriesUtils.AnalyseStackedSeries(this, series == null ? null : series._dataCache, _dataCache);
    }
  }
}
