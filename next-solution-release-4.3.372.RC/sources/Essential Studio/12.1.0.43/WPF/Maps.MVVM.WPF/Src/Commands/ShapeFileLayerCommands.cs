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

    #region ShapeFileLayerSelectionChangedCommand
    // ShapeFileLayerSelectionChangedCommand
    public class ShapeFileLayerSelectionChangedCommand : ShapeFileLayerCommandBase<ShapeFileLayerSelectionChangedCommandBehavior>
    { }

    public class ShapeFileLayerSelectionChangedCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerSelectionChangedCommandBehavior<T>, new()
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

    public class ShapeFileLayerPreviewZoomInCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerPreviewZoomInCommandBehavior<T>, new()
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

    public class ShapeFileLayerZoomedInCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerZoomedInCommandBehavior<T>, new()
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

    public class ShapeFileLayerPreviewZoomOutCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerPreviewZoomOutCommandBehavior<T>, new()
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

    public class ShapeFileLayerZoomedOutCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerZoomedOutCommandBehavior<T>, new()
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

    public class ShapeFileLayerPanningCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerPanningCommandBehavior<T>, new()
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

    public class ShapeFileLayerPannedCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerPannedCommandBehavior<T>, new()
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

    #region ShapeFileLayerPreviewMouseDoubleClickCommand
    // ShapeFileLayerPreviewMouseDoubleClickCommand
    public class ShapeFileLayerPreviewMouseDoubleClickCommand : ShapeFileLayerCommandBase<ShapeFileLayerPreviewMouseDoubleClickCommandBehavior>
    { }

    public class ShapeFileLayerPreviewMouseDoubleClickCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerPreviewMouseDoubleClickCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerPreviewMouseDoubleClickCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public ShapeFileLayerPreviewMouseDoubleClickCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerPreviewMouseDoubleClickCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewMouseDoubleClick += OnEventRaised;
        }
    }

    // ShapeFileLayerPreviewMouseDoubleClickCommandBehavior
    public class ShapeFileLayerPreviewMouseDoubleClickCommandBehavior : ShapeFileLayerPreviewMouseDoubleClickCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerMouseDoubleClickCommand
    // ShapeFileLayerMouseDoubleClickCommand
    public class ShapeFileLayerMouseDoubleClickCommand : ShapeFileLayerCommandBase<ShapeFileLayerMouseDoubleClickCommandBehavior>
    { }

    public class ShapeFileLayerMouseDoubleClickCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerMouseDoubleClickCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerMouseDoubleClickCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public ShapeFileLayerMouseDoubleClickCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerMouseDoubleClickCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseDoubleClick += OnEventRaised;
        }
    }

    // ShapeFileLayerMouseDoubleClickCommandBehavior
    public class ShapeFileLayerMouseDoubleClickCommandBehavior : ShapeFileLayerMouseDoubleClickCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerTargetUpdatedCommand
    // ShapeFileLayerTargetUpdatedCommand
    public class ShapeFileLayerTargetUpdatedCommand : ShapeFileLayerCommandBase<ShapeFileLayerTargetUpdatedCommandBehavior>
    { }

    public class ShapeFileLayerTargetUpdatedCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerTargetUpdatedCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerTargetUpdatedCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, DataTransferEventArgs>
    {
        public ShapeFileLayerTargetUpdatedCommandBehavior(Func<object, DataTransferEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerTargetUpdatedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.TargetUpdated += OnEventRaised;
        }
    }

    // ShapeFileLayerTargetUpdatedCommandBehavior
    public class ShapeFileLayerTargetUpdatedCommandBehavior : ShapeFileLayerTargetUpdatedCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerSourceUpdatedCommand
    // ShapeFileLayerSourceUpdatedCommand
    public class ShapeFileLayerSourceUpdatedCommand : ShapeFileLayerCommandBase<ShapeFileLayerSourceUpdatedCommandBehavior>
    { }

    public class ShapeFileLayerSourceUpdatedCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerSourceUpdatedCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerSourceUpdatedCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, DataTransferEventArgs>
    {
        public ShapeFileLayerSourceUpdatedCommandBehavior(Func<object, DataTransferEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerSourceUpdatedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.SourceUpdated += OnEventRaised;
        }
    }

    // ShapeFileLayerSourceUpdatedCommandBehavior
    public class ShapeFileLayerSourceUpdatedCommandBehavior : ShapeFileLayerSourceUpdatedCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerDataContextChangedCommand
    // ShapeFileLayerDataContextChangedCommand
    public class ShapeFileLayerDataContextChangedCommand : ShapeFileLayerCommandBase<ShapeFileLayerDataContextChangedCommandBehavior>
    { }

    public class ShapeFileLayerDataContextChangedCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerDataContextChangedCommandBehavior<T>, new()
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

    #region ShapeFileLayerRequestBringIntoViewCommand
    // ShapeFileLayerRequestBringIntoViewCommand
    public class ShapeFileLayerRequestBringIntoViewCommand : ShapeFileLayerCommandBase<ShapeFileLayerRequestBringIntoViewCommandBehavior>
    { }

    public class ShapeFileLayerRequestBringIntoViewCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerRequestBringIntoViewCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerRequestBringIntoViewCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, RequestBringIntoViewEventArgs>
    {
        public ShapeFileLayerRequestBringIntoViewCommandBehavior(Func<object, RequestBringIntoViewEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerRequestBringIntoViewCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.RequestBringIntoView += OnEventRaised;
        }
    }

    // ShapeFileLayerRequestBringIntoViewCommandBehavior
    public class ShapeFileLayerRequestBringIntoViewCommandBehavior : ShapeFileLayerRequestBringIntoViewCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerSizeChangedCommand
    // ShapeFileLayerSizeChangedCommand
    public class ShapeFileLayerSizeChangedCommand : ShapeFileLayerCommandBase<ShapeFileLayerSizeChangedCommandBehavior>
    { }

    public class ShapeFileLayerSizeChangedCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerSizeChangedCommandBehavior<T>, new()
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

    #region ShapeFileLayerInitializedCommand
    // ShapeFileLayerInitializedCommand
    public class ShapeFileLayerInitializedCommand : ShapeFileLayerCommandBase<ShapeFileLayerInitializedCommandBehavior>
    { }

    public class ShapeFileLayerInitializedCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerInitializedCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerInitializedCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, EventArgs>
    {
        public ShapeFileLayerInitializedCommandBehavior(Func<object, EventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerInitializedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Initialized += OnEventRaised;
        }
    }

    // ShapeFileLayerInitializedCommandBehavior
    public class ShapeFileLayerInitializedCommandBehavior : ShapeFileLayerInitializedCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerLoadedCommand
    // ShapeFileLayerLoadedCommand
    public class ShapeFileLayerLoadedCommand : ShapeFileLayerCommandBase<ShapeFileLayerLoadedCommandBehavior>
    { }

    public class ShapeFileLayerLoadedCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerLoadedCommandBehavior<T>, new()
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

    public class ShapeFileLayerUnloadedCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerUnloadedCommandBehavior<T>, new()
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

    #region ShapeFileLayerToolTipOpeningCommand
    // ShapeFileLayerToolTipOpeningCommand
    public class ShapeFileLayerToolTipOpeningCommand : ShapeFileLayerCommandBase<ShapeFileLayerToolTipOpeningCommandBehavior>
    { }

    public class ShapeFileLayerToolTipOpeningCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerToolTipOpeningCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerToolTipOpeningCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, ToolTipEventArgs>
    {
        public ShapeFileLayerToolTipOpeningCommandBehavior(Func<object, ToolTipEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerToolTipOpeningCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ToolTipOpening += OnEventRaised;
        }
    }

    // ShapeFileLayerToolTipOpeningCommandBehavior
    public class ShapeFileLayerToolTipOpeningCommandBehavior : ShapeFileLayerToolTipOpeningCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerToolTipClosingCommand
    // ShapeFileLayerToolTipClosingCommand
    public class ShapeFileLayerToolTipClosingCommand : ShapeFileLayerCommandBase<ShapeFileLayerToolTipClosingCommandBehavior>
    { }

    public class ShapeFileLayerToolTipClosingCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerToolTipClosingCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerToolTipClosingCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, ToolTipEventArgs>
    {
        public ShapeFileLayerToolTipClosingCommandBehavior(Func<object, ToolTipEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerToolTipClosingCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ToolTipClosing += OnEventRaised;
        }
    }

    // ShapeFileLayerToolTipClosingCommandBehavior
    public class ShapeFileLayerToolTipClosingCommandBehavior : ShapeFileLayerToolTipClosingCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerContextMenuOpeningCommand
    // ShapeFileLayerContextMenuOpeningCommand
    public class ShapeFileLayerContextMenuOpeningCommand : ShapeFileLayerCommandBase<ShapeFileLayerContextMenuOpeningCommandBehavior>
    { }

    public class ShapeFileLayerContextMenuOpeningCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerContextMenuOpeningCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerContextMenuOpeningCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, ContextMenuEventArgs>
    {
        public ShapeFileLayerContextMenuOpeningCommandBehavior(Func<object, ContextMenuEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerContextMenuOpeningCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ContextMenuOpening += OnEventRaised;
        }
    }

    // ShapeFileLayerContextMenuOpeningCommandBehavior
    public class ShapeFileLayerContextMenuOpeningCommandBehavior : ShapeFileLayerContextMenuOpeningCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerContextMenuClosingCommand
    // ShapeFileLayerContextMenuClosingCommand
    public class ShapeFileLayerContextMenuClosingCommand : ShapeFileLayerCommandBase<ShapeFileLayerContextMenuClosingCommandBehavior>
    { }

    public class ShapeFileLayerContextMenuClosingCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerContextMenuClosingCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerContextMenuClosingCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, ContextMenuEventArgs>
    {
        public ShapeFileLayerContextMenuClosingCommandBehavior(Func<object, ContextMenuEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerContextMenuClosingCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ContextMenuClosing += OnEventRaised;
        }
    }

    // ShapeFileLayerContextMenuClosingCommandBehavior
    public class ShapeFileLayerContextMenuClosingCommandBehavior : ShapeFileLayerContextMenuClosingCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerPreviewMouseDownCommand
    // ShapeFileLayerPreviewMouseDownCommand
    public class ShapeFileLayerPreviewMouseDownCommand : ShapeFileLayerCommandBase<ShapeFileLayerPreviewMouseDownCommandBehavior>
    { }

    public class ShapeFileLayerPreviewMouseDownCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerPreviewMouseDownCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerPreviewMouseDownCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public ShapeFileLayerPreviewMouseDownCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerPreviewMouseDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewMouseDown += OnEventRaised;
        }
    }

    // ShapeFileLayerPreviewMouseDownCommandBehavior
    public class ShapeFileLayerPreviewMouseDownCommandBehavior : ShapeFileLayerPreviewMouseDownCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerMouseDownCommand
    // ShapeFileLayerMouseDownCommand
    public class ShapeFileLayerMouseDownCommand : ShapeFileLayerCommandBase<ShapeFileLayerMouseDownCommandBehavior>
    { }

    public class ShapeFileLayerMouseDownCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerMouseDownCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerMouseDownCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public ShapeFileLayerMouseDownCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerMouseDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseDown += OnEventRaised;
        }
    }

    // ShapeFileLayerMouseDownCommandBehavior
    public class ShapeFileLayerMouseDownCommandBehavior : ShapeFileLayerMouseDownCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerPreviewMouseUpCommand
    // ShapeFileLayerPreviewMouseUpCommand
    public class ShapeFileLayerPreviewMouseUpCommand : ShapeFileLayerCommandBase<ShapeFileLayerPreviewMouseUpCommandBehavior>
    { }

    public class ShapeFileLayerPreviewMouseUpCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerPreviewMouseUpCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerPreviewMouseUpCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public ShapeFileLayerPreviewMouseUpCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerPreviewMouseUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewMouseUp += OnEventRaised;
        }
    }

    // ShapeFileLayerPreviewMouseUpCommandBehavior
    public class ShapeFileLayerPreviewMouseUpCommandBehavior : ShapeFileLayerPreviewMouseUpCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerMouseUpCommand
    // ShapeFileLayerMouseUpCommand
    public class ShapeFileLayerMouseUpCommand : ShapeFileLayerCommandBase<ShapeFileLayerMouseUpCommandBehavior>
    { }

    public class ShapeFileLayerMouseUpCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerMouseUpCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerMouseUpCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public ShapeFileLayerMouseUpCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerMouseUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseUp += OnEventRaised;
        }
    }

    // ShapeFileLayerMouseUpCommandBehavior
    public class ShapeFileLayerMouseUpCommandBehavior : ShapeFileLayerMouseUpCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerPreviewMouseLeftButtonDownCommand
    // ShapeFileLayerPreviewMouseLeftButtonDownCommand
    public class ShapeFileLayerPreviewMouseLeftButtonDownCommand : ShapeFileLayerCommandBase<ShapeFileLayerPreviewMouseLeftButtonDownCommandBehavior>
    { }

    public class ShapeFileLayerPreviewMouseLeftButtonDownCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerPreviewMouseLeftButtonDownCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerPreviewMouseLeftButtonDownCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public ShapeFileLayerPreviewMouseLeftButtonDownCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerPreviewMouseLeftButtonDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewMouseLeftButtonDown += OnEventRaised;
        }
    }

    // ShapeFileLayerPreviewMouseLeftButtonDownCommandBehavior
    public class ShapeFileLayerPreviewMouseLeftButtonDownCommandBehavior : ShapeFileLayerPreviewMouseLeftButtonDownCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerMouseLeftButtonDownCommand
    // ShapeFileLayerMouseLeftButtonDownCommand
    public class ShapeFileLayerMouseLeftButtonDownCommand : ShapeFileLayerCommandBase<ShapeFileLayerMouseLeftButtonDownCommandBehavior>
    { }

    public class ShapeFileLayerMouseLeftButtonDownCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerMouseLeftButtonDownCommandBehavior<T>, new()
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

    #region ShapeFileLayerPreviewMouseLeftButtonUpCommand
    // ShapeFileLayerPreviewMouseLeftButtonUpCommand
    public class ShapeFileLayerPreviewMouseLeftButtonUpCommand : ShapeFileLayerCommandBase<ShapeFileLayerPreviewMouseLeftButtonUpCommandBehavior>
    { }

    public class ShapeFileLayerPreviewMouseLeftButtonUpCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerPreviewMouseLeftButtonUpCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerPreviewMouseLeftButtonUpCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public ShapeFileLayerPreviewMouseLeftButtonUpCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerPreviewMouseLeftButtonUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewMouseLeftButtonUp += OnEventRaised;
        }
    }

    // ShapeFileLayerPreviewMouseLeftButtonUpCommandBehavior
    public class ShapeFileLayerPreviewMouseLeftButtonUpCommandBehavior : ShapeFileLayerPreviewMouseLeftButtonUpCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerMouseLeftButtonUpCommand
    // ShapeFileLayerMouseLeftButtonUpCommand
    public class ShapeFileLayerMouseLeftButtonUpCommand : ShapeFileLayerCommandBase<ShapeFileLayerMouseLeftButtonUpCommandBehavior>
    { }

    public class ShapeFileLayerMouseLeftButtonUpCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerMouseLeftButtonUpCommandBehavior<T>, new()
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

    #region ShapeFileLayerPreviewMouseRightButtonDownCommand
    // ShapeFileLayerPreviewMouseRightButtonDownCommand
    public class ShapeFileLayerPreviewMouseRightButtonDownCommand : ShapeFileLayerCommandBase<ShapeFileLayerPreviewMouseRightButtonDownCommandBehavior>
    { }

    public class ShapeFileLayerPreviewMouseRightButtonDownCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerPreviewMouseRightButtonDownCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerPreviewMouseRightButtonDownCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public ShapeFileLayerPreviewMouseRightButtonDownCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerPreviewMouseRightButtonDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewMouseRightButtonDown += OnEventRaised;
        }
    }

    // ShapeFileLayerPreviewMouseRightButtonDownCommandBehavior
    public class ShapeFileLayerPreviewMouseRightButtonDownCommandBehavior : ShapeFileLayerPreviewMouseRightButtonDownCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerMouseRightButtonDownCommand
    // ShapeFileLayerMouseRightButtonDownCommand
    public class ShapeFileLayerMouseRightButtonDownCommand : ShapeFileLayerCommandBase<ShapeFileLayerMouseRightButtonDownCommandBehavior>
    { }

    public class ShapeFileLayerMouseRightButtonDownCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerMouseRightButtonDownCommandBehavior<T>, new()
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

    #region ShapeFileLayerPreviewMouseRightButtonUpCommand
    // ShapeFileLayerPreviewMouseRightButtonUpCommand
    public class ShapeFileLayerPreviewMouseRightButtonUpCommand : ShapeFileLayerCommandBase<ShapeFileLayerPreviewMouseRightButtonUpCommandBehavior>
    { }

    public class ShapeFileLayerPreviewMouseRightButtonUpCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerPreviewMouseRightButtonUpCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerPreviewMouseRightButtonUpCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public ShapeFileLayerPreviewMouseRightButtonUpCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerPreviewMouseRightButtonUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewMouseRightButtonUp += OnEventRaised;
        }
    }

    // ShapeFileLayerPreviewMouseRightButtonUpCommandBehavior
    public class ShapeFileLayerPreviewMouseRightButtonUpCommandBehavior : ShapeFileLayerPreviewMouseRightButtonUpCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerMouseRightButtonUpCommand
    // ShapeFileLayerMouseRightButtonUpCommand
    public class ShapeFileLayerMouseRightButtonUpCommand : ShapeFileLayerCommandBase<ShapeFileLayerMouseRightButtonUpCommandBehavior>
    { }

    public class ShapeFileLayerMouseRightButtonUpCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerMouseRightButtonUpCommandBehavior<T>, new()
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

    #region ShapeFileLayerPreviewMouseMoveCommand
    // ShapeFileLayerPreviewMouseMoveCommand
    public class ShapeFileLayerPreviewMouseMoveCommand : ShapeFileLayerCommandBase<ShapeFileLayerPreviewMouseMoveCommandBehavior>
    { }

    public class ShapeFileLayerPreviewMouseMoveCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerPreviewMouseMoveCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerPreviewMouseMoveCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, MouseEventArgs>
    {
        public ShapeFileLayerPreviewMouseMoveCommandBehavior(Func<object, MouseEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerPreviewMouseMoveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewMouseMove += OnEventRaised;
        }
    }

    // ShapeFileLayerPreviewMouseMoveCommandBehavior
    public class ShapeFileLayerPreviewMouseMoveCommandBehavior : ShapeFileLayerPreviewMouseMoveCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerMouseMoveCommand
    // ShapeFileLayerMouseMoveCommand
    public class ShapeFileLayerMouseMoveCommand : ShapeFileLayerCommandBase<ShapeFileLayerMouseMoveCommandBehavior>
    { }

    public class ShapeFileLayerMouseMoveCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerMouseMoveCommandBehavior<T>, new()
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

    #region ShapeFileLayerPreviewMouseWheelCommand
    // ShapeFileLayerPreviewMouseWheelCommand
    public class ShapeFileLayerPreviewMouseWheelCommand : ShapeFileLayerCommandBase<ShapeFileLayerPreviewMouseWheelCommandBehavior>
    { }

    public class ShapeFileLayerPreviewMouseWheelCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerPreviewMouseWheelCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerPreviewMouseWheelCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, MouseWheelEventArgs>
    {
        public ShapeFileLayerPreviewMouseWheelCommandBehavior(Func<object, MouseWheelEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerPreviewMouseWheelCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewMouseWheel += OnEventRaised;
        }
    }

    // ShapeFileLayerPreviewMouseWheelCommandBehavior
    public class ShapeFileLayerPreviewMouseWheelCommandBehavior : ShapeFileLayerPreviewMouseWheelCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerMouseWheelCommand
    // ShapeFileLayerMouseWheelCommand
    public class ShapeFileLayerMouseWheelCommand : ShapeFileLayerCommandBase<ShapeFileLayerMouseWheelCommandBehavior>
    { }

    public class ShapeFileLayerMouseWheelCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerMouseWheelCommandBehavior<T>, new()
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

    #region ShapeFileLayerMouseEnterCommand
    // ShapeFileLayerMouseEnterCommand
    public class ShapeFileLayerMouseEnterCommand : ShapeFileLayerCommandBase<ShapeFileLayerMouseEnterCommandBehavior>
    { }

    public class ShapeFileLayerMouseEnterCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerMouseEnterCommandBehavior<T>, new()
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

    public class ShapeFileLayerMouseLeaveCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerMouseLeaveCommandBehavior<T>, new()
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

    #region ShapeFileLayerGotMouseCaptureCommand
    // ShapeFileLayerGotMouseCaptureCommand
    public class ShapeFileLayerGotMouseCaptureCommand : ShapeFileLayerCommandBase<ShapeFileLayerGotMouseCaptureCommandBehavior>
    { }

    public class ShapeFileLayerGotMouseCaptureCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerGotMouseCaptureCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerGotMouseCaptureCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, MouseEventArgs>
    {
        public ShapeFileLayerGotMouseCaptureCommandBehavior(Func<object, MouseEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerGotMouseCaptureCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.GotMouseCapture += OnEventRaised;
        }
    }

    // ShapeFileLayerGotMouseCaptureCommandBehavior
    public class ShapeFileLayerGotMouseCaptureCommandBehavior : ShapeFileLayerGotMouseCaptureCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerLostMouseCaptureCommand
    // ShapeFileLayerLostMouseCaptureCommand
    public class ShapeFileLayerLostMouseCaptureCommand : ShapeFileLayerCommandBase<ShapeFileLayerLostMouseCaptureCommandBehavior>
    { }

    public class ShapeFileLayerLostMouseCaptureCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerLostMouseCaptureCommandBehavior<T>, new()
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

    #region ShapeFileLayerQueryCursorCommand
    // ShapeFileLayerQueryCursorCommand
    public class ShapeFileLayerQueryCursorCommand : ShapeFileLayerCommandBase<ShapeFileLayerQueryCursorCommandBehavior>
    { }

    public class ShapeFileLayerQueryCursorCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerQueryCursorCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerQueryCursorCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, QueryCursorEventArgs>
    {
        public ShapeFileLayerQueryCursorCommandBehavior(Func<object, QueryCursorEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerQueryCursorCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.QueryCursor += OnEventRaised;
        }
    }

    // ShapeFileLayerQueryCursorCommandBehavior
    public class ShapeFileLayerQueryCursorCommandBehavior : ShapeFileLayerQueryCursorCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerPreviewStylusDownCommand
    // ShapeFileLayerPreviewStylusDownCommand
    public class ShapeFileLayerPreviewStylusDownCommand : ShapeFileLayerCommandBase<ShapeFileLayerPreviewStylusDownCommandBehavior>
    { }

    public class ShapeFileLayerPreviewStylusDownCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerPreviewStylusDownCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerPreviewStylusDownCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, StylusDownEventArgs>
    {
        public ShapeFileLayerPreviewStylusDownCommandBehavior(Func<object, StylusDownEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerPreviewStylusDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewStylusDown += OnEventRaised;
        }
    }

    // ShapeFileLayerPreviewStylusDownCommandBehavior
    public class ShapeFileLayerPreviewStylusDownCommandBehavior : ShapeFileLayerPreviewStylusDownCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerStylusDownCommand
    // ShapeFileLayerStylusDownCommand
    public class ShapeFileLayerStylusDownCommand : ShapeFileLayerCommandBase<ShapeFileLayerStylusDownCommandBehavior>
    { }

    public class ShapeFileLayerStylusDownCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerStylusDownCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerStylusDownCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, StylusDownEventArgs>
    {
        public ShapeFileLayerStylusDownCommandBehavior(Func<object, StylusDownEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerStylusDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusDown += OnEventRaised;
        }
    }

    // ShapeFileLayerStylusDownCommandBehavior
    public class ShapeFileLayerStylusDownCommandBehavior : ShapeFileLayerStylusDownCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerPreviewStylusUpCommand
    // ShapeFileLayerPreviewStylusUpCommand
    public class ShapeFileLayerPreviewStylusUpCommand : ShapeFileLayerCommandBase<ShapeFileLayerPreviewStylusUpCommandBehavior>
    { }

    public class ShapeFileLayerPreviewStylusUpCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerPreviewStylusUpCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerPreviewStylusUpCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public ShapeFileLayerPreviewStylusUpCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerPreviewStylusUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewStylusUp += OnEventRaised;
        }
    }

    // ShapeFileLayerPreviewStylusUpCommandBehavior
    public class ShapeFileLayerPreviewStylusUpCommandBehavior : ShapeFileLayerPreviewStylusUpCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerStylusUpCommand
    // ShapeFileLayerStylusUpCommand
    public class ShapeFileLayerStylusUpCommand : ShapeFileLayerCommandBase<ShapeFileLayerStylusUpCommandBehavior>
    { }

    public class ShapeFileLayerStylusUpCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerStylusUpCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerStylusUpCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public ShapeFileLayerStylusUpCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerStylusUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusUp += OnEventRaised;
        }
    }

    // ShapeFileLayerStylusUpCommandBehavior
    public class ShapeFileLayerStylusUpCommandBehavior : ShapeFileLayerStylusUpCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerPreviewStylusMoveCommand
    // ShapeFileLayerPreviewStylusMoveCommand
    public class ShapeFileLayerPreviewStylusMoveCommand : ShapeFileLayerCommandBase<ShapeFileLayerPreviewStylusMoveCommandBehavior>
    { }

    public class ShapeFileLayerPreviewStylusMoveCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerPreviewStylusMoveCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerPreviewStylusMoveCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public ShapeFileLayerPreviewStylusMoveCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerPreviewStylusMoveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewStylusMove += OnEventRaised;
        }
    }

    // ShapeFileLayerPreviewStylusMoveCommandBehavior
    public class ShapeFileLayerPreviewStylusMoveCommandBehavior : ShapeFileLayerPreviewStylusMoveCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerStylusMoveCommand
    // ShapeFileLayerStylusMoveCommand
    public class ShapeFileLayerStylusMoveCommand : ShapeFileLayerCommandBase<ShapeFileLayerStylusMoveCommandBehavior>
    { }

    public class ShapeFileLayerStylusMoveCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerStylusMoveCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerStylusMoveCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public ShapeFileLayerStylusMoveCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerStylusMoveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusMove += OnEventRaised;
        }
    }

    // ShapeFileLayerStylusMoveCommandBehavior
    public class ShapeFileLayerStylusMoveCommandBehavior : ShapeFileLayerStylusMoveCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerPreviewStylusInAirMoveCommand
    // ShapeFileLayerPreviewStylusInAirMoveCommand
    public class ShapeFileLayerPreviewStylusInAirMoveCommand : ShapeFileLayerCommandBase<ShapeFileLayerPreviewStylusInAirMoveCommandBehavior>
    { }

    public class ShapeFileLayerPreviewStylusInAirMoveCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerPreviewStylusInAirMoveCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerPreviewStylusInAirMoveCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public ShapeFileLayerPreviewStylusInAirMoveCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerPreviewStylusInAirMoveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewStylusInAirMove += OnEventRaised;
        }
    }

    // ShapeFileLayerPreviewStylusInAirMoveCommandBehavior
    public class ShapeFileLayerPreviewStylusInAirMoveCommandBehavior : ShapeFileLayerPreviewStylusInAirMoveCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerStylusInAirMoveCommand
    // ShapeFileLayerStylusInAirMoveCommand
    public class ShapeFileLayerStylusInAirMoveCommand : ShapeFileLayerCommandBase<ShapeFileLayerStylusInAirMoveCommandBehavior>
    { }

    public class ShapeFileLayerStylusInAirMoveCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerStylusInAirMoveCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerStylusInAirMoveCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public ShapeFileLayerStylusInAirMoveCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerStylusInAirMoveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusInAirMove += OnEventRaised;
        }
    }

    // ShapeFileLayerStylusInAirMoveCommandBehavior
    public class ShapeFileLayerStylusInAirMoveCommandBehavior : ShapeFileLayerStylusInAirMoveCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerStylusEnterCommand
    // ShapeFileLayerStylusEnterCommand
    public class ShapeFileLayerStylusEnterCommand : ShapeFileLayerCommandBase<ShapeFileLayerStylusEnterCommandBehavior>
    { }

    public class ShapeFileLayerStylusEnterCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerStylusEnterCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerStylusEnterCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public ShapeFileLayerStylusEnterCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerStylusEnterCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusEnter += OnEventRaised;
        }
    }

    // ShapeFileLayerStylusEnterCommandBehavior
    public class ShapeFileLayerStylusEnterCommandBehavior : ShapeFileLayerStylusEnterCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerStylusLeaveCommand
    // ShapeFileLayerStylusLeaveCommand
    public class ShapeFileLayerStylusLeaveCommand : ShapeFileLayerCommandBase<ShapeFileLayerStylusLeaveCommandBehavior>
    { }

    public class ShapeFileLayerStylusLeaveCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerStylusLeaveCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerStylusLeaveCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public ShapeFileLayerStylusLeaveCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerStylusLeaveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusLeave += OnEventRaised;
        }
    }

    // ShapeFileLayerStylusLeaveCommandBehavior
    public class ShapeFileLayerStylusLeaveCommandBehavior : ShapeFileLayerStylusLeaveCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerPreviewStylusInRangeCommand
    // ShapeFileLayerPreviewStylusInRangeCommand
    public class ShapeFileLayerPreviewStylusInRangeCommand : ShapeFileLayerCommandBase<ShapeFileLayerPreviewStylusInRangeCommandBehavior>
    { }

    public class ShapeFileLayerPreviewStylusInRangeCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerPreviewStylusInRangeCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerPreviewStylusInRangeCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public ShapeFileLayerPreviewStylusInRangeCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerPreviewStylusInRangeCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewStylusInRange += OnEventRaised;
        }
    }

    // ShapeFileLayerPreviewStylusInRangeCommandBehavior
    public class ShapeFileLayerPreviewStylusInRangeCommandBehavior : ShapeFileLayerPreviewStylusInRangeCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerStylusInRangeCommand
    // ShapeFileLayerStylusInRangeCommand
    public class ShapeFileLayerStylusInRangeCommand : ShapeFileLayerCommandBase<ShapeFileLayerStylusInRangeCommandBehavior>
    { }

    public class ShapeFileLayerStylusInRangeCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerStylusInRangeCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerStylusInRangeCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public ShapeFileLayerStylusInRangeCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerStylusInRangeCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusInRange += OnEventRaised;
        }
    }

    // ShapeFileLayerStylusInRangeCommandBehavior
    public class ShapeFileLayerStylusInRangeCommandBehavior : ShapeFileLayerStylusInRangeCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerPreviewStylusOutOfRangeCommand
    // ShapeFileLayerPreviewStylusOutOfRangeCommand
    public class ShapeFileLayerPreviewStylusOutOfRangeCommand : ShapeFileLayerCommandBase<ShapeFileLayerPreviewStylusOutOfRangeCommandBehavior>
    { }

    public class ShapeFileLayerPreviewStylusOutOfRangeCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerPreviewStylusOutOfRangeCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerPreviewStylusOutOfRangeCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public ShapeFileLayerPreviewStylusOutOfRangeCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerPreviewStylusOutOfRangeCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewStylusOutOfRange += OnEventRaised;
        }
    }

    // ShapeFileLayerPreviewStylusOutOfRangeCommandBehavior
    public class ShapeFileLayerPreviewStylusOutOfRangeCommandBehavior : ShapeFileLayerPreviewStylusOutOfRangeCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerStylusOutOfRangeCommand
    // ShapeFileLayerStylusOutOfRangeCommand
    public class ShapeFileLayerStylusOutOfRangeCommand : ShapeFileLayerCommandBase<ShapeFileLayerStylusOutOfRangeCommandBehavior>
    { }

    public class ShapeFileLayerStylusOutOfRangeCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerStylusOutOfRangeCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerStylusOutOfRangeCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public ShapeFileLayerStylusOutOfRangeCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerStylusOutOfRangeCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusOutOfRange += OnEventRaised;
        }
    }

    // ShapeFileLayerStylusOutOfRangeCommandBehavior
    public class ShapeFileLayerStylusOutOfRangeCommandBehavior : ShapeFileLayerStylusOutOfRangeCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerPreviewStylusSystemGestureCommand
    // ShapeFileLayerPreviewStylusSystemGestureCommand
    public class ShapeFileLayerPreviewStylusSystemGestureCommand : ShapeFileLayerCommandBase<ShapeFileLayerPreviewStylusSystemGestureCommandBehavior>
    { }

    public class ShapeFileLayerPreviewStylusSystemGestureCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerPreviewStylusSystemGestureCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerPreviewStylusSystemGestureCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, StylusSystemGestureEventArgs>
    {
        public ShapeFileLayerPreviewStylusSystemGestureCommandBehavior(Func<object, StylusSystemGestureEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerPreviewStylusSystemGestureCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewStylusSystemGesture += OnEventRaised;
        }
    }

    // ShapeFileLayerPreviewStylusSystemGestureCommandBehavior
    public class ShapeFileLayerPreviewStylusSystemGestureCommandBehavior : ShapeFileLayerPreviewStylusSystemGestureCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerStylusSystemGestureCommand
    // ShapeFileLayerStylusSystemGestureCommand
    public class ShapeFileLayerStylusSystemGestureCommand : ShapeFileLayerCommandBase<ShapeFileLayerStylusSystemGestureCommandBehavior>
    { }

    public class ShapeFileLayerStylusSystemGestureCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerStylusSystemGestureCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerStylusSystemGestureCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, StylusSystemGestureEventArgs>
    {
        public ShapeFileLayerStylusSystemGestureCommandBehavior(Func<object, StylusSystemGestureEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerStylusSystemGestureCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusSystemGesture += OnEventRaised;
        }
    }

    // ShapeFileLayerStylusSystemGestureCommandBehavior
    public class ShapeFileLayerStylusSystemGestureCommandBehavior : ShapeFileLayerStylusSystemGestureCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerGotStylusCaptureCommand
    // ShapeFileLayerGotStylusCaptureCommand
    public class ShapeFileLayerGotStylusCaptureCommand : ShapeFileLayerCommandBase<ShapeFileLayerGotStylusCaptureCommandBehavior>
    { }

    public class ShapeFileLayerGotStylusCaptureCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerGotStylusCaptureCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerGotStylusCaptureCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public ShapeFileLayerGotStylusCaptureCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerGotStylusCaptureCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.GotStylusCapture += OnEventRaised;
        }
    }

    // ShapeFileLayerGotStylusCaptureCommandBehavior
    public class ShapeFileLayerGotStylusCaptureCommandBehavior : ShapeFileLayerGotStylusCaptureCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerLostStylusCaptureCommand
    // ShapeFileLayerLostStylusCaptureCommand
    public class ShapeFileLayerLostStylusCaptureCommand : ShapeFileLayerCommandBase<ShapeFileLayerLostStylusCaptureCommandBehavior>
    { }

    public class ShapeFileLayerLostStylusCaptureCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerLostStylusCaptureCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerLostStylusCaptureCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public ShapeFileLayerLostStylusCaptureCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerLostStylusCaptureCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.LostStylusCapture += OnEventRaised;
        }
    }

    // ShapeFileLayerLostStylusCaptureCommandBehavior
    public class ShapeFileLayerLostStylusCaptureCommandBehavior : ShapeFileLayerLostStylusCaptureCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerStylusButtonDownCommand
    // ShapeFileLayerStylusButtonDownCommand
    public class ShapeFileLayerStylusButtonDownCommand : ShapeFileLayerCommandBase<ShapeFileLayerStylusButtonDownCommandBehavior>
    { }

    public class ShapeFileLayerStylusButtonDownCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerStylusButtonDownCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerStylusButtonDownCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, StylusButtonEventArgs>
    {
        public ShapeFileLayerStylusButtonDownCommandBehavior(Func<object, StylusButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerStylusButtonDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusButtonDown += OnEventRaised;
        }
    }

    // ShapeFileLayerStylusButtonDownCommandBehavior
    public class ShapeFileLayerStylusButtonDownCommandBehavior : ShapeFileLayerStylusButtonDownCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerStylusButtonUpCommand
    // ShapeFileLayerStylusButtonUpCommand
    public class ShapeFileLayerStylusButtonUpCommand : ShapeFileLayerCommandBase<ShapeFileLayerStylusButtonUpCommandBehavior>
    { }

    public class ShapeFileLayerStylusButtonUpCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerStylusButtonUpCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerStylusButtonUpCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, StylusButtonEventArgs>
    {
        public ShapeFileLayerStylusButtonUpCommandBehavior(Func<object, StylusButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerStylusButtonUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusButtonUp += OnEventRaised;
        }
    }

    // ShapeFileLayerStylusButtonUpCommandBehavior
    public class ShapeFileLayerStylusButtonUpCommandBehavior : ShapeFileLayerStylusButtonUpCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerPreviewStylusButtonDownCommand
    // ShapeFileLayerPreviewStylusButtonDownCommand
    public class ShapeFileLayerPreviewStylusButtonDownCommand : ShapeFileLayerCommandBase<ShapeFileLayerPreviewStylusButtonDownCommandBehavior>
    { }

    public class ShapeFileLayerPreviewStylusButtonDownCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerPreviewStylusButtonDownCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerPreviewStylusButtonDownCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, StylusButtonEventArgs>
    {
        public ShapeFileLayerPreviewStylusButtonDownCommandBehavior(Func<object, StylusButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerPreviewStylusButtonDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewStylusButtonDown += OnEventRaised;
        }
    }

    // ShapeFileLayerPreviewStylusButtonDownCommandBehavior
    public class ShapeFileLayerPreviewStylusButtonDownCommandBehavior : ShapeFileLayerPreviewStylusButtonDownCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerPreviewStylusButtonUpCommand
    // ShapeFileLayerPreviewStylusButtonUpCommand
    public class ShapeFileLayerPreviewStylusButtonUpCommand : ShapeFileLayerCommandBase<ShapeFileLayerPreviewStylusButtonUpCommandBehavior>
    { }

    public class ShapeFileLayerPreviewStylusButtonUpCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerPreviewStylusButtonUpCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerPreviewStylusButtonUpCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, StylusButtonEventArgs>
    {
        public ShapeFileLayerPreviewStylusButtonUpCommandBehavior(Func<object, StylusButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerPreviewStylusButtonUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewStylusButtonUp += OnEventRaised;
        }
    }

    // ShapeFileLayerPreviewStylusButtonUpCommandBehavior
    public class ShapeFileLayerPreviewStylusButtonUpCommandBehavior : ShapeFileLayerPreviewStylusButtonUpCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerPreviewKeyDownCommand
    // ShapeFileLayerPreviewKeyDownCommand
    public class ShapeFileLayerPreviewKeyDownCommand : ShapeFileLayerCommandBase<ShapeFileLayerPreviewKeyDownCommandBehavior>
    { }

    public class ShapeFileLayerPreviewKeyDownCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerPreviewKeyDownCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerPreviewKeyDownCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, KeyEventArgs>
    {
        public ShapeFileLayerPreviewKeyDownCommandBehavior(Func<object, KeyEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerPreviewKeyDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewKeyDown += OnEventRaised;
        }
    }

    // ShapeFileLayerPreviewKeyDownCommandBehavior
    public class ShapeFileLayerPreviewKeyDownCommandBehavior : ShapeFileLayerPreviewKeyDownCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerKeyDownCommand
    // ShapeFileLayerKeyDownCommand
    public class ShapeFileLayerKeyDownCommand : ShapeFileLayerCommandBase<ShapeFileLayerKeyDownCommandBehavior>
    { }

    public class ShapeFileLayerKeyDownCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerKeyDownCommandBehavior<T>, new()
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

    #region ShapeFileLayerPreviewKeyUpCommand
    // ShapeFileLayerPreviewKeyUpCommand
    public class ShapeFileLayerPreviewKeyUpCommand : ShapeFileLayerCommandBase<ShapeFileLayerPreviewKeyUpCommandBehavior>
    { }

    public class ShapeFileLayerPreviewKeyUpCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerPreviewKeyUpCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerPreviewKeyUpCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, KeyEventArgs>
    {
        public ShapeFileLayerPreviewKeyUpCommandBehavior(Func<object, KeyEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerPreviewKeyUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewKeyUp += OnEventRaised;
        }
    }

    // ShapeFileLayerPreviewKeyUpCommandBehavior
    public class ShapeFileLayerPreviewKeyUpCommandBehavior : ShapeFileLayerPreviewKeyUpCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerKeyUpCommand
    // ShapeFileLayerKeyUpCommand
    public class ShapeFileLayerKeyUpCommand : ShapeFileLayerCommandBase<ShapeFileLayerKeyUpCommandBehavior>
    { }

    public class ShapeFileLayerKeyUpCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerKeyUpCommandBehavior<T>, new()
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

    #region ShapeFileLayerPreviewGotKeyboardFocusCommand
    // ShapeFileLayerPreviewGotKeyboardFocusCommand
    public class ShapeFileLayerPreviewGotKeyboardFocusCommand : ShapeFileLayerCommandBase<ShapeFileLayerPreviewGotKeyboardFocusCommandBehavior>
    { }

    public class ShapeFileLayerPreviewGotKeyboardFocusCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerPreviewGotKeyboardFocusCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerPreviewGotKeyboardFocusCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, KeyboardFocusChangedEventArgs>
    {
        public ShapeFileLayerPreviewGotKeyboardFocusCommandBehavior(Func<object, KeyboardFocusChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerPreviewGotKeyboardFocusCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewGotKeyboardFocus += OnEventRaised;
        }
    }

    // ShapeFileLayerPreviewGotKeyboardFocusCommandBehavior
    public class ShapeFileLayerPreviewGotKeyboardFocusCommandBehavior : ShapeFileLayerPreviewGotKeyboardFocusCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerGotKeyboardFocusCommand
    // ShapeFileLayerGotKeyboardFocusCommand
    public class ShapeFileLayerGotKeyboardFocusCommand : ShapeFileLayerCommandBase<ShapeFileLayerGotKeyboardFocusCommandBehavior>
    { }

    public class ShapeFileLayerGotKeyboardFocusCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerGotKeyboardFocusCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerGotKeyboardFocusCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, KeyboardFocusChangedEventArgs>
    {
        public ShapeFileLayerGotKeyboardFocusCommandBehavior(Func<object, KeyboardFocusChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerGotKeyboardFocusCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.GotKeyboardFocus += OnEventRaised;
        }
    }

    // ShapeFileLayerGotKeyboardFocusCommandBehavior
    public class ShapeFileLayerGotKeyboardFocusCommandBehavior : ShapeFileLayerGotKeyboardFocusCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerPreviewLostKeyboardFocusCommand
    // ShapeFileLayerPreviewLostKeyboardFocusCommand
    public class ShapeFileLayerPreviewLostKeyboardFocusCommand : ShapeFileLayerCommandBase<ShapeFileLayerPreviewLostKeyboardFocusCommandBehavior>
    { }

    public class ShapeFileLayerPreviewLostKeyboardFocusCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerPreviewLostKeyboardFocusCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerPreviewLostKeyboardFocusCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, KeyboardFocusChangedEventArgs>
    {
        public ShapeFileLayerPreviewLostKeyboardFocusCommandBehavior(Func<object, KeyboardFocusChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerPreviewLostKeyboardFocusCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewLostKeyboardFocus += OnEventRaised;
        }
    }

    // ShapeFileLayerPreviewLostKeyboardFocusCommandBehavior
    public class ShapeFileLayerPreviewLostKeyboardFocusCommandBehavior : ShapeFileLayerPreviewLostKeyboardFocusCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerLostKeyboardFocusCommand
    // ShapeFileLayerLostKeyboardFocusCommand
    public class ShapeFileLayerLostKeyboardFocusCommand : ShapeFileLayerCommandBase<ShapeFileLayerLostKeyboardFocusCommandBehavior>
    { }

    public class ShapeFileLayerLostKeyboardFocusCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerLostKeyboardFocusCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerLostKeyboardFocusCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, KeyboardFocusChangedEventArgs>
    {
        public ShapeFileLayerLostKeyboardFocusCommandBehavior(Func<object, KeyboardFocusChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerLostKeyboardFocusCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.LostKeyboardFocus += OnEventRaised;
        }
    }

    // ShapeFileLayerLostKeyboardFocusCommandBehavior
    public class ShapeFileLayerLostKeyboardFocusCommandBehavior : ShapeFileLayerLostKeyboardFocusCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerPreviewTextInputCommand
    // ShapeFileLayerPreviewTextInputCommand
    public class ShapeFileLayerPreviewTextInputCommand : ShapeFileLayerCommandBase<ShapeFileLayerPreviewTextInputCommandBehavior>
    { }

    public class ShapeFileLayerPreviewTextInputCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerPreviewTextInputCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerPreviewTextInputCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, TextCompositionEventArgs>
    {
        public ShapeFileLayerPreviewTextInputCommandBehavior(Func<object, TextCompositionEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerPreviewTextInputCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewTextInput += OnEventRaised;
        }
    }

    // ShapeFileLayerPreviewTextInputCommandBehavior
    public class ShapeFileLayerPreviewTextInputCommandBehavior : ShapeFileLayerPreviewTextInputCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerTextInputCommand
    // ShapeFileLayerTextInputCommand
    public class ShapeFileLayerTextInputCommand : ShapeFileLayerCommandBase<ShapeFileLayerTextInputCommandBehavior>
    { }

    public class ShapeFileLayerTextInputCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerTextInputCommandBehavior<T>, new()
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

    #region ShapeFileLayerPreviewQueryContinueDragCommand
    // ShapeFileLayerPreviewQueryContinueDragCommand
    public class ShapeFileLayerPreviewQueryContinueDragCommand : ShapeFileLayerCommandBase<ShapeFileLayerPreviewQueryContinueDragCommandBehavior>
    { }

    public class ShapeFileLayerPreviewQueryContinueDragCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerPreviewQueryContinueDragCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerPreviewQueryContinueDragCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, QueryContinueDragEventArgs>
    {
        public ShapeFileLayerPreviewQueryContinueDragCommandBehavior(Func<object, QueryContinueDragEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerPreviewQueryContinueDragCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewQueryContinueDrag += OnEventRaised;
        }
    }

    // ShapeFileLayerPreviewQueryContinueDragCommandBehavior
    public class ShapeFileLayerPreviewQueryContinueDragCommandBehavior : ShapeFileLayerPreviewQueryContinueDragCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerQueryContinueDragCommand
    // ShapeFileLayerQueryContinueDragCommand
    public class ShapeFileLayerQueryContinueDragCommand : ShapeFileLayerCommandBase<ShapeFileLayerQueryContinueDragCommandBehavior>
    { }

    public class ShapeFileLayerQueryContinueDragCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerQueryContinueDragCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerQueryContinueDragCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, QueryContinueDragEventArgs>
    {
        public ShapeFileLayerQueryContinueDragCommandBehavior(Func<object, QueryContinueDragEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerQueryContinueDragCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.QueryContinueDrag += OnEventRaised;
        }
    }

    // ShapeFileLayerQueryContinueDragCommandBehavior
    public class ShapeFileLayerQueryContinueDragCommandBehavior : ShapeFileLayerQueryContinueDragCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerPreviewGiveFeedbackCommand
    // ShapeFileLayerPreviewGiveFeedbackCommand
    public class ShapeFileLayerPreviewGiveFeedbackCommand : ShapeFileLayerCommandBase<ShapeFileLayerPreviewGiveFeedbackCommandBehavior>
    { }

    public class ShapeFileLayerPreviewGiveFeedbackCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerPreviewGiveFeedbackCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerPreviewGiveFeedbackCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, GiveFeedbackEventArgs>
    {
        public ShapeFileLayerPreviewGiveFeedbackCommandBehavior(Func<object, GiveFeedbackEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerPreviewGiveFeedbackCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewGiveFeedback += OnEventRaised;
        }
    }

    // ShapeFileLayerPreviewGiveFeedbackCommandBehavior
    public class ShapeFileLayerPreviewGiveFeedbackCommandBehavior : ShapeFileLayerPreviewGiveFeedbackCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerGiveFeedbackCommand
    // ShapeFileLayerGiveFeedbackCommand
    public class ShapeFileLayerGiveFeedbackCommand : ShapeFileLayerCommandBase<ShapeFileLayerGiveFeedbackCommandBehavior>
    { }

    public class ShapeFileLayerGiveFeedbackCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerGiveFeedbackCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerGiveFeedbackCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, GiveFeedbackEventArgs>
    {
        public ShapeFileLayerGiveFeedbackCommandBehavior(Func<object, GiveFeedbackEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerGiveFeedbackCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.GiveFeedback += OnEventRaised;
        }
    }

    // ShapeFileLayerGiveFeedbackCommandBehavior
    public class ShapeFileLayerGiveFeedbackCommandBehavior : ShapeFileLayerGiveFeedbackCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerPreviewDragEnterCommand
    // ShapeFileLayerPreviewDragEnterCommand
    public class ShapeFileLayerPreviewDragEnterCommand : ShapeFileLayerCommandBase<ShapeFileLayerPreviewDragEnterCommandBehavior>
    { }

    public class ShapeFileLayerPreviewDragEnterCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerPreviewDragEnterCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerPreviewDragEnterCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, DragEventArgs>
    {
        public ShapeFileLayerPreviewDragEnterCommandBehavior(Func<object, DragEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerPreviewDragEnterCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewDragEnter += OnEventRaised;
        }
    }

    // ShapeFileLayerPreviewDragEnterCommandBehavior
    public class ShapeFileLayerPreviewDragEnterCommandBehavior : ShapeFileLayerPreviewDragEnterCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerDragEnterCommand
    // ShapeFileLayerDragEnterCommand
    public class ShapeFileLayerDragEnterCommand : ShapeFileLayerCommandBase<ShapeFileLayerDragEnterCommandBehavior>
    { }

    public class ShapeFileLayerDragEnterCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerDragEnterCommandBehavior<T>, new()
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

    #region ShapeFileLayerPreviewDragOverCommand
    // ShapeFileLayerPreviewDragOverCommand
    public class ShapeFileLayerPreviewDragOverCommand : ShapeFileLayerCommandBase<ShapeFileLayerPreviewDragOverCommandBehavior>
    { }

    public class ShapeFileLayerPreviewDragOverCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerPreviewDragOverCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerPreviewDragOverCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, DragEventArgs>
    {
        public ShapeFileLayerPreviewDragOverCommandBehavior(Func<object, DragEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerPreviewDragOverCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewDragOver += OnEventRaised;
        }
    }

    // ShapeFileLayerPreviewDragOverCommandBehavior
    public class ShapeFileLayerPreviewDragOverCommandBehavior : ShapeFileLayerPreviewDragOverCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerDragOverCommand
    // ShapeFileLayerDragOverCommand
    public class ShapeFileLayerDragOverCommand : ShapeFileLayerCommandBase<ShapeFileLayerDragOverCommandBehavior>
    { }

    public class ShapeFileLayerDragOverCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerDragOverCommandBehavior<T>, new()
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

    #region ShapeFileLayerPreviewDragLeaveCommand
    // ShapeFileLayerPreviewDragLeaveCommand
    public class ShapeFileLayerPreviewDragLeaveCommand : ShapeFileLayerCommandBase<ShapeFileLayerPreviewDragLeaveCommandBehavior>
    { }

    public class ShapeFileLayerPreviewDragLeaveCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerPreviewDragLeaveCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerPreviewDragLeaveCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, DragEventArgs>
    {
        public ShapeFileLayerPreviewDragLeaveCommandBehavior(Func<object, DragEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerPreviewDragLeaveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewDragLeave += OnEventRaised;
        }
    }

    // ShapeFileLayerPreviewDragLeaveCommandBehavior
    public class ShapeFileLayerPreviewDragLeaveCommandBehavior : ShapeFileLayerPreviewDragLeaveCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerDragLeaveCommand
    // ShapeFileLayerDragLeaveCommand
    public class ShapeFileLayerDragLeaveCommand : ShapeFileLayerCommandBase<ShapeFileLayerDragLeaveCommandBehavior>
    { }

    public class ShapeFileLayerDragLeaveCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerDragLeaveCommandBehavior<T>, new()
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

    #region ShapeFileLayerPreviewDropCommand
    // ShapeFileLayerPreviewDropCommand
    public class ShapeFileLayerPreviewDropCommand : ShapeFileLayerCommandBase<ShapeFileLayerPreviewDropCommandBehavior>
    { }

    public class ShapeFileLayerPreviewDropCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerPreviewDropCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerPreviewDropCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, DragEventArgs>
    {
        public ShapeFileLayerPreviewDropCommandBehavior(Func<object, DragEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerPreviewDropCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewDrop += OnEventRaised;
        }
    }

    // ShapeFileLayerPreviewDropCommandBehavior
    public class ShapeFileLayerPreviewDropCommandBehavior : ShapeFileLayerPreviewDropCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerDropCommand
    // ShapeFileLayerDropCommand
    public class ShapeFileLayerDropCommand : ShapeFileLayerCommandBase<ShapeFileLayerDropCommandBehavior>
    { }

    public class ShapeFileLayerDropCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerDropCommandBehavior<T>, new()
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

