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
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.Windows.Shared;
using System.Windows;

namespace Syncfusion.Windows.Tools.MVVM
{
    #region RibbonSplitMenuButtonCommands
    // RibbonSplitMenuButtonCommand

    #region RibbonSplitMenuButtonIconSizeChangedCommand
    /// <summary>
    /// 
    /// </summary>
    public class RibbonSplitMenuButtonIconSizeChangedCommand : ControlCommandBase<RibbonSplitMenuButtonIconSizeChangedCommandBehavior, SplitMenuButton>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class RibbonSplitMenuButtonIconSizeChangedCommandBehavior : CommandBehaviorBase<SplitMenuButton>
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
            TargetObject.IconSizeChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RibbonSplitMenuButtonIconSizeChangedCommandBehavior<T> : RibbonSplitMenuButtonIconSizeChangedCommandBehavior
    { }
    #endregion


    #region RibbonSplitMenuButtonSmallIconChangedCommand
    /// <summary>
    /// 
    /// </summary>
    public class RibbonSplitMenuButtonSmallIconChangedCommand : ControlCommandBase<RibbonSplitMenuButtonSmallIconChangedCommandBehavior, SplitMenuButton>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class RibbonSplitMenuButtonSmallIconChangedCommandBehavior : CommandBehaviorBase<SplitMenuButton>
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
            TargetObject.SmallIconChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RibbonSplitMenuButtonSmallIconChangedCommandBehavior<T> : RibbonSplitMenuButtonSmallIconChangedCommandBehavior
    { }
    #endregion

#endregion
}
