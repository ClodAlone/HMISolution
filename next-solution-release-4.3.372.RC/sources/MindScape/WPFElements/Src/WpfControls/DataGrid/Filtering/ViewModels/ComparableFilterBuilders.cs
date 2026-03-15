using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// Provides a way to build IFilter objects.
  /// </summary>
  public interface IComparableFilterBuilder : IFilterBuilder
  {
    /// <summary>
    /// Builds an <see cref="IFilter"/>.
    /// </summary>
    /// <param name="value1">The first value of the <see cref="IFilter"/>.</param>
    /// <param name="value2">The second value of the <see cref="IFilter"/>.</param>
    /// <returns>An IFilter.</returns>
    IFilter Build(IComparable value1, IComparable value2);
  }

  /// <summary>
  /// Does not build a filter. Useful for select no filter from a drop down.
  /// </summary>
  public class NoFilterBuilder : IComparableFilterBuilder
  {
    /// <summary>
    /// Gets the display name of the <see cref="NoFilterBuilder"/>.
    /// </summary>
    public string Name
    {
      get { return ""; }
    }

    /// <summary>
    /// Always returns null.
    /// </summary>
    /// <param name="value1">Unused.</param>
    /// <param name="value2">Unused.</param>
    /// <returns>null.</returns>
    public IFilter Build(IComparable value1, IComparable value2)
    {
      return null;
    }
  }

  /// <summary>
  /// Builds a <see cref="GreaterThanFilter"/>.
  /// </summary>
  public class GreaterThanFilterBuilder : IComparableFilterBuilder
  {
    /// <summary>
    /// Gets the display name of the <see cref="GreaterThanFilter"/>.
    /// </summary>
    public string Name
    {
      get { return "Greater than"; }
    }

    /// <summary>
    /// Builds a <see cref="GreaterThanFilter"/>.
    /// </summary>
    /// <param name="value1">The comparison value.</param>
    /// <param name="value2">Unused.</param>
    /// <returns>A <see cref="GreaterThanFilter"/> based of the given options.</returns>
    public IFilter Build(IComparable value1, IComparable value2)
    {
      return new GreaterThanFilter(value1);
    }
  }

  /// <summary>
  /// Builds a <see cref="LessThanFilter"/>.
  /// </summary>
  public class LessThanFilterBuilder : IComparableFilterBuilder
  {
    /// <summary>
    /// Gets the display name of the <see cref="LessThanFilter"/>.
    /// </summary>
    public string Name
    {
      get { return "Less than"; }
    }

    /// <summary>
    /// Builds a <see cref="LessThanFilter"/>.
    /// </summary>
    /// <param name="value1">The comparison value.</param>
    /// <param name="value2">Unused.</param>
    /// <returns>A <see cref="LessThanFilter"/> based of the given options.</returns>
    public IFilter Build(IComparable value1, IComparable value2)
    {
      return new LessThanFilter(value1);
    }
  }

  /// <summary>
  /// Builds a <see cref="GreaterThanOrEqualToFilter"/>.
  /// </summary>
  public class GreaterThanOrEqualToFilterBuilder : IComparableFilterBuilder
  {
    /// <summary>
    /// Gets the display name of the <see cref="GreaterThanOrEqualToFilter"/>.
    /// </summary>
    public string Name
    {
      get { return "Greater than or equal to"; }
    }

    /// <summary>
    /// Builds a <see cref="GreaterThanOrEqualToFilter"/>.
    /// </summary>
    /// <param name="value1">The comparison value.</param>
    /// <param name="value2">Unused.</param>
    /// <returns>A <see cref="GreaterThanOrEqualToFilter"/> based of the given options.</returns>
    public IFilter Build(IComparable value1, IComparable value2)
    {
      return new GreaterThanOrEqualToFilter(value1);
    }
  }

  /// <summary>
  /// Builds a <see cref="LessThanOrEqualToFilter"/>.
  /// </summary>
  public class LessThanOrEqualToFilterBuilder : IComparableFilterBuilder
  {
    /// <summary>
    /// Gets the display name of the <see cref="LessThanOrEqualToFilter"/>.
    /// </summary>
    public string Name
    {
      get { return "Less than or equal to"; }
    }

    /// <summary>
    /// Builds a <see cref="LessThanOrEqualToFilter"/>.
    /// </summary>
    /// <param name="value1">The comparison value.</param>
    /// <param name="value2">Unused.</param>
    /// <returns>A <see cref="LessThanOrEqualToFilter"/> based of the given options.</returns>
    public IFilter Build(IComparable value1, IComparable value2)
    {
      return new LessThanOrEqualToFilter(value1);
    }
  }

  /// <summary>
  /// Builds an <see cref="EqualsFilter"/>.
  /// </summary>
  public class EqualsFilterBuilder : IComparableFilterBuilder
  {
    /// <summary>
    /// Gets the display name of the <see cref="EqualsFilter"/>.
    /// </summary>
    public string Name
    {
      get { return "Equals"; }
    }

    /// <summary>
    /// Builds a <see cref="EqualsFilter"/>.
    /// </summary>
    /// <param name="value1">The comparison value.</param>
    /// <param name="value2">Unused.</param>
    /// <returns>A <see cref="EqualsFilter"/> based of the given options.</returns>
    public IFilter Build(IComparable value1, IComparable value2)
    {
      return new EqualsFilter(value1);
    }
  }

  /// <summary>
  /// Builds a <see cref="NotEqualFilter"/>.
  /// </summary>
  public class NotEqualFilterBuilder : IComparableFilterBuilder
  {
    /// <summary>
    /// Gets the display name of the <see cref="NotEqualFilter"/>.
    /// </summary>
    public string Name
    {
      get { return "Does not equal"; }
    }

    /// <summary>
    /// Builds a <see cref="NotEqualFilter"/>.
    /// </summary>
    /// <param name="value1">The comparison value.</param>
    /// <param name="value2">Unused.</param>
    /// <returns>A <see cref="NotEqualFilter"/> based of the given options.</returns>
    public IFilter Build(IComparable value1, IComparable value2)
    {
      return new NotEqualFilter(value1);
    }
  }
}
