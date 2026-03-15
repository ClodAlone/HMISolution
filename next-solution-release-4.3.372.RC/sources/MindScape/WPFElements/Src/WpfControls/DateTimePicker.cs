using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using Infralution.Licensing;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Reflection;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// A control for entering dates and times.
  /// </summary>
  [LicenseProvider(typeof(PublicEncryptedLicenseProvider))]
  public class DateTimePicker : Control
  {
    static DateTimePicker()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(DateTimePicker), 
        new FrameworkPropertyMetadata(typeof(DateTimePicker)));
    }

    private DateTimePickerModel _model = new DateTimePickerModel();

    /// <summary>
    /// Initializes a new instance of the <see cref="DateTimePicker"/> class.
    /// </summary>
    public DateTimePicker()
    {
      // Licensing
      //new WpfElementsCore(Assembly.GetCallingAssembly());
      //LicenseHelper.Attach(this, Assembly.GetCallingAssembly());
      // End licensing

      SetValue(DisplayElementsPropertyKey, _model.Text);
      if (DisplayElements != null)
      {
        DisplayElements.CollectionChanged += new NotifyCollectionChangedEventHandler(DisplayElements_CollectionChanged);
        foreach (DateTimeDisplayElement element in DisplayElements)
        {
          element.Foreground = Foreground;
        }
      }

      _model.ValueChanged += new EventHandler(Model_ValueChanged);
    }

    private void DisplayElements_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (e.NewItems != null)
      {
        foreach (DateTimeDisplayElement element in e.NewItems)
        {
          element.Foreground = Foreground;
        }
      }
    }

    void Model_ValueChanged(object sender, EventArgs e)
    {
      Value = _model.Value;
    }

    /// <summary>
    /// Called when a property value changes.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
      base.OnPropertyChanged(e);

      if (e.Property == Control.ForegroundProperty && DisplayElements != null)
      {
        foreach (DateTimeDisplayElement element in DisplayElements)
        {
          element.Foreground = Foreground;
        }
      }
    }

    /// <summary>
    /// Gets or sets the date/time value.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ValueProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>BindsTwoWayByDefault</td></tr>
    /// </table>
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
    public DateTime Value
    {
      get { return (DateTime)GetValue(ValueProperty); }
      set { SetValue(ValueProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Value"/> property.
    /// </summary>
    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register("Value", typeof(DateTime), typeof(DateTimePicker),
        new FrameworkPropertyMetadata(
          DateTime.Now,
          FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
          OnValueChanged));

    #region Minimum Property

    /// <summary>
    /// Gets or sets the minimum date that can be selected.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="MinimumProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public DateTime Minimum
    {
      get { return (DateTime)GetValue(MinimumProperty); }
      set { SetValue(MinimumProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Minimum"/> property.
    /// </summary>
    public static readonly DependencyProperty MinimumProperty =
      DependencyProperty.Register("Minimum", typeof(DateTime), typeof(DateTimePicker),
      new FrameworkPropertyMetadata(OnMinimumChanged));

    private static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DateTimePicker)d).OnMinimumChanged();
    }

    private void OnMinimumChanged()
    {
      _model.Minimum = Minimum;
    }

    #endregion // Minimum Property

    #region Maximum Property

    /// <summary>
    /// Gets or sets the maximum date that can be selected.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="MaximumProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public DateTime Maximum
    {
      get { return (DateTime)GetValue(MaximumProperty); }
      set { SetValue(MaximumProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Maximum"/> property.
    /// </summary>
    public static readonly DependencyProperty MaximumProperty =
      DependencyProperty.Register("Maximum", typeof(DateTime), typeof(DateTimePicker),
      new FrameworkPropertyMetadata(OnMaximumChanged));

    private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DateTimePicker)d).OnMaximumChanged();
    }

    private void OnMaximumChanged()
    {
      _model.Maximum = Maximum;
    }

    #endregion // Maximum Property

    /// <summary>
    /// Gets or sets the culture.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="CultureProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public CultureInfo Culture
    {
      get { return (CultureInfo)GetValue(CultureProperty); }
      set { SetValue(CultureProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Culture"/> property.
    /// </summary>
    public static readonly DependencyProperty CultureProperty =
        DependencyProperty.Register("Culture", typeof(CultureInfo), typeof(DateTimePicker),
        new FrameworkPropertyMetadata(
          CultureInfo.CurrentCulture,
          OnCultureChanged));


    /// <summary>
    /// Gets or sets the date/time format.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="FormatProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public DateTimePickerFormat Format
    {
      get { return (DateTimePickerFormat)GetValue(FormatProperty); }
      set { SetValue(FormatProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Format"/> property.
    /// </summary>
    public static readonly DependencyProperty FormatProperty =
        DependencyProperty.Register("Format", typeof(DateTimePickerFormat), typeof(DateTimePicker), 
        new FrameworkPropertyMetadata(OnFormatChanged));


    /// <summary>
    /// Gets or sets the custom date/time format string.  This is ignored unless <see cref="Format"/>
    /// is set to Custom.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="CustomFormatProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public string CustomFormat
    {
      get { return (string)GetValue(CustomFormatProperty); }
      set { SetValue(CustomFormatProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="CustomFormat"/> property.
    /// </summary>
    public static readonly DependencyProperty CustomFormatProperty =
        DependencyProperty.Register("CustomFormat", typeof(string), typeof(DateTimePicker), 
        new FrameworkPropertyMetadata(OnCustomFormatChanged));

    /// <summary>
    /// Gets the elements used to display and edit the value.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="DisplayElementsProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public ObservableCollection<DateTimeDisplayElement> DisplayElements
    {
      get { return (ObservableCollection<DateTimeDisplayElement>)GetValue(DisplayElementsProperty); }
    }

    internal static readonly DependencyPropertyKey DisplayElementsPropertyKey =
        DependencyProperty.RegisterReadOnly("DisplayElements", 
        typeof(ObservableCollection<DateTimeDisplayElement>),
        typeof(DateTimePicker), new FrameworkPropertyMetadata());

    /// <summary>
    /// Identifies the <see cref="DisplayElements"/> property.
    /// </summary>
    public static readonly DependencyProperty DisplayElementsProperty = 
      DisplayElementsPropertyKey.DependencyProperty;

    private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DateTimePicker)d).OnValueChanged();
    }

    private static void OnCultureChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DateTimePicker)d).OnCultureChanged();
    }

    private static void OnCustomFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DateTimePicker)d).OnCustomFormatChanged();
    }

    private static void OnFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DateTimePicker)d).OnFormatChanged();
    }

    private void OnValueChanged()
    {
      _model.Value = Value;
    }

    private void OnCultureChanged()
    {
      _model.Culture = Culture;
    }

    private void OnCustomFormatChanged()
    {
      _model.CustomFormat = CustomFormat;
    }

    private void OnFormatChanged()
    {
      _model.Format = Format;
    }
  }
}
