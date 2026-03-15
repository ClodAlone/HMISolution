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

	#region ImageryLayerPreviewZoomInCommand
	// ImageryLayerPreviewZoomInCommand
	public class ImageryLayerPreviewZoomInCommand : ImageryLayerCommandBase<ImageryLayerPreviewZoomInCommandBehavior>
	{ }

    public class ImageryLayerPreviewZoomInCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerPreviewZoomInCommandBehavior<T>,new()
    { }

    public class ImageryLayerPreviewZoomInCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, ZoomEventArgs>
    {
        public ImageryLayerPreviewZoomInCommandBehavior(Func<object, ZoomEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerPreviewZoomInCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewZoomIn += OnEventRaised;
        }
    }

	// ImageryLayerPreviewZoomInCommandBehavior
    public class ImageryLayerPreviewZoomInCommandBehavior : ImageryLayerPreviewZoomInCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerZoomedInCommand
	// ImageryLayerZoomedInCommand
	public class ImageryLayerZoomedInCommand : ImageryLayerCommandBase<ImageryLayerZoomedInCommandBehavior>
	{ }

    public class ImageryLayerZoomedInCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerZoomedInCommandBehavior<T>,new()
    { }

    public class ImageryLayerZoomedInCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, ZoomEventArgs>
    {
        public ImageryLayerZoomedInCommandBehavior(Func<object, ZoomEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerZoomedInCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ZoomedIn += OnEventRaised;
        }
    }

	// ImageryLayerZoomedInCommandBehavior
    public class ImageryLayerZoomedInCommandBehavior : ImageryLayerZoomedInCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerPreviewZoomOutCommand
	// ImageryLayerPreviewZoomOutCommand
	public class ImageryLayerPreviewZoomOutCommand : ImageryLayerCommandBase<ImageryLayerPreviewZoomOutCommandBehavior>
	{ }

    public class ImageryLayerPreviewZoomOutCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerPreviewZoomOutCommandBehavior<T>,new()
    { }

    public class ImageryLayerPreviewZoomOutCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, ZoomEventArgs>
    {
        public ImageryLayerPreviewZoomOutCommandBehavior(Func<object, ZoomEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerPreviewZoomOutCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewZoomOut += OnEventRaised;
        }
    }

	// ImageryLayerPreviewZoomOutCommandBehavior
    public class ImageryLayerPreviewZoomOutCommandBehavior : ImageryLayerPreviewZoomOutCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerZoomedOutCommand
	// ImageryLayerZoomedOutCommand
	public class ImageryLayerZoomedOutCommand : ImageryLayerCommandBase<ImageryLayerZoomedOutCommandBehavior>
	{ }

    public class ImageryLayerZoomedOutCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerZoomedOutCommandBehavior<T>,new()
    { }

    public class ImageryLayerZoomedOutCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, ZoomEventArgs>
    {
        public ImageryLayerZoomedOutCommandBehavior(Func<object, ZoomEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerZoomedOutCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ZoomedOut += OnEventRaised;
        }
    }

	// ImageryLayerZoomedOutCommandBehavior
    public class ImageryLayerZoomedOutCommandBehavior : ImageryLayerZoomedOutCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerPanningCommand
	// ImageryLayerPanningCommand
	public class ImageryLayerPanningCommand : ImageryLayerCommandBase<ImageryLayerPanningCommandBehavior>
	{ }

    public class ImageryLayerPanningCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerPanningCommandBehavior<T>,new()
    { }

    public class ImageryLayerPanningCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, PanEventArgs>
    {
        public ImageryLayerPanningCommandBehavior(Func<object, PanEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerPanningCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Panning += OnEventRaised;
        }
    }

	// ImageryLayerPanningCommandBehavior
    public class ImageryLayerPanningCommandBehavior : ImageryLayerPanningCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerPannedCommand
	// ImageryLayerPannedCommand
	public class ImageryLayerPannedCommand : ImageryLayerCommandBase<ImageryLayerPannedCommandBehavior>
	{ }

    public class ImageryLayerPannedCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerPannedCommandBehavior<T>,new()
    { }

    public class ImageryLayerPannedCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, PanEventArgs>
    {
        public ImageryLayerPannedCommandBehavior(Func<object, PanEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerPannedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Panned += OnEventRaised;
        }
    }

	// ImageryLayerPannedCommandBehavior
    public class ImageryLayerPannedCommandBehavior : ImageryLayerPannedCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerPreviewMouseDoubleClickCommand
	// ImageryLayerPreviewMouseDoubleClickCommand
	public class ImageryLayerPreviewMouseDoubleClickCommand : ImageryLayerCommandBase<ImageryLayerPreviewMouseDoubleClickCommandBehavior>
	{ }

    public class ImageryLayerPreviewMouseDoubleClickCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerPreviewMouseDoubleClickCommandBehavior<T>,new()
    { }

    public class ImageryLayerPreviewMouseDoubleClickCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public ImageryLayerPreviewMouseDoubleClickCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerPreviewMouseDoubleClickCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewMouseDoubleClick += OnEventRaised;
        }
    }

	// ImageryLayerPreviewMouseDoubleClickCommandBehavior
    public class ImageryLayerPreviewMouseDoubleClickCommandBehavior : ImageryLayerPreviewMouseDoubleClickCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerMouseDoubleClickCommand
	// ImageryLayerMouseDoubleClickCommand
	public class ImageryLayerMouseDoubleClickCommand : ImageryLayerCommandBase<ImageryLayerMouseDoubleClickCommandBehavior>
	{ }

    public class ImageryLayerMouseDoubleClickCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerMouseDoubleClickCommandBehavior<T>,new()
    { }

    public class ImageryLayerMouseDoubleClickCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public ImageryLayerMouseDoubleClickCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerMouseDoubleClickCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseDoubleClick += OnEventRaised;
        }
    }

	// ImageryLayerMouseDoubleClickCommandBehavior
    public class ImageryLayerMouseDoubleClickCommandBehavior : ImageryLayerMouseDoubleClickCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerTargetUpdatedCommand
	// ImageryLayerTargetUpdatedCommand
	public class ImageryLayerTargetUpdatedCommand : ImageryLayerCommandBase<ImageryLayerTargetUpdatedCommandBehavior>
	{ }

    public class ImageryLayerTargetUpdatedCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerTargetUpdatedCommandBehavior<T>,new()
    { }

    public class ImageryLayerTargetUpdatedCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, DataTransferEventArgs>
    {
        public ImageryLayerTargetUpdatedCommandBehavior(Func<object, DataTransferEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerTargetUpdatedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.TargetUpdated += OnEventRaised;
        }
    }

	// ImageryLayerTargetUpdatedCommandBehavior
    public class ImageryLayerTargetUpdatedCommandBehavior : ImageryLayerTargetUpdatedCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerSourceUpdatedCommand
	// ImageryLayerSourceUpdatedCommand
	public class ImageryLayerSourceUpdatedCommand : ImageryLayerCommandBase<ImageryLayerSourceUpdatedCommandBehavior>
	{ }

    public class ImageryLayerSourceUpdatedCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerSourceUpdatedCommandBehavior<T>,new()
    { }

    public class ImageryLayerSourceUpdatedCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, DataTransferEventArgs>
    {
        public ImageryLayerSourceUpdatedCommandBehavior(Func<object, DataTransferEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerSourceUpdatedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.SourceUpdated += OnEventRaised;
        }
    }

	// ImageryLayerSourceUpdatedCommandBehavior
    public class ImageryLayerSourceUpdatedCommandBehavior : ImageryLayerSourceUpdatedCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerDataContextChangedCommand
	// ImageryLayerDataContextChangedCommand
	public class ImageryLayerDataContextChangedCommand : ImageryLayerCommandBase<ImageryLayerDataContextChangedCommandBehavior>
	{ }

    public class ImageryLayerDataContextChangedCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerDataContextChangedCommandBehavior<T>,new()
    { }

    public class ImageryLayerDataContextChangedCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public ImageryLayerDataContextChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerDataContextChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.DataContextChanged += OnEventRaised;
        }
    }

	// ImageryLayerDataContextChangedCommandBehavior
    public class ImageryLayerDataContextChangedCommandBehavior : ImageryLayerDataContextChangedCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerRequestBringIntoViewCommand
	// ImageryLayerRequestBringIntoViewCommand
	public class ImageryLayerRequestBringIntoViewCommand : ImageryLayerCommandBase<ImageryLayerRequestBringIntoViewCommandBehavior>
	{ }

    public class ImageryLayerRequestBringIntoViewCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerRequestBringIntoViewCommandBehavior<T>,new()
    { }

    public class ImageryLayerRequestBringIntoViewCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, RequestBringIntoViewEventArgs>
    {
        public ImageryLayerRequestBringIntoViewCommandBehavior(Func<object, RequestBringIntoViewEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerRequestBringIntoViewCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.RequestBringIntoView += OnEventRaised;
        }
    }

	// ImageryLayerRequestBringIntoViewCommandBehavior
    public class ImageryLayerRequestBringIntoViewCommandBehavior : ImageryLayerRequestBringIntoViewCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerSizeChangedCommand
	// ImageryLayerSizeChangedCommand
	public class ImageryLayerSizeChangedCommand : ImageryLayerCommandBase<ImageryLayerSizeChangedCommandBehavior>
	{ }

    public class ImageryLayerSizeChangedCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerSizeChangedCommandBehavior<T>,new()
    { }

    public class ImageryLayerSizeChangedCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, SizeChangedEventArgs>
    {
        public ImageryLayerSizeChangedCommandBehavior(Func<object, SizeChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerSizeChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.SizeChanged += OnEventRaised;
        }
    }

	// ImageryLayerSizeChangedCommandBehavior
    public class ImageryLayerSizeChangedCommandBehavior : ImageryLayerSizeChangedCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerInitializedCommand
	// ImageryLayerInitializedCommand
	public class ImageryLayerInitializedCommand : ImageryLayerCommandBase<ImageryLayerInitializedCommandBehavior>
	{ }

    public class ImageryLayerInitializedCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerInitializedCommandBehavior<T>,new()
    { }

    public class ImageryLayerInitializedCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, EventArgs>
    {
        public ImageryLayerInitializedCommandBehavior(Func<object, EventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerInitializedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Initialized += OnEventRaised;
        }
    }

	// ImageryLayerInitializedCommandBehavior
    public class ImageryLayerInitializedCommandBehavior : ImageryLayerInitializedCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerLoadedCommand
	// ImageryLayerLoadedCommand
	public class ImageryLayerLoadedCommand : ImageryLayerCommandBase<ImageryLayerLoadedCommandBehavior>
	{ }

    public class ImageryLayerLoadedCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerLoadedCommandBehavior<T>,new()
    { }

    public class ImageryLayerLoadedCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, RoutedEventArgs>
    {
        public ImageryLayerLoadedCommandBehavior(Func<object, RoutedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerLoadedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Loaded += OnEventRaised;
        }
    }

	// ImageryLayerLoadedCommandBehavior
    public class ImageryLayerLoadedCommandBehavior : ImageryLayerLoadedCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerUnloadedCommand
	// ImageryLayerUnloadedCommand
	public class ImageryLayerUnloadedCommand : ImageryLayerCommandBase<ImageryLayerUnloadedCommandBehavior>
	{ }

    public class ImageryLayerUnloadedCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerUnloadedCommandBehavior<T>,new()
    { }

    public class ImageryLayerUnloadedCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, RoutedEventArgs>
    {
        public ImageryLayerUnloadedCommandBehavior(Func<object, RoutedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerUnloadedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Unloaded += OnEventRaised;
        }
    }

	// ImageryLayerUnloadedCommandBehavior
    public class ImageryLayerUnloadedCommandBehavior : ImageryLayerUnloadedCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerToolTipOpeningCommand
	// ImageryLayerToolTipOpeningCommand
	public class ImageryLayerToolTipOpeningCommand : ImageryLayerCommandBase<ImageryLayerToolTipOpeningCommandBehavior>
	{ }

    public class ImageryLayerToolTipOpeningCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerToolTipOpeningCommandBehavior<T>,new()
    { }

    public class ImageryLayerToolTipOpeningCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, ToolTipEventArgs>
    {
        public ImageryLayerToolTipOpeningCommandBehavior(Func<object, ToolTipEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerToolTipOpeningCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ToolTipOpening += OnEventRaised;
        }
    }

	// ImageryLayerToolTipOpeningCommandBehavior
    public class ImageryLayerToolTipOpeningCommandBehavior : ImageryLayerToolTipOpeningCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerToolTipClosingCommand
	// ImageryLayerToolTipClosingCommand
	public class ImageryLayerToolTipClosingCommand : ImageryLayerCommandBase<ImageryLayerToolTipClosingCommandBehavior>
	{ }

    public class ImageryLayerToolTipClosingCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerToolTipClosingCommandBehavior<T>,new()
    { }

    public class ImageryLayerToolTipClosingCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, ToolTipEventArgs>
    {
        public ImageryLayerToolTipClosingCommandBehavior(Func<object, ToolTipEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerToolTipClosingCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ToolTipClosing += OnEventRaised;
        }
    }

	// ImageryLayerToolTipClosingCommandBehavior
    public class ImageryLayerToolTipClosingCommandBehavior : ImageryLayerToolTipClosingCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerContextMenuOpeningCommand
	// ImageryLayerContextMenuOpeningCommand
	public class ImageryLayerContextMenuOpeningCommand : ImageryLayerCommandBase<ImageryLayerContextMenuOpeningCommandBehavior>
	{ }

    public class ImageryLayerContextMenuOpeningCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerContextMenuOpeningCommandBehavior<T>,new()
    { }

    public class ImageryLayerContextMenuOpeningCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, ContextMenuEventArgs>
    {
        public ImageryLayerContextMenuOpeningCommandBehavior(Func<object, ContextMenuEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerContextMenuOpeningCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ContextMenuOpening += OnEventRaised;
        }
    }

	// ImageryLayerContextMenuOpeningCommandBehavior
    public class ImageryLayerContextMenuOpeningCommandBehavior : ImageryLayerContextMenuOpeningCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerContextMenuClosingCommand
	// ImageryLayerContextMenuClosingCommand
	public class ImageryLayerContextMenuClosingCommand : ImageryLayerCommandBase<ImageryLayerContextMenuClosingCommandBehavior>
	{ }

    public class ImageryLayerContextMenuClosingCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerContextMenuClosingCommandBehavior<T>,new()
    { }

    public class ImageryLayerContextMenuClosingCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, ContextMenuEventArgs>
    {
        public ImageryLayerContextMenuClosingCommandBehavior(Func<object, ContextMenuEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerContextMenuClosingCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ContextMenuClosing += OnEventRaised;
        }
    }

	// ImageryLayerContextMenuClosingCommandBehavior
    public class ImageryLayerContextMenuClosingCommandBehavior : ImageryLayerContextMenuClosingCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerPreviewMouseDownCommand
	// ImageryLayerPreviewMouseDownCommand
	public class ImageryLayerPreviewMouseDownCommand : ImageryLayerCommandBase<ImageryLayerPreviewMouseDownCommandBehavior>
	{ }

    public class ImageryLayerPreviewMouseDownCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerPreviewMouseDownCommandBehavior<T>,new()
    { }

    public class ImageryLayerPreviewMouseDownCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public ImageryLayerPreviewMouseDownCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerPreviewMouseDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewMouseDown += OnEventRaised;
        }
    }

	// ImageryLayerPreviewMouseDownCommandBehavior
    public class ImageryLayerPreviewMouseDownCommandBehavior : ImageryLayerPreviewMouseDownCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerMouseDownCommand
	// ImageryLayerMouseDownCommand
	public class ImageryLayerMouseDownCommand : ImageryLayerCommandBase<ImageryLayerMouseDownCommandBehavior>
	{ }

    public class ImageryLayerMouseDownCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerMouseDownCommandBehavior<T>,new()
    { }

    public class ImageryLayerMouseDownCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public ImageryLayerMouseDownCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerMouseDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseDown += OnEventRaised;
        }
    }

	// ImageryLayerMouseDownCommandBehavior
    public class ImageryLayerMouseDownCommandBehavior : ImageryLayerMouseDownCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerPreviewMouseUpCommand
	// ImageryLayerPreviewMouseUpCommand
	public class ImageryLayerPreviewMouseUpCommand : ImageryLayerCommandBase<ImageryLayerPreviewMouseUpCommandBehavior>
	{ }

    public class ImageryLayerPreviewMouseUpCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerPreviewMouseUpCommandBehavior<T>,new()
    { }

    public class ImageryLayerPreviewMouseUpCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public ImageryLayerPreviewMouseUpCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerPreviewMouseUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewMouseUp += OnEventRaised;
        }
    }

	// ImageryLayerPreviewMouseUpCommandBehavior
    public class ImageryLayerPreviewMouseUpCommandBehavior : ImageryLayerPreviewMouseUpCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerMouseUpCommand
	// ImageryLayerMouseUpCommand
	public class ImageryLayerMouseUpCommand : ImageryLayerCommandBase<ImageryLayerMouseUpCommandBehavior>
	{ }

    public class ImageryLayerMouseUpCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerMouseUpCommandBehavior<T>,new()
    { }

    public class ImageryLayerMouseUpCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public ImageryLayerMouseUpCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerMouseUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseUp += OnEventRaised;
        }
    }

	// ImageryLayerMouseUpCommandBehavior
    public class ImageryLayerMouseUpCommandBehavior : ImageryLayerMouseUpCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerPreviewMouseLeftButtonDownCommand
	// ImageryLayerPreviewMouseLeftButtonDownCommand
	public class ImageryLayerPreviewMouseLeftButtonDownCommand : ImageryLayerCommandBase<ImageryLayerPreviewMouseLeftButtonDownCommandBehavior>
	{ }

    public class ImageryLayerPreviewMouseLeftButtonDownCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerPreviewMouseLeftButtonDownCommandBehavior<T>,new()
    { }

    public class ImageryLayerPreviewMouseLeftButtonDownCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public ImageryLayerPreviewMouseLeftButtonDownCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerPreviewMouseLeftButtonDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewMouseLeftButtonDown += OnEventRaised;
        }
    }

	// ImageryLayerPreviewMouseLeftButtonDownCommandBehavior
    public class ImageryLayerPreviewMouseLeftButtonDownCommandBehavior : ImageryLayerPreviewMouseLeftButtonDownCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerMouseLeftButtonDownCommand
	// ImageryLayerMouseLeftButtonDownCommand
	public class ImageryLayerMouseLeftButtonDownCommand : ImageryLayerCommandBase<ImageryLayerMouseLeftButtonDownCommandBehavior>
	{ }

    public class ImageryLayerMouseLeftButtonDownCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerMouseLeftButtonDownCommandBehavior<T>,new()
    { }

    public class ImageryLayerMouseLeftButtonDownCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public ImageryLayerMouseLeftButtonDownCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerMouseLeftButtonDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseLeftButtonDown += OnEventRaised;
        }
    }

	// ImageryLayerMouseLeftButtonDownCommandBehavior
    public class ImageryLayerMouseLeftButtonDownCommandBehavior : ImageryLayerMouseLeftButtonDownCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerPreviewMouseLeftButtonUpCommand
	// ImageryLayerPreviewMouseLeftButtonUpCommand
	public class ImageryLayerPreviewMouseLeftButtonUpCommand : ImageryLayerCommandBase<ImageryLayerPreviewMouseLeftButtonUpCommandBehavior>
	{ }

    public class ImageryLayerPreviewMouseLeftButtonUpCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerPreviewMouseLeftButtonUpCommandBehavior<T>,new()
    { }

    public class ImageryLayerPreviewMouseLeftButtonUpCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public ImageryLayerPreviewMouseLeftButtonUpCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerPreviewMouseLeftButtonUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewMouseLeftButtonUp += OnEventRaised;
        }
    }

	// ImageryLayerPreviewMouseLeftButtonUpCommandBehavior
    public class ImageryLayerPreviewMouseLeftButtonUpCommandBehavior : ImageryLayerPreviewMouseLeftButtonUpCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerMouseLeftButtonUpCommand
	// ImageryLayerMouseLeftButtonUpCommand
	public class ImageryLayerMouseLeftButtonUpCommand : ImageryLayerCommandBase<ImageryLayerMouseLeftButtonUpCommandBehavior>
	{ }

    public class ImageryLayerMouseLeftButtonUpCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerMouseLeftButtonUpCommandBehavior<T>,new()
    { }

    public class ImageryLayerMouseLeftButtonUpCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public ImageryLayerMouseLeftButtonUpCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerMouseLeftButtonUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseLeftButtonUp += OnEventRaised;
        }
    }

	// ImageryLayerMouseLeftButtonUpCommandBehavior
    public class ImageryLayerMouseLeftButtonUpCommandBehavior : ImageryLayerMouseLeftButtonUpCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerPreviewMouseRightButtonDownCommand
	// ImageryLayerPreviewMouseRightButtonDownCommand
	public class ImageryLayerPreviewMouseRightButtonDownCommand : ImageryLayerCommandBase<ImageryLayerPreviewMouseRightButtonDownCommandBehavior>
	{ }

    public class ImageryLayerPreviewMouseRightButtonDownCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerPreviewMouseRightButtonDownCommandBehavior<T>,new()
    { }

    public class ImageryLayerPreviewMouseRightButtonDownCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public ImageryLayerPreviewMouseRightButtonDownCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerPreviewMouseRightButtonDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewMouseRightButtonDown += OnEventRaised;
        }
    }

	// ImageryLayerPreviewMouseRightButtonDownCommandBehavior
    public class ImageryLayerPreviewMouseRightButtonDownCommandBehavior : ImageryLayerPreviewMouseRightButtonDownCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerMouseRightButtonDownCommand
	// ImageryLayerMouseRightButtonDownCommand
	public class ImageryLayerMouseRightButtonDownCommand : ImageryLayerCommandBase<ImageryLayerMouseRightButtonDownCommandBehavior>
	{ }

    public class ImageryLayerMouseRightButtonDownCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerMouseRightButtonDownCommandBehavior<T>,new()
    { }

    public class ImageryLayerMouseRightButtonDownCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public ImageryLayerMouseRightButtonDownCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerMouseRightButtonDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseRightButtonDown += OnEventRaised;
        }
    }

	// ImageryLayerMouseRightButtonDownCommandBehavior
    public class ImageryLayerMouseRightButtonDownCommandBehavior : ImageryLayerMouseRightButtonDownCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerPreviewMouseRightButtonUpCommand
	// ImageryLayerPreviewMouseRightButtonUpCommand
	public class ImageryLayerPreviewMouseRightButtonUpCommand : ImageryLayerCommandBase<ImageryLayerPreviewMouseRightButtonUpCommandBehavior>
	{ }

    public class ImageryLayerPreviewMouseRightButtonUpCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerPreviewMouseRightButtonUpCommandBehavior<T>,new()
    { }

    public class ImageryLayerPreviewMouseRightButtonUpCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public ImageryLayerPreviewMouseRightButtonUpCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerPreviewMouseRightButtonUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewMouseRightButtonUp += OnEventRaised;
        }
    }

	// ImageryLayerPreviewMouseRightButtonUpCommandBehavior
    public class ImageryLayerPreviewMouseRightButtonUpCommandBehavior : ImageryLayerPreviewMouseRightButtonUpCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerMouseRightButtonUpCommand
	// ImageryLayerMouseRightButtonUpCommand
	public class ImageryLayerMouseRightButtonUpCommand : ImageryLayerCommandBase<ImageryLayerMouseRightButtonUpCommandBehavior>
	{ }

    public class ImageryLayerMouseRightButtonUpCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerMouseRightButtonUpCommandBehavior<T>,new()
    { }

    public class ImageryLayerMouseRightButtonUpCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public ImageryLayerMouseRightButtonUpCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerMouseRightButtonUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseRightButtonUp += OnEventRaised;
        }
    }

	// ImageryLayerMouseRightButtonUpCommandBehavior
    public class ImageryLayerMouseRightButtonUpCommandBehavior : ImageryLayerMouseRightButtonUpCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerPreviewMouseMoveCommand
	// ImageryLayerPreviewMouseMoveCommand
	public class ImageryLayerPreviewMouseMoveCommand : ImageryLayerCommandBase<ImageryLayerPreviewMouseMoveCommandBehavior>
	{ }

    public class ImageryLayerPreviewMouseMoveCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerPreviewMouseMoveCommandBehavior<T>,new()
    { }

    public class ImageryLayerPreviewMouseMoveCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, MouseEventArgs>
    {
        public ImageryLayerPreviewMouseMoveCommandBehavior(Func<object, MouseEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerPreviewMouseMoveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewMouseMove += OnEventRaised;
        }
    }

	// ImageryLayerPreviewMouseMoveCommandBehavior
    public class ImageryLayerPreviewMouseMoveCommandBehavior : ImageryLayerPreviewMouseMoveCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerMouseMoveCommand
	// ImageryLayerMouseMoveCommand
	public class ImageryLayerMouseMoveCommand : ImageryLayerCommandBase<ImageryLayerMouseMoveCommandBehavior>
	{ }

    public class ImageryLayerMouseMoveCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerMouseMoveCommandBehavior<T>,new()
    { }

    public class ImageryLayerMouseMoveCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, MouseEventArgs>
    {
        public ImageryLayerMouseMoveCommandBehavior(Func<object, MouseEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerMouseMoveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseMove += OnEventRaised;
        }
    }

	// ImageryLayerMouseMoveCommandBehavior
    public class ImageryLayerMouseMoveCommandBehavior : ImageryLayerMouseMoveCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerPreviewMouseWheelCommand
	// ImageryLayerPreviewMouseWheelCommand
	public class ImageryLayerPreviewMouseWheelCommand : ImageryLayerCommandBase<ImageryLayerPreviewMouseWheelCommandBehavior>
	{ }

    public class ImageryLayerPreviewMouseWheelCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerPreviewMouseWheelCommandBehavior<T>,new()
    { }

    public class ImageryLayerPreviewMouseWheelCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, MouseWheelEventArgs>
    {
        public ImageryLayerPreviewMouseWheelCommandBehavior(Func<object, MouseWheelEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerPreviewMouseWheelCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewMouseWheel += OnEventRaised;
        }
    }

	// ImageryLayerPreviewMouseWheelCommandBehavior
    public class ImageryLayerPreviewMouseWheelCommandBehavior : ImageryLayerPreviewMouseWheelCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerMouseWheelCommand
	// ImageryLayerMouseWheelCommand
	public class ImageryLayerMouseWheelCommand : ImageryLayerCommandBase<ImageryLayerMouseWheelCommandBehavior>
	{ }

    public class ImageryLayerMouseWheelCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerMouseWheelCommandBehavior<T>,new()
    { }

    public class ImageryLayerMouseWheelCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, MouseWheelEventArgs>
    {
        public ImageryLayerMouseWheelCommandBehavior(Func<object, MouseWheelEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerMouseWheelCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseWheel += OnEventRaised;
        }
    }

	// ImageryLayerMouseWheelCommandBehavior
    public class ImageryLayerMouseWheelCommandBehavior : ImageryLayerMouseWheelCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerMouseEnterCommand
	// ImageryLayerMouseEnterCommand
	public class ImageryLayerMouseEnterCommand : ImageryLayerCommandBase<ImageryLayerMouseEnterCommandBehavior>
	{ }

    public class ImageryLayerMouseEnterCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerMouseEnterCommandBehavior<T>,new()
    { }

    public class ImageryLayerMouseEnterCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, MouseEventArgs>
    {
        public ImageryLayerMouseEnterCommandBehavior(Func<object, MouseEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerMouseEnterCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseEnter += OnEventRaised;
        }
    }

	// ImageryLayerMouseEnterCommandBehavior
    public class ImageryLayerMouseEnterCommandBehavior : ImageryLayerMouseEnterCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerMouseLeaveCommand
	// ImageryLayerMouseLeaveCommand
	public class ImageryLayerMouseLeaveCommand : ImageryLayerCommandBase<ImageryLayerMouseLeaveCommandBehavior>
	{ }

    public class ImageryLayerMouseLeaveCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerMouseLeaveCommandBehavior<T>,new()
    { }

    public class ImageryLayerMouseLeaveCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, MouseEventArgs>
    {
        public ImageryLayerMouseLeaveCommandBehavior(Func<object, MouseEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerMouseLeaveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseLeave += OnEventRaised;
        }
    }

	// ImageryLayerMouseLeaveCommandBehavior
    public class ImageryLayerMouseLeaveCommandBehavior : ImageryLayerMouseLeaveCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerGotMouseCaptureCommand
	// ImageryLayerGotMouseCaptureCommand
	public class ImageryLayerGotMouseCaptureCommand : ImageryLayerCommandBase<ImageryLayerGotMouseCaptureCommandBehavior>
	{ }

    public class ImageryLayerGotMouseCaptureCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerGotMouseCaptureCommandBehavior<T>,new()
    { }

    public class ImageryLayerGotMouseCaptureCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, MouseEventArgs>
    {
        public ImageryLayerGotMouseCaptureCommandBehavior(Func<object, MouseEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerGotMouseCaptureCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.GotMouseCapture += OnEventRaised;
        }
    }

	// ImageryLayerGotMouseCaptureCommandBehavior
    public class ImageryLayerGotMouseCaptureCommandBehavior : ImageryLayerGotMouseCaptureCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerLostMouseCaptureCommand
	// ImageryLayerLostMouseCaptureCommand
	public class ImageryLayerLostMouseCaptureCommand : ImageryLayerCommandBase<ImageryLayerLostMouseCaptureCommandBehavior>
	{ }

    public class ImageryLayerLostMouseCaptureCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerLostMouseCaptureCommandBehavior<T>,new()
    { }

    public class ImageryLayerLostMouseCaptureCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, MouseEventArgs>
    {
        public ImageryLayerLostMouseCaptureCommandBehavior(Func<object, MouseEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerLostMouseCaptureCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.LostMouseCapture += OnEventRaised;
        }
    }

	// ImageryLayerLostMouseCaptureCommandBehavior
    public class ImageryLayerLostMouseCaptureCommandBehavior : ImageryLayerLostMouseCaptureCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerQueryCursorCommand
	// ImageryLayerQueryCursorCommand
	public class ImageryLayerQueryCursorCommand : ImageryLayerCommandBase<ImageryLayerQueryCursorCommandBehavior>
	{ }

    public class ImageryLayerQueryCursorCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerQueryCursorCommandBehavior<T>,new()
    { }

    public class ImageryLayerQueryCursorCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, QueryCursorEventArgs>
    {
        public ImageryLayerQueryCursorCommandBehavior(Func<object, QueryCursorEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerQueryCursorCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.QueryCursor += OnEventRaised;
        }
    }

	// ImageryLayerQueryCursorCommandBehavior
    public class ImageryLayerQueryCursorCommandBehavior : ImageryLayerQueryCursorCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerPreviewStylusDownCommand
	// ImageryLayerPreviewStylusDownCommand
	public class ImageryLayerPreviewStylusDownCommand : ImageryLayerCommandBase<ImageryLayerPreviewStylusDownCommandBehavior>
	{ }

    public class ImageryLayerPreviewStylusDownCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerPreviewStylusDownCommandBehavior<T>,new()
    { }

    public class ImageryLayerPreviewStylusDownCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, StylusDownEventArgs>
    {
        public ImageryLayerPreviewStylusDownCommandBehavior(Func<object, StylusDownEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerPreviewStylusDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewStylusDown += OnEventRaised;
        }
    }

	// ImageryLayerPreviewStylusDownCommandBehavior
    public class ImageryLayerPreviewStylusDownCommandBehavior : ImageryLayerPreviewStylusDownCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerStylusDownCommand
	// ImageryLayerStylusDownCommand
	public class ImageryLayerStylusDownCommand : ImageryLayerCommandBase<ImageryLayerStylusDownCommandBehavior>
	{ }

    public class ImageryLayerStylusDownCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerStylusDownCommandBehavior<T>,new()
    { }

    public class ImageryLayerStylusDownCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, StylusDownEventArgs>
    {
        public ImageryLayerStylusDownCommandBehavior(Func<object, StylusDownEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerStylusDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusDown += OnEventRaised;
        }
    }

	// ImageryLayerStylusDownCommandBehavior
    public class ImageryLayerStylusDownCommandBehavior : ImageryLayerStylusDownCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerPreviewStylusUpCommand
	// ImageryLayerPreviewStylusUpCommand
	public class ImageryLayerPreviewStylusUpCommand : ImageryLayerCommandBase<ImageryLayerPreviewStylusUpCommandBehavior>
	{ }

    public class ImageryLayerPreviewStylusUpCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerPreviewStylusUpCommandBehavior<T>,new()
    { }

    public class ImageryLayerPreviewStylusUpCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public ImageryLayerPreviewStylusUpCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerPreviewStylusUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewStylusUp += OnEventRaised;
        }
    }

	// ImageryLayerPreviewStylusUpCommandBehavior
    public class ImageryLayerPreviewStylusUpCommandBehavior : ImageryLayerPreviewStylusUpCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerStylusUpCommand
	// ImageryLayerStylusUpCommand
	public class ImageryLayerStylusUpCommand : ImageryLayerCommandBase<ImageryLayerStylusUpCommandBehavior>
	{ }

    public class ImageryLayerStylusUpCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerStylusUpCommandBehavior<T>,new()
    { }

    public class ImageryLayerStylusUpCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public ImageryLayerStylusUpCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerStylusUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusUp += OnEventRaised;
        }
    }

	// ImageryLayerStylusUpCommandBehavior
    public class ImageryLayerStylusUpCommandBehavior : ImageryLayerStylusUpCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerPreviewStylusMoveCommand
	// ImageryLayerPreviewStylusMoveCommand
	public class ImageryLayerPreviewStylusMoveCommand : ImageryLayerCommandBase<ImageryLayerPreviewStylusMoveCommandBehavior>
	{ }

    public class ImageryLayerPreviewStylusMoveCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerPreviewStylusMoveCommandBehavior<T>,new()
    { }

    public class ImageryLayerPreviewStylusMoveCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public ImageryLayerPreviewStylusMoveCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerPreviewStylusMoveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewStylusMove += OnEventRaised;
        }
    }

	// ImageryLayerPreviewStylusMoveCommandBehavior
    public class ImageryLayerPreviewStylusMoveCommandBehavior : ImageryLayerPreviewStylusMoveCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerStylusMoveCommand
	// ImageryLayerStylusMoveCommand
	public class ImageryLayerStylusMoveCommand : ImageryLayerCommandBase<ImageryLayerStylusMoveCommandBehavior>
	{ }

    public class ImageryLayerStylusMoveCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerStylusMoveCommandBehavior<T>,new()
    { }

    public class ImageryLayerStylusMoveCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public ImageryLayerStylusMoveCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerStylusMoveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusMove += OnEventRaised;
        }
    }

	// ImageryLayerStylusMoveCommandBehavior
    public class ImageryLayerStylusMoveCommandBehavior : ImageryLayerStylusMoveCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerPreviewStylusInAirMoveCommand
	// ImageryLayerPreviewStylusInAirMoveCommand
	public class ImageryLayerPreviewStylusInAirMoveCommand : ImageryLayerCommandBase<ImageryLayerPreviewStylusInAirMoveCommandBehavior>
	{ }

    public class ImageryLayerPreviewStylusInAirMoveCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerPreviewStylusInAirMoveCommandBehavior<T>,new()
    { }

    public class ImageryLayerPreviewStylusInAirMoveCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public ImageryLayerPreviewStylusInAirMoveCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerPreviewStylusInAirMoveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewStylusInAirMove += OnEventRaised;
        }
    }

	// ImageryLayerPreviewStylusInAirMoveCommandBehavior
    public class ImageryLayerPreviewStylusInAirMoveCommandBehavior : ImageryLayerPreviewStylusInAirMoveCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerStylusInAirMoveCommand
	// ImageryLayerStylusInAirMoveCommand
	public class ImageryLayerStylusInAirMoveCommand : ImageryLayerCommandBase<ImageryLayerStylusInAirMoveCommandBehavior>
	{ }

    public class ImageryLayerStylusInAirMoveCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerStylusInAirMoveCommandBehavior<T>,new()
    { }

    public class ImageryLayerStylusInAirMoveCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public ImageryLayerStylusInAirMoveCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerStylusInAirMoveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusInAirMove += OnEventRaised;
        }
    }

	// ImageryLayerStylusInAirMoveCommandBehavior
    public class ImageryLayerStylusInAirMoveCommandBehavior : ImageryLayerStylusInAirMoveCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerStylusEnterCommand
	// ImageryLayerStylusEnterCommand
	public class ImageryLayerStylusEnterCommand : ImageryLayerCommandBase<ImageryLayerStylusEnterCommandBehavior>
	{ }

    public class ImageryLayerStylusEnterCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerStylusEnterCommandBehavior<T>,new()
    { }

    public class ImageryLayerStylusEnterCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public ImageryLayerStylusEnterCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerStylusEnterCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusEnter += OnEventRaised;
        }
    }

	// ImageryLayerStylusEnterCommandBehavior
    public class ImageryLayerStylusEnterCommandBehavior : ImageryLayerStylusEnterCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerStylusLeaveCommand
	// ImageryLayerStylusLeaveCommand
	public class ImageryLayerStylusLeaveCommand : ImageryLayerCommandBase<ImageryLayerStylusLeaveCommandBehavior>
	{ }

    public class ImageryLayerStylusLeaveCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerStylusLeaveCommandBehavior<T>,new()
    { }

    public class ImageryLayerStylusLeaveCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public ImageryLayerStylusLeaveCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerStylusLeaveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusLeave += OnEventRaised;
        }
    }

	// ImageryLayerStylusLeaveCommandBehavior
    public class ImageryLayerStylusLeaveCommandBehavior : ImageryLayerStylusLeaveCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerPreviewStylusInRangeCommand
	// ImageryLayerPreviewStylusInRangeCommand
	public class ImageryLayerPreviewStylusInRangeCommand : ImageryLayerCommandBase<ImageryLayerPreviewStylusInRangeCommandBehavior>
	{ }

    public class ImageryLayerPreviewStylusInRangeCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerPreviewStylusInRangeCommandBehavior<T>,new()
    { }

    public class ImageryLayerPreviewStylusInRangeCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public ImageryLayerPreviewStylusInRangeCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerPreviewStylusInRangeCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewStylusInRange += OnEventRaised;
        }
    }

	// ImageryLayerPreviewStylusInRangeCommandBehavior
    public class ImageryLayerPreviewStylusInRangeCommandBehavior : ImageryLayerPreviewStylusInRangeCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerStylusInRangeCommand
	// ImageryLayerStylusInRangeCommand
	public class ImageryLayerStylusInRangeCommand : ImageryLayerCommandBase<ImageryLayerStylusInRangeCommandBehavior>
	{ }

    public class ImageryLayerStylusInRangeCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerStylusInRangeCommandBehavior<T>,new()
    { }

    public class ImageryLayerStylusInRangeCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public ImageryLayerStylusInRangeCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerStylusInRangeCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusInRange += OnEventRaised;
        }
    }

	// ImageryLayerStylusInRangeCommandBehavior
    public class ImageryLayerStylusInRangeCommandBehavior : ImageryLayerStylusInRangeCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerPreviewStylusOutOfRangeCommand
	// ImageryLayerPreviewStylusOutOfRangeCommand
	public class ImageryLayerPreviewStylusOutOfRangeCommand : ImageryLayerCommandBase<ImageryLayerPreviewStylusOutOfRangeCommandBehavior>
	{ }

    public class ImageryLayerPreviewStylusOutOfRangeCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerPreviewStylusOutOfRangeCommandBehavior<T>,new()
    { }

    public class ImageryLayerPreviewStylusOutOfRangeCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public ImageryLayerPreviewStylusOutOfRangeCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerPreviewStylusOutOfRangeCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewStylusOutOfRange += OnEventRaised;
        }
    }

	// ImageryLayerPreviewStylusOutOfRangeCommandBehavior
    public class ImageryLayerPreviewStylusOutOfRangeCommandBehavior : ImageryLayerPreviewStylusOutOfRangeCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerStylusOutOfRangeCommand
	// ImageryLayerStylusOutOfRangeCommand
	public class ImageryLayerStylusOutOfRangeCommand : ImageryLayerCommandBase<ImageryLayerStylusOutOfRangeCommandBehavior>
	{ }

    public class ImageryLayerStylusOutOfRangeCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerStylusOutOfRangeCommandBehavior<T>,new()
    { }

    public class ImageryLayerStylusOutOfRangeCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public ImageryLayerStylusOutOfRangeCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerStylusOutOfRangeCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusOutOfRange += OnEventRaised;
        }
    }

	// ImageryLayerStylusOutOfRangeCommandBehavior
    public class ImageryLayerStylusOutOfRangeCommandBehavior : ImageryLayerStylusOutOfRangeCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerPreviewStylusSystemGestureCommand
	// ImageryLayerPreviewStylusSystemGestureCommand
	public class ImageryLayerPreviewStylusSystemGestureCommand : ImageryLayerCommandBase<ImageryLayerPreviewStylusSystemGestureCommandBehavior>
	{ }

    public class ImageryLayerPreviewStylusSystemGestureCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerPreviewStylusSystemGestureCommandBehavior<T>,new()
    { }

    public class ImageryLayerPreviewStylusSystemGestureCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, StylusSystemGestureEventArgs>
    {
        public ImageryLayerPreviewStylusSystemGestureCommandBehavior(Func<object, StylusSystemGestureEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerPreviewStylusSystemGestureCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewStylusSystemGesture += OnEventRaised;
        }
    }

	// ImageryLayerPreviewStylusSystemGestureCommandBehavior
    public class ImageryLayerPreviewStylusSystemGestureCommandBehavior : ImageryLayerPreviewStylusSystemGestureCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerStylusSystemGestureCommand
	// ImageryLayerStylusSystemGestureCommand
	public class ImageryLayerStylusSystemGestureCommand : ImageryLayerCommandBase<ImageryLayerStylusSystemGestureCommandBehavior>
	{ }

    public class ImageryLayerStylusSystemGestureCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerStylusSystemGestureCommandBehavior<T>,new()
    { }

    public class ImageryLayerStylusSystemGestureCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, StylusSystemGestureEventArgs>
    {
        public ImageryLayerStylusSystemGestureCommandBehavior(Func<object, StylusSystemGestureEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerStylusSystemGestureCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusSystemGesture += OnEventRaised;
        }
    }

	// ImageryLayerStylusSystemGestureCommandBehavior
    public class ImageryLayerStylusSystemGestureCommandBehavior : ImageryLayerStylusSystemGestureCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerGotStylusCaptureCommand
	// ImageryLayerGotStylusCaptureCommand
	public class ImageryLayerGotStylusCaptureCommand : ImageryLayerCommandBase<ImageryLayerGotStylusCaptureCommandBehavior>
	{ }

    public class ImageryLayerGotStylusCaptureCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerGotStylusCaptureCommandBehavior<T>,new()
    { }

    public class ImageryLayerGotStylusCaptureCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public ImageryLayerGotStylusCaptureCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerGotStylusCaptureCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.GotStylusCapture += OnEventRaised;
        }
    }

	// ImageryLayerGotStylusCaptureCommandBehavior
    public class ImageryLayerGotStylusCaptureCommandBehavior : ImageryLayerGotStylusCaptureCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerLostStylusCaptureCommand
	// ImageryLayerLostStylusCaptureCommand
	public class ImageryLayerLostStylusCaptureCommand : ImageryLayerCommandBase<ImageryLayerLostStylusCaptureCommandBehavior>
	{ }

    public class ImageryLayerLostStylusCaptureCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerLostStylusCaptureCommandBehavior<T>,new()
    { }

    public class ImageryLayerLostStylusCaptureCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public ImageryLayerLostStylusCaptureCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerLostStylusCaptureCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.LostStylusCapture += OnEventRaised;
        }
    }

	// ImageryLayerLostStylusCaptureCommandBehavior
    public class ImageryLayerLostStylusCaptureCommandBehavior : ImageryLayerLostStylusCaptureCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerStylusButtonDownCommand
	// ImageryLayerStylusButtonDownCommand
	public class ImageryLayerStylusButtonDownCommand : ImageryLayerCommandBase<ImageryLayerStylusButtonDownCommandBehavior>
	{ }

    public class ImageryLayerStylusButtonDownCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerStylusButtonDownCommandBehavior<T>,new()
    { }

    public class ImageryLayerStylusButtonDownCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, StylusButtonEventArgs>
    {
        public ImageryLayerStylusButtonDownCommandBehavior(Func<object, StylusButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerStylusButtonDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusButtonDown += OnEventRaised;
        }
    }

	// ImageryLayerStylusButtonDownCommandBehavior
    public class ImageryLayerStylusButtonDownCommandBehavior : ImageryLayerStylusButtonDownCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerStylusButtonUpCommand
	// ImageryLayerStylusButtonUpCommand
	public class ImageryLayerStylusButtonUpCommand : ImageryLayerCommandBase<ImageryLayerStylusButtonUpCommandBehavior>
	{ }

    public class ImageryLayerStylusButtonUpCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerStylusButtonUpCommandBehavior<T>,new()
    { }

    public class ImageryLayerStylusButtonUpCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, StylusButtonEventArgs>
    {
        public ImageryLayerStylusButtonUpCommandBehavior(Func<object, StylusButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerStylusButtonUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusButtonUp += OnEventRaised;
        }
    }

	// ImageryLayerStylusButtonUpCommandBehavior
    public class ImageryLayerStylusButtonUpCommandBehavior : ImageryLayerStylusButtonUpCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerPreviewStylusButtonDownCommand
	// ImageryLayerPreviewStylusButtonDownCommand
	public class ImageryLayerPreviewStylusButtonDownCommand : ImageryLayerCommandBase<ImageryLayerPreviewStylusButtonDownCommandBehavior>
	{ }

    public class ImageryLayerPreviewStylusButtonDownCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerPreviewStylusButtonDownCommandBehavior<T>,new()
    { }

    public class ImageryLayerPreviewStylusButtonDownCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, StylusButtonEventArgs>
    {
        public ImageryLayerPreviewStylusButtonDownCommandBehavior(Func<object, StylusButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerPreviewStylusButtonDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewStylusButtonDown += OnEventRaised;
        }
    }

	// ImageryLayerPreviewStylusButtonDownCommandBehavior
    public class ImageryLayerPreviewStylusButtonDownCommandBehavior : ImageryLayerPreviewStylusButtonDownCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerPreviewStylusButtonUpCommand
	// ImageryLayerPreviewStylusButtonUpCommand
	public class ImageryLayerPreviewStylusButtonUpCommand : ImageryLayerCommandBase<ImageryLayerPreviewStylusButtonUpCommandBehavior>
	{ }

    public class ImageryLayerPreviewStylusButtonUpCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerPreviewStylusButtonUpCommandBehavior<T>,new()
    { }

    public class ImageryLayerPreviewStylusButtonUpCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, StylusButtonEventArgs>
    {
        public ImageryLayerPreviewStylusButtonUpCommandBehavior(Func<object, StylusButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerPreviewStylusButtonUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewStylusButtonUp += OnEventRaised;
        }
    }

	// ImageryLayerPreviewStylusButtonUpCommandBehavior
    public class ImageryLayerPreviewStylusButtonUpCommandBehavior : ImageryLayerPreviewStylusButtonUpCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerPreviewKeyDownCommand
	// ImageryLayerPreviewKeyDownCommand
	public class ImageryLayerPreviewKeyDownCommand : ImageryLayerCommandBase<ImageryLayerPreviewKeyDownCommandBehavior>
	{ }

    public class ImageryLayerPreviewKeyDownCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerPreviewKeyDownCommandBehavior<T>,new()
    { }

    public class ImageryLayerPreviewKeyDownCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, KeyEventArgs>
    {
        public ImageryLayerPreviewKeyDownCommandBehavior(Func<object, KeyEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerPreviewKeyDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewKeyDown += OnEventRaised;
        }
    }

	// ImageryLayerPreviewKeyDownCommandBehavior
    public class ImageryLayerPreviewKeyDownCommandBehavior : ImageryLayerPreviewKeyDownCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerKeyDownCommand
	// ImageryLayerKeyDownCommand
	public class ImageryLayerKeyDownCommand : ImageryLayerCommandBase<ImageryLayerKeyDownCommandBehavior>
	{ }

    public class ImageryLayerKeyDownCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerKeyDownCommandBehavior<T>,new()
    { }

    public class ImageryLayerKeyDownCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, KeyEventArgs>
    {
        public ImageryLayerKeyDownCommandBehavior(Func<object, KeyEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerKeyDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.KeyDown += OnEventRaised;
        }
    }

	// ImageryLayerKeyDownCommandBehavior
    public class ImageryLayerKeyDownCommandBehavior : ImageryLayerKeyDownCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerPreviewKeyUpCommand
	// ImageryLayerPreviewKeyUpCommand
	public class ImageryLayerPreviewKeyUpCommand : ImageryLayerCommandBase<ImageryLayerPreviewKeyUpCommandBehavior>
	{ }

    public class ImageryLayerPreviewKeyUpCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerPreviewKeyUpCommandBehavior<T>,new()
    { }

    public class ImageryLayerPreviewKeyUpCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, KeyEventArgs>
    {
        public ImageryLayerPreviewKeyUpCommandBehavior(Func<object, KeyEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerPreviewKeyUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewKeyUp += OnEventRaised;
        }
    }

	// ImageryLayerPreviewKeyUpCommandBehavior
    public class ImageryLayerPreviewKeyUpCommandBehavior : ImageryLayerPreviewKeyUpCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerKeyUpCommand
	// ImageryLayerKeyUpCommand
	public class ImageryLayerKeyUpCommand : ImageryLayerCommandBase<ImageryLayerKeyUpCommandBehavior>
	{ }

    public class ImageryLayerKeyUpCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerKeyUpCommandBehavior<T>,new()
    { }

    public class ImageryLayerKeyUpCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, KeyEventArgs>
    {
        public ImageryLayerKeyUpCommandBehavior(Func<object, KeyEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerKeyUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.KeyUp += OnEventRaised;
        }
    }

	// ImageryLayerKeyUpCommandBehavior
    public class ImageryLayerKeyUpCommandBehavior : ImageryLayerKeyUpCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerPreviewGotKeyboardFocusCommand
	// ImageryLayerPreviewGotKeyboardFocusCommand
	public class ImageryLayerPreviewGotKeyboardFocusCommand : ImageryLayerCommandBase<ImageryLayerPreviewGotKeyboardFocusCommandBehavior>
	{ }

    public class ImageryLayerPreviewGotKeyboardFocusCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerPreviewGotKeyboardFocusCommandBehavior<T>,new()
    { }

    public class ImageryLayerPreviewGotKeyboardFocusCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, KeyboardFocusChangedEventArgs>
    {
        public ImageryLayerPreviewGotKeyboardFocusCommandBehavior(Func<object, KeyboardFocusChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerPreviewGotKeyboardFocusCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewGotKeyboardFocus += OnEventRaised;
        }
    }

	// ImageryLayerPreviewGotKeyboardFocusCommandBehavior
    public class ImageryLayerPreviewGotKeyboardFocusCommandBehavior : ImageryLayerPreviewGotKeyboardFocusCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerGotKeyboardFocusCommand
	// ImageryLayerGotKeyboardFocusCommand
	public class ImageryLayerGotKeyboardFocusCommand : ImageryLayerCommandBase<ImageryLayerGotKeyboardFocusCommandBehavior>
	{ }

    public class ImageryLayerGotKeyboardFocusCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerGotKeyboardFocusCommandBehavior<T>,new()
    { }

    public class ImageryLayerGotKeyboardFocusCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, KeyboardFocusChangedEventArgs>
    {
        public ImageryLayerGotKeyboardFocusCommandBehavior(Func<object, KeyboardFocusChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerGotKeyboardFocusCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.GotKeyboardFocus += OnEventRaised;
        }
    }

	// ImageryLayerGotKeyboardFocusCommandBehavior
    public class ImageryLayerGotKeyboardFocusCommandBehavior : ImageryLayerGotKeyboardFocusCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerPreviewLostKeyboardFocusCommand
	// ImageryLayerPreviewLostKeyboardFocusCommand
	public class ImageryLayerPreviewLostKeyboardFocusCommand : ImageryLayerCommandBase<ImageryLayerPreviewLostKeyboardFocusCommandBehavior>
	{ }

    public class ImageryLayerPreviewLostKeyboardFocusCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerPreviewLostKeyboardFocusCommandBehavior<T>,new()
    { }

    public class ImageryLayerPreviewLostKeyboardFocusCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, KeyboardFocusChangedEventArgs>
    {
        public ImageryLayerPreviewLostKeyboardFocusCommandBehavior(Func<object, KeyboardFocusChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerPreviewLostKeyboardFocusCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewLostKeyboardFocus += OnEventRaised;
        }
    }

	// ImageryLayerPreviewLostKeyboardFocusCommandBehavior
    public class ImageryLayerPreviewLostKeyboardFocusCommandBehavior : ImageryLayerPreviewLostKeyboardFocusCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerLostKeyboardFocusCommand
	// ImageryLayerLostKeyboardFocusCommand
	public class ImageryLayerLostKeyboardFocusCommand : ImageryLayerCommandBase<ImageryLayerLostKeyboardFocusCommandBehavior>
	{ }

    public class ImageryLayerLostKeyboardFocusCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerLostKeyboardFocusCommandBehavior<T>,new()
    { }

    public class ImageryLayerLostKeyboardFocusCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, KeyboardFocusChangedEventArgs>
    {
        public ImageryLayerLostKeyboardFocusCommandBehavior(Func<object, KeyboardFocusChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerLostKeyboardFocusCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.LostKeyboardFocus += OnEventRaised;
        }
    }

	// ImageryLayerLostKeyboardFocusCommandBehavior
    public class ImageryLayerLostKeyboardFocusCommandBehavior : ImageryLayerLostKeyboardFocusCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerPreviewTextInputCommand
	// ImageryLayerPreviewTextInputCommand
	public class ImageryLayerPreviewTextInputCommand : ImageryLayerCommandBase<ImageryLayerPreviewTextInputCommandBehavior>
	{ }

    public class ImageryLayerPreviewTextInputCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerPreviewTextInputCommandBehavior<T>,new()
    { }

    public class ImageryLayerPreviewTextInputCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, TextCompositionEventArgs>
    {
        public ImageryLayerPreviewTextInputCommandBehavior(Func<object, TextCompositionEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerPreviewTextInputCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewTextInput += OnEventRaised;
        }
    }

	// ImageryLayerPreviewTextInputCommandBehavior
    public class ImageryLayerPreviewTextInputCommandBehavior : ImageryLayerPreviewTextInputCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerTextInputCommand
	// ImageryLayerTextInputCommand
	public class ImageryLayerTextInputCommand : ImageryLayerCommandBase<ImageryLayerTextInputCommandBehavior>
	{ }

    public class ImageryLayerTextInputCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerTextInputCommandBehavior<T>,new()
    { }

    public class ImageryLayerTextInputCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, TextCompositionEventArgs>
    {
        public ImageryLayerTextInputCommandBehavior(Func<object, TextCompositionEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerTextInputCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.TextInput += OnEventRaised;
        }
    }

	// ImageryLayerTextInputCommandBehavior
    public class ImageryLayerTextInputCommandBehavior : ImageryLayerTextInputCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerPreviewQueryContinueDragCommand
	// ImageryLayerPreviewQueryContinueDragCommand
	public class ImageryLayerPreviewQueryContinueDragCommand : ImageryLayerCommandBase<ImageryLayerPreviewQueryContinueDragCommandBehavior>
	{ }

    public class ImageryLayerPreviewQueryContinueDragCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerPreviewQueryContinueDragCommandBehavior<T>,new()
    { }

    public class ImageryLayerPreviewQueryContinueDragCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, QueryContinueDragEventArgs>
    {
        public ImageryLayerPreviewQueryContinueDragCommandBehavior(Func<object, QueryContinueDragEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerPreviewQueryContinueDragCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewQueryContinueDrag += OnEventRaised;
        }
    }

	// ImageryLayerPreviewQueryContinueDragCommandBehavior
    public class ImageryLayerPreviewQueryContinueDragCommandBehavior : ImageryLayerPreviewQueryContinueDragCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerQueryContinueDragCommand
	// ImageryLayerQueryContinueDragCommand
	public class ImageryLayerQueryContinueDragCommand : ImageryLayerCommandBase<ImageryLayerQueryContinueDragCommandBehavior>
	{ }

    public class ImageryLayerQueryContinueDragCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerQueryContinueDragCommandBehavior<T>,new()
    { }

    public class ImageryLayerQueryContinueDragCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, QueryContinueDragEventArgs>
    {
        public ImageryLayerQueryContinueDragCommandBehavior(Func<object, QueryContinueDragEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerQueryContinueDragCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.QueryContinueDrag += OnEventRaised;
        }
    }

	// ImageryLayerQueryContinueDragCommandBehavior
    public class ImageryLayerQueryContinueDragCommandBehavior : ImageryLayerQueryContinueDragCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerPreviewGiveFeedbackCommand
	// ImageryLayerPreviewGiveFeedbackCommand
	public class ImageryLayerPreviewGiveFeedbackCommand : ImageryLayerCommandBase<ImageryLayerPreviewGiveFeedbackCommandBehavior>
	{ }

    public class ImageryLayerPreviewGiveFeedbackCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerPreviewGiveFeedbackCommandBehavior<T>,new()
    { }

    public class ImageryLayerPreviewGiveFeedbackCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, GiveFeedbackEventArgs>
    {
        public ImageryLayerPreviewGiveFeedbackCommandBehavior(Func<object, GiveFeedbackEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerPreviewGiveFeedbackCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewGiveFeedback += OnEventRaised;
        }
    }

	// ImageryLayerPreviewGiveFeedbackCommandBehavior
    public class ImageryLayerPreviewGiveFeedbackCommandBehavior : ImageryLayerPreviewGiveFeedbackCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerGiveFeedbackCommand
	// ImageryLayerGiveFeedbackCommand
	public class ImageryLayerGiveFeedbackCommand : ImageryLayerCommandBase<ImageryLayerGiveFeedbackCommandBehavior>
	{ }

    public class ImageryLayerGiveFeedbackCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerGiveFeedbackCommandBehavior<T>,new()
    { }

    public class ImageryLayerGiveFeedbackCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, GiveFeedbackEventArgs>
    {
        public ImageryLayerGiveFeedbackCommandBehavior(Func<object, GiveFeedbackEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerGiveFeedbackCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.GiveFeedback += OnEventRaised;
        }
    }

	// ImageryLayerGiveFeedbackCommandBehavior
    public class ImageryLayerGiveFeedbackCommandBehavior : ImageryLayerGiveFeedbackCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerPreviewDragEnterCommand
	// ImageryLayerPreviewDragEnterCommand
	public class ImageryLayerPreviewDragEnterCommand : ImageryLayerCommandBase<ImageryLayerPreviewDragEnterCommandBehavior>
	{ }

    public class ImageryLayerPreviewDragEnterCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerPreviewDragEnterCommandBehavior<T>,new()
    { }

    public class ImageryLayerPreviewDragEnterCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, DragEventArgs>
    {
        public ImageryLayerPreviewDragEnterCommandBehavior(Func<object, DragEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerPreviewDragEnterCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewDragEnter += OnEventRaised;
        }
    }

	// ImageryLayerPreviewDragEnterCommandBehavior
    public class ImageryLayerPreviewDragEnterCommandBehavior : ImageryLayerPreviewDragEnterCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerDragEnterCommand
	// ImageryLayerDragEnterCommand
	public class ImageryLayerDragEnterCommand : ImageryLayerCommandBase<ImageryLayerDragEnterCommandBehavior>
	{ }

    public class ImageryLayerDragEnterCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerDragEnterCommandBehavior<T>,new()
    { }

    public class ImageryLayerDragEnterCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, DragEventArgs>
    {
        public ImageryLayerDragEnterCommandBehavior(Func<object, DragEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerDragEnterCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.DragEnter += OnEventRaised;
        }
    }

	// ImageryLayerDragEnterCommandBehavior
    public class ImageryLayerDragEnterCommandBehavior : ImageryLayerDragEnterCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerPreviewDragOverCommand
	// ImageryLayerPreviewDragOverCommand
	public class ImageryLayerPreviewDragOverCommand : ImageryLayerCommandBase<ImageryLayerPreviewDragOverCommandBehavior>
	{ }

    public class ImageryLayerPreviewDragOverCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerPreviewDragOverCommandBehavior<T>,new()
    { }

    public class ImageryLayerPreviewDragOverCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, DragEventArgs>
    {
        public ImageryLayerPreviewDragOverCommandBehavior(Func<object, DragEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerPreviewDragOverCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewDragOver += OnEventRaised;
        }
    }

	// ImageryLayerPreviewDragOverCommandBehavior
    public class ImageryLayerPreviewDragOverCommandBehavior : ImageryLayerPreviewDragOverCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerDragOverCommand
	// ImageryLayerDragOverCommand
	public class ImageryLayerDragOverCommand : ImageryLayerCommandBase<ImageryLayerDragOverCommandBehavior>
	{ }

    public class ImageryLayerDragOverCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerDragOverCommandBehavior<T>,new()
    { }

    public class ImageryLayerDragOverCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, DragEventArgs>
    {
        public ImageryLayerDragOverCommandBehavior(Func<object, DragEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerDragOverCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.DragOver += OnEventRaised;
        }
    }

	// ImageryLayerDragOverCommandBehavior
    public class ImageryLayerDragOverCommandBehavior : ImageryLayerDragOverCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerPreviewDragLeaveCommand
	// ImageryLayerPreviewDragLeaveCommand
	public class ImageryLayerPreviewDragLeaveCommand : ImageryLayerCommandBase<ImageryLayerPreviewDragLeaveCommandBehavior>
	{ }

    public class ImageryLayerPreviewDragLeaveCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerPreviewDragLeaveCommandBehavior<T>,new()
    { }

    public class ImageryLayerPreviewDragLeaveCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, DragEventArgs>
    {
        public ImageryLayerPreviewDragLeaveCommandBehavior(Func<object, DragEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerPreviewDragLeaveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewDragLeave += OnEventRaised;
        }
    }

	// ImageryLayerPreviewDragLeaveCommandBehavior
    public class ImageryLayerPreviewDragLeaveCommandBehavior : ImageryLayerPreviewDragLeaveCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerDragLeaveCommand
	// ImageryLayerDragLeaveCommand
	public class ImageryLayerDragLeaveCommand : ImageryLayerCommandBase<ImageryLayerDragLeaveCommandBehavior>
	{ }

    public class ImageryLayerDragLeaveCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerDragLeaveCommandBehavior<T>,new()
    { }

    public class ImageryLayerDragLeaveCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, DragEventArgs>
    {
        public ImageryLayerDragLeaveCommandBehavior(Func<object, DragEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerDragLeaveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.DragLeave += OnEventRaised;
        }
    }

	// ImageryLayerDragLeaveCommandBehavior
    public class ImageryLayerDragLeaveCommandBehavior : ImageryLayerDragLeaveCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerPreviewDropCommand
	// ImageryLayerPreviewDropCommand
	public class ImageryLayerPreviewDropCommand : ImageryLayerCommandBase<ImageryLayerPreviewDropCommandBehavior>
	{ }

    public class ImageryLayerPreviewDropCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerPreviewDropCommandBehavior<T>,new()
    { }

    public class ImageryLayerPreviewDropCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, DragEventArgs>
    {
        public ImageryLayerPreviewDropCommandBehavior(Func<object, DragEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerPreviewDropCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewDrop += OnEventRaised;
        }
    }

	// ImageryLayerPreviewDropCommandBehavior
    public class ImageryLayerPreviewDropCommandBehavior : ImageryLayerPreviewDropCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerDropCommand
	// ImageryLayerDropCommand
	public class ImageryLayerDropCommand : ImageryLayerCommandBase<ImageryLayerDropCommandBehavior>
	{ }

    public class ImageryLayerDropCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerDropCommandBehavior<T>,new()
    { }

    public class ImageryLayerDropCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, DragEventArgs>
    {
        public ImageryLayerDropCommandBehavior(Func<object, DragEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerDropCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Drop += OnEventRaised;
        }
    }

	// ImageryLayerDropCommandBehavior
    public class ImageryLayerDropCommandBehavior : ImageryLayerDropCommandBehavior<object>
    { }
	#endregion

