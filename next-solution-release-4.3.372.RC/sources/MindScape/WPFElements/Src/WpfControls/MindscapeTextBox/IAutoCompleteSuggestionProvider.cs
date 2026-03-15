using System;
using System.Collections.Generic;
using System.Text;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Provides completion suggestions for an <see cref="AutoCompleteBox"/>.
  /// </summary>
  public interface IAutoCompleteSuggestionProvider
  {
    /// <summary>
    /// Gets the suggestions to be displayed for the specified input.
    /// </summary>
    /// <param name="input">The current user input in the <see cref="AutoCompleteBox"/>.</param>
    /// <param name="maxCount">The maximum number of results that the AutoCompleteBox will display.
    /// This parameter is advisory.  Specifically, implementations do not need to track how many items
    /// they have returned: the AutoCompleteBox will not ask for more than this number of items.  The
    /// maximum count is provided so that implementations can filter or prioritise.</param>
    /// <returns>A sequence of suggested matches for the user input.</returns>
    IEnumerable<string> GetSuggestions(string input, int maxCount);
  }
}
