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

    #region RibbonTabPanelItemChangedCommand
    /// <summary>
    /// RibbonTabPanelItemChangedCommand
    /// </summary>
    public class RibbonTabPanelItemChangedCommand : ControlCommandBase<RibbonTabPanelItemChangedCommandBehavior, Ribbon>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class RibbonTabPanelItemChangedCommandBehavior : CommandBehaviorBase<Ribbon>
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
            TargetObject.TabPanelItemChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// RibbonTabPanelItemChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RibbonTabPanelItemChangedCommandBehavior<T> : RibbonTabPanelItemChangedCommandBehavior
    { }
    #endregion

    #region RibbonIsQATBelowChangedCommand
    /// <summary>
    /// RibbonIsQATBelowChangedCommand
    /// </summary>
    public class RibbonIsQATBelowChangedCommand : ControlCommandBase<RibbonIsQATBelowChangedCommandBehavior, Ribbon>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class RibbonIsQATBelowChangedCommandBehavior : CommandBehaviorBase<Ribbon>
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
            TargetObject.IsQATBelowChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// RibbonIsQATBelowChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RibbonIsQATBelowChangedCommandBehavior<T> : RibbonIsQATBelowChangedCommandBehavior
    { }
    #endregion

    #region RibbonOnRibbonContextMenuOpenedCommand
    /// <summary>
    /// RibbonOnRibbonContextMenuOpenedCommand
    /// </summary>
    public class RibbonOnRibbonContextMenuOpenedCommand : ControlCommandBase<RibbonOnRibbonContextMenuOpenedCommandBehavior, Ribbon>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class RibbonOnRibbonContextMenuOpenedCommandBehavior : CommandBehaviorBase<Ribbon>
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
            TargetObject.OnRibbonContextMenuOpened += OnEventRaised;
        }
    }

    /// <summary>
    /// RibbonOnRibbonContextMenuOpenedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RibbonOnRibbonContextMenuOpenedCommandBehavior<T> : RibbonOnRibbonContextMenuOpenedCommandBehavior
    { }
    #endregion

    #region RibbonSelectedTabChangedCommand
    /// <summary>
    /// RibbonSelectedTabChangedCommand
    /// </summary>
    public class RibbonSelectedTabChangedCommand : ControlCommandBase<RibbonSelectedTabChangedCommandBehavior, Ribbon>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class RibbonSelectedTabChangedCommandBehavior : CommandBehaviorBase<Ribbon>
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
            TargetObject.SelectedTabChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// RibbonSelectedTabChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RibbonSelectedTabChangedCommandBehavior<T> : RibbonSelectedTabChangedCommandBehavior
    { }
    #endregion
}


