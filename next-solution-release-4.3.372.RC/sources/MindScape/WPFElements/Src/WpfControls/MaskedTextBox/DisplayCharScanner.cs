using System;
using System.Collections.Generic;
using System.Globalization;

namespace Mindscape.WpfElements
{
  internal class DisplayCharScanner
  {
    private int _scanPosition = -1;

    private readonly string _displayText;
    private readonly string _textExcludingLiteralsAndPrompts;
    private readonly string _mask;
    private readonly char _promptChar;
    private readonly CultureInfo _culture;
    private IPromptDefaults _promptDefaults;
    private IProvideEditPositions _editPositions;

    internal DisplayCharScanner(string displayText, string textExcludingLiteralsAndPrompts,
      string mask, char promptChar, CultureInfo culture, IPromptDefaults promptDefaults,
      IProvideEditPositions editPositions)
    {
      Invariant.ArgumentNotNull(displayText, "displayText");
      Invariant.ArgumentNotNull(textExcludingLiteralsAndPrompts, "textExcludingLiteralsAndPrompts");
      Invariant.ArgumentNotNull(mask, "mask");
      Invariant.ArgumentNotNull(culture, "culture");
      Invariant.ArgumentNotNull(promptDefaults, "promptDefaults");
      Invariant.ArgumentNotNull(editPositions, "editPositions");

      _displayText = displayText;
      _textExcludingLiteralsAndPrompts = textExcludingLiteralsAndPrompts;
      _mask = mask;
      _promptChar = promptChar;
      _culture = culture;
      _promptDefaults = promptDefaults;
      _editPositions = editPositions;
    }

    internal IEnumerable<MaskedTextDisplayElement> Scan()
    {
      InitialiseDisplayIndexToMaskIndexMap();

      CharEnumerator withoutIterator = _textExcludingLiteralsAndPrompts.GetEnumerator();
      CharEnumerator withIterator = _displayText.GetEnumerator();

      _scanPosition = -1;

      while (withoutIterator.MoveNext())
      {
        while (withIterator.MoveNext())
        {
          ++_scanPosition;
          if (withIterator.Current == withoutIterator.Current)
          {
            yield return new InputDisplayElement(withIterator.Current);
            break;
          }
          else
          {
            if (withoutIterator.Current == ' ' && withIterator.Current == _promptChar)
            {
              yield return FromLiteralOrPromptChar(withIterator.Current);
              break;
            }
            else
            {
              yield return FromLiteralOrPromptChar(withIterator.Current);
            }
          }
        }
      }

      while (withIterator.MoveNext())
      {
        ++_scanPosition;
        yield return FromLiteralOrPromptChar(withIterator.Current);
      }
    }

    private int[] _displayIndexToMaskIndexMap;

    private void InitialiseDisplayIndexToMaskIndexMap()
    {
      _displayIndexToMaskIndexMap = CalculateDisplayIndexToMaskIndexMap();
    }

    // internal rather than private in order to facilitate testing
    internal int[] CalculateDisplayIndexToMaskIndexMap()
    {
      int[] map = new int[_displayText.Length];

      bool escapeNext = false;
      int currentDisplayIndex = 0;

      for (int maskPosition = 0; maskPosition < _mask.Length; ++maskPosition)
      {
        int width;
        if (escapeNext)
        {
          width = 1;
          escapeNext = false;
        }
        else
        {
          width = GetOutputWidthForMaskCharacter(_mask[maskPosition], out escapeNext);
        }

        for (int i = 0; i < width; ++i)
        {
          map[currentDisplayIndex] = maskPosition;
          ++currentDisplayIndex;
        }
      }

      return map;
    }

    private int GetOutputWidthForMaskCharacter(char ch, out bool escapeNext)
    {
      escapeNext = false;

      switch (ch)
      {
        case '\\':
          escapeNext = true;
          return 0;

        case '<':
        case '>':
        case '|':
          return 0;

        case '$':
          return _culture.NumberFormat.CurrencySymbol.Length;

        // Windows supports different decimal and thousands separators depending
        // on whether you're in a currency or numeric context.  The MaskedTextProvider
        // doesn't distinguish these cases in its mask.  We could try to infer the context
        // from the presence of a currency symbol, but it's a bit of an edge case,
        // and the Windows Forms MaskedTextBox always goes with the numeric separator,
        // never the currency separator, so we'll do the same.

        case '.':
          return _culture.NumberFormat.NumberDecimalSeparator.Length;

        case ',':
          return _culture.NumberFormat.NumberGroupSeparator.Length;

        case ':':
          return _culture.DateTimeFormat.TimeSeparator.Length;

        case '/':
          return _culture.DateTimeFormat.DateSeparator.Length;

        default:
          return 1;
      }
    }

    private MaskedTextDisplayElement FromLiteralOrPromptChar(char ch)
    {
      if (ch == _promptChar)
      {
        // EditPositions doesn't refer to the mask.  It refers to the formatted string.  So
        // it omits zero-width elements.
        if (_editPositions.IsEditPosition(_scanPosition))
        {
          int maskIndex = _displayIndexToMaskIndexMap[_scanPosition];
          char maskCharacter = _mask[maskIndex];
          return GetPromptDisplayChar(maskCharacter);
        }
      }

      return new LiteralDisplayElement(ch);
    }

    private PromptDisplayElement GetPromptDisplayChar(char maskCharacter)
    {
      ExpectedInputType charType = ExpectedInputType.Any;
      bool optional = false;

      switch (maskCharacter)
      {
        case '0': charType = ExpectedInputType.Digit; optional = false; break;
        case '9': charType = ExpectedInputType.Digit; optional = true; break;
        case '#': charType = ExpectedInputType.Digit; optional = true; break;
        case 'L': charType = ExpectedInputType.Letter; optional = false; break;
        case '?': charType = ExpectedInputType.Letter; optional = true; break;
        case '&': charType = ExpectedInputType.Any; optional = false; break;
        case 'C': charType = ExpectedInputType.Any; optional = true; break;
        case 'A': charType = ExpectedInputType.Alphanumeric; optional = false; break;
        case 'a': charType = ExpectedInputType.Alphanumeric; optional = true; break;
        default: charType = ExpectedInputType.Any; optional = false; break;
      }

      return new PromptDisplayElement(charType, optional, _promptDefaults);
    }
  }
}
