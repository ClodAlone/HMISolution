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
using Syncfusion.Windows.Shared;

using Syncfusion.Windows.Controls.Map;

namespace Syncfusion.Maps.MVVM
{   

	#region MapControlPreviewMouseDoubleClickCommand
	// MapControlPreviewMouseDoubleClickCommand
	public class MapControlPreviewMouseDoubleClickCommand : MapControlCommandBase<MapControlPreviewMouseDoubleClickCommandBehavior>
	{ }

    public class MapControlPreviewMouseDoubleClickCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlPreviewMouseDoubleClickCommandBehavior<T>,new()
    { }

    public class MapControlPreviewMouseDoubleClickCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public MapControlPreviewMouseDoubleClickCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlPreviewMouseDoubleClickCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewMouseDoubleClick += OnEventRaised;
        }
    }

	// MapControlPreviewMouseDoubleClickCommandBehavior
    public class MapControlPreviewMouseDoubleClickCommandBehavior : MapControlPreviewMouseDoubleClickCommandBehavior<object>
    { }
	#endregion

	#region MapControlMouseDoubleClickCommand
	// MapControlMouseDoubleClickCommand
	public class MapControlMouseDoubleClickCommand : MapControlCommandBase<MapControlMouseDoubleClickCommandBehavior>
	{ }

    public class MapControlMouseDoubleClickCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlMouseDoubleClickCommandBehavior<T>,new()
    { }

    public class MapControlMouseDoubleClickCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public MapControlMouseDoubleClickCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlMouseDoubleClickCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseDoubleClick += OnEventRaised;
        }
    }

	// MapControlMouseDoubleClickCommandBehavior
    public class MapControlMouseDoubleClickCommandBehavior : MapControlMouseDoubleClickCommandBehavior<object>
    { }
	#endregion

	#region MapControlTargetUpdatedCommand
	// MapControlTargetUpdatedCommand
	public class MapControlTargetUpdatedCommand : MapControlCommandBase<MapControlTargetUpdatedCommandBehavior>
	{ }

    public class MapControlTargetUpdatedCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlTargetUpdatedCommandBehavior<T>,new()
    { }

    public class MapControlTargetUpdatedCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, DataTransferEventArgs>
    {
        public MapControlTargetUpdatedCommandBehavior(Func<object, DataTransferEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlTargetUpdatedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.TargetUpdated += OnEventRaised;
        }
    }

	// MapControlTargetUpdatedCommandBehavior
    public class MapControlTargetUpdatedCommandBehavior : MapControlTargetUpdatedCommandBehavior<object>
    { }
	#endregion

	#region MapControlSourceUpdatedCommand
	// MapControlSourceUpdatedCommand
	public class MapControlSourceUpdatedCommand : MapControlCommandBase<MapControlSourceUpdatedCommandBehavior>
	{ }

    public class MapControlSourceUpdatedCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlSourceUpdatedCommandBehavior<T>,new()
    { }

    public class MapControlSourceUpdatedCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, DataTransferEventArgs>
    {
        public MapControlSourceUpdatedCommandBehavior(Func<object, DataTransferEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlSourceUpdatedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.SourceUpdated += OnEventRaised;
        }
    }

	// MapControlSourceUpdatedCommandBehavior
    public class MapControlSourceUpdatedCommandBehavior : MapControlSourceUpdatedCommandBehavior<object>
    { }
	#endregion

	#region MapControlDataContextChangedCommand
	// MapControlDataContextChangedCommand
	public class MapControlDataContextChangedCommand : MapControlCommandBase<MapControlDataContextChangedCommandBehavior>
	{ }

    public class MapControlDataContextChangedCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlDataContextChangedCommandBehavior<T>,new()
    { }

    public class MapControlDataContextChangedCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public MapControlDataContextChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlDataContextChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.DataContextChanged += OnEventRaised;
        }
    }

	// MapControlDataContextChangedCommandBehavior
    public class MapControlDataContextChangedCommandBehavior : MapControlDataContextChangedCommandBehavior<object>
    { }
	#endregion

	#region MapControlRequestBringIntoViewCommand
	// MapControlRequestBringIntoViewCommand
	public class MapControlRequestBringIntoViewCommand : MapControlCommandBase<MapControlRequestBringIntoViewCommandBehavior>
	{ }

    public class MapControlRequestBringIntoViewCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlRequestBringIntoViewCommandBehavior<T>,new()
    { }

    public class MapControlRequestBringIntoViewCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, RequestBringIntoViewEventArgs>
    {
        public MapControlRequestBringIntoViewCommandBehavior(Func<object, RequestBringIntoViewEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlRequestBringIntoViewCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.RequestBringIntoView += OnEventRaised;
        }
    }

	// MapControlRequestBringIntoViewCommandBehavior
    public class MapControlRequestBringIntoViewCommandBehavior : MapControlRequestBringIntoViewCommandBehavior<object>
    { }
	#endregion

	#region MapControlSizeChangedCommand
	// MapControlSizeChangedCommand
	public class MapControlSizeChangedCommand : MapControlCommandBase<MapControlSizeChangedCommandBehavior>
	{ }

    public class MapControlSizeChangedCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlSizeChangedCommandBehavior<T>,new()
    { }

    public class MapControlSizeChangedCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, SizeChangedEventArgs>
    {
        public MapControlSizeChangedCommandBehavior(Func<object, SizeChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlSizeChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.SizeChanged += OnEventRaised;
        }
    }

	// MapControlSizeChangedCommandBehavior
    public class MapControlSizeChangedCommandBehavior : MapControlSizeChangedCommandBehavior<object>
    { }
	#endregion

	#region MapControlInitializedCommand
	// MapControlInitializedCommand
	public class MapControlInitializedCommand : MapControlCommandBase<MapControlInitializedCommandBehavior>
	{ }

    public class MapControlInitializedCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlInitializedCommandBehavior<T>,new()
    { }

    public class MapControlInitializedCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, EventArgs>
    {
        public MapControlInitializedCommandBehavior(Func<object, EventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlInitializedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Initialized += OnEventRaised;
        }
    }

	// MapControlInitializedCommandBehavior
    public class MapControlInitializedCommandBehavior : MapControlInitializedCommandBehavior<object>
    { }
	#endregion

	#region MapControlLoadedCommand
	// MapControlLoadedCommand
	public class MapControlLoadedCommand : MapControlCommandBase<MapControlLoadedCommandBehavior>
	{ }

    public class MapControlLoadedCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlLoadedCommandBehavior<T>,new()
    { }

    public class MapControlLoadedCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, RoutedEventArgs>
    {
        public MapControlLoadedCommandBehavior(Func<object, RoutedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlLoadedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Loaded += OnEventRaised;
        }
    }

	// MapControlLoadedCommandBehavior
    public class MapControlLoadedCommandBehavior : MapControlLoadedCommandBehavior<object>
    { }
	#endregion

	#region MapControlUnloadedCommand
	// MapControlUnloadedCommand
	public class MapControlUnloadedCommand : MapControlCommandBase<MapControlUnloadedCommandBehavior>
	{ }

    public class MapControlUnloadedCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlUnloadedCommandBehavior<T>,new()
    { }

    public class MapControlUnloadedCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, RoutedEventArgs>
    {
        public MapControlUnloadedCommandBehavior(Func<object, RoutedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlUnloadedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Unloaded += OnEventRaised;
        }
    }

	// MapControlUnloadedCommandBehavior
    public class MapControlUnloadedCommandBehavior : MapControlUnloadedCommandBehavior<object>
    { }
	#endregion

	#region MapControlToolTipOpeningCommand
	// MapControlToolTipOpeningCommand
	public class MapControlToolTipOpeningCommand : MapControlCommandBase<MapControlToolTipOpeningCommandBehavior>
	{ }

    public class MapControlToolTipOpeningCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlToolTipOpeningCommandBehavior<T>,new()
    { }

    public class MapControlToolTipOpeningCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, ToolTipEventArgs>
    {
        public MapControlToolTipOpeningCommandBehavior(Func<object, ToolTipEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlToolTipOpeningCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ToolTipOpening += OnEventRaised;
        }
    }

	// MapControlToolTipOpeningCommandBehavior
    public class MapControlToolTipOpeningCommandBehavior : MapControlToolTipOpeningCommandBehavior<object>
    { }
	#endregion

	#region MapControlToolTipClosingCommand
	// MapControlToolTipClosingCommand
	public class MapControlToolTipClosingCommand : MapControlCommandBase<MapControlToolTipClosingCommandBehavior>
	{ }

    public class MapControlToolTipClosingCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlToolTipClosingCommandBehavior<T>,new()
    { }

    public class MapControlToolTipClosingCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, ToolTipEventArgs>
    {
        public MapControlToolTipClosingCommandBehavior(Func<object, ToolTipEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlToolTipClosingCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ToolTipClosing += OnEventRaised;
        }
    }

	// MapControlToolTipClosingCommandBehavior
    public class MapControlToolTipClosingCommandBehavior : MapControlToolTipClosingCommandBehavior<object>
    { }
	#endregion

	#region MapControlContextMenuOpeningCommand
	// MapControlContextMenuOpeningCommand
	public class MapControlContextMenuOpeningCommand : MapControlCommandBase<MapControlContextMenuOpeningCommandBehavior>
	{ }

    public class MapControlContextMenuOpeningCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlContextMenuOpeningCommandBehavior<T>,new()
    { }

    public class MapControlContextMenuOpeningCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, ContextMenuEventArgs>
    {
        public MapControlContextMenuOpeningCommandBehavior(Func<object, ContextMenuEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlContextMenuOpeningCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ContextMenuOpening += OnEventRaised;
        }
    }

	// MapControlContextMenuOpeningCommandBehavior
    public class MapControlContextMenuOpeningCommandBehavior : MapControlContextMenuOpeningCommandBehavior<object>
    { }
	#endregion

	#region MapControlContextMenuClosingCommand
	// MapControlContextMenuClosingCommand
	public class MapControlContextMenuClosingCommand : MapControlCommandBase<MapControlContextMenuClosingCommandBehavior>
	{ }

    public class MapControlContextMenuClosingCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlContextMenuClosingCommandBehavior<T>,new()
    { }

    public class MapControlContextMenuClosingCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, ContextMenuEventArgs>
    {
        public MapControlContextMenuClosingCommandBehavior(Func<object, ContextMenuEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlContextMenuClosingCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ContextMenuClosing += OnEventRaised;
        }
    }

	// MapControlContextMenuClosingCommandBehavior
    public class MapControlContextMenuClosingCommandBehavior : MapControlContextMenuClosingCommandBehavior<object>
    { }
	#endregion

	#region MapControlPreviewMouseDownCommand
	// MapControlPreviewMouseDownCommand
	public class MapControlPreviewMouseDownCommand : MapControlCommandBase<MapControlPreviewMouseDownCommandBehavior>
	{ }

    public class MapControlPreviewMouseDownCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlPreviewMouseDownCommandBehavior<T>,new()
    { }

    public class MapControlPreviewMouseDownCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public MapControlPreviewMouseDownCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlPreviewMouseDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewMouseDown += OnEventRaised;
        }
    }

	// MapControlPreviewMouseDownCommandBehavior
    public class MapControlPreviewMouseDownCommandBehavior : MapControlPreviewMouseDownCommandBehavior<object>
    { }
	#endregion

	#region MapControlMouseDownCommand
	// MapControlMouseDownCommand
	public class MapControlMouseDownCommand : MapControlCommandBase<MapControlMouseDownCommandBehavior>
	{ }

    public class MapControlMouseDownCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlMouseDownCommandBehavior<T>,new()
    { }

    public class MapControlMouseDownCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public MapControlMouseDownCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlMouseDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseDown += OnEventRaised;
        }
    }

	// MapControlMouseDownCommandBehavior
    public class MapControlMouseDownCommandBehavior : MapControlMouseDownCommandBehavior<object>
    { }
	#endregion

	#region MapControlPreviewMouseUpCommand
	// MapControlPreviewMouseUpCommand
	public class MapControlPreviewMouseUpCommand : MapControlCommandBase<MapControlPreviewMouseUpCommandBehavior>
	{ }

    public class MapControlPreviewMouseUpCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlPreviewMouseUpCommandBehavior<T>,new()
    { }

    public class MapControlPreviewMouseUpCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public MapControlPreviewMouseUpCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlPreviewMouseUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewMouseUp += OnEventRaised;
        }
    }

	// MapControlPreviewMouseUpCommandBehavior
    public class MapControlPreviewMouseUpCommandBehavior : MapControlPreviewMouseUpCommandBehavior<object>
    { }
	#endregion

	#region MapControlMouseUpCommand
	// MapControlMouseUpCommand
	public class MapControlMouseUpCommand : MapControlCommandBase<MapControlMouseUpCommandBehavior>
	{ }

    public class MapControlMouseUpCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlMouseUpCommandBehavior<T>,new()
    { }

    public class MapControlMouseUpCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public MapControlMouseUpCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlMouseUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseUp += OnEventRaised;
        }
    }

	// MapControlMouseUpCommandBehavior
    public class MapControlMouseUpCommandBehavior : MapControlMouseUpCommandBehavior<object>
    { }
	#endregion

	#region MapControlPreviewMouseLeftButtonDownCommand
	// MapControlPreviewMouseLeftButtonDownCommand
	public class MapControlPreviewMouseLeftButtonDownCommand : MapControlCommandBase<MapControlPreviewMouseLeftButtonDownCommandBehavior>
	{ }

    public class MapControlPreviewMouseLeftButtonDownCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlPreviewMouseLeftButtonDownCommandBehavior<T>,new()
    { }

    public class MapControlPreviewMouseLeftButtonDownCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public MapControlPreviewMouseLeftButtonDownCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlPreviewMouseLeftButtonDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewMouseLeftButtonDown += OnEventRaised;
        }
    }

	// MapControlPreviewMouseLeftButtonDownCommandBehavior
    public class MapControlPreviewMouseLeftButtonDownCommandBehavior : MapControlPreviewMouseLeftButtonDownCommandBehavior<object>
    { }
	#endregion

	#region MapControlMouseLeftButtonDownCommand
	// MapControlMouseLeftButtonDownCommand
	public class MapControlMouseLeftButtonDownCommand : MapControlCommandBase<MapControlMouseLeftButtonDownCommandBehavior>
	{ }

    public class MapControlMouseLeftButtonDownCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlMouseLeftButtonDownCommandBehavior<T>,new()
    { }

    public class MapControlMouseLeftButtonDownCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public MapControlMouseLeftButtonDownCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlMouseLeftButtonDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseLeftButtonDown += OnEventRaised;
        }
    }

	// MapControlMouseLeftButtonDownCommandBehavior
    public class MapControlMouseLeftButtonDownCommandBehavior : MapControlMouseLeftButtonDownCommandBehavior<object>
    { }
	#endregion

	#region MapControlPreviewMouseLeftButtonUpCommand
	// MapControlPreviewMouseLeftButtonUpCommand
	public class MapControlPreviewMouseLeftButtonUpCommand : MapControlCommandBase<MapControlPreviewMouseLeftButtonUpCommandBehavior>
	{ }

    public class MapControlPreviewMouseLeftButtonUpCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlPreviewMouseLeftButtonUpCommandBehavior<T>,new()
    { }

    public class MapControlPreviewMouseLeftButtonUpCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public MapControlPreviewMouseLeftButtonUpCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlPreviewMouseLeftButtonUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewMouseLeftButtonUp += OnEventRaised;
        }
    }

	// MapControlPreviewMouseLeftButtonUpCommandBehavior
    public class MapControlPreviewMouseLeftButtonUpCommandBehavior : MapControlPreviewMouseLeftButtonUpCommandBehavior<object>
    { }
	#endregion

	#region MapControlMouseLeftButtonUpCommand
	// MapControlMouseLeftButtonUpCommand
	public class MapControlMouseLeftButtonUpCommand : MapControlCommandBase<MapControlMouseLeftButtonUpCommandBehavior>
	{ }

    public class MapControlMouseLeftButtonUpCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlMouseLeftButtonUpCommandBehavior<T>,new()
    { }

    public class MapControlMouseLeftButtonUpCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public MapControlMouseLeftButtonUpCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlMouseLeftButtonUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseLeftButtonUp += OnEventRaised;
        }
    }

	// MapControlMouseLeftButtonUpCommandBehavior
    public class MapControlMouseLeftButtonUpCommandBehavior : MapControlMouseLeftButtonUpCommandBehavior<object>
    { }
	#endregion

	#region MapControlPreviewMouseRightButtonDownCommand
	// MapControlPreviewMouseRightButtonDownCommand
	public class MapControlPreviewMouseRightButtonDownCommand : MapControlCommandBase<MapControlPreviewMouseRightButtonDownCommandBehavior>
	{ }

    public class MapControlPreviewMouseRightButtonDownCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlPreviewMouseRightButtonDownCommandBehavior<T>,new()
    { }

    public class MapControlPreviewMouseRightButtonDownCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public MapControlPreviewMouseRightButtonDownCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlPreviewMouseRightButtonDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewMouseRightButtonDown += OnEventRaised;
        }
    }

	// MapControlPreviewMouseRightButtonDownCommandBehavior
    public class MapControlPreviewMouseRightButtonDownCommandBehavior : MapControlPreviewMouseRightButtonDownCommandBehavior<object>
    { }
	#endregion

	#region MapControlMouseRightButtonDownCommand
	// MapControlMouseRightButtonDownCommand
	public class MapControlMouseRightButtonDownCommand : MapControlCommandBase<MapControlMouseRightButtonDownCommandBehavior>
	{ }

    public class MapControlMouseRightButtonDownCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlMouseRightButtonDownCommandBehavior<T>,new()
    { }

    public class MapControlMouseRightButtonDownCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public MapControlMouseRightButtonDownCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlMouseRightButtonDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseRightButtonDown += OnEventRaised;
        }
    }

	// MapControlMouseRightButtonDownCommandBehavior
    public class MapControlMouseRightButtonDownCommandBehavior : MapControlMouseRightButtonDownCommandBehavior<object>
    { }
	#endregion

	#region MapControlPreviewMouseRightButtonUpCommand
	// MapControlPreviewMouseRightButtonUpCommand
	public class MapControlPreviewMouseRightButtonUpCommand : MapControlCommandBase<MapControlPreviewMouseRightButtonUpCommandBehavior>
	{ }

    public class MapControlPreviewMouseRightButtonUpCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlPreviewMouseRightButtonUpCommandBehavior<T>,new()
    { }

    public class MapControlPreviewMouseRightButtonUpCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public MapControlPreviewMouseRightButtonUpCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlPreviewMouseRightButtonUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewMouseRightButtonUp += OnEventRaised;
        }
    }

	// MapControlPreviewMouseRightButtonUpCommandBehavior
    public class MapControlPreviewMouseRightButtonUpCommandBehavior : MapControlPreviewMouseRightButtonUpCommandBehavior<object>
    { }
	#endregion

	#region MapControlMouseRightButtonUpCommand
	// MapControlMouseRightButtonUpCommand
	public class MapControlMouseRightButtonUpCommand : MapControlCommandBase<MapControlMouseRightButtonUpCommandBehavior>
	{ }

    public class MapControlMouseRightButtonUpCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlMouseRightButtonUpCommandBehavior<T>,new()
    { }

    public class MapControlMouseRightButtonUpCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public MapControlMouseRightButtonUpCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlMouseRightButtonUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseRightButtonUp += OnEventRaised;
        }
    }

	// MapControlMouseRightButtonUpCommandBehavior
    public class MapControlMouseRightButtonUpCommandBehavior : MapControlMouseRightButtonUpCommandBehavior<object>
    { }
	#endregion

	#region MapControlPreviewMouseMoveCommand
	// MapControlPreviewMouseMoveCommand
	public class MapControlPreviewMouseMoveCommand : MapControlCommandBase<MapControlPreviewMouseMoveCommandBehavior>
	{ }

    public class MapControlPreviewMouseMoveCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlPreviewMouseMoveCommandBehavior<T>,new()
    { }

    public class MapControlPreviewMouseMoveCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, MouseEventArgs>
    {
        public MapControlPreviewMouseMoveCommandBehavior(Func<object, MouseEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlPreviewMouseMoveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewMouseMove += OnEventRaised;
        }
    }

	// MapControlPreviewMouseMoveCommandBehavior
    public class MapControlPreviewMouseMoveCommandBehavior : MapControlPreviewMouseMoveCommandBehavior<object>
    { }
	#endregion

	#region MapControlMouseMoveCommand
	// MapControlMouseMoveCommand
	public class MapControlMouseMoveCommand : MapControlCommandBase<MapControlMouseMoveCommandBehavior>
	{ }

    public class MapControlMouseMoveCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlMouseMoveCommandBehavior<T>,new()
    { }

    public class MapControlMouseMoveCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, MouseEventArgs>
    {
        public MapControlMouseMoveCommandBehavior(Func<object, MouseEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlMouseMoveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseMove += OnEventRaised;
        }
    }

	// MapControlMouseMoveCommandBehavior
    public class MapControlMouseMoveCommandBehavior : MapControlMouseMoveCommandBehavior<object>
    { }
	#endregion

	#region MapControlPreviewMouseWheelCommand
	// MapControlPreviewMouseWheelCommand
	public class MapControlPreviewMouseWheelCommand : MapControlCommandBase<MapControlPreviewMouseWheelCommandBehavior>
	{ }

    public class MapControlPreviewMouseWheelCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlPreviewMouseWheelCommandBehavior<T>,new()
    { }

    public class MapControlPreviewMouseWheelCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, MouseWheelEventArgs>
    {
        public MapControlPreviewMouseWheelCommandBehavior(Func<object, MouseWheelEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlPreviewMouseWheelCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewMouseWheel += OnEventRaised;
        }
    }

	// MapControlPreviewMouseWheelCommandBehavior
    public class MapControlPreviewMouseWheelCommandBehavior : MapControlPreviewMouseWheelCommandBehavior<object>
    { }
	#endregion

	#region MapControlMouseWheelCommand
	// MapControlMouseWheelCommand
	public class MapControlMouseWheelCommand : MapControlCommandBase<MapControlMouseWheelCommandBehavior>
	{ }

    public class MapControlMouseWheelCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlMouseWheelCommandBehavior<T>,new()
    { }

    public class MapControlMouseWheelCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, MouseWheelEventArgs>
    {
        public MapControlMouseWheelCommandBehavior(Func<object, MouseWheelEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlMouseWheelCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseWheel += OnEventRaised;
        }
    }

	// MapControlMouseWheelCommandBehavior
    public class MapControlMouseWheelCommandBehavior : MapControlMouseWheelCommandBehavior<object>
    { }
	#endregion

	#region MapControlMouseEnterCommand
	// MapControlMouseEnterCommand
	public class MapControlMouseEnterCommand : MapControlCommandBase<MapControlMouseEnterCommandBehavior>
	{ }

    public class MapControlMouseEnterCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlMouseEnterCommandBehavior<T>,new()
    { }

    public class MapControlMouseEnterCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, MouseEventArgs>
    {
        public MapControlMouseEnterCommandBehavior(Func<object, MouseEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlMouseEnterCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseEnter += OnEventRaised;
        }
    }

	// MapControlMouseEnterCommandBehavior
    public class MapControlMouseEnterCommandBehavior : MapControlMouseEnterCommandBehavior<object>
    { }
	#endregion

	#region MapControlMouseLeaveCommand
	// MapControlMouseLeaveCommand
	public class MapControlMouseLeaveCommand : MapControlCommandBase<MapControlMouseLeaveCommandBehavior>
	{ }

    public class MapControlMouseLeaveCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlMouseLeaveCommandBehavior<T>,new()
    { }

    public class MapControlMouseLeaveCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, MouseEventArgs>
    {
        public MapControlMouseLeaveCommandBehavior(Func<object, MouseEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlMouseLeaveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseLeave += OnEventRaised;
        }
    }

	// MapControlMouseLeaveCommandBehavior
    public class MapControlMouseLeaveCommandBehavior : MapControlMouseLeaveCommandBehavior<object>
    { }
	#endregion

	#region MapControlGotMouseCaptureCommand
	// MapControlGotMouseCaptureCommand
	public class MapControlGotMouseCaptureCommand : MapControlCommandBase<MapControlGotMouseCaptureCommandBehavior>
	{ }

    public class MapControlGotMouseCaptureCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlGotMouseCaptureCommandBehavior<T>,new()
    { }

    public class MapControlGotMouseCaptureCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, MouseEventArgs>
    {
        public MapControlGotMouseCaptureCommandBehavior(Func<object, MouseEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlGotMouseCaptureCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.GotMouseCapture += OnEventRaised;
        }
    }

	// MapControlGotMouseCaptureCommandBehavior
    public class MapControlGotMouseCaptureCommandBehavior : MapControlGotMouseCaptureCommandBehavior<object>
    { }
	#endregion

	#region MapControlLostMouseCaptureCommand
	// MapControlLostMouseCaptureCommand
	public class MapControlLostMouseCaptureCommand : MapControlCommandBase<MapControlLostMouseCaptureCommandBehavior>
	{ }

    public class MapControlLostMouseCaptureCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlLostMouseCaptureCommandBehavior<T>,new()
    { }

    public class MapControlLostMouseCaptureCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, MouseEventArgs>
    {
        public MapControlLostMouseCaptureCommandBehavior(Func<object, MouseEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlLostMouseCaptureCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.LostMouseCapture += OnEventRaised;
        }
    }

	// MapControlLostMouseCaptureCommandBehavior
    public class MapControlLostMouseCaptureCommandBehavior : MapControlLostMouseCaptureCommandBehavior<object>
    { }
	#endregion

	#region MapControlQueryCursorCommand
	// MapControlQueryCursorCommand
	public class MapControlQueryCursorCommand : MapControlCommandBase<MapControlQueryCursorCommandBehavior>
	{ }

    public class MapControlQueryCursorCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlQueryCursorCommandBehavior<T>,new()
    { }

    public class MapControlQueryCursorCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, QueryCursorEventArgs>
    {
        public MapControlQueryCursorCommandBehavior(Func<object, QueryCursorEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlQueryCursorCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.QueryCursor += OnEventRaised;
        }
    }

	// MapControlQueryCursorCommandBehavior
    public class MapControlQueryCursorCommandBehavior : MapControlQueryCursorCommandBehavior<object>
    { }
	#endregion

	#region MapControlPreviewStylusDownCommand
	// MapControlPreviewStylusDownCommand
	public class MapControlPreviewStylusDownCommand : MapControlCommandBase<MapControlPreviewStylusDownCommandBehavior>
	{ }

    public class MapControlPreviewStylusDownCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlPreviewStylusDownCommandBehavior<T>,new()
    { }

    public class MapControlPreviewStylusDownCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, StylusDownEventArgs>
    {
        public MapControlPreviewStylusDownCommandBehavior(Func<object, StylusDownEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlPreviewStylusDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewStylusDown += OnEventRaised;
        }
    }

	// MapControlPreviewStylusDownCommandBehavior
    public class MapControlPreviewStylusDownCommandBehavior : MapControlPreviewStylusDownCommandBehavior<object>
    { }
	#endregion

	#region MapControlStylusDownCommand
	// MapControlStylusDownCommand
	public class MapControlStylusDownCommand : MapControlCommandBase<MapControlStylusDownCommandBehavior>
	{ }

    public class MapControlStylusDownCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlStylusDownCommandBehavior<T>,new()
    { }

    public class MapControlStylusDownCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, StylusDownEventArgs>
    {
        public MapControlStylusDownCommandBehavior(Func<object, StylusDownEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlStylusDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusDown += OnEventRaised;
        }
    }

	// MapControlStylusDownCommandBehavior
    public class MapControlStylusDownCommandBehavior : MapControlStylusDownCommandBehavior<object>
    { }
	#endregion

	#region MapControlPreviewStylusUpCommand
	// MapControlPreviewStylusUpCommand
	public class MapControlPreviewStylusUpCommand : MapControlCommandBase<MapControlPreviewStylusUpCommandBehavior>
	{ }

    public class MapControlPreviewStylusUpCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlPreviewStylusUpCommandBehavior<T>,new()
    { }

    public class MapControlPreviewStylusUpCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public MapControlPreviewStylusUpCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlPreviewStylusUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewStylusUp += OnEventRaised;
        }
    }

	// MapControlPreviewStylusUpCommandBehavior
    public class MapControlPreviewStylusUpCommandBehavior : MapControlPreviewStylusUpCommandBehavior<object>
    { }
	#endregion

	#region MapControlStylusUpCommand
	// MapControlStylusUpCommand
	public class MapControlStylusUpCommand : MapControlCommandBase<MapControlStylusUpCommandBehavior>
	{ }

    public class MapControlStylusUpCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlStylusUpCommandBehavior<T>,new()
    { }

    public class MapControlStylusUpCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public MapControlStylusUpCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlStylusUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusUp += OnEventRaised;
        }
    }

	// MapControlStylusUpCommandBehavior
    public class MapControlStylusUpCommandBehavior : MapControlStylusUpCommandBehavior<object>
    { }
	#endregion

	#region MapControlPreviewStylusMoveCommand
	// MapControlPreviewStylusMoveCommand
	public class MapControlPreviewStylusMoveCommand : MapControlCommandBase<MapControlPreviewStylusMoveCommandBehavior>
	{ }

    public class MapControlPreviewStylusMoveCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlPreviewStylusMoveCommandBehavior<T>,new()
    { }

    public class MapControlPreviewStylusMoveCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public MapControlPreviewStylusMoveCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlPreviewStylusMoveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewStylusMove += OnEventRaised;
        }
    }

	// MapControlPreviewStylusMoveCommandBehavior
    public class MapControlPreviewStylusMoveCommandBehavior : MapControlPreviewStylusMoveCommandBehavior<object>
    { }
	#endregion

	#region MapControlStylusMoveCommand
	// MapControlStylusMoveCommand
	public class MapControlStylusMoveCommand : MapControlCommandBase<MapControlStylusMoveCommandBehavior>
	{ }

    public class MapControlStylusMoveCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlStylusMoveCommandBehavior<T>,new()
    { }

    public class MapControlStylusMoveCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public MapControlStylusMoveCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlStylusMoveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusMove += OnEventRaised;
        }
    }

	// MapControlStylusMoveCommandBehavior
    public class MapControlStylusMoveCommandBehavior : MapControlStylusMoveCommandBehavior<object>
    { }
	#endregion

	#region MapControlPreviewStylusInAirMoveCommand
	// MapControlPreviewStylusInAirMoveCommand
	public class MapControlPreviewStylusInAirMoveCommand : MapControlCommandBase<MapControlPreviewStylusInAirMoveCommandBehavior>
	{ }

    public class MapControlPreviewStylusInAirMoveCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlPreviewStylusInAirMoveCommandBehavior<T>,new()
    { }

    public class MapControlPreviewStylusInAirMoveCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public MapControlPreviewStylusInAirMoveCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlPreviewStylusInAirMoveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewStylusInAirMove += OnEventRaised;
        }
    }

	// MapControlPreviewStylusInAirMoveCommandBehavior
    public class MapControlPreviewStylusInAirMoveCommandBehavior : MapControlPreviewStylusInAirMoveCommandBehavior<object>
    { }
	#endregion

	#region MapControlStylusInAirMoveCommand
	// MapControlStylusInAirMoveCommand
	public class MapControlStylusInAirMoveCommand : MapControlCommandBase<MapControlStylusInAirMoveCommandBehavior>
	{ }

    public class MapControlStylusInAirMoveCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlStylusInAirMoveCommandBehavior<T>,new()
    { }

    public class MapControlStylusInAirMoveCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public MapControlStylusInAirMoveCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlStylusInAirMoveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusInAirMove += OnEventRaised;
        }
    }

	// MapControlStylusInAirMoveCommandBehavior
    public class MapControlStylusInAirMoveCommandBehavior : MapControlStylusInAirMoveCommandBehavior<object>
    { }
	#endregion

	#region MapControlStylusEnterCommand
	// MapControlStylusEnterCommand
	public class MapControlStylusEnterCommand : MapControlCommandBase<MapControlStylusEnterCommandBehavior>
	{ }

    public class MapControlStylusEnterCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlStylusEnterCommandBehavior<T>,new()
    { }

    public class MapControlStylusEnterCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public MapControlStylusEnterCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlStylusEnterCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusEnter += OnEventRaised;
        }
    }

	// MapControlStylusEnterCommandBehavior
    public class MapControlStylusEnterCommandBehavior : MapControlStylusEnterCommandBehavior<object>
    { }
	#endregion

	#region MapControlStylusLeaveCommand
	// MapControlStylusLeaveCommand
	public class MapControlStylusLeaveCommand : MapControlCommandBase<MapControlStylusLeaveCommandBehavior>
	{ }

    public class MapControlStylusLeaveCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlStylusLeaveCommandBehavior<T>,new()
    { }

    public class MapControlStylusLeaveCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public MapControlStylusLeaveCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlStylusLeaveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusLeave += OnEventRaised;
        }
    }

	// MapControlStylusLeaveCommandBehavior
    public class MapControlStylusLeaveCommandBehavior : MapControlStylusLeaveCommandBehavior<object>
    { }
	#endregion

	#region MapControlPreviewStylusInRangeCommand
	// MapControlPreviewStylusInRangeCommand
	public class MapControlPreviewStylusInRangeCommand : MapControlCommandBase<MapControlPreviewStylusInRangeCommandBehavior>
	{ }

    public class MapControlPreviewStylusInRangeCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlPreviewStylusInRangeCommandBehavior<T>,new()
    { }

    public class MapControlPreviewStylusInRangeCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public MapControlPreviewStylusInRangeCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlPreviewStylusInRangeCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewStylusInRange += OnEventRaised;
        }
    }

	// MapControlPreviewStylusInRangeCommandBehavior
    public class MapControlPreviewStylusInRangeCommandBehavior : MapControlPreviewStylusInRangeCommandBehavior<object>
    { }
	#endregion

	#region MapControlStylusInRangeCommand
	// MapControlStylusInRangeCommand
	public class MapControlStylusInRangeCommand : MapControlCommandBase<MapControlStylusInRangeCommandBehavior>
	{ }

    public class MapControlStylusInRangeCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlStylusInRangeCommandBehavior<T>,new()
    { }

    public class MapControlStylusInRangeCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public MapControlStylusInRangeCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlStylusInRangeCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusInRange += OnEventRaised;
        }
    }

	// MapControlStylusInRangeCommandBehavior
    public class MapControlStylusInRangeCommandBehavior : MapControlStylusInRangeCommandBehavior<object>
    { }
	#endregion

	#region MapControlPreviewStylusOutOfRangeCommand
	// MapControlPreviewStylusOutOfRangeCommand
	public class MapControlPreviewStylusOutOfRangeCommand : MapControlCommandBase<MapControlPreviewStylusOutOfRangeCommandBehavior>
	{ }

    public class MapControlPreviewStylusOutOfRangeCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlPreviewStylusOutOfRangeCommandBehavior<T>,new()
    { }

    public class MapControlPreviewStylusOutOfRangeCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public MapControlPreviewStylusOutOfRangeCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlPreviewStylusOutOfRangeCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewStylusOutOfRange += OnEventRaised;
        }
    }

	// MapControlPreviewStylusOutOfRangeCommandBehavior
    public class MapControlPreviewStylusOutOfRangeCommandBehavior : MapControlPreviewStylusOutOfRangeCommandBehavior<object>
    { }
	#endregion

	#region MapControlStylusOutOfRangeCommand
	// MapControlStylusOutOfRangeCommand
	public class MapControlStylusOutOfRangeCommand : MapControlCommandBase<MapControlStylusOutOfRangeCommandBehavior>
	{ }

    public class MapControlStylusOutOfRangeCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlStylusOutOfRangeCommandBehavior<T>,new()
    { }

    public class MapControlStylusOutOfRangeCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public MapControlStylusOutOfRangeCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlStylusOutOfRangeCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusOutOfRange += OnEventRaised;
        }
    }

	// MapControlStylusOutOfRangeCommandBehavior
    public class MapControlStylusOutOfRangeCommandBehavior : MapControlStylusOutOfRangeCommandBehavior<object>
    { }
	#endregion

	#region MapControlPreviewStylusSystemGestureCommand
	// MapControlPreviewStylusSystemGestureCommand
	public class MapControlPreviewStylusSystemGestureCommand : MapControlCommandBase<MapControlPreviewStylusSystemGestureCommandBehavior>
	{ }

    public class MapControlPreviewStylusSystemGestureCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlPreviewStylusSystemGestureCommandBehavior<T>,new()
    { }

    public class MapControlPreviewStylusSystemGestureCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, StylusSystemGestureEventArgs>
    {
        public MapControlPreviewStylusSystemGestureCommandBehavior(Func<object, StylusSystemGestureEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlPreviewStylusSystemGestureCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewStylusSystemGesture += OnEventRaised;
        }
    }

	// MapControlPreviewStylusSystemGestureCommandBehavior
    public class MapControlPreviewStylusSystemGestureCommandBehavior : MapControlPreviewStylusSystemGestureCommandBehavior<object>
    { }
	#endregion

	#region MapControlStylusSystemGestureCommand
	// MapControlStylusSystemGestureCommand
	public class MapControlStylusSystemGestureCommand : MapControlCommandBase<MapControlStylusSystemGestureCommandBehavior>
	{ }

    public class MapControlStylusSystemGestureCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlStylusSystemGestureCommandBehavior<T>,new()
    { }

    public class MapControlStylusSystemGestureCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, StylusSystemGestureEventArgs>
    {
        public MapControlStylusSystemGestureCommandBehavior(Func<object, StylusSystemGestureEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlStylusSystemGestureCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusSystemGesture += OnEventRaised;
        }
    }

	// MapControlStylusSystemGestureCommandBehavior
    public class MapControlStylusSystemGestureCommandBehavior : MapControlStylusSystemGestureCommandBehavior<object>
    { }
	#endregion

	#region MapControlGotStylusCaptureCommand
	// MapControlGotStylusCaptureCommand
	public class MapControlGotStylusCaptureCommand : MapControlCommandBase<MapControlGotStylusCaptureCommandBehavior>
	{ }

    public class MapControlGotStylusCaptureCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlGotStylusCaptureCommandBehavior<T>,new()
    { }

    public class MapControlGotStylusCaptureCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public MapControlGotStylusCaptureCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlGotStylusCaptureCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.GotStylusCapture += OnEventRaised;
        }
    }

	// MapControlGotStylusCaptureCommandBehavior
    public class MapControlGotStylusCaptureCommandBehavior : MapControlGotStylusCaptureCommandBehavior<object>
    { }
	#endregion

	#region MapControlLostStylusCaptureCommand
	// MapControlLostStylusCaptureCommand
	public class MapControlLostStylusCaptureCommand : MapControlCommandBase<MapControlLostStylusCaptureCommandBehavior>
	{ }

    public class MapControlLostStylusCaptureCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlLostStylusCaptureCommandBehavior<T>,new()
    { }

    public class MapControlLostStylusCaptureCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public MapControlLostStylusCaptureCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlLostStylusCaptureCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.LostStylusCapture += OnEventRaised;
        }
    }

	// MapControlLostStylusCaptureCommandBehavior
    public class MapControlLostStylusCaptureCommandBehavior : MapControlLostStylusCaptureCommandBehavior<object>
    { }
	#endregion

	#region MapControlStylusButtonDownCommand
	// MapControlStylusButtonDownCommand
	public class MapControlStylusButtonDownCommand : MapControlCommandBase<MapControlStylusButtonDownCommandBehavior>
	{ }

    public class MapControlStylusButtonDownCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlStylusButtonDownCommandBehavior<T>,new()
    { }

    public class MapControlStylusButtonDownCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, StylusButtonEventArgs>
    {
        public MapControlStylusButtonDownCommandBehavior(Func<object, StylusButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlStylusButtonDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusButtonDown += OnEventRaised;
        }
    }

	// MapControlStylusButtonDownCommandBehavior
    public class MapControlStylusButtonDownCommandBehavior : MapControlStylusButtonDownCommandBehavior<object>
    { }
	#endregion

	#region MapControlStylusButtonUpCommand
	// MapControlStylusButtonUpCommand
	public class MapControlStylusButtonUpCommand : MapControlCommandBase<MapControlStylusButtonUpCommandBehavior>
	{ }

    public class MapControlStylusButtonUpCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlStylusButtonUpCommandBehavior<T>,new()
    { }

    public class MapControlStylusButtonUpCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, StylusButtonEventArgs>
    {
        public MapControlStylusButtonUpCommandBehavior(Func<object, StylusButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlStylusButtonUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusButtonUp += OnEventRaised;
        }
    }

	// MapControlStylusButtonUpCommandBehavior
    public class MapControlStylusButtonUpCommandBehavior : MapControlStylusButtonUpCommandBehavior<object>
    { }
	#endregion

	#region MapControlPreviewStylusButtonDownCommand
	// MapControlPreviewStylusButtonDownCommand
	public class MapControlPreviewStylusButtonDownCommand : MapControlCommandBase<MapControlPreviewStylusButtonDownCommandBehavior>
	{ }

    public class MapControlPreviewStylusButtonDownCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlPreviewStylusButtonDownCommandBehavior<T>,new()
    { }

    public class MapControlPreviewStylusButtonDownCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, StylusButtonEventArgs>
    {
        public MapControlPreviewStylusButtonDownCommandBehavior(Func<object, StylusButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlPreviewStylusButtonDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewStylusButtonDown += OnEventRaised;
        }
    }

	// MapControlPreviewStylusButtonDownCommandBehavior
    public class MapControlPreviewStylusButtonDownCommandBehavior : MapControlPreviewStylusButtonDownCommandBehavior<object>
    { }
	#endregion

	#region MapControlPreviewStylusButtonUpCommand
	// MapControlPreviewStylusButtonUpCommand
	public class MapControlPreviewStylusButtonUpCommand : MapControlCommandBase<MapControlPreviewStylusButtonUpCommandBehavior>
	{ }

    public class MapControlPreviewStylusButtonUpCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlPreviewStylusButtonUpCommandBehavior<T>,new()
    { }

    public class MapControlPreviewStylusButtonUpCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, StylusButtonEventArgs>
    {
        public MapControlPreviewStylusButtonUpCommandBehavior(Func<object, StylusButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlPreviewStylusButtonUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewStylusButtonUp += OnEventRaised;
        }
    }

	// MapControlPreviewStylusButtonUpCommandBehavior
    public class MapControlPreviewStylusButtonUpCommandBehavior : MapControlPreviewStylusButtonUpCommandBehavior<object>
    { }
	#endregion

	#region MapControlPreviewKeyDownCommand
	// MapControlPreviewKeyDownCommand
	public class MapControlPreviewKeyDownCommand : MapControlCommandBase<MapControlPreviewKeyDownCommandBehavior>
	{ }

    public class MapControlPreviewKeyDownCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlPreviewKeyDownCommandBehavior<T>,new()
    { }

    public class MapControlPreviewKeyDownCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, KeyEventArgs>
    {
        public MapControlPreviewKeyDownCommandBehavior(Func<object, KeyEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlPreviewKeyDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewKeyDown += OnEventRaised;
        }
    }

	// MapControlPreviewKeyDownCommandBehavior
    public class MapControlPreviewKeyDownCommandBehavior : MapControlPreviewKeyDownCommandBehavior<object>
    { }
	#endregion

	#region MapControlKeyDownCommand
	// MapControlKeyDownCommand
	public class MapControlKeyDownCommand : MapControlCommandBase<MapControlKeyDownCommandBehavior>
	{ }

    public class MapControlKeyDownCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlKeyDownCommandBehavior<T>,new()
    { }

    public class MapControlKeyDownCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, KeyEventArgs>
    {
        public MapControlKeyDownCommandBehavior(Func<object, KeyEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlKeyDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.KeyDown += OnEventRaised;
        }
    }

	// MapControlKeyDownCommandBehavior
    public class MapControlKeyDownCommandBehavior : MapControlKeyDownCommandBehavior<object>
    { }
	#endregion

	#region MapControlPreviewKeyUpCommand
	// MapControlPreviewKeyUpCommand
	public class MapControlPreviewKeyUpCommand : MapControlCommandBase<MapControlPreviewKeyUpCommandBehavior>
	{ }

    public class MapControlPreviewKeyUpCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlPreviewKeyUpCommandBehavior<T>,new()
    { }

    public class MapControlPreviewKeyUpCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, KeyEventArgs>
    {
        public MapControlPreviewKeyUpCommandBehavior(Func<object, KeyEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlPreviewKeyUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewKeyUp += OnEventRaised;
        }
    }

	// MapControlPreviewKeyUpCommandBehavior
    public class MapControlPreviewKeyUpCommandBehavior : MapControlPreviewKeyUpCommandBehavior<object>
    { }
	#endregion

	#region MapControlKeyUpCommand
	// MapControlKeyUpCommand
	public class MapControlKeyUpCommand : MapControlCommandBase<MapControlKeyUpCommandBehavior>
	{ }

    public class MapControlKeyUpCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlKeyUpCommandBehavior<T>,new()
    { }

    public class MapControlKeyUpCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, KeyEventArgs>
    {
        public MapControlKeyUpCommandBehavior(Func<object, KeyEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlKeyUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.KeyUp += OnEventRaised;
        }
    }

	// MapControlKeyUpCommandBehavior
    public class MapControlKeyUpCommandBehavior : MapControlKeyUpCommandBehavior<object>
    { }
	#endregion

	#region MapControlPreviewGotKeyboardFocusCommand
	// MapControlPreviewGotKeyboardFocusCommand
	public class MapControlPreviewGotKeyboardFocusCommand : MapControlCommandBase<MapControlPreviewGotKeyboardFocusCommandBehavior>
	{ }

    public class MapControlPreviewGotKeyboardFocusCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlPreviewGotKeyboardFocusCommandBehavior<T>,new()
    { }

    public class MapControlPreviewGotKeyboardFocusCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, KeyboardFocusChangedEventArgs>
    {
        public MapControlPreviewGotKeyboardFocusCommandBehavior(Func<object, KeyboardFocusChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlPreviewGotKeyboardFocusCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewGotKeyboardFocus += OnEventRaised;
        }
    }

	// MapControlPreviewGotKeyboardFocusCommandBehavior
    public class MapControlPreviewGotKeyboardFocusCommandBehavior : MapControlPreviewGotKeyboardFocusCommandBehavior<object>
    { }
	#endregion

	#region MapControlGotKeyboardFocusCommand
	// MapControlGotKeyboardFocusCommand
	public class MapControlGotKeyboardFocusCommand : MapControlCommandBase<MapControlGotKeyboardFocusCommandBehavior>
	{ }

    public class MapControlGotKeyboardFocusCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlGotKeyboardFocusCommandBehavior<T>,new()
    { }

    public class MapControlGotKeyboardFocusCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, KeyboardFocusChangedEventArgs>
    {
        public MapControlGotKeyboardFocusCommandBehavior(Func<object, KeyboardFocusChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlGotKeyboardFocusCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.GotKeyboardFocus += OnEventRaised;
        }
    }

	// MapControlGotKeyboardFocusCommandBehavior
    public class MapControlGotKeyboardFocusCommandBehavior : MapControlGotKeyboardFocusCommandBehavior<object>
    { }
	#endregion

	#region MapControlPreviewLostKeyboardFocusCommand
	// MapControlPreviewLostKeyboardFocusCommand
	public class MapControlPreviewLostKeyboardFocusCommand : MapControlCommandBase<MapControlPreviewLostKeyboardFocusCommandBehavior>
	{ }

    public class MapControlPreviewLostKeyboardFocusCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlPreviewLostKeyboardFocusCommandBehavior<T>,new()
    { }

    public class MapControlPreviewLostKeyboardFocusCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, KeyboardFocusChangedEventArgs>
    {
        public MapControlPreviewLostKeyboardFocusCommandBehavior(Func<object, KeyboardFocusChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlPreviewLostKeyboardFocusCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewLostKeyboardFocus += OnEventRaised;
        }
    }

	// MapControlPreviewLostKeyboardFocusCommandBehavior
    public class MapControlPreviewLostKeyboardFocusCommandBehavior : MapControlPreviewLostKeyboardFocusCommandBehavior<object>
    { }
	#endregion

	#region MapControlLostKeyboardFocusCommand
	// MapControlLostKeyboardFocusCommand
	public class MapControlLostKeyboardFocusCommand : MapControlCommandBase<MapControlLostKeyboardFocusCommandBehavior>
	{ }

    public class MapControlLostKeyboardFocusCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlLostKeyboardFocusCommandBehavior<T>,new()
    { }

    public class MapControlLostKeyboardFocusCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, KeyboardFocusChangedEventArgs>
    {
        public MapControlLostKeyboardFocusCommandBehavior(Func<object, KeyboardFocusChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlLostKeyboardFocusCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.LostKeyboardFocus += OnEventRaised;
        }
    }

	// MapControlLostKeyboardFocusCommandBehavior
    public class MapControlLostKeyboardFocusCommandBehavior : MapControlLostKeyboardFocusCommandBehavior<object>
    { }
	#endregion

	#region MapControlPreviewTextInputCommand
	// MapControlPreviewTextInputCommand
	public class MapControlPreviewTextInputCommand : MapControlCommandBase<MapControlPreviewTextInputCommandBehavior>
	{ }

    public class MapControlPreviewTextInputCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlPreviewTextInputCommandBehavior<T>,new()
    { }

    public class MapControlPreviewTextInputCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, TextCompositionEventArgs>
    {
        public MapControlPreviewTextInputCommandBehavior(Func<object, TextCompositionEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlPreviewTextInputCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewTextInput += OnEventRaised;
        }
    }

	// MapControlPreviewTextInputCommandBehavior
    public class MapControlPreviewTextInputCommandBehavior : MapControlPreviewTextInputCommandBehavior<object>
    { }
	#endregion

	#region MapControlTextInputCommand
	// MapControlTextInputCommand
	public class MapControlTextInputCommand : MapControlCommandBase<MapControlTextInputCommandBehavior>
	{ }

    public class MapControlTextInputCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlTextInputCommandBehavior<T>,new()
    { }

    public class MapControlTextInputCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, TextCompositionEventArgs>
    {
        public MapControlTextInputCommandBehavior(Func<object, TextCompositionEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlTextInputCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.TextInput += OnEventRaised;
        }
    }

	// MapControlTextInputCommandBehavior
    public class MapControlTextInputCommandBehavior : MapControlTextInputCommandBehavior<object>
    { }
	#endregion

	#region MapControlPreviewQueryContinueDragCommand
	// MapControlPreviewQueryContinueDragCommand
	public class MapControlPreviewQueryContinueDragCommand : MapControlCommandBase<MapControlPreviewQueryContinueDragCommandBehavior>
	{ }

    public class MapControlPreviewQueryContinueDragCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlPreviewQueryContinueDragCommandBehavior<T>,new()
    { }

    public class MapControlPreviewQueryContinueDragCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, QueryContinueDragEventArgs>
    {
        public MapControlPreviewQueryContinueDragCommandBehavior(Func<object, QueryContinueDragEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlPreviewQueryContinueDragCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewQueryContinueDrag += OnEventRaised;
        }
    }

	// MapControlPreviewQueryContinueDragCommandBehavior
    public class MapControlPreviewQueryContinueDragCommandBehavior : MapControlPreviewQueryContinueDragCommandBehavior<object>
    { }
	#endregion

	#region MapControlQueryContinueDragCommand
	// MapControlQueryContinueDragCommand
	public class MapControlQueryContinueDragCommand : MapControlCommandBase<MapControlQueryContinueDragCommandBehavior>
	{ }

    public class MapControlQueryContinueDragCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlQueryContinueDragCommandBehavior<T>,new()
    { }

    public class MapControlQueryContinueDragCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, QueryContinueDragEventArgs>
    {
        public MapControlQueryContinueDragCommandBehavior(Func<object, QueryContinueDragEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlQueryContinueDragCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.QueryContinueDrag += OnEventRaised;
        }
    }

	// MapControlQueryContinueDragCommandBehavior
    public class MapControlQueryContinueDragCommandBehavior : MapControlQueryContinueDragCommandBehavior<object>
    { }
	#endregion

	#region MapControlPreviewGiveFeedbackCommand
	// MapControlPreviewGiveFeedbackCommand
	public class MapControlPreviewGiveFeedbackCommand : MapControlCommandBase<MapControlPreviewGiveFeedbackCommandBehavior>
	{ }

    public class MapControlPreviewGiveFeedbackCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlPreviewGiveFeedbackCommandBehavior<T>,new()
    { }

    public class MapControlPreviewGiveFeedbackCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, GiveFeedbackEventArgs>
    {
        public MapControlPreviewGiveFeedbackCommandBehavior(Func<object, GiveFeedbackEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlPreviewGiveFeedbackCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewGiveFeedback += OnEventRaised;
        }
    }

	// MapControlPreviewGiveFeedbackCommandBehavior
    public class MapControlPreviewGiveFeedbackCommandBehavior : MapControlPreviewGiveFeedbackCommandBehavior<object>
    { }
	#endregion

	#region MapControlGiveFeedbackCommand
	// MapControlGiveFeedbackCommand
	public class MapControlGiveFeedbackCommand : MapControlCommandBase<MapControlGiveFeedbackCommandBehavior>
	{ }

    public class MapControlGiveFeedbackCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlGiveFeedbackCommandBehavior<T>,new()
    { }

    public class MapControlGiveFeedbackCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, GiveFeedbackEventArgs>
    {
        public MapControlGiveFeedbackCommandBehavior(Func<object, GiveFeedbackEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlGiveFeedbackCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.GiveFeedback += OnEventRaised;
        }
    }

	// MapControlGiveFeedbackCommandBehavior
    public class MapControlGiveFeedbackCommandBehavior : MapControlGiveFeedbackCommandBehavior<object>
    { }
	#endregion

	#region MapControlPreviewDragEnterCommand
	// MapControlPreviewDragEnterCommand
	public class MapControlPreviewDragEnterCommand : MapControlCommandBase<MapControlPreviewDragEnterCommandBehavior>
	{ }

    public class MapControlPreviewDragEnterCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlPreviewDragEnterCommandBehavior<T>,new()
    { }

    public class MapControlPreviewDragEnterCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, DragEventArgs>
    {
        public MapControlPreviewDragEnterCommandBehavior(Func<object, DragEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlPreviewDragEnterCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewDragEnter += OnEventRaised;
        }
    }

	// MapControlPreviewDragEnterCommandBehavior
    public class MapControlPreviewDragEnterCommandBehavior : MapControlPreviewDragEnterCommandBehavior<object>
    { }
	#endregion

	#region MapControlDragEnterCommand
	// MapControlDragEnterCommand
	public class MapControlDragEnterCommand : MapControlCommandBase<MapControlDragEnterCommandBehavior>
	{ }

    public class MapControlDragEnterCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlDragEnterCommandBehavior<T>,new()
    { }

    public class MapControlDragEnterCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, DragEventArgs>
    {
        public MapControlDragEnterCommandBehavior(Func<object, DragEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlDragEnterCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.DragEnter += OnEventRaised;
        }
    }

	// MapControlDragEnterCommandBehavior
    public class MapControlDragEnterCommandBehavior : MapControlDragEnterCommandBehavior<object>
    { }
	#endregion

	#region MapControlPreviewDragOverCommand
	// MapControlPreviewDragOverCommand
	public class MapControlPreviewDragOverCommand : MapControlCommandBase<MapControlPreviewDragOverCommandBehavior>
	{ }

    public class MapControlPreviewDragOverCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlPreviewDragOverCommandBehavior<T>,new()
    { }

    public class MapControlPreviewDragOverCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, DragEventArgs>
    {
        public MapControlPreviewDragOverCommandBehavior(Func<object, DragEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlPreviewDragOverCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewDragOver += OnEventRaised;
        }
    }

	// MapControlPreviewDragOverCommandBehavior
    public class MapControlPreviewDragOverCommandBehavior : MapControlPreviewDragOverCommandBehavior<object>
    { }
	#endregion

	#region MapControlDragOverCommand
	// MapControlDragOverCommand
	public class MapControlDragOverCommand : MapControlCommandBase<MapControlDragOverCommandBehavior>
	{ }

    public class MapControlDragOverCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlDragOverCommandBehavior<T>,new()
    { }

    public class MapControlDragOverCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, DragEventArgs>
    {
        public MapControlDragOverCommandBehavior(Func<object, DragEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlDragOverCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.DragOver += OnEventRaised;
        }
    }

	// MapControlDragOverCommandBehavior
    public class MapControlDragOverCommandBehavior : MapControlDragOverCommandBehavior<object>
    { }
	#endregion

	#region MapControlPreviewDragLeaveCommand
	// MapControlPreviewDragLeaveCommand
	public class MapControlPreviewDragLeaveCommand : MapControlCommandBase<MapControlPreviewDragLeaveCommandBehavior>
	{ }

    public class MapControlPreviewDragLeaveCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlPreviewDragLeaveCommandBehavior<T>,new()
    { }

    public class MapControlPreviewDragLeaveCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, DragEventArgs>
    {
        public MapControlPreviewDragLeaveCommandBehavior(Func<object, DragEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlPreviewDragLeaveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewDragLeave += OnEventRaised;
        }
    }

	// MapControlPreviewDragLeaveCommandBehavior
    public class MapControlPreviewDragLeaveCommandBehavior : MapControlPreviewDragLeaveCommandBehavior<object>
    { }
	#endregion

	#region MapControlDragLeaveCommand
	// MapControlDragLeaveCommand
	public class MapControlDragLeaveCommand : MapControlCommandBase<MapControlDragLeaveCommandBehavior>
	{ }

    public class MapControlDragLeaveCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlDragLeaveCommandBehavior<T>,new()
    { }

    public class MapControlDragLeaveCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, DragEventArgs>
    {
        public MapControlDragLeaveCommandBehavior(Func<object, DragEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlDragLeaveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.DragLeave += OnEventRaised;
        }
    }

	// MapControlDragLeaveCommandBehavior
    public class MapControlDragLeaveCommandBehavior : MapControlDragLeaveCommandBehavior<object>
    { }
	#endregion

	#region MapControlPreviewDropCommand
	// MapControlPreviewDropCommand
	public class MapControlPreviewDropCommand : MapControlCommandBase<MapControlPreviewDropCommandBehavior>
	{ }

    public class MapControlPreviewDropCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlPreviewDropCommandBehavior<T>,new()
    { }

    public class MapControlPreviewDropCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, DragEventArgs>
    {
        public MapControlPreviewDropCommandBehavior(Func<object, DragEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlPreviewDropCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewDrop += OnEventRaised;
        }
    }

	// MapControlPreviewDropCommandBehavior
    public class MapControlPreviewDropCommandBehavior : MapControlPreviewDropCommandBehavior<object>
    { }
	#endregion

	#region MapControlDropCommand
	// MapControlDropCommand
	public class MapControlDropCommand : MapControlCommandBase<MapControlDropCommandBehavior>
	{ }

    public class MapControlDropCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlDropCommandBehavior<T>,new()
    { }

    public class MapControlDropCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, DragEventArgs>
    {
        public MapControlDropCommandBehavior(Func<object, DragEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlDropCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Drop += OnEventRaised;
        }
    }

	// MapControlDropCommandBehavior
    public class MapControlDropCommandBehavior : MapControlDropCommandBehavior<object>
    { }
	#endregion

