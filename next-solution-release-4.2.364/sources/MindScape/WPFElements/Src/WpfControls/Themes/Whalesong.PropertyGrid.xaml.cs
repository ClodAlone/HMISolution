using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Mindscape.WpfElements.WpfPropertyGrid.Themes
{
  /// <summary>
  /// A <see cref="ResourceDictionary"/> containing resources for the Whalesong property grid style.
  /// </summary>
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
  partial class Whalesong
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="Whalesong"/> class.
    /// </summary>
    public Whalesong()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the Whalesong <see cref="Style"/>.
    /// </summary>
    public static ComponentResourceKey StyleKey
    {
      get { return new ComponentResourceKey(typeof(Whalesong), "Style"); }
    }
  }
}
