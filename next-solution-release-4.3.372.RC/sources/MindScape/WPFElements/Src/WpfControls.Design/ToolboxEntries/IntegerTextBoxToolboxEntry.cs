using System.Windows;

namespace Mindscape.WpfElements.Design
{
  internal partial class ToolboxEntries
  {
    internal static readonly ToolboxEntry IntegerTextBoxToolboxEntry = new ToolboxEntry(

      typeof(IntegerTextBox),
      "A text box for editing integer values",

      new DependencyProperty[] {
      },

      new PropertyCategory[] {
        new PropertyCategory("Behavior", new DependencyProperty[] {
          IntegerTextBox.ValueProperty,
          IntegerTextBox.MaximumProperty,
          IntegerTextBox.MinimumProperty,
          IntegerTextBox.SelectAllOnEntryProperty,
        }),
        new PropertyCategory("Appearance", new DependencyProperty[] {
          IntegerTextBox.CultureProperty,
          IntegerTextBox.PrecisionProperty,
          IntegerTextBox.ShowSeparatorsProperty,
        })
      }

      );
  }
}
