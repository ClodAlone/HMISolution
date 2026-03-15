#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Tools.Controls;

namespace Syncfusion.Windows.Tools.MVVM
{

    #region HierarchyNavigatorItemsControlHierarchyNavigatorRefreshButtonClickCommand
    // HierarchyNavigatorItemsControlHierarchyNavigatorRefreshButtonClickCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class HierarchyNavigatorItemsControlHierarchyNavigatorRefreshButtonClickCommand : ControlCommandBase<HierarchyNavigatorItemsControlHierarchyNavigatorRefreshButtonClickCommandBehavior, HierarchyNavigatorItemsControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class HierarchyNavigatorItemsControlHierarchyNavigatorRefreshButtonClickCommandBehavior : CommandBehaviorBase<HierarchyNavigatorItemsControl>
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
            TargetObject.HierarchyNavigatorRefreshButtonClick += OnEventRaised;
        }
    }

    // HierarchyNavigatorItemsControlHierarchyNavigatorRefreshButtonClickCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class HierarchyNavigatorItemsControlHierarchyNavigatorRefreshButtonClickCommandBehavior<T> : HierarchyNavigatorItemsControlHierarchyNavigatorRefreshButtonClickCommandBehavior
    { }
    #endregion

    #region HierarchyNavigatorItemsControlHierarchyNavigatorSelectedItemChangedCommand
    // HierarchyNavigatorItemsControlHierarchyNavigatorSelectedItemChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class HierarchyNavigatorItemsControlHierarchyNavigatorSelectedItemChangedCommand : ControlCommandBase<HierarchyNavigatorItemsControlHierarchyNavigatorSelectedItemChangedCommandBehavior, HierarchyNavigatorItemsControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class HierarchyNavigatorItemsControlHierarchyNavigatorSelectedItemChangedCommandBehavior : CommandBehaviorBase<HierarchyNavigatorItemsControl>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, HierarchyNavigatorSelectedItemChangedEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.HierarchyNavigatorSelectedItemChanged += OnEventRaised;
        }
    }

    // HierarchyNavigatorItemsControlHierarchyNavigatorSelectedItemChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class HierarchyNavigatorItemsControlHierarchyNavigatorSelectedItemChangedCommandBehavior<T> : HierarchyNavigatorItemsControlHierarchyNavigatorSelectedItemChangedCommandBehavior
    { }
    #endregion

    #region HierarchyNavigatorItemsControlNavigationPopupOpeningCommand
    // HierarchyNavigatorItemsControlNavigationPopupOpeningCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class HierarchyNavigatorItemsControlNavigationPopupOpeningCommand : ControlCommandBase<HierarchyNavigatorItemsControlNavigationPopupOpeningCommandBehavior, HierarchyNavigatorItemsControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class HierarchyNavigatorItemsControlNavigationPopupOpeningCommandBehavior : CommandBehaviorBase<HierarchyNavigatorItemsControl>
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
            TargetObject.NavigationPopupOpening += OnEventRaised;
        }
    }

    // HierarchyNavigatorItemsControlNavigationPopupOpeningCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class HierarchyNavigatorItemsControlNavigationPopupOpeningCommandBehavior<T> : HierarchyNavigatorItemsControlNavigationPopupOpeningCommandBehavior
    { }
    #endregion

    #region HierarchyNavigatorItemsControlNavigationPopupOpenedCommand
    // HierarchyNavigatorItemsControlNavigationPopupOpenedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class HierarchyNavigatorItemsControlNavigationPopupOpenedCommand : ControlCommandBase<HierarchyNavigatorItemsControlNavigationPopupOpenedCommandBehavior, HierarchyNavigatorItemsControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class HierarchyNavigatorItemsControlNavigationPopupOpenedCommandBehavior : CommandBehaviorBase<HierarchyNavigatorItemsControl>
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
            TargetObject.NavigationPopupOpened += OnEventRaised;
        }
    }

    // HierarchyNavigatorItemsControlNavigationPopupOpenedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class HierarchyNavigatorItemsControlNavigationPopupOpenedCommandBehavior<T> : HierarchyNavigatorItemsControlNavigationPopupOpenedCommandBehavior
    { }
    #endregion

    #region HierarchyNavigatorItemsControlNavigationPopupClosingCommand
    // HierarchyNavigatorItemsControlNavigationPopupClosingCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class HierarchyNavigatorItemsControlNavigationPopupClosingCommand : ControlCommandBase<HierarchyNavigatorItemsControlNavigationPopupClosingCommandBehavior, HierarchyNavigatorItemsControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class HierarchyNavigatorItemsControlNavigationPopupClosingCommandBehavior : CommandBehaviorBase<HierarchyNavigatorItemsControl>
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
            TargetObject.NavigationPopupClosing += OnEventRaised;
        }
    }

    // HierarchyNavigatorItemsControlNavigationPopupClosingCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class HierarchyNavigatorItemsControlNavigationPopupClosingCommandBehavior<T> : HierarchyNavigatorItemsControlNavigationPopupClosingCommandBehavior
    { }
    #endregion

    #region HierarchyNavigatorItemsControlNavigationPopupClosedCommand
    // HierarchyNavigatorItemsControlNavigationPopupClosedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class HierarchyNavigatorItemsControlNavigationPopupClosedCommand : ControlCommandBase<HierarchyNavigatorItemsControlNavigationPopupClosedCommandBehavior, HierarchyNavigatorItemsControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class HierarchyNavigatorItemsControlNavigationPopupClosedCommandBehavior : CommandBehaviorBase<HierarchyNavigatorItemsControl>
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
            TargetObject.NavigationPopupClosed += OnEventRaised;
        }
    }

    // HierarchyNavigatorItemsControlNavigationPopupClosedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class HierarchyNavigatorItemsControlNavigationPopupClosedCommandBehavior<T> : HierarchyNavigatorItemsControlNavigationPopupClosedCommandBehavior
    { }
    #endregion
}
