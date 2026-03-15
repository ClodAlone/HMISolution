
namespace Mindscape.WpfElements
{
  internal abstract class TextUndoInfo<TTextPresenter>
  {
    private readonly string _text;

    internal string Text
    {
      get { return _text; }
    } 

    protected TextUndoInfo(string text)
    {
      _text = text;
    }

    internal abstract void SetCursorPosition(TTextPresenter textPresenter);
  }
}
