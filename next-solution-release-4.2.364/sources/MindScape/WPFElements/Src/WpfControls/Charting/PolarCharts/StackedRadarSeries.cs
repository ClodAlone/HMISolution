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
  /// Plots a stacked radar series on a <see cref="PolarChart"/> control.
  /// </summary>
  public class StackedRadarSeries : RadarSeries
  {
    private IList<Point> _pointsCache;
    private Dictionary<double, double> _dataCache;
    private Path _path;

    static StackedRadarSeries()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(StackedRadarSeries),
        new FrameworkPropertyMetadata(typeof(StackedRadarSeries)));
    }

    /*internal override Path LinePath
    {
      get { return _path; }
    }*/

    /// <summary>
    /// Plots the <see cref="StackedRadarSeries"/> on the chart canvas.
    /// </summary>
    protected override void BuildChartCore()
    {
      PolyLineSegment area = new PolyLineSegment();
      PathFigure areaFigure;
      Path areaPath = GeometryUtils.BuildPath(area, out areaFigure);
      areaFigure.IsClosed = true;
      areaFigure.IsFilled = true;
      areaPath.IsHitTestVisible = false;
      if (AreaStyle != null)
      {
        areaPath.Style = AreaStyle;
      }
      if (areaPath.Fill == null && SeriesBrush != null)
      {
        areaPath.Fill = GetAreaBrush();
      }

      PolyLineSegment line = new PolyLineSegment();
      PathFigure figure;
      _path = GeometryUtils.BuildPath(line, out figure);
      figure.IsClosed = true;
      _path.IsHitTestVisible = false;
      if (LineStyle != null)
      {
        _path.Style = LineStyle;
      }
      if (DashArray != null)
      {
        DoubleCollection collection = new DoubleCollection();
        foreach (double d in DashArray)
        {
          collection.Add(d);
        }
        _path.StrokeDashArray = collection;
      }
      if (_path.Stroke == null && SeriesBrush != null)
      {
        _path.Stroke = SeriesBrush;
      }

      Canvas.Children.Add(areaPath);
      Canvas.Children.Add(_path);

      StackedRadarSeries previousSeries = GetPreviousSeries();
      _pointsCache = new List<Point>();
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

        min = Math.Min(min, point.Theta);
        max = Math.Max(max, point.Theta);

        Point normalPoint = ConvertLogicalToPhysicalPoint(point);
        _pointsCache.Add(normalPoint);

        if (index == 0)
        {
          figure.StartPoint = normalPoint;
          areaFigure.StartPoint = normalPoint;
        }
        else
        {
          line.Points.Add(normalPoint);
          area.Points.Add(normalPoint);
        }

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
          AddDataLabel(index, point, nextPoint, _path.Stroke);
        }*/
      }

      if (previousSeries != null)
      {
        area.Points.Add(areaFigure.StartPoint);
        //area.Points.Add(ConvertLogicalToPhysicalPoint(new PolarPoint(max, previousSeries.GetData(max))));
        /*for (int i = previousSeries._pointsCache.Count - 1; i >= 0; i--)
        {
          area.Points.Add(previousSeries._pointsCache[i]);
        }*/
        foreach (Point point in previousSeries._pointsCache)
        {
          area.Points.Add(point);
        }
        if (previousSeries._pointsCache.Count > 0)
        {
          area.Points.Add(previousSeries._pointsCache[0]);
        }
        //area.Points.Add(ConvertLogicalToPhysicalPoint(new PolarPoint(min, previousSeries.GetData(min))));
      }
      /*else
      {
        area.Points.Add(ConvertLogicalToPhysicalPoint(new PolarPoint(max, 0)));
        area.Points.Add(ConvertLogicalToPhysicalPoint(new PolarPoint(min, 0)));
      }*/
    }

    private double GetData(double x)
    {
      // TODO: include the case where the previous series doesn't contain data at the given x value.
      double data;
      _dataCache.TryGetValue(x, out data);
      return data;
    }

    private StackedRadarSeries GetPreviousSeries()
    {
      int index = Series.IndexOf(this);
      for (int i = index - 1; i >= 0; i--)
      {
        if (Series[i] is StackedRadarSeries)
        {
          return Series[i] as StackedRadarSeries;
        }
      }
      return null;
    }

    internal override void AnalyseSeries()
    {
      StackedRadarSeries series = GetPreviousSeries();
      _dataCache = new Dictionary<double, double>();
      StackedSeriesUtils.AnalyseStackedSeries(this, series == null ? null : series._dataCache, _dataCache);
    }
  }
}
