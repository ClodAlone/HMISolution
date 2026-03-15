
namespace Mindscape.WpfElements
{
  internal abstract class MaskedTextUndoInfo : TextUndoInfo<ITextPresenter>
  {
    protected MaskedTextUndoInfo(string text) : base(text) { }

    internal static MaskedTextUndoInfo Create(string text, ITextPresenter textPresenter)
    {
      Invariant.ArgumentNotNull(textPresenter, "textPresenter");

      if (textPresenter.Selection.IsEmpty)
      {
        int caretPosition = TextUtils.GetOffsetOfPointer(textPresenter.CaretPosition);
        return new CaretMaskedTextUndoInfo(text, caretPosition);
      }
      else
      {
        int start = TextUtils.GetOffsetOfPointer(textPresenter.Selection.Start);
        int end = TextUtils.GetOffsetOfPointer(textPresenter.Selection.End);
        return new RangeMaskedTextUndoInfo(text, start, end);
      }
    }
  }

  internal class CaretMaskedTextUndoInfo : MaskedTextUndoInfo
  {
    private readonly int _caretPosition;

    internal CaretMaskedTextUndoInfo(string text, int caretPosition)
      : base(text)
    {
      _caretPosition = caretPosition;
    }

    internal override void SetCursorPosition(ITextPresenter textPresenter)
    {
      TextUtils.MoveCaretToTextOffset(textPresenter, _caretPosition);
    }
  }

  internal class RangeMaskedTextUndoInfo : MaskedTextUndoInfo
  {
    private readonly int _start;
    private readonly int _end;

    internal RangeMaskedTextUndoInfo(string text, int start, int end)
      : base(text)
    {
      _start = start;
      _end = end;
    }

    internal override void SetCursorPosition(ITextPresenter textPresenter)
    {
      TextUtils.SetSelectionUsingTextOffset(textPresenter, _start, _end);
    }
  }
}
