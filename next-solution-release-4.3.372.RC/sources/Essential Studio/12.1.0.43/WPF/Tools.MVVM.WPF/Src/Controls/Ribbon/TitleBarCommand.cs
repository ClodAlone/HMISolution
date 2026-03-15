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

	#region TitleBarHasVisibleContextTabGroupChangedCommand
	// TitleBarHasVisibleContextTabGroupChangedCommand
    /// <summary>
    /// 
    /// </summary>
    public class TitleBarHasVisibleContextTabGroupChangedCommand : ControlCommandBase<TitleBarHasVisibleContextTabGroupChangedCommandBehavior, Syncfusion.Windows.Tools.Controls.TitleBar>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TitleBarHasVisibleContextTabGroupChangedCommandBehavior : CommandBehaviorBase<Syncfusion.Windows.Tools.Controls.TitleBar>
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
            TargetObject.HasVisibleContextTabGroupChanged += OnEventRaised;
        }
    }

	// TitleBarHasVisibleContextTabGroupChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TitleBarHasVisibleContextTabGroupChangedCommandBehavior<T> : TitleBarHasVisibleContextTabGroupChangedCommandBehavior
    { }
	#endregion

	#region TitleBarRibbonIsVisibleChangedCommand
	// TitleBarRibbonIsVisibleChangedCommand
    /// <summary>
    /// 
    /// </summary>
    public class TitleBarRibbonIsVisibleChangedCommand : ControlCommandBase<TitleBarRibbonIsVisibleChangedCommandBehavior, Syncfusion.Windows.Tools.Controls.TitleBar>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TitleBarRibbonIsVisibleChangedCommandBehavior : CommandBehaviorBase<Syncfusion.Windows.Tools.Controls.TitleBar>
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
            TargetObject.RibbonIsVisibleChanged += OnEventRaised;
        }
    }

	// TitleBarRibbonIsVisibleChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TitleBarRibbonIsVisibleChangedCommandBehavior<T> : TitleBarRibbonIsVisibleChangedCommandBehavior
    { }
	#endregion
}


