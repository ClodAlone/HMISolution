#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

using Syncfusion.Windows.Controls.Gantt;
using Syncfusion.Windows.Controls.Grid;
using Syncfusion.Windows.ComponentModel;

namespace Syncfusion.Windows.Controls.Gantt.MVVM
{
    #region GanttGridNodesPopulated
    // GanttGridNodesPopulatedCommand<T, TBehavior>
    public class GanttGridNodesPopulatedCommand<T, TBehavior> : GanttControlCommandBase<TBehavior> where TBehavior : GanttGridNodesPopulatedCommandBehavior<T>, new()
    { }

    // GanttGridNodesPopulatedCommandBehavior<TReturn>
    public class GanttGridNodesPopulatedCommandBehavior<TReturn> : GanttControlCommandBehaviorBase<TReturn, EventArgs>
    {
        public GanttGridNodesPopulatedCommandBehavior(Func<object, EventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GanttGridNodesPopulatedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.GanttGrid.NodesPopulated += OnEventRaised;
        }
    }

    // GanttGridNodesPopulatedCommand
    public class GanttGridNodesPopulatedCommand : GanttControlCommandBase<GanttGridNodesPopulatedCommandBehavior>
    { }

    // GanttGridNodesPopulatedCommandBehavior
    public class GanttGridNodesPopulatedCommandBehavior : GanttGridNodesPopulatedCommandBehavior<object>
    { }

    // GanttGridNodesPopulatedCommandWithEventArgs	
    public class GanttGridNodesPopulatedCommandWithEventArgs : GanttGridNodesPopulatedCommand<EventArgs, GanttGridNodesPopulatedCommandBehaviorWithEventArgs>
    { }

    // GanttGridNodesPopulatedCommandBehaviorWithEventArgs
    public class GanttGridNodesPopulatedCommandBehaviorWithEventArgs : GanttGridNodesPopulatedCommandBehavior<EventArgs>
    {
        public GanttGridNodesPopulatedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GanttGridModelLoaded
    // GanttGridModelLoadedCommand<T, TBehavior>
    public class GanttGridModelLoadedCommand<T, TBehavior> : GanttControlCommandBase<TBehavior> where TBehavior : GanttGridModelLoadedCommandBehavior<T>, new()
    { }

    // GanttGridModelLoadedCommandBehavior<TReturn>
    public class GanttGridModelLoadedCommandBehavior<TReturn> : GanttControlCommandBehaviorBase<TReturn, EventArgs>
    {
        public GanttGridModelLoadedCommandBehavior(Func<object, EventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GanttGridModelLoadedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.GanttGrid.ModelLoaded += OnEventRaised;
        }
    }

    // GanttGridModelLoadedCommand
    public class GanttGridModelLoadedCommand : GanttControlCommandBase<GanttGridModelLoadedCommandBehavior>
    { }

    // GanttGridModelLoadedCommandBehavior
    public class GanttGridModelLoadedCommandBehavior : GanttGridModelLoadedCommandBehavior<object>
    { }

    // GanttGridModelLoadedCommandWithEventArgs	
    public class GanttGridModelLoadedCommandWithEventArgs : GanttGridModelLoadedCommand<EventArgs, GanttGridModelLoadedCommandBehaviorWithEventArgs>
    { }

    // GanttGridModelLoadedCommandBehaviorWithEventArgs
    public class GanttGridModelLoadedCommandBehaviorWithEventArgs : GanttGridModelLoadedCommandBehavior<EventArgs>
    {
        public GanttGridModelLoadedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GanttGridRequestTreeItems
    // GanttGridRequestTreeItemsCommand<T, TBehavior>
    public class GanttGridRequestTreeItemsCommand<T, TBehavior> : GanttControlCommandBase<TBehavior> where TBehavior : GanttGridRequestTreeItemsCommandBehavior<T>, new()
    { }

    // GanttGridRequestTreeItemsCommandBehavior<TReturn>
    public class GanttGridRequestTreeItemsCommandBehavior<TReturn> : GanttControlCommandBehaviorBase<TReturn, GridTreeRequestTreeItemsEventArgs>
    {
        public GanttGridRequestTreeItemsCommandBehavior(Func<object, GridTreeRequestTreeItemsEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GanttGridRequestTreeItemsCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.GanttGrid.RequestTreeItems += OnEventRaised;
        }
    }

    // GanttGridRequestTreeItemsCommand
    public class GanttGridRequestTreeItemsCommand : GanttControlCommandBase<GanttGridRequestTreeItemsCommandBehavior>
    { }

