
namespace Mindscape.WpfElements
{
  /// <summary>
  /// Represents the type of input expected in the position represented by
  /// an <see cref="IPrompt"/>.
  /// </summary>
  public enum ExpectedInputType
  {
    /// <summary>
    /// The position expects a digit.
    /// </summary>
    Digit,

    /// <summary>
    /// The position expects a letter.
    /// </summary>
    Letter,

    /// <summary>
    /// The position expects a digit or a letter.
    /// </summary>
    Alphanumeric,

    /// <summary>
    /// The position expects any printable character.
    /// </summary>
    Any
  }
}
