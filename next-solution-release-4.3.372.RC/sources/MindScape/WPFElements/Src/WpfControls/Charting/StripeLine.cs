using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// Renders an individual stripe line on a chart. Stripe lines can have optional outlines and a label.
  /// </summary>
  public class StripeLine : Canvas
  {
    private ContentControl _contentHost;

    /// <summary>
    /// Initializes anew instance of the <see cref="StripeLine"/> clas..
    /// </summary>
    public StripeLine()
    {
      Loaded += new RoutedEventHandler(StripeLine_Loaded);
      SizeChanged += new SizeChangedEventHandler(StripeLine_SizeChanged);
    }

    private void StripeLine_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      BuildStripe();
    }

    private void StripeLine_Loaded(object sender, RoutedEventArgs e)
    {
      Chart chart = VisualTreeUtils.FindAncestor<Chart>(this);
      if (chart != null)
      {
        if (XAxis == null)
        {
          XAxis = chart.XAxis;
        }
        if (YAxis == null)
        {
          YAxis = chart.YAxis;
        }
      }
    }

    #region StartValue Property

    /// <summary>
    /// Gets or sets the axis plot position of the start of the stripe line.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="StartValueProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public double StartValue
    {
      get { return (double)GetValue(StartValueProperty); }
      set { SetValue(StartValueProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="StartValue"/> property.
    /// </summary>
    public static readonly DependencyProperty StartValueProperty =
      DependencyProperty.Register("StartValue", typeof(double), typeof(StripeLine),
      new FrameworkPropertyMetadata(Double.NaN, OnStartValueChanged));

    private static void OnStartValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((StripeLine)d).OnStartValueChanged();
    }

    private void OnStartValueChanged()
    {
      BuildStripe();
    }

    #endregion // StartValue Property

    #region EndValue Property

    /// <summary>
    /// Gets or sets the axis plot position of the end of the stripe line.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="EndValueProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public double EndValue
    {
      get { return (double)GetValue(EndValueProperty); }
      set { SetValue(EndValueProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="EndValue"/> property.
    /// </summary>
    public static readonly DependencyProperty EndValueProperty =
      DependencyProperty.Register("EndValue", typeof(double), typeof(StripeLine),
      new FrameworkPropertyMetadata(Double.NaN, OnEndValueChanged));

    private static void OnEndValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((StripeLine)d).OnEndValueChanged();
    }

    private void OnEndValueChanged()
    {
      BuildStripe();
    }

    #endregion // EndValue Property

    #region Range Property

    /// <summary>
    /// Gets or sets the logical distance between the <see cref="StartValue"/> and the <see cref="EndValue"/>.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="RangeProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public double Range
    {
      get { return (double)GetValue(RangeProperty); }
      set { SetValue(RangeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Range"/> property.
    /// </summary>
    public static readonly DependencyProperty RangeProperty =
      DependencyProperty.Register("Range", typeof(double), typeof(StripeLine),
      new FrameworkPropertyMetadata(Double.NaN, OnRangeChanged));

    private static void OnRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((StripeLine)d).OnRangeChanged();
    }

    private void OnRangeChanged()
    {
      BuildStripe();
    }

    #endregion // Range Property

    #region Content Property

    /// <summary>
    /// Gets or sets the optional content displayed on the stripe line.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ContentProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public object Content
    {
      get { return GetValue(ContentProperty); }
      set { SetValue(ContentProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Content"/> property.
    /// </summary>
    public static readonly DependencyProperty ContentProperty =
      DependencyProperty.Register("Content", typeof(object), typeof(StripeLine),
      new FrameworkPropertyMetadata(OnContentChanged));

    private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((StripeLine)d).OnContentChanged();
    }

    private void OnContentChanged()
    {
      BuildStripe();
    }

    #endregion // Content Property

    #region ContentTemplate Property

    /// <summary>
    /// Gets or sets the <see cref="DataTemplate"/> used to display the content.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ContentTemplateProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public DataTemplate ContentTemplate
    {
      get { return (DataTemplate)GetValue(ContentTemplateProperty); }
      set { SetValue(ContentTemplateProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ContentTemplate"/> property.
    /// </summary>
    public static readonly DependencyProperty ContentTemplateProperty =
      DependencyProperty.Register("ContentTemplate", typeof(DataTemplate), typeof(StripeLine),
      new FrameworkPropertyMetadata(OnContentTemplateChanged));

    private static void OnContentTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((StripeLine)d).OnContentTemplateChanged();
    }

    private void OnContentTemplateChanged()
    {
      BuildStripe();
    }

    #endregion // ContentTemplate Property

    #region StripeStyle Property

    /// <summary>
    /// Gets or sets the <see cref="Style"/> applied to the stripe. The target type of the style is <see cref="Shape"/>.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="StripeStyleProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public Style StripeStyle
    {
      get { return (Style)GetValue(StripeStyleProperty); }
      set { SetValue(StripeStyleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="StripeStyle"/> property.
    /// </summary>
    public static readonly DependencyProperty StripeStyleProperty =
      DependencyProperty.Register("StripeStyle", typeof(Style), typeof(StripeLine),
      new FrameworkPropertyMetadata(BuildDefaultStripeStyle(), OnStripeStyleChanged));

    private static Style BuildDefaultStripeStyle()
    {
      Style style = new Style(typeof(Shape));
      style.Setters.Add(new Setter(Shape.StrokeThicknessProperty, 1.0));
      style.Setters.Add(new Setter(Shape.StrokeProperty, Brushes.Black));
      style.Setters.Add(new Setter(Shape.FillProperty, new SolidColorBrush(new Color() { A = 125, R = 200, G = 200, B = 200 })));
      return style;
    }

    private static void OnStripeStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((StripeLine)d).OnStripeStyleChanged();
    }

    private void OnStripeStyleChanged()
    {
      BuildStripe();
    }

    #endregion // StripeStyle Property

    #region YAxis property

    /// <summary>
    /// Gets or sets the Y axis. By default this will be the primary Y axis of the host chart.
    /// This is a dependency property.
    /// </summary>
    public ChartAxis YAxis
    {
      get { return (ChartAxis)GetValue(YAxisProperty); }
      set { SetValue(YAxisProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="YAxis"/> property.
    /// </summary>
    public static readonly DependencyProperty YAxisProperty =
      DependencyProperty.Register("YAxis", typeof(ChartAxis), typeof(StripeLine),
      new PropertyMetadata(new PropertyChangedCallback(OnYAxisChanged)));

    private static void OnYAxisChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((StripeLine)d).OnYAxisChanged(e);
    }

    private void OnYAxisChanged(DependencyPropertyChangedEventArgs e)
    {
      ChartAxis oldAxis = e.OldValue as ChartAxis;
      if (oldAxis != null)
      {
        oldAxis.AxisRendered -= new EventHandler(Axis_AxisUpdated);
      }
      if (YAxis != null)
      {
        YAxis.AxisRendered += new EventHandler(Axis_AxisUpdated);
        BuildStripe();
      }
    }

    #endregion // YAxis property

    #region XAxis property

    /// <summary>
    /// Gets or sets the X axis. By default this will be the primary X axis of the host chart.
    /// This is a dependency property.
    /// </summary>
    public ChartAxis XAxis
    {
      get { return (ChartAxis)GetValue(XAxisProperty); }
      set { SetValue(XAxisProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="XAxis"/> property.
    /// </summary>
    public static readonly DependencyProperty XAxisProperty =
      DependencyProperty.Register("XAxis", typeof(ChartAxis), typeof(StripeLine),
      new PropertyMetadata(new PropertyChangedCallback(OnXAxisChanged)));

    private static void OnXAxisChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((StripeLine)d).OnXAxisChanged(e);
    }

    private void OnXAxisChanged(DependencyPropertyChangedEventArgs e)
    {
      ChartAxis oldAxis = e.OldValue as ChartAxis;
      if (oldAxis != null)
      {
        oldAxis.AxisRendered -= new EventHandler(Axis_AxisUpdated);
      }
      if (XAxis != null)
      {
        XAxis.AxisRendered += new EventHandler(Axis_AxisUpdated);
        BuildStripe();
      }
    }

    #endregion // XAxis property

    #region Orientation Property

    /// <summary>
    /// Gets or sets the orientation of the marked stripes. The default is horizontal.
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
      DependencyProperty.Register("Orientation", typeof(Orientation), typeof(StripeLine),
      new FrameworkPropertyMetadata(Orientation.Horizontal, OnOrientationChanged));

    private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((StripeLine)d).OnOrientationChanged();
    }

    private void OnOrientationChanged()
    {
      BuildStripe();
    }

    #endregion // Orientation Property

    private void Axis_AxisUpdated(object sender, EventArgs e)
    {
      BuildStripe();
    }

    private void BuildStripe()
    {
      if (Orientation == Orientation.Horizontal)
      {
        BuildHorizontalStripeLine();
      }
      else
      {
        BuildVerticalStripeLine();
      }
    }

    private void BuildHorizontalStripeLine()
    {
      if (YAxis != null)
      {
        if (!Double.IsNaN(StartValue))
        {
          double logicalStart = StartValue;
          double logicalEnd = StartValue;
          if (!Double.IsNaN(EndValue))
          {
            logicalEnd = EndValue;
          }
          else if (!Double.IsNaN(Range))
          {
            logicalEnd = logicalStart + Range;
          }
          if (_contentHost != null)
          {
            _contentHost.SizeChanged -= new SizeChangedEventHandler(ContentHost_SizeChanged);
          }
          Children.Clear();

          double physicalStart = Math.Round(ActualHeight - YAxis.ConvertLogicalToPhysical(logicalStart));
          double physicalEnd = Math.Round(ActualHeight - YAxis.ConvertLogicalToPhysical(logicalEnd));

          if (logicalEnd == logicalStart)
          {
            Line startLine = new Line();
            startLine.Style = StripeStyle;
            startLine.X1 = 0;
            startLine.X2 = ActualWidth;
            startLine.Y1 = startLine.Y2 = physicalStart;
            Children.Add(startLine);
          }
          else
          {
            Rectangle stripe = new Rectangle();
            stripe.Style = StripeStyle;
            stripe.Width = ActualWidth + 100;
            stripe.Height = Math.Abs(physicalEnd - physicalStart);
            Canvas.SetTop(stripe, Math.Min(physicalStart, physicalEnd));
            Canvas.SetLeft(stripe, -50);
            Children.Add(stripe);
          }

          if (Content != null || ContentTemplate != null)
          {
            _contentHost = new ContentControl();
            _contentHost.Content = Content;
            _contentHost.ContentTemplate = ContentTemplate;
            _contentHost.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            Canvas.SetLeft(_contentHost, ActualWidth - _contentHost.DesiredSize.Width);
            Canvas.SetTop(_contentHost, physicalStart - _contentHost.DesiredSize.Height);
            Children.Add(_contentHost);
            _contentHost.SizeChanged += new SizeChangedEventHandler(ContentHost_SizeChanged);
          }
        }
      }
    }

    private void ContentHost_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      ContentControl contentHost = sender as ContentControl;
      if (contentHost != null && !Double.IsNaN(StartValue))
      {
        if (Orientation == Orientation.Horizontal)
        {
          double physicalStart = Math.Round(ActualHeight - YAxis.ConvertLogicalToPhysical(StartValue));
          Canvas.SetLeft(contentHost, ActualWidth - contentHost.DesiredSize.Width);
          Canvas.SetTop(contentHost, physicalStart - contentHost.DesiredSize.Height);
        }
        else
        {
          double physicalStart = Math.Round(XAxis.ConvertLogicalToPhysical(StartValue));
          Canvas.SetLeft(contentHost, physicalStart - contentHost.DesiredSize.Height);
          Canvas.SetTop(contentHost, contentHost.DesiredSize.Width);
        }
      }
    }

    private void BuildVerticalStripeLine()
    {
      if (XAxis != null)
      {
        if (!Double.IsNaN(StartValue))
        {
          double logicalStart = StartValue;
          double logicalEnd = StartValue;
          if (!Double.IsNaN(EndValue))
          {
            logicalEnd = EndValue;
          }
          else if (!Double.IsNaN(Range))
          {
            logicalEnd = logicalStart + Range;
          }
          if (_contentHost != null)
          {
            _contentHost.SizeChanged -= new SizeChangedEventHandler(ContentHost_SizeChanged);
          }
          Children.Clear();

          double physicalStart = Math.Round(XAxis.ConvertLogicalToPhysical(logicalStart));
          double physicalEnd = Math.Round(XAxis.ConvertLogicalToPhysical(logicalEnd));

          if (logicalEnd == logicalStart)
          {
            Line startLine = new Line();
            startLine.Style = StripeStyle;
            startLine.Y1 = 0;
            startLine.Y2 = ActualHeight;
            startLine.X1 = startLine.X2 = physicalStart;
            Children.Add(startLine);
          }
          else
          {
            Rectangle stripe = new Rectangle();
            stripe.Style = StripeStyle;
            stripe.Height = ActualHeight + 100;
            stripe.Width = Math.Abs(physicalEnd - physicalStart);
            Canvas.SetLeft(stripe, Math.Min(physicalStart, physicalEnd));
            Canvas.SetTop(stripe, -50);
            Children.Add(stripe);
          }

          if (Content != null || ContentTemplate != null)
          {
            _contentHost = new ContentControl();
            _contentHost.Content = Content;
            _contentHost.ContentTemplate = ContentTemplate;
            _contentHost.RenderTransform = new RotateTransform(-90);
            _contentHost.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            Canvas.SetLeft(_contentHost, physicalStart - _contentHost.DesiredSize.Height);
            Canvas.SetTop(_contentHost, _contentHost.DesiredSize.Width);
            Children.Add(_contentHost);
            _contentHost.SizeChanged += new SizeChangedEventHandler(ContentHost_SizeChanged);
          }
        }
      }
    }
  }
}
