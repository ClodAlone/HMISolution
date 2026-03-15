#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Syncfusion.Windows;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Controls.Grid;
using System.Windows.Input;
using System.Windows.Data;
using System.Windows;
using System.Windows.Controls;

namespace Syncfusion.Windows.Controls.Spreadsheet.MVVM
{
    #region SpreadsheetControlCommandBase
    public class SpreadsheetControlCommandBase<TBehavior> : ControlCommandBase<TBehavior, SpreadsheetControl> where TBehavior : CommandBehaviorBase<SpreadsheetControl>, new()
    { }

    public class SpreadsheetControlCommandBehaviorBase<TReturn, TEventArgs> : BuilderCommandBehaviorBase<SpreadsheetControl, TEventArgs, TReturn>
    { }
    #endregion
    #region SpreadsheetControlWorkSheetAdding
    // SpreadsheetControlWorkSheetAddingCommand<T, TBehavior>
    public class SpreadsheetControlWorkSheetAddingCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlWorkSheetAddingCommandBehavior<T>, new()
    { }

    // SpreadsheetControlWorkSheetAddingCommandBehavior<TReturn>
    public class SpreadsheetControlWorkSheetAddingCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, WorkSheetAddingEventArgs>
    {
        public SpreadsheetControlWorkSheetAddingCommandBehavior(Func<object, WorkSheetAddingEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlWorkSheetAddingCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.WorkSheetAdding += OnEventRaised;
        }
    }

    // SpreadsheetControlWorkSheetAddingCommand
    public class SpreadsheetControlWorkSheetAddingCommand : SpreadsheetControlCommandBase<SpreadsheetControlWorkSheetAddingCommandBehavior>
    { }

    // SpreadsheetControlWorkSheetAddingCommandBehavior
    public class SpreadsheetControlWorkSheetAddingCommandBehavior : SpreadsheetControlWorkSheetAddingCommandBehavior<object>
    { }

    // SpreadsheetControlWorkSheetAddingCommandWithEventArgs	
    public class SpreadsheetControlWorkSheetAddingCommandWithEventArgs : SpreadsheetControlWorkSheetAddingCommand<WorkSheetAddingEventArgs, SpreadsheetControlWorkSheetAddingCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlWorkSheetAddingCommandBehaviorWithEventArgs
    public class SpreadsheetControlWorkSheetAddingCommandBehaviorWithEventArgs : SpreadsheetControlWorkSheetAddingCommandBehavior<WorkSheetAddingEventArgs>
    {
        public SpreadsheetControlWorkSheetAddingCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlWorkSheetAdded
    // SpreadsheetControlWorkSheetAddedCommand<T, TBehavior>
    public class SpreadsheetControlWorkSheetAddedCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlWorkSheetAddedCommandBehavior<T>, new()
    { }

    // SpreadsheetControlWorkSheetAddedCommandBehavior<TReturn>
    public class SpreadsheetControlWorkSheetAddedCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, WorkSheetAddedEventArgs>
    {
        public SpreadsheetControlWorkSheetAddedCommandBehavior(Func<object, WorkSheetAddedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlWorkSheetAddedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.WorkSheetAdded += OnEventRaised;
        }
    }

    // SpreadsheetControlWorkSheetAddedCommand
    public class SpreadsheetControlWorkSheetAddedCommand : SpreadsheetControlCommandBase<SpreadsheetControlWorkSheetAddedCommandBehavior>
    { }

    // SpreadsheetControlWorkSheetAddedCommandBehavior
    public class SpreadsheetControlWorkSheetAddedCommandBehavior : SpreadsheetControlWorkSheetAddedCommandBehavior<object>
    { }

    // SpreadsheetControlWorkSheetAddedCommandWithEventArgs	
    public class SpreadsheetControlWorkSheetAddedCommandWithEventArgs : SpreadsheetControlWorkSheetAddedCommand<WorkSheetAddedEventArgs, SpreadsheetControlWorkSheetAddedCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlWorkSheetAddedCommandBehaviorWithEventArgs
    public class SpreadsheetControlWorkSheetAddedCommandBehaviorWithEventArgs : SpreadsheetControlWorkSheetAddedCommandBehavior<WorkSheetAddedEventArgs>
    {
        public SpreadsheetControlWorkSheetAddedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlWorkBookLoaded
    // SpreadsheetControlWorkBookLoadedCommand<T, TBehavior>
    public class SpreadsheetControlWorkBookLoadedCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlWorkBookLoadedCommandBehavior<T>, new()
    { }

    // SpreadsheetControlWorkBookLoadedCommandBehavior<TReturn>
    public class SpreadsheetControlWorkBookLoadedCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, WorkbookLoadedEventArgs>
    {
        public SpreadsheetControlWorkBookLoadedCommandBehavior(Func<object, WorkbookLoadedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlWorkBookLoadedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.WorkBookLoaded += OnEventRaised;
        }
    }

    // SpreadsheetControlWorkBookLoadedCommand
    public class SpreadsheetControlWorkBookLoadedCommand : SpreadsheetControlCommandBase<SpreadsheetControlWorkBookLoadedCommandBehavior>
    { }

    // SpreadsheetControlWorkBookLoadedCommandBehavior
    public class SpreadsheetControlWorkBookLoadedCommandBehavior : SpreadsheetControlWorkBookLoadedCommandBehavior<object>
    { }

    // SpreadsheetControlWorkBookLoadedCommandWithEventArgs	
    public class SpreadsheetControlWorkBookLoadedCommandWithEventArgs : SpreadsheetControlWorkBookLoadedCommand<WorkbookLoadedEventArgs, SpreadsheetControlWorkBookLoadedCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlWorkBookLoadedCommandBehaviorWithEventArgs
    public class SpreadsheetControlWorkBookLoadedCommandBehaviorWithEventArgs : SpreadsheetControlWorkBookLoadedCommandBehavior<WorkbookLoadedEventArgs>
    {
        public SpreadsheetControlWorkBookLoadedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlCurrentCellValidating
    // SpreadsheetControlCurrentCellValidatingCommand<T, TBehavior>
    public class SpreadsheetControlCurrentCellValidatingCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlCurrentCellValidatingCommandBehavior<T>, new()
    { }
#if !SILVERLIGHT
    // SpreadsheetControlCurrentCellValidatingCommandBehavior<TReturn>
    public class SpreadsheetControlCurrentCellValidatingCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, CurrentCellValidatingEventArgs>
    {
        public SpreadsheetControlCurrentCellValidatingCommandBehavior(Func<object, CurrentCellValidatingEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlCurrentCellValidatingCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.CurrentCellValidating += OnEventRaised;
        }
    }
#else
    // SpreadsheetControlCurrentCellValidatingCommandBehavior<TReturn>
    public class SpreadsheetControlCurrentCellValidatingCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, CurrentCellValidateEventArgs>
    {
        public SpreadsheetControlCurrentCellValidatingCommandBehavior(Func<object, CurrentCellValidateEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlCurrentCellValidatingCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.CurrentCellValidating += OnEventRaised;
        }
    }
#endif

    // SpreadsheetControlCurrentCellValidatingCommand
    public class SpreadsheetControlCurrentCellValidatingCommand : SpreadsheetControlCommandBase<SpreadsheetControlCurrentCellValidatingCommandBehavior>
    { }

    // SpreadsheetControlCurrentCellValidatingCommandBehavior
    public class SpreadsheetControlCurrentCellValidatingCommandBehavior : SpreadsheetControlCurrentCellValidatingCommandBehavior<object>
    { }
#if !SILVERLIGHT
    // SpreadsheetControlCurrentCellValidatingCommandWithEventArgs	
    public class SpreadsheetControlCurrentCellValidatingCommandWithEventArgs : SpreadsheetControlCurrentCellValidatingCommand<CurrentCellValidatingEventArgs, SpreadsheetControlCurrentCellValidatingCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlCurrentCellValidatingCommandBehaviorWithEventArgs
    public class SpreadsheetControlCurrentCellValidatingCommandBehaviorWithEventArgs : SpreadsheetControlCurrentCellValidatingCommandBehavior<CurrentCellValidatingEventArgs>
    {
        public SpreadsheetControlCurrentCellValidatingCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
#else
    // SpreadsheetControlCurrentCellValidatingCommandWithEventArgs	
    public class SpreadsheetControlCurrentCellValidatingCommandWithEventArgs : SpreadsheetControlCurrentCellValidatingCommand<CurrentCellValidateEventArgs, SpreadsheetControlCurrentCellValidatingCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlCurrentCellValidatingCommandBehaviorWithEventArgs
    public class SpreadsheetControlCurrentCellValidatingCommandBehaviorWithEventArgs : SpreadsheetControlCurrentCellValidatingCommandBehavior<CurrentCellValidateEventArgs>
    {
        public SpreadsheetControlCurrentCellValidatingCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
#endif
    #endregion
    #region SpreadsheetControlCellRequestNavigate
    // SpreadsheetControlCellRequestNavigateCommand<T, TBehavior>
    public class SpreadsheetControlCellRequestNavigateCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlCellRequestNavigateCommandBehavior<T>, new()
    { }

    // SpreadsheetControlCellRequestNavigateCommandBehavior<TReturn>
    public class SpreadsheetControlCellRequestNavigateCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, CellRequestNavigateEventArgs>
    {
        public SpreadsheetControlCellRequestNavigateCommandBehavior(Func<object, CellRequestNavigateEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlCellRequestNavigateCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.CellRequestNavigate += OnEventRaised;
        }
    }

    // SpreadsheetControlCellRequestNavigateCommand
    public class SpreadsheetControlCellRequestNavigateCommand : SpreadsheetControlCommandBase<SpreadsheetControlCellRequestNavigateCommandBehavior>
    { }

    // SpreadsheetControlCellRequestNavigateCommandBehavior
    public class SpreadsheetControlCellRequestNavigateCommandBehavior : SpreadsheetControlCellRequestNavigateCommandBehavior<object>
    { }

    // SpreadsheetControlCellRequestNavigateCommandWithEventArgs	
    public class SpreadsheetControlCellRequestNavigateCommandWithEventArgs : SpreadsheetControlCellRequestNavigateCommand<CellRequestNavigateEventArgs, SpreadsheetControlCellRequestNavigateCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlCellRequestNavigateCommandBehaviorWithEventArgs
    public class SpreadsheetControlCellRequestNavigateCommandBehaviorWithEventArgs : SpreadsheetControlCellRequestNavigateCommandBehavior<CellRequestNavigateEventArgs>
    {
        public SpreadsheetControlCellRequestNavigateCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlSizeChanged
    // SpreadsheetControlSizeChangedCommand<T, TBehavior>
    public class SpreadsheetControlSizeChangedCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlSizeChangedCommandBehavior<T>, new()
    { }

    // SpreadsheetControlSizeChangedCommandBehavior<TReturn>
    public class SpreadsheetControlSizeChangedCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, SizeChangedEventArgs>
    {
        public SpreadsheetControlSizeChangedCommandBehavior(Func<object, SizeChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlSizeChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.SizeChanged += OnEventRaised;
        }
    }

    // SpreadsheetControlSizeChangedCommand
    public class SpreadsheetControlSizeChangedCommand : SpreadsheetControlCommandBase<SpreadsheetControlSizeChangedCommandBehavior>
    { }

    // SpreadsheetControlSizeChangedCommandBehavior
    public class SpreadsheetControlSizeChangedCommandBehavior : SpreadsheetControlSizeChangedCommandBehavior<object>
    { }

    // SpreadsheetControlSizeChangedCommandWithEventArgs	
    public class SpreadsheetControlSizeChangedCommandWithEventArgs : SpreadsheetControlSizeChangedCommand<SizeChangedEventArgs, SpreadsheetControlSizeChangedCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlSizeChangedCommandBehaviorWithEventArgs
    public class SpreadsheetControlSizeChangedCommandBehaviorWithEventArgs : SpreadsheetControlSizeChangedCommandBehavior<SizeChangedEventArgs>
    {
        public SpreadsheetControlSizeChangedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlLoaded
    // SpreadsheetControlLoadedCommand<T, TBehavior>
    public class SpreadsheetControlLoadedCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlLoadedCommandBehavior<T>, new()
    { }

    // SpreadsheetControlLoadedCommandBehavior<TReturn>
    public class SpreadsheetControlLoadedCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, RoutedEventArgs>
    {
        public SpreadsheetControlLoadedCommandBehavior(Func<object, RoutedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlLoadedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Loaded += OnEventRaised;
        }
    }

    // SpreadsheetControlLoadedCommand
    public class SpreadsheetControlLoadedCommand : SpreadsheetControlCommandBase<SpreadsheetControlLoadedCommandBehavior>
    { }

    // SpreadsheetControlLoadedCommandBehavior
    public class SpreadsheetControlLoadedCommandBehavior : SpreadsheetControlLoadedCommandBehavior<object>
    { }

    // SpreadsheetControlLoadedCommandWithEventArgs	
    public class SpreadsheetControlLoadedCommandWithEventArgs : SpreadsheetControlLoadedCommand<RoutedEventArgs, SpreadsheetControlLoadedCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlLoadedCommandBehaviorWithEventArgs
    public class SpreadsheetControlLoadedCommandBehaviorWithEventArgs : SpreadsheetControlLoadedCommandBehavior<RoutedEventArgs>
    {
        public SpreadsheetControlLoadedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlUnloaded
    // SpreadsheetControlUnloadedCommand<T, TBehavior>
    public class SpreadsheetControlUnloadedCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlUnloadedCommandBehavior<T>, new()
    { }

    // SpreadsheetControlUnloadedCommandBehavior<TReturn>
    public class SpreadsheetControlUnloadedCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, RoutedEventArgs>
    {
        public SpreadsheetControlUnloadedCommandBehavior(Func<object, RoutedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlUnloadedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Unloaded += OnEventRaised;
        }
    }

    // SpreadsheetControlUnloadedCommand
    public class SpreadsheetControlUnloadedCommand : SpreadsheetControlCommandBase<SpreadsheetControlUnloadedCommandBehavior>
    { }

    // SpreadsheetControlUnloadedCommandBehavior
    public class SpreadsheetControlUnloadedCommandBehavior : SpreadsheetControlUnloadedCommandBehavior<object>
    { }

    // SpreadsheetControlUnloadedCommandWithEventArgs	
    public class SpreadsheetControlUnloadedCommandWithEventArgs : SpreadsheetControlUnloadedCommand<RoutedEventArgs, SpreadsheetControlUnloadedCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlUnloadedCommandBehaviorWithEventArgs
    public class SpreadsheetControlUnloadedCommandBehaviorWithEventArgs : SpreadsheetControlUnloadedCommandBehavior<RoutedEventArgs>
    {
        public SpreadsheetControlUnloadedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlMouseLeftButtonDown
    // SpreadsheetControlMouseLeftButtonDownCommand<T, TBehavior>
    public class SpreadsheetControlMouseLeftButtonDownCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlMouseLeftButtonDownCommandBehavior<T>, new()
    { }

    // SpreadsheetControlMouseLeftButtonDownCommandBehavior<TReturn>
    public class SpreadsheetControlMouseLeftButtonDownCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public SpreadsheetControlMouseLeftButtonDownCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlMouseLeftButtonDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseLeftButtonDown += OnEventRaised;
        }
    }

    // SpreadsheetControlMouseLeftButtonDownCommand
    public class SpreadsheetControlMouseLeftButtonDownCommand : SpreadsheetControlCommandBase<SpreadsheetControlMouseLeftButtonDownCommandBehavior>
    { }

    // SpreadsheetControlMouseLeftButtonDownCommandBehavior
    public class SpreadsheetControlMouseLeftButtonDownCommandBehavior : SpreadsheetControlMouseLeftButtonDownCommandBehavior<object>
    { }

    // SpreadsheetControlMouseLeftButtonDownCommandWithEventArgs	
    public class SpreadsheetControlMouseLeftButtonDownCommandWithEventArgs : SpreadsheetControlMouseLeftButtonDownCommand<MouseButtonEventArgs, SpreadsheetControlMouseLeftButtonDownCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlMouseLeftButtonDownCommandBehaviorWithEventArgs
    public class SpreadsheetControlMouseLeftButtonDownCommandBehaviorWithEventArgs : SpreadsheetControlMouseLeftButtonDownCommandBehavior<MouseButtonEventArgs>
    {
        public SpreadsheetControlMouseLeftButtonDownCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlMouseLeftButtonUp
    // SpreadsheetControlMouseLeftButtonUpCommand<T, TBehavior>
    public class SpreadsheetControlMouseLeftButtonUpCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlMouseLeftButtonUpCommandBehavior<T>, new()
    { }

    // SpreadsheetControlMouseLeftButtonUpCommandBehavior<TReturn>
    public class SpreadsheetControlMouseLeftButtonUpCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public SpreadsheetControlMouseLeftButtonUpCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlMouseLeftButtonUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseLeftButtonUp += OnEventRaised;
        }
    }

    // SpreadsheetControlMouseLeftButtonUpCommand
    public class SpreadsheetControlMouseLeftButtonUpCommand : SpreadsheetControlCommandBase<SpreadsheetControlMouseLeftButtonUpCommandBehavior>
    { }

    // SpreadsheetControlMouseLeftButtonUpCommandBehavior
    public class SpreadsheetControlMouseLeftButtonUpCommandBehavior : SpreadsheetControlMouseLeftButtonUpCommandBehavior<object>
    { }

    // SpreadsheetControlMouseLeftButtonUpCommandWithEventArgs	
    public class SpreadsheetControlMouseLeftButtonUpCommandWithEventArgs : SpreadsheetControlMouseLeftButtonUpCommand<MouseButtonEventArgs, SpreadsheetControlMouseLeftButtonUpCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlMouseLeftButtonUpCommandBehaviorWithEventArgs
    public class SpreadsheetControlMouseLeftButtonUpCommandBehaviorWithEventArgs : SpreadsheetControlMouseLeftButtonUpCommandBehavior<MouseButtonEventArgs>
    {
        public SpreadsheetControlMouseLeftButtonUpCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlMouseRightButtonDown
    // SpreadsheetControlMouseRightButtonDownCommand<T, TBehavior>
    public class SpreadsheetControlMouseRightButtonDownCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlMouseRightButtonDownCommandBehavior<T>, new()
    { }

    // SpreadsheetControlMouseRightButtonDownCommandBehavior<TReturn>
    public class SpreadsheetControlMouseRightButtonDownCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public SpreadsheetControlMouseRightButtonDownCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlMouseRightButtonDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseRightButtonDown += OnEventRaised;
        }
    }

    // SpreadsheetControlMouseRightButtonDownCommand
    public class SpreadsheetControlMouseRightButtonDownCommand : SpreadsheetControlCommandBase<SpreadsheetControlMouseRightButtonDownCommandBehavior>
    { }

    // SpreadsheetControlMouseRightButtonDownCommandBehavior
    public class SpreadsheetControlMouseRightButtonDownCommandBehavior : SpreadsheetControlMouseRightButtonDownCommandBehavior<object>
    { }

    // SpreadsheetControlMouseRightButtonDownCommandWithEventArgs	
    public class SpreadsheetControlMouseRightButtonDownCommandWithEventArgs : SpreadsheetControlMouseRightButtonDownCommand<MouseButtonEventArgs, SpreadsheetControlMouseRightButtonDownCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlMouseRightButtonDownCommandBehaviorWithEventArgs
    public class SpreadsheetControlMouseRightButtonDownCommandBehaviorWithEventArgs : SpreadsheetControlMouseRightButtonDownCommandBehavior<MouseButtonEventArgs>
    {
        public SpreadsheetControlMouseRightButtonDownCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlMouseRightButtonUp
    // SpreadsheetControlMouseRightButtonUpCommand<T, TBehavior>
    public class SpreadsheetControlMouseRightButtonUpCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlMouseRightButtonUpCommandBehavior<T>, new()
    { }

    // SpreadsheetControlMouseRightButtonUpCommandBehavior<TReturn>
    public class SpreadsheetControlMouseRightButtonUpCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public SpreadsheetControlMouseRightButtonUpCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlMouseRightButtonUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseRightButtonUp += OnEventRaised;
        }
    }

