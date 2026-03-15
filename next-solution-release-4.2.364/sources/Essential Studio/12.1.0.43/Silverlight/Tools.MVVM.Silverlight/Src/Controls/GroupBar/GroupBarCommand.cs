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

    #region GroupBarSelectionChangedCommand
    // GroupBarSelectionChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class GroupBarSelectionChangedCommand : ControlCommandBase<GroupBarSelectionChangedCommandBehavior, GroupBar>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class GroupBarSelectionChangedCommandBehavior : CommandBehaviorBase<GroupBar>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, GroupBarSelectionChangedEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.SelectionChanged += OnEventRaised;
        }
    }

    // GroupBarSelectionChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class GroupBarSelectionChangedCommandBehavior<T> : GroupBarSelectionChangedCommandBehavior
    { }
    #endregion
}