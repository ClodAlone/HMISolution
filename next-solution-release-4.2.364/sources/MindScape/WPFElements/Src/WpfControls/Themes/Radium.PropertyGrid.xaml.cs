using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Mindscape.WpfElements.WpfPropertyGrid.Themes
{
  /// <summary>
  /// A <see cref="ResourceDictionary"/> containing resources for the Radium property grid style.
  /// </summary>
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
  partial class Radium
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="Radium"/> class.
    /// </summary>
    public Radium()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the Radium <see cref="Style"/>.
    /// </summary>
    public static ComponentResourceKey StyleKey
    {
      get { return new ComponentResourceKey(typeof(Radium), "Style"); }
    }
  }
}
