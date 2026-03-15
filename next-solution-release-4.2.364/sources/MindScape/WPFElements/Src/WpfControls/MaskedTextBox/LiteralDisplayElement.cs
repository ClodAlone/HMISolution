using System.Windows;

namespace Mindscape.WpfElements
{
  internal class LiteralDisplayElement : CharacterDisplayElement
  {
    public LiteralDisplayElement(char ch) : base(ch) { }

    internal override DisplayElementType ElementType
    {
      get { return DisplayElementType.Literal; }
    }

    protected override Style GetStyle(IDisplayElementStyleProvider styleProvider)
    {
      return styleProvider.LiteralStyle;
    }
  }
}
