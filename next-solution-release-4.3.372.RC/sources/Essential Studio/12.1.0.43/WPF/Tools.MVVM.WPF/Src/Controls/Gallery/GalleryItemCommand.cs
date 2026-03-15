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

	#region GalleryItemCaptionChangedCommand
	// GalleryItemCaptionChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryItemCaptionChangedCommand : ControlCommandBase<GalleryItemCaptionChangedCommandBehavior, GalleryItem>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryItemCaptionChangedCommandBehavior : CommandBehaviorBase<GalleryItem>
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

	// GalleryItemCaptionChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryItemCaptionChangedCommandBehavior<T> : GalleryItemCaptionChangedCommandBehavior
    { }
	#endregion

	#region GalleryItemWidthChangedCommand
	// GalleryItemWidthChangedCommand
    /// <summary>
    /// 
    /// </summary>
    public class GalleryItemWidthChangedCommand : ControlCommandBase<GalleryItemWidthChangedCommandBehavior, Syncfusion.Windows.Tools.Controls.GalleryItem>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryItemWidthChangedCommandBehavior : CommandBehaviorBase<Syncfusion.Windows.Tools.Controls.GalleryItem>
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
            TargetObject.WidthChanged += OnEventRaised;
        }
    }

	// GalleryItemWidthChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryItemWidthChangedCommandBehavior<T> : GalleryItemWidthChangedCommandBehavior
    { }
	#endregion

	#region GalleryItemHeightChangedCommand
	// GalleryItemHeightChangedCommand
    /// <summary>
    /// 
    /// </summary>
    public class GalleryItemHeightChangedCommand : ControlCommandBase<GalleryItemHeightChangedCommandBehavior, Syncfusion.Windows.Tools.Controls.GalleryItem>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryItemHeightChangedCommandBehavior : CommandBehaviorBase<Syncfusion.Windows.Tools.Controls.GalleryItem>
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
            TargetObject.HeightChanged += OnEventRaised;
        }
    }

	// GalleryItemHeightChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryItemHeightChangedCommandBehavior<T> : GalleryItemHeightChangedCommandBehavior
    { }
	#endregion

	#region GalleryItemDescriptionChangedCommand
	// GalleryItemDescriptionChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryItemDescriptionChangedCommand : ControlCommandBase<GalleryItemDescriptionChangedCommandBehavior, GalleryItem>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryItemDescriptionChangedCommandBehavior : CommandBehaviorBase<GalleryItem>
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

	// GalleryItemDescriptionChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryItemDescriptionChangedCommandBehavior<T> : GalleryItemDescriptionChangedCommandBehavior
    { }
	#endregion

	#region GalleryItemIsSelectedChangedCommand
	// GalleryItemIsSelectedChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryItemIsSelectedChangedCommand : ControlCommandBase<GalleryItemIsSelectedChangedCommandBehavior, GalleryItem>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryItemIsSelectedChangedCommandBehavior : CommandBehaviorBase<GalleryItem>
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

	// GalleryItemIsSelectedChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryItemIsSelectedChangedCommandBehavior<T> : GalleryItemIsSelectedChangedCommandBehavior
    { }
	#endregion

	#region GalleryItemVisibilityChangedCommand
	// GalleryItemVisibilityChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryItemVisibilityChangedCommand : ControlCommandBase<GalleryItemVisibilityChangedCommandBehavior, GalleryItem>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryItemVisibilityChangedCommandBehavior : CommandBehaviorBase<GalleryItem>
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

	// GalleryItemVisibilityChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryItemVisibilityChangedCommandBehavior<T> : GalleryItemVisibilityChangedCommandBehavior
    { }
	#endregion

	#region GalleryItemVisualModeChangedCommand
	// GalleryItemVisualModeChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryItemVisualModeChangedCommand : ControlCommandBase<GalleryItemVisualModeChangedCommandBehavior, GalleryItem>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryItemVisualModeChangedCommandBehavior : CommandBehaviorBase<GalleryItem>
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
            TargetObject.VisualModeChanged += OnEventRaised;
        }
    }

	// GalleryItemVisualModeChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryItemVisualModeChangedCommandBehavior<T> : GalleryItemVisualModeChangedCommandBehavior
    { }
	#endregion

	#region GalleryItemHasFocusChangedCommand
	// GalleryItemHasFocusChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryItemHasFocusChangedCommand : ControlCommandBase<GalleryItemHasFocusChangedCommandBehavior, GalleryItem>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryItemHasFocusChangedCommandBehavior : CommandBehaviorBase<GalleryItem>
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
            TargetObject.HasFocusChanged += OnEventRaised;
        }
    }

	// GalleryItemHasFocusChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryItemHasFocusChangedCommandBehavior<T> : GalleryItemHasFocusChangedCommandBehavior
    { }
	#endregion

	#region GalleryItemIsAlwaysShownCaptionChangedCommand
	// GalleryItemIsAlwaysShownCaptionChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryItemIsAlwaysShownCaptionChangedCommand : ControlCommandBase<GalleryItemIsAlwaysShownCaptionChangedCommandBehavior, GalleryItem>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryItemIsAlwaysShownCaptionChangedCommandBehavior : CommandBehaviorBase<GalleryItem>
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
            TargetObject.IsAlwaysShownCaptionChanged += OnEventRaised;
        }
    }

	// GalleryItemIsAlwaysShownCaptionChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryItemIsAlwaysShownCaptionChangedCommandBehavior<T> : GalleryItemIsAlwaysShownCaptionChangedCommandBehavior
    { }
	#endregion
}


