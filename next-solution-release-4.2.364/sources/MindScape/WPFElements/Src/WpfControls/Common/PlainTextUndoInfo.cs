
namespace Mindscape.WpfElements
{
    internal abstract class PlainTextUndoInfo : TextUndoInfo<IPlainTextPresenter>
    {
      protected PlainTextUndoInfo(string text) : base(text) { }

      internal static PlainTextUndoInfo Create(string text, IPlainTextPresenter textPresenter)
      {
        Invariant.ArgumentNotNull(textPresenter, "textPresenter");

        if (textPresenter.SelectionLength == 0)
        {
          int caretPosition = textPresenter.CaretPosition;
          return new CaretPlainTextUndoInfo(text, caretPosition);
        }
        else
        {
          int start = textPresenter.SelectionStart;
          int length = textPresenter.SelectionLength;
          return new RangePlainTextUndoInfo(text, start, length);
        }
      }
    }

    internal class CaretPlainTextUndoInfo : PlainTextUndoInfo
    {
      private readonly int _caretPosition;

      internal CaretPlainTextUndoInfo(string text, int caretPosition)
        : base(text)
      {
        _caretPosition = caretPosition;
      }

      internal override void SetCursorPosition(IPlainTextPresenter textPresenter)
      {
        textPresenter.CaretPosition = _caretPosition;
      }
    }

    internal class RangePlainTextUndoInfo : PlainTextUndoInfo
    {
      private readonly int _start;
      private readonly int _length;

      internal RangePlainTextUndoInfo(string text, int start, int length)
        : base(text)
      {
        _start = start;
        _length = length;
      }

      internal override void SetCursorPosition(IPlainTextPresenter textPresenter)
      {
        textPresenter.Select(_start, _length);
      }
    }
}