    // SpreadsheetControlMouseRightButtonUpCommand
    public class SpreadsheetControlMouseRightButtonUpCommand : SpreadsheetControlCommandBase<SpreadsheetControlMouseRightButtonUpCommandBehavior>
    { }

    // SpreadsheetControlMouseRightButtonUpCommandBehavior
    public class SpreadsheetControlMouseRightButtonUpCommandBehavior : SpreadsheetControlMouseRightButtonUpCommandBehavior<object>
    { }

    // SpreadsheetControlMouseRightButtonUpCommandWithEventArgs	
    public class SpreadsheetControlMouseRightButtonUpCommandWithEventArgs : SpreadsheetControlMouseRightButtonUpCommand<MouseButtonEventArgs, SpreadsheetControlMouseRightButtonUpCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlMouseRightButtonUpCommandBehaviorWithEventArgs
    public class SpreadsheetControlMouseRightButtonUpCommandBehaviorWithEventArgs : SpreadsheetControlMouseRightButtonUpCommandBehavior<MouseButtonEventArgs>
    {
        public SpreadsheetControlMouseRightButtonUpCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlMouseMove
    // SpreadsheetControlMouseMoveCommand<T, TBehavior>
    public class SpreadsheetControlMouseMoveCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlMouseMoveCommandBehavior<T>, new()
    { }

    // SpreadsheetControlMouseMoveCommandBehavior<TReturn>
    public class SpreadsheetControlMouseMoveCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, MouseEventArgs>
    {
        public SpreadsheetControlMouseMoveCommandBehavior(Func<object, MouseEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlMouseMoveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseMove += OnEventRaised;
        }
    }

    // SpreadsheetControlMouseMoveCommand
    public class SpreadsheetControlMouseMoveCommand : SpreadsheetControlCommandBase<SpreadsheetControlMouseMoveCommandBehavior>
    { }

    // SpreadsheetControlMouseMoveCommandBehavior
    public class SpreadsheetControlMouseMoveCommandBehavior : SpreadsheetControlMouseMoveCommandBehavior<object>
    { }

    // SpreadsheetControlMouseMoveCommandWithEventArgs	
    public class SpreadsheetControlMouseMoveCommandWithEventArgs : SpreadsheetControlMouseMoveCommand<MouseEventArgs, SpreadsheetControlMouseMoveCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlMouseMoveCommandBehaviorWithEventArgs
    public class SpreadsheetControlMouseMoveCommandBehaviorWithEventArgs : SpreadsheetControlMouseMoveCommandBehavior<MouseEventArgs>
    {
        public SpreadsheetControlMouseMoveCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlMouseWheel
    // SpreadsheetControlMouseWheelCommand<T, TBehavior>
    public class SpreadsheetControlMouseWheelCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlMouseWheelCommandBehavior<T>, new()
    { }

    // SpreadsheetControlMouseWheelCommandBehavior<TReturn>
    public class SpreadsheetControlMouseWheelCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, MouseWheelEventArgs>
    {
        public SpreadsheetControlMouseWheelCommandBehavior(Func<object, MouseWheelEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlMouseWheelCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseWheel += OnEventRaised;
        }
    }

    // SpreadsheetControlMouseWheelCommand
    public class SpreadsheetControlMouseWheelCommand : SpreadsheetControlCommandBase<SpreadsheetControlMouseWheelCommandBehavior>
    { }

    // SpreadsheetControlMouseWheelCommandBehavior
    public class SpreadsheetControlMouseWheelCommandBehavior : SpreadsheetControlMouseWheelCommandBehavior<object>
    { }

    // SpreadsheetControlMouseWheelCommandWithEventArgs	
    public class SpreadsheetControlMouseWheelCommandWithEventArgs : SpreadsheetControlMouseWheelCommand<MouseWheelEventArgs, SpreadsheetControlMouseWheelCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlMouseWheelCommandBehaviorWithEventArgs
    public class SpreadsheetControlMouseWheelCommandBehaviorWithEventArgs : SpreadsheetControlMouseWheelCommandBehavior<MouseWheelEventArgs>
    {
        public SpreadsheetControlMouseWheelCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlMouseEnter
    // SpreadsheetControlMouseEnterCommand<T, TBehavior>
    public class SpreadsheetControlMouseEnterCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlMouseEnterCommandBehavior<T>, new()
    { }

    // SpreadsheetControlMouseEnterCommandBehavior<TReturn>
    public class SpreadsheetControlMouseEnterCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, MouseEventArgs>
    {
        public SpreadsheetControlMouseEnterCommandBehavior(Func<object, MouseEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlMouseEnterCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseEnter += OnEventRaised;
        }
    }

    // SpreadsheetControlMouseEnterCommand
    public class SpreadsheetControlMouseEnterCommand : SpreadsheetControlCommandBase<SpreadsheetControlMouseEnterCommandBehavior>
    { }

    // SpreadsheetControlMouseEnterCommandBehavior
    public class SpreadsheetControlMouseEnterCommandBehavior : SpreadsheetControlMouseEnterCommandBehavior<object>
    { }

    // SpreadsheetControlMouseEnterCommandWithEventArgs	
    public class SpreadsheetControlMouseEnterCommandWithEventArgs : SpreadsheetControlMouseEnterCommand<MouseEventArgs, SpreadsheetControlMouseEnterCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlMouseEnterCommandBehaviorWithEventArgs
    public class SpreadsheetControlMouseEnterCommandBehaviorWithEventArgs : SpreadsheetControlMouseEnterCommandBehavior<MouseEventArgs>
    {
        public SpreadsheetControlMouseEnterCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlMouseLeave
    // SpreadsheetControlMouseLeaveCommand<T, TBehavior>
    public class SpreadsheetControlMouseLeaveCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlMouseLeaveCommandBehavior<T>, new()
    { }

    // SpreadsheetControlMouseLeaveCommandBehavior<TReturn>
    public class SpreadsheetControlMouseLeaveCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, MouseEventArgs>
    {
        public SpreadsheetControlMouseLeaveCommandBehavior(Func<object, MouseEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlMouseLeaveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseLeave += OnEventRaised;
        }
    }

    // SpreadsheetControlMouseLeaveCommand
    public class SpreadsheetControlMouseLeaveCommand : SpreadsheetControlCommandBase<SpreadsheetControlMouseLeaveCommandBehavior>
    { }

    // SpreadsheetControlMouseLeaveCommandBehavior
    public class SpreadsheetControlMouseLeaveCommandBehavior : SpreadsheetControlMouseLeaveCommandBehavior<object>
    { }

    // SpreadsheetControlMouseLeaveCommandWithEventArgs	
    public class SpreadsheetControlMouseLeaveCommandWithEventArgs : SpreadsheetControlMouseLeaveCommand<MouseEventArgs, SpreadsheetControlMouseLeaveCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlMouseLeaveCommandBehaviorWithEventArgs
    public class SpreadsheetControlMouseLeaveCommandBehaviorWithEventArgs : SpreadsheetControlMouseLeaveCommandBehavior<MouseEventArgs>
    {
        public SpreadsheetControlMouseLeaveCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlLostMouseCapture
    // SpreadsheetControlLostMouseCaptureCommand<T, TBehavior>
    public class SpreadsheetControlLostMouseCaptureCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlLostMouseCaptureCommandBehavior<T>, new()
    { }

    // SpreadsheetControlLostMouseCaptureCommandBehavior<TReturn>
    public class SpreadsheetControlLostMouseCaptureCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, MouseEventArgs>
    {
        public SpreadsheetControlLostMouseCaptureCommandBehavior(Func<object, MouseEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlLostMouseCaptureCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.LostMouseCapture += OnEventRaised;
        }
    }

    // SpreadsheetControlLostMouseCaptureCommand
    public class SpreadsheetControlLostMouseCaptureCommand : SpreadsheetControlCommandBase<SpreadsheetControlLostMouseCaptureCommandBehavior>
    { }

    // SpreadsheetControlLostMouseCaptureCommandBehavior
    public class SpreadsheetControlLostMouseCaptureCommandBehavior : SpreadsheetControlLostMouseCaptureCommandBehavior<object>
    { }

    // SpreadsheetControlLostMouseCaptureCommandWithEventArgs	
    public class SpreadsheetControlLostMouseCaptureCommandWithEventArgs : SpreadsheetControlLostMouseCaptureCommand<MouseEventArgs, SpreadsheetControlLostMouseCaptureCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlLostMouseCaptureCommandBehaviorWithEventArgs
    public class SpreadsheetControlLostMouseCaptureCommandBehaviorWithEventArgs : SpreadsheetControlLostMouseCaptureCommandBehavior<MouseEventArgs>
    {
        public SpreadsheetControlLostMouseCaptureCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlKeyDown
    // SpreadsheetControlKeyDownCommand<T, TBehavior>
    public class SpreadsheetControlKeyDownCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlKeyDownCommandBehavior<T>, new()
    { }

    // SpreadsheetControlKeyDownCommandBehavior<TReturn>
    public class SpreadsheetControlKeyDownCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, KeyEventArgs>
    {
        public SpreadsheetControlKeyDownCommandBehavior(Func<object, KeyEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlKeyDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.KeyDown += OnEventRaised;
        }
    }

    // SpreadsheetControlKeyDownCommand
    public class SpreadsheetControlKeyDownCommand : SpreadsheetControlCommandBase<SpreadsheetControlKeyDownCommandBehavior>
    { }

    // SpreadsheetControlKeyDownCommandBehavior
    public class SpreadsheetControlKeyDownCommandBehavior : SpreadsheetControlKeyDownCommandBehavior<object>
    { }

    // SpreadsheetControlKeyDownCommandWithEventArgs	
    public class SpreadsheetControlKeyDownCommandWithEventArgs : SpreadsheetControlKeyDownCommand<KeyEventArgs, SpreadsheetControlKeyDownCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlKeyDownCommandBehaviorWithEventArgs
    public class SpreadsheetControlKeyDownCommandBehaviorWithEventArgs : SpreadsheetControlKeyDownCommandBehavior<KeyEventArgs>
    {
        public SpreadsheetControlKeyDownCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlKeyUp
    // SpreadsheetControlKeyUpCommand<T, TBehavior>
    public class SpreadsheetControlKeyUpCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlKeyUpCommandBehavior<T>, new()
    { }

    // SpreadsheetControlKeyUpCommandBehavior<TReturn>
    public class SpreadsheetControlKeyUpCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, KeyEventArgs>
    {
        public SpreadsheetControlKeyUpCommandBehavior(Func<object, KeyEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlKeyUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.KeyUp += OnEventRaised;
        }
    }

    // SpreadsheetControlKeyUpCommand
    public class SpreadsheetControlKeyUpCommand : SpreadsheetControlCommandBase<SpreadsheetControlKeyUpCommandBehavior>
    { }

    // SpreadsheetControlKeyUpCommandBehavior
    public class SpreadsheetControlKeyUpCommandBehavior : SpreadsheetControlKeyUpCommandBehavior<object>
    { }

    // SpreadsheetControlKeyUpCommandWithEventArgs	
    public class SpreadsheetControlKeyUpCommandWithEventArgs : SpreadsheetControlKeyUpCommand<KeyEventArgs, SpreadsheetControlKeyUpCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlKeyUpCommandBehaviorWithEventArgs
    public class SpreadsheetControlKeyUpCommandBehaviorWithEventArgs : SpreadsheetControlKeyUpCommandBehavior<KeyEventArgs>
    {
        public SpreadsheetControlKeyUpCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlTextInput
    // SpreadsheetControlTextInputCommand<T, TBehavior>
    public class SpreadsheetControlTextInputCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlTextInputCommandBehavior<T>, new()
    { }

    // SpreadsheetControlTextInputCommandBehavior<TReturn>
    public class SpreadsheetControlTextInputCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, TextCompositionEventArgs>
    {
        public SpreadsheetControlTextInputCommandBehavior(Func<object, TextCompositionEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlTextInputCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.TextInput += OnEventRaised;
        }
    }

    // SpreadsheetControlTextInputCommand
    public class SpreadsheetControlTextInputCommand : SpreadsheetControlCommandBase<SpreadsheetControlTextInputCommandBehavior>
    { }

    // SpreadsheetControlTextInputCommandBehavior
    public class SpreadsheetControlTextInputCommandBehavior : SpreadsheetControlTextInputCommandBehavior<object>
    { }

    // SpreadsheetControlTextInputCommandWithEventArgs	
    public class SpreadsheetControlTextInputCommandWithEventArgs : SpreadsheetControlTextInputCommand<TextCompositionEventArgs, SpreadsheetControlTextInputCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlTextInputCommandBehaviorWithEventArgs
    public class SpreadsheetControlTextInputCommandBehaviorWithEventArgs : SpreadsheetControlTextInputCommandBehavior<TextCompositionEventArgs>
    {
        public SpreadsheetControlTextInputCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlDragEnter
    // SpreadsheetControlDragEnterCommand<T, TBehavior>
    public class SpreadsheetControlDragEnterCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlDragEnterCommandBehavior<T>, new()
    { }

    // SpreadsheetControlDragEnterCommandBehavior<TReturn>
    public class SpreadsheetControlDragEnterCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, DragEventArgs>
    {
        public SpreadsheetControlDragEnterCommandBehavior(Func<object, DragEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlDragEnterCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.DragEnter += OnEventRaised;
        }
    }

    // SpreadsheetControlDragEnterCommand
    public class SpreadsheetControlDragEnterCommand : SpreadsheetControlCommandBase<SpreadsheetControlDragEnterCommandBehavior>
    { }

    // SpreadsheetControlDragEnterCommandBehavior
    public class SpreadsheetControlDragEnterCommandBehavior : SpreadsheetControlDragEnterCommandBehavior<object>
    { }

    // SpreadsheetControlDragEnterCommandWithEventArgs	
    public class SpreadsheetControlDragEnterCommandWithEventArgs : SpreadsheetControlDragEnterCommand<DragEventArgs, SpreadsheetControlDragEnterCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlDragEnterCommandBehaviorWithEventArgs
    public class SpreadsheetControlDragEnterCommandBehaviorWithEventArgs : SpreadsheetControlDragEnterCommandBehavior<DragEventArgs>
    {
        public SpreadsheetControlDragEnterCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlDragOver
    // SpreadsheetControlDragOverCommand<T, TBehavior>
    public class SpreadsheetControlDragOverCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlDragOverCommandBehavior<T>, new()
    { }

    // SpreadsheetControlDragOverCommandBehavior<TReturn>
    public class SpreadsheetControlDragOverCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, DragEventArgs>
    {
        public SpreadsheetControlDragOverCommandBehavior(Func<object, DragEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlDragOverCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.DragOver += OnEventRaised;
        }
    }

    // SpreadsheetControlDragOverCommand
    public class SpreadsheetControlDragOverCommand : SpreadsheetControlCommandBase<SpreadsheetControlDragOverCommandBehavior>
    { }

    // SpreadsheetControlDragOverCommandBehavior
    public class SpreadsheetControlDragOverCommandBehavior : SpreadsheetControlDragOverCommandBehavior<object>
    { }

    // SpreadsheetControlDragOverCommandWithEventArgs	
    public class SpreadsheetControlDragOverCommandWithEventArgs : SpreadsheetControlDragOverCommand<DragEventArgs, SpreadsheetControlDragOverCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlDragOverCommandBehaviorWithEventArgs
    public class SpreadsheetControlDragOverCommandBehaviorWithEventArgs : SpreadsheetControlDragOverCommandBehavior<DragEventArgs>
    {
        public SpreadsheetControlDragOverCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlDragLeave
    // SpreadsheetControlDragLeaveCommand<T, TBehavior>
    public class SpreadsheetControlDragLeaveCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlDragLeaveCommandBehavior<T>, new()
    { }

    // SpreadsheetControlDragLeaveCommandBehavior<TReturn>
    public class SpreadsheetControlDragLeaveCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, DragEventArgs>
    {
        public SpreadsheetControlDragLeaveCommandBehavior(Func<object, DragEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlDragLeaveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.DragLeave += OnEventRaised;
        }
    }

    // SpreadsheetControlDragLeaveCommand
    public class SpreadsheetControlDragLeaveCommand : SpreadsheetControlCommandBase<SpreadsheetControlDragLeaveCommandBehavior>
    { }

    // SpreadsheetControlDragLeaveCommandBehavior
    public class SpreadsheetControlDragLeaveCommandBehavior : SpreadsheetControlDragLeaveCommandBehavior<object>
    { }

    // SpreadsheetControlDragLeaveCommandWithEventArgs	
    public class SpreadsheetControlDragLeaveCommandWithEventArgs : SpreadsheetControlDragLeaveCommand<DragEventArgs, SpreadsheetControlDragLeaveCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlDragLeaveCommandBehaviorWithEventArgs
    public class SpreadsheetControlDragLeaveCommandBehaviorWithEventArgs : SpreadsheetControlDragLeaveCommandBehavior<DragEventArgs>
    {
        public SpreadsheetControlDragLeaveCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlDrop
    // SpreadsheetControlDropCommand<T, TBehavior>
    public class SpreadsheetControlDropCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlDropCommandBehavior<T>, new()
    { }

    // SpreadsheetControlDropCommandBehavior<TReturn>
    public class SpreadsheetControlDropCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, DragEventArgs>
    {
        public SpreadsheetControlDropCommandBehavior(Func<object, DragEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlDropCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Drop += OnEventRaised;
        }
    }

    // SpreadsheetControlDropCommand
    public class SpreadsheetControlDropCommand : SpreadsheetControlCommandBase<SpreadsheetControlDropCommandBehavior>
    { }

    // SpreadsheetControlDropCommandBehavior
    public class SpreadsheetControlDropCommandBehavior : SpreadsheetControlDropCommandBehavior<object>
    { }

    // SpreadsheetControlDropCommandWithEventArgs	
    public class SpreadsheetControlDropCommandWithEventArgs : SpreadsheetControlDropCommand<DragEventArgs, SpreadsheetControlDropCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlDropCommandBehaviorWithEventArgs
    public class SpreadsheetControlDropCommandBehaviorWithEventArgs : SpreadsheetControlDropCommandBehavior<DragEventArgs>
    {
        public SpreadsheetControlDropCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlLayoutUpdated
    // SpreadsheetControlLayoutUpdatedCommand<T, TBehavior>
    public class SpreadsheetControlLayoutUpdatedCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlLayoutUpdatedCommandBehavior<T>, new()
    { }

    // SpreadsheetControlLayoutUpdatedCommandBehavior<TReturn>
    public class SpreadsheetControlLayoutUpdatedCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, EventArgs>
    {
        public SpreadsheetControlLayoutUpdatedCommandBehavior(Func<object, EventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlLayoutUpdatedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.LayoutUpdated += OnEventRaised;
        }
    }

    // SpreadsheetControlLayoutUpdatedCommand
    public class SpreadsheetControlLayoutUpdatedCommand : SpreadsheetControlCommandBase<SpreadsheetControlLayoutUpdatedCommandBehavior>
    { }

    // SpreadsheetControlLayoutUpdatedCommandBehavior
    public class SpreadsheetControlLayoutUpdatedCommandBehavior : SpreadsheetControlLayoutUpdatedCommandBehavior<object>
    { }

    // SpreadsheetControlLayoutUpdatedCommandWithEventArgs	
    public class SpreadsheetControlLayoutUpdatedCommandWithEventArgs : SpreadsheetControlLayoutUpdatedCommand<EventArgs, SpreadsheetControlLayoutUpdatedCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlLayoutUpdatedCommandBehaviorWithEventArgs
    public class SpreadsheetControlLayoutUpdatedCommandBehaviorWithEventArgs : SpreadsheetControlLayoutUpdatedCommandBehavior<EventArgs>
    {
        public SpreadsheetControlLayoutUpdatedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlGotFocus
    // SpreadsheetControlGotFocusCommand<T, TBehavior>
    public class SpreadsheetControlGotFocusCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlGotFocusCommandBehavior<T>, new()
    { }

    // SpreadsheetControlGotFocusCommandBehavior<TReturn>
    public class SpreadsheetControlGotFocusCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, RoutedEventArgs>
    {
        public SpreadsheetControlGotFocusCommandBehavior(Func<object, RoutedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlGotFocusCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.GotFocus += OnEventRaised;
        }
    }

    // SpreadsheetControlGotFocusCommand
    public class SpreadsheetControlGotFocusCommand : SpreadsheetControlCommandBase<SpreadsheetControlGotFocusCommandBehavior>
    { }

