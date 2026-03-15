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

	#region ColorPickerPaletteColorChangedCommand
	// ColorPickerPaletteColorChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ColorPickerPaletteColorChangedCommand : ControlCommandBase<ColorPickerPaletteColorChangedCommandBehavior, ColorPickerPalette>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ColorPickerPaletteColorChangedCommandBehavior : CommandBehaviorBase<ColorPickerPalette>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.CommandParameter = e.NewValue;
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

	// ColorPickerPaletteColorChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ColorPickerPaletteColorChangedCommandBehavior<T> : ColorPickerPaletteColorChangedCommandBehavior
    { }
	#endregion

	#region ColorPickerPalettePopupHeightChangedCommand
	// ColorPickerPalettePopupHeightChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ColorPickerPalettePopupHeightChangedCommand : ControlCommandBase<ColorPickerPalettePopupHeightChangedCommandBehavior, ColorPickerPalette>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ColorPickerPalettePopupHeightChangedCommandBehavior : CommandBehaviorBase<ColorPickerPalette>
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
            TargetObject.PopupHeightChanged += OnEventRaised;
        }
    }

	// ColorPickerPalettePopupHeightChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ColorPickerPalettePopupHeightChangedCommandBehavior<T> : ColorPickerPalettePopupHeightChangedCommandBehavior
    { }
	#endregion

	#region ColorPickerPaletteDropDownOpenedCommand
	// ColorPickerPaletteDropDownOpenedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ColorPickerPaletteDropDownOpenedCommand : ControlCommandBase<ColorPickerPaletteDropDownOpenedCommandBehavior, ColorPickerPalette>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ColorPickerPaletteDropDownOpenedCommandBehavior : CommandBehaviorBase<ColorPickerPalette>
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
            TargetObject.DropDownOpened += OnEventRaised;
        }
    }

	// ColorPickerPaletteDropDownOpenedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ColorPickerPaletteDropDownOpenedCommandBehavior<T> : ColorPickerPaletteDropDownOpenedCommandBehavior
    { }
	#endregion
}


