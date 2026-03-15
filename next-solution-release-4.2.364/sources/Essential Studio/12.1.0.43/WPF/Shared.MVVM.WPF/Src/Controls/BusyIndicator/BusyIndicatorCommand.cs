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

	#region BusyIndicatorIsBusyChangedCommand
	// BusyIndicatorIsBusyChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class BusyIndicatorIsBusyChangedCommand : ControlCommandBase<BusyIndicatorIsBusyChangedCommandBehavior, BusyIndicator>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class BusyIndicatorIsBusyChangedCommandBehavior : CommandBehaviorBase<BusyIndicator>
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
            TargetObject.IsBusyChanged += OnEventRaised;
        }
    }

	// BusyIndicatorIsBusyChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BusyIndicatorIsBusyChangedCommandBehavior<T> : BusyIndicatorIsBusyChangedCommandBehavior
    { }
	#endregion

	#region BusyIndicatorDescriptionPlacementChangedCommand
	// BusyIndicatorDescriptionPlacementChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class BusyIndicatorDescriptionPlacementChangedCommand : ControlCommandBase<BusyIndicatorDescriptionPlacementChangedCommandBehavior, BusyIndicator>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class BusyIndicatorDescriptionPlacementChangedCommandBehavior : CommandBehaviorBase<BusyIndicator>
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
            TargetObject.DescriptionPlacementChanged += OnEventRaised;
        }
    }

	// BusyIndicatorDescriptionPlacementChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BusyIndicatorDescriptionPlacementChangedCommandBehavior<T> : BusyIndicatorDescriptionPlacementChangedCommandBehavior
    { }
	#endregion

	#region BusyIndicatorProgressValueChangedCommand
	// BusyIndicatorProgressValueChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class BusyIndicatorProgressValueChangedCommand : ControlCommandBase<BusyIndicatorProgressValueChangedCommandBehavior, BusyIndicator>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class BusyIndicatorProgressValueChangedCommandBehavior : CommandBehaviorBase<BusyIndicator>
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
            TargetObject.ProgressValueChanged += OnEventRaised;
        }
    }

	// BusyIndicatorProgressValueChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BusyIndicatorProgressValueChangedCommandBehavior<T> : BusyIndicatorProgressValueChangedCommandBehavior
    { }
	#endregion

	#region BusyIndicatorEnableGrayScaleEffectChangedCommand
	// BusyIndicatorEnableGrayScaleEffectChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class BusyIndicatorEnableGrayScaleEffectChangedCommand : ControlCommandBase<BusyIndicatorEnableGrayScaleEffectChangedCommandBehavior, BusyIndicator>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class BusyIndicatorEnableGrayScaleEffectChangedCommandBehavior : CommandBehaviorBase<BusyIndicator>
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
            TargetObject.EnableGrayScaleEffectChanged += OnEventRaised;
        }
    }

	// BusyIndicatorEnableGrayScaleEffectChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BusyIndicatorEnableGrayScaleEffectChangedCommandBehavior<T> : BusyIndicatorEnableGrayScaleEffectChangedCommandBehavior
    { }
	#endregion

	#region BusyIndicatorCancelClickCommand
	// BusyIndicatorCancelClickCommand
    /// <summary>
    /// 
    /// </summary>
	public class BusyIndicatorCancelClickCommand : ControlCommandBase<BusyIndicatorCancelClickCommandBehavior, BusyIndicator>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class BusyIndicatorCancelClickCommandBehavior : CommandBehaviorBase<BusyIndicator>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, CancelEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.CancelClick += OnEventRaised;
        }
    }

	// BusyIndicatorCancelClickCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BusyIndicatorCancelClickCommandBehavior<T> : BusyIndicatorCancelClickCommandBehavior
    { }
	#endregion

	#region BusyIndicatorClosingCommand
	// BusyIndicatorClosingCommand
    /// <summary>
    /// 
    /// </summary>
	public class BusyIndicatorClosingCommand : ControlCommandBase<BusyIndicatorClosingCommandBehavior, BusyIndicator>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class BusyIndicatorClosingCommandBehavior : CommandBehaviorBase<BusyIndicator>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, CancelEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.Closing += OnEventRaised;
        }
    }

	// BusyIndicatorClosingCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BusyIndicatorClosingCommandBehavior<T> : BusyIndicatorClosingCommandBehavior
    { }
	#endregion

	#region BusyIndicatorClosedCommand
	// BusyIndicatorClosedCommand
    /// <summary>
    /// 
    /// </summary>
	public class BusyIndicatorClosedCommand : ControlCommandBase<BusyIndicatorClosedCommandBehavior, BusyIndicator>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class BusyIndicatorClosedCommandBehavior : CommandBehaviorBase<BusyIndicator>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, RoutedEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.Closed += OnEventRaised;
        }
    }

	// BusyIndicatorClosedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BusyIndicatorClosedCommandBehavior<T> : BusyIndicatorClosedCommandBehavior
    { }
	#endregion
}


