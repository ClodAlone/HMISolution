using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// Provides a way to build boolean IFilter objects.
  /// </summary>
  public interface IBooleanFilterBuilder : IFilterBuilder
  {
    /// <summary>
    /// Builds an <see cref="IFilter"/>.
    /// </summary>
    /// <param name="firstFilter">The first part of the boolean filter.</param>
    /// <param name="secondFilter">The second part of the boolean filter.</param>
    /// <returns>A boolean <see cref="IFilter"/>.</returns>
    IFilter Build(IFilter firstFilter, IFilter secondFilter);
  }

  /// <summary>
  /// Builds an <see cref="AndFilter"/>.
  /// </summary>
  public class AndFilterBuilder : IBooleanFilterBuilder
  {
    /// <summary>
    /// Gets the display name of the <see cref="AndFilter"/>.
    /// </summary>
    public string Name
    {
      get { return "And"; }
    }

    /// <summary>
    /// Builds an <see cref="AndFilter"/>.
    /// </summary>
    /// <param name="firstFilter">The first part of the <see cref="AndFilter"/>.</param>
    /// <param name="secondFilter">The second part of the <see cref="AndFilter"/>.</param>
    /// <returns>An <see cref="AndFilter"/> containing the given filters.</returns>
    public IFilter Build(IFilter firstFilter, IFilter secondFilter)
    {
      if (firstFilter == null && secondFilter == null)
      {
        return null;
      }
      if (firstFilter != null && secondFilter == null)
      {
        return firstFilter;
      }
      if (firstFilter == null && secondFilter != null)
      {
        return secondFilter;
      }
      AndFilter filter = new AndFilter();
      filter.Add(firstFilter);
      filter.Add(secondFilter);
      return filter;
    }
  }

  /// <summary>
  /// Builds an <see cref="OrFilter"/>.
  /// </summary>
  public class OrFilterBuilder : IBooleanFilterBuilder
  {
    /// <summary>
    /// Gets the display name of the <see cref="OrFilter"/>.
    /// </summary>
    public string Name
    {
      get { return "Or"; }
    }

    /// <summary>
    /// Builds an <see cref="OrFilter"/>.
    /// </summary>
    /// <param name="firstFilter">The first part of the <see cref="OrFilter"/>.</param>
    /// <param name="secondFilter">The second part of the <see cref="OrFilter"/>.</param>
    /// <returns>An <see cref="OrFilter"/> containing the given filters.</returns>
    public IFilter Build(IFilter firstFilter, IFilter secondFilter)
    {
      if (firstFilter == null && secondFilter == null)
      {
        return null;
      }
      if (firstFilter != null && secondFilter == null)
      {
        return firstFilter;
      }
      if (firstFilter == null && secondFilter != null)
      {
        return secondFilter;
      }
      OrFilter filter = new OrFilter();
      filter.Add(firstFilter);
      filter.Add(secondFilter);
      return filter;
    }
  }
}
