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

	#region RibbonButtonSmallIconChangedCommand
	// RibbonButtonSmallIconChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class RibbonButtonSmallIconChangedCommand : ControlCommandBase<RibbonButtonSmallIconChangedCommandBehavior, RibbonButton>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class RibbonButtonSmallIconChangedCommandBehavior : CommandBehaviorBase<RibbonButton>
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

	// RibbonButtonSmallIconChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RibbonButtonSmallIconChangedCommandBehavior<T> : RibbonButtonSmallIconChangedCommandBehavior
    { }
	#endregion

	#region RibbonButtonLargeIconChangedCommand
	// RibbonButtonLargeIconChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class RibbonButtonLargeIconChangedCommand : ControlCommandBase<RibbonButtonLargeIconChangedCommandBehavior, RibbonButton>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class RibbonButtonLargeIconChangedCommandBehavior : CommandBehaviorBase<RibbonButton>
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
            TargetObject.LargeIconChanged += OnEventRaised;
        }
    }

	// RibbonButtonLargeIconChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RibbonButtonLargeIconChangedCommandBehavior<T> : RibbonButtonLargeIconChangedCommandBehavior
    { }
	#endregion

	#region RibbonButtonLabelChangedCommand
	// RibbonButtonLabelChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class RibbonButtonLabelChangedCommand : ControlCommandBase<RibbonButtonLabelChangedCommandBehavior, RibbonButton>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class RibbonButtonLabelChangedCommandBehavior : CommandBehaviorBase<RibbonButton>
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

	// RibbonButtonLabelChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RibbonButtonLabelChangedCommandBehavior<T> : RibbonButtonLabelChangedCommandBehavior
    { }
	#endregion

	#region RibbonButtonSizeFormChangedCommand
	// RibbonButtonSizeFormChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class RibbonButtonSizeFormChangedCommand : ControlCommandBase<RibbonButtonSizeFormChangedCommandBehavior, RibbonButton>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class RibbonButtonSizeFormChangedCommandBehavior : CommandBehaviorBase<RibbonButton>
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
            TargetObject.SizeFormChanged += OnEventRaised;
        }
    }

	// RibbonButtonSizeFormChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RibbonButtonSizeFormChangedCommandBehavior<T> : RibbonButtonSizeFormChangedCommandBehavior
    { }
	#endregion

	#region RibbonButtonIsSelectedChangedCommand
	// RibbonButtonIsSelectedChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class RibbonButtonIsSelectedChangedCommand : ControlCommandBase<RibbonButtonIsSelectedChangedCommandBehavior, RibbonButton>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class RibbonButtonIsSelectedChangedCommandBehavior : CommandBehaviorBase<RibbonButton>
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
            TargetObject.IsSelectedChanged += OnEventRaised;
        }
    }

	// RibbonButtonIsSelectedChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RibbonButtonIsSelectedChangedCommandBehavior<T> : RibbonButtonIsSelectedChangedCommandBehavior
    { }
	#endregion

	#region RibbonButtonIsToggleChangedCommand
	// RibbonButtonIsToggleChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class RibbonButtonIsToggleChangedCommand : ControlCommandBase<RibbonButtonIsToggleChangedCommandBehavior, RibbonButton>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class RibbonButtonIsToggleChangedCommandBehavior : CommandBehaviorBase<RibbonButton>
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
            TargetObject.IsToggleChanged += OnEventRaised;
        }
    }

	// RibbonButtonIsToggleChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RibbonButtonIsToggleChangedCommandBehavior<T> : RibbonButtonIsToggleChangedCommandBehavior
    { }
	#endregion

	#region RibbonButtonIsMultiLineChangedCommand
	// RibbonButtonIsMultiLineChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class RibbonButtonIsMultiLineChangedCommand : ControlCommandBase<RibbonButtonIsMultiLineChangedCommandBehavior, RibbonButton>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class RibbonButtonIsMultiLineChangedCommandBehavior : CommandBehaviorBase<RibbonButton>
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
            TargetObject.IsMultiLineChanged += OnEventRaised;
        }
    }

	// RibbonButtonIsMultiLineChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RibbonButtonIsMultiLineChangedCommandBehavior<T> : RibbonButtonIsMultiLineChangedCommandBehavior
    { }
	#endregion

	#region RibbonButtonSplitLabelIntoTwoLineChangedCommand
	// RibbonButtonSplitLabelIntoTwoLineChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class RibbonButtonSplitLabelIntoTwoLineChangedCommand : ControlCommandBase<RibbonButtonSplitLabelIntoTwoLineChangedCommandBehavior, RibbonButton>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class RibbonButtonSplitLabelIntoTwoLineChangedCommandBehavior : CommandBehaviorBase<RibbonButton>
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
            TargetObject.SplitLabelIntoTwoLineChanged += OnEventRaised;
        }
    }

	// RibbonButtonSplitLabelIntoTwoLineChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RibbonButtonSplitLabelIntoTwoLineChangedCommandBehavior<T> : RibbonButtonSplitLabelIntoTwoLineChangedCommandBehavior
    { }
	#endregion
}


