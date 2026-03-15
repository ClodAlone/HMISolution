#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Syncfusion.UI.Xaml.Diagram.Utility;

namespace Syncfusion.UI.Xaml.Diagram.Controller
{
    internal interface IUndoRedoController
    {
        object CompositeID { get; set; }
        void Undo();
        void Redo();
        bool BeginComposite(IUndoable source = null);
        void EndComposite(IUndoable source = null);
    }

    internal class AddDelete
    {
        private ElementType _itemType;
        private object _item;
        private bool _isAdd;

        public bool IsAdd
        {
            get { return _isAdd; }
            set { _isAdd = value; }
        }

        public object Item
        {
            get { return _item; }
            set { _item = value; }
        }

        public ElementType ItemType
        {
            get { return _itemType; }
            set { _itemType = value; }
        }

        public AddDelete(object item, ElementType type, bool isAdd)
        {
            _item = item;
            _itemType = type;
            _isAdd = isAdd;
        }
    }

    internal sealed class UndoRedoController
        : ISharedData, IUndoRedoController
    {
        private SharedData SharedData;

        private readonly Stack<Tuple<IUndoable, object>> UndoStack = new Stack<Tuple<IUndoable, object>>();
        private Stack<Tuple<IUndoable, object>> RedoStack = new Stack<Tuple<IUndoable, object>>();
        private UndoableState _mState;

        public void Init(SharedData shared)
        {
            State = UndoableState.Idle;
            SharedData = shared;
        }

        public void Undo()
        {
            if (CanUndo())
            {
                State = UndoableState.Undoing;
                var undo = UndoStack.Peek();
                if (undo.Item1 == null || undo.Item1.CanUndo(undo.Item2))
                {
                    if (undo.Item1 != null)
                    {
                        undo.Item1.State = UndoableState.Undoing;
                        UndoStack.Pop();
                        Tuple<IUndoable, object> redo = new Tuple<IUndoable, object>(undo.Item1, undo.Item1.Undo(undo.Item2));
                        RedoStack.Push(redo);
                        undo.Item1.State = UndoableState.Idle;
                    }
                    else
                    {
                        UndoStack.Pop();
                        RedoStack.Push(undo);
                    }
                    if (undo.Item2 is UndoableState &&
                        ((UndoableState) undo.Item2) == UndoableState.CompositeLogging)
                    {
                        ChainUndoRedo = !ChainUndoRedo;
                    }

                    if (ChainUndoRedo)
                    {
                        Undo();
                    }
                }
                State = UndoableState.Idle;
                SharedData.Commands.Undo.InvalidateCanExecute();
                SharedData.Commands.Redo.InvalidateCanExecute();
            }
        }

        public void Redo()
        {
            if (CanRedo())
            {
                State = UndoableState.Redoing;
                var redo = RedoStack.Peek();
                if (redo.Item1 == null || redo.Item1.CanRedo(redo.Item2))
                {
                    if (redo.Item1 != null)
                    {
                        redo.Item1.State = UndoableState.Redoing;
                        RedoStack.Pop();
                        Tuple<IUndoable, object> undo = new Tuple<IUndoable, object>(redo.Item1, redo.Item1.Redo(redo.Item2));
                        UndoStack.Push(undo);
                        redo.Item1.State = UndoableState.Idle;
                    }
                    else
                    {
                        RedoStack.Pop();
                        UndoStack.Push(redo);
                    }
                    if (redo.Item2 is UndoableState &&
                        ((UndoableState) redo.Item2) == UndoableState.CompositeLogging)
                    {
                        ChainUndoRedo = !ChainUndoRedo;
                    }

                    if (ChainUndoRedo)
                    {
                        Redo();
                    }
                }
                State = UndoableState.Idle;
                SharedData.Commands.Undo.InvalidateCanExecute();
                SharedData.Commands.Redo.InvalidateCanExecute();
            }
        }

        public UndoableState State
        {
            get { return _mState; }
            set
            {
                _mState = value;
                if (_mState == UndoableState.Idle || _mState == UndoableState.CompositeLogging)
                {
                    CanLogData = true;
                }
                else
                {
                    CanLogData = false;
                }
            }
        }

        public object CompositeID { get; set; }

        public bool ChainUndoRedo { get; set; }

        public bool BeginComposite(IUndoable source = null)
        {
            if (CompositeID == null)
            {
                if (CanLogData)
                {
                    State = UndoableState.CompositeLogging;
                    LogData(source, UndoableState.CompositeLogging);
                    CompositeID = new object();
                    return true;
                }
            }
            return false;
        }

        public void EndComposite(IUndoable source = null)
        {
            if (CompositeID != null)
            {
                CompositeID = null;
                var stakPeek = UndoStack.Peek();
                if (stakPeek.Item1 == source && 
                    stakPeek.Item2 is UndoableState && 
                    (UndoableState)stakPeek.Item2 == UndoableState.CompositeLogging)
                {
                    UndoStack.Pop();
                }
                else
                {
                    LogData(source, UndoableState.CompositeLogging);
                }
                State = UndoableState.Idle;
            }
        }

        public bool CanUndo()
        {
            if (UndoStack.Any())
            {
                if (State == UndoableState.Idle)
                {
                    return true;
                }
                else if (State == UndoableState.Undoing && ChainUndoRedo)
                {
                    return true;
                }
            }
            return false;
        }

        public bool CanRedo()
        {
            if (RedoStack.Any())
            {
                if (State == UndoableState.Idle)
                {
                    return true;
                }
                else if (State == UndoableState.Redoing && ChainUndoRedo)
                {
                    return true;
                }
            }
            return false;
        }

        public bool CanLogData { get; set; }
        //{
        //    if (State == UndoableState.Idle || State == UndoableState.CompositeLogging)
        //    {
        //        return true;
        //    }
        //    return false;
        //}

        public bool LogData(IUndoable source, object data)
        {
            RedoStack.Clear();
            if (CanLogData)
            {
                UndoStack.Push(new Tuple<IUndoable, object>(source, data));
                SharedData.Commands.Undo.InvalidateCanExecute();
                SharedData.Commands.Redo.InvalidateCanExecute();
                return true;
            }
            return false;
        }

        public void Clear()
        {
            UndoStack.Clear();
            RedoStack.Clear();
        }

        public void Dispose()
        {
            this.Clear();
            this.SharedData = null;
        }
    }

    internal interface IUndoable
    {
        UndoableState State { get; set; }

        bool CanUndo(object data);
        bool CanRedo(object data);
        bool CanLogData();
        bool LogData(object data);
        object Undo(object data);
        object Redo(object data);
    }

    internal enum UndoableState
    {   
        Idle,
        Disabled,
        CompositeLogging,
        Undoing,
        Redoing
    }
}