#if SyncfusionFramework4_0

	#region ImageryLayerPreviewTouchDownCommand
	// ImageryLayerPreviewTouchDownCommand
	public class ImageryLayerPreviewTouchDownCommand : ImageryLayerCommandBase<ImageryLayerPreviewTouchDownCommandBehavior>
	{ }

    public class ImageryLayerPreviewTouchDownCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerPreviewTouchDownCommandBehavior<T>,new()
    { }

    public class ImageryLayerPreviewTouchDownCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, TouchEventArgs>
    {
        public ImageryLayerPreviewTouchDownCommandBehavior(Func<object, TouchEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerPreviewTouchDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewTouchDown += OnEventRaised;
        }
    }

	// ImageryLayerPreviewTouchDownCommandBehavior
    public class ImageryLayerPreviewTouchDownCommandBehavior : ImageryLayerPreviewTouchDownCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerTouchDownCommand
	// ImageryLayerTouchDownCommand
	public class ImageryLayerTouchDownCommand : ImageryLayerCommandBase<ImageryLayerTouchDownCommandBehavior>
	{ }

    public class ImageryLayerTouchDownCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerTouchDownCommandBehavior<T>,new()
    { }

    public class ImageryLayerTouchDownCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, TouchEventArgs>
    {
        public ImageryLayerTouchDownCommandBehavior(Func<object, TouchEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerTouchDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.TouchDown += OnEventRaised;
        }
    }

	// ImageryLayerTouchDownCommandBehavior
    public class ImageryLayerTouchDownCommandBehavior : ImageryLayerTouchDownCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerPreviewTouchMoveCommand
	// ImageryLayerPreviewTouchMoveCommand
	public class ImageryLayerPreviewTouchMoveCommand : ImageryLayerCommandBase<ImageryLayerPreviewTouchMoveCommandBehavior>
	{ }

    public class ImageryLayerPreviewTouchMoveCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerPreviewTouchMoveCommandBehavior<T>,new()
    { }

    public class ImageryLayerPreviewTouchMoveCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, TouchEventArgs>
    {
        public ImageryLayerPreviewTouchMoveCommandBehavior(Func<object, TouchEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerPreviewTouchMoveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewTouchMove += OnEventRaised;
        }
    }

	// ImageryLayerPreviewTouchMoveCommandBehavior
    public class ImageryLayerPreviewTouchMoveCommandBehavior : ImageryLayerPreviewTouchMoveCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerTouchMoveCommand
	// ImageryLayerTouchMoveCommand
	public class ImageryLayerTouchMoveCommand : ImageryLayerCommandBase<ImageryLayerTouchMoveCommandBehavior>
	{ }

    public class ImageryLayerTouchMoveCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerTouchMoveCommandBehavior<T>,new()
    { }

    public class ImageryLayerTouchMoveCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, TouchEventArgs>
    {
        public ImageryLayerTouchMoveCommandBehavior(Func<object, TouchEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerTouchMoveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.TouchMove += OnEventRaised;
        }
    }

	// ImageryLayerTouchMoveCommandBehavior
    public class ImageryLayerTouchMoveCommandBehavior : ImageryLayerTouchMoveCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerPreviewTouchUpCommand
	// ImageryLayerPreviewTouchUpCommand
	public class ImageryLayerPreviewTouchUpCommand : ImageryLayerCommandBase<ImageryLayerPreviewTouchUpCommandBehavior>
	{ }

    public class ImageryLayerPreviewTouchUpCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerPreviewTouchUpCommandBehavior<T>,new()
    { }

    public class ImageryLayerPreviewTouchUpCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, TouchEventArgs>
    {
        public ImageryLayerPreviewTouchUpCommandBehavior(Func<object, TouchEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerPreviewTouchUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewTouchUp += OnEventRaised;
        }
    }

	// ImageryLayerPreviewTouchUpCommandBehavior
    public class ImageryLayerPreviewTouchUpCommandBehavior : ImageryLayerPreviewTouchUpCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerTouchUpCommand
	// ImageryLayerTouchUpCommand
	public class ImageryLayerTouchUpCommand : ImageryLayerCommandBase<ImageryLayerTouchUpCommandBehavior>
	{ }

    public class ImageryLayerTouchUpCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerTouchUpCommandBehavior<T>,new()
    { }

    public class ImageryLayerTouchUpCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, TouchEventArgs>
    {
        public ImageryLayerTouchUpCommandBehavior(Func<object, TouchEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerTouchUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.TouchUp += OnEventRaised;
        }
    }

	// ImageryLayerTouchUpCommandBehavior
    public class ImageryLayerTouchUpCommandBehavior : ImageryLayerTouchUpCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerGotTouchCaptureCommand
	// ImageryLayerGotTouchCaptureCommand
	public class ImageryLayerGotTouchCaptureCommand : ImageryLayerCommandBase<ImageryLayerGotTouchCaptureCommandBehavior>
	{ }

    public class ImageryLayerGotTouchCaptureCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerGotTouchCaptureCommandBehavior<T>,new()
    { }

    public class ImageryLayerGotTouchCaptureCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, TouchEventArgs>
    {
        public ImageryLayerGotTouchCaptureCommandBehavior(Func<object, TouchEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerGotTouchCaptureCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.GotTouchCapture += OnEventRaised;
        }
    }

	// ImageryLayerGotTouchCaptureCommandBehavior
    public class ImageryLayerGotTouchCaptureCommandBehavior : ImageryLayerGotTouchCaptureCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerLostTouchCaptureCommand
	// ImageryLayerLostTouchCaptureCommand
	public class ImageryLayerLostTouchCaptureCommand : ImageryLayerCommandBase<ImageryLayerLostTouchCaptureCommandBehavior>
	{ }

    public class ImageryLayerLostTouchCaptureCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerLostTouchCaptureCommandBehavior<T>,new()
    { }

    public class ImageryLayerLostTouchCaptureCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, TouchEventArgs>
    {
        public ImageryLayerLostTouchCaptureCommandBehavior(Func<object, TouchEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerLostTouchCaptureCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.LostTouchCapture += OnEventRaised;
        }
    }

	// ImageryLayerLostTouchCaptureCommandBehavior
    public class ImageryLayerLostTouchCaptureCommandBehavior : ImageryLayerLostTouchCaptureCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerTouchEnterCommand
	// ImageryLayerTouchEnterCommand
	public class ImageryLayerTouchEnterCommand : ImageryLayerCommandBase<ImageryLayerTouchEnterCommandBehavior>
	{ }

    public class ImageryLayerTouchEnterCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerTouchEnterCommandBehavior<T>,new()
    { }

    public class ImageryLayerTouchEnterCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, TouchEventArgs>
    {
        public ImageryLayerTouchEnterCommandBehavior(Func<object, TouchEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerTouchEnterCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.TouchEnter += OnEventRaised;
        }
    }

	// ImageryLayerTouchEnterCommandBehavior
    public class ImageryLayerTouchEnterCommandBehavior : ImageryLayerTouchEnterCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerTouchLeaveCommand
	// ImageryLayerTouchLeaveCommand
	public class ImageryLayerTouchLeaveCommand : ImageryLayerCommandBase<ImageryLayerTouchLeaveCommandBehavior>
	{ }

    public class ImageryLayerTouchLeaveCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerTouchLeaveCommandBehavior<T>,new()
    { }

    public class ImageryLayerTouchLeaveCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, TouchEventArgs>
    {
        public ImageryLayerTouchLeaveCommandBehavior(Func<object, TouchEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerTouchLeaveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.TouchLeave += OnEventRaised;
        }
    }

	// ImageryLayerTouchLeaveCommandBehavior
    public class ImageryLayerTouchLeaveCommandBehavior : ImageryLayerTouchLeaveCommandBehavior<object>
    { }
	#endregion
