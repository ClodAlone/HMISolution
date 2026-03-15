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

	#region TabSplitterMouseOverBackgroundChangedCommand
	// TabSplitterMouseOverBackgroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TabSplitterMouseOverBackgroundChangedCommand : ControlCommandBase<TabSplitterMouseOverBackgroundChangedCommandBehavior, TabSplitter>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TabSplitterMouseOverBackgroundChangedCommandBehavior : CommandBehaviorBase<TabSplitter>
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
            TargetObject.MouseOverBackgroundChanged += OnEventRaised;
        }
    }

	// TabSplitterMouseOverBackgroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TabSplitterMouseOverBackgroundChangedCommandBehavior<T> : TabSplitterMouseOverBackgroundChangedCommandBehavior
    { }
	#endregion

	#region TabSplitterMouseOverForegroundChangedCommand
	// TabSplitterMouseOverForegroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TabSplitterMouseOverForegroundChangedCommand : ControlCommandBase<TabSplitterMouseOverForegroundChangedCommandBehavior, TabSplitter>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TabSplitterMouseOverForegroundChangedCommandBehavior : CommandBehaviorBase<TabSplitter>
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
            TargetObject.MouseOverForegroundChanged += OnEventRaised;
        }
    }

	// TabSplitterMouseOverForegroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TabSplitterMouseOverForegroundChangedCommandBehavior<T> : TabSplitterMouseOverForegroundChangedCommandBehavior
    { }
	#endregion

	#region TabSplitterSelectedBackgroundChangedCommand
	// TabSplitterSelectedBackgroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TabSplitterSelectedBackgroundChangedCommand : ControlCommandBase<TabSplitterSelectedBackgroundChangedCommandBehavior, TabSplitter>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TabSplitterSelectedBackgroundChangedCommandBehavior : CommandBehaviorBase<TabSplitter>
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
            TargetObject.SelectedBackgroundChanged += OnEventRaised;
        }
    }

	// TabSplitterSelectedBackgroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TabSplitterSelectedBackgroundChangedCommandBehavior<T> : TabSplitterSelectedBackgroundChangedCommandBehavior
    { }
	#endregion

	#region TabSplitterSelectedForegroundChangedCommand
	// TabSplitterSelectedForegroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TabSplitterSelectedForegroundChangedCommand : ControlCommandBase<TabSplitterSelectedForegroundChangedCommandBehavior, TabSplitter>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TabSplitterSelectedForegroundChangedCommandBehavior : CommandBehaviorBase<TabSplitter>
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
            TargetObject.SelectedForegroundChanged += OnEventRaised;
        }
    }

	// TabSplitterSelectedForegroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TabSplitterSelectedForegroundChangedCommandBehavior<T> : TabSplitterSelectedForegroundChangedCommandBehavior
    { }
	#endregion

	#region TabSplitterBottomSelectedContentChangedCommand
	// TabSplitterBottomSelectedContentChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TabSplitterBottomSelectedContentChangedCommand : ControlCommandBase<TabSplitterBottomSelectedContentChangedCommandBehavior, TabSplitter>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TabSplitterBottomSelectedContentChangedCommandBehavior : CommandBehaviorBase<TabSplitter>
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
            TargetObject.BottomSelectedContentChanged += OnEventRaised;
        }
    }

	// TabSplitterBottomSelectedContentChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TabSplitterBottomSelectedContentChangedCommandBehavior<T> : TabSplitterBottomSelectedContentChangedCommandBehavior
    { }
	#endregion

	#region TabSplitterTopSelectedContentChangedCommand
	// TabSplitterTopSelectedContentChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TabSplitterTopSelectedContentChangedCommand : ControlCommandBase<TabSplitterTopSelectedContentChangedCommandBehavior, TabSplitter>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TabSplitterTopSelectedContentChangedCommandBehavior : CommandBehaviorBase<TabSplitter>
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
            TargetObject.TopSelectedContentChanged += OnEventRaised;
        }
    }

	// TabSplitterTopSelectedContentChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TabSplitterTopSelectedContentChangedCommandBehavior<T> : TabSplitterTopSelectedContentChangedCommandBehavior
    { }
	#endregion

	#region TabSplitterTabSplitterListContextMenuItemTemplateChangedCommand
	// TabSplitterTabSplitterListContextMenuItemTemplateChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TabSplitterTabSplitterListContextMenuItemTemplateChangedCommand : ControlCommandBase<TabSplitterTabSplitterListContextMenuItemTemplateChangedCommandBehavior, TabSplitter>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TabSplitterTabSplitterListContextMenuItemTemplateChangedCommandBehavior : CommandBehaviorBase<TabSplitter>
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
            TargetObject.TabSplitterListContextMenuItemTemplateChanged += OnEventRaised;
        }
    }

	// TabSplitterTabSplitterListContextMenuItemTemplateChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TabSplitterTabSplitterListContextMenuItemTemplateChangedCommandBehavior<T> : TabSplitterTabSplitterListContextMenuItemTemplateChangedCommandBehavior
    { }
	#endregion

	#region TabSplitterBeforeDropDownContextMenuOpenCommand
	// TabSplitterBeforeDropDownContextMenuOpenCommand
    /// <summary>
    /// 
    /// </summary>
	public class TabSplitterBeforeDropDownContextMenuOpenCommand : ControlCommandBase<TabSplitterBeforeDropDownContextMenuOpenCommandBehavior, TabSplitter>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TabSplitterBeforeDropDownContextMenuOpenCommandBehavior : CommandBehaviorBase<TabSplitter>
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

	// TabSplitterBeforeDropDownContextMenuOpenCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TabSplitterBeforeDropDownContextMenuOpenCommandBehavior<T> : TabSplitterBeforeDropDownContextMenuOpenCommandBehavior
    { }
	#endregion

	#region TabSplitterBeforeDropDownContextMenuCloseCommand
	// TabSplitterBeforeDropDownContextMenuCloseCommand
    /// <summary>
    /// 
    /// </summary>
	public class TabSplitterBeforeDropDownContextMenuCloseCommand : ControlCommandBase<TabSplitterBeforeDropDownContextMenuCloseCommandBehavior, TabSplitter>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TabSplitterBeforeDropDownContextMenuCloseCommandBehavior : CommandBehaviorBase<TabSplitter>
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

	// TabSplitterBeforeDropDownContextMenuCloseCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TabSplitterBeforeDropDownContextMenuCloseCommandBehavior<T> : TabSplitterBeforeDropDownContextMenuCloseCommandBehavior
    { }
	#endregion
}


