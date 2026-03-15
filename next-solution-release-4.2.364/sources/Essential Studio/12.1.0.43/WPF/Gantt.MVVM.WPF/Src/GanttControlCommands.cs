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

using Syncfusion.Windows.Controls.Gantt;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Controls.Gantt.MVVM
{
    #region Command & Behaviou Base
    
    public class GanttControlCommandBase<TBehavior> : ControlCommandBase<TBehavior, GanttControl> where TBehavior : CommandBehaviorBase<GanttControl>, new()
    { }

    public class GanttControlCommandBehaviorBase<TReturn, TEventArgs> : BuilderCommandBehaviorBase<GanttControl, TEventArgs, TReturn>
    { }

    #endregion

    #region GanttControlItemsSourceChanged
    // GanttControlItemsSourceChangedCommand<T, TBehavior>
    public class GanttControlItemsSourceChangedCommand<T, TBehavior> : GanttControlCommandBase<TBehavior> where TBehavior : GanttControlItemsSourceChangedCommandBehavior<T>, new()
    { }

    // GanttControlItemsSourceChangedCommandBehavior<TReturn>
    public class GanttControlItemsSourceChangedCommandBehavior<TReturn> : GanttControlCommandBehaviorBase<TReturn, DependencyPropertyChangedEventArgs>
    {
        public GanttControlItemsSourceChangedCommandBehavior(Func<object, DependencyPropertyChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GanttControlItemsSourceChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ItemsSourceChanged += OnEventRaised;
        }
    }

    // GanttControlItemsSourceChangedCommand
    public class GanttControlItemsSourceChangedCommand : GanttControlCommandBase<GanttControlItemsSourceChangedCommandBehavior>
    { }

    // GanttControlItemsSourceChangedCommandBehavior
    public class GanttControlItemsSourceChangedCommandBehavior : GanttControlItemsSourceChangedCommandBehavior<object>
    { }

    // GanttControlItemsSourceChangedCommandWithEventArgs	
    public class GanttControlItemsSourceChangedCommandWithEventArgs : GanttControlItemsSourceChangedCommand<DependencyPropertyChangedEventArgs, GanttControlItemsSourceChangedCommandBehaviorWithEventArgs>
    { }

    // GanttControlItemsSourceChangedCommandBehaviorWithEventArgs
    public class GanttControlItemsSourceChangedCommandBehaviorWithEventArgs : GanttControlItemsSourceChangedCommandBehavior<DependencyPropertyChangedEventArgs>
    {
        public GanttControlItemsSourceChangedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GanttControlZoomChanged
    // GanttControlZoomChangedCommand<T, TBehavior>
    public class GanttControlZoomChangedCommand<T, TBehavior> : GanttControlCommandBase<TBehavior> where TBehavior : GanttControlZoomChangedCommandBehavior<T>, new()
    { }

    // GanttControlZoomChangedCommandBehavior<TReturn>
    public class GanttControlZoomChangedCommandBehavior<TReturn> : GanttControlCommandBehaviorBase<TReturn, ZoomChangedEventArgs>
    {
        public GanttControlZoomChangedCommandBehavior(Func<object, ZoomChangedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GanttControlZoomChangedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ZoomChanged += OnEventRaised;
        }
    }

    // GanttControlZoomChangedCommand
    public class GanttControlZoomChangedCommand : GanttControlCommandBase<GanttControlZoomChangedCommandBehavior>
    { }

    // GanttControlZoomChangedCommandBehavior
    public class GanttControlZoomChangedCommandBehavior : GanttControlZoomChangedCommandBehavior<object>
    { }

    // GanttControlZoomChangedCommandWithEventArgs	
    public class GanttControlZoomChangedCommandWithEventArgs : GanttControlZoomChangedCommand<ZoomChangedEventArgs, GanttControlZoomChangedCommandBehaviorWithEventArgs>
    { }

    // GanttControlZoomChangedCommandBehaviorWithEventArgs
    public class GanttControlZoomChangedCommandBehaviorWithEventArgs : GanttControlZoomChangedCommandBehavior<ZoomChangedEventArgs>
    {
        public GanttControlZoomChangedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GanttControlTemplateApplied
    // GanttControlTemplateAppliedCommand<T, TBehavior>
    public class GanttControlTemplateAppliedCommand<T, TBehavior> : GanttControlCommandBase<TBehavior> where TBehavior : GanttControlTemplateAppliedCommandBehavior<T>, new()
    { }

    // GanttControlTemplateAppliedCommandBehavior<TReturn>
    public class GanttControlTemplateAppliedCommandBehavior<TReturn> : GanttControlCommandBehaviorBase<TReturn, TemplateAppliedEventArgs>
    {
        public GanttControlTemplateAppliedCommandBehavior(Func<object, TemplateAppliedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GanttControlTemplateAppliedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.TemplateApplied += OnEventRaised;
        }
    }

    // GanttControlTemplateAppliedCommand
    public class GanttControlTemplateAppliedCommand : GanttControlCommandBase<GanttControlTemplateAppliedCommandBehavior>
    { }

    // GanttControlTemplateAppliedCommandBehavior
    public class GanttControlTemplateAppliedCommandBehavior : GanttControlTemplateAppliedCommandBehavior<object>
    { }

    // GanttControlTemplateAppliedCommandWithEventArgs	
    public class GanttControlTemplateAppliedCommandWithEventArgs : GanttControlTemplateAppliedCommand<TemplateAppliedEventArgs, GanttControlTemplateAppliedCommandBehaviorWithEventArgs>
    { }

    // GanttControlTemplateAppliedCommandBehaviorWithEventArgs
    public class GanttControlTemplateAppliedCommandBehaviorWithEventArgs : GanttControlTemplateAppliedCommandBehavior<TemplateAppliedEventArgs>
    {
        public GanttControlTemplateAppliedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GanttControlScheduleCellCreated
    // GanttControlScheduleCellCreatedCommand<T, TBehavior>
    public class GanttControlScheduleCellCreatedCommand<T, TBehavior> : GanttControlCommandBase<TBehavior> where TBehavior : GanttControlScheduleCellCreatedCommandBehavior<T>, new()
    { }

    // GanttControlScheduleCellCreatedCommandBehavior<TReturn>
    public class GanttControlScheduleCellCreatedCommandBehavior<TReturn> : GanttControlCommandBehaviorBase<TReturn, ScheduleCellCreatedEventArgs>
    {
        public GanttControlScheduleCellCreatedCommandBehavior(Func<object, ScheduleCellCreatedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GanttControlScheduleCellCreatedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ScheduleCellCreated += OnEventRaised;
        }
    }

    // GanttControlScheduleCellCreatedCommand
    public class GanttControlScheduleCellCreatedCommand : GanttControlCommandBase<GanttControlScheduleCellCreatedCommandBehavior>
    { }

    // GanttControlScheduleCellCreatedCommandBehavior
    public class GanttControlScheduleCellCreatedCommandBehavior : GanttControlScheduleCellCreatedCommandBehavior<object>
    { }

    // GanttControlScheduleCellCreatedCommandWithEventArgs	
    public class GanttControlScheduleCellCreatedCommandWithEventArgs : GanttControlScheduleCellCreatedCommand<ScheduleCellCreatedEventArgs, GanttControlScheduleCellCreatedCommandBehaviorWithEventArgs>
    { }

    // GanttControlScheduleCellCreatedCommandBehaviorWithEventArgs
    public class GanttControlScheduleCellCreatedCommandBehaviorWithEventArgs : GanttControlScheduleCellCreatedCommandBehavior<ScheduleCellCreatedEventArgs>
    {
        public GanttControlScheduleCellCreatedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }
    #endregion

    #region GanttControlStripLineCreated

    // GanttControlStripLineCreatedCommand<T, TBehavior>
    public class GanttControlStripLineCreatedCommand<T, TBehavior> : GanttControlCommandBase<TBehavior> where TBehavior : GanttControlStripLineCreatedCommandBehavior<T>, new()
    { }

    // GanttControlStripLineCreatedCommandBehavior<TReturn>
    public class GanttControlStripLineCreatedCommandBehavior<TReturn> : GanttControlCommandBehaviorBase<TReturn, StriplineCreatedEventArgs>
    {
        public GanttControlStripLineCreatedCommandBehavior(Func<object, StriplineCreatedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GanttControlStripLineCreatedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.StripLineCreated += OnEventRaised;
        }
    }

    // GanttControlStripLineCreatedCommand
    public class GanttControlStripLineCreatedCommand : GanttControlCommandBase<GanttControlStripLineCreatedCommandBehavior>
    { }

    // GanttControlStripLineCreatedCommandBehavior
    public class GanttControlStripLineCreatedCommandBehavior : GanttControlStripLineCreatedCommandBehavior<object>
    { }

    // GanttControlStripLineCreatedCommandWithEventArgs	
    public class GanttControlStripLineCreatedCommandWithEventArgs : GanttControlStripLineCreatedCommand<StriplineCreatedEventArgs, GanttControlStripLineCreatedCommandBehaviorWithEventArgs>
    { }

    // GanttControlStripLineCreatedCommandBehaviorWithEventArgs
    public class GanttControlStripLineCreatedCommandBehaviorWithEventArgs : GanttControlStripLineCreatedCommandBehavior<StriplineCreatedEventArgs>
    {
        public GanttControlStripLineCreatedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }

#endregion

    #region GanttControlResourceContaimnerCreated

    // GanttControlResourceContainerCommand<T, TBehavior>
    public class GanttControlResourceContainerCreatedCommand<T, TBehavior> : GanttControlCommandBase<TBehavior> where TBehavior : GanttControlResourceContainerCreatedCommandBehavior<T>, new()
    { }

    // GanttControlResourceContainerCommandBehavior<TReturn>
    public class GanttControlResourceContainerCreatedCommandBehavior<TReturn> : GanttControlCommandBehaviorBase<TReturn, ResourceContainerCreatedEventArgs>
    {
        public GanttControlResourceContainerCreatedCommandBehavior(Func<object, ResourceContainerCreatedEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        public GanttControlResourceContainerCreatedCommandBehavior()
            : this(null)
        { }

        protected override void OnTargetAttached()
        {
            TargetObject.ResourceContainerCreated += OnEventRaised;
        }
    }

    // GanttControlResourceContainerCreatedCommand
    public class GanttControlResourceContainerCreatedCommand : GanttControlCommandBase<GanttControlResourceContainerCreatedCommandBehavior>
    { }

    // GanttControlResourceContainerCreatedCommandBehavior
    public class GanttControlResourceContainerCreatedCommandBehavior : GanttControlResourceContainerCreatedCommandBehavior<object>
    { }

    // GanttControlResourceContainerCreatedCommandWithEventArgs	
    public class GanttControlResourceContainerCreatedCommandWithEventArgs : GanttControlResourceContainerCreatedCommand<ResourceContainerCreatedEventArgs, GanttControlResourceContainerCreatedCommandBehaviorWithEventArgs>
    { }

    // GanttControlResourceContainerCreatedCommandBehaviorWithEventArgs
    public class GanttControlResourceContainerCreatedCommandBehaviorWithEventArgs : GanttControlResourceContainerCreatedCommandBehavior<ResourceContainerCreatedEventArgs>
    {
        public GanttControlResourceContainerCreatedCommandBehaviorWithEventArgs()
            : base((o, e) => e)
        { }
    }

#endregion
}


