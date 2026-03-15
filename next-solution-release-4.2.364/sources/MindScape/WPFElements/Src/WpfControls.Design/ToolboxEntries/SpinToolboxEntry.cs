using System.Windows;

namespace Mindscape.WpfElements.Design
{
  internal partial class ToolboxEntries
  {
    internal static readonly ToolboxEntry SpinToolboxEntry = new ToolboxEntry(

      typeof(Spin),
      "A control for modifying a numeric value using up/down buttons",

      new DependencyProperty[] {
      },

      new PropertyCategory[] {
        new PropertyCategory("Behavior", new DependencyProperty[] {
          Spin.ChangeProperty,
          Spin.ValueProperty,
          Spin.MinimumProperty,
          Spin.MaximumProperty,
        }),
        new PropertyCategory("Appearance", new DependencyProperty[] {
        })
      }

      );
  }
}
