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

using Syncfusion.Windows.Controls.Map;
using System.Windows.Media;

namespace Syncfusion.Maps.MVVM
{   

	#region ShapeFileLayerSelectionChangedCommand
	// ShapeFileLayerSelectionChangedCommand
	public class ShapeFileLayerSelectionChangedCommand : ShapeFileLayerCommandBase<ShapeFileLayerSelectionChangedCommandBehavior>
	{ }

    public class ShapeFileLayerSelectionChangedCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerSelectionChangedCommandBehavior<T>,new()
    { }

    public class ShapeFileLayerSelectionChangedCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, SelectionChangedEventArgs>
    {
        public ShapeFileLayerSelectionChangedCommandBehavior(Func<object, SelectionChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerSelectionChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.SelectionChanged += OnEventRaised;
        }
    }

	// ShapeFileLayerSelectionChangedCommandBehavior
    public class ShapeFileLayerSelectionChangedCommandBehavior : ShapeFileLayerSelectionChangedCommandBehavior<object>
    { }
	#endregion

	#region ShapeFileLayerPreviewZoomInCommand
	// ShapeFileLayerPreviewZoomInCommand
	public class ShapeFileLayerPreviewZoomInCommand : ShapeFileLayerCommandBase<ShapeFileLayerPreviewZoomInCommandBehavior>
	{ }

    public class ShapeFileLayerPreviewZoomInCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerPreviewZoomInCommandBehavior<T>,new()
    { }

    public class ShapeFileLayerPreviewZoomInCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, ZoomEventArgs>
    {
        public ShapeFileLayerPreviewZoomInCommandBehavior(Func<object, ZoomEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerPreviewZoomInCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewZoomIn += OnEventRaised;
        }
    }

	// ShapeFileLayerPreviewZoomInCommandBehavior
    public class ShapeFileLayerPreviewZoomInCommandBehavior : ShapeFileLayerPreviewZoomInCommandBehavior<object>
    { }
	#endregion

	#region ShapeFileLayerZoomedInCommand
	// ShapeFileLayerZoomedInCommand
	public class ShapeFileLayerZoomedInCommand : ShapeFileLayerCommandBase<ShapeFileLayerZoomedInCommandBehavior>
	{ }

    public class ShapeFileLayerZoomedInCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerZoomedInCommandBehavior<T>,new()
    { }

    public class ShapeFileLayerZoomedInCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, ZoomEventArgs>
    {
        public ShapeFileLayerZoomedInCommandBehavior(Func<object, ZoomEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerZoomedInCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ZoomedIn += OnEventRaised;
        }
    }

	// ShapeFileLayerZoomedInCommandBehavior
    public class ShapeFileLayerZoomedInCommandBehavior : ShapeFileLayerZoomedInCommandBehavior<object>
    { }
	#endregion

	#region ShapeFileLayerPreviewZoomOutCommand
	// ShapeFileLayerPreviewZoomOutCommand
	public class ShapeFileLayerPreviewZoomOutCommand : ShapeFileLayerCommandBase<ShapeFileLayerPreviewZoomOutCommandBehavior>
	{ }

    public class ShapeFileLayerPreviewZoomOutCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerPreviewZoomOutCommandBehavior<T>,new()
    { }

    public class ShapeFileLayerPreviewZoomOutCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, ZoomEventArgs>
    {
        public ShapeFileLayerPreviewZoomOutCommandBehavior(Func<object, ZoomEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerPreviewZoomOutCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewZoomOut += OnEventRaised;
        }
    }

	// ShapeFileLayerPreviewZoomOutCommandBehavior
    public class ShapeFileLayerPreviewZoomOutCommandBehavior : ShapeFileLayerPreviewZoomOutCommandBehavior<object>
    { }
	#endregion

	#region ShapeFileLayerZoomedOutCommand
	// ShapeFileLayerZoomedOutCommand
	public class ShapeFileLayerZoomedOutCommand : ShapeFileLayerCommandBase<ShapeFileLayerZoomedOutCommandBehavior>
	{ }

    public class ShapeFileLayerZoomedOutCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerZoomedOutCommandBehavior<T>,new()
    { }

    public class ShapeFileLayerZoomedOutCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, ZoomEventArgs>
    {
        public ShapeFileLayerZoomedOutCommandBehavior(Func<object, ZoomEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerZoomedOutCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ZoomedOut += OnEventRaised;
        }
    }

