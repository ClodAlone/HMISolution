using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// Provides inclusive range filtering logic for comparable types.
  /// </summary>
  public class RangeFilter : IFilter
  {
    private readonly IComparable _start;
    private readonly IComparable _end;

    /// <summary>
    /// Initializes a new instance of the <see cref="RangeFilter"/> class.
    /// </summary>
    /// <param name="start">The inclusive start value of the comparison range.</param>
    /// <param name="end">The inclusive end value of the comparison range.</param>
    public RangeFilter(IComparable start, IComparable end)
    {
      // TODO: check that the types are the same, or make this class generic
      if (start.CompareTo(end) > 0)
      {
        IComparable temp = start;
        start = end;
        end = temp;
      }
      _start = start;
      _end = end;
    }

    /// <summary>
    /// Gets the inclusive start value of the comparison range.
    /// </summary>
    public IComparable Start
    {
      get { return _start; }
    }

    /// <summary>
    /// Gets the inclusive end value of the comparison range.
    /// </summary>
    public IComparable End
    {
      get { return _end; }
    }

    /// <summary>
    /// Returns whether or not the given object is within the inclusive comparison range.
    /// </summary>
    /// <param name="o">The object to check.</param>
    /// <returns>True if the given object is within the inclusive comparison range. False otherwise.</returns>
    public bool IsMatch(object o)
    {
      if (o != null && o is IComparable)
      {
        IComparable comparable = (IComparable)o;
        // TODO: check that the types are the same, or make this class generic
        return comparable.CompareTo(Start) >= 0 && comparable.CompareTo(End) <= 0;
      }
      return false;
    }
  }
}
