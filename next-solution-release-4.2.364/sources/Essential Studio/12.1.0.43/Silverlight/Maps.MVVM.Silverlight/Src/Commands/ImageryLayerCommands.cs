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

    #region ImageryLayerPreviewZoomInCommand
    // ImageryLayerPreviewZoomInCommand
    public class ImageryLayerPreviewZoomInCommand : ImageryLayerCommandBase<ImageryLayerPreviewZoomInCommandBehavior>
    { }

    public class ImageryLayerPreviewZoomInCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerPreviewZoomInCommandBehavior<T>, new()
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

    public class ImageryLayerZoomedInCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerZoomedInCommandBehavior<T>, new()
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

    public class ImageryLayerPreviewZoomOutCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerPreviewZoomOutCommandBehavior<T>, new()
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

    public class ImageryLayerZoomedOutCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerZoomedOutCommandBehavior<T>, new()
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

    public class ImageryLayerPanningCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerPanningCommandBehavior<T>, new()
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

    public class ImageryLayerPannedCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerPannedCommandBehavior<T>, new()
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

    #region ImageryLayerIsEnabledChangedCommand
    // ImageryLayerIsEnabledChangedCommand
    public class ImageryLayerIsEnabledChangedCommand : ImageryLayerCommandBase<ImageryLayerIsEnabledChangedCommandBehavior>
    { }

    public class ImageryLayerIsEnabledChangedCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerIsEnabledChangedCommandBehavior<T>, new()
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

    #region ImageryLayerLoadedCommand
    // ImageryLayerLoadedCommand
    public class ImageryLayerLoadedCommand : ImageryLayerCommandBase<ImageryLayerLoadedCommandBehavior>
    { }

    public class ImageryLayerLoadedCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerLoadedCommandBehavior<T>, new()
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

    public class ImageryLayerUnloadedCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerUnloadedCommandBehavior<T>, new()
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

    #region ImageryLayerSizeChangedCommand
    // ImageryLayerSizeChangedCommand
    public class ImageryLayerSizeChangedCommand : ImageryLayerCommandBase<ImageryLayerSizeChangedCommandBehavior>
    { }

    public class ImageryLayerSizeChangedCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerSizeChangedCommandBehavior<T>, new()
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

    #region ImageryLayerLayoutUpdatedCommand
    // ImageryLayerLayoutUpdatedCommand
    public class ImageryLayerLayoutUpdatedCommand : ImageryLayerCommandBase<ImageryLayerLayoutUpdatedCommandBehavior>
    { }

    public class ImageryLayerLayoutUpdatedCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerLayoutUpdatedCommandBehavior<T>, new()
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

    #region ImageryLayerBindingValidationErrorCommand
    // ImageryLayerBindingValidationErrorCommand
    public class ImageryLayerBindingValidationErrorCommand : ImageryLayerCommandBase<ImageryLayerBindingValidationErrorCommandBehavior>
    { }

    public class ImageryLayerBindingValidationErrorCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerBindingValidationErrorCommandBehavior<T>, new()
    { }

    public class ImageryLayerBindingValidationErrorCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, ValidationErrorEventArgs>
    {
        public ImageryLayerBindingValidationErrorCommandBehavior(Func<object, ValidationErrorEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerBindingValidationErrorCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.BindingValidationError += OnEventRaised;
        }
    }

    // ImageryLayerBindingValidationErrorCommandBehavior
    public class ImageryLayerBindingValidationErrorCommandBehavior : ImageryLayerBindingValidationErrorCommandBehavior<object>
    { }
    #endregion

