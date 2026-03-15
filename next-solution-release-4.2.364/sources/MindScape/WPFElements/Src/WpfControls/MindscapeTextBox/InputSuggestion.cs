using System;
using System.Collections.Generic;
using System.Text;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Contains information about an input-suggestion match in an
  /// <see cref="AutoCompleteBox"/>.
  /// </summary>
  public struct InputSuggestion
  {
    private readonly string _input;
    private readonly string _suggestion;

    internal InputSuggestion(string input, string suggestion)
    {
      _input = input;
      _suggestion = suggestion;
    }

    /// <summary>
    /// The user input.
    /// </summary>
    public string Input
    {
      get { return _input; }
    }

    /// <summary>
    /// The matching suggestion.
    /// </summary>
    public string Suggestion
    {
      get { return _suggestion; }
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current <see cref="InputSuggestion"/>.
    /// </summary>
    /// <param name="obj">The object to test for equality.</param>
    /// <returns>true if equal; otherwise false.</returns>
    public override bool Equals(object obj)
    {
      if (obj is InputSuggestion)
      {
        InputSuggestion other = (InputSuggestion)obj;
        return other.Input == Input && other.Suggestion == Suggestion;
      }
      return false;
    }

    /// <summary>
    /// Returns a hash code for the <see cref="InputSuggestion"/>.
    /// </summary>
    /// <returns>A hash code.</returns>
    public override int GetHashCode()
    {
      return (Suggestion == null ? 0 : Suggestion.GetHashCode())
        + 29 * (Input == null ? 0 : Input.GetHashCode());
    }

    /// <summary>
    /// Compares two <see cref="InputSuggestion"/> objects.
    /// </summary>
    /// <param name="left">An InputSuggestion to compare.</param>
    /// <param name="right">An InputSuggestion to compare.</param>
    /// <returns>true if equal; otherwise false.</returns>
    public static bool operator ==(InputSuggestion left, InputSuggestion right)
    {
      return left.Equals(right);
    }

    /// <summary>
    /// Compares two <see cref="InputSuggestion"/> objects.
    /// </summary>
    /// <param name="left">An InputSuggestion to compare.</param>
    /// <param name="right">An InputSuggestion to compare.</param>
    /// <returns>false if equal; otherwise true.</returns>
    public static bool operator !=(InputSuggestion left, InputSuggestion right)
    {
      return !(left == right);
    }

    /// <summary>
    /// Represents the input-suggestion match using the suggestion text.
    /// </summary>
    /// <returns>The suggestion text.</returns>
    public override string ToString()
    {
      return Suggestion;
    }
  }
}
