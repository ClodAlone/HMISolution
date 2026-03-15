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

	#region WizardPageSelectingCommand
	// WizardPageSelectingCommand
    /// <summary>
    /// 
    /// </summary>
	public class WizardPageSelectingCommand : ControlCommandBase<WizardPageSelectingCommandBehavior, WizardPage>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class WizardPageSelectingCommandBehavior : CommandBehaviorBase<WizardPage>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, CancelRoutedEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.Selecting += OnEventRaised;
        }
    }

	// WizardPageSelectingCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WizardPageSelectingCommandBehavior<T> : WizardPageSelectingCommandBehavior
    { }
	#endregion

	#region WizardPageUnselectingCommand
	// WizardPageUnselectingCommand
    /// <summary>
    /// 
    /// </summary>
	public class WizardPageUnselectingCommand : ControlCommandBase<WizardPageUnselectingCommandBehavior, WizardPage>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class WizardPageUnselectingCommandBehavior : CommandBehaviorBase<WizardPage>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, CancelRoutedEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.Unselecting += OnEventRaised;
        }
    }

	// WizardPageUnselectingCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WizardPageUnselectingCommandBehavior<T> : WizardPageUnselectingCommandBehavior
    { }
	#endregion

	#region WizardPageSelectedCommand
	// WizardPageSelectedCommand
    /// <summary>
    /// 
    /// </summary>
	public class WizardPageSelectedCommand : ControlCommandBase<WizardPageSelectedCommandBehavior, WizardPage>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class WizardPageSelectedCommandBehavior : CommandBehaviorBase<WizardPage>
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

	// WizardPageSelectedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WizardPageSelectedCommandBehavior<T> : WizardPageSelectedCommandBehavior
    { }
	#endregion

	#region WizardPageUnselectedCommand
	// WizardPageUnselectedCommand
    /// <summary>
    /// 
    /// </summary>
	public class WizardPageUnselectedCommand : ControlCommandBase<WizardPageUnselectedCommandBehavior, WizardPage>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class WizardPageUnselectedCommandBehavior : CommandBehaviorBase<WizardPage>
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

	// WizardPageUnselectedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WizardPageUnselectedCommandBehavior<T> : WizardPageUnselectedCommandBehavior
    { }
	#endregion
}


