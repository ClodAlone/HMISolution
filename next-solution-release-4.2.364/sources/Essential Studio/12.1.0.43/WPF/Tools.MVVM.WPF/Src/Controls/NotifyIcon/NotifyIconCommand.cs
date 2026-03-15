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

	#region NotifyIconBalloonTipOpeningCommand
	// NotifyIconBalloonTipOpeningCommand
    /// <summary>
    /// 
    /// </summary>
	public class NotifyIconBalloonTipOpeningCommand : ControlCommandBase<NotifyIconBalloonTipOpeningCommandBehavior, NotifyIcon>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class NotifyIconBalloonTipOpeningCommandBehavior : CommandBehaviorBase<NotifyIcon>
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
            TargetObject.BalloonTipOpening += OnEventRaised;
        }
    }

	// NotifyIconBalloonTipOpeningCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NotifyIconBalloonTipOpeningCommandBehavior<T> : NotifyIconBalloonTipOpeningCommandBehavior
    { }
	#endregion

	#region NotifyIconBalloonTipOpenedCommand
	// NotifyIconBalloonTipOpenedCommand
    /// <summary>
    /// 
    /// </summary>
	public class NotifyIconBalloonTipOpenedCommand : ControlCommandBase<NotifyIconBalloonTipOpenedCommandBehavior, NotifyIcon>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class NotifyIconBalloonTipOpenedCommandBehavior : CommandBehaviorBase<NotifyIcon>
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
            TargetObject.BalloonTipOpened += OnEventRaised;
        }
    }

	// NotifyIconBalloonTipOpenedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NotifyIconBalloonTipOpenedCommandBehavior<T> : NotifyIconBalloonTipOpenedCommandBehavior
    { }
	#endregion

	#region NotifyIconBalloonTipHidingCommand
	// NotifyIconBalloonTipHidingCommand
    /// <summary>
    /// 
    /// </summary>
	public class NotifyIconBalloonTipHidingCommand : ControlCommandBase<NotifyIconBalloonTipHidingCommandBehavior, NotifyIcon>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class NotifyIconBalloonTipHidingCommandBehavior : CommandBehaviorBase<NotifyIcon>
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
            TargetObject.BalloonTipHiding += OnEventRaised;
        }
    }

	// NotifyIconBalloonTipHidingCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NotifyIconBalloonTipHidingCommandBehavior<T> : NotifyIconBalloonTipHidingCommandBehavior
    { }
	#endregion

	#region NotifyIconBalloonTipHiddenCommand
	// NotifyIconBalloonTipHiddenCommand
    /// <summary>
    /// 
    /// </summary>
	public class NotifyIconBalloonTipHiddenCommand : ControlCommandBase<NotifyIconBalloonTipHiddenCommandBehavior, NotifyIcon>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class NotifyIconBalloonTipHiddenCommandBehavior : CommandBehaviorBase<NotifyIcon>
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
            TargetObject.BalloonTipHidden += OnEventRaised;
        }
    }

	// NotifyIconBalloonTipHiddenCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NotifyIconBalloonTipHiddenCommandBehavior<T> : NotifyIconBalloonTipHiddenCommandBehavior
    { }
	#endregion

	#region NotifyIconCloseButtonClickCommand
	// NotifyIconCloseButtonClickCommand
    /// <summary>
    /// 
    /// </summary>
	public class NotifyIconCloseButtonClickCommand : ControlCommandBase<NotifyIconCloseButtonClickCommandBehavior, NotifyIcon>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class NotifyIconCloseButtonClickCommandBehavior : CommandBehaviorBase<NotifyIcon>
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
            TargetObject.CloseButtonClick += OnEventRaised;
        }
    }

	// NotifyIconCloseButtonClickCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NotifyIconCloseButtonClickCommandBehavior<T> : NotifyIconCloseButtonClickCommandBehavior
    { }
	#endregion

	#region NotifyIconClickCommand
	// NotifyIconClickCommand
    /// <summary>
    /// 
    /// </summary>
	public class NotifyIconClickCommand : ControlCommandBase<NotifyIconClickCommandBehavior, NotifyIcon>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class NotifyIconClickCommandBehavior : CommandBehaviorBase<NotifyIcon>
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
            TargetObject.Click += OnEventRaised;
        }
    }

	// NotifyIconClickCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NotifyIconClickCommandBehavior<T> : NotifyIconClickCommandBehavior
    { }
	#endregion

	#region NotifyIconIconClickCommand
	// NotifyIconIconClickCommand
    /// <summary>
    /// 
    /// </summary>
	public class NotifyIconIconClickCommand : ControlCommandBase<NotifyIconIconClickCommandBehavior, NotifyIcon>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class NotifyIconIconClickCommandBehavior : CommandBehaviorBase<NotifyIcon>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, MouseButtonEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.IconClick += OnEventRaised;
        }
    }

	// NotifyIconIconClickCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NotifyIconIconClickCommandBehavior<T> : NotifyIconIconClickCommandBehavior
    { }
	#endregion

	#region NotifyIconIconDoubleClickCommand
	// NotifyIconIconDoubleClickCommand
    /// <summary>
    /// 
    /// </summary>
	public class NotifyIconIconDoubleClickCommand : ControlCommandBase<NotifyIconIconDoubleClickCommandBehavior, NotifyIcon>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class NotifyIconIconDoubleClickCommandBehavior : CommandBehaviorBase<NotifyIcon>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, MouseButtonEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.IconDoubleClick += OnEventRaised;
        }
    }

	// NotifyIconIconDoubleClickCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NotifyIconIconDoubleClickCommandBehavior<T> : NotifyIconIconDoubleClickCommandBehavior
    { }
	#endregion
}


