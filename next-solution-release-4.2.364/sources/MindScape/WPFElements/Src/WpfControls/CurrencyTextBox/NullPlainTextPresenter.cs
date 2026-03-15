
namespace Mindscape.WpfElements
{
  internal class NullPlainTextPresenter : IPlainTextPresenter
  {
    internal static IPlainTextPresenter Instance = new NullPlainTextPresenter();

    public int CaretPosition
    {
      get { return 0; }
      set { /* no op */ }
    }

    public int SelectionStart
    {
      get { return 0; }
    }

    public int SelectionLength
    {
      get { return 0; }
    }

    public void Select(int start, int length)
    {
      /* do nothing */
    }
  }
}
