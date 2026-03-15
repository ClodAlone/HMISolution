using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// Provides 'less than' filter logic.
  /// </summary>
  public class LessThanFilter : IFilter
  {
    private readonly IComparable _value;

    /// <summary>
    /// Initializes a new instance of the <see cref="LessThanFilter"/>.
    /// </summary>
    /// <param name="value">The comparison value.</param>
    public LessThanFilter(IComparable value)
    {
      _value = value;
      if (NumericalUtils.IsPrimitiveNumerical(_value))
      {
        _value = NumericalUtils.ConvertToDouble(_value);
      }
    }

    /// <summary>
    /// Gets the comparison value.
    /// </summary>
    public IComparable Value
    {
      get { return _value; }
    }

    /// <summary>
    /// Returns whether or not the given object is less than the comparison value.
    /// </summary>
    /// <param name="o">The object to check.</param>
    /// <returns>True if the given object is less than the comparison value. False otherwise.</returns>
    public bool IsMatch(object o)
    {
      if (o != null && o is IComparable)
      {
        IComparable comparable = (IComparable)o;
        if (NumericalUtils.IsPrimitiveNumerical(comparable))
        {
          comparable = NumericalUtils.ConvertToDouble(comparable);
        }
        return comparable.CompareTo(Value) < 0;
      }
      return false;
    }
  }
}
