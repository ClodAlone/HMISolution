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

    #region RangeSliderRangeChangedCommand
    // RangeSliderRangeChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class RangeSliderRangeChangedCommand : ControlCommandBase<RangeSliderRangeChangedCommandBehavior, RangeSlider>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class RangeSliderRangeChangedCommandBehavior : CommandBehaviorBase<RangeSlider>
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
            TargetObject.RangeChanged += OnEventRaised;
        }
    }

    // RangeSliderRangeChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class RangeSliderRangeChangedCommandBehavior<T> : RangeSliderRangeChangedCommandBehavior
    { }
    #endregion

    #region RangeSliderValueChangedCommand
    // RangeSliderValueChangedCommand
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class RangeSliderValueChangedCommand : ControlCommandBase<RangeSliderValueChangedCommandBehavior, RangeSlider>
    { }

    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class RangeSliderValueChangedCommandBehavior : CommandBehaviorBase<RangeSlider>
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
            TargetObject.ValueChanged += OnEventRaised;
        }
    }

    // RangeSliderValueChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
    public class RangeSliderValueChangedCommandBehavior<T> : RangeSliderValueChangedCommandBehavior
    { }
    #endregion
}

