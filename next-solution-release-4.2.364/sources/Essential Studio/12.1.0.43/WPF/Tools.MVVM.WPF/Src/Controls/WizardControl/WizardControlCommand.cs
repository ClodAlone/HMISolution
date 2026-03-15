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

	#region WizardControlFinishCommand
	// WizardControlFinishCommand
    /// <summary>
    /// 
    /// </summary>
	public class WizardControlFinishCommand : ControlCommandBase<WizardControlFinishCommandBehavior, WizardControl>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class WizardControlFinishCommandBehavior : CommandBehaviorBase<WizardControl>
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
            TargetObject.Finish += OnEventRaised;
        }
    }

	// WizardControlFinishCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WizardControlFinishCommandBehavior<T> : WizardControlFinishCommandBehavior
    { }
	#endregion

	#region WizardControlCancelCommand
	// WizardControlCancelCommand
    /// <summary>
    /// 
    /// </summary>
	public class WizardControlCancelCommand : ControlCommandBase<WizardControlCancelCommandBehavior, WizardControl>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class WizardControlCancelCommandBehavior : CommandBehaviorBase<WizardControl>
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
            TargetObject.Cancel += OnEventRaised;
        }
    }

	// WizardControlCancelCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WizardControlCancelCommandBehavior<T> : WizardControlCancelCommandBehavior
    { }
	#endregion

	#region WizardControlHelpCommand
	// WizardControlHelpCommand
    /// <summary>
    /// 
    /// </summary>
	public class WizardControlHelpCommand : ControlCommandBase<WizardControlHelpCommandBehavior, WizardControl>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class WizardControlHelpCommandBehavior : CommandBehaviorBase<WizardControl>
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
            TargetObject.Help += OnEventRaised;
        }
    }

	// WizardControlHelpCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WizardControlHelpCommandBehavior<T> : WizardControlHelpCommandBehavior
    { }
	#endregion

	#region WizardControlNextCommand
	// WizardControlNextCommand
    /// <summary>
    /// 
    /// </summary>
	public class WizardControlNextCommand : ControlCommandBase<WizardControlNextCommandBehavior, WizardControl>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class WizardControlNextCommandBehavior : CommandBehaviorBase<WizardControl>
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
            TargetObject.Next += OnEventRaised;
        }
    }

	// WizardControlNextCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WizardControlNextCommandBehavior<T> : WizardControlNextCommandBehavior
    { }
	#endregion

	#region WizardControlSelectedPageChangingCommand
	// WizardControlSelectedPageChangingCommand
    /// <summary>
    /// 
    /// </summary>
	public class WizardControlSelectedPageChangingCommand : ControlCommandBase<WizardControlSelectedPageChangingCommandBehavior, WizardControl>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class WizardControlSelectedPageChangingCommandBehavior : CommandBehaviorBase<WizardControl>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, WizardPageSelectionChangeEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.SelectedPageChanging += OnEventRaised;
        }
    }

	// WizardControlSelectedPageChangingCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WizardControlSelectedPageChangingCommandBehavior<T> : WizardControlSelectedPageChangingCommandBehavior
    { }
	#endregion

	#region WizardControlSelectedPageChangedCommand
	// WizardControlSelectedPageChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class WizardControlSelectedPageChangedCommand : ControlCommandBase<WizardControlSelectedPageChangedCommandBehavior, WizardControl>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class WizardControlSelectedPageChangedCommandBehavior : CommandBehaviorBase<WizardControl>
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
            TargetObject.SelectedPageChanged += OnEventRaised;
        }
    }

	// WizardControlSelectedPageChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WizardControlSelectedPageChangedCommandBehavior<T> : WizardControlSelectedPageChangedCommandBehavior
    { }
	#endregion
}


