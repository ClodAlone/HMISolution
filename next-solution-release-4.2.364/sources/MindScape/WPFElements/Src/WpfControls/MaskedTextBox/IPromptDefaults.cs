
namespace Mindscape.WpfElements
{
  /// <summary>
  /// Provides information about display defaults for a prompt 
  /// position in a <see cref="MaskedTextBox"/>.
  /// </summary>
  public interface IPromptDefaults
  {
    /// <summary>
    /// Gets the character displayed by default for prompts.
    /// </summary>
    char PromptChar { get; }
  }
}
