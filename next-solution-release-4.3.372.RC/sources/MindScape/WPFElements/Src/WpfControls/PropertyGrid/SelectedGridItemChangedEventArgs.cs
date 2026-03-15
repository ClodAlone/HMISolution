using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Mindscape.WpfElements.WpfPropertyGrid
{
  /// <summary>
  /// Represents the method that will handle the SelectedGridItemChanged event of a
  /// <see cref="PropertyGrid"/>.
  /// </summary>
  /// <param name="sender">The source of the event.</param>
  /// <param name="e">The event data.</param>
  public delegate void SelectedGridItemChangedEventHandler(object sender, SelectedGridItemChangedEventArgs e);

  /// <summary>
  /// Provides data for the SelectedGridItemChanged event of the <see cref="PropertyGrid"/> control.
  /// </summary>
  public class SelectedGridItemChangedEventArgs : RoutedEventArgs
  {
    private readonly PropertyGridRow _newSelection;
    private readonly PropertyGridRow _oldSelection;

    /// <summary>
    /// Gets the newly selected <see cref="PropertyGridRow"/>.
    /// </summary>
    public PropertyGridRow NewSelection
    {
      get { return _newSelection; }
    }

    /// <summary>
    /// Gets the previously selected <see cref="PropertyGridRow"/>.
    /// </summary>
    public PropertyGridRow OldSelection
    {
      get { return _oldSelection; }
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="SelectedGridItemChangedEventArgs"/> type.
    /// </summary>
    /// <param name="oldSelection">The previously selected <see cref="PropertyGridRow"/>.</param>
    /// <param name="newSelection">The newly selected <see cref="PropertyGridRow"/>.</param>
    public SelectedGridItemChangedEventArgs(PropertyGridRow oldSelection, PropertyGridRow newSelection)
      : base(PropertyGrid.SelectedGridItemChangedEvent)
    {
      _oldSelection = oldSelection;
      _newSelection = newSelection;
    }
  }
}
