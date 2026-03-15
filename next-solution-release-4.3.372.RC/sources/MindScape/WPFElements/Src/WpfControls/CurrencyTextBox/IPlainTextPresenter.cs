
namespace Mindscape.WpfElements
{
  internal interface IPlainTextPresenter
  {
    int CaretPosition { get; set; }
    int SelectionStart { get; }
    int SelectionLength { get; }
    void Select(int start, int length);
  }
}
