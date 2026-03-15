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

	#region IntegerTextBoxMinValueChangedCommand
	// IntegerTextBoxMinValueChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class IntegerTextBoxMinValueChangedCommand : ControlCommandBase<IntegerTextBoxMinValueChangedCommandBehavior, IntegerTextBox>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class IntegerTextBoxMinValueChangedCommandBehavior : CommandBehaviorBase<IntegerTextBox>
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

	// IntegerTextBoxMinValueChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class IntegerTextBoxMinValueChangedCommandBehavior<T> : IntegerTextBoxMinValueChangedCommandBehavior
    { }
	#endregion

	#region IntegerTextBoxValueChangedCommand
	// IntegerTextBoxValueChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class IntegerTextBoxValueChangedCommand : ControlCommandBase<IntegerTextBoxValueChangedCommandBehavior, IntegerTextBox>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class IntegerTextBoxValueChangedCommandBehavior : CommandBehaviorBase<IntegerTextBox>
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

	// IntegerTextBoxValueChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class IntegerTextBoxValueChangedCommandBehavior<T> : IntegerTextBoxValueChangedCommandBehavior
    { }
	#endregion

	#region IntegerTextBoxMaxValueChangedCommand
	// IntegerTextBoxMaxValueChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class IntegerTextBoxMaxValueChangedCommand : ControlCommandBase<IntegerTextBoxMaxValueChangedCommandBehavior, IntegerTextBox>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class IntegerTextBoxMaxValueChangedCommandBehavior : CommandBehaviorBase<IntegerTextBox>
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

	// IntegerTextBoxMaxValueChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class IntegerTextBoxMaxValueChangedCommandBehavior<T> : IntegerTextBoxMaxValueChangedCommandBehavior
    { }
	#endregion

	#region IntegerTextBoxNumberGroupSizesChangedCommand
	// IntegerTextBoxNumberGroupSizesChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class IntegerTextBoxNumberGroupSizesChangedCommand : ControlCommandBase<IntegerTextBoxNumberGroupSizesChangedCommandBehavior, IntegerTextBox>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class IntegerTextBoxNumberGroupSizesChangedCommandBehavior : CommandBehaviorBase<IntegerTextBox>
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

	// IntegerTextBoxNumberGroupSizesChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class IntegerTextBoxNumberGroupSizesChangedCommandBehavior<T> : IntegerTextBoxNumberGroupSizesChangedCommandBehavior
    { }
	#endregion

	#region IntegerTextBoxNumberGroupSeparatorChangedCommand
	// IntegerTextBoxNumberGroupSeparatorChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class IntegerTextBoxNumberGroupSeparatorChangedCommand : ControlCommandBase<IntegerTextBoxNumberGroupSeparatorChangedCommandBehavior, IntegerTextBox>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class IntegerTextBoxNumberGroupSeparatorChangedCommandBehavior : CommandBehaviorBase<IntegerTextBox>
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

	// IntegerTextBoxNumberGroupSeparatorChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class IntegerTextBoxNumberGroupSeparatorChangedCommandBehavior<T> : IntegerTextBoxNumberGroupSeparatorChangedCommandBehavior
    { }
	#endregion
}