	// ShapeFileLayerZoomedOutCommandBehavior
    public class ShapeFileLayerZoomedOutCommandBehavior : ShapeFileLayerZoomedOutCommandBehavior<object>
    { }
	#endregion

	#region ShapeFileLayerPanningCommand
	// ShapeFileLayerPanningCommand
	public class ShapeFileLayerPanningCommand : ShapeFileLayerCommandBase<ShapeFileLayerPanningCommandBehavior>
	{ }

    public class ShapeFileLayerPanningCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerPanningCommandBehavior<T>,new()
    { }

    public class ShapeFileLayerPanningCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, PanEventArgs>
    {
        public ShapeFileLayerPanningCommandBehavior(Func<object, PanEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerPanningCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Panning += OnEventRaised;
        }
    }

	// ShapeFileLayerPanningCommandBehavior
    public class ShapeFileLayerPanningCommandBehavior : ShapeFileLayerPanningCommandBehavior<object>
    { }
	#endregion

	#region ShapeFileLayerPannedCommand
	// ShapeFileLayerPannedCommand
	public class ShapeFileLayerPannedCommand : ShapeFileLayerCommandBase<ShapeFileLayerPannedCommandBehavior>
	{ }

    public class ShapeFileLayerPannedCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerPannedCommandBehavior<T>,new()
    { }

    public class ShapeFileLayerPannedCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, PanEventArgs>
    {
        public ShapeFileLayerPannedCommandBehavior(Func<object, PanEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerPannedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Panned += OnEventRaised;
        }
    }

	// ShapeFileLayerPannedCommandBehavior
    public class ShapeFileLayerPannedCommandBehavior : ShapeFileLayerPannedCommandBehavior<object>
    { }
	#endregion

	#region ShapeFileLayerIsEnabledChangedCommand
	// ShapeFileLayerIsEnabledChangedCommand
	public class ShapeFileLayerIsEnabledChangedCommand : ShapeFileLayerCommandBase<ShapeFileLayerIsEnabledChangedCommandBehavior>
	{ }

    public class ShapeFileLayerIsEnabledChangedCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerIsEnabledChangedCommandBehavior<T>,new()
    { }

