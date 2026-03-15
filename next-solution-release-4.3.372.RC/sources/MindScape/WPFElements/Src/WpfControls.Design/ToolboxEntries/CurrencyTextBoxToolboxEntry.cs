using System.Windows;

namespace Mindscape.WpfElements.Design
{
  internal partial class ToolboxEntries
  {
    internal static readonly ToolboxEntry CurrencyTextBoxToolboxEntry = new ToolboxEntry(

      typeof(CurrencyTextBox),
      "A text box for editing currency values",

      new DependencyProperty[] {
      },

      new PropertyCategory[] {
        new PropertyCategory("Behavior", new DependencyProperty[] {
          CurrencyTextBox.ValueProperty,
          CurrencyTextBox.MaximumProperty,
          CurrencyTextBox.MinimumProperty,
          CurrencyTextBox.SelectAllOnEntryProperty,
        }),
        new PropertyCategory("Appearance", new DependencyProperty[] {
          CurrencyTextBox.CultureProperty,
          CurrencyTextBox.ShowSeparatorsProperty,
        })
      }

      );
  }
}
