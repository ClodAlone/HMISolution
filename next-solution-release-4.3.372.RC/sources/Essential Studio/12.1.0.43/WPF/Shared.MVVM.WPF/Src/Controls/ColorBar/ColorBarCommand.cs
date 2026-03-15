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

	#region ColorBarSliderValueChangedCommand
	// ColorBarSliderValueChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ColorBarSliderValueChangedCommand : ControlCommandBase<ColorBarSliderValueChangedCommandBehavior, ColorBar>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ColorBarSliderValueChangedCommandBehavior : CommandBehaviorBase<ColorBar>
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
            TargetObject.SliderValueChanged += OnEventRaised;
        }
    }

	// ColorBarSliderValueChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ColorBarSliderValueChangedCommandBehavior<T> : ColorBarSliderValueChangedCommandBehavior
    { }
	#endregion

	#region ColorBarColorChangedCommand
	// ColorBarColorChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ColorBarColorChangedCommand : ControlCommandBase<ColorBarColorChangedCommandBehavior, ColorBar>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ColorBarColorChangedCommandBehavior : CommandBehaviorBase<ColorBar>
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
            TargetObject.ColorChanged += OnEventRaised;
        }
    }

	// ColorBarColorChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ColorBarColorChangedCommandBehavior<T> : ColorBarColorChangedCommandBehavior
    { }
	#endregion
}


