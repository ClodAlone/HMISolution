using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.ComponentModel;
using Infralution.Licensing;
using System.Reflection;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// A radial guage which uses a needle to point to the current value
  /// </summary>
  [LicenseProvider(typeof(PublicEncryptedLicenseProvider))]
  public class RadialGauge : AnimatedValueDisplay
  {
    private Path _arc;
    private FrameworkElement _needle;
    private ObservableCollection<GaugeLabel> _labels = new ObservableCollection<GaugeLabel>();
    private ObservableCollection<Border> _tickMarks = new ObservableCollection<Border>();
    private ItemsControl _labelsHost;
    private ItemsControl _tickMarksHost;
    private double _spacing;

    static RadialGauge()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(RadialGauge), new FrameworkPropertyMetadata(typeof(RadialGauge)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RadialGauge"/> class.
    /// </summary>
    public RadialGauge()
    {
      // Licensing
      //new WpfElementsCore(Assembly.GetCallingAssembly());
      //LicenseHelper.Attach(this, Assembly.GetCallingAssembly());
      // End licensing

      SizeChanged += new SizeChangedEventHandler(RadialGauge_SizeChanged);
    }

    /// <summary>
    /// Called when the displayed value changes during animation.
    /// </summary>
    protected override void OnDisplayedValueChanged()
    {
      UpdateNeedleRotation();
    }

    private void RadialGauge_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      SetValue(SizePropertyKey, Math.Min(e.NewSize.Width, e.NewSize.Height));
      UpdateArc();
      UpdateNeedleRotation();
      UpdateLabels();
      UpdateTickMarks();
    }

    /// <summary>
    /// Called by the framework when the control template is applied.
    /// </summary>
    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      _arc = GetTemplateChild("PART_Arc") as Path;
      _needle = GetTemplateChild("PART_Needle") as FrameworkElement;
      _labelsHost = GetTemplateChild("PART_LabelsHost") as ItemsControl;
      _tickMarksHost = GetTemplateChild("PART_TickMarksHost") as ItemsControl;
      _labels = new ObservableCollection<GaugeLabel>();
      _tickMarks = new ObservableCollection<Border>();
      if (_labelsHost != null)
      {
        _labelsHost.ItemsSource = _labels;
      }
      if (_tickMarksHost != null)
      {
        _tickMarksHost.ItemsSource = _tickMarks;
      }

      UpdateArc();
      UpdateNeedleRotation();
      UpdateLabels();
      UpdateTickMarks();
    }

    /*/// <summary>
    /// Overrides the measure pass behavior.
    /// </summary>
    /// <param name="availableSize">The availbale size that the <see cref="RadialGauge"/> will fit within.</param>
    /// <returns>The desired size of the <see cref="RadialGauge"/>.</returns>
    protected override Size MeasureOverride(Size availableSize)
    {
      double size = Math.Min(availableSize.Width, availableSize.Height);
      MaxWidth = size;
      MaxHeight = size;
      return new Size(size, size);
    }*/

    /// <summary>
    /// Called when the minimum value changes.
    /// </summary>
    /// <param name="oldMinimum">The old minimum.</param>
    /// <param name="newMinimum">The new minimum.</param>
    protected override void OnMinimumChanged(double oldMinimum, double newMinimum)
    {
      base.OnMinimumChanged(oldMinimum, newMinimum);

      UpdateSpacing();
      UpdateArc();
      UpdateNeedleRotation();
      UpdateLabels();
      UpdateTickMarks();
    }

    /// <summary>
    /// Called when the maximum value changes.
    /// </summary>
    /// <param name="oldMaximum">The old maximum.</param>
    /// <param name="newMaximum">The new maximum.</param>
    protected override void OnMaximumChanged(double oldMaximum, double newMaximum)
    {
      base.OnMaximumChanged(oldMaximum, newMaximum);

      UpdateSpacing();
      UpdateArc();
      UpdateNeedleRotation();
      UpdateLabels();
      UpdateTickMarks();
    }

    private void UpdateArc()
    {
      if (_arc != null)
      {
        _arc.Data = BuildArc();
        _arc.Visibility = ArcVisibility;
      }
    }

    // TODO: building the arc in code makes it difficult to customize:
    private PathGeometry BuildArc()
    {
      double size = Size;
      double margin = 40;
      double radius = Math.Max(0, size / 2.0 - margin);

      PathGeometry geo = new PathGeometry();
      PathFigure figure = new PathFigure();

      Point startPoint = GeometryUtils.GetPosition(StartAngle, radius);
      startPoint.X = (size / 2.0) + startPoint.X;
      startPoint.Y = (size / 2.0) - startPoint.Y;
      figure.StartPoint = startPoint;

      ArcSegment arcSegment = new ArcSegment();
      arcSegment.SweepDirection = SweepDirection.Clockwise;
      Point endPoint = GeometryUtils.GetPosition(StartAngle + SweepAngle, radius);
      endPoint.X = (size / 2.0) + endPoint.X;
      endPoint.Y = (size / 2.0) - endPoint.Y;
      arcSegment.Point = endPoint;
      arcSegment.Size = new Size(Math.Max(0, (size - (margin * 2)) / 2.0), Math.Max(0, (size - (margin * 2)) / 2.0));
      arcSegment.RotationAngle = SweepAngle;
      arcSegment.IsLargeArc = SweepAngle > 180 ? true : false;

      figure.Segments.Add(arcSegment);
      geo.Figures.Add(figure);
      return geo;
    }

    // TODO: since the needle is a Path, its a bit limited in customization. Maybe make a Needle control?
    // May also want a way to specify the point of rotation.
    private void UpdateNeedleRotation()
    {
      if (_needle != null)
      {
        RotateTransform rotation = new RotateTransform();
        rotation.CenterY = (Size / 2.0) - 20;
        rotation.CenterX = _needle.ActualWidth / 2.0;

        double angle = GetAngle(DisplayedValue);
        rotation.Angle = angle;
        _needle.RenderTransform = rotation;
      }
    }

    private double GetAngle(double value)
    {
      double range = Maximum - Minimum;
      double ratio = (value - Minimum) / range;
      double angle = StartAngle + (SweepAngle * ratio);
      return angle;
    }

    // TODO: I guess it's fine populating the lables in code. The chart controls do this and it works quite well.
    // May need to experiment with label positioning customization though.
    private void UpdateLabels()
    {
      _labels.Clear();
      double value = Minimum;
      if (_spacing > 0)
      {
        while (value <= Maximum)
        {
          GaugeLabel label = BuildLabel(value);
          Point position = GetLabelPosition(GetAngle(value));

          TransformGroup group = new TransformGroup();
          TranslateTransform translation = new TranslateTransform();
          translation.X = position.X;
          translation.Y = -position.Y;
          if (Size < 100)
          {
            translation.X -= 50 - (Size / 2.0);
            translation.Y -= 50 - (Size / 2.0);
          }
          RotateTransform rotation = new RotateTransform();
          rotation.Angle = GetAngle(value);
          rotation.CenterX = 50;
          rotation.CenterY = 50;
          group.Children.Add(rotation);
          group.Children.Add(translation);
          label.RenderTransform = group;

          label.Width = 100;
          label.Height = 100;
          _labels.Add(label);
          value += _spacing;
        }
      }
    }

    private void UpdateTickMarks()
    {
      _tickMarks.Clear();
      double value = Minimum;
      if (_spacing > 0)
      {
        while (value <= Maximum)
        {
          Border tick = BuildTickMark();
          Point position = GetTickMarkPosition(GetAngle(value));

          TransformGroup group = new TransformGroup();
          TranslateTransform translation = new TranslateTransform();
          translation.X = position.X;
          translation.Y = -position.Y;
          if (Size < 100)
          {
            translation.X -= 50 - (Size / 2.0);
            translation.Y -= 50 - (Size / 2.0);
          }
          RotateTransform rotation = new RotateTransform();
          rotation.Angle = GetAngle(value);
          rotation.CenterX = tick.Width / 2.0;
          rotation.CenterY = tick.Height / 2.0;
          group.Children.Add(rotation);
          group.Children.Add(translation);
          tick.RenderTransform = group;

          _tickMarks.Add(tick);
          value += _spacing;
        }
      }
      if (_tickMarksHost != null)
      {
        _tickMarksHost.Visibility = MajorTickMarkVisibility;
      }
    }

    private Point GetLabelPosition(double angle)
    {
      double labelRadius = (Size / 2.0) - 22;
      Point position = GeometryUtils.GetPosition(angle, labelRadius);
      //position.X = (ActualWidth / 2.0) + position.X;
      //position.Y = (ActualWidth / 2.0) - position.Y;
      return position;
    }

    private GaugeLabel BuildLabel(object label)
    {
      GaugeLabel gaugeLabel = new GaugeLabel();
      gaugeLabel.Label = label;
      return gaugeLabel;
    }

    private Point GetTickMarkPosition(double angle)
    {
      double labelRadius = (Size / 2.0) - 40;
      Point position = GeometryUtils.GetPosition(angle, labelRadius);
      //position.X = (ActualWidth / 2.0) + position.X;
      //position.Y = (ActualWidth / 2.0) - position.Y;
      return position;
    }

    private Border BuildTickMark()
    {
      Border tickMark = new Border();
      //tickMark.Width = 1;
      //tickMark.Height = 5;
      //tickMark.Background = new SolidColorBrush(Colors.White);
      tickMark.Style = MajorTickMarkStyle;
      return tickMark;
    }

    // TODO: reuse the logic from the chart controls here:
    private void UpdateSpacing()
    {
      if (MajorTickSpacing != 0)
      {
        _spacing = MajorTickSpacing;
      }
      else
      {
        double range = Maximum - Minimum;

        if (range > 0)
        {
          double spacing = range / 6.0;
          int count = 0;
          bool small = spacing < 1;
          while (spacing < 1)
          {
            spacing *= 10;
            count++;
          }
          while (spacing > 10)
          {
            spacing /= 10;
            count++;
          }

          if (spacing < 1.5)
          {
            spacing = 1;
          }
          else if (spacing < 2)
          {
            spacing = 2;
          }
          else if (spacing < 3.5)
          {
            spacing = 5;
          }
          else
          {
            spacing = 10;
          }
          if (small)
          {
            spacing /= Math.Pow(10, count);
          }
          else
          {
            spacing *= Math.Pow(10, count);
          }

          _spacing = spacing;
        }
        else
        {
          _spacing = 1;
        }
      }
    }

    #region MajorTickMarkStyle Property

    /// <summary>
    /// Gets or sets the <see cref="Style"/> for displaying major tick marks. This is mainly used to set the size and color of the tick marks.
    /// Tick marks are <see cref="Border"/> objects.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="MajorTickMarkStyleProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public Style MajorTickMarkStyle
    {
      get { return (Style)GetValue(MajorTickMarkStyleProperty); }
      set { SetValue(MajorTickMarkStyleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="MajorTickMarkStyle"/> property.
    /// </summary>
    public static readonly DependencyProperty MajorTickMarkStyleProperty =
      DependencyProperty.Register("MajorTickMarkStyle", typeof(Style), typeof(RadialGauge),
      new FrameworkPropertyMetadata(OnMajorTickMarkStyleChanged));

    private static void OnMajorTickMarkStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((RadialGauge)d).OnMajorTickMarkStyleChanged();
    }

    private void OnMajorTickMarkStyleChanged()
    {
      UpdateTickMarks();
    }

    #endregion // MajorTickMarkStyle Property

    #region MajorTickMarkSpacing Property

    /// <summary>
    /// Gets or sets the logical spacing between major tick marks and labels. When this property is set to 0, the logical spacing will be
    /// calculated automatically. The default is 0.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="MajorTickSpacingProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public double MajorTickSpacing
    {
      get { return (double)GetValue(MajorTickSpacingProperty); }
      set { SetValue(MajorTickSpacingProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="MajorTickSpacing"/> property.
    /// </summary>
    public static readonly DependencyProperty MajorTickSpacingProperty =
      DependencyProperty.Register("MajorTickSpacing", typeof(double), typeof(RadialGauge),
      new FrameworkPropertyMetadata(OnMajorTickSpacingChanged));

    private static void OnMajorTickSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((RadialGauge)d).OnMajorTickSpacingChanged();
    }

    private void OnMajorTickSpacingChanged()
    {
      UpdateSpacing();
      UpdateLabels();
      UpdateTickMarks();
    }

    #endregion // MajorTickMarkSpacing Property

    #region IsArcVisible Property

    /// <summary>
    /// Gets or sets the visibility of the arc. The default is visible.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ArcVisibilityProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public Visibility ArcVisibility
    {
      get { return (Visibility)GetValue(ArcVisibilityProperty); }
      set { SetValue(ArcVisibilityProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ArcVisibility"/> property.
    /// </summary>
    public static readonly DependencyProperty ArcVisibilityProperty =
      DependencyProperty.Register("ArcVisibility", typeof(Visibility), typeof(RadialGauge),
      new FrameworkPropertyMetadata(Visibility.Visible, OnArcVisibilityChanged));

    private static void OnArcVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((RadialGauge)d).OnArcVisibilityChanged();
    }

    private void OnArcVisibilityChanged()
    {
      UpdateArc();
    }

    #endregion // IsArcVisible Property

    #region MajorTickMarkVisibility Property

    /// <summary>
    /// Gets or sets the visibility of the major tick marks. The default is collapsed.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="MajorTickMarkVisibilityProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public Visibility MajorTickMarkVisibility
    {
      get { return (Visibility)GetValue(MajorTickMarkVisibilityProperty); }
      set { SetValue(MajorTickMarkVisibilityProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="MajorTickMarkVisibility"/> property.
    /// </summary>
    public static readonly DependencyProperty MajorTickMarkVisibilityProperty =
      DependencyProperty.Register("MajorTickMarkVisibility", typeof(Visibility), typeof(RadialGauge),
      new FrameworkPropertyMetadata(Visibility.Collapsed, OnMajorTickMarkVisibilityChanged));

    private static void OnMajorTickMarkVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((RadialGauge)d).OnMajorTickMarkVisibilityChanged();
    }

    private void OnMajorTickMarkVisibilityChanged()
    {
      UpdateTickMarks();
    }

    #endregion // MajorTickMarkVisibility Property

    #region Size Property

    /// <summary>
    /// Gets the size of the <see cref="RadialGauge"/>. This can be used for both the width and the height.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="SizeProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public double Size
    {
      get { return (double)GetValue(SizeProperty); }
    }

    private static readonly DependencyPropertyKey SizePropertyKey =
        DependencyProperty.RegisterReadOnly("Size", typeof(double), typeof(RadialGauge), new UIPropertyMetadata(0.0));

    /// <summary>
    /// Identifies the <see cref="Size"/> property.
    /// </summary>
    public static readonly DependencyProperty SizeProperty =
        SizePropertyKey.DependencyProperty;

    #endregion // Size Property

    #region StartAngle property

    /// <summary>
    /// Gets or sets the angle where the first label is placed. A value of 0 will put the first label at the top of the gauge.
    /// Larger values move around the gauge in a clockwise direction. The angle is in degrees.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="StartAngleProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public double StartAngle
    {
      get { return (double)GetValue(StartAngleProperty); }
      set { SetValue(StartAngleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="StartAngle"/> property.
    /// </summary>
    public static readonly DependencyProperty StartAngleProperty =
      DependencyProperty.Register("StartAngle", typeof(double), typeof(RadialGauge),
      new FrameworkPropertyMetadata(-120.0, OnStartAngleChanged));

    private static void OnStartAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((RadialGauge)d).OnStartAngleChanged();
    }

    private void OnStartAngleChanged()
    {
      UpdateArc();
      UpdateNeedleRotation();
      UpdateLabels();
      UpdateTickMarks();
    }

    #endregion // StartAngle property

    #region SweepAngle property

    /// <summary>
    /// Gets or sets the angle between the first and last labels. Labels are positioned in a clockwise direction starting from the StartAngle.
    /// The SweepAngle is in degrees.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="SweepAngleProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public double SweepAngle
    {
      get { return (double)GetValue(SweepAngleProperty); }
      set { SetValue(SweepAngleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="SweepAngle"/> property.
    /// </summary>
    public static readonly DependencyProperty SweepAngleProperty =
      DependencyProperty.Register("SweepAngle", typeof(double), typeof(RadialGauge),
      new FrameworkPropertyMetadata(240.0, OnSweepAngleChanged));

    private static void OnSweepAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((RadialGauge)d).OnSweepAngleChanged();
    }

    private void OnSweepAngleChanged()
    {
      UpdateArc();
      UpdateNeedleRotation();
      UpdateLabels();
      UpdateTickMarks();
    }

    #endregion // SweepAngle property
  }
}
