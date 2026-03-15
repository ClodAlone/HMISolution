using System;
using System.ComponentModel;
using Infralution.Licensing;
using System.Windows;
using System.Globalization;
using System.Windows.Controls;
using System.Reflection;
using System.Windows.Controls.Primitives;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// A control for entering dates, with support for a drop-down calendar.
  /// </summary>
  [LicenseProvider(typeof(PublicEncryptedLicenseProvider))]
  public class DropDownDatePicker : Control
  {
    static DropDownDatePicker()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(DropDownDatePicker),
        new FrameworkPropertyMetadata(typeof(DropDownDatePicker)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DropDownDatePicker"/> class.
    /// </summary>
    public DropDownDatePicker()
    {
      // Licensing
      //new WpfElementsCore(Assembly.GetCallingAssembly());
      //LicenseHelper.Attach(this, Assembly.GetCallingAssembly());
      // End licensing
    }

    private DropDownEditBox _editBox;

    /// <summary>
    /// Called by the framework when the control template is applied.
    /// </summary>
    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      _editBox = VisualTreeUtils.GetChild<DropDownEditBox>(this);
      if (_editBox != null)
      {
        _editBox.IsDropDownOpenChanged += new EventHandler(EditBox_IsDropDownOpenChanged);
      }
    }

    private void EditBox_IsDropDownOpenChanged(object sender, EventArgs e)
    {
      DropDownEditBox editBox = sender as DropDownEditBox;
      if (editBox != null)
      {
        if (editBox.IsDropDownOpen)
        {
          _popup = VisualTreeUtils.GetChild<Popup>(this);
          Dispatcher.BeginInvoke(new Action(TryFocusCalendar));
        }
        else
        {
          _calendar.KeyDown -= new System.Windows.Input.KeyEventHandler(Calendar_KeyDown);
        }
      }
    }

    private Popup _popup;
    private MonthCalendar _calendar;

    private void TryFocusCalendar()
    {
      if (_popup != null && _popup.Child != null)
      {
        _calendar = VisualTreeUtils.GetChild<MonthCalendar>(_popup.Child);
        if (_calendar != null)
        {
          _calendar.Focus();
          _calendar.KeyDown += new System.Windows.Input.KeyEventHandler(Calendar_KeyDown);
        }
      }
    }

    private void Calendar_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
      if (e.Key == System.Windows.Input.Key.Enter && _editBox != null)
      {
        _editBox.IsDropDownOpen = false;
      }
    }

    /// <summary>
    /// Gets or sets the date value.
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
        DateTimePicker.ValueProperty.AddOwner(typeof(DropDownDatePicker),
        new FrameworkPropertyMetadata(
          DateTime.Now,
          FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

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
        DateTimePicker.CultureProperty.AddOwner(typeof(DropDownDatePicker));

    /// <summary>
    /// Gets or sets the date format.
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
        DateTimePicker.FormatProperty.AddOwner(typeof(DropDownDatePicker));


    /// <summary>
    /// Gets or sets the custom date format string.  This is ignored unless <see cref="Format"/>
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
        DateTimePicker.CustomFormatProperty.AddOwner(typeof(DropDownDatePicker));


    /// <summary>
    /// Gets or sets how the time of day is set when the user selects the
    /// Today button.  The default is <see cref="F:TodayButtonTimeAction.Zero"/>.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="TodayButtonTimeActionProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public TodayButtonTimeAction TodayButtonTimeAction
    {
      get { return (TodayButtonTimeAction)GetValue(TodayButtonTimeActionProperty); }
      set { SetValue(TodayButtonTimeActionProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="TodayButtonTimeAction"/> property.
    /// </summary>
    public static readonly DependencyProperty TodayButtonTimeActionProperty =
      MonthCalendar.TodayButtonTimeActionProperty.AddOwner(typeof(DropDownDatePicker));

    private DateTime Constrained(DateTime date)
    {
      if (date < Minimum)
      {
        return Minimum;
      }
      else if (date > Maximum)
      {
        return Maximum;
      }
      return date;
    }

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
        MonthCalendar.MinimumProperty.AddOwner(typeof(DropDownDatePicker),
        new FrameworkPropertyMetadata(DateTime.MinValue, OnRangeConstraintChanged));
    
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
        MonthCalendar.MaximumProperty.AddOwner(typeof(DropDownDatePicker),
        new FrameworkPropertyMetadata(DateTime.MaxValue, OnRangeConstraintChanged));


    private static void OnRangeConstraintChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      DropDownDatePicker calendar = (DropDownDatePicker)d;
      calendar.Value = calendar.Constrained(calendar.Value);
    }

    /// <summary>
    /// Gets or sets whether the <see cref="DropDownDatePicker"/> automatically
    /// closes after the user makes a selection from the drop-down.  The default is false.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="CloseOnSelectProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool CloseOnSelect
    {
      get { return (bool)GetValue(CloseOnSelectProperty); }
      set { SetValue(CloseOnSelectProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="CloseOnSelect"/> property.
    /// </summary>
    public static readonly DependencyProperty CloseOnSelectProperty =
      DependencyProperty.Register("CloseOnSelect", typeof(bool), typeof(DropDownDatePicker),
      new FrameworkPropertyMetadata(false));
  }
}
