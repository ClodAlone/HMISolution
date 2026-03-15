using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Mindscape.WpfElements.Themes
{
  /// <summary>
  /// A <see cref="ResourceDictionary"/> containing resources for the OfficeSilver style.
  /// </summary>
  partial class OfficeSilver
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="OfficeSilver"/> class.
    /// </summary>
    public OfficeSilver()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Gets the <see cref="ComponentResourceKey"/> for the <see cref="OfficeSilver"/> style.
    /// </summary>
    public static ComponentResourceKey StyleKey
    {
      get { return new ComponentResourceKey(typeof(OfficeSilver), "Style"); }
    }
  }
}
