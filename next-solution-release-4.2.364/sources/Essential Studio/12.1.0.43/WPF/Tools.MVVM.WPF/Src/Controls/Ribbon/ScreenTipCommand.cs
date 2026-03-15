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

	#region ScreenTipDescriptionChangedCommand
	// ScreenTipDescriptionChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ScreenTipDescriptionChangedCommand : ControlCommandBase<ScreenTipDescriptionChangedCommandBehavior, ScreenTip>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ScreenTipDescriptionChangedCommandBehavior : CommandBehaviorBase<ScreenTip>
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
            TargetObject.DescriptionChanged += OnEventRaised;
        }
    }

	// ScreenTipDescriptionChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ScreenTipDescriptionChangedCommandBehavior<T> : ScreenTipDescriptionChangedCommandBehavior
    { }
	#endregion

	#region ScreenTipHelpTextChangedCommand
	// ScreenTipHelpTextChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ScreenTipHelpTextChangedCommand : ControlCommandBase<ScreenTipHelpTextChangedCommandBehavior, ScreenTip>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ScreenTipHelpTextChangedCommandBehavior : CommandBehaviorBase<ScreenTip>
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
            TargetObject.HelpTextChanged += OnEventRaised;
        }
    }

	// ScreenTipHelpTextChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ScreenTipHelpTextChangedCommandBehavior<T> : ScreenTipHelpTextChangedCommandBehavior
    { }
	#endregion

	#region ScreenTipImageSourceChangedCommand
	// ScreenTipImageSourceChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ScreenTipImageSourceChangedCommand : ControlCommandBase<ScreenTipImageSourceChangedCommandBehavior, ScreenTip>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ScreenTipImageSourceChangedCommandBehavior : CommandBehaviorBase<ScreenTip>
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
            TargetObject.ImageSourceChanged += OnEventRaised;
        }
    }

	// ScreenTipImageSourceChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ScreenTipImageSourceChangedCommandBehavior<T> : ScreenTipImageSourceChangedCommandBehavior
    { }
	#endregion

	#region ScreenTipIsOpenChangedCommand
	// ScreenTipIsOpenChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ScreenTipIsOpenChangedCommand : ControlCommandBase<ScreenTipIsOpenChangedCommandBehavior, ScreenTip>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ScreenTipIsOpenChangedCommandBehavior : CommandBehaviorBase<ScreenTip>
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
            TargetObject.IsOpenChanged += OnEventRaised;
        }
    }

	// ScreenTipIsOpenChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ScreenTipIsOpenChangedCommandBehavior<T> : ScreenTipIsOpenChangedCommandBehavior
    { }
	#endregion
}


