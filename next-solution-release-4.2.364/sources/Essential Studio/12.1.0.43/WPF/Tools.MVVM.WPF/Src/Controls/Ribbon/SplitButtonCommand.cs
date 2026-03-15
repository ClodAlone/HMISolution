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

	#region SplitButtonHitTestAreaChangedCommand
	// SplitButtonHitTestAreaChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class SplitButtonHitTestAreaChangedCommand : ControlCommandBase<SplitButtonHitTestAreaChangedCommandBehavior, SplitButton>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class SplitButtonHitTestAreaChangedCommandBehavior : CommandBehaviorBase<SplitButton>
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
            TargetObject.HitTestAreaChanged += OnEventRaised;
        }
    }

	// SplitButtonHitTestAreaChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SplitButtonHitTestAreaChangedCommandBehavior<T> : SplitButtonHitTestAreaChangedCommandBehavior
    { }
	#endregion

	#region SplitButtonClickCommand
	// SplitButtonClickCommand
    /// <summary>
    /// 
    /// </summary>
	public class SplitButtonClickCommand : ControlCommandBase<SplitButtonClickCommandBehavior, SplitButton>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class SplitButtonClickCommandBehavior : CommandBehaviorBase<SplitButton>
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

	// SplitButtonClickCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SplitButtonClickCommandBehavior<T> : SplitButtonClickCommandBehavior
    { }
	#endregion
}


