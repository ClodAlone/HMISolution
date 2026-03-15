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
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Windows.Data;
using System.Diagnostics;
using System.Collections.Specialized;

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// Plots a bar series on a <see cref="Chart"/> control.  A bar series is represented
  /// by a set of vertical bars.
  /// </summary>
  public class BarSeries : DataSeries
  {
    private readonly ObservableCollection<Brush> _brushes = new ObservableCollection<Brush>();

    private Point _zeroPoint;
    private double _logicalBarSize;
    private int _seriesIndex;
    private double _logicalBarWidth;
    private double _totalBarSize;
    private double _barSize;

    private double _chartHeight;
    private double _logicalBarHeight;

    static BarSeries()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(BarSeries),
        new FrameworkPropertyMetadata(typeof(BarSeries)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BarSeries"/> class.
    /// </summary>
    public BarSeries()
    {
      _brushes.CollectionChanged += new NotifyCollectionChangedEventHandler(Brushes_CollectionChanged);

      // TODO: this should probabaly override the metadata rather than setting property directly. (Is that possible?)
      DataSampler = new FixedSampleCountSampler() { MaxDataPointCount = 100 }; // TODO: this value should be based on the number of BarSeries in the Chart.
    }

    private void Brushes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      // TODO: a faster way would just be to iterate the rendered data points collection. Keep in mind the SeriesBrush property.
      RequestRebuild();
      OnLegendItemsChanged();
    }

    /// <summary>
    /// Gets the brushes used to fill the bars.  If there are more bars than brushes, the
    /// brushes are used in rotation.
    /// </summary>
    public ObservableCollection<Brush> Brushes
    {
      get { return _brushes; }
    }

    internal override bool ReverseAxes
    {
      get { return Orientation == Orientation.Horizontal; }
    }

    internal override bool AlwaysPlotLastDataPoint { get { return false; } }

    /// <summary>
    /// Gets whether or not this <see cref="BarSeries"/> supports data sampling.
    /// </summary>
    protected override bool SupportsDataSampling
    {
      get
      {
        return IsDataOrdered;
      }
    }

    #region Orientation property

    /// <summary>
    /// Gets or sets the <see cref="Orientation"/> of the bars. The default is Orientation.Vertical.
    /// This is a dependency property.
    /// </summary>
    public Orientation Orientation
    {
      get { return (Orientation)GetValue(OrientationProperty); }
      set { SetValue(OrientationProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Orientation"/> property.
    /// </summary>
    public static readonly DependencyProperty OrientationProperty =
      DependencyProperty.Register("Orientation", typeof(Orientation), typeof(BarSeries),
      new PropertyMetadata(Orientation.Vertical, new PropertyChangedCallback(OnOrientationChanged)));

    private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((BarSeries)d).OnOrientationChanged();
    }

    private void OnOrientationChanged()
    {
      UpdateAxes();
    }

    #endregion // Orientation property

    #region BarStyle property

    /// <summary>
    /// Gets or sets the <see cref="Style"/> applied to the bars.
    /// This is a dependency property.
    /// </summary>
    public Style BarStyle
    {
      get { return (Style)GetValue(BarStyleProperty); }
      set { SetValue(BarStyleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="BarStyle"/> property.
    /// </summary>
    public static readonly DependencyProperty BarStyleProperty =
      DependencyProperty.Register("BarStyle", typeof(Style), typeof(BarSeries),
      new PropertyMetadata(new PropertyChangedCallback(OnBarStyleChanged)));

    private static void OnBarStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((BarSeries)d).OnBarStyleChanged();
    }

    private void OnBarStyleChanged()
    {
      // TODO: a faster way would be to iterate the rendered data points and change the style. Keep in mind the Brushes and SeriesBrush properties.
      RequestRebuild();
      /*foreach (CartesianDataPoint dataPoint in DataPoints)
      {
        Bar bar = dataPoint as Bar;
        if (bar != null)
        {
          bar.Style = BarStyle;
        }
      }*/
    }

    #endregion // BarStyle property

    #region ShowDataLabels property

    /// <summary>
    /// Gets or sets the whether or not to display the data labels.
    /// This is a dependency property.
    /// </summary>
    public bool ShowDataLabels
    {
      get { return (bool)GetValue(ShowDataLabelsProperty); }
      set { SetValue(ShowDataLabelsProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ShowDataLabels"/> property.
    /// </summary>
    public static readonly DependencyProperty ShowDataLabelsProperty =
      DependencyProperty.Register("ShowDataLabels", typeof(bool), typeof(BarSeries),
      new PropertyMetadata(false, new PropertyChangedCallback(OnShowDataLabelsChanged)));

    private static void OnShowDataLabelsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((BarSeries)d).OnShowDataLabelsChanged();
    }

    private void OnShowDataLabelsChanged()
    {
      // TODO
    }

    #endregion // ShowDataLabels property

    #region DataLabelStyle property

    /// <summary>
    /// Gets or sets the <see cref="Style"/> applied to the data labels.
    /// This is a dependency property.
    /// </summary>
    public Style DataLabelStyle
    {
      get { return (Style)GetValue(DataLabelStyleProperty); }
      set { SetValue(DataLabelStyleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="DataLabelStyle"/> property.
    /// </summary>
    public static readonly DependencyProperty DataLabelStyleProperty =
      DependencyProperty.Register("DataLabelStyle", typeof(Style), typeof(BarSeries),
      new PropertyMetadata(new PropertyChangedCallback(OnDataLabelStyleChanged)));

    private static void OnDataLabelStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((BarSeries)d).OnDataLabelStyleChanged();
    }

    private void OnDataLabelStyleChanged()
    {
      //TODO: apply new style to all the data labels
    }

    #endregion // DataLabelStyle property

    #region ShowAllLegendItems property

    /// <summary>
    /// Gets or sets whether or not each item in the ItemsSource should have its own legend item.
    /// This is a dependency property.
    /// </summary>
    public bool ShowAllLegendItems
    {
      get { return (bool)GetValue(ShowAllLegendItemsProperty); }
      set { SetValue(ShowAllLegendItemsProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ShowAllLegendItems"/> property.
    /// </summary>
    public static readonly DependencyProperty ShowAllLegendItemsProperty =
      DependencyProperty.Register("ShowAllLegendItems", typeof(bool), typeof(BarSeries),
      new PropertyMetadata(false, new PropertyChangedCallback(OnShowAllLegendItemsChanged)));

    private static void OnShowAllLegendItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((BarSeries)d).OnShowAllLegendItemsChanged();
    }

    private void OnShowAllLegendItemsChanged()
    {
      //TODO: Update LegendItems property
    }

    #endregion // ShowAllLegendItems property

    #region BaselineStyle property

    /// <summary>
    /// Gets or sets the style applied to the baseline of the bar chart.
    /// This is a dependency property.
    /// </summary>
    public Style BaselineStyle
    {
      get { return (Style)GetValue(BaselineStyleProperty); }
      set { SetValue(BaselineStyleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="BaselineStyle"/> property.
    /// </summary>
    public static readonly DependencyProperty BaselineStyleProperty =
      DependencyProperty.Register("BaselineStyle", typeof(Style), typeof(BarSeries),
      new PropertyMetadata(null, new PropertyChangedCallback(OnBaselineStyleChanged)));

    // This style depends on the orientation which is why it is not static.
    internal Style BuildDefaultBaselineStyle()
    {
      Style style = new Style(typeof(Border));
      if (Orientation == Orientation.Vertical)
      {
        style.Setters.Add(new Setter(Border.HeightProperty, 1.0));
        style.Setters.Add(new Setter(Border.BackgroundProperty, new SolidColorBrush(Colors.Black)));
      }
      else
      {
        style.Setters.Add(new Setter(Border.WidthProperty, 1.0));
        style.Setters.Add(new Setter(Border.BackgroundProperty, new SolidColorBrush(Colors.Black)));
      }
      return style;
    }

    private static void OnBaselineStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((BarSeries)d).OnBaselineStyleChanged();
    }

    private void OnBaselineStyleChanged()
    {
      // TODO apply new zero line style.
    }

    #endregion // BaselineStyle property

    #region BarSizeFactor property

    /// <summary>
    /// Gets or sets the size factor used to calculate the thickness of the bars. This will typically be a value between 0 and 1.
    /// This value is the factor between a single axis unit and the thickness of a bar. A value of 1 will cause all the bars to
    /// be adjacent with no gaps between them (like a histogram). Smaller values put spaces between the bars. When plotting multiple
    /// bar series, the BarSizeFactor is used to calculate the total thickness of all the bars plotted on the same independent axis value.
    /// This is a dependency property. The default is 0.8.
    /// </summary>
    public double BarSizeFactor
    {
      get { return (double)GetValue(BarSizeFactorProperty); }
      set { SetValue(BarSizeFactorProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="BarSizeFactor"/> property.
    /// </summary>
    public static readonly DependencyProperty BarSizeFactorProperty =
      DependencyProperty.Register("BarSizeFactor", typeof(double), typeof(BarSeries),
      new PropertyMetadata(0.8, new PropertyChangedCallback(OnBarSizeFactorChanged)));

    private static void OnBarSizeFactorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((BarSeries)d).OnBarSizeFactorChanged();
    }

    private void OnBarSizeFactorChanged()
    {
      RequestRebuild();
    }

    #endregion // BarSizeFactor property

    #region LogicalBarSize Property

    /// <summary>
    /// Gets or sets the logical size available to bars in this bar series.
    /// When set to zero, the logical size will be automatically calculated based on the spread of the data.
    /// The default is zero.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="LogicalBarSizeProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public double LogicalBarSize
    {
      get { return (double)GetValue(LogicalBarSizeProperty); }
      set { SetValue(LogicalBarSizeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="LogicalBarSize"/> property.
    /// </summary>
    public static readonly DependencyProperty LogicalBarSizeProperty =
      DependencyProperty.Register("LogicalBarSize", typeof(double), typeof(BarSeries),
      new FrameworkPropertyMetadata(0.0, OnLogicalBarSizeChanged));

    private static void OnLogicalBarSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((BarSeries)d).OnLogicalBarSizeChanged();
    }

    private void OnLogicalBarSizeChanged()
    {
      RequestRebuild();
    }

    #endregion // LogicalBarSize Property

    #region BaseLine Property

    /// <summary>
    /// Gets or sets the baseline position of this <see cref="BarSeries"/>. This is the line that separates negative and positive values.
    /// The default is 0.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="BaselineProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public double Baseline
    {
      get { return (double)GetValue(BaselineProperty); }
      set { SetValue(BaselineProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Baseline"/> property.
    /// </summary>
    public static readonly DependencyProperty BaselineProperty =
      DependencyProperty.Register("Baseline", typeof(double), typeof(BarSeries),
      new FrameworkPropertyMetadata(0.0, OnBaselineChanged));

    private static void OnBaselineChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((BarSeries)d).OnBaselineChanged();
    }

    private void OnBaselineChanged()
    {
      RequestAnalyseAndRebuildAll();
    }

    #endregion // Baseline Property

    #region BarRenderingMode Property

    /// <summary>
    /// Gets or sets how to render the bars. The default is side-by-side.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="BarRenderingModeProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public BarRenderingMode BarRenderingMode
    {
      get { return (BarRenderingMode)GetValue(BarRenderingModeProperty); }
      set { SetValue(BarRenderingModeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="BarRenderingMode"/> property.
    /// </summary>
    public static readonly DependencyProperty BarRenderingModeProperty =
      DependencyProperty.Register("BarRenderingMode", typeof(BarRenderingMode), typeof(BarSeries),
      new FrameworkPropertyMetadata(BarRenderingMode.SideBySide, OnBarRenderingModeChanged));

    private static void OnBarRenderingModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((BarSeries)d).OnBarRenderingModeChanged();
    }

    private void OnBarRenderingModeChanged()
    {
      RequestRebuild();
    }

    #endregion // BarRenderingMode Property

    internal override void OnXAxisChanged()
    {
      base.OnXAxisChanged();

      if (XAxis != null && XAxis.IsLabelLayoutAuto && Orientation == Orientation.Vertical)
      {
        XAxis.SetLabelLayoutInternal(AxisLabelLayout.Inside);
      }
    }

    internal override void OnYAxisChanged()
    {
      base.OnYAxisChanged();

      if (YAxis != null && YAxis.IsLabelLayoutAuto && Orientation == Orientation.Horizontal)
      {
        YAxis.SetLabelLayoutInternal(AxisLabelLayout.Inside);
      }
    }

    private void UpdateAxes()
    {
      if (XAxis != null && XAxis.IsLabelLayoutAuto && Orientation == Orientation.Vertical)
      {
        XAxis.SetLabelLayoutInternal(AxisLabelLayout.Inside);
        if (YAxis != null)
        {
          YAxis.SetLabelLayoutInternal(AxisLabelLayout.Normal);
        }
      }
      if (YAxis != null && YAxis.IsLabelLayoutAuto && Orientation == Orientation.Horizontal)
      {
        YAxis.SetLabelLayoutInternal(AxisLabelLayout.Inside);
        if (XAxis != null)
        {
          XAxis.SetLabelLayoutInternal(AxisLabelLayout.Normal);
        }
      }
    }

    private Binding _titleBinding;

    /// <summary>
    /// Gets or sets the data binding used to extract the bar title from each data point.
    /// </summary>
    public Binding TitleBinding
    {
      get { return _titleBinding; }
      set
      {
        _titleBinding = value;
        RequestRebuild();
        OnLegendItemsChanged();
      }
    }

    /// <summary>
    /// Gets a list of legend items to be displayed in a legend for this <see cref="BarSeries"/>.
    /// </summary>
    public override IList<LegendItem> LegendItems
    {
      get
      {
        IList<LegendItem> items = new List<LegendItem>();
        if (IsShownInLegend)
        {
          if (ItemsSource != null && ShowAllLegendItems)
          {
            int index = 0;
            int count = 0;
            foreach (object o in ItemsSource)
            {
              Brush brush = SeriesBrush; // new SolidColorBrush(new Color() { A = 255, R = 40, G = 97, B = 169 });
              if (Brushes != null && Brushes.Count > 0)
              {
                brush = Brushes[index];
              }
              items.Add(new LegendItem(GetLegendName(new Bar(o, Orientation), count), LegendIconTemplate, brush, this));
              if (Brushes.Count > 0)
              {
                index = ++index % Brushes.Count;
              }
              count++;
            }
          }
          else
          {
            Brush brush = SeriesBrush;
            if (Brushes != null && Brushes.Count > 0)
            {
              brush = Brushes[0];
            }
            items.Add(new LegendItem(Title, LegendIconTemplate, brush, this));
          }
        }
        return items;
      }
    }

    internal override void OnTitleChanged()
    {
      OnLegendItemsChanged();
    }

    private string GetLegendName(Bar bar, int index)
    {
      string title = Title;
      if (TitleBinding != null)
      {
        bar.SetBinding(Bar.TitleProperty, TitleBinding);
        title = bar.Title;
      }
      else if (ShowAllLegendItems)
      {
        bar.Title = "Item " + (index + 1);
        title = bar.Title;
      }
      return title;
    }

    internal override void OnLegendIconTemplateChanged()
    {
      OnLegendItemsChanged();
    }

    /// <summary>
    /// Sets the title of the given <see cref="Bar"/> based on the TitleBinding property or
    /// the bar index if there is no TitleBinding.
    /// </summary>
    /// <param name="bar">The <see cref="Bar"/> to find a title for.</param>
    /// <param name="index">The index of the <see cref="Bar"/> within the items source. This is used
    /// to create a default title if the Bar has no TitleBinding.</param>
    protected void SetTitle(Bar bar, int index)
    {
      if (TitleBinding != null)
      {
        bar.SetBinding(Bar.TitleProperty, TitleBinding);
      }
      else
      {
        bar.Title = "Item " + (index + 1);
      }
    }

    internal Bar GetBar(object o, int index, out Point point)
    {
      Bar bar = GetDataPoint(o, index) as Bar;
      if (o == null)
      {
        point = new Point();
      }
      else if (bar == null)
      {
        bar = new Bar(o, Orientation);
        point = GetPoint(bar, index);
      }
      else
      {
         point = bar.LogicalPoint;
      }
      if (bar != null && (bar.YObject == null || bar.XObject == null))
      {
        bar = null;
      }
      if (Double.IsNaN(point.X) || Double.IsNaN(point.Y))
      {
        bar = null;
      }
      return bar;
    }

    internal override void PrepareToPlotData()
    {
      // TODO: refactor
      if (ReverseAxes)
      {
        double barSeriesCount = BarSeriesCount;
        _seriesIndex = SeriesIndex;

        _chartHeight = Math.Floor(Canvas.ActualHeight) - 1; // TODO: it would be great to remove the negative one from various places.

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
        double barSeriesCount = BarSeriesCount;
        _seriesIndex = SeriesIndex;

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
        double barLength = Math.Abs(_zeroPoint.Y - normalPoint.Y) + 1;
        bar.IsNegative = point.Y < Baseline;
        Canvas.Children.Add(bar);

        //Canvas.SetLeft(bar, Math.Round(normalPoint.X - totalBarSize / 2.0 + seriesIndex * barSize));
        double left = Math.Round(XAxis.ConvertLogicalToPhysical(point.X - (_logicalBarSize / 2.0) + (_seriesIndex * _logicalBarWidth)));
        //bar.Width = Math.Max(0, Math.Round(barSize));
        double right = Math.Round(XAxis.ConvertLogicalToPhysical(point.X - (_logicalBarSize / 2.0) + (_seriesIndex * _logicalBarWidth) + _logicalBarWidth));
        //Debug.WriteLine("Left: " + left + ", Right: " + right);
        if (left > right)
        {
          double temp = left;
          left = right;
          right = temp;
        }
        double barY = Math.Min(normalPoint.Y, _zeroPoint.Y) - 1;
        Canvas.SetTop(bar, barY);
        Canvas.SetLeft(bar, left);
        bar.Width = Math.Max(0, right - Canvas.GetLeft(bar) + 1);
        bar.Height = Math.Round(barLength, 0);

        if (BarStyle != null)
        {
          bar.Style = BarStyle;
        }

        if (_brushes.Count > 0)
        {
          bar.Background = _brushes[index % _brushes.Count];
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
          if (point.Y < Baseline)
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

    internal override void FinishPlottingData()
    {
      // TODO: this stuff can probably be moved to PrepareToPlotData
      if (ReverseAxes)
      {
        if (XAxis != null && Baseline > XAxis.ActualMinimumValue && Baseline < XAxis.ActualMaximumValue)
        {
          Border baseline = new Border();
          baseline.Style = BaselineStyle == null ? BuildDefaultBaselineStyle() : BaselineStyle;
          baseline.Height = Canvas.ActualHeight;
          Canvas.SetLeft(baseline, Math.Round(_zeroPoint.X));
          Canvas.Children.Add(baseline);
        }
      }
      else
      {
        if (YAxis != null && Baseline > YAxis.ActualMinimumValue && Baseline < YAxis.ActualMaximumValue)
        {
          Border baseline = new Border();
          baseline.Style = BaselineStyle == null ? BuildDefaultBaselineStyle() : BaselineStyle;
          baseline.Width = Canvas.ActualWidth;
          Canvas.SetTop(baseline, Math.Round(_zeroPoint.Y - baseline.Height));
          Canvas.Children.Add(baseline);
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

        double barLength = Math.Abs(_zeroPoint.X - normalPoint.X) + 1;
        bar.IsNegative = point.X < Baseline;
        if (BarStyle != null)
        {
          bar.Style = BarStyle;
        }
        bar.Width = Math.Round(barLength);
        double barX = Math.Min(normalPoint.X, _zeroPoint.X);
        Canvas.SetLeft(bar, barX);
        double top = _chartHeight - Math.Round(YAxis.ConvertLogicalToPhysical(point.Y + (_logicalBarSize / 2.0) - (_seriesIndex * _logicalBarHeight)));
        //Canvas.SetTop(bar, Math.Round(normalPoint.Y - totalBarSize / 2.0 + seriesIndex * barSize));
        double bottom = _chartHeight - Math.Round(YAxis.ConvertLogicalToPhysical(point.Y + (_logicalBarSize / 2.0) - (_seriesIndex * _logicalBarHeight) - _logicalBarHeight));
        if (bottom < top)
        {
          double temp = top;
          top = bottom;
          bottom = temp;
        }
        Canvas.SetTop(bar, top);
        bar.Height = Math.Max(0, bottom - top + 1);

        if (_brushes.Count > 0)
        {
          bar.Background = _brushes[index % _brushes.Count];
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
          if (point.X < Baseline)
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

    private int BarSeriesCount
    {
      get
      {
        int count = 0;
        bool countedOverlap = false;
        foreach (DataSeries series in Series)
        {
          BarSeries barSeries = series as BarSeries;
          if (barSeries != null && series.Visibility != Visibility.Collapsed && series.XAxis == XAxis)
          {
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
    }

    private int SeriesIndex
    {
      get
      {
        int index = Series.IndexOf(this);
        int overlapIndex = -1;
        int indexCount = 0;
        for (int i = 0; i <= index; i++)
        {
          BarSeries barSeries = Series[i] as BarSeries;
          if (barSeries != null && Series[i].Visibility != Visibility.Collapsed && barSeries.XAxis == XAxis)
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
}
