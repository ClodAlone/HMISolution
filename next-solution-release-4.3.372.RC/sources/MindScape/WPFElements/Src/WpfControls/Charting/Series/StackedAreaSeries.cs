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
  /// Plots a stacked area series on a <see cref="Chart"/> control.
  /// </summary>
  public class StackedAreaSeries : AreaSeries
  {
    private IList<Point> _pointsCache;
    private Dictionary<double, double> _dataCache;
    private Path _path;
    private Path _areaPath;

    private double _min;
    private double _max;
    private StackedAreaSeries _previousSeries;
    private PathFigure _figure;
    private PathFigure _areaFigure;
    private PolyLineSegment _line;
    private PolyLineSegment _area;

    static StackedAreaSeries()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(StackedAreaSeries),
        new FrameworkPropertyMetadata(typeof(StackedAreaSeries)));
    }

    internal override Path LinePath
    {
      get { return _path; }
    }

    internal override void PrepareToPlotData()
    {
      _areaPath = BuildAreaPath(_areaPath);

      PathGeometry geo = new PathGeometry();
      _areaFigure = new PathFigure();
      _area = new PolyLineSegment();
      _areaFigure.Segments.Add(_area);
      geo.Figures.Add(_areaFigure);
      _areaPath.Data = geo;
      _areaFigure.IsFilled = true;
      _areaFigure.IsClosed = true;

      // Build line
      if (_path == null)
      {
        _path = BuildLine(_path);
      }
      else
      {
        UpdatePath(_path);
      }
      PathGeometry geometry = new PathGeometry();
      _figure = new PathFigure();
      _line = new PolyLineSegment();
      _figure.Segments.Add(_line);
      geometry.Figures.Add(_figure);
      _path.Data = geometry;
      //

      _previousSeries = GetPreviousSeries();
      _pointsCache = new List<Point>();
      if (_dataCache == null)
      {
        _dataCache = new Dictionary<double, double>();
      }

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

      _min = Math.Min(_min, point.X);
      _max = Math.Max(_max, point.X);

      Point normalPoint = ConvertLogicalToPhysicalPoint(point);
      _pointsCache.Add(normalPoint);

      if (index == MinimumIndex)
      {
        _figure.StartPoint = normalPoint;
        _areaFigure.StartPoint = normalPoint;
      }
      else
      {
        _line.Points.Add(normalPoint);
        _area.Points.Add(normalPoint);
      }

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
        AddDataLabel(index, point, nextPoint, _path.Stroke);
      }
    }

    internal override void FinishPlottingData()
    {
      if (_previousSeries != null)
      {
        _area.Points.Add(ConvertLogicalToPhysicalPoint(new Point(_max, _previousSeries.GetData(_max))));
        for (int i = _previousSeries._pointsCache.Count - 1; i >= 0; i--)
        {
          _area.Points.Add(_previousSeries._pointsCache[i]);
        }
        _area.Points.Add(ConvertLogicalToPhysicalPoint(new Point(_min, _previousSeries.GetData(_min))));
      }
      else
      {
        _area.Points.Add(ConvertLogicalToPhysicalPoint(new Point(_max, 0)));
        _area.Points.Add(ConvertLogicalToPhysicalPoint(new Point(_min, 0)));
      }
    }

    private double GetData(double x)
    {
      // TODO: include the case where the previous series doesn't contain data at the given x value.
      double data;
      _dataCache.TryGetValue(x, out data);
      return data;
    }

    private StackedAreaSeries GetPreviousSeries()
    {
      int index = Series.IndexOf(this);
      for (int i = index - 1; i >= 0; i--)
      {
        if (Series[i] is StackedAreaSeries && Series[i].Visibility != Visibility.Collapsed)
        {
          return Series[i] as StackedAreaSeries;
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
        StackedAreaSeries series = GetPreviousSeries();
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