#if SILVERLIGHT5

    #region ImageryLayerDataContextChangedCommand
    // ImageryLayerDataContextChangedCommand
    public class ImageryLayerDataContextChangedCommand : ImageryLayerCommandBase<ImageryLayerDataContextChangedCommandBehavior>
    { }

    public class ImageryLayerDataContextChangedCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerDataContextChangedCommandBehavior<T>, new()
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
#endif
    #region ImageryLayerMouseMoveCommand
    // ImageryLayerMouseMoveCommand
    public class ImageryLayerMouseMoveCommand : ImageryLayerCommandBase<ImageryLayerMouseMoveCommandBehavior>
    { }

    public class ImageryLayerMouseMoveCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerMouseMoveCommandBehavior<T>, new()
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

    #region ImageryLayerMouseEnterCommand
    // ImageryLayerMouseEnterCommand
    public class ImageryLayerMouseEnterCommand : ImageryLayerCommandBase<ImageryLayerMouseEnterCommandBehavior>
    { }

    public class ImageryLayerMouseEnterCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerMouseEnterCommandBehavior<T>, new()
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

    public class ImageryLayerMouseLeaveCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerMouseLeaveCommandBehavior<T>, new()
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

    #region ImageryLayerMouseLeftButtonDownCommand
    // ImageryLayerMouseLeftButtonDownCommand
    public class ImageryLayerMouseLeftButtonDownCommand : ImageryLayerCommandBase<ImageryLayerMouseLeftButtonDownCommandBehavior>
    { }

    public class ImageryLayerMouseLeftButtonDownCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerMouseLeftButtonDownCommandBehavior<T>, new()
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

    #region ImageryLayerMouseLeftButtonUpCommand
    // ImageryLayerMouseLeftButtonUpCommand
    public class ImageryLayerMouseLeftButtonUpCommand : ImageryLayerCommandBase<ImageryLayerMouseLeftButtonUpCommandBehavior>
    { }

    public class ImageryLayerMouseLeftButtonUpCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerMouseLeftButtonUpCommandBehavior<T>, new()
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

    #region ImageryLayerMouseRightButtonDownCommand
    // ImageryLayerMouseRightButtonDownCommand
    public class ImageryLayerMouseRightButtonDownCommand : ImageryLayerCommandBase<ImageryLayerMouseRightButtonDownCommandBehavior>
    { }

    public class ImageryLayerMouseRightButtonDownCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerMouseRightButtonDownCommandBehavior<T>, new()
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

    #region ImageryLayerMouseRightButtonUpCommand
    // ImageryLayerMouseRightButtonUpCommand
    public class ImageryLayerMouseRightButtonUpCommand : ImageryLayerCommandBase<ImageryLayerMouseRightButtonUpCommandBehavior>
    { }

    public class ImageryLayerMouseRightButtonUpCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerMouseRightButtonUpCommandBehavior<T>, new()
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

    #region ImageryLayerMouseWheelCommand
    // ImageryLayerMouseWheelCommand
    public class ImageryLayerMouseWheelCommand : ImageryLayerCommandBase<ImageryLayerMouseWheelCommandBehavior>
    { }

    public class ImageryLayerMouseWheelCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerMouseWheelCommandBehavior<T>, new()
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

    #region ImageryLayerKeyUpCommand
    // ImageryLayerKeyUpCommand
    public class ImageryLayerKeyUpCommand : ImageryLayerCommandBase<ImageryLayerKeyUpCommandBehavior>
    { }

    public class ImageryLayerKeyUpCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerKeyUpCommandBehavior<T>, new()
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

    #region ImageryLayerKeyDownCommand
    // ImageryLayerKeyDownCommand
    public class ImageryLayerKeyDownCommand : ImageryLayerCommandBase<ImageryLayerKeyDownCommandBehavior>
    { }

    public class ImageryLayerKeyDownCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerKeyDownCommandBehavior<T>, new()
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

