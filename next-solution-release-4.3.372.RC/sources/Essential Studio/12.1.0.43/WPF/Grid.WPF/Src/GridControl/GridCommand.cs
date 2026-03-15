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
using Syncfusion.Windows.ComponentModel;
using Syncfusion.Windows.Styles;
using System.Collections;
using System.Diagnostics;
using Syncfusion.Windows.Diagnostics;
using Syncfusion.Windows.Controls.Grid.Resources;
using Syncfusion.Windows.Controls.Cells;

namespace Syncfusion.Windows.Controls.Grid
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
                //commandStack.Model.BeginUpdate(BeginUpdateOptions.InvalidateAndScroll, "GridTransactionCommand.Execute: " + description);
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
                    //commandStack.Model.EndUpdate();
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
    /// Holds undo information about a previous <see cref="GridModel.ChangeCells"/> operation.
    /// </summary>
    /// <remarks>
    /// The <see cref="Execute"/> method will call <see cref="GridModel.ChangeCells"/> 
    /// with information stored in this object and scroll the affect range into the current grid view.
    /// </remarks>
    public class GridChangeCellsCommand : GridModelCommand
    {
        GridRangeInfo range;
        GridStyleInfo[] cellsInfo;
        StyleModifyType modifyType;

        /// <summary>
        /// Initializes the <see cref="GridChangeCellsCommand"/> with information how to execute
        /// a <see cref="GridModel.ChangeCells"/> command at a later time.
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
            Grid.InvalidateCell(range);
#if SILVERLIGHT
            Grid.InvalidateVisual();
#endif
            //Grid.ScrollCellInView(range, GridScrollCurrentCellReason.Command);
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
            Grid.ChangeSelectionState(currentRow, currentCol, ranges);
            Grid.CommandStack.selChanged = ranges != null && ranges.Length > 0;
#if SILVERLIGHT
            Grid.InvalidateVisual();
#endif
        }
    }

    /// <summary>
    /// Holds undo information about the current cells value.
    /// </summary>
    /// <remarks>
    /// The <see cref="Execute"/> method will call <see cref="GridCurrentCell.BeginEdit"/> 
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
        /// This method will call <see cref="GridCurrentCell.BeginEdit"/> 
        /// and then set the associated renderer's <see cref="GridCellRendererBase.ControlValue"/>.
        /// </summary>
        public override void Execute()
        {
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
    public class GridModelSetCoveredRangesCommand : GridModelCommand
    {
        CoveredCellInfo ranges;
        bool setOrReset;

        /// <summary>
        /// Initializes the <see cref="GridModelSetCoveredRangesCommand"/> with information how to execute
        /// a <see cref="GridModelCoveredRanges.SetCoveredRange"/> method at a later time.
        /// </summary>
        /// <param name="cr">A reference to the <see cref="GridModel.CoveredRanges"/> object.</param>
        /// <param name="ranges">The list with ranges that should be changed.</param>
        /// <param name="setOrReset">True if ranges should be made covered; False if covered ranges should be removed.</param>
        public GridModelSetCoveredRangesCommand(GridModel model, CoveredCellInfo ranges, bool setOrReset)
            : base(model)
        {
            if (setOrReset)
            {
                SetDescription(SR.GetString("CommandAddCoveredRanges", ranges.ToString()));
            }
            else
            {
                SetDescription(SR.GetString("CommandRemoveCoveredRanges", ranges.ToString()));
            }

            this.ranges = ranges;
            this.setOrReset = setOrReset;
        }

        /// <override/>
        /// <summary>
        /// Executes the command.
        /// </summary>
        public override void Execute()
        {
            Grid.CoveredCells.SetCoveredRanges(ranges, setOrReset);
            if (Grid.ActiveGridView != null)
                Grid.ActiveGridView.InvalidateCells();
        }
    }

    /// <summary>
    /// Holds undo information about a previous <see cref="GridModelRowColOperations.RemoveRange"/> operation.
    /// </summary>
    /// <remarks>
    /// The <see cref="Execute"/> method will call <see cref="GridModelRowColOperations.InsertRange"/> 
    /// with information stored in this object and scroll the affect range into the current grid view.
    /// </remarks>
    public class GridModelInsertRowsCommand : GridModelCommand
    {
        private int insertAt;
        private int count;

        /// <summary>
        /// Initializes the <see cref="GridModelInsertRowsCommand"/> with information how to execute
        /// a <see cref="GridModelRowColOperations.InsertRange"/> command at a later time and associates it with a <see cref="GridModelRowColOperations"/>
        /// instance.
        /// </summary>
        /// <param name="rco">The <see cref="GridModelRowColOperations"/> this command is associated with.</param>
        /// <param name="insertAt">The row or column where cells should be inserted.</param>
        /// <param name="count">The number of rows or columns to insert.</param>
        /// <param name="iro">Holds additional information for <see cref="GridModelRowColOperations.InsertRange"/> command such as cell contents, row, and column
        /// sizes, hidden state, and covered cells state.</param>
        public GridModelInsertRowsCommand(GridModel model, int insertAt, int count)
            : base(model)
        {
            SetDescription(SR.GetString("CommandInsertRows", count, insertAt));
            this.insertAt = insertAt;
            this.count = count;
        }

        /// <override/>
        /// <summary>
        /// Executes the command.
        /// </summary>
        public override void Execute()
        {
            Grid.InsertRows(insertAt, count);
        }
    }

    /// <summary>
    /// Holds undo information about a previous <see cref="GridModelRowColOperations.RemoveRange"/> operation.
    /// </summary>
    /// <remarks>
    /// The <see cref="Execute"/> method will call <see cref="GridModelRowColOperations.InsertRange"/> 
    /// with information stored in this object and scroll the affect range into the current grid view.
    /// </remarks>
    public class GridModelInsertColumnsCommand : GridModelCommand
    {
        private int insertAt;
        private int count;

        /// <summary>
        /// Initializes the <see cref="GridModelInsertColumnsCommand"/> with information how to execute
        /// a <see cref="GridModelRowColOperations.InsertRange"/> command at a later time and associates it with a <see cref="GridModelRowColOperations"/>
        /// instance.
        /// </summary>
        /// <param name="rco">The <see cref="GridModelRowColOperations"/> this command is associated with.</param>
        /// <param name="insertAt">The row or column where cells should be inserted.</param>
        /// <param name="count">The number of rows or columns to insert.</param>
        /// <param name="iro">Holds additional information for <see cref="GridModelRowColOperations.InsertRange"/> command such as cell contents, row, and column
        /// sizes, hidden state, and covered cells state.</param>
        public GridModelInsertColumnsCommand(GridModel model, int insertAt, int count)
            : base(model)
        {
            SetDescription(SR.GetString("CommandInsertColumns", count, insertAt));
            this.insertAt = insertAt;
            this.count = count;
        }

        /// <override/>
        /// <summary>
        /// Executes the command.
        /// </summary>
        public override void Execute()
        {
            Grid.InsertColumns(insertAt, count);
        }
    }

    /// <summary>
    /// Holds undo information about a previous <see cref="GridModelRowColOperations.InsertRange"/> operation.
    /// </summary>
    /// <remarks>
    /// The <see cref="Execute"/> method will call <see cref="GridModelRowColOperations.RemoveRange"/> 
    /// with information stored in this object and scroll the affect range into the current grid view.
    /// </remarks>
    public class GridModelRemoveRowsCommand : GridModelCommand
    {
        private int from;
        private int to;

        /// <summary>
        /// Initializes the <see cref="GridModelRemoveRowsCommand"/> with information how to execute
        /// a <see cref="GridModelRowColOperations.RemoveRange"/> command at a later time and associates it with a <see cref="GridModelRowColOperations"/>
        /// instance.
        /// </summary>
        /// <param name="rco">The <see cref="GridModelRowColOperations"/> this command is associated with.</param>
        /// <param name="from">The first row or column.</param>
        /// <param name="last">The last row or column.</param>
        public GridModelRemoveRowsCommand(GridModel model, int from, int last)
            : base(model)
        {
            SetDescription(SR.GetString("CommandRemoveRows", from, last));
            this.from = from;
            this.to = last;
        }

        /// <override/>
        /// <summary>Executes the command.</summary>
        public override void Execute()
        {
            Grid.RemoveRows(from, to);
        }
    }

    /// <summary>
    /// Holds undo information about a previous <see cref="GridModelRowColOperations.InsertRange"/> operation.
    /// </summary>
    /// <remarks>
    /// The <see cref="Execute"/> method will call <see cref="GridModelRowColOperations.RemoveRange"/> 
    /// with information stored in this object and scroll the affect range into the current grid view.
    /// </remarks>
    public class GridModelRemoveColumnsCommand : GridModelCommand
    {
        private int from;
        private int to;

        /// <summary>
        /// Initializes the <see cref="GridModelRemoveColumnsCommand"/> with information how to execute
        /// a <see cref="GridModelRowColOperations.RemoveRange"/> command at a later time and associates it with a <see cref="GridModelRowColOperations"/>
        /// instance.
        /// </summary>
        /// <param name="rco">The <see cref="GridModelRowColOperations"/> this command is associated with.</param>
        /// <param name="from">The first row or column.</param>
        /// <param name="last">The last row or column.</param>
        public GridModelRemoveColumnsCommand(GridModel model, int from, int last)
            : base(model)
        {
            SetDescription(SR.GetString("CommandRemoveColumns", from, last));
            this.from = from;
            this.to = last;
        }

        /// <override/>
        /// <summary>Executes the command.</summary>
        public override void Execute()
        {
            Grid.RemoveColumns(from, to);
        }
    }

    /// <summary>
    /// Holds undo information about a previous <see cref="GridModelRowColOperations.MoveRange"/> operation.
    /// </summary>
    /// <remarks>
    /// The <see cref="Execute"/> method will call <see cref="GridModelRowColOperations.MoveRange"/> 
    /// with information stored in this object and scroll the affect range into the current grid view.
    /// </remarks>
    public class GridModelMoveRowsCommand : GridModelCommand
    {
        private int from;
        private int count;
        private int target;

        /// <summary>
        /// Initializes the <see cref="GridModelMoveRowsCommand"/> with information how to execute
        /// a <see cref="GridModelRowColOperations.MoveRange"/> command at a later time and associates it with a <see cref="GridModelRowColOperations"/>
        /// instance.
        /// </summary>
        /// <param name="rco">The <see cref="GridModelRowColOperations"/> this command is associated with.</param>
        /// <param name="from">The first row or column.</param>
        /// <param name="count">The number of rows or columns to move.</param>
        /// <param name="target">The target row or column.</param>
        public GridModelMoveRowsCommand(GridModel model, int from, int count, int target)
            : base(model)
        {
            SetDescription(SR.GetString("CommandMoveRows", from, count, target));
            this.from = from;
            this.count = count;
            this.target = target;
        }

        /// <override/>
        /// <summary>Executes the command.</summary>
        public override void Execute()
        {
            Grid.MoveRows(target, count, from);
        }
    }

    /// <summary>
    /// Holds undo information about a previous <see cref="GridModelRowColOperations.MoveRange"/> operation.
    /// </summary>
    /// <remarks>
    /// The <see cref="Execute"/> method will call <see cref="GridModelRowColOperations.MoveRange"/> 
    /// with information stored in this object and scroll the affect range into the current grid view.
    /// </remarks>
    public class GridModelMoveColumnsCommand : GridModelCommand
    {
        private int from;
        private int count;
        private int target;

        /// <summary>
        /// Initializes the <see cref="GridModelMoveRowsCommand"/> with information how to execute
        /// a <see cref="GridModelRowColOperations.MoveRange"/> command at a later time and associates it with a <see cref="GridModelRowColOperations"/>
        /// instance.
        /// </summary>
        /// <param name="rco">The <see cref="GridModelRowColOperations"/> this command is associated with.</param>
        /// <param name="from">The first row or column.</param>
        /// <param name="count">The number of rows or columns to move.</param>
        /// <param name="target">The target row or column.</param>
        public GridModelMoveColumnsCommand(GridModel model, int from, int count, int target)
            : base(model)
        {
            SetDescription(SR.GetString("CommandMoveColumns", from, count, target));
            this.from = from;
            this.count = count;
            this.target = target;
        }

        /// <override/>
        /// <summary>Executes the command.</summary>
        public override void Execute()
        {
            Grid.MoveColumns(target, count, from);
        }
    }

    public class GridModelSetDefaultRowSizeCommand : GridModelCommand
    {
        private double defaultSize;

        // Constructor
        public GridModelSetDefaultRowSizeCommand(GridModel model, double defaultSize)
            : base(model)
        {
            SetDescription(SR.GetString("CommandDefaultRowSize" , defaultSize));
            this.defaultSize = defaultSize;
        }

        /// <override/>
        public override void Execute()
        {
            Grid.RowHeights.DefaultLineSize = defaultSize;
        }
    }

    public class GridModelSetDefaultColumnSizeCommand : GridModelCommand
    {
        private double defaultSize;

        // Constructor
        public GridModelSetDefaultColumnSizeCommand(GridModel model, double defaultSize)
            : base(model)
        {
            SetDescription(SR.GetString("CommandDefaultColumnSize", defaultSize));
            this.defaultSize = defaultSize;
        }

        /// <override/>
        public override void Execute()
        {
            Grid.ColumnWidths.DefaultLineSize = defaultSize;
        }
    }

    public class GridModelSetFrozenRowsCountCommand : GridModelCommand
    {
        private int frozenCount;

        // Constructor
        public GridModelSetFrozenRowsCountCommand(GridModel model, int frozenCount)
            : base(model)
        {
            SetDescription(SR.GetString("CommandFrozenRowsCount", frozenCount));
            this.frozenCount = frozenCount;
        }

        /// <override/>
        public override void Execute()
        {
            Grid.FrozenRows = frozenCount;
        }
    }

    public class GridModelSetFrozenColumnsCountCommand : GridModelCommand
    {
        private int frozenCount;

        // Constructor
        public GridModelSetFrozenColumnsCountCommand(GridModel model, int frozenCount)
            : base(model)
        {
            SetDescription(SR.GetString("CommandFrozenColumnsCount", frozenCount));
            this.frozenCount = frozenCount;
        }

        /// <override/>
        public override void Execute()
        {
            Grid.FrozenColumns = frozenCount;
        }
    }

    public class GridModelSetHeaderRowsCountCommand : GridModelCommand
    {
        private int headerCount;

        // Constructor
        public GridModelSetHeaderRowsCountCommand(GridModel model, int headerCount)
            : base(model)
        {
            SetDescription(SR.GetString("CommandHeaderRowsCount" , headerCount));
            this.headerCount = headerCount;
        }

        public override void Execute()
        {
            Grid.HeaderRows = headerCount;
            if (Grid.ActiveGridView != null)
                Grid.ActiveGridView.InvalidateCells();
        }
    }

    public class GridModelSetHeaderColumnsCountCommand : GridModelCommand
    {
        private int headerCount;

        // Constructor
        public GridModelSetHeaderColumnsCountCommand(GridModel model, int headerCount)
            : base(model)
        {
            SetDescription(SR.GetString("CommandHeaderColumnsCount", headerCount));
            this.headerCount = headerCount;
        }

        public override void Execute()
        {
            Grid.HeaderColumns = headerCount;
            if (Grid.ActiveGridView != null)
                Grid.ActiveGridView.InvalidateCells();
        }
    }

    public class GridModelSetFooterRowsCountCommand : GridModelCommand
    {
        private int footerCount;

        // Constructor
        public GridModelSetFooterRowsCountCommand(GridModel model, int footerCount)
            : base(model)
        {
            SetDescription(SR.GetString("CommandFooterRowsCount", footerCount));
            this.footerCount = footerCount;
        }

        public override void Execute()
        {
            Grid.FooterRows = footerCount;
            if (Grid.ActiveGridView != null)
                Grid.ActiveGridView.InvalidateCells();
        }
    }

    public class GridModelSetFooterColumnsCountCommand : GridModelCommand
    {
        private int footerCount;

        // Constructor
        public GridModelSetFooterColumnsCountCommand(GridModel model, int footerCount)
            : base(model)
        {
            SetDescription(SR.GetString("CommandFooterColumnsCount", footerCount));
            this.footerCount = footerCount;
        }

        public override void Execute()
        {
            Grid.FooterColumns = footerCount;
            if (Grid.ActiveGridView != null)
                Grid.ActiveGridView.InvalidateCells();
        }
    }

    /// <summary>
    /// This command object holds all information to execute the SetRange 
    /// command. 
    /// </summary>
    /// <remarks>
    /// GridRowColHideDictionary is typically generated by the SetRange command
    /// and pushed onto the grid's command stack. 
    /// </remarks>
    public class GridModelSetRowHideCommand : GridModelCommand
    {
        private int from;
        private int to;
        private bool values;

        /// <summary>
        /// Initializes <see ref="GridModelSetRowColHideCommand"/> object.
        /// </summary>
        /// <param name="rchi">A reference to the <see cref="GridModelHideRowColsIndexer"/> object.</param>
        /// <param name="from">First row or column index in range.</param>
        /// <param name="last">Last row or column index in range.</param>
        /// <param name="values">An array with hidden states to be applied.</param>
        public GridModelSetRowHideCommand(GridModel model, int from, int last, bool values)
            : base(model)
        {
            SetDescription(SR.GetString("CommandHideRow", from, to));
            this.from = from;
            this.to = last;
            this.values = values;
        }

        public override void Execute()
        {
            Grid.RowHeights.SetHidden(from, to, values);
        }
    }

    /// <summary>
    /// This command object holds all information to execute the SetRange 
    /// command. 
    /// </summary>
    /// <remarks>
    /// GridRowColHideDictionary is typically generated by the SetRange command
    /// and pushed onto the grid's command stack. 
    /// </remarks>
    public class GridModelSetColumnHideCommand : GridModelCommand
    {
        private int from;
        private int to;
        private bool values;

        /// <summary>
        /// Initializes <see ref="GridModelSetRowColHideCommand"/> object.
        /// </summary>
        /// <param name="rchi">A reference to the <see cref="GridModelHideRowColsIndexer"/> object.</param>
        /// <param name="from">First row or column index in range.</param>
        /// <param name="last">Last row or column index in range.</param>
        /// <param name="values">An array with hidden states to be applied.</param>
        public GridModelSetColumnHideCommand(GridModel model, int from, int last, bool values)
            : base(model)
        {
            SetDescription(SR.GetString("CommandHideRow", from, to));
            this.from = from;
            this.to = last;
            this.values = values;
        }

        public override void Execute()
        {
            Grid.ColumnWidths.SetHidden(from, to, values);
        }
    }

    /// <summary>
    /// This command object holds all information to execute the SetRange 
    /// command. 
    /// </summary>
    /// <remarks>
    /// GridModelSetRowColSizeCommand is typically generated by the SetRange command
    /// and pushed onto the grid's command stack. 
    /// </remarks>    
    public class GridModelSetRowSizeCommand : GridModelCommand
    {
        private int from;
        private int to;
        private double values;

        // Constructor.

        /// <summary>
        /// Initializes a new GridModelSetRowColSizeCommand object with all the commands.
        /// </summary>
        /// <param name="rcsi">A reference to the target GridModelRowColSizeIndexer.</param>
        /// <param name="from">First row or column index in range.</param>
        /// <param name="last">Last row or column index in range.</param>
        /// <param name="values">The sizes to be applies to the range.</param>
        public GridModelSetRowSizeCommand(GridModel model, int from, int last, double values)
            : base(model)
        {
            SetDescription(SR.GetString("CommandRowHeight", from, to));
            this.from = from;
            this.to = last;
            this.values = values;
        }

        /// <override/>
        public override void Execute()
        {
            Grid.RowHeights.SetRange(from, to, values);
            Grid.InvalidateVisual();
        }
    }

    /// <summary>
    /// This command object holds all information to execute the SetRange 
    /// command. 
    /// </summary>
    /// <remarks>
    /// GridModelSetRowColSizeCommand is typically generated by the SetRange command
    /// and pushed onto the grid's command stack. 
    /// </remarks>    
    public class GridModelSetColumnSizeCommand : GridModelCommand
    {
        private int from;
        private int to;
        private double values;

        // Constructor.

        /// <summary>
        /// Initializes a new GridModelSetRowColSizeCommand object with all the commands.
        /// </summary>
        /// <param name="rcsi">A reference to the target GridModelRowColSizeIndexer.</param>
        /// <param name="from">First row or column index in range.</param>
        /// <param name="last">Last row or column index in range.</param>
        /// <param name="values">The sizes to be applies to the range.</param>
        public GridModelSetColumnSizeCommand(GridModel model, int from, int last, double values)
            : base(model)
        {
            SetDescription(SR.GetString("CommandColumnWidth", from, to));
            this.from = from;
            this.to = last;
            this.values = values;
        }

        /// <override/>
        public override void Execute()
        {
            Grid.ColumnWidths.SetRange(from, to, values);
            Grid.InvalidateVisual();
        }
    }

}
