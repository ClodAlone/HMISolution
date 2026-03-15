//-------------------------------------------------------------------------------------------------
// <copyright file="GridModelCommandManager.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Security;
using System.Security.Permissions;

using Syncfusion.Collections;
using Syncfusion.ComponentModel;
using Syncfusion.Diagnostics;
using Syncfusion.Drawing;
using Syncfusion.Styles;
using Syncfusion.Windows.Forms;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// This class manages undo and redo commands for a grid.
    /// </summary>
    /// <remarks>
    /// You access this class from a grid with the <see cref="GridModel.CommandStack"/>
    /// property of a <see cref="GridModel"/> instance.
    /// </remarks>
    public sealed class GridModelCommandManager : GridModelBound
    {
        Stack undoStack = null;
        Stack redoStack = null;
        GridCommandMode mode = GridCommandMode.Recording;
        ////bool inUndo = false;
        GridTransactionCommand transCmd = null;
        ////SyncfusionCommand savedSelectionStateCommand = null;
        int nestedTransactionCount = 0;
        bool rolledBack = false;
        ////bool inRollback = false;
        internal bool selChanged = false;
        bool enabled = false;

        /// <summary>
        /// Initializes a <see cref="GridModelCommandManager"/> and associates it 
        /// with a <see cref="GridModel"/>.
        /// </summary>
        /// <param name="model">A reference to the parent <see cref="GridModel"/>.</param>
        public GridModelCommandManager(GridModel model)
            : base(model)
        {
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="BeginTrans"/> has been called.
        /// </summary>
        public bool InTransaction
        {
            get
            {
                return transCmd != null;
            }
        }

        /// <summary>
        /// Gets the <see cref="GridTransactionCommand"/> while the transaction is recorded. If <see cref="BeginTrans"/>
        /// was not called, this will be NULL.
        /// </summary>
        public GridTransactionCommand CurrentTransactionCommand
        {
            get
            {
                return transCmd;
            }
        }

        // Wire this class with OnChanged events. OnChanged events should provide enough information
        // to undo the change.

        /// <summary>
        /// Creates a <see cref="GridSelectionStateCommand"/> with information about the grid's
        /// current selection state.
        /// </summary>
        /// <remarks>
        /// The <see cref="GridModel"/> has an internal dirty flag that this method will reset. Only
        /// when the user moves the current cell or changes the current selection, will the dirty flag be set.
        /// <para/>
        /// If the dirty flag was reset and there were no changes to selection, this method will return NULL.
        /// <para/>
        /// If the dirty flag was True, this method will return the current selection state and reset the dirty flag.
        /// </remarks>
        /// <returns>
        /// A <see cref="GridSelectionStateCommand"/> with information about the grid's
        /// current selection state.
        /// </returns>
        public GridSelectionStateCommand CreateSelectionStateCommand()
        {
            GridSelectionStateCommand selectionStateCommand = null;
            if (!InTransaction && Model.selectionStateChanged)
            {
#if DEBUG
                if (Switches.CommandStack.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo();
                }
#endif
                GridRangeInfo[] ranges = new GridRangeInfo[Model.SelectedRanges.Count];
                if (ranges.Length > 0)
                {
                    Model.SelectedRanges.CopyTo(ranges, 0); 
                }

                selectionStateCommand = new GridSelectionStateCommand(Model, Model.currentRow, Model.currentCol, ranges);
                Model.selectionStateChanged = false;
            }

            return selectionStateCommand;
        }

        private Stack PushStack
        {
            get
            {
                if (transCmd != null)
                {
                    return transCmd.Stack;
                }
                else if (mode == GridCommandMode.Undo)
                {
                    return RedoStack;
                }
                else
                {
                    return UndoStack;
                }
            }
        }

        private Stack PullStack
        {
            get
            {
                Stack stack;
                if (mode == GridCommandMode.Redo)
                {
                    stack = RedoStack;
                }
                else
                {
                    stack = UndoStack;
                }

                return stack;
            }
        }

        /// <overload>
        /// Pushes a command onto the undo stack.
        /// </overload>
        /// <summary>
        /// Pushes a command onto the undo stack.
        /// </summary>
        /// <param name="cmd">The <see cref="SyncfusionCommand"/> with undo information.</param>
        /// <remarks>
        /// When the grid is performing an <see cref="Undo"/>, the command will be pushed onto the redo stack. Otherwise, 
        /// commands are pushed onto the undo stack.
        /// </remarks>
        public void Push(SyncfusionCommand cmd)
        {
            Push(cmd, CreateSelectionStateCommand());
        }

        /// <summary>
        /// Pushes a command together with selection state onto the undo stack.
        /// </summary>
        /// <param name="cmd">The <see cref="SyncfusionCommand"/> with undo information.</param>
        /// <param name="selectionStateCommand">The command object with selection information created by a <see cref="CreateSelectionStateCommand"/> call.</param>
        /// <genoverload/>
        public void Push(SyncfusionCommand cmd, SyncfusionCommand selectionStateCommand)
        {
            PushStack.Push(cmd);
            if (selectionStateCommand != null)
            {
                PushStack.Push(selectionStateCommand);
            }

            if (mode == GridCommandMode.Recording)
            {
                RedoStack.Clear();
            }

            Model.RaiseCommandStackChanged(EventArgs.Empty);
        }

        private SyncfusionCommand Peek()
        {
            Stack stack = PullStack;
            return stack.Peek() as SyncfusionCommand;
        }

        private SyncfusionCommand Pop()
        {
            try
            {
                Stack stack = PullStack;
                if (stack.Count > 0)
                {
                    return stack.Pop() as SyncfusionCommand;
                }

                return null;
            }
            finally
            {
                Model.RaiseCommandStackChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the current undo mode that indicates if the grid is in a regular operation or performing an undo or rollback.
        /// </summary>
        public GridCommandMode Mode
        {
            get { return mode; }
            set { mode = value; }
        }

        /// <summary>
        /// Empties both the undo and redo stack.
        /// </summary>
        public void Clear()
        {
#if DEBUG
            if (Switches.CommandStack.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo();
            }
#endif
            undoStack.Clear();
            redoStack.Clear();
        }

        /// <summary>
        /// Gets or sets a value indicating whether the grid should record undo information or if no undo information should be recorded.
        /// </summary>
        public bool Enabled
        {
            get
            {
                return enabled;
            }

            set
            {
#if DEBUG
                if (Switches.CommandStack.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(value);
                }
#else
                ;
#endif

                enabled = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether a grid operation should generate undo information or if undo is temporarily suspended.
        /// </summary>
        /// <remarks>
        /// You should call this method from your command if you add operations to the grid that
        /// you want to be able to undo.
        /// </remarks>
        /// <para/>
        /// <example>
        /// The following example checks <see cref="ShouldGenerateUndoInfo"/> before it
        /// pushes saved state onto the undo stack:
        /// <code lang="C#">
        /// if (OnDefaultSizeChanging(new GridDefaultSizeChangingEventArgs(value)))
        /// {
        ///     bool success = false;
        ///     int savedValue = DefaultSize;
        ///     try
        ///     {
        ///         defaultSize = value;
        ///         success = true;
        ///         if (model.CommandStack.ShouldGenerateUndoInfo)
        ///             model.CommandStack.Push(new GridModelSetDefaultSizeCommand(this, savedValue));
        ///     }
        ///     finally
        ///     {
        ///         OnDefaultSizeChanged(new GridDefaultSizeChangedEventArgs(savedValue, success));
        ///     }
        /// }
        /// </code>
        /// </example>
        public bool ShouldGenerateUndoInfo
        {
            get
            {
                return enabled && model.ShouldRecordUndo && mode != GridCommandMode.Rollback;
            }
        }

        /// <summary>
        /// Gets a value indicating whether grid is in the default mode that records undo information.
        /// </summary>
        public bool IsRecording
        {
            get
            {
                return enabled && mode == GridCommandMode.Recording;
            }
        }

        /// <summary>
        /// Gets the stack with undo commands.
        /// </summary>
        public Stack UndoStack
        {
            get
            {
                if (undoStack == null)
                {
                    undoStack = new Stack();
                }

                return undoStack;
            }
        }

        /// <summary>
        /// Gets the stack with redo commands.
        /// </summary>
        public Stack RedoStack
        {
            get
            {
                if (redoStack == null)
                {
                    redoStack = new Stack();
                }

                return redoStack;
            }
        }

        /// <summary>
        /// Execute the latest command from the undo stack.
        /// </summary>
        /// <remarks>
        /// The redo stack will be cleared.
        /// <para/>
        /// If a <see cref="GridSelectionStateCommand"/> is found this will also be executed so that
        /// selection state will be restored to the same state as it was when the undo command was
        /// recorded.
        /// </remarks>
        public void Undo()
        {
            bool endUpdate = false;
            try
            {
                GridCurrentCell cc = Model.ActiveGridView != null ? Model.ActiveGridView.CurrentCell : null;
                if (cc != null && cc.IsModified)
                {
                    object val = cc.Renderer.ControlText;
                    cc.RejectChanges();
                    cc.Deactivate(true);
                    cc.Refresh();
                    RedoStack.Push(new GridCurrentCellValueCommand(Model, cc.RowIndex, cc.ColIndex, val));
                    return;
                }

                if (IsRecording && RedoStack.Count == 0)
                {
                    SyncfusionCommand savedSelectionStateCommand = this.CreateSelectionStateCommand();
                    if (savedSelectionStateCommand != null)
                    {
                        Model.BeginUpdate(BeginUpdateOptions.InvalidateAndScroll);
                        endUpdate = true;
                        RedoStack.Push(savedSelectionStateCommand);
                    }
                }

                if (IsRecording && RedoStack.Count > 0 && UndoStack.Count > 0 && UndoStack.Peek() is GridSelectionStateCommand)
                {
                    if (!endUpdate)
                    {
                        Model.BeginUpdate(BeginUpdateOptions.InvalidateAndScroll);
                    }

                    endUpdate = true;
                    Execute(GridCommandMode.Undo);
                }

                Execute(GridCommandMode.Undo);
            }
            finally
            {
                if (endUpdate)
                {
                    Model.EndUpdate();
                }
            }
        }

        /// <summary>
        /// Execute the latest command from the redo stack.
        /// </summary>
        /// <remarks>
        /// If a <see cref="GridSelectionStateCommand"/> is found, this will also be executed so that
        /// selection state will be restored to the same state as it was when the undo / redo command was
        /// recorded.
        /// </remarks>
        public void Redo()
        {
            bool endUpdate = false;
            try
            {
                if (IsRecording && UndoStack.Count > 0)
                {
                    if (RedoStack.Count > 0 && RedoStack.Peek() is GridSelectionStateCommand)
                    {
                        Model.BeginUpdate(BeginUpdateOptions.InvalidateAndScroll);
                        endUpdate = true;
                        while (RedoStack.Count > 0 && RedoStack.Peek() is GridSelectionStateCommand)
                        {
                            Execute(GridCommandMode.Redo);
                        }
                    }
                }

                Execute(GridCommandMode.Redo);
            }
            finally
            {
                if (endUpdate)
                {
                    Model.EndUpdate();
                }
            }
        }

        void Execute(GridCommandMode mode)
        {
            if (InTransaction)
            {
                throw new InvalidOperationException("Can't perform Undo or Redo during a transaction (" + this.transCmd.Description + ")");
            }

            GridCommandMode savedMode = Mode;
            Mode = mode;
            SyncfusionCommand cmd = Pop(); ////stack.Pop() as SyncfusionCommand;
#if DEBUG
            if (Switches.CommandStack.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(mode, cmd);
            }
#else
            ;
#endif

            try
            {
                if (cmd != null)
                {
                    cmd.Execute();
                    ////if ((mode == GridCommandMode.Redo ? cmd : Peek()) is GridSelectionStateCommand)
                    ////    Execute(mode);
                }
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                {
                    throw;
                }
            }

            Mode = savedMode;
        }

        /// <summary>
        /// Starts a transaction that combines several subsequent commands into one transaction.
        /// </summary>
        /// <param name="s">A description for the transaction. This text can appear for example as "Undo" information in a menu 
        /// to give feedback to the user about command on the undo stack.
        /// </param>
        /// <remarks>
        /// When you call <see cref="BeginTrans"/>, a <see cref="GridTransactionCommand"/> is created and the <see cref="GridModelCommandManager"/>
        /// is switched into a special mode where new commands will not be pushed onto the undo stack. Instead all new commands
        /// will be pushed into the current <see cref="GridTransactionCommand"/> instance. 
        /// <para/>
        /// When you call <see cref="CommitTrans"/>, the current <see cref="GridTransactionCommand"/>  command will be pushed
        /// onto the undo stack and the <see cref="GridModelCommandManager"/> will switch back to its default behavior
        /// where new commands are pushed onto the undo stack.
        /// <para/>
        /// When you call <see cref="BeginTrans"/>, an internal counter will increase but no additional <see cref="GridTransactionCommand"/> is
        /// created. Only once you call <see cref="CommitTrans"/> as many times as you have called <see cref="BeginTrans"/> will the
        /// transaction will be considered complete and the current <see cref="GridTransactionCommand"/> command will be pushed
        /// onto the undo stack.
        /// <para/>
        /// That means nested transactions are supported. But when you <see cref="Undo"/> or <see cref="Rollback"/> a transaction,
        /// all nested transaction will be treated as one single transaction.
        /// </remarks> 
        public void BeginTrans(string s)
        {
#if DEBUG
            if (Switches.CommandStack.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(s);
                Trace.WriteLineIf(Switches.CommandStack.TraceVerbose, "BeginTrans(" + s + ")");
            }
#else
            ;
#endif

            if (InTransaction)
            {
                this.nestedTransactionCount++;
            }
            else
            {
                nestedTransactionCount = 0;
                rolledBack = false;
                Model.selectionStateChanged = true;
                SyncfusionCommand savedSelectionStateCommand = this.CreateSelectionStateCommand();
                GridTransactionCommand cmd = new GridTransactionCommand(this, s);
                this.transCmd = cmd;
                if (savedSelectionStateCommand != null)
                {
                    Push(savedSelectionStateCommand);
                }
            }
        }

        /// <summary>
        /// Ends a transaction that was started with a previous <see cref="BeginTrans"/> call.
        /// </summary>
        /// <remarks>
        /// See <see cref="BeginTrans"/> for discussion about transaction in a grid.
        /// </remarks>
        public void CommitTrans()
        {
#if DEBUG
            if (Switches.CommandStack.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(transCmd.Description, "nestedTransactionCount: " + nestedTransactionCount.ToString());
            }
#else
            ;
#endif

            if (InTransaction)
            {
                if (nestedTransactionCount > 0)
                {
                    nestedTransactionCount--;
                }
                else
                {
                    SyncfusionCommand cmd = transCmd;
                    transCmd = null;
                    Push(cmd);
                }
            }
            else if (!rolledBack)
            {
                throw new InvalidOperationException("CommitTrans without BeginTrans");
            }
        }

        /// <summary>
        /// Rolls back a transaction in progress that was started with a previous <see cref="BeginTrans"/> call.
        /// </summary>
        /// <remarks>
        /// All commands since a <see cref="BeginTrans"/> call will be undone.
        /// </remarks>
        public void Rollback()
        {
#if DEBUG
            if (Switches.CommandStack.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(transCmd.Description, "nestedTransactionCount: " + nestedTransactionCount.ToString());
            }
#else
            ;
#endif

            if (InTransaction)
            {
                GridTransactionCommand cmd = transCmd;
                rolledBack = true;
                nestedTransactionCount = 0;
                transCmd = null;
                cmd.Execute();
            }
            else if (!rolledBack)
            {
                throw new InvalidOperationException("Rollback without BeginTrans");
            }
        }
    }
}
