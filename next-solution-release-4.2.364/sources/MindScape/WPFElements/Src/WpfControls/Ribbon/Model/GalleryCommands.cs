using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// <see cref="Gallery"/> commands.
  /// </summary>
  public static class GalleryCommands
  {
    /// <summary>
    /// Navigates up one row in a <see cref="Gallery"/>.
    /// </summary>
    public static readonly RoutedCommand NavigateUpCommand = new RoutedCommand("NavigateUpCommand", typeof(Gallery));

    /// <summary>
    /// Navigates down one row in a <see cref="Gallery"/>.
    /// </summary>
    public static readonly RoutedCommand NavigateDownCommand = new RoutedCommand("NavigateDownCommand", typeof(Gallery));
  }
}