#if SILVERLIGHT5

    #region ImageryLayerMediaCommandCommand
    // ImageryLayerMediaCommandCommand
	public class ImageryLayerMediaCommandCommand : ImageryLayerCommandBase<ImageryLayerMediaCommandCommandBehavior>
	{ }

    public class ImageryLayerMediaCommandCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerMediaCommandCommandBehavior<T>,new()
    { }

    public class ImageryLayerMediaCommandCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, MediaCommandEventArgs>
    {
        public ImageryLayerMediaCommandCommandBehavior(Func<object, MediaCommandEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerMediaCommandCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MediaCommand += OnEventRaised;
        }
    }

	// ImageryLayerMediaCommandCommandBehavior
    public class ImageryLayerMediaCommandCommandBehavior : ImageryLayerMediaCommandCommandBehavior<object>
    { }
    #endregion

#endif

    #region ImageryLayerGotFocusCommand
    // ImageryLayerGotFocusCommand
    public class ImageryLayerGotFocusCommand : ImageryLayerCommandBase<ImageryLayerGotFocusCommandBehavior>
    { }

    public class ImageryLayerGotFocusCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerGotFocusCommandBehavior<T>, new()
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

    public class ImageryLayerLostFocusCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerLostFocusCommandBehavior<T>, new()
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

    #region ImageryLayerLostMouseCaptureCommand
    // ImageryLayerLostMouseCaptureCommand
    public class ImageryLayerLostMouseCaptureCommand : ImageryLayerCommandBase<ImageryLayerLostMouseCaptureCommandBehavior>
    { }

    public class ImageryLayerLostMouseCaptureCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerLostMouseCaptureCommandBehavior<T>, new()
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

    #region ImageryLayerTextInputCommand
    // ImageryLayerTextInputCommand
    public class ImageryLayerTextInputCommand : ImageryLayerCommandBase<ImageryLayerTextInputCommandBehavior>
    { }

    public class ImageryLayerTextInputCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerTextInputCommandBehavior<T>, new()
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

    #region ImageryLayerTextInputStartCommand
    // ImageryLayerTextInputStartCommand
    public class ImageryLayerTextInputStartCommand : ImageryLayerCommandBase<ImageryLayerTextInputStartCommandBehavior>
    { }

    public class ImageryLayerTextInputStartCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerTextInputStartCommandBehavior<T>, new()
    { }

    public class ImageryLayerTextInputStartCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, TextCompositionEventArgs>
    {
        public ImageryLayerTextInputStartCommandBehavior(Func<object, TextCompositionEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerTextInputStartCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.TextInputStart += OnEventRaised;
        }
    }

    // ImageryLayerTextInputStartCommandBehavior
    public class ImageryLayerTextInputStartCommandBehavior : ImageryLayerTextInputStartCommandBehavior<object>
    { }
    #endregion

    #region ImageryLayerTextInputUpdateCommand
    // ImageryLayerTextInputUpdateCommand
    public class ImageryLayerTextInputUpdateCommand : ImageryLayerCommandBase<ImageryLayerTextInputUpdateCommandBehavior>
    { }

    public class ImageryLayerTextInputUpdateCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerTextInputUpdateCommandBehavior<T>, new()
    { }

    public class ImageryLayerTextInputUpdateCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, TextCompositionEventArgs>
    {
        public ImageryLayerTextInputUpdateCommandBehavior(Func<object, TextCompositionEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerTextInputUpdateCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.TextInputUpdate += OnEventRaised;
        }
    }

    // ImageryLayerTextInputUpdateCommandBehavior
    public class ImageryLayerTextInputUpdateCommandBehavior : ImageryLayerTextInputUpdateCommandBehavior<object>
    { }
    #endregion

    #region ImageryLayerDragEnterCommand
    // ImageryLayerDragEnterCommand
    public class ImageryLayerDragEnterCommand : ImageryLayerCommandBase<ImageryLayerDragEnterCommandBehavior>
    { }

    public class ImageryLayerDragEnterCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerDragEnterCommandBehavior<T>, new()
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

    #region ImageryLayerDragLeaveCommand
    // ImageryLayerDragLeaveCommand
    public class ImageryLayerDragLeaveCommand : ImageryLayerCommandBase<ImageryLayerDragLeaveCommandBehavior>
    { }

    public class ImageryLayerDragLeaveCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerDragLeaveCommandBehavior<T>, new()
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

    #region ImageryLayerDragOverCommand
    // ImageryLayerDragOverCommand
    public class ImageryLayerDragOverCommand : ImageryLayerCommandBase<ImageryLayerDragOverCommandBehavior>
    { }

    public class ImageryLayerDragOverCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerDragOverCommandBehavior<T>, new()
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

    #region ImageryLayerDropCommand
    // ImageryLayerDropCommand
    public class ImageryLayerDropCommand : ImageryLayerCommandBase<ImageryLayerDropCommandBehavior>
    { }

    public class ImageryLayerDropCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerDropCommandBehavior<T>, new()
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

    #region ImageryLayerManipulationStartedCommand
    // ImageryLayerManipulationStartedCommand
    public class ImageryLayerManipulationStartedCommand : ImageryLayerCommandBase<ImageryLayerManipulationStartedCommandBehavior>
    { }

    public class ImageryLayerManipulationStartedCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerManipulationStartedCommandBehavior<T>, new()
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

    public class ImageryLayerManipulationDeltaCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerManipulationDeltaCommandBehavior<T>, new()
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

    #region ImageryLayerManipulationCompletedCommand
    // ImageryLayerManipulationCompletedCommand
    public class ImageryLayerManipulationCompletedCommand : ImageryLayerCommandBase<ImageryLayerManipulationCompletedCommandBehavior>
    { }

    public class ImageryLayerManipulationCompletedCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerManipulationCompletedCommandBehavior<T>, new()
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

