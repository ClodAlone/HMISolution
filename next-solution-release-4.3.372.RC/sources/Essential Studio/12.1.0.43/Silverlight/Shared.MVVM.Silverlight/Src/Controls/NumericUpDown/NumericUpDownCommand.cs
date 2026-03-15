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
    #region NumericUpDownValueChangedCommand
    /// <summary>
    /// NumericUpDownValueChangedCommand
    /// </summary>
    public class NumericUpDownValueChangedCommand : ControlCommandBase<NumericUpDownValueChangedCommandBehavior, Syncfusion.Windows.Controls.NumericUpDown>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class NumericUpDownValueChangedCommandBehavior : CommandBehaviorBase<Syncfusion.Windows.Controls.NumericUpDown>
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
            TargetObject.ValueChanged += OnEventRaised;
        }
    }

    /// <summary>
    /// NumericUpDownValueChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NumericUpDownValueChangedCommandBehavior<T> : NumericUpDownValueChangedCommandBehavior
    { }
    #endregion


    #region NumericUpDownValueChangingCommand
    /// <summary>
    /// NumericUpDownValueChangingCommand
    /// </summary>
    public class NumericUpDownValueChangingCommand : ControlCommandBase<NumericUpDownValueChangingCommandBehavior, Syncfusion.Windows.Controls.NumericUpDown>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class NumericUpDownValueChangingCommandBehavior : CommandBehaviorBase<Syncfusion.Windows.Controls.NumericUpDown>
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
            TargetObject.ValueChanging += OnEventRaised;
        }
    }

    /// <summary>
    /// NumericUpDownValueChangingCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NumericUpDownValueChangingCommandBehavior<T> : NumericUpDownValueChangingCommandBehavior
    { }
    #endregion


    #region NumericUpDownParsingCommand
    /// <summary>
    /// NumericUpDownParsingCommand
    /// </summary>
    public class NumericUpDownParsingCommand : ControlCommandBase<NumericUpDownParsingCommandBehavior, Syncfusion.Windows.Controls.NumericUpDown>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class NumericUpDownParsingCommandBehavior : CommandBehaviorBase<Syncfusion.Windows.Controls.NumericUpDown>
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
            TargetObject.Parsing += OnEventRaised;
        }
    }

    /// <summary>
    /// NumericUpDownParsingCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NumericUpDownParsingCommandBehavior<T> : NumericUpDownParsingCommandBehavior
    { }
    #endregion


    #region NumericUpDownParseErrorCommand
    /// <summary>
    /// NumericUpDownParseErrorCommand
    /// </summary>
    public class NumericUpDownParseErrorCommand : ControlCommandBase<NumericUpDownParseErrorCommandBehavior, Syncfusion.Windows.Controls.NumericUpDown>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class NumericUpDownParseErrorCommandBehavior : CommandBehaviorBase<Syncfusion.Windows.Controls.NumericUpDown>
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
            TargetObject.ParseError += OnEventRaised;
        }
    }

    /// <summary>
    /// NumericUpDownParseErrorCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NumericUpDownParseErrorCommandBehavior<T> : NumericUpDownParseErrorCommandBehavior
    { }
    #endregion






}

