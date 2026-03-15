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
  /// Plots a stcaked bar series on a <see cref="Chart"/> control.
  /// </summary>
  public class StackedBarSeries : BarSeries
  {
    private Dictionary<double, double> _dataCache;

    private StackedBarSeries _previousSeries;
    private double _barSize;
    private double _totalBarSize;
    private int _seriesIndex;
    private Point _zeroPoint;

    static StackedBarSeries()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(StackedBarSeries),
        new FrameworkPropertyMetadata(typeof(StackedBarSeries)));
    }

    #region StackIdentifier property

    /// <summary>
    /// Gets or sets the StackIdentifier.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="StackIdentifierProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public object StackIdentifier
    {
      get { return GetValue(StackIdentifierProperty); }
      set { SetValue(StackIdentifierProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="StackIdentifier"/> property.
    /// </summary>
    public static readonly DependencyProperty StackIdentifierProperty =
      DependencyProperty.Register("StackIdentifier", typeof(object), typeof(StackedBarSeries),
      new FrameworkPropertyMetadata(OnStackIdentifierChanged));

    private static void OnStackIdentifierChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((StackedBarSeries)d).OnStackIdentifierChanged();
    }

    private void OnStackIdentifierChanged()
    {
      //TODO: refresh the whole chart control if this is changed dynamically.
    }

    #endregion // StackIdentifier property

    internal override void PrepareToPlotData()
    {
      if (ReverseAxes)
      {
        double barSeriesCount = StackCount;
        _seriesIndex = StackIndex;

        double spacing = MinDelta == 0 ? YAxis.ActualMajorTickMarkSpacing : MinDelta * IndexStep;
        if (LogicalBarSize > 0)
        {
          spacing = LogicalBarSize;
        }
        _totalBarSize = (YAxis.ConvertLogicalToPhysicalSize(spacing) * BarSizeFactor);// (int)(YAxis.GetPhysicalLabelSpacing(chartHeight) * barSizeRatio);
        _barSize = Math.Ceiling(_totalBarSize / barSeriesCount);

        _previousSeries = GetPreviousSeries();
        _dataCache = new Dictionary<double, double>();

        _zeroPoint = ConvertLogicalToPhysicalPoint(new Point());
        _zeroPoint.X = Math.Round(_zeroPoint.X);
      }
      else
      {
        double barSeriesCount = StackCount;
        _seriesIndex = StackIndex;

        double spacing = MinDelta == 0 ? XAxis.ActualMajorTickMarkSpacing : MinDelta * IndexStep;
        if (LogicalBarSize > 0)
        {
          spacing = LogicalBarSize;
        }
        _totalBarSize = (XAxis.ConvertLogicalToPhysicalSize(spacing) * BarSizeFactor);// (int)(XAxis.GetPhysicalLabelSpacing(chartWidth) * barSizeRatio);
        _barSize = Math.Ceiling(_totalBarSize / barSeriesCount);

        _previousSeries = GetPreviousSeries();
        _dataCache = new Dictionary<double, double>();

        _zeroPoint = ConvertLogicalToPhysicalPoint(new Point());
        _zeroPoint.Y = Math.Round(_zeroPoint.Y);
      }
    }

    internal override void PlotDataPoint(int index)
    {
      object o = ItemsSource[index];
      Point point;
      Bar bar = GetBarDataPoint(o, index, out point);
      PrepareDataPoint(bar, index);

      double previousData = 0;
      if (_previousSeries != null)
      {
        previousData = _previousSeries.GetData(point.X);
        point.Y += previousData;
      }
      _dataCache[point.X] = point.Y;

      SetTitle(bar, index); // This sets the title of the bar object.
      Point normalPoint = ConvertLogicalToPhysicalPoint(point);
      Point previousPoint = ConvertLogicalToPhysicalPoint(new Point(0, previousData));
      previousPoint.Y = Math.Round(previousPoint.Y);
      normalPoint.Y = Math.Round(normalPoint.Y);

      double barLength = Math.Abs(previousPoint.Y - normalPoint.Y) + (previousData == 0 ? 1 : 0);

      bar.IsNegative = point.Y < 0;
      if (BarStyle != null)
      {
        bar.Style = BarStyle;
      }
      bar.Width = _barSize;
      bar.Height = barLength;
      Canvas.SetLeft(bar, Math.Round(normalPoint.X - _totalBarSize / 2.0 + _seriesIndex * _barSize));
      double barY = Math.Min(normalPoint.Y, _zeroPoint.Y) - 1;

      Canvas.SetTop(bar, barY);

      if (Brushes.Count > 0)
      {
        bar.Background = Brushes[index % Brushes.Count];
      }
      else
      {
        bar.Background = SeriesBrush;
      }

      Canvas.Children.Add(bar);

      // Data Label:
      if (ShowDataLabels)
      {
        DataLabel label = new DataLabel(bar, bar.YObject);
        if (DataLabelStyle != null)
        {
          label.Style = DataLabelStyle;
        }
        label.Background = bar.Background;
        label.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
        Canvas.SetLeft(label, Math.Round(normalPoint.X - _totalBarSize / 2.0 + _seriesIndex * _barSize + _barSize / 2.0 - label.DesiredSize.Width / 2.0));
        double labelHeight = label.DesiredSize.Height;
        double labelTop = normalPoint.Y - 3 - labelHeight;
        if (point.Y < 0)
        {
          labelTop = normalPoint.Y + 3;
        }
        labelTop = Math.Max(labelTop, 3);
        labelTop = Math.Min(labelTop, Canvas.ActualHeight - 3 - labelHeight);
        Canvas.SetTop(label, Math.Round(labelTop));
        Canvas.SetZIndex(label, 200);
        Canvas.Children.Add(label);
      }

      index++;
    }

    internal override void PlotDataPoint_ReverseAxes(int index)
    {
      object o = ItemsSource[index];
      Point point;
      Bar bar = GetBarDataPoint(o, index, out point);
      PrepareDataPoint(bar, index);

      double previousData = 0;
      if (_previousSeries != null)
      {
        previousData = _previousSeries.GetData(point.Y);
        point.X += previousData;
      }
      _dataCache[point.Y] = point.X;

      SetTitle(bar, index); // This sets the title of the bar object.
      Point normalPoint = ConvertLogicalToPhysicalPoint(new Point(point.X, point.Y));
      Point previousPoint = ConvertLogicalToPhysicalPoint(new Point(previousData, 0));
      previousPoint.X = Math.Round(previousPoint.X);
      normalPoint.X = Math.Round(normalPoint.X);

      double barLength = Math.Abs(previousPoint.X - normalPoint.X) + (previousData == 0 ? 1 : 0);

      bar.IsNegative = point.X < 0;
      if (BarStyle != null)
      {
        bar.Style = BarStyle;
      }
      bar.Width = Math.Round(barLength);
      bar.Height = _barSize;
      double barX = previousPoint.X - (previousData == 0 ? 1 : 0);

      Canvas.SetLeft(bar, barX);
      Canvas.SetTop(bar, Math.Round(normalPoint.Y - _totalBarSize / 2.0 + _seriesIndex * _barSize));

      if (Brushes.Count > 0)
      {
        bar.Background = Brushes[index % Brushes.Count];
      }
      else
      {
        bar.Background = SeriesBrush;
      }

      Canvas.Children.Add(bar);

      // Data Label:
      if (ShowDataLabels)
      {
        DataLabel label = new DataLabel(bar, bar.XObject);
        if (DataLabelStyle != null)
        {
          label.Style = DataLabelStyle;
        }
        label.Background = bar.Background;
        label.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
        Canvas.SetTop(label, Math.Round(normalPoint.Y - _totalBarSize / 2.0 + _seriesIndex * _barSize) + _barSize / 2.0 - label.DesiredSize.Height / 2.0);
        double labelWidth = label.DesiredSize.Width;
        double labelLeft = normalPoint.X + 3;
        if (point.X < 0)
        {
          labelLeft = normalPoint.X - 3 - labelWidth;
        }
        labelLeft = Math.Max(labelLeft, 3);
        labelLeft = Math.Min(labelLeft, Canvas.ActualWidth - 3 - labelWidth);
        Canvas.SetLeft(label, Math.Round(labelLeft));
        Canvas.SetZIndex(label, 200);
        Canvas.Children.Add(label);
      }

      index++;
    }

    private Bar GetBarDataPoint(object o, int index, out Point point)
    {
      Bar bar = GetDataPoint(o, index) as Bar;
      if (bar == null)
      {
        bar = new Bar(o, Orientation);
        point = GetPoint(bar, index);
      }
      else
      {
        point = bar.LogicalPoint;
      }
      return bar;
    }

    private double GetData(double x)
    {
      // TODO: include the case where the previous series doesn't contain data at the given x value.
      double length;
      _dataCache.TryGetValue(x, out length);
      return length;
    }

    private int StackCount
    {
      get
      {
        IList<object> ids = new List<object>();
        int count = 0;
        foreach (DataSeries series in Series)
        {
          StackedBarSeries barSeries = series as StackedBarSeries;
          if (barSeries != null)
          {
            if (!ids.Contains(barSeries.StackIdentifier))
            {
              ids.Add(barSeries.StackIdentifier);
              count++;
            }
          }
        }
        return count;
      }
    }

    private int StackIndex
    {
      get
      {
        IList<object> ids = new List<object>();
        int index = 0;
        foreach (DataSeries series in Series)
        {
          StackedBarSeries barSeries = series as StackedBarSeries;
          if (barSeries != null)
          {
            if (barSeries.StackIdentifier == StackIdentifier || (barSeries.StackIdentifier != null && barSeries.StackIdentifier.Equals(StackIdentifier)))
            {
              break;
            }
            if (!ids.Contains(barSeries.StackIdentifier))
            {
              ids.Add(barSeries.StackIdentifier);
              index++;
            }
          }
        }
        return index;
      }
    }

    private StackedBarSeries GetPreviousSeries()
    {
      int index = Series.IndexOf(this);
      for (int i = index - 1; i >= 0; i--)
      {
        StackedBarSeries barSeries = Series[i] as StackedBarSeries;
        if (barSeries != null && barSeries.Visibility != Visibility.Collapsed)
        {
          if (barSeries.StackIdentifier == StackIdentifier || (barSeries.StackIdentifier != null && barSeries.StackIdentifier.Equals(StackIdentifier)))
          {
            return barSeries;
          }
        }
      }
      return null;
    }

    internal override void AnalyseSeries()
    {
      _dataCache = new Dictionary<double, double>();
      base.AnalyseSeries();
    }

    internal override void GetDataPointDimensions(object dataPoint, Point point, out double dependentLow, out double dependentHigh)
    {
      if (Series != null)
      {
        StackedBarSeries series = GetPreviousSeries();
        StackedSeriesUtils.GetDataPointDimensions(this, series == null ? null : series._dataCache, _dataCache, point, out dependentLow, out dependentHigh);
      }
      else
      {
        dependentLow = point.Y;
        dependentHigh = point.Y;
      }
    }
  }
}
