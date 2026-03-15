using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mindscape.WpfElements.Charting;
using System.Windows;

namespace Mindscape.WpfElements.Design
{
  internal partial class ToolboxEntries
  {
    internal static readonly ToolboxEntry PolarChartToolboxEntry = new ToolboxEntry(

      typeof(PolarChart),
      "Displays polar charts and graphs using appropriate Series elements",

      new DependencyProperty[] {
      },

      new PropertyCategory[] {
      }

      );
  }
}
