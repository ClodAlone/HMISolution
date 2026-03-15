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

	#region DoubleTextBoxMinValueChangedCommand
	// DoubleTextBoxMinValueChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DoubleTextBoxMinValueChangedCommand : ControlCommandBase<DoubleTextBoxMinValueChangedCommandBehavior, DoubleTextBox>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DoubleTextBoxMinValueChangedCommandBehavior : CommandBehaviorBase<DoubleTextBox>
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

	// DoubleTextBoxMinValueChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DoubleTextBoxMinValueChangedCommandBehavior<T> : DoubleTextBoxMinValueChangedCommandBehavior
    { }
	#endregion

	#region DoubleTextBoxValueChangedCommand
	// DoubleTextBoxValueChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DoubleTextBoxValueChangedCommand : ControlCommandBase<DoubleTextBoxValueChangedCommandBehavior, DoubleTextBox>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DoubleTextBoxValueChangedCommandBehavior : CommandBehaviorBase<DoubleTextBox>
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

	// DoubleTextBoxValueChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DoubleTextBoxValueChangedCommandBehavior<T> : DoubleTextBoxValueChangedCommandBehavior
    { }
	#endregion

	#region DoubleTextBoxValueChangingCommand
	// DoubleTextBoxValueChangingCommand
    /// <summary>
    /// 
    /// </summary>
	public class DoubleTextBoxValueChangingCommand : ControlCommandBase<DoubleTextBoxValueChangingCommandBehavior, DoubleTextBox>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DoubleTextBoxValueChangingCommandBehavior : CommandBehaviorBase<DoubleTextBox>
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

	// DoubleTextBoxValueChangingCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DoubleTextBoxValueChangingCommandBehavior<T> : DoubleTextBoxValueChangingCommandBehavior
    { }
	#endregion

	#region DoubleTextBoxMaxValueChangedCommand
	// DoubleTextBoxMaxValueChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DoubleTextBoxMaxValueChangedCommand : ControlCommandBase<DoubleTextBoxMaxValueChangedCommandBehavior, DoubleTextBox>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DoubleTextBoxMaxValueChangedCommandBehavior : CommandBehaviorBase<DoubleTextBox>
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

	// DoubleTextBoxMaxValueChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DoubleTextBoxMaxValueChangedCommandBehavior<T> : DoubleTextBoxMaxValueChangedCommandBehavior
    { }
	#endregion

	#region DoubleTextBoxNumberDecimalDigitsChangedCommand
	// DoubleTextBoxNumberDecimalDigitsChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DoubleTextBoxNumberDecimalDigitsChangedCommand : ControlCommandBase<DoubleTextBoxNumberDecimalDigitsChangedCommandBehavior, DoubleTextBox>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DoubleTextBoxNumberDecimalDigitsChangedCommandBehavior : CommandBehaviorBase<DoubleTextBox>
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
            TargetObject.NumberDecimalDigitsChanged += OnEventRaised;
        }
    }

	// DoubleTextBoxNumberDecimalDigitsChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DoubleTextBoxNumberDecimalDigitsChangedCommandBehavior<T> : DoubleTextBoxNumberDecimalDigitsChangedCommandBehavior
    { }
	#endregion

	#region DoubleTextBoxNumberDecimalSeparatorChangedCommand
	// DoubleTextBoxNumberDecimalSeparatorChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DoubleTextBoxNumberDecimalSeparatorChangedCommand : ControlCommandBase<DoubleTextBoxNumberDecimalSeparatorChangedCommandBehavior, DoubleTextBox>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DoubleTextBoxNumberDecimalSeparatorChangedCommandBehavior : CommandBehaviorBase<DoubleTextBox>
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
            TargetObject.NumberDecimalSeparatorChanged += OnEventRaised;
        }
    }

	// DoubleTextBoxNumberDecimalSeparatorChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DoubleTextBoxNumberDecimalSeparatorChangedCommandBehavior<T> : DoubleTextBoxNumberDecimalSeparatorChangedCommandBehavior
    { }
	#endregion

	#region DoubleTextBoxNumberGroupSizesChangedCommand
	// DoubleTextBoxNumberGroupSizesChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DoubleTextBoxNumberGroupSizesChangedCommand : ControlCommandBase<DoubleTextBoxNumberGroupSizesChangedCommandBehavior, DoubleTextBox>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DoubleTextBoxNumberGroupSizesChangedCommandBehavior : CommandBehaviorBase<DoubleTextBox>
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
            TargetObject.NumberGroupSizesChanged += OnEventRaised;
        }
    }

	// DoubleTextBoxNumberGroupSizesChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DoubleTextBoxNumberGroupSizesChangedCommandBehavior<T> : DoubleTextBoxNumberGroupSizesChangedCommandBehavior
    { }
	#endregion

	#region DoubleTextBoxNumberGroupSeparatorChangedCommand
	// DoubleTextBoxNumberGroupSeparatorChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DoubleTextBoxNumberGroupSeparatorChangedCommand : ControlCommandBase<DoubleTextBoxNumberGroupSeparatorChangedCommandBehavior, DoubleTextBox>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DoubleTextBoxNumberGroupSeparatorChangedCommandBehavior : CommandBehaviorBase<DoubleTextBox>
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
            TargetObject.NumberGroupSeparatorChanged += OnEventRaised;
        }
    }

	// DoubleTextBoxNumberGroupSeparatorChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DoubleTextBoxNumberGroupSeparatorChangedCommandBehavior<T> : DoubleTextBoxNumberGroupSeparatorChangedCommandBehavior
    { }
	#endregion

	#region DoubleTextBoxMinimumNumberDecimalDigitsChangedCommand
	// DoubleTextBoxMinimumNumberDecimalDigitsChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DoubleTextBoxMinimumNumberDecimalDigitsChangedCommand : ControlCommandBase<DoubleTextBoxMinimumNumberDecimalDigitsChangedCommandBehavior, DoubleTextBox>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DoubleTextBoxMinimumNumberDecimalDigitsChangedCommandBehavior : CommandBehaviorBase<DoubleTextBox>
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
            TargetObject.MinimumNumberDecimalDigitsChanged += OnEventRaised;
        }
    }

	// DoubleTextBoxMinimumNumberDecimalDigitsChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DoubleTextBoxMinimumNumberDecimalDigitsChangedCommandBehavior<T> : DoubleTextBoxMinimumNumberDecimalDigitsChangedCommandBehavior
    { }
	#endregion
    
	#region DoubleTextBoxMaximumNumberDecimalDigitsChangedCommand
	// DoubleTextBoxMaximumNumberDecimalDigitsChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DoubleTextBoxMaximumNumberDecimalDigitsChangedCommand : ControlCommandBase<DoubleTextBoxMaximumNumberDecimalDigitsChangedCommandBehavior, DoubleTextBox>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DoubleTextBoxMaximumNumberDecimalDigitsChangedCommandBehavior : CommandBehaviorBase<DoubleTextBox>
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
            TargetObject.MaximumNumberDecimalDigitsChanged += OnEventRaised;
        }
    }

	// DoubleTextBoxMaximumNumberDecimalDigitsChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DoubleTextBoxMaximumNumberDecimalDigitsChangedCommandBehavior<T> : DoubleTextBoxMaximumNumberDecimalDigitsChangedCommandBehavior
    { }
	#endregion
}


