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
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.Windows.Shared;


namespace Syncfusion.Windows.Tools.MVVM
{

    #region DockingManagerDockingManager_loadedCommand
    /// <summary>
    /// DockingManagerDockingManager_loadedCommand
    /// </summary>
    public class DockingManagerDockingManager_loadedCommand : ControlCommandBase<DockingManagerDockingManager_loadedCommandBehavior, DockingManager>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerDockingManager_loadedCommandBehavior : CommandBehaviorBase<DockingManager>
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
            TargetObject.DockingManager_loaded += OnEventRaised;
        }
    }

    /// <summary>
    /// DockingManagerDockingManager_loadedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerDockingManager_loadedCommandBehavior<T> : DockingManagerDockingManager_loadedCommandBehavior
    { }
    #endregion

    #region DockingManagerCanClosePropertyChangedCommand
    /// <summary>
    /// DockingManagerCanClosePropertyChangedCommand
    /// </summary>
    public class DockingManagerCanClosePropertyChangedCommand : ControlCommandBase<DockingManagerCanClosePropertyChangedCommandBehavior, DockingManager>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerCanClosePropertyChangedCommandBehavior : CommandBehaviorBase<DockingManager>
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
            TargetObject.CanClosePropertyChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// DockingManagerCanClosePropertyChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerCanClosePropertyChangedCommandBehavior<T> : DockingManagerCanClosePropertyChangedCommandBehavior
    { }
    #endregion

    #region DockingManagerCanResizePropertyChangedCommand
    /// <summary>
    /// DockingManagerCanResizePropertyChangedCommand
    /// </summary>
    public class DockingManagerCanResizePropertyChangedCommand : ControlCommandBase<DockingManagerCanResizePropertyChangedCommandBehavior, DockingManager>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerCanResizePropertyChangedCommandBehavior : CommandBehaviorBase<DockingManager>
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
            TargetObject.CanResizePropertyChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// DockingManagerCanResizePropertyChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerCanResizePropertyChangedCommandBehavior<T> : DockingManagerCanResizePropertyChangedCommandBehavior
    { }
    #endregion

    #region DockingManagerActiveWindowChangedCommand
    /// <summary>
    /// DockingManagerActiveWindowChangedCommand
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

    /// <summary>
    /// DockingManagerActiveWindowChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerActiveWindowChangedCommandBehavior<T> : DockingManagerActiveWindowChangedCommandBehavior
    { }
    #endregion

    #region DockingManagerPreviewWindowClosedCommand
    /// <summary>
    /// DockingManagerPreviewWindowClosedCommand
    /// </summary>
    public class DockingManagerPreviewWindowClosedCommand : ControlCommandBase<DockingManagerPreviewWindowClosedCommandBehavior, DockingManager>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerPreviewWindowClosedCommandBehavior : CommandBehaviorBase<DockingManager>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, CloseEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.PreviewWindowClosed += OnEventRaised;
        }
    }

    /// <summary>
    /// DockingManagerPreviewWindowClosedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerPreviewWindowClosedCommandBehavior<T> : DockingManagerPreviewWindowClosedCommandBehavior
    { }
    #endregion

    #region DockingManagerWindowClosedCommand
    /// <summary>
    /// DockingManagerWindowClosedCommand
    /// </summary>
    public class DockingManagerWindowClosedCommand : ControlCommandBase<DockingManagerWindowClosedCommandBehavior, DockingManager>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerWindowClosedCommandBehavior : CommandBehaviorBase<DockingManager>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, WindowCloseEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.WindowClosed += OnEventRaised;
        }
    }

    /// <summary>
    /// DockingManagerWindowClosedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerWindowClosedCommandBehavior<T> : DockingManagerWindowClosedCommandBehavior
    { }
    #endregion

    #region DockingManagerPreviewWindowPinnedCommand
    /// <summary>
    /// DockingManagerPreviewWindowPinnedCommand
    /// </summary>
    public class DockingManagerPreviewWindowPinnedCommand : ControlCommandBase<DockingManagerPreviewWindowPinnedCommandBehavior, DockingManager>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerPreviewWindowPinnedCommandBehavior : CommandBehaviorBase<DockingManager>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, PreviewWindowPinnedEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.PreviewWindowPinned += OnEventRaised;
        }
    }

    /// <summary>
    /// DockingManagerPreviewWindowPinnedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerPreviewWindowPinnedCommandBehavior<T> : DockingManagerPreviewWindowPinnedCommandBehavior
    { }
    #endregion

    #region DockingManagerWindowPinnedCommand
    /// <summary>
    /// DockingManagerWindowPinnedCommand
    /// </summary>
    public class DockingManagerWindowPinnedCommand : ControlCommandBase<DockingManagerWindowPinnedCommandBehavior, DockingManager>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerWindowPinnedCommandBehavior : CommandBehaviorBase<DockingManager>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, WindowPinnedEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.WindowPinned += OnEventRaised;
        }
    }

    /// <summary>
    /// DockingManagerWindowPinnedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerWindowPinnedCommandBehavior<T> : DockingManagerWindowPinnedCommandBehavior
    { }
    #endregion

    #region DockingManagerPreviewWindowUnPinnedCommand
    /// <summary>
    /// DockingManagerPreviewWindowUnPinnedCommand
    /// </summary>
    public class DockingManagerPreviewWindowUnPinnedCommand : ControlCommandBase<DockingManagerPreviewWindowUnPinnedCommandBehavior, DockingManager>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerPreviewWindowUnPinnedCommandBehavior : CommandBehaviorBase<DockingManager>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, PreviewWindowUnPinnedEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.PreviewWindowUnPinned += OnEventRaised;
        }
    }

    /// <summary>
    /// DockingManagerPreviewWindowUnPinnedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerPreviewWindowUnPinnedCommandBehavior<T> : DockingManagerPreviewWindowUnPinnedCommandBehavior
    { }
    #endregion

    #region DockingManagerWindowUnPinnedCommand
    /// <summary>
    /// DockingManagerWindowUnPinnedCommand
    /// </summary>
    public class DockingManagerWindowUnPinnedCommand : ControlCommandBase<DockingManagerWindowUnPinnedCommandBehavior, DockingManager>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerWindowUnPinnedCommandBehavior : CommandBehaviorBase<DockingManager>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, WindowUnPinnedEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.WindowUnPinned += OnEventRaised;
        }
    }

    /// <summary>
    /// DockingManagerWindowUnPinnedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerWindowUnPinnedCommandBehavior<T> : DockingManagerWindowUnPinnedCommandBehavior
    { }
    #endregion

    #region DockingManagerDragAllowCommand
    /// <summary>
    /// DockingManagerDragAllowCommand
    /// </summary>
    public class DockingManagerDragAllowCommand : ControlCommandBase<DockingManagerDragAllowCommandBehavior, DockingManager>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerDragAllowCommandBehavior : CommandBehaviorBase<DockingManager>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, DragAllowEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.DragAllow += OnEventRaised;
        }
    }

    /// <summary>
    /// DockingManagerDragAllowCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerDragAllowCommandBehavior<T> : DockingManagerDragAllowCommandBehavior
    { }
    #endregion

    #region DockingManagerDockAllowCommand
    /// <summary>
    /// DockingManagerDockAllowCommand
    /// </summary>
    public class DockingManagerDockAllowCommand : ControlCommandBase<DockingManagerDockAllowCommandBehavior, DockingManager>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerDockAllowCommandBehavior : CommandBehaviorBase<DockingManager>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, DockAllowEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.DockAllow += OnEventRaised;
        }
    }

    /// <summary>
    /// DockingManagerDockAllowCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerDockAllowCommandBehavior<T> : DockingManagerDockAllowCommandBehavior
    { }
    #endregion

    #region DockingManagerDockStateChangedCommand
    /// <summary>
    /// DockingManagerDockStateChangedCommand
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
        protected virtual void OnEventRaised(object sender, DockStateChangedEventArgs e)
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

    /// <summary>
    /// DockingManagerDockStateChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerDockStateChangedCommandBehavior<T> : DockingManagerDockStateChangedCommandBehavior
    { }
    #endregion

    #region DockingManagerDockStateChangingCommand
    /// <summary>
    /// DockingManagerDockStateChangingCommand
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

    /// <summary>
    /// DockingManagerDockStateChangingCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerDockStateChangingCommandBehavior<T> : DockingManagerDockStateChangingCommandBehavior
    { }
    #endregion

    #region DockingManagerInitializeControlOnLoadCommand
    /// <summary>
    /// DockingManagerInitializeControlOnLoadCommand
    /// </summary>
    public class DockingManagerInitializeControlOnLoadCommand : ControlCommandBase<DockingManagerInitializeControlOnLoadCommandBehavior, DockingManager>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerInitializeControlOnLoadCommandBehavior : CommandBehaviorBase<DockingManager>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, InitializeControlOnLoadEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.InitializeControlOnLoad += OnEventRaised;
        }
    }

    /// <summary>
    /// DockingManagerInitializeControlOnLoadCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerInitializeControlOnLoadCommandBehavior<T> : DockingManagerInitializeControlOnLoadCommandBehavior
    { }
    #endregion

    #region DockingManagerDockContextMenuCommand
    /// <summary>
    /// DockingManagerDockContextMenuCommand
    /// </summary>
    public class DockingManagerDockContextMenuCommand : ControlCommandBase<DockingManagerDockContextMenuCommandBehavior, DockingManager>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerDockContextMenuCommandBehavior : CommandBehaviorBase<DockingManager>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, DockContextMenuEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.DockContextMenu += OnEventRaised;
        }
    }

    /// <summary>
    /// DockingManagerDockContextMenuCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerDockContextMenuCommandBehavior<T> : DockingManagerDockContextMenuCommandBehavior
    { }
    #endregion

    #region DockingManagerMaximizedCommand
    /// <summary>
    /// DockingManagerMaximizedCommand
    /// </summary>
    public class DockingManagerMaximizedCommand : ControlCommandBase<DockingManagerMaximizedCommandBehavior, DockingManager>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerMaximizedCommandBehavior : CommandBehaviorBase<DockingManager>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, MaximizedEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.Maximized += OnEventRaised;
        }
    }

    /// <summary>
    /// DockingManagerMaximizedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerMaximizedCommandBehavior<T> : DockingManagerMaximizedCommandBehavior
    { }
    #endregion

    #region DockingManagerMaximizingCommand
    /// <summary>
    /// DockingManagerMaximizingCommand
    /// </summary>
    public class DockingManagerMaximizingCommand : ControlCommandBase<DockingManagerMaximizingCommandBehavior, DockingManager>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerMaximizingCommandBehavior : CommandBehaviorBase<DockingManager>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, MaximizingEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.Maximizing += OnEventRaised;
        }
    }

    /// <summary>
    /// DockingManagerMaximizingCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerMaximizingCommandBehavior<T> : DockingManagerMaximizingCommandBehavior
    { }
    #endregion

    #region DockingManagerAutoHideTabContextMenuOpenCommand
    /// <summary>
    /// DockingManagerAutoHideTabContextMenuOpenCommand
    /// </summary>
    public class DockingManagerAutoHideTabContextMenuOpenCommand : ControlCommandBase<DockingManagerAutoHideTabContextMenuOpenCommandBehavior, DockingManager>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class DockingManagerAutoHideTabContextMenuOpenCommandBehavior : CommandBehaviorBase<DockingManager>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, AutoHideTabContextMenuOpenEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.AutoHideTabContextMenuOpen += OnEventRaised;
        }
    }

    /// <summary>
    /// DockingManagerAutoHideTabContextMenuOpenCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DockingManagerAutoHideTabContextMenuOpenCommandBehavior<T> : DockingManagerAutoHideTabContextMenuOpenCommandBehavior
    { }
    #endregion
}



