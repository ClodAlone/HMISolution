using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mindscape.WpfElements.WpfPropertyGrid
{
  /// <summary>
  /// Specifies how the <see cref="PropertyGrid"/> control handles internal node data.
  /// </summary>
  public enum NodeClearanceMode
  {
    /// <summary>
    /// Nodes remain valid when no longer in use, but retain their
    /// internal data as long as the node is referenced.
    /// </summary>
    Default,

    /// <summary>
    /// Nodes clear their internal data when no longer used.  Nodes
    /// may become invalid as soon as they are removed from the grid.
    /// </summary>
    Aggressive
  }
}
