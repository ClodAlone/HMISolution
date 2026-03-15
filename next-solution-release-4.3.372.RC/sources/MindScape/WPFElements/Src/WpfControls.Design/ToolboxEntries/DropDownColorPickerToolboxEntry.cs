using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Mindscape.WpfElements.Design
{
  internal partial class ToolboxEntries
  {
    internal static readonly ToolboxEntry DropDownColorPickerToolboxEntry = new ToolboxEntry(

      typeof(DropDownColorPicker),
      "A control with a drop down panel containing UI for selecting colors",

      new DependencyProperty[] {
        DropDownColorPicker.PaletteProperty,
        DropDownColorPicker.IsDropDownOpenProperty,
        DropDownColorPicker.RecentColorsProperty,
      },

      new PropertyCategory[] {
        new PropertyCategory("Behavior", new DependencyProperty[] {
          DropDownColorPicker.SelectedColorProperty,
          DropDownColorPicker.StaysOpenProperty,
        }),
        new PropertyCategory("Appearance", new DependencyProperty[] {
          DropDownColorPicker.HeaderTemplateProperty,
          DropDownColorPicker.DropDownTemplateProperty,
        }),
      }

      );
  }
}
