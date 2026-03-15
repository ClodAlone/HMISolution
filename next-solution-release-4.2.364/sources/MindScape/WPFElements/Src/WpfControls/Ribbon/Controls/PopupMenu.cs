using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Collections;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Timers;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Hosts a menu system within a <see cref="Popup"/>.
  /// </summary>
  public class PopupMenu : ItemsControl
  {
    private MenuItem _openMenuItem;
    private readonly DispatcherTimer _closeTimer;
    private readonly DispatcherTimer _openTimer;

    static PopupMenu()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(PopupMenu),
        new FrameworkPropertyMetadata(typeof(PopupMenu)));
      ItemsPanelProperty.OverrideMetadata(typeof(PopupMenu),
        new FrameworkPropertyMetadata(BuildDefaultItemsPanel()));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PopupMenu"/> class.
    /// </summary>
    public PopupMenu()
    {
      _closeTimer = new DispatcherTimer();
      _closeTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
      _closeTimer.Tick += new EventHandler(CloseTimer_Tick);

      _openTimer = new DispatcherTimer();
      _openTimer.Interval = new TimeSpan(0, 0, 0, 0, 300);
      _openTimer.Tick += new EventHandler(OpenTimer_Tick);
    }

    /// <summary>
    /// Called when the mouse moves over this <see cref="PopupMenu"/>.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnMouseMove(MouseEventArgs e)
    {
      base.OnMouseMove(e);

      MenuItem mouseOverMenuItem = VisualTreeUtils.FindAncestor("MenuItem", Mouse.DirectlyOver as DependencyObject) as MenuItem;
      PopupMenu popupMenu = VisualTreeUtils.FindAncestor("PopupMenu", Mouse.DirectlyOver as DependencyObject) as PopupMenu;
      if ((mouseOverMenuItem == null || mouseOverMenuItem != _openMenuItem) && _openMenuItem != null && popupMenu == this)
      {
        _closeTimer.Start();
      }

      if (mouseOverMenuItem != null && mouseOverMenuItem != _requestToOpenItem)
      {
        _openTimer.Stop();
      }
    }

    private void CloseTimer_Tick(object sender, EventArgs e)
    {
      _closeTimer.Stop();
      MenuItem mouseOverMenuItem = VisualTreeUtils.FindAncestor("MenuItem", Mouse.DirectlyOver as DependencyObject) as MenuItem;
      PopupMenu popupMenu = VisualTreeUtils.FindAncestor("PopupMenu", Mouse.DirectlyOver as DependencyObject) as PopupMenu;
      if ((mouseOverMenuItem == null || mouseOverMenuItem != _openMenuItem) && _openMenuItem != null && popupMenu == this)
      {
        _openMenuItem.IsSubmenuOpen = false;
        _openMenuItem = null;
      }
    }

    private static ItemsPanelTemplate BuildDefaultItemsPanel()
    {
      FrameworkElementFactory factory = new FrameworkElementFactory(typeof(PopupMenuPanel));
      ItemsPanelTemplate template = new ItemsPanelTemplate(factory);
      return template;
    }

    /// <summary>
    /// Clears the given element.
    /// </summary>
    /// <param name="element">The element to clear.</param>
    /// <param name="item">The item that the given element currently displays.</param>
    protected override void ClearContainerForItemOverride(DependencyObject element, object item)
    {
      base.ClearContainerForItemOverride(element, item);

      MenuItem menuItem = element as MenuItem;
      if (menuItem != null)
      {
        menuItem.MouseEnter -= new MouseEventHandler(MenuItem_MouseEnter);
        menuItem.Click -= new RoutedEventHandler(MenuItem_Click);
      }
      else
      {
        Gallery gallery = element as Gallery;
        if (gallery != null)
        {
          gallery.SelectedValueChanged -= new EventHandler(Gallery_SelectedValueChanged);
        }
      }
    }

    /// <summary>
    /// Prepares the specified element to display the specified item.
    /// </summary>
    /// <param name="element">The element used to display the specified item.</param>
    /// <param name="item">The item to display.</param>
    protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
    {
      base.PrepareContainerForItemOverride(element, item);

      FrameworkElement frameworkElement = element as FrameworkElement;
      if (frameworkElement != null)
      {
        ResourceKey styleKey = null;
        MenuItem menuItem = element as MenuItem;
        if (menuItem != null)
        {
          styleKey = MenuItemStyleKey;
          menuItem.MouseEnter += new MouseEventHandler(MenuItem_MouseEnter);
          menuItem.Click += new RoutedEventHandler(MenuItem_Click);
        }
        else
        {
          Gallery gallery = element as Gallery;
          if (gallery != null)
          {
            gallery.SelectedValueChanged += new EventHandler(Gallery_SelectedValueChanged);
            //gallery.Height = 200;
          }
        }
        if (styleKey != null)
        {
          frameworkElement.SetResourceReference(FrameworkElement.StyleProperty, styleKey);
        }
      }
    }

    private void MenuItem_Click(object sender, RoutedEventArgs e)
    {
      ClosePopups();
    }

    private void Gallery_SelectedValueChanged(object sender, EventArgs e)
    {
      ClosePopups();
    }

    private MenuItem _requestToOpenItem;

    private void MenuItem_MouseEnter(object sender, MouseEventArgs e)
    {
      MenuItem menuItem = sender as MenuItem;
      if (menuItem.HasItems)
      {
        _requestToOpenItem = menuItem;
        _openTimer.Start();
      }
    }

    private void OpenTimer_Tick(object sender, EventArgs e)
    {
      _openTimer.Stop();
      MenuItem item = VisualTreeUtils.FindAncestor("MenuItem", Mouse.DirectlyOver as DependencyObject) as MenuItem;
      if (item != null && item == _requestToOpenItem)
      {
        _requestToOpenItem.IsSubmenuOpen = true;
        _openMenuItem = _requestToOpenItem;
      }
    }

    private void ClosePopups()
    {
      Popup popup = VisualTreeUtils.FindPopup(this);
      while (popup != null)
      {
        popup.IsOpen = false;
        popup = VisualTreeUtils.FindPopup(popup);
      }
    }

    #region Resource Keys

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the <see cref="MenuItem"/> style.
    /// </summary>
    public static ResourceKey MenuItemStyleKey
    {
      get { return new ComponentResourceKey(typeof(PopupMenu), "PopupMenuItemStyle"); }
    }

    #endregion // Resource Keys
  }
}
