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
  /// Plots a stacked line series on a <see cref="Chart"/> control.
  /// </summary>
  public class StackedLineSeries : LineAreaSeriesBase
  {
    private Dictionary<double, double> _dataCache;
    private Path _path;
    private Path _selectionPath;

    private StackedLineSeries _previousSeries;
    private PathFigure _figure;
    private PolyLineSegment _line;

    static StackedLineSeries()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(StackedLineSeries),
        new FrameworkPropertyMetadata(typeof(StackedLineSeries)));
    }

    internal override Path LinePath
    {
      get { return _path; }
    }

    internal override void PrepareToPlotData()
    {
      _selectionPath = BuildSelectionLine(_selectionPath);

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

      _selectionPath.Data = _path.Data;
      _previousSeries = GetPreviousSeries();
      _dataCache = new Dictionary<double, double>();
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
      if (index == MinimumIndex)
      {
        _figure.StartPoint = normalPoint;
      }
      else
      {
        _line.Points.Add(normalPoint);
      }

      SetupChartSymbol(symbol, normalPoint);

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

    private double GetData(double x)
    {
      // TODO: include the case where the previous series doesn't contain data at the given x value.
      double data;
      _dataCache.TryGetValue(x, out data);
      return data;
    }

    private StackedLineSeries GetPreviousSeries()
    {
      int index = Series.IndexOf(this);
      for (int i = index - 1; i >= 0; i--)
      {
        if (Series[i] is StackedLineSeries && Series[i].Visibility != Visibility.Collapsed)
        {
          return Series[i] as StackedLineSeries;
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
        StackedLineSeries series = GetPreviousSeries();
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
