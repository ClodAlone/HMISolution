using System.ComponentModel;
using System.Globalization;

namespace Mindscape.WpfElements
{
  internal class DefaultMaskedTextStrategy : IMaskedTextStrategy
  {
    private MaskedTextProvider _provider;

    public DefaultMaskedTextStrategy(string mask, CultureInfo culture)
    {
      _provider = new MaskedTextProvider(mask, culture);
    }

    public string GetText(bool includeLiterals, bool includePrompts)
    {
      return _provider.ToString(includePrompts, includeLiterals);
    }

    public bool SetText(string text, out int textPosition, out MaskedTextResultHint hint)
    {
      return _provider.Set(text, out textPosition, out hint);
    }

    public string ToDisplayString()
    {
      return _provider.ToDisplayString();
    }

    public bool VerifyChar(char input, int position, out MaskedTextResultHint hint)
    {
      return _provider.VerifyChar(input, position, out hint);
    }

    public string Mask
    {
      get { return _provider.Mask; }
    }

    public bool IsEditPosition(int position)
    {
      return _provider.IsEditPosition(position);
    }

    public int FindEditPositionFrom(int position, bool direction)
    {
      return _provider.FindEditPositionFrom(position, direction);
    }

    public bool InsertAt(string input, int position, out int textPosition, out MaskedTextResultHint hint)
    {
      return _provider.InsertAt(input, position, out textPosition, out hint);
    }

    public bool RemoveAt(int start, int end, out int textPosition, out MaskedTextResultHint hint)
    {
      return _provider.RemoveAt(start, end, out textPosition, out hint);
    }

    public bool Replace(string input, int start, int end, out int textPosition, out MaskedTextResultHint hint)
    {
      return _provider.Replace(input, start, end, out textPosition, out hint);
    }

    public char PromptChar
    {
      get
      {
        return _provider.PromptChar;
      }
      set
      {
        _provider.PromptChar = value;
      }
    }

    public bool MaskCompleted
    {
      get { return _provider.MaskCompleted; }
    }

    public bool MaskFull
    {
      get { return _provider.MaskFull; }
    }

    public CultureInfo Culture
    {
      get { return _provider.Culture; }
    }
  }
}
