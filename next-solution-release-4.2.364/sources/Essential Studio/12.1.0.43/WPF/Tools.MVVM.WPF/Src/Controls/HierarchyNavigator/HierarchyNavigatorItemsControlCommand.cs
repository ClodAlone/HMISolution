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

	#region HierarchyNavigatorItemsControlHierarchyNavigatorRefreshButtonClickCommand
	// HierarchyNavigatorItemsControlHierarchyNavigatorRefreshButtonClickCommand
    /// <summary>
    /// 
    /// </summary>
	public class HierarchyNavigatorItemsControlHierarchyNavigatorRefreshButtonClickCommand : ControlCommandBase<HierarchyNavigatorItemsControlHierarchyNavigatorRefreshButtonClickCommandBehavior, HierarchyNavigatorItemsControl>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class HierarchyNavigatorItemsControlHierarchyNavigatorRefreshButtonClickCommandBehavior : CommandBehaviorBase<HierarchyNavigatorItemsControl>
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
            TargetObject.HierarchyNavigatorRefreshButtonClick += OnEventRaised;
        }
    }

	// HierarchyNavigatorItemsControlHierarchyNavigatorRefreshButtonClickCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HierarchyNavigatorItemsControlHierarchyNavigatorRefreshButtonClickCommandBehavior<T> : HierarchyNavigatorItemsControlHierarchyNavigatorRefreshButtonClickCommandBehavior
    { }
	#endregion

	#region HierarchyNavigatorItemsControlHierarchyNavigatorSelectedItemChangedCommand
	// HierarchyNavigatorItemsControlHierarchyNavigatorSelectedItemChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class HierarchyNavigatorItemsControlHierarchyNavigatorSelectedItemChangedCommand : ControlCommandBase<HierarchyNavigatorItemsControlHierarchyNavigatorSelectedItemChangedCommandBehavior, HierarchyNavigatorItemsControl>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class HierarchyNavigatorItemsControlHierarchyNavigatorSelectedItemChangedCommandBehavior : CommandBehaviorBase<HierarchyNavigatorItemsControl>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, HierarchyNavigatorSelectedItemChangedEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.HierarchyNavigatorSelectedItemChanged += OnEventRaised;
        }
    }

	// HierarchyNavigatorItemsControlHierarchyNavigatorSelectedItemChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HierarchyNavigatorItemsControlHierarchyNavigatorSelectedItemChangedCommandBehavior<T> : HierarchyNavigatorItemsControlHierarchyNavigatorSelectedItemChangedCommandBehavior
    { }
	#endregion

	#region HierarchyNavigatorItemsControlNavigationPopupOpeningCommand
	// HierarchyNavigatorItemsControlNavigationPopupOpeningCommand
    /// <summary>
    /// 
    /// </summary>
	public class HierarchyNavigatorItemsControlNavigationPopupOpeningCommand : ControlCommandBase<HierarchyNavigatorItemsControlNavigationPopupOpeningCommandBehavior, HierarchyNavigatorItemsControl>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class HierarchyNavigatorItemsControlNavigationPopupOpeningCommandBehavior : CommandBehaviorBase<HierarchyNavigatorItemsControl>
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
            TargetObject.NavigationPopupOpening += OnEventRaised;
        }
    }

	// HierarchyNavigatorItemsControlNavigationPopupOpeningCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HierarchyNavigatorItemsControlNavigationPopupOpeningCommandBehavior<T> : HierarchyNavigatorItemsControlNavigationPopupOpeningCommandBehavior
    { }
	#endregion

	#region HierarchyNavigatorItemsControlNavigationPopupOpenedCommand
	// HierarchyNavigatorItemsControlNavigationPopupOpenedCommand
    /// <summary>
    /// 
    /// </summary>
	public class HierarchyNavigatorItemsControlNavigationPopupOpenedCommand : ControlCommandBase<HierarchyNavigatorItemsControlNavigationPopupOpenedCommandBehavior, HierarchyNavigatorItemsControl>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class HierarchyNavigatorItemsControlNavigationPopupOpenedCommandBehavior : CommandBehaviorBase<HierarchyNavigatorItemsControl>
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
            TargetObject.NavigationPopupOpened += OnEventRaised;
        }
    }

	// HierarchyNavigatorItemsControlNavigationPopupOpenedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HierarchyNavigatorItemsControlNavigationPopupOpenedCommandBehavior<T> : HierarchyNavigatorItemsControlNavigationPopupOpenedCommandBehavior
    { }
	#endregion

	#region HierarchyNavigatorItemsControlNavigationPopupClosingCommand
	// HierarchyNavigatorItemsControlNavigationPopupClosingCommand
    /// <summary>
    /// 
    /// </summary>
	public class HierarchyNavigatorItemsControlNavigationPopupClosingCommand : ControlCommandBase<HierarchyNavigatorItemsControlNavigationPopupClosingCommandBehavior, HierarchyNavigatorItemsControl>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class HierarchyNavigatorItemsControlNavigationPopupClosingCommandBehavior : CommandBehaviorBase<HierarchyNavigatorItemsControl>
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
            TargetObject.NavigationPopupClosing += OnEventRaised;
        }
    }

	// HierarchyNavigatorItemsControlNavigationPopupClosingCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HierarchyNavigatorItemsControlNavigationPopupClosingCommandBehavior<T> : HierarchyNavigatorItemsControlNavigationPopupClosingCommandBehavior
    { }
	#endregion

	#region HierarchyNavigatorItemsControlNavigationPopupClosedCommand
	// HierarchyNavigatorItemsControlNavigationPopupClosedCommand
    /// <summary>
    /// 
    /// </summary>
	public class HierarchyNavigatorItemsControlNavigationPopupClosedCommand : ControlCommandBase<HierarchyNavigatorItemsControlNavigationPopupClosedCommandBehavior, HierarchyNavigatorItemsControl>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class HierarchyNavigatorItemsControlNavigationPopupClosedCommandBehavior : CommandBehaviorBase<HierarchyNavigatorItemsControl>
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
            TargetObject.NavigationPopupClosed += OnEventRaised;
        }
    }

	// HierarchyNavigatorItemsControlNavigationPopupClosedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HierarchyNavigatorItemsControlNavigationPopupClosedCommandBehavior<T> : HierarchyNavigatorItemsControlNavigationPopupClosedCommandBehavior
    { }
	#endregion
}


