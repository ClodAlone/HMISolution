#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// Generated at 4/30/2012 8:10:20 PM 
#endregion
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Syncfusion.Windows.ComponentModel;
using Syncfusion.Windows.Controls.Grid;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Controls.Grid.MVVM
{
    #region Command Base

    public class GridDataControlCommandBase<TBehavior> : ControlCommandBase<TBehavior, GridDataControl> where TBehavior : CommandBehaviorBase<GridDataControl>, new()
    { }

    public class GridDataControlCommandBehaviorBase<TReturn, TEventArgs> : BuilderCommandBehaviorBase<GridDataControl, TEventArgs, TReturn>
    { }

    #endregion

    #region GridDataControlModelLoaded
    // GridDataControlModelLoadedCommand<T, TBehavior>
    public class GridDataControlModelLoadedCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataControlModelLoadedCommandBehavior<T>, new()
    { }

    // GridDataControlModelLoadedCommandBehavior<TReturn>
    public class GridDataControlModelLoadedCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, EventArgs>
    {
        public GridDataControlModelLoadedCommandBehavior(Func<object, EventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataControlModelLoadedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ModelLoaded += OnEventRaised;
        }
    }

    // GridDataControlModelLoadedCommand
    public class GridDataControlModelLoadedCommand : GridDataControlCommandBase<GridDataControlModelLoadedCommandBehavior>
    { }

    // GridDataControlModelLoadedCommandBehavior
    public class GridDataControlModelLoadedCommandBehavior : GridDataControlModelLoadedCommandBehavior<object>
    { }

    // GridDataControlModelLoadedCommandWithEventArgs	
    public class GridDataControlModelLoadedCommandWithEventArgs : GridDataControlModelLoadedCommand<EventArgs, GridDataControlModelLoadedCommandBehaviorWithEventArgs>
    { }

    // GridDataControlModelLoadedCommandBehaviorWithEventArgs
    public class GridDataControlModelLoadedCommandBehaviorWithEventArgs : GridDataControlModelLoadedCommandBehavior<EventArgs>
    {
        public GridDataControlModelLoadedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataControlItemsSourceChanged
    // GridDataControlItemsSourceChangedCommand<T, TBehavior>
    public class GridDataControlItemsSourceChangedCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataControlItemsSourceChangedCommandBehavior<T>, new()
    { }

    // GridDataControlItemsSourceChangedCommandBehavior<TReturn>
    public class GridDataControlItemsSourceChangedCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, SyncfusionRoutedEventArgs>
    {
        public GridDataControlItemsSourceChangedCommandBehavior(Func<object, SyncfusionRoutedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataControlItemsSourceChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ItemsSourceChanged += OnEventRaised;
        }
    }

    // GridDataControlItemsSourceChangedCommand
    public class GridDataControlItemsSourceChangedCommand : GridDataControlCommandBase<GridDataControlItemsSourceChangedCommandBehavior>
    { }

    // GridDataControlItemsSourceChangedCommandBehavior
    public class GridDataControlItemsSourceChangedCommandBehavior : GridDataControlItemsSourceChangedCommandBehavior<object>
    { }

    // GridDataControlItemsSourceChangedCommandWithEventArgs	
    public class GridDataControlItemsSourceChangedCommandWithEventArgs : GridDataControlItemsSourceChangedCommand<SyncfusionRoutedEventArgs, GridDataControlItemsSourceChangedCommandBehaviorWithEventArgs>
    { }

    // GridDataControlItemsSourceChangedCommandBehaviorWithEventArgs
    public class GridDataControlItemsSourceChangedCommandBehaviorWithEventArgs : GridDataControlItemsSourceChangedCommandBehavior<SyncfusionRoutedEventArgs>
    {
        public GridDataControlItemsSourceChangedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

#if !SILVERLIGHT
    #region GridDataControlCurrentCellActivating
    // GridDataControlCurrentCellActivatingCommand<T, TBehavior>
    public class GridDataControlCurrentCellActivatingCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataControlCurrentCellActivatingCommandBehavior<T>, new()
    { }

    // GridDataControlCurrentCellActivatingCommandBehavior<TReturn>
    public class GridDataControlCurrentCellActivatingCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, GridCurrentCellActivatingEventArgs>
    {
        public GridDataControlCurrentCellActivatingCommandBehavior(Func<object, GridCurrentCellActivatingEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataControlCurrentCellActivatingCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.CurrentCellActivating += OnEventRaised;
        }
    }

    // GridDataControlCurrentCellActivatingCommand
    public class GridDataControlCurrentCellActivatingCommand : GridDataControlCommandBase<GridDataControlCurrentCellActivatingCommandBehavior>
    { }

    // GridDataControlCurrentCellActivatingCommandBehavior
    public class GridDataControlCurrentCellActivatingCommandBehavior : GridDataControlCurrentCellActivatingCommandBehavior<object>
    { }

    // GridDataControlCurrentCellActivatingCommandWithEventArgs	
    public class GridDataControlCurrentCellActivatingCommandWithEventArgs : GridDataControlCurrentCellActivatingCommand<GridCurrentCellActivatingEventArgs, GridDataControlCurrentCellActivatingCommandBehaviorWithEventArgs>
    { }

    // GridDataControlCurrentCellActivatingCommandBehaviorWithEventArgs
    public class GridDataControlCurrentCellActivatingCommandBehaviorWithEventArgs : GridDataControlCurrentCellActivatingCommandBehavior<GridCurrentCellActivatingEventArgs>
    {
        public GridDataControlCurrentCellActivatingCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataControlCurrentCellActivated
    // GridDataControlCurrentCellActivatedCommand<T, TBehavior>
    public class GridDataControlCurrentCellActivatedCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataControlCurrentCellActivatedCommandBehavior<T>, new()
    { }

    // GridDataControlCurrentCellActivatedCommandBehavior<TReturn>
    public class GridDataControlCurrentCellActivatedCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, SyncfusionRoutedEventArgs>
    {
        public GridDataControlCurrentCellActivatedCommandBehavior(Func<object, SyncfusionRoutedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataControlCurrentCellActivatedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.CurrentCellActivated += OnEventRaised;
        }
    }

    // GridDataControlCurrentCellActivatedCommand
    public class GridDataControlCurrentCellActivatedCommand : GridDataControlCommandBase<GridDataControlCurrentCellActivatedCommandBehavior>
    { }

    // GridDataControlCurrentCellActivatedCommandBehavior
    public class GridDataControlCurrentCellActivatedCommandBehavior : GridDataControlCurrentCellActivatedCommandBehavior<object>
    { }

    // GridDataControlCurrentCellActivatedCommandWithEventArgs	
    public class GridDataControlCurrentCellActivatedCommandWithEventArgs : GridDataControlCurrentCellActivatedCommand<SyncfusionRoutedEventArgs, GridDataControlCurrentCellActivatedCommandBehaviorWithEventArgs>
    { }

    // GridDataControlCurrentCellActivatedCommandBehaviorWithEventArgs
    public class GridDataControlCurrentCellActivatedCommandBehaviorWithEventArgs : GridDataControlCurrentCellActivatedCommandBehavior<SyncfusionRoutedEventArgs>
    {
        public GridDataControlCurrentCellActivatedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataControlCurrentCellActivateFailed
    // GridDataControlCurrentCellActivateFailedCommand<T, TBehavior>
    public class GridDataControlCurrentCellActivateFailedCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataControlCurrentCellActivateFailedCommandBehavior<T>, new()
    { }

    // GridDataControlCurrentCellActivateFailedCommandBehavior<TReturn>
    public class GridDataControlCurrentCellActivateFailedCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, GridCurrentCellActivateFailedEventArgs>
    {
        public GridDataControlCurrentCellActivateFailedCommandBehavior(Func<object, GridCurrentCellActivateFailedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataControlCurrentCellActivateFailedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.CurrentCellActivateFailed += OnEventRaised;
        }
    }

    // GridDataControlCurrentCellActivateFailedCommand
    public class GridDataControlCurrentCellActivateFailedCommand : GridDataControlCommandBase<GridDataControlCurrentCellActivateFailedCommandBehavior>
    { }

    // GridDataControlCurrentCellActivateFailedCommandBehavior
    public class GridDataControlCurrentCellActivateFailedCommandBehavior : GridDataControlCurrentCellActivateFailedCommandBehavior<object>
    { }

    // GridDataControlCurrentCellActivateFailedCommandWithEventArgs	
    public class GridDataControlCurrentCellActivateFailedCommandWithEventArgs : GridDataControlCurrentCellActivateFailedCommand<GridCurrentCellActivateFailedEventArgs, GridDataControlCurrentCellActivateFailedCommandBehaviorWithEventArgs>
    { }

    // GridDataControlCurrentCellActivateFailedCommandBehaviorWithEventArgs
    public class GridDataControlCurrentCellActivateFailedCommandBehaviorWithEventArgs : GridDataControlCurrentCellActivateFailedCommandBehavior<GridCurrentCellActivateFailedEventArgs>
    {
        public GridDataControlCurrentCellActivateFailedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataControlCurrentCellDeactivating
    // GridDataControlCurrentCellDeactivatingCommand<T, TBehavior>
    public class GridDataControlCurrentCellDeactivatingCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataControlCurrentCellDeactivatingCommandBehavior<T>, new()
    { }

    // GridDataControlCurrentCellDeactivatingCommandBehavior<TReturn>
    public class GridDataControlCurrentCellDeactivatingCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, SyncfusionCancelRoutedEventArgs>
    {
        public GridDataControlCurrentCellDeactivatingCommandBehavior(Func<object, SyncfusionCancelRoutedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataControlCurrentCellDeactivatingCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.CurrentCellDeactivating += OnEventRaised;
        }
    }

    // GridDataControlCurrentCellDeactivatingCommand
    public class GridDataControlCurrentCellDeactivatingCommand : GridDataControlCommandBase<GridDataControlCurrentCellDeactivatingCommandBehavior>
    { }

    // GridDataControlCurrentCellDeactivatingCommandBehavior
    public class GridDataControlCurrentCellDeactivatingCommandBehavior : GridDataControlCurrentCellDeactivatingCommandBehavior<object>
    { }

    // GridDataControlCurrentCellDeactivatingCommandWithEventArgs	
    public class GridDataControlCurrentCellDeactivatingCommandWithEventArgs : GridDataControlCurrentCellDeactivatingCommand<SyncfusionCancelRoutedEventArgs, GridDataControlCurrentCellDeactivatingCommandBehaviorWithEventArgs>
    { }

    // GridDataControlCurrentCellDeactivatingCommandBehaviorWithEventArgs
    public class GridDataControlCurrentCellDeactivatingCommandBehaviorWithEventArgs : GridDataControlCurrentCellDeactivatingCommandBehavior<SyncfusionCancelRoutedEventArgs>
    {
        public GridDataControlCurrentCellDeactivatingCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataControlCurrentCellDeactivated
    // GridDataControlCurrentCellDeactivatedCommand<T, TBehavior>
    public class GridDataControlCurrentCellDeactivatedCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataControlCurrentCellDeactivatedCommandBehavior<T>, new()
    { }

    // GridDataControlCurrentCellDeactivatedCommandBehavior<TReturn>
    public class GridDataControlCurrentCellDeactivatedCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, GridCurrentCellDeactivatedEventArgs>
    {
        public GridDataControlCurrentCellDeactivatedCommandBehavior(Func<object, GridCurrentCellDeactivatedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataControlCurrentCellDeactivatedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.CurrentCellDeactivated += OnEventRaised;
        }
    }

    // GridDataControlCurrentCellDeactivatedCommand
    public class GridDataControlCurrentCellDeactivatedCommand : GridDataControlCommandBase<GridDataControlCurrentCellDeactivatedCommandBehavior>
    { }

    // GridDataControlCurrentCellDeactivatedCommandBehavior
    public class GridDataControlCurrentCellDeactivatedCommandBehavior : GridDataControlCurrentCellDeactivatedCommandBehavior<object>
    { }

    // GridDataControlCurrentCellDeactivatedCommandWithEventArgs	
    public class GridDataControlCurrentCellDeactivatedCommandWithEventArgs : GridDataControlCurrentCellDeactivatedCommand<GridCurrentCellDeactivatedEventArgs, GridDataControlCurrentCellDeactivatedCommandBehaviorWithEventArgs>
    { }

    // GridDataControlCurrentCellDeactivatedCommandBehaviorWithEventArgs
    public class GridDataControlCurrentCellDeactivatedCommandBehaviorWithEventArgs : GridDataControlCurrentCellDeactivatedCommandBehavior<GridCurrentCellDeactivatedEventArgs>
    {
        public GridDataControlCurrentCellDeactivatedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataControlCurrentCellDeactivateFailed
    // GridDataControlCurrentCellDeactivateFailedCommand<T, TBehavior>
    public class GridDataControlCurrentCellDeactivateFailedCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataControlCurrentCellDeactivateFailedCommandBehavior<T>, new()
    { }

    // GridDataControlCurrentCellDeactivateFailedCommandBehavior<TReturn>
    public class GridDataControlCurrentCellDeactivateFailedCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, SyncfusionRoutedEventArgs>
    {
        public GridDataControlCurrentCellDeactivateFailedCommandBehavior(Func<object, SyncfusionRoutedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataControlCurrentCellDeactivateFailedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.CurrentCellDeactivateFailed += OnEventRaised;
        }
    }

    // GridDataControlCurrentCellDeactivateFailedCommand
    public class GridDataControlCurrentCellDeactivateFailedCommand : GridDataControlCommandBase<GridDataControlCurrentCellDeactivateFailedCommandBehavior>
    { }

    // GridDataControlCurrentCellDeactivateFailedCommandBehavior
    public class GridDataControlCurrentCellDeactivateFailedCommandBehavior : GridDataControlCurrentCellDeactivateFailedCommandBehavior<object>
    { }

    // GridDataControlCurrentCellDeactivateFailedCommandWithEventArgs	
    public class GridDataControlCurrentCellDeactivateFailedCommandWithEventArgs : GridDataControlCurrentCellDeactivateFailedCommand<SyncfusionRoutedEventArgs, GridDataControlCurrentCellDeactivateFailedCommandBehaviorWithEventArgs>
    { }

    // GridDataControlCurrentCellDeactivateFailedCommandBehaviorWithEventArgs
    public class GridDataControlCurrentCellDeactivateFailedCommandBehaviorWithEventArgs : GridDataControlCurrentCellDeactivateFailedCommandBehavior<SyncfusionRoutedEventArgs>
    {
        public GridDataControlCurrentCellDeactivateFailedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataControlCurrentCellConfirmChangesFailed
    // GridDataControlCurrentCellConfirmChangesFailedCommand<T, TBehavior>
    public class GridDataControlCurrentCellConfirmChangesFailedCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataControlCurrentCellConfirmChangesFailedCommandBehavior<T>, new()
    { }

    // GridDataControlCurrentCellConfirmChangesFailedCommandBehavior<TReturn>
    public class GridDataControlCurrentCellConfirmChangesFailedCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, SyncfusionRoutedEventArgs>
    {
        public GridDataControlCurrentCellConfirmChangesFailedCommandBehavior(Func<object, SyncfusionRoutedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataControlCurrentCellConfirmChangesFailedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.CurrentCellConfirmChangesFailed += OnEventRaised;
        }
    }

    // GridDataControlCurrentCellConfirmChangesFailedCommand
    public class GridDataControlCurrentCellConfirmChangesFailedCommand : GridDataControlCommandBase<GridDataControlCurrentCellConfirmChangesFailedCommandBehavior>
    { }

    // GridDataControlCurrentCellConfirmChangesFailedCommandBehavior
    public class GridDataControlCurrentCellConfirmChangesFailedCommandBehavior : GridDataControlCurrentCellConfirmChangesFailedCommandBehavior<object>
    { }

    // GridDataControlCurrentCellConfirmChangesFailedCommandWithEventArgs	
    public class GridDataControlCurrentCellConfirmChangesFailedCommandWithEventArgs : GridDataControlCurrentCellConfirmChangesFailedCommand<SyncfusionRoutedEventArgs, GridDataControlCurrentCellConfirmChangesFailedCommandBehaviorWithEventArgs>
    { }

    // GridDataControlCurrentCellConfirmChangesFailedCommandBehaviorWithEventArgs
    public class GridDataControlCurrentCellConfirmChangesFailedCommandBehaviorWithEventArgs : GridDataControlCurrentCellConfirmChangesFailedCommandBehavior<SyncfusionRoutedEventArgs>
    {
        public GridDataControlCurrentCellConfirmChangesFailedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataControlCurrentCellAcceptedChanges
    // GridDataControlCurrentCellAcceptedChangesCommand<T, TBehavior>
    public class GridDataControlCurrentCellAcceptedChangesCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataControlCurrentCellAcceptedChangesCommandBehavior<T>, new()
    { }

    // GridDataControlCurrentCellAcceptedChangesCommandBehavior<TReturn>
    public class GridDataControlCurrentCellAcceptedChangesCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, SyncfusionRoutedEventArgs>
    {
        public GridDataControlCurrentCellAcceptedChangesCommandBehavior(Func<object, SyncfusionRoutedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataControlCurrentCellAcceptedChangesCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.CurrentCellAcceptedChanges += OnEventRaised;
        }
    }

    // GridDataControlCurrentCellAcceptedChangesCommand
    public class GridDataControlCurrentCellAcceptedChangesCommand : GridDataControlCommandBase<GridDataControlCurrentCellAcceptedChangesCommandBehavior>
    { }

    // GridDataControlCurrentCellAcceptedChangesCommandBehavior
    public class GridDataControlCurrentCellAcceptedChangesCommandBehavior : GridDataControlCurrentCellAcceptedChangesCommandBehavior<object>
    { }

    // GridDataControlCurrentCellAcceptedChangesCommandWithEventArgs	
    public class GridDataControlCurrentCellAcceptedChangesCommandWithEventArgs : GridDataControlCurrentCellAcceptedChangesCommand<SyncfusionRoutedEventArgs, GridDataControlCurrentCellAcceptedChangesCommandBehaviorWithEventArgs>
    { }

    // GridDataControlCurrentCellAcceptedChangesCommandBehaviorWithEventArgs
    public class GridDataControlCurrentCellAcceptedChangesCommandBehaviorWithEventArgs : GridDataControlCurrentCellAcceptedChangesCommandBehavior<SyncfusionRoutedEventArgs>
    {
        public GridDataControlCurrentCellAcceptedChangesCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataControlCurrentCellChanging
    // GridDataControlCurrentCellChangingCommand<T, TBehavior>
    public class GridDataControlCurrentCellChangingCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataControlCurrentCellChangingCommandBehavior<T>, new()
    { }

    // GridDataControlCurrentCellChangingCommandBehavior<TReturn>
    public class GridDataControlCurrentCellChangingCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, SyncfusionCancelRoutedEventArgs>
    {
        public GridDataControlCurrentCellChangingCommandBehavior(Func<object, SyncfusionCancelRoutedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataControlCurrentCellChangingCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.CurrentCellChanging += OnEventRaised;
        }
    }

    // GridDataControlCurrentCellChangingCommand
    public class GridDataControlCurrentCellChangingCommand : GridDataControlCommandBase<GridDataControlCurrentCellChangingCommandBehavior>
    { }

    // GridDataControlCurrentCellChangingCommandBehavior
    public class GridDataControlCurrentCellChangingCommandBehavior : GridDataControlCurrentCellChangingCommandBehavior<object>
    { }

    // GridDataControlCurrentCellChangingCommandWithEventArgs	
    public class GridDataControlCurrentCellChangingCommandWithEventArgs : GridDataControlCurrentCellChangingCommand<SyncfusionCancelRoutedEventArgs, GridDataControlCurrentCellChangingCommandBehaviorWithEventArgs>
    { }

    // GridDataControlCurrentCellChangingCommandBehaviorWithEventArgs
    public class GridDataControlCurrentCellChangingCommandBehaviorWithEventArgs : GridDataControlCurrentCellChangingCommandBehavior<SyncfusionCancelRoutedEventArgs>
    {
        public GridDataControlCurrentCellChangingCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataControlCurrentCellStartEditing
    // GridDataControlCurrentCellStartEditingCommand<T, TBehavior>
    public class GridDataControlCurrentCellStartEditingCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataControlCurrentCellStartEditingCommandBehavior<T>, new()
    { }

    // GridDataControlCurrentCellStartEditingCommandBehavior<TReturn>
    public class GridDataControlCurrentCellStartEditingCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, SyncfusionCancelRoutedEventArgs>
    {
        public GridDataControlCurrentCellStartEditingCommandBehavior(Func<object, SyncfusionCancelRoutedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataControlCurrentCellStartEditingCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.CurrentCellStartEditing += OnEventRaised;
        }
    }

    // GridDataControlCurrentCellStartEditingCommand
    public class GridDataControlCurrentCellStartEditingCommand : GridDataControlCommandBase<GridDataControlCurrentCellStartEditingCommandBehavior>
    { }

    // GridDataControlCurrentCellStartEditingCommandBehavior
    public class GridDataControlCurrentCellStartEditingCommandBehavior : GridDataControlCurrentCellStartEditingCommandBehavior<object>
    { }

    // GridDataControlCurrentCellStartEditingCommandWithEventArgs	
    public class GridDataControlCurrentCellStartEditingCommandWithEventArgs : GridDataControlCurrentCellStartEditingCommand<SyncfusionCancelRoutedEventArgs, GridDataControlCurrentCellStartEditingCommandBehaviorWithEventArgs>
    { }

    // GridDataControlCurrentCellStartEditingCommandBehaviorWithEventArgs
    public class GridDataControlCurrentCellStartEditingCommandBehaviorWithEventArgs : GridDataControlCurrentCellStartEditingCommandBehavior<SyncfusionCancelRoutedEventArgs>
    {
        public GridDataControlCurrentCellStartEditingCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataControlCurrentCellLoaded
    // GridDataControlCurrentCellLoadedCommand<T, TBehavior>
    public class GridDataControlCurrentCellLoadedCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataControlCurrentCellLoadedCommandBehavior<T>, new()
    { }

    // GridDataControlCurrentCellLoadedCommandBehavior<TReturn>
    public class GridDataControlCurrentCellLoadedCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, GridCurrentCellLoadedEventArgs>
    {
        public GridDataControlCurrentCellLoadedCommandBehavior(Func<object, GridCurrentCellLoadedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataControlCurrentCellLoadedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.CurrentCellLoaded += OnEventRaised;
        }
    }

    // GridDataControlCurrentCellLoadedCommand
    public class GridDataControlCurrentCellLoadedCommand : GridDataControlCommandBase<GridDataControlCurrentCellLoadedCommandBehavior>
    { }

    // GridDataControlCurrentCellLoadedCommandBehavior
    public class GridDataControlCurrentCellLoadedCommandBehavior : GridDataControlCurrentCellLoadedCommandBehavior<object>
    { }

    // GridDataControlCurrentCellLoadedCommandWithEventArgs	
    public class GridDataControlCurrentCellLoadedCommandWithEventArgs : GridDataControlCurrentCellLoadedCommand<GridCurrentCellLoadedEventArgs, GridDataControlCurrentCellLoadedCommandBehaviorWithEventArgs>
    { }

    // GridDataControlCurrentCellLoadedCommandBehaviorWithEventArgs
    public class GridDataControlCurrentCellLoadedCommandBehaviorWithEventArgs : GridDataControlCurrentCellLoadedCommandBehavior<GridCurrentCellLoadedEventArgs>
    {
        public GridDataControlCurrentCellLoadedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataControlCurrentCellEditingComplete
    // GridDataControlCurrentCellEditingCompleteCommand<T, TBehavior>
    public class GridDataControlCurrentCellEditingCompleteCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataControlCurrentCellEditingCompleteCommandBehavior<T>, new()
    { }

    // GridDataControlCurrentCellEditingCompleteCommandBehavior<TReturn>
    public class GridDataControlCurrentCellEditingCompleteCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, SyncfusionRoutedEventArgs>
    {
        public GridDataControlCurrentCellEditingCompleteCommandBehavior(Func<object, SyncfusionRoutedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataControlCurrentCellEditingCompleteCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.CurrentCellEditingComplete += OnEventRaised;
        }
    }

    // GridDataControlCurrentCellEditingCompleteCommand
    public class GridDataControlCurrentCellEditingCompleteCommand : GridDataControlCommandBase<GridDataControlCurrentCellEditingCompleteCommandBehavior>
    { }

    // GridDataControlCurrentCellEditingCompleteCommandBehavior
    public class GridDataControlCurrentCellEditingCompleteCommandBehavior : GridDataControlCurrentCellEditingCompleteCommandBehavior<object>
    { }

    // GridDataControlCurrentCellEditingCompleteCommandWithEventArgs	
    public class GridDataControlCurrentCellEditingCompleteCommandWithEventArgs : GridDataControlCurrentCellEditingCompleteCommand<SyncfusionRoutedEventArgs, GridDataControlCurrentCellEditingCompleteCommandBehaviorWithEventArgs>
    { }

    // GridDataControlCurrentCellEditingCompleteCommandBehaviorWithEventArgs
    public class GridDataControlCurrentCellEditingCompleteCommandBehaviorWithEventArgs : GridDataControlCurrentCellEditingCompleteCommandBehavior<SyncfusionRoutedEventArgs>
    {
        public GridDataControlCurrentCellEditingCompleteCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataControlCurrentCellRejectedChanges
    // GridDataControlCurrentCellRejectedChangesCommand<T, TBehavior>
    public class GridDataControlCurrentCellRejectedChangesCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataControlCurrentCellRejectedChangesCommandBehavior<T>, new()
    { }

    // GridDataControlCurrentCellRejectedChangesCommandBehavior<TReturn>
    public class GridDataControlCurrentCellRejectedChangesCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, SyncfusionRoutedEventArgs>
    {
        public GridDataControlCurrentCellRejectedChangesCommandBehavior(Func<object, SyncfusionRoutedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataControlCurrentCellRejectedChangesCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.CurrentCellRejectedChanges += OnEventRaised;
        }
    }

    // GridDataControlCurrentCellRejectedChangesCommand
    public class GridDataControlCurrentCellRejectedChangesCommand : GridDataControlCommandBase<GridDataControlCurrentCellRejectedChangesCommandBehavior>
    { }

    // GridDataControlCurrentCellRejectedChangesCommandBehavior
    public class GridDataControlCurrentCellRejectedChangesCommandBehavior : GridDataControlCurrentCellRejectedChangesCommandBehavior<object>
    { }

    // GridDataControlCurrentCellRejectedChangesCommandWithEventArgs	
    public class GridDataControlCurrentCellRejectedChangesCommandWithEventArgs : GridDataControlCurrentCellRejectedChangesCommand<SyncfusionRoutedEventArgs, GridDataControlCurrentCellRejectedChangesCommandBehaviorWithEventArgs>
    { }

    // GridDataControlCurrentCellRejectedChangesCommandBehaviorWithEventArgs
    public class GridDataControlCurrentCellRejectedChangesCommandBehaviorWithEventArgs : GridDataControlCurrentCellRejectedChangesCommandBehavior<SyncfusionRoutedEventArgs>
    {
        public GridDataControlCurrentCellRejectedChangesCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataControlCurrentCellChanged
    // GridDataControlCurrentCellChangedCommand<T, TBehavior>
    public class GridDataControlCurrentCellChangedCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataControlCurrentCellChangedCommandBehavior<T>, new()
    { }

    // GridDataControlCurrentCellChangedCommandBehavior<TReturn>
    public class GridDataControlCurrentCellChangedCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, SyncfusionRoutedEventArgs>
    {
        public GridDataControlCurrentCellChangedCommandBehavior(Func<object, SyncfusionRoutedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataControlCurrentCellChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.CurrentCellChanged += OnEventRaised;
        }
    }

    // GridDataControlCurrentCellChangedCommand
    public class GridDataControlCurrentCellChangedCommand : GridDataControlCommandBase<GridDataControlCurrentCellChangedCommandBehavior>
    { }

    // GridDataControlCurrentCellChangedCommandBehavior
    public class GridDataControlCurrentCellChangedCommandBehavior : GridDataControlCurrentCellChangedCommandBehavior<object>
    { }

    // GridDataControlCurrentCellChangedCommandWithEventArgs	
    public class GridDataControlCurrentCellChangedCommandWithEventArgs : GridDataControlCurrentCellChangedCommand<SyncfusionRoutedEventArgs, GridDataControlCurrentCellChangedCommandBehaviorWithEventArgs>
    { }

    // GridDataControlCurrentCellChangedCommandBehaviorWithEventArgs
    public class GridDataControlCurrentCellChangedCommandBehaviorWithEventArgs : GridDataControlCurrentCellChangedCommandBehavior<SyncfusionRoutedEventArgs>
    {
        public GridDataControlCurrentCellChangedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataControlCurrentCellMoved
    // GridDataControlCurrentCellMovedCommand<T, TBehavior>
    public class GridDataControlCurrentCellMovedCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataControlCurrentCellMovedCommandBehavior<T>, new()
    { }

    // GridDataControlCurrentCellMovedCommandBehavior<TReturn>
    public class GridDataControlCurrentCellMovedCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, GridCurrentCellMovedEventArgs>
    {
        public GridDataControlCurrentCellMovedCommandBehavior(Func<object, GridCurrentCellMovedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataControlCurrentCellMovedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.CurrentCellMoved += OnEventRaised;
        }
    }

    // GridDataControlCurrentCellMovedCommand
    public class GridDataControlCurrentCellMovedCommand : GridDataControlCommandBase<GridDataControlCurrentCellMovedCommandBehavior>
    { }

    // GridDataControlCurrentCellMovedCommandBehavior
    public class GridDataControlCurrentCellMovedCommandBehavior : GridDataControlCurrentCellMovedCommandBehavior<object>
    { }

    // GridDataControlCurrentCellMovedCommandWithEventArgs	
    public class GridDataControlCurrentCellMovedCommandWithEventArgs : GridDataControlCurrentCellMovedCommand<GridCurrentCellMovedEventArgs, GridDataControlCurrentCellMovedCommandBehaviorWithEventArgs>
    { }

    // GridDataControlCurrentCellMovedCommandBehaviorWithEventArgs
    public class GridDataControlCurrentCellMovedCommandBehaviorWithEventArgs : GridDataControlCurrentCellMovedCommandBehavior<GridCurrentCellMovedEventArgs>
    {
        public GridDataControlCurrentCellMovedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataControlCurrentCellMoveFailed
    // GridDataControlCurrentCellMoveFailedCommand<T, TBehavior>
    public class GridDataControlCurrentCellMoveFailedCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataControlCurrentCellMoveFailedCommandBehavior<T>, new()
    { }

    // GridDataControlCurrentCellMoveFailedCommandBehavior<TReturn>
    public class GridDataControlCurrentCellMoveFailedCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, GridCurrentCellMoveFailedEventArgs>
    {
        public GridDataControlCurrentCellMoveFailedCommandBehavior(Func<object, GridCurrentCellMoveFailedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataControlCurrentCellMoveFailedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.CurrentCellMoveFailed += OnEventRaised;
        }
    }

    // GridDataControlCurrentCellMoveFailedCommand
    public class GridDataControlCurrentCellMoveFailedCommand : GridDataControlCommandBase<GridDataControlCurrentCellMoveFailedCommandBehavior>
    { }

    // GridDataControlCurrentCellMoveFailedCommandBehavior
    public class GridDataControlCurrentCellMoveFailedCommandBehavior : GridDataControlCurrentCellMoveFailedCommandBehavior<object>
    { }

    // GridDataControlCurrentCellMoveFailedCommandWithEventArgs	
    public class GridDataControlCurrentCellMoveFailedCommandWithEventArgs : GridDataControlCurrentCellMoveFailedCommand<GridCurrentCellMoveFailedEventArgs, GridDataControlCurrentCellMoveFailedCommandBehaviorWithEventArgs>
    { }

    // GridDataControlCurrentCellMoveFailedCommandBehaviorWithEventArgs
    public class GridDataControlCurrentCellMoveFailedCommandBehaviorWithEventArgs : GridDataControlCurrentCellMoveFailedCommandBehavior<GridCurrentCellMoveFailedEventArgs>
    {
        public GridDataControlCurrentCellMoveFailedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataControlCurrentCellMoving
    // GridDataControlCurrentCellMovingCommand<T, TBehavior>
    public class GridDataControlCurrentCellMovingCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataControlCurrentCellMovingCommandBehavior<T>, new()
    { }

    // GridDataControlCurrentCellMovingCommandBehavior<TReturn>
    public class GridDataControlCurrentCellMovingCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, GridCurrentCellMovingEventArgs>
    {
        public GridDataControlCurrentCellMovingCommandBehavior(Func<object, GridCurrentCellMovingEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataControlCurrentCellMovingCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.CurrentCellMoving += OnEventRaised;
        }
    }

    // GridDataControlCurrentCellMovingCommand
    public class GridDataControlCurrentCellMovingCommand : GridDataControlCommandBase<GridDataControlCurrentCellMovingCommandBehavior>
    { }

    // GridDataControlCurrentCellMovingCommandBehavior
    public class GridDataControlCurrentCellMovingCommandBehavior : GridDataControlCurrentCellMovingCommandBehavior<object>
    { }

    // GridDataControlCurrentCellMovingCommandWithEventArgs	
    public class GridDataControlCurrentCellMovingCommandWithEventArgs : GridDataControlCurrentCellMovingCommand<GridCurrentCellMovingEventArgs, GridDataControlCurrentCellMovingCommandBehaviorWithEventArgs>
    { }

    // GridDataControlCurrentCellMovingCommandBehaviorWithEventArgs
    public class GridDataControlCurrentCellMovingCommandBehaviorWithEventArgs : GridDataControlCurrentCellMovingCommandBehavior<GridCurrentCellMovingEventArgs>
    {
        public GridDataControlCurrentCellMovingCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataControlCurrentCellValidating
    // GridDataControlCurrentCellValidatingCommand<T, TBehavior>
    public class GridDataControlCurrentCellValidatingCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataControlCurrentCellValidatingCommandBehavior<T>, new()
    { }

    // GridDataControlCurrentCellValidatingCommandBehavior<TReturn>
    public class GridDataControlCurrentCellValidatingCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, CurrentCellValidatingEventArgs>
    {
        public GridDataControlCurrentCellValidatingCommandBehavior(Func<object, CurrentCellValidatingEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataControlCurrentCellValidatingCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.CurrentCellValidating += OnEventRaised;
        }
    }

    // GridDataControlCurrentCellValidatingCommand
    public class GridDataControlCurrentCellValidatingCommand : GridDataControlCommandBase<GridDataControlCurrentCellValidatingCommandBehavior>
    { }

    // GridDataControlCurrentCellValidatingCommandBehavior
    public class GridDataControlCurrentCellValidatingCommandBehavior : GridDataControlCurrentCellValidatingCommandBehavior<object>
    { }

    // GridDataControlCurrentCellValidatingCommandWithEventArgs	
    public class GridDataControlCurrentCellValidatingCommandWithEventArgs : GridDataControlCurrentCellValidatingCommand<CurrentCellValidatingEventArgs, GridDataControlCurrentCellValidatingCommandBehaviorWithEventArgs>
    { }

    // GridDataControlCurrentCellValidatingCommandBehaviorWithEventArgs
    public class GridDataControlCurrentCellValidatingCommandBehaviorWithEventArgs : GridDataControlCurrentCellValidatingCommandBehavior<CurrentCellValidatingEventArgs>
    {
        public GridDataControlCurrentCellValidatingCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataControlCurrentCellValidated
    // GridDataControlCurrentCellValidatedCommand<T, TBehavior>
    public class GridDataControlCurrentCellValidatedCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataControlCurrentCellValidatedCommandBehavior<T>, new()
    { }

    // GridDataControlCurrentCellValidatedCommandBehavior<TReturn>
    public class GridDataControlCurrentCellValidatedCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, SyncfusionRoutedEventArgs>
    {
        public GridDataControlCurrentCellValidatedCommandBehavior(Func<object, SyncfusionRoutedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataControlCurrentCellValidatedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.CurrentCellValidated += OnEventRaised;
        }
    }

    // GridDataControlCurrentCellValidatedCommand
    public class GridDataControlCurrentCellValidatedCommand : GridDataControlCommandBase<GridDataControlCurrentCellValidatedCommandBehavior>
    { }

    // GridDataControlCurrentCellValidatedCommandBehavior
    public class GridDataControlCurrentCellValidatedCommandBehavior : GridDataControlCurrentCellValidatedCommandBehavior<object>
    { }

    // GridDataControlCurrentCellValidatedCommandWithEventArgs	
    public class GridDataControlCurrentCellValidatedCommandWithEventArgs : GridDataControlCurrentCellValidatedCommand<SyncfusionRoutedEventArgs, GridDataControlCurrentCellValidatedCommandBehaviorWithEventArgs>
    { }

    // GridDataControlCurrentCellValidatedCommandBehaviorWithEventArgs
    public class GridDataControlCurrentCellValidatedCommandBehaviorWithEventArgs : GridDataControlCurrentCellValidatedCommandBehavior<SyncfusionRoutedEventArgs>
    {
        public GridDataControlCurrentCellValidatedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataControlCellPreviewKeyDown
    // GridDataControlCellPreviewKeyDownCommand<T, TBehavior>
    public class GridDataControlCellPreviewKeyDownCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataControlCellPreviewKeyDownCommandBehavior<T>, new()
    { }

    // GridDataControlCellPreviewKeyDownCommandBehavior<TReturn>
    public class GridDataControlCellPreviewKeyDownCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, GridCellKeyEventArgs>
    {
        public GridDataControlCellPreviewKeyDownCommandBehavior(Func<object, GridCellKeyEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataControlCellPreviewKeyDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.CellPreviewKeyDown += OnEventRaised;
        }
    }

    // GridDataControlCellPreviewKeyDownCommand
    public class GridDataControlCellPreviewKeyDownCommand : GridDataControlCommandBase<GridDataControlCellPreviewKeyDownCommandBehavior>
    { }

    // GridDataControlCellPreviewKeyDownCommandBehavior
    public class GridDataControlCellPreviewKeyDownCommandBehavior : GridDataControlCellPreviewKeyDownCommandBehavior<object>
    { }

    // GridDataControlCellPreviewKeyDownCommandWithEventArgs	
    public class GridDataControlCellPreviewKeyDownCommandWithEventArgs : GridDataControlCellPreviewKeyDownCommand<GridCellKeyEventArgs, GridDataControlCellPreviewKeyDownCommandBehaviorWithEventArgs>
    { }

    // GridDataControlCellPreviewKeyDownCommandBehaviorWithEventArgs
    public class GridDataControlCellPreviewKeyDownCommandBehaviorWithEventArgs : GridDataControlCellPreviewKeyDownCommandBehavior<GridCellKeyEventArgs>
    {
        public GridDataControlCellPreviewKeyDownCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataControlCellKeyDown
    // GridDataControlCellKeyDownCommand<T, TBehavior>
    public class GridDataControlCellKeyDownCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataControlCellKeyDownCommandBehavior<T>, new()
    { }

    // GridDataControlCellKeyDownCommandBehavior<TReturn>
    public class GridDataControlCellKeyDownCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, GridCellKeyEventArgs>
    {
        public GridDataControlCellKeyDownCommandBehavior(Func<object, GridCellKeyEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataControlCellKeyDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.CellKeyDown += OnEventRaised;
        }
    }

    // GridDataControlCellKeyDownCommand
    public class GridDataControlCellKeyDownCommand : GridDataControlCommandBase<GridDataControlCellKeyDownCommandBehavior>
    { }

    // GridDataControlCellKeyDownCommandBehavior
    public class GridDataControlCellKeyDownCommandBehavior : GridDataControlCellKeyDownCommandBehavior<object>
    { }

    // GridDataControlCellKeyDownCommandWithEventArgs	
    public class GridDataControlCellKeyDownCommandWithEventArgs : GridDataControlCellKeyDownCommand<GridCellKeyEventArgs, GridDataControlCellKeyDownCommandBehaviorWithEventArgs>
    { }

    // GridDataControlCellKeyDownCommandBehaviorWithEventArgs
    public class GridDataControlCellKeyDownCommandBehaviorWithEventArgs : GridDataControlCellKeyDownCommandBehavior<GridCellKeyEventArgs>
    {
        public GridDataControlCellKeyDownCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataControlRecordsSelectionChanging
    // GridDataControlRecordsSelectionChangingCommand<T, TBehavior>
    public class GridDataControlRecordsSelectionChangingCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataControlRecordsSelectionChangingCommandBehavior<T>, new()
    { }

    // GridDataControlRecordsSelectionChangingCommandBehavior<TReturn>
    public class GridDataControlRecordsSelectionChangingCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, GridDataRecordSelectionChangingEventArgs>
    {
        public GridDataControlRecordsSelectionChangingCommandBehavior(Func<object, GridDataRecordSelectionChangingEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataControlRecordsSelectionChangingCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.RecordsSelectionChanging += OnEventRaised;
        }
    }

    // GridDataControlRecordsSelectionChangingCommand
    public class GridDataControlRecordsSelectionChangingCommand : GridDataControlCommandBase<GridDataControlRecordsSelectionChangingCommandBehavior>
    { }

    // GridDataControlRecordsSelectionChangingCommandBehavior
    public class GridDataControlRecordsSelectionChangingCommandBehavior : GridDataControlRecordsSelectionChangingCommandBehavior<object>
    { }

    // GridDataControlRecordsSelectionChangingCommandWithEventArgs	
    public class GridDataControlRecordsSelectionChangingCommandWithEventArgs : GridDataControlRecordsSelectionChangingCommand<GridDataRecordSelectionChangingEventArgs, GridDataControlRecordsSelectionChangingCommandBehaviorWithEventArgs>
    { }

    // GridDataControlRecordsSelectionChangingCommandBehaviorWithEventArgs
    public class GridDataControlRecordsSelectionChangingCommandBehaviorWithEventArgs : GridDataControlRecordsSelectionChangingCommandBehavior<GridDataRecordSelectionChangingEventArgs>
    {
        public GridDataControlRecordsSelectionChangingCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataControlRecordsSelectionChanged
    // GridDataControlRecordsSelectionChangedCommand<T, TBehavior>
    public class GridDataControlRecordsSelectionChangedCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataControlRecordsSelectionChangedCommandBehavior<T>, new()
    { }

    // GridDataControlRecordsSelectionChangedCommandBehavior<TReturn>
    public class GridDataControlRecordsSelectionChangedCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, GridDataRecordsSelectionChangedEventArgs>
    {
        public GridDataControlRecordsSelectionChangedCommandBehavior(Func<object, GridDataRecordsSelectionChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataControlRecordsSelectionChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.RecordsSelectionChanged += OnEventRaised;
        }
    }

    // GridDataControlRecordsSelectionChangedCommand
    public class GridDataControlRecordsSelectionChangedCommand : GridDataControlCommandBase<GridDataControlRecordsSelectionChangedCommandBehavior>
    { }

    // GridDataControlRecordsSelectionChangedCommandBehavior
    public class GridDataControlRecordsSelectionChangedCommandBehavior : GridDataControlRecordsSelectionChangedCommandBehavior<object>
    { }

    // GridDataControlRecordsSelectionChangedCommandWithEventArgs	
    public class GridDataControlRecordsSelectionChangedCommandWithEventArgs : GridDataControlRecordsSelectionChangedCommand<GridDataRecordsSelectionChangedEventArgs, GridDataControlRecordsSelectionChangedCommandBehaviorWithEventArgs>
    { }

    // GridDataControlRecordsSelectionChangedCommandBehaviorWithEventArgs
    public class GridDataControlRecordsSelectionChangedCommandBehaviorWithEventArgs : GridDataControlRecordsSelectionChangedCommandBehavior<GridDataRecordsSelectionChangedEventArgs>
    {
        public GridDataControlRecordsSelectionChangedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataControlCellButtonClick
    // GridDataControlCellButtonClickCommand<T, TBehavior>
    public class GridDataControlCellButtonClickCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataControlCellButtonClickCommandBehavior<T>, new()
    { }

    // GridDataControlCellButtonClickCommandBehavior<TReturn>
    public class GridDataControlCellButtonClickCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, GridCellButtonClickEventArgs>
    {
        public GridDataControlCellButtonClickCommandBehavior(Func<object, GridCellButtonClickEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataControlCellButtonClickCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.CellButtonClick += OnEventRaised;
        }
    }

    // GridDataControlCellButtonClickCommand
    public class GridDataControlCellButtonClickCommand : GridDataControlCommandBase<GridDataControlCellButtonClickCommandBehavior>
    { }

    // GridDataControlCellButtonClickCommandBehavior
    public class GridDataControlCellButtonClickCommandBehavior : GridDataControlCellButtonClickCommandBehavior<object>
    { }

    // GridDataControlCellButtonClickCommandWithEventArgs	
    public class GridDataControlCellButtonClickCommandWithEventArgs : GridDataControlCellButtonClickCommand<GridCellButtonClickEventArgs, GridDataControlCellButtonClickCommandBehaviorWithEventArgs>
    { }

    // GridDataControlCellButtonClickCommandBehaviorWithEventArgs
    public class GridDataControlCellButtonClickCommandBehaviorWithEventArgs : GridDataControlCellButtonClickCommandBehavior<GridCellButtonClickEventArgs>
    {
        public GridDataControlCellButtonClickCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataControlDropDownSelectionChanged
    // GridDataControlDropDownSelectionChangedCommand<T, TBehavior>
    public class GridDataControlDropDownSelectionChangedCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataControlDropDownSelectionChangedCommandBehavior<T>, new()
    { }

    // GridDataControlDropDownSelectionChangedCommandBehavior<TReturn>
    public class GridDataControlDropDownSelectionChangedCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, GridCellComboValueChangedEventArgs>
    {
        public GridDataControlDropDownSelectionChangedCommandBehavior(Func<object, GridCellComboValueChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataControlDropDownSelectionChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.DropDownSelectionChanged += OnEventRaised;
        }
    }

    // GridDataControlDropDownSelectionChangedCommand
    public class GridDataControlDropDownSelectionChangedCommand : GridDataControlCommandBase<GridDataControlDropDownSelectionChangedCommandBehavior>
    { }

    // GridDataControlDropDownSelectionChangedCommandBehavior
    public class GridDataControlDropDownSelectionChangedCommandBehavior : GridDataControlDropDownSelectionChangedCommandBehavior<object>
    { }

    // GridDataControlDropDownSelectionChangedCommandWithEventArgs	
    public class GridDataControlDropDownSelectionChangedCommandWithEventArgs : GridDataControlDropDownSelectionChangedCommand<GridCellComboValueChangedEventArgs, GridDataControlDropDownSelectionChangedCommandBehaviorWithEventArgs>
    { }

    // GridDataControlDropDownSelectionChangedCommandBehaviorWithEventArgs
    public class GridDataControlDropDownSelectionChangedCommandBehaviorWithEventArgs : GridDataControlDropDownSelectionChangedCommandBehavior<GridCellComboValueChangedEventArgs>
    {
        public GridDataControlDropDownSelectionChangedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataControlCellClick
    // GridDataControlCellClickCommand<T, TBehavior>
    public class GridDataControlCellClickCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataControlCellClickCommandBehavior<T>, new()
    { }

    // GridDataControlCellClickCommandBehavior<TReturn>
    public class GridDataControlCellClickCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, GridCellClickEventArgs>
    {
        public GridDataControlCellClickCommandBehavior(Func<object, GridCellClickEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataControlCellClickCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.CellClick += OnEventRaised;
        }
    }

    // GridDataControlCellClickCommand
    public class GridDataControlCellClickCommand : GridDataControlCommandBase<GridDataControlCellClickCommandBehavior>
    { }

    // GridDataControlCellClickCommandBehavior
    public class GridDataControlCellClickCommandBehavior : GridDataControlCellClickCommandBehavior<object>
    { }

    // GridDataControlCellClickCommandWithEventArgs	
    public class GridDataControlCellClickCommandWithEventArgs : GridDataControlCellClickCommand<GridCellClickEventArgs, GridDataControlCellClickCommandBehaviorWithEventArgs>
    { }

    // GridDataControlCellClickCommandBehaviorWithEventArgs
    public class GridDataControlCellClickCommandBehaviorWithEventArgs : GridDataControlCellClickCommandBehavior<GridCellClickEventArgs>
    {
        public GridDataControlCellClickCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataControlCellCursor
    // GridDataControlCellCursorCommand<T, TBehavior>
    public class GridDataControlCellCursorCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataControlCellCursorCommandBehavior<T>, new()
    { }

    // GridDataControlCellCursorCommandBehavior<TReturn>
    public class GridDataControlCellCursorCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, GridCellCursorEventArgs>
    {
        public GridDataControlCellCursorCommandBehavior(Func<object, GridCellCursorEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataControlCellCursorCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.CellCursor += OnEventRaised;
        }
    }

    // GridDataControlCellCursorCommand
    public class GridDataControlCellCursorCommand : GridDataControlCommandBase<GridDataControlCellCursorCommandBehavior>
    { }

    // GridDataControlCellCursorCommandBehavior
    public class GridDataControlCellCursorCommandBehavior : GridDataControlCellCursorCommandBehavior<object>
    { }

    // GridDataControlCellCursorCommandWithEventArgs	
    public class GridDataControlCellCursorCommandWithEventArgs : GridDataControlCellCursorCommand<GridCellCursorEventArgs, GridDataControlCellCursorCommandBehaviorWithEventArgs>
    { }

    // GridDataControlCellCursorCommandBehaviorWithEventArgs
    public class GridDataControlCellCursorCommandBehaviorWithEventArgs : GridDataControlCellCursorCommandBehavior<GridCellCursorEventArgs>
    {
        public GridDataControlCellCursorCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataControlCellMouseHoverEnter
    // GridDataControlCellMouseHoverEnterCommand<T, TBehavior>
    public class GridDataControlCellMouseHoverEnterCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataControlCellMouseHoverEnterCommandBehavior<T>, new()
    { }

    // GridDataControlCellMouseHoverEnterCommandBehavior<TReturn>
    public class GridDataControlCellMouseHoverEnterCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, GridCellMouseEventArgs>
    {
        public GridDataControlCellMouseHoverEnterCommandBehavior(Func<object, GridCellMouseEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataControlCellMouseHoverEnterCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.CellMouseHoverEnter += OnEventRaised;
        }
    }

    // GridDataControlCellMouseHoverEnterCommand
    public class GridDataControlCellMouseHoverEnterCommand : GridDataControlCommandBase<GridDataControlCellMouseHoverEnterCommandBehavior>
    { }

    // GridDataControlCellMouseHoverEnterCommandBehavior
    public class GridDataControlCellMouseHoverEnterCommandBehavior : GridDataControlCellMouseHoverEnterCommandBehavior<object>
    { }

    // GridDataControlCellMouseHoverEnterCommandWithEventArgs	
    public class GridDataControlCellMouseHoverEnterCommandWithEventArgs : GridDataControlCellMouseHoverEnterCommand<GridCellMouseEventArgs, GridDataControlCellMouseHoverEnterCommandBehaviorWithEventArgs>
    { }

    // GridDataControlCellMouseHoverEnterCommandBehaviorWithEventArgs
    public class GridDataControlCellMouseHoverEnterCommandBehaviorWithEventArgs : GridDataControlCellMouseHoverEnterCommandBehavior<GridCellMouseEventArgs>
    {
        public GridDataControlCellMouseHoverEnterCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataControlCellMouseHover
    // GridDataControlCellMouseHoverCommand<T, TBehavior>
    public class GridDataControlCellMouseHoverCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataControlCellMouseHoverCommandBehavior<T>, new()
    { }

    // GridDataControlCellMouseHoverCommandBehavior<TReturn>
    public class GridDataControlCellMouseHoverCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, GridCellMouseControllerEventArgs>
    {
        public GridDataControlCellMouseHoverCommandBehavior(Func<object, GridCellMouseControllerEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataControlCellMouseHoverCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.CellMouseHover += OnEventRaised;
        }
    }

    // GridDataControlCellMouseHoverCommand
    public class GridDataControlCellMouseHoverCommand : GridDataControlCommandBase<GridDataControlCellMouseHoverCommandBehavior>
    { }

    // GridDataControlCellMouseHoverCommandBehavior
    public class GridDataControlCellMouseHoverCommandBehavior : GridDataControlCellMouseHoverCommandBehavior<object>
    { }

    // GridDataControlCellMouseHoverCommandWithEventArgs	
    public class GridDataControlCellMouseHoverCommandWithEventArgs : GridDataControlCellMouseHoverCommand<GridCellMouseControllerEventArgs, GridDataControlCellMouseHoverCommandBehaviorWithEventArgs>
    { }

    // GridDataControlCellMouseHoverCommandBehaviorWithEventArgs
    public class GridDataControlCellMouseHoverCommandBehaviorWithEventArgs : GridDataControlCellMouseHoverCommandBehavior<GridCellMouseControllerEventArgs>
    {
        public GridDataControlCellMouseHoverCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataControlCellMouseHoverLeave
    // GridDataControlCellMouseHoverLeaveCommand<T, TBehavior>
    public class GridDataControlCellMouseHoverLeaveCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataControlCellMouseHoverLeaveCommandBehavior<T>, new()
    { }

    // GridDataControlCellMouseHoverLeaveCommandBehavior<TReturn>
    public class GridDataControlCellMouseHoverLeaveCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, GridCellMouseEventArgs>
    {
        public GridDataControlCellMouseHoverLeaveCommandBehavior(Func<object, GridCellMouseEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataControlCellMouseHoverLeaveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.CellMouseHoverLeave += OnEventRaised;
        }
    }

    // GridDataControlCellMouseHoverLeaveCommand
    public class GridDataControlCellMouseHoverLeaveCommand : GridDataControlCommandBase<GridDataControlCellMouseHoverLeaveCommandBehavior>
    { }

    // GridDataControlCellMouseHoverLeaveCommandBehavior
    public class GridDataControlCellMouseHoverLeaveCommandBehavior : GridDataControlCellMouseHoverLeaveCommandBehavior<object>
    { }

    // GridDataControlCellMouseHoverLeaveCommandWithEventArgs	
    public class GridDataControlCellMouseHoverLeaveCommandWithEventArgs : GridDataControlCellMouseHoverLeaveCommand<GridCellMouseEventArgs, GridDataControlCellMouseHoverLeaveCommandBehaviorWithEventArgs>
    { }

    // GridDataControlCellMouseHoverLeaveCommandBehaviorWithEventArgs
    public class GridDataControlCellMouseHoverLeaveCommandBehaviorWithEventArgs : GridDataControlCellMouseHoverLeaveCommandBehavior<GridCellMouseEventArgs>
    {
        public GridDataControlCellMouseHoverLeaveCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataControlCellMouseDown
    // GridDataControlCellMouseDownCommand<T, TBehavior>
    public class GridDataControlCellMouseDownCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataControlCellMouseDownCommandBehavior<T>, new()
    { }

    // GridDataControlCellMouseDownCommandBehavior<TReturn>
    public class GridDataControlCellMouseDownCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, GridCellMouseControllerEventArgs>
    {
        public GridDataControlCellMouseDownCommandBehavior(Func<object, GridCellMouseControllerEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataControlCellMouseDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.CellMouseDown += OnEventRaised;
        }
    }

    // GridDataControlCellMouseDownCommand
    public class GridDataControlCellMouseDownCommand : GridDataControlCommandBase<GridDataControlCellMouseDownCommandBehavior>
    { }

    // GridDataControlCellMouseDownCommandBehavior
    public class GridDataControlCellMouseDownCommandBehavior : GridDataControlCellMouseDownCommandBehavior<object>
    { }

    // GridDataControlCellMouseDownCommandWithEventArgs	
    public class GridDataControlCellMouseDownCommandWithEventArgs : GridDataControlCellMouseDownCommand<GridCellMouseControllerEventArgs, GridDataControlCellMouseDownCommandBehaviorWithEventArgs>
    { }

    // GridDataControlCellMouseDownCommandBehaviorWithEventArgs
    public class GridDataControlCellMouseDownCommandBehaviorWithEventArgs : GridDataControlCellMouseDownCommandBehavior<GridCellMouseControllerEventArgs>
    {
        public GridDataControlCellMouseDownCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataControlCellMouseMove
    // GridDataControlCellMouseMoveCommand<T, TBehavior>
    public class GridDataControlCellMouseMoveCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataControlCellMouseMoveCommandBehavior<T>, new()
    { }

    // GridDataControlCellMouseMoveCommandBehavior<TReturn>
    public class GridDataControlCellMouseMoveCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, GridCellMouseControllerEventArgs>
    {
        public GridDataControlCellMouseMoveCommandBehavior(Func<object, GridCellMouseControllerEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataControlCellMouseMoveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.CellMouseMove += OnEventRaised;
        }
    }

    // GridDataControlCellMouseMoveCommand
    public class GridDataControlCellMouseMoveCommand : GridDataControlCommandBase<GridDataControlCellMouseMoveCommandBehavior>
    { }

    // GridDataControlCellMouseMoveCommandBehavior
    public class GridDataControlCellMouseMoveCommandBehavior : GridDataControlCellMouseMoveCommandBehavior<object>
    { }

    // GridDataControlCellMouseMoveCommandWithEventArgs	
    public class GridDataControlCellMouseMoveCommandWithEventArgs : GridDataControlCellMouseMoveCommand<GridCellMouseControllerEventArgs, GridDataControlCellMouseMoveCommandBehaviorWithEventArgs>
    { }

    // GridDataControlCellMouseMoveCommandBehaviorWithEventArgs
    public class GridDataControlCellMouseMoveCommandBehaviorWithEventArgs : GridDataControlCellMouseMoveCommandBehavior<GridCellMouseControllerEventArgs>
    {
        public GridDataControlCellMouseMoveCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataControlCellMouseUp
    // GridDataControlCellMouseUpCommand<T, TBehavior>
    public class GridDataControlCellMouseUpCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataControlCellMouseUpCommandBehavior<T>, new()
    { }

    // GridDataControlCellMouseUpCommandBehavior<TReturn>
    public class GridDataControlCellMouseUpCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, GridCellMouseControllerEventArgs>
    {
        public GridDataControlCellMouseUpCommandBehavior(Func<object, GridCellMouseControllerEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataControlCellMouseUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.CellMouseUp += OnEventRaised;
        }
    }

    // GridDataControlCellMouseUpCommand
    public class GridDataControlCellMouseUpCommand : GridDataControlCommandBase<GridDataControlCellMouseUpCommandBehavior>
    { }

    // GridDataControlCellMouseUpCommandBehavior
    public class GridDataControlCellMouseUpCommandBehavior : GridDataControlCellMouseUpCommandBehavior<object>
    { }

    // GridDataControlCellMouseUpCommandWithEventArgs	
    public class GridDataControlCellMouseUpCommandWithEventArgs : GridDataControlCellMouseUpCommand<GridCellMouseControllerEventArgs, GridDataControlCellMouseUpCommandBehaviorWithEventArgs>
    { }

    // GridDataControlCellMouseUpCommandBehaviorWithEventArgs
    public class GridDataControlCellMouseUpCommandBehaviorWithEventArgs : GridDataControlCellMouseUpCommandBehavior<GridCellMouseControllerEventArgs>
    {
        public GridDataControlCellMouseUpCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataControlCellCancelMode
    // GridDataControlCellCancelModeCommand<T, TBehavior>
    public class GridDataControlCellCancelModeCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataControlCellCancelModeCommandBehavior<T>, new()
    { }

    // GridDataControlCellCancelModeCommandBehavior<TReturn>
    public class GridDataControlCellCancelModeCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, SyncfusionRoutedEventArgs>
    {
        public GridDataControlCellCancelModeCommandBehavior(Func<object, SyncfusionRoutedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataControlCellCancelModeCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.CellCancelMode += OnEventRaised;
        }
    }

    // GridDataControlCellCancelModeCommand
    public class GridDataControlCellCancelModeCommand : GridDataControlCommandBase<GridDataControlCellCancelModeCommandBehavior>
    { }

    // GridDataControlCellCancelModeCommandBehavior
    public class GridDataControlCellCancelModeCommandBehavior : GridDataControlCellCancelModeCommandBehavior<object>
    { }

    // GridDataControlCellCancelModeCommandWithEventArgs	
    public class GridDataControlCellCancelModeCommandWithEventArgs : GridDataControlCellCancelModeCommand<SyncfusionRoutedEventArgs, GridDataControlCellCancelModeCommandBehaviorWithEventArgs>
    { }

    // GridDataControlCellCancelModeCommandBehaviorWithEventArgs
    public class GridDataControlCellCancelModeCommandBehaviorWithEventArgs : GridDataControlCellCancelModeCommandBehavior<SyncfusionRoutedEventArgs>
    {
        public GridDataControlCellCancelModeCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataControlCellRestoreMode
    // GridDataControlCellRestoreModeCommand<T, TBehavior>
    public class GridDataControlCellRestoreModeCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataControlCellRestoreModeCommandBehavior<T>, new()
    { }

    // GridDataControlCellRestoreModeCommandBehavior<TReturn>
    public class GridDataControlCellRestoreModeCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, SyncfusionRoutedEventArgs>
    {
        public GridDataControlCellRestoreModeCommandBehavior(Func<object, SyncfusionRoutedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataControlCellRestoreModeCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.CellRestoreMode += OnEventRaised;
        }
    }

    // GridDataControlCellRestoreModeCommand
    public class GridDataControlCellRestoreModeCommand : GridDataControlCommandBase<GridDataControlCellRestoreModeCommandBehavior>
    { }

    // GridDataControlCellRestoreModeCommandBehavior
    public class GridDataControlCellRestoreModeCommandBehavior : GridDataControlCellRestoreModeCommandBehavior<object>
    { }

    // GridDataControlCellRestoreModeCommandWithEventArgs	
    public class GridDataControlCellRestoreModeCommandWithEventArgs : GridDataControlCellRestoreModeCommand<SyncfusionRoutedEventArgs, GridDataControlCellRestoreModeCommandBehaviorWithEventArgs>
    { }

    // GridDataControlCellRestoreModeCommandBehaviorWithEventArgs
    public class GridDataControlCellRestoreModeCommandBehaviorWithEventArgs : GridDataControlCellRestoreModeCommandBehavior<SyncfusionRoutedEventArgs>
    {
        public GridDataControlCellRestoreModeCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataControlResizingColumns
    // GridDataControlResizingColumnsCommand<T, TBehavior>
    public class GridDataControlResizingColumnsCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataControlResizingColumnsCommandBehavior<T>, new()
    { }

    // GridDataControlResizingColumnsCommandBehavior<TReturn>
    public class GridDataControlResizingColumnsCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, GridResizingColumnsEventArgs>
    {
        public GridDataControlResizingColumnsCommandBehavior(Func<object, GridResizingColumnsEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataControlResizingColumnsCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ResizingColumns += OnEventRaised;
        }
    }

    // GridDataControlResizingColumnsCommand
    public class GridDataControlResizingColumnsCommand : GridDataControlCommandBase<GridDataControlResizingColumnsCommandBehavior>
    { }

    // GridDataControlResizingColumnsCommandBehavior
    public class GridDataControlResizingColumnsCommandBehavior : GridDataControlResizingColumnsCommandBehavior<object>
    { }

    // GridDataControlResizingColumnsCommandWithEventArgs	
    public class GridDataControlResizingColumnsCommandWithEventArgs : GridDataControlResizingColumnsCommand<GridResizingColumnsEventArgs, GridDataControlResizingColumnsCommandBehaviorWithEventArgs>
    { }

    // GridDataControlResizingColumnsCommandBehaviorWithEventArgs
    public class GridDataControlResizingColumnsCommandBehaviorWithEventArgs : GridDataControlResizingColumnsCommandBehavior<GridResizingColumnsEventArgs>
    {
        public GridDataControlResizingColumnsCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataControlQueryAllowDragColumn
    // GridDataControlQueryAllowDragColumnCommand<T, TBehavior>
    public class GridDataControlQueryAllowDragColumnCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataControlQueryAllowDragColumnCommandBehavior<T>, new()
    { }

    // GridDataControlQueryAllowDragColumnCommandBehavior<TReturn>
    public class GridDataControlQueryAllowDragColumnCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, GridQueryDragColumnHeaderEventArgs>
    {
        public GridDataControlQueryAllowDragColumnCommandBehavior(Func<object, GridQueryDragColumnHeaderEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataControlQueryAllowDragColumnCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.QueryAllowDragColumn += OnEventRaised;
        }
    }

    // GridDataControlQueryAllowDragColumnCommand
    public class GridDataControlQueryAllowDragColumnCommand : GridDataControlCommandBase<GridDataControlQueryAllowDragColumnCommandBehavior>
    { }

    // GridDataControlQueryAllowDragColumnCommandBehavior
    public class GridDataControlQueryAllowDragColumnCommandBehavior : GridDataControlQueryAllowDragColumnCommandBehavior<object>
    { }

    // GridDataControlQueryAllowDragColumnCommandWithEventArgs	
    public class GridDataControlQueryAllowDragColumnCommandWithEventArgs : GridDataControlQueryAllowDragColumnCommand<GridQueryDragColumnHeaderEventArgs, GridDataControlQueryAllowDragColumnCommandBehaviorWithEventArgs>
    { }

    // GridDataControlQueryAllowDragColumnCommandBehaviorWithEventArgs
    public class GridDataControlQueryAllowDragColumnCommandBehaviorWithEventArgs : GridDataControlQueryAllowDragColumnCommandBehavior<GridQueryDragColumnHeaderEventArgs>
    {
        public GridDataControlQueryAllowDragColumnCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GridDataControlResizingRows
    // GridDataControlResizingRowsCommand<T, TBehavior>
    public class GridDataControlResizingRowsCommand<T, TBehavior> : GridDataControlCommandBase<TBehavior> where TBehavior : GridDataControlResizingRowsCommandBehavior<T>, new()
    { }

    // GridDataControlResizingRowsCommandBehavior<TReturn>
    public class GridDataControlResizingRowsCommandBehavior<TReturn> : GridDataControlCommandBehaviorBase<TReturn, GridResizingRowsEventArgs>
    {
        public GridDataControlResizingRowsCommandBehavior(Func<object, GridResizingRowsEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GridDataControlResizingRowsCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ResizingRows += OnEventRaised;
        }
    }

    // GridDataControlResizingRowsCommand
    public class GridDataControlResizingRowsCommand : GridDataControlCommandBase<GridDataControlResizingRowsCommandBehavior>
    { }

    // GridDataControlResizingRowsCommandBehavior
    public class GridDataControlResizingRowsCommandBehavior : GridDataControlResizingRowsCommandBehavior<object>
    { }

    // GridDataControlResizingRowsCommandWithEventArgs	
    public class GridDataControlResizingRowsCommandWithEventArgs : GridDataControlResizingRowsCommand<GridResizingRowsEventArgs, GridDataControlResizingRowsCommandBehaviorWithEventArgs>
    { }

    // GridDataControlResizingRowsCommandBehaviorWithEventArgs
    public class GridDataControlResizingRowsCommandBehaviorWithEventArgs : GridDataControlResizingRowsCommandBehavior<GridResizingRowsEventArgs>
    {
        public GridDataControlResizingRowsCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

#endif
}


