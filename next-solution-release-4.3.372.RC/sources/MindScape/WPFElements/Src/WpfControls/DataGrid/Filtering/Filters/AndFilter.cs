using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// A filter that will match only if all of the sub filters match.
  /// </summary>
  public class AndFilter : IFilter
  {
    private readonly IList<IFilter> _filters = new List<IFilter>();

    /// <summary>
    /// Adds the given <see cref="IFilter"/> to this <see cref="AndFilter"/>.
    /// </summary>
    /// <param name="filter">The <see cref="IFilter"/> to add.</param>
    public void Add(IFilter filter)
    {
      if(filter == null)
      {
        return;
      }
      foreach (IFilter f in _filters)
      {
        if (f.Equals(filter))
        {
          return;
        }
      }
      _filters.Add(filter);
    }

    internal ReadOnlyCollection<IFilter> Filters
    {
      get { return new ReadOnlyCollection<IFilter>(_filters); }
    }

    /// <summary>
    /// Returns whether or not the given object matches all of the sub filters.
    /// </summary>
    /// <param name="o">The object to check.</param>
    /// <returns>True if the given object matches all of the sub filters. False otherwise.</returns>
    public bool IsMatch(object o)
    {
      foreach (IFilter filter in _filters)
      {
        bool isMatch = filter.IsMatch(o);
        if (!isMatch)
        {
          return false;
        }
      }
      return true;
    }
  }
}