    public class ShapeFileLayerIsEnabledChangedCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public ShapeFileLayerIsEnabledChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerIsEnabledChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsEnabledChanged += OnEventRaised;
        }
    }

	// ShapeFileLayerIsEnabledChangedCommandBehavior
    public class ShapeFileLayerIsEnabledChangedCommandBehavior : ShapeFileLayerIsEnabledChangedCommandBehavior<object>
    { }
	#endregion

	#region ShapeFileLayerLoadedCommand
	// ShapeFileLayerLoadedCommand
	public class ShapeFileLayerLoadedCommand : ShapeFileLayerCommandBase<ShapeFileLayerLoadedCommandBehavior>
	{ }

    public class ShapeFileLayerLoadedCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerLoadedCommandBehavior<T>,new()
    { }

    public class ShapeFileLayerLoadedCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, RoutedEventArgs>
    {
        public ShapeFileLayerLoadedCommandBehavior(Func<object, RoutedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerLoadedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Loaded += OnEventRaised;
        }
    }

	// ShapeFileLayerLoadedCommandBehavior
    public class ShapeFileLayerLoadedCommandBehavior : ShapeFileLayerLoadedCommandBehavior<object>
    { }
	#endregion

	#region ShapeFileLayerUnloadedCommand
	// ShapeFileLayerUnloadedCommand
	public class ShapeFileLayerUnloadedCommand : ShapeFileLayerCommandBase<ShapeFileLayerUnloadedCommandBehavior>
	{ }

    public class ShapeFileLayerUnloadedCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerUnloadedCommandBehavior<T>,new()
    { }

    public class ShapeFileLayerUnloadedCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, RoutedEventArgs>
    {
        public ShapeFileLayerUnloadedCommandBehavior(Func<object, RoutedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerUnloadedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Unloaded += OnEventRaised;
        }
    }

	// ShapeFileLayerUnloadedCommandBehavior
    public class ShapeFileLayerUnloadedCommandBehavior : ShapeFileLayerUnloadedCommandBehavior<object>
    { }
	#endregion

	#region ShapeFileLayerSizeChangedCommand
	// ShapeFileLayerSizeChangedCommand
	public class ShapeFileLayerSizeChangedCommand : ShapeFileLayerCommandBase<ShapeFileLayerSizeChangedCommandBehavior>
	{ }

    public class ShapeFileLayerSizeChangedCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerSizeChangedCommandBehavior<T>,new()
    { }

    public class ShapeFileLayerSizeChangedCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, SizeChangedEventArgs>
    {
        public ShapeFileLayerSizeChangedCommandBehavior(Func<object, SizeChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerSizeChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.SizeChanged += OnEventRaised;
        }
    }

	// ShapeFileLayerSizeChangedCommandBehavior
    public class ShapeFileLayerSizeChangedCommandBehavior : ShapeFileLayerSizeChangedCommandBehavior<object>
    { }
	#endregion

	#region ShapeFileLayerLayoutUpdatedCommand
	// ShapeFileLayerLayoutUpdatedCommand
	public class ShapeFileLayerLayoutUpdatedCommand : ShapeFileLayerCommandBase<ShapeFileLayerLayoutUpdatedCommandBehavior>
	{ }

    public class ShapeFileLayerLayoutUpdatedCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerLayoutUpdatedCommandBehavior<T>,new()
    { }

    public class ShapeFileLayerLayoutUpdatedCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, EventArgs>
    {
        public ShapeFileLayerLayoutUpdatedCommandBehavior(Func<object, EventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerLayoutUpdatedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.LayoutUpdated += OnEventRaised;
        }
    }

	// ShapeFileLayerLayoutUpdatedCommandBehavior
    public class ShapeFileLayerLayoutUpdatedCommandBehavior : ShapeFileLayerLayoutUpdatedCommandBehavior<object>
    { }
	#endregion

	#region ShapeFileLayerBindingValidationErrorCommand
	// ShapeFileLayerBindingValidationErrorCommand
	public class ShapeFileLayerBindingValidationErrorCommand : ShapeFileLayerCommandBase<ShapeFileLayerBindingValidationErrorCommandBehavior>
	{ }

    public class ShapeFileLayerBindingValidationErrorCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerBindingValidationErrorCommandBehavior<T>,new()
    { }

    public class ShapeFileLayerBindingValidationErrorCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, ValidationErrorEventArgs>
    {
        public ShapeFileLayerBindingValidationErrorCommandBehavior(Func<object, ValidationErrorEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerBindingValidationErrorCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.BindingValidationError += OnEventRaised;
        }
    }

	// ShapeFileLayerBindingValidationErrorCommandBehavior
    public class ShapeFileLayerBindingValidationErrorCommandBehavior : ShapeFileLayerBindingValidationErrorCommandBehavior<object>
    { }
	#endregion

#if SILVERLIGHT5

	#region ShapeFileLayerDataContextChangedCommand
	// ShapeFileLayerDataContextChangedCommand
	public class ShapeFileLayerDataContextChangedCommand : ShapeFileLayerCommandBase<ShapeFileLayerDataContextChangedCommandBehavior>
	{ }

    public class ShapeFileLayerDataContextChangedCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerDataContextChangedCommandBehavior<T>,new()
    { }

    public class ShapeFileLayerDataContextChangedCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public ShapeFileLayerDataContextChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerDataContextChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.DataContextChanged += OnEventRaised;
        }
    }

	// ShapeFileLayerDataContextChangedCommandBehavior
    public class ShapeFileLayerDataContextChangedCommandBehavior : ShapeFileLayerDataContextChangedCommandBehavior<object>
    { }
	#endregion
