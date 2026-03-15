using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using System.ComponentModel;
using System.Windows.Threading;
using System.Windows.Controls;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Represents a pane or tab in an <see cref="OutlookBar"/> control.
  /// </summary>
  [ContentProperty("Content")]
  public class OutlookBarItem : DependencyObject, INotifyPropertyChanged
  {
    #region Header property

    /// <summary>
    /// Gets or sets the content displayed on the selection button for this pane.
    /// This is a dependency property.
    /// </summary>
    public object Header
    {
      get { return GetValue(HeaderProperty); }
      set { SetValue(HeaderProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Header"/> property.
    /// </summary>
    public static readonly DependencyProperty HeaderProperty =
      DependencyProperty.Register("Header", typeof(object), typeof(OutlookBarItem),
      null);

    #endregion // Header property

    #region HeaderTemplate Property

    /// <summary>
    /// Gets or sets the <see cref="DataTemplate"/> used to render the header.
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
      DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(OutlookBarItem),
      new FrameworkPropertyMetadata(OnHeaderTemplateChanged));

    private static void OnHeaderTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((OutlookBarItem)d).OnHeaderTemplateChanged();
    }

    private void OnHeaderTemplateChanged()
    {
    }

    #endregion // HeaderTemplate Property

    #region HeaderTemplateSelector Property

    /// <summary>
    /// Gets or sets the <see cref="DataTemplateSelector"/> used to render the header.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="HeaderTemplateSelectorProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public DataTemplateSelector HeaderTemplateSelector
    {
      get { return (DataTemplateSelector)GetValue(HeaderTemplateSelectorProperty); }
      set { SetValue(HeaderTemplateSelectorProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="HeaderTemplateSelector"/> property.
    /// </summary>
    public static readonly DependencyProperty HeaderTemplateSelectorProperty =
      DependencyProperty.Register("HeaderTemplateSelector", typeof(DataTemplateSelector), typeof(OutlookBarItem),
      new FrameworkPropertyMetadata(OnHeaderTemplateSelectorChanged));

    private static void OnHeaderTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((OutlookBarItem)d).OnHeaderTemplateSelectorChanged();
    }

    private void OnHeaderTemplateSelectorChanged()
    {
    }

    #endregion // HeaderTemplateSelector Property

    #region CollapsedHeader property

    /// <summary>
    /// Gets or sets the content displayed on the selection bar when the item is displayed as a small icon.
    /// This is a dependency property.
    /// </summary>
    public object CollapsedHeader
    {
      get { return GetValue(CollapsedHeaderProperty); }
      set { SetValue(CollapsedHeaderProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="CollapsedHeader"/> property.
    /// </summary>
    public static readonly DependencyProperty CollapsedHeaderProperty =
      DependencyProperty.Register("CollapsedHeader", typeof(object), typeof(OutlookBarItem),
      null);

    #endregion // CollapsedHeader property

    #region CollapsedHeaderTemplate Property

    /// <summary>
    /// Gets or sets the <see cref="DataTemplate"/> used to render the collapsed header.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="CollapsedHeaderTemplateProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public DataTemplate CollapsedHeaderTemplate
    {
      get { return (DataTemplate)GetValue(CollapsedHeaderTemplateProperty); }
      set { SetValue(CollapsedHeaderTemplateProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="CollapsedHeaderTemplate"/> property.
    /// </summary>
    public static readonly DependencyProperty CollapsedHeaderTemplateProperty =
      DependencyProperty.Register("CollapsedHeaderTemplate", typeof(DataTemplate), typeof(OutlookBarItem),
      new FrameworkPropertyMetadata(OnCollapsedHeaderTemplateChanged));

    private static void OnCollapsedHeaderTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((OutlookBarItem)d).OnCollapsedHeaderTemplateChanged();
    }

    private void OnCollapsedHeaderTemplateChanged()
    {
    }

    #endregion // CollapsedHeaderTemplate Property

    #region CollapsedHeaderTemplateSelector Property

    /// <summary>
    /// Gets or sets the <see cref="DataTemplateSelector"/> used to render the collapsed header.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="CollapsedHeaderTemplateSelectorProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public DataTemplateSelector CollapsedHeaderTemplateSelector
    {
      get { return (DataTemplateSelector)GetValue(CollapsedHeaderTemplateSelectorProperty); }
      set { SetValue(CollapsedHeaderTemplateSelectorProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="CollapsedHeaderTemplateSelector"/> property.
    /// </summary>
    public static readonly DependencyProperty CollapsedHeaderTemplateSelectorProperty =
      DependencyProperty.Register("CollapsedHeaderTemplateSelector", typeof(DataTemplateSelector), typeof(OutlookBarItem),
      new FrameworkPropertyMetadata(OnCollapsedHeaderTemplateSelectorChanged));

    private static void OnCollapsedHeaderTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((OutlookBarItem)d).OnCollapsedHeaderTemplateSelectorChanged();
    }

    private void OnCollapsedHeaderTemplateSelectorChanged()
    {
    }

    #endregion // CollapsedHeaderTemplateSelector Property

    #region IsSelected property

    /// <summary>
    /// Gets or sets whether or not this <see cref="OutlookBarItem"/> is selected.
    /// This is a dependency property.
    /// </summary>
    public bool IsSelected
    {
      get { return (bool)GetValue(IsSelectedProperty); }
      set { SetValue(IsSelectedProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsSelected"/> property.
    /// </summary>
    public static readonly DependencyProperty IsSelectedProperty =
      DependencyProperty.Register("IsSelected", typeof(bool), typeof(OutlookBarItem),
      new PropertyMetadata(new PropertyChangedCallback(OnIsSelectedChanged)));

    private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((OutlookBarItem)d).OnIsSelectedChanged();
    }

    private void OnIsSelectedChanged()
    {
      RoutedEventHandler handler = IsSelectedChanged;
      if (handler != null)
      {
        handler(this, new RoutedEventArgs());
      }
      Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(NotifyIsSelectedChanged));
    }

    private void NotifyIsSelectedChanged()
    {
      OnPropertyChanged("IsSelected");
    }

    /// <summary>
    /// Raised when the <see cref="IsSelected"/> property changes.
    /// </summary>
    public event RoutedEventHandler IsSelectedChanged;

    #endregion // IsSelected property

    #region Content property

    /// <summary>
    /// Gets or sets the content of this <see cref="OutlookBarItem"/>.
    /// This is a dependency property.
    /// </summary>
    public object Content
    {
      get { return GetValue(ContentProperty); }
      set { SetValue(ContentProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Content"/> property.
    /// </summary>
    public static readonly DependencyProperty ContentProperty =
      DependencyProperty.Register("Content", typeof(object), typeof(OutlookBarItem),
      null);

    #endregion // Content property

    #region Visibility Property

    /// <summary>
    /// Gets or sets the Visibility.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="VisibilityProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public Visibility Visibility
    {
      get { return (Visibility)GetValue(VisibilityProperty); }
      set { SetValue(VisibilityProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Visibility"/> property.
    /// </summary>
    public static readonly DependencyProperty VisibilityProperty =
      DependencyProperty.Register("Visibility", typeof(Visibility), typeof(OutlookBarItem),
      new FrameworkPropertyMetadata(OnVisibilityChanged));

    private static void OnVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((OutlookBarItem)d).OnVisibilityChanged();
    }

    private void OnVisibilityChanged()
    {
      EventHandler handler = VisibilityChanged;
      if (handler != null)
      {
        handler(this, new EventArgs());
      }
    }

    internal event EventHandler VisibilityChanged;

    #endregion // Visibility Property

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="propertyName">The name of the property whose value has changed.</param>
    internal virtual void OnPropertyChanged(string propertyName)
    {
      if (PropertyChanged != null)
      {
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }
  }
}
