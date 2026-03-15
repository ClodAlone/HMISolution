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

	#region TabSplitterItemSplitterPagesSelectionChangedCommand
	// TabSplitterItemSplitterPagesSelectionChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TabSplitterItemSplitterPagesSelectionChangedCommand : ControlCommandBase<TabSplitterItemSplitterPagesSelectionChangedCommandBehavior, TabSplitterItem>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TabSplitterItemSplitterPagesSelectionChangedCommandBehavior : CommandBehaviorBase<TabSplitterItem>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, SplitterPagesSelectionChangedEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.SplitterPagesSelectionChanged += OnEventRaised;
        }
    }

	// TabSplitterItemSplitterPagesSelectionChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TabSplitterItemSplitterPagesSelectionChangedCommandBehavior<T> : TabSplitterItemSplitterPagesSelectionChangedCommandBehavior
    { }
	#endregion

	#region TabSplitterItemOrientationChangedCommand
	// TabSplitterItemOrientationChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TabSplitterItemOrientationChangedCommand : ControlCommandBase<TabSplitterItemOrientationChangedCommandBehavior, TabSplitterItem>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TabSplitterItemOrientationChangedCommandBehavior : CommandBehaviorBase<TabSplitterItem>
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
            TargetObject.OrientationChanged += OnEventRaised;
        }
    }

	// TabSplitterItemOrientationChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TabSplitterItemOrientationChangedCommandBehavior<T> : TabSplitterItemOrientationChangedCommandBehavior
    { }
	#endregion

	#region TabSplitterItemIsCollapsedBottomPanelChangedCommand
	// TabSplitterItemIsCollapsedBottomPanelChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TabSplitterItemIsCollapsedBottomPanelChangedCommand : ControlCommandBase<TabSplitterItemIsCollapsedBottomPanelChangedCommandBehavior, TabSplitterItem>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TabSplitterItemIsCollapsedBottomPanelChangedCommandBehavior : CommandBehaviorBase<TabSplitterItem>
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
            TargetObject.IsCollapsedBottomPanelChanged += OnEventRaised;
        }
    }

	// TabSplitterItemIsCollapsedBottomPanelChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TabSplitterItemIsCollapsedBottomPanelChangedCommandBehavior<T> : TabSplitterItemIsCollapsedBottomPanelChangedCommandBehavior
    { }
	#endregion

	#region TabSplitterItemSelectedPageChangedCommand
	// TabSplitterItemSelectedPageChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TabSplitterItemSelectedPageChangedCommand : ControlCommandBase<TabSplitterItemSelectedPageChangedCommandBehavior, TabSplitterItem>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TabSplitterItemSelectedPageChangedCommandBehavior : CommandBehaviorBase<TabSplitterItem>
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
            TargetObject.SelectedPageChanged += OnEventRaised;
        }
    }

	// TabSplitterItemSelectedPageChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TabSplitterItemSelectedPageChangedCommandBehavior<T> : TabSplitterItemSelectedPageChangedCommandBehavior
    { }
	#endregion
}


