using System.Diagnostics;
using System.Windows.Documents;

namespace Mindscape.WpfElements
{
  [DebuggerDisplay("{ElementType} {ExpectedInputType}")]
  internal class PromptDisplayElement : MaskedTextDisplayElement, IPrompt
  {
    private readonly ExpectedInputType _expectedInputType;

    public ExpectedInputType ExpectedInputType
    {
      get { return _expectedInputType; }
    }

    private readonly bool _isOptional;

    public bool IsOptional
    {
      get { return _isOptional; }
    }

    private readonly IPromptDefaults _displayContext;

    public IPromptDefaults DisplayContext
    {
      get { return _displayContext; }
    }

    public PromptDisplayElement(ExpectedInputType expectedCharType, bool optional, IPromptDefaults displayContext)
    {
      _expectedInputType = expectedCharType;
      _isOptional = optional;
      _displayContext = displayContext;
    }

    internal override DisplayElementType ElementType
    {
      get { return DisplayElementType.Prompt; }
    }

    internal override Inline CreateRepresentation(IDisplayElementStyleProvider styleProvider)
    {
      Inline representation;

      IPromptCharDisplaySelector selector = styleProvider.PromptCharDisplaySelector;

      if (selector != null && selector.OverridesRepresentation(this))
      {
        representation = selector.CreateRepresentation(this);
      }
      else
      {
        representation = new Run(_displayContext.PromptChar.ToString());
      }

      representation.Style = styleProvider.PromptStyle;
      return representation;
    }
  }
}
