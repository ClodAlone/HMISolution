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

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// Plots a spline series on a <see cref="PolarChart"/> control.
  /// </summary>
  public class PolarSplineSeries : PolarLineSeriesBase
  {
    static PolarSplineSeries()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(PolarSplineSeries),
        new FrameworkPropertyMetadata(typeof(PolarSplineSeries)));
    }

    /// <summary>
    /// Plots the <see cref="PolarSplineSeries"/> on the chart canvas.
    /// </summary>
    protected override void BuildChartCore()
    {
      Path spline = new Path();
      spline.IsHitTestVisible = false;
      PointCollection points = new PointCollection();
      Canvas.Children.Add(spline);

      if (LineStyle != null)
      {
        spline.Style = LineStyle;
      }
      if (spline.Stroke == null && SeriesBrush != null)
      {
        spline.Stroke = SeriesBrush;
      }
      if (DashArray != null)
      {
        DoubleCollection collection = new DoubleCollection();
        foreach (double d in DashArray)
        {
          collection.Add(d);
        }
        spline.StrokeDashArray = collection;
      }

      for (int index = 0; index < ItemsSource.Count; index++)
      {
        Object o = ItemsSource[index];
        PolarPoint point;
        PolarChartSymbol symbol = GetChartSymbol(o, index, out point);
        PrepareDataPoint(symbol, index);
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
        //AddDataLabel(index, _spline.Stroke);
      }

      PathGeometry geo = SplineUtils.CreateSpline(points, 0.4, null, IsClosed, false, 0.5);
      spline.Data = geo;
    }
  }
}
