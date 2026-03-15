using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Mindscape.WpfElements.Themes
{
  /// <summary>
  /// A <see cref="ResourceDictionary"/> containing resources for the AlloyLight style.
  /// </summary>
  partial class AlloyLight
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="AlloyLight"/> class.
    /// </summary>
    public AlloyLight()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Gets the <see cref="ComponentResourceKey"/> for the <see cref="AlloyLight"/> style.
    /// </summary>
    public static ComponentResourceKey StyleKey
    {
      get { return new ComponentResourceKey(typeof(AlloyLight), "Style"); }
    }
  }
}
