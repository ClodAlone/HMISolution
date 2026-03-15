using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// Specifies the zoom mode of a <see cref="Chart"/> control.
  /// </summary>
  public enum ZoomMode
  {
    /// <summary>
    /// No zooming.
    /// </summary>
    None,

    /// <summary>
    /// Zooming can be performed along the vertical axis only.
    /// </summary>
    Vertical,

    /// <summary>
    /// Zooming can be performed along the horizontal axis only.
    /// </summary>
    Horizontal,

    /// <summary>
    /// Zooming can be performed along both vertical and horizontal axes.
    /// </summary>
    Both
  }
}