    // SpreadsheetControlGotFocusCommandBehavior
    public class SpreadsheetControlGotFocusCommandBehavior : SpreadsheetControlGotFocusCommandBehavior<object>
    { }

    // SpreadsheetControlGotFocusCommandWithEventArgs	
    public class SpreadsheetControlGotFocusCommandWithEventArgs : SpreadsheetControlGotFocusCommand<RoutedEventArgs, SpreadsheetControlGotFocusCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlGotFocusCommandBehaviorWithEventArgs
    public class SpreadsheetControlGotFocusCommandBehaviorWithEventArgs : SpreadsheetControlGotFocusCommandBehavior<RoutedEventArgs>
    {
        public SpreadsheetControlGotFocusCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlLostFocus
    // SpreadsheetControlLostFocusCommand<T, TBehavior>
    public class SpreadsheetControlLostFocusCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlLostFocusCommandBehavior<T>, new()
    { }

    // SpreadsheetControlLostFocusCommandBehavior<TReturn>
    public class SpreadsheetControlLostFocusCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, RoutedEventArgs>
    {
        public SpreadsheetControlLostFocusCommandBehavior(Func<object, RoutedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlLostFocusCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.LostFocus += OnEventRaised;
        }
    }

    // SpreadsheetControlLostFocusCommand
    public class SpreadsheetControlLostFocusCommand : SpreadsheetControlCommandBase<SpreadsheetControlLostFocusCommandBehavior>
    { }

    // SpreadsheetControlLostFocusCommandBehavior
    public class SpreadsheetControlLostFocusCommandBehavior : SpreadsheetControlLostFocusCommandBehavior<object>
    { }

    // SpreadsheetControlLostFocusCommandWithEventArgs	
    public class SpreadsheetControlLostFocusCommandWithEventArgs : SpreadsheetControlLostFocusCommand<RoutedEventArgs, SpreadsheetControlLostFocusCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlLostFocusCommandBehaviorWithEventArgs
    public class SpreadsheetControlLostFocusCommandBehaviorWithEventArgs : SpreadsheetControlLostFocusCommandBehavior<RoutedEventArgs>
    {
        public SpreadsheetControlLostFocusCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlIsEnabledChanged
    // SpreadsheetControlIsEnabledChangedCommand<T, TBehavior>
    public class SpreadsheetControlIsEnabledChangedCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlIsEnabledChangedCommandBehavior<T>, new()
    { }

    // SpreadsheetControlIsEnabledChangedCommandBehavior<TReturn>
    public class SpreadsheetControlIsEnabledChangedCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public SpreadsheetControlIsEnabledChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlIsEnabledChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsEnabledChanged += OnEventRaised;
        }
    }

    // SpreadsheetControlIsEnabledChangedCommand
    public class SpreadsheetControlIsEnabledChangedCommand : SpreadsheetControlCommandBase<SpreadsheetControlIsEnabledChangedCommandBehavior>
    { }

    // SpreadsheetControlIsEnabledChangedCommandBehavior
    public class SpreadsheetControlIsEnabledChangedCommandBehavior : SpreadsheetControlIsEnabledChangedCommandBehavior<object>
    { }

    // SpreadsheetControlIsEnabledChangedCommandWithEventArgs	
    public class SpreadsheetControlIsEnabledChangedCommandWithEventArgs : SpreadsheetControlIsEnabledChangedCommand<DependencyPropertyChangedEventArgs, SpreadsheetControlIsEnabledChangedCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlIsEnabledChangedCommandBehaviorWithEventArgs
    public class SpreadsheetControlIsEnabledChangedCommandBehaviorWithEventArgs : SpreadsheetControlIsEnabledChangedCommandBehavior<DependencyPropertyChangedEventArgs>
    {
        public SpreadsheetControlIsEnabledChangedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
#if !SILVERLIGHT
    #region SpreadsheetControlDataContextChanged
    // SpreadsheetControlDataContextChangedCommand<T, TBehavior>
    public class SpreadsheetControlDataContextChangedCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlDataContextChangedCommandBehavior<T>, new()
    { }

    // SpreadsheetControlDataContextChangedCommandBehavior<TReturn>
    public class SpreadsheetControlDataContextChangedCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public SpreadsheetControlDataContextChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlDataContextChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.DataContextChanged += OnEventRaised;
        }
    }

    // SpreadsheetControlDataContextChangedCommand
    public class SpreadsheetControlDataContextChangedCommand : SpreadsheetControlCommandBase<SpreadsheetControlDataContextChangedCommandBehavior>
    { }

    // SpreadsheetControlDataContextChangedCommandBehavior
    public class SpreadsheetControlDataContextChangedCommandBehavior : SpreadsheetControlDataContextChangedCommandBehavior<object>
    { }

    // SpreadsheetControlDataContextChangedCommandWithEventArgs	
    public class SpreadsheetControlDataContextChangedCommandWithEventArgs : SpreadsheetControlDataContextChangedCommand<DependencyPropertyChangedEventArgs, SpreadsheetControlDataContextChangedCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlDataContextChangedCommandBehaviorWithEventArgs
    public class SpreadsheetControlDataContextChangedCommandBehaviorWithEventArgs : SpreadsheetControlDataContextChangedCommandBehavior<DependencyPropertyChangedEventArgs>
    {
        public SpreadsheetControlDataContextChangedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlPreviewMouseDoubleClick
    // SpreadsheetControlPreviewMouseDoubleClickCommand<T, TBehavior>
    public class SpreadsheetControlPreviewMouseDoubleClickCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlPreviewMouseDoubleClickCommandBehavior<T>, new()
    { }

    // SpreadsheetControlPreviewMouseDoubleClickCommandBehavior<TReturn>
    public class SpreadsheetControlPreviewMouseDoubleClickCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public SpreadsheetControlPreviewMouseDoubleClickCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlPreviewMouseDoubleClickCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewMouseDoubleClick += OnEventRaised;
        }
    }

    // SpreadsheetControlPreviewMouseDoubleClickCommand
    public class SpreadsheetControlPreviewMouseDoubleClickCommand : SpreadsheetControlCommandBase<SpreadsheetControlPreviewMouseDoubleClickCommandBehavior>
    { }

    // SpreadsheetControlPreviewMouseDoubleClickCommandBehavior
    public class SpreadsheetControlPreviewMouseDoubleClickCommandBehavior : SpreadsheetControlPreviewMouseDoubleClickCommandBehavior<object>
    { }

    // SpreadsheetControlPreviewMouseDoubleClickCommandWithEventArgs	
    public class SpreadsheetControlPreviewMouseDoubleClickCommandWithEventArgs : SpreadsheetControlPreviewMouseDoubleClickCommand<MouseButtonEventArgs, SpreadsheetControlPreviewMouseDoubleClickCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlPreviewMouseDoubleClickCommandBehaviorWithEventArgs
    public class SpreadsheetControlPreviewMouseDoubleClickCommandBehaviorWithEventArgs : SpreadsheetControlPreviewMouseDoubleClickCommandBehavior<MouseButtonEventArgs>
    {
        public SpreadsheetControlPreviewMouseDoubleClickCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlMouseDoubleClick
    // SpreadsheetControlMouseDoubleClickCommand<T, TBehavior>
    public class SpreadsheetControlMouseDoubleClickCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlMouseDoubleClickCommandBehavior<T>, new()
    { }

    // SpreadsheetControlMouseDoubleClickCommandBehavior<TReturn>
    public class SpreadsheetControlMouseDoubleClickCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public SpreadsheetControlMouseDoubleClickCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlMouseDoubleClickCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseDoubleClick += OnEventRaised;
        }
    }

    // SpreadsheetControlMouseDoubleClickCommand
    public class SpreadsheetControlMouseDoubleClickCommand : SpreadsheetControlCommandBase<SpreadsheetControlMouseDoubleClickCommandBehavior>
    { }

    // SpreadsheetControlMouseDoubleClickCommandBehavior
    public class SpreadsheetControlMouseDoubleClickCommandBehavior : SpreadsheetControlMouseDoubleClickCommandBehavior<object>
    { }

    // SpreadsheetControlMouseDoubleClickCommandWithEventArgs	
    public class SpreadsheetControlMouseDoubleClickCommandWithEventArgs : SpreadsheetControlMouseDoubleClickCommand<MouseButtonEventArgs, SpreadsheetControlMouseDoubleClickCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlMouseDoubleClickCommandBehaviorWithEventArgs
    public class SpreadsheetControlMouseDoubleClickCommandBehaviorWithEventArgs : SpreadsheetControlMouseDoubleClickCommandBehavior<MouseButtonEventArgs>
    {
        public SpreadsheetControlMouseDoubleClickCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlTargetUpdated
    // SpreadsheetControlTargetUpdatedCommand<T, TBehavior>
    public class SpreadsheetControlTargetUpdatedCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlTargetUpdatedCommandBehavior<T>, new()
    { }

    // SpreadsheetControlTargetUpdatedCommandBehavior<TReturn>
    public class SpreadsheetControlTargetUpdatedCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, DataTransferEventArgs>
    {
        public SpreadsheetControlTargetUpdatedCommandBehavior(Func<object, DataTransferEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlTargetUpdatedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.TargetUpdated += OnEventRaised;
        }
    }

    // SpreadsheetControlTargetUpdatedCommand
    public class SpreadsheetControlTargetUpdatedCommand : SpreadsheetControlCommandBase<SpreadsheetControlTargetUpdatedCommandBehavior>
    { }

    // SpreadsheetControlTargetUpdatedCommandBehavior
    public class SpreadsheetControlTargetUpdatedCommandBehavior : SpreadsheetControlTargetUpdatedCommandBehavior<object>
    { }

    // SpreadsheetControlTargetUpdatedCommandWithEventArgs	
    public class SpreadsheetControlTargetUpdatedCommandWithEventArgs : SpreadsheetControlTargetUpdatedCommand<DataTransferEventArgs, SpreadsheetControlTargetUpdatedCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlTargetUpdatedCommandBehaviorWithEventArgs
    public class SpreadsheetControlTargetUpdatedCommandBehaviorWithEventArgs : SpreadsheetControlTargetUpdatedCommandBehavior<DataTransferEventArgs>
    {
        public SpreadsheetControlTargetUpdatedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlSourceUpdated
    // SpreadsheetControlSourceUpdatedCommand<T, TBehavior>
    public class SpreadsheetControlSourceUpdatedCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlSourceUpdatedCommandBehavior<T>, new()
    { }

    // SpreadsheetControlSourceUpdatedCommandBehavior<TReturn>
    public class SpreadsheetControlSourceUpdatedCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, DataTransferEventArgs>
    {
        public SpreadsheetControlSourceUpdatedCommandBehavior(Func<object, DataTransferEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlSourceUpdatedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.SourceUpdated += OnEventRaised;
        }
    }

    // SpreadsheetControlSourceUpdatedCommand
    public class SpreadsheetControlSourceUpdatedCommand : SpreadsheetControlCommandBase<SpreadsheetControlSourceUpdatedCommandBehavior>
    { }

    // SpreadsheetControlSourceUpdatedCommandBehavior
    public class SpreadsheetControlSourceUpdatedCommandBehavior : SpreadsheetControlSourceUpdatedCommandBehavior<object>
    { }

    // SpreadsheetControlSourceUpdatedCommandWithEventArgs	
    public class SpreadsheetControlSourceUpdatedCommandWithEventArgs : SpreadsheetControlSourceUpdatedCommand<DataTransferEventArgs, SpreadsheetControlSourceUpdatedCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlSourceUpdatedCommandBehaviorWithEventArgs
    public class SpreadsheetControlSourceUpdatedCommandBehaviorWithEventArgs : SpreadsheetControlSourceUpdatedCommandBehavior<DataTransferEventArgs>
    {
        public SpreadsheetControlSourceUpdatedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlRequestBringIntoView
    // SpreadsheetControlRequestBringIntoViewCommand<T, TBehavior>
    public class SpreadsheetControlRequestBringIntoViewCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlRequestBringIntoViewCommandBehavior<T>, new()
    { }

    // SpreadsheetControlRequestBringIntoViewCommandBehavior<TReturn>
    public class SpreadsheetControlRequestBringIntoViewCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, RequestBringIntoViewEventArgs>
    {
        public SpreadsheetControlRequestBringIntoViewCommandBehavior(Func<object, RequestBringIntoViewEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlRequestBringIntoViewCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.RequestBringIntoView += OnEventRaised;
        }
    }

    // SpreadsheetControlRequestBringIntoViewCommand
    public class SpreadsheetControlRequestBringIntoViewCommand : SpreadsheetControlCommandBase<SpreadsheetControlRequestBringIntoViewCommandBehavior>
    { }

    // SpreadsheetControlRequestBringIntoViewCommandBehavior
    public class SpreadsheetControlRequestBringIntoViewCommandBehavior : SpreadsheetControlRequestBringIntoViewCommandBehavior<object>
    { }

    // SpreadsheetControlRequestBringIntoViewCommandWithEventArgs	
    public class SpreadsheetControlRequestBringIntoViewCommandWithEventArgs : SpreadsheetControlRequestBringIntoViewCommand<RequestBringIntoViewEventArgs, SpreadsheetControlRequestBringIntoViewCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlRequestBringIntoViewCommandBehaviorWithEventArgs
    public class SpreadsheetControlRequestBringIntoViewCommandBehaviorWithEventArgs : SpreadsheetControlRequestBringIntoViewCommandBehavior<RequestBringIntoViewEventArgs>
    {
        public SpreadsheetControlRequestBringIntoViewCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlInitialized
    // SpreadsheetControlInitializedCommand<T, TBehavior>
    public class SpreadsheetControlInitializedCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlInitializedCommandBehavior<T>, new()
    { }

    // SpreadsheetControlInitializedCommandBehavior<TReturn>
    public class SpreadsheetControlInitializedCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, EventArgs>
    {
        public SpreadsheetControlInitializedCommandBehavior(Func<object, EventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlInitializedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Initialized += OnEventRaised;
        }
    }

    // SpreadsheetControlInitializedCommand
    public class SpreadsheetControlInitializedCommand : SpreadsheetControlCommandBase<SpreadsheetControlInitializedCommandBehavior>
    { }

    // SpreadsheetControlInitializedCommandBehavior
    public class SpreadsheetControlInitializedCommandBehavior : SpreadsheetControlInitializedCommandBehavior<object>
    { }

    // SpreadsheetControlInitializedCommandWithEventArgs	
    public class SpreadsheetControlInitializedCommandWithEventArgs : SpreadsheetControlInitializedCommand<EventArgs, SpreadsheetControlInitializedCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlInitializedCommandBehaviorWithEventArgs
    public class SpreadsheetControlInitializedCommandBehaviorWithEventArgs : SpreadsheetControlInitializedCommandBehavior<EventArgs>
    {
        public SpreadsheetControlInitializedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlToolTipOpening
    // SpreadsheetControlToolTipOpeningCommand<T, TBehavior>
    public class SpreadsheetControlToolTipOpeningCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlToolTipOpeningCommandBehavior<T>, new()
    { }

    // SpreadsheetControlToolTipOpeningCommandBehavior<TReturn>
    public class SpreadsheetControlToolTipOpeningCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, ToolTipEventArgs>
    {
        public SpreadsheetControlToolTipOpeningCommandBehavior(Func<object, ToolTipEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlToolTipOpeningCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ToolTipOpening += OnEventRaised;
        }
    }

    // SpreadsheetControlToolTipOpeningCommand
    public class SpreadsheetControlToolTipOpeningCommand : SpreadsheetControlCommandBase<SpreadsheetControlToolTipOpeningCommandBehavior>
    { }

    // SpreadsheetControlToolTipOpeningCommandBehavior
    public class SpreadsheetControlToolTipOpeningCommandBehavior : SpreadsheetControlToolTipOpeningCommandBehavior<object>
    { }

    // SpreadsheetControlToolTipOpeningCommandWithEventArgs	
    public class SpreadsheetControlToolTipOpeningCommandWithEventArgs : SpreadsheetControlToolTipOpeningCommand<ToolTipEventArgs, SpreadsheetControlToolTipOpeningCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlToolTipOpeningCommandBehaviorWithEventArgs
    public class SpreadsheetControlToolTipOpeningCommandBehaviorWithEventArgs : SpreadsheetControlToolTipOpeningCommandBehavior<ToolTipEventArgs>
    {
        public SpreadsheetControlToolTipOpeningCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlToolTipClosing
    // SpreadsheetControlToolTipClosingCommand<T, TBehavior>
    public class SpreadsheetControlToolTipClosingCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlToolTipClosingCommandBehavior<T>, new()
    { }

    // SpreadsheetControlToolTipClosingCommandBehavior<TReturn>
    public class SpreadsheetControlToolTipClosingCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, ToolTipEventArgs>
    {
        public SpreadsheetControlToolTipClosingCommandBehavior(Func<object, ToolTipEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlToolTipClosingCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ToolTipClosing += OnEventRaised;
        }
    }

    // SpreadsheetControlToolTipClosingCommand
    public class SpreadsheetControlToolTipClosingCommand : SpreadsheetControlCommandBase<SpreadsheetControlToolTipClosingCommandBehavior>
    { }

    // SpreadsheetControlToolTipClosingCommandBehavior
    public class SpreadsheetControlToolTipClosingCommandBehavior : SpreadsheetControlToolTipClosingCommandBehavior<object>
    { }

    // SpreadsheetControlToolTipClosingCommandWithEventArgs	
    public class SpreadsheetControlToolTipClosingCommandWithEventArgs : SpreadsheetControlToolTipClosingCommand<ToolTipEventArgs, SpreadsheetControlToolTipClosingCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlToolTipClosingCommandBehaviorWithEventArgs
    public class SpreadsheetControlToolTipClosingCommandBehaviorWithEventArgs : SpreadsheetControlToolTipClosingCommandBehavior<ToolTipEventArgs>
    {
        public SpreadsheetControlToolTipClosingCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlContextMenuOpening
    // SpreadsheetControlContextMenuOpeningCommand<T, TBehavior>
    public class SpreadsheetControlContextMenuOpeningCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlContextMenuOpeningCommandBehavior<T>, new()
    { }

    // SpreadsheetControlContextMenuOpeningCommandBehavior<TReturn>
    public class SpreadsheetControlContextMenuOpeningCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, ContextMenuEventArgs>
    {
        public SpreadsheetControlContextMenuOpeningCommandBehavior(Func<object, ContextMenuEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlContextMenuOpeningCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ContextMenuOpening += OnEventRaised;
        }
    }

    // SpreadsheetControlContextMenuOpeningCommand
    public class SpreadsheetControlContextMenuOpeningCommand : SpreadsheetControlCommandBase<SpreadsheetControlContextMenuOpeningCommandBehavior>
    { }

    // SpreadsheetControlContextMenuOpeningCommandBehavior
    public class SpreadsheetControlContextMenuOpeningCommandBehavior : SpreadsheetControlContextMenuOpeningCommandBehavior<object>
    { }

    // SpreadsheetControlContextMenuOpeningCommandWithEventArgs	
    public class SpreadsheetControlContextMenuOpeningCommandWithEventArgs : SpreadsheetControlContextMenuOpeningCommand<ContextMenuEventArgs, SpreadsheetControlContextMenuOpeningCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlContextMenuOpeningCommandBehaviorWithEventArgs
    public class SpreadsheetControlContextMenuOpeningCommandBehaviorWithEventArgs : SpreadsheetControlContextMenuOpeningCommandBehavior<ContextMenuEventArgs>
    {
        public SpreadsheetControlContextMenuOpeningCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlContextMenuClosing
    // SpreadsheetControlContextMenuClosingCommand<T, TBehavior>
    public class SpreadsheetControlContextMenuClosingCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlContextMenuClosingCommandBehavior<T>, new()
    { }

    // SpreadsheetControlContextMenuClosingCommandBehavior<TReturn>
    public class SpreadsheetControlContextMenuClosingCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, ContextMenuEventArgs>
    {
        public SpreadsheetControlContextMenuClosingCommandBehavior(Func<object, ContextMenuEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlContextMenuClosingCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ContextMenuClosing += OnEventRaised;
        }
    }

