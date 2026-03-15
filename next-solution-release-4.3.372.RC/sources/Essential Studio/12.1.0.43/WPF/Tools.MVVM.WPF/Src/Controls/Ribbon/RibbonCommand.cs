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

    #region RibbonRibbonStateChangedCommand
    // RibbonRibbonStateChangedCommand
    /// <summary>
    /// 
    /// </summary>
    public class RibbonRibbonStateChangedCommand : ControlCommandBase<RibbonRibbonStateChangedCommandBehavior, Ribbon>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class RibbonRibbonStateChangedCommandBehavior : CommandBehaviorBase<Ribbon>
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
            TargetObject.RibbonStateChanged += OnEventRaised;
        }
    }

    // RibbonRibbonStateChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RibbonRibbonStateChangedCommandBehavior<T> : RibbonRibbonStateChangedCommandBehavior
    { }
    #endregion

    #region RibbonTabPanelItemChangedCommand
    // RibbonTabPanelItemChangedCommand
    /// <summary>
    /// 
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

    // RibbonTabPanelItemChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RibbonTabPanelItemChangedCommandBehavior<T> : RibbonTabPanelItemChangedCommandBehavior
    { }
    #endregion

    #region RibbonSelectedIndexChangedCommand
    // RibbonSelectedIndexChangedCommand
    /// <summary>
    /// 
    /// </summary>
    public class RibbonSelectedIndexChangedCommand : ControlCommandBase<RibbonSelectedIndexChangedCommandBehavior, Ribbon>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class RibbonSelectedIndexChangedCommandBehavior : CommandBehaviorBase<Ribbon>
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
            TargetObject.SelectedIndexChanged += OnEventRaised;
        }
    }

    // RibbonSelectedIndexChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RibbonSelectedIndexChangedCommandBehavior<T> : RibbonSelectedIndexChangedCommandBehavior
    { }
    #endregion

    #region RibbonHasVisibleContextTabGroupChangedCommand
    // RibbonHasVisibleContextTabGroupChangedCommand
    /// <summary>
    /// 
    /// </summary>
    public class RibbonHasVisibleContextTabGroupChangedCommand : ControlCommandBase<RibbonHasVisibleContextTabGroupChangedCommandBehavior, Ribbon>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class RibbonHasVisibleContextTabGroupChangedCommandBehavior : CommandBehaviorBase<Ribbon>
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
            TargetObject.HasVisibleContextTabGroupChanged += OnEventRaised;
        }
    }

    // RibbonHasVisibleContextTabGroupChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RibbonHasVisibleContextTabGroupChangedCommandBehavior<T> : RibbonHasVisibleContextTabGroupChangedCommandBehavior
    { }
    #endregion

    #region RibbonIsQATBelowChangedCommand
    // RibbonIsQATBelowChangedCommand
    /// <summary>
    /// 
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

    // RibbonIsQATBelowChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RibbonIsQATBelowChangedCommandBehavior<T> : RibbonIsQATBelowChangedCommandBehavior
    { }
    #endregion

    #region RibbonBackStageOpeningCommand
    // RibbonBackStageOpeningCommand
    /// <summary>
    /// 
    /// </summary>
    public class RibbonBackStageOpeningCommand : ControlCommandBase<RibbonBackStageOpeningCommandBehavior, Ribbon>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class RibbonBackStageOpeningCommandBehavior : CommandBehaviorBase<Ribbon>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, CancelEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.BackStageOpening += OnEventRaised;
        }
    }

    // RibbonBackStageOpeningCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RibbonBackStageOpeningCommandBehavior<T> : RibbonBackStageOpeningCommandBehavior
    { }
    #endregion

    #region RibbonBackStageClosingCommand
    // RibbonBackStageClosingCommand
    /// <summary>
    /// 
    /// </summary>
    public class RibbonBackStageClosingCommand : ControlCommandBase<RibbonBackStageClosingCommandBehavior, Ribbon>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class RibbonBackStageClosingCommandBehavior : CommandBehaviorBase<Ribbon>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, CancelEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.BackStageClosing += OnEventRaised;
        }
    }

    // RibbonBackStageClosingCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RibbonBackStageClosingCommandBehavior<T> : RibbonBackStageClosingCommandBehavior
    { }
    #endregion

    #region RibbonBackStageOpenedCommand
    // RibbonBackStageOpenedCommand
    /// <summary>
    /// 
    /// </summary>
    public class RibbonBackStageOpenedCommand : ControlCommandBase<RibbonBackStageOpenedCommandBehavior, Ribbon>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class RibbonBackStageOpenedCommandBehavior : CommandBehaviorBase<Ribbon>
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
            TargetObject.BackStageOpened += OnEventRaised;
        }
    }

    // RibbonBackStageOpenedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RibbonBackStageOpenedCommandBehavior<T> : RibbonBackStageOpenedCommandBehavior
    { }
    #endregion

    #region RibbonBackStageClosedCommand
    // RibbonBackStageClosedCommand
    /// <summary>
    /// 
    /// </summary>
    public class RibbonBackStageClosedCommand : ControlCommandBase<RibbonBackStageClosedCommandBehavior, Ribbon>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class RibbonBackStageClosedCommandBehavior : CommandBehaviorBase<Ribbon>
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
            TargetObject.BackStageClosed += OnEventRaised;
        }
    }

    // RibbonBackStageClosedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RibbonBackStageClosedCommandBehavior<T> : RibbonBackStageClosedCommandBehavior
    { }
    #endregion

    #region RibbonQATCustomizeDialogOpeningCommand
    // RibbonQATCustomizeDialogOpeningCommand
    /// <summary>
    /// 
    /// </summary>
    public class RibbonQATCustomizeDialogOpeningCommand : ControlCommandBase<RibbonQATCustomizeDialogOpeningCommandBehavior, Ribbon>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class RibbonQATCustomizeDialogOpeningCommandBehavior : CommandBehaviorBase<Ribbon>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, CancelEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.QATCustomizeDialogOpening += OnEventRaised;
        }
    }

    // RibbonQATCustomizeDialogOpeningCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RibbonQATCustomizeDialogOpeningCommandBehavior<T> : RibbonQATCustomizeDialogOpeningCommandBehavior
    { }
    #endregion

    #region RibbonQATCustomizeDialogClosedCommand
    // RibbonQATCustomizeDialogClosedCommand
    /// <summary>
    /// 
    /// </summary>
    public class RibbonQATCustomizeDialogClosedCommand : ControlCommandBase<RibbonQATCustomizeDialogClosedCommandBehavior, Ribbon>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class RibbonQATCustomizeDialogClosedCommandBehavior : CommandBehaviorBase<Ribbon>
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
            TargetObject.QATCustomizeDialogClosed += OnEventRaised;
        }
    }

    // RibbonQATCustomizeDialogClosedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RibbonQATCustomizeDialogClosedCommandBehavior<T> : RibbonQATCustomizeDialogClosedCommandBehavior
    { }
    #endregion

    #region RibbonBeforeQatDropDownPopupCommand
    // RibbonBeforeQatDropDownPopupCommand
    /// <summary>
    /// 
    /// </summary>
    public class RibbonBeforeQatDropDownPopupCommand : ControlCommandBase<RibbonBeforeQatDropDownPopupCommandBehavior, Ribbon>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class RibbonBeforeQatDropDownPopupCommandBehavior : CommandBehaviorBase<Ribbon>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, CancelEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.BeforeQatDropDownPopup += OnEventRaised;
        }
    }

    // RibbonBeforeQatDropDownPopupCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RibbonBeforeQatDropDownPopupCommandBehavior<T> : RibbonBeforeQatDropDownPopupCommandBehavior
    { }
    #endregion

    #region RibbonAfterQatDropDownPopupCommand
    // RibbonAfterQatDropDownPopupCommand
    /// <summary>
    /// 
    /// </summary>
    public class RibbonAfterQatDropDownPopupCommand : ControlCommandBase<RibbonAfterQatDropDownPopupCommandBehavior, Ribbon>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class RibbonAfterQatDropDownPopupCommandBehavior : CommandBehaviorBase<Ribbon>
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
            TargetObject.AfterQatDropDownPopup += OnEventRaised;
        }
    }

    // RibbonAfterQatDropDownPopupCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RibbonAfterQatDropDownPopupCommandBehavior<T> : RibbonAfterQatDropDownPopupCommandBehavior
    { }
    #endregion

    #region RibbonRibbonContextMenuOpeningCommand
    // RibbonRibbonContextMenuOpeningCommand
    /// <summary>
    /// 
    /// </summary>
    public class RibbonRibbonContextMenuOpeningCommand : ControlCommandBase<RibbonRibbonContextMenuOpeningCommandBehavior, Ribbon>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class RibbonRibbonContextMenuOpeningCommandBehavior : CommandBehaviorBase<Ribbon>
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
            TargetObject.RibbonContextMenuOpening += OnEventRaised;
        }
    }

    // RibbonRibbonContextMenuOpeningCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RibbonRibbonContextMenuOpeningCommandBehavior<T> : RibbonRibbonContextMenuOpeningCommandBehavior
    { }
    #endregion

    #region RibbonRibbonContextMenuClosingCommand
    // QATItemsCollectionChangedRibbonRibbonContextMenuClosingCommand
    /// <summary>
    /// 
    /// </summary>
    public class RibbonRibbonContextMenuClosingCommand : ControlCommandBase<RibbonRibbonContextMenuClosingCommandBehavior, Ribbon>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class RibbonRibbonContextMenuClosingCommandBehavior : CommandBehaviorBase<Ribbon>
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
            TargetObject.RibbonContextMenuClosing += OnEventRaised;
        }
    }

    // RibbonRibbonContextMenuClosingCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RibbonRibbonContextMenuClosingCommandBehavior<T> : RibbonRibbonContextMenuClosingCommandBehavior
    { }
    #endregion

    #region RibbonQATItemsCollectionChangedCommand
    // QATItemsCollectionChanged
    /// <summary>
    /// 
    /// </summary>
    public class RibbonQATItemsCollectionChangedCommand : ControlCommandBase<RibbonQATItemsCollectionChangedCommandBehavior, Ribbon>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class RibbonQATItemsCollectionChangedCommandBehavior : CommandBehaviorBase<Ribbon>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(QATItemsCollectionChangedEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.QATItemsCollectionChanged += OnEventRaised;
        }
    }

    // RibbonRibbonContextMenuClosingCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RibbonQATItemsCollectionChangedCommandBehavior<T> : RibbonQATItemsCollectionChangedCommandBehavior
    { }
    #endregion
}
