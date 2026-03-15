#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Syncfusion.Windows.Chart;
using Syncfusion.Windows.Shared;

namespace Syncfusion.MVVM.Silverlight
{

    #region ChartBaseCommand
    public class ChartCommandBase<TBehavior> : ControlCommandBase<TBehavior, Chart> where TBehavior : CommandBehaviorBase<Chart>, new()
    { }

    public class ChartCommandBehaviorBase<TReturn, TEventArgs> : BuilderCommandBehaviorBase<Chart, TEventArgs, TReturn>
    { }
    #endregion
    #region ChartAreaBaseCommand
    public class ChartAreaCommandBase<TBehavior> : ControlCommandBase<TBehavior, ChartArea> where TBehavior : CommandBehaviorBase<ChartArea>, new()
    { }

    public class ChartAreaCommandBehaviorBase<TReturn, TEventArgs> : BuilderCommandBehaviorBase<ChartArea, TEventArgs, TReturn>
    { }
    #endregion

    #region AreaCommands
    #region ChartAreaIsEnabledChangedCommand
    // ChartAreaIsEnabledChangedCommand
    public class ChartAreaIsEnabledChangedCommand : ChartAreaCommandBase<ChartAreaIsEnabledChangedCommandBehavior>
    { }

    public class ChartAreaIsEnabledChangedCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaIsEnabledChangedCommandBehavior<T>, new()
    { }

    public class ChartAreaIsEnabledChangedCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public ChartAreaIsEnabledChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaIsEnabledChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsEnabledChanged += OnEventRaised;
        }
    }

    // ChartAreaIsEnabledChangedCommandBehavior
    public class ChartAreaIsEnabledChangedCommandBehavior : ChartAreaIsEnabledChangedCommandBehavior<object>
    { }
    #endregion

    #region ChartAreaLoadedCommand
    // ChartAreaLoadedCommand
    public class ChartAreaLoadedCommand : ChartAreaCommandBase<ChartAreaLoadedCommandBehavior>
    { }

    public class ChartAreaLoadedCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaLoadedCommandBehavior<T>, new()
    { }

    public class ChartAreaLoadedCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, RoutedEventArgs>
    {
        public ChartAreaLoadedCommandBehavior(Func<object, RoutedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaLoadedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Loaded += OnEventRaised;
        }
    }

    // ChartAreaLoadedCommandBehavior
    public class ChartAreaLoadedCommandBehavior : ChartAreaLoadedCommandBehavior<object>
    { }
    #endregion

    #region ChartAreaUnloadedCommand
    // ChartAreaUnloadedCommand
    public class ChartAreaUnloadedCommand : ChartAreaCommandBase<ChartAreaUnloadedCommandBehavior>
    { }

    public class ChartAreaUnloadedCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaUnloadedCommandBehavior<T>, new()
    { }

    public class ChartAreaUnloadedCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, RoutedEventArgs>
    {
        public ChartAreaUnloadedCommandBehavior(Func<object, RoutedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaUnloadedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Unloaded += OnEventRaised;
        }
    }

    // ChartAreaUnloadedCommandBehavior
    public class ChartAreaUnloadedCommandBehavior : ChartAreaUnloadedCommandBehavior<object>
    { }
    #endregion

    #region ChartAreaSizeChangedCommand
    // ChartAreaSizeChangedCommand
    public class ChartAreaSizeChangedCommand : ChartAreaCommandBase<ChartAreaSizeChangedCommandBehavior>
    { }

    public class ChartAreaSizeChangedCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaSizeChangedCommandBehavior<T>, new()
    { }

    public class ChartAreaSizeChangedCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, SizeChangedEventArgs>
    {
        public ChartAreaSizeChangedCommandBehavior(Func<object, SizeChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaSizeChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.SizeChanged += OnEventRaised;
        }
    }

    // ChartAreaSizeChangedCommandBehavior
    public class ChartAreaSizeChangedCommandBehavior : ChartAreaSizeChangedCommandBehavior<object>
    { }
    #endregion

    #region ChartAreaLayoutUpdatedCommand
    // ChartAreaLayoutUpdatedCommand
    public class ChartAreaLayoutUpdatedCommand : ChartAreaCommandBase<ChartAreaLayoutUpdatedCommandBehavior>
    { }

    public class ChartAreaLayoutUpdatedCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaLayoutUpdatedCommandBehavior<T>, new()
    { }

    public class ChartAreaLayoutUpdatedCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, EventArgs>
    {
        public ChartAreaLayoutUpdatedCommandBehavior(Func<object, EventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaLayoutUpdatedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.LayoutUpdated += OnEventRaised;
        }
    }

    // ChartAreaLayoutUpdatedCommandBehavior
    public class ChartAreaLayoutUpdatedCommandBehavior : ChartAreaLayoutUpdatedCommandBehavior<object>
    { }
    #endregion

    #region ChartAreaBindingValidationErrorCommand
    // ChartAreaBindingValidationErrorCommand
    public class ChartAreaBindingValidationErrorCommand : ChartAreaCommandBase<ChartAreaBindingValidationErrorCommandBehavior>
    { }

    public class ChartAreaBindingValidationErrorCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaBindingValidationErrorCommandBehavior<T>, new()
    { }

    public class ChartAreaBindingValidationErrorCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, ValidationErrorEventArgs>
    {
        public ChartAreaBindingValidationErrorCommandBehavior(Func<object, ValidationErrorEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaBindingValidationErrorCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.BindingValidationError += OnEventRaised;
        }
    }

    // ChartAreaBindingValidationErrorCommandBehavior
    public class ChartAreaBindingValidationErrorCommandBehavior : ChartAreaBindingValidationErrorCommandBehavior<object>
    { }
    #endregion
#if SILVERLIGHT5
    #region ChartAreaDataContextChangedCommand
    // ChartAreaDataContextChangedCommand
    public class ChartAreaDataContextChangedCommand : ChartAreaCommandBase<ChartAreaDataContextChangedCommandBehavior>
    { }

    public class ChartAreaDataContextChangedCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaDataContextChangedCommandBehavior<T>, new()
    { }

    public class ChartAreaDataContextChangedCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public ChartAreaDataContextChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaDataContextChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.DataContextChanged += OnEventRaised;
        }
    }

    // ChartAreaDataContextChangedCommandBehavior
    public class ChartAreaDataContextChangedCommandBehavior : ChartAreaDataContextChangedCommandBehavior<object>
    { }
    #endregion
