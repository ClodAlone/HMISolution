using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Mindscape.WpfElements.Design
{
  internal partial class ToolboxEntries
  {
    internal static readonly ToolboxEntry AutoCompleteBoxToolboxEntry = new ToolboxEntry(

      typeof(AutoCompleteBox),
      "A text box with an autocompletion drop-down",

      new DependencyProperty[] {
        AutoCompleteBox.IsDropDownOpenProperty,
        AutoCompleteBox.MatchingSuggestionsProperty,
      },

      new PropertyCategory[] {
        new PropertyCategory("Behavior", new DependencyProperty[] {
          AutoCompleteBox.SuggestionsSourceProperty,
          AutoCompleteBox.MaxSuggestionCountProperty,
          AutoCompleteBox.MinimumPrefixLengthProperty,
        }),
      }

      );
  }
}
