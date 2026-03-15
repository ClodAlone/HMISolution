using System.Windows;

namespace Mindscape.WpfElements.Design
{
  internal partial class ToolboxEntries
  {
    internal static readonly ToolboxEntry MaskedTextBoxToolboxEntry = new ToolboxEntry(

      typeof(MaskedTextBox),
      "A text box that uses a mask to prevent incorrect user input",

      new DependencyProperty[] {
        MaskedTextBox.LastOperationResultProperty,
        MaskedTextBox.LastOperationSucceededProperty,
        MaskedTextBox.IsMaskCompletedProperty,
        MaskedTextBox.IsMaskFullProperty,
      },

      new PropertyCategory[] {
        new PropertyCategory("Behavior", new DependencyProperty[] {
          MaskedTextBox.MaskProperty,
          MaskedTextBox.AutoSkipLiteralsProperty,
        }),
        new PropertyCategory("Appearance", new DependencyProperty[] {
          MaskedTextBox.TextProperty,
          MaskedTextBox.PromptCharDisplaySelectorProperty,
          MaskedTextBox.LiteralStyleProperty,
          MaskedTextBox.InputStyleProperty,
          MaskedTextBox.PromptStyleProperty,
        })
      }

      );
  }
}
