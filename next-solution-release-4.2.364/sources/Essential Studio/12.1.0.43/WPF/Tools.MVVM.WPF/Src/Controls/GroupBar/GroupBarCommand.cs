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
using System.Collections.Specialized;


namespace Syncfusion.Windows.Tools.MVVM
{   

	#region GroupBarOrientationChangingCommand
	// GroupBarOrientationChangingCommand
    /// <summary>
    /// 
    /// </summary>
	public class GroupBarOrientationChangingCommand : ControlCommandBase<GroupBarOrientationChangingCommandBehavior, GroupBar>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GroupBarOrientationChangingCommandBehavior : CommandBehaviorBase<GroupBar>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, OrientationChangeEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.OrientationChanging += OnEventRaised;
        }
    }

	// GroupBarOrientationChangingCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GroupBarOrientationChangingCommandBehavior<T> : GroupBarOrientationChangingCommandBehavior
    { }
	#endregion

	#region GroupBarOrientationChangedCommand
	// GroupBarOrientationChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GroupBarOrientationChangedCommand : ControlCommandBase<GroupBarOrientationChangedCommandBehavior, GroupBar>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GroupBarOrientationChangedCommandBehavior : CommandBehaviorBase<GroupBar>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, OrientationChangeEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.OrientationChanged += OnEventRaised;
        }
    }

	// GroupBarOrientationChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GroupBarOrientationChangedCommandBehavior<T> : GroupBarOrientationChangedCommandBehavior
    { }
	#endregion

	#region GroupBarFlowDirectionChangedCommand
	// GroupBarFlowDirectionChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GroupBarFlowDirectionChangedCommand : ControlCommandBase<GroupBarFlowDirectionChangedCommandBehavior, GroupBar>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GroupBarFlowDirectionChangedCommandBehavior : CommandBehaviorBase<GroupBar>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, FlowDirectionChangedEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.FlowDirectionChanged += OnEventRaised;
        }
    }

	// GroupBarFlowDirectionChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GroupBarFlowDirectionChangedCommandBehavior<T> : GroupBarFlowDirectionChangedCommandBehavior
    { }
	#endregion

	#region GroupBarVisualModeChangedCommand
	// GroupBarVisualModeChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GroupBarVisualModeChangedCommand : ControlCommandBase<GroupBarVisualModeChangedCommandBehavior, GroupBar>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GroupBarVisualModeChangedCommandBehavior : CommandBehaviorBase<GroupBar>
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
            TargetObject.VisualModeChanged += OnEventRaised;
        }
    }

	// GroupBarVisualModeChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GroupBarVisualModeChangedCommandBehavior<T> : GroupBarVisualModeChangedCommandBehavior
    { }
	#endregion

	#region GroupBarBeforeSplitUpCommand
	// GroupBarBeforeSplitUpCommand
    /// <summary>
    /// 
    /// </summary>
	public class GroupBarBeforeSplitUpCommand : ControlCommandBase<GroupBarBeforeSplitUpCommandBehavior, GroupBar>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GroupBarBeforeSplitUpCommandBehavior : CommandBehaviorBase<GroupBar>
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
            TargetObject.BeforeSplitUp += OnEventRaised;
        }
    }

	// GroupBarBeforeSplitUpCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GroupBarBeforeSplitUpCommandBehavior<T> : GroupBarBeforeSplitUpCommandBehavior
    { }
	#endregion

	#region GroupBarBeforeSplitDownCommand
	// GroupBarBeforeSplitDownCommand
    /// <summary>
    /// 
    /// </summary>
	public class GroupBarBeforeSplitDownCommand : ControlCommandBase<GroupBarBeforeSplitDownCommandBehavior, GroupBar>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GroupBarBeforeSplitDownCommandBehavior : CommandBehaviorBase<GroupBar>
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
            TargetObject.BeforeSplitDown += OnEventRaised;
        }
    }

	// GroupBarBeforeSplitDownCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GroupBarBeforeSplitDownCommandBehavior<T> : GroupBarBeforeSplitDownCommandBehavior
    { }
	#endregion

	#region GroupBarAfterSplitUpCommand
	// GroupBarAfterSplitUpCommand
    /// <summary>
    /// 
    /// </summary>
	public class GroupBarAfterSplitUpCommand : ControlCommandBase<GroupBarAfterSplitUpCommandBehavior, GroupBar>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GroupBarAfterSplitUpCommandBehavior : CommandBehaviorBase<GroupBar>
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
            TargetObject.AfterSplitUp += OnEventRaised;
        }
    }

	// GroupBarAfterSplitUpCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GroupBarAfterSplitUpCommandBehavior<T> : GroupBarAfterSplitUpCommandBehavior
    { }
	#endregion

	#region GroupBarAfterSplitDownCommand
	// GroupBarAfterSplitDownCommand
    /// <summary>
    /// 
    /// </summary>
	public class GroupBarAfterSplitDownCommand : ControlCommandBase<GroupBarAfterSplitDownCommandBehavior, GroupBar>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GroupBarAfterSplitDownCommandBehavior : CommandBehaviorBase<GroupBar>
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
            TargetObject.AfterSplitDown += OnEventRaised;
        }
    }

	// GroupBarAfterSplitDownCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GroupBarAfterSplitDownCommandBehavior<T> : GroupBarAfterSplitDownCommandBehavior
    { }
	#endregion

	#region GroupBarSelectedObjectChangedCommand
	// GroupBarSelectedObjectChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GroupBarSelectedObjectChangedCommand : ControlCommandBase<GroupBarSelectedObjectChangedCommandBehavior, GroupBar>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GroupBarSelectedObjectChangedCommandBehavior : CommandBehaviorBase<GroupBar>
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
            TargetObject.SelectedObjectChanged += OnEventRaised;
        }
    }

	// GroupBarSelectedObjectChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GroupBarSelectedObjectChangedCommandBehavior<T> : GroupBarSelectedObjectChangedCommandBehavior
    { }
	#endregion

	#region GroupBarSelectedItemChangedCommand
	// GroupBarSelectedItemChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GroupBarSelectedItemChangedCommand : ControlCommandBase<GroupBarSelectedItemChangedCommandBehavior, GroupBar>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GroupBarSelectedItemChangedCommandBehavior : CommandBehaviorBase<GroupBar>
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

	// GroupBarSelectedItemChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GroupBarSelectedItemChangedCommandBehavior<T> : GroupBarSelectedItemChangedCommandBehavior
    { }
	#endregion

	#region GroupBarSelectedTabChangedCommand
	// GroupBarSelectedTabChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GroupBarSelectedTabChangedCommand : ControlCommandBase<GroupBarSelectedTabChangedCommandBehavior, GroupBar>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GroupBarSelectedTabChangedCommandBehavior : CommandBehaviorBase<GroupBar>
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
            TargetObject.SelectedTabChanged += OnEventRaised;
        }
    }

	// GroupBarSelectedTabChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GroupBarSelectedTabChangedCommandBehavior<T> : GroupBarSelectedTabChangedCommandBehavior
    { }
	#endregion

	#region GroupBarNavigationMenuOpeningCommand
	// GroupBarNavigationMenuOpeningCommand
    /// <summary>
    /// 
    /// </summary>
	public class GroupBarNavigationMenuOpeningCommand : ControlCommandBase<GroupBarNavigationMenuOpeningCommandBehavior, GroupBar>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GroupBarNavigationMenuOpeningCommandBehavior : CommandBehaviorBase<GroupBar>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, EventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.NavigationMenuOpening += OnEventRaised;
        }
    }

	// GroupBarNavigationMenuOpeningCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GroupBarNavigationMenuOpeningCommandBehavior<T> : GroupBarNavigationMenuOpeningCommandBehavior
    { }
	#endregion

	#region GroupBarNavigationMenuClosingCommand
	// GroupBarNavigationMenuClosingCommand
    /// <summary>
    /// 
    /// </summary>
	public class GroupBarNavigationMenuClosingCommand : ControlCommandBase<GroupBarNavigationMenuClosingCommandBehavior, GroupBar>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GroupBarNavigationMenuClosingCommandBehavior : CommandBehaviorBase<GroupBar>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, CancelEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.NavigationMenuClosing += OnEventRaised;
        }
    }

	// GroupBarNavigationMenuClosingCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GroupBarNavigationMenuClosingCommandBehavior<T> : GroupBarNavigationMenuClosingCommandBehavior
    { }
	#endregion

	#region GroupBarNavigationOptionsMenuItemClickCommand
	// GroupBarNavigationOptionsMenuItemClickCommand
    /// <summary>
    /// 
    /// </summary>
	public class GroupBarNavigationOptionsMenuItemClickCommand : ControlCommandBase<GroupBarNavigationOptionsMenuItemClickCommandBehavior, GroupBar>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GroupBarNavigationOptionsMenuItemClickCommandBehavior : CommandBehaviorBase<GroupBar>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, EventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.NavigationOptionsMenuItemClick += OnEventRaised;
        }
    }

	// GroupBarNavigationOptionsMenuItemClickCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GroupBarNavigationOptionsMenuItemClickCommandBehavior<T> : GroupBarNavigationOptionsMenuItemClickCommandBehavior
    { }
	#endregion

	#region GroupBarContextMenuItemClickCommand
	// GroupBarContextMenuItemClickCommand
    /// <summary>
    /// 
    /// </summary>
	public class GroupBarContextMenuItemClickCommand : ControlCommandBase<GroupBarContextMenuItemClickCommandBehavior, GroupBar>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GroupBarContextMenuItemClickCommandBehavior : CommandBehaviorBase<GroupBar>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, GroupBarContextMenuItemEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.ContextMenuItemClick += OnEventRaised;
        }
    }

	// GroupBarContextMenuItemClickCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GroupBarContextMenuItemClickCommandBehavior<T> : GroupBarContextMenuItemClickCommandBehavior
    { }
	#endregion

	#region GroupBarGroupBarItemAddedCommand
	// GroupBarGroupBarItemAddedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GroupBarGroupBarItemAddedCommand : ControlCommandBase<GroupBarGroupBarItemAddedCommandBehavior, GroupBar>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GroupBarGroupBarItemAddedCommandBehavior : CommandBehaviorBase<GroupBar>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, NotifyCollectionChangedEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.GroupBarItemAdded += OnEventRaised;
        }
    }

	// GroupBarGroupBarItemAddedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GroupBarGroupBarItemAddedCommandBehavior<T> : GroupBarGroupBarItemAddedCommandBehavior
    { }
	#endregion

	#region GroupBarGroupBarItemRemovedCommand
	// GroupBarGroupBarItemRemovedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GroupBarGroupBarItemRemovedCommand : ControlCommandBase<GroupBarGroupBarItemRemovedCommandBehavior, GroupBar>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GroupBarGroupBarItemRemovedCommandBehavior : CommandBehaviorBase<GroupBar>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, NotifyCollectionChangedEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.GroupBarItemRemoved += OnEventRaised;
        }
    }

	// GroupBarGroupBarItemRemovedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GroupBarGroupBarItemRemovedCommandBehavior<T> : GroupBarGroupBarItemRemovedCommandBehavior
    { }
	#endregion

	#region GroupBarCollapsedChangedCommand
	// GroupBarCollapsedChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GroupBarCollapsedChangedCommand : ControlCommandBase<GroupBarCollapsedChangedCommandBehavior, GroupBar>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GroupBarCollapsedChangedCommandBehavior : CommandBehaviorBase<GroupBar>
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
            TargetObject.CollapsedChanged += OnEventRaised;
        }
    }

	// GroupBarCollapsedChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GroupBarCollapsedChangedCommandBehavior<T> : GroupBarCollapsedChangedCommandBehavior
    { }
	#endregion

	#region GroupBarBeforeGroupBarItemPopupOpenedCommand
	// GroupBarBeforeGroupBarItemPopupOpenedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GroupBarBeforeGroupBarItemPopupOpenedCommand : ControlCommandBase<GroupBarBeforeGroupBarItemPopupOpenedCommandBehavior, GroupBar>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GroupBarBeforeGroupBarItemPopupOpenedCommandBehavior : CommandBehaviorBase<GroupBar>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, BeforeGroupBarItemPopupOpenedEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.BeforeGroupBarItemPopupOpened += OnEventRaised;
        }
    }

	// GroupBarBeforeGroupBarItemPopupOpenedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GroupBarBeforeGroupBarItemPopupOpenedCommandBehavior<T> : GroupBarBeforeGroupBarItemPopupOpenedCommandBehavior
    { }
	#endregion

	#region GroupBarAfterGroupBarItemPopupClosedCommand
	// GroupBarAfterGroupBarItemPopupClosedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GroupBarAfterGroupBarItemPopupClosedCommand : ControlCommandBase<GroupBarAfterGroupBarItemPopupClosedCommandBehavior, GroupBar>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GroupBarAfterGroupBarItemPopupClosedCommandBehavior : CommandBehaviorBase<GroupBar>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, EventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.AfterGroupBarItemPopupClosed += OnEventRaised;
        }
    }

	// GroupBarAfterGroupBarItemPopupClosedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GroupBarAfterGroupBarItemPopupClosedCommandBehavior<T> : GroupBarAfterGroupBarItemPopupClosedCommandBehavior
    { }
	#endregion

	#region GroupBarCollapsedWidthChangedCommand
	// GroupBarCollapsedWidthChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GroupBarCollapsedWidthChangedCommand : ControlCommandBase<GroupBarCollapsedWidthChangedCommandBehavior, GroupBar>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GroupBarCollapsedWidthChangedCommandBehavior : CommandBehaviorBase<GroupBar>
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
            TargetObject.CollapsedWidthChanged += OnEventRaised;
        }
    }

	// GroupBarCollapsedWidthChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GroupBarCollapsedWidthChangedCommandBehavior<T> : GroupBarCollapsedWidthChangedCommandBehavior
    { }
	#endregion

	#region GroupBarPopupResizeDirectionChangedCommand
	// GroupBarPopupResizeDirectionChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GroupBarPopupResizeDirectionChangedCommand : ControlCommandBase<GroupBarPopupResizeDirectionChangedCommandBehavior, GroupBar>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GroupBarPopupResizeDirectionChangedCommandBehavior : CommandBehaviorBase<GroupBar>
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
            TargetObject.PopupResizeDirectionChanged += OnEventRaised;
        }
    }

	// GroupBarPopupResizeDirectionChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GroupBarPopupResizeDirectionChangedCommandBehavior<T> : GroupBarPopupResizeDirectionChangedCommandBehavior
    { }
	#endregion

	#region GroupBarShowGripperChangedCommand
	// GroupBarShowGripperChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GroupBarShowGripperChangedCommand : ControlCommandBase<GroupBarShowGripperChangedCommandBehavior, GroupBar>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GroupBarShowGripperChangedCommandBehavior : CommandBehaviorBase<GroupBar>
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
            TargetObject.ShowGripperChanged += OnEventRaised;
        }
    }

	// GroupBarShowGripperChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GroupBarShowGripperChangedCommandBehavior<T> : GroupBarShowGripperChangedCommandBehavior
    { }
	#endregion

	#region GroupBarStackItemHostVisibilityChangedCommand
	// GroupBarStackItemHostVisibilityChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GroupBarStackItemHostVisibilityChangedCommand : ControlCommandBase<GroupBarStackItemHostVisibilityChangedCommandBehavior, GroupBar>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GroupBarStackItemHostVisibilityChangedCommandBehavior : CommandBehaviorBase<GroupBar>
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
            TargetObject.StackItemHostVisibilityChanged += OnEventRaised;
        }
    }

	// GroupBarStackItemHostVisibilityChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GroupBarStackItemHostVisibilityChangedCommandBehavior<T> : GroupBarStackItemHostVisibilityChangedCommandBehavior
    { }
	#endregion

	#region GroupBarNavigationPopupSizeChangedCommand
	// GroupBarNavigationPopupSizeChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GroupBarNavigationPopupSizeChangedCommand : ControlCommandBase<GroupBarNavigationPopupSizeChangedCommandBehavior, GroupBar>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GroupBarNavigationPopupSizeChangedCommandBehavior : CommandBehaviorBase<GroupBar>
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
            TargetObject.NavigationPopupSizeChanged += OnEventRaised;
        }
    }

	// GroupBarNavigationPopupSizeChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GroupBarNavigationPopupSizeChangedCommandBehavior<T> : GroupBarNavigationPopupSizeChangedCommandBehavior
    { }
	#endregion

	#region GroupBarCornerRadiusChangedCommand
	// GroupBarCornerRadiusChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GroupBarCornerRadiusChangedCommand : ControlCommandBase<GroupBarCornerRadiusChangedCommandBehavior, GroupBar>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GroupBarCornerRadiusChangedCommandBehavior : CommandBehaviorBase<GroupBar>
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
            TargetObject.CornerRadiusChanged += OnEventRaised;
        }
    }

	// GroupBarCornerRadiusChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GroupBarCornerRadiusChangedCommandBehavior<T> : GroupBarCornerRadiusChangedCommandBehavior
    { }
	#endregion

	#region GroupBarStackItemHostHeightChangedCommand
	// GroupBarStackItemHostHeightChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GroupBarStackItemHostHeightChangedCommand : ControlCommandBase<GroupBarStackItemHostHeightChangedCommandBehavior, GroupBar>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GroupBarStackItemHostHeightChangedCommandBehavior : CommandBehaviorBase<GroupBar>
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
            TargetObject.StackItemHostHeightChanged += OnEventRaised;
        }
    }

	// GroupBarStackItemHostHeightChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GroupBarStackItemHostHeightChangedCommandBehavior<T> : GroupBarStackItemHostHeightChangedCommandBehavior
    { }
	#endregion

	#region GroupBarGroupBarHeaderStyleChangedCommand
	// GroupBarGroupBarHeaderStyleChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GroupBarGroupBarHeaderStyleChangedCommand : ControlCommandBase<GroupBarGroupBarHeaderStyleChangedCommandBehavior, GroupBar>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GroupBarGroupBarHeaderStyleChangedCommandBehavior : CommandBehaviorBase<GroupBar>
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
            TargetObject.GroupBarHeaderStyleChanged += OnEventRaised;
        }
    }

	// GroupBarGroupBarHeaderStyleChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GroupBarGroupBarHeaderStyleChangedCommandBehavior<T> : GroupBarGroupBarHeaderStyleChangedCommandBehavior
    { }
	#endregion
}


