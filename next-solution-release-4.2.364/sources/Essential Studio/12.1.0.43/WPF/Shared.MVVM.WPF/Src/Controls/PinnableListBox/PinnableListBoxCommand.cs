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

	#region PinnableListBoxPinStatusChangedCommand
	// PinnableListBoxPinStatusChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class PinnableListBoxPinStatusChangedCommand : ControlCommandBase<PinnableListBoxPinStatusChangedCommandBehavior, PinnableListBox>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class PinnableListBoxPinStatusChangedCommandBehavior : CommandBehaviorBase<PinnableListBox>
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
            TargetObject.PinStatusChangedEvent += OnEventRaised;
        }
    }

	// PinnableListBoxPinStatusChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PinnableListBoxPinStatusChangedCommandBehavior<T> : PinnableListBoxPinStatusChangedCommandBehavior
    { }
	#endregion

	
}


