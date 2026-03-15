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

using System.Windows.Data;
using System.Windows.Input;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.Windows.Shared;


namespace Syncfusion.Windows.Tools.MVVM
{
    #region CheckListBoxSelectedCommand
    // CheckListBoxSelectedCommand
    /// <summary>
    /// 
    /// </summary>
    public class CheckListBoxSelectedCommand : ControlCommandBase<CheckListBoxSelectedCommandBehavior, CheckListBox>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class CheckListBoxSelectedCommandBehavior : CommandBehaviorBase<CheckListBox>
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
            TargetObject.SelectionChanged += OnEventRaised;
        }
    }

    // CheckListBoxSelectedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CheckListBoxSelectedCommandBehavior<T> : CheckListBoxSelectedCommandBehavior
    { }
    #endregion

	#region CheckListBoxIsDragDropEnabledChangedCommand
	// CheckListBoxIsDragDropEnabledChangedCommand
    /// <summary>
    /// 
    /// </summary>
	public class CheckListBoxIsDragDropEnabledChangedCommand : ControlCommandBase<CheckListBoxIsDragDropEnabledChangedCommandBehavior, CheckListBox>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CheckListBoxIsDragDropEnabledChangedCommandBehavior : CommandBehaviorBase<CheckListBox>
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
            TargetObject.IsDragDropEnabledChanged += OnEventRaised;
        }
    }

	// CheckListBoxIsDragDropEnabledChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CheckListBoxIsDragDropEnabledChangedCommandBehavior<T> : CheckListBoxIsDragDropEnabledChangedCommandBehavior
    { }
	#endregion

	#region CheckListBoxDragStartCommand
	// CheckListBoxDragStartCommand
    /// <summary>
    /// 
    /// </summary>
	public class CheckListBoxDragStartCommand : ControlCommandBase<CheckListBoxDragStartCommandBehavior, CheckListBox>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CheckListBoxDragStartCommandBehavior : CommandBehaviorBase<CheckListBox>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, DragCheckLixtBoxEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.DragStart += OnEventRaised;
        }
    }

	// CheckListBoxDragStartCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CheckListBoxDragStartCommandBehavior<T> : CheckListBoxDragStartCommandBehavior
    { }
	#endregion

	#region CheckListBoxDragEndCommand
	// CheckListBoxDragEndCommand
    /// <summary>
    /// 
    /// </summary>
	public class CheckListBoxDragEndCommand : ControlCommandBase<CheckListBoxDragEndCommandBehavior, CheckListBox>
	{ }
    /// <summary>
    /// 
    /// </summary>
    public class CheckListBoxDragEndCommandBehavior : CommandBehaviorBase<CheckListBox>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		protected virtual void OnEventRaised(object sender, DragCheckLixtBoxEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.DragEnd += OnEventRaised;
        }
    }

	// CheckListBoxDragEndCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CheckListBoxDragEndCommandBehavior<T> : CheckListBoxDragEndCommandBehavior
    { }
	#endregion
}


