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

	#region SplitButtonAdvClickCommand
	// SplitButtonAdvClickCommand
    /// <summary>
    /// 
    /// </summary>
	public class SplitButtonAdvClickCommand : ControlCommandBase<SplitButtonAdvClickCommandBehavior, SplitButtonAdv>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class SplitButtonAdvClickCommandBehavior : CommandBehaviorBase<SplitButtonAdv>
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

	// SplitButtonAdvClickCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SplitButtonAdvClickCommandBehavior<T> : SplitButtonAdvClickCommandBehavior
    { }
	#endregion
}