#if SyncfusionFramework4_0

    #region ShapeFileLayerPreviewTouchDownCommand
    // ShapeFileLayerPreviewTouchDownCommand
    public class ShapeFileLayerPreviewTouchDownCommand : ShapeFileLayerCommandBase<ShapeFileLayerPreviewTouchDownCommandBehavior>
    { }

    public class ShapeFileLayerPreviewTouchDownCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerPreviewTouchDownCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerPreviewTouchDownCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, TouchEventArgs>
    {
        public ShapeFileLayerPreviewTouchDownCommandBehavior(Func<object, TouchEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerPreviewTouchDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewTouchDown += OnEventRaised;
        }
    }

    // ShapeFileLayerPreviewTouchDownCommandBehavior
    public class ShapeFileLayerPreviewTouchDownCommandBehavior : ShapeFileLayerPreviewTouchDownCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerTouchDownCommand
    // ShapeFileLayerTouchDownCommand
    public class ShapeFileLayerTouchDownCommand : ShapeFileLayerCommandBase<ShapeFileLayerTouchDownCommandBehavior>
    { }

    public class ShapeFileLayerTouchDownCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerTouchDownCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerTouchDownCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, TouchEventArgs>
    {
        public ShapeFileLayerTouchDownCommandBehavior(Func<object, TouchEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerTouchDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.TouchDown += OnEventRaised;
        }
    }

    // ShapeFileLayerTouchDownCommandBehavior
    public class ShapeFileLayerTouchDownCommandBehavior : ShapeFileLayerTouchDownCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerPreviewTouchMoveCommand
    // ShapeFileLayerPreviewTouchMoveCommand
    public class ShapeFileLayerPreviewTouchMoveCommand : ShapeFileLayerCommandBase<ShapeFileLayerPreviewTouchMoveCommandBehavior>
    { }

    public class ShapeFileLayerPreviewTouchMoveCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerPreviewTouchMoveCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerPreviewTouchMoveCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, TouchEventArgs>
    {
        public ShapeFileLayerPreviewTouchMoveCommandBehavior(Func<object, TouchEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerPreviewTouchMoveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewTouchMove += OnEventRaised;
        }
    }

    // ShapeFileLayerPreviewTouchMoveCommandBehavior
    public class ShapeFileLayerPreviewTouchMoveCommandBehavior : ShapeFileLayerPreviewTouchMoveCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerTouchMoveCommand
    // ShapeFileLayerTouchMoveCommand
    public class ShapeFileLayerTouchMoveCommand : ShapeFileLayerCommandBase<ShapeFileLayerTouchMoveCommandBehavior>
    { }

    public class ShapeFileLayerTouchMoveCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerTouchMoveCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerTouchMoveCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, TouchEventArgs>
    {
        public ShapeFileLayerTouchMoveCommandBehavior(Func<object, TouchEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerTouchMoveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.TouchMove += OnEventRaised;
        }
    }

    // ShapeFileLayerTouchMoveCommandBehavior
    public class ShapeFileLayerTouchMoveCommandBehavior : ShapeFileLayerTouchMoveCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerPreviewTouchUpCommand
    // ShapeFileLayerPreviewTouchUpCommand
    public class ShapeFileLayerPreviewTouchUpCommand : ShapeFileLayerCommandBase<ShapeFileLayerPreviewTouchUpCommandBehavior>
    { }

    public class ShapeFileLayerPreviewTouchUpCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerPreviewTouchUpCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerPreviewTouchUpCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, TouchEventArgs>
    {
        public ShapeFileLayerPreviewTouchUpCommandBehavior(Func<object, TouchEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerPreviewTouchUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewTouchUp += OnEventRaised;
        }
    }

    // ShapeFileLayerPreviewTouchUpCommandBehavior
    public class ShapeFileLayerPreviewTouchUpCommandBehavior : ShapeFileLayerPreviewTouchUpCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerTouchUpCommand
    // ShapeFileLayerTouchUpCommand
    public class ShapeFileLayerTouchUpCommand : ShapeFileLayerCommandBase<ShapeFileLayerTouchUpCommandBehavior>
    { }

    public class ShapeFileLayerTouchUpCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerTouchUpCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerTouchUpCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, TouchEventArgs>
    {
        public ShapeFileLayerTouchUpCommandBehavior(Func<object, TouchEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerTouchUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.TouchUp += OnEventRaised;
        }
    }

    // ShapeFileLayerTouchUpCommandBehavior
    public class ShapeFileLayerTouchUpCommandBehavior : ShapeFileLayerTouchUpCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerGotTouchCaptureCommand
    // ShapeFileLayerGotTouchCaptureCommand
    public class ShapeFileLayerGotTouchCaptureCommand : ShapeFileLayerCommandBase<ShapeFileLayerGotTouchCaptureCommandBehavior>
    { }

    public class ShapeFileLayerGotTouchCaptureCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerGotTouchCaptureCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerGotTouchCaptureCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, TouchEventArgs>
    {
        public ShapeFileLayerGotTouchCaptureCommandBehavior(Func<object, TouchEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerGotTouchCaptureCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.GotTouchCapture += OnEventRaised;
        }
    }

    // ShapeFileLayerGotTouchCaptureCommandBehavior
    public class ShapeFileLayerGotTouchCaptureCommandBehavior : ShapeFileLayerGotTouchCaptureCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerLostTouchCaptureCommand
    // ShapeFileLayerLostTouchCaptureCommand
    public class ShapeFileLayerLostTouchCaptureCommand : ShapeFileLayerCommandBase<ShapeFileLayerLostTouchCaptureCommandBehavior>
    { }

    public class ShapeFileLayerLostTouchCaptureCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerLostTouchCaptureCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerLostTouchCaptureCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, TouchEventArgs>
    {
        public ShapeFileLayerLostTouchCaptureCommandBehavior(Func<object, TouchEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerLostTouchCaptureCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.LostTouchCapture += OnEventRaised;
        }
    }

    // ShapeFileLayerLostTouchCaptureCommandBehavior
    public class ShapeFileLayerLostTouchCaptureCommandBehavior : ShapeFileLayerLostTouchCaptureCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerTouchEnterCommand
    // ShapeFileLayerTouchEnterCommand
    public class ShapeFileLayerTouchEnterCommand : ShapeFileLayerCommandBase<ShapeFileLayerTouchEnterCommandBehavior>
    { }

    public class ShapeFileLayerTouchEnterCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerTouchEnterCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerTouchEnterCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, TouchEventArgs>
    {
        public ShapeFileLayerTouchEnterCommandBehavior(Func<object, TouchEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerTouchEnterCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.TouchEnter += OnEventRaised;
        }
    }

    // ShapeFileLayerTouchEnterCommandBehavior
    public class ShapeFileLayerTouchEnterCommandBehavior : ShapeFileLayerTouchEnterCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerTouchLeaveCommand
    // ShapeFileLayerTouchLeaveCommand
    public class ShapeFileLayerTouchLeaveCommand : ShapeFileLayerCommandBase<ShapeFileLayerTouchLeaveCommandBehavior>
    { }

    public class ShapeFileLayerTouchLeaveCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerTouchLeaveCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerTouchLeaveCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, TouchEventArgs>
    {
        public ShapeFileLayerTouchLeaveCommandBehavior(Func<object, TouchEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerTouchLeaveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.TouchLeave += OnEventRaised;
        }
    }

    // ShapeFileLayerTouchLeaveCommandBehavior
    public class ShapeFileLayerTouchLeaveCommandBehavior : ShapeFileLayerTouchLeaveCommandBehavior<object>
    { }
    #endregion

