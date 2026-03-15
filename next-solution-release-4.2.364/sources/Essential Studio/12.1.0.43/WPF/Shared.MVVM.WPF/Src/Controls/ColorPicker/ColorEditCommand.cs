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

	#region ColorEditRChangedCommand
	// ColorEditRChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ColorEditRChangedCommand : ControlCommandBase<ColorEditRChangedCommandBehavior, ColorEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ColorEditRChangedCommandBehavior : CommandBehaviorBase<ColorEdit>
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
            TargetObject.RChanged += OnEventRaised;
        }
    }

	// ColorEditRChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ColorEditRChangedCommandBehavior<T> : ColorEditRChangedCommandBehavior
    { }
	#endregion

	#region ColorEditGChangedCommand
	// ColorEditGChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ColorEditGChangedCommand : ControlCommandBase<ColorEditGChangedCommandBehavior, ColorEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ColorEditGChangedCommandBehavior : CommandBehaviorBase<ColorEdit>
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
            TargetObject.GChanged += OnEventRaised;
        }
    }

	// ColorEditGChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ColorEditGChangedCommandBehavior<T> : ColorEditGChangedCommandBehavior
    { }
	#endregion

	#region ColorEditBChangedCommand
	// ColorEditBChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ColorEditBChangedCommand : ControlCommandBase<ColorEditBChangedCommandBehavior, ColorEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ColorEditBChangedCommandBehavior : CommandBehaviorBase<ColorEdit>
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
            TargetObject.BChanged += OnEventRaised;
        }
    }

	// ColorEditBChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ColorEditBChangedCommandBehavior<T> : ColorEditBChangedCommandBehavior
    { }
	#endregion

	#region ColorEditAChangedCommand
	// ColorEditAChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ColorEditAChangedCommand : ControlCommandBase<ColorEditAChangedCommandBehavior, ColorEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ColorEditAChangedCommandBehavior : CommandBehaviorBase<ColorEdit>
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
            TargetObject.AChanged += OnEventRaised;
        }
    }

	// ColorEditAChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ColorEditAChangedCommandBehavior<T> : ColorEditAChangedCommandBehavior
    { }
	#endregion

	#region ColorEditHChangedCommand
	// ColorEditHChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ColorEditHChangedCommand : ControlCommandBase<ColorEditHChangedCommandBehavior, ColorEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ColorEditHChangedCommandBehavior : CommandBehaviorBase<ColorEdit>
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
            TargetObject.HChanged += OnEventRaised;
        }
    }

	// ColorEditHChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ColorEditHChangedCommandBehavior<T> : ColorEditHChangedCommandBehavior
    { }
	#endregion

	#region ColorEditSChangedCommand
	// ColorEditSChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ColorEditSChangedCommand : ControlCommandBase<ColorEditSChangedCommandBehavior, ColorEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ColorEditSChangedCommandBehavior : CommandBehaviorBase<ColorEdit>
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
            TargetObject.SChanged += OnEventRaised;
        }
    }

	// ColorEditSChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ColorEditSChangedCommandBehavior<T> : ColorEditSChangedCommandBehavior
    { }
	#endregion

	#region ColorEditVChangedCommand
	// ColorEditVChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ColorEditVChangedCommand : ControlCommandBase<ColorEditVChangedCommandBehavior, ColorEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ColorEditVChangedCommandBehavior : CommandBehaviorBase<ColorEdit>
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
            TargetObject.VChanged += OnEventRaised;
        }
    }

	// ColorEditVChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ColorEditVChangedCommandBehavior<T> : ColorEditVChangedCommandBehavior
    { }
	#endregion

	#region ColorEditSliderValueHSVChangedCommand
	// ColorEditSliderValueHSVChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ColorEditSliderValueHSVChangedCommand : ControlCommandBase<ColorEditSliderValueHSVChangedCommandBehavior, ColorEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ColorEditSliderValueHSVChangedCommandBehavior : CommandBehaviorBase<ColorEdit>
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
            TargetObject.SliderValueHSVChanged += OnEventRaised;
        }
    }

	// ColorEditSliderValueHSVChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ColorEditSliderValueHSVChangedCommandBehavior<T> : ColorEditSliderValueHSVChangedCommandBehavior
    { }
	#endregion

	#region ColorEditVisualizationStyleChangedCommand
	// ColorEditVisualizationStyleChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ColorEditVisualizationStyleChangedCommand : ControlCommandBase<ColorEditVisualizationStyleChangedCommandBehavior, ColorEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ColorEditVisualizationStyleChangedCommandBehavior : CommandBehaviorBase<ColorEdit>
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
            TargetObject.VisualizationStyleChanged += OnEventRaised;
        }
    }

	// ColorEditVisualizationStyleChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ColorEditVisualizationStyleChangedCommandBehavior<T> : ColorEditVisualizationStyleChangedCommandBehavior
    { }
	#endregion

	#region ColorEditColorChangedCommand
	// ColorEditColorChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ColorEditColorChangedCommand : ControlCommandBase<ColorEditColorChangedCommandBehavior, ColorEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ColorEditColorChangedCommandBehavior : CommandBehaviorBase<ColorEdit>
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

	// ColorEditColorChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ColorEditColorChangedCommandBehavior<T> : ColorEditColorChangedCommandBehavior
    { }
	#endregion

	#region ColorEditGradientPropertyEditorModeChangedCommand
	// ColorEditGradientPropertyEditorModeChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ColorEditGradientPropertyEditorModeChangedCommand : ControlCommandBase<ColorEditGradientPropertyEditorModeChangedCommandBehavior, ColorEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ColorEditGradientPropertyEditorModeChangedCommandBehavior : CommandBehaviorBase<ColorEdit>
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

	// ColorEditGradientPropertyEditorModeChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ColorEditGradientPropertyEditorModeChangedCommandBehavior<T> : ColorEditGradientPropertyEditorModeChangedCommandBehavior
    { }
	#endregion

	#region ColorEditIsOpenGradientPropertyEditorChangedCommand
	// ColorEditIsOpenGradientPropertyEditorChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ColorEditIsOpenGradientPropertyEditorChangedCommand : ControlCommandBase<ColorEditIsOpenGradientPropertyEditorChangedCommandBehavior, ColorEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ColorEditIsOpenGradientPropertyEditorChangedCommandBehavior : CommandBehaviorBase<ColorEdit>
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

	// ColorEditIsOpenGradientPropertyEditorChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ColorEditIsOpenGradientPropertyEditorChangedCommandBehavior<T> : ColorEditIsOpenGradientPropertyEditorChangedCommandBehavior
    { }
	#endregion

	#region ColorEditIsGradientPropertyEnabledChangedCommand
	// ColorEditIsGradientPropertyEnabledChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ColorEditIsGradientPropertyEnabledChangedCommand : ControlCommandBase<ColorEditIsGradientPropertyEnabledChangedCommandBehavior, ColorEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ColorEditIsGradientPropertyEnabledChangedCommandBehavior : CommandBehaviorBase<ColorEdit>
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

	// ColorEditIsGradientPropertyEnabledChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ColorEditIsGradientPropertyEnabledChangedCommandBehavior<T> : ColorEditIsGradientPropertyEnabledChangedCommandBehavior
    { }
	#endregion

	#region ColorEditEnableToolTipChangedCommand
	// ColorEditEnableToolTipChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ColorEditEnableToolTipChangedCommand : ControlCommandBase<ColorEditEnableToolTipChangedCommandBehavior, ColorEdit>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ColorEditEnableToolTipChangedCommandBehavior : CommandBehaviorBase<ColorEdit>
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

	// ColorEditEnableToolTipChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ColorEditEnableToolTipChangedCommandBehavior<T> : ColorEditEnableToolTipChangedCommandBehavior
    { }
	#endregion
}


