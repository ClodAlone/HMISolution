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

	#region HierarchyNavigatorHierarchyNavigatorRefreshButtonClickCommand
	// HierarchyNavigatorHierarchyNavigatorRefreshButtonClickCommand
    /// <summary>
    /// 
    /// </summary>
	public class HierarchyNavigatorHierarchyNavigatorRefreshButtonClickCommand : ControlCommandBase<HierarchyNavigatorHierarchyNavigatorRefreshButtonClickCommandBehavior, HierarchyNavigator>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class HierarchyNavigatorHierarchyNavigatorRefreshButtonClickCommandBehavior : CommandBehaviorBase<HierarchyNavigator>
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

	// HierarchyNavigatorHierarchyNavigatorRefreshButtonClickCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HierarchyNavigatorHierarchyNavigatorRefreshButtonClickCommandBehavior<T> : HierarchyNavigatorHierarchyNavigatorRefreshButtonClickCommandBehavior
    { }
	#endregion

	#region HierarchyNavigatorHierarchyNavigatorSelectedItemChangedCommand
	// HierarchyNavigatorHierarchyNavigatorSelectedItemChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class HierarchyNavigatorHierarchyNavigatorSelectedItemChangedCommand : ControlCommandBase<HierarchyNavigatorHierarchyNavigatorSelectedItemChangedCommandBehavior, HierarchyNavigator>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class HierarchyNavigatorHierarchyNavigatorSelectedItemChangedCommandBehavior : CommandBehaviorBase<HierarchyNavigator>
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

	// HierarchyNavigatorHierarchyNavigatorSelectedItemChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HierarchyNavigatorHierarchyNavigatorSelectedItemChangedCommandBehavior<T> : HierarchyNavigatorHierarchyNavigatorSelectedItemChangedCommandBehavior
    { }
	#endregion

	#region HierarchyNavigatorNavigationPopupOpenedCommand
	// HierarchyNavigatorNavigationPopupOpenedCommand
    /// <summary>
    /// 
    /// </summary>
	public class HierarchyNavigatorNavigationPopupOpenedCommand : ControlCommandBase<HierarchyNavigatorNavigationPopupOpenedCommandBehavior, HierarchyNavigator>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class HierarchyNavigatorNavigationPopupOpenedCommandBehavior : CommandBehaviorBase<HierarchyNavigator>
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

	// HierarchyNavigatorNavigationPopupOpenedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HierarchyNavigatorNavigationPopupOpenedCommandBehavior<T> : HierarchyNavigatorNavigationPopupOpenedCommandBehavior
    { }
	#endregion

	#region HierarchyNavigatorNavigationPopupOpeningCommand
	// HierarchyNavigatorNavigationPopupOpeningCommand
    /// <summary>
    /// 
    /// </summary>
	public class HierarchyNavigatorNavigationPopupOpeningCommand : ControlCommandBase<HierarchyNavigatorNavigationPopupOpeningCommandBehavior, HierarchyNavigator>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class HierarchyNavigatorNavigationPopupOpeningCommandBehavior : CommandBehaviorBase<HierarchyNavigator>
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

	// HierarchyNavigatorNavigationPopupOpeningCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HierarchyNavigatorNavigationPopupOpeningCommandBehavior<T> : HierarchyNavigatorNavigationPopupOpeningCommandBehavior
    { }
	#endregion

	#region HierarchyNavigatorNavigationPopupClosingCommand
	// HierarchyNavigatorNavigationPopupClosingCommand
    /// <summary>
    /// 
    /// </summary>
	public class HierarchyNavigatorNavigationPopupClosingCommand : ControlCommandBase<HierarchyNavigatorNavigationPopupClosingCommandBehavior, HierarchyNavigator>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class HierarchyNavigatorNavigationPopupClosingCommandBehavior : CommandBehaviorBase<HierarchyNavigator>
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

	// HierarchyNavigatorNavigationPopupClosingCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HierarchyNavigatorNavigationPopupClosingCommandBehavior<T> : HierarchyNavigatorNavigationPopupClosingCommandBehavior
    { }
	#endregion

	#region HierarchyNavigatorNavigationPopupClosedCommand
	// HierarchyNavigatorNavigationPopupClosedCommand
    /// <summary>
    /// 
    /// </summary>
	public class HierarchyNavigatorNavigationPopupClosedCommand : ControlCommandBase<HierarchyNavigatorNavigationPopupClosedCommandBehavior, HierarchyNavigator>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class HierarchyNavigatorNavigationPopupClosedCommandBehavior : CommandBehaviorBase<HierarchyNavigator>
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

	// HierarchyNavigatorNavigationPopupClosedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HierarchyNavigatorNavigationPopupClosedCommandBehavior<T> : HierarchyNavigatorNavigationPopupClosedCommandBehavior
    { }
	#endregion
}


