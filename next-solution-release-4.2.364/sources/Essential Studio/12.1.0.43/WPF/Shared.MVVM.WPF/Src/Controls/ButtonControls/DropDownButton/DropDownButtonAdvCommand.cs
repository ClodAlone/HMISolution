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

	#region DropDownButtonAdvDropDownOpeningCommand
	// DropDownButtonAdvDropDownOpeningCommand
    /// <summary>
    /// 
    /// </summary>
	public class DropDownButtonAdvDropDownOpeningCommand : ControlCommandBase<DropDownButtonAdvDropDownOpeningCommandBehavior, DropDownButtonAdv>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DropDownButtonAdvDropDownOpeningCommandBehavior : CommandBehaviorBase<DropDownButtonAdv>
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
            TargetObject.DropDownOpening += OnEventRaised;
        }
    }

	// DropDownButtonAdvDropDownOpeningCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DropDownButtonAdvDropDownOpeningCommandBehavior<T> : DropDownButtonAdvDropDownOpeningCommandBehavior
    { }
	#endregion

	#region DropDownButtonAdvDropDownOpenedCommand
	// DropDownButtonAdvDropDownOpenedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DropDownButtonAdvDropDownOpenedCommand : ControlCommandBase<DropDownButtonAdvDropDownOpenedCommandBehavior, DropDownButtonAdv>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DropDownButtonAdvDropDownOpenedCommandBehavior : CommandBehaviorBase<DropDownButtonAdv>
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
            TargetObject.DropDownOpened += OnEventRaised;
        }
    }

	// DropDownButtonAdvDropDownOpenedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DropDownButtonAdvDropDownOpenedCommandBehavior<T> : DropDownButtonAdvDropDownOpenedCommandBehavior
    { }
	#endregion

	#region DropDownButtonAdvDropDownClosingCommand
	// DropDownButtonAdvDropDownClosingCommand
    /// <summary>
    /// 
    /// </summary>
	public class DropDownButtonAdvDropDownClosingCommand : ControlCommandBase<DropDownButtonAdvDropDownClosingCommandBehavior, DropDownButtonAdv>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DropDownButtonAdvDropDownClosingCommandBehavior : CommandBehaviorBase<DropDownButtonAdv>
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
            TargetObject.DropDownClosing += OnEventRaised;
        }
    }

	// DropDownButtonAdvDropDownClosingCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DropDownButtonAdvDropDownClosingCommandBehavior<T> : DropDownButtonAdvDropDownClosingCommandBehavior
    { }
	#endregion

	#region DropDownButtonAdvDropDownClosedCommand
	// DropDownButtonAdvDropDownClosedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DropDownButtonAdvDropDownClosedCommand : ControlCommandBase<DropDownButtonAdvDropDownClosedCommandBehavior, DropDownButtonAdv>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DropDownButtonAdvDropDownClosedCommandBehavior : CommandBehaviorBase<DropDownButtonAdv>
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
            TargetObject.DropDownClosed += OnEventRaised;
        }
    }

	// DropDownButtonAdvDropDownClosedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DropDownButtonAdvDropDownClosedCommandBehavior<T> : DropDownButtonAdvDropDownClosedCommandBehavior
    { }
	#endregion
}


