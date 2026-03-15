#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.Generic;
using System;
using System.Linq;
using System.Windows;
#if WPF
#else
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Media;
#endif

#if WPF
namespace Syncfusion.Windows.Tools.RichTextBoxAdv
#else
namespace Syncfusion.UI.Xaml.RichTextBoxAdv
#endif
{
    internal class History
    {
        #region Fields
        private Stack<HistoryInfo> undoStack;
        private Stack<HistoryInfo> redoStack;
        private SfRichTextBoxAdv ownerControl;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the owner control
        /// </summary>
        internal SfRichTextBoxAdv OwnerControl
        {
            get
            {
                return ownerControl;
            }
        }
        /// <summary>
        /// Gets the undo stack
        /// </summary>
        internal Stack<HistoryInfo> UndoStack
        {
            get
            {
                return undoStack;
            }
        }
        /// <summary>
        /// Gets the Redo stack
        /// </summary>
        internal Stack<HistoryInfo> RedoStack
        {
            get
            {
                return redoStack;
            }
        }
        internal bool IsUndoing = false;
        internal bool IsRedoing = false;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="History"/> class.
        /// </summary>
        /// <param name="richTextBoxAdv">The rich text box adv.</param>
        public History(SfRichTextBoxAdv richTextBoxAdv)
        {
            undoStack = new Stack<HistoryInfo>(500);
            redoStack = new Stack<HistoryInfo>(500);
            ownerControl = richTextBoxAdv;
        }
        #endregion

        #region Implemenations
        /// <summary>
        /// Clears the history
        /// </summary>
        internal void ClearHistory()
        {
            undoStack.Clear();
            redoStack.Clear();
#if !WPF
            UIDispatcher.Execute(() =>
            {
                OwnerControl.UndoCommand.ExecuteChanged();
                OwnerControl.RedoCommand.ExecuteChanged();
            });
#endif
        }
        /// <summary>
        /// Clears the undo stack.
        /// </summary>
        internal void ClearUndoStack()
        {
            undoStack.Clear();
#if !WPF
            OwnerControl.UndoCommand.ExecuteChanged();
#endif
        }
        /// <summary>
        /// Clears the redo stack.
        /// </summary>
        internal void ClearRedoStack()
        {
            redoStack.Clear();
#if !WPF
            OwnerControl.RedoCommand.ExecuteChanged();
#endif
        }
        /// <summary>
        /// Records the changes.
        /// </summary>
        /// <param name="historyInfo">The history info.</param>
        internal void RecordChanges(HistoryInfo historyInfo)
        {
            if (OwnerControl.DisableHistory)
                return;
            if (IsUndoing)
            {
                redoStack.Push(historyInfo);
#if !WPF
                OwnerControl.RedoCommand.ExecuteChanged();
#endif
            }
            else
            {
                if (!IsRedoing)
                {
                    redoStack.Clear();
#if !WPF
                    OwnerControl.RedoCommand.ExecuteChanged();
#endif
                }
                undoStack.Push(historyInfo);
#if !WPF
                OwnerControl.UndoCommand.ExecuteChanged();
#endif
            }
        }
        /// <summary>
        /// Undoes this instance.
        /// </summary>
        internal void Undo()
        {
            if (OwnerControl.IsReadOnlyMode || UndoStack.Count == 0 || OwnerControl.DisableHistory)
                return;
            HistoryInfo historyInfo = UndoStack.Pop();
            IsUndoing = true;
            historyInfo.Revert();
            IsUndoing = false;
            OwnerControl.Viewer.CheckForCursorVisibility(false);
        }
        /// <summary>
        /// Redoes this instance.
        /// </summary>
        internal void Redo()
        {
            if (OwnerControl.IsReadOnlyMode || RedoStack.Count == 0 || OwnerControl.DisableHistory)
                return;
            HistoryInfo historyInfo = RedoStack.Pop();
            IsRedoing = true;
            historyInfo.Revert();
            IsRedoing = false;
            OwnerControl.Viewer.CheckForCursorVisibility(false);
        }
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        internal void Dispose()
        {
            ClearHistory();
            ownerControl = null;
        }
        #endregion
    }
}
