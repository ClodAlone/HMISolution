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
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.Windows.Shared;


namespace Syncfusion.Windows.Tools.MVVM
{   

	#region DropDownMenuItemIsCheckedChangedCommand
	// DropDownMenuItemIsCheckedChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DropDownMenuItemIsCheckedChangedCommand : ControlCommandBase<DropDownMenuItemIsCheckedChangedCommandBehavior, DropDownMenuItem>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DropDownMenuItemIsCheckedChangedCommandBehavior : CommandBehaviorBase<DropDownMenuItem>
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
            TargetObject.IsCheckedChanged += OnEventRaised;
        }
    }

	// DropDownMenuItemIsCheckedChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DropDownMenuItemIsCheckedChangedCommandBehavior<T> : DropDownMenuItemIsCheckedChangedCommandBehavior
    { }
	#endregion

	#region DropDownMenuItemClickCommand
	// DropDownMenuItemClickCommand
    /// <summary>
    /// 
    /// </summary>
	public class DropDownMenuItemClickCommand : ControlCommandBase<DropDownMenuItemClickCommandBehavior, DropDownMenuItem>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DropDownMenuItemClickCommandBehavior : CommandBehaviorBase<DropDownMenuItem>
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

	// DropDownMenuItemClickCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DropDownMenuItemClickCommandBehavior<T> : DropDownMenuItemClickCommandBehavior
    { }
	#endregion
}


