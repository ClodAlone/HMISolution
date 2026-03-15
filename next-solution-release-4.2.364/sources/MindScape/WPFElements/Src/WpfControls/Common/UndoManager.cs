using System;
using System.Collections.Generic;
using System.Windows.Threading;

namespace Mindscape.WpfElements
{
  internal class UndoManager<T> : DispatcherObject, IUndoManager
  {
    private Stack<T> _undoStack = new Stack<T>();
    private bool _undoing = false;
    private Action<T> _undoCallback;

    public UndoManager(Action<T> undoCallback)
    {
      Invariant.ArgumentNotNull(undoCallback, "undoCallback");

      _undoCallback = undoCallback;
    }

    public bool CanUndo
    {
      get { VerifyAccess(); return _undoStack.Count > 0; }
    }

    public bool Undoing
    {
      get { VerifyAccess(); return _undoing; }
    }

    public void Undo()
    {
      VerifyAccess();

      try
      {
        _undoing = true;

        if (CanUndo)
        {
          T undoInfo = _undoStack.Pop();
          _undoCallback(undoInfo);
        }
      }
      finally
      {
        _undoing = false;
      }
    }

    internal void PushUndo(T undoInfo)
    {
      VerifyAccess();
      _undoStack.Push(undoInfo);
    }

    public void Clear()
    {
      VerifyAccess();
      _undoStack.Clear();
    }
  }

}
