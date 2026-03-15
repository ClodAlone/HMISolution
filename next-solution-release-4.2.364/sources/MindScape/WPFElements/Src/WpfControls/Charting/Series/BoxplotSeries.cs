using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// Renders data as a box and whisker diagram on the charting canvas.
  /// </summary>
  public class BoxplotSeries : DataSeries
  {
    static BoxplotSeries()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(BoxplotSeries),
        new FrameworkPropertyMetadata(typeof(BoxplotSeries)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BoxplotSeries"/> class.
    /// </summary>
    public BoxplotSeries()
    {
      // TODO: this should probabaly override the metadata rather than setting property directly. (Is that possible?)
      DataSampler = new FixedSampleCountSampler() { MaxDataPointCount = 100 };
    }

    internal override void GetDataPointDimensions(object dataPoint, Point point, out double dependentLow, out double dependentHigh)
    {
      BoxAndWhiskers box = new BoxAndWhiskers(dataPoint, Orientation); // TODO: this is a little bit slow.
      dependentLow = GetMinimum(box);
      dependentHigh = GetMaximum(box);
    }

    /// <summary>
    /// Gets whether or not this <see cref="BoxplotSeries"/> supports data sampling.
    /// </summary>
    protected override bool SupportsDataSampling
    {
      get
      {
        return IsDataOrdered;
      }
    }

    internal override void PlotDataPoint(int index)
    {
      object o = ItemsSource[index];
      Point point;
      BoxAndWhiskers box = GetBoxAndWhiskers(o, index, out point);
      PrepareDataPoint(box, index);

      double spacing = MinDelta == 0 ? XAxis.ActualMajorTickMarkSpacing : MinDelta;
      double boxWidth = XAxis.ConvertLogicalToPhysicalSize(spacing) * 0.6;

      double logicalMinimum = GetMinimum(box);
      double logicalMaximum = GetMaximum(box);
      double logicalLowerQuartile = GetLowerQuartile(box);
      double logicalUpperQuartile = GetUpperQuartile(box);
      double logicalMedian = GetMedian(box);

      double minimum = YAxis.ConvertLogicalToPhysical(logicalMinimum);
      double maximum = YAxis.ConvertLogicalToPhysical(logicalMaximum);
      double lowerQuartile = YAxis.ConvertLogicalToPhysical(logicalLowerQuartile);
      double upperQuartile = YAxis.ConvertLogicalToPhysical(logicalUpperQuartile);
      double median = YAxis.ConvertLogicalToPhysical(logicalMedian);

      point.Y = logicalMinimum;
      box.LogicalPoint = point;
      
      if (MinimumBinding != null)
      {
        //box.SetBinding(CartesianDataPoint.YObjectProperty, MinimumBinding);
      }
      box.LowerQuartilePosition = maximum - lowerQuartile;
      box.UpperQuartilePosition = maximum - upperQuartile;
      box.MedianPosition = maximum - median;
      box.BoxWidth = boxWidth;
      Point normalPoint = ConvertLogicalToPhysicalPoint(point);

      double size = Math.Max(0, maximum - minimum);

      box.MinimumPosition = size;

      if (BoxAndWhiskerStyle != null)
      {
        box.Style = BoxAndWhiskerStyle;
      }
      box.Background = SeriesBrush;

      Canvas.SetLeft(box, normalPoint.X - (boxWidth / 2.0));
      Canvas.SetTop(box, normalPoint.Y - size);
      Canvas.Children.Add(box);
    }

    internal override void PlotDataPoint_ReverseAxes(int index)
    {
      object o = ItemsSource[index];
      Point point;
      BoxAndWhiskers box = GetBoxAndWhiskers(o, index, out point);
      PrepareDataPoint(box, index);

      double spacing = MinDelta == 0 ? YAxis.ActualMajorTickMarkSpacing : MinDelta;
      double boxWidth = YAxis.ConvertLogicalToPhysicalSize(spacing) * 0.6;

      double logicalMinimum = GetMinimum(box);
      double logicalMaximum = GetMaximum(box);
      double logicalLowerQuartile = GetLowerQuartile(box);
      double logicalUpperQuartile = GetUpperQuartile(box);
      double logicalMedian = GetMedian(box);

      double minimum = XAxis.ConvertLogicalToPhysical(logicalMinimum);
      double maximum = XAxis.ConvertLogicalToPhysical(logicalMaximum);
      double lowerQuartile = XAxis.ConvertLogicalToPhysical(logicalLowerQuartile);
      double upperQuartile = XAxis.ConvertLogicalToPhysical(logicalUpperQuartile);
      double median = XAxis.ConvertLogicalToPhysical(logicalMedian);

      point.X = logicalMinimum;
      box.LogicalPoint = point;

      box.LowerQuartilePosition = maximum - lowerQuartile;
      box.UpperQuartilePosition = maximum - upperQuartile;
      box.MedianPosition = maximum - median;
      box.BoxWidth = boxWidth;
      Point normalPoint = ConvertLogicalToPhysicalPoint(point);

      double size = Math.Max(0, maximum - minimum);

      box.MinimumPosition = size;

      if (BoxAndWhiskerStyle != null)
      {
        box.Style = BoxAndWhiskerStyle;
      }
      box.Background = SeriesBrush;

      Canvas.SetTop(box, normalPoint.Y - (boxWidth / 2.0));
      Canvas.SetLeft(box, normalPoint.X);
      Canvas.Children.Add(box);
    }

    internal BoxAndWhiskers GetBoxAndWhiskers(object o, int index, out Point point)
    {
      BoxAndWhiskers stick = GetDataPoint(o, index) as BoxAndWhiskers;
      if (stick == null)
      {
        stick = new BoxAndWhiskers(o, Orientation);
        point = GetPoint(stick, index);
      }
      else
      {
        point = stick.LogicalPoint;
      }
      return stick;
    }

    internal override void OnXAxisChanged()
    {
      base.OnXAxisChanged();

      if (XAxis != null && XAxis.IsAuto)
      {
        XAxis.SetLabelLayoutInternal(AxisLabelLayout.Inside);
        XAxis.TickLayout = AxisTickLayout.Inside;
      }
    }

    internal override void OnYAxisChanged()
    {
      base.OnYAxisChanged();

      if (YAxis != null && YAxis.IsAuto)
      {
        YAxis.SetLabelLayoutInternal(AxisLabelLayout.Inside);
        YAxis.TickLayout = AxisTickLayout.Inside;
      }
    }

    private void UpdateAxes()
    {
      if (XAxis != null && Orientation == Orientation.Vertical)
      {
        XAxis.SetLabelLayoutInternal(AxisLabelLayout.Inside);
        XAxis.TickLayout = AxisTickLayout.Inside;
        if (YAxis != null)
        {
          YAxis.SetLabelLayoutInternal(AxisLabelLayout.Normal);
          YAxis.TickLayout = AxisTickLayout.Normal;
        }
      }
      else if (YAxis != null)
      {
        YAxis.SetLabelLayoutInternal(AxisLabelLayout.Inside);
        YAxis.TickLayout = AxisTickLayout.Inside;
        if (XAxis != null)
        {
          XAxis.SetLabelLayoutInternal(AxisLabelLayout.Normal);
          XAxis.TickLayout = AxisTickLayout.Normal;
        }
      }
    }

    #region BoxAndWhiskerStyle Property

    /// <summary>
    /// Gets or sets the BoxAndWhiskerStyle.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="BoxAndWhiskerStyleProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public Style BoxAndWhiskerStyle
    {
      get { return (Style)GetValue(BoxAndWhiskerStyleProperty); }
      set { SetValue(BoxAndWhiskerStyleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="BoxAndWhiskerStyle"/> property.
    /// </summary>
    public static readonly DependencyProperty BoxAndWhiskerStyleProperty =
      DependencyProperty.Register("BoxAndWhiskerStyle", typeof(Style), typeof(BoxplotSeries),
      new FrameworkPropertyMetadata(OnBoxAndWhiskerStyleChanged));

    private static void OnBoxAndWhiskerStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((BoxplotSeries)d).OnBoxAndWhiskerStyleChanged();
    }

    private void OnBoxAndWhiskerStyleChanged()
    {
      // TODO: only need to apply the new style to the data points rather than building the whole chart again. Keep in mind the SeriesBrush property.
      RequestRebuild();
    }

    #endregion // BoxAndWhiskerStyle Property

    #region Orientation Property

    /// <summary>
    /// Gets or sets the Orientation.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="OrientationProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public Orientation Orientation
    {
      get { return (Orientation)GetValue(OrientationProperty); }
      set { SetValue(OrientationProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Orientation"/> property.
    /// </summary>
    public static readonly DependencyProperty OrientationProperty =
      DependencyProperty.Register("Orientation", typeof(Orientation), typeof(BoxplotSeries),
      new FrameworkPropertyMetadata(Orientation.Vertical, OnOrientationChanged));

    private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((BoxplotSeries)d).OnOrientationChanged();
    }

    private void OnOrientationChanged()
    {
      UpdateAxes();
    }

    #endregion // Orientation Property

    internal override bool ReverseAxes
    {
      get
      {
        return Orientation == Orientation.Horizontal;
      }
    }

    /// <summary>
    /// Gets or set the binding used to extract the minimum value from each data point.
    /// </summary>
    public Binding MinimumBinding { get; set; }

    /// <summary>
    /// Gets or set the binding used to extract the maximum value from each data point.
    /// </summary>
    public Binding MaximumBinding { get; set; }

    /// <summary>
    /// Gets or set the binding used to extract the lower quartile value from each data point.
    /// </summary>
    public Binding LowerQuartileBinding { get; set; }

    /// <summary>
    /// Gets or set the binding used to extract the upper quartile value from each data point.
    /// </summary>
    public Binding UpperQuartileBinding { get; set; }

    /// <summary>
    /// Gets or set the binding used to extract the meadian value from each data point.
    /// </summary>
    public Binding MedianBinding { get; set; }

    // TODO: use the extractor technique to improve the performance of using the bindings.
    //       Remember that when rendering, a binding should be set on each data point to listen for property changes. But when analysing the series we just need the property value.

    internal double GetMinimum(BoxAndWhiskers box)
    {
      if (MinimumBinding != null)
      {
        box.SetBinding(BoxAndWhiskers.MinimumProperty, MinimumBinding);
      }
      else if (box.DataContext is BoxplotDataPoint)
      {
        BoxplotDataPoint boxplotDataPoint = box.DataContext as BoxplotDataPoint;
        return boxplotDataPoint.Minimum;
      }
      return box.Minimum;
    }

    internal double GetMaximum(BoxAndWhiskers box)
    {
      if (MaximumBinding != null)
      {
        box.SetBinding(BoxAndWhiskers.MaximumProperty, MaximumBinding);
      }
      else if (box.DataContext is BoxplotDataPoint)
      {
        BoxplotDataPoint boxplotDataPoint = box.DataContext as BoxplotDataPoint;
        return boxplotDataPoint.Maximum;
      }
      return box.Maximum;
    }

    internal double GetLowerQuartile(BoxAndWhiskers box)
    {
      if (LowerQuartileBinding != null)
      {
        box.SetBinding(BoxAndWhiskers.LowerQuartileProperty, LowerQuartileBinding);
      }
      else if (box.DataContext is BoxplotDataPoint)
      {
        BoxplotDataPoint boxplotDataPoint = box.DataContext as BoxplotDataPoint;
        return boxplotDataPoint.LowerQuartile;
      }
      return box.LowerQuartile;
    }

    internal double GetUpperQuartile(BoxAndWhiskers box)
    {
      if (UpperQuartileBinding != null)
      {
        box.SetBinding(BoxAndWhiskers.UpperQuartileProperty, UpperQuartileBinding);
      }
      else if (box.DataContext is BoxplotDataPoint)
      {
        BoxplotDataPoint boxplotDataPoint = box.DataContext as BoxplotDataPoint;
        return boxplotDataPoint.UpperQuartile;
      }
      return box.UpperQuartile;
    }

    internal double GetMedian(BoxAndWhiskers box)
    {
      if (MedianBinding != null)
      {
        box.SetBinding(BoxAndWhiskers.MedianProperty, MedianBinding);
      }
      else if (box.DataContext is BoxplotDataPoint)
      {
        BoxplotDataPoint boxplotDataPoint = box.DataContext as BoxplotDataPoint;
        return boxplotDataPoint.Median;
      }
      return box.Median;
    }
  }
}
