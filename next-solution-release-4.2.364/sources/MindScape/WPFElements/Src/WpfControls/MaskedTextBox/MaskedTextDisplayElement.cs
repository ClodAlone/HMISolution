using System.Windows.Documents;

namespace Mindscape.WpfElements
{
  internal abstract class MaskedTextDisplayElement
  {
    internal abstract DisplayElementType ElementType { get; }
    internal abstract Inline CreateRepresentation(IDisplayElementStyleProvider styleProvider);
  }
}
