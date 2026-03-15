using System.Windows.Controls;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Identifies the type of display element in a <see cref="DateTimePicker"/>
  /// control.  This is typically used in a <see cref="DataTemplateSelector"/>
  /// to present appropriate user interfaces for different types of element.
  /// </summary>
  public enum DateTimeDisplayElementType
  {
    /// <summary>
    /// The element is presented as a numeric value.  For example, the year.
    /// Elements of this type are instances of <see cref="DateTimeNumericDisplayElement"/>.
    /// </summary>
    Numeric,

    /// <summary>
    /// The element is presented as text, and the user may select only from
    /// pre-defined values.  For example, the month when displayed in textual format
    /// (the "MMM" family of format strings).
    /// Elements of this type are instances of <see cref="DateTimeSelectDisplayElement"/>.
    /// </summary>
    Select,

    /// <summary>
    /// The element may not be edited by the user.  For example, the day of the
    /// week, a separator, or literal text.
    /// Elements of this type are instances of <see cref="DateTimeReadOnlyDisplayElement"/>.
    /// </summary>
    ReadOnly
  }
}
