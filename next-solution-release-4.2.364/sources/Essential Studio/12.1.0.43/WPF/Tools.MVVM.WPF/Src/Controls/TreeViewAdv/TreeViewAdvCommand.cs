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
using System.Windows.Controls;


namespace Syncfusion.Windows.Tools.MVVM
{   

	#region TreeViewAdvItemGeneratedCommand
	// TreeViewAdvItemGeneratedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TreeViewAdvItemGeneratedCommand : ControlCommandBase<TreeViewAdvItemGeneratedCommandBehavior, TreeViewAdv>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TreeViewAdvItemGeneratedCommandBehavior : CommandBehaviorBase<TreeViewAdv>
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

	// TreeViewAdvItemGeneratedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TreeViewAdvItemGeneratedCommandBehavior<T> : TreeViewAdvItemGeneratedCommandBehavior
    { }
	#endregion

	#region TreeViewAdvExpandingCommand
	// TreeViewAdvExpandingCommand
    /// <summary>
    /// 
    /// </summary>
	public class TreeViewAdvExpandingCommand : ControlCommandBase<TreeViewAdvExpandingCommandBehavior, TreeViewAdv>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TreeViewAdvExpandingCommandBehavior : CommandBehaviorBase<TreeViewAdv>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, ExpandingCollapsingEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.Expanding += OnEventRaised;
        }
    }

	// TreeViewAdvExpandingCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TreeViewAdvExpandingCommandBehavior<T> : TreeViewAdvExpandingCommandBehavior
    { }
	#endregion

	#region TreeViewAdvCollapsingCommand
	// TreeViewAdvCollapsingCommand
    /// <summary>
    /// 
    /// </summary>
	public class TreeViewAdvCollapsingCommand : ControlCommandBase<TreeViewAdvCollapsingCommandBehavior, TreeViewAdv>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TreeViewAdvCollapsingCommandBehavior : CommandBehaviorBase<TreeViewAdv>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, ExpandingCollapsingEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.Collapsing += OnEventRaised;
        }
    }

	// TreeViewAdvCollapsingCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TreeViewAdvCollapsingCommandBehavior<T> : TreeViewAdvCollapsingCommandBehavior
    { }
	#endregion

	#region TreeViewAdvExpandedCommand
	// TreeViewAdvExpandedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TreeViewAdvExpandedCommand : ControlCommandBase<TreeViewAdvExpandedCommandBehavior, TreeViewAdv>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TreeViewAdvExpandedCommandBehavior : CommandBehaviorBase<TreeViewAdv>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, ExpandedCollapsedEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.Expanded += OnEventRaised;
        }
    }

	// TreeViewAdvExpandedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TreeViewAdvExpandedCommandBehavior<T> : TreeViewAdvExpandedCommandBehavior
    { }
	#endregion

	#region TreeViewAdvCollapsedCommand
	// TreeViewAdvCollapsedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TreeViewAdvCollapsedCommand : ControlCommandBase<TreeViewAdvCollapsedCommandBehavior, TreeViewAdv>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TreeViewAdvCollapsedCommandBehavior : CommandBehaviorBase<TreeViewAdv>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, ExpandedCollapsedEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.Collapsed += OnEventRaised;
        }
    }

	// TreeViewAdvCollapsedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TreeViewAdvCollapsedCommandBehavior<T> : TreeViewAdvCollapsedCommandBehavior
    { }
	#endregion

	#region TreeViewAdvBeforeItemSortCommand
	// TreeViewAdvBeforeItemSortCommand
    /// <summary>
    /// 
    /// </summary>
	public class TreeViewAdvBeforeItemSortCommand : ControlCommandBase<TreeViewAdvBeforeItemSortCommandBehavior, TreeViewAdv>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TreeViewAdvBeforeItemSortCommandBehavior : CommandBehaviorBase<TreeViewAdv>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, SortModeChangeEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.BeforeItemSort += OnEventRaised;
        }
    }

	// TreeViewAdvBeforeItemSortCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TreeViewAdvBeforeItemSortCommandBehavior<T> : TreeViewAdvBeforeItemSortCommandBehavior
    { }
	#endregion

	#region TreeViewAdvAfterItemSortCommand
	// TreeViewAdvAfterItemSortCommand
    /// <summary>
    /// 
    /// </summary>
	public class TreeViewAdvAfterItemSortCommand : ControlCommandBase<TreeViewAdvAfterItemSortCommandBehavior, TreeViewAdv>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TreeViewAdvAfterItemSortCommandBehavior : CommandBehaviorBase<TreeViewAdv>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, SortModeChangeEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.AfterItemSort += OnEventRaised;
        }
    }

	// TreeViewAdvAfterItemSortCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TreeViewAdvAfterItemSortCommandBehavior<T> : TreeViewAdvAfterItemSortCommandBehavior
    { }
	#endregion

	#region TreeViewAdvSelectedItemChangedCommand
	// TreeViewAdvSelectedItemChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TreeViewAdvSelectedItemChangedCommand : ControlCommandBase<TreeViewAdvSelectedItemChangedCommandBehavior, TreeViewAdv>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TreeViewAdvSelectedItemChangedCommandBehavior : CommandBehaviorBase<TreeViewAdv>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.SelectedItemChanged += OnEventRaised;
        }
    }

	// TreeViewAdvSelectedItemChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TreeViewAdvSelectedItemChangedCommandBehavior<T> : TreeViewAdvSelectedItemChangedCommandBehavior
    { }
	#endregion

	#region TreeViewAdvSelectionChangedCommand
	// TreeViewAdvSelectionChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TreeViewAdvSelectionChangedCommand : ControlCommandBase<TreeViewAdvSelectionChangedCommandBehavior, TreeViewAdv>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TreeViewAdvSelectionChangedCommandBehavior : CommandBehaviorBase<TreeViewAdv>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, SelectionChangedEventArgs e)
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

	// TreeViewAdvSelectionChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TreeViewAdvSelectionChangedCommandBehavior<T> : TreeViewAdvSelectionChangedCommandBehavior
    { }
	#endregion

	#region TreeViewAdvPreviewSelectedItemChangedCommand
	// TreeViewAdvPreviewSelectedItemChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TreeViewAdvPreviewSelectedItemChangedCommand : ControlCommandBase<TreeViewAdvPreviewSelectedItemChangedCommandBehavior, TreeViewAdv>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TreeViewAdvPreviewSelectedItemChangedCommandBehavior : CommandBehaviorBase<TreeViewAdv>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.PreviewSelectedItemChanged += OnEventRaised;
        }
    }

	// TreeViewAdvPreviewSelectedItemChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TreeViewAdvPreviewSelectedItemChangedCommandBehavior<T> : TreeViewAdvPreviewSelectedItemChangedCommandBehavior
    { }
	#endregion

	#region TreeViewAdvDragStartCommand
	// TreeViewAdvDragStartCommand
    /// <summary>
    /// 
    /// </summary>
	public class TreeViewAdvDragStartCommand : ControlCommandBase<TreeViewAdvDragStartCommandBehavior, TreeViewAdv>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TreeViewAdvDragStartCommandBehavior : CommandBehaviorBase<TreeViewAdv>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, DragTreeViewItemAdvEventArgs e)
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

	// TreeViewAdvDragStartCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TreeViewAdvDragStartCommandBehavior<T> : TreeViewAdvDragStartCommandBehavior
    { }
	#endregion

	#region TreeViewAdvDragOverCommand
	// TreeViewAdvDragOverCommand
    /// <summary>
    /// 
    /// </summary>
	public class TreeViewAdvDragOverCommand : ControlCommandBase<TreeViewAdvDragOverCommandBehavior, TreeViewAdv>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TreeViewAdvDragOverCommandBehavior : CommandBehaviorBase<TreeViewAdv>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, DragTreeViewItemAdvEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.DragOver += OnEventRaised;
        }
    }

	// TreeViewAdvDragOverCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TreeViewAdvDragOverCommandBehavior<T> : TreeViewAdvDragOverCommandBehavior
    { }
	#endregion

	#region TreeViewAdvDragEndCommand
	// TreeViewAdvDragEndCommand
    /// <summary>
    /// 
    /// </summary>
	public class TreeViewAdvDragEndCommand : ControlCommandBase<TreeViewAdvDragEndCommandBehavior, TreeViewAdv>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TreeViewAdvDragEndCommandBehavior : CommandBehaviorBase<TreeViewAdv>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, DragTreeViewItemAdvEventArgs e)
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

	// TreeViewAdvDragEndCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TreeViewAdvDragEndCommandBehavior<T> : TreeViewAdvDragEndCommandBehavior
    { }
	#endregion

	#region TreeViewAdvAllowDynamicResizingChangedCommand
	// TreeViewAdvAllowDynamicResizingChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TreeViewAdvAllowDynamicResizingChangedCommand : ControlCommandBase<TreeViewAdvAllowDynamicResizingChangedCommandBehavior, TreeViewAdv>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TreeViewAdvAllowDynamicResizingChangedCommandBehavior : CommandBehaviorBase<TreeViewAdv>
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
            TargetObject.AllowDynamicResizingChanged += OnEventRaised;
        }
    }

	// TreeViewAdvAllowDynamicResizingChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TreeViewAdvAllowDynamicResizingChangedCommandBehavior<T> : TreeViewAdvAllowDynamicResizingChangedCommandBehavior
    { }
	#endregion

	#region TreeViewAdvLoadOnDemandCommand
	// TreeViewAdvLoadOnDemandCommand
    /// <summary>
    /// 
    /// </summary>
	public class TreeViewAdvLoadOnDemandCommand : ControlCommandBase<TreeViewAdvLoadOnDemandCommandBehavior, TreeViewAdv>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TreeViewAdvLoadOnDemandCommandBehavior : CommandBehaviorBase<TreeViewAdv>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, LoadonDemandEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.LoadOnDemand += OnEventRaised;
        }
    }

	// TreeViewAdvLoadOnDemandCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TreeViewAdvLoadOnDemandCommandBehavior<T> : TreeViewAdvLoadOnDemandCommandBehavior
    { }
	#endregion
}


