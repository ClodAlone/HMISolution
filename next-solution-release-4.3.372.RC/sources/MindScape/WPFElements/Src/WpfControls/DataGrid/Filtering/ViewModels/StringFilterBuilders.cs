using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// Provides a way to build IFilter objects.
  /// </summary>
  public interface IStringFilterBuilder : IFilterBuilder
  {
    /// <summary>
    /// Builds an <see cref="IFilter"/>.
    /// </summary>
    /// <param name="value1">The first value for the IFilter.</param>
    /// <param name="value2">The second value for the IFilter.</param>
    /// <param name="matchCase">The case sensitivity of the IFilter.</param>
    /// <returns>An IFilter.</returns>
    IFilter Build(string value1, string value2, bool matchCase);
  }

  /// <summary>
  /// Builds a <see cref="StartsWithFilter"/>.
  /// </summary>
  public class StartsWithFilterBuilder : IStringFilterBuilder
  {
    /// <summary>
    /// Returns the display name of the <see cref="StartsWithFilter"/>.
    /// </summary>
    public string Name
    {
      get { return "Starts with"; }
    }

    /// <summary>
    /// Builds a <see cref="StartsWithFilter"/>.
    /// </summary>
    /// <param name="value1">The comparison value.</param>
    /// <param name="value2">Unused.</param>
    /// <param name="matchCase">The case sensitivity of the <see cref="StartsWithFilter"/>.</param>
    /// <returns>A <see cref="StartsWithFilter"/> based on the given options.</returns>
    public IFilter Build(string value1, string value2, bool matchCase)
    {
      return String.IsNullOrEmpty(value1) ? null : new StartsWithFilter(value1, matchCase);
    }
  }

  /// <summary>
  /// Builds a <see cref="EndsWithFilter"/>.
  /// </summary>
  public class EndsWithFilterBuilder : IStringFilterBuilder
  {
    /// <summary>
    /// Returns the display name of the <see cref="EndsWithFilter"/>.
    /// </summary>
    public string Name
    {
      get { return "Ends with"; }
    }

    /// <summary>
    /// Builds a <see cref="EndsWithFilter"/>.
    /// </summary>
    /// <param name="value1">The comparison value.</param>
    /// <param name="value2">Unused.</param>
    /// <param name="matchCase">The case sensitivity of the <see cref="EndsWithFilter"/>.</param>
    /// <returns>A <see cref="EndsWithFilter"/> based on the given options.</returns>
    public IFilter Build(string value1, string value2, bool matchCase)
    {
      return String.IsNullOrEmpty(value1) ? null : new EndsWithFilter(value1, matchCase);
    }
  }

  /// <summary>
  /// Builds a <see cref="ContainsFilter"/>.
  /// </summary>
  public class ContainsFilterBuilder : IStringFilterBuilder
  {
    /// <summary>
    /// Returns the display name of the <see cref="ContainsFilter"/>.
    /// </summary>
    public string Name
    {
      get { return "Contains"; }
    }

    /// <summary>
    /// Builds a <see cref="ContainsFilter"/>.
    /// </summary>
    /// <param name="value1">The comparison value.</param>
    /// <param name="value2">Unused.</param>
    /// <param name="matchCase">The case sensitivity of the <see cref="ContainsFilter"/>.</param>
    /// <returns>A <see cref="ContainsFilter"/> based on the given options.</returns>
    public IFilter Build(string value1, string value2, bool matchCase)
    {
      return String.IsNullOrEmpty(value1) ? null : new ContainsFilter(value1, matchCase);
    }
  }
}