#if SyncfusionFramework4_0

	#region MapControlPreviewTouchDownCommand
	// MapControlPreviewTouchDownCommand
	public class MapControlPreviewTouchDownCommand : MapControlCommandBase<MapControlPreviewTouchDownCommandBehavior>
	{ }

    public class MapControlPreviewTouchDownCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlPreviewTouchDownCommandBehavior<T>,new()
    { }

    public class MapControlPreviewTouchDownCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, TouchEventArgs>
    {
        public MapControlPreviewTouchDownCommandBehavior(Func<object, TouchEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlPreviewTouchDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewTouchDown += OnEventRaised;
        }
    }

	// MapControlPreviewTouchDownCommandBehavior
    public class MapControlPreviewTouchDownCommandBehavior : MapControlPreviewTouchDownCommandBehavior<object>
    { }
	#endregion

	#region MapControlTouchDownCommand
	// MapControlTouchDownCommand
	public class MapControlTouchDownCommand : MapControlCommandBase<MapControlTouchDownCommandBehavior>
	{ }

    public class MapControlTouchDownCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlTouchDownCommandBehavior<T>,new()
    { }

    public class MapControlTouchDownCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, TouchEventArgs>
    {
        public MapControlTouchDownCommandBehavior(Func<object, TouchEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlTouchDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.TouchDown += OnEventRaised;
        }
    }

	// MapControlTouchDownCommandBehavior
    public class MapControlTouchDownCommandBehavior : MapControlTouchDownCommandBehavior<object>
    { }
	#endregion

	#region MapControlPreviewTouchMoveCommand
	// MapControlPreviewTouchMoveCommand
	public class MapControlPreviewTouchMoveCommand : MapControlCommandBase<MapControlPreviewTouchMoveCommandBehavior>
	{ }

    public class MapControlPreviewTouchMoveCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlPreviewTouchMoveCommandBehavior<T>,new()
    { }

    public class MapControlPreviewTouchMoveCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, TouchEventArgs>
    {
        public MapControlPreviewTouchMoveCommandBehavior(Func<object, TouchEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlPreviewTouchMoveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewTouchMove += OnEventRaised;
        }
    }

	// MapControlPreviewTouchMoveCommandBehavior
    public class MapControlPreviewTouchMoveCommandBehavior : MapControlPreviewTouchMoveCommandBehavior<object>
    { }
	#endregion

	#region MapControlTouchMoveCommand
	// MapControlTouchMoveCommand
	public class MapControlTouchMoveCommand : MapControlCommandBase<MapControlTouchMoveCommandBehavior>
	{ }

    public class MapControlTouchMoveCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlTouchMoveCommandBehavior<T>,new()
    { }

    public class MapControlTouchMoveCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, TouchEventArgs>
    {
        public MapControlTouchMoveCommandBehavior(Func<object, TouchEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlTouchMoveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.TouchMove += OnEventRaised;
        }
    }

	// MapControlTouchMoveCommandBehavior
    public class MapControlTouchMoveCommandBehavior : MapControlTouchMoveCommandBehavior<object>
    { }
	#endregion

	#region MapControlPreviewTouchUpCommand
	// MapControlPreviewTouchUpCommand
	public class MapControlPreviewTouchUpCommand : MapControlCommandBase<MapControlPreviewTouchUpCommandBehavior>
	{ }

    public class MapControlPreviewTouchUpCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlPreviewTouchUpCommandBehavior<T>,new()
    { }

    public class MapControlPreviewTouchUpCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, TouchEventArgs>
    {
        public MapControlPreviewTouchUpCommandBehavior(Func<object, TouchEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlPreviewTouchUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewTouchUp += OnEventRaised;
        }
    }

	// MapControlPreviewTouchUpCommandBehavior
    public class MapControlPreviewTouchUpCommandBehavior : MapControlPreviewTouchUpCommandBehavior<object>
    { }
	#endregion

	#region MapControlTouchUpCommand
	// MapControlTouchUpCommand
	public class MapControlTouchUpCommand : MapControlCommandBase<MapControlTouchUpCommandBehavior>
	{ }

    public class MapControlTouchUpCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlTouchUpCommandBehavior<T>,new()
    { }

    public class MapControlTouchUpCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, TouchEventArgs>
    {
        public MapControlTouchUpCommandBehavior(Func<object, TouchEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlTouchUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.TouchUp += OnEventRaised;
        }
    }

	// MapControlTouchUpCommandBehavior
    public class MapControlTouchUpCommandBehavior : MapControlTouchUpCommandBehavior<object>
    { }
	#endregion

	#region MapControlGotTouchCaptureCommand
	// MapControlGotTouchCaptureCommand
	public class MapControlGotTouchCaptureCommand : MapControlCommandBase<MapControlGotTouchCaptureCommandBehavior>
	{ }

    public class MapControlGotTouchCaptureCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlGotTouchCaptureCommandBehavior<T>,new()
    { }

    public class MapControlGotTouchCaptureCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, TouchEventArgs>
    {
        public MapControlGotTouchCaptureCommandBehavior(Func<object, TouchEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlGotTouchCaptureCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.GotTouchCapture += OnEventRaised;
        }
    }

	// MapControlGotTouchCaptureCommandBehavior
    public class MapControlGotTouchCaptureCommandBehavior : MapControlGotTouchCaptureCommandBehavior<object>
    { }
	#endregion

	#region MapControlLostTouchCaptureCommand
	// MapControlLostTouchCaptureCommand
	public class MapControlLostTouchCaptureCommand : MapControlCommandBase<MapControlLostTouchCaptureCommandBehavior>
	{ }

    public class MapControlLostTouchCaptureCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlLostTouchCaptureCommandBehavior<T>,new()
    { }

    public class MapControlLostTouchCaptureCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, TouchEventArgs>
    {
        public MapControlLostTouchCaptureCommandBehavior(Func<object, TouchEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlLostTouchCaptureCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.LostTouchCapture += OnEventRaised;
        }
    }

	// MapControlLostTouchCaptureCommandBehavior
    public class MapControlLostTouchCaptureCommandBehavior : MapControlLostTouchCaptureCommandBehavior<object>
    { }
	#endregion

	#region MapControlTouchEnterCommand
	// MapControlTouchEnterCommand
	public class MapControlTouchEnterCommand : MapControlCommandBase<MapControlTouchEnterCommandBehavior>
	{ }

    public class MapControlTouchEnterCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlTouchEnterCommandBehavior<T>,new()
    { }

    public class MapControlTouchEnterCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, TouchEventArgs>
    {
        public MapControlTouchEnterCommandBehavior(Func<object, TouchEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlTouchEnterCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.TouchEnter += OnEventRaised;
        }
    }

	// MapControlTouchEnterCommandBehavior
    public class MapControlTouchEnterCommandBehavior : MapControlTouchEnterCommandBehavior<object>
    { }
	#endregion

	#region MapControlTouchLeaveCommand
	// MapControlTouchLeaveCommand
	public class MapControlTouchLeaveCommand : MapControlCommandBase<MapControlTouchLeaveCommandBehavior>
	{ }

    public class MapControlTouchLeaveCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlTouchLeaveCommandBehavior<T>,new()
    { }

    public class MapControlTouchLeaveCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, TouchEventArgs>
    {
        public MapControlTouchLeaveCommandBehavior(Func<object, TouchEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlTouchLeaveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.TouchLeave += OnEventRaised;
        }
    }

	// MapControlTouchLeaveCommandBehavior
    public class MapControlTouchLeaveCommandBehavior : MapControlTouchLeaveCommandBehavior<object>
    { }
	#endregion

