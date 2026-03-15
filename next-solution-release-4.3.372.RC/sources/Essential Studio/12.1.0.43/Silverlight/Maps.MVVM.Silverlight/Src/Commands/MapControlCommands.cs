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

	#region MapControlBindingValidationErrorCommand
	// MapControlBindingValidationErrorCommand
	public class MapControlBindingValidationErrorCommand : MapControlCommandBase<MapControlBindingValidationErrorCommandBehavior>
	{ }

    public class MapControlBindingValidationErrorCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlBindingValidationErrorCommandBehavior<T>,new()
    { }

    public class MapControlBindingValidationErrorCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, ValidationErrorEventArgs>
    {
        public MapControlBindingValidationErrorCommandBehavior(Func<object, ValidationErrorEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlBindingValidationErrorCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.BindingValidationError += OnEventRaised;
        }
    }

	// MapControlBindingValidationErrorCommandBehavior
    public class MapControlBindingValidationErrorCommandBehavior : MapControlBindingValidationErrorCommandBehavior<object>
    { }
	#endregion

#if SILVERLIGHT5

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

#endif

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

#if SILVERLIGHT5

	#region MapControlMediaCommandCommand
	// MapControlMediaCommandCommand
	public class MapControlMediaCommandCommand : MapControlCommandBase<MapControlMediaCommandCommandBehavior>
	{ }

    public class MapControlMediaCommandCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlMediaCommandCommandBehavior<T>,new()
    { }

    public class MapControlMediaCommandCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, MediaCommandEventArgs>
    {
        public MapControlMediaCommandCommandBehavior(Func<object, MediaCommandEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlMediaCommandCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MediaCommand += OnEventRaised;
        }
    }

	// MapControlMediaCommandCommandBehavior
    public class MapControlMediaCommandCommandBehavior : MapControlMediaCommandCommandBehavior<object>
    { }
	#endregion

#endif

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

	#region MapControlTextInputStartCommand
	// MapControlTextInputStartCommand
	public class MapControlTextInputStartCommand : MapControlCommandBase<MapControlTextInputStartCommandBehavior>
	{ }

    public class MapControlTextInputStartCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlTextInputStartCommandBehavior<T>,new()
    { }

    public class MapControlTextInputStartCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, TextCompositionEventArgs>
    {
        public MapControlTextInputStartCommandBehavior(Func<object, TextCompositionEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlTextInputStartCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.TextInputStart += OnEventRaised;
        }
    }

	// MapControlTextInputStartCommandBehavior
    public class MapControlTextInputStartCommandBehavior : MapControlTextInputStartCommandBehavior<object>
    { }
	#endregion

	#region MapControlTextInputUpdateCommand
	// MapControlTextInputUpdateCommand
	public class MapControlTextInputUpdateCommand : MapControlCommandBase<MapControlTextInputUpdateCommandBehavior>
	{ }

    public class MapControlTextInputUpdateCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlTextInputUpdateCommandBehavior<T>,new()
    { }

    public class MapControlTextInputUpdateCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, TextCompositionEventArgs>
    {
        public MapControlTextInputUpdateCommandBehavior(Func<object, TextCompositionEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlTextInputUpdateCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.TextInputUpdate += OnEventRaised;
        }
    }

	// MapControlTextInputUpdateCommandBehavior
    public class MapControlTextInputUpdateCommandBehavior : MapControlTextInputUpdateCommandBehavior<object>
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

#if SILVERLIGHT5

	#region MapControlTapCommand
	// MapControlTapCommand
	public class MapControlTapCommand : MapControlCommandBase<MapControlTapCommandBehavior>
	{ }

    public class MapControlTapCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlTapCommandBehavior<T>,new()
    { }

    public class MapControlTapCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, GestureEventArgs>
    {
        public MapControlTapCommandBehavior(Func<object, GestureEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlTapCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Tap += OnEventRaised;
        }
    }

	// MapControlTapCommandBehavior
    public class MapControlTapCommandBehavior : MapControlTapCommandBehavior<object>
    { }
	#endregion

	#region MapControlDoubleTapCommand
	// MapControlDoubleTapCommand
	public class MapControlDoubleTapCommand : MapControlCommandBase<MapControlDoubleTapCommandBehavior>
	{ }

    public class MapControlDoubleTapCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlDoubleTapCommandBehavior<T>,new()
    { }

    public class MapControlDoubleTapCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, GestureEventArgs>
    {
        public MapControlDoubleTapCommandBehavior(Func<object, GestureEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlDoubleTapCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.DoubleTap += OnEventRaised;
        }
    }

	// MapControlDoubleTapCommandBehavior
    public class MapControlDoubleTapCommandBehavior : MapControlDoubleTapCommandBehavior<object>
    { }
	#endregion



	#region MapControlHoldCommand
	// MapControlHoldCommand
	public class MapControlHoldCommand : MapControlCommandBase<MapControlHoldCommandBehavior>
	{ }

    public class MapControlHoldCommand<T, TBehavior> : MapControlCommandBase<TBehavior> where TBehavior : MapControlHoldCommandBehavior<T>,new()
    { }

    public class MapControlHoldCommandBehavior<TReturn> : MapControlCommandBehaviorBase<TReturn, GestureEventArgs>
    {
        public MapControlHoldCommandBehavior(Func<object, GestureEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public MapControlHoldCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Hold += OnEventRaised;
        }
    }

	// MapControlHoldCommandBehavior
    public class MapControlHoldCommandBehavior : MapControlHoldCommandBehavior<object>
    { }
	#endregion
    #endif
}


