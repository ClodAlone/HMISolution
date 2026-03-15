using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// Maps the comparable type of a <see cref="ComparableFilterDescription"/> to a <see cref="DataTemplate"/>.
  /// </summary>
  public class ComparableTypeTemplate : IDataTemplateMatcher
  {
    /// <summary>
    /// Tests whether an object matches the criterion for this <see cref="DataTemplate"/>.
    /// </summary>
    /// <param name="item">The object to be tested.</param>
    /// <returns>True if the object matches the criterion; otherwise false.</returns>
    public bool Matches(object item)
    {
      if (item is ComparableFilterDescription && ComparableType != null)
      {
        ComparableFilterDescription description = (ComparableFilterDescription)item;
        return (ComparableType.IsAssignableFrom(description.ComparableType));
      }
      return false;
    }

    /// <summary>
    /// Gets or sets the comparable type.
    /// </summary>
    public Type ComparableType { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="DataTemplate"/>.
    /// </summary>
    public DataTemplate Template { get; set; }
  }
}