#if SILVERLIGHT5

    #region ImageryLayerTapCommand
    // ImageryLayerTapCommand
    public class ImageryLayerTapCommand : ImageryLayerCommandBase<ImageryLayerTapCommandBehavior>
    { }

    public class ImageryLayerTapCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerTapCommandBehavior<T>, new()
    { }

    public class ImageryLayerTapCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, GestureEventArgs>
    {
        public ImageryLayerTapCommandBehavior(Func<object, GestureEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerTapCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Tap += OnEventRaised;
        }
    }

    // ImageryLayerTapCommandBehavior
    public class ImageryLayerTapCommandBehavior : ImageryLayerTapCommandBehavior<object>
    { }
    #endregion

    #region ImageryLayerDoubleTapCommand
    // ImageryLayerDoubleTapCommand
    public class ImageryLayerDoubleTapCommand : ImageryLayerCommandBase<ImageryLayerDoubleTapCommandBehavior>
    { }

    public class ImageryLayerDoubleTapCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerDoubleTapCommandBehavior<T>, new()
    { }

    public class ImageryLayerDoubleTapCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, GestureEventArgs>
    {
        public ImageryLayerDoubleTapCommandBehavior(Func<object, GestureEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerDoubleTapCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.DoubleTap += OnEventRaised;
        }
    }

    // ImageryLayerDoubleTapCommandBehavior
    public class ImageryLayerDoubleTapCommandBehavior : ImageryLayerDoubleTapCommandBehavior<object>
    { }
    #endregion

    #region ImageryLayerHoldCommand
    // ImageryLayerHoldCommand
    public class ImageryLayerHoldCommand : ImageryLayerCommandBase<ImageryLayerHoldCommandBehavior>
    { }

    public class ImageryLayerHoldCommand<T, TBehavior> : ImageryLayerCommandBase<TBehavior> where TBehavior : ImageryLayerHoldCommandBehavior<T>, new()
    { }

    public class ImageryLayerHoldCommandBehavior<TReturn> : ImageryLayerCommandBehaviorBase<TReturn, GestureEventArgs>
    {
        public ImageryLayerHoldCommandBehavior(Func<object, GestureEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ImageryLayerHoldCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Hold += OnEventRaised;
        }
    }

    // ImageryLayerHoldCommandBehavior
    public class ImageryLayerHoldCommandBehavior : ImageryLayerHoldCommandBehavior<object>
    { }
    #endregion
#endif
}


