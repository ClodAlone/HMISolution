using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Mindscape.WpfElements.Themes
{
  /// <summary>
  /// A <see cref="ResourceDictionary"/> containing resources for the OfficeBlue style.
  /// </summary>
  partial class OfficeBlue
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="OfficeBlue"/> class.
    /// </summary>
    public OfficeBlue()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Gets the <see cref="ComponentResourceKey"/> for the <see cref="OfficeBlue"/> style.
    /// </summary>
    public static ComponentResourceKey StyleKey
    {
      get { return new ComponentResourceKey(typeof(OfficeBlue), "Style"); }
    }
  }
}
