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

	#region RibbonTabCaptionChangedCommand
	// RibbonTabCaptionChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class RibbonTabCaptionChangedCommand : ControlCommandBase<RibbonTabCaptionChangedCommandBehavior, RibbonTab>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class RibbonTabCaptionChangedCommandBehavior : CommandBehaviorBase<RibbonTab>
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
            TargetObject.CaptionChanged += OnEventRaised;
        }
    }

	// RibbonTabCaptionChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RibbonTabCaptionChangedCommandBehavior<T> : RibbonTabCaptionChangedCommandBehavior
    { }
	#endregion

	#region RibbonTabVisibilityChangedCommand
	// RibbonTabVisibilityChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class RibbonTabVisibilityChangedCommand : ControlCommandBase<RibbonTabVisibilityChangedCommandBehavior, RibbonTab>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class RibbonTabVisibilityChangedCommandBehavior : CommandBehaviorBase<RibbonTab>
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
            TargetObject.VisibilityChanged += OnEventRaised;
        }
    }

	// RibbonTabVisibilityChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RibbonTabVisibilityChangedCommandBehavior<T> : RibbonTabVisibilityChangedCommandBehavior
    { }
	#endregion

	#region RibbonTabContextColorChangedCommand
	// RibbonTabContextColorChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class RibbonTabContextColorChangedCommand : ControlCommandBase<RibbonTabContextColorChangedCommandBehavior, RibbonTab>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class RibbonTabContextColorChangedCommandBehavior : CommandBehaviorBase<RibbonTab>
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
            TargetObject.ContextColorChanged += OnEventRaised;
        }
    }

	// RibbonTabContextColorChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RibbonTabContextColorChangedCommandBehavior<T> : RibbonTabContextColorChangedCommandBehavior
    { }
	#endregion

	#region RibbonTabTabButtonTemplateChangedCommand
	// RibbonTabTabButtonTemplateChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class RibbonTabTabButtonTemplateChangedCommand : ControlCommandBase<RibbonTabTabButtonTemplateChangedCommandBehavior, RibbonTab>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class RibbonTabTabButtonTemplateChangedCommandBehavior : CommandBehaviorBase<RibbonTab>
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
            TargetObject.TabButtonTemplateChanged += OnEventRaised;
        }
    }

	// RibbonTabTabButtonTemplateChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RibbonTabTabButtonTemplateChangedCommandBehavior<T> : RibbonTabTabButtonTemplateChangedCommandBehavior
    { }
	#endregion

	#region RibbonTabTabButtonStyleChangedCommand
	// RibbonTabTabButtonStyleChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class RibbonTabTabButtonStyleChangedCommand : ControlCommandBase<RibbonTabTabButtonStyleChangedCommandBehavior, RibbonTab>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class RibbonTabTabButtonStyleChangedCommandBehavior : CommandBehaviorBase<RibbonTab>
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
            TargetObject.TabButtonStyleChanged += OnEventRaised;
        }
    }

	// RibbonTabTabButtonStyleChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RibbonTabTabButtonStyleChangedCommandBehavior<T> : RibbonTabTabButtonStyleChangedCommandBehavior
    { }
	#endregion
}


