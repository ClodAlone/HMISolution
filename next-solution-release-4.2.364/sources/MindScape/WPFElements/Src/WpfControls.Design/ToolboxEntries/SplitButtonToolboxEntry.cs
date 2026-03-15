using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Mindscape.WpfElements.Design
{
  internal partial class ToolboxEntries
  {
    internal static readonly ToolboxEntry SplitButtonToolboxEntry = new ToolboxEntry(

      typeof(SplitButton),
      "A button which also provides an optional drop-down menu for selecting  alternate commands",

      new object[] {
        "DropDownItems"
      },

      new PropertyCategory[] {
        new PropertyCategory("Appearance", new DependencyProperty[] {
          SplitButton.IsDropDownOpenProperty,
        }),
      }

      );
  }
}
