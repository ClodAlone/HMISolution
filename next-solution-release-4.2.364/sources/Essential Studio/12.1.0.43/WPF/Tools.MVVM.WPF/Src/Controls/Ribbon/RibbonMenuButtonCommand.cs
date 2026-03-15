#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Tools.Controls;
using System.Windows;

namespace Syncfusion.Windows.Tools.MVVM
{
    #region RibbonMenuButtonCommands
    // RibbonMenuButtonCommand
    /// <summary>
    /// 
    /// </summary>
    public class RibbonMenuButtonDescriptionChangedCommand : ControlCommandBase<RibbonMenuButtonDescriptionChangedCommandBehavior, MenuButton>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class RibbonMenuButtonDescriptionChangedCommandBehavior : CommandBehaviorBase<MenuButton>
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
            TargetObject.DescriptionChanged += OnEventRaised;
        }
    }

    // RibbonBarLauncherClickCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RibbonMenuButtonDescriptionChangedCommandBehavior<T> : RibbonMenuButtonDescriptionChangedCommandBehavior
    { }
    #endregion
}
