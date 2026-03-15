using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Mindscape.WpfElements.Themes
{
  /// <summary>
  /// A <see cref="ResourceDictionary"/> containing resources for the OfficeBlack style.
  /// </summary>
  partial class OfficeBlack
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="OfficeBlack"/> class.
    /// </summary>
    public OfficeBlack()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Gets the <see cref="ComponentResourceKey"/> for the <see cref="OfficeBlack"/> style.
    /// </summary>
    public static ComponentResourceKey StyleKey
    {
      get { return new ComponentResourceKey(typeof(OfficeBlack), "Style"); }
    }
  }
}