#endif
	#region ImageryLayerIsMouseDirectlyOverChangedCommand
	// ImageryLayerIsMouseDirectlyOverChangedCommand
	public class ImageryLayerIsMouseDirectlyOverChangedCommand : ImageryLayerCommandBase<ImageryLayerIsMouseDirectlyOverChangedCommandBehavior>
	{ }

    public class ImageryLayerIsMouseDirectlyOverChangedCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerIsMouseDirectlyOverChangedCommandBehavior<T>,new()
    { }

    public class ImageryLayerIsMouseDirectlyOverChangedCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public ImageryLayerIsMouseDirectlyOverChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerIsMouseDirectlyOverChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsMouseDirectlyOverChanged += OnEventRaised;
        }
    }

	// ImageryLayerIsMouseDirectlyOverChangedCommandBehavior
    public class ImageryLayerIsMouseDirectlyOverChangedCommandBehavior : ImageryLayerIsMouseDirectlyOverChangedCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerIsKeyboardFocusWithinChangedCommand
	// ImageryLayerIsKeyboardFocusWithinChangedCommand
	public class ImageryLayerIsKeyboardFocusWithinChangedCommand : ImageryLayerCommandBase<ImageryLayerIsKeyboardFocusWithinChangedCommandBehavior>
	{ }

    public class ImageryLayerIsKeyboardFocusWithinChangedCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerIsKeyboardFocusWithinChangedCommandBehavior<T>,new()
    { }

    public class ImageryLayerIsKeyboardFocusWithinChangedCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public ImageryLayerIsKeyboardFocusWithinChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerIsKeyboardFocusWithinChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsKeyboardFocusWithinChanged += OnEventRaised;
        }
    }

	// ImageryLayerIsKeyboardFocusWithinChangedCommandBehavior
    public class ImageryLayerIsKeyboardFocusWithinChangedCommandBehavior : ImageryLayerIsKeyboardFocusWithinChangedCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerIsMouseCapturedChangedCommand
	// ImageryLayerIsMouseCapturedChangedCommand
	public class ImageryLayerIsMouseCapturedChangedCommand : ImageryLayerCommandBase<ImageryLayerIsMouseCapturedChangedCommandBehavior>
	{ }

    public class ImageryLayerIsMouseCapturedChangedCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerIsMouseCapturedChangedCommandBehavior<T>,new()
    { }

    public class ImageryLayerIsMouseCapturedChangedCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public ImageryLayerIsMouseCapturedChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerIsMouseCapturedChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsMouseCapturedChanged += OnEventRaised;
        }
    }

	// ImageryLayerIsMouseCapturedChangedCommandBehavior
    public class ImageryLayerIsMouseCapturedChangedCommandBehavior : ImageryLayerIsMouseCapturedChangedCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerIsMouseCaptureWithinChangedCommand
	// ImageryLayerIsMouseCaptureWithinChangedCommand
	public class ImageryLayerIsMouseCaptureWithinChangedCommand : ImageryLayerCommandBase<ImageryLayerIsMouseCaptureWithinChangedCommandBehavior>
	{ }

    public class ImageryLayerIsMouseCaptureWithinChangedCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerIsMouseCaptureWithinChangedCommandBehavior<T>,new()
    { }

    public class ImageryLayerIsMouseCaptureWithinChangedCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public ImageryLayerIsMouseCaptureWithinChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerIsMouseCaptureWithinChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsMouseCaptureWithinChanged += OnEventRaised;
        }
    }

	// ImageryLayerIsMouseCaptureWithinChangedCommandBehavior
    public class ImageryLayerIsMouseCaptureWithinChangedCommandBehavior : ImageryLayerIsMouseCaptureWithinChangedCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerIsStylusDirectlyOverChangedCommand
	// ImageryLayerIsStylusDirectlyOverChangedCommand
	public class ImageryLayerIsStylusDirectlyOverChangedCommand : ImageryLayerCommandBase<ImageryLayerIsStylusDirectlyOverChangedCommandBehavior>
	{ }

    public class ImageryLayerIsStylusDirectlyOverChangedCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerIsStylusDirectlyOverChangedCommandBehavior<T>,new()
    { }

    public class ImageryLayerIsStylusDirectlyOverChangedCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public ImageryLayerIsStylusDirectlyOverChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerIsStylusDirectlyOverChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsStylusDirectlyOverChanged += OnEventRaised;
        }
    }

	// ImageryLayerIsStylusDirectlyOverChangedCommandBehavior
    public class ImageryLayerIsStylusDirectlyOverChangedCommandBehavior : ImageryLayerIsStylusDirectlyOverChangedCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerIsStylusCapturedChangedCommand
	// ImageryLayerIsStylusCapturedChangedCommand
	public class ImageryLayerIsStylusCapturedChangedCommand : ImageryLayerCommandBase<ImageryLayerIsStylusCapturedChangedCommandBehavior>
	{ }

    public class ImageryLayerIsStylusCapturedChangedCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerIsStylusCapturedChangedCommandBehavior<T>,new()
    { }

    public class ImageryLayerIsStylusCapturedChangedCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public ImageryLayerIsStylusCapturedChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerIsStylusCapturedChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsStylusCapturedChanged += OnEventRaised;
        }
    }

	// ImageryLayerIsStylusCapturedChangedCommandBehavior
    public class ImageryLayerIsStylusCapturedChangedCommandBehavior : ImageryLayerIsStylusCapturedChangedCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerIsStylusCaptureWithinChangedCommand
	// ImageryLayerIsStylusCaptureWithinChangedCommand
	public class ImageryLayerIsStylusCaptureWithinChangedCommand : ImageryLayerCommandBase<ImageryLayerIsStylusCaptureWithinChangedCommandBehavior>
	{ }

    public class ImageryLayerIsStylusCaptureWithinChangedCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerIsStylusCaptureWithinChangedCommandBehavior<T>,new()
    { }

    public class ImageryLayerIsStylusCaptureWithinChangedCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public ImageryLayerIsStylusCaptureWithinChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerIsStylusCaptureWithinChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsStylusCaptureWithinChanged += OnEventRaised;
        }
    }

	// ImageryLayerIsStylusCaptureWithinChangedCommandBehavior
    public class ImageryLayerIsStylusCaptureWithinChangedCommandBehavior : ImageryLayerIsStylusCaptureWithinChangedCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerIsKeyboardFocusedChangedCommand
	// ImageryLayerIsKeyboardFocusedChangedCommand
	public class ImageryLayerIsKeyboardFocusedChangedCommand : ImageryLayerCommandBase<ImageryLayerIsKeyboardFocusedChangedCommandBehavior>
	{ }

    public class ImageryLayerIsKeyboardFocusedChangedCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerIsKeyboardFocusedChangedCommandBehavior<T>,new()
    { }

    public class ImageryLayerIsKeyboardFocusedChangedCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public ImageryLayerIsKeyboardFocusedChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerIsKeyboardFocusedChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsKeyboardFocusedChanged += OnEventRaised;
        }
    }

	// ImageryLayerIsKeyboardFocusedChangedCommandBehavior
    public class ImageryLayerIsKeyboardFocusedChangedCommandBehavior : ImageryLayerIsKeyboardFocusedChangedCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerLayoutUpdatedCommand
	// ImageryLayerLayoutUpdatedCommand
	public class ImageryLayerLayoutUpdatedCommand : ImageryLayerCommandBase<ImageryLayerLayoutUpdatedCommandBehavior>
	{ }

    public class ImageryLayerLayoutUpdatedCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerLayoutUpdatedCommandBehavior<T>,new()
    { }

    public class ImageryLayerLayoutUpdatedCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, EventArgs>
    {
        public ImageryLayerLayoutUpdatedCommandBehavior(Func<object, EventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerLayoutUpdatedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.LayoutUpdated += OnEventRaised;
        }
    }

	// ImageryLayerLayoutUpdatedCommandBehavior
    public class ImageryLayerLayoutUpdatedCommandBehavior : ImageryLayerLayoutUpdatedCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerGotFocusCommand
	// ImageryLayerGotFocusCommand
	public class ImageryLayerGotFocusCommand : ImageryLayerCommandBase<ImageryLayerGotFocusCommandBehavior>
	{ }

    public class ImageryLayerGotFocusCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerGotFocusCommandBehavior<T>,new()
    { }

    public class ImageryLayerGotFocusCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, RoutedEventArgs>
    {
        public ImageryLayerGotFocusCommandBehavior(Func<object, RoutedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerGotFocusCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.GotFocus += OnEventRaised;
        }
    }

	// ImageryLayerGotFocusCommandBehavior
    public class ImageryLayerGotFocusCommandBehavior : ImageryLayerGotFocusCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerLostFocusCommand
	// ImageryLayerLostFocusCommand
	public class ImageryLayerLostFocusCommand : ImageryLayerCommandBase<ImageryLayerLostFocusCommandBehavior>
	{ }

    public class ImageryLayerLostFocusCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerLostFocusCommandBehavior<T>,new()
    { }

    public class ImageryLayerLostFocusCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, RoutedEventArgs>
    {
        public ImageryLayerLostFocusCommandBehavior(Func<object, RoutedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerLostFocusCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.LostFocus += OnEventRaised;
        }
    }

	// ImageryLayerLostFocusCommandBehavior
    public class ImageryLayerLostFocusCommandBehavior : ImageryLayerLostFocusCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerIsEnabledChangedCommand
	// ImageryLayerIsEnabledChangedCommand
	public class ImageryLayerIsEnabledChangedCommand : ImageryLayerCommandBase<ImageryLayerIsEnabledChangedCommandBehavior>
	{ }

    public class ImageryLayerIsEnabledChangedCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerIsEnabledChangedCommandBehavior<T>,new()
    { }

    public class ImageryLayerIsEnabledChangedCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public ImageryLayerIsEnabledChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerIsEnabledChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsEnabledChanged += OnEventRaised;
        }
    }

	// ImageryLayerIsEnabledChangedCommandBehavior
    public class ImageryLayerIsEnabledChangedCommandBehavior : ImageryLayerIsEnabledChangedCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerIsHitTestVisibleChangedCommand
	// ImageryLayerIsHitTestVisibleChangedCommand
	public class ImageryLayerIsHitTestVisibleChangedCommand : ImageryLayerCommandBase<ImageryLayerIsHitTestVisibleChangedCommandBehavior>
	{ }

    public class ImageryLayerIsHitTestVisibleChangedCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerIsHitTestVisibleChangedCommandBehavior<T>,new()
    { }

    public class ImageryLayerIsHitTestVisibleChangedCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public ImageryLayerIsHitTestVisibleChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerIsHitTestVisibleChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsHitTestVisibleChanged += OnEventRaised;
        }
    }

	// ImageryLayerIsHitTestVisibleChangedCommandBehavior
    public class ImageryLayerIsHitTestVisibleChangedCommandBehavior : ImageryLayerIsHitTestVisibleChangedCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerIsVisibleChangedCommand
	// ImageryLayerIsVisibleChangedCommand
	public class ImageryLayerIsVisibleChangedCommand : ImageryLayerCommandBase<ImageryLayerIsVisibleChangedCommandBehavior>
	{ }

    public class ImageryLayerIsVisibleChangedCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerIsVisibleChangedCommandBehavior<T>,new()
    { }

    public class ImageryLayerIsVisibleChangedCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public ImageryLayerIsVisibleChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerIsVisibleChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsVisibleChanged += OnEventRaised;
        }
    }

	// ImageryLayerIsVisibleChangedCommandBehavior
    public class ImageryLayerIsVisibleChangedCommandBehavior : ImageryLayerIsVisibleChangedCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerFocusableChangedCommand
	// ImageryLayerFocusableChangedCommand
	public class ImageryLayerFocusableChangedCommand : ImageryLayerCommandBase<ImageryLayerFocusableChangedCommandBehavior>
	{ }

    public class ImageryLayerFocusableChangedCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerFocusableChangedCommandBehavior<T>,new()
    { }

    public class ImageryLayerFocusableChangedCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public ImageryLayerFocusableChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerFocusableChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.FocusableChanged += OnEventRaised;
        }
    }

	// ImageryLayerFocusableChangedCommandBehavior
    public class ImageryLayerFocusableChangedCommandBehavior : ImageryLayerFocusableChangedCommandBehavior<object>
    { }
	#endregion
