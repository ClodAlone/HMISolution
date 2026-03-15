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

    #region TabButtonCaptionChangedCommand
    /// <summary>
    /// TabButtonCaptionChangedCommand
    /// </summary>
    public class TabButtonCaptionChangedCommand : ControlCommandBase<TabButtonCaptionChangedCommandBehavior, TabButton>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class TabButtonCaptionChangedCommandBehavior : CommandBehaviorBase<TabButton>
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
            TargetObject.CaptionChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// TabButtonCaptionChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TabButtonCaptionChangedCommandBehavior<T> : TabButtonCaptionChangedCommandBehavior
    { }
    #endregion

    #region TabButtonSeparatorOpacityChangedCommand
    /// <summary>
    /// TabButtonSeparatorOpacityChangedCommand
    /// </summary>
    public class TabButtonSeparatorOpacityChangedCommand : ControlCommandBase<TabButtonSeparatorOpacityChangedCommandBehavior, TabButton>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class TabButtonSeparatorOpacityChangedCommandBehavior : CommandBehaviorBase<TabButton>
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
            TargetObject.SeparatorOpacityChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// TabButtonSeparatorOpacityChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TabButtonSeparatorOpacityChangedCommandBehavior<T> : TabButtonSeparatorOpacityChangedCommandBehavior
    { }
    #endregion
}


