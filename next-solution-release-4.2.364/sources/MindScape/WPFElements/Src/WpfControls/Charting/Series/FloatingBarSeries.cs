using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// Plots a floating bar series on a <see cref="Chart"/> control.  A floating bar series is represented
  /// by a set of vertical or horizontal bars each having its own baseline.
  /// </summary>
  public class FloatingBarSeries : BarSeries
  {
    // TODO: some of the stuff in the class could probably stay in the BarSeries class.

    private Point _zeroPoint;
    private double _logicalBarSize;
    private int _seriesIndex;
    private double _logicalBarWidth;
    private double _totalBarSize;
    private double _barSize;

    private double _chartHeight;
    private double _logicalBarHeight;

    static FloatingBarSeries()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(FloatingBarSeries),
        new FrameworkPropertyMetadata(typeof(FloatingBarSeries)));
    }

    internal override void GetDataPointDimensions(object dataPoint, Point point, out double dependentLow, out double dependentHigh)
    {
      Bar bar = new Bar(dataPoint, Orientation); // TODO: this is a little bit slow.
      double baseline = GetBaseline(bar);
      dependentLow = Math.Min(point.Y, baseline);
      dependentHigh = Math.Max(point.Y, baseline);
    }

    internal override void PrepareToPlotData()
    {
      // TODO: refactor
      if (ReverseAxes)
      {
        double barSeriesCount = GetBarSeriesCount();
        _seriesIndex = GetSeriesIndex();

        _chartHeight = Canvas.ActualHeight - 1;

        double spacing = MinDelta == 0 ? YAxis.ActualMajorTickMarkSpacing : MinDelta * IndexStep;
        if (LogicalBarSize > 0)
        {
          spacing = LogicalBarSize;
        }
        _logicalBarSize = spacing * BarSizeFactor;
        _logicalBarHeight = _logicalBarSize / barSeriesCount;

        _totalBarSize = (YAxis.ConvertLogicalToPhysicalSize(spacing) * BarSizeFactor);
        _barSize = Math.Ceiling(_totalBarSize / barSeriesCount);

        _zeroPoint = ConvertLogicalToPhysicalPoint(new Point(Baseline, 0));
        _zeroPoint.X = Math.Round(_zeroPoint.X);
      }
      else
      {
        double barSeriesCount = GetBarSeriesCount();
        _seriesIndex = GetSeriesIndex();

        double spacing = MinDelta == 0 ? XAxis.ActualMajorTickMarkSpacing : MinDelta * IndexStep;
        if (LogicalBarSize > 0)
        {
          spacing = LogicalBarSize;
        }
        _logicalBarSize = spacing * BarSizeFactor;
        _logicalBarWidth = _logicalBarSize / barSeriesCount;

        _totalBarSize = XAxis.ConvertLogicalToPhysicalSize(spacing) * BarSizeFactor;
        _barSize = _totalBarSize / barSeriesCount;

        _zeroPoint = ConvertLogicalToPhysicalPoint(new Point(0, Baseline));
        _zeroPoint.Y = Math.Round(_zeroPoint.Y);
      }
    }

    internal override void PlotDataPoint(int index)
    {
      Object o = ItemsSource[index];
      Point point;
      Bar bar = GetBar(o, index, out point);
      if (bar != null)
      {
        //Point point = GetPoint(bar, count);
        SetTitle(bar, index); // To set the title of the bar object.
        PrepareDataPoint(bar, index);
        Point normalPoint = ConvertLogicalToPhysicalPoint(point);
        normalPoint.Y = Math.Round(normalPoint.Y);

        double baseline = GetBaseline(bar);
        _zeroPoint = new Point(0, baseline);
        _zeroPoint = ConvertLogicalToPhysicalPoint(_zeroPoint);
        _zeroPoint.Y = Math.Round(_zeroPoint.Y);

        double barLength = Math.Abs(_zeroPoint.Y - normalPoint.Y) + 1;
        bar.IsNegative = point.Y < Baseline;
        Canvas.Children.Add(bar);

        //Canvas.SetLeft(bar, Math.Round(normalPoint.X - totalBarSize / 2.0 + seriesIndex * barSize));
        double left = Math.Round(XAxis.ConvertLogicalToPhysical(point.X - (_logicalBarSize / 2.0) + (_seriesIndex * _logicalBarWidth)));
        Canvas.SetLeft(bar, left);
        double barY = Math.Min(normalPoint.Y, _zeroPoint.Y) - 1;
        Canvas.SetTop(bar, barY);

        //bar.Width = Math.Max(0, Math.Round(barSize));
        double right = Math.Round(XAxis.ConvertLogicalToPhysical(point.X - (_logicalBarSize / 2.0) + (_seriesIndex * _logicalBarWidth) + _logicalBarWidth));
        //Debug.WriteLine("Left: " + left + ", Right: " + right);
        bar.Width = Math.Max(0, right - Canvas.GetLeft(bar) + 1);
        bar.Height = Math.Round(barLength, 0);

        if (BarStyle != null)
        {
          bar.Style = BarStyle;
        }

        if (Brushes.Count > 0)
        {
          bar.Background = Brushes[index % Brushes.Count];
        }
        else
        {
          bar.Background = SeriesBrush;
        }

        // Data Label:
        if (ShowDataLabels)
        {
          DataLabel label = new DataLabel(bar, point.Y);
          if (DataLabelStyle != null)
          {
            label.Style = DataLabelStyle;
          }
          label.Background = bar.Background;
          label.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
          Canvas.SetLeft(label, Math.Round(normalPoint.X - _totalBarSize / 2.0 + _seriesIndex * _barSize + _barSize / 2.0 - label.DesiredSize.Width / 2.0));
          double labelHeight = label.DesiredSize.Height;
          double labelTop = normalPoint.Y - 3 - labelHeight;
          if (point.Y < baseline)
          {
            labelTop = normalPoint.Y + 3;
          }
          labelTop = Math.Max(labelTop, 3);
          labelTop = Math.Min(labelTop, Canvas.ActualHeight - 3 - labelHeight);
          Canvas.SetTop(label, Math.Round(labelTop));
          Canvas.SetZIndex(label, 200);
          Canvas.Children.Add(label);
        }
      }
    }

    internal override void PlotDataPoint_ReverseAxes(int index)
    {
      Object o = ItemsSource[index];
      Point point;
      Bar bar = GetBar(o, index, out point);
      if (bar != null)
      {
        //Point point = GetPoint(bar, count, true);
        SetTitle(bar, index); // To set the title of the bar object.
        PrepareDataPoint(bar, index);
        Point normalPoint = ConvertLogicalToPhysicalPoint(new Point(point.X, point.Y));
        normalPoint.X = Math.Round(normalPoint.X);

        double baseline = GetBaseline(bar);
        _zeroPoint = new Point(baseline, 0);
        _zeroPoint = ConvertLogicalToPhysicalPoint(_zeroPoint);
        _zeroPoint.X = Math.Round(_zeroPoint.X);

        double barLength = Math.Abs(_zeroPoint.X - normalPoint.X) + 1;
        bar.IsNegative = point.X < Baseline;
        if (BarStyle != null)
        {
          bar.Style = BarStyle;
        }
        bar.Width = Math.Round(barLength);
        double barX = Math.Min(normalPoint.X, _zeroPoint.X) - 1;
        Canvas.SetLeft(bar, barX);
        double top = _chartHeight - Math.Round(YAxis.ConvertLogicalToPhysical(point.Y + (_logicalBarSize / 2.0) - (_seriesIndex * _logicalBarHeight)));
        //Canvas.SetTop(bar, Math.Round(normalPoint.Y - totalBarSize / 2.0 + seriesIndex * barSize));
        Canvas.SetTop(bar, top);

        double bottom = _chartHeight - Math.Round(YAxis.ConvertLogicalToPhysical(point.Y + (_logicalBarSize / 2.0) - (_seriesIndex * _logicalBarHeight) - _logicalBarHeight));
        bar.Height = Math.Max(0, bottom - top + 1);

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
          DataLabel label = new DataLabel(bar, point.X);
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
      }
    }

    internal override void FinishPlottingData()
    {
      // TODO: this stuff can probably be moved to PrepareToPlotData
      if (ReverseAxes)
      {
        _zeroPoint = ConvertLogicalToPhysicalPoint(new Point(Baseline, 0));
        _zeroPoint.X = Math.Round(_zeroPoint.X);
        Border baseline = new Border();
        baseline.Style = BaselineStyle == null ? BuildDefaultBaselineStyle() : BaselineStyle;
        baseline.Height = Canvas.ActualHeight;
        Canvas.SetLeft(baseline, Math.Round(_zeroPoint.X - baseline.Width, 0));
        Canvas.Children.Add(baseline);
      }
      else
      {
        _zeroPoint = ConvertLogicalToPhysicalPoint(new Point(0, Baseline));
        _zeroPoint.Y = Math.Round(_zeroPoint.Y);
        Border baseline = new Border();
        baseline.Style = BaselineStyle == null ? BuildDefaultBaselineStyle() : BaselineStyle;
        baseline.Width = Canvas.ActualWidth;
        Canvas.SetTop(baseline, Math.Round(_zeroPoint.Y - baseline.Height));
        Canvas.Children.Add(baseline);
      }
    }

    // TODO: re analyse and render the series if the BaselineBinding changes.
    /// <summary>
    /// Gets or sets the binding used to extract the baseline value from a data point model.
    /// </summary>
    public Binding BaselineBinding { get; set; }

    // TODO: at the moment the baseline value must be a double. This should be updated to support any object which the axis would then convert.
    private double GetBaseline(Bar bar)
    {
      ValueExtractor extractor = new ValueExtractor();
      extractor.Value = 0.0;
      if (BaselineBinding != null)
      {
        extractor.DataContext = bar.DataContext;
        extractor.SetBinding(ValueExtractor.ValueProperty, BaselineBinding);
      }
      else if (bar.DataContext is Point3)
      {
        Point3 point3 = bar.DataContext as Point3;
        extractor.Value = point3.Z;
      }
      else if (bar.DataContext is StringDoubleDouble)
      {
        StringDoubleDouble sdd = bar.DataContext as StringDoubleDouble;
        extractor.Value = sdd.Size;
      }
      if (ReverseAxes && XAxis != null && XAxis.ValueConverter != null)
      {
        extractor.Value = XAxis.ValueConverter.GetAxisPlotPosition(extractor.Value);
      }
      else if (YAxis != null && YAxis.ValueConverter != null)
      {
        extractor.Value = YAxis.ValueConverter.GetAxisPlotPosition(extractor.Value);
      }
      return NumericalUtils.ConvertToDouble(extractor.Value) ?? 0;
    }

    private int GetBarSeriesCount()
    {
      int count = 0;
      bool countedOverlap = false;
      foreach (DataSeries series in Series)
      {
        if (series is FloatingBarSeries && series.Visibility != Visibility.Collapsed)
        {
          FloatingBarSeries barSeries = series as FloatingBarSeries;
          if (barSeries.BarRenderingMode == BarRenderingMode.Overlap)
          {
            if (!countedOverlap)
            {
              countedOverlap = true;
              count++;
            }
          }
          else
          {
            count++;
          }
        }
      }
      return count;
    }

    private int GetSeriesIndex()
    {
      /*int index = Series.IndexOf(this);
      int indexCount = 0;
      for (int i = 0; i < index; i++)
      {
        if (Series[i] is BarSeries && Series[i].Visibility != Visibility.Collapsed)
        {
          indexCount++;
        }
      }
      return indexCount;*/
      int overlapIndex = -1;
      int indexCount = 0;
      foreach (DataSeries series in Series)
      {
        FloatingBarSeries barSeries = series as FloatingBarSeries;
        if (barSeries != null && barSeries.Visibility != Visibility.Collapsed)
        {
          if (barSeries.BarRenderingMode == BarRenderingMode.Overlap)
          {
            if (overlapIndex == -1)
            {
              overlapIndex = indexCount;
              indexCount++;
            }
            if (barSeries == this)
            {
              return overlapIndex;
            }
          }
          else
          {
            if (barSeries == this)
            {
              return indexCount;
            }
            indexCount++;
          }
        }
      }
      return indexCount;
    }
  }
}
