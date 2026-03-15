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

	#region RibbonComboBoxLabelChangedCommand
	// RibbonComboBoxLabelChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class RibbonComboBoxLabelChangedCommand : ControlCommandBase<RibbonComboBoxLabelChangedCommandBehavior, RibbonComboBox>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class RibbonComboBoxLabelChangedCommandBehavior : CommandBehaviorBase<RibbonComboBox>
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
            TargetObject.LabelChanged += OnEventRaised;
        }
    }

	// RibbonComboBoxLabelChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RibbonComboBoxLabelChangedCommandBehavior<T> : RibbonComboBoxLabelChangedCommandBehavior
    { }
	#endregion

	#region RibbonComboBoxSmallIconChangedCommand
	// RibbonComboBoxSmallIconChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class RibbonComboBoxSmallIconChangedCommand : ControlCommandBase<RibbonComboBoxSmallIconChangedCommandBehavior, RibbonComboBox>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class RibbonComboBoxSmallIconChangedCommandBehavior : CommandBehaviorBase<RibbonComboBox>
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
            TargetObject.SmallIconChanged += OnEventRaised;
        }
    }

	// RibbonComboBoxSmallIconChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RibbonComboBoxSmallIconChangedCommandBehavior<T> : RibbonComboBoxSmallIconChangedCommandBehavior
    { }
	#endregion
}


