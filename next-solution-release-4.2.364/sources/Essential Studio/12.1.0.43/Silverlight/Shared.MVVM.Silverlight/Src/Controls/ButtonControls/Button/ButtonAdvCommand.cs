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

    #region ButtonAdvCheckedCommand
    
    /// <summary>
    /// ButtonAdvCheckedCommand
    /// </summary>
    public class ButtonAdvCheckedCommand : ControlCommandBase<ButtonAdvCheckedCommandBehavior, ButtonAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class ButtonAdvCheckedCommandBehavior : CommandBehaviorBase<ButtonAdv>
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
            TargetObject.Checked += OnEventRaised;
        }
    }

    
    /// <summary>
    /// ButtonAdvCheckedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ButtonAdvCheckedCommandBehavior<T> : ButtonAdvCheckedCommandBehavior
    { }
    #endregion


    #region ButtonAdvClickCommand
    
    /// <summary>
    /// ButtonAdvClickCommand
    /// </summary>
    public class ButtonAdvClickCommand : ControlCommandBase<ButtonAdvClickCommandBehavior, ButtonAdv>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class ButtonAdvClickCommandBehavior : CommandBehaviorBase<ButtonAdv>
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
            TargetObject.Click+= OnEventRaised;
        }
    }

    
    /// <summary>
    /// ButtonAdvClickCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ButtonAdvClickCommandBehavior<T> : ButtonAdvClickCommandBehavior
    { }
    #endregion
}