    // SpreadsheetControlContextMenuClosingCommand
    public class SpreadsheetControlContextMenuClosingCommand : SpreadsheetControlCommandBase<SpreadsheetControlContextMenuClosingCommandBehavior>
    { }

    // SpreadsheetControlContextMenuClosingCommandBehavior
    public class SpreadsheetControlContextMenuClosingCommandBehavior : SpreadsheetControlContextMenuClosingCommandBehavior<object>
    { }

    // SpreadsheetControlContextMenuClosingCommandWithEventArgs	
    public class SpreadsheetControlContextMenuClosingCommandWithEventArgs : SpreadsheetControlContextMenuClosingCommand<ContextMenuEventArgs, SpreadsheetControlContextMenuClosingCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlContextMenuClosingCommandBehaviorWithEventArgs
    public class SpreadsheetControlContextMenuClosingCommandBehaviorWithEventArgs : SpreadsheetControlContextMenuClosingCommandBehavior<ContextMenuEventArgs>
    {
        public SpreadsheetControlContextMenuClosingCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlPreviewMouseDown
    // SpreadsheetControlPreviewMouseDownCommand<T, TBehavior>
    public class SpreadsheetControlPreviewMouseDownCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlPreviewMouseDownCommandBehavior<T>, new()
    { }

    // SpreadsheetControlPreviewMouseDownCommandBehavior<TReturn>
    public class SpreadsheetControlPreviewMouseDownCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public SpreadsheetControlPreviewMouseDownCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlPreviewMouseDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewMouseDown += OnEventRaised;
        }
    }

    // SpreadsheetControlPreviewMouseDownCommand
    public class SpreadsheetControlPreviewMouseDownCommand : SpreadsheetControlCommandBase<SpreadsheetControlPreviewMouseDownCommandBehavior>
    { }

    // SpreadsheetControlPreviewMouseDownCommandBehavior
    public class SpreadsheetControlPreviewMouseDownCommandBehavior : SpreadsheetControlPreviewMouseDownCommandBehavior<object>
    { }

    // SpreadsheetControlPreviewMouseDownCommandWithEventArgs	
    public class SpreadsheetControlPreviewMouseDownCommandWithEventArgs : SpreadsheetControlPreviewMouseDownCommand<MouseButtonEventArgs, SpreadsheetControlPreviewMouseDownCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlPreviewMouseDownCommandBehaviorWithEventArgs
    public class SpreadsheetControlPreviewMouseDownCommandBehaviorWithEventArgs : SpreadsheetControlPreviewMouseDownCommandBehavior<MouseButtonEventArgs>
    {
        public SpreadsheetControlPreviewMouseDownCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlMouseDown
    // SpreadsheetControlMouseDownCommand<T, TBehavior>
    public class SpreadsheetControlMouseDownCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlMouseDownCommandBehavior<T>, new()
    { }

    // SpreadsheetControlMouseDownCommandBehavior<TReturn>
    public class SpreadsheetControlMouseDownCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public SpreadsheetControlMouseDownCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlMouseDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseDown += OnEventRaised;
        }
    }

    // SpreadsheetControlMouseDownCommand
    public class SpreadsheetControlMouseDownCommand : SpreadsheetControlCommandBase<SpreadsheetControlMouseDownCommandBehavior>
    { }

    // SpreadsheetControlMouseDownCommandBehavior
    public class SpreadsheetControlMouseDownCommandBehavior : SpreadsheetControlMouseDownCommandBehavior<object>
    { }

    // SpreadsheetControlMouseDownCommandWithEventArgs	
    public class SpreadsheetControlMouseDownCommandWithEventArgs : SpreadsheetControlMouseDownCommand<MouseButtonEventArgs, SpreadsheetControlMouseDownCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlMouseDownCommandBehaviorWithEventArgs
    public class SpreadsheetControlMouseDownCommandBehaviorWithEventArgs : SpreadsheetControlMouseDownCommandBehavior<MouseButtonEventArgs>
    {
        public SpreadsheetControlMouseDownCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlPreviewMouseUp
    // SpreadsheetControlPreviewMouseUpCommand<T, TBehavior>
    public class SpreadsheetControlPreviewMouseUpCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlPreviewMouseUpCommandBehavior<T>, new()
    { }

    // SpreadsheetControlPreviewMouseUpCommandBehavior<TReturn>
    public class SpreadsheetControlPreviewMouseUpCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public SpreadsheetControlPreviewMouseUpCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlPreviewMouseUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewMouseUp += OnEventRaised;
        }
    }

    // SpreadsheetControlPreviewMouseUpCommand
    public class SpreadsheetControlPreviewMouseUpCommand : SpreadsheetControlCommandBase<SpreadsheetControlPreviewMouseUpCommandBehavior>
    { }

    // SpreadsheetControlPreviewMouseUpCommandBehavior
    public class SpreadsheetControlPreviewMouseUpCommandBehavior : SpreadsheetControlPreviewMouseUpCommandBehavior<object>
    { }

    // SpreadsheetControlPreviewMouseUpCommandWithEventArgs	
    public class SpreadsheetControlPreviewMouseUpCommandWithEventArgs : SpreadsheetControlPreviewMouseUpCommand<MouseButtonEventArgs, SpreadsheetControlPreviewMouseUpCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlPreviewMouseUpCommandBehaviorWithEventArgs
    public class SpreadsheetControlPreviewMouseUpCommandBehaviorWithEventArgs : SpreadsheetControlPreviewMouseUpCommandBehavior<MouseButtonEventArgs>
    {
        public SpreadsheetControlPreviewMouseUpCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlMouseUp
    // SpreadsheetControlMouseUpCommand<T, TBehavior>
    public class SpreadsheetControlMouseUpCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlMouseUpCommandBehavior<T>, new()
    { }

    // SpreadsheetControlMouseUpCommandBehavior<TReturn>
    public class SpreadsheetControlMouseUpCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public SpreadsheetControlMouseUpCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlMouseUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseUp += OnEventRaised;
        }
    }

    // SpreadsheetControlMouseUpCommand
    public class SpreadsheetControlMouseUpCommand : SpreadsheetControlCommandBase<SpreadsheetControlMouseUpCommandBehavior>
    { }

    // SpreadsheetControlMouseUpCommandBehavior
    public class SpreadsheetControlMouseUpCommandBehavior : SpreadsheetControlMouseUpCommandBehavior<object>
    { }

    // SpreadsheetControlMouseUpCommandWithEventArgs	
    public class SpreadsheetControlMouseUpCommandWithEventArgs : SpreadsheetControlMouseUpCommand<MouseButtonEventArgs, SpreadsheetControlMouseUpCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlMouseUpCommandBehaviorWithEventArgs
    public class SpreadsheetControlMouseUpCommandBehaviorWithEventArgs : SpreadsheetControlMouseUpCommandBehavior<MouseButtonEventArgs>
    {
        public SpreadsheetControlMouseUpCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlPreviewMouseLeftButtonDown
    // SpreadsheetControlPreviewMouseLeftButtonDownCommand<T, TBehavior>
    public class SpreadsheetControlPreviewMouseLeftButtonDownCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlPreviewMouseLeftButtonDownCommandBehavior<T>, new()
    { }

    // SpreadsheetControlPreviewMouseLeftButtonDownCommandBehavior<TReturn>
    public class SpreadsheetControlPreviewMouseLeftButtonDownCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public SpreadsheetControlPreviewMouseLeftButtonDownCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlPreviewMouseLeftButtonDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewMouseLeftButtonDown += OnEventRaised;
        }
    }

    // SpreadsheetControlPreviewMouseLeftButtonDownCommand
    public class SpreadsheetControlPreviewMouseLeftButtonDownCommand : SpreadsheetControlCommandBase<SpreadsheetControlPreviewMouseLeftButtonDownCommandBehavior>
    { }

    // SpreadsheetControlPreviewMouseLeftButtonDownCommandBehavior
    public class SpreadsheetControlPreviewMouseLeftButtonDownCommandBehavior : SpreadsheetControlPreviewMouseLeftButtonDownCommandBehavior<object>
    { }

    // SpreadsheetControlPreviewMouseLeftButtonDownCommandWithEventArgs	
    public class SpreadsheetControlPreviewMouseLeftButtonDownCommandWithEventArgs : SpreadsheetControlPreviewMouseLeftButtonDownCommand<MouseButtonEventArgs, SpreadsheetControlPreviewMouseLeftButtonDownCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlPreviewMouseLeftButtonDownCommandBehaviorWithEventArgs
    public class SpreadsheetControlPreviewMouseLeftButtonDownCommandBehaviorWithEventArgs : SpreadsheetControlPreviewMouseLeftButtonDownCommandBehavior<MouseButtonEventArgs>
    {
        public SpreadsheetControlPreviewMouseLeftButtonDownCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlPreviewMouseLeftButtonUp
    // SpreadsheetControlPreviewMouseLeftButtonUpCommand<T, TBehavior>
    public class SpreadsheetControlPreviewMouseLeftButtonUpCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlPreviewMouseLeftButtonUpCommandBehavior<T>, new()
    { }

    // SpreadsheetControlPreviewMouseLeftButtonUpCommandBehavior<TReturn>
    public class SpreadsheetControlPreviewMouseLeftButtonUpCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public SpreadsheetControlPreviewMouseLeftButtonUpCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlPreviewMouseLeftButtonUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewMouseLeftButtonUp += OnEventRaised;
        }
    }

    // SpreadsheetControlPreviewMouseLeftButtonUpCommand
    public class SpreadsheetControlPreviewMouseLeftButtonUpCommand : SpreadsheetControlCommandBase<SpreadsheetControlPreviewMouseLeftButtonUpCommandBehavior>
    { }

    // SpreadsheetControlPreviewMouseLeftButtonUpCommandBehavior
    public class SpreadsheetControlPreviewMouseLeftButtonUpCommandBehavior : SpreadsheetControlPreviewMouseLeftButtonUpCommandBehavior<object>
    { }

    // SpreadsheetControlPreviewMouseLeftButtonUpCommandWithEventArgs	
    public class SpreadsheetControlPreviewMouseLeftButtonUpCommandWithEventArgs : SpreadsheetControlPreviewMouseLeftButtonUpCommand<MouseButtonEventArgs, SpreadsheetControlPreviewMouseLeftButtonUpCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlPreviewMouseLeftButtonUpCommandBehaviorWithEventArgs
    public class SpreadsheetControlPreviewMouseLeftButtonUpCommandBehaviorWithEventArgs : SpreadsheetControlPreviewMouseLeftButtonUpCommandBehavior<MouseButtonEventArgs>
    {
        public SpreadsheetControlPreviewMouseLeftButtonUpCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlPreviewMouseRightButtonDown
    // SpreadsheetControlPreviewMouseRightButtonDownCommand<T, TBehavior>
    public class SpreadsheetControlPreviewMouseRightButtonDownCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlPreviewMouseRightButtonDownCommandBehavior<T>, new()
    { }

    // SpreadsheetControlPreviewMouseRightButtonDownCommandBehavior<TReturn>
    public class SpreadsheetControlPreviewMouseRightButtonDownCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public SpreadsheetControlPreviewMouseRightButtonDownCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlPreviewMouseRightButtonDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewMouseRightButtonDown += OnEventRaised;
        }
    }

    // SpreadsheetControlPreviewMouseRightButtonDownCommand
    public class SpreadsheetControlPreviewMouseRightButtonDownCommand : SpreadsheetControlCommandBase<SpreadsheetControlPreviewMouseRightButtonDownCommandBehavior>
    { }

    // SpreadsheetControlPreviewMouseRightButtonDownCommandBehavior
    public class SpreadsheetControlPreviewMouseRightButtonDownCommandBehavior : SpreadsheetControlPreviewMouseRightButtonDownCommandBehavior<object>
    { }

    // SpreadsheetControlPreviewMouseRightButtonDownCommandWithEventArgs	
    public class SpreadsheetControlPreviewMouseRightButtonDownCommandWithEventArgs : SpreadsheetControlPreviewMouseRightButtonDownCommand<MouseButtonEventArgs, SpreadsheetControlPreviewMouseRightButtonDownCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlPreviewMouseRightButtonDownCommandBehaviorWithEventArgs
    public class SpreadsheetControlPreviewMouseRightButtonDownCommandBehaviorWithEventArgs : SpreadsheetControlPreviewMouseRightButtonDownCommandBehavior<MouseButtonEventArgs>
    {
        public SpreadsheetControlPreviewMouseRightButtonDownCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlPreviewMouseRightButtonUp
    // SpreadsheetControlPreviewMouseRightButtonUpCommand<T, TBehavior>
    public class SpreadsheetControlPreviewMouseRightButtonUpCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlPreviewMouseRightButtonUpCommandBehavior<T>, new()
    { }

    // SpreadsheetControlPreviewMouseRightButtonUpCommandBehavior<TReturn>
    public class SpreadsheetControlPreviewMouseRightButtonUpCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public SpreadsheetControlPreviewMouseRightButtonUpCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlPreviewMouseRightButtonUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewMouseRightButtonUp += OnEventRaised;
        }
    }

    // SpreadsheetControlPreviewMouseRightButtonUpCommand
    public class SpreadsheetControlPreviewMouseRightButtonUpCommand : SpreadsheetControlCommandBase<SpreadsheetControlPreviewMouseRightButtonUpCommandBehavior>
    { }

    // SpreadsheetControlPreviewMouseRightButtonUpCommandBehavior
    public class SpreadsheetControlPreviewMouseRightButtonUpCommandBehavior : SpreadsheetControlPreviewMouseRightButtonUpCommandBehavior<object>
    { }

    // SpreadsheetControlPreviewMouseRightButtonUpCommandWithEventArgs	
    public class SpreadsheetControlPreviewMouseRightButtonUpCommandWithEventArgs : SpreadsheetControlPreviewMouseRightButtonUpCommand<MouseButtonEventArgs, SpreadsheetControlPreviewMouseRightButtonUpCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlPreviewMouseRightButtonUpCommandBehaviorWithEventArgs
    public class SpreadsheetControlPreviewMouseRightButtonUpCommandBehaviorWithEventArgs : SpreadsheetControlPreviewMouseRightButtonUpCommandBehavior<MouseButtonEventArgs>
    {
        public SpreadsheetControlPreviewMouseRightButtonUpCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlPreviewMouseMove
    // SpreadsheetControlPreviewMouseMoveCommand<T, TBehavior>
    public class SpreadsheetControlPreviewMouseMoveCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlPreviewMouseMoveCommandBehavior<T>, new()
    { }

    // SpreadsheetControlPreviewMouseMoveCommandBehavior<TReturn>
    public class SpreadsheetControlPreviewMouseMoveCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, MouseEventArgs>
    {
        public SpreadsheetControlPreviewMouseMoveCommandBehavior(Func<object, MouseEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlPreviewMouseMoveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewMouseMove += OnEventRaised;
        }
    }

    // SpreadsheetControlPreviewMouseMoveCommand
    public class SpreadsheetControlPreviewMouseMoveCommand : SpreadsheetControlCommandBase<SpreadsheetControlPreviewMouseMoveCommandBehavior>
    { }

    // SpreadsheetControlPreviewMouseMoveCommandBehavior
    public class SpreadsheetControlPreviewMouseMoveCommandBehavior : SpreadsheetControlPreviewMouseMoveCommandBehavior<object>
    { }

    // SpreadsheetControlPreviewMouseMoveCommandWithEventArgs	
    public class SpreadsheetControlPreviewMouseMoveCommandWithEventArgs : SpreadsheetControlPreviewMouseMoveCommand<MouseEventArgs, SpreadsheetControlPreviewMouseMoveCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlPreviewMouseMoveCommandBehaviorWithEventArgs
    public class SpreadsheetControlPreviewMouseMoveCommandBehaviorWithEventArgs : SpreadsheetControlPreviewMouseMoveCommandBehavior<MouseEventArgs>
    {
        public SpreadsheetControlPreviewMouseMoveCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlPreviewMouseWheel
    // SpreadsheetControlPreviewMouseWheelCommand<T, TBehavior>
    public class SpreadsheetControlPreviewMouseWheelCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlPreviewMouseWheelCommandBehavior<T>, new()
    { }

    // SpreadsheetControlPreviewMouseWheelCommandBehavior<TReturn>
    public class SpreadsheetControlPreviewMouseWheelCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, MouseWheelEventArgs>
    {
        public SpreadsheetControlPreviewMouseWheelCommandBehavior(Func<object, MouseWheelEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlPreviewMouseWheelCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewMouseWheel += OnEventRaised;
        }
    }

    // SpreadsheetControlPreviewMouseWheelCommand
    public class SpreadsheetControlPreviewMouseWheelCommand : SpreadsheetControlCommandBase<SpreadsheetControlPreviewMouseWheelCommandBehavior>
    { }

    // SpreadsheetControlPreviewMouseWheelCommandBehavior
    public class SpreadsheetControlPreviewMouseWheelCommandBehavior : SpreadsheetControlPreviewMouseWheelCommandBehavior<object>
    { }

    // SpreadsheetControlPreviewMouseWheelCommandWithEventArgs	
    public class SpreadsheetControlPreviewMouseWheelCommandWithEventArgs : SpreadsheetControlPreviewMouseWheelCommand<MouseWheelEventArgs, SpreadsheetControlPreviewMouseWheelCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlPreviewMouseWheelCommandBehaviorWithEventArgs
    public class SpreadsheetControlPreviewMouseWheelCommandBehaviorWithEventArgs : SpreadsheetControlPreviewMouseWheelCommandBehavior<MouseWheelEventArgs>
    {
        public SpreadsheetControlPreviewMouseWheelCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlGotMouseCapture
    // SpreadsheetControlGotMouseCaptureCommand<T, TBehavior>
    public class SpreadsheetControlGotMouseCaptureCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlGotMouseCaptureCommandBehavior<T>, new()
    { }

    // SpreadsheetControlGotMouseCaptureCommandBehavior<TReturn>
    public class SpreadsheetControlGotMouseCaptureCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, MouseEventArgs>
    {
        public SpreadsheetControlGotMouseCaptureCommandBehavior(Func<object, MouseEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlGotMouseCaptureCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.GotMouseCapture += OnEventRaised;
        }
    }

    // SpreadsheetControlGotMouseCaptureCommand
    public class SpreadsheetControlGotMouseCaptureCommand : SpreadsheetControlCommandBase<SpreadsheetControlGotMouseCaptureCommandBehavior>
    { }

    // SpreadsheetControlGotMouseCaptureCommandBehavior
    public class SpreadsheetControlGotMouseCaptureCommandBehavior : SpreadsheetControlGotMouseCaptureCommandBehavior<object>
    { }

    // SpreadsheetControlGotMouseCaptureCommandWithEventArgs	
    public class SpreadsheetControlGotMouseCaptureCommandWithEventArgs : SpreadsheetControlGotMouseCaptureCommand<MouseEventArgs, SpreadsheetControlGotMouseCaptureCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlGotMouseCaptureCommandBehaviorWithEventArgs
    public class SpreadsheetControlGotMouseCaptureCommandBehaviorWithEventArgs : SpreadsheetControlGotMouseCaptureCommandBehavior<MouseEventArgs>
    {
        public SpreadsheetControlGotMouseCaptureCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlQueryCursor
    // SpreadsheetControlQueryCursorCommand<T, TBehavior>
    public class SpreadsheetControlQueryCursorCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlQueryCursorCommandBehavior<T>, new()
    { }

    // SpreadsheetControlQueryCursorCommandBehavior<TReturn>
    public class SpreadsheetControlQueryCursorCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, QueryCursorEventArgs>
    {
        public SpreadsheetControlQueryCursorCommandBehavior(Func<object, QueryCursorEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlQueryCursorCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.QueryCursor += OnEventRaised;
        }
    }

    // SpreadsheetControlQueryCursorCommand
    public class SpreadsheetControlQueryCursorCommand : SpreadsheetControlCommandBase<SpreadsheetControlQueryCursorCommandBehavior>
    { }

    // SpreadsheetControlQueryCursorCommandBehavior
    public class SpreadsheetControlQueryCursorCommandBehavior : SpreadsheetControlQueryCursorCommandBehavior<object>
    { }

