using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Mindscape.WpfElements.Design
{
  internal partial class ToolboxEntries
  {
    internal static readonly ToolboxEntry RichTextToolBarToolboxEntry = new ToolboxEntry(

      typeof(RichTextToolBar),
      "Provides a user interface for applying formatting and commands to a RichTextBox",

      new DependencyProperty[] {
      },

      new PropertyCategory[] {
        new PropertyCategory("Behavior", new DependencyProperty[] {
          RichTextToolBar.RichTextBoxProperty,
        }),
      }

      );
  }
}
