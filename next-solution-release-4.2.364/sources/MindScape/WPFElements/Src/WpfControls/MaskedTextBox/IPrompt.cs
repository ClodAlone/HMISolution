
namespace Mindscape.WpfElements
{
  /// <summary>
  /// Represents a position where the <see cref="MaskedTextBox"/> is prompting for input.
  /// </summary>
  public interface IPrompt
  {
    /// <summary>
    /// Gets the type of input expected.
    /// </summary>
    ExpectedInputType ExpectedInputType { get; }

    /// <summary>
    /// Gets whether input is optional at this position.
    /// </summary>
    bool IsOptional { get; }

    /// <summary>
    /// Gets default display information.
    /// </summary>
    IPromptDefaults DisplayContext { get; }
  }
}
