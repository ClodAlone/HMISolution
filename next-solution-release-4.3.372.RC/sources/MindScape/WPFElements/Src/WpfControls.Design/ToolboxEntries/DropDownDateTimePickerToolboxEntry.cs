using System.Windows;

namespace Mindscape.WpfElements.Design
{
  internal partial class ToolboxEntries
  {
    internal static readonly ToolboxEntry DropDownDateTimePickerToolboxEntry = new ToolboxEntry(

      typeof(DropDownDatePicker),
      "A control for entering dates, with support for a drop-down calendar.",

      new DependencyProperty[] {
      },

      new PropertyCategory[] {
        new PropertyCategory("Behavior", new DependencyProperty[] {
          DropDownDatePicker.ValueProperty,
          DropDownDatePicker.CustomFormatProperty,
        }),
        new PropertyCategory("Appearance", new DependencyProperty[] {
          DropDownDatePicker.CultureProperty,
          DropDownDatePicker.FormatProperty,
        })
      }

      );
  }
}
