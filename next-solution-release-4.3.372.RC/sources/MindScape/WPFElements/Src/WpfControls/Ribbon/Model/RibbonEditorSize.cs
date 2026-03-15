using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Specifies the size identifier of an editor within a <see cref="Ribbon"/> control.
  /// </summary>
  public enum RibbonEditorSize
  {
    /// <summary>
    /// The editor is small - it only displays what it needs to.
    /// </summary>
    Small,

    /// <summary>
    /// The editor is medium - it still uses the small icon, but displays addtional elements such as the header.
    /// </summary>
    Medium,

    /// <summary>
    /// The editor is large - it uses the large icon and fills the height of the <see cref="Ribbon"/> control.
    /// </summary>
    Large
  }
}
