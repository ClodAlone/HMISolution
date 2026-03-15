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

using System.Windows.Data;
using System.Windows.Input;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.Windows.Shared;


namespace Syncfusion.Windows.Tools.MVVM
{   

	#region TaskBarVisualStyleChangedCommand
	// TaskBarVisualStyleChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TaskBarVisualStyleChangedCommand : ControlCommandBase<TaskBarVisualStyleChangedCommandBehavior, TaskBar>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TaskBarVisualStyleChangedCommandBehavior : CommandBehaviorBase<TaskBar>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, DependencyPropertyChangedEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.VisualStyleChanged += OnEventRaised;
        }
    }

	// TaskBarVisualStyleChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TaskBarVisualStyleChangedCommandBehavior<T> : TaskBarVisualStyleChangedCommandBehavior
    { }
	#endregion

	#region TaskBarGroupOrientationChangedCommand
	// TaskBarGroupOrientationChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TaskBarGroupOrientationChangedCommand : ControlCommandBase<TaskBarGroupOrientationChangedCommandBehavior, TaskBar>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TaskBarGroupOrientationChangedCommandBehavior : CommandBehaviorBase<TaskBar>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, DependencyPropertyChangedEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.GroupOrientationChanged += OnEventRaised;
        }
    }

	// TaskBarGroupOrientationChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TaskBarGroupOrientationChangedCommandBehavior<T> : TaskBarGroupOrientationChangedCommandBehavior
    { }
	#endregion

	#region TaskBarGroupMarginChangedCommand
	// TaskBarGroupMarginChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TaskBarGroupMarginChangedCommand : ControlCommandBase<TaskBarGroupMarginChangedCommandBehavior, TaskBar>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TaskBarGroupMarginChangedCommandBehavior : CommandBehaviorBase<TaskBar>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, DependencyPropertyChangedEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.GroupMarginChanged += OnEventRaised;
        }
    }

	// TaskBarGroupMarginChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TaskBarGroupMarginChangedCommandBehavior<T> : TaskBarGroupMarginChangedCommandBehavior
    { }
	#endregion

	#region TaskBarGroupWidthChangedCommand
	// TaskBarGroupWidthChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TaskBarGroupWidthChangedCommand : ControlCommandBase<TaskBarGroupWidthChangedCommandBehavior, TaskBar>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TaskBarGroupWidthChangedCommandBehavior : CommandBehaviorBase<TaskBar>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, DependencyPropertyChangedEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.GroupWidthChanged += OnEventRaised;
        }
    }

	// TaskBarGroupWidthChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TaskBarGroupWidthChangedCommandBehavior<T> : TaskBarGroupWidthChangedCommandBehavior
    { }
	#endregion

	#region TaskBarSpeedChangedCommand
	// TaskBarSpeedChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TaskBarSpeedChangedCommand : ControlCommandBase<TaskBarSpeedChangedCommandBehavior, TaskBar>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TaskBarSpeedChangedCommandBehavior : CommandBehaviorBase<TaskBar>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, DependencyPropertyChangedEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.SpeedChanged += OnEventRaised;
        }
    }

	// TaskBarSpeedChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TaskBarSpeedChangedCommandBehavior<T> : TaskBarSpeedChangedCommandBehavior
    { }
	#endregion

	#region TaskBarGroupPaddingChangedCommand
	// TaskBarGroupPaddingChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TaskBarGroupPaddingChangedCommand : ControlCommandBase<TaskBarGroupPaddingChangedCommandBehavior, TaskBar>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TaskBarGroupPaddingChangedCommandBehavior : CommandBehaviorBase<TaskBar>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, DependencyPropertyChangedEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.GroupPaddingChanged += OnEventRaised;
        }
    }

	// TaskBarGroupPaddingChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TaskBarGroupPaddingChangedCommandBehavior<T> : TaskBarGroupPaddingChangedCommandBehavior
    { }
	#endregion

	#region TaskBarButtonSizeChangedCommand
	// TaskBarButtonSizeChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TaskBarButtonSizeChangedCommand : ControlCommandBase<TaskBarButtonSizeChangedCommandBehavior, TaskBar>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TaskBarButtonSizeChangedCommandBehavior : CommandBehaviorBase<TaskBar>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, DependencyPropertyChangedEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.ButtonSizeChanged += OnEventRaised;
        }
    }

	// TaskBarButtonSizeChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TaskBarButtonSizeChangedCommandBehavior<T> : TaskBarButtonSizeChangedCommandBehavior
    { }
	#endregion

	#region TaskBarHeaderStyleChangedCommand
	// TaskBarHeaderStyleChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TaskBarHeaderStyleChangedCommand : ControlCommandBase<TaskBarHeaderStyleChangedCommandBehavior, TaskBar>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TaskBarHeaderStyleChangedCommandBehavior : CommandBehaviorBase<TaskBar>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, DependencyPropertyChangedEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.HeaderStyleChanged += OnEventRaised;
        }
    }

	// TaskBarHeaderStyleChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TaskBarHeaderStyleChangedCommandBehavior<T> : TaskBarHeaderStyleChangedCommandBehavior
    { }
	#endregion

	#region TaskBarIsOpenedChangedCommand
	// TaskBarIsOpenedChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TaskBarIsOpenedChangedCommand : ControlCommandBase<TaskBarIsOpenedChangedCommandBehavior, TaskBar>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TaskBarIsOpenedChangedCommandBehavior : CommandBehaviorBase<TaskBar>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, DependencyPropertyChangedEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.IsOpenedChanged += OnEventRaised;
        }
    }

	// TaskBarIsOpenedChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TaskBarIsOpenedChangedCommandBehavior<T> : TaskBarIsOpenedChangedCommandBehavior
    { }
	#endregion

	#region TaskBarSelectedItemChangedCommand
	// TaskBarSelectedItemChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TaskBarSelectedItemChangedCommand : ControlCommandBase<TaskBarSelectedItemChangedCommandBehavior, TaskBar>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TaskBarSelectedItemChangedCommandBehavior : CommandBehaviorBase<TaskBar>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, RoutedEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.SelectedItemChanged += OnEventRaised;
        }
    }

	// TaskBarSelectedItemChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TaskBarSelectedItemChangedCommandBehavior<T> : TaskBarSelectedItemChangedCommandBehavior
    { }
	#endregion
}


