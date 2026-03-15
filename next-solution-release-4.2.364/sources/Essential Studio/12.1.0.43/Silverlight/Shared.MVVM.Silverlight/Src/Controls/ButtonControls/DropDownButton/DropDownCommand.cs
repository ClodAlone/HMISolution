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

    #region DropDownIsOpenChangedCommand
    /// <summary>
    /// DropDownIsOpenChangedCommand
    /// </summary>
    public class DropDownIsOpenChangedCommand : ControlCommandBase<DropDownIsOpenChangedCommandBehavior, DropDown>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class DropDownIsOpenChangedCommandBehavior : CommandBehaviorBase<DropDown>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, EventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.IsOpenChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// DropDownIsOpenChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DropDownIsOpenChangedCommandBehavior<T> : DropDownIsOpenChangedCommandBehavior
    { }
    #endregion

    #region DropDownOutsideRectMouseMoveCommand
    /// <summary>
    /// DropDownOutsideRectMouseMoveCommand
    /// </summary>
    public class DropDownOutsideRectMouseMoveCommand : ControlCommandBase<DropDownOutsideRectMouseMoveCommandBehavior, DropDown>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class DropDownOutsideRectMouseMoveCommandBehavior : CommandBehaviorBase<DropDown>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, MouseEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.OutsideRectMouseMove += OnEventRaised;
        }
    }

    /// <summary>
    /// DropDownOutsideRectMouseMoveCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DropDownOutsideRectMouseMoveCommandBehavior<T> : DropDownOutsideRectMouseMoveCommandBehavior
    { }
    #endregion

    #region DropDownOutsideRectMouseDownCommand
    /// <summary>
    /// DropDownOutsideRectMouseDownCommand
    /// </summary>
    public class DropDownOutsideRectMouseDownCommand : ControlCommandBase<DropDownOutsideRectMouseDownCommandBehavior, DropDown>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class DropDownOutsideRectMouseDownCommandBehavior : CommandBehaviorBase<DropDown>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, MouseButtonEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.OutsideRectMouseDown += OnEventRaised;
        }
    }

    /// <summary>
    /// DropDownOutsideRectMouseDownCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DropDownOutsideRectMouseDownCommandBehavior<T> : DropDownOutsideRectMouseDownCommandBehavior
    { }
    #endregion
}


