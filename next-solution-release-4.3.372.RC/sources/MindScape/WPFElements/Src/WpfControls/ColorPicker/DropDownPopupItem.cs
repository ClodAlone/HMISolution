using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// A control that acts like a menu item which can be added to a popup. The drop down pane can support any content.
  /// </summary>
  [ContentProperty("Content")]
  public class DropDownPopupItem : Control
  {
    static DropDownPopupItem()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(DropDownPopupItem),
        new FrameworkPropertyMetadata(typeof(DropDownPopupItem)));
    }

    /// <summary>
    /// Called when the mouse enters this <see cref="DropDownPopupItem"/>.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnMouseEnter(MouseEventArgs e)
    {
      base.OnMouseEnter(e);

      _host = VisualTreeUtils.FindContaining<Decorator>(this);
      if (_host != null)
      {
        _host.MouseMove += new MouseEventHandler(Host_MouseMove);
        _host.Unloaded += new RoutedEventHandler(Host_Unloaded);
      }

      IsDropDownOpen = true;
    }

    private void Host_Unloaded(object sender, RoutedEventArgs e)
    {
      IsDropDownOpen = false;
    }

    private void Host_MouseMove(object sender, MouseEventArgs e)
    {
      if (!IsMouseOver)
      {
        IsDropDownOpen = false;
        _host.MouseMove -= new MouseEventHandler(Host_MouseMove);
        _host.Unloaded -= new RoutedEventHandler(Host_Unloaded);
      }
    }

    private Decorator _host;

    #region IsDropDownOpen Property

    /// <summary>
    /// Gets or sets whether or not the drop down pane is open.
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
      DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(DropDownPopupItem),
      new FrameworkPropertyMetadata(OnIsDropDownOpenChanged));

    private static void OnIsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DropDownPopupItem)d).OnIsDropDownOpenChanged(e);
    }

    private void OnIsDropDownOpenChanged(DependencyPropertyChangedEventArgs e)
    {
    }

    #endregion // IsDropDownOpen Property

    #region Header Property

    /// <summary>
    /// Gets or sets the header that labels this <see cref="DropDownPopupItem"/>.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="HeaderProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public object Header
    {
      get { return GetValue(HeaderProperty); }
      set { SetValue(HeaderProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Header"/> property.
    /// </summary>
    public static readonly DependencyProperty HeaderProperty =
      DependencyProperty.Register("Header", typeof(object), typeof(DropDownPopupItem),
      new FrameworkPropertyMetadata(OnHeaderChanged));

    private static void OnHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DropDownPopupItem)d).OnHeaderChanged(e);
    }

    private void OnHeaderChanged(DependencyPropertyChangedEventArgs e)
    {
    }

    #endregion // Header Property

    #region Content Property

    /// <summary>
    /// Gets or sets the content of the drop down pane.
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
      DependencyProperty.Register("Content", typeof(object), typeof(DropDownPopupItem),
      new FrameworkPropertyMetadata(OnContentChanged));

    private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DropDownPopupItem)d).OnContentChanged(e);
    }

    private void OnContentChanged(DependencyPropertyChangedEventArgs e)
    {
    }

    #endregion // Content Property

    #region ContentTemplate Property

    /// <summary>
    /// Gets or sets the <see cref="DataTemplate"/> for the drop down content.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ContentTemplateProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public DataTemplate ContentTemplate
    {
      get { return (DataTemplate)GetValue(ContentTemplateProperty); }
      set { SetValue(ContentTemplateProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ContentTemplate"/> property.
    /// </summary>
    public static readonly DependencyProperty ContentTemplateProperty =
      DependencyProperty.Register("ContentTemplate", typeof(DataTemplate), typeof(DropDownPopupItem),
      new FrameworkPropertyMetadata(OnContentTemplateChanged));

    private static void OnContentTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DropDownPopupItem)d).OnContentTemplateChanged(e);
    }

    private void OnContentTemplateChanged(DependencyPropertyChangedEventArgs e)
    {
    }

    #endregion // ContentTemplate Property

    #region Icon Property

    /// <summary>
    /// Gets or sets the icon that appears in this <see cref="DropDownPopupItem"/>.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IconProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public object Icon
    {
      get { return GetValue(IconProperty); }
      set { SetValue(IconProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Icon"/> property.
    /// </summary>
    public static readonly DependencyProperty IconProperty =
      DependencyProperty.Register("Icon", typeof(object), typeof(DropDownPopupItem),
      new FrameworkPropertyMetadata(OnIconChanged));

    private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DropDownPopupItem)d).OnIconChanged(e);
    }

    private void OnIconChanged(DependencyPropertyChangedEventArgs e)
    {
    }

    #endregion // Icon Property
  }
}
