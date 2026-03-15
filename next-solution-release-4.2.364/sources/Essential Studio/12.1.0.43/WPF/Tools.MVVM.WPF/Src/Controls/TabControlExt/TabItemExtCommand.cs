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

	#region TabItemExtFlowDirectionChangedCommand
	// TabItemExtFlowDirectionChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TabItemExtFlowDirectionChangedCommand : ControlCommandBase<TabItemExtFlowDirectionChangedCommandBehavior, TabItemExt>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TabItemExtFlowDirectionChangedCommandBehavior : CommandBehaviorBase<TabItemExt>
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

	// TabItemExtFlowDirectionChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TabItemExtFlowDirectionChangedCommandBehavior<T> : TabItemExtFlowDirectionChangedCommandBehavior
    { }
	#endregion

	#region TabItemExtUseCustomEditableTemplateChangedCommand
	// TabItemExtUseCustomEditableTemplateChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TabItemExtUseCustomEditableTemplateChangedCommand : ControlCommandBase<TabItemExtUseCustomEditableTemplateChangedCommandBehavior, TabItemExt>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TabItemExtUseCustomEditableTemplateChangedCommandBehavior : CommandBehaviorBase<TabItemExt>
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
            TargetObject.UseCustomEditableTemplateChanged += OnEventRaised;
        }
    }

	// TabItemExtUseCustomEditableTemplateChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TabItemExtUseCustomEditableTemplateChangedCommandBehavior<T> : TabItemExtUseCustomEditableTemplateChangedCommandBehavior
    { }
	#endregion

	#region TabItemExtCustomEditableTemplateChangedCommand
	// TabItemExtCustomEditableTemplateChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TabItemExtCustomEditableTemplateChangedCommand : ControlCommandBase<TabItemExtCustomEditableTemplateChangedCommandBehavior, TabItemExt>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TabItemExtCustomEditableTemplateChangedCommandBehavior : CommandBehaviorBase<TabItemExt>
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
            TargetObject.CustomEditableTemplateChanged += OnEventRaised;
        }
    }

	// TabItemExtCustomEditableTemplateChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TabItemExtCustomEditableTemplateChangedCommandBehavior<T> : TabItemExtCustomEditableTemplateChangedCommandBehavior
    { }
	#endregion

	#region TabItemExtContextMenuItemsChangedCommand
	// TabItemExtContextMenuItemsChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TabItemExtContextMenuItemsChangedCommand : ControlCommandBase<TabItemExtContextMenuItemsChangedCommandBehavior, TabItemExt>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TabItemExtContextMenuItemsChangedCommandBehavior : CommandBehaviorBase<TabItemExt>
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
            TargetObject.ContextMenuItemsChanged += OnEventRaised;
        }
    }

	// TabItemExtContextMenuItemsChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TabItemExtContextMenuItemsChangedCommandBehavior<T> : TabItemExtContextMenuItemsChangedCommandBehavior
    { }
	#endregion

	#region TabItemExtTabItemContextMenuItemTemplateChangedCommand
	// TabItemExtTabItemContextMenuItemTemplateChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TabItemExtTabItemContextMenuItemTemplateChangedCommand : ControlCommandBase<TabItemExtTabItemContextMenuItemTemplateChangedCommandBehavior, TabItemExt>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TabItemExtTabItemContextMenuItemTemplateChangedCommandBehavior : CommandBehaviorBase<TabItemExt>
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
            TargetObject.TabItemContextMenuItemTemplateChanged += OnEventRaised;
        }
    }

	// TabItemExtTabItemContextMenuItemTemplateChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TabItemExtTabItemContextMenuItemTemplateChangedCommandBehavior<T> : TabItemExtTabItemContextMenuItemTemplateChangedCommandBehavior
    { }
	#endregion

	#region TabItemExtHoverBackgroundChangedCommand
	// TabItemExtHoverBackgroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TabItemExtHoverBackgroundChangedCommand : ControlCommandBase<TabItemExtHoverBackgroundChangedCommandBehavior, TabItemExt>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TabItemExtHoverBackgroundChangedCommandBehavior : CommandBehaviorBase<TabItemExt>
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
            TargetObject.HoverBackgroundChanged += OnEventRaised;
        }
    }

	// TabItemExtHoverBackgroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TabItemExtHoverBackgroundChangedCommandBehavior<T> : TabItemExtHoverBackgroundChangedCommandBehavior
    { }
	#endregion

	#region TabItemExtImageAlignmentChangedCommand
	// TabItemExtImageAlignmentChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TabItemExtImageAlignmentChangedCommand : ControlCommandBase<TabItemExtImageAlignmentChangedCommandBehavior, TabItemExt>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TabItemExtImageAlignmentChangedCommandBehavior : CommandBehaviorBase<TabItemExt>
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
            TargetObject.ImageAlignmentChanged += OnEventRaised;
        }
    }

	// TabItemExtImageAlignmentChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TabItemExtImageAlignmentChangedCommandBehavior<T> : TabItemExtImageAlignmentChangedCommandBehavior
    { }
	#endregion

	#region TabItemExtImageChangedCommand
	// TabItemExtImageChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TabItemExtImageChangedCommand : ControlCommandBase<TabItemExtImageChangedCommandBehavior, TabItemExt>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TabItemExtImageChangedCommandBehavior : CommandBehaviorBase<TabItemExt>
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
            TargetObject.ImageChanged += OnEventRaised;
        }
    }

	// TabItemExtImageChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TabItemExtImageChangedCommandBehavior<T> : TabItemExtImageChangedCommandBehavior
    { }
	#endregion

	#region TabItemExtItemToolTipChangedCommand
	// TabItemExtItemToolTipChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class TabItemExtItemToolTipChangedCommand : ControlCommandBase<TabItemExtItemToolTipChangedCommandBehavior, TabItemExt>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class TabItemExtItemToolTipChangedCommandBehavior : CommandBehaviorBase<TabItemExt>
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
            TargetObject.ItemToolTipChanged += OnEventRaised;
        }
    }

	// TabItemExtItemToolTipChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TabItemExtItemToolTipChangedCommandBehavior<T> : TabItemExtItemToolTipChangedCommandBehavior
    { }
	#endregion
}


