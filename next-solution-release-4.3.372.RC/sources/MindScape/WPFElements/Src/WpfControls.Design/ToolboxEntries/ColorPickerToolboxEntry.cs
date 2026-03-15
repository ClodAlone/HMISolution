using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Mindscape.WpfElements.Design
{
  internal partial class ToolboxEntries
  {
    internal static readonly ToolboxEntry ColorPickerToolboxEntry = new ToolboxEntry(

      typeof(ColorPicker),
      "A control for selecting colors",

      new DependencyProperty[] {
        ColorPicker.PaletteProperty,
        ColorPicker.RecentColorsProperty,
      },

      new PropertyCategory[] {
        new PropertyCategory("Behavior", new DependencyProperty[] {
          ColorPicker.SelectedColorProperty,
        }),
      }

      );
  }
}
