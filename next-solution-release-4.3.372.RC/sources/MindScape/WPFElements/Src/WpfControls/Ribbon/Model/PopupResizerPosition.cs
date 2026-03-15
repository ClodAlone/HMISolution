using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Primitives;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Specifies the position of a <see cref="PopupResizer"/>.
  /// </summary>
  public enum PopupResizerPosition
  {
    /// <summary>
    /// The <see cref="PopupResizer"/> is in the top left corner of the <see cref="Popup"/>.
    /// </summary>
    TopLeft,

    /// <summary>
    /// The <see cref="PopupResizer"/> is in the top right corner of the <see cref="Popup"/>.
    /// </summary>
    TopRight,

    /// <summary>
    /// The <see cref="PopupResizer"/> is in the bottom left corner of the <see cref="Popup"/>.
    /// </summary>
    BottomLeft,

    /// <summary>
    /// The <see cref="PopupResizer"/> is in the bottom right corner of the <see cref="Popup"/>.
    /// </summary>
    BottomRight,

    /// <summary>
    /// The <see cref="PopupResizer"/> is at the top edge of the <see cref="Popup"/>.
    /// </summary>
    Top,

    /// <summary>
    /// The <see cref="PopupResizer"/> is at the bottom edge of the <see cref="Popup"/>.
    /// </summary>
    Bottom
  }
}
