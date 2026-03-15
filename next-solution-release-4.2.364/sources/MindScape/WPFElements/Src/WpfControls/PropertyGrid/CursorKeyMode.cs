using System;
using System.Collections.Generic;
using System.Text;

namespace Mindscape.WpfElements.WpfPropertyGrid
{
  /// <summary>
  /// Determines how the grid handles cursor keys when typed into editors.
  /// </summary>
  public enum CursorKeyMode
  {
    /// <summary>
    /// Cursor keys are intercepted by the grid and used to change the selection.
    /// This is the default.
    /// </summary>
    Navigate,

    /// <summary>
    /// Cursor keys are passed to the editor.  The user may still use the cursor
    /// keys to change the selection, but only when the focus is on a property
    /// name rather than an editor.  Choose this option if you use editors that
    /// have special handling for cursor keys such as <see cref="SpinDecorator"/>.
    /// </summary>
    PassToEditor
  }
}
