using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Maps a data type to a <see cref="DataTemplate"/>.
  /// </summary>
  public class TypeTemplate : IDataTemplateMatcher
  {
    /// <summary>
    /// The data type to which the <see cref="Template"/> property applies.
    /// </summary>
    public Type DataType { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="DataTemplate"/> to be used for the specified <see cref="DataType"/>.
    /// </summary>
    public DataTemplate Template { get; set; }

    /// <summary>
    /// Tests whether an object matches the criterion for this <see cref="DataTemplate"/>.
    /// </summary>
    /// <param name="item">The object to be tested.</param>
    /// <returns>true if the object matches the criterion; otherwise false.</returns>
    public virtual bool Matches(object item)
    {
      return (item != null && DataType.IsAssignableFrom(item.GetType()));
    }
  }
}
