#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Tools.Controls;

namespace Syncfusion.Windows.Tools.MVVM
{

    #region HtmlHostUrlSourceChangedCommand
    // HtmlHostUrlSourceChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class HtmlHostUrlSourceChangedCommand : ControlCommandBase<HtmlHostUrlSourceChangedCommandBehavior, HtmlHost>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class HtmlHostUrlSourceChangedCommandBehavior : CommandBehaviorBase<HtmlHost>
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
            TargetObject.UrlSourceChanged += OnEventRaised;
        }
    }

    // HtmlHostUrlSourceChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class HtmlHostUrlSourceChangedCommandBehavior<T> : HtmlHostUrlSourceChangedCommandBehavior
    { }
    #endregion

    #region HtmlHostHtmlSourceChangedCommand
    // HtmlHostHtmlSourceChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class HtmlHostHtmlSourceChangedCommand : ControlCommandBase<HtmlHostHtmlSourceChangedCommandBehavior, HtmlHost>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class HtmlHostHtmlSourceChangedCommandBehavior : CommandBehaviorBase<HtmlHost>
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
            TargetObject.HtmlSourceChanged += OnEventRaised;
        }
    }

    // HtmlHostHtmlSourceChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class HtmlHostHtmlSourceChangedCommandBehavior<T> : HtmlHostHtmlSourceChangedCommandBehavior
    { }
    #endregion

    #region HtmlHostFrameOpacityChangedCommand
    // HtmlHostFrameOpacityChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class HtmlHostFrameOpacityChangedCommand : ControlCommandBase<HtmlHostFrameOpacityChangedCommandBehavior, HtmlHost>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class HtmlHostFrameOpacityChangedCommandBehavior : CommandBehaviorBase<HtmlHost>
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
            TargetObject.FrameOpacityChanged += OnEventRaised;
        }
    }

    // HtmlHostFrameOpacityChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class HtmlHostFrameOpacityChangedCommandBehavior<T> : HtmlHostFrameOpacityChangedCommandBehavior
    { }
    #endregion
}
