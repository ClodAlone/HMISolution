using System.Windows.Documents;

namespace Mindscape.WpfElements
{
  internal interface ITextPresenter
  {
    TextPointer CaretPosition { get; set; }
    TextRange Selection { get; }
  }
}