    // GanttGridRequestTreeItemsCommandBehavior
    public class GanttGridRequestTreeItemsCommandBehavior : GanttGridRequestTreeItemsCommandBehavior<object>
    { }

    // GanttGridRequestTreeItemsCommandWithEventArgs	
    public class GanttGridRequestTreeItemsCommandWithEventArgs : GanttGridRequestTreeItemsCommand<GridTreeRequestTreeItemsEventArgs, GanttGridRequestTreeItemsCommandBehaviorWithEventArgs>
    { }

    // GanttGridRequestTreeItemsCommandBehaviorWithEventArgs
    public class GanttGridRequestTreeItemsCommandBehaviorWithEventArgs : GanttGridRequestTreeItemsCommandBehavior<GridTreeRequestTreeItemsEventArgs>
    {
        public GanttGridRequestTreeItemsCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GanttGridRequestNodeImage
    // GanttGridRequestNodeImageCommand<T, TBehavior>
    public class GanttGridRequestNodeImageCommand<T, TBehavior> : GanttControlCommandBase<TBehavior> where TBehavior : GanttGridRequestNodeImageCommandBehavior<T>, new()
    { }

    // GanttGridRequestNodeImageCommandBehavior<TReturn>
    public class GanttGridRequestNodeImageCommandBehavior<TReturn> : GanttControlCommandBehaviorBase<TReturn, GridTreeRequestNodeImageEventArgs>
    {
        public GanttGridRequestNodeImageCommandBehavior(Func<object, GridTreeRequestNodeImageEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GanttGridRequestNodeImageCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.GanttGrid.RequestNodeImage += OnEventRaised;
        }
    }

    // GanttGridRequestNodeImageCommand
    public class GanttGridRequestNodeImageCommand : GanttControlCommandBase<GanttGridRequestNodeImageCommandBehavior>
    { }

    // GanttGridRequestNodeImageCommandBehavior
    public class GanttGridRequestNodeImageCommandBehavior : GanttGridRequestNodeImageCommandBehavior<object>
    { }

    // GanttGridRequestNodeImageCommandWithEventArgs	
    public class GanttGridRequestNodeImageCommandWithEventArgs : GanttGridRequestNodeImageCommand<GridTreeRequestNodeImageEventArgs, GanttGridRequestNodeImageCommandBehaviorWithEventArgs>
    { }

    // GanttGridRequestNodeImageCommandBehaviorWithEventArgs
    public class GanttGridRequestNodeImageCommandBehaviorWithEventArgs : GanttGridRequestNodeImageCommandBehavior<GridTreeRequestNodeImageEventArgs>
    {
        public GanttGridRequestNodeImageCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GanttGridCreatingTreeNode
    // GanttGridCreatingTreeNodeCommand<T, TBehavior>
    public class GanttGridCreatingTreeNodeCommand<T, TBehavior> : GanttControlCommandBase<TBehavior> where TBehavior : GanttGridCreatingTreeNodeCommandBehavior<T>, new()
    { }

    // GanttGridCreatingTreeNodeCommandBehavior<TReturn>
    public class GanttGridCreatingTreeNodeCommandBehavior<TReturn> : GanttControlCommandBehaviorBase<TReturn, GridTreeCreatingNodeEventArgs>
    {
        public GanttGridCreatingTreeNodeCommandBehavior(Func<object, GridTreeCreatingNodeEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GanttGridCreatingTreeNodeCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.GanttGrid.CreatingTreeNode += OnEventRaised;
        }
    }

    // GanttGridCreatingTreeNodeCommand
    public class GanttGridCreatingTreeNodeCommand : GanttControlCommandBase<GanttGridCreatingTreeNodeCommandBehavior>
    { }

    // GanttGridCreatingTreeNodeCommandBehavior
    public class GanttGridCreatingTreeNodeCommandBehavior : GanttGridCreatingTreeNodeCommandBehavior<object>
    { }

    // GanttGridCreatingTreeNodeCommandWithEventArgs	
    public class GanttGridCreatingTreeNodeCommandWithEventArgs : GanttGridCreatingTreeNodeCommand<GridTreeCreatingNodeEventArgs, GanttGridCreatingTreeNodeCommandBehaviorWithEventArgs>
    { }

    // GanttGridCreatingTreeNodeCommandBehaviorWithEventArgs
    public class GanttGridCreatingTreeNodeCommandBehaviorWithEventArgs : GanttGridCreatingTreeNodeCommandBehavior<GridTreeCreatingNodeEventArgs>
    {
        public GanttGridCreatingTreeNodeCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GanttGridParentPropertyNameChanged
    // GanttGridParentPropertyNameChangedCommand<T, TBehavior>
    public class GanttGridParentPropertyNameChangedCommand<T, TBehavior> : GanttControlCommandBase<TBehavior> where TBehavior : GanttGridParentPropertyNameChangedCommandBehavior<T>, new()
    { }

