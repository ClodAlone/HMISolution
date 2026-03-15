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

	#region DocumentContainerAdjustStartPositionChangedCommand
	// DocumentContainerAdjustStartPositionChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DocumentContainerAdjustStartPositionChangedCommand : ControlCommandBase<DocumentContainerAdjustStartPositionChangedCommandBehavior, DocumentContainer>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DocumentContainerAdjustStartPositionChangedCommandBehavior : CommandBehaviorBase<DocumentContainer>
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
            TargetObject.AdjustStartPositionChanged += OnEventRaised;
        }
    }

	// DocumentContainerAdjustStartPositionChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DocumentContainerAdjustStartPositionChangedCommandBehavior<T> : DocumentContainerAdjustStartPositionChangedCommandBehavior
    { }
	#endregion

	#region DocumentContainerCloseAllTabsCommand
	// DocumentContainerCloseAllTabsCommand
    /// <summary>
    /// 
    /// </summary>
	public class DocumentContainerCloseAllTabsCommand : ControlCommandBase<DocumentContainerCloseAllTabsCommandBehavior, DocumentContainer>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DocumentContainerCloseAllTabsCommandBehavior : CommandBehaviorBase<DocumentContainer>
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
            TargetObject.CloseAllTabs += OnEventRaised;
        }
    }

	// DocumentContainerCloseAllTabsCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DocumentContainerCloseAllTabsCommandBehavior<T> : DocumentContainerCloseAllTabsCommandBehavior
    { }
	#endregion

	#region DocumentContainerCloseOtherTabsCommand
	// DocumentContainerCloseOtherTabsCommand
    /// <summary>
    /// 
    /// </summary>
	public class DocumentContainerCloseOtherTabsCommand : ControlCommandBase<DocumentContainerCloseOtherTabsCommandBehavior, DocumentContainer>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DocumentContainerCloseOtherTabsCommandBehavior : CommandBehaviorBase<DocumentContainer>
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
            TargetObject.CloseOtherTabs += OnEventRaised;
        }
    }

	// DocumentContainerCloseOtherTabsCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DocumentContainerCloseOtherTabsCommandBehavior<T> : DocumentContainerCloseOtherTabsCommandBehavior
    { }
	#endregion

	#region DocumentContainerShowTabListContextMenuChangedCommand
	// DocumentContainerShowTabListContextMenuChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DocumentContainerShowTabListContextMenuChangedCommand : ControlCommandBase<DocumentContainerShowTabListContextMenuChangedCommandBehavior, DocumentContainer>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DocumentContainerShowTabListContextMenuChangedCommandBehavior : CommandBehaviorBase<DocumentContainer>
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

	// DocumentContainerShowTabListContextMenuChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DocumentContainerShowTabListContextMenuChangedCommandBehavior<T> : DocumentContainerShowTabListContextMenuChangedCommandBehavior
    { }
	#endregion

	#region DocumentContainerShowTabItemContextMenuChangedCommand
	// DocumentContainerShowTabItemContextMenuChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DocumentContainerShowTabItemContextMenuChangedCommand : ControlCommandBase<DocumentContainerShowTabItemContextMenuChangedCommandBehavior, DocumentContainer>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DocumentContainerShowTabItemContextMenuChangedCommandBehavior : CommandBehaviorBase<DocumentContainer>
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

	// DocumentContainerShowTabItemContextMenuChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DocumentContainerShowTabItemContextMenuChangedCommandBehavior<T> : DocumentContainerShowTabItemContextMenuChangedCommandBehavior
    { }
	#endregion

	#region DocumentContainerShowDragAdornerChangedCommand
	// DocumentContainerShowDragAdornerChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DocumentContainerShowDragAdornerChangedCommand : ControlCommandBase<DocumentContainerShowDragAdornerChangedCommandBehavior, DocumentContainer>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DocumentContainerShowDragAdornerChangedCommandBehavior : CommandBehaviorBase<DocumentContainer>
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
            TargetObject.ShowDragAdornerChanged += OnEventRaised;
        }
    }

	// DocumentContainerShowDragAdornerChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DocumentContainerShowDragAdornerChangedCommandBehavior<T> : DocumentContainerShowDragAdornerChangedCommandBehavior
    { }
	#endregion

	#region DocumentContainerDragDropTemplateChangedCommand
	// DocumentContainerDragDropTemplateChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DocumentContainerDragDropTemplateChangedCommand : ControlCommandBase<DocumentContainerDragDropTemplateChangedCommandBehavior, DocumentContainer>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DocumentContainerDragDropTemplateChangedCommandBehavior : CommandBehaviorBase<DocumentContainer>
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
            TargetObject.DragDropTemplateChanged += OnEventRaised;
        }
    }

	// DocumentContainerDragDropTemplateChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DocumentContainerDragDropTemplateChangedCommandBehavior<T> : DocumentContainerDragDropTemplateChangedCommandBehavior
    { }
	#endregion

	#region DocumentContainerPersistStateSetCommand
	// DocumentContainerPersistStateSetCommand
    /// <summary>
    /// 
    /// </summary>
	public class DocumentContainerPersistStateSetCommand : ControlCommandBase<DocumentContainerPersistStateSetCommandBehavior, DocumentContainer>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DocumentContainerPersistStateSetCommandBehavior : CommandBehaviorBase<DocumentContainer>
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
            TargetObject.PersistStateSet += OnEventRaised;
        }
    }

	// DocumentContainerPersistStateSetCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DocumentContainerPersistStateSetCommandBehavior<T> : DocumentContainerPersistStateSetCommandBehavior
    { }
	#endregion

	#region DocumentContainerCornerRadiusChangedCommand
	// DocumentContainerCornerRadiusChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DocumentContainerCornerRadiusChangedCommand : ControlCommandBase<DocumentContainerCornerRadiusChangedCommandBehavior, DocumentContainer>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DocumentContainerCornerRadiusChangedCommandBehavior : CommandBehaviorBase<DocumentContainer>
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
            TargetObject.CornerRadiusChanged += OnEventRaised;
        }
    }

	// DocumentContainerCornerRadiusChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DocumentContainerCornerRadiusChangedCommandBehavior<T> : DocumentContainerCornerRadiusChangedCommandBehavior
    { }
	#endregion

	#region DocumentContainerIsLogicalOwnershipEnabledChangedCommand
	// DocumentContainerIsLogicalOwnershipEnabledChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DocumentContainerIsLogicalOwnershipEnabledChangedCommand : ControlCommandBase<DocumentContainerIsLogicalOwnershipEnabledChangedCommandBehavior, DocumentContainer>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DocumentContainerIsLogicalOwnershipEnabledChangedCommandBehavior : CommandBehaviorBase<DocumentContainer>
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
            TargetObject.IsLogicalOwnershipEnabledChanged += OnEventRaised;
        }
    }

	// DocumentContainerIsLogicalOwnershipEnabledChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DocumentContainerIsLogicalOwnershipEnabledChangedCommandBehavior<T> : DocumentContainerIsLogicalOwnershipEnabledChangedCommandBehavior
    { }
	#endregion

	#region DocumentContainerActiveDocumentChangedCommand
	// DocumentContainerActiveDocumentChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DocumentContainerActiveDocumentChangedCommand : ControlCommandBase<DocumentContainerActiveDocumentChangedCommandBehavior, DocumentContainer>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DocumentContainerActiveDocumentChangedCommandBehavior : CommandBehaviorBase<DocumentContainer>
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
            TargetObject.ActiveDocumentChanged += OnEventRaised;
        }
    }

	// DocumentContainerActiveDocumentChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DocumentContainerActiveDocumentChangedCommandBehavior<T> : DocumentContainerActiveDocumentChangedCommandBehavior
    { }
	#endregion

	#region DocumentContainerIsKeepCircleChangedCommand
	// DocumentContainerIsKeepCircleChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DocumentContainerIsKeepCircleChangedCommand : ControlCommandBase<DocumentContainerIsKeepCircleChangedCommandBehavior, DocumentContainer>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DocumentContainerIsKeepCircleChangedCommandBehavior : CommandBehaviorBase<DocumentContainer>
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
            TargetObject.IsKeepCircleChanged += OnEventRaised;
        }
    }

	// DocumentContainerIsKeepCircleChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DocumentContainerIsKeepCircleChangedCommandBehavior<T> : DocumentContainerIsKeepCircleChangedCommandBehavior
    { }
	#endregion

	#region DocumentContainerSwitchModeChangedCommand
	// DocumentContainerSwitchModeChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DocumentContainerSwitchModeChangedCommand : ControlCommandBase<DocumentContainerSwitchModeChangedCommandBehavior, DocumentContainer>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DocumentContainerSwitchModeChangedCommandBehavior : CommandBehaviorBase<DocumentContainer>
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
            TargetObject.SwitchModeChanged += OnEventRaised;
        }
    }

	// DocumentContainerSwitchModeChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DocumentContainerSwitchModeChangedCommandBehavior<T> : DocumentContainerSwitchModeChangedCommandBehavior
    { }
	#endregion

	#region DocumentContainerPersistStateChangedCommand
	// DocumentContainerPersistStateChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DocumentContainerPersistStateChangedCommand : ControlCommandBase<DocumentContainerPersistStateChangedCommandBehavior, DocumentContainer>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DocumentContainerPersistStateChangedCommandBehavior : CommandBehaviorBase<DocumentContainer>
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
            TargetObject.PersistStateChanged += OnEventRaised;
        }
    }

	// DocumentContainerPersistStateChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DocumentContainerPersistStateChangedCommandBehavior<T> : DocumentContainerPersistStateChangedCommandBehavior
    { }
	#endregion

	#region DocumentContainerDefaultMenuItemsPanelTemplateChangedCommand
	// DocumentContainerDefaultMenuItemsPanelTemplateChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DocumentContainerDefaultMenuItemsPanelTemplateChangedCommand : ControlCommandBase<DocumentContainerDefaultMenuItemsPanelTemplateChangedCommandBehavior, DocumentContainer>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DocumentContainerDefaultMenuItemsPanelTemplateChangedCommandBehavior : CommandBehaviorBase<DocumentContainer>
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
            TargetObject.DefaultMenuItemsPanelTemplateChanged += OnEventRaised;
        }
    }

	// DocumentContainerDefaultMenuItemsPanelTemplateChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DocumentContainerDefaultMenuItemsPanelTemplateChangedCommandBehavior<T> : DocumentContainerDefaultMenuItemsPanelTemplateChangedCommandBehavior
    { }
	#endregion

	#region DocumentContainerIsAllowMDIResizeChangedCommand
	// DocumentContainerIsAllowMDIResizeChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DocumentContainerIsAllowMDIResizeChangedCommand : ControlCommandBase<DocumentContainerIsAllowMDIResizeChangedCommandBehavior, DocumentContainer>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DocumentContainerIsAllowMDIResizeChangedCommandBehavior : CommandBehaviorBase<DocumentContainer>
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
            TargetObject.IsAllowMDIResizeChanged += OnEventRaised;
        }
    }

	// DocumentContainerIsAllowMDIResizeChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DocumentContainerIsAllowMDIResizeChangedCommandBehavior<T> : DocumentContainerIsAllowMDIResizeChangedCommandBehavior
    { }
	#endregion

	#region DocumentContainerUseInteropCompatibilityChangedCommand
	// DocumentContainerUseInteropCompatibilityChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DocumentContainerUseInteropCompatibilityChangedCommand : ControlCommandBase<DocumentContainerUseInteropCompatibilityChangedCommandBehavior, DocumentContainer>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DocumentContainerUseInteropCompatibilityChangedCommandBehavior : CommandBehaviorBase<DocumentContainer>
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
            TargetObject.UseInteropCompatibilityChanged += OnEventRaised;
        }
    }

	// DocumentContainerUseInteropCompatibilityChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DocumentContainerUseInteropCompatibilityChangedCommandBehavior<T> : DocumentContainerUseInteropCompatibilityChangedCommandBehavior
    { }
	#endregion

	#region DocumentContainerShowingFlipControlChangedCommand
	// DocumentContainerShowingFlipControlChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DocumentContainerShowingFlipControlChangedCommand : ControlCommandBase<DocumentContainerShowingFlipControlChangedCommandBehavior, DocumentContainer>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DocumentContainerShowingFlipControlChangedCommandBehavior : CommandBehaviorBase<DocumentContainer>
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
            TargetObject.ShowingFlipControlChanged += OnEventRaised;
        }
    }

	// DocumentContainerShowingFlipControlChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DocumentContainerShowingFlipControlChangedCommandBehavior<T> : DocumentContainerShowingFlipControlChangedCommandBehavior
    { }
	#endregion

	#region DocumentContainerIsInMDIMaximizedStateChangedCommand
	// DocumentContainerIsInMDIMaximizedStateChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DocumentContainerIsInMDIMaximizedStateChangedCommand : ControlCommandBase<DocumentContainerIsInMDIMaximizedStateChangedCommandBehavior, DocumentContainer>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DocumentContainerIsInMDIMaximizedStateChangedCommandBehavior : CommandBehaviorBase<DocumentContainer>
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
            TargetObject.IsInMDIMaximizedStateChanged += OnEventRaised;
        }
    }

	// DocumentContainerIsInMDIMaximizedStateChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DocumentContainerIsInMDIMaximizedStateChangedCommandBehavior<T> : DocumentContainerIsInMDIMaximizedStateChangedCommandBehavior
    { }
	#endregion

	#region DocumentContainerDisabledButtonsBehaviorChangedCommand
	// DocumentContainerDisabledButtonsBehaviorChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DocumentContainerDisabledButtonsBehaviorChangedCommand : ControlCommandBase<DocumentContainerDisabledButtonsBehaviorChangedCommandBehavior, DocumentContainer>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DocumentContainerDisabledButtonsBehaviorChangedCommandBehavior : CommandBehaviorBase<DocumentContainer>
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
            TargetObject.DisabledButtonsBehaviorChanged += OnEventRaised;
        }
    }

	// DocumentContainerDisabledButtonsBehaviorChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DocumentContainerDisabledButtonsBehaviorChangedCommandBehavior<T> : DocumentContainerDisabledButtonsBehaviorChangedCommandBehavior
    { }
	#endregion

	#region DocumentContainerModeChangedCommand
	// DocumentContainerModeChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DocumentContainerModeChangedCommand : ControlCommandBase<DocumentContainerModeChangedCommandBehavior, DocumentContainer>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DocumentContainerModeChangedCommandBehavior : CommandBehaviorBase<DocumentContainer>
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
            TargetObject.ModeChanged += OnEventRaised;
        }
    }

	// DocumentContainerModeChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DocumentContainerModeChangedCommandBehavior<T> : DocumentContainerModeChangedCommandBehavior
    { }
	#endregion

	#region DocumentContainerIsDocumentStateRequiredChangedCommand
	// DocumentContainerIsDocumentStateRequiredChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DocumentContainerIsDocumentStateRequiredChangedCommand : ControlCommandBase<DocumentContainerIsDocumentStateRequiredChangedCommandBehavior, DocumentContainer>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DocumentContainerIsDocumentStateRequiredChangedCommandBehavior : CommandBehaviorBase<DocumentContainer>
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
            TargetObject.IsDocumentStateRequiredChanged += OnEventRaised;
        }
    }

	// DocumentContainerIsDocumentStateRequiredChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DocumentContainerIsDocumentStateRequiredChangedCommandBehavior<T> : DocumentContainerIsDocumentStateRequiredChangedCommandBehavior
    { }
	#endregion

	#region DocumentContainerCanMDIMaximizeChangedCommand
	// DocumentContainerCanMDIMaximizeChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DocumentContainerCanMDIMaximizeChangedCommand : ControlCommandBase<DocumentContainerCanMDIMaximizeChangedCommandBehavior, DocumentContainer>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DocumentContainerCanMDIMaximizeChangedCommandBehavior : CommandBehaviorBase<DocumentContainer>
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
            TargetObject.CanMDIMaximizeChanged += OnEventRaised;
        }
    }

	// DocumentContainerCanMDIMaximizeChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DocumentContainerCanMDIMaximizeChangedCommandBehavior<T> : DocumentContainerCanMDIMaximizeChangedCommandBehavior
    { }
	#endregion

	#region DocumentContainerCanMDIMinimizeChangedCommand
	// DocumentContainerCanMDIMinimizeChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DocumentContainerCanMDIMinimizeChangedCommand : ControlCommandBase<DocumentContainerCanMDIMinimizeChangedCommandBehavior, DocumentContainer>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DocumentContainerCanMDIMinimizeChangedCommandBehavior : CommandBehaviorBase<DocumentContainer>
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
            TargetObject.CanMDIMinimizeChanged += OnEventRaised;
        }
    }

	// DocumentContainerCanMDIMinimizeChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DocumentContainerCanMDIMinimizeChangedCommandBehavior<T> : DocumentContainerCanMDIMinimizeChangedCommandBehavior
    { }
	#endregion

	#region DocumentContainerDelayPreviewTimeChangedCommand
	// DocumentContainerDelayPreviewTimeChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DocumentContainerDelayPreviewTimeChangedCommand : ControlCommandBase<DocumentContainerDelayPreviewTimeChangedCommandBehavior, DocumentContainer>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DocumentContainerDelayPreviewTimeChangedCommandBehavior : CommandBehaviorBase<DocumentContainer>
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
            TargetObject.DelayPreviewTimeChanged += OnEventRaised;
        }
    }

	// DocumentContainerDelayPreviewTimeChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DocumentContainerDelayPreviewTimeChangedCommandBehavior<T> : DocumentContainerDelayPreviewTimeChangedCommandBehavior
    { }
	#endregion

	#region DocumentContainerMDICommandsTargetChangedCommand
	// DocumentContainerMDICommandsTargetChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DocumentContainerMDICommandsTargetChangedCommand : ControlCommandBase<DocumentContainerMDICommandsTargetChangedCommandBehavior, DocumentContainer>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DocumentContainerMDICommandsTargetChangedCommandBehavior : CommandBehaviorBase<DocumentContainer>
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
            TargetObject.MDICommandsTargetChanged += OnEventRaised;
        }
    }

	// DocumentContainerMDICommandsTargetChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DocumentContainerMDICommandsTargetChangedCommandBehavior<T> : DocumentContainerMDICommandsTargetChangedCommandBehavior
    { }
	#endregion

	#region DocumentContainerIsEnabledScrollChangedCommand
	// DocumentContainerIsEnabledScrollChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DocumentContainerIsEnabledScrollChangedCommand : ControlCommandBase<DocumentContainerIsEnabledScrollChangedCommandBehavior, DocumentContainer>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DocumentContainerIsEnabledScrollChangedCommandBehavior : CommandBehaviorBase<DocumentContainer>
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
            TargetObject.IsEnabledScrollChanged += OnEventRaised;
        }
    }

	// DocumentContainerIsEnabledScrollChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DocumentContainerIsEnabledScrollChangedCommandBehavior<T> : DocumentContainerIsEnabledScrollChangedCommandBehavior
    { }
	#endregion

	#region DocumentContainerCloseButtonClickCommand
	// DocumentContainerCloseButtonClickCommand
    /// <summary>
    /// 
    /// </summary>
	public class DocumentContainerCloseButtonClickCommand : ControlCommandBase<DocumentContainerCloseButtonClickCommandBehavior, DocumentContainer>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DocumentContainerCloseButtonClickCommandBehavior : CommandBehaviorBase<DocumentContainer>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, CloseButtonEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.CloseButtonClick += OnEventRaised;
        }
    }

	// DocumentContainerCloseButtonClickCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DocumentContainerCloseButtonClickCommandBehavior<T> : DocumentContainerCloseButtonClickCommandBehavior
    { }
	#endregion

	#region DocumentContainerUseFlyCloseChangedCommand
	// DocumentContainerUseFlyCloseChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DocumentContainerUseFlyCloseChangedCommand : ControlCommandBase<DocumentContainerUseFlyCloseChangedCommandBehavior, DocumentContainer>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DocumentContainerUseFlyCloseChangedCommandBehavior : CommandBehaviorBase<DocumentContainer>
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
            TargetObject.UseFlyCloseChanged += OnEventRaised;
        }
    }

	// DocumentContainerUseFlyCloseChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DocumentContainerUseFlyCloseChangedCommandBehavior<T> : DocumentContainerUseFlyCloseChangedCommandBehavior
    { }
	#endregion

	#region DocumentContainerDocumentClosingCommand
	// DocumentContainerDocumentClosingCommand
    /// <summary>
    /// 
    /// </summary>
	public class DocumentContainerDocumentClosingCommand : ControlCommandBase<DocumentContainerDocumentClosingCommandBehavior, DocumentContainer>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DocumentContainerDocumentClosingCommandBehavior : CommandBehaviorBase<DocumentContainer>
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
            TargetObject.DocumentClosing += OnEventRaised;
        }
    }

	// DocumentContainerDocumentClosingCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DocumentContainerDocumentClosingCommandBehavior<T> : DocumentContainerDocumentClosingCommandBehavior
    { }
	#endregion

	#region DocumentContainerTabClosedCommand
	// DocumentContainerTabClosedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DocumentContainerTabClosedCommand : ControlCommandBase<DocumentContainerTabClosedCommandBehavior, DocumentContainer>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DocumentContainerTabClosedCommandBehavior : CommandBehaviorBase<DocumentContainer>
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

	// DocumentContainerTabClosedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DocumentContainerTabClosedCommandBehavior<T> : DocumentContainerTabClosedCommandBehavior
    { }
	#endregion

	#region DocumentContainerOpacityFactorOfVistaFlipChangedCommand
	// DocumentContainerOpacityFactorOfVistaFlipChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DocumentContainerOpacityFactorOfVistaFlipChangedCommand : ControlCommandBase<DocumentContainerOpacityFactorOfVistaFlipChangedCommandBehavior, DocumentContainer>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DocumentContainerOpacityFactorOfVistaFlipChangedCommandBehavior : CommandBehaviorBase<DocumentContainer>
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
            TargetObject.OpacityFactorOfVistaFlipChanged += OnEventRaised;
        }
    }

	// DocumentContainerOpacityFactorOfVistaFlipChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DocumentContainerOpacityFactorOfVistaFlipChangedCommandBehavior<T> : DocumentContainerOpacityFactorOfVistaFlipChangedCommandBehavior
    { }
	#endregion

	#region DocumentContainerFirstFlipItemOpacityChangedCommand
	// DocumentContainerFirstFlipItemOpacityChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DocumentContainerFirstFlipItemOpacityChangedCommand : ControlCommandBase<DocumentContainerFirstFlipItemOpacityChangedCommandBehavior, DocumentContainer>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DocumentContainerFirstFlipItemOpacityChangedCommandBehavior : CommandBehaviorBase<DocumentContainer>
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
            TargetObject.FirstFlipItemOpacityChanged += OnEventRaised;
        }
    }

	// DocumentContainerFirstFlipItemOpacityChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DocumentContainerFirstFlipItemOpacityChangedCommandBehavior<T> : DocumentContainerFirstFlipItemOpacityChangedCommandBehavior
    { }
	#endregion

	#region DocumentContainerVistaFlipItemsHeightFactorChangedCommand
	// DocumentContainerVistaFlipItemsHeightFactorChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DocumentContainerVistaFlipItemsHeightFactorChangedCommand : ControlCommandBase<DocumentContainerVistaFlipItemsHeightFactorChangedCommandBehavior, DocumentContainer>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DocumentContainerVistaFlipItemsHeightFactorChangedCommandBehavior : CommandBehaviorBase<DocumentContainer>
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
            TargetObject.VistaFlipItemsHeightFactorChanged += OnEventRaised;
        }
    }

	// DocumentContainerVistaFlipItemsHeightFactorChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DocumentContainerVistaFlipItemsHeightFactorChangedCommandBehavior<T> : DocumentContainerVistaFlipItemsHeightFactorChangedCommandBehavior
    { }
	#endregion

	#region DocumentContainerVistaFlipItemsWidthFactorChangedCommand
	// DocumentContainerVistaFlipItemsWidthFactorChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DocumentContainerVistaFlipItemsWidthFactorChangedCommand : ControlCommandBase<DocumentContainerVistaFlipItemsWidthFactorChangedCommandBehavior, DocumentContainer>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DocumentContainerVistaFlipItemsWidthFactorChangedCommandBehavior : CommandBehaviorBase<DocumentContainer>
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
            TargetObject.VistaFlipItemsWidthFactorChanged += OnEventRaised;
        }
    }

	// DocumentContainerVistaFlipItemsWidthFactorChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DocumentContainerVistaFlipItemsWidthFactorChangedCommandBehavior<T> : DocumentContainerVistaFlipItemsWidthFactorChangedCommandBehavior
    { }
	#endregion

	#region DocumentContainerFactoryOfViewVistaFlipChangedCommand
	// DocumentContainerFactoryOfViewVistaFlipChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DocumentContainerFactoryOfViewVistaFlipChangedCommand : ControlCommandBase<DocumentContainerFactoryOfViewVistaFlipChangedCommandBehavior, DocumentContainer>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DocumentContainerFactoryOfViewVistaFlipChangedCommandBehavior : CommandBehaviorBase<DocumentContainer>
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
            TargetObject.FactoryOfViewVistaFlipChanged += OnEventRaised;
        }
    }

	// DocumentContainerFactoryOfViewVistaFlipChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DocumentContainerFactoryOfViewVistaFlipChangedCommandBehavior<T> : DocumentContainerFactoryOfViewVistaFlipChangedCommandBehavior
    { }
	#endregion

	#region DocumentContainerVistaFlipAnimationDurationChangedCommand
	// DocumentContainerVistaFlipAnimationDurationChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DocumentContainerVistaFlipAnimationDurationChangedCommand : ControlCommandBase<DocumentContainerVistaFlipAnimationDurationChangedCommandBehavior, DocumentContainer>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DocumentContainerVistaFlipAnimationDurationChangedCommandBehavior : CommandBehaviorBase<DocumentContainer>
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
            TargetObject.VistaFlipAnimationDurationChanged += OnEventRaised;
        }
    }

	// DocumentContainerVistaFlipAnimationDurationChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DocumentContainerVistaFlipAnimationDurationChangedCommandBehavior<T> : DocumentContainerVistaFlipAnimationDurationChangedCommandBehavior
    { }
	#endregion

	#region DocumentContainerKeepLimitedVistaItemsStackChangedCommand
	// DocumentContainerKeepLimitedVistaItemsStackChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DocumentContainerKeepLimitedVistaItemsStackChangedCommand : ControlCommandBase<DocumentContainerKeepLimitedVistaItemsStackChangedCommandBehavior, DocumentContainer>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DocumentContainerKeepLimitedVistaItemsStackChangedCommandBehavior : CommandBehaviorBase<DocumentContainer>
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
            TargetObject.KeepLimitedVistaItemsStackChanged += OnEventRaised;
        }
    }

	// DocumentContainerKeepLimitedVistaItemsStackChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DocumentContainerKeepLimitedVistaItemsStackChangedCommandBehavior<T> : DocumentContainerKeepLimitedVistaItemsStackChangedCommandBehavior
    { }
	#endregion

	#region DocumentContainerToolWindowsListChangedCommand
	// DocumentContainerToolWindowsListChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DocumentContainerToolWindowsListChangedCommand : ControlCommandBase<DocumentContainerToolWindowsListChangedCommandBehavior, DocumentContainer>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DocumentContainerToolWindowsListChangedCommandBehavior : CommandBehaviorBase<DocumentContainer>
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
            TargetObject.ToolWindowsListChanged += OnEventRaised;
        }
    }

	// DocumentContainerToolWindowsListChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DocumentContainerToolWindowsListChangedCommandBehavior<T> : DocumentContainerToolWindowsListChangedCommandBehavior
    { }
	#endregion

	#region DocumentContainerToolWindowsListHeaderChangedCommand
	// DocumentContainerToolWindowsListHeaderChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DocumentContainerToolWindowsListHeaderChangedCommand : ControlCommandBase<DocumentContainerToolWindowsListHeaderChangedCommandBehavior, DocumentContainer>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DocumentContainerToolWindowsListHeaderChangedCommandBehavior : CommandBehaviorBase<DocumentContainer>
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
            TargetObject.ToolWindowsListHeaderChanged += OnEventRaised;
        }
    }

	// DocumentContainerToolWindowsListHeaderChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DocumentContainerToolWindowsListHeaderChangedCommandBehavior<T> : DocumentContainerToolWindowsListHeaderChangedCommandBehavior
    { }
	#endregion

	#region DocumentContainerToolWindowsListHeaderTemplateChangedCommand
	// DocumentContainerToolWindowsListHeaderTemplateChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DocumentContainerToolWindowsListHeaderTemplateChangedCommand : ControlCommandBase<DocumentContainerToolWindowsListHeaderTemplateChangedCommandBehavior, DocumentContainer>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DocumentContainerToolWindowsListHeaderTemplateChangedCommandBehavior : CommandBehaviorBase<DocumentContainer>
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
            TargetObject.ToolWindowsListHeaderTemplateChanged += OnEventRaised;
        }
    }

	// DocumentContainerToolWindowsListHeaderTemplateChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DocumentContainerToolWindowsListHeaderTemplateChangedCommandBehavior<T> : DocumentContainerToolWindowsListHeaderTemplateChangedCommandBehavior
    { }
	#endregion

	#region DocumentContainerDocumentListHeaderChangedCommand
	// DocumentContainerDocumentListHeaderChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DocumentContainerDocumentListHeaderChangedCommand : ControlCommandBase<DocumentContainerDocumentListHeaderChangedCommandBehavior, DocumentContainer>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DocumentContainerDocumentListHeaderChangedCommandBehavior : CommandBehaviorBase<DocumentContainer>
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
            TargetObject.DocumentListHeaderChanged += OnEventRaised;
        }
    }

	// DocumentContainerDocumentListHeaderChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DocumentContainerDocumentListHeaderChangedCommandBehavior<T> : DocumentContainerDocumentListHeaderChangedCommandBehavior
    { }
	#endregion

	#region DocumentContainerDocumentListHeaderTemplateChangedCommand
	// DocumentContainerDocumentListHeaderTemplateChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DocumentContainerDocumentListHeaderTemplateChangedCommand : ControlCommandBase<DocumentContainerDocumentListHeaderTemplateChangedCommandBehavior, DocumentContainer>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DocumentContainerDocumentListHeaderTemplateChangedCommandBehavior : CommandBehaviorBase<DocumentContainer>
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
            TargetObject.DocumentListHeaderTemplateChanged += OnEventRaised;
        }
    }

	// DocumentContainerDocumentListHeaderTemplateChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DocumentContainerDocumentListHeaderTemplateChangedCommandBehavior<T> : DocumentContainerDocumentListHeaderTemplateChangedCommandBehavior
    { }
	#endregion

	#region DocumentContainerToolWindowsItemSelectedCommand
	// DocumentContainerToolWindowsItemSelectedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DocumentContainerToolWindowsItemSelectedCommand : ControlCommandBase<DocumentContainerToolWindowsItemSelectedCommandBehavior, DocumentContainer>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DocumentContainerToolWindowsItemSelectedCommandBehavior : CommandBehaviorBase<DocumentContainer>
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
            TargetObject.ToolWindowsItemSelected += OnEventRaised;
        }
    }

	// DocumentContainerToolWindowsItemSelectedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DocumentContainerToolWindowsItemSelectedCommandBehavior<T> : DocumentContainerToolWindowsItemSelectedCommandBehavior
    { }
	#endregion
}


