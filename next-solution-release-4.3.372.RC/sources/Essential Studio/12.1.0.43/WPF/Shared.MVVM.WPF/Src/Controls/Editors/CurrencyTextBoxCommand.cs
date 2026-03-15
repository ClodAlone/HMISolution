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

	#region CurrencyTextBoxMinValueChangedCommand
	// CurrencyTextBoxMinValueChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CurrencyTextBoxMinValueChangedCommand : ControlCommandBase<CurrencyTextBoxMinValueChangedCommandBehavior, CurrencyTextBox>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CurrencyTextBoxMinValueChangedCommandBehavior : CommandBehaviorBase<CurrencyTextBox>
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

	// CurrencyTextBoxMinValueChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CurrencyTextBoxMinValueChangedCommandBehavior<T> : CurrencyTextBoxMinValueChangedCommandBehavior
    { }
	#endregion

	#region CurrencyTextBoxCurrencySymbolPositionChangedCommand
	// CurrencyTextBoxCurrencySymbolPositionChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CurrencyTextBoxCurrencySymbolPositionChangedCommand : ControlCommandBase<CurrencyTextBoxCurrencySymbolPositionChangedCommandBehavior, CurrencyTextBox>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CurrencyTextBoxCurrencySymbolPositionChangedCommandBehavior : CommandBehaviorBase<CurrencyTextBox>
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
            TargetObject.CurrencySymbolPositionChanged += OnEventRaised;
        }
    }

	// CurrencyTextBoxCurrencySymbolPositionChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CurrencyTextBoxCurrencySymbolPositionChangedCommandBehavior<T> : CurrencyTextBoxCurrencySymbolPositionChangedCommandBehavior
    { }
	#endregion

	#region CurrencyTextBoxValueChangedCommand
	// CurrencyTextBoxValueChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CurrencyTextBoxValueChangedCommand : ControlCommandBase<CurrencyTextBoxValueChangedCommandBehavior, CurrencyTextBox>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CurrencyTextBoxValueChangedCommandBehavior : CommandBehaviorBase<CurrencyTextBox>
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
            TargetObject.ValueChanged += OnEventRaised;
        }
    }

	// CurrencyTextBoxValueChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CurrencyTextBoxValueChangedCommandBehavior<T> : CurrencyTextBoxValueChangedCommandBehavior
    { }
	#endregion

	#region CurrencyTextBoxMaxValueChangedCommand
	// CurrencyTextBoxMaxValueChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CurrencyTextBoxMaxValueChangedCommand : ControlCommandBase<CurrencyTextBoxMaxValueChangedCommandBehavior, CurrencyTextBox>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CurrencyTextBoxMaxValueChangedCommandBehavior : CommandBehaviorBase<CurrencyTextBox>
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

	// CurrencyTextBoxMaxValueChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CurrencyTextBoxMaxValueChangedCommandBehavior<T> : CurrencyTextBoxMaxValueChangedCommandBehavior
    { }
	#endregion

	#region CurrencyTextBoxCurrencyDecimalDigitsChangedCommand
	// CurrencyTextBoxCurrencyDecimalDigitsChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CurrencyTextBoxCurrencyDecimalDigitsChangedCommand : ControlCommandBase<CurrencyTextBoxCurrencyDecimalDigitsChangedCommandBehavior, CurrencyTextBox>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CurrencyTextBoxCurrencyDecimalDigitsChangedCommandBehavior : CommandBehaviorBase<CurrencyTextBox>
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
            TargetObject.CurrencyDecimalDigitsChanged += OnEventRaised;
        }
    }

	// CurrencyTextBoxCurrencyDecimalDigitsChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CurrencyTextBoxCurrencyDecimalDigitsChangedCommandBehavior<T> : CurrencyTextBoxCurrencyDecimalDigitsChangedCommandBehavior
    { }
	#endregion

	#region CurrencyTextBoxCurrencyDecimalSeparatorChangedCommand
	// CurrencyTextBoxCurrencyDecimalSeparatorChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CurrencyTextBoxCurrencyDecimalSeparatorChangedCommand : ControlCommandBase<CurrencyTextBoxCurrencyDecimalSeparatorChangedCommandBehavior, CurrencyTextBox>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CurrencyTextBoxCurrencyDecimalSeparatorChangedCommandBehavior : CommandBehaviorBase<CurrencyTextBox>
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
            TargetObject.CurrencyDecimalSeparatorChanged += OnEventRaised;
        }
    }

	// CurrencyTextBoxCurrencyDecimalSeparatorChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CurrencyTextBoxCurrencyDecimalSeparatorChangedCommandBehavior<T> : CurrencyTextBoxCurrencyDecimalSeparatorChangedCommandBehavior
    { }
	#endregion

	#region CurrencyTextBoxCurrencyGroupSeparatorChangedCommand
	// CurrencyTextBoxCurrencyGroupSeparatorChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CurrencyTextBoxCurrencyGroupSeparatorChangedCommand : ControlCommandBase<CurrencyTextBoxCurrencyGroupSeparatorChangedCommandBehavior, CurrencyTextBox>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CurrencyTextBoxCurrencyGroupSeparatorChangedCommandBehavior : CommandBehaviorBase<CurrencyTextBox>
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
            TargetObject.CurrencyGroupSeparatorChanged += OnEventRaised;
        }
    }

	// CurrencyTextBoxCurrencyGroupSeparatorChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CurrencyTextBoxCurrencyGroupSeparatorChangedCommandBehavior<T> : CurrencyTextBoxCurrencyGroupSeparatorChangedCommandBehavior
    { }
	#endregion

	#region CurrencyTextBoxCurrencyGroupSizesChangedCommand
	// CurrencyTextBoxCurrencyGroupSizesChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CurrencyTextBoxCurrencyGroupSizesChangedCommand : ControlCommandBase<CurrencyTextBoxCurrencyGroupSizesChangedCommandBehavior, CurrencyTextBox>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CurrencyTextBoxCurrencyGroupSizesChangedCommandBehavior : CommandBehaviorBase<CurrencyTextBox>
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
            TargetObject.CurrencyGroupSizesChanged += OnEventRaised;
        }
    }

	// CurrencyTextBoxCurrencyGroupSizesChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CurrencyTextBoxCurrencyGroupSizesChangedCommandBehavior<T> : CurrencyTextBoxCurrencyGroupSizesChangedCommandBehavior
    { }
	#endregion

	#region CurrencyTextBoxCurrencyNegativePatternChangedCommand
	// CurrencyTextBoxCurrencyNegativePatternChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CurrencyTextBoxCurrencyNegativePatternChangedCommand : ControlCommandBase<CurrencyTextBoxCurrencyNegativePatternChangedCommandBehavior, CurrencyTextBox>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CurrencyTextBoxCurrencyNegativePatternChangedCommandBehavior : CommandBehaviorBase<CurrencyTextBox>
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
            TargetObject.CurrencyNegativePatternChanged += OnEventRaised;
        }
    }

	// CurrencyTextBoxCurrencyNegativePatternChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CurrencyTextBoxCurrencyNegativePatternChangedCommandBehavior<T> : CurrencyTextBoxCurrencyNegativePatternChangedCommandBehavior
    { }
	#endregion

	#region CurrencyTextBoxCurrencyPositivePatternChangedCommand
	// CurrencyTextBoxCurrencyPositivePatternChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CurrencyTextBoxCurrencyPositivePatternChangedCommand : ControlCommandBase<CurrencyTextBoxCurrencyPositivePatternChangedCommandBehavior, CurrencyTextBox>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CurrencyTextBoxCurrencyPositivePatternChangedCommandBehavior : CommandBehaviorBase<CurrencyTextBox>
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
            TargetObject.CurrencyPositivePatternChanged += OnEventRaised;
        }
    }

	// CurrencyTextBoxCurrencyPositivePatternChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CurrencyTextBoxCurrencyPositivePatternChangedCommandBehavior<T> : CurrencyTextBoxCurrencyPositivePatternChangedCommandBehavior
    { }
	#endregion

	#region CurrencyTextBoxCurrencySymbolChangedCommand
	// CurrencyTextBoxCurrencySymbolChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CurrencyTextBoxCurrencySymbolChangedCommand : ControlCommandBase<CurrencyTextBoxCurrencySymbolChangedCommandBehavior, CurrencyTextBox>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CurrencyTextBoxCurrencySymbolChangedCommandBehavior : CommandBehaviorBase<CurrencyTextBox>
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
            TargetObject.CurrencySymbolChanged += OnEventRaised;
        }
    }

	// CurrencyTextBoxCurrencySymbolChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CurrencyTextBoxCurrencySymbolChangedCommandBehavior<T> : CurrencyTextBoxCurrencySymbolChangedCommandBehavior
    { }
	#endregion
}


