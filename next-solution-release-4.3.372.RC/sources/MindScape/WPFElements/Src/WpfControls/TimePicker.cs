using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Windows;
using System.ComponentModel;
using Infralution.Licensing;
using System.Windows.Input;
using System.Windows.Threading;
using System.Reflection;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Provides a user interface for selecting a time of day by entering a time
  /// or selecting from a drop-down list.
  /// </summary>
  [LicenseProvider(typeof(PublicEncryptedLicenseProvider))]
  public class TimePicker : TimePickerBase
  {
    private bool _hasFocus;

    static TimePicker()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(TimePicker),
        new FrameworkPropertyMetadata(typeof(TimePicker)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TimePicker"/> class.
    /// </summary>
    public TimePicker()
    {
      // Licensing
      //new WpfElementsCore(Assembly.GetCallingAssembly());
      //LicenseHelper.Attach(this, Assembly.GetCallingAssembly());
      // End licensing

      Loaded += new RoutedEventHandler(TimePicker_Loaded);
      Text = ConvertTimeOfDayToString(SelectedTime);
    }

    private void TimePicker_Loaded(object sender, RoutedEventArgs e)
    {
      if (ItemsSource == null)
      {
        AutoFillSuggestions();
      }
    }

    /// <summary>
    /// Called when this <see cref="TimePicker"/> loses keyboard focus.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
    {
      base.OnLostKeyboardFocus(e);

      _hasFocus = false;
      OnTextChanged();
    }

    /// <summary>
    /// Called when this <see cref="TimePicker"/> gets keyboard focus.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
    {
      base.OnGotKeyboardFocus(e);

      _hasFocus = true;
    }

    private void AutoFillSuggestions()
    {
      ItemsSource = CreateTimeList(MinTime.Hour, MinTime.Minute);
    }

    private IList<TimeOfDay> CreateTimeList(int startHour, int startMinute)
    {
      IList<TimeOfDay> times = new List<TimeOfDay>();
      int hour = startHour;
      int min = startMinute;
      while (true)
      {
        TimeOfDay nextTime = new TimeOfDay(hour, min);
        if (nextTime > MaxTime)
        {
          break;
        }
        times.Add(nextTime);
        min += (int)TimeSuggestionInterval.TotalMinutes;
        while (min >= 60)
        {
          min -= 60;
          hour++;
          if (hour >= 24)
          {
            break;
          }
        }
      }

      return times;
    }

    /// <summary>
    /// Increases the <see cref="SelectedTime"/> based on the Change property.
    /// </summary>
    public override void Increase()
    {
      int deltahours = (int)(Change.TotalMinutes / 60);
      int deltaMins = (int)(Change.TotalMinutes - (deltahours * 60));
      int hour = SelectedTime.Hour;
      int min = SelectedTime.Minute + deltaMins;
      if (min >= 60)
      {
        min -= 60;
        hour++;
      }
      hour += deltahours;
      if (hour >= 24)
      {
        hour -= 24;
      }
      SelectedTime = new TimeOfDay(hour, min);
    }

    /// <summary>
    /// Decreases the <see cref="SelectedTime"/> based on the Change property.
    /// </summary>
    public override void Decrease()
    {
      int deltahours = (int)(Change.TotalMinutes / 60);
      int deltaMins = (int)(Change.TotalMinutes - (deltahours * 60));
      int hour = SelectedTime.Hour;
      int min = SelectedTime.Minute - deltaMins;
      if (min < 0)
      {
        min = 60 + min;
        hour--;
      }
      hour -= deltahours;
      if (hour < 0)
      {
        hour = 24 + hour;
      }
      SelectedTime = new TimeOfDay(hour, min);
    }

    private string ConvertTimeOfDayToString(TimeOfDay timeOfDay)
    {
      CultureInfo culture = CultureInfo.CurrentUICulture;
      string s = timeOfDay.TwelveHourClockHour + ":";
      if (TimeDisplayMode == TimeDisplayMode.TwentyFourHourTime)
      {
        s = "";
        if (timeOfDay.Hour < 10)
        {
          s = "0";
        }
        s += timeOfDay.Hour + ":";
      }
      s += timeOfDay.Minute.ToString("D2", culture);
      if (TimeDisplayMode == TimeDisplayMode.TwelveHourTime)
      {
        if (timeOfDay.Hour < 12)
        {
          s += culture.DateTimeFormat.AMDesignator;
        }
        else
        {
          s += culture.DateTimeFormat.PMDesignator;
        }
      }
      return s;
    }

    private TimeOfDay ConvertStringToTimeOfDay(string s)
    {
      int colonIndex = s.IndexOf(':');
      if (colonIndex > 0)
      {
        string hourString = s.Substring(0, colonIndex);
        try
        {
          int hour = Int32.Parse(hourString, CultureInfo.CurrentUICulture);
          int maxHour = 24; // TimeDisplayMode == TimeDisplayMode.TwelveHourTime ? 13 : 24;
          int minHour = -1; // TimeDisplayMode == TimeDisplayMode.TwelveHourTime ? 0 : -1;
          if (hour > minHour && hour < maxHour)
          {
            if (s.Length >= colonIndex + 3)
            {
              string minuteString = s.Substring(colonIndex + 1, 2);
              try
              {
                CultureInfo culture = CultureInfo.CurrentUICulture;
                int minute = Int32.Parse(minuteString, culture);
                if (s.Length > colonIndex + 3 && TimeDisplayMode == TimeDisplayMode.TwelveHourTime)
                {
                  string designator = s.Substring(colonIndex + 3);
                  if (designator.Equals(culture.DateTimeFormat.PMDesignator))
                  {
                    if (hour < 12)
                    {
                      hour += 12;
                    }
                  }
                  else
                  {
                    if (hour == 12)
                    {
                      hour = 0;
                    }
                  }
                }
                return new TimeOfDay(hour, minute);
              }
              catch (FormatException) { }
            }
          }
        }
        catch (FormatException) { }
      }
      return new TimeOfDay(0, 0);
    }

    #region TimeDisplayMode Property

    /// <summary>
    /// Gets or sets the TimeDisplayMode that specifies how to display time.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="TimeDisplayModeProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public TimeDisplayMode TimeDisplayMode
    {
      get { return (TimeDisplayMode)GetValue(TimeDisplayModeProperty); }
      set { SetValue(TimeDisplayModeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="TimeDisplayMode"/> property.
    /// </summary>
    public static readonly DependencyProperty TimeDisplayModeProperty =
      DependencyProperty.Register("TimeDisplayMode", typeof(TimeDisplayMode), typeof(TimePicker),
      new FrameworkPropertyMetadata(TimeDisplayMode.TwelveHourTime, OnTimeDisplayModeChanged));

    private static void OnTimeDisplayModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((TimePicker)d).OnTimeDisplayModeChanged();
    }

    private void OnTimeDisplayModeChanged()
    {
      Text = ConvertTimeOfDayToString(SelectedTime);
    }

    #endregion // TimeDisplayMode Property

    #region SelectedTime property

    /// <summary>
    /// Gets or sets the selected <see cref="TimeOfDay"/>.
    /// This is a dependency property.
    /// </summary>
    public TimeOfDay SelectedTime
    {
      get { return (TimeOfDay)GetValue(SelectedTimeProperty); }
      set { SetValue(SelectedTimeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="SelectedTime"/> property.
    /// </summary>
    public static readonly DependencyProperty SelectedTimeProperty =
      DependencyProperty.Register("SelectedTime", typeof(TimeOfDay), typeof(TimePicker),
      new FrameworkPropertyMetadata(new TimeOfDay(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedTimeChanged));

    /// <summary>
    /// Raised when the <see cref="SelectedTime"/> changes.
    /// </summary>
    public event RoutedEventHandler SelectedTimeChanged;

    private static void OnSelectedTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((TimePicker)d).OnSelectedTimeChanged();
    }

    private void OnSelectedTimeChanged()
    {
      if (SelectedTime < MinTime)
      {
        SelectedTime = MinTime;
      }
      else if (SelectedTime > MaxTime)
      {
        SelectedTime = MaxTime;
      }
      else
      {
        Text = ConvertTimeOfDayToString(SelectedTime);
        IsDropDownOpen = false;
        RoutedEventHandler handler = SelectedTimeChanged;
        if (handler != null)
        {
          handler(this, new RoutedEventArgs());
        }
      }
    }

    #endregion // SelectedTime property

    #region MaxTime Property

    /// <summary>
    /// Gets or sets the maximum time of day to be displayed in the <see cref="TimePicker"/>.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="MaxTimeProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public TimeOfDay MaxTime
    {
      get { return (TimeOfDay)GetValue(MaxTimeProperty); }
      set { SetValue(MaxTimeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="MaxTime"/> property.
    /// </summary>
    public static readonly DependencyProperty MaxTimeProperty =
      DependencyProperty.Register("MaxTime", typeof(TimeOfDay), typeof(TimePicker),
      new FrameworkPropertyMetadata(new TimeOfDay(23, 59), OnMaxTimeChanged));

    private static void OnMaxTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((TimePicker)d).OnMaxTimeChanged();
    }

    private void OnMaxTimeChanged()
    {
      if (MaxTime < MinTime)
      {
        MinTime = MaxTime;
      }
      if (SelectedTime > MaxTime)
      {
        SelectedTime = MaxTime;
      }
      if (IsAutoFillSuggestionsEnabled)
      {
        AutoFillSuggestions();
      }
    }

    #endregion // MaxTime Property

    #region MinTime Property

    /// <summary>
    /// Gets or sets the minimum time of day to be displayed in the <see cref="TimePicker"/>.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="MinTimeProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public TimeOfDay MinTime
    {
      get { return (TimeOfDay)GetValue(MinTimeProperty); }
      set { SetValue(MinTimeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="MinTime"/> property.
    /// </summary>
    public static readonly DependencyProperty MinTimeProperty =
      DependencyProperty.Register("MinTime", typeof(TimeOfDay), typeof(TimePicker),
      new FrameworkPropertyMetadata(new TimeOfDay(0, 0), OnMinTimeChanged));

    private static void OnMinTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((TimePicker)d).OnMinTimeChanged();
    }

    private void OnMinTimeChanged()
    {
      if (MinTime > MaxTime)
      {
        MaxTime = MinTime;
      }
      if (SelectedTime < MinTime)
      {
        SelectedTime = MinTime;
      }
      if (IsAutoFillSuggestionsEnabled)
      {
        AutoFillSuggestions();
      }
    }

    #endregion // MinTime Property

    #region TimeSuggestionInterval Property

    /// <summary>
    /// Gets or sets the interval between times when automatically filling the suggestion list.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="TimeSuggestionIntervalProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public TimeSpan TimeSuggestionInterval
    {
      get { return (TimeSpan)GetValue(TimeSuggestionIntervalProperty); }
      set { SetValue(TimeSuggestionIntervalProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="TimeSuggestionInterval"/> property.
    /// </summary>
    public static readonly DependencyProperty TimeSuggestionIntervalProperty =
      DependencyProperty.Register("TimeSuggestionInterval", typeof(TimeSpan), typeof(TimePicker),
      new FrameworkPropertyMetadata(new TimeSpan(0, 30, 0), OnTimeSuggestionIntervalChanged));

    private static void OnTimeSuggestionIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((TimePicker)d).OnTimeSuggestionIntervalChanged();
    }

    private void OnTimeSuggestionIntervalChanged()
    {
      if (IsAutoFillSuggestionsEnabled)
      {
        AutoFillSuggestions();
      }
    }

    #endregion // TimeSuggestionInterval Property

    #region IsAutoFillSuggestionsEnabled Property

    /// <summary>
    /// Gets or sets whether or not this control should automatically fill the suggestion list when the MinTime, MaxTime or TimeSuggestionInterval
    /// properties change. The default is false.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsAutoFillSuggestionsEnabledProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsAutoFillSuggestionsEnabled
    {
        get { return (bool)GetValue(IsAutoFillSuggestionsEnabledProperty); }
        set { SetValue(IsAutoFillSuggestionsEnabledProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsAutoFillSuggestionsEnabled"/> property.
    /// </summary>
    public static readonly DependencyProperty IsAutoFillSuggestionsEnabledProperty =
      DependencyProperty.Register("IsAutoFillSuggestionsEnabled", typeof(bool), typeof(TimePicker),
      new FrameworkPropertyMetadata(false, OnIsAutoFillSuggestionsEnabledChanged));

    private static void OnIsAutoFillSuggestionsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((TimePicker)d).OnIsAutoFillSuggestionsEnabledChanged();
    }

    private void OnIsAutoFillSuggestionsEnabledChanged()
    {
      if (IsAutoFillSuggestionsEnabled)
      {
        AutoFillSuggestions();
      }
    }

    #endregion // IsAutoFillSuggestionsEnabled Property

    #region IsMaskEnabled Property

    /// <summary>
    /// Gets or sets whether or not to use a mask for the text input of this <see cref="TimePicker"/>.
    /// The default is false.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsMaskEnabledProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsMaskEnabled
    {
      get { return (bool)GetValue(IsMaskEnabledProperty); }
      set { SetValue(IsMaskEnabledProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsMaskEnabled"/> property.
    /// </summary>
    public static readonly DependencyProperty IsMaskEnabledProperty =
      DependencyProperty.Register("IsMaskEnabled", typeof(bool), typeof(TimePicker),
      new FrameworkPropertyMetadata(false));

    #endregion // IsMaskEnabled Property

    #region Text property

    /// <summary>
    /// Gets or sets the text displayed in the <see cref="TimePicker"/>.
    /// This is a dependency property.
    /// </summary>
    public string Text
    {
      get { return (string)GetValue(TextProperty); }
      set { SetValue(TextProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Text"/> property.
    /// </summary>
    public static readonly DependencyProperty TextProperty =
      DependencyProperty.Register("Text", typeof(string), typeof(TimePicker),
      new PropertyMetadata(new PropertyChangedCallback(OnTextChanged)));

    private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((TimePicker)d).OnTextChanged();
    }

    private void OnTextChanged()
    {
      if (!_hasFocus && IsLoaded)
      {
        TimeOfDay timeOfDay = ConvertStringToTimeOfDay(Text);
        SelectedTime = timeOfDay;
        Text = ConvertTimeOfDayToString(SelectedTime);
        IsDropDownOpen = false;
        Dispatcher.BeginInvoke(DispatcherPriority.Normal, new RaiseTextChangedDelegate(RaiseTextChanged));
      }
    }

    private delegate void RaiseTextChangedDelegate();

    private void RaiseTextChanged()
    {
      OnPropertyChanged("Text");
    }

    #endregion // Text property

    #region ItemsSource property

    /// <summary>
    /// Gets or sets the collection of times to be displayed in the drop-down part of the control.
    /// This is a dependency property.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification="ItemsSource DP idiom")]
    public IList<TimeOfDay> ItemsSource
    {
      get { return (IList<TimeOfDay>)GetValue(ItemsSourceProperty); }
      set { SetValue(ItemsSourceProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ItemsSource"/> property.
    /// </summary>
    public static readonly DependencyProperty ItemsSourceProperty =
      DependencyProperty.Register("ItemsSource", typeof(IList<TimeOfDay>), typeof(TimePicker),
      null);

    #endregion // ItemsSource property
  }

  /// <summary>
  /// Specifies how to visualy display time.
  /// </summary>
  public enum TimeDisplayMode
  {
    /// <summary>
    /// Time should be displayed in twelve hour time. (e.g. 3:00PM)
    /// </summary>
    TwelveHourTime,

    /// <summary>
    /// Time should be displayed in twenty four hour time. (e.g. 15:00)
    /// </summary>
    TwentyFourHourTime
  }
}