#endif
	#region ShapeFileLayerMouseMoveCommand
	// ShapeFileLayerMouseMoveCommand
	public class ShapeFileLayerMouseMoveCommand : ShapeFileLayerCommandBase<ShapeFileLayerMouseMoveCommandBehavior>
	{ }

    public class ShapeFileLayerMouseMoveCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerMouseMoveCommandBehavior<T>,new()
    { }

    public class ShapeFileLayerMouseMoveCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, MouseEventArgs>
    {
        public ShapeFileLayerMouseMoveCommandBehavior(Func<object, MouseEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerMouseMoveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseMove += OnEventRaised;
        }
    }

	// ShapeFileLayerMouseMoveCommandBehavior
    public class ShapeFileLayerMouseMoveCommandBehavior : ShapeFileLayerMouseMoveCommandBehavior<object>
    { }
	#endregion

	#region ShapeFileLayerMouseEnterCommand
	// ShapeFileLayerMouseEnterCommand
	public class ShapeFileLayerMouseEnterCommand : ShapeFileLayerCommandBase<ShapeFileLayerMouseEnterCommandBehavior>
	{ }

    public class ShapeFileLayerMouseEnterCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerMouseEnterCommandBehavior<T>,new()
    { }

    public class ShapeFileLayerMouseEnterCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, MouseEventArgs>
    {
        public ShapeFileLayerMouseEnterCommandBehavior(Func<object, MouseEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerMouseEnterCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseEnter += OnEventRaised;
        }
    }

	// ShapeFileLayerMouseEnterCommandBehavior
    public class ShapeFileLayerMouseEnterCommandBehavior : ShapeFileLayerMouseEnterCommandBehavior<object>
    { }
	#endregion

	#region ShapeFileLayerMouseLeaveCommand
	// ShapeFileLayerMouseLeaveCommand
	public class ShapeFileLayerMouseLeaveCommand : ShapeFileLayerCommandBase<ShapeFileLayerMouseLeaveCommandBehavior>
	{ }

    public class ShapeFileLayerMouseLeaveCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerMouseLeaveCommandBehavior<T>,new()
    { }

    public class ShapeFileLayerMouseLeaveCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, MouseEventArgs>
    {
        public ShapeFileLayerMouseLeaveCommandBehavior(Func<object, MouseEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerMouseLeaveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseLeave += OnEventRaised;
        }
    }

	// ShapeFileLayerMouseLeaveCommandBehavior
    public class ShapeFileLayerMouseLeaveCommandBehavior : ShapeFileLayerMouseLeaveCommandBehavior<object>
    { }
	#endregion

	#region ShapeFileLayerMouseLeftButtonDownCommand
	// ShapeFileLayerMouseLeftButtonDownCommand
	public class ShapeFileLayerMouseLeftButtonDownCommand : ShapeFileLayerCommandBase<ShapeFileLayerMouseLeftButtonDownCommandBehavior>
	{ }

    public class ShapeFileLayerMouseLeftButtonDownCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerMouseLeftButtonDownCommandBehavior<T>,new()
    { }

    public class ShapeFileLayerMouseLeftButtonDownCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public ShapeFileLayerMouseLeftButtonDownCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerMouseLeftButtonDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseLeftButtonDown += OnEventRaised;
        }
    }

	// ShapeFileLayerMouseLeftButtonDownCommandBehavior
    public class ShapeFileLayerMouseLeftButtonDownCommandBehavior : ShapeFileLayerMouseLeftButtonDownCommandBehavior<object>
    { }
	#endregion

	#region ShapeFileLayerMouseLeftButtonUpCommand
	// ShapeFileLayerMouseLeftButtonUpCommand
	public class ShapeFileLayerMouseLeftButtonUpCommand : ShapeFileLayerCommandBase<ShapeFileLayerMouseLeftButtonUpCommandBehavior>
	{ }

    public class ShapeFileLayerMouseLeftButtonUpCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerMouseLeftButtonUpCommandBehavior<T>,new()
    { }

    public class ShapeFileLayerMouseLeftButtonUpCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public ShapeFileLayerMouseLeftButtonUpCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerMouseLeftButtonUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseLeftButtonUp += OnEventRaised;
        }
    }

	// ShapeFileLayerMouseLeftButtonUpCommandBehavior
    public class ShapeFileLayerMouseLeftButtonUpCommandBehavior : ShapeFileLayerMouseLeftButtonUpCommandBehavior<object>
    { }
	#endregion

	#region ShapeFileLayerMouseRightButtonDownCommand
	// ShapeFileLayerMouseRightButtonDownCommand
	public class ShapeFileLayerMouseRightButtonDownCommand : ShapeFileLayerCommandBase<ShapeFileLayerMouseRightButtonDownCommandBehavior>
	{ }

    public class ShapeFileLayerMouseRightButtonDownCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerMouseRightButtonDownCommandBehavior<T>,new()
    { }

    public class ShapeFileLayerMouseRightButtonDownCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public ShapeFileLayerMouseRightButtonDownCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerMouseRightButtonDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseRightButtonDown += OnEventRaised;
        }
    }

	// ShapeFileLayerMouseRightButtonDownCommandBehavior
    public class ShapeFileLayerMouseRightButtonDownCommandBehavior : ShapeFileLayerMouseRightButtonDownCommandBehavior<object>
    { }
	#endregion

	#region ShapeFileLayerMouseRightButtonUpCommand
	// ShapeFileLayerMouseRightButtonUpCommand
	public class ShapeFileLayerMouseRightButtonUpCommand : ShapeFileLayerCommandBase<ShapeFileLayerMouseRightButtonUpCommandBehavior>
	{ }

    public class ShapeFileLayerMouseRightButtonUpCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerMouseRightButtonUpCommandBehavior<T>,new()
    { }

    public class ShapeFileLayerMouseRightButtonUpCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public ShapeFileLayerMouseRightButtonUpCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerMouseRightButtonUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseRightButtonUp += OnEventRaised;
        }
    }

	// ShapeFileLayerMouseRightButtonUpCommandBehavior
    public class ShapeFileLayerMouseRightButtonUpCommandBehavior : ShapeFileLayerMouseRightButtonUpCommandBehavior<object>
    { }
	#endregion

	#region ShapeFileLayerMouseWheelCommand
	// ShapeFileLayerMouseWheelCommand
	public class ShapeFileLayerMouseWheelCommand : ShapeFileLayerCommandBase<ShapeFileLayerMouseWheelCommandBehavior>
	{ }

    public class ShapeFileLayerMouseWheelCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerMouseWheelCommandBehavior<T>,new()
    { }

    public class ShapeFileLayerMouseWheelCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, MouseWheelEventArgs>
    {
        public ShapeFileLayerMouseWheelCommandBehavior(Func<object, MouseWheelEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerMouseWheelCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseWheel += OnEventRaised;
        }
    }

	// ShapeFileLayerMouseWheelCommandBehavior
    public class ShapeFileLayerMouseWheelCommandBehavior : ShapeFileLayerMouseWheelCommandBehavior<object>
    { }
	#endregion

	#region ShapeFileLayerKeyUpCommand
	// ShapeFileLayerKeyUpCommand
	public class ShapeFileLayerKeyUpCommand : ShapeFileLayerCommandBase<ShapeFileLayerKeyUpCommandBehavior>
	{ }

    public class ShapeFileLayerKeyUpCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerKeyUpCommandBehavior<T>,new()
    { }

    public class ShapeFileLayerKeyUpCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, KeyEventArgs>
    {
        public ShapeFileLayerKeyUpCommandBehavior(Func<object, KeyEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerKeyUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.KeyUp += OnEventRaised;
        }
    }

	// ShapeFileLayerKeyUpCommandBehavior
    public class ShapeFileLayerKeyUpCommandBehavior : ShapeFileLayerKeyUpCommandBehavior<object>
    { }
	#endregion

	#region ShapeFileLayerKeyDownCommand
	// ShapeFileLayerKeyDownCommand
	public class ShapeFileLayerKeyDownCommand : ShapeFileLayerCommandBase<ShapeFileLayerKeyDownCommandBehavior>
	{ }

    public class ShapeFileLayerKeyDownCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerKeyDownCommandBehavior<T>,new()
    { }

    public class ShapeFileLayerKeyDownCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, KeyEventArgs>
    {
        public ShapeFileLayerKeyDownCommandBehavior(Func<object, KeyEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerKeyDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.KeyDown += OnEventRaised;
        }
    }

	// ShapeFileLayerKeyDownCommandBehavior
    public class ShapeFileLayerKeyDownCommandBehavior : ShapeFileLayerKeyDownCommandBehavior<object>
    { }
	#endregion

