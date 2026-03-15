using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;
using Infralution.Licensing;
using System;
using System.Diagnostics;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// A control for displaying a summary and a drop-down detail pane, similar to a combo box.
  /// </summary>
  [LicenseProvider(typeof(PublicEncryptedLicenseProvider))]
  public class DropDownEditBox : Control
  {
    static DropDownEditBox()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(DropDownEditBox),
        new FrameworkPropertyMetadata(typeof(DropDownEditBox)));

      EventManager.RegisterClassHandler(typeof(DropDownEditBox), DropDownEditBox.PreviewMouseDownEvent,
        new MouseButtonEventHandler(OnPreviewMouseButtonDown));
      EventManager.RegisterClassHandler(typeof(DropDownEditBox), DropDownEditBox.MouseWheelEvent,
        new MouseWheelEventHandler(OnMouseWheel));
      EventManager.RegisterClassHandler(typeof(DropDownEditBox), DropDownEditBox.PreviewKeyDownEvent,
        new KeyEventHandler(OnPreviewKeyDown));
      EventManager.RegisterClassHandler(typeof(DropDownEditBox), DropDownEditBox.DropDownCloseRequestedEvent,
        new EventHandler<RoutedEventArgs>(OnDropDownCloseRequested));
    }

    /// <summary>
    /// Gets or sets the content to be presented in the summary and drop-down areas.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ContentProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public object Content
    {
      get { return GetValue(ContentProperty); }
      set { SetValue(ContentProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Content"/> property.
    /// </summary>
    public static readonly DependencyProperty ContentProperty =
      ContentControl.ContentProperty.AddOwner(typeof(DropDownEditBox));


    /// <summary>
    /// Gets or sets the maximum height of the drop-down pane.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="MaxDropDownHeightProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    [TypeConverter(typeof(LengthConverter))]
    public double MaxDropDownHeight
    {
      get { return (double)GetValue(MaxDropDownHeightProperty); }
      set { SetValue(MaxDropDownHeightProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="MaxDropDownHeight"/> property.
    /// </summary>
    public static readonly DependencyProperty MaxDropDownHeightProperty =
        DependencyProperty.Register("MaxDropDownHeight", typeof(double), typeof(DropDownEditBox),
        new UIPropertyMetadata(SystemParameters.PrimaryScreenHeight / 3));


    /// <summary>
    /// Gets or sets whether the drop-down is currently open.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsDropDownOpenProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsDropDownOpen
    {
      get { return (bool)GetValue(IsDropDownOpenProperty); }
      set { SetValue(IsDropDownOpenProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsDropDownOpen"/> property.
    /// </summary>
    public static readonly DependencyProperty IsDropDownOpenProperty =
        DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(DropDownEditBox),
        new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsDropDownOpenChanged));

    /// <summary>
    /// Gets or sets the template for the summary part of the control (the
    /// part which is visible when the control is not dropped down).
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="HeaderTemplateProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public DataTemplate HeaderTemplate
    {
      get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
      set { SetValue(HeaderTemplateProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="HeaderTemplate"/> property.
    /// </summary>
    public static readonly DependencyProperty HeaderTemplateProperty =
        DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(DropDownEditBox));


    /// <summary>
    /// Gets or sets the template for the content of drop-down part of the control.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="DropDownTemplateProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public DataTemplate DropDownTemplate
    {
      get { return (DataTemplate)GetValue(DropDownTemplateProperty); }
      set { SetValue(DropDownTemplateProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="DropDownTemplate"/> property.
    /// </summary>
    public static readonly DependencyProperty DropDownTemplateProperty =
        DependencyProperty.Register("DropDownTemplate", typeof(DataTemplate), typeof(DropDownEditBox));

    private static void OnIsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      DropDownEditBox control = (DropDownEditBox)d;
      bool isDropDownOpen = (bool)(e.NewValue);
      Window window = VisualTreeUtils.FindContaining<Window>(d);

      if (isDropDownOpen)
      {
        Mouse.Capture(control, CaptureMode.SubTree);
        control.Focus();
        if (window != null)
        {
          window.LocationChanged += control.OnContainingWindowMoved;
          window.SizeChanged += control.OnContainingWindowMoved;
          window.StateChanged += control.OnContainingWindowMoved;
        }
      }
      else
      {
        if (Mouse.Captured == control)
        {
          Mouse.Capture(null);
        }
        if (window != null)
        {
          window.LocationChanged -= control.OnContainingWindowMoved;
          window.SizeChanged -= control.OnContainingWindowMoved;
          window.StateChanged -= control.OnContainingWindowMoved;
        }
      }

      control.OnIsDropDownOpenChanged();
    }

    internal event EventHandler IsDropDownOpenChanged;

    private void OnIsDropDownOpenChanged()
    {
      EventHandler handler = IsDropDownOpenChanged;
      if (handler != null)
      {
        handler(this, EventArgs.Empty);
      }
    }

    private void OnContainingWindowMoved(object sender, EventArgs e)
    {
      IsDropDownOpen = false;
    }

    /// <summary>
    /// Provides control-specific handling for the <see cref="UIElement.IsKeyboardFocusWithinChanged"/> event.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e)
    {
      base.OnIsKeyboardFocusWithinChanged(e);

      if (IsDropDownOpen && !IsKeyboardFocusWithin)
      {
        //DependencyObject focusedElement = Keyboard.FocusedElement as DependencyObject;
        //if (focusedElement != null)
        {
          IsDropDownOpen = false;
        }
      }
    }

    private static void OnPreviewMouseButtonDown(object sender, MouseButtonEventArgs e)
    {
      DropDownEditBox control = (DropDownEditBox)sender;

      if (Mouse.Captured == control && e.OriginalSource == control)
      {
        control.IsDropDownOpen = false;
      }
    }

    /// <summary>
    /// Called when the left mouse button is pressed on this <see cref="DropDownEditBox"/>.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
      base.OnMouseLeftButtonDown(e);

      if (IsDropDownOpen)
      {
        e.Handled = true;
      }
    }

    private static void OnMouseWheel(object sender, MouseWheelEventArgs e)
    {
      DropDownEditBox control = (DropDownEditBox)sender;

      if (Mouse.Captured == control && e.OriginalSource == control)
      {
        control.IsDropDownOpen = false;
      }

      e.Handled = true;
    }

    private static void OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
      DropDownEditBox control = (DropDownEditBox)sender;

      if (e.Key == Key.Escape && control.IsDropDownOpen)
      {
        control.IsDropDownOpen = false;

        e.Handled = true;
      }
    }

    #region Custom close support

    /// <summary>
    /// Identifies the <see cref="DropDownCloseRequested"/> routed event.
    /// </summary>
    public static readonly RoutedEvent DropDownCloseRequestedEvent =
      EventManager.RegisterRoutedEvent("DropDownCloseRequested", RoutingStrategy.Bubble,
        typeof(EventHandler<RoutedEventArgs>), typeof(DropDownEditBox));

    /// <summary>
    /// Raised by contained controls when they wish to close the drop-down.
    /// </summary>
    public event EventHandler<RoutedEventArgs> DropDownCloseRequested
    {
      add { AddHandler(DropDownCloseRequestedEvent, value); }
      remove { RemoveHandler(DropDownCloseRequestedEvent, value); }
    }

    private static void OnDropDownCloseRequested(object sender, RoutedEventArgs e)
    {
      DropDownEditBox control = (DropDownEditBox)sender;

      if (control.IsDropDownOpen)
      {
        control.IsDropDownOpen = false;
        e.Handled = true;
      }
    }

    #endregion

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the <see cref="Style"/> for spin
    /// buttons.
    /// </summary>
    public static object ToggleButtonStyleKey
    {
      get { return new ComponentResourceKey(typeof(DropDownEditBox), "ToggleButtonStyle"); }
    }

    /// <summary>
    /// Gets the value of the RequestCloseOnAction attached property for a given <see cref="DependencyObject" />.
    /// </summary>
    /// <param name="obj">The element from which to read the property value.</param>
    /// <returns>The value of the RequestCloseOnAction attached property.</returns>
    public static bool GetRequestCloseOnAction(DependencyObject obj)
    {
      return (bool)obj.GetValue(RequestCloseOnActionProperty);
    }

    /// <summary>
    /// Sets the value of the RequestCloseOnAction attached property for a given <see cref="DependencyObject" />.
    /// </summary>
    /// <param name="obj">The element on which to set the property value.</param>
    /// <param name="value">The value to which to set the property.</param>
    public static void SetRequestCloseOnAction(DependencyObject obj, bool value)
    {
      obj.SetValue(RequestCloseOnActionProperty, value);
    }

    /// <summary>
    /// Identifies the RequestCloseOnAction attached property.
    /// </summary>
    public static readonly DependencyProperty RequestCloseOnActionProperty =
      DependencyProperty.RegisterAttached("RequestCloseOnAction", typeof(bool), typeof(DropDownEditBox),
      new FrameworkPropertyMetadata(false));

    internal static void NotifyPossibleCloseAction(FrameworkElement sender)
    {
      if (GetRequestCloseOnAction(sender))
      {
        sender.RaiseEvent(new RoutedEventArgs(DropDownCloseRequestedEvent));
      }
    }
  }
}