#endif

    #region ShapeFileLayerIsMouseDirectlyOverChangedCommand
    // ShapeFileLayerIsMouseDirectlyOverChangedCommand
    public class ShapeFileLayerIsMouseDirectlyOverChangedCommand : ShapeFileLayerCommandBase<ShapeFileLayerIsMouseDirectlyOverChangedCommandBehavior>
    { }

    public class ShapeFileLayerIsMouseDirectlyOverChangedCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerIsMouseDirectlyOverChangedCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerIsMouseDirectlyOverChangedCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public ShapeFileLayerIsMouseDirectlyOverChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerIsMouseDirectlyOverChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsMouseDirectlyOverChanged += OnEventRaised;
        }
    }

    // ShapeFileLayerIsMouseDirectlyOverChangedCommandBehavior
    public class ShapeFileLayerIsMouseDirectlyOverChangedCommandBehavior : ShapeFileLayerIsMouseDirectlyOverChangedCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerIsKeyboardFocusWithinChangedCommand
    // ShapeFileLayerIsKeyboardFocusWithinChangedCommand
    public class ShapeFileLayerIsKeyboardFocusWithinChangedCommand : ShapeFileLayerCommandBase<ShapeFileLayerIsKeyboardFocusWithinChangedCommandBehavior>
    { }

    public class ShapeFileLayerIsKeyboardFocusWithinChangedCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerIsKeyboardFocusWithinChangedCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerIsKeyboardFocusWithinChangedCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public ShapeFileLayerIsKeyboardFocusWithinChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerIsKeyboardFocusWithinChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsKeyboardFocusWithinChanged += OnEventRaised;
        }
    }

    // ShapeFileLayerIsKeyboardFocusWithinChangedCommandBehavior
    public class ShapeFileLayerIsKeyboardFocusWithinChangedCommandBehavior : ShapeFileLayerIsKeyboardFocusWithinChangedCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerIsMouseCapturedChangedCommand
    // ShapeFileLayerIsMouseCapturedChangedCommand
    public class ShapeFileLayerIsMouseCapturedChangedCommand : ShapeFileLayerCommandBase<ShapeFileLayerIsMouseCapturedChangedCommandBehavior>
    { }

    public class ShapeFileLayerIsMouseCapturedChangedCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerIsMouseCapturedChangedCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerIsMouseCapturedChangedCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public ShapeFileLayerIsMouseCapturedChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerIsMouseCapturedChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsMouseCapturedChanged += OnEventRaised;
        }
    }

    // ShapeFileLayerIsMouseCapturedChangedCommandBehavior
    public class ShapeFileLayerIsMouseCapturedChangedCommandBehavior : ShapeFileLayerIsMouseCapturedChangedCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerIsMouseCaptureWithinChangedCommand
    // ShapeFileLayerIsMouseCaptureWithinChangedCommand
    public class ShapeFileLayerIsMouseCaptureWithinChangedCommand : ShapeFileLayerCommandBase<ShapeFileLayerIsMouseCaptureWithinChangedCommandBehavior>
    { }

    public class ShapeFileLayerIsMouseCaptureWithinChangedCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerIsMouseCaptureWithinChangedCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerIsMouseCaptureWithinChangedCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public ShapeFileLayerIsMouseCaptureWithinChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerIsMouseCaptureWithinChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsMouseCaptureWithinChanged += OnEventRaised;
        }
    }

    // ShapeFileLayerIsMouseCaptureWithinChangedCommandBehavior
    public class ShapeFileLayerIsMouseCaptureWithinChangedCommandBehavior : ShapeFileLayerIsMouseCaptureWithinChangedCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerIsStylusDirectlyOverChangedCommand
    // ShapeFileLayerIsStylusDirectlyOverChangedCommand
    public class ShapeFileLayerIsStylusDirectlyOverChangedCommand : ShapeFileLayerCommandBase<ShapeFileLayerIsStylusDirectlyOverChangedCommandBehavior>
    { }

    public class ShapeFileLayerIsStylusDirectlyOverChangedCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerIsStylusDirectlyOverChangedCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerIsStylusDirectlyOverChangedCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public ShapeFileLayerIsStylusDirectlyOverChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerIsStylusDirectlyOverChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsStylusDirectlyOverChanged += OnEventRaised;
        }
    }

    // ShapeFileLayerIsStylusDirectlyOverChangedCommandBehavior
    public class ShapeFileLayerIsStylusDirectlyOverChangedCommandBehavior : ShapeFileLayerIsStylusDirectlyOverChangedCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerIsStylusCapturedChangedCommand
    // ShapeFileLayerIsStylusCapturedChangedCommand
    public class ShapeFileLayerIsStylusCapturedChangedCommand : ShapeFileLayerCommandBase<ShapeFileLayerIsStylusCapturedChangedCommandBehavior>
    { }

    public class ShapeFileLayerIsStylusCapturedChangedCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerIsStylusCapturedChangedCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerIsStylusCapturedChangedCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public ShapeFileLayerIsStylusCapturedChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerIsStylusCapturedChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsStylusCapturedChanged += OnEventRaised;
        }
    }

    // ShapeFileLayerIsStylusCapturedChangedCommandBehavior
    public class ShapeFileLayerIsStylusCapturedChangedCommandBehavior : ShapeFileLayerIsStylusCapturedChangedCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerIsStylusCaptureWithinChangedCommand
    // ShapeFileLayerIsStylusCaptureWithinChangedCommand
    public class ShapeFileLayerIsStylusCaptureWithinChangedCommand : ShapeFileLayerCommandBase<ShapeFileLayerIsStylusCaptureWithinChangedCommandBehavior>
    { }

    public class ShapeFileLayerIsStylusCaptureWithinChangedCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerIsStylusCaptureWithinChangedCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerIsStylusCaptureWithinChangedCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public ShapeFileLayerIsStylusCaptureWithinChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerIsStylusCaptureWithinChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsStylusCaptureWithinChanged += OnEventRaised;
        }
    }

    // ShapeFileLayerIsStylusCaptureWithinChangedCommandBehavior
    public class ShapeFileLayerIsStylusCaptureWithinChangedCommandBehavior : ShapeFileLayerIsStylusCaptureWithinChangedCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerIsKeyboardFocusedChangedCommand
    // ShapeFileLayerIsKeyboardFocusedChangedCommand
    public class ShapeFileLayerIsKeyboardFocusedChangedCommand : ShapeFileLayerCommandBase<ShapeFileLayerIsKeyboardFocusedChangedCommandBehavior>
    { }

    public class ShapeFileLayerIsKeyboardFocusedChangedCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerIsKeyboardFocusedChangedCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerIsKeyboardFocusedChangedCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public ShapeFileLayerIsKeyboardFocusedChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerIsKeyboardFocusedChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsKeyboardFocusedChanged += OnEventRaised;
        }
    }

    // ShapeFileLayerIsKeyboardFocusedChangedCommandBehavior
    public class ShapeFileLayerIsKeyboardFocusedChangedCommandBehavior : ShapeFileLayerIsKeyboardFocusedChangedCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerLayoutUpdatedCommand
    // ShapeFileLayerLayoutUpdatedCommand
    public class ShapeFileLayerLayoutUpdatedCommand : ShapeFileLayerCommandBase<ShapeFileLayerLayoutUpdatedCommandBehavior>
    { }

    public class ShapeFileLayerLayoutUpdatedCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerLayoutUpdatedCommandBehavior<T>, new()
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

    #region ShapeFileLayerGotFocusCommand
    // ShapeFileLayerGotFocusCommand
    public class ShapeFileLayerGotFocusCommand : ShapeFileLayerCommandBase<ShapeFileLayerGotFocusCommandBehavior>
    { }

    public class ShapeFileLayerGotFocusCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerGotFocusCommandBehavior<T>, new()
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

    public class ShapeFileLayerLostFocusCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerLostFocusCommandBehavior<T>, new()
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

    #region ShapeFileLayerIsEnabledChangedCommand
    // ShapeFileLayerIsEnabledChangedCommand
    public class ShapeFileLayerIsEnabledChangedCommand : ShapeFileLayerCommandBase<ShapeFileLayerIsEnabledChangedCommandBehavior>
    { }

    public class ShapeFileLayerIsEnabledChangedCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerIsEnabledChangedCommandBehavior<T>, new()
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

    #region ShapeFileLayerIsHitTestVisibleChangedCommand
    // ShapeFileLayerIsHitTestVisibleChangedCommand
    public class ShapeFileLayerIsHitTestVisibleChangedCommand : ShapeFileLayerCommandBase<ShapeFileLayerIsHitTestVisibleChangedCommandBehavior>
    { }

    public class ShapeFileLayerIsHitTestVisibleChangedCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerIsHitTestVisibleChangedCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerIsHitTestVisibleChangedCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public ShapeFileLayerIsHitTestVisibleChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerIsHitTestVisibleChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsHitTestVisibleChanged += OnEventRaised;
        }
    }

    // ShapeFileLayerIsHitTestVisibleChangedCommandBehavior
    public class ShapeFileLayerIsHitTestVisibleChangedCommandBehavior : ShapeFileLayerIsHitTestVisibleChangedCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerIsVisibleChangedCommand
    // ShapeFileLayerIsVisibleChangedCommand
    public class ShapeFileLayerIsVisibleChangedCommand : ShapeFileLayerCommandBase<ShapeFileLayerIsVisibleChangedCommandBehavior>
    { }

    public class ShapeFileLayerIsVisibleChangedCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerIsVisibleChangedCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerIsVisibleChangedCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public ShapeFileLayerIsVisibleChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerIsVisibleChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsVisibleChanged += OnEventRaised;
        }
    }

    // ShapeFileLayerIsVisibleChangedCommandBehavior
    public class ShapeFileLayerIsVisibleChangedCommandBehavior : ShapeFileLayerIsVisibleChangedCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerFocusableChangedCommand
    // ShapeFileLayerFocusableChangedCommand
    public class ShapeFileLayerFocusableChangedCommand : ShapeFileLayerCommandBase<ShapeFileLayerFocusableChangedCommandBehavior>
    { }

    public class ShapeFileLayerFocusableChangedCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerFocusableChangedCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerFocusableChangedCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public ShapeFileLayerFocusableChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerFocusableChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.FocusableChanged += OnEventRaised;
        }
    }

    // ShapeFileLayerFocusableChangedCommandBehavior
    public class ShapeFileLayerFocusableChangedCommandBehavior : ShapeFileLayerFocusableChangedCommandBehavior<object>
    { }
    #endregion

