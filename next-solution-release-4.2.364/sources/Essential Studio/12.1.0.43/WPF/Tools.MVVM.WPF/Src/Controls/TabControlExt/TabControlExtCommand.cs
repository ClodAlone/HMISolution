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

    #region TabControlExtSelectionChangedCommand
    // TabControlExtSelectionChangedCommand
    /// <summary>
    /// 
    /// </summary>
    public class TabControlExtSelectionChangedCommand : ControlCommandBase<TabControlExtSelectionChangedCommandBehavior, TabControlExt>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class TabControlExtSelectionChangedCommandBehavior : CommandBehaviorBase<TabControlExt>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, EventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.SelectionChanged += OnEventRaised;
        }
    }

    // TabControlExtSelectionChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TabControlExtSelectionChangedCommandBehavior<T> : TabControlExtSelectionChangedCommandBehavior
    { }
    #endregion


	#region TabControlExtNewButtonClickCommand
	// TabControlExtNewButtonClickCommand
    /// <summary>
    /// 
    /// </summary>
	public class TabControlExtNewButtonClickCommand : ControlCommandBase<TabControlExtNewButtonClickCommandBehavior, TabControlExt>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TabControlExtNewButtonClickCommandBehavior : CommandBehaviorBase<TabControlExt>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, EventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.NewButtonClick += OnEventRaised;
        }
    }

	// TabControlExtNewButtonClickCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TabControlExtNewButtonClickCommandBehavior<T> : TabControlExtNewButtonClickCommandBehavior
    { }
	#endregion

	#region TabControlExtTabListContextMenuItemTemplateChangedCommand
	// TabControlExtTabListContextMenuItemTemplateChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TabControlExtTabListContextMenuItemTemplateChangedCommand : ControlCommandBase<TabControlExtTabListContextMenuItemTemplateChangedCommandBehavior, TabControlExt>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TabControlExtTabListContextMenuItemTemplateChangedCommandBehavior : CommandBehaviorBase<TabControlExt>
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
            TargetObject.TabListContextMenuItemTemplateChanged += OnEventRaised;
        }
    }

	// TabControlExtTabListContextMenuItemTemplateChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TabControlExtTabListContextMenuItemTemplateChangedCommandBehavior<T> : TabControlExtTabListContextMenuItemTemplateChangedCommandBehavior
    { }
	#endregion

	#region TabControlExtScrollingTimeChangedCommand
	// TabControlExtScrollingTimeChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TabControlExtScrollingTimeChangedCommand : ControlCommandBase<TabControlExtScrollingTimeChangedCommandBehavior, TabControlExt>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TabControlExtScrollingTimeChangedCommandBehavior : CommandBehaviorBase<TabControlExt>
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
            TargetObject.ScrollingTimeChanged += OnEventRaised;
        }
    }

	// TabControlExtScrollingTimeChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TabControlExtScrollingTimeChangedCommandBehavior<T> : TabControlExtScrollingTimeChangedCommandBehavior
    { }
	#endregion

	#region TabControlExtSelectedIndexChangedCommand
	// TabControlExtSelectedIndexChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TabControlExtSelectedIndexChangedCommand : ControlCommandBase<TabControlExtSelectedIndexChangedCommandBehavior, TabControlExt>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TabControlExtSelectedIndexChangedCommandBehavior : CommandBehaviorBase<TabControlExt>
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
            TargetObject.SelectedIndexChanged += OnEventRaised;
        }
    }

	// TabControlExtSelectedIndexChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TabControlExtSelectedIndexChangedCommandBehavior<T> : TabControlExtSelectedIndexChangedCommandBehavior
    { }
	#endregion

	#region TabControlExtFlowDirectionChangedCommand
	// TabControlExtFlowDirectionChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TabControlExtFlowDirectionChangedCommand : ControlCommandBase<TabControlExtFlowDirectionChangedCommandBehavior, TabControlExt>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TabControlExtFlowDirectionChangedCommandBehavior : CommandBehaviorBase<TabControlExt>
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
            TargetObject.FlowDirectionChanged += OnEventRaised;
        }
    }

	// TabControlExtFlowDirectionChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TabControlExtFlowDirectionChangedCommandBehavior<T> : TabControlExtFlowDirectionChangedCommandBehavior
    { }
	#endregion

	#region TabControlExtTabStripPlacementChangedCommand
	// TabControlExtTabStripPlacementChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TabControlExtTabStripPlacementChangedCommand : ControlCommandBase<TabControlExtTabStripPlacementChangedCommandBehavior, TabControlExt>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TabControlExtTabStripPlacementChangedCommandBehavior : CommandBehaviorBase<TabControlExt>
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
            TargetObject.TabStripPlacementChanged += OnEventRaised;
        }
    }

	// TabControlExtTabStripPlacementChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TabControlExtTabStripPlacementChangedCommandBehavior<T> : TabControlExtTabStripPlacementChangedCommandBehavior
    { }
	#endregion

	#region TabControlExtDragMarkerStyleChangedCommand
	// TabControlExtDragMarkerStyleChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TabControlExtDragMarkerStyleChangedCommand : ControlCommandBase<TabControlExtDragMarkerStyleChangedCommandBehavior, TabControlExt>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TabControlExtDragMarkerStyleChangedCommandBehavior : CommandBehaviorBase<TabControlExt>
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
            TargetObject.DragMarkerStyleChanged += OnEventRaised;
        }
    }

	// TabControlExtDragMarkerStyleChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TabControlExtDragMarkerStyleChangedCommandBehavior<T> : TabControlExtDragMarkerStyleChangedCommandBehavior
    { }
	#endregion

	#region TabControlExtDragMarkerColorChangedCommand
	// TabControlExtDragMarkerColorChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TabControlExtDragMarkerColorChangedCommand : ControlCommandBase<TabControlExtDragMarkerColorChangedCommandBehavior, TabControlExt>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TabControlExtDragMarkerColorChangedCommandBehavior : CommandBehaviorBase<TabControlExt>
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
            TargetObject.DragMarkerColorChanged += OnEventRaised;
        }
    }

	// TabControlExtDragMarkerColorChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TabControlExtDragMarkerColorChangedCommandBehavior<T> : TabControlExtDragMarkerColorChangedCommandBehavior
    { }
	#endregion

	#region TabControlExtTabPanelBackgroundChangedCommand
	// TabControlExtTabPanelBackgroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TabControlExtTabPanelBackgroundChangedCommand : ControlCommandBase<TabControlExtTabPanelBackgroundChangedCommandBehavior, TabControlExt>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TabControlExtTabPanelBackgroundChangedCommandBehavior : CommandBehaviorBase<TabControlExt>
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
            TargetObject.TabPanelBackgroundChanged += OnEventRaised;
        }
    }

	// TabControlExtTabPanelBackgroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TabControlExtTabPanelBackgroundChangedCommandBehavior<T> : TabControlExtTabPanelBackgroundChangedCommandBehavior
    { }
	#endregion

	#region TabControlExtEnableLabelEditChangedCommand
	// TabControlExtEnableLabelEditChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TabControlExtEnableLabelEditChangedCommand : ControlCommandBase<TabControlExtEnableLabelEditChangedCommandBehavior, TabControlExt>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TabControlExtEnableLabelEditChangedCommandBehavior : CommandBehaviorBase<TabControlExt>
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
            TargetObject.EnableLabelEditChanged += OnEventRaised;
        }
    }

	// TabControlExtEnableLabelEditChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TabControlExtEnableLabelEditChangedCommandBehavior<T> : TabControlExtEnableLabelEditChangedCommandBehavior
    { }
	#endregion

	#region TabControlExtTabPanelItemChangedCommand
	// TabControlExtTabPanelItemChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TabControlExtTabPanelItemChangedCommand : ControlCommandBase<TabControlExtTabPanelItemChangedCommandBehavior, TabControlExt>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TabControlExtTabPanelItemChangedCommandBehavior : CommandBehaviorBase<TabControlExt>
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
            TargetObject.TabPanelItemChanged += OnEventRaised;
        }
    }

	// TabControlExtTabPanelItemChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TabControlExtTabPanelItemChangedCommandBehavior<T> : TabControlExtTabPanelItemChangedCommandBehavior
    { }
	#endregion

	#region TabControlExtShowTabListContextMenuChangedCommand
	// TabControlExtShowTabListContextMenuChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TabControlExtShowTabListContextMenuChangedCommand : ControlCommandBase<TabControlExtShowTabListContextMenuChangedCommandBehavior, TabControlExt>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TabControlExtShowTabListContextMenuChangedCommandBehavior : CommandBehaviorBase<TabControlExt>
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
            TargetObject.ShowTabListContextMenuChanged += OnEventRaised;
        }
    }

	// TabControlExtShowTabListContextMenuChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TabControlExtShowTabListContextMenuChangedCommandBehavior<T> : TabControlExtShowTabListContextMenuChangedCommandBehavior
    { }
	#endregion

	#region TabControlExtShowTabItemContextMenuChangedCommand
	// TabControlExtShowTabItemContextMenuChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TabControlExtShowTabItemContextMenuChangedCommand : ControlCommandBase<TabControlExtShowTabItemContextMenuChangedCommandBehavior, TabControlExt>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TabControlExtShowTabItemContextMenuChangedCommandBehavior : CommandBehaviorBase<TabControlExt>
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
            TargetObject.ShowTabItemContextMenuChanged += OnEventRaised;
        }
    }

	// TabControlExtShowTabItemContextMenuChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TabControlExtShowTabItemContextMenuChangedCommandBehavior<T> : TabControlExtShowTabItemContextMenuChangedCommandBehavior
    { }
	#endregion

	#region TabControlExtCloseButtonTypeChangedCommand
	// TabControlExtCloseButtonTypeChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TabControlExtCloseButtonTypeChangedCommand : ControlCommandBase<TabControlExtCloseButtonTypeChangedCommandBehavior, TabControlExt>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TabControlExtCloseButtonTypeChangedCommandBehavior : CommandBehaviorBase<TabControlExt>
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
            TargetObject.CloseButtonTypeChanged += OnEventRaised;
        }
    }

	// TabControlExtCloseButtonTypeChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TabControlExtCloseButtonTypeChangedCommandBehavior<T> : TabControlExtCloseButtonTypeChangedCommandBehavior
    { }
	#endregion

	#region TabControlExtHotTrackingEnabledChangedCommand
	// TabControlExtHotTrackingEnabledChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TabControlExtHotTrackingEnabledChangedCommand : ControlCommandBase<TabControlExtHotTrackingEnabledChangedCommandBehavior, TabControlExt>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TabControlExtHotTrackingEnabledChangedCommandBehavior : CommandBehaviorBase<TabControlExt>
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
            TargetObject.HotTrackingEnabledChanged += OnEventRaised;
        }
    }

	// TabControlExtHotTrackingEnabledChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TabControlExtHotTrackingEnabledChangedCommandBehavior<T> : TabControlExtHotTrackingEnabledChangedCommandBehavior
    { }
	#endregion

	#region TabControlExtTabPanelTemplateChangedCommand
	// TabControlExtTabPanelTemplateChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TabControlExtTabPanelTemplateChangedCommand : ControlCommandBase<TabControlExtTabPanelTemplateChangedCommandBehavior, TabControlExt>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TabControlExtTabPanelTemplateChangedCommandBehavior : CommandBehaviorBase<TabControlExt>
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
            TargetObject.TabPanelTemplateChanged += OnEventRaised;
        }
    }

	// TabControlExtTabPanelTemplateChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TabControlExtTabPanelTemplateChangedCommandBehavior<T> : TabControlExtTabPanelTemplateChangedCommandBehavior
    { }
	#endregion

	#region TabControlExtTabPanelStyleChangedCommand
	// TabControlExtTabPanelStyleChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TabControlExtTabPanelStyleChangedCommand : ControlCommandBase<TabControlExtTabPanelStyleChangedCommandBehavior, TabControlExt>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TabControlExtTabPanelStyleChangedCommandBehavior : CommandBehaviorBase<TabControlExt>
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
            TargetObject.TabPanelStyleChanged += OnEventRaised;
        }
    }

	// TabControlExtTabPanelStyleChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TabControlExtTabPanelStyleChangedCommandBehavior<T> : TabControlExtTabPanelStyleChangedCommandBehavior
    { }
	#endregion

	#region TabControlExtTabScrollButtonVisibilityChangedCommand
	// TabControlExtTabScrollButtonVisibilityChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TabControlExtTabScrollButtonVisibilityChangedCommand : ControlCommandBase<TabControlExtTabScrollButtonVisibilityChangedCommandBehavior, TabControlExt>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TabControlExtTabScrollButtonVisibilityChangedCommandBehavior : CommandBehaviorBase<TabControlExt>
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
            TargetObject.TabScrollButtonVisibilityChanged += OnEventRaised;
        }
    }

	// TabControlExtTabScrollButtonVisibilityChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TabControlExtTabScrollButtonVisibilityChangedCommandBehavior<T> : TabControlExtTabScrollButtonVisibilityChangedCommandBehavior
    { }
	#endregion

	#region TabControlExtTabScrollStyleChangedCommand
	// TabControlExtTabScrollStyleChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TabControlExtTabScrollStyleChangedCommand : ControlCommandBase<TabControlExtTabScrollStyleChangedCommandBehavior, TabControlExt>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TabControlExtTabScrollStyleChangedCommandBehavior : CommandBehaviorBase<TabControlExt>
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
            TargetObject.TabScrollStyleChanged += OnEventRaised;
        }
    }

	// TabControlExtTabScrollStyleChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TabControlExtTabScrollStyleChangedCommandBehavior<T> : TabControlExtTabScrollStyleChangedCommandBehavior
    { }
	#endregion

	#region TabControlExtTabItemLayoutChangedCommand
	// TabControlExtTabItemLayoutChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TabControlExtTabItemLayoutChangedCommand : ControlCommandBase<TabControlExtTabItemLayoutChangedCommandBehavior, TabControlExt>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TabControlExtTabItemLayoutChangedCommandBehavior : CommandBehaviorBase<TabControlExt>
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
            TargetObject.TabItemLayoutChanged += OnEventRaised;
        }
    }

	// TabControlExtTabItemLayoutChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TabControlExtTabItemLayoutChangedCommandBehavior<T> : TabControlExtTabItemLayoutChangedCommandBehavior
    { }
	#endregion

	#region TabControlExtTabItemSizeChangedCommand
	// TabControlExtTabItemSizeChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TabControlExtTabItemSizeChangedCommand : ControlCommandBase<TabControlExtTabItemSizeChangedCommandBehavior, TabControlExt>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TabControlExtTabItemSizeChangedCommandBehavior : CommandBehaviorBase<TabControlExt>
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
            TargetObject.TabItemSizeChanged += OnEventRaised;
        }
    }

	// TabControlExtTabItemSizeChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TabControlExtTabItemSizeChangedCommandBehavior<T> : TabControlExtTabItemSizeChangedCommandBehavior
    { }
	#endregion

	#region TabControlExtKeepTabInFrontChangedCommand
	// TabControlExtKeepTabInFrontChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TabControlExtKeepTabInFrontChangedCommand : ControlCommandBase<TabControlExtKeepTabInFrontChangedCommandBehavior, TabControlExt>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TabControlExtKeepTabInFrontChangedCommandBehavior : CommandBehaviorBase<TabControlExt>
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
            TargetObject.KeepTabInFrontChanged += OnEventRaised;
        }
    }

	// TabControlExtKeepTabInFrontChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TabControlExtKeepTabInFrontChangedCommandBehavior<T> : TabControlExtKeepTabInFrontChangedCommandBehavior
    { }
	#endregion

	#region TabControlExtRotateTextWhenVerticalChangedCommand
	// TabControlExtRotateTextWhenVerticalChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TabControlExtRotateTextWhenVerticalChangedCommand : ControlCommandBase<TabControlExtRotateTextWhenVerticalChangedCommandBehavior, TabControlExt>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TabControlExtRotateTextWhenVerticalChangedCommandBehavior : CommandBehaviorBase<TabControlExt>
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
            TargetObject.RotateTextWhenVerticalChanged += OnEventRaised;
        }
    }

	// TabControlExtRotateTextWhenVerticalChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TabControlExtRotateTextWhenVerticalChangedCommandBehavior<T> : TabControlExtRotateTextWhenVerticalChangedCommandBehavior
    { }
	#endregion

	#region TabControlExtOnCloseOtherTabsCommand
	// TabControlExtOnCloseOtherTabsCommand
    /// <summary>
    /// 
    /// </summary>
	public class TabControlExtOnCloseOtherTabsCommand : ControlCommandBase<TabControlExtOnCloseOtherTabsCommandBehavior, TabControlExt>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TabControlExtOnCloseOtherTabsCommandBehavior : CommandBehaviorBase<TabControlExt>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, CloseTabEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.OnCloseOtherTabs += OnEventRaised;
        }
    }

	// TabControlExtOnCloseOtherTabsCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TabControlExtOnCloseOtherTabsCommandBehavior<T> : TabControlExtOnCloseOtherTabsCommandBehavior
    { }
	#endregion

	#region TabControlExtOnCloseAllTabsCommand
	// TabControlExtOnCloseAllTabsCommand
    /// <summary>
    /// 
    /// </summary>
	public class TabControlExtOnCloseAllTabsCommand : ControlCommandBase<TabControlExtOnCloseAllTabsCommandBehavior, TabControlExt>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TabControlExtOnCloseAllTabsCommandBehavior : CommandBehaviorBase<TabControlExt>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, CloseTabEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.OnCloseAllTabs += OnEventRaised;
        }
    }

	// TabControlExtOnCloseAllTabsCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TabControlExtOnCloseAllTabsCommandBehavior<T> : TabControlExtOnCloseAllTabsCommandBehavior
    { }
	#endregion

	#region TabControlExtOnCloseButtonClickCommand
	// TabControlExtOnCloseButtonClickCommand
    /// <summary>
    /// 
    /// </summary>
	public class TabControlExtOnCloseButtonClickCommand : ControlCommandBase<TabControlExtOnCloseButtonClickCommandBehavior, TabControlExt>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TabControlExtOnCloseButtonClickCommandBehavior : CommandBehaviorBase<TabControlExt>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, CloseTabEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.OnCloseButtonClick += OnEventRaised;
        }
    }

	// TabControlExtOnCloseButtonClickCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TabControlExtOnCloseButtonClickCommandBehavior<T> : TabControlExtOnCloseButtonClickCommandBehavior
    { }
	#endregion

	#region TabControlExtBeforeDropDownContextMenuOpenCommand
	// TabControlExtBeforeDropDownContextMenuOpenCommand
    /// <summary>
    /// 
    /// </summary>
	public class TabControlExtBeforeDropDownContextMenuOpenCommand : ControlCommandBase<TabControlExtBeforeDropDownContextMenuOpenCommandBehavior, TabControlExt>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TabControlExtBeforeDropDownContextMenuOpenCommandBehavior : CommandBehaviorBase<TabControlExt>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, EventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.BeforeDropDownContextMenuOpen += OnEventRaised;
        }
    }

	// TabControlExtBeforeDropDownContextMenuOpenCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TabControlExtBeforeDropDownContextMenuOpenCommandBehavior<T> : TabControlExtBeforeDropDownContextMenuOpenCommandBehavior
    { }
	#endregion

	#region TabControlExtBeforeDropDownContextMenuCloseCommand
	// TabControlExtBeforeDropDownContextMenuCloseCommand
    /// <summary>
    /// 
    /// </summary>
	public class TabControlExtBeforeDropDownContextMenuCloseCommand : ControlCommandBase<TabControlExtBeforeDropDownContextMenuCloseCommandBehavior, TabControlExt>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TabControlExtBeforeDropDownContextMenuCloseCommandBehavior : CommandBehaviorBase<TabControlExt>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, EventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.BeforeDropDownContextMenuClose += OnEventRaised;
        }
    }

	// TabControlExtBeforeDropDownContextMenuCloseCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TabControlExtBeforeDropDownContextMenuCloseCommandBehavior<T> : TabControlExtBeforeDropDownContextMenuCloseCommandBehavior
    { }
	#endregion

	#region TabControlExtBeforeLabelEditCommand
	// TabControlExtBeforeLabelEditCommand
    /// <summary>
    /// 
    /// </summary>
	public class TabControlExtBeforeLabelEditCommand : ControlCommandBase<TabControlExtBeforeLabelEditCommandBehavior, TabControlExt>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TabControlExtBeforeLabelEditCommandBehavior : CommandBehaviorBase<TabControlExt>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, BeforeLabelEditEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.BeforeLabelEdit += OnEventRaised;
        }
    }

	// TabControlExtBeforeLabelEditCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TabControlExtBeforeLabelEditCommandBehavior<T> : TabControlExtBeforeLabelEditCommandBehavior
    { }
	#endregion

	#region TabControlExtAfterLabelEditCommand
	// TabControlExtAfterLabelEditCommand
    /// <summary>
    /// 
    /// </summary>
	public class TabControlExtAfterLabelEditCommand : ControlCommandBase<TabControlExtAfterLabelEditCommandBehavior, TabControlExt>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TabControlExtAfterLabelEditCommandBehavior : CommandBehaviorBase<TabControlExt>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, AfterLabelEditEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.AfterLabelEdit += OnEventRaised;
        }
    }

	// TabControlExtAfterLabelEditCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TabControlExtAfterLabelEditCommandBehavior<T> : TabControlExtAfterLabelEditCommandBehavior
    { }
	#endregion

	#region TabControlExtTakeDragItemEventCommand
	// TabControlExtTakeDragItemEventCommand
    /// <summary>
    /// 
    /// </summary>
	public class TabControlExtTakeDragItemEventCommand : ControlCommandBase<TabControlExtTakeDragItemEventCommandBehavior, TabControlExt>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TabControlExtTakeDragItemEventCommandBehavior : CommandBehaviorBase<TabControlExt>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, TakeDragItemEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.TakeDragItemEvent += OnEventRaised;
        }
    }

	// TabControlExtTakeDragItemEventCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TabControlExtTakeDragItemEventCommandBehavior<T> : TabControlExtTakeDragItemEventCommandBehavior
    { }
	#endregion

	#region TabControlExtSelectedItemChangedEventCommand
	// TabControlExtSelectedItemChangedEventCommand
    /// <summary>
    /// 
    /// </summary>
	public class TabControlExtSelectedItemChangedEventCommand : ControlCommandBase<TabControlExtSelectedItemChangedEventCommandBehavior, TabControlExt>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TabControlExtSelectedItemChangedEventCommandBehavior : CommandBehaviorBase<TabControlExt>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, SelectedItemChangedEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.SelectedItemChangedEvent += OnEventRaised;
        }
    }

	// TabControlExtSelectedItemChangedEventCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TabControlExtSelectedItemChangedEventCommandBehavior<T> : TabControlExtSelectedItemChangedEventCommandBehavior
    { }
	#endregion

	#region TabControlExtPreviewSelectedItemChangedEventCommand
	// TabControlExtPreviewSelectedItemChangedEventCommand
    /// <summary>
    /// 
    /// </summary>
	public class TabControlExtPreviewSelectedItemChangedEventCommand : ControlCommandBase<TabControlExtPreviewSelectedItemChangedEventCommandBehavior, TabControlExt>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TabControlExtPreviewSelectedItemChangedEventCommandBehavior : CommandBehaviorBase<TabControlExt>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, PreviewSelectedItemChangedEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.PreviewSelectedItemChangedEvent += OnEventRaised;
        }
    }

	// TabControlExtPreviewSelectedItemChangedEventCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TabControlExtPreviewSelectedItemChangedEventCommandBehavior<T> : TabControlExtPreviewSelectedItemChangedEventCommandBehavior
    { }
	#endregion

	#region TabControlExtSelectedItemFontWeightChangedCommand
	// TabControlExtSelectedItemFontWeightChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TabControlExtSelectedItemFontWeightChangedCommand : ControlCommandBase<TabControlExtSelectedItemFontWeightChangedCommandBehavior, TabControlExt>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TabControlExtSelectedItemFontWeightChangedCommandBehavior : CommandBehaviorBase<TabControlExt>
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
            TargetObject.SelectedItemFontWeightChanged += OnEventRaised;
        }
    }

	// TabControlExtSelectedItemFontWeightChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TabControlExtSelectedItemFontWeightChangedCommandBehavior<T> : TabControlExtSelectedItemFontWeightChangedCommandBehavior
    { }
	#endregion

	#region TabControlExtAllowDragDropChangedCommand
	// TabControlExtAllowDragDropChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TabControlExtAllowDragDropChangedCommand : ControlCommandBase<TabControlExtAllowDragDropChangedCommandBehavior, TabControlExt>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TabControlExtAllowDragDropChangedCommandBehavior : CommandBehaviorBase<TabControlExt>
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
            TargetObject.AllowDragDropChanged += OnEventRaised;
        }
    }

	// TabControlExtAllowDragDropChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TabControlExtAllowDragDropChangedCommandBehavior<T> : TabControlExtAllowDragDropChangedCommandBehavior
    { }
	#endregion

	#region TabControlExtTabClosingCommand
	// TabControlExtTabClosingCommand
    /// <summary>
    /// 
    /// </summary>
	public class TabControlExtTabClosingCommand : ControlCommandBase<TabControlExtTabClosingCommandBehavior, TabControlExt>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TabControlExtTabClosingCommandBehavior : CommandBehaviorBase<TabControlExt>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, CancelingRoutedEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.TabClosing += OnEventRaised;
        }
    }

	// TabControlExtTabClosingCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TabControlExtTabClosingCommandBehavior<T> : TabControlExtTabClosingCommandBehavior
    { }
	#endregion

	#region TabControlExtTabClosedCommand
	// TabControlExtTabClosedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TabControlExtTabClosedCommand : ControlCommandBase<TabControlExtTabClosedCommandBehavior, TabControlExt>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TabControlExtTabClosedCommandBehavior : CommandBehaviorBase<TabControlExt>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, CloseTabEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.TabClosed += OnEventRaised;
        }
    }

	// TabControlExtTabClosedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TabControlExtTabClosedCommandBehavior<T> : TabControlExtTabClosedCommandBehavior
    { }
	#endregion

	#region TabControlExtDragStartCommand
	// TabControlExtDragStartCommand
    /// <summary>
    /// 
    /// </summary>
	public class TabControlExtDragStartCommand : ControlCommandBase<TabControlExtDragStartCommandBehavior, TabControlExt>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TabControlExtDragStartCommandBehavior : CommandBehaviorBase<TabControlExt>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, TabControlExtDragEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.DragStart += OnEventRaised;
        }
    }

	// TabControlExtDragStartCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TabControlExtDragStartCommandBehavior<T> : TabControlExtDragStartCommandBehavior
    { }
	#endregion

	#region TabControlExtDragEndCommand
	// TabControlExtDragEndCommand
    /// <summary>
    /// 
    /// </summary>
	public class TabControlExtDragEndCommand : ControlCommandBase<TabControlExtDragEndCommandBehavior, TabControlExt>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TabControlExtDragEndCommandBehavior : CommandBehaviorBase<TabControlExt>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, TabControlExtDragEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.DragEnd += OnEventRaised;
        }
    }

	// TabControlExtDragEndCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TabControlExtDragEndCommandBehavior<T> : TabControlExtDragEndCommandBehavior
    { }
	#endregion
}


