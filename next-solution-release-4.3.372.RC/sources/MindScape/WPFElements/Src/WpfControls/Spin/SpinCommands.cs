using System.Windows.Input;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Contains commands used with the <see cref="Spin"/> control.
  /// </summary>
  public static class SpinCommands
  {
    /// <summary>
    /// Gets the value that represents the Increase command.
    /// </summary>
    public static readonly RoutedCommand Increase =
      new RoutedCommand("Increase", typeof(SpinCommands));

    /// <summary>
    /// Gets the value that represents the Decrease command.
    /// </summary>
    public static readonly RoutedCommand Decrease =
      new RoutedCommand("Decrease", typeof(SpinCommands));
  }
}
