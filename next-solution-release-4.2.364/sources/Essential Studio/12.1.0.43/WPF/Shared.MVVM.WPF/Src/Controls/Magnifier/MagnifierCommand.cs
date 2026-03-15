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

	#region MagnifierFrameTypeChangedCommand
	// MagnifierFrameTypeChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class MagnifierFrameTypeChangedCommand : ControlCommandBase<MagnifierFrameTypeChangedCommandBehavior, Magnifier>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class MagnifierFrameTypeChangedCommandBehavior : CommandBehaviorBase<Magnifier>
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
            TargetObject.FrameTypeChanged += OnEventRaised;
        }
    }

	// MagnifierFrameTypeChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MagnifierFrameTypeChangedCommandBehavior<T> : MagnifierFrameTypeChangedCommandBehavior
    { }
	#endregion

	#region MagnifierFrameHeightChangedCommand
	// MagnifierFrameHeightChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class MagnifierFrameHeightChangedCommand : ControlCommandBase<MagnifierFrameHeightChangedCommandBehavior, Magnifier>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class MagnifierFrameHeightChangedCommandBehavior : CommandBehaviorBase<Magnifier>
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
            TargetObject.FrameHeightChanged += OnEventRaised;
        }
    }

	// MagnifierFrameHeightChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MagnifierFrameHeightChangedCommandBehavior<T> : MagnifierFrameHeightChangedCommandBehavior
    { }
	#endregion

	#region MagnifierFrameWidthChangedCommand
	// MagnifierFrameWidthChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class MagnifierFrameWidthChangedCommand : ControlCommandBase<MagnifierFrameWidthChangedCommandBehavior, Magnifier>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class MagnifierFrameWidthChangedCommandBehavior : CommandBehaviorBase<Magnifier>
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
            TargetObject.FrameWidthChanged += OnEventRaised;
        }
    }

	// MagnifierFrameWidthChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MagnifierFrameWidthChangedCommandBehavior<T> : MagnifierFrameWidthChangedCommandBehavior
    { }
	#endregion

	#region MagnifierFrameRadiusChangedCommand
	// MagnifierFrameRadiusChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class MagnifierFrameRadiusChangedCommand : ControlCommandBase<MagnifierFrameRadiusChangedCommandBehavior, Magnifier>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class MagnifierFrameRadiusChangedCommandBehavior : CommandBehaviorBase<Magnifier>
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
            TargetObject.FrameRadiusChanged += OnEventRaised;
        }
    }

	// MagnifierFrameRadiusChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MagnifierFrameRadiusChangedCommandBehavior<T> : MagnifierFrameRadiusChangedCommandBehavior
    { }
	#endregion

	#region MagnifierFrameCornerRadiusChangedCommand
	// MagnifierFrameCornerRadiusChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class MagnifierFrameCornerRadiusChangedCommand : ControlCommandBase<MagnifierFrameCornerRadiusChangedCommandBehavior, Magnifier>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class MagnifierFrameCornerRadiusChangedCommandBehavior : CommandBehaviorBase<Magnifier>
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

	// MagnifierFrameCornerRadiusChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MagnifierFrameCornerRadiusChangedCommandBehavior<T> : MagnifierFrameCornerRadiusChangedCommandBehavior
    { }
	#endregion

	#region MagnifierFrameBackgroundChangedCommand
	// MagnifierFrameBackgroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class MagnifierFrameBackgroundChangedCommand : ControlCommandBase<MagnifierFrameBackgroundChangedCommandBehavior, Magnifier>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class MagnifierFrameBackgroundChangedCommandBehavior : CommandBehaviorBase<Magnifier>
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

	// MagnifierFrameBackgroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MagnifierFrameBackgroundChangedCommandBehavior<T> : MagnifierFrameBackgroundChangedCommandBehavior
    { }
	#endregion

	#region MagnifierZoomFactorChangedCommand
	// MagnifierZoomFactorChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class MagnifierZoomFactorChangedCommand : ControlCommandBase<MagnifierZoomFactorChangedCommandBehavior, Magnifier>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class MagnifierZoomFactorChangedCommandBehavior : CommandBehaviorBase<Magnifier>
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
            TargetObject.ZoomFactorChanged += OnEventRaised;
        }
    }

	// MagnifierZoomFactorChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MagnifierZoomFactorChangedCommandBehavior<T> : MagnifierZoomFactorChangedCommandBehavior
    { }
	#endregion

	#region MagnifierEnableExportChangedCommand
	// MagnifierEnableExportChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class MagnifierEnableExportChangedCommand : ControlCommandBase<MagnifierEnableExportChangedCommandBehavior, Magnifier>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class MagnifierEnableExportChangedCommandBehavior : CommandBehaviorBase<Magnifier>
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
            TargetObject.EnableExportChanged += OnEventRaised;
        }
    }

	// MagnifierEnableExportChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MagnifierEnableExportChangedCommandBehavior<T> : MagnifierEnableExportChangedCommandBehavior
    { }
	#endregion

	#region MagnifierTargetElementChangedCommand
	// MagnifierTargetElementChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class MagnifierTargetElementChangedCommand : ControlCommandBase<MagnifierTargetElementChangedCommandBehavior, Magnifier>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class MagnifierTargetElementChangedCommandBehavior : CommandBehaviorBase<Magnifier>
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
            TargetObject.TargetElementChanged += OnEventRaised;
        }
    }

	// MagnifierTargetElementChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MagnifierTargetElementChangedCommandBehavior<T> : MagnifierTargetElementChangedCommandBehavior
    { }
	#endregion
}


