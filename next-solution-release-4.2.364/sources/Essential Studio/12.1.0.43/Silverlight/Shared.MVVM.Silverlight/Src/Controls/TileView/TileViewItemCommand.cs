#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.Windows.Shared;


namespace Syncfusion.Windows.Tools.MVVM
{

    #region TileViewItemStateChangedCommand
    /// <summary>
    /// TileViewItemStateChangedCommand
    /// </summary>
    public class TileViewItemStateChangedCommand : ControlCommandBase<TileViewItemStateChangedCommandBehavior, TileViewItem>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class TileViewItemStateChangedCommandBehavior : CommandBehaviorBase<TileViewItem>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, TileViewEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.StateChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// TileViewItemStateChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TileViewItemStateChangedCommandBehavior<T> : TileViewItemStateChangedCommandBehavior
    { }
    #endregion

    #region TileViewItemStateChangingCommand
    /// <summary>
    /// TileViewItemStateChangingCommand
    /// </summary>
    public class TileViewItemStateChangingCommand : ControlCommandBase<TileViewItemStateChangingCommandBehavior, TileViewItem>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class TileViewItemStateChangingCommandBehavior : CommandBehaviorBase<TileViewItem>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, TileViewCancelEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.StateChanging += OnEventRaised;
        }
    }

    /// <summary>
    /// TileViewItemStateChangingCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TileViewItemStateChangingCommandBehavior<T> : TileViewItemStateChangingCommandBehavior
    { }
    #endregion

    #region TileViewItemSelectedCommand
    /// <summary>
    /// TileViewItemSelectedCommand
    /// </summary>
    public class TileViewItemSelectedCommand : ControlCommandBase<TileViewItemSelectedCommandBehavior, TileViewItem>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class TileViewItemSelectedCommandBehavior : CommandBehaviorBase<TileViewItem>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, RoutedEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.Selected += OnEventRaised;
        }
    }

    /// <summary>
    /// TileViewItemSelectedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TileViewItemSelectedCommandBehavior<T> : TileViewItemSelectedCommandBehavior
    { }
    #endregion
}


