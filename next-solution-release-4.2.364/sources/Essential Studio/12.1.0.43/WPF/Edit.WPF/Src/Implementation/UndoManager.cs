#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Syncfusion.Windows.Edit
{
#if SyncfusionFramework4_0

    using System.ComponentModel;

    [DesignTimeVisible(false)]
#endif
    internal class UndoManager
    {
        #region Initialization

        public UndoManager(EditControl Control)
        {
            this.ParentControl = Control;
        }

        #endregion Initialization

        #region private and internal variables

        /// <summary>
        /// Private object of type EditAction
        /// </summary>
        private Stack<EditAction> undostack;

        /// <summary>
        /// Private object of type EditAction
        /// </summary>
        private Stack<EditAction> redostack;

        /// <summary>
        /// internal bool variable.
        /// </summary>
        private bool isNewUndoItem = false;

        private bool isRedo = false;

        internal string currentText;

        #endregion private and internal variables

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether IsNewUndoItem is true or false.
        /// </summary>
        internal bool IsNewUndoItem
        {
            get
            {
                return isNewUndoItem;
            }

            set
            {
                isNewUndoItem = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the instance of parent EditControl.
        /// </summary>
        internal EditControl ParentControl
        {
            get;
            set;
        }

        #endregion Properties

        #region Implementation

        /// <summary>
        /// Helper method to add an EditAction object to UndoStack.
        /// </summary>
        /// <remarks>
        /// <para>Add the Action into the Stack.</para>
        /// </remarks>
        /// <param name="action">Gets the Current EditAction</param>
        public void Add(EditAction action)
        {
            if (undostack == null)
            {
                undostack = new Stack<EditAction>();
            }

            if (IsNewUndoItem && redostack != null)
            {
                redostack.Clear();
            }

            IsNewUndoItem = false;

            EditAction groupaction = CheckGroupAction(action);
            if (groupaction != null)
            {
                if (groupaction.CursorIndex + groupaction.Text.Length == action.CursorIndex)
                {
                    groupaction.Text += action.Text;
                    undostack.Push(groupaction);
                }
                else
                {
                    if (groupaction.CursorIndex != action.CursorIndex)
                        undostack.Push(groupaction);
                    undostack.Push(action);
                }
            }
            else
            {
                if (!isRedo)
                    undostack.Push(action);
                else
                    isRedo = false;
            }
        }

        /// <summary>
        /// Pops the last item in the UndoStack and returns an EditAction object
        /// </summary>
        /// <remarks>
        /// This method is used to catch the last action.
        /// </remarks>
        /// <returns>
        /// Returns last Edit action.
        /// </returns>
        public EditAction GetLastAction()
        {
            if (undostack != null)
            {
                if (undostack.Count > 0)
                {
                    return undostack.Pop();
                }
                else
                {
                    return null;
                }
            }

            return null;
        }

        /// <summary>
        /// Pops the last item in the RedoStack and returns an EditAction object
        /// </summary>
        /// <returns>Returns last Redo action.</returns>
        private EditAction GetRedoAction()
        {
            if (redostack != null)
            {
                if (redostack.Count > 0)
                {
                    return redostack.Pop();
                }
                else
                {
                    return null;
                }
            }

            return null;
        }

        /// <summary>
        /// returns the number of actions in the Undo stack
        /// </summary>
        /// <remarks>
        /// This method is get the total no of action count.
        /// </remarks>
        /// <returns>
        /// Returns 0 when undo stack is null
        /// </returns>
        public int ActionCount()
        {
            if (undostack != null)
            {
                return undostack.Count;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Helper method that compares the current action with the previous action and groups it, if the current and previous action are Type
        /// </summary>
        /// <param name="current">Gets the current Edit action.</param>
        /// <returns>Returns action type.</returns>
        internal EditAction CheckGroupAction(EditAction current)
        {
            EditAction action = GetLastAction();

            if (action == null)
            {
                return null;
            }

            if (action.Action == current.Action && current.Action == ActionType.Type)
            {
                if (ParentControl != null)
                {
                    if (!ParentControl.AddUndoManager)
                        return action;
                    else
                    {
                        undostack.Push(action);
                        return null;
                    }
                }
                else
                    return action;
            }
            else
            {
                undostack.Push(action);
                return null;
            }
        }

        /// <summary>
        /// Method that performs Undo operations based on the ActionType. With Control being
        /// the target of the action.
        /// </summary>
        /// <remarks>
        /// The Undo action method used to undo the last action.  This method is based on
        /// the Action type.
        /// </remarks>
        public void UndoAction()
        {
            EditAction previousaction = GetLastAction();
            int cursorindex = 0;
            currentText = previousaction.Text;
            LineItem item = this.ParentControl.Lines[previousaction.LineNumber - 1]; ;

            this.ParentControl.ScrollControl.ClearSelection();
            switch (previousaction.Action)
            {
                case ActionType.Type:
                case ActionType.Tab:
                    cursorindex = this.ParentControl.RemoveText(previousaction);
                    if (previousaction.IsSelected)
                    {
                        cursorindex = this.ParentControl.PasteSelectedText(previousaction);
                    }
                    if (previousaction.Action == ActionType.Tab)
                    {
                        this.BringLineItemToView(previousaction.LineNumber - 1, previousaction.CursorIndex, previousaction);
                    }
                    else
                    {
                        this.BringLineItemToView(previousaction.LineNumber - 1, cursorindex, previousaction);
                    }
                    break;

                case ActionType.Delete:
                    if (previousaction.IsSelected)
                    {
                        cursorindex = this.ParentControl.PasteSelectedText(previousaction);
                        this.BringLineItemToView(previousaction.Pointer.StartLine, previousaction.CursorIndex, previousaction);
                    }
                    else
                    {
                        cursorindex = this.ParentControl.AddText(previousaction);
                        if (previousaction.NewLine)
                        {
                            this.BringLineItemToView(previousaction.LineNumber - 1, this.ParentControl.Lines[previousaction.LineNumber - 1].Text.Length, previousaction);
                        }
                        else
                        {
                            this.BringLineItemToView(previousaction.LineNumber - 1, cursorindex, previousaction);
                        }
                    }
                    break;

                case ActionType.Backspace:
                    if (previousaction.IsSelected)
                    {
                        cursorindex = this.ParentControl.PasteSelectedText(previousaction);
                        this.BringLineItemToView(previousaction.Pointer.StartLine, cursorindex, previousaction);
                    }
                    else
                    {
                        cursorindex = this.ParentControl.AddText(previousaction);
                        if (previousaction.NewLine)
                        {
                            this.BringLineItemToView(previousaction.LineNumber, 0, previousaction);
                        }
                        else
                        {
                            this.BringLineItemToView(previousaction.LineNumber - 1, cursorindex + 1, previousaction);
                        }
                    }

                    break;

                case ActionType.Cut:
                    cursorindex = this.ParentControl.PasteSelectedText(previousaction);
                    this.BringLineItemToView(previousaction.Pointer.StartLine, previousaction.Pointer.StartIndex, previousaction);
                    break;

                case ActionType.Paste:
                    cursorindex = this.ParentControl.ScrollControl.RemoveSelectedText(previousaction.MultilinePointer);
                    if (previousaction.IsSelected)
                    {
                        cursorindex = this.ParentControl.PasteSelectedText(previousaction);
                    }

                    this.BringLineItemToView(previousaction.MultilinePointer.StartLine, previousaction.MultilinePointer.StartIndex, previousaction);
                    break;

                case ActionType.Enter:
                    cursorindex = this.ParentControl.UndoEnter(previousaction);
                    this.BringLineItemToView(previousaction.LineNumber - 1, cursorindex, previousaction);
                    break;

                case ActionType.Replace:
                    item.Text = Regex.Replace(item.Text, previousaction.SelectedText, (match) =>
                        {
                            if (match.Index == previousaction.CursorIndex)
                            {
                                return previousaction.Text;
                            }
                            return match.Value;
                        });
                    // item.Text = InsertionManager.ReplaceText(item.Text, previousaction.Text, previousaction.CursorIndex);
                    this.BringLineItemToView(previousaction.LineNumber - 1, previousaction.CursorIndex + previousaction.Text.Length, previousaction);
                    break;

                case ActionType.ReplaceAll:
                    foreach (KeyValuePair<LineItem, MatchCollection> pairKey in previousaction.ReplacedItems)
                    {
                        item = pairKey.Key as LineItem;
                        MatchCollection matches = pairKey.Value as MatchCollection;
                        foreach (Match match in matches)
                        {
                            string tempTxt = item.Text.Remove(match.Index, previousaction.SelectedText.Length);
                            tempTxt = tempTxt.Insert(match.Index, previousaction.Text);
                            item.Text = tempTxt;
                        }
                    }
                    this.BringLineItemToView(previousaction.LineNumber - 1, previousaction.CursorIndex, previousaction);
                    break;

                case ActionType.IncreaseIndent:
                    {
                        if (!previousaction.IsSelected)
                        {
                            this.ParentControl.DecreaseIndent(this.ParentControl.Lines[previousaction.LineNumber - 1], this.ParentControl.TabSpaces);
                        }
                        else
                        {
                            this.ParentControl.DecreaseIndent(previousaction.Pointer, this.ParentControl.TabSpaces);
                        }
                        this.GetLastAction();
                        this.BringLineItemToView(previousaction.LineNumber - 1, previousaction.CursorIndex, previousaction);
                        break;
                    }
                case ActionType.DecreaseIndent:
                    {
                        if (!previousaction.IsSelected)
                        {
                            this.ParentControl.IncreaseIndent(this.ParentControl.Lines[previousaction.LineNumber - 1], this.ParentControl.TabSpaces);
                        }
                        else
                        {
                            this.ParentControl.IncreaseIndent(previousaction.Pointer, this.ParentControl.TabSpaces);
                        }
                        this.GetLastAction();
                        this.BringLineItemToView(previousaction.LineNumber - 1, previousaction.CursorIndex + this.ParentControl.TabSpaces, previousaction);
                        break;
                    }
                case ActionType.CommentSelection:
                    {
                        if (previousaction.IsSelected)
                        {
                            this.ParentControl.CurrentLanguage.OnUncommentCommandExecute(previousaction.Pointer);
                        }
                        else
                        {
                            this.ParentControl.CurrentLanguage.OnUncommentCommandExecute(this.ParentControl.Lines[previousaction.LineNumber - 1]);
                        }
                        this.GetLastAction();
                        this.BringLineItemToView(previousaction.LineNumber - 1, previousaction.CursorIndex, previousaction);
                        break;
                    }
                case ActionType.UncommentSelection:
                    {
                        if (previousaction.IsSelected)
                        {
                            for (int i = previousaction.Pointer.StartLine; i <= previousaction.Pointer.EndLine; i++)
                            {
                                LineItem lineItem = this.ParentControl.Lines[i];
                                UndoComment undoItem = previousaction.UncommentedLexems[lineItem];
                                this.ParentControl.CurrentLanguage.OnCommentCommandExecute(lineItem, undoItem.CursorIndex, undoItem.CommentLexem);
                            }
                            //this.ParentControl.CurrentLanguage.OnCommentCommandExecute(previousaction.Pointer);
                        }
                        else
                        {
                            this.ParentControl.CurrentLanguage.OnCommentCommandExecute(this.ParentControl.Lines[previousaction.LineNumber - 1]);
                        }
                        this.GetLastAction();
                        this.BringLineItemToView(previousaction.LineNumber - 1, previousaction.CursorIndex, previousaction);
                        break;
                    }
                default:
                    break;
            }

            AddRedo(previousaction);
        }

        /// <summary>
        /// Helper method to update cursor position and selection when undo or redo operations are done.
        /// </summary>
        /// <param name="linenumber"></param>
        /// <param name="cursorindex"></param>
        /// <param name="action"></param>
        private void BringLineItemToView(int linenumber, int cursorindex, EditAction action)
        {
            this.ParentControl.ScrollControl.ScrollRows.ScrollInView(linenumber);
            this.ParentControl.ScrollControl.MoveCursorToLineItem(linenumber);
            if (this.ParentControl.ScrollControl.Caret != null && linenumber < this.ParentControl.Lines.Count)
            {
                this.ParentControl.ScrollControl.CaretIndex = this.ParentControl.ScrollControl.Caret.MoveToLocation(cursorindex);
            }
            else
            {
                var item = this.ParentControl.Lines[linenumber];
                item.SetCursorOnLoad = true;
                item.SetCursorIndex = cursorindex;
            }

            if (action.IsSelectAll)
            {
                this.ParentControl.ScrollControl.ExecuteSelectAll();
            }

            if (action.IsSelected && action.Pointer != null)
            {
                this.ParentControl.ScrollControl.UpdateSelectionPointer(action.Pointer.StartLine, action.Pointer.EndLine, action.Pointer.StartIndex, action.Pointer.EndIndex);
                this.ParentControl.SelectedText = this.ParentControl.ScrollControl.GetSelectedText(this.ParentControl.ScrollControl.TextSelectionPointer);
            }
        }

        /// <summary>
        /// Helper method to update cursor position and selection when undo or redo operations are done.
        /// </summary>
        /// <param name="linenumber"></param>
        /// <param name="cursorindex"></param>
        /// <param name="action"></param>
        /// <param name="allowSelection"></param>
        private void BringLineItemToView(int linenumber, int cursorindex, EditAction action, bool allowSelection)
        {
            this.ParentControl.ScrollControl.ScrollRows.ScrollInView(linenumber);
            this.ParentControl.ScrollControl.MoveCursorToLineItem(linenumber);
            if (this.ParentControl.ScrollControl.Caret != null && linenumber < this.ParentControl.Lines.Count)
            {
                this.ParentControl.ScrollControl.CaretIndex = this.ParentControl.ScrollControl.Caret.MoveToLocation(cursorindex);
            }
            else
            {
                var item = this.ParentControl.Lines[linenumber];
                item.SetCursorOnLoad = true;
                item.SetCursorIndex = cursorindex;
                this.ParentControl.ScrollControl.CaretIndex = cursorindex;
            }

            if (allowSelection)
            {
                if (action.IsSelectAll)
                {
                    this.ParentControl.ScrollControl.ExecuteSelectAll();
                }

                if (action.IsSelected && action.Pointer != null)
                {
                    this.ParentControl.ScrollControl.UpdateSelectionPointer(action.Pointer.StartLine, action.Pointer.EndLine, action.Pointer.StartIndex, action.Pointer.EndIndex);
                    this.ParentControl.SelectedText = this.ParentControl.ScrollControl.GetSelectedText(this.ParentControl.ScrollControl.TextSelectionPointer);
                }
            }
        }

        /// <summary>
        /// Method that performs Redo operations based on the ActionType. With Control being
        /// the target of the action.
        /// </summary>
        /// <remarks>
        /// This method is used for Redo operation.  This Redo operation is based on the
        /// ActionType.
        /// </remarks>
        public void RedoAction()
        {
            EditAction previousaction = GetRedoAction();
            int cursorindex = 0;
            currentText = previousaction.Text;
            isRedo = true;
            LineItem item = this.ParentControl.Lines[previousaction.LineNumber - 1];
            switch (previousaction.Action)
            {
                case ActionType.Type:
                case ActionType.Tab:
                    cursorindex = previousaction.CursorIndex;
                    if (previousaction.IsSelected)
                    {
                        cursorindex = this.ParentControl.ScrollControl.RemoveSelectedText(previousaction.Pointer);
                    }

                    item.Text = InsertionManager.InsertText(item.Text, previousaction.Text, previousaction.CursorIndex);
                    this.BringLineItemToView(previousaction.LineNumber - 1, cursorindex + previousaction.Text.Length, previousaction, false);
                    break;

                case ActionType.Delete:
                    if (previousaction.IsSelected)
                    {
                        cursorindex = this.ParentControl.ScrollControl.RemoveSelectedText(previousaction.Pointer);
                        this.BringLineItemToView(previousaction.Pointer.StartLine, cursorindex, previousaction, false);
                    }
                    else
                    {
                        if (previousaction.NewLine)
                        {
                            item.Text = item.Text + this.ParentControl.Lines[previousaction.LineNumber].Text;
                            this.ParentControl.Lines.RemoveAt(previousaction.LineNumber);
                            this.BringLineItemToView(previousaction.LineNumber - 1, previousaction.CursorIndex, previousaction);
                        }
                        else
                        {
                            cursorindex = this.ParentControl.RemoveText(previousaction);
                            this.BringLineItemToView(previousaction.LineNumber - 1, cursorindex, previousaction);
                        }
                    }
                    break;

                case ActionType.Backspace:
                    if (previousaction.IsSelected)
                    {
                        cursorindex = this.ParentControl.ScrollControl.RemoveSelectedText(previousaction.Pointer);
                        this.BringLineItemToView(previousaction.Pointer.StartLine, cursorindex, previousaction, false);
                    }
                    else
                    {
                        if (previousaction.NewLine)
                        {
                            item.Text = item.Text + this.ParentControl.Lines[previousaction.LineNumber].Text;
                            this.ParentControl.Lines.RemoveAt(previousaction.LineNumber);
                            this.BringLineItemToView(previousaction.LineNumber - 1, previousaction.CursorIndex, previousaction);
                        }
                        else
                        {
                            cursorindex = this.ParentControl.RemoveText(previousaction);
                            this.BringLineItemToView(previousaction.LineNumber - 1, cursorindex + 1, previousaction, false);
                        }
                    }
                    break;

                case ActionType.Cut:
                    cursorindex = this.ParentControl.ScrollControl.RemoveSelectedText(previousaction.Pointer);
                    this.BringLineItemToView(previousaction.Pointer.StartLine, cursorindex, previousaction, false);
                    break;

                case ActionType.Paste:
                    if (previousaction.IsSelected && previousaction.Pointer != null)
                    {
                        this.ParentControl.ScrollControl.RemoveSelectedText(previousaction.Pointer);
                    }
                    var pointer = this.ParentControl.PasteText(previousaction.LineNumber - 1, previousaction.MultilinePointer.StartIndex, previousaction.Text);
                    this.BringLineItemToView(previousaction.MultilinePointer.EndLine, pointer.EndIndex, previousaction, false);
                    break;

                case ActionType.Enter:
                    cursorindex = this.ParentControl.RedoEnter(previousaction);
                    break;

                case ActionType.Replace:
                    //item.Text = InsertionManager.ReplaceText(item.Text, previousaction.SelectedText, previousaction.CursorIndex);
                    item.Text = Regex.Replace(item.Text, previousaction.Text, (match) =>
                       {
                           if (match.Index == previousaction.CursorIndex)
                           {
                               return previousaction.SelectedText;
                           }
                           return match.Value;
                       });
                    this.BringLineItemToView(previousaction.LineNumber - 1, previousaction.CursorIndex + previousaction.SelectedText.Length, previousaction);
                    break;

                case ActionType.ReplaceAll:
                    foreach (KeyValuePair<LineItem, MatchCollection> pairKey in previousaction.ReplacedItems)
                    {
                        item = pairKey.Key as LineItem;
                        MatchCollection matches = pairKey.Value as MatchCollection;
                        foreach (Match match in matches)
                        {
                            string tempTxt = item.Text.Remove(match.Index, previousaction.Text.Length);
                            tempTxt = tempTxt.Insert(match.Index, previousaction.SelectedText);
                            item.Text = tempTxt;
                        }
                    }
                    this.BringLineItemToView(previousaction.LineNumber - 1, previousaction.CursorIndex, previousaction);
                    break;

                case ActionType.CommentSelection:
                    {
                        if (previousaction.IsSelected)
                        {
                            this.ParentControl.CurrentLanguage.OnCommentCommandExecute(previousaction.Pointer);
                        }
                        else
                        {
                            this.ParentControl.CurrentLanguage.OnCommentCommandExecute(this.ParentControl.Lines[previousaction.LineNumber - 1]);
                        }
                        this.GetLastAction();
                        this.BringLineItemToView(previousaction.LineNumber - 1, previousaction.CursorIndex, previousaction, false);
                        break;
                    }
                case ActionType.UncommentSelection:
                    {
                        if (previousaction.IsSelected)
                        {
                            this.ParentControl.CurrentLanguage.OnUncommentCommandExecute(previousaction.Pointer);
                        }
                        else
                        {
                            this.ParentControl.CurrentLanguage.OnUncommentCommandExecute(this.ParentControl.Lines[previousaction.LineNumber - 1]);
                        }
                        this.GetLastAction();
                        this.BringLineItemToView(previousaction.LineNumber - 1, previousaction.CursorIndex, previousaction, false);
                        break;
                    }
                case ActionType.IncreaseIndent:
                    {
                        if (previousaction.IsSelected)
                        {
                            this.ParentControl.IncreaseIndent(previousaction.Pointer, this.ParentControl.TabSpaces);
                        }
                        else
                        {
                            this.ParentControl.IncreaseIndent(this.ParentControl.Lines[previousaction.LineNumber - 1], this.ParentControl.TabSpaces);
                        }
                        this.GetLastAction();
                        this.BringLineItemToView(previousaction.LineNumber - 1, previousaction.CursorIndex, previousaction);
                        break;
                    }
                case ActionType.DecreaseIndent:
                    {
                        if (previousaction.IsSelected)
                        {
                            this.ParentControl.DecreaseIndent(previousaction.Pointer, this.ParentControl.TabSpaces);
                        }
                        else
                        {
                            this.ParentControl.DecreaseIndent(this.ParentControl.Lines[previousaction.LineNumber - 1], this.ParentControl.TabSpaces);
                        }
                        this.GetLastAction();
                        this.BringLineItemToView(previousaction.LineNumber - 1, previousaction.CursorIndex, previousaction);
                        break;
                    }
                default:
                    break;
            }
            if (previousaction.Action == ActionType.Type)
                this.ParentControl.AddUndoManager = true;
            Add(previousaction);
            this.ParentControl.AddUndoManager = false;
        }

        /// <summary>
        /// Helper method to add items to Redo stack object
        /// </summary>
        /// <param name="action">The Edit Action.</param>
        private void AddRedo(EditAction action)
        {
            if (redostack == null)
            {
                redostack = new Stack<EditAction>();
            }

            redostack.Push(action);
        }

        /// <summary>
        /// Returns number of items in the RedoStack.
        /// </summary>
        /// <returns>Returns redo stack count.</returns>
        internal int RedoCount()
        {
            if (redostack == null)
            {
                return 0;
            }
            else
            {
                return redostack.Count;
            }
        }

        /// <summary>
        /// Clears all Actions.
        /// </summary>
        internal void ClearActions()
        {
            if (undostack != null)
            {
                undostack.Clear();
            }

            if (redostack != null)
            {
                redostack.Clear();
            }
        }

        #endregion Implementation
    }

    #region Helper Class

    /// <summary>
    /// EditAction is a Helper class used to store the data regarding the actions performed on the EditControl
    /// </summary>
#if SyncfusionFramework4_0

    [DesignTimeVisible(false)]
#endif
    internal class EditAction
    {
        #region Local variables

        /// <summary>
        /// Private object of type ActionType.
        /// </summary>
        private ActionType action;

        /// <summary>
        /// instance for LineNumber.
        /// </summary>
        private int line;

        /// <summary>
        /// instance for CursorIndex.
        /// </summary>
        private int cursorindex;

        /// <summary>
        /// instance for Text.
        /// </summary>
        private string text;

        /// <summary>
        /// Private bool variable of NewLine.
        /// </summary>
        private bool newline = false;

        /// <summary>
        /// Private object of type SelectionPointer
        /// </summary>
        private SelectionPointer selectionpointer = null;

        /// <summary>
        /// Private object of type SelectionPointer
        /// </summary>
        private SelectionPointer multilinepointer = null;

        /// <summary>
        /// instance for SelectedText.
        /// </summary>
        private string seltext;

        /// <summary>
        /// Private bool variable of IsSelected.
        /// </summary>
        private bool isselected = false;

        /// <summary>
        /// Private bool variable of IsSelectAll.
        /// </summary>
        private bool isselectall = false;

        #endregion Local variables

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the  class.
        /// </summary>
        /// <remarks>
        /// The EditAction Constructor.
        /// </remarks>
        public EditAction()
        {
        }

        #endregion Constructor

        #region Properties

        /// <summary>
        /// Gets or sets the LineNumber
        /// </summary>
        /// <remarks>
        /// Specified the Line number.
        /// </remarks>
        /// <value>
        /// Type: System.Int32
        /// </value>
        public int LineNumber
        {
            get
            {
                return line;
            }

            set
            {
                line = value;
            }
        }

        /// <summary>
        /// Gets or sets the CursorIndex.
        /// </summary>
        /// <remarks>
        /// Specifies the current CursorIndex.
        /// </remarks>
        /// <value>
        /// Type: System.Int32
        /// </value>
        public int CursorIndex
        {
            get
            {
                return cursorindex;
            }

            set
            {
                cursorindex = value;
            }
        }

        /// <summary>
        /// Gets or sets the action type.
        /// </summary>
        /// <remarks>
        /// This Property is used to contain the ActionType for Undo and Redo operations.
        /// </remarks>
        /// <value>
        /// Type: <see
        /// cref="T:Syncfusion.Windows.Edit.ActionType">Syncfusion.Windows.Edit.ActionType</see>
        /// </value>
        public ActionType Action
        {
            get
            {
                return action;
            }

            set
            {
                action = value;
            }
        }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <remarks>
        /// This Property is used to get and set the text from the EditControl.
        /// </remarks>
        /// <value>
        /// Type: System.String
        /// </value>
        public string Text
        {
            get
            {
                return text;
            }

            set
            {
                text = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the NewLine is true or false.
        /// </summary>
        /// <remarks>
        /// This Property specifies the NewLine action.
        /// </remarks>
        /// <value>
        /// <c>true</c> if newline action performed; otherwise, <c>false</c>.
        /// </value>
        public bool NewLine
        {
            get
            {
                return newline;
            }

            set
            {
                newline = value;
            }
        }

        /// <summary>
        /// Gets or sets the selection pointer.
        /// </summary>
        /// <remarks>
        /// This Property specifies the selection pointer values.
        /// </remarks>
        /// <value>
        /// Type: <see
        /// cref="T:Syncfusion.Windows.Edit.SelectionPointer">Syncfusion.Windows.Edit.SelectionPointer</see>
        /// </value>
        public SelectionPointer Pointer
        {
            get
            {
                return selectionpointer;
            }

            set
            {
                selectionpointer = value;
            }
        }

        /// <summary>
        /// Gets or sets the multiline pointer.
        /// </summary>
        /// <remarks>
        /// This Property specifies the multiple selection pointer values.
        /// </remarks>
        /// <value>
        /// Type: <see
        /// cref="T:Syncfusion.Windows.Edit.SelectionPointer">Syncfusion.Windows.Edit.SelectionPointer</see>
        /// </value>
        public SelectionPointer MultilinePointer
        {
            get
            {
                return multilinepointer;
            }

            set
            {
                multilinepointer = value;
            }
        }

        /// <summary>
        /// Gets or sets the Selected Text
        /// </summary>
        /// <remarks>
        /// This Property contains the selected text.
        /// </remarks>
        /// <value>
        /// Type: System.String
        /// </value>
        public string SelectedText
        {
            get
            {
                return seltext;
            }

            set
            {
                seltext = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsSelected is true or false;
        /// </summary>
        /// <remarks>
        /// This property specifies the selection is allowed or not.
        /// </remarks>
        /// <value>
        /// <c>true</c> if this instance is selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsSelected
        {
            get
            {
                return isselected;
            }

            set
            {
                isselected = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsSelectAll is true or false.
        /// </summary>
        /// <remarks>
        /// This Property specifies the Select all is allowed or not.
        /// </remarks>
        /// <value>
        /// <c>true</c> if this instance is select all; otherwise, <c>false</c>.
        /// </value>
        public bool IsSelectAll
        {
            get
            {
                return isselectall;
            }

            set
            {
                isselectall = value;
            }
        }

        public Dictionary<LineItem, MatchCollection> ReplacedItems;

        public Dictionary<LineItem, UndoComment> UncommentedLexems;

        /// <summary>
        /// Gets or sets the Lexem.
        /// </summary>
        /// <remarks>
        /// This Property specifies the Lexem value.
        /// </remarks>
        public ILexem Lexem
        {
            get;
            set;
        }

        #endregion Properties
    }

    internal class UndoComment
    {
        public ILexem CommentLexem { get; set; }

        public int CursorIndex { get; set; }
    }

    #endregion Helper Class
}