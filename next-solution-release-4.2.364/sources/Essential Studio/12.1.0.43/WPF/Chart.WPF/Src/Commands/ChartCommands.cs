#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// Generated at 5/1/2012 11:56:48 PM 
#endregion
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Syncfusion.Windows.Chart;
using Syncfusion.Windows.Shared;


namespace Syncfusion.Windows.Chart.MVVM
{

    #region Base
    public class ChartCommandBase<TBehavior> : ControlCommandBase<TBehavior, Chart> where TBehavior : CommandBehaviorBase<Chart>, new()
    { }


    public class ChartCommandBehaviorBase<TReturn, TEventArgs> : BuilderCommandBehaviorBase<Chart, TEventArgs, TReturn>
    { }



    public class ChartAreaCommandBase<TBehavior> : ControlCommandBase<TBehavior, ChartArea> where TBehavior : CommandBehaviorBase<ChartArea>, new()
    { }


    public class ChartAreaCommandBehaviorBase<TReturn, TEventArgs> : BuilderCommandBehaviorBase<ChartArea, TEventArgs, TReturn>
    { }




    #endregion

    #region AreaCommands
    #region ChartAreaSegmentDragging
    // ChartAreaSegmentDraggingCommand<T, TBehavior>
    public class ChartAreaSegmentDraggingCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaSegmentDraggingCommandBehavior<T>, new()
    { }

    // ChartAreaSegmentDraggingCommandBehavior<TReturn>
    public class ChartAreaSegmentDraggingCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, SegmentDragEventArgs>
    {
        public ChartAreaSegmentDraggingCommandBehavior(Func<object, SegmentDragEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaSegmentDraggingCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.SegmentDragging += OnEventRaised;
        }
    }

    // ChartAreaSegmentDraggingCommand
    public class ChartAreaSegmentDraggingCommand : ChartAreaCommandBase<ChartAreaSegmentDraggingCommandBehavior>
    { }

    // ChartAreaSegmentDraggingCommandBehavior
    public class ChartAreaSegmentDraggingCommandBehavior : ChartAreaSegmentDraggingCommandBehavior<object>
    { }

    // ChartAreaSegmentDraggingCommandWithEventArgs	
    public class ChartAreaSegmentDraggingCommandWithEventArgs : ChartAreaSegmentDraggingCommand<SegmentDragEventArgs, ChartAreaSegmentDraggingCommandBehaviorWithEventArgs>
    { }

    // ChartAreaSegmentDraggingCommandBehaviorWithEventArgs
    public class ChartAreaSegmentDraggingCommandBehaviorWithEventArgs : ChartAreaSegmentDraggingCommandBehavior<SegmentDragEventArgs>
    {
        public ChartAreaSegmentDraggingCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region ChartAreaSegmentDragged
    // ChartAreaSegmentDraggedCommand<T, TBehavior>
    public class ChartAreaSegmentDraggedCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaSegmentDraggedCommandBehavior<T>, new()
    { }

    // ChartAreaSegmentDraggedCommandBehavior<TReturn>
    public class ChartAreaSegmentDraggedCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, SegmentDragEventArgs>
    {
        public ChartAreaSegmentDraggedCommandBehavior(Func<object, SegmentDragEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaSegmentDraggedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.SegmentDragged += OnEventRaised;
        }
    }

    // ChartAreaSegmentDraggedCommand
    public class ChartAreaSegmentDraggedCommand : ChartAreaCommandBase<ChartAreaSegmentDraggedCommandBehavior>
    { }

    // ChartAreaSegmentDraggedCommandBehavior
    public class ChartAreaSegmentDraggedCommandBehavior : ChartAreaSegmentDraggedCommandBehavior<object>
    { }

    // ChartAreaSegmentDraggedCommandWithEventArgs	
    public class ChartAreaSegmentDraggedCommandWithEventArgs : ChartAreaSegmentDraggedCommand<SegmentDragEventArgs, ChartAreaSegmentDraggedCommandBehaviorWithEventArgs>
    { }

    // ChartAreaSegmentDraggedCommandBehaviorWithEventArgs
    public class ChartAreaSegmentDraggedCommandBehaviorWithEventArgs : ChartAreaSegmentDraggedCommandBehavior<SegmentDragEventArgs>
    {
        public ChartAreaSegmentDraggedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaSegmentDropping
    // ChartAreaSegmentDroppingCommand<T, TBehavior>
    public class ChartAreaSegmentDroppingCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaSegmentDroppingCommandBehavior<T>, new()
    { }

    // ChartAreaSegmentDroppingCommandBehavior<TReturn>
    public class ChartAreaSegmentDroppingCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, SegmentDropEventArgs>
    {
        public ChartAreaSegmentDroppingCommandBehavior(Func<object, SegmentDropEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaSegmentDroppingCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.SegmentDropping += OnEventRaised;
        }
    }

    // ChartAreaSegmentDroppingCommand
    public class ChartAreaSegmentDroppingCommand : ChartAreaCommandBase<ChartAreaSegmentDroppingCommandBehavior>
    { }

    // ChartAreaSegmentDroppingCommandBehavior
    public class ChartAreaSegmentDroppingCommandBehavior : ChartAreaSegmentDroppingCommandBehavior<object>
    { }

    // ChartAreaSegmentDroppingCommandWithEventArgs	
    public class ChartAreaSegmentDroppingCommandWithEventArgs : ChartAreaSegmentDroppingCommand<SegmentDropEventArgs, ChartAreaSegmentDroppingCommandBehaviorWithEventArgs>
    { }

    // ChartAreaSegmentDroppingCommandBehaviorWithEventArgs
    public class ChartAreaSegmentDroppingCommandBehaviorWithEventArgs : ChartAreaSegmentDroppingCommandBehavior<SegmentDropEventArgs>
    {
        public ChartAreaSegmentDroppingCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaSegmentDropped
    // ChartAreaSegmentDroppedCommand<T, TBehavior>
    public class ChartAreaSegmentDroppedCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaSegmentDroppedCommandBehavior<T>, new()
    { }

    // ChartAreaSegmentDroppedCommandBehavior<TReturn>
    public class ChartAreaSegmentDroppedCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, SegmentDropEventArgs>
    {
        public ChartAreaSegmentDroppedCommandBehavior(Func<object, SegmentDropEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaSegmentDroppedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.SegmentDropped += OnEventRaised;
        }
    }

    // ChartAreaSegmentDroppedCommand
    public class ChartAreaSegmentDroppedCommand : ChartAreaCommandBase<ChartAreaSegmentDroppedCommandBehavior>
    { }

    // ChartAreaSegmentDroppedCommandBehavior
    public class ChartAreaSegmentDroppedCommandBehavior : ChartAreaSegmentDroppedCommandBehavior<object>
    { }

    // ChartAreaSegmentDroppedCommandWithEventArgs	
    public class ChartAreaSegmentDroppedCommandWithEventArgs : ChartAreaSegmentDroppedCommand<SegmentDropEventArgs, ChartAreaSegmentDroppedCommandBehaviorWithEventArgs>
    { }

    // ChartAreaSegmentDroppedCommandBehaviorWithEventArgs
    public class ChartAreaSegmentDroppedCommandBehaviorWithEventArgs : ChartAreaSegmentDroppedCommandBehavior<SegmentDropEventArgs>
    {
        public ChartAreaSegmentDroppedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaView3DModeChanged
    // ChartAreaView3DModeChangedCommand<T, TBehavior>
    public class ChartAreaView3DModeChangedCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaView3DModeChangedCommandBehavior<T>, new()
    { }

    // ChartAreaView3DModeChangedCommandBehavior<TReturn>
    public class ChartAreaView3DModeChangedCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public ChartAreaView3DModeChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaView3DModeChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.View3DModeChanged += OnEventRaised;
        }
    }

    // ChartAreaView3DModeChangedCommand
    public class ChartAreaView3DModeChangedCommand : ChartAreaCommandBase<ChartAreaView3DModeChangedCommandBehavior>
    { }

    // ChartAreaView3DModeChangedCommandBehavior
    public class ChartAreaView3DModeChangedCommandBehavior : ChartAreaView3DModeChangedCommandBehavior<object>
    { }

    // ChartAreaView3DModeChangedCommandWithEventArgs	
    public class ChartAreaView3DModeChangedCommandWithEventArgs : ChartAreaView3DModeChangedCommand<DependencyPropertyChangedEventArgs, ChartAreaView3DModeChangedCommandBehaviorWithEventArgs>
    { }

    // ChartAreaView3DModeChangedCommandBehaviorWithEventArgs
    public class ChartAreaView3DModeChangedCommandBehaviorWithEventArgs : ChartAreaView3DModeChangedCommandBehavior<DependencyPropertyChangedEventArgs>
    {
        public ChartAreaView3DModeChangedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaChartScrolling
    // ChartAreaChartScrollingCommand<T, TBehavior>
    public class ChartAreaChartScrollingCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaChartScrollingCommandBehavior<T>, new()
    { }

    // ChartAreaChartScrollingCommandBehavior<TReturn>
    public class ChartAreaChartScrollingCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, ChartScrollEventArgs>
    {
        public ChartAreaChartScrollingCommandBehavior(Func<object, ChartScrollEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaChartScrollingCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ChartScrolling += OnEventRaised;
        }
    }

    // ChartAreaChartScrollingCommand
    public class ChartAreaChartScrollingCommand : ChartAreaCommandBase<ChartAreaChartScrollingCommandBehavior>
    { }

    // ChartAreaChartScrollingCommandBehavior
    public class ChartAreaChartScrollingCommandBehavior : ChartAreaChartScrollingCommandBehavior<object>
    { }

    // ChartAreaChartScrollingCommandWithEventArgs	
    public class ChartAreaChartScrollingCommandWithEventArgs : ChartAreaChartScrollingCommand<ChartScrollEventArgs, ChartAreaChartScrollingCommandBehaviorWithEventArgs>
    { }

    // ChartAreaChartScrollingCommandBehaviorWithEventArgs
    public class ChartAreaChartScrollingCommandBehaviorWithEventArgs : ChartAreaChartScrollingCommandBehavior<ChartScrollEventArgs>
    {
        public ChartAreaChartScrollingCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaChartZoomed
    // ChartAreaChartZoomedCommand<T, TBehavior>
    public class ChartAreaChartZoomedCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaChartZoomedCommandBehavior<T>, new()
    { }

    // ChartAreaChartZoomedCommandBehavior<TReturn>
    public class ChartAreaChartZoomedCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, ChartZoomedEventArgs>
    {
        public ChartAreaChartZoomedCommandBehavior(Func<object, ChartZoomedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaChartZoomedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ChartZoomed += OnEventRaised;
        }
    }

    // ChartAreaChartZoomedCommand
    public class ChartAreaChartZoomedCommand : ChartAreaCommandBase<ChartAreaChartZoomedCommandBehavior>
    { }

    // ChartAreaChartZoomedCommandBehavior
    public class ChartAreaChartZoomedCommandBehavior : ChartAreaChartZoomedCommandBehavior<object>
    { }

    // ChartAreaChartZoomedCommandWithEventArgs	
    public class ChartAreaChartZoomedCommandWithEventArgs : ChartAreaChartZoomedCommand<ChartZoomedEventArgs, ChartAreaChartZoomedCommandBehaviorWithEventArgs>
    { }

    // ChartAreaChartZoomedCommandBehaviorWithEventArgs
    public class ChartAreaChartZoomedCommandBehaviorWithEventArgs : ChartAreaChartZoomedCommandBehavior<ChartZoomedEventArgs>
    {
        public ChartAreaChartZoomedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaChartZoomSector
    // ChartAreaChartZoomSectorCommand<T, TBehavior>
    public class ChartAreaChartZoomSectorCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaChartZoomSectorCommandBehavior<T>, new()
    { }

    // ChartAreaChartZoomSectorCommandBehavior<TReturn>
    public class ChartAreaChartZoomSectorCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, ChartZoomSectorEventArgs>
    {
        public ChartAreaChartZoomSectorCommandBehavior(Func<object, ChartZoomSectorEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaChartZoomSectorCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ChartZoomSector += OnEventRaised;
        }
    }

    // ChartAreaChartZoomSectorCommand
    public class ChartAreaChartZoomSectorCommand : ChartAreaCommandBase<ChartAreaChartZoomSectorCommandBehavior>
    { }

    // ChartAreaChartZoomSectorCommandBehavior
    public class ChartAreaChartZoomSectorCommandBehavior : ChartAreaChartZoomSectorCommandBehavior<object>
    { }

    // ChartAreaChartZoomSectorCommandWithEventArgs	
    public class ChartAreaChartZoomSectorCommandWithEventArgs : ChartAreaChartZoomSectorCommand<ChartZoomSectorEventArgs, ChartAreaChartZoomSectorCommandBehaviorWithEventArgs>
    { }

    // ChartAreaChartZoomSectorCommandBehaviorWithEventArgs
    public class ChartAreaChartZoomSectorCommandBehaviorWithEventArgs : ChartAreaChartZoomSectorCommandBehavior<ChartZoomSectorEventArgs>
    {
        public ChartAreaChartZoomSectorCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaChartZoomedOut
    // ChartAreaChartZoomedOutCommand<T, TBehavior>
    public class ChartAreaChartZoomedOutCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaChartZoomedOutCommandBehavior<T>, new()
    { }

    // ChartAreaChartZoomedOutCommandBehavior<TReturn>
    public class ChartAreaChartZoomedOutCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, ChartZoomedOutEventArgs>
    {
        public ChartAreaChartZoomedOutCommandBehavior(Func<object, ChartZoomedOutEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaChartZoomedOutCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ChartZoomedOut += OnEventRaised;
        }      
    }

    // ChartAreaChartZoomedOutCommand
    public class ChartAreaChartZoomedOutCommand : ChartAreaCommandBase<ChartAreaChartZoomedOutCommandBehavior>
    { }

    // ChartAreaChartZoomedOutCommandBehavior
    public class ChartAreaChartZoomedOutCommandBehavior : ChartAreaChartZoomedOutCommandBehavior<object>
    { }

    // ChartAreaChartZoomedOutCommandWithEventArgs	
    public class ChartAreaChartZoomedOutCommandWithEventArgs : ChartAreaChartZoomedOutCommand<ChartZoomedOutEventArgs, ChartAreaChartZoomedOutCommandBehaviorWithEventArgs>
    { }

    // ChartAreaChartZoomedOutCommandBehaviorWithEventArgs
    public class ChartAreaChartZoomedOutCommandBehaviorWithEventArgs : ChartAreaChartZoomedOutCommandBehavior<ChartZoomedOutEventArgs>
    {
        public ChartAreaChartZoomedOutCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaChartPanning
    // ChartAreaChartPanningCommand<T, TBehavior>
    public class ChartAreaChartPanningCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaChartPanningCommandBehavior<T>, new()
    { }

    // ChartAreaChartPanningCommandBehavior<TReturn>
    public class ChartAreaChartPanningCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, ChartPanningEventArgs>
    {
        public ChartAreaChartPanningCommandBehavior(Func<object, ChartPanningEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaChartPanningCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ChartPanning += OnEventRaised;
        }
    }

    // ChartAreaChartPanningCommand
    public class ChartAreaChartPanningCommand : ChartAreaCommandBase<ChartAreaChartPanningCommandBehavior>
    { }

    // ChartAreaChartPanningCommandBehavior
    public class ChartAreaChartPanningCommandBehavior : ChartAreaChartPanningCommandBehavior<object>
    { }

    // ChartAreaChartPanningCommandWithEventArgs	
    public class ChartAreaChartPanningCommandWithEventArgs : ChartAreaChartPanningCommand<ChartPanningEventArgs, ChartAreaChartPanningCommandBehaviorWithEventArgs>
    { }

    // ChartAreaChartPanningCommandBehaviorWithEventArgs
    public class ChartAreaChartPanningCommandBehaviorWithEventArgs : ChartAreaChartPanningCommandBehavior<ChartPanningEventArgs>
    {
        public ChartAreaChartPanningCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaChartZoomReset
    // ChartAreaChartZoomResetCommand<T, TBehavior>
    public class ChartAreaChartZoomResetCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaChartZoomResetCommandBehavior<T>, new()
    { }

    // ChartAreaChartZoomResetCommandBehavior<TReturn>
    public class ChartAreaChartZoomResetCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, ChartZoomReseteventArgs>
    {
        public ChartAreaChartZoomResetCommandBehavior(Func<object, ChartZoomReseteventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaChartZoomResetCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ChartZoomReset += OnEventRaised;
        }
    }

    // ChartAreaChartZoomResetCommand
    public class ChartAreaChartZoomResetCommand : ChartAreaCommandBase<ChartAreaChartZoomResetCommandBehavior>
    { }

    // ChartAreaChartZoomResetCommandBehavior
    public class ChartAreaChartZoomResetCommandBehavior : ChartAreaChartZoomResetCommandBehavior<object>
    { }

    // ChartAreaChartZoomResetCommandWithEventArgs	
    public class ChartAreaChartZoomResetCommandWithEventArgs : ChartAreaChartZoomResetCommand<ChartZoomReseteventArgs, ChartAreaChartZoomResetCommandBehaviorWithEventArgs>
    { }

    // ChartAreaChartZoomResetCommandBehaviorWithEventArgs
    public class ChartAreaChartZoomResetCommandBehaviorWithEventArgs : ChartAreaChartZoomResetCommandBehavior<ChartZoomReseteventArgs>
    {
        public ChartAreaChartZoomResetCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaPreviewMouseDoubleClick
    // ChartAreaPreviewMouseDoubleClickCommand<T, TBehavior>
    public class ChartAreaPreviewMouseDoubleClickCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaPreviewMouseDoubleClickCommandBehavior<T>, new()
    { }

    // ChartAreaPreviewMouseDoubleClickCommandBehavior<TReturn>
    public class ChartAreaPreviewMouseDoubleClickCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public ChartAreaPreviewMouseDoubleClickCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaPreviewMouseDoubleClickCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewMouseDoubleClick += OnEventRaised;
        }
    }

    // ChartAreaPreviewMouseDoubleClickCommand
    public class ChartAreaPreviewMouseDoubleClickCommand : ChartAreaCommandBase<ChartAreaPreviewMouseDoubleClickCommandBehavior>
    { }

    // ChartAreaPreviewMouseDoubleClickCommandBehavior
    public class ChartAreaPreviewMouseDoubleClickCommandBehavior : ChartAreaPreviewMouseDoubleClickCommandBehavior<object>
    { }

    // ChartAreaPreviewMouseDoubleClickCommandWithEventArgs	
    public class ChartAreaPreviewMouseDoubleClickCommandWithEventArgs : ChartAreaPreviewMouseDoubleClickCommand<MouseButtonEventArgs, ChartAreaPreviewMouseDoubleClickCommandBehaviorWithEventArgs>
    { }

    // ChartAreaPreviewMouseDoubleClickCommandBehaviorWithEventArgs
    public class ChartAreaPreviewMouseDoubleClickCommandBehaviorWithEventArgs : ChartAreaPreviewMouseDoubleClickCommandBehavior<MouseButtonEventArgs>
    {
        public ChartAreaPreviewMouseDoubleClickCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaMouseDoubleClick
    // ChartAreaMouseDoubleClickCommand<T, TBehavior>
    public class ChartAreaMouseDoubleClickCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaMouseDoubleClickCommandBehavior<T>, new()
    { }

    // ChartAreaMouseDoubleClickCommandBehavior<TReturn>
    public class ChartAreaMouseDoubleClickCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public ChartAreaMouseDoubleClickCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaMouseDoubleClickCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseDoubleClick += OnEventRaised;
        }
    }

    // ChartAreaMouseDoubleClickCommand
    public class ChartAreaMouseDoubleClickCommand : ChartAreaCommandBase<ChartAreaMouseDoubleClickCommandBehavior>
    { }

    // ChartAreaMouseDoubleClickCommandBehavior
    public class ChartAreaMouseDoubleClickCommandBehavior : ChartAreaMouseDoubleClickCommandBehavior<object>
    { }

    // ChartAreaMouseDoubleClickCommandWithEventArgs	
    public class ChartAreaMouseDoubleClickCommandWithEventArgs : ChartAreaMouseDoubleClickCommand<MouseButtonEventArgs, ChartAreaMouseDoubleClickCommandBehaviorWithEventArgs>
    { }

    // ChartAreaMouseDoubleClickCommandBehaviorWithEventArgs
    public class ChartAreaMouseDoubleClickCommandBehaviorWithEventArgs : ChartAreaMouseDoubleClickCommandBehavior<MouseButtonEventArgs>
    {
        public ChartAreaMouseDoubleClickCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaTargetUpdated
    // ChartAreaTargetUpdatedCommand<T, TBehavior>
    public class ChartAreaTargetUpdatedCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaTargetUpdatedCommandBehavior<T>, new()
    { }

    // ChartAreaTargetUpdatedCommandBehavior<TReturn>
    public class ChartAreaTargetUpdatedCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, DataTransferEventArgs>
    {
        public ChartAreaTargetUpdatedCommandBehavior(Func<object, DataTransferEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaTargetUpdatedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.TargetUpdated += OnEventRaised;
        }
    }

    // ChartAreaTargetUpdatedCommand
    public class ChartAreaTargetUpdatedCommand : ChartAreaCommandBase<ChartAreaTargetUpdatedCommandBehavior>
    { }

    // ChartAreaTargetUpdatedCommandBehavior
    public class ChartAreaTargetUpdatedCommandBehavior : ChartAreaTargetUpdatedCommandBehavior<object>
    { }

    // ChartAreaTargetUpdatedCommandWithEventArgs	
    public class ChartAreaTargetUpdatedCommandWithEventArgs : ChartAreaTargetUpdatedCommand<DataTransferEventArgs, ChartAreaTargetUpdatedCommandBehaviorWithEventArgs>
    { }

    // ChartAreaTargetUpdatedCommandBehaviorWithEventArgs
    public class ChartAreaTargetUpdatedCommandBehaviorWithEventArgs : ChartAreaTargetUpdatedCommandBehavior<DataTransferEventArgs>
    {
        public ChartAreaTargetUpdatedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaSourceUpdated
    // ChartAreaSourceUpdatedCommand<T, TBehavior>
    public class ChartAreaSourceUpdatedCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaSourceUpdatedCommandBehavior<T>, new()
    { }

    // ChartAreaSourceUpdatedCommandBehavior<TReturn>
    public class ChartAreaSourceUpdatedCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, DataTransferEventArgs>
    {
        public ChartAreaSourceUpdatedCommandBehavior(Func<object, DataTransferEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaSourceUpdatedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.SourceUpdated += OnEventRaised;
        }
    }

    // ChartAreaSourceUpdatedCommand
    public class ChartAreaSourceUpdatedCommand : ChartAreaCommandBase<ChartAreaSourceUpdatedCommandBehavior>
    { }

    // ChartAreaSourceUpdatedCommandBehavior
    public class ChartAreaSourceUpdatedCommandBehavior : ChartAreaSourceUpdatedCommandBehavior<object>
    { }

    // ChartAreaSourceUpdatedCommandWithEventArgs	
    public class ChartAreaSourceUpdatedCommandWithEventArgs : ChartAreaSourceUpdatedCommand<DataTransferEventArgs, ChartAreaSourceUpdatedCommandBehaviorWithEventArgs>
    { }

    // ChartAreaSourceUpdatedCommandBehaviorWithEventArgs
    public class ChartAreaSourceUpdatedCommandBehaviorWithEventArgs : ChartAreaSourceUpdatedCommandBehavior<DataTransferEventArgs>
    {
        public ChartAreaSourceUpdatedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaDataContextChanged
    // ChartAreaDataContextChangedCommand<T, TBehavior>
    public class ChartAreaDataContextChangedCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaDataContextChangedCommandBehavior<T>, new()
    { }

    // ChartAreaDataContextChangedCommandBehavior<TReturn>
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

    // ChartAreaDataContextChangedCommand
    public class ChartAreaDataContextChangedCommand : ChartAreaCommandBase<ChartAreaDataContextChangedCommandBehavior>
    { }

    // ChartAreaDataContextChangedCommandBehavior
    public class ChartAreaDataContextChangedCommandBehavior : ChartAreaDataContextChangedCommandBehavior<object>
    { }

    // ChartAreaDataContextChangedCommandWithEventArgs	
    public class ChartAreaDataContextChangedCommandWithEventArgs : ChartAreaDataContextChangedCommand<DependencyPropertyChangedEventArgs, ChartAreaDataContextChangedCommandBehaviorWithEventArgs>
    { }

    // ChartAreaDataContextChangedCommandBehaviorWithEventArgs
    public class ChartAreaDataContextChangedCommandBehaviorWithEventArgs : ChartAreaDataContextChangedCommandBehavior<DependencyPropertyChangedEventArgs>
    {
        public ChartAreaDataContextChangedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaRequestBringIntoView
    // ChartAreaRequestBringIntoViewCommand<T, TBehavior>
    public class ChartAreaRequestBringIntoViewCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaRequestBringIntoViewCommandBehavior<T>, new()
    { }

    // ChartAreaRequestBringIntoViewCommandBehavior<TReturn>
    public class ChartAreaRequestBringIntoViewCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, RequestBringIntoViewEventArgs>
    {
        public ChartAreaRequestBringIntoViewCommandBehavior(Func<object, RequestBringIntoViewEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaRequestBringIntoViewCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.RequestBringIntoView += OnEventRaised;
        }
    }

    // ChartAreaRequestBringIntoViewCommand
    public class ChartAreaRequestBringIntoViewCommand : ChartAreaCommandBase<ChartAreaRequestBringIntoViewCommandBehavior>
    { }

    // ChartAreaRequestBringIntoViewCommandBehavior
    public class ChartAreaRequestBringIntoViewCommandBehavior : ChartAreaRequestBringIntoViewCommandBehavior<object>
    { }

    // ChartAreaRequestBringIntoViewCommandWithEventArgs	
    public class ChartAreaRequestBringIntoViewCommandWithEventArgs : ChartAreaRequestBringIntoViewCommand<RequestBringIntoViewEventArgs, ChartAreaRequestBringIntoViewCommandBehaviorWithEventArgs>
    { }

    // ChartAreaRequestBringIntoViewCommandBehaviorWithEventArgs
    public class ChartAreaRequestBringIntoViewCommandBehaviorWithEventArgs : ChartAreaRequestBringIntoViewCommandBehavior<RequestBringIntoViewEventArgs>
    {
        public ChartAreaRequestBringIntoViewCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaSizeChanged
    // ChartAreaSizeChangedCommand<T, TBehavior>
    public class ChartAreaSizeChangedCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaSizeChangedCommandBehavior<T>, new()
    { }

    // ChartAreaSizeChangedCommandBehavior<TReturn>
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

    // ChartAreaSizeChangedCommand
    public class ChartAreaSizeChangedCommand : ChartAreaCommandBase<ChartAreaSizeChangedCommandBehavior>
    { }

    // ChartAreaSizeChangedCommandBehavior
    public class ChartAreaSizeChangedCommandBehavior : ChartAreaSizeChangedCommandBehavior<object>
    { }

    // ChartAreaSizeChangedCommandWithEventArgs	
    public class ChartAreaSizeChangedCommandWithEventArgs : ChartAreaSizeChangedCommand<SizeChangedEventArgs, ChartAreaSizeChangedCommandBehaviorWithEventArgs>
    { }

    // ChartAreaSizeChangedCommandBehaviorWithEventArgs
    public class ChartAreaSizeChangedCommandBehaviorWithEventArgs : ChartAreaSizeChangedCommandBehavior<SizeChangedEventArgs>
    {
        public ChartAreaSizeChangedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaInitialized
    // ChartAreaInitializedCommand<T, TBehavior>
    public class ChartAreaInitializedCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaInitializedCommandBehavior<T>, new()
    { }

    // ChartAreaInitializedCommandBehavior<TReturn>
    public class ChartAreaInitializedCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, EventArgs>
    {
        public ChartAreaInitializedCommandBehavior(Func<object, EventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaInitializedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Initialized += OnEventRaised;
        }
    }

    // ChartAreaInitializedCommand
    public class ChartAreaInitializedCommand : ChartAreaCommandBase<ChartAreaInitializedCommandBehavior>
    { }

    // ChartAreaInitializedCommandBehavior
    public class ChartAreaInitializedCommandBehavior : ChartAreaInitializedCommandBehavior<object>
    { }

    // ChartAreaInitializedCommandWithEventArgs	
    public class ChartAreaInitializedCommandWithEventArgs : ChartAreaInitializedCommand<EventArgs, ChartAreaInitializedCommandBehaviorWithEventArgs>
    { }

    // ChartAreaInitializedCommandBehaviorWithEventArgs
    public class ChartAreaInitializedCommandBehaviorWithEventArgs : ChartAreaInitializedCommandBehavior<EventArgs>
    {
        public ChartAreaInitializedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaLoaded
    // ChartAreaLoadedCommand<T, TBehavior>
    public class ChartAreaLoadedCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaLoadedCommandBehavior<T>, new()
    { }

    // ChartAreaLoadedCommandBehavior<TReturn>
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

    // ChartAreaLoadedCommand
    public class ChartAreaLoadedCommand : ChartAreaCommandBase<ChartAreaLoadedCommandBehavior>
    { }

    // ChartAreaLoadedCommandBehavior
    public class ChartAreaLoadedCommandBehavior : ChartAreaLoadedCommandBehavior<object>
    { }

    // ChartAreaLoadedCommandWithEventArgs	
    public class ChartAreaLoadedCommandWithEventArgs : ChartAreaLoadedCommand<RoutedEventArgs, ChartAreaLoadedCommandBehaviorWithEventArgs>
    { }

    // ChartAreaLoadedCommandBehaviorWithEventArgs
    public class ChartAreaLoadedCommandBehaviorWithEventArgs : ChartAreaLoadedCommandBehavior<RoutedEventArgs>
    {
        public ChartAreaLoadedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaUnloaded
    // ChartAreaUnloadedCommand<T, TBehavior>
    public class ChartAreaUnloadedCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaUnloadedCommandBehavior<T>, new()
    { }

    // ChartAreaUnloadedCommandBehavior<TReturn>
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

    // ChartAreaUnloadedCommand
    public class ChartAreaUnloadedCommand : ChartAreaCommandBase<ChartAreaUnloadedCommandBehavior>
    { }

    // ChartAreaUnloadedCommandBehavior
    public class ChartAreaUnloadedCommandBehavior : ChartAreaUnloadedCommandBehavior<object>
    { }

    // ChartAreaUnloadedCommandWithEventArgs	
    public class ChartAreaUnloadedCommandWithEventArgs : ChartAreaUnloadedCommand<RoutedEventArgs, ChartAreaUnloadedCommandBehaviorWithEventArgs>
    { }

    // ChartAreaUnloadedCommandBehaviorWithEventArgs
    public class ChartAreaUnloadedCommandBehaviorWithEventArgs : ChartAreaUnloadedCommandBehavior<RoutedEventArgs>
    {
        public ChartAreaUnloadedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaToolTipOpening
    // ChartAreaToolTipOpeningCommand<T, TBehavior>
    public class ChartAreaToolTipOpeningCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaToolTipOpeningCommandBehavior<T>, new()
    { }

    // ChartAreaToolTipOpeningCommandBehavior<TReturn>
    public class ChartAreaToolTipOpeningCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, ToolTipEventArgs>
    {
        public ChartAreaToolTipOpeningCommandBehavior(Func<object, ToolTipEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaToolTipOpeningCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ToolTipOpening += OnEventRaised;
        }
    }

    // ChartAreaToolTipOpeningCommand
    public class ChartAreaToolTipOpeningCommand : ChartAreaCommandBase<ChartAreaToolTipOpeningCommandBehavior>
    { }

    // ChartAreaToolTipOpeningCommandBehavior
    public class ChartAreaToolTipOpeningCommandBehavior : ChartAreaToolTipOpeningCommandBehavior<object>
    { }

    // ChartAreaToolTipOpeningCommandWithEventArgs	
    public class ChartAreaToolTipOpeningCommandWithEventArgs : ChartAreaToolTipOpeningCommand<ToolTipEventArgs, ChartAreaToolTipOpeningCommandBehaviorWithEventArgs>
    { }

    // ChartAreaToolTipOpeningCommandBehaviorWithEventArgs
    public class ChartAreaToolTipOpeningCommandBehaviorWithEventArgs : ChartAreaToolTipOpeningCommandBehavior<ToolTipEventArgs>
    {
        public ChartAreaToolTipOpeningCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaToolTipClosing
    // ChartAreaToolTipClosingCommand<T, TBehavior>
    public class ChartAreaToolTipClosingCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaToolTipClosingCommandBehavior<T>, new()
    { }

    // ChartAreaToolTipClosingCommandBehavior<TReturn>
    public class ChartAreaToolTipClosingCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, ToolTipEventArgs>
    {
        public ChartAreaToolTipClosingCommandBehavior(Func<object, ToolTipEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaToolTipClosingCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ToolTipClosing += OnEventRaised;
        }
    }

    // ChartAreaToolTipClosingCommand
    public class ChartAreaToolTipClosingCommand : ChartAreaCommandBase<ChartAreaToolTipClosingCommandBehavior>
    { }

    // ChartAreaToolTipClosingCommandBehavior
    public class ChartAreaToolTipClosingCommandBehavior : ChartAreaToolTipClosingCommandBehavior<object>
    { }

    // ChartAreaToolTipClosingCommandWithEventArgs	
    public class ChartAreaToolTipClosingCommandWithEventArgs : ChartAreaToolTipClosingCommand<ToolTipEventArgs, ChartAreaToolTipClosingCommandBehaviorWithEventArgs>
    { }

    // ChartAreaToolTipClosingCommandBehaviorWithEventArgs
    public class ChartAreaToolTipClosingCommandBehaviorWithEventArgs : ChartAreaToolTipClosingCommandBehavior<ToolTipEventArgs>
    {
        public ChartAreaToolTipClosingCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaContextMenuOpening
    // ChartAreaContextMenuOpeningCommand<T, TBehavior>
    public class ChartAreaContextMenuOpeningCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaContextMenuOpeningCommandBehavior<T>, new()
    { }

    // ChartAreaContextMenuOpeningCommandBehavior<TReturn>
    public class ChartAreaContextMenuOpeningCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, ContextMenuEventArgs>
    {
        public ChartAreaContextMenuOpeningCommandBehavior(Func<object, ContextMenuEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaContextMenuOpeningCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ContextMenuOpening += OnEventRaised;
        }
    }

    // ChartAreaContextMenuOpeningCommand
    public class ChartAreaContextMenuOpeningCommand : ChartAreaCommandBase<ChartAreaContextMenuOpeningCommandBehavior>
    { }

    // ChartAreaContextMenuOpeningCommandBehavior
    public class ChartAreaContextMenuOpeningCommandBehavior : ChartAreaContextMenuOpeningCommandBehavior<object>
    { }

    // ChartAreaContextMenuOpeningCommandWithEventArgs	
    public class ChartAreaContextMenuOpeningCommandWithEventArgs : ChartAreaContextMenuOpeningCommand<ContextMenuEventArgs, ChartAreaContextMenuOpeningCommandBehaviorWithEventArgs>
    { }

    // ChartAreaContextMenuOpeningCommandBehaviorWithEventArgs
    public class ChartAreaContextMenuOpeningCommandBehaviorWithEventArgs : ChartAreaContextMenuOpeningCommandBehavior<ContextMenuEventArgs>
    {
        public ChartAreaContextMenuOpeningCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaContextMenuClosing
    // ChartAreaContextMenuClosingCommand<T, TBehavior>
    public class ChartAreaContextMenuClosingCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaContextMenuClosingCommandBehavior<T>, new()
    { }

    // ChartAreaContextMenuClosingCommandBehavior<TReturn>
    public class ChartAreaContextMenuClosingCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, ContextMenuEventArgs>
    {
        public ChartAreaContextMenuClosingCommandBehavior(Func<object, ContextMenuEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaContextMenuClosingCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ContextMenuClosing += OnEventRaised;
        }
    }

    // ChartAreaContextMenuClosingCommand
    public class ChartAreaContextMenuClosingCommand : ChartAreaCommandBase<ChartAreaContextMenuClosingCommandBehavior>
    { }

    // ChartAreaContextMenuClosingCommandBehavior
    public class ChartAreaContextMenuClosingCommandBehavior : ChartAreaContextMenuClosingCommandBehavior<object>
    { }

    // ChartAreaContextMenuClosingCommandWithEventArgs	
    public class ChartAreaContextMenuClosingCommandWithEventArgs : ChartAreaContextMenuClosingCommand<ContextMenuEventArgs, ChartAreaContextMenuClosingCommandBehaviorWithEventArgs>
    { }

    // ChartAreaContextMenuClosingCommandBehaviorWithEventArgs
    public class ChartAreaContextMenuClosingCommandBehaviorWithEventArgs : ChartAreaContextMenuClosingCommandBehavior<ContextMenuEventArgs>
    {
        public ChartAreaContextMenuClosingCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaPreviewMouseDown
    // ChartAreaPreviewMouseDownCommand<T, TBehavior>
    public class ChartAreaPreviewMouseDownCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaPreviewMouseDownCommandBehavior<T>, new()
    { }

    // ChartAreaPreviewMouseDownCommandBehavior<TReturn>
    public class ChartAreaPreviewMouseDownCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public ChartAreaPreviewMouseDownCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaPreviewMouseDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewMouseDown += OnEventRaised;
        }
    }

    // ChartAreaPreviewMouseDownCommand
    public class ChartAreaPreviewMouseDownCommand : ChartAreaCommandBase<ChartAreaPreviewMouseDownCommandBehavior>
    { }

    // ChartAreaPreviewMouseDownCommandBehavior
    public class ChartAreaPreviewMouseDownCommandBehavior : ChartAreaPreviewMouseDownCommandBehavior<object>
    { }

    // ChartAreaPreviewMouseDownCommandWithEventArgs	
    public class ChartAreaPreviewMouseDownCommandWithEventArgs : ChartAreaPreviewMouseDownCommand<MouseButtonEventArgs, ChartAreaPreviewMouseDownCommandBehaviorWithEventArgs>
    { }

    // ChartAreaPreviewMouseDownCommandBehaviorWithEventArgs
    public class ChartAreaPreviewMouseDownCommandBehaviorWithEventArgs : ChartAreaPreviewMouseDownCommandBehavior<MouseButtonEventArgs>
    {
        public ChartAreaPreviewMouseDownCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaMouseDown
    // ChartAreaMouseDownCommand<T, TBehavior>
    public class ChartAreaMouseDownCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaMouseDownCommandBehavior<T>, new()
    { }

    // ChartAreaMouseDownCommandBehavior<TReturn>
    public class ChartAreaMouseDownCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public ChartAreaMouseDownCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaMouseDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseDown += OnEventRaised;
        }
    }

    // ChartAreaMouseDownCommand
    public class ChartAreaMouseDownCommand : ChartAreaCommandBase<ChartAreaMouseDownCommandBehavior>
    { }

    // ChartAreaMouseDownCommandBehavior
    public class ChartAreaMouseDownCommandBehavior : ChartAreaMouseDownCommandBehavior<object>
    { }

    // ChartAreaMouseDownCommandWithEventArgs	
    public class ChartAreaMouseDownCommandWithEventArgs : ChartAreaMouseDownCommand<MouseButtonEventArgs, ChartAreaMouseDownCommandBehaviorWithEventArgs>
    { }

    // ChartAreaMouseDownCommandBehaviorWithEventArgs
    public class ChartAreaMouseDownCommandBehaviorWithEventArgs : ChartAreaMouseDownCommandBehavior<MouseButtonEventArgs>
    {
        public ChartAreaMouseDownCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaPreviewMouseUp
    // ChartAreaPreviewMouseUpCommand<T, TBehavior>
    public class ChartAreaPreviewMouseUpCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaPreviewMouseUpCommandBehavior<T>, new()
    { }

    // ChartAreaPreviewMouseUpCommandBehavior<TReturn>
    public class ChartAreaPreviewMouseUpCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public ChartAreaPreviewMouseUpCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaPreviewMouseUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewMouseUp += OnEventRaised;
        }
    }

    // ChartAreaPreviewMouseUpCommand
    public class ChartAreaPreviewMouseUpCommand : ChartAreaCommandBase<ChartAreaPreviewMouseUpCommandBehavior>
    { }

    // ChartAreaPreviewMouseUpCommandBehavior
    public class ChartAreaPreviewMouseUpCommandBehavior : ChartAreaPreviewMouseUpCommandBehavior<object>
    { }

    // ChartAreaPreviewMouseUpCommandWithEventArgs	
    public class ChartAreaPreviewMouseUpCommandWithEventArgs : ChartAreaPreviewMouseUpCommand<MouseButtonEventArgs, ChartAreaPreviewMouseUpCommandBehaviorWithEventArgs>
    { }

    // ChartAreaPreviewMouseUpCommandBehaviorWithEventArgs
    public class ChartAreaPreviewMouseUpCommandBehaviorWithEventArgs : ChartAreaPreviewMouseUpCommandBehavior<MouseButtonEventArgs>
    {
        public ChartAreaPreviewMouseUpCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaMouseUp
    // ChartAreaMouseUpCommand<T, TBehavior>
    public class ChartAreaMouseUpCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaMouseUpCommandBehavior<T>, new()
    { }

    // ChartAreaMouseUpCommandBehavior<TReturn>
    public class ChartAreaMouseUpCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public ChartAreaMouseUpCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaMouseUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseUp += OnEventRaised;
        }
    }

    // ChartAreaMouseUpCommand
    public class ChartAreaMouseUpCommand : ChartAreaCommandBase<ChartAreaMouseUpCommandBehavior>
    { }

    // ChartAreaMouseUpCommandBehavior
    public class ChartAreaMouseUpCommandBehavior : ChartAreaMouseUpCommandBehavior<object>
    { }

    // ChartAreaMouseUpCommandWithEventArgs	
    public class ChartAreaMouseUpCommandWithEventArgs : ChartAreaMouseUpCommand<MouseButtonEventArgs, ChartAreaMouseUpCommandBehaviorWithEventArgs>
    { }

    // ChartAreaMouseUpCommandBehaviorWithEventArgs
    public class ChartAreaMouseUpCommandBehaviorWithEventArgs : ChartAreaMouseUpCommandBehavior<MouseButtonEventArgs>
    {
        public ChartAreaMouseUpCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaPreviewMouseLeftButtonDown
    // ChartAreaPreviewMouseLeftButtonDownCommand<T, TBehavior>
    public class ChartAreaPreviewMouseLeftButtonDownCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaPreviewMouseLeftButtonDownCommandBehavior<T>, new()
    { }

    // ChartAreaPreviewMouseLeftButtonDownCommandBehavior<TReturn>
    public class ChartAreaPreviewMouseLeftButtonDownCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public ChartAreaPreviewMouseLeftButtonDownCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaPreviewMouseLeftButtonDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewMouseLeftButtonDown += OnEventRaised;
        }
    }

    // ChartAreaPreviewMouseLeftButtonDownCommand
    public class ChartAreaPreviewMouseLeftButtonDownCommand : ChartAreaCommandBase<ChartAreaPreviewMouseLeftButtonDownCommandBehavior>
    { }

    // ChartAreaPreviewMouseLeftButtonDownCommandBehavior
    public class ChartAreaPreviewMouseLeftButtonDownCommandBehavior : ChartAreaPreviewMouseLeftButtonDownCommandBehavior<object>
    { }

    // ChartAreaPreviewMouseLeftButtonDownCommandWithEventArgs	
    public class ChartAreaPreviewMouseLeftButtonDownCommandWithEventArgs : ChartAreaPreviewMouseLeftButtonDownCommand<MouseButtonEventArgs, ChartAreaPreviewMouseLeftButtonDownCommandBehaviorWithEventArgs>
    { }

    // ChartAreaPreviewMouseLeftButtonDownCommandBehaviorWithEventArgs
    public class ChartAreaPreviewMouseLeftButtonDownCommandBehaviorWithEventArgs : ChartAreaPreviewMouseLeftButtonDownCommandBehavior<MouseButtonEventArgs>
    {
        public ChartAreaPreviewMouseLeftButtonDownCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaMouseLeftButtonDown
    // ChartAreaMouseLeftButtonDownCommand<T, TBehavior>
    public class ChartAreaMouseLeftButtonDownCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaMouseLeftButtonDownCommandBehavior<T>, new()
    { }

    // ChartAreaMouseLeftButtonDownCommandBehavior<TReturn>
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

    // ChartAreaMouseLeftButtonDownCommand
    public class ChartAreaMouseLeftButtonDownCommand : ChartAreaCommandBase<ChartAreaMouseLeftButtonDownCommandBehavior>
    { }

    // ChartAreaMouseLeftButtonDownCommandBehavior
    public class ChartAreaMouseLeftButtonDownCommandBehavior : ChartAreaMouseLeftButtonDownCommandBehavior<object>
    { }

    // ChartAreaMouseLeftButtonDownCommandWithEventArgs	
    public class ChartAreaMouseLeftButtonDownCommandWithEventArgs : ChartAreaMouseLeftButtonDownCommand<MouseButtonEventArgs, ChartAreaMouseLeftButtonDownCommandBehaviorWithEventArgs>
    { }

    // ChartAreaMouseLeftButtonDownCommandBehaviorWithEventArgs
    public class ChartAreaMouseLeftButtonDownCommandBehaviorWithEventArgs : ChartAreaMouseLeftButtonDownCommandBehavior<MouseButtonEventArgs>
    {
        public ChartAreaMouseLeftButtonDownCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaPreviewMouseLeftButtonUp
    // ChartAreaPreviewMouseLeftButtonUpCommand<T, TBehavior>
    public class ChartAreaPreviewMouseLeftButtonUpCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaPreviewMouseLeftButtonUpCommandBehavior<T>, new()
    { }

    // ChartAreaPreviewMouseLeftButtonUpCommandBehavior<TReturn>
    public class ChartAreaPreviewMouseLeftButtonUpCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public ChartAreaPreviewMouseLeftButtonUpCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaPreviewMouseLeftButtonUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewMouseLeftButtonUp += OnEventRaised;
        }
    }

    // ChartAreaPreviewMouseLeftButtonUpCommand
    public class ChartAreaPreviewMouseLeftButtonUpCommand : ChartAreaCommandBase<ChartAreaPreviewMouseLeftButtonUpCommandBehavior>
    { }

    // ChartAreaPreviewMouseLeftButtonUpCommandBehavior
    public class ChartAreaPreviewMouseLeftButtonUpCommandBehavior : ChartAreaPreviewMouseLeftButtonUpCommandBehavior<object>
    { }

    // ChartAreaPreviewMouseLeftButtonUpCommandWithEventArgs	
    public class ChartAreaPreviewMouseLeftButtonUpCommandWithEventArgs : ChartAreaPreviewMouseLeftButtonUpCommand<MouseButtonEventArgs, ChartAreaPreviewMouseLeftButtonUpCommandBehaviorWithEventArgs>
    { }

    // ChartAreaPreviewMouseLeftButtonUpCommandBehaviorWithEventArgs
    public class ChartAreaPreviewMouseLeftButtonUpCommandBehaviorWithEventArgs : ChartAreaPreviewMouseLeftButtonUpCommandBehavior<MouseButtonEventArgs>
    {
        public ChartAreaPreviewMouseLeftButtonUpCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaMouseLeftButtonUp
    // ChartAreaMouseLeftButtonUpCommand<T, TBehavior>
    public class ChartAreaMouseLeftButtonUpCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaMouseLeftButtonUpCommandBehavior<T>, new()
    { }

    // ChartAreaMouseLeftButtonUpCommandBehavior<TReturn>
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

    // ChartAreaMouseLeftButtonUpCommand
    public class ChartAreaMouseLeftButtonUpCommand : ChartAreaCommandBase<ChartAreaMouseLeftButtonUpCommandBehavior>
    { }

    // ChartAreaMouseLeftButtonUpCommandBehavior
    public class ChartAreaMouseLeftButtonUpCommandBehavior : ChartAreaMouseLeftButtonUpCommandBehavior<object>
    { }

    // ChartAreaMouseLeftButtonUpCommandWithEventArgs	
    public class ChartAreaMouseLeftButtonUpCommandWithEventArgs : ChartAreaMouseLeftButtonUpCommand<MouseButtonEventArgs, ChartAreaMouseLeftButtonUpCommandBehaviorWithEventArgs>
    { }

    // ChartAreaMouseLeftButtonUpCommandBehaviorWithEventArgs
    public class ChartAreaMouseLeftButtonUpCommandBehaviorWithEventArgs : ChartAreaMouseLeftButtonUpCommandBehavior<MouseButtonEventArgs>
    {
        public ChartAreaMouseLeftButtonUpCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaPreviewMouseRightButtonDown
    // ChartAreaPreviewMouseRightButtonDownCommand<T, TBehavior>
    public class ChartAreaPreviewMouseRightButtonDownCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaPreviewMouseRightButtonDownCommandBehavior<T>, new()
    { }

    // ChartAreaPreviewMouseRightButtonDownCommandBehavior<TReturn>
    public class ChartAreaPreviewMouseRightButtonDownCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public ChartAreaPreviewMouseRightButtonDownCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaPreviewMouseRightButtonDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewMouseRightButtonDown += OnEventRaised;
        }
    }

    // ChartAreaPreviewMouseRightButtonDownCommand
    public class ChartAreaPreviewMouseRightButtonDownCommand : ChartAreaCommandBase<ChartAreaPreviewMouseRightButtonDownCommandBehavior>
    { }

    // ChartAreaPreviewMouseRightButtonDownCommandBehavior
    public class ChartAreaPreviewMouseRightButtonDownCommandBehavior : ChartAreaPreviewMouseRightButtonDownCommandBehavior<object>
    { }

    // ChartAreaPreviewMouseRightButtonDownCommandWithEventArgs	
    public class ChartAreaPreviewMouseRightButtonDownCommandWithEventArgs : ChartAreaPreviewMouseRightButtonDownCommand<MouseButtonEventArgs, ChartAreaPreviewMouseRightButtonDownCommandBehaviorWithEventArgs>
    { }

    // ChartAreaPreviewMouseRightButtonDownCommandBehaviorWithEventArgs
    public class ChartAreaPreviewMouseRightButtonDownCommandBehaviorWithEventArgs : ChartAreaPreviewMouseRightButtonDownCommandBehavior<MouseButtonEventArgs>
    {
        public ChartAreaPreviewMouseRightButtonDownCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaMouseRightButtonDown
    // ChartAreaMouseRightButtonDownCommand<T, TBehavior>
    public class ChartAreaMouseRightButtonDownCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaMouseRightButtonDownCommandBehavior<T>, new()
    { }

    // ChartAreaMouseRightButtonDownCommandBehavior<TReturn>
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

    // ChartAreaMouseRightButtonDownCommand
    public class ChartAreaMouseRightButtonDownCommand : ChartAreaCommandBase<ChartAreaMouseRightButtonDownCommandBehavior>
    { }

    // ChartAreaMouseRightButtonDownCommandBehavior
    public class ChartAreaMouseRightButtonDownCommandBehavior : ChartAreaMouseRightButtonDownCommandBehavior<object>
    { }

    // ChartAreaMouseRightButtonDownCommandWithEventArgs	
    public class ChartAreaMouseRightButtonDownCommandWithEventArgs : ChartAreaMouseRightButtonDownCommand<MouseButtonEventArgs, ChartAreaMouseRightButtonDownCommandBehaviorWithEventArgs>
    { }

    // ChartAreaMouseRightButtonDownCommandBehaviorWithEventArgs
    public class ChartAreaMouseRightButtonDownCommandBehaviorWithEventArgs : ChartAreaMouseRightButtonDownCommandBehavior<MouseButtonEventArgs>
    {
        public ChartAreaMouseRightButtonDownCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaPreviewMouseRightButtonUp
    // ChartAreaPreviewMouseRightButtonUpCommand<T, TBehavior>
    public class ChartAreaPreviewMouseRightButtonUpCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaPreviewMouseRightButtonUpCommandBehavior<T>, new()
    { }

    // ChartAreaPreviewMouseRightButtonUpCommandBehavior<TReturn>
    public class ChartAreaPreviewMouseRightButtonUpCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public ChartAreaPreviewMouseRightButtonUpCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaPreviewMouseRightButtonUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewMouseRightButtonUp += OnEventRaised;
        }
    }

    // ChartAreaPreviewMouseRightButtonUpCommand
    public class ChartAreaPreviewMouseRightButtonUpCommand : ChartAreaCommandBase<ChartAreaPreviewMouseRightButtonUpCommandBehavior>
    { }

    // ChartAreaPreviewMouseRightButtonUpCommandBehavior
    public class ChartAreaPreviewMouseRightButtonUpCommandBehavior : ChartAreaPreviewMouseRightButtonUpCommandBehavior<object>
    { }

    // ChartAreaPreviewMouseRightButtonUpCommandWithEventArgs	
    public class ChartAreaPreviewMouseRightButtonUpCommandWithEventArgs : ChartAreaPreviewMouseRightButtonUpCommand<MouseButtonEventArgs, ChartAreaPreviewMouseRightButtonUpCommandBehaviorWithEventArgs>
    { }

    // ChartAreaPreviewMouseRightButtonUpCommandBehaviorWithEventArgs
    public class ChartAreaPreviewMouseRightButtonUpCommandBehaviorWithEventArgs : ChartAreaPreviewMouseRightButtonUpCommandBehavior<MouseButtonEventArgs>
    {
        public ChartAreaPreviewMouseRightButtonUpCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaMouseRightButtonUp
    // ChartAreaMouseRightButtonUpCommand<T, TBehavior>
    public class ChartAreaMouseRightButtonUpCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaMouseRightButtonUpCommandBehavior<T>, new()
    { }

    // ChartAreaMouseRightButtonUpCommandBehavior<TReturn>
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

    // ChartAreaMouseRightButtonUpCommand
    public class ChartAreaMouseRightButtonUpCommand : ChartAreaCommandBase<ChartAreaMouseRightButtonUpCommandBehavior>
    { }

    // ChartAreaMouseRightButtonUpCommandBehavior
    public class ChartAreaMouseRightButtonUpCommandBehavior : ChartAreaMouseRightButtonUpCommandBehavior<object>
    { }

    // ChartAreaMouseRightButtonUpCommandWithEventArgs	
    public class ChartAreaMouseRightButtonUpCommandWithEventArgs : ChartAreaMouseRightButtonUpCommand<MouseButtonEventArgs, ChartAreaMouseRightButtonUpCommandBehaviorWithEventArgs>
    { }

    // ChartAreaMouseRightButtonUpCommandBehaviorWithEventArgs
    public class ChartAreaMouseRightButtonUpCommandBehaviorWithEventArgs : ChartAreaMouseRightButtonUpCommandBehavior<MouseButtonEventArgs>
    {
        public ChartAreaMouseRightButtonUpCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaPreviewMouseMove
    // ChartAreaPreviewMouseMoveCommand<T, TBehavior>
    public class ChartAreaPreviewMouseMoveCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaPreviewMouseMoveCommandBehavior<T>, new()
    { }

    // ChartAreaPreviewMouseMoveCommandBehavior<TReturn>
    public class ChartAreaPreviewMouseMoveCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, MouseEventArgs>
    {
        public ChartAreaPreviewMouseMoveCommandBehavior(Func<object, MouseEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaPreviewMouseMoveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewMouseMove += OnEventRaised;
        }
    }

    // ChartAreaPreviewMouseMoveCommand
    public class ChartAreaPreviewMouseMoveCommand : ChartAreaCommandBase<ChartAreaPreviewMouseMoveCommandBehavior>
    { }

    // ChartAreaPreviewMouseMoveCommandBehavior
    public class ChartAreaPreviewMouseMoveCommandBehavior : ChartAreaPreviewMouseMoveCommandBehavior<object>
    { }

    // ChartAreaPreviewMouseMoveCommandWithEventArgs	
    public class ChartAreaPreviewMouseMoveCommandWithEventArgs : ChartAreaPreviewMouseMoveCommand<MouseEventArgs, ChartAreaPreviewMouseMoveCommandBehaviorWithEventArgs>
    { }

    // ChartAreaPreviewMouseMoveCommandBehaviorWithEventArgs
    public class ChartAreaPreviewMouseMoveCommandBehaviorWithEventArgs : ChartAreaPreviewMouseMoveCommandBehavior<MouseEventArgs>
    {
        public ChartAreaPreviewMouseMoveCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaMouseMove
    // ChartAreaMouseMoveCommand<T, TBehavior>
    public class ChartAreaMouseMoveCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaMouseMoveCommandBehavior<T>, new()
    { }

    // ChartAreaMouseMoveCommandBehavior<TReturn>
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

    // ChartAreaMouseMoveCommand
    public class ChartAreaMouseMoveCommand : ChartAreaCommandBase<ChartAreaMouseMoveCommandBehavior>
    { }

    // ChartAreaMouseMoveCommandBehavior
    public class ChartAreaMouseMoveCommandBehavior : ChartAreaMouseMoveCommandBehavior<object>
    { }

    // ChartAreaMouseMoveCommandWithEventArgs	
    public class ChartAreaMouseMoveCommandWithEventArgs : ChartAreaMouseMoveCommand<MouseEventArgs, ChartAreaMouseMoveCommandBehaviorWithEventArgs>
    { }

    // ChartAreaMouseMoveCommandBehaviorWithEventArgs
    public class ChartAreaMouseMoveCommandBehaviorWithEventArgs : ChartAreaMouseMoveCommandBehavior<MouseEventArgs>
    {
        public ChartAreaMouseMoveCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaPreviewMouseWheel
    // ChartAreaPreviewMouseWheelCommand<T, TBehavior>
    public class ChartAreaPreviewMouseWheelCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaPreviewMouseWheelCommandBehavior<T>, new()
    { }

    // ChartAreaPreviewMouseWheelCommandBehavior<TReturn>
    public class ChartAreaPreviewMouseWheelCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, MouseWheelEventArgs>
    {
        public ChartAreaPreviewMouseWheelCommandBehavior(Func<object, MouseWheelEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaPreviewMouseWheelCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewMouseWheel += OnEventRaised;
        }
    }

    // ChartAreaPreviewMouseWheelCommand
    public class ChartAreaPreviewMouseWheelCommand : ChartAreaCommandBase<ChartAreaPreviewMouseWheelCommandBehavior>
    { }

    // ChartAreaPreviewMouseWheelCommandBehavior
    public class ChartAreaPreviewMouseWheelCommandBehavior : ChartAreaPreviewMouseWheelCommandBehavior<object>
    { }

    // ChartAreaPreviewMouseWheelCommandWithEventArgs	
    public class ChartAreaPreviewMouseWheelCommandWithEventArgs : ChartAreaPreviewMouseWheelCommand<MouseWheelEventArgs, ChartAreaPreviewMouseWheelCommandBehaviorWithEventArgs>
    { }

    // ChartAreaPreviewMouseWheelCommandBehaviorWithEventArgs
    public class ChartAreaPreviewMouseWheelCommandBehaviorWithEventArgs : ChartAreaPreviewMouseWheelCommandBehavior<MouseWheelEventArgs>
    {
        public ChartAreaPreviewMouseWheelCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaMouseWheel
    // ChartAreaMouseWheelCommand<T, TBehavior>
    public class ChartAreaMouseWheelCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaMouseWheelCommandBehavior<T>, new()
    { }

    // ChartAreaMouseWheelCommandBehavior<TReturn>
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

    // ChartAreaMouseWheelCommand
    public class ChartAreaMouseWheelCommand : ChartAreaCommandBase<ChartAreaMouseWheelCommandBehavior>
    { }

    // ChartAreaMouseWheelCommandBehavior
    public class ChartAreaMouseWheelCommandBehavior : ChartAreaMouseWheelCommandBehavior<object>
    { }

    // ChartAreaMouseWheelCommandWithEventArgs	
    public class ChartAreaMouseWheelCommandWithEventArgs : ChartAreaMouseWheelCommand<MouseWheelEventArgs, ChartAreaMouseWheelCommandBehaviorWithEventArgs>
    { }

    // ChartAreaMouseWheelCommandBehaviorWithEventArgs
    public class ChartAreaMouseWheelCommandBehaviorWithEventArgs : ChartAreaMouseWheelCommandBehavior<MouseWheelEventArgs>
    {
        public ChartAreaMouseWheelCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaMouseEnter
    // ChartAreaMouseEnterCommand<T, TBehavior>
    public class ChartAreaMouseEnterCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaMouseEnterCommandBehavior<T>, new()
    { }

    // ChartAreaMouseEnterCommandBehavior<TReturn>
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

    // ChartAreaMouseEnterCommand
    public class ChartAreaMouseEnterCommand : ChartAreaCommandBase<ChartAreaMouseEnterCommandBehavior>
    { }

    // ChartAreaMouseEnterCommandBehavior
    public class ChartAreaMouseEnterCommandBehavior : ChartAreaMouseEnterCommandBehavior<object>
    { }

    // ChartAreaMouseEnterCommandWithEventArgs	
    public class ChartAreaMouseEnterCommandWithEventArgs : ChartAreaMouseEnterCommand<MouseEventArgs, ChartAreaMouseEnterCommandBehaviorWithEventArgs>
    { }

    // ChartAreaMouseEnterCommandBehaviorWithEventArgs
    public class ChartAreaMouseEnterCommandBehaviorWithEventArgs : ChartAreaMouseEnterCommandBehavior<MouseEventArgs>
    {
        public ChartAreaMouseEnterCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaMouseLeave
    // ChartAreaMouseLeaveCommand<T, TBehavior>
    public class ChartAreaMouseLeaveCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaMouseLeaveCommandBehavior<T>, new()
    { }

    // ChartAreaMouseLeaveCommandBehavior<TReturn>
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

    // ChartAreaMouseLeaveCommand
    public class ChartAreaMouseLeaveCommand : ChartAreaCommandBase<ChartAreaMouseLeaveCommandBehavior>
    { }

    // ChartAreaMouseLeaveCommandBehavior
    public class ChartAreaMouseLeaveCommandBehavior : ChartAreaMouseLeaveCommandBehavior<object>
    { }

    // ChartAreaMouseLeaveCommandWithEventArgs	
    public class ChartAreaMouseLeaveCommandWithEventArgs : ChartAreaMouseLeaveCommand<MouseEventArgs, ChartAreaMouseLeaveCommandBehaviorWithEventArgs>
    { }

    // ChartAreaMouseLeaveCommandBehaviorWithEventArgs
    public class ChartAreaMouseLeaveCommandBehaviorWithEventArgs : ChartAreaMouseLeaveCommandBehavior<MouseEventArgs>
    {
        public ChartAreaMouseLeaveCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaGotMouseCapture
    // ChartAreaGotMouseCaptureCommand<T, TBehavior>
    public class ChartAreaGotMouseCaptureCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaGotMouseCaptureCommandBehavior<T>, new()
    { }

    // ChartAreaGotMouseCaptureCommandBehavior<TReturn>
    public class ChartAreaGotMouseCaptureCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, MouseEventArgs>
    {
        public ChartAreaGotMouseCaptureCommandBehavior(Func<object, MouseEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaGotMouseCaptureCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.GotMouseCapture += OnEventRaised;
        }
    }

    // ChartAreaGotMouseCaptureCommand
    public class ChartAreaGotMouseCaptureCommand : ChartAreaCommandBase<ChartAreaGotMouseCaptureCommandBehavior>
    { }

    // ChartAreaGotMouseCaptureCommandBehavior
    public class ChartAreaGotMouseCaptureCommandBehavior : ChartAreaGotMouseCaptureCommandBehavior<object>
    { }

    // ChartAreaGotMouseCaptureCommandWithEventArgs	
    public class ChartAreaGotMouseCaptureCommandWithEventArgs : ChartAreaGotMouseCaptureCommand<MouseEventArgs, ChartAreaGotMouseCaptureCommandBehaviorWithEventArgs>
    { }

    // ChartAreaGotMouseCaptureCommandBehaviorWithEventArgs
    public class ChartAreaGotMouseCaptureCommandBehaviorWithEventArgs : ChartAreaGotMouseCaptureCommandBehavior<MouseEventArgs>
    {
        public ChartAreaGotMouseCaptureCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaLostMouseCapture
    // ChartAreaLostMouseCaptureCommand<T, TBehavior>
    public class ChartAreaLostMouseCaptureCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaLostMouseCaptureCommandBehavior<T>, new()
    { }

    // ChartAreaLostMouseCaptureCommandBehavior<TReturn>
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

    // ChartAreaLostMouseCaptureCommand
    public class ChartAreaLostMouseCaptureCommand : ChartAreaCommandBase<ChartAreaLostMouseCaptureCommandBehavior>
    { }

    // ChartAreaLostMouseCaptureCommandBehavior
    public class ChartAreaLostMouseCaptureCommandBehavior : ChartAreaLostMouseCaptureCommandBehavior<object>
    { }

    // ChartAreaLostMouseCaptureCommandWithEventArgs	
    public class ChartAreaLostMouseCaptureCommandWithEventArgs : ChartAreaLostMouseCaptureCommand<MouseEventArgs, ChartAreaLostMouseCaptureCommandBehaviorWithEventArgs>
    { }

    // ChartAreaLostMouseCaptureCommandBehaviorWithEventArgs
    public class ChartAreaLostMouseCaptureCommandBehaviorWithEventArgs : ChartAreaLostMouseCaptureCommandBehavior<MouseEventArgs>
    {
        public ChartAreaLostMouseCaptureCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaQueryCursor
    // ChartAreaQueryCursorCommand<T, TBehavior>
    public class ChartAreaQueryCursorCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaQueryCursorCommandBehavior<T>, new()
    { }

    // ChartAreaQueryCursorCommandBehavior<TReturn>
    public class ChartAreaQueryCursorCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, QueryCursorEventArgs>
    {
        public ChartAreaQueryCursorCommandBehavior(Func<object, QueryCursorEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaQueryCursorCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.QueryCursor += OnEventRaised;
        }
    }

    // ChartAreaQueryCursorCommand
    public class ChartAreaQueryCursorCommand : ChartAreaCommandBase<ChartAreaQueryCursorCommandBehavior>
    { }

    // ChartAreaQueryCursorCommandBehavior
    public class ChartAreaQueryCursorCommandBehavior : ChartAreaQueryCursorCommandBehavior<object>
    { }

    // ChartAreaQueryCursorCommandWithEventArgs	
    public class ChartAreaQueryCursorCommandWithEventArgs : ChartAreaQueryCursorCommand<QueryCursorEventArgs, ChartAreaQueryCursorCommandBehaviorWithEventArgs>
    { }

    // ChartAreaQueryCursorCommandBehaviorWithEventArgs
    public class ChartAreaQueryCursorCommandBehaviorWithEventArgs : ChartAreaQueryCursorCommandBehavior<QueryCursorEventArgs>
    {
        public ChartAreaQueryCursorCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaPreviewStylusDown
    // ChartAreaPreviewStylusDownCommand<T, TBehavior>
    public class ChartAreaPreviewStylusDownCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaPreviewStylusDownCommandBehavior<T>, new()
    { }

    // ChartAreaPreviewStylusDownCommandBehavior<TReturn>
    public class ChartAreaPreviewStylusDownCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, StylusDownEventArgs>
    {
        public ChartAreaPreviewStylusDownCommandBehavior(Func<object, StylusDownEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaPreviewStylusDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewStylusDown += OnEventRaised;
        }
    }

    // ChartAreaPreviewStylusDownCommand
    public class ChartAreaPreviewStylusDownCommand : ChartAreaCommandBase<ChartAreaPreviewStylusDownCommandBehavior>
    { }

    // ChartAreaPreviewStylusDownCommandBehavior
    public class ChartAreaPreviewStylusDownCommandBehavior : ChartAreaPreviewStylusDownCommandBehavior<object>
    { }

    // ChartAreaPreviewStylusDownCommandWithEventArgs	
    public class ChartAreaPreviewStylusDownCommandWithEventArgs : ChartAreaPreviewStylusDownCommand<StylusDownEventArgs, ChartAreaPreviewStylusDownCommandBehaviorWithEventArgs>
    { }

    // ChartAreaPreviewStylusDownCommandBehaviorWithEventArgs
    public class ChartAreaPreviewStylusDownCommandBehaviorWithEventArgs : ChartAreaPreviewStylusDownCommandBehavior<StylusDownEventArgs>
    {
        public ChartAreaPreviewStylusDownCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaStylusDown
    // ChartAreaStylusDownCommand<T, TBehavior>
    public class ChartAreaStylusDownCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaStylusDownCommandBehavior<T>, new()
    { }

    // ChartAreaStylusDownCommandBehavior<TReturn>
    public class ChartAreaStylusDownCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, StylusDownEventArgs>
    {
        public ChartAreaStylusDownCommandBehavior(Func<object, StylusDownEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaStylusDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusDown += OnEventRaised;
        }
    }

    // ChartAreaStylusDownCommand
    public class ChartAreaStylusDownCommand : ChartAreaCommandBase<ChartAreaStylusDownCommandBehavior>
    { }

    // ChartAreaStylusDownCommandBehavior
    public class ChartAreaStylusDownCommandBehavior : ChartAreaStylusDownCommandBehavior<object>
    { }

    // ChartAreaStylusDownCommandWithEventArgs	
    public class ChartAreaStylusDownCommandWithEventArgs : ChartAreaStylusDownCommand<StylusDownEventArgs, ChartAreaStylusDownCommandBehaviorWithEventArgs>
    { }

    // ChartAreaStylusDownCommandBehaviorWithEventArgs
    public class ChartAreaStylusDownCommandBehaviorWithEventArgs : ChartAreaStylusDownCommandBehavior<StylusDownEventArgs>
    {
        public ChartAreaStylusDownCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaPreviewStylusUp
    // ChartAreaPreviewStylusUpCommand<T, TBehavior>
    public class ChartAreaPreviewStylusUpCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaPreviewStylusUpCommandBehavior<T>, new()
    { }

    // ChartAreaPreviewStylusUpCommandBehavior<TReturn>
    public class ChartAreaPreviewStylusUpCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public ChartAreaPreviewStylusUpCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaPreviewStylusUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewStylusUp += OnEventRaised;
        }
    }

    // ChartAreaPreviewStylusUpCommand
    public class ChartAreaPreviewStylusUpCommand : ChartAreaCommandBase<ChartAreaPreviewStylusUpCommandBehavior>
    { }

    // ChartAreaPreviewStylusUpCommandBehavior
    public class ChartAreaPreviewStylusUpCommandBehavior : ChartAreaPreviewStylusUpCommandBehavior<object>
    { }

    // ChartAreaPreviewStylusUpCommandWithEventArgs	
    public class ChartAreaPreviewStylusUpCommandWithEventArgs : ChartAreaPreviewStylusUpCommand<StylusEventArgs, ChartAreaPreviewStylusUpCommandBehaviorWithEventArgs>
    { }

    // ChartAreaPreviewStylusUpCommandBehaviorWithEventArgs
    public class ChartAreaPreviewStylusUpCommandBehaviorWithEventArgs : ChartAreaPreviewStylusUpCommandBehavior<StylusEventArgs>
    {
        public ChartAreaPreviewStylusUpCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaStylusUp
    // ChartAreaStylusUpCommand<T, TBehavior>
    public class ChartAreaStylusUpCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaStylusUpCommandBehavior<T>, new()
    { }

    // ChartAreaStylusUpCommandBehavior<TReturn>
    public class ChartAreaStylusUpCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public ChartAreaStylusUpCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaStylusUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusUp += OnEventRaised;
        }
    }

    // ChartAreaStylusUpCommand
    public class ChartAreaStylusUpCommand : ChartAreaCommandBase<ChartAreaStylusUpCommandBehavior>
    { }

    // ChartAreaStylusUpCommandBehavior
    public class ChartAreaStylusUpCommandBehavior : ChartAreaStylusUpCommandBehavior<object>
    { }

    // ChartAreaStylusUpCommandWithEventArgs	
    public class ChartAreaStylusUpCommandWithEventArgs : ChartAreaStylusUpCommand<StylusEventArgs, ChartAreaStylusUpCommandBehaviorWithEventArgs>
    { }

    // ChartAreaStylusUpCommandBehaviorWithEventArgs
    public class ChartAreaStylusUpCommandBehaviorWithEventArgs : ChartAreaStylusUpCommandBehavior<StylusEventArgs>
    {
        public ChartAreaStylusUpCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaPreviewStylusMove
    // ChartAreaPreviewStylusMoveCommand<T, TBehavior>
    public class ChartAreaPreviewStylusMoveCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaPreviewStylusMoveCommandBehavior<T>, new()
    { }

    // ChartAreaPreviewStylusMoveCommandBehavior<TReturn>
    public class ChartAreaPreviewStylusMoveCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public ChartAreaPreviewStylusMoveCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaPreviewStylusMoveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewStylusMove += OnEventRaised;
        }
    }

    // ChartAreaPreviewStylusMoveCommand
    public class ChartAreaPreviewStylusMoveCommand : ChartAreaCommandBase<ChartAreaPreviewStylusMoveCommandBehavior>
    { }

    // ChartAreaPreviewStylusMoveCommandBehavior
    public class ChartAreaPreviewStylusMoveCommandBehavior : ChartAreaPreviewStylusMoveCommandBehavior<object>
    { }

    // ChartAreaPreviewStylusMoveCommandWithEventArgs	
    public class ChartAreaPreviewStylusMoveCommandWithEventArgs : ChartAreaPreviewStylusMoveCommand<StylusEventArgs, ChartAreaPreviewStylusMoveCommandBehaviorWithEventArgs>
    { }

    // ChartAreaPreviewStylusMoveCommandBehaviorWithEventArgs
    public class ChartAreaPreviewStylusMoveCommandBehaviorWithEventArgs : ChartAreaPreviewStylusMoveCommandBehavior<StylusEventArgs>
    {
        public ChartAreaPreviewStylusMoveCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaStylusMove
    // ChartAreaStylusMoveCommand<T, TBehavior>
    public class ChartAreaStylusMoveCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaStylusMoveCommandBehavior<T>, new()
    { }

    // ChartAreaStylusMoveCommandBehavior<TReturn>
    public class ChartAreaStylusMoveCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public ChartAreaStylusMoveCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaStylusMoveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusMove += OnEventRaised;
        }
    }

    // ChartAreaStylusMoveCommand
    public class ChartAreaStylusMoveCommand : ChartAreaCommandBase<ChartAreaStylusMoveCommandBehavior>
    { }

    // ChartAreaStylusMoveCommandBehavior
    public class ChartAreaStylusMoveCommandBehavior : ChartAreaStylusMoveCommandBehavior<object>
    { }

    // ChartAreaStylusMoveCommandWithEventArgs	
    public class ChartAreaStylusMoveCommandWithEventArgs : ChartAreaStylusMoveCommand<StylusEventArgs, ChartAreaStylusMoveCommandBehaviorWithEventArgs>
    { }

    // ChartAreaStylusMoveCommandBehaviorWithEventArgs
    public class ChartAreaStylusMoveCommandBehaviorWithEventArgs : ChartAreaStylusMoveCommandBehavior<StylusEventArgs>
    {
        public ChartAreaStylusMoveCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaPreviewStylusInAirMove
    // ChartAreaPreviewStylusInAirMoveCommand<T, TBehavior>
    public class ChartAreaPreviewStylusInAirMoveCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaPreviewStylusInAirMoveCommandBehavior<T>, new()
    { }

    // ChartAreaPreviewStylusInAirMoveCommandBehavior<TReturn>
    public class ChartAreaPreviewStylusInAirMoveCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public ChartAreaPreviewStylusInAirMoveCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaPreviewStylusInAirMoveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewStylusInAirMove += OnEventRaised;
        }
    }

    // ChartAreaPreviewStylusInAirMoveCommand
    public class ChartAreaPreviewStylusInAirMoveCommand : ChartAreaCommandBase<ChartAreaPreviewStylusInAirMoveCommandBehavior>
    { }

    // ChartAreaPreviewStylusInAirMoveCommandBehavior
    public class ChartAreaPreviewStylusInAirMoveCommandBehavior : ChartAreaPreviewStylusInAirMoveCommandBehavior<object>
    { }

    // ChartAreaPreviewStylusInAirMoveCommandWithEventArgs	
    public class ChartAreaPreviewStylusInAirMoveCommandWithEventArgs : ChartAreaPreviewStylusInAirMoveCommand<StylusEventArgs, ChartAreaPreviewStylusInAirMoveCommandBehaviorWithEventArgs>
    { }

    // ChartAreaPreviewStylusInAirMoveCommandBehaviorWithEventArgs
    public class ChartAreaPreviewStylusInAirMoveCommandBehaviorWithEventArgs : ChartAreaPreviewStylusInAirMoveCommandBehavior<StylusEventArgs>
    {
        public ChartAreaPreviewStylusInAirMoveCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaStylusInAirMove
    // ChartAreaStylusInAirMoveCommand<T, TBehavior>
    public class ChartAreaStylusInAirMoveCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaStylusInAirMoveCommandBehavior<T>, new()
    { }

    // ChartAreaStylusInAirMoveCommandBehavior<TReturn>
    public class ChartAreaStylusInAirMoveCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public ChartAreaStylusInAirMoveCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaStylusInAirMoveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusInAirMove += OnEventRaised;
        }
    }

    // ChartAreaStylusInAirMoveCommand
    public class ChartAreaStylusInAirMoveCommand : ChartAreaCommandBase<ChartAreaStylusInAirMoveCommandBehavior>
    { }

    // ChartAreaStylusInAirMoveCommandBehavior
    public class ChartAreaStylusInAirMoveCommandBehavior : ChartAreaStylusInAirMoveCommandBehavior<object>
    { }

    // ChartAreaStylusInAirMoveCommandWithEventArgs	
    public class ChartAreaStylusInAirMoveCommandWithEventArgs : ChartAreaStylusInAirMoveCommand<StylusEventArgs, ChartAreaStylusInAirMoveCommandBehaviorWithEventArgs>
    { }

    // ChartAreaStylusInAirMoveCommandBehaviorWithEventArgs
    public class ChartAreaStylusInAirMoveCommandBehaviorWithEventArgs : ChartAreaStylusInAirMoveCommandBehavior<StylusEventArgs>
    {
        public ChartAreaStylusInAirMoveCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaStylusEnter
    // ChartAreaStylusEnterCommand<T, TBehavior>
    public class ChartAreaStylusEnterCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaStylusEnterCommandBehavior<T>, new()
    { }

    // ChartAreaStylusEnterCommandBehavior<TReturn>
    public class ChartAreaStylusEnterCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public ChartAreaStylusEnterCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaStylusEnterCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusEnter += OnEventRaised;
        }
    }

    // ChartAreaStylusEnterCommand
    public class ChartAreaStylusEnterCommand : ChartAreaCommandBase<ChartAreaStylusEnterCommandBehavior>
    { }

    // ChartAreaStylusEnterCommandBehavior
    public class ChartAreaStylusEnterCommandBehavior : ChartAreaStylusEnterCommandBehavior<object>
    { }

    // ChartAreaStylusEnterCommandWithEventArgs	
    public class ChartAreaStylusEnterCommandWithEventArgs : ChartAreaStylusEnterCommand<StylusEventArgs, ChartAreaStylusEnterCommandBehaviorWithEventArgs>
    { }

    // ChartAreaStylusEnterCommandBehaviorWithEventArgs
    public class ChartAreaStylusEnterCommandBehaviorWithEventArgs : ChartAreaStylusEnterCommandBehavior<StylusEventArgs>
    {
        public ChartAreaStylusEnterCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaStylusLeave
    // ChartAreaStylusLeaveCommand<T, TBehavior>
    public class ChartAreaStylusLeaveCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaStylusLeaveCommandBehavior<T>, new()
    { }

    // ChartAreaStylusLeaveCommandBehavior<TReturn>
    public class ChartAreaStylusLeaveCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public ChartAreaStylusLeaveCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaStylusLeaveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusLeave += OnEventRaised;
        }
    }

    // ChartAreaStylusLeaveCommand
    public class ChartAreaStylusLeaveCommand : ChartAreaCommandBase<ChartAreaStylusLeaveCommandBehavior>
    { }

    // ChartAreaStylusLeaveCommandBehavior
    public class ChartAreaStylusLeaveCommandBehavior : ChartAreaStylusLeaveCommandBehavior<object>
    { }

    // ChartAreaStylusLeaveCommandWithEventArgs	
    public class ChartAreaStylusLeaveCommandWithEventArgs : ChartAreaStylusLeaveCommand<StylusEventArgs, ChartAreaStylusLeaveCommandBehaviorWithEventArgs>
    { }

    // ChartAreaStylusLeaveCommandBehaviorWithEventArgs
    public class ChartAreaStylusLeaveCommandBehaviorWithEventArgs : ChartAreaStylusLeaveCommandBehavior<StylusEventArgs>
    {
        public ChartAreaStylusLeaveCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaPreviewStylusInRange
    // ChartAreaPreviewStylusInRangeCommand<T, TBehavior>
    public class ChartAreaPreviewStylusInRangeCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaPreviewStylusInRangeCommandBehavior<T>, new()
    { }

    // ChartAreaPreviewStylusInRangeCommandBehavior<TReturn>
    public class ChartAreaPreviewStylusInRangeCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public ChartAreaPreviewStylusInRangeCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaPreviewStylusInRangeCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewStylusInRange += OnEventRaised;
        }
    }

    // ChartAreaPreviewStylusInRangeCommand
    public class ChartAreaPreviewStylusInRangeCommand : ChartAreaCommandBase<ChartAreaPreviewStylusInRangeCommandBehavior>
    { }

    // ChartAreaPreviewStylusInRangeCommandBehavior
    public class ChartAreaPreviewStylusInRangeCommandBehavior : ChartAreaPreviewStylusInRangeCommandBehavior<object>
    { }

    // ChartAreaPreviewStylusInRangeCommandWithEventArgs	
    public class ChartAreaPreviewStylusInRangeCommandWithEventArgs : ChartAreaPreviewStylusInRangeCommand<StylusEventArgs, ChartAreaPreviewStylusInRangeCommandBehaviorWithEventArgs>
    { }

    // ChartAreaPreviewStylusInRangeCommandBehaviorWithEventArgs
    public class ChartAreaPreviewStylusInRangeCommandBehaviorWithEventArgs : ChartAreaPreviewStylusInRangeCommandBehavior<StylusEventArgs>
    {
        public ChartAreaPreviewStylusInRangeCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaStylusInRange
    // ChartAreaStylusInRangeCommand<T, TBehavior>
    public class ChartAreaStylusInRangeCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaStylusInRangeCommandBehavior<T>, new()
    { }

    // ChartAreaStylusInRangeCommandBehavior<TReturn>
    public class ChartAreaStylusInRangeCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public ChartAreaStylusInRangeCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaStylusInRangeCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusInRange += OnEventRaised;
        }
    }

    // ChartAreaStylusInRangeCommand
    public class ChartAreaStylusInRangeCommand : ChartAreaCommandBase<ChartAreaStylusInRangeCommandBehavior>
    { }

    // ChartAreaStylusInRangeCommandBehavior
    public class ChartAreaStylusInRangeCommandBehavior : ChartAreaStylusInRangeCommandBehavior<object>
    { }

    // ChartAreaStylusInRangeCommandWithEventArgs	
    public class ChartAreaStylusInRangeCommandWithEventArgs : ChartAreaStylusInRangeCommand<StylusEventArgs, ChartAreaStylusInRangeCommandBehaviorWithEventArgs>
    { }

    // ChartAreaStylusInRangeCommandBehaviorWithEventArgs
    public class ChartAreaStylusInRangeCommandBehaviorWithEventArgs : ChartAreaStylusInRangeCommandBehavior<StylusEventArgs>
    {
        public ChartAreaStylusInRangeCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaPreviewStylusOutOfRange
    // ChartAreaPreviewStylusOutOfRangeCommand<T, TBehavior>
    public class ChartAreaPreviewStylusOutOfRangeCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaPreviewStylusOutOfRangeCommandBehavior<T>, new()
    { }

    // ChartAreaPreviewStylusOutOfRangeCommandBehavior<TReturn>
    public class ChartAreaPreviewStylusOutOfRangeCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public ChartAreaPreviewStylusOutOfRangeCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaPreviewStylusOutOfRangeCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewStylusOutOfRange += OnEventRaised;
        }
    }

    // ChartAreaPreviewStylusOutOfRangeCommand
    public class ChartAreaPreviewStylusOutOfRangeCommand : ChartAreaCommandBase<ChartAreaPreviewStylusOutOfRangeCommandBehavior>
    { }

    // ChartAreaPreviewStylusOutOfRangeCommandBehavior
    public class ChartAreaPreviewStylusOutOfRangeCommandBehavior : ChartAreaPreviewStylusOutOfRangeCommandBehavior<object>
    { }

    // ChartAreaPreviewStylusOutOfRangeCommandWithEventArgs	
    public class ChartAreaPreviewStylusOutOfRangeCommandWithEventArgs : ChartAreaPreviewStylusOutOfRangeCommand<StylusEventArgs, ChartAreaPreviewStylusOutOfRangeCommandBehaviorWithEventArgs>
    { }

    // ChartAreaPreviewStylusOutOfRangeCommandBehaviorWithEventArgs
    public class ChartAreaPreviewStylusOutOfRangeCommandBehaviorWithEventArgs : ChartAreaPreviewStylusOutOfRangeCommandBehavior<StylusEventArgs>
    {
        public ChartAreaPreviewStylusOutOfRangeCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaStylusOutOfRange
    // ChartAreaStylusOutOfRangeCommand<T, TBehavior>
    public class ChartAreaStylusOutOfRangeCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaStylusOutOfRangeCommandBehavior<T>, new()
    { }

    // ChartAreaStylusOutOfRangeCommandBehavior<TReturn>
    public class ChartAreaStylusOutOfRangeCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public ChartAreaStylusOutOfRangeCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaStylusOutOfRangeCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusOutOfRange += OnEventRaised;
        }
    }

    // ChartAreaStylusOutOfRangeCommand
    public class ChartAreaStylusOutOfRangeCommand : ChartAreaCommandBase<ChartAreaStylusOutOfRangeCommandBehavior>
    { }

    // ChartAreaStylusOutOfRangeCommandBehavior
    public class ChartAreaStylusOutOfRangeCommandBehavior : ChartAreaStylusOutOfRangeCommandBehavior<object>
    { }

    // ChartAreaStylusOutOfRangeCommandWithEventArgs	
    public class ChartAreaStylusOutOfRangeCommandWithEventArgs : ChartAreaStylusOutOfRangeCommand<StylusEventArgs, ChartAreaStylusOutOfRangeCommandBehaviorWithEventArgs>
    { }

    // ChartAreaStylusOutOfRangeCommandBehaviorWithEventArgs
    public class ChartAreaStylusOutOfRangeCommandBehaviorWithEventArgs : ChartAreaStylusOutOfRangeCommandBehavior<StylusEventArgs>
    {
        public ChartAreaStylusOutOfRangeCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaPreviewStylusSystemGesture
    // ChartAreaPreviewStylusSystemGestureCommand<T, TBehavior>
    public class ChartAreaPreviewStylusSystemGestureCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaPreviewStylusSystemGestureCommandBehavior<T>, new()
    { }

    // ChartAreaPreviewStylusSystemGestureCommandBehavior<TReturn>
    public class ChartAreaPreviewStylusSystemGestureCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, StylusSystemGestureEventArgs>
    {
        public ChartAreaPreviewStylusSystemGestureCommandBehavior(Func<object, StylusSystemGestureEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaPreviewStylusSystemGestureCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewStylusSystemGesture += OnEventRaised;
        }
    }

    // ChartAreaPreviewStylusSystemGestureCommand
    public class ChartAreaPreviewStylusSystemGestureCommand : ChartAreaCommandBase<ChartAreaPreviewStylusSystemGestureCommandBehavior>
    { }

    // ChartAreaPreviewStylusSystemGestureCommandBehavior
    public class ChartAreaPreviewStylusSystemGestureCommandBehavior : ChartAreaPreviewStylusSystemGestureCommandBehavior<object>
    { }

    // ChartAreaPreviewStylusSystemGestureCommandWithEventArgs	
    public class ChartAreaPreviewStylusSystemGestureCommandWithEventArgs : ChartAreaPreviewStylusSystemGestureCommand<StylusSystemGestureEventArgs, ChartAreaPreviewStylusSystemGestureCommandBehaviorWithEventArgs>
    { }

    // ChartAreaPreviewStylusSystemGestureCommandBehaviorWithEventArgs
    public class ChartAreaPreviewStylusSystemGestureCommandBehaviorWithEventArgs : ChartAreaPreviewStylusSystemGestureCommandBehavior<StylusSystemGestureEventArgs>
    {
        public ChartAreaPreviewStylusSystemGestureCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaStylusSystemGesture
    // ChartAreaStylusSystemGestureCommand<T, TBehavior>
    public class ChartAreaStylusSystemGestureCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaStylusSystemGestureCommandBehavior<T>, new()
    { }

    // ChartAreaStylusSystemGestureCommandBehavior<TReturn>
    public class ChartAreaStylusSystemGestureCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, StylusSystemGestureEventArgs>
    {
        public ChartAreaStylusSystemGestureCommandBehavior(Func<object, StylusSystemGestureEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaStylusSystemGestureCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusSystemGesture += OnEventRaised;
        }
    }

    // ChartAreaStylusSystemGestureCommand
    public class ChartAreaStylusSystemGestureCommand : ChartAreaCommandBase<ChartAreaStylusSystemGestureCommandBehavior>
    { }

    // ChartAreaStylusSystemGestureCommandBehavior
    public class ChartAreaStylusSystemGestureCommandBehavior : ChartAreaStylusSystemGestureCommandBehavior<object>
    { }

    // ChartAreaStylusSystemGestureCommandWithEventArgs	
    public class ChartAreaStylusSystemGestureCommandWithEventArgs : ChartAreaStylusSystemGestureCommand<StylusSystemGestureEventArgs, ChartAreaStylusSystemGestureCommandBehaviorWithEventArgs>
    { }

    // ChartAreaStylusSystemGestureCommandBehaviorWithEventArgs
    public class ChartAreaStylusSystemGestureCommandBehaviorWithEventArgs : ChartAreaStylusSystemGestureCommandBehavior<StylusSystemGestureEventArgs>
    {
        public ChartAreaStylusSystemGestureCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaGotStylusCapture
    // ChartAreaGotStylusCaptureCommand<T, TBehavior>
    public class ChartAreaGotStylusCaptureCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaGotStylusCaptureCommandBehavior<T>, new()
    { }

    // ChartAreaGotStylusCaptureCommandBehavior<TReturn>
    public class ChartAreaGotStylusCaptureCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public ChartAreaGotStylusCaptureCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaGotStylusCaptureCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.GotStylusCapture += OnEventRaised;
        }
    }

    // ChartAreaGotStylusCaptureCommand
    public class ChartAreaGotStylusCaptureCommand : ChartAreaCommandBase<ChartAreaGotStylusCaptureCommandBehavior>
    { }

    // ChartAreaGotStylusCaptureCommandBehavior
    public class ChartAreaGotStylusCaptureCommandBehavior : ChartAreaGotStylusCaptureCommandBehavior<object>
    { }

    // ChartAreaGotStylusCaptureCommandWithEventArgs	
    public class ChartAreaGotStylusCaptureCommandWithEventArgs : ChartAreaGotStylusCaptureCommand<StylusEventArgs, ChartAreaGotStylusCaptureCommandBehaviorWithEventArgs>
    { }

    // ChartAreaGotStylusCaptureCommandBehaviorWithEventArgs
    public class ChartAreaGotStylusCaptureCommandBehaviorWithEventArgs : ChartAreaGotStylusCaptureCommandBehavior<StylusEventArgs>
    {
        public ChartAreaGotStylusCaptureCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaLostStylusCapture
    // ChartAreaLostStylusCaptureCommand<T, TBehavior>
    public class ChartAreaLostStylusCaptureCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaLostStylusCaptureCommandBehavior<T>, new()
    { }

    // ChartAreaLostStylusCaptureCommandBehavior<TReturn>
    public class ChartAreaLostStylusCaptureCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public ChartAreaLostStylusCaptureCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaLostStylusCaptureCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.LostStylusCapture += OnEventRaised;
        }
    }

    // ChartAreaLostStylusCaptureCommand
    public class ChartAreaLostStylusCaptureCommand : ChartAreaCommandBase<ChartAreaLostStylusCaptureCommandBehavior>
    { }

    // ChartAreaLostStylusCaptureCommandBehavior
    public class ChartAreaLostStylusCaptureCommandBehavior : ChartAreaLostStylusCaptureCommandBehavior<object>
    { }

    // ChartAreaLostStylusCaptureCommandWithEventArgs	
    public class ChartAreaLostStylusCaptureCommandWithEventArgs : ChartAreaLostStylusCaptureCommand<StylusEventArgs, ChartAreaLostStylusCaptureCommandBehaviorWithEventArgs>
    { }

    // ChartAreaLostStylusCaptureCommandBehaviorWithEventArgs
    public class ChartAreaLostStylusCaptureCommandBehaviorWithEventArgs : ChartAreaLostStylusCaptureCommandBehavior<StylusEventArgs>
    {
        public ChartAreaLostStylusCaptureCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaStylusButtonDown
    // ChartAreaStylusButtonDownCommand<T, TBehavior>
    public class ChartAreaStylusButtonDownCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaStylusButtonDownCommandBehavior<T>, new()
    { }

    // ChartAreaStylusButtonDownCommandBehavior<TReturn>
    public class ChartAreaStylusButtonDownCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, StylusButtonEventArgs>
    {
        public ChartAreaStylusButtonDownCommandBehavior(Func<object, StylusButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaStylusButtonDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusButtonDown += OnEventRaised;
        }
    }

    // ChartAreaStylusButtonDownCommand
    public class ChartAreaStylusButtonDownCommand : ChartAreaCommandBase<ChartAreaStylusButtonDownCommandBehavior>
    { }

    // ChartAreaStylusButtonDownCommandBehavior
    public class ChartAreaStylusButtonDownCommandBehavior : ChartAreaStylusButtonDownCommandBehavior<object>
    { }

    // ChartAreaStylusButtonDownCommandWithEventArgs	
    public class ChartAreaStylusButtonDownCommandWithEventArgs : ChartAreaStylusButtonDownCommand<StylusButtonEventArgs, ChartAreaStylusButtonDownCommandBehaviorWithEventArgs>
    { }

    // ChartAreaStylusButtonDownCommandBehaviorWithEventArgs
    public class ChartAreaStylusButtonDownCommandBehaviorWithEventArgs : ChartAreaStylusButtonDownCommandBehavior<StylusButtonEventArgs>
    {
        public ChartAreaStylusButtonDownCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaStylusButtonUp
    // ChartAreaStylusButtonUpCommand<T, TBehavior>
    public class ChartAreaStylusButtonUpCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaStylusButtonUpCommandBehavior<T>, new()
    { }

    // ChartAreaStylusButtonUpCommandBehavior<TReturn>
    public class ChartAreaStylusButtonUpCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, StylusButtonEventArgs>
    {
        public ChartAreaStylusButtonUpCommandBehavior(Func<object, StylusButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaStylusButtonUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusButtonUp += OnEventRaised;
        }
    }

    // ChartAreaStylusButtonUpCommand
    public class ChartAreaStylusButtonUpCommand : ChartAreaCommandBase<ChartAreaStylusButtonUpCommandBehavior>
    { }

    // ChartAreaStylusButtonUpCommandBehavior
    public class ChartAreaStylusButtonUpCommandBehavior : ChartAreaStylusButtonUpCommandBehavior<object>
    { }

    // ChartAreaStylusButtonUpCommandWithEventArgs	
    public class ChartAreaStylusButtonUpCommandWithEventArgs : ChartAreaStylusButtonUpCommand<StylusButtonEventArgs, ChartAreaStylusButtonUpCommandBehaviorWithEventArgs>
    { }

    // ChartAreaStylusButtonUpCommandBehaviorWithEventArgs
    public class ChartAreaStylusButtonUpCommandBehaviorWithEventArgs : ChartAreaStylusButtonUpCommandBehavior<StylusButtonEventArgs>
    {
        public ChartAreaStylusButtonUpCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaPreviewStylusButtonDown
    // ChartAreaPreviewStylusButtonDownCommand<T, TBehavior>
    public class ChartAreaPreviewStylusButtonDownCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaPreviewStylusButtonDownCommandBehavior<T>, new()
    { }

    // ChartAreaPreviewStylusButtonDownCommandBehavior<TReturn>
    public class ChartAreaPreviewStylusButtonDownCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, StylusButtonEventArgs>
    {
        public ChartAreaPreviewStylusButtonDownCommandBehavior(Func<object, StylusButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaPreviewStylusButtonDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewStylusButtonDown += OnEventRaised;
        }
    }

    // ChartAreaPreviewStylusButtonDownCommand
    public class ChartAreaPreviewStylusButtonDownCommand : ChartAreaCommandBase<ChartAreaPreviewStylusButtonDownCommandBehavior>
    { }

    // ChartAreaPreviewStylusButtonDownCommandBehavior
    public class ChartAreaPreviewStylusButtonDownCommandBehavior : ChartAreaPreviewStylusButtonDownCommandBehavior<object>
    { }

    // ChartAreaPreviewStylusButtonDownCommandWithEventArgs	
    public class ChartAreaPreviewStylusButtonDownCommandWithEventArgs : ChartAreaPreviewStylusButtonDownCommand<StylusButtonEventArgs, ChartAreaPreviewStylusButtonDownCommandBehaviorWithEventArgs>
    { }

    // ChartAreaPreviewStylusButtonDownCommandBehaviorWithEventArgs
    public class ChartAreaPreviewStylusButtonDownCommandBehaviorWithEventArgs : ChartAreaPreviewStylusButtonDownCommandBehavior<StylusButtonEventArgs>
    {
        public ChartAreaPreviewStylusButtonDownCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaPreviewStylusButtonUp
    // ChartAreaPreviewStylusButtonUpCommand<T, TBehavior>
    public class ChartAreaPreviewStylusButtonUpCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaPreviewStylusButtonUpCommandBehavior<T>, new()
    { }

    // ChartAreaPreviewStylusButtonUpCommandBehavior<TReturn>
    public class ChartAreaPreviewStylusButtonUpCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, StylusButtonEventArgs>
    {
        public ChartAreaPreviewStylusButtonUpCommandBehavior(Func<object, StylusButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaPreviewStylusButtonUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewStylusButtonUp += OnEventRaised;
        }
    }

    // ChartAreaPreviewStylusButtonUpCommand
    public class ChartAreaPreviewStylusButtonUpCommand : ChartAreaCommandBase<ChartAreaPreviewStylusButtonUpCommandBehavior>
    { }

    // ChartAreaPreviewStylusButtonUpCommandBehavior
    public class ChartAreaPreviewStylusButtonUpCommandBehavior : ChartAreaPreviewStylusButtonUpCommandBehavior<object>
    { }

    // ChartAreaPreviewStylusButtonUpCommandWithEventArgs	
    public class ChartAreaPreviewStylusButtonUpCommandWithEventArgs : ChartAreaPreviewStylusButtonUpCommand<StylusButtonEventArgs, ChartAreaPreviewStylusButtonUpCommandBehaviorWithEventArgs>
    { }

    // ChartAreaPreviewStylusButtonUpCommandBehaviorWithEventArgs
    public class ChartAreaPreviewStylusButtonUpCommandBehaviorWithEventArgs : ChartAreaPreviewStylusButtonUpCommandBehavior<StylusButtonEventArgs>
    {
        public ChartAreaPreviewStylusButtonUpCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaPreviewKeyDown
    // ChartAreaPreviewKeyDownCommand<T, TBehavior>
    public class ChartAreaPreviewKeyDownCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaPreviewKeyDownCommandBehavior<T>, new()
    { }

    // ChartAreaPreviewKeyDownCommandBehavior<TReturn>
    public class ChartAreaPreviewKeyDownCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, KeyEventArgs>
    {
        public ChartAreaPreviewKeyDownCommandBehavior(Func<object, KeyEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaPreviewKeyDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewKeyDown += OnEventRaised;
        }
    }

    // ChartAreaPreviewKeyDownCommand
    public class ChartAreaPreviewKeyDownCommand : ChartAreaCommandBase<ChartAreaPreviewKeyDownCommandBehavior>
    { }

    // ChartAreaPreviewKeyDownCommandBehavior
    public class ChartAreaPreviewKeyDownCommandBehavior : ChartAreaPreviewKeyDownCommandBehavior<object>
    { }

    // ChartAreaPreviewKeyDownCommandWithEventArgs	
    public class ChartAreaPreviewKeyDownCommandWithEventArgs : ChartAreaPreviewKeyDownCommand<KeyEventArgs, ChartAreaPreviewKeyDownCommandBehaviorWithEventArgs>
    { }

    // ChartAreaPreviewKeyDownCommandBehaviorWithEventArgs
    public class ChartAreaPreviewKeyDownCommandBehaviorWithEventArgs : ChartAreaPreviewKeyDownCommandBehavior<KeyEventArgs>
    {
        public ChartAreaPreviewKeyDownCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaKeyDown
    // ChartAreaKeyDownCommand<T, TBehavior>
    public class ChartAreaKeyDownCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaKeyDownCommandBehavior<T>, new()
    { }

    // ChartAreaKeyDownCommandBehavior<TReturn>
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

    // ChartAreaKeyDownCommand
    public class ChartAreaKeyDownCommand : ChartAreaCommandBase<ChartAreaKeyDownCommandBehavior>
    { }

    // ChartAreaKeyDownCommandBehavior
    public class ChartAreaKeyDownCommandBehavior : ChartAreaKeyDownCommandBehavior<object>
    { }

    // ChartAreaKeyDownCommandWithEventArgs	
    public class ChartAreaKeyDownCommandWithEventArgs : ChartAreaKeyDownCommand<KeyEventArgs, ChartAreaKeyDownCommandBehaviorWithEventArgs>
    { }

    // ChartAreaKeyDownCommandBehaviorWithEventArgs
    public class ChartAreaKeyDownCommandBehaviorWithEventArgs : ChartAreaKeyDownCommandBehavior<KeyEventArgs>
    {
        public ChartAreaKeyDownCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaPreviewKeyUp
    // ChartAreaPreviewKeyUpCommand<T, TBehavior>
    public class ChartAreaPreviewKeyUpCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaPreviewKeyUpCommandBehavior<T>, new()
    { }

    // ChartAreaPreviewKeyUpCommandBehavior<TReturn>
    public class ChartAreaPreviewKeyUpCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, KeyEventArgs>
    {
        public ChartAreaPreviewKeyUpCommandBehavior(Func<object, KeyEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaPreviewKeyUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewKeyUp += OnEventRaised;
        }
    }

    // ChartAreaPreviewKeyUpCommand
    public class ChartAreaPreviewKeyUpCommand : ChartAreaCommandBase<ChartAreaPreviewKeyUpCommandBehavior>
    { }

    // ChartAreaPreviewKeyUpCommandBehavior
    public class ChartAreaPreviewKeyUpCommandBehavior : ChartAreaPreviewKeyUpCommandBehavior<object>
    { }

    // ChartAreaPreviewKeyUpCommandWithEventArgs	
    public class ChartAreaPreviewKeyUpCommandWithEventArgs : ChartAreaPreviewKeyUpCommand<KeyEventArgs, ChartAreaPreviewKeyUpCommandBehaviorWithEventArgs>
    { }

    // ChartAreaPreviewKeyUpCommandBehaviorWithEventArgs
    public class ChartAreaPreviewKeyUpCommandBehaviorWithEventArgs : ChartAreaPreviewKeyUpCommandBehavior<KeyEventArgs>
    {
        public ChartAreaPreviewKeyUpCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaKeyUp
    // ChartAreaKeyUpCommand<T, TBehavior>
    public class ChartAreaKeyUpCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaKeyUpCommandBehavior<T>, new()
    { }

    // ChartAreaKeyUpCommandBehavior<TReturn>
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

    // ChartAreaKeyUpCommand
    public class ChartAreaKeyUpCommand : ChartAreaCommandBase<ChartAreaKeyUpCommandBehavior>
    { }

    // ChartAreaKeyUpCommandBehavior
    public class ChartAreaKeyUpCommandBehavior : ChartAreaKeyUpCommandBehavior<object>
    { }

    // ChartAreaKeyUpCommandWithEventArgs	
    public class ChartAreaKeyUpCommandWithEventArgs : ChartAreaKeyUpCommand<KeyEventArgs, ChartAreaKeyUpCommandBehaviorWithEventArgs>
    { }

    // ChartAreaKeyUpCommandBehaviorWithEventArgs
    public class ChartAreaKeyUpCommandBehaviorWithEventArgs : ChartAreaKeyUpCommandBehavior<KeyEventArgs>
    {
        public ChartAreaKeyUpCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaPreviewGotKeyboardFocus
    // ChartAreaPreviewGotKeyboardFocusCommand<T, TBehavior>
    public class ChartAreaPreviewGotKeyboardFocusCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaPreviewGotKeyboardFocusCommandBehavior<T>, new()
    { }

    // ChartAreaPreviewGotKeyboardFocusCommandBehavior<TReturn>
    public class ChartAreaPreviewGotKeyboardFocusCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, KeyboardFocusChangedEventArgs>
    {
        public ChartAreaPreviewGotKeyboardFocusCommandBehavior(Func<object, KeyboardFocusChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaPreviewGotKeyboardFocusCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewGotKeyboardFocus += OnEventRaised;
        }
    }

    // ChartAreaPreviewGotKeyboardFocusCommand
    public class ChartAreaPreviewGotKeyboardFocusCommand : ChartAreaCommandBase<ChartAreaPreviewGotKeyboardFocusCommandBehavior>
    { }

    // ChartAreaPreviewGotKeyboardFocusCommandBehavior
    public class ChartAreaPreviewGotKeyboardFocusCommandBehavior : ChartAreaPreviewGotKeyboardFocusCommandBehavior<object>
    { }

    // ChartAreaPreviewGotKeyboardFocusCommandWithEventArgs	
    public class ChartAreaPreviewGotKeyboardFocusCommandWithEventArgs : ChartAreaPreviewGotKeyboardFocusCommand<KeyboardFocusChangedEventArgs, ChartAreaPreviewGotKeyboardFocusCommandBehaviorWithEventArgs>
    { }

    // ChartAreaPreviewGotKeyboardFocusCommandBehaviorWithEventArgs
    public class ChartAreaPreviewGotKeyboardFocusCommandBehaviorWithEventArgs : ChartAreaPreviewGotKeyboardFocusCommandBehavior<KeyboardFocusChangedEventArgs>
    {
        public ChartAreaPreviewGotKeyboardFocusCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaGotKeyboardFocus
    // ChartAreaGotKeyboardFocusCommand<T, TBehavior>
    public class ChartAreaGotKeyboardFocusCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaGotKeyboardFocusCommandBehavior<T>, new()
    { }

    // ChartAreaGotKeyboardFocusCommandBehavior<TReturn>
    public class ChartAreaGotKeyboardFocusCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, KeyboardFocusChangedEventArgs>
    {
        public ChartAreaGotKeyboardFocusCommandBehavior(Func<object, KeyboardFocusChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaGotKeyboardFocusCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.GotKeyboardFocus += OnEventRaised;
        }
    }

    // ChartAreaGotKeyboardFocusCommand
    public class ChartAreaGotKeyboardFocusCommand : ChartAreaCommandBase<ChartAreaGotKeyboardFocusCommandBehavior>
    { }

    // ChartAreaGotKeyboardFocusCommandBehavior
    public class ChartAreaGotKeyboardFocusCommandBehavior : ChartAreaGotKeyboardFocusCommandBehavior<object>
    { }

    // ChartAreaGotKeyboardFocusCommandWithEventArgs	
    public class ChartAreaGotKeyboardFocusCommandWithEventArgs : ChartAreaGotKeyboardFocusCommand<KeyboardFocusChangedEventArgs, ChartAreaGotKeyboardFocusCommandBehaviorWithEventArgs>
    { }

    // ChartAreaGotKeyboardFocusCommandBehaviorWithEventArgs
    public class ChartAreaGotKeyboardFocusCommandBehaviorWithEventArgs : ChartAreaGotKeyboardFocusCommandBehavior<KeyboardFocusChangedEventArgs>
    {
        public ChartAreaGotKeyboardFocusCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaPreviewLostKeyboardFocus
    // ChartAreaPreviewLostKeyboardFocusCommand<T, TBehavior>
    public class ChartAreaPreviewLostKeyboardFocusCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaPreviewLostKeyboardFocusCommandBehavior<T>, new()
    { }

    // ChartAreaPreviewLostKeyboardFocusCommandBehavior<TReturn>
    public class ChartAreaPreviewLostKeyboardFocusCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, KeyboardFocusChangedEventArgs>
    {
        public ChartAreaPreviewLostKeyboardFocusCommandBehavior(Func<object, KeyboardFocusChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaPreviewLostKeyboardFocusCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewLostKeyboardFocus += OnEventRaised;
        }
    }

    // ChartAreaPreviewLostKeyboardFocusCommand
    public class ChartAreaPreviewLostKeyboardFocusCommand : ChartAreaCommandBase<ChartAreaPreviewLostKeyboardFocusCommandBehavior>
    { }

    // ChartAreaPreviewLostKeyboardFocusCommandBehavior
    public class ChartAreaPreviewLostKeyboardFocusCommandBehavior : ChartAreaPreviewLostKeyboardFocusCommandBehavior<object>
    { }

    // ChartAreaPreviewLostKeyboardFocusCommandWithEventArgs	
    public class ChartAreaPreviewLostKeyboardFocusCommandWithEventArgs : ChartAreaPreviewLostKeyboardFocusCommand<KeyboardFocusChangedEventArgs, ChartAreaPreviewLostKeyboardFocusCommandBehaviorWithEventArgs>
    { }

    // ChartAreaPreviewLostKeyboardFocusCommandBehaviorWithEventArgs
    public class ChartAreaPreviewLostKeyboardFocusCommandBehaviorWithEventArgs : ChartAreaPreviewLostKeyboardFocusCommandBehavior<KeyboardFocusChangedEventArgs>
    {
        public ChartAreaPreviewLostKeyboardFocusCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaLostKeyboardFocus
    // ChartAreaLostKeyboardFocusCommand<T, TBehavior>
    public class ChartAreaLostKeyboardFocusCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaLostKeyboardFocusCommandBehavior<T>, new()
    { }

    // ChartAreaLostKeyboardFocusCommandBehavior<TReturn>
    public class ChartAreaLostKeyboardFocusCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, KeyboardFocusChangedEventArgs>
    {
        public ChartAreaLostKeyboardFocusCommandBehavior(Func<object, KeyboardFocusChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaLostKeyboardFocusCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.LostKeyboardFocus += OnEventRaised;
        }
    }

    // ChartAreaLostKeyboardFocusCommand
    public class ChartAreaLostKeyboardFocusCommand : ChartAreaCommandBase<ChartAreaLostKeyboardFocusCommandBehavior>
    { }

    // ChartAreaLostKeyboardFocusCommandBehavior
    public class ChartAreaLostKeyboardFocusCommandBehavior : ChartAreaLostKeyboardFocusCommandBehavior<object>
    { }

    // ChartAreaLostKeyboardFocusCommandWithEventArgs	
    public class ChartAreaLostKeyboardFocusCommandWithEventArgs : ChartAreaLostKeyboardFocusCommand<KeyboardFocusChangedEventArgs, ChartAreaLostKeyboardFocusCommandBehaviorWithEventArgs>
    { }

    // ChartAreaLostKeyboardFocusCommandBehaviorWithEventArgs
    public class ChartAreaLostKeyboardFocusCommandBehaviorWithEventArgs : ChartAreaLostKeyboardFocusCommandBehavior<KeyboardFocusChangedEventArgs>
    {
        public ChartAreaLostKeyboardFocusCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaPreviewTextInput
    // ChartAreaPreviewTextInputCommand<T, TBehavior>
    public class ChartAreaPreviewTextInputCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaPreviewTextInputCommandBehavior<T>, new()
    { }

    // ChartAreaPreviewTextInputCommandBehavior<TReturn>
    public class ChartAreaPreviewTextInputCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, TextCompositionEventArgs>
    {
        public ChartAreaPreviewTextInputCommandBehavior(Func<object, TextCompositionEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaPreviewTextInputCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewTextInput += OnEventRaised;
        }
    }

    // ChartAreaPreviewTextInputCommand
    public class ChartAreaPreviewTextInputCommand : ChartAreaCommandBase<ChartAreaPreviewTextInputCommandBehavior>
    { }

    // ChartAreaPreviewTextInputCommandBehavior
    public class ChartAreaPreviewTextInputCommandBehavior : ChartAreaPreviewTextInputCommandBehavior<object>
    { }

    // ChartAreaPreviewTextInputCommandWithEventArgs	
    public class ChartAreaPreviewTextInputCommandWithEventArgs : ChartAreaPreviewTextInputCommand<TextCompositionEventArgs, ChartAreaPreviewTextInputCommandBehaviorWithEventArgs>
    { }

    // ChartAreaPreviewTextInputCommandBehaviorWithEventArgs
    public class ChartAreaPreviewTextInputCommandBehaviorWithEventArgs : ChartAreaPreviewTextInputCommandBehavior<TextCompositionEventArgs>
    {
        public ChartAreaPreviewTextInputCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaTextInput
    // ChartAreaTextInputCommand<T, TBehavior>
    public class ChartAreaTextInputCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaTextInputCommandBehavior<T>, new()
    { }

    // ChartAreaTextInputCommandBehavior<TReturn>
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

    // ChartAreaTextInputCommand
    public class ChartAreaTextInputCommand : ChartAreaCommandBase<ChartAreaTextInputCommandBehavior>
    { }

    // ChartAreaTextInputCommandBehavior
    public class ChartAreaTextInputCommandBehavior : ChartAreaTextInputCommandBehavior<object>
    { }

    // ChartAreaTextInputCommandWithEventArgs	
    public class ChartAreaTextInputCommandWithEventArgs : ChartAreaTextInputCommand<TextCompositionEventArgs, ChartAreaTextInputCommandBehaviorWithEventArgs>
    { }

    // ChartAreaTextInputCommandBehaviorWithEventArgs
    public class ChartAreaTextInputCommandBehaviorWithEventArgs : ChartAreaTextInputCommandBehavior<TextCompositionEventArgs>
    {
        public ChartAreaTextInputCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaPreviewQueryContinueDrag
    // ChartAreaPreviewQueryContinueDragCommand<T, TBehavior>
    public class ChartAreaPreviewQueryContinueDragCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaPreviewQueryContinueDragCommandBehavior<T>, new()
    { }

    // ChartAreaPreviewQueryContinueDragCommandBehavior<TReturn>
    public class ChartAreaPreviewQueryContinueDragCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, QueryContinueDragEventArgs>
    {
        public ChartAreaPreviewQueryContinueDragCommandBehavior(Func<object, QueryContinueDragEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaPreviewQueryContinueDragCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewQueryContinueDrag += OnEventRaised;
        }
    }

    // ChartAreaPreviewQueryContinueDragCommand
    public class ChartAreaPreviewQueryContinueDragCommand : ChartAreaCommandBase<ChartAreaPreviewQueryContinueDragCommandBehavior>
    { }

    // ChartAreaPreviewQueryContinueDragCommandBehavior
    public class ChartAreaPreviewQueryContinueDragCommandBehavior : ChartAreaPreviewQueryContinueDragCommandBehavior<object>
    { }

    // ChartAreaPreviewQueryContinueDragCommandWithEventArgs	
    public class ChartAreaPreviewQueryContinueDragCommandWithEventArgs : ChartAreaPreviewQueryContinueDragCommand<QueryContinueDragEventArgs, ChartAreaPreviewQueryContinueDragCommandBehaviorWithEventArgs>
    { }

    // ChartAreaPreviewQueryContinueDragCommandBehaviorWithEventArgs
    public class ChartAreaPreviewQueryContinueDragCommandBehaviorWithEventArgs : ChartAreaPreviewQueryContinueDragCommandBehavior<QueryContinueDragEventArgs>
    {
        public ChartAreaPreviewQueryContinueDragCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaQueryContinueDrag
    // ChartAreaQueryContinueDragCommand<T, TBehavior>
    public class ChartAreaQueryContinueDragCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaQueryContinueDragCommandBehavior<T>, new()
    { }

    // ChartAreaQueryContinueDragCommandBehavior<TReturn>
    public class ChartAreaQueryContinueDragCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, QueryContinueDragEventArgs>
    {
        public ChartAreaQueryContinueDragCommandBehavior(Func<object, QueryContinueDragEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaQueryContinueDragCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.QueryContinueDrag += OnEventRaised;
        }
    }

    // ChartAreaQueryContinueDragCommand
    public class ChartAreaQueryContinueDragCommand : ChartAreaCommandBase<ChartAreaQueryContinueDragCommandBehavior>
    { }

    // ChartAreaQueryContinueDragCommandBehavior
    public class ChartAreaQueryContinueDragCommandBehavior : ChartAreaQueryContinueDragCommandBehavior<object>
    { }

    // ChartAreaQueryContinueDragCommandWithEventArgs	
    public class ChartAreaQueryContinueDragCommandWithEventArgs : ChartAreaQueryContinueDragCommand<QueryContinueDragEventArgs, ChartAreaQueryContinueDragCommandBehaviorWithEventArgs>
    { }

    // ChartAreaQueryContinueDragCommandBehaviorWithEventArgs
    public class ChartAreaQueryContinueDragCommandBehaviorWithEventArgs : ChartAreaQueryContinueDragCommandBehavior<QueryContinueDragEventArgs>
    {
        public ChartAreaQueryContinueDragCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaPreviewGiveFeedback
    // ChartAreaPreviewGiveFeedbackCommand<T, TBehavior>
    public class ChartAreaPreviewGiveFeedbackCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaPreviewGiveFeedbackCommandBehavior<T>, new()
    { }

    // ChartAreaPreviewGiveFeedbackCommandBehavior<TReturn>
    public class ChartAreaPreviewGiveFeedbackCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, GiveFeedbackEventArgs>
    {
        public ChartAreaPreviewGiveFeedbackCommandBehavior(Func<object, GiveFeedbackEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaPreviewGiveFeedbackCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewGiveFeedback += OnEventRaised;
        }
    }

    // ChartAreaPreviewGiveFeedbackCommand
    public class ChartAreaPreviewGiveFeedbackCommand : ChartAreaCommandBase<ChartAreaPreviewGiveFeedbackCommandBehavior>
    { }

    // ChartAreaPreviewGiveFeedbackCommandBehavior
    public class ChartAreaPreviewGiveFeedbackCommandBehavior : ChartAreaPreviewGiveFeedbackCommandBehavior<object>
    { }

    // ChartAreaPreviewGiveFeedbackCommandWithEventArgs	
    public class ChartAreaPreviewGiveFeedbackCommandWithEventArgs : ChartAreaPreviewGiveFeedbackCommand<GiveFeedbackEventArgs, ChartAreaPreviewGiveFeedbackCommandBehaviorWithEventArgs>
    { }

    // ChartAreaPreviewGiveFeedbackCommandBehaviorWithEventArgs
    public class ChartAreaPreviewGiveFeedbackCommandBehaviorWithEventArgs : ChartAreaPreviewGiveFeedbackCommandBehavior<GiveFeedbackEventArgs>
    {
        public ChartAreaPreviewGiveFeedbackCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaGiveFeedback
    // ChartAreaGiveFeedbackCommand<T, TBehavior>
    public class ChartAreaGiveFeedbackCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaGiveFeedbackCommandBehavior<T>, new()
    { }

    // ChartAreaGiveFeedbackCommandBehavior<TReturn>
    public class ChartAreaGiveFeedbackCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, GiveFeedbackEventArgs>
    {
        public ChartAreaGiveFeedbackCommandBehavior(Func<object, GiveFeedbackEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaGiveFeedbackCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.GiveFeedback += OnEventRaised;
        }
    }

    // ChartAreaGiveFeedbackCommand
    public class ChartAreaGiveFeedbackCommand : ChartAreaCommandBase<ChartAreaGiveFeedbackCommandBehavior>
    { }

    // ChartAreaGiveFeedbackCommandBehavior
    public class ChartAreaGiveFeedbackCommandBehavior : ChartAreaGiveFeedbackCommandBehavior<object>
    { }

    // ChartAreaGiveFeedbackCommandWithEventArgs	
    public class ChartAreaGiveFeedbackCommandWithEventArgs : ChartAreaGiveFeedbackCommand<GiveFeedbackEventArgs, ChartAreaGiveFeedbackCommandBehaviorWithEventArgs>
    { }

    // ChartAreaGiveFeedbackCommandBehaviorWithEventArgs
    public class ChartAreaGiveFeedbackCommandBehaviorWithEventArgs : ChartAreaGiveFeedbackCommandBehavior<GiveFeedbackEventArgs>
    {
        public ChartAreaGiveFeedbackCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaPreviewDragEnter
    // ChartAreaPreviewDragEnterCommand<T, TBehavior>
    public class ChartAreaPreviewDragEnterCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaPreviewDragEnterCommandBehavior<T>, new()
    { }

    // ChartAreaPreviewDragEnterCommandBehavior<TReturn>
    public class ChartAreaPreviewDragEnterCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, DragEventArgs>
    {
        public ChartAreaPreviewDragEnterCommandBehavior(Func<object, DragEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaPreviewDragEnterCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewDragEnter += OnEventRaised;
        }
    }

    // ChartAreaPreviewDragEnterCommand
    public class ChartAreaPreviewDragEnterCommand : ChartAreaCommandBase<ChartAreaPreviewDragEnterCommandBehavior>
    { }

    // ChartAreaPreviewDragEnterCommandBehavior
    public class ChartAreaPreviewDragEnterCommandBehavior : ChartAreaPreviewDragEnterCommandBehavior<object>
    { }

    // ChartAreaPreviewDragEnterCommandWithEventArgs	
    public class ChartAreaPreviewDragEnterCommandWithEventArgs : ChartAreaPreviewDragEnterCommand<DragEventArgs, ChartAreaPreviewDragEnterCommandBehaviorWithEventArgs>
    { }

    // ChartAreaPreviewDragEnterCommandBehaviorWithEventArgs
    public class ChartAreaPreviewDragEnterCommandBehaviorWithEventArgs : ChartAreaPreviewDragEnterCommandBehavior<DragEventArgs>
    {
        public ChartAreaPreviewDragEnterCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaDragEnter
    // ChartAreaDragEnterCommand<T, TBehavior>
    public class ChartAreaDragEnterCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaDragEnterCommandBehavior<T>, new()
    { }

    // ChartAreaDragEnterCommandBehavior<TReturn>
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

    // ChartAreaDragEnterCommand
    public class ChartAreaDragEnterCommand : ChartAreaCommandBase<ChartAreaDragEnterCommandBehavior>
    { }

    // ChartAreaDragEnterCommandBehavior
    public class ChartAreaDragEnterCommandBehavior : ChartAreaDragEnterCommandBehavior<object>
    { }

    // ChartAreaDragEnterCommandWithEventArgs	
    public class ChartAreaDragEnterCommandWithEventArgs : ChartAreaDragEnterCommand<DragEventArgs, ChartAreaDragEnterCommandBehaviorWithEventArgs>
    { }

    // ChartAreaDragEnterCommandBehaviorWithEventArgs
    public class ChartAreaDragEnterCommandBehaviorWithEventArgs : ChartAreaDragEnterCommandBehavior<DragEventArgs>
    {
        public ChartAreaDragEnterCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaPreviewDragOver
    // ChartAreaPreviewDragOverCommand<T, TBehavior>
    public class ChartAreaPreviewDragOverCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaPreviewDragOverCommandBehavior<T>, new()
    { }

    // ChartAreaPreviewDragOverCommandBehavior<TReturn>
    public class ChartAreaPreviewDragOverCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, DragEventArgs>
    {
        public ChartAreaPreviewDragOverCommandBehavior(Func<object, DragEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaPreviewDragOverCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewDragOver += OnEventRaised;
        }
    }

    // ChartAreaPreviewDragOverCommand
    public class ChartAreaPreviewDragOverCommand : ChartAreaCommandBase<ChartAreaPreviewDragOverCommandBehavior>
    { }

    // ChartAreaPreviewDragOverCommandBehavior
    public class ChartAreaPreviewDragOverCommandBehavior : ChartAreaPreviewDragOverCommandBehavior<object>
    { }

    // ChartAreaPreviewDragOverCommandWithEventArgs	
    public class ChartAreaPreviewDragOverCommandWithEventArgs : ChartAreaPreviewDragOverCommand<DragEventArgs, ChartAreaPreviewDragOverCommandBehaviorWithEventArgs>
    { }

    // ChartAreaPreviewDragOverCommandBehaviorWithEventArgs
    public class ChartAreaPreviewDragOverCommandBehaviorWithEventArgs : ChartAreaPreviewDragOverCommandBehavior<DragEventArgs>
    {
        public ChartAreaPreviewDragOverCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaDragOver
    // ChartAreaDragOverCommand<T, TBehavior>
    public class ChartAreaDragOverCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaDragOverCommandBehavior<T>, new()
    { }

    // ChartAreaDragOverCommandBehavior<TReturn>
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

    // ChartAreaDragOverCommand
    public class ChartAreaDragOverCommand : ChartAreaCommandBase<ChartAreaDragOverCommandBehavior>
    { }

    // ChartAreaDragOverCommandBehavior
    public class ChartAreaDragOverCommandBehavior : ChartAreaDragOverCommandBehavior<object>
    { }

    // ChartAreaDragOverCommandWithEventArgs	
    public class ChartAreaDragOverCommandWithEventArgs : ChartAreaDragOverCommand<DragEventArgs, ChartAreaDragOverCommandBehaviorWithEventArgs>
    { }

    // ChartAreaDragOverCommandBehaviorWithEventArgs
    public class ChartAreaDragOverCommandBehaviorWithEventArgs : ChartAreaDragOverCommandBehavior<DragEventArgs>
    {
        public ChartAreaDragOverCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaPreviewDragLeave
    // ChartAreaPreviewDragLeaveCommand<T, TBehavior>
    public class ChartAreaPreviewDragLeaveCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaPreviewDragLeaveCommandBehavior<T>, new()
    { }

    // ChartAreaPreviewDragLeaveCommandBehavior<TReturn>
    public class ChartAreaPreviewDragLeaveCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, DragEventArgs>
    {
        public ChartAreaPreviewDragLeaveCommandBehavior(Func<object, DragEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaPreviewDragLeaveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewDragLeave += OnEventRaised;
        }
    }

    // ChartAreaPreviewDragLeaveCommand
    public class ChartAreaPreviewDragLeaveCommand : ChartAreaCommandBase<ChartAreaPreviewDragLeaveCommandBehavior>
    { }

    // ChartAreaPreviewDragLeaveCommandBehavior
    public class ChartAreaPreviewDragLeaveCommandBehavior : ChartAreaPreviewDragLeaveCommandBehavior<object>
    { }

    // ChartAreaPreviewDragLeaveCommandWithEventArgs	
    public class ChartAreaPreviewDragLeaveCommandWithEventArgs : ChartAreaPreviewDragLeaveCommand<DragEventArgs, ChartAreaPreviewDragLeaveCommandBehaviorWithEventArgs>
    { }

    // ChartAreaPreviewDragLeaveCommandBehaviorWithEventArgs
    public class ChartAreaPreviewDragLeaveCommandBehaviorWithEventArgs : ChartAreaPreviewDragLeaveCommandBehavior<DragEventArgs>
    {
        public ChartAreaPreviewDragLeaveCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaDragLeave
    // ChartAreaDragLeaveCommand<T, TBehavior>
    public class ChartAreaDragLeaveCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaDragLeaveCommandBehavior<T>, new()
    { }

    // ChartAreaDragLeaveCommandBehavior<TReturn>
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

    // ChartAreaDragLeaveCommand
    public class ChartAreaDragLeaveCommand : ChartAreaCommandBase<ChartAreaDragLeaveCommandBehavior>
    { }

    // ChartAreaDragLeaveCommandBehavior
    public class ChartAreaDragLeaveCommandBehavior : ChartAreaDragLeaveCommandBehavior<object>
    { }

    // ChartAreaDragLeaveCommandWithEventArgs	
    public class ChartAreaDragLeaveCommandWithEventArgs : ChartAreaDragLeaveCommand<DragEventArgs, ChartAreaDragLeaveCommandBehaviorWithEventArgs>
    { }

    // ChartAreaDragLeaveCommandBehaviorWithEventArgs
    public class ChartAreaDragLeaveCommandBehaviorWithEventArgs : ChartAreaDragLeaveCommandBehavior<DragEventArgs>
    {
        public ChartAreaDragLeaveCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaPreviewDrop
    // ChartAreaPreviewDropCommand<T, TBehavior>
    public class ChartAreaPreviewDropCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaPreviewDropCommandBehavior<T>, new()
    { }

    // ChartAreaPreviewDropCommandBehavior<TReturn>
    public class ChartAreaPreviewDropCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, DragEventArgs>
    {
        public ChartAreaPreviewDropCommandBehavior(Func<object, DragEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaPreviewDropCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewDrop += OnEventRaised;
        }
    }

    // ChartAreaPreviewDropCommand
    public class ChartAreaPreviewDropCommand : ChartAreaCommandBase<ChartAreaPreviewDropCommandBehavior>
    { }

    // ChartAreaPreviewDropCommandBehavior
    public class ChartAreaPreviewDropCommandBehavior : ChartAreaPreviewDropCommandBehavior<object>
    { }

    // ChartAreaPreviewDropCommandWithEventArgs	
    public class ChartAreaPreviewDropCommandWithEventArgs : ChartAreaPreviewDropCommand<DragEventArgs, ChartAreaPreviewDropCommandBehaviorWithEventArgs>
    { }

    // ChartAreaPreviewDropCommandBehaviorWithEventArgs
    public class ChartAreaPreviewDropCommandBehaviorWithEventArgs : ChartAreaPreviewDropCommandBehavior<DragEventArgs>
    {
        public ChartAreaPreviewDropCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaDrop
    // ChartAreaDropCommand<T, TBehavior>
    public class ChartAreaDropCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaDropCommandBehavior<T>, new()
    { }

    // ChartAreaDropCommandBehavior<TReturn>
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

    // ChartAreaDropCommand
    public class ChartAreaDropCommand : ChartAreaCommandBase<ChartAreaDropCommandBehavior>
    { }

    // ChartAreaDropCommandBehavior
    public class ChartAreaDropCommandBehavior : ChartAreaDropCommandBehavior<object>
    { }

    // ChartAreaDropCommandWithEventArgs	
    public class ChartAreaDropCommandWithEventArgs : ChartAreaDropCommand<DragEventArgs, ChartAreaDropCommandBehaviorWithEventArgs>
    { }

    // ChartAreaDropCommandBehaviorWithEventArgs
    public class ChartAreaDropCommandBehaviorWithEventArgs : ChartAreaDropCommandBehavior<DragEventArgs>
    {
        public ChartAreaDropCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
#if SyncfusionFramework4_0
    #region ChartAreaPreviewTouchDown
    // ChartAreaPreviewTouchDownCommand<T, TBehavior>
    public class ChartAreaPreviewTouchDownCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaPreviewTouchDownCommandBehavior<T>, new()
    { }

    // ChartAreaPreviewTouchDownCommandBehavior<TReturn>
    public class ChartAreaPreviewTouchDownCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, TouchEventArgs>
    {
        public ChartAreaPreviewTouchDownCommandBehavior(Func<object, TouchEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaPreviewTouchDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewTouchDown += OnEventRaised;
        }
    }

    // ChartAreaPreviewTouchDownCommand
    public class ChartAreaPreviewTouchDownCommand : ChartAreaCommandBase<ChartAreaPreviewTouchDownCommandBehavior>
    { }

    // ChartAreaPreviewTouchDownCommandBehavior
    public class ChartAreaPreviewTouchDownCommandBehavior : ChartAreaPreviewTouchDownCommandBehavior<object>
    { }

    // ChartAreaPreviewTouchDownCommandWithEventArgs	
    public class ChartAreaPreviewTouchDownCommandWithEventArgs : ChartAreaPreviewTouchDownCommand<TouchEventArgs, ChartAreaPreviewTouchDownCommandBehaviorWithEventArgs>
    { }

    // ChartAreaPreviewTouchDownCommandBehaviorWithEventArgs
    public class ChartAreaPreviewTouchDownCommandBehaviorWithEventArgs : ChartAreaPreviewTouchDownCommandBehavior<TouchEventArgs>
    {
        public ChartAreaPreviewTouchDownCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaTouchDown
    // ChartAreaTouchDownCommand<T, TBehavior>
    public class ChartAreaTouchDownCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaTouchDownCommandBehavior<T>, new()
    { }

    // ChartAreaTouchDownCommandBehavior<TReturn>
    public class ChartAreaTouchDownCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, TouchEventArgs>
    {
        public ChartAreaTouchDownCommandBehavior(Func<object, TouchEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaTouchDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.TouchDown += OnEventRaised;
        }
    }

    // ChartAreaTouchDownCommand
    public class ChartAreaTouchDownCommand : ChartAreaCommandBase<ChartAreaTouchDownCommandBehavior>
    { }

    // ChartAreaTouchDownCommandBehavior
    public class ChartAreaTouchDownCommandBehavior : ChartAreaTouchDownCommandBehavior<object>
    { }

    // ChartAreaTouchDownCommandWithEventArgs	
    public class ChartAreaTouchDownCommandWithEventArgs : ChartAreaTouchDownCommand<TouchEventArgs, ChartAreaTouchDownCommandBehaviorWithEventArgs>
    { }

    // ChartAreaTouchDownCommandBehaviorWithEventArgs
    public class ChartAreaTouchDownCommandBehaviorWithEventArgs : ChartAreaTouchDownCommandBehavior<TouchEventArgs>
    {
        public ChartAreaTouchDownCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaPreviewTouchMove
    // ChartAreaPreviewTouchMoveCommand<T, TBehavior>
    public class ChartAreaPreviewTouchMoveCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaPreviewTouchMoveCommandBehavior<T>, new()
    { }

    // ChartAreaPreviewTouchMoveCommandBehavior<TReturn>
    public class ChartAreaPreviewTouchMoveCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, TouchEventArgs>
    {
        public ChartAreaPreviewTouchMoveCommandBehavior(Func<object, TouchEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaPreviewTouchMoveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewTouchMove += OnEventRaised;
        }
    }

    // ChartAreaPreviewTouchMoveCommand
    public class ChartAreaPreviewTouchMoveCommand : ChartAreaCommandBase<ChartAreaPreviewTouchMoveCommandBehavior>
    { }

    // ChartAreaPreviewTouchMoveCommandBehavior
    public class ChartAreaPreviewTouchMoveCommandBehavior : ChartAreaPreviewTouchMoveCommandBehavior<object>
    { }

    // ChartAreaPreviewTouchMoveCommandWithEventArgs	
    public class ChartAreaPreviewTouchMoveCommandWithEventArgs : ChartAreaPreviewTouchMoveCommand<TouchEventArgs, ChartAreaPreviewTouchMoveCommandBehaviorWithEventArgs>
    { }

    // ChartAreaPreviewTouchMoveCommandBehaviorWithEventArgs
    public class ChartAreaPreviewTouchMoveCommandBehaviorWithEventArgs : ChartAreaPreviewTouchMoveCommandBehavior<TouchEventArgs>
    {
        public ChartAreaPreviewTouchMoveCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaTouchMove
    // ChartAreaTouchMoveCommand<T, TBehavior>
    public class ChartAreaTouchMoveCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaTouchMoveCommandBehavior<T>, new()
    { }

    // ChartAreaTouchMoveCommandBehavior<TReturn>
    public class ChartAreaTouchMoveCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, TouchEventArgs>
    {
        public ChartAreaTouchMoveCommandBehavior(Func<object, TouchEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaTouchMoveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.TouchMove += OnEventRaised;
        }
    }

    // ChartAreaTouchMoveCommand
    public class ChartAreaTouchMoveCommand : ChartAreaCommandBase<ChartAreaTouchMoveCommandBehavior>
    { }

    // ChartAreaTouchMoveCommandBehavior
    public class ChartAreaTouchMoveCommandBehavior : ChartAreaTouchMoveCommandBehavior<object>
    { }

    // ChartAreaTouchMoveCommandWithEventArgs	
    public class ChartAreaTouchMoveCommandWithEventArgs : ChartAreaTouchMoveCommand<TouchEventArgs, ChartAreaTouchMoveCommandBehaviorWithEventArgs>
    { }

    // ChartAreaTouchMoveCommandBehaviorWithEventArgs
    public class ChartAreaTouchMoveCommandBehaviorWithEventArgs : ChartAreaTouchMoveCommandBehavior<TouchEventArgs>
    {
        public ChartAreaTouchMoveCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaPreviewTouchUp
    // ChartAreaPreviewTouchUpCommand<T, TBehavior>
    public class ChartAreaPreviewTouchUpCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaPreviewTouchUpCommandBehavior<T>, new()
    { }

    // ChartAreaPreviewTouchUpCommandBehavior<TReturn>
    public class ChartAreaPreviewTouchUpCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, TouchEventArgs>
    {
        public ChartAreaPreviewTouchUpCommandBehavior(Func<object, TouchEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaPreviewTouchUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewTouchUp += OnEventRaised;
        }
    }

    // ChartAreaPreviewTouchUpCommand
    public class ChartAreaPreviewTouchUpCommand : ChartAreaCommandBase<ChartAreaPreviewTouchUpCommandBehavior>
    { }

    // ChartAreaPreviewTouchUpCommandBehavior
    public class ChartAreaPreviewTouchUpCommandBehavior : ChartAreaPreviewTouchUpCommandBehavior<object>
    { }

    // ChartAreaPreviewTouchUpCommandWithEventArgs	
    public class ChartAreaPreviewTouchUpCommandWithEventArgs : ChartAreaPreviewTouchUpCommand<TouchEventArgs, ChartAreaPreviewTouchUpCommandBehaviorWithEventArgs>
    { }

    // ChartAreaPreviewTouchUpCommandBehaviorWithEventArgs
    public class ChartAreaPreviewTouchUpCommandBehaviorWithEventArgs : ChartAreaPreviewTouchUpCommandBehavior<TouchEventArgs>
    {
        public ChartAreaPreviewTouchUpCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaTouchUp
    // ChartAreaTouchUpCommand<T, TBehavior>
    public class ChartAreaTouchUpCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaTouchUpCommandBehavior<T>, new()
    { }

    // ChartAreaTouchUpCommandBehavior<TReturn>
    public class ChartAreaTouchUpCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, TouchEventArgs>
    {
        public ChartAreaTouchUpCommandBehavior(Func<object, TouchEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaTouchUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.TouchUp += OnEventRaised;
        }
    }

    // ChartAreaTouchUpCommand
    public class ChartAreaTouchUpCommand : ChartAreaCommandBase<ChartAreaTouchUpCommandBehavior>
    { }

    // ChartAreaTouchUpCommandBehavior
    public class ChartAreaTouchUpCommandBehavior : ChartAreaTouchUpCommandBehavior<object>
    { }

    // ChartAreaTouchUpCommandWithEventArgs	
    public class ChartAreaTouchUpCommandWithEventArgs : ChartAreaTouchUpCommand<TouchEventArgs, ChartAreaTouchUpCommandBehaviorWithEventArgs>
    { }

    // ChartAreaTouchUpCommandBehaviorWithEventArgs
    public class ChartAreaTouchUpCommandBehaviorWithEventArgs : ChartAreaTouchUpCommandBehavior<TouchEventArgs>
    {
        public ChartAreaTouchUpCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaGotTouchCapture
    // ChartAreaGotTouchCaptureCommand<T, TBehavior>
    public class ChartAreaGotTouchCaptureCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaGotTouchCaptureCommandBehavior<T>, new()
    { }

    // ChartAreaGotTouchCaptureCommandBehavior<TReturn>
    public class ChartAreaGotTouchCaptureCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, TouchEventArgs>
    {
        public ChartAreaGotTouchCaptureCommandBehavior(Func<object, TouchEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaGotTouchCaptureCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.GotTouchCapture += OnEventRaised;
        }
    }

    // ChartAreaGotTouchCaptureCommand
    public class ChartAreaGotTouchCaptureCommand : ChartAreaCommandBase<ChartAreaGotTouchCaptureCommandBehavior>
    { }

    // ChartAreaGotTouchCaptureCommandBehavior
    public class ChartAreaGotTouchCaptureCommandBehavior : ChartAreaGotTouchCaptureCommandBehavior<object>
    { }

    // ChartAreaGotTouchCaptureCommandWithEventArgs	
    public class ChartAreaGotTouchCaptureCommandWithEventArgs : ChartAreaGotTouchCaptureCommand<TouchEventArgs, ChartAreaGotTouchCaptureCommandBehaviorWithEventArgs>
    { }

    // ChartAreaGotTouchCaptureCommandBehaviorWithEventArgs
    public class ChartAreaGotTouchCaptureCommandBehaviorWithEventArgs : ChartAreaGotTouchCaptureCommandBehavior<TouchEventArgs>
    {
        public ChartAreaGotTouchCaptureCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaLostTouchCapture
    // ChartAreaLostTouchCaptureCommand<T, TBehavior>
    public class ChartAreaLostTouchCaptureCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaLostTouchCaptureCommandBehavior<T>, new()
    { }

    // ChartAreaLostTouchCaptureCommandBehavior<TReturn>
    public class ChartAreaLostTouchCaptureCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, TouchEventArgs>
    {
        public ChartAreaLostTouchCaptureCommandBehavior(Func<object, TouchEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaLostTouchCaptureCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.LostTouchCapture += OnEventRaised;
        }
    }

    // ChartAreaLostTouchCaptureCommand
    public class ChartAreaLostTouchCaptureCommand : ChartAreaCommandBase<ChartAreaLostTouchCaptureCommandBehavior>
    { }

    // ChartAreaLostTouchCaptureCommandBehavior
    public class ChartAreaLostTouchCaptureCommandBehavior : ChartAreaLostTouchCaptureCommandBehavior<object>
    { }

    // ChartAreaLostTouchCaptureCommandWithEventArgs	
    public class ChartAreaLostTouchCaptureCommandWithEventArgs : ChartAreaLostTouchCaptureCommand<TouchEventArgs, ChartAreaLostTouchCaptureCommandBehaviorWithEventArgs>
    { }

    // ChartAreaLostTouchCaptureCommandBehaviorWithEventArgs
    public class ChartAreaLostTouchCaptureCommandBehaviorWithEventArgs : ChartAreaLostTouchCaptureCommandBehavior<TouchEventArgs>
    {
        public ChartAreaLostTouchCaptureCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaTouchEnter
    // ChartAreaTouchEnterCommand<T, TBehavior>
    public class ChartAreaTouchEnterCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaTouchEnterCommandBehavior<T>, new()
    { }

    // ChartAreaTouchEnterCommandBehavior<TReturn>
    public class ChartAreaTouchEnterCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, TouchEventArgs>
    {
        public ChartAreaTouchEnterCommandBehavior(Func<object, TouchEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaTouchEnterCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.TouchEnter += OnEventRaised;
        }
    }

    // ChartAreaTouchEnterCommand
    public class ChartAreaTouchEnterCommand : ChartAreaCommandBase<ChartAreaTouchEnterCommandBehavior>
    { }

    // ChartAreaTouchEnterCommandBehavior
    public class ChartAreaTouchEnterCommandBehavior : ChartAreaTouchEnterCommandBehavior<object>
    { }

    // ChartAreaTouchEnterCommandWithEventArgs	
    public class ChartAreaTouchEnterCommandWithEventArgs : ChartAreaTouchEnterCommand<TouchEventArgs, ChartAreaTouchEnterCommandBehaviorWithEventArgs>
    { }

    // ChartAreaTouchEnterCommandBehaviorWithEventArgs
    public class ChartAreaTouchEnterCommandBehaviorWithEventArgs : ChartAreaTouchEnterCommandBehavior<TouchEventArgs>
    {
        public ChartAreaTouchEnterCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaTouchLeave
    // ChartAreaTouchLeaveCommand<T, TBehavior>
    public class ChartAreaTouchLeaveCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaTouchLeaveCommandBehavior<T>, new()
    { }

    // ChartAreaTouchLeaveCommandBehavior<TReturn>
    public class ChartAreaTouchLeaveCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, TouchEventArgs>
    {
        public ChartAreaTouchLeaveCommandBehavior(Func<object, TouchEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaTouchLeaveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.TouchLeave += OnEventRaised;
        }
    }

    // ChartAreaTouchLeaveCommand
    public class ChartAreaTouchLeaveCommand : ChartAreaCommandBase<ChartAreaTouchLeaveCommandBehavior>
    { }

    // ChartAreaTouchLeaveCommandBehavior
    public class ChartAreaTouchLeaveCommandBehavior : ChartAreaTouchLeaveCommandBehavior<object>
    { }

    // ChartAreaTouchLeaveCommandWithEventArgs	
    public class ChartAreaTouchLeaveCommandWithEventArgs : ChartAreaTouchLeaveCommand<TouchEventArgs, ChartAreaTouchLeaveCommandBehaviorWithEventArgs>
    { }

    // ChartAreaTouchLeaveCommandBehaviorWithEventArgs
    public class ChartAreaTouchLeaveCommandBehaviorWithEventArgs : ChartAreaTouchLeaveCommandBehavior<TouchEventArgs>
    {
        public ChartAreaTouchLeaveCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
#endif
    #region ChartAreaIsMouseDirectlyOverChanged
    // ChartAreaIsMouseDirectlyOverChangedCommand<T, TBehavior>
    public class ChartAreaIsMouseDirectlyOverChangedCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaIsMouseDirectlyOverChangedCommandBehavior<T>, new()
    { }

    // ChartAreaIsMouseDirectlyOverChangedCommandBehavior<TReturn>
    public class ChartAreaIsMouseDirectlyOverChangedCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public ChartAreaIsMouseDirectlyOverChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaIsMouseDirectlyOverChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsMouseDirectlyOverChanged += OnEventRaised;
        }
    }

    // ChartAreaIsMouseDirectlyOverChangedCommand
    public class ChartAreaIsMouseDirectlyOverChangedCommand : ChartAreaCommandBase<ChartAreaIsMouseDirectlyOverChangedCommandBehavior>
    { }

    // ChartAreaIsMouseDirectlyOverChangedCommandBehavior
    public class ChartAreaIsMouseDirectlyOverChangedCommandBehavior : ChartAreaIsMouseDirectlyOverChangedCommandBehavior<object>
    { }

    // ChartAreaIsMouseDirectlyOverChangedCommandWithEventArgs	
    public class ChartAreaIsMouseDirectlyOverChangedCommandWithEventArgs : ChartAreaIsMouseDirectlyOverChangedCommand<DependencyPropertyChangedEventArgs, ChartAreaIsMouseDirectlyOverChangedCommandBehaviorWithEventArgs>
    { }

    // ChartAreaIsMouseDirectlyOverChangedCommandBehaviorWithEventArgs
    public class ChartAreaIsMouseDirectlyOverChangedCommandBehaviorWithEventArgs : ChartAreaIsMouseDirectlyOverChangedCommandBehavior<DependencyPropertyChangedEventArgs>
    {
        public ChartAreaIsMouseDirectlyOverChangedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaIsKeyboardFocusWithinChanged
    // ChartAreaIsKeyboardFocusWithinChangedCommand<T, TBehavior>
    public class ChartAreaIsKeyboardFocusWithinChangedCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaIsKeyboardFocusWithinChangedCommandBehavior<T>, new()
    { }

    // ChartAreaIsKeyboardFocusWithinChangedCommandBehavior<TReturn>
    public class ChartAreaIsKeyboardFocusWithinChangedCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public ChartAreaIsKeyboardFocusWithinChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaIsKeyboardFocusWithinChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsKeyboardFocusWithinChanged += OnEventRaised;
        }
    }

    // ChartAreaIsKeyboardFocusWithinChangedCommand
    public class ChartAreaIsKeyboardFocusWithinChangedCommand : ChartAreaCommandBase<ChartAreaIsKeyboardFocusWithinChangedCommandBehavior>
    { }

    // ChartAreaIsKeyboardFocusWithinChangedCommandBehavior
    public class ChartAreaIsKeyboardFocusWithinChangedCommandBehavior : ChartAreaIsKeyboardFocusWithinChangedCommandBehavior<object>
    { }

    // ChartAreaIsKeyboardFocusWithinChangedCommandWithEventArgs	
    public class ChartAreaIsKeyboardFocusWithinChangedCommandWithEventArgs : ChartAreaIsKeyboardFocusWithinChangedCommand<DependencyPropertyChangedEventArgs, ChartAreaIsKeyboardFocusWithinChangedCommandBehaviorWithEventArgs>
    { }

    // ChartAreaIsKeyboardFocusWithinChangedCommandBehaviorWithEventArgs
    public class ChartAreaIsKeyboardFocusWithinChangedCommandBehaviorWithEventArgs : ChartAreaIsKeyboardFocusWithinChangedCommandBehavior<DependencyPropertyChangedEventArgs>
    {
        public ChartAreaIsKeyboardFocusWithinChangedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaIsMouseCapturedChanged
    // ChartAreaIsMouseCapturedChangedCommand<T, TBehavior>
    public class ChartAreaIsMouseCapturedChangedCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaIsMouseCapturedChangedCommandBehavior<T>, new()
    { }

    // ChartAreaIsMouseCapturedChangedCommandBehavior<TReturn>
    public class ChartAreaIsMouseCapturedChangedCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public ChartAreaIsMouseCapturedChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaIsMouseCapturedChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsMouseCapturedChanged += OnEventRaised;
        }
    }

    // ChartAreaIsMouseCapturedChangedCommand
    public class ChartAreaIsMouseCapturedChangedCommand : ChartAreaCommandBase<ChartAreaIsMouseCapturedChangedCommandBehavior>
    { }

    // ChartAreaIsMouseCapturedChangedCommandBehavior
    public class ChartAreaIsMouseCapturedChangedCommandBehavior : ChartAreaIsMouseCapturedChangedCommandBehavior<object>
    { }

    // ChartAreaIsMouseCapturedChangedCommandWithEventArgs	
    public class ChartAreaIsMouseCapturedChangedCommandWithEventArgs : ChartAreaIsMouseCapturedChangedCommand<DependencyPropertyChangedEventArgs, ChartAreaIsMouseCapturedChangedCommandBehaviorWithEventArgs>
    { }

    // ChartAreaIsMouseCapturedChangedCommandBehaviorWithEventArgs
    public class ChartAreaIsMouseCapturedChangedCommandBehaviorWithEventArgs : ChartAreaIsMouseCapturedChangedCommandBehavior<DependencyPropertyChangedEventArgs>
    {
        public ChartAreaIsMouseCapturedChangedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaIsMouseCaptureWithinChanged
    // ChartAreaIsMouseCaptureWithinChangedCommand<T, TBehavior>
    public class ChartAreaIsMouseCaptureWithinChangedCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaIsMouseCaptureWithinChangedCommandBehavior<T>, new()
    { }

    // ChartAreaIsMouseCaptureWithinChangedCommandBehavior<TReturn>
    public class ChartAreaIsMouseCaptureWithinChangedCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public ChartAreaIsMouseCaptureWithinChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaIsMouseCaptureWithinChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsMouseCaptureWithinChanged += OnEventRaised;
        }
    }

    // ChartAreaIsMouseCaptureWithinChangedCommand
    public class ChartAreaIsMouseCaptureWithinChangedCommand : ChartAreaCommandBase<ChartAreaIsMouseCaptureWithinChangedCommandBehavior>
    { }

    // ChartAreaIsMouseCaptureWithinChangedCommandBehavior
    public class ChartAreaIsMouseCaptureWithinChangedCommandBehavior : ChartAreaIsMouseCaptureWithinChangedCommandBehavior<object>
    { }

    // ChartAreaIsMouseCaptureWithinChangedCommandWithEventArgs	
    public class ChartAreaIsMouseCaptureWithinChangedCommandWithEventArgs : ChartAreaIsMouseCaptureWithinChangedCommand<DependencyPropertyChangedEventArgs, ChartAreaIsMouseCaptureWithinChangedCommandBehaviorWithEventArgs>
    { }

    // ChartAreaIsMouseCaptureWithinChangedCommandBehaviorWithEventArgs
    public class ChartAreaIsMouseCaptureWithinChangedCommandBehaviorWithEventArgs : ChartAreaIsMouseCaptureWithinChangedCommandBehavior<DependencyPropertyChangedEventArgs>
    {
        public ChartAreaIsMouseCaptureWithinChangedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaIsStylusDirectlyOverChanged
    // ChartAreaIsStylusDirectlyOverChangedCommand<T, TBehavior>
    public class ChartAreaIsStylusDirectlyOverChangedCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaIsStylusDirectlyOverChangedCommandBehavior<T>, new()
    { }

    // ChartAreaIsStylusDirectlyOverChangedCommandBehavior<TReturn>
    public class ChartAreaIsStylusDirectlyOverChangedCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public ChartAreaIsStylusDirectlyOverChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaIsStylusDirectlyOverChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsStylusDirectlyOverChanged += OnEventRaised;
        }
    }

    // ChartAreaIsStylusDirectlyOverChangedCommand
    public class ChartAreaIsStylusDirectlyOverChangedCommand : ChartAreaCommandBase<ChartAreaIsStylusDirectlyOverChangedCommandBehavior>
    { }

    // ChartAreaIsStylusDirectlyOverChangedCommandBehavior
    public class ChartAreaIsStylusDirectlyOverChangedCommandBehavior : ChartAreaIsStylusDirectlyOverChangedCommandBehavior<object>
    { }

    // ChartAreaIsStylusDirectlyOverChangedCommandWithEventArgs	
    public class ChartAreaIsStylusDirectlyOverChangedCommandWithEventArgs : ChartAreaIsStylusDirectlyOverChangedCommand<DependencyPropertyChangedEventArgs, ChartAreaIsStylusDirectlyOverChangedCommandBehaviorWithEventArgs>
    { }

    // ChartAreaIsStylusDirectlyOverChangedCommandBehaviorWithEventArgs
    public class ChartAreaIsStylusDirectlyOverChangedCommandBehaviorWithEventArgs : ChartAreaIsStylusDirectlyOverChangedCommandBehavior<DependencyPropertyChangedEventArgs>
    {
        public ChartAreaIsStylusDirectlyOverChangedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaIsStylusCapturedChanged
    // ChartAreaIsStylusCapturedChangedCommand<T, TBehavior>
    public class ChartAreaIsStylusCapturedChangedCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaIsStylusCapturedChangedCommandBehavior<T>, new()
    { }

    // ChartAreaIsStylusCapturedChangedCommandBehavior<TReturn>
    public class ChartAreaIsStylusCapturedChangedCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public ChartAreaIsStylusCapturedChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaIsStylusCapturedChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsStylusCapturedChanged += OnEventRaised;
        }
    }

    // ChartAreaIsStylusCapturedChangedCommand
    public class ChartAreaIsStylusCapturedChangedCommand : ChartAreaCommandBase<ChartAreaIsStylusCapturedChangedCommandBehavior>
    { }

    // ChartAreaIsStylusCapturedChangedCommandBehavior
    public class ChartAreaIsStylusCapturedChangedCommandBehavior : ChartAreaIsStylusCapturedChangedCommandBehavior<object>
    { }

    // ChartAreaIsStylusCapturedChangedCommandWithEventArgs	
    public class ChartAreaIsStylusCapturedChangedCommandWithEventArgs : ChartAreaIsStylusCapturedChangedCommand<DependencyPropertyChangedEventArgs, ChartAreaIsStylusCapturedChangedCommandBehaviorWithEventArgs>
    { }

    // ChartAreaIsStylusCapturedChangedCommandBehaviorWithEventArgs
    public class ChartAreaIsStylusCapturedChangedCommandBehaviorWithEventArgs : ChartAreaIsStylusCapturedChangedCommandBehavior<DependencyPropertyChangedEventArgs>
    {
        public ChartAreaIsStylusCapturedChangedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaIsStylusCaptureWithinChanged
    // ChartAreaIsStylusCaptureWithinChangedCommand<T, TBehavior>
    public class ChartAreaIsStylusCaptureWithinChangedCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaIsStylusCaptureWithinChangedCommandBehavior<T>, new()
    { }

    // ChartAreaIsStylusCaptureWithinChangedCommandBehavior<TReturn>
    public class ChartAreaIsStylusCaptureWithinChangedCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public ChartAreaIsStylusCaptureWithinChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaIsStylusCaptureWithinChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsStylusCaptureWithinChanged += OnEventRaised;
        }
    }

    // ChartAreaIsStylusCaptureWithinChangedCommand
    public class ChartAreaIsStylusCaptureWithinChangedCommand : ChartAreaCommandBase<ChartAreaIsStylusCaptureWithinChangedCommandBehavior>
    { }

    // ChartAreaIsStylusCaptureWithinChangedCommandBehavior
    public class ChartAreaIsStylusCaptureWithinChangedCommandBehavior : ChartAreaIsStylusCaptureWithinChangedCommandBehavior<object>
    { }

    // ChartAreaIsStylusCaptureWithinChangedCommandWithEventArgs	
    public class ChartAreaIsStylusCaptureWithinChangedCommandWithEventArgs : ChartAreaIsStylusCaptureWithinChangedCommand<DependencyPropertyChangedEventArgs, ChartAreaIsStylusCaptureWithinChangedCommandBehaviorWithEventArgs>
    { }

    // ChartAreaIsStylusCaptureWithinChangedCommandBehaviorWithEventArgs
    public class ChartAreaIsStylusCaptureWithinChangedCommandBehaviorWithEventArgs : ChartAreaIsStylusCaptureWithinChangedCommandBehavior<DependencyPropertyChangedEventArgs>
    {
        public ChartAreaIsStylusCaptureWithinChangedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaIsKeyboardFocusedChanged
    // ChartAreaIsKeyboardFocusedChangedCommand<T, TBehavior>
    public class ChartAreaIsKeyboardFocusedChangedCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaIsKeyboardFocusedChangedCommandBehavior<T>, new()
    { }

    // ChartAreaIsKeyboardFocusedChangedCommandBehavior<TReturn>
    public class ChartAreaIsKeyboardFocusedChangedCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public ChartAreaIsKeyboardFocusedChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaIsKeyboardFocusedChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsKeyboardFocusedChanged += OnEventRaised;
        }
    }

    // ChartAreaIsKeyboardFocusedChangedCommand
    public class ChartAreaIsKeyboardFocusedChangedCommand : ChartAreaCommandBase<ChartAreaIsKeyboardFocusedChangedCommandBehavior>
    { }

    // ChartAreaIsKeyboardFocusedChangedCommandBehavior
    public class ChartAreaIsKeyboardFocusedChangedCommandBehavior : ChartAreaIsKeyboardFocusedChangedCommandBehavior<object>
    { }

    // ChartAreaIsKeyboardFocusedChangedCommandWithEventArgs	
    public class ChartAreaIsKeyboardFocusedChangedCommandWithEventArgs : ChartAreaIsKeyboardFocusedChangedCommand<DependencyPropertyChangedEventArgs, ChartAreaIsKeyboardFocusedChangedCommandBehaviorWithEventArgs>
    { }

    // ChartAreaIsKeyboardFocusedChangedCommandBehaviorWithEventArgs
    public class ChartAreaIsKeyboardFocusedChangedCommandBehaviorWithEventArgs : ChartAreaIsKeyboardFocusedChangedCommandBehavior<DependencyPropertyChangedEventArgs>
    {
        public ChartAreaIsKeyboardFocusedChangedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaLayoutUpdated
    // ChartAreaLayoutUpdatedCommand<T, TBehavior>
    public class ChartAreaLayoutUpdatedCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaLayoutUpdatedCommandBehavior<T>, new()
    { }

    // ChartAreaLayoutUpdatedCommandBehavior<TReturn>
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

    // ChartAreaLayoutUpdatedCommand
    public class ChartAreaLayoutUpdatedCommand : ChartAreaCommandBase<ChartAreaLayoutUpdatedCommandBehavior>
    { }

    // ChartAreaLayoutUpdatedCommandBehavior
    public class ChartAreaLayoutUpdatedCommandBehavior : ChartAreaLayoutUpdatedCommandBehavior<object>
    { }

    // ChartAreaLayoutUpdatedCommandWithEventArgs	
    public class ChartAreaLayoutUpdatedCommandWithEventArgs : ChartAreaLayoutUpdatedCommand<EventArgs, ChartAreaLayoutUpdatedCommandBehaviorWithEventArgs>
    { }

    // ChartAreaLayoutUpdatedCommandBehaviorWithEventArgs
    public class ChartAreaLayoutUpdatedCommandBehaviorWithEventArgs : ChartAreaLayoutUpdatedCommandBehavior<EventArgs>
    {
        public ChartAreaLayoutUpdatedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaGotFocus
    // ChartAreaGotFocusCommand<T, TBehavior>
    public class ChartAreaGotFocusCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaGotFocusCommandBehavior<T>, new()
    { }

    // ChartAreaGotFocusCommandBehavior<TReturn>
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

    // ChartAreaGotFocusCommand
    public class ChartAreaGotFocusCommand : ChartAreaCommandBase<ChartAreaGotFocusCommandBehavior>
    { }

    // ChartAreaGotFocusCommandBehavior
    public class ChartAreaGotFocusCommandBehavior : ChartAreaGotFocusCommandBehavior<object>
    { }

    // ChartAreaGotFocusCommandWithEventArgs	
    public class ChartAreaGotFocusCommandWithEventArgs : ChartAreaGotFocusCommand<RoutedEventArgs, ChartAreaGotFocusCommandBehaviorWithEventArgs>
    { }

    // ChartAreaGotFocusCommandBehaviorWithEventArgs
    public class ChartAreaGotFocusCommandBehaviorWithEventArgs : ChartAreaGotFocusCommandBehavior<RoutedEventArgs>
    {
        public ChartAreaGotFocusCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaLostFocus
    // ChartAreaLostFocusCommand<T, TBehavior>
    public class ChartAreaLostFocusCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaLostFocusCommandBehavior<T>, new()
    { }

    // ChartAreaLostFocusCommandBehavior<TReturn>
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

    // ChartAreaLostFocusCommand
    public class ChartAreaLostFocusCommand : ChartAreaCommandBase<ChartAreaLostFocusCommandBehavior>
    { }

    // ChartAreaLostFocusCommandBehavior
    public class ChartAreaLostFocusCommandBehavior : ChartAreaLostFocusCommandBehavior<object>
    { }

    // ChartAreaLostFocusCommandWithEventArgs	
    public class ChartAreaLostFocusCommandWithEventArgs : ChartAreaLostFocusCommand<RoutedEventArgs, ChartAreaLostFocusCommandBehaviorWithEventArgs>
    { }

    // ChartAreaLostFocusCommandBehaviorWithEventArgs
    public class ChartAreaLostFocusCommandBehaviorWithEventArgs : ChartAreaLostFocusCommandBehavior<RoutedEventArgs>
    {
        public ChartAreaLostFocusCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaIsEnabledChanged
    // ChartAreaIsEnabledChangedCommand<T, TBehavior>
    public class ChartAreaIsEnabledChangedCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaIsEnabledChangedCommandBehavior<T>, new()
    { }

    // ChartAreaIsEnabledChangedCommandBehavior<TReturn>
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

    // ChartAreaIsEnabledChangedCommand
    public class ChartAreaIsEnabledChangedCommand : ChartAreaCommandBase<ChartAreaIsEnabledChangedCommandBehavior>
    { }

    // ChartAreaIsEnabledChangedCommandBehavior
    public class ChartAreaIsEnabledChangedCommandBehavior : ChartAreaIsEnabledChangedCommandBehavior<object>
    { }

    // ChartAreaIsEnabledChangedCommandWithEventArgs	
    public class ChartAreaIsEnabledChangedCommandWithEventArgs : ChartAreaIsEnabledChangedCommand<DependencyPropertyChangedEventArgs, ChartAreaIsEnabledChangedCommandBehaviorWithEventArgs>
    { }

    // ChartAreaIsEnabledChangedCommandBehaviorWithEventArgs
    public class ChartAreaIsEnabledChangedCommandBehaviorWithEventArgs : ChartAreaIsEnabledChangedCommandBehavior<DependencyPropertyChangedEventArgs>
    {
        public ChartAreaIsEnabledChangedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaIsHitTestVisibleChanged
    // ChartAreaIsHitTestVisibleChangedCommand<T, TBehavior>
    public class ChartAreaIsHitTestVisibleChangedCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaIsHitTestVisibleChangedCommandBehavior<T>, new()
    { }

    // ChartAreaIsHitTestVisibleChangedCommandBehavior<TReturn>
    public class ChartAreaIsHitTestVisibleChangedCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public ChartAreaIsHitTestVisibleChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaIsHitTestVisibleChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsHitTestVisibleChanged += OnEventRaised;
        }
    }

    // ChartAreaIsHitTestVisibleChangedCommand
    public class ChartAreaIsHitTestVisibleChangedCommand : ChartAreaCommandBase<ChartAreaIsHitTestVisibleChangedCommandBehavior>
    { }

    // ChartAreaIsHitTestVisibleChangedCommandBehavior
    public class ChartAreaIsHitTestVisibleChangedCommandBehavior : ChartAreaIsHitTestVisibleChangedCommandBehavior<object>
    { }

    // ChartAreaIsHitTestVisibleChangedCommandWithEventArgs	
    public class ChartAreaIsHitTestVisibleChangedCommandWithEventArgs : ChartAreaIsHitTestVisibleChangedCommand<DependencyPropertyChangedEventArgs, ChartAreaIsHitTestVisibleChangedCommandBehaviorWithEventArgs>
    { }

    // ChartAreaIsHitTestVisibleChangedCommandBehaviorWithEventArgs
    public class ChartAreaIsHitTestVisibleChangedCommandBehaviorWithEventArgs : ChartAreaIsHitTestVisibleChangedCommandBehavior<DependencyPropertyChangedEventArgs>
    {
        public ChartAreaIsHitTestVisibleChangedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaIsVisibleChanged
    // ChartAreaIsVisibleChangedCommand<T, TBehavior>
    public class ChartAreaIsVisibleChangedCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaIsVisibleChangedCommandBehavior<T>, new()
    { }

    // ChartAreaIsVisibleChangedCommandBehavior<TReturn>
    public class ChartAreaIsVisibleChangedCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public ChartAreaIsVisibleChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaIsVisibleChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsVisibleChanged += OnEventRaised;
        }
    }

    // ChartAreaIsVisibleChangedCommand
    public class ChartAreaIsVisibleChangedCommand : ChartAreaCommandBase<ChartAreaIsVisibleChangedCommandBehavior>
    { }

    // ChartAreaIsVisibleChangedCommandBehavior
    public class ChartAreaIsVisibleChangedCommandBehavior : ChartAreaIsVisibleChangedCommandBehavior<object>
    { }

    // ChartAreaIsVisibleChangedCommandWithEventArgs	
    public class ChartAreaIsVisibleChangedCommandWithEventArgs : ChartAreaIsVisibleChangedCommand<DependencyPropertyChangedEventArgs, ChartAreaIsVisibleChangedCommandBehaviorWithEventArgs>
    { }

    // ChartAreaIsVisibleChangedCommandBehaviorWithEventArgs
    public class ChartAreaIsVisibleChangedCommandBehaviorWithEventArgs : ChartAreaIsVisibleChangedCommandBehavior<DependencyPropertyChangedEventArgs>
    {
        public ChartAreaIsVisibleChangedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaFocusableChanged
    // ChartAreaFocusableChangedCommand<T, TBehavior>
    public class ChartAreaFocusableChangedCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaFocusableChangedCommandBehavior<T>, new()
    { }

    // ChartAreaFocusableChangedCommandBehavior<TReturn>
    public class ChartAreaFocusableChangedCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public ChartAreaFocusableChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaFocusableChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.FocusableChanged += OnEventRaised;
        }
    }

    // ChartAreaFocusableChangedCommand
    public class ChartAreaFocusableChangedCommand : ChartAreaCommandBase<ChartAreaFocusableChangedCommandBehavior>
    { }

    // ChartAreaFocusableChangedCommandBehavior
    public class ChartAreaFocusableChangedCommandBehavior : ChartAreaFocusableChangedCommandBehavior<object>
    { }

    // ChartAreaFocusableChangedCommandWithEventArgs	
    public class ChartAreaFocusableChangedCommandWithEventArgs : ChartAreaFocusableChangedCommand<DependencyPropertyChangedEventArgs, ChartAreaFocusableChangedCommandBehaviorWithEventArgs>
    { }

    // ChartAreaFocusableChangedCommandBehaviorWithEventArgs
    public class ChartAreaFocusableChangedCommandBehaviorWithEventArgs : ChartAreaFocusableChangedCommandBehavior<DependencyPropertyChangedEventArgs>
    {
        public ChartAreaFocusableChangedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
#if SyncfusionFramework4_0
    #region ChartAreaManipulationStarting
    // ChartAreaManipulationStartingCommand<T, TBehavior>
    public class ChartAreaManipulationStartingCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaManipulationStartingCommandBehavior<T>, new()
    { }

    // ChartAreaManipulationStartingCommandBehavior<TReturn>
    public class ChartAreaManipulationStartingCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, ManipulationStartingEventArgs>
    {
        public ChartAreaManipulationStartingCommandBehavior(Func<object, ManipulationStartingEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaManipulationStartingCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ManipulationStarting += OnEventRaised;
        }
    }

    // ChartAreaManipulationStartingCommand
    public class ChartAreaManipulationStartingCommand : ChartAreaCommandBase<ChartAreaManipulationStartingCommandBehavior>
    { }

    // ChartAreaManipulationStartingCommandBehavior
    public class ChartAreaManipulationStartingCommandBehavior : ChartAreaManipulationStartingCommandBehavior<object>
    { }

    // ChartAreaManipulationStartingCommandWithEventArgs	
    public class ChartAreaManipulationStartingCommandWithEventArgs : ChartAreaManipulationStartingCommand<ManipulationStartingEventArgs, ChartAreaManipulationStartingCommandBehaviorWithEventArgs>
    { }

    // ChartAreaManipulationStartingCommandBehaviorWithEventArgs
    public class ChartAreaManipulationStartingCommandBehaviorWithEventArgs : ChartAreaManipulationStartingCommandBehavior<ManipulationStartingEventArgs>
    {
        public ChartAreaManipulationStartingCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaManipulationStarted
    // ChartAreaManipulationStartedCommand<T, TBehavior>
    public class ChartAreaManipulationStartedCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaManipulationStartedCommandBehavior<T>, new()
    { }

    // ChartAreaManipulationStartedCommandBehavior<TReturn>
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

    // ChartAreaManipulationStartedCommand
    public class ChartAreaManipulationStartedCommand : ChartAreaCommandBase<ChartAreaManipulationStartedCommandBehavior>
    { }

    // ChartAreaManipulationStartedCommandBehavior
    public class ChartAreaManipulationStartedCommandBehavior : ChartAreaManipulationStartedCommandBehavior<object>
    { }

    // ChartAreaManipulationStartedCommandWithEventArgs	
    public class ChartAreaManipulationStartedCommandWithEventArgs : ChartAreaManipulationStartedCommand<ManipulationStartedEventArgs, ChartAreaManipulationStartedCommandBehaviorWithEventArgs>
    { }

    // ChartAreaManipulationStartedCommandBehaviorWithEventArgs
    public class ChartAreaManipulationStartedCommandBehaviorWithEventArgs : ChartAreaManipulationStartedCommandBehavior<ManipulationStartedEventArgs>
    {
        public ChartAreaManipulationStartedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaManipulationDelta
    // ChartAreaManipulationDeltaCommand<T, TBehavior>
    public class ChartAreaManipulationDeltaCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaManipulationDeltaCommandBehavior<T>, new()
    { }

    // ChartAreaManipulationDeltaCommandBehavior<TReturn>
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

    // ChartAreaManipulationDeltaCommand
    public class ChartAreaManipulationDeltaCommand : ChartAreaCommandBase<ChartAreaManipulationDeltaCommandBehavior>
    { }

    // ChartAreaManipulationDeltaCommandBehavior
    public class ChartAreaManipulationDeltaCommandBehavior : ChartAreaManipulationDeltaCommandBehavior<object>
    { }

    // ChartAreaManipulationDeltaCommandWithEventArgs	
    public class ChartAreaManipulationDeltaCommandWithEventArgs : ChartAreaManipulationDeltaCommand<ManipulationDeltaEventArgs, ChartAreaManipulationDeltaCommandBehaviorWithEventArgs>
    { }

    // ChartAreaManipulationDeltaCommandBehaviorWithEventArgs
    public class ChartAreaManipulationDeltaCommandBehaviorWithEventArgs : ChartAreaManipulationDeltaCommandBehavior<ManipulationDeltaEventArgs>
    {
        public ChartAreaManipulationDeltaCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaManipulationInertiaStarting
    // ChartAreaManipulationInertiaStartingCommand<T, TBehavior>
    public class ChartAreaManipulationInertiaStartingCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaManipulationInertiaStartingCommandBehavior<T>, new()
    { }

    // ChartAreaManipulationInertiaStartingCommandBehavior<TReturn>
    public class ChartAreaManipulationInertiaStartingCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, ManipulationInertiaStartingEventArgs>
    {
        public ChartAreaManipulationInertiaStartingCommandBehavior(Func<object, ManipulationInertiaStartingEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaManipulationInertiaStartingCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ManipulationInertiaStarting += OnEventRaised;
        }
    }

    // ChartAreaManipulationInertiaStartingCommand
    public class ChartAreaManipulationInertiaStartingCommand : ChartAreaCommandBase<ChartAreaManipulationInertiaStartingCommandBehavior>
    { }

    // ChartAreaManipulationInertiaStartingCommandBehavior
    public class ChartAreaManipulationInertiaStartingCommandBehavior : ChartAreaManipulationInertiaStartingCommandBehavior<object>
    { }

    // ChartAreaManipulationInertiaStartingCommandWithEventArgs	
    public class ChartAreaManipulationInertiaStartingCommandWithEventArgs : ChartAreaManipulationInertiaStartingCommand<ManipulationInertiaStartingEventArgs, ChartAreaManipulationInertiaStartingCommandBehaviorWithEventArgs>
    { }

    // ChartAreaManipulationInertiaStartingCommandBehaviorWithEventArgs
    public class ChartAreaManipulationInertiaStartingCommandBehaviorWithEventArgs : ChartAreaManipulationInertiaStartingCommandBehavior<ManipulationInertiaStartingEventArgs>
    {
        public ChartAreaManipulationInertiaStartingCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaManipulationBoundaryFeedback
    // ChartAreaManipulationBoundaryFeedbackCommand<T, TBehavior>
    public class ChartAreaManipulationBoundaryFeedbackCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaManipulationBoundaryFeedbackCommandBehavior<T>, new()
    { }

    // ChartAreaManipulationBoundaryFeedbackCommandBehavior<TReturn>
    public class ChartAreaManipulationBoundaryFeedbackCommandBehavior<TReturn> : ChartAreaCommandBehaviorBase<TReturn, ManipulationBoundaryFeedbackEventArgs>
    {
        public ChartAreaManipulationBoundaryFeedbackCommandBehavior(Func<object, ManipulationBoundaryFeedbackEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartAreaManipulationBoundaryFeedbackCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ManipulationBoundaryFeedback += OnEventRaised;
        }
    }

    // ChartAreaManipulationBoundaryFeedbackCommand
    public class ChartAreaManipulationBoundaryFeedbackCommand : ChartAreaCommandBase<ChartAreaManipulationBoundaryFeedbackCommandBehavior>
    { }

    // ChartAreaManipulationBoundaryFeedbackCommandBehavior
    public class ChartAreaManipulationBoundaryFeedbackCommandBehavior : ChartAreaManipulationBoundaryFeedbackCommandBehavior<object>
    { }

    // ChartAreaManipulationBoundaryFeedbackCommandWithEventArgs	
    public class ChartAreaManipulationBoundaryFeedbackCommandWithEventArgs : ChartAreaManipulationBoundaryFeedbackCommand<ManipulationBoundaryFeedbackEventArgs, ChartAreaManipulationBoundaryFeedbackCommandBehaviorWithEventArgs>
    { }

    // ChartAreaManipulationBoundaryFeedbackCommandBehaviorWithEventArgs
    public class ChartAreaManipulationBoundaryFeedbackCommandBehaviorWithEventArgs : ChartAreaManipulationBoundaryFeedbackCommandBehavior<ManipulationBoundaryFeedbackEventArgs>
    {
        public ChartAreaManipulationBoundaryFeedbackCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartAreaManipulationCompleted
    // ChartAreaManipulationCompletedCommand<T, TBehavior>
    public class ChartAreaManipulationCompletedCommand<T, TBehavior> : ChartAreaCommandBase<TBehavior> where TBehavior : ChartAreaManipulationCompletedCommandBehavior<T>, new()
    { }

    // ChartAreaManipulationCompletedCommandBehavior<TReturn>
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

    // ChartAreaManipulationCompletedCommand
    public class ChartAreaManipulationCompletedCommand : ChartAreaCommandBase<ChartAreaManipulationCompletedCommandBehavior>
    { }

    // ChartAreaManipulationCompletedCommandBehavior
    public class ChartAreaManipulationCompletedCommandBehavior : ChartAreaManipulationCompletedCommandBehavior<object>
    { }

    // ChartAreaManipulationCompletedCommandWithEventArgs	
    public class ChartAreaManipulationCompletedCommandWithEventArgs : ChartAreaManipulationCompletedCommand<ManipulationCompletedEventArgs, ChartAreaManipulationCompletedCommandBehaviorWithEventArgs>
    { }

    // ChartAreaManipulationCompletedCommandBehaviorWithEventArgs
    public class ChartAreaManipulationCompletedCommandBehaviorWithEventArgs : ChartAreaManipulationCompletedCommandBehavior<ManipulationCompletedEventArgs>
    {
        public ChartAreaManipulationCompletedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
#endif
    #endregion

    #region ChartCommands
    
    #region ChartPreviewMouseDoubleClick
    // ChartPreviewMouseDoubleClickCommand<T, TBehavior>
    public class ChartPreviewMouseDoubleClickCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartPreviewMouseDoubleClickCommandBehavior<T>, new()
    { }

    // ChartPreviewMouseDoubleClickCommandBehavior<TReturn>
    public class ChartPreviewMouseDoubleClickCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public ChartPreviewMouseDoubleClickCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartPreviewMouseDoubleClickCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewMouseDoubleClick += OnEventRaised;
        }
    }

    // ChartPreviewMouseDoubleClickCommand
    public class ChartPreviewMouseDoubleClickCommand : ChartCommandBase<ChartPreviewMouseDoubleClickCommandBehavior>
    { }

    // ChartPreviewMouseDoubleClickCommandBehavior
    public class ChartPreviewMouseDoubleClickCommandBehavior : ChartPreviewMouseDoubleClickCommandBehavior<object>
    { }

    // ChartPreviewMouseDoubleClickCommandWithEventArgs	
    public class ChartPreviewMouseDoubleClickCommandWithEventArgs : ChartPreviewMouseDoubleClickCommand<MouseButtonEventArgs, ChartPreviewMouseDoubleClickCommandBehaviorWithEventArgs>
    { }

    // ChartPreviewMouseDoubleClickCommandBehaviorWithEventArgs
    public class ChartPreviewMouseDoubleClickCommandBehaviorWithEventArgs : ChartPreviewMouseDoubleClickCommandBehavior<MouseButtonEventArgs>
    {
        public ChartPreviewMouseDoubleClickCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
   
    #region ChartMouseDoubleClick
    // ChartMouseDoubleClickCommand<T, TBehavior>
    public class ChartMouseDoubleClickCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartMouseDoubleClickCommandBehavior<T>, new()
    { }

    // ChartMouseDoubleClickCommandBehavior<TReturn>
    public class ChartMouseDoubleClickCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public ChartMouseDoubleClickCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartMouseDoubleClickCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseDoubleClick += OnEventRaised;
        }
    }

    // ChartMouseDoubleClickCommand
    public class ChartMouseDoubleClickCommand : ChartCommandBase<ChartMouseDoubleClickCommandBehavior>
    { }

    // ChartMouseDoubleClickCommandBehavior
    public class ChartMouseDoubleClickCommandBehavior : ChartMouseDoubleClickCommandBehavior<object>
    { }

    // ChartMouseDoubleClickCommandWithEventArgs	
    public class ChartMouseDoubleClickCommandWithEventArgs : ChartMouseDoubleClickCommand<MouseButtonEventArgs, ChartMouseDoubleClickCommandBehaviorWithEventArgs>
    { }

    // ChartMouseDoubleClickCommandBehaviorWithEventArgs
    public class ChartMouseDoubleClickCommandBehaviorWithEventArgs : ChartMouseDoubleClickCommandBehavior<MouseButtonEventArgs>
    {
        public ChartMouseDoubleClickCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region ChartTargetUpdated
    // ChartTargetUpdatedCommand<T, TBehavior>
    public class ChartTargetUpdatedCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartTargetUpdatedCommandBehavior<T>, new()
    { }

    // ChartTargetUpdatedCommandBehavior<TReturn>
    public class ChartTargetUpdatedCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, DataTransferEventArgs>
    {
        public ChartTargetUpdatedCommandBehavior(Func<object, DataTransferEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartTargetUpdatedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.TargetUpdated += OnEventRaised;
        }
    }

    // ChartTargetUpdatedCommand
    public class ChartTargetUpdatedCommand : ChartCommandBase<ChartTargetUpdatedCommandBehavior>
    { }

    // ChartTargetUpdatedCommandBehavior
    public class ChartTargetUpdatedCommandBehavior : ChartTargetUpdatedCommandBehavior<object>
    { }

    // ChartTargetUpdatedCommandWithEventArgs	
    public class ChartTargetUpdatedCommandWithEventArgs : ChartTargetUpdatedCommand<DataTransferEventArgs, ChartTargetUpdatedCommandBehaviorWithEventArgs>
    { }

    // ChartTargetUpdatedCommandBehaviorWithEventArgs
    public class ChartTargetUpdatedCommandBehaviorWithEventArgs : ChartTargetUpdatedCommandBehavior<DataTransferEventArgs>
    {
        public ChartTargetUpdatedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region ChartSourceUpdated
    // ChartSourceUpdatedCommand<T, TBehavior>
    public class ChartSourceUpdatedCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartSourceUpdatedCommandBehavior<T>, new()
    { }

    // ChartSourceUpdatedCommandBehavior<TReturn>
    public class ChartSourceUpdatedCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, DataTransferEventArgs>
    {
        public ChartSourceUpdatedCommandBehavior(Func<object, DataTransferEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartSourceUpdatedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.SourceUpdated += OnEventRaised;
        }
    }

    // ChartSourceUpdatedCommand
    public class ChartSourceUpdatedCommand : ChartCommandBase<ChartSourceUpdatedCommandBehavior>
    { }

    // ChartSourceUpdatedCommandBehavior
    public class ChartSourceUpdatedCommandBehavior : ChartSourceUpdatedCommandBehavior<object>
    { }

    // ChartSourceUpdatedCommandWithEventArgs	
    public class ChartSourceUpdatedCommandWithEventArgs : ChartSourceUpdatedCommand<DataTransferEventArgs, ChartSourceUpdatedCommandBehaviorWithEventArgs>
    { }

    // ChartSourceUpdatedCommandBehaviorWithEventArgs
    public class ChartSourceUpdatedCommandBehaviorWithEventArgs : ChartSourceUpdatedCommandBehavior<DataTransferEventArgs>
    {
        public ChartSourceUpdatedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartDataContextChanged
    // ChartDataContextChangedCommand<T, TBehavior>
    public class ChartDataContextChangedCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartDataContextChangedCommandBehavior<T>, new()
    { }

    // ChartDataContextChangedCommandBehavior<TReturn>
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

    // ChartDataContextChangedCommand
    public class ChartDataContextChangedCommand : ChartCommandBase<ChartDataContextChangedCommandBehavior>
    { }

    // ChartDataContextChangedCommandBehavior
    public class ChartDataContextChangedCommandBehavior : ChartDataContextChangedCommandBehavior<object>
    { }

    // ChartDataContextChangedCommandWithEventArgs	
    public class ChartDataContextChangedCommandWithEventArgs : ChartDataContextChangedCommand<DependencyPropertyChangedEventArgs, ChartDataContextChangedCommandBehaviorWithEventArgs>
    { }

    // ChartDataContextChangedCommandBehaviorWithEventArgs
    public class ChartDataContextChangedCommandBehaviorWithEventArgs : ChartDataContextChangedCommandBehavior<DependencyPropertyChangedEventArgs>
    {
        public ChartDataContextChangedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartRequestBringIntoView
    // ChartRequestBringIntoViewCommand<T, TBehavior>
    public class ChartRequestBringIntoViewCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartRequestBringIntoViewCommandBehavior<T>, new()
    { }

    // ChartRequestBringIntoViewCommandBehavior<TReturn>
    public class ChartRequestBringIntoViewCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, RequestBringIntoViewEventArgs>
    {
        public ChartRequestBringIntoViewCommandBehavior(Func<object, RequestBringIntoViewEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartRequestBringIntoViewCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.RequestBringIntoView += OnEventRaised;
        }
    }

    // ChartRequestBringIntoViewCommand
    public class ChartRequestBringIntoViewCommand : ChartCommandBase<ChartRequestBringIntoViewCommandBehavior>
    { }

    // ChartRequestBringIntoViewCommandBehavior
    public class ChartRequestBringIntoViewCommandBehavior : ChartRequestBringIntoViewCommandBehavior<object>
    { }

    // ChartRequestBringIntoViewCommandWithEventArgs	
    public class ChartRequestBringIntoViewCommandWithEventArgs : ChartRequestBringIntoViewCommand<RequestBringIntoViewEventArgs, ChartRequestBringIntoViewCommandBehaviorWithEventArgs>
    { }

    // ChartRequestBringIntoViewCommandBehaviorWithEventArgs
    public class ChartRequestBringIntoViewCommandBehaviorWithEventArgs : ChartRequestBringIntoViewCommandBehavior<RequestBringIntoViewEventArgs>
    {
        public ChartRequestBringIntoViewCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartSizeChanged
    // ChartSizeChangedCommand<T, TBehavior>
    public class ChartSizeChangedCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartSizeChangedCommandBehavior<T>, new()
    { }

    // ChartSizeChangedCommandBehavior<TReturn>
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

    // ChartSizeChangedCommand
    public class ChartSizeChangedCommand : ChartCommandBase<ChartSizeChangedCommandBehavior>
    { }

    // ChartSizeChangedCommandBehavior
    public class ChartSizeChangedCommandBehavior : ChartSizeChangedCommandBehavior<object>
    { }

    // ChartSizeChangedCommandWithEventArgs	
    public class ChartSizeChangedCommandWithEventArgs : ChartSizeChangedCommand<SizeChangedEventArgs, ChartSizeChangedCommandBehaviorWithEventArgs>
    { }

    // ChartSizeChangedCommandBehaviorWithEventArgs
    public class ChartSizeChangedCommandBehaviorWithEventArgs : ChartSizeChangedCommandBehavior<SizeChangedEventArgs>
    {
        public ChartSizeChangedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartInitialized
    // ChartInitializedCommand<T, TBehavior>
    public class ChartInitializedCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartInitializedCommandBehavior<T>, new()
    { }

    // ChartInitializedCommandBehavior<TReturn>
    public class ChartInitializedCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, EventArgs>
    {
        public ChartInitializedCommandBehavior(Func<object, EventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartInitializedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.Initialized += OnEventRaised;
        }
    }

    // ChartInitializedCommand
    public class ChartInitializedCommand : ChartCommandBase<ChartInitializedCommandBehavior>
    { }

    // ChartInitializedCommandBehavior
    public class ChartInitializedCommandBehavior : ChartInitializedCommandBehavior<object>
    { }

    // ChartInitializedCommandWithEventArgs	
    public class ChartInitializedCommandWithEventArgs : ChartInitializedCommand<EventArgs, ChartInitializedCommandBehaviorWithEventArgs>
    { }

    // ChartInitializedCommandBehaviorWithEventArgs
    public class ChartInitializedCommandBehaviorWithEventArgs : ChartInitializedCommandBehavior<EventArgs>
    {
        public ChartInitializedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartLoaded
    // ChartLoadedCommand<T, TBehavior>
    public class ChartLoadedCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartLoadedCommandBehavior<T>, new()
    { }

    // ChartLoadedCommandBehavior<TReturn>
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

    // ChartLoadedCommand
    public class ChartLoadedCommand : ChartCommandBase<ChartLoadedCommandBehavior>
    { }

    // ChartLoadedCommandBehavior
    public class ChartLoadedCommandBehavior : ChartLoadedCommandBehavior<object>
    { }

    // ChartLoadedCommandWithEventArgs	
    public class ChartLoadedCommandWithEventArgs : ChartLoadedCommand<RoutedEventArgs, ChartLoadedCommandBehaviorWithEventArgs>
    { }

    // ChartLoadedCommandBehaviorWithEventArgs
    public class ChartLoadedCommandBehaviorWithEventArgs : ChartLoadedCommandBehavior<RoutedEventArgs>
    {
        public ChartLoadedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartUnloaded
    // ChartUnloadedCommand<T, TBehavior>
    public class ChartUnloadedCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartUnloadedCommandBehavior<T>, new()
    { }

    // ChartUnloadedCommandBehavior<TReturn>
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

    // ChartUnloadedCommand
    public class ChartUnloadedCommand : ChartCommandBase<ChartUnloadedCommandBehavior>
    { }

    // ChartUnloadedCommandBehavior
    public class ChartUnloadedCommandBehavior : ChartUnloadedCommandBehavior<object>
    { }

    // ChartUnloadedCommandWithEventArgs	
    public class ChartUnloadedCommandWithEventArgs : ChartUnloadedCommand<RoutedEventArgs, ChartUnloadedCommandBehaviorWithEventArgs>
    { }

    // ChartUnloadedCommandBehaviorWithEventArgs
    public class ChartUnloadedCommandBehaviorWithEventArgs : ChartUnloadedCommandBehavior<RoutedEventArgs>
    {
        public ChartUnloadedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartToolTipOpening
    // ChartToolTipOpeningCommand<T, TBehavior>
    public class ChartToolTipOpeningCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartToolTipOpeningCommandBehavior<T>, new()
    { }

    // ChartToolTipOpeningCommandBehavior<TReturn>
    public class ChartToolTipOpeningCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, ToolTipEventArgs>
    {
        public ChartToolTipOpeningCommandBehavior(Func<object, ToolTipEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartToolTipOpeningCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ToolTipOpening += OnEventRaised;
        }
    }

    // ChartToolTipOpeningCommand
    public class ChartToolTipOpeningCommand : ChartCommandBase<ChartToolTipOpeningCommandBehavior>
    { }

    // ChartToolTipOpeningCommandBehavior
    public class ChartToolTipOpeningCommandBehavior : ChartToolTipOpeningCommandBehavior<object>
    { }

    // ChartToolTipOpeningCommandWithEventArgs	
    public class ChartToolTipOpeningCommandWithEventArgs : ChartToolTipOpeningCommand<ToolTipEventArgs, ChartToolTipOpeningCommandBehaviorWithEventArgs>
    { }

    // ChartToolTipOpeningCommandBehaviorWithEventArgs
    public class ChartToolTipOpeningCommandBehaviorWithEventArgs : ChartToolTipOpeningCommandBehavior<ToolTipEventArgs>
    {
        public ChartToolTipOpeningCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartToolTipClosing
    // ChartToolTipClosingCommand<T, TBehavior>
    public class ChartToolTipClosingCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartToolTipClosingCommandBehavior<T>, new()
    { }

    // ChartToolTipClosingCommandBehavior<TReturn>
    public class ChartToolTipClosingCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, ToolTipEventArgs>
    {
        public ChartToolTipClosingCommandBehavior(Func<object, ToolTipEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartToolTipClosingCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ToolTipClosing += OnEventRaised;
        }
    }

    // ChartToolTipClosingCommand
    public class ChartToolTipClosingCommand : ChartCommandBase<ChartToolTipClosingCommandBehavior>
    { }

    // ChartToolTipClosingCommandBehavior
    public class ChartToolTipClosingCommandBehavior : ChartToolTipClosingCommandBehavior<object>
    { }

    // ChartToolTipClosingCommandWithEventArgs	
    public class ChartToolTipClosingCommandWithEventArgs : ChartToolTipClosingCommand<ToolTipEventArgs, ChartToolTipClosingCommandBehaviorWithEventArgs>
    { }

    // ChartToolTipClosingCommandBehaviorWithEventArgs
    public class ChartToolTipClosingCommandBehaviorWithEventArgs : ChartToolTipClosingCommandBehavior<ToolTipEventArgs>
    {
        public ChartToolTipClosingCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartContextMenuOpening
    // ChartContextMenuOpeningCommand<T, TBehavior>
    public class ChartContextMenuOpeningCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartContextMenuOpeningCommandBehavior<T>, new()
    { }

    // ChartContextMenuOpeningCommandBehavior<TReturn>
    public class ChartContextMenuOpeningCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, ContextMenuEventArgs>
    {
        public ChartContextMenuOpeningCommandBehavior(Func<object, ContextMenuEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartContextMenuOpeningCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ContextMenuOpening += OnEventRaised;
        }
    }

    // ChartContextMenuOpeningCommand
    public class ChartContextMenuOpeningCommand : ChartCommandBase<ChartContextMenuOpeningCommandBehavior>
    { }

    // ChartContextMenuOpeningCommandBehavior
    public class ChartContextMenuOpeningCommandBehavior : ChartContextMenuOpeningCommandBehavior<object>
    { }

    // ChartContextMenuOpeningCommandWithEventArgs	
    public class ChartContextMenuOpeningCommandWithEventArgs : ChartContextMenuOpeningCommand<ContextMenuEventArgs, ChartContextMenuOpeningCommandBehaviorWithEventArgs>
    { }

    // ChartContextMenuOpeningCommandBehaviorWithEventArgs
    public class ChartContextMenuOpeningCommandBehaviorWithEventArgs : ChartContextMenuOpeningCommandBehavior<ContextMenuEventArgs>
    {
        public ChartContextMenuOpeningCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartContextMenuClosing
    // ChartContextMenuClosingCommand<T, TBehavior>
    public class ChartContextMenuClosingCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartContextMenuClosingCommandBehavior<T>, new()
    { }

    // ChartContextMenuClosingCommandBehavior<TReturn>
    public class ChartContextMenuClosingCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, ContextMenuEventArgs>
    {
        public ChartContextMenuClosingCommandBehavior(Func<object, ContextMenuEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartContextMenuClosingCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ContextMenuClosing += OnEventRaised;
        }
    }

    // ChartContextMenuClosingCommand
    public class ChartContextMenuClosingCommand : ChartCommandBase<ChartContextMenuClosingCommandBehavior>
    { }

    // ChartContextMenuClosingCommandBehavior
    public class ChartContextMenuClosingCommandBehavior : ChartContextMenuClosingCommandBehavior<object>
    { }

    // ChartContextMenuClosingCommandWithEventArgs	
    public class ChartContextMenuClosingCommandWithEventArgs : ChartContextMenuClosingCommand<ContextMenuEventArgs, ChartContextMenuClosingCommandBehaviorWithEventArgs>
    { }

    // ChartContextMenuClosingCommandBehaviorWithEventArgs
    public class ChartContextMenuClosingCommandBehaviorWithEventArgs : ChartContextMenuClosingCommandBehavior<ContextMenuEventArgs>
    {
        public ChartContextMenuClosingCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartPreviewMouseDown
    // ChartPreviewMouseDownCommand<T, TBehavior>
    public class ChartPreviewMouseDownCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartPreviewMouseDownCommandBehavior<T>, new()
    { }

    // ChartPreviewMouseDownCommandBehavior<TReturn>
    public class ChartPreviewMouseDownCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public ChartPreviewMouseDownCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartPreviewMouseDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewMouseDown += OnEventRaised;
        }
    }

    // ChartPreviewMouseDownCommand
    public class ChartPreviewMouseDownCommand : ChartCommandBase<ChartPreviewMouseDownCommandBehavior>
    { }

    // ChartPreviewMouseDownCommandBehavior
    public class ChartPreviewMouseDownCommandBehavior : ChartPreviewMouseDownCommandBehavior<object>
    { }

    // ChartPreviewMouseDownCommandWithEventArgs	
    public class ChartPreviewMouseDownCommandWithEventArgs : ChartPreviewMouseDownCommand<MouseButtonEventArgs, ChartPreviewMouseDownCommandBehaviorWithEventArgs>
    { }

    // ChartPreviewMouseDownCommandBehaviorWithEventArgs
    public class ChartPreviewMouseDownCommandBehaviorWithEventArgs : ChartPreviewMouseDownCommandBehavior<MouseButtonEventArgs>
    {
        public ChartPreviewMouseDownCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartMouseDown
    // ChartMouseDownCommand<T, TBehavior>
    public class ChartMouseDownCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartMouseDownCommandBehavior<T>, new()
    { }

    // ChartMouseDownCommandBehavior<TReturn>
    public class ChartMouseDownCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public ChartMouseDownCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartMouseDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseDown += OnEventRaised;
        }
    }

    // ChartMouseDownCommand
    public class ChartMouseDownCommand : ChartCommandBase<ChartMouseDownCommandBehavior>
    { }

    // ChartMouseDownCommandBehavior
    public class ChartMouseDownCommandBehavior : ChartMouseDownCommandBehavior<object>
    { }

    // ChartMouseDownCommandWithEventArgs	
    public class ChartMouseDownCommandWithEventArgs : ChartMouseDownCommand<MouseButtonEventArgs, ChartMouseDownCommandBehaviorWithEventArgs>
    { }

    // ChartMouseDownCommandBehaviorWithEventArgs
    public class ChartMouseDownCommandBehaviorWithEventArgs : ChartMouseDownCommandBehavior<MouseButtonEventArgs>
    {
        public ChartMouseDownCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartPreviewMouseUp
    // ChartPreviewMouseUpCommand<T, TBehavior>
    public class ChartPreviewMouseUpCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartPreviewMouseUpCommandBehavior<T>, new()
    { }

    // ChartPreviewMouseUpCommandBehavior<TReturn>
    public class ChartPreviewMouseUpCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public ChartPreviewMouseUpCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartPreviewMouseUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewMouseUp += OnEventRaised;
        }
    }

    // ChartPreviewMouseUpCommand
    public class ChartPreviewMouseUpCommand : ChartCommandBase<ChartPreviewMouseUpCommandBehavior>
    { }

    // ChartPreviewMouseUpCommandBehavior
    public class ChartPreviewMouseUpCommandBehavior : ChartPreviewMouseUpCommandBehavior<object>
    { }

    // ChartPreviewMouseUpCommandWithEventArgs	
    public class ChartPreviewMouseUpCommandWithEventArgs : ChartPreviewMouseUpCommand<MouseButtonEventArgs, ChartPreviewMouseUpCommandBehaviorWithEventArgs>
    { }

    // ChartPreviewMouseUpCommandBehaviorWithEventArgs
    public class ChartPreviewMouseUpCommandBehaviorWithEventArgs : ChartPreviewMouseUpCommandBehavior<MouseButtonEventArgs>
    {
        public ChartPreviewMouseUpCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartMouseUp
    // ChartMouseUpCommand<T, TBehavior>
    public class ChartMouseUpCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartMouseUpCommandBehavior<T>, new()
    { }

    // ChartMouseUpCommandBehavior<TReturn>
    public class ChartMouseUpCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public ChartMouseUpCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartMouseUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.MouseUp += OnEventRaised;
        }
    }

    // ChartMouseUpCommand
    public class ChartMouseUpCommand : ChartCommandBase<ChartMouseUpCommandBehavior>
    { }

    // ChartMouseUpCommandBehavior
    public class ChartMouseUpCommandBehavior : ChartMouseUpCommandBehavior<object>
    { }

    // ChartMouseUpCommandWithEventArgs	
    public class ChartMouseUpCommandWithEventArgs : ChartMouseUpCommand<MouseButtonEventArgs, ChartMouseUpCommandBehaviorWithEventArgs>
    { }

    // ChartMouseUpCommandBehaviorWithEventArgs
    public class ChartMouseUpCommandBehaviorWithEventArgs : ChartMouseUpCommandBehavior<MouseButtonEventArgs>
    {
        public ChartMouseUpCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartPreviewMouseLeftButtonDown
    // ChartPreviewMouseLeftButtonDownCommand<T, TBehavior>
    public class ChartPreviewMouseLeftButtonDownCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartPreviewMouseLeftButtonDownCommandBehavior<T>, new()
    { }

    // ChartPreviewMouseLeftButtonDownCommandBehavior<TReturn>
    public class ChartPreviewMouseLeftButtonDownCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public ChartPreviewMouseLeftButtonDownCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartPreviewMouseLeftButtonDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewMouseLeftButtonDown += OnEventRaised;
        }
    }

    // ChartPreviewMouseLeftButtonDownCommand
    public class ChartPreviewMouseLeftButtonDownCommand : ChartCommandBase<ChartPreviewMouseLeftButtonDownCommandBehavior>
    { }

    // ChartPreviewMouseLeftButtonDownCommandBehavior
    public class ChartPreviewMouseLeftButtonDownCommandBehavior : ChartPreviewMouseLeftButtonDownCommandBehavior<object>
    { }

    // ChartPreviewMouseLeftButtonDownCommandWithEventArgs	
    public class ChartPreviewMouseLeftButtonDownCommandWithEventArgs : ChartPreviewMouseLeftButtonDownCommand<MouseButtonEventArgs, ChartPreviewMouseLeftButtonDownCommandBehaviorWithEventArgs>
    { }

    // ChartPreviewMouseLeftButtonDownCommandBehaviorWithEventArgs
    public class ChartPreviewMouseLeftButtonDownCommandBehaviorWithEventArgs : ChartPreviewMouseLeftButtonDownCommandBehavior<MouseButtonEventArgs>
    {
        public ChartPreviewMouseLeftButtonDownCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartMouseLeftButtonDown
    // ChartMouseLeftButtonDownCommand<T, TBehavior>
    public class ChartMouseLeftButtonDownCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartMouseLeftButtonDownCommandBehavior<T>, new()
    { }

    // ChartMouseLeftButtonDownCommandBehavior<TReturn>
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

    // ChartMouseLeftButtonDownCommand
    public class ChartMouseLeftButtonDownCommand : ChartCommandBase<ChartMouseLeftButtonDownCommandBehavior>
    { }

    // ChartMouseLeftButtonDownCommandBehavior
    public class ChartMouseLeftButtonDownCommandBehavior : ChartMouseLeftButtonDownCommandBehavior<object>
    { }

    // ChartMouseLeftButtonDownCommandWithEventArgs	
    public class ChartMouseLeftButtonDownCommandWithEventArgs : ChartMouseLeftButtonDownCommand<MouseButtonEventArgs, ChartMouseLeftButtonDownCommandBehaviorWithEventArgs>
    { }

    // ChartMouseLeftButtonDownCommandBehaviorWithEventArgs
    public class ChartMouseLeftButtonDownCommandBehaviorWithEventArgs : ChartMouseLeftButtonDownCommandBehavior<MouseButtonEventArgs>
    {
        public ChartMouseLeftButtonDownCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartPreviewMouseLeftButtonUp
    // ChartPreviewMouseLeftButtonUpCommand<T, TBehavior>
    public class ChartPreviewMouseLeftButtonUpCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartPreviewMouseLeftButtonUpCommandBehavior<T>, new()
    { }

    // ChartPreviewMouseLeftButtonUpCommandBehavior<TReturn>
    public class ChartPreviewMouseLeftButtonUpCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public ChartPreviewMouseLeftButtonUpCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartPreviewMouseLeftButtonUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewMouseLeftButtonUp += OnEventRaised;
        }
    }

    // ChartPreviewMouseLeftButtonUpCommand
    public class ChartPreviewMouseLeftButtonUpCommand : ChartCommandBase<ChartPreviewMouseLeftButtonUpCommandBehavior>
    { }

    // ChartPreviewMouseLeftButtonUpCommandBehavior
    public class ChartPreviewMouseLeftButtonUpCommandBehavior : ChartPreviewMouseLeftButtonUpCommandBehavior<object>
    { }

    // ChartPreviewMouseLeftButtonUpCommandWithEventArgs	
    public class ChartPreviewMouseLeftButtonUpCommandWithEventArgs : ChartPreviewMouseLeftButtonUpCommand<MouseButtonEventArgs, ChartPreviewMouseLeftButtonUpCommandBehaviorWithEventArgs>
    { }

    // ChartPreviewMouseLeftButtonUpCommandBehaviorWithEventArgs
    public class ChartPreviewMouseLeftButtonUpCommandBehaviorWithEventArgs : ChartPreviewMouseLeftButtonUpCommandBehavior<MouseButtonEventArgs>
    {
        public ChartPreviewMouseLeftButtonUpCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartMouseLeftButtonUp
    // ChartMouseLeftButtonUpCommand<T, TBehavior>
    public class ChartMouseLeftButtonUpCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartMouseLeftButtonUpCommandBehavior<T>, new()
    { }

    // ChartMouseLeftButtonUpCommandBehavior<TReturn>
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

    // ChartMouseLeftButtonUpCommand
    public class ChartMouseLeftButtonUpCommand : ChartCommandBase<ChartMouseLeftButtonUpCommandBehavior>
    { }

    // ChartMouseLeftButtonUpCommandBehavior
    public class ChartMouseLeftButtonUpCommandBehavior : ChartMouseLeftButtonUpCommandBehavior<object>
    { }

    // ChartMouseLeftButtonUpCommandWithEventArgs	
    public class ChartMouseLeftButtonUpCommandWithEventArgs : ChartMouseLeftButtonUpCommand<MouseButtonEventArgs, ChartMouseLeftButtonUpCommandBehaviorWithEventArgs>
    { }

    // ChartMouseLeftButtonUpCommandBehaviorWithEventArgs
    public class ChartMouseLeftButtonUpCommandBehaviorWithEventArgs : ChartMouseLeftButtonUpCommandBehavior<MouseButtonEventArgs>
    {
        public ChartMouseLeftButtonUpCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartPreviewMouseRightButtonDown
    // ChartPreviewMouseRightButtonDownCommand<T, TBehavior>
    public class ChartPreviewMouseRightButtonDownCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartPreviewMouseRightButtonDownCommandBehavior<T>, new()
    { }

    // ChartPreviewMouseRightButtonDownCommandBehavior<TReturn>
    public class ChartPreviewMouseRightButtonDownCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public ChartPreviewMouseRightButtonDownCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartPreviewMouseRightButtonDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewMouseRightButtonDown += OnEventRaised;
        }
    }

    // ChartPreviewMouseRightButtonDownCommand
    public class ChartPreviewMouseRightButtonDownCommand : ChartCommandBase<ChartPreviewMouseRightButtonDownCommandBehavior>
    { }

    // ChartPreviewMouseRightButtonDownCommandBehavior
    public class ChartPreviewMouseRightButtonDownCommandBehavior : ChartPreviewMouseRightButtonDownCommandBehavior<object>
    { }

    // ChartPreviewMouseRightButtonDownCommandWithEventArgs	
    public class ChartPreviewMouseRightButtonDownCommandWithEventArgs : ChartPreviewMouseRightButtonDownCommand<MouseButtonEventArgs, ChartPreviewMouseRightButtonDownCommandBehaviorWithEventArgs>
    { }

    // ChartPreviewMouseRightButtonDownCommandBehaviorWithEventArgs
    public class ChartPreviewMouseRightButtonDownCommandBehaviorWithEventArgs : ChartPreviewMouseRightButtonDownCommandBehavior<MouseButtonEventArgs>
    {
        public ChartPreviewMouseRightButtonDownCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartMouseRightButtonDown
    // ChartMouseRightButtonDownCommand<T, TBehavior>
    public class ChartMouseRightButtonDownCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartMouseRightButtonDownCommandBehavior<T>, new()
    { }

    // ChartMouseRightButtonDownCommandBehavior<TReturn>
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

    // ChartMouseRightButtonDownCommand
    public class ChartMouseRightButtonDownCommand : ChartCommandBase<ChartMouseRightButtonDownCommandBehavior>
    { }

    // ChartMouseRightButtonDownCommandBehavior
    public class ChartMouseRightButtonDownCommandBehavior : ChartMouseRightButtonDownCommandBehavior<object>
    { }

    // ChartMouseRightButtonDownCommandWithEventArgs	
    public class ChartMouseRightButtonDownCommandWithEventArgs : ChartMouseRightButtonDownCommand<MouseButtonEventArgs, ChartMouseRightButtonDownCommandBehaviorWithEventArgs>
    { }

    // ChartMouseRightButtonDownCommandBehaviorWithEventArgs
    public class ChartMouseRightButtonDownCommandBehaviorWithEventArgs : ChartMouseRightButtonDownCommandBehavior<MouseButtonEventArgs>
    {
        public ChartMouseRightButtonDownCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartPreviewMouseRightButtonUp
    // ChartPreviewMouseRightButtonUpCommand<T, TBehavior>
    public class ChartPreviewMouseRightButtonUpCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartPreviewMouseRightButtonUpCommandBehavior<T>, new()
    { }

    // ChartPreviewMouseRightButtonUpCommandBehavior<TReturn>
    public class ChartPreviewMouseRightButtonUpCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, MouseButtonEventArgs>
    {
        public ChartPreviewMouseRightButtonUpCommandBehavior(Func<object, MouseButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartPreviewMouseRightButtonUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewMouseRightButtonUp += OnEventRaised;
        }
    }

    // ChartPreviewMouseRightButtonUpCommand
    public class ChartPreviewMouseRightButtonUpCommand : ChartCommandBase<ChartPreviewMouseRightButtonUpCommandBehavior>
    { }

    // ChartPreviewMouseRightButtonUpCommandBehavior
    public class ChartPreviewMouseRightButtonUpCommandBehavior : ChartPreviewMouseRightButtonUpCommandBehavior<object>
    { }

    // ChartPreviewMouseRightButtonUpCommandWithEventArgs	
    public class ChartPreviewMouseRightButtonUpCommandWithEventArgs : ChartPreviewMouseRightButtonUpCommand<MouseButtonEventArgs, ChartPreviewMouseRightButtonUpCommandBehaviorWithEventArgs>
    { }

    // ChartPreviewMouseRightButtonUpCommandBehaviorWithEventArgs
    public class ChartPreviewMouseRightButtonUpCommandBehaviorWithEventArgs : ChartPreviewMouseRightButtonUpCommandBehavior<MouseButtonEventArgs>
    {
        public ChartPreviewMouseRightButtonUpCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartMouseRightButtonUp
    // ChartMouseRightButtonUpCommand<T, TBehavior>
    public class ChartMouseRightButtonUpCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartMouseRightButtonUpCommandBehavior<T>, new()
    { }

    // ChartMouseRightButtonUpCommandBehavior<TReturn>
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

    // ChartMouseRightButtonUpCommand
    public class ChartMouseRightButtonUpCommand : ChartCommandBase<ChartMouseRightButtonUpCommandBehavior>
    { }

    // ChartMouseRightButtonUpCommandBehavior
    public class ChartMouseRightButtonUpCommandBehavior : ChartMouseRightButtonUpCommandBehavior<object>
    { }

    // ChartMouseRightButtonUpCommandWithEventArgs	
    public class ChartMouseRightButtonUpCommandWithEventArgs : ChartMouseRightButtonUpCommand<MouseButtonEventArgs, ChartMouseRightButtonUpCommandBehaviorWithEventArgs>
    { }

    // ChartMouseRightButtonUpCommandBehaviorWithEventArgs
    public class ChartMouseRightButtonUpCommandBehaviorWithEventArgs : ChartMouseRightButtonUpCommandBehavior<MouseButtonEventArgs>
    {
        public ChartMouseRightButtonUpCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartPreviewMouseMove
    // ChartPreviewMouseMoveCommand<T, TBehavior>
    public class ChartPreviewMouseMoveCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartPreviewMouseMoveCommandBehavior<T>, new()
    { }

    // ChartPreviewMouseMoveCommandBehavior<TReturn>
    public class ChartPreviewMouseMoveCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, MouseEventArgs>
    {
        public ChartPreviewMouseMoveCommandBehavior(Func<object, MouseEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartPreviewMouseMoveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewMouseMove += OnEventRaised;
        }
    }

    // ChartPreviewMouseMoveCommand
    public class ChartPreviewMouseMoveCommand : ChartCommandBase<ChartPreviewMouseMoveCommandBehavior>
    { }

    // ChartPreviewMouseMoveCommandBehavior
    public class ChartPreviewMouseMoveCommandBehavior : ChartPreviewMouseMoveCommandBehavior<object>
    { }

    // ChartPreviewMouseMoveCommandWithEventArgs	
    public class ChartPreviewMouseMoveCommandWithEventArgs : ChartPreviewMouseMoveCommand<MouseEventArgs, ChartPreviewMouseMoveCommandBehaviorWithEventArgs>
    { }

    // ChartPreviewMouseMoveCommandBehaviorWithEventArgs
    public class ChartPreviewMouseMoveCommandBehaviorWithEventArgs : ChartPreviewMouseMoveCommandBehavior<MouseEventArgs>
    {
        public ChartPreviewMouseMoveCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartMouseMove
    // ChartMouseMoveCommand<T, TBehavior>
    public class ChartMouseMoveCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartMouseMoveCommandBehavior<T>, new()
    { }

    // ChartMouseMoveCommandBehavior<TReturn>
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

    // ChartMouseMoveCommand
    public class ChartMouseMoveCommand : ChartCommandBase<ChartMouseMoveCommandBehavior>
    { }

    // ChartMouseMoveCommandBehavior
    public class ChartMouseMoveCommandBehavior : ChartMouseMoveCommandBehavior<object>
    { }

    // ChartMouseMoveCommandWithEventArgs	
    public class ChartMouseMoveCommandWithEventArgs : ChartMouseMoveCommand<MouseEventArgs, ChartMouseMoveCommandBehaviorWithEventArgs>
    { }

    // ChartMouseMoveCommandBehaviorWithEventArgs
    public class ChartMouseMoveCommandBehaviorWithEventArgs : ChartMouseMoveCommandBehavior<MouseEventArgs>
    {
        public ChartMouseMoveCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartPreviewMouseWheel
    // ChartPreviewMouseWheelCommand<T, TBehavior>
    public class ChartPreviewMouseWheelCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartPreviewMouseWheelCommandBehavior<T>, new()
    { }

    // ChartPreviewMouseWheelCommandBehavior<TReturn>
    public class ChartPreviewMouseWheelCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, MouseWheelEventArgs>
    {
        public ChartPreviewMouseWheelCommandBehavior(Func<object, MouseWheelEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartPreviewMouseWheelCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewMouseWheel += OnEventRaised;
        }
    }

    // ChartPreviewMouseWheelCommand
    public class ChartPreviewMouseWheelCommand : ChartCommandBase<ChartPreviewMouseWheelCommandBehavior>
    { }

    // ChartPreviewMouseWheelCommandBehavior
    public class ChartPreviewMouseWheelCommandBehavior : ChartPreviewMouseWheelCommandBehavior<object>
    { }

    // ChartPreviewMouseWheelCommandWithEventArgs	
    public class ChartPreviewMouseWheelCommandWithEventArgs : ChartPreviewMouseWheelCommand<MouseWheelEventArgs, ChartPreviewMouseWheelCommandBehaviorWithEventArgs>
    { }

    // ChartPreviewMouseWheelCommandBehaviorWithEventArgs
    public class ChartPreviewMouseWheelCommandBehaviorWithEventArgs : ChartPreviewMouseWheelCommandBehavior<MouseWheelEventArgs>
    {
        public ChartPreviewMouseWheelCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartMouseWheel
    // ChartMouseWheelCommand<T, TBehavior>
    public class ChartMouseWheelCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartMouseWheelCommandBehavior<T>, new()
    { }

    // ChartMouseWheelCommandBehavior<TReturn>
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

    // ChartMouseWheelCommand
    public class ChartMouseWheelCommand : ChartCommandBase<ChartMouseWheelCommandBehavior>
    { }

    // ChartMouseWheelCommandBehavior
    public class ChartMouseWheelCommandBehavior : ChartMouseWheelCommandBehavior<object>
    { }

    // ChartMouseWheelCommandWithEventArgs	
    public class ChartMouseWheelCommandWithEventArgs : ChartMouseWheelCommand<MouseWheelEventArgs, ChartMouseWheelCommandBehaviorWithEventArgs>
    { }

    // ChartMouseWheelCommandBehaviorWithEventArgs
    public class ChartMouseWheelCommandBehaviorWithEventArgs : ChartMouseWheelCommandBehavior<MouseWheelEventArgs>
    {
        public ChartMouseWheelCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartMouseEnter
    // ChartMouseEnterCommand<T, TBehavior>
    public class ChartMouseEnterCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartMouseEnterCommandBehavior<T>, new()
    { }

    // ChartMouseEnterCommandBehavior<TReturn>
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

    // ChartMouseEnterCommand
    public class ChartMouseEnterCommand : ChartCommandBase<ChartMouseEnterCommandBehavior>
    { }

    // ChartMouseEnterCommandBehavior
    public class ChartMouseEnterCommandBehavior : ChartMouseEnterCommandBehavior<object>
    { }

    // ChartMouseEnterCommandWithEventArgs	
    public class ChartMouseEnterCommandWithEventArgs : ChartMouseEnterCommand<MouseEventArgs, ChartMouseEnterCommandBehaviorWithEventArgs>
    { }

    // ChartMouseEnterCommandBehaviorWithEventArgs
    public class ChartMouseEnterCommandBehaviorWithEventArgs : ChartMouseEnterCommandBehavior<MouseEventArgs>
    {
        public ChartMouseEnterCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartMouseLeave
    // ChartMouseLeaveCommand<T, TBehavior>
    public class ChartMouseLeaveCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartMouseLeaveCommandBehavior<T>, new()
    { }

    // ChartMouseLeaveCommandBehavior<TReturn>
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

    // ChartMouseLeaveCommand
    public class ChartMouseLeaveCommand : ChartCommandBase<ChartMouseLeaveCommandBehavior>
    { }

    // ChartMouseLeaveCommandBehavior
    public class ChartMouseLeaveCommandBehavior : ChartMouseLeaveCommandBehavior<object>
    { }

    // ChartMouseLeaveCommandWithEventArgs	
    public class ChartMouseLeaveCommandWithEventArgs : ChartMouseLeaveCommand<MouseEventArgs, ChartMouseLeaveCommandBehaviorWithEventArgs>
    { }

    // ChartMouseLeaveCommandBehaviorWithEventArgs
    public class ChartMouseLeaveCommandBehaviorWithEventArgs : ChartMouseLeaveCommandBehavior<MouseEventArgs>
    {
        public ChartMouseLeaveCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartGotMouseCapture
    // ChartGotMouseCaptureCommand<T, TBehavior>
    public class ChartGotMouseCaptureCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartGotMouseCaptureCommandBehavior<T>, new()
    { }

    // ChartGotMouseCaptureCommandBehavior<TReturn>
    public class ChartGotMouseCaptureCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, MouseEventArgs>
    {
        public ChartGotMouseCaptureCommandBehavior(Func<object, MouseEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartGotMouseCaptureCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.GotMouseCapture += OnEventRaised;
        }
    }

    // ChartGotMouseCaptureCommand
    public class ChartGotMouseCaptureCommand : ChartCommandBase<ChartGotMouseCaptureCommandBehavior>
    { }

    // ChartGotMouseCaptureCommandBehavior
    public class ChartGotMouseCaptureCommandBehavior : ChartGotMouseCaptureCommandBehavior<object>
    { }

    // ChartGotMouseCaptureCommandWithEventArgs	
    public class ChartGotMouseCaptureCommandWithEventArgs : ChartGotMouseCaptureCommand<MouseEventArgs, ChartGotMouseCaptureCommandBehaviorWithEventArgs>
    { }

    // ChartGotMouseCaptureCommandBehaviorWithEventArgs
    public class ChartGotMouseCaptureCommandBehaviorWithEventArgs : ChartGotMouseCaptureCommandBehavior<MouseEventArgs>
    {
        public ChartGotMouseCaptureCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartLostMouseCapture
    // ChartLostMouseCaptureCommand<T, TBehavior>
    public class ChartLostMouseCaptureCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartLostMouseCaptureCommandBehavior<T>, new()
    { }

    // ChartLostMouseCaptureCommandBehavior<TReturn>
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

    // ChartLostMouseCaptureCommand
    public class ChartLostMouseCaptureCommand : ChartCommandBase<ChartLostMouseCaptureCommandBehavior>
    { }

    // ChartLostMouseCaptureCommandBehavior
    public class ChartLostMouseCaptureCommandBehavior : ChartLostMouseCaptureCommandBehavior<object>
    { }

    // ChartLostMouseCaptureCommandWithEventArgs	
    public class ChartLostMouseCaptureCommandWithEventArgs : ChartLostMouseCaptureCommand<MouseEventArgs, ChartLostMouseCaptureCommandBehaviorWithEventArgs>
    { }

    // ChartLostMouseCaptureCommandBehaviorWithEventArgs
    public class ChartLostMouseCaptureCommandBehaviorWithEventArgs : ChartLostMouseCaptureCommandBehavior<MouseEventArgs>
    {
        public ChartLostMouseCaptureCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartQueryCursor
    // ChartQueryCursorCommand<T, TBehavior>
    public class ChartQueryCursorCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartQueryCursorCommandBehavior<T>, new()
    { }

    // ChartQueryCursorCommandBehavior<TReturn>
    public class ChartQueryCursorCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, QueryCursorEventArgs>
    {
        public ChartQueryCursorCommandBehavior(Func<object, QueryCursorEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartQueryCursorCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.QueryCursor += OnEventRaised;
        }
    }

    // ChartQueryCursorCommand
    public class ChartQueryCursorCommand : ChartCommandBase<ChartQueryCursorCommandBehavior>
    { }

    // ChartQueryCursorCommandBehavior
    public class ChartQueryCursorCommandBehavior : ChartQueryCursorCommandBehavior<object>
    { }

    // ChartQueryCursorCommandWithEventArgs	
    public class ChartQueryCursorCommandWithEventArgs : ChartQueryCursorCommand<QueryCursorEventArgs, ChartQueryCursorCommandBehaviorWithEventArgs>
    { }

    // ChartQueryCursorCommandBehaviorWithEventArgs
    public class ChartQueryCursorCommandBehaviorWithEventArgs : ChartQueryCursorCommandBehavior<QueryCursorEventArgs>
    {
        public ChartQueryCursorCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartPreviewStylusDown
    // ChartPreviewStylusDownCommand<T, TBehavior>
    public class ChartPreviewStylusDownCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartPreviewStylusDownCommandBehavior<T>, new()
    { }

    // ChartPreviewStylusDownCommandBehavior<TReturn>
    public class ChartPreviewStylusDownCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, StylusDownEventArgs>
    {
        public ChartPreviewStylusDownCommandBehavior(Func<object, StylusDownEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartPreviewStylusDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewStylusDown += OnEventRaised;
        }
    }

    // ChartPreviewStylusDownCommand
    public class ChartPreviewStylusDownCommand : ChartCommandBase<ChartPreviewStylusDownCommandBehavior>
    { }

    // ChartPreviewStylusDownCommandBehavior
    public class ChartPreviewStylusDownCommandBehavior : ChartPreviewStylusDownCommandBehavior<object>
    { }

    // ChartPreviewStylusDownCommandWithEventArgs	
    public class ChartPreviewStylusDownCommandWithEventArgs : ChartPreviewStylusDownCommand<StylusDownEventArgs, ChartPreviewStylusDownCommandBehaviorWithEventArgs>
    { }

    // ChartPreviewStylusDownCommandBehaviorWithEventArgs
    public class ChartPreviewStylusDownCommandBehaviorWithEventArgs : ChartPreviewStylusDownCommandBehavior<StylusDownEventArgs>
    {
        public ChartPreviewStylusDownCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartStylusDown
    // ChartStylusDownCommand<T, TBehavior>
    public class ChartStylusDownCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartStylusDownCommandBehavior<T>, new()
    { }

    // ChartStylusDownCommandBehavior<TReturn>
    public class ChartStylusDownCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, StylusDownEventArgs>
    {
        public ChartStylusDownCommandBehavior(Func<object, StylusDownEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartStylusDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusDown += OnEventRaised;
        }
    }

    // ChartStylusDownCommand
    public class ChartStylusDownCommand : ChartCommandBase<ChartStylusDownCommandBehavior>
    { }

    // ChartStylusDownCommandBehavior
    public class ChartStylusDownCommandBehavior : ChartStylusDownCommandBehavior<object>
    { }

    // ChartStylusDownCommandWithEventArgs	
    public class ChartStylusDownCommandWithEventArgs : ChartStylusDownCommand<StylusDownEventArgs, ChartStylusDownCommandBehaviorWithEventArgs>
    { }

    // ChartStylusDownCommandBehaviorWithEventArgs
    public class ChartStylusDownCommandBehaviorWithEventArgs : ChartStylusDownCommandBehavior<StylusDownEventArgs>
    {
        public ChartStylusDownCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartPreviewStylusUp
    // ChartPreviewStylusUpCommand<T, TBehavior>
    public class ChartPreviewStylusUpCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartPreviewStylusUpCommandBehavior<T>, new()
    { }

    // ChartPreviewStylusUpCommandBehavior<TReturn>
    public class ChartPreviewStylusUpCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public ChartPreviewStylusUpCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartPreviewStylusUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewStylusUp += OnEventRaised;
        }
    }

    // ChartPreviewStylusUpCommand
    public class ChartPreviewStylusUpCommand : ChartCommandBase<ChartPreviewStylusUpCommandBehavior>
    { }

    // ChartPreviewStylusUpCommandBehavior
    public class ChartPreviewStylusUpCommandBehavior : ChartPreviewStylusUpCommandBehavior<object>
    { }

    // ChartPreviewStylusUpCommandWithEventArgs	
    public class ChartPreviewStylusUpCommandWithEventArgs : ChartPreviewStylusUpCommand<StylusEventArgs, ChartPreviewStylusUpCommandBehaviorWithEventArgs>
    { }

    // ChartPreviewStylusUpCommandBehaviorWithEventArgs
    public class ChartPreviewStylusUpCommandBehaviorWithEventArgs : ChartPreviewStylusUpCommandBehavior<StylusEventArgs>
    {
        public ChartPreviewStylusUpCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartStylusUp
    // ChartStylusUpCommand<T, TBehavior>
    public class ChartStylusUpCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartStylusUpCommandBehavior<T>, new()
    { }

    // ChartStylusUpCommandBehavior<TReturn>
    public class ChartStylusUpCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public ChartStylusUpCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartStylusUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusUp += OnEventRaised;
        }
    }

    // ChartStylusUpCommand
    public class ChartStylusUpCommand : ChartCommandBase<ChartStylusUpCommandBehavior>
    { }

    // ChartStylusUpCommandBehavior
    public class ChartStylusUpCommandBehavior : ChartStylusUpCommandBehavior<object>
    { }

    // ChartStylusUpCommandWithEventArgs	
    public class ChartStylusUpCommandWithEventArgs : ChartStylusUpCommand<StylusEventArgs, ChartStylusUpCommandBehaviorWithEventArgs>
    { }

    // ChartStylusUpCommandBehaviorWithEventArgs
    public class ChartStylusUpCommandBehaviorWithEventArgs : ChartStylusUpCommandBehavior<StylusEventArgs>
    {
        public ChartStylusUpCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartPreviewStylusMove
    // ChartPreviewStylusMoveCommand<T, TBehavior>
    public class ChartPreviewStylusMoveCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartPreviewStylusMoveCommandBehavior<T>, new()
    { }

    // ChartPreviewStylusMoveCommandBehavior<TReturn>
    public class ChartPreviewStylusMoveCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public ChartPreviewStylusMoveCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartPreviewStylusMoveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewStylusMove += OnEventRaised;
        }
    }

    // ChartPreviewStylusMoveCommand
    public class ChartPreviewStylusMoveCommand : ChartCommandBase<ChartPreviewStylusMoveCommandBehavior>
    { }

    // ChartPreviewStylusMoveCommandBehavior
    public class ChartPreviewStylusMoveCommandBehavior : ChartPreviewStylusMoveCommandBehavior<object>
    { }

    // ChartPreviewStylusMoveCommandWithEventArgs	
    public class ChartPreviewStylusMoveCommandWithEventArgs : ChartPreviewStylusMoveCommand<StylusEventArgs, ChartPreviewStylusMoveCommandBehaviorWithEventArgs>
    { }

    // ChartPreviewStylusMoveCommandBehaviorWithEventArgs
    public class ChartPreviewStylusMoveCommandBehaviorWithEventArgs : ChartPreviewStylusMoveCommandBehavior<StylusEventArgs>
    {
        public ChartPreviewStylusMoveCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartStylusMove
    // ChartStylusMoveCommand<T, TBehavior>
    public class ChartStylusMoveCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartStylusMoveCommandBehavior<T>, new()
    { }

    // ChartStylusMoveCommandBehavior<TReturn>
    public class ChartStylusMoveCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public ChartStylusMoveCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartStylusMoveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusMove += OnEventRaised;
        }
    }

    // ChartStylusMoveCommand
    public class ChartStylusMoveCommand : ChartCommandBase<ChartStylusMoveCommandBehavior>
    { }

    // ChartStylusMoveCommandBehavior
    public class ChartStylusMoveCommandBehavior : ChartStylusMoveCommandBehavior<object>
    { }

    // ChartStylusMoveCommandWithEventArgs	
    public class ChartStylusMoveCommandWithEventArgs : ChartStylusMoveCommand<StylusEventArgs, ChartStylusMoveCommandBehaviorWithEventArgs>
    { }

    // ChartStylusMoveCommandBehaviorWithEventArgs
    public class ChartStylusMoveCommandBehaviorWithEventArgs : ChartStylusMoveCommandBehavior<StylusEventArgs>
    {
        public ChartStylusMoveCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartPreviewStylusInAirMove
    // ChartPreviewStylusInAirMoveCommand<T, TBehavior>
    public class ChartPreviewStylusInAirMoveCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartPreviewStylusInAirMoveCommandBehavior<T>, new()
    { }

    // ChartPreviewStylusInAirMoveCommandBehavior<TReturn>
    public class ChartPreviewStylusInAirMoveCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public ChartPreviewStylusInAirMoveCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartPreviewStylusInAirMoveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewStylusInAirMove += OnEventRaised;
        }
    }

    // ChartPreviewStylusInAirMoveCommand
    public class ChartPreviewStylusInAirMoveCommand : ChartCommandBase<ChartPreviewStylusInAirMoveCommandBehavior>
    { }

    // ChartPreviewStylusInAirMoveCommandBehavior
    public class ChartPreviewStylusInAirMoveCommandBehavior : ChartPreviewStylusInAirMoveCommandBehavior<object>
    { }

    // ChartPreviewStylusInAirMoveCommandWithEventArgs	
    public class ChartPreviewStylusInAirMoveCommandWithEventArgs : ChartPreviewStylusInAirMoveCommand<StylusEventArgs, ChartPreviewStylusInAirMoveCommandBehaviorWithEventArgs>
    { }

    // ChartPreviewStylusInAirMoveCommandBehaviorWithEventArgs
    public class ChartPreviewStylusInAirMoveCommandBehaviorWithEventArgs : ChartPreviewStylusInAirMoveCommandBehavior<StylusEventArgs>
    {
        public ChartPreviewStylusInAirMoveCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartStylusInAirMove
    // ChartStylusInAirMoveCommand<T, TBehavior>
    public class ChartStylusInAirMoveCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartStylusInAirMoveCommandBehavior<T>, new()
    { }

    // ChartStylusInAirMoveCommandBehavior<TReturn>
    public class ChartStylusInAirMoveCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public ChartStylusInAirMoveCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartStylusInAirMoveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusInAirMove += OnEventRaised;
        }
    }

    // ChartStylusInAirMoveCommand
    public class ChartStylusInAirMoveCommand : ChartCommandBase<ChartStylusInAirMoveCommandBehavior>
    { }

    // ChartStylusInAirMoveCommandBehavior
    public class ChartStylusInAirMoveCommandBehavior : ChartStylusInAirMoveCommandBehavior<object>
    { }

    // ChartStylusInAirMoveCommandWithEventArgs	
    public class ChartStylusInAirMoveCommandWithEventArgs : ChartStylusInAirMoveCommand<StylusEventArgs, ChartStylusInAirMoveCommandBehaviorWithEventArgs>
    { }

    // ChartStylusInAirMoveCommandBehaviorWithEventArgs
    public class ChartStylusInAirMoveCommandBehaviorWithEventArgs : ChartStylusInAirMoveCommandBehavior<StylusEventArgs>
    {
        public ChartStylusInAirMoveCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartStylusEnter
    // ChartStylusEnterCommand<T, TBehavior>
    public class ChartStylusEnterCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartStylusEnterCommandBehavior<T>, new()
    { }

    // ChartStylusEnterCommandBehavior<TReturn>
    public class ChartStylusEnterCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public ChartStylusEnterCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartStylusEnterCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusEnter += OnEventRaised;
        }
    }

    // ChartStylusEnterCommand
    public class ChartStylusEnterCommand : ChartCommandBase<ChartStylusEnterCommandBehavior>
    { }

    // ChartStylusEnterCommandBehavior
    public class ChartStylusEnterCommandBehavior : ChartStylusEnterCommandBehavior<object>
    { }

    // ChartStylusEnterCommandWithEventArgs	
    public class ChartStylusEnterCommandWithEventArgs : ChartStylusEnterCommand<StylusEventArgs, ChartStylusEnterCommandBehaviorWithEventArgs>
    { }

    // ChartStylusEnterCommandBehaviorWithEventArgs
    public class ChartStylusEnterCommandBehaviorWithEventArgs : ChartStylusEnterCommandBehavior<StylusEventArgs>
    {
        public ChartStylusEnterCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartStylusLeave
    // ChartStylusLeaveCommand<T, TBehavior>
    public class ChartStylusLeaveCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartStylusLeaveCommandBehavior<T>, new()
    { }

    // ChartStylusLeaveCommandBehavior<TReturn>
    public class ChartStylusLeaveCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public ChartStylusLeaveCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartStylusLeaveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusLeave += OnEventRaised;
        }
    }

    // ChartStylusLeaveCommand
    public class ChartStylusLeaveCommand : ChartCommandBase<ChartStylusLeaveCommandBehavior>
    { }

    // ChartStylusLeaveCommandBehavior
    public class ChartStylusLeaveCommandBehavior : ChartStylusLeaveCommandBehavior<object>
    { }

    // ChartStylusLeaveCommandWithEventArgs	
    public class ChartStylusLeaveCommandWithEventArgs : ChartStylusLeaveCommand<StylusEventArgs, ChartStylusLeaveCommandBehaviorWithEventArgs>
    { }

    // ChartStylusLeaveCommandBehaviorWithEventArgs
    public class ChartStylusLeaveCommandBehaviorWithEventArgs : ChartStylusLeaveCommandBehavior<StylusEventArgs>
    {
        public ChartStylusLeaveCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartPreviewStylusInRange
    // ChartPreviewStylusInRangeCommand<T, TBehavior>
    public class ChartPreviewStylusInRangeCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartPreviewStylusInRangeCommandBehavior<T>, new()
    { }

    // ChartPreviewStylusInRangeCommandBehavior<TReturn>
    public class ChartPreviewStylusInRangeCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public ChartPreviewStylusInRangeCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartPreviewStylusInRangeCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewStylusInRange += OnEventRaised;
        }
    }

    // ChartPreviewStylusInRangeCommand
    public class ChartPreviewStylusInRangeCommand : ChartCommandBase<ChartPreviewStylusInRangeCommandBehavior>
    { }

    // ChartPreviewStylusInRangeCommandBehavior
    public class ChartPreviewStylusInRangeCommandBehavior : ChartPreviewStylusInRangeCommandBehavior<object>
    { }

    // ChartPreviewStylusInRangeCommandWithEventArgs	
    public class ChartPreviewStylusInRangeCommandWithEventArgs : ChartPreviewStylusInRangeCommand<StylusEventArgs, ChartPreviewStylusInRangeCommandBehaviorWithEventArgs>
    { }

    // ChartPreviewStylusInRangeCommandBehaviorWithEventArgs
    public class ChartPreviewStylusInRangeCommandBehaviorWithEventArgs : ChartPreviewStylusInRangeCommandBehavior<StylusEventArgs>
    {
        public ChartPreviewStylusInRangeCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartStylusInRange
    // ChartStylusInRangeCommand<T, TBehavior>
    public class ChartStylusInRangeCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartStylusInRangeCommandBehavior<T>, new()
    { }

    // ChartStylusInRangeCommandBehavior<TReturn>
    public class ChartStylusInRangeCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public ChartStylusInRangeCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartStylusInRangeCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusInRange += OnEventRaised;
        }
    }

    // ChartStylusInRangeCommand
    public class ChartStylusInRangeCommand : ChartCommandBase<ChartStylusInRangeCommandBehavior>
    { }

    // ChartStylusInRangeCommandBehavior
    public class ChartStylusInRangeCommandBehavior : ChartStylusInRangeCommandBehavior<object>
    { }

    // ChartStylusInRangeCommandWithEventArgs	
    public class ChartStylusInRangeCommandWithEventArgs : ChartStylusInRangeCommand<StylusEventArgs, ChartStylusInRangeCommandBehaviorWithEventArgs>
    { }

    // ChartStylusInRangeCommandBehaviorWithEventArgs
    public class ChartStylusInRangeCommandBehaviorWithEventArgs : ChartStylusInRangeCommandBehavior<StylusEventArgs>
    {
        public ChartStylusInRangeCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartPreviewStylusOutOfRange
    // ChartPreviewStylusOutOfRangeCommand<T, TBehavior>
    public class ChartPreviewStylusOutOfRangeCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartPreviewStylusOutOfRangeCommandBehavior<T>, new()
    { }

    // ChartPreviewStylusOutOfRangeCommandBehavior<TReturn>
    public class ChartPreviewStylusOutOfRangeCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public ChartPreviewStylusOutOfRangeCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartPreviewStylusOutOfRangeCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewStylusOutOfRange += OnEventRaised;
        }
    }

    // ChartPreviewStylusOutOfRangeCommand
    public class ChartPreviewStylusOutOfRangeCommand : ChartCommandBase<ChartPreviewStylusOutOfRangeCommandBehavior>
    { }

    // ChartPreviewStylusOutOfRangeCommandBehavior
    public class ChartPreviewStylusOutOfRangeCommandBehavior : ChartPreviewStylusOutOfRangeCommandBehavior<object>
    { }

    // ChartPreviewStylusOutOfRangeCommandWithEventArgs	
    public class ChartPreviewStylusOutOfRangeCommandWithEventArgs : ChartPreviewStylusOutOfRangeCommand<StylusEventArgs, ChartPreviewStylusOutOfRangeCommandBehaviorWithEventArgs>
    { }

    // ChartPreviewStylusOutOfRangeCommandBehaviorWithEventArgs
    public class ChartPreviewStylusOutOfRangeCommandBehaviorWithEventArgs : ChartPreviewStylusOutOfRangeCommandBehavior<StylusEventArgs>
    {
        public ChartPreviewStylusOutOfRangeCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartStylusOutOfRange
    // ChartStylusOutOfRangeCommand<T, TBehavior>
    public class ChartStylusOutOfRangeCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartStylusOutOfRangeCommandBehavior<T>, new()
    { }

    // ChartStylusOutOfRangeCommandBehavior<TReturn>
    public class ChartStylusOutOfRangeCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public ChartStylusOutOfRangeCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartStylusOutOfRangeCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusOutOfRange += OnEventRaised;
        }
    }

    // ChartStylusOutOfRangeCommand
    public class ChartStylusOutOfRangeCommand : ChartCommandBase<ChartStylusOutOfRangeCommandBehavior>
    { }

    // ChartStylusOutOfRangeCommandBehavior
    public class ChartStylusOutOfRangeCommandBehavior : ChartStylusOutOfRangeCommandBehavior<object>
    { }

    // ChartStylusOutOfRangeCommandWithEventArgs	
    public class ChartStylusOutOfRangeCommandWithEventArgs : ChartStylusOutOfRangeCommand<StylusEventArgs, ChartStylusOutOfRangeCommandBehaviorWithEventArgs>
    { }

    // ChartStylusOutOfRangeCommandBehaviorWithEventArgs
    public class ChartStylusOutOfRangeCommandBehaviorWithEventArgs : ChartStylusOutOfRangeCommandBehavior<StylusEventArgs>
    {
        public ChartStylusOutOfRangeCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartPreviewStylusSystemGesture
    // ChartPreviewStylusSystemGestureCommand<T, TBehavior>
    public class ChartPreviewStylusSystemGestureCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartPreviewStylusSystemGestureCommandBehavior<T>, new()
    { }

    // ChartPreviewStylusSystemGestureCommandBehavior<TReturn>
    public class ChartPreviewStylusSystemGestureCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, StylusSystemGestureEventArgs>
    {
        public ChartPreviewStylusSystemGestureCommandBehavior(Func<object, StylusSystemGestureEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartPreviewStylusSystemGestureCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewStylusSystemGesture += OnEventRaised;
        }
    }

    // ChartPreviewStylusSystemGestureCommand
    public class ChartPreviewStylusSystemGestureCommand : ChartCommandBase<ChartPreviewStylusSystemGestureCommandBehavior>
    { }

    // ChartPreviewStylusSystemGestureCommandBehavior
    public class ChartPreviewStylusSystemGestureCommandBehavior : ChartPreviewStylusSystemGestureCommandBehavior<object>
    { }

    // ChartPreviewStylusSystemGestureCommandWithEventArgs	
    public class ChartPreviewStylusSystemGestureCommandWithEventArgs : ChartPreviewStylusSystemGestureCommand<StylusSystemGestureEventArgs, ChartPreviewStylusSystemGestureCommandBehaviorWithEventArgs>
    { }

    // ChartPreviewStylusSystemGestureCommandBehaviorWithEventArgs
    public class ChartPreviewStylusSystemGestureCommandBehaviorWithEventArgs : ChartPreviewStylusSystemGestureCommandBehavior<StylusSystemGestureEventArgs>
    {
        public ChartPreviewStylusSystemGestureCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartStylusSystemGesture
    // ChartStylusSystemGestureCommand<T, TBehavior>
    public class ChartStylusSystemGestureCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartStylusSystemGestureCommandBehavior<T>, new()
    { }

    // ChartStylusSystemGestureCommandBehavior<TReturn>
    public class ChartStylusSystemGestureCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, StylusSystemGestureEventArgs>
    {
        public ChartStylusSystemGestureCommandBehavior(Func<object, StylusSystemGestureEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartStylusSystemGestureCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusSystemGesture += OnEventRaised;
        }
    }

    // ChartStylusSystemGestureCommand
    public class ChartStylusSystemGestureCommand : ChartCommandBase<ChartStylusSystemGestureCommandBehavior>
    { }

    // ChartStylusSystemGestureCommandBehavior
    public class ChartStylusSystemGestureCommandBehavior : ChartStylusSystemGestureCommandBehavior<object>
    { }

    // ChartStylusSystemGestureCommandWithEventArgs	
    public class ChartStylusSystemGestureCommandWithEventArgs : ChartStylusSystemGestureCommand<StylusSystemGestureEventArgs, ChartStylusSystemGestureCommandBehaviorWithEventArgs>
    { }

    // ChartStylusSystemGestureCommandBehaviorWithEventArgs
    public class ChartStylusSystemGestureCommandBehaviorWithEventArgs : ChartStylusSystemGestureCommandBehavior<StylusSystemGestureEventArgs>
    {
        public ChartStylusSystemGestureCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartGotStylusCapture
    // ChartGotStylusCaptureCommand<T, TBehavior>
    public class ChartGotStylusCaptureCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartGotStylusCaptureCommandBehavior<T>, new()
    { }

    // ChartGotStylusCaptureCommandBehavior<TReturn>
    public class ChartGotStylusCaptureCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public ChartGotStylusCaptureCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartGotStylusCaptureCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.GotStylusCapture += OnEventRaised;
        }
    }

    // ChartGotStylusCaptureCommand
    public class ChartGotStylusCaptureCommand : ChartCommandBase<ChartGotStylusCaptureCommandBehavior>
    { }

    // ChartGotStylusCaptureCommandBehavior
    public class ChartGotStylusCaptureCommandBehavior : ChartGotStylusCaptureCommandBehavior<object>
    { }

    // ChartGotStylusCaptureCommandWithEventArgs	
    public class ChartGotStylusCaptureCommandWithEventArgs : ChartGotStylusCaptureCommand<StylusEventArgs, ChartGotStylusCaptureCommandBehaviorWithEventArgs>
    { }

    // ChartGotStylusCaptureCommandBehaviorWithEventArgs
    public class ChartGotStylusCaptureCommandBehaviorWithEventArgs : ChartGotStylusCaptureCommandBehavior<StylusEventArgs>
    {
        public ChartGotStylusCaptureCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartLostStylusCapture
    // ChartLostStylusCaptureCommand<T, TBehavior>
    public class ChartLostStylusCaptureCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartLostStylusCaptureCommandBehavior<T>, new()
    { }

    // ChartLostStylusCaptureCommandBehavior<TReturn>
    public class ChartLostStylusCaptureCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, StylusEventArgs>
    {
        public ChartLostStylusCaptureCommandBehavior(Func<object, StylusEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartLostStylusCaptureCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.LostStylusCapture += OnEventRaised;
        }
    }

    // ChartLostStylusCaptureCommand
    public class ChartLostStylusCaptureCommand : ChartCommandBase<ChartLostStylusCaptureCommandBehavior>
    { }

    // ChartLostStylusCaptureCommandBehavior
    public class ChartLostStylusCaptureCommandBehavior : ChartLostStylusCaptureCommandBehavior<object>
    { }

    // ChartLostStylusCaptureCommandWithEventArgs	
    public class ChartLostStylusCaptureCommandWithEventArgs : ChartLostStylusCaptureCommand<StylusEventArgs, ChartLostStylusCaptureCommandBehaviorWithEventArgs>
    { }

    // ChartLostStylusCaptureCommandBehaviorWithEventArgs
    public class ChartLostStylusCaptureCommandBehaviorWithEventArgs : ChartLostStylusCaptureCommandBehavior<StylusEventArgs>
    {
        public ChartLostStylusCaptureCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartStylusButtonDown
    // ChartStylusButtonDownCommand<T, TBehavior>
    public class ChartStylusButtonDownCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartStylusButtonDownCommandBehavior<T>, new()
    { }

    // ChartStylusButtonDownCommandBehavior<TReturn>
    public class ChartStylusButtonDownCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, StylusButtonEventArgs>
    {
        public ChartStylusButtonDownCommandBehavior(Func<object, StylusButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartStylusButtonDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusButtonDown += OnEventRaised;
        }
    }

    // ChartStylusButtonDownCommand
    public class ChartStylusButtonDownCommand : ChartCommandBase<ChartStylusButtonDownCommandBehavior>
    { }

    // ChartStylusButtonDownCommandBehavior
    public class ChartStylusButtonDownCommandBehavior : ChartStylusButtonDownCommandBehavior<object>
    { }

    // ChartStylusButtonDownCommandWithEventArgs	
    public class ChartStylusButtonDownCommandWithEventArgs : ChartStylusButtonDownCommand<StylusButtonEventArgs, ChartStylusButtonDownCommandBehaviorWithEventArgs>
    { }

    // ChartStylusButtonDownCommandBehaviorWithEventArgs
    public class ChartStylusButtonDownCommandBehaviorWithEventArgs : ChartStylusButtonDownCommandBehavior<StylusButtonEventArgs>
    {
        public ChartStylusButtonDownCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartStylusButtonUp
    // ChartStylusButtonUpCommand<T, TBehavior>
    public class ChartStylusButtonUpCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartStylusButtonUpCommandBehavior<T>, new()
    { }

    // ChartStylusButtonUpCommandBehavior<TReturn>
    public class ChartStylusButtonUpCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, StylusButtonEventArgs>
    {
        public ChartStylusButtonUpCommandBehavior(Func<object, StylusButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartStylusButtonUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StylusButtonUp += OnEventRaised;
        }
    }

    // ChartStylusButtonUpCommand
    public class ChartStylusButtonUpCommand : ChartCommandBase<ChartStylusButtonUpCommandBehavior>
    { }

    // ChartStylusButtonUpCommandBehavior
    public class ChartStylusButtonUpCommandBehavior : ChartStylusButtonUpCommandBehavior<object>
    { }

    // ChartStylusButtonUpCommandWithEventArgs	
    public class ChartStylusButtonUpCommandWithEventArgs : ChartStylusButtonUpCommand<StylusButtonEventArgs, ChartStylusButtonUpCommandBehaviorWithEventArgs>
    { }

    // ChartStylusButtonUpCommandBehaviorWithEventArgs
    public class ChartStylusButtonUpCommandBehaviorWithEventArgs : ChartStylusButtonUpCommandBehavior<StylusButtonEventArgs>
    {
        public ChartStylusButtonUpCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartPreviewStylusButtonDown
    // ChartPreviewStylusButtonDownCommand<T, TBehavior>
    public class ChartPreviewStylusButtonDownCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartPreviewStylusButtonDownCommandBehavior<T>, new()
    { }

    // ChartPreviewStylusButtonDownCommandBehavior<TReturn>
    public class ChartPreviewStylusButtonDownCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, StylusButtonEventArgs>
    {
        public ChartPreviewStylusButtonDownCommandBehavior(Func<object, StylusButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartPreviewStylusButtonDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewStylusButtonDown += OnEventRaised;
        }
    }

    // ChartPreviewStylusButtonDownCommand
    public class ChartPreviewStylusButtonDownCommand : ChartCommandBase<ChartPreviewStylusButtonDownCommandBehavior>
    { }

    // ChartPreviewStylusButtonDownCommandBehavior
    public class ChartPreviewStylusButtonDownCommandBehavior : ChartPreviewStylusButtonDownCommandBehavior<object>
    { }

    // ChartPreviewStylusButtonDownCommandWithEventArgs	
    public class ChartPreviewStylusButtonDownCommandWithEventArgs : ChartPreviewStylusButtonDownCommand<StylusButtonEventArgs, ChartPreviewStylusButtonDownCommandBehaviorWithEventArgs>
    { }

    // ChartPreviewStylusButtonDownCommandBehaviorWithEventArgs
    public class ChartPreviewStylusButtonDownCommandBehaviorWithEventArgs : ChartPreviewStylusButtonDownCommandBehavior<StylusButtonEventArgs>
    {
        public ChartPreviewStylusButtonDownCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartPreviewStylusButtonUp
    // ChartPreviewStylusButtonUpCommand<T, TBehavior>
    public class ChartPreviewStylusButtonUpCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartPreviewStylusButtonUpCommandBehavior<T>, new()
    { }

    // ChartPreviewStylusButtonUpCommandBehavior<TReturn>
    public class ChartPreviewStylusButtonUpCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, StylusButtonEventArgs>
    {
        public ChartPreviewStylusButtonUpCommandBehavior(Func<object, StylusButtonEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartPreviewStylusButtonUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewStylusButtonUp += OnEventRaised;
        }
    }

    // ChartPreviewStylusButtonUpCommand
    public class ChartPreviewStylusButtonUpCommand : ChartCommandBase<ChartPreviewStylusButtonUpCommandBehavior>
    { }

    // ChartPreviewStylusButtonUpCommandBehavior
    public class ChartPreviewStylusButtonUpCommandBehavior : ChartPreviewStylusButtonUpCommandBehavior<object>
    { }

    // ChartPreviewStylusButtonUpCommandWithEventArgs	
    public class ChartPreviewStylusButtonUpCommandWithEventArgs : ChartPreviewStylusButtonUpCommand<StylusButtonEventArgs, ChartPreviewStylusButtonUpCommandBehaviorWithEventArgs>
    { }

    // ChartPreviewStylusButtonUpCommandBehaviorWithEventArgs
    public class ChartPreviewStylusButtonUpCommandBehaviorWithEventArgs : ChartPreviewStylusButtonUpCommandBehavior<StylusButtonEventArgs>
    {
        public ChartPreviewStylusButtonUpCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartPreviewKeyDown
    // ChartPreviewKeyDownCommand<T, TBehavior>
    public class ChartPreviewKeyDownCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartPreviewKeyDownCommandBehavior<T>, new()
    { }

    // ChartPreviewKeyDownCommandBehavior<TReturn>
    public class ChartPreviewKeyDownCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, KeyEventArgs>
    {
        public ChartPreviewKeyDownCommandBehavior(Func<object, KeyEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartPreviewKeyDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewKeyDown += OnEventRaised;
        }
        
    }

    // ChartPreviewKeyDownCommand
    public class ChartPreviewKeyDownCommand : ChartCommandBase<ChartPreviewKeyDownCommandBehavior>
    { }

    // ChartPreviewKeyDownCommandBehavior
    public class ChartPreviewKeyDownCommandBehavior : ChartPreviewKeyDownCommandBehavior<object>
    { }

    // ChartPreviewKeyDownCommandWithEventArgs	
    public class ChartPreviewKeyDownCommandWithEventArgs : ChartPreviewKeyDownCommand<KeyEventArgs, ChartPreviewKeyDownCommandBehaviorWithEventArgs>
    { }

    // ChartPreviewKeyDownCommandBehaviorWithEventArgs
    public class ChartPreviewKeyDownCommandBehaviorWithEventArgs : ChartPreviewKeyDownCommandBehavior<KeyEventArgs>
    {
        public ChartPreviewKeyDownCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartKeyDown
    // ChartKeyDownCommand<T, TBehavior>
    public class ChartKeyDownCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartKeyDownCommandBehavior<T>, new()
    { }

    // ChartKeyDownCommandBehavior<TReturn>
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

    // ChartKeyDownCommand
    public class ChartKeyDownCommand : ChartCommandBase<ChartKeyDownCommandBehavior>
    { }

    // ChartKeyDownCommandBehavior
    public class ChartKeyDownCommandBehavior : ChartKeyDownCommandBehavior<object>
    { }

    // ChartKeyDownCommandWithEventArgs	
    public class ChartKeyDownCommandWithEventArgs : ChartKeyDownCommand<KeyEventArgs, ChartKeyDownCommandBehaviorWithEventArgs>
    { }

    // ChartKeyDownCommandBehaviorWithEventArgs
    public class ChartKeyDownCommandBehaviorWithEventArgs : ChartKeyDownCommandBehavior<KeyEventArgs>
    {
        public ChartKeyDownCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartPreviewKeyUp
    // ChartPreviewKeyUpCommand<T, TBehavior>
    public class ChartPreviewKeyUpCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartPreviewKeyUpCommandBehavior<T>, new()
    { }

    // ChartPreviewKeyUpCommandBehavior<TReturn>
    public class ChartPreviewKeyUpCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, KeyEventArgs>
    {
        public ChartPreviewKeyUpCommandBehavior(Func<object, KeyEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartPreviewKeyUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewKeyUp += OnEventRaised;
        }
    }

    // ChartPreviewKeyUpCommand
    public class ChartPreviewKeyUpCommand : ChartCommandBase<ChartPreviewKeyUpCommandBehavior>
    { }

    // ChartPreviewKeyUpCommandBehavior
    public class ChartPreviewKeyUpCommandBehavior : ChartPreviewKeyUpCommandBehavior<object>
    { }

    // ChartPreviewKeyUpCommandWithEventArgs	
    public class ChartPreviewKeyUpCommandWithEventArgs : ChartPreviewKeyUpCommand<KeyEventArgs, ChartPreviewKeyUpCommandBehaviorWithEventArgs>
    { }

    // ChartPreviewKeyUpCommandBehaviorWithEventArgs
    public class ChartPreviewKeyUpCommandBehaviorWithEventArgs : ChartPreviewKeyUpCommandBehavior<KeyEventArgs>
    {
        public ChartPreviewKeyUpCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartKeyUp
    // ChartKeyUpCommand<T, TBehavior>
    public class ChartKeyUpCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartKeyUpCommandBehavior<T>, new()
    { }

    // ChartKeyUpCommandBehavior<TReturn>
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

    // ChartKeyUpCommand
    public class ChartKeyUpCommand : ChartCommandBase<ChartKeyUpCommandBehavior>
    { }

    // ChartKeyUpCommandBehavior
    public class ChartKeyUpCommandBehavior : ChartKeyUpCommandBehavior<object>
    { }

    // ChartKeyUpCommandWithEventArgs	
    public class ChartKeyUpCommandWithEventArgs : ChartKeyUpCommand<KeyEventArgs, ChartKeyUpCommandBehaviorWithEventArgs>
    { }

    // ChartKeyUpCommandBehaviorWithEventArgs
    public class ChartKeyUpCommandBehaviorWithEventArgs : ChartKeyUpCommandBehavior<KeyEventArgs>
    {
        public ChartKeyUpCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartPreviewGotKeyboardFocus
    // ChartPreviewGotKeyboardFocusCommand<T, TBehavior>
    public class ChartPreviewGotKeyboardFocusCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartPreviewGotKeyboardFocusCommandBehavior<T>, new()
    { }

    // ChartPreviewGotKeyboardFocusCommandBehavior<TReturn>
    public class ChartPreviewGotKeyboardFocusCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, KeyboardFocusChangedEventArgs>
    {
        public ChartPreviewGotKeyboardFocusCommandBehavior(Func<object, KeyboardFocusChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartPreviewGotKeyboardFocusCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewGotKeyboardFocus += OnEventRaised;
        }
    }

    // ChartPreviewGotKeyboardFocusCommand
    public class ChartPreviewGotKeyboardFocusCommand : ChartCommandBase<ChartPreviewGotKeyboardFocusCommandBehavior>
    { }

    // ChartPreviewGotKeyboardFocusCommandBehavior
    public class ChartPreviewGotKeyboardFocusCommandBehavior : ChartPreviewGotKeyboardFocusCommandBehavior<object>
    { }

    // ChartPreviewGotKeyboardFocusCommandWithEventArgs	
    public class ChartPreviewGotKeyboardFocusCommandWithEventArgs : ChartPreviewGotKeyboardFocusCommand<KeyboardFocusChangedEventArgs, ChartPreviewGotKeyboardFocusCommandBehaviorWithEventArgs>
    { }

    // ChartPreviewGotKeyboardFocusCommandBehaviorWithEventArgs
    public class ChartPreviewGotKeyboardFocusCommandBehaviorWithEventArgs : ChartPreviewGotKeyboardFocusCommandBehavior<KeyboardFocusChangedEventArgs>
    {
        public ChartPreviewGotKeyboardFocusCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartGotKeyboardFocus
    // ChartGotKeyboardFocusCommand<T, TBehavior>
    public class ChartGotKeyboardFocusCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartGotKeyboardFocusCommandBehavior<T>, new()
    { }

    // ChartGotKeyboardFocusCommandBehavior<TReturn>
    public class ChartGotKeyboardFocusCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, KeyboardFocusChangedEventArgs>
    {
        public ChartGotKeyboardFocusCommandBehavior(Func<object, KeyboardFocusChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartGotKeyboardFocusCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.GotKeyboardFocus += OnEventRaised;
        }
    }

    // ChartGotKeyboardFocusCommand
    public class ChartGotKeyboardFocusCommand : ChartCommandBase<ChartGotKeyboardFocusCommandBehavior>
    { }

    // ChartGotKeyboardFocusCommandBehavior
    public class ChartGotKeyboardFocusCommandBehavior : ChartGotKeyboardFocusCommandBehavior<object>
    { }

    // ChartGotKeyboardFocusCommandWithEventArgs	
    public class ChartGotKeyboardFocusCommandWithEventArgs : ChartGotKeyboardFocusCommand<KeyboardFocusChangedEventArgs, ChartGotKeyboardFocusCommandBehaviorWithEventArgs>
    { }

    // ChartGotKeyboardFocusCommandBehaviorWithEventArgs
    public class ChartGotKeyboardFocusCommandBehaviorWithEventArgs : ChartGotKeyboardFocusCommandBehavior<KeyboardFocusChangedEventArgs>
    {
        public ChartGotKeyboardFocusCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartPreviewLostKeyboardFocus
    // ChartPreviewLostKeyboardFocusCommand<T, TBehavior>
    public class ChartPreviewLostKeyboardFocusCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartPreviewLostKeyboardFocusCommandBehavior<T>, new()
    { }

    // ChartPreviewLostKeyboardFocusCommandBehavior<TReturn>
    public class ChartPreviewLostKeyboardFocusCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, KeyboardFocusChangedEventArgs>
    {
        public ChartPreviewLostKeyboardFocusCommandBehavior(Func<object, KeyboardFocusChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartPreviewLostKeyboardFocusCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewLostKeyboardFocus += OnEventRaised;
        }
    }

    // ChartPreviewLostKeyboardFocusCommand
    public class ChartPreviewLostKeyboardFocusCommand : ChartCommandBase<ChartPreviewLostKeyboardFocusCommandBehavior>
    { }

    // ChartPreviewLostKeyboardFocusCommandBehavior
    public class ChartPreviewLostKeyboardFocusCommandBehavior : ChartPreviewLostKeyboardFocusCommandBehavior<object>
    { }

    // ChartPreviewLostKeyboardFocusCommandWithEventArgs	
    public class ChartPreviewLostKeyboardFocusCommandWithEventArgs : ChartPreviewLostKeyboardFocusCommand<KeyboardFocusChangedEventArgs, ChartPreviewLostKeyboardFocusCommandBehaviorWithEventArgs>
    { }

    // ChartPreviewLostKeyboardFocusCommandBehaviorWithEventArgs
    public class ChartPreviewLostKeyboardFocusCommandBehaviorWithEventArgs : ChartPreviewLostKeyboardFocusCommandBehavior<KeyboardFocusChangedEventArgs>
    {
        public ChartPreviewLostKeyboardFocusCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartLostKeyboardFocus
    // ChartLostKeyboardFocusCommand<T, TBehavior>
    public class ChartLostKeyboardFocusCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartLostKeyboardFocusCommandBehavior<T>, new()
    { }

    // ChartLostKeyboardFocusCommandBehavior<TReturn>
    public class ChartLostKeyboardFocusCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, KeyboardFocusChangedEventArgs>
    {
        public ChartLostKeyboardFocusCommandBehavior(Func<object, KeyboardFocusChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartLostKeyboardFocusCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.LostKeyboardFocus += OnEventRaised;
        }
    }

    // ChartLostKeyboardFocusCommand
    public class ChartLostKeyboardFocusCommand : ChartCommandBase<ChartLostKeyboardFocusCommandBehavior>
    { }

    // ChartLostKeyboardFocusCommandBehavior
    public class ChartLostKeyboardFocusCommandBehavior : ChartLostKeyboardFocusCommandBehavior<object>
    { }

    // ChartLostKeyboardFocusCommandWithEventArgs	
    public class ChartLostKeyboardFocusCommandWithEventArgs : ChartLostKeyboardFocusCommand<KeyboardFocusChangedEventArgs, ChartLostKeyboardFocusCommandBehaviorWithEventArgs>
    { }

    // ChartLostKeyboardFocusCommandBehaviorWithEventArgs
    public class ChartLostKeyboardFocusCommandBehaviorWithEventArgs : ChartLostKeyboardFocusCommandBehavior<KeyboardFocusChangedEventArgs>
    {
        public ChartLostKeyboardFocusCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartPreviewTextInput
    // ChartPreviewTextInputCommand<T, TBehavior>
    public class ChartPreviewTextInputCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartPreviewTextInputCommandBehavior<T>, new()
    { }

    // ChartPreviewTextInputCommandBehavior<TReturn>
    public class ChartPreviewTextInputCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, TextCompositionEventArgs>
    {
        public ChartPreviewTextInputCommandBehavior(Func<object, TextCompositionEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartPreviewTextInputCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewTextInput += OnEventRaised;
        }
    }

    // ChartPreviewTextInputCommand
    public class ChartPreviewTextInputCommand : ChartCommandBase<ChartPreviewTextInputCommandBehavior>
    { }

    // ChartPreviewTextInputCommandBehavior
    public class ChartPreviewTextInputCommandBehavior : ChartPreviewTextInputCommandBehavior<object>
    { }

    // ChartPreviewTextInputCommandWithEventArgs	
    public class ChartPreviewTextInputCommandWithEventArgs : ChartPreviewTextInputCommand<TextCompositionEventArgs, ChartPreviewTextInputCommandBehaviorWithEventArgs>
    { }

    // ChartPreviewTextInputCommandBehaviorWithEventArgs
    public class ChartPreviewTextInputCommandBehaviorWithEventArgs : ChartPreviewTextInputCommandBehavior<TextCompositionEventArgs>
    {
        public ChartPreviewTextInputCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartTextInput
    // ChartTextInputCommand<T, TBehavior>
    public class ChartTextInputCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartTextInputCommandBehavior<T>, new()
    { }

    // ChartTextInputCommandBehavior<TReturn>
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

    // ChartTextInputCommand
    public class ChartTextInputCommand : ChartCommandBase<ChartTextInputCommandBehavior>
    { }

    // ChartTextInputCommandBehavior
    public class ChartTextInputCommandBehavior : ChartTextInputCommandBehavior<object>
    { }

    // ChartTextInputCommandWithEventArgs	
    public class ChartTextInputCommandWithEventArgs : ChartTextInputCommand<TextCompositionEventArgs, ChartTextInputCommandBehaviorWithEventArgs>
    { }

    // ChartTextInputCommandBehaviorWithEventArgs
    public class ChartTextInputCommandBehaviorWithEventArgs : ChartTextInputCommandBehavior<TextCompositionEventArgs>
    {
        public ChartTextInputCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartPreviewQueryContinueDrag
    // ChartPreviewQueryContinueDragCommand<T, TBehavior>
    public class ChartPreviewQueryContinueDragCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartPreviewQueryContinueDragCommandBehavior<T>, new()
    { }

    // ChartPreviewQueryContinueDragCommandBehavior<TReturn>
    public class ChartPreviewQueryContinueDragCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, QueryContinueDragEventArgs>
    {
        public ChartPreviewQueryContinueDragCommandBehavior(Func<object, QueryContinueDragEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartPreviewQueryContinueDragCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewQueryContinueDrag += OnEventRaised;
        }
    }

    // ChartPreviewQueryContinueDragCommand
    public class ChartPreviewQueryContinueDragCommand : ChartCommandBase<ChartPreviewQueryContinueDragCommandBehavior>
    { }

    // ChartPreviewQueryContinueDragCommandBehavior
    public class ChartPreviewQueryContinueDragCommandBehavior : ChartPreviewQueryContinueDragCommandBehavior<object>
    { }

    // ChartPreviewQueryContinueDragCommandWithEventArgs	
    public class ChartPreviewQueryContinueDragCommandWithEventArgs : ChartPreviewQueryContinueDragCommand<QueryContinueDragEventArgs, ChartPreviewQueryContinueDragCommandBehaviorWithEventArgs>
    { }

    // ChartPreviewQueryContinueDragCommandBehaviorWithEventArgs
    public class ChartPreviewQueryContinueDragCommandBehaviorWithEventArgs : ChartPreviewQueryContinueDragCommandBehavior<QueryContinueDragEventArgs>
    {
        public ChartPreviewQueryContinueDragCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartQueryContinueDrag
    // ChartQueryContinueDragCommand<T, TBehavior>
    public class ChartQueryContinueDragCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartQueryContinueDragCommandBehavior<T>, new()
    { }

    // ChartQueryContinueDragCommandBehavior<TReturn>
    public class ChartQueryContinueDragCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, QueryContinueDragEventArgs>
    {
        public ChartQueryContinueDragCommandBehavior(Func<object, QueryContinueDragEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartQueryContinueDragCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.QueryContinueDrag += OnEventRaised;
        }
    }

    // ChartQueryContinueDragCommand
    public class ChartQueryContinueDragCommand : ChartCommandBase<ChartQueryContinueDragCommandBehavior>
    { }

    // ChartQueryContinueDragCommandBehavior
    public class ChartQueryContinueDragCommandBehavior : ChartQueryContinueDragCommandBehavior<object>
    { }

    // ChartQueryContinueDragCommandWithEventArgs	
    public class ChartQueryContinueDragCommandWithEventArgs : ChartQueryContinueDragCommand<QueryContinueDragEventArgs, ChartQueryContinueDragCommandBehaviorWithEventArgs>
    { }

    // ChartQueryContinueDragCommandBehaviorWithEventArgs
    public class ChartQueryContinueDragCommandBehaviorWithEventArgs : ChartQueryContinueDragCommandBehavior<QueryContinueDragEventArgs>
    {
        public ChartQueryContinueDragCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartPreviewGiveFeedback
    // ChartPreviewGiveFeedbackCommand<T, TBehavior>
    public class ChartPreviewGiveFeedbackCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartPreviewGiveFeedbackCommandBehavior<T>, new()
    { }

    // ChartPreviewGiveFeedbackCommandBehavior<TReturn>
    public class ChartPreviewGiveFeedbackCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, GiveFeedbackEventArgs>
    {
        public ChartPreviewGiveFeedbackCommandBehavior(Func<object, GiveFeedbackEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartPreviewGiveFeedbackCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewGiveFeedback += OnEventRaised;
        }
    }

    // ChartPreviewGiveFeedbackCommand
    public class ChartPreviewGiveFeedbackCommand : ChartCommandBase<ChartPreviewGiveFeedbackCommandBehavior>
    { }

    // ChartPreviewGiveFeedbackCommandBehavior
    public class ChartPreviewGiveFeedbackCommandBehavior : ChartPreviewGiveFeedbackCommandBehavior<object>
    { }

    // ChartPreviewGiveFeedbackCommandWithEventArgs	
    public class ChartPreviewGiveFeedbackCommandWithEventArgs : ChartPreviewGiveFeedbackCommand<GiveFeedbackEventArgs, ChartPreviewGiveFeedbackCommandBehaviorWithEventArgs>
    { }

    // ChartPreviewGiveFeedbackCommandBehaviorWithEventArgs
    public class ChartPreviewGiveFeedbackCommandBehaviorWithEventArgs : ChartPreviewGiveFeedbackCommandBehavior<GiveFeedbackEventArgs>
    {
        public ChartPreviewGiveFeedbackCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartGiveFeedback
    // ChartGiveFeedbackCommand<T, TBehavior>
    public class ChartGiveFeedbackCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartGiveFeedbackCommandBehavior<T>, new()
    { }

    // ChartGiveFeedbackCommandBehavior<TReturn>
    public class ChartGiveFeedbackCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, GiveFeedbackEventArgs>
    {
        public ChartGiveFeedbackCommandBehavior(Func<object, GiveFeedbackEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartGiveFeedbackCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.GiveFeedback += OnEventRaised;
        }
    }

    // ChartGiveFeedbackCommand
    public class ChartGiveFeedbackCommand : ChartCommandBase<ChartGiveFeedbackCommandBehavior>
    { }

    // ChartGiveFeedbackCommandBehavior
    public class ChartGiveFeedbackCommandBehavior : ChartGiveFeedbackCommandBehavior<object>
    { }

    // ChartGiveFeedbackCommandWithEventArgs	
    public class ChartGiveFeedbackCommandWithEventArgs : ChartGiveFeedbackCommand<GiveFeedbackEventArgs, ChartGiveFeedbackCommandBehaviorWithEventArgs>
    { }

    // ChartGiveFeedbackCommandBehaviorWithEventArgs
    public class ChartGiveFeedbackCommandBehaviorWithEventArgs : ChartGiveFeedbackCommandBehavior<GiveFeedbackEventArgs>
    {
        public ChartGiveFeedbackCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartPreviewDragEnter
    // ChartPreviewDragEnterCommand<T, TBehavior>
    public class ChartPreviewDragEnterCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartPreviewDragEnterCommandBehavior<T>, new()
    { }

    // ChartPreviewDragEnterCommandBehavior<TReturn>
    public class ChartPreviewDragEnterCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, DragEventArgs>
    {
        public ChartPreviewDragEnterCommandBehavior(Func<object, DragEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartPreviewDragEnterCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewDragEnter += OnEventRaised;
        }
    }

    // ChartPreviewDragEnterCommand
    public class ChartPreviewDragEnterCommand : ChartCommandBase<ChartPreviewDragEnterCommandBehavior>
    { }

    // ChartPreviewDragEnterCommandBehavior
    public class ChartPreviewDragEnterCommandBehavior : ChartPreviewDragEnterCommandBehavior<object>
    { }

    // ChartPreviewDragEnterCommandWithEventArgs	
    public class ChartPreviewDragEnterCommandWithEventArgs : ChartPreviewDragEnterCommand<DragEventArgs, ChartPreviewDragEnterCommandBehaviorWithEventArgs>
    { }

    // ChartPreviewDragEnterCommandBehaviorWithEventArgs
    public class ChartPreviewDragEnterCommandBehaviorWithEventArgs : ChartPreviewDragEnterCommandBehavior<DragEventArgs>
    {
        public ChartPreviewDragEnterCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartDragEnter
    // ChartDragEnterCommand<T, TBehavior>
    public class ChartDragEnterCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartDragEnterCommandBehavior<T>, new()
    { }

    // ChartDragEnterCommandBehavior<TReturn>
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

    // ChartDragEnterCommand
    public class ChartDragEnterCommand : ChartCommandBase<ChartDragEnterCommandBehavior>
    { }

    // ChartDragEnterCommandBehavior
    public class ChartDragEnterCommandBehavior : ChartDragEnterCommandBehavior<object>
    { }

    // ChartDragEnterCommandWithEventArgs	
    public class ChartDragEnterCommandWithEventArgs : ChartDragEnterCommand<DragEventArgs, ChartDragEnterCommandBehaviorWithEventArgs>
    { }

    // ChartDragEnterCommandBehaviorWithEventArgs
    public class ChartDragEnterCommandBehaviorWithEventArgs : ChartDragEnterCommandBehavior<DragEventArgs>
    {
        public ChartDragEnterCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartPreviewDragOver
    // ChartPreviewDragOverCommand<T, TBehavior>
    public class ChartPreviewDragOverCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartPreviewDragOverCommandBehavior<T>, new()
    { }

    // ChartPreviewDragOverCommandBehavior<TReturn>
    public class ChartPreviewDragOverCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, DragEventArgs>
    {
        public ChartPreviewDragOverCommandBehavior(Func<object, DragEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartPreviewDragOverCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewDragOver += OnEventRaised;
        }
    }

    // ChartPreviewDragOverCommand
    public class ChartPreviewDragOverCommand : ChartCommandBase<ChartPreviewDragOverCommandBehavior>
    { }

    // ChartPreviewDragOverCommandBehavior
    public class ChartPreviewDragOverCommandBehavior : ChartPreviewDragOverCommandBehavior<object>
    { }

    // ChartPreviewDragOverCommandWithEventArgs	
    public class ChartPreviewDragOverCommandWithEventArgs : ChartPreviewDragOverCommand<DragEventArgs, ChartPreviewDragOverCommandBehaviorWithEventArgs>
    { }

    // ChartPreviewDragOverCommandBehaviorWithEventArgs
    public class ChartPreviewDragOverCommandBehaviorWithEventArgs : ChartPreviewDragOverCommandBehavior<DragEventArgs>
    {
        public ChartPreviewDragOverCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartDragOver
    // ChartDragOverCommand<T, TBehavior>
    public class ChartDragOverCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartDragOverCommandBehavior<T>, new()
    { }

    // ChartDragOverCommandBehavior<TReturn>
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

    // ChartDragOverCommand
    public class ChartDragOverCommand : ChartCommandBase<ChartDragOverCommandBehavior>
    { }

    // ChartDragOverCommandBehavior
    public class ChartDragOverCommandBehavior : ChartDragOverCommandBehavior<object>
    { }

    // ChartDragOverCommandWithEventArgs	
    public class ChartDragOverCommandWithEventArgs : ChartDragOverCommand<DragEventArgs, ChartDragOverCommandBehaviorWithEventArgs>
    { }

    // ChartDragOverCommandBehaviorWithEventArgs
    public class ChartDragOverCommandBehaviorWithEventArgs : ChartDragOverCommandBehavior<DragEventArgs>
    {
        public ChartDragOverCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartPreviewDragLeave
    // ChartPreviewDragLeaveCommand<T, TBehavior>
    public class ChartPreviewDragLeaveCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartPreviewDragLeaveCommandBehavior<T>, new()
    { }

    // ChartPreviewDragLeaveCommandBehavior<TReturn>
    public class ChartPreviewDragLeaveCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, DragEventArgs>
    {
        public ChartPreviewDragLeaveCommandBehavior(Func<object, DragEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartPreviewDragLeaveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewDragLeave += OnEventRaised;
        }
    }

    // ChartPreviewDragLeaveCommand
    public class ChartPreviewDragLeaveCommand : ChartCommandBase<ChartPreviewDragLeaveCommandBehavior>
    { }

    // ChartPreviewDragLeaveCommandBehavior
    public class ChartPreviewDragLeaveCommandBehavior : ChartPreviewDragLeaveCommandBehavior<object>
    { }

    // ChartPreviewDragLeaveCommandWithEventArgs	
    public class ChartPreviewDragLeaveCommandWithEventArgs : ChartPreviewDragLeaveCommand<DragEventArgs, ChartPreviewDragLeaveCommandBehaviorWithEventArgs>
    { }

    // ChartPreviewDragLeaveCommandBehaviorWithEventArgs
    public class ChartPreviewDragLeaveCommandBehaviorWithEventArgs : ChartPreviewDragLeaveCommandBehavior<DragEventArgs>
    {
        public ChartPreviewDragLeaveCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartDragLeave
    // ChartDragLeaveCommand<T, TBehavior>
    public class ChartDragLeaveCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartDragLeaveCommandBehavior<T>, new()
    { }

    // ChartDragLeaveCommandBehavior<TReturn>
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

    // ChartDragLeaveCommand
    public class ChartDragLeaveCommand : ChartCommandBase<ChartDragLeaveCommandBehavior>
    { }

    // ChartDragLeaveCommandBehavior
    public class ChartDragLeaveCommandBehavior : ChartDragLeaveCommandBehavior<object>
    { }

    // ChartDragLeaveCommandWithEventArgs	
    public class ChartDragLeaveCommandWithEventArgs : ChartDragLeaveCommand<DragEventArgs, ChartDragLeaveCommandBehaviorWithEventArgs>
    { }

    // ChartDragLeaveCommandBehaviorWithEventArgs
    public class ChartDragLeaveCommandBehaviorWithEventArgs : ChartDragLeaveCommandBehavior<DragEventArgs>
    {
        public ChartDragLeaveCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartPreviewDrop
    // ChartPreviewDropCommand<T, TBehavior>
    public class ChartPreviewDropCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartPreviewDropCommandBehavior<T>, new()
    { }

    // ChartPreviewDropCommandBehavior<TReturn>
    public class ChartPreviewDropCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, DragEventArgs>
    {
        public ChartPreviewDropCommandBehavior(Func<object, DragEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartPreviewDropCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewDrop += OnEventRaised;
        }
    }

    // ChartPreviewDropCommand
    public class ChartPreviewDropCommand : ChartCommandBase<ChartPreviewDropCommandBehavior>
    { }

    // ChartPreviewDropCommandBehavior
    public class ChartPreviewDropCommandBehavior : ChartPreviewDropCommandBehavior<object>
    { }

    // ChartPreviewDropCommandWithEventArgs	
    public class ChartPreviewDropCommandWithEventArgs : ChartPreviewDropCommand<DragEventArgs, ChartPreviewDropCommandBehaviorWithEventArgs>
    { }

    // ChartPreviewDropCommandBehaviorWithEventArgs
    public class ChartPreviewDropCommandBehaviorWithEventArgs : ChartPreviewDropCommandBehavior<DragEventArgs>
    {
        public ChartPreviewDropCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartDrop
    // ChartDropCommand<T, TBehavior>
    public class ChartDropCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartDropCommandBehavior<T>, new()
    { }

    // ChartDropCommandBehavior<TReturn>
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

    // ChartDropCommand
    public class ChartDropCommand : ChartCommandBase<ChartDropCommandBehavior>
    { }

    // ChartDropCommandBehavior
    public class ChartDropCommandBehavior : ChartDropCommandBehavior<object>
    { }

    // ChartDropCommandWithEventArgs	
    public class ChartDropCommandWithEventArgs : ChartDropCommand<DragEventArgs, ChartDropCommandBehaviorWithEventArgs>
    { }

    // ChartDropCommandBehaviorWithEventArgs
    public class ChartDropCommandBehaviorWithEventArgs : ChartDropCommandBehavior<DragEventArgs>
    {
        public ChartDropCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
#if SyncfusionFramework4_0
    #region ChartPreviewTouchDown
    // ChartPreviewTouchDownCommand<T, TBehavior>
    public class ChartPreviewTouchDownCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartPreviewTouchDownCommandBehavior<T>, new()
    { }

    // ChartPreviewTouchDownCommandBehavior<TReturn>
    public class ChartPreviewTouchDownCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, TouchEventArgs>
    {
        public ChartPreviewTouchDownCommandBehavior(Func<object, TouchEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartPreviewTouchDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewTouchDown += OnEventRaised;
        }
    }

    // ChartPreviewTouchDownCommand
    public class ChartPreviewTouchDownCommand : ChartCommandBase<ChartPreviewTouchDownCommandBehavior>
    { }

    // ChartPreviewTouchDownCommandBehavior
    public class ChartPreviewTouchDownCommandBehavior : ChartPreviewTouchDownCommandBehavior<object>
    { }

    // ChartPreviewTouchDownCommandWithEventArgs	
    public class ChartPreviewTouchDownCommandWithEventArgs : ChartPreviewTouchDownCommand<TouchEventArgs, ChartPreviewTouchDownCommandBehaviorWithEventArgs>
    { }

    // ChartPreviewTouchDownCommandBehaviorWithEventArgs
    public class ChartPreviewTouchDownCommandBehaviorWithEventArgs : ChartPreviewTouchDownCommandBehavior<TouchEventArgs>
    {
        public ChartPreviewTouchDownCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartTouchDown
    // ChartTouchDownCommand<T, TBehavior>
    public class ChartTouchDownCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartTouchDownCommandBehavior<T>, new()
    { }

    // ChartTouchDownCommandBehavior<TReturn>
    public class ChartTouchDownCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, TouchEventArgs>
    {
        public ChartTouchDownCommandBehavior(Func<object, TouchEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartTouchDownCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.TouchDown += OnEventRaised;
        }
    }

    // ChartTouchDownCommand
    public class ChartTouchDownCommand : ChartCommandBase<ChartTouchDownCommandBehavior>
    { }

    // ChartTouchDownCommandBehavior
    public class ChartTouchDownCommandBehavior : ChartTouchDownCommandBehavior<object>
    { }

    // ChartTouchDownCommandWithEventArgs	
    public class ChartTouchDownCommandWithEventArgs : ChartTouchDownCommand<TouchEventArgs, ChartTouchDownCommandBehaviorWithEventArgs>
    { }

    // ChartTouchDownCommandBehaviorWithEventArgs
    public class ChartTouchDownCommandBehaviorWithEventArgs : ChartTouchDownCommandBehavior<TouchEventArgs>
    {
        public ChartTouchDownCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartPreviewTouchMove
    // ChartPreviewTouchMoveCommand<T, TBehavior>
    public class ChartPreviewTouchMoveCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartPreviewTouchMoveCommandBehavior<T>, new()
    { }

    // ChartPreviewTouchMoveCommandBehavior<TReturn>
    public class ChartPreviewTouchMoveCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, TouchEventArgs>
    {
        public ChartPreviewTouchMoveCommandBehavior(Func<object, TouchEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartPreviewTouchMoveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewTouchMove += OnEventRaised;
        }
    }

    // ChartPreviewTouchMoveCommand
    public class ChartPreviewTouchMoveCommand : ChartCommandBase<ChartPreviewTouchMoveCommandBehavior>
    { }

    // ChartPreviewTouchMoveCommandBehavior
    public class ChartPreviewTouchMoveCommandBehavior : ChartPreviewTouchMoveCommandBehavior<object>
    { }

    // ChartPreviewTouchMoveCommandWithEventArgs	
    public class ChartPreviewTouchMoveCommandWithEventArgs : ChartPreviewTouchMoveCommand<TouchEventArgs, ChartPreviewTouchMoveCommandBehaviorWithEventArgs>
    { }

    // ChartPreviewTouchMoveCommandBehaviorWithEventArgs
    public class ChartPreviewTouchMoveCommandBehaviorWithEventArgs : ChartPreviewTouchMoveCommandBehavior<TouchEventArgs>
    {
        public ChartPreviewTouchMoveCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartTouchMove
    // ChartTouchMoveCommand<T, TBehavior>
    public class ChartTouchMoveCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartTouchMoveCommandBehavior<T>, new()
    { }

    // ChartTouchMoveCommandBehavior<TReturn>
    public class ChartTouchMoveCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, TouchEventArgs>
    {
        public ChartTouchMoveCommandBehavior(Func<object, TouchEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartTouchMoveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.TouchMove += OnEventRaised;
        }
    }

    // ChartTouchMoveCommand
    public class ChartTouchMoveCommand : ChartCommandBase<ChartTouchMoveCommandBehavior>
    { }

    // ChartTouchMoveCommandBehavior
    public class ChartTouchMoveCommandBehavior : ChartTouchMoveCommandBehavior<object>
    { }

    // ChartTouchMoveCommandWithEventArgs	
    public class ChartTouchMoveCommandWithEventArgs : ChartTouchMoveCommand<TouchEventArgs, ChartTouchMoveCommandBehaviorWithEventArgs>
    { }

    // ChartTouchMoveCommandBehaviorWithEventArgs
    public class ChartTouchMoveCommandBehaviorWithEventArgs : ChartTouchMoveCommandBehavior<TouchEventArgs>
    {
        public ChartTouchMoveCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartPreviewTouchUp
    // ChartPreviewTouchUpCommand<T, TBehavior>
    public class ChartPreviewTouchUpCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartPreviewTouchUpCommandBehavior<T>, new()
    { }

    // ChartPreviewTouchUpCommandBehavior<TReturn>
    public class ChartPreviewTouchUpCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, TouchEventArgs>
    {
        public ChartPreviewTouchUpCommandBehavior(Func<object, TouchEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartPreviewTouchUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.PreviewTouchUp += OnEventRaised;
        }
    }

    // ChartPreviewTouchUpCommand
    public class ChartPreviewTouchUpCommand : ChartCommandBase<ChartPreviewTouchUpCommandBehavior>
    { }

    // ChartPreviewTouchUpCommandBehavior
    public class ChartPreviewTouchUpCommandBehavior : ChartPreviewTouchUpCommandBehavior<object>
    { }

    // ChartPreviewTouchUpCommandWithEventArgs	
    public class ChartPreviewTouchUpCommandWithEventArgs : ChartPreviewTouchUpCommand<TouchEventArgs, ChartPreviewTouchUpCommandBehaviorWithEventArgs>
    { }

    // ChartPreviewTouchUpCommandBehaviorWithEventArgs
    public class ChartPreviewTouchUpCommandBehaviorWithEventArgs : ChartPreviewTouchUpCommandBehavior<TouchEventArgs>
    {
        public ChartPreviewTouchUpCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartTouchUp
    // ChartTouchUpCommand<T, TBehavior>
    public class ChartTouchUpCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartTouchUpCommandBehavior<T>, new()
    { }

    // ChartTouchUpCommandBehavior<TReturn>
    public class ChartTouchUpCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, TouchEventArgs>
    {
        public ChartTouchUpCommandBehavior(Func<object, TouchEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartTouchUpCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.TouchUp += OnEventRaised;
        }
    }

    // ChartTouchUpCommand
    public class ChartTouchUpCommand : ChartCommandBase<ChartTouchUpCommandBehavior>
    { }

    // ChartTouchUpCommandBehavior
    public class ChartTouchUpCommandBehavior : ChartTouchUpCommandBehavior<object>
    { }

    // ChartTouchUpCommandWithEventArgs	
    public class ChartTouchUpCommandWithEventArgs : ChartTouchUpCommand<TouchEventArgs, ChartTouchUpCommandBehaviorWithEventArgs>
    { }

    // ChartTouchUpCommandBehaviorWithEventArgs
    public class ChartTouchUpCommandBehaviorWithEventArgs : ChartTouchUpCommandBehavior<TouchEventArgs>
    {
        public ChartTouchUpCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartGotTouchCapture
    // ChartGotTouchCaptureCommand<T, TBehavior>
    public class ChartGotTouchCaptureCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartGotTouchCaptureCommandBehavior<T>, new()
    { }

    // ChartGotTouchCaptureCommandBehavior<TReturn>
    public class ChartGotTouchCaptureCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, TouchEventArgs>
    {
        public ChartGotTouchCaptureCommandBehavior(Func<object, TouchEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartGotTouchCaptureCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.GotTouchCapture += OnEventRaised;
        }
    }

    // ChartGotTouchCaptureCommand
    public class ChartGotTouchCaptureCommand : ChartCommandBase<ChartGotTouchCaptureCommandBehavior>
    { }

    // ChartGotTouchCaptureCommandBehavior
    public class ChartGotTouchCaptureCommandBehavior : ChartGotTouchCaptureCommandBehavior<object>
    { }

    // ChartGotTouchCaptureCommandWithEventArgs	
    public class ChartGotTouchCaptureCommandWithEventArgs : ChartGotTouchCaptureCommand<TouchEventArgs, ChartGotTouchCaptureCommandBehaviorWithEventArgs>
    { }

    // ChartGotTouchCaptureCommandBehaviorWithEventArgs
    public class ChartGotTouchCaptureCommandBehaviorWithEventArgs : ChartGotTouchCaptureCommandBehavior<TouchEventArgs>
    {
        public ChartGotTouchCaptureCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartLostTouchCapture
    // ChartLostTouchCaptureCommand<T, TBehavior>
    public class ChartLostTouchCaptureCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartLostTouchCaptureCommandBehavior<T>, new()
    { }

    // ChartLostTouchCaptureCommandBehavior<TReturn>
    public class ChartLostTouchCaptureCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, TouchEventArgs>
    {
        public ChartLostTouchCaptureCommandBehavior(Func<object, TouchEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartLostTouchCaptureCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.LostTouchCapture += OnEventRaised;
        }
    }

    // ChartLostTouchCaptureCommand
    public class ChartLostTouchCaptureCommand : ChartCommandBase<ChartLostTouchCaptureCommandBehavior>
    { }

    // ChartLostTouchCaptureCommandBehavior
    public class ChartLostTouchCaptureCommandBehavior : ChartLostTouchCaptureCommandBehavior<object>
    { }

    // ChartLostTouchCaptureCommandWithEventArgs	
    public class ChartLostTouchCaptureCommandWithEventArgs : ChartLostTouchCaptureCommand<TouchEventArgs, ChartLostTouchCaptureCommandBehaviorWithEventArgs>
    { }

    // ChartLostTouchCaptureCommandBehaviorWithEventArgs
    public class ChartLostTouchCaptureCommandBehaviorWithEventArgs : ChartLostTouchCaptureCommandBehavior<TouchEventArgs>
    {
        public ChartLostTouchCaptureCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartTouchEnter
    // ChartTouchEnterCommand<T, TBehavior>
    public class ChartTouchEnterCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartTouchEnterCommandBehavior<T>, new()
    { }

    // ChartTouchEnterCommandBehavior<TReturn>
    public class ChartTouchEnterCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, TouchEventArgs>
    {
        public ChartTouchEnterCommandBehavior(Func<object, TouchEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartTouchEnterCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.TouchEnter += OnEventRaised;
        }
    }

    // ChartTouchEnterCommand
    public class ChartTouchEnterCommand : ChartCommandBase<ChartTouchEnterCommandBehavior>
    { }

    // ChartTouchEnterCommandBehavior
    public class ChartTouchEnterCommandBehavior : ChartTouchEnterCommandBehavior<object>
    { }

    // ChartTouchEnterCommandWithEventArgs	
    public class ChartTouchEnterCommandWithEventArgs : ChartTouchEnterCommand<TouchEventArgs, ChartTouchEnterCommandBehaviorWithEventArgs>
    { }

    // ChartTouchEnterCommandBehaviorWithEventArgs
    public class ChartTouchEnterCommandBehaviorWithEventArgs : ChartTouchEnterCommandBehavior<TouchEventArgs>
    {
        public ChartTouchEnterCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartTouchLeave
    // ChartTouchLeaveCommand<T, TBehavior>
    public class ChartTouchLeaveCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartTouchLeaveCommandBehavior<T>, new()
    { }

    // ChartTouchLeaveCommandBehavior<TReturn>
    public class ChartTouchLeaveCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, TouchEventArgs>
    {
        public ChartTouchLeaveCommandBehavior(Func<object, TouchEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartTouchLeaveCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.TouchLeave += OnEventRaised;
        }
    }

    // ChartTouchLeaveCommand
    public class ChartTouchLeaveCommand : ChartCommandBase<ChartTouchLeaveCommandBehavior>
    { }

    // ChartTouchLeaveCommandBehavior
    public class ChartTouchLeaveCommandBehavior : ChartTouchLeaveCommandBehavior<object>
    { }

    // ChartTouchLeaveCommandWithEventArgs	
    public class ChartTouchLeaveCommandWithEventArgs : ChartTouchLeaveCommand<TouchEventArgs, ChartTouchLeaveCommandBehaviorWithEventArgs>
    { }

    // ChartTouchLeaveCommandBehaviorWithEventArgs
    public class ChartTouchLeaveCommandBehaviorWithEventArgs : ChartTouchLeaveCommandBehavior<TouchEventArgs>
    {
        public ChartTouchLeaveCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
#endif
    #region ChartIsMouseDirectlyOverChanged
    // ChartIsMouseDirectlyOverChangedCommand<T, TBehavior>
    public class ChartIsMouseDirectlyOverChangedCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartIsMouseDirectlyOverChangedCommandBehavior<T>, new()
    { }

    // ChartIsMouseDirectlyOverChangedCommandBehavior<TReturn>
    public class ChartIsMouseDirectlyOverChangedCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public ChartIsMouseDirectlyOverChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartIsMouseDirectlyOverChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsMouseDirectlyOverChanged += OnEventRaised;
        }
    }

    // ChartIsMouseDirectlyOverChangedCommand
    public class ChartIsMouseDirectlyOverChangedCommand : ChartCommandBase<ChartIsMouseDirectlyOverChangedCommandBehavior>
    { }

    // ChartIsMouseDirectlyOverChangedCommandBehavior
    public class ChartIsMouseDirectlyOverChangedCommandBehavior : ChartIsMouseDirectlyOverChangedCommandBehavior<object>
    { }

    // ChartIsMouseDirectlyOverChangedCommandWithEventArgs	
    public class ChartIsMouseDirectlyOverChangedCommandWithEventArgs : ChartIsMouseDirectlyOverChangedCommand<DependencyPropertyChangedEventArgs, ChartIsMouseDirectlyOverChangedCommandBehaviorWithEventArgs>
    { }

    // ChartIsMouseDirectlyOverChangedCommandBehaviorWithEventArgs
    public class ChartIsMouseDirectlyOverChangedCommandBehaviorWithEventArgs : ChartIsMouseDirectlyOverChangedCommandBehavior<DependencyPropertyChangedEventArgs>
    {
        public ChartIsMouseDirectlyOverChangedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartIsKeyboardFocusWithinChanged
    // ChartIsKeyboardFocusWithinChangedCommand<T, TBehavior>
    public class ChartIsKeyboardFocusWithinChangedCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartIsKeyboardFocusWithinChangedCommandBehavior<T>, new()
    { }

    // ChartIsKeyboardFocusWithinChangedCommandBehavior<TReturn>
    public class ChartIsKeyboardFocusWithinChangedCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public ChartIsKeyboardFocusWithinChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartIsKeyboardFocusWithinChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsKeyboardFocusWithinChanged += OnEventRaised;
        }
    }

    // ChartIsKeyboardFocusWithinChangedCommand
    public class ChartIsKeyboardFocusWithinChangedCommand : ChartCommandBase<ChartIsKeyboardFocusWithinChangedCommandBehavior>
    { }

    // ChartIsKeyboardFocusWithinChangedCommandBehavior
    public class ChartIsKeyboardFocusWithinChangedCommandBehavior : ChartIsKeyboardFocusWithinChangedCommandBehavior<object>
    { }

    // ChartIsKeyboardFocusWithinChangedCommandWithEventArgs	
    public class ChartIsKeyboardFocusWithinChangedCommandWithEventArgs : ChartIsKeyboardFocusWithinChangedCommand<DependencyPropertyChangedEventArgs, ChartIsKeyboardFocusWithinChangedCommandBehaviorWithEventArgs>
    { }

    // ChartIsKeyboardFocusWithinChangedCommandBehaviorWithEventArgs
    public class ChartIsKeyboardFocusWithinChangedCommandBehaviorWithEventArgs : ChartIsKeyboardFocusWithinChangedCommandBehavior<DependencyPropertyChangedEventArgs>
    {
        public ChartIsKeyboardFocusWithinChangedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartIsMouseCapturedChanged
    // ChartIsMouseCapturedChangedCommand<T, TBehavior>
    public class ChartIsMouseCapturedChangedCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartIsMouseCapturedChangedCommandBehavior<T>, new()
    { }

    // ChartIsMouseCapturedChangedCommandBehavior<TReturn>
    public class ChartIsMouseCapturedChangedCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public ChartIsMouseCapturedChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartIsMouseCapturedChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsMouseCapturedChanged += OnEventRaised;
        }
    }

    // ChartIsMouseCapturedChangedCommand
    public class ChartIsMouseCapturedChangedCommand : ChartCommandBase<ChartIsMouseCapturedChangedCommandBehavior>
    { }

    // ChartIsMouseCapturedChangedCommandBehavior
    public class ChartIsMouseCapturedChangedCommandBehavior : ChartIsMouseCapturedChangedCommandBehavior<object>
    { }

    // ChartIsMouseCapturedChangedCommandWithEventArgs	
    public class ChartIsMouseCapturedChangedCommandWithEventArgs : ChartIsMouseCapturedChangedCommand<DependencyPropertyChangedEventArgs, ChartIsMouseCapturedChangedCommandBehaviorWithEventArgs>
    { }

    // ChartIsMouseCapturedChangedCommandBehaviorWithEventArgs
    public class ChartIsMouseCapturedChangedCommandBehaviorWithEventArgs : ChartIsMouseCapturedChangedCommandBehavior<DependencyPropertyChangedEventArgs>
    {
        public ChartIsMouseCapturedChangedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartIsMouseCaptureWithinChanged
    // ChartIsMouseCaptureWithinChangedCommand<T, TBehavior>
    public class ChartIsMouseCaptureWithinChangedCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartIsMouseCaptureWithinChangedCommandBehavior<T>, new()
    { }

    // ChartIsMouseCaptureWithinChangedCommandBehavior<TReturn>
    public class ChartIsMouseCaptureWithinChangedCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public ChartIsMouseCaptureWithinChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartIsMouseCaptureWithinChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsMouseCaptureWithinChanged += OnEventRaised;
        }
    }

    // ChartIsMouseCaptureWithinChangedCommand
    public class ChartIsMouseCaptureWithinChangedCommand : ChartCommandBase<ChartIsMouseCaptureWithinChangedCommandBehavior>
    { }

    // ChartIsMouseCaptureWithinChangedCommandBehavior
    public class ChartIsMouseCaptureWithinChangedCommandBehavior : ChartIsMouseCaptureWithinChangedCommandBehavior<object>
    { }

    // ChartIsMouseCaptureWithinChangedCommandWithEventArgs	
    public class ChartIsMouseCaptureWithinChangedCommandWithEventArgs : ChartIsMouseCaptureWithinChangedCommand<DependencyPropertyChangedEventArgs, ChartIsMouseCaptureWithinChangedCommandBehaviorWithEventArgs>
    { }

    // ChartIsMouseCaptureWithinChangedCommandBehaviorWithEventArgs
    public class ChartIsMouseCaptureWithinChangedCommandBehaviorWithEventArgs : ChartIsMouseCaptureWithinChangedCommandBehavior<DependencyPropertyChangedEventArgs>
    {
        public ChartIsMouseCaptureWithinChangedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartIsStylusDirectlyOverChanged
    // ChartIsStylusDirectlyOverChangedCommand<T, TBehavior>
    public class ChartIsStylusDirectlyOverChangedCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartIsStylusDirectlyOverChangedCommandBehavior<T>, new()
    { }

    // ChartIsStylusDirectlyOverChangedCommandBehavior<TReturn>
    public class ChartIsStylusDirectlyOverChangedCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public ChartIsStylusDirectlyOverChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartIsStylusDirectlyOverChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsStylusDirectlyOverChanged += OnEventRaised;
        }
    }

    // ChartIsStylusDirectlyOverChangedCommand
    public class ChartIsStylusDirectlyOverChangedCommand : ChartCommandBase<ChartIsStylusDirectlyOverChangedCommandBehavior>
    { }

    // ChartIsStylusDirectlyOverChangedCommandBehavior
    public class ChartIsStylusDirectlyOverChangedCommandBehavior : ChartIsStylusDirectlyOverChangedCommandBehavior<object>
    { }

    // ChartIsStylusDirectlyOverChangedCommandWithEventArgs	
    public class ChartIsStylusDirectlyOverChangedCommandWithEventArgs : ChartIsStylusDirectlyOverChangedCommand<DependencyPropertyChangedEventArgs, ChartIsStylusDirectlyOverChangedCommandBehaviorWithEventArgs>
    { }

    // ChartIsStylusDirectlyOverChangedCommandBehaviorWithEventArgs
    public class ChartIsStylusDirectlyOverChangedCommandBehaviorWithEventArgs : ChartIsStylusDirectlyOverChangedCommandBehavior<DependencyPropertyChangedEventArgs>
    {
        public ChartIsStylusDirectlyOverChangedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartIsStylusCapturedChanged
    // ChartIsStylusCapturedChangedCommand<T, TBehavior>
    public class ChartIsStylusCapturedChangedCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartIsStylusCapturedChangedCommandBehavior<T>, new()
    { }

    // ChartIsStylusCapturedChangedCommandBehavior<TReturn>
    public class ChartIsStylusCapturedChangedCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public ChartIsStylusCapturedChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartIsStylusCapturedChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsStylusCapturedChanged += OnEventRaised;
        }
    }

    // ChartIsStylusCapturedChangedCommand
    public class ChartIsStylusCapturedChangedCommand : ChartCommandBase<ChartIsStylusCapturedChangedCommandBehavior>
    { }

    // ChartIsStylusCapturedChangedCommandBehavior
    public class ChartIsStylusCapturedChangedCommandBehavior : ChartIsStylusCapturedChangedCommandBehavior<object>
    { }

    // ChartIsStylusCapturedChangedCommandWithEventArgs	
    public class ChartIsStylusCapturedChangedCommandWithEventArgs : ChartIsStylusCapturedChangedCommand<DependencyPropertyChangedEventArgs, ChartIsStylusCapturedChangedCommandBehaviorWithEventArgs>
    { }

    // ChartIsStylusCapturedChangedCommandBehaviorWithEventArgs
    public class ChartIsStylusCapturedChangedCommandBehaviorWithEventArgs : ChartIsStylusCapturedChangedCommandBehavior<DependencyPropertyChangedEventArgs>
    {
        public ChartIsStylusCapturedChangedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartIsStylusCaptureWithinChanged
    // ChartIsStylusCaptureWithinChangedCommand<T, TBehavior>
    public class ChartIsStylusCaptureWithinChangedCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartIsStylusCaptureWithinChangedCommandBehavior<T>, new()
    { }

    // ChartIsStylusCaptureWithinChangedCommandBehavior<TReturn>
    public class ChartIsStylusCaptureWithinChangedCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public ChartIsStylusCaptureWithinChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartIsStylusCaptureWithinChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsStylusCaptureWithinChanged += OnEventRaised;
        }
    }

    // ChartIsStylusCaptureWithinChangedCommand
    public class ChartIsStylusCaptureWithinChangedCommand : ChartCommandBase<ChartIsStylusCaptureWithinChangedCommandBehavior>
    { }

    // ChartIsStylusCaptureWithinChangedCommandBehavior
    public class ChartIsStylusCaptureWithinChangedCommandBehavior : ChartIsStylusCaptureWithinChangedCommandBehavior<object>
    { }

    // ChartIsStylusCaptureWithinChangedCommandWithEventArgs	
    public class ChartIsStylusCaptureWithinChangedCommandWithEventArgs : ChartIsStylusCaptureWithinChangedCommand<DependencyPropertyChangedEventArgs, ChartIsStylusCaptureWithinChangedCommandBehaviorWithEventArgs>
    { }

    // ChartIsStylusCaptureWithinChangedCommandBehaviorWithEventArgs
    public class ChartIsStylusCaptureWithinChangedCommandBehaviorWithEventArgs : ChartIsStylusCaptureWithinChangedCommandBehavior<DependencyPropertyChangedEventArgs>
    {
        public ChartIsStylusCaptureWithinChangedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartIsKeyboardFocusedChanged
    // ChartIsKeyboardFocusedChangedCommand<T, TBehavior>
    public class ChartIsKeyboardFocusedChangedCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartIsKeyboardFocusedChangedCommandBehavior<T>, new()
    { }

    // ChartIsKeyboardFocusedChangedCommandBehavior<TReturn>
    public class ChartIsKeyboardFocusedChangedCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public ChartIsKeyboardFocusedChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartIsKeyboardFocusedChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsKeyboardFocusedChanged += OnEventRaised;
        }
    }

    // ChartIsKeyboardFocusedChangedCommand
    public class ChartIsKeyboardFocusedChangedCommand : ChartCommandBase<ChartIsKeyboardFocusedChangedCommandBehavior>
    { }

    // ChartIsKeyboardFocusedChangedCommandBehavior
    public class ChartIsKeyboardFocusedChangedCommandBehavior : ChartIsKeyboardFocusedChangedCommandBehavior<object>
    { }

    // ChartIsKeyboardFocusedChangedCommandWithEventArgs	
    public class ChartIsKeyboardFocusedChangedCommandWithEventArgs : ChartIsKeyboardFocusedChangedCommand<DependencyPropertyChangedEventArgs, ChartIsKeyboardFocusedChangedCommandBehaviorWithEventArgs>
    { }

    // ChartIsKeyboardFocusedChangedCommandBehaviorWithEventArgs
    public class ChartIsKeyboardFocusedChangedCommandBehaviorWithEventArgs : ChartIsKeyboardFocusedChangedCommandBehavior<DependencyPropertyChangedEventArgs>
    {
        public ChartIsKeyboardFocusedChangedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartLayoutUpdated
    // ChartLayoutUpdatedCommand<T, TBehavior>
    public class ChartLayoutUpdatedCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartLayoutUpdatedCommandBehavior<T>, new()
    { }

    // ChartLayoutUpdatedCommandBehavior<TReturn>
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

    // ChartLayoutUpdatedCommand
    public class ChartLayoutUpdatedCommand : ChartCommandBase<ChartLayoutUpdatedCommandBehavior>
    { }

    // ChartLayoutUpdatedCommandBehavior
    public class ChartLayoutUpdatedCommandBehavior : ChartLayoutUpdatedCommandBehavior<object>
    { }

    // ChartLayoutUpdatedCommandWithEventArgs	
    public class ChartLayoutUpdatedCommandWithEventArgs : ChartLayoutUpdatedCommand<EventArgs, ChartLayoutUpdatedCommandBehaviorWithEventArgs>
    { }

    // ChartLayoutUpdatedCommandBehaviorWithEventArgs
    public class ChartLayoutUpdatedCommandBehaviorWithEventArgs : ChartLayoutUpdatedCommandBehavior<EventArgs>
    {
        public ChartLayoutUpdatedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartGotFocus
    // ChartGotFocusCommand<T, TBehavior>
    public class ChartGotFocusCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartGotFocusCommandBehavior<T>, new()
    { }

    // ChartGotFocusCommandBehavior<TReturn>
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

    // ChartGotFocusCommand
    public class ChartGotFocusCommand : ChartCommandBase<ChartGotFocusCommandBehavior>
    { }

    // ChartGotFocusCommandBehavior
    public class ChartGotFocusCommandBehavior : ChartGotFocusCommandBehavior<object>
    { }

    // ChartGotFocusCommandWithEventArgs	
    public class ChartGotFocusCommandWithEventArgs : ChartGotFocusCommand<RoutedEventArgs, ChartGotFocusCommandBehaviorWithEventArgs>
    { }

    // ChartGotFocusCommandBehaviorWithEventArgs
    public class ChartGotFocusCommandBehaviorWithEventArgs : ChartGotFocusCommandBehavior<RoutedEventArgs>
    {
        public ChartGotFocusCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartLostFocus
    // ChartLostFocusCommand<T, TBehavior>
    public class ChartLostFocusCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartLostFocusCommandBehavior<T>, new()
    { }

    // ChartLostFocusCommandBehavior<TReturn>
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

    // ChartLostFocusCommand
    public class ChartLostFocusCommand : ChartCommandBase<ChartLostFocusCommandBehavior>
    { }

    // ChartLostFocusCommandBehavior
    public class ChartLostFocusCommandBehavior : ChartLostFocusCommandBehavior<object>
    { }

    // ChartLostFocusCommandWithEventArgs	
    public class ChartLostFocusCommandWithEventArgs : ChartLostFocusCommand<RoutedEventArgs, ChartLostFocusCommandBehaviorWithEventArgs>
    { }

    // ChartLostFocusCommandBehaviorWithEventArgs
    public class ChartLostFocusCommandBehaviorWithEventArgs : ChartLostFocusCommandBehavior<RoutedEventArgs>
    {
        public ChartLostFocusCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartIsEnabledChanged
    // ChartIsEnabledChangedCommand<T, TBehavior>
    public class ChartIsEnabledChangedCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartIsEnabledChangedCommandBehavior<T>, new()
    { }

    // ChartIsEnabledChangedCommandBehavior<TReturn>
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

    // ChartIsEnabledChangedCommand
    public class ChartIsEnabledChangedCommand : ChartCommandBase<ChartIsEnabledChangedCommandBehavior>
    { }

    // ChartIsEnabledChangedCommandBehavior
    public class ChartIsEnabledChangedCommandBehavior : ChartIsEnabledChangedCommandBehavior<object>
    { }

    // ChartIsEnabledChangedCommandWithEventArgs	
    public class ChartIsEnabledChangedCommandWithEventArgs : ChartIsEnabledChangedCommand<DependencyPropertyChangedEventArgs, ChartIsEnabledChangedCommandBehaviorWithEventArgs>
    { }

    // ChartIsEnabledChangedCommandBehaviorWithEventArgs
    public class ChartIsEnabledChangedCommandBehaviorWithEventArgs : ChartIsEnabledChangedCommandBehavior<DependencyPropertyChangedEventArgs>
    {
        public ChartIsEnabledChangedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartIsHitTestVisibleChanged
    // ChartIsHitTestVisibleChangedCommand<T, TBehavior>
    public class ChartIsHitTestVisibleChangedCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartIsHitTestVisibleChangedCommandBehavior<T>, new()
    { }

    // ChartIsHitTestVisibleChangedCommandBehavior<TReturn>
    public class ChartIsHitTestVisibleChangedCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public ChartIsHitTestVisibleChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartIsHitTestVisibleChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsHitTestVisibleChanged += OnEventRaised;
        }
    }

    // ChartIsHitTestVisibleChangedCommand
    public class ChartIsHitTestVisibleChangedCommand : ChartCommandBase<ChartIsHitTestVisibleChangedCommandBehavior>
    { }

    // ChartIsHitTestVisibleChangedCommandBehavior
    public class ChartIsHitTestVisibleChangedCommandBehavior : ChartIsHitTestVisibleChangedCommandBehavior<object>
    { }

    // ChartIsHitTestVisibleChangedCommandWithEventArgs	
    public class ChartIsHitTestVisibleChangedCommandWithEventArgs : ChartIsHitTestVisibleChangedCommand<DependencyPropertyChangedEventArgs, ChartIsHitTestVisibleChangedCommandBehaviorWithEventArgs>
    { }

    // ChartIsHitTestVisibleChangedCommandBehaviorWithEventArgs
    public class ChartIsHitTestVisibleChangedCommandBehaviorWithEventArgs : ChartIsHitTestVisibleChangedCommandBehavior<DependencyPropertyChangedEventArgs>
    {
        public ChartIsHitTestVisibleChangedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartIsVisibleChanged
    // ChartIsVisibleChangedCommand<T, TBehavior>
    public class ChartIsVisibleChangedCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartIsVisibleChangedCommandBehavior<T>, new()
    { }

    // ChartIsVisibleChangedCommandBehavior<TReturn>
    public class ChartIsVisibleChangedCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public ChartIsVisibleChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartIsVisibleChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.IsVisibleChanged += OnEventRaised;
        }
    }

    // ChartIsVisibleChangedCommand
    public class ChartIsVisibleChangedCommand : ChartCommandBase<ChartIsVisibleChangedCommandBehavior>
    { }

    // ChartIsVisibleChangedCommandBehavior
    public class ChartIsVisibleChangedCommandBehavior : ChartIsVisibleChangedCommandBehavior<object>
    { }

    // ChartIsVisibleChangedCommandWithEventArgs	
    public class ChartIsVisibleChangedCommandWithEventArgs : ChartIsVisibleChangedCommand<DependencyPropertyChangedEventArgs, ChartIsVisibleChangedCommandBehaviorWithEventArgs>
    { }

    // ChartIsVisibleChangedCommandBehaviorWithEventArgs
    public class ChartIsVisibleChangedCommandBehaviorWithEventArgs : ChartIsVisibleChangedCommandBehavior<DependencyPropertyChangedEventArgs>
    {
        public ChartIsVisibleChangedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartFocusableChanged
    // ChartFocusableChangedCommand<T, TBehavior>
    public class ChartFocusableChangedCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartFocusableChangedCommandBehavior<T>, new()
    { }

    // ChartFocusableChangedCommandBehavior<TReturn>
    public class ChartFocusableChangedCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public ChartFocusableChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartFocusableChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.FocusableChanged += OnEventRaised;
        }
    }

    // ChartFocusableChangedCommand
    public class ChartFocusableChangedCommand : ChartCommandBase<ChartFocusableChangedCommandBehavior>
    { }

    // ChartFocusableChangedCommandBehavior
    public class ChartFocusableChangedCommandBehavior : ChartFocusableChangedCommandBehavior<object>
    { }

    // ChartFocusableChangedCommandWithEventArgs	
    public class ChartFocusableChangedCommandWithEventArgs : ChartFocusableChangedCommand<DependencyPropertyChangedEventArgs, ChartFocusableChangedCommandBehaviorWithEventArgs>
    { }

    // ChartFocusableChangedCommandBehaviorWithEventArgs
    public class ChartFocusableChangedCommandBehaviorWithEventArgs : ChartFocusableChangedCommandBehavior<DependencyPropertyChangedEventArgs>
    {
        public ChartFocusableChangedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
#if SyncfusionFramework4_0
    #region ChartManipulationStarting
    // ChartManipulationStartingCommand<T, TBehavior>
    public class ChartManipulationStartingCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartManipulationStartingCommandBehavior<T>, new()
    { }

    // ChartManipulationStartingCommandBehavior<TReturn>
    public class ChartManipulationStartingCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, ManipulationStartingEventArgs>
    {
        public ChartManipulationStartingCommandBehavior(Func<object, ManipulationStartingEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartManipulationStartingCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ManipulationStarting += OnEventRaised;
        }
    }

    // ChartManipulationStartingCommand
    public class ChartManipulationStartingCommand : ChartCommandBase<ChartManipulationStartingCommandBehavior>
    { }

    // ChartManipulationStartingCommandBehavior
    public class ChartManipulationStartingCommandBehavior : ChartManipulationStartingCommandBehavior<object>
    { }

    // ChartManipulationStartingCommandWithEventArgs	
    public class ChartManipulationStartingCommandWithEventArgs : ChartManipulationStartingCommand<ManipulationStartingEventArgs, ChartManipulationStartingCommandBehaviorWithEventArgs>
    { }

    // ChartManipulationStartingCommandBehaviorWithEventArgs
    public class ChartManipulationStartingCommandBehaviorWithEventArgs : ChartManipulationStartingCommandBehavior<ManipulationStartingEventArgs>
    {
        public ChartManipulationStartingCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartManipulationStarted
    // ChartManipulationStartedCommand<T, TBehavior>
    public class ChartManipulationStartedCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartManipulationStartedCommandBehavior<T>, new()
    { }

    // ChartManipulationStartedCommandBehavior<TReturn>
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

    // ChartManipulationStartedCommand
    public class ChartManipulationStartedCommand : ChartCommandBase<ChartManipulationStartedCommandBehavior>
    { }

    // ChartManipulationStartedCommandBehavior
    public class ChartManipulationStartedCommandBehavior : ChartManipulationStartedCommandBehavior<object>
    { }

    // ChartManipulationStartedCommandWithEventArgs	
    public class ChartManipulationStartedCommandWithEventArgs : ChartManipulationStartedCommand<ManipulationStartedEventArgs, ChartManipulationStartedCommandBehaviorWithEventArgs>
    { }

    // ChartManipulationStartedCommandBehaviorWithEventArgs
    public class ChartManipulationStartedCommandBehaviorWithEventArgs : ChartManipulationStartedCommandBehavior<ManipulationStartedEventArgs>
    {
        public ChartManipulationStartedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartManipulationDelta
    // ChartManipulationDeltaCommand<T, TBehavior>
    public class ChartManipulationDeltaCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartManipulationDeltaCommandBehavior<T>, new()
    { }

    // ChartManipulationDeltaCommandBehavior<TReturn>
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

    // ChartManipulationDeltaCommand
    public class ChartManipulationDeltaCommand : ChartCommandBase<ChartManipulationDeltaCommandBehavior>
    { }

    // ChartManipulationDeltaCommandBehavior
    public class ChartManipulationDeltaCommandBehavior : ChartManipulationDeltaCommandBehavior<object>
    { }

    // ChartManipulationDeltaCommandWithEventArgs	
    public class ChartManipulationDeltaCommandWithEventArgs : ChartManipulationDeltaCommand<ManipulationDeltaEventArgs, ChartManipulationDeltaCommandBehaviorWithEventArgs>
    { }

    // ChartManipulationDeltaCommandBehaviorWithEventArgs
    public class ChartManipulationDeltaCommandBehaviorWithEventArgs : ChartManipulationDeltaCommandBehavior<ManipulationDeltaEventArgs>
    {
        public ChartManipulationDeltaCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartManipulationInertiaStarting
    // ChartManipulationInertiaStartingCommand<T, TBehavior>
    public class ChartManipulationInertiaStartingCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartManipulationInertiaStartingCommandBehavior<T>, new()
    { }

    // ChartManipulationInertiaStartingCommandBehavior<TReturn>
    public class ChartManipulationInertiaStartingCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, ManipulationInertiaStartingEventArgs>
    {
        public ChartManipulationInertiaStartingCommandBehavior(Func<object, ManipulationInertiaStartingEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartManipulationInertiaStartingCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ManipulationInertiaStarting += OnEventRaised;
        }
    }

    // ChartManipulationInertiaStartingCommand
    public class ChartManipulationInertiaStartingCommand : ChartCommandBase<ChartManipulationInertiaStartingCommandBehavior>
    { }

    // ChartManipulationInertiaStartingCommandBehavior
    public class ChartManipulationInertiaStartingCommandBehavior : ChartManipulationInertiaStartingCommandBehavior<object>
    { }

    // ChartManipulationInertiaStartingCommandWithEventArgs	
    public class ChartManipulationInertiaStartingCommandWithEventArgs : ChartManipulationInertiaStartingCommand<ManipulationInertiaStartingEventArgs, ChartManipulationInertiaStartingCommandBehaviorWithEventArgs>
    { }

    // ChartManipulationInertiaStartingCommandBehaviorWithEventArgs
    public class ChartManipulationInertiaStartingCommandBehaviorWithEventArgs : ChartManipulationInertiaStartingCommandBehavior<ManipulationInertiaStartingEventArgs>
    {
        public ChartManipulationInertiaStartingCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartManipulationBoundaryFeedback
    // ChartManipulationBoundaryFeedbackCommand<T, TBehavior>
    public class ChartManipulationBoundaryFeedbackCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartManipulationBoundaryFeedbackCommandBehavior<T>, new()
    { }

    // ChartManipulationBoundaryFeedbackCommandBehavior<TReturn>
    public class ChartManipulationBoundaryFeedbackCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, ManipulationBoundaryFeedbackEventArgs>
    {
        public ChartManipulationBoundaryFeedbackCommandBehavior(Func<object, ManipulationBoundaryFeedbackEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartManipulationBoundaryFeedbackCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ManipulationBoundaryFeedback += OnEventRaised;
        }
    }

    // ChartManipulationBoundaryFeedbackCommand
    public class ChartManipulationBoundaryFeedbackCommand : ChartCommandBase<ChartManipulationBoundaryFeedbackCommandBehavior>
    { }

    // ChartManipulationBoundaryFeedbackCommandBehavior
    public class ChartManipulationBoundaryFeedbackCommandBehavior : ChartManipulationBoundaryFeedbackCommandBehavior<object>
    { }

    // ChartManipulationBoundaryFeedbackCommandWithEventArgs	
    public class ChartManipulationBoundaryFeedbackCommandWithEventArgs : ChartManipulationBoundaryFeedbackCommand<ManipulationBoundaryFeedbackEventArgs, ChartManipulationBoundaryFeedbackCommandBehaviorWithEventArgs>
    { }

    // ChartManipulationBoundaryFeedbackCommandBehaviorWithEventArgs
    public class ChartManipulationBoundaryFeedbackCommandBehaviorWithEventArgs : ChartManipulationBoundaryFeedbackCommandBehavior<ManipulationBoundaryFeedbackEventArgs>
    {
        public ChartManipulationBoundaryFeedbackCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
    #region ChartManipulationCompleted
    // ChartManipulationCompletedCommand<T, TBehavior>
    public class ChartManipulationCompletedCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartManipulationCompletedCommandBehavior<T>, new()
    { }

    // ChartManipulationCompletedCommandBehavior<TReturn>
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

    // ChartManipulationCompletedCommand
    public class ChartManipulationCompletedCommand : ChartCommandBase<ChartManipulationCompletedCommandBehavior>
    { }

    // ChartManipulationCompletedCommandBehavior
    public class ChartManipulationCompletedCommandBehavior : ChartManipulationCompletedCommandBehavior<object>
    { }

    // ChartManipulationCompletedCommandWithEventArgs	
    public class ChartManipulationCompletedCommandWithEventArgs : ChartManipulationCompletedCommand<ManipulationCompletedEventArgs, ChartManipulationCompletedCommandBehaviorWithEventArgs>
    { }

    // ChartManipulationCompletedCommandBehaviorWithEventArgs
    public class ChartManipulationCompletedCommandBehaviorWithEventArgs : ChartManipulationCompletedCommandBehavior<ManipulationCompletedEventArgs>
    {
        public ChartManipulationCompletedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion
#endif

    #region ChartApplyCustomTabPages
    // ChartAreaInitializedCommand<T, TBehavior>
    public class ChartApplyCustomTabPagesCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartApplyCustomTabPagesCommandBehavior<T>, new()
    { }

    // ChartAreaInitializedCommandBehavior<TReturn>
    public class ChartApplyCustomTabPagesCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, EventArgs>
    {
        public ChartApplyCustomTabPagesCommandBehavior(Func<object, EventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartApplyCustomTabPagesCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ApplyCustomTabPages += OnEventRaised;
        }
    }

    // ChartAreaInitializedCommand
    public class ChartApplyCustomTabPagesCommand : ChartCommandBase<ChartApplyCustomTabPagesCommandBehavior>
    { }

    // ChartAreaInitializedCommandBehavior
    public class ChartApplyCustomTabPagesCommandBehavior : ChartApplyCustomTabPagesCommandBehavior<object>
    { }

    // ChartAreaInitializedCommandWithEventArgs	
    public class ChartApplyCustomTabPagesCommandWithEventArgs : ChartApplyCustomTabPagesCommand<EventArgs, ChartApplyCustomTabPagesCommandBehaviorWithEventArgs>
    { }

    // ChartAreaInitializedCommandBehaviorWithEventArgs
    public class ChartApplyCustomTabPagesCommandBehaviorWithEventArgs : ChartApplyCustomTabPagesCommandBehavior<EventArgs>
    {
        public ChartApplyCustomTabPagesCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region ChartInitializeCustomTabPages
    // ChartAreaInitializedCommand<T, TBehavior>
    public class ChartInitializeCustomTabPagesCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartInitializeCustomTabPagesCommandBehavior<T>, new()
    { }

    // ChartAreaInitializedCommandBehavior<TReturn>
    public class ChartInitializeCustomTabPagesCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, EventArgs>
    {
        public ChartInitializeCustomTabPagesCommandBehavior(Func<object, EventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public ChartInitializeCustomTabPagesCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ApplyCustomTabPages += OnEventRaised;
        }
    }

    // ChartAreaInitializedCommand
    public class ChartInitializeCustomTabPagesCommand : ChartCommandBase<ChartInitializeCustomTabPagesCommandBehavior>
    { }

    // ChartAreaInitializedCommandBehavior
    public class ChartInitializeCustomTabPagesCommandBehavior : ChartInitializeCustomTabPagesCommandBehavior<object>
    { }

    // ChartAreaInitializedCommandWithEventArgs	
    public class ChartInitializeCustomTabPagesCommandWithEventArgs : ChartInitializeCustomTabPagesCommand<EventArgs, ChartInitializeCustomTabPagesCommandBehaviorWithEventArgs>
    { }

    // ChartAreaInitializedCommandBehaviorWithEventArgs
    public class ChartInitializeCustomTabPagesCommandBehaviorWithEventArgs : ChartInitializeCustomTabPagesCommandBehavior<EventArgs>
    {
        public ChartInitializeCustomTabPagesCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion



    //#region ChartPropertyWindowClosed
    //// ChartAreaInitializedCommand<T, TBehavior>
    //public class ChartPropertyWindowClosedCommand<T, TBehavior> : ChartCommandBase<TBehavior> where TBehavior : ChartPropertyWindowClosedCommandBehavior<T>, new()
    //{ }

    //// ChartAreaInitializedCommandBehavior<TReturn>
    //public class ChartPropertyWindowClosedCommandBehavior<TReturn> : ChartCommandBehaviorBase<TReturn, ChartPropertyWindowEventArgs>
    //{
    //    public ChartPropertyWindowClosedCommandBehavior(Func<object, ChartPropertyWindowEventArgs, TReturn> builder)
    //    {
    //        this.builder = builder;
    //    }

    //    public ChartPropertyWindowClosedCommandBehavior()
    //        : this(null)
    //    { }

    //    protected override void OnTargetAttached()
    //    {
    //        TargetObject.ChartPropertyWindowClosed += OnEventRaised;
    //    }
    //}

    //// ChartAreaInitializedCommand
    //public class ChartPropertyWindowClosedCommand : ChartCommandBase<ChartPropertyWindowClosedCommandBehavior>
    //{ }

    //// ChartAreaInitializedCommandBehavior
    //public class ChartPropertyWindowClosedCommandBehavior : ChartPropertyWindowClosedCommandBehavior<object>
    //{ }

    //// ChartAreaInitializedCommandWithEventArgs	
    //public class ChartPropertyWindowClosedCommandWithEventArgs : ChartPropertyWindowClosedCommand<EventArgs, ChartPropertyWindowClosedCommandBehaviorWithEventArgs>
    //{ }

    //// ChartAreaInitializedCommandBehaviorWithEventArgs
    //public class ChartPropertyWindowClosedCommandBehaviorWithEventArgs : ChartPropertyWindowClosedCommandBehavior<EventArgs>
    //{
    //    public ChartPropertyWindowClosedCommandBehaviorWithEventArgs()
    //        : base((o, e) => e)
    //    { }
    //}
    //#endregion
#endregion



    
}
