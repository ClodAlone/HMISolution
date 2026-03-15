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

    #region HierarchyNavigatorBarContentDropDownItemSelectedCommand
    // HierarchyNavigatorBarContentDropDownItemSelectedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class HierarchyNavigatorBarContentDropDownItemSelectedCommand : ControlCommandBase<HierarchyNavigatorBarContentDropDownItemSelectedCommandBehavior, HierarchyNavigatorBarContent>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class HierarchyNavigatorBarContentDropDownItemSelectedCommandBehavior : CommandBehaviorBase<HierarchyNavigatorBarContent>
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
            TargetObject.DropDownItemSelected += OnEventRaised;
        }
    }

    // HierarchyNavigatorBarContentDropDownItemSelectedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class HierarchyNavigatorBarContentDropDownItemSelectedCommandBehavior<T> : HierarchyNavigatorBarContentDropDownItemSelectedCommandBehavior
    { }
    #endregion

    #region HierarchyNavigatorBarContentNavigationPopupShowHideClickCommand
    // HierarchyNavigatorBarContentNavigationPopupShowHideClickCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class HierarchyNavigatorBarContentNavigationPopupShowHideClickCommand : ControlCommandBase<HierarchyNavigatorBarContentNavigationPopupShowHideClickCommandBehavior, HierarchyNavigatorBarContent>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)] 
    public class HierarchyNavigatorBarContentNavigationPopupShowHideClickCommandBehavior : CommandBehaviorBase<HierarchyNavigatorBarContent>
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
            TargetObject.NavigationPopupShowHideClick += OnEventRaised;
        }
    }

    // HierarchyNavigatorBarContentNavigationPopupShowHideClickCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class HierarchyNavigatorBarContentNavigationPopupShowHideClickCommandBehavior<T> : HierarchyNavigatorBarContentNavigationPopupShowHideClickCommandBehavior
    { }
    #endregion
}
