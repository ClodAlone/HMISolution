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

    #region RibbonDropDownButtonDropDownOpeningCommand
    /// <summary>
    /// RibbonDropDownButtonDropDownOpeningCommand
    /// </summary>
    public class RibbonDropDownButtonDropDownOpeningCommand : ControlCommandBase<RibbonDropDownButtonDropDownOpeningCommandBehavior,RibbonDropDownButton>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class RibbonDropDownButtonDropDownOpeningCommandBehavior : CommandBehaviorBase<RibbonDropDownButton>
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
            TargetObject.DropDownOpening += OnEventRaised;
        }
    }

    /// <summary>
    /// RibbonDropDownButtonDropDownOpeningCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RibbonDropDownButtonDropDownOpeningCommandBehavior<T> : RibbonDropDownButtonDropDownOpeningCommandBehavior
    { }
    #endregion

    #region RibbonDropDownButtonDropDownOpenedCommand
    /// <summary>
    /// RibbonDropDownButtonDropDownOpenedCommand
    /// </summary>
    public class RibbonDropDownButtonDropDownOpenedCommand : ControlCommandBase<RibbonDropDownButtonDropDownOpenedCommandBehavior, RibbonDropDownButton>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class RibbonDropDownButtonDropDownOpenedCommandBehavior : CommandBehaviorBase<RibbonDropDownButton>
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
            TargetObject.DropDownOpened += OnEventRaised;
        }
    }

    /// <summary>
    /// RibbonDropDownButtonDropDownOpenedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RibbonDropDownButtonDropDownOpenedCommandBehavior<T> : RibbonDropDownButtonDropDownOpenedCommandBehavior
    { }
    #endregion

    #region RibbonDropDownButtonDropDownClosingCommand
    /// <summary>
    /// RibbonDropDownButtonDropDownClosingCommand
    /// </summary>
    public class RibbonDropDownButtonDropDownClosingCommand : ControlCommandBase<RibbonDropDownButtonDropDownClosingCommandBehavior, RibbonDropDownButton>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class RibbonDropDownButtonDropDownClosingCommandBehavior : CommandBehaviorBase<RibbonDropDownButton>
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
            TargetObject.DropDownClosing += OnEventRaised;
        }
    }

    /// <summary>
    /// RibbonDropDownButtonDropDownClosingCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RibbonDropDownButtonDropDownClosingCommandBehavior<T> : RibbonDropDownButtonDropDownClosingCommandBehavior
    { }
    #endregion

    #region RibbonDropDownButtonDropDownClosedCommand
    /// <summary>
    /// RibbonDropDownButtonDropDownClosedCommand
    /// </summary>
    public class RibbonDropDownButtonDropDownClosedCommand : ControlCommandBase<RibbonDropDownButtonDropDownClosedCommandBehavior, RibbonDropDownButton>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class RibbonDropDownButtonDropDownClosedCommandBehavior : CommandBehaviorBase<RibbonDropDownButton>
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
            TargetObject.DropDownClosed += OnEventRaised;
        }
    }

    /// <summary>
    /// RibbonDropDownButtonDropDownClosedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RibbonDropDownButtonDropDownClosedCommandBehavior<T> : RibbonDropDownButtonDropDownClosedCommandBehavior
    { }
    #endregion
}