    // SpreadsheetControlQueryCursorCommandWithEventArgs	
    public class SpreadsheetControlQueryCursorCommandWithEventArgs : SpreadsheetControlQueryCursorCommand<QueryCursorEventArgs, SpreadsheetControlQueryCursorCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlQueryCursorCommandBehaviorWithEventArgs
    public class SpreadsheetControlQueryCursorCommandBehaviorWithEventArgs : SpreadsheetControlQueryCursorCommandBehavior<QueryCursorEventArgs>
    {
        public SpreadsheetControlQueryCursorCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlPreviewStylusDown
    // SpreadsheetControlPreviewStylusDownCommand<T, TBehavior>
    public class SpreadsheetControlPreviewStylusDownCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlPreviewStylusDownCommandBehavior<T>, new()
    { }

    // SpreadsheetControlPreviewStylusDownCommandBehavior<TReturn>
    public class SpreadsheetControlPreviewStylusDownCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, StylusDownEventArgs>
    {
        public SpreadsheetControlPreviewStylusDownCommandBehavior(Func<object, StylusDownEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlPreviewStylusDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewStylusDown += OnEventRaised;
        }
    }

    // SpreadsheetControlPreviewStylusDownCommand
    public class SpreadsheetControlPreviewStylusDownCommand : SpreadsheetControlCommandBase<SpreadsheetControlPreviewStylusDownCommandBehavior>
    { }

    // SpreadsheetControlPreviewStylusDownCommandBehavior
    public class SpreadsheetControlPreviewStylusDownCommandBehavior : SpreadsheetControlPreviewStylusDownCommandBehavior<object>
    { }

    // SpreadsheetControlPreviewStylusDownCommandWithEventArgs	
    public class SpreadsheetControlPreviewStylusDownCommandWithEventArgs : SpreadsheetControlPreviewStylusDownCommand<StylusDownEventArgs, SpreadsheetControlPreviewStylusDownCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlPreviewStylusDownCommandBehaviorWithEventArgs
    public class SpreadsheetControlPreviewStylusDownCommandBehaviorWithEventArgs : SpreadsheetControlPreviewStylusDownCommandBehavior<StylusDownEventArgs>
    {
        public SpreadsheetControlPreviewStylusDownCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlStylusDown
    // SpreadsheetControlStylusDownCommand<T, TBehavior>
    public class SpreadsheetControlStylusDownCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlStylusDownCommandBehavior<T>, new()
    { }

    // SpreadsheetControlStylusDownCommandBehavior<TReturn>
    public class SpreadsheetControlStylusDownCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, StylusDownEventArgs>
    {
        public SpreadsheetControlStylusDownCommandBehavior(Func<object, StylusDownEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlStylusDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusDown += OnEventRaised;
        }
    }

    // SpreadsheetControlStylusDownCommand
    public class SpreadsheetControlStylusDownCommand : SpreadsheetControlCommandBase<SpreadsheetControlStylusDownCommandBehavior>
    { }

    // SpreadsheetControlStylusDownCommandBehavior
    public class SpreadsheetControlStylusDownCommandBehavior : SpreadsheetControlStylusDownCommandBehavior<object>
    { }

    // SpreadsheetControlStylusDownCommandWithEventArgs	
    public class SpreadsheetControlStylusDownCommandWithEventArgs : SpreadsheetControlStylusDownCommand<StylusDownEventArgs, SpreadsheetControlStylusDownCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlStylusDownCommandBehaviorWithEventArgs
    public class SpreadsheetControlStylusDownCommandBehaviorWithEventArgs : SpreadsheetControlStylusDownCommandBehavior<StylusDownEventArgs>
    {
        public SpreadsheetControlStylusDownCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlPreviewStylusUp
    // SpreadsheetControlPreviewStylusUpCommand<T, TBehavior>
    public class SpreadsheetControlPreviewStylusUpCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlPreviewStylusUpCommandBehavior<T>, new()
    { }

    // SpreadsheetControlPreviewStylusUpCommandBehavior<TReturn>
    public class SpreadsheetControlPreviewStylusUpCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public SpreadsheetControlPreviewStylusUpCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlPreviewStylusUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewStylusUp += OnEventRaised;
        }
    }

    // SpreadsheetControlPreviewStylusUpCommand
    public class SpreadsheetControlPreviewStylusUpCommand : SpreadsheetControlCommandBase<SpreadsheetControlPreviewStylusUpCommandBehavior>
    { }

    // SpreadsheetControlPreviewStylusUpCommandBehavior
    public class SpreadsheetControlPreviewStylusUpCommandBehavior : SpreadsheetControlPreviewStylusUpCommandBehavior<object>
    { }

    // SpreadsheetControlPreviewStylusUpCommandWithEventArgs	
    public class SpreadsheetControlPreviewStylusUpCommandWithEventArgs : SpreadsheetControlPreviewStylusUpCommand<StylusEventArgs, SpreadsheetControlPreviewStylusUpCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlPreviewStylusUpCommandBehaviorWithEventArgs
    public class SpreadsheetControlPreviewStylusUpCommandBehaviorWithEventArgs : SpreadsheetControlPreviewStylusUpCommandBehavior<StylusEventArgs>
    {
        public SpreadsheetControlPreviewStylusUpCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlStylusUp
    // SpreadsheetControlStylusUpCommand<T, TBehavior>
    public class SpreadsheetControlStylusUpCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlStylusUpCommandBehavior<T>, new()
    { }

    // SpreadsheetControlStylusUpCommandBehavior<TReturn>
    public class SpreadsheetControlStylusUpCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public SpreadsheetControlStylusUpCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlStylusUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusUp += OnEventRaised;
        }
    }

    // SpreadsheetControlStylusUpCommand
    public class SpreadsheetControlStylusUpCommand : SpreadsheetControlCommandBase<SpreadsheetControlStylusUpCommandBehavior>
    { }

    // SpreadsheetControlStylusUpCommandBehavior
    public class SpreadsheetControlStylusUpCommandBehavior : SpreadsheetControlStylusUpCommandBehavior<object>
    { }

    // SpreadsheetControlStylusUpCommandWithEventArgs	
    public class SpreadsheetControlStylusUpCommandWithEventArgs : SpreadsheetControlStylusUpCommand<StylusEventArgs, SpreadsheetControlStylusUpCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlStylusUpCommandBehaviorWithEventArgs
    public class SpreadsheetControlStylusUpCommandBehaviorWithEventArgs : SpreadsheetControlStylusUpCommandBehavior<StylusEventArgs>
    {
        public SpreadsheetControlStylusUpCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlPreviewStylusMove
    // SpreadsheetControlPreviewStylusMoveCommand<T, TBehavior>
    public class SpreadsheetControlPreviewStylusMoveCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlPreviewStylusMoveCommandBehavior<T>, new()
    { }

    // SpreadsheetControlPreviewStylusMoveCommandBehavior<TReturn>
    public class SpreadsheetControlPreviewStylusMoveCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public SpreadsheetControlPreviewStylusMoveCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlPreviewStylusMoveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewStylusMove += OnEventRaised;
        }
    }

    // SpreadsheetControlPreviewStylusMoveCommand
    public class SpreadsheetControlPreviewStylusMoveCommand : SpreadsheetControlCommandBase<SpreadsheetControlPreviewStylusMoveCommandBehavior>
    { }

    // SpreadsheetControlPreviewStylusMoveCommandBehavior
    public class SpreadsheetControlPreviewStylusMoveCommandBehavior : SpreadsheetControlPreviewStylusMoveCommandBehavior<object>
    { }

    // SpreadsheetControlPreviewStylusMoveCommandWithEventArgs	
    public class SpreadsheetControlPreviewStylusMoveCommandWithEventArgs : SpreadsheetControlPreviewStylusMoveCommand<StylusEventArgs, SpreadsheetControlPreviewStylusMoveCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlPreviewStylusMoveCommandBehaviorWithEventArgs
    public class SpreadsheetControlPreviewStylusMoveCommandBehaviorWithEventArgs : SpreadsheetControlPreviewStylusMoveCommandBehavior<StylusEventArgs>
    {
        public SpreadsheetControlPreviewStylusMoveCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlStylusMove
    // SpreadsheetControlStylusMoveCommand<T, TBehavior>
    public class SpreadsheetControlStylusMoveCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlStylusMoveCommandBehavior<T>, new()
    { }

    // SpreadsheetControlStylusMoveCommandBehavior<TReturn>
    public class SpreadsheetControlStylusMoveCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public SpreadsheetControlStylusMoveCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlStylusMoveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusMove += OnEventRaised;
        }
    }

    // SpreadsheetControlStylusMoveCommand
    public class SpreadsheetControlStylusMoveCommand : SpreadsheetControlCommandBase<SpreadsheetControlStylusMoveCommandBehavior>
    { }

    // SpreadsheetControlStylusMoveCommandBehavior
    public class SpreadsheetControlStylusMoveCommandBehavior : SpreadsheetControlStylusMoveCommandBehavior<object>
    { }

    // SpreadsheetControlStylusMoveCommandWithEventArgs	
    public class SpreadsheetControlStylusMoveCommandWithEventArgs : SpreadsheetControlStylusMoveCommand<StylusEventArgs, SpreadsheetControlStylusMoveCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlStylusMoveCommandBehaviorWithEventArgs
    public class SpreadsheetControlStylusMoveCommandBehaviorWithEventArgs : SpreadsheetControlStylusMoveCommandBehavior<StylusEventArgs>
    {
        public SpreadsheetControlStylusMoveCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlPreviewStylusInAirMove
    // SpreadsheetControlPreviewStylusInAirMoveCommand<T, TBehavior>
    public class SpreadsheetControlPreviewStylusInAirMoveCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlPreviewStylusInAirMoveCommandBehavior<T>, new()
    { }

    // SpreadsheetControlPreviewStylusInAirMoveCommandBehavior<TReturn>
    public class SpreadsheetControlPreviewStylusInAirMoveCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public SpreadsheetControlPreviewStylusInAirMoveCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlPreviewStylusInAirMoveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewStylusInAirMove += OnEventRaised;
        }
    }

    // SpreadsheetControlPreviewStylusInAirMoveCommand
    public class SpreadsheetControlPreviewStylusInAirMoveCommand : SpreadsheetControlCommandBase<SpreadsheetControlPreviewStylusInAirMoveCommandBehavior>
    { }

    // SpreadsheetControlPreviewStylusInAirMoveCommandBehavior
    public class SpreadsheetControlPreviewStylusInAirMoveCommandBehavior : SpreadsheetControlPreviewStylusInAirMoveCommandBehavior<object>
    { }

    // SpreadsheetControlPreviewStylusInAirMoveCommandWithEventArgs	
    public class SpreadsheetControlPreviewStylusInAirMoveCommandWithEventArgs : SpreadsheetControlPreviewStylusInAirMoveCommand<StylusEventArgs, SpreadsheetControlPreviewStylusInAirMoveCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlPreviewStylusInAirMoveCommandBehaviorWithEventArgs
    public class SpreadsheetControlPreviewStylusInAirMoveCommandBehaviorWithEventArgs : SpreadsheetControlPreviewStylusInAirMoveCommandBehavior<StylusEventArgs>
    {
        public SpreadsheetControlPreviewStylusInAirMoveCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlStylusInAirMove
    // SpreadsheetControlStylusInAirMoveCommand<T, TBehavior>
    public class SpreadsheetControlStylusInAirMoveCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlStylusInAirMoveCommandBehavior<T>, new()
    { }

    // SpreadsheetControlStylusInAirMoveCommandBehavior<TReturn>
    public class SpreadsheetControlStylusInAirMoveCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public SpreadsheetControlStylusInAirMoveCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlStylusInAirMoveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusInAirMove += OnEventRaised;
        }
    }

    // SpreadsheetControlStylusInAirMoveCommand
    public class SpreadsheetControlStylusInAirMoveCommand : SpreadsheetControlCommandBase<SpreadsheetControlStylusInAirMoveCommandBehavior>
    { }

    // SpreadsheetControlStylusInAirMoveCommandBehavior
    public class SpreadsheetControlStylusInAirMoveCommandBehavior : SpreadsheetControlStylusInAirMoveCommandBehavior<object>
    { }

    // SpreadsheetControlStylusInAirMoveCommandWithEventArgs	
    public class SpreadsheetControlStylusInAirMoveCommandWithEventArgs : SpreadsheetControlStylusInAirMoveCommand<StylusEventArgs, SpreadsheetControlStylusInAirMoveCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlStylusInAirMoveCommandBehaviorWithEventArgs
    public class SpreadsheetControlStylusInAirMoveCommandBehaviorWithEventArgs : SpreadsheetControlStylusInAirMoveCommandBehavior<StylusEventArgs>
    {
        public SpreadsheetControlStylusInAirMoveCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlStylusEnter
    // SpreadsheetControlStylusEnterCommand<T, TBehavior>
    public class SpreadsheetControlStylusEnterCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlStylusEnterCommandBehavior<T>, new()
    { }

    // SpreadsheetControlStylusEnterCommandBehavior<TReturn>
    public class SpreadsheetControlStylusEnterCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public SpreadsheetControlStylusEnterCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlStylusEnterCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusEnter += OnEventRaised;
        }
    }

    // SpreadsheetControlStylusEnterCommand
    public class SpreadsheetControlStylusEnterCommand : SpreadsheetControlCommandBase<SpreadsheetControlStylusEnterCommandBehavior>
    { }

    // SpreadsheetControlStylusEnterCommandBehavior
    public class SpreadsheetControlStylusEnterCommandBehavior : SpreadsheetControlStylusEnterCommandBehavior<object>
    { }

    // SpreadsheetControlStylusEnterCommandWithEventArgs	
    public class SpreadsheetControlStylusEnterCommandWithEventArgs : SpreadsheetControlStylusEnterCommand<StylusEventArgs, SpreadsheetControlStylusEnterCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlStylusEnterCommandBehaviorWithEventArgs
    public class SpreadsheetControlStylusEnterCommandBehaviorWithEventArgs : SpreadsheetControlStylusEnterCommandBehavior<StylusEventArgs>
    {
        public SpreadsheetControlStylusEnterCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlStylusLeave
    // SpreadsheetControlStylusLeaveCommand<T, TBehavior>
    public class SpreadsheetControlStylusLeaveCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlStylusLeaveCommandBehavior<T>, new()
    { }

    // SpreadsheetControlStylusLeaveCommandBehavior<TReturn>
    public class SpreadsheetControlStylusLeaveCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public SpreadsheetControlStylusLeaveCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlStylusLeaveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusLeave += OnEventRaised;
        }
    }

    // SpreadsheetControlStylusLeaveCommand
    public class SpreadsheetControlStylusLeaveCommand : SpreadsheetControlCommandBase<SpreadsheetControlStylusLeaveCommandBehavior>
    { }

    // SpreadsheetControlStylusLeaveCommandBehavior
    public class SpreadsheetControlStylusLeaveCommandBehavior : SpreadsheetControlStylusLeaveCommandBehavior<object>
    { }

    // SpreadsheetControlStylusLeaveCommandWithEventArgs	
    public class SpreadsheetControlStylusLeaveCommandWithEventArgs : SpreadsheetControlStylusLeaveCommand<StylusEventArgs, SpreadsheetControlStylusLeaveCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlStylusLeaveCommandBehaviorWithEventArgs
    public class SpreadsheetControlStylusLeaveCommandBehaviorWithEventArgs : SpreadsheetControlStylusLeaveCommandBehavior<StylusEventArgs>
    {
        public SpreadsheetControlStylusLeaveCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlPreviewStylusInRange
    // SpreadsheetControlPreviewStylusInRangeCommand<T, TBehavior>
    public class SpreadsheetControlPreviewStylusInRangeCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlPreviewStylusInRangeCommandBehavior<T>, new()
    { }

    // SpreadsheetControlPreviewStylusInRangeCommandBehavior<TReturn>
    public class SpreadsheetControlPreviewStylusInRangeCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public SpreadsheetControlPreviewStylusInRangeCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlPreviewStylusInRangeCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewStylusInRange += OnEventRaised;
        }
    }

    // SpreadsheetControlPreviewStylusInRangeCommand
    public class SpreadsheetControlPreviewStylusInRangeCommand : SpreadsheetControlCommandBase<SpreadsheetControlPreviewStylusInRangeCommandBehavior>
    { }

    // SpreadsheetControlPreviewStylusInRangeCommandBehavior
    public class SpreadsheetControlPreviewStylusInRangeCommandBehavior : SpreadsheetControlPreviewStylusInRangeCommandBehavior<object>
    { }

    // SpreadsheetControlPreviewStylusInRangeCommandWithEventArgs	
    public class SpreadsheetControlPreviewStylusInRangeCommandWithEventArgs : SpreadsheetControlPreviewStylusInRangeCommand<StylusEventArgs, SpreadsheetControlPreviewStylusInRangeCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlPreviewStylusInRangeCommandBehaviorWithEventArgs
    public class SpreadsheetControlPreviewStylusInRangeCommandBehaviorWithEventArgs : SpreadsheetControlPreviewStylusInRangeCommandBehavior<StylusEventArgs>
    {
        public SpreadsheetControlPreviewStylusInRangeCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlStylusInRange
    // SpreadsheetControlStylusInRangeCommand<T, TBehavior>
    public class SpreadsheetControlStylusInRangeCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlStylusInRangeCommandBehavior<T>, new()
    { }

    // SpreadsheetControlStylusInRangeCommandBehavior<TReturn>
    public class SpreadsheetControlStylusInRangeCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public SpreadsheetControlStylusInRangeCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlStylusInRangeCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusInRange += OnEventRaised;
        }
    }

    // SpreadsheetControlStylusInRangeCommand
    public class SpreadsheetControlStylusInRangeCommand : SpreadsheetControlCommandBase<SpreadsheetControlStylusInRangeCommandBehavior>
    { }

    // SpreadsheetControlStylusInRangeCommandBehavior
    public class SpreadsheetControlStylusInRangeCommandBehavior : SpreadsheetControlStylusInRangeCommandBehavior<object>
    { }

    // SpreadsheetControlStylusInRangeCommandWithEventArgs	
    public class SpreadsheetControlStylusInRangeCommandWithEventArgs : SpreadsheetControlStylusInRangeCommand<StylusEventArgs, SpreadsheetControlStylusInRangeCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlStylusInRangeCommandBehaviorWithEventArgs
    public class SpreadsheetControlStylusInRangeCommandBehaviorWithEventArgs : SpreadsheetControlStylusInRangeCommandBehavior<StylusEventArgs>
    {
        public SpreadsheetControlStylusInRangeCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlPreviewStylusOutOfRange
    // SpreadsheetControlPreviewStylusOutOfRangeCommand<T, TBehavior>
    public class SpreadsheetControlPreviewStylusOutOfRangeCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlPreviewStylusOutOfRangeCommandBehavior<T>, new()
    { }

    // SpreadsheetControlPreviewStylusOutOfRangeCommandBehavior<TReturn>
    public class SpreadsheetControlPreviewStylusOutOfRangeCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public SpreadsheetControlPreviewStylusOutOfRangeCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlPreviewStylusOutOfRangeCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewStylusOutOfRange += OnEventRaised;
        }
    }

    // SpreadsheetControlPreviewStylusOutOfRangeCommand
    public class SpreadsheetControlPreviewStylusOutOfRangeCommand : SpreadsheetControlCommandBase<SpreadsheetControlPreviewStylusOutOfRangeCommandBehavior>
    { }

    // SpreadsheetControlPreviewStylusOutOfRangeCommandBehavior
    public class SpreadsheetControlPreviewStylusOutOfRangeCommandBehavior : SpreadsheetControlPreviewStylusOutOfRangeCommandBehavior<object>
    { }

    // SpreadsheetControlPreviewStylusOutOfRangeCommandWithEventArgs	
    public class SpreadsheetControlPreviewStylusOutOfRangeCommandWithEventArgs : SpreadsheetControlPreviewStylusOutOfRangeCommand<StylusEventArgs, SpreadsheetControlPreviewStylusOutOfRangeCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlPreviewStylusOutOfRangeCommandBehaviorWithEventArgs
    public class SpreadsheetControlPreviewStylusOutOfRangeCommandBehaviorWithEventArgs : SpreadsheetControlPreviewStylusOutOfRangeCommandBehavior<StylusEventArgs>
    {
        public SpreadsheetControlPreviewStylusOutOfRangeCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlStylusOutOfRange
    // SpreadsheetControlStylusOutOfRangeCommand<T, TBehavior>
    public class SpreadsheetControlStylusOutOfRangeCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlStylusOutOfRangeCommandBehavior<T>, new()
    { }

