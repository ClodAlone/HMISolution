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

    #region TabItemAdvHasHeaderChangedCommand
    // TabItemAdvHasHeaderChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabItemAdvHasHeaderChangedCommand : ControlCommandBase<TabItemAdvHasHeaderChangedCommandBehavior, TabItemAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabItemAdvHasHeaderChangedCommandBehavior : CommandBehaviorBase<TabItemAdv>
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
            TargetObject.HasHeaderChanged += OnEventRaised;
        }
    }

    // TabItemAdvHasHeaderChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TabItemAdvHasHeaderChangedCommandBehavior<T> : TabItemAdvHasHeaderChangedCommandBehavior
    { }
    #endregion

    #region TabItemAdvHeaderChangedCommand
    // TabItemAdvHeaderChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabItemAdvHeaderChangedCommand : ControlCommandBase<TabItemAdvHeaderChangedCommandBehavior, TabItemAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabItemAdvHeaderChangedCommandBehavior : CommandBehaviorBase<TabItemAdv>
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
            TargetObject.HeaderChanged += OnEventRaised;
        }
    }

    // TabItemAdvHeaderChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TabItemAdvHeaderChangedCommandBehavior<T> : TabItemAdvHeaderChangedCommandBehavior
    { }
    #endregion

    #region TabItemAdvIsSelectedChangedCommand
    // TabItemAdvIsSelectedChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabItemAdvIsSelectedChangedCommand : ControlCommandBase<TabItemAdvIsSelectedChangedCommandBehavior, TabItemAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabItemAdvIsSelectedChangedCommandBehavior : CommandBehaviorBase<TabItemAdv>
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
            TargetObject.IsSelectedChanged += OnEventRaised;
        }
    }

    // TabItemAdvIsSelectedChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TabItemAdvIsSelectedChangedCommandBehavior<T> : TabItemAdvIsSelectedChangedCommandBehavior
    { }
    #endregion

    #region TabItemAdvHoverBackgroundChangedCommand
    // TabItemAdvHoverBackgroundChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabItemAdvHoverBackgroundChangedCommand : ControlCommandBase<TabItemAdvHoverBackgroundChangedCommandBehavior, TabItemAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabItemAdvHoverBackgroundChangedCommandBehavior : CommandBehaviorBase<TabItemAdv>
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
            TargetObject.HoverBackgroundChanged += OnEventRaised;
        }
    }

    // TabItemAdvHoverBackgroundChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TabItemAdvHoverBackgroundChangedCommandBehavior<T> : TabItemAdvHoverBackgroundChangedCommandBehavior
    { }
    #endregion

    #region TabItemAdvImageAlignmentChangedCommand
    // TabItemAdvImageAlignmentChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabItemAdvImageAlignmentChangedCommand : ControlCommandBase<TabItemAdvImageAlignmentChangedCommandBehavior, TabItemAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabItemAdvImageAlignmentChangedCommandBehavior : CommandBehaviorBase<TabItemAdv>
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
            TargetObject.ImageAlignmentChanged += OnEventRaised;
        }
    }

    // TabItemAdvImageAlignmentChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TabItemAdvImageAlignmentChangedCommandBehavior<T> : TabItemAdvImageAlignmentChangedCommandBehavior
    { }
    #endregion

    #region TabItemAdvHeaderAlignmentChangedCommand
    // TabItemAdvHeaderAlignmentChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabItemAdvHeaderAlignmentChangedCommand : ControlCommandBase<TabItemAdvHeaderAlignmentChangedCommandBehavior, TabItemAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabItemAdvHeaderAlignmentChangedCommandBehavior : CommandBehaviorBase<TabItemAdv>
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
            TargetObject.HeaderAlignmentChanged += OnEventRaised;
        }
    }

    // TabItemAdvHeaderAlignmentChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TabItemAdvHeaderAlignmentChangedCommandBehavior<T> : TabItemAdvHeaderAlignmentChangedCommandBehavior
    { }
    #endregion

    #region TabItemAdvImageChangedCommand
    // TabItemAdvImageChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabItemAdvImageChangedCommand : ControlCommandBase<TabItemAdvImageChangedCommandBehavior, TabItemAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class TabItemAdvImageChangedCommandBehavior : CommandBehaviorBase<TabItemAdv>
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
            TargetObject.ImageChanged += OnEventRaised;
        }
    }

    // TabItemAdvImageChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class TabItemAdvImageChangedCommandBehavior<T> : TabItemAdvImageChangedCommandBehavior
    { }
    #endregion
}
