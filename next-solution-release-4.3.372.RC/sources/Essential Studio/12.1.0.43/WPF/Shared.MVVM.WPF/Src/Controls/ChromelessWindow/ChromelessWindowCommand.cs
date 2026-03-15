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

	#region ChromelessWindowIsGlassActiveChangedCommand
	// ChromelessWindowIsGlassActiveChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ChromelessWindowIsGlassActiveChangedCommand : ControlCommandBase<ChromelessWindowIsGlassActiveChangedCommandBehavior, ChromelessWindow>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ChromelessWindowIsGlassActiveChangedCommandBehavior : CommandBehaviorBase<ChromelessWindow>
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
            TargetObject.IsGlassActiveChanged += OnEventRaised;
        }
    }

	// ChromelessWindowIsGlassActiveChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ChromelessWindowIsGlassActiveChangedCommandBehavior<T> : ChromelessWindowIsGlassActiveChangedCommandBehavior
    { }
	#endregion
}


