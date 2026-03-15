using System.Windows.Controls;
using System.Windows.Documents;

namespace Mindscape.WpfElements
{
  internal class RichTextBoxTextPresenter : ITextPresenter
  {
    private readonly RichTextBox _rtb;

    public RichTextBoxTextPresenter(RichTextBox rtb)
    {
      Invariant.ArgumentNotNull(rtb, "richTextBox");

      _rtb = rtb;
    }

    public TextPointer CaretPosition
    {
      get
      {
        return _rtb.CaretPosition;
      }
      set
      {
        _rtb.CaretPosition = value;
      }
    }

    public TextRange Selection
    {
      get { return _rtb.Selection; }
    }
  }
}
