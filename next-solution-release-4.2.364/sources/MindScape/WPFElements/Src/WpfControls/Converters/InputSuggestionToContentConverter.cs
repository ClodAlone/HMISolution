using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Converts an <see cref="InputSuggestion"/> to a formatted text block containing the
  /// suggestion with the user input is highlighted in bold.
  /// </summary>
  public class InputSuggestionToContentConverter : IValueConverter
  {
    /// <summary>
    /// Converts an <see cref="InputSuggestion"/> to a formatted text block usable as content.
    /// </summary>
    /// <param name="value">The InputSuggestion value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>A formatted text block containing the
    /// suggestion with the user input is highlighted in bold..</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      TextBlock result = new TextBlock();
      if (value != null)
      {
        InputSuggestion suggestion = (InputSuggestion)value;
        int startIndex = suggestion.Suggestion.IndexOf(suggestion.Input, StringComparison.CurrentCultureIgnoreCase);
        int endIndex = startIndex + suggestion.Input.Length;
        if (startIndex >= 0)
        {
          result.Inlines.Add(new Run(suggestion.Suggestion.Substring(0, startIndex)));
          result.Inlines.Add(new Run(suggestion.Suggestion.Substring(startIndex, suggestion.Input.Length)) { FontWeight = FontWeights.Bold });
          result.Inlines.Add(new Run(suggestion.Suggestion.Substring(endIndex, suggestion.Suggestion.Length - endIndex)));
        }
        else
        {
          result.Text = suggestion.Suggestion;
        }
      }
      return result;
    }

    /// <summary>
    /// Converts a value from a binding target for writing to a binding source.
    /// </summary>
    /// <param name="value">The value produced by the binding target.</param>
    /// <param name="targetType">The type to convert to.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>This conversion direction is not implemented by this converter.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
