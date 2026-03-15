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
  /// Plots a stacked line series on a <see cref="PolarChart"/> control.
  /// </summary>
  public class StackedPolarLineSeries : PolarLineSeries
  {
    private Dictionary<double, double> _dataCache;
    private Path _path;

    static StackedPolarLineSeries()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(StackedPolarLineSeries),
        new FrameworkPropertyMetadata(typeof(StackedPolarLineSeries)));
    }

    /*internal override Path LinePath
    {
      get { return _path; }
    }*/

    /// <summary>
    /// Plots the <see cref="StackedPolarLineSeries"/> on the chart canvas.
    /// </summary>
    protected override void BuildChartCore()
    {
      PolyLineSegment line = new PolyLineSegment();
      PathFigure figure;
      _path = GeometryUtils.BuildPath(line, out figure);
      figure.IsClosed = IsClosed;
      figure.IsFilled = false;
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

      Canvas.Children.Add(_path);

      StackedPolarLineSeries previousSeries = GetPreviousSeries();
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
        if (index == 0)
        {
          figure.StartPoint = normalPoint;
        }
        else
        {
          line.Points.Add(normalPoint);
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
    }

    private double GetData(double x)
    {
      // TODO: include the case where the previous series doesn't contain data at the given x value.
      double data;
      _dataCache.TryGetValue(x, out data);
      return data;
    }

    private StackedPolarLineSeries GetPreviousSeries()
    {
      int index = Series.IndexOf(this);
      for (int i = index - 1; i >= 0; i--)
      {
        if (Series[i] is StackedPolarLineSeries)
        {
          return Series[i] as StackedPolarLineSeries;
        }
      }
      return null;
    }

    internal override void AnalyseSeries()
    {
      StackedPolarLineSeries series = GetPreviousSeries();
      _dataCache = new Dictionary<double, double>();
      StackedSeriesUtils.AnalyseStackedSeries(this, series == null ? null : series._dataCache, _dataCache);
    }
  }
}