#endif
    #region ChartAreaMouseMoveCommand
    // ChartAreaMouseMoveCommand
    public class ChartAreaMouseMoveCommand : ChartAreaCommandBase<ChartAreaMouseMoveCommandBehavior>
    { }

    public class ChartAreaMouseMoveCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaMouseMoveCommandBehavior<T>, new()
    { }

    public class ChartAreaMouseMoveCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, MouseEventArgs>
    {
        public ChartAreaMouseMoveCommandBehavior(Func<object, MouseEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaMouseMoveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseMove += OnEventRaised;
        }
    }

    // ChartAreaMouseMoveCommandBehavior
    public class ChartAreaMouseMoveCommandBehavior : ChartAreaMouseMoveCommandBehavior<object>
    { }
    #endregion

    #region ChartAreaMouseEnterCommand
    // ChartAreaMouseEnterCommand
    public class ChartAreaMouseEnterCommand : ChartAreaCommandBase<ChartAreaMouseEnterCommandBehavior>
    { }

    public class ChartAreaMouseEnterCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaMouseEnterCommandBehavior<T>, new()
    { }

    public class ChartAreaMouseEnterCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, MouseEventArgs>
    {
        public ChartAreaMouseEnterCommandBehavior(Func<object, MouseEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaMouseEnterCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseEnter += OnEventRaised;
        }
    }

    // ChartAreaMouseEnterCommandBehavior
    public class ChartAreaMouseEnterCommandBehavior : ChartAreaMouseEnterCommandBehavior<object>
    { }
    #endregion

    #region ChartAreaMouseLeaveCommand
    // ChartAreaMouseLeaveCommand
    public class ChartAreaMouseLeaveCommand : ChartAreaCommandBase<ChartAreaMouseLeaveCommandBehavior>
    { }

    public class ChartAreaMouseLeaveCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaMouseLeaveCommandBehavior<T>, new()
    { }

    public class ChartAreaMouseLeaveCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, MouseEventArgs>
    {
        public ChartAreaMouseLeaveCommandBehavior(Func<object, MouseEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaMouseLeaveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseLeave += OnEventRaised;
        }
    }

    // ChartAreaMouseLeaveCommandBehavior
    public class ChartAreaMouseLeaveCommandBehavior : ChartAreaMouseLeaveCommandBehavior<object>
    { }
    #endregion

    #region ChartAreaMouseLeftButtonDownCommand
    // ChartAreaMouseLeftButtonDownCommand
    public class ChartAreaMouseLeftButtonDownCommand : ChartAreaCommandBase<ChartAreaMouseLeftButtonDownCommandBehavior>
    { }

    public class ChartAreaMouseLeftButtonDownCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaMouseLeftButtonDownCommandBehavior<T>, new()
    { }

    public class ChartAreaMouseLeftButtonDownCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public ChartAreaMouseLeftButtonDownCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaMouseLeftButtonDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseLeftButtonDown += OnEventRaised;
        }
    }

    // ChartAreaMouseLeftButtonDownCommandBehavior
    public class ChartAreaMouseLeftButtonDownCommandBehavior : ChartAreaMouseLeftButtonDownCommandBehavior<object>
    { }
    #endregion

    #region ChartAreaMouseLeftButtonUpCommand
    // ChartAreaMouseLeftButtonUpCommand
    public class ChartAreaMouseLeftButtonUpCommand : ChartAreaCommandBase<ChartAreaMouseLeftButtonUpCommandBehavior>
    { }

    public class ChartAreaMouseLeftButtonUpCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaMouseLeftButtonUpCommandBehavior<T>, new()
    { }

    public class ChartAreaMouseLeftButtonUpCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public ChartAreaMouseLeftButtonUpCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaMouseLeftButtonUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseLeftButtonUp += OnEventRaised;
        }
    }

    // ChartAreaMouseLeftButtonUpCommandBehavior
    public class ChartAreaMouseLeftButtonUpCommandBehavior : ChartAreaMouseLeftButtonUpCommandBehavior<object>
    { }
    #endregion

    #region ChartAreaMouseRightButtonDownCommand
    // ChartAreaMouseRightButtonDownCommand
    public class ChartAreaMouseRightButtonDownCommand : ChartAreaCommandBase<ChartAreaMouseRightButtonDownCommandBehavior>
    { }

    public class ChartAreaMouseRightButtonDownCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaMouseRightButtonDownCommandBehavior<T>, new()
    { }

    public class ChartAreaMouseRightButtonDownCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public ChartAreaMouseRightButtonDownCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaMouseRightButtonDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseRightButtonDown += OnEventRaised;
        }
    }

    // ChartAreaMouseRightButtonDownCommandBehavior
    public class ChartAreaMouseRightButtonDownCommandBehavior : ChartAreaMouseRightButtonDownCommandBehavior<object>
    { }
    #endregion

    #region ChartAreaMouseRightButtonUpCommand
    // ChartAreaMouseRightButtonUpCommand
    public class ChartAreaMouseRightButtonUpCommand : ChartAreaCommandBase<ChartAreaMouseRightButtonUpCommandBehavior>
    { }

    public class ChartAreaMouseRightButtonUpCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaMouseRightButtonUpCommandBehavior<T>, new()
    { }

    public class ChartAreaMouseRightButtonUpCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public ChartAreaMouseRightButtonUpCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaMouseRightButtonUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseRightButtonUp += OnEventRaised;
        }
    }

    // ChartAreaMouseRightButtonUpCommandBehavior
    public class ChartAreaMouseRightButtonUpCommandBehavior : ChartAreaMouseRightButtonUpCommandBehavior<object>
    { }
    #endregion

    #region ChartAreaMouseWheelCommand
    // ChartAreaMouseWheelCommand
    public class ChartAreaMouseWheelCommand : ChartAreaCommandBase<ChartAreaMouseWheelCommandBehavior>
    { }

    public class ChartAreaMouseWheelCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaMouseWheelCommandBehavior<T>, new()
    { }

    public class ChartAreaMouseWheelCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, MouseWheelEventArgs>
    {
        public ChartAreaMouseWheelCommandBehavior(Func<object, MouseWheelEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaMouseWheelCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseWheel += OnEventRaised;
        }
    }

    // ChartAreaMouseWheelCommandBehavior
    public class ChartAreaMouseWheelCommandBehavior : ChartAreaMouseWheelCommandBehavior<object>
    { }
    #endregion

    #region ChartAreaKeyUpCommand
    // ChartAreaKeyUpCommand
    public class ChartAreaKeyUpCommand : ChartAreaCommandBase<ChartAreaKeyUpCommandBehavior>
    { }

    public class ChartAreaKeyUpCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaKeyUpCommandBehavior<T>, new()
    { }

    public class ChartAreaKeyUpCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, KeyEventArgs>
    {
        public ChartAreaKeyUpCommandBehavior(Func<object, KeyEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaKeyUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.KeyUp += OnEventRaised;
        }
    }

    // ChartAreaKeyUpCommandBehavior
    public class ChartAreaKeyUpCommandBehavior : ChartAreaKeyUpCommandBehavior<object>
    { }
    #endregion

    #region ChartAreaKeyDownCommand
    // ChartAreaKeyDownCommand
    public class ChartAreaKeyDownCommand : ChartAreaCommandBase<ChartAreaKeyDownCommandBehavior>
    { }

    public class ChartAreaKeyDownCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaKeyDownCommandBehavior<T>, new()
    { }

    public class ChartAreaKeyDownCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, KeyEventArgs>
    {
        public ChartAreaKeyDownCommandBehavior(Func<object, KeyEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaKeyDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.KeyDown += OnEventRaised;
        }
    }

    // ChartAreaKeyDownCommandBehavior
    public class ChartAreaKeyDownCommandBehavior : ChartAreaKeyDownCommandBehavior<object>
    { }
    #endregion
