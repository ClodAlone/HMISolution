using System.Windows;

namespace Mindscape.WpfElements.Design
{
  internal partial class ToolboxEntries
  {
    internal static readonly ToolboxEntry ProportionalStackPanelToolboxEntry = new ToolboxEntry(

      typeof(ProportionalStackPanel),
      "Arranges child elements into a single line and sizes them according to requested proportions",

      new DependencyProperty[] {
      },

      new PropertyCategory[] {
        new PropertyCategory("Behavior", new DependencyProperty[] {
        }),
        new PropertyCategory("Appearance", new DependencyProperty[] {
        })
      }

      );
  }
}
