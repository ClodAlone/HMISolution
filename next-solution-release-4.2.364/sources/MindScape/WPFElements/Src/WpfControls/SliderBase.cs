using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// The base class for slider controls.
  /// </summary>
  public abstract class SliderBase : Control
  {
    private ItemsControl _topLeftLabelHost;
    private ItemsControl _bottomRightLabelHost;

    #region SmallChange property

    /// <summary>
    /// Gets or sets the amount to be added to or subtracted from a slider value when
    /// a small change is made (e.g. mouse wheel or arrow key).
    /// This is a dependency property.
    /// </summary>
    public double SmallChange
    {
      get { return (double)GetValue(SmallChangeProperty); }
      set { SetValue(SmallChangeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="SmallChange"/> property.
    /// </summary>
    public static readonly DependencyProperty SmallChangeProperty =
      DependencyProperty.Register("SmallChange", typeof(double), typeof(SliderBase),
      new PropertyMetadata(0.1));

    #endregion // SmallChange property

    #region LargeChange property

    /// <summary>
    /// Gets or sets the amount to be added to or subtracted from a slider value when
    /// a large change is made (e.g. the user clicks on the track and IsInstantMoveEnabled is false).
    /// This is a dependency property.
    /// </summary>
    public double LargeChange
    {
      get { return (double)GetValue(LargeChangeProperty); }
      set { SetValue(LargeChangeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="LargeChange"/> property.
    /// </summary>
    public static readonly DependencyProperty LargeChangeProperty =
      DependencyProperty.Register("LargeChange", typeof(double), typeof(SliderBase),
      new PropertyMetadata(1.0));

    #endregion // LargeChange property

    #region Maximum property

    /// <summary>
    /// Gets or sets the maximum slider value.
    /// This is a dependency property.
    /// </summary>
    public double Maximum
    {
      get { return (double)GetValue(MaximumProperty); }
      set { SetValue(MaximumProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Maximum"/> property.  The default is 1.
    /// </summary>
    public static readonly DependencyProperty MaximumProperty =
      DependencyProperty.Register("Maximum", typeof(double), typeof(SliderBase),
      new PropertyMetadata(1.0, new PropertyChangedCallback(OnMaximumChanged)));

    private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((SliderBase)d).OnMaximumChanged();
    }

    /// <summary>
    /// Called when the <see cref="Maximum"/> property changes.
    /// </summary>
    protected virtual void OnMaximumChanged()
    {
      if (Maximum < Minimum)
      {
        //Maximum = Minimum;
        Minimum = Maximum;
      }
      else
      {
        UpdateTickMarks();
      }
    }

    #endregion // Maximum property

    #region Minimum property

    /// <summary>
    /// Gets or sets the minimum slider value.  The default is 0.
    /// This is a dependency property.
    /// </summary>
    public double Minimum
    {
      get { return (double)GetValue(MinimumProperty); }
      set { SetValue(MinimumProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Minimum"/> property.
    /// </summary>
    public static readonly DependencyProperty MinimumProperty =
      DependencyProperty.Register("Minimum", typeof(double), typeof(SliderBase),
      new PropertyMetadata(0.0, new PropertyChangedCallback(OnMinimumChanged)));

    private static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((SliderBase)d).OnMinimumChanged();
    }

    /// <summary>
    /// Called when the <see cref="Minimum"/> property changes.
    /// </summary>
    protected virtual void OnMinimumChanged()
    {
      if (Minimum > Maximum)
      {
        //Minimum = Maximum;
        Maximum = Minimum;
      }
      else
      {
        UpdateTickMarks();
      }
    }

    #endregion // Minimum property

    #region Orientation property

    /// <summary>
    /// Gets or sets the orientation of the slider track.
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
      DependencyProperty.Register("Orientation", typeof(Orientation), typeof(SliderBase),
      new PropertyMetadata(Orientation.Horizontal, OnOrientationChanged));

    private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((SliderBase)d).OnOrientationChanged();
    }

    /// <summary>
    /// Called when the <see cref="Orientation"/> of this slider has changed.
    /// </summary>
    protected virtual void OnOrientationChanged()
    {
    }

    #endregion // Orientation property

    #region IsInstantMoveEnabled property

    /// <summary>
    /// Gets or sets whether, when the user clicks on the track, the slider value should jump
    /// immediately to the clicked value.  The default is false, meaning the slider value should
    /// execute a <see cref="LargeChange"/> towards the clicked value.
    /// This is a dependency property.
    /// </summary>
    public bool IsInstantMoveEnabled
    {
      get { return (bool)GetValue(IsInstantMoveEnabledProperty); }
      set { SetValue(IsInstantMoveEnabledProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsInstantMoveEnabled"/> property.
    /// </summary>
    public static readonly DependencyProperty IsInstantMoveEnabledProperty =
      DependencyProperty.Register("IsInstantMoveEnabled", typeof(bool), typeof(SliderBase),
      null);

    #endregion // IsInstantMoveEnabled property

    #region TickSpacing property

    /// <summary>
    /// Gets or sets the spacing between ticks.  The default is 10.
    /// This is a dependency property.
    /// </summary>
    public double TickSpacing
    {
      get { return (double)GetValue(TickSpacingProperty); }
      set { SetValue(TickSpacingProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="TickSpacing"/> property.
    /// </summary>
    public static readonly DependencyProperty TickSpacingProperty =
      DependencyProperty.Register("TickSpacing", typeof(double), typeof(SliderBase),
      new PropertyMetadata(1.0, new PropertyChangedCallback(OnTickSpacingChanged)));

    private static void OnTickSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((SliderBase)d).OnTickSpacingChanged();
    }

    /// <summary>
    /// Called when the <see cref="TickSpacing"/> property changes.
    /// </summary>
    protected virtual void OnTickSpacingChanged()
    {
      if (TickSpacing <= 0)
      {
        TickSpacing = 1;
      }
      else
      {
        UpdateTickMarks();
      }
    }

    #endregion // TickSpacing property

    #region ShowTopLeftTickMarks property

    /// <summary>
    /// Gets or sets whether tick marks should be displayed above the slider track
    /// (to the left in vertical orientation).
    /// This is a dependency property.
    /// </summary>
    public bool ShowTopLeftTickMarks
    {
      get { return (bool)GetValue(ShowTopLeftTickMarksProperty); }
      set { SetValue(ShowTopLeftTickMarksProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ShowTopLeftTickMarks"/> property.
    /// </summary>
    public static readonly DependencyProperty ShowTopLeftTickMarksProperty =
      DependencyProperty.Register("ShowTopLeftTickMarks", typeof(bool), typeof(SliderBase),
      null);

    #endregion // ShowTopLeftTickMarks property

    #region ShowBottomRightTickMarks property

    /// <summary>
    /// Gets or sets whether tick marks should be displayed below the slider track
    /// (to the right in vertical orientation).
    /// This is a dependency property.
    /// </summary>
    public bool ShowBottomRightTickMarks
    {
      get { return (bool)GetValue(ShowBottomRightTickMarksProperty); }
      set { SetValue(ShowBottomRightTickMarksProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ShowBottomRightTickMarks"/> property.
    /// </summary>
    public static readonly DependencyProperty ShowBottomRightTickMarksProperty =
      DependencyProperty.Register("ShowBottomRightTickMarks", typeof(bool), typeof(SliderBase),
      null);

    #endregion // ShowBottomRightTickMarks property

    #region ShowTopLeftLabels Property

    /// <summary>
    /// Gets or sets whether labels should be displayed above the slider track.
    /// (to the left in vertical orientation).
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ShowTopLeftLabelsProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool ShowTopLeftLabels
    {
      get { return (bool)GetValue(ShowTopLeftLabelsProperty); }
      set { SetValue(ShowTopLeftLabelsProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ShowTopLeftLabels"/> property.
    /// </summary>
    public static readonly DependencyProperty ShowTopLeftLabelsProperty =
      DependencyProperty.Register("ShowTopLeftLabels", typeof(bool), typeof(SliderBase),
      new FrameworkPropertyMetadata(false, OnShowTopLeftLabelsChanged));

    private static void OnShowTopLeftLabelsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((SliderBase)d).OnShowTopLeftLabelsChanged();
    }

    private void OnShowTopLeftLabelsChanged()
    {
    }

    #endregion // ShowTopLeftLabels Property

    #region ShowBottomRightLabels Property

    /// <summary>
    /// Gets or sets whether labels should be displayed below the slider track
    /// (to the right in vertical orientation).
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ShowBottomRightLabelsProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool ShowBottomRightLabels
    {
      get { return (bool)GetValue(ShowBottomRightLabelsProperty); }
      set { SetValue(ShowBottomRightLabelsProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ShowBottomRightLabels"/> property.
    /// </summary>
    public static readonly DependencyProperty ShowBottomRightLabelsProperty =
      DependencyProperty.Register("ShowBottomRightLabels", typeof(bool), typeof(SliderBase),
      new FrameworkPropertyMetadata(false, OnShowBottomRightLabelsChanged));

    private static void OnShowBottomRightLabelsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((SliderBase)d).OnShowBottomRightLabelsChanged();
    }

    private void OnShowBottomRightLabelsChanged()
    {
    }

    #endregion // ShowBottomRightLabels Property

    #region SnapToTickMarks property

    /// <summary>
    /// Gets or sets whether slider values should snap to tick marks.
    /// This is a dependency property.
    /// </summary>
    public bool SnapToTickMarks
    {
      get { return (bool)GetValue(SnapToTickMarksProperty); }
      set { SetValue(SnapToTickMarksProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="SnapToTickMarks"/> property.
    /// </summary>
    public static readonly DependencyProperty SnapToTickMarksProperty =
      DependencyProperty.Register("SnapToTickMarks", typeof(bool), typeof(SliderBase),
      new PropertyMetadata(OnSnapToTickMarksChanged));

    private static void OnSnapToTickMarksChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((SliderBase)d).OnSnapToTickMarksChanged();
    }

    /// <summary>
    /// Called when the <see cref="SnapToTickMarks"/> property changes.
    /// </summary>
    protected virtual void OnSnapToTickMarksChanged()
    {
    }

    #endregion // SnapToTickMarks property

    #region IsMouseWheelEnabled property

    /// <summary>
    /// Gets or sets whether the control should respond to the mouse wheel.  If the
    /// mouse wheel is enabled, its effect is to modify the slider value by <see cref="SmallChange"/>.
    /// This is a dependency property.
    /// </summary>
    public bool IsMouseWheelEnabled
    {
      get { return (bool)GetValue(IsMouseWheelEnabledProperty); }
      set { SetValue(IsMouseWheelEnabledProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsMouseWheelEnabled"/> property.
    /// </summary>
    public static readonly DependencyProperty IsMouseWheelEnabledProperty =
      DependencyProperty.Register("IsMouseWheelEnabled", typeof(bool), typeof(SliderBase),
      null);

    #endregion // IsMouseWheelEnabled property

    #region IsReversed Property

    /// <summary>
    /// Gets or sets whether or not the slider direction is reversed.
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
      DependencyProperty.Register("IsReversed", typeof(bool), typeof(SliderBase),
      new FrameworkPropertyMetadata(OnIsReversedChanged));

    private static void OnIsReversedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((SliderBase)d).OnIsReversedChanged();
    }

    internal virtual void OnIsReversedChanged()
    {
      UpdateTickMarks();
    }

    #endregion // IsReversed Property

    #region LabelStep Property

    /// <summary>
    /// Gets or sets a value that determines if any labels should not be rendered.
    /// A value of 1 will allow all labels to be rendered. A value of 2 means only every second label will be rendered and so on.
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
      DependencyProperty.Register("LabelStep", typeof(int), typeof(SliderBase),
      new FrameworkPropertyMetadata(1, OnLabelStepChanged));

    private static void OnLabelStepChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((SliderBase)d).OnLabelStepChanged();
    }

    private void OnLabelStepChanged()
    {
    }

    #endregion // LabelStep Property

    #region LabelTemplate Property

    /// <summary>
    /// Gets or sets the <see cref="DataTemplate"/> used to render labels.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="LabelTemplateProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public DataTemplate LabelTemplate
    {
      get { return (DataTemplate)GetValue(LabelTemplateProperty); }
      set { SetValue(LabelTemplateProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="LabelTemplate"/> property.
    /// </summary>
    public static readonly DependencyProperty LabelTemplateProperty =
      DependencyProperty.Register("LabelTemplate", typeof(DataTemplate), typeof(SliderBase),
      new FrameworkPropertyMetadata(OnLabelTemplateChanged));

    private static void OnLabelTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((SliderBase)d).OnLabelTemplateChanged();
    }

    private void OnLabelTemplateChanged()
    {
    }

    #endregion // LabelTemplate Property

    /// <summary>
    /// Called by the framework when the control template is applied.
    /// </summary>
    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      _topLeftLabelHost = GetTemplateChild("PART_TopLeftLabelHost") as ItemsControl;
      _bottomRightLabelHost = GetTemplateChild("PART_BottomRightLabelHost") as ItemsControl;
    }

    internal void UpdateTickMarks()
    {
      List<double> ticks = new List<double>();
      List<ContentControl> labels1 = new List<ContentControl>();
      List<ContentControl> labels2 = new List<ContentControl>();
      double logicalSize = Maximum - Minimum;
      if (Double.IsNaN(Maximum) || Double.IsNaN(Minimum) || logicalSize / TickSpacing > 2000)
      {
        return;
      }
      double min = 0;
      double start = min - Minimum % TickSpacing;
      if (start <= 0)
      {
        start += TickSpacing;
      }
      double max = min + logicalSize;
      double physicalSize = Math.Max(0, (Orientation == Orientation.Horizontal ? ActualWidth : ActualHeight) - ExtraSpacing);
      double ratio = logicalSize == 0 ? 0 : physicalSize / logicalSize;
      ticks.Add(0);
      if (ShowTopLeftLabels)
      {
        labels1.Add(BuildLabel(IsReversed ? Maximum : Minimum, 0));
      }
      if (ShowBottomRightLabels)
      {
        labels2.Add(BuildLabel(IsReversed ? Maximum : Minimum, 0));
      }
      int labelStep = 1;
      if (TickSpacing > 0)
      {
        double tick = start;
        while (tick < max)
        {
          double position = tick * ratio;
          if (IsReversed)
          {
            position = physicalSize - position;
          }
          ticks.Add(Math.Max(0, position));

          if (labelStep == LabelStep)
          {
            if (ShowTopLeftLabels)
            {
              labels1.Add(BuildLabel(tick + Minimum, position));
            }
            if (ShowBottomRightLabels)
            {
              labels2.Add(BuildLabel(tick + Minimum, position));
            }
          }
          labelStep++;
          if (labelStep > LabelStep)
          {
            labelStep = 1;
          }

          tick += TickSpacing;
        }
      }
      ticks.Add(physicalSize);
      if (ShowTopLeftLabels)
      {
        labels1.Add(BuildLabel(IsReversed ? Minimum :Maximum, physicalSize));
      }
      if (ShowBottomRightLabels)
      {
        labels2.Add(BuildLabel(IsReversed ? Minimum : Maximum, physicalSize));
      }
      SetValue(TickPositionsPropertyKey, ticks.AsReadOnly());

      if (_topLeftLabelHost != null)
      {
        _topLeftLabelHost.ItemsSource = labels1;
      }
      if (_bottomRightLabelHost != null)
      {
        _bottomRightLabelHost.ItemsSource = labels2;
      }
    }

    // TODO: use element recycling to improve performance of creating labels.
    private ContentControl BuildLabel(double value, double position)
    {
      ContentControl label = new ContentControl();
      label.Content = value;
      label.ContentTemplate = LabelTemplate;
      label.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
      TranslateTransform transform = new TranslateTransform(position - (label.DesiredSize.Width / 2.0), 0);
      if (Orientation == Orientation.Vertical)
      {
        transform = new TranslateTransform(0, ActualHeight - ExtraSpacing - position - (label.DesiredSize.Height / 2.0));
      }
      label.RenderTransform = transform;
      return label;
    }

    #region TickPositions Property

    /// <summary>
    /// Gets a list of positions where tick marks should be displayed. Each value in the list is the distance between
    /// the start of the slider track and a particular tick mark.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="TickPositionsProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public ReadOnlyCollection<double> TickPositions
    {
      get { return (ReadOnlyCollection<double>)GetValue(TickPositionsProperty); }
    }

    private static readonly DependencyPropertyKey TickPositionsPropertyKey =
        DependencyProperty.RegisterReadOnly("TickPositions", typeof(ReadOnlyCollection<double>), typeof(SliderBase), new UIPropertyMetadata(new List<double>().AsReadOnly()));

    /// <summary>
    /// Identifies the <see cref="TickPositions"/> property.
    /// </summary>
    public static readonly DependencyProperty TickPositionsProperty = TickPositionsPropertyKey.DependencyProperty;

    #endregion // TickPositions Property

    /// <summary>
    /// When overridden in a derived class, gets the extra spacing allocated around the slider track.
    /// This is used in tick mark layout calculations.
    /// </summary>
    protected abstract double ExtraSpacing { get; }

    /// <summary>
    /// Gets the closest tick mark at or below the given value.
    /// </summary>
    /// <param name="current">The value to snap.</param>
    /// <returns>The value of the previous tick mark relative to the given value.</returns>
    protected double GetPreviousTickMark(double current)
    {
      // This keeps in mind that there is always a tick mark at Minimum & Maximum, but its not nessecarily divisible by TickSpacing.
      double remainder = (current == Maximum ? 0 : current % TickSpacing);
      remainder = current == Minimum ? 0 : remainder;
      return Math.Min(Maximum, Math.Max(Minimum, current - remainder));
    }

    /// <summary>
    /// Gets the closest tick mark at or above the given value.
    /// </summary>
    /// <param name="current">The value to snap.</param>
    /// <returns>The value of the next tick mark relative to the given value.</returns>
    protected double GetNextTickMark(double current)
    {
      // This keeps in mind that there is always a tick mark at Minimum & Maximum, but its not nessecarily divisible by TickSpacing.
      double remainder = (current == Maximum ? 0 : current % TickSpacing);
      remainder = current == Minimum ? 0 : remainder;
      // If current is already at a tick mark (remainder = 0) then don't modify it.
      return Math.Min(Maximum, Math.Max(Minimum, current + (remainder == 0 ? 0 : (TickSpacing - remainder))));
    }

    /// <summary>
    /// Gets the closest tick mark to the given value.
    /// </summary>
    /// <param name="current">The value to snap.</param>
    /// <returns>The value of the closest tick mark to the given value.</returns>
    protected double GetClosestTickMark(double current)
    {
      double result = current;
      double remainder = current % TickSpacing;
      if (remainder <= TickSpacing / 2)
      {
        result = current - remainder;
      }
      else
      {
        result = current + (TickSpacing - remainder);
      }
      return result;
    }
  }
}
