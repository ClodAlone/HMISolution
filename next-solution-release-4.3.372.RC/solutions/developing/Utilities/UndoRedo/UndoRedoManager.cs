using System;
using System.Collections.Generic;

namespace Utilities.UndoRedo
{
    public class UndoRedoManager<T>
    {
        #region Declarations

        readonly Object lockObject = new Object();
        readonly short MaxActionsToStore;
        readonly Stack<UndoRedoDataObject<T>> UndoStack;
        readonly Stack<UndoRedoDataObject<T>> RedoStack;

        #endregion

        #region Constructors

        public UndoRedoManager()
        {
            MaxActionsToStore = 0;
            UndoStack = new Stack<UndoRedoDataObject<T>>();
            RedoStack = new Stack<UndoRedoDataObject<T>>();
        }

        public UndoRedoManager(short numactions)
        {
            MaxActionsToStore = numactions;
            UndoStack = new Stack<UndoRedoDataObject<T>>();
            RedoStack = new Stack<UndoRedoDataObject<T>>();
        }

        #endregion

        #region Members

        public void AddUndoAction(UndoRedoDataObject<T> action)
        {
            lock (lockObject)
            {
                if (MaxActionsToStore > 0)
                    CheckMaxActionsToStore(UndoStack, MaxActionsToStore);

                UndoStack.Push(action);
            }
        }

        public void AddRedoAction(UndoRedoDataObject<T> action)
        {
            lock (lockObject)
            {
                if (MaxActionsToStore > 0)
                    CheckMaxActionsToStore(RedoStack, MaxActionsToStore);

                RedoStack.Push(action);
            }
        }

        public UndoRedoDataObject<T> Undo()
        {
            lock (lockObject)
            {
                if (UndoStack.Count > 0)
                    return UndoStack.Pop();
            }

            return null;
        }

        public UndoRedoDataObject<T> Redo()
        {
            lock (lockObject)
            {
                if (RedoStack.Count > 0)
                    return RedoStack.Pop();
            }

            return null;
        }

        public bool CanUndo()
        {
            lock (lockObject)
            {
                return UndoStack.Count > 0;
            }
        }

        public bool CanRedo()
        {
            lock (lockObject)
            {
                return RedoStack.Count > 0;
            }
        }

        public void CleanUndoActions()
        {
            lock (lockObject)
            {
                UndoStack.Clear();
            }
        }

        public void CleanRedoActions()
        {
            lock (lockObject)
            {
                RedoStack.Clear();
            }
        }

        #endregion

        #region Privates Members

        void CheckMaxActionsToStore(Stack<UndoRedoDataObject<T>> listactions, short numactions)
        {
            if (listactions.Count >= MaxActionsToStore)
            {
                var actions = new Stack<UndoRedoDataObject<T>>(numactions);
                // fill queue with the stack list
                while (listactions.Count > 1)
                    actions.Push(listactions.Pop());
                // remove the latest object
                listactions.Pop();
                // fill stack with the queue list - 1
                while (actions.Count > 0)
                    listactions.Push(actions.Pop());

                actions.Clear();
            }
        }

        #endregion
    }
}
