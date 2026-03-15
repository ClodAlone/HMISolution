using System.Windows;
using System.Windows.Documents;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Supports customization of the display of prompts in a <see cref="MaskedTextBox"/>.
  /// </summary>
  public interface IPromptCharDisplaySelector
  {
    /// <summary>
    /// Gets whether the implementation provides a custom presentation for the specified
    /// prompt.  If this returns false, the <see cref="MaskedTextBox"/> will use the default
    /// presentation (the prompt character).
    /// </summary>
    /// <param name="prompt">The prompt.</param>
    /// <returns>true if there is a custom presentation for this prompt, otherwise false.</returns>
    bool OverridesRepresentation(IPrompt prompt);

    /// <summary>
    /// Creates the custom presentation for the specified prompt.
    /// </summary>
    /// <param name="prompt">The prompt.</param>
    /// <returns>An <see cref="Inline"/> to represent the prompt in the <see cref="MaskedTextBox"/>.
    /// This should contain exactly one character or <see cref="UIElement"/>.  The MaskedTextBox
    /// will throw an exception if an implementation returns an <see cref="Inline"/> containing
    /// a run of more than one character or element.</returns>
    Inline CreateRepresentation(IPrompt prompt);
  }
}
