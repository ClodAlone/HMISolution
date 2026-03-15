using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Mindscape.WpfElements.Design
{
  internal partial class ToolboxEntries
  {
    internal static readonly ToolboxEntry OutlookBarToolboxEntry = new ToolboxEntry(

      typeof(OutlookBar),
      "A control which selects between multiple content panes using buttons that can be collapsed to save space",

      new object[] {
        "SelectedItem",
      },

      new PropertyCategory[] {
        new PropertyCategory("Appearance", new DependencyProperty[] {
          OutlookBar.ExpandedItemCountProperty,
        }),
      }

      );
  }
}
