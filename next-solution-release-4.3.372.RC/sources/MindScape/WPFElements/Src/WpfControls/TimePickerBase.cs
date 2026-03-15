using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System.ComponentModel;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// A base class for time picker controls that have a drop-down part and up/down functionality.
  /// </summary>
  public abstract class TimePickerBase : Control, INotifyPropertyChanged
  {
    static TimePickerBase()
    {
      EventManager.RegisterClassHandler(typeof(TimePickerBase), TimePickerBase.PreviewMouseDownEvent,
        new MouseButtonEventHandler(OnPreviewMouseButtonDown));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TimePickerBase"/> class.
    /// </summary>
    protected TimePickerBase()
    {
      CommandBindings.Add(new CommandBinding(SpinCommands.Increase, Increase_Executed));
      CommandBindings.Add(new CommandBinding(SpinCommands.Decrease, Decrease_Executed));
      LostFocus += new RoutedEventHandler(TimePickerBase_LostFocus);
      PreviewKeyDown += new KeyEventHandler(TimePickerBase_PreviewKeyDown);
    }

    private void TimePickerBase_PreviewKeyDown(object sender, KeyEventArgs e)
    {
      if (!IsReadOnly)
      {
        switch (e.Key)
        {
          case Key.Up:
            Increase();
            break;
          case Key.Down:
            Decrease();
            break;
        }
      }
    }

    private void TimePickerBase_LostFocus(object sender, RoutedEventArgs e)
    {
      IsDropDownOpen = false;
    }

    private void Increase_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      Increase();
    }

    private void Decrease_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      Decrease();
    }

    /// <summary>
    /// Increases the selected time based on the <see cref="Change"/> property.
    /// </summary>
    public abstract void Increase();

    /// <summary>
    /// Decreases the selected time based on the <see cref="Change"/> property.
    /// </summary>
    public abstract void Decrease();

    private static void OnPreviewMouseButtonDown(object sender, MouseButtonEventArgs e)
    {
      TimePickerBase control = (TimePickerBase)sender;

      if (Mouse.Captured == control && e.OriginalSource == control)
      {
        control.IsDropDownOpen = false;
      }
    }

    #region IsDropDownOpen property

    /// <summary>
    /// Gets or sets whether the drop-down part of the <see cref="TimePickerBase"/> is open.
    /// This is a dependency property.
    /// </summary>
    public bool IsDropDownOpen
    {
      get { return (bool)GetValue(IsDropDownOpenProperty); }
      set { SetValue(IsDropDownOpenProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsDropDownOpen"/> property.
    /// </summary>
    public static readonly DependencyProperty IsDropDownOpenProperty =
      DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(TimePickerBase),
      new FrameworkPropertyMetadata(OnIsDropDownOpenChanged));

    private static void OnIsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      TimePickerBase control = (TimePickerBase)d;
      bool isDropDownOpen = (bool)(e.NewValue);

      if (isDropDownOpen)
      {
        Mouse.Capture(control, CaptureMode.SubTree);
        //control.Focus();
      }
      else
      {
        if (Mouse.Captured == control)
        {
          Mouse.Capture(null);
        }
      }
    }

    #endregion // IsDropDownOpen property

    #region Change property

    /// <summary>
    /// Gets or sets a <see cref="TimeSpan"/> used to increase or decrease the selected time.
    /// The default is 30 minutes.
    /// This is a dependency property.
    /// </summary>
    public TimeSpan Change
    {
      get { return (TimeSpan)GetValue(ChangeProperty); }
      set { SetValue(ChangeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Change"/> property.
    /// </summary>
    public static readonly DependencyProperty ChangeProperty =
      DependencyProperty.Register("Change", typeof(TimeSpan), typeof(TimePickerBase),
      new PropertyMetadata(new TimeSpan(0, 30, 0)));

    #endregion // Change property

    #region IsUpDownVisible property

    /// <summary>
    /// Gets or sets whether or not the up/down controls are visible.
    /// This is a dependency property.
    /// </summary>
    public bool IsUpDownVisible
    {
      get { return (bool)GetValue(IsUpDownVisibleProperty); }
      set { SetValue(IsUpDownVisibleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsUpDownVisible"/> property.
    /// </summary>
    public static readonly DependencyProperty IsUpDownVisibleProperty =
      DependencyProperty.Register("IsUpDownVisible", typeof(bool), typeof(TimePickerBase),
      new PropertyMetadata(false));

    #endregion // IsUpDownVisible property

    #region IsDropDownToggleVisible property

    /// <summary>
    /// Gets or sets whether or not the drop-down toggle button is visible or not.
    /// This is a dependency property.
    /// </summary>
    public bool IsDropDownToggleVisible
    {
      get { return (bool)GetValue(IsDropDownToggleVisibleProperty); }
      set { SetValue(IsDropDownToggleVisibleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsDropDownToggleVisible"/> property.
    /// </summary>
    public static readonly DependencyProperty IsDropDownToggleVisibleProperty =
      DependencyProperty.Register("IsDropDownToggleVisible", typeof(bool), typeof(TimePickerBase),
      new PropertyMetadata(true));

    #endregion // IsDropDownToggleVisible property

    #region IsReadOnly Property

    /// <summary>
    /// Gets or sets whether or not the text box part of this control is read only.
    /// The default is false.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsReadOnlyProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsReadOnly
    {
      get { return (bool)GetValue(IsReadOnlyProperty); }
      set { SetValue(IsReadOnlyProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsReadOnly"/> property.
    /// </summary>
    public static readonly DependencyProperty IsReadOnlyProperty =
      DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(TimePickerBase),
      new FrameworkPropertyMetadata(false));

    #endregion // IsReadOnly Property

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="propertyName">The name of the property whose value has changed.</param>
    protected virtual void OnPropertyChanged(string propertyName)
    {
      if (PropertyChanged != null)
      {
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }
  }
}
