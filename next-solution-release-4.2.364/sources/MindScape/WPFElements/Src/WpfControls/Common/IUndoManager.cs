
namespace Mindscape.WpfElements
{
  internal interface IUndoManager
  {
    bool CanUndo { get; }
    bool Undoing { get; }
    void Undo();
    void Clear();
  }
}