#if SILVERLIGHT5

	#region ShapeFileLayerMediaCommandCommand
	// ShapeFileLayerMediaCommandCommand
	public class ShapeFileLayerMediaCommandCommand : ShapeFileLayerCommandBase<ShapeFileLayerMediaCommandCommandBehavior>
	{ }

    public class ShapeFileLayerMediaCommandCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerMediaCommandCommandBehavior<T>,new()
    { }

    public class ShapeFileLayerMediaCommandCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, MediaCommandEventArgs>
    {
        public ShapeFileLayerMediaCommandCommandBehavior(Func<object, MediaCommandEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerMediaCommandCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MediaCommand += OnEventRaised;
        }
    }

	// ShapeFileLayerMediaCommandCommandBehavior
    public class ShapeFileLayerMediaCommandCommandBehavior : ShapeFileLayerMediaCommandCommandBehavior<object>
    { }
	#endregion

#endif

	#region ShapeFileLayerGotFocusCommand
	// ShapeFileLayerGotFocusCommand
	public class ShapeFileLayerGotFocusCommand : ShapeFileLayerCommandBase<ShapeFileLayerGotFocusCommandBehavior>
	{ }

    public class ShapeFileLayerGotFocusCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerGotFocusCommandBehavior<T>,new()
    { }

    public class ShapeFileLayerGotFocusCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, RoutedEventArgs>
    {
        public ShapeFileLayerGotFocusCommandBehavior(Func<object, RoutedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerGotFocusCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.GotFocus += OnEventRaised;
        }
    }

	// ShapeFileLayerGotFocusCommandBehavior
    public class ShapeFileLayerGotFocusCommandBehavior : ShapeFileLayerGotFocusCommandBehavior<object>
    { }
	#endregion

	#region ShapeFileLayerLostFocusCommand
	// ShapeFileLayerLostFocusCommand
	public class ShapeFileLayerLostFocusCommand : ShapeFileLayerCommandBase<ShapeFileLayerLostFocusCommandBehavior>
	{ }

    public class ShapeFileLayerLostFocusCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerLostFocusCommandBehavior<T>,new()
    { }

    public class ShapeFileLayerLostFocusCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, RoutedEventArgs>
    {
        public ShapeFileLayerLostFocusCommandBehavior(Func<object, RoutedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerLostFocusCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.LostFocus += OnEventRaised;
        }
    }

	// ShapeFileLayerLostFocusCommandBehavior
    public class ShapeFileLayerLostFocusCommandBehavior : ShapeFileLayerLostFocusCommandBehavior<object>
    { }
	#endregion

	#region ShapeFileLayerLostMouseCaptureCommand
	// ShapeFileLayerLostMouseCaptureCommand
	public class ShapeFileLayerLostMouseCaptureCommand : ShapeFileLayerCommandBase<ShapeFileLayerLostMouseCaptureCommandBehavior>
	{ }

    public class ShapeFileLayerLostMouseCaptureCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerLostMouseCaptureCommandBehavior<T>,new()
    { }

    public class ShapeFileLayerLostMouseCaptureCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, MouseEventArgs>
    {
        public ShapeFileLayerLostMouseCaptureCommandBehavior(Func<object, MouseEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerLostMouseCaptureCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.LostMouseCapture += OnEventRaised;
        }
    }

	// ShapeFileLayerLostMouseCaptureCommandBehavior
    public class ShapeFileLayerLostMouseCaptureCommandBehavior : ShapeFileLayerLostMouseCaptureCommandBehavior<object>
    { }
	#endregion

	#region ShapeFileLayerTextInputCommand
	// ShapeFileLayerTextInputCommand
	public class ShapeFileLayerTextInputCommand : ShapeFileLayerCommandBase<ShapeFileLayerTextInputCommandBehavior>
	{ }

    public class ShapeFileLayerTextInputCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerTextInputCommandBehavior<T>,new()
    { }

    public class ShapeFileLayerTextInputCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, TextCompositionEventArgs>
    {
        public ShapeFileLayerTextInputCommandBehavior(Func<object, TextCompositionEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerTextInputCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.TextInput += OnEventRaised;
        }
    }

	// ShapeFileLayerTextInputCommandBehavior
    public class ShapeFileLayerTextInputCommandBehavior : ShapeFileLayerTextInputCommandBehavior<object>
    { }
	#endregion

	#region ShapeFileLayerTextInputStartCommand
	// ShapeFileLayerTextInputStartCommand
	public class ShapeFileLayerTextInputStartCommand : ShapeFileLayerCommandBase<ShapeFileLayerTextInputStartCommandBehavior>
	{ }

    public class ShapeFileLayerTextInputStartCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerTextInputStartCommandBehavior<T>,new()
    { }

    public class ShapeFileLayerTextInputStartCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, TextCompositionEventArgs>
    {
        public ShapeFileLayerTextInputStartCommandBehavior(Func<object, TextCompositionEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerTextInputStartCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.TextInputStart += OnEventRaised;
        }
    }

	// ShapeFileLayerTextInputStartCommandBehavior
    public class ShapeFileLayerTextInputStartCommandBehavior : ShapeFileLayerTextInputStartCommandBehavior<object>
    { }
	#endregion

	#region ShapeFileLayerTextInputUpdateCommand
	// ShapeFileLayerTextInputUpdateCommand
	public class ShapeFileLayerTextInputUpdateCommand : ShapeFileLayerCommandBase<ShapeFileLayerTextInputUpdateCommandBehavior>
	{ }

    public class ShapeFileLayerTextInputUpdateCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerTextInputUpdateCommandBehavior<T>,new()
    { }

    public class ShapeFileLayerTextInputUpdateCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, TextCompositionEventArgs>
    {
        public ShapeFileLayerTextInputUpdateCommandBehavior(Func<object, TextCompositionEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerTextInputUpdateCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.TextInputUpdate += OnEventRaised;
        }
    }

	// ShapeFileLayerTextInputUpdateCommandBehavior
    public class ShapeFileLayerTextInputUpdateCommandBehavior : ShapeFileLayerTextInputUpdateCommandBehavior<object>
    { }
	#endregion

	#region ShapeFileLayerDragEnterCommand
	// ShapeFileLayerDragEnterCommand
	public class ShapeFileLayerDragEnterCommand : ShapeFileLayerCommandBase<ShapeFileLayerDragEnterCommandBehavior>
	{ }

    public class ShapeFileLayerDragEnterCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerDragEnterCommandBehavior<T>,new()
    { }

    public class ShapeFileLayerDragEnterCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, DragEventArgs>
    {
        public ShapeFileLayerDragEnterCommandBehavior(Func<object, DragEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerDragEnterCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.DragEnter += OnEventRaised;
        }
    }

	// ShapeFileLayerDragEnterCommandBehavior
    public class ShapeFileLayerDragEnterCommandBehavior : ShapeFileLayerDragEnterCommandBehavior<object>
    { }
	#endregion

	#region ShapeFileLayerDragLeaveCommand
	// ShapeFileLayerDragLeaveCommand
	public class ShapeFileLayerDragLeaveCommand : ShapeFileLayerCommandBase<ShapeFileLayerDragLeaveCommandBehavior>
	{ }

    public class ShapeFileLayerDragLeaveCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerDragLeaveCommandBehavior<T>,new()
    { }

    public class ShapeFileLayerDragLeaveCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, DragEventArgs>
    {
        public ShapeFileLayerDragLeaveCommandBehavior(Func<object, DragEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerDragLeaveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.DragLeave += OnEventRaised;
        }
    }

	// ShapeFileLayerDragLeaveCommandBehavior
    public class ShapeFileLayerDragLeaveCommandBehavior : ShapeFileLayerDragLeaveCommandBehavior<object>
    { }
	#endregion

	#region ShapeFileLayerDragOverCommand
	// ShapeFileLayerDragOverCommand
	public class ShapeFileLayerDragOverCommand : ShapeFileLayerCommandBase<ShapeFileLayerDragOverCommandBehavior>
	{ }

    public class ShapeFileLayerDragOverCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerDragOverCommandBehavior<T>,new()
    { }

    public class ShapeFileLayerDragOverCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, DragEventArgs>
    {
        public ShapeFileLayerDragOverCommandBehavior(Func<object, DragEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerDragOverCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.DragOver += OnEventRaised;
        }
    }

	// ShapeFileLayerDragOverCommandBehavior
    public class ShapeFileLayerDragOverCommandBehavior : ShapeFileLayerDragOverCommandBehavior<object>
    { }
	#endregion

	#region ShapeFileLayerDropCommand
	// ShapeFileLayerDropCommand
	public class ShapeFileLayerDropCommand : ShapeFileLayerCommandBase<ShapeFileLayerDropCommandBehavior>
	{ }

    public class ShapeFileLayerDropCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerDropCommandBehavior<T>,new()
    { }

    public class ShapeFileLayerDropCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, DragEventArgs>
    {
        public ShapeFileLayerDropCommandBehavior(Func<object, DragEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerDropCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Drop += OnEventRaised;
        }
    }

	// ShapeFileLayerDropCommandBehavior
    public class ShapeFileLayerDropCommandBehavior : ShapeFileLayerDropCommandBehavior<object>
    { }
	#endregion

	#region ShapeFileLayerManipulationStartedCommand
	// ShapeFileLayerManipulationStartedCommand
	public class ShapeFileLayerManipulationStartedCommand : ShapeFileLayerCommandBase<ShapeFileLayerManipulationStartedCommandBehavior>
	{ }

    public class ShapeFileLayerManipulationStartedCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerManipulationStartedCommandBehavior<T>,new()
    { }

    public class ShapeFileLayerManipulationStartedCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, ManipulationStartedEventArgs>
    {
        public ShapeFileLayerManipulationStartedCommandBehavior(Func<object, ManipulationStartedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerManipulationStartedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ManipulationStarted += OnEventRaised;
        }
    }

	// ShapeFileLayerManipulationStartedCommandBehavior
    public class ShapeFileLayerManipulationStartedCommandBehavior : ShapeFileLayerManipulationStartedCommandBehavior<object>
    { }
	#endregion

	#region ShapeFileLayerManipulationDeltaCommand
	// ShapeFileLayerManipulationDeltaCommand
	public class ShapeFileLayerManipulationDeltaCommand : ShapeFileLayerCommandBase<ShapeFileLayerManipulationDeltaCommandBehavior>
	{ }

    public class ShapeFileLayerManipulationDeltaCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerManipulationDeltaCommandBehavior<T>,new()
    { }

    public class ShapeFileLayerManipulationDeltaCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, ManipulationDeltaEventArgs>
    {
        public ShapeFileLayerManipulationDeltaCommandBehavior(Func<object, ManipulationDeltaEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerManipulationDeltaCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ManipulationDelta += OnEventRaised;
        }
    }

	// ShapeFileLayerManipulationDeltaCommandBehavior
    public class ShapeFileLayerManipulationDeltaCommandBehavior : ShapeFileLayerManipulationDeltaCommandBehavior<object>
    { }
	#endregion

	#region ShapeFileLayerManipulationCompletedCommand
	// ShapeFileLayerManipulationCompletedCommand
	public class ShapeFileLayerManipulationCompletedCommand : ShapeFileLayerCommandBase<ShapeFileLayerManipulationCompletedCommandBehavior>
	{ }

    public class ShapeFileLayerManipulationCompletedCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerManipulationCompletedCommandBehavior<T>,new()
    { }

    public class ShapeFileLayerManipulationCompletedCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, ManipulationCompletedEventArgs>
    {
        public ShapeFileLayerManipulationCompletedCommandBehavior(Func<object, ManipulationCompletedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerManipulationCompletedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ManipulationCompleted += OnEventRaised;
        }
    }

	// ShapeFileLayerManipulationCompletedCommandBehavior
    public class ShapeFileLayerManipulationCompletedCommandBehavior : ShapeFileLayerManipulationCompletedCommandBehavior<object>
    { }
	#endregion

