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

    #region RibbonRadioButtonCheckedCommand
    /// <summary>
    /// RibbonRadioButtonCheckedCommand
    /// </summary>
    public class RibbonRadioButtonCheckedCommand : ControlCommandBase<RibbonRadioButtonCheckedCommandBehavior, RibbonRadioButton>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class RibbonRadioButtonCheckedCommandBehavior : CommandBehaviorBase<RibbonRadioButton>
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
    /// RibbonRadioButtonCheckedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RibbonRadioButtonCheckedCommandBehavior<T> : RibbonRadioButtonCheckedCommandBehavior
    { }
    #endregion

    #region RibbonRadioButtonUnCheckedCommand
    /// <summary>
    /// RibbonRadioButtonUnCheckedCommand
    /// </summary>
    public class RibbonRadioButtonUnCheckedCommand : ControlCommandBase<RibbonRadioButtonUnCheckedCommandBehavior, RibbonRadioButton>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class RibbonRadioButtonUnCheckedCommandBehavior : CommandBehaviorBase<RibbonRadioButton>
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
    /// RibbonRadioButtonUnCheckedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RibbonRadioButtonUnCheckedCommandBehavior<T> : RibbonRadioButtonUnCheckedCommandBehavior
    { }
    #endregion

    #region RibbonRadioButtonIndeterminateCommand
    /// <summary>
    /// RibbonRadioButtonIndeterminateCommand
    /// </summary>
    public class RibbonRadioButtonIndeterminateCommand : ControlCommandBase<RibbonRadioButtonIndeterminateCommandBehavior, RibbonRadioButton>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class RibbonRadioButtonIndeterminateCommandBehavior : CommandBehaviorBase<RibbonRadioButton>
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
    /// RibbonRadioButtonIndeterminateCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RibbonRadioButtonIndeterminateCommandBehavior<T> : RibbonRadioButtonIndeterminateCommandBehavior
    { }
    #endregion
}


