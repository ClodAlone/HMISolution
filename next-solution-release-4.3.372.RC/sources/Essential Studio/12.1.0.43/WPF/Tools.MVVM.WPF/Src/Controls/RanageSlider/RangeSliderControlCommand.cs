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

	#region RangeSliderControlRangeChangedCommand
	// RangeSliderControlRangeChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class RangeSliderControlRangeChangedCommand : ControlCommandBase<RangeSliderControlRangeChangedCommandBehavior, RangeSliderControl>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class RangeSliderControlRangeChangedCommandBehavior : CommandBehaviorBase<RangeSliderControl>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, RoutedPropertyChangedEventArgs<DoubleRange> e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.RangeChanged += OnEventRaised;
        }
    }

	// RangeSliderControlRangeChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RangeSliderControlRangeChangedCommandBehavior<T> : RangeSliderControlRangeChangedCommandBehavior
    { }
	#endregion

	#region RangeSliderControlValueChangedCommand
	// RangeSliderControlValueChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class RangeSliderControlValueChangedCommand : ControlCommandBase<RangeSliderControlValueChangedCommandBehavior, RangeSliderControl>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class RangeSliderControlValueChangedCommandBehavior : CommandBehaviorBase<RangeSliderControl>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, RoutedPropertyChangedEventArgs<Double> e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.ValueChanged += OnEventRaised;
        }
    }

	// RangeSliderControlValueChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RangeSliderControlValueChangedCommandBehavior<T> : RangeSliderControlValueChangedCommandBehavior
    { }
	#endregion
}