#if SILVERLIGHT5

	#region ShapeFileLayerTapCommand
	// ShapeFileLayerTapCommand
	public class ShapeFileLayerTapCommand : ShapeFileLayerCommandBase<ShapeFileLayerTapCommandBehavior>
	{ }

    public class ShapeFileLayerTapCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerTapCommandBehavior<T>,new()
    { }

    public class ShapeFileLayerTapCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, GestureEventArgs>
    {
        public ShapeFileLayerTapCommandBehavior(Func<object, GestureEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerTapCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Tap += OnEventRaised;
        }
    }

	// ShapeFileLayerTapCommandBehavior
    public class ShapeFileLayerTapCommandBehavior : ShapeFileLayerTapCommandBehavior<object>
    { }
	#endregion

	#region ShapeFileLayerDoubleTapCommand
	// ShapeFileLayerDoubleTapCommand
	public class ShapeFileLayerDoubleTapCommand : ShapeFileLayerCommandBase<ShapeFileLayerDoubleTapCommandBehavior>
	{ }

    public class ShapeFileLayerDoubleTapCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerDoubleTapCommandBehavior<T>,new()
    { }

    public class ShapeFileLayerDoubleTapCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, GestureEventArgs>
    {
        public ShapeFileLayerDoubleTapCommandBehavior(Func<object, GestureEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerDoubleTapCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.DoubleTap += OnEventRaised;
        }
    }

	// ShapeFileLayerDoubleTapCommandBehavior
    public class ShapeFileLayerDoubleTapCommandBehavior : ShapeFileLayerDoubleTapCommandBehavior<object>
    { }
	#endregion

	#region ShapeFileLayerHoldCommand
	// ShapeFileLayerHoldCommand
	public class ShapeFileLayerHoldCommand : ShapeFileLayerCommandBase<ShapeFileLayerHoldCommandBehavior>
	{ }

    public class ShapeFileLayerHoldCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerHoldCommandBehavior<T>,new()
    { }

    public class ShapeFileLayerHoldCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, GestureEventArgs>
    {
        public ShapeFileLayerHoldCommandBehavior(Func<object, GestureEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerHoldCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Hold += OnEventRaised;
        }
    }

	// ShapeFileLayerHoldCommandBehavior
    public class ShapeFileLayerHoldCommandBehavior : ShapeFileLayerHoldCommandBehavior<object>
    { }
	#endregion
#endif
}



