using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Maps a selection criterion to a <see cref="DataTemplate"/> in a <see cref="MatchingTemplateSelector"/>.
  /// </summary>
  public interface IDataTemplateMatcher
  {
    /// <summary>
    /// Tests whether an object matches the criterion for the <see cref="DataTemplate"/>.
    /// </summary>
    /// <param name="item">The object to be tested.</param>
    /// <returns>true if the object matches the criterion; otherwise false.</returns>
    bool Matches(object item);

    /// <summary>
    /// Gets the <see cref="DataTemplate"/> to be used for objects that meet this criterion.
    /// </summary>
    DataTemplate Template { get; }
  }
}
