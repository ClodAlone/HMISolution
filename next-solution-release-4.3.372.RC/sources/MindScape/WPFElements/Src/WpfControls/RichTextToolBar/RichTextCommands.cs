using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Contains commands used with the <see cref="RichTextToolBar"/> control.
  /// </summary>
  public static class RichTextCommands
  {
    /// <summary>
    /// A command for changing the font weight of the selected text.
    /// </summary>
    public static readonly RoutedCommand ToggleFontWeight = new RoutedCommand("ToggleFontWeight", typeof(RichTextToolBar));

    /// <summary>
    /// A command for changing the font style of the selected text.
    /// </summary>
    public static readonly RoutedCommand ToggleFontStyle = new RoutedCommand("ToggleFontStyle", typeof(RichTextToolBar));

    /// <summary>
    /// A command for changing the text decorations of the selected text.
    /// </summary>
    public static readonly RoutedCommand ToggleTextDecorations = new RoutedCommand("ToggleTextDecorations", typeof(RichTextToolBar));

    /// <summary>
    /// A command for changing the text alignment of the selected text.
    /// </summary>
    public static readonly RoutedCommand ChangeTextAlignment = new RoutedCommand("ChangeTextAlignment", typeof(RichTextToolBar));

    /// <summary>
    /// A command for applying the current foreground color to the selected text.
    /// </summary>
    public static readonly RoutedCommand ApplyForegroundColor = new RoutedCommand("ApplyForegroundColor", typeof(RichTextToolBar));

    /// <summary>
    /// A command for applying the current background color to the selected text.
    /// </summary>
    public static readonly RoutedCommand ApplyBackgroundColor = new RoutedCommand("ApplyBackgroundColor", typeof(RichTextToolBar));
  }
}