#if SILVERLIGHT5
    #region ChartAreaMediaCommandCommand
    // ChartAreaMediaCommandCommand
    public class ChartAreaMediaCommandCommand : ChartAreaCommandBase<ChartAreaMediaCommandCommandBehavior>
    { }

    public class ChartAreaMediaCommandCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaMediaCommandCommandBehavior<T>, new()
    { }

    public class ChartAreaMediaCommandCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, MediaCommandEventArgs>
    {
        public ChartAreaMediaCommandCommandBehavior(Func<object, MediaCommandEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaMediaCommandCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MediaCommand += OnEventRaised;
        }
    }

    // ChartAreaMediaCommandCommandBehavior
    public class ChartAreaMediaCommandCommandBehavior : ChartAreaMediaCommandCommandBehavior<object>
    { }
    #endregion
#endif
    #region ChartAreaGotFocusCommand
    // ChartAreaGotFocusCommand
    public class ChartAreaGotFocusCommand : ChartAreaCommandBase<ChartAreaGotFocusCommandBehavior>
    { }

    public class ChartAreaGotFocusCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaGotFocusCommandBehavior<T>, new()
    { }

    public class ChartAreaGotFocusCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, RoutedEventArgs>
    {
        public ChartAreaGotFocusCommandBehavior(Func<object, RoutedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaGotFocusCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.GotFocus += OnEventRaised;
        }
    }

    // ChartAreaGotFocusCommandBehavior
    public class ChartAreaGotFocusCommandBehavior : ChartAreaGotFocusCommandBehavior<object>
    { }
    #endregion

    #region ChartAreaLostFocusCommand
    // ChartAreaLostFocusCommand
    public class ChartAreaLostFocusCommand : ChartAreaCommandBase<ChartAreaLostFocusCommandBehavior>
    { }

    public class ChartAreaLostFocusCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaLostFocusCommandBehavior<T>, new()
    { }

    public class ChartAreaLostFocusCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, RoutedEventArgs>
    {
        public ChartAreaLostFocusCommandBehavior(Func<object, RoutedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaLostFocusCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.LostFocus += OnEventRaised;
        }
    }

    // ChartAreaLostFocusCommandBehavior
    public class ChartAreaLostFocusCommandBehavior : ChartAreaLostFocusCommandBehavior<object>
    { }
    #endregion

    #region ChartAreaLostMouseCaptureCommand
    // ChartAreaLostMouseCaptureCommand
    public class ChartAreaLostMouseCaptureCommand : ChartAreaCommandBase<ChartAreaLostMouseCaptureCommandBehavior>
    { }

    public class ChartAreaLostMouseCaptureCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaLostMouseCaptureCommandBehavior<T>, new()
    { }

    public class ChartAreaLostMouseCaptureCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, MouseEventArgs>
    {
        public ChartAreaLostMouseCaptureCommandBehavior(Func<object, MouseEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaLostMouseCaptureCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.LostMouseCapture += OnEventRaised;
        }
    }

    // ChartAreaLostMouseCaptureCommandBehavior
    public class ChartAreaLostMouseCaptureCommandBehavior : ChartAreaLostMouseCaptureCommandBehavior<object>
    { }
    #endregion

    #region ChartAreaTextInputCommand
    // ChartAreaTextInputCommand
    public class ChartAreaTextInputCommand : ChartAreaCommandBase<ChartAreaTextInputCommandBehavior>
    { }

    public class ChartAreaTextInputCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaTextInputCommandBehavior<T>, new()
    { }

    public class ChartAreaTextInputCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, TextCompositionEventArgs>
    {
        public ChartAreaTextInputCommandBehavior(Func<object, TextCompositionEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaTextInputCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.TextInput += OnEventRaised;
        }
    }

    // ChartAreaTextInputCommandBehavior
    public class ChartAreaTextInputCommandBehavior : ChartAreaTextInputCommandBehavior<object>
    { }
    #endregion

    #region ChartAreaTextInputStartCommand
    // ChartAreaTextInputStartCommand
    public class ChartAreaTextInputStartCommand : ChartAreaCommandBase<ChartAreaTextInputStartCommandBehavior>
    { }

    public class ChartAreaTextInputStartCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaTextInputStartCommandBehavior<T>, new()
    { }

    public class ChartAreaTextInputStartCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, TextCompositionEventArgs>
    {
        public ChartAreaTextInputStartCommandBehavior(Func<object, TextCompositionEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaTextInputStartCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.TextInputStart += OnEventRaised;
        }
    }

    // ChartAreaTextInputStartCommandBehavior
    public class ChartAreaTextInputStartCommandBehavior : ChartAreaTextInputStartCommandBehavior<object>
    { }
    #endregion

    #region ChartAreaTextInputUpdateCommand
    // ChartAreaTextInputUpdateCommand
    public class ChartAreaTextInputUpdateCommand : ChartAreaCommandBase<ChartAreaTextInputUpdateCommandBehavior>
    { }

    public class ChartAreaTextInputUpdateCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaTextInputUpdateCommandBehavior<T>, new()
    { }

    public class ChartAreaTextInputUpdateCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, TextCompositionEventArgs>
    {
        public ChartAreaTextInputUpdateCommandBehavior(Func<object, TextCompositionEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaTextInputUpdateCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.TextInputUpdate += OnEventRaised;
        }
    }

    // ChartAreaTextInputUpdateCommandBehavior
    public class ChartAreaTextInputUpdateCommandBehavior : ChartAreaTextInputUpdateCommandBehavior<object>
    { }
    #endregion

    #region ChartAreaDragEnterCommand
    // ChartAreaDragEnterCommand
    public class ChartAreaDragEnterCommand : ChartAreaCommandBase<ChartAreaDragEnterCommandBehavior>
    { }

    public class ChartAreaDragEnterCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaDragEnterCommandBehavior<T>, new()
    { }

    public class ChartAreaDragEnterCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, DragEventArgs>
    {
        public ChartAreaDragEnterCommandBehavior(Func<object, DragEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaDragEnterCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.DragEnter += OnEventRaised;
        }
    }

    // ChartAreaDragEnterCommandBehavior
    public class ChartAreaDragEnterCommandBehavior : ChartAreaDragEnterCommandBehavior<object>
    { }
    #endregion

    #region ChartAreaDragLeaveCommand
    // ChartAreaDragLeaveCommand
    public class ChartAreaDragLeaveCommand : ChartAreaCommandBase<ChartAreaDragLeaveCommandBehavior>
    { }

    public class ChartAreaDragLeaveCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaDragLeaveCommandBehavior<T>, new()
    { }

    public class ChartAreaDragLeaveCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, DragEventArgs>
    {
        public ChartAreaDragLeaveCommandBehavior(Func<object, DragEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaDragLeaveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.DragLeave += OnEventRaised;
        }
    }

    // ChartAreaDragLeaveCommandBehavior
    public class ChartAreaDragLeaveCommandBehavior : ChartAreaDragLeaveCommandBehavior<object>
    { }
    #endregion

    #region ChartAreaDragOverCommand
    // ChartAreaDragOverCommand
    public class ChartAreaDragOverCommand : ChartAreaCommandBase<ChartAreaDragOverCommandBehavior>
    { }

    public class ChartAreaDragOverCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaDragOverCommandBehavior<T>, new()
    { }

    public class ChartAreaDragOverCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, DragEventArgs>
    {
        public ChartAreaDragOverCommandBehavior(Func<object, DragEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaDragOverCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.DragOver += OnEventRaised;
        }
    }

    // ChartAreaDragOverCommandBehavior
    public class ChartAreaDragOverCommandBehavior : ChartAreaDragOverCommandBehavior<object>
    { }
    #endregion

    #region ChartAreaDropCommand
    // ChartAreaDropCommand
    public class ChartAreaDropCommand : ChartAreaCommandBase<ChartAreaDropCommandBehavior>
    { }

    public class ChartAreaDropCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaDropCommandBehavior<T>, new()
    { }

    public class ChartAreaDropCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, DragEventArgs>
    {
        public ChartAreaDropCommandBehavior(Func<object, DragEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaDropCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Drop += OnEventRaised;
        }
    }

    // ChartAreaDropCommandBehavior
    public class ChartAreaDropCommandBehavior : ChartAreaDropCommandBehavior<object>
    { }
    #endregion

    #region ChartAreaManipulationStartedCommand
    // ChartAreaManipulationStartedCommand
    public class ChartAreaManipulationStartedCommand : ChartAreaCommandBase<ChartAreaManipulationStartedCommandBehavior>
    { }

    public class ChartAreaManipulationStartedCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaManipulationStartedCommandBehavior<T>, new()
    { }

    public class ChartAreaManipulationStartedCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, ManipulationStartedEventArgs>
    {
        public ChartAreaManipulationStartedCommandBehavior(Func<object, ManipulationStartedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaManipulationStartedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ManipulationStarted += OnEventRaised;
        }
    }

    // ChartAreaManipulationStartedCommandBehavior
    public class ChartAreaManipulationStartedCommandBehavior : ChartAreaManipulationStartedCommandBehavior<object>
    { }
    #endregion

    #region ChartAreaManipulationDeltaCommand
    // ChartAreaManipulationDeltaCommand
    public class ChartAreaManipulationDeltaCommand : ChartAreaCommandBase<ChartAreaManipulationDeltaCommandBehavior>
    { }

    public class ChartAreaManipulationDeltaCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaManipulationDeltaCommandBehavior<T>, new()
    { }

    public class ChartAreaManipulationDeltaCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, ManipulationDeltaEventArgs>
    {
        public ChartAreaManipulationDeltaCommandBehavior(Func<object, ManipulationDeltaEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaManipulationDeltaCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ManipulationDelta += OnEventRaised;
        }
    }

    // ChartAreaManipulationDeltaCommandBehavior
    public class ChartAreaManipulationDeltaCommandBehavior : ChartAreaManipulationDeltaCommandBehavior<object>
    { }
    #endregion

    #region ChartAreaManipulationCompletedCommand
    // ChartAreaManipulationCompletedCommand
    public class ChartAreaManipulationCompletedCommand : ChartAreaCommandBase<ChartAreaManipulationCompletedCommandBehavior>
    { }

    public class ChartAreaManipulationCompletedCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaManipulationCompletedCommandBehavior<T>, new()
    { }

    public class ChartAreaManipulationCompletedCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, ManipulationCompletedEventArgs>
    {
        public ChartAreaManipulationCompletedCommandBehavior(Func<object, ManipulationCompletedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaManipulationCompletedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ManipulationCompleted += OnEventRaised;
        }
    }

    // ChartAreaManipulationCompletedCommandBehavior
    public class ChartAreaManipulationCompletedCommandBehavior : ChartAreaManipulationCompletedCommandBehavior<object>
    { }
    #endregion