#endif

	#region MapControlIsMouseDirectlyOverChangedCommand
	// MapControlIsMouseDirectlyOverChangedCommand
	public class MapControlIsMouseDirectlyOverChangedCommand : MapControlCommandBase<MapControlIsMouseDirectlyOverChangedCommandBehavior>
	{ }

    public class MapControlIsMouseDirectlyOverChangedCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlIsMouseDirectlyOverChangedCommandBehavior<T>,new()
    { }

    public class MapControlIsMouseDirectlyOverChangedCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public MapControlIsMouseDirectlyOverChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlIsMouseDirectlyOverChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsMouseDirectlyOverChanged += OnEventRaised;
        }
    }

	// MapControlIsMouseDirectlyOverChangedCommandBehavior
    public class MapControlIsMouseDirectlyOverChangedCommandBehavior : MapControlIsMouseDirectlyOverChangedCommandBehavior<object>
    { }
	#endregion

	#region MapControlIsKeyboardFocusWithinChangedCommand
	// MapControlIsKeyboardFocusWithinChangedCommand
	public class MapControlIsKeyboardFocusWithinChangedCommand : MapControlCommandBase<MapControlIsKeyboardFocusWithinChangedCommandBehavior>
	{ }

    public class MapControlIsKeyboardFocusWithinChangedCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlIsKeyboardFocusWithinChangedCommandBehavior<T>,new()
    { }

    public class MapControlIsKeyboardFocusWithinChangedCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public MapControlIsKeyboardFocusWithinChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlIsKeyboardFocusWithinChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsKeyboardFocusWithinChanged += OnEventRaised;
        }
    }

	// MapControlIsKeyboardFocusWithinChangedCommandBehavior
    public class MapControlIsKeyboardFocusWithinChangedCommandBehavior : MapControlIsKeyboardFocusWithinChangedCommandBehavior<object>
    { }
	#endregion

	#region MapControlIsMouseCapturedChangedCommand
	// MapControlIsMouseCapturedChangedCommand
	public class MapControlIsMouseCapturedChangedCommand : MapControlCommandBase<MapControlIsMouseCapturedChangedCommandBehavior>
	{ }

    public class MapControlIsMouseCapturedChangedCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlIsMouseCapturedChangedCommandBehavior<T>,new()
    { }

    public class MapControlIsMouseCapturedChangedCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public MapControlIsMouseCapturedChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlIsMouseCapturedChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsMouseCapturedChanged += OnEventRaised;
        }
    }

	// MapControlIsMouseCapturedChangedCommandBehavior
    public class MapControlIsMouseCapturedChangedCommandBehavior : MapControlIsMouseCapturedChangedCommandBehavior<object>
    { }
	#endregion

	#region MapControlIsMouseCaptureWithinChangedCommand
	// MapControlIsMouseCaptureWithinChangedCommand
	public class MapControlIsMouseCaptureWithinChangedCommand : MapControlCommandBase<MapControlIsMouseCaptureWithinChangedCommandBehavior>
	{ }

    public class MapControlIsMouseCaptureWithinChangedCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlIsMouseCaptureWithinChangedCommandBehavior<T>,new()
    { }

    public class MapControlIsMouseCaptureWithinChangedCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public MapControlIsMouseCaptureWithinChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlIsMouseCaptureWithinChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsMouseCaptureWithinChanged += OnEventRaised;
        }
    }

	// MapControlIsMouseCaptureWithinChangedCommandBehavior
    public class MapControlIsMouseCaptureWithinChangedCommandBehavior : MapControlIsMouseCaptureWithinChangedCommandBehavior<object>
    { }
	#endregion

	#region MapControlIsStylusDirectlyOverChangedCommand
	// MapControlIsStylusDirectlyOverChangedCommand
	public class MapControlIsStylusDirectlyOverChangedCommand : MapControlCommandBase<MapControlIsStylusDirectlyOverChangedCommandBehavior>
	{ }

    public class MapControlIsStylusDirectlyOverChangedCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlIsStylusDirectlyOverChangedCommandBehavior<T>,new()
    { }

    public class MapControlIsStylusDirectlyOverChangedCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public MapControlIsStylusDirectlyOverChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlIsStylusDirectlyOverChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsStylusDirectlyOverChanged += OnEventRaised;
        }
    }

	// MapControlIsStylusDirectlyOverChangedCommandBehavior
    public class MapControlIsStylusDirectlyOverChangedCommandBehavior : MapControlIsStylusDirectlyOverChangedCommandBehavior<object>
    { }
	#endregion

	#region MapControlIsStylusCapturedChangedCommand
	// MapControlIsStylusCapturedChangedCommand
	public class MapControlIsStylusCapturedChangedCommand : MapControlCommandBase<MapControlIsStylusCapturedChangedCommandBehavior>
	{ }

    public class MapControlIsStylusCapturedChangedCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlIsStylusCapturedChangedCommandBehavior<T>,new()
    { }

    public class MapControlIsStylusCapturedChangedCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public MapControlIsStylusCapturedChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlIsStylusCapturedChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsStylusCapturedChanged += OnEventRaised;
        }
    }

	// MapControlIsStylusCapturedChangedCommandBehavior
    public class MapControlIsStylusCapturedChangedCommandBehavior : MapControlIsStylusCapturedChangedCommandBehavior<object>
    { }
	#endregion

	#region MapControlIsStylusCaptureWithinChangedCommand
	// MapControlIsStylusCaptureWithinChangedCommand
	public class MapControlIsStylusCaptureWithinChangedCommand : MapControlCommandBase<MapControlIsStylusCaptureWithinChangedCommandBehavior>
	{ }

    public class MapControlIsStylusCaptureWithinChangedCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlIsStylusCaptureWithinChangedCommandBehavior<T>,new()
    { }

    public class MapControlIsStylusCaptureWithinChangedCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public MapControlIsStylusCaptureWithinChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlIsStylusCaptureWithinChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsStylusCaptureWithinChanged += OnEventRaised;
        }
    }

	// MapControlIsStylusCaptureWithinChangedCommandBehavior
    public class MapControlIsStylusCaptureWithinChangedCommandBehavior : MapControlIsStylusCaptureWithinChangedCommandBehavior<object>
    { }
	#endregion

	#region MapControlIsKeyboardFocusedChangedCommand
	// MapControlIsKeyboardFocusedChangedCommand
	public class MapControlIsKeyboardFocusedChangedCommand : MapControlCommandBase<MapControlIsKeyboardFocusedChangedCommandBehavior>
	{ }

    public class MapControlIsKeyboardFocusedChangedCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlIsKeyboardFocusedChangedCommandBehavior<T>,new()
    { }

    public class MapControlIsKeyboardFocusedChangedCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public MapControlIsKeyboardFocusedChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlIsKeyboardFocusedChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsKeyboardFocusedChanged += OnEventRaised;
        }
    }

	// MapControlIsKeyboardFocusedChangedCommandBehavior
    public class MapControlIsKeyboardFocusedChangedCommandBehavior : MapControlIsKeyboardFocusedChangedCommandBehavior<object>
    { }
	#endregion

	#region MapControlLayoutUpdatedCommand
	// MapControlLayoutUpdatedCommand
	public class MapControlLayoutUpdatedCommand : MapControlCommandBase<MapControlLayoutUpdatedCommandBehavior>
	{ }

    public class MapControlLayoutUpdatedCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlLayoutUpdatedCommandBehavior<T>,new()
    { }

    public class MapControlLayoutUpdatedCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, EventArgs>
    {
        public MapControlLayoutUpdatedCommandBehavior(Func<object, EventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlLayoutUpdatedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.LayoutUpdated += OnEventRaised;
        }
    }

	// MapControlLayoutUpdatedCommandBehavior
    public class MapControlLayoutUpdatedCommandBehavior : MapControlLayoutUpdatedCommandBehavior<object>
    { }
	#endregion

	#region MapControlGotFocusCommand
	// MapControlGotFocusCommand
	public class MapControlGotFocusCommand : MapControlCommandBase<MapControlGotFocusCommandBehavior>
	{ }

    public class MapControlGotFocusCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlGotFocusCommandBehavior<T>,new()
    { }

    public class MapControlGotFocusCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, RoutedEventArgs>
    {
        public MapControlGotFocusCommandBehavior(Func<object, RoutedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlGotFocusCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.GotFocus += OnEventRaised;
        }
    }

	// MapControlGotFocusCommandBehavior
    public class MapControlGotFocusCommandBehavior : MapControlGotFocusCommandBehavior<object>
    { }
	#endregion

	#region MapControlLostFocusCommand
	// MapControlLostFocusCommand
	public class MapControlLostFocusCommand : MapControlCommandBase<MapControlLostFocusCommandBehavior>
	{ }

    public class MapControlLostFocusCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlLostFocusCommandBehavior<T>,new()
    { }

    public class MapControlLostFocusCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, RoutedEventArgs>
    {
        public MapControlLostFocusCommandBehavior(Func<object, RoutedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlLostFocusCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.LostFocus += OnEventRaised;
        }
    }

	// MapControlLostFocusCommandBehavior
    public class MapControlLostFocusCommandBehavior : MapControlLostFocusCommandBehavior<object>
    { }
	#endregion

	#region MapControlIsEnabledChangedCommand
	// MapControlIsEnabledChangedCommand
	public class MapControlIsEnabledChangedCommand : MapControlCommandBase<MapControlIsEnabledChangedCommandBehavior>
	{ }

    public class MapControlIsEnabledChangedCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlIsEnabledChangedCommandBehavior<T>,new()
    { }

    public class MapControlIsEnabledChangedCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public MapControlIsEnabledChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlIsEnabledChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsEnabledChanged += OnEventRaised;
        }
    }

	// MapControlIsEnabledChangedCommandBehavior
    public class MapControlIsEnabledChangedCommandBehavior : MapControlIsEnabledChangedCommandBehavior<object>
    { }
	#endregion

	#region MapControlIsHitTestVisibleChangedCommand
	// MapControlIsHitTestVisibleChangedCommand
	public class MapControlIsHitTestVisibleChangedCommand : MapControlCommandBase<MapControlIsHitTestVisibleChangedCommandBehavior>
	{ }

    public class MapControlIsHitTestVisibleChangedCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlIsHitTestVisibleChangedCommandBehavior<T>,new()
    { }

    public class MapControlIsHitTestVisibleChangedCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public MapControlIsHitTestVisibleChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlIsHitTestVisibleChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsHitTestVisibleChanged += OnEventRaised;
        }
    }

	// MapControlIsHitTestVisibleChangedCommandBehavior
    public class MapControlIsHitTestVisibleChangedCommandBehavior : MapControlIsHitTestVisibleChangedCommandBehavior<object>
    { }
	#endregion

	#region MapControlIsVisibleChangedCommand
	// MapControlIsVisibleChangedCommand
	public class MapControlIsVisibleChangedCommand : MapControlCommandBase<MapControlIsVisibleChangedCommandBehavior>
	{ }

    public class MapControlIsVisibleChangedCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlIsVisibleChangedCommandBehavior<T>,new()
    { }

    public class MapControlIsVisibleChangedCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public MapControlIsVisibleChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlIsVisibleChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsVisibleChanged += OnEventRaised;
        }
    }

	// MapControlIsVisibleChangedCommandBehavior
    public class MapControlIsVisibleChangedCommandBehavior : MapControlIsVisibleChangedCommandBehavior<object>
    { }
	#endregion

	#region MapControlFocusableChangedCommand
	// MapControlFocusableChangedCommand
	public class MapControlFocusableChangedCommand : MapControlCommandBase<MapControlFocusableChangedCommandBehavior>
	{ }

    public class MapControlFocusableChangedCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlFocusableChangedCommandBehavior<T>,new()
    { }

    public class MapControlFocusableChangedCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public MapControlFocusableChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlFocusableChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.FocusableChanged += OnEventRaised;
        }
    }

	// MapControlFocusableChangedCommandBehavior
    public class MapControlFocusableChangedCommandBehavior : MapControlFocusableChangedCommandBehavior<object>
    { }
	#endregion

