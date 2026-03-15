using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Mindscape.WpfElements.Design
{
  internal partial class ToolboxEntries
  {
    internal static readonly ToolboxEntry DualProgressBarToolboxEntry = new ToolboxEntry(

      typeof(DualProgressBar),
      "A progress bar that can display two indicators, for overall and sub-operation progress",

      new DependencyProperty[] {
      },

      new PropertyCategory[] {
        new PropertyCategory("Appearance", new DependencyProperty[] {
          DualProgressBar.CenterContentProperty,
          DualProgressBar.StartContentProperty,
          DualProgressBar.EndContentProperty,
        }),
        new PropertyCategory("Data", new DependencyProperty[] {
          DualProgressBar.NestedPercentageProperty,
        }),
      }

      );
  }
}
