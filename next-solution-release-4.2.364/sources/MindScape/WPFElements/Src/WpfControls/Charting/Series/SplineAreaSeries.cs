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
  /// Plots a spline area series on a <see cref="Chart"/> control.
  /// </summary>
  public class SplineAreaSeries : AreaSeries
  {
    private Path _spline;
    private Path _areaPath;

    private double _min;
    private double _max;
    private PointCollection _points;

    static SplineAreaSeries()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(SplineAreaSeries),
        new FrameworkPropertyMetadata(typeof(SplineAreaSeries)));
    }

    internal override Path LinePath
    {
      get { return _spline; }
    }

    internal override void PrepareToPlotData()
    {
      _areaPath = BuildAreaPath(_areaPath);

      // Build line
      if (_spline == null)
      {
        _spline = BuildLine(_spline);
      }
      else
      {
        UpdatePath(_spline);
      }
      //

      _points = new PointCollection();

      _min = Double.MaxValue;
      _max = Double.MinValue;
    }

    internal override void PlotDataPoint(int index)
    {
      Object o = ItemsSource[index];
      Point point;
      ChartSymbol symbol = GetChartSymbol(o, index, out point);
      PrepareDataPoint(symbol, index);
      Point normalPoint = ConvertLogicalToPhysicalPoint(point);
      _min = Math.Min(_min, point.X);
      _max = Math.Max(_max, point.X);
      _points.Add(normalPoint);

      SetupChartSymbol(symbol, normalPoint);

      // Data Label:
      AddDataLabel(index, _spline.Stroke);
    }

    internal override void FinishPlottingData()
    {
      PathGeometry geo = SplineUtils.CreateSpline(_points);
      _spline.Data = geo;

      Point minPoint = ConvertLogicalToPhysicalPoint(new Point(_min, 0));
      Point maxPoint = ConvertLogicalToPhysicalPoint(new Point(_max, 0));
      PathGeometry pathGeo = SplineUtils.CreateSpline(_points);
      PolyLineSegment pathSegment = pathGeo.Figures[0].Segments[0] as PolyLineSegment;
      pathSegment.Points.Add(maxPoint);
      pathSegment.Points.Add(minPoint);
      pathGeo.Figures[0].IsClosed = true;
      pathGeo.Figures[0].IsFilled = true;
      _areaPath.Data = pathGeo;
    }
  }
}