#if SyncfusionFramework4_0

    #region ShapeFileLayerManipulationStartingCommand
    // ShapeFileLayerManipulationStartingCommand
    public class ShapeFileLayerManipulationStartingCommand : ShapeFileLayerCommandBase<ShapeFileLayerManipulationStartingCommandBehavior>
    { }

    public class ShapeFileLayerManipulationStartingCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerManipulationStartingCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerManipulationStartingCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, ManipulationStartingEventArgs>
    {
        public ShapeFileLayerManipulationStartingCommandBehavior(Func<object, ManipulationStartingEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerManipulationStartingCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ManipulationStarting += OnEventRaised;
        }
    }

    // ShapeFileLayerManipulationStartingCommandBehavior
    public class ShapeFileLayerManipulationStartingCommandBehavior : ShapeFileLayerManipulationStartingCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerManipulationStartedCommand
    // ShapeFileLayerManipulationStartedCommand
    public class ShapeFileLayerManipulationStartedCommand : ShapeFileLayerCommandBase<ShapeFileLayerManipulationStartedCommandBehavior>
    { }

    public class ShapeFileLayerManipulationStartedCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerManipulationStartedCommandBehavior<T>, new()
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

    public class ShapeFileLayerManipulationDeltaCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerManipulationDeltaCommandBehavior<T>, new()
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

    #region ShapeFileLayerManipulationInertiaStartingCommand
    // ShapeFileLayerManipulationInertiaStartingCommand
    public class ShapeFileLayerManipulationInertiaStartingCommand : ShapeFileLayerCommandBase<ShapeFileLayerManipulationInertiaStartingCommandBehavior>
    { }

    public class ShapeFileLayerManipulationInertiaStartingCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerManipulationInertiaStartingCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerManipulationInertiaStartingCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, ManipulationInertiaStartingEventArgs>
    {
        public ShapeFileLayerManipulationInertiaStartingCommandBehavior(Func<object, ManipulationInertiaStartingEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerManipulationInertiaStartingCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ManipulationInertiaStarting += OnEventRaised;
        }
    }

    // ShapeFileLayerManipulationInertiaStartingCommandBehavior
    public class ShapeFileLayerManipulationInertiaStartingCommandBehavior : ShapeFileLayerManipulationInertiaStartingCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerManipulationBoundaryFeedbackCommand
    // ShapeFileLayerManipulationBoundaryFeedbackCommand
    public class ShapeFileLayerManipulationBoundaryFeedbackCommand : ShapeFileLayerCommandBase<ShapeFileLayerManipulationBoundaryFeedbackCommandBehavior>
    { }

    public class ShapeFileLayerManipulationBoundaryFeedbackCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerManipulationBoundaryFeedbackCommandBehavior<T>, new()
    { }

    public class ShapeFileLayerManipulationBoundaryFeedbackCommandBehavior<TReturn> : ShapeFileLayerCommandBehaviorBase<TReturn, ManipulationBoundaryFeedbackEventArgs>
    {
        public ShapeFileLayerManipulationBoundaryFeedbackCommandBehavior(Func<object, ManipulationBoundaryFeedbackEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ShapeFileLayerManipulationBoundaryFeedbackCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ManipulationBoundaryFeedback += OnEventRaised;
        }
    }

    // ShapeFileLayerManipulationBoundaryFeedbackCommandBehavior
    public class ShapeFileLayerManipulationBoundaryFeedbackCommandBehavior : ShapeFileLayerManipulationBoundaryFeedbackCommandBehavior<object>
    { }
    #endregion

    #region ShapeFileLayerManipulationCompletedCommand
    // ShapeFileLayerManipulationCompletedCommand
    public class ShapeFileLayerManipulationCompletedCommand : ShapeFileLayerCommandBase<ShapeFileLayerManipulationCompletedCommandBehavior>
    { }

    public class ShapeFileLayerManipulationCompletedCommand<T, TBehavior> : ShapeFileLayerCommandBase<TBehavior> where TBehavior : ShapeFileLayerManipulationCompletedCommandBehavior<T>, new()
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
#endif
}


