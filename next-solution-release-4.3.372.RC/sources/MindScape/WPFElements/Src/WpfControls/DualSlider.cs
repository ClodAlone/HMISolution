using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using Infralution.Licensing;
using System.Windows.Threading;
using System.Windows.Media;
using System.Reflection;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// A control that supports selecting two values, such as the start and end of a range, using 
  /// a slider with two thumbs.
  /// </summary>
  [TemplatePart(Name = LargeDecreaseButtonPartName, Type = typeof(RepeatButton))]
  [TemplatePart(Name = LargeIncreaseButtonPartName, Type = typeof(RepeatButton))]
  [TemplatePart(Name = StartThumbPartName, Type = typeof(Thumb))]
  [TemplatePart(Name = EndThumbPartName, Type = typeof(Thumb))]
  [TemplatePart(Name = RangeThumbPartName, Type = typeof(Thumb))]
  [LicenseProvider(typeof(PublicEncryptedLicenseProvider))]
  public class DualSlider : SliderBase
  {
    private const string LargeDecreaseButtonPartName = "PART_LargeDecreaseButton";
    private const string LargeIncreaseButtonPartName = "PART_LargeIncreaseButton";
    private const string StartThumbPartName = "PART_StartThumb";
    private const string EndThumbPartName = "PART_EndThumb";
    private const string RangeThumbPartName = "PART_RangeThumb";

    private Thumb _startThumb, _rangeThumb, _endThumb;
    private RepeatButton _largeDecreaseButton, _largeIncreaseButton;
    private Point _mouseOverDecreasePoint, _mouseOverIncreasePoint;
    private bool _ignoreValidation;
    private bool _loading;

    private bool _rangeEventLock = false;

    static DualSlider()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(DualSlider),
        new FrameworkPropertyMetadata(typeof(DualSlider)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DualSlider"/> control.
    /// </summary>
    public DualSlider()
    {
      // Licensing
      //new WpfElementsCore(Assembly.GetCallingAssembly());
      //LicenseHelper.Attach(this, Assembly.GetCallingAssembly());
      // End licensing

      _loading = true;

      Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(SetDefaultRangeRestrictions));
      MouseWheel += new MouseWheelEventHandler(DualSlider_MouseWheel);
      KeyDown += new KeyEventHandler(DualSlider_KeyDown);
      SizeChanged += new SizeChangedEventHandler(DualSlider_SizeChanged);
      Loaded += new RoutedEventHandler(DualSlider_Loaded);
    }

    private void DualSlider_Loaded(object sender, RoutedEventArgs e)
    {
      _rangeEventLock = true;
      double oldRangeStart = RangeStart;
      double oldRangeEnd = RangeEnd;
      SetRangeEnd(RangeEnd);
      SetRangeStart(RangeStart);
      OnRangeChanged(oldRangeStart, oldRangeEnd);
      _rangeEventLock = false;
      _loading = false;
    }

    private void DualSlider_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      UpdateControlPositions();
      UpdateTickMarks();
    }

    private void DualSlider_KeyDown(object sender, KeyEventArgs e)
    {
      double change = SmallChange;
      if (SnapToTickMarks)
      {
        change = Math.Max(TickSpacing, SmallChange);
      }
      if (e.Key == Key.Right || e.Key == Key.Up)
      {
        MoveSliderThumbs(change);
      }
      else if (e.Key == Key.Left || e.Key == Key.Down)
      {
        MoveSliderThumbs(-change);
      }
    }

    private void DualSlider_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
    {
      if (IsMouseWheelEnabled)
      {
        double change = SmallChange;
        if (SnapToTickMarks)
        {
          change = Math.Max(TickSpacing, SmallChange);
        }
        if (e.Delta > 0)
        {
          MoveSliderThumbs(change);
        }
        else if (e.Delta < 0)
        {
          MoveSliderThumbs(-change);
        }
      }
    }

    private void MoveSliderThumbs(double delta)
    {
      _rangeEventLock = true;
      _ignoreValidation = true;
      double oldRangeStart = RangeStart;
      double oldRangeEnd = RangeEnd;
      double range = Math.Abs(Range);
      if (delta > 0)
      {
        if (RangeStart <= RangeEnd)
        {
          RangeEnd = Math.Min(Maximum, SnapToTickMarks ? GetClosestTickMark(RangeEnd + delta) : RangeEnd + delta);
          SetRangeStart(RangeEnd - range);
        }
        else
        {
          RangeStart = Math.Min(Maximum, SnapToTickMarks ? GetClosestTickMark(RangeStart + delta) : RangeStart + delta);
          SetRangeEnd(RangeStart - range);
        }
      }
      else if (delta < 0)
      {
        if (RangeStart <= RangeEnd)
        {
          RangeStart = Math.Max(Minimum, SnapToTickMarks ? GetClosestTickMark(RangeStart + delta) : RangeStart + delta);
          SetRangeEnd(RangeStart + range);
        }
        else
        {
          RangeEnd = Math.Max(Minimum, SnapToTickMarks ? GetClosestTickMark(RangeEnd + delta) : RangeEnd + delta);
          SetRangeStart(RangeEnd + range);
        }
      }
      OnRangeChanged(oldRangeStart, oldRangeEnd);
      _ignoreValidation = false;
      _rangeEventLock = false;
    }

    private void SetDefaultRangeRestrictions()
    {
      if (MinimumRange == 0 && MaximumRange == 0)
      {
        MinimumRange = Minimum - Maximum;
        MaximumRange = Maximum - Minimum;
      }
    }

    /// <summary>
    /// Called by the framework when the control template is applied.
    /// </summary>
    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      RemoveEventHandlers();

      _largeDecreaseButton = GetTemplateChild(LargeDecreaseButtonPartName) as RepeatButton;
      _startThumb = GetTemplateChild(StartThumbPartName) as Thumb;
      _rangeThumb = GetTemplateChild(RangeThumbPartName) as Thumb;
      _endThumb = GetTemplateChild(EndThumbPartName) as Thumb;
      _largeIncreaseButton = GetTemplateChild(LargeIncreaseButtonPartName) as RepeatButton;

      AttachEventHandlers();

      UpdateControlPositions();
      UpdateTickMarks();
    }

    private void RemoveEventHandlers()
    {
      if (_largeDecreaseButton != null)
      {
        _largeDecreaseButton.Click -= new RoutedEventHandler(LargeDecreaseButton_Click);
        _largeDecreaseButton.MouseEnter -= new MouseEventHandler(LargeDecreaseButton_MouseMove);
        _largeDecreaseButton.MouseMove -= new MouseEventHandler(LargeDecreaseButton_MouseMove);
      }
      if (_startThumb != null)
      {
        if (IsReversed)
        {
          _startThumb.DragDelta -= new DragDeltaEventHandler(EndThumb_DragDelta);
        }
        else
        {
          _startThumb.DragDelta -= new DragDeltaEventHandler(StartThumb_DragDelta);
        }
        _startThumb.DragCompleted -= new DragCompletedEventHandler(Thumb_DragCompleted);
      }
      if (_rangeThumb != null)
      {
        _rangeThumb.DragDelta -= new DragDeltaEventHandler(RangeThumb_DragDelta);
        _rangeThumb.DragCompleted -= new DragCompletedEventHandler(Thumb_DragCompleted);
      }
      if (_endThumb != null)
      {
        if (IsReversed)
        {
          _endThumb.DragDelta -= new DragDeltaEventHandler(StartThumb_DragDelta);
        }
        else
        {
          _endThumb.DragDelta -= new DragDeltaEventHandler(EndThumb_DragDelta);
        }
        _endThumb.DragCompleted -= new DragCompletedEventHandler(Thumb_DragCompleted);
      }
      if (_largeIncreaseButton != null)
      {
        _largeIncreaseButton.Click -= new RoutedEventHandler(LargeIncreaseButton_Click);
        _largeIncreaseButton.MouseEnter -= new MouseEventHandler(LargeIncreaseButton_MouseMove);
        _largeIncreaseButton.MouseMove -= new MouseEventHandler(LargeIncreaseButton_MouseMove);
      }
    }

    private void AttachEventHandlers()
    {
      if (_largeDecreaseButton != null)
      {
        _largeDecreaseButton.Click += new RoutedEventHandler(LargeDecreaseButton_Click);
        _largeDecreaseButton.MouseEnter += new MouseEventHandler(LargeDecreaseButton_MouseMove);
        _largeDecreaseButton.MouseMove += new MouseEventHandler(LargeDecreaseButton_MouseMove);
      }
      if (_startThumb != null)
      {
        if (IsReversed)
        {
          _startThumb.DragDelta += new DragDeltaEventHandler(EndThumb_DragDelta);
        }
        else
        {
          _startThumb.DragDelta += new DragDeltaEventHandler(StartThumb_DragDelta);
        }
        _startThumb.DragCompleted += new DragCompletedEventHandler(Thumb_DragCompleted);
      }
      if (_rangeThumb != null)
      {
        _rangeThumb.DragDelta += new DragDeltaEventHandler(RangeThumb_DragDelta);
        _rangeThumb.DragCompleted += new DragCompletedEventHandler(Thumb_DragCompleted);
      }
      if (_endThumb != null)
      {
        if (IsReversed)
        {
          _endThumb.DragDelta += new DragDeltaEventHandler(StartThumb_DragDelta);
        }
        else
        {
          _endThumb.DragDelta += new DragDeltaEventHandler(EndThumb_DragDelta);
        }
        _endThumb.DragCompleted += new DragCompletedEventHandler(Thumb_DragCompleted);
      }
      if (_largeIncreaseButton != null)
      {
        _largeIncreaseButton.Click += new RoutedEventHandler(LargeIncreaseButton_Click);
        _largeIncreaseButton.MouseEnter += new MouseEventHandler(LargeIncreaseButton_MouseMove);
        _largeIncreaseButton.MouseMove += new MouseEventHandler(LargeIncreaseButton_MouseMove);
      }
    }

    private void Thumb_DragCompleted(object sender, DragCompletedEventArgs e)
    {
      double oldRangeStart = RangeStart - (Orientation == Orientation.Horizontal ? e.HorizontalChange : e.VerticalChange);
      double oldRangeEnd = RangeEnd - (Orientation == Orientation.Horizontal ? e.HorizontalChange : e.VerticalChange);
      // TODO: should really only call this if the range changes.
      OnFinishedRangeUpdate(oldRangeStart, oldRangeEnd);
    }

    /// <summary>
    /// Raised when the user releases the mouse to finish a range update.
    /// </summary>
    public event EventHandler<RangeChangedEventArgs> FinishedRangeUpdate;

    private void OnFinishedRangeUpdate(double oldRangeStart, double oldRangeEnd)
    {
      EventHandler<RangeChangedEventArgs> handler = FinishedRangeUpdate;
      if (handler != null)
      {
        handler(this, new RangeChangedEventArgs(oldRangeStart, oldRangeEnd));
      }
    }

    private void LargeDecreaseButton_MouseMove(object sender, MouseEventArgs e)
    {
      if (RangeStart <= RangeEnd)
      {
        _mouseOverDecreasePoint = e.GetPosition(_startThumb);
      }
      else
      {
        _mouseOverDecreasePoint = e.GetPosition(_endThumb);
      }
    }

    private void LargeIncreaseButton_MouseMove(object sender, MouseEventArgs e)
    {
      if (RangeStart <= RangeEnd)
      {
        _mouseOverIncreasePoint = e.GetPosition(_endThumb);
      }
      else
      {
        _mouseOverIncreasePoint = e.GetPosition(_startThumb);
      }
    }

    private void LargeIncreaseButton_Click(object sender, RoutedEventArgs e)
    {
      _rangeEventLock = true;
      double oldRangeStart = RangeStart;
      double oldRangeEnd = RangeEnd;
      if (IsInstantMoveEnabled)
      {
        double logicalDelta = ConvertPhysicalToLogical(Orientation == Orientation.Horizontal ? _mouseOverIncreasePoint.X : -_mouseOverIncreasePoint.Y);

        if (RangeStart <= RangeEnd)
        {
          SetRangeEnd(RangeEnd + logicalDelta);
        }
        else
        {
          SetRangeStart(RangeStart + logicalDelta);
        }
        _mouseOverIncreasePoint = new Point(0, 0);
        _mouseOverDecreasePoint = new Point(0, 0);
      }
      else
      {
        double change = LargeChange;
        if (SnapToTickMarks)
        {
          change = Math.Max(TickSpacing, LargeChange);
        }
        if (RangeStart > RangeEnd)
        {
          SetRangeStart(RangeStart + change);
        }
        else
        {
          SetRangeEnd(RangeEnd + change);
        }
      }
      OnRangeChanged(oldRangeStart, oldRangeEnd);
      _rangeEventLock = false;
    }

    private void LargeDecreaseButton_Click(object sender, RoutedEventArgs e)
    {
      _rangeEventLock = true;
      double oldRangeStart = RangeStart;
      double oldRangeEnd = RangeEnd;
      if (IsInstantMoveEnabled)
      {
        double logicalDelta = ConvertPhysicalToLogical(Orientation == Orientation.Horizontal ? _mouseOverDecreasePoint.X : -_mouseOverDecreasePoint.Y);

        if (RangeStart <= RangeEnd)
        {
          SetRangeStart(RangeStart + logicalDelta);
        }
        else
        {
          SetRangeEnd(RangeEnd + logicalDelta);
        }
        _mouseOverDecreasePoint = new Point(0, 0);
        _mouseOverIncreasePoint = new Point(0, 0);
      }
      else
      {
        double change = LargeChange;
        if (SnapToTickMarks)
        {
          change = Math.Max(TickSpacing, LargeChange);
        }
        if (RangeStart <= RangeEnd)
        {
          SetRangeStart(RangeStart - change);
        }
        else
        {
          SetRangeEnd(RangeEnd - change);
        }
      }
      OnRangeChanged(oldRangeStart, oldRangeEnd);
      _rangeEventLock = false;
    }

    private void RangeThumb_DragDelta(object sender, DragDeltaEventArgs e)
    {
      _ignoreValidation = true;
      _rangeEventLock = true;
      double oldRangeStart = RangeStart;
      double oldRangeEnd = RangeEnd;
      double logicalDelta = ConvertPhysicalToLogical(Orientation == Orientation.Horizontal ? e.HorizontalChange : -e.VerticalChange);

      double newRangeStart = RangeStart + logicalDelta;
      double newRangeEnd = RangeEnd + logicalDelta;
      if (SnapToTickMarks)
      {
        newRangeStart = GetClosestTickMark(newRangeStart);
        newRangeEnd = GetClosestTickMark(newRangeEnd);
      }
      double range = RangeEnd - RangeStart;
      if (RangeStart < RangeEnd)
      {
        if (newRangeStart < Minimum)
        {
          RangeStart = Minimum;
          SetRangeEnd(Minimum + range);
        }
        else if (newRangeEnd > Maximum)
        {
          RangeEnd = Maximum;
          SetRangeStart(Maximum - range);
        }
        else
        {
          RangeStart = newRangeStart;
          SetRangeEnd(RangeStart + range);
        }
      }
      else
      {
        if (newRangeStart > Maximum)
        {
          RangeStart = Maximum;
          SetRangeEnd(Maximum + range);
        }
        else if (newRangeEnd < Minimum)
        {
          RangeEnd = Minimum;
          SetRangeStart(Minimum - range);
        }
        else
        {
          RangeEnd = newRangeEnd;
          SetRangeStart(RangeEnd - range);
        }
      }
      OnRangeChanged(oldRangeStart, oldRangeEnd);
      _ignoreValidation = false;
      _rangeEventLock = false;
    }

    private void StartThumb_DragDelta(object sender, DragDeltaEventArgs e)
    {
      double logicalDelta = ConvertPhysicalToLogical(Orientation == Orientation.Horizontal ? e.HorizontalChange : -e.VerticalChange);
      SetRangeStart(RangeStart + logicalDelta);
    }

    private void EndThumb_DragDelta(object sender, DragDeltaEventArgs e)
    {
      double logicalDelta = ConvertPhysicalToLogical(Orientation == Orientation.Horizontal ? e.HorizontalChange : -e.VerticalChange);
      SetRangeEnd(RangeEnd + logicalDelta);
    }

    private double ConvertPhysicalToLogical(double physical)
    {
      double logicalSize = Maximum - Minimum;
      double physicalSize = (Orientation == Orientation.Horizontal ? ActualWidth : ActualHeight) - (2 * EndBuffer);
      if (IsReversed)
      {
        physical = -physical;
      }
      double ratio = logicalSize / physicalSize;
      double logicalPosition = physical * ratio;
      return logicalPosition;
    }

    private void SetRangeStart(double start)
    {
      _ignoreValidation = true;
      double min = Math.Max(Minimum, Math.Min(Maximum, RangeEnd - MaximumRange));
      double max = Math.Min(Maximum, Math.Max(Minimum, RangeEnd - MinimumRange));
      if (!AllowOverlap)
      {
        max = Math.Min(RangeEnd, max);
      }

      if (SnapToTickMarks)
      {
        start = GetClosestTickMark(start);
        min = GetNextTickMark(min);
        max = GetPreviousTickMark(max);
      }
      if (start < min)
      {
        RangeStart = min;
      }
      else if (start > max)
      {
        RangeStart = max;
      }
      else
      {
        RangeStart = start;
      }
      _ignoreValidation = false;
    }

    private void SetRangeEnd(double end)
    {
      _ignoreValidation = true;
      double min = Math.Max(Minimum, Math.Min(Maximum, RangeStart + MinimumRange));
      double max = Math.Min(Maximum, Math.Max(Minimum, RangeStart + MaximumRange));
      if (!AllowOverlap)
      {
        min = Math.Max(RangeStart, min);
      }

      if (SnapToTickMarks)
      {
        end = GetClosestTickMark(end);
        min = GetNextTickMark(min);
        max = GetPreviousTickMark(max);
      }
      if (end < min)
      {
        RangeEnd = min;
      }
      else if (end > max)
      {
        RangeEnd = max;
      }
      else
      {
        RangeEnd = end;
      }
      _ignoreValidation = false;
    }

    private void UpdateControlPositions()
    {
      if (!Double.IsNaN(Maximum) && !Double.IsNaN(Minimum))
      {
        double logicalSize = Maximum - Minimum;
        double rootSize = ActualWidth;
        if (Orientation == Orientation.Vertical)
        {
          rootSize = ActualHeight;
        }
        double physicalSize = rootSize - (2 * EndBuffer);

        // Start position:
        double logicalStartPosition = RangeStart - Minimum;
        if (IsReversed)
        {
          logicalStartPosition = RangeEnd - Minimum;
        }
        double startRatio = logicalSize == 0 ? 0 : logicalStartPosition / logicalSize;
        double physicalStartPosition = physicalSize * startRatio;
        if (IsReversed)
        {
          physicalStartPosition = physicalSize - physicalStartPosition;
        }
        physicalStartPosition = Math.Max(EndBuffer, physicalStartPosition + EndBuffer - 1);

        // End position:
        double logicalEndPosition = RangeEnd - Minimum;
        if (IsReversed)
        {
          logicalEndPosition = RangeStart - Minimum;
        }
        double endRatio = logicalSize == 0 ? 0 : logicalEndPosition / logicalSize;
        double physicalEndPosition = physicalSize * endRatio;
        if (IsReversed)
        {
          physicalEndPosition = physicalSize - physicalEndPosition;
        }
        physicalEndPosition = Math.Max(EndBuffer, physicalEndPosition + EndBuffer - 1);

        double decreaseButtonSize = Math.Max(0, RangeStart <= RangeEnd ? physicalStartPosition : physicalEndPosition);
        double increaseButtonSize = Math.Max(0, RangeStart <= RangeEnd ? Math.Max(0, rootSize - physicalEndPosition) : Math.Max(0, rootSize - physicalStartPosition));

        if (Orientation == Orientation.Horizontal)
        {
          if (_startThumb != null)
          {
            _startThumb.RenderTransform = new TranslateTransform(physicalStartPosition, 0);
          }
          if (_endThumb != null)
          {
            _endThumb.RenderTransform = new TranslateTransform(physicalEndPosition, 0);
          }

          if (_rangeThumb != null)
          {
            _rangeThumb.RenderTransform = new TranslateTransform(Math.Min(physicalStartPosition, physicalEndPosition), 0);
            _rangeThumb.Width = Math.Abs(physicalEndPosition - physicalStartPosition + 1);
          }

          if (_largeDecreaseButton != null)
          {
            _largeDecreaseButton.Width = Math.Max(0, decreaseButtonSize);
            _largeIncreaseButton.RenderTransform = new TranslateTransform(Math.Max(physicalEndPosition, physicalStartPosition), 0);
          }
          if (_largeIncreaseButton != null)
          {
            _largeIncreaseButton.Width = Math.Max(0, increaseButtonSize);
          }
        }
        else
        {
          if (_startThumb != null)
          {
            _startThumb.RenderTransform = new TranslateTransform(0, -physicalStartPosition);
          }
          if (_endThumb != null)
          {
            _endThumb.RenderTransform = new TranslateTransform(0, -physicalEndPosition);
          }

          if (_rangeThumb != null)
          {
            _rangeThumb.RenderTransform = new TranslateTransform(0, -Math.Min(physicalStartPosition, physicalEndPosition));
            _rangeThumb.Height = Math.Abs(physicalEndPosition - physicalStartPosition);
          }

          if (_largeDecreaseButton != null)
          {
            _largeDecreaseButton.Height = Math.Max(0, decreaseButtonSize);
          }
          if (_largeIncreaseButton != null)
          {
            _largeIncreaseButton.RenderTransform = new TranslateTransform(0, -Math.Max(physicalEndPosition, physicalStartPosition));
            _largeIncreaseButton.Height = Math.Max(0, increaseButtonSize);
          }
        }
      }
    }

    #region RangeStart property

    /// <summary>
    /// Gets or sets the start of the selected range (the value of the first slider thumb).
    /// This is a dependency property.
    /// </summary>
    public double RangeStart
    {
      get { return (double)GetValue(RangeStartProperty); }
      set { SetValue(RangeStartProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="RangeStart"/> property.
    /// </summary>
    public static readonly DependencyProperty RangeStartProperty =
      DependencyProperty.Register("RangeStart", typeof(double), typeof(DualSlider),
      new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnRangeStartChanged));

    private static void OnRangeStartChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DualSlider)d).OnRangeStartChanged(e);
    }

    private void OnRangeStartChanged(DependencyPropertyChangedEventArgs e)
    {
      if (!_ignoreValidation && !_loading)
      {
        SetRangeStart(RangeStart);
      }
      UpdateControlPositions();
      if (!_rangeEventLock)
      {
        OnRangeChanged((double)e.OldValue, RangeEnd);
      }
    }

    #endregion // RangeStart property

    #region RangeEnd property

    /// <summary>
    /// Gets or sets the end of the selected range (the value of the second slider thumb).
    /// This is a dependency property.
    /// </summary>
    public double RangeEnd
    {
      get { return (double)GetValue(RangeEndProperty); }
      set { SetValue(RangeEndProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="RangeEnd"/> property.
    /// </summary>
    public static readonly DependencyProperty RangeEndProperty =
      DependencyProperty.Register("RangeEnd", typeof(double), typeof(DualSlider),
      new FrameworkPropertyMetadata(100.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnRangeEndChanged));

    private static void OnRangeEndChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DualSlider)d).OnRangeEndChanged(e);
    }

    private void OnRangeEndChanged(DependencyPropertyChangedEventArgs e)
    {
      if (!_ignoreValidation && !_loading)
      {
        SetRangeEnd(RangeEnd);
      }
      UpdateControlPositions();
      if (!_rangeEventLock)
      {
        OnRangeChanged(RangeEnd, (double)e.OldValue);
      }
    }

    #endregion // RangeEnd property

    #region EndBuffer property

    /// <summary>
    /// Gets or sets the distance that the slider track extends beyond the minimum and maximum
    /// values.
    /// This is a dependency property.
    /// </summary>
    public double EndBuffer
    {
      get { return (double)GetValue(EndBufferProperty); }
      set { SetValue(EndBufferProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="EndBuffer"/> property.
    /// </summary>
    public static readonly DependencyProperty EndBufferProperty =
      DependencyProperty.Register("EndBuffer", typeof(double), typeof(DualSlider),
      null);

    #endregion // EndBuffer property

    #region AllowOverlap property

    /// <summary>
    /// Gets or sets whether the sliders are allowed to pass each other (i.e. the second slider
    /// value is allowed to be lower than the first slider value).
    /// This is a dependency property.
    /// </summary>
    public bool AllowOverlap
    {
      get { return (bool)GetValue(AllowOverlapProperty); }
      set { SetValue(AllowOverlapProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="AllowOverlap"/> property.
    /// </summary>
    public static readonly DependencyProperty AllowOverlapProperty =
      DependencyProperty.Register("AllowOverlap", typeof(bool), typeof(DualSlider),
      new PropertyMetadata(OnAllowOverlapChanged));

    private static void OnAllowOverlapChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DualSlider)d).OnAllowOverlapChanged();
    }

    private void OnAllowOverlapChanged()
    {
      if (!AllowOverlap)
      {
        MaximumRange = Math.Abs(MaximumRange);
      }
      double range = Math.Abs(RangeEnd - RangeStart);
      RangeEnd = Math.Min(Maximum, RangeStart + Math.Min(range, MaximumRange));
    }

    #endregion // AllowOverlap property

    #region MinimumRange property

    /// <summary>
    /// Gets or sets the minimum distance between the slider values.  The user cannot drag the sliders
    /// closer than this value.  The default is no minimum.
    /// This is a dependency property.
    /// </summary>
    public double MinimumRange
    {
      get { return (double)GetValue(MinimumRangeProperty); }
      set { SetValue(MinimumRangeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="MinimumRange"/> property.
    /// </summary>
    public static readonly DependencyProperty MinimumRangeProperty =
      DependencyProperty.Register("MinimumRange", typeof(double), typeof(DualSlider),
      new PropertyMetadata(Double.MinValue, OnMinimumRangeChanged));

    private static void OnMinimumRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DualSlider)d).OnMinimumRangeChanged(e);
    }

    private void OnMinimumRangeChanged(DependencyPropertyChangedEventArgs e)
    {
      if (MinimumRange > MaximumRange)
      {
        MinimumRange = MaximumRange;
      }
      if (MinimumRange > Maximum - Minimum)
      {
        MinimumRange = Maximum - Minimum;
      }
      if (!IsValidRangeRestriction())
      {
        //MaximumRange = MinimumRange + TickSpacing;
        MinimumRange = (double)e.OldValue;
      }
      if (!_loading)
      {
        AdjustStartAndEnd();
      }
    }

    #endregion // MinimumRange property

    #region MaximumRange property

    /// <summary>
    /// Gets or sets the maximum distance between the slider values.  The user cannot drag the sliders
    /// further apart than this value.  The default is no maximum.
    /// This is a dependency property.
    /// </summary>
    public double MaximumRange
    {
      get { return (double)GetValue(MaximumRangeProperty); }
      set { SetValue(MaximumRangeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="MaximumRange"/> property.
    /// </summary>
    public static readonly DependencyProperty MaximumRangeProperty =
      DependencyProperty.Register("MaximumRange", typeof(double), typeof(DualSlider),
      new PropertyMetadata(Double.MaxValue, OnMaximumRangeChanged));

    private static void OnMaximumRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DualSlider)d).OnMaximumRangeChanged(e);
    }

    private void OnMaximumRangeChanged(DependencyPropertyChangedEventArgs e)
    {
      if (MaximumRange < MinimumRange)
      {
        MaximumRange = MinimumRange;
      }
      if (MaximumRange < -(Maximum - Minimum))
      {
        MaximumRange = -(Maximum - Minimum);
      }
      if (!IsValidRangeRestriction())
      {
        //MinimumRange = MaximumRange - TickSpacing;
        MaximumRange = (double)e.OldValue;
      }
      if (MaximumRange < 0)
      {
        AllowOverlap = true;
      }
      if (!_loading)
      {
        AdjustStartAndEnd();
      }
    }

    #endregion // MaximumRange property

    internal override void OnIsReversedChanged()
    {
      base.OnIsReversedChanged();
      UpdateControlPositions();
      if (_startThumb != null)
      {
        if (IsReversed)
        {
          _startThumb.DragDelta -= new DragDeltaEventHandler(StartThumb_DragDelta);
          _startThumb.DragDelta += new DragDeltaEventHandler(EndThumb_DragDelta);
        }
        else
        {
          _startThumb.DragDelta -= new DragDeltaEventHandler(EndThumb_DragDelta);
          _startThumb.DragDelta += new DragDeltaEventHandler(StartThumb_DragDelta);
        }
      }
      if (_endThumb != null)
      {
        if (IsReversed)
        {
          _endThumb.DragDelta -= new DragDeltaEventHandler(EndThumb_DragDelta);
          _endThumb.DragDelta += new DragDeltaEventHandler(StartThumb_DragDelta);
        }
        else
        {
          _endThumb.DragDelta -= new DragDeltaEventHandler(StartThumb_DragDelta);
          _endThumb.DragDelta += new DragDeltaEventHandler(EndThumb_DragDelta);
        }
      }
    }

    private bool IsValidRangeRestriction()
    {
      bool result = true;
      if (SnapToTickMarks)
      {
        double diff = MaximumRange - MinimumRange;
        double min = Math.Abs(MinimumRange);
        double max = min + diff;
        if (min % TickSpacing != 0)
        {
          min = GetPreviousTickMark(min);
          min += TickSpacing;
          if (min > max)
          {
            result = false;
          }
        }
      }
      return result;
    }

    /// <summary>
    /// Called when the SnapToTickMarks property changes.
    /// </summary>
    protected override void OnSnapToTickMarksChanged()
    {
      if (!IsValidRangeRestriction())
      {
        //MaximumRange = Maximum - Minimum;
        //MinimumRange = -(Maximum - Minimum);
        MaximumRange = MinimumRange + TickSpacing;
      }
      AdjustStartAndEnd();
    }

    /// <summary>
    /// Called when the TickSpacing property changes.
    /// </summary>
    protected override void OnTickSpacingChanged()
    {
      base.OnTickSpacingChanged();
      if (!IsValidRangeRestriction())
      {
        //MaximumRange = Maximum - Minimum;
        //MinimumRange = -(Maximum - Minimum);
        MaximumRange = MinimumRange + TickSpacing;
      }
      AdjustStartAndEnd();
    }

    /// <summary>
    /// Called when the Minimum property changes.
    /// </summary>
    protected override void OnMinimumChanged()
    {
      base.OnMinimumChanged();
      AdjustStartAndEnd();
      UpdateControlPositions();
      UpdateTickMarks();
    }

    /// <summary>
    /// Called when the Maximum property changes.
    /// </summary>
    protected override void OnMaximumChanged()
    {
      base.OnMaximumChanged();
      AdjustStartAndEnd();
      UpdateControlPositions();
      UpdateTickMarks();
    }

    private void AdjustStartAndEnd()
    {
      if (!_loading)
      {
        _rangeEventLock = true;
        double oldRangeStart = RangeStart;
        double oldRangeEnd = RangeEnd;
        SetRangeStart(RangeStart);
        SetRangeEnd(RangeEnd);
        OnRangeChanged(oldRangeStart, oldRangeEnd);
        _rangeEventLock = false;
      }
    }

    #region Range Property

    /// <summary>
    /// Gets the magnitude of the range being displayed by the <see cref="DualSlider"/> (the difference
    /// between the first and second slider values).
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
    }

    private static readonly DependencyPropertyKey RangePropertyKey =
        DependencyProperty.RegisterReadOnly("Range", typeof(double), typeof(DualSlider), new UIPropertyMetadata(0.0));

    /// <summary>
    /// Identifies the <see cref="Range"/> property.
    /// </summary>
    public static readonly DependencyProperty RangeProperty = RangePropertyKey.DependencyProperty;

    /// <summary>
    /// Raised when the StartRange or/and EndRange of the <see cref="DualSlider"/> changes.
    /// </summary>
    public event EventHandler<RangeChangedEventArgs> RangeChanged;

    #endregion // Range Property

    private void OnRangeChanged(double oldRangeStart, double oldRangeEnd)
    {
      SetValue(RangePropertyKey, RangeEnd - RangeStart);
      if (oldRangeStart != RangeStart || oldRangeEnd != RangeEnd)
      {
        EventHandler<RangeChangedEventArgs> handler = RangeChanged;
        if (handler != null)
        {
          handler(this, new RangeChangedEventArgs(oldRangeStart, oldRangeEnd));
        }
      }
    }

    /// <summary>
    /// Gets the extra spacing allocated around the slider track.
    /// </summary>
    protected override double ExtraSpacing
    {
      get { return 2 * EndBuffer; }
    }
  }

  /// <summary>
  /// Provides event data for range changed events.
  /// </summary>
  public class RangeChangedEventArgs : EventArgs
  {
    private readonly double _oldRangeStart;
    private readonly double _oldRangeEnd;

    internal RangeChangedEventArgs(double oldRangeStart, double oldRangeEnd)
    {
      _oldRangeStart = oldRangeStart;
      _oldRangeEnd = oldRangeEnd;
    }

    /// <summary>
    /// Gets the old range start value.
    /// </summary>
    public double OldRangeStart
    {
      get { return _oldRangeStart; }
    }

    /// <summary>
    /// Gets the old range end value.
    /// </summary>
    public double OldRangeEnd
    {
      get { return _oldRangeEnd; }
    }
  }
}
