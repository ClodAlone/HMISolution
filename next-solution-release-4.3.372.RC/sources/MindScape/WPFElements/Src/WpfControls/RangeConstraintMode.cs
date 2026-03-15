using System;
using System.Collections.Generic;
using System.Text;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Specifies when a numeric text box enforces range (minimum
  /// and maximum) constraints.
  /// </summary>
  public enum RangeConstraintMode
  {
    /// <summary>
    /// Range constraints are enforced at all times.  The user is not permitted
    /// to edit the value in a way that would take it out of range.
    /// </summary>
    Always,

    /// <summary>
    /// Range constraints are not enforced while the user is editing the text
    /// box.  When the text box loses focus, if the value is below
    /// the minimum, it is reset to the minimum, and if value is
    /// above the maximum, it is reset to the maximum.
    /// </summary>
    OnLostFocus,

    /// <summary>
    /// Range constraints are not enforced while the user is editing the text
    /// box, unless the user presses the Return key.  When the text box loses focus
    /// or the user presses the Return key, if the value is below
    /// the minimum, it is reset to the minimum, and if the value is
    /// above the maximum, it is reset to the maximum.
    /// </summary>
    OnLostFocusOrReturn
  }
}
