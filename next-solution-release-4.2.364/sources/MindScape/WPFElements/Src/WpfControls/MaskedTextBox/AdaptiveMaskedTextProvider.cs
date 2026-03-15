using System;
using System.ComponentModel;
using System.Globalization;

namespace Mindscape.WpfElements
{
  internal class AdaptiveMaskedTextProvider : IProvideEditPositions
  {
    private IMaskedTextStrategy _strategy = new NoMaskMaskedTextStrategy(CultureInfo.CurrentCulture);

    public string GetText(bool includeLiterals, bool includePrompts)
    {
      return _strategy.GetText(includeLiterals, includePrompts);
    }

    public bool SetText(string text, out int textPosition, out MaskedTextResultHint hint)
    {
      return _strategy.SetText(text, out textPosition, out hint);
    }

    public string ToDisplayString()
    {
      return _strategy.ToDisplayString();
    }

    public string Mask
    {
      get
      {
        return _strategy.Mask;
      }
      set
      {
        RecreateStrategy(value, Culture);
      }
    }

    private void RecreateStrategy(string newMask, CultureInfo newCulture)
    {
      string text = GetText(true, false);
      char promptChar = PromptChar;

      if (String.IsNullOrEmpty(newMask))
      {
        _strategy = new NoMaskMaskedTextStrategy(newCulture);
      }
      else
      {
        _strategy = new DefaultMaskedTextStrategy(newMask, newCulture);
      }

      if (text != null)
      {
        int dummyPosition;
        MaskedTextResultHint dummyHint;
        SetText(text, out dummyPosition, out dummyHint);
      }
      if (promptChar != '\0')
      {
        PromptChar = promptChar;
      }
    }

    public CultureInfo Culture
    {
      get { return _strategy.Culture; }

      set
      {
        RecreateStrategy(Mask, value);
      }
    }

    public bool VerifyChar(char input, int position, out MaskedTextResultHint hint)
    {
      return _strategy.VerifyChar(input, position, out hint);
    }

    public bool IsEditPosition(int position)
    {
      return _strategy.IsEditPosition(position);
    }

    public int FindEditPositionFrom(int position, bool direction)
    {
      return _strategy.FindEditPositionFrom(position, direction);
    }

    public bool InsertAt(string input, int position, out int textPosition, out MaskedTextResultHint hint)
    {
      return _strategy.InsertAt(input, position, out textPosition, out hint);
    }

    public bool RemoveAt(int start, int end, out int textPosition, out MaskedTextResultHint hint)
    {
      return _strategy.RemoveAt(start, end, out textPosition, out hint);
    }

    public bool Replace(string input, int start, int end, out int textPosition, out MaskedTextResultHint hint)
    {
      return _strategy.Replace(input, start, end, out textPosition, out hint);
    }

    public char PromptChar
    {
      get
      {
        return _strategy.PromptChar;
      }
      set
      {
        _strategy.PromptChar = value;
      }
    }

    public bool MaskCompleted
    {
      get { return _strategy.MaskCompleted; }
    }

    public bool MaskFull
    {
      get { return _strategy.MaskFull; }
    }
  }
}
