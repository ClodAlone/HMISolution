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

	#region RibbonBarLauncherClickCommand
	// RibbonBarLauncherClickCommand
    /// <summary>
    /// 
    /// </summary>
	public class RibbonBarLauncherClickCommand : ControlCommandBase<RibbonBarLauncherClickCommandBehavior, RibbonBar>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class RibbonBarLauncherClickCommandBehavior : CommandBehaviorBase<RibbonBar>
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
            TargetObject.LauncherClick += OnEventRaised;
        }
    }

	// RibbonBarLauncherClickCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RibbonBarLauncherClickCommandBehavior<T> : RibbonBarLauncherClickCommandBehavior
    { }
	#endregion

	#region RibbonBarCollapseImageChangedCommand
	// RibbonBarCollapseImageChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class RibbonBarCollapseImageChangedCommand : ControlCommandBase<RibbonBarCollapseImageChangedCommandBehavior, RibbonBar>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class RibbonBarCollapseImageChangedCommandBehavior : CommandBehaviorBase<RibbonBar>
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
            TargetObject.CollapseImageChanged += OnEventRaised;
        }
    }

	// RibbonBarCollapseImageChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RibbonBarCollapseImageChangedCommandBehavior<T> : RibbonBarCollapseImageChangedCommandBehavior
    { }
	#endregion

	#region RibbonBarPanelStateChangedCommand
	// RibbonBarPanelStateChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class RibbonBarPanelStateChangedCommand : ControlCommandBase<RibbonBarPanelStateChangedCommandBehavior, RibbonBar>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class RibbonBarPanelStateChangedCommandBehavior : CommandBehaviorBase<RibbonBar>
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
            TargetObject.PanelStateChanged += OnEventRaised;
        }
    }

	// RibbonBarPanelStateChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RibbonBarPanelStateChangedCommandBehavior<T> : RibbonBarPanelStateChangedCommandBehavior
    { }
	#endregion

	#region RibbonBarHeaderChangedCommand
	// RibbonBarHeaderChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class RibbonBarHeaderChangedCommand : ControlCommandBase<RibbonBarHeaderChangedCommandBehavior, RibbonBar>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class RibbonBarHeaderChangedCommandBehavior : CommandBehaviorBase<RibbonBar>
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
            TargetObject.HeaderChanged += OnEventRaised;
        }
    }

	// RibbonBarHeaderChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RibbonBarHeaderChangedCommandBehavior<T> : RibbonBarHeaderChangedCommandBehavior
    { }
	#endregion
}