    // SpreadsheetControlStylusOutOfRangeCommandBehavior<TReturn>
    public class SpreadsheetControlStylusOutOfRangeCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public SpreadsheetControlStylusOutOfRangeCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlStylusOutOfRangeCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusOutOfRange += OnEventRaised;
        }
    }

    // SpreadsheetControlStylusOutOfRangeCommand
    public class SpreadsheetControlStylusOutOfRangeCommand : SpreadsheetControlCommandBase<SpreadsheetControlStylusOutOfRangeCommandBehavior>
    { }

    // SpreadsheetControlStylusOutOfRangeCommandBehavior
    public class SpreadsheetControlStylusOutOfRangeCommandBehavior : SpreadsheetControlStylusOutOfRangeCommandBehavior<object>
    { }

    // SpreadsheetControlStylusOutOfRangeCommandWithEventArgs	
    public class SpreadsheetControlStylusOutOfRangeCommandWithEventArgs : SpreadsheetControlStylusOutOfRangeCommand<StylusEventArgs, SpreadsheetControlStylusOutOfRangeCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlStylusOutOfRangeCommandBehaviorWithEventArgs
    public class SpreadsheetControlStylusOutOfRangeCommandBehaviorWithEventArgs : SpreadsheetControlStylusOutOfRangeCommandBehavior<StylusEventArgs>
    {
        public SpreadsheetControlStylusOutOfRangeCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlPreviewStylusSystemGesture
    // SpreadsheetControlPreviewStylusSystemGestureCommand<T, TBehavior>
    public class SpreadsheetControlPreviewStylusSystemGestureCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlPreviewStylusSystemGestureCommandBehavior<T>, new()
    { }

    // SpreadsheetControlPreviewStylusSystemGestureCommandBehavior<TReturn>
    public class SpreadsheetControlPreviewStylusSystemGestureCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, StylusSystemGestureEventArgs>
    {
        public SpreadsheetControlPreviewStylusSystemGestureCommandBehavior(Func<object, StylusSystemGestureEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlPreviewStylusSystemGestureCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewStylusSystemGesture += OnEventRaised;
        }
    }

    // SpreadsheetControlPreviewStylusSystemGestureCommand
    public class SpreadsheetControlPreviewStylusSystemGestureCommand : SpreadsheetControlCommandBase<SpreadsheetControlPreviewStylusSystemGestureCommandBehavior>
    { }

    // SpreadsheetControlPreviewStylusSystemGestureCommandBehavior
    public class SpreadsheetControlPreviewStylusSystemGestureCommandBehavior : SpreadsheetControlPreviewStylusSystemGestureCommandBehavior<object>
    { }

    // SpreadsheetControlPreviewStylusSystemGestureCommandWithEventArgs	
    public class SpreadsheetControlPreviewStylusSystemGestureCommandWithEventArgs : SpreadsheetControlPreviewStylusSystemGestureCommand<StylusSystemGestureEventArgs, SpreadsheetControlPreviewStylusSystemGestureCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlPreviewStylusSystemGestureCommandBehaviorWithEventArgs
    public class SpreadsheetControlPreviewStylusSystemGestureCommandBehaviorWithEventArgs : SpreadsheetControlPreviewStylusSystemGestureCommandBehavior<StylusSystemGestureEventArgs>
    {
        public SpreadsheetControlPreviewStylusSystemGestureCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlStylusSystemGesture
    // SpreadsheetControlStylusSystemGestureCommand<T, TBehavior>
    public class SpreadsheetControlStylusSystemGestureCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlStylusSystemGestureCommandBehavior<T>, new()
    { }

    // SpreadsheetControlStylusSystemGestureCommandBehavior<TReturn>
    public class SpreadsheetControlStylusSystemGestureCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, StylusSystemGestureEventArgs>
    {
        public SpreadsheetControlStylusSystemGestureCommandBehavior(Func<object, StylusSystemGestureEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlStylusSystemGestureCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusSystemGesture += OnEventRaised;
        }
    }

    // SpreadsheetControlStylusSystemGestureCommand
    public class SpreadsheetControlStylusSystemGestureCommand : SpreadsheetControlCommandBase<SpreadsheetControlStylusSystemGestureCommandBehavior>
    { }

    // SpreadsheetControlStylusSystemGestureCommandBehavior
    public class SpreadsheetControlStylusSystemGestureCommandBehavior : SpreadsheetControlStylusSystemGestureCommandBehavior<object>
    { }

    // SpreadsheetControlStylusSystemGestureCommandWithEventArgs	
    public class SpreadsheetControlStylusSystemGestureCommandWithEventArgs : SpreadsheetControlStylusSystemGestureCommand<StylusSystemGestureEventArgs, SpreadsheetControlStylusSystemGestureCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlStylusSystemGestureCommandBehaviorWithEventArgs
    public class SpreadsheetControlStylusSystemGestureCommandBehaviorWithEventArgs : SpreadsheetControlStylusSystemGestureCommandBehavior<StylusSystemGestureEventArgs>
    {
        public SpreadsheetControlStylusSystemGestureCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlGotStylusCapture
    // SpreadsheetControlGotStylusCaptureCommand<T, TBehavior>
    public class SpreadsheetControlGotStylusCaptureCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlGotStylusCaptureCommandBehavior<T>, new()
    { }

    // SpreadsheetControlGotStylusCaptureCommandBehavior<TReturn>
    public class SpreadsheetControlGotStylusCaptureCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public SpreadsheetControlGotStylusCaptureCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlGotStylusCaptureCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.GotStylusCapture += OnEventRaised;
        }
    }

    // SpreadsheetControlGotStylusCaptureCommand
    public class SpreadsheetControlGotStylusCaptureCommand : SpreadsheetControlCommandBase<SpreadsheetControlGotStylusCaptureCommandBehavior>
    { }

    // SpreadsheetControlGotStylusCaptureCommandBehavior
    public class SpreadsheetControlGotStylusCaptureCommandBehavior : SpreadsheetControlGotStylusCaptureCommandBehavior<object>
    { }

    // SpreadsheetControlGotStylusCaptureCommandWithEventArgs	
    public class SpreadsheetControlGotStylusCaptureCommandWithEventArgs : SpreadsheetControlGotStylusCaptureCommand<StylusEventArgs, SpreadsheetControlGotStylusCaptureCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlGotStylusCaptureCommandBehaviorWithEventArgs
    public class SpreadsheetControlGotStylusCaptureCommandBehaviorWithEventArgs : SpreadsheetControlGotStylusCaptureCommandBehavior<StylusEventArgs>
    {
        public SpreadsheetControlGotStylusCaptureCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlLostStylusCapture
    // SpreadsheetControlLostStylusCaptureCommand<T, TBehavior>
    public class SpreadsheetControlLostStylusCaptureCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlLostStylusCaptureCommandBehavior<T>, new()
    { }

    // SpreadsheetControlLostStylusCaptureCommandBehavior<TReturn>
    public class SpreadsheetControlLostStylusCaptureCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public SpreadsheetControlLostStylusCaptureCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlLostStylusCaptureCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.LostStylusCapture += OnEventRaised;
        }
    }

    // SpreadsheetControlLostStylusCaptureCommand
    public class SpreadsheetControlLostStylusCaptureCommand : SpreadsheetControlCommandBase<SpreadsheetControlLostStylusCaptureCommandBehavior>
    { }

    // SpreadsheetControlLostStylusCaptureCommandBehavior
    public class SpreadsheetControlLostStylusCaptureCommandBehavior : SpreadsheetControlLostStylusCaptureCommandBehavior<object>
    { }

    // SpreadsheetControlLostStylusCaptureCommandWithEventArgs	
    public class SpreadsheetControlLostStylusCaptureCommandWithEventArgs : SpreadsheetControlLostStylusCaptureCommand<StylusEventArgs, SpreadsheetControlLostStylusCaptureCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlLostStylusCaptureCommandBehaviorWithEventArgs
    public class SpreadsheetControlLostStylusCaptureCommandBehaviorWithEventArgs : SpreadsheetControlLostStylusCaptureCommandBehavior<StylusEventArgs>
    {
        public SpreadsheetControlLostStylusCaptureCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlStylusButtonDown
    // SpreadsheetControlStylusButtonDownCommand<T, TBehavior>
    public class SpreadsheetControlStylusButtonDownCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlStylusButtonDownCommandBehavior<T>, new()
    { }

    // SpreadsheetControlStylusButtonDownCommandBehavior<TReturn>
    public class SpreadsheetControlStylusButtonDownCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, StylusButtonEventArgs>
    {
        public SpreadsheetControlStylusButtonDownCommandBehavior(Func<object, StylusButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlStylusButtonDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusButtonDown += OnEventRaised;
        }
    }

    // SpreadsheetControlStylusButtonDownCommand
    public class SpreadsheetControlStylusButtonDownCommand : SpreadsheetControlCommandBase<SpreadsheetControlStylusButtonDownCommandBehavior>
    { }

    // SpreadsheetControlStylusButtonDownCommandBehavior
    public class SpreadsheetControlStylusButtonDownCommandBehavior : SpreadsheetControlStylusButtonDownCommandBehavior<object>
    { }

    // SpreadsheetControlStylusButtonDownCommandWithEventArgs	
    public class SpreadsheetControlStylusButtonDownCommandWithEventArgs : SpreadsheetControlStylusButtonDownCommand<StylusButtonEventArgs, SpreadsheetControlStylusButtonDownCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlStylusButtonDownCommandBehaviorWithEventArgs
    public class SpreadsheetControlStylusButtonDownCommandBehaviorWithEventArgs : SpreadsheetControlStylusButtonDownCommandBehavior<StylusButtonEventArgs>
    {
        public SpreadsheetControlStylusButtonDownCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlStylusButtonUp
    // SpreadsheetControlStylusButtonUpCommand<T, TBehavior>
    public class SpreadsheetControlStylusButtonUpCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlStylusButtonUpCommandBehavior<T>, new()
    { }

    // SpreadsheetControlStylusButtonUpCommandBehavior<TReturn>
    public class SpreadsheetControlStylusButtonUpCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, StylusButtonEventArgs>
    {
        public SpreadsheetControlStylusButtonUpCommandBehavior(Func<object, StylusButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlStylusButtonUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusButtonUp += OnEventRaised;
        }
    }

    // SpreadsheetControlStylusButtonUpCommand
    public class SpreadsheetControlStylusButtonUpCommand : SpreadsheetControlCommandBase<SpreadsheetControlStylusButtonUpCommandBehavior>
    { }

    // SpreadsheetControlStylusButtonUpCommandBehavior
    public class SpreadsheetControlStylusButtonUpCommandBehavior : SpreadsheetControlStylusButtonUpCommandBehavior<object>
    { }

    // SpreadsheetControlStylusButtonUpCommandWithEventArgs	
    public class SpreadsheetControlStylusButtonUpCommandWithEventArgs : SpreadsheetControlStylusButtonUpCommand<StylusButtonEventArgs, SpreadsheetControlStylusButtonUpCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlStylusButtonUpCommandBehaviorWithEventArgs
    public class SpreadsheetControlStylusButtonUpCommandBehaviorWithEventArgs : SpreadsheetControlStylusButtonUpCommandBehavior<StylusButtonEventArgs>
    {
        public SpreadsheetControlStylusButtonUpCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlPreviewStylusButtonDown
    // SpreadsheetControlPreviewStylusButtonDownCommand<T, TBehavior>
    public class SpreadsheetControlPreviewStylusButtonDownCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlPreviewStylusButtonDownCommandBehavior<T>, new()
    { }

    // SpreadsheetControlPreviewStylusButtonDownCommandBehavior<TReturn>
    public class SpreadsheetControlPreviewStylusButtonDownCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, StylusButtonEventArgs>
    {
        public SpreadsheetControlPreviewStylusButtonDownCommandBehavior(Func<object, StylusButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlPreviewStylusButtonDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewStylusButtonDown += OnEventRaised;
        }
    }

    // SpreadsheetControlPreviewStylusButtonDownCommand
    public class SpreadsheetControlPreviewStylusButtonDownCommand : SpreadsheetControlCommandBase<SpreadsheetControlPreviewStylusButtonDownCommandBehavior>
    { }

    // SpreadsheetControlPreviewStylusButtonDownCommandBehavior
    public class SpreadsheetControlPreviewStylusButtonDownCommandBehavior : SpreadsheetControlPreviewStylusButtonDownCommandBehavior<object>
    { }

    // SpreadsheetControlPreviewStylusButtonDownCommandWithEventArgs	
    public class SpreadsheetControlPreviewStylusButtonDownCommandWithEventArgs : SpreadsheetControlPreviewStylusButtonDownCommand<StylusButtonEventArgs, SpreadsheetControlPreviewStylusButtonDownCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlPreviewStylusButtonDownCommandBehaviorWithEventArgs
    public class SpreadsheetControlPreviewStylusButtonDownCommandBehaviorWithEventArgs : SpreadsheetControlPreviewStylusButtonDownCommandBehavior<StylusButtonEventArgs>
    {
        public SpreadsheetControlPreviewStylusButtonDownCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlPreviewStylusButtonUp
    // SpreadsheetControlPreviewStylusButtonUpCommand<T, TBehavior>
    public class SpreadsheetControlPreviewStylusButtonUpCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlPreviewStylusButtonUpCommandBehavior<T>, new()
    { }

    // SpreadsheetControlPreviewStylusButtonUpCommandBehavior<TReturn>
    public class SpreadsheetControlPreviewStylusButtonUpCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, StylusButtonEventArgs>
    {
        public SpreadsheetControlPreviewStylusButtonUpCommandBehavior(Func<object, StylusButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlPreviewStylusButtonUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewStylusButtonUp += OnEventRaised;
        }
    }

    // SpreadsheetControlPreviewStylusButtonUpCommand
    public class SpreadsheetControlPreviewStylusButtonUpCommand : SpreadsheetControlCommandBase<SpreadsheetControlPreviewStylusButtonUpCommandBehavior>
    { }

    // SpreadsheetControlPreviewStylusButtonUpCommandBehavior
    public class SpreadsheetControlPreviewStylusButtonUpCommandBehavior : SpreadsheetControlPreviewStylusButtonUpCommandBehavior<object>
    { }

    // SpreadsheetControlPreviewStylusButtonUpCommandWithEventArgs	
    public class SpreadsheetControlPreviewStylusButtonUpCommandWithEventArgs : SpreadsheetControlPreviewStylusButtonUpCommand<StylusButtonEventArgs, SpreadsheetControlPreviewStylusButtonUpCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlPreviewStylusButtonUpCommandBehaviorWithEventArgs
    public class SpreadsheetControlPreviewStylusButtonUpCommandBehaviorWithEventArgs : SpreadsheetControlPreviewStylusButtonUpCommandBehavior<StylusButtonEventArgs>
    {
        public SpreadsheetControlPreviewStylusButtonUpCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlPreviewKeyDown
    // SpreadsheetControlPreviewKeyDownCommand<T, TBehavior>
    public class SpreadsheetControlPreviewKeyDownCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlPreviewKeyDownCommandBehavior<T>, new()
    { }

    // SpreadsheetControlPreviewKeyDownCommandBehavior<TReturn>
    public class SpreadsheetControlPreviewKeyDownCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, KeyEventArgs>
    {
        public SpreadsheetControlPreviewKeyDownCommandBehavior(Func<object, KeyEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlPreviewKeyDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewKeyDown += OnEventRaised;
        }
    }

    // SpreadsheetControlPreviewKeyDownCommand
    public class SpreadsheetControlPreviewKeyDownCommand : SpreadsheetControlCommandBase<SpreadsheetControlPreviewKeyDownCommandBehavior>
    { }

    // SpreadsheetControlPreviewKeyDownCommandBehavior
    public class SpreadsheetControlPreviewKeyDownCommandBehavior : SpreadsheetControlPreviewKeyDownCommandBehavior<object>
    { }

    // SpreadsheetControlPreviewKeyDownCommandWithEventArgs	
    public class SpreadsheetControlPreviewKeyDownCommandWithEventArgs : SpreadsheetControlPreviewKeyDownCommand<KeyEventArgs, SpreadsheetControlPreviewKeyDownCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlPreviewKeyDownCommandBehaviorWithEventArgs
    public class SpreadsheetControlPreviewKeyDownCommandBehaviorWithEventArgs : SpreadsheetControlPreviewKeyDownCommandBehavior<KeyEventArgs>
    {
        public SpreadsheetControlPreviewKeyDownCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlPreviewKeyUp
    // SpreadsheetControlPreviewKeyUpCommand<T, TBehavior>
    public class SpreadsheetControlPreviewKeyUpCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlPreviewKeyUpCommandBehavior<T>, new()
    { }

    // SpreadsheetControlPreviewKeyUpCommandBehavior<TReturn>
    public class SpreadsheetControlPreviewKeyUpCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, KeyEventArgs>
    {
        public SpreadsheetControlPreviewKeyUpCommandBehavior(Func<object, KeyEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlPreviewKeyUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewKeyUp += OnEventRaised;
        }
    }

    // SpreadsheetControlPreviewKeyUpCommand
    public class SpreadsheetControlPreviewKeyUpCommand : SpreadsheetControlCommandBase<SpreadsheetControlPreviewKeyUpCommandBehavior>
    { }

    // SpreadsheetControlPreviewKeyUpCommandBehavior
    public class SpreadsheetControlPreviewKeyUpCommandBehavior : SpreadsheetControlPreviewKeyUpCommandBehavior<object>
    { }

    // SpreadsheetControlPreviewKeyUpCommandWithEventArgs	
    public class SpreadsheetControlPreviewKeyUpCommandWithEventArgs : SpreadsheetControlPreviewKeyUpCommand<KeyEventArgs, SpreadsheetControlPreviewKeyUpCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlPreviewKeyUpCommandBehaviorWithEventArgs
    public class SpreadsheetControlPreviewKeyUpCommandBehaviorWithEventArgs : SpreadsheetControlPreviewKeyUpCommandBehavior<KeyEventArgs>
    {
        public SpreadsheetControlPreviewKeyUpCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlPreviewGotKeyboardFocus
    // SpreadsheetControlPreviewGotKeyboardFocusCommand<T, TBehavior>
    public class SpreadsheetControlPreviewGotKeyboardFocusCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlPreviewGotKeyboardFocusCommandBehavior<T>, new()
    { }

    // SpreadsheetControlPreviewGotKeyboardFocusCommandBehavior<TReturn>
    public class SpreadsheetControlPreviewGotKeyboardFocusCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, KeyboardFocusChangedEventArgs>
    {
        public SpreadsheetControlPreviewGotKeyboardFocusCommandBehavior(Func<object, KeyboardFocusChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlPreviewGotKeyboardFocusCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewGotKeyboardFocus += OnEventRaised;
        }
    }

    // SpreadsheetControlPreviewGotKeyboardFocusCommand
    public class SpreadsheetControlPreviewGotKeyboardFocusCommand : SpreadsheetControlCommandBase<SpreadsheetControlPreviewGotKeyboardFocusCommandBehavior>
    { }

    // SpreadsheetControlPreviewGotKeyboardFocusCommandBehavior
    public class SpreadsheetControlPreviewGotKeyboardFocusCommandBehavior : SpreadsheetControlPreviewGotKeyboardFocusCommandBehavior<object>
    { }

    // SpreadsheetControlPreviewGotKeyboardFocusCommandWithEventArgs	
    public class SpreadsheetControlPreviewGotKeyboardFocusCommandWithEventArgs : SpreadsheetControlPreviewGotKeyboardFocusCommand<KeyboardFocusChangedEventArgs, SpreadsheetControlPreviewGotKeyboardFocusCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlPreviewGotKeyboardFocusCommandBehaviorWithEventArgs
    public class SpreadsheetControlPreviewGotKeyboardFocusCommandBehaviorWithEventArgs : SpreadsheetControlPreviewGotKeyboardFocusCommandBehavior<KeyboardFocusChangedEventArgs>
    {
        public SpreadsheetControlPreviewGotKeyboardFocusCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlGotKeyboardFocus
    // SpreadsheetControlGotKeyboardFocusCommand<T, TBehavior>
    public class SpreadsheetControlGotKeyboardFocusCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlGotKeyboardFocusCommandBehavior<T>, new()
    { }

