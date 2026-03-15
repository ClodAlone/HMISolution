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

    #region TreeViewItemAdvCollapsedCommand
    // TreeViewItemAdvCollapsedCommand
    /// <summary>
    /// 
    /// </summary>
    public class TreeViewItemAdvCollapsedCommand : ControlCommandBase<TreeViewItemAdvCollapsedCommandBehavior, TreeViewItemAdv>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class TreeViewItemAdvCollapsedCommandBehavior : CommandBehaviorBase<TreeViewItemAdv>
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
            TargetObject.Collapsed += OnEventRaised;
        }
    }

    // TreeViewItemAdvCollapsedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TreeViewItemAdvCollapsedCommandBehavior<T> : TreeViewItemAdvCollapsedCommandBehavior
    { }
    #endregion

    #region TreeViewItemAdvExpandedCommand
    // TreeViewItemAdvExpandedCommand
    /// <summary>
    /// 
    /// </summary>
    public class TreeViewItemAdvExpandedCommand : ControlCommandBase<TreeViewItemAdvExpandedCommandBehavior, TreeViewItemAdv>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class TreeViewItemAdvExpandedCommandBehavior : CommandBehaviorBase<TreeViewItemAdv>
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
            TargetObject.Expanded += OnEventRaised;
        }
    }

    // TreeViewItemAdvExpandedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TreeViewItemAdvExpandedCommandBehavior<T> : TreeViewItemAdvExpandedCommandBehavior
    { }
    #endregion

    #region TreeViewItemAdvSelectedCommand
    // TreeViewItemAdvSelectedCommand
    /// <summary>
    /// 
    /// </summary>
    public class TreeViewItemAdvSelectedCommand : ControlCommandBase<TreeViewItemAdvSelectedCommandBehavior, TreeViewItemAdv>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class TreeViewItemAdvSelectedCommandBehavior : CommandBehaviorBase<TreeViewItemAdv>
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

    // TreeViewItemAdvSelectedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TreeViewItemAdvSelectedCommandBehavior<T> : TreeViewItemAdvSelectedCommandBehavior
    { }
    #endregion

    #region TreeViewItemAdvUnselectedCommand
    // TreeViewItemAdvUnselectedCommand
    /// <summary>
    /// 
    /// </summary>
    public class TreeViewItemAdvUnselectedCommand : ControlCommandBase<TreeViewItemAdvUnselectedCommandBehavior, TreeViewItemAdv>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class TreeViewItemAdvUnselectedCommandBehavior : CommandBehaviorBase<TreeViewItemAdv>
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
            TargetObject.Unselected += OnEventRaised;
        }
    }

    // TreeViewItemAdvUnselectedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TreeViewItemAdvUnselectedCommandBehavior<T> : TreeViewItemAdvUnselectedCommandBehavior
    { }
    #endregion

    #region TreeViewItemAdvBeforeItemEditCommand
    // TreeViewItemAdvBeforeItemEditCommand
    /// <summary>
    /// 
    /// </summary>
    public class TreeViewItemAdvBeforeItemEditCommand : ControlCommandBase<TreeViewItemAdvBeforeItemEditCommandBehavior, TreeViewItemAdv>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class TreeViewItemAdvBeforeItemEditCommandBehavior : CommandBehaviorBase<TreeViewItemAdv>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, EditModeChangeEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.BeforeItemEdit += OnEventRaised;
        }
    }

    // TreeViewItemAdvBeforeItemEditCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TreeViewItemAdvBeforeItemEditCommandBehavior<T> : TreeViewItemAdvBeforeItemEditCommandBehavior
    { }
    #endregion

    #region TreeViewItemAdvAfterItemEditCommand
    // TreeViewItemAdvAfterItemEditCommand
    /// <summary>
    /// 
    /// </summary>
    public class TreeViewItemAdvAfterItemEditCommand : ControlCommandBase<TreeViewItemAdvAfterItemEditCommandBehavior, TreeViewItemAdv>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class TreeViewItemAdvAfterItemEditCommandBehavior : CommandBehaviorBase<TreeViewItemAdv>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, EditModeChangeEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.AfterItemEdit += OnEventRaised;
        }
    }

    // TreeViewItemAdvAfterItemEditCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TreeViewItemAdvAfterItemEditCommandBehavior<T> : TreeViewItemAdvAfterItemEditCommandBehavior
    { }
    #endregion

    #region TreeViewItemAdvEditKeyUpCommand
    // TreeViewItemAdvEditKeyUpCommand
    /// <summary>
    /// 
    /// </summary>
    public class TreeViewItemAdvEditKeyUpCommand : ControlCommandBase<TreeViewItemAdvEditKeyUpCommandBehavior, TreeViewItemAdv>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class TreeViewItemAdvEditKeyUpCommandBehavior : CommandBehaviorBase<TreeViewItemAdv>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, KeyEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.EditKeyUp += OnEventRaised;
        }
    }

    // TreeViewItemAdvEditKeyUpCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TreeViewItemAdvEditKeyUpCommandBehavior<T> : TreeViewItemAdvEditKeyUpCommandBehavior
    { }
    #endregion

    #region TreeViewItemAdvEditKeyDownCommand
    // TreeViewItemAdvEditKeyDownCommand
    /// <summary>
    /// 
    /// </summary>
    public class TreeViewItemAdvEditKeyDownCommand : ControlCommandBase<TreeViewItemAdvEditKeyDownCommandBehavior, TreeViewItemAdv>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class TreeViewItemAdvEditKeyDownCommandBehavior : CommandBehaviorBase<TreeViewItemAdv>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, KeyEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.EditKeyDown += OnEventRaised;
        }
    }

    // TreeViewItemAdvEditKeyDownCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TreeViewItemAdvEditKeyDownCommandBehavior<T> : TreeViewItemAdvEditKeyDownCommandBehavior
    { }
    #endregion
}


