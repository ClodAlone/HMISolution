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
  /// Plots an area series on a <see cref="Chart"/> control.
  /// </summary>
  public class AreaSeries : LineAreaSeriesBase
  {
    private Path _path;
    private Path _areaPath;

    private double _min;
    private double _max;
    private PathFigure _figure;
    private PathFigure _areaFigure;
    private PolyLineSegment _line;
    private PolyLineSegment _area;

    static AreaSeries()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(AreaSeries),
        new FrameworkPropertyMetadata(typeof(AreaSeries)));
    }

    #region AreaStyle property

    /// <summary>
    /// Gets or sets the <see cref="Style"/> to be applied to the area.
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
      DependencyProperty.Register("AreaStyle", typeof(Style), typeof(AreaSeries),
      new PropertyMetadata(BuildDefaultAreaStyle(), new PropertyChangedCallback(OnAreaStyleChanged)));

    private static Style BuildDefaultAreaStyle()
    {
      Style style = new Style(typeof(Path));
      LinearGradientBrush background = new LinearGradientBrush(); // TODO: looks like this isn't being used.
      background.StartPoint = new Point(0, 0);
      background.EndPoint = new Point(0, 1);
      background.GradientStops.Add(new GradientStop() { Offset = 0, Color = new Color() { A = 119, R = 51, G = 51, B = 221 } });
      background.GradientStops.Add(new GradientStop() { Offset = 1, Color = new Color() { A = 209, R = 51, G = 51, B = 221 } });
      //style.Setters.Add(new Setter(Path.FillProperty, background));
      return style;
    }

    private static void OnAreaStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((AreaSeries)d).OnAreaStyleChanged();
    }

    private void OnAreaStyleChanged()
    {
      if (_areaPath != null && AreaStyle != null)
      {
        _areaPath.Style = AreaStyle;
      }
    }

    #endregion // AreaStyle property

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

      _min = Double.MaxValue;
      _max = Double.MinValue;
    }

    internal override void PlotDataPoint(int index)
    {
      Object o = ItemsSource[index];
      Point point;
      ChartSymbol symbol = GetChartSymbol(o, index, out point);
      PrepareDataPoint(symbol, index);

      _min = Math.Min(_min, point.X);
      _max = Math.Max(_max, point.X);

      Point normalPoint = ConvertLogicalToPhysicalPoint(point);
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

      // Data Label:
      AddDataLabel(index, _path.Stroke);
    }

    internal override void FinishPlottingData()
    {
      //base.FinishPlottingData();
      _area.Points.Add(ConvertLogicalToPhysicalPoint(new Point(_max, 0)));
      _area.Points.Add(ConvertLogicalToPhysicalPoint(new Point(_min, 0)));
    }

    internal Path BuildAreaPath(Path oldPath)
    {
      // Remove event handler
      if (oldPath != null)
      {
        oldPath.MouseLeftButtonDown -= new MouseButtonEventHandler(Path_MouseLeftButtonDown);
        oldPath.MouseRightButtonDown -= new MouseButtonEventHandler(Path_MouseLeftButtonDown);
      }
      // Build path and apply styling
      Path areaPath = new Path();
      areaPath.DataContext = this;
      if (AreaStyle != null)
      {
        areaPath.Style = AreaStyle;
      }
      if (areaPath.Fill == null && SeriesBrush != null)
      {
        areaPath.Fill = GetAreaBrush();
      }
      // Add event handler
      areaPath.MouseLeftButtonDown += new MouseButtonEventHandler(Path_MouseLeftButtonDown);
      areaPath.MouseRightButtonDown += new MouseButtonEventHandler(Path_MouseLeftButtonDown);
      // Add to canvas
      Canvas.Children.Add(areaPath);
      return areaPath;
    }

    /// <summary>
    /// Returns the <see cref="Brush"/> to be used on the area if the AreaStyle did not cause the area to be given a fill.
    /// </summary>
    /// <returns>The <see cref="Brush"/> used to color the area.</returns>
    internal Brush GetAreaBrush()
    {
      SolidColorBrush seriesBrush = SeriesBrush as SolidColorBrush;
      if (seriesBrush != null)
      {
        LinearGradientBrush background = new LinearGradientBrush();
        background.StartPoint = new Point(0, 0);
        background.EndPoint = new Point(0, 1);
        byte r = seriesBrush.Color.R;
        byte g = seriesBrush.Color.G;
        byte b = seriesBrush.Color.B;
        background.GradientStops.Add(new GradientStop() { Offset = 0, Color = new Color() { A = 119, R = r, G = g, B = b } });
        background.GradientStops.Add(new GradientStop() { Offset = 1, Color = new Color() { A = 209, R = r, G = g, B = b } });
        return background;
      }
      return SeriesBrush;
    }
  }
}
