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

    #region RibbonCheckBoxCheckedCommand
    /// <summary>
    /// RibbonCheckBoxCheckedCommand
    /// </summary>
    public class RibbonCheckBoxCheckedCommand : ControlCommandBase<RibbonCheckBoxCheckedCommandBehavior, RibbonCheckBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class RibbonCheckBoxCheckedCommandBehavior : CommandBehaviorBase<RibbonCheckBox>
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
            TargetObject.Checked += OnEventRaised;
        }
    }

    /// <summary>
    /// RibbonCheckBoxCheckedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RibbonCheckBoxCheckedCommandBehavior<T> : RibbonCheckBoxCheckedCommandBehavior
    { }
    #endregion

    #region RibbonCheckBoxUnCheckedCommand
    /// <summary>
    /// RibbonCheckBoxUnCheckedCommand
    /// </summary>
    public class RibbonCheckBoxUnCheckedCommand : ControlCommandBase<RibbonCheckBoxUnCheckedCommandBehavior, RibbonCheckBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class RibbonCheckBoxUnCheckedCommandBehavior : CommandBehaviorBase<RibbonCheckBox>
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
            TargetObject.Unchecked+= OnEventRaised;
        }
    }

    /// <summary>
    /// RibbonCheckBoxUnCheckedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RibbonCheckBoxUnCheckedCommandBehavior<T> : RibbonCheckBoxUnCheckedCommandBehavior
    { }
    #endregion

    #region RibbonCheckBoxIndeterminateCommand
    /// <summary>
    /// RibbonCheckBoxIndeterminateCommand
    /// </summary>
    public class RibbonCheckBoxIndeterminateCommand : ControlCommandBase<RibbonCheckBoxIndeterminateCommandBehavior, RibbonCheckBox>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class RibbonCheckBoxIndeterminateCommandBehavior : CommandBehaviorBase<RibbonCheckBox>
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
            TargetObject.Indeterminate += OnEventRaised;
        }
    }

    /// <summary>
    /// RibbonCheckBoxIndeterminateCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RibbonCheckBoxIndeterminateCommandBehavior<T> : RibbonCheckBoxIndeterminateCommandBehavior
    { }
    #endregion
}


