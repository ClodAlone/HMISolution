using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Represents an item displayed by a <see cref="Gallery"/> control.
  /// </summary>
  public class GalleryItem : ContentControl
  {
    static GalleryItem()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(GalleryItem),
        new FrameworkPropertyMetadata(typeof(GalleryItem)));
    }

    /// <summary>
    /// Called when the left mouse button is pressed over this control.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
      base.OnMouseLeftButtonDown(e);

      CaptureMouse();
      SetValue(IsPressedPropertyKey, true);
    }

    /// <summary>
    /// Called when the mouse moves over this control.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnMouseMove(MouseEventArgs e)
    {
      base.OnMouseMove(e);

      if (IsMouseCaptured)
      {
        Point position = e.GetPosition(this);
        if (position.X < 0 || position.Y < 0 || position.X > ActualWidth || position.Y > ActualHeight)
        {
          SetValue(IsPressedPropertyKey, false);
        }
        else
        {
          SetValue(IsPressedPropertyKey, true);
        }
      }
    }

    /// <summary>
    /// Called when the left mouse button is released over this control.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
      base.OnMouseLeftButtonUp(e);

      SetValue(IsPressedPropertyKey, false);
      bool hadMouseCapture = IsMouseCaptured;
      ReleaseMouseCapture();
      if (IsMouseOver && hadMouseCapture)
      {
        IsSelected = true;
      }
    }

    /// <summary>
    /// Called when this control loses mouse capture.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnLostMouseCapture(MouseEventArgs e)
    {
      SetValue(IsPressedPropertyKey, false);
    }

    #region IsSelected Property

    /// <summary>
    /// Gets or sets whether or not this <see cref="GalleryItem"/> is currently selected.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsSelectedProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsSelected
    {
      get { return (bool)GetValue(IsSelectedProperty); }
      set { SetValue(IsSelectedProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsSelected"/> property.
    /// </summary>
    public static readonly DependencyProperty IsSelectedProperty =
      DependencyProperty.Register("IsSelected", typeof(bool), typeof(GalleryItem),
      new FrameworkPropertyMetadata(OnIsSelectedChanged));

    private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((GalleryItem)d).OnIsSelectedChanged();
    }

    private void OnIsSelectedChanged()
    {
      Gallery gallery = VisualTreeUtils.FindAncestor<Gallery>(this);
      if (gallery != null)
      {
        gallery.GalleryItem_IsSelectedChanged(this);
      }
    }

    #endregion // IsSelected Property

    #region IsPressed Property

    /// <summary>
    /// Gets whether or not the left mouse button is pressed over this <see cref="GalleryItem"/>.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsPressedProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsPressed
    {
      get { return (bool)GetValue(IsPressedProperty); }
    }

    private static readonly DependencyPropertyKey IsPressedPropertyKey =
        DependencyProperty.RegisterReadOnly("IsPressed", typeof(bool), typeof(GalleryItem), new UIPropertyMetadata(false));

    /// <summary>
    /// Identifies the <see cref="IsPressed"/> property.
    /// </summary>
    public static readonly DependencyProperty IsPressedProperty =
        IsPressedPropertyKey.DependencyProperty;

    #endregion // IsPressed Property
  }
}
