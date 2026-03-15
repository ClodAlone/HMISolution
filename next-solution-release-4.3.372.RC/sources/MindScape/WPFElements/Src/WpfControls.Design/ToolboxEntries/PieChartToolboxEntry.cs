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
    internal static readonly ToolboxEntry PieChartToolboxEntry = new ToolboxEntry(

      typeof(PieChart),
      "Displays pie charts using PieSeries elements",

      new DependencyProperty[] {
      },

      new PropertyCategory[] {
      }

      );
  }
}
