using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Primitives;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Specifies the resize more of a <see cref="PopupResizer"/>.
  /// </summary>
  public enum PopupResizeMode
  {
    /// <summary>
    /// The <see cref="PopupResizer"/> is placed in a corner
    /// and is used to resize both the width and height of the <see cref="Popup"/>.
    /// </summary>
    Corner,

    /// <summary>
    /// The <see cref="PopupResizer"/> is placed on the top or bottom edge
    /// and is used to resize only the eight of the <see cref="Popup"/>.
    /// </summary>
    Edge
  }
}
