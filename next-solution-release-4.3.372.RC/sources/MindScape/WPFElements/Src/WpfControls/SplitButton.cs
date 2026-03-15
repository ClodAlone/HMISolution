using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Collections.Specialized;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using Infralution.Licensing;
using System.Reflection;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// A button which also provides an optional drop-down menu for selecting  alternate commands.
  /// </summary>
  [LicenseProvider(typeof(PublicEncryptedLicenseProvider))]
  public class SplitButton : Button
  {
    private readonly ObservableCollection<object> _dropDownItems = new ObservableCollection<object>();

    static SplitButton()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitButton),
        new FrameworkPropertyMetadata(typeof(SplitButton)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SplitButton"/> class.
    /// </summary>
    public SplitButton()
    {
      // Licensing
      //new WpfElementsCore(Assembly.GetCallingAssembly());
      //LicenseHelper.Attach(this, Assembly.GetCallingAssembly());
      // End licensing
    }

    /// <summary>
    /// Called when the <see cref="SplitButton"/> loses focus.
    /// </summary>
    /// <param name="e"><see cref="RoutedEventArgs"/> containing the event data.</param>
    protected override void OnLostFocus(RoutedEventArgs e)
    {
      base.OnGotFocus(e);

      IsDropDownOpen = false;
    }

    /// <summary>
    /// Called when the left mouse button is pressed over the <see cref="SplitButton"/>.
    /// </summary>
    /// <param name="e"><see cref="MouseButtonEventArgs"/> containing the event data.</param>
    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
      base.OnMouseLeftButtonDown(e);

      IsDropDownOpen = false;
    }

    /// <summary>
    /// Called by the framework when the control template is applied.
    /// </summary>
    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      ToggleButton button = GetTemplateChild("PART_ToggleButton") as ToggleButton;
      if (button != null)
      {
        button.Click += new RoutedEventHandler(button_Click);
      }
    }

    private void button_Click(object sender, RoutedEventArgs e)
    {
      /*ToggleButton button = sender as ToggleButton;
      if (_wasOpen)
      {
        button.IsChecked = false;
        IsDropDownOpen = false;
      }*/
      e.Handled = true;
      //_wasOpen = (bool)button.IsChecked;
    }

    /// <summary>
    /// Called when this <see cref="SplitButton"/> is clicked.
    /// </summary>
    protected override void OnClick()
    {
      if (!IsDropDownOpen)
      {
        base.OnClick();
      }
    }

    #region IsDropDownOpen property

    /// <summary>
    /// Gets or sets the whether the drop-down menu part of the <see cref="SplitButton"/> is open.
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
      DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(SplitButton),
      null);

    #endregion // IsDropDownOpen property

    /// <summary>
    /// Gets the list of items to display in the drop-down menu.
    /// This supports <see cref="MenuItem"/> and <see cref="Separator"/> objects.
    /// </summary>
    public ObservableCollection<object> DropDownItems
    {
      get { return _dropDownItems; }
    }
  }
}