#if SILVERLIGHT5

    #region ChartAreaTapCommand
    // ChartAreaTapCommand
    public class ChartAreaTapCommand : ChartAreaCommandBase<ChartAreaTapCommandBehavior>
    { }

    public class ChartAreaTapCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaTapCommandBehavior<T>, new()
    { }

    public class ChartAreaTapCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, GestureEventArgs>
    {
        public ChartAreaTapCommandBehavior(Func<object, GestureEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaTapCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Tap += OnEventRaised;
        }
    }

    // ChartAreaTapCommandBehavior
    public class ChartAreaTapCommandBehavior : ChartAreaTapCommandBehavior<object>
    { }
    #endregion

    #region ChartAreaDoubleTapCommand
    // ChartAreaDoubleTapCommand
    public class ChartAreaDoubleTapCommand : ChartAreaCommandBase<ChartAreaDoubleTapCommandBehavior>
    { }

    public class ChartAreaDoubleTapCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaDoubleTapCommandBehavior<T>, new()
    { }

    public class ChartAreaDoubleTapCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, GestureEventArgs>
    {
        public ChartAreaDoubleTapCommandBehavior(Func<object, GestureEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaDoubleTapCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.DoubleTap += OnEventRaised;
        }
    }

    // ChartAreaDoubleTapCommandBehavior
    public class ChartAreaDoubleTapCommandBehavior : ChartAreaDoubleTapCommandBehavior<object>
    { }
    #endregion

    #region ChartAreaHoldCommand
    // ChartAreaHoldCommand
    public class ChartAreaHoldCommand : ChartAreaCommandBase<ChartAreaHoldCommandBehavior>
    { }

    public class ChartAreaHoldCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaHoldCommandBehavior<T>, new()
    { }

    public class ChartAreaHoldCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, GestureEventArgs>
    {
        public ChartAreaHoldCommandBehavior(Func<object, GestureEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaHoldCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Hold += OnEventRaised;
        }
    }

    // ChartAreaHoldCommandBehavior
    public class ChartAreaHoldCommandBehavior : ChartAreaHoldCommandBehavior<object>
    { }
    #endregion
