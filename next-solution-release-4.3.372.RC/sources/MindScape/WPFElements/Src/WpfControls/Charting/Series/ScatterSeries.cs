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
  /// Plots a scatter series on a <see cref="Chart"/> control.
  /// </summary>
  public class ScatterSeries : PointSeriesBase
  {
    private ScatterCache _scatterCache;

    static ScatterSeries()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(ScatterSeries),
        new FrameworkPropertyMetadata(typeof(ScatterSeries)));
    }

    /// <summary>
    /// Plots the <see cref="ScatterSeries"/> on the chart canvas.
    /// </summary>
    protected override void BuildChartCore()
    {
      int index = 0;
      foreach (object o in _scatterCache == null ? ItemsSource : _scatterCache.GetPointsToRender(XAxis.ActualMinimumValue, XAxis.ActualMaximumValue, YAxis.ActualMinimumValue, YAxis.ActualMaximumValue, AllowsDataSampling))
      {
        Point point;
        ChartSymbol symbol = GetChartSymbol(o, index, out point);
        if (symbol != null)
        {
          PrepareDataPoint(symbol, index);
          Point normalPoint = ConvertLogicalToPhysicalPoint(point);

          SetupChartSymbol(symbol, normalPoint);
          Canvas.SetZIndex(symbol, 1);

          // Data Label:
          AddDataLabel(index, symbol.Background);
        }
        index++;
      }
    }

    internal override void AnalyseSeries()
    {
      base.AnalyseSeries();
      OnUpdateAxes();
      RefreshScatterCache();
    }

    private void RefreshScatterCache()
    {
      if (XAxis != null && YAxis != null && ItemsSource != null)
      {
        _scatterCache = new ScatterCache(XAxis.MinimumValue, XAxis.MaximumValue, YAxis.MinimumValue, YAxis.MaximumValue, this);

        IAxisValueConverter xConverter = XAxis == null ? null : XAxis.ValueConverter;
        IAxisValueConverter yConverter = YAxis == null ? null : YAxis.ValueConverter;
        bool hasConverter = xConverter != null || yConverter != null;
        bool hasBinding = XBinding != null || YBinding != null;
        int count = 0;
        foreach (object o in ItemsSource)
        {
          if (o != null)
          {
            Point point = (!hasBinding && !hasConverter && o is Point) ? (Point)o : GetPoint(o, count);
            _scatterCache.Add(o, point);
          }
          count++;
        }
      }
    }

    // TODO: need to improve the performance of dynamically changing scatter series:

    internal override void DataPointAddedCore(object dataPoint, int index)
    {
      RefreshScatterCache();
      /*if (_scatterCache != null)
      {
        IAxisValueConverter xConverter = XAxis == null ? null : XAxis.ValueConverter;
        IAxisValueConverter yConverter = YAxis == null ? null : YAxis.ValueConverter;
        bool hasConverter = xConverter != null || yConverter != null;
        bool hasBinding = XBinding != null || YBinding != null;
        Point point = (!hasBinding && !hasConverter && dataPoint is Point) ? (Point)dataPoint : GetPoint(dataPoint, index);
        _scatterCache.Add(dataPoint, point);
      }*/
    }

    internal override void DataPointRemovedCore(object dataPoint, int index)
    {
      RefreshScatterCache();
    }

    internal override void ItemsSourceResetCore()
    {
      RefreshScatterCache();
    }
  }
}
