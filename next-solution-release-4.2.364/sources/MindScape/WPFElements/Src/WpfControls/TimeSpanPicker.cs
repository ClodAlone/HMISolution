using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.ComponentModel;
using Infralution.Licensing;
using System.Globalization;
using System.Windows.Threading;
using System.Reflection;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Provides a user interface for selecting a duration or <see cref="TimeSpan"/>
  /// by entering a value directly or selecting from a drop-down list.
  /// </summary>
  [LicenseProvider(typeof(PublicEncryptedLicenseProvider))]
  public class TimeSpanPicker : TimePickerBase
  {
    private bool _isInitiatingTextChangeFromTimeSpanChange;

    static TimeSpanPicker()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(TimeSpanPicker),
        new FrameworkPropertyMetadata(typeof(TimeSpanPicker)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TimeSpanPicker"/> class.
    /// </summary>
    public TimeSpanPicker()
    {
      // Licensing
      //new WpfElementsCore(Assembly.GetCallingAssembly());
      //LicenseHelper.Attach(this, Assembly.GetCallingAssembly());
      // End licensing

      ItemsSource = CreateDurationList();
      try
      {
        _isInitiatingTextChangeFromTimeSpanChange = true;
        Text = DateTimeUtils.ConvertTimeSpanToString(SelectedTimeSpan);
      }
      finally
      {
        _isInitiatingTextChangeFromTimeSpanChange = false;
      }
    }

    /// <summary>
    /// Increases the <see cref="SelectedTimeSpan"/> based on the Change property.
    /// </summary>
    public override void Increase()
    {
      SelectedTimeSpan += Change;
    }

    /// <summary>
    /// Decreases the <see cref="SelectedTimeSpan"/> based on the Change property.
    /// </summary>
    public override void Decrease()
    {
      SelectedTimeSpan -= Change;
    }

    private static IList<TimeSpan> CreateDurationList()
    {
      IList<TimeSpan> timeSpans = new List<TimeSpan>();
      timeSpans.Add(new TimeSpan(0, 0, 0));
      timeSpans.Add(new TimeSpan(0, 5, 0));
      timeSpans.Add(new TimeSpan(0, 10, 0));
      timeSpans.Add(new TimeSpan(0, 15, 0));
      timeSpans.Add(new TimeSpan(0, 30, 0));
      for (int i = 1; i < 13; i++)
      {
        timeSpans.Add(new TimeSpan(i, 0, 0));
      }
      timeSpans.Add(new TimeSpan(18, 0, 0));
      for (int i = 1; i < 5; i++)
      {
        timeSpans.Add(new TimeSpan(i, 0, 0, 0));
      }
      timeSpans.Add(new TimeSpan(7, 0, 0, 0));
      timeSpans.Add(new TimeSpan(14, 0, 0, 0));
      return timeSpans;
    }

    #region SelectedTimeSpan property

    /// <summary>
    /// Gets or sets the selected TimeSpan.
    /// This is a dependency property.
    /// </summary>
    public TimeSpan SelectedTimeSpan
    {
      get { return (TimeSpan)GetValue(SelectedTimeSpanProperty); }
      set { SetValue(SelectedTimeSpanProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="SelectedTimeSpan"/> property.
    /// </summary>
    public static readonly DependencyProperty SelectedTimeSpanProperty =
      DependencyProperty.Register("SelectedTimeSpan", typeof(TimeSpan), typeof(TimeSpanPicker),
      new FrameworkPropertyMetadata(TimeSpan.Zero, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedTimeSpanChanged));

    /// <summary>
    /// Raised when the <see cref="SelectedTimeSpan"/> changes.
    /// </summary>
    public event RoutedEventHandler SelectedTimeSpanChanged;

    private static void OnSelectedTimeSpanChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((TimeSpanPicker)d).OnSelectedTimeSpanChanged();
    }

    private void OnSelectedTimeSpanChanged()
    {
      try
      {
        _isInitiatingTextChangeFromTimeSpanChange = true;
        Text = DateTimeUtils.ConvertTimeSpanToString(SelectedTimeSpan);
      }
      finally
      {
        _isInitiatingTextChangeFromTimeSpanChange = false;
      }
      IsDropDownOpen = false;
      RoutedEventHandler handler = SelectedTimeSpanChanged;
      if (handler != null)
      {
        handler(this, new RoutedEventArgs());
      }
    }

    #endregion // SelectedTimeSpan property

    #region Text property

    /// <summary>
    /// Gets or sets the text displayed in the <see cref="TimeSpanPicker"/>.
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
      DependencyProperty.Register("Text", typeof(string), typeof(TimeSpanPicker),
      new FrameworkPropertyMetadata(OnTextChanged));

    private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((TimeSpanPicker)d).OnTextChanged();
    }

    private void OnTextChanged()
    {
      if (!_isInitiatingTextChangeFromTimeSpanChange)
      {
        TimeSpan timeSpan = DateTimeUtils.ConvertStringToTimeSpan(Text);
        SelectedTimeSpan = timeSpan;
        Text = DateTimeUtils.ConvertTimeSpanToString(timeSpan);
      }
      IsDropDownOpen = false;
      Dispatcher.BeginInvoke(DispatcherPriority.Normal, new RaiseTextChangedDelegate(RaiseTextChanged));
    }

    private delegate void RaiseTextChangedDelegate();

    private void RaiseTextChanged()
    {
      OnPropertyChanged("Text");
    }

    #endregion // Text property

    #region ItemsSource property

    /// <summary>
    /// Gets or sets the collection of <see cref="TimeSpan"/> values offered in the drop-down
    /// part of the <see cref="TimeSpanPicker"/>.
    /// This is a dependency property.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification="DP ItemsSource idiom")]
    public IList<TimeSpan> ItemsSource
    {
      get { return (IList<TimeSpan>)GetValue(ItemsSourceProperty); }
      set { SetValue(ItemsSourceProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ItemsSource"/> property.
    /// </summary>
    public static readonly DependencyProperty ItemsSourceProperty =
      DependencyProperty.Register("ItemsSource", typeof(IList<TimeSpan>), typeof(TimeSpanPicker),
      null);

    #endregion // ItemsSource property
  }
}
