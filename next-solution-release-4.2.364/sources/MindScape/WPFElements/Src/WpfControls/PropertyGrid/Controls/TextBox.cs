using System.Windows;
using System.Windows.Input;

namespace Mindscape.WpfElements.WpfPropertyGrid
{
  /// <summary>
  /// Represents a text box which automatically selects its content when the user clicks
  /// into the text box.
  /// </summary>
  public class TextBox : System.Windows.Controls.TextBox
  {
    /// <summary>
    /// Adds class handling for the <see cref="UIElement.GotKeyboardFocus"/> event.
    /// </summary>
    /// <param name="e">Provides data about the event.</param>
    protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
    {
      SelectAll();

      base.OnGotKeyboardFocus(e);
    }

    /// <summary>
    /// Adds class handling for the <see cref="UIElement.PreviewMouseLeftButtonDown"/> event.
    /// </summary>
    /// <param name="e">Provides data about the event.</param>
    protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
    {
      if (Focusable && !IsFocused)
      {
        Focus();

        e.Handled = true;
      }

      base.OnPreviewMouseLeftButtonDown(e);
    }
  }
}