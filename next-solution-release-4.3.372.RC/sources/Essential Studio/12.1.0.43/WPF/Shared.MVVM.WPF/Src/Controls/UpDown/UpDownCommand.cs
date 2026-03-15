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

	#region UpDownAllowEditChangedCommand
	// UpDownAllowEditChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class UpDownAllowEditChangedCommand : ControlCommandBase<UpDownAllowEditChangedCommandBehavior, UpDown>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class UpDownAllowEditChangedCommandBehavior : CommandBehaviorBase<UpDown>
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
            TargetObject.AllowEditChanged += OnEventRaised;
        }
    }

	// UpDownAllowEditChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UpDownAllowEditChangedCommandBehavior<T> : UpDownAllowEditChangedCommandBehavior
    { }
	#endregion

	#region UpDownStepChangedCommand
	// UpDownStepChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class UpDownStepChangedCommand : ControlCommandBase<UpDownStepChangedCommandBehavior, UpDown>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class UpDownStepChangedCommandBehavior : CommandBehaviorBase<UpDown>
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
            TargetObject.StepChanged += OnEventRaised;
        }
    }

	// UpDownStepChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UpDownStepChangedCommandBehavior<T> : UpDownStepChangedCommandBehavior
    { }
	#endregion

	#region UpDownUseNullOptionChangedCommand
	// UpDownUseNullOptionChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class UpDownUseNullOptionChangedCommand : ControlCommandBase<UpDownUseNullOptionChangedCommandBehavior, UpDown>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class UpDownUseNullOptionChangedCommandBehavior : CommandBehaviorBase<UpDown>
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
            TargetObject.UseNullOptionChanged += OnEventRaised;
        }
    }

	// UpDownUseNullOptionChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UpDownUseNullOptionChangedCommandBehavior<T> : UpDownUseNullOptionChangedCommandBehavior
    { }
	#endregion

	#region UpDownValueChangedCommand
	// UpDownValueChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class UpDownValueChangedCommand : ControlCommandBase<UpDownValueChangedCommandBehavior, UpDown>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class UpDownValueChangedCommandBehavior : CommandBehaviorBase<UpDown>
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
            TargetObject.ValueChanged += OnEventRaised;
        }
    }

	// UpDownValueChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UpDownValueChangedCommandBehavior<T> : UpDownValueChangedCommandBehavior
    { }
	#endregion

	#region UpDownValueChangingCommand
	// UpDownValueChangingCommand
    /// <summary>
    /// 
    /// </summary>
	public class UpDownValueChangingCommand : ControlCommandBase<UpDownValueChangingCommandBehavior, UpDown>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class UpDownValueChangingCommandBehavior : CommandBehaviorBase<UpDown>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, ValueChangingEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.ValueChanging += OnEventRaised;
        }
    }

	// UpDownValueChangingCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UpDownValueChangingCommandBehavior<T> : UpDownValueChangingCommandBehavior
    { }
	#endregion

	#region UpDownMinValueChangedCommand
	// UpDownMinValueChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class UpDownMinValueChangedCommand : ControlCommandBase<UpDownMinValueChangedCommandBehavior, UpDown>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class UpDownMinValueChangedCommandBehavior : CommandBehaviorBase<UpDown>
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
            TargetObject.MinValueChanged += OnEventRaised;
        }
    }

	// UpDownMinValueChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UpDownMinValueChangedCommandBehavior<T> : UpDownMinValueChangedCommandBehavior
    { }
	#endregion

	#region UpDownIsScrollingOnCircleChangedCommand
	// UpDownIsScrollingOnCircleChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class UpDownIsScrollingOnCircleChangedCommand : ControlCommandBase<UpDownIsScrollingOnCircleChangedCommandBehavior, UpDown>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class UpDownIsScrollingOnCircleChangedCommandBehavior : CommandBehaviorBase<UpDown>
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
            TargetObject.IsScrollingOnCircleChanged += OnEventRaised;
        }
    }

	// UpDownIsScrollingOnCircleChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UpDownIsScrollingOnCircleChangedCommandBehavior<T> : UpDownIsScrollingOnCircleChangedCommandBehavior
    { }
	#endregion

	#region UpDownMaxValueChangedCommand
	// UpDownMaxValueChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class UpDownMaxValueChangedCommand : ControlCommandBase<UpDownMaxValueChangedCommandBehavior, UpDown>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class UpDownMaxValueChangedCommandBehavior : CommandBehaviorBase<UpDown>
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
            TargetObject.MaxValueChanged += OnEventRaised;
        }
    }

	// UpDownMaxValueChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UpDownMaxValueChangedCommandBehavior<T> : UpDownMaxValueChangedCommandBehavior
    { }
	#endregion

	#region UpDownNumberFormatInfoChangedCommand
	// UpDownNumberFormatInfoChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class UpDownNumberFormatInfoChangedCommand : ControlCommandBase<UpDownNumberFormatInfoChangedCommandBehavior, UpDown>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class UpDownNumberFormatInfoChangedCommandBehavior : CommandBehaviorBase<UpDown>
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
            TargetObject.NumberFormatInfoChanged += OnEventRaised;
        }
    }

	// UpDownNumberFormatInfoChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UpDownNumberFormatInfoChangedCommandBehavior<T> : UpDownNumberFormatInfoChangedCommandBehavior
    { }
	#endregion

	#region UpDownZeroColorChangedCommand
	// UpDownZeroColorChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class UpDownZeroColorChangedCommand : ControlCommandBase<UpDownZeroColorChangedCommandBehavior, UpDown>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class UpDownZeroColorChangedCommandBehavior : CommandBehaviorBase<UpDown>
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
            TargetObject.ZeroColorChanged += OnEventRaised;
        }
    }

	// UpDownZeroColorChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UpDownZeroColorChangedCommandBehavior<T> : UpDownZeroColorChangedCommandBehavior
    { }
	#endregion

	#region UpDownNegativeForegroundChangedCommand
	// UpDownNegativeForegroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class UpDownNegativeForegroundChangedCommand : ControlCommandBase<UpDownNegativeForegroundChangedCommandBehavior, UpDown>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class UpDownNegativeForegroundChangedCommandBehavior : CommandBehaviorBase<UpDown>
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
            TargetObject.NegativeForegroundChanged += OnEventRaised;
        }
    }

	// UpDownNegativeForegroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UpDownNegativeForegroundChangedCommandBehavior<T> : UpDownNegativeForegroundChangedCommandBehavior
    { }
	#endregion

	#region UpDownMinValidationChangedCommand
	// UpDownMinValidationChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class UpDownMinValidationChangedCommand : ControlCommandBase<UpDownMinValidationChangedCommandBehavior, UpDown>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class UpDownMinValidationChangedCommandBehavior : CommandBehaviorBase<UpDown>
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
            TargetObject.MinValidationChanged += OnEventRaised;
        }
    }

	// UpDownMinValidationChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UpDownMinValidationChangedCommandBehavior<T> : UpDownMinValidationChangedCommandBehavior
    { }
	#endregion

	#region UpDownMaxValidationChangedCommand
	// UpDownMaxValidationChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class UpDownMaxValidationChangedCommand : ControlCommandBase<UpDownMaxValidationChangedCommandBehavior, UpDown>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class UpDownMaxValidationChangedCommandBehavior : CommandBehaviorBase<UpDown>
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
            TargetObject.MaxValidationChanged += OnEventRaised;
        }
    }

	// UpDownMaxValidationChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UpDownMaxValidationChangedCommandBehavior<T> : UpDownMaxValidationChangedCommandBehavior
    { }
	#endregion

		
	#region UpDownNullValueTextChangedCommand
	// UpDownNullValueTextChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class UpDownNullValueTextChangedCommand : ControlCommandBase<UpDownNullValueTextChangedCommandBehavior, UpDown>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class UpDownNullValueTextChangedCommandBehavior : CommandBehaviorBase<UpDown>
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
            TargetObject.NullValueTextChanged += OnEventRaised;
        }
    }

	// UpDownNullValueTextChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UpDownNullValueTextChangedCommandBehavior<T> : UpDownNullValueTextChangedCommandBehavior
    { }
	#endregion

	#region UpDownFocusedBackgroundChangedCommand
	// UpDownFocusedBackgroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class UpDownFocusedBackgroundChangedCommand : ControlCommandBase<UpDownFocusedBackgroundChangedCommandBehavior, UpDown>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class UpDownFocusedBackgroundChangedCommandBehavior : CommandBehaviorBase<UpDown>
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
            TargetObject.FocusedBackgroundChanged += OnEventRaised;
        }
    }

	// UpDownFocusedBackgroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UpDownFocusedBackgroundChangedCommandBehavior<T> : UpDownFocusedBackgroundChangedCommandBehavior
    { }
	#endregion

	#region UpDownFocusedForegroundChangedCommand
	// UpDownFocusedForegroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class UpDownFocusedForegroundChangedCommand : ControlCommandBase<UpDownFocusedForegroundChangedCommandBehavior, UpDown>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class UpDownFocusedForegroundChangedCommandBehavior : CommandBehaviorBase<UpDown>
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
            TargetObject.FocusedForegroundChanged += OnEventRaised;
        }
    }

	// UpDownFocusedForegroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UpDownFocusedForegroundChangedCommandBehavior<T> : UpDownFocusedForegroundChangedCommandBehavior
    { }
	#endregion

	#region UpDownFocusedBorderBrushChangedCommand
	// UpDownFocusedBorderBrushChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class UpDownFocusedBorderBrushChangedCommand : ControlCommandBase<UpDownFocusedBorderBrushChangedCommandBehavior, UpDown>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class UpDownFocusedBorderBrushChangedCommandBehavior : CommandBehaviorBase<UpDown>
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
            TargetObject.FocusedBorderBrushChanged += OnEventRaised;
        }
    }

	// UpDownFocusedBorderBrushChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UpDownFocusedBorderBrushChangedCommandBehavior<T> : UpDownFocusedBorderBrushChangedCommandBehavior
    { }
	#endregion
}


