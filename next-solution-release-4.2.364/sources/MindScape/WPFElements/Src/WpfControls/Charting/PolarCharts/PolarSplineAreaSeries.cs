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
  /// Plots a spline area series on a <see cref="PolarChart"/> control.
  /// </summary>
  public class PolarSplineAreaSeries : RadarSeries
  {
    static PolarSplineAreaSeries()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(PolarSplineAreaSeries),
        new FrameworkPropertyMetadata(typeof(PolarSplineAreaSeries)));
    }

    /// <summary>
    /// Plots the <see cref="PolarSplineAreaSeries"/> on the chart canvas.
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

      Path _spline = new Path();
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

      double min = Double.MaxValue;
      double max = Double.MinValue;
      for (int index = 0; index < ItemsSource.Count; index++)
      {
        Object o = ItemsSource[index];
        PolarPoint point;
        PolarChartSymbol symbol = GetChartSymbol(o, index, out point);
        PrepareDataPoint(symbol, index);
        Point normalPoint = ConvertLogicalToPhysicalPoint(point);
        min = Math.Min(min, point.Theta);
        max = Math.Max(max, point.Rho);
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

      PathGeometry geo = SplineUtils.CreateSpline(points, 0.4, null, true, false, 0.5);
      _spline.Data = geo;

      //Point minPoint = ConvertLogicalToPhysicalPoint(new Point(min, 0));
      //Point maxPoint = ConvertLogicalToPhysicalPoint(new Point(max, 0));
      PathGeometry pathGeo = SplineUtils.CreateSpline(points, 0.4, null, true, true, 0.5);
      //PolyLineSegment pathSegment = pathGeo.Figures[0].Segments[0] as PolyLineSegment;
      //pathSegment.Points.Add(maxPoint);
      //pathSegment.Points.Add(minPoint);
      //pathGeo.Figures[0].IsClosed = true;
      //pathGeo.Figures[0].IsFilled = true;
      area.Data = pathGeo;
    }
  }
}
