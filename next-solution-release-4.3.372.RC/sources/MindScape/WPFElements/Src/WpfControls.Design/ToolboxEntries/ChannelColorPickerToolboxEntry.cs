using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Mindscape.WpfElements.Design
{
  internal partial class ToolboxEntries
  {
    internal static readonly ToolboxEntry ChannelColorPickerToolboxEntry = new ToolboxEntry(

      typeof(ChannelColorPicker),
      "A control for editing the red, green, blue and alpha channels of a color",

      new DependencyProperty[] {
        ChannelColorPicker.NotifyingColorProperty,
      },

      new PropertyCategory[] {
        new PropertyCategory("Behavior", new DependencyProperty[] {
          ChannelColorPicker.SelectedColorProperty,
        }),
      }

      );
  }
}
