using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Mindscape.WpfElements.Themes
{
  /// <summary>
  /// A <see cref="ResourceDictionary"/> containing resources for the Alloy style.
  /// </summary>
  partial class Alloy
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="Alloy"/> class.
    /// </summary>
    public Alloy()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Gets the <see cref="ComponentResourceKey"/> for the <see cref="Alloy"/> style.
    /// </summary>
    public static ComponentResourceKey StyleKey
    {
      get { return new ComponentResourceKey(typeof(Alloy), "Style"); }
    }
  }
}