#endif
    #endregion

    #region ChartCommands
    #region ChartIsEnabledChangedCommand
    // ChartIsEnabledChangedCommand
    public class ChartIsEnabledChangedCommand : ChartCommandBase<ChartIsEnabledChangedCommandBehavior>
    { }

    public class ChartIsEnabledChangedCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartIsEnabledChangedCommandBehavior<T>, new()
    { }

    public class ChartIsEnabledChangedCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public ChartIsEnabledChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartIsEnabledChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsEnabledChanged += OnEventRaised;
        }
    }

    // ChartIsEnabledChangedCommandBehavior
    public class ChartIsEnabledChangedCommandBehavior : ChartIsEnabledChangedCommandBehavior<object>
    { }
    #endregion

    #region ChartLoadedCommand
    // ChartLoadedCommand
    public class ChartLoadedCommand : ChartCommandBase<ChartLoadedCommandBehavior>
    { }

    public class ChartLoadedCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartLoadedCommandBehavior<T>, new()
    { }

    public class ChartLoadedCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, RoutedEventArgs>
    {
        public ChartLoadedCommandBehavior(Func<object, RoutedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartLoadedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Loaded += OnEventRaised;
        }
    }

    // ChartLoadedCommandBehavior
    public class ChartLoadedCommandBehavior : ChartLoadedCommandBehavior<object>
    { }
    #endregion

    #region ChartUnloadedCommand
    // ChartUnloadedCommand
    public class ChartUnloadedCommand : ChartCommandBase<ChartUnloadedCommandBehavior>
    { }

    public class ChartUnloadedCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartUnloadedCommandBehavior<T>, new()
    { }

    public class ChartUnloadedCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, RoutedEventArgs>
    {
        public ChartUnloadedCommandBehavior(Func<object, RoutedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartUnloadedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Unloaded += OnEventRaised;
        }
    }

    // ChartUnloadedCommandBehavior
    public class ChartUnloadedCommandBehavior : ChartUnloadedCommandBehavior<object>
    { }
    #endregion

    #region ChartSizeChangedCommand
    // ChartSizeChangedCommand
    public class ChartSizeChangedCommand : ChartCommandBase<ChartSizeChangedCommandBehavior>
    { }

    public class ChartSizeChangedCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartSizeChangedCommandBehavior<T>, new()
    { }

    public class ChartSizeChangedCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, SizeChangedEventArgs>
    {
        public ChartSizeChangedCommandBehavior(Func<object, SizeChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartSizeChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.SizeChanged += OnEventRaised;
        }
    }

    // ChartSizeChangedCommandBehavior
    public class ChartSizeChangedCommandBehavior : ChartSizeChangedCommandBehavior<object>
    { }
    #endregion

    #region ChartLayoutUpdatedCommand
    // ChartLayoutUpdatedCommand
    public class ChartLayoutUpdatedCommand : ChartCommandBase<ChartLayoutUpdatedCommandBehavior>
    { }

    public class ChartLayoutUpdatedCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartLayoutUpdatedCommandBehavior<T>, new()
    { }

    public class ChartLayoutUpdatedCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, EventArgs>
    {
        public ChartLayoutUpdatedCommandBehavior(Func<object, EventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartLayoutUpdatedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.LayoutUpdated += OnEventRaised;
        }
    }

    // ChartLayoutUpdatedCommandBehavior
    public class ChartLayoutUpdatedCommandBehavior : ChartLayoutUpdatedCommandBehavior<object>
    { }
    #endregion

    #region ChartBindingValidationErrorCommand
    // ChartBindingValidationErrorCommand
    public class ChartBindingValidationErrorCommand : ChartCommandBase<ChartBindingValidationErrorCommandBehavior>
    { }

    public class ChartBindingValidationErrorCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartBindingValidationErrorCommandBehavior<T>, new()
    { }

    public class ChartBindingValidationErrorCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, ValidationErrorEventArgs>
    {
        public ChartBindingValidationErrorCommandBehavior(Func<object, ValidationErrorEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartBindingValidationErrorCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.BindingValidationError += OnEventRaised;
        }
    }

    // ChartBindingValidationErrorCommandBehavior
    public class ChartBindingValidationErrorCommandBehavior : ChartBindingValidationErrorCommandBehavior<object>
    { }
    #endregion
