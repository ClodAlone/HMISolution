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

	#region RibbonMenuItemFlowDirectionChangedCommand
	// RibbonMenuItemFlowDirectionChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class RibbonMenuItemFlowDirectionChangedCommand : ControlCommandBase<RibbonMenuItemFlowDirectionChangedCommandBehavior, RibbonMenuItem>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class RibbonMenuItemFlowDirectionChangedCommandBehavior : CommandBehaviorBase<RibbonMenuItem>
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
            TargetObject.FlowDirectionChanged += OnEventRaised;
        }
    }

	// RibbonMenuItemFlowDirectionChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RibbonMenuItemFlowDirectionChangedCommandBehavior<T> : RibbonMenuItemFlowDirectionChangedCommandBehavior
    { }
	#endregion

	#region RibbonMenuItemIconSizeChangedCommand
	// RibbonMenuItemIconSizeChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class RibbonMenuItemIconSizeChangedCommand : ControlCommandBase<RibbonMenuItemIconSizeChangedCommandBehavior, RibbonMenuItem>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class RibbonMenuItemIconSizeChangedCommandBehavior : CommandBehaviorBase<RibbonMenuItem>
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
            TargetObject.IconSizeChanged += OnEventRaised;
        }
    }

	// RibbonMenuItemIconSizeChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RibbonMenuItemIconSizeChangedCommandBehavior<T> : RibbonMenuItemIconSizeChangedCommandBehavior
    { }
	#endregion
}


