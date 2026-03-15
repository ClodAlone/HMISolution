using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UFRecipeEditor.UndoRedo
{
    internal class UndoRedoManager
    {
        #region Declarations

        readonly Object lockObject = new Object();
        readonly short MaxActionsToStore;
        readonly Stack<UndoRedoDataObject> UndoStack;
        readonly Stack<UndoRedoDataObject> RedoStack;

        #endregion

        #region Constructors

        public UndoRedoManager()
        {
            MaxActionsToStore = 0;
            UndoStack = new Stack<UndoRedoDataObject>();
            RedoStack = new Stack<UndoRedoDataObject>();
        }

        public UndoRedoManager(short numactions)
        {
            MaxActionsToStore = numactions;
            UndoStack = new Stack<UndoRedoDataObject>();
            RedoStack = new Stack<UndoRedoDataObject>();
        }

        #endregion

        #region Members

        public void AddUndoAction(UndoRedoDataObject action)
        {
            lock (lockObject)
            {
                if (MaxActionsToStore > 0)
                    CheckMaxActionsToStore(UndoStack, MaxActionsToStore);

                UndoStack.Push(action);
            }
        }

        public void AddRedoAction(UndoRedoDataObject action)
        {
            lock (lockObject)
            {
                if (MaxActionsToStore > 0)
                    CheckMaxActionsToStore(RedoStack, MaxActionsToStore);

                RedoStack.Push(action);
            }
        }

        public UndoRedoDataObject Undo()
        {
            lock (lockObject)
            {
                if (UndoStack.Count > 0)
                    return UndoStack.Pop();
            }

            return null;
        }

        public UndoRedoDataObject Redo()
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

        #endregion

        #region Privates Members

        void CheckMaxActionsToStore(Stack<UndoRedoDataObject> listactions, short numactions)
        {
            if (listactions.Count >= MaxActionsToStore)
            {
                var actions = new Stack<UndoRedoDataObject>(numactions);
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
