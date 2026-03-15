using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// Provides 'not equal' filter logic.
  /// </summary>
  public class NotEqualFilter : IFilter
  {
    private readonly object _value;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotEqualFilter"/>.
    /// </summary>
    /// <param name="value">The comparison value.</param>
    public NotEqualFilter(object value)
    {
      _value = value;
    }

    /// <summary>
    /// Gets the comparison value.
    /// </summary>
    public object Value
    {
      get { return _value; }
    }

    /// <summary>
    /// Returns whether or not the given value is not equal to the comparison value.
    /// </summary>
    /// <param name="o">The object to check.</param>
    /// <returns>True if the given object does not equal the comparison value. False otherwise.</returns>
    public bool IsMatch(object o)
    {
      if (o == null)
      {
        return Value != null;
      }
      return !o.Equals(Value);
    }
  }
}
