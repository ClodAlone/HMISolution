using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.ComponentModel;
using System.Windows.Input;
using Mindscape.WpfElements.Charting;
using System.Windows.Media;
using System.Windows.Markup;
using Infralution.Licensing;
using System.Diagnostics;
using System.Reflection;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// A control that can host a <see cref="Chart"/> control and provides functionality for the user to explore
  /// through various time ranges by sliding the time window.
  /// </summary>
  [ContentProperty("Chart")]
  [TemplatePart(Name = MainDualSliderName, Type = typeof(TimeExplorerMainDualSlider))]
  [TemplatePart(Name = InternalDualSliderName, Type = typeof(DualSlider))]
  [TemplatePart(Name = MinorButtonHostName, Type = typeof(ItemsControl))]
  [TemplatePart(Name = MajorButtonHostName, Type = typeof(ItemsControl))]
  [LicenseProvider(typeof(PublicEncryptedLicenseProvider))]
  public class TimeExplorer : Control
  {
    private const string MainDualSliderName = "PART_MainDualSlider";
    private const string InternalDualSliderName = "PART_InternalDualSlider";
    private const string MinorButtonHostName = "PART_MinorButtonHost";
    private const string MajorButtonHostName = "PART_MajorButtonHost";

    private DualSlider _internalDualSlider;
    private TimeExplorerMainDualSlider _mainDualSlider;
    private ItemsControl _minorButtonHost;
    private ItemsControl _majorButtonHost;
    private bool _settingTimeRangeInternal;

    // selection drag fields:
    private int _startSelectionIndex = -1;
    private int _previousSelectionIndex = -1;
    private TimeExplorerTimeUnit _startSelectionUnit;
    private TimeExplorerTimeUnit _endSelectionUnit;

    static TimeExplorer()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(TimeExplorer), new FrameworkPropertyMetadata(typeof(TimeExplorer)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TimeExplorer"/> class.
    /// </summary>
    public TimeExplorer()
    {
      // Licensing
      //new WpfElementsCore(Assembly.GetCallingAssembly());
      //LicenseHelper.Attach(this, Assembly.GetCallingAssembly());
      // End licensing

      SizeChanged += new SizeChangedEventHandler(TimeExplorer_SizeChanged);
      Loaded += new RoutedEventHandler(TimeExplorer_Loaded);
      Spacing = 1;
    }

    private void TimeExplorer_Loaded(object sender, RoutedEventArgs e)
    {
      UpdateMinMaxValues();

      OnViewportEndTimeChanged();
      OnViewportStartTimeChanged();
      OnRangeEndTimeChanged();
      OnRangeStartTimeChanged();

      UpdateInternalSlider();

      AttachInternalDualSliderEventHandlers(_internalDualSlider);

      if (_mainDualSlider != null)
      {
        _mainDualSlider.RangeChanged += new EventHandler<RangeChangedEventArgs>(MainDualSlider_RangeChanged);
      }
    }

    private void TimeExplorer_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      UpdateInternalSlider();
    }

    /// <summary>
    /// Called by the framework when the control template is applied.
    /// </summary>
    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      _mainDualSlider = GetTemplateChild(MainDualSliderName) as TimeExplorerMainDualSlider;
      _internalDualSlider = GetTemplateChild(InternalDualSliderName) as DualSlider;
      _minorButtonHost = GetTemplateChild(MinorButtonHostName) as ItemsControl;
      _majorButtonHost = GetTemplateChild(MajorButtonHostName) as ItemsControl;

      AttachButtonHostHandlers(_minorButtonHost);
      AttachButtonHostHandlers(_majorButtonHost);
    }

    /// <summary>
    /// Called when the mouse wheel is used over this <see cref="TimeExplorer"/>.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
      if (Mouse.PrimaryDevice.LeftButton == MouseButtonState.Released)
      {
        Point position = e.GetPosition(this);
        double LeftRatio = position.X / ActualWidth;

        double delta = e.Delta > 0 ? 1 : -1;
        double xRange = _mainDualSlider.RangeEnd - _mainDualSlider.RangeStart;
        double zoomRatio = 0.9;
        if (delta < 0)
        {
          zoomRatio = 1.1;
        }
        double newXRange = xRange * zoomRatio;
        double xRangeDiff = xRange - newXRange;

        _mainDualSlider.RangeStart += xRangeDiff * LeftRatio;
        _mainDualSlider.RangeEnd -= xRangeDiff - (xRangeDiff * LeftRatio);
      }
    }

    private void AttachButtonHostHandlers(ItemsControl host)
    {
      host.MouseLeftButtonDown += new MouseButtonEventHandler(ButtonHost_MouseLeftButtonDown);
      host.MouseLeftButtonUp += new MouseButtonEventHandler(ButtonHost_MouseLeftButtonUp);
      host.MouseMove += new MouseEventHandler(ButtonHost_MouseMove);
    }

    private void ButtonHost_MouseMove(object sender, MouseEventArgs e)
    {
      ItemsControl host = sender as ItemsControl;
      if (host.IsMouseCaptured && _startSelectionIndex != -1)
      {
        Point position = e.GetPosition(host);
        for (int i = 0; i < host.Items.Count; i++)
        {
          ContentPresenter element = host.ItemContainerGenerator.ContainerFromIndex(i) as ContentPresenter;
          if (element != null)
          {
            Point topLeft = element.TranslatePoint(new Point(0, 0), host);
            if (position.X > topLeft.X && position.X < topLeft.X + element.ActualWidth)
            {
              _endSelectionUnit = host.Items[i] as TimeExplorerTimeUnit;
              SelectionUpdateInfo info = SelectionRangeUtils.GetSelectionUpdateInfo(_startSelectionIndex, _previousSelectionIndex, i);
              for (int j = info.StartEmptyIndex; j <= info.EndEmptyIndex; j++)
              {
                TimeExplorerTimeUnit unit = host.Items[j] as TimeExplorerTimeUnit;
                if (unit != null)
                {
                  unit.IsSelected = false;
                }
              }
              for (int j = info.StartFillIndex; j <= info.EndFillIndex; j++)
              {
                TimeExplorerTimeUnit unit = host.Items[j] as TimeExplorerTimeUnit;
                if (unit != null)
                {
                  unit.IsSelected = true;
                }
              }
              _previousSelectionIndex = i;
              // TODO: have an option to update the time window while dragging the mouse. By default this should be true. This will use the commented code below:
              // Remember to only raise the time-range-changed event when the mouse is released though.
              //UpdateTimeRange(_startSelectionUnit, _endSelectionUnit);
              break;
            }
          }
        }
      }
    }

    private void ButtonHost_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      ItemsControl host = sender as ItemsControl;
      if (host.IsMouseCaptured)
      {
        host.ReleaseMouseCapture();
        for (int i = 0; i < host.Items.Count; i++)
        {
          TimeExplorerTimeUnit unit = host.Items[i] as TimeExplorerTimeUnit;
          if (unit != null)
          {
            unit.IsSelected = false;
          }
        }
        if (_startSelectionUnit != null && _endSelectionUnit != null)
        {
          UpdateTimeRange(_startSelectionUnit, _endSelectionUnit);
        }
      }
      _startSelectionIndex = -1;
    }

    private void UpdateTimeRange(TimeExplorerTimeUnit unit1, TimeExplorerTimeUnit unit2)
    {
      if (unit2.StartDateTime < unit1.StartDateTime)
      {
        TimeExplorerTimeUnit temp = unit1;
        unit1 = unit2;
        unit2 = temp;
      }
      SetDateTimeRange(unit1.StartDateTime, unit2.EndDateTime);
    }

    private void ButtonHost_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      ItemsControl host = sender as ItemsControl;
      
      host.CaptureMouse();

      Point position = e.GetPosition(host);
      for (int i = 0; i < host.Items.Count; i++)
      {
        ContentPresenter element = host.ItemContainerGenerator.ContainerFromIndex(i) as ContentPresenter;
        if (element != null)
        {
          Point topLeft = element.TranslatePoint(new Point(0, 0), host);
          if (position.X > topLeft.X && position.X < topLeft.X + element.ActualWidth)
          {
            _startSelectionIndex = i;
            _previousSelectionIndex = _startSelectionIndex;
            _startSelectionUnit = host.Items[i] as TimeExplorerTimeUnit;
            _endSelectionUnit = _startSelectionUnit;
            if (_startSelectionUnit != null)
            {
              _startSelectionUnit.IsSelected = true;
            }
            break;
          }
        }
      }
    }

    private void AttachInternalDualSliderEventHandlers(DualSlider dualSlider)
    {
      if (dualSlider != null)
      {
        dualSlider.FinishedRangeUpdate += new EventHandler<RangeChangedEventArgs>(InternalDualSlider_FinishedRangeUpdate);
      }
    }

    private void InternalDualSlider_FinishedRangeUpdate(object sender, RangeChangedEventArgs e)
    {
      if (Chart != null)
      {
        bool changed = false;
        _settingTimeRangeInternal = true;
        DateTime start = GetDateTime(_internalDualSlider.RangeStart);
        DateTime end = GetDateTime(_internalDualSlider.RangeEnd);
        if (RangeStartTime != start || RangeEndTime != end)
        {
          changed = true;
        }
        RangeStartTime = start;
        RangeEndTime = end;
        _settingTimeRangeInternal = false;
        if (changed)
        {
          OnTimeRangeChanged();
        }
      }
    }

    private void SetDateTimeRange(DateTime start, DateTime end)
    {
      // Here the order is important. Would be best to remove such ordering dependencies though to aid in xaml binding.
      if (start < RangeEndTime)
      {
        RangeStartTime = start;
        RangeEndTime = end;
      }
      else
      {
        RangeEndTime = end;
        RangeStartTime = start;
      }
      OnTimeRangeChanged();
    }

    /// <summary>
    /// Raised when the range of the time window has changed.
    /// </summary>
    public event EventHandler TimeRangeChanged;

    private void OnTimeRangeChanged()
    {
      EventHandler handler = TimeRangeChanged;
      if (handler != null)
      {
        handler(this, new EventArgs());
      }
    }

    private void MainDualSlider_RangeChanged(object sender, RangeChangedEventArgs e)
    {
      if (_mainDualSlider.Minimum != _mainDualSlider.Maximum)
      {
        Dispatcher.BeginInvoke(new Action(UpdateInternalSlider));
        if (!_settingTimeRangeInternal)
        {
          ViewportStartTime = GetDateTime(_mainDualSlider.RangeStart);
          // This condition is a kludge to solve an issue that was introduced when changing the default axis stuff in the Chart control.
          // It would be nice to remove such messy code, but this kludge is satisfactory.
          // The '1' in the condition represents the first millisecond of the fisrt day of year 1 which is too insignificant to care about.
          // The second part of the condition represents an undesired state for this method anyway.
          // The point of the condition is to prevent the ViewportEndTime being set incorrectly as the control is being loaded and bindings are being resolved.
          if (_mainDualSlider.RangeEnd != 1 && _mainDualSlider.RangeStart != _mainDualSlider.RangeEnd)
          {
            ViewportEndTime = GetDateTime(_mainDualSlider.RangeEnd);
          }
        }
      }
    }

    private void UpdateInternalSlider()
    {
      if (!DesignerProperties.GetIsInDesignMode(this))
      {
        if (Chart != null && _mainDualSlider != null)
        {
          double ratio = (_mainDualSlider.RangeEnd - _mainDualSlider.RangeStart + GetBuffer()) / Chart.ActualWidth;
          if (ratio != 0)
          {
            double min = (Chart.XAxis.MinimumValue - _mainDualSlider.RangeStart) / ratio;
            // TODO: the Math.Min statement below suggests that this method should be called when the _mainDualSlider.Minimum/Maximum property changes.
            //double max = (Math.Min(_internalDualSlider.Maximum, _mainDualSlider.Maximum) - _mainDualSlider.RangeStart) / ratio;
            double max = (Chart.XAxis.MaximumValue + GetBuffer() - _mainDualSlider.RangeStart) / ratio;

            double width = max - min;
            width = width >= 0 ? width : Chart.ActualWidth;
            _internalDualSlider.Width = width;
            _internalDualSlider.Margin = new Thickness(Double.IsNaN(min) ? 0 : Double.IsInfinity(min) ? 0 : min, 0, 0, 0);

            PopulateBackground();
          }
        }
      }
    }

    #region time unit logic

    private void PopulateBackground()
    {
      if (!DesignerProperties.GetIsInDesignMode(this))
      {
        if (Chart != null && _minorButtonHost != null && _majorButtonHost != null && IsLoaded)
        {
          double buffer = GetBuffer();
          _minorButtonHost.Items.Clear();
          _majorButtonHost.Items.Clear();
          _minorButtonHost.Visibility = MinorRangeButtonsVisibility;
          _majorButtonHost.Visibility = MajorRangeButtonsVisibility;

          TimeRangeUnit majorTimeUnit;
          TimeRangeUnit minorTimeUnit;
          int minorUnitCount;
          GetTimeUnits(out majorTimeUnit, out minorTimeUnit, out minorUnitCount);

          TimeSpan minorTimeSpan = TimeExplorerUtils.GetTimeSpan(minorTimeUnit);
          minorTimeSpan = new TimeSpan(minorTimeSpan.Ticks * minorUnitCount);
          double day1 = Chart.XAxis.ValueConverter.GetAxisPlotPosition(new DateTime(1, 1, 1));
          double day2 = Chart.XAxis.ValueConverter.GetAxisPlotPosition(new DateTime(1, 1, 1) + minorTimeSpan);
          Spacing = day2 - day1;
          _mainDualSlider.MinimumRange = Spacing;

          if (MinorRangeButtonsVisibility == Visibility.Visible)
          {
            PopulateButtons(minorTimeUnit, minorUnitCount, _minorButtonHost, buffer, 1);
          }

          if (MajorRangeButtonsVisibility == Visibility.Visible)
          {
            PopulateButtons(majorTimeUnit, 1, _majorButtonHost, buffer, 0);
          }

          // TODO: these values probably don't need to be set every time the background is populated.
          _internalDualSlider.Minimum = Chart.XAxis.MinimumValue;
          _internalDualSlider.Maximum = Chart.XAxis.MaximumValue + buffer;
          _internalDualSlider.TickSpacing = minorTimeUnit == TimeRangeUnit.Week ? 1 : Spacing;
          //_internalDualSlider.MinimumRange = Spacing / 2.0;

          if (_mainDualSlider != null)
          {
            _mainDualSlider.Spacing = buffer;
          }
        }
      }
    }

    private void GetTimeUnits(out TimeRangeUnit majorTimeUnit, out TimeRangeUnit minorTimeUnit, out int minorUnitCount)
    {
      TimeSpan span = ViewportEndTime - ViewportStartTime;
      minorUnitCount = 1;
      if (span.Ticks < new TimeSpan(3, 0, 0).Ticks)
      {
        majorTimeUnit = TimeRangeUnit.Hour;
        minorTimeUnit = TimeRangeUnit.Minute;
      }
      else if (span.Ticks < new TimeSpan(3, 0, 0, 0).Ticks)
      {
        majorTimeUnit = TimeRangeUnit.Day;
        minorTimeUnit = TimeRangeUnit.Hour;
      }
      else if (span.Ticks < new TimeSpan(1, 0, 0, 0).Ticks * 60)
      {
        majorTimeUnit = TimeRangeUnit.Week;
        minorTimeUnit = TimeRangeUnit.Day;
      }
      else if (span.Ticks < new TimeSpan(365, 0, 0, 0).Ticks / 2.0)
      {
        majorTimeUnit = TimeRangeUnit.Month;
        minorTimeUnit = TimeRangeUnit.Week;
      }
      else
      {
        majorTimeUnit = TimeRangeUnit.Year;
        minorTimeUnit = TimeRangeUnit.Month;
      }

      if (minorTimeUnit == TimeRangeUnit.Hour || minorTimeUnit == TimeRangeUnit.Minute)
      {
        TimeSpan approximateUnitLength = new TimeSpan();
        switch (minorTimeUnit)
        {
          case TimeRangeUnit.Hour:
            approximateUnitLength = new TimeSpan(1, 0, 0);
            break;
          case TimeRangeUnit.Minute:
            approximateUnitLength = new TimeSpan(0, 1, 0);
            break;
        }

        double approximateTotalUnitCount = span.Ticks / approximateUnitLength.Ticks;
        minorUnitCount = GetBestUnitCount(minorTimeUnit, approximateTotalUnitCount); //(int)Math.Max(1, approximateTotalUnitCount / 7.0);
      }
    }

    // TODO: this is copied from DateTimeAxisValueConverter - find a good way to reuse this code.
    private int GetBestUnitCount(TimeRangeUnit unit, double approximateTotalUnitCount)
    {
      double unitCount = Math.Max(1, approximateTotalUnitCount / 7.0);
      int[] unitCounts = new[] { 1, 2, 5 };

      switch (unit)
      {
        case TimeRangeUnit.Hour:
          unitCounts = new[] { 1, 2, 3, 4, 6, 12 };
          break;
        case TimeRangeUnit.Minute:
          unitCounts = new[] { 1, 2, 5, 10, 15, 30 };
          break;
      }

      for (int i = 1; i < unitCounts.Length; i++)
      {
        int low = unitCounts[i - 1];
        int high = unitCounts[i];
        double mid = (low + high) / 2.0;
        if (unitCount >= low && unitCount <= mid)
        {
          unitCount = low;
          break;
        }
        if (unitCount > mid && unitCount <= high)
        {
          unitCount = high;
          break;
        }
        if (i == unitCounts.Length - 1)
        {
          int count = unitCounts[unitCounts.Length - 1];
          unitCount = ((int)(unitCount / count)) * count;
        }
      }
      return (int)unitCount;
    }

    private void PopulateButtons(TimeRangeUnit unit, int unitCount, ItemsControl buttonHost, double buffer, double spacing)
    {
      TimeSpan timeSpan = TimeExplorerUtils.GetTimeSpan(unit);
      timeSpan = new TimeSpan(timeSpan.Ticks * unitCount);
      double date1 = Chart.XAxis.ValueConverter.GetAxisPlotPosition(new DateTime(1, 1, 1));
      double date2 = Chart.XAxis.ValueConverter.GetAxisPlotPosition(new DateTime(1, 1, 1) + timeSpan);
      double physical1 = ConvertLogicalToPhysical(date1);
      double physical2 = ConvertLogicalToPhysical(date2);
      if (physical1 == physical2)
      {
        return;
      }
      double physicalWidth = physical2 - physical1;

      double commonSize = physicalWidth;
      if (unit == TimeRangeUnit.Year)
      {
        commonSize = physicalWidth * 365;
      }
      else if (unit == TimeRangeUnit.Month)
      {
        commonSize = physicalWidth * 30;
      }

      DateTime dateTime = ((DateTime)Chart.XAxis.ValueConverter.GetDataObjectAt(_mainDualSlider.RangeStart)).Date;
      dateTime = dateTime.StartOfTimeUnit(unit);
      TimeSpan span = (DateTime)Chart.XAxis.ValueConverter.GetDataObjectAt(_mainDualSlider.RangeStart) - dateTime;
      DateTime spanDate = new DateTime(span.Ticks);

      double logicalStart = Chart.XAxis.ValueConverter.GetAxisPlotPosition(spanDate);
      double ratio = (_mainDualSlider.RangeEnd - _mainDualSlider.RangeStart + buffer) / ActualWidth;
      double physicalStart = Math.Round(-logicalStart / ratio);

      double remainder = 0;
      double remainderDelta = physicalWidth - (int)physicalWidth;
      double x = physicalStart;
      while (x < ActualWidth)
      {
        double actualPhysicalWidth = physicalWidth - spacing;
        if (unit == TimeRangeUnit.Year)
        {
          actualPhysicalWidth = physicalWidth * 365 + (DateTime.IsLeapYear(dateTime.Year) ? 1 : 0) - spacing;
        }
        else if (unit == TimeRangeUnit.Month)
        {
          actualPhysicalWidth = physicalWidth * DateTime.DaysInMonth(dateTime.Year, dateTime.Month) - spacing;
        }
        TimeExplorerTimeUnit buttonModel = new TimeExplorerTimeUnit();
        buttonModel.Label = TimeExplorerUtils.GetLabel(unit, unitCount, dateTime, commonSize, ShortWeekName, FullWeekName);
        buttonModel.Width = Math.Max(0, (int)(actualPhysicalWidth + remainder));
        buttonModel.StartDateTime = dateTime;
        if (unit == TimeRangeUnit.Year)
        {
          buttonModel.EndDateTime = dateTime.AddYears(1);
        }
        else if (unit == TimeRangeUnit.Month)
        {
          buttonModel.EndDateTime = dateTime.AddMonths(1);
        }
        else
        {
          buttonModel.EndDateTime = dateTime + timeSpan;
        }

        remainder += remainderDelta;
        if (remainder > 1)
        {
          remainder -= 1;
        }

        buttonHost.Items.Add(buttonModel);
        double delta = buttonModel.Width + spacing;
        if (delta == 0)
        {
          break;
        }
        x += delta;
        if (unit == TimeRangeUnit.Year)
        {
          dateTime = dateTime.AddYears(1);
        }
        else if (unit == TimeRangeUnit.Month)
        {
          dateTime = dateTime.AddMonths(1);
        }
        else
        {
          dateTime += timeSpan;
        }
      }
      buttonHost.RenderTransform = new TranslateTransform() { X = physicalStart };
    }

    #endregion // time unit logic

    private double Spacing { get; set; }

    internal double ConvertLogicalToPhysical(double logicalPosition)
    {
      double physicalSize = Chart.ActualWidth;
      double logicalSize = _mainDualSlider.RangeEnd - _mainDualSlider.RangeStart + GetBuffer();
      double ratio = logicalSize / physicalSize;
      double physicalPosition = logicalPosition / ratio;
      return physicalPosition;
    }

    private double GetBuffer()
    {
      double minDelta = Chart.XAxis.MajorTickSpacing;
      if(Chart.Series.Count > 0 && Chart.Series[0].MinDelta != 0)
      {
        minDelta = Chart.Series[0].MinDelta;
      }
      return Chart.XAxis.LabelLayout == AxisLabelLayout.Inside ? minDelta : 0;
    }

    private double GetAxisPlotValue(DateTime dateTime)
    {
      return Chart.XAxis.ValueConverter.GetAxisPlotPosition(dateTime);
    }

    private DateTime GetDateTime(double axisvalue)
    {
      return Chart == null ? new DateTime() : (DateTime)Chart.XAxis.ValueConverter.GetDataObjectAt(axisvalue);
    }

    #region Chart property

    /// <summary>
    /// Gets or sets the <see cref="Chart"/> control to be hosted by this <see cref="TimeExplorer"/>.
    /// This is a dependency property.
    /// </summary>
    public Chart Chart
    {
      get { return (Chart)GetValue(ChartProperty); }
      set { SetValue(ChartProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Chart"/> property.
    /// </summary>
    public static readonly DependencyProperty ChartProperty =
      DependencyProperty.Register("Chart", typeof(Chart), typeof(TimeExplorer),
      new PropertyMetadata(new PropertyChangedCallback(OnChartChanged)));

    private static void OnChartChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((TimeExplorer)d).OnChartChanged(e);
    }

    private void OnChartChanged(DependencyPropertyChangedEventArgs e)
    {
      Chart oldChart = e.OldValue as Chart;
      if (oldChart != null && oldChart.XAxis != null)
      {
        oldChart.XAxis.RangeChanged -= new EventHandler(XAxis_RangeChanged);
      }
      if (Chart != null)
      {
        Chart.ZoomMode = ZoomMode.Horizontal;
        Chart.LegendPosition = LegendPosition.None;
        Chart.Padding = new Thickness(0);
        Chart.XAxis.ValueConverter = new DateTimeAxisValueConverter();
        Chart.XAxis.Visibility = Visibility.Hidden;
        Chart.XAxis.Height = 0;
        Chart.XAxis.RangeChanged += new EventHandler(XAxis_RangeChanged); // TODO: remove event handler
        Chart.YAxis.Visibility = Visibility.Hidden;
        Chart.YAxis.Width = 0;
        UpdateMinMaxValues();
      }
    }

    private bool _isMinMaxLoaded = false;
    private bool _initializationLock = false;

    private void XAxis_RangeChanged(object sender, EventArgs e)
    {
      // NOTE: This is assuming that when the minimum/maximum of the axis changes, the actualMinimum/maximum values will change causing this event to be raised.
      //       This is the current behavior, but this may change. If this behavior changes, this will effect the case when the min/max values change without changing the actual min/max values.
      if (Chart != null && !_initializationLock)
      {
        bool _wasLoaded = _isMinMaxLoaded;
        _isMinMaxLoaded = Chart.XAxis.MinimumValue != Chart.XAxis.MaximumValue;
        UpdateMinMaxValues();

        if (_isMinMaxLoaded)
        {
          if (_wasLoaded)
          {
            // Normal axis range changed operation.
            ViewportEndTime = (DateTime)Chart.XAxis.ActualMaximum;
            ViewportStartTime = (DateTime)Chart.XAxis.ActualMinimum;
          }
          else
          {
            // Logic for the very first time the min/max values are resolved:
            if (ViewportEndTime.Equals(ViewportStartTime))
            {
              // if the viewport range has not been set externally, set it from the chart range.
              ViewportEndTime = (DateTime)Chart.XAxis.ActualMaximum;
              ViewportStartTime = (DateTime)Chart.XAxis.ActualMinimum;
            }
            else
            {
              // Otherwise make sure the set viewport values are applied back to the chart range.
              _initializationLock = true;
              OnViewportEndTimeChanged();
              OnViewportStartTimeChanged();
              _initializationLock = false;
            }

            if (RangeEndTime.Equals(RangeStartTime))
            {
              // If the time range has not been externally set, set the time range to be the maximum range.
              RangeEndTime = (DateTime)Chart.XAxis.Maximum;
              RangeStartTime = (DateTime)Chart.XAxis.Minimum;
            }
            else
            {
              // Otherwise update the dual slider values with the current time range.
              OnRangeEndTimeChanged();
              OnRangeStartTimeChanged();
            }
          }
        }
      }
    }

    private void UpdateMinMaxValues()
    {
      if (Chart != null)
      {
        ChartAxis axis = Chart.XAxis;
        if (_mainDualSlider != null)
        {
          _mainDualSlider.Maximum = axis.MaximumValue;
          _mainDualSlider.Minimum = axis.MinimumValue;
        }
        if (_internalDualSlider != null)
        {
          _internalDualSlider.Maximum = axis.MaximumValue;
          _internalDualSlider.Minimum = axis.MinimumValue;
        }
      }
    }

    #endregion // Chart property

    #region RangeStartTime property

    /// <summary>
    /// Gets or sets the <see cref="DateTime"/> at the start of the time window.
    /// This is a dependency property.
    /// </summary>
    public DateTime RangeStartTime
    {
      get { return (DateTime)GetValue(RangeStartTimeProperty); }
      set { SetValue(RangeStartTimeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="RangeStartTime"/> property.
    /// </summary>
    public static readonly DependencyProperty RangeStartTimeProperty =
      DependencyProperty.Register("RangeStartTime", typeof(DateTime), typeof(TimeExplorer),
      new PropertyMetadata(new PropertyChangedCallback(OnRangeStartTimeChanged)));

    private static void OnRangeStartTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((TimeExplorer)d).OnRangeStartTimeChanged();
    }

    private void OnRangeStartTimeChanged()
    {
      if (!_settingTimeRangeInternal)
      {
        if (_internalDualSlider != null && Chart != null)
        {
          double start = GetAxisPlotValue(RangeStartTime);
          _internalDualSlider.RangeStart = start;
        }
      }
    }

    #endregion // RangeStartTime property

    #region RangeEndTime property

    /// <summary>
    /// Gets or sets the <see cref="DateTime"/> at the end of the time window.
    /// This is a dependency property.
    /// </summary>
    public DateTime RangeEndTime
    {
      get { return (DateTime)GetValue(RangeEndTimeProperty); }
      set { SetValue(RangeEndTimeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="RangeEndTime"/> property.
    /// </summary>
    public static readonly DependencyProperty RangeEndTimeProperty =
      DependencyProperty.Register("RangeEndTime", typeof(DateTime), typeof(TimeExplorer),
      new PropertyMetadata(new PropertyChangedCallback(OnRangeEndTimeChanged)));

    private static void OnRangeEndTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((TimeExplorer)d).OnRangeEndTimeChanged();
    }

    private void OnRangeEndTimeChanged()
    {
      if (!_settingTimeRangeInternal)
      {
        if (_internalDualSlider != null && Chart != null)
        {
          double end = GetAxisPlotValue(RangeEndTime);
          _internalDualSlider.RangeEnd = end;
        }
      }
    }

    #endregion // RangeEndTime property

    #region ViewportStartTime Property

    /// <summary>
    /// Gets or sets the <see cref="DateTime"/> at the start of the overall viewport.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ViewportStartTimeProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public DateTime ViewportStartTime
    {
      get { return (DateTime)GetValue(ViewportStartTimeProperty); }
      set { SetValue(ViewportStartTimeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ViewportStartTime"/> property.
    /// </summary>
    public static readonly DependencyProperty ViewportStartTimeProperty =
      DependencyProperty.Register("ViewportStartTime", typeof(DateTime), typeof(TimeExplorer),
      new FrameworkPropertyMetadata(OnViewportStartTimeChanged));

    private static void OnViewportStartTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((TimeExplorer)d).OnViewportStartTimeChanged();
    }

    private void OnViewportStartTimeChanged()
    {
      if (!_settingTimeRangeInternal)
      {
        if (_mainDualSlider != null && Chart != null)
        {
          _settingTimeRangeInternal = true;
          _mainDualSlider.RangeStart = GetAxisPlotValue(ViewportStartTime);
          Chart.XAxis.ActualMinimum = ViewportStartTime;
          _settingTimeRangeInternal = false;
        }
      }
    }

    #endregion // ViewportStartTime Property

    #region ViewportEndTime Property

    /// <summary>
    /// Gets or sets the <see cref="DateTime"/> at the end of the overall viewport.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ViewportEndTimeProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public DateTime ViewportEndTime
    {
      get { return (DateTime)GetValue(ViewportEndTimeProperty); }
      set { SetValue(ViewportEndTimeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ViewportEndTime"/> property.
    /// </summary>
    public static readonly DependencyProperty ViewportEndTimeProperty =
      DependencyProperty.Register("ViewportEndTime", typeof(DateTime), typeof(TimeExplorer),
      new FrameworkPropertyMetadata(OnViewportEndTimeChanged));

    private static void OnViewportEndTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((TimeExplorer)d).OnViewportEndTimeChanged();
    }

    private void OnViewportEndTimeChanged()
    {
      if (!_settingTimeRangeInternal)
      {
        if (_mainDualSlider != null && Chart != null)
        {
          _settingTimeRangeInternal = true;
          _mainDualSlider.RangeEnd = GetAxisPlotValue(ViewportEndTime);
          Chart.XAxis.ActualMaximum = ViewportEndTime;
          _settingTimeRangeInternal = false;
        }
      }
    }

    #endregion // ViewportEndTime Property

    #region CanEditRangeMagnitude Property

    /// <summary>
    /// Gets or sets whether or not the start and end thumbs of the range slider are enabled. The default is true.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="CanEditRangeMagnitudeProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool CanEditRangeMagnitude
    {
      get { return (bool)GetValue(CanEditRangeMagnitudeProperty); }
      set { SetValue(CanEditRangeMagnitudeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="CanEditRangeMagnitude"/> property.
    /// </summary>
    public static readonly DependencyProperty CanEditRangeMagnitudeProperty =
      DependencyProperty.Register("CanEditRangeMagnitude", typeof(bool), typeof(TimeExplorer),
      new FrameworkPropertyMetadata(true, OnCanEditRangeMagnitudeChanged));

    private static void OnCanEditRangeMagnitudeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((TimeExplorer)d).OnCanEditRangeMagnitudeChanged();
    }

    private void OnCanEditRangeMagnitudeChanged()
    {
    }

    #endregion // CanEditRangeMagnitude Property

    #region MinorButtonsVisibility Property

    /// <summary>
    /// Gets or sets the visibility of the minor date-time range buttons. The default is Visible.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="MinorRangeButtonsVisibilityProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public Visibility MinorRangeButtonsVisibility
    {
      get { return (Visibility)GetValue(MinorRangeButtonsVisibilityProperty); }
      set { SetValue(MinorRangeButtonsVisibilityProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="MinorRangeButtonsVisibility"/> property.
    /// </summary>
    public static readonly DependencyProperty MinorRangeButtonsVisibilityProperty =
      DependencyProperty.Register("MinorRangeButtonsVisibility", typeof(Visibility), typeof(TimeExplorer),
      new FrameworkPropertyMetadata(Visibility.Visible, OnMinorRangeButtonsVisibilityChanged));

    private static void OnMinorRangeButtonsVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((TimeExplorer)d).OnMinorRangeButtonsVisibilityChanged();
    }

    private void OnMinorRangeButtonsVisibilityChanged()
    {
      PopulateBackground();
    }

    #endregion // MinorButtonsVisibility Property

    #region MajorRangeButtonsVisibility Property

    /// <summary>
    /// Gets or sets the visibility of the major date-time range buttons. The default is Visible.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="MajorRangeButtonsVisibilityProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public Visibility MajorRangeButtonsVisibility
    {
      get { return (Visibility)GetValue(MajorRangeButtonsVisibilityProperty); }
      set { SetValue(MajorRangeButtonsVisibilityProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="MajorRangeButtonsVisibility"/> property.
    /// </summary>
    public static readonly DependencyProperty MajorRangeButtonsVisibilityProperty =
      DependencyProperty.Register("MajorRangeButtonsVisibility", typeof(Visibility), typeof(TimeExplorer),
      new FrameworkPropertyMetadata(Visibility.Visible, OnMajorRangeButtonsVisibilityChanged));

    private static void OnMajorRangeButtonsVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((TimeExplorer)d).OnMajorRangeButtonsVisibilityChanged();
    }

    private void OnMajorRangeButtonsVisibilityChanged()
    {
      PopulateBackground();
    }

    #endregion // MajorRangeButtonsVisibility Property

    #region ShortWeekName Property

    /// <summary>
    /// Gets or sets the string used for short-formatting week units, the default is "W".
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ShortWeekNameProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public string ShortWeekName
    {
      get { return (string)GetValue(ShortWeekNameProperty); }
      set { SetValue(ShortWeekNameProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ShortWeekName"/> property.
    /// </summary>
    public static readonly DependencyProperty ShortWeekNameProperty =
      DependencyProperty.Register("ShortWeekName", typeof(string), typeof(TimeExplorer),
      new FrameworkPropertyMetadata("W", OnShortWeekNameChanged));

    private static void OnShortWeekNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((TimeExplorer)d).OnShortWeekNameChanged();
    }

    private void OnShortWeekNameChanged()
    {
    }

    #endregion // ShortWeekName Property

    #region FullWeekName Property

    /// <summary>
    /// Gets or sets the string used for full-formatting week units, the default is "Week".
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="FullWeekNameProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public string FullWeekName
    {
      get { return (string)GetValue(FullWeekNameProperty); }
      set { SetValue(FullWeekNameProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="FullWeekName"/> property.
    /// </summary>
    public static readonly DependencyProperty FullWeekNameProperty =
      DependencyProperty.Register("FullWeekName", typeof(string), typeof(TimeExplorer),
      new FrameworkPropertyMetadata("Week", OnFullWeekNameChanged));

    private static void OnFullWeekNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((TimeExplorer)d).OnFullWeekNameChanged();
    }

    private void OnFullWeekNameChanged()
    {
    }

    #endregion // FullWeekName Property
  }

  internal enum TimeRangeUnit
  {
    Year,

    Month,

    Week,

    Day,

    Hour,

    Minute
  }
}
