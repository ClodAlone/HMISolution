//-------------------------------------------------------------------------------------------------
// <copyright file="GridCommands.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

using Syncfusion.Drawing;
using Syncfusion.Styles;
using Syncfusion.ComponentModel;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// A class that encapsulates a command to be executed at a later point in time.
    /// </summary>
    /// <remarks>
    /// The <see cref="SyncfusionCommand.Execute"/> method is overriden in specialized command classes and
    /// performs the command that is saved in this command.
    /// </remarks>
    public abstract class GridModelCommand : SyncfusionCommand
    {
        private string description;
        private GridModel grid;

        /// <overload>
        /// Initializes a new <see cref="GridModelCommand"/>.
        /// </overload>
        /// <summary>
        /// Initializes a new <see cref="GridModelCommand"/>.
        /// </summary>
        public GridModelCommand()
        {
            this.description = string.Empty;
        }

        /// <summary>
        /// Initializes a new <see cref="GridModelCommand"/> and associates it with a <see cref="GridModel"/>.
        /// </summary>
        /// <param name="grid">The <see cref="GridModel"/> this command is executed on.</param>
        public GridModelCommand(GridModel grid)
        {
            this.grid = grid;
        }

        /// <override/>
        /// <summary>Gets a description for the command.</summary>
        public override string Description
        {
            get
            {
                return this.description;
            }
        }

        /// <summary>
        /// Gets the <see cref="GridModel"/> this command is executed on.
        /// </summary>
        public GridModel Grid
        {
            get
            {
                return this.grid;
            }
        }

        /// <summary>
        /// Changes the description of this command.
        /// </summary>
        /// <param name="value">The new description text.</param>
        protected void SetDescription(string value)
        {
            this.description = value;
        }
    }

    /// <summary>
    /// Holds a collection of <see cref="SyncfusionCommand"/> objects that should all be executed together as one command
    /// when the <see cref="GridModelCommandManager.Undo"/> or <see cref="GridModelCommandManager.Redo"/> of a grid <see cref="GridModel.CommandStack"/>
    /// is called.
    /// </summary>
    /// <remarks>
    /// The <see cref="GridModelCommandManager.BeginTrans"/> of a <see cref="GridModel.CommandStack"/> object will create
    /// a <see cref="GridTransactionCommand"/> object and redirect subsequent commands into the current <see cref="GridTransactionCommand"/> object 
    /// until <see cref="GridModelCommandManager.CommitTrans"/> is called.
    /// </remarks>
    public class GridTransactionCommand : SyncfusionCommand
    {
        private Stack stack = new Stack();
        private string description;
        GridModelCommandManager commandStack;

        /// <overload>
        /// Initializes a new <see cref="GridTransactionCommand"/> object and associates it with a <see cref="GridModelCommandManager"/>.
        /// </overload>
        /// <summary>
        /// Initializes a new <see cref="GridTransactionCommand"/> object and associates it with a <see cref="GridModelCommandManager"/>.
        /// </summary>
        /// <param name="commandStack">The <see cref="GridModelCommandManager"/> this command should be executed on.</param>
        public GridTransactionCommand(GridModelCommandManager commandStack)
            : this(commandStack, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="GridTransactionCommand"/> object and associates it with a <see cref="GridModelCommandManager"/> and sets a description text.
        /// </summary>
        /// <param name="commandStack">The <see cref="GridModelCommandManager"/> this command should be executed on.</param>
        /// <param name="s">The description text for this command.</param>
        public GridTransactionCommand(GridModelCommandManager commandStack, string s)
        {
            this.commandStack = commandStack;
            this.description = s;
        }

        /// <override/>
        /// <summary>Executes the command.</summary>
        public override void Execute()
        {
            bool endUpdate = false;
            if (stack.Count > 1)
            {
                commandStack.Model.BeginUpdate(BeginUpdateOptions.InvalidateAndScroll, "GridTransactionCommand.Execute: " + description);
                endUpdate = true;
            }

            try
            {
                commandStack.BeginTrans(description);
                SyncfusionCommand command;
                while ((command = Pop()) != null)
                {
                    command.Execute();
                }

                commandStack.CommitTrans();
            }
            finally
            {
                if (endUpdate)
                {
                    commandStack.Model.EndUpdate();
                }
            }
        }

        /// <override/>
        /// <summary>Gets a description for the command.</summary>
        public override string Description
        {
            get
            {
                return this.description;
            }
        }

        /// <summary>
        /// Adds a new command to the current transaction.
        /// </summary>
        /// <param name="cmd">The Command.</param>
        public void Push(SyncfusionCommand cmd)
        {
            stack.Push(cmd);
        }

        /// <summary>
        /// Returns a reference to the latest command in the stack and leaves the command on the stack.
        /// </summary>
        /// <returns>A reference to the latest <see cref="SyncfusionCommand"/> command.</returns>
        public SyncfusionCommand Peek()
        {
            return stack.Peek() as SyncfusionCommand;
        }

        /// <summary>
        /// Gets a reference to the stack with all commands that belong to this transaction.
        /// </summary>
        public Stack Stack
        {
            get
            {
                return stack;
            }
        }

        /// <summary>
        /// Returns a reference to the latest command in the stack and removes the command from the stack.
        /// </summary>
        /// <returns>A reference to the latest <see cref="SyncfusionCommand"/> command.</returns>
        public SyncfusionCommand Pop()
        {
            if (stack.Count > 0)
            {
                return stack.Pop() as SyncfusionCommand;
            }

            return null;
        }
    }

    /// <summary>
    /// Holds undo information about a previous <see cref="GridModel.ChangeCells(Syncfusion.Windows.Forms.Grid.GridRangeInfo,Syncfusion.Windows.Forms.Grid.GridStyleInfo[])"/> operation.
    /// </summary>
    /// <remarks>
    /// The <see cref="Execute"/> method will call <see cref="GridModel.ChangeCells(Syncfusion.Windows.Forms.Grid.GridRangeInfo,Syncfusion.Windows.Forms.Grid.GridStyleInfo[])"/> 
    /// with information stored in this object and scroll the affect range into the current grid view.
    /// </remarks>
    public class GridChangeCellsCommand : GridModelCommand
    {
        GridRangeInfo range;
        GridStyleInfo[] cellsInfo;
        StyleModifyType modifyType;

        /// <summary>
        /// Initializes the <see cref="GridChangeCellsCommand"/> with information how to execute
        /// a <see cref="GridModel.ChangeCells(Syncfusion.Windows.Forms.Grid.GridRangeInfo,Syncfusion.Windows.Forms.Grid.GridStyleInfo[])"/> command at a later time.
        /// </summary>
        /// <param name="table">The <see cref="GridModel"/> this command is associated with.</param>
        /// <param name="range">A <see cref="GridRangeInfo"/> that specifies the range of cells.</param>
        /// <param name="cellsInfo">The array of <see cref="GridStyleInfo"/> objects that holds cell information.</param>
        /// <param name="modifyType">A <see cref="StyleModifyType"/> that specifies the style operation to be performed.</param>
        /// <returns>A <see cref="System.Boolean"/> that indicates if the operation was successful.</returns>
        public GridChangeCellsCommand(GridModel table, GridRangeInfo range, GridStyleInfo[] cellsInfo, StyleModifyType modifyType)
            : base(table)
        {
            SetDescription(SR.GetString("CommandChangeCells", range));
            this.range = range;
            this.cellsInfo = cellsInfo;
            this.modifyType = modifyType;
        }

        /// <override/>
        /// <summary>Executes the command.</summary>
        public override void Execute()
        {
            Grid.ChangeCells(range, cellsInfo, modifyType);
            Grid.ScrollCellInView(range, GridScrollCurrentCellReason.Command);
        }
    }

    /// <summary>
    /// Holds undo information about the selection state at a previous operation.
    /// </summary>
    /// <remarks>
    /// A <see cref="GridSelectionStateCommand"/> is created by the <see cref="GridModelCommandManager.CreateSelectionStateCommand"/>
    /// of <see cref="GridModel.CommandStack"/>. 
    /// </remarks>
    public class GridSelectionStateCommand : GridModelCommand
    {
        int currentRow;
        int currentCol;
        GridRangeInfo[] ranges;

        /// <summary>
        /// Initializes a new <see cref="GridSelectionStateCommand"/> with information
        /// about current cell position and selected ranges.
        /// </summary>
        /// <param name="table">The <see cref="GridModel"/> this command is associated with.</param>
        /// <param name="currentRow">The row index of current cell.</param>
        /// <param name="currentCol">The column index of current cell.</param>
        /// <param name="ranges">The currently selected ranges.</param>
        public GridSelectionStateCommand(GridModel table, int currentRow, int currentCol, GridRangeInfo[] ranges)
            : base(table)
        {
            SetDescription(SR.GetString("CommandSelectionState", currentRow, currentCol));
            this.currentRow = currentRow;
            this.currentCol = currentCol;
            this.ranges = ranges;
        }

        /// <override/>
        /// <summary>Executes the command.</summary>
        public override void Execute()
        {
#if DEBUG
            Trace.WriteLineIf(Switches.CommandStack.TraceVerbose, String.Format("ChangeSelectionStateCommand {0},{1},{2}", currentRow, currentCol, ranges.ToString()));
#endif

            Grid.ChangeSelectionState(currentRow, currentCol, ranges);
            Grid.CommandStack.selChanged = ranges != null && ranges.Length > 0;
        }
    }

    /// <summary>
    /// Holds undo information about the current cells value.
    /// </summary>
    /// <remarks>
    /// The <see cref="Execute"/> method will call <see cref="GridCurrentCell.BeginEdit()"/> 
    /// and then set the associated renderer's <see cref="GridCellRendererBase.ControlValue"/>.
    /// </remarks>
    public class GridCurrentCellValueCommand : GridModelCommand
    {
        int currentRow;
        int currentCol;
        object value;

        /// <summary>
        /// Initializes a new <see cref="GridSelectionStateCommand"/> with information
        /// about current cell position and selected ranges.
        /// </summary>
        /// <param name="table">The <see cref="GridModel"/> this command is associated with.</param>
        /// <param name="currentRow">The row index of current cell.</param>
        /// <param name="currentCol">The column index of current cell.</param>
        /// <param name="value">The current cells value.</param>
        public GridCurrentCellValueCommand(GridModel table, int currentRow, int currentCol, object value)
            : base(table)
        {
            SetDescription(SR.GetString("CommandCurrentCellValue", currentRow, currentCol));
            this.currentRow = currentRow;
            this.currentCol = currentCol;
            this.value = value;
        }

        /// <override/>
        /// <summary>
        /// This method will call <see cref="GridCurrentCell.BeginEdit()"/> 
        /// and then set the associated renderer's <see cref="GridCellRendererBase.ControlValue"/>.
        /// </summary>
        public override void Execute()
        {
#if DEBUG
            Trace.WriteLineIf(Switches.CommandStack.TraceVerbose, String.Format("ChangeSelectionStateCommand {0},{1},{2}", currentRow, currentCol, value.ToString()));
#endif

            if (Grid.ActiveGridView != null)
            {
                GridCurrentCell cc = Grid.ActiveGridView.CurrentCell;
                cc.MoveTo(currentRow, currentCol);
                cc.BeginEdit();
                cc.Renderer.ControlValue = value;
            }
        }
    }

    /// <summary>
    /// Holds undo information about a previous <see cref="GridModelCoveredRanges.Add"/> or
    /// <see cref="GridModelCoveredRanges.Remove"/> operation on a <see cref="GridModel.CoveredRanges"/>
    /// property in a <see cref="GridModel"/> instance.
    /// </summary>
    /// <remarks>
    /// The <see cref="Execute"/> method will call <see cref="GridModelCoveredRanges.SetCoveredRange"/>
    /// with information stored in this object and scroll the affect range into the current grid view.
    /// </remarks>
    public class GridModelSetCoveredRangesCommand
        : GridModelCommand
    {
        GridRangeInfoList ranges;
        bool setOrReset;
        private GridModelCoveredRanges cr;

        /// <summary>
        /// Initializes the <see cref="GridModelSetCoveredRangesCommand"/> with information how to execute
        /// a <see cref="GridModelCoveredRanges.SetCoveredRange"/> method at a later time.
        /// </summary>
        /// <param name="cr">A reference to the <see cref="GridModel.CoveredRanges"/> object.</param>
        /// <param name="ranges">The list with ranges that should be changed.</param>
        /// <param name="setOrReset">True if ranges should be made covered; False if covered ranges should be removed.</param>
        public GridModelSetCoveredRangesCommand(GridModelCoveredRanges cr, GridRangeInfoList ranges, bool setOrReset)
            : base(cr.model)
        {
            if (setOrReset)
            {
                SetDescription(SR.GetString("CommandAddCoveredRanges", ranges.ToString()));
            }
            else
            {
                SetDescription(SR.GetString("CommandRemoveCoveredRanges", ranges.ToString()));
            }

            this.cr = cr;
            this.ranges = ranges;
            this.setOrReset = setOrReset;
        }

        /// <override/>
        /// <summary>
        /// Executes the command.
        /// </summary>
        public override void Execute()
        {
            cr.SetCoveredRanges(ranges, setOrReset);
            Grid.ScrollCellInView(ranges.ActiveRange, GridScrollCurrentCellReason.Command);
        }
    }

    /// <summary>
    /// Holds undo information about a previous <see cref="GridModelBanneredRanges.Add"/> or
    /// <see cref="GridModelBanneredRanges.Remove"/> operation on a <see cref="GridModel.BanneredRanges"/>
    /// property in a <see cref="GridModel"/> instance.
    /// </summary>
    /// <remarks>
    /// The <see cref="Execute"/> method will call <see cref="GridModelBanneredRanges.SetBanneredRange"/>
    /// with information stored in this object and scroll the affect range into the current grid view.
    /// </remarks>
    public class GridModelSetBanneredRangesCommand
        : GridModelCommand
    {
        GridRangeInfoList ranges;
        bool setOrReset;
        private GridModelBanneredRanges cr;

        /// <summary>
        /// Initializes the <see cref="GridModelSetBanneredRangesCommand"/> with information how to execute
        /// a <see cref="GridModelBanneredRanges.SetBanneredRange"/> method at a later time.
        /// </summary>
        /// <param name="cr">A reference to the <see cref="GridModel.BanneredRanges"/> object.</param>
        /// <param name="ranges">The list with ranges that should be changed.</param>
        /// <param name="setOrReset">True if ranges should be made bannered; False if bannered ranges should be removed.</param>
        public GridModelSetBanneredRangesCommand(GridModelBanneredRanges cr, GridRangeInfoList ranges, bool setOrReset)
            : base(cr.model)
        {
            if (setOrReset)
            {
                SetDescription(SR.GetString("CommandAddBanneredRanges", ranges.ToString()));
            }
            else
            {
                SetDescription(SR.GetString("CommandRemoveBanneredRanges", ranges.ToString()));
            }

            this.cr = cr;
            this.ranges = ranges;
            this.setOrReset = setOrReset;
        }

        /// <override/>
        /// <summary>
        /// Executes the command.
        /// </summary>
        public override void Execute()
        {
            cr.SetBanneredRanges(ranges, setOrReset);
            Grid.ScrollCellInView(ranges.ActiveRange, GridScrollCurrentCellReason.Command);
        }
    }

    /// <summary>
    /// Holds undo information about a previous <see cref="GridModelRowColOperations.RemoveRange"/> operation.
    /// </summary>
    /// <remarks>
    /// The <see cref="Execute"/> method will call <see cref="GridModelRowColOperations.InsertRange(int,int)"/> 
    /// with information stored in this object and scroll the affect range into the current grid view.
    /// </remarks>
    public class GridModelInsertRangeCommand : GridModelCommand
    {
        private int insertAt;
        private int count;
        private GridModelInsertRangeOptions iro;
        private GridModelRowColOperations rco;

        /// <summary>
        /// Initializes the <see cref="GridModelInsertRangeCommand"/> with information how to execute
        /// a <see cref="GridModelRowColOperations.InsertRange(int,int)"/> command at a later time and associates it with a <see cref="GridModelRowColOperations"/>
        /// instance.
        /// </summary>
        /// <param name="rco">The <see cref="GridModelRowColOperations"/> this command is associated with.</param>
        /// <param name="insertAt">The row or column where cells should be inserted.</param>
        /// <param name="count">The number of rows or columns to insert.</param>
        /// <param name="iro">Holds additional information for <see cref="GridModelRowColOperations.InsertRange(int,int)"/> command such as cell contents, row, and column
        /// sizes, hidden state, and covered cells state.</param>
        public GridModelInsertRangeCommand(GridModelRowColOperations rco, int insertAt, int count, GridModelInsertRangeOptions iro)
            : base(rco.model)
        {
            SetDescription(SR.GetString("CommandInsert" + rco.RowColName, insertAt, count));
            this.rco = rco;
            this.insertAt = insertAt;
            this.count = count;
            this.iro = iro;
        }

        /// <override/>
        /// <summary>
        /// Executes the command.
        /// </summary>
        public override void Execute()
        {
            rco.InsertRange(insertAt, count, iro);
            Grid.ScrollCellInView(rco.CreateRangeFromTo(insertAt, insertAt + count - 1), GridScrollCurrentCellReason.Command);
        }
    }

    /// <summary>
    /// Holds undo information about a previous <see cref="GridModelRowColOperations.MoveRange(int,int)"/> operation.
    /// </summary>
    /// <remarks>
    /// The <see cref="Execute"/> method will call <see cref="GridModelRowColOperations.MoveRange(int,int)"/> 
    /// with information stored in this object and scroll the affect range into the current grid view.
    /// </remarks>
    public class GridModelMoveRangeCommand : GridModelCommand
    {
        private int from;
        private int count;
        private int target;
        private GridModelRowColOperations rco;

        /// <summary>
        /// Initializes the <see cref="GridModelMoveRangeCommand"/> with information how to execute
        /// a <see cref="GridModelRowColOperations.MoveRange(int, int)"/> command at a later time and associates it with a <see cref="GridModelRowColOperations"/>
        /// instance.
        /// </summary>
        /// <param name="rco">The <see cref="GridModelRowColOperations"/> this command is associated with.</param>
        /// <param name="from">The first row or column.</param>
        /// <param name="count">The number of rows or columns to move.</param>
        /// <param name="target">The target row or column.</param>
        public GridModelMoveRangeCommand(GridModelRowColOperations rco, int from, int count, int target)
            : base(rco.model)
        {
            SetDescription(SR.GetString("CommandMove" + rco.RowColName, from, count, target));
            this.rco = rco;
            this.from = from;
            this.count = count;
            this.target = target;
        }

        /// <override/>
        /// <summary>Executes the command.</summary>
        public override void Execute()
        {
            rco.MoveRange(from, count, target);
            Grid.ScrollCellInView(rco.CreateRangeFromTo(target, target + count - 1), GridScrollCurrentCellReason.Command);
        }
    }

    /// <summary>
    /// Holds undo information about a previous <see cref="GridModelRowColOperations.InsertRange(int, int)"/> operation.
    /// </summary>
    /// <remarks>
    /// The <see cref="Execute"/> method will call <see cref="GridModelRowColOperations.RemoveRange"/> 
    /// with information stored in this object and scroll the affect range into the current grid view.
    /// </remarks>
    public class GridModelRemoveRangeCommand : GridModelCommand
    {
        private int from;
        private int to;
        private GridModelRowColOperations rco;

        /// <summary>
        /// Initializes the <see cref="GridModelRemoveRangeCommand"/> with information how to execute
        /// a <see cref="GridModelRowColOperations.RemoveRange"/> command at a later time and associates it with a <see cref="GridModelRowColOperations"/>
        /// instance.
        /// </summary>
        /// <param name="rco">The <see cref="GridModelRowColOperations"/> this command is associated with.</param>
        /// <param name="from">The first row or column.</param>
        /// <param name="last">The last row or column.</param>
        public GridModelRemoveRangeCommand(GridModelRowColOperations rco, int from, int last)
            : base(rco.model)
        {
            SetDescription(SR.GetString("CommandRemove" + rco.RowColName, from, last));
            this.rco = rco;
            this.from = from;
            this.to = last;
        }

        /// <override/>
        /// <summary>Executes the command.</summary>
        public override void Execute()
        {
            rco.RemoveRange(from, to);
            Grid.ScrollCellInView(rco.CreateRangeFromTo(from, from), GridScrollCurrentCellReason.Command);
        }
    }

    [Syncfusion.Documentation.DocumentationExclude()]
    class GridModelSetDefaultSizeCommand : GridModelCommand
    {
        private int/*float*/ defaultSize;
        private GridModelRowColOperations rco;

        // Constructor
        public GridModelSetDefaultSizeCommand(GridModelRowColOperations rco, int/*float*/ defaultSize)
            : base(rco.model)
        {
            SetDescription(SR.GetString("CommandDefaultSize" + rco.RowColName, defaultSize));
            this.rco = rco;
            this.defaultSize = defaultSize;
        }

        /// <override/>
        public override void Execute()
        {
            rco.DefaultSize = defaultSize;
        }
    }

    [Syncfusion.Documentation.DocumentationExclude()]
    class GridModelSetFrozenCountCommand : GridModelCommand
    {
        private int frozenCount;
        private GridModelRowColOperations rco;

        // Constructor
        public GridModelSetFrozenCountCommand(GridModelRowColOperations rco, int frozenCount)
            : base(rco.model)
        {
            SetDescription(SR.GetString("CommandFrozenCount" + rco.RowColName, frozenCount));
            this.rco = rco;
            this.frozenCount = frozenCount;
        }

        /// <override/>
        public override void Execute()
        {
            rco.FrozenCount = frozenCount;
        }
    }

    [Syncfusion.Documentation.DocumentationExclude()]
    class GridModelSetHeaderCountCommand : GridModelCommand
    {
        private int headerCount;
        private GridModelRowColOperations rco;

        // Constructor
        public GridModelSetHeaderCountCommand(GridModelRowColOperations rco, int headerCount)
            : base(rco.model)
        {
            SetDescription(SR.GetString("CommandHeaderCount" + rco.RowColName, headerCount));
            this.rco = rco;
            this.headerCount = headerCount;
        }

        public override void Execute()
        {
            rco.HeaderCount = headerCount;
        }
    }
}
