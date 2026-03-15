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

    #region HierarchyNavigatorHierarchyNavigatorRefreshButtonClickCommand
    // HierarchyNavigatorHierarchyNavigatorRefreshButtonClickCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class HierarchyNavigatorHierarchyNavigatorRefreshButtonClickCommand : ControlCommandBase<HierarchyNavigatorHierarchyNavigatorRefreshButtonClickCommandBehavior, HierarchyNavigator>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class HierarchyNavigatorHierarchyNavigatorRefreshButtonClickCommandBehavior : CommandBehaviorBase<HierarchyNavigator>
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

    // HierarchyNavigatorHierarchyNavigatorRefreshButtonClickCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class HierarchyNavigatorHierarchyNavigatorRefreshButtonClickCommandBehavior<T> : HierarchyNavigatorHierarchyNavigatorRefreshButtonClickCommandBehavior
    { }
    #endregion

    #region HierarchyNavigatorHierarchyNavigatorSelectedItemChangedCommand
    // HierarchyNavigatorHierarchyNavigatorSelectedItemChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class HierarchyNavigatorHierarchyNavigatorSelectedItemChangedCommand : ControlCommandBase<HierarchyNavigatorHierarchyNavigatorSelectedItemChangedCommandBehavior, HierarchyNavigator>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class HierarchyNavigatorHierarchyNavigatorSelectedItemChangedCommandBehavior : CommandBehaviorBase<HierarchyNavigator>
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

    // HierarchyNavigatorHierarchyNavigatorSelectedItemChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class HierarchyNavigatorHierarchyNavigatorSelectedItemChangedCommandBehavior<T> : HierarchyNavigatorHierarchyNavigatorSelectedItemChangedCommandBehavior
    { }
    #endregion

    #region HierarchyNavigatorNavigationPopupOpenedCommand
    // HierarchyNavigatorNavigationPopupOpenedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class HierarchyNavigatorNavigationPopupOpenedCommand : ControlCommandBase<HierarchyNavigatorNavigationPopupOpenedCommandBehavior, HierarchyNavigator>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class HierarchyNavigatorNavigationPopupOpenedCommandBehavior : CommandBehaviorBase<HierarchyNavigator>
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

    // HierarchyNavigatorNavigationPopupOpenedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class HierarchyNavigatorNavigationPopupOpenedCommandBehavior<T> : HierarchyNavigatorNavigationPopupOpenedCommandBehavior
    { }
    #endregion

    #region HierarchyNavigatorNavigationPopupOpeningCommand
    // HierarchyNavigatorNavigationPopupOpeningCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class HierarchyNavigatorNavigationPopupOpeningCommand : ControlCommandBase<HierarchyNavigatorNavigationPopupOpeningCommandBehavior, HierarchyNavigator>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class HierarchyNavigatorNavigationPopupOpeningCommandBehavior : CommandBehaviorBase<HierarchyNavigator>
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

    // HierarchyNavigatorNavigationPopupOpeningCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class HierarchyNavigatorNavigationPopupOpeningCommandBehavior<T> : HierarchyNavigatorNavigationPopupOpeningCommandBehavior
    { }
    #endregion

    #region HierarchyNavigatorNavigationPopupClosingCommand
    // HierarchyNavigatorNavigationPopupClosingCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class HierarchyNavigatorNavigationPopupClosingCommand : ControlCommandBase<HierarchyNavigatorNavigationPopupClosingCommandBehavior, HierarchyNavigator>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class HierarchyNavigatorNavigationPopupClosingCommandBehavior : CommandBehaviorBase<HierarchyNavigator>
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

    // HierarchyNavigatorNavigationPopupClosingCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class HierarchyNavigatorNavigationPopupClosingCommandBehavior<T> : HierarchyNavigatorNavigationPopupClosingCommandBehavior
    { }
    #endregion

    #region HierarchyNavigatorNavigationPopupClosedCommand
    // HierarchyNavigatorNavigationPopupClosedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class HierarchyNavigatorNavigationPopupClosedCommand : ControlCommandBase<HierarchyNavigatorNavigationPopupClosedCommandBehavior, HierarchyNavigator>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class HierarchyNavigatorNavigationPopupClosedCommandBehavior : CommandBehaviorBase<HierarchyNavigator>
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

    // HierarchyNavigatorNavigationPopupClosedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class HierarchyNavigatorNavigationPopupClosedCommandBehavior<T> : HierarchyNavigatorNavigationPopupClosedCommandBehavior
    { }
    #endregion
}
