using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Mindscape.WpfElements.Design
{
  internal partial class ToolboxEntries
  {
    internal static readonly ToolboxEntry TimePickerToolboxEntry = new ToolboxEntry(

      typeof(TimePicker),
      "A control for selecting a time of day",

      new DependencyProperty[] {
      },

      new PropertyCategory[] {
        new PropertyCategory("Appearance", new DependencyProperty[] {
          TimePicker.IsDropDownOpenProperty,
          TimePicker.IsDropDownToggleVisibleProperty,
        }),
        new PropertyCategory("Behavior", new DependencyProperty[] {
          TimePicker.ItemsSourceProperty,
          TimePicker.ChangeProperty,
        }),
        new PropertyCategory("Data", new DependencyProperty[] {
          TimePicker.TextProperty,
          TimePicker.SelectedTimeProperty,
        }),
      }

      );
  }
}
