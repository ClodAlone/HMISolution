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

    #region DockingManagerContainerStyleChangedCommand
    // DockingManagerContainerStyleChangedCommand
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerContainerStyleChangedCommand : ControlCommandBase<DockingManagerContainerStyleChangedCommandBehavior, DockingManager>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerContainerStyleChangedCommandBehavior : CommandBehaviorBase<DockingManager>
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
            TargetObject.ContainerStyleChanged += OnEventRaised;
        }
    }

    // DockingManagerContainerStyleChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerContainerStyleChangedCommandBehavior<T> : DockingManagerContainerStyleChangedCommandBehavior
    { }
    #endregion

    #region DockingManagerContainerModeChangedCommand
    // DockingManagerContainerModeChangedCommand
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerContainerModeChangedCommand : ControlCommandBase<DockingManagerContainerModeChangedCommandBehavior, DockingManager>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerContainerModeChangedCommandBehavior : CommandBehaviorBase<DockingManager>
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
            TargetObject.ContainerModeChanged += OnEventRaised;
        }
    }

    // DockingManagerContainerModeChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerContainerModeChangedCommandBehavior<T> : DockingManagerContainerModeChangedCommandBehavior
    { }
    #endregion

    #region DockingManagerSwitchModeChangedCommand
    // DockingManagerSwitchModeChangedCommand
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerSwitchModeChangedCommand : ControlCommandBase<DockingManagerSwitchModeChangedCommandBehavior, DockingManager>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerSwitchModeChangedCommandBehavior : CommandBehaviorBase<DockingManager>
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

    // DockingManagerSwitchModeChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerSwitchModeChangedCommandBehavior<T> : DockingManagerSwitchModeChangedCommandBehavior
    { }
    #endregion

    #region DockingManagerCloseAllTabsCommand
    // DockingManagerCloseAllTabsCommand
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerCloseAllTabsCommand : ControlCommandBase<DockingManagerCloseAllTabsCommandBehavior, DockingManager>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerCloseAllTabsCommandBehavior : CommandBehaviorBase<DockingManager>
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

    // DockingManagerCloseAllTabsCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerCloseAllTabsCommandBehavior<T> : DockingManagerCloseAllTabsCommandBehavior
    { }
    #endregion

    #region DockingManagerCloseOtherTabsCommand
    // DockingManagerCloseOtherTabsCommand
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerCloseOtherTabsCommand : ControlCommandBase<DockingManagerCloseOtherTabsCommandBehavior, DockingManager>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerCloseOtherTabsCommandBehavior : CommandBehaviorBase<DockingManager>
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

    // DockingManagerCloseOtherTabsCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerCloseOtherTabsCommandBehavior<T> : DockingManagerCloseOtherTabsCommandBehavior
    { }
    #endregion

    #region DockingManagerCloseButtonClickCommand
    // DockingManagerCloseButtonClickCommand
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerCloseButtonClickCommand : ControlCommandBase<DockingManagerCloseButtonClickCommandBehavior, DockingManager>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerCloseButtonClickCommandBehavior : CommandBehaviorBase<DockingManager>
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

    // DockingManagerCloseButtonClickCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerCloseButtonClickCommandBehavior<T> : DockingManagerCloseButtonClickCommandBehavior
    { }
    #endregion

    #region DockingManagerIsSelectedDocumentCommand
    // DockingManagerIsSelectedDocumentCommand
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerIsSelectedDocumentCommand : ControlCommandBase<DockingManagerIsSelectedDocumentCommandBehavior, DockingManager>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerIsSelectedDocumentCommandBehavior : CommandBehaviorBase<DockingManager>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, IsSelectedChangedEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.IsSelectedDocument += OnEventRaised;
        }
    }

    // DockingManagerIsSelectedDocumentCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerIsSelectedDocumentCommandBehavior<T> : DockingManagerIsSelectedDocumentCommandBehavior
    { }
    #endregion

    #region DockingManagerShowTabListContextMenuChangedCommand
    // DockingManagerShowTabListContextMenuChangedCommand
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerShowTabListContextMenuChangedCommand : ControlCommandBase<DockingManagerShowTabListContextMenuChangedCommandBehavior, DockingManager>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerShowTabListContextMenuChangedCommandBehavior : CommandBehaviorBase<DockingManager>
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

    // DockingManagerShowTabListContextMenuChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerShowTabListContextMenuChangedCommandBehavior<T> : DockingManagerShowTabListContextMenuChangedCommandBehavior
    { }
    #endregion

    #region DockingManagerShowTabItemContextMenuChangedCommand
    // DockingManagerShowTabItemContextMenuChangedCommand
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerShowTabItemContextMenuChangedCommand : ControlCommandBase<DockingManagerShowTabItemContextMenuChangedCommandBehavior, DockingManager>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerShowTabItemContextMenuChangedCommandBehavior : CommandBehaviorBase<DockingManager>
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

    // DockingManagerShowTabItemContextMenuChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerShowTabItemContextMenuChangedCommandBehavior<T> : DockingManagerShowTabItemContextMenuChangedCommandBehavior
    { }
    #endregion

    #region DockingManagerIsInMDIMaximizedStateChangedCommand
    // DockingManagerIsInMDIMaximizedStateChangedCommand
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerIsInMDIMaximizedStateChangedCommand : ControlCommandBase<DockingManagerIsInMDIMaximizedStateChangedCommandBehavior, DockingManager>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerIsInMDIMaximizedStateChangedCommandBehavior : CommandBehaviorBase<DockingManager>
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

    // DockingManagerIsInMDIMaximizedStateChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerIsInMDIMaximizedStateChangedCommandBehavior<T> : DockingManagerIsInMDIMaximizedStateChangedCommandBehavior
    { }
    #endregion

    #region DockingManagerFloatWindowHeaderBackgroundChangedCommand
    // DockingManagerFloatWindowHeaderBackgroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerFloatWindowHeaderBackgroundChangedCommand : ControlCommandBase<DockingManagerFloatWindowHeaderBackgroundChangedCommandBehavior, DockingManager>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerFloatWindowHeaderBackgroundChangedCommandBehavior : CommandBehaviorBase<DockingManager>
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
            TargetObject.FloatWindowHeaderBackgroundChanged += OnEventRaised;
        }
    }

    // DockingManagerFloatWindowHeaderBackgroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerFloatWindowHeaderBackgroundChangedCommandBehavior<T> : DockingManagerFloatWindowHeaderBackgroundChangedCommandBehavior
    { }
    #endregion

    #region DockingManagerFloatWindowBorderBrushChangedCommand
    // DockingManagerFloatWindowBorderBrushChangedCommand
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerFloatWindowBorderBrushChangedCommand : ControlCommandBase<DockingManagerFloatWindowBorderBrushChangedCommandBehavior, DockingManager>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerFloatWindowBorderBrushChangedCommandBehavior : CommandBehaviorBase<DockingManager>
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
            TargetObject.FloatWindowBorderBrushChanged += OnEventRaised;
        }
    }

    // DockingManagerFloatWindowBorderBrushChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerFloatWindowBorderBrushChangedCommandBehavior<T> : DockingManagerFloatWindowBorderBrushChangedCommandBehavior
    { }
    #endregion

    #region DockingManagerFloatWindowSelectedBorderBrushChangedCommand
    // DockingManagerFloatWindowSelectedBorderBrushChangedCommand
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerFloatWindowSelectedBorderBrushChangedCommand : ControlCommandBase<DockingManagerFloatWindowSelectedBorderBrushChangedCommandBehavior, DockingManager>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerFloatWindowSelectedBorderBrushChangedCommandBehavior : CommandBehaviorBase<DockingManager>
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
            TargetObject.FloatWindowSelectedBorderBrushChanged += OnEventRaised;
        }
    }

    // DockingManagerFloatWindowSelectedBorderBrushChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerFloatWindowSelectedBorderBrushChangedCommandBehavior<T> : DockingManagerFloatWindowSelectedBorderBrushChangedCommandBehavior
    { }
    #endregion

    #region DockingManagerFloatWindowMouseOverBorderBrushChangedCommand
    // DockingManagerFloatWindowMouseOverBorderBrushChangedCommand
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerFloatWindowMouseOverBorderBrushChangedCommand : ControlCommandBase<DockingManagerFloatWindowMouseOverBorderBrushChangedCommandBehavior, DockingManager>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerFloatWindowMouseOverBorderBrushChangedCommandBehavior : CommandBehaviorBase<DockingManager>
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
            TargetObject.FloatWindowMouseOverBorderBrushChanged += OnEventRaised;
        }
    }

    // DockingManagerFloatWindowMouseOverBorderBrushChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerFloatWindowMouseOverBorderBrushChangedCommandBehavior<T> : DockingManagerFloatWindowMouseOverBorderBrushChangedCommandBehavior
    { }
    #endregion

    #region DockingManagerFloatWindowBorderThicknessChangedCommand
    // DockingManagerFloatWindowBorderThicknessChangedCommand
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerFloatWindowBorderThicknessChangedCommand : ControlCommandBase<DockingManagerFloatWindowBorderThicknessChangedCommandBehavior, DockingManager>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerFloatWindowBorderThicknessChangedCommandBehavior : CommandBehaviorBase<DockingManager>
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
            TargetObject.FloatWindowBorderThicknessChanged += OnEventRaised;
        }
    }

    // DockingManagerFloatWindowBorderThicknessChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerFloatWindowBorderThicknessChangedCommandBehavior<T> : DockingManagerFloatWindowBorderThicknessChangedCommandBehavior
    { }
    #endregion

    #region DockingManagerFloatWindowSelectedHeaderBackgroundChangedCommand
    // DockingManagerFloatWindowSelectedHeaderBackgroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerFloatWindowSelectedHeaderBackgroundChangedCommand : ControlCommandBase<DockingManagerFloatWindowSelectedHeaderBackgroundChangedCommandBehavior, DockingManager>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerFloatWindowSelectedHeaderBackgroundChangedCommandBehavior : CommandBehaviorBase<DockingManager>
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
            TargetObject.FloatWindowSelectedHeaderBackgroundChanged += OnEventRaised;
        }
    }

    // DockingManagerFloatWindowSelectedHeaderBackgroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerFloatWindowSelectedHeaderBackgroundChangedCommandBehavior<T> : DockingManagerFloatWindowSelectedHeaderBackgroundChangedCommandBehavior
    { }
    #endregion

    #region DockingManagerFloatWindowMouseOverHeaderBackgroundChangedCommand
    // DockingManagerFloatWindowMouseOverHeaderBackgroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerFloatWindowMouseOverHeaderBackgroundChangedCommand : ControlCommandBase<DockingManagerFloatWindowMouseOverHeaderBackgroundChangedCommandBehavior, DockingManager>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerFloatWindowMouseOverHeaderBackgroundChangedCommandBehavior : CommandBehaviorBase<DockingManager>
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
            TargetObject.FloatWindowMouseOverHeaderBackgroundChanged += OnEventRaised;
        }
    }

    // DockingManagerFloatWindowMouseOverHeaderBackgroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerFloatWindowMouseOverHeaderBackgroundChangedCommandBehavior<T> : DockingManagerFloatWindowMouseOverHeaderBackgroundChangedCommandBehavior
    { }
    #endregion

    #region DockingManagerFloatWindowHeaderForegroundChangedCommand
    // DockingManagerFloatWindowHeaderForegroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerFloatWindowHeaderForegroundChangedCommand : ControlCommandBase<DockingManagerFloatWindowHeaderForegroundChangedCommandBehavior, DockingManager>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerFloatWindowHeaderForegroundChangedCommandBehavior : CommandBehaviorBase<DockingManager>
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
            TargetObject.FloatWindowHeaderForegroundChanged += OnEventRaised;
        }
    }

    // DockingManagerFloatWindowHeaderForegroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerFloatWindowHeaderForegroundChangedCommandBehavior<T> : DockingManagerFloatWindowHeaderForegroundChangedCommandBehavior
    { }
    #endregion

    #region DockingManagerFloatWindowSelectedHeaderForegroundChangedCommand
    // DockingManagerFloatWindowSelectedHeaderForegroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerFloatWindowSelectedHeaderForegroundChangedCommand : ControlCommandBase<DockingManagerFloatWindowSelectedHeaderForegroundChangedCommandBehavior, DockingManager>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerFloatWindowSelectedHeaderForegroundChangedCommandBehavior : CommandBehaviorBase<DockingManager>
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
            TargetObject.FloatWindowSelectedHeaderForegroundChanged += OnEventRaised;
        }
    }

    // DockingManagerFloatWindowSelectedHeaderForegroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerFloatWindowSelectedHeaderForegroundChangedCommandBehavior<T> : DockingManagerFloatWindowSelectedHeaderForegroundChangedCommandBehavior
    { }
    #endregion

    #region DockingManagerFloatWindowMouseOverHeaderForegroundChangedCommand
    // DockingManagerFloatWindowMouseOverHeaderForegroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerFloatWindowMouseOverHeaderForegroundChangedCommand : ControlCommandBase<DockingManagerFloatWindowMouseOverHeaderForegroundChangedCommandBehavior, DockingManager>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerFloatWindowMouseOverHeaderForegroundChangedCommandBehavior : CommandBehaviorBase<DockingManager>
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
            TargetObject.FloatWindowMouseOverHeaderForegroundChanged += OnEventRaised;
        }
    }

    // DockingManagerFloatWindowMouseOverHeaderForegroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerFloatWindowMouseOverHeaderForegroundChangedCommandBehavior<T> : DockingManagerFloatWindowMouseOverHeaderForegroundChangedCommandBehavior
    { }
    #endregion

    #region DockingManagerSplitterBackgroundChangedCommand
    // DockingManagerSplitterBackgroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerSplitterBackgroundChangedCommand : ControlCommandBase<DockingManagerSplitterBackgroundChangedCommandBehavior, DockingManager>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerSplitterBackgroundChangedCommandBehavior : CommandBehaviorBase<DockingManager>
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
            TargetObject.SplitterBackgroundChanged += OnEventRaised;
        }
    }

    // DockingManagerSplitterBackgroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerSplitterBackgroundChangedCommandBehavior<T> : DockingManagerSplitterBackgroundChangedCommandBehavior
    { }
    #endregion

    #region DockingManagerSplitterSizeChangedCommand
    // DockingManagerSplitterSizeChangedCommand
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerSplitterSizeChangedCommand : ControlCommandBase<DockingManagerSplitterSizeChangedCommandBehavior, DockingManager>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerSplitterSizeChangedCommandBehavior : CommandBehaviorBase<DockingManager>
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
            TargetObject.SplitterSizeChanged += OnEventRaised;
        }
    }

    // DockingManagerSplitterSizeChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerSplitterSizeChangedCommandBehavior<T> : DockingManagerSplitterSizeChangedCommandBehavior
    { }
    #endregion

    #region DockingManagerTabItemsCornerRadiusChangedCommand
    // DockingManagerTabItemsCornerRadiusChangedCommand
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerTabItemsCornerRadiusChangedCommand : ControlCommandBase<DockingManagerTabItemsCornerRadiusChangedCommandBehavior, DockingManager>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerTabItemsCornerRadiusChangedCommandBehavior : CommandBehaviorBase<DockingManager>
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
            TargetObject.TabItemsCornerRadiusChanged += OnEventRaised;
        }
    }

    // DockingManagerTabItemsCornerRadiusChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerTabItemsCornerRadiusChangedCommandBehavior<T> : DockingManagerTabItemsCornerRadiusChangedCommandBehavior
    { }
    #endregion

    #region DockingManagerSidePanelBackgroundChangedCommand
    // DockingManagerSidePanelBackgroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerSidePanelBackgroundChangedCommand : ControlCommandBase<DockingManagerSidePanelBackgroundChangedCommandBehavior, DockingManager>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerSidePanelBackgroundChangedCommandBehavior : CommandBehaviorBase<DockingManager>
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
            TargetObject.SidePanelBackgroundChanged += OnEventRaised;
        }
    }

    // DockingManagerSidePanelBackgroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerSidePanelBackgroundChangedCommandBehavior<T> : DockingManagerSidePanelBackgroundChangedCommandBehavior
    { }
    #endregion

    #region DockingManagerTabPanelBackgroundChangedCommand
    // DockingManagerTabPanelBackgroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerTabPanelBackgroundChangedCommand : ControlCommandBase<DockingManagerTabPanelBackgroundChangedCommandBehavior, DockingManager>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerTabPanelBackgroundChangedCommandBehavior : CommandBehaviorBase<DockingManager>
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

    // DockingManagerTabPanelBackgroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerTabPanelBackgroundChangedCommandBehavior<T> : DockingManagerTabPanelBackgroundChangedCommandBehavior
    { }
    #endregion

    #region DockingManagerTabPanelBorderBrushChangedCommand
    // DockingManagerTabPanelBorderBrushChangedCommand
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerTabPanelBorderBrushChangedCommand : ControlCommandBase<DockingManagerTabPanelBorderBrushChangedCommandBehavior, DockingManager>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerTabPanelBorderBrushChangedCommandBehavior : CommandBehaviorBase<DockingManager>
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
            TargetObject.TabPanelBorderBrushChanged += OnEventRaised;
        }
    }

    // DockingManagerTabPanelBorderBrushChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerTabPanelBorderBrushChangedCommandBehavior<T> : DockingManagerTabPanelBorderBrushChangedCommandBehavior
    { }
    #endregion

    #region DockingManagerSidePanelBorderBrushChangedCommand
    // DockingManagerSidePanelBorderBrushChangedCommand
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerSidePanelBorderBrushChangedCommand : ControlCommandBase<DockingManagerSidePanelBorderBrushChangedCommandBehavior, DockingManager>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerSidePanelBorderBrushChangedCommandBehavior : CommandBehaviorBase<DockingManager>
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
            TargetObject.SidePanelBorderBrushChanged += OnEventRaised;
        }
    }

    // DockingManagerSidePanelBorderBrushChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerSidePanelBorderBrushChangedCommandBehavior<T> : DockingManagerSidePanelBorderBrushChangedCommandBehavior
    { }
    #endregion

    #region DockingManagerSidePanelBorderThicknessChangedCommand
    // DockingManagerSidePanelBorderThicknessChangedCommand
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerSidePanelBorderThicknessChangedCommand : ControlCommandBase<DockingManagerSidePanelBorderThicknessChangedCommandBehavior, DockingManager>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerSidePanelBorderThicknessChangedCommandBehavior : CommandBehaviorBase<DockingManager>
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
            TargetObject.SidePanelBorderThicknessChanged += OnEventRaised;
        }
    }

    // DockingManagerSidePanelBorderThicknessChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerSidePanelBorderThicknessChangedCommandBehavior<T> : DockingManagerSidePanelBorderThicknessChangedCommandBehavior
    { }
    #endregion

    #region DockingManagerTabPanelBorderThicknessChangedCommand
    // DockingManagerTabPanelBorderThicknessChangedCommand
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerTabPanelBorderThicknessChangedCommand : ControlCommandBase<DockingManagerTabPanelBorderThicknessChangedCommandBehavior, DockingManager>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerTabPanelBorderThicknessChangedCommandBehavior : CommandBehaviorBase<DockingManager>
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
            TargetObject.TabPanelBorderThicknessChanged += OnEventRaised;
        }
    }

    // DockingManagerTabPanelBorderThicknessChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerTabPanelBorderThicknessChangedCommandBehavior<T> : DockingManagerTabPanelBorderThicknessChangedCommandBehavior
    { }
    #endregion

    #region DockingManagerTabItemBorderThicknessChangedCommand
    // DockingManagerTabItemBorderThicknessChangedCommand
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerTabItemBorderThicknessChangedCommand : ControlCommandBase<DockingManagerTabItemBorderThicknessChangedCommandBehavior, DockingManager>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerTabItemBorderThicknessChangedCommandBehavior : CommandBehaviorBase<DockingManager>
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
            TargetObject.TabItemBorderThicknessChanged += OnEventRaised;
        }
    }

    // DockingManagerTabItemBorderThicknessChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerTabItemBorderThicknessChangedCommandBehavior<T> : DockingManagerTabItemBorderThicknessChangedCommandBehavior
    { }
    #endregion

    #region DockingManagerTabItemsBorderThicknessSelectedChangedCommand
    // DockingManagerTabItemsBorderThicknessSelectedChangedCommand
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerTabItemsBorderThicknessSelectedChangedCommand : ControlCommandBase<DockingManagerTabItemsBorderThicknessSelectedChangedCommandBehavior, DockingManager>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerTabItemsBorderThicknessSelectedChangedCommandBehavior : CommandBehaviorBase<DockingManager>
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
            TargetObject.TabItemsBorderThicknessSelectedChanged += OnEventRaised;
        }
    }

    // DockingManagerTabItemsBorderThicknessSelectedChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerTabItemsBorderThicknessSelectedChangedCommandBehavior<T> : DockingManagerTabItemsBorderThicknessSelectedChangedCommandBehavior
    { }
    #endregion

    #region DockingManagerPersistStateChangedCommand
    // DockingManagerPersistStateChangedCommand
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerPersistStateChangedCommand : ControlCommandBase<DockingManagerPersistStateChangedCommandBehavior, DockingManager>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerPersistStateChangedCommandBehavior : CommandBehaviorBase<DockingManager>
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

    // DockingManagerPersistStateChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// 
    public class DockingManagerPersistStateChangedCommandBehavior<T> : DockingManagerPersistStateChangedCommandBehavior
    { }
    #endregion

    #region DockingManagerActiveWindowChangedCommand
    // DockingManagerActiveWindowChangedCommand
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerActiveWindowChangedCommand : ControlCommandBase<DockingManagerActiveWindowChangedCommandBehavior, DockingManager>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerActiveWindowChangedCommandBehavior : CommandBehaviorBase<DockingManager>
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
            TargetObject.ActiveWindowChanged += OnEventRaised;
        }
    }

    // DockingManagerActiveWindowChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerActiveWindowChangedCommandBehavior<T> : DockingManagerActiveWindowChangedCommandBehavior
    { }
    #endregion

    #region DockingManagerIsActiveWindowChangedCommand
    // DockingManagerIsActiveWindowChangedCommand
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerIsActiveWindowChangedCommand : ControlCommandBase<DockingManagerIsActiveWindowChangedCommandBehavior, DockingManager>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerIsActiveWindowChangedCommandBehavior : CommandBehaviorBase<DockingManager>
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
            TargetObject.IsActiveWindowChanged += OnEventRaised;
        }
    }

    // DockingManagerIsActiveWindowChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerIsActiveWindowChangedCommandBehavior<T> : DockingManagerIsActiveWindowChangedCommandBehavior
    { }
    #endregion

    #region DockingManagerIsSelectedTabChangedCommand
    // DockingManagerIsSelectedTabChangedCommand
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerIsSelectedTabChangedCommand : ControlCommandBase<DockingManagerIsSelectedTabChangedCommandBehavior, DockingManager>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerIsSelectedTabChangedCommandBehavior : CommandBehaviorBase<DockingManager>
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
            TargetObject.IsSelectedTabChanged += OnEventRaised;
        }
    }

    // DockingManagerIsSelectedTabChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerIsSelectedTabChangedCommandBehavior<T> : DockingManagerIsSelectedTabChangedCommandBehavior
    { }
    #endregion

    #region DockingManagerTargetNameInDockedModeChangedCommand
    // DockingManagerTargetNameInDockedModeChangedCommand
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerTargetNameInDockedModeChangedCommand : ControlCommandBase<DockingManagerTargetNameInDockedModeChangedCommandBehavior, DockingManager>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerTargetNameInDockedModeChangedCommandBehavior : CommandBehaviorBase<DockingManager>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, DockTargetNameChangedEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.TargetNameInDockedModeChanged += OnEventRaised;
        }
    }

    // DockingManagerTargetNameInDockedModeChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerTargetNameInDockedModeChangedCommandBehavior<T> : DockingManagerTargetNameInDockedModeChangedCommandBehavior
    { }
    #endregion

    #region DockingManagerTargetNameInFloatingModeChangedCommand
    // DockingManagerTargetNameInFloatingModeChangedCommand
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerTargetNameInFloatingModeChangedCommand : ControlCommandBase<DockingManagerTargetNameInFloatingModeChangedCommandBehavior, DockingManager>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerTargetNameInFloatingModeChangedCommandBehavior : CommandBehaviorBase<DockingManager>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, DockTargetNameChangedEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.TargetNameInFloatingModeChanged += OnEventRaised;
        }
    }

    // DockingManagerTargetNameInFloatingModeChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerTargetNameInFloatingModeChangedCommandBehavior<T> : DockingManagerTargetNameInFloatingModeChangedCommandBehavior
    { }
    #endregion

    #region DockingManagerDockStateChangedCommand
    // DockingManagerDockStateChangedCommand
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerDockStateChangedCommand : ControlCommandBase<DockingManagerDockStateChangedCommandBehavior, DockingManager>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerDockStateChangedCommandBehavior : CommandBehaviorBase<DockingManager>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, DockStateEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.DockStateChanged += OnEventRaised;
        }
    }

    // DockingManagerDockStateChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerDockStateChangedCommandBehavior<T> : DockingManagerDockStateChangedCommandBehavior
    { }
    #endregion

    #region DockingManagerDockWindowStateChangedCommand
    // DockingManagerDockWindowStateChangedCommand
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerDockWindowStateChangedCommand : ControlCommandBase<DockingManagerDockWindowStateChangedCommandBehavior, DockingManager>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerDockWindowStateChangedCommandBehavior : CommandBehaviorBase<DockingManager>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, DockWindowStateEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.DockWindowStateChanged += OnEventRaised;
        }
    }

    // DockingManagerDockWindowStateChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerDockWindowStateChangedCommandBehavior<T> : DockingManagerDockWindowStateChangedCommandBehavior
    { }
    #endregion

    #region DockingManagerDockStateChangingCommand
    // DockingManagerDockStateChangingCommand
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerDockStateChangingCommand : ControlCommandBase<DockingManagerDockStateChangingCommandBehavior, DockingManager>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerDockStateChangingCommandBehavior : CommandBehaviorBase<DockingManager>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, DockStateChangingEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.DockStateChanging += OnEventRaised;
        }
    }

    // DockingManagerDockStateChangingCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerDockStateChangingCommandBehavior<T> : DockingManagerDockStateChangingCommandBehavior
    { }
    #endregion

    #region DockingManagerWindowResizingCommand
    // DockingManagerWindowResizingCommand
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerWindowResizingCommand : ControlCommandBase<DockingManagerWindowResizingCommandBehavior, DockingManager>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerWindowResizingCommandBehavior : CommandBehaviorBase<DockingManager>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, WindowResizingEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.WindowResizing += OnEventRaised;
        }
    }

    // DockingManagerWindowResizingCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerWindowResizingCommandBehavior<T> : DockingManagerWindowResizingCommandBehavior
    { }
    #endregion

    #region DockingManagerWindowMovingCommand
    // DockingManagerWindowMovingCommand
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerWindowMovingCommand : ControlCommandBase<DockingManagerWindowMovingCommandBehavior, DockingManager>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerWindowMovingCommandBehavior : CommandBehaviorBase<DockingManager>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, WindowMovingEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.WindowMoving += OnEventRaised;
        }
    }

    // DockingManagerWindowMovingCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerWindowMovingCommandBehavior<T> : DockingManagerWindowMovingCommandBehavior
    { }
    #endregion

    #region DockingManagerWindowClosingCommand
    // DockingManagerWindowClosingCommand
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerWindowClosingCommand : ControlCommandBase<DockingManagerWindowClosingCommandBehavior, DockingManager>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerWindowClosingCommandBehavior : CommandBehaviorBase<DockingManager>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, WindowClosingEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.WindowClosing += OnEventRaised;
        }
    }

    // DockingManagerWindowClosingCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerWindowClosingCommandBehavior<T> : DockingManagerWindowClosingCommandBehavior
    { }
    #endregion

    #region DockingManagerTransferredFromManagerCommand
    // DockingManagerTransferredFromManagerCommand
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerTransferredFromManagerCommand : ControlCommandBase<DockingManagerTransferredFromManagerCommandBehavior, DockingManager>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerTransferredFromManagerCommandBehavior : CommandBehaviorBase<DockingManager>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, TransferManagerEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.TransferredFromManager += OnEventRaised;
        }
    }

    // DockingManagerTransferredFromManagerCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerTransferredFromManagerCommandBehavior<T> : DockingManagerTransferredFromManagerCommandBehavior
    { }
    #endregion

    #region DockingManagerTransferredToManagerCommand
    // DockingManagerTransferredToManagerCommand
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerTransferredToManagerCommand : ControlCommandBase<DockingManagerTransferredToManagerCommandBehavior, DockingManager>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerTransferredToManagerCommandBehavior : CommandBehaviorBase<DockingManager>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, TransferManagerEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.TransferredToManager += OnEventRaised;
        }
    }

    // DockingManagerTransferredToManagerCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerTransferredToManagerCommandBehavior<T> : DockingManagerTransferredToManagerCommandBehavior
    { }
    #endregion

    #region DockingManagerActiveWindowChangingCommand
    // DockingManagerActiveWindowChangingCommand
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerActiveWindowChangingCommand : ControlCommandBase<DockingManagerActiveWindowChangingCommandBehavior, DockingManager>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerActiveWindowChangingCommandBehavior : CommandBehaviorBase<DockingManager>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, ActiveWindowChangingEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.ActiveWindowChanging += OnEventRaised;
        }
    }

    // DockingManagerActiveWindowChangingCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerActiveWindowChangingCommandBehavior<T> : DockingManagerActiveWindowChangingCommandBehavior
    { }
	#endregion

    #region DockingManagerElementHiddenCommand
    // DockingManagerElementHiddenCommand
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerElementHiddenCommand : ControlCommandBase<DockingManagerElementHiddenCommandBehavior, DockingManager>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerElementHiddenCommandBehavior : CommandBehaviorBase<DockingManager>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        protected virtual void OnEventRaised(object sender)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.ElementHidden += OnEventRaised;
        }
    }

    // DockingManagerElementHiddenCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerElementHiddenCommandBehavior<T> : DockingManagerElementHiddenCommandBehavior
    { }
    #endregion

    #region DockingManagerElementShownCommand
    // DockingManagerElementShownCommand
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerElementShownCommand : ControlCommandBase<DockingManagerElementShownCommandBehavior, DockingManager>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerElementShownCommandBehavior : CommandBehaviorBase<DockingManager>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        protected virtual void OnEventRaised(object sender)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.ElementShown += OnEventRaised;
        }
    }

    // DockingManagerElementShownCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerElementShownCommandBehavior<T> : DockingManagerElementShownCommandBehavior
    { }
    #endregion


	#region DockingManagerDockProviderShownCommand
	// DockingManagerDockProviderShownCommand
    /// <summary>
    /// 
    /// </summary>
	public class DockingManagerDockProviderShownCommand : ControlCommandBase<DockingManagerDockProviderShownCommandBehavior, DockingManager>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerDockProviderShownCommandBehavior : CommandBehaviorBase<DockingManager>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, DockProviderShownEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.DockProviderShown += OnEventRaised;
        }
    }

	// DockingManagerDockProviderShownCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerDockProviderShownCommandBehavior<T> : DockingManagerDockProviderShownCommandBehavior
    { }
	#endregion

	#region DockingManagerTabClosedCommand
	// DockingManagerTabClosedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DockingManagerTabClosedCommand : ControlCommandBase<DockingManagerTabClosedCommandBehavior, DockingManager>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerTabClosedCommandBehavior : CommandBehaviorBase<DockingManager>
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

	// DockingManagerTabClosedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerTabClosedCommandBehavior<T> : DockingManagerTabClosedCommandBehavior
    { }
	#endregion

	#region DockingManagerAutoHideAnimationStartCommand
	// DockingManagerAutoHideAnimationStartCommand
    /// <summary>
    /// 
    /// </summary>
	public class DockingManagerAutoHideAnimationStartCommand : ControlCommandBase<DockingManagerAutoHideAnimationStartCommandBehavior, DockingManager>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerAutoHideAnimationStartCommandBehavior : CommandBehaviorBase<DockingManager>
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
            TargetObject.AutoHideAnimationStart += OnEventRaised;
        }
    }

	// DockingManagerAutoHideAnimationStartCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerAutoHideAnimationStartCommandBehavior<T> : DockingManagerAutoHideAnimationStartCommandBehavior
    { }
	#endregion

	#region DockingManagerAutoHideAnimationStopCommand
	// DockingManagerAutoHideAnimationStopCommand
    /// <summary>
    /// 
    /// </summary>
	public class DockingManagerAutoHideAnimationStopCommand : ControlCommandBase<DockingManagerAutoHideAnimationStopCommandBehavior, DockingManager>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerAutoHideAnimationStopCommandBehavior : CommandBehaviorBase<DockingManager>
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
            TargetObject.AutoHideAnimationStop += OnEventRaised;
        }
    }

	// DockingManagerAutoHideAnimationStopCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerAutoHideAnimationStopCommandBehavior<T> : DockingManagerAutoHideAnimationStopCommandBehavior
    { }
	#endregion

	#region DockingManagerWindowActivatedCommand
	// DockingManagerWindowActivatedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DockingManagerWindowActivatedCommand : ControlCommandBase<DockingManagerWindowActivatedCommandBehavior, DockingManager>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerWindowActivatedCommandBehavior : CommandBehaviorBase<DockingManager>
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
            TargetObject.WindowActivated += OnEventRaised;
        }
    }

	// DockingManagerWindowActivatedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerWindowActivatedCommandBehavior<T> : DockingManagerWindowActivatedCommandBehavior
    { }
	#endregion

	#region DockingManagerWindowDeactivatedCommand
	// DockingManagerWindowDeactivatedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DockingManagerWindowDeactivatedCommand : ControlCommandBase<DockingManagerWindowDeactivatedCommandBehavior, DockingManager>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerWindowDeactivatedCommandBehavior : CommandBehaviorBase<DockingManager>
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
            TargetObject.WindowDeactivated += OnEventRaised;
        }
    }

	// DockingManagerWindowDeactivatedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerWindowDeactivatedCommandBehavior<T> : DockingManagerWindowDeactivatedCommandBehavior
    { }
	#endregion

	#region DockingManagerContextMenuItemClickCommand
	// DockingManagerContextMenuItemClickCommand
    /// <summary>
    /// 
    /// </summary>
	public class DockingManagerContextMenuItemClickCommand : ControlCommandBase<DockingManagerContextMenuItemClickCommandBehavior, DockingManager>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerContextMenuItemClickCommandBehavior : CommandBehaviorBase<DockingManager>
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
            TargetObject.ContextMenuItemClick += OnEventRaised;
        }
    }

	// DockingManagerContextMenuItemClickCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerContextMenuItemClickCommandBehavior<T> : DockingManagerContextMenuItemClickCommandBehavior
    { }
	#endregion

	#region DockingManagerWindowVisibilityChangedCommand
	// DockingManagerWindowVisibilityChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DockingManagerWindowVisibilityChangedCommand : ControlCommandBase<DockingManagerWindowVisibilityChangedCommandBehavior, DockingManager>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerWindowVisibilityChangedCommandBehavior : CommandBehaviorBase<DockingManager>
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
            TargetObject.WindowVisibilityChanged += OnEventRaised;
        }
    }

	// DockingManagerWindowVisibilityChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerWindowVisibilityChangedCommandBehavior<T> : DockingManagerWindowVisibilityChangedCommandBehavior
    { }
	#endregion

	#region DockingManagerWindowDragStartCommand
	// DockingManagerWindowDragStartCommand
    /// <summary>
    /// 
    /// </summary>
	public class DockingManagerWindowDragStartCommand : ControlCommandBase<DockingManagerWindowDragStartCommandBehavior, DockingManager>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerWindowDragStartCommandBehavior : CommandBehaviorBase<DockingManager>
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
            TargetObject.WindowDragStart += OnEventRaised;
        }
    }

	// DockingManagerWindowDragStartCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerWindowDragStartCommandBehavior<T> : DockingManagerWindowDragStartCommandBehavior
    { }
	#endregion

	#region DockingManagerWindowDragEndCommand
	// DockingManagerWindowDragEndCommand
    /// <summary>
    /// 
    /// </summary>
	public class DockingManagerWindowDragEndCommand : ControlCommandBase<DockingManagerWindowDragEndCommandBehavior, DockingManager>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerWindowDragEndCommandBehavior : CommandBehaviorBase<DockingManager>
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
            TargetObject.WindowDragEnd += OnEventRaised;
        }
    }

	// DockingManagerWindowDragEndCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerWindowDragEndCommandBehavior<T> : DockingManagerWindowDragEndCommandBehavior
    { }
	#endregion

	#region DockingManagerBeforeContextMenuOpenCommand
	// DockingManagerBeforeContextMenuOpenCommand
    /// <summary>
    /// 
    /// </summary>
	public class DockingManagerBeforeContextMenuOpenCommand : ControlCommandBase<DockingManagerBeforeContextMenuOpenCommandBehavior, DockingManager>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerBeforeContextMenuOpenCommandBehavior : CommandBehaviorBase<DockingManager>
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
            TargetObject.BeforeContextMenuOpen += OnEventRaised;
        }
    }

	// DockingManagerBeforeContextMenuOpenCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerBeforeContextMenuOpenCommandBehavior<T> : DockingManagerBeforeContextMenuOpenCommandBehavior
    { }
	#endregion

	#region DockingManagerDockMenuClickCommand
	// DockingManagerDockMenuClickCommand
    /// <summary>
    /// 
    /// </summary>
	public class DockingManagerDockMenuClickCommand : ControlCommandBase<DockingManagerDockMenuClickCommandBehavior, DockingManager>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerDockMenuClickCommandBehavior : CommandBehaviorBase<DockingManager>
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
            TargetObject.DockMenuClick += OnEventRaised;
        }
    }

	// DockingManagerDockMenuClickCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerDockMenuClickCommandBehavior<T> : DockingManagerDockMenuClickCommandBehavior
    { }
	#endregion

	#region DockingManagerIsFrozeChangedCommand
	// DockingManagerIsFrozeChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DockingManagerIsFrozeChangedCommand : ControlCommandBase<DockingManagerIsFrozeChangedCommandBehavior, DockingManager>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerIsFrozeChangedCommandBehavior : CommandBehaviorBase<DockingManager>
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
            TargetObject.IsFrozeChanged += OnEventRaised;
        }
    }

	// DockingManagerIsFrozeChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerIsFrozeChangedCommandBehavior<T> : DockingManagerIsFrozeChangedCommandBehavior
    { }
	#endregion

	#region DockingManagerTunnelActiveWindowChangedCommand
	// DockingManagerTunnelActiveWindowChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class DockingManagerTunnelActiveWindowChangedCommand : ControlCommandBase<DockingManagerTunnelActiveWindowChangedCommandBehavior, DockingManager>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerTunnelActiveWindowChangedCommandBehavior : CommandBehaviorBase<DockingManager>
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
            TargetObject.TunnelActiveWindowChanged += OnEventRaised;
        }
    }

	// DockingManagerTunnelActiveWindowChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerTunnelActiveWindowChangedCommandBehavior<T> : DockingManagerTunnelActiveWindowChangedCommandBehavior
    { }
	#endregion
}
