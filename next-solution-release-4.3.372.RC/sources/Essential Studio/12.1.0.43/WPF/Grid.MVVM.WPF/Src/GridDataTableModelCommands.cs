#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Syncfusion.Windows.ComponentModel;
using Syncfusion.Windows.Controls.Grid;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Controls.Grid.MVVM
{
    #region GridDataTableModelQueryUnboundCellInfo
    // GridDataTableModelQueryUnboundCellInfoCommand<T, TBehavior>
    public class GridDataTableModelQueryUnboundCellInfoCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataTableModelQueryUnboundCellInfoCommandBehavior<T>, new()
    { }

    // GridDataTableModelQueryUnboundCellInfoCommandBehavior<TReturn>
    public class GridDataTableModelQueryUnboundCellInfoCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, GridQueryCellInfoEventArgs>
    {
        public GridDataTableModelQueryUnboundCellInfoCommandBehavior(Func<object, GridQueryCellInfoEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataTableModelQueryUnboundCellInfoCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Model.QueryUnboundCellInfo += OnEventRaised;
        }
    }

    // GridDataTableModelQueryUnboundCellInfoCommand
    public class GridDataTableModelQueryUnboundCellInfoCommand : GridDataControlCommandBase<GridDataTableModelQueryUnboundCellInfoCommandBehavior>
    { }

    // GridDataTableModelQueryUnboundCellInfoCommandBehavior
    public class GridDataTableModelQueryUnboundCellInfoCommandBehavior : GridDataTableModelQueryUnboundCellInfoCommandBehavior<object>
    { }

    // GridDataTableModelQueryUnboundCellInfoCommandWithEventArgs	
    public class GridDataTableModelQueryUnboundCellInfoCommandWithEventArgs : GridDataTableModelQueryUnboundCellInfoCommand<GridQueryCellInfoEventArgs, GridDataTableModelQueryUnboundCellInfoCommandBehaviorWithEventArgs>
    { }

    // GridDataTableModelQueryUnboundCellInfoCommandBehaviorWithEventArgs
    public class GridDataTableModelQueryUnboundCellInfoCommandBehaviorWithEventArgs : GridDataTableModelQueryUnboundCellInfoCommandBehavior<GridQueryCellInfoEventArgs>
    {
        public GridDataTableModelQueryUnboundCellInfoCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataTableModelInitialized
    // GridDataTableModelInitializedCommand<T, TBehavior>
    public class GridDataTableModelInitializedCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataTableModelInitializedCommandBehavior<T>, new()
    { }

    // GridDataTableModelInitializedCommandBehavior<TReturn>
    public class GridDataTableModelInitializedCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, EventArgs>
    {
        public GridDataTableModelInitializedCommandBehavior(Func<object, EventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataTableModelInitializedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Model.Initialized += OnEventRaised;
        }
    }

    // GridDataTableModelInitializedCommand
    public class GridDataTableModelInitializedCommand : GridDataControlCommandBase<GridDataTableModelInitializedCommandBehavior>
    { }

    // GridDataTableModelInitializedCommandBehavior
    public class GridDataTableModelInitializedCommandBehavior : GridDataTableModelInitializedCommandBehavior<object>
    { }

    // GridDataTableModelInitializedCommandWithEventArgs	
    public class GridDataTableModelInitializedCommandWithEventArgs : GridDataTableModelInitializedCommand<EventArgs, GridDataTableModelInitializedCommandBehaviorWithEventArgs>
    { }

    // GridDataTableModelInitializedCommandBehaviorWithEventArgs
    public class GridDataTableModelInitializedCommandBehaviorWithEventArgs : GridDataTableModelInitializedCommandBehavior<EventArgs>
    {
        public GridDataTableModelInitializedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataTableModelFilterChanging
    // GridDataTableModelFilterChangingCommand<T, TBehavior>
    public class GridDataTableModelFilterChangingCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataTableModelFilterChangingCommandBehavior<T>, new()
    { }

    // GridDataTableModelFilterChangingCommandBehavior<TReturn>
    public class GridDataTableModelFilterChangingCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, GridFilterEventArgs>
    {
        public GridDataTableModelFilterChangingCommandBehavior(Func<object, GridFilterEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataTableModelFilterChangingCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Model.FilterChanging += OnEventRaised;
        }
    }

    // GridDataTableModelFilterChangingCommand
    public class GridDataTableModelFilterChangingCommand : GridDataControlCommandBase<GridDataTableModelFilterChangingCommandBehavior>
    { }

    // GridDataTableModelFilterChangingCommandBehavior
    public class GridDataTableModelFilterChangingCommandBehavior : GridDataTableModelFilterChangingCommandBehavior<object>
    { }

    // GridDataTableModelFilterChangingCommandWithEventArgs	
    public class GridDataTableModelFilterChangingCommandWithEventArgs : GridDataTableModelFilterChangingCommand<GridFilterEventArgs, GridDataTableModelFilterChangingCommandBehaviorWithEventArgs>
    { }

    // GridDataTableModelFilterChangingCommandBehaviorWithEventArgs
    public class GridDataTableModelFilterChangingCommandBehaviorWithEventArgs : GridDataTableModelFilterChangingCommandBehavior<GridFilterEventArgs>
    {
        public GridDataTableModelFilterChangingCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataTableModelFilterChanged
    // GridDataTableModelFilterChangedCommand<T, TBehavior>
    public class GridDataTableModelFilterChangedCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataTableModelFilterChangedCommandBehavior<T>, new()
    { }

    // GridDataTableModelFilterChangedCommandBehavior<TReturn>
    public class GridDataTableModelFilterChangedCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, GridFilterEventArgs>
    {
        public GridDataTableModelFilterChangedCommandBehavior(Func<object, GridFilterEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataTableModelFilterChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Model.FilterChanged += OnEventRaised;
        }
    }

    // GridDataTableModelFilterChangedCommand
    public class GridDataTableModelFilterChangedCommand : GridDataControlCommandBase<GridDataTableModelFilterChangedCommandBehavior>
    { }

    // GridDataTableModelFilterChangedCommandBehavior
    public class GridDataTableModelFilterChangedCommandBehavior : GridDataTableModelFilterChangedCommandBehavior<object>
    { }

    // GridDataTableModelFilterChangedCommandWithEventArgs	
    public class GridDataTableModelFilterChangedCommandWithEventArgs : GridDataTableModelFilterChangedCommand<GridFilterEventArgs, GridDataTableModelFilterChangedCommandBehaviorWithEventArgs>
    { }

    // GridDataTableModelFilterChangedCommandBehaviorWithEventArgs
    public class GridDataTableModelFilterChangedCommandBehaviorWithEventArgs : GridDataTableModelFilterChangedCommandBehavior<GridFilterEventArgs>
    {
        public GridDataTableModelFilterChangedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataTableModelQueryUnboundColumnValue
    // GridDataTableModelQueryUnboundColumnValueCommand<T, TBehavior>
    public class GridDataTableModelQueryUnboundColumnValueCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataTableModelQueryUnboundColumnValueCommandBehavior<T>, new()
    { }

    // GridDataTableModelQueryUnboundColumnValueCommandBehavior<TReturn>
    public class GridDataTableModelQueryUnboundColumnValueCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, GridDataQueryUnboundColumnCellEventArgs>
    {
        public GridDataTableModelQueryUnboundColumnValueCommandBehavior(Func<object, GridDataQueryUnboundColumnCellEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataTableModelQueryUnboundColumnValueCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Model.QueryUnboundColumnValue += OnEventRaised;
        }
    }

    // GridDataTableModelQueryUnboundColumnValueCommand
    public class GridDataTableModelQueryUnboundColumnValueCommand : GridDataControlCommandBase<GridDataTableModelQueryUnboundColumnValueCommandBehavior>
    { }

    // GridDataTableModelQueryUnboundColumnValueCommandBehavior
    public class GridDataTableModelQueryUnboundColumnValueCommandBehavior : GridDataTableModelQueryUnboundColumnValueCommandBehavior<object>
    { }

    // GridDataTableModelQueryUnboundColumnValueCommandWithEventArgs	
    public class GridDataTableModelQueryUnboundColumnValueCommandWithEventArgs : GridDataTableModelQueryUnboundColumnValueCommand<GridDataQueryUnboundColumnCellEventArgs, GridDataTableModelQueryUnboundColumnValueCommandBehaviorWithEventArgs>
    { }

    // GridDataTableModelQueryUnboundColumnValueCommandBehaviorWithEventArgs
    public class GridDataTableModelQueryUnboundColumnValueCommandBehaviorWithEventArgs : GridDataTableModelQueryUnboundColumnValueCommandBehavior<GridDataQueryUnboundColumnCellEventArgs>
    {
        public GridDataTableModelQueryUnboundColumnValueCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataTableModelColumnsReset
    // GridDataTableModelColumnsResetCommand<T, TBehavior>
    public class GridDataTableModelColumnsResetCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataTableModelColumnsResetCommandBehavior<T>, new()
    { }

    // GridDataTableModelColumnsResetCommandBehavior<TReturn>
    public class GridDataTableModelColumnsResetCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, EventArgs>
    {
        public GridDataTableModelColumnsResetCommandBehavior(Func<object, EventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataTableModelColumnsResetCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Model.ColumnsReset += OnEventRaised;
        }
    }

    // GridDataTableModelColumnsResetCommand
    public class GridDataTableModelColumnsResetCommand : GridDataControlCommandBase<GridDataTableModelColumnsResetCommandBehavior>
    { }

    // GridDataTableModelColumnsResetCommandBehavior
    public class GridDataTableModelColumnsResetCommandBehavior : GridDataTableModelColumnsResetCommandBehavior<object>
    { }

    // GridDataTableModelColumnsResetCommandWithEventArgs	
    public class GridDataTableModelColumnsResetCommandWithEventArgs : GridDataTableModelColumnsResetCommand<EventArgs, GridDataTableModelColumnsResetCommandBehaviorWithEventArgs>
    { }

    // GridDataTableModelColumnsResetCommandBehaviorWithEventArgs
    public class GridDataTableModelColumnsResetCommandBehaviorWithEventArgs : GridDataTableModelColumnsResetCommandBehavior<EventArgs>
    {
        public GridDataTableModelColumnsResetCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataTableModelOleDropAtRowCol
    // GridDataTableModelOleDropAtRowColCommand<T, TBehavior>
    public class GridDataTableModelOleDropAtRowColCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataTableModelOleDropAtRowColCommandBehavior<T>, new()
    { }

    // GridDataTableModelOleDropAtRowColCommandBehavior<TReturn>
    public class GridDataTableModelOleDropAtRowColCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, GridOleDropAtRowColEventArgs>
    {
        public GridDataTableModelOleDropAtRowColCommandBehavior(Func<object, GridOleDropAtRowColEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataTableModelOleDropAtRowColCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Model.OleDropAtRowCol += OnEventRaised;
        }
    }

    // GridDataTableModelOleDropAtRowColCommand
    public class GridDataTableModelOleDropAtRowColCommand : GridDataControlCommandBase<GridDataTableModelOleDropAtRowColCommandBehavior>
    { }

    // GridDataTableModelOleDropAtRowColCommandBehavior
    public class GridDataTableModelOleDropAtRowColCommandBehavior : GridDataTableModelOleDropAtRowColCommandBehavior<object>
    { }

    // GridDataTableModelOleDropAtRowColCommandWithEventArgs	
    public class GridDataTableModelOleDropAtRowColCommandWithEventArgs : GridDataTableModelOleDropAtRowColCommand<GridOleDropAtRowColEventArgs, GridDataTableModelOleDropAtRowColCommandBehaviorWithEventArgs>
    { }

    // GridDataTableModelOleDropAtRowColCommandBehaviorWithEventArgs
    public class GridDataTableModelOleDropAtRowColCommandBehaviorWithEventArgs : GridDataTableModelOleDropAtRowColCommandBehavior<GridOleDropAtRowColEventArgs>
    {
        public GridDataTableModelOleDropAtRowColCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataTableModelOleDroppedData
    // GridDataTableModelOleDroppedDataCommand<T, TBehavior>
    public class GridDataTableModelOleDroppedDataCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataTableModelOleDroppedDataCommandBehavior<T>, new()
    { }

    // GridDataTableModelOleDroppedDataCommandBehavior<TReturn>
    public class GridDataTableModelOleDroppedDataCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, EventArgs>
    {
        public GridDataTableModelOleDroppedDataCommandBehavior(Func<object, EventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataTableModelOleDroppedDataCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Model.OleDroppedData += OnEventRaised;
        }
    }

    // GridDataTableModelOleDroppedDataCommand
    public class GridDataTableModelOleDroppedDataCommand : GridDataControlCommandBase<GridDataTableModelOleDroppedDataCommandBehavior>
    { }

    // GridDataTableModelOleDroppedDataCommandBehavior
    public class GridDataTableModelOleDroppedDataCommandBehavior : GridDataTableModelOleDroppedDataCommandBehavior<object>
    { }

    // GridDataTableModelOleDroppedDataCommandWithEventArgs	
    public class GridDataTableModelOleDroppedDataCommandWithEventArgs : GridDataTableModelOleDroppedDataCommand<EventArgs, GridDataTableModelOleDroppedDataCommandBehaviorWithEventArgs>
    { }

    // GridDataTableModelOleDroppedDataCommandBehaviorWithEventArgs
    public class GridDataTableModelOleDroppedDataCommandBehaviorWithEventArgs : GridDataTableModelOleDroppedDataCommandBehavior<EventArgs>
    {
        public GridDataTableModelOleDroppedDataCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataTableModelQueryOleDataSourceData
    // GridDataTableModelQueryOleDataSourceDataCommand<T, TBehavior>
    public class GridDataTableModelQueryOleDataSourceDataCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataTableModelQueryOleDataSourceDataCommandBehavior<T>, new()
    { }

    // GridDataTableModelQueryOleDataSourceDataCommandBehavior<TReturn>
    public class GridDataTableModelQueryOleDataSourceDataCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, GridQueryOleDataSourceDataEventArgs>
    {
        public GridDataTableModelQueryOleDataSourceDataCommandBehavior(Func<object, GridQueryOleDataSourceDataEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataTableModelQueryOleDataSourceDataCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Model.QueryOleDataSourceData += OnEventRaised;
        }
    }

    // GridDataTableModelQueryOleDataSourceDataCommand
    public class GridDataTableModelQueryOleDataSourceDataCommand : GridDataControlCommandBase<GridDataTableModelQueryOleDataSourceDataCommandBehavior>
    { }

    // GridDataTableModelQueryOleDataSourceDataCommandBehavior
    public class GridDataTableModelQueryOleDataSourceDataCommandBehavior : GridDataTableModelQueryOleDataSourceDataCommandBehavior<object>
    { }

    // GridDataTableModelQueryOleDataSourceDataCommandWithEventArgs	
    public class GridDataTableModelQueryOleDataSourceDataCommandWithEventArgs : GridDataTableModelQueryOleDataSourceDataCommand<GridQueryOleDataSourceDataEventArgs, GridDataTableModelQueryOleDataSourceDataCommandBehaviorWithEventArgs>
    { }

    // GridDataTableModelQueryOleDataSourceDataCommandBehaviorWithEventArgs
    public class GridDataTableModelQueryOleDataSourceDataCommandBehaviorWithEventArgs : GridDataTableModelQueryOleDataSourceDataCommandBehavior<GridQueryOleDataSourceDataEventArgs>
    {
        public GridDataTableModelQueryOleDataSourceDataCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataTableModelQueryDragDropMoveClearCells
    // GridDataTableModelQueryDragDropMoveClearCellsCommand<T, TBehavior>
    public class GridDataTableModelQueryDragDropMoveClearCellsCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataTableModelQueryDragDropMoveClearCellsCommandBehavior<T>, new()
    { }

    // GridDataTableModelQueryDragDropMoveClearCellsCommandBehavior<TReturn>
    public class GridDataTableModelQueryDragDropMoveClearCellsCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, CancelEventArgs>
    {
        public GridDataTableModelQueryDragDropMoveClearCellsCommandBehavior(Func<object, CancelEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataTableModelQueryDragDropMoveClearCellsCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Model.QueryDragDropMoveClearCells += OnEventRaised;
        }
    }

    // GridDataTableModelQueryDragDropMoveClearCellsCommand
    public class GridDataTableModelQueryDragDropMoveClearCellsCommand : GridDataControlCommandBase<GridDataTableModelQueryDragDropMoveClearCellsCommandBehavior>
    { }

    // GridDataTableModelQueryDragDropMoveClearCellsCommandBehavior
    public class GridDataTableModelQueryDragDropMoveClearCellsCommandBehavior : GridDataTableModelQueryDragDropMoveClearCellsCommandBehavior<object>
    { }

    // GridDataTableModelQueryDragDropMoveClearCellsCommandWithEventArgs	
    public class GridDataTableModelQueryDragDropMoveClearCellsCommandWithEventArgs : GridDataTableModelQueryDragDropMoveClearCellsCommand<CancelEventArgs, GridDataTableModelQueryDragDropMoveClearCellsCommandBehaviorWithEventArgs>
    { }

    // GridDataTableModelQueryDragDropMoveClearCellsCommandBehaviorWithEventArgs
    public class GridDataTableModelQueryDragDropMoveClearCellsCommandBehaviorWithEventArgs : GridDataTableModelQueryDragDropMoveClearCellsCommandBehavior<CancelEventArgs>
    {
        public GridDataTableModelQueryDragDropMoveClearCellsCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataTableModelDisposing
    // GridDataTableModelDisposingCommand<T, TBehavior>
    public class GridDataTableModelDisposingCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataTableModelDisposingCommandBehavior<T>, new()
    { }

    // GridDataTableModelDisposingCommandBehavior<TReturn>
    public class GridDataTableModelDisposingCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, EventArgs>
    {
        public GridDataTableModelDisposingCommandBehavior(Func<object, EventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataTableModelDisposingCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Model.Disposing += OnEventRaised;
        }
    }

    // GridDataTableModelDisposingCommand
    public class GridDataTableModelDisposingCommand : GridDataControlCommandBase<GridDataTableModelDisposingCommandBehavior>
    { }

    // GridDataTableModelDisposingCommandBehavior
    public class GridDataTableModelDisposingCommandBehavior : GridDataTableModelDisposingCommandBehavior<object>
    { }

    // GridDataTableModelDisposingCommandWithEventArgs	
    public class GridDataTableModelDisposingCommandWithEventArgs : GridDataTableModelDisposingCommand<EventArgs, GridDataTableModelDisposingCommandBehaviorWithEventArgs>
    { }

    // GridDataTableModelDisposingCommandBehaviorWithEventArgs
    public class GridDataTableModelDisposingCommandBehaviorWithEventArgs : GridDataTableModelDisposingCommandBehavior<EventArgs>
    {
        public GridDataTableModelDisposingCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

#if !SILVERLIGHT
    #region GridDataTableModelQueryContextMenuInfo
    // GridDataTableModelQueryContextMenuInfoCommand<T, TBehavior>
    public class GridDataTableModelQueryContextMenuInfoCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataTableModelQueryContextMenuInfoCommandBehavior<T>, new()
    { }

    // GridDataTableModelQueryContextMenuInfoCommandBehavior<TReturn>
    public class GridDataTableModelQueryContextMenuInfoCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, GridQueryContextMenuInfoEventArgs>
    {
        public GridDataTableModelQueryContextMenuInfoCommandBehavior(Func<object, GridQueryContextMenuInfoEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataTableModelQueryContextMenuInfoCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Model.QueryContextMenuInfo += OnEventRaised;
        }
    }

    // GridDataTableModelQueryContextMenuInfoCommand
    public class GridDataTableModelQueryContextMenuInfoCommand : GridDataControlCommandBase<GridDataTableModelQueryContextMenuInfoCommandBehavior>
    { }

    // GridDataTableModelQueryContextMenuInfoCommandBehavior
    public class GridDataTableModelQueryContextMenuInfoCommandBehavior : GridDataTableModelQueryContextMenuInfoCommandBehavior<object>
    { }

    // GridDataTableModelQueryContextMenuInfoCommandWithEventArgs	
    public class GridDataTableModelQueryContextMenuInfoCommandWithEventArgs : GridDataTableModelQueryContextMenuInfoCommand<GridQueryContextMenuInfoEventArgs, GridDataTableModelQueryContextMenuInfoCommandBehaviorWithEventArgs>
    { }

    // GridDataTableModelQueryContextMenuInfoCommandBehaviorWithEventArgs
    public class GridDataTableModelQueryContextMenuInfoCommandBehaviorWithEventArgs : GridDataTableModelQueryContextMenuInfoCommandBehavior<GridQueryContextMenuInfoEventArgs>
    {
        public GridDataTableModelQueryContextMenuInfoCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
#endif

    #region GridDataTableModelQueryCellInfo
    // GridDataTableModelQueryCellInfoCommand<T, TBehavior>
    public class GridDataTableModelQueryCellInfoCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataTableModelQueryCellInfoCommandBehavior<T>, new()
    { }

    // GridDataTableModelQueryCellInfoCommandBehavior<TReturn>
    public class GridDataTableModelQueryCellInfoCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, GridQueryCellInfoEventArgs>
    {
        public GridDataTableModelQueryCellInfoCommandBehavior(Func<object, GridQueryCellInfoEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataTableModelQueryCellInfoCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Model.QueryCellInfo += OnEventRaised;
        }
    }

    // GridDataTableModelQueryCellInfoCommand
    public class GridDataTableModelQueryCellInfoCommand : GridDataControlCommandBase<GridDataTableModelQueryCellInfoCommandBehavior>
    { }

    // GridDataTableModelQueryCellInfoCommandBehavior
    public class GridDataTableModelQueryCellInfoCommandBehavior : GridDataTableModelQueryCellInfoCommandBehavior<object>
    { }

    // GridDataTableModelQueryCellInfoCommandWithEventArgs	
    public class GridDataTableModelQueryCellInfoCommandWithEventArgs : GridDataTableModelQueryCellInfoCommand<GridQueryCellInfoEventArgs, GridDataTableModelQueryCellInfoCommandBehaviorWithEventArgs>
    { }

    // GridDataTableModelQueryCellInfoCommandBehaviorWithEventArgs
    public class GridDataTableModelQueryCellInfoCommandBehaviorWithEventArgs : GridDataTableModelQueryCellInfoCommandBehavior<GridQueryCellInfoEventArgs>
    {
        public GridDataTableModelQueryCellInfoCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataTableModelQueryVisibleColumnInfo

    // GridDataTableModelQueryVisibleColumnInfoCommand<T, TBehavior>
    public class GridDataTableModelQueryVisibleColumnInfoCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataTableModelQueryVisibleColumnInfoCommandBehavior<T>, new()
    { }

    // GridDataTableModelQueryVisibleColumnInfoCommandBehavior<TReturn>
    public class GridDataTableModelQueryVisibleColumnInfoCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, QueryVisibleColumnInfoArgs>
    {
        public GridDataTableModelQueryVisibleColumnInfoCommandBehavior(Func<object, QueryVisibleColumnInfoArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataTableModelQueryVisibleColumnInfoCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Model.QueryVisibleColumnInfo += OnEventRaised;
        }
    }

    // GridDataTableModelQueryVisibleColumnInfoCommand
    public class GridDataTableModelQueryVisibleColumnInfoCommand : GridDataControlCommandBase<GridDataTableModelQueryVisibleColumnInfoCommandBehavior>
    { }

    // GridDataTableModelQueryVisibleColumnInfoCommandBehavior
    public class GridDataTableModelQueryVisibleColumnInfoCommandBehavior : GridDataTableModelQueryVisibleColumnInfoCommandBehavior<object>
    { }

    // GridDataTableModelQueryVisibleColumnInfoCommandWithEventArgs	
    public class GridDataTableModelQueryVisibleColumnInfoCommandWithEventArgs : GridDataTableModelQueryVisibleColumnInfoCommand<QueryVisibleColumnInfoArgs, GridDataTableModelQueryVisibleColumnInfoCommandBehaviorWithEventArgs>
    { }

    // GridDataTableModelQueryVisibleColumnInfoCommandBehaviorWithEventArgs
    public class GridDataTableModelQueryVisibleColumnInfoCommandBehaviorWithEventArgs : GridDataTableModelQueryVisibleColumnInfoCommandBehavior<QueryVisibleColumnInfoArgs>
    {
        public GridDataTableModelQueryVisibleColumnInfoCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataTableModelCommitCellInfo
    // GridDataTableModelCommitCellInfoCommand<T, TBehavior>
    public class GridDataTableModelCommitCellInfoCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataTableModelCommitCellInfoCommandBehavior<T>, new()
    { }

    // GridDataTableModelCommitCellInfoCommandBehavior<TReturn>
    public class GridDataTableModelCommitCellInfoCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, GridCommitCellInfoEventArgs>
    {
        public GridDataTableModelCommitCellInfoCommandBehavior(Func<object, GridCommitCellInfoEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataTableModelCommitCellInfoCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Model.CommitCellInfo += OnEventRaised;
        }
    }

    // GridDataTableModelCommitCellInfoCommand
    public class GridDataTableModelCommitCellInfoCommand : GridDataControlCommandBase<GridDataTableModelCommitCellInfoCommandBehavior>
    { }

    // GridDataTableModelCommitCellInfoCommandBehavior
    public class GridDataTableModelCommitCellInfoCommandBehavior : GridDataTableModelCommitCellInfoCommandBehavior<object>
    { }

    // GridDataTableModelCommitCellInfoCommandWithEventArgs	
    public class GridDataTableModelCommitCellInfoCommandWithEventArgs : GridDataTableModelCommitCellInfoCommand<GridCommitCellInfoEventArgs, GridDataTableModelCommitCellInfoCommandBehaviorWithEventArgs>
    { }

    // GridDataTableModelCommitCellInfoCommandBehaviorWithEventArgs
    public class GridDataTableModelCommitCellInfoCommandBehaviorWithEventArgs : GridDataTableModelCommitCellInfoCommandBehavior<GridCommitCellInfoEventArgs>
    {
        public GridDataTableModelCommitCellInfoCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataTableModelCommittedCellInfo
    // GridDataTableModelCommittedCellInfoCommand<T, TBehavior>
    public class GridDataTableModelCommittedCellInfoCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataTableModelCommittedCellInfoCommandBehavior<T>, new()
    { }

    // GridDataTableModelCommittedCellInfoCommandBehavior<TReturn>
    public class GridDataTableModelCommittedCellInfoCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, GridCommitCellInfoEventArgs>
    {
        public GridDataTableModelCommittedCellInfoCommandBehavior(Func<object, GridCommitCellInfoEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataTableModelCommittedCellInfoCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Model.CommittedCellInfo += OnEventRaised;
        }
    }

    // GridDataTableModelCommittedCellInfoCommand
    public class GridDataTableModelCommittedCellInfoCommand : GridDataControlCommandBase<GridDataTableModelCommittedCellInfoCommandBehavior>
    { }

    // GridDataTableModelCommittedCellInfoCommandBehavior
    public class GridDataTableModelCommittedCellInfoCommandBehavior : GridDataTableModelCommittedCellInfoCommandBehavior<object>
    { }

    // GridDataTableModelCommittedCellInfoCommandWithEventArgs	
    public class GridDataTableModelCommittedCellInfoCommandWithEventArgs : GridDataTableModelCommittedCellInfoCommand<GridCommitCellInfoEventArgs, GridDataTableModelCommittedCellInfoCommandBehaviorWithEventArgs>
    { }

    // GridDataTableModelCommittedCellInfoCommandBehaviorWithEventArgs
    public class GridDataTableModelCommittedCellInfoCommandBehaviorWithEventArgs : GridDataTableModelCommittedCellInfoCommandBehavior<GridCommitCellInfoEventArgs>
    {
        public GridDataTableModelCommittedCellInfoCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataTableModelQueryBaseStyles
    // GridDataTableModelQueryBaseStylesCommand<T, TBehavior>
    public class GridDataTableModelQueryBaseStylesCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataTableModelQueryBaseStylesCommandBehavior<T>, new()
    { }

    // GridDataTableModelQueryBaseStylesCommandBehavior<TReturn>
    public class GridDataTableModelQueryBaseStylesCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, GridQueryBaseStylesEventArgs>
    {
        public GridDataTableModelQueryBaseStylesCommandBehavior(Func<object, GridQueryBaseStylesEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataTableModelQueryBaseStylesCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Model.QueryBaseStyles += OnEventRaised;
        }
    }

    // GridDataTableModelQueryBaseStylesCommand
    public class GridDataTableModelQueryBaseStylesCommand : GridDataControlCommandBase<GridDataTableModelQueryBaseStylesCommandBehavior>
    { }

    // GridDataTableModelQueryBaseStylesCommandBehavior
    public class GridDataTableModelQueryBaseStylesCommandBehavior : GridDataTableModelQueryBaseStylesCommandBehavior<object>
    { }

    // GridDataTableModelQueryBaseStylesCommandWithEventArgs	
    public class GridDataTableModelQueryBaseStylesCommandWithEventArgs : GridDataTableModelQueryBaseStylesCommand<GridQueryBaseStylesEventArgs, GridDataTableModelQueryBaseStylesCommandBehaviorWithEventArgs>
    { }

    // GridDataTableModelQueryBaseStylesCommandBehaviorWithEventArgs
    public class GridDataTableModelQueryBaseStylesCommandBehaviorWithEventArgs : GridDataTableModelQueryBaseStylesCommandBehavior<GridQueryBaseStylesEventArgs>
    {
        public GridDataTableModelQueryBaseStylesCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataTableModelRowsInserted
    // GridDataTableModelRowsInsertedCommand<T, TBehavior>
    public class GridDataTableModelRowsInsertedCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataTableModelRowsInsertedCommandBehavior<T>, new()
    { }

    // GridDataTableModelRowsInsertedCommandBehavior<TReturn>
    public class GridDataTableModelRowsInsertedCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, GridRangeInsertedEventArgs>
    {
        public GridDataTableModelRowsInsertedCommandBehavior(Func<object, GridRangeInsertedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataTableModelRowsInsertedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Model.RowsInserted += OnEventRaised;
        }
    }

    // GridDataTableModelRowsInsertedCommand
    public class GridDataTableModelRowsInsertedCommand : GridDataControlCommandBase<GridDataTableModelRowsInsertedCommandBehavior>
    { }

    // GridDataTableModelRowsInsertedCommandBehavior
    public class GridDataTableModelRowsInsertedCommandBehavior : GridDataTableModelRowsInsertedCommandBehavior<object>
    { }

    // GridDataTableModelRowsInsertedCommandWithEventArgs	
    public class GridDataTableModelRowsInsertedCommandWithEventArgs : GridDataTableModelRowsInsertedCommand<GridRangeInsertedEventArgs, GridDataTableModelRowsInsertedCommandBehaviorWithEventArgs>
    { }

    // GridDataTableModelRowsInsertedCommandBehaviorWithEventArgs
    public class GridDataTableModelRowsInsertedCommandBehaviorWithEventArgs : GridDataTableModelRowsInsertedCommandBehavior<GridRangeInsertedEventArgs>
    {
        public GridDataTableModelRowsInsertedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataTableModelRowsRemoved
    // GridDataTableModelRowsRemovedCommand<T, TBehavior>
    public class GridDataTableModelRowsRemovedCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataTableModelRowsRemovedCommandBehavior<T>, new()
    { }

    // GridDataTableModelRowsRemovedCommandBehavior<TReturn>
    public class GridDataTableModelRowsRemovedCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, GridRangeRemovedEventArgs>
    {
        public GridDataTableModelRowsRemovedCommandBehavior(Func<object, GridRangeRemovedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataTableModelRowsRemovedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Model.RowsRemoved += OnEventRaised;
        }
    }

    // GridDataTableModelRowsRemovedCommand
    public class GridDataTableModelRowsRemovedCommand : GridDataControlCommandBase<GridDataTableModelRowsRemovedCommandBehavior>
    { }

    // GridDataTableModelRowsRemovedCommandBehavior
    public class GridDataTableModelRowsRemovedCommandBehavior : GridDataTableModelRowsRemovedCommandBehavior<object>
    { }

    // GridDataTableModelRowsRemovedCommandWithEventArgs	
    public class GridDataTableModelRowsRemovedCommandWithEventArgs : GridDataTableModelRowsRemovedCommand<GridRangeRemovedEventArgs, GridDataTableModelRowsRemovedCommandBehaviorWithEventArgs>
    { }

    // GridDataTableModelRowsRemovedCommandBehaviorWithEventArgs
    public class GridDataTableModelRowsRemovedCommandBehaviorWithEventArgs : GridDataTableModelRowsRemovedCommandBehavior<GridRangeRemovedEventArgs>
    {
        public GridDataTableModelRowsRemovedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataTableModelColumnsInserted
    // GridDataTableModelColumnsInsertedCommand<T, TBehavior>
    public class GridDataTableModelColumnsInsertedCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataTableModelColumnsInsertedCommandBehavior<T>, new()
    { }

    // GridDataTableModelColumnsInsertedCommandBehavior<TReturn>
    public class GridDataTableModelColumnsInsertedCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, GridRangeInsertedEventArgs>
    {
        public GridDataTableModelColumnsInsertedCommandBehavior(Func<object, GridRangeInsertedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataTableModelColumnsInsertedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Model.ColumnsInserted += OnEventRaised;
        }
    }

    // GridDataTableModelColumnsInsertedCommand
    public class GridDataTableModelColumnsInsertedCommand : GridDataControlCommandBase<GridDataTableModelColumnsInsertedCommandBehavior>
    { }

    // GridDataTableModelColumnsInsertedCommandBehavior
    public class GridDataTableModelColumnsInsertedCommandBehavior : GridDataTableModelColumnsInsertedCommandBehavior<object>
    { }

    // GridDataTableModelColumnsInsertedCommandWithEventArgs	
    public class GridDataTableModelColumnsInsertedCommandWithEventArgs : GridDataTableModelColumnsInsertedCommand<GridRangeInsertedEventArgs, GridDataTableModelColumnsInsertedCommandBehaviorWithEventArgs>
    { }

    // GridDataTableModelColumnsInsertedCommandBehaviorWithEventArgs
    public class GridDataTableModelColumnsInsertedCommandBehaviorWithEventArgs : GridDataTableModelColumnsInsertedCommandBehavior<GridRangeInsertedEventArgs>
    {
        public GridDataTableModelColumnsInsertedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataTableModelColumnsRemoved
    // GridDataTableModelColumnsRemovedCommand<T, TBehavior>
    public class GridDataTableModelColumnsRemovedCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataTableModelColumnsRemovedCommandBehavior<T>, new()
    { }

    // GridDataTableModelColumnsRemovedCommandBehavior<TReturn>
    public class GridDataTableModelColumnsRemovedCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, GridRangeRemovedEventArgs>
    {
        public GridDataTableModelColumnsRemovedCommandBehavior(Func<object, GridRangeRemovedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataTableModelColumnsRemovedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Model.ColumnsRemoved += OnEventRaised;
        }
    }

    // GridDataTableModelColumnsRemovedCommand
    public class GridDataTableModelColumnsRemovedCommand : GridDataControlCommandBase<GridDataTableModelColumnsRemovedCommandBehavior>
    { }

    // GridDataTableModelColumnsRemovedCommandBehavior
    public class GridDataTableModelColumnsRemovedCommandBehavior : GridDataTableModelColumnsRemovedCommandBehavior<object>
    { }

    // GridDataTableModelColumnsRemovedCommandWithEventArgs	
    public class GridDataTableModelColumnsRemovedCommandWithEventArgs : GridDataTableModelColumnsRemovedCommand<GridRangeRemovedEventArgs, GridDataTableModelColumnsRemovedCommandBehaviorWithEventArgs>
    { }

    // GridDataTableModelColumnsRemovedCommandBehaviorWithEventArgs
    public class GridDataTableModelColumnsRemovedCommandBehaviorWithEventArgs : GridDataTableModelColumnsRemovedCommandBehavior<GridRangeRemovedEventArgs>
    {
        public GridDataTableModelColumnsRemovedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataTableModelRowsMoved
    // GridDataTableModelRowsMovedCommand<T, TBehavior>
    public class GridDataTableModelRowsMovedCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataTableModelRowsMovedCommandBehavior<T>, new()
    { }

    // GridDataTableModelRowsMovedCommandBehavior<TReturn>
    public class GridDataTableModelRowsMovedCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, GridRangeMovedEventArgs>
    {
        public GridDataTableModelRowsMovedCommandBehavior(Func<object, GridRangeMovedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataTableModelRowsMovedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Model.RowsMoved += OnEventRaised;
        }
    }

    // GridDataTableModelRowsMovedCommand
    public class GridDataTableModelRowsMovedCommand : GridDataControlCommandBase<GridDataTableModelRowsMovedCommandBehavior>
    { }

    // GridDataTableModelRowsMovedCommandBehavior
    public class GridDataTableModelRowsMovedCommandBehavior : GridDataTableModelRowsMovedCommandBehavior<object>
    { }

    // GridDataTableModelRowsMovedCommandWithEventArgs	
    public class GridDataTableModelRowsMovedCommandWithEventArgs : GridDataTableModelRowsMovedCommand<GridRangeMovedEventArgs, GridDataTableModelRowsMovedCommandBehaviorWithEventArgs>
    { }

    // GridDataTableModelRowsMovedCommandBehaviorWithEventArgs
    public class GridDataTableModelRowsMovedCommandBehaviorWithEventArgs : GridDataTableModelRowsMovedCommandBehavior<GridRangeMovedEventArgs>
    {
        public GridDataTableModelRowsMovedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataTableModelColumnsMoved
    // GridDataTableModelColumnsMovedCommand<T, TBehavior>
    public class GridDataTableModelColumnsMovedCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataTableModelColumnsMovedCommandBehavior<T>, new()
    { }

    // GridDataTableModelColumnsMovedCommandBehavior<TReturn>
    public class GridDataTableModelColumnsMovedCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, GridRangeMovedEventArgs>
    {
        public GridDataTableModelColumnsMovedCommandBehavior(Func<object, GridRangeMovedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataTableModelColumnsMovedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Model.ColumnsMoved += OnEventRaised;
        }
    }

    // GridDataTableModelColumnsMovedCommand
    public class GridDataTableModelColumnsMovedCommand : GridDataControlCommandBase<GridDataTableModelColumnsMovedCommandBehavior>
    { }

    // GridDataTableModelColumnsMovedCommandBehavior
    public class GridDataTableModelColumnsMovedCommandBehavior : GridDataTableModelColumnsMovedCommandBehavior<object>
    { }

    // GridDataTableModelColumnsMovedCommandWithEventArgs	
    public class GridDataTableModelColumnsMovedCommandWithEventArgs : GridDataTableModelColumnsMovedCommand<GridRangeMovedEventArgs, GridDataTableModelColumnsMovedCommandBehaviorWithEventArgs>
    { }

    // GridDataTableModelColumnsMovedCommandBehaviorWithEventArgs
    public class GridDataTableModelColumnsMovedCommandBehaviorWithEventArgs : GridDataTableModelColumnsMovedCommandBehavior<GridRangeMovedEventArgs>
    {
        public GridDataTableModelColumnsMovedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataTableModelBaseStylesMapChanged
    // GridDataTableModelBaseStylesMapChangedCommand<T, TBehavior>
    public class GridDataTableModelBaseStylesMapChangedCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataTableModelBaseStylesMapChangedCommandBehavior<T>, new()
    { }

    // GridDataTableModelBaseStylesMapChangedCommandBehavior<TReturn>
    public class GridDataTableModelBaseStylesMapChangedCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, EventArgs>
    {
        public GridDataTableModelBaseStylesMapChangedCommandBehavior(Func<object, EventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataTableModelBaseStylesMapChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Model.BaseStylesMapChanged += OnEventRaised;
        }
    }

    // GridDataTableModelBaseStylesMapChangedCommand
    public class GridDataTableModelBaseStylesMapChangedCommand : GridDataControlCommandBase<GridDataTableModelBaseStylesMapChangedCommandBehavior>
    { }

    // GridDataTableModelBaseStylesMapChangedCommandBehavior
    public class GridDataTableModelBaseStylesMapChangedCommandBehavior : GridDataTableModelBaseStylesMapChangedCommandBehavior<object>
    { }

    // GridDataTableModelBaseStylesMapChangedCommandWithEventArgs	
    public class GridDataTableModelBaseStylesMapChangedCommandWithEventArgs : GridDataTableModelBaseStylesMapChangedCommand<EventArgs, GridDataTableModelBaseStylesMapChangedCommandBehaviorWithEventArgs>
    { }

    // GridDataTableModelBaseStylesMapChangedCommandBehaviorWithEventArgs
    public class GridDataTableModelBaseStylesMapChangedCommandBehaviorWithEventArgs : GridDataTableModelBaseStylesMapChangedCommandBehavior<EventArgs>
    {
        public GridDataTableModelBaseStylesMapChangedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

#if !SILVERLIGHT
    #region GridDataTableModelCellModelsChanged
    // GridDataTableModelCellModelsChangedCommand<T, TBehavior>
    public class GridDataTableModelCellModelsChangedCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataTableModelCellModelsChangedCommandBehavior<T>, new()
    { }

    // GridDataTableModelCellModelsChangedCommandBehavior<TReturn>
    public class GridDataTableModelCellModelsChangedCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, CollectionChangeEventArgs>
    {
        public GridDataTableModelCellModelsChangedCommandBehavior(Func<object, CollectionChangeEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataTableModelCellModelsChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Model.CellModelsChanged += OnEventRaised;
        }
    }

    // GridDataTableModelCellModelsChangedCommand
    public class GridDataTableModelCellModelsChangedCommand : GridDataControlCommandBase<GridDataTableModelCellModelsChangedCommandBehavior>
    { }

    // GridDataTableModelCellModelsChangedCommandBehavior
    public class GridDataTableModelCellModelsChangedCommandBehavior : GridDataTableModelCellModelsChangedCommandBehavior<object>
    { }

    // GridDataTableModelCellModelsChangedCommandWithEventArgs	
    public class GridDataTableModelCellModelsChangedCommandWithEventArgs : GridDataTableModelCellModelsChangedCommand<CollectionChangeEventArgs, GridDataTableModelCellModelsChangedCommandBehaviorWithEventArgs>
    { }

    // GridDataTableModelCellModelsChangedCommandBehaviorWithEventArgs
    public class GridDataTableModelCellModelsChangedCommandBehaviorWithEventArgs : GridDataTableModelCellModelsChangedCommandBehavior<CollectionChangeEventArgs>
    {
        public GridDataTableModelCellModelsChangedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
#endif

    #region GridDataTableModelQueryCellModel
    // GridDataTableModelQueryCellModelCommand<T, TBehavior>
    public class GridDataTableModelQueryCellModelCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataTableModelQueryCellModelCommandBehavior<T>, new()
    { }

    // GridDataTableModelQueryCellModelCommandBehavior<TReturn>
    public class GridDataTableModelQueryCellModelCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, GridQueryCellModelEventArgs>
    {
        public GridDataTableModelQueryCellModelCommandBehavior(Func<object, GridQueryCellModelEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataTableModelQueryCellModelCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Model.QueryCellModel += OnEventRaised;
        }
    }

    // GridDataTableModelQueryCellModelCommand
    public class GridDataTableModelQueryCellModelCommand : GridDataControlCommandBase<GridDataTableModelQueryCellModelCommandBehavior>
    { }

    // GridDataTableModelQueryCellModelCommandBehavior
    public class GridDataTableModelQueryCellModelCommandBehavior : GridDataTableModelQueryCellModelCommandBehavior<object>
    { }

    // GridDataTableModelQueryCellModelCommandWithEventArgs	
    public class GridDataTableModelQueryCellModelCommandWithEventArgs : GridDataTableModelQueryCellModelCommand<GridQueryCellModelEventArgs, GridDataTableModelQueryCellModelCommandBehaviorWithEventArgs>
    { }

    // GridDataTableModelQueryCellModelCommandBehaviorWithEventArgs
    public class GridDataTableModelQueryCellModelCommandBehaviorWithEventArgs : GridDataTableModelQueryCellModelCommandBehavior<GridQueryCellModelEventArgs>
    {
        public GridDataTableModelQueryCellModelCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataTableModelQueryCellFormattedText
    // GridDataTableModelQueryCellFormattedTextCommand<T, TBehavior>
    public class GridDataTableModelQueryCellFormattedTextCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataTableModelQueryCellFormattedTextCommandBehavior<T>, new()
    { }

    // GridDataTableModelQueryCellFormattedTextCommandBehavior<TReturn>
    public class GridDataTableModelQueryCellFormattedTextCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, GridCellTextEventArgs>
    {
        public GridDataTableModelQueryCellFormattedTextCommandBehavior(Func<object, GridCellTextEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataTableModelQueryCellFormattedTextCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Model.QueryCellFormattedText += OnEventRaised;
        }
    }

    // GridDataTableModelQueryCellFormattedTextCommand
    public class GridDataTableModelQueryCellFormattedTextCommand : GridDataControlCommandBase<GridDataTableModelQueryCellFormattedTextCommandBehavior>
    { }

    // GridDataTableModelQueryCellFormattedTextCommandBehavior
    public class GridDataTableModelQueryCellFormattedTextCommandBehavior : GridDataTableModelQueryCellFormattedTextCommandBehavior<object>
    { }

    // GridDataTableModelQueryCellFormattedTextCommandWithEventArgs	
    public class GridDataTableModelQueryCellFormattedTextCommandWithEventArgs : GridDataTableModelQueryCellFormattedTextCommand<GridCellTextEventArgs, GridDataTableModelQueryCellFormattedTextCommandBehaviorWithEventArgs>
    { }

    // GridDataTableModelQueryCellFormattedTextCommandBehaviorWithEventArgs
    public class GridDataTableModelQueryCellFormattedTextCommandBehaviorWithEventArgs : GridDataTableModelQueryCellFormattedTextCommandBehavior<GridCellTextEventArgs>
    {
        public GridDataTableModelQueryCellFormattedTextCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataTableModelSaveCellFormattedText
    // GridDataTableModelSaveCellFormattedTextCommand<T, TBehavior>
    public class GridDataTableModelSaveCellFormattedTextCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataTableModelSaveCellFormattedTextCommandBehavior<T>, new()
    { }

    // GridDataTableModelSaveCellFormattedTextCommandBehavior<TReturn>
    public class GridDataTableModelSaveCellFormattedTextCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, GridCellTextEventArgs>
    {
        public GridDataTableModelSaveCellFormattedTextCommandBehavior(Func<object, GridCellTextEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataTableModelSaveCellFormattedTextCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Model.SaveCellFormattedText += OnEventRaised;
        }
    }

    // GridDataTableModelSaveCellFormattedTextCommand
    public class GridDataTableModelSaveCellFormattedTextCommand : GridDataControlCommandBase<GridDataTableModelSaveCellFormattedTextCommandBehavior>
    { }

    // GridDataTableModelSaveCellFormattedTextCommandBehavior
    public class GridDataTableModelSaveCellFormattedTextCommandBehavior : GridDataTableModelSaveCellFormattedTextCommandBehavior<object>
    { }

    // GridDataTableModelSaveCellFormattedTextCommandWithEventArgs	
    public class GridDataTableModelSaveCellFormattedTextCommandWithEventArgs : GridDataTableModelSaveCellFormattedTextCommand<GridCellTextEventArgs, GridDataTableModelSaveCellFormattedTextCommandBehaviorWithEventArgs>
    { }

    // GridDataTableModelSaveCellFormattedTextCommandBehaviorWithEventArgs
    public class GridDataTableModelSaveCellFormattedTextCommandBehaviorWithEventArgs : GridDataTableModelSaveCellFormattedTextCommandBehavior<GridCellTextEventArgs>
    {
        public GridDataTableModelSaveCellFormattedTextCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataTableModelParseCommonFormats
    // GridDataTableModelParseCommonFormatsCommand<T, TBehavior>
    public class GridDataTableModelParseCommonFormatsCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataTableModelParseCommonFormatsCommandBehavior<T>, new()
    { }

    // GridDataTableModelParseCommonFormatsCommandBehavior<TReturn>
    public class GridDataTableModelParseCommonFormatsCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, GridCellTextEventArgs>
    {
        public GridDataTableModelParseCommonFormatsCommandBehavior(Func<object, GridCellTextEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataTableModelParseCommonFormatsCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Model.ParseCommonFormats += OnEventRaised;
        }
    }

    // GridDataTableModelParseCommonFormatsCommand
    public class GridDataTableModelParseCommonFormatsCommand : GridDataControlCommandBase<GridDataTableModelParseCommonFormatsCommandBehavior>
    { }

    // GridDataTableModelParseCommonFormatsCommandBehavior
    public class GridDataTableModelParseCommonFormatsCommandBehavior : GridDataTableModelParseCommonFormatsCommandBehavior<object>
    { }

    // GridDataTableModelParseCommonFormatsCommandWithEventArgs	
    public class GridDataTableModelParseCommonFormatsCommandWithEventArgs : GridDataTableModelParseCommonFormatsCommand<GridCellTextEventArgs, GridDataTableModelParseCommonFormatsCommandBehaviorWithEventArgs>
    { }

    // GridDataTableModelParseCommonFormatsCommandBehaviorWithEventArgs
    public class GridDataTableModelParseCommonFormatsCommandBehaviorWithEventArgs : GridDataTableModelParseCommonFormatsCommandBehavior<GridCellTextEventArgs>
    {
        public GridDataTableModelParseCommonFormatsCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataTableModelQueryCellText
    // GridDataTableModelQueryCellTextCommand<T, TBehavior>
    public class GridDataTableModelQueryCellTextCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataTableModelQueryCellTextCommandBehavior<T>, new()
    { }

    // GridDataTableModelQueryCellTextCommandBehavior<TReturn>
    public class GridDataTableModelQueryCellTextCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, GridCellTextEventArgs>
    {
        public GridDataTableModelQueryCellTextCommandBehavior(Func<object, GridCellTextEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataTableModelQueryCellTextCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Model.QueryCellText += OnEventRaised;
        }
    }

    // GridDataTableModelQueryCellTextCommand
    public class GridDataTableModelQueryCellTextCommand : GridDataControlCommandBase<GridDataTableModelQueryCellTextCommandBehavior>
    { }

    // GridDataTableModelQueryCellTextCommandBehavior
    public class GridDataTableModelQueryCellTextCommandBehavior : GridDataTableModelQueryCellTextCommandBehavior<object>
    { }

    // GridDataTableModelQueryCellTextCommandWithEventArgs	
    public class GridDataTableModelQueryCellTextCommandWithEventArgs : GridDataTableModelQueryCellTextCommand<GridCellTextEventArgs, GridDataTableModelQueryCellTextCommandBehaviorWithEventArgs>
    { }

    // GridDataTableModelQueryCellTextCommandBehaviorWithEventArgs
    public class GridDataTableModelQueryCellTextCommandBehaviorWithEventArgs : GridDataTableModelQueryCellTextCommandBehavior<GridCellTextEventArgs>
    {
        public GridDataTableModelQueryCellTextCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataTableModelSaveCellText
    // GridDataTableModelSaveCellTextCommand<T, TBehavior>
    public class GridDataTableModelSaveCellTextCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataTableModelSaveCellTextCommandBehavior<T>, new()
    { }

    // GridDataTableModelSaveCellTextCommandBehavior<TReturn>
    public class GridDataTableModelSaveCellTextCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, GridCellTextEventArgs>
    {
        public GridDataTableModelSaveCellTextCommandBehavior(Func<object, GridCellTextEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataTableModelSaveCellTextCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Model.SaveCellText += OnEventRaised;
        }
    }

    // GridDataTableModelSaveCellTextCommand
    public class GridDataTableModelSaveCellTextCommand : GridDataControlCommandBase<GridDataTableModelSaveCellTextCommandBehavior>
    { }

    // GridDataTableModelSaveCellTextCommandBehavior
    public class GridDataTableModelSaveCellTextCommandBehavior : GridDataTableModelSaveCellTextCommandBehavior<object>
    { }

    // GridDataTableModelSaveCellTextCommandWithEventArgs	
    public class GridDataTableModelSaveCellTextCommandWithEventArgs : GridDataTableModelSaveCellTextCommand<GridCellTextEventArgs, GridDataTableModelSaveCellTextCommandBehaviorWithEventArgs>
    { }

    // GridDataTableModelSaveCellTextCommandBehaviorWithEventArgs
    public class GridDataTableModelSaveCellTextCommandBehaviorWithEventArgs : GridDataTableModelSaveCellTextCommandBehavior<GridCellTextEventArgs>
    {
        public GridDataTableModelSaveCellTextCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataTableModelQueryCoveredRange
    // GridDataTableModelQueryCoveredRangeCommand<T, TBehavior>
    public class GridDataTableModelQueryCoveredRangeCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataTableModelQueryCoveredRangeCommandBehavior<T>, new()
    { }

    // GridDataTableModelQueryCoveredRangeCommandBehavior<TReturn>
    public class GridDataTableModelQueryCoveredRangeCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, GridQueryCoveredRangeEventArgs>
    {
        public GridDataTableModelQueryCoveredRangeCommandBehavior(Func<object, GridQueryCoveredRangeEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataTableModelQueryCoveredRangeCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Model.QueryCoveredRange += OnEventRaised;
        }
    }

    // GridDataTableModelQueryCoveredRangeCommand
    public class GridDataTableModelQueryCoveredRangeCommand : GridDataControlCommandBase<GridDataTableModelQueryCoveredRangeCommandBehavior>
    { }

    // GridDataTableModelQueryCoveredRangeCommandBehavior
    public class GridDataTableModelQueryCoveredRangeCommandBehavior : GridDataTableModelQueryCoveredRangeCommandBehavior<object>
    { }

    // GridDataTableModelQueryCoveredRangeCommandWithEventArgs	
    public class GridDataTableModelQueryCoveredRangeCommandWithEventArgs : GridDataTableModelQueryCoveredRangeCommand<GridQueryCoveredRangeEventArgs, GridDataTableModelQueryCoveredRangeCommandBehaviorWithEventArgs>
    { }

    // GridDataTableModelQueryCoveredRangeCommandBehaviorWithEventArgs
    public class GridDataTableModelQueryCoveredRangeCommandBehaviorWithEventArgs : GridDataTableModelQueryCoveredRangeCommandBehavior<GridQueryCoveredRangeEventArgs>
    {
        public GridDataTableModelQueryCoveredRangeCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataTableModelQueryCellSpanBackgrounds
    // GridDataTableModelQueryCellSpanBackgroundsCommand<T, TBehavior>
    public class GridDataTableModelQueryCellSpanBackgroundsCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataTableModelQueryCellSpanBackgroundsCommandBehavior<T>, new()
    { }

    // GridDataTableModelQueryCellSpanBackgroundsCommandBehavior<TReturn>
    public class GridDataTableModelQueryCellSpanBackgroundsCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, GridQueryCellSpanBackgroundsEventArgs>
    {
        public GridDataTableModelQueryCellSpanBackgroundsCommandBehavior(Func<object, GridQueryCellSpanBackgroundsEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataTableModelQueryCellSpanBackgroundsCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Model.QueryCellSpanBackgrounds += OnEventRaised;
        }
    }

    // GridDataTableModelQueryCellSpanBackgroundsCommand
    public class GridDataTableModelQueryCellSpanBackgroundsCommand : GridDataControlCommandBase<GridDataTableModelQueryCellSpanBackgroundsCommandBehavior>
    { }

    // GridDataTableModelQueryCellSpanBackgroundsCommandBehavior
    public class GridDataTableModelQueryCellSpanBackgroundsCommandBehavior : GridDataTableModelQueryCellSpanBackgroundsCommandBehavior<object>
    { }

    // GridDataTableModelQueryCellSpanBackgroundsCommandWithEventArgs	
    public class GridDataTableModelQueryCellSpanBackgroundsCommandWithEventArgs : GridDataTableModelQueryCellSpanBackgroundsCommand<GridQueryCellSpanBackgroundsEventArgs, GridDataTableModelQueryCellSpanBackgroundsCommandBehaviorWithEventArgs>
    { }

    // GridDataTableModelQueryCellSpanBackgroundsCommandBehaviorWithEventArgs
    public class GridDataTableModelQueryCellSpanBackgroundsCommandBehaviorWithEventArgs : GridDataTableModelQueryCellSpanBackgroundsCommandBehavior<GridQueryCellSpanBackgroundsEventArgs>
    {
        public GridDataTableModelQueryCellSpanBackgroundsCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataTableModelSelectionChanged
    // GridDataTableModelSelectionChangedCommand<T, TBehavior>
    public class GridDataTableModelSelectionChangedCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataTableModelSelectionChangedCommandBehavior<T>, new()
    { }

    // GridDataTableModelSelectionChangedCommandBehavior<TReturn>
    public class GridDataTableModelSelectionChangedCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, GridSelectionChangedEventArgs>
    {
        public GridDataTableModelSelectionChangedCommandBehavior(Func<object, GridSelectionChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataTableModelSelectionChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Model.SelectionChanged += OnEventRaised;
        }
    }

    // GridDataTableModelSelectionChangedCommand
    public class GridDataTableModelSelectionChangedCommand : GridDataControlCommandBase<GridDataTableModelSelectionChangedCommandBehavior>
    { }

    // GridDataTableModelSelectionChangedCommandBehavior
    public class GridDataTableModelSelectionChangedCommandBehavior : GridDataTableModelSelectionChangedCommandBehavior<object>
    { }

    // GridDataTableModelSelectionChangedCommandWithEventArgs	
    public class GridDataTableModelSelectionChangedCommandWithEventArgs : GridDataTableModelSelectionChangedCommand<GridSelectionChangedEventArgs, GridDataTableModelSelectionChangedCommandBehaviorWithEventArgs>
    { }

    // GridDataTableModelSelectionChangedCommandBehaviorWithEventArgs
    public class GridDataTableModelSelectionChangedCommandBehaviorWithEventArgs : GridDataTableModelSelectionChangedCommandBehavior<GridSelectionChangedEventArgs>
    {
        public GridDataTableModelSelectionChangedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataTableModelSelectionChanging
    // GridDataTableModelSelectionChangingCommand<T, TBehavior>
    public class GridDataTableModelSelectionChangingCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataTableModelSelectionChangingCommandBehavior<T>, new()
    { }

    // GridDataTableModelSelectionChangingCommandBehavior<TReturn>
    public class GridDataTableModelSelectionChangingCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, GridSelectionChangingEventArgs>
    {
        public GridDataTableModelSelectionChangingCommandBehavior(Func<object, GridSelectionChangingEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataTableModelSelectionChangingCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Model.SelectionChanging += OnEventRaised;
        }
    }

    // GridDataTableModelSelectionChangingCommand
    public class GridDataTableModelSelectionChangingCommand : GridDataControlCommandBase<GridDataTableModelSelectionChangingCommandBehavior>
    { }

    // GridDataTableModelSelectionChangingCommandBehavior
    public class GridDataTableModelSelectionChangingCommandBehavior : GridDataTableModelSelectionChangingCommandBehavior<object>
    { }

    // GridDataTableModelSelectionChangingCommandWithEventArgs	
    public class GridDataTableModelSelectionChangingCommandWithEventArgs : GridDataTableModelSelectionChangingCommand<GridSelectionChangingEventArgs, GridDataTableModelSelectionChangingCommandBehaviorWithEventArgs>
    { }

    // GridDataTableModelSelectionChangingCommandBehaviorWithEventArgs
    public class GridDataTableModelSelectionChangingCommandBehaviorWithEventArgs : GridDataTableModelSelectionChangingCommandBehavior<GridSelectionChangingEventArgs>
    {
        public GridDataTableModelSelectionChangingCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataTableModelCellRequestNavigate
    // GridDataTableModelCellRequestNavigateCommand<T, TBehavior>
    public class GridDataTableModelCellRequestNavigateCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataTableModelCellRequestNavigateCommandBehavior<T>, new()
    { }

    // GridDataTableModelCellRequestNavigateCommandBehavior<TReturn>
    public class GridDataTableModelCellRequestNavigateCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, CellRequestNavigateEventArgs>
    {
        public GridDataTableModelCellRequestNavigateCommandBehavior(Func<object, CellRequestNavigateEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataTableModelCellRequestNavigateCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Model.CellRequestNavigate += OnEventRaised;
        }
    }

    // GridDataTableModelCellRequestNavigateCommand
    public class GridDataTableModelCellRequestNavigateCommand : GridDataControlCommandBase<GridDataTableModelCellRequestNavigateCommandBehavior>
    { }

    // GridDataTableModelCellRequestNavigateCommandBehavior
    public class GridDataTableModelCellRequestNavigateCommandBehavior : GridDataTableModelCellRequestNavigateCommandBehavior<object>
    { }

    // GridDataTableModelCellRequestNavigateCommandWithEventArgs	
    public class GridDataTableModelCellRequestNavigateCommandWithEventArgs : GridDataTableModelCellRequestNavigateCommand<CellRequestNavigateEventArgs, GridDataTableModelCellRequestNavigateCommandBehaviorWithEventArgs>
    { }

    // GridDataTableModelCellRequestNavigateCommandBehaviorWithEventArgs
    public class GridDataTableModelCellRequestNavigateCommandBehaviorWithEventArgs : GridDataTableModelCellRequestNavigateCommandBehavior<CellRequestNavigateEventArgs>
    {
        public GridDataTableModelCellRequestNavigateCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataTableModelClipboardCanPaste
    // GridDataTableModelClipboardCanPasteCommand<T, TBehavior>
    public class GridDataTableModelClipboardCanPasteCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataTableModelClipboardCanPasteCommandBehavior<T>, new()
    { }

    // GridDataTableModelClipboardCanPasteCommandBehavior<TReturn>
    public class GridDataTableModelClipboardCanPasteCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, GridCutPasteEventArgs>
    {
        public GridDataTableModelClipboardCanPasteCommandBehavior(Func<object, GridCutPasteEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataTableModelClipboardCanPasteCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Model.ClipboardCanPaste += OnEventRaised;
        }
    }

    // GridDataTableModelClipboardCanPasteCommand
    public class GridDataTableModelClipboardCanPasteCommand : GridDataControlCommandBase<GridDataTableModelClipboardCanPasteCommandBehavior>
    { }

    // GridDataTableModelClipboardCanPasteCommandBehavior
    public class GridDataTableModelClipboardCanPasteCommandBehavior : GridDataTableModelClipboardCanPasteCommandBehavior<object>
    { }

    // GridDataTableModelClipboardCanPasteCommandWithEventArgs	
    public class GridDataTableModelClipboardCanPasteCommandWithEventArgs : GridDataTableModelClipboardCanPasteCommand<GridCutPasteEventArgs, GridDataTableModelClipboardCanPasteCommandBehaviorWithEventArgs>
    { }

    // GridDataTableModelClipboardCanPasteCommandBehaviorWithEventArgs
    public class GridDataTableModelClipboardCanPasteCommandBehaviorWithEventArgs : GridDataTableModelClipboardCanPasteCommandBehavior<GridCutPasteEventArgs>
    {
        public GridDataTableModelClipboardCanPasteCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataTableModelClipboardCanCopy
    // GridDataTableModelClipboardCanCopyCommand<T, TBehavior>
    public class GridDataTableModelClipboardCanCopyCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataTableModelClipboardCanCopyCommandBehavior<T>, new()
    { }

    // GridDataTableModelClipboardCanCopyCommandBehavior<TReturn>
    public class GridDataTableModelClipboardCanCopyCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, GridCutPasteEventArgs>
    {
        public GridDataTableModelClipboardCanCopyCommandBehavior(Func<object, GridCutPasteEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataTableModelClipboardCanCopyCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Model.ClipboardCanCopy += OnEventRaised;
        }
    }

    // GridDataTableModelClipboardCanCopyCommand
    public class GridDataTableModelClipboardCanCopyCommand : GridDataControlCommandBase<GridDataTableModelClipboardCanCopyCommandBehavior>
    { }

    // GridDataTableModelClipboardCanCopyCommandBehavior
    public class GridDataTableModelClipboardCanCopyCommandBehavior : GridDataTableModelClipboardCanCopyCommandBehavior<object>
    { }

    // GridDataTableModelClipboardCanCopyCommandWithEventArgs	
    public class GridDataTableModelClipboardCanCopyCommandWithEventArgs : GridDataTableModelClipboardCanCopyCommand<GridCutPasteEventArgs, GridDataTableModelClipboardCanCopyCommandBehaviorWithEventArgs>
    { }

    // GridDataTableModelClipboardCanCopyCommandBehaviorWithEventArgs
    public class GridDataTableModelClipboardCanCopyCommandBehaviorWithEventArgs : GridDataTableModelClipboardCanCopyCommandBehavior<GridCutPasteEventArgs>
    {
        public GridDataTableModelClipboardCanCopyCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataTableModelClipboardCanCut
    // GridDataTableModelClipboardCanCutCommand<T, TBehavior>
    public class GridDataTableModelClipboardCanCutCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataTableModelClipboardCanCutCommandBehavior<T>, new()
    { }

    // GridDataTableModelClipboardCanCutCommandBehavior<TReturn>
    public class GridDataTableModelClipboardCanCutCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, GridCutPasteEventArgs>
    {
        public GridDataTableModelClipboardCanCutCommandBehavior(Func<object, GridCutPasteEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataTableModelClipboardCanCutCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Model.ClipboardCanCut += OnEventRaised;
        }
    }

    // GridDataTableModelClipboardCanCutCommand
    public class GridDataTableModelClipboardCanCutCommand : GridDataControlCommandBase<GridDataTableModelClipboardCanCutCommandBehavior>
    { }

    // GridDataTableModelClipboardCanCutCommandBehavior
    public class GridDataTableModelClipboardCanCutCommandBehavior : GridDataTableModelClipboardCanCutCommandBehavior<object>
    { }

    // GridDataTableModelClipboardCanCutCommandWithEventArgs	
    public class GridDataTableModelClipboardCanCutCommandWithEventArgs : GridDataTableModelClipboardCanCutCommand<GridCutPasteEventArgs, GridDataTableModelClipboardCanCutCommandBehaviorWithEventArgs>
    { }

    // GridDataTableModelClipboardCanCutCommandBehaviorWithEventArgs
    public class GridDataTableModelClipboardCanCutCommandBehaviorWithEventArgs : GridDataTableModelClipboardCanCutCommandBehavior<GridCutPasteEventArgs>
    {
        public GridDataTableModelClipboardCanCutCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataTableModelClipboardPaste
    // GridDataTableModelClipboardPasteCommand<T, TBehavior>
    public class GridDataTableModelClipboardPasteCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataTableModelClipboardPasteCommandBehavior<T>, new()
    { }

    // GridDataTableModelClipboardPasteCommandBehavior<TReturn>
    public class GridDataTableModelClipboardPasteCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, GridCutPasteEventArgs>
    {
        public GridDataTableModelClipboardPasteCommandBehavior(Func<object, GridCutPasteEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataTableModelClipboardPasteCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Model.ClipboardPaste += OnEventRaised;
        }
    }

    // GridDataTableModelClipboardPasteCommand
    public class GridDataTableModelClipboardPasteCommand : GridDataControlCommandBase<GridDataTableModelClipboardPasteCommandBehavior>
    { }

    // GridDataTableModelClipboardPasteCommandBehavior
    public class GridDataTableModelClipboardPasteCommandBehavior : GridDataTableModelClipboardPasteCommandBehavior<object>
    { }

    // GridDataTableModelClipboardPasteCommandWithEventArgs	
    public class GridDataTableModelClipboardPasteCommandWithEventArgs : GridDataTableModelClipboardPasteCommand<GridCutPasteEventArgs, GridDataTableModelClipboardPasteCommandBehaviorWithEventArgs>
    { }

    // GridDataTableModelClipboardPasteCommandBehaviorWithEventArgs
    public class GridDataTableModelClipboardPasteCommandBehaviorWithEventArgs : GridDataTableModelClipboardPasteCommandBehavior<GridCutPasteEventArgs>
    {
        public GridDataTableModelClipboardPasteCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataTableModelClipboardPasted
    // GridDataTableModelClipboardPastedCommand<T, TBehavior>
    public class GridDataTableModelClipboardPastedCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataTableModelClipboardPastedCommandBehavior<T>, new()
    { }

    // GridDataTableModelClipboardPastedCommandBehavior<TReturn>
    public class GridDataTableModelClipboardPastedCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, GridCutPasteEventArgs>
    {
        public GridDataTableModelClipboardPastedCommandBehavior(Func<object, GridCutPasteEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataTableModelClipboardPastedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Model.ClipboardPasted += OnEventRaised;
        }
    }

    // GridDataTableModelClipboardPastedCommand
    public class GridDataTableModelClipboardPastedCommand : GridDataControlCommandBase<GridDataTableModelClipboardPastedCommandBehavior>
    { }

    // GridDataTableModelClipboardPastedCommandBehavior
    public class GridDataTableModelClipboardPastedCommandBehavior : GridDataTableModelClipboardPastedCommandBehavior<object>
    { }

    // GridDataTableModelClipboardPastedCommandWithEventArgs	
    public class GridDataTableModelClipboardPastedCommandWithEventArgs : GridDataTableModelClipboardPastedCommand<GridCutPasteEventArgs, GridDataTableModelClipboardPastedCommandBehaviorWithEventArgs>
    { }

    // GridDataTableModelClipboardPastedCommandBehaviorWithEventArgs
    public class GridDataTableModelClipboardPastedCommandBehaviorWithEventArgs : GridDataTableModelClipboardPastedCommandBehavior<GridCutPasteEventArgs>
    {
        public GridDataTableModelClipboardPastedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataTableModelClipboardCopy
    // GridDataTableModelClipboardCopyCommand<T, TBehavior>
    public class GridDataTableModelClipboardCopyCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataTableModelClipboardCopyCommandBehavior<T>, new()
    { }

    // GridDataTableModelClipboardCopyCommandBehavior<TReturn>
    public class GridDataTableModelClipboardCopyCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, GridCutPasteEventArgs>
    {
        public GridDataTableModelClipboardCopyCommandBehavior(Func<object, GridCutPasteEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataTableModelClipboardCopyCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Model.ClipboardCopy += OnEventRaised;
        }
    }

    // GridDataTableModelClipboardCopyCommand
    public class GridDataTableModelClipboardCopyCommand : GridDataControlCommandBase<GridDataTableModelClipboardCopyCommandBehavior>
    { }

    // GridDataTableModelClipboardCopyCommandBehavior
    public class GridDataTableModelClipboardCopyCommandBehavior : GridDataTableModelClipboardCopyCommandBehavior<object>
    { }

    // GridDataTableModelClipboardCopyCommandWithEventArgs	
    public class GridDataTableModelClipboardCopyCommandWithEventArgs : GridDataTableModelClipboardCopyCommand<GridCutPasteEventArgs, GridDataTableModelClipboardCopyCommandBehaviorWithEventArgs>
    { }

    // GridDataTableModelClipboardCopyCommandBehaviorWithEventArgs
    public class GridDataTableModelClipboardCopyCommandBehaviorWithEventArgs : GridDataTableModelClipboardCopyCommandBehavior<GridCutPasteEventArgs>
    {
        public GridDataTableModelClipboardCopyCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataTableModelClipboardCut
    // GridDataTableModelClipboardCutCommand<T, TBehavior>
    public class GridDataTableModelClipboardCutCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataTableModelClipboardCutCommandBehavior<T>, new()
    { }

    // GridDataTableModelClipboardCutCommandBehavior<TReturn>
    public class GridDataTableModelClipboardCutCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, GridCutPasteEventArgs>
    {
        public GridDataTableModelClipboardCutCommandBehavior(Func<object, GridCutPasteEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataTableModelClipboardCutCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Model.ClipboardCut += OnEventRaised;
        }
    }

    // GridDataTableModelClipboardCutCommand
    public class GridDataTableModelClipboardCutCommand : GridDataControlCommandBase<GridDataTableModelClipboardCutCommandBehavior>
    { }

    // GridDataTableModelClipboardCutCommandBehavior
    public class GridDataTableModelClipboardCutCommandBehavior : GridDataTableModelClipboardCutCommandBehavior<object>
    { }

    // GridDataTableModelClipboardCutCommandWithEventArgs	
    public class GridDataTableModelClipboardCutCommandWithEventArgs : GridDataTableModelClipboardCutCommand<GridCutPasteEventArgs, GridDataTableModelClipboardCutCommandBehaviorWithEventArgs>
    { }

    // GridDataTableModelClipboardCutCommandBehaviorWithEventArgs
    public class GridDataTableModelClipboardCutCommandBehaviorWithEventArgs : GridDataTableModelClipboardCutCommandBehavior<GridCutPasteEventArgs>
    {
        public GridDataTableModelClipboardCutCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
}
