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

    #region CarouselSelectionChangedCommand
    /// <summary>
    /// CarouselSelectionChangedCommand
    /// </summary>
    public class CarouselSelectionChangedCommand : ControlCommandBase<CarouselSelectionChangedCommandBehavior, Carousel>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class CarouselSelectionChangedCommandBehavior : CommandBehaviorBase<Carousel>
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
            TargetObject.SelectionChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// CarouselSelectionChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CarouselSelectionChangedCommandBehavior<T> : CarouselSelectionChangedCommandBehavior
    { }
    #endregion

    #region CarouselSelectedIndexChangedCommand
    
    /// <summary>
    /// CarouselSelectedIndexChangedCommand
    /// </summary>
    public class CarouselSelectedIndexChangedCommand : ControlCommandBase<CarouselSelectedIndexChangedCommandBehavior, Carousel>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class CarouselSelectedIndexChangedCommandBehavior : CommandBehaviorBase<Carousel>
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
            TargetObject.SelectedIndexChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// CarouselSelectedIndexChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CarouselSelectedIndexChangedCommandBehavior<T> : CarouselSelectedIndexChangedCommandBehavior
    { }
    #endregion

    #region CarouselSelectedValueChangedCommand
    // CarouselSelectedValueChangedCommand
    /// <summary>
    /// 
    /// </summary>
    public class CarouselSelectedValueChangedCommand : ControlCommandBase<CarouselSelectedValueChangedCommandBehavior, Carousel>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class CarouselSelectedValueChangedCommandBehavior : CommandBehaviorBase<Carousel>
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
            TargetObject.SelectedValueChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// CarouselSelectedValueChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CarouselSelectedValueChangedCommandBehavior<T> : CarouselSelectedValueChangedCommandBehavior
    { }
    #endregion
}