#if SyncfusionFramework4_0

	#region MapControlManipulationStartingCommand
	// MapControlManipulationStartingCommand
	public class MapControlManipulationStartingCommand : MapControlCommandBase<MapControlManipulationStartingCommandBehavior>
	{ }

    public class MapControlManipulationStartingCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlManipulationStartingCommandBehavior<T>,new()
    { }

    public class MapControlManipulationStartingCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, ManipulationStartingEventArgs>
    {
        public MapControlManipulationStartingCommandBehavior(Func<object, ManipulationStartingEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlManipulationStartingCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ManipulationStarting += OnEventRaised;
        }
    }

	// MapControlManipulationStartingCommandBehavior
    public class MapControlManipulationStartingCommandBehavior : MapControlManipulationStartingCommandBehavior<object>
    { }
	#endregion

	#region MapControlManipulationStartedCommand
	// MapControlManipulationStartedCommand
	public class MapControlManipulationStartedCommand : MapControlCommandBase<MapControlManipulationStartedCommandBehavior>
	{ }

    public class MapControlManipulationStartedCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlManipulationStartedCommandBehavior<T>,new()
    { }

    public class MapControlManipulationStartedCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, ManipulationStartedEventArgs>
    {
        public MapControlManipulationStartedCommandBehavior(Func<object, ManipulationStartedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlManipulationStartedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ManipulationStarted += OnEventRaised;
        }
    }

	// MapControlManipulationStartedCommandBehavior
    public class MapControlManipulationStartedCommandBehavior : MapControlManipulationStartedCommandBehavior<object>
    { }
	#endregion

	#region MapControlManipulationDeltaCommand
	// MapControlManipulationDeltaCommand
	public class MapControlManipulationDeltaCommand : MapControlCommandBase<MapControlManipulationDeltaCommandBehavior>
	{ }

    public class MapControlManipulationDeltaCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlManipulationDeltaCommandBehavior<T>,new()
    { }

    public class MapControlManipulationDeltaCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, ManipulationDeltaEventArgs>
    {
        public MapControlManipulationDeltaCommandBehavior(Func<object, ManipulationDeltaEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlManipulationDeltaCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ManipulationDelta += OnEventRaised;
        }
    }

	// MapControlManipulationDeltaCommandBehavior
    public class MapControlManipulationDeltaCommandBehavior : MapControlManipulationDeltaCommandBehavior<object>
    { }
	#endregion

	#region MapControlManipulationInertiaStartingCommand
	// MapControlManipulationInertiaStartingCommand
	public class MapControlManipulationInertiaStartingCommand : MapControlCommandBase<MapControlManipulationInertiaStartingCommandBehavior>
	{ }

    public class MapControlManipulationInertiaStartingCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlManipulationInertiaStartingCommandBehavior<T>,new()
    { }

    public class MapControlManipulationInertiaStartingCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, ManipulationInertiaStartingEventArgs>
    {
        public MapControlManipulationInertiaStartingCommandBehavior(Func<object, ManipulationInertiaStartingEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlManipulationInertiaStartingCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ManipulationInertiaStarting += OnEventRaised;
        }
    }

	// MapControlManipulationInertiaStartingCommandBehavior
    public class MapControlManipulationInertiaStartingCommandBehavior : MapControlManipulationInertiaStartingCommandBehavior<object>
    { }
	#endregion

	#region MapControlManipulationBoundaryFeedbackCommand
	// MapControlManipulationBoundaryFeedbackCommand
	public class MapControlManipulationBoundaryFeedbackCommand : MapControlCommandBase<MapControlManipulationBoundaryFeedbackCommandBehavior>
	{ }

    public class MapControlManipulationBoundaryFeedbackCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlManipulationBoundaryFeedbackCommandBehavior<T>,new()
    { }

    public class MapControlManipulationBoundaryFeedbackCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, ManipulationBoundaryFeedbackEventArgs>
    {
        public MapControlManipulationBoundaryFeedbackCommandBehavior(Func<object, ManipulationBoundaryFeedbackEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlManipulationBoundaryFeedbackCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ManipulationBoundaryFeedback += OnEventRaised;
        }
    }

	// MapControlManipulationBoundaryFeedbackCommandBehavior
    public class MapControlManipulationBoundaryFeedbackCommandBehavior : MapControlManipulationBoundaryFeedbackCommandBehavior<object>
    { }
	#endregion

	#region MapControlManipulationCompletedCommand
	// MapControlManipulationCompletedCommand
	public class MapControlManipulationCompletedCommand : MapControlCommandBase<MapControlManipulationCompletedCommandBehavior>
	{ }

    public class MapControlManipulationCompletedCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlManipulationCompletedCommandBehavior<T>,new()
    { }

    public class MapControlManipulationCompletedCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, ManipulationCompletedEventArgs>
    {
        public MapControlManipulationCompletedCommandBehavior(Func<object, ManipulationCompletedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlManipulationCompletedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ManipulationCompleted += OnEventRaised;
        }
    }

	// MapControlManipulationCompletedCommandBehavior
    public class MapControlManipulationCompletedCommandBehavior : MapControlManipulationCompletedCommandBehavior<object>
    { }
	#endregion
#endif
}


