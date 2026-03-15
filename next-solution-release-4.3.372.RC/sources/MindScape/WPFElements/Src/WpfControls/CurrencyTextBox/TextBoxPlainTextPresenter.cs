using System.Windows.Controls;

namespace Mindscape.WpfElements
{
  internal class TextBoxTextPresenter : IPlainTextPresenter
  {
    private TextBox _textBox;

    public TextBoxTextPresenter(TextBox textBox)
    {
      Invariant.ArgumentNotNull(textBox, "textBox");

      _textBox = textBox;
    }

    public int CaretPosition
    {
      get { return _textBox.CaretIndex; }
      set { _textBox.CaretIndex = value; }
    }

    public int SelectionStart
    {
      get { return _textBox.SelectionStart; }
    }

    public int SelectionLength
    {
      get { return _textBox.SelectionLength; }
    }

    public void Select(int start, int length)
    {
      _textBox.Select(start, length);
    }
  }
}
