using System;
using System.Windows.Documents;
using Mindscape.WpfElements.Properties;

namespace Mindscape.WpfElements
{
  internal static class TextUtils
  {
    internal static TextPointer GetPointerAtOffset(TextPointer from, int characterOffset)
    {
      TextPointer candidate = from;
      TextRange tr;
      while (true)
      {
        tr = new TextRange(from, candidate);
        if (tr.Text.Length == characterOffset)
        {
          return candidate;
        }
        if (tr.Text.Length > characterOffset)
        {
          throw new InvalidOperationException();
        }
        candidate = candidate.GetNextInsertionPosition(LogicalDirection.Forward);
      }
    }

    internal static int GetOffsetOfPointer(TextPointer textPointer)
    {
      TextRange tr = new TextRange(textPointer.DocumentStart, textPointer);
      int offset = tr.Text.Length;
      return offset;
    }

    internal static void MoveCaretToTextOffset(ITextPresenter textPresenter, int offset)
    {
      TextPointer documentStart = textPresenter.CaretPosition.DocumentStart;
      TextPointer position = TextUtils.GetPointerAtOffset(documentStart, offset);
      textPresenter.CaretPosition = position;
    }

    internal static void SetSelectionUsingTextOffset(ITextPresenter textPresenter, int start, int end)
    {
      TextPointer documentStart = textPresenter.CaretPosition.DocumentStart;
      TextPointer selectionStartPosition = TextUtils.GetPointerAtOffset(documentStart, start);
      TextPointer selectionEndPosition = TextUtils.GetPointerAtOffset(documentStart, end);
      textPresenter.Selection.Select(selectionStartPosition, selectionEndPosition);
    }

    internal static void ValidateIsSingleTextElement(Inline inline)
    {
      Run run = inline as Run;
      if (run != null)
      {
        if (run.Text.Length == 1)
        {
          return;
        }
        else
        {
          throw new MaskedTextDisplayException(Resources.PromptsMustBeExactlyOneCharacter);
        }
      }

      InlineUIContainer container = inline as InlineUIContainer;
      if (container != null)
      {
        return;
      }

      Span span = inline as Span;
      if (span != null)
      {
        if (span.Inlines.Count == 1)
        {
          ValidateIsSingleTextElement(span.Inlines.FirstInline);
        }
        else
        {
          throw new MaskedTextDisplayException(Resources.SpanPromptsMustContainExactlyOneInline);
        }
      }

      throw new MaskedTextDisplayException(Resources.UnsupportedPromptRepresentationType);
    }

    internal static bool AreSameUpTo(string first, string second, int length)
    {
      int safeFirstPos = Math.Min(first.Length, length);
      int safeSecondPos = Math.Min(second.Length, length);

      return (safeFirstPos == safeSecondPos) &&
        first.Substring(0, safeFirstPos) == second.Substring(0, safeSecondPos);
    }
  }
}
