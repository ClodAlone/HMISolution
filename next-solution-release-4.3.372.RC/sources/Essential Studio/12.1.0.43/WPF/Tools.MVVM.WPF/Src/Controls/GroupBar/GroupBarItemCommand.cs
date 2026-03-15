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

	#region GroupBarItemHeaderStyleChangedCommand
	// GroupBarItemHeaderStyleChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GroupBarItemHeaderStyleChangedCommand : ControlCommandBase<GroupBarItemHeaderStyleChangedCommandBehavior, GroupBarItem>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GroupBarItemHeaderStyleChangedCommandBehavior : CommandBehaviorBase<GroupBarItem>
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

	// GroupBarItemHeaderStyleChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GroupBarItemHeaderStyleChangedCommandBehavior<T> : GroupBarItemHeaderStyleChangedCommandBehavior
    { }
	#endregion

	#region GroupBarItemClickCommand
	// GroupBarItemClickCommand
    /// <summary>
    /// 
    /// </summary>
	public class GroupBarItemClickCommand : ControlCommandBase<GroupBarItemClickCommandBehavior, GroupBarItem>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GroupBarItemClickCommandBehavior : CommandBehaviorBase<GroupBarItem>
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
            TargetObject.Click += OnEventRaised;
        }
    }

	// GroupBarItemClickCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GroupBarItemClickCommandBehavior<T> : GroupBarItemClickCommandBehavior
    { }
	#endregion

	#region GroupBarItemDoubleClickCommand
	// GroupBarItemDoubleClickCommand
    /// <summary>
    /// 
    /// </summary>
	public class GroupBarItemDoubleClickCommand : ControlCommandBase<GroupBarItemDoubleClickCommandBehavior, GroupBarItem>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GroupBarItemDoubleClickCommandBehavior : CommandBehaviorBase<GroupBarItem>
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
            TargetObject.DoubleClick += OnEventRaised;
        }
    }

	// GroupBarItemDoubleClickCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GroupBarItemDoubleClickCommandBehavior<T> : GroupBarItemDoubleClickCommandBehavior
    { }
	#endregion

	#region GroupBarItemPressCommand
	// GroupBarItemPressCommand
    /// <summary>
    /// 
    /// </summary>
	public class GroupBarItemPressCommand : ControlCommandBase<GroupBarItemPressCommandBehavior, GroupBarItem>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GroupBarItemPressCommandBehavior : CommandBehaviorBase<GroupBarItem>
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
            TargetObject.Press += OnEventRaised;
        }
    }

	// GroupBarItemPressCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GroupBarItemPressCommandBehavior<T> : GroupBarItemPressCommandBehavior
    { }
	#endregion

	#region GroupBarItemHoverCommand
	// GroupBarItemHoverCommand
    /// <summary>
    /// 
    /// </summary>
	public class GroupBarItemHoverCommand : ControlCommandBase<GroupBarItemHoverCommandBehavior, GroupBarItem>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GroupBarItemHoverCommandBehavior : CommandBehaviorBase<GroupBarItem>
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
            TargetObject.Hover += OnEventRaised;
        }
    }

	// GroupBarItemHoverCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GroupBarItemHoverCommandBehavior<T> : GroupBarItemHoverCommandBehavior
    { }
	#endregion

	#region GroupBarItemAfterEditCommand
	// GroupBarItemAfterEditCommand
    /// <summary>
    /// 
    /// </summary>
	public class GroupBarItemAfterEditCommand : ControlCommandBase<GroupBarItemAfterEditCommandBehavior, GroupBarItem>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GroupBarItemAfterEditCommandBehavior : CommandBehaviorBase<GroupBarItem>
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
            TargetObject.AfterEdit += OnEventRaised;
        }
    }

	// GroupBarItemAfterEditCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GroupBarItemAfterEditCommandBehavior<T> : GroupBarItemAfterEditCommandBehavior
    { }
	#endregion

	#region GroupBarItemBeforeEditCommand
	// GroupBarItemBeforeEditCommand
    /// <summary>
    /// 
    /// </summary>
	public class GroupBarItemBeforeEditCommand : ControlCommandBase<GroupBarItemBeforeEditCommandBehavior, GroupBarItem>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GroupBarItemBeforeEditCommandBehavior : CommandBehaviorBase<GroupBarItem>
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
            TargetObject.BeforeEdit += OnEventRaised;
        }
    }

	// GroupBarItemBeforeEditCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GroupBarItemBeforeEditCommandBehavior<T> : GroupBarItemBeforeEditCommandBehavior
    { }
	#endregion
}


