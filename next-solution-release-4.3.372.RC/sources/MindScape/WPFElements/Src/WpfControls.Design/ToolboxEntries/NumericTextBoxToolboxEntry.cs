using System.Windows;

namespace Mindscape.WpfElements.Design
{
  internal partial class ToolboxEntries
  {
    internal static readonly ToolboxEntry NumericTextBoxToolboxEntry = new ToolboxEntry(

      typeof(NumericTextBox),
      "A text box for editing numeric values",

      new DependencyProperty[] {
      },

      new PropertyCategory[] {
        new PropertyCategory("Behavior", new DependencyProperty[] {
          NumericTextBox.ValueProperty,
          NumericTextBox.MaximumProperty,
          NumericTextBox.MinimumProperty,
          NumericTextBox.SelectAllOnEntryProperty,
        }),
        new PropertyCategory("Appearance", new DependencyProperty[] {
          NumericTextBox.CultureProperty,
          NumericTextBox.ShowSeparatorsProperty,
        })
      }

      );
  }
}
