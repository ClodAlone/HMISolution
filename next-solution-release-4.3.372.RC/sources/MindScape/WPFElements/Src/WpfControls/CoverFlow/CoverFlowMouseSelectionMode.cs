using System;
using System.Collections.Generic;
using System.Text;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Specifies how the mouse can be used to interactively select an 
  /// item within a <see cref="CoverFlow"/>.
  /// </summary>
  public enum CoverFlowMouseSelectionMode
  {
    /// <summary>
    /// Items can be selected by moving the mouse over them.
    /// </summary>
    MouseEnter,

    /// <summary>
    /// Items can be selected by clicking them with the mouse.
    /// </summary>
    MousePressed,

    /// <summary>
    /// Items can not be selected interactively with the mouse.
    /// </summary>
    None
  }
}
