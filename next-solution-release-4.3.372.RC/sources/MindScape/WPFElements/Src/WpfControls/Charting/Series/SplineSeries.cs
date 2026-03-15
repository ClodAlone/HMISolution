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
  /// Plots a spline series on a <see cref="Chart"/> control.
  /// </summary>
  public class SplineSeries : LineAreaSeriesBase
  {
    private Path _spline;
    private Path _selectionPath;

    private Point _previousPoint;

    private PointCollection _points;
    private List<int> _missingIndices;
    private int _nullPointCount;

    static SplineSeries()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(SplineSeries),
        new FrameworkPropertyMetadata(typeof(SplineSeries)));
    }

    internal override Path LinePath
    {
      get { return _spline; }
    }

    internal override void PrepareToPlotData()
    {
      _selectionPath = BuildSelectionLine(_selectionPath);

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
      _missingIndices = new List<int>();
      _previousPoint = new Point(Double.NaN, Double.NaN);
      _nullPointCount = 0;
    }

    internal override void PlotDataPoint(int index)
    {
      Object o = ItemsSource[index];
      Point point;
      ChartSymbol symbol = GetChartSymbol(o, index, out point);
      PrepareDataPoint(symbol, index);
      Point normalPoint = ConvertLogicalToPhysicalPoint(point);

      if (!Double.IsNaN(point.X) && !Double.IsNaN(point.Y))
      {
        _points.Add(normalPoint);
      }
      else if (!Double.IsNaN(_previousPoint.X) && !Double.IsNaN(_previousPoint.Y))
      {
        _missingIndices.Add(index - 1 - _nullPointCount);
        _nullPointCount++;
      }
      else
      {
        _nullPointCount++;
      }

      SetupChartSymbol(symbol, normalPoint);
      _previousPoint = point;

      // Data Label:
      AddDataLabel(index, _spline.Stroke);
    }

    internal override void FinishPlottingData()
    {
      PathGeometry geo = SplineUtils.CreateSpline(_points, _missingIndices);
      _spline.Data = geo;
      _selectionPath.Data = geo;
    }
  }
}
