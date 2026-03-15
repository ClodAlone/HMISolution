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
  /// Plots a line series on a <see cref="Chart"/> control.  A line series is represented
  /// by a sequence of symbols, joined by lines.
  /// </summary>
  public class LineSeries : LineAreaSeriesBase
  {
    private Path _path;
    private Path _selectionPath;

    private PolyLineSegment _line;
    private PathFigure _figure;
    private Point _previousPoint;

    static LineSeries()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(LineSeries),
        new FrameworkPropertyMetadata(typeof(LineSeries)));
    }

    internal override Path LinePath
    {
      get { return _path; }
    }

    internal override void PrepareToPlotData()
    {
      _selectionPath = BuildSelectionLine(_selectionPath);

      // Update line
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
      _previousPoint = new Point(Double.NaN, Double.NaN);
    }

    internal override void PlotDataPoint(int index)
    {
      Object o = ItemsSource[index];
      Point point;
      ChartSymbol symbol = null;// = GetChartSymbol(o, index, out point);

      if (SymbolStyle == null)
      {
        point = GetPoint(o, index);
      }
      else
      {
        symbol = GetChartSymbol(o, index, out point);
        PrepareDataPoint(symbol, index);
      }

      if (!Double.IsNaN(point.X) && !Double.IsNaN(point.Y))
      {
        Point normalPoint = ConvertLogicalToPhysicalPoint(point);
        if (Double.IsNaN(_previousPoint.X) || Double.IsNaN(_previousPoint.Y))
        {
          _figure.StartPoint = normalPoint;
        }
        else
        {
          _line.Points.Add(normalPoint);
        }

        SetupChartSymbol(symbol, normalPoint);
      }
      else
      {
        _line = new PolyLineSegment();
        _figure = new PathFigure();
        _figure.Segments.Add(_line);
        PathGeometry geo = _path.Data as PathGeometry;
        geo.Figures.Add(_figure);
      }
      _previousPoint = point;

      // Data Label:
      AddDataLabel(index, _path.Stroke); // TODO: this probably shouldn't be used for 'missing' data points that aren't rendered.
      //AddDataLabel(symbol, _path.Stroke); // TODO: this probably shouldn't be used for 'missing' data points that aren't rendered.
    }
  }
}
