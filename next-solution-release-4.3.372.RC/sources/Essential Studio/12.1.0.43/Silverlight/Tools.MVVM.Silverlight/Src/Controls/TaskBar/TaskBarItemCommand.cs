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
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.MVVM
{

    #region TaskBarItemArrowStyleChangedCommand
    // TaskBarItemArrowStyleChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TaskBarItemArrowStyleChangedCommand : ControlCommandBase<TaskBarItemArrowStyleChangedCommandBehavior, TaskBarItem>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TaskBarItemArrowStyleChangedCommandBehavior : CommandBehaviorBase<TaskBarItem>
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
            TargetObject.ArrowStyleChanged += OnEventRaised;
        }
    }

    // TaskBarItemArrowStyleChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TaskBarItemArrowStyleChangedCommandBehavior<T> : TaskBarItemArrowStyleChangedCommandBehavior
    { }
    #endregion

    #region TaskBarItemHeaderHeightChangedCommand
    // TaskBarItemHeaderHeightChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TaskBarItemHeaderHeightChangedCommand : ControlCommandBase<TaskBarItemHeaderHeightChangedCommandBehavior, TaskBarItem>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TaskBarItemHeaderHeightChangedCommandBehavior : CommandBehaviorBase<TaskBarItem>
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
            TargetObject.HeaderHeightChanged += OnEventRaised;
        }
    }

    // TaskBarItemHeaderHeightChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TaskBarItemHeaderHeightChangedCommandBehavior<T> : TaskBarItemHeaderHeightChangedCommandBehavior
    { }
    #endregion

    #region TaskBarItemIsExpandedChangedCommand
    // TaskBarItemIsExpandedChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TaskBarItemIsExpandedChangedCommand : ControlCommandBase<TaskBarItemIsExpandedChangedCommandBehavior, TaskBarItem>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TaskBarItemIsExpandedChangedCommandBehavior : CommandBehaviorBase<TaskBarItem>
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
            TargetObject.IsExpandedChanged += OnEventRaised;
        }
    }

    // TaskBarItemIsExpandedChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TaskBarItemIsExpandedChangedCommandBehavior<T> : TaskBarItemIsExpandedChangedCommandBehavior
    { }
    #endregion

    #region TaskBarItemIsHeaderShownChangedCommand
    // TaskBarItemIsHeaderShownChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TaskBarItemIsHeaderShownChangedCommand : ControlCommandBase<TaskBarItemIsHeaderShownChangedCommandBehavior, TaskBarItem>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TaskBarItemIsHeaderShownChangedCommandBehavior : CommandBehaviorBase<TaskBarItem>
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
            TargetObject.IsHeaderShownChanged += OnEventRaised;
        }
    }

    // TaskBarItemIsHeaderShownChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TaskBarItemIsHeaderShownChangedCommandBehavior<T> : TaskBarItemIsHeaderShownChangedCommandBehavior
    { }
    #endregion
}
