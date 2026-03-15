using System.Windows;

namespace Mindscape.WpfElements
{
  internal class InputDisplayElement : CharacterDisplayElement
  {
    public InputDisplayElement(char ch) : base(ch) { }

    internal override DisplayElementType ElementType
    {
      get { return DisplayElementType.Input; }
    }

    protected override Style GetStyle(IDisplayElementStyleProvider styleProvider)
    {
      return styleProvider.InputStyle;
    }
  }
}
