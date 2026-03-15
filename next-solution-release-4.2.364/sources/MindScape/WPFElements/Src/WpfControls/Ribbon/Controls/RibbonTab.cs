using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// A tab displayed within a <see cref="Ribbon"/> control.
  /// </summary>
  public class RibbonTab : HeaderedItemsControl
  {
    static RibbonTab()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonTab),
        new FrameworkPropertyMetadata(typeof(RibbonTab)));
    }

    #region IsSelected Property

    /// <summary>
    /// Gets or sets whether or not this tab is selected.
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
      DependencyProperty.Register("IsSelected", typeof(bool), typeof(RibbonTab),
      new FrameworkPropertyMetadata(OnIsSelectedChanged));

    private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((RibbonTab)d).OnIsSelectedChanged();
    }

    private void OnIsSelectedChanged()
    {
      EventHandler handler = IsSelectedChanged;
      if (handler != null)
      {
        handler(this, EventArgs.Empty);
      }
    }

    internal event EventHandler IsSelectedChanged;

    #endregion // IsSelected Property

    /// <summary>
    /// Called when a mouse button is pressed over this <see cref="RibbonTab"/>.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
      base.OnMouseDown(e);

      IsSelected = true;
    }
  }
}
