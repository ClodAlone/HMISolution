using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Mindscape.WpfElements.Design
{
  internal partial class ToolboxEntries
  {
    internal static readonly ToolboxEntry PaletteColorPickerToolboxEntry = new ToolboxEntry(

      typeof(PaletteColorPicker),
      "A control where users can select a color from a fixed palette",

      new DependencyProperty[] {
        PaletteColorPicker.PaletteProperty,
      },

      new PropertyCategory[] {
        new PropertyCategory("Appearance", new DependencyProperty[] {
          PaletteColorPicker.CollapsedViewTemplateProperty,
          PaletteColorPicker.SelectedColorProperty,
        }),
      }

      );
  }
}
