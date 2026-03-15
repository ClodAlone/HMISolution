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

    #region WindowControlActivatedCommand
    /// <summary>
    /// WindowControlActivatedCommand
    /// </summary>
    public class WindowControlActivatedCommand : ControlCommandBase<WindowControlActivatedCommandBehavior, WindowControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class WindowControlActivatedCommandBehavior : CommandBehaviorBase<WindowControl>
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
            TargetObject.Activated += OnEventRaised;
        }
    }

    /// <summary>
    /// WindowControlActivatedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WindowControlActivatedCommandBehavior<T> : WindowControlActivatedCommandBehavior
    { }
    #endregion

    #region WindowControlDeactivatedCommand
    /// <summary>
    /// WindowControlDeactivatedCommand
    /// </summary>
    public class WindowControlDeactivatedCommand : ControlCommandBase<WindowControlDeactivatedCommandBehavior, WindowControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class WindowControlDeactivatedCommandBehavior : CommandBehaviorBase<WindowControl>
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
            TargetObject.Deactivated += OnEventRaised;
        }
    }

    /// <summary>
    /// WindowControlDeactivatedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WindowControlDeactivatedCommandBehavior<T> : WindowControlDeactivatedCommandBehavior
    { }
    #endregion

    #region WindowControlClosedCommand
    /// <summary>
    /// WindowControlClosedCommand
    /// </summary>
    public class WindowControlClosedCommand : ControlCommandBase<WindowControlClosedCommandBehavior, WindowControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class WindowControlClosedCommandBehavior : CommandBehaviorBase<WindowControl>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, ClosedEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.Closed += OnEventRaised;
        }
    }

    /// <summary>
    /// WindowControlClosedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WindowControlClosedCommandBehavior<T> : WindowControlClosedCommandBehavior
    { }
    #endregion

    #region WindowControlClosingCommand
    /// <summary>
    /// WindowControlClosingCommand
    /// </summary>
    public class WindowControlClosingCommand : ControlCommandBase<WindowControlClosingCommandBehavior, WindowControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class WindowControlClosingCommandBehavior : CommandBehaviorBase<WindowControl>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, ClosedEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.Closing += OnEventRaised;
        }
    }

    /// <summary>
    /// WindowControlClosingCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WindowControlClosingCommandBehavior<T> : WindowControlClosingCommandBehavior
    { }
    #endregion

    #region WindowControlOpenedCommand
    /// <summary>
    /// WindowControlOpenedCommand
    /// </summary>
    public class WindowControlOpenedCommand : ControlCommandBase<WindowControlOpenedCommandBehavior, WindowControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class WindowControlOpenedCommandBehavior : CommandBehaviorBase<WindowControl>
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
            TargetObject.Opened += OnEventRaised;
        }
    }

    /// <summary>
    /// WindowControlOpenedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WindowControlOpenedCommandBehavior<T> : WindowControlOpenedCommandBehavior
    { }
    #endregion

    #region WindowControlOpeningCommand
    /// <summary>
    /// WindowControlOpeningCommand
    /// </summary>
    public class WindowControlOpeningCommand : ControlCommandBase<WindowControlOpeningCommandBehavior, WindowControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class WindowControlOpeningCommandBehavior : CommandBehaviorBase<WindowControl>
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
            TargetObject.Opening += OnEventRaised;
        }
    }

    /// <summary>
    /// WindowControlOpeningCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WindowControlOpeningCommandBehavior<T> : WindowControlOpeningCommandBehavior
    { }
    #endregion

    #region WindowControlSystemMenuOpenedCommand
    /// <summary>
    /// WindowControlSystemMenuOpenedCommand
    /// </summary>
    public class WindowControlSystemMenuOpenedCommand : ControlCommandBase<WindowControlSystemMenuOpenedCommandBehavior, WindowControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class WindowControlSystemMenuOpenedCommandBehavior : CommandBehaviorBase<WindowControl>
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
            TargetObject.SystemMenuOpened += OnEventRaised;
        }
    }

    /// <summary>
    /// WindowControlSystemMenuOpenedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WindowControlSystemMenuOpenedCommandBehavior<T> : WindowControlSystemMenuOpenedCommandBehavior
    { }
    #endregion

    #region WindowControlSystemMenuOpeningCommand
    /// <summary>
    /// WindowControlSystemMenuOpeningCommand
    /// </summary>
    public class WindowControlSystemMenuOpeningCommand : ControlCommandBase<WindowControlSystemMenuOpeningCommandBehavior, WindowControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class WindowControlSystemMenuOpeningCommandBehavior : CommandBehaviorBase<WindowControl>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, SystemMenuOpeningEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.SystemMenuOpening += OnEventRaised;
        }
    }

    /// <summary>
    /// WindowControlSystemMenuOpeningCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WindowControlSystemMenuOpeningCommandBehavior<T> : WindowControlSystemMenuOpeningCommandBehavior
    { }
    #endregion

    #region WindowControlOnWindowStateChangingCommand
    /// <summary>
    /// WindowControlOnWindowStateChangingCommand
    /// </summary>
    public class WindowControlOnWindowStateChangingCommand : ControlCommandBase<WindowControlOnWindowStateChangingCommandBehavior, WindowControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class WindowControlOnWindowStateChangingCommandBehavior : CommandBehaviorBase<WindowControl>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, WindowStateChangingEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.OnWindowStateChanging += OnEventRaised;
        }
    }

    /// <summary>
    /// WindowControlOnWindowStateChangingCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WindowControlOnWindowStateChangingCommandBehavior<T> : WindowControlOnWindowStateChangingCommandBehavior
    { }
    #endregion

    #region WindowControlOnWindowStateChangedCommand
    /// <summary>
    /// WindowControlOnWindowStateChangedCommand
    /// </summary>
    public class WindowControlOnWindowStateChangedCommand : ControlCommandBase<WindowControlOnWindowStateChangedCommandBehavior, WindowControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class WindowControlOnWindowStateChangedCommandBehavior : CommandBehaviorBase<WindowControl>
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
            TargetObject.OnWindowStateChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// WindowControlOnWindowStateChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WindowControlOnWindowStateChangedCommandBehavior<T> : WindowControlOnWindowStateChangedCommandBehavior
    { }
    #endregion

    #region WindowControlContextMenuOpenedCommand
    /// <summary>
    /// WindowControlContextMenuOpenedCommand
    /// </summary>
    public class WindowControlContextMenuOpenedCommand : ControlCommandBase<WindowControlContextMenuOpenedCommandBehavior, WindowControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class WindowControlContextMenuOpenedCommandBehavior : CommandBehaviorBase<WindowControl>
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
            TargetObject.ContextMenuOpened += OnEventRaised;
        }
    }

    /// <summary>
    /// WindowControlContextMenuOpenedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WindowControlContextMenuOpenedCommandBehavior<T> : WindowControlContextMenuOpenedCommandBehavior
    { }
    #endregion

    #region WindowControlContextMenuClosedCommand
    /// <summary>
    /// WindowControlContextMenuClosedCommand
    /// </summary>
    public class WindowControlContextMenuClosedCommand : ControlCommandBase<WindowControlContextMenuClosedCommandBehavior, WindowControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class WindowControlContextMenuClosedCommandBehavior : CommandBehaviorBase<WindowControl>
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
            TargetObject.ContextMenuClosed += OnEventRaised;
        }
    }

    /// <summary>
    /// WindowControlContextMenuClosedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WindowControlContextMenuClosedCommandBehavior<T> : WindowControlContextMenuClosedCommandBehavior
    { }
    #endregion

    #region WindowControlMouseDoubleClickCommand
    /// <summary>
    /// WindowControlMouseDoubleClickCommand
    /// </summary>
    public class WindowControlMouseDoubleClickCommand : ControlCommandBase<WindowControlMouseDoubleClickCommandBehavior, WindowControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class WindowControlMouseDoubleClickCommandBehavior : CommandBehaviorBase<WindowControl>
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
            TargetObject.MouseDoubleClick += OnEventRaised;
        }
    }

    /// <summary>
    /// WindowControlMouseDoubleClickCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WindowControlMouseDoubleClickCommandBehavior<T> : WindowControlMouseDoubleClickCommandBehavior
    { }
    #endregion
}


