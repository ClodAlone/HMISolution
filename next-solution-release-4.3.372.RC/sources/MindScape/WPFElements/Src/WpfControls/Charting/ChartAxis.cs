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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Threading;
using System.Windows.Data;

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// Represents an axis for a <see cref="Chart"/>.
  /// </summary>
  [TemplatePart(Name = LabelsHostPartName, Type = typeof(ItemsControl))]
  [TemplatePart(Name = MinorTickMarksHostPartName, Type = typeof(ItemsControl))]
  [TemplatePart(Name = MajorTickMarksHostPartName, Type = typeof(ItemsControl))]
  public class ChartAxis : Control
  {
    private const string LabelsHostPartName = "PART_LabelsHost";
    private const string MinorTickMarksHostPartName = "PART_MinorTickMarksHost";
    private const string MajorTickMarksHostPartName = "PART_MajorTickMarksHost";

    private ObservableCollection<Border> _majorTickMarks = new ObservableCollection<Border>();
    private ObservableCollection<Border> _minorTickMarks = new ObservableCollection<Border>();
    private ObservableCollection<ContentControl> _labels = new ObservableCollection<ContentControl>();

    private ItemsControl _labelsHost, _minorTickHost, _majorTickHost;
    private DualSlider _slider;

    private double _spacing = 1;

    private bool _allowMajorTickSpacingZoomAdjustment = true;
    private bool _settingBothMinMax;
    private bool _isMinimumAuto = true;
    private bool _isMaximumAuto = true;

    static ChartAxis()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(ChartAxis),
        new FrameworkPropertyMetadata(typeof(ChartAxis)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ChartAxis"/> class.
    /// </summary>
    public ChartAxis()
    {
      _dataMap = new Dictionary<object, double>();
      SizeChanged += new SizeChangedEventHandler(ChartAxis_SizeChanged);
      Loaded += new RoutedEventHandler(ChartAxis_Loaded);
    }

    private void ChartAxis_Loaded(object sender, RoutedEventArgs e)
    {
      _maximumLock = true;
      //Maximum = GetObject(MaximumValue);
      if (!(Maximum is string))
      {
        UpdateMaximumFromMaximumValue(MaximumValue);
      }
      _maximumLock = false;
      _minimumLock = true;
      //Minimum = GetObject(MinimumValue);
      if (!(Minimum is string))
      {
        UpdateMinimumFromMinimumValue(MinimumValue);
      }
      _minimumLock = false;
    }

    private void ChartAxis_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      UpdateStartPadding();
      UpdateAxisSize();
      RenderAxis(true);
    }

    /// <summary>
    /// Called when a dependency property value changes.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
      base.OnPropertyChanged(e);
      if (e.Property == ChartAxis.PaddingProperty)
      {
        UpdateAxisSize();
      }
    }

    /// <summary>
    /// Called by the framework when the control template is applied.
    /// </summary>
    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      _labelsHost = GetTemplateChild(LabelsHostPartName) as ItemsControl;
      _minorTickHost = GetTemplateChild(MinorTickMarksHostPartName) as ItemsControl;
      _majorTickHost = GetTemplateChild(MajorTickMarksHostPartName) as ItemsControl;
      if (_labelsHost != null)
      {
        _labelsHost.ItemsSource = _labels;
      }
      if (_minorTickHost != null)
      {
        _minorTickHost.ItemsSource = _minorTickMarks;
      }
      if (_majorTickHost != null)
      {
        _majorTickHost.ItemsSource = _majorTickMarks;
      }

      _slider = VisualTreeUtils.GetChild<DualSlider>(this);
      if (_slider != null)
      {
        _slider.Minimum = MinimumValue;
        _slider.Maximum = MaximumValue;
        _slider.RangeEnd = ActualMaximumValue;
        _slider.RangeStart = ActualMinimumValue;
        _slider.RangeChanged += new EventHandler<RangeChangedEventArgs>(Slider_RangeChanged);
      }
    }

    private bool _sliderLock;

    private void Slider_RangeChanged(object sender, RangeChangedEventArgs e)
    {
      if (!_sliderLock)
      {
        _sliderLock = true;
        ActualMaximumValue = _slider.RangeEnd;
        ActualMinimumValue = _slider.RangeStart;
        _sliderLock = false;
      }
    }

    // Lets internal stuff know if this axis was automatically created rather than manually by the programmer.
    internal bool IsAuto { get; set; }

    // TODO: it would be good if the SetRange and Pan methods accept objects.

    /// <summary>
    /// Sets both the minimum and maximum values of the <see cref="ChartAxis"/>.
    /// </summary>
    /// <param name="min">The new minimum value.</param>
    /// <param name="max">The new maximum value.</param>
    public void SetRange(double min, double max)
    {
      _settingBothMinMax = true;
      MinimumValue = min;
      _settingBothMinMax = false;
      MaximumValue = max;
    }

    /// <summary>
    /// Changes the <see cref="ActualMinimum"/> and <see cref="ActualMaximum"/> by the given value. This does not change the difference between
    /// the <see cref="ActualMinimum"/> and <see cref="ActualMaximum"/>.
    /// </summary>
    /// <param name="delta">The logical distance to pan the axis.</param>
    /// <returns>The logical distance that the axis actually moved by.</returns>
    public double Pan(double delta)
    {
      if (delta < 0)
      {
        delta = -Math.Min(Math.Abs(delta), ActualMinimumValue - MinimumValue);
      }
      else
      {
        delta = Math.Min(delta, MaximumValue - ActualMaximumValue);
      }
      ActualMaximumValue += delta;
      ActualMinimumValue += delta;
      return delta;
    }

    internal bool IsMinimumAuto
    {
      get { return _isMinimumAuto; }
      set
      {
        //if (_isMinimumAuto != value)
        {
          _isMinimumAuto = value;
          EventHandler handler = IsMinimumAutoChanged;
          if (handler != null)
          {
            handler(this, EventArgs.Empty);
          }
        }
      }
    }

    internal bool IsMaximumAuto
    {
      get { return _isMaximumAuto; }
      set
      {
        //if (_isMaximumAuto != value)
        {
          _isMaximumAuto = value;
          EventHandler handler = IsMaximumAutoChanged;
          if (handler != null)
          {
            handler(this, EventArgs.Empty);
          }
        }
      }
    }

    internal event EventHandler IsMinimumAutoChanged;
    internal event EventHandler IsMaximumAutoChanged;

    internal bool IsMinOrMaxAuto
    {
      get { return IsMinimumAuto || IsMaximumAuto; }
    }

    /// <summary>
    /// Raised when either the ActualMinimum or ActualMaximum properties change.
    /// </summary>
    public event EventHandler RangeChanged;

    private void OnRangeChanged()
    {
      EventHandler handler = RangeChanged;
      if (handler != null)
      {
        handler(this, EventArgs.Empty);
      }
    }

    private bool _minimumLock;
    private bool _maximumLock;
    private bool _actualMinimumLock;
    private bool _actualMaximumLock;

    #region Maximum property

    /// <summary>
    /// Gets or sets the maximum value that the <see cref="ChartAxis"/> can display.
    /// This is a dependency property.
    /// </summary>
    public object Maximum
    {
      get { return GetValue(MaximumProperty); }
      set { SetValue(MaximumProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Maximum"/> property.
    /// </summary>
    public static readonly DependencyProperty MaximumProperty =
      DependencyProperty.Register("Maximum", typeof(object), typeof(ChartAxis),
      new PropertyMetadata(Double.NaN, new PropertyChangedCallback(OnMaximumChanged)));

    private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ChartAxis)d).OnMaximumChanged();
    }

    private void OnMaximumChanged()
    {
      if (!_maximumLock)
      {
        if (Maximum is string)
        {
          double value;
          bool success = Double.TryParse((string)Maximum, out value);
          if (success)
          {
            Maximum = value;
            return;
          }
          else
          {
            IsMaximumAuto = false;
            IsAuto = false;
          }
        }
        else
        {
          _maximumLock = true;
          MaximumValue = GetLogicalPosition(Maximum);
          _maximumLock = false;
        }
      }
    }

    internal void UpdateMaximumValueFromMaximum()
    {
      _maximumLock = true;
      MaximumValue = GetLogicalPosition(Maximum);
      _maximumLock = false;
    }

    private bool _isSettingMaximumInternal;
    private bool _coerceLock;

    internal void SetMaximumInternal(double maximum)
    {
      _isSettingMaximumInternal = true;
      MaximumValue = maximum;
      _isSettingMaximumInternal = false;
    }

    private double _maximumValue;

    private void UpdateMaximumFromMaximumValue(double value)
    {
      object o = GetObject(value);
      object maximum = Maximum;
      if (Maximum is DateTime)
      {
        maximum = ((DateTime)Maximum).RoundNearest(new TimeSpan(0, 0, 0, 0, 1));
      }
      if (maximum == null || !maximum.Equals(o))
      {
        Maximum = o;
      }
    }

    internal double MaximumValue
    {
      get { return _maximumValue; }
      private set
      {
        _maximumValue = value;
        if (!_maximumLock)
        {
          _maximumLock = true;
          if (IsLoaded)
          {
            //Maximum = GetObject(value);
            UpdateMaximumFromMaximumValue(value);
          }
          _maximumLock = false;
        }

        if (MaximumValue < MinimumValue && !_coerceLock)
        {
          SetMinimumInternal(MaximumValue);
        }
        if (_slider != null)
        {
          _slider.Maximum = MaximumValue;
        }

        if (IsAutoViewportEnabled)
        {
          ActualMaximumValue = MaximumValue;
        }

        if (!_isSettingMaximumInternal)
        {
          IsMaximumAuto = Double.IsNaN(MaximumValue);
        }
      }
    }

    #endregion // Maximum property

    #region Minimum property

    /// <summary>
    /// Gets or sets the minimum value that the <see cref="ChartAxis"/> can display.
    /// This is a dependency property.
    /// </summary>
    public object Minimum
    {
      get { return GetValue(MinimumProperty); }
      set { SetValue(MinimumProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Minimum"/> property.
    /// </summary>
    public static readonly DependencyProperty MinimumProperty =
      DependencyProperty.Register("Minimum", typeof(object), typeof(ChartAxis),
      new PropertyMetadata(Double.NaN, new PropertyChangedCallback(OnMinimumChanged)));

    private static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ChartAxis)d).OnMinimumChanged();
    }

    private void OnMinimumChanged()
    {
      if (!_minimumLock)
      {
        if (Minimum is string)
        {
          double value;
          bool success = Double.TryParse((string)Minimum, out value);
          if (success)
          {
            Minimum = value;
            return;
          }
          else
          {
            IsMinimumAuto = false;
            IsAuto = false;
          }
        }
        else
        {
          _minimumLock = true;
          MinimumValue = GetLogicalPosition(Minimum);
          _minimumLock = false;
        }
      }
    }

    internal void UpdateMinimumValueFromMinimum()
    {
      _minimumLock = true;
      MinimumValue = GetLogicalPosition(Minimum);
      _minimumLock = false;
    }

    private bool _isSettingMinimumInternal;

    internal void SetMinimumInternal(double minimum)
    {
      _isSettingMinimumInternal = true;
      MinimumValue = minimum;
      _isSettingMinimumInternal = false;
    }

    private double _minimumValue;

    private void UpdateMinimumFromMinimumValue(double value)
    {
      object o = GetObject(value);
      object minimum = Minimum;
      if (Minimum is DateTime)
      {
        minimum = ((DateTime)Minimum).RoundNearest(new TimeSpan(0, 0, 0, 0, 1));
      }
      if (minimum == null || !minimum.Equals(o))
      {
        Minimum = o;
      }
    }

    internal double MinimumValue
    {
      get { return _minimumValue; }
      set
      {
        _minimumValue = value;
        if (!_minimumLock)
        {
          _minimumLock = true;
          if (IsLoaded)
          {
            //Minimum = GetObject(value);
            UpdateMinimumFromMinimumValue(value);
          }
          _minimumLock = false;
        }

        if (MinimumValue > MaximumValue && !_coerceLock)
        {
          SetMaximumInternal(MinimumValue);
        }
        if (_slider != null)
        {
          _slider.Minimum = MinimumValue;
        }

        if (IsAutoViewportEnabled)
        {
          ActualMinimumValue = MinimumValue;
        }

        if (!_isSettingMinimumInternal)
        {
          IsMinimumAuto = Double.IsNaN(MinimumValue);
        }
      }
    }

    #endregion // Minimum property

    #region ActualMinimum property

    /// <summary>
    /// Gets or sets the minimum value that is currently visible on the <see cref="ChartAxis"/>.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>This property can be used for programmatic panning and zooming.</remarks>
    public object ActualMinimum
    {
      get { return GetValue(ActualMinimumProperty); }
      set { SetValue(ActualMinimumProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ActualMinimum"/> property.
    /// </summary>
    public static readonly DependencyProperty ActualMinimumProperty =
      DependencyProperty.Register("ActualMinimum", typeof(object), typeof(ChartAxis),
      new PropertyMetadata(new PropertyChangedCallback(OnActualMinimumChanged)));

    private static void OnActualMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ChartAxis)d).OnActualMinimumChanged();
    }

    private void OnActualMinimumChanged()
    {
      if (!_actualMinimumLock)
      {
        if (ActualMinimum is string)
        {
          double value;
          bool success = Double.TryParse((string)ActualMinimum, out value);
          if (success)
          {
            ActualMinimum = value;
            return;
          }
        }

        _actualMinimumLock = true;
        ActualMinimumValue = GetLogicalPosition(ActualMinimum);
        _actualMinimumLock = false;
      }
    }

    private double _actualMinimumValue;

    internal double ActualMinimumValue
    {
      get { return _actualMinimumValue; }
      set
      {
        _actualMinimumValue = value;
        if (!_actualMinimumLock)
        {
          _actualMinimumLock = true;
          ActualMinimum = GetObject(value);
          _actualMinimumLock = false;
        }

        if (ActualMinimumValue.Equals(Double.NaN))
        {
          return;
        }
        if (ActualMinimumValue < MinimumValue && MaximumValue > MinimumValue)
        {
          _actualMinimumLock = false;
          ActualMinimumValue = MinimumValue;
        }
        else if (ActualMinimumValue > MaximumValue && MaximumValue > MinimumValue)
        {
          _actualMinimumLock = false;
          ActualMinimumValue = MaximumValue;
        }
        else
        {
          UpdateLogicalAxisSize();
          if (!_settingBothMinMax)
          {
            UpdateAxisLater();
          }
          if (ActualMinimumValue > ActualMaximumValue)
          {
            ActualMaximumValue = ActualMinimumValue;
          }
          else if (ActualMaximumValue - ActualMinimumValue < MinimumRange && MaximumValue - MinimumValue > MinimumRange)
          {
            _actualMinimumLock = false;
            ActualMinimumValue = Math.Max(ActualMinimumValue, ActualMaximumValue - MinimumRange);
          }
          else
          {
            UpdateSpacing();
          }
          if (_slider != null && !_sliderLock)
          {
            _sliderLock = true;
            _slider.RangeStart = ActualMinimumValue;
            _sliderLock = false;
          }
          OnRangeChanged();
        }
      }
    }

    #endregion // ActualMinimum property

    #region ActualMaximum property

    /// <summary>
    /// Gets or sets the maximum value that is currently visible on the <see cref="ChartAxis"/>.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>This property can be used for programmatic panning and zooming.</remarks>
    public object ActualMaximum
    {
      get { return GetValue(ActualMaximumProperty); }
      set { SetValue(ActualMaximumProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ActualMaximum"/> property.
    /// </summary>
    public static readonly DependencyProperty ActualMaximumProperty =
      DependencyProperty.Register("ActualMaximum", typeof(object), typeof(ChartAxis),
      new PropertyMetadata(new PropertyChangedCallback(OnActualMaximumChanged)));

    private static void OnActualMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ChartAxis)d).OnActualMaximumChanged();
    }

    private void OnActualMaximumChanged()
    {
      if (!_actualMaximumLock)
      {
        if (ActualMaximum is string)
        {
          double value;
          bool success = Double.TryParse((string)ActualMaximum, out value);
          if (success)
          {
            ActualMaximum = value;
            return;
          }
        }

        _actualMaximumLock = true;
        ActualMaximumValue = GetLogicalPosition(ActualMaximum);
        _actualMaximumLock = false;
      }
    }

    private double _actualMaximumValue;

    internal double ActualMaximumValue
    {
      get { return _actualMaximumValue; }
      set
      {
        _actualMaximumValue = value;
        if (!_actualMaximumLock)
        {
          _actualMaximumLock = true;
          ActualMaximum = GetObject(value);
          _actualMaximumLock = false;
        }

        if (ActualMaximumValue > MaximumValue && MaximumValue > MinimumValue)
        {
          _actualMaximumLock = false;
          ActualMaximumValue = MaximumValue;
        }
        else if (ActualMaximumValue < MinimumValue && MaximumValue > MinimumValue)
        {
          _actualMaximumLock = false;
          ActualMaximumValue = MinimumValue;
        }
        else
        {
          UpdateLogicalAxisSize();
          UpdateAxisLater();
          if (ActualMaximumValue < ActualMinimumValue)
          {
            ActualMinimumValue = ActualMaximumValue;
          }
          else if (ActualMaximumValue - ActualMinimumValue < MinimumRange && MaximumValue - MinimumValue > MinimumRange)
          {
            double newActualMaximumValue = ActualMinimumValue + MinimumRange;
            if (newActualMaximumValue < ActualMaximumValue)
            {
              _actualMaximumLock = false;
              ActualMaximumValue = ActualMinimumValue + MinimumRange;
            }
          }
          else
          {
            UpdateSpacing();
          }
          if (_slider != null && !_sliderLock)
          {
            _sliderLock = true;
            _slider.RangeEnd = ActualMaximumValue;
            _sliderLock = false;
          }
          OnRangeChanged();
        }
      }
    }

    #endregion // ActualMaximum property

    #region IsAutoViewportEnabled Property

    /// <summary>
    /// Gets or sets whether or not the viewport of the axis will update when the minimum and maximum values change. The default is true.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsAutoViewportEnabledProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsAutoViewportEnabled
    {
      get { return (bool)GetValue(IsAutoViewportEnabledProperty); }
      set { SetValue(IsAutoViewportEnabledProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsAutoViewportEnabled"/> property.
    /// </summary>
    public static readonly DependencyProperty IsAutoViewportEnabledProperty =
      DependencyProperty.Register("IsAutoViewportEnabled", typeof(bool), typeof(ChartAxis),
      new FrameworkPropertyMetadata(true, OnIsAutoViewportEnabledChanged));

    private static void OnIsAutoViewportEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ChartAxis)d).OnIsAutoViewportEnabledChanged(e);
    }

    private void OnIsAutoViewportEnabledChanged(DependencyPropertyChangedEventArgs e)
    {
    }

    #endregion // IsAutoViewportEnabled Property

    #region MinimumRange property

    /// <summary>
    /// Gets or sets the minimum logic distance between the ActualMinimum and ActualMaximum.
    /// The default is 0.0
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="MinimumRangeProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public double MinimumRange
    {
      get { return (double)GetValue(MinimumRangeProperty); }
      set { SetValue(MinimumRangeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="MinimumRange"/> property.
    /// </summary>
    public static readonly DependencyProperty MinimumRangeProperty =
      DependencyProperty.Register("MinimumRange", typeof(double), typeof(ChartAxis),
      new FrameworkPropertyMetadata(0.0, OnMinimumRangeChanged));

    private static void OnMinimumRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ChartAxis)d).OnMinimumRangeChanged();
    }

    private void OnMinimumRangeChanged()
    {
      // TODO: validate current range
    }

    #endregion // MinimumRange property

    #region Orientation property

    /// <summary>
    /// Gets the orientation of the <see cref="ChartAxis"/>.
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
      internal set { SetValue(OrientationPropertyKey, value); }
    }

    private static readonly DependencyPropertyKey OrientationPropertyKey =
        DependencyProperty.RegisterReadOnly("Orientation", typeof(Orientation), typeof(ChartAxis),
        new UIPropertyMetadata(Orientation.Vertical, OnOrientationChanged));

    /// <summary>
    /// Identifies the <see cref="Orientation"/> property.
    /// </summary>
    public static readonly DependencyProperty OrientationProperty =
        OrientationPropertyKey.DependencyProperty;

    private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ChartAxis)d).OnOrientationChanged();
    }

    private void OnOrientationChanged()
    {
      UpdateStartPadding();
      UpdateAxisSize();
      _labels = new ObservableCollection<ContentControl>();
      _majorTickMarks = new ObservableCollection<Border>();
      _minorTickMarks = new ObservableCollection<Border>();
    }

    #endregion // Orientation property

    #region Placement Property

    /// <summary>
    /// Gets or sets the placement of this <see cref="ChartAxis"/> against the chart canvas.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="PlacementProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public AxisPlacement Placement
    {
      get { return (AxisPlacement)GetValue(PlacementProperty); }
      set { SetValue(PlacementProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Placement"/> property.
    /// </summary>
    public static readonly DependencyProperty PlacementProperty =
      DependencyProperty.Register("Placement", typeof(AxisPlacement), typeof(ChartAxis),
      new FrameworkPropertyMetadata(AxisPlacement.Auto, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPlacementChanged));

    private static void OnPlacementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ChartAxis)d).OnPlacementChanged();
    }

    internal event EventHandler PlacementChanged;

    private void OnPlacementChanged()
    {
      _majorTickMarks = new ObservableCollection<Border>();
      _minorTickMarks = new ObservableCollection<Border>();
      _labels = new ObservableCollection<ContentControl>();
      EventHandler handler = PlacementChanged;
      if (handler != null)
      {
        handler(this, EventArgs.Empty);
      }
    }

    #endregion // Placement Property

    #region StackIdentifier Property

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
      DependencyProperty.Register("StackIdentifier", typeof(object), typeof(ChartAxis),
      new FrameworkPropertyMetadata(OnStackIdentifierChanged));

    private static void OnStackIdentifierChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ChartAxis)d).OnStackIdentifierChanged(e);
    }

    private void OnStackIdentifierChanged(DependencyPropertyChangedEventArgs e)
    {
    }

    #endregion // StackIdentifier Property

    #region LabelLayout property

    /// <summary>
    /// Gets or sets the <see cref="AxisLabelLayout"/> of the <see cref="ChartAxis"/>.
    /// This is a dependency property.
    /// </summary>
    public AxisLabelLayout LabelLayout
    {
      get { return (AxisLabelLayout)GetValue(LabelLayoutProperty); }
      set { SetValue(LabelLayoutProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="LabelLayout"/> property.
    /// </summary>
    public static readonly DependencyProperty LabelLayoutProperty =
      DependencyProperty.Register("LabelLayout", typeof(AxisLabelLayout), typeof(ChartAxis),
      new PropertyMetadata(AxisLabelLayout.Normal, new PropertyChangedCallback(OnLabelLayoutChanged)));

    private static void OnLabelLayoutChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ChartAxis)d).OnLabelLayoutChanged();
    }

    private void OnLabelLayoutChanged()
    {
      if (!_isSettingLabelLayoutInternal)
      {
        IsLabelLayoutAuto = false;
      }

      UpdateAxisBuffer();
      UpdateLogicalAxisSize();
      RenderAxis(true);
    }

    private bool _isSettingLabelLayoutInternal = false;

    internal void SetLabelLayoutInternal(AxisLabelLayout labelLayout)
    {
      _isSettingLabelLayoutInternal = true;
      LabelLayout = labelLayout;
      _isSettingLabelLayoutInternal = false;
    }

    // TODO: LabelLayout should be nullable. That should make all this is-auto logic easier.
    private bool _isLabelLayoutAuto = true;

    internal bool IsLabelLayoutAuto
    {
      get { return _isLabelLayoutAuto && BindingOperations.GetBinding(this, LabelLayoutProperty) == null; }
      private set
      {
        _isLabelLayoutAuto = value;
      }
    }

    #endregion // LabelLayout property

    #region TickLayout property

    /// <summary>
    /// Gets or sets the <see cref="AxisTickLayout"/> that specifies how the tick marks are arranged along the axis.
    /// This is a dependency property.
    /// </summary>
    public AxisTickLayout TickLayout
    {
      get { return (AxisTickLayout)GetValue(TickLayoutProperty); }
      set { SetValue(TickLayoutProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="TickLayout"/> property.
    /// </summary>
    public static readonly DependencyProperty TickLayoutProperty =
      DependencyProperty.Register("TickLayout", typeof(AxisTickLayout), typeof(ChartAxis),
      new PropertyMetadata(AxisTickLayout.Normal, new PropertyChangedCallback(OnTickLayoutChanged)));

    private static void OnTickLayoutChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ChartAxis)d).OnTickLayoutChanged();
    }

    private void OnTickLayoutChanged()
    {
      RenderAxis(true);
    }

    #endregion // TickLayout property

    #region Title property

    /// <summary>
    /// Gets or sets the title of the <see cref="ChartAxis"/>.
    /// This is a dependency property.
    /// </summary>
    public string Title
    {
      get { return (string)GetValue(TitleProperty); }
      set { SetValue(TitleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Title"/> property.
    /// </summary>
    public static readonly DependencyProperty TitleProperty =
      DependencyProperty.Register("Title", typeof(string), typeof(ChartAxis),
      new PropertyMetadata(new PropertyChangedCallback(OnTitleChanged)));

    private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ChartAxis)d).OnTitleChanged();
    }

    private void OnTitleChanged()
    {
      EventHandler handler = TitleChanged;
      if (handler != null)
      {
        handler(this, EventArgs.Empty);
      }
    }

    internal event EventHandler TitleChanged;

    #endregion // Title property

    #region TitleVisibility property

    /// <summary>
    /// Gets or sets the <see cref="Visibility"/> of the axis title.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="TitleVisibilityProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public Visibility TitleVisibility
    {
      get { return (Visibility)GetValue(TitleVisibilityProperty); }
      set { SetValue(TitleVisibilityProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="TitleVisibility"/> property.
    /// </summary>
    public static readonly DependencyProperty TitleVisibilityProperty =
      DependencyProperty.Register("TitleVisibility", typeof(Visibility), typeof(ChartAxis),
      new FrameworkPropertyMetadata(Visibility.Visible, OnTitleVisibilityChanged));

    private static void OnTitleVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ChartAxis)d).OnTitleVisibilityChanged();
    }

    private void OnTitleVisibilityChanged()
    {
    }

    #endregion // TitleVisibility property

    #region LabelLevelCount property

    /// <summary>
    /// Gets or sets the number of levels used for placing the axis labels.
    /// This is a dependency property.
    /// </summary>
    public int LabelLevelCount
    {
      get { return (int)GetValue(LabelLevelCountProperty); }
      set { SetValue(LabelLevelCountProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="LabelLevelCount"/> property.
    /// </summary>
    public static readonly DependencyProperty LabelLevelCountProperty =
      DependencyProperty.Register("LabelLevelCount", typeof(int), typeof(ChartAxis),
      new PropertyMetadata(1, new PropertyChangedCallback(OnLabelLevelCountChanged)));

    private static void OnLabelLevelCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ChartAxis)d).OnLabelLevelCountChanged();
    }

    private void OnLabelLevelCountChanged()
    {
      if (LabelLevelCount < 1)
      {
        LabelLevelCount = 1;
      }
      else
      {
        RenderAxis(true);
      }
    }

    #endregion // LabelLevelCount property

    #region LabelStep property

    /// <summary>
    /// Gets or sets a value that determines if any axis labels should not be rendered.
    /// A value of 1 will allow all labels to be rendered. A value of 2 means only every second label will be rendered and so on.
    /// A value of 0 will automatically calculate a suitable label step that tries to prevent labels from overlapping.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="LabelStepProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public int LabelStep
    {
      get { return (int)GetValue(LabelStepProperty); }
      set { SetValue(LabelStepProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="LabelStep"/> property.
    /// </summary>
    public static readonly DependencyProperty LabelStepProperty =
      DependencyProperty.Register("LabelStep", typeof(int), typeof(ChartAxis),
      new FrameworkPropertyMetadata(OnLabelStepChanged));

    private static void OnLabelStepChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ChartAxis)d).OnLabelStepChanged();
    }

    private void OnLabelStepChanged()
    {
      UpdateAxisLater();
    }

    private int GetLabelStep()
    {
      // TODO: this method seems to be called many times as the chart is being initialized.
      int labelStep = LabelStep;
      if (LabelStep <= 0)
      {
        // Automatic label step calculation:
        labelStep = 1;
        // TODO: This doesn't take LabelTemplate or font size into consideration.
        // How to improve this?
        if (LabelLevelCount == 1)
        {
          object labelContent = GetLabel(ActualMaximumValue);
          string labelString = GetLabelString(LabelFormat, labelContent);
          ContentControl label = BuildAxisLabel(labelContent, labelString, 0, 0, 0);
          ContentControl host = new ContentControl();
          host.FontSize = FontSize;
          host.FontWeight = FontWeight;
          host.Content = label;
          host.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
          double size = Orientation == Orientation.Horizontal ? label.DesiredSize.Width + 10 : label.DesiredSize.Height;
          //double size = Orientation == Orientation.Horizontal ? MeasurementUtils.GetTextWidth(labelString) + 10 : MeasurementUtils.GetTextHeight(labelString);

          double labelSpacing = ActualMajorTickMarkSpacing;

          IAdvancedAxisValueConverter valueConverter = ValueConverter as IAdvancedAxisValueConverter;
          if (valueConverter != null)
          {
            double nextTick = valueConverter.GetNextAxisPlotPosition(ActualMinimumValue, ActualMaximumValue, ActualMinimumValue);
            labelSpacing = nextTick - ActualMinimumValue;
          }

          double labelRatio = ActualWidth == 0 ? 0 : LogicalAxisSize / ActualWidth;
          if (Orientation == Orientation.Vertical)
          {
            labelRatio = ActualHeight == 0 ? 0 : LogicalAxisSize / ActualHeight;
          }

          double physicalLabelSpacing = labelRatio == 0 ? 0 : labelSpacing / labelRatio;

          labelStep = physicalLabelSpacing == 0 ? 1 : Math.Max(1, (int)Math.Ceiling(size / physicalLabelSpacing));
          if (labelStep == 1) // TODO: this doesn't look neccessary...
          {
            return labelStep;
          }
        }
      }
      return labelStep;
    }

    #endregion // LabelStep property

    #region ValueConverter property

    /// <summary>
    /// Gets or sets the <see cref="IAxisValueConverter"/> used for converting between axis values and objects.
    /// This is a dependency property.
    /// </summary>
    public IAxisValueConverter ValueConverter
    {
      get { return (IAxisValueConverter)GetValue(ValueConverterProperty); }
      set { SetValue(ValueConverterProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ValueConverter"/> property.
    /// </summary>
    public static readonly DependencyProperty ValueConverterProperty =
      DependencyProperty.Register("ValueConverter", typeof(IAxisValueConverter), typeof(ChartAxis),
      new PropertyMetadata(new PropertyChangedCallback(OnValueConverterChanged)));

    private static void OnValueConverterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ChartAxis)d).OnValueConverterChanged();
    }

    internal event EventHandler ValueConverterChanged;

    private void OnValueConverterChanged()
    {
      //OnPositionsChanged();
      _valueConverter = ValueConverter;
      if (ValueConverter != null)
      {
        _dataMap.Clear();
      }
      _coerceLock = true;
      if (!IsMaximumAuto)
      {
        OnMaximumChanged();
      }
      if (!IsMinimumAuto)
      {
        OnMinimumChanged();
      }
      _coerceLock = false;
      EventHandler handler = ValueConverterChanged;
      if (handler != null)
      {
        handler(this, new EventArgs());
      }
    }

    // Since the ValueConverter is requested so frequently, and Dependancy Properties are so slow,
    // we cache the ValueConverter in this field for quick accessability.
    private IAxisValueConverter _valueConverter;

    #endregion // ValueConverter property

    #region LabelFormat property

    /// <summary>
    /// Gets or sets the string format used to display the axis labels.
    /// This is a dependency property.
    /// </summary>
    public string LabelFormat
    {
      get { return (string)GetValue(LabelFormatProperty); }
      set { SetValue(LabelFormatProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="LabelFormat"/> property.
    /// </summary>
    public static readonly DependencyProperty LabelFormatProperty =
      DependencyProperty.Register("LabelFormat", typeof(string), typeof(ChartAxis),
      new PropertyMetadata("{0:0.###}", new PropertyChangedCallback(OnLabelFormatChanged)));

    private static void OnLabelFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ChartAxis)d).OnLabelFormatChanged();
    }

    private void OnLabelFormatChanged()
    {
      UpdateAxisLater();
    }

    #endregion // LabelFormat property

    #region AxisLineStyle property

    /// <summary>
    /// Gets or sets the <see cref="Style"/> applied to the axis line.
    /// This is a dependency property.
    /// </summary>
    public Style AxisLineStyle
    {
      get { return (Style)GetValue(AxisLineStyleProperty); }
      set { SetValue(AxisLineStyleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="AxisLineStyle"/> property.
    /// </summary>
    public static readonly DependencyProperty AxisLineStyleProperty =
      DependencyProperty.Register("AxisLineStyle", typeof(Style), typeof(ChartAxis),
      new PropertyMetadata(GetDefaultAxisLineStyle(), new PropertyChangedCallback(OnAxisLineStyleChanged)));

    private static void OnAxisLineStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ChartAxis)d).OnAxisLineStyleChanged();
    }

    private void OnAxisLineStyleChanged()
    {
    }

    private static Style GetDefaultAxisLineStyle()
    {
      Style style = new Style(typeof(Border));
      style.Setters.Add(new Setter(Border.BackgroundProperty, new SolidColorBrush(Colors.Black)));
      return style;
    }

    #endregion // AxisLineStyle property

    #region LabelTemplate property

    /// <summary>
    /// Gets or sets the <see cref="DataTemplate"/> applied to the axis labels.
    /// This is a dependency property.
    /// </summary>
    public DataTemplate LabelTemplate
    {
      get { return (DataTemplate)GetValue(LabelTemplateProperty); }
      set { SetValue(LabelTemplateProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="LabelTemplate"/> property.
    /// </summary>
    public static readonly DependencyProperty LabelTemplateProperty =
      DependencyProperty.Register("LabelTemplate", typeof(DataTemplate), typeof(ChartAxis),
      new PropertyMetadata(new PropertyChangedCallback(OnLabelTemplateChanged)));

    private static void OnLabelTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ChartAxis)d).OnLabelTemplateChanged();
    }

    private void OnLabelTemplateChanged()
    {
    }

    #endregion // LabelTemplate property

    #region TickMarkMode property

    /// <summary>
    /// Gets or sets how tick marks behave when the user pans a zoomed-in chart.
    /// This is a dependency property.
    /// </summary>
    public TickMarkMode TickMarkMode
    {
      get { return (TickMarkMode)GetValue(TickMarkModeProperty); }
      set { SetValue(TickMarkModeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="TickMarkMode"/> property.
    /// </summary>
    public static readonly DependencyProperty TickMarkModeProperty =
      DependencyProperty.Register("TickMarkMode", typeof(TickMarkMode), typeof(ChartAxis),
      new PropertyMetadata(TickMarkMode.Movable, new PropertyChangedCallback(OnTickMarkModeChanged)));

    private static void OnTickMarkModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ChartAxis)d).OnTickMarkModeChanged();
    }

    private void OnTickMarkModeChanged()
    {
      RenderAxis(true);
    }

    #endregion // TickMarkMode property

    #region LabelRotation property

    /// <summary>
    /// Gets or sets the rotation angle of the axis labels in degrees. Ideal values for this property
    /// are between 0 - 90 and 270 - 360. The default is 0 degrees.
    /// This feature is currently only supported by X axes.
    /// This is a dependency property.
    /// </summary>
    public double LabelRotation
    {
      get { return (double)GetValue(LabelRotationProperty); }
      set { SetValue(LabelRotationProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="LabelRotation"/> property.
    /// </summary>
    public static readonly DependencyProperty LabelRotationProperty =
      DependencyProperty.Register("LabelRotation", typeof(double), typeof(ChartAxis),
      new PropertyMetadata(new PropertyChangedCallback(OnLabelRotationChanged)));

    private static void OnLabelRotationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ChartAxis)d).OnLabelRotationChanged();
    }

    private void OnLabelRotationChanged()
    {
      RenderAxis(true);
    }

    #endregion // LabelRotation property

    #region MinorTickMarkStyle property

    /// <summary>
    /// Gets or sets the <see cref="Style"/> applied to the minor tick marks.
    /// Tick marks are <see cref="Border"/> objects.
    /// This is a dependency property.
    /// </summary>
    public Style MinorTickMarkStyle
    {
      get { return (Style)GetValue(MinorTickMarkStyleProperty); }
      set { SetValue(MinorTickMarkStyleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="MinorTickMarkStyle"/> property.
    /// </summary>
    public static readonly DependencyProperty MinorTickMarkStyleProperty =
      DependencyProperty.Register("MinorTickMarkStyle", typeof(Style), typeof(ChartAxis),
      new PropertyMetadata(GetDefaultTickMarkStyle(new SolidColorBrush(Colors.Transparent))));

    private static Style GetDefaultTickMarkStyle(Brush background)
    {
      Style style = new Style(typeof(Border));
      style.Setters.Add(new Setter(Border.BackgroundProperty, background));
      return style;
    }

    #endregion // MinorTickMarkStyle property

    #region MajorTickMarkStyle property

    /// <summary>
    /// Gets or sets the <see cref="Style"/> applied to major tick marks.
    /// Tick marks are <see cref="Border"/> objects.
    /// This is a dependency property.
    /// </summary>
    public Style MajorTickMarkStyle
    {
      get { return (Style)GetValue(MajorTickMarkStyleProperty); }
      set { SetValue(MajorTickMarkStyleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="MajorTickMarkStyle"/> property.
    /// </summary>
    public static readonly DependencyProperty MajorTickMarkStyleProperty =
      DependencyProperty.Register("MajorTickMarkStyle", typeof(Style), typeof(ChartAxis),
      new PropertyMetadata(GetDefaultTickMarkStyle(new SolidColorBrush(Colors.Black))));

    #endregion // MajorTickMarkStyle property

    #region MajorTickSpacing property

    /// <summary>
    /// Gets or sets the logical axis distance between the major tick marks. 
    /// This is a dependency property.
    /// </summary>
    /// <remarks>If this property is zero (the default), the logical tick mark
    /// spacing will be calculated automatically based on the actual minimum and maximum property values.
    /// When setting this property manually, make sure it is reasonable based on the 
    /// actual minimum and maximum property values so as not to create too many tick marks.</remarks>
    public double MajorTickSpacing
    {
      get { return (double)GetValue(MajorTickSpacingProperty); }
      set { SetValue(MajorTickSpacingProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="MajorTickSpacing"/> property.
    /// </summary>
    public static readonly DependencyProperty MajorTickSpacingProperty =
      DependencyProperty.Register("MajorTickSpacing", typeof(double), typeof(ChartAxis),
      new PropertyMetadata(0.0, new PropertyChangedCallback(OnMajorTickSpacingChanged)));

    private static void OnMajorTickSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ChartAxis)d).OnMajorTickSpacingChanged();
    }

    private void OnMajorTickSpacingChanged()
    {
      UpdateSpacing();
      UpdateAxisLater();
    }

    #endregion // MajorTickSpacing property

    #region TickMarkOffset Property

    /// <summary>
    /// Gets or sets the logical tick mark shift for controling how the tick marks align with the data.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="TickMarkOffsetProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public double TickMarkOffset
    {
      get { return (double)GetValue(TickMarkOffsetProperty); }
      set { SetValue(TickMarkOffsetProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="TickMarkOffset"/> property.
    /// </summary>
    public static readonly DependencyProperty TickMarkOffsetProperty =
      DependencyProperty.Register("TickMarkOffset", typeof(double), typeof(ChartAxis),
      new FrameworkPropertyMetadata(OnTickMarkOffsetChanged));

    private static void OnTickMarkOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ChartAxis)d).OnTickMarkOffsetChanged();
    }

    private void OnTickMarkOffsetChanged()
    {
      UpdateAxisLater();
    }

    #endregion // TickMarkOffset Property

    #region LabelOffset Property

    /// <summary>
    /// Gets or sets the logical axis label shift for controling how the labels align with the data.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="LabelOffsetProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public double LabelOffset
    {
      get { return (double)GetValue(LabelOffsetProperty); }
      set { SetValue(LabelOffsetProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="LabelOffset"/> property.
    /// </summary>
    public static readonly DependencyProperty LabelOffsetProperty =
      DependencyProperty.Register("LabelOffset", typeof(double), typeof(ChartAxis),
      new FrameworkPropertyMetadata(OnLabelOffsetChanged));

    private static void OnLabelOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ChartAxis)d).OnLabelOffsetChanged();
    }

    private void OnLabelOffsetChanged()
    {
      UpdateAxisLater();
    }

    #endregion // LabelOffset Property

    #region LabelSpacing Property

    /// <summary>
    /// Gets or sets the logical spacing between the axis labels. If this property value is zero (the default), the label spacing
    /// will be the same as the major tick spacing.
    /// This is a dependency property.
    /// </summary>
    public double LabelSpacing
    {
      get { return (double)GetValue(LabelSpacingProperty); }
      set { SetValue(LabelSpacingProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="LabelSpacing"/> property.
    /// </summary>
    public static readonly DependencyProperty LabelSpacingProperty =
      DependencyProperty.Register("LabelSpacing", typeof(double), typeof(ChartAxis),
      new FrameworkPropertyMetadata(0.0, OnLabelSpacingChanged));

    private static void OnLabelSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ChartAxis)d).OnLabelSpacingChanged();
    }

    private void OnLabelSpacingChanged()
    {
      UpdateAxisLater();
    }

    #endregion // LabelSpacing Property

    #region MinorTickMarkVisibility property

    /// <summary>
    /// Gets or sets the <see cref="Visibility"/> of the minor tick marks.
    /// This is a dependency property.
    /// </summary>
    public Visibility MinorTickMarkVisibility
    {
      get { return (Visibility)GetValue(MinorTickMarkVisibilityProperty); }
      set { SetValue(MinorTickMarkVisibilityProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="MinorTickMarkVisibility"/> property.
    /// </summary>
    public static readonly DependencyProperty MinorTickMarkVisibilityProperty =
      DependencyProperty.Register("MinorTickMarkVisibility", typeof(Visibility), typeof(ChartAxis),
      new PropertyMetadata(Visibility.Collapsed, new PropertyChangedCallback(OnMinorTickMarkVisibilityChanged)));

    private static void OnMinorTickMarkVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ChartAxis)d).OnMinorTickMarkVisibilityChanged();
    }

    private void OnMinorTickMarkVisibilityChanged()
    {
      // TODO: don't really need to re-render the whole chart. Should just change visibility of the minor tickmarks.
      UpdateAxisLater();
    }

    #endregion // MinorTickMarkVisibility property

    #region MinorTickMarkCount Property

    /// <summary>
    /// Gets or sets the number of minor tick marks to render between each major tick mark. The default is 4.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="MinorTickMarkCountProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public int MinorTickMarkCount
    {
      get { return (int)GetValue(MinorTickMarkCountProperty); }
      set { SetValue(MinorTickMarkCountProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="MinorTickMarkCount"/> property.
    /// </summary>
    public static readonly DependencyProperty MinorTickMarkCountProperty =
      DependencyProperty.Register("MinorTickMarkCount", typeof(int), typeof(ChartAxis),
      new FrameworkPropertyMetadata(4, OnMinorTickMarkCountChanged));

    private static void OnMinorTickMarkCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ChartAxis)d).OnMinorTickMarkCountChanged();
    }

    private void OnMinorTickMarkCountChanged()
    {
      UpdateAxisLater();
    }

    #endregion // MinorTickMarkCount Property

    #region IsSliderVisible Property

    /// <summary>
    /// Gets or sets whether or not to display the axis dual-slider.
    /// The slider is a UI control for zooming and panning the axis.
    /// The default is false.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsSliderVisibleProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsSliderVisible
    {
      get { return (bool)GetValue(IsSliderVisibleProperty); }
      set { SetValue(IsSliderVisibleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsSliderVisible"/> property.
    /// </summary>
    public static readonly DependencyProperty IsSliderVisibleProperty =
      DependencyProperty.Register("IsSliderVisible", typeof(bool), typeof(ChartAxis),
      new FrameworkPropertyMetadata(false, OnIsSliderVisibleChanged));

    private static void OnIsSliderVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ChartAxis)d).OnIsSliderVisibleChanged();
    }

    private void OnIsSliderVisibleChanged()
    {
    }

    #endregion // IsSliderVisible Property

    #region SliderStyle Property

    /// <summary>
    /// Gets or sets the <see cref="Style"/> applied to the axis dual-slider.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="SliderStyleProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public Style SliderStyle
    {
      get { return (Style)GetValue(SliderStyleProperty); }
      set { SetValue(SliderStyleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="SliderStyle"/> property.
    /// </summary>
    public static readonly DependencyProperty SliderStyleProperty =
      DependencyProperty.Register("SliderStyle", typeof(Style), typeof(ChartAxis),
      new FrameworkPropertyMetadata(OnSliderStyleChanged));

    private static void OnSliderStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ChartAxis)d).OnSliderStyleChanged();
    }

    private void OnSliderStyleChanged()
    {
    }

    #endregion // SliderStyle Property

    #region IsReversed Property

    private bool _isReversed = false; // Dependancy property value cache to speed things up.

    /// <summary>
    /// Gets or sets whether or not to reverse the axis.
    /// When set to true, the axis and chart data will be rendered from right to left for X axes, and top to bottom for Y axes. The default is false.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsReversedProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsReversed
    {
      get { return (bool)GetValue(IsReversedProperty); }
      set { SetValue(IsReversedProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsReversed"/> property.
    /// </summary>
    public static readonly DependencyProperty IsReversedProperty =
      DependencyProperty.Register("IsReversed", typeof(bool), typeof(ChartAxis),
      new FrameworkPropertyMetadata(OnIsReversedChanged));

    private static void OnIsReversedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ChartAxis)d).OnIsReversedChanged();
    }

    private void OnIsReversedChanged()
    {
      _isReversed = IsReversed;
      UpdateAxisLater();
    }

    #endregion // IsReversed Property

    #region IsTitleRotated Property

    /// <summary>
    /// Gets or sets whether or not the title is rotated. This only affects Y axes.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsTitleRotatedProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsTitleRotated
    {
      get { return (bool)GetValue(IsTitleRotatedProperty); }
      set { SetValue(IsTitleRotatedProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsTitleRotated"/> property.
    /// </summary>
    public static readonly DependencyProperty IsTitleRotatedProperty =
      DependencyProperty.Register("IsTitleRotated", typeof(bool), typeof(ChartAxis),
      new FrameworkPropertyMetadata(true, OnIsTitleRotatedChanged));

    private static void OnIsTitleRotatedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ChartAxis)d).OnIsTitleRotatedChanged();
    }

    private void OnIsTitleRotatedChanged()
    {
    }

    #endregion // IsTitleRotated Property

    #region TitleWrapping Property

    /// <summary>
    /// Gets or sets the text wrapping behavior of the axis title.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="TitleWrappingProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public TextWrapping TitleWrapping
    {
      get { return (TextWrapping)GetValue(TitleWrappingProperty); }
      set { SetValue(TitleWrappingProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="TitleWrapping"/> property.
    /// </summary>
    public static readonly DependencyProperty TitleWrappingProperty =
      DependencyProperty.Register("TitleWrapping", typeof(TextWrapping), typeof(ChartAxis),
      new FrameworkPropertyMetadata(TextWrapping.NoWrap, OnTitleWrappingChanged));

    private static void OnTitleWrappingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ChartAxis)d).OnTitleWrappingChanged();
    }

    private void OnTitleWrappingChanged()
    {
    }

    #endregion // TitleWrapping Property

    private Dictionary<double, object> _labelMap; // TODO: explore whether this needs to be a map or not. Aren't the 'double' values always whole numbers? maybe this can be a standard list of objects.
    
    // TODO: This should only be temporarily internal.
    // The problem is when using category axis support, changing the ItemsSource of a series uses the old mappings.
    // We are exposing this to clear the mapping when the ItemsSource changes, but in some rare scenarios we may not want to do this.
    // Also I don't like that this is exposed.
    // Maybe we can move the mapping responsibility to the data series rather than the axis?
    internal Dictionary<double, object> LabelMap
    {
      get { return _labelMap; }
      set
      {
        _labelMap = value;
        RenderAxis(true);
      }
    }

    private readonly Dictionary<object, double> _dataMap = new Dictionary<object, double>();

    // TODO: This should only be temporary.
    // The problem is when using category axis support, changing the ItemsSource of a series uses the old mappings.
    // We are exposing this to clear the mapping when the ItemsSource changes, but in some rare scenarios we may not want to do this.
    // Also I don't like that this is exposed.
    // Maybe we can move the mapping responsibility to the data series rather than the axis?
    internal Dictionary<object, double> DataMap
    {
      get { return _dataMap; }
    }

    /// <summary>
    /// Calculates the logical data position of the given object against the <see cref="ChartAxis"/>.
    /// </summary>
    /// <param name="o">The object to calculate the position of.</param>
    /// <returns>The logical position of the given object against the <see cref="ChartAxis"/>.</returns>
    public double GetLogicalPosition(object o)
    {
      double x = 0;
      
      if (_valueConverter != null)
      {
        x = _valueConverter.GetAxisPlotPosition(o);
      }
      else
      {
        double? temp = NumericalUtils.ConvertToDouble(o);
        if (temp != null)
        {
          x = temp.Value;
        }
        else if (o is double[]) // TODO: This relates to HeatmapSeries. Should this be in a default value converter?
        {
          x = ((double[])o).Length;
        }
        else if (o is float[])
        {
          x = ((float[])o).Length;
        }
        else if (o != null)
        {
          if (_dataMap.ContainsKey(o))
          {
            x = _dataMap[o];
          }
          else
          {
            x = _dataMap.Count;
            _dataMap[o] = x;
            if (LabelMap == null)
            {
              LabelMap = new Dictionary<double, object>();
              UpdateSpacing();
            }
            int count = LabelMap.Count;
            LabelMap[x] = o;
            if (LabelMap.Count != count)
            {
              UpdateAxisLater(); // TODO: is this really needed? is there really any scenario where the map is going to be updated without updating the axes?
            }
          }
        }
        else
        {
          x = Double.NaN;
        }
      }
      return x;
    }

    private object GetObject(double value)
    {
      object o = null;

      if (_valueConverter != null)
      {
        o = _valueConverter.GetDataObjectAt(value);
      }
      else
      {
        if (LabelMap != null && LabelMap.ContainsKey(value))
        {
          o = LabelMap[value];
        }
        else
        {
          o = value;
        }
      }
      return o;
    }

    #region Render Axis

    private bool _isUpdateAxisQueued;

    // TODO: this is temporarily internal
    internal void UpdateAxisLater()
    {
      if (!_isUpdateAxisQueued)
      {
        _isUpdateAxisQueued = true;
        /*Chart chart = VisualTreeUtils.FindAncestor<Chart>(this);
        if (chart != null && chart.IsBuildChartQueued)
        {
          chart.SkipFirstBuild = true;
        }*/
        Dispatcher.BeginInvoke(new Action<bool>(RenderAxis), false);
        OnAxisUpdated();
      }
    }

    private void RenderAxis(bool raiseEvent)
    {
      if (Double.IsNaN(ActualMinimumValue) || Double.IsNaN(ActualMaximumValue))
      {
        OnAxisUpdated();
        _isUpdateAxisQueued = false;
        return;
      }

      RenderTickMarks();
      RenderLabels();

      if (raiseEvent)
      {
        OnAxisUpdated();
      }
      OnAxisRendered();
      
      _isUpdateAxisQueued = false;
    }

    internal event EventHandler AxisRendered;

    private void OnAxisRendered()
    {
      EventHandler handler = AxisRendered;
      if (handler != null)
      {
        handler(this, EventArgs.Empty);
      }
    }

    internal event EventHandler AxisUpdated;

    private void OnAxisUpdated()
    {
      EventHandler handler = AxisUpdated;
      if (handler != null)
      {
        handler(this, EventArgs.Empty);
      }
    }

    #endregion // Render Axis

    // TODO: the tick mark and axis rendering methods share some common logic which may be able to be refactored out.

    #region Render Tick Marks

    private void RenderTickMarks()
    {
      IAdvancedAxisValueConverter axisValueConverter = ValueConverter as IAdvancedAxisValueConverter;

      double logicalSpacing = ActualMajorTickMarkSpacing;

      double logicalPosition = ActualMinimumValue;

      if (TickMarkMode == TickMarkMode.Movable)
      {
        if (axisValueConverter != null)
        {
          logicalPosition = axisValueConverter.NormalizeAxisPlotPosition(ActualMinimumValue, ActualMaximumValue, logicalPosition);
        }
        else
        {
          double movementOffset = logicalPosition % logicalSpacing;
          if (movementOffset < 0)
          {
            movementOffset += logicalSpacing;
          }
          logicalPosition -= movementOffset;
        }
      }

      double logicalOffset = 0;
      if (IsTickAndLabelLayoutDifferent)
      {
        if (axisValueConverter != null)
        {
          double previousAxisPlotPosition = axisValueConverter.GetPreviousAxisPlotPosition(ActualMinimumValue, ActualMaximumValue, logicalPosition);
          logicalOffset = (logicalPosition - previousAxisPlotPosition) / 2.0;
        }
        else
        {
          logicalOffset = logicalSpacing / 2.0;
        }
      }
      else if (TickLayout == AxisTickLayout.Inside && LabelLayout == AxisLabelLayout.Inside)
      {
        logicalOffset = logicalSpacing;
      }
      logicalPosition -= logicalOffset;

      logicalPosition += (TickMarkOffset % logicalSpacing);

      _logicalMajorTickMarkPositions = new List<double>();

      double previousPhysicalPosition = 0;

      double startPadding = StartPadding;

      int majorTickCount = 0;
      int minorTickCount = 0;
      int count = 0;

      if (Double.IsInfinity(logicalSpacing))
      {
        AddTickMark(majorTickCount, 0, 6, MajorTickMarkStyle, _majorTickMarks, Visibility.Visible);
        majorTickCount++;
        AddTickMark(majorTickCount, ActualHeight, 6, MajorTickMarkStyle, _majorTickMarks, Visibility.Visible);
        majorTickCount++;
      }
      else
      {
        while (logicalPosition <= ActualMaximumValue + logicalSpacing)
        {
          double physicalPosition = Math.Round(ConvertLogicalToPhysical(logicalPosition) - (AxisSize * StackIndex));

          if (physicalPosition.Equals(previousPhysicalPosition))
          {
            count++;
          }
          else
          {
            count = 0;
          }
          if (count > 5)
          {
            break;
          }

          previousPhysicalPosition = physicalPosition;

          if (physicalPosition >= 0 && physicalPosition <= AxisSize + startPadding)
          {
            _logicalMajorTickMarkPositions.Add(logicalPosition);
            AddTickMark(majorTickCount, physicalPosition, 6, MajorTickMarkStyle, _majorTickMarks, Visibility.Visible);
            majorTickCount++;
          }

          // Minor tick mark loop:
          for (int i = 1; i <= MinorTickMarkCount; i++)
          {
            double divisor = MinorTickMarkCount + 1.0;
            double nextMajorTick = logicalPosition + logicalSpacing;
            if (axisValueConverter != null)
            {
              nextMajorTick = axisValueConverter.GetNextAxisPlotPosition(ActualMinimumValue, ActualMaximumValue, logicalPosition);
            }
            double majorSpacing = nextMajorTick - logicalPosition;
            double logicalMinorTickPosition = logicalPosition + (majorSpacing / divisor * i);
            double physicalMinorTickPosition = Math.Round(ConvertLogicalToPhysical(logicalMinorTickPosition) - (AxisSize * StackIndex));
            if (physicalMinorTickPosition >= 0 && physicalMinorTickPosition <= AxisSize)
            {
              AddTickMark(minorTickCount, physicalMinorTickPosition, 3, MinorTickMarkStyle, _minorTickMarks, MinorTickMarkVisibility);
              minorTickCount++;
            }
          }

          if (axisValueConverter == null || ActualMinimumValue == ActualMaximumValue)
          {
            logicalPosition += logicalSpacing;
          }
          else
          {
            double nextLogicalPosition = axisValueConverter.GetNextAxisPlotPosition(ActualMinimumValue, ActualMaximumValue, logicalPosition);
            if (nextLogicalPosition == logicalPosition)
            {
              break;
            }
            logicalPosition = nextLogicalPosition;
          }
        }
      }

      while (_minorTickMarks.Count > minorTickCount)
      {
        _minorTickMarks.RemoveAt(_minorTickMarks.Count - 1);
      }
      while (_majorTickMarks.Count > majorTickCount)
      {
        _majorTickMarks.RemoveAt(_majorTickMarks.Count - 1);
      }
    }

    private IList<double> _logicalMajorTickMarkPositions = new List<double>();

    internal IList<double> LogicalMajorTickMarkPositions
    {
      get { return _logicalMajorTickMarkPositions; }
    }

    internal bool IsTickAndLabelLayoutDifferent
    {
      get
      {
        return (TickLayout == AxisTickLayout.Normal && LabelLayout == AxisLabelLayout.Inside) || (TickLayout == AxisTickLayout.Inside && LabelLayout == AxisLabelLayout.Normal);
      }
    }

    private void AddTickMark(int tickMarkCount, double physicalPosition, int size, Style style, ObservableCollection<Border> cache, Visibility visibility)
    {
      if (tickMarkCount < cache.Count)
      {
        UpdateTickMark(cache[tickMarkCount], physicalPosition, style);
        cache[tickMarkCount].Visibility = visibility;
      }
      else
      {
        Border tickMark = BuildTickMark(physicalPosition, size, style);
        tickMark.Visibility = visibility;
        cache.Add(tickMark);
      }
    }

    private void UpdateTickMark(Border tickMark, double position, Style style)
    {
      tickMark.Visibility = Visibility.Visible;
      tickMark.Style = style;
      if (Double.IsInfinity(position) || Double.IsNaN(position))
      {
        position = 0;
      }
      if (Orientation == Orientation.Horizontal)
      {
        TranslateTransform translation = tickMark.RenderTransform as TranslateTransform;
        translation.X = position;
      }
      else
      {
        TranslateTransform translation = tickMark.RenderTransform as TranslateTransform;
        translation.Y = -position;
      }
    }

    private Border BuildTickMark(double position, double size, Style style)
    {
      if (Double.IsInfinity(position) || Double.IsNaN(position))
      {
        position = 0;
      }
      Border tickMark = new Border();
      tickMark.Style = style;
      if (Orientation == Orientation.Horizontal)
      {
        tickMark.Width = 1;
        tickMark.HorizontalAlignment = HorizontalAlignment.Left;
        tickMark.VerticalAlignment = VerticalAlignment.Top;
        if (tickMark.Height == 0 || tickMark.Height.Equals(Double.NaN))
        {
          tickMark.Height = size;
        }
        tickMark.RenderTransform = new TranslateTransform() { X = position };
      }
      else
      {
        if (tickMark.Width == 0 || tickMark.Width.Equals(Double.NaN))
        {
          tickMark.Width = size;
        }
        tickMark.VerticalAlignment = VerticalAlignment.Bottom;
        tickMark.HorizontalAlignment = HorizontalAlignment.Right;
        tickMark.Height = 1;
        tickMark.RenderTransform = new TranslateTransform() { Y = -position };
      }
      return tickMark;
    }

    #endregion // Render Tick Marks

    #region Render Labels

    private void RenderLabels()
    {
      IAdvancedAxisValueConverter axisValueConverter = ValueConverter as IAdvancedAxisValueConverter;
      string labelFormat = LabelFormat;

      double logicalSpacing = LabelSpacing != 0 ? LabelSpacing : ActualMajorTickMarkSpacing;
      double logicalPosition = ActualMinimumValue;

      if (TickMarkMode == TickMarkMode.Movable)
      {
        if (axisValueConverter != null)
        {
          logicalPosition = axisValueConverter.NormalizeAxisPlotPosition(ActualMinimumValue, ActualMaximumValue, logicalPosition);
        }
        else
        {
          double movementOffset = logicalPosition % logicalSpacing;
          if (movementOffset < 0)
          {
            movementOffset += logicalSpacing;
          }
          logicalPosition -= movementOffset;
        }
      }

      logicalPosition += (LabelOffset % logicalSpacing);

      int labelLevel = 0;
      if (logicalSpacing != 0 && TickMarkMode == TickMarkMode.Movable)
      {
        double labelLvl = (ActualMinimumValue / logicalSpacing) % LabelLevelCount;
        if (Double.IsNaN(labelLvl))
        {
          labelLvl = 0;
        }
        labelLevel = Math.Abs((int)(labelLvl));
        labelLevel = Math.Max(labelLevel, 0);
        labelLevel = Math.Abs(labelLevel);
      }

      int labelStep = GetLabelStep();
      int labelStepIndex = 0;
      if (TickMarkMode == TickMarkMode.Movable)
      {
        if (logicalSpacing != 0)
        {
          double stepIndex = (ActualMinimumValue / logicalSpacing) % labelStep;
          if(Double.IsNaN(stepIndex))
          {
            stepIndex = 0;
          }
          labelStepIndex = Math.Abs((int)(stepIndex));
          labelStepIndex = Math.Max(labelStepIndex, 0);
          labelStepIndex = Math.Abs(labelStepIndex);
        }
        if (logicalPosition < 0)
        {
          labelStepIndex = (labelStepIndex + (labelStep - 1)) % labelStep;
        }
      }

      double previousPhysicalPosition = 0;
      double previousLabelRight = 0;

      double startPadding = StartPadding;

      int labelCount = 0;
      int count = 0;

      if (Double.IsInfinity(logicalSpacing))
      {
        AddAxisLabel(labelCount, 0, GetLabelString(labelFormat, 0), 0, 0, LabelRotation);
        labelCount++;
        AddAxisLabel(labelCount, logicalSpacing, GetLabelString(labelFormat, logicalSpacing), ActualHeight, 0, LabelRotation);
        labelCount++;
      }
      else
      {
        while (logicalPosition <= ActualMaximumValue + logicalSpacing)
        {
          double physicalPosition = Math.Round(ConvertLogicalToPhysical(logicalPosition) - (AxisSize * StackIndex));

          if (physicalPosition == previousPhysicalPosition)
          {
            count++;
          }
          else
          {
            count = 0;
          }
          if (count > 5)
          {
            break;
          }

          previousPhysicalPosition = physicalPosition;

          if (physicalPosition <= AxisSize + startPadding && physicalPosition >= -1 && (AxisSize > 0 || logicalPosition <= ActualMaximumValue) && labelStepIndex == 0)
          {
            object labelContent = GetLabel(logicalPosition);
            string labelString = GetLabelString(labelFormat, labelContent);

            double levelOffset = 0;
            if (Orientation == Orientation.Horizontal && LabelLevelCount > 1)
            {
              levelOffset = labelLevel * (CalculateHeight(labelString) + 1);
            }

            bool canfitLabel = true;
            if (Orientation == Orientation.Horizontal)
            {
              Chart chart = VisualTreeUtils.FindAncestor<Chart>(this);
              if (chart != null && chart.LegendPosition != LegendPosition.Right)
              {
                double labelWidth = CalculateWidth(labelString);
                if (physicalPosition + (labelWidth / 2.0) > AxisSize)
                {
                  physicalPosition = AxisSize - (labelWidth / 2.0) - 5.0;
                  if (physicalPosition - (labelWidth / 2.0) < previousLabelRight)
                  {
                    canfitLabel = false;
                  }
                }
                previousLabelRight = physicalPosition + (labelWidth / 2.0);
              }
            }

            if (canfitLabel)
            {
              AddAxisLabel(labelCount, labelContent, labelString, physicalPosition, levelOffset, LabelRotation);
              labelCount++;
            }
          }
          labelLevel = ++labelLevel % LabelLevelCount;
          labelStepIndex = ++labelStepIndex % labelStep;

          if (axisValueConverter == null)
          {
            logicalPosition += logicalSpacing;
          }
          else
          {
            double nextLogicalPosition = axisValueConverter.GetNextAxisPlotPosition(ActualMinimumValue, ActualMaximumValue, logicalPosition);
            if (nextLogicalPosition == logicalPosition)
            {
              break;
            }
            logicalPosition = nextLogicalPosition;
          }

          if (ActualMaximumValue - ActualMinimumValue == 0) // So that the labels don't overlap on an empty chart with default axes.
          {
            break;
          }
        }
      }

      if (labelCount == 0)
      {
        AddAxisLabel(labelCount, ActualMinimum, GetLabelString(labelFormat, ActualMinimum), 0, 0, LabelRotation);
        labelCount++;
      }

      while (_labels.Count > labelCount)
      {
        _labels.RemoveAt(_labels.Count - 1);
      }
    }

    private void AddAxisLabel(int labelCount, object labelContent, string formattedLabelContent, double physicalPosition, double levelOffset, double rotation)
    {
      if (labelCount < _labels.Count)
      {
        UpdateAxisLabel(_labels[labelCount], labelContent, formattedLabelContent, levelOffset, physicalPosition, rotation);
      }
      else
      {
        ContentControl label = BuildAxisLabel(labelContent, formattedLabelContent, levelOffset, physicalPosition, rotation);
        _labels.Add(label);
      }
    }

    private void UpdateAxisLabel(ContentControl label, object labelContent, string formattedLabelContent, double levelOffset, double positionOffset, double rotation)
    {
      label.Content = new AxisLabel(labelContent, formattedLabelContent);
      label.ContentTemplate = LabelTemplate;
      label.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
      if (Orientation == Orientation.Horizontal)
      {
        label.Margin = new Thickness(0, levelOffset, 0, 0);
        RotateTransform rotateTransform = label.LayoutTransform as RotateTransform;
        rotateTransform.Angle = rotation;
        TranslateTransform translation = label.RenderTransform as TranslateTransform;
        if (rotation != 0)
        {
          positionOffset = AdjustLabelPosition(label.DesiredSize.Width, label.DesiredSize.Height, positionOffset);
        }
        translation.X = positionOffset - (label.DesiredSize.Width / 2.0);
      }
      else
      {
        TranslateTransform translation = label.RenderTransform as TranslateTransform;
        translation.Y = -positionOffset + (label.DesiredSize.Height / 2.0);
      }
    }

    private ContentControl BuildAxisLabel(object labelContent, string formattedLabelContent, double levelOffset, double positionOffset, double rotation)
    {
      ContentControl label = new ContentControl();
      label.ContentTemplate = LabelTemplate;
      label.RenderTransformOrigin = new Point(0.5, 0.5);
      //label.Foreground = Foreground; // TODO: is this needed? Could this be messing with the label template foreground?
      label.Content = new AxisLabel(labelContent, formattedLabelContent);
      label.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
      if (Orientation == Orientation.Horizontal)
      {
        label.Margin = new Thickness(0, levelOffset, 0, 0);
        if (rotation != 0)
        {
          positionOffset = AdjustLabelPosition(label.DesiredSize.Width, label.DesiredSize.Height, positionOffset);
        }
        TranslateTransform translation = new TranslateTransform() { X = positionOffset - (label.DesiredSize.Width / 2.0) };
        RotateTransform rotateTransform = new RotateTransform() { Angle = rotation };
        label.LayoutTransform = rotateTransform;
        label.RenderTransform = translation;
        label.HorizontalAlignment = HorizontalAlignment.Left;
        label.VerticalAlignment = VerticalAlignment.Top;
      }
      else
      {
        TranslateTransform translation = new TranslateTransform() { Y = -positionOffset + (label.DesiredSize.Height / 2.0) };
        RotateTransform rotateTransform = new RotateTransform();
        label.LayoutTransform = rotateTransform;
        label.RenderTransform = translation;
        label.VerticalAlignment = VerticalAlignment.Bottom;
        label.HorizontalAlignment = HorizontalAlignment.Center;
      }
      return label;
    }

    // returns the ideal label position based on the label rotation.
    private double AdjustLabelPosition(double width, double height, double currentPosition)
    {
      // Modifying the angle so we only need to work in 1 trigonometry quadrent:
      double angle = LabelRotation;
      if (angle > 270)
      {
        angle = 360 - angle;
      }
      else if (angle > 180)
      {
        angle -= 180;
      }
      else if (angle > 90)
      {
        angle = 180 - angle;
      }
      // Convert the angle to radians:
      double radians = angle / (180 / Math.PI);
      // Calculate rotated height:
      double widthWidth = Math.Cos(radians) * width;
      double heightWidth = Math.Sin(radians) * height;
      double rotatedWidth = Math.Max(0, widthWidth + heightWidth);
      if ((LabelRotation > 0 && LabelRotation < 90) || (LabelRotation > 180 && LabelRotation < 270))
      {
        currentPosition += (rotatedWidth / 2);
        currentPosition -= (heightWidth / 2);
      }
      else if ((LabelRotation > 90 && LabelRotation < 180) || (LabelRotation > 270 && LabelRotation < 360))
      {
        currentPosition -= (rotatedWidth / 2);
        currentPosition += (heightWidth / 2);
      }
      return currentPosition;
    }
    
    #endregion // Render Labels

    internal object GetLabel(double labelPosition)
    {
      object labelContent = labelPosition;
      if (ValueConverter != null)
      {
        labelContent = ValueConverter.GetDataObjectAt(labelPosition);
      }
      else if (LabelMap != null && LabelMap.Count > 0)
      {
        LabelMap.TryGetValue((int)labelPosition, out labelContent);
      }
      if (labelContent == null)
      {
        labelContent = labelPosition;
      }
      return labelContent;
    }

    private string GetLabelString(string labelFormat, object labelObject)
    {
      string labelString = null;
      if (labelFormat == null || "{0:0.###}".Equals(labelFormat))
      {
        IAdvancedAxisValueConverter converter = ValueConverter as IAdvancedAxisValueConverter;
        if (converter != null)
        {
          string format = converter.GetFormat(ActualMinimumValue, ActualMaximumValue);
          if (format != null)
          {
            try
            {
              labelString = String.Format(format, labelObject);
            }
            catch (FormatException)
            {
              labelString = labelObject.ToString();
            }
          }
        }
      }

      if (labelString == null && labelFormat != null)
      {
        try
        {
          labelString = String.Format(labelFormat, labelObject);
        }
        catch (FormatException)
        {
          labelString = labelObject.ToString();
        }
      }

      if (labelString == null)
      {
        labelString = labelObject.ToString();
      }

      return labelString;
    }

    internal double ActualMajorTickMarkSpacing
    {
      get { return _spacing; }
    }

    internal void UpdateSpacing()
    {
      if (MajorTickSpacing != 0 && ((ActualMaximumValue == MaximumValue && ActualMinimumValue == MinimumValue) || AllowMajorTickSpacingZoomAdjustment == false))
      {
        _spacing = MajorTickSpacing;
      }
      else
      {
        double divisor = 7;
        if (MajorTickSpacing != 0 && !Double.IsNaN(MaximumValue) && !Double.IsNaN(MinimumValue))
        {
          divisor = (MaximumValue - MinimumValue) / MajorTickSpacing;
        }
        double range = ActualMaximumValue - ActualMinimumValue;
        double spacing = divisor == 0 ? 1 : range / divisor;
        if (range != 0)
        {
          spacing = NumericalUtils.CalculateTickMarkSpacing(spacing);
        }
        double minDelta = Double.IsNaN(MinDelta) ? 1 : MinDelta;
        _spacing = Math.Max(spacing == 0 ? 1 : spacing, MajorTickSpacing == 0 ? minDelta : 0);
        if (_dataMap != null && _dataMap.Count > 0)
        {
          _spacing = Math.Max(1, _spacing);
        }
      }
    }

    /*private double CalculateMinDelta()
    {
      Chart chart = VisualTreeUtils.FindAncestor<Chart>(this);
      double minDelta = 0;
      if (chart != null)
      {
        foreach (DataSeries series in chart.Series)
        {
          if (Orientation == Orientation.Horizontal || series.ReverseAxes)
          {
            minDelta = Math.Max(minDelta, series.MinDelta * Math.Max(1.0, series.GetIndexStep()));
          }
        }
      }
      return minDelta;
    }*/

    #region Physical and Logical Conversions

    internal int StackIndex { get; set; }

    // TODO: We'll probably want to make some of these methods public at some stage.

    /// <summary>
    /// Calculates the physical position of the given data value against the <see cref="ChartAxis"/>.
    /// </summary>
    /// <param name="logicalPosition">The logical value to convert</param>
    /// <returns>The physical position of the given data value against the <see cref="ChartAxis"/>.</returns>
    internal double ConvertLogicalToPhysical(double logicalPosition)
    {
      // TODO: should this method reverse the position if the axis if vertical? At the moment this will return the distance from the bottom of the axis.
      //double labelSpacing = MinDelta;
      //double labelRatio = LogicalAxisSize / AxisSize;
      //double physicalLabelSpacing = labelSpacing / labelRatio;
      //double buffer = GetLabelBuffer(labelSpacing);
      if (Double.IsPositiveInfinity(logicalPosition))
      {
        return AxisSize;
      }
      if (Double.IsNegativeInfinity(logicalPosition))
      {
        return 0;
      }
      double padding = StartPadding;
      double result = _conversionRatio == 0 ? 0 : ((logicalPosition - ActualMinimumValue + _axisBuffer) / _conversionRatio) + padding + (AxisSize * StackIndex);
      if (_isReversed)
      {
        result = AxisSize - result + (padding * 2);
      }
      return result;
    }

    private double _startPadding;

    private double StartPadding
    {
      get { return _startPadding; }
    }

    private void UpdateStartPadding()
    {
      _startPadding = 0;
      if (Padding != null)
      {
        _startPadding = Orientation == Orientation.Horizontal ? Padding.Left : Padding.Bottom;
      }
    }

    private double EndPadding
    {
      get
      {
        double padding = 0;
        if (Padding != null)
        {
          padding = Orientation == Orientation.Horizontal ? Padding.Right : Padding.Top;
        }
        return padding;
      }
    }

    /// <summary>
    /// Calculates the logical position of the given physical value against the <see cref="ChartAxis"/>.
    /// </summary>
    /// <param name="physicalPosition">The physical value to convert.</param>
    /// <returns>The logical position of the given physical value against the <see cref="ChartAxis"/>.</returns>
    internal double ConvertPhysicalToLogical(double physicalPosition)
    {
      double padding = StartPadding;
      if (Orientation == Orientation.Vertical)
      {
        padding = EndPadding;
        physicalPosition = AxisSize + padding - physicalPosition;
      }
      else
      {
        physicalPosition -= padding;
      }
      if (IsReversed)
      {
        physicalPosition = AxisSize - physicalPosition;
      }
      double labelSpacing = MinDelta; // GetSpacing();
      double labelRatio = LogicalAxisSize / AxisSize;
      double physicalLabelSpacing = labelSpacing / labelRatio;
      double buffer = GetLabelBuffer(physicalLabelSpacing);
      double result = ((physicalPosition - buffer) / (AxisSize - buffer * 2) * (ActualMaximumValue - ActualMinimumValue)) + ActualMinimumValue;
      return Double.IsNaN(result) ? ActualMinimumValue : result;
    }

    /// <summary>
    /// Converts the given logical size to a physical size.
    /// </summary>
    /// <param name="logicalSize">The logical size to convert.</param>
    /// <returns>The physical interpretation of the given logical size.</returns>
    internal double ConvertLogicalToPhysicalSize(double logicalSize)
    {
      if (AxisSize.Equals(Double.NaN) || AxisSize == 0)
      {
        return 0;
      }
      double ratio = LogicalAxisSize / AxisSize;
      return ratio == 0 ? 0 : logicalSize / ratio;
    }

    /// <summary>
    /// Converts the given physical size to a logical size.
    /// </summary>
    /// <param name="physicalSize">The physical size to convert.</param>
    /// <returns>The logical interpretation of the given physical size.</returns>
    internal double ConvertPhysicalToLogicalSize(double physicalSize)
    {
      if (AxisSize.Equals(Double.NaN) || AxisSize == 0)
      {
        return 0;
      }
      double ratio = LogicalAxisSize / AxisSize;
      return physicalSize * ratio;
    }

    private double _logicalAxisSize = 0.0;

    private double LogicalAxisSize
    {
      get
      {
        return _logicalAxisSize;
      }
    }

    private void UpdateLogicalAxisSize()
    {
      _logicalAxisSize = ActualMaximumValue - ActualMinimumValue;
      if (LabelLayout == AxisLabelLayout.Inside)
      {
        _logicalAxisSize += MinDelta;
      }
      UpdateConversionRatio();
    }

    private double _conversionRatio;

    internal double ConversionRatio { get { return _conversionRatio; } }

    private void UpdateConversionRatio()
    {
      _conversionRatio = AxisSize == 0 || LogicalAxisSize == 0 ? 0 : LogicalAxisSize / AxisSize;
    }

    private double _minDelta = 1.0;

    internal double MinDelta
    {
      get { return _minDelta; }
      set
      {
        if (_minDelta != value)
        {
          _minDelta = value;
          UpdateAxisBuffer();
          UpdateLogicalAxisSize();
          UpdateSpacing();
          RenderAxis(false);
        }
      }
    }

    /*private void UpdateMinDelta()
    {
      Chart chart = VisualTreeUtils.FindAncestor<Chart>(this);
      if (chart != null)
      {
        _minDelta = Orientation == Orientation.Horizontal ? chart.HorizontalMinDelta : chart.VerticalMinDelta;
      }
      else
      {
        _minDelta = 0;
      }
    }*/

    private double _axisBuffer = 0.0; // half the MinDelta when LabelLayout is Inside.

    private void UpdateAxisBuffer()
    {
      if (LabelLayout == AxisLabelLayout.Inside)
      {
        _axisBuffer = MinDelta / 2.0;
      }
      else
      {
        _axisBuffer = 0.0;
      }
    }

    private double GetLabelBuffer(double physicalLabelSpacing)
    {
      double buffer = 0;
      if (LabelLayout == AxisLabelLayout.Inside)
      {
        buffer = physicalLabelSpacing / 2.0;
      }
      return buffer;
    }

    private Canvas _chartCanvas;

    internal Canvas ChartCanvas
    {
      get { return _chartCanvas; }
      set
      {
        _chartCanvas = value;
        UpdateAxisSize();
      }
    }

    private double _axisSize;

    // Gets the pixel size of this axis. A horizontal axis returns the width. A vertical axis returns the height.
    internal double AxisSize
    {
      get { return _axisSize; }
    }

    private void UpdateAxisSize()
    {
      double padding = 0;
      if (Padding != null)
      {
        padding = Orientation == Orientation.Horizontal ? Padding.Left + Padding.Right : Padding.Top + Padding.Bottom;
      }
      if (ChartCanvas != null && StackIdentifier == null)
      {
        _axisSize = Math.Max(0, (Orientation == Orientation.Horizontal ? ChartCanvas.ActualWidth : ChartCanvas.ActualHeight) - 1 - padding);
      }
      else
      {
        _axisSize = Math.Max(0, (Orientation == Orientation.Horizontal ? ActualWidth : ActualHeight) - 1 - padding);
      }
      UpdateConversionRatio();
    }

    #endregion // Physical and Logical Conversions

    #region Basic Properties

    /// <summary>
    /// Gets a collection of major tick marks.
    /// </summary>
    public ReadOnlyCollection<Border> MajorTickMarks
    {
      get { return new ReadOnlyCollection<Border>(_majorTickMarks); }
    }

    /// <summary>
    /// Gets a collection of minor tick marks.
    /// </summary>
    public ReadOnlyCollection<Border> MinorTickMarks
    {
      get { return new ReadOnlyCollection<Border>(_minorTickMarks); }
    }

    /// <summary>
    /// Gets the collection of the generated axis labels to place along the <see cref="ChartAxis"/>.
    /// </summary>
    public ReadOnlyCollection<AxisLabel> Labels
    {
      get
      {
        List<AxisLabel> labels = new List<AxisLabel>();
        foreach (ContentControl label in _labels)
        {
          labels.Add(label.Content as AxisLabel);
        }
        return new ReadOnlyCollection<AxisLabel>(labels);
      }
    }

    /// <summary>
    /// Gets or sets whether or not the major tick spacing is adjusted while zooming.
    /// When set to true, the specified MajorTickSpacing value determines the number of tick marks at maximum zoom.
    /// While zooming, the density of ticks marks will be kept as constant as possible.
    /// When set to false, the specified MajorTickSpacing will be used at all zoom levels.
    /// The default is true.
    /// </summary>
    public bool AllowMajorTickSpacingZoomAdjustment
    {
      get { return _allowMajorTickSpacingZoomAdjustment; }
      set { _allowMajorTickSpacingZoomAdjustment = value; }
    }

    #endregion // Basic Properties

    private double CalculateWidth(string s)
    {
      StackPanel panel = new StackPanel();
      panel.Orientation = Orientation.Horizontal;

      TextBlock block = new TextBlock();
      block.FontSize = FontSize;
      block.Text = s;
      panel.Children.Add(block);
      panel.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
      return panel.DesiredSize.Width;
    }

    private double CalculateHeight(string s)
    {
      TextBlock block = new TextBlock();
      block.FontSize = FontSize;
      block.Text = s;
      StackPanel panel = new StackPanel();
      panel.Children.Add(block);
      panel.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
      return panel.DesiredSize.Height;
    }
  }
}
