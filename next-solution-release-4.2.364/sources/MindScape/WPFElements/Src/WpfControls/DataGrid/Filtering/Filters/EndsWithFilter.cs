using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// Provides filtering logic for the end of a string value.
  /// </summary>
  public class EndsWithFilter : IFilter
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="EndsWithFilter"/>.
    /// </summary>
    /// <param name="value">The comparison string.</param>
    /// <param name="matchCase">The case sensitivity.</param>
    public EndsWithFilter(string value, bool matchCase)
    {
      Value = value;
      MatchCase = matchCase;
    }

    /// <summary>
    /// Gets or sets the comparison string.
    /// </summary>
    public string Value { get; private set; }

    /// <summary>
    /// Gets whether or not this filter matches casing.
    /// </summary>
    public bool MatchCase { get; private set; }

    /// <summary>
    /// Returns true if the given string ends with the Value string.
    /// </summary>
    /// <param name="o">The object to check</param>
    /// <returns>True if the given string ends with the Value string. False otherwise.</returns>
    public bool IsMatch(object o)
    {
      if (o != null && Value != null)
      {
        return o.ToString().EndsWith(Value, !MatchCase, CultureInfo.CurrentCulture);
      }
      return false;
    }
  }
}
