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

    #region CheckedListBoxItemIsCheckedChangedCommand
    // CheckedListBoxItemIsCheckedChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CheckedListBoxItemIsCheckedChangedCommand : ControlCommandBase<CheckedListBoxItemIsCheckedChangedCommandBehavior, CheckedListBoxItem>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CheckedListBoxItemIsCheckedChangedCommandBehavior : CommandBehaviorBase<CheckedListBoxItem>
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
            TargetObject.IsCheckedChanged += OnEventRaised;
        }
    }

    // CheckedListBoxItemIsCheckedChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class CheckedListBoxItemIsCheckedChangedCommandBehavior<T> : CheckedListBoxItemIsCheckedChangedCommandBehavior
    { }
    #endregion

    #region CheckedListBoxItemIsSelectedChangedCommand
    // CheckedListBoxItemIsSelectedChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CheckedListBoxItemIsSelectedChangedCommand : ControlCommandBase<CheckedListBoxItemIsSelectedChangedCommandBehavior, CheckedListBoxItem>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CheckedListBoxItemIsSelectedChangedCommandBehavior : CommandBehaviorBase<CheckedListBoxItem>
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

    // CheckedListBoxItemIsSelectedChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class CheckedListBoxItemIsSelectedChangedCommandBehavior<T> : CheckedListBoxItemIsSelectedChangedCommandBehavior
    { }
    #endregion

    #region CheckedListBoxItemIsMouseOverChangedCommand
    // CheckedListBoxItemIsMouseOverChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CheckedListBoxItemIsMouseOverChangedCommand : ControlCommandBase<CheckedListBoxItemIsMouseOverChangedCommandBehavior, CheckedListBoxItem>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CheckedListBoxItemIsMouseOverChangedCommandBehavior : CommandBehaviorBase<CheckedListBoxItem>
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
            TargetObject.IsMouseOverChanged += OnEventRaised;
        }
    }

    // CheckedListBoxItemIsMouseOverChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class CheckedListBoxItemIsMouseOverChangedCommandBehavior<T> : CheckedListBoxItemIsMouseOverChangedCommandBehavior
    { }
    #endregion

    #region CheckedListBoxItemLeftImageSourceChangedCommand
    // CheckedListBoxItemLeftImageSourceChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CheckedListBoxItemLeftImageSourceChangedCommand : ControlCommandBase<CheckedListBoxItemLeftImageSourceChangedCommandBehavior, CheckedListBoxItem>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CheckedListBoxItemLeftImageSourceChangedCommandBehavior : CommandBehaviorBase<CheckedListBoxItem>
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
            TargetObject.LeftImageSourceChanged += OnEventRaised;
        }
    }

    // CheckedListBoxItemLeftImageSourceChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class CheckedListBoxItemLeftImageSourceChangedCommandBehavior<T> : CheckedListBoxItemLeftImageSourceChangedCommandBehavior
    { }
    #endregion

    #region CheckedListBoxItemRightImageSourceChangedCommand
    // CheckedListBoxItemRightImageSourceChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CheckedListBoxItemRightImageSourceChangedCommand : ControlCommandBase<CheckedListBoxItemRightImageSourceChangedCommandBehavior, CheckedListBoxItem>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CheckedListBoxItemRightImageSourceChangedCommandBehavior : CommandBehaviorBase<CheckedListBoxItem>
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
            TargetObject.RightImageSourceChanged += OnEventRaised;
        }
    }

    // CheckedListBoxItemRightImageSourceChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class CheckedListBoxItemRightImageSourceChangedCommandBehavior<T> : CheckedListBoxItemRightImageSourceChangedCommandBehavior
    { }
    #endregion

    #region CheckedListBoxItemLeftImageWidthChangedCommand
    // CheckedListBoxItemLeftImageWidthChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CheckedListBoxItemLeftImageWidthChangedCommand : ControlCommandBase<CheckedListBoxItemLeftImageWidthChangedCommandBehavior, CheckedListBoxItem>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CheckedListBoxItemLeftImageWidthChangedCommandBehavior : CommandBehaviorBase<CheckedListBoxItem>
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
            TargetObject.LeftImageWidthChanged += OnEventRaised;
        }
    }

    // CheckedListBoxItemLeftImageWidthChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class CheckedListBoxItemLeftImageWidthChangedCommandBehavior<T> : CheckedListBoxItemLeftImageWidthChangedCommandBehavior
    { }
    #endregion

    #region CheckedListBoxItemRightImageWidthChangedCommand
    // CheckedListBoxItemRightImageWidthChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CheckedListBoxItemRightImageWidthChangedCommand : ControlCommandBase<CheckedListBoxItemRightImageWidthChangedCommandBehavior, CheckedListBoxItem>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CheckedListBoxItemRightImageWidthChangedCommandBehavior : CommandBehaviorBase<CheckedListBoxItem>
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
            TargetObject.RightImageWidthChanged += OnEventRaised;
        }
    }

    // CheckedListBoxItemRightImageWidthChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class CheckedListBoxItemRightImageWidthChangedCommandBehavior<T> : CheckedListBoxItemRightImageWidthChangedCommandBehavior
    { }
    #endregion

    #region CheckedListBoxItemLeftImageHeightChangedCommand
    // CheckedListBoxItemLeftImageHeightChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CheckedListBoxItemLeftImageHeightChangedCommand : ControlCommandBase<CheckedListBoxItemLeftImageHeightChangedCommandBehavior, CheckedListBoxItem>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CheckedListBoxItemLeftImageHeightChangedCommandBehavior : CommandBehaviorBase<CheckedListBoxItem>
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
            TargetObject.LeftImageHeightChanged += OnEventRaised;
        }
    }

    // CheckedListBoxItemLeftImageHeightChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class CheckedListBoxItemLeftImageHeightChangedCommandBehavior<T> : CheckedListBoxItemLeftImageHeightChangedCommandBehavior
    { }
    #endregion

    #region CheckedListBoxItemRightImageHeightChangedCommand
    // CheckedListBoxItemRightImageHeightChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CheckedListBoxItemRightImageHeightChangedCommand : ControlCommandBase<CheckedListBoxItemRightImageHeightChangedCommandBehavior, CheckedListBoxItem>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CheckedListBoxItemRightImageHeightChangedCommandBehavior : CommandBehaviorBase<CheckedListBoxItem>
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
            TargetObject.RightImageHeightChanged += OnEventRaised;
        }
    }

    // CheckedListBoxItemRightImageHeightChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class CheckedListBoxItemRightImageHeightChangedCommandBehavior<T> : CheckedListBoxItemRightImageHeightChangedCommandBehavior
    { }
    #endregion

    #region CheckedListBoxItemImageVerticalAlignmentChangedCommand
    // CheckedListBoxItemImageVerticalAlignmentChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CheckedListBoxItemImageVerticalAlignmentChangedCommand : ControlCommandBase<CheckedListBoxItemImageVerticalAlignmentChangedCommandBehavior, CheckedListBoxItem>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CheckedListBoxItemImageVerticalAlignmentChangedCommandBehavior : CommandBehaviorBase<CheckedListBoxItem>
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
            TargetObject.ImageVerticalAlignmentChanged += OnEventRaised;
        }
    }

    // CheckedListBoxItemImageVerticalAlignmentChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class CheckedListBoxItemImageVerticalAlignmentChangedCommandBehavior<T> : CheckedListBoxItemImageVerticalAlignmentChangedCommandBehavior
    { }
    #endregion

    #region CheckedListBoxItemImageMarginChangedCommand
    // CheckedListBoxItemImageMarginChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CheckedListBoxItemImageMarginChangedCommand : ControlCommandBase<CheckedListBoxItemImageMarginChangedCommandBehavior, CheckedListBoxItem>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CheckedListBoxItemImageMarginChangedCommandBehavior : CommandBehaviorBase<CheckedListBoxItem>
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
            TargetObject.ImageMarginChanged += OnEventRaised;
        }
    }

    // CheckedListBoxItemImageMarginChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class CheckedListBoxItemImageMarginChangedCommandBehavior<T> : CheckedListBoxItemImageMarginChangedCommandBehavior
    { }
    #endregion
}
