using System.Windows.Documents;

namespace Mindscape.WpfElements
{
  internal class NullTextPresenter : ITextPresenter
  {
    internal static ITextPresenter Instance = new NullTextPresenter();

    private FlowDocument _impl = new FlowDocument();

    public TextPointer CaretPosition
    {
      get
      {
        return _impl.ContentStart;
      }
      set
      {
        // do nothing
      }
    }

    public TextRange Selection
    {
      get { return new TextRange(_impl.ContentStart, _impl.ContentEnd); }
    }
  }
}
