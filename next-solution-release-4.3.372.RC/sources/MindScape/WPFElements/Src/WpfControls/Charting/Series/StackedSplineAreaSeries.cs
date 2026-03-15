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
  /// Plots a stacked spline area series on a <see cref="Chart"/> control.
  /// </summary>
  public class StackedSplineAreaSeries : AreaSeries
  {
    private Dictionary<double, double> _dataCache;
    private PolyLineSegment _lineCache;
    private Path _spline;
    private Path _areaPath;

    private double _min;
    private double _max;
    private StackedSplineAreaSeries _previousSeries;
    private PointCollection _points;

    static StackedSplineAreaSeries()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(StackedSplineAreaSeries),
        new FrameworkPropertyMetadata(typeof(StackedSplineAreaSeries)));
    }

    internal override Path LinePath
    {
      get { return _spline; }
    }

    internal override void PrepareToPlotData()
    {
      _areaPath = BuildAreaPath(_areaPath);

      // Build line
      if (_spline == null)
      {
        _spline = BuildLine(_spline);
      }
      else
      {
        UpdatePath(_spline);
      }
      //

      _points = new PointCollection();

      _previousSeries = GetPreviousSeries();
      _dataCache = new Dictionary<double, double>();

      _min = Double.MaxValue;
      _max = Double.MinValue;
    }

    internal override void PlotDataPoint(int index)
    {
      Object o = ItemsSource[index];
      Point point;
      ChartSymbol symbol = GetChartSymbol(o, index, out point);
      PrepareDataPoint(symbol, index);

      if (_previousSeries != null)
      {
        point.Y += _previousSeries.GetData(point.X);
      }
      _dataCache[point.X] = point.Y;

      Point normalPoint = ConvertLogicalToPhysicalPoint(point);
      _min = Math.Min(_min, point.X);
      _max = Math.Max(_max, point.X);
      _points.Add(normalPoint);

      SetupChartSymbol(symbol, normalPoint);
      Canvas.SetZIndex(symbol, 100);

      // Data Label:
      if (ShowDataLabels)
      {
        Point nextPoint = point;
        if (index < ItemsSource.Count - 1)
        {
          ChartSymbol nextSymbol = new ChartSymbol(ItemsSource[index + 1]);
          nextPoint = GetPoint(nextSymbol, index);
          if (_previousSeries != null)
          {
            nextPoint.Y += _previousSeries.GetData(nextPoint.X);
          }
        }
        AddDataLabel(index, point, nextPoint, _spline.Stroke);
      }
    }

    internal override void FinishPlottingData()
    {
      PathGeometry geo = SplineUtils.CreateSpline(_points);
      _spline.Data = geo;
      _lineCache = geo.Figures[0].Segments[0] as PolyLineSegment;

      Point minPoint = ConvertLogicalToPhysicalPoint(new Point(_min, 0));
      Point maxPoint = ConvertLogicalToPhysicalPoint(new Point(_max, 0));
      PathGeometry pathGeo = SplineUtils.CreateSpline(_points);
      PolyLineSegment pathSegment = pathGeo.Figures[0].Segments[0] as PolyLineSegment;
      if (_previousSeries != null)
      {
        pathSegment.Points.Add(ConvertLogicalToPhysicalPoint(new Point(_max, _previousSeries.GetData(_max))));
        for (int i = _previousSeries._lineCache.Points.Count - 1; i >= 0; i--)
        {
          pathSegment.Points.Add(_previousSeries._lineCache.Points[i]);
        }
        pathSegment.Points.Add(ConvertLogicalToPhysicalPoint(new Point(_min, _previousSeries.GetData(_min))));
      }
      else
      {
        pathSegment.Points.Add(maxPoint);
        pathSegment.Points.Add(minPoint);
      }
      pathGeo.Figures[0].IsClosed = true;
      pathGeo.Figures[0].IsFilled = true;
      _areaPath.Data = pathGeo;
    }

    private double GetData(double x)
    {
      // TODO: include the case where the previous series doesn't contain data at the given x value.
      double data;
      _dataCache.TryGetValue(x, out data);
      return data;
    }

    private StackedSplineAreaSeries GetPreviousSeries()
    {
      int index = Series.IndexOf(this);
      for (int i = index - 1; i >= 0; i--)
      {
        if (Series[i] is StackedSplineAreaSeries && Series[i].Visibility != Visibility.Collapsed)
        {
          return Series[i] as StackedSplineAreaSeries;
        }
      }
      return null;
    }

    internal override void AnalyseSeries()
    {
      _dataCache = new Dictionary<double, double>();
      base.AnalyseSeries();
    }

    internal override void GetDataPointDimensions(object dataPoint, Point point, out double dependentLow, out double dependentHigh)
    {
      if (Series != null)
      {
        StackedSplineAreaSeries series = GetPreviousSeries();
        StackedSeriesUtils.GetDataPointDimensions(this, series == null ? null : series._dataCache, _dataCache, point, out dependentLow, out dependentHigh);
      }
      else
      {
        dependentLow = point.Y;
        dependentHigh = point.Y;
      }
    }
  }
}
