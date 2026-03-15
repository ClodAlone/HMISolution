using System.Windows;

namespace Mindscape.WpfElements.Design
{
  internal partial class ToolboxEntries
  {
    internal static readonly ToolboxEntry DropDownToolboxEntry = new ToolboxEntry(

      typeof(DropDownEditBox),
      "A control for displaying a summary and a drop-down detail pane, similar to a combo box",

      new DependencyProperty[] {
        DropDownEditBox.ContentProperty,
        DropDownEditBox.HeaderTemplateProperty,
        DropDownEditBox.DropDownTemplateProperty,
      },

      new PropertyCategory[] {
        new PropertyCategory("Behavior", new DependencyProperty[] {
          DropDownEditBox.IsDropDownOpenProperty,
        }),
        new PropertyCategory("Appearance", new DependencyProperty[] {
          DropDownEditBox.MaxDropDownHeightProperty,
        })
      }

      );
  }
}
