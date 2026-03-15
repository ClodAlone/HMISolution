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

    #region ContextMenuAdvOpenedCommand
    /// <summary>
    /// ContextMenuAdvOpenedCommand
    /// </summary>
    public class ContextMenuAdvOpenedCommand : ControlCommandBase<ContextMenuAdvOpenedCommandBehavior, ContextMenuAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class ContextMenuAdvOpenedCommandBehavior : CommandBehaviorBase<ContextMenuAdv>
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
    /// ContextMenuAdvOpenedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ContextMenuAdvOpenedCommandBehavior<T> : ContextMenuAdvOpenedCommandBehavior
    { }
    #endregion

    #region ContextMenuAdvClosedCommand
    /// <summary>
    /// ContextMenuAdvClosedCommand
    /// </summary>
    public class ContextMenuAdvClosedCommand : ControlCommandBase<ContextMenuAdvClosedCommandBehavior, ContextMenuAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class ContextMenuAdvClosedCommandBehavior : CommandBehaviorBase<ContextMenuAdv>
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
            TargetObject.Closed += OnEventRaised;
        }
    }

    /// <summary>
    /// ContextMenuAdvClosedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ContextMenuAdvClosedCommandBehavior<T> : ContextMenuAdvClosedCommandBehavior
    { }
    #endregion
}


