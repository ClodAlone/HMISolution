using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Documents;

namespace Mindscape.WpfElements.UnitTests
{
  public class RichCaret : ITextPresenter
  {
    private TextPointer _caretPosition;

    public RichCaret(int caretPosition)
    {
      Paragraph p = new Paragraph();
      for (int i = 0; i < caretPosition * 2; ++i)
      {
        p.Inlines.Add(new Run("*"));
      }
      FlowDocument fd = new FlowDocument(p);
      _caretPosition = TextUtils.GetPointerAtOffset(fd.ContentStart, caretPosition);
    }

    public TextPointer CaretPosition
    {
      get { return _caretPosition; }
      set { _caretPosition = value; }
    }

    public TextRange Selection
    {
      get { return new TextRange(_caretPosition, _caretPosition); }
    }
  }

  public class RichRange : ITextPresenter
  {
    private TextRange _selection;

    public RichRange(int start, int end)
    {
      Paragraph p = new Paragraph();
      for (int i = 0; i < end * 2; ++i)
      {
        p.Inlines.Add(new Run("*"));
      }
      FlowDocument fd = new FlowDocument(p);
      TextPointer startPointer = TextUtils.GetPointerAtOffset(fd.ContentStart, start);
      TextPointer endPointer = TextUtils.GetPointerAtOffset(fd.ContentStart, end);
      _selection = new TextRange(startPointer, endPointer);
    }

    public TextPointer CaretPosition
    {
      get { return _selection.Start; }
      set { throw new NotImplementedException(); }
    }

    public TextRange Selection
    {
      get { return _selection; }
    }
  }

  public class PlainCaret : IPlainTextPresenter
  {
    private int _position;
    public PlainCaret(int position)
    {
      _position = position;
    }

    public int CaretPosition
    {
      get
      {
        return _position;
      }
      set
      {
        _position = value;
      }
    }

    public int SelectionStart
    {
      get { return _position; }
    }

    public int SelectionLength
    {
      get { return 0; }
    }

    public void Select(int start, int length)
    {
      throw new NotImplementedException();
    }
  }

  public class PlainRange : IPlainTextPresenter
  {
    private int _start;
    private int _length;

    public PlainRange(int start, int length)
    {
      _start = start;
      _length = length;
    }

    public int CaretPosition
    {
      get
      {
        return _start;
      }
      set
      {
        throw new NotImplementedException();
      }
    }

    public int SelectionStart
    {
      get { return _start; }
    }

    public int SelectionLength
    {
      get { return _length; }
    }

    public void Select(int start, int length)
    {
      _start = start;
      _length = length;
    }
  }
}
