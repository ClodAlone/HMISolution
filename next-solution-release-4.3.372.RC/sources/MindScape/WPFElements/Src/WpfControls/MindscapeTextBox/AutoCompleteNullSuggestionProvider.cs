using System;
using System.Collections.Generic;
using System.Text;

namespace Mindscape.WpfElements
{
  internal class AutoCompleteNullSuggestionProvider : IAutoCompleteSuggestionProvider
  {
    public IEnumerable<string> GetSuggestions(string input, int maxCount)
    {
      yield break;
    }

    internal static readonly AutoCompleteNullSuggestionProvider Instance = new AutoCompleteNullSuggestionProvider();
  }
}
