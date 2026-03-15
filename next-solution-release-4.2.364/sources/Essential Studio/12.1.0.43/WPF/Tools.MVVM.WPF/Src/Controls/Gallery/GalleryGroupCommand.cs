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

	#region GalleryGroupItemGeneratedCommand
	// GalleryGroupItemGeneratedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryGroupItemGeneratedCommand : ControlCommandBase<GalleryGroupItemGeneratedCommandBehavior, GalleryGroup>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryGroupItemGeneratedCommandBehavior : CommandBehaviorBase<GalleryGroup>
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
            TargetObject.ItemGenerated += OnEventRaised;
        }
    }

	// GalleryGroupItemGeneratedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryGroupItemGeneratedCommandBehavior<T> : GalleryGroupItemGeneratedCommandBehavior
    { }
	#endregion

	#region GalleryGroupGalleryItemMouseLeftButtonDownCommand
	// GalleryGroupGalleryItemMouseLeftButtonDownCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryGroupGalleryItemMouseLeftButtonDownCommand : ControlCommandBase<GalleryGroupGalleryItemMouseLeftButtonDownCommandBehavior, GalleryGroup>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryGroupGalleryItemMouseLeftButtonDownCommandBehavior : CommandBehaviorBase<GalleryGroup>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, MouseButtonEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.GalleryItemMouseLeftButtonDown += OnEventRaised;
        }
    }

	// GalleryGroupGalleryItemMouseLeftButtonDownCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryGroupGalleryItemMouseLeftButtonDownCommandBehavior<T> : GalleryGroupGalleryItemMouseLeftButtonDownCommandBehavior
    { }
	#endregion

	#region GalleryGroupAllowedItemResizeModeChangedCommand
	// GalleryGroupAllowedItemResizeModeChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryGroupAllowedItemResizeModeChangedCommand : ControlCommandBase<GalleryGroupAllowedItemResizeModeChangedCommandBehavior, GalleryGroup>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryGroupAllowedItemResizeModeChangedCommandBehavior : CommandBehaviorBase<GalleryGroup>
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

	// GalleryGroupAllowedItemResizeModeChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryGroupAllowedItemResizeModeChangedCommandBehavior<T> : GalleryGroupAllowedItemResizeModeChangedCommandBehavior
    { }
	#endregion

	#region GalleryGroupAllowMultiSelectChangedCommand
	// GalleryGroupAllowMultiSelectChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryGroupAllowMultiSelectChangedCommand : ControlCommandBase<GalleryGroupAllowMultiSelectChangedCommandBehavior, GalleryGroup>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryGroupAllowMultiSelectChangedCommandBehavior : CommandBehaviorBase<GalleryGroup>
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

	// GalleryGroupAllowMultiSelectChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryGroupAllowMultiSelectChangedCommandBehavior<T> : GalleryGroupAllowMultiSelectChangedCommandBehavior
    { }
	#endregion

	#region GalleryGroupItemContentTemplateChangedCommand
	// GalleryGroupItemContentTemplateChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryGroupItemContentTemplateChangedCommand : ControlCommandBase<GalleryGroupItemContentTemplateChangedCommandBehavior, GalleryGroup>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryGroupItemContentTemplateChangedCommandBehavior : CommandBehaviorBase<GalleryGroup>
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

	// GalleryGroupItemContentTemplateChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryGroupItemContentTemplateChangedCommandBehavior<T> : GalleryGroupItemContentTemplateChangedCommandBehavior
    { }
	#endregion

	#region GalleryGroupItemContentTemplateSelectorChangedCommand
	// GalleryGroupItemContentTemplateSelectorChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryGroupItemContentTemplateSelectorChangedCommand : ControlCommandBase<GalleryGroupItemContentTemplateSelectorChangedCommandBehavior, GalleryGroup>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryGroupItemContentTemplateSelectorChangedCommandBehavior : CommandBehaviorBase<GalleryGroup>
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

	// GalleryGroupItemContentTemplateSelectorChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryGroupItemContentTemplateSelectorChangedCommandBehavior<T> : GalleryGroupItemContentTemplateSelectorChangedCommandBehavior
    { }
	#endregion

	#region GalleryGroupItemMarginChangedCommand
	// GalleryGroupItemMarginChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryGroupItemMarginChangedCommand : ControlCommandBase<GalleryGroupItemMarginChangedCommandBehavior, GalleryGroup>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryGroupItemMarginChangedCommandBehavior : CommandBehaviorBase<GalleryGroup>
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

	// GalleryGroupItemMarginChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryGroupItemMarginChangedCommandBehavior<T> : GalleryGroupItemMarginChangedCommandBehavior
    { }
	#endregion

	#region GalleryGroupItemWidthChangedCommand
	// GalleryGroupItemWidthChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryGroupItemWidthChangedCommand : ControlCommandBase<GalleryGroupItemWidthChangedCommandBehavior, GalleryGroup>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryGroupItemWidthChangedCommandBehavior : CommandBehaviorBase<GalleryGroup>
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

	// GalleryGroupItemWidthChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryGroupItemWidthChangedCommandBehavior<T> : GalleryGroupItemWidthChangedCommandBehavior
    { }
	#endregion

	#region GalleryGroupItemHeightChangedCommand
	// GalleryGroupItemHeightChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryGroupItemHeightChangedCommand : ControlCommandBase<GalleryGroupItemHeightChangedCommandBehavior, GalleryGroup>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryGroupItemHeightChangedCommandBehavior : CommandBehaviorBase<GalleryGroup>
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

	// GalleryGroupItemHeightChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryGroupItemHeightChangedCommandBehavior<T> : GalleryGroupItemHeightChangedCommandBehavior
    { }
	#endregion

	#region GalleryGroupItemMinWidthChangedCommand
	// GalleryGroupItemMinWidthChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryGroupItemMinWidthChangedCommand : ControlCommandBase<GalleryGroupItemMinWidthChangedCommandBehavior, GalleryGroup>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryGroupItemMinWidthChangedCommandBehavior : CommandBehaviorBase<GalleryGroup>
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

	// GalleryGroupItemMinWidthChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryGroupItemMinWidthChangedCommandBehavior<T> : GalleryGroupItemMinWidthChangedCommandBehavior
    { }
	#endregion

	#region GalleryGroupItemMaxWidthChangedCommand
	// GalleryGroupItemMaxWidthChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryGroupItemMaxWidthChangedCommand : ControlCommandBase<GalleryGroupItemMaxWidthChangedCommandBehavior, GalleryGroup>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryGroupItemMaxWidthChangedCommandBehavior : CommandBehaviorBase<GalleryGroup>
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

	// GalleryGroupItemMaxWidthChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryGroupItemMaxWidthChangedCommandBehavior<T> : GalleryGroupItemMaxWidthChangedCommandBehavior
    { }
	#endregion

	#region GalleryGroupItemMinHeightChangedCommand
	// GalleryGroupItemMinHeightChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryGroupItemMinHeightChangedCommand : ControlCommandBase<GalleryGroupItemMinHeightChangedCommandBehavior, GalleryGroup>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryGroupItemMinHeightChangedCommandBehavior : CommandBehaviorBase<GalleryGroup>
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

	// GalleryGroupItemMinHeightChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryGroupItemMinHeightChangedCommandBehavior<T> : GalleryGroupItemMinHeightChangedCommandBehavior
    { }
	#endregion

	#region GalleryGroupItemMaxHeightChangedCommand
	// GalleryGroupItemMaxHeightChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryGroupItemMaxHeightChangedCommand : ControlCommandBase<GalleryGroupItemMaxHeightChangedCommandBehavior, GalleryGroup>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryGroupItemMaxHeightChangedCommandBehavior : CommandBehaviorBase<GalleryGroup>
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

	// GalleryGroupItemMaxHeightChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryGroupItemMaxHeightChangedCommandBehavior<T> : GalleryGroupItemMaxHeightChangedCommandBehavior
    { }
	#endregion

	#region GalleryGroupAllowVaryingItemSizeChangedCommand
	// GalleryGroupAllowVaryingItemSizeChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryGroupAllowVaryingItemSizeChangedCommand : ControlCommandBase<GalleryGroupAllowVaryingItemSizeChangedCommandBehavior, GalleryGroup>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryGroupAllowVaryingItemSizeChangedCommandBehavior : CommandBehaviorBase<GalleryGroup>
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

	// GalleryGroupAllowVaryingItemSizeChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryGroupAllowVaryingItemSizeChangedCommandBehavior<T> : GalleryGroupAllowVaryingItemSizeChangedCommandBehavior
    { }
	#endregion

	#region GalleryGroupSpaceLimitBetweenItemsChangedCommand
	// GalleryGroupSpaceLimitBetweenItemsChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryGroupSpaceLimitBetweenItemsChangedCommand : ControlCommandBase<GalleryGroupSpaceLimitBetweenItemsChangedCommandBehavior, GalleryGroup>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryGroupSpaceLimitBetweenItemsChangedCommandBehavior : CommandBehaviorBase<GalleryGroup>
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

	// GalleryGroupSpaceLimitBetweenItemsChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryGroupSpaceLimitBetweenItemsChangedCommandBehavior<T> : GalleryGroupSpaceLimitBetweenItemsChangedCommandBehavior
    { }
	#endregion

	#region GalleryGroupVisualModeChangedCommand
	// GalleryGroupVisualModeChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryGroupVisualModeChangedCommand : ControlCommandBase<GalleryGroupVisualModeChangedCommandBehavior, GalleryGroup>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryGroupVisualModeChangedCommandBehavior : CommandBehaviorBase<GalleryGroup>
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

	// GalleryGroupVisualModeChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryGroupVisualModeChangedCommandBehavior<T> : GalleryGroupVisualModeChangedCommandBehavior
    { }
	#endregion

	#region GalleryGroupIsDragOverChangedCommand
	// GalleryGroupIsDragOverChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryGroupIsDragOverChangedCommand : ControlCommandBase<GalleryGroupIsDragOverChangedCommandBehavior, GalleryGroup>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryGroupIsDragOverChangedCommandBehavior : CommandBehaviorBase<GalleryGroup>
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
            TargetObject.IsDragOverChanged += OnEventRaised;
        }
    }

	// GalleryGroupIsDragOverChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryGroupIsDragOverChangedCommandBehavior<T> : GalleryGroupIsDragOverChangedCommandBehavior
    { }
	#endregion

	#region GalleryGroupCaptionAlignmentChangedCommand
	// GalleryGroupCaptionAlignmentChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryGroupCaptionAlignmentChangedCommand : ControlCommandBase<GalleryGroupCaptionAlignmentChangedCommandBehavior, GalleryGroup>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryGroupCaptionAlignmentChangedCommandBehavior : CommandBehaviorBase<GalleryGroup>
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

	// GalleryGroupCaptionAlignmentChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryGroupCaptionAlignmentChangedCommandBehavior<T> : GalleryGroupCaptionAlignmentChangedCommandBehavior
    { }
	#endregion

	#region GalleryGroupDescriptionAlignmentChangedCommand
	// GalleryGroupDescriptionAlignmentChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryGroupDescriptionAlignmentChangedCommand : ControlCommandBase<GalleryGroupDescriptionAlignmentChangedCommandBehavior, GalleryGroup>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryGroupDescriptionAlignmentChangedCommandBehavior : CommandBehaviorBase<GalleryGroup>
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

	// GalleryGroupDescriptionAlignmentChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryGroupDescriptionAlignmentChangedCommandBehavior<T> : GalleryGroupDescriptionAlignmentChangedCommandBehavior
    { }
	#endregion

	#region GalleryGroupIsAlwaysShownCaptionChangedCommand
	// GalleryGroupIsAlwaysShownCaptionChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class GalleryGroupIsAlwaysShownCaptionChangedCommand : ControlCommandBase<GalleryGroupIsAlwaysShownCaptionChangedCommandBehavior, GalleryGroup>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class GalleryGroupIsAlwaysShownCaptionChangedCommandBehavior : CommandBehaviorBase<GalleryGroup>
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

	// GalleryGroupIsAlwaysShownCaptionChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GalleryGroupIsAlwaysShownCaptionChangedCommandBehavior<T> : GalleryGroupIsAlwaysShownCaptionChangedCommandBehavior
    { }
	#endregion
}


