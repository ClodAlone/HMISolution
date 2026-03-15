using System;
using System.Collections.Generic;
using System.Text;

namespace Mindscape.WpfElements
{
  internal class AutoCompleteListSuggestionProvider : IAutoCompleteSuggestionProvider
  {
    private readonly IEnumerable<string> _suggestions;

    public AutoCompleteListSuggestionProvider(IEnumerable<string> suggestions)
    {
      _suggestions = suggestions;
    }

    public IEnumerable<string> GetSuggestions(string input, int maxCount)
    {
      if (maxCount <= 0)
      {
        yield break;
      }

      foreach (string suggestion in _suggestions)
      {
        if (suggestion.StartsWith(input, StringComparison.CurrentCultureIgnoreCase))
        {
          yield return suggestion;
        }
      }
    }
  }
}
