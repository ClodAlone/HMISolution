using System;
using System.ComponentModel;
using System.Globalization;

namespace Mindscape.WpfElements
{
  internal class NoMaskMaskedTextStrategy : IMaskedTextStrategy
  {
    private string _text;

    public NoMaskMaskedTextStrategy(CultureInfo culture)
    {
      Culture = culture;
    }

    public string GetText(bool includeLiterals, bool includePrompts)
    {
      return _text ?? String.Empty;
    }

    public bool SetText(string text, out int textPosition, out MaskedTextResultHint hint)
    {
      _text = text;
      textPosition = text.Length;
      hint = MaskedTextResultHint.Success;
      return true;
    }

    public string ToDisplayString()
    {
      return _text;
    }

    public bool VerifyChar(char input, int position, out MaskedTextResultHint hint)
    {
      hint = MaskedTextResultHint.Success;
      return true;
    }

    public string Mask
    {
      get { return String.Empty; }
    }

    public bool IsEditPosition(int position)
    {
      return true;
    }

    public int FindEditPositionFrom(int position, bool direction)
    {
      return position;
    }

    public bool InsertAt(string input, int position, out int textPosition, out MaskedTextResultHint hint)
    {
      _text = _text.Insert(position, input);
      textPosition = position + input.Length;
      hint = MaskedTextResultHint.Success;
      return true;
    }

    public bool RemoveAt(int start, int end, out int textPosition, out MaskedTextResultHint hint)
    {
      _text = _text.Remove(start, end - start + 1);
      textPosition = start;
      hint = MaskedTextResultHint.Success;
      return true;
    }

    public bool Replace(string input, int start, int end, out int textPosition, out MaskedTextResultHint hint)
    {
      string left = _text.Substring(0, start);
      string right = _text.Substring(end);
      _text = left + input + right;
      textPosition = left.Length + input.Length;
      hint = MaskedTextResultHint.Success;
      return true;
    }

    public char PromptChar { get; set; }

    public bool MaskCompleted
    {
      get { return true; }
    }

    public bool MaskFull
    {
      get { return false; }
    }

    public CultureInfo Culture { get; set; }
  }
}
