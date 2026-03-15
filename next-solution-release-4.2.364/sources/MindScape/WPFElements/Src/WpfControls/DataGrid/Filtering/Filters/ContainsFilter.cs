using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// Provides filtering logic for contained strings.
  /// </summary>
  public class ContainsFilter : IFilter
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ContainsFilter"/>.
    /// </summary>
    /// <param name="value">The comparison string.</param>
    /// <param name="matchCase">The case sensitivity.</param>
    public ContainsFilter(string value, bool matchCase)
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
    /// Returns true if the given string contains the Value string.
    /// </summary>
    /// <param name="o">The object to check</param>
    /// <returns>True if the given string contains the Value string. False otherwise.</returns>
    public bool IsMatch(object o)
    {
      if (o != null && Value != null)
      {
        if (!MatchCase)
        {
          return o.ToString().ToLower(CultureInfo.CurrentCulture).Contains(Value.ToLower(CultureInfo.CurrentCulture));
        }
        return o.ToString().Contains(Value);
      }
      return false;
    }
  }
}
