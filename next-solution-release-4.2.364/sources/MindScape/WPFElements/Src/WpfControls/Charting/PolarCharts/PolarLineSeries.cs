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
  /// Plots a line series on a <see cref="PolarChart"/> control.  A <see cref="PolarLineSeries"/> is represented
  /// by a sequence of symbols, joined by line segments.
  /// </summary>
  public class PolarLineSeries : PolarLineSeriesBase
  {
    static PolarLineSeries()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(PolarLineSeries),
        new FrameworkPropertyMetadata(typeof(PolarLineSeries)));
    }
    
    /// <summary>
    /// Plots the <see cref="PolarLineSeries"/> on the chart canvas.
    /// </summary>
    protected override void BuildChartCore()
    {
      PolyLineSegment line = new PolyLineSegment();
      PathFigure figure;
      Path path = GeometryUtils.BuildPath(line, out figure);
      figure.IsClosed = IsClosed;
      figure.IsFilled = false;
      path.IsHitTestVisible = false;

      if (LineStyle != null)
      {
        path.Style = LineStyle;
      }
      if (DashArray != null)
      {
        DoubleCollection collection = new DoubleCollection();
        foreach (double d in DashArray)
        {
          collection.Add(d);
        }
        path.StrokeDashArray = collection;
      }
      if (path.Stroke == null && SeriesBrush != null)
      {
        path.Stroke = SeriesBrush;
      }

      Canvas.Children.Add(path);

      for (int i = 0; i < ItemsSource.Count; i++)
      {
        Object o = ItemsSource[i];
        PolarPoint point;
        PolarChartSymbol symbol = GetChartSymbol(o, i, out point);
        PrepareDataPoint(symbol, i);
        Point normalPoint = ConvertLogicalToPhysicalPoint(point);
        if (i == 0)
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
          Canvas.SetLeft(symbol, Math.Round(normalPoint.X));
          Canvas.SetTop(symbol, Math.Round(normalPoint.Y));
          if (symbol.Parent == null)
          {
            Canvas.Children.Add(symbol);
          }
        }

        // Data Label:
        //AddDataLabel(i, _path.Stroke);
      }
    }
  }
}
