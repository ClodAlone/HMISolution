using System.Windows;

namespace Mindscape.WpfElements.Design
{
  internal partial class ToolboxEntries
  {
    internal static readonly ToolboxEntry DateTimePickerToolboxEntry = new ToolboxEntry(

      typeof(DateTimePicker),
      "A control for entering dates and times",

      new DependencyProperty[] {
        DateTimePicker.DisplayElementsProperty,
      },

      new PropertyCategory[] {
        new PropertyCategory("Behavior", new DependencyProperty[] {
          DateTimePicker.ValueProperty,
          DateTimePicker.CustomFormatProperty,
        }),
        new PropertyCategory("Appearance", new DependencyProperty[] {
          DateTimePicker.CultureProperty,
          DateTimePicker.FormatProperty,
        })
      }

      );
  }
}
