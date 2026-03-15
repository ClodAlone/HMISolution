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

	#region GalleryGroupCornerRadiusChangedCommand
	// GalleryGroupCornerRadiusChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryGroupCornerRadiusChangedCommand : ControlCommandBase<GalleryGroupCornerRadiusChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryGroupCornerRadiusChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.GroupCornerRadiusChanged += OnEventRaised;
        }
    }

	// GalleryGroupCornerRadiusChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryGroupCornerRadiusChangedCommandBehavior<T> : GalleryGroupCornerRadiusChangedCommandBehavior
    { }
	#endregion

	#region GalleryGroupBorderThicknessChangedCommand
	// GalleryGroupBorderThicknessChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryGroupBorderThicknessChangedCommand : ControlCommandBase<GalleryGroupBorderThicknessChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryGroupBorderThicknessChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.GroupBorderThicknessChanged += OnEventRaised;
        }
    }

	// GalleryGroupBorderThicknessChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryGroupBorderThicknessChangedCommandBehavior<T> : GalleryGroupBorderThicknessChangedCommandBehavior
    { }
	#endregion

	#region GalleryGroupBorderBrushChangedCommand
	// GalleryGroupBorderBrushChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryGroupBorderBrushChangedCommand : ControlCommandBase<GalleryGroupBorderBrushChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryGroupBorderBrushChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.GroupBorderBrushChanged += OnEventRaised;
        }
    }

	// GalleryGroupBorderBrushChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryGroupBorderBrushChangedCommandBehavior<T> : GalleryGroupBorderBrushChangedCommandBehavior
    { }
	#endregion

	#region GalleryGroupBackgroundChangedCommand
	// GalleryGroupBackgroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryGroupBackgroundChangedCommand : ControlCommandBase<GalleryGroupBackgroundChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryGroupBackgroundChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.GroupBackgroundChanged += OnEventRaised;
        }
    }

	// GalleryGroupBackgroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryGroupBackgroundChangedCommandBehavior<T> : GalleryGroupBackgroundChangedCommandBehavior
    { }
	#endregion

	#region GalleryGroupForegroundChangedCommand
	// GalleryGroupForegroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryGroupForegroundChangedCommand : ControlCommandBase<GalleryGroupForegroundChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryGroupForegroundChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.GroupForegroundChanged += OnEventRaised;
        }
    }

	// GalleryGroupForegroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryGroupForegroundChangedCommandBehavior<T> : GalleryGroupForegroundChangedCommandBehavior
    { }
	#endregion

	#region GalleryGroupMouseOverBackgroundChangedCommand
	// GalleryGroupMouseOverBackgroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryGroupMouseOverBackgroundChangedCommand : ControlCommandBase<GalleryGroupMouseOverBackgroundChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryGroupMouseOverBackgroundChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.GroupMouseOverBackgroundChanged += OnEventRaised;
        }
    }

	// GalleryGroupMouseOverBackgroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryGroupMouseOverBackgroundChangedCommandBehavior<T> : GalleryGroupMouseOverBackgroundChangedCommandBehavior
    { }
	#endregion

	#region GalleryGroupMouseOverForegroundChangedCommand
	// GalleryGroupMouseOverForegroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryGroupMouseOverForegroundChangedCommand : ControlCommandBase<GalleryGroupMouseOverForegroundChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryGroupMouseOverForegroundChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.GroupMouseOverForegroundChanged += OnEventRaised;
        }
    }

	// GalleryGroupMouseOverForegroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryGroupMouseOverForegroundChangedCommandBehavior<T> : GalleryGroupMouseOverForegroundChangedCommandBehavior
    { }
	#endregion

	#region GalleryGroupSelectedBackgroundChangedCommand
	// GalleryGroupSelectedBackgroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryGroupSelectedBackgroundChangedCommand : ControlCommandBase<GalleryGroupSelectedBackgroundChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryGroupSelectedBackgroundChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.GroupSelectedBackgroundChanged += OnEventRaised;
        }
    }

	// GalleryGroupSelectedBackgroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryGroupSelectedBackgroundChangedCommandBehavior<T> : GalleryGroupSelectedBackgroundChangedCommandBehavior
    { }
	#endregion

	#region GalleryGroupSelectedForegroundChangedCommand
	// GalleryGroupSelectedForegroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryGroupSelectedForegroundChangedCommand : ControlCommandBase<GalleryGroupSelectedForegroundChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryGroupSelectedForegroundChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.GroupSelectedForegroundChanged += OnEventRaised;
        }
    }

	// GalleryGroupSelectedForegroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryGroupSelectedForegroundChangedCommandBehavior<T> : GalleryGroupSelectedForegroundChangedCommandBehavior
    { }
	#endregion

	#region GalleryGroupPaddingChangedCommand
	// GalleryGroupPaddingChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryGroupPaddingChangedCommand : ControlCommandBase<GalleryGroupPaddingChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryGroupPaddingChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.GroupPaddingChanged += OnEventRaised;
        }
    }

	// GalleryGroupPaddingChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryGroupPaddingChangedCommandBehavior<T> : GalleryGroupPaddingChangedCommandBehavior
    { }
	#endregion

	#region GalleryGroupMarginChangedCommand
	// GalleryGroupMarginChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryGroupMarginChangedCommand : ControlCommandBase<GalleryGroupMarginChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryGroupMarginChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.GroupMarginChanged += OnEventRaised;
        }
    }

	// GalleryGroupMarginChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryGroupMarginChangedCommandBehavior<T> : GalleryGroupMarginChangedCommandBehavior
    { }
	#endregion

	#region GalleryGroupMouseOverBorderBrushChangedCommand
	// GalleryGroupMouseOverBorderBrushChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryGroupMouseOverBorderBrushChangedCommand : ControlCommandBase<GalleryGroupMouseOverBorderBrushChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryGroupMouseOverBorderBrushChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.GroupMouseOverBorderBrushChanged += OnEventRaised;
        }
    }

	// GalleryGroupMouseOverBorderBrushChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryGroupMouseOverBorderBrushChangedCommandBehavior<T> : GalleryGroupMouseOverBorderBrushChangedCommandBehavior
    { }
	#endregion

	#region GalleryGroupSelectedBorderBrushChangedCommand
	// GalleryGroupSelectedBorderBrushChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryGroupSelectedBorderBrushChangedCommand : ControlCommandBase<GalleryGroupSelectedBorderBrushChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryGroupSelectedBorderBrushChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.GroupSelectedBorderBrushChanged += OnEventRaised;
        }
    }

	// GalleryGroupSelectedBorderBrushChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryGroupSelectedBorderBrushChangedCommandBehavior<T> : GalleryGroupSelectedBorderBrushChangedCommandBehavior
    { }
	#endregion

	#region GalleryPanelCornerRadiusChangedCommand
	// GalleryPanelCornerRadiusChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryPanelCornerRadiusChangedCommand : ControlCommandBase<GalleryPanelCornerRadiusChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryPanelCornerRadiusChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.PanelCornerRadiusChanged += OnEventRaised;
        }
    }

	// GalleryPanelCornerRadiusChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryPanelCornerRadiusChangedCommandBehavior<T> : GalleryPanelCornerRadiusChangedCommandBehavior
    { }
	#endregion

	#region GalleryPanelBorderThicknessChangedCommand
	// GalleryPanelBorderThicknessChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryPanelBorderThicknessChangedCommand : ControlCommandBase<GalleryPanelBorderThicknessChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryPanelBorderThicknessChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.PanelBorderThicknessChanged += OnEventRaised;
        }
    }

	// GalleryPanelBorderThicknessChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryPanelBorderThicknessChangedCommandBehavior<T> : GalleryPanelBorderThicknessChangedCommandBehavior
    { }
	#endregion

	#region GalleryPanelBorderBrushChangedCommand
	// GalleryPanelBorderBrushChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryPanelBorderBrushChangedCommand : ControlCommandBase<GalleryPanelBorderBrushChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryPanelBorderBrushChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.PanelBorderBrushChanged += OnEventRaised;
        }
    }

	// GalleryPanelBorderBrushChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryPanelBorderBrushChangedCommandBehavior<T> : GalleryPanelBorderBrushChangedCommandBehavior
    { }
	#endregion

	#region GalleryPanelBackgroundChangedCommand
	// GalleryPanelBackgroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryPanelBackgroundChangedCommand : ControlCommandBase<GalleryPanelBackgroundChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryPanelBackgroundChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.PanelBackgroundChanged += OnEventRaised;
        }
    }

	// GalleryPanelBackgroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryPanelBackgroundChangedCommandBehavior<T> : GalleryPanelBackgroundChangedCommandBehavior
    { }
	#endregion

	#region GalleryPanelMouseOverBackgroundChangedCommand
	// GalleryPanelMouseOverBackgroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryPanelMouseOverBackgroundChangedCommand : ControlCommandBase<GalleryPanelMouseOverBackgroundChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryPanelMouseOverBackgroundChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.PanelMouseOverBackgroundChanged += OnEventRaised;
        }
    }

	// GalleryPanelMouseOverBackgroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryPanelMouseOverBackgroundChangedCommandBehavior<T> : GalleryPanelMouseOverBackgroundChangedCommandBehavior
    { }
	#endregion

	#region GalleryPanelSelectedBackgroundChangedCommand
	// GalleryPanelSelectedBackgroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryPanelSelectedBackgroundChangedCommand : ControlCommandBase<GalleryPanelSelectedBackgroundChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryPanelSelectedBackgroundChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.PanelSelectedBackgroundChanged += OnEventRaised;
        }
    }

	// GalleryPanelSelectedBackgroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryPanelSelectedBackgroundChangedCommandBehavior<T> : GalleryPanelSelectedBackgroundChangedCommandBehavior
    { }
	#endregion

	#region GalleryPanelPaddingChangedCommand
	// GalleryPanelPaddingChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryPanelPaddingChangedCommand : ControlCommandBase<GalleryPanelPaddingChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryPanelPaddingChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.PanelPaddingChanged += OnEventRaised;
        }
    }

	// GalleryPanelPaddingChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryPanelPaddingChangedCommandBehavior<T> : GalleryPanelPaddingChangedCommandBehavior
    { }
	#endregion

	#region GalleryPanelMarginChangedCommand
	// GalleryPanelMarginChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryPanelMarginChangedCommand : ControlCommandBase<GalleryPanelMarginChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryPanelMarginChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.PanelMarginChanged += OnEventRaised;
        }
    }

	// GalleryPanelMarginChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryPanelMarginChangedCommandBehavior<T> : GalleryPanelMarginChangedCommandBehavior
    { }
	#endregion

	#region GalleryPanelMouseOverBorderBrushChangedCommand
	// GalleryPanelMouseOverBorderBrushChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryPanelMouseOverBorderBrushChangedCommand : ControlCommandBase<GalleryPanelMouseOverBorderBrushChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryPanelMouseOverBorderBrushChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.PanelMouseOverBorderBrushChanged += OnEventRaised;
        }
    }

	// GalleryPanelMouseOverBorderBrushChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryPanelMouseOverBorderBrushChangedCommandBehavior<T> : GalleryPanelMouseOverBorderBrushChangedCommandBehavior
    { }
	#endregion

	#region GalleryPanelSelectedBorderBrushChangedCommand
	// GalleryPanelSelectedBorderBrushChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryPanelSelectedBorderBrushChangedCommand : ControlCommandBase<GalleryPanelSelectedBorderBrushChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryPanelSelectedBorderBrushChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.PanelSelectedBorderBrushChanged += OnEventRaised;
        }
    }

	// GalleryPanelSelectedBorderBrushChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryPanelSelectedBorderBrushChangedCommandBehavior<T> : GalleryPanelSelectedBorderBrushChangedCommandBehavior
    { }
	#endregion

	#region GalleryItemContentTemplateChangedCommand
	// GalleryItemContentTemplateChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryItemContentTemplateChangedCommand : ControlCommandBase<GalleryItemContentTemplateChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryItemContentTemplateChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.ItemContentTemplateChanged += OnEventRaised;
        }
    }

	// GalleryItemContentTemplateChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryItemContentTemplateChangedCommandBehavior<T> : GalleryItemContentTemplateChangedCommandBehavior
    { }
	#endregion

	#region GalleryItemContentTemplateSelectorChangedCommand
	// GalleryItemContentTemplateSelectorChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryItemContentTemplateSelectorChangedCommand : ControlCommandBase<GalleryItemContentTemplateSelectorChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryItemContentTemplateSelectorChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.ItemContentTemplateSelectorChanged += OnEventRaised;
        }
    }

	// GalleryItemContentTemplateSelectorChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryItemContentTemplateSelectorChangedCommandBehavior<T> : GalleryItemContentTemplateSelectorChangedCommandBehavior
    { }
	#endregion

	#region GalleryItemMarginChangedCommand
	// GalleryItemMarginChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryItemMarginChangedCommand : ControlCommandBase<GalleryItemMarginChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryItemMarginChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.ItemMarginChanged += OnEventRaised;
        }
    }

	// GalleryItemMarginChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryItemMarginChangedCommandBehavior<T> : GalleryItemMarginChangedCommandBehavior
    { }
	#endregion

	#region GalleryItemWidthChangedCommand
	// GalleryItemWidthChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryControlItemWidthChangedCommand : ControlCommandBase<GalleryControlItemWidthChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryControlItemWidthChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.ItemWidthChanged += OnEventRaised;
        }
    }

	// GalleryItemWidthChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryControlItemWidthChangedCommandBehavior<T> : GalleryControlItemWidthChangedCommandBehavior
    { }
	#endregion

	#region GalleryItemHeightChangedCommand
	// GalleryItemHeightChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryControlItemHeightChangedCommand : ControlCommandBase<GalleryControlItemHeightChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryControlItemHeightChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.ItemHeightChanged += OnEventRaised;
        }
    }

	// GalleryItemHeightChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryControlItemHeightChangedCommandBehavior<T> : GalleryControlItemHeightChangedCommandBehavior
    { }
	#endregion

	#region GalleryItemMinWidthChangedCommand
	// GalleryItemMinWidthChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryItemMinWidthChangedCommand : ControlCommandBase<GalleryItemMinWidthChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryItemMinWidthChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.ItemMinWidthChanged += OnEventRaised;
        }
    }

	// GalleryItemMinWidthChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryItemMinWidthChangedCommandBehavior<T> : GalleryItemMinWidthChangedCommandBehavior
    { }
	#endregion

	#region GalleryItemMaxWidthChangedCommand
	// GalleryItemMaxWidthChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryItemMaxWidthChangedCommand : ControlCommandBase<GalleryItemMaxWidthChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryItemMaxWidthChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.ItemMaxWidthChanged += OnEventRaised;
        }
    }

	// GalleryItemMaxWidthChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryItemMaxWidthChangedCommandBehavior<T> : GalleryItemMaxWidthChangedCommandBehavior
    { }
	#endregion

	#region GalleryItemMinHeightChangedCommand
	// GalleryItemMinHeightChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryItemMinHeightChangedCommand : ControlCommandBase<GalleryItemMinHeightChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryItemMinHeightChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.ItemMinHeightChanged += OnEventRaised;
        }
    }

	// GalleryItemMinHeightChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryItemMinHeightChangedCommandBehavior<T> : GalleryItemMinHeightChangedCommandBehavior
    { }
	#endregion

	#region GalleryItemMaxHeightChangedCommand
	// GalleryItemMaxHeightChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryItemMaxHeightChangedCommand : ControlCommandBase<GalleryItemMaxHeightChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryItemMaxHeightChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.ItemMaxHeightChanged += OnEventRaised;
        }
    }

	// GalleryItemMaxHeightChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryItemMaxHeightChangedCommandBehavior<T> : GalleryItemMaxHeightChangedCommandBehavior
    { }
	#endregion

	#region GalleryItemCornerRadiusChangedCommand
	// GalleryItemCornerRadiusChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryItemCornerRadiusChangedCommand : ControlCommandBase<GalleryItemCornerRadiusChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryItemCornerRadiusChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.ItemCornerRadiusChanged += OnEventRaised;
        }
    }

	// GalleryItemCornerRadiusChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryItemCornerRadiusChangedCommandBehavior<T> : GalleryItemCornerRadiusChangedCommandBehavior
    { }
	#endregion

	#region GalleryItemBorderThicknessChangedCommand
	// GalleryItemBorderThicknessChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryItemBorderThicknessChangedCommand : ControlCommandBase<GalleryItemBorderThicknessChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryItemBorderThicknessChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.ItemBorderThicknessChanged += OnEventRaised;
        }
    }

	// GalleryItemBorderThicknessChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryItemBorderThicknessChangedCommandBehavior<T> : GalleryItemBorderThicknessChangedCommandBehavior
    { }
	#endregion

	#region GalleryItemBorderBrushChangedCommand
	// GalleryItemBorderBrushChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryItemBorderBrushChangedCommand : ControlCommandBase<GalleryItemBorderBrushChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryItemBorderBrushChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.ItemBorderBrushChanged += OnEventRaised;
        }
    }

	// GalleryItemBorderBrushChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryItemBorderBrushChangedCommandBehavior<T> : GalleryItemBorderBrushChangedCommandBehavior
    { }
	#endregion

	#region GalleryItemBackgroundChangedCommand
	// GalleryItemBackgroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryItemBackgroundChangedCommand : ControlCommandBase<GalleryItemBackgroundChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryItemBackgroundChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.ItemBackgroundChanged += OnEventRaised;
        }
    }

	// GalleryItemBackgroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryItemBackgroundChangedCommandBehavior<T> : GalleryItemBackgroundChangedCommandBehavior
    { }
	#endregion

	#region GalleryItemForegroundChangedCommand
	// GalleryItemForegroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryItemForegroundChangedCommand : ControlCommandBase<GalleryItemForegroundChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryItemForegroundChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.ItemForegroundChanged += OnEventRaised;
        }
    }

	// GalleryItemForegroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryItemForegroundChangedCommandBehavior<T> : GalleryItemForegroundChangedCommandBehavior
    { }
	#endregion

	#region GalleryItemMouseOverBackgroundChangedCommand
	// GalleryItemMouseOverBackgroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryItemMouseOverBackgroundChangedCommand : ControlCommandBase<GalleryItemMouseOverBackgroundChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryItemMouseOverBackgroundChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.ItemMouseOverBackgroundChanged += OnEventRaised;
        }
    }

	// GalleryItemMouseOverBackgroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryItemMouseOverBackgroundChangedCommandBehavior<T> : GalleryItemMouseOverBackgroundChangedCommandBehavior
    { }
	#endregion

	#region GalleryItemMouseOverForegroundChangedCommand
	// GalleryItemMouseOverForegroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryItemMouseOverForegroundChangedCommand : ControlCommandBase<GalleryItemMouseOverForegroundChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryItemMouseOverForegroundChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.ItemMouseOverForegroundChanged += OnEventRaised;
        }
    }

	// GalleryItemMouseOverForegroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryItemMouseOverForegroundChangedCommandBehavior<T> : GalleryItemMouseOverForegroundChangedCommandBehavior
    { }
	#endregion

	#region GalleryItemSelectedBackgroundChangedCommand
	// GalleryItemSelectedBackgroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryItemSelectedBackgroundChangedCommand : ControlCommandBase<GalleryItemSelectedBackgroundChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryItemSelectedBackgroundChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.ItemSelectedBackgroundChanged += OnEventRaised;
        }
    }

	// GalleryItemSelectedBackgroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryItemSelectedBackgroundChangedCommandBehavior<T> : GalleryItemSelectedBackgroundChangedCommandBehavior
    { }
	#endregion

	#region GalleryItemSelectedForegroundChangedCommand
	// GalleryItemSelectedForegroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryItemSelectedForegroundChangedCommand : ControlCommandBase<GalleryItemSelectedForegroundChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryItemSelectedForegroundChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.ItemSelectedForegroundChanged += OnEventRaised;
        }
    }

	// GalleryItemSelectedForegroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryItemSelectedForegroundChangedCommandBehavior<T> : GalleryItemSelectedForegroundChangedCommandBehavior
    { }
	#endregion

	#region GalleryItemPaddingChangedCommand
	// GalleryItemPaddingChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryItemPaddingChangedCommand : ControlCommandBase<GalleryItemPaddingChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryItemPaddingChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.ItemPaddingChanged += OnEventRaised;
        }
    }

	// GalleryItemPaddingChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryItemPaddingChangedCommandBehavior<T> : GalleryItemPaddingChangedCommandBehavior
    { }
	#endregion

	#region GalleryItemMouseOverBorderBrushChangedCommand
	// GalleryItemMouseOverBorderBrushChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryItemMouseOverBorderBrushChangedCommand : ControlCommandBase<GalleryItemMouseOverBorderBrushChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryItemMouseOverBorderBrushChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.ItemMouseOverBorderBrushChanged += OnEventRaised;
        }
    }

	// GalleryItemMouseOverBorderBrushChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryItemMouseOverBorderBrushChangedCommandBehavior<T> : GalleryItemMouseOverBorderBrushChangedCommandBehavior
    { }
	#endregion

	#region GalleryItemSelectedBorderBrushChangedCommand
	// GalleryItemSelectedBorderBrushChangedCommand
    /// <summary>
    /// /
    /// </summary>
	public class GalleryItemSelectedBorderBrushChangedCommand : ControlCommandBase<GalleryItemSelectedBorderBrushChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryItemSelectedBorderBrushChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.ItemSelectedBorderBrushChanged += OnEventRaised;
        }
    }

	// GalleryItemSelectedBorderBrushChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryItemSelectedBorderBrushChangedCommandBehavior<T> : GalleryItemSelectedBorderBrushChangedCommandBehavior
    { }
	#endregion

	#region GalleryFocusedItemBorderBrushChangedCommand
	// GalleryFocusedItemBorderBrushChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryFocusedItemBorderBrushChangedCommand : ControlCommandBase<GalleryFocusedItemBorderBrushChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryFocusedItemBorderBrushChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.FocusedItemBorderBrushChanged += OnEventRaised;
        }
    }

	// GalleryFocusedItemBorderBrushChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryFocusedItemBorderBrushChangedCommandBehavior<T> : GalleryFocusedItemBorderBrushChangedCommandBehavior
    { }
	#endregion

	#region GalleryFocusedItemBackgroundChangedCommand
	// GalleryFocusedItemBackgroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryFocusedItemBackgroundChangedCommand : ControlCommandBase<GalleryFocusedItemBackgroundChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryFocusedItemBackgroundChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.FocusedItemBackgroundChanged += OnEventRaised;
        }
    }

	// GalleryFocusedItemBackgroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryFocusedItemBackgroundChangedCommandBehavior<T> : GalleryFocusedItemBackgroundChangedCommandBehavior
    { }
	#endregion

	#region GalleryFocusedItemForegroundChangedCommand
	// GalleryFocusedItemForegroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryFocusedItemForegroundChangedCommand : ControlCommandBase<GalleryFocusedItemForegroundChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryFocusedItemForegroundChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.FocusedItemForegroundChanged += OnEventRaised;
        }
    }

	// GalleryFocusedItemForegroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryFocusedItemForegroundChangedCommandBehavior<T> : GalleryFocusedItemForegroundChangedCommandBehavior
    { }
	#endregion

	#region GalleryFocusedItemMouseOverBorderBrushChangedCommand
	// GalleryFocusedItemMouseOverBorderBrushChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryFocusedItemMouseOverBorderBrushChangedCommand : ControlCommandBase<GalleryFocusedItemMouseOverBorderBrushChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryFocusedItemMouseOverBorderBrushChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.FocusedItemMouseOverBorderBrushChanged += OnEventRaised;
        }
    }

	// GalleryFocusedItemMouseOverBorderBrushChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryFocusedItemMouseOverBorderBrushChangedCommandBehavior<T> : GalleryFocusedItemMouseOverBorderBrushChangedCommandBehavior
    { }
	#endregion

	#region GalleryFocusedItemMouseOverBackgroundChangedCommand
	// GalleryFocusedItemMouseOverBackgroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryFocusedItemMouseOverBackgroundChangedCommand : ControlCommandBase<GalleryFocusedItemMouseOverBackgroundChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryFocusedItemMouseOverBackgroundChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.FocusedItemMouseOverBackgroundChanged += OnEventRaised;
        }
    }

	// GalleryFocusedItemMouseOverBackgroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryFocusedItemMouseOverBackgroundChangedCommandBehavior<T> : GalleryFocusedItemMouseOverBackgroundChangedCommandBehavior
    { }
	#endregion

	#region GalleryFocusedItemMouseOverForegroundChangedCommand
	// GalleryFocusedItemMouseOverForegroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryFocusedItemMouseOverForegroundChangedCommand : ControlCommandBase<GalleryFocusedItemMouseOverForegroundChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryFocusedItemMouseOverForegroundChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.FocusedItemMouseOverForegroundChanged += OnEventRaised;
        }
    }

	// GalleryFocusedItemMouseOverForegroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryFocusedItemMouseOverForegroundChangedCommandBehavior<T> : GalleryFocusedItemMouseOverForegroundChangedCommandBehavior
    { }
	#endregion

	#region GallerySelectedItemBorderThicknessChangedCommand
	// GallerySelectedItemBorderThicknessChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GallerySelectedItemBorderThicknessChangedCommand : ControlCommandBase<GallerySelectedItemBorderThicknessChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GallerySelectedItemBorderThicknessChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.SelectedItemBorderThicknessChanged += OnEventRaised;
        }
    }

	// GallerySelectedItemBorderThicknessChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GallerySelectedItemBorderThicknessChangedCommandBehavior<T> : GallerySelectedItemBorderThicknessChangedCommandBehavior
    { }
	#endregion

	#region GalleryFocusedItemBorderThicknessChangedCommand
	// GalleryFocusedItemBorderThicknessChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryFocusedItemBorderThicknessChangedCommand : ControlCommandBase<GalleryFocusedItemBorderThicknessChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryFocusedItemBorderThicknessChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.FocusedItemBorderThicknessChanged += OnEventRaised;
        }
    }

	// GalleryFocusedItemBorderThicknessChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryFocusedItemBorderThicknessChangedCommandBehavior<T> : GalleryFocusedItemBorderThicknessChangedCommandBehavior
    { }
	#endregion

	#region GalleryAllowedAnimationsChangedCommand
	// GalleryAllowedAnimationsChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryAllowedAnimationsChangedCommand : ControlCommandBase<GalleryAllowedAnimationsChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryAllowedAnimationsChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.AllowedAnimationsChanged += OnEventRaised;
        }
    }

	// GalleryAllowedAnimationsChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryAllowedAnimationsChangedCommandBehavior<T> : GalleryAllowedAnimationsChangedCommandBehavior
    { }
	#endregion

	#region GalleryAllowMultiSelectChangedCommand
	// GalleryAllowMultiSelectChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryAllowMultiSelectChangedCommand : ControlCommandBase<GalleryAllowMultiSelectChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryAllowMultiSelectChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.AllowMultiSelectChanged += OnEventRaised;
        }
    }

	// GalleryAllowMultiSelectChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryAllowMultiSelectChangedCommandBehavior<T> : GalleryAllowMultiSelectChangedCommandBehavior
    { }
	#endregion

	#region GalleryAllowVaryingItemSizeChangedCommand
	// GalleryAllowVaryingItemSizeChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryAllowVaryingItemSizeChangedCommand : ControlCommandBase<GalleryAllowVaryingItemSizeChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryAllowVaryingItemSizeChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.AllowVaryingItemSizeChanged += OnEventRaised;
        }
    }

	// GalleryAllowVaryingItemSizeChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryAllowVaryingItemSizeChangedCommandBehavior<T> : GalleryAllowVaryingItemSizeChangedCommandBehavior
    { }
	#endregion

	#region GalleryAllowedItemResizeModeChangedCommand
	// GalleryAllowedItemResizeModeChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryAllowedItemResizeModeChangedCommand : ControlCommandBase<GalleryAllowedItemResizeModeChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryAllowedItemResizeModeChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.AllowedItemResizeModeChanged += OnEventRaised;
        }
    }

	// GalleryAllowedItemResizeModeChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryAllowedItemResizeModeChangedCommandBehavior<T> : GalleryAllowedItemResizeModeChangedCommandBehavior
    { }
	#endregion

	#region GallerySpaceLimitBetweenItemsChangedCommand
	// GallerySpaceLimitBetweenItemsChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GallerySpaceLimitBetweenItemsChangedCommand : ControlCommandBase<GallerySpaceLimitBetweenItemsChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GallerySpaceLimitBetweenItemsChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.SpaceLimitBetweenItemsChanged += OnEventRaised;
        }
    }

	// GallerySpaceLimitBetweenItemsChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GallerySpaceLimitBetweenItemsChangedCommandBehavior<T> : GallerySpaceLimitBetweenItemsChangedCommandBehavior
    { }
	#endregion

	#region GalleryVisualModeChangedCommand
	// GalleryVisualModeChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryVisualModeChangedCommand : ControlCommandBase<GalleryVisualModeChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryVisualModeChangedCommandBehavior : CommandBehaviorBase<Gallery>
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

	// GalleryVisualModeChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryVisualModeChangedCommandBehavior<T> : GalleryVisualModeChangedCommandBehavior
    { }
	#endregion

	#region GalleryGalleryCornerRadiusChangedCommand
	// GalleryGalleryCornerRadiusChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryGalleryCornerRadiusChangedCommand : ControlCommandBase<GalleryGalleryCornerRadiusChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryGalleryCornerRadiusChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.GalleryCornerRadiusChanged += OnEventRaised;
        }
    }

	// GalleryGalleryCornerRadiusChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryGalleryCornerRadiusChangedCommandBehavior<T> : GalleryGalleryCornerRadiusChangedCommandBehavior
    { }
	#endregion

	#region GalleryBorderThicknessChangedCommand
	// GalleryBorderThicknessChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryBorderThicknessChangedCommand : ControlCommandBase<GalleryBorderThicknessChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryBorderThicknessChangedCommandBehavior : CommandBehaviorBase<Gallery>
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

	// GalleryBorderThicknessChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryBorderThicknessChangedCommandBehavior<T> : GalleryBorderThicknessChangedCommandBehavior
    { }
	#endregion

	#region GalleryBorderBrushChangedCommand
	// GalleryBorderBrushChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryBorderBrushChangedCommand : ControlCommandBase<GalleryBorderBrushChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryBorderBrushChangedCommandBehavior : CommandBehaviorBase<Gallery>
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

	// GalleryBorderBrushChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryBorderBrushChangedCommandBehavior<T> : GalleryBorderBrushChangedCommandBehavior
    { }
	#endregion

	#region GalleryBackgroundChangedCommand
	// GalleryBackgroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryBackgroundChangedCommand : ControlCommandBase<GalleryBackgroundChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryBackgroundChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.BackgroundChanged += OnEventRaised;
        }
    }

	// GalleryBackgroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryBackgroundChangedCommandBehavior<T> : GalleryBackgroundChangedCommandBehavior
    { }
	#endregion

	#region GalleryPaddingChangedCommand
	// GalleryPaddingChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryPaddingChangedCommand : ControlCommandBase<GalleryPaddingChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryPaddingChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.PaddingChanged += OnEventRaised;
        }
    }

	// GalleryPaddingChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryPaddingChangedCommandBehavior<T> : GalleryPaddingChangedCommandBehavior
    { }
	#endregion

	#region GalleryMarginChangedCommand
	// GalleryMarginChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryMarginChangedCommand : ControlCommandBase<GalleryMarginChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryMarginChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.MarginChanged += OnEventRaised;
        }
    }

	// GalleryMarginChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryMarginChangedCommandBehavior<T> : GalleryMarginChangedCommandBehavior
    { }
	#endregion

	#region GalleryIsHeaderVisibleChangedCommand
	// GalleryIsHeaderVisibleChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryIsHeaderVisibleChangedCommand : ControlCommandBase<GalleryIsHeaderVisibleChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryIsHeaderVisibleChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.IsHeaderVisibleChanged += OnEventRaised;
        }
    }

	// GalleryIsHeaderVisibleChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryIsHeaderVisibleChangedCommandBehavior<T> : GalleryIsHeaderVisibleChangedCommandBehavior
    { }
	#endregion

	#region GalleryCanDragDropChangedCommand
	// GalleryCanDragDropChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryCanDragDropChangedCommand : ControlCommandBase<GalleryCanDragDropChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryCanDragDropChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.CanDragDropChanged += OnEventRaised;
        }
    }

	// GalleryCanDragDropChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryCanDragDropChangedCommandBehavior<T> : GalleryCanDragDropChangedCommandBehavior
    { }
	#endregion

	#region GalleryCaptionAlignmentChangedCommand
	// GalleryCaptionAlignmentChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryCaptionAlignmentChangedCommand : ControlCommandBase<GalleryCaptionAlignmentChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryCaptionAlignmentChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.CaptionAlignmentChanged += OnEventRaised;
        }
    }

	// GalleryCaptionAlignmentChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryCaptionAlignmentChangedCommandBehavior<T> : GalleryCaptionAlignmentChangedCommandBehavior
    { }
	#endregion

	#region GalleryDescriptionAlignmentChangedCommand
	// GalleryDescriptionAlignmentChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryDescriptionAlignmentChangedCommand : ControlCommandBase<GalleryDescriptionAlignmentChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryDescriptionAlignmentChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.DescriptionAlignmentChanged += OnEventRaised;
        }
    }

	// GalleryDescriptionAlignmentChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryDescriptionAlignmentChangedCommandBehavior<T> : GalleryDescriptionAlignmentChangedCommandBehavior
    { }
	#endregion

	#region GalleryCaptionBorderBrushChangedCommand
	// GalleryCaptionBorderBrushChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryCaptionBorderBrushChangedCommand : ControlCommandBase<GalleryCaptionBorderBrushChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryCaptionBorderBrushChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.CaptionBorderBrushChanged += OnEventRaised;
        }
    }

	// GalleryCaptionBorderBrushChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryCaptionBorderBrushChangedCommandBehavior<T> : GalleryCaptionBorderBrushChangedCommandBehavior
    { }
	#endregion

	#region GalleryCaptionBorderThicknessChangedCommand
	// GalleryCaptionBorderThicknessChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryCaptionBorderThicknessChangedCommand : ControlCommandBase<GalleryCaptionBorderThicknessChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryCaptionBorderThicknessChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.CaptionBorderThicknessChanged += OnEventRaised;
        }
    }

	// GalleryCaptionBorderThicknessChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryCaptionBorderThicknessChangedCommandBehavior<T> : GalleryCaptionBorderThicknessChangedCommandBehavior
    { }
	#endregion

	#region GalleryCaptionBackgroundChangedCommand
	// GalleryCaptionBackgroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryCaptionBackgroundChangedCommand : ControlCommandBase<GalleryCaptionBackgroundChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryCaptionBackgroundChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.CaptionBackgroundChanged += OnEventRaised;
        }
    }

	// GalleryCaptionBackgroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryCaptionBackgroundChangedCommandBehavior<T> : GalleryCaptionBackgroundChangedCommandBehavior
    { }
	#endregion

	#region GalleryCaptionCornerRadiusChangedCommand
	// GalleryCaptionCornerRadiusChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryCaptionCornerRadiusChangedCommand : ControlCommandBase<GalleryCaptionCornerRadiusChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryCaptionCornerRadiusChangedCommandBehavior : CommandBehaviorBase<Gallery>
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
            TargetObject.CaptionCornerRadiusChanged += OnEventRaised;
        }
    }

	// GalleryCaptionCornerRadiusChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryCaptionCornerRadiusChangedCommandBehavior<T> : GalleryCaptionCornerRadiusChangedCommandBehavior
    { }
	#endregion

	#region GalleryIsAlwaysShownCaptionChangedCommand
	// GalleryIsAlwaysShownCaptionChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryIsAlwaysShownCaptionChangedCommand : ControlCommandBase<GalleryIsAlwaysShownCaptionChangedCommandBehavior, Gallery>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryIsAlwaysShownCaptionChangedCommandBehavior : CommandBehaviorBase<Gallery>
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

	// GalleryIsAlwaysShownCaptionChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryIsAlwaysShownCaptionChangedCommandBehavior<T> : GalleryIsAlwaysShownCaptionChangedCommandBehavior
    { }
	#endregion
}


