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

    #region TileViewControlIsClickHeaderToMaximizePropertyChangedCommand
    /// <summary>
    /// TileViewControlIsClickHeaderToMaximizePropertyChangedCommand
    /// </summary>
    public class TileViewControlIsClickHeaderToMaximizePropertyChangedCommand : ControlCommandBase<TileViewControlIsClickHeaderToMaximizePropertyChangedCommandBehavior, TileViewControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class TileViewControlIsClickHeaderToMaximizePropertyChangedCommandBehavior : CommandBehaviorBase<TileViewControl>
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
            TargetObject.IsClickHeaderToMaximizePropertyChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// TileViewControlIsClickHeaderToMaximizePropertyChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TileViewControlIsClickHeaderToMaximizePropertyChangedCommandBehavior<T> : TileViewControlIsClickHeaderToMaximizePropertyChangedCommandBehavior
    { }
    #endregion

    #region TileViewControlIsMinMaxButtonOnMouseOverOnlyChangedCommand
    /// <summary>
    /// TileViewControlIsMinMaxButtonOnMouseOverOnlyChangedCommand
    /// </summary>
    public class TileViewControlIsMinMaxButtonOnMouseOverOnlyChangedCommand : ControlCommandBase<TileViewControlIsMinMaxButtonOnMouseOverOnlyChangedCommandBehavior, TileViewControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class TileViewControlIsMinMaxButtonOnMouseOverOnlyChangedCommandBehavior : CommandBehaviorBase<TileViewControl>
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
            TargetObject.IsMinMaxButtonOnMouseOverOnlyChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// TileViewControlIsMinMaxButtonOnMouseOverOnlyChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TileViewControlIsMinMaxButtonOnMouseOverOnlyChangedCommandBehavior<T> : TileViewControlIsMinMaxButtonOnMouseOverOnlyChangedCommandBehavior
    { }
    #endregion

    #region TileViewControlIsSplitterVisibilityChangedCommand
    /// <summary>
    /// TileViewControlIsSplitterVisibilityChangedCommand
    /// </summary>
    public class TileViewControlIsSplitterVisibilityChangedCommand : ControlCommandBase<TileViewControlIsSplitterVisibilityChangedCommandBehavior, TileViewControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class TileViewControlIsSplitterVisibilityChangedCommandBehavior : CommandBehaviorBase<TileViewControl>
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
            TargetObject.IsSplitterVisibilityChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// TileViewControlIsSplitterVisibilityChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TileViewControlIsSplitterVisibilityChangedCommandBehavior<T> : TileViewControlIsSplitterVisibilityChangedCommandBehavior
    { }
    #endregion

    #region TileViewControlRowCountChangedCommand
    /// <summary>
    /// TileViewControlRowCountChangedCommand
    /// </summary>
    public class TileViewControlRowCountChangedCommand : ControlCommandBase<TileViewControlRowCountChangedCommandBehavior, TileViewControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class TileViewControlRowCountChangedCommandBehavior : CommandBehaviorBase<TileViewControl>
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
            TargetObject.RowCountChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// TileViewControlRowCountChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TileViewControlRowCountChangedCommandBehavior<T> : TileViewControlRowCountChangedCommandBehavior
    { }
    #endregion

    #region TileViewControlColumnCountChangedCommand
    /// <summary>
    /// TileViewControlColumnCountChangedCommand
    /// </summary>
    public class TileViewControlColumnCountChangedCommand : ControlCommandBase<TileViewControlColumnCountChangedCommandBehavior, TileViewControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class TileViewControlColumnCountChangedCommandBehavior : CommandBehaviorBase<TileViewControl>
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
            TargetObject.ColumnCountChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// TileViewControlColumnCountChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TileViewControlColumnCountChangedCommandBehavior<T> : TileViewControlColumnCountChangedCommandBehavior
    { }
    #endregion

    #region TileViewControlAllowItemRepositioningChangedCommand
    /// <summary>
    /// TileViewControlAllowItemRepositioningChangedCommand
    /// </summary>
    public class TileViewControlAllowItemRepositioningChangedCommand : ControlCommandBase<TileViewControlAllowItemRepositioningChangedCommandBehavior, TileViewControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class TileViewControlAllowItemRepositioningChangedCommandBehavior : CommandBehaviorBase<TileViewControl>
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
            TargetObject.AllowItemRepositioningChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// TileViewControlAllowItemRepositioningChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TileViewControlAllowItemRepositioningChangedCommandBehavior<T> : TileViewControlAllowItemRepositioningChangedCommandBehavior
    { }
    #endregion

    #region TileViewControlMinimizedItemsOrientationChangedCommand
    /// <summary>
    /// TileViewControlMinimizedItemsOrientationChangedCommand
    /// </summary>
    public class TileViewControlMinimizedItemsOrientationChangedCommand : ControlCommandBase<TileViewControlMinimizedItemsOrientationChangedCommandBehavior, TileViewControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class TileViewControlMinimizedItemsOrientationChangedCommandBehavior : CommandBehaviorBase<TileViewControl>
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
            TargetObject.MinimizedItemsOrientationChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// TileViewControlMinimizedItemsOrientationChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TileViewControlMinimizedItemsOrientationChangedCommandBehavior<T> : TileViewControlMinimizedItemsOrientationChangedCommandBehavior
    { }
    #endregion

    #region TileViewControlMinimizedItemsPercentageChangedCommand
    /// <summary>
    /// TileViewControlMinimizedItemsPercentageChangedCommand
    /// </summary>
    public class TileViewControlMinimizedItemsPercentageChangedCommand : ControlCommandBase<TileViewControlMinimizedItemsPercentageChangedCommandBehavior, TileViewControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class TileViewControlMinimizedItemsPercentageChangedCommandBehavior : CommandBehaviorBase<TileViewControl>
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
            TargetObject.MinimizedItemsPercentageChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// TileViewControlMinimizedItemsPercentageChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TileViewControlMinimizedItemsPercentageChangedCommandBehavior<T> : TileViewControlMinimizedItemsPercentageChangedCommandBehavior
    { }
    #endregion

    #region TileViewControlSplitterThicknessChangedCommand
    /// <summary>
    /// TileViewControlSplitterThicknessChangedCommand
    /// </summary>
    public class TileViewControlSplitterThicknessChangedCommand : ControlCommandBase<TileViewControlSplitterThicknessChangedCommandBehavior, TileViewControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class TileViewControlSplitterThicknessChangedCommandBehavior : CommandBehaviorBase<TileViewControl>
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
            TargetObject.SplitterThicknessChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// TileViewControlSplitterThicknessChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TileViewControlSplitterThicknessChangedCommandBehavior<T> : TileViewControlSplitterThicknessChangedCommandBehavior
    { }
    #endregion

    #region TileViewControlSelectedItemChangedCommand
    /// <summary>
    /// TileViewControlSelectedItemChangedCommand
    /// </summary>
    public class TileViewControlSelectedItemChangedCommand : ControlCommandBase<TileViewControlSelectedItemChangedCommandBehavior, TileViewControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class TileViewControlSelectedItemChangedCommandBehavior : CommandBehaviorBase<TileViewControl>
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
            TargetObject.SelectedItemChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// TileViewControlSelectedItemChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TileViewControlSelectedItemChangedCommandBehavior<T> : TileViewControlSelectedItemChangedCommandBehavior
    { }
    #endregion

    #region TileViewControlMinimizedCommand
    /// <summary>
    /// TileViewControlMinimizedCommand
    /// </summary>
    public class TileViewControlMinimizedCommand : ControlCommandBase<TileViewControlMinimizedCommandBehavior, TileViewControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class TileViewControlMinimizedCommandBehavior : CommandBehaviorBase<TileViewControl>
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
            TargetObject.Minimized += OnEventRaised;
        }
    }

    /// <summary>
    /// TileViewControlMinimizedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TileViewControlMinimizedCommandBehavior<T> : TileViewControlMinimizedCommandBehavior
    { }
    #endregion

    #region TileViewControlMinimizingCommand
    /// <summary>
    /// TileViewControlMinimizingCommand
    /// </summary>
    public class TileViewControlMinimizingCommand : ControlCommandBase<TileViewControlMinimizingCommandBehavior, TileViewControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class TileViewControlMinimizingCommandBehavior : CommandBehaviorBase<TileViewControl>
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
            TargetObject.Minimizing += OnEventRaised;
        }
    }

    /// <summary>
    /// TileViewControlMinimizingCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TileViewControlMinimizingCommandBehavior<T> : TileViewControlMinimizingCommandBehavior
    { }
    #endregion

    #region TileViewControlMaximizingCommand
    /// <summary>
    /// TileViewControlMaximizingCommand
    /// </summary>
    public class TileViewControlMaximizingCommand : ControlCommandBase<TileViewControlMaximizingCommandBehavior, TileViewControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class TileViewControlMaximizingCommandBehavior : CommandBehaviorBase<TileViewControl>
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
            TargetObject.Maximizing += OnEventRaised;
        }
    }

    /// <summary>
    /// TileViewControlMaximizingCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TileViewControlMaximizingCommandBehavior<T> : TileViewControlMaximizingCommandBehavior
    { }
    #endregion

    #region TileViewControlMaximizedCommand
    /// <summary>
    /// TileViewControlMaximizedCommand
    /// </summary>
    public class TileViewControlMaximizedCommand : ControlCommandBase<TileViewControlMaximizedCommandBehavior, TileViewControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class TileViewControlMaximizedCommandBehavior : CommandBehaviorBase<TileViewControl>
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
            TargetObject.Maximized += OnEventRaised;
        }
    }

    /// <summary>
    /// TileViewControlMaximizedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TileViewControlMaximizedCommandBehavior<T> : TileViewControlMaximizedCommandBehavior
    { }
    #endregion

    #region TileViewControlRestoringCommand
    /// <summary>
    /// TileViewControlRestoringCommand
    /// </summary>
    public class TileViewControlRestoringCommand : ControlCommandBase<TileViewControlRestoringCommandBehavior, TileViewControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class TileViewControlRestoringCommandBehavior : CommandBehaviorBase<TileViewControl>
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
            TargetObject.Restoring += OnEventRaised;
        }
    }

    /// <summary>
    /// TileViewControlRestoringCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TileViewControlRestoringCommandBehavior<T> : TileViewControlRestoringCommandBehavior
    { }
    #endregion

    #region TileViewControlRestoredCommand
    /// <summary>
    /// TileViewControlRestoredCommand
    /// </summary>
    public class TileViewControlRestoredCommand : ControlCommandBase<TileViewControlRestoredCommandBehavior, TileViewControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class TileViewControlRestoredCommandBehavior : CommandBehaviorBase<TileViewControl>
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
            TargetObject.Restored += OnEventRaised;
        }
    }

    /// <summary>
    /// TileViewControlRestoredCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TileViewControlRestoredCommandBehavior<T> : TileViewControlRestoredCommandBehavior
    { }
    #endregion

    #region TileViewControlMaximizedItemChangedCommand
    /// <summary>
    /// TileViewControlMaximizedItemChangedCommand
    /// </summary>
    public class TileViewControlMaximizedItemChangedCommand : ControlCommandBase<TileViewControlMaximizedItemChangedCommandBehavior, TileViewControl>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class TileViewControlMaximizedItemChangedCommandBehavior : CommandBehaviorBase<TileViewControl>
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
            TargetObject.MaximizedItemChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// TileViewControlMaximizedItemChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TileViewControlMaximizedItemChangedCommandBehavior<T> : TileViewControlMaximizedItemChangedCommandBehavior
    { }
    #endregion
}