    // SpreadsheetControlGotKeyboardFocusCommandBehavior<TReturn>
    public class SpreadsheetControlGotKeyboardFocusCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, KeyboardFocusChangedEventArgs>
    {
        public SpreadsheetControlGotKeyboardFocusCommandBehavior(Func<object, KeyboardFocusChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlGotKeyboardFocusCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.GotKeyboardFocus += OnEventRaised;
        }
    }

    // SpreadsheetControlGotKeyboardFocusCommand
    public class SpreadsheetControlGotKeyboardFocusCommand : SpreadsheetControlCommandBase<SpreadsheetControlGotKeyboardFocusCommandBehavior>
    { }

    // SpreadsheetControlGotKeyboardFocusCommandBehavior
    public class SpreadsheetControlGotKeyboardFocusCommandBehavior : SpreadsheetControlGotKeyboardFocusCommandBehavior<object>
    { }

    // SpreadsheetControlGotKeyboardFocusCommandWithEventArgs	
    public class SpreadsheetControlGotKeyboardFocusCommandWithEventArgs : SpreadsheetControlGotKeyboardFocusCommand<KeyboardFocusChangedEventArgs, SpreadsheetControlGotKeyboardFocusCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlGotKeyboardFocusCommandBehaviorWithEventArgs
    public class SpreadsheetControlGotKeyboardFocusCommandBehaviorWithEventArgs : SpreadsheetControlGotKeyboardFocusCommandBehavior<KeyboardFocusChangedEventArgs>
    {
        public SpreadsheetControlGotKeyboardFocusCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlPreviewLostKeyboardFocus
    // SpreadsheetControlPreviewLostKeyboardFocusCommand<T, TBehavior>
    public class SpreadsheetControlPreviewLostKeyboardFocusCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlPreviewLostKeyboardFocusCommandBehavior<T>, new()
    { }

    // SpreadsheetControlPreviewLostKeyboardFocusCommandBehavior<TReturn>
    public class SpreadsheetControlPreviewLostKeyboardFocusCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, KeyboardFocusChangedEventArgs>
    {
        public SpreadsheetControlPreviewLostKeyboardFocusCommandBehavior(Func<object, KeyboardFocusChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlPreviewLostKeyboardFocusCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewLostKeyboardFocus += OnEventRaised;
        }
    }

    // SpreadsheetControlPreviewLostKeyboardFocusCommand
    public class SpreadsheetControlPreviewLostKeyboardFocusCommand : SpreadsheetControlCommandBase<SpreadsheetControlPreviewLostKeyboardFocusCommandBehavior>
    { }

    // SpreadsheetControlPreviewLostKeyboardFocusCommandBehavior
    public class SpreadsheetControlPreviewLostKeyboardFocusCommandBehavior : SpreadsheetControlPreviewLostKeyboardFocusCommandBehavior<object>
    { }

    // SpreadsheetControlPreviewLostKeyboardFocusCommandWithEventArgs	
    public class SpreadsheetControlPreviewLostKeyboardFocusCommandWithEventArgs : SpreadsheetControlPreviewLostKeyboardFocusCommand<KeyboardFocusChangedEventArgs, SpreadsheetControlPreviewLostKeyboardFocusCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlPreviewLostKeyboardFocusCommandBehaviorWithEventArgs
    public class SpreadsheetControlPreviewLostKeyboardFocusCommandBehaviorWithEventArgs : SpreadsheetControlPreviewLostKeyboardFocusCommandBehavior<KeyboardFocusChangedEventArgs>
    {
        public SpreadsheetControlPreviewLostKeyboardFocusCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlLostKeyboardFocus
    // SpreadsheetControlLostKeyboardFocusCommand<T, TBehavior>
    public class SpreadsheetControlLostKeyboardFocusCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlLostKeyboardFocusCommandBehavior<T>, new()
    { }

    // SpreadsheetControlLostKeyboardFocusCommandBehavior<TReturn>
    public class SpreadsheetControlLostKeyboardFocusCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, KeyboardFocusChangedEventArgs>
    {
        public SpreadsheetControlLostKeyboardFocusCommandBehavior(Func<object, KeyboardFocusChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlLostKeyboardFocusCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.LostKeyboardFocus += OnEventRaised;
        }
    }

    // SpreadsheetControlLostKeyboardFocusCommand
    public class SpreadsheetControlLostKeyboardFocusCommand : SpreadsheetControlCommandBase<SpreadsheetControlLostKeyboardFocusCommandBehavior>
    { }

    // SpreadsheetControlLostKeyboardFocusCommandBehavior
    public class SpreadsheetControlLostKeyboardFocusCommandBehavior : SpreadsheetControlLostKeyboardFocusCommandBehavior<object>
    { }

    // SpreadsheetControlLostKeyboardFocusCommandWithEventArgs	
    public class SpreadsheetControlLostKeyboardFocusCommandWithEventArgs : SpreadsheetControlLostKeyboardFocusCommand<KeyboardFocusChangedEventArgs, SpreadsheetControlLostKeyboardFocusCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlLostKeyboardFocusCommandBehaviorWithEventArgs
    public class SpreadsheetControlLostKeyboardFocusCommandBehaviorWithEventArgs : SpreadsheetControlLostKeyboardFocusCommandBehavior<KeyboardFocusChangedEventArgs>
    {
        public SpreadsheetControlLostKeyboardFocusCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlPreviewTextInput
    // SpreadsheetControlPreviewTextInputCommand<T, TBehavior>
    public class SpreadsheetControlPreviewTextInputCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlPreviewTextInputCommandBehavior<T>, new()
    { }

    // SpreadsheetControlPreviewTextInputCommandBehavior<TReturn>
    public class SpreadsheetControlPreviewTextInputCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, TextCompositionEventArgs>
    {
        public SpreadsheetControlPreviewTextInputCommandBehavior(Func<object, TextCompositionEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlPreviewTextInputCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewTextInput += OnEventRaised;
        }
    }

    // SpreadsheetControlPreviewTextInputCommand
    public class SpreadsheetControlPreviewTextInputCommand : SpreadsheetControlCommandBase<SpreadsheetControlPreviewTextInputCommandBehavior>
    { }

    // SpreadsheetControlPreviewTextInputCommandBehavior
    public class SpreadsheetControlPreviewTextInputCommandBehavior : SpreadsheetControlPreviewTextInputCommandBehavior<object>
    { }

    // SpreadsheetControlPreviewTextInputCommandWithEventArgs	
    public class SpreadsheetControlPreviewTextInputCommandWithEventArgs : SpreadsheetControlPreviewTextInputCommand<TextCompositionEventArgs, SpreadsheetControlPreviewTextInputCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlPreviewTextInputCommandBehaviorWithEventArgs
    public class SpreadsheetControlPreviewTextInputCommandBehaviorWithEventArgs : SpreadsheetControlPreviewTextInputCommandBehavior<TextCompositionEventArgs>
    {
        public SpreadsheetControlPreviewTextInputCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlPreviewQueryContinueDrag
    // SpreadsheetControlPreviewQueryContinueDragCommand<T, TBehavior>
    public class SpreadsheetControlPreviewQueryContinueDragCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlPreviewQueryContinueDragCommandBehavior<T>, new()
    { }

    // SpreadsheetControlPreviewQueryContinueDragCommandBehavior<TReturn>
    public class SpreadsheetControlPreviewQueryContinueDragCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, QueryContinueDragEventArgs>
    {
        public SpreadsheetControlPreviewQueryContinueDragCommandBehavior(Func<object, QueryContinueDragEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlPreviewQueryContinueDragCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewQueryContinueDrag += OnEventRaised;
        }
    }

    // SpreadsheetControlPreviewQueryContinueDragCommand
    public class SpreadsheetControlPreviewQueryContinueDragCommand : SpreadsheetControlCommandBase<SpreadsheetControlPreviewQueryContinueDragCommandBehavior>
    { }

    // SpreadsheetControlPreviewQueryContinueDragCommandBehavior
    public class SpreadsheetControlPreviewQueryContinueDragCommandBehavior : SpreadsheetControlPreviewQueryContinueDragCommandBehavior<object>
    { }

    // SpreadsheetControlPreviewQueryContinueDragCommandWithEventArgs	
    public class SpreadsheetControlPreviewQueryContinueDragCommandWithEventArgs : SpreadsheetControlPreviewQueryContinueDragCommand<QueryContinueDragEventArgs, SpreadsheetControlPreviewQueryContinueDragCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlPreviewQueryContinueDragCommandBehaviorWithEventArgs
    public class SpreadsheetControlPreviewQueryContinueDragCommandBehaviorWithEventArgs : SpreadsheetControlPreviewQueryContinueDragCommandBehavior<QueryContinueDragEventArgs>
    {
        public SpreadsheetControlPreviewQueryContinueDragCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlQueryContinueDrag
    // SpreadsheetControlQueryContinueDragCommand<T, TBehavior>
    public class SpreadsheetControlQueryContinueDragCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlQueryContinueDragCommandBehavior<T>, new()
    { }

    // SpreadsheetControlQueryContinueDragCommandBehavior<TReturn>
    public class SpreadsheetControlQueryContinueDragCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, QueryContinueDragEventArgs>
    {
        public SpreadsheetControlQueryContinueDragCommandBehavior(Func<object, QueryContinueDragEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlQueryContinueDragCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.QueryContinueDrag += OnEventRaised;
        }
    }

    // SpreadsheetControlQueryContinueDragCommand
    public class SpreadsheetControlQueryContinueDragCommand : SpreadsheetControlCommandBase<SpreadsheetControlQueryContinueDragCommandBehavior>
    { }

    // SpreadsheetControlQueryContinueDragCommandBehavior
    public class SpreadsheetControlQueryContinueDragCommandBehavior : SpreadsheetControlQueryContinueDragCommandBehavior<object>
    { }

    // SpreadsheetControlQueryContinueDragCommandWithEventArgs	
    public class SpreadsheetControlQueryContinueDragCommandWithEventArgs : SpreadsheetControlQueryContinueDragCommand<QueryContinueDragEventArgs, SpreadsheetControlQueryContinueDragCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlQueryContinueDragCommandBehaviorWithEventArgs
    public class SpreadsheetControlQueryContinueDragCommandBehaviorWithEventArgs : SpreadsheetControlQueryContinueDragCommandBehavior<QueryContinueDragEventArgs>
    {
        public SpreadsheetControlQueryContinueDragCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlPreviewGiveFeedback
    // SpreadsheetControlPreviewGiveFeedbackCommand<T, TBehavior>
    public class SpreadsheetControlPreviewGiveFeedbackCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlPreviewGiveFeedbackCommandBehavior<T>, new()
    { }

    // SpreadsheetControlPreviewGiveFeedbackCommandBehavior<TReturn>
    public class SpreadsheetControlPreviewGiveFeedbackCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, GiveFeedbackEventArgs>
    {
        public SpreadsheetControlPreviewGiveFeedbackCommandBehavior(Func<object, GiveFeedbackEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlPreviewGiveFeedbackCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewGiveFeedback += OnEventRaised;
        }
    }

    // SpreadsheetControlPreviewGiveFeedbackCommand
    public class SpreadsheetControlPreviewGiveFeedbackCommand : SpreadsheetControlCommandBase<SpreadsheetControlPreviewGiveFeedbackCommandBehavior>
    { }

    // SpreadsheetControlPreviewGiveFeedbackCommandBehavior
    public class SpreadsheetControlPreviewGiveFeedbackCommandBehavior : SpreadsheetControlPreviewGiveFeedbackCommandBehavior<object>
    { }

    // SpreadsheetControlPreviewGiveFeedbackCommandWithEventArgs	
    public class SpreadsheetControlPreviewGiveFeedbackCommandWithEventArgs : SpreadsheetControlPreviewGiveFeedbackCommand<GiveFeedbackEventArgs, SpreadsheetControlPreviewGiveFeedbackCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlPreviewGiveFeedbackCommandBehaviorWithEventArgs
    public class SpreadsheetControlPreviewGiveFeedbackCommandBehaviorWithEventArgs : SpreadsheetControlPreviewGiveFeedbackCommandBehavior<GiveFeedbackEventArgs>
    {
        public SpreadsheetControlPreviewGiveFeedbackCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlGiveFeedback
    // SpreadsheetControlGiveFeedbackCommand<T, TBehavior>
    public class SpreadsheetControlGiveFeedbackCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlGiveFeedbackCommandBehavior<T>, new()
    { }

    // SpreadsheetControlGiveFeedbackCommandBehavior<TReturn>
    public class SpreadsheetControlGiveFeedbackCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, GiveFeedbackEventArgs>
    {
        public SpreadsheetControlGiveFeedbackCommandBehavior(Func<object, GiveFeedbackEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlGiveFeedbackCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.GiveFeedback += OnEventRaised;
        }
    }

    // SpreadsheetControlGiveFeedbackCommand
    public class SpreadsheetControlGiveFeedbackCommand : SpreadsheetControlCommandBase<SpreadsheetControlGiveFeedbackCommandBehavior>
    { }

    // SpreadsheetControlGiveFeedbackCommandBehavior
    public class SpreadsheetControlGiveFeedbackCommandBehavior : SpreadsheetControlGiveFeedbackCommandBehavior<object>
    { }

    // SpreadsheetControlGiveFeedbackCommandWithEventArgs	
    public class SpreadsheetControlGiveFeedbackCommandWithEventArgs : SpreadsheetControlGiveFeedbackCommand<GiveFeedbackEventArgs, SpreadsheetControlGiveFeedbackCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlGiveFeedbackCommandBehaviorWithEventArgs
    public class SpreadsheetControlGiveFeedbackCommandBehaviorWithEventArgs : SpreadsheetControlGiveFeedbackCommandBehavior<GiveFeedbackEventArgs>
    {
        public SpreadsheetControlGiveFeedbackCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlPreviewDragEnter
    // SpreadsheetControlPreviewDragEnterCommand<T, TBehavior>
    public class SpreadsheetControlPreviewDragEnterCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlPreviewDragEnterCommandBehavior<T>, new()
    { }

    // SpreadsheetControlPreviewDragEnterCommandBehavior<TReturn>
    public class SpreadsheetControlPreviewDragEnterCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, DragEventArgs>
    {
        public SpreadsheetControlPreviewDragEnterCommandBehavior(Func<object, DragEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlPreviewDragEnterCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewDragEnter += OnEventRaised;
        }
    }

    // SpreadsheetControlPreviewDragEnterCommand
    public class SpreadsheetControlPreviewDragEnterCommand : SpreadsheetControlCommandBase<SpreadsheetControlPreviewDragEnterCommandBehavior>
    { }

    // SpreadsheetControlPreviewDragEnterCommandBehavior
    public class SpreadsheetControlPreviewDragEnterCommandBehavior : SpreadsheetControlPreviewDragEnterCommandBehavior<object>
    { }

    // SpreadsheetControlPreviewDragEnterCommandWithEventArgs	
    public class SpreadsheetControlPreviewDragEnterCommandWithEventArgs : SpreadsheetControlPreviewDragEnterCommand<DragEventArgs, SpreadsheetControlPreviewDragEnterCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlPreviewDragEnterCommandBehaviorWithEventArgs
    public class SpreadsheetControlPreviewDragEnterCommandBehaviorWithEventArgs : SpreadsheetControlPreviewDragEnterCommandBehavior<DragEventArgs>
    {
        public SpreadsheetControlPreviewDragEnterCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlPreviewDragOver
    // SpreadsheetControlPreviewDragOverCommand<T, TBehavior>
    public class SpreadsheetControlPreviewDragOverCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlPreviewDragOverCommandBehavior<T>, new()
    { }

    // SpreadsheetControlPreviewDragOverCommandBehavior<TReturn>
    public class SpreadsheetControlPreviewDragOverCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, DragEventArgs>
    {
        public SpreadsheetControlPreviewDragOverCommandBehavior(Func<object, DragEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlPreviewDragOverCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewDragOver += OnEventRaised;
        }
    }

    // SpreadsheetControlPreviewDragOverCommand
    public class SpreadsheetControlPreviewDragOverCommand : SpreadsheetControlCommandBase<SpreadsheetControlPreviewDragOverCommandBehavior>
    { }

    // SpreadsheetControlPreviewDragOverCommandBehavior
    public class SpreadsheetControlPreviewDragOverCommandBehavior : SpreadsheetControlPreviewDragOverCommandBehavior<object>
    { }

    // SpreadsheetControlPreviewDragOverCommandWithEventArgs	
    public class SpreadsheetControlPreviewDragOverCommandWithEventArgs : SpreadsheetControlPreviewDragOverCommand<DragEventArgs, SpreadsheetControlPreviewDragOverCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlPreviewDragOverCommandBehaviorWithEventArgs
    public class SpreadsheetControlPreviewDragOverCommandBehaviorWithEventArgs : SpreadsheetControlPreviewDragOverCommandBehavior<DragEventArgs>
    {
        public SpreadsheetControlPreviewDragOverCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlPreviewDragLeave
    // SpreadsheetControlPreviewDragLeaveCommand<T, TBehavior>
    public class SpreadsheetControlPreviewDragLeaveCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlPreviewDragLeaveCommandBehavior<T>, new()
    { }

    // SpreadsheetControlPreviewDragLeaveCommandBehavior<TReturn>
    public class SpreadsheetControlPreviewDragLeaveCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, DragEventArgs>
    {
        public SpreadsheetControlPreviewDragLeaveCommandBehavior(Func<object, DragEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlPreviewDragLeaveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewDragLeave += OnEventRaised;
        }
    }

    // SpreadsheetControlPreviewDragLeaveCommand
    public class SpreadsheetControlPreviewDragLeaveCommand : SpreadsheetControlCommandBase<SpreadsheetControlPreviewDragLeaveCommandBehavior>
    { }

    // SpreadsheetControlPreviewDragLeaveCommandBehavior
    public class SpreadsheetControlPreviewDragLeaveCommandBehavior : SpreadsheetControlPreviewDragLeaveCommandBehavior<object>
    { }

    // SpreadsheetControlPreviewDragLeaveCommandWithEventArgs	
    public class SpreadsheetControlPreviewDragLeaveCommandWithEventArgs : SpreadsheetControlPreviewDragLeaveCommand<DragEventArgs, SpreadsheetControlPreviewDragLeaveCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlPreviewDragLeaveCommandBehaviorWithEventArgs
    public class SpreadsheetControlPreviewDragLeaveCommandBehaviorWithEventArgs : SpreadsheetControlPreviewDragLeaveCommandBehavior<DragEventArgs>
    {
        public SpreadsheetControlPreviewDragLeaveCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlPreviewDrop
    // SpreadsheetControlPreviewDropCommand<T, TBehavior>
    public class SpreadsheetControlPreviewDropCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlPreviewDropCommandBehavior<T>, new()
    { }

    // SpreadsheetControlPreviewDropCommandBehavior<TReturn>
    public class SpreadsheetControlPreviewDropCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, DragEventArgs>
    {
        public SpreadsheetControlPreviewDropCommandBehavior(Func<object, DragEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlPreviewDropCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewDrop += OnEventRaised;
        }
    }

    // SpreadsheetControlPreviewDropCommand
    public class SpreadsheetControlPreviewDropCommand : SpreadsheetControlCommandBase<SpreadsheetControlPreviewDropCommandBehavior>
    { }

    // SpreadsheetControlPreviewDropCommandBehavior
    public class SpreadsheetControlPreviewDropCommandBehavior : SpreadsheetControlPreviewDropCommandBehavior<object>
    { }