#if SILVERLIGHT5
    #region ChartDataContextChangedCommand
    // ChartDataContextChangedCommand
    public class ChartDataContextChangedCommand : ChartCommandBase<ChartDataContextChangedCommandBehavior>
    { }

    public class ChartDataContextChangedCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartDataContextChangedCommandBehavior<T>, new()
    { }

    public class ChartDataContextChangedCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public ChartDataContextChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartDataContextChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.DataContextChanged += OnEventRaised;
        }
    }

    // ChartDataContextChangedCommandBehavior
    public class ChartDataContextChangedCommandBehavior : ChartDataContextChangedCommandBehavior<object>
    { }
    #endregion
#endif
    #region ChartMouseMoveCommand
    // ChartMouseMoveCommand
    public class ChartMouseMoveCommand : ChartCommandBase<ChartMouseMoveCommandBehavior>
    { }

    public class ChartMouseMoveCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartMouseMoveCommandBehavior<T>, new()
    { }

    public class ChartMouseMoveCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, MouseEventArgs>
    {
        public ChartMouseMoveCommandBehavior(Func<object, MouseEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartMouseMoveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseMove += OnEventRaised;
        }
    }

    // ChartMouseMoveCommandBehavior
    public class ChartMouseMoveCommandBehavior : ChartMouseMoveCommandBehavior<object>
    { }
    #endregion

    #region ChartMouseEnterCommand
    // ChartMouseEnterCommand
    public class ChartMouseEnterCommand : ChartCommandBase<ChartMouseEnterCommandBehavior>
    { }

    public class ChartMouseEnterCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartMouseEnterCommandBehavior<T>, new()
    { }

    public class ChartMouseEnterCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, MouseEventArgs>
    {
        public ChartMouseEnterCommandBehavior(Func<object, MouseEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartMouseEnterCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseEnter += OnEventRaised;
        }
    }

    // ChartMouseEnterCommandBehavior
    public class ChartMouseEnterCommandBehavior : ChartMouseEnterCommandBehavior<object>
    { }
    #endregion

    #region ChartMouseLeaveCommand
    // ChartMouseLeaveCommand
    public class ChartMouseLeaveCommand : ChartCommandBase<ChartMouseLeaveCommandBehavior>
    { }

    public class ChartMouseLeaveCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartMouseLeaveCommandBehavior<T>, new()
    { }

    public class ChartMouseLeaveCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, MouseEventArgs>
    {
        public ChartMouseLeaveCommandBehavior(Func<object, MouseEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartMouseLeaveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseLeave += OnEventRaised;
        }
    }

    // ChartMouseLeaveCommandBehavior
    public class ChartMouseLeaveCommandBehavior : ChartMouseLeaveCommandBehavior<object>
    { }
    #endregion

    #region ChartMouseLeftButtonDownCommand
    // ChartMouseLeftButtonDownCommand
    public class ChartMouseLeftButtonDownCommand : ChartCommandBase<ChartMouseLeftButtonDownCommandBehavior>
    { }

    public class ChartMouseLeftButtonDownCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartMouseLeftButtonDownCommandBehavior<T>, new()
    { }

    public class ChartMouseLeftButtonDownCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public ChartMouseLeftButtonDownCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartMouseLeftButtonDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseLeftButtonDown += OnEventRaised;
        }
    }

    // ChartMouseLeftButtonDownCommandBehavior
    public class ChartMouseLeftButtonDownCommandBehavior : ChartMouseLeftButtonDownCommandBehavior<object>
    { }
    #endregion

    #region ChartMouseLeftButtonUpCommand
    // ChartMouseLeftButtonUpCommand
    public class ChartMouseLeftButtonUpCommand : ChartCommandBase<ChartMouseLeftButtonUpCommandBehavior>
    { }

    public class ChartMouseLeftButtonUpCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartMouseLeftButtonUpCommandBehavior<T>, new()
    { }

    public class ChartMouseLeftButtonUpCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public ChartMouseLeftButtonUpCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartMouseLeftButtonUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseLeftButtonUp += OnEventRaised;
        }
    }

    // ChartMouseLeftButtonUpCommandBehavior
    public class ChartMouseLeftButtonUpCommandBehavior : ChartMouseLeftButtonUpCommandBehavior<object>
    { }
    #endregion

    #region ChartMouseRightButtonDownCommand
    // ChartMouseRightButtonDownCommand
    public class ChartMouseRightButtonDownCommand : ChartCommandBase<ChartMouseRightButtonDownCommandBehavior>
    { }

    public class ChartMouseRightButtonDownCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartMouseRightButtonDownCommandBehavior<T>, new()
    { }

    public class ChartMouseRightButtonDownCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public ChartMouseRightButtonDownCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartMouseRightButtonDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseRightButtonDown += OnEventRaised;
        }
    }

    // ChartMouseRightButtonDownCommandBehavior
    public class ChartMouseRightButtonDownCommandBehavior : ChartMouseRightButtonDownCommandBehavior<object>
    { }
    #endregion

    #region ChartMouseRightButtonUpCommand
    // ChartMouseRightButtonUpCommand
    public class ChartMouseRightButtonUpCommand : ChartCommandBase<ChartMouseRightButtonUpCommandBehavior>
    { }

    public class ChartMouseRightButtonUpCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartMouseRightButtonUpCommandBehavior<T>, new()
    { }

    public class ChartMouseRightButtonUpCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public ChartMouseRightButtonUpCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartMouseRightButtonUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseRightButtonUp += OnEventRaised;
        }
    }

    // ChartMouseRightButtonUpCommandBehavior
    public class ChartMouseRightButtonUpCommandBehavior : ChartMouseRightButtonUpCommandBehavior<object>
    { }
    #endregion

    #region ChartMouseWheelCommand
    // ChartMouseWheelCommand
    public class ChartMouseWheelCommand : ChartCommandBase<ChartMouseWheelCommandBehavior>
    { }

    public class ChartMouseWheelCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartMouseWheelCommandBehavior<T>, new()
    { }

    public class ChartMouseWheelCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, MouseWheelEventArgs>
    {
        public ChartMouseWheelCommandBehavior(Func<object, MouseWheelEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartMouseWheelCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseWheel += OnEventRaised;
        }
    }

    // ChartMouseWheelCommandBehavior
    public class ChartMouseWheelCommandBehavior : ChartMouseWheelCommandBehavior<object>
    { }
    #endregion

    #region ChartKeyUpCommand
    // ChartKeyUpCommand
    public class ChartKeyUpCommand : ChartCommandBase<ChartKeyUpCommandBehavior>
    { }

    public class ChartKeyUpCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartKeyUpCommandBehavior<T>, new()
    { }

    public class ChartKeyUpCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, KeyEventArgs>
    {
        public ChartKeyUpCommandBehavior(Func<object, KeyEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartKeyUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.KeyUp += OnEventRaised;
        }
    }

    // ChartKeyUpCommandBehavior
    public class ChartKeyUpCommandBehavior : ChartKeyUpCommandBehavior<object>
    { }
    #endregion

    #region ChartKeyDownCommand
    // ChartKeyDownCommand
    public class ChartKeyDownCommand : ChartCommandBase<ChartKeyDownCommandBehavior>
    { }

    public class ChartKeyDownCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartKeyDownCommandBehavior<T>, new()
    { }

    public class ChartKeyDownCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, KeyEventArgs>
    {
        public ChartKeyDownCommandBehavior(Func<object, KeyEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartKeyDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.KeyDown += OnEventRaised;
        }
    }

    // ChartKeyDownCommandBehavior
    public class ChartKeyDownCommandBehavior : ChartKeyDownCommandBehavior<object>
    { }
    #endregion
