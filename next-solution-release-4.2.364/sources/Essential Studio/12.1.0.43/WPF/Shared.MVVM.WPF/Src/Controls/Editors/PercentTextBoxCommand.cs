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

	#region PercentTextBoxPercentageSymbolChangedCommand
	// PercentTextBoxPercentageSymbolChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class PercentTextBoxPercentageSymbolChangedCommand : ControlCommandBase<PercentTextBoxPercentageSymbolChangedCommandBehavior, PercentTextBox>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class PercentTextBoxPercentageSymbolChangedCommandBehavior : CommandBehaviorBase<PercentTextBox>
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
            TargetObject.PercentageSymbolChanged += OnEventRaised;
        }
    }

	// PercentTextBoxPercentageSymbolChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PercentTextBoxPercentageSymbolChangedCommandBehavior<T> : PercentTextBoxPercentageSymbolChangedCommandBehavior
    { }
	#endregion

	#region PercentTextBoxPercentEditModeChangedCommand
	// PercentTextBoxPercentEditModeChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class PercentTextBoxPercentEditModeChangedCommand : ControlCommandBase<PercentTextBoxPercentEditModeChangedCommandBehavior, PercentTextBox>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class PercentTextBoxPercentEditModeChangedCommandBehavior : CommandBehaviorBase<PercentTextBox>
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
            TargetObject.PercentEditModeChanged += OnEventRaised;
        }
    }

	// PercentTextBoxPercentEditModeChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PercentTextBoxPercentEditModeChangedCommandBehavior<T> : PercentTextBoxPercentEditModeChangedCommandBehavior
    { }
	#endregion

	#region PercentTextBoxPercentValueChangedCommand
	// PercentTextBoxPercentValueChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class PercentTextBoxPercentValueChangedCommand : ControlCommandBase<PercentTextBoxPercentValueChangedCommandBehavior, PercentTextBox>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class PercentTextBoxPercentValueChangedCommandBehavior : CommandBehaviorBase<PercentTextBox>
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
            TargetObject.PercentValueChanged += OnEventRaised;
        }
    }

	// PercentTextBoxPercentValueChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PercentTextBoxPercentValueChangedCommandBehavior<T> : PercentTextBoxPercentValueChangedCommandBehavior
    { }
	#endregion

	#region PercentTextBoxMinValueChangedCommand
	// PercentTextBoxMinValueChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class PercentTextBoxMinValueChangedCommand : ControlCommandBase<PercentTextBoxMinValueChangedCommandBehavior, PercentTextBox>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class PercentTextBoxMinValueChangedCommandBehavior : CommandBehaviorBase<PercentTextBox>
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

	// PercentTextBoxMinValueChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PercentTextBoxMinValueChangedCommandBehavior<T> : PercentTextBoxMinValueChangedCommandBehavior
    { }
	#endregion

	#region PercentTextBoxMaxValueChangedCommand
	// PercentTextBoxMaxValueChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class PercentTextBoxMaxValueChangedCommand : ControlCommandBase<PercentTextBoxMaxValueChangedCommandBehavior, PercentTextBox>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class PercentTextBoxMaxValueChangedCommandBehavior : CommandBehaviorBase<PercentTextBox>
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

	// PercentTextBoxMaxValueChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PercentTextBoxMaxValueChangedCommandBehavior<T> : PercentTextBoxMaxValueChangedCommandBehavior
    { }
	#endregion

	#region PercentTextBoxPercentDecimalDigitsChangedCommand
	// PercentTextBoxPercentDecimalDigitsChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class PercentTextBoxPercentDecimalDigitsChangedCommand : ControlCommandBase<PercentTextBoxPercentDecimalDigitsChangedCommandBehavior, PercentTextBox>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class PercentTextBoxPercentDecimalDigitsChangedCommandBehavior : CommandBehaviorBase<PercentTextBox>
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
            TargetObject.PercentDecimalDigitsChanged += OnEventRaised;
        }
    }

	// PercentTextBoxPercentDecimalDigitsChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PercentTextBoxPercentDecimalDigitsChangedCommandBehavior<T> : PercentTextBoxPercentDecimalDigitsChangedCommandBehavior
    { }
	#endregion

	#region PercentTextBoxPercentDecimalSeparatorChangedCommand
	// PercentTextBoxPercentDecimalSeparatorChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class PercentTextBoxPercentDecimalSeparatorChangedCommand : ControlCommandBase<PercentTextBoxPercentDecimalSeparatorChangedCommandBehavior, PercentTextBox>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class PercentTextBoxPercentDecimalSeparatorChangedCommandBehavior : CommandBehaviorBase<PercentTextBox>
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
            TargetObject.PercentDecimalSeparatorChanged += OnEventRaised;
        }
    }

	// PercentTextBoxPercentDecimalSeparatorChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PercentTextBoxPercentDecimalSeparatorChangedCommandBehavior<T> : PercentTextBoxPercentDecimalSeparatorChangedCommandBehavior
    { }
	#endregion

	#region PercentTextBoxPercentGroupSeparatorChangedCommand
	// PercentTextBoxPercentGroupSeparatorChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class PercentTextBoxPercentGroupSeparatorChangedCommand : ControlCommandBase<PercentTextBoxPercentGroupSeparatorChangedCommandBehavior, PercentTextBox>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class PercentTextBoxPercentGroupSeparatorChangedCommandBehavior : CommandBehaviorBase<PercentTextBox>
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
            TargetObject.PercentGroupSeparatorChanged += OnEventRaised;
        }
    }

	// PercentTextBoxPercentGroupSeparatorChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PercentTextBoxPercentGroupSeparatorChangedCommandBehavior<T> : PercentTextBoxPercentGroupSeparatorChangedCommandBehavior
    { }
	#endregion

	#region PercentTextBoxPercentGroupSizesChangedCommand
	// PercentTextBoxPercentGroupSizesChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class PercentTextBoxPercentGroupSizesChangedCommand : ControlCommandBase<PercentTextBoxPercentGroupSizesChangedCommandBehavior, PercentTextBox>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class PercentTextBoxPercentGroupSizesChangedCommandBehavior : CommandBehaviorBase<PercentTextBox>
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
            TargetObject.PercentGroupSizesChanged += OnEventRaised;
        }
    }

	// PercentTextBoxPercentGroupSizesChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PercentTextBoxPercentGroupSizesChangedCommandBehavior<T> : PercentTextBoxPercentGroupSizesChangedCommandBehavior
    { }
	#endregion

	#region PercentTextBoxValidationOnLostFocusChangedCommand
	// PercentTextBoxValidationOnLostFocusChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class PercentTextBoxValidationOnLostFocusChangedCommand : ControlCommandBase<PercentTextBoxValidationOnLostFocusChangedCommandBehavior, PercentTextBox>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class PercentTextBoxValidationOnLostFocusChangedCommandBehavior : CommandBehaviorBase<PercentTextBox>
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
            TargetObject.ValidationOnLostFocusChanged += OnEventRaised;
        }
    }

	// PercentTextBoxValidationOnLostFocusChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PercentTextBoxValidationOnLostFocusChangedCommandBehavior<T> : PercentTextBoxValidationOnLostFocusChangedCommandBehavior
    { }
	#endregion
}


