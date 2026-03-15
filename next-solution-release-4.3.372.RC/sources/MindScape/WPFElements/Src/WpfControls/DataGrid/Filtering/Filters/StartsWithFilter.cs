using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// Provides filtering logic for the start of a string value.
  /// </summary>
  public class StartsWithFilter : IFilter
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="StartsWithFilter"/>.
    /// </summary>
    /// <param name="value">The comparison string.</param>
    /// <param name="matchCase">The case sensitivity.</param>
    public StartsWithFilter(string value, bool matchCase)
    {
      Value = value;
      MatchCase = matchCase;
    }

    /// <summary>
    /// Gets the comparison string.
    /// </summary>
    public string Value { get; private set; }

    /// <summary>
    /// Gets whether or not this filter matches casing.
    /// </summary>
    public bool MatchCase { get; private set; }

    /// <summary>
    /// Returns true if the given string starts with the Value string.
    /// </summary>
    /// <param name="o">The object to check</param>
    /// <returns>True if the given string starts with the Value string. False otherwise.</returns>
    public bool IsMatch(object o)
    {
      if (o != null && Value != null)
      {
        return o.ToString().StartsWith(Value, !MatchCase, CultureInfo.CurrentCulture);
      }
      return false;
    }
  }
}