#if SILVERLIGHT5
    #region ChartMediaCommandCommand
    // ChartMediaCommandCommand
    public class ChartMediaCommandCommand : ChartCommandBase<ChartMediaCommandCommandBehavior>
    { }

    public class ChartMediaCommandCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartMediaCommandCommandBehavior<T>, new()
    { }

    public class ChartMediaCommandCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, MediaCommandEventArgs>
    {
        public ChartMediaCommandCommandBehavior(Func<object, MediaCommandEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartMediaCommandCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MediaCommand += OnEventRaised;
        }
    }

    // ChartMediaCommandCommandBehavior
    public class ChartMediaCommandCommandBehavior : ChartMediaCommandCommandBehavior<object>
    { }
    #endregion
#endif
    #region ChartGotFocusCommand
    // ChartGotFocusCommand
    public class ChartGotFocusCommand : ChartCommandBase<ChartGotFocusCommandBehavior>
    { }

    public class ChartGotFocusCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartGotFocusCommandBehavior<T>, new()
    { }

    public class ChartGotFocusCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, RoutedEventArgs>
    {
        public ChartGotFocusCommandBehavior(Func<object, RoutedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartGotFocusCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.GotFocus += OnEventRaised;
        }
    }

    // ChartGotFocusCommandBehavior
    public class ChartGotFocusCommandBehavior : ChartGotFocusCommandBehavior<object>
    { }
    #endregion

    #region ChartLostFocusCommand
    // ChartLostFocusCommand
    public class ChartLostFocusCommand : ChartCommandBase<ChartLostFocusCommandBehavior>
    { }

    public class ChartLostFocusCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartLostFocusCommandBehavior<T>, new()
    { }

    public class ChartLostFocusCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, RoutedEventArgs>
    {
        public ChartLostFocusCommandBehavior(Func<object, RoutedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartLostFocusCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.LostFocus += OnEventRaised;
        }
    }

    // ChartLostFocusCommandBehavior
    public class ChartLostFocusCommandBehavior : ChartLostFocusCommandBehavior<object>
    { }
    #endregion

    #region ChartLostMouseCaptureCommand
    // ChartLostMouseCaptureCommand
    public class ChartLostMouseCaptureCommand : ChartCommandBase<ChartLostMouseCaptureCommandBehavior>
    { }

    public class ChartLostMouseCaptureCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartLostMouseCaptureCommandBehavior<T>, new()
    { }

    public class ChartLostMouseCaptureCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, MouseEventArgs>
    {
        public ChartLostMouseCaptureCommandBehavior(Func<object, MouseEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartLostMouseCaptureCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.LostMouseCapture += OnEventRaised;
        }
    }

    // ChartLostMouseCaptureCommandBehavior
    public class ChartLostMouseCaptureCommandBehavior : ChartLostMouseCaptureCommandBehavior<object>
    { }
    #endregion

    #region ChartTextInputCommand
    // ChartTextInputCommand
    public class ChartTextInputCommand : ChartCommandBase<ChartTextInputCommandBehavior>
    { }

    public class ChartTextInputCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartTextInputCommandBehavior<T>, new()
    { }

    public class ChartTextInputCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, TextCompositionEventArgs>
    {
        public ChartTextInputCommandBehavior(Func<object, TextCompositionEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartTextInputCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.TextInput += OnEventRaised;
        }
    }

    // ChartTextInputCommandBehavior
    public class ChartTextInputCommandBehavior : ChartTextInputCommandBehavior<object>
    { }
    #endregion

    #region ChartTextInputStartCommand
    // ChartTextInputStartCommand
    public class ChartTextInputStartCommand : ChartCommandBase<ChartTextInputStartCommandBehavior>
    { }

    public class ChartTextInputStartCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartTextInputStartCommandBehavior<T>, new()
    { }

    public class ChartTextInputStartCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, TextCompositionEventArgs>
    {
        public ChartTextInputStartCommandBehavior(Func<object, TextCompositionEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartTextInputStartCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.TextInputStart += OnEventRaised;
        }
    }

    // ChartTextInputStartCommandBehavior
    public class ChartTextInputStartCommandBehavior : ChartTextInputStartCommandBehavior<object>
    { }
    #endregion

    #region ChartTextInputUpdateCommand
    // ChartTextInputUpdateCommand
    public class ChartTextInputUpdateCommand : ChartCommandBase<ChartTextInputUpdateCommandBehavior>
    { }

    public class ChartTextInputUpdateCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartTextInputUpdateCommandBehavior<T>, new()
    { }

    public class ChartTextInputUpdateCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, TextCompositionEventArgs>
    {
        public ChartTextInputUpdateCommandBehavior(Func<object, TextCompositionEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartTextInputUpdateCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.TextInputUpdate += OnEventRaised;
        }
    }

    // ChartTextInputUpdateCommandBehavior
    public class ChartTextInputUpdateCommandBehavior : ChartTextInputUpdateCommandBehavior<object>
    { }
    #endregion

    #region ChartDragEnterCommand
    // ChartDragEnterCommand
    public class ChartDragEnterCommand : ChartCommandBase<ChartDragEnterCommandBehavior>
    { }

    public class ChartDragEnterCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartDragEnterCommandBehavior<T>, new()
    { }

    public class ChartDragEnterCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, DragEventArgs>
    {
        public ChartDragEnterCommandBehavior(Func<object, DragEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartDragEnterCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.DragEnter += OnEventRaised;
        }
    }

    // ChartDragEnterCommandBehavior
    public class ChartDragEnterCommandBehavior : ChartDragEnterCommandBehavior<object>
    { }
    #endregion

    #region ChartDragLeaveCommand
    // ChartDragLeaveCommand
    public class ChartDragLeaveCommand : ChartCommandBase<ChartDragLeaveCommandBehavior>
    { }

    public class ChartDragLeaveCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartDragLeaveCommandBehavior<T>, new()
    { }

    public class ChartDragLeaveCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, DragEventArgs>
    {
        public ChartDragLeaveCommandBehavior(Func<object, DragEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartDragLeaveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.DragLeave += OnEventRaised;
        }
    }

    // ChartDragLeaveCommandBehavior
    public class ChartDragLeaveCommandBehavior : ChartDragLeaveCommandBehavior<object>
    { }
    #endregion

    #region ChartDragOverCommand
    // ChartDragOverCommand
    public class ChartDragOverCommand : ChartCommandBase<ChartDragOverCommandBehavior>
    { }

    public class ChartDragOverCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartDragOverCommandBehavior<T>, new()
    { }

    public class ChartDragOverCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, DragEventArgs>
    {
        public ChartDragOverCommandBehavior(Func<object, DragEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartDragOverCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.DragOver += OnEventRaised;
        }
    }

    // ChartDragOverCommandBehavior
    public class ChartDragOverCommandBehavior : ChartDragOverCommandBehavior<object>
    { }
    #endregion

    #region ChartDropCommand
    // ChartDropCommand
    public class ChartDropCommand : ChartCommandBase<ChartDropCommandBehavior>
    { }

    public class ChartDropCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartDropCommandBehavior<T>, new()
    { }

    public class ChartDropCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, DragEventArgs>
    {
        public ChartDropCommandBehavior(Func<object, DragEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartDropCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Drop += OnEventRaised;
        }
    }

    // ChartDropCommandBehavior
    public class ChartDropCommandBehavior : ChartDropCommandBehavior<object>
    { }
    #endregion

    #region ChartManipulationStartedCommand
    // ChartManipulationStartedCommand
    public class ChartManipulationStartedCommand : ChartCommandBase<ChartManipulationStartedCommandBehavior>
    { }

    public class ChartManipulationStartedCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartManipulationStartedCommandBehavior<T>, new()
    { }

    public class ChartManipulationStartedCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, ManipulationStartedEventArgs>
    {
        public ChartManipulationStartedCommandBehavior(Func<object, ManipulationStartedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartManipulationStartedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ManipulationStarted += OnEventRaised;
        }
    }

    // ChartManipulationStartedCommandBehavior
    public class ChartManipulationStartedCommandBehavior : ChartManipulationStartedCommandBehavior<object>
    { }
    #endregion

    #region ChartManipulationDeltaCommand
    // ChartManipulationDeltaCommand
    public class ChartManipulationDeltaCommand : ChartCommandBase<ChartManipulationDeltaCommandBehavior>
    { }

    public class ChartManipulationDeltaCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartManipulationDeltaCommandBehavior<T>, new()
    { }

    public class ChartManipulationDeltaCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, ManipulationDeltaEventArgs>
    {
        public ChartManipulationDeltaCommandBehavior(Func<object, ManipulationDeltaEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartManipulationDeltaCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ManipulationDelta += OnEventRaised;
        }
    }

    // ChartManipulationDeltaCommandBehavior
    public class ChartManipulationDeltaCommandBehavior : ChartManipulationDeltaCommandBehavior<object>
    { }
    #endregion

    #region ChartManipulationCompletedCommand
    // ChartManipulationCompletedCommand
    public class ChartManipulationCompletedCommand : ChartCommandBase<ChartManipulationCompletedCommandBehavior>
    { }

    public class ChartManipulationCompletedCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartManipulationCompletedCommandBehavior<T>, new()
    { }

    public class ChartManipulationCompletedCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, ManipulationCompletedEventArgs>
    {
        public ChartManipulationCompletedCommandBehavior(Func<object, ManipulationCompletedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartManipulationCompletedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ManipulationCompleted += OnEventRaised;
        }
    }

    // ChartManipulationCompletedCommandBehavior
    public class ChartManipulationCompletedCommandBehavior : ChartManipulationCompletedCommandBehavior<object>
    { }
    #endregion