#if SyncfusionFramework4_0
	#region ImageryLayerManipulationStartingCommand
	// ImageryLayerManipulationStartingCommand
	public class ImageryLayerManipulationStartingCommand : ImageryLayerCommandBase<ImageryLayerManipulationStartingCommandBehavior>
	{ }

    public class ImageryLayerManipulationStartingCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerManipulationStartingCommandBehavior<T>,new()
    { }

    public class ImageryLayerManipulationStartingCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, ManipulationStartingEventArgs>
    {
        public ImageryLayerManipulationStartingCommandBehavior(Func<object, ManipulationStartingEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerManipulationStartingCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ManipulationStarting += OnEventRaised;
        }
    }

	// ImageryLayerManipulationStartingCommandBehavior
    public class ImageryLayerManipulationStartingCommandBehavior : ImageryLayerManipulationStartingCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerManipulationStartedCommand
	// ImageryLayerManipulationStartedCommand
	public class ImageryLayerManipulationStartedCommand : ImageryLayerCommandBase<ImageryLayerManipulationStartedCommandBehavior>
	{ }

    public class ImageryLayerManipulationStartedCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerManipulationStartedCommandBehavior<T>,new()
    { }

    public class ImageryLayerManipulationStartedCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, ManipulationStartedEventArgs>
    {
        public ImageryLayerManipulationStartedCommandBehavior(Func<object, ManipulationStartedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerManipulationStartedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ManipulationStarted += OnEventRaised;
        }
    }

	// ImageryLayerManipulationStartedCommandBehavior
    public class ImageryLayerManipulationStartedCommandBehavior : ImageryLayerManipulationStartedCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerManipulationDeltaCommand
	// ImageryLayerManipulationDeltaCommand
	public class ImageryLayerManipulationDeltaCommand : ImageryLayerCommandBase<ImageryLayerManipulationDeltaCommandBehavior>
	{ }

    public class ImageryLayerManipulationDeltaCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerManipulationDeltaCommandBehavior<T>,new()
    { }

    public class ImageryLayerManipulationDeltaCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, ManipulationDeltaEventArgs>
    {
        public ImageryLayerManipulationDeltaCommandBehavior(Func<object, ManipulationDeltaEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerManipulationDeltaCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ManipulationDelta += OnEventRaised;
        }
    }

	// ImageryLayerManipulationDeltaCommandBehavior
    public class ImageryLayerManipulationDeltaCommandBehavior : ImageryLayerManipulationDeltaCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerManipulationInertiaStartingCommand
	// ImageryLayerManipulationInertiaStartingCommand
	public class ImageryLayerManipulationInertiaStartingCommand : ImageryLayerCommandBase<ImageryLayerManipulationInertiaStartingCommandBehavior>
	{ }

    public class ImageryLayerManipulationInertiaStartingCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerManipulationInertiaStartingCommandBehavior<T>,new()
    { }

    public class ImageryLayerManipulationInertiaStartingCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, ManipulationInertiaStartingEventArgs>
    {
        public ImageryLayerManipulationInertiaStartingCommandBehavior(Func<object, ManipulationInertiaStartingEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerManipulationInertiaStartingCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ManipulationInertiaStarting += OnEventRaised;
        }
    }

	// ImageryLayerManipulationInertiaStartingCommandBehavior
    public class ImageryLayerManipulationInertiaStartingCommandBehavior : ImageryLayerManipulationInertiaStartingCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerManipulationBoundaryFeedbackCommand
	// ImageryLayerManipulationBoundaryFeedbackCommand
	public class ImageryLayerManipulationBoundaryFeedbackCommand : ImageryLayerCommandBase<ImageryLayerManipulationBoundaryFeedbackCommandBehavior>
	{ }

    public class ImageryLayerManipulationBoundaryFeedbackCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerManipulationBoundaryFeedbackCommandBehavior<T>,new()
    { }

    public class ImageryLayerManipulationBoundaryFeedbackCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, ManipulationBoundaryFeedbackEventArgs>
    {
        public ImageryLayerManipulationBoundaryFeedbackCommandBehavior(Func<object, ManipulationBoundaryFeedbackEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerManipulationBoundaryFeedbackCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ManipulationBoundaryFeedback += OnEventRaised;
        }
    }

	// ImageryLayerManipulationBoundaryFeedbackCommandBehavior
    public class ImageryLayerManipulationBoundaryFeedbackCommandBehavior : ImageryLayerManipulationBoundaryFeedbackCommandBehavior<object>
    { }
	#endregion

	#region ImageryLayerManipulationCompletedCommand
	// ImageryLayerManipulationCompletedCommand
	public class ImageryLayerManipulationCompletedCommand : ImageryLayerCommandBase<ImageryLayerManipulationCompletedCommandBehavior>
	{ }

    public class ImageryLayerManipulationCompletedCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerManipulationCompletedCommandBehavior<T>,new()
    { }

    public class ImageryLayerManipulationCompletedCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, ManipulationCompletedEventArgs>
    {
        public ImageryLayerManipulationCompletedCommandBehavior(Func<object, ManipulationCompletedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerManipulationCompletedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ManipulationCompleted += OnEventRaised;
        }
    }

	// ImageryLayerManipulationCompletedCommandBehavior
    public class ImageryLayerManipulationCompletedCommandBehavior : ImageryLayerManipulationCompletedCommandBehavior<object>
    { }
	#endregion
#endif
}


