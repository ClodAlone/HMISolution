using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;

namespace Mindscape.WpfElements
{
  [DebuggerDisplay("{ElementType} {Char}")]
  internal abstract class CharacterDisplayElement : MaskedTextDisplayElement
  {
    private readonly char _char;

    public char Char
    {
      get { return _char; }
    }

    protected CharacterDisplayElement(char ch)
    {
      _char = ch;
    }

    internal override Inline CreateRepresentation(IDisplayElementStyleProvider styleProvider)
    {
      Run run = new Run(Char.ToString());
      run.Style = GetStyle(styleProvider);
      return run;
    }

    protected abstract Style GetStyle(IDisplayElementStyleProvider styleProvider);
  }
}