#if SILVERLIGHT5
    #region ChartTapCommand
    // ChartTapCommand
    public class ChartTapCommand : ChartCommandBase<ChartTapCommandBehavior>
    { }

    public class ChartTapCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartTapCommandBehavior<T>, new()
    { }

    public class ChartTapCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, GestureEventArgs>
    {
        public ChartTapCommandBehavior(Func<object, GestureEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartTapCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Tap += OnEventRaised;
        }
    }

    // ChartTapCommandBehavior
    public class ChartTapCommandBehavior : ChartTapCommandBehavior<object>
    { }
    #endregion

    #region ChartDoubleTapCommand
    // ChartDoubleTapCommand
    public class ChartDoubleTapCommand : ChartCommandBase<ChartDoubleTapCommandBehavior>
    { }

    public class ChartDoubleTapCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartDoubleTapCommandBehavior<T>, new()
    { }

    public class ChartDoubleTapCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, GestureEventArgs>
    {
        public ChartDoubleTapCommandBehavior(Func<object, GestureEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartDoubleTapCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.DoubleTap += OnEventRaised;
        }
    }

    // ChartDoubleTapCommandBehavior
    public class ChartDoubleTapCommandBehavior : ChartDoubleTapCommandBehavior<object>
    { }
    #endregion

    #region ChartHoldCommand
    // ChartHoldCommand
    public class ChartHoldCommand : ChartCommandBase<ChartHoldCommandBehavior>
    { }

    public class ChartHoldCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartHoldCommandBehavior<T>, new()
    { }

    public class ChartHoldCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, GestureEventArgs>
    {
        public ChartHoldCommandBehavior(Func<object, GestureEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartHoldCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Hold += OnEventRaised;
        }
    }

    // ChartHoldCommandBehavior
    public class ChartHoldCommandBehavior : ChartHoldCommandBehavior<object>
    { }
    #endregion
#endif
    #endregion

}
