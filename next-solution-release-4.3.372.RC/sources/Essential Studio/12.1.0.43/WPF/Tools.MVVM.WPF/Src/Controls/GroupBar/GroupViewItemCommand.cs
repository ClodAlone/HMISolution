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

	#region GroupViewItemClickCommand
	// GroupViewItemClickCommand
    /// <summary>
    /// 
    /// </summary>
	public class GroupViewItemClickCommand : ControlCommandBase<GroupViewItemClickCommandBehavior, GroupViewItem>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GroupViewItemClickCommandBehavior : CommandBehaviorBase<GroupViewItem>
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

	// GroupViewItemClickCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GroupViewItemClickCommandBehavior<T> : GroupViewItemClickCommandBehavior
    { }
	#endregion

	#region GroupViewItemDoubleClickCommand
	// GroupViewItemDoubleClickCommand
    /// <summary>
    /// 
    /// </summary>
	public class GroupViewItemDoubleClickCommand : ControlCommandBase<GroupViewItemDoubleClickCommandBehavior, GroupViewItem>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GroupViewItemDoubleClickCommandBehavior : CommandBehaviorBase<GroupViewItem>
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

	// GroupViewItemDoubleClickCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GroupViewItemDoubleClickCommandBehavior<T> : GroupViewItemDoubleClickCommandBehavior
    { }
	#endregion

	#region GroupViewItemHoverCommand
	// GroupViewItemHoverCommand
    /// <summary>
    /// 
    /// </summary>
	public class GroupViewItemHoverCommand : ControlCommandBase<GroupViewItemHoverCommandBehavior, GroupViewItem>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GroupViewItemHoverCommandBehavior : CommandBehaviorBase<GroupViewItem>
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

	// GroupViewItemHoverCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GroupViewItemHoverCommandBehavior<T> : GroupViewItemHoverCommandBehavior
    { }
	#endregion

	#region GroupViewItemPressCommand
	// GroupViewItemPressCommand
    /// <summary>
    /// 
    /// </summary>
	public class GroupViewItemPressCommand : ControlCommandBase<GroupViewItemPressCommandBehavior, GroupViewItem>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GroupViewItemPressCommandBehavior : CommandBehaviorBase<GroupViewItem>
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

	// GroupViewItemPressCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GroupViewItemPressCommandBehavior<T> : GroupViewItemPressCommandBehavior
    { }
	#endregion

	#region GroupViewItemSelectedCommand
	// GroupViewItemSelectedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GroupViewItemSelectedCommand : ControlCommandBase<GroupViewItemSelectedCommandBehavior, GroupViewItem>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GroupViewItemSelectedCommandBehavior : CommandBehaviorBase<GroupViewItem>
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
            TargetObject.Selected += OnEventRaised;
        }
    }

	// GroupViewItemSelectedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GroupViewItemSelectedCommandBehavior<T> : GroupViewItemSelectedCommandBehavior
    { }
	#endregion

	#region GroupViewItemUnselectedCommand
	// GroupViewItemUnselectedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GroupViewItemUnselectedCommand : ControlCommandBase<GroupViewItemUnselectedCommandBehavior, GroupViewItem>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GroupViewItemUnselectedCommandBehavior : CommandBehaviorBase<GroupViewItem>
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
            TargetObject.Unselected += OnEventRaised;
        }
    }

	// GroupViewItemUnselectedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GroupViewItemUnselectedCommandBehavior<T> : GroupViewItemUnselectedCommandBehavior
    { }
	#endregion

	#region GroupViewItemAfterEditCommand
	// GroupViewItemAfterEditCommand
    /// <summary>
    /// 
    /// </summary>
	public class GroupViewItemAfterEditCommand : ControlCommandBase<GroupViewItemAfterEditCommandBehavior, GroupViewItem>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GroupViewItemAfterEditCommandBehavior : CommandBehaviorBase<GroupViewItem>
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

	// GroupViewItemAfterEditCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GroupViewItemAfterEditCommandBehavior<T> : GroupViewItemAfterEditCommandBehavior
    { }
	#endregion

	#region GroupViewItemBeforeEditCommand
	// GroupViewItemBeforeEditCommand
    /// <summary>
    /// 
    /// </summary>
	public class GroupViewItemBeforeEditCommand : ControlCommandBase<GroupViewItemBeforeEditCommandBehavior, GroupViewItem>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GroupViewItemBeforeEditCommandBehavior : CommandBehaviorBase<GroupViewItem>
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

	// GroupViewItemBeforeEditCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GroupViewItemBeforeEditCommandBehavior<T> : GroupViewItemBeforeEditCommandBehavior
    { }
	#endregion

	#region GroupViewItemDragStartCommand
	// GroupViewItemDragStartCommand
    /// <summary>
    /// 
    /// </summary>
	public class GroupViewItemDragStartCommand : ControlCommandBase<GroupViewItemDragStartCommandBehavior, GroupViewItem>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GroupViewItemDragStartCommandBehavior : CommandBehaviorBase<GroupViewItem>
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
            TargetObject.DragStart += OnEventRaised;
        }
    }

	// GroupViewItemDragStartCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GroupViewItemDragStartCommandBehavior<T> : GroupViewItemDragStartCommandBehavior
    { }
	#endregion

	#region GroupViewItemDragEndCommand
	// GroupViewItemDragEndCommand
    /// <summary>
    /// 
    /// </summary>
	public class GroupViewItemDragEndCommand : ControlCommandBase<GroupViewItemDragEndCommandBehavior, GroupViewItem>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GroupViewItemDragEndCommandBehavior : CommandBehaviorBase<GroupViewItem>
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
            TargetObject.DragEnd += OnEventRaised;
        }
    }

	// GroupViewItemDragEndCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GroupViewItemDragEndCommandBehavior<T> : GroupViewItemDragEndCommandBehavior
    { }
	#endregion
}


