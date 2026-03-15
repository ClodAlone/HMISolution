using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Documents;

namespace Mindscape.WpfElements
{
  internal class MaskedTextBoxModel : IFilteringTextBoxModel, IPromptDefaults
  {
    private readonly AdaptiveMaskedTextProvider _provider;

    public bool IncludeLiteralsInText { get; set; }
    public bool IncludePromptsInText { get; set; }

    public MaskedTextBoxModel()
    {
      _provider = new AdaptiveMaskedTextProvider();

      IncludeLiteralsInText = true;
      IncludePromptsInText = false;
      Culture = CultureInfo.CurrentCulture;
    }

    public string Text
    {
      get
      {
        return _provider.GetText(IncludeLiteralsInText, IncludePromptsInText);
      }

      set
      {
        int textPosition;
        MaskedTextResultHint hint;

        _provider.SetText(value, out textPosition, out hint);
        _lastOperationResult = hint;
      }
    }

    public string DisplayText
    {
      get { return _provider.ToDisplayString(); }
    }

    public string Mask
    {
      get { return _provider.Mask; }
      set { _provider.Mask = value; }
    }

    public char PromptChar
    {
      get { return _provider.PromptChar; }
      set { _provider.PromptChar = value; }
    }

    public CultureInfo Culture
    {
      get { return _provider.Culture; }
      set { _provider.Culture = value; }
    }

    public bool IsMaskCompleted
    {
      get { return _provider.MaskCompleted; }
    }

    public bool IsMaskFull
    {
      get { return _provider.MaskFull; }
    }

    public bool AutoSkipLiterals { get; set; }

    internal IEnumerable<MaskedTextDisplayElement> GetDisplayChars()
    {
      DisplayCharScanner scanner = CreateDisplayCharScanner();
      return scanner.Scan();
    }

    internal DisplayCharScanner CreateDisplayCharScanner()
    {
      return new DisplayCharScanner(
        GetMaskedText(true), GetMaskedText(false), Mask,
        PromptChar, Culture, this, _provider);
    }

    private MaskedTextResultHint _lastOperationResult;

    public MaskedTextResultHint LastOperationResult
    {
      get { return _lastOperationResult; }
    }

    public bool LastOperationSucceeded
    {
      get { return MaskedTextProvider.GetOperationResultFromHint(_lastOperationResult); }
    }

    public FlowDocument CreateDisplayDocument(IDisplayElementStyleProvider styleProvider)
    {
      Paragraph p = new Paragraph();

      foreach (MaskedTextDisplayElement element in GetDisplayChars())
      {
        Inline elementRepresentation = element.CreateRepresentation(styleProvider);
        TextUtils.ValidateIsSingleTextElement(elementRepresentation);
        p.Inlines.Add(elementRepresentation);
      }

      return new FlowDocument(p);
    }

    public void InsertAtDisplayPos(string input, int position)
    {
      int textPosition;
      MaskedTextResultHint hint;

      for (int i = 0; i < input.Length; ++i)
      {
        if (!_provider.VerifyChar(input[i], position + i, out hint))
        {
          _lastOperationResult = hint;
          return;
        }
      }

      _provider.InsertAt(input, position, out textPosition, out hint);
      _lastOperationResult = hint;
    }

    public void ReplaceAtDisplayPosRange(string input, int startPos, int endPos)
    {
      int textPosition;
      MaskedTextResultHint hint;
      _provider.Replace(input, startPos, endPos, out textPosition, out hint);
      _lastOperationResult = hint;
    }

    public void RemoveAtDisplayPos(int position)
    {
      if (!_provider.IsEditPosition(position))
      {
        _lastOperationResult = MaskedTextResultHint.NonEditPosition;
        return;
      }

      int textPosition;
      MaskedTextResultHint hint;
      _provider.RemoveAt(position, position, out textPosition, out hint);
      _lastOperationResult = hint;
    }

    public void RemoveAtDisplayPosRange(int startPos, int endPos)
    {
      int textPosition;
      MaskedTextResultHint hint;
      _provider.RemoveAt(startPos, endPos, out textPosition, out hint);
      _lastOperationResult = hint;
    }

    public int InsertText(ITextPresenter textPresenter, string text)
    {
      if (textPresenter.Selection.IsEmpty)
      {
        return InsertAtCaret(textPresenter, text);
      }
      else
      {
        return ReplaceSelection(textPresenter, text);
      }
    }

    private int InsertAtCaret(ITextPresenter textPresenter, string text)
    {
      int offset = TextUtils.GetOffsetOfPointer(textPresenter.CaretPosition);

      if (AutoSkipLiterals)
      {
        offset = NextEditPosition(offset, true);
      }

      InsertAtDisplayPos(text, offset);

      return offset + text.Length;
    }

    private int ReplaceSelection(ITextPresenter textPresenter, string text)
    {
      int start = TextUtils.GetOffsetOfPointer(textPresenter.Selection.Start);
      int end = TextUtils.GetOffsetOfPointer(textPresenter.Selection.End) - 1;

      ReplaceAtDisplayPosRange(text, start, end);

      return start + text.Length;
    }

    public int DeleteText(ITextPresenter textPresenter, DeleteDirection deleteDirection)
    {
      int newPos;
      if (textPresenter.Selection.IsEmpty)
      {
        int deltaIfEmpty = (deleteDirection == DeleteDirection.Backward ? -1 : 0);
        bool direction = (deleteDirection == DeleteDirection.Forward);
        int offset = TextUtils.GetOffsetOfPointer(textPresenter.CaretPosition);

        offset = offset + deltaIfEmpty;

        if (AutoSkipLiterals)
        {
          offset = NextEditPosition(offset, direction);
        }

        RemoveAtDisplayPos(offset);
        newPos = offset;
      }
      else
      {
        int start = TextUtils.GetOffsetOfPointer(textPresenter.Selection.Start);
        int end = TextUtils.GetOffsetOfPointer(textPresenter.Selection.End) - 1;
        RemoveAtDisplayPosRange(start, end);
        newPos = start;
      }
      return newPos;
    }

    private string GetMaskedText(bool includeLiteralsAndPrompts)
    {
      return _provider.GetText(includeLiteralsAndPrompts, includeLiteralsAndPrompts);
    }

    private int NextEditPosition(int beginningAt, bool direction)
    {
      return _provider.FindEditPositionFrom(beginningAt, direction);
    }
  }
}
