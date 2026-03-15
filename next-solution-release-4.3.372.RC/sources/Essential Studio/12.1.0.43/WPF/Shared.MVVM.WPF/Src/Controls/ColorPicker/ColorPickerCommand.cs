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

	#region ColorPickerColorChangedCommand
	// ColorPickerColorChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ColorPickerColorChangedCommand : ControlCommandBase<ColorPickerColorChangedCommandBehavior, ColorPicker>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ColorPickerColorChangedCommandBehavior : CommandBehaviorBase<ColorPicker>
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

	// ColorPickerColorChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ColorPickerColorChangedCommandBehavior<T> : ColorPickerColorChangedCommandBehavior
    { }
	#endregion

	#region ColorPickerEnableToolTipChangedCommand
	// ColorPickerEnableToolTipChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ColorPickerEnableToolTipChangedCommand : ControlCommandBase<ColorPickerEnableToolTipChangedCommandBehavior, ColorPicker>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ColorPickerEnableToolTipChangedCommandBehavior : CommandBehaviorBase<ColorPicker>
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
            TargetObject.EnableToolTipChanged += OnEventRaised;
        }
    }

	// ColorPickerEnableToolTipChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ColorPickerEnableToolTipChangedCommandBehavior<T> : ColorPickerEnableToolTipChangedCommandBehavior
    { }
	#endregion

	#region ColorPickerSelectedBrushChangedCommand
	// ColorPickerSelectedBrushChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ColorPickerSelectedBrushChangedCommand : ControlCommandBase<ColorPickerSelectedBrushChangedCommandBehavior, ColorPicker>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ColorPickerSelectedBrushChangedCommandBehavior : CommandBehaviorBase<ColorPicker>
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
            TargetObject.SelectedBrushChanged += OnEventRaised;
        }
    }

	// ColorPickerSelectedBrushChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ColorPickerSelectedBrushChangedCommandBehavior<T> : ColorPickerSelectedBrushChangedCommandBehavior
    { }
	#endregion

	#region ColorPickerSelectedBrushModeChangedCommand
	// ColorPickerSelectedBrushModeChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ColorPickerSelectedBrushModeChangedCommand : ControlCommandBase<ColorPickerSelectedBrushModeChangedCommandBehavior, ColorPicker>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ColorPickerSelectedBrushModeChangedCommandBehavior : CommandBehaviorBase<ColorPicker>
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
            TargetObject.SelectedBrushModeChanged += OnEventRaised;
        }
    }

	// ColorPickerSelectedBrushModeChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ColorPickerSelectedBrushModeChangedCommandBehavior<T> : ColorPickerSelectedBrushModeChangedCommandBehavior
    { }
	#endregion

	#region ColorPickerGradientPropertyEditorModeChangedCommand
	// ColorPickerGradientPropertyEditorModeChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ColorPickerGradientPropertyEditorModeChangedCommand : ControlCommandBase<ColorPickerGradientPropertyEditorModeChangedCommandBehavior, ColorPicker>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ColorPickerGradientPropertyEditorModeChangedCommandBehavior : CommandBehaviorBase<ColorPicker>
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
            TargetObject.GradientPropertyEditorModeChanged += OnEventRaised;
        }
    }

	// ColorPickerGradientPropertyEditorModeChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ColorPickerGradientPropertyEditorModeChangedCommandBehavior<T> : ColorPickerGradientPropertyEditorModeChangedCommandBehavior
    { }
	#endregion

	#region ColorPickerGradientBrushDisplayModeChangedCommand
	// ColorPickerGradientBrushDisplayModeChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ColorPickerGradientBrushDisplayModeChangedCommand : ControlCommandBase<ColorPickerGradientBrushDisplayModeChangedCommandBehavior, ColorPicker>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ColorPickerGradientBrushDisplayModeChangedCommandBehavior : CommandBehaviorBase<ColorPicker>
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
            TargetObject.GradientBrushDisplayModeChanged += OnEventRaised;
        }
    }

	// ColorPickerGradientBrushDisplayModeChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ColorPickerGradientBrushDisplayModeChangedCommandBehavior<T> : ColorPickerGradientBrushDisplayModeChangedCommandBehavior
    { }
	#endregion

	#region ColorPickerIsOpenGradientPropertyEditorChangedCommand
	// ColorPickerIsOpenGradientPropertyEditorChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ColorPickerIsOpenGradientPropertyEditorChangedCommand : ControlCommandBase<ColorPickerIsOpenGradientPropertyEditorChangedCommandBehavior, ColorPicker>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ColorPickerIsOpenGradientPropertyEditorChangedCommandBehavior : CommandBehaviorBase<ColorPicker>
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
            TargetObject.IsOpenGradientPropertyEditorChanged += OnEventRaised;
        }
    }

	// ColorPickerIsOpenGradientPropertyEditorChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ColorPickerIsOpenGradientPropertyEditorChangedCommandBehavior<T> : ColorPickerIsOpenGradientPropertyEditorChangedCommandBehavior
    { }
	#endregion

	#region ColorPickerIsGradientPropertyEnabledChangedCommand
	// ColorPickerIsGradientPropertyEnabledChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ColorPickerIsGradientPropertyEnabledChangedCommand : ControlCommandBase<ColorPickerIsGradientPropertyEnabledChangedCommandBehavior, ColorPicker>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ColorPickerIsGradientPropertyEnabledChangedCommandBehavior : CommandBehaviorBase<ColorPicker>
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
            TargetObject.IsGradientPropertyEnabledChanged += OnEventRaised;
        }
    }

	// ColorPickerIsGradientPropertyEnabledChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ColorPickerIsGradientPropertyEnabledChangedCommandBehavior<T> : ColorPickerIsGradientPropertyEnabledChangedCommandBehavior
    { }
	#endregion
}


