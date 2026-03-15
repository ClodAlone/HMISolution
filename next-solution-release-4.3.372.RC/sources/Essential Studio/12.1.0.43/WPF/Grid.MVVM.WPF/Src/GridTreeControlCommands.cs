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
using Syncfusion.Windows.Controls.Grid;
using Syncfusion.Windows.ComponentModel;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Controls.Grid.MVVM
{

     public class GridTreeControlCommandBase<TBehavior> : ControlCommandBase<TBehavior, GridTreeControl> where TBehavior : CommandBehaviorBase<GridTreeControl>, new()
    { }

    public class GridTreeControlCommandBehaviorBase<TReturn, TEventArgs> : BuilderCommandBehaviorBase<GridTreeControl, TEventArgs, TReturn>
    { }
    #region GridTreeControlNodesPopulatedCommand

    public class GridTreeControlNodesPopulatedCommand<T, TBehavior> : GridTreeControlCommandBase<TBehavior> where TBehavior : GridTreeControlNodesPopulatedCommandBehavior<T>, new()
    { }

    public class GridTreeControlNodesPopulatedCommandBehavior<TReturn> : GridTreeControlCommandBehaviorBase<TReturn, EventArgs>
    {
        public GridTreeControlNodesPopulatedCommandBehavior(Func<object, EventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridTreeControlNodesPopulatedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.NodesPopulated += OnEventRaised;
        }
    }

    // GridTreeControlNodesPopulatedCommand
    public class GridTreeControlNodesPopulatedCommand : GridTreeControlCommandBase<GridTreeControlNodesPopulatedCommandBehavior>
    { }

    // GridTreeControlNodesPopulatedCommandBehavior
    public class GridTreeControlNodesPopulatedCommandBehavior : GridTreeControlNodesPopulatedCommandBehavior<object>
    { }

    // GridTreeControlNodesPopulatedCommandWithEventArgs	
    public class GridTreeControlNodesPopulatedCommandWithEventArgs : GridTreeControlNodesPopulatedCommand<EventArgs, GridTreeControlNodesPopulatedCommandBehaviorWithEventArgs>
    { }

    // GridTreeControlNodesPopulatedCommandBehaviorWithEventArgs
    public class GridTreeControlNodesPopulatedCommandBehaviorWithEventArgs : GridTreeControlNodesPopulatedCommandBehavior<EventArgs>
    {
        public GridTreeControlNodesPopulatedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }

    #endregion

    #region GridTreeControlModelLoadedCommand

    public class GridTreeControlModelLoadedCommand<T, TBehavior> : GridTreeControlCommandBase<TBehavior> where TBehavior : GridTreeControlModelLoadedCommandBehavior<T>, new()
    { }

    public class GridTreeControlModelLoadedCommandBehavior<TReturn> : GridTreeControlCommandBehaviorBase<TReturn, EventArgs>
    {
        public GridTreeControlModelLoadedCommandBehavior(Func<object, EventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridTreeControlModelLoadedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ModelLoaded += OnEventRaised;
        }
    }

    // GridTreeControlModelLoadedCommand
    public class GridTreeControlModelLoadedCommand : GridTreeControlCommandBase<GridTreeControlModelLoadedCommandBehavior>
    { }

    // GridTreeControlModelLoadedCommandBehavior
    public class GridTreeControlModelLoadedCommandBehavior : GridTreeControlModelLoadedCommandBehavior<object>
    { }

    // GridTreeControlModelLoadedCommandWithEventArgs	
    public class GridTreeControlModelLoadedCommandWithEventArgs : GridTreeControlModelLoadedCommand<EventArgs, GridTreeControlModelLoadedCommandBehaviorWithEventArgs>
    { }

    // GridTreeControlModelLoadedCommandBehaviorWithEventArgs
    public class GridTreeControlModelLoadedCommandBehaviorWithEventArgs : GridTreeControlModelLoadedCommandBehavior<EventArgs>
    {
        public GridTreeControlModelLoadedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }

    #endregion

    #region GridTreeControlRequestTreeItemsCommand

    public class GridTreeControlRequestTreeItemsCommand<T, TBehavior> : GridTreeControlCommandBase<TBehavior> where TBehavior : GridTreeControlRequestTreeItemsCommandBehavior<T>, new()
    { }

    public class GridTreeControlRequestTreeItemsCommandBehavior<TReturn> : GridTreeControlCommandBehaviorBase<TReturn, GridTreeRequestTreeItemsEventArgs>
    {
        public GridTreeControlRequestTreeItemsCommandBehavior(Func<object, GridTreeRequestTreeItemsEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridTreeControlRequestTreeItemsCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.RequestTreeItems += OnEventRaised;
        }
    }

    // GridTreeControlRequestTreeItemsCommand
    public class GridTreeControlRequestTreeItemsCommand : GridTreeControlCommandBase<GridTreeControlRequestTreeItemsCommandBehavior>
    { }

    // GridTreeControlRequestTreeItemsCommandBehavior
    public class GridTreeControlRequestTreeItemsCommandBehavior : GridTreeControlRequestTreeItemsCommandBehavior<object>
    { }

    // GridTreeControlRequestTreeItemsCommandWithEventArgs	
    public class GridTreeControlRequestTreeItemsCommandWithEventArgs : GridTreeControlRequestTreeItemsCommand<GridTreeRequestTreeItemsEventArgs, GridTreeControlRequestTreeItemsCommandBehaviorWithEventArgs>
    { }

    // GridTreeControlRequestTreeItemsCommandBehaviorWithEventArgs
    public class GridTreeControlRequestTreeItemsCommandBehaviorWithEventArgs : GridTreeControlRequestTreeItemsCommandBehavior<GridTreeRequestTreeItemsEventArgs>
    {
        public GridTreeControlRequestTreeItemsCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }

    #endregion

    #region GridTreeControlRequestNodeImageCommand

    public class GridTreeControlRequestNodeImageCommand<T, TBehavior> : GridTreeControlCommandBase<TBehavior> where TBehavior : GridTreeControlRequestNodeImageCommandBehavior<T>, new()
    { }

    public class GridTreeControlRequestNodeImageCommandBehavior<TReturn> : GridTreeControlCommandBehaviorBase<TReturn, GridTreeRequestNodeImageEventArgs>
    {
        public GridTreeControlRequestNodeImageCommandBehavior(Func<object, GridTreeRequestNodeImageEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridTreeControlRequestNodeImageCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.RequestNodeImage += OnEventRaised;
        }
    }

    // GridTreeControlRequestNodeImageCommand
    public class GridTreeControlRequestNodeImageCommand : GridTreeControlCommandBase<GridTreeControlRequestNodeImageCommandBehavior>
    { }

    // GridTreeControlRequestNodeImageCommandBehavior
    public class GridTreeControlRequestNodeImageCommandBehavior : GridTreeControlRequestNodeImageCommandBehavior<object>
    { }

    // GridTreeControlRequestNodeImageCommandWithEventArgs	
    public class GridTreeControlRequestNodeImageCommandWithEventArgs : GridTreeControlRequestNodeImageCommand<GridTreeRequestNodeImageEventArgs, GridTreeControlRequestNodeImageCommandBehaviorWithEventArgs>
    { }

    // GridTreeControlRequestNodeImageCommandBehaviorWithEventArgs
    public class GridTreeControlRequestNodeImageCommandBehaviorWithEventArgs : GridTreeControlRequestNodeImageCommandBehavior<GridTreeRequestNodeImageEventArgs>
    {
        public GridTreeControlRequestNodeImageCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }

    #endregion

    #region GridTreeControlCreatingTreeNodeCommand

    public class GridTreeControlCreatingTreeNodeCommand<T, TBehavior> : GridTreeControlCommandBase<TBehavior> where TBehavior : GridTreeControlCreatingTreeNodeCommandBehavior<T>, new()
    { }

    public class GridTreeControlCreatingTreeNodeCommandBehavior<TReturn> : GridTreeControlCommandBehaviorBase<TReturn, GridTreeCreatingNodeEventArgs>
    {
        public GridTreeControlCreatingTreeNodeCommandBehavior(Func<object, GridTreeCreatingNodeEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridTreeControlCreatingTreeNodeCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.CreatingTreeNode += OnEventRaised;
        }
    }

    // GridTreeControlCreatingTreeNodeCommand
    public class GridTreeControlCreatingTreeNodeCommand : GridTreeControlCommandBase<GridTreeControlCreatingTreeNodeCommandBehavior>
    { }

    // GridTreeControlCreatingTreeNodeCommandBehavior
    public class GridTreeControlCreatingTreeNodeCommandBehavior : GridTreeControlCreatingTreeNodeCommandBehavior<object>
    { }

    // GridTreeControlCreatingTreeNodeCommandWithEventArgs	
    public class GridTreeControlCreatingTreeNodeCommandWithEventArgs : GridTreeControlCreatingTreeNodeCommand<GridTreeCreatingNodeEventArgs, GridTreeControlCreatingTreeNodeCommandBehaviorWithEventArgs>
    { }

    // GridTreeControlCreatingTreeNodeCommandBehaviorWithEventArgs
    public class GridTreeControlCreatingTreeNodeCommandBehaviorWithEventArgs : GridTreeControlCreatingTreeNodeCommandBehavior<GridTreeCreatingNodeEventArgs>
    {
        public GridTreeControlCreatingTreeNodeCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }

    #endregion

    #region GridTreeControlParentPropertyNameChangedCommand

    public class GridTreeControlParentPropertyNameChangedCommand<T, TBehavior> : GridTreeControlCommandBase<TBehavior> where TBehavior : GridTreeControlParentPropertyNameChangedCommandBehavior<T>, new()
    { }

    public class GridTreeControlParentPropertyNameChangedCommandBehavior<TReturn> : GridTreeControlCommandBehaviorBase<TReturn, SyncfusionRoutedEventArgs>
    {
        public GridTreeControlParentPropertyNameChangedCommandBehavior(Func<object, SyncfusionRoutedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridTreeControlParentPropertyNameChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ParentPropertyNameChanged += OnEventRaised;
        }
    }

    // GridTreeControlParentPropertyNameChangedCommand
    public class GridTreeControlParentPropertyNameChangedCommand : GridTreeControlCommandBase<GridTreeControlParentPropertyNameChangedCommandBehavior>
    { }

    // GridTreeControlParentPropertyNameChangedCommandBehavior
    public class GridTreeControlParentPropertyNameChangedCommandBehavior : GridTreeControlParentPropertyNameChangedCommandBehavior<object>
    { }

    // GridTreeControlParentPropertyNameChangedCommandWithEventArgs	
    public class GridTreeControlParentPropertyNameChangedCommandWithEventArgs : GridTreeControlParentPropertyNameChangedCommand<SyncfusionRoutedEventArgs, GridTreeControlParentPropertyNameChangedCommandBehaviorWithEventArgs>
    { }

    // GridTreeControlParentPropertyNameChangedCommandBehaviorWithEventArgs
    public class GridTreeControlParentPropertyNameChangedCommandBehaviorWithEventArgs : GridTreeControlParentPropertyNameChangedCommandBehavior<SyncfusionRoutedEventArgs>
    {
        public GridTreeControlParentPropertyNameChangedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }

    #endregion

    #region GridTreeControlChildPropertyNameChangedCommand

    public class GridTreeControlChildPropertyNameChangedCommand<T, TBehavior> : GridTreeControlCommandBase<TBehavior> where TBehavior : GridTreeControlChildPropertyNameChangedCommandBehavior<T>, new()
    { }

    public class GridTreeControlChildPropertyNameChangedCommandBehavior<TReturn> : GridTreeControlCommandBehaviorBase<TReturn, SyncfusionRoutedEventArgs>
    {
        public GridTreeControlChildPropertyNameChangedCommandBehavior(Func<object, SyncfusionRoutedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridTreeControlChildPropertyNameChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ChildPropertyNameChanged += OnEventRaised;
        }
    }

    // GridTreeControlChildPropertyNameChangedCommand
    public class GridTreeControlChildPropertyNameChangedCommand : GridTreeControlCommandBase<GridTreeControlChildPropertyNameChangedCommandBehavior>
    { }

    // GridTreeControlChildPropertyNameChangedCommandBehavior
    public class GridTreeControlChildPropertyNameChangedCommandBehavior : GridTreeControlChildPropertyNameChangedCommandBehavior<object>
    { }

    // GridTreeControlChildPropertyNameChangedCommandWithEventArgs	
    public class GridTreeControlChildPropertyNameChangedCommandWithEventArgs : GridTreeControlChildPropertyNameChangedCommand<SyncfusionRoutedEventArgs, GridTreeControlChildPropertyNameChangedCommandBehaviorWithEventArgs>
    { }

    // GridTreeControlChildPropertyNameChangedCommandBehaviorWithEventArgs
    public class GridTreeControlChildPropertyNameChangedCommandBehaviorWithEventArgs : GridTreeControlChildPropertyNameChangedCommandBehavior<SyncfusionRoutedEventArgs>
    {
        public GridTreeControlChildPropertyNameChangedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }

    #endregion

    #region GridTreeControlItemsSourceChangedCommand

    public class GridTreeControlItemsSourceChangedCommand<T, TBehavior> : GridTreeControlCommandBase<TBehavior> where TBehavior : GridTreeControlItemsSourceChangedCommandBehavior<T>, new()
    { }

    public class GridTreeControlItemsSourceChangedCommandBehavior<TReturn> : GridTreeControlCommandBehaviorBase<TReturn, SyncfusionRoutedEventArgs>
    {
        public GridTreeControlItemsSourceChangedCommandBehavior(Func<object, SyncfusionRoutedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridTreeControlItemsSourceChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ItemsSourceChanged += OnEventRaised;
        }
    }

    // GridTreeControlItemsSourceChangedCommand
    public class GridTreeControlItemsSourceChangedCommand : GridTreeControlCommandBase<GridTreeControlItemsSourceChangedCommandBehavior>
    { }

    // GridTreeControlItemsSourceChangedCommandBehavior
    public class GridTreeControlItemsSourceChangedCommandBehavior : GridTreeControlItemsSourceChangedCommandBehavior<object>
    { }

    // GridTreeControlItemsSourceChangedCommandWithEventArgs	
    public class GridTreeControlItemsSourceChangedCommandWithEventArgs : GridTreeControlItemsSourceChangedCommand<SyncfusionRoutedEventArgs, GridTreeControlItemsSourceChangedCommandBehaviorWithEventArgs>
    { }

    // GridTreeControlItemsSourceChangedCommandBehaviorWithEventArgs
    public class GridTreeControlItemsSourceChangedCommandBehaviorWithEventArgs : GridTreeControlItemsSourceChangedCommandBehavior<SyncfusionRoutedEventArgs>
    {
        public GridTreeControlItemsSourceChangedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }

    #endregion

    #region GridTreeControlExpandStateChangingCommand

    public class GridTreeControlExpandStateChangingCommand<T, TBehavior> : GridTreeControlCommandBase<TBehavior> where TBehavior : GridTreeControlExpandStateChangingCommandBehavior<T>, new()
    { }

    public class GridTreeControlExpandStateChangingCommandBehavior<TReturn> : GridTreeControlCommandBehaviorBase<TReturn, GridTreeNodeCancelEventArgs>
    {
        public GridTreeControlExpandStateChangingCommandBehavior(Func<object, GridTreeNodeCancelEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridTreeControlExpandStateChangingCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ExpandStateChanging += OnEventRaised;
        }
    }

    // GridTreeControlExpandStateChangingCommand
    public class GridTreeControlExpandStateChangingCommand : GridTreeControlCommandBase<GridTreeControlExpandStateChangingCommandBehavior>
    { }

    // GridTreeControlExpandStateChangingCommandBehavior
    public class GridTreeControlExpandStateChangingCommandBehavior : GridTreeControlExpandStateChangingCommandBehavior<object>
    { }

    // GridTreeControlExpandStateChangingCommandWithEventArgs	
    public class GridTreeControlExpandStateChangingCommandWithEventArgs : GridTreeControlExpandStateChangingCommand<GridTreeNodeCancelEventArgs, GridTreeControlExpandStateChangingCommandBehaviorWithEventArgs>
    { }

    // GridTreeControlExpandStateChangingCommandBehaviorWithEventArgs
    public class GridTreeControlExpandStateChangingCommandBehaviorWithEventArgs : GridTreeControlExpandStateChangingCommandBehavior<GridTreeNodeCancelEventArgs>
    {
        public GridTreeControlExpandStateChangingCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }

    #endregion

    #region GridTreeControlExpandStateChangedCommand

    public class GridTreeControlExpandStateChangedCommand<T, TBehavior> : GridTreeControlCommandBase<TBehavior> where TBehavior : GridTreeControlExpandStateChangedCommandBehavior<T>, new()
    { }

    public class GridTreeControlExpandStateChangedCommandBehavior<TReturn> : GridTreeControlCommandBehaviorBase<TReturn, GridTreeNodeEventArgs>
    {
        public GridTreeControlExpandStateChangedCommandBehavior(Func<object, GridTreeNodeEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridTreeControlExpandStateChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ExpandStateChanged += OnEventRaised;
        }
    }

    // GridTreeControlExpandStateChangedCommand
    public class GridTreeControlExpandStateChangedCommand : GridTreeControlCommandBase<GridTreeControlExpandStateChangedCommandBehavior>
    { }

    // GridTreeControlExpandStateChangedCommandBehavior
    public class GridTreeControlExpandStateChangedCommandBehavior : GridTreeControlExpandStateChangedCommandBehavior<object>
    { }

    // GridTreeControlExpandStateChangedCommandWithEventArgs	
    public class GridTreeControlExpandStateChangedCommandWithEventArgs : GridTreeControlExpandStateChangedCommand<GridTreeNodeEventArgs, GridTreeControlExpandStateChangedCommandBehaviorWithEventArgs>
    { }

    // GridTreeControlExpandStateChangedCommandBehaviorWithEventArgs
    public class GridTreeControlExpandStateChangedCommandBehaviorWithEventArgs : GridTreeControlExpandStateChangedCommandBehavior<GridTreeNodeEventArgs>
    {
        public GridTreeControlExpandStateChangedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }

    #endregion

}
