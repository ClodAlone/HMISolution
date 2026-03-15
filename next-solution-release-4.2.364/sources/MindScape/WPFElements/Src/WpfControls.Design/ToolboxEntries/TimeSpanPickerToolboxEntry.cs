using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Mindscape.WpfElements.Design
{
  internal partial class ToolboxEntries
  {
    internal static readonly ToolboxEntry TimeSpanPickerToolboxEntry = new ToolboxEntry(

      typeof(TimeSpanPicker),
      "A control for selecting a duration",

      new DependencyProperty[] {
      },

      new PropertyCategory[] {
        new PropertyCategory("Appearance", new DependencyProperty[] {
          TimeSpanPicker.IsDropDownOpenProperty,
          TimeSpanPicker.IsDropDownToggleVisibleProperty,
        }),
        new PropertyCategory("Behavior", new DependencyProperty[] {
          TimeSpanPicker.ItemsSourceProperty,
          TimeSpanPicker.ChangeProperty,
        }),
        new PropertyCategory("Data", new DependencyProperty[] {
          TimeSpanPicker.TextProperty,
          TimeSpanPicker.SelectedTimeSpanProperty,
        }),
      }

      );
  }
}
