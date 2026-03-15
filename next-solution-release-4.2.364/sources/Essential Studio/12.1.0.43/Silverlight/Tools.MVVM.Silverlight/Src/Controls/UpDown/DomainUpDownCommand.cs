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

    #region DomainUpDownCornerRadiusChangedCommand
    // DomainUpDownCornerRadiusChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class DomainUpDownCornerRadiusChangedCommand : ControlCommandBase<DomainUpDownCornerRadiusChangedCommandBehavior, DomainUpDown>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class DomainUpDownCornerRadiusChangedCommandBehavior : CommandBehaviorBase<DomainUpDown>
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
            TargetObject.CornerRadiusChanged += OnEventRaised;
        }
    }

    // DomainUpDownCornerRadiusChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class DomainUpDownCornerRadiusChangedCommandBehavior<T> : DomainUpDownCornerRadiusChangedCommandBehavior
    { }
    #endregion

    #region DomainUpDownFlowDirectionChangedCommand
    // DomainUpDownFlowDirectionChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class DomainUpDownFlowDirectionChangedCommand : ControlCommandBase<DomainUpDownFlowDirectionChangedCommandBehavior, DomainUpDown>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class DomainUpDownFlowDirectionChangedCommandBehavior : CommandBehaviorBase<DomainUpDown>
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
            TargetObject.FlowDirectionChanged += OnEventRaised;
        }
    }

    // DomainUpDownFlowDirectionChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class DomainUpDownFlowDirectionChangedCommandBehavior<T> : DomainUpDownFlowDirectionChangedCommandBehavior
    { }
    #endregion

    #region DomainUpDownTextAlignmentChangedCommand
    // DomainUpDownTextAlignmentChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class DomainUpDownTextAlignmentChangedCommand : ControlCommandBase<DomainUpDownTextAlignmentChangedCommandBehavior, DomainUpDown>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class DomainUpDownTextAlignmentChangedCommandBehavior : CommandBehaviorBase<DomainUpDown>
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
            TargetObject.TextAlignmentChanged += OnEventRaised;
        }
    }

    // DomainUpDownTextAlignmentChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class DomainUpDownTextAlignmentChangedCommandBehavior<T> : DomainUpDownTextAlignmentChangedCommandBehavior
    { }
    #endregion

    #region DomainUpDownValueChangedCommand
    // DomainUpDownValueChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class DomainUpDownValueChangedCommand : ControlCommandBase<DomainUpDownValueChangedCommandBehavior, DomainUpDown>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class DomainUpDownValueChangedCommandBehavior : CommandBehaviorBase<DomainUpDown>
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
            TargetObject.ValueChanged += OnEventRaised;
        }
    }

    // DomainUpDownValueChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class DomainUpDownValueChangedCommandBehavior<T> : DomainUpDownValueChangedCommandBehavior
    { }
    #endregion
}
