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

	#region ClockClockCornerRadiusChangedCommand
	// ClockClockCornerRadiusChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ClockClockCornerRadiusChangedCommand : ControlCommandBase<ClockClockCornerRadiusChangedCommandBehavior, Clock>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ClockClockCornerRadiusChangedCommandBehavior : CommandBehaviorBase<Clock>
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
            TargetObject.ClockCornerRadiusChanged += OnEventRaised;
        }
    }

	// ClockClockCornerRadiusChangedCommandBehavior
    /// <summary>
    /// /
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClockClockCornerRadiusChangedCommandBehavior<T> : ClockClockCornerRadiusChangedCommandBehavior
    { }
	#endregion

	#region ClockBorderThicknessChangedCommand
	// ClockBorderThicknessChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ClockBorderThicknessChangedCommand : ControlCommandBase<ClockBorderThicknessChangedCommandBehavior, Clock>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ClockBorderThicknessChangedCommandBehavior : CommandBehaviorBase<Clock>
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
            TargetObject.BorderThicknessChanged += OnEventRaised;
        }
    }

	// ClockBorderThicknessChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClockBorderThicknessChangedCommandBehavior<T> : ClockBorderThicknessChangedCommandBehavior
    { }
	#endregion

	#region ClockSecondHandThicknessChangedCommand
	// ClockSecondHandThicknessChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ClockSecondHandThicknessChangedCommand : ControlCommandBase<ClockSecondHandThicknessChangedCommandBehavior, Clock>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ClockSecondHandThicknessChangedCommandBehavior : CommandBehaviorBase<Clock>
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
            TargetObject.SecondHandThicknessChanged += OnEventRaised;
        }
    }

	// ClockSecondHandThicknessChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClockSecondHandThicknessChangedCommandBehavior<T> : ClockSecondHandThicknessChangedCommandBehavior
    { }
	#endregion

	#region ClockInnerBorderThicknessChangedCommand
	// ClockInnerBorderThicknessChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ClockInnerBorderThicknessChangedCommand : ControlCommandBase<ClockInnerBorderThicknessChangedCommandBehavior, Clock>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ClockInnerBorderThicknessChangedCommandBehavior : CommandBehaviorBase<Clock>
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
            TargetObject.InnerBorderThicknessChanged += OnEventRaised;
        }
    }

	// ClockInnerBorderThicknessChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClockInnerBorderThicknessChangedCommandBehavior<T> : ClockInnerBorderThicknessChangedCommandBehavior
    { }
	#endregion

	#region ClockDialBorderThicknessChangedCommand
	// ClockDialBorderThicknessChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ClockDialBorderThicknessChangedCommand : ControlCommandBase<ClockDialBorderThicknessChangedCommandBehavior, Clock>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ClockDialBorderThicknessChangedCommandBehavior : CommandBehaviorBase<Clock>
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
            TargetObject.DialBorderThicknessChanged += OnEventRaised;
        }
    }

	// ClockDialBorderThicknessChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClockDialBorderThicknessChangedCommandBehavior<T> : ClockDialBorderThicknessChangedCommandBehavior
    { }
	#endregion

	#region ClockAMPMSelectorPositionChangedCommand
	// ClockAMPMSelectorPositionChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ClockAMPMSelectorPositionChangedCommand : ControlCommandBase<ClockAMPMSelectorPositionChangedCommandBehavior, Clock>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ClockAMPMSelectorPositionChangedCommandBehavior : CommandBehaviorBase<Clock>
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
            TargetObject.AMPMSelectorPositionChanged += OnEventRaised;
        }
    }

	// ClockAMPMSelectorPositionChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClockAMPMSelectorPositionChangedCommandBehavior<T> : ClockAMPMSelectorPositionChangedCommandBehavior
    { }
	#endregion

	#region ClockDateTimeChangedCommand
	// ClockDateTimeChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ClockDateTimeChangedCommand : ControlCommandBase<ClockDateTimeChangedCommandBehavior, Clock>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ClockDateTimeChangedCommandBehavior : CommandBehaviorBase<Clock>
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
            TargetObject.DateTimeChanged += OnEventRaised;
        }
    }

	// ClockDateTimeChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClockDateTimeChangedCommandBehavior<T> : ClockDateTimeChangedCommandBehavior
    { }
	#endregion

	#region ClockBorderBrushChangedCommand
	// ClockBorderBrushChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ClockBorderBrushChangedCommand : ControlCommandBase<ClockBorderBrushChangedCommandBehavior, Clock>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ClockBorderBrushChangedCommandBehavior : CommandBehaviorBase<Clock>
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
            TargetObject.BorderBrushChanged += OnEventRaised;
        }
    }

	// ClockBorderBrushChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClockBorderBrushChangedCommandBehavior<T> : ClockBorderBrushChangedCommandBehavior
    { }
	#endregion

	#region ClockClockFrameBrushChangedCommand
	// ClockClockFrameBrushChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ClockClockFrameBrushChangedCommand : ControlCommandBase<ClockClockFrameBrushChangedCommandBehavior, Clock>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ClockClockFrameBrushChangedCommandBehavior : CommandBehaviorBase<Clock>
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
            TargetObject.ClockFrameBrushChanged += OnEventRaised;
        }
    }

	// ClockClockFrameBrushChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClockClockFrameBrushChangedCommandBehavior<T> : ClockClockFrameBrushChangedCommandBehavior
    { }
	#endregion

	#region ClockDialBackgroundChangedCommand
	// ClockDialBackgroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ClockDialBackgroundChangedCommand : ControlCommandBase<ClockDialBackgroundChangedCommandBehavior, Clock>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ClockDialBackgroundChangedCommandBehavior : CommandBehaviorBase<Clock>
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
            TargetObject.DialBackgroundChanged += OnEventRaised;
        }
    }

	// ClockDialBackgroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClockDialBackgroundChangedCommandBehavior<T> : ClockDialBackgroundChangedCommandBehavior
    { }
	#endregion

	#region ClockDialCenterBackgroundChangedCommand
	// ClockDialCenterBackgroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ClockDialCenterBackgroundChangedCommand : ControlCommandBase<ClockDialCenterBackgroundChangedCommandBehavior, Clock>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ClockDialCenterBackgroundChangedCommandBehavior : CommandBehaviorBase<Clock>
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
            TargetObject.DialCenterBackgroundChanged += OnEventRaised;
        }
    }

	// ClockDialCenterBackgroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClockDialCenterBackgroundChangedCommandBehavior<T> : ClockDialCenterBackgroundChangedCommandBehavior
    { }
	#endregion

	#region ClockInnerBorderBrushChangedCommand
	// ClockInnerBorderBrushChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ClockInnerBorderBrushChangedCommand : ControlCommandBase<ClockInnerBorderBrushChangedCommandBehavior, Clock>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ClockInnerBorderBrushChangedCommandBehavior : CommandBehaviorBase<Clock>
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
            TargetObject.InnerBorderBrushChanged += OnEventRaised;
        }
    }

	// ClockInnerBorderBrushChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClockInnerBorderBrushChangedCommandBehavior<T> : ClockInnerBorderBrushChangedCommandBehavior
    { }
	#endregion

	#region ClockFrameBorderThicknessChangedCommand
	// ClockFrameBorderThicknessChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ClockFrameBorderThicknessChangedCommand : ControlCommandBase<ClockFrameBorderThicknessChangedCommandBehavior, Clock>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ClockFrameBorderThicknessChangedCommandBehavior : CommandBehaviorBase<Clock>
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
            TargetObject.FrameBorderThicknessChanged += OnEventRaised;
        }
    }

	// ClockFrameBorderThicknessChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClockFrameBorderThicknessChangedCommandBehavior<T> : ClockFrameBorderThicknessChangedCommandBehavior
    { }
	#endregion

	#region ClockFrameInnerBorderThicknessChangedCommand
	// ClockFrameInnerBorderThicknessChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ClockFrameInnerBorderThicknessChangedCommand : ControlCommandBase<ClockFrameInnerBorderThicknessChangedCommandBehavior, Clock>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ClockFrameInnerBorderThicknessChangedCommandBehavior : CommandBehaviorBase<Clock>
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
            TargetObject.FrameInnerBorderThicknessChanged += OnEventRaised;
        }
    }

	// ClockFrameInnerBorderThicknessChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClockFrameInnerBorderThicknessChangedCommandBehavior<T> : ClockFrameInnerBorderThicknessChangedCommandBehavior
    { }
	#endregion

	#region ClockFrameBorderBrushChangedCommand
	// ClockFrameBorderBrushChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ClockFrameBorderBrushChangedCommand : ControlCommandBase<ClockFrameBorderBrushChangedCommandBehavior, Clock>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ClockFrameBorderBrushChangedCommandBehavior : CommandBehaviorBase<Clock>
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
            TargetObject.FrameBorderBrushChanged += OnEventRaised;
        }
    }

	// ClockFrameBorderBrushChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClockFrameBorderBrushChangedCommandBehavior<T> : ClockFrameBorderBrushChangedCommandBehavior
    { }
	#endregion

	#region ClockFrameInnerBorderBrushChangedCommand
	// ClockFrameInnerBorderBrushChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ClockFrameInnerBorderBrushChangedCommand : ControlCommandBase<ClockFrameInnerBorderBrushChangedCommandBehavior, Clock>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ClockFrameInnerBorderBrushChangedCommandBehavior : CommandBehaviorBase<Clock>
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
            TargetObject.FrameInnerBorderBrushChanged += OnEventRaised;
        }
    }

	// ClockFrameInnerBorderBrushChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClockFrameInnerBorderBrushChangedCommandBehavior<T> : ClockFrameInnerBorderBrushChangedCommandBehavior
    { }
	#endregion

	#region ClockFrameBackgroundChangedCommand
	// ClockFrameBackgroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ClockFrameBackgroundChangedCommand : ControlCommandBase<ClockFrameBackgroundChangedCommandBehavior, Clock>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ClockFrameBackgroundChangedCommandBehavior : CommandBehaviorBase<Clock>
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
            TargetObject.FrameBackgroundChanged += OnEventRaised;
        }
    }

	// ClockFrameBackgroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClockFrameBackgroundChangedCommandBehavior<T> : ClockFrameBackgroundChangedCommandBehavior
    { }
	#endregion

	#region ClockFrameCornerRadiusChangedCommand
	// ClockFrameCornerRadiusChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ClockFrameCornerRadiusChangedCommand : ControlCommandBase<ClockFrameCornerRadiusChangedCommandBehavior, Clock>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ClockFrameCornerRadiusChangedCommandBehavior : CommandBehaviorBase<Clock>
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
            TargetObject.FrameCornerRadiusChanged += OnEventRaised;
        }
    }

	// ClockFrameCornerRadiusChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClockFrameCornerRadiusChangedCommandBehavior<T> : ClockFrameCornerRadiusChangedCommandBehavior
    { }
	#endregion

	#region ClockAMPMSelectorBorderThicknessChangedCommand
	// ClockAMPMSelectorBorderThicknessChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ClockAMPMSelectorBorderThicknessChangedCommand : ControlCommandBase<ClockAMPMSelectorBorderThicknessChangedCommandBehavior, Clock>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ClockAMPMSelectorBorderThicknessChangedCommandBehavior : CommandBehaviorBase<Clock>
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
            TargetObject.AMPMSelectorBorderThicknessChanged += OnEventRaised;
        }
    }

	// ClockAMPMSelectorBorderThicknessChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClockAMPMSelectorBorderThicknessChangedCommandBehavior<T> : ClockAMPMSelectorBorderThicknessChangedCommandBehavior
    { }
	#endregion

	#region ClockAMPMSelectorBorderBrushChangedCommand
	// ClockAMPMSelectorBorderBrushChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ClockAMPMSelectorBorderBrushChangedCommand : ControlCommandBase<ClockAMPMSelectorBorderBrushChangedCommandBehavior, Clock>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ClockAMPMSelectorBorderBrushChangedCommandBehavior : CommandBehaviorBase<Clock>
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
            TargetObject.AMPMSelectorBorderBrushChanged += OnEventRaised;
        }
    }

	// ClockAMPMSelectorBorderBrushChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClockAMPMSelectorBorderBrushChangedCommandBehavior<T> : ClockAMPMSelectorBorderBrushChangedCommandBehavior
    { }
	#endregion

	#region ClockAMPMSelectorBackgroundChangedCommand
	// ClockAMPMSelectorBackgroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ClockAMPMSelectorBackgroundChangedCommand : ControlCommandBase<ClockAMPMSelectorBackgroundChangedCommandBehavior, Clock>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ClockAMPMSelectorBackgroundChangedCommandBehavior : CommandBehaviorBase<Clock>
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
            TargetObject.AMPMSelectorBackgroundChanged += OnEventRaised;
        }
    }

	// ClockAMPMSelectorBackgroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClockAMPMSelectorBackgroundChangedCommandBehavior<T> : ClockAMPMSelectorBackgroundChangedCommandBehavior
    { }
	#endregion

	#region ClockAMPMSelectorForegroundChangedCommand
	// ClockAMPMSelectorForegroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ClockAMPMSelectorForegroundChangedCommand : ControlCommandBase<ClockAMPMSelectorForegroundChangedCommandBehavior, Clock>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ClockAMPMSelectorForegroundChangedCommandBehavior : CommandBehaviorBase<Clock>
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
            TargetObject.AMPMSelectorForegroundChanged += OnEventRaised;
        }
    }

	// ClockAMPMSelectorForegroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClockAMPMSelectorForegroundChangedCommandBehavior<T> : ClockAMPMSelectorForegroundChangedCommandBehavior
    { }
	#endregion

	#region ClockAMPMSelectorButtonsArrowBrushChangedCommand
	// ClockAMPMSelectorButtonsArrowBrushChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ClockAMPMSelectorButtonsArrowBrushChangedCommand : ControlCommandBase<ClockAMPMSelectorButtonsArrowBrushChangedCommandBehavior, Clock>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ClockAMPMSelectorButtonsArrowBrushChangedCommandBehavior : CommandBehaviorBase<Clock>
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
            TargetObject.AMPMSelectorButtonsArrowBrushChanged += OnEventRaised;
        }
    }

	// ClockAMPMSelectorButtonsArrowBrushChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClockAMPMSelectorButtonsArrowBrushChangedCommandBehavior<T> : ClockAMPMSelectorButtonsArrowBrushChangedCommandBehavior
    { }
	#endregion

	#region ClockAMPMSelectorButtonsBackgroundChangedCommand
	// ClockAMPMSelectorButtonsBackgroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ClockAMPMSelectorButtonsBackgroundChangedCommand : ControlCommandBase<ClockAMPMSelectorButtonsBackgroundChangedCommandBehavior, Clock>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ClockAMPMSelectorButtonsBackgroundChangedCommandBehavior : CommandBehaviorBase<Clock>
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
            TargetObject.AMPMSelectorButtonsBackgroundChanged += OnEventRaised;
        }
    }

	// ClockAMPMSelectorButtonsBackgroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClockAMPMSelectorButtonsBackgroundChangedCommandBehavior<T> : ClockAMPMSelectorButtonsBackgroundChangedCommandBehavior
    { }
	#endregion

	#region ClockAMPMSelectorCornerRadiusChangedCommand
	// ClockAMPMSelectorCornerRadiusChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ClockAMPMSelectorCornerRadiusChangedCommand : ControlCommandBase<ClockAMPMSelectorCornerRadiusChangedCommandBehavior, Clock>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ClockAMPMSelectorCornerRadiusChangedCommandBehavior : CommandBehaviorBase<Clock>
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
            TargetObject.AMPMSelectorCornerRadiusChanged += OnEventRaised;
        }
    }

	// ClockAMPMSelectorCornerRadiusChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClockAMPMSelectorCornerRadiusChangedCommandBehavior<T> : ClockAMPMSelectorCornerRadiusChangedCommandBehavior
    { }
	#endregion

	#region ClockAMPMSelectorButtonsBorderBrushChangedCommand
	// ClockAMPMSelectorButtonsBorderBrushChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ClockAMPMSelectorButtonsBorderBrushChangedCommand : ControlCommandBase<ClockAMPMSelectorButtonsBorderBrushChangedCommandBehavior, Clock>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ClockAMPMSelectorButtonsBorderBrushChangedCommandBehavior : CommandBehaviorBase<Clock>
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
            TargetObject.AMPMSelectorButtonsBorderBrushChanged += OnEventRaised;
        }
    }

	// ClockAMPMSelectorButtonsBorderBrushChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClockAMPMSelectorButtonsBorderBrushChangedCommandBehavior<T> : ClockAMPMSelectorButtonsBorderBrushChangedCommandBehavior
    { }
	#endregion

	#region ClockAMPMMouseOverButtonsBorderBrushChangedCommand
	// ClockAMPMMouseOverButtonsBorderBrushChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ClockAMPMMouseOverButtonsBorderBrushChangedCommand : ControlCommandBase<ClockAMPMMouseOverButtonsBorderBrushChangedCommandBehavior, Clock>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ClockAMPMMouseOverButtonsBorderBrushChangedCommandBehavior : CommandBehaviorBase<Clock>
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
            TargetObject.AMPMMouseOverButtonsBorderBrushChanged += OnEventRaised;
        }
    }

	// ClockAMPMMouseOverButtonsBorderBrushChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClockAMPMMouseOverButtonsBorderBrushChangedCommandBehavior<T> : ClockAMPMMouseOverButtonsBorderBrushChangedCommandBehavior
    { }
	#endregion

	#region ClockAMPMMouseOverButtonsArrowBrushChangedCommand
	// ClockAMPMMouseOverButtonsArrowBrushChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ClockAMPMMouseOverButtonsArrowBrushChangedCommand : ControlCommandBase<ClockAMPMMouseOverButtonsArrowBrushChangedCommandBehavior, Clock>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ClockAMPMMouseOverButtonsArrowBrushChangedCommandBehavior : CommandBehaviorBase<Clock>
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
            TargetObject.AMPMMouseOverButtonsArrowBrushChanged += OnEventRaised;
        }
    }

	// ClockAMPMMouseOverButtonsArrowBrushChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClockAMPMMouseOverButtonsArrowBrushChangedCommandBehavior<T> : ClockAMPMMouseOverButtonsArrowBrushChangedCommandBehavior
    { }
	#endregion

	#region ClockAMPMMouseOverButtonsBackgroundChangedCommand
	// ClockAMPMMouseOverButtonsBackgroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ClockAMPMMouseOverButtonsBackgroundChangedCommand : ControlCommandBase<ClockAMPMMouseOverButtonsBackgroundChangedCommandBehavior, Clock>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ClockAMPMMouseOverButtonsBackgroundChangedCommandBehavior : CommandBehaviorBase<Clock>
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
            TargetObject.AMPMMouseOverButtonsBackgroundChanged += OnEventRaised;
        }
    }

	// ClockAMPMMouseOverButtonsBackgroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClockAMPMMouseOverButtonsBackgroundChangedCommandBehavior<T> : ClockAMPMMouseOverButtonsBackgroundChangedCommandBehavior
    { }
	#endregion

	#region ClockClockPointBrushChangedCommand
	// ClockClockPointBrushChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ClockClockPointBrushChangedCommand : ControlCommandBase<ClockClockPointBrushChangedCommandBehavior, Clock>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ClockClockPointBrushChangedCommandBehavior : CommandBehaviorBase<Clock>
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
            TargetObject.ClockPointBrushChanged += OnEventRaised;
        }
    }

	// ClockClockPointBrushChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClockClockPointBrushChangedCommandBehavior<T> : ClockClockPointBrushChangedCommandBehavior
    { }
	#endregion

	#region ClockCenterCircleBrushChangedCommand
	// ClockCenterCircleBrushChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ClockCenterCircleBrushChangedCommand : ControlCommandBase<ClockCenterCircleBrushChangedCommandBehavior, Clock>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ClockCenterCircleBrushChangedCommandBehavior : CommandBehaviorBase<Clock>
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
            TargetObject.CenterCircleBrushChanged += OnEventRaised;
        }
    }

	// ClockCenterCircleBrushChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClockCenterCircleBrushChangedCommandBehavior<T> : ClockCenterCircleBrushChangedCommandBehavior
    { }
	#endregion

	#region ClockSecondHandBrushChangedCommand
	// ClockSecondHandBrushChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ClockSecondHandBrushChangedCommand : ControlCommandBase<ClockSecondHandBrushChangedCommandBehavior, Clock>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ClockSecondHandBrushChangedCommandBehavior : CommandBehaviorBase<Clock>
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
            TargetObject.SecondHandBrushChanged += OnEventRaised;
        }
    }

	// ClockSecondHandBrushChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClockSecondHandBrushChangedCommandBehavior<T> : ClockSecondHandBrushChangedCommandBehavior
    { }
	#endregion

	#region ClockSecondHandMouseOverBrushChangedCommand
	// ClockSecondHandMouseOverBrushChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ClockSecondHandMouseOverBrushChangedCommand : ControlCommandBase<ClockSecondHandMouseOverBrushChangedCommandBehavior, Clock>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ClockSecondHandMouseOverBrushChangedCommandBehavior : CommandBehaviorBase<Clock>
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
            TargetObject.SecondHandMouseOverBrushChanged += OnEventRaised;
        }
    }

	// ClockSecondHandMouseOverBrushChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClockSecondHandMouseOverBrushChangedCommandBehavior<T> : ClockSecondHandMouseOverBrushChangedCommandBehavior
    { }
	#endregion

	#region ClockMinuteHandBrushChangedCommand
	// ClockMinuteHandBrushChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ClockMinuteHandBrushChangedCommand : ControlCommandBase<ClockMinuteHandBrushChangedCommandBehavior, Clock>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ClockMinuteHandBrushChangedCommandBehavior : CommandBehaviorBase<Clock>
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
            TargetObject.MinuteHandBrushChanged += OnEventRaised;
        }
    }

	// ClockMinuteHandBrushChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClockMinuteHandBrushChangedCommandBehavior<T> : ClockMinuteHandBrushChangedCommandBehavior
    { }
	#endregion

	#region ClockMinuteHandBorderBrushChangedCommand
	// ClockMinuteHandBorderBrushChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ClockMinuteHandBorderBrushChangedCommand : ControlCommandBase<ClockMinuteHandBorderBrushChangedCommandBehavior, Clock>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ClockMinuteHandBorderBrushChangedCommandBehavior : CommandBehaviorBase<Clock>
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
            TargetObject.MinuteHandBorderBrushChanged += OnEventRaised;
        }
    }

	// ClockMinuteHandBorderBrushChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClockMinuteHandBorderBrushChangedCommandBehavior<T> : ClockMinuteHandBorderBrushChangedCommandBehavior
    { }
	#endregion

	#region ClockMinuteHandMouseOverBrushChangedCommand
	// ClockMinuteHandMouseOverBrushChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ClockMinuteHandMouseOverBrushChangedCommand : ControlCommandBase<ClockMinuteHandMouseOverBrushChangedCommandBehavior, Clock>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ClockMinuteHandMouseOverBrushChangedCommandBehavior : CommandBehaviorBase<Clock>
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
            TargetObject.MinuteHandMouseOverBrushChanged += OnEventRaised;
        }
    }

	// ClockMinuteHandMouseOverBrushChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClockMinuteHandMouseOverBrushChangedCommandBehavior<T> : ClockMinuteHandMouseOverBrushChangedCommandBehavior
    { }
	#endregion

	#region ClockMinuteHandMouseOverBorderBrushChangedCommand
	// ClockMinuteHandMouseOverBorderBrushChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ClockMinuteHandMouseOverBorderBrushChangedCommand : ControlCommandBase<ClockMinuteHandMouseOverBorderBrushChangedCommandBehavior, Clock>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ClockMinuteHandMouseOverBorderBrushChangedCommandBehavior : CommandBehaviorBase<Clock>
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
            TargetObject.MinuteHandMouseOverBorderBrushChanged += OnEventRaised;
        }
    }

	// ClockMinuteHandMouseOverBorderBrushChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClockMinuteHandMouseOverBorderBrushChangedCommandBehavior<T> : ClockMinuteHandMouseOverBorderBrushChangedCommandBehavior
    { }
	#endregion

	#region ClockHourHandBrushChangedCommand
	// ClockHourHandBrushChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ClockHourHandBrushChangedCommand : ControlCommandBase<ClockHourHandBrushChangedCommandBehavior, Clock>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ClockHourHandBrushChangedCommandBehavior : CommandBehaviorBase<Clock>
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
            TargetObject.HourHandBrushChanged += OnEventRaised;
        }
    }

	// ClockHourHandBrushChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClockHourHandBrushChangedCommandBehavior<T> : ClockHourHandBrushChangedCommandBehavior
    { }
	#endregion

	#region ClockHourHandBorderBrushChangedCommand
	// ClockHourHandBorderBrushChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ClockHourHandBorderBrushChangedCommand : ControlCommandBase<ClockHourHandBorderBrushChangedCommandBehavior, Clock>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ClockHourHandBorderBrushChangedCommandBehavior : CommandBehaviorBase<Clock>
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
            TargetObject.HourHandBorderBrushChanged += OnEventRaised;
        }
    }

	// ClockHourHandBorderBrushChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClockHourHandBorderBrushChangedCommandBehavior<T> : ClockHourHandBorderBrushChangedCommandBehavior
    { }
	#endregion

	#region ClockHourHandMouseOverBrushChangedCommand
	// ClockHourHandMouseOverBrushChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ClockHourHandMouseOverBrushChangedCommand : ControlCommandBase<ClockHourHandMouseOverBrushChangedCommandBehavior, Clock>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ClockHourHandMouseOverBrushChangedCommandBehavior : CommandBehaviorBase<Clock>
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
            TargetObject.HourHandMouseOverBrushChanged += OnEventRaised;
        }
    }

	// ClockHourHandMouseOverBrushChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClockHourHandMouseOverBrushChangedCommandBehavior<T> : ClockHourHandMouseOverBrushChangedCommandBehavior
    { }
	#endregion

	#region ClockHourHandMouseOverBorderBrushChangedCommand
	// ClockHourHandMouseOverBorderBrushChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ClockHourHandMouseOverBorderBrushChangedCommand : ControlCommandBase<ClockHourHandMouseOverBorderBrushChangedCommandBehavior, Clock>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ClockHourHandMouseOverBorderBrushChangedCommandBehavior : CommandBehaviorBase<Clock>
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
            TargetObject.HourHandMouseOverBorderBrushChanged += OnEventRaised;
        }
    }

	// ClockHourHandMouseOverBorderBrushChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClockHourHandMouseOverBorderBrushChangedCommandBehavior<T> : ClockHourHandMouseOverBorderBrushChangedCommandBehavior
    { }
	#endregion

	#region ClockHourHandPressedBrushChangedCommand
	// ClockHourHandPressedBrushChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ClockHourHandPressedBrushChangedCommand : ControlCommandBase<ClockHourHandPressedBrushChangedCommandBehavior, Clock>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ClockHourHandPressedBrushChangedCommandBehavior : CommandBehaviorBase<Clock>
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
            TargetObject.HourHandPressedBrushChanged += OnEventRaised;
        }
    }

	// ClockHourHandPressedBrushChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClockHourHandPressedBrushChangedCommandBehavior<T> : ClockHourHandPressedBrushChangedCommandBehavior
    { }
	#endregion

	#region ClockMinuteHandPressedBrushChangedCommand
	// ClockMinuteHandPressedBrushChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ClockMinuteHandPressedBrushChangedCommand : ControlCommandBase<ClockMinuteHandPressedBrushChangedCommandBehavior, Clock>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ClockMinuteHandPressedBrushChangedCommandBehavior : CommandBehaviorBase<Clock>
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
            TargetObject.MinuteHandPressedBrushChanged += OnEventRaised;
        }
    }

	// ClockMinuteHandPressedBrushChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClockMinuteHandPressedBrushChangedCommandBehavior<T> : ClockMinuteHandPressedBrushChangedCommandBehavior
    { }
	#endregion

	#region ClockSecondHandPressedBrushChangedCommand
	// ClockSecondHandPressedBrushChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ClockSecondHandPressedBrushChangedCommand : ControlCommandBase<ClockSecondHandPressedBrushChangedCommandBehavior, Clock>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ClockSecondHandPressedBrushChangedCommandBehavior : CommandBehaviorBase<Clock>
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
            TargetObject.SecondHandPressedBrushChanged += OnEventRaised;
        }
    }

	// ClockSecondHandPressedBrushChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClockSecondHandPressedBrushChangedCommandBehavior<T> : ClockSecondHandPressedBrushChangedCommandBehavior
    { }
	#endregion

	#region ClockIsInsideAmPmVisibleChangedCommand
	// ClockIsInsideAmPmVisibleChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ClockIsInsideAmPmVisibleChangedCommand : ControlCommandBase<ClockIsInsideAmPmVisibleChangedCommandBehavior, Clock>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ClockIsInsideAmPmVisibleChangedCommandBehavior : CommandBehaviorBase<Clock>
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
            TargetObject.IsInsideAmPmVisibleChanged += OnEventRaised;
        }
    }

	// ClockIsInsideAmPmVisibleChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClockIsInsideAmPmVisibleChangedCommandBehavior<T> : ClockIsInsideAmPmVisibleChangedCommandBehavior
    { }
	#endregion

	#region ClockIsDigitalAmPmVisibleChangedCommand
	// ClockIsDigitalAmPmVisibleChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class ClockIsDigitalAmPmVisibleChangedCommand : ControlCommandBase<ClockIsDigitalAmPmVisibleChangedCommandBehavior, Clock>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class ClockIsDigitalAmPmVisibleChangedCommandBehavior : CommandBehaviorBase<Clock>
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
            TargetObject.IsDigitalAmPmVisibleChanged += OnEventRaised;
        }
    }

	// ClockIsDigitalAmPmVisibleChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClockIsDigitalAmPmVisibleChangedCommandBehavior<T> : ClockIsDigitalAmPmVisibleChangedCommandBehavior
    { }
	#endregion
}


