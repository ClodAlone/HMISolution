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

	#region TileViewItemStateChangedCommand
	// TileViewItemStateChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TileViewItemStateChangedCommand : ControlCommandBase<TileViewItemStateChangedCommandBehavior, TileViewItem>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TileViewItemStateChangedCommandBehavior : CommandBehaviorBase<TileViewItem>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, TileViewEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.StateChanged += OnEventRaised;
        }
    }

	// TileViewItemStateChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TileViewItemStateChangedCommandBehavior<T> : TileViewItemStateChangedCommandBehavior
    { }
	#endregion

	#region TileViewItemStateChangingCommand
	// TileViewItemStateChangingCommand
    /// <summary>
    /// 
    /// </summary>
	public class TileViewItemStateChangingCommand : ControlCommandBase<TileViewItemStateChangingCommandBehavior, TileViewItem>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TileViewItemStateChangingCommandBehavior : CommandBehaviorBase<TileViewItem>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, TileViewCancelEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.StateChanging += OnEventRaised;
        }
    }

	// TileViewItemStateChangingCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TileViewItemStateChangingCommandBehavior<T> : TileViewItemStateChangingCommandBehavior
    { }
	#endregion

	#region TileViewItemSelectedCommand
	// TileViewItemSelectedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TileViewItemSelectedCommand : ControlCommandBase<TileViewItemSelectedCommandBehavior, TileViewItem>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TileViewItemSelectedCommandBehavior : CommandBehaviorBase<TileViewItem>
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

	// TileViewItemSelectedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TileViewItemSelectedCommandBehavior<T> : TileViewItemSelectedCommandBehavior
    { }
	#endregion
}


