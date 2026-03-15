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
  /// Plots a radar series on the <see cref="PolarChart"/> control.
  /// A radar series is the polar version of a cartesian <see cref="AreaSeries"/>.
  /// </summary>
  public class RadarSeries : PolarLineAreaSeriesBase
  {
    private Path _path;

    static RadarSeries()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(RadarSeries),
        new FrameworkPropertyMetadata(typeof(RadarSeries)));
    }

    #region AreaStyle property

    /// <summary>
    /// Gets or sets the AreaStyle.
    /// This is a dependency property.
    /// </summary>
    public Style AreaStyle
    {
      get { return (Style)GetValue(AreaStyleProperty); }
      set { SetValue(AreaStyleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="AreaStyle"/> property.
    /// </summary>
    public static readonly DependencyProperty AreaStyleProperty =
      DependencyProperty.Register("AreaStyle", typeof(Style), typeof(RadarSeries),
      new PropertyMetadata(new PropertyChangedCallback(OnAreaStyleChanged)));

    private static void OnAreaStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((RadarSeries)d).OnAreaStyleChanged();
    }

    private void OnAreaStyleChanged()
    {
      // TODO: apply new style to area.
    }

    #endregion // AreaStyle property
    
    /// <summary>
    /// Plots the <see cref="RadarSeries"/> on the chart canvas.
    /// </summary>
    protected override void BuildChartCore()
    {
      PolyLineSegment area = new PolyLineSegment();
      PathFigure areaFigure;
      Path areaPath = GeometryUtils.BuildPath(area, out areaFigure);
      areaFigure.IsFilled = true;
      areaFigure.IsClosed = true;
      if (AreaStyle != null)
      {
        areaPath.Style = AreaStyle;
      }
      if (areaPath.Fill == null && SeriesBrush != null)
      {
        areaPath.Fill = GetAreaBrush();
      }

      PolyLineSegment line = new PolyLineSegment();
      PathFigure lineFigure;
      _path = GeometryUtils.BuildPath(line, out lineFigure);
      lineFigure.IsClosed = true;
      lineFigure.IsFilled = false;
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

      double min = Double.MaxValue;
      double max = Double.MinValue;
      int index = 0;
      int maxIndex = ItemsSource.Count - 1;
      while (true)
      {
        if (index > maxIndex)
        {
          index = maxIndex;
        }
        Object o = ItemsSource[index];
        PolarPoint point;
        PolarChartSymbol symbol = GetChartSymbol(o, index, out point);
        PrepareDataPoint(symbol, index);

        min = Math.Min(min, point.Theta);
        max = Math.Max(max, point.Theta);

        Point normalPoint = ConvertLogicalToPhysicalPoint(point);
        if (index == 0)
        {
          lineFigure.StartPoint = normalPoint;
          areaFigure.StartPoint = normalPoint;
        }
        else
        {
          line.Points.Add(normalPoint);
          area.Points.Add(normalPoint);
        }

        if (SymbolStyle != null)
        {
          symbol.Style = SymbolStyle;
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
        //AddDataLabel(index, _path.Stroke);

        if (index == maxIndex)
        {
          break;
        }
        index ++;
      }
    }
  }
}