    // SpreadsheetControlPreviewDropCommandWithEventArgs	
    public class SpreadsheetControlPreviewDropCommandWithEventArgs : SpreadsheetControlPreviewDropCommand<DragEventArgs, SpreadsheetControlPreviewDropCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlPreviewDropCommandBehaviorWithEventArgs
    public class SpreadsheetControlPreviewDropCommandBehaviorWithEventArgs : SpreadsheetControlPreviewDropCommandBehavior<DragEventArgs>
    {
        public SpreadsheetControlPreviewDropCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlIsMouseDirectlyOverChanged
    // SpreadsheetControlIsMouseDirectlyOverChangedCommand<T, TBehavior>
    public class SpreadsheetControlIsMouseDirectlyOverChangedCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlIsMouseDirectlyOverChangedCommandBehavior<T>, new()
    { }

    // SpreadsheetControlIsMouseDirectlyOverChangedCommandBehavior<TReturn>
    public class SpreadsheetControlIsMouseDirectlyOverChangedCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public SpreadsheetControlIsMouseDirectlyOverChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlIsMouseDirectlyOverChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsMouseDirectlyOverChanged += OnEventRaised;
        }
    }

    // SpreadsheetControlIsMouseDirectlyOverChangedCommand
    public class SpreadsheetControlIsMouseDirectlyOverChangedCommand : SpreadsheetControlCommandBase<SpreadsheetControlIsMouseDirectlyOverChangedCommandBehavior>
    { }

    // SpreadsheetControlIsMouseDirectlyOverChangedCommandBehavior
    public class SpreadsheetControlIsMouseDirectlyOverChangedCommandBehavior : SpreadsheetControlIsMouseDirectlyOverChangedCommandBehavior<object>
    { }

    // SpreadsheetControlIsMouseDirectlyOverChangedCommandWithEventArgs	
    public class SpreadsheetControlIsMouseDirectlyOverChangedCommandWithEventArgs : SpreadsheetControlIsMouseDirectlyOverChangedCommand<DependencyPropertyChangedEventArgs, SpreadsheetControlIsMouseDirectlyOverChangedCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlIsMouseDirectlyOverChangedCommandBehaviorWithEventArgs
    public class SpreadsheetControlIsMouseDirectlyOverChangedCommandBehaviorWithEventArgs : SpreadsheetControlIsMouseDirectlyOverChangedCommandBehavior<DependencyPropertyChangedEventArgs>
    {
        public SpreadsheetControlIsMouseDirectlyOverChangedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlIsKeyboardFocusWithinChanged
    // SpreadsheetControlIsKeyboardFocusWithinChangedCommand<T, TBehavior>
    public class SpreadsheetControlIsKeyboardFocusWithinChangedCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlIsKeyboardFocusWithinChangedCommandBehavior<T>, new()
    { }

    // SpreadsheetControlIsKeyboardFocusWithinChangedCommandBehavior<TReturn>
    public class SpreadsheetControlIsKeyboardFocusWithinChangedCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public SpreadsheetControlIsKeyboardFocusWithinChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlIsKeyboardFocusWithinChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsKeyboardFocusWithinChanged += OnEventRaised;
        }
    }

    // SpreadsheetControlIsKeyboardFocusWithinChangedCommand
    public class SpreadsheetControlIsKeyboardFocusWithinChangedCommand : SpreadsheetControlCommandBase<SpreadsheetControlIsKeyboardFocusWithinChangedCommandBehavior>
    { }

    // SpreadsheetControlIsKeyboardFocusWithinChangedCommandBehavior
    public class SpreadsheetControlIsKeyboardFocusWithinChangedCommandBehavior : SpreadsheetControlIsKeyboardFocusWithinChangedCommandBehavior<object>
    { }

    // SpreadsheetControlIsKeyboardFocusWithinChangedCommandWithEventArgs	
    public class SpreadsheetControlIsKeyboardFocusWithinChangedCommandWithEventArgs : SpreadsheetControlIsKeyboardFocusWithinChangedCommand<DependencyPropertyChangedEventArgs, SpreadsheetControlIsKeyboardFocusWithinChangedCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlIsKeyboardFocusWithinChangedCommandBehaviorWithEventArgs
    public class SpreadsheetControlIsKeyboardFocusWithinChangedCommandBehaviorWithEventArgs : SpreadsheetControlIsKeyboardFocusWithinChangedCommandBehavior<DependencyPropertyChangedEventArgs>
    {
        public SpreadsheetControlIsKeyboardFocusWithinChangedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlIsMouseCapturedChanged
    // SpreadsheetControlIsMouseCapturedChangedCommand<T, TBehavior>
    public class SpreadsheetControlIsMouseCapturedChangedCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlIsMouseCapturedChangedCommandBehavior<T>, new()
    { }

    // SpreadsheetControlIsMouseCapturedChangedCommandBehavior<TReturn>
    public class SpreadsheetControlIsMouseCapturedChangedCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public SpreadsheetControlIsMouseCapturedChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlIsMouseCapturedChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsMouseCapturedChanged += OnEventRaised;
        }
    }

    // SpreadsheetControlIsMouseCapturedChangedCommand
    public class SpreadsheetControlIsMouseCapturedChangedCommand : SpreadsheetControlCommandBase<SpreadsheetControlIsMouseCapturedChangedCommandBehavior>
    { }

    // SpreadsheetControlIsMouseCapturedChangedCommandBehavior
    public class SpreadsheetControlIsMouseCapturedChangedCommandBehavior : SpreadsheetControlIsMouseCapturedChangedCommandBehavior<object>
    { }

    // SpreadsheetControlIsMouseCapturedChangedCommandWithEventArgs	
    public class SpreadsheetControlIsMouseCapturedChangedCommandWithEventArgs : SpreadsheetControlIsMouseCapturedChangedCommand<DependencyPropertyChangedEventArgs, SpreadsheetControlIsMouseCapturedChangedCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlIsMouseCapturedChangedCommandBehaviorWithEventArgs
    public class SpreadsheetControlIsMouseCapturedChangedCommandBehaviorWithEventArgs : SpreadsheetControlIsMouseCapturedChangedCommandBehavior<DependencyPropertyChangedEventArgs>
    {
        public SpreadsheetControlIsMouseCapturedChangedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlIsMouseCaptureWithinChanged
    // SpreadsheetControlIsMouseCaptureWithinChangedCommand<T, TBehavior>
    public class SpreadsheetControlIsMouseCaptureWithinChangedCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlIsMouseCaptureWithinChangedCommandBehavior<T>, new()
    { }

    // SpreadsheetControlIsMouseCaptureWithinChangedCommandBehavior<TReturn>
    public class SpreadsheetControlIsMouseCaptureWithinChangedCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public SpreadsheetControlIsMouseCaptureWithinChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlIsMouseCaptureWithinChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsMouseCaptureWithinChanged += OnEventRaised;
        }
    }

    // SpreadsheetControlIsMouseCaptureWithinChangedCommand
    public class SpreadsheetControlIsMouseCaptureWithinChangedCommand : SpreadsheetControlCommandBase<SpreadsheetControlIsMouseCaptureWithinChangedCommandBehavior>
    { }

    // SpreadsheetControlIsMouseCaptureWithinChangedCommandBehavior
    public class SpreadsheetControlIsMouseCaptureWithinChangedCommandBehavior : SpreadsheetControlIsMouseCaptureWithinChangedCommandBehavior<object>
    { }

    // SpreadsheetControlIsMouseCaptureWithinChangedCommandWithEventArgs	
    public class SpreadsheetControlIsMouseCaptureWithinChangedCommandWithEventArgs : SpreadsheetControlIsMouseCaptureWithinChangedCommand<DependencyPropertyChangedEventArgs, SpreadsheetControlIsMouseCaptureWithinChangedCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlIsMouseCaptureWithinChangedCommandBehaviorWithEventArgs
    public class SpreadsheetControlIsMouseCaptureWithinChangedCommandBehaviorWithEventArgs : SpreadsheetControlIsMouseCaptureWithinChangedCommandBehavior<DependencyPropertyChangedEventArgs>
    {
        public SpreadsheetControlIsMouseCaptureWithinChangedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlIsStylusDirectlyOverChanged
    // SpreadsheetControlIsStylusDirectlyOverChangedCommand<T, TBehavior>
    public class SpreadsheetControlIsStylusDirectlyOverChangedCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlIsStylusDirectlyOverChangedCommandBehavior<T>, new()
    { }

    // SpreadsheetControlIsStylusDirectlyOverChangedCommandBehavior<TReturn>
    public class SpreadsheetControlIsStylusDirectlyOverChangedCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public SpreadsheetControlIsStylusDirectlyOverChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlIsStylusDirectlyOverChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsStylusDirectlyOverChanged += OnEventRaised;
        }
    }

    // SpreadsheetControlIsStylusDirectlyOverChangedCommand
    public class SpreadsheetControlIsStylusDirectlyOverChangedCommand : SpreadsheetControlCommandBase<SpreadsheetControlIsStylusDirectlyOverChangedCommandBehavior>
    { }

    // SpreadsheetControlIsStylusDirectlyOverChangedCommandBehavior
    public class SpreadsheetControlIsStylusDirectlyOverChangedCommandBehavior : SpreadsheetControlIsStylusDirectlyOverChangedCommandBehavior<object>
    { }

    // SpreadsheetControlIsStylusDirectlyOverChangedCommandWithEventArgs	
    public class SpreadsheetControlIsStylusDirectlyOverChangedCommandWithEventArgs : SpreadsheetControlIsStylusDirectlyOverChangedCommand<DependencyPropertyChangedEventArgs, SpreadsheetControlIsStylusDirectlyOverChangedCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlIsStylusDirectlyOverChangedCommandBehaviorWithEventArgs
    public class SpreadsheetControlIsStylusDirectlyOverChangedCommandBehaviorWithEventArgs : SpreadsheetControlIsStylusDirectlyOverChangedCommandBehavior<DependencyPropertyChangedEventArgs>
    {
        public SpreadsheetControlIsStylusDirectlyOverChangedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlIsStylusCapturedChanged
    // SpreadsheetControlIsStylusCapturedChangedCommand<T, TBehavior>
    public class SpreadsheetControlIsStylusCapturedChangedCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlIsStylusCapturedChangedCommandBehavior<T>, new()
    { }

    // SpreadsheetControlIsStylusCapturedChangedCommandBehavior<TReturn>
    public class SpreadsheetControlIsStylusCapturedChangedCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public SpreadsheetControlIsStylusCapturedChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlIsStylusCapturedChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsStylusCapturedChanged += OnEventRaised;
        }
    }

    // SpreadsheetControlIsStylusCapturedChangedCommand
    public class SpreadsheetControlIsStylusCapturedChangedCommand : SpreadsheetControlCommandBase<SpreadsheetControlIsStylusCapturedChangedCommandBehavior>
    { }

    // SpreadsheetControlIsStylusCapturedChangedCommandBehavior
    public class SpreadsheetControlIsStylusCapturedChangedCommandBehavior : SpreadsheetControlIsStylusCapturedChangedCommandBehavior<object>
    { }

    // SpreadsheetControlIsStylusCapturedChangedCommandWithEventArgs	
    public class SpreadsheetControlIsStylusCapturedChangedCommandWithEventArgs : SpreadsheetControlIsStylusCapturedChangedCommand<DependencyPropertyChangedEventArgs, SpreadsheetControlIsStylusCapturedChangedCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlIsStylusCapturedChangedCommandBehaviorWithEventArgs
    public class SpreadsheetControlIsStylusCapturedChangedCommandBehaviorWithEventArgs : SpreadsheetControlIsStylusCapturedChangedCommandBehavior<DependencyPropertyChangedEventArgs>
    {
        public SpreadsheetControlIsStylusCapturedChangedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlIsStylusCaptureWithinChanged
    // SpreadsheetControlIsStylusCaptureWithinChangedCommand<T, TBehavior>
    public class SpreadsheetControlIsStylusCaptureWithinChangedCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlIsStylusCaptureWithinChangedCommandBehavior<T>, new()
    { }

    // SpreadsheetControlIsStylusCaptureWithinChangedCommandBehavior<TReturn>
    public class SpreadsheetControlIsStylusCaptureWithinChangedCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public SpreadsheetControlIsStylusCaptureWithinChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlIsStylusCaptureWithinChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsStylusCaptureWithinChanged += OnEventRaised;
        }
    }

    // SpreadsheetControlIsStylusCaptureWithinChangedCommand
    public class SpreadsheetControlIsStylusCaptureWithinChangedCommand : SpreadsheetControlCommandBase<SpreadsheetControlIsStylusCaptureWithinChangedCommandBehavior>
    { }

    // SpreadsheetControlIsStylusCaptureWithinChangedCommandBehavior
    public class SpreadsheetControlIsStylusCaptureWithinChangedCommandBehavior : SpreadsheetControlIsStylusCaptureWithinChangedCommandBehavior<object>
    { }

    // SpreadsheetControlIsStylusCaptureWithinChangedCommandWithEventArgs	
    public class SpreadsheetControlIsStylusCaptureWithinChangedCommandWithEventArgs : SpreadsheetControlIsStylusCaptureWithinChangedCommand<DependencyPropertyChangedEventArgs, SpreadsheetControlIsStylusCaptureWithinChangedCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlIsStylusCaptureWithinChangedCommandBehaviorWithEventArgs
    public class SpreadsheetControlIsStylusCaptureWithinChangedCommandBehaviorWithEventArgs : SpreadsheetControlIsStylusCaptureWithinChangedCommandBehavior<DependencyPropertyChangedEventArgs>
    {
        public SpreadsheetControlIsStylusCaptureWithinChangedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlIsKeyboardFocusedChanged
    // SpreadsheetControlIsKeyboardFocusedChangedCommand<T, TBehavior>
    public class SpreadsheetControlIsKeyboardFocusedChangedCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlIsKeyboardFocusedChangedCommandBehavior<T>, new()
    { }

    // SpreadsheetControlIsKeyboardFocusedChangedCommandBehavior<TReturn>
    public class SpreadsheetControlIsKeyboardFocusedChangedCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public SpreadsheetControlIsKeyboardFocusedChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlIsKeyboardFocusedChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsKeyboardFocusedChanged += OnEventRaised;
        }
    }

    // SpreadsheetControlIsKeyboardFocusedChangedCommand
    public class SpreadsheetControlIsKeyboardFocusedChangedCommand : SpreadsheetControlCommandBase<SpreadsheetControlIsKeyboardFocusedChangedCommandBehavior>
    { }

    // SpreadsheetControlIsKeyboardFocusedChangedCommandBehavior
    public class SpreadsheetControlIsKeyboardFocusedChangedCommandBehavior : SpreadsheetControlIsKeyboardFocusedChangedCommandBehavior<object>
    { }

    // SpreadsheetControlIsKeyboardFocusedChangedCommandWithEventArgs	
    public class SpreadsheetControlIsKeyboardFocusedChangedCommandWithEventArgs : SpreadsheetControlIsKeyboardFocusedChangedCommand<DependencyPropertyChangedEventArgs, SpreadsheetControlIsKeyboardFocusedChangedCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlIsKeyboardFocusedChangedCommandBehaviorWithEventArgs
    public class SpreadsheetControlIsKeyboardFocusedChangedCommandBehaviorWithEventArgs : SpreadsheetControlIsKeyboardFocusedChangedCommandBehavior<DependencyPropertyChangedEventArgs>
    {
        public SpreadsheetControlIsKeyboardFocusedChangedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlIsHitTestVisibleChanged
    // SpreadsheetControlIsHitTestVisibleChangedCommand<T, TBehavior>
    public class SpreadsheetControlIsHitTestVisibleChangedCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlIsHitTestVisibleChangedCommandBehavior<T>, new()
    { }

    // SpreadsheetControlIsHitTestVisibleChangedCommandBehavior<TReturn>
    public class SpreadsheetControlIsHitTestVisibleChangedCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public SpreadsheetControlIsHitTestVisibleChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlIsHitTestVisibleChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsHitTestVisibleChanged += OnEventRaised;
        }
    }

    // SpreadsheetControlIsHitTestVisibleChangedCommand
    public class SpreadsheetControlIsHitTestVisibleChangedCommand : SpreadsheetControlCommandBase<SpreadsheetControlIsHitTestVisibleChangedCommandBehavior>
    { }

    // SpreadsheetControlIsHitTestVisibleChangedCommandBehavior
    public class SpreadsheetControlIsHitTestVisibleChangedCommandBehavior : SpreadsheetControlIsHitTestVisibleChangedCommandBehavior<object>
    { }

    // SpreadsheetControlIsHitTestVisibleChangedCommandWithEventArgs	
    public class SpreadsheetControlIsHitTestVisibleChangedCommandWithEventArgs : SpreadsheetControlIsHitTestVisibleChangedCommand<DependencyPropertyChangedEventArgs, SpreadsheetControlIsHitTestVisibleChangedCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlIsHitTestVisibleChangedCommandBehaviorWithEventArgs
    public class SpreadsheetControlIsHitTestVisibleChangedCommandBehaviorWithEventArgs : SpreadsheetControlIsHitTestVisibleChangedCommandBehavior<DependencyPropertyChangedEventArgs>
    {
        public SpreadsheetControlIsHitTestVisibleChangedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlIsVisibleChanged
    // SpreadsheetControlIsVisibleChangedCommand<T, TBehavior>
    public class SpreadsheetControlIsVisibleChangedCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlIsVisibleChangedCommandBehavior<T>, new()
    { }

    // SpreadsheetControlIsVisibleChangedCommandBehavior<TReturn>
    public class SpreadsheetControlIsVisibleChangedCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public SpreadsheetControlIsVisibleChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlIsVisibleChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsVisibleChanged += OnEventRaised;
        }
    }

    // SpreadsheetControlIsVisibleChangedCommand
    public class SpreadsheetControlIsVisibleChangedCommand : SpreadsheetControlCommandBase<SpreadsheetControlIsVisibleChangedCommandBehavior>
    { }

    // SpreadsheetControlIsVisibleChangedCommandBehavior
    public class SpreadsheetControlIsVisibleChangedCommandBehavior : SpreadsheetControlIsVisibleChangedCommandBehavior<object>
    { }

    // SpreadsheetControlIsVisibleChangedCommandWithEventArgs	
    public class SpreadsheetControlIsVisibleChangedCommandWithEventArgs : SpreadsheetControlIsVisibleChangedCommand<DependencyPropertyChangedEventArgs, SpreadsheetControlIsVisibleChangedCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlIsVisibleChangedCommandBehaviorWithEventArgs
    public class SpreadsheetControlIsVisibleChangedCommandBehaviorWithEventArgs : SpreadsheetControlIsVisibleChangedCommandBehavior<DependencyPropertyChangedEventArgs>
    {
        public SpreadsheetControlIsVisibleChangedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region SpreadsheetControlFocusableChanged
    // SpreadsheetControlFocusableChangedCommand<T, TBehavior>
    public class SpreadsheetControlFocusableChangedCommand<T, TBehavior> : SpreadsheetControlCommandBase<TBehavior> where TBehavior : SpreadsheetControlFocusableChangedCommandBehavior<T>, new()
    { }

    // SpreadsheetControlFocusableChangedCommandBehavior<TReturn>
    public class SpreadsheetControlFocusableChangedCommandBehavior<TReturn> : SpreadsheetControlCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public SpreadsheetControlFocusableChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public SpreadsheetControlFocusableChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.FocusableChanged += OnEventRaised;
        }
    }

    // SpreadsheetControlFocusableChangedCommand
    public class SpreadsheetControlFocusableChangedCommand : SpreadsheetControlCommandBase<SpreadsheetControlFocusableChangedCommandBehavior>
    { }

    // SpreadsheetControlFocusableChangedCommandBehavior
    public class SpreadsheetControlFocusableChangedCommandBehavior : SpreadsheetControlFocusableChangedCommandBehavior<object>
    { }

    // SpreadsheetControlFocusableChangedCommandWithEventArgs	
    public class SpreadsheetControlFocusableChangedCommandWithEventArgs : SpreadsheetControlFocusableChangedCommand<DependencyPropertyChangedEventArgs, SpreadsheetControlFocusableChangedCommandBehaviorWithEventArgs>
    { }

    // SpreadsheetControlFocusableChangedCommandBehaviorWithEventArgs
    public class SpreadsheetControlFocusableChangedCommandBehaviorWithEventArgs : SpreadsheetControlFocusableChangedCommandBehavior<DependencyPropertyChangedEventArgs>
    {
        public SpreadsheetControlFocusableChangedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
#endif
}
