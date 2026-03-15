using System;
using System.Windows.Documents;

namespace Mindscape.WpfElements
{
  internal class DefaultPromptCharDisplaySelector : IPromptCharDisplaySelector
  {
    public bool OverridesRepresentation(IPrompt displayChar)
    {
      return false;
    }

    public Inline CreateRepresentation(IPrompt displayChar)
    {
      throw new NotImplementedException();
    }
  }
}
