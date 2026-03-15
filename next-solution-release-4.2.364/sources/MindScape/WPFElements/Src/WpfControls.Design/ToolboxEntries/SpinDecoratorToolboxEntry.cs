using System.Windows;

namespace Mindscape.WpfElements.Design
{
  internal partial class ToolboxEntries
  {
    internal static readonly ToolboxEntry SpinDecoratorToolboxEntry = new ToolboxEntry(

      typeof(SpinDecorator),
      "Associates a control with a Spin control",

      new DependencyProperty[] {
      },

      new PropertyCategory[] {
        new PropertyCategory("Behavior", new DependencyProperty[] {
          SpinDecorator.ChangeProperty,
          SpinDecorator.ValuePropertyProperty,
          SpinDecorator.MinimumPropertyProperty,
          SpinDecorator.MaximumPropertyProperty,
        }),
        new PropertyCategory("Appearance", new DependencyProperty[] {
          SpinDecorator.ShowSpinUIProperty,
        })
      }

      );
  }
}
