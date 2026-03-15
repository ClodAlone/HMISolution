using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// Provides filter logic for the equality of 2 objects.
  /// </summary>
  public class EqualsFilter : IFilter
  {
    private readonly object _value;

    /// <summary>
    /// Initializes a new instance of the <see cref="EqualsFilter"/>.
    /// </summary>
    /// <param name="value">The comparison value.</param>
    public EqualsFilter(object value)
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
    /// Returns whether or not the given object equals the comparison value.
    /// </summary>
    /// <param name="o">The object to check.</param>
    /// <returns>True if the given object matches the comparison value. False otherwise.</returns>
    public bool IsMatch(object o)
    {
      if (o == null)
      {
        return Value == null;
      }
      // TODO: need to support comparing 2 different number types that are actually the same value. e.g. 3.0 vs 3
      return o.Equals(Value);
    }
  }
}
