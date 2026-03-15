using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// Plots a stair/step series on a <see cref="Chart"/> control.
  /// </summary>
  public class StepSeries : LineAreaSeriesBase
  {
    private Path _path;
    private Path _selectionPath;

    private PolyLineSegment _line;
    private PathFigure _figure;
    private Point _previousPoint;
    private Point? _previousNormalPoint = null;

    static StepSeries()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(StepSeries),
        new FrameworkPropertyMetadata(typeof(StepSeries)));
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
      _previousPoint = new Point(Double.NaN, Double.NaN);
    }

    internal override void PlotDataPoint(int index)
    {
      Object o = ItemsSource[index];
      Point point;
      ChartSymbol symbol = null;

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
          _line.Points.Add(new Point(normalPoint.X, _previousNormalPoint.Value.Y));
          _line.Points.Add(normalPoint);
        }

        SetupChartSymbol(symbol, normalPoint);
        _previousNormalPoint = normalPoint;
      }
      else
      {
        _line = new PolyLineSegment();
        _figure = new PathFigure();
        _figure.Segments.Add(_line);
        PathGeometry geo = _path.Data as PathGeometry;
        geo.Figures.Add(_figure);
        _previousNormalPoint = null;
      }
      _previousPoint = point;

      // Data Label:
      AddDataLabel(index, _path.Stroke); // TODO: this probably shouldn't be used for 'missing' data points that aren't rendered.
      //AddDataLabel(symbol, _path.Stroke); // TODO: this probably shouldn't be used for 'missing' data points that aren't rendered.
    }
  }
}
