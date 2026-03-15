using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Specifies how font names should be displayed in the font selector of
  /// a <see cref="RichTextToolBar"/>.
  /// </summary>
  public enum FontSelectorDisplayMode
  {
    /// <summary>
    /// Display each font name using that font itself.
    /// </summary>
    Preview,

    /// <summary>
    /// Display the font name using the default font.
    /// </summary>
    Plain,
  }
}
