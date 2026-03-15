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

    #region RibbonBarLauncherClickCommand
    /// <summary>
    /// RibbonBarLauncherClickCommand
    /// </summary>
    public class RibbonBarLauncherClickCommand : ControlCommandBase<RibbonBarLauncherClickCommandBehavior, RibbonBar>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class RibbonBarLauncherClickCommandBehavior : CommandBehaviorBase<RibbonBar>
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
            TargetObject.LauncherClick += OnEventRaised;
        }
    }

    /// <summary>
    /// RibbonBarLauncherClickCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RibbonBarLauncherClickCommandBehavior<T> : RibbonBarLauncherClickCommandBehavior
    { }
    #endregion

    #region RibbonBarCollapsedChangedCommand
    /// <summary>
    /// RibbonBarCollapsedChangedCommand
    /// </summary>
    public class RibbonBarCollapsedChangedCommand : ControlCommandBase<RibbonBarCollapsedChangedCommandBehavior, RibbonBar>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class RibbonBarCollapsedChangedCommandBehavior : CommandBehaviorBase<RibbonBar>
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
            TargetObject.CollapsedChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// RibbonBarCollapsedChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RibbonBarCollapsedChangedCommandBehavior<T> : RibbonBarCollapsedChangedCommandBehavior
    { }
    #endregion
}


