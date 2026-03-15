using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Mindscape.WpfElements.Design
{
  internal partial class ToolboxEntries
  {
    internal static readonly ToolboxEntry RibbonToolboxEntry = new ToolboxEntry(

      typeof(Ribbon),
      "A command hub to sit at the top of your application",

      new DependencyProperty[] {
      },

      new PropertyCategory[] {
      }

    );
  }
}
