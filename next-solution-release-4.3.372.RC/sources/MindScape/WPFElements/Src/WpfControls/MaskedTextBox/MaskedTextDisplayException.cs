using System;
using System.Runtime.Serialization;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Indicates that masked text cannot be displayed because of invalid styling or
  /// prompt representation (see <see cref="IDisplayElementStyleProvider"/> and
  /// <see cref="IPromptCharDisplaySelector"/>).
  /// </summary>
  [Serializable]
  public class MaskedTextDisplayException : Exception
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="MaskedTextDisplayException"/> class.
    /// </summary>
    public MaskedTextDisplayException() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MaskedTextDisplayException"/> class.
    /// </summary>
    /// <param name="message">The message associated with the exception.</param>
    public MaskedTextDisplayException(string message) 
      : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MaskedTextDisplayException"/> class.
    /// </summary>
    /// <param name="message">The message associated with the exception.</param>
    /// <param name="inner">The inner exception.</param>
    public MaskedTextDisplayException(string message, Exception inner) 
      : base(message, inner) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MaskedTextDisplayException"/> class.
    /// </summary>
    /// <param name="info">The serialized object data about the exception being thrown.</param>
    /// <param name="context">Contextual information about the source or destination.</param>
    protected MaskedTextDisplayException(SerializationInfo info, StreamingContext context) 
      : base(info, context) { }
  }
}
