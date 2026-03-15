using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Mindscape.WpfElements.Design
{
  internal partial class ToolboxEntries
  {
    internal static readonly ToolboxEntry HsvColorPickerToolboxEntry = new ToolboxEntry(

      typeof(HsvColorPicker),
      "A control for editing the hue, saturation and value channels of a color",

      new DependencyProperty[] {
        HsvColorPicker.NotifyingColorProperty,
      },

      new PropertyCategory[] {
        new PropertyCategory("Behavior", new DependencyProperty[] {
          HsvColorPicker.SelectedColorProperty,
        }),
      }

      );
  }
}