    // GanttGridParentPropertyNameChangedCommandBehavior<TReturn>
    public class GanttGridParentPropertyNameChangedCommandBehavior<TReturn> : GanttControlCommandBehaviorBase<TReturn, SyncfusionRoutedEventArgs>
    {
        public GanttGridParentPropertyNameChangedCommandBehavior(Func<object, SyncfusionRoutedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GanttGridParentPropertyNameChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.GanttGrid.ParentPropertyNameChanged += OnEventRaised;
        }
    }

    // GanttGridParentPropertyNameChangedCommand
    public class GanttGridParentPropertyNameChangedCommand : GanttControlCommandBase<GanttGridParentPropertyNameChangedCommandBehavior>
    { }

    // GanttGridParentPropertyNameChangedCommandBehavior
    public class GanttGridParentPropertyNameChangedCommandBehavior : GanttGridParentPropertyNameChangedCommandBehavior<object>
    { }

    // GanttGridParentPropertyNameChangedCommandWithEventArgs	
    public class GanttGridParentPropertyNameChangedCommandWithEventArgs : GanttGridParentPropertyNameChangedCommand<SyncfusionRoutedEventArgs, GanttGridParentPropertyNameChangedCommandBehaviorWithEventArgs>
    { }

    // GanttGridParentPropertyNameChangedCommandBehaviorWithEventArgs
    public class GanttGridParentPropertyNameChangedCommandBehaviorWithEventArgs : GanttGridParentPropertyNameChangedCommandBehavior<SyncfusionRoutedEventArgs>
    {
        public GanttGridParentPropertyNameChangedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GanttGridChildPropertyNameChanged
    // GanttGridChildPropertyNameChangedCommand<T, TBehavior>
    public class GanttGridChildPropertyNameChangedCommand<T, TBehavior> : GanttControlCommandBase<TBehavior> where TBehavior : GanttGridChildPropertyNameChangedCommandBehavior<T>, new()
    { }

    // GanttGridChildPropertyNameChangedCommandBehavior<TReturn>
    public class GanttGridChildPropertyNameChangedCommandBehavior<TReturn> : GanttControlCommandBehaviorBase<TReturn, SyncfusionRoutedEventArgs>
    {
        public GanttGridChildPropertyNameChangedCommandBehavior(Func<object, SyncfusionRoutedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GanttGridChildPropertyNameChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.GanttGrid.ChildPropertyNameChanged += OnEventRaised;
        }
    }

    // GanttGridChildPropertyNameChangedCommand
    public class GanttGridChildPropertyNameChangedCommand : GanttControlCommandBase<GanttGridChildPropertyNameChangedCommandBehavior>
    { }

    // GanttGridChildPropertyNameChangedCommandBehavior
    public class GanttGridChildPropertyNameChangedCommandBehavior : GanttGridChildPropertyNameChangedCommandBehavior<object>
    { }

    // GanttGridChildPropertyNameChangedCommandWithEventArgs	
    public class GanttGridChildPropertyNameChangedCommandWithEventArgs : GanttGridChildPropertyNameChangedCommand<SyncfusionRoutedEventArgs, GanttGridChildPropertyNameChangedCommandBehaviorWithEventArgs>
    { }

    // GanttGridChildPropertyNameChangedCommandBehaviorWithEventArgs
    public class GanttGridChildPropertyNameChangedCommandBehaviorWithEventArgs : GanttGridChildPropertyNameChangedCommandBehavior<SyncfusionRoutedEventArgs>
    {
        public GanttGridChildPropertyNameChangedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GanttGridItemsSourceChanged
    // GanttGridItemsSourceChangedCommand<T, TBehavior>
    public class GanttGridItemsSourceChangedCommand<T, TBehavior> : GanttControlCommandBase<TBehavior> where TBehavior : GanttGridItemsSourceChangedCommandBehavior<T>, new()
    { }

    // GanttGridItemsSourceChangedCommandBehavior<TReturn>
    public class GanttGridItemsSourceChangedCommandBehavior<TReturn> : GanttControlCommandBehaviorBase<TReturn, SyncfusionRoutedEventArgs>
    {
        public GanttGridItemsSourceChangedCommandBehavior(Func<object, SyncfusionRoutedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GanttGridItemsSourceChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.GanttGrid.ItemsSourceChanged += OnEventRaised;
        }
    }

