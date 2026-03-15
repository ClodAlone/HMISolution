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

    #region GroupBarItemExpandedChangedCommand
    // GroupBarItemExpandedChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class GroupBarItemExpandedChangedCommand : ControlCommandBase<GroupBarItemExpandedChangedCommandBehavior, GroupBarItem>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class GroupBarItemExpandedChangedCommandBehavior : CommandBehaviorBase<GroupBarItem>
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
            TargetObject.ExpandedChanged += OnEventRaised;
        }
    }

    // GroupBarItemExpandedChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class GroupBarItemExpandedChangedCommandBehavior<T> : GroupBarItemExpandedChangedCommandBehavior
    { }
    #endregion

    #region GroupBarItemImageWidthChangedCommand
    // GroupBarItemImageWidthChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class GroupBarItemImageWidthChangedCommand : ControlCommandBase<GroupBarItemImageWidthChangedCommandBehavior, GroupBarItem>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class GroupBarItemImageWidthChangedCommandBehavior : CommandBehaviorBase<GroupBarItem>
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
            TargetObject.ImageWidthChanged += OnEventRaised;
        }
    }

    // GroupBarItemImageWidthChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class GroupBarItemImageWidthChangedCommandBehavior<T> : GroupBarItemImageWidthChangedCommandBehavior
    { }
    #endregion

    #region GroupBarItemImageHeightChangedCommand
    // GroupBarItemImageHeightChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class GroupBarItemImageHeightChangedCommand : ControlCommandBase<GroupBarItemImageHeightChangedCommandBehavior, GroupBarItem>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class GroupBarItemImageHeightChangedCommandBehavior : CommandBehaviorBase<GroupBarItem>
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
            TargetObject.ImageHeightChanged += OnEventRaised;
        }
    }

    // GroupBarItemImageHeightChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class GroupBarItemImageHeightChangedCommandBehavior<T> : GroupBarItemImageHeightChangedCommandBehavior
    { }
    #endregion

    #region GroupBarItemBorderBrushChangedCommand
    // GroupBarItemBorderBrushChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class GroupBarItemBorderBrushChangedCommand : ControlCommandBase<GroupBarItemBorderBrushChangedCommandBehavior, GroupBarItem>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class GroupBarItemBorderBrushChangedCommandBehavior : CommandBehaviorBase<GroupBarItem>
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
            TargetObject.BorderBrushChanged += OnEventRaised;
        }
    }

    // GroupBarItemBorderBrushChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class GroupBarItemBorderBrushChangedCommandBehavior<T> : GroupBarItemBorderBrushChangedCommandBehavior
    { }
    #endregion

    #region GroupBarItemBorderThicknessChangedCommand
    // GroupBarItemBorderThicknessChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class GroupBarItemBorderThicknessChangedCommand : ControlCommandBase<GroupBarItemBorderThicknessChangedCommandBehavior, GroupBarItem>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class GroupBarItemBorderThicknessChangedCommandBehavior : CommandBehaviorBase<GroupBarItem>
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
            TargetObject.BorderThicknessChanged += OnEventRaised;
        }
    }

    // GroupBarItemBorderThicknessChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class GroupBarItemBorderThicknessChangedCommandBehavior<T> : GroupBarItemBorderThicknessChangedCommandBehavior
    { }
    #endregion
}

