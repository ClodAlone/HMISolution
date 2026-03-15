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

	#region BalloonTipBalloonTipOpeningCommand
	// BalloonTipBalloonTipOpeningCommand
    /// <summary>
    /// 
    /// </summary>
	public class BalloonTipBalloonTipOpeningCommand : ControlCommandBase<BalloonTipBalloonTipOpeningCommandBehavior, BalloonTip>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class BalloonTipBalloonTipOpeningCommandBehavior : CommandBehaviorBase<BalloonTip>
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

	// BalloonTipBalloonTipOpeningCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BalloonTipBalloonTipOpeningCommandBehavior<T> : BalloonTipBalloonTipOpeningCommandBehavior
    { }
	#endregion

	#region BalloonTipBalloonTipOpenedCommand
	// BalloonTipBalloonTipOpenedCommand
    /// <summary>
    /// 
    /// </summary>
	public class BalloonTipBalloonTipOpenedCommand : ControlCommandBase<BalloonTipBalloonTipOpenedCommandBehavior, BalloonTip>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class BalloonTipBalloonTipOpenedCommandBehavior : CommandBehaviorBase<BalloonTip>
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

	// BalloonTipBalloonTipOpenedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BalloonTipBalloonTipOpenedCommandBehavior<T> : BalloonTipBalloonTipOpenedCommandBehavior
    { }
	#endregion

	#region BalloonTipBalloonTipHidingCommand
	// BalloonTipBalloonTipHidingCommand
    /// <summary>
    /// 
    /// </summary>
	public class BalloonTipBalloonTipHidingCommand : ControlCommandBase<BalloonTipBalloonTipHidingCommandBehavior, BalloonTip>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class BalloonTipBalloonTipHidingCommandBehavior : CommandBehaviorBase<BalloonTip>
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

	// BalloonTipBalloonTipHidingCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BalloonTipBalloonTipHidingCommandBehavior<T> : BalloonTipBalloonTipHidingCommandBehavior
    { }
	#endregion

	#region BalloonTipBalloonTipHiddenCommand
	// BalloonTipBalloonTipHiddenCommand
    /// <summary>
    /// 
    /// </summary>
	public class BalloonTipBalloonTipHiddenCommand : ControlCommandBase<BalloonTipBalloonTipHiddenCommandBehavior, BalloonTip>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class BalloonTipBalloonTipHiddenCommandBehavior : CommandBehaviorBase<BalloonTip>
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

	// BalloonTipBalloonTipHiddenCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BalloonTipBalloonTipHiddenCommandBehavior<T> : BalloonTipBalloonTipHiddenCommandBehavior
    { }
	#endregion

	#region BalloonTipCloseButtonClickCommand
	// BalloonTipCloseButtonClickCommand
    /// <summary>
    /// 
    /// </summary>
	public class BalloonTipCloseButtonClickCommand : ControlCommandBase<BalloonTipCloseButtonClickCommandBehavior, BalloonTip>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class BalloonTipCloseButtonClickCommandBehavior : CommandBehaviorBase<BalloonTip>
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

	// BalloonTipCloseButtonClickCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BalloonTipCloseButtonClickCommandBehavior<T> : BalloonTipCloseButtonClickCommandBehavior
    { }
	#endregion

	#region BalloonTipClickCommand
	// BalloonTipClickCommand
    /// <summary>
    /// 
    /// </summary>
	public class BalloonTipClickCommand : ControlCommandBase<BalloonTipClickCommandBehavior, BalloonTip>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class BalloonTipClickCommandBehavior : CommandBehaviorBase<BalloonTip>
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

	// BalloonTipClickCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BalloonTipClickCommandBehavior<T> : BalloonTipClickCommandBehavior
    { }
	#endregion
}