    // GanttGridItemsSourceChangedCommand
    public class GanttGridItemsSourceChangedCommand : GanttControlCommandBase<GanttGridItemsSourceChangedCommandBehavior>
    { }

    // GanttGridItemsSourceChangedCommandBehavior
    public class GanttGridItemsSourceChangedCommandBehavior : GanttGridItemsSourceChangedCommandBehavior<object>
    { }

    // GanttGridItemsSourceChangedCommandWithEventArgs	
    public class GanttGridItemsSourceChangedCommandWithEventArgs : GanttGridItemsSourceChangedCommand<SyncfusionRoutedEventArgs, GanttGridItemsSourceChangedCommandBehaviorWithEventArgs>
    { }

    // GanttGridItemsSourceChangedCommandBehaviorWithEventArgs
    public class GanttGridItemsSourceChangedCommandBehaviorWithEventArgs : GanttGridItemsSourceChangedCommandBehavior<SyncfusionRoutedEventArgs>
    {
        public GanttGridItemsSourceChangedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GanttGridExpandStateChanging
    // GanttGridExpandStateChangingCommand<T, TBehavior>
    public class GanttGridExpandStateChangingCommand<T, TBehavior> : GanttControlCommandBase<TBehavior> where TBehavior : GanttGridExpandStateChangingCommandBehavior<T>, new()
    { }

    // GanttGridExpandStateChangingCommandBehavior<TReturn>
    public class GanttGridExpandStateChangingCommandBehavior<TReturn> : GanttControlCommandBehaviorBase<TReturn, GridTreeNodeCancelEventArgs>
    {
        public GanttGridExpandStateChangingCommandBehavior(Func<object, GridTreeNodeCancelEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GanttGridExpandStateChangingCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.GanttGrid.ExpandStateChanging += OnEventRaised;
        }
    }

    // GanttGridExpandStateChangingCommand
    public class GanttGridExpandStateChangingCommand : GanttControlCommandBase<GanttGridExpandStateChangingCommandBehavior>
    { }

    // GanttGridExpandStateChangingCommandBehavior
    public class GanttGridExpandStateChangingCommandBehavior : GanttGridExpandStateChangingCommandBehavior<object>
    { }

    // GanttGridExpandStateChangingCommandWithEventArgs	
    public class GanttGridExpandStateChangingCommandWithEventArgs : GanttGridExpandStateChangingCommand<GridTreeNodeCancelEventArgs, GanttGridExpandStateChangingCommandBehaviorWithEventArgs>
    { }

    // GanttGridExpandStateChangingCommandBehaviorWithEventArgs
    public class GanttGridExpandStateChangingCommandBehaviorWithEventArgs : GanttGridExpandStateChangingCommandBehavior<GridTreeNodeCancelEventArgs>
    {
        public GanttGridExpandStateChangingCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GanttGridExpandStateChanged
    // GanttGridExpandStateChangedCommand<T, TBehavior>
    public class GanttGridExpandStateChangedCommand<T, TBehavior> : GanttControlCommandBase<TBehavior> where TBehavior : GanttGridExpandStateChangedCommandBehavior<T>, new()
    { }

    // GanttGridExpandStateChangedCommandBehavior<TReturn>
    public class GanttGridExpandStateChangedCommandBehavior<TReturn> : GanttControlCommandBehaviorBase<TReturn, GridTreeNodeEventArgs>
    {
        public GanttGridExpandStateChangedCommandBehavior(Func<object, GridTreeNodeEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GanttGridExpandStateChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.GanttGrid.ExpandStateChanged += OnEventRaised;
        }
    }

    // GanttGridExpandStateChangedCommand
    public class GanttGridExpandStateChangedCommand : GanttControlCommandBase<GanttGridExpandStateChangedCommandBehavior>
    { }

    // GanttGridExpandStateChangedCommandBehavior
    public class GanttGridExpandStateChangedCommandBehavior : GanttGridExpandStateChangedCommandBehavior<object>
    { }

    // GanttGridExpandStateChangedCommandWithEventArgs	
    public class GanttGridExpandStateChangedCommandWithEventArgs : GanttGridExpandStateChangedCommand<GridTreeNodeEventArgs, GanttGridExpandStateChangedCommandBehaviorWithEventArgs>
    { }

    // GanttGridExpandStateChangedCommandBehaviorWithEventArgs
    public class GanttGridExpandStateChangedCommandBehaviorWithEventArgs : GanttGridExpandStateChangedCommandBehavior<GridTreeNodeEventArgs>
    {
        public GanttGridExpandStateChangedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
}
